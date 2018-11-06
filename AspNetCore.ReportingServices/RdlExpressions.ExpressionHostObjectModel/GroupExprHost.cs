using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class GroupExprHost : IndexedExprHost
	{
		public IndexedExprHost ParentExpressionsHost;

		public IndexedExprHost ReGroupExpressionsHost;

		public IndexedExprHost VariableValueHosts;

		[CLSCompliant(false)]
		protected IList<FilterExprHost> m_filterHostsRemotable;

		public IndexedExprHost UserSortExpressionsHost;

		public PageBreakExprHost PageBreakExprHost;

		public virtual object LabelExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object PageNameExpr
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
				return this.m_filterHostsRemotable;
			}
		}
	}
}
