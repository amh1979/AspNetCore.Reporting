using AspNetCore.ReportingServices.RdlObjectModel.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	[XmlElementClass("MapSpatialDataRegion", typeof(MapSpatialDataRegion))]
	[XmlElementClass("MapSpatialDataSet", typeof(MapSpatialDataSet))]
	[XmlElementClass("MapShapefile", typeof(MapShapefile))]
	internal abstract class MapSpatialData : ReportObject
	{
		public MapSpatialData()
		{
		}

		internal MapSpatialData(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}
	}
}
