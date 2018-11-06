using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CustomReportItem : Tablix, IRunningValueHolder, IErrorContext
	{
		private string m_type;

		private ReportItemCollection m_altReportItem;

		private CustomReportItemHeadingList m_columns;

		private CustomReportItemHeadingList m_rows;

		private DataCellsList m_dataRowCells;

		private RunningValueInfoList m_cellRunningValues;

		private IntList m_cellExprHostIDs;

		private ReportItemCollection m_renderReportItem;

		[NonSerialized]
		private int m_expectedColumns;

		[NonSerialized]
		private int m_expectedRows;

		[NonSerialized]
		private CustomReportItemExprHost m_exprHost;

		[NonSerialized]
		private CustomReportItemHeadingList m_staticColumns;

		[NonSerialized]
		private bool m_staticColumnsInitialized;

		[NonSerialized]
		private CustomReportItemHeadingList m_staticRows;

		[NonSerialized]
		private bool m_staticRowsInitialized;

		[NonSerialized]
		private CustomReportItemInstance m_criInstance;

		[NonSerialized]
		private CustomReportItemInstanceInfo m_criInstanceInfo;

		[NonSerialized]
		private ReportProcessing.ProcessingContext m_processingContext;

		[NonSerialized]
		private int m_repeatedSiblingIndex = -1;

		[NonSerialized]
		private ObjectType m_customObjectType;

		[NonSerialized]
		private string m_customObjectName;

		[NonSerialized]
		private string m_customPropertyName;

		[NonSerialized]
		private string m_customTopLevelRenderItemName;

		[NonSerialized]
		private bool m_firstInstance = true;

		internal override ObjectType ObjectType
		{
			get
			{
				return ObjectType.CustomReportItem;
			}
		}

		internal override TablixHeadingList TablixColumns
		{
			get
			{
				return this.m_columns;
			}
		}

		internal override TablixHeadingList TablixRows
		{
			get
			{
				return this.m_rows;
			}
		}

		internal override RunningValueInfoList TablixCellRunningValues
		{
			get
			{
				return this.m_cellRunningValues;
			}
		}

		internal string Type
		{
			get
			{
				return this.m_type;
			}
			set
			{
				this.m_type = value;
			}
		}

		internal ReportItemCollection AltReportItem
		{
			get
			{
				return this.m_altReportItem;
			}
			set
			{
				this.m_altReportItem = value;
			}
		}

		internal CustomReportItemHeadingList Columns
		{
			get
			{
				return this.m_columns;
			}
			set
			{
				this.m_columns = value;
			}
		}

		internal CustomReportItemHeadingList Rows
		{
			get
			{
				return this.m_rows;
			}
			set
			{
				this.m_rows = value;
			}
		}

		internal DataCellsList DataRowCells
		{
			get
			{
				return this.m_dataRowCells;
			}
			set
			{
				this.m_dataRowCells = value;
			}
		}

		internal RunningValueInfoList CellRunningValues
		{
			get
			{
				return this.m_cellRunningValues;
			}
			set
			{
				this.m_cellRunningValues = value;
			}
		}

		internal IntList CellExprHostIDs
		{
			get
			{
				return this.m_cellExprHostIDs;
			}
			set
			{
				this.m_cellExprHostIDs = value;
			}
		}

		internal int ExpectedColumns
		{
			get
			{
				return this.m_expectedColumns;
			}
			set
			{
				this.m_expectedColumns = value;
			}
		}

		internal int ExpectedRows
		{
			get
			{
				return this.m_expectedRows;
			}
			set
			{
				this.m_expectedRows = value;
			}
		}

		internal CustomReportItemHeadingList StaticColumns
		{
			get
			{
				if (!this.m_staticColumnsInitialized)
				{
					this.InitializeStaticGroups(false);
					this.m_staticColumnsInitialized = true;
				}
				return this.m_staticColumns;
			}
		}

		internal CustomReportItemHeadingList StaticRows
		{
			get
			{
				if (!this.m_staticRowsInitialized)
				{
					this.InitializeStaticGroups(true);
					this.m_staticRowsInitialized = true;
				}
				return this.m_staticRows;
			}
		}

		internal ReportItemCollection RenderReportItem
		{
			get
			{
				return this.m_renderReportItem;
			}
			set
			{
				this.m_renderReportItem = value;
			}
		}

		internal bool FirstInstanceOfRenderReportItem
		{
			get
			{
				return this.m_firstInstance;
			}
			set
			{
				this.m_firstInstance = value;
			}
		}

		internal ReportProcessing.ProcessingContext ProcessingContext
		{
			get
			{
				return this.m_processingContext;
			}
			set
			{
				this.m_processingContext = value;
			}
		}

		internal ObjectType CustomObjectType
		{
			get
			{
				return this.m_customObjectType;
			}
			set
			{
				this.m_customObjectType = value;
			}
		}

		internal string CustomObjectName
		{
			get
			{
				return this.m_customObjectName;
			}
			set
			{
				this.m_customObjectName = value;
			}
		}

		protected override DataRegionExprHost DataRegionExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal CustomReportItem(ReportItem parent)
			: base(parent)
		{
		}

		internal CustomReportItem(int id, int idAltReportitem, ReportItem parent)
			: base(id, parent)
		{
			this.m_dataRowCells = new DataCellsList();
			this.m_cellRunningValues = new RunningValueInfoList();
			this.m_altReportItem = new ReportItemCollection(idAltReportitem, false);
		}

		RunningValueInfoList IRunningValueHolder.GetRunningValueList()
		{
			return base.m_runningValues;
		}

		void IRunningValueHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(null != this.m_cellRunningValues);
			if (this.m_cellRunningValues.Count == 0)
			{
				this.m_cellRunningValues = null;
			}
			Global.Tracer.Assert(null != base.m_runningValues);
			if (base.m_runningValues.Count == 0)
			{
				base.m_runningValues = null;
			}
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = this.ObjectType;
			context.ObjectName = base.m_name;
			context.ExprHostBuilder.CustomReportItemStart(base.m_name);
			if (base.m_dataSetName != null)
			{
				context.RegisterDataRegion(this);
			}
			base.Initialize(context);
			if (this.m_altReportItem != null)
			{
				if (this.m_altReportItem.Count == 0)
				{
					this.m_altReportItem = null;
				}
				else
				{
					context.RegisterReportItems(this.m_altReportItem);
					this.m_altReportItem.Initialize(context, false);
					context.UnRegisterReportItems(this.m_altReportItem);
				}
			}
			context.RegisterRunningValues(base.m_runningValues);
			if (base.m_visibility != null)
			{
				base.m_visibility.Initialize(context, true, false);
			}
			if (base.m_dataSetName != null)
			{
				this.CustomInitialize(context);
				context.UnRegisterDataRegion(this);
			}
			if (base.m_visibility != null)
			{
				base.m_visibility.UnRegisterReceiver(context);
			}
			context.UnRegisterRunningValues(base.m_runningValues);
			base.ExprHostID = context.ExprHostBuilder.CustomReportItemEnd();
			return false;
		}

		private void CustomInitialize(InitializationContext context)
		{
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.Location &= ~LocationFlags.InMatrixCellTopLevelItem;
			Global.Tracer.Assert(this.m_columns == null || this.m_expectedColumns > 0);
			Global.Tracer.Assert(this.m_rows == null || this.m_expectedRows > 0);
			if (this.ValidateRDLStructure(context) && this.ValidateProcessingRestrictions(context))
			{
				context.AggregateEscalateScopes = new StringList();
				context.AggregateEscalateScopes.Add(base.m_name);
				if (this.m_columns != null)
				{
					int num = 0;
					int num2 = 0;
					this.m_expectedColumns += this.m_columns.Initialize(0, this.m_dataRowCells, ref num, ref num2, context);
					base.ColumnCount = num2 + 1;
					if (1 == base.ColumnCount && this.m_columns[0].Static)
					{
						for (int i = 0; i < this.m_columns.Count; i++)
						{
							context.SpecialTransferRunningValues(this.m_columns[i].RunningValues);
						}
					}
				}
				if (this.m_rows != null)
				{
					int num3 = 0;
					int num4 = 0;
					this.m_expectedRows += this.m_rows.Initialize(0, this.m_dataRowCells, ref num3, ref num4, context);
					base.RowCount = num4 + 1;
					if (1 == base.RowCount && this.m_rows[0].Static)
					{
						for (int j = 0; j < this.m_rows.Count; j++)
						{
							context.SpecialTransferRunningValues(this.m_rows[j].RunningValues);
						}
					}
				}
				context.AggregateEscalateScopes = null;
				context.AggregateRewriteScopes = null;
				this.DataCellInitialize(context);
				this.CopyHeadingAggregates(this.m_rows);
				this.m_rows.TransferHeadingAggregates();
				this.CopyHeadingAggregates(this.m_columns);
				this.m_columns.TransferHeadingAggregates();
			}
		}

		private void InitializeStaticGroups(bool isRows)
		{
			Global.Tracer.Assert(this.m_rows != null && null != this.m_columns);
			CustomReportItemHeadingList customReportItemHeadingList = isRows ? this.m_rows : this.m_columns;
			int num = 0;
			while (true)
			{
				if (customReportItemHeadingList != null)
				{
					num++;
					if (!customReportItemHeadingList[0].Static)
					{
						customReportItemHeadingList = (CustomReportItemHeadingList)customReportItemHeadingList.InnerHeadings();
						continue;
					}
					break;
				}
				return;
			}
			if (isRows)
			{
				this.m_staticRows = customReportItemHeadingList;
			}
			else
			{
				this.m_staticColumns = customReportItemHeadingList;
			}
			Global.Tracer.Assert(null == customReportItemHeadingList.InnerHeadings());
		}

		private bool ValidateProcessingRestrictions(InitializationContext context)
		{
			if (this.m_expectedColumns * this.m_expectedRows == 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsMissingDataGroupings, Severity.Error, context.ObjectType, context.ObjectName, (this.m_expectedColumns == 0) ? "DataColumnGroupings" : "DataRowGroupings");
				return false;
			}
			if (this.m_dataRowCells != null && this.m_dataRowCells.Count != 0)
			{
				Global.Tracer.Assert(this.m_rows != null && null != this.m_columns);
				if (CustomReportItemHeading.ValidateProcessingRestrictions(this.m_rows, false, false, context))
				{
					return CustomReportItemHeading.ValidateProcessingRestrictions(this.m_columns, true, false, context);
				}
				return false;
			}
			context.ErrorContext.Register(ProcessingErrorCode.rsMissingDataCells, Severity.Error, context.ObjectType, context.ObjectName, "DataRows");
			return false;
		}

		private bool ValidateRDLStructure(InitializationContext context)
		{
			if (this.m_dataRowCells == null || this.m_dataRowCells.Count == 0)
			{
				if (this.m_expectedColumns * this.m_expectedRows != 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfDataRows, Severity.Error, context.ObjectType, context.ObjectName, this.m_expectedRows.ToString(CultureInfo.InvariantCulture.NumberFormat));
					return false;
				}
			}
			else
			{
				if (this.m_expectedRows == 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsMissingDataGrouping, Severity.Error, context.ObjectType, context.ObjectName, "DataRowGroupings");
				}
				if (this.m_expectedColumns == 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsMissingDataGrouping, Severity.Error, context.ObjectType, context.ObjectName, "DataColumnGroupings");
				}
				if (this.m_expectedRows * this.m_expectedColumns == 0)
				{
					return false;
				}
				int count = this.m_dataRowCells.Count;
				if (this.m_expectedRows != count)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfDataRows, Severity.Error, context.ObjectType, context.ObjectName, this.m_expectedRows.ToString(CultureInfo.InvariantCulture.NumberFormat));
					return false;
				}
				bool flag = false;
				for (int i = 0; i < count; i++)
				{
					if (flag)
					{
						break;
					}
					Global.Tracer.Assert(null != this.m_dataRowCells[i]);
					if (this.m_dataRowCells[i].Count != this.m_expectedColumns)
					{
						flag = true;
					}
				}
				if (flag)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfDataCellsInDataRow, Severity.Error, context.ObjectType, context.ObjectName, this.m_expectedColumns.ToString(CultureInfo.InvariantCulture.NumberFormat));
					return false;
				}
			}
			return true;
		}

		private void DataCellInitialize(InitializationContext context)
		{
			context.Location |= LocationFlags.InMatrixCell;
			context.MatrixName = base.m_name;
			context.RegisterTablixCellScope(1 == this.m_expectedColumns && null == this.m_columns[0].Grouping, base.m_cellAggregates, base.m_cellPostSortAggregates);
			Global.Tracer.Assert(0 != this.m_expectedColumns * this.m_expectedRows);
			this.m_cellExprHostIDs = new IntList(this.m_expectedColumns * this.m_expectedRows);
			context.RegisterRunningValues(this.m_cellRunningValues);
			this.SetupRowScopesAndInitialize(this.m_rows, 0, context);
			context.UnRegisterRunningValues(this.m_cellRunningValues);
			if (context.IsRunningValueDirectionColumn())
			{
				base.m_processingInnerGrouping = Pivot.ProcessingInnerGroupings.Row;
			}
			context.UnRegisterTablixCellScope();
		}

		private void SetupRowScopesAndInitialize(CustomReportItemHeadingList rowHeadings, int cellRowIndex, InitializationContext context)
		{
			Global.Tracer.Assert(null != rowHeadings);
			int count = rowHeadings.Count;
			for (int i = 0; i < count; i++)
			{
				CustomReportItemHeading customReportItemHeading = rowHeadings[i];
				if (customReportItemHeading.Grouping != null)
				{
					context.Location |= LocationFlags.InGrouping;
					context.RegisterGroupingScopeForTablixCell(customReportItemHeading.Grouping.Name, false, customReportItemHeading.Grouping.SimpleGroupExpressions, customReportItemHeading.Aggregates, customReportItemHeading.PostSortAggregates, customReportItemHeading.RecursiveAggregates, customReportItemHeading.Grouping);
				}
				if (customReportItemHeading.InnerHeadings != null)
				{
					this.SetupRowScopesAndInitialize(customReportItemHeading.InnerHeadings, cellRowIndex, context);
				}
				else
				{
					Global.Tracer.Assert(this.m_dataRowCells != null && cellRowIndex < this.m_dataRowCells.Count);
					int num = 0;
					this.SetupColumnScopesAndInitialize(this.m_columns, this.m_dataRowCells[cellRowIndex], ref num, context);
					Global.Tracer.Assert(num == this.m_dataRowCells[cellRowIndex].Count);
					cellRowIndex++;
				}
				if (customReportItemHeading.Grouping != null)
				{
					context.UnRegisterGroupingScopeForTablixCell(customReportItemHeading.Grouping.Name, false);
				}
			}
		}

		private void SetupColumnScopesAndInitialize(CustomReportItemHeadingList columnHeadings, DataCellList cellList, ref int cellIndex, InitializationContext context)
		{
			Global.Tracer.Assert(null != columnHeadings);
			int count = columnHeadings.Count;
			for (int i = 0; i < count; i++)
			{
				CustomReportItemHeading customReportItemHeading = columnHeadings[i];
				if (customReportItemHeading.Grouping != null)
				{
					context.Location |= LocationFlags.InGrouping;
					context.RegisterGroupingScopeForTablixCell(customReportItemHeading.Grouping.Name, true, customReportItemHeading.Grouping.SimpleGroupExpressions, customReportItemHeading.Aggregates, customReportItemHeading.PostSortAggregates, customReportItemHeading.RecursiveAggregates, customReportItemHeading.Grouping);
				}
				if (customReportItemHeading.InnerHeadings != null)
				{
					this.SetupColumnScopesAndInitialize(customReportItemHeading.InnerHeadings, cellList, ref cellIndex, context);
				}
				else
				{
					context.ExprHostBuilder.DataCellStart();
					cellList[cellIndex].Initialize(null, context);
					this.m_cellExprHostIDs.Add(context.ExprHostBuilder.DataCellEnd());
					cellIndex++;
				}
				if (customReportItemHeading.Grouping != null)
				{
					context.UnRegisterGroupingScopeForTablixCell(customReportItemHeading.Grouping.Name, true);
				}
			}
		}

		internal void CopyHeadingAggregates(CustomReportItemHeadingList headings)
		{
			if (headings != null)
			{
				int count = headings.Count;
				for (int i = 0; i < count; i++)
				{
					CustomReportItemHeading customReportItemHeading = headings[i];
					customReportItemHeading.CopySubHeadingAggregates();
					Tablix.CopyAggregates(customReportItemHeading.Aggregates, base.m_aggregates);
					Tablix.CopyAggregates(customReportItemHeading.PostSortAggregates, base.m_postSortAggregates);
					Tablix.CopyAggregates(customReportItemHeading.RecursiveAggregates, base.m_aggregates);
				}
			}
		}

		internal override int GetDynamicHeadingCount(bool outerGroupings)
		{
			if (outerGroupings && base.m_processingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
			{
				goto IL_0017;
			}
			if (!outerGroupings && base.m_processingInnerGrouping == Pivot.ProcessingInnerGroupings.Row)
			{
				goto IL_0017;
			}
			int num = base.ColumnCount;
			if (this.StaticColumns != null)
			{
				num--;
			}
			goto IL_003f;
			IL_0017:
			num = base.RowCount;
			if (this.StaticRows != null)
			{
				num--;
			}
			goto IL_003f;
			IL_003f:
			return num;
		}

		internal override TablixHeadingList SkipStatics(TablixHeadingList headings)
		{
			Global.Tracer.Assert(headings != null && 1 <= headings.Count && headings is CustomReportItemHeadingList);
			if (((CustomReportItemHeadingList)headings)[0].Static)
			{
				return null;
			}
			return headings;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				this.m_exprHost = reportExprHost.CustomReportItemHostsRemotable[base.ExprHostID];
				base.DataRegionSetExprHost(this.m_exprHost, reportObjectModel);
				if (this.m_exprHost.DataCellHostsRemotable != null)
				{
					IList<DataCellExprHost> dataCellHostsRemotable = this.m_exprHost.DataCellHostsRemotable;
					int num = 0;
					Global.Tracer.Assert(this.m_dataRowCells.Count <= this.m_cellExprHostIDs.Count);
					for (int i = 0; i < this.m_dataRowCells.Count; i++)
					{
						DataCellList dataCellList = this.m_dataRowCells[i];
						Global.Tracer.Assert(null != dataCellList);
						for (int j = 0; j < dataCellList.Count; j++)
						{
							DataValueList dataValueList = dataCellList[j];
							Global.Tracer.Assert(null != dataValueList);
							int num3 = this.m_cellExprHostIDs[num++];
							if (num3 >= 0)
							{
								dataValueList.SetExprHost(dataCellHostsRemotable[num3].DataValueHostsRemotable, reportObjectModel);
							}
						}
					}
				}
			}
		}

		internal override Hashtable GetOuterScopeNames(int dynamicLevel)
		{
			Hashtable hashtable = new Hashtable();
			CustomReportItemHeadingList customReportItemHeadingList = (CustomReportItemHeadingList)base.GetOuterHeading();
			int num = 0;
			while (num <= dynamicLevel && customReportItemHeadingList != null)
			{
				if (customReportItemHeadingList[0].Grouping != null)
				{
					hashtable.Add(customReportItemHeadingList[0].Grouping.Name, customReportItemHeadingList[0].Grouping);
					num++;
				}
				customReportItemHeadingList = (CustomReportItemHeadingList)customReportItemHeadingList.InnerHeadings();
			}
			return hashtable;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Type, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.ReportItemColDef, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemCollection));
			memberInfoList.Add(new MemberInfo(MemberName.Columns, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.CustomReportItemHeadingList));
			memberInfoList.Add(new MemberInfo(MemberName.Rows, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.CustomReportItemHeadingList));
			memberInfoList.Add(new MemberInfo(MemberName.DataRowCells, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataCellsList));
			memberInfoList.Add(new MemberInfo(MemberName.CellRunningValues, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.RunningValueInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.CellExprHostIDs, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.IntList));
			memberInfoList.Add(new MemberInfo(MemberName.RenderReportItemColDef, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemCollection));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Tablix, memberInfoList);
		}

		internal void CustomProcessingInitialize(CustomReportItemInstance instance, CustomReportItemInstanceInfo instanceInfo, ReportProcessing.ProcessingContext context, int repeatedSiblingIndex)
		{
			this.m_criInstance = instance;
			this.m_criInstanceInfo = instanceInfo;
			this.m_processingContext = context;
			this.m_repeatedSiblingIndex = repeatedSiblingIndex;
			this.m_customObjectType = ObjectType.CustomReportItem;
			this.m_customObjectName = base.m_name;
			this.m_customPropertyName = null;
		}

		internal void CustomProcessingReset()
		{
			this.m_criInstance = null;
			this.m_criInstanceInfo = null;
			this.m_processingContext = null;
			this.m_repeatedSiblingIndex = -1;
			this.m_customObjectType = ObjectType.CustomReportItem;
			this.m_customObjectName = base.m_name;
			this.m_customPropertyName = null;
		}

		internal void DeconstructRenderItem(AspNetCore.ReportingServices.ReportRendering.ReportItem renderItem, CustomReportItemInstance criInstance)
		{
			if (this.FirstInstanceOfRenderReportItem)
			{
				this.m_renderReportItem = null;
				this.FirstInstanceOfRenderReportItem = false;
			}
			if (renderItem == null)
			{
				if (this.FirstInstanceOfRenderReportItem)
				{
					this.m_renderReportItem = null;
					this.m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemNull, Severity.Error, this.m_customObjectType, this.m_customObjectName, this.m_type);
					throw new ReportProcessingException(this.m_processingContext.ErrorContext.Messages);
				}
				this.m_renderReportItem = null;
				this.m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderInstanceNull, Severity.Error, this.m_customObjectType, this.m_customObjectName, this.m_type);
				throw new ReportProcessingException(this.m_processingContext.ErrorContext.Messages);
			}
			bool flag = null == this.m_renderReportItem;
			if (flag)
			{
				this.m_renderReportItem = new ReportItemCollection();
				this.m_renderReportItem.ID = this.m_processingContext.CreateIDForSubreport();
			}
			else
			{
				Global.Tracer.Assert(1 == this.m_renderReportItem.Count);
				if (renderItem.Name != this.m_customTopLevelRenderItemName)
				{
					this.m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemDefinitionName, Severity.Error, ObjectType.CustomReportItem, renderItem.Name, this.m_type, base.m_name, this.m_customTopLevelRenderItemName);
					throw new ReportProcessingException(this.m_processingContext.ErrorContext.Messages);
				}
			}
			if (renderItem is AspNetCore.ReportingServices.ReportRendering.Image)
			{
				AspNetCore.ReportingServices.ReportRendering.Image image = renderItem as AspNetCore.ReportingServices.ReportRendering.Image;
				this.m_customObjectType = ObjectType.Image;
				this.m_customObjectName = image.Name;
				Image image2 = null;
				if (this.m_renderReportItem.Count == 0)
				{
					this.m_customTopLevelRenderItemName = image.Name;
					image2 = this.DeconstructImageDefinition(image, true);
					this.m_renderReportItem.AddCustomRenderItem(image2);
				}
				else
				{
					image2 = (this.m_renderReportItem[0] as Image);
					if (image2 == null)
					{
						this.m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemInstanceType, Severity.Error, this.m_renderReportItem[0].ObjectType, this.m_renderReportItem[0].Name, this.m_type, base.m_name, ErrorContext.GetLocalizedObjectTypeString(this.m_customObjectType));
						throw new ReportProcessingException(this.m_processingContext.ErrorContext.Messages);
					}
				}
				ImageInstance imageInstance = new ImageInstance(this.m_processingContext, image2, this.m_repeatedSiblingIndex, true);
				ImageInstanceInfo instanceInfo = imageInstance.InstanceInfo;
				this.DeconstructImageInstance(image2, imageInstance, instanceInfo, image, flag, true, this);
				criInstance.AltReportItemColInstance = new ReportItemColInstance(this.m_processingContext, this.m_renderReportItem);
				criInstance.AltReportItemColInstance.Add(imageInstance);
			}
		}

		private void DeconstructImageInstance(Image image, ImageInstance imageInstance, ImageInstanceInfo imageInstanceInfo, AspNetCore.ReportingServices.ReportRendering.Image renderImage, bool isfirstInstance, bool isRootItem, IErrorContext errorContext)
		{
			this.DeconstructReportItemInstance(image, imageInstance, imageInstanceInfo, renderImage, isRootItem);
			byte[] imageData = renderImage.Processing.m_imageData;
			string mimeType = renderImage.Processing.m_mimeType;
			if (!Validator.ValidateMimeType(mimeType))
			{
				this.m_customPropertyName = "MIMEType";
				if (mimeType == null)
				{
					errorContext.Register(ProcessingErrorCode.rsMissingMIMEType, Severity.Warning);
				}
				else
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidMIMEType, Severity.Warning, mimeType);
				}
				imageInstanceInfo.BrokenImage = true;
			}
			if (imageData == null)
			{
				imageInstanceInfo.BrokenImage = true;
			}
			else if (this.m_processingContext.InPageSection && !this.m_processingContext.CreatePageSectionImageChunks)
			{
				imageInstanceInfo.Data = new ImageData(imageData, mimeType);
			}
			else if (this.m_processingContext.CreateReportChunkCallback != null && imageData.Length != 0)
			{
				string text = Guid.NewGuid().ToString();
				using (Stream stream = this.m_processingContext.CreateReportChunkCallback(text, ReportProcessing.ReportChunkTypes.Image, mimeType))
				{
					stream.Write(imageData, 0, imageData.Length);
				}
				this.m_processingContext.ImageStreamNames[text] = new ImageInfo(text, mimeType);
				imageInstanceInfo.ImageValue = text;
			}
			else
			{
				imageInstanceInfo.BrokenImage = true;
			}
			Global.Tracer.Assert(image.Action == null || null != renderImage.ActionInfo);
			if (renderImage.ActionInfo != null)
			{
				Action action = image.Action;
				ActionInstance action2 = default(ActionInstance);
				renderImage.ActionInfo.Deconstruct(imageInstance.UniqueName, ref action, out action2, this);
				image.Action = action;
				imageInstanceInfo.Action = action2;
			}
			Style styleClass = image.StyleClass;
			object[] styleAttributeValues = default(object[]);
			CustomReportItem.DeconstructRenderStyle(isfirstInstance, ((AspNetCore.ReportingServices.ReportRendering.ReportItem)renderImage).Processing.SharedStyles, ((AspNetCore.ReportingServices.ReportRendering.ReportItem)renderImage).Processing.NonSharedStyles, ref styleClass, out styleAttributeValues, this);
			image.StyleClass = styleClass;
			imageInstanceInfo.StyleAttributeValues = styleAttributeValues;
			if (renderImage.ImageMap != null)
			{
				imageInstanceInfo.ImageMapAreas = renderImage.ImageMap.Deconstruct(this.m_processingContext, this);
			}
		}

		private Image DeconstructImageDefinition(AspNetCore.ReportingServices.ReportRendering.Image renderItem, bool isRootItem)
		{
			int id = this.m_processingContext.CreateIDForSubreport();
			Image image = new Image(id, base.m_parent);
			this.DeconstructReportItemDefinition(image, renderItem, isRootItem);
			image.Source = Image.SourceType.External;
			image.Value = new ExpressionInfo(ExpressionInfo.Types.Expression);
			image.MIMEType = new ExpressionInfo(ExpressionInfo.Types.Expression);
			image.Sizing = (Image.Sizings)renderItem.Sizing;
			return image;
		}

		private void SetLabel(string label, ReportItem definition, ReportItemInstance instance, ReportItemInstanceInfo instanceInfo)
		{
			if (label != null)
			{
				Global.Tracer.Assert(instance != null && null != instanceInfo);
				instanceInfo.Label = label;
				this.m_processingContext.NavigationInfo.AddToDocumentMap(instance.GetDocumentMapUniqueName(), false, definition.StartPage, label);
			}
		}

		private void SetBookmark(string bookmark, ReportItem definition, ReportItemInstance instance, ReportItemInstanceInfo instanceInfo)
		{
			if (bookmark != null)
			{
				Global.Tracer.Assert(instance != null && null != instanceInfo);
				this.m_processingContext.NavigationInfo.ProcessBookmark(definition, instance, instanceInfo, bookmark);
			}
		}

		private void DeconstructReportItemInstance(ReportItem definition, ReportItemInstance instance, ReportItemInstanceInfo instanceInfo, AspNetCore.ReportingServices.ReportRendering.ReportItem renderItem, bool isRootItem)
		{
			Global.Tracer.Assert(definition != null && instanceInfo != null && renderItem != null && null != renderItem.Processing);
			instanceInfo.ToolTip = renderItem.Processing.Tooltip;
			definition.StartPage = this.m_criInstance.ReportItemDef.StartPage;
			if (!isRootItem)
			{
				this.SetLabel(renderItem.Processing.Label, definition, instance, instanceInfo);
			}
			else
			{
				string text = null;
				if (base.Label != null)
				{
					text = ((base.Label.Type != 0) ? base.Label.Value : this.m_criInstanceInfo.Label);
				}
				if (text == null)
				{
					text = renderItem.Processing.Label;
				}
				this.SetLabel(text, definition, instance, instanceInfo);
			}
			if (!isRootItem)
			{
				this.SetBookmark(renderItem.Processing.Bookmark, definition, instance, instanceInfo);
			}
			else
			{
				string text2 = null;
				if (base.Bookmark != null)
				{
					text2 = ((base.Bookmark.Type != 0) ? base.Bookmark.Value : this.m_criInstanceInfo.Bookmark);
				}
				if (text2 == null)
				{
					text2 = renderItem.Processing.Bookmark;
				}
				this.SetBookmark(text2, definition, instance, instanceInfo);
			}
			if (definition.CustomProperties == null && renderItem.CustomProperties != null)
			{
				Global.Tracer.Assert(0 < renderItem.CustomProperties.Count);
				this.m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemProperties, Severity.Error, this.m_customObjectType, this.m_customObjectName, this.m_type, base.Name, "CustomProperties", "0", renderItem.CustomProperties.Count.ToString(CultureInfo.InvariantCulture));
				throw new ReportProcessingException(this.m_processingContext.ErrorContext.Messages);
			}
			if (definition.CustomProperties != null && (renderItem.CustomProperties == null || definition.CustomProperties.Count != renderItem.CustomProperties.Count))
			{
				int num = (renderItem.CustomProperties != null) ? renderItem.CustomProperties.Count : 0;
				this.m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemProperties, Severity.Error, this.m_customObjectType, this.m_customObjectName, this.m_type, base.Name, "CustomProperties", definition.CustomProperties.Count.ToString(CultureInfo.InvariantCulture), num.ToString(CultureInfo.InvariantCulture));
				throw new ReportProcessingException(this.m_processingContext.ErrorContext.Messages);
			}
			if (renderItem.CustomProperties != null)
			{
				int count = renderItem.CustomProperties.Count;
				Global.Tracer.Assert(definition.CustomProperties != null && count == renderItem.CustomProperties.Count);
				instanceInfo.CustomPropertyInstances = renderItem.CustomProperties.Deconstruct();
			}
			if (isRootItem)
			{
				instanceInfo.StartHidden = base.StartHidden;
			}
		}

		private void DeconstructReportItemDefinition(ReportItem definition, AspNetCore.ReportingServices.ReportRendering.ReportItem renderItem, bool isRootItem)
		{
			Global.Tracer.Assert(definition != null && null != renderItem);
			definition.Name = base.Name + "." + renderItem.Name;
			definition.DataElementName = definition.Name;
			definition.ZIndex = renderItem.ZIndex;
			definition.ToolTip = new ExpressionInfo(ExpressionInfo.Types.Expression);
			definition.Label = new ExpressionInfo(ExpressionInfo.Types.Expression);
			definition.Bookmark = new ExpressionInfo(ExpressionInfo.Types.Expression);
			definition.IsFullSize = false;
			definition.RepeatedSibling = false;
			definition.Computed = true;
			if (renderItem.CustomProperties != null)
			{
				int count = renderItem.CustomProperties.Count;
				definition.CustomProperties = new DataValueList(count);
				for (int i = 0; i < count; i++)
				{
					DataValue dataValue = new DataValue();
					dataValue.Name = new ExpressionInfo(ExpressionInfo.Types.Expression);
					dataValue.Value = new ExpressionInfo(ExpressionInfo.Types.Expression);
					definition.CustomProperties.Add(dataValue);
				}
			}
			if (!isRootItem)
			{
				definition.Top = renderItem.Top.ToString();
				definition.TopValue = renderItem.Top.ToMillimeters();
				definition.Left = renderItem.Left.ToString();
				definition.LeftValue = renderItem.Left.ToMillimeters();
				definition.Height = renderItem.Height.ToString();
				definition.HeightValue = renderItem.Height.ToMillimeters();
				definition.Width = renderItem.Width.ToString();
				definition.WidthValue = renderItem.Width.ToMillimeters();
			}
			else
			{
				this.OverrideDefinitionSettings(definition);
			}
		}

		internal static void DeconstructRenderStyle(bool firstStyleInstance, DataValueInstanceList sharedStyles, DataValueInstanceList nonSharedStyles, ref Style style, out object[] styleAttributeValues, CustomReportItem context)
		{
			styleAttributeValues = null;
			int num = (sharedStyles != null) ? sharedStyles.Count : 0;
			int num2 = (nonSharedStyles != null) ? nonSharedStyles.Count : 0;
			int num3 = num + num2;
			if (style != null && num3 == 0)
			{
				Global.Tracer.Assert(style.StyleAttributes != null && 0 <= style.StyleAttributes.Count - style.CustomSharedStyleCount);
				styleAttributeValues = new object[style.StyleAttributes.Count - style.CustomSharedStyleCount];
				return;
			}
			if (style == null && !firstStyleInstance && 0 < num3)
			{
				context.ProcessingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemProperties, Severity.Error, context.CustomObjectType, context.CustomObjectName, context.Type, context.Name, "Styles", "0", num3.ToString(CultureInfo.InvariantCulture));
				throw new ReportProcessingException(context.ProcessingContext.ErrorContext.Messages);
			}
			if (firstStyleInstance)
			{
				if (0 >= num3)
				{
					return;
				}
				style = new Style(ConstructionPhase.Deserializing);
				style.CustomSharedStyleCount = num;
				style.StyleAttributes = new StyleAttributeHashtable(num3);
				if (num2 > 0)
				{
					style.ExpressionList = new ExpressionInfoList(num2);
					styleAttributeValues = new object[num2];
				}
				for (int i = 0; i < num; i++)
				{
					string name = sharedStyles[i].Name;
					Global.Tracer.Assert(!style.StyleAttributes.ContainsKey(name));
					context.m_customPropertyName = name;
					object obj = ProcessingValidator.ValidateCustomStyle(name, sharedStyles[i].Value, context);
					AttributeInfo attributeInfo = new AttributeInfo();
					attributeInfo.IsExpression = false;
					if (obj != null)
					{
						if ("NumeralVariant" == name)
						{
							attributeInfo.IntValue = (int)obj;
						}
						else
						{
							attributeInfo.Value = (string)obj;
						}
					}
					style.StyleAttributes.Add(name, attributeInfo);
				}
				int num4 = 0;
				string name2;
				while (true)
				{
					if (num4 < num2)
					{
						name2 = nonSharedStyles[num4].Name;
						if (!style.StyleAttributes.ContainsKey(name2))
						{
							context.m_customPropertyName = name2;
							object obj2 = ProcessingValidator.ValidateCustomStyle(name2, nonSharedStyles[num4].Value, context);
							int count = style.ExpressionList.Count;
							ExpressionInfo value = new ExpressionInfo(ExpressionInfo.Types.Expression);
							style.ExpressionList.Add(value);
							AttributeInfo attributeInfo2 = new AttributeInfo();
							attributeInfo2.IsExpression = true;
							attributeInfo2.IntValue = count;
							style.StyleAttributes.Add(name2, attributeInfo2);
							styleAttributeValues[num4] = obj2;
							num4++;
							continue;
						}
						break;
					}
					return;
				}
				context.ProcessingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemDuplicateStyle, Severity.Error, context.CustomObjectType, context.CustomObjectName, context.Type, context.Name, name2);
				throw new ReportProcessingException(context.ProcessingContext.ErrorContext.Messages);
			}
			if (sharedStyles != null && sharedStyles.Count != style.CustomSharedStyleCount)
			{
				context.ProcessingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemProperties, Severity.Error, context.CustomObjectType, context.CustomObjectName, context.Type, context.Name, "SharedStyles", style.CustomSharedStyleCount.ToString(CultureInfo.InvariantCulture), sharedStyles.Count.ToString(CultureInfo.InvariantCulture));
				throw new ReportProcessingException(context.ProcessingContext.ErrorContext.Messages);
			}
			int num5 = 0;
			string text;
			while (true)
			{
				if (num5 < num2)
				{
					if (num5 == 0)
					{
						styleAttributeValues = new object[style.StyleAttributes.Count - style.CustomSharedStyleCount];
					}
					text = (context.m_customPropertyName = nonSharedStyles[num5].Name);
					object obj3 = ProcessingValidator.ValidateCustomStyle(text, nonSharedStyles[num5].Value, context);
					AttributeInfo attributeInfo3 = style.StyleAttributes[text];
					if (attributeInfo3 == null)
					{
						context.ProcessingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemInvalidStyle, Severity.Error, context.CustomObjectType, context.CustomObjectName, context.Type, context.Name, text);
						throw new ReportProcessingException(context.ProcessingContext.ErrorContext.Messages);
					}
					if (attributeInfo3.IsExpression)
					{
						int intValue = attributeInfo3.IntValue;
						Global.Tracer.Assert(0 <= intValue && intValue < styleAttributeValues.Length);
						styleAttributeValues[intValue] = obj3;
						num5++;
						continue;
					}
					break;
				}
				return;
			}
			context.ProcessingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemInvalidStyleType, Severity.Error, context.CustomObjectType, context.CustomObjectName, context.Type, context.Name, text);
			throw new ReportProcessingException(context.ProcessingContext.ErrorContext.Messages);
		}

		private void OverrideDefinitionSettings(ReportItem target)
		{
			Global.Tracer.Assert(target != null && null != target.Name);
			target.Top = base.m_top;
			target.TopValue = base.m_topValue;
			target.Left = base.m_left;
			target.LeftValue = base.m_leftValue;
			target.Height = base.m_height;
			target.HeightValue = base.m_heightValue;
			target.Width = base.m_width;
			target.WidthValue = base.m_widthValue;
			target.Visibility = base.m_visibility;
			target.IsFullSize = base.m_isFullSize;
			target.DataElementOutput = base.m_dataElementOutput;
			target.DistanceBeforeTop = base.m_distanceBeforeTop;
			target.DistanceFromReportTop = base.m_distanceFromReportTop;
			target.SiblingAboveMe = base.m_siblingAboveMe;
		}

		internal static bool CloneObject(object o, out object clone)
		{
			clone = null;
			if (o != null && DBNull.Value != o)
			{
				if (o is string)
				{
					clone = string.Copy(o as string);
				}
				else if (o is char)
				{
					clone = '\0';
					clone = (char)o;
				}
				else if (o is bool)
				{
					clone = ((bool)o).CompareTo(true);
				}
				else if (o is short)
				{
					clone = (short)0;
					clone = (short)o;
				}
				else if (o is int)
				{
					clone = 0;
					clone = (int)o;
				}
				else if (o is long)
				{
					clone = 0L;
					clone = (long)o;
				}
				else if (o is ushort)
				{
					clone = (ushort)0;
					clone = (ushort)o;
				}
				else if (o is uint)
				{
					clone = 0u;
					clone = (uint)o;
				}
				else if (o is ulong)
				{
					clone = 0uL;
					clone = (ulong)o;
				}
				else if (o is byte)
				{
					clone = (byte)0;
					clone = (byte)o;
				}
				else if (o is sbyte)
				{
					clone = (sbyte)0;
					clone = (sbyte)o;
				}
				else if (o is TimeSpan)
				{
					clone = new TimeSpan(((TimeSpan)o).Ticks);
				}
				else if (o is DateTime)
				{
					clone = new DateTime(((DateTime)o).Ticks);
				}
				else if (o is float)
				{
					clone = 0f;
					clone = (float)o;
				}
				else if (o is double)
				{
					clone = 0.0;
					clone = (double)o;
				}
				else if (o is decimal)
				{
					clone = 0m;
					clone = (decimal)o;
				}
				return null != clone;
			}
			return true;
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, params string[] arguments)
		{
			Global.Tracer.Assert(null != this.m_processingContext, "An unexpected error happened during deconstructing a custom report item");
			this.m_processingContext.ErrorContext.Register(code, severity, this.m_customObjectType, this.m_customObjectName, this.m_customPropertyName, arguments);
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, params string[] arguments)
		{
			Global.Tracer.Assert(null != this.m_processingContext, "An unexpected error happened during deconstructing a custom report item");
			this.m_processingContext.ErrorContext.Register(code, severity, objectType, objectName, propertyName, arguments);
		}
	}
}
