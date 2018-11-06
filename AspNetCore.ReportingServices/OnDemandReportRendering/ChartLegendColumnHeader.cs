using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendColumnHeader : IROMStyleDefinitionContainer
	{
		private Chart m_chart;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumnHeader m_chartLegendColumnHeaderDef;

		private ChartLegendColumnHeaderInstance m_instance;

		private Style m_style;

		private ReportStringProperty m_value;

		public Style Style
		{
			get
			{
				if (this.m_style == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendColumnHeaderDef.StyleClass != null)
				{
					this.m_style = new Style(this.m_chart, this.m_chart, this.m_chartLegendColumnHeaderDef, this.m_chart.RenderingContext);
				}
				return this.m_style;
			}
		}

		public ReportStringProperty Value
		{
			get
			{
				if (this.m_value == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendColumnHeaderDef.Value != null)
				{
					this.m_value = new ReportStringProperty(this.m_chartLegendColumnHeaderDef.Value);
				}
				return this.m_value;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumnHeader ChartLegendColumnHeaderDef
		{
			get
			{
				return this.m_chartLegendColumnHeaderDef;
			}
		}

		public ChartLegendColumnHeaderInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new ChartLegendColumnHeaderInstance(this);
				}
				return this.m_instance;
			}
		}

		internal ChartLegendColumnHeader(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumnHeader chartLegendColumnHeaderDef, Chart chart)
		{
			this.m_chartLegendColumnHeaderDef = chartLegendColumnHeaderDef;
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
