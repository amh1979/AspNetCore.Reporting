using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class TablixExprHost : DataRegionExprHost<TablixMemberExprHost, TablixCellExprHost>
	{
		[CLSCompliant(false)]
		protected IList<TablixCellExprHost> m_cornerCellHostsRemotable;

		public virtual object TopMarginExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object BottomMarginExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object LeftMarginExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object RightMarginExpr
		{
			get
			{
				return null;
			}
		}

		internal IList<TablixCellExprHost> CornerCellHostsRemotable
		{
			get
			{
				return this.m_cornerCellHostsRemotable;
			}
		}
	}
}
