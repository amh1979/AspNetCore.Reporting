using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class CustomReportItemExprHost : DataRegionExprHost
	{
		protected DataGroupingExprHost[] DataGroupingHosts;

		[CLSCompliant(false)]
		protected IList<DataGroupingExprHost> m_dataGroupingHostsRemotable;

		protected DataCellExprHost[] DataCellHosts;

		[CLSCompliant(false)]
		protected IList<DataCellExprHost> m_dataCellHostsRemotable;

		internal IList<DataGroupingExprHost> DataGroupingHostsRemotable
		{
			get
			{
				if (this.m_dataGroupingHostsRemotable == null && this.DataGroupingHosts != null)
				{
					this.m_dataGroupingHostsRemotable = new RemoteArrayWrapper<DataGroupingExprHost>(this.DataGroupingHosts);
				}
				return this.m_dataGroupingHostsRemotable;
			}
		}

		internal IList<DataCellExprHost> DataCellHostsRemotable
		{
			get
			{
				if (this.m_dataCellHostsRemotable == null && this.DataCellHosts != null)
				{
					this.m_dataCellHostsRemotable = new RemoteArrayWrapper<DataCellExprHost>(this.DataCellHosts);
				}
				return this.m_dataCellHostsRemotable;
			}
		}
	}
}
