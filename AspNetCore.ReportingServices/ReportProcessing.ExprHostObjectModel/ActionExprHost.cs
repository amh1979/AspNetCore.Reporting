using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class ActionExprHost : ReportObjectModelProxy
	{
		protected ParamExprHost[] DrillThroughParameterHosts;

		[CLSCompliant(false)]
		protected IList<ParamExprHost> m_drillThroughParameterHostsRemotable;

		public virtual object HyperlinkExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DrillThroughReportNameExpr
		{
			get
			{
				return null;
			}
		}

		internal IList<ParamExprHost> DrillThroughParameterHostsRemotable
		{
			get
			{
				if (this.m_drillThroughParameterHostsRemotable == null && this.DrillThroughParameterHosts != null)
				{
					this.m_drillThroughParameterHostsRemotable = new RemoteArrayWrapper<ParamExprHost>(this.DrillThroughParameterHosts);
				}
				return this.m_drillThroughParameterHostsRemotable;
			}
		}

		public virtual object DrillThroughBookmarkLinkExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object BookmarkLinkExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object LabelExpr
		{
			get
			{
				return null;
			}
		}
	}
}
