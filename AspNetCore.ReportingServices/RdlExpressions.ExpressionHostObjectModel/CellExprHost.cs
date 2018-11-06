using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class CellExprHost : ReportObjectModelProxy
	{
		[CLSCompliant(false)]
		protected IList<JoinConditionExprHost> m_joinConditionExprHostsRemotable;

		internal IList<JoinConditionExprHost> JoinConditionExprHostsRemotable
		{
			get
			{
				return this.m_joinConditionExprHostsRemotable;
			}
		}
	}
}
