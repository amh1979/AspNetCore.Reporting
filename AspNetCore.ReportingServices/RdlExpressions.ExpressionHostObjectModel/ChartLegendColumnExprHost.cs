namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartLegendColumnExprHost : StyleExprHost
	{
		public ChartLegendColumnHeaderExprHost HeaderHost;

		public ActionInfoExprHost ActionInfoHost;

		public virtual object ColumnTypeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ValueExpr
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

		public virtual object MinimumWidthExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object MaximumWidthExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object SeriesSymbolWidthExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object SeriesSymbolHeightExpr
		{
			get
			{
				return null;
			}
		}
	}
}
