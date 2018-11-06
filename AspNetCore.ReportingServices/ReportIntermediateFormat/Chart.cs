using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class Chart : DataRegion, IPersistable
	{
		private ChartMemberList m_categoryMembers;

		private ChartMemberList m_seriesMembers;

		private ChartSeriesList m_chartSeriesCollection;

		private List<ChartDerivedSeries> m_chartDerivedSeriesCollection;

		private ExpressionInfo m_palette;

		private ExpressionInfo m_paletteHatchBehavior;

		private List<ChartArea> m_chartAreas;

		private List<ChartLegend> m_legends;

		private List<ChartTitle> m_titles;

		private List<ChartCustomPaletteColor> m_customPaletteColors;

		private DataValueList m_codeParameters;

		private ChartBorderSkin m_borderSkin;

		private ChartNoDataMessage m_noDataMessage;

		private ExpressionInfo m_dynamicHeight;

		private ExpressionInfo m_dynamicWidth;

		private bool m_dataValueSequenceRendering;

		private bool m_columnGroupingIsSwitched;

		private bool m_enableCategoryDrilldown;

		[NonSerialized]
		private bool m_hasDataValueAggregates;

		[NonSerialized]
		private bool m_hasSeriesPlotTypeLine;

		[NonSerialized]
		private bool? m_hasStaticColumns;

		[NonSerialized]
		private bool? m_hasStaticRows;

		[NonSerialized]
		private static readonly Declaration m_Declaration = Chart.GetDeclaration();

		[NonSerialized]
		private int m_actionOwnerCounter;

		[NonSerialized]
		private ChartExprHost m_chartExprHost;

		internal override AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart;
			}
		}

		internal override HierarchyNodeList ColumnMembers
		{
			get
			{
				return this.m_categoryMembers;
			}
		}

		internal override HierarchyNodeList RowMembers
		{
			get
			{
				return this.m_seriesMembers;
			}
		}

		public override bool IsColumnGroupingSwitched
		{
			get
			{
				return this.m_columnGroupingIsSwitched;
			}
		}

		internal override RowList Rows
		{
			get
			{
				return this.m_chartSeriesCollection;
			}
		}

		internal ChartMemberList CategoryMembers
		{
			get
			{
				return this.m_categoryMembers;
			}
			set
			{
				this.m_categoryMembers = value;
			}
		}

		internal ChartMemberList SeriesMembers
		{
			get
			{
				return this.m_seriesMembers;
			}
			set
			{
				this.m_seriesMembers = value;
			}
		}

		internal ChartSeriesList ChartSeriesCollection
		{
			get
			{
				return this.m_chartSeriesCollection;
			}
			set
			{
				this.m_chartSeriesCollection = value;
			}
		}

		internal List<ChartDerivedSeries> DerivedSeriesCollection
		{
			get
			{
				return this.m_chartDerivedSeriesCollection;
			}
			set
			{
				this.m_chartDerivedSeriesCollection = value;
			}
		}

		internal bool HasStaticColumns
		{
			get
			{
				if (this.m_hasStaticColumns.HasValue)
				{
					return this.m_hasStaticColumns.Value;
				}
				if (this.m_categoryMembers != null && this.m_categoryMembers.Count != 0)
				{
					if (this.m_categoryMembers.Count > 1)
					{
						this.m_hasStaticColumns = true;
					}
					ChartMember member = this.m_categoryMembers[0];
					this.m_hasStaticColumns = this.ContainsStatic(member);
					return this.m_hasStaticColumns.Value;
				}
				return false;
			}
		}

		internal bool HasStaticRows
		{
			get
			{
				if (this.m_hasStaticRows.HasValue)
				{
					return this.m_hasStaticRows.Value;
				}
				if (this.m_seriesMembers != null && this.m_seriesMembers.Count != 0)
				{
					if (this.m_seriesMembers.Count > 1)
					{
						this.m_hasStaticRows = true;
					}
					ChartMember member = this.m_seriesMembers[0];
					this.m_hasStaticRows = this.ContainsStatic(member);
					return this.m_hasStaticRows.Value;
				}
				return false;
			}
		}

		internal ExpressionInfo DynamicWidth
		{
			get
			{
				return this.m_dynamicWidth;
			}
			set
			{
				this.m_dynamicWidth = value;
			}
		}

		internal ExpressionInfo DynamicHeight
		{
			get
			{
				return this.m_dynamicHeight;
			}
			set
			{
				this.m_dynamicHeight = value;
			}
		}

		internal List<ChartArea> ChartAreas
		{
			get
			{
				return this.m_chartAreas;
			}
			set
			{
				this.m_chartAreas = value;
			}
		}

		internal List<ChartLegend> Legends
		{
			get
			{
				return this.m_legends;
			}
			set
			{
				this.m_legends = value;
			}
		}

		internal List<ChartTitle> Titles
		{
			get
			{
				return this.m_titles;
			}
			set
			{
				this.m_titles = value;
			}
		}

		internal ExpressionInfo Palette
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

		internal ExpressionInfo PaletteHatchBehavior
		{
			get
			{
				return this.m_paletteHatchBehavior;
			}
			set
			{
				this.m_paletteHatchBehavior = value;
			}
		}

		internal DataValueList CodeParameters
		{
			get
			{
				return this.m_codeParameters;
			}
			set
			{
				this.m_codeParameters = value;
			}
		}

		internal List<ChartCustomPaletteColor> CustomPaletteColors
		{
			get
			{
				return this.m_customPaletteColors;
			}
			set
			{
				this.m_customPaletteColors = value;
			}
		}

		internal ChartBorderSkin BorderSkin
		{
			get
			{
				return this.m_borderSkin;
			}
			set
			{
				this.m_borderSkin = value;
			}
		}

		internal ChartNoDataMessage NoDataMessage
		{
			get
			{
				return this.m_noDataMessage;
			}
			set
			{
				this.m_noDataMessage = value;
			}
		}

		internal ChartExprHost ChartExprHost
		{
			get
			{
				return this.m_chartExprHost;
			}
		}

		protected override IndexedExprHost UserSortExpressionsHost
		{
			get
			{
				if (this.m_chartExprHost == null)
				{
					return null;
				}
				return this.m_chartExprHost.UserSortExpressionsHost;
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

		internal int SeriesCount
		{
			get
			{
				return base.RowCount;
			}
			set
			{
				base.RowCount = value;
			}
		}

		internal int CategoryCount
		{
			get
			{
				return base.ColumnCount;
			}
			set
			{
				base.ColumnCount = value;
			}
		}

		internal bool DataValueSequenceRendering
		{
			get
			{
				return this.m_dataValueSequenceRendering;
			}
		}

		internal bool EnableCategoryDrilldown
		{
			get
			{
				return this.m_enableCategoryDrilldown;
			}
			set
			{
				this.m_enableCategoryDrilldown = value;
			}
		}

		internal Chart(ReportItem parent)
			: base(parent)
		{
		}

		internal Chart(int id, ReportItem parent)
			: base(id, parent)
		{
		}

		internal void SetColumnGroupingDirection(bool isOuter)
		{
			this.m_columnGroupingIsSwitched = isOuter;
		}

		private bool ContainsStatic(ChartMember member)
		{
			while (member != null)
			{
				if (member.Grouping == null)
				{
					return true;
				}
				if (member.ChartMembers != null && member.ChartMembers.Count > 0)
				{
					if (member.ChartMembers.Count > 1)
					{
						return true;
					}
					member = member.ChartMembers[0];
				}
				else
				{
					member = null;
				}
			}
			return false;
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = this.ObjectType;
			context.ObjectName = base.m_name;
			if ((context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDetail) != 0 && (context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InGrouping) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsDataRegionInDetailList, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			else
			{
				if (!context.RegisterDataRegion(this))
				{
					return false;
				}
				context.Location |= (AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataSet | AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegion);
				context.ExprHostBuilder.DataRegionStart(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.Chart, base.m_name);
				base.Initialize(context);
				base.ExprHostID = context.ExprHostBuilder.DataRegionEnd(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.Chart);
				context.UnRegisterDataRegion(this);
			}
			return false;
		}

		protected override bool InitializeMembers(InitializationContext context)
		{
			bool flag = base.InitializeMembers(context);
			if (flag)
			{
				bool hasSeriesPlotTypeLine = this.m_hasSeriesPlotTypeLine;
			}
			return flag;
		}

		protected override void InitializeCorner(InitializationContext context)
		{
			if (this.m_chartAreas != null)
			{
				for (int i = 0; i < this.m_chartAreas.Count; i++)
				{
					this.m_chartAreas[i].Initialize(context);
				}
			}
			if (this.m_legends != null)
			{
				for (int j = 0; j < this.m_legends.Count; j++)
				{
					this.m_legends[j].Initialize(context);
				}
			}
			if (this.m_titles != null)
			{
				for (int k = 0; k < this.m_titles.Count; k++)
				{
					this.m_titles[k].Initialize(context);
				}
			}
			if (this.m_codeParameters != null)
			{
				this.m_codeParameters.Initialize("CodeParameters", context);
			}
			if (this.m_customPaletteColors != null)
			{
				for (int l = 0; l < this.m_customPaletteColors.Count; l++)
				{
					this.m_customPaletteColors[l].Initialize(context, l);
				}
			}
			if (this.m_borderSkin != null)
			{
				this.m_borderSkin.Initialize(context);
			}
			if (this.m_noDataMessage != null)
			{
				this.m_noDataMessage.Initialize(context);
			}
			if (this.m_palette != null)
			{
				this.m_palette.Initialize("Palette", context);
				context.ExprHostBuilder.ChartPalette(this.m_palette);
			}
			if (this.m_dynamicHeight != null)
			{
				this.m_dynamicHeight.Initialize("DynamicHeight", context);
				context.ExprHostBuilder.DynamicHeight(this.m_dynamicHeight);
			}
			if (this.m_dynamicWidth != null)
			{
				this.m_dynamicWidth.Initialize("DynamicWidth", context);
				context.ExprHostBuilder.DynamicWidth(this.m_dynamicWidth);
			}
			if (this.m_paletteHatchBehavior != null)
			{
				this.m_paletteHatchBehavior.Initialize("PaletteHatchBehavior", context);
				context.ExprHostBuilder.ChartPaletteHatchBehavior(this.m_paletteHatchBehavior);
			}
			this.m_dataValueSequenceRendering = this.CalculateDataValueSequenceRendering();
		}

		protected override bool ValidateInnerStructure(InitializationContext context)
		{
			if (this.m_chartSeriesCollection != null && this.m_chartSeriesCollection.Count != 0)
			{
				return true;
			}
			context.ErrorContext.Register(ProcessingErrorCode.rsMissingChartDataPoints, Severity.Error, context.ObjectType, context.ObjectName, "ChartData");
			return false;
		}

		private bool CalculateDataValueSequenceRendering()
		{
			if (base.m_customProperties != null && this.m_chartSeriesCollection != null)
			{
				for (int i = 0; i < base.m_customProperties.Count; i++)
				{
					DataValue dataValue = base.m_customProperties[i];
					if (dataValue != null)
					{
						ExpressionInfo name = dataValue.Name;
						ExpressionInfo value = dataValue.Value;
						if (name != null && value != null && !name.IsExpression && !value.IsExpression && name.StringValue == "__Upgraded2005__" && value.StringValue == "__Upgraded2005__")
						{
							for (int j = 0; j < this.m_chartSeriesCollection.Count; j++)
							{
								ChartSeries chartSeries = this.m_chartSeriesCollection[j];
								if (chartSeries.Type != null)
								{
									if (chartSeries.Type.IsExpression || (chartSeries.Subtype != null && chartSeries.Subtype.IsExpression))
									{
										return false;
									}
									if (!this.IsYukonDataRendererType(chartSeries.Type.StringValue, (chartSeries.Subtype != null) ? chartSeries.Subtype.StringValue : null))
									{
										return false;
									}
								}
							}
							return true;
						}
					}
				}
			}
			return false;
		}

		private bool IsYukonDataRendererType(string type, string subType)
		{
			if (!AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(type, "Column") && !AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(type, "Bar") && !AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(type, "Line") && !AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(type, "Shape") && !AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(type, "Scatter") && !AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(type, "Area"))
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(type, "Range") && (subType == null || AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(subType, "Stock") || AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(subType, "CandleStick")))
				{
					return true;
				}
				return false;
			}
			return true;
		}

		internal ChartDataPoint GetDataPoint(int seriesIndex, int categoryIndex)
		{
			return this.m_chartSeriesCollection[seriesIndex].DataPoints[categoryIndex];
		}

		internal ChartDataPoint GetDataPoint(int cellIndex)
		{
			int index = cellIndex / this.CategoryCount;
			int index2 = cellIndex % this.CategoryCount;
			return this.m_chartSeriesCollection[index].DataPoints[index2];
		}

		internal ChartMember GetChartMember(ChartSeries chartSeries)
		{
			try
			{
				int memberCellIndex = this.m_chartSeriesCollection.IndexOf(chartSeries);
				return this.GetChartMember(this.m_seriesMembers, memberCellIndex);
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				return null;
			}
		}

		internal ChartMember GetChartMember(ChartMemberList chartMemberList, int memberCellIndex)
		{
			foreach (ChartMember chartMember3 in chartMemberList)
			{
				if (chartMember3.ChartMembers == null)
				{
					if (chartMember3.MemberCellIndex == memberCellIndex)
					{
						return chartMember3;
					}
				}
				else
				{
					ChartMember chartMember2 = this.GetChartMember(chartMember3.ChartMembers, memberCellIndex);
					if (chartMember2 != null)
					{
						return chartMember2;
					}
				}
			}
			return null;
		}

		internal List<ChartDerivedSeries> GetChildrenDerivedSeries(string chartSeriesName)
		{
			if (this.m_chartDerivedSeriesCollection == null)
			{
				return null;
			}
			List<ChartDerivedSeries> list = null;
			foreach (ChartDerivedSeries item in this.m_chartDerivedSeriesCollection)
			{
				if (AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(item.SourceChartSeriesName, chartSeriesName, false) == 0)
				{
					if (list == null)
					{
						list = new List<ChartDerivedSeries>();
					}
					list.Add(item);
				}
			}
			return list;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			Chart chart = (Chart)(context.CurrentDataRegionClone = (Chart)base.PublishClone(context));
			if (this.m_categoryMembers != null)
			{
				chart.m_categoryMembers = new ChartMemberList(this.m_categoryMembers.Count);
				foreach (ChartMember categoryMember in this.m_categoryMembers)
				{
					chart.m_categoryMembers.Add(categoryMember.PublishClone(context, chart));
				}
			}
			if (this.m_seriesMembers != null)
			{
				chart.m_seriesMembers = new ChartMemberList(this.m_seriesMembers.Count);
				foreach (ChartMember seriesMember in this.m_seriesMembers)
				{
					chart.m_seriesMembers.Add(seriesMember.PublishClone(context, chart));
				}
			}
			if (this.m_chartSeriesCollection != null)
			{
				chart.m_chartSeriesCollection = new ChartSeriesList(this.m_chartSeriesCollection.Count);
				foreach (ChartSeries item in this.m_chartSeriesCollection)
				{
					chart.m_chartSeriesCollection.Add((ChartSeries)item.PublishClone(context));
				}
			}
			if (this.m_chartDerivedSeriesCollection != null)
			{
				chart.m_chartDerivedSeriesCollection = new List<ChartDerivedSeries>(this.m_chartDerivedSeriesCollection.Count);
				foreach (ChartDerivedSeries item2 in this.m_chartDerivedSeriesCollection)
				{
					chart.m_chartDerivedSeriesCollection.Add((ChartDerivedSeries)item2.PublishClone(context));
				}
			}
			if (this.m_chartAreas != null)
			{
				chart.m_chartAreas = new List<ChartArea>(this.m_chartAreas.Count);
				foreach (ChartArea chartArea in this.m_chartAreas)
				{
					chart.m_chartAreas.Add((ChartArea)chartArea.PublishClone(context));
				}
			}
			if (this.m_legends != null)
			{
				chart.m_legends = new List<ChartLegend>(this.m_legends.Count);
				foreach (ChartLegend legend in this.m_legends)
				{
					chart.m_legends.Add((ChartLegend)legend.PublishClone(context));
				}
			}
			if (this.m_titles != null)
			{
				chart.m_titles = new List<ChartTitle>(this.m_titles.Count);
				foreach (ChartTitle title in this.m_titles)
				{
					chart.m_titles.Add((ChartTitle)title.PublishClone(context));
				}
			}
			if (this.m_codeParameters != null)
			{
				chart.m_codeParameters = new DataValueList(this.m_codeParameters.Count);
				foreach (DataValue codeParameter in this.m_codeParameters)
				{
					chart.m_codeParameters.Add((DataValue)codeParameter.PublishClone(context));
				}
			}
			if (this.m_customPaletteColors != null)
			{
				chart.m_customPaletteColors = new List<ChartCustomPaletteColor>(this.m_customPaletteColors.Count);
				foreach (ChartCustomPaletteColor customPaletteColor in this.m_customPaletteColors)
				{
					chart.m_customPaletteColors.Add((ChartCustomPaletteColor)customPaletteColor.PublishClone(context));
				}
			}
			if (this.m_noDataMessage != null)
			{
				chart.m_noDataMessage = (ChartNoDataMessage)this.m_noDataMessage.PublishClone(context);
			}
			if (this.m_borderSkin != null)
			{
				chart.m_borderSkin = (ChartBorderSkin)this.m_borderSkin.PublishClone(context);
			}
			if (this.m_dynamicHeight != null)
			{
				chart.m_dynamicHeight = (ExpressionInfo)this.m_dynamicHeight.PublishClone(context);
			}
			if (this.m_dynamicWidth != null)
			{
				chart.m_dynamicWidth = (ExpressionInfo)this.m_dynamicWidth.PublishClone(context);
			}
			if (this.m_palette != null)
			{
				chart.m_palette = (ExpressionInfo)this.m_palette.PublishClone(context);
			}
			if (this.m_paletteHatchBehavior != null)
			{
				chart.m_paletteHatchBehavior = (ExpressionInfo)this.m_paletteHatchBehavior.PublishClone(context);
			}
			return chart;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.CategoryMembers, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMember));
			list.Add(new MemberInfo(MemberName.SeriesMembers, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMember));
			list.Add(new MemberInfo(MemberName.ChartSeriesCollection, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries));
			list.Add(new MemberInfo(MemberName.ChartDerivedSeriesCollection, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDerivedSeries));
			list.Add(new MemberInfo(MemberName.Palette, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartAreas, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartArea));
			list.Add(new MemberInfo(MemberName.ChartLegends, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegend));
			list.Add(new MemberInfo(MemberName.Titles, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTitle));
			list.Add(new MemberInfo(MemberName.CodeParameters, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataValue));
			list.Add(new MemberInfo(MemberName.CustomPaletteColors, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartCustomPaletteColor));
			list.Add(new MemberInfo(MemberName.BorderSkin, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartBorderSkin));
			list.Add(new MemberInfo(MemberName.NoDataMessage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartNoDataMessage));
			list.Add(new MemberInfo(MemberName.DynamicHeight, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DynamicWidth, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DataValueSequenceRendering, Token.Boolean));
			list.Add(new MemberInfo(MemberName.PaletteHatchBehavior, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ColumnGroupingIsSwitched, Token.Boolean));
			list.Add(new MemberInfo(MemberName.EnableCategoryDrilldown, Token.Boolean, Lifetime.AddedIn(200)));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion, list);
		}

		internal int GenerateActionOwnerID()
		{
			return ++this.m_actionOwnerCounter;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(Chart.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.CategoryMembers:
					writer.Write(this.m_categoryMembers);
					break;
				case MemberName.SeriesMembers:
					writer.Write(this.m_seriesMembers);
					break;
				case MemberName.ChartSeriesCollection:
					writer.Write(this.m_chartSeriesCollection);
					break;
				case MemberName.ChartDerivedSeriesCollection:
					writer.Write(this.m_chartDerivedSeriesCollection);
					break;
				case MemberName.Palette:
					writer.Write(this.m_palette);
					break;
				case MemberName.ChartAreas:
					writer.Write(this.m_chartAreas);
					break;
				case MemberName.Titles:
					writer.Write(this.m_titles);
					break;
				case MemberName.ChartLegends:
					writer.Write(this.m_legends);
					break;
				case MemberName.NoDataMessage:
					writer.Write(this.m_noDataMessage);
					break;
				case MemberName.BorderSkin:
					writer.Write(this.m_borderSkin);
					break;
				case MemberName.DynamicHeight:
					writer.Write(this.m_dynamicHeight);
					break;
				case MemberName.DynamicWidth:
					writer.Write(this.m_dynamicWidth);
					break;
				case MemberName.CodeParameters:
					writer.Write(this.m_codeParameters);
					break;
				case MemberName.CustomPaletteColors:
					writer.Write(this.m_customPaletteColors);
					break;
				case MemberName.DataValueSequenceRendering:
					writer.Write(this.m_dataValueSequenceRendering);
					break;
				case MemberName.PaletteHatchBehavior:
					writer.Write(this.m_paletteHatchBehavior);
					break;
				case MemberName.ColumnGroupingIsSwitched:
					writer.Write(this.m_columnGroupingIsSwitched);
					break;
				case MemberName.EnableCategoryDrilldown:
					writer.Write(this.m_enableCategoryDrilldown);
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
			reader.RegisterDeclaration(Chart.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.CategoryMembers:
					this.m_categoryMembers = reader.ReadListOfRIFObjects<ChartMemberList>();
					break;
				case MemberName.SeriesMembers:
					this.m_seriesMembers = reader.ReadListOfRIFObjects<ChartMemberList>();
					break;
				case MemberName.ChartSeriesCollection:
					this.m_chartSeriesCollection = reader.ReadListOfRIFObjects<ChartSeriesList>();
					break;
				case MemberName.ChartDerivedSeriesCollection:
					this.m_chartDerivedSeriesCollection = reader.ReadGenericListOfRIFObjects<ChartDerivedSeries>();
					break;
				case MemberName.Palette:
					this.m_palette = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartLegends:
					this.m_legends = reader.ReadGenericListOfRIFObjects<ChartLegend>();
					break;
				case MemberName.ChartAreas:
					this.m_chartAreas = reader.ReadGenericListOfRIFObjects<ChartArea>();
					break;
				case MemberName.Titles:
					this.m_titles = reader.ReadGenericListOfRIFObjects<ChartTitle>();
					break;
				case MemberName.DynamicHeight:
					this.m_dynamicHeight = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DynamicWidth:
					this.m_dynamicWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CodeParameters:
					this.m_codeParameters = reader.ReadListOfRIFObjects<DataValueList>();
					break;
				case MemberName.CustomPaletteColors:
					this.m_customPaletteColors = reader.ReadGenericListOfRIFObjects<ChartCustomPaletteColor>();
					break;
				case MemberName.NoDataMessage:
					this.m_noDataMessage = (ChartNoDataMessage)reader.ReadRIFObject();
					break;
				case MemberName.BorderSkin:
					this.m_borderSkin = (ChartBorderSkin)reader.ReadRIFObject();
					break;
				case MemberName.DataValueSequenceRendering:
					this.m_dataValueSequenceRendering = reader.ReadBoolean();
					break;
				case MemberName.PaletteHatchBehavior:
					this.m_paletteHatchBehavior = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ColumnGroupingIsSwitched:
					this.m_columnGroupingIsSwitched = reader.ReadBoolean();
					break;
				case MemberName.EnableCategoryDrilldown:
					this.m_enableCategoryDrilldown = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				this.m_chartExprHost = reportExprHost.ChartHostsRemotable[base.ExprHostID];
				base.DataRegionSetExprHost(this.m_chartExprHost, this.m_chartExprHost.SortHost, this.m_chartExprHost.FilterHostsRemotable, this.m_chartExprHost.UserSortExpressionsHost, this.m_chartExprHost.PageBreakExprHost, this.m_chartExprHost.JoinConditionExprHostsRemotable, reportObjectModel);
			}
		}

		internal override void DataRegionContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions)
		{
			if (this.m_chartExprHost != null)
			{
				IList<ChartAreaExprHost> chartAreasHostsRemotable = this.m_chartExprHost.ChartAreasHostsRemotable;
				if (this.m_chartAreas != null && chartAreasHostsRemotable != null)
				{
					for (int i = 0; i < this.m_chartAreas.Count; i++)
					{
						ChartArea chartArea = this.m_chartAreas[i];
						if (chartArea != null && chartArea.ExpressionHostID > -1)
						{
							chartArea.SetExprHost(chartAreasHostsRemotable[chartArea.ExpressionHostID], reportObjectModel);
						}
					}
				}
				IList<ChartTitleExprHost> titlesHostsRemotable = this.m_chartExprHost.TitlesHostsRemotable;
				if (this.m_titles != null && titlesHostsRemotable != null)
				{
					for (int j = 0; j < this.m_titles.Count; j++)
					{
						ChartTitle chartTitle = this.m_titles[j];
						if (chartTitle != null && chartTitle.ExpressionHostID > -1)
						{
							chartTitle.SetExprHost(titlesHostsRemotable[chartTitle.ExpressionHostID], reportObjectModel);
						}
					}
				}
				IList<ChartLegendExprHost> legendsHostsRemotable = this.m_chartExprHost.LegendsHostsRemotable;
				if (this.m_legends != null && legendsHostsRemotable != null)
				{
					for (int k = 0; k < this.m_legends.Count; k++)
					{
						ChartLegend chartLegend = this.m_legends[k];
						if (chartLegend != null && chartLegend.ExpressionHostID > -1)
						{
							chartLegend.SetExprHost(legendsHostsRemotable[chartLegend.ExpressionHostID], reportObjectModel);
						}
					}
				}
				IList<ChartCustomPaletteColorExprHost> customPaletteColorHostsRemotable = this.m_chartExprHost.CustomPaletteColorHostsRemotable;
				if (this.m_customPaletteColors != null && customPaletteColorHostsRemotable != null)
				{
					for (int l = 0; l < this.m_customPaletteColors.Count; l++)
					{
						ChartCustomPaletteColor chartCustomPaletteColor = this.m_customPaletteColors[l];
						if (chartCustomPaletteColor != null && chartCustomPaletteColor.ExpressionHostID > -1)
						{
							chartCustomPaletteColor.SetExprHost(customPaletteColorHostsRemotable[chartCustomPaletteColor.ExpressionHostID], reportObjectModel);
						}
					}
				}
				if (this.m_codeParameters != null && this.m_chartExprHost.CodeParametersHostsRemotable != null)
				{
					this.m_codeParameters.SetExprHost(this.m_chartExprHost.CodeParametersHostsRemotable, reportObjectModel);
				}
				if (this.m_borderSkin != null && this.m_chartExprHost.BorderSkinHost != null)
				{
					this.m_borderSkin.SetExprHost(this.m_chartExprHost.BorderSkinHost, reportObjectModel);
				}
				if (this.m_noDataMessage != null && this.m_chartExprHost.NoDataMessageHost != null)
				{
					this.m_noDataMessage.SetExprHost(this.m_chartExprHost.NoDataMessageHost, reportObjectModel);
				}
				IList<ChartSeriesExprHost> seriesCollectionHostsRemotable = this.m_chartExprHost.SeriesCollectionHostsRemotable;
				IList<ChartDataPointExprHost> cellHostsRemotable = this.m_chartExprHost.CellHostsRemotable;
				Global.Tracer.Assert(this.m_chartSeriesCollection != null, "(m_chartSeriesCollection != null)");
				for (int m = 0; m < this.m_chartSeriesCollection.Count; m++)
				{
					ChartSeries chartSeries = this.m_chartSeriesCollection[m];
					Global.Tracer.Assert(null != chartSeries, "(null != series)");
					if (seriesCollectionHostsRemotable != null && chartSeries.ExpressionHostID > -1)
					{
						chartSeries.SetExprHost(seriesCollectionHostsRemotable[chartSeries.ExpressionHostID], reportObjectModel);
					}
					if (cellHostsRemotable != null)
					{
						Global.Tracer.Assert(null != chartSeries.DataPoints, "(null != series.DataPoints)");
						for (int n = 0; n < chartSeries.DataPoints.Count; n++)
						{
							ChartDataPoint chartDataPoint = chartSeries.DataPoints[n];
							Global.Tracer.Assert(null != chartDataPoint, "(null != dataPoint)");
							if (chartDataPoint.ExpressionHostID > -1)
							{
								chartDataPoint.SetExprHost(cellHostsRemotable[chartDataPoint.ExpressionHostID], reportObjectModel);
							}
						}
					}
				}
			}
		}

		internal override object EvaluateNoRowsMessageExpression()
		{
			return this.m_chartExprHost.NoRowsExpr;
		}

		internal string EvaluateDynamicWidth(AspNetCore.ReportingServices.OnDemandReportRendering.ChartInstance chartInstance, OnDemandProcessingContext context)
		{
			if (this.m_dynamicWidth == null)
			{
				return null;
			}
			context.SetupContext(this, chartInstance);
			return context.ReportRuntime.EvaluateChartDynamicSizeExpression(this, this.m_dynamicWidth, "DynamicWidth", true);
		}

		internal string EvaluateDynamicHeight(AspNetCore.ReportingServices.OnDemandReportRendering.ChartInstance chartInstance, OnDemandProcessingContext context)
		{
			if (this.m_dynamicHeight == null)
			{
				return null;
			}
			context.SetupContext(this, chartInstance);
			return context.ReportRuntime.EvaluateChartDynamicSizeExpression(this, this.m_dynamicHeight, "DynamicHeight", false);
		}

		internal ChartPalette EvaluatePalette(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return EnumTranslator.TranslateChartPalette(context.ReportRuntime.EvaluateChartPaletteExpression(this, base.Name), context.ReportRuntime);
		}

		internal PaletteHatchBehavior EvaluatePaletteHatchBehavior(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return EnumTranslator.TranslatePaletteHatchBehavior(context.ReportRuntime.EvaluateChartPaletteHatchBehaviorExpression(this, base.Name), context.ReportRuntime);
		}

		protected override ReportHierarchyNode CreateHierarchyNode(int id)
		{
			return new ChartMember(id, this);
		}

		protected override Row CreateRow(int id, int columnCount)
		{
			ChartSeries chartSeries = new ChartSeries(this, id);
			chartSeries.DataPoints = new ChartDataPointList(columnCount);
			return chartSeries;
		}

		protected override Cell CreateCell(int id, int rowIndex, int colIndex)
		{
			return new ChartDataPoint(id, this);
		}
	}
}
