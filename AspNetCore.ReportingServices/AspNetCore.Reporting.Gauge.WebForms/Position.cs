using System.Drawing;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class Position
	{
		private GaugeLocation location;

		private GaugeSize size;

		private ContentAlignment locationAlignment;

		public bool DefaultValues
		{
			get
			{
				if (this.location.DefaultValues)
				{
					return this.size.DefaultValues;
				}
				return false;
			}
		}

		internal RectangleF Rectangle
		{
			get
			{
				RectangleF result = new RectangleF(this.location, this.size);
				switch (this.locationAlignment)
				{
				case ContentAlignment.TopCenter:
					result.X -= (float)(this.size.Width / 2.0);
					break;
				case ContentAlignment.TopRight:
					result.X -= this.size.Width;
					break;
				case ContentAlignment.MiddleLeft:
					result.Y -= (float)(this.size.Height / 2.0);
					break;
				case ContentAlignment.MiddleCenter:
					result.X -= (float)(this.size.Width / 2.0);
					result.Y -= (float)(this.size.Height / 2.0);
					break;
				case ContentAlignment.MiddleRight:
					result.X -= this.size.Width;
					result.Y -= (float)(this.size.Height / 2.0);
					break;
				case ContentAlignment.BottomLeft:
					result.Y -= this.size.Height;
					break;
				case ContentAlignment.BottomCenter:
					result.X -= (float)(this.size.Width / 2.0);
					result.Y -= this.size.Height;
					break;
				case ContentAlignment.BottomRight:
					result.X -= this.size.Width;
					result.Y -= this.size.Height;
					break;
				}
				return result;
			}
		}

		public Position(GaugeLocation location, GaugeSize size, ContentAlignment locationAlignment)
		{
			this.location = location;
			this.size = size;
			this.locationAlignment = locationAlignment;
		}
	}
}
