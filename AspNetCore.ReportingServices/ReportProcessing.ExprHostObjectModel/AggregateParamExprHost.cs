namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
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
