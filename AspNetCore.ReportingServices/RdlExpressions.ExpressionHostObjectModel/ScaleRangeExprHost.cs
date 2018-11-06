namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ScaleRangeExprHost : StyleExprHost
	{
		public GaugeInputValueExprHost StartValueHost;

		public GaugeInputValueExprHost EndValueHost;

		public ActionInfoExprHost ActionInfoHost;

		public virtual object DistanceFromScaleExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object StartWidthExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object EndWidthExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object InRangeBarPointerColorExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object InRangeLabelColorExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object InRangeTickMarksColorExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object PlacementExpr
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
