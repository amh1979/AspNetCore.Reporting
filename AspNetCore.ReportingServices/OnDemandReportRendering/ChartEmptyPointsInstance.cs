namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartEmptyPointsInstance : BaseInstance
	{
		private ChartEmptyPoints m_chartEmptyPointsDef;

		private StyleInstance m_style;

		private object m_axisLabel;

		private string m_toolTip;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_chartEmptyPointsDef, this.m_chartEmptyPointsDef.ChartDef, this.m_chartEmptyPointsDef.ChartDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public object AxisLabel
		{
			get
			{
				if (this.m_axisLabel == null && !this.m_chartEmptyPointsDef.ChartDef.IsOldSnapshot)
				{
					this.m_axisLabel = this.m_chartEmptyPointsDef.ChartEmptyPointsDef.EvaluateAxisLabel(this.ReportScopeInstance, this.m_chartEmptyPointsDef.ChartDef.RenderingContext.OdpContext).Value;
				}
				return this.m_axisLabel;
			}
		}

		public string ToolTip
		{
			get
			{
				if (this.m_toolTip == null)
				{
					this.m_toolTip = this.m_chartEmptyPointsDef.ChartEmptyPointsDef.EvaluateToolTip(this.ReportScopeInstance, this.m_chartEmptyPointsDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_toolTip;
			}
		}

		internal ChartEmptyPointsInstance(ChartEmptyPoints chartEmptyPointsDef)
			: base(chartEmptyPointsDef.ReportScope)
		{
			this.m_chartEmptyPointsDef = chartEmptyPointsDef;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_axisLabel = null;
			this.m_toolTip = null;
		}
	}
}
