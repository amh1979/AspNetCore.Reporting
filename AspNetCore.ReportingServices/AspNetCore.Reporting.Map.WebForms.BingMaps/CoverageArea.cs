using System.Runtime.Serialization;

namespace AspNetCore.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	internal class CoverageArea
	{
		[DataMember(Name = "bbox", EmitDefaultValue = false)]
		public double[] BoundingBox
		{
			get;
			set;
		}

		[DataMember(Name = "zoomMax", EmitDefaultValue = false)]
		public int ZoomMax
		{
			get;
			set;
		}

		[DataMember(Name = "zoomMin", EmitDefaultValue = false)]
		public int ZoomMin
		{
			get;
			set;
		}
	}
}
