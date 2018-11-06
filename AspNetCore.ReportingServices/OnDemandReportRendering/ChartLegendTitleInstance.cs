namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendTitleInstance : BaseInstance
	{
		private ChartLegendTitle m_chartLegendTitleDef;

		private StyleInstance m_style;

		private string m_caption;

		private ChartSeparators? m_titleSeparator;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_chartLegendTitleDef, this.m_chartLegendTitleDef.ChartDef, this.m_chartLegendTitleDef.ChartDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public string Caption
		{
			get
			{
				if (this.m_caption == null && !this.m_chartLegendTitleDef.ChartDef.IsOldSnapshot)
				{
					this.m_caption = this.m_chartLegendTitleDef.ChartLegendTitleDef.EvaluateCaption(this.ReportScopeInstance, this.m_chartLegendTitleDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_caption;
			}
		}

		public ChartSeparators TitleSeparator
		{
			get
			{
				if (!this.m_titleSeparator.HasValue && !this.m_chartLegendTitleDef.ChartDef.IsOldSnapshot)
				{
					this.m_titleSeparator = this.m_chartLegendTitleDef.ChartLegendTitleDef.EvaluateTitleSeparator(this.ReportScopeInstance, this.m_chartLegendTitleDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_titleSeparator.Value;
			}
		}

		internal ChartLegendTitleInstance(ChartLegendTitle chartLegendTitleDef)
			: base(chartLegendTitleDef.ChartDef)
		{
			this.m_chartLegendTitleDef = chartLegendTitleDef;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_caption = null;
			this.m_titleSeparator = null;
		}
	}
}
