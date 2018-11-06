using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class DataCellExprHost : ReportObjectModelProxy
	{
		protected DataValueExprHost[] DataValueHosts;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_dataValueHostsRemotable;

		internal IList<DataValueExprHost> DataValueHostsRemotable
		{
			get
			{
				if (this.m_dataValueHostsRemotable == null && this.DataValueHosts != null)
				{
					this.m_dataValueHostsRemotable = new RemoteArrayWrapper<DataValueExprHost>(this.DataValueHosts);
				}
				return this.m_dataValueHostsRemotable;
			}
		}
	}
}
