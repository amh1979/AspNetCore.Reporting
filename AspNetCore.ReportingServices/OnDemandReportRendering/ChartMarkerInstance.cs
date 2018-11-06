namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartMarkerInstance : BaseInstance
	{
		private ChartMarker m_markerDef;

		private StyleInstance m_style;

		private ReportSize m_size;

		private ChartMarkerTypes? m_type;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_markerDef, this.m_markerDef.ReportScope, this.m_markerDef.ChartDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public ReportSize Size
		{
			get
			{
				if (this.m_size == null && !this.m_markerDef.ChartDef.IsOldSnapshot)
				{
					this.m_size = new ReportSize(this.m_markerDef.MarkerDef.EvaluateChartMarkerSize(this.ReportScopeInstance, this.m_markerDef.ChartDef.RenderingContext.OdpContext));
				}
				return this.m_size;
			}
		}

		public ChartMarkerTypes Type
		{
			get
			{
				if (!this.m_type.HasValue && !this.m_markerDef.ChartDef.IsOldSnapshot)
				{
					this.m_type = this.m_markerDef.MarkerDef.EvaluateChartMarkerType(this.ReportScopeInstance, this.m_markerDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_type.Value;
			}
		}

		internal ChartMarkerInstance(ChartMarker markerDef)
			: base(markerDef.ReportScope)
		{
			this.m_markerDef = markerDef;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_type = null;
			this.m_size = null;
		}
	}
}
