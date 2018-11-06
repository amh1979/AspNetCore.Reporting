namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class BaseInstance
	{
		internal IReportScope m_reportScope;

		internal virtual IReportScopeInstance ReportScopeInstance
		{
			get
			{
				return this.m_reportScope.ReportScopeInstance;
			}
		}

		internal BaseInstance(IReportScope reportScope)
		{
			this.m_reportScope = reportScope;
		}

		internal virtual void SetNewContext()
		{
			this.ResetInstanceCache();
		}

		protected abstract void ResetInstanceCache();
	}
}
