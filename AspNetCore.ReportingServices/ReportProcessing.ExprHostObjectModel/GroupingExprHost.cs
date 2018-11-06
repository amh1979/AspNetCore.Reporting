using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class GroupingExprHost : IndexedExprHost
	{
		protected FilterExprHost[] FilterHosts;

		[CLSCompliant(false)]
		protected IList<FilterExprHost> m_filterHostsRemotable;

		public IndexedExprHost ParentExpressionsHost;

		protected DataValueExprHost[] CustomPropertyHosts;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

		public IndexedExprHost UserSortExpressionsHost;

		public virtual object LabelExpr
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
	}
}
