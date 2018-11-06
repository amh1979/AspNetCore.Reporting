using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class RadialScaleExprHost : GaugeScaleExprHost
	{
		[CLSCompliant(false)]
		protected IList<RadialPointerExprHost> m_radialPointersHostsRemotable;

		internal IList<RadialPointerExprHost> RadialPointersHostsRemotable
		{
			get
			{
				return this.m_radialPointersHostsRemotable;
			}
		}

		public virtual object RadiusExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object StartAngleExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object SweepAngleExpr
		{
			get
			{
				return null;
			}
		}
	}
}
