using System.Runtime.Serialization;

namespace AspNetCore.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	[KnownType(typeof(Point))]
	internal class Shape
	{
		[DataMember(Name = "boundingBox", EmitDefaultValue = false)]
		public double[] BoundingBox
		{
			get;
			set;
		}
	}
}
