namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class GaugePanelItemExprHost : StyleExprHost
	{
		public ActionInfoExprHost ActionInfoHost;

		public virtual object TopExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object LeftExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object HeightExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object WidthExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ZIndexExpr
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
