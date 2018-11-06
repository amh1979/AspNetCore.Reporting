namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPolygonCollection : MapObjectCollectionBase<MapPolygon>, ISpatialElementCollection
	{
		private Map m_map;

		private MapPolygonLayer m_mapPolygonLayer;

		public override int Count
		{
			get
			{
				return this.m_mapPolygonLayer.MapPolygonLayerDef.MapPolygons.Count;
			}
		}

		internal MapPolygonCollection(MapPolygonLayer mapPolygonLayer, Map map)
		{
			this.m_mapPolygonLayer = mapPolygonLayer;
			this.m_map = map;
		}

		protected override MapPolygon CreateMapObject(int index)
		{
			return new MapPolygon(this.m_mapPolygonLayer.MapPolygonLayerDef.MapPolygons[index], this.m_mapPolygonLayer, this.m_map);
		}

		MapSpatialElement ISpatialElementCollection.GetItem(int index)
		{
			return ((ReportElementCollectionBase<MapPolygon>)this)[index];
		}
	}
}
