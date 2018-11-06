using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapPolygonLayerExprHost : MapVectorLayerExprHost
	{
		public MapPolygonTemplateExprHost MapPolygonTemplateHost;

		public MapPolygonRulesExprHost MapPolygonRulesHost;

		public MapPointTemplateExprHost MapPointTemplateHost;

		public MapPointRulesExprHost MapPointRulesHost;

		[CLSCompliant(false)]
		protected IList<MapPolygonExprHost> m_mapPolygonsHostsRemotable;

		internal IList<MapPolygonExprHost> MapPolygonsHostsRemotable
		{
			get
			{
				return this.m_mapPolygonsHostsRemotable;
			}
		}
	}
}
