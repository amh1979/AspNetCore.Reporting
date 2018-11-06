namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLineCollection : MapObjectCollectionBase<MapLine>, ISpatialElementCollection
	{
		private Map m_map;

		private MapLineLayer m_mapLineLayer;

		public override int Count
		{
			get
			{
				return this.m_mapLineLayer.MapLineLayerDef.MapLines.Count;
			}
		}

		internal MapLineCollection(MapLineLayer mapLineLayer, Map map)
		{
			this.m_mapLineLayer = mapLineLayer;
			this.m_map = map;
		}

		protected override MapLine CreateMapObject(int index)
		{
			return new MapLine(this.m_mapLineLayer.MapLineLayerDef.MapLines[index], this.m_mapLineLayer, this.m_map);
		}

		MapSpatialElement ISpatialElementCollection.GetItem(int index)
		{
			return ((ReportElementCollectionBase<MapLine>)this)[index];
		}
	}
}
