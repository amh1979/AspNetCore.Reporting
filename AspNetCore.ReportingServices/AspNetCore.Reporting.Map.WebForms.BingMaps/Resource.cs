using System.Runtime.Serialization;

namespace AspNetCore.Reporting.Map.WebForms.BingMaps
{
	[KnownType(typeof(TrafficIncident))]
	[DataContract]
	[KnownType(typeof(Location))]
	[KnownType(typeof(Route))]
	[KnownType(typeof(ElevationData))]
	[KnownType(typeof(ImageryMetadata))]
	[KnownType(typeof(SeaLevelData))]
	[KnownType(typeof(CompressedPointList))]
	[KnownType(typeof(GeospatialEndpointResponse))]
	internal class Resource
	{
		[DataMember(Name = "bbox", EmitDefaultValue = false)]
		public double[] BoundingBox
		{
			get;
			set;
		}

		[DataMember(Name = "__type", EmitDefaultValue = false)]
		public string Type
		{
			get;
			set;
		}
	}
}
