namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeCellInstance : BaseInstance, IReportScopeInstance
	{
		private GaugeCell m_gaugeCellDef;

		private bool m_isNewContext = true;

		string IReportScopeInstance.UniqueName
		{
			get
			{
				return this.m_gaugeCellDef.GaugeCellDef.UniqueName;
			}
		}

		bool IReportScopeInstance.IsNewContext
		{
			get
			{
				return this.m_isNewContext;
			}
			set
			{
				this.m_isNewContext = value;
			}
		}

		IReportScope IReportScopeInstance.ReportScope
		{
			get
			{
				return base.m_reportScope;
			}
		}

		internal GaugeCellInstance(GaugeCell gaugeCellDef)
			: base(gaugeCellDef)
		{
			this.m_gaugeCellDef = gaugeCellDef;
		}

		protected override void ResetInstanceCache()
		{
		}

		internal override void SetNewContext()
		{
			if (!this.m_isNewContext)
			{
				this.m_isNewContext = true;
				base.SetNewContext();
			}
		}
	}
}
