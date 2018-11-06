using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimChartSeries : ChartSeries
	{
		private List<ShimChartDataPoint> m_cells;

		private bool m_plotAsLine;

		private ReportEnumProperty<ChartSeriesType> m_chartSeriesType;

		private ReportEnumProperty<ChartSeriesSubtype> m_chartSeriesSubtype;

		public override ChartDataPoint this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					return this.m_cells[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_cells.Count;
			}
		}

		public override string Name
		{
			get
			{
				return null;
			}
		}

		public override Style Style
		{
			get
			{
				return null;
			}
		}

		internal override ActionInfo ActionInfo
		{
			get
			{
				return null;
			}
		}

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				return null;
			}
		}

		public override ReportEnumProperty<ChartSeriesType> Type
		{
			get
			{
				return this.m_chartSeriesType;
			}
		}

		public override ReportEnumProperty<ChartSeriesSubtype> Subtype
		{
			get
			{
				return this.m_chartSeriesSubtype;
			}
		}

		public override ChartSmartLabel SmartLabel
		{
			get
			{
				return null;
			}
		}

		public override ChartEmptyPoints EmptyPoints
		{
			get
			{
				return null;
			}
		}

		public override ReportStringProperty LegendName
		{
			get
			{
				return null;
			}
		}

		internal override ReportStringProperty LegendText
		{
			get
			{
				return null;
			}
		}

		internal override ReportBoolProperty HideInLegend
		{
			get
			{
				return null;
			}
		}

		public override ReportStringProperty ChartAreaName
		{
			get
			{
				return null;
			}
		}

		public override ReportStringProperty ValueAxisName
		{
			get
			{
				return null;
			}
		}

		public override ReportStringProperty CategoryAxisName
		{
			get
			{
				return null;
			}
		}

		public override ChartDataLabel DataLabel
		{
			get
			{
				return null;
			}
		}

		public override ChartMarker Marker
		{
			get
			{
				return null;
			}
		}

		internal override ReportStringProperty ToolTip
		{
			get
			{
				return null;
			}
		}

		public override ReportBoolProperty Hidden
		{
			get
			{
				return null;
			}
		}

		public override ChartItemInLegend ChartItemInLegend
		{
			get
			{
				return null;
			}
		}

		public override ChartSeriesInstance Instance
		{
			get
			{
				return null;
			}
		}

		internal ShimChartSeries(Chart owner, int seriesIndex, ShimChartMember seriesParentMember)
			: base(owner, seriesIndex)
		{
			this.m_cells = new List<ShimChartDataPoint>();
			this.m_plotAsLine = seriesParentMember.CurrentRenderChartMember.IsPlotTypeLine();
			this.GenerateChartDataPoints(seriesParentMember, null, owner.CategoryHierarchy.MemberCollection as ShimChartMemberCollection);
		}

		private void GenerateChartDataPoints(ShimChartMember seriesParentMember, ShimChartMember categoryParentMember, ShimChartMemberCollection categoryMembers)
		{
			if (categoryMembers == null)
			{
				this.m_cells.Add(new ShimChartDataPoint(base.m_chart, base.m_seriesIndex, this.m_cells.Count, seriesParentMember, categoryParentMember));
				this.TranslateChartType(base.m_chart.RenderChartDef.Type, base.m_chart.RenderChartDef.SubType);
			}
			else
			{
				int count = categoryMembers.Count;
				for (int i = 0; i < count; i++)
				{
					ShimChartMember shimChartMember = ((ReportElementCollectionBase<ChartMember>)categoryMembers)[i] as ShimChartMember;
					this.GenerateChartDataPoints(seriesParentMember, shimChartMember, shimChartMember.Children as ShimChartMemberCollection);
				}
			}
		}

		private void TranslateChartType(AspNetCore.ReportingServices.ReportProcessing.Chart.ChartTypes chartType, AspNetCore.ReportingServices.ReportProcessing.Chart.ChartSubTypes chartSubType)
		{
			ChartSeriesType value = ChartSeriesType.Column;
			ChartSeriesSubtype value2 = ChartSeriesSubtype.Plain;
			if (this.m_plotAsLine && chartType != AspNetCore.ReportingServices.ReportProcessing.Chart.ChartTypes.Line)
			{
				value = ChartSeriesType.Line;
				value2 = ChartSeriesSubtype.Plain;
			}
			else
			{
				switch (chartType)
				{
				case AspNetCore.ReportingServices.ReportProcessing.Chart.ChartTypes.Area:
					value = ChartSeriesType.Area;
					value2 = this.TranslateChartSubType(chartSubType);
					break;
				case AspNetCore.ReportingServices.ReportProcessing.Chart.ChartTypes.Bar:
					value = ChartSeriesType.Bar;
					value2 = this.TranslateChartSubType(chartSubType);
					break;
				case AspNetCore.ReportingServices.ReportProcessing.Chart.ChartTypes.Column:
					value = ChartSeriesType.Column;
					value2 = this.TranslateChartSubType(chartSubType);
					break;
				case AspNetCore.ReportingServices.ReportProcessing.Chart.ChartTypes.Line:
					value = ChartSeriesType.Line;
					value2 = this.TranslateChartSubType(chartSubType);
					break;
				case AspNetCore.ReportingServices.ReportProcessing.Chart.ChartTypes.Pie:
					value = ChartSeriesType.Shape;
					value2 = (ChartSeriesSubtype)((chartSubType != AspNetCore.ReportingServices.ReportProcessing.Chart.ChartSubTypes.Exploded) ? 5 : 6);
					break;
				case AspNetCore.ReportingServices.ReportProcessing.Chart.ChartTypes.Doughnut:
					value = ChartSeriesType.Shape;
					value2 = (ChartSeriesSubtype)((chartSubType != AspNetCore.ReportingServices.ReportProcessing.Chart.ChartSubTypes.Exploded) ? 7 : 8);
					break;
				case AspNetCore.ReportingServices.ReportProcessing.Chart.ChartTypes.Scatter:
					if (chartSubType == AspNetCore.ReportingServices.ReportProcessing.Chart.ChartSubTypes.Plain)
					{
						value = ChartSeriesType.Scatter;
						value2 = ChartSeriesSubtype.Plain;
					}
					else
					{
						value = ChartSeriesType.Line;
						value2 = (ChartSeriesSubtype)((chartSubType != AspNetCore.ReportingServices.ReportProcessing.Chart.ChartSubTypes.Line) ? 3 : 0);
					}
					break;
				case AspNetCore.ReportingServices.ReportProcessing.Chart.ChartTypes.Bubble:
					value = ChartSeriesType.Scatter;
					value2 = ChartSeriesSubtype.Bubble;
					break;
				case AspNetCore.ReportingServices.ReportProcessing.Chart.ChartTypes.Stock:
					value = ChartSeriesType.Range;
					value2 = this.TranslateChartSubType(chartSubType);
					break;
				}
			}
			this.m_chartSeriesType = new ReportEnumProperty<ChartSeriesType>(value);
			this.m_chartSeriesSubtype = new ReportEnumProperty<ChartSeriesSubtype>(value2);
		}

		private ChartSeriesSubtype TranslateChartSubType(AspNetCore.ReportingServices.ReportProcessing.Chart.ChartSubTypes chartSubTypes)
		{
			switch (chartSubTypes)
			{
			case AspNetCore.ReportingServices.ReportProcessing.Chart.ChartSubTypes.Candlestick:
				return ChartSeriesSubtype.Candlestick;
			case AspNetCore.ReportingServices.ReportProcessing.Chart.ChartSubTypes.HighLowClose:
			case AspNetCore.ReportingServices.ReportProcessing.Chart.ChartSubTypes.OpenHighLowClose:
				return ChartSeriesSubtype.Stock;
			case AspNetCore.ReportingServices.ReportProcessing.Chart.ChartSubTypes.PercentStacked:
				return ChartSeriesSubtype.PercentStacked;
			case AspNetCore.ReportingServices.ReportProcessing.Chart.ChartSubTypes.Smooth:
				return ChartSeriesSubtype.Smooth;
			case AspNetCore.ReportingServices.ReportProcessing.Chart.ChartSubTypes.Stacked:
				return ChartSeriesSubtype.Stacked;
			default:
				return ChartSeriesSubtype.Plain;
			}
		}

		internal override void SetNewContext()
		{
		}
	}
}
