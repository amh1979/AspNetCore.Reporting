namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class GaugePointerExprHost : StyleExprHost
	{
		public GaugeInputValueExprHost GaugeInputValueHost;

		public PointerImageExprHost PointerImageHost;

		public ActionInfoExprHost ActionInfoHost;

		public virtual object BarStartExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DistanceFromScaleExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object MarkerLengthExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object MarkerStyleExpr
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

		public virtual object SnappingEnabledExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object SnappingIntervalExpr
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

		public virtual object WidthExpr
		{
			get
			{
				return null;
			}
		}
	}
}
