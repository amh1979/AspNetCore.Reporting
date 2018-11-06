using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MemberNodeExprHost<TMemberType> : ReportObjectModelProxy, IMemberNode where TMemberType : IMemberNode
	{
		protected GroupExprHost m_groupHost;

		protected SortExprHost m_sortHost;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

		[CLSCompliant(false)]
        internal IList<IMemberNode> m_memberTreeHostsRemotable;

		[CLSCompliant(false)]
		protected IList<JoinConditionExprHost> m_joinConditionExprHostsRemotable;

		GroupExprHost IMemberNode.GroupHost
		{
			get
			{
				return this.m_groupHost;
			}
		}

		SortExprHost IMemberNode.SortHost
		{
			get
			{
				return this.m_sortHost;
			}
		}

		IList<DataValueExprHost> IMemberNode.CustomPropertyHostsRemotable
		{
			get
			{
				return this.m_customPropertyHostsRemotable;
			}
		}

		IList<IMemberNode> IMemberNode.MemberTreeHostsRemotable
		{
			get
			{
				return this.m_memberTreeHostsRemotable;
			}
		}

		IList<JoinConditionExprHost> IMemberNode.JoinConditionExprHostsRemotable
		{
			get
			{
				return this.m_joinConditionExprHostsRemotable;
			}
		}
	}
}
