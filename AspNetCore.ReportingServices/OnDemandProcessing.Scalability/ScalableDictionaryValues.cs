using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class ScalableDictionaryValues : IScalableDictionaryEntry, IStorable, IPersistable, ITransferable
	{
		private object[] m_keys;

		private object[] m_values;

		private int m_count;

		private static readonly Declaration m_declaration = ScalableDictionaryValues.GetDeclaration();

		public object[] Keys
		{
			get
			{
				return this.m_keys;
			}
		}

		public object[] Values
		{
			get
			{
				return this.m_values;
			}
		}

		public int Count
		{
			get
			{
				return this.m_count;
			}
			set
			{
				this.m_count = value;
			}
		}

		public int Capacity
		{
			get
			{
				return this.m_keys.Length;
			}
		}

		public int Size
		{
			get
			{
				return 4 + ItemSizes.SizeOf(this.m_keys) + ItemSizes.SizeOf(this.m_values);
			}
		}

		public int EmptySize
		{
			get
			{
				return ItemSizes.NonNullIStorableOverhead + 4 + ItemSizes.SizeOfEmptyObjectArray(this.m_keys.Length) * 2;
			}
		}

		internal ScalableDictionaryValues()
		{
		}

		public ScalableDictionaryValues(int capacity)
		{
			this.m_count = 0;
			this.m_keys = new object[capacity];
			this.m_values = new object[capacity];
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ScalableDictionaryValues.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Keys:
					writer.WriteVariantOrPersistableArray(this.m_keys);
					break;
				case MemberName.Values:
					writer.WriteVariantOrPersistableArray(this.m_values);
					break;
				case MemberName.Count:
					writer.Write(this.m_count);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ScalableDictionaryValues.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Keys:
					this.m_keys = reader.ReadVariantArray();
					break;
				case MemberName.Values:
					this.m_values = reader.ReadVariantArray();
					break;
				case MemberName.Count:
					this.m_count = reader.ReadInt32();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryValues;
		}

		internal static Declaration GetDeclaration()
		{
			if (ScalableDictionaryValues.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Keys, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.Object));
				list.Add(new MemberInfo(MemberName.Values, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.Object));
				list.Add(new MemberInfo(MemberName.Count, Token.Int32));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return ScalableDictionaryValues.m_declaration;
		}

		public void TransferTo(IScalabilityCache scaleCache)
		{
			for (int i = 0; i < this.m_count; i++)
			{
				ITransferable transferable = this.m_values[i] as ITransferable;
				if (transferable != null)
				{
					transferable.TransferTo(scaleCache);
				}
			}
		}
	}
}
