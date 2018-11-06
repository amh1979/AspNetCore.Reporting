namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartDataLabelExprHost : StyleExprHost
	{
		public ActionInfoExprHost ActionInfoHost;

		public virtual object VisibleExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object LabelExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ChartDataLabelPositionExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object RotationExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object UseValueAsLabelExpr
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
