using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartBorderSkin : IROMStyleDefinitionContainer
	{
		private Chart m_chart;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartBorderSkin m_chartBorderSkinDef;

		private ChartBorderSkinInstance m_instance;

		private Style m_style;

		private ReportEnumProperty<ChartBorderSkinType> m_borderSkinType;

		public Style Style
		{
			get
			{
				if (this.m_style == null && !this.m_chart.IsOldSnapshot && this.m_chartBorderSkinDef.StyleClass != null)
				{
					this.m_style = new Style(this.m_chart, this.m_chart, this.m_chartBorderSkinDef, this.m_chart.RenderingContext);
				}
				return this.m_style;
			}
		}

		public ReportEnumProperty<ChartBorderSkinType> BorderSkinType
		{
			get
			{
				if (this.m_borderSkinType == null && !this.m_chart.IsOldSnapshot && this.m_chartBorderSkinDef.BorderSkinType != null)
				{
					this.m_borderSkinType = new ReportEnumProperty<ChartBorderSkinType>(this.m_chartBorderSkinDef.BorderSkinType.IsExpression, this.m_chartBorderSkinDef.BorderSkinType.OriginalText, EnumTranslator.TranslateChartBorderSkinType(this.m_chartBorderSkinDef.BorderSkinType.StringValue, null));
				}
				return this.m_borderSkinType;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartBorderSkin ChartBorderSkinDef
		{
			get
			{
				return this.m_chartBorderSkinDef;
			}
		}

		public ChartBorderSkinInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new ChartBorderSkinInstance(this);
				}
				return this.m_instance;
			}
		}

		internal ChartBorderSkin(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartBorderSkin chartBorderSkinDef, Chart chart)
		{
			this.m_chartBorderSkinDef = chartBorderSkinDef;
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
