using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeCell : IScope, ISelfReferential, IDataRowHolder, IOnDemandScopeInstance, IStorable, IPersistable
	{
		protected RuntimeDataTablixGroupLeafObjReference m_owner;

		protected int m_outerGroupDynamicIndex;

		protected List<int> m_rowIndexes;

		protected List<int> m_colIndexes;

		protected List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_cellNonCustomAggObjs;

		protected BucketedDataAggregateObjs m_cellAggregatesOfAggregates;

		protected BucketedDataAggregateObjs m_cellPostSortAggregatesOfAggregates;

		protected Cell m_canonicalCellScopeDef;

		protected List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_cellCustomAggObjs;

		protected AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] m_cellAggValueList;

		protected AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[,][] m_runningValueValues;

		protected AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[,][] m_runningValueOfAggregateValues;

		protected ScalableList<DataFieldRow> m_dataRows;

		protected DataActions m_dataAction;

		protected bool m_innermost;

		protected DataFieldRow m_firstRow;

		protected bool m_firstRowIsAggregate;

		private int m_nextCell = -1;

		protected int[] m_sortFilterExpressionScopeInfoIndices;

		private long m_scopeInstanceNumber;

		private bool m_hasProcessedAggregateRow;

		[NonSerialized]
		protected RuntimeCellReference m_selfReference;

		[NonSerialized]
		private static Declaration m_declaration = RuntimeCell.GetDeclaration();

		internal int NextCell
		{
			get
			{
				return this.m_nextCell;
			}
			set
			{
				this.m_nextCell = value;
			}
		}

		public bool IsNoRows
		{
			get
			{
				return this.m_firstRow == null;
			}
		}

		public bool IsMostRecentlyCreatedScopeInstance
		{
			get
			{
				return this.m_canonicalCellScopeDef.CanonicalDataScopeInfo.IsLastScopeInstanceNumber(this.m_scopeInstanceNumber);
			}
		}

		public bool HasUnProcessedServerAggregate
		{
			get
			{
				if (this.m_cellCustomAggObjs != null && this.m_cellCustomAggObjs.Count > 0)
				{
					return !this.m_hasProcessedAggregateRow;
				}
				return false;
			}
		}

		bool IScope.TargetForNonDetailSort
		{
			get
			{
				return this.m_owner.Value().GetCellTargetForNonDetailSort();
			}
		}

		int[] IScope.SortFilterExpressionScopeInfoIndices
		{
			get
			{
				if (this.m_sortFilterExpressionScopeInfoIndices == null)
				{
					OnDemandProcessingContext odpContext = this.m_owner.Value().OdpContext;
					this.m_sortFilterExpressionScopeInfoIndices = new int[odpContext.RuntimeSortFilterInfo.Count];
					for (int i = 0; i < odpContext.RuntimeSortFilterInfo.Count; i++)
					{
						this.m_sortFilterExpressionScopeInfoIndices[i] = -1;
					}
				}
				return this.m_sortFilterExpressionScopeInfoIndices;
			}
		}

		IRIFReportScope IScope.RIFReportScope
		{
			get
			{
				int index = this.m_rowIndexes[0];
				int index2 = this.m_colIndexes[0];
				AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = this.m_owner.Value().DataRegionDef;
				return dataRegionDef.Rows[index].Cells[index2];
			}
		}

		int IScope.Depth
		{
			get
			{
				return this.m_outerGroupDynamicIndex;
			}
		}

		internal RuntimeCellReference SelfReference
		{
			get
			{
				return this.m_selfReference;
			}
		}

		public virtual int Size
		{
			get
			{
				return ItemSizes.SizeOf(this.m_owner) + 4 + ItemSizes.SizeOf(this.m_rowIndexes) + ItemSizes.SizeOf(this.m_colIndexes) + ItemSizes.SizeOf(this.m_cellNonCustomAggObjs) + ItemSizes.SizeOf(this.m_cellCustomAggObjs) + ItemSizes.SizeOf(this.m_cellAggValueList) + ItemSizes.SizeOf(this.m_runningValueValues) + ItemSizes.SizeOf(this.m_runningValueOfAggregateValues) + ItemSizes.SizeOf(this.m_dataRows) + 1 + ItemSizes.SizeOf(this.m_firstRow) + 1 + 4 + ItemSizes.SizeOf(this.m_sortFilterExpressionScopeInfoIndices) + ItemSizes.SizeOf(this.m_selfReference) + ItemSizes.SizeOf(this.m_cellAggregatesOfAggregates) + ItemSizes.SizeOf(this.m_cellPostSortAggregatesOfAggregates) + ItemSizes.ReferenceSize + 4 + 8 + 1;
			}
		}

		protected RuntimeCell()
		{
		}

		internal RuntimeCell(RuntimeDataTablixGroupLeafObjReference owner, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode outerGroupingMember, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode innerGroupingMember, bool innermost)
		{
			this.m_owner = owner;
			this.m_outerGroupDynamicIndex = outerGroupingMember.HierarchyDynamicIndex;
			this.m_innermost = innermost;
			this.m_dataAction = DataActions.None;
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = owner.Value().DataRegionDef;
			OnDemandProcessingContext odpContext = owner.Value().OdpContext;
			RuntimeCell.GetCellIndexes(outerGroupingMember, innerGroupingMember, dataRegionDef, out this.m_rowIndexes, out this.m_colIndexes);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			int num = this.m_rowIndexes.Count * this.m_colIndexes.Count;
			foreach (int rowIndex in this.m_rowIndexes)
			{
				foreach (int colIndex in this.m_colIndexes)
				{
					Cell cell = dataRegionDef.Rows[rowIndex].Cells[colIndex];
					if (cell != null && this.m_canonicalCellScopeDef == null && cell.DataScopeInfo != null)
					{
						this.m_canonicalCellScopeDef = cell;
						if (this.m_canonicalCellScopeDef.CanonicalDataScopeInfo == null)
						{
							if (num == 1)
							{
								this.m_canonicalCellScopeDef.CanonicalDataScopeInfo = cell.DataScopeInfo;
							}
							else
							{
								flag3 = true;
								this.m_canonicalCellScopeDef.CanonicalDataScopeInfo = new DataScopeInfo(cell.DataScopeInfo.ScopeID);
							}
						}
					}
					if (cell != null && !cell.SimpleGroupTreeCell)
					{
						if (cell.AggregateIndexes != null)
						{
							RuntimeDataRegionObj.CreateAggregates(odpContext, dataRegionDef.CellAggregates, cell.AggregateIndexes, ref this.m_cellNonCustomAggObjs, ref this.m_cellCustomAggObjs);
						}
						if (cell.DataScopeInfo != null)
						{
							DataScopeInfo dataScopeInfo = cell.DataScopeInfo;
							if (flag3)
							{
								cell.CanonicalDataScopeInfo = this.m_canonicalCellScopeDef.CanonicalDataScopeInfo;
								this.m_canonicalCellScopeDef.CanonicalDataScopeInfo.MergeFrom(dataScopeInfo);
							}
							if (dataScopeInfo.AggregatesOfAggregates != null)
							{
								RuntimeDataRegionObj.CreateAggregates<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>(odpContext, (BucketedAggregatesCollection<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>)dataScopeInfo.AggregatesOfAggregates, ref this.m_cellAggregatesOfAggregates);
							}
							if (dataScopeInfo.PostSortAggregatesOfAggregates != null)
							{
								RuntimeDataRegionObj.CreateAggregates<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>(odpContext, (BucketedAggregatesCollection<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>)dataScopeInfo.PostSortAggregatesOfAggregates, ref this.m_cellPostSortAggregatesOfAggregates);
							}
						}
						if (cell.PostSortAggregateIndexes != null)
						{
							flag = true;
						}
						if (!flag2)
						{
							flag2 = ((IRIFReportScope)cell).NeedToCacheDataRows;
						}
						this.ConstructCellContents(cell, ref this.m_dataAction);
					}
				}
			}
			if (flag)
			{
				this.m_cellAggValueList = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[dataRegionDef.CellPostSortAggregates.Count];
				this.m_dataAction |= DataActions.PostSortAggregates;
			}
			if (!FlagUtils.HasFlag(this.m_dataAction, DataActions.PostSortAggregates) && dataRegionDef.CellRunningValues != null && flag2)
			{
				this.m_dataAction |= DataActions.PostSortAggregates;
			}
			this.HandleSortFilterEvent();
			if (this.m_canonicalCellScopeDef != null && this.m_canonicalCellScopeDef.CanonicalDataScopeInfo != null && this.m_canonicalCellScopeDef.CanonicalDataScopeInfo.HasAggregatesToUpdateAtRowScope)
			{
				this.m_dataAction |= DataActions.AggregatesOfAggregates;
			}
			if (this.m_dataAction != 0)
			{
				this.m_dataRows = new ScalableList<DataFieldRow>(this.m_outerGroupDynamicIndex + 1, odpContext.TablixProcessingScalabilityCache, 30);
			}
			odpContext.CreatedScopeInstance(this.m_canonicalCellScopeDef);
			this.m_scopeInstanceNumber = RuntimeDataRegionObj.AssignScopeInstanceNumber(this.GetCanonicalDataScopeInfo());
		}

		internal static void GetCellIndexes(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode outerGroupingMember, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode innerGroupingMember, AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, out List<int> rowIndexes, out List<int> colIndexes)
		{
			if (innerGroupingMember.IsColumn)
			{
				rowIndexes = ((outerGroupingMember != null) ? outerGroupingMember.GetCellIndexes() : dataRegionDef.OutermostStaticRowIndexes);
				colIndexes = innerGroupingMember.GetCellIndexes();
			}
			else
			{
				rowIndexes = innerGroupingMember.GetCellIndexes();
				colIndexes = ((outerGroupingMember != null) ? outerGroupingMember.GetCellIndexes() : dataRegionDef.OutermostStaticColumnIndexes);
			}
		}

		internal static bool HasOnlySimpleGroupTreeCells(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode outerGroupingMember, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode innerGroupingMember, AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef)
		{
			List<int> list = default(List<int>);
			List<int> list2 = default(List<int>);
			RuntimeCell.GetCellIndexes(outerGroupingMember, innerGroupingMember, dataRegionDef, out list, out list2);
			foreach (int item in list)
			{
				foreach (int item2 in list2)
				{
					Cell cell = dataRegionDef.Rows[item].Cells[item2];
					if (cell != null && !cell.SimpleGroupTreeCell)
					{
						return false;
					}
				}
			}
			return true;
		}

		protected abstract void ConstructCellContents(Cell cell, ref DataActions dataAction);

		protected abstract void CreateInstanceCellContents(Cell cell, DataCellInstance cellInstance, OnDemandProcessingContext odpContext);

		internal virtual bool NextRow()
		{
			OnDemandProcessingContext odpContext = this.m_owner.Value().OdpContext;
			RuntimeDataRegionObj.CommonFirstRow(odpContext, ref this.m_firstRowIsAggregate, ref this.m_firstRow);
			this.NextAggregateRow();
			if (!odpContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
			{
				this.NextNonAggregateRow();
			}
			return true;
		}

		private void NextNonAggregateRow()
		{
			OnDemandProcessingContext odpContext = this.m_owner.Value().OdpContext;
			RuntimeDataRegionObj.UpdateAggregates(odpContext, this.m_cellNonCustomAggObjs, false);
			if (this.m_dataRows != null)
			{
				RuntimeDataTablixObj.SaveData(this.m_dataRows, odpContext);
			}
		}

		private void NextAggregateRow()
		{
			OnDemandProcessingContext odpContext = this.m_owner.Value().OdpContext;
			FieldsImpl fieldsImpl = odpContext.ReportObjectModel.FieldsImpl;
			if (fieldsImpl.ValidAggregateRow && fieldsImpl.AggregationFieldCount == 0)
			{
				this.m_hasProcessedAggregateRow = true;
				if (this.m_cellCustomAggObjs != null)
				{
					RuntimeDataRegionObj.UpdateAggregates(odpContext, this.m_cellCustomAggObjs, false);
				}
			}
		}

		internal void SortAndFilter(AggregateUpdateContext aggContext)
		{
			this.SetupEnvironment();
			AggregateUpdateQueue workQueue = RuntimeDataRegionObj.AggregateOfAggregatesStart(aggContext, this, this.GetCanonicalDataScopeInfo(), this.m_cellAggregatesOfAggregates, AggregateUpdateFlags.Both, false);
			this.TraverseCellContents(ProcessingStages.SortAndFilter, aggContext);
			RuntimeDataRegionObj.AggregatesOfAggregatesEnd(this, aggContext, workQueue, this.GetCanonicalDataScopeInfo(), this.m_cellAggregatesOfAggregates, true);
		}

		protected virtual void TraverseCellContents(ProcessingStages operation, AggregateUpdateContext context)
		{
		}

		public void UpdateAggregates(AggregateUpdateContext aggContext)
		{
			this.SetupEnvironment();
			if (RuntimeDataRegionObj.UpdateAggregatesAtScope(aggContext, this, this.GetCanonicalDataScopeInfo(), AggregateUpdateFlags.Both, false))
			{
				this.TraverseCellContents(ProcessingStages.UpdateAggregates, aggContext);
			}
		}

		private DataScopeInfo GetCanonicalDataScopeInfo()
		{
			if (this.m_canonicalCellScopeDef == null)
			{
				return null;
			}
			return this.m_canonicalCellScopeDef.CanonicalDataScopeInfo;
		}

		protected void HandleSortFilterEvent()
		{
			using (this.m_owner.PinValue())
			{
				RuntimeDataTablixGroupLeafObj runtimeDataTablixGroupLeafObj = this.m_owner.Value();
				if (runtimeDataTablixGroupLeafObj.NeedHandleCellSortFilterEvent())
				{
					OnDemandProcessingContext odpContext = runtimeDataTablixGroupLeafObj.OdpContext;
					int count = odpContext.RuntimeSortFilterInfo.Count;
					for (int i = 0; i < count; i++)
					{
						IReference<RuntimeSortFilterEventInfo> reference = odpContext.RuntimeSortFilterInfo[i];
						using (reference.PinValue())
						{
							RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
							if (runtimeSortFilterEventInfo.EventSource.IsTablixCellScope)
							{
								AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem parent = runtimeSortFilterEventInfo.EventSource.Parent;
								while (parent != null && !parent.IsDataRegion)
								{
									parent = parent.Parent;
								}
								if (parent == runtimeDataTablixGroupLeafObj.DataRegionDef && ((IScope)this).TargetScopeMatched(i, false) && !runtimeDataTablixGroupLeafObj.GetOwnerDataTablix().Value().TargetForNonDetailSort && !runtimeSortFilterEventInfo.HasEventSourceScope)
								{
									runtimeSortFilterEventInfo.SetEventSourceScope(false, this.SelfReference, -1);
								}
							}
						}
					}
				}
			}
		}

		internal void CalculateRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
			OnDemandProcessingContext odpContext = this.m_owner.Value().OdpContext;
			if (this.GetCanonicalDataScopeInfo() != null)
			{
				List<string> list = null;
				List<string> list2 = null;
				RuntimeDataTablixObj.AddRunningValues(odpContext, this.GetCanonicalDataScopeInfo().RunningValuesOfAggregates, ref list, ref list2, groupCol, lastGroup);
			}
			bool flag = this.m_dataRows != null && FlagUtils.HasFlag(this.m_dataAction, DataActions.PostSortAggregates);
			AggregateUpdateQueue workQueue = RuntimeDataRegionObj.AggregateOfAggregatesStart(aggContext, this, this.GetCanonicalDataScopeInfo(), this.m_cellPostSortAggregatesOfAggregates, (AggregateUpdateFlags)(flag ? 1 : 3), true);
			if (flag)
			{
				DataActions dataActions = DataActions.PostSortAggregates;
				if (aggContext.LastScopeNeedsRowAggregateProcessing())
				{
					dataActions |= DataActions.PostSortAggregatesOfAggregates;
				}
				this.ReadRows(dataActions, aggContext);
				this.m_dataRows.Clear();
				this.m_dataRows = null;
			}
			using (this.m_owner.PinValue())
			{
				RuntimeDataTablixGroupLeafObj runtimeDataTablixGroupLeafObj = this.m_owner.Value();
				if (this.m_cellAggValueList != null)
				{
					List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> cellPostSortAggregates = runtimeDataTablixGroupLeafObj.CellPostSortAggregates;
					if (cellPostSortAggregates != null && 0 < cellPostSortAggregates.Count)
					{
						for (int i = 0; i < cellPostSortAggregates.Count; i++)
						{
							this.m_cellAggValueList[i] = cellPostSortAggregates[i].AggregateResult();
							cellPostSortAggregates[i].Init();
						}
					}
				}
			}
			this.CalculateInnerRunningValues(groupCol, lastGroup, aggContext);
			RuntimeDataRegionObj.AggregatesOfAggregatesEnd(this, aggContext, workQueue, this.GetCanonicalDataScopeInfo(), this.m_cellPostSortAggregatesOfAggregates, true);
			if (odpContext.HasPreviousAggregates)
			{
				this.CalculatePreviousAggregates();
			}
			this.DoneReadingRows();
		}

		internal void DoneReadingRows()
		{
			bool flag = this.m_runningValueValues != null;
			bool flag2 = this.m_runningValueValues != null;
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = this.m_owner.Value().DataRegionDef;
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo> cellRunningValues = dataRegionDef.CellRunningValues;
			if (cellRunningValues != null || (this.m_canonicalCellScopeDef != null && this.m_canonicalCellScopeDef.CanonicalDataScopeInfo != null && this.m_canonicalCellScopeDef.CanonicalDataScopeInfo.HasRunningValues))
			{
				AggregatesImpl aggregatesImpl = this.m_owner.Value().OdpContext.ReportObjectModel.AggregatesImpl;
				RowList rows = dataRegionDef.Rows;
				if (this.m_runningValueValues == null)
				{
					this.m_runningValueValues = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[this.m_rowIndexes.Count, this.m_colIndexes.Count][];
				}
				if (this.m_runningValueOfAggregateValues == null)
				{
					this.m_runningValueOfAggregateValues = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[this.m_rowIndexes.Count, this.m_colIndexes.Count][];
				}
				for (int i = 0; i < this.m_rowIndexes.Count; i++)
				{
					for (int j = 0; j < this.m_colIndexes.Count; j++)
					{
						int index = this.m_rowIndexes[i];
						int index2 = this.m_colIndexes[j];
						Cell cell = rows[index].Cells[index2];
						List<int> runningValueIndexes = cell.RunningValueIndexes;
						if (runningValueIndexes != null)
						{
							flag = true;
							AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] array = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[runningValueIndexes.Count];
							this.m_runningValueValues[i, j] = array;
							for (int k = 0; k < runningValueIndexes.Count; k++)
							{
								AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValueInfo = cellRunningValues[runningValueIndexes[k]];
								AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = aggregatesImpl.GetAggregateObj(runningValueInfo.Name);
								array[k] = aggregateObj.AggregateResult();
							}
						}
						if (cell.DataScopeInfo != null)
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] array2 = null;
							RuntimeRICollection.StoreRunningValues(aggregatesImpl, cell.DataScopeInfo.RunningValuesOfAggregates, ref array2);
							if (array2 != null)
							{
								flag2 = true;
								this.m_runningValueOfAggregateValues[i, j] = array2;
							}
						}
					}
				}
			}
			if (!flag)
			{
				this.m_runningValueValues = null;
			}
			if (!flag2)
			{
				this.m_runningValueOfAggregateValues = null;
			}
		}

		protected virtual void CalculateInnerRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
		}

		private void CalculatePreviousAggregates()
		{
			this.SetupEnvironment();
			((IScope)this).CalculatePreviousAggregates();
		}

		public void ReadRows(DataActions action, ITraversalContext context)
		{
			for (int i = 0; i < this.m_dataRows.Count; i++)
			{
				DataFieldRow dataFieldRow = this.m_dataRows[i];
				dataFieldRow.SetFields(this.m_owner.Value().OdpContext.ReportObjectModel.FieldsImpl);
				this.ReadRow(action, context);
			}
		}

		protected void SetupAggregates(List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> aggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] aggValues)
		{
			if (aggregates != null)
			{
				for (int i = 0; i < aggregates.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj dataAggregateObj = aggregates[i];
					this.m_owner.Value().OdpContext.ReportObjectModel.AggregatesImpl.Set(dataAggregateObj.Name, dataAggregateObj.AggregateDef, dataAggregateObj.DuplicateNames, (aggValues == null) ? dataAggregateObj.AggregateResult() : aggValues[i]);
				}
			}
		}

		public void SetupEnvironment()
		{
			this.SetupAggregates(this.m_cellNonCustomAggObjs, null);
			this.SetupAggregates(this.m_cellCustomAggObjs, null);
			RuntimeDataRegionObj.SetupAggregates(this.m_owner.Value().OdpContext, this.m_cellAggregatesOfAggregates);
			RuntimeDataRegionObj.SetupAggregates(this.m_owner.Value().OdpContext, this.m_cellPostSortAggregatesOfAggregates);
			OnDemandProcessingContext odpContext = this.m_owner.Value().OdpContext;
			if (this.m_cellAggValueList != null)
			{
				using (this.m_owner.PinValue())
				{
					this.SetupAggregates(this.m_owner.Value().CellPostSortAggregates, this.m_cellAggValueList);
				}
			}
			if (this.m_canonicalCellScopeDef != null && this.m_canonicalCellScopeDef.DataScopeInfo != null && this.m_canonicalCellScopeDef.DataScopeInfo.DataSet != null && this.m_canonicalCellScopeDef.DataScopeInfo.DataSet.DataSetCore.FieldsContext != null)
			{
				odpContext.ReportObjectModel.RestoreFields(this.m_canonicalCellScopeDef.DataScopeInfo.DataSet.DataSetCore.FieldsContext);
			}
			if (this.m_firstRow != null)
			{
				this.m_firstRow.SetFields(odpContext.ReportObjectModel.FieldsImpl);
			}
			else
			{
				odpContext.ReportObjectModel.ResetFieldValues();
			}
			odpContext.ReportRuntime.CurrentScope = this;
		}

		public abstract IOnDemandMemberOwnerInstanceReference GetDataRegionInstance(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion);

		public abstract IReference<IDataCorrelation> GetIdcReceiver(IRIFReportDataScope scope);

		internal virtual void CreateInstance(IMemberHierarchy dataRegionOrRowMemberInstance, int columnMemberSequenceId)
		{
			this.SetupEnvironment();
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = this.m_owner.Value().DataRegionDef;
			OnDemandProcessingContext odpContext = this.m_owner.Value().OdpContext;
			for (int i = 0; i < this.m_rowIndexes.Count; i++)
			{
				for (int j = 0; j < this.m_colIndexes.Count; j++)
				{
					int index = this.m_rowIndexes[i];
					int index2 = this.m_colIndexes[j];
					Cell cell = dataRegionDef.Rows[index].Cells[index2];
					if (cell != null)
					{
						DataCellInstance dataCellInstance = null;
						if (cell.SimpleGroupTreeCell)
						{
							if (this.m_firstRow != null && cell.InDynamicRowAndColumnContext)
							{
								dataCellInstance = DataCellInstance.CreateInstance(dataRegionOrRowMemberInstance, odpContext, cell, this.m_firstRow.StreamOffset, columnMemberSequenceId);
							}
						}
						else
						{
							dataCellInstance = DataCellInstance.CreateInstance(dataRegionOrRowMemberInstance, odpContext, cell, (this.m_runningValueValues != null) ? this.m_runningValueValues[i, j] : null, (this.m_runningValueOfAggregateValues != null) ? this.m_runningValueOfAggregateValues[i, j] : null, (this.m_firstRow != null) ? this.m_firstRow.StreamOffset : 0, columnMemberSequenceId);
						}
						if (dataCellInstance != null)
						{
							if (!cell.SimpleGroupTreeCell)
							{
								this.CreateInstanceCellContents(cell, dataCellInstance, odpContext);
							}
							dataCellInstance.InstanceComplete();
						}
						if (cell.InScopeEventSources != null)
						{
							UserSortFilterContext.ProcessEventSources(odpContext, this, cell.InScopeEventSources);
						}
					}
				}
			}
		}

		private Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping> GetOuterScopes()
		{
			Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping> dictionary = null;
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = this.m_owner.Value().DataRegionDef;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode memberDef = this.m_owner.Value().MemberDef;
			if (memberDef.CellScopes == null)
			{
				memberDef.CellScopes = new Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping>[dataRegionDef.OuterGroupingDynamicMemberCount];
			}
			dictionary = memberDef.CellScopes[this.m_outerGroupDynamicIndex];
			if (dictionary == null)
			{
				IReference<RuntimeDataTablixGroupRootObj> reference = dataRegionDef.CurrentOuterGroupRootObjs[this.m_outerGroupDynamicIndex];
				Global.Tracer.Assert(null != reference, "(null != outerGroupRoot)");
				AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode hierarchyDef = reference.Value().HierarchyDef;
				Global.Tracer.Assert(null != hierarchyDef, "(null != outerGrouping)");
				dictionary = hierarchyDef.GetScopeNames();
				memberDef.CellScopes[this.m_outerGroupDynamicIndex] = dictionary;
			}
			return dictionary;
		}

		bool IScope.IsTargetForSort(int index, bool detailSort)
		{
			return this.m_owner.Value().GetCellTargetForSort(index, detailSort);
		}

		string IScope.GetScopeName()
		{
			return null;
		}

		IReference<IScope> IScope.GetOuterScope(bool includeSubReportContainingScope)
		{
			return this.m_owner;
		}

		public void ReadRow(DataActions dataAction, ITraversalContext context)
		{
			if (FlagUtils.HasFlag(dataAction, DataActions.PostSortAggregatesOfAggregates) || FlagUtils.HasFlag(dataAction, DataActions.AggregatesOfAggregates))
			{
				AggregateUpdateContext aggregateUpdateContext = (AggregateUpdateContext)context;
				aggregateUpdateContext.UpdateAggregatesForRow();
			}
			if (FlagUtils.HasFlag(dataAction, DataActions.PostSortAggregates))
			{
				using (this.m_owner.PinValue())
				{
					this.m_owner.Value().ReadRow(DataActions.PostSortAggregates, context);
				}
			}
		}

		void IScope.CalculatePreviousAggregates()
		{
			using (this.m_owner.PinValue())
			{
				this.m_owner.Value().CalculatePreviousAggregates();
			}
		}

		bool IScope.InScope(string scope)
		{
			if (this.m_owner.Value().InScope(scope))
			{
				return true;
			}
			Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping> outerScopes = this.GetOuterScopes();
			if (outerScopes != null && outerScopes.Count != 0)
			{
				return outerScopes.ContainsKey(scope);
			}
			return false;
		}

		int IScope.RecursiveLevel(string scope)
		{
			if (scope == null)
			{
				return 0;
			}
			int num = ((IScope)this.m_owner).RecursiveLevel(scope);
			if (-1 != num)
			{
				return num;
			}
			Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping> outerScopes = this.GetOuterScopes();
			if (outerScopes != null && outerScopes.Count != 0)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = default(AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping);
				if (outerScopes.TryGetValue(scope, out grouping))
				{
					return grouping.RecursiveLevel;
				}
				return -1;
			}
			return -1;
		}

		bool IScope.TargetScopeMatched(int index, bool detailSort)
		{
			if (!this.m_owner.Value().TargetScopeMatched(index, detailSort))
			{
				return false;
			}
			Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping> outerScopes = this.GetOuterScopes();
			IDictionaryEnumerator dictionaryEnumerator = (IDictionaryEnumerator)(object)outerScopes.GetEnumerator();
			while (dictionaryEnumerator.MoveNext())
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = (AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping)dictionaryEnumerator.Value;
				if ((!detailSort || grouping.SortFilterScopeInfo != null) && grouping.SortFilterScopeMatched != null && !grouping.SortFilterScopeMatched[index])
				{
					return false;
				}
			}
			if (detailSort)
			{
				return true;
			}
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = this.m_owner.Value().DataRegionDef;
			IReference<RuntimeSortFilterEventInfo> reference = this.m_owner.Value().OdpContext.RuntimeSortFilterInfo[index];
			using (reference.PinValue())
			{
				RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
				List<object>[] sortSourceScopeInfo = runtimeSortFilterEventInfo.SortSourceScopeInfo;
				AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping groupingDef = this.m_owner.Value().GroupingDef;
				if (groupingDef.SortFilterScopeIndex != null && -1 != groupingDef.SortFilterScopeIndex[index])
				{
					int num = groupingDef.SortFilterScopeIndex[index] + 1;
					if (!this.m_innermost)
					{
						int innerGroupingMaximumDynamicLevel = dataRegionDef.InnerGroupingMaximumDynamicLevel;
						int num2 = this.m_owner.Value().HeadingLevel + 1;
						while (num2 < innerGroupingMaximumDynamicLevel && num < sortSourceScopeInfo.Length)
						{
							if (sortSourceScopeInfo[num] != null)
							{
								return false;
							}
							num2++;
							num++;
						}
					}
				}
				Global.Tracer.Assert(null != dataRegionDef.CurrentOuterGroupRootObjs[this.m_outerGroupDynamicIndex], "(null != dataRegionDef.CurrentOuterGroupRootObjs[m_cellLevel])");
				if (this.m_outerGroupDynamicIndex + 1 < dataRegionDef.OuterGroupingDynamicMemberCount)
				{
					IReference<RuntimeDataTablixGroupRootObj> reference2 = dataRegionDef.CurrentOuterGroupRootObjs[this.m_outerGroupDynamicIndex + 1];
					AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode reportHierarchyNode = (reference2 != null) ? reference2.Value().HierarchyDef : null;
					if (reportHierarchyNode != null && reportHierarchyNode.Grouping.SortFilterScopeIndex != null && -1 != reportHierarchyNode.Grouping.SortFilterScopeIndex[index])
					{
						int outerGroupingMaximumDynamicLevel = dataRegionDef.OuterGroupingMaximumDynamicLevel;
						int num = reportHierarchyNode.Grouping.SortFilterScopeIndex[index];
						int num3 = this.m_outerGroupDynamicIndex + 1;
						while (num3 < outerGroupingMaximumDynamicLevel && num < sortSourceScopeInfo.Length)
						{
							if (sortSourceScopeInfo[num] != null)
							{
								return false;
							}
							num3++;
							num++;
						}
					}
				}
			}
			return true;
		}

		void IScope.GetScopeValues(IReference<IHierarchyObj> targetScopeObj, List<object>[] scopeValues, ref int index)
		{
			((RuntimeDataRegionObj)this.m_owner.Value()).GetScopeValues(targetScopeObj, scopeValues, ref index);
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = this.m_owner.Value().DataRegionDef;
			if (!this.m_innermost)
			{
				int innerGroupingMaximumDynamicLevel = dataRegionDef.InnerGroupingMaximumDynamicLevel;
				for (int i = this.m_owner.Value().HeadingLevel + 1; i < innerGroupingMaximumDynamicLevel; i++)
				{
					if (index >= scopeValues.Length)
					{
						break;
					}
					scopeValues[index++] = null;
				}
			}
			Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping> outerScopes = this.GetOuterScopes();
			IDictionaryEnumerator dictionaryEnumerator = (IDictionaryEnumerator)(object)outerScopes.GetEnumerator();
			while (dictionaryEnumerator.MoveNext())
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = (AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping)dictionaryEnumerator.Value;
				if (index < scopeValues.Length)
				{
					Global.Tracer.Assert(index < scopeValues.Length, "Inner groupings");
					scopeValues[index++] = grouping.CurrentGroupExpressionValues;
				}
			}
			int outerGroupingMaximumDynamicLevel = dataRegionDef.OuterGroupingMaximumDynamicLevel;
			for (int j = outerScopes.Count; j < outerGroupingMaximumDynamicLevel; j++)
			{
				if (index >= scopeValues.Length)
				{
					break;
				}
				scopeValues[index++] = null;
			}
		}

		void IScope.GetGroupNameValuePairs(Dictionary<string, object> pairs)
		{
			((IScope)this.m_owner).GetGroupNameValuePairs(pairs);
			Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping> outerScopes = this.GetOuterScopes();
			if (outerScopes == null && outerScopes.Count == 0)
			{
				return;
			}
			IEnumerator enumerator = (IEnumerator)(object)outerScopes.Values.GetEnumerator();
			while (enumerator.MoveNext())
			{
				RuntimeDataRegionObj.AddGroupNameValuePair(this.m_owner.Value().OdpContext, enumerator.Current as AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping, pairs);
			}
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(RuntimeCell.m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Owner:
					writer.Write(this.m_owner);
					break;
				case MemberName.OuterGroupDynamicIndex:
					writer.Write(this.m_outerGroupDynamicIndex);
					break;
				case MemberName.RowIndexes:
					writer.WriteListOfPrimitives(this.m_rowIndexes);
					break;
				case MemberName.ColumnIndexes:
					writer.WriteListOfPrimitives(this.m_colIndexes);
					break;
				case MemberName.CellNonCustomAggObjs:
					writer.Write(this.m_cellNonCustomAggObjs);
					break;
				case MemberName.CellCustomAggObjs:
					writer.Write(this.m_cellCustomAggObjs);
					break;
				case MemberName.CellAggValueList:
					writer.Write(this.m_cellAggValueList);
					break;
				case MemberName.RunningValueValues:
					writer.Write(this.m_runningValueValues);
					break;
				case MemberName.RunningValueOfAggregateValues:
					writer.Write(this.m_runningValueOfAggregateValues);
					break;
				case MemberName.DataRows:
					writer.Write(this.m_dataRows);
					break;
				case MemberName.Innermost:
					writer.Write(this.m_innermost);
					break;
				case MemberName.FirstRow:
					writer.Write(this.m_firstRow);
					break;
				case MemberName.FirstRowIsAggregate:
					writer.Write(this.m_firstRowIsAggregate);
					break;
				case MemberName.NextCell:
					writer.Write(this.m_nextCell);
					break;
				case MemberName.SortFilterExpressionScopeInfoIndices:
					writer.Write(this.m_sortFilterExpressionScopeInfoIndices);
					break;
				case MemberName.AggregatesOfAggregates:
					writer.Write(this.m_cellAggregatesOfAggregates);
					break;
				case MemberName.PostSortAggregatesOfAggregates:
					writer.Write(this.m_cellPostSortAggregatesOfAggregates);
					break;
				case MemberName.CanonicalCellScope:
				{
					int value = scalabilityCache.StoreStaticReference(this.m_canonicalCellScopeDef);
					writer.Write(value);
					break;
				}
				case MemberName.DataAction:
					writer.WriteEnum((int)this.m_dataAction);
					break;
				case MemberName.ScopeInstanceNumber:
					writer.Write(this.m_scopeInstanceNumber);
					break;
				case MemberName.HasProcessedAggregateRow:
					writer.Write(this.m_hasProcessedAggregateRow);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(RuntimeCell.m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Owner:
					this.m_owner = (RuntimeDataTablixGroupLeafObjReference)reader.ReadRIFObject();
					break;
				case MemberName.OuterGroupDynamicIndex:
					this.m_outerGroupDynamicIndex = reader.ReadInt32();
					break;
				case MemberName.RowIndexes:
					this.m_rowIndexes = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.ColumnIndexes:
					this.m_colIndexes = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.CellNonCustomAggObjs:
					this.m_cellNonCustomAggObjs = reader.ReadListOfRIFObjects<List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.CellCustomAggObjs:
					this.m_cellCustomAggObjs = reader.ReadListOfRIFObjects<List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.CellAggValueList:
					this.m_cellAggValueList = reader.ReadArrayOfRIFObjects<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult>();
					break;
				case MemberName.RunningValueValues:
					this.m_runningValueValues = reader.Read2DArrayOfArrayOfRIFObjects<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult>();
					break;
				case MemberName.RunningValueOfAggregateValues:
					this.m_runningValueOfAggregateValues = reader.Read2DArrayOfArrayOfRIFObjects<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult>();
					break;
				case MemberName.DataRows:
					this.m_dataRows = reader.ReadRIFObject<ScalableList<DataFieldRow>>();
					break;
				case MemberName.Innermost:
					this.m_innermost = reader.ReadBoolean();
					break;
				case MemberName.FirstRow:
					this.m_firstRow = (DataFieldRow)reader.ReadRIFObject();
					break;
				case MemberName.FirstRowIsAggregate:
					this.m_firstRowIsAggregate = reader.ReadBoolean();
					break;
				case MemberName.NextCell:
					this.m_nextCell = reader.ReadInt32();
					break;
				case MemberName.SortFilterExpressionScopeInfoIndices:
					this.m_sortFilterExpressionScopeInfoIndices = reader.ReadInt32Array();
					break;
				case MemberName.AggregatesOfAggregates:
					this.m_cellAggregatesOfAggregates = (BucketedDataAggregateObjs)reader.ReadRIFObject();
					break;
				case MemberName.PostSortAggregatesOfAggregates:
					this.m_cellPostSortAggregatesOfAggregates = (BucketedDataAggregateObjs)reader.ReadRIFObject();
					break;
				case MemberName.CanonicalCellScope:
				{
					int id = reader.ReadInt32();
					this.m_canonicalCellScopeDef = (Cell)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.DataAction:
					this.m_dataAction = (DataActions)reader.ReadEnum();
					break;
				case MemberName.ScopeInstanceNumber:
					this.m_scopeInstanceNumber = reader.ReadInt64();
					break;
				case MemberName.HasProcessedAggregateRow:
					this.m_hasProcessedAggregateRow = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public virtual AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCell;
		}

		internal static Declaration GetDeclaration()
		{
			if (RuntimeCell.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Owner, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupLeafObjReference));
				list.Add(new MemberInfo(MemberName.OuterGroupDynamicIndex, Token.Int32));
				list.Add(new MemberInfo(MemberName.RowIndexes, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
				list.Add(new MemberInfo(MemberName.ColumnIndexes, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
				list.Add(new MemberInfo(MemberName.CellNonCustomAggObjs, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.CellCustomAggObjs, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.CellAggValueList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjResult));
				list.Add(new MemberInfo(MemberName.RunningValueValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Array2D, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray));
				list.Add(new MemberInfo(MemberName.DataRows, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
				list.Add(new MemberInfo(MemberName.Innermost, Token.Boolean));
				list.Add(new MemberInfo(MemberName.FirstRow, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRow));
				list.Add(new MemberInfo(MemberName.FirstRowIsAggregate, Token.Boolean));
				list.Add(new MemberInfo(MemberName.NextCell, Token.Int32));
				list.Add(new MemberInfo(MemberName.SortFilterExpressionScopeInfoIndices, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Int32));
				list.Add(new MemberInfo(MemberName.AggregatesOfAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BucketedDataAggregateObjs));
				list.Add(new MemberInfo(MemberName.PostSortAggregatesOfAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BucketedDataAggregateObjs));
				list.Add(new MemberInfo(MemberName.CanonicalCellScope, Token.Int32));
				list.Add(new MemberInfo(MemberName.DataAction, Token.Enum));
				list.Add(new MemberInfo(MemberName.RunningValueOfAggregateValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Array2D, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray));
				list.Add(new MemberInfo(MemberName.ScopeInstanceNumber, Token.Int64));
				list.Add(new MemberInfo(MemberName.HasProcessedAggregateRow, Token.Boolean));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCell, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return RuntimeCell.m_declaration;
		}

		public void SetReference(IReference selfRef)
		{
			this.m_selfReference = (RuntimeCellReference)selfRef;
		}
	}
}
