namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class FilterExprHost : IndexedExprHost
	{
		public virtual object FilterExpressionExpr
		{
			get
			{
				return null;
			}
		}
	}
}
