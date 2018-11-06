using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapElementViewExprHost : MapViewExprHost
	{
		[CLSCompliant(false)]
		protected IList<MapBindingFieldPairExprHost> m_mapBindingFieldPairsHostsRemotable;

		public virtual object LayerNameExpr
		{
			get
			{
				return null;
			}
		}

		internal IList<MapBindingFieldPairExprHost> MapBindingFieldPairsHostsRemotable
		{
			get
			{
				return this.m_mapBindingFieldPairsHostsRemotable;
			}
		}
	}
}
