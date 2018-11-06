using System;

namespace AspNetCore.Reporting.Map.WebForms.BingMaps
{
	internal class ImageryMetadataRequest
	{
		internal enum ImageryType
		{
			Aerial,
			AerialWithLabels,
			Birdseye,
			BirdseyeWithLabels,
			CanvasDark,
			CanvasLight,
			CanvasGray,
			Road,
			RoadOnDemand,
			OrdnanceSurvey,
			CollinsBart
		}

		private double orientation;

		private int zoomLevel;

		public string BingMapsKey
		{
			get;
			set;
		}

		public string Culture
		{
			get;
			set;
		}

		public ImageryType ImagerySet
		{
			get;
			set;
		}

		public bool IncludeImageryProviders
		{
			get;
			set;
		}

		public double Orientation
		{
			get
			{
				return this.orientation;
			}
			set
			{
				if (value < 0.0)
				{
					this.orientation = value % 360.0 + 360.0;
				}
				else if (value > 360.0)
				{
					this.orientation = value % 360.0;
				}
				else
				{
					this.orientation = value;
				}
			}
		}

		public int ZoomLevel
		{
			get
			{
				return this.zoomLevel;
			}
			set
			{
				if (value < 1)
				{
					this.zoomLevel = 1;
				}
				else if (value > 21)
				{
					this.zoomLevel = 21;
				}
				else
				{
					this.zoomLevel = value;
				}
			}
		}

		public bool UseHTTPS
		{
			get;
			set;
		}

		public string GetRequestUrl()
		{
			string str = "https://dev.virtualearth.net/REST/v1/Imagery/Metadata/";
			str += Enum.GetName(typeof(ImageryType), this.ImagerySet);
			str += "?";
			if (this.orientation != 0.0)
			{
				str = str + "&dir=" + this.orientation;
			}
			if (this.IncludeImageryProviders)
			{
				str += "&incl=ImageryProviders";
			}
			if (this.UseHTTPS)
			{
				str += "&uriScheme=https";
			}
			if (!string.IsNullOrEmpty(this.Culture))
			{
				str = str + "&c=" + this.Culture;
			}
			return str + "&key=" + this.BingMapsKey + "&clientApi=SSRS";
		}
	}
}
