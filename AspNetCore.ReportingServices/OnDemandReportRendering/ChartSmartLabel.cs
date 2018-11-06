using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartSmartLabel
	{
		private Chart m_chart;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSmartLabel m_chartSmartLabelDef;

		private ChartSmartLabelInstance m_instance;

		private ReportEnumProperty<ChartAllowOutsideChartArea> m_allowOutSidePlotArea;

		private ReportColorProperty m_calloutBackColor;

		private ReportEnumProperty<ChartCalloutLineAnchor> m_calloutLineAnchor;

		private ReportColorProperty m_calloutLineColor;

		private ReportEnumProperty<ChartCalloutLineStyle> m_calloutLineStyle;

		private ReportSizeProperty m_calloutLineWidth;

		private ReportEnumProperty<ChartCalloutStyle> m_calloutStyle;

		private ReportBoolProperty m_showOverlapped;

		private ReportBoolProperty m_markerOverlapping;

		private ReportSizeProperty m_maxMovingDistance;

		private ReportSizeProperty m_minMovingDistance;

		private ChartNoMoveDirections m_noMoveDirections;

		private ReportBoolProperty m_disabled;

		private InternalChartSeries m_chartSeries;

		public ReportEnumProperty<ChartAllowOutsideChartArea> AllowOutSidePlotArea
		{
			get
			{
				if (this.m_allowOutSidePlotArea == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_allowOutSidePlotArea = new ReportEnumProperty<ChartAllowOutsideChartArea>(ChartAllowOutsideChartArea.False);
					}
					else if (this.m_chartSmartLabelDef.AllowOutSidePlotArea != null)
					{
						this.m_allowOutSidePlotArea = new ReportEnumProperty<ChartAllowOutsideChartArea>(this.m_chartSmartLabelDef.AllowOutSidePlotArea.IsExpression, this.m_chartSmartLabelDef.AllowOutSidePlotArea.OriginalText, EnumTranslator.TranslateChartAllowOutsideChartArea(this.m_chartSmartLabelDef.AllowOutSidePlotArea.StringValue, null));
					}
				}
				return this.m_allowOutSidePlotArea;
			}
		}

		public ReportColorProperty CalloutBackColor
		{
			get
			{
				if (this.m_calloutBackColor == null && !this.m_chart.IsOldSnapshot && this.m_chartSmartLabelDef.CalloutBackColor != null)
				{
					ExpressionInfo calloutBackColor = this.m_chartSmartLabelDef.CalloutBackColor;
					this.m_calloutBackColor = new ReportColorProperty(calloutBackColor.IsExpression, calloutBackColor.OriginalText, calloutBackColor.IsExpression ? null : new ReportColor(calloutBackColor.StringValue.Trim(), true), calloutBackColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
				}
				return this.m_calloutBackColor;
			}
		}

		public ReportEnumProperty<ChartCalloutLineAnchor> CalloutLineAnchor
		{
			get
			{
				if (this.m_calloutLineAnchor == null && !this.m_chart.IsOldSnapshot && this.m_chartSmartLabelDef.CalloutLineAnchor != null)
				{
					this.m_calloutLineAnchor = new ReportEnumProperty<ChartCalloutLineAnchor>(this.m_chartSmartLabelDef.CalloutLineAnchor.IsExpression, this.m_chartSmartLabelDef.CalloutLineAnchor.OriginalText, EnumTranslator.TranslateChartCalloutLineAnchor(this.m_chartSmartLabelDef.CalloutLineAnchor.StringValue, null));
				}
				return this.m_calloutLineAnchor;
			}
		}

		public ReportColorProperty CalloutLineColor
		{
			get
			{
				if (this.m_calloutLineColor == null && !this.m_chart.IsOldSnapshot && this.m_chartSmartLabelDef.CalloutLineColor != null)
				{
					ExpressionInfo calloutLineColor = this.m_chartSmartLabelDef.CalloutLineColor;
					this.m_calloutLineColor = new ReportColorProperty(calloutLineColor.IsExpression, calloutLineColor.OriginalText, calloutLineColor.IsExpression ? null : new ReportColor(calloutLineColor.StringValue.Trim(), true), calloutLineColor.IsExpression ? new ReportColor("", Color.Black, true) : null);
				}
				return this.m_calloutLineColor;
			}
		}

		public ReportEnumProperty<ChartCalloutLineStyle> CalloutLineStyle
		{
			get
			{
				if (this.m_calloutLineStyle == null && !this.m_chart.IsOldSnapshot && this.m_chartSmartLabelDef.CalloutLineStyle != null)
				{
					this.m_calloutLineStyle = new ReportEnumProperty<ChartCalloutLineStyle>(this.m_chartSmartLabelDef.CalloutLineStyle.IsExpression, this.m_chartSmartLabelDef.CalloutLineStyle.OriginalText, EnumTranslator.TranslateChartCalloutLineStyle(this.m_chartSmartLabelDef.CalloutLineStyle.StringValue, null));
				}
				return this.m_calloutLineStyle;
			}
		}

		public ReportSizeProperty CalloutLineWidth
		{
			get
			{
				if (this.m_calloutLineWidth == null && !this.m_chart.IsOldSnapshot && this.m_chartSmartLabelDef.CalloutLineWidth != null)
				{
					this.m_calloutLineWidth = new ReportSizeProperty(this.m_chartSmartLabelDef.CalloutLineWidth);
				}
				return this.m_calloutLineWidth;
			}
		}

		public ReportEnumProperty<ChartCalloutStyle> CalloutStyle
		{
			get
			{
				if (this.m_calloutStyle == null && !this.m_chart.IsOldSnapshot && this.m_chartSmartLabelDef.CalloutStyle != null)
				{
					this.m_calloutStyle = new ReportEnumProperty<ChartCalloutStyle>(this.m_chartSmartLabelDef.CalloutStyle.IsExpression, this.m_chartSmartLabelDef.CalloutStyle.OriginalText, EnumTranslator.TranslateChartCalloutStyle(this.m_chartSmartLabelDef.CalloutStyle.StringValue, null));
				}
				return this.m_calloutStyle;
			}
		}

		public ReportBoolProperty ShowOverlapped
		{
			get
			{
				if (this.m_showOverlapped == null && !this.m_chart.IsOldSnapshot && this.m_chartSmartLabelDef.ShowOverlapped != null)
				{
					this.m_showOverlapped = new ReportBoolProperty(this.m_chartSmartLabelDef.ShowOverlapped);
				}
				return this.m_showOverlapped;
			}
		}

		public ReportBoolProperty MarkerOverlapping
		{
			get
			{
				if (this.m_markerOverlapping == null && !this.m_chart.IsOldSnapshot && this.m_chartSmartLabelDef.MarkerOverlapping != null)
				{
					this.m_markerOverlapping = new ReportBoolProperty(this.m_chartSmartLabelDef.MarkerOverlapping);
				}
				return this.m_markerOverlapping;
			}
		}

		public ReportSizeProperty MaxMovingDistance
		{
			get
			{
				if (this.m_maxMovingDistance == null && !this.m_chart.IsOldSnapshot && this.m_chartSmartLabelDef.MaxMovingDistance != null)
				{
					this.m_maxMovingDistance = new ReportSizeProperty(this.m_chartSmartLabelDef.MaxMovingDistance);
				}
				return this.m_maxMovingDistance;
			}
		}

		public ReportSizeProperty MinMovingDistance
		{
			get
			{
				if (this.m_minMovingDistance == null && !this.m_chart.IsOldSnapshot && this.m_chartSmartLabelDef.MinMovingDistance != null)
				{
					this.m_minMovingDistance = new ReportSizeProperty(this.m_chartSmartLabelDef.MinMovingDistance);
				}
				return this.m_minMovingDistance;
			}
		}

		public ChartNoMoveDirections NoMoveDirections
		{
			get
			{
				if (this.m_noMoveDirections == null && !this.m_chart.IsOldSnapshot && this.m_chartSmartLabelDef.NoMoveDirections != null)
				{
					this.m_noMoveDirections = new ChartNoMoveDirections(this.m_chartSeries, this.m_chartSmartLabelDef.NoMoveDirections, this.m_chart);
				}
				return this.m_noMoveDirections;
			}
		}

		public ReportBoolProperty Disabled
		{
			get
			{
				if (this.m_disabled == null && !this.m_chart.IsOldSnapshot && this.m_chartSmartLabelDef.Disabled != null)
				{
					this.m_disabled = new ReportBoolProperty(this.m_chartSmartLabelDef.Disabled);
				}
				return this.m_disabled;
			}
		}

		internal IReportScope ReportScope
		{
			get
			{
				if (this.m_chartSeries != null)
				{
					return this.m_chartSeries.ReportScope;
				}
				return this.m_chart;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSmartLabel ChartSmartLabelDef
		{
			get
			{
				return this.m_chartSmartLabelDef;
			}
		}

		public ChartSmartLabelInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new ChartSmartLabelInstance(this);
				}
				return this.m_instance;
			}
		}

		internal ChartSmartLabel(InternalChartSeries chartSeries, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabelDef, Chart chart)
		{
			this.m_chartSeries = chartSeries;
			this.m_chartSmartLabelDef = chartSmartLabelDef;
			this.m_chart = chart;
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			if (this.m_noMoveDirections != null)
			{
				this.m_noMoveDirections.SetNewContext();
			}
		}
	}
}
