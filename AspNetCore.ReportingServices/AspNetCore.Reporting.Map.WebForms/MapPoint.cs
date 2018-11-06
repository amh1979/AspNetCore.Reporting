using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(MapPointConverter))]
	internal struct MapPoint
	{
		private double x;

		private double y;

		[SRDescription("DescriptionAttributeMapPoint_X")]
		public double X
		{
			get
			{
				return this.x;
			}
			set
			{
				this.x = value;
			}
		}

		[SRDescription("DescriptionAttributeMapPoint_Y")]
		public double Y
		{
			get
			{
				return this.y;
			}
			set
			{
				this.y = value;
			}
		}

		public MapPoint(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		public override string ToString()
		{
			return this.X.ToString(CultureInfo.CurrentCulture) + ", " + this.Y.ToString(CultureInfo.CurrentCulture);
		}

		public override bool Equals(object obj)
		{
			if (obj is MapPoint)
			{
				MapPoint mapPoint = (MapPoint)obj;
				if (mapPoint.X == this.X)
				{
					return mapPoint.Y == this.Y;
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((ValueType)(object)this).GetHashCode();
		}

		public PointF ToPointF()
		{
			return new PointF((float)this.X, (float)this.Y);
		}
	}
}
