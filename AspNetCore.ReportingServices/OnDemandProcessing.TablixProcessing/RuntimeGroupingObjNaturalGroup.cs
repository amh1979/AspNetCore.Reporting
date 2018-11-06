using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeGroupingObjNaturalGroup : RuntimeGroupingObjLinkedList
	{
		private object m_lastValue;

		private IReference<RuntimeHierarchyObj> m_lastChild;

		[NonSerialized]
		private static Declaration m_declaration = RuntimeGroupingObjNaturalGroup.GetDeclaration();

		public override int Size
		{
			get
			{
				return base.Size + ItemSizes.SizeOf(this.m_lastValue) + ItemSizes.SizeOf(this.m_lastChild);
			}
		}

		internal RuntimeGroupingObjNaturalGroup()
		{
		}

		internal RuntimeGroupingObjNaturalGroup(RuntimeHierarchyObj owner, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(owner, objectType)
		{
		}

		internal override void NextRow(object keyValue, bool hasParent, object parentKey)
		{
			if (this.m_lastChild != null && base.m_owner.OdpContext.EqualityComparer.Equals(this.m_lastValue, keyValue))
			{
				using (this.m_lastChild.PinValue())
				{
					RuntimeHierarchyObj runtimeHierarchyObj = this.m_lastChild.Value();
					runtimeHierarchyObj.NextRow();
				}
			}
			else
			{
				this.m_lastValue = keyValue;
				this.m_lastChild = base.CreateHierarchyObjAndAddToParent();
			}
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RuntimeGroupingObjNaturalGroup.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.LastValue:
					writer.Write(this.m_lastValue);
					break;
				case MemberName.LastChild:
					writer.Write(this.m_lastChild);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(RuntimeGroupingObjNaturalGroup.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.LastValue:
					this.m_lastValue = reader.ReadVariant();
					break;
				case MemberName.LastChild:
					this.m_lastChild = (IReference<RuntimeHierarchyObj>)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjNaturalGroup;
		}

		public new static Declaration GetDeclaration()
		{
			if (RuntimeGroupingObjNaturalGroup.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.LastValue, Token.Object));
				list.Add(new MemberInfo(MemberName.LastChild, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObjReference));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjNaturalGroup, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjLinkedList, list);
			}
			return RuntimeGroupingObjNaturalGroup.m_declaration;
		}
	}
}
