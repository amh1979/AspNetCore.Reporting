using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapLineLayerExprHost : MapVectorLayerExprHost
	{
		public MapLineTemplateExprHost MapLineTemplateHost;

		public MapLineRulesExprHost MapLineRulesHost;

		[CLSCompliant(false)]
		protected IList<MapLineExprHost> m_mapLinesHostsRemotable;

		internal IList<MapLineExprHost> MapLinesHostsRemotable
		{
			get
			{
				return this.m_mapLinesHostsRemotable;
			}
		}
	}
}
