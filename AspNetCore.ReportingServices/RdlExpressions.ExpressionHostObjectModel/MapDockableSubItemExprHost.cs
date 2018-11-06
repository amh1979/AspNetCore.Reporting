namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapDockableSubItemExprHost : MapSubItemExprHost
	{
		public ActionInfoExprHost ActionInfoHost;

		public virtual object MapPositionExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DockOutsideViewportExpr
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

		public virtual object ToolTipExpr
		{
			get
			{
				return null;
			}
		}
	}
}
