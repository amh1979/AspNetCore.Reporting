using System.Runtime.Serialization;

namespace AspNetCore.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	internal class ResourceSet
	{
		[DataMember(Name = "estimatedTotal", EmitDefaultValue = false)]
		public long EstimatedTotal
		{
			get;
			set;
		}

		[DataMember(Name = "resources", EmitDefaultValue = false)]
		public Resource[] Resources
		{
			get;
			set;
		}
	}
}
