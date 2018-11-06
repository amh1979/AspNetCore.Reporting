using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartLegendCustomItemExprHost : StyleExprHost
	{
		public ChartMarkerExprHost ChartMarkerHost;

		public ActionInfoExprHost ActionInfoHost;

		[CLSCompliant(false)]
		protected IList<ChartLegendCustomItemCellExprHost> m_legendCustomItemCellsHostsRemotable;

		public virtual object SeparatorExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object SeparatorColorExpr
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

		internal IList<ChartLegendCustomItemCellExprHost> ChartLegendCustomItemCellsHostsRemotable
		{
			get
			{
				return this.m_legendCustomItemCellsHostsRemotable;
			}
		}
	}
}
