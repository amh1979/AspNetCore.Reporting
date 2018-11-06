namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapBucketCollection : MapObjectCollectionBase<MapBucket>
	{
		private Map m_map;

		private MapAppearanceRule m_mapApperanceRule;

		public override int Count
		{
			get
			{
				return this.m_mapApperanceRule.MapAppearanceRuleDef.MapBuckets.Count;
			}
		}

		internal MapBucketCollection(MapAppearanceRule mapApperanceRule, Map map)
		{
			this.m_mapApperanceRule = mapApperanceRule;
			this.m_map = map;
		}

		protected override MapBucket CreateMapObject(int index)
		{
			return new MapBucket(this.m_mapApperanceRule.MapAppearanceRuleDef.MapBuckets[index], this.m_map);
		}
	}
}
