using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class StorageItem : ItemHolder, IStorable, IPersistable
	{
		internal int Priority;

		internal ReferenceID Id;

		[NonSerialized]
		internal long PersistedSize;

		[NonSerialized]
		private int m_size;

		[NonSerialized]
		internal int PinCount;

		[NonSerialized]
		internal bool HasBeenUnPinned;

		[NonSerialized]
		internal long Offset = -1L;

		[NonSerialized]
		private LinkedBucketedQueue<BaseReference> m_otherReferences;

		[NonSerialized]
		private static readonly Declaration m_declaration = StorageItem.GetDeclaration();

		public int Size
		{
			get
			{
				return this.m_size;
			}
		}

		internal int ActiveReferenceCount
		{
			get
			{
				int num = 0;
				if (base.Reference != (object)null)
				{
					num++;
				}
				if (this.m_otherReferences != null)
				{
					num += this.m_otherReferences.Count;
				}
				return num;
			}
		}

		public StorageItem()
		{
		}

		public StorageItem(ReferenceID id, int priority, IStorable item, int initialSize)
		{
			this.Priority = priority;
			base.Item = item;
			this.Offset = -1L;
			this.Id = id;
			this.PersistedSize = -1L;
			this.m_size = this.CalculateSize(initialSize);
		}

		public void AddReference(BaseReference newReference)
		{
			if (base.Reference == (object)null)
			{
				base.Reference = newReference;
			}
			else
			{
				if (this.m_otherReferences == null)
				{
					this.m_otherReferences = new LinkedBucketedQueue<BaseReference>(5);
				}
				this.m_otherReferences.Enqueue(newReference);
			}
		}

		public int UpdateSize()
		{
			int size = this.m_size;
			this.m_size = this.CalculateSize(ItemSizes.SizeOf(base.Item));
			return this.m_size - size;
		}

		private int CalculateSize(int itemSize)
		{
			return base.BaseSize() + itemSize + 8 + 8 + 4 + 4 + 1 + 8 + ItemSizes.ReferenceSize;
		}

		internal override int ComputeSizeForReference()
		{
			return 0;
		}

		public void UpdateSize(int sizeDeltaBytes)
		{
			this.m_size += sizeDeltaBytes;
		}

		public void Flush(IStorage storage, IIndexStrategy indexStrategy)
		{
			bool isTemporary = this.Id.IsTemporary;
			if (isTemporary)
			{
				this.Id = indexStrategy.GenerateId(this.Id);
			}
			this.UnlinkReferences(isTemporary);
			if (this.Offset >= 0)
			{
				long num = storage.Update(this, this.Offset, this.PersistedSize);
				if (num != this.Offset)
				{
					this.Offset = num;
					indexStrategy.Update(this.Id, this.Offset);
				}
			}
			else
			{
				this.Offset = storage.Allocate(this);
				indexStrategy.Update(this.Id, this.Offset);
			}
		}

		internal void UnlinkReferences(bool updateId)
		{
			if (base.Reference != (object)null)
			{
				if (updateId)
				{
					base.Reference.Id = this.Id;
				}
				base.Reference.Item = null;
			}
			if (this.m_otherReferences != null)
			{
				while (this.m_otherReferences.Count > 0)
				{
					BaseReference baseReference = this.m_otherReferences.Dequeue();
					baseReference.Item = null;
					if (updateId)
					{
						baseReference.Id = this.Id;
					}
				}
			}
			base.Reference = null;
			this.m_otherReferences = null;
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(StorageItem.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Item:
					writer.Write(base.Item);
					break;
				case MemberName.Priority:
					writer.Write(this.Priority);
					break;
				case MemberName.ID:
					writer.Write(this.Id.Value);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(StorageItem.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Item:
					base.Item = (IStorable)reader.ReadRIFObject();
					break;
				case MemberName.Priority:
					this.Priority = reader.ReadInt32();
					break;
				case MemberName.ID:
					this.Id = new ReferenceID(reader.ReadInt64());
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorageItem;
		}

		[SkipMemberStaticValidation(MemberName.Item)]
		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Priority, Token.Int32));
			list.Add(new MemberInfo(MemberName.Item, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObject));
			list.Add(new MemberInfo(MemberName.ID, Token.Int64));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorageItem, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}
	}
}
