using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeChartCriGroupLeafObj : RuntimeDataTablixGroupLeafObj
	{
		[NonSerialized]
		private static Declaration m_declaration = RuntimeChartCriGroupLeafObj.GetDeclaration();

		internal RuntimeChartCriGroupLeafObj()
		{
		}

		internal RuntimeChartCriGroupLeafObj(RuntimeDataTablixGroupRootObjReference groupRootRef, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(groupRootRef, objectType)
		{
		}

		internal override RuntimeDataTablixObjReference GetNestedDataRegion(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion)
		{
			Global.Tracer.Assert(false, "This type of group leaf does not support nested data regions.");
			throw new InvalidOperationException();
		}

		protected override void ConstructOutermostCellContents(Cell cell)
		{
		}

		internal override void CreateCell(RuntimeCells cellsCollection, int collectionKey, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode outerGroupingMember, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode innerGroupingMember, AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef)
		{
			RuntimeCell runtimeCell = new RuntimeChartCriCell((RuntimeChartCriGroupLeafObjReference)base.m_selfReference, outerGroupingMember, innerGroupingMember, !base.m_hasInnerHierarchy);
			if ((BaseReference)runtimeCell.SelfReference == (object)null)
			{
				cellsCollection.AddCell(collectionKey, runtimeCell);
			}
			else
			{
				IReference<RuntimeCell> selfReference = runtimeCell.SelfReference;
				selfReference.UnPinValue();
				cellsCollection.AddCell(collectionKey, selfReference);
			}
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RuntimeChartCriGroupLeafObj.m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				Global.Tracer.Assert(false);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(RuntimeChartCriGroupLeafObj.m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				Global.Tracer.Assert(false);
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriGroupLeafObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (RuntimeChartCriGroupLeafObj.m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriGroupLeafObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupLeafObj, memberInfoList);
			}
			return RuntimeChartCriGroupLeafObj.m_declaration;
		}
	}
}
