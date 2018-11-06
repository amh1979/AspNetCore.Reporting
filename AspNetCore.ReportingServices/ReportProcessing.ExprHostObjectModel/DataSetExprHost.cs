using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class DataSetExprHost : ReportObjectModelProxy
	{
		protected CalcFieldExprHost[] FieldHosts;

		[CLSCompliant(false)]
		protected IList<CalcFieldExprHost> m_fieldHostsRemotable;

		public IndexedExprHost QueryParametersHost;

		protected FilterExprHost[] FilterHosts;

		[CLSCompliant(false)]
		protected IList<FilterExprHost> m_filterHostsRemotable;

		public IndexedExprHost UserSortExpressionsHost;

		internal IList<CalcFieldExprHost> FieldHostsRemotable
		{
			get
			{
				if (this.m_fieldHostsRemotable == null && this.FieldHosts != null)
				{
					this.m_fieldHostsRemotable = new RemoteArrayWrapper<CalcFieldExprHost>(this.FieldHosts);
				}
				return this.m_fieldHostsRemotable;
			}
		}

		public virtual object QueryCommandTextExpr
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
