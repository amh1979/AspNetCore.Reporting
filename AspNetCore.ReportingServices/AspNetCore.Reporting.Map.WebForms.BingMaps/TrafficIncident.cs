using System;
using System.Runtime.Serialization;

namespace AspNetCore.Reporting.Map.WebForms.BingMaps
{
	[DataContract(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
	internal class TrafficIncident : Resource
	{
		[DataMember(Name = "point", EmitDefaultValue = false)]
		public Point Point
		{
			get;
			set;
		}

		[DataMember(Name = "congestion", EmitDefaultValue = false)]
		public string Congestion
		{
			get;
			set;
		}

		[DataMember(Name = "description", EmitDefaultValue = false)]
		public string Description
		{
			get;
			set;
		}

		[DataMember(Name = "detour", EmitDefaultValue = false)]
		public string Detour
		{
			get;
			set;
		}

		[DataMember(Name = "start", EmitDefaultValue = false)]
		public string Start
		{
			get;
			set;
		}

		public DateTime StartDateTimeUtc
		{
			get
			{
				if (string.IsNullOrEmpty(this.Start))
				{
					return DateTime.Now;
				}
				return DateTimeHelper.FromOdataJson(this.Start);
			}
			set
			{
				string text = DateTimeHelper.ToOdataJson(value);
				if (text != null)
				{
					this.Start = text;
				}
				else
				{
					this.Start = string.Empty;
				}
			}
		}

		[DataMember(Name = "end", EmitDefaultValue = false)]
		public string End
		{
			get;
			set;
		}

		public DateTime EndDateTimeUtc
		{
			get
			{
				if (string.IsNullOrEmpty(this.End))
				{
					return DateTime.Now;
				}
				return DateTimeHelper.FromOdataJson(this.End);
			}
			set
			{
				string text = DateTimeHelper.ToOdataJson(value);
				if (text != null)
				{
					this.End = text;
				}
				else
				{
					this.End = string.Empty;
				}
			}
		}

		[DataMember(Name = "incidentId", EmitDefaultValue = false)]
		public long IncidentId
		{
			get;
			set;
		}

		[DataMember(Name = "lane", EmitDefaultValue = false)]
		public string Lane
		{
			get;
			set;
		}

		[DataMember(Name = "lastModified", EmitDefaultValue = false)]
		public string LastModified
		{
			get;
			set;
		}

		public DateTime LastModifiedDateTimeUtc
		{
			get
			{
				if (string.IsNullOrEmpty(this.LastModified))
				{
					return DateTime.Now;
				}
				return DateTimeHelper.FromOdataJson(this.LastModified);
			}
			set
			{
				string text = DateTimeHelper.ToOdataJson(value);
				if (text != null)
				{
					this.LastModified = text;
				}
				else
				{
					this.LastModified = string.Empty;
				}
			}
		}

		[DataMember(Name = "roadClosed", EmitDefaultValue = false)]
		public bool RoadClosed
		{
			get;
			set;
		}

		[DataMember(Name = "severity", EmitDefaultValue = false)]
		public int Severity
		{
			get;
			set;
		}

		[DataMember(Name = "toPoint", EmitDefaultValue = false)]
		public Point ToPoint
		{
			get;
			set;
		}

		[DataMember(Name = "locationCodes", EmitDefaultValue = false)]
		public string[] LocationCodes
		{
			get;
			set;
		}

		[DataMember(Name = "type", EmitDefaultValue = false)]
		public new int Type
		{
			get;
			set;
		}

		[DataMember(Name = "verified", EmitDefaultValue = false)]
		public bool Verified
		{
			get;
			set;
		}
	}
}
