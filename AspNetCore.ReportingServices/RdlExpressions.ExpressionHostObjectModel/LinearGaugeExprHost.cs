using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class LinearGaugeExprHost : GaugeExprHost
	{
		[CLSCompliant(false)]
		protected IList<LinearScaleExprHost> m_linearScalesHostsRemotable;

		internal IList<LinearScaleExprHost> LinearScalesHostsRemotable
		{
			get
			{
				return this.m_linearScalesHostsRemotable;
			}
		}

		public virtual object OrientationExpr
		{
			get
			{
				return null;
			}
		}
	}
}
