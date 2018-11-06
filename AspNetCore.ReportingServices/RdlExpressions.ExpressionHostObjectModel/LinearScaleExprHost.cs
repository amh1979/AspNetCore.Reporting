using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class LinearScaleExprHost : GaugeScaleExprHost
	{
		[CLSCompliant(false)]
		protected IList<LinearPointerExprHost> m_linearPointersHostsRemotable;

		internal IList<LinearPointerExprHost> LinearPointersHostsRemotable
		{
			get
			{
				return this.m_linearPointersHostsRemotable;
			}
		}

		public virtual object StartMarginExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object EndMarginExpr
		{
			get
			{
				return null;
			}
		}
	}
}
