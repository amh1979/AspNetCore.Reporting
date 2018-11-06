using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ReportItemExprHost : StyleExprHost, IVisibilityHiddenExprHost
	{
		public ActionInfoExprHost ActionInfoHost;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

		public PageBreakExprHost PageBreakExprHost;

		internal IList<DataValueExprHost> CustomPropertyHostsRemotable
		{
			get
			{
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

		public virtual object PageNameExpr
		{
			get
			{
				return null;
			}
		}
	}
}
