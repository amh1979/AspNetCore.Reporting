using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.Svg
{
	internal class ChartParameters : SvgParameters
	{
		internal Color chartBrushColor;

		internal Color chartBrushSecondColor;

		internal Matrix chartMatrix;

		internal Font chartFont;

		internal StringFormat chartStringFormat;

		internal SvgGradientType chartSvgGradientType;

		internal Size chartPictureSize;

		internal WrapMode imageWrapMode;

		private SvgFillType svgFillType;

		private Color chartPenColor;

		private float chartPenWidth;

		private SvgDashStyle chartDashStyle = SvgDashStyle.Solid;

		private FillMode tempFillMode;

		private Brush brush;

		private Pen pen;

		protected override Color BrushColor
		{
			get
			{
				return this.chartBrushColor;
			}
		}

		public Brush Brush
		{
			get
			{
				return this.brush;
			}
			set
			{
				this.brush = value;
				this.SetBrush();
			}
		}

		public Pen Pen
		{
			get
			{
				return this.pen;
			}
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
				return this.chartBrushSecondColor;
			}
		}

		protected override SvgGradientType GradientType
		{
			get
			{
				return this.chartSvgGradientType;
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
				return this.chartPenColor;
			}
		}

		protected override double PenWidth
		{
			get
			{
				return (double)this.chartPenWidth;
			}
		}

		protected override SvgDashStyle DashStyle
		{
			get
			{
				return this.chartDashStyle;
			}
		}

		protected override Matrix Transform
		{
			get
			{
				return this.chartMatrix;
			}
		}

		protected override Font Font
		{
			get
			{
				return this.chartFont;
			}
		}

		protected override StringFormat StringFormat
		{
			get
			{
				return this.chartStringFormat;
			}
		}

		public override Size PictureSize
		{
			get
			{
				return this.chartPictureSize;
			}
			set
			{
				this.chartPictureSize = value;
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
				return this.chartBrushColor;
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
				this.chartBrushColor = ((SolidBrush)this.brush).Color;
				this.svgFillType = SvgFillType.Solid;
			}
			else if (this.brush is LinearGradientBrush)
			{
				this.chartBrushColor = ((LinearGradientBrush)this.brush).LinearColors[0];
				this.chartBrushSecondColor = ((LinearGradientBrush)this.brush).LinearColors[1];
				this.svgFillType = SvgFillType.Gradient;
			}
			else if (this.brush is HatchBrush)
			{
				this.chartBrushColor = ((HatchBrush)this.brush).BackgroundColor;
				this.svgFillType = SvgFillType.Solid;
			}
		}

		private void SetPen()
		{
			if (this.pen != null)
			{
				this.chartPenColor = this.pen.Color;
				this.chartPenWidth = this.pen.Width;
				switch (this.pen.DashStyle)
				{
				case System.Drawing.Drawing2D.DashStyle.Custom:
					this.chartDashStyle = SvgDashStyle.Custom;
					break;
				case System.Drawing.Drawing2D.DashStyle.Dash:
					this.chartDashStyle = SvgDashStyle.Dash;
					break;
				case System.Drawing.Drawing2D.DashStyle.DashDot:
					this.chartDashStyle = SvgDashStyle.DashDot;
					break;
				case System.Drawing.Drawing2D.DashStyle.DashDotDot:
					this.chartDashStyle = SvgDashStyle.DashDotDot;
					break;
				case System.Drawing.Drawing2D.DashStyle.Dot:
					this.chartDashStyle = SvgDashStyle.Dot;
					break;
				case System.Drawing.Drawing2D.DashStyle.Solid:
					this.chartDashStyle = SvgDashStyle.Solid;
					break;
				}
			}
		}
	}
}
