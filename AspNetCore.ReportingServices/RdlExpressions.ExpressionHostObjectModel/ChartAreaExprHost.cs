using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartAreaExprHost : StyleExprHost
	{
		[CLSCompliant(false)]
		protected IList<ChartAxisExprHost> m_categoryAxesHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ChartAxisExprHost> m_valueAxesHostsRemotable;

		public Chart3DPropertiesExprHost Chart3DPropertiesHost;

		public ChartElementPositionExprHost ChartElementPositionHost;

		public ChartElementPositionExprHost ChartInnerPlotPositionHost;

		internal IList<ChartAxisExprHost> CategoryAxesHostsRemotable
		{
			get
			{
				return this.m_categoryAxesHostsRemotable;
			}
		}

		internal IList<ChartAxisExprHost> ValueAxesHostsRemotable
		{
			get
			{
				return this.m_valueAxesHostsRemotable;
			}
		}

		public virtual object HiddenExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object AlignOrientationExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object EquallySizedAxesFontExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object CursorExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object AxesViewExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ChartAlignTypePositionExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object InnerPlotPositionExpr
		{
			get
			{
				return null;
			}
		}
	}
}
