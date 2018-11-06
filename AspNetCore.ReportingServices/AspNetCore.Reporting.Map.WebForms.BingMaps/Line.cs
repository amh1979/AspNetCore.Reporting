using System.Runtime.Serialization;

namespace AspNetCore.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	internal class Line
	{
		[DataMember(Name = "type", EmitDefaultValue = false)]
		public string Type
		{
			get;
			set;
		}

		[DataMember(Name = "coordinates", EmitDefaultValue = false)]
		public double[][] Coordinates
		{
			get;
			set;
		}
	}
}
