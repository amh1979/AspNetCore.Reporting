using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapBindingFieldPairCollection : MapObjectCollectionBase<MapBindingFieldPair>
	{
		private Map m_map;

		private MapVectorLayer m_mapVectorLayer;

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair> m_mapBindingFieldCollectionDef;

		public override int Count
		{
			get
			{
				return this.m_mapBindingFieldCollectionDef.Count;
			}
		}

		internal MapBindingFieldPairCollection(MapVectorLayer mapVectorLayer, Map map)
		{
			this.m_mapBindingFieldCollectionDef = mapVectorLayer.MapVectorLayerDef.MapBindingFieldPairs;
			this.m_mapVectorLayer = mapVectorLayer;
			this.m_map = map;
		}

		internal MapBindingFieldPairCollection(List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair> mapBindingFieldCollectionDef, Map map)
		{
			this.m_mapBindingFieldCollectionDef = mapBindingFieldCollectionDef;
			this.m_mapVectorLayer = null;
			this.m_map = map;
		}

		protected override MapBindingFieldPair CreateMapObject(int index)
		{
			return new MapBindingFieldPair(this.m_mapBindingFieldCollectionDef[index], this.m_mapVectorLayer, this.m_map);
		}
	}
}
