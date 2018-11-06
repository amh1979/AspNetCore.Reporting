using System.Runtime.Serialization;

namespace AspNetCore.Reporting.Map.WebForms.BingMaps
{
	[DataContract(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
	internal class CompressedPointList : Resource
	{
		[DataMember(Name = "value", EmitDefaultValue = false)]
		public string Value
		{
			get;
			set;
		}
	}
}
