namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class ReportParamExprHost : IndexedExprHost
	{
		public IndexedExprHost ValidValuesHost;

		public IndexedExprHost ValidValueLabelsHost;

		public virtual object ValidationExpressionExpr
		{
			get
			{
				return null;
			}
		}
	}
}
