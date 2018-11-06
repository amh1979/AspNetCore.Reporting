using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class ActionInfoExprHost : StyleExprHost
	{
		protected ActionExprHost[] ActionItemHosts;

		[CLSCompliant(false)]
		protected IList<ActionExprHost> m_actionItemHostsRemotable;

		internal IList<ActionExprHost> ActionItemHostsRemotable
		{
			get
			{
				if (this.m_actionItemHostsRemotable == null && this.ActionItemHosts != null)
				{
					this.m_actionItemHostsRemotable = new RemoteArrayWrapper<ActionExprHost>(this.ActionItemHosts);
				}
				return this.m_actionItemHostsRemotable;
			}
		}
	}
}
