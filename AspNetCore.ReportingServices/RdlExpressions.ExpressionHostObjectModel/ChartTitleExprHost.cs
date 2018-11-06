namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartTitleExprHost : ChartTitleBaseExprHost
	{
		public ActionInfoExprHost ActionInfoHost;

		public ChartElementPositionExprHost ChartElementPositionHost;

		public virtual object HiddenExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DockingExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DockingOffsetExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DockOutsideChartAreaExpr
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

		public virtual object TextOrientationExpr
		{
			get
			{
				return null;
			}
		}
	}
}
