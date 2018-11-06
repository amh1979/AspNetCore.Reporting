namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapTileCollection : MapObjectCollectionBase<MapTile>
	{
		private Map m_map;

		private MapTileLayer m_mapTileLayer;

		public override int Count
		{
			get
			{
				return this.m_mapTileLayer.MapTileLayerDef.MapTiles.Count;
			}
		}

		internal MapTileCollection(MapTileLayer mapTileLayer, Map map)
		{
			this.m_mapTileLayer = mapTileLayer;
			this.m_map = map;
		}

		protected override MapTile CreateMapObject(int index)
		{
			return new MapTile(this.m_mapTileLayer.MapTileLayerDef.MapTiles[index], this.m_map);
		}
	}
}
