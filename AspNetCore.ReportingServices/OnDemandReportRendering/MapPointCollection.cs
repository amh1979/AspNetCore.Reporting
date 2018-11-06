namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPointCollection : MapObjectCollectionBase<MapPoint>, ISpatialElementCollection
	{
		private Map m_map;

		private MapPointLayer m_mapPointLayer;

		public override int Count
		{
			get
			{
				return this.m_mapPointLayer.MapPointLayerDef.MapPoints.Count;
			}
		}

		internal MapPointCollection(MapPointLayer mapPointLayer, Map map)
		{
			this.m_mapPointLayer = mapPointLayer;
			this.m_map = map;
		}

		protected override MapPoint CreateMapObject(int index)
		{
			return new MapPoint(this.m_mapPointLayer.MapPointLayerDef.MapPoints[index], this.m_mapPointLayer, this.m_map);
		}

		MapSpatialElement ISpatialElementCollection.GetItem(int index)
		{
			return ((ReportElementCollectionBase<MapPoint>)this)[index];
		}
	}
}
