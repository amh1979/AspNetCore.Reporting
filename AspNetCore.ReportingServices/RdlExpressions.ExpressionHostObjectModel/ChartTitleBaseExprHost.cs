namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartTitleBaseExprHost : StyleExprHost
	{
		public virtual object CaptionExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ChartTitlePositionExpr
		{
			get
			{
				return null;
			}
		}
	}
}
