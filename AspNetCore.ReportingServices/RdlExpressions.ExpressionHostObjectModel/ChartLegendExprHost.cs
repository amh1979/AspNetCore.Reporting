using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartLegendExprHost : StyleExprHost
	{
		[CLSCompliant(false)]
		protected IList<ChartLegendCustomItemExprHost> m_legendCustomItemsHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ChartLegendColumnExprHost> m_legendColumnsHostsRemotable;

		public ChartLegendTitleExprHost TitleExprHost;

		public ChartElementPositionExprHost ChartElementPositionHost;

		internal IList<ChartLegendCustomItemExprHost> ChartLegendCustomItemsHostsRemotable
		{
			get
			{
				return this.m_legendCustomItemsHostsRemotable;
			}
		}

		internal IList<ChartLegendColumnExprHost> ChartLegendColumnsHostsRemotable
		{
			get
			{
				return this.m_legendColumnsHostsRemotable;
			}
		}

		public virtual object ChartLegendPositionExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object LayoutExpr
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

		public virtual object DockOutsideChartAreaExpr
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

		public virtual object HeaderSeparatorExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object HeaderSeparatorColorExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ColumnSeparatorExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ColumnSeparatorColorExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ColumnSpacingExpr
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

		public virtual object ReversedExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object MaxAutoSizeExpr
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
