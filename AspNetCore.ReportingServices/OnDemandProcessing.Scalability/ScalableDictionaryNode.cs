using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class ScalableDictionaryNode : IScalableDictionaryEntry, IStorable, IPersistable, ITransferable
	{
		internal IScalableDictionaryEntry[] Entries;

		internal int Count;

		[NonSerialized]
		private static readonly Declaration m_declaration = ScalableDictionaryNode.GetDeclaration();

		public int Size
		{
			get
			{
				return 4 + ItemSizes.SizeOf(this.Entries);
			}
		}

		public int EmptySize
		{
			get
			{
				return ItemSizes.NonNullIStorableOverhead + 4 + ItemSizes.SizeOfEmptyObjectArray(this.Entries.Length);
			}
		}

		internal ScalableDictionaryNode()
		{
		}

		internal ScalableDictionaryNode(int capacity)
		{
			this.Entries = new IScalableDictionaryEntry[capacity];
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ScalableDictionaryNode.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Entries:
					writer.Write(this.Entries);
					break;
				case MemberName.Count:
					writer.Write(this.Count);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ScalableDictionaryNode.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Entries:
					this.Entries = reader.ReadArrayOfRIFObjects<IScalableDictionaryEntry>();
					break;
				case MemberName.Count:
					this.Count = reader.ReadInt32();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNode;
		}

		internal static Declaration GetDeclaration()
		{
			if (ScalableDictionaryNode.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Entries, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IScalableDictionaryEntry));
				list.Add(new MemberInfo(MemberName.Count, Token.Int32));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNode, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return ScalableDictionaryNode.m_declaration;
		}

		public void TransferTo(IScalabilityCache scaleCache)
		{
			for (int i = 0; i < this.Entries.Length; i++)
			{
				IScalableDictionaryEntry scalableDictionaryEntry = this.Entries[i];
				if (scalableDictionaryEntry != null)
				{
					switch (scalableDictionaryEntry.GetObjectType())
					{
					case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference:
					{
						ScalableDictionaryNodeReference scalableDictionaryNodeReference = scalableDictionaryEntry as ScalableDictionaryNodeReference;
						this.Entries[i] = (ScalableDictionaryNodeReference)scalableDictionaryNodeReference.TransferTo(scaleCache);
						break;
					}
					case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryValues:
					{
						ScalableDictionaryValues scalableDictionaryValues = scalableDictionaryEntry as ScalableDictionaryValues;
						scalableDictionaryValues.TransferTo(scaleCache);
						break;
					}
					default:
						Global.Tracer.Assert(false, "Unknown ObjectType");
						break;
					}
				}
			}
		}
	}
}
