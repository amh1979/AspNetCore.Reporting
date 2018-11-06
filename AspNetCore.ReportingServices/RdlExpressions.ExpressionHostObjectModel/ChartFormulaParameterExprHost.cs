namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartFormulaParameterExprHost : StyleExprHost
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
