namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapMarkerCollection : MapObjectCollectionBase<MapMarker>
	{
		private Map m_map;

		private MapMarkerRule m_markerRule;

		public override int Count
		{
			get
			{
				return this.m_markerRule.MapMarkerRuleDef.MapMarkers.Count;
			}
		}

		internal MapMarkerCollection(MapMarkerRule markerRule, Map map)
		{
			this.m_markerRule = markerRule;
			this.m_map = map;
		}

		protected override MapMarker CreateMapObject(int index)
		{
			return new MapMarker(this.m_markerRule.MapMarkerRuleDef.MapMarkers[index], this.m_map);
		}
	}
}
