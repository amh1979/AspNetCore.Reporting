namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapLegendExprHost : MapDockableSubItemExprHost
	{
		public MapLegendTitleExprHost MapLegendTitleHost;

		public virtual object LayoutExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object AutoFitTextDisabledExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object MinFontSizeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object InterlacedRowsExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object InterlacedRowsColorExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object EquallySpacedItemsExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object TextWrapThresholdExpr
		{
			get
			{
				return null;
			}
		}
	}
}
