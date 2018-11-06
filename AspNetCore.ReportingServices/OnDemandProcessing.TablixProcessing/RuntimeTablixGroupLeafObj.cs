using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeTablixGroupLeafObj : RuntimeDataTablixWithScopedItemsGroupLeafObj
	{
		[NonSerialized]
		private static Declaration m_declaration = RuntimeTablixGroupLeafObj.GetDeclaration();

		internal RuntimeTablixGroupLeafObj()
		{
		}

		internal RuntimeTablixGroupLeafObj(RuntimeDataTablixGroupRootObjReference groupRootRef, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(groupRootRef, objectType)
		{
		}

		protected override List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> GetGroupScopedContents(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode member)
		{
			TablixMember tablixMember = (TablixMember)member;
			return tablixMember.GroupScopedContentsForProcessing;
		}

		protected override List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> GetCellContents(Cell cell)
		{
			TablixCell tablixCell = cell as TablixCell;
			if (tablixCell != null && tablixCell.HasInnerGroupTreeHierarchy)
			{
				return tablixCell.CellContentCollection;
			}
			return null;
		}

		protected override RuntimeCell CreateCell(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode outerGroupingMember, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode innerGroupingMember)
		{
			return new RuntimeTablixCell((RuntimeTablixGroupLeafObjReference)base.m_selfReference, (TablixMember)outerGroupingMember, (TablixMember)innerGroupingMember, !base.m_hasInnerHierarchy);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixGroupLeafObj;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public new static Declaration GetDeclaration()
		{
			if (RuntimeTablixGroupLeafObj.m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				RuntimeTablixGroupLeafObj.m_declaration = new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixGroupLeafObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixWithScopedItemsGroupLeafObj, memberInfoList);
			}
			return RuntimeTablixGroupLeafObj.m_declaration;
		}
	}
}
