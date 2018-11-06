namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartBorderSkinInstance : BaseInstance
	{
		private ChartBorderSkin m_chartBorderSkinDef;

		private StyleInstance m_style;

		private ChartBorderSkinType? m_borderSkinType;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_chartBorderSkinDef, this.m_chartBorderSkinDef.ChartDef, this.m_chartBorderSkinDef.ChartDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public ChartBorderSkinType BorderSkinType
		{
			get
			{
				if (!this.m_borderSkinType.HasValue && !this.m_chartBorderSkinDef.ChartDef.IsOldSnapshot)
				{
					this.m_borderSkinType = this.m_chartBorderSkinDef.ChartBorderSkinDef.EvaluateBorderSkinType(this.ReportScopeInstance, this.m_chartBorderSkinDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_borderSkinType.Value;
			}
		}

		internal ChartBorderSkinInstance(ChartBorderSkin chartBorderSkinDef)
			: base(chartBorderSkinDef.ChartDef)
		{
			this.m_chartBorderSkinDef = chartBorderSkinDef;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_borderSkinType = null;
		}
	}
}
