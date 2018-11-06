namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class CustomCodeProxyBase
	{
		private IReportObjectModelProxyForCustomCode m_reportObjectModel;

		protected IReportObjectModelProxyForCustomCode Report
		{
			get
			{
				return this.m_reportObjectModel;
			}
		}

		protected CustomCodeProxyBase(IReportObjectModelProxyForCustomCode reportObjectModel)
		{
			this.m_reportObjectModel = reportObjectModel;
		}

		protected virtual void OnInit()
		{
		}

		internal void CallOnInit()
		{
			this.OnInit();
		}
	}
}
