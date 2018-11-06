using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class RadialGaugeExprHost : GaugeExprHost
	{
		[CLSCompliant(false)]
		protected IList<RadialScaleExprHost> m_radialScalesHostsRemotable;

		internal IList<RadialScaleExprHost> RadialScalesHostsRemotable
		{
			get
			{
				return this.m_radialScalesHostsRemotable;
			}
		}

		public virtual object PivotXExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object PivotYExpr
		{
			get
			{
				return null;
			}
		}
	}
}
