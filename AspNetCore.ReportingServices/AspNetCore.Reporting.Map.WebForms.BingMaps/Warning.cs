using System;
using System.Runtime.Serialization;

namespace AspNetCore.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	internal class Warning
	{
		[DataMember(Name = "origin", EmitDefaultValue = false)]
		public string Origin
		{
			get;
			set;
		}

		public Coordinate OriginCoordinate
		{
			get
			{
				if (string.IsNullOrEmpty(this.Origin))
				{
					return null;
				}
				string[] array = this.Origin.Split(new char[1]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				double longitude = default(double);
				double latitude = default(double);
				if (array.Length >= 2 && double.TryParse(array[0], out latitude) && double.TryParse(array[1], out longitude))
				{
					return new Coordinate(latitude, longitude);
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					this.Origin = string.Empty;
				}
				else
				{
					this.Origin = string.Format("{0},{1}", value.Latitude, value.Longitude);
				}
			}
		}

		[DataMember(Name = "severity", EmitDefaultValue = false)]
		public string Severity
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

		[DataMember(Name = "to", EmitDefaultValue = false)]
		public string To
		{
			get;
			set;
		}

		public Coordinate ToCoordinate
		{
			get
			{
				if (string.IsNullOrEmpty(this.To))
				{
					return null;
				}
				string[] array = this.To.Split(new char[1]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				double longitude = default(double);
				double latitude = default(double);
				if (array.Length >= 2 && double.TryParse(array[0], out latitude) && double.TryParse(array[1], out longitude))
				{
					return new Coordinate(latitude, longitude);
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					this.To = string.Empty;
				}
				else
				{
					this.To = string.Format("{0},{1}", value.Latitude, value.Longitude);
				}
			}
		}

		[DataMember(Name = "warningType", EmitDefaultValue = false)]
		public string WarningType
		{
			get;
			set;
		}
	}
}
