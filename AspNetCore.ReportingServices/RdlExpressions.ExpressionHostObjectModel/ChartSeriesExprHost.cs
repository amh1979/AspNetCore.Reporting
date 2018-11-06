using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartSeriesExprHost : StyleExprHost
	{
		public ActionInfoExprHost ActionInfoHost;

		public ChartEmptyPointsExprHost EmptyPointsHost;

		public ChartSmartLabelExprHost SmartLabelHost;

		public ChartDataLabelExprHost DataLabelHost;

		public ChartMarkerExprHost ChartMarkerHost;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ChartDerivedSeriesExprHost> m_derivedSeriesCollectionHostsRemotable;

		public ChartDataPointInLegendExprHost DataPointInLegendHost;

		public virtual object TypeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object SubtypeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object LegendNameExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object LegendTextExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ChartAreaNameExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ValueAxisNameExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object CategoryAxisNameExpr
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

		public virtual object HideInLegendExpr
		{
			get
			{
				return null;
			}
		}

		internal IList<DataValueExprHost> CustomPropertyHostsRemotable
		{
			get
			{
				return this.m_customPropertyHostsRemotable;
			}
		}

		internal IList<ChartDerivedSeriesExprHost> ChartDerivedSeriesCollectionHostsRemotable
		{
			get
			{
				return this.m_derivedSeriesCollectionHostsRemotable;
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
