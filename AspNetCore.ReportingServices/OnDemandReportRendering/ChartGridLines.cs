using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartGridLines : IROMStyleDefinitionContainer
	{
		private Chart m_chart;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartGridLines m_gridLinesDef;

		private GridLines m_renderGridLinesDef;

		private object[] m_styleValues;

		private Style m_style;

		private ChartGridLinesInstance m_instance;

		private ReportEnumProperty<ChartAutoBool> m_enabled;

		private ReportDoubleProperty m_interval;

		private ReportDoubleProperty m_intervalOffset;

		private ReportEnumProperty<ChartIntervalType> m_intervalType;

		private ReportEnumProperty<ChartIntervalType> m_intervalOffsetType;

		public ReportEnumProperty<ChartAutoBool> Enabled
		{
			get
			{
				if (this.m_enabled == null && !this.m_chart.IsOldSnapshot && this.m_gridLinesDef.Enabled != null)
				{
					this.m_enabled = new ReportEnumProperty<ChartAutoBool>(this.m_gridLinesDef.Enabled.IsExpression, this.m_gridLinesDef.Enabled.OriginalText, EnumTranslator.TranslateChartAutoBool(this.m_gridLinesDef.Enabled.StringValue, null));
				}
				return this.m_enabled;
			}
		}

		public Style Style
		{
			get
			{
				if (this.m_style == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_style = new Style(this.m_renderGridLinesDef.StyleClass, this.m_styleValues, this.m_chart.RenderingContext);
					}
					else if (this.m_gridLinesDef.StyleClass != null)
					{
						this.m_style = new Style(this.m_chart, this.m_chart, this.m_gridLinesDef, this.m_chart.RenderingContext);
					}
				}
				return this.m_style;
			}
		}

		public ReportDoubleProperty Interval
		{
			get
			{
				if (this.m_interval == null && !this.m_chart.IsOldSnapshot && this.m_gridLinesDef.Interval != null)
				{
					this.m_interval = new ReportDoubleProperty(this.m_gridLinesDef.Interval);
				}
				return this.m_interval;
			}
		}

		public ReportDoubleProperty IntervalOffset
		{
			get
			{
				if (this.m_intervalOffset == null && !this.m_chart.IsOldSnapshot && this.m_gridLinesDef.IntervalOffset != null)
				{
					this.m_intervalOffset = new ReportDoubleProperty(this.m_gridLinesDef.IntervalOffset);
				}
				return this.m_intervalOffset;
			}
		}

		public ReportEnumProperty<ChartIntervalType> IntervalType
		{
			get
			{
				if (this.m_intervalType == null && !this.m_chart.IsOldSnapshot && this.m_gridLinesDef.IntervalType != null)
				{
					this.m_intervalType = new ReportEnumProperty<ChartIntervalType>(this.m_gridLinesDef.IntervalType.IsExpression, this.m_gridLinesDef.IntervalType.OriginalText, EnumTranslator.TranslateChartIntervalType(this.m_gridLinesDef.IntervalType.StringValue, null));
				}
				return this.m_intervalType;
			}
		}

		public ReportEnumProperty<ChartIntervalType> IntervalOffsetType
		{
			get
			{
				if (this.m_intervalOffsetType == null && !this.m_chart.IsOldSnapshot && this.m_gridLinesDef.IntervalOffsetType != null)
				{
					this.m_intervalOffsetType = new ReportEnumProperty<ChartIntervalType>(this.m_gridLinesDef.IntervalOffsetType.IsExpression, this.m_gridLinesDef.IntervalOffsetType.OriginalText, EnumTranslator.TranslateChartIntervalType(this.m_gridLinesDef.IntervalOffsetType.StringValue, null));
				}
				return this.m_intervalOffsetType;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartGridLines ChartGridLinesDef
		{
			get
			{
				return this.m_gridLinesDef;
			}
		}

		public ChartGridLinesInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new ChartGridLinesInstance(this);
				}
				return this.m_instance;
			}
		}

		internal ChartGridLines(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartGridLines gridLinesDef, Chart chart)
		{
			this.m_chart = chart;
			this.m_gridLinesDef = gridLinesDef;
		}

		internal ChartGridLines(GridLines renderGridLinesDef, object[] styleValues, Chart chart)
		{
			this.m_chart = chart;
			this.m_renderGridLinesDef = renderGridLinesDef;
			this.m_styleValues = styleValues;
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
		}
	}
}
