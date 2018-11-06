using System.Runtime.Serialization;

namespace AspNetCore.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	internal class QueryParseValue
	{
		[DataMember(Name = "property", EmitDefaultValue = false)]
		public string Property
		{
			get;
			set;
		}

		[DataMember(Name = "value", EmitDefaultValue = false)]
		public string Value
		{
			get;
			set;
		}
	}
}
