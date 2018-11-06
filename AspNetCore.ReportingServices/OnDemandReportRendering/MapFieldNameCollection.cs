using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapFieldNameCollection : MapObjectCollectionBase<MapFieldName>
	{
		private Map m_map;

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapFieldName> m_mapFieldNames;

		public override int Count
		{
			get
			{
				return this.m_mapFieldNames.Count;
			}
		}

		internal MapFieldNameCollection(List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapFieldName> mapFieldNames, Map map)
		{
			this.m_mapFieldNames = mapFieldNames;
			this.m_map = map;
		}

		protected override MapFieldName CreateMapObject(int index)
		{
			return new MapFieldName(this.m_mapFieldNames[index], this.m_map);
		}
	}
}
