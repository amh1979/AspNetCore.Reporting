using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapAppearanceRuleExprHost : ReportObjectModelProxy
	{
		[CLSCompliant(false)]
		protected IList<MapBucketExprHost> m_mapBucketsHostsRemotable;

		public virtual object DataValueExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DistributionTypeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object BucketCountExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object StartValueExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object EndValueExpr
		{
			get
			{
				return null;
			}
		}

		internal IList<MapBucketExprHost> MapBucketsHostsRemotable
		{
			get
			{
				return this.m_mapBucketsHostsRemotable;
			}
		}

		public virtual object LegendTextExpr
		{
			get
			{
				return null;
			}
		}
	}
}
