using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class StorageObjectCreator : IScalabilityObjectCreator
	{
		private static List<Declaration> m_declarations = StorageObjectCreator.BuildDeclarations();

		private static StorageObjectCreator m_instance = null;

		internal static StorageObjectCreator Instance
		{
			get
			{
				if (StorageObjectCreator.m_instance == null)
				{
					StorageObjectCreator.m_instance = new StorageObjectCreator();
				}
				return StorageObjectCreator.m_instance;
			}
		}

		private StorageObjectCreator()
		{
		}

		public bool TryCreateObject(ObjectType objectType, out IPersistable persistObj)
		{
			switch (objectType)
			{
			case ObjectType.Aggregate:
				persistObj = new Aggregate();
				break;
			case ObjectType.AggregateRow:
				persistObj = new AggregateRow();
				break;
			case ObjectType.Avg:
				persistObj = new Avg();
				break;
			case ObjectType.BTree:
				persistObj = new BTree();
				break;
			case ObjectType.BTreeNode:
				persistObj = new BTreeNode();
				break;
			case ObjectType.BTreeNodeTupleList:
				persistObj = new BTreeNodeTupleList();
				break;
			case ObjectType.BTreeNodeTuple:
				persistObj = new BTreeNodeTuple();
				break;
			case ObjectType.BTreeNodeHierarchyObj:
				persistObj = new BTreeNodeHierarchyObj();
				break;
			case ObjectType.CalculatedFieldWrapperImpl:
				persistObj = new CalculatedFieldWrapperImpl();
				break;
			case ObjectType.ChildLeafInfo:
				persistObj = new ChildLeafInfo();
				break;
			case ObjectType.Count:
				persistObj = new Count();
				break;
			case ObjectType.CountDistinct:
				persistObj = new CountDistinct();
				break;
			case ObjectType.CountRows:
				persistObj = new CountRows();
				break;
			case ObjectType.DataAggregateObj:
				persistObj = new DataAggregateObj();
				break;
			case ObjectType.DataAggregateObjResult:
				persistObj = new DataAggregateObjResult();
				break;
			case ObjectType.DataFieldRow:
				persistObj = new DataFieldRow();
				break;
			case ObjectType.DataRegionMemberInstance:
				persistObj = new DataRegionMemberInstance();
				break;
			case ObjectType.FieldImpl:
				persistObj = new FieldImpl();
				break;
			case ObjectType.First:
				persistObj = new First();
				break;
			case ObjectType.Last:
				persistObj = new Last();
				break;
			case ObjectType.Max:
				persistObj = new Max();
				break;
			case ObjectType.Min:
				persistObj = new Min();
				break;
			case ObjectType.Previous:
				persistObj = new Previous();
				break;
			case ObjectType.RuntimeCells:
				persistObj = new RuntimeCells();
				break;
			case ObjectType.RuntimeChartCriCell:
				persistObj = new RuntimeChartCriCell();
				break;
			case ObjectType.RuntimeChartCriGroupLeafObj:
				persistObj = new RuntimeChartCriGroupLeafObj();
				break;
			case ObjectType.RuntimeChartObj:
				persistObj = new RuntimeChartObj();
				break;
			case ObjectType.RuntimeGaugePanelObj:
				persistObj = new RuntimeGaugePanelObj();
				break;
			case ObjectType.RuntimeCriObj:
				persistObj = new RuntimeCriObj();
				break;
			case ObjectType.RuntimeDataTablixGroupRootObj:
				persistObj = new RuntimeDataTablixGroupRootObj();
				break;
			case ObjectType.RuntimeDataTablixMemberObj:
				persistObj = new RuntimeDataTablixMemberObj();
				break;
			case ObjectType.RuntimeExpressionInfo:
				persistObj = new RuntimeExpressionInfo();
				break;
			case ObjectType.RuntimeHierarchyObj:
				persistObj = new RuntimeHierarchyObj();
				break;
			case ObjectType.RuntimeRICollection:
				persistObj = new RuntimeRICollection();
				break;
			case ObjectType.RuntimeSortDataHolder:
				persistObj = new RuntimeSortDataHolder();
				break;
			case ObjectType.RuntimeSortFilterEventInfo:
				persistObj = new RuntimeSortFilterEventInfo();
				break;
			case ObjectType.RuntimeSortHierarchyObj:
				persistObj = new RuntimeSortHierarchyObj();
				break;
			case ObjectType.RuntimeDataRowSortHierarchyObj:
				persistObj = new RuntimeDataRowSortHierarchyObj();
				break;
			case ObjectType.RuntimeTablixCell:
				persistObj = new RuntimeTablixCell();
				break;
			case ObjectType.RuntimeTablixGroupLeafObj:
				persistObj = new RuntimeTablixGroupLeafObj();
				break;
			case ObjectType.RuntimeTablixObj:
				persistObj = new RuntimeTablixObj();
				break;
			case ObjectType.RuntimeUserSortTargetInfo:
				persistObj = new RuntimeUserSortTargetInfo();
				break;
			case ObjectType.ScopeLookupTable:
				persistObj = new ScopeLookupTable();
				break;
			case ObjectType.SortExpressionScopeInstanceHolder:
				persistObj = new RuntimeSortFilterEventInfo.SortExpressionScopeInstanceHolder();
				break;
			case ObjectType.SortFilterExpressionScopeObj:
				persistObj = new RuntimeSortFilterEventInfo.SortFilterExpressionScopeObj();
				break;
			case ObjectType.SortHierarchyStruct:
				persistObj = new RuntimeSortHierarchyObj.SortHierarchyStructure();
				break;
			case ObjectType.SortScopeValuesHolder:
				persistObj = new RuntimeSortFilterEventInfo.SortScopeValuesHolder();
				break;
			case ObjectType.StDev:
				persistObj = new StDev();
				break;
			case ObjectType.StDevP:
				persistObj = new StDevP();
				break;
			case ObjectType.StorageItem:
				persistObj = new StorageItem();
				break;
			case ObjectType.Sum:
				persistObj = new Sum();
				break;
			case ObjectType.Var:
				persistObj = new Var();
				break;
			case ObjectType.VarP:
				persistObj = new VarP();
				break;
			case ObjectType.FilterKey:
				persistObj = new Filters.FilterKey();
				break;
			case ObjectType.LookupMatches:
				persistObj = new LookupMatches();
				break;
			case ObjectType.LookupMatchesWithRows:
				persistObj = new LookupMatchesWithRows();
				break;
			case ObjectType.LookupTable:
				persistObj = new LookupTable();
				break;
			case ObjectType.Union:
				persistObj = new Union();
				break;
			case ObjectType.RuntimeMapDataRegionObj:
				persistObj = new RuntimeMapDataRegionObj();
				break;
			case ObjectType.DataScopeInfo:
				persistObj = new DataScopeInfo();
				break;
			case ObjectType.BucketedDataAggregateObjs:
				persistObj = new BucketedDataAggregateObjs();
				break;
			case ObjectType.DataAggregateObjBucket:
				persistObj = new DataAggregateObjBucket();
				break;
			case ObjectType.RuntimeGroupingObjHash:
				persistObj = new RuntimeGroupingObjHash();
				break;
			case ObjectType.RuntimeGroupingObjTree:
				persistObj = new RuntimeGroupingObjTree();
				break;
			case ObjectType.RuntimeGroupingObjDetail:
				persistObj = new RuntimeGroupingObjDetail();
				break;
			case ObjectType.RuntimeGroupingObjDetailUserSort:
				persistObj = new RuntimeGroupingObjDetailUserSort();
				break;
			case ObjectType.RuntimeGroupingObjLinkedList:
				persistObj = new RuntimeGroupingObjLinkedList();
				break;
			case ObjectType.RuntimeGroupingObjNaturalGroup:
				persistObj = new RuntimeGroupingObjNaturalGroup();
				break;
			default:
				persistObj = null;
				return false;
			}
			return true;
		}

		public List<Declaration> GetDeclarations()
		{
			return StorageObjectCreator.m_declarations;
		}

		private static List<Declaration> BuildDeclarations()
		{
			List<Declaration> list = new List<Declaration>(83);
			list.Add(Aggregate.GetDeclaration());
			list.Add(AggregateRow.GetDeclaration());
			list.Add(Avg.GetDeclaration());
			list.Add(BTree.GetDeclaration());
			list.Add(BTreeNode.GetDeclaration());
			list.Add(BTreeNodeTuple.GetDeclaration());
			list.Add(BTreeNodeTupleList.GetDeclaration());
			list.Add(BTreeNodeHierarchyObj.GetDeclaration());
			list.Add(CalculatedFieldWrapperImpl.GetDeclaration());
			list.Add(ChildLeafInfo.GetDeclaration());
			list.Add(Count.GetDeclaration());
			list.Add(CountDistinct.GetDeclaration());
			list.Add(CountRows.GetDeclaration());
			list.Add(DataAggregateObj.GetDeclaration());
			list.Add(DataAggregateObjResult.GetDeclaration());
			list.Add(DataRegionMemberInstance.GetDeclaration());
			list.Add(DataFieldRow.GetDeclaration());
			list.Add(FieldImpl.GetDeclaration());
			list.Add(First.GetDeclaration());
			list.Add(Last.GetDeclaration());
			list.Add(Max.GetDeclaration());
			list.Add(Min.GetDeclaration());
			list.Add(Previous.GetDeclaration());
			list.Add(RuntimeCell.GetDeclaration());
			list.Add(RuntimeCells.GetDeclaration());
			list.Add(RuntimeCellWithContents.GetDeclaration());
			list.Add(RuntimeChartCriCell.GetDeclaration());
			list.Add(RuntimeChartCriGroupLeafObj.GetDeclaration());
			list.Add(RuntimeChartCriObj.GetDeclaration());
			list.Add(RuntimeChartObj.GetDeclaration());
			list.Add(RuntimeCriObj.GetDeclaration());
			list.Add(RuntimeDataRegionObj.GetDeclaration());
			list.Add(RuntimeDataTablixObj.GetDeclaration());
			list.Add(RuntimeDataTablixGroupLeafObj.GetDeclaration());
			list.Add(RuntimeDataTablixGroupRootObj.GetDeclaration());
			list.Add(RuntimeDataTablixMemberObj.GetDeclaration());
			list.Add(RuntimeDataTablixWithScopedItemsObj.GetDeclaration());
			list.Add(RuntimeDataTablixWithScopedItemsGroupLeafObj.GetDeclaration());
			list.Add(RuntimeDetailObj.GetDeclaration());
			list.Add(RuntimeExpressionInfo.GetDeclaration());
			list.Add(RuntimeGroupLeafObj.GetDeclaration());
			list.Add(RuntimeGroupObj.GetDeclaration());
			list.Add(RuntimeGroupRootObj.GetDeclaration());
			list.Add(RuntimeGroupingObj.GetDeclaration());
			list.Add(RuntimeHierarchyObj.GetDeclaration());
			list.Add(RuntimeMemberObj.GetDeclaration());
			list.Add(RuntimeRDLDataRegionObj.GetDeclaration());
			list.Add(RuntimeRICollection.GetDeclaration());
			list.Add(RuntimeSortDataHolder.GetDeclaration());
			list.Add(RuntimeSortFilterEventInfo.GetDeclaration());
			list.Add(RuntimeSortFilterEventInfo.SortExpressionScopeInstanceHolder.GetDeclaration());
			list.Add(RuntimeSortFilterEventInfo.SortFilterExpressionScopeObj.GetDeclaration());
			list.Add(RuntimeSortFilterEventInfo.SortScopeValuesHolder.GetDeclaration());
			list.Add(RuntimeSortHierarchyObj.GetDeclaration());
			list.Add(RuntimeSortHierarchyObj.SortHierarchyStructure.GetDeclaration());
			list.Add(RuntimeDataRowSortHierarchyObj.GetDeclaration());
			list.Add(RuntimeTablixCell.GetDeclaration());
			list.Add(RuntimeTablixGroupLeafObj.GetDeclaration());
			list.Add(RuntimeTablixObj.GetDeclaration());
			list.Add(RuntimeUserSortTargetInfo.GetDeclaration());
			list.Add(ScopeInstance.GetDeclaration());
			list.Add(ScopeLookupTable.GetDeclaration());
			list.Add(StDev.GetDeclaration());
			list.Add(StDevP.GetDeclaration());
			list.Add(Sum.GetDeclaration());
			list.Add(Var.GetDeclaration());
			list.Add(VarBase.GetDeclaration());
			list.Add(VarP.GetDeclaration());
			list.Add(Filters.FilterKey.GetDeclaration());
			list.Add(RuntimeGaugePanelObj.GetDeclaration());
			list.Add(LookupMatches.GetDeclaration());
			list.Add(LookupMatchesWithRows.GetDeclaration());
			list.Add(LookupTable.GetDeclaration());
			list.Add(RuntimeMapDataRegionObj.GetDeclaration());
			list.Add(DataScopeInfo.GetDeclaration());
			list.Add(BucketedDataAggregateObjs.GetDeclaration());
			list.Add(DataAggregateObjBucket.GetDeclaration());
			list.Add(RuntimeGroupingObjHash.GetDeclaration());
			list.Add(RuntimeGroupingObjTree.GetDeclaration());
			list.Add(RuntimeGroupingObjDetail.GetDeclaration());
			list.Add(RuntimeGroupingObjDetailUserSort.GetDeclaration());
			list.Add(RuntimeGroupingObjLinkedList.GetDeclaration());
			list.Add(RuntimeGroupingObjNaturalGroup.GetDeclaration());
			return list;
		}
	}
}
