using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class ChartExprHost : DataRegionExprHost
	{
		public MultiChartExprHost MultiChartHost;

		public ChartDynamicGroupExprHost RowGroupingsHost;

		public IndexedExprHost StaticRowLabelsHost;

		public ChartDynamicGroupExprHost ColumnGroupingsHost;

		public IndexedExprHost StaticColumnLabelsHost;

		protected ChartDataPointExprHost[] ChartDataPointHosts;

		[CLSCompliant(false)]
		protected IList<ChartDataPointExprHost> m_chartDataPointHostsRemotable;

		public ChartTitleExprHost TitleHost;

		public AxisExprHost CategoryAxisHost;

		public AxisExprHost ValueAxisHost;

		public StyleExprHost LegendHost;

		public StyleExprHost PlotAreaHost;

		internal IList<ChartDataPointExprHost> ChartDataPointHostsRemotable
		{
			get
			{
				if (this.m_chartDataPointHostsRemotable == null && this.ChartDataPointHosts != null)
				{
					this.m_chartDataPointHostsRemotable = new RemoteArrayWrapper<ChartDataPointExprHost>(this.ChartDataPointHosts);
				}
				return this.m_chartDataPointHostsRemotable;
			}
		}
	}
}
