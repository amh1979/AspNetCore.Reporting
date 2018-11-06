using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ActionInfoExprHost : StyleExprHost
	{
		[CLSCompliant(false)]
		protected IList<ActionExprHost> m_actionItemHostsRemotable;

		internal IList<ActionExprHost> ActionItemHostsRemotable
		{
			get
			{
				return this.m_actionItemHostsRemotable;
			}
		}
	}
}
