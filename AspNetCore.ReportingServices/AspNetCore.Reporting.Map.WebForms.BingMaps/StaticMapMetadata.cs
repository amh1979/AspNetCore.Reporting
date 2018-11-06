using System.Runtime.Serialization;

namespace AspNetCore.Reporting.Map.WebForms.BingMaps
{
	[DataContract(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
	internal class StaticMapMetadata : ImageryMetadata
	{
		[DataMember(Name = "mapCenter", EmitDefaultValue = false)]
		public Point MapCenter
		{
			get;
			set;
		}

		[DataMember(Name = "pushpins", EmitDefaultValue = false)]
		public PushpinMetdata[] Pushpins
		{
			get;
			set;
		}

		[DataMember(Name = "zoom", EmitDefaultValue = false)]
		public string Zoom
		{
			get;
			set;
		}
	}
}
