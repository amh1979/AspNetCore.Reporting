using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class ReportItemExprHost : StyleExprHost, IVisibilityHiddenExprHost
	{
		public ActionExprHost ActionHost;

		public ActionInfoExprHost ActionInfoHost;

		protected DataValueExprHost[] CustomPropertyHosts;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

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

		public virtual object LabelExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object BookmarkExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ToolTipExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object VisibilityHiddenExpr
		{
			get
			{
				return null;
			}
		}
	}
}
