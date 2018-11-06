namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapCustomColorCollection : MapObjectCollectionBase<MapCustomColor>
	{
		private Map m_map;

		private MapCustomColorRule m_customColorRule;

		public override int Count
		{
			get
			{
				return this.m_customColorRule.MapCustomColorRuleDef.MapCustomColors.Count;
			}
		}

		internal MapCustomColorCollection(MapCustomColorRule customColorRule, Map map)
		{
			this.m_customColorRule = customColorRule;
			this.m_map = map;
		}

		protected override MapCustomColor CreateMapObject(int index)
		{
			return new MapCustomColor(this.m_customColorRule.MapCustomColorRuleDef.MapCustomColors[index], this.m_map);
		}
	}
}
