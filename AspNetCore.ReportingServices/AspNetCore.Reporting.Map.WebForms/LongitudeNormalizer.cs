
//

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class LongitudeNormalizer //: IGeographySink
	{
		private const double MaxLongitude = 179.99999999;
        /*
		private SqlGeographyBuilder Builder;

		public SqlGeography Result
		{
			get
			{
				return this.Builder.ConstructedGeography;
			}
		}
        */
		private double GetNormalizedLatitude(double longitude)
		{
			double num = longitude;
			if (longitude < -180.0)
			{
				num = (longitude - 180.0) % 360.0 + 180.0;
			}
			else if (longitude > 180.0)
			{
				num = (longitude + 180.0) % 360.0 - 180.0;
			}
			if (num > 179.99999999)
			{
				num = 179.99999999;
			}
			else if (num < -179.99999999)
			{
				num = -179.99999999;
			}
			return num;
		}

		public LongitudeNormalizer()
		{
			//this.Builder = new SqlGeographyBuilder();
		}

		public void AddLine(double latitude, double longitude, double? z, double? m)
		{
			//this.Builder.AddLine(latitude, this.GetNormalizedLatitude(longitude));
		}

		public void BeginFigure(double latitude, double longitude, double? z, double? m)
		{
			//this.Builder.BeginFigure(latitude, this.GetNormalizedLatitude(longitude));
		}

		/*
        public void BeginGeography(OpenGisGeographyType type)
		{
			this.Builder.BeginGeography(type);
		}
        */
		public void EndFigure()
		{
			//this.Builder.EndFigure();
		}

		public void EndGeography()
		{
			//this.Builder.EndGeography();
		}

		public void SetSrid(int srid)
		{
			//this.Builder.SetSrid(srid);
		}
	}
}
