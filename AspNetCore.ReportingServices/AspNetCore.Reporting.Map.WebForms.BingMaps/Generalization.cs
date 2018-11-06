using System.Runtime.Serialization;

namespace AspNetCore.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	internal class Generalization
	{
		[DataMember(Name = "pathIndices", EmitDefaultValue = false)]
		public int[] PathIndices
		{
			get;
			set;
		}

		[DataMember(Name = "latLongTolerance", EmitDefaultValue = false)]
		public double LatLongTolerance
		{
			get;
			set;
		}
	}
}
