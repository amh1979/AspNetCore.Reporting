namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
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
