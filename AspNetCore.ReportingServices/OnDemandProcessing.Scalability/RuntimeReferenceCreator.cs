using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeReferenceCreator : IReferenceCreator
	{
		private static RuntimeReferenceCreator m_instance = new RuntimeReferenceCreator();

		internal static RuntimeReferenceCreator Instance
		{
			get
			{
				return RuntimeReferenceCreator.m_instance;
			}
		}

		private RuntimeReferenceCreator()
		{
		}

		public bool TryCreateReference(IStorable refTarget, out BaseReference newReference)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType objectType = refTarget.GetObjectType();
			AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType referenceObjectType = default(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType);
			if (this.TryMapObjectTypeToReferenceType(objectType, out referenceObjectType))
			{
				return this.TryCreateReference(referenceObjectType, out newReference);
			}
			newReference = null;
			return false;
		}

		public bool TryCreateReference(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType referenceObjectType, out BaseReference reference)
		{
			switch (referenceObjectType)
			{
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Null:
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None:
				Global.Tracer.Assert(false, "Cannot create reference to Nothing or Null");
				reference = null;
				return false;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixCellReference:
				reference = new RuntimeTablixCellReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCellReference:
				reference = new RuntimeCellReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDetailObjReference:
				reference = new RuntimeDetailObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixObjReference:
				reference = new RuntimeDataTablixObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObjReference:
				reference = new RuntimeHierarchyObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObjReference:
				reference = new RuntimeDataRegionObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupRootObjReference:
				reference = new RuntimeDataTablixGroupRootObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupRootObjReference:
				reference = new RuntimeGroupRootObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObjReference:
				reference = new RuntimeGroupObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixGroupLeafObjReference:
				reference = new RuntimeTablixGroupLeafObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriGroupLeafObjReference:
				reference = new RuntimeChartCriGroupLeafObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupLeafObjReference:
				reference = new RuntimeDataTablixGroupLeafObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObjReference:
				reference = new RuntimeGroupLeafObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeOnDemandDataSetObjReference:
				reference = new RuntimeOnDemandDataSetObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortHierarchyObjReference:
				reference = new RuntimeSortHierarchyObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixObjReference:
				reference = new RuntimeTablixObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartObjReference:
				reference = new RuntimeChartObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGaugePanelObjReference:
				reference = new RuntimeGaugePanelObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMapDataRegionObjReference:
				reference = new RuntimeMapDataRegionObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCriObjReference:
				reference = new RuntimeCriObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AggregateRowReference:
				reference = new SimpleReference<AggregateRow>(referenceObjectType);
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateReference:
				reference = new SimpleReference<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregate>(referenceObjectType);
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjReference:
				reference = new SimpleReference<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj>(referenceObjectType);
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRowReference:
				reference = new SimpleReference<DataFieldRow>(referenceObjectType);
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IHierarchyObjReference:
				reference = new SimpleReference<IHierarchyObj>(referenceObjectType);
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCellsReference:
				reference = new SimpleReference<RuntimeCells>(referenceObjectType);
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriCellReference:
				reference = new RuntimeChartCriCellReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriObjReference:
				reference = new RuntimeChartCriObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixMemberObjReference:
				reference = new RuntimeDataTablixMemberObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjReference:
				reference = new SimpleReference<RuntimeGroupingObj>(referenceObjectType);
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObjReference:
				reference = new RuntimeMemberObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRDLDataRegionObjReference:
				reference = new SimpleReference<RuntimeRDLDataRegionObj>(referenceObjectType);
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRICollectionReference:
				reference = new SimpleReference<RuntimeRICollection>(referenceObjectType);
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortFilterEventInfoReference:
				reference = new SimpleReference<AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing.RuntimeSortFilterEventInfo>(referenceObjectType);
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeUserSortTargetInfoReference:
				reference = new SimpleReference<AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing.RuntimeUserSortTargetInfo>(referenceObjectType);
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortExpressionScopeInstanceHolderReference:
				reference = new SortExpressionScopeInstanceHolderReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterExpressionScopeObjReference:
				reference = new SortFilterExpressionScopeObjReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArrayReference:
				reference = new SimpleReference<StorableArray>(referenceObjectType);
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference:
				reference = new ScalableDictionaryNodeReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTableReference:
				reference = new SimpleReference<LookupTable>(referenceObjectType);
				break;
			default:
				reference = null;
				return false;
			}
			return true;
		}

		private bool TryMapObjectTypeToReferenceType(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType targetType, out AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType referenceType)
		{
			switch (targetType)
			{
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRow:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRowReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ObjectModelImpl:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeOnDemandDataSetObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortHierarchyObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortHierarchyObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IHierarchyObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IHierarchyObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortFilterEventInfo:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortFilterEventInfoReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterExpressionScopeObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterExpressionScopeObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortExpressionScopeInstanceHolder:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortExpressionScopeInstanceHolderReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCell:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCellReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRICollection:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRICollectionReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixCell:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixCellReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriCell:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriCellReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRDLDataRegionObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRDLDataRegionObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeUserSortTargetInfo:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeUserSortTargetInfoReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AggregateRow:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AggregateRowReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IScope:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IScopeReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCells:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCellsReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupLeafObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupLeafObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDetailObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDetailObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupRootObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupRootObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupRootObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupRootObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixMemberObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixMemberObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGaugePanelObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGaugePanelObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMapDataRegionObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMapDataRegionObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCriObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCriObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixGroupLeafObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixGroupLeafObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriGroupLeafObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriGroupLeafObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeOnDemandDataSetObj:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeOnDemandDataSetObjReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArray:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArrayReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNode:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTable:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTableReference;
				break;
			default:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None;
				return false;
			}
			return true;
		}
	}
}
