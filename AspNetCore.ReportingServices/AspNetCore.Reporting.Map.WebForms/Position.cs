using System.Drawing;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class Position
	{
		private MapLocation location;

		private MapSize size;

		private ContentAlignment locationAlignment;

		public float X
		{
			get
			{
				return this.location.X;
			}
		}

		public float Y
		{
			get
			{
				return this.location.Y;
			}
		}

		public float Width
		{
			get
			{
				return this.size.Width;
			}
		}

		public float Height
		{
			get
			{
				return this.size.Height;
			}
		}

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

		public Position(MapLocation location, MapSize size, ContentAlignment locationAlignment)
		{
			this.location = location;
			this.size = size;
			this.locationAlignment = locationAlignment;
		}
	}
}
