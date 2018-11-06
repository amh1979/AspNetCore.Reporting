using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Chart : Pivot, IRunningValueHolder
	{
		internal enum ChartTypes
		{
			Column,
			Bar,
			Line,
			Pie,
			Scatter,
			Bubble,
			Area,
			Doughnut,
			Stock
		}

		internal enum ChartSubTypes
		{
			Default,
			Stacked,
			PercentStacked,
			Plain,
			Smooth,
			Exploded,
			Line,
			SmoothLine,
			HighLowClose,
			OpenHighLowClose,
			Candlestick
		}

		internal enum ChartPalette
		{
			Default,
			EarthTones,
			Excel,
			GrayScale,
			Light,
			Pastel,
			SemiTransparent
		}

		private ChartHeading m_columns;

		private ChartHeading m_rows;

		private ChartDataPointList m_cellDataPoints;

		private RunningValueInfoList m_cellRunningValues;

		private MultiChart m_multiChart;

		private Legend m_legend;

		private Axis m_categoryAxis;

		private Axis m_valueAxis;

		[Reference]
		private ChartHeading m_staticColumns;

		[Reference]
		private ChartHeading m_staticRows;

		private ChartTypes m_type;

		private ChartSubTypes m_subType;

		private ChartPalette m_palette;

		private ChartTitle m_title;

		private int m_pointWidth;

		private ThreeDProperties m_3dProperties;

		private PlotArea m_plotArea;

		[NonSerialized]
		private ChartExprHost m_exprHost;

		[NonSerialized]
		private IntList m_numberOfSeriesDataPoints;

		[NonSerialized]
		private BoolList m_seriesPlotType;

		[NonSerialized]
		private bool m_hasSeriesPlotTypeLine;

		[NonSerialized]
		private bool m_hasDataValueAggregates;

		internal override ObjectType ObjectType
		{
			get
			{
				return ObjectType.Chart;
			}
		}

		internal override PivotHeading PivotColumns
		{
			get
			{
				return this.m_columns;
			}
		}

		internal override PivotHeading PivotRows
		{
			get
			{
				return this.m_rows;
			}
		}

		internal ChartHeading Columns
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

		internal ChartHeading Rows
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

		internal MultiChart MultiChart
		{
			get
			{
				return this.m_multiChart;
			}
			set
			{
				this.m_multiChart = value;
			}
		}

		internal ChartDataPointList ChartDataPoints
		{
			get
			{
				return this.m_cellDataPoints;
			}
			set
			{
				this.m_cellDataPoints = value;
			}
		}

		internal override RunningValueInfoList PivotCellRunningValues
		{
			get
			{
				return this.m_cellRunningValues;
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

		internal Legend Legend
		{
			get
			{
				return this.m_legend;
			}
			set
			{
				this.m_legend = value;
			}
		}

		internal Axis CategoryAxis
		{
			get
			{
				return this.m_categoryAxis;
			}
			set
			{
				this.m_categoryAxis = value;
			}
		}

		internal Axis ValueAxis
		{
			get
			{
				return this.m_valueAxis;
			}
			set
			{
				this.m_valueAxis = value;
			}
		}

		internal override PivotHeading PivotStaticColumns
		{
			get
			{
				return this.m_staticColumns;
			}
		}

		internal override PivotHeading PivotStaticRows
		{
			get
			{
				return this.m_staticRows;
			}
		}

		internal ChartHeading StaticColumns
		{
			get
			{
				return this.m_staticColumns;
			}
			set
			{
				this.m_staticColumns = value;
			}
		}

		internal ChartHeading StaticRows
		{
			get
			{
				return this.m_staticRows;
			}
			set
			{
				this.m_staticRows = value;
			}
		}

		internal ChartTypes Type
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

		internal ChartSubTypes SubType
		{
			get
			{
				return this.m_subType;
			}
			set
			{
				this.m_subType = value;
			}
		}

		internal ChartTitle Title
		{
			get
			{
				return this.m_title;
			}
			set
			{
				this.m_title = value;
			}
		}

		internal int PointWidth
		{
			get
			{
				return this.m_pointWidth;
			}
			set
			{
				this.m_pointWidth = value;
			}
		}

		internal ThreeDProperties ThreeDProperties
		{
			get
			{
				return this.m_3dProperties;
			}
			set
			{
				this.m_3dProperties = value;
			}
		}

		internal ChartPalette Palette
		{
			get
			{
				return this.m_palette;
			}
			set
			{
				this.m_palette = value;
			}
		}

		internal PlotArea PlotArea
		{
			get
			{
				return this.m_plotArea;
			}
			set
			{
				this.m_plotArea = value;
			}
		}

		internal ChartExprHost ChartExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		protected override DataRegionExprHost DataRegionExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal IntList NumberOfSeriesDataPoints
		{
			get
			{
				return this.m_numberOfSeriesDataPoints;
			}
			set
			{
				this.m_numberOfSeriesDataPoints = value;
			}
		}

		internal BoolList SeriesPlotType
		{
			get
			{
				return this.m_seriesPlotType;
			}
			set
			{
				this.m_seriesPlotType = value;
			}
		}

		internal bool HasSeriesPlotTypeLine
		{
			get
			{
				return this.m_hasSeriesPlotTypeLine;
			}
			set
			{
				this.m_hasSeriesPlotTypeLine = value;
			}
		}

		internal bool HasDataValueAggregates
		{
			get
			{
				return this.m_hasDataValueAggregates;
			}
			set
			{
				this.m_hasDataValueAggregates = value;
			}
		}

		internal int StaticSeriesCount
		{
			get
			{
				ExpressionInfoList expressionInfoList = (this.PivotStaticRows != null) ? ((ChartHeading)this.PivotStaticRows).Labels : null;
				if (expressionInfoList == null)
				{
					return 1;
				}
				return expressionInfoList.Count;
			}
		}

		internal int StaticCategoryCount
		{
			get
			{
				ExpressionInfoList expressionInfoList = (this.PivotStaticColumns != null) ? ((ChartHeading)this.PivotStaticColumns).Labels : null;
				if (expressionInfoList == null)
				{
					return 1;
				}
				return expressionInfoList.Count;
			}
		}

		internal Chart(ReportItem parent)
			: base(parent)
		{
		}

		internal Chart(int id, ReportItem parent)
			: base(id, parent)
		{
			this.m_cellDataPoints = new ChartDataPointList();
			this.m_cellRunningValues = new RunningValueInfoList();
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

		internal static object[] CreateStyle(ReportProcessing.ProcessingContext pc, Style styleDef, string objectName, int uniqueName)
		{
			object[] array = null;
			if (styleDef != null && styleDef.ExpressionList != null && 0 < styleDef.ExpressionList.Count)
			{
				array = new object[styleDef.ExpressionList.Count];
				ReportProcessing.RuntimeRICollection.EvaluateStyleAttributes(ObjectType.Chart, objectName, styleDef, uniqueName, array, pc);
			}
			return array;
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = this.ObjectType;
			context.ObjectName = base.m_name;
			if ((context.Location & LocationFlags.InDetail) != 0 && (context.Location & LocationFlags.InGrouping) == (LocationFlags)0)
			{
				context.ErrorContext.Register((ProcessingErrorCode)((base.m_parent is Table) ? 23 : 21), Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			else
			{
				context.RegisterDataRegion(this);
				this.InternalInitialize(context);
				context.UnRegisterDataRegion(this);
			}
			return false;
		}

		private void InternalInitialize(InitializationContext context)
		{
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ExprHostBuilder.ChartStart(base.m_name);
			base.Initialize(context);
			context.RegisterRunningValues(base.m_runningValues);
			if (base.m_visibility != null)
			{
				base.m_visibility.Initialize(context, true, false);
			}
			this.CornerInitialize(context);
			context.Location &= ~LocationFlags.InMatrixCellTopLevelItem;
			bool flag = false;
			bool flag2 = false;
			int expectedNumberOfCategories = default(int);
			this.ColumnsInitialize(context, out expectedNumberOfCategories, out flag);
			flag2 = flag;
			int expectedNumberOfSeries = default(int);
			this.RowsInitialize(context, out expectedNumberOfSeries, out flag);
			if (flag)
			{
				flag2 = true;
			}
			this.ChartDataPointInitialize(context, expectedNumberOfCategories, expectedNumberOfSeries, flag2);
			if (base.m_visibility != null)
			{
				base.m_visibility.UnRegisterReceiver(context);
			}
			context.UnRegisterRunningValues(base.m_runningValues);
			base.CopyHeadingAggregates(this.m_rows);
			this.m_rows.TransferHeadingAggregates();
			base.CopyHeadingAggregates(this.m_columns);
			this.m_columns.TransferHeadingAggregates();
			base.ExprHostID = context.ExprHostBuilder.ChartEnd();
		}

		private void CornerInitialize(InitializationContext context)
		{
			if (this.m_categoryAxis != null)
			{
				context.ExprHostBuilder.ChartCategoryAxisStart();
				this.m_categoryAxis.Initialize(context, Axis.Mode.CategoryAxis);
				context.ExprHostBuilder.ChartCategoryAxisEnd();
			}
			if (this.m_valueAxis != null)
			{
				context.ExprHostBuilder.ChartValueAxisStart();
				this.m_valueAxis.Initialize(context, Axis.Mode.ValueAxis);
				context.ExprHostBuilder.ChartValueAxisEnd();
			}
			if (this.m_multiChart != null)
			{
				this.m_multiChart.Initialize(context);
			}
			if (this.m_legend != null)
			{
				this.m_legend.Initialize(context);
			}
			if (this.m_title != null)
			{
				this.m_title.Initialize(context);
			}
			if (this.m_3dProperties != null)
			{
				this.m_3dProperties.Initialize(context);
			}
			if (this.m_plotArea != null)
			{
				this.m_plotArea.Initialize(context);
			}
			if (this.m_categoryAxis != null && this.m_categoryAxis.Scalar)
			{
				Global.Tracer.Assert(null != this.m_columns);
				if (this.m_columns.SubHeading != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsMultipleGroupingsOnChartScalarAxis, Severity.Error, context.ObjectType, context.ObjectName, "CategoryAxis");
				}
				else if (this.StaticColumns != null && this.StaticColumns.Labels != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsStaticGroupingOnChartScalarAxis, Severity.Error, context.ObjectType, context.ObjectName, "CategoryAxis");
				}
				else if (this.m_columns.Grouping != null && this.m_columns.Grouping.GroupExpressions != null && 1 < this.m_columns.Grouping.GroupExpressions.Count)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsMultipleGroupExpressionsOnChartScalarAxis, Severity.Error, context.ObjectType, context.ObjectName, "CategoryAxis");
				}
				else
				{
					Global.Tracer.Assert(null == this.m_columns.SubHeading);
					Global.Tracer.Assert(this.StaticColumns == null || null == this.StaticColumns.Labels);
					this.m_columns.ChartGroupExpression = true;
					if (this.m_columns.Labels != null && this.m_columns.Grouping != null && this.m_columns.Grouping.GroupExpressions != null && ReportProcessing.CompareWithInvariantCulture(this.m_columns.Labels[0].Value, this.m_columns.Grouping.GroupExpressions[0].Value, true) != 0)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsLabelExpressionOnChartScalarAxisIsIgnored, Severity.Warning, context.ObjectType, context.ObjectName, "CategoryAxis");
						this.m_columns.Labels = null;
					}
					if (this.m_columns.Grouping != null && ChartTypes.Area == this.m_type)
					{
						Global.Tracer.Assert(null != this.m_columns.Grouping.GroupExpressions);
						if (this.m_columns.Sorting == null || this.m_columns.Sorting.SortExpressions == null || this.m_columns.Sorting.SortExpressions[0] == null)
						{
							this.m_columns.Grouping.GroupAndSort = true;
							this.m_columns.Grouping.SortDirections = new BoolList(1);
							this.m_columns.Grouping.SortDirections.Add(true);
						}
						else if (ReportProcessing.CompareWithInvariantCulture(this.m_columns.Grouping.GroupExpressions[0].Value, this.m_columns.Sorting.SortExpressions[0].Value, true) != 0)
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsUnsortedCategoryInAreaChart, Severity.Error, context.ObjectType, context.ObjectName, "CategoryGrouping", this.m_columns.Grouping.Name);
						}
					}
					else if (this.m_columns.Grouping != null)
					{
						if (ChartTypes.Line != this.m_type)
						{
							if (this.m_type != 0)
							{
								return;
							}
							if (!this.m_hasSeriesPlotTypeLine)
							{
								return;
							}
						}
						Global.Tracer.Assert(null != this.m_columns.Grouping.GroupExpressions);
						if (!this.m_columns.Grouping.GroupAndSort)
						{
							bool flag = false;
							if (this.m_columns.Sorting == null || this.m_columns.Sorting.SortExpressions == null || this.m_columns.Sorting.SortExpressions[0] == null)
							{
								flag = true;
							}
							else if (ReportProcessing.CompareWithInvariantCulture(this.m_columns.Grouping.GroupExpressions[0].Value, this.m_columns.Sorting.SortExpressions[0].Value, true) != 0)
							{
								flag = true;
							}
							if (flag)
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsLineChartMightScatter, Severity.Warning, context.ObjectType, context.ObjectName, "CategoryGrouping");
							}
						}
					}
				}
			}
		}

		private void ColumnsInitialize(InitializationContext context, out int expectedNumberOfCategories, out bool computedSubtotal)
		{
			Global.Tracer.Assert(null != this.m_columns);
			computedSubtotal = false;
			this.m_columns.DynamicInitialize(true, 0, context);
			this.m_columns.StaticInitialize(context);
			expectedNumberOfCategories = ((this.m_staticColumns == null) ? 1 : this.m_staticColumns.NumberOfStatics);
			if (this.m_columns.Grouping == null)
			{
				Global.Tracer.Assert(null != this.m_columns);
				context.SpecialTransferRunningValues(this.m_columns.RunningValues);
			}
		}

		private void RowsInitialize(InitializationContext context, out int expectedNumberOfSeries, out bool computedSubtotal)
		{
			Global.Tracer.Assert(null != this.m_rows);
			computedSubtotal = false;
			this.m_rows.DynamicInitialize(false, 0, context);
			this.m_rows.StaticInitialize(context);
			expectedNumberOfSeries = ((this.m_staticRows == null) ? 1 : this.m_staticRows.NumberOfStatics);
			if (this.m_rows != null && this.m_rows.Grouping == null)
			{
				context.SpecialTransferRunningValues(this.m_rows.RunningValues);
			}
			if (this.m_hasSeriesPlotTypeLine && this.m_seriesPlotType != null)
			{
				if (this.m_type == ChartTypes.Column)
				{
					if (this.m_staticRows == null)
					{
						this.m_type = ChartTypes.Line;
					}
					else
					{
						this.m_staticRows.PlotTypesLine = this.m_seriesPlotType;
					}
				}
				else
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsChartSeriesPlotTypeIgnored, Severity.Warning, context.ObjectType, context.ObjectName, "PlotType");
				}
			}
		}

		private void ChartDataPointInitialize(InitializationContext context, int expectedNumberOfCategories, int expectedNumberOfSeries, bool computedCells)
		{
			if (this.m_cellDataPoints == null || this.m_cellDataPoints.Count == 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsMissingChartDataPoints, Severity.Error, context.ObjectType, context.ObjectName, "ChartData");
			}
			else
			{
				if (expectedNumberOfSeries != this.m_numberOfSeriesDataPoints.Count)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfChartSeries, Severity.Error, context.ObjectType, context.ObjectName, "ChartSeries");
				}
				bool flag = false;
				for (int i = 0; i < this.m_numberOfSeriesDataPoints.Count; i++)
				{
					if (flag)
					{
						break;
					}
					if (this.m_numberOfSeriesDataPoints[i] != expectedNumberOfCategories)
					{
						flag = true;
					}
				}
				if (flag)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfChartDataPointsInSeries, Severity.Error, context.ObjectType, context.ObjectName, "ChartSeries");
				}
				int num = expectedNumberOfCategories * expectedNumberOfSeries;
				if (num != this.m_cellDataPoints.Count)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfChartDataPoints, Severity.Error, context.ObjectType, context.ObjectName, "DataPoints", this.m_cellDataPoints.Count.ToString(CultureInfo.InvariantCulture), num.ToString(CultureInfo.InvariantCulture));
				}
				context.Location |= LocationFlags.InMatrixCell;
				context.MatrixName = base.m_name;
				context.RegisterTablixCellScope(this.m_columns.SubHeading == null && null == this.m_columns.Grouping, base.m_cellAggregates, base.m_cellPostSortAggregates);
				for (ChartHeading chartHeading = this.m_rows; chartHeading != null; chartHeading = chartHeading.SubHeading)
				{
					if (chartHeading.Grouping != null)
					{
						context.Location |= LocationFlags.InGrouping;
						context.RegisterGroupingScopeForTablixCell(chartHeading.Grouping.Name, false, chartHeading.Grouping.SimpleGroupExpressions, chartHeading.Aggregates, chartHeading.PostSortAggregates, chartHeading.RecursiveAggregates, chartHeading.Grouping);
					}
				}
				if (this.m_rows.Grouping != null && this.m_rows.Subtotal != null && this.m_staticRows != null)
				{
					context.CopyRunningValues(this.StaticRows.RunningValues, base.m_aggregates);
				}
				for (ChartHeading chartHeading = this.m_columns; chartHeading != null; chartHeading = chartHeading.SubHeading)
				{
					if (chartHeading.Grouping != null)
					{
						context.Location |= LocationFlags.InGrouping;
						context.RegisterGroupingScopeForTablixCell(chartHeading.Grouping.Name, true, chartHeading.Grouping.SimpleGroupExpressions, chartHeading.Aggregates, chartHeading.PostSortAggregates, chartHeading.RecursiveAggregates, chartHeading.Grouping);
					}
				}
				if (this.m_columns.Grouping != null && this.m_columns.Subtotal != null && this.m_staticColumns != null)
				{
					context.CopyRunningValues(this.StaticColumns.RunningValues, base.m_aggregates);
				}
				Global.Tracer.Assert(null != this.m_cellDataPoints);
				int count = this.m_cellDataPoints.Count;
				int num2 = 1;
				switch (this.m_type)
				{
				case ChartTypes.Stock:
					num2 = ((ChartSubTypes.HighLowClose != this.m_subType) ? 4 : 3);
					break;
				case ChartTypes.Bubble:
					num2 = 3;
					break;
				case ChartTypes.Scatter:
					num2 = 2;
					break;
				}
				context.RegisterRunningValues(this.m_cellRunningValues);
				for (int j = 0; j < count; j++)
				{
					Global.Tracer.Assert(null != this.m_cellDataPoints[j].DataValues);
					if (num2 > this.m_cellDataPoints[j].DataValues.Count)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfDataValues, Severity.Error, context.ObjectType, context.ObjectName, "DataValue", this.m_cellDataPoints[j].DataValues.Count.ToString(CultureInfo.InvariantCulture), num2.ToString(CultureInfo.InvariantCulture));
					}
					this.m_cellDataPoints[j].Initialize(context);
				}
				context.UnRegisterRunningValues(this.m_cellRunningValues);
				if (context.IsRunningValueDirectionColumn())
				{
					base.m_processingInnerGrouping = ProcessingInnerGroupings.Row;
				}
				for (ChartHeading chartHeading = this.m_rows; chartHeading != null; chartHeading = chartHeading.SubHeading)
				{
					if (chartHeading.Grouping != null)
					{
						context.UnRegisterGroupingScopeForTablixCell(chartHeading.Grouping.Name, false);
					}
				}
				for (ChartHeading chartHeading = this.m_columns; chartHeading != null; chartHeading = chartHeading.SubHeading)
				{
					if (chartHeading.Grouping != null)
					{
						context.UnRegisterGroupingScopeForTablixCell(chartHeading.Grouping.Name, true);
					}
				}
				context.UnRegisterTablixCellScope();
			}
		}

		internal bool IsValidChartSubType()
		{
			if (this.m_subType == ChartSubTypes.Default)
			{
				ChartTypes type = this.m_type;
				if (type == ChartTypes.Stock)
				{
					this.m_subType = ChartSubTypes.HighLowClose;
				}
				else
				{
					this.m_subType = ChartSubTypes.Plain;
				}
				return true;
			}
			bool result = true;
			switch (this.m_type)
			{
			case ChartTypes.Column:
				switch (this.m_subType)
				{
				default:
					result = false;
					break;
				case ChartSubTypes.Stacked:
				case ChartSubTypes.PercentStacked:
				case ChartSubTypes.Plain:
					break;
				}
				break;
			case ChartTypes.Bar:
				switch (this.m_subType)
				{
				default:
					result = false;
					break;
				case ChartSubTypes.Stacked:
				case ChartSubTypes.PercentStacked:
				case ChartSubTypes.Plain:
					break;
				}
				break;
			case ChartTypes.Line:
				switch (this.m_subType)
				{
				default:
					result = false;
					break;
				case ChartSubTypes.Stacked:
				case ChartSubTypes.PercentStacked:
				case ChartSubTypes.Plain:
				case ChartSubTypes.Smooth:
					break;
				}
				break;
			case ChartTypes.Pie:
				switch (this.m_subType)
				{
				default:
					result = false;
					break;
				case ChartSubTypes.Plain:
				case ChartSubTypes.Exploded:
					break;
				}
				break;
			case ChartTypes.Scatter:
				switch (this.m_subType)
				{
				default:
					result = false;
					break;
				case ChartSubTypes.Plain:
				case ChartSubTypes.Line:
				case ChartSubTypes.SmoothLine:
					break;
				}
				break;
			case ChartTypes.Bubble:
			{
				ChartSubTypes subType = this.m_subType;
				if (subType != ChartSubTypes.Plain)
				{
					result = false;
				}
				break;
			}
			case ChartTypes.Area:
				switch (this.m_subType)
				{
				default:
					result = false;
					break;
				case ChartSubTypes.Stacked:
				case ChartSubTypes.PercentStacked:
				case ChartSubTypes.Plain:
					break;
				}
				break;
			case ChartTypes.Doughnut:
				switch (this.m_subType)
				{
				default:
					result = false;
					break;
				case ChartSubTypes.Plain:
				case ChartSubTypes.Exploded:
					break;
				}
				break;
			case ChartTypes.Stock:
				switch (this.m_subType)
				{
				default:
					result = false;
					break;
				case ChartSubTypes.HighLowClose:
				case ChartSubTypes.OpenHighLowClose:
				case ChartSubTypes.Candlestick:
					break;
				}
				break;
			default:
				Global.Tracer.Assert(false, string.Empty);
				result = false;
				break;
			}
			return result;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				this.m_exprHost = reportExprHost.ChartHostsRemotable[base.ExprHostID];
				base.DataRegionSetExprHost(this.m_exprHost, reportObjectModel);
				if (this.m_multiChart != null && this.m_exprHost.MultiChartHost != null)
				{
					this.m_multiChart.SetExprHost(this.m_exprHost.MultiChartHost, reportObjectModel);
				}
				IList<ChartDataPointExprHost> chartDataPointHostsRemotable = this.m_exprHost.ChartDataPointHostsRemotable;
				for (int i = 0; i < this.m_cellDataPoints.Count; i++)
				{
					ChartDataPoint chartDataPoint = this.m_cellDataPoints[i];
					if (chartDataPoint != null && chartDataPoint.ExprHostID != -1)
					{
						chartDataPoint.SetExprHost(chartDataPointHostsRemotable[chartDataPoint.ExprHostID], reportObjectModel);
					}
				}
				if (this.m_categoryAxis != null && this.m_exprHost.CategoryAxisHost != null)
				{
					this.m_categoryAxis.SetExprHost(this.m_exprHost.CategoryAxisHost, reportObjectModel);
				}
				if (this.m_valueAxis != null && this.m_exprHost.ValueAxisHost != null)
				{
					this.m_valueAxis.SetExprHost(this.m_exprHost.ValueAxisHost, reportObjectModel);
				}
				if (this.m_title != null && this.m_exprHost.TitleHost != null)
				{
					this.m_title.SetExprHost(this.m_exprHost.TitleHost, reportObjectModel);
				}
				if (this.m_exprHost.StaticColumnLabelsHost != null)
				{
					this.m_exprHost.StaticColumnLabelsHost.SetReportObjectModel(reportObjectModel);
				}
				if (this.m_exprHost.StaticRowLabelsHost != null)
				{
					this.m_exprHost.StaticRowLabelsHost.SetReportObjectModel(reportObjectModel);
				}
				if (this.m_legend != null && this.m_exprHost.LegendHost != null)
				{
					this.m_legend.SetExprHost(this.m_exprHost.LegendHost, reportObjectModel);
				}
				if (this.m_plotArea != null && this.m_exprHost.PlotAreaHost != null)
				{
					this.m_plotArea.SetExprHost(this.m_exprHost.PlotAreaHost, reportObjectModel);
				}
			}
		}

		internal ChartDataPoint GetDataPoint(int seriesIndex, int categoryIndex)
		{
			int index = seriesIndex * this.StaticCategoryCount + categoryIndex;
			return this.m_cellDataPoints[index];
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Columns, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartHeading));
			memberInfoList.Add(new MemberInfo(MemberName.Rows, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartHeading));
			memberInfoList.Add(new MemberInfo(MemberName.CellDataPoints, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartDataPointList));
			memberInfoList.Add(new MemberInfo(MemberName.CellRunningValues, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.RunningValueInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.MultiChart, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.MultiChart));
			memberInfoList.Add(new MemberInfo(MemberName.Legend, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Legend));
			memberInfoList.Add(new MemberInfo(MemberName.CategoryAxis, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Axis));
			memberInfoList.Add(new MemberInfo(MemberName.ValueAxis, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Axis));
			memberInfoList.Add(new MemberInfo(MemberName.StaticColumns, Token.Reference, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartHeading));
			memberInfoList.Add(new MemberInfo(MemberName.StaticRows, Token.Reference, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartHeading));
			memberInfoList.Add(new MemberInfo(MemberName.Type, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.SubType, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.Palette, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.Title, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartTitle));
			memberInfoList.Add(new MemberInfo(MemberName.PointWidth, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.ThreeDProperties, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ThreeDProperties));
			memberInfoList.Add(new MemberInfo(MemberName.PlotArea, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.PlotArea));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Pivot, memberInfoList);
		}
	}
}
