using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal abstract class PartitionedTreeScalabilityCache : BaseScalabilityCache
	{
		private long m_cacheFreeableBytes;

		private LinkedBucketedQueue<BaseReference> m_serializationQueue;

		private LinkedBucketedQueue<BaseReference> m_pinnedItems;

		private LinkedLRUCache<ItemHolder> m_cachePriority;

		private SegmentedDictionary<ReferenceID, BaseReference> m_cacheLookup;

		private long m_nextId = -1L;

		private TreePartitionManager m_partitionManager;

		private bool m_lockedDownForFlush;

		public abstract override ScalabilityCacheType CacheType
		{
			get;
		}

		protected sealed override long InternalFreeableBytes
		{
			get
			{
				return this.m_cacheFreeableBytes;
			}
		}

		internal long CacheSizeBytes
		{
			get
			{
				return base.m_cacheSizeBytes;
			}
		}

		internal long CacheFreeableBytes
		{
			get
			{
				return this.m_cacheFreeableBytes;
			}
		}

		internal long CacheCapacityBytes
		{
			get
			{
				return base.m_cacheCapacityBytes;
			}
			set
			{
				base.m_cacheCapacityBytes = value;
			}
		}

		public PartitionedTreeScalabilityCache(TreePartitionManager partitionManager, IStorage storage, long cacheExpansionIntervalMs, double cacheExpansionRatio, long minReservedMemoryBytes)
			: base(storage, cacheExpansionIntervalMs, cacheExpansionRatio, ComponentType.Processing, minReservedMemoryBytes)
		{
			this.m_serializationQueue = new LinkedBucketedQueue<BaseReference>(100);
			this.m_cachePriority = new LinkedLRUCache<ItemHolder>();
			this.m_cacheFreeableBytes = 0L;
			this.m_partitionManager = partitionManager;
		}

		public sealed override IReference<T> Allocate<T>(T obj, int priority)
		{
			Global.Tracer.Assert(false, "Allocate should not be used on PartitionedTreeScalabilityCache.  Use AllocateAndPin instead.");
			return null;
		}

		public sealed override IReference<T> Allocate<T>(T obj, int priority, int initialSize)
		{
			Global.Tracer.Assert(false, "Allocate should not be used on PartitionedTreeScalabilityCache.  Use AllocateAndPin instead.");
			return null;
		}

		public sealed override IReference<T> AllocateAndPin<T>(T obj, int priority)
		{
			return (IReference<T>)this.AllocateAndPin((IStorable)(object)obj, ItemSizes.SizeOf((IStorable)(object)obj));
		}

		public sealed override IReference<T> AllocateAndPin<T>(T obj, int priority, int initialSize)
		{
			return (IReference<T>)this.AllocateAndPin((IStorable)(object)obj, initialSize);
		}

		protected BaseReference AllocateAndPin(IStorable obj, int initialSize)
		{
			Global.Tracer.Assert(obj != null, "Cannot allocate reference to null");
			BaseReference baseReference = base.CreateReference(obj);
			baseReference.Init(this, this.GenerateTempId());
			this.CacheItem(baseReference, obj, false, initialSize);
			baseReference.PinValue();
			return baseReference;
		}

		public sealed override IReference<T> GenerateFixedReference<T>(T obj)
		{
			BaseReference baseReference = base.CreateReference((IStorable)(object)obj);
			baseReference.Init(this, this.GenerateTempId());
			ItemHolder itemHolder = new ItemHolder();
			itemHolder.Reference = baseReference;
			itemHolder.Item = (IStorable)(object)obj;
			baseReference.Item = itemHolder;
			baseReference.InQueue = InQueueState.InQueue;
			return (IReference<T>)baseReference;
		}

		public sealed override int StoreStaticReference(object item)
		{
			Global.Tracer.Assert(false, "Static references are not supported in the PartitionedTreeScalabilityCache");
			return -1;
		}

		public sealed override object FetchStaticReference(int id)
		{
			Global.Tracer.Assert(false, "Static references are not supported in the PartitionedTreeScalabilityCache");
			return null;
		}

		public sealed override IReference PoolReference(IReference reference)
		{
			BaseReference baseReference = default(BaseReference);
			if (this.CacheTryGetValue(reference.Id, out baseReference))
			{
				reference = baseReference;
			}
			return reference;
		}

		internal IReference<T> AllocateEmptyTreePartition<T>(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType referenceObjectType)
		{
			BaseReference baseReference = default(BaseReference);
			if (!base.m_referenceCreator.TryCreateReference(referenceObjectType, out baseReference))
			{
				Global.Tracer.Assert(false, "Cannot create reference of type: {0}", referenceObjectType);
			}
			baseReference.Init(this, this.m_partitionManager.AllocateNewTreePartition());
			return (IReference<T>)baseReference;
		}

		internal void SetTreePartitionContentsAndPin<T>(IReference<T> emptyPartitionRef, T contents) where T : IStorable
		{
			BaseReference baseReference = (BaseReference)emptyPartitionRef;
			this.m_partitionManager.TreeHasChanged = true;
			this.CacheItem(baseReference, (IStorable)(object)contents, false, ItemSizes.SizeOf((IStorable)(object)contents));
			baseReference.PinValue();
			this.CacheSetValue(baseReference.Id, baseReference);
		}

		internal void Flush()
		{
			foreach (BaseReference item in this.m_serializationQueue)
			{
				this.WriteItem(item);
			}
			this.m_serializationQueue.Clear();
			this.m_cachePriority.Clear();
			this.m_cacheLookup = null;
			base.m_cacheCapacityBytes = 0L;
			this.m_cacheFreeableBytes = 0L;
			base.m_cacheSizeBytes = 0L;
			base.m_storage.Flush();
		}

		internal void PrepareForFlush()
		{
			this.m_cachePriority.Clear();
			this.m_cacheLookup = null;
			this.m_cacheFreeableBytes = 0L;
			base.m_pendingFreeBytes = 0L;
			this.m_lockedDownForFlush = true;
		}

		public sealed override void Dispose()
		{
			this.m_cacheLookup = null;
			this.m_cachePriority = null;
			this.m_serializationQueue = null;
			this.m_pinnedItems = null;
			base.Dispose();
		}

		internal override BaseReference TransferTo(BaseReference reference)
		{
			Global.Tracer.Assert(false, "PartitionedTreeScalabilityCache does not support the TransferTo operation");
			return null;
		}

		internal sealed override void Free(BaseReference reference)
		{
			Global.Tracer.Assert(false, "PartitionedTreeScalabilityCache does not support Free");
		}

		internal sealed override IStorable Retrieve(BaseReference reference)
		{
			if (reference.Item == null)
			{
				ReferenceID id = reference.Id;
				BaseReference baseReference = default(BaseReference);
				if (this.CacheTryGetValue(id, out baseReference) && baseReference.Item != null)
				{
					IStorable item = baseReference.Item.Item;
					this.CacheItem(reference, item, true, ItemSizes.SizeOf(item));
				}
				else
				{
					this.LoadItem(reference);
				}
			}
			IStorable result = null;
			if (reference.Item != null)
			{
				result = reference.Item.Item;
			}
			return result;
		}

		internal sealed override void ReferenceValueCallback(BaseReference reference)
		{
			if (reference.InQueue == InQueueState.Exempt)
			{
				this.m_cachePriority.Bump(reference.Item);
			}
			base.ReferenceValueCallback(reference);
		}

		internal sealed override void Pin(BaseReference reference)
		{
			this.Retrieve(reference);
		}

		internal sealed override void UnPin(BaseReference reference)
		{
			if (reference.PinCount == 0 && (reference.Id.IsTemporary || reference.Id.HasMultiPart) && reference.InQueue == InQueueState.None)
			{
				reference.InQueue = InQueueState.InQueue;
				this.m_serializationQueue.Enqueue(reference);
				if (!this.m_lockedDownForFlush)
				{
					ItemHolder item = reference.Item;
					IStorable obj = null;
					if (item != null)
					{
						obj = item.Item;
					}
					this.m_cacheFreeableBytes += ItemSizes.SizeOf(obj);
				}
			}
			if (!this.m_lockedDownForFlush)
			{
				base.PeriodicOperationCheck();
			}
		}

		internal sealed override void ReferenceSerializeCallback(BaseReference reference)
		{
		}

		internal sealed override void UpdateTargetSize(BaseReference reference, int sizeDeltaBytes)
		{
			base.m_cacheSizeBytes += sizeDeltaBytes;
			base.m_totalAuditedBytes += sizeDeltaBytes;
			if (!reference.Id.IsTemporary)
			{
				this.m_cacheFreeableBytes += sizeDeltaBytes;
			}
		}

		internal bool CacheTryGetValue(ReferenceID id, out BaseReference item)
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

		internal void CacheSetValue(ReferenceID id, BaseReference value)
		{
			if (this.m_cacheLookup == null)
			{
				this.m_cacheLookup = new SegmentedDictionary<ReferenceID, BaseReference>(503, 17, ReferenceIDEqualityComparer.Instance);
			}
			this.m_cacheLookup[id] = value;
		}

		private IStorable LoadItem(BaseReference reference)
		{
			if (base.m_inStreamOper)
			{
				Global.Tracer.Assert(false, "PartitionedTreeScalabilityCache should not Load during serialization");
			}
			IStorable storable = null;
			try
			{
				base.m_inStreamOper = true;
				base.m_deserializationTimer.Start();
				ReferenceID id = reference.Id;
				long num;
				if (!id.IsTemporary && id.HasMultiPart)
				{
					num = this.m_partitionManager.GetTreePartitionOffset(id);
					if (num < 0)
					{
						return null;
					}
				}
				else
				{
					num = reference.Id.Value;
				}
				if (num < 0)
				{
					Global.Tracer.Assert(false, "Invalid offset for item.  ReferenceID: {0}, Offset: {1}", id, num);
				}
				long num2 = default(long);
				storable = (IStorable)base.m_storage.Retrieve(num, out num2);
			}
			finally
			{
				base.m_inStreamOper = false;
				base.m_deserializationTimer.Stop();
			}
			this.CacheItem(reference, storable, true, ItemSizes.SizeOf(storable));
			return storable;
		}

		private void CacheItem(BaseReference reference, IStorable item, bool fromDeserialize, int newItemSize)
		{
			ItemHolder itemHolder = new ItemHolder();
			itemHolder.Reference = reference;
			itemHolder.Item = item;
			reference.Item = itemHolder;
			base.FreeCacheSpace(newItemSize, this.m_cacheFreeableBytes);
			if (fromDeserialize)
			{
				this.CacheSetValue(reference.Id, reference);
				this.m_cacheFreeableBytes += newItemSize;
			}
			else
			{
				base.m_totalAuditedBytes += newItemSize;
			}
			base.m_cacheSizeBytes += newItemSize;
			this.EnqueueItem(reference);
		}

		private void EnqueueItem(BaseReference itemRef)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType objectType = itemRef.GetObjectType();
			if (objectType == AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInstanceReference)
			{
				if (this.m_pinnedItems == null)
				{
					this.m_pinnedItems = new LinkedBucketedQueue<BaseReference>(25);
				}
				this.m_pinnedItems.Enqueue(itemRef);
			}
			else
			{
				ReferenceID id = itemRef.Id;
				if (!id.IsTemporary)
				{
					if (id.HasMultiPart && this.m_partitionManager.GetTreePartitionOffset(id) == TreePartitionManager.EmptyTreePartitionOffset)
					{
						return;
					}
					this.m_cachePriority.Add(itemRef.Item);
					itemRef.InQueue = InQueueState.Exempt;
				}
			}
		}

		protected sealed override void FulfillInProgressFree()
		{
			int num = this.m_cachePriority.Count;
			while (base.m_inProgressFreeBytes > 0 && num > 0)
			{
				num--;
				ItemHolder itemHolder = this.m_cachePriority.Peek();
				BaseReference reference = itemHolder.Reference;
				if (reference.PinCount == 0)
				{
					this.m_cachePriority.ExtractLRU();
					reference.InQueue = InQueueState.None;
					if (itemHolder.Item != null)
					{
						this.UpdateStatsForRemovedItem(reference, ref base.m_inProgressFreeBytes);
						this.CacheRemoveValue(reference.Id);
						itemHolder.Item = null;
						itemHolder.Reference = null;
						reference.Item = null;
					}
				}
				else
				{
					this.m_cachePriority.Bump(itemHolder);
				}
			}
			if (base.m_inProgressFreeBytes > 0)
			{
				using (IDecumulator<BaseReference> decumulator = this.m_serializationQueue.GetDecumulator())
				{
					while (base.m_inProgressFreeBytes > 0 && decumulator.MoveNext())
					{
						BaseReference current = decumulator.Current;
						decumulator.RemoveCurrent();
						if (current.Item != null)
						{
							if (current.PinCount == 0)
							{
								this.UpdateStatsForRemovedItem(current, ref base.m_inProgressFreeBytes);
							}
							this.WriteItem(current);
							if (current.PinCount > 0)
							{
								this.EnqueueItem(current);
								this.CacheSetValue(current.Id, current);
							}
						}
					}
				}
			}
		}

		private void UpdateStatsForRemovedItem(BaseReference itemRef, ref long bytesToFree)
		{
			long num = ItemSizes.SizeOf(itemRef.Item.Item);
			long num2 = base.m_cacheSizeBytes - num;
			long num3 = this.m_cacheFreeableBytes - num;
			if (num3 < 0)
			{
				num3 = 0L;
			}
			if (num2 < num3)
			{
				num2 = num3;
			}
			this.m_cacheFreeableBytes = num3;
			base.m_cacheSizeBytes = num2;
			bytesToFree -= num;
		}

		private void WriteItem(BaseReference itemRef)
		{
			ItemHolder item = itemRef.Item;
			IStorable item2 = item.Item;
			ReferenceID id = itemRef.Id;
			long num = base.m_storage.Allocate(item2);
			if (id.HasMultiPart && !id.IsTemporary)
			{
				this.m_partitionManager.UpdateTreePartitionOffset(id, num);
				if (itemRef.PinCount == 0)
				{
					this.CacheRemoveValue(id);
				}
			}
			else
			{
				id = new ReferenceID(num);
				id.IsTemporary = false;
				id.HasMultiPart = false;
				itemRef.Id = id;
			}
			if (itemRef.PinCount == 0)
			{
				item.Item = null;
				item.Reference = null;
				itemRef.Item = null;
			}
		}

		private ReferenceID GenerateTempId()
		{
			long nextId;
			this.m_nextId = (nextId = this.m_nextId) - 1;
			return new ReferenceID(nextId);
		}
	}
}
