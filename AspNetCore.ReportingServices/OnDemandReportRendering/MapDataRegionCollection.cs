using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapDataRegionCollection : MapObjectCollectionBase<MapDataRegion>
	{
		private Map m_map;

		public MapDataRegion this[string name]
		{
			get
			{
				for (int i = 0; i < this.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion mapDataRegion = this.m_map.MapDef.MapDataRegions[i];
					if (string.CompareOrdinal(name, mapDataRegion.Name) == 0)
					{
						return ((ReportElementCollectionBase<MapDataRegion>)this)[i];
					}
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsNotInCollection, name);
			}
		}

		public override int Count
		{
			get
			{
				if (this.m_map.MapDef.MapDataRegions != null)
				{
					return this.m_map.MapDef.MapDataRegions.Count;
				}
				return 0;
			}
		}

		internal MapDataRegionCollection(Map map)
		{
			this.m_map = map;
		}

		protected override MapDataRegion CreateMapObject(int index)
		{
			return new MapDataRegion(this.m_map, index, this.m_map.MapDef.MapDataRegions[index], this.m_map.RenderingContext);
		}
	}
}
