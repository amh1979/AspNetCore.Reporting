using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class DataGroupingExprHost : ReportObjectModelProxy
	{
		public GroupingExprHost GroupingHost;

		public SortingExprHost SortingHost;

		protected DataValueExprHost[] CustomPropertyHosts;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

		protected DataGroupingExprHost[] DataGroupingHosts;

		[CLSCompliant(false)]
		protected IList<DataGroupingExprHost> m_dataGroupingHostsRemotable;

		internal IList<DataValueExprHost> CustomPropertyHostsRemotable
		{
			get
			{
				if (this.m_customPropertyHostsRemotable == null && this.CustomPropertyHosts != null)
				{
					this.m_customPropertyHostsRemotable = new RemoteArrayWrapper<DataValueExprHost>(this.CustomPropertyHosts);
				}
				return this.m_customPropertyHostsRemotable;
			}
		}

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
	}
}
