using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class MapParameters : SvgParameters
	{
		internal Color mapBrushColor;

		internal Color mapBrushSecondColor;

		internal Matrix mapMatrix;

		internal Font mapFont;

		internal StringFormat mapStringFormat;

		internal SvgGradientType mapSvgGradientType;

		internal Size mapPictureSize;

		internal WrapMode imageWrapMode;

		private SvgFillType svgFillType;

		private Color mapPenColor;

		private float mapPenWidth;

		private SvgDashStyle mapDashStyle = SvgDashStyle.Solid;

		private FillMode tempFillMode;

		private Brush brush;

		private Pen pen;

		protected override Color BrushColor
		{
			get
			{
				return this.mapBrushColor;
			}
		}

		public Brush Brush
		{
			set
			{
				this.brush = value;
				this.SetBrush();
			}
		}

		public Pen Pen
		{
			set
			{
				this.pen = value;
				this.SetPen();
			}
		}

		protected override Color BrushSecondColor
		{
			get
			{
				return this.mapBrushSecondColor;
			}
		}

		protected override SvgGradientType GradientType
		{
			get
			{
				return this.mapSvgGradientType;
			}
		}

		protected override SvgFillType FillType
		{
			get
			{
				return this.svgFillType;
			}
		}

		protected override Color PenColor
		{
			get
			{
				return this.mapPenColor;
			}
		}

		protected override double PenWidth
		{
			get
			{
				return (double)this.mapPenWidth;
			}
		}

		protected override SvgDashStyle DashStyle
		{
			get
			{
				return this.mapDashStyle;
			}
		}

		protected override Matrix Transform
		{
			get
			{
				return this.mapMatrix;
			}
		}

		protected override Font Font
		{
			get
			{
				return this.mapFont;
			}
		}

		protected override StringFormat StringFormat
		{
			get
			{
				return this.mapStringFormat;
			}
		}

		public override Size PictureSize
		{
			get
			{
				return this.mapPictureSize;
			}
			set
			{
				this.mapPictureSize = value;
			}
		}

		protected override FillMode FillMode
		{
			get
			{
				return this.tempFillMode;
			}
			set
			{
				this.tempFillMode = value;
			}
		}

		protected override SvgLineCapStyle SvgLineCap
		{
			get
			{
				if (this.pen.StartCap != 0 && this.pen.StartCap != LineCap.NoAnchor)
				{
					if (this.pen.StartCap != LineCap.Round && this.pen.StartCap != LineCap.RoundAnchor)
					{
						return SvgLineCapStyle.Square;
					}
					return SvgLineCapStyle.Round;
				}
				return SvgLineCapStyle.Butt;
			}
		}

		protected override SvgImageWrapMode ImageWrapMode
		{
			get
			{
				return (SvgImageWrapMode)this.imageWrapMode;
			}
		}

		protected override Color TextColor
		{
			get
			{
				return this.mapBrushColor;
			}
		}

		protected string ToUSString(float number)
		{
			return number.ToString(CultureInfo.InvariantCulture);
		}

		protected string ToUSString(double number)
		{
			return number.ToString(CultureInfo.InvariantCulture);
		}

		protected override string GetX(double x)
		{
			return this.ToUSString(x);
		}

		protected override string GetY(double y)
		{
			return this.ToUSString(y);
		}

		protected override string GetX(PointF point)
		{
			return this.ToUSString(point.X);
		}

		protected override string GetX(RectangleF rectangle)
		{
			return this.ToUSString(rectangle.X);
		}

		protected override string GetWidth(RectangleF rectangle)
		{
			return this.ToUSString(rectangle.Width);
		}

		protected override string GetHeight(RectangleF rectangle)
		{
			return this.ToUSString(rectangle.Height);
		}

		protected override string GetY(PointF point)
		{
			return this.ToUSString(point.Y);
		}

		protected override string GetY(RectangleF rectangle)
		{
			return this.ToUSString(rectangle.Y);
		}

		private void SetBrush()
		{
			if (this.brush is SolidBrush)
			{
				this.mapBrushColor = ((SolidBrush)this.brush).Color;
				this.svgFillType = SvgFillType.Solid;
			}
			else if (this.brush is LinearGradientBrush)
			{
				this.mapBrushColor = ((LinearGradientBrush)this.brush).LinearColors[0];
				this.mapBrushSecondColor = ((LinearGradientBrush)this.brush).LinearColors[1];
				this.svgFillType = SvgFillType.Gradient;
			}
			else if (this.brush is HatchBrush)
			{
				this.mapBrushColor = ((HatchBrush)this.brush).BackgroundColor;
				this.svgFillType = SvgFillType.Solid;
			}
		}

		private void SetPen()
		{
			if (this.pen != null)
			{
				this.mapPenColor = this.pen.Color;
				this.mapPenWidth = this.pen.Width;
				switch (this.pen.DashStyle)
				{
				case System.Drawing.Drawing2D.DashStyle.Custom:
					this.mapDashStyle = SvgDashStyle.Custom;
					break;
				case System.Drawing.Drawing2D.DashStyle.Dash:
					this.mapDashStyle = SvgDashStyle.Dash;
					break;
				case System.Drawing.Drawing2D.DashStyle.DashDot:
					this.mapDashStyle = SvgDashStyle.DashDot;
					break;
				case System.Drawing.Drawing2D.DashStyle.DashDotDot:
					this.mapDashStyle = SvgDashStyle.DashDotDot;
					break;
				case System.Drawing.Drawing2D.DashStyle.Dot:
					this.mapDashStyle = SvgDashStyle.Dot;
					break;
				case System.Drawing.Drawing2D.DashStyle.Solid:
					this.mapDashStyle = SvgDashStyle.Solid;
					break;
				}
			}
		}
	}
}
