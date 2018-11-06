using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapVectorLayerExprHost : MapLayerExprHost
	{
		[CLSCompliant(false)]
		protected IList<MapBindingFieldPairExprHost> m_mapBindingFieldPairsHostsRemotable;

		public MapSpatialDataExprHost MapSpatialDataHost;

		internal IList<MapBindingFieldPairExprHost> MapBindingFieldPairsHostsRemotable
		{
			get
			{
				return this.m_mapBindingFieldPairsHostsRemotable;
			}
		}
	}
}
