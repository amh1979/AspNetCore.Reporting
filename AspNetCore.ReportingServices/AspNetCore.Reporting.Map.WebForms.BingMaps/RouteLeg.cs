using System;
using System.Runtime.Serialization;

namespace AspNetCore.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	internal class RouteLeg
	{
		[DataMember(Name = "travelDistance", EmitDefaultValue = false)]
		public double TravelDistance
		{
			get;
			set;
		}

		[DataMember(Name = "travelDuration", EmitDefaultValue = false)]
		public double TravelDuration
		{
			get;
			set;
		}

		[DataMember(Name = "cost", EmitDefaultValue = false)]
		public double Cost
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

		[DataMember(Name = "actualStart", EmitDefaultValue = false)]
		public Point ActualStart
		{
			get;
			set;
		}

		[DataMember(Name = "actualEnd", EmitDefaultValue = false)]
		public Point ActualEnd
		{
			get;
			set;
		}

		[DataMember(Name = "startLocation", EmitDefaultValue = false)]
		public Location StartLocation
		{
			get;
			set;
		}

		[DataMember(Name = "endLocation", EmitDefaultValue = false)]
		public Location EndLocation
		{
			get;
			set;
		}

		[DataMember(Name = "itineraryItems", EmitDefaultValue = false)]
		public ItineraryItem[] ItineraryItems
		{
			get;
			set;
		}

		[DataMember(Name = "routeRegion", EmitDefaultValue = false)]
		public string RouteRegion
		{
			get;
			set;
		}

		[DataMember(Name = "routeSubLegs", EmitDefaultValue = false)]
		public RouteSubLeg[] RouteSubLegs
		{
			get;
			set;
		}

		[DataMember(Name = "startTime", EmitDefaultValue = false)]
		public string StartTime
		{
			get;
			set;
		}

		public DateTime StartTimeUtc
		{
			get
			{
				if (string.IsNullOrEmpty(this.StartTime))
				{
					return DateTime.Now;
				}
				return DateTimeHelper.FromOdataJson(this.StartTime);
			}
			set
			{
				string text = DateTimeHelper.ToOdataJson(value);
				if (text != null)
				{
					this.StartTime = text;
				}
				else
				{
					this.StartTime = string.Empty;
				}
			}
		}

		[DataMember(Name = "endTime", EmitDefaultValue = false)]
		public string EndTime
		{
			get;
			set;
		}

		public DateTime EndTimeUtc
		{
			get
			{
				if (string.IsNullOrEmpty(this.EndTime))
				{
					return DateTime.Now;
				}
				return DateTimeHelper.FromOdataJson(this.EndTime);
			}
			set
			{
				string text = DateTimeHelper.ToOdataJson(value);
				if (text != null)
				{
					this.EndTime = text;
				}
				else
				{
					this.EndTime = string.Empty;
				}
			}
		}

		[DataMember(Name = "alternateVias", EmitDefaultValue = false)]
		public object[] AlternateVias
		{
			get;
			set;
		}
	}
}
