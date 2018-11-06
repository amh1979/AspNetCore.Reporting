using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLayerCollection : MapObjectCollectionBase<MapLayer>
	{
		private Map m_map;

		public override int Count
		{
			get
			{
				return this.m_map.MapDef.MapLayers.Count;
			}
		}

		internal MapLayerCollection(Map map)
		{
			this.m_map = map;
		}

		protected override MapLayer CreateMapObject(int index)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapLayer mapLayer = this.m_map.MapDef.MapLayers[index];
			if (mapLayer is AspNetCore.ReportingServices.ReportIntermediateFormat.MapTileLayer)
			{
				return new MapTileLayer((AspNetCore.ReportingServices.ReportIntermediateFormat.MapTileLayer)mapLayer, this.m_map);
			}
			if (mapLayer is AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonLayer)
			{
				return new MapPolygonLayer((AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonLayer)mapLayer, this.m_map);
			}
			if (mapLayer is AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointLayer)
			{
				return new MapPointLayer((AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointLayer)mapLayer, this.m_map);
			}
			if (mapLayer is AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineLayer)
			{
				return new MapLineLayer((AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineLayer)mapLayer, this.m_map);
			}
			return null;
		}
	}
}
