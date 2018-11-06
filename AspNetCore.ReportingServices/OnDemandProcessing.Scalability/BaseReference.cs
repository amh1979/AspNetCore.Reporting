using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal abstract class BaseReference : IReference, IStorable, IPersistable, IDisposable
	{
		private ReferenceID m_id;

		[NonSerialized]
		protected BaseScalabilityCache m_scalabilityCache;

		[NonSerialized]
		internal ItemHolder Item;

		[NonSerialized]
		private int m_pinCount;

		[NonSerialized]
		private static readonly Declaration m_declaration = BaseReference.GetDeclaration();

		public ReferenceID Id
		{
			get
			{
				return this.m_id;
			}
			set
			{
				this.m_id = value;
			}
		}

		internal int PinCount
		{
			get
			{
				return this.m_pinCount;
			}
			set
			{
				this.m_pinCount = value;
			}
		}

		internal BaseScalabilityCache ScalabilityCache
		{
			get
			{
				return this.m_scalabilityCache;
			}
		}

		internal InQueueState InQueue
		{
			get
			{
				if (this.Item != null)
				{
					return this.Item.InQueue;
				}
				return InQueueState.None;
			}
			set
			{
				this.Item.InQueue = value;
			}
		}

		public int Size
		{
			get
			{
				int num = 16 + ItemSizes.ReferenceSize + 4 + ItemSizes.ReferenceSize;
				if (this.Item != null)
				{
					num += this.Item.ComputeSizeForReference();
				}
				return num;
			}
		}

		internal void Init(BaseScalabilityCache storageManager)
		{
			this.SetScalabilityCache(storageManager);
		}

		internal void Init(BaseScalabilityCache storageManager, ReferenceID id)
		{
			this.SetScalabilityCache(storageManager);
			this.m_id = id;
		}

		public IReference TransferTo(IScalabilityCache scaleCache)
		{
			return ((BaseScalabilityCache)scaleCache).TransferTo(this);
		}

		public IDisposable PinValue()
		{
			this.m_pinCount++;
			this.m_scalabilityCache.Pin(this);
			return this;
		}

		public void UnPinValue()
		{
			this.m_pinCount--;
			this.m_scalabilityCache.UnPin(this);
		}

		public void Free()
		{
			this.m_scalabilityCache.Free(this);
		}

		public void UpdateSize(int sizeBytesDelta)
		{
			this.m_scalabilityCache.UpdateTargetSize(this, sizeBytesDelta);
		}

		[DebuggerStepThrough]
		internal IStorable InternalValue()
		{
			IStorable result;
			if (this.Item != null)
			{
				result = this.Item.Item;
				this.m_scalabilityCache.ReferenceValueCallback(this);
			}
			else
			{
				result = this.m_scalabilityCache.Retrieve(this);
			}
			return result;
		}

		private void SetScalabilityCache(BaseScalabilityCache cache)
		{
			this.m_scalabilityCache = cache;
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			this.m_scalabilityCache.ReferenceSerializeCallback(this);
			long value = this.m_id.Value;
			if (!object.ReferenceEquals(writer.PersistenceHelper, this.m_scalabilityCache))
			{
				BaseScalabilityCache baseScalabilityCache = writer.PersistenceHelper as BaseScalabilityCache;
				PairObj<ReferenceID, BaseScalabilityCache> item = new PairObj<ReferenceID, BaseScalabilityCache>(this.m_id, this.m_scalabilityCache);
				value = baseScalabilityCache.StoreStaticReference(item);
			}
			writer.RegisterDeclaration(BaseReference.m_declaration);
			while (writer.NextMember())
			{
				if (writer.CurrentMember.MemberName == MemberName.ID)
				{
					writer.Write(value);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(BaseReference.m_declaration);
			long num = 0L;
			while (reader.NextMember())
			{
				if (reader.CurrentMember.MemberName == MemberName.ID)
				{
					num = reader.ReadInt64();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
			BaseScalabilityCache baseScalabilityCache = reader.PersistenceHelper as BaseScalabilityCache;
			ScalabilityCacheType cacheType = baseScalabilityCache.CacheType;
			if (num < 0 && cacheType != ScalabilityCacheType.GroupTree && cacheType != ScalabilityCacheType.Lookup)
			{
				PairObj<ReferenceID, BaseScalabilityCache> pairObj = (PairObj<ReferenceID, BaseScalabilityCache>)baseScalabilityCache.FetchStaticReference((int)num);
				this.m_id = pairObj.First;
				baseScalabilityCache = pairObj.Second;
			}
			else
			{
				this.m_id = new ReferenceID(num);
			}
			this.SetScalabilityCache(baseScalabilityCache);
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public abstract AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType();

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ID, Token.Int64));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Reference, list);
		}

		void IDisposable.Dispose()
		{
			this.UnPinValue();
		}

		public static bool operator ==(BaseReference reference, object obj)
		{
			if (object.ReferenceEquals(reference, obj))
			{
				return true;
			}
			if (object.ReferenceEquals(reference, null))
			{
				return false;
			}
			return reference.Equals(obj);
		}

		public static bool operator !=(BaseReference reference, object obj)
		{
			return !(reference == obj);
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(obj, null))
			{
				return false;
			}
			BaseReference baseReference = obj as BaseReference;
			if (baseReference == (object)null)
			{
				return false;
			}
			return this.m_id == baseReference.m_id;
		}

		public override int GetHashCode()
		{
			return (int)this.m_id.Value;
		}
	}
}
