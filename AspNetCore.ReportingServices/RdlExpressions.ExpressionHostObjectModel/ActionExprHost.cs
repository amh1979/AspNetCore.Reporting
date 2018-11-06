using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ActionExprHost : ReportObjectModelProxy
	{
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
