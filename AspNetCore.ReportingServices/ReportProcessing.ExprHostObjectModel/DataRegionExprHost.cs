using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class DataRegionExprHost : ReportItemExprHost
	{
		protected FilterExprHost[] FilterHosts;

		[CLSCompliant(false)]
		protected IList<FilterExprHost> m_filterHostsRemotable;

		public IndexedExprHost UserSortExpressionsHost;

		public virtual object NoRowsExpr
		{
			get
			{
				return null;
			}
		}

		internal IList<FilterExprHost> FilterHostsRemotable
		{
			get
			{
				if (this.m_filterHostsRemotable == null && this.FilterHosts != null)
				{
					this.m_filterHostsRemotable = new RemoteArrayWrapper<FilterExprHost>(this.FilterHosts);
				}
				return this.m_filterHostsRemotable;
			}
		}
	}
}
