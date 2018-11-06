using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapPointLayerExprHost : MapVectorLayerExprHost
	{
		public MapPointTemplateExprHost MapPointTemplateHost;

		public MapPointRulesExprHost MapPointRulesHost;

		[CLSCompliant(false)]
		protected IList<MapPointExprHost> m_mapPointsHostsRemotable;

		internal IList<MapPointExprHost> MapPointsHostsRemotable
		{
			get
			{
				return this.m_mapPointsHostsRemotable;
			}
		}
	}
}
