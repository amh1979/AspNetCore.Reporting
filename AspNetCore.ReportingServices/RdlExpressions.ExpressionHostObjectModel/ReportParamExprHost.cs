namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ReportParamExprHost : IndexedExprHost
	{
		public IndexedExprHost ValidValuesHost;

		public IndexedExprHost ValidValueLabelsHost;

		public virtual object PromptExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ValidationExpressionExpr
		{
			get
			{
				return null;
			}
		}
	}
}
