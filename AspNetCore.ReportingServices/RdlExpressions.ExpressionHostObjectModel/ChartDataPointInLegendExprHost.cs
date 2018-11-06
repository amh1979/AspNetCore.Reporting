namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartDataPointInLegendExprHost : StyleExprHost
	{
		public ActionInfoExprHost ActionInfoHost;

		public virtual object LegendTextExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ToolTipExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object HiddenExpr
		{
			get
			{
				return null;
			}
		}
	}
}
