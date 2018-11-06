namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendCustomItemInstance : BaseInstance
	{
		private ChartLegendCustomItem m_chartLegendCustomItemDef;

		private StyleInstance m_style;

		private ChartSeparators? m_separator;

		private ReportColor m_separatorColor;

		private string m_toolTip;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_chartLegendCustomItemDef, this.m_chartLegendCustomItemDef.ChartDef, this.m_chartLegendCustomItemDef.ChartDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public ChartSeparators Separator
		{
			get
			{
				if (!this.m_separator.HasValue && !this.m_chartLegendCustomItemDef.ChartDef.IsOldSnapshot)
				{
					this.m_separator = this.m_chartLegendCustomItemDef.ChartLegendCustomItemDef.EvaluateSeparator(this.ReportScopeInstance, this.m_chartLegendCustomItemDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_separator.Value;
			}
		}

		public ReportColor SeparatorColor
		{
			get
			{
				if (this.m_separatorColor == null && !this.m_chartLegendCustomItemDef.ChartDef.IsOldSnapshot)
				{
					this.m_separatorColor = new ReportColor(this.m_chartLegendCustomItemDef.ChartLegendCustomItemDef.EvaluateSeparatorColor(this.ReportScopeInstance, this.m_chartLegendCustomItemDef.ChartDef.RenderingContext.OdpContext), true);
				}
				return this.m_separatorColor;
			}
		}

		public string ToolTip
		{
			get
			{
				if (this.m_toolTip == null && !this.m_chartLegendCustomItemDef.ChartDef.IsOldSnapshot)
				{
					this.m_toolTip = this.m_chartLegendCustomItemDef.ChartLegendCustomItemDef.EvaluateToolTip(this.ReportScopeInstance, this.m_chartLegendCustomItemDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_toolTip;
			}
		}

		internal ChartLegendCustomItemInstance(ChartLegendCustomItem chartLegendCustomItemDef)
			: base(chartLegendCustomItemDef.ChartDef)
		{
			this.m_chartLegendCustomItemDef = chartLegendCustomItemDef;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_separator = null;
			this.m_separatorColor = null;
			this.m_toolTip = null;
		}
	}
}
