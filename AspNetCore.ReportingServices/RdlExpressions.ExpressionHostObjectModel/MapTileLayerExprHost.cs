using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapTileLayerExprHost : MapLayerExprHost
	{
		[CLSCompliant(false)]
		protected IList<MapTileExprHost> m_mapTilesHostsRemotable;

		public virtual object ServiceUrlExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object TileStyleExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object UseSecureConnectionExpr
		{
			get
			{
				return null;
			}
		}

		internal IList<MapTileExprHost> MapTilesHostsRemotable
		{
			get
			{
				return this.m_mapTilesHostsRemotable;
			}
		}
	}
}
