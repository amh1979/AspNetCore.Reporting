using System.Runtime.Serialization;

namespace AspNetCore.Reporting.Map.WebForms.BingMaps
{
	[DataContract(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
	internal class ElevationData : Resource
	{
		[DataMember(Name = "elevations", EmitDefaultValue = false)]
		public int[] Elevations
		{
			get;
			set;
		}

		[DataMember(Name = "zoomLevel", EmitDefaultValue = false)]
		public int ZoomLevel
		{
			get;
			set;
		}
	}
}
