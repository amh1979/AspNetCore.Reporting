using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapSpatialDataSetExprHost : MapSpatialDataExprHost
	{
		[CLSCompliant(false)]
		protected IList<MapFieldNameExprHost> m_mapFieldNamesHostsRemotable;

		public virtual object DataSetNameExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object SpatialFieldExpr
		{
			get
			{
				return null;
			}
		}

		internal IList<MapFieldNameExprHost> MapFieldNamesHostsRemotable
		{
			get
			{
				return this.m_mapFieldNamesHostsRemotable;
			}
		}
	}
}
