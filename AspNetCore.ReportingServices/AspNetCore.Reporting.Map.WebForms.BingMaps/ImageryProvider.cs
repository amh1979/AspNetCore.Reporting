using System.Runtime.Serialization;

namespace AspNetCore.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	internal class ImageryProvider
	{
		[DataMember(Name = "attribution", EmitDefaultValue = false)]
		public string Attribution
		{
			get;
			set;
		}

		[DataMember(Name = "coverageAreas", EmitDefaultValue = false)]
		public CoverageArea[] CoverageAreas
		{
			get;
			set;
		}
	}
}
