namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapSpatialElementTemplateExprHost : StyleExprHost
	{
		public ActionInfoExprHost ActionInfoHost;

		public virtual object HiddenExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object OffsetXExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object OffsetYExpr
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

		public virtual object ToolTipExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DataElementLabelExpr
		{
			get
			{
				return null;
			}
		}
	}
}
