namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapFieldCollection : MapObjectCollectionBase<MapField>
	{
		private Map m_map;

		private MapSpatialElement m_mapSpatialElement;

		public override int Count
		{
			get
			{
				return this.m_mapSpatialElement.MapSpatialElementDef.MapFields.Count;
			}
		}

		internal MapFieldCollection(MapSpatialElement mapSpatialElement, Map map)
		{
			this.m_mapSpatialElement = mapSpatialElement;
			this.m_map = map;
		}

		protected override MapField CreateMapObject(int index)
		{
			return new MapField(this.m_mapSpatialElement.MapSpatialElementDef.MapFields[index], this.m_map);
		}
	}
}
