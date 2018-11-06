namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapFieldDefinitionCollection : MapObjectCollectionBase<MapFieldDefinition>
	{
		private Map m_map;

		private MapVectorLayer m_mapVectorLayer;

		public override int Count
		{
			get
			{
				return this.m_mapVectorLayer.MapVectorLayerDef.MapFieldDefinitions.Count;
			}
		}

		internal MapFieldDefinitionCollection(MapVectorLayer mapVectorLayer, Map map)
		{
			this.m_mapVectorLayer = mapVectorLayer;
			this.m_map = map;
		}

		protected override MapFieldDefinition CreateMapObject(int index)
		{
			return new MapFieldDefinition(this.m_mapVectorLayer.MapVectorLayerDef.MapFieldDefinitions[index], this.m_map);
		}

		internal MapFieldDefinition GetFieldDefinition(string name)
		{
			foreach (MapFieldDefinition item in this)
			{
				if (string.CompareOrdinal(name, item.Name) == 0)
				{
					return item;
				}
			}
			return null;
		}
	}
}
