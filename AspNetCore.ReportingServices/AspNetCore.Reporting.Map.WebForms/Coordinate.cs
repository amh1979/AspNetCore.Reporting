using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace AspNetCore.Reporting.Map.WebForms
{
	[DataContract]
	internal class Coordinate
	{
		private double _latitude;

		private double _longitude;

		[DataMember(Name = "lat", EmitDefaultValue = false)]
		public double Latitude
		{
			get
			{
				return this._latitude;
			}
			set
			{
				if (!double.IsNaN(value) && value <= 90.0 && value >= -90.0)
				{
					this._latitude = Math.Round(value, 5, MidpointRounding.AwayFromZero);
				}
			}
		}

		[DataMember(Name = "lon", EmitDefaultValue = false)]
		public double Longitude
		{
			get
			{
				return this._longitude;
			}
			set
			{
				if (!double.IsNaN(value) && value <= 180.0 && value >= -180.0)
				{
					this._longitude = Math.Round(value, 5, MidpointRounding.AwayFromZero);
				}
			}
		}

		public Coordinate()
		{
		}

		public Coordinate(double latitude, double longitude)
		{
			this.Latitude = latitude;
			this.Longitude = longitude;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0:0.#####},{1:0.#####}", this.Latitude, this.Longitude);
		}
	}
}
