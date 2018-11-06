namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class AggregateParamExprHost : ReportObjectModelProxy
	{
		public virtual object ValueExpr
		{
			get
			{
				return null;
			}
		}
	}
}
