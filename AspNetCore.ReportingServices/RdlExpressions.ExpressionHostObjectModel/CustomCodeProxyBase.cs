namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class CustomCodeProxyBase
	{
		private IReportObjectModelProxyForCustomCode m_reportObjectModel;

		internal IReportObjectModelProxyForCustomCode Report
		{
			get
			{
				return this.m_reportObjectModel;
			}
		}

        internal CustomCodeProxyBase(IReportObjectModelProxyForCustomCode reportObjectModel)
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
