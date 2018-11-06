using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeTablixCell : RuntimeCellWithContents
	{
		[NonSerialized]
		private static Declaration m_declaration = RuntimeTablixCell.GetDeclaration();

		internal RuntimeTablixCell()
		{
		}

		internal RuntimeTablixCell(RuntimeTablixGroupLeafObjReference owner, TablixMember outerGroupingMember, TablixMember innerGroupingMember, bool innermost)
			: base(owner, outerGroupingMember, innerGroupingMember, innermost)
		{
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

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixCell;
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
			Global.Tracer.Assert(false, base.GetType().Name + " should not resolve references");
		}

		public new static Declaration GetDeclaration()
		{
			if (RuntimeTablixCell.m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				RuntimeTablixCell.m_declaration = new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixCell, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCellWithContents, memberInfoList);
			}
			return RuntimeTablixCell.m_declaration;
		}
	}
}
