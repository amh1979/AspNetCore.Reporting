namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapSubItemExprHost : StyleExprHost
	{
		public MapLocationExprHost MapLocationHost;

		public MapSizeExprHost MapSizeHost;

		public virtual object LeftMarginExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object RightMarginExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object TopMarginExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object BottomMarginExpr
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
	}
}
