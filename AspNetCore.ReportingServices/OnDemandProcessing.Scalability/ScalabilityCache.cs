using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class ScalabilityCache : BaseScalabilityCache
	{
		private const long CacheExpansionIntervalMs = 3000L;

		private const double CacheExpansionRatio = 0.2;

		private const int ID_NULL = -2147483647;

		public const int ID_NOREF = -2147483648;

		private LinkedLRUCache<StorageItem> m_cachePriority;

		private SegmentedDictionary<ReferenceID, StorageItem> m_cacheLookup;

		private IIndexStrategy m_offsetMap;

		private Dictionary<int, StaticReferenceHolder> m_staticReferences;

		private Dictionary<object, int> m_staticIdLookup;

		public override ScalabilityCacheType CacheType
		{
			get
			{
				return ScalabilityCacheType.Standard;
			}
		}

		protected override long InternalFreeableBytes
		{
			get
			{
				return base.m_cacheSizeBytes;
			}
		}

		public ScalabilityCache(IStorage storage, IIndexStrategy indexStrategy, ComponentType ownerComponent, long minReservedMemoryBytes)
			: base(storage, 3000L, 0.2, ownerComponent, minReservedMemoryBytes)
		{
			this.m_cachePriority = new LinkedLRUCache<StorageItem>();
			this.m_offsetMap = indexStrategy;
		}

		public override IReference<T> Allocate<T>(T obj, int priority)
		{
			return this.InternalAllocate(obj, priority, false, ItemSizes.SizeOf((IStorable)(object)obj));
		}

		public override IReference<T> Allocate<T>(T obj, int priority, int initialSize)
		{
			return this.InternalAllocate(obj, priority, false, initialSize);
		}

		public override IReference<T> AllocateAndPin<T>(T obj, int priority)
		{
			return this.InternalAllocate(obj, priority, true, ItemSizes.SizeOf((IStorable)(object)obj));
		}

		public override IReference<T> AllocateAndPin<T>(T obj, int priority, int initialSize)
		{
			return this.InternalAllocate(obj, priority, true, initialSize);
		}

		private IReference<T> InternalAllocate<T>(T obj, int priority, bool startPinned, int initialSize) where T : IStorable
		{
			Global.Tracer.Assert(obj != null, "Cannot allocate reference to null");
			BaseReference baseReference = base.CreateReference((IStorable)(object)obj);
			baseReference.Init(this, this.m_offsetMap.GenerateTempId());
			this.CacheItem(baseReference, (IStorable)(object)obj, priority, initialSize);
			if (startPinned)
			{
				baseReference.PinValue();
			}
			return (IReference<T>)baseReference;
		}

		public override void Dispose()
		{
			try
			{
				if (this.m_offsetMap != null)
				{
					this.m_offsetMap.Close();
					this.m_offsetMap = null;
				}
				this.m_cacheLookup = null;
				this.m_cachePriority = null;
				this.m_staticIdLookup = null;
				this.m_staticReferences = null;
			}
			finally
			{
				base.Dispose();
			}
		}

		public override IReference<T> GenerateFixedReference<T>(T obj)
		{
			BaseReference baseReference = base.CreateReference((IStorable)(object)obj);
			baseReference.Init(this, this.m_offsetMap.GenerateTempId());
			StorageItem storageItem = (StorageItem)(baseReference.Item = new StorageItem(baseReference.Id, -1, (IStorable)(object)obj, ItemSizes.SizeOf((IStorable)(object)obj)));
			storageItem.AddReference(baseReference);
			storageItem.InQueue = InQueueState.Exempt;
			storageItem.HasBeenUnPinned = true;
			ISelfReferential selfReferential = ((object)obj) as ISelfReferential;
			if (selfReferential != null)
			{
				selfReferential.SetReference(baseReference);
			}
			return (IReference<T>)baseReference;
		}

		public override int StoreStaticReference(object item)
		{
			int num = default(int);
			if (item == null)
			{
				num = -2147483647;
			}
			else
			{
				IStaticReferenceable staticReferenceable = item as IStaticReferenceable;
				if (staticReferenceable != null)
				{
					num = this.InternalStoreStaticReference(staticReferenceable.ID, item);
					staticReferenceable.SetID(num);
				}
				else
				{
					bool flag = true;
					if (this.m_staticIdLookup == null || !this.m_staticIdLookup.TryGetValue(item, out num))
					{
						num = -2147483648;
						flag = false;
					}
					num = this.InternalStoreStaticReference(num, item);
					if (!flag)
					{
						if (this.m_staticIdLookup == null)
						{
							this.m_staticIdLookup = new Dictionary<object, int>();
						}
						this.m_staticIdLookup[item] = num;
					}
				}
			}
			return num;
		}

		private int InternalStoreStaticReference(int id, object item)
		{
			if (this.m_staticReferences == null)
			{
				this.m_staticReferences = new Dictionary<int, StaticReferenceHolder>();
			}
			int num = id;
			if (id == -2147483648)
			{
				num = (int)this.m_offsetMap.GenerateTempId().Value;
			}
			StaticReferenceHolder staticReferenceHolder = default(StaticReferenceHolder);
			while (this.m_staticReferences.TryGetValue(num, out staticReferenceHolder) && !object.ReferenceEquals(item, staticReferenceHolder.Value))
			{
				num = (int)this.m_offsetMap.GenerateTempId().Value;
			}
			if (staticReferenceHolder != null)
			{
				staticReferenceHolder.RefCount++;
			}
			else
			{
				staticReferenceHolder = new StaticReferenceHolder();
				staticReferenceHolder.Value = item;
				staticReferenceHolder.RefCount = 1;
				this.m_staticReferences[num] = staticReferenceHolder;
			}
			return num;
		}

		public override object FetchStaticReference(int id)
		{
			if (id == -2147483647)
			{
				return null;
			}
			StaticReferenceHolder staticReferenceHolder = default(StaticReferenceHolder);
			object result;
			if (this.m_staticReferences.TryGetValue(id, out staticReferenceHolder))
			{
				result = staticReferenceHolder.Value;
				staticReferenceHolder.RefCount--;
				if (staticReferenceHolder.RefCount <= 0)
				{
					this.m_staticReferences.Remove(id);
				}
			}
			else
			{
				Global.Tracer.Assert(false, "Missing static reference");
				result = null;
			}
			return result;
		}

		public override IReference PoolReference(IReference reference)
		{
			StorageItem storageItem = default(StorageItem);
			if (this.CacheTryGetValue(reference.Id, out storageItem) && storageItem.Reference != (object)null)
			{
				reference = storageItem.Reference;
			}
			return reference;
		}

		internal override void UpdateTargetSize(BaseReference reference, int sizeDeltaBytes)
		{
			StorageItem storageItem = (StorageItem)reference.Item;
			storageItem.UpdateSize(sizeDeltaBytes);
			base.m_cacheSizeBytes += sizeDeltaBytes;
			base.m_totalAuditedBytes += sizeDeltaBytes;
		}

		internal override BaseReference TransferTo(BaseReference reference)
		{
			Global.Tracer.Assert(false, "ScalabilityCache does not support the TransferTo operation");
			return null;
		}

		internal override void ReferenceSerializeCallback(BaseReference reference)
		{
			ReferenceID id = reference.Id;
			if (id.IsTemporary)
			{
				StorageItem storageItem = (StorageItem)reference.Item;
				ReferenceID referenceID = this.m_offsetMap.GenerateId(id);
				if (id != referenceID)
				{
					reference.Id = referenceID;
					storageItem.Id = referenceID;
					this.CacheRemoveValue(id);
				}
				this.CacheSetValue(reference.Id, storageItem);
			}
		}

		internal override void Free(BaseReference reference)
		{
			if (!(reference == (object)null))
			{
				ReferenceID id = reference.Id;
				StorageItem storageItem = default(StorageItem);
				if (this.CacheTryGetValue(id, out storageItem))
				{
					this.CacheRemoveValue(id);
				}
				if (storageItem == null)
				{
					storageItem = (StorageItem)reference.Item;
				}
				if (storageItem != null)
				{
					if (storageItem.InQueue == InQueueState.InQueue)
					{
						this.m_cachePriority.Remove(storageItem);
					}
					int num = ItemSizes.SizeOf(storageItem);
					base.m_cacheSizeBytes -= num;
					base.m_totalAuditedBytes -= num;
					base.m_totalFreedBytes += num;
					base.UpdatePeakCacheSize();
					storageItem.Item = null;
					storageItem.UnlinkReferences(false);
				}
				reference.Item = null;
			}
		}

		internal override IStorable Retrieve(BaseReference reference)
		{
			StorageItem storageItem = default(StorageItem);
			if (!this.CacheTryGetValue(reference.Id, out storageItem))
			{
				storageItem = this.LoadItem(reference);
			}
			base.PeriodicOperationCheck();
			return storageItem.Item;
		}

		internal override void Pin(BaseReference reference)
		{
			StorageItem storageItem = (StorageItem)reference.Item;
			if (storageItem == null)
			{
				if (this.CacheTryGetValue(reference.Id, out storageItem))
				{
					reference.Item = storageItem;
					storageItem.AddReference(reference);
					if (storageItem.InQueue == InQueueState.InQueue)
					{
						this.m_cachePriority.Bump(storageItem);
					}
				}
				else
				{
					storageItem = this.LoadItem(reference);
				}
			}
			else if (storageItem.InQueue == InQueueState.InQueue)
			{
				this.m_cachePriority.Bump(storageItem);
			}
			storageItem.PinCount++;
		}

		internal override void UnPin(BaseReference reference)
		{
			StorageItem storageItem = (StorageItem)reference.Item;
			if (--storageItem.PinCount == 0)
			{
				if (storageItem.InQueue == InQueueState.None)
				{
					this.EnqueueItem(storageItem);
				}
				if (!storageItem.HasBeenUnPinned)
				{
					int num = storageItem.UpdateSize();
					base.m_cacheSizeBytes += num;
					base.m_totalAuditedBytes += num;
					storageItem.HasBeenUnPinned = true;
				}
			}
		}

		internal bool CacheTryGetValue(ReferenceID id, out StorageItem item)
		{
			item = null;
			bool result = false;
			if (this.m_cacheLookup != null)
			{
				result = this.m_cacheLookup.TryGetValue(id, out item);
			}
			return result;
		}

		internal bool CacheRemoveValue(ReferenceID id)
		{
			bool result = false;
			if (this.m_cacheLookup != null)
			{
				result = this.m_cacheLookup.Remove(id);
			}
			return result;
		}

		internal void CacheSetValue(ReferenceID id, StorageItem value)
		{
			if (this.m_cacheLookup == null)
			{
				this.m_cacheLookup = new SegmentedDictionary<ReferenceID, StorageItem>(503, 17, ReferenceIDEqualityComparer.Instance);
			}
			this.m_cacheLookup[id] = value;
		}

		private StorageItem LoadItem(BaseReference reference)
		{
			if (base.m_inStreamOper)
			{
				Global.Tracer.Assert(false, "ScalabilityCache should not Load during serialization");
			}
			StorageItem storageItem = null;
			try
			{
				base.m_inStreamOper = true;
				base.m_deserializationTimer.Start();
				long num = this.m_offsetMap.Retrieve(reference.Id);
				if (num >= 0)
				{
					long persistedSize = default(long);
					storageItem = (StorageItem)base.m_storage.Retrieve(num, out persistedSize);
					storageItem.Offset = num;
					storageItem.PersistedSize = persistedSize;
					storageItem.UpdateSize();
					storageItem.HasBeenUnPinned = true;
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
			finally
			{
				base.m_inStreamOper = false;
				base.m_deserializationTimer.Stop();
			}
			this.CacheItem(reference, storageItem, true);
			return storageItem;
		}

		private void CacheItem(BaseReference reference, StorageItem item, bool fromDeserialize)
		{
			reference.Item = item;
			item.AddReference(reference);
			int num = ItemSizes.SizeOf(item);
			base.FreeCacheSpace(num, base.m_cacheSizeBytes);
			if (fromDeserialize)
			{
				this.CacheSetValue(reference.Id, item);
			}
			else
			{
				base.m_totalAuditedBytes += num;
			}
			base.m_cacheSizeBytes += num;
			this.EnqueueItem(item);
			object item2 = item.Item;
			ISelfReferential selfReferential = item2 as ISelfReferential;
			if (selfReferential != null)
			{
				selfReferential.SetReference(reference);
			}
		}

		private void CacheItem(BaseReference reference, IStorable value, int priority, int initialSize)
		{
			StorageItem item = new StorageItem(reference.Id, priority, value, initialSize);
			this.CacheItem(reference, item, false);
		}

		private void EnqueueItem(StorageItem item)
		{
			this.m_cachePriority.Add(item);
			item.InQueue = InQueueState.InQueue;
		}

		protected override void FulfillInProgressFree()
		{
			base.m_storage.FreezeAllocations = true;
			while (base.m_inProgressFreeBytes > 0 && this.m_cachePriority.Count > 0)
			{
				StorageItem storageItem = this.m_cachePriority.ExtractLRU();
				storageItem.InQueue = InQueueState.None;
				if (storageItem.Item != null && storageItem.PinCount == 0)
				{
					this.CacheRemoveValue(storageItem.Id);
					int num = ItemSizes.SizeOf(storageItem);
					storageItem.Flush(base.m_storage, this.m_offsetMap);
					base.m_cacheSizeBytes -= num;
					if (base.m_cacheSizeBytes < 0)
					{
						base.m_cacheSizeBytes = 0L;
					}
					base.m_inProgressFreeBytes -= num;
					if (base.m_inProgressFreeBytes < 0)
					{
						base.m_inProgressFreeBytes = 0L;
					}
				}
			}
			base.m_storage.FreezeAllocations = false;
		}
	}
}
