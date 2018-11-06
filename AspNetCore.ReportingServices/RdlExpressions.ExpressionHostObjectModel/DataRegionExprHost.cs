using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class DataRegionExprHost<TMemberType, TCellType> : ReportItemExprHost where TMemberType : MemberNodeExprHost<TMemberType> where TCellType : CellExprHost
	{
		[CLSCompliant(false)]
		protected IList<FilterExprHost> m_filterHostsRemotable;

		protected SortExprHost m_sortHost;

		[CLSCompliant(false)]
        internal IList<IMemberNode> m_memberTreeHostsRemotable;

		[CLSCompliant(false)]
		protected IList<TCellType> m_cellHostsRemotable;

		public IndexedExprHost UserSortExpressionsHost;

		[CLSCompliant(false)]
		protected IList<JoinConditionExprHost> m_joinConditionExprHostsRemotable;

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
				return this.m_filterHostsRemotable;
			}
		}

		internal SortExprHost SortHost
		{
			get
			{
				return this.m_sortHost;
			}
		}

		internal IList<IMemberNode> MemberTreeHostsRemotable
		{
			get
			{
				return this.m_memberTreeHostsRemotable;
			}
		}

		internal IList<TCellType> CellHostsRemotable
		{
			get
			{
				return this.m_cellHostsRemotable;
			}
		}

		internal IList<JoinConditionExprHost> JoinConditionExprHostsRemotable
		{
			get
			{
				return this.m_joinConditionExprHostsRemotable;
			}
		}
	}
}
