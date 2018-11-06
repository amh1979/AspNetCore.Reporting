using System.Runtime.Serialization;

namespace AspNetCore.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	internal class Shield
	{
		[DataMember(Name = "labels", EmitDefaultValue = false)]
		public string[] Labels
		{
			get;
			set;
		}

		[DataMember(Name = "roadShieldType", EmitDefaultValue = false)]
		public int RoadShieldType
		{
			get;
			set;
		}
	}
}
