using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapCustomColorRuleExprHost : MapColorRuleExprHost
	{
		[CLSCompliant(false)]
		protected IList<MapCustomColorExprHost> m_mapCustomColorsHostsRemotable;

		internal IList<MapCustomColorExprHost> MapCustomColorsHostsRemotable
		{
			get
			{
				return this.m_mapCustomColorsHostsRemotable;
			}
		}
	}
}
