using System.Runtime.Serialization;

namespace AspNetCore.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	internal class RoadShield
	{
		[DataMember(Name = "bucket", EmitDefaultValue = false)]
		public int Bucket
		{
			get;
			set;
		}

		[DataMember(Name = "shields", EmitDefaultValue = false)]
		public Shield[] Shields
		{
			get;
			set;
		}
	}
}
