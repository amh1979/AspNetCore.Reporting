using System.Runtime.Serialization;

namespace AspNetCore.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	internal class Hint
	{
		[DataMember(Name = "hintType", EmitDefaultValue = false)]
		public string HintType
		{
			get;
			set;
		}

		[DataMember(Name = "text", EmitDefaultValue = false)]
		public string Text
		{
			get;
			set;
		}
	}
}
