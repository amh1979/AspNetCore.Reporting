using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartEmptyPointsExprHost : StyleExprHost
	{
		public ChartMarkerExprHost ChartMarkerHost;

		public ChartDataLabelExprHost DataLabelHost;

		public ChartDataPointInLegendExprHost DataPointInLegendHost;

		public ActionInfoExprHost ActionInfoHost;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

		public virtual object AxisLabelExpr
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

		public virtual object ToolTipExpr
		{
			get
			{
				return null;
			}
		}
	}
}
