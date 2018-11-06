using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendTitle : IROMStyleDefinitionContainer
	{
		private Chart m_chart;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendTitle m_chartLegendTitleDef;

		private ChartLegendTitleInstance m_instance;

		private Style m_style;

		private ReportStringProperty m_caption;

		private ReportEnumProperty<ChartSeparators> m_titleSeparator;

		public Style Style
		{
			get
			{
				if (this.m_style == null && !this.m_chart.IsOldSnapshot)
				{
					this.m_style = new Style(this.m_chart, this.m_chart, this.m_chartLegendTitleDef, this.m_chart.RenderingContext);
				}
				return this.m_style;
			}
		}

		public ReportStringProperty Caption
		{
			get
			{
				if (this.m_caption == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendTitleDef.Caption != null)
				{
					this.m_caption = new ReportStringProperty(this.m_chartLegendTitleDef.Caption);
				}
				return this.m_caption;
			}
		}

		public ReportEnumProperty<ChartSeparators> TitleSeparator
		{
			get
			{
				if (this.m_titleSeparator == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendTitleDef.TitleSeparator != null)
				{
					this.m_titleSeparator = new ReportEnumProperty<ChartSeparators>(this.m_chartLegendTitleDef.TitleSeparator.IsExpression, this.m_chartLegendTitleDef.TitleSeparator.OriginalText, EnumTranslator.TranslateChartSeparator(this.m_chartLegendTitleDef.TitleSeparator.StringValue, null));
				}
				return this.m_titleSeparator;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendTitle ChartLegendTitleDef
		{
			get
			{
				return this.m_chartLegendTitleDef;
			}
		}

		public ChartLegendTitleInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new ChartLegendTitleInstance(this);
				}
				return this.m_instance;
			}
		}

		internal ChartLegendTitle(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendTitle chartLegendTitleDef, Chart chart)
		{
			this.m_chartLegendTitleDef = chartLegendTitleDef;
			this.m_chart = chart;
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
