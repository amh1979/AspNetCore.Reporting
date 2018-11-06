using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class GaugeGraphics : RenderingEngine
	{
		internal CommonElements common;

		private Pen pen;

		private SolidBrush solidBrush;

		private float width;

		private float height;

		internal bool softShadows = true;

		private AntiAliasing antiAliasing = AntiAliasing.All;

		internal bool IsMetafile;

		internal PointF InitialOffset = new PointF(0f, 0f);

		private Stack graphicStates = new Stack();

		public new Graphics Graphics
		{
			get
			{
				return base.Graphics;
			}
			set
			{
				if (base.Graphics != value)
				{
					base.Graphics = value;
					if (base.Graphics.Transform != null)
					{
						this.InitialOffset.X = base.Graphics.Transform.OffsetX;
						this.InitialOffset.Y = base.Graphics.Transform.OffsetY;
					}
				}
			}
		}

		internal AntiAliasing AntiAliasing
		{
			get
			{
				return this.antiAliasing;
			}
			set
			{
				this.antiAliasing = value;
				if (this.Graphics != null)
				{
					if ((this.antiAliasing & AntiAliasing.Graphics) == AntiAliasing.Graphics)
					{
						base.SmoothingMode = SmoothingMode.AntiAlias;
					}
					else
					{
						base.SmoothingMode = SmoothingMode.None;
					}
				}
			}
		}

		internal static Brush GetHatchBrush(GaugeHatchStyle hatchStyle, Color backColor, Color foreColor)
		{
			HatchStyle hatchstyle = (HatchStyle)Enum.Parse(typeof(HatchStyle), ((Enum)(object)hatchStyle).ToString((IFormatProvider)CultureInfo.InvariantCulture));
			return new HatchBrush(hatchstyle, foreColor, backColor);
		}

		internal Brush GetTextureBrush(string name, Color backImageTranspColor, GaugeImageWrapMode mode)
		{
			Image image = this.common.ImageLoader.LoadImage(name);
			ImageAttributes imageAttributes = new ImageAttributes();
			imageAttributes.SetWrapMode((WrapMode)((mode == GaugeImageWrapMode.Unscaled) ? GaugeImageWrapMode.Scaled : mode));
			if (backImageTranspColor != Color.Empty)
			{
				imageAttributes.SetColorKey(backImageTranspColor, backImageTranspColor, ColorAdjustType.Default);
			}
			return new TextureBrush(image, new RectangleF(0f, 0f, (float)image.Width, (float)image.Height), imageAttributes);
		}

		internal Brush GetShadowBrush()
		{
			return new SolidBrush(this.GetShadowColor());
		}

		internal Color GetShadowColor()
		{
			int alpha = (int)(255.0 * this.common.GaugeCore.ShadowIntensity / 100.0);
			return Color.FromArgb(alpha, Color.Black);
		}

		internal Brush GetGradientBrush(RectangleF rectangle, Color firstColor, Color secondColor, GradientType type)
		{
			rectangle.Inflate(1f, 1f);
			Brush brush = null;
			float angle = 0f;
			if (rectangle.Height != 0.0 && rectangle.Width != 0.0)
			{
				switch (type)
				{
				case GradientType.LeftRight:
				case GradientType.VerticalCenter:
					angle = 0f;
					break;
				case GradientType.TopBottom:
				case GradientType.HorizontalCenter:
					angle = 90f;
					break;
				case GradientType.DiagonalLeft:
					angle = (float)(Math.Atan((double)(rectangle.Width / rectangle.Height)) * 180.0 / 3.1415926535897931);
					break;
				case GradientType.DiagonalRight:
					angle = (float)(180.0 - Math.Atan((double)(rectangle.Width / rectangle.Height)) * 180.0 / 3.1415926535897931);
					break;
				}
				if (type != GradientType.TopBottom && type != GradientType.LeftRight && type != GradientType.DiagonalLeft && type != GradientType.DiagonalRight && type != GradientType.HorizontalCenter && type != GradientType.VerticalCenter)
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					graphicsPath.AddRectangle(rectangle);
					brush = new PathGradientBrush(graphicsPath);
					((PathGradientBrush)brush).CenterColor = firstColor;
					((PathGradientBrush)brush).CenterPoint = new PointF((float)(rectangle.X + rectangle.Width / 2.0), (float)(rectangle.Y + rectangle.Height / 2.0));
					Color[] surroundColors = new Color[1]
					{
						secondColor
					};
					((PathGradientBrush)brush).SurroundColors = surroundColors;
					if (graphicsPath != null)
					{
						graphicsPath.Dispose();
					}
					return brush;
				}
				RectangleF rect = new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
				switch (type)
				{
				case GradientType.HorizontalCenter:
					rect.Height /= 2f;
					brush = new LinearGradientBrush(rect, firstColor, secondColor, angle);
					((LinearGradientBrush)brush).WrapMode = WrapMode.TileFlipX;
					break;
				case GradientType.VerticalCenter:
					rect.Width /= 2f;
					brush = new LinearGradientBrush(rect, firstColor, secondColor, angle);
					((LinearGradientBrush)brush).WrapMode = WrapMode.TileFlipX;
					break;
				default:
					brush = new LinearGradientBrush(rectangle, firstColor, secondColor, angle);
					break;
				}
				return brush;
			}
			return new SolidBrush(Color.Black);
		}

		internal Brush GetPieGradientBrush(RectangleF rectangle, Color firstColor, Color secondColor)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddEllipse(rectangle);
			PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
			pathGradientBrush.CenterColor = firstColor;
			pathGradientBrush.CenterPoint = new PointF((float)(rectangle.X + rectangle.Width / 2.0), (float)(rectangle.Y + rectangle.Height / 2.0));
			Color[] array = new Color[1]
			{
				secondColor
			};
			Color[] array3 = pathGradientBrush.SurroundColors = array;
			if (graphicsPath != null)
			{
				graphicsPath.Dispose();
			}
			return pathGradientBrush;
		}

		internal DashStyle GetPenStyle(GaugeDashStyle style)
		{
			switch (style)
			{
			case GaugeDashStyle.Dash:
				return DashStyle.Dash;
			case GaugeDashStyle.DashDot:
				return DashStyle.DashDot;
			case GaugeDashStyle.DashDotDot:
				return DashStyle.DashDotDot;
			case GaugeDashStyle.Dot:
				return DashStyle.Dot;
			default:
				return DashStyle.Solid;
			}
		}

		internal Brush GetMarkerBrush(GraphicsPath path, MarkerStyle markerStyle, PointF pointOrigin, float angle, Color fillColor, GradientType fillGradientType, Color fillGradientEndColor, GaugeHatchStyle fillHatchStyle)
		{
			Brush brush = null;
			if (fillHatchStyle != 0)
			{
				brush = GaugeGraphics.GetHatchBrush(fillHatchStyle, fillColor, fillGradientEndColor);
			}
			else if (fillGradientType != 0)
			{
				RectangleF bounds = path.GetBounds();
				if (markerStyle == MarkerStyle.Circle && fillGradientType == GradientType.DiagonalLeft)
				{
					Matrix matrix = new Matrix();
					matrix.RotateAt(45f, new PointF((float)(bounds.X + bounds.Width / 2.0), (float)(bounds.Y + bounds.Height / 2.0)));
					if (bounds.Width != bounds.Height)
					{
						bounds = path.GetBounds(matrix);
					}
					brush = this.GetGradientBrush(bounds, fillColor, fillGradientEndColor, GradientType.LeftRight);
					((LinearGradientBrush)brush).Transform = matrix;
				}
				else if (markerStyle == MarkerStyle.Circle && fillGradientType == GradientType.DiagonalRight)
				{
					Matrix matrix2 = new Matrix();
					matrix2.RotateAt(135f, new PointF((float)(bounds.X + bounds.Width / 2.0), (float)(bounds.Y + bounds.Height / 2.0)));
					if (bounds.Width != bounds.Height)
					{
						bounds = path.GetBounds(matrix2);
					}
					brush = this.GetGradientBrush(bounds, fillColor, fillGradientEndColor, GradientType.TopBottom);
					((LinearGradientBrush)brush).Transform = matrix2;
				}
				else if (markerStyle == MarkerStyle.Circle && fillGradientType == GradientType.Center)
				{
					bounds.Inflate(1f, 1f);
					using (GraphicsPath graphicsPath = new GraphicsPath())
					{
						graphicsPath.AddArc(bounds, 0f, 360f);
						PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
						pathGradientBrush.CenterColor = fillColor;
						pathGradientBrush.CenterPoint = new PointF((float)(bounds.X + bounds.Width / 2.0), (float)(bounds.Y + bounds.Height / 2.0));
						pathGradientBrush.SurroundColors = new Color[1]
						{
							fillGradientEndColor
						};
						brush = pathGradientBrush;
					}
				}
				else
				{
					brush = this.GetGradientBrush(path.GetBounds(), fillColor, fillGradientEndColor, fillGradientType);
				}
				if (brush is LinearGradientBrush)
				{
					((LinearGradientBrush)brush).RotateTransform(angle, MatrixOrder.Append);
					((LinearGradientBrush)brush).TranslateTransform(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
				}
				else if (brush is PathGradientBrush)
				{
					((PathGradientBrush)brush).RotateTransform(angle, MatrixOrder.Append);
					((PathGradientBrush)brush).TranslateTransform(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
				}
			}
			else
			{
				brush = new SolidBrush(fillColor);
			}
			return brush;
		}

		internal GraphicsPath CreateMarker(PointF point, float markerWidth, float markerHeight, MarkerStyle markerStyle)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			RectangleF empty = RectangleF.Empty;
			empty.X = (float)(point.X - markerWidth / 2.0);
			empty.Y = (float)(point.Y - markerHeight / 2.0);
			empty.Width = markerWidth;
			empty.Height = markerHeight;
			switch (markerStyle)
			{
			case MarkerStyle.Circle:
				graphicsPath.AddEllipse(empty);
				break;
			case MarkerStyle.Diamond:
			{
				PointF[] array = new PointF[4];
				array[0].X = empty.X;
				array[0].Y = (float)(empty.Y + empty.Height / 2.0);
				array[1].X = (float)(empty.X + empty.Width / 2.0);
				array[1].Y = empty.Top;
				array[2].X = empty.Right;
				array[2].Y = (float)(empty.Y + empty.Height / 2.0);
				array[3].X = (float)(empty.X + empty.Width / 2.0);
				array[3].Y = empty.Bottom;
				graphicsPath.AddPolygon(array);
				break;
			}
			case MarkerStyle.Star:
				graphicsPath.AddPolygon(this.CreateStarPolygon(empty, 5));
				break;
			case MarkerStyle.None:
			case MarkerStyle.Rectangle:
			{
				PointF[] array = new PointF[4];
				array[0].X = empty.X;
				array[0].Y = empty.Y;
				array[1].X = empty.X + empty.Width;
				array[1].Y = empty.Y;
				array[2].X = empty.X + empty.Width;
				array[2].Y = empty.Y + empty.Height;
				array[3].X = empty.X;
				array[3].Y = empty.Y + empty.Height;
				graphicsPath.AddPolygon(array);
				break;
			}
			case MarkerStyle.Trapezoid:
			{
				PointF[] array = new PointF[4];
				array[0].X = empty.X;
				array[0].Y = empty.Bottom;
				array[1].X = (float)(empty.X + empty.Width / 4.0);
				array[1].Y = empty.Top;
				array[2].X = (float)(empty.X + empty.Width / 4.0 * 3.0);
				array[2].Y = empty.Top;
				array[3].X = empty.Right;
				array[3].Y = empty.Bottom;
				graphicsPath.AddPolygon(array);
				break;
			}
			case MarkerStyle.Triangle:
			{
				PointF[] array = new PointF[3];
				array[0].X = empty.X;
				array[0].Y = empty.Bottom;
				array[1].X = (float)(empty.X + empty.Width / 2.0);
				array[1].Y = empty.Top;
				array[2].X = empty.Right;
				array[2].Y = empty.Bottom;
				graphicsPath.AddPolygon(array);
				break;
			}
			case MarkerStyle.Wedge:
				if (empty.Width >= empty.Height)
				{
					graphicsPath = this.CreateMarker(point, markerWidth, markerHeight, MarkerStyle.Triangle);
				}
				else
				{
					float num4 = (float)Math.Pow(Math.Pow((double)empty.Width, 2.0) - Math.Pow(empty.Width / 2.0, 2.0), 0.5);
					PointF[] array = new PointF[5];
					array[0].X = empty.X;
					array[0].Y = empty.Y + num4;
					array[1].X = (float)(empty.X + empty.Width / 2.0);
					array[1].Y = empty.Y;
					array[2].X = empty.X + empty.Width;
					array[2].Y = empty.Y + num4;
					array[3].X = empty.X + empty.Width;
					array[3].Y = empty.Y + empty.Height;
					array[4].X = empty.X;
					array[4].Y = empty.Y + empty.Height;
					graphicsPath.AddPolygon(array);
				}
				break;
			case MarkerStyle.Pentagon:
			{
				float y = (float)Math.Cos(1.2566370614359172);
				float num = (float)Math.Cos(0.62831853071795862);
				float num2 = (float)Math.Sin(1.2566370614359172);
				float num3 = (float)Math.Sin(2.5132741228718345);
				PointF[] array = new PointF[5];
				array[0].X = 0f;
				array[0].Y = 1f;
				array[1].X = num2;
				array[1].Y = y;
				array[2].X = num3;
				array[2].Y = (float)(0.0 - num);
				array[3].X = (float)(0.0 - num3);
				array[3].Y = (float)(0.0 - num);
				array[4].X = (float)(0.0 - num2);
				array[4].Y = y;
				using (Matrix matrix = new Matrix())
				{
					matrix.Scale((float)(markerWidth / 2.0), (float)(markerHeight / 2.0));
					matrix.TransformPoints(array);
					matrix.Reset();
					matrix.Rotate(180f);
					matrix.TransformPoints(array);
					matrix.Reset();
					matrix.Translate(point.X, point.Y);
					matrix.TransformPoints(array);
				}
				graphicsPath.AddPolygon(array);
				break;
			}
			default:
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionInvalidMarkerType"));
			}
			return graphicsPath;
		}

		internal PointF[] CreateStarPolygon(RectangleF rectReal, int numberOfCorners)
		{
			bool flag = true;
			PointF[] array = new PointF[numberOfCorners * 2];
			PointF[] array2 = new PointF[1];
			RectangleF rectangleF = new RectangleF(0f, 0f, 1f, 1f);
			using (Matrix matrix = new Matrix())
			{
				for (int i = 0; i < numberOfCorners * 2; i++)
				{
					array2[0] = new PointF((float)(rectangleF.X + rectangleF.Width / 2.0), (float)(flag ? rectangleF.Y : (rectangleF.Y + rectangleF.Height / 4.0)));
					matrix.Reset();
					matrix.RotateAt((float)((float)i * (360.0 / ((float)numberOfCorners * 2.0))), new PointF((float)(rectangleF.X + rectangleF.Width / 2.0), (float)(rectangleF.Y + rectangleF.Height / 2.0)));
					matrix.TransformPoints(array2);
					array[i] = array2[0];
					flag = !flag;
				}
				matrix.Reset();
				matrix.Scale(rectReal.Width, rectReal.Height);
				matrix.TransformPoints(array);
				matrix.Reset();
				matrix.Translate(rectReal.X, rectReal.Y);
				matrix.TransformPoints(array);
				return array;
			}
		}

		internal void DrawPathShadowAbs(GraphicsPath path, Color shadowColor, float shadowWidth)
		{
			if (shadowWidth != 0.0)
			{
				Matrix matrix = new Matrix();
				matrix.Translate(shadowWidth, shadowWidth);
				path.Transform(matrix);
				using (Brush brush = new SolidBrush(shadowColor))
				{
					base.FillPath(brush, path);
				}
				matrix.Reset();
				matrix.Translate((float)(0.0 - shadowWidth), (float)(0.0 - shadowWidth));
				path.Transform(matrix);
			}
		}

		internal void DrawPathAbs(GraphicsPath path, Color backColor, GaugeHatchStyle backHatchStyle, string backImage, GaugeImageWrapMode backImageMode, Color backImageTranspColor, GaugeImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, GaugeDashStyle borderStyle, PenAlignment penAlignment)
		{
			Brush brush = null;
			Brush brush2 = null;
			if (backColor.IsEmpty)
			{
				backColor = Color.White;
			}
			if (backGradientEndColor.IsEmpty)
			{
				backGradientEndColor = Color.White;
			}
			if (borderColor.IsEmpty)
			{
				borderColor = Color.White;
				borderWidth = 0;
			}
			this.pen.Color = borderColor;
			this.pen.Width = (float)borderWidth;
			this.pen.Alignment = penAlignment;
			this.pen.DashStyle = this.GetPenStyle(borderStyle);
			if (backGradientType == GradientType.None)
			{
				this.solidBrush.Color = backColor;
				brush = this.solidBrush;
			}
			else
			{
				RectangleF bounds = path.GetBounds();
				bounds.Inflate(new SizeF(2f, 2f));
				brush = this.GetGradientBrush(bounds, backColor, backGradientEndColor, backGradientType);
			}
			if (backHatchStyle != 0)
			{
				brush = GaugeGraphics.GetHatchBrush(backHatchStyle, backColor, backGradientEndColor);
			}
			if (backImage.Length > 0 && backImageMode != GaugeImageWrapMode.Unscaled && backImageMode != GaugeImageWrapMode.Scaled)
			{
				brush2 = brush;
				brush = this.GetTextureBrush(backImage, backImageTranspColor, backImageMode);
			}
			RectangleF bounds2 = path.GetBounds();
			if (backImage.Length > 0 && (backImageMode == GaugeImageWrapMode.Unscaled || backImageMode == GaugeImageWrapMode.Scaled))
			{
				Image image = this.common.ImageLoader.LoadImage(backImage);
				ImageAttributes imageAttributes = new ImageAttributes();
				if (backImageTranspColor != Color.Empty)
				{
					imageAttributes.SetColorKey(backImageTranspColor, backImageTranspColor, ColorAdjustType.Default);
				}
				RectangleF rectangleF = default(RectangleF);
				rectangleF.X = bounds2.X;
				rectangleF.Y = bounds2.Y;
				rectangleF.Width = bounds2.Width;
				rectangleF.Height = bounds2.Height;
				if (backImageMode == GaugeImageWrapMode.Unscaled)
				{
					rectangleF.Width = (float)image.Width;
					rectangleF.Height = (float)image.Height;
					if (rectangleF.Width < bounds2.Width)
					{
						switch (backImageAlign)
						{
						case GaugeImageAlign.TopRight:
						case GaugeImageAlign.Right:
						case GaugeImageAlign.BottomRight:
							rectangleF.X = bounds2.Right - rectangleF.Width;
							break;
						case GaugeImageAlign.Top:
						case GaugeImageAlign.Bottom:
						case GaugeImageAlign.Center:
							rectangleF.X = (float)(bounds2.X + (bounds2.Width - rectangleF.Width) / 2.0);
							break;
						}
					}
					if (rectangleF.Height < bounds2.Height)
					{
						switch (backImageAlign)
						{
						case GaugeImageAlign.BottomRight:
						case GaugeImageAlign.Bottom:
						case GaugeImageAlign.BottomLeft:
							rectangleF.Y = bounds2.Bottom - rectangleF.Height;
							break;
						case GaugeImageAlign.Right:
						case GaugeImageAlign.Left:
						case GaugeImageAlign.Center:
							rectangleF.Y = (float)(bounds2.Y + (bounds2.Height - rectangleF.Height) / 2.0);
							break;
						}
					}
				}
				base.FillPath(brush, path);
				Region clip = base.Clip;
				base.Clip = new Region(path);
				base.DrawImage(image, new Rectangle((int)Math.Round((double)rectangleF.X), (int)Math.Round((double)rectangleF.Y), (int)Math.Round((double)rectangleF.Width), (int)Math.Round((double)rectangleF.Height)), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				base.Clip = clip;
			}
			else
			{
				if (brush2 != null && backImageTranspColor != Color.Empty)
				{
					base.FillPath(brush2, path);
				}
				base.FillPath(brush, path);
			}
			if (borderColor != Color.Empty && borderWidth > 0 && borderStyle != 0)
			{
				base.DrawPath(this.pen, path);
			}
		}

		internal Brush CreateBrush(RectangleF rect, Color backColor, GaugeHatchStyle backHatchStyle, string backImage, GaugeImageWrapMode backImageMode, Color backImageTranspColor, GaugeImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor)
		{
			Brush result = new SolidBrush(backColor);
			if (backImage.Length > 0 && backImageMode != GaugeImageWrapMode.Unscaled && backImageMode != GaugeImageWrapMode.Scaled)
			{
				result = this.GetTextureBrush(backImage, backImageTranspColor, backImageMode);
			}
			else if (backHatchStyle != 0)
			{
				result = GaugeGraphics.GetHatchBrush(backHatchStyle, backColor, backGradientEndColor);
			}
			else if (backGradientType != 0)
			{
				result = this.GetGradientBrush(rect, backColor, backGradientEndColor, backGradientType);
			}
			return result;
		}

		public PointF PixelsToPercents(PointF pointInPixels)
		{
			return this.GetRelativePoint(pointInPixels);
		}

		public PointF PercentsToPixels(PointF pointInPercents)
		{
			return this.GetAbsolutePoint(pointInPercents);
		}

		public SizeF PixelsToPercents(SizeF sizeInPixels)
		{
			return this.GetRelativeSize(sizeInPixels);
		}

		public SizeF PercentsToPixels(SizeF sizeInPercents)
		{
			return this.GetAbsoluteSize(sizeInPercents);
		}

		internal float GetRelativeX(float absoluteX)
		{
			return (float)(absoluteX * 100.0 / (float)(this.width - 1.0));
		}

		internal float GetRelativeY(float absoluteY)
		{
			return (float)(absoluteY * 100.0 / (float)(this.height - 1.0));
		}

		internal float GetRelativeWidth(float absoluteWidth)
		{
			return (float)(absoluteWidth * 100.0 / (float)(this.width - 1.0));
		}

		internal float GetRelativeHeight(float absoluteHeight)
		{
			return (float)(absoluteHeight * 100.0 / (float)(this.height - 1.0));
		}

		internal float GetAbsoluteX(float relativeX)
		{
			return (float)(relativeX * (float)(this.width - 1.0) / 100.0);
		}

		internal float GetAbsoluteY(float relativeY)
		{
			return (float)(relativeY * (float)(this.height - 1.0) / 100.0);
		}

		internal float GetAbsoluteWidth(float relativeWidth)
		{
			return (float)(relativeWidth * (float)(this.width - 1.0) / 100.0);
		}

		internal float GetAbsoluteHeight(float relativeHeight)
		{
			return (float)(relativeHeight * (float)(this.height - 1.0) / 100.0);
		}

		public RectangleF GetRelativeRectangle(RectangleF absolute)
		{
			RectangleF empty = RectangleF.Empty;
			empty.X = this.GetRelativeX(absolute.X);
			empty.Y = this.GetRelativeY(absolute.Y);
			empty.Width = this.GetRelativeWidth(absolute.Width);
			empty.Height = this.GetRelativeHeight(absolute.Height);
			return empty;
		}

		public PointF GetRelativePoint(PointF absolute)
		{
			PointF empty = PointF.Empty;
			empty.X = this.GetRelativeX(absolute.X);
			empty.Y = this.GetRelativeY(absolute.Y);
			return empty;
		}

		public SizeF GetRelativeSize(SizeF size)
		{
			SizeF empty = SizeF.Empty;
			empty.Width = this.GetRelativeWidth(size.Width);
			empty.Height = this.GetRelativeHeight(size.Height);
			return empty;
		}

		internal float GetAbsoluteDimension(float relative)
		{
			if (this.width < this.height)
			{
				return this.GetAbsoluteWidth(relative);
			}
			return this.GetAbsoluteHeight(relative);
		}

		internal float GetRelativeDimension(float absolute)
		{
			if (this.width < this.height)
			{
				return this.GetRelativeWidth(absolute);
			}
			return this.GetRelativeHeight(absolute);
		}

		public PointF GetAbsolutePoint(PointF relative)
		{
			PointF empty = PointF.Empty;
			empty.X = this.GetAbsoluteX(relative.X);
			empty.Y = this.GetAbsoluteY(relative.Y);
			return empty;
		}

		public RectangleF GetAbsoluteRectangle(RectangleF relative)
		{
			RectangleF empty = RectangleF.Empty;
			empty.X = this.GetAbsoluteX(relative.X);
			empty.Y = this.GetAbsoluteY(relative.Y);
			empty.Width = this.GetAbsoluteWidth(relative.Width);
			empty.Height = this.GetAbsoluteHeight(relative.Height);
			return empty;
		}

		public SizeF GetAbsoluteSize(SizeF relative)
		{
			SizeF empty = SizeF.Empty;
			empty.Width = this.GetAbsoluteWidth(relative.Width);
			empty.Height = this.GetAbsoluteHeight(relative.Height);
			return empty;
		}

		internal GraphicsPath CreateRoundedRectPath(RectangleF rect, float[] cornerRadius)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddLine(rect.X + cornerRadius[0], rect.Y, rect.Right - cornerRadius[1], rect.Y);
			graphicsPath.AddArc((float)(rect.Right - 2.0 * cornerRadius[1]), rect.Y, (float)(2.0 * cornerRadius[1]), (float)(2.0 * cornerRadius[2]), 270f, 90f);
			graphicsPath.AddLine(rect.Right, rect.Y + cornerRadius[2], rect.Right, rect.Bottom - cornerRadius[3]);
			graphicsPath.AddArc((float)(rect.Right - 2.0 * cornerRadius[4]), (float)(rect.Bottom - 2.0 * cornerRadius[3]), (float)(2.0 * cornerRadius[4]), (float)(2.0 * cornerRadius[3]), 0f, 90f);
			graphicsPath.AddLine(rect.Right - cornerRadius[4], rect.Bottom, rect.X + cornerRadius[5], rect.Bottom);
			graphicsPath.AddArc(rect.X, (float)(rect.Bottom - 2.0 * cornerRadius[6]), (float)(2.0 * cornerRadius[5]), (float)(2.0 * cornerRadius[6]), 90f, 90f);
			graphicsPath.AddLine(rect.X, rect.Bottom - cornerRadius[6], rect.X, rect.Y + cornerRadius[7]);
			graphicsPath.AddArc(rect.X, rect.Y, (float)(2.0 * cornerRadius[0]), (float)(2.0 * cornerRadius[7]), 180f, 90f);
			return graphicsPath;
		}

		internal Brush GetCircularRangeBrush(RectangleF rect, float startAngle, float sweepAngle, Color backColor, GaugeHatchStyle backHatchStyle, string backImage, GaugeImageWrapMode backImageMode, Color backImageTranspColor, GaugeImageAlign backImageAlign, RangeGradientType backGradientType, Color backGradientEndColor)
		{
			Brush brush = null;
			RectangleF absoluteRectangle = this.GetAbsoluteRectangle(rect);
			if (backHatchStyle != 0)
			{
				return GaugeGraphics.GetHatchBrush(backHatchStyle, backColor, backGradientEndColor);
			}
			if (!backGradientEndColor.IsEmpty)
			{
				switch (backGradientType)
				{
				case RangeGradientType.Center:
					return this.GetPieGradientBrush(absoluteRectangle, backColor, backGradientEndColor);
				case RangeGradientType.StartToEnd:
					using (GraphicsPath graphicsPath2 = new GraphicsPath())
					{
						graphicsPath2.AddPie((float)(absoluteRectangle.X - 1.0), (float)(absoluteRectangle.Y - 1.0), (float)(absoluteRectangle.Width + 2.0), (float)(absoluteRectangle.Height + 2.0), (float)(startAngle - 1.0), (float)(sweepAngle + 1.0));
						graphicsPath2.Flatten(null, 0.3f);
						PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath2);
						pathGradientBrush.SurroundColors = this.GetSurroundColors(backColor, backGradientEndColor, graphicsPath2.PointCount);
						pathGradientBrush.CenterColor = this.GetSurroundColors(backColor, backGradientEndColor, 3)[1];
						pathGradientBrush.CenterPoint = new PointF((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0), (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0));
						return pathGradientBrush;
					}
				default:
					using (GraphicsPath graphicsPath = new GraphicsPath())
					{
						graphicsPath.AddPie(absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height, startAngle, sweepAngle);
						return this.GetGradientBrush(graphicsPath.GetBounds(), backColor, backGradientEndColor, (GradientType)Enum.Parse(typeof(GradientType), backGradientType.ToString()));
					}
				case RangeGradientType.None:
					break;
				}
			}
			if (backImage.Length > 0 && backImageMode != GaugeImageWrapMode.Unscaled && backImageMode != GaugeImageWrapMode.Scaled)
			{
				return this.GetTextureBrush(backImage, backImageTranspColor, backImageMode);
			}
			return new SolidBrush(backColor);
		}

		internal GraphicsPath GetCircularRangePath(RectangleF rect, float startAngle, float sweepAngle, float startRadius, float endRadius, Placement placement)
		{
			if (rect.Width != 0.0 && rect.Height != 0.0)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				RectangleF absoluteRectangle = this.GetAbsoluteRectangle(rect);
				float num = this.GetAbsoluteDimension(startRadius);
				float num2 = this.GetAbsoluteDimension(endRadius);
				if (placement == Placement.Outside)
				{
					num = (float)(0.0 - num);
					num2 = (float)(0.0 - num2);
				}
				float num3 = (float)((!(absoluteRectangle.Width > absoluteRectangle.Height)) ? (absoluteRectangle.Width / 2.0 - 9.9999997473787516E-05) : (absoluteRectangle.Height / 2.0 - 9.9999997473787516E-05));
				if (num > num3)
				{
					num = num3;
				}
				if (num2 > num3)
				{
					num2 = num3;
				}
				if (Math.Round((double)(num - num2), 4) == 0.0)
				{
					if (placement == Placement.Cross)
					{
						absoluteRectangle.Inflate((float)(num / 2.0), (float)(num / 2.0));
					}
					if (absoluteRectangle.Width > 0.0 && absoluteRectangle.Height > 0.0)
					{
						graphicsPath.AddArc(absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height, startAngle + sweepAngle, (float)(0.0 - sweepAngle));
					}
					float num4 = (float)(absoluteRectangle.Width - num * 2.0);
					float num5 = (float)(absoluteRectangle.Height - num * 2.0);
					if (num4 > 0.0 && num5 > 0.0)
					{
						graphicsPath.AddArc(absoluteRectangle.X + num, absoluteRectangle.Y + num, num4, num5, startAngle, sweepAngle);
					}
				}
				else if (placement != Placement.Cross)
				{
					graphicsPath.AddArc(absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height, startAngle + sweepAngle, (float)(0.0 - sweepAngle));
					int num6 = (int)(Math.Abs(sweepAngle) / 5.0) + 1;
					if (num6 < 5)
					{
						num6 = 5;
					}
					float num7 = startAngle;
					float num8 = sweepAngle / (float)(num6 - 1);
					PointF[] array = new PointF[num6];
					PointF[] array2 = new PointF[1]
					{
						default(PointF)
					};
					PointF point = new PointF((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0), (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0));
					using (Matrix matrix = new Matrix())
					{
						for (int i = 0; i < num6; i++)
						{
							array2[0].X = point.X;
							array2[0].Y = absoluteRectangle.Y + absoluteRectangle.Height - num;
							array2[0].Y -= (num2 - num) * (float)i / (float)num6;
							matrix.RotateAt((float)(num7 - 90.0), point);
							matrix.TransformPoints(array2);
							matrix.Reset();
							array[i] = array2[0];
							num7 += num8;
						}
					}
					graphicsPath.AddCurve(array);
				}
				else
				{
					int num9 = (int)(Math.Abs(sweepAngle) / 5.0) + 1;
					if (num9 < 5)
					{
						num9 = 5;
					}
					float num10 = startAngle;
					float num11 = sweepAngle / (float)(num9 - 1);
					PointF[] array3 = new PointF[num9];
					PointF[] array4 = new PointF[num9];
					PointF[] array5 = new PointF[1]
					{
						default(PointF)
					};
					PointF[] array6 = new PointF[1]
					{
						default(PointF)
					};
					PointF point2 = new PointF((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0), (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0));
					using (Matrix matrix2 = new Matrix())
					{
						for (int j = 0; j < num9; j++)
						{
							array5[0].X = point2.X;
							array5[0].Y = (float)(absoluteRectangle.Y + absoluteRectangle.Height - num / 2.0);
							array5[0].Y -= (float)((num2 - num) * (float)j / (float)num9 / 2.0);
							array6[0].X = point2.X;
							array6[0].Y = (float)(absoluteRectangle.Y + absoluteRectangle.Height + num / 2.0);
							array6[0].Y += (float)((num2 - num) * (float)j / (float)num9 / 2.0);
							matrix2.RotateAt((float)(num10 - 90.0), point2);
							matrix2.TransformPoints(array5);
							matrix2.TransformPoints(array6);
							matrix2.Reset();
							array3[j] = array5[0];
							array4[j] = array6[0];
							num10 += num11;
						}
					}
					PointF[] array7 = new PointF[num9];
					for (int k = 0; k < num9; k++)
					{
						array7[k] = array4[num9 - k - 1];
					}
					graphicsPath.AddCurve(array3);
					graphicsPath.AddCurve(array7);
				}
				graphicsPath.CloseFigure();
				return graphicsPath;
			}
			return null;
		}

		internal Color[] GetSurroundColors(Color startColor, Color endColor, int colorCount)
		{
			Color[] array = new Color[colorCount];
			array[0] = startColor;
			array[colorCount - 1] = endColor;
			float num = (float)((endColor.A - startColor.A) / (colorCount - 1));
			float num2 = (float)((endColor.R - startColor.R) / (colorCount - 1));
			float num3 = (float)((endColor.G - startColor.G) / (colorCount - 1));
			float num4 = (float)((endColor.B - startColor.B) / (colorCount - 1));
			float num5 = (float)(int)startColor.A;
			float num6 = (float)(int)startColor.R;
			float num7 = (float)(int)startColor.G;
			float num8 = (float)(int)startColor.B;
			for (int i = 1; i < colorCount - 1; i++)
			{
				num5 += num;
				num6 += num2;
				num7 += num3;
				num8 += num4;
				array[i] = Color.FromArgb((int)num5, (int)num6, (int)num7, (int)num8);
			}
			return array;
		}

		internal GraphicsPath GetLinearRangePath(float startPosition, float endPosition, float startWidth, float endWidth, float scalePosition, GaugeOrientation orientation, float distanceFromScale, Placement placement, float scaleBarWidth)
		{
			PointF[] array = new PointF[4];
			array[0].X = endPosition;
			array[1].X = startPosition;
			array[2].X = startPosition;
			array[3].X = endPosition;
			switch (placement)
			{
			case Placement.Cross:
				array[0].Y = (float)(scalePosition + endWidth / 2.0 - distanceFromScale);
				array[1].Y = (float)(scalePosition + startWidth / 2.0 - distanceFromScale);
				array[2].Y = (float)(scalePosition - startWidth / 2.0 - distanceFromScale);
				array[3].Y = (float)(scalePosition - endWidth / 2.0 - distanceFromScale);
				break;
			case Placement.Inside:
				array[0].Y = (float)(scalePosition - scaleBarWidth / 2.0 - distanceFromScale);
				array[1].Y = array[0].Y;
				array[2].Y = array[0].Y - startWidth;
				array[3].Y = array[1].Y - endWidth;
				break;
			default:
				array[0].Y = (float)(scalePosition + scaleBarWidth / 2.0 + distanceFromScale);
				array[1].Y = array[0].Y;
				array[2].Y = array[0].Y + startWidth;
				array[3].Y = array[1].Y + endWidth;
				break;
			}
			if (orientation == GaugeOrientation.Vertical)
			{
				for (int i = 0; i < 4; i++)
				{
					float x = array[i].X;
					array[i].X = array[i].Y;
					array[i].Y = x;
				}
			}
			for (int j = 0; j < 4; j++)
			{
				array[j] = this.GetAbsolutePoint(array[j]);
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddLines(array);
			graphicsPath.CloseFigure();
			return graphicsPath;
		}

		internal Brush GetLinearRangeBrush(RectangleF absRect, Color backColor, GaugeHatchStyle backHatchStyle, RangeGradientType backGradientType, Color backGradientEndColor, GaugeOrientation orientation, bool reversedScale, double startValue, double endValue)
		{
			Brush brush = null;
			if (backHatchStyle != 0)
			{
				return GaugeGraphics.GetHatchBrush(backHatchStyle, backColor, backGradientEndColor);
			}
			switch (backGradientType)
			{
			case RangeGradientType.StartToEnd:
				if (orientation == GaugeOrientation.Horizontal)
				{
					backGradientType = RangeGradientType.LeftRight;
				}
				else
				{
					backGradientType = RangeGradientType.TopBottom;
					Color color = backColor;
					backColor = backGradientEndColor;
					backGradientEndColor = color;
				}
				if (startValue > endValue)
				{
					Color color2 = backColor;
					backColor = backGradientEndColor;
					backGradientEndColor = color2;
				}
				if (reversedScale)
				{
					Color color3 = backColor;
					backColor = backGradientEndColor;
					backGradientEndColor = color3;
				}
				goto default;
			default:
				return this.GetGradientBrush(absRect, backColor, backGradientEndColor, (GradientType)Enum.Parse(typeof(GradientType), backGradientType.ToString()));
			case RangeGradientType.None:
				return new SolidBrush(backColor);
			}
		}

		internal GraphicsPath GetThermometerPath(float startPosition, float endPosition, float barWidth, float scalePosition, GaugeOrientation orientation, float distanceFromScale, Placement placement, bool reversedScale, float scaleBarWidth, float bulbOffset, float bulbSize, ThermometerStyle thermometerStyle)
		{
			PointF[] array = new PointF[4];
			array[0].X = endPosition;
			array[1].X = startPosition;
			array[2].X = startPosition;
			array[3].X = endPosition;
			switch (placement)
			{
			case Placement.Cross:
				array[0].Y = (float)(scalePosition + barWidth / 2.0 - distanceFromScale);
				array[1].Y = (float)(scalePosition + barWidth / 2.0 - distanceFromScale);
				array[2].Y = (float)(scalePosition - barWidth / 2.0 - distanceFromScale);
				array[3].Y = (float)(scalePosition - barWidth / 2.0 - distanceFromScale);
				break;
			case Placement.Inside:
				array[0].Y = (float)(scalePosition - scaleBarWidth / 2.0 - distanceFromScale);
				array[1].Y = array[0].Y;
				array[2].Y = array[0].Y - barWidth;
				array[3].Y = array[1].Y - barWidth;
				break;
			default:
				array[0].Y = (float)(scalePosition + scaleBarWidth / 2.0 + distanceFromScale);
				array[1].Y = array[0].Y;
				array[2].Y = array[0].Y + barWidth;
				array[3].Y = array[1].Y + barWidth;
				break;
			}
			if (orientation == GaugeOrientation.Vertical)
			{
				for (int i = 0; i < 4; i++)
				{
					float x = array[i].X;
					array[i].X = array[i].Y;
					array[i].Y = x;
				}
			}
			for (int j = 0; j < 4; j++)
			{
				array[j] = this.GetAbsolutePoint(array[j]);
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddLines(array);
			if (bulbSize > 0.0)
			{
				float absoluteDimension = this.GetAbsoluteDimension(bulbOffset);
				float absoluteDimension2 = this.GetAbsoluteDimension(bulbSize);
				float num = (float)(absoluteDimension2 / 2.0);
				RectangleF bounds = graphicsPath.GetBounds();
				graphicsPath.Reset();
				PointF point = new PointF((float)(bounds.X + bounds.Width / 2.0), (float)(bounds.Y + bounds.Height / 2.0));
				if (orientation == GaugeOrientation.Horizontal)
				{
					bounds.Offset((float)(0.0 - absoluteDimension), 0f);
					bounds.Width += absoluteDimension;
					PointF pointF = new PointF(bounds.X - num, (float)(bounds.Y + bounds.Height / 2.0));
					RectangleF rect = new RectangleF(pointF.X, pointF.Y, 0f, 0f);
					rect.Inflate(num, num);
					float num2 = 90f;
					if (rect.Height >= bounds.Height)
					{
						num2 = (float)((float)Math.Asin(bounds.Height / 2.0 / num) * 57.2957763671875);
					}
					float sweepAngle = (float)(360.0 - num2 * 2.0);
					float x2 = num - num * (float)Math.Cos(num2 * 3.1415927410125732 / 180.0);
					rect.Offset(x2, 0f);
					if (thermometerStyle == ThermometerStyle.Flask)
					{
						num2 = 90f;
						sweepAngle = 180f;
					}
					graphicsPath.AddArc(rect, num2, sweepAngle);
					graphicsPath.AddLine(bounds.Left, bounds.Top, bounds.Right, bounds.Top);
					graphicsPath.AddLine(bounds.Right, bounds.Top, bounds.Right, bounds.Bottom);
					graphicsPath.AddLine(bounds.Right, bounds.Bottom, bounds.Left, bounds.Bottom);
				}
				else
				{
					bounds.Height += absoluteDimension;
					PointF pointF2 = new PointF((float)(bounds.X + bounds.Width / 2.0), bounds.Y + bounds.Height + num);
					RectangleF rect2 = new RectangleF(pointF2.X, pointF2.Y, 0f, 0f);
					rect2.Inflate(num, num);
					float num3 = 90f;
					if (rect2.Width >= bounds.Width)
					{
						num3 = (float)((float)Math.Asin(bounds.Width / 2.0 / num) * 57.2957763671875);
					}
					float sweepAngle2 = (float)(360.0 - num3 * 2.0);
					float num4 = num - num * (float)Math.Cos(num3 * 3.1415927410125732 / 180.0);
					rect2.Offset(0f, (float)(0.0 - num4));
					if (thermometerStyle == ThermometerStyle.Flask)
					{
						num3 = 90f;
						sweepAngle2 = 180f;
					}
					graphicsPath.AddArc(rect2, (float)(num3 - 90.0), sweepAngle2);
					graphicsPath.AddLine(bounds.Left, bounds.Bottom, bounds.Left, bounds.Top);
					graphicsPath.AddLine(bounds.Left, bounds.Top, bounds.Right, bounds.Top);
					graphicsPath.AddLine(bounds.Right, bounds.Top, bounds.Right, bounds.Bottom);
				}
				if (reversedScale)
				{
					using (Matrix matrix = new Matrix())
					{
						matrix.RotateAt(180f, point, MatrixOrder.Append);
						graphicsPath.Transform(matrix);
					}
				}
			}
			graphicsPath.CloseFigure();
			return graphicsPath;
		}

		internal void GetCircularEdgeReflection(RectangleF bounds, float angle, int alpha, PointF pointOrigin, out GraphicsPath pathResult, out Brush brushResult)
		{
			pathResult = null;
			brushResult = null;
			if (!((double)bounds.Width < 0.0001) && !((double)bounds.Height < 0.0001))
			{
				float num = 0.05f;
				float num2 = 0.05f;
				RectangleF rectangleF = bounds;
				rectangleF.Inflate((float)((0.0 - bounds.Width) * num), (float)((0.0 - bounds.Height) * num));
				RectangleF rect = rectangleF;
				rectangleF.Inflate((float)((0.0 - rectangleF.Width) * num2), rectangleF.Height * num2);
				pathResult = new GraphicsPath();
				pathResult.AddArc(rectangleF, angle, 90f);
				pathResult.AddArc(rect, (float)(angle + 90.0), -90f);
				LinearGradientBrush linearGradientBrush = new LinearGradientBrush(bounds, Color.Transparent, Color.FromArgb(alpha, Color.White), 0f);
				Blend blend = new Blend();
				blend.Positions = new float[5]
				{
					0f,
					0.1f,
					0.5f,
					0.9f,
					1f
				};
				blend.Factors = new float[5]
				{
					0f,
					0f,
					1f,
					0f,
					0f
				};
				linearGradientBrush.Blend = blend;
				linearGradientBrush.RotateTransform(135f, MatrixOrder.Append);
				linearGradientBrush.TranslateTransform(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
				brushResult = linearGradientBrush;
				using (Matrix matrix = new Matrix())
				{
					matrix.Rotate(45f, MatrixOrder.Append);
					matrix.Translate(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
					pathResult.Transform(matrix);
				}
			}
		}

		internal void SetPictureSize(float width, float height)
		{
			this.width = Math.Max(width, 2f);
			this.height = Math.Max(height, 2f);
		}

		internal void CreateDrawRegion(RectangleF rect)
		{
			this.graphicStates.Push(new GaugeGraphState(base.Save(), this.width, this.height));
			RectangleF absoluteRectangle = this.GetAbsoluteRectangle(rect);
			if (base.Transform == null)
			{
				base.Transform = new Matrix();
			}
			base.TranslateTransform(absoluteRectangle.Location.X, absoluteRectangle.Location.Y);
			this.SetPictureSize(absoluteRectangle.Size.Width, absoluteRectangle.Size.Height);
		}

		internal void RestoreDrawRegion()
		{
			GaugeGraphState gaugeGraphState = (GaugeGraphState)this.graphicStates.Pop();
			base.Restore(gaugeGraphState.state);
			this.SetPictureSize(gaugeGraphState.width, gaugeGraphState.height);
		}

		internal GaugeGraphics(CommonElements common)
		{
			this.common = common;
			common.Graph = this;
			this.pen = new Pen(Color.Black);
			this.solidBrush = new SolidBrush(Color.Black);
		}

		public override void Close()
		{
			this.common.Graph = null;
			base.Close();
		}

		internal Pen GetSelectionPen(bool designTimeSelection, Color borderColor)
		{
			Pen pen = null;
			if (designTimeSelection)
			{
				pen = new Pen(Color.Black, 1f);
				pen.DashStyle = DashStyle.Dot;
				pen.DashPattern = new float[2]
				{
					2f,
					2f
				};
				pen.Width = (float)(1.0 / this.Graphics.PageScale);
			}
			else
			{
				pen = new Pen(borderColor, 1f);
				pen.DashStyle = DashStyle.Dot;
				pen.DashPattern = new float[2]
				{
					2f,
					2f
				};
			}
			return pen;
		}

		internal Brush GetDesignTimeSelectionFillBrush()
		{
			return new SolidBrush(Color.White);
		}

		internal Pen GetDesignTimeSelectionBorderPen()
		{
			Pen pen = null;
			pen = new Pen(Color.Black);
			pen.Width = (float)(1.0 / this.Graphics.PageScale);
			return pen;
		}

		internal void DrawSelection(RectangleF rect, bool designTimeSelection, Color borderColor, Color markerColor)
		{
			this.DrawSelection(rect, (float)(3.0 / this.Graphics.PageScale), designTimeSelection, borderColor, markerColor);
		}

		internal void DrawSelection(RectangleF rect, float inflateBy, bool designTimeSelection, Color borderColor, Color markerColor)
		{
			float num = 20f;
			rect.Inflate(inflateBy, inflateBy);
			rect = RectangleF.Intersect(rect, this.Graphics.VisibleClipBounds);
			PointF pointF = new PointF((float)(rect.X + rect.Width / 2.0), (float)(rect.Y + rect.Height / 2.0));
			float num2 = (float)(6.0 / this.Graphics.PageScale);
			using (Pen pen = this.GetSelectionPen(designTimeSelection, borderColor))
			{
				base.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
			}
			PointF[] array = new PointF[8];
			array[0].X = rect.X;
			array[0].Y = rect.Y;
			array[1].X = pointF.X;
			array[1].Y = rect.Y;
			array[2].X = rect.X + rect.Width;
			array[2].Y = rect.Y;
			array[3].X = rect.X;
			array[3].Y = pointF.Y;
			array[4].X = rect.X + rect.Width;
			array[4].Y = pointF.Y;
			array[5].X = rect.X;
			array[5].Y = rect.Y + rect.Height;
			array[6].X = pointF.X;
			array[6].Y = rect.Y + rect.Height;
			array[7].X = rect.X + rect.Width;
			array[7].Y = rect.Y + rect.Height;
			Brush brush = null;
			Pen pen2 = null;
			if (designTimeSelection)
			{
				brush = this.GetDesignTimeSelectionFillBrush();
				pen2 = this.GetDesignTimeSelectionBorderPen();
			}
			else
			{
				brush = new SolidBrush(markerColor);
				pen2 = new Pen(borderColor, 1f);
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (i != 1 && i != 6)
				{
					goto IL_0267;
				}
				if (!(rect.Width < num))
				{
					goto IL_0267;
				}
				continue;
				IL_0267:
				if (i != 3 && i != 4)
				{
					goto IL_027b;
				}
				if (!(rect.Height < num))
				{
					goto IL_027b;
				}
				continue;
				IL_027b:
				base.FillEllipse(brush, new RectangleF((float)(array[i].X - num2 / 2.0), (float)(array[i].Y - num2 / 2.0), num2, num2));
				base.DrawEllipse(pen2, new RectangleF((float)(array[i].X - num2 / 2.0), (float)(array[i].Y - num2 / 2.0), num2, num2));
			}
			if (brush != null)
			{
				brush.Dispose();
			}
			if (pen2 != null)
			{
				pen2.Dispose();
			}
		}

		internal void DrawRadialSelection(GaugeGraphics g, GraphicsPath selectionPath, PointF[] markerPositions, bool designTimeSelection, Color borderColor, Color markerColor)
		{
			base.DrawPath(this.GetSelectionPen(designTimeSelection, borderColor), selectionPath);
			float num = (float)(6.0 / this.Graphics.PageScale);
			Brush brush = null;
			Pen pen = null;
			if (designTimeSelection)
			{
				brush = this.GetDesignTimeSelectionFillBrush();
				pen = this.GetDesignTimeSelectionBorderPen();
			}
			else
			{
				brush = new SolidBrush(markerColor);
				pen = new Pen(borderColor, 1f);
			}
			for (int i = 0; i < markerPositions.Length; i++)
			{
				base.FillEllipse(brush, new RectangleF((float)(markerPositions[i].X - num / 2.0), (float)(markerPositions[i].Y - num / 2.0), num, num));
				base.DrawEllipse(pen, new RectangleF((float)(markerPositions[i].X - num / 2.0), (float)(markerPositions[i].Y - num / 2.0), num, num));
			}
			if (brush != null)
			{
				brush.Dispose();
			}
			if (pen != null)
			{
				pen.Dispose();
			}
		}
	}
}
