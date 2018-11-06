using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ParagraphExprHost : StyleExprHost
	{
		[CLSCompliant(false)]
		protected IList<TextRunExprHost> m_textRunHostsRemotable;

		internal IList<TextRunExprHost> TextRunHostsRemotable
		{
			get
			{
				return this.m_textRunHostsRemotable;
			}
		}

		public virtual object LeftIndentExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object RightIndentExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object HangingIndentExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object SpaceBeforeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object SpaceAfterExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ListStyleExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ListLevelExpr
		{
			get
			{
				return null;
			}
		}
	}
}
