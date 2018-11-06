using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class StorableArray : IStorable, IPersistable, ITransferable
	{
		internal object[] Array;

		private static Declaration m_declaration = StorableArray.GetDeclaration();

		public int Size
		{
			get
			{
				return ItemSizes.SizeOf(this.Array);
			}
		}

		public int EmptySize
		{
			get
			{
				return ItemSizes.NonNullIStorableOverhead + ItemSizes.SizeOfEmptyObjectArray(this.Array.Length);
			}
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(StorableArray.m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.Array)
				{
					writer.WriteVariantOrPersistableArray(this.Array);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(StorableArray.m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.Array)
				{
					this.Array = reader.ReadVariantArray();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArray;
		}

		internal static Declaration GetDeclaration()
		{
			if (StorableArray.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Array, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.Object));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return StorableArray.m_declaration;
		}

		public void TransferTo(IScalabilityCache scaleCache)
		{
			if (this.Array != null && this.Array.Length > 0)
			{
				IReference reference = this.Array[0] as IReference;
				if (reference != null)
				{
					this.Array[0] = reference.TransferTo(scaleCache);
					for (int i = 1; i < this.Array.Length; i++)
					{
						reference = (this.Array[i] as IReference);
						if (reference != null)
						{
							this.Array[i] = reference.TransferTo(scaleCache);
						}
					}
				}
				else
				{
					ITransferable transferable = this.Array[0] as ITransferable;
					if (transferable != null)
					{
						transferable.TransferTo(scaleCache);
						for (int j = 1; j < this.Array.Length; j++)
						{
							transferable = (this.Array[j] as ITransferable);
							if (transferable != null)
							{
								transferable.TransferTo(scaleCache);
							}
						}
					}
				}
			}
		}
	}
}
