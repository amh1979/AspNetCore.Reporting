using AspNetCore.Reporting.Chart.WebForms.Borders3D;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class ChartGraphics : ChartGraphics3D
	{
		internal CommonElements common;

		internal Pen pen;

		private SolidBrush solidBrush;

		private Matrix myMatrix;

		private int width;

		private int height;

		internal bool softShadows = true;

		private AntiAliasingTypes antiAliasing = AntiAliasingTypes.All;

		internal bool IsMetafile;

		public new Graphics Graphics
		{
			get
			{
				return base.Graphics;
			}
			set
			{
				base.Graphics = value;
			}
		}

		internal AntiAliasingTypes AntiAliasing
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
					if ((this.antiAliasing & AntiAliasingTypes.Graphics) == AntiAliasingTypes.Graphics)
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

		internal void DrawLineRel(Color color, int width, ChartDashStyle style, PointF firstPointF, PointF secondPointF)
		{
			this.DrawLineAbs(color, width, style, this.GetAbsolutePoint(firstPointF), this.GetAbsolutePoint(secondPointF));
		}

		internal void DrawLineAbs(Color color, int width, ChartDashStyle style, PointF firstPoint, PointF secondPoint)
		{
			if (width != 0 && style != 0)
			{
				if (this.pen.Color != color)
				{
					this.pen.Color = color;
				}
				if (this.pen.Width != (float)width)
				{
					this.pen.Width = (float)width;
				}
				if (this.pen.DashStyle != this.GetPenStyle(style))
				{
					this.pen.DashStyle = this.GetPenStyle(style);
				}
				SmoothingMode smoothingMode = base.SmoothingMode;
				if (width <= 1 && style != ChartDashStyle.Solid && (firstPoint.X == secondPoint.X || firstPoint.Y == secondPoint.Y))
				{
					base.SmoothingMode = SmoothingMode.Default;
				}
				base.DrawLine(this.pen, (float)Math.Round((double)firstPoint.X), (float)Math.Round((double)firstPoint.Y), (float)Math.Round((double)secondPoint.X), (float)Math.Round((double)secondPoint.Y));
				base.SmoothingMode = smoothingMode;
			}
		}

		internal void DrawLineRel(Color color, int width, ChartDashStyle style, PointF firstPoint, PointF secondPoint, Color shadowColor, int shadowOffset)
		{
			this.DrawLineAbs(color, width, style, this.GetAbsolutePoint(firstPoint), this.GetAbsolutePoint(secondPoint), shadowColor, shadowOffset);
		}

		internal void DrawLineAbs(Color color, int width, ChartDashStyle style, PointF firstPoint, PointF secondPoint, Color shadowColor, int shadowOffset)
		{
			if (shadowOffset != 0)
			{
				Color color2 = (shadowColor.A == 255) ? Color.FromArgb((int)color.A / 2, shadowColor) : shadowColor;
				PointF firstPoint2 = new PointF(firstPoint.X + (float)shadowOffset, firstPoint.Y + (float)shadowOffset);
				PointF secondPoint2 = new PointF(secondPoint.X + (float)shadowOffset, secondPoint.Y + (float)shadowOffset);
				base.shadowDrawingMode = true;
				this.DrawLineAbs(color2, width, style, firstPoint2, secondPoint2);
				base.shadowDrawingMode = false;
			}
			this.DrawLineAbs(color, width, style, firstPoint, secondPoint);
		}

		public Brush GetHatchBrush(ChartHatchStyle hatchStyle, Color backColor, Color foreColor)
		{
			HatchStyle hatchstyle = (HatchStyle)Enum.Parse(typeof(HatchStyle), hatchStyle.ToString());
			return new HatchBrush(hatchstyle, foreColor, backColor);
		}

		internal Brush GetTextureBrush(string name, Color backImageTranspColor, ChartImageWrapMode mode, Color backColor)
		{
			Image image = this.common.ImageLoader.LoadImage(name);
			ImageAttributes imageAttributes = new ImageAttributes();
			imageAttributes.SetWrapMode((WrapMode)((mode == ChartImageWrapMode.Unscaled) ? ChartImageWrapMode.Scaled : mode));
			if (backImageTranspColor != Color.Empty)
			{
				imageAttributes.SetColorKey(backImageTranspColor, backImageTranspColor, ColorAdjustType.Default);
			}
			if (backImageTranspColor == Color.Empty && image is Metafile && backColor != Color.Transparent)
			{
				TextureBrush textureBrush = null;
				Bitmap image2 = new Bitmap(image.Width, image.Height);
				using (Graphics graphics = Graphics.FromImage(image2))
				{
					using (SolidBrush brush = new SolidBrush(backColor))
					{
						graphics.FillRectangle(brush, 0, 0, image.Width, image.Height);
						graphics.DrawImageUnscaled(image, 0, 0);
						return new TextureBrush(image2, new RectangleF(0f, 0f, (float)image.Width, (float)image.Height), imageAttributes);
					}
				}
			}
			TextureBrush result;
			if (ImageLoader.DoDpisMatch(image, this.Graphics))
			{
				result = new TextureBrush(image, new RectangleF(0f, 0f, (float)image.Width, (float)image.Height), imageAttributes);
			}
			else
			{
				Image scaledImage = ImageLoader.GetScaledImage(image, this.Graphics);
				result = new TextureBrush(scaledImage, new RectangleF(0f, 0f, (float)scaledImage.Width, (float)scaledImage.Height), imageAttributes);
				scaledImage.Dispose();
			}
			return result;
		}

		public Brush GetGradientBrush(RectangleF rectangle, Color firstColor, Color secondColor, GradientType type)
		{
			base.SetGradient(firstColor, secondColor, type);
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

		internal DashStyle GetPenStyle(ChartDashStyle style)
		{
			switch (style)
			{
			case ChartDashStyle.Dash:
				return DashStyle.Dash;
			case ChartDashStyle.DashDot:
				return DashStyle.DashDot;
			case ChartDashStyle.DashDotDot:
				return DashStyle.DashDotDot;
			case ChartDashStyle.Dot:
				return DashStyle.Dot;
			default:
				return DashStyle.Solid;
			}
		}

		public PointF[] CreateStarPolygon(RectangleF rect, int numberOfCorners)
		{
			int num = checked(numberOfCorners * 2);
			bool flag = true;
			PointF[] array = new PointF[num];
			PointF[] array2 = new PointF[1];
			for (int i = 0; i < num; i++)
			{
				array2[0] = new PointF((float)(rect.X + rect.Width / 2.0), (float)(flag ? rect.Y : (rect.Y + rect.Height / 4.0)));
				Matrix matrix = new Matrix();
				matrix.RotateAt((float)((float)i * (360.0 / ((float)numberOfCorners * 2.0))), new PointF((float)(rect.X + rect.Width / 2.0), (float)(rect.Y + rect.Height / 2.0)));
				matrix.TransformPoints(array2);
				array[i] = array2[0];
				flag = !flag;
			}
			return array;
		}

		internal void DrawMarkerRel(PointF point, MarkerStyle markerStyle, int markerSize, Color markerColor, Color markerBorderColor, int markerBorderSize, string markerImage, Color markerImageTranspColor, int shadowSize, Color shadowColor, RectangleF imageScaleRect)
		{
			this.DrawMarkerAbs(this.GetAbsolutePoint(point), markerStyle, markerSize, markerColor, markerBorderColor, markerBorderSize, markerImage, markerImageTranspColor, shadowSize, shadowColor, imageScaleRect, false);
		}

		internal void DrawMarkerAbs(PointF point, MarkerStyle markerStyle, int markerSize, Color markerColor, Color markerBorderColor, int markerBorderSize, string markerImage, Color markerImageTranspColor, int shadowSize, Color shadowColor, RectangleF imageScaleRect, bool forceAntiAlias)
		{
			if (markerBorderSize <= 0)
			{
				markerBorderColor = Color.Transparent;
			}
			if (markerImage.Length > 0)
			{
				Image image = this.common.ImageLoader.LoadImage(markerImage);
				RectangleF empty = RectangleF.Empty;
				if (imageScaleRect == RectangleF.Empty)
				{
					SizeF sizeF = default(SizeF);
					ImageLoader.GetAdjustedImageSize(image, this.Graphics, ref sizeF);
					imageScaleRect.Width = sizeF.Width;
					imageScaleRect.Height = sizeF.Height;
				}
				empty.X = (float)(point.X - imageScaleRect.Width / 2.0);
				empty.Y = (float)(point.Y - imageScaleRect.Height / 2.0);
				empty.Width = imageScaleRect.Width;
				empty.Height = imageScaleRect.Height;
				ImageAttributes imageAttributes = new ImageAttributes();
				if (markerImageTranspColor != Color.Empty)
				{
					imageAttributes.SetColorKey(markerImageTranspColor, markerImageTranspColor, ColorAdjustType.Default);
				}
				if (shadowSize != 0 && shadowColor != Color.Empty)
				{
					ImageAttributes imageAttributes2 = new ImageAttributes();
					imageAttributes2.SetColorKey(markerImageTranspColor, markerImageTranspColor, ColorAdjustType.Default);
					ColorMatrix colorMatrix = new ColorMatrix();
					colorMatrix.Matrix00 = 0.25f;
					colorMatrix.Matrix11 = 0.25f;
					colorMatrix.Matrix22 = 0.25f;
					colorMatrix.Matrix33 = 0.5f;
					colorMatrix.Matrix44 = 1f;
					imageAttributes2.SetColorMatrix(colorMatrix);
					base.shadowDrawingMode = true;
					base.DrawImage(image, new Rectangle((int)empty.X + shadowSize, (int)empty.Y + shadowSize, (int)empty.Width, (int)empty.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes2);
					base.shadowDrawingMode = false;
				}
				base.DrawImage(image, new Rectangle((int)empty.X, (int)empty.Y, (int)empty.Width, (int)empty.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
			}
			else if (markerStyle != 0 && markerSize > 0 && markerColor != Color.Empty)
			{
				SmoothingMode smoothingMode = base.SmoothingMode;
				if (forceAntiAlias)
				{
					base.SmoothingMode = SmoothingMode.AntiAlias;
				}
				SolidBrush brush = new SolidBrush(markerColor);
				RectangleF empty2 = RectangleF.Empty;
				empty2.X = (float)(point.X - (float)markerSize / 2.0);
				empty2.Y = (float)(point.Y - (float)markerSize / 2.0);
				empty2.Width = (float)markerSize;
				empty2.Height = (float)markerSize;
				switch (markerStyle)
				{
				case MarkerStyle.Star4:
				case MarkerStyle.Star5:
				case MarkerStyle.Star6:
				case MarkerStyle.Star10:
				{
					int numberOfCorners = 4;
					switch (markerStyle)
					{
					case MarkerStyle.Star5:
						numberOfCorners = 5;
						break;
					case MarkerStyle.Star6:
						numberOfCorners = 6;
						break;
					case MarkerStyle.Star10:
						numberOfCorners = 10;
						break;
					}
					PointF[] points = this.CreateStarPolygon(empty2, numberOfCorners);
					if (shadowSize != 0 && shadowColor != Color.Empty)
					{
						Matrix matrix4 = base.Transform.Clone();
						matrix4.Translate((float)shadowSize, (float)shadowSize);
						Matrix transform5 = base.Transform;
						base.Transform = matrix4;
						base.shadowDrawingMode = true;
						base.FillPolygon(new SolidBrush((shadowColor.A != 255) ? shadowColor : Color.FromArgb((int)markerColor.A / 2, shadowColor)), points);
						base.shadowDrawingMode = false;
						base.Transform = transform5;
					}
					base.FillPolygon(brush, points);
					base.DrawPolygon(new Pen(markerBorderColor, (float)markerBorderSize), points);
					break;
				}
				case MarkerStyle.Circle:
					if (shadowSize != 0 && shadowColor != Color.Empty)
					{
						base.shadowDrawingMode = true;
						if (!this.softShadows)
						{
							SolidBrush brush2 = new SolidBrush((shadowColor.A != 255) ? shadowColor : Color.FromArgb((int)markerColor.A / 2, shadowColor));
							RectangleF rect = empty2;
							rect.X += (float)shadowSize;
							rect.Y += (float)shadowSize;
							base.FillEllipse(brush2, rect);
						}
						else
						{
							GraphicsPath graphicsPath3 = new GraphicsPath();
							graphicsPath3.AddEllipse((float)(empty2.X + (float)shadowSize - 1.0), (float)(empty2.Y + (float)shadowSize - 1.0), (float)(empty2.Width + 2.0), (float)(empty2.Height + 2.0));
							PathGradientBrush pathGradientBrush3 = new PathGradientBrush(graphicsPath3);
							pathGradientBrush3.CenterColor = shadowColor;
							Color[] array9 = new Color[1]
							{
								Color.Transparent
							};
							Color[] array11 = pathGradientBrush3.SurroundColors = array9;
							pathGradientBrush3.CenterPoint = new PointF(point.X, point.Y);
							PointF focusScales3 = new PointF((float)(1.0 - 2.0 * (float)shadowSize / empty2.Width), (float)(1.0 - 2.0 * (float)shadowSize / empty2.Height));
							if (focusScales3.X < 0.0)
							{
								focusScales3.X = 0f;
							}
							if (focusScales3.Y < 0.0)
							{
								focusScales3.Y = 0f;
							}
							pathGradientBrush3.FocusScales = focusScales3;
							base.FillPath(pathGradientBrush3, graphicsPath3);
						}
						base.shadowDrawingMode = false;
					}
					base.FillEllipse(brush, empty2);
					base.DrawEllipse(new Pen(markerBorderColor, (float)markerBorderSize), empty2);
					break;
				case MarkerStyle.Square:
					if (shadowSize != 0 && shadowColor != Color.Empty)
					{
						base.shadowDrawingMode = true;
						this.FillRectangleShadowAbs(empty2, shadowColor, (float)shadowSize, shadowColor);
						base.shadowDrawingMode = false;
					}
					base.FillRectangle(brush, empty2);
					base.DrawRectangle(new Pen(markerBorderColor, (float)markerBorderSize), (int)Math.Round((double)empty2.X, 0), (int)Math.Round((double)empty2.Y, 0), (int)Math.Round((double)empty2.Width, 0), (int)Math.Round((double)empty2.Height, 0));
					break;
				case MarkerStyle.Cross:
				{
					float num = (float)Math.Ceiling((float)markerSize / 4.0);
					float num2 = (float)markerSize;
					PointF[] array5 = new PointF[12];
					array5[0].X = (float)(point.X - num2 / 2.0);
					array5[0].Y = (float)(point.Y + num / 2.0);
					array5[1].X = (float)(point.X - num2 / 2.0);
					array5[1].Y = (float)(point.Y - num / 2.0);
					array5[2].X = (float)(point.X - num / 2.0);
					array5[2].Y = (float)(point.Y - num / 2.0);
					array5[3].X = (float)(point.X - num / 2.0);
					array5[3].Y = (float)(point.Y - num2 / 2.0);
					array5[4].X = (float)(point.X + num / 2.0);
					array5[4].Y = (float)(point.Y - num2 / 2.0);
					array5[5].X = (float)(point.X + num / 2.0);
					array5[5].Y = (float)(point.Y - num / 2.0);
					array5[6].X = (float)(point.X + num2 / 2.0);
					array5[6].Y = (float)(point.Y - num / 2.0);
					array5[7].X = (float)(point.X + num2 / 2.0);
					array5[7].Y = (float)(point.Y + num / 2.0);
					array5[8].X = (float)(point.X + num / 2.0);
					array5[8].Y = (float)(point.Y + num / 2.0);
					array5[9].X = (float)(point.X + num / 2.0);
					array5[9].Y = (float)(point.Y + num2 / 2.0);
					array5[10].X = (float)(point.X - num / 2.0);
					array5[10].Y = (float)(point.Y + num2 / 2.0);
					array5[11].X = (float)(point.X - num / 2.0);
					array5[11].Y = (float)(point.Y + num / 2.0);
					Matrix matrix2 = new Matrix();
					matrix2.RotateAt(45f, point);
					matrix2.TransformPoints(array5);
					if (shadowSize != 0 && shadowColor != Color.Empty)
					{
						base.shadowDrawingMode = true;
						Matrix matrix3 = base.Transform.Clone();
						matrix3.Translate((float)(this.softShadows ? (shadowSize + 1) : shadowSize), (float)(this.softShadows ? (shadowSize + 1) : shadowSize));
						Matrix transform2 = base.Transform;
						base.Transform = matrix3;
						if (!this.softShadows)
						{
							base.FillPolygon(new SolidBrush((shadowColor.A != 255) ? shadowColor : Color.FromArgb((int)markerColor.A / 2, shadowColor)), array5);
						}
						else
						{
							GraphicsPath graphicsPath2 = new GraphicsPath();
							graphicsPath2.AddPolygon(array5);
							PathGradientBrush pathGradientBrush2 = new PathGradientBrush(graphicsPath2);
							pathGradientBrush2.CenterColor = shadowColor;
							Color[] array6 = new Color[1]
							{
								Color.Transparent
							};
							Color[] array8 = pathGradientBrush2.SurroundColors = array6;
							pathGradientBrush2.CenterPoint = new PointF(point.X, point.Y);
							PointF focusScales2 = new PointF((float)(1.0 - 2.0 * (float)shadowSize / empty2.Width), (float)(1.0 - 2.0 * (float)shadowSize / empty2.Height));
							if (focusScales2.X < 0.0)
							{
								focusScales2.X = 0f;
							}
							if (focusScales2.Y < 0.0)
							{
								focusScales2.Y = 0f;
							}
							pathGradientBrush2.FocusScales = focusScales2;
							base.FillPath(pathGradientBrush2, graphicsPath2);
						}
						base.Transform = transform2;
						base.shadowDrawingMode = false;
					}
					Matrix transform3 = base.Transform.Clone();
					Matrix transform4 = base.Transform;
					base.Transform = transform3;
					base.FillPolygon(brush, array5);
					base.DrawPolygon(new Pen(markerBorderColor, (float)markerBorderSize), array5);
					base.Transform = transform4;
					break;
				}
				case MarkerStyle.Diamond:
				{
					PointF[] array12 = new PointF[4];
					array12[0].X = empty2.X;
					array12[0].Y = (float)(empty2.Y + empty2.Height / 2.0);
					array12[1].X = (float)(empty2.X + empty2.Width / 2.0);
					array12[1].Y = empty2.Top;
					array12[2].X = empty2.Right;
					array12[2].Y = (float)(empty2.Y + empty2.Height / 2.0);
					array12[3].X = (float)(empty2.X + empty2.Width / 2.0);
					array12[3].Y = empty2.Bottom;
					if (shadowSize != 0 && shadowColor != Color.Empty)
					{
						base.shadowDrawingMode = true;
						Matrix matrix5 = base.Transform.Clone();
						matrix5.Translate((float)((!this.softShadows) ? shadowSize : 0), (float)((!this.softShadows) ? shadowSize : 0));
						Matrix transform6 = base.Transform;
						base.Transform = matrix5;
						if (!this.softShadows)
						{
							base.FillPolygon(new SolidBrush((shadowColor.A != 255) ? shadowColor : Color.FromArgb((int)markerColor.A / 2, shadowColor)), array12);
						}
						else
						{
							float num3 = (float)markerSize * (float)Math.Sin(0.78539816339744828);
							RectangleF empty3 = RectangleF.Empty;
							empty3.X = (float)(point.X - num3 / 2.0);
							empty3.Y = (float)(point.Y - num3 / 2.0 - (float)shadowSize);
							empty3.Width = num3;
							empty3.Height = num3;
							matrix5.RotateAt(45f, point);
							base.Transform = matrix5;
							this.FillRectangleShadowAbs(empty3, shadowColor, (float)shadowSize, shadowColor);
						}
						base.Transform = transform6;
						base.shadowDrawingMode = false;
					}
					base.FillPolygon(brush, array12);
					base.DrawPolygon(new Pen(markerBorderColor, (float)markerBorderSize), array12);
					break;
				}
				case MarkerStyle.Triangle:
				{
					PointF[] array = new PointF[3];
					array[0].X = empty2.X;
					array[0].Y = empty2.Bottom;
					array[1].X = (float)(empty2.X + empty2.Width / 2.0);
					array[1].Y = empty2.Top;
					array[2].X = empty2.Right;
					array[2].Y = empty2.Bottom;
					if (shadowSize != 0 && shadowColor != Color.Empty)
					{
						base.shadowDrawingMode = true;
						Matrix matrix = base.Transform.Clone();
						matrix.Translate((float)(this.softShadows ? (shadowSize - 1) : shadowSize), (float)(this.softShadows ? (shadowSize + 1) : shadowSize));
						Matrix transform = base.Transform;
						base.Transform = matrix;
						if (!this.softShadows)
						{
							base.FillPolygon(new SolidBrush((shadowColor.A != 255) ? shadowColor : Color.FromArgb((int)markerColor.A / 2, shadowColor)), array);
						}
						else
						{
							GraphicsPath graphicsPath = new GraphicsPath();
							graphicsPath.AddPolygon(array);
							PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
							pathGradientBrush.CenterColor = shadowColor;
							Color[] array2 = new Color[1]
							{
								Color.Transparent
							};
							Color[] array4 = pathGradientBrush.SurroundColors = array2;
							pathGradientBrush.CenterPoint = new PointF(point.X, point.Y);
							PointF focusScales = new PointF((float)(1.0 - 2.0 * (float)shadowSize / empty2.Width), (float)(1.0 - 2.0 * (float)shadowSize / empty2.Height));
							if (focusScales.X < 0.0)
							{
								focusScales.X = 0f;
							}
							if (focusScales.Y < 0.0)
							{
								focusScales.Y = 0f;
							}
							pathGradientBrush.FocusScales = focusScales;
							base.FillPath(pathGradientBrush, graphicsPath);
						}
						base.Transform = transform;
						base.shadowDrawingMode = false;
					}
					base.FillPolygon(brush, array);
					base.DrawPolygon(new Pen(markerBorderColor, (float)markerBorderSize), array);
					break;
				}
				default:
					throw new InvalidOperationException(SR.ExceptionGraphicsMarkerStyleUnknown);
				}
				if (forceAntiAlias)
				{
					base.SmoothingMode = smoothingMode;
				}
			}
		}

		public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, TextOrientation textOrientation)
		{
			if (textOrientation == TextOrientation.Stacked)
			{
				text = ChartGraphics.GetStackedText(text);
			}
			return base.MeasureString(text, font, layoutArea, stringFormat);
		}

		internal SizeF MeasureStringRel(string text, Font font, SizeF layoutArea, StringFormat stringFormat, TextOrientation textOrientation)
		{
			if (textOrientation == TextOrientation.Stacked)
			{
				text = ChartGraphics.GetStackedText(text);
			}
			return this.MeasureStringRel(text, font, layoutArea, stringFormat);
		}

		public void DrawString(string text, Font font, Brush brush, RectangleF rect, StringFormat format, TextOrientation textOrientation)
		{
			if (textOrientation == TextOrientation.Stacked)
			{
				text = ChartGraphics.GetStackedText(text);
			}
			base.DrawString(text, font, brush, rect, format);
		}

		internal void DrawStringRel(string text, Font font, Brush brush, PointF position, StringFormat format, int angle, TextOrientation textOrientation)
		{
			if (textOrientation == TextOrientation.Stacked)
			{
				text = ChartGraphics.GetStackedText(text);
			}
			this.DrawStringRel(text, font, brush, position, format, angle);
		}

		internal void DrawStringRel(string text, Font font, Brush brush, RectangleF position, StringFormat format, TextOrientation textOrientation)
		{
			if (textOrientation == TextOrientation.Stacked)
			{
				text = ChartGraphics.GetStackedText(text);
			}
			this.DrawStringRel(text, font, brush, position, format);
		}

		internal static string GetStackedText(string text)
		{
			string text2 = string.Empty;
			foreach (char c in text)
			{
				text2 += c;
				if (c != '\n')
				{
					text2 += '\n';
				}
			}
			return text2;
		}

		internal void DrawPointLabelStringRel(CommonElements common, string text, Font font, Brush brush, RectangleF position, StringFormat format, int angle, RectangleF backPosition, Color backColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, Series series, DataPoint point, int pointIndex)
		{
			base.StartHotRegion(point, true);
			this.DrawPointLabelBackground(common, angle, PointF.Empty, backPosition, backColor, borderColor, borderWidth, borderStyle, series, point, pointIndex);
			base.EndHotRegion();
			this.DrawStringRel(text, font, brush, position, format, angle);
		}

		internal void DrawPointLabelStringRel(CommonElements common, string text, Font font, Brush brush, PointF position, StringFormat format, int angle, RectangleF backPosition, Color backColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, Series series, DataPoint point, int pointIndex)
		{
			base.StartHotRegion(point, true);
			this.DrawPointLabelBackground(common, angle, position, backPosition, backColor, borderColor, borderWidth, borderStyle, series, point, pointIndex);
			base.EndHotRegion();
			this.DrawStringRel(text, font, brush, position, format, angle);
		}

		private void DrawPointLabelBackground(CommonElements common, int angle, PointF textPosition, RectangleF backPosition, Color backColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, Series series, DataPoint point, int pointIndex)
		{
			if (!backPosition.IsEmpty)
			{
				RectangleF rect = this.Round(this.GetAbsoluteRectangle(backPosition));
				PointF empty = PointF.Empty;
				empty = ((!textPosition.IsEmpty) ? this.GetAbsolutePoint(textPosition) : new PointF((float)(rect.X + rect.Width / 2.0), (float)(rect.Y + rect.Height / 2.0)));
				this.myMatrix = base.Transform.Clone();
				this.myMatrix.RotateAt((float)angle, empty);
				GraphicsState gstate = base.Save();
				base.Transform = this.myMatrix;
				if (!backColor.IsEmpty || !borderColor.IsEmpty)
				{
					using (Brush brush = new SolidBrush(backColor))
					{
						base.FillRectangle(brush, rect);
					}
					if (borderWidth > 0 && !borderColor.IsEmpty && borderStyle != 0)
					{
						AntiAliasingTypes antiAliasingTypes = this.AntiAliasing;
						try
						{
							this.AntiAliasing = AntiAliasingTypes.None;
							using (Pen pen = new Pen(borderColor, (float)borderWidth))
							{
								pen.DashStyle = this.GetPenStyle(borderStyle);
								base.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
							}
						}
						finally
						{
							this.AntiAliasing = antiAliasingTypes;
						}
					}
				}
				else
				{
					using (Brush brush2 = new SolidBrush(Color.Transparent))
					{
						base.FillRectangle(brush2, rect);
					}
				}
				base.Restore(gstate);
				if (common != null && common.ProcessModeRegions)
				{
					common.HotRegionsList.FindInsertIndex();
					string toolTip = point.ToolTip;
					string href = point.Href;
					string mapAreaAttributes = point.MapAreaAttributes;
					object tag = ((IMapAreaAttributes)point).Tag;
					point.ToolTip = point.LabelToolTip;
					point.Href = point.LabelHref;
					point.MapAreaAttributes = point.LabelMapAreaAttributes;
					((IMapAreaAttributes)point).Tag = point.LabelTag;
					if (angle == 0)
					{
						common.HotRegionsList.AddHotRegion(this, backPosition, point, series.Name, pointIndex);
					}
					else
					{
						GraphicsPath graphicsPath = new GraphicsPath();
						graphicsPath.AddRectangle(rect);
						graphicsPath.Transform(this.myMatrix);
						common.HotRegionsList.AddHotRegion(graphicsPath, false, this, point, series.Name, pointIndex);
					}
					point.ToolTip = toolTip;
					point.Href = href;
					point.MapAreaAttributes = mapAreaAttributes;
					((IMapAreaAttributes)point).Tag = tag;
					if (common.HotRegionsList.List != null)
					{
						((HotRegion)common.HotRegionsList.List[common.HotRegionsList.List.Count - 1]).Type = ChartElementType.DataPointLabel;
					}
				}
			}
		}

		internal void DrawStringRel(string text, Font font, Brush brush, PointF position, StringFormat format, int angle)
		{
			this.DrawStringAbs(text, font, brush, this.GetAbsolutePoint(position), format, angle);
		}

		internal void DrawStringAbs(string text, Font font, Brush brush, PointF absPosition, StringFormat format, int angle)
		{
			this.myMatrix = base.Transform.Clone();
			this.myMatrix.RotateAt((float)angle, absPosition);
			GraphicsState gstate = base.Save();
			base.Transform = this.myMatrix;
			base.DrawString(text, font, brush, absPosition, format);
			base.Restore(gstate);
		}

		internal GraphicsPath GetTranformedTextRectPath(PointF center, SizeF size, int angle)
		{
			size.Width += 10f;
			size.Height += 10f;
			PointF absolutePoint = this.GetAbsolutePoint(center);
			PointF[] array = new PointF[4]
			{
				new PointF((float)(absolutePoint.X - size.Width / 2.0), (float)(absolutePoint.Y - size.Height / 2.0)),
				new PointF((float)(absolutePoint.X + size.Width / 2.0), (float)(absolutePoint.Y - size.Height / 2.0)),
				new PointF((float)(absolutePoint.X + size.Width / 2.0), (float)(absolutePoint.Y + size.Height / 2.0)),
				new PointF((float)(absolutePoint.X - size.Width / 2.0), (float)(absolutePoint.Y + size.Height / 2.0))
			};
			Matrix matrix = base.Transform.Clone();
			matrix.RotateAt((float)angle, absolutePoint);
			matrix.TransformPoints(array);
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddLines(array);
			graphicsPath.CloseAllFigures();
			return graphicsPath;
		}

		internal void DrawLabelStringRel(Axis axis, int labelRowIndex, LabelMark labelMark, Color markColor, string text, string image, Color imageTranspColor, Font font, Brush brush, RectangleF position, StringFormat format, int angle, RectangleF boundaryRect, CustomLabel label, bool truncatedLeft, bool truncatedRight)
		{
			StringFormat stringFormat = (StringFormat)format.Clone();
			SizeF sizeF = SizeF.Empty;
			if (position.Width != 0.0 && position.Height != 0.0)
			{
				RectangleF rectangleF = this.GetAbsoluteRectangle(position);
				if (rectangleF.Width < 1.0)
				{
					rectangleF.Width = 1f;
				}
				if (rectangleF.Height < 1.0)
				{
					rectangleF.Height = 1f;
				}
				CommonElements commonElements = axis.Common;
				if (commonElements.ProcessModeRegions)
				{
					commonElements.HotRegionsList.AddHotRegion(Rectangle.Round(rectangleF), label, ChartElementType.AxisLabels, false, true);
				}
				if (labelRowIndex > 0)
				{
					stringFormat.LineAlignment = StringAlignment.Center;
					stringFormat.Alignment = StringAlignment.Center;
					angle = 0;
					if (axis.AxisPosition == AxisPosition.Left)
					{
						angle = -90;
					}
					else if (axis.AxisPosition == AxisPosition.Right)
					{
						angle = 90;
					}
					else if (axis.AxisPosition != AxisPosition.Top)
					{
						AxisPosition axisPosition = axis.AxisPosition;
					}
				}
				PointF empty = PointF.Empty;
				if (axis.AxisPosition == AxisPosition.Left)
				{
					empty.X = rectangleF.Right;
					empty.Y = (float)(rectangleF.Y + rectangleF.Height / 2.0);
				}
				else if (axis.AxisPosition == AxisPosition.Right)
				{
					empty.X = rectangleF.Left;
					empty.Y = (float)(rectangleF.Y + rectangleF.Height / 2.0);
				}
				else if (axis.AxisPosition == AxisPosition.Top)
				{
					empty.X = (float)(rectangleF.X + rectangleF.Width / 2.0);
					empty.Y = rectangleF.Bottom;
				}
				else if (axis.AxisPosition == AxisPosition.Bottom)
				{
					empty.X = (float)(rectangleF.X + rectangleF.Width / 2.0);
					empty.Y = rectangleF.Top;
				}
				if ((axis.AxisPosition == AxisPosition.Top || axis.AxisPosition == AxisPosition.Bottom) && angle != 0)
				{
					empty.X = (float)(rectangleF.X + rectangleF.Width / 2.0);
					empty.Y = ((axis.AxisPosition == AxisPosition.Top) ? rectangleF.Bottom : rectangleF.Y);
					RectangleF empty2 = RectangleF.Empty;
					empty2.X = (float)(rectangleF.X + rectangleF.Width / 2.0);
					empty2.Y = (float)(rectangleF.Y - rectangleF.Width / 2.0);
					empty2.Height = rectangleF.Width;
					empty2.Width = rectangleF.Height;
					if (axis.AxisPosition == AxisPosition.Bottom)
					{
						if (angle < 0)
						{
							empty2.X -= empty2.Width;
						}
						stringFormat.Alignment = StringAlignment.Near;
						if (angle < 0)
						{
							stringFormat.Alignment = StringAlignment.Far;
						}
						stringFormat.LineAlignment = StringAlignment.Center;
					}
					if (axis.AxisPosition == AxisPosition.Top)
					{
						empty2.Y += rectangleF.Height;
						if (angle > 0)
						{
							empty2.X -= empty2.Width;
						}
						stringFormat.Alignment = StringAlignment.Far;
						if (angle < 0)
						{
							stringFormat.Alignment = StringAlignment.Near;
						}
						stringFormat.LineAlignment = StringAlignment.Center;
					}
					rectangleF = empty2;
				}
				if ((axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right) && (angle == 90 || angle == -90))
				{
					empty.X = (float)(rectangleF.X + rectangleF.Width / 2.0);
					empty.Y = (float)(rectangleF.Y + rectangleF.Height / 2.0);
					RectangleF empty3 = RectangleF.Empty;
					empty3.X = (float)(empty.X - rectangleF.Height / 2.0);
					empty3.Y = (float)(empty.Y - rectangleF.Width / 2.0);
					empty3.Height = rectangleF.Width;
					empty3.Width = rectangleF.Height;
					rectangleF = empty3;
					StringAlignment alignment = stringFormat.Alignment;
					stringFormat.Alignment = stringFormat.LineAlignment;
					stringFormat.LineAlignment = alignment;
					if (angle == 90)
					{
						if (stringFormat.LineAlignment == StringAlignment.Far)
						{
							stringFormat.LineAlignment = StringAlignment.Near;
						}
						else if (stringFormat.LineAlignment == StringAlignment.Near)
						{
							stringFormat.LineAlignment = StringAlignment.Far;
						}
					}
					if (angle == -90)
					{
						if (stringFormat.Alignment == StringAlignment.Far)
						{
							stringFormat.Alignment = StringAlignment.Near;
						}
						else if (stringFormat.Alignment == StringAlignment.Near)
						{
							stringFormat.Alignment = StringAlignment.Far;
						}
					}
				}
				Matrix matrix = null;
				if (angle != 0)
				{
					this.myMatrix = base.Transform.Clone();
					this.myMatrix.RotateAt((float)angle, empty);
					matrix = base.Transform;
					base.Transform = this.myMatrix;
				}
				RectangleF rect = Rectangle.Empty;
				float num = 0f;
				float num2 = 0f;
				if (angle != 0 && angle != 90 && angle != -90)
				{
					sizeF = base.MeasureString(text.Replace("\\n", "\n"), font, rectangleF.Size, stringFormat);
					rect.Width = sizeF.Width;
					rect.Height = sizeF.Height;
					if (stringFormat.Alignment == StringAlignment.Far)
					{
						rect.X = rectangleF.Right - sizeF.Width;
					}
					else if (stringFormat.Alignment == StringAlignment.Near)
					{
						rect.X = rectangleF.X;
					}
					else if (stringFormat.Alignment == StringAlignment.Center)
					{
						rect.X = (float)(rectangleF.X + rectangleF.Width / 2.0 - sizeF.Width / 2.0);
					}
					if (stringFormat.LineAlignment == StringAlignment.Far)
					{
						rect.Y = rectangleF.Bottom - sizeF.Height;
					}
					else if (stringFormat.LineAlignment == StringAlignment.Near)
					{
						rect.Y = rectangleF.Y;
					}
					else if (stringFormat.LineAlignment == StringAlignment.Center)
					{
						rect.Y = (float)(rectangleF.Y + rectangleF.Height / 2.0 - sizeF.Height / 2.0);
					}
					num = (float)((float)Math.Sin((float)(90 - angle) / 180.0 * 3.1415926535897931) * rect.Height / 2.0);
					num2 = (float)((float)Math.Sin((float)Math.Abs(angle) / 180.0 * 3.1415926535897931) * rect.Height / 2.0);
					if (axis.AxisPosition == AxisPosition.Left)
					{
						this.myMatrix.Translate((float)(0.0 - num2), 0f);
					}
					else if (axis.AxisPosition == AxisPosition.Right)
					{
						this.myMatrix.Translate(num2, 0f);
					}
					else if (axis.AxisPosition == AxisPosition.Top)
					{
						this.myMatrix.Translate(0f, (float)(0.0 - num));
					}
					else if (axis.AxisPosition == AxisPosition.Bottom)
					{
						this.myMatrix.Translate(0f, num);
					}
					if (boundaryRect != RectangleF.Empty)
					{
						Region region = new Region(rect);
						region.Transform(this.myMatrix);
						if (axis.AxisPosition == AxisPosition.Left)
						{
							boundaryRect.Width += boundaryRect.X;
							boundaryRect.X = 0f;
						}
						else if (axis.AxisPosition == AxisPosition.Right)
						{
							boundaryRect.Width = (float)this.common.Width - boundaryRect.X;
						}
						else if (axis.AxisPosition == AxisPosition.Top)
						{
							boundaryRect.Height += boundaryRect.Y;
							boundaryRect.Y = 0f;
						}
						else if (axis.AxisPosition == AxisPosition.Bottom)
						{
							boundaryRect.Height = (float)this.common.Height - boundaryRect.Y;
						}
						region.Exclude(this.GetAbsoluteRectangle(boundaryRect));
						if (!region.IsEmpty(this.Graphics))
						{
							base.Transform = matrix;
							float num3 = region.GetBounds(this.Graphics).Width / (float)Math.Cos((float)Math.Abs(angle) / 180.0 * 3.1415926535897931);
							if (axis.AxisPosition == AxisPosition.Left)
							{
								num3 -= rect.Height * (float)Math.Tan((float)Math.Abs(angle) / 180.0 * 3.1415926535897931);
								rectangleF.Y = rect.Y;
								rectangleF.X = rect.X + num3;
								rectangleF.Width = rect.Width - num3;
								rectangleF.Height = rect.Height;
							}
							else if (axis.AxisPosition == AxisPosition.Right)
							{
								num3 -= rect.Height * (float)Math.Tan((float)Math.Abs(angle) / 180.0 * 3.1415926535897931);
								rectangleF.Y = rect.Y;
								rectangleF.X = rect.X;
								rectangleF.Width = rect.Width - num3;
								rectangleF.Height = rect.Height;
							}
							else if (axis.AxisPosition == AxisPosition.Top)
							{
								rectangleF.Y = rect.Y;
								rectangleF.X = rect.X;
								rectangleF.Width = rect.Width - num3;
								rectangleF.Height = rect.Height;
								if (angle > 0)
								{
									rectangleF.X += num3;
								}
							}
							else if (axis.AxisPosition == AxisPosition.Bottom)
							{
								rectangleF.Y = rect.Y;
								rectangleF.X = rect.X;
								rectangleF.Width = rect.Width - num3;
								rectangleF.Height = rect.Height;
								if (angle < 0)
								{
									rectangleF.X += num3;
								}
							}
						}
					}
					base.Transform = this.myMatrix;
				}
				RectangleF rectangleF2 = new RectangleF(rectangleF.Location, rectangleF.Size);
				Image image2 = null;
				SizeF sizeF2 = default(SizeF);
				if (image.Length > 0)
				{
					ImageLoader.GetAdjustedImageSize(image2, this.Graphics, ref sizeF2);
					rectangleF2.Width -= (float)image2.Size.Width;
					rectangleF2.X += (float)image2.Size.Width;
					if (rectangleF2.Width < 1.0)
					{
						rectangleF2.Width = 1f;
					}
				}
				if (labelRowIndex > 0 && labelMark != 0)
				{
					sizeF = base.MeasureString(text.Replace("\\n", "\n"), font, rectangleF2.Size, stringFormat);
					SizeF labelSize = new SizeF(sizeF.Width, sizeF.Height);
					if (image2 != null)
					{
						labelSize.Width += (float)image2.Width;
					}
					this.DrawSecondRowLabelMark(axis, markColor, rectangleF, labelSize, labelMark, truncatedLeft, truncatedRight, matrix);
				}
				if ((stringFormat.FormatFlags & StringFormatFlags.LineLimit) != 0)
				{
					stringFormat.FormatFlags ^= StringFormatFlags.LineLimit;
					if (base.MeasureString("I", font, rectangleF.Size, stringFormat).Height < rectangleF.Height)
					{
						stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
					}
				}
				else
				{
					if ((stringFormat.FormatFlags & StringFormatFlags.NoClip) != 0)
					{
						stringFormat.FormatFlags ^= StringFormatFlags.NoClip;
					}
					SizeF sizeF3 = base.MeasureString("I", font, rectangleF.Size, stringFormat);
					stringFormat.FormatFlags ^= StringFormatFlags.NoClip;
					if (sizeF3.Height > rectangleF.Height)
					{
						float num4 = sizeF3.Height - rectangleF.Height;
						rectangleF.Y -= (float)(num4 / 2.0);
						rectangleF.Height += num4;
					}
				}
				base.DrawString(text.Replace("\\n", "\n"), font, brush, rectangleF2, stringFormat);
				if (commonElements.ProcessModeRegions)
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					graphicsPath.AddRectangle(rectangleF2);
					graphicsPath.Transform(base.Transform);
					string empty4 = string.Empty;
					string empty5 = string.Empty;
					empty4 = label.Href;
					empty5 = label.MapAreaAttributes;
					commonElements.HotRegionsList.AddHotRegion(this, graphicsPath, false, label.ToolTip, empty4, empty5, label, ChartElementType.AxisLabels);
				}
				if (image2 != null)
				{
					if (sizeF.IsEmpty)
					{
						sizeF = base.MeasureString(text.Replace("\\n", "\n"), font, rectangleF2.Size, stringFormat);
					}
					RectangleF rectangleF3 = new RectangleF((float)(rectangleF.X + (rectangleF.Width - (float)image2.Size.Width - sizeF.Width) / 2.0), (float)(rectangleF.Y + (rectangleF.Height - (float)image2.Size.Height) / 2.0), (float)image2.Size.Width, (float)image2.Size.Height);
					if (stringFormat.LineAlignment == StringAlignment.Center)
					{
						rectangleF3.Y = (float)(rectangleF.Y + (rectangleF.Height - (float)image2.Size.Height) / 2.0);
					}
					else if (stringFormat.LineAlignment == StringAlignment.Far)
					{
						rectangleF3.Y = (float)(rectangleF.Bottom - (sizeF.Height + (float)image2.Size.Height) / 2.0);
					}
					else if (stringFormat.LineAlignment == StringAlignment.Near)
					{
						rectangleF3.Y = (float)(rectangleF.Top + (sizeF.Height - (float)image2.Size.Height) / 2.0);
					}
					if (stringFormat.Alignment == StringAlignment.Center)
					{
						rectangleF3.X = (float)(rectangleF.X + (rectangleF.Width - (float)image2.Size.Width - sizeF.Width) / 2.0);
					}
					else if (stringFormat.Alignment == StringAlignment.Far)
					{
						rectangleF3.X = rectangleF.Right - (float)image2.Size.Width - sizeF.Width;
					}
					else if (stringFormat.Alignment == StringAlignment.Near)
					{
						rectangleF3.X = rectangleF.X;
					}
					ImageAttributes imageAttributes = new ImageAttributes();
					if (imageTranspColor != Color.Empty)
					{
						imageAttributes.SetColorKey(imageTranspColor, imageTranspColor, ColorAdjustType.Default);
					}
					base.DrawImage(image2, Rectangle.Round(rectangleF3), 0, 0, image2.Width, image2.Height, GraphicsUnit.Pixel, imageAttributes);
					if (commonElements.ProcessModeRegions)
					{
						GraphicsPath graphicsPath2 = new GraphicsPath();
						graphicsPath2.AddRectangle(rectangleF3);
						graphicsPath2.Transform(base.Transform);
						string empty6 = string.Empty;
						string empty7 = string.Empty;
						empty6 = label.ImageHref;
						empty7 = label.ImageMapAreaAttributes;
						commonElements.HotRegionsList.AddHotRegion(this, graphicsPath2, false, string.Empty, empty6, empty7, label, ChartElementType.AxisLabelImage);
					}
				}
				if (matrix != null)
				{
					base.Transform = matrix;
				}
			}
		}

		private void DrawSecondRowLabelBoxMark(Axis axis, Color markColor, RectangleF absPosition, SizeF labelSize, bool truncatedLeft, bool truncatedRight, Matrix originalTransform)
		{
			Matrix transform = base.Transform;
			if (originalTransform != null)
			{
				base.Transform = originalTransform;
			}
			PointF value = new PointF((float)(absPosition.X + absPosition.Width / 2.0), (float)(absPosition.Y + absPosition.Height / 2.0));
			Point.Round(value);
			if (axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right)
			{
				RectangleF empty = RectangleF.Empty;
				empty.X = (float)(value.X - absPosition.Height / 2.0);
				empty.Y = (float)(value.Y - absPosition.Width / 2.0);
				empty.Height = absPosition.Width;
				empty.Width = absPosition.Height;
				absPosition = empty;
			}
			float num = (float)axis.GetAxisPosition(true);
			PointF relative = new PointF(num, num);
			relative = this.GetAbsolutePoint(relative);
			Rectangle rectangle = Rectangle.Round(absPosition);
			rectangle.Width = (int)Math.Round((double)absPosition.Right) - rectangle.X;
			rectangle.Height = (int)Math.Round((double)absPosition.Bottom) - rectangle.Y;
			Pen pen = new Pen(markColor.IsEmpty ? axis.MajorTickMark.LineColor : markColor, (float)axis.MajorTickMark.LineWidth);
			pen.DashStyle = this.GetPenStyle(axis.MajorTickMark.LineStyle);
			if (axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right)
			{
				base.DrawLine(pen, (float)rectangle.Left, (float)rectangle.Top, (float)rectangle.Left, (float)rectangle.Bottom);
				base.DrawLine(pen, (float)rectangle.Right, (float)rectangle.Top, (float)rectangle.Right, (float)rectangle.Bottom);
			}
			else
			{
				base.DrawLine(pen, (float)rectangle.Left, (float)rectangle.Top, (float)rectangle.Right, (float)rectangle.Top);
				base.DrawLine(pen, (float)rectangle.Left, (float)rectangle.Bottom, (float)rectangle.Right, (float)rectangle.Bottom);
			}
			if (!truncatedLeft)
			{
				if (axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right)
				{
					base.DrawLine(pen, (float)((axis.AxisPosition == AxisPosition.Left) ? rectangle.Left : rectangle.Right), (float)rectangle.Bottom, relative.X, (float)rectangle.Bottom);
				}
				else
				{
					base.DrawLine(pen, (float)rectangle.Left, (float)((axis.AxisPosition == AxisPosition.Top) ? rectangle.Top : rectangle.Bottom), (float)rectangle.Left, relative.Y);
				}
			}
			if (!truncatedRight)
			{
				if (axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right)
				{
					base.DrawLine(pen, (float)((axis.AxisPosition == AxisPosition.Left) ? rectangle.Left : rectangle.Right), (float)rectangle.Top, relative.X, (float)rectangle.Top);
				}
				else
				{
					base.DrawLine(pen, (float)rectangle.Right, (float)((axis.AxisPosition == AxisPosition.Top) ? rectangle.Top : rectangle.Bottom), (float)rectangle.Right, relative.Y);
				}
			}
			if (pen != null)
			{
				pen.Dispose();
			}
			if (originalTransform != null)
			{
				base.Transform = transform;
			}
		}

		private void DrawSecondRowLabelMark(Axis axis, Color markColor, RectangleF absPosition, SizeF labelSize, LabelMark labelMark, bool truncatedLeft, bool truncatedRight, Matrix oldTransform)
		{
			if (axis.MajorTickMark.LineWidth != 0 && axis.MajorTickMark.LineStyle != 0 && !(axis.MajorTickMark.LineColor == Color.Empty))
			{
				SmoothingMode smoothingMode = base.SmoothingMode;
				base.SmoothingMode = SmoothingMode.None;
				if (labelMark == LabelMark.Box)
				{
					this.DrawSecondRowLabelBoxMark(axis, markColor, absPosition, labelSize, truncatedLeft, truncatedRight, oldTransform);
				}
				else
				{
					Point point = Point.Round(new PointF((float)(absPosition.X + absPosition.Width / 2.0), (float)(absPosition.Y + absPosition.Height / 2.0)));
					Rectangle rectangle = Rectangle.Round(absPosition);
					rectangle.Width = (int)Math.Round((double)absPosition.Right) - rectangle.X;
					rectangle.Height = (int)Math.Round((double)absPosition.Bottom) - rectangle.Y;
					PointF[] array = new PointF[3];
					PointF[] array2 = new PointF[3];
					array[0].X = (float)rectangle.Left;
					array[0].Y = (float)rectangle.Bottom;
					array[1].X = (float)rectangle.Left;
					array[1].Y = (float)point.Y;
					array[2].X = (float)Math.Round((double)point.X - labelSize.Width / 2.0 - 1.0);
					array[2].Y = (float)point.Y;
					array2[0].X = (float)rectangle.Right;
					array2[0].Y = (float)rectangle.Bottom;
					array2[1].X = (float)rectangle.Right;
					array2[1].Y = (float)point.Y;
					array2[2].X = (float)Math.Round((double)point.X + labelSize.Width / 2.0 - 1.0);
					array2[2].Y = (float)point.Y;
					if (axis.AxisPosition == AxisPosition.Bottom)
					{
						array[0].Y = (float)rectangle.Top;
						array2[0].Y = (float)rectangle.Top;
					}
					if (labelMark == LabelMark.SideMark)
					{
						array[2] = array[1];
						array2[2] = array2[1];
					}
					if (truncatedLeft)
					{
						array[0] = array[1];
					}
					if (truncatedRight)
					{
						array2[0] = array2[1];
					}
					Pen pen = new Pen(markColor.IsEmpty ? axis.MajorTickMark.LineColor : markColor, (float)axis.MajorTickMark.LineWidth);
					pen.DashStyle = this.GetPenStyle(axis.MajorTickMark.LineStyle);
					base.DrawLines(pen, array);
					base.DrawLines(pen, array2);
					if (pen != null)
					{
						pen.Dispose();
					}
				}
				base.SmoothingMode = smoothingMode;
			}
		}

		internal SizeF MeasureStringRel(string text, Font font)
		{
			SizeF size = base.MeasureString(text, font);
			return this.GetRelativeSize(size);
		}

		internal SizeF MeasureStringRel(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
		{
			SizeF absoluteSize = this.GetAbsoluteSize(layoutArea);
			SizeF size = base.MeasureString(text, font, absoluteSize, stringFormat);
			return this.GetRelativeSize(size);
		}

		internal Size MeasureStringAbs(string text, Font font)
		{
			SizeF sizeF = base.MeasureString(text, font);
			return new Size((int)Math.Ceiling((double)sizeF.Width), (int)Math.Ceiling((double)sizeF.Height));
		}

		internal Size MeasureStringAbs(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
		{
			SizeF sizeF = base.MeasureString(text, font, layoutArea, stringFormat);
			return new Size((int)Math.Ceiling((double)sizeF.Width), (int)Math.Ceiling((double)sizeF.Height));
		}

		internal void DrawStringRel(string text, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
		{
			if (layoutRectangle.Width != 0.0 && layoutRectangle.Height != 0.0)
			{
				RectangleF absoluteRectangle = this.GetAbsoluteRectangle(layoutRectangle);
				base.DrawString(text, font, brush, absoluteRectangle, format);
			}
		}

		internal void DrawStringRel(string text, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format, int angle)
		{
			PointF empty = PointF.Empty;
			if (layoutRectangle.Width != 0.0 && layoutRectangle.Height != 0.0)
			{
				RectangleF absoluteRectangle = this.GetAbsoluteRectangle(layoutRectangle);
				SizeF sizeF = base.MeasureString(text, font, absoluteRectangle.Size, format);
				if (format.Alignment == StringAlignment.Near)
				{
					empty.X = (float)(absoluteRectangle.X + sizeF.Width / 2.0);
					empty.Y = (float)((absoluteRectangle.Bottom + absoluteRectangle.Top) / 2.0);
				}
				else if (format.Alignment == StringAlignment.Far)
				{
					empty.X = (float)(absoluteRectangle.Right - sizeF.Width / 2.0);
					empty.Y = (float)((absoluteRectangle.Bottom + absoluteRectangle.Top) / 2.0);
				}
				else
				{
					empty.X = (float)((absoluteRectangle.Left + absoluteRectangle.Right) / 2.0);
					empty.Y = (float)((absoluteRectangle.Bottom + absoluteRectangle.Top) / 2.0);
				}
				this.myMatrix = base.Transform.Clone();
				this.myMatrix.RotateAt((float)angle, empty);
				Matrix transform = base.Transform;
				base.Transform = this.myMatrix;
				base.DrawString(text, font, brush, absoluteRectangle, format);
				base.Transform = transform;
			}
		}

		internal void DrawRectangleBarStyle(BarDrawingStyle barDrawingStyle, bool isVertical, RectangleF rect, int borderWidth)
		{
			if (barDrawingStyle != 0 && rect.Width > 0.0 && rect.Height > 0.0)
			{
				switch (barDrawingStyle)
				{
				case BarDrawingStyle.Cylinder:
				{
					RectangleF rect2 = rect;
					if (isVertical)
					{
						rect2.Width *= 0.3f;
					}
					else
					{
						rect2.Height *= 0.3f;
					}
					if (rect2.Width > 0.0 && rect2.Height > 0.0)
					{
						this.FillRectangleAbs(rect2, Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, (GradientType)(isVertical ? 1 : 2), Color.FromArgb(120, Color.White), Color.Empty, 0, ChartDashStyle.NotSet, PenAlignment.Inset);
						if (isVertical)
						{
							rect2.X += (float)(rect2.Width + 1.0);
							rect2.Width = rect.Right - rect2.X;
						}
						else
						{
							rect2.Y += (float)(rect2.Height + 1.0);
							rect2.Height = rect.Bottom - rect2.Y;
						}
						this.FillRectangleAbs(rect2, Color.FromArgb(120, Color.White), ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, (GradientType)(isVertical ? 1 : 2), Color.FromArgb(150, Color.Black), Color.Empty, 0, ChartDashStyle.NotSet, PenAlignment.Inset);
					}
					break;
				}
				case BarDrawingStyle.Emboss:
				{
					float num2 = 3f;
					if (rect.Width < 6.0 || rect.Height < 6.0)
					{
						num2 = 1f;
					}
					else if (rect.Width < 15.0 || rect.Height < 15.0)
					{
						num2 = 2f;
					}
					using (GraphicsPath graphicsPath4 = new GraphicsPath())
					{
						PointF[] points = new PointF[6]
						{
							new PointF(rect.Left, rect.Bottom),
							new PointF(rect.Left, rect.Top),
							new PointF(rect.Right, rect.Top),
							new PointF(rect.Right - num2, rect.Top + num2),
							new PointF(rect.Left + num2, rect.Top + num2),
							new PointF(rect.Left + num2, rect.Bottom - num2)
						};
						graphicsPath4.AddPolygon(points);
						using (SolidBrush brush4 = new SolidBrush(Color.FromArgb(100, Color.White)))
						{
							base.FillPath(brush4, graphicsPath4);
						}
					}
					using (GraphicsPath graphicsPath5 = new GraphicsPath())
					{
						PointF[] points2 = new PointF[6]
						{
							new PointF(rect.Right, rect.Top),
							new PointF(rect.Right, rect.Bottom),
							new PointF(rect.Left, rect.Bottom),
							new PointF(rect.Left + num2, rect.Bottom - num2),
							new PointF(rect.Right - num2, rect.Bottom - num2),
							new PointF(rect.Right - num2, rect.Top + num2)
						};
						graphicsPath5.AddPolygon(points2);
						using (SolidBrush brush5 = new SolidBrush(Color.FromArgb(80, Color.Black)))
						{
							base.FillPath(brush5, graphicsPath5);
						}
					}
					break;
				}
				case BarDrawingStyle.LightToDark:
				{
					float num3 = 4f;
					if (rect.Width < 6.0 || rect.Height < 6.0)
					{
						num3 = 2f;
					}
					else if (rect.Width < 15.0 || rect.Height < 15.0)
					{
						num3 = 3f;
					}
					RectangleF rect3 = rect;
					rect3.Inflate((float)(0.0 - num3), (float)(0.0 - num3));
					if (isVertical)
					{
						rect3.Height = (float)Math.Floor(rect3.Height / 3.0);
					}
					else
					{
						rect3.X = rect3.Right - (float)Math.Floor(rect3.Width / 3.0);
						rect3.Width = (float)Math.Floor(rect3.Width / 3.0);
					}
					if (rect3.Width > 0.0 && rect3.Height > 0.0)
					{
						this.FillRectangleAbs(rect3, isVertical ? Color.FromArgb(120, Color.White) : Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, (GradientType)((!isVertical) ? 1 : 2), isVertical ? Color.Transparent : Color.FromArgb(120, Color.White), Color.Empty, 0, ChartDashStyle.NotSet, PenAlignment.Inset);
						rect3 = rect;
						rect3.Inflate((float)(0.0 - num3), (float)(0.0 - num3));
						if (isVertical)
						{
							rect3.Y = rect3.Bottom - (float)Math.Floor(rect3.Height / 3.0);
							rect3.Height = (float)Math.Floor(rect3.Height / 3.0);
						}
						else
						{
							rect3.Width = (float)Math.Floor(rect3.Width / 3.0);
						}
						this.FillRectangleAbs(rect3, (!isVertical) ? Color.FromArgb(80, Color.Black) : Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, (GradientType)((!isVertical) ? 1 : 2), (!isVertical) ? Color.Transparent : Color.FromArgb(80, Color.Black), Color.Empty, 0, ChartDashStyle.NotSet, PenAlignment.Inset);
					}
					break;
				}
				case BarDrawingStyle.Wedge:
				{
					float num = (float)(isVertical ? (rect.Width / 2.0) : (rect.Height / 2.0));
					if (isVertical && 2.0 * num > rect.Height)
					{
						num = (float)(rect.Height / 2.0);
					}
					if (!isVertical && 2.0 * num > rect.Width)
					{
						num = (float)(rect.Width / 2.0);
					}
					RectangleF rectangleF = rect;
					using (GraphicsPath graphicsPath = new GraphicsPath())
					{
						if (isVertical)
						{
							graphicsPath.AddLine((float)(rectangleF.X + rectangleF.Width / 2.0), rectangleF.Y + num, (float)(rectangleF.X + rectangleF.Width / 2.0), rectangleF.Bottom - num);
							graphicsPath.AddLine((float)(rectangleF.X + rectangleF.Width / 2.0), rectangleF.Bottom - num, rectangleF.Right, rectangleF.Bottom);
							graphicsPath.AddLine(rectangleF.Right, rectangleF.Bottom, rectangleF.Right, rectangleF.Y);
						}
						else
						{
							graphicsPath.AddLine(rectangleF.X + num, (float)(rectangleF.Y + rectangleF.Height / 2.0), rectangleF.Right - num, (float)(rectangleF.Y + rectangleF.Height / 2.0));
							graphicsPath.AddLine(rectangleF.Right - num, (float)(rectangleF.Y + rectangleF.Height / 2.0), rectangleF.Right, rectangleF.Bottom);
							graphicsPath.AddLine(rectangleF.Right, rectangleF.Bottom, rectangleF.Left, rectangleF.Bottom);
						}
						graphicsPath.CloseAllFigures();
						using (SolidBrush brush = new SolidBrush(Color.FromArgb(90, Color.Black)))
						{
							base.FillPath(brush, graphicsPath);
						}
					}
					using (GraphicsPath graphicsPath2 = new GraphicsPath())
					{
						if (isVertical)
						{
							graphicsPath2.AddLine(rectangleF.X, rectangleF.Y, (float)(rectangleF.X + rectangleF.Width / 2.0), rectangleF.Y + num);
							graphicsPath2.AddLine((float)(rectangleF.X + rectangleF.Width / 2.0), rectangleF.Y + num, rectangleF.Right, rectangleF.Y);
						}
						else
						{
							graphicsPath2.AddLine(rectangleF.Right, rectangleF.Y, rectangleF.Right - num, (float)(rectangleF.Y + rectangleF.Height / 2.0));
							graphicsPath2.AddLine(rectangleF.Right - num, (float)(rectangleF.Y + rectangleF.Height / 2.0), rectangleF.Right, rectangleF.Bottom);
						}
						using (SolidBrush brush2 = new SolidBrush(Color.FromArgb(50, Color.Black)))
						{
							base.FillPath(brush2, graphicsPath2);
							using (Pen pen = new Pen(Color.FromArgb(20, Color.Black), 1f))
							{
								base.DrawPath(pen, graphicsPath2);
								if (isVertical)
								{
									base.DrawLine(pen, (float)(rect.X + rect.Width / 2.0), rect.Y + num, (float)(rect.X + rect.Width / 2.0), rect.Bottom - num);
								}
								else
								{
									base.DrawLine(pen, rect.X + num, (float)(rect.Y + rect.Height / 2.0), rect.X + num, (float)(rect.Bottom - rect.Height / 2.0));
								}
							}
							using (Pen pen2 = new Pen(Color.FromArgb(40, Color.White), 1f))
							{
								base.DrawPath(pen2, graphicsPath2);
								if (isVertical)
								{
									base.DrawLine(pen2, (float)(rect.X + rect.Width / 2.0), rect.Y + num, (float)(rect.X + rect.Width / 2.0), rect.Bottom - num);
								}
								else
								{
									base.DrawLine(pen2, rect.X + num, (float)(rect.Y + rect.Height / 2.0), rect.X + num, (float)(rect.Bottom - rect.Height / 2.0));
								}
							}
						}
					}
					using (GraphicsPath graphicsPath3 = new GraphicsPath())
					{
						if (isVertical)
						{
							graphicsPath3.AddLine(rectangleF.X, rectangleF.Bottom, (float)(rectangleF.X + rectangleF.Width / 2.0), rectangleF.Bottom - num);
							graphicsPath3.AddLine((float)(rectangleF.X + rectangleF.Width / 2.0), rectangleF.Bottom - num, rectangleF.Right, rectangleF.Bottom);
						}
						else
						{
							graphicsPath3.AddLine(rectangleF.X, rectangleF.Y, rectangleF.X + num, (float)(rectangleF.Y + rectangleF.Height / 2.0));
							graphicsPath3.AddLine(rectangleF.X + num, (float)(rectangleF.Y + rectangleF.Height / 2.0), rectangleF.X, rectangleF.Bottom);
						}
						using (SolidBrush brush3 = new SolidBrush(Color.FromArgb(50, Color.Black)))
						{
							base.FillPath(brush3, graphicsPath3);
							using (Pen pen3 = new Pen(Color.FromArgb(20, Color.Black), 1f))
							{
								base.DrawPath(pen3, graphicsPath3);
							}
							using (Pen pen4 = new Pen(Color.FromArgb(40, Color.White), 1f))
							{
								base.DrawPath(pen4, graphicsPath3);
							}
						}
					}
					break;
				}
				}
			}
		}

		internal void FillRectangleRel(RectangleF rectF, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, Color shadowColor, int shadowOffset, PenAlignment penAlignment, BarDrawingStyle barDrawingStyle, bool isVertical)
		{
			this.FillRectangleRel(rectF, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backGradientEndColor, borderColor, borderWidth, borderStyle, shadowColor, shadowOffset, penAlignment, false, 0, false, barDrawingStyle, isVertical);
		}

		internal void FillRectangleRel(RectangleF rectF, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, Color shadowColor, int shadowOffset, PenAlignment penAlignment)
		{
			this.FillRectangleRel(rectF, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backGradientEndColor, borderColor, borderWidth, borderStyle, shadowColor, shadowOffset, penAlignment, false, 0, false, BarDrawingStyle.Default, true);
		}

		internal void FillRectangleRel(RectangleF rectF, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, Color shadowColor, int shadowOffset, PenAlignment penAlignment, bool circular, int circularSectorsCount, bool circle3D)
		{
			this.FillRectangleRel(rectF, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backGradientEndColor, borderColor, borderWidth, borderStyle, shadowColor, shadowOffset, penAlignment, circular, circularSectorsCount, circle3D, BarDrawingStyle.Default, true);
		}

		internal void FillRectangleRel(RectangleF rectF, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, Color shadowColor, int shadowOffset, PenAlignment penAlignment, bool circular, int circularSectorsCount, bool circle3D, BarDrawingStyle barDrawingStyle, bool isVertical)
		{
			Brush brush = null;
			Brush brush2 = null;
			SmoothingMode smoothingMode = base.SmoothingMode;
			if (!circular)
			{
				base.SmoothingMode = SmoothingMode.Default;
			}
			if (backColor.IsEmpty)
			{
				backColor = Color.White;
			}
			if (backGradientEndColor.IsEmpty)
			{
				backGradientEndColor = Color.White;
			}
			if (borderColor.IsEmpty || borderStyle == ChartDashStyle.NotSet)
			{
				borderWidth = 0;
			}
			RectangleF rectangleF = this.GetAbsoluteRectangle(rectF);
			if (rectangleF.Width < 1.0 && rectangleF.Width > 0.0)
			{
				rectangleF.Width = 1f;
			}
			if (rectangleF.Height < 1.0 && rectangleF.Height > 0.0)
			{
				rectangleF.Height = 1f;
			}
			rectangleF = this.Round(rectangleF);
			RectangleF rectangleF2 = (penAlignment != PenAlignment.Inset || borderWidth <= 0) ? rectangleF : ((base.ActiveRenderingType == RenderingType.Svg || this.IsMetafile) ? new RectangleF(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height) : ((this.Graphics.Transform.Elements[0] != 1.0 || this.Graphics.Transform.Elements[3] != 1.0) ? new RectangleF(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height) : new RectangleF(rectangleF.X + (float)borderWidth, rectangleF.Y + (float)borderWidth, (float)(rectangleF.Width - (float)borderWidth * 2.0 + 1.0), (float)(rectangleF.Height - (float)borderWidth * 2.0 + 1.0))));
			if (rectangleF2.Width > 2.0 * (float)this.width)
			{
				rectangleF2.Width = (float)(2.0 * (float)this.width);
			}
			if (rectangleF2.Height > 2.0 * (float)this.height)
			{
				rectangleF2.Height = (float)(2.0 * (float)this.height);
			}
			if (backImage.Length > 0 && backImageMode != ChartImageWrapMode.Unscaled && backImageMode != ChartImageWrapMode.Scaled)
			{
				if (backColor != Color.Empty && backColor != Color.Transparent)
				{
					brush2 = new SolidBrush(backColor);
				}
				brush = this.GetTextureBrush(backImage, backImageTranspColor, backImageMode, backColor);
			}
			else
			{
				brush = ((backHatchStyle == ChartHatchStyle.None) ? ((backGradientType == GradientType.None) ? ((!(backColor == Color.Empty) && !(backColor == Color.Transparent)) ? new SolidBrush(backColor) : null) : this.GetGradientBrush(rectangleF, backColor, backGradientEndColor, backGradientType)) : this.GetHatchBrush(backHatchStyle, backColor, backGradientEndColor));
			}
			this.FillRectangleShadowAbs(rectangleF, shadowColor, (float)shadowOffset, backColor, circular, circularSectorsCount);
			if (backImage.Length > 0 && (backImageMode == ChartImageWrapMode.Unscaled || backImageMode == ChartImageWrapMode.Scaled))
			{
				Image image = this.common.ImageLoader.LoadImage(backImage);
				ImageAttributes imageAttributes = new ImageAttributes();
				if (backImageTranspColor != Color.Empty)
				{
					imageAttributes.SetColorKey(backImageTranspColor, backImageTranspColor, ColorAdjustType.Default);
				}
				RectangleF rectangleF3 = default(RectangleF);
				rectangleF3.X = rectangleF2.X;
				rectangleF3.Y = rectangleF2.Y;
				rectangleF3.Width = rectangleF2.Width;
				rectangleF3.Height = rectangleF2.Height;
				if (backImageMode == ChartImageWrapMode.Unscaled)
				{
					SizeF sizeF = default(SizeF);
					ImageLoader.GetAdjustedImageSize(image, this.Graphics, ref sizeF);
					rectangleF3.Width = Math.Min(rectangleF2.Width, sizeF.Width);
					rectangleF3.Height = Math.Min(rectangleF2.Height, sizeF.Height);
					if (rectangleF3.Width < rectangleF2.Width)
					{
						switch (backImageAlign)
						{
						case ChartImageAlign.TopRight:
						case ChartImageAlign.Right:
						case ChartImageAlign.BottomRight:
							rectangleF3.X = rectangleF2.Right - rectangleF3.Width;
							break;
						case ChartImageAlign.Top:
						case ChartImageAlign.Bottom:
						case ChartImageAlign.Center:
							rectangleF3.X = (float)(rectangleF2.X + (rectangleF2.Width - rectangleF3.Width) / 2.0);
							break;
						}
					}
					if (rectangleF3.Height < rectangleF2.Height)
					{
						switch (backImageAlign)
						{
						case ChartImageAlign.BottomRight:
						case ChartImageAlign.Bottom:
						case ChartImageAlign.BottomLeft:
							rectangleF3.Y = rectangleF2.Bottom - rectangleF3.Height;
							break;
						case ChartImageAlign.Right:
						case ChartImageAlign.Left:
						case ChartImageAlign.Center:
							rectangleF3.Y = (float)(rectangleF2.Y + (rectangleF2.Height - rectangleF3.Height) / 2.0);
							break;
						}
					}
				}
				if (brush != null)
				{
					if (circular)
					{
						this.DrawCircleAbs(null, brush, rectangleF2, circularSectorsCount, circle3D);
					}
					else
					{
						base.FillRectangle(brush, rectangleF2);
					}
				}
				base.DrawImage(image, new Rectangle((int)Math.Round((double)rectangleF3.X), (int)Math.Round((double)rectangleF3.Y), (int)Math.Round((double)rectangleF3.Width), (int)Math.Round((double)rectangleF3.Height)), 0f, 0f, (backImageMode == ChartImageWrapMode.Unscaled) ? rectangleF3.Width : ((float)image.Width), (backImageMode == ChartImageWrapMode.Unscaled) ? rectangleF3.Height : ((float)image.Height), GraphicsUnit.Pixel, imageAttributes);
			}
			else
			{
				if (brush2 != null && backImageTranspColor != Color.Empty)
				{
					if (circular)
					{
						this.DrawCircleAbs(null, brush2, rectangleF2, circularSectorsCount, circle3D);
					}
					else
					{
						base.FillRectangle(brush2, rectangleF2);
					}
				}
				if (brush != null)
				{
					if (circular)
					{
						this.DrawCircleAbs(null, brush, rectangleF2, circularSectorsCount, circle3D);
					}
					else
					{
						base.FillRectangle(brush, rectangleF2);
					}
				}
			}
			this.DrawRectangleBarStyle(barDrawingStyle, isVertical, rectangleF2, (borderStyle != 0) ? borderWidth : 0);
			if (borderWidth > 0 && borderStyle != 0)
			{
				if (this.pen.Color != borderColor)
				{
					this.pen.Color = borderColor;
				}
				if (this.pen.Width != (float)borderWidth)
				{
					this.pen.Width = (float)borderWidth;
				}
				if (this.pen.Alignment != penAlignment)
				{
					this.pen.Alignment = penAlignment;
				}
				if (this.pen.DashStyle != this.GetPenStyle(borderStyle))
				{
					this.pen.DashStyle = this.GetPenStyle(borderStyle);
				}
				if (circular)
				{
					this.DrawCircleAbs(this.pen, null, rectangleF, circularSectorsCount, false);
				}
				else
				{
					if (this.pen.Alignment == PenAlignment.Inset && this.pen.Width > 1.0)
					{
						rectangleF.Width += 1f;
						rectangleF.Height += 1f;
					}
					base.DrawRectangle(this.pen, rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height);
				}
			}
			if (brush != null)
			{
				brush.Dispose();
			}
			base.SmoothingMode = smoothingMode;
		}

		public void FillRectangleShadowAbs(RectangleF rect, Color shadowColor, float shadowOffset, Color backColor)
		{
			this.FillRectangleShadowAbs(rect, shadowColor, shadowOffset, backColor, false, 0);
		}

		internal void FillRectangleShadowAbs(RectangleF rect, Color shadowColor, float shadowOffset, Color backColor, bool circular, int circularSectorsCount)
		{
			if (rect.Height != 0.0 && rect.Width != 0.0 && shadowOffset != 0.0 && shadowOffset != 0.0 && !(shadowColor == Color.Empty))
			{
				bool flag = false;
				Region clip = null;
				if (!circular && backColor == Color.Transparent)
				{
					flag = true;
					clip = base.Clip;
					Region region = new Region();
					region.MakeInfinite();
					region.Xor(rect);
					base.Clip = region;
				}
				if (!this.softShadows || circularSectorsCount > 2)
				{
					RectangleF empty = RectangleF.Empty;
					RectangleF rectangleF = this.Round(rect);
					SolidBrush brush = new SolidBrush((shadowColor.A != 255) ? shadowColor : Color.FromArgb((int)backColor.A / 2, shadowColor));
					empty.X = rectangleF.X + shadowOffset;
					empty.Y = rectangleF.Y + shadowOffset;
					empty.Width = rectangleF.Width;
					empty.Height = rectangleF.Height;
					base.shadowDrawingMode = true;
					if (circular)
					{
						this.DrawCircleAbs(null, brush, empty, circularSectorsCount, false);
					}
					else
					{
						base.FillRectangle(brush, empty);
					}
					base.shadowDrawingMode = false;
				}
				else
				{
					RectangleF empty2 = RectangleF.Empty;
					RectangleF rectangleF2 = this.Round(rect);
					empty2.X = (float)(rectangleF2.X + shadowOffset - 1.0);
					empty2.Y = (float)(rectangleF2.Y + shadowOffset - 1.0);
					empty2.Width = (float)(rectangleF2.Width + 2.0);
					empty2.Height = (float)(rectangleF2.Height + 2.0);
					float val = (float)(shadowOffset * 0.699999988079071);
					val = Math.Max(val, 2f);
					val = Math.Min(val, (float)(empty2.Width / 4.0));
					val = Math.Min(val, (float)(empty2.Height / 4.0));
					val = (float)Math.Ceiling((double)val);
					if (circular)
					{
						val = (float)(empty2.Width / 2.0);
					}
					GraphicsPath graphicsPath = new GraphicsPath();
					if (circular && empty2.Width != empty2.Height)
					{
						float num = (float)(empty2.Width / 2.0);
						float num2 = (float)(empty2.Height / 2.0);
						graphicsPath.AddLine(empty2.X + num, empty2.Y, empty2.Right - num, empty2.Y);
						graphicsPath.AddArc((float)(empty2.Right - 2.0 * num), empty2.Y, (float)(2.0 * num), (float)(2.0 * num2), 270f, 90f);
						graphicsPath.AddLine(empty2.Right, empty2.Y + num2, empty2.Right, empty2.Bottom - num2);
						graphicsPath.AddArc((float)(empty2.Right - 2.0 * num), (float)(empty2.Bottom - 2.0 * num2), (float)(2.0 * num), (float)(2.0 * num2), 0f, 90f);
						graphicsPath.AddLine(empty2.Right - num, empty2.Bottom, empty2.X + num, empty2.Bottom);
						graphicsPath.AddArc(empty2.X, (float)(empty2.Bottom - 2.0 * num2), (float)(2.0 * num), (float)(2.0 * num2), 90f, 90f);
						graphicsPath.AddLine(empty2.X, empty2.Bottom - num2, empty2.X, empty2.Y + num2);
						graphicsPath.AddArc(empty2.X, empty2.Y, (float)(2.0 * num), (float)(2.0 * num2), 180f, 90f);
					}
					else
					{
						graphicsPath.AddLine(empty2.X + val, empty2.Y, empty2.Right - val, empty2.Y);
						graphicsPath.AddArc((float)(empty2.Right - 2.0 * val), empty2.Y, (float)(2.0 * val), (float)(2.0 * val), 270f, 90f);
						graphicsPath.AddLine(empty2.Right, empty2.Y + val, empty2.Right, empty2.Bottom - val);
						graphicsPath.AddArc((float)(empty2.Right - 2.0 * val), (float)(empty2.Bottom - 2.0 * val), (float)(2.0 * val), (float)(2.0 * val), 0f, 90f);
						graphicsPath.AddLine(empty2.Right - val, empty2.Bottom, empty2.X + val, empty2.Bottom);
						graphicsPath.AddArc(empty2.X, (float)(empty2.Bottom - 2.0 * val), (float)(2.0 * val), (float)(2.0 * val), 90f, 90f);
						graphicsPath.AddLine(empty2.X, empty2.Bottom - val, empty2.X, empty2.Y + val);
						graphicsPath.AddArc(empty2.X, empty2.Y, (float)(2.0 * val), (float)(2.0 * val), 180f, 90f);
					}
					PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
					pathGradientBrush.CenterColor = shadowColor;
					Color[] array = new Color[1]
					{
						Color.Transparent
					};
					Color[] array3 = pathGradientBrush.SurroundColors = array;
					pathGradientBrush.CenterPoint = new PointF((float)(empty2.X + empty2.Width / 2.0), (float)(empty2.Y + empty2.Height / 2.0));
					PointF focusScales = new PointF((float)(1.0 - 2.0 * shadowOffset / empty2.Width), (float)(1.0 - 2.0 * shadowOffset / empty2.Height));
					if (focusScales.X < 0.0)
					{
						focusScales.X = 0f;
					}
					if (focusScales.Y < 0.0)
					{
						focusScales.Y = 0f;
					}
					pathGradientBrush.FocusScales = focusScales;
					base.shadowDrawingMode = true;
					base.FillPath(pathGradientBrush, graphicsPath);
					base.shadowDrawingMode = false;
				}
				if (flag)
				{
					Region clip2 = base.Clip;
					base.Clip = clip;
					clip2.Dispose();
				}
			}
		}

		internal GraphicsPath GetPolygonCirclePath(RectangleF position, int polygonSectorsNumber)
		{
			PointF pointF = new PointF((float)(position.X + position.Width / 2.0), position.Y);
			PointF point = new PointF((float)(position.X + position.Width / 2.0), (float)(position.Y + position.Height / 2.0));
			float num = 0f;
			GraphicsPath graphicsPath = new GraphicsPath();
			PointF pt = PointF.Empty;
			float num2 = 0f;
			num = (float)((polygonSectorsNumber > 2) ? (360.0 / (float)polygonSectorsNumber) : 1.0);
			for (num2 = 0f; num2 < 360.0; num2 += num)
			{
				Matrix matrix = new Matrix();
				matrix.RotateAt(num2, point);
				PointF[] array = new PointF[1]
				{
					pointF
				};
				matrix.TransformPoints(array);
				if (!pt.IsEmpty)
				{
					graphicsPath.AddLine(pt, array[0]);
				}
				pt = array[0];
			}
			graphicsPath.CloseAllFigures();
			return graphicsPath;
		}

		internal void DrawCircleAbs(Pen pen, Brush brush, RectangleF position, int polygonSectorsNumber, bool circle3D)
		{
			bool flag = circle3D && brush != null;
			if (polygonSectorsNumber <= 2 && !flag)
			{
				if (brush != null)
				{
					base.FillEllipse(brush, position);
				}
				if (pen != null)
				{
					base.DrawEllipse(pen, position);
				}
			}
			else
			{
				PointF pointF = new PointF((float)(position.X + position.Width / 2.0), position.Y);
				PointF pointF2 = new PointF((float)(position.X + position.Width / 2.0), (float)(position.Y + position.Height / 2.0));
				float num = 0f;
				GraphicsPath graphicsPath = new GraphicsPath();
				PointF pointF3 = PointF.Empty;
				float num2 = 0f;
				SmoothingMode smoothingMode = base.SmoothingMode;
				if (flag)
				{
					base.SmoothingMode = SmoothingMode.None;
				}
				num = (float)((polygonSectorsNumber > 2) ? (360.0 / (float)polygonSectorsNumber) : 1.0);
				for (num2 = 0f; num2 < 360.0; num2 += num)
				{
					Matrix matrix = new Matrix();
					matrix.RotateAt(num2, pointF2);
					PointF[] array = new PointF[1]
					{
						pointF
					};
					matrix.TransformPoints(array);
					if (!pointF3.IsEmpty)
					{
						graphicsPath.AddLine(pointF3, array[0]);
						if (flag)
						{
							graphicsPath.AddLine(array[0], pointF2);
							graphicsPath.AddLine(pointF2, pointF3);
							base.FillPath(this.GetSector3DBrush(brush, num2, num), graphicsPath);
							graphicsPath.Reset();
						}
					}
					pointF3 = array[0];
				}
				graphicsPath.CloseAllFigures();
				if (!pointF3.IsEmpty && flag)
				{
					graphicsPath.AddLine(pointF3, pointF);
					graphicsPath.AddLine(pointF, pointF2);
					graphicsPath.AddLine(pointF2, pointF3);
					base.FillPath(this.GetSector3DBrush(brush, num2, num), graphicsPath);
					graphicsPath.Reset();
				}
				if (flag)
				{
					base.SmoothingMode = smoothingMode;
				}
				if (brush != null && !circle3D)
				{
					base.FillPath(brush, graphicsPath);
				}
				if (pen != null)
				{
					base.DrawPath(pen, graphicsPath);
				}
			}
		}

		internal Brush GetSector3DBrush(Brush brush, float curentSector, float sectorSize)
		{
			Color beginColor = Color.Gray;
			if (brush is HatchBrush)
			{
				beginColor = ((HatchBrush)brush).BackgroundColor;
			}
			else if (brush is LinearGradientBrush)
			{
				beginColor = ((LinearGradientBrush)brush).LinearColors[0];
			}
			else if (brush is PathGradientBrush)
			{
				beginColor = ((PathGradientBrush)brush).CenterColor;
			}
			else if (brush is SolidBrush)
			{
				beginColor = ((SolidBrush)brush).Color;
			}
			curentSector = (float)(curentSector - sectorSize / 2.0);
			if (sectorSize == 72.0 && curentSector == 180.0)
			{
				curentSector = (float)(curentSector * 0.800000011920929);
			}
			if (curentSector > 180.0)
			{
				curentSector = (float)(360.0 - curentSector);
			}
			curentSector = (float)(curentSector / 180.0);
			beginColor = this.GetBrightGradientColor(beginColor, (double)curentSector);
			return new SolidBrush(beginColor);
		}

		internal Color GetBrightGradientColor(Color beginColor, double position)
		{
			double num = 0.5;
			if (position < num)
			{
				return ChartGraphics.GetGradientColor(Color.FromArgb(beginColor.A, 255, 255, 255), beginColor, 1.0 - num + position);
			}
			if (0.0 - num + position < 1.0)
			{
				return ChartGraphics.GetGradientColor(beginColor, Color.Black, 0.0 - num + position);
			}
			return Color.FromArgb(beginColor.A, 0, 0, 0);
		}

		internal void FillRectangleAbs(RectangleF rect, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, PenAlignment penAlignment)
		{
			Brush brush = null;
			Brush brush2 = null;
			SmoothingMode smoothingMode = base.SmoothingMode;
			base.SmoothingMode = SmoothingMode.None;
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
				brush = this.GetGradientBrush(rect, backColor, backGradientEndColor, backGradientType);
			}
			if (backHatchStyle != 0)
			{
				brush = this.GetHatchBrush(backHatchStyle, backColor, backGradientEndColor);
			}
			if (backImage.Length > 0 && backImageMode != ChartImageWrapMode.Unscaled && backImageMode != ChartImageWrapMode.Scaled)
			{
				brush2 = brush;
				brush = this.GetTextureBrush(backImage, backImageTranspColor, backImageMode, backColor);
			}
			RectangleF rectangleF = new RectangleF(rect.X + (float)borderWidth, rect.Y + (float)borderWidth, rect.Width - (float)(borderWidth * 2), rect.Height - (float)(borderWidth * 2));
			rectangleF.Width += 1f;
			rectangleF.Height += 1f;
			if (backImage.Length > 0 && (backImageMode == ChartImageWrapMode.Unscaled || backImageMode == ChartImageWrapMode.Scaled))
			{
				Image image = this.common.ImageLoader.LoadImage(backImage);
				ImageAttributes imageAttributes = new ImageAttributes();
				if (backImageTranspColor != Color.Empty)
				{
					imageAttributes.SetColorKey(backImageTranspColor, backImageTranspColor, ColorAdjustType.Default);
				}
				RectangleF rectangleF2 = default(RectangleF);
				rectangleF2.X = rectangleF.X;
				rectangleF2.Y = rectangleF.Y;
				rectangleF2.Width = rectangleF.Width;
				rectangleF2.Height = rectangleF.Height;
				if (backImageMode == ChartImageWrapMode.Unscaled)
				{
					SizeF sizeF = default(SizeF);
					ImageLoader.GetAdjustedImageSize(image, this.Graphics, ref sizeF);
					rectangleF2.Width = sizeF.Width;
					rectangleF2.Height = sizeF.Height;
					if (rectangleF2.Width < rectangleF.Width)
					{
						switch (backImageAlign)
						{
						case ChartImageAlign.TopRight:
						case ChartImageAlign.Right:
						case ChartImageAlign.BottomRight:
							rectangleF2.X = rectangleF.Right - rectangleF2.Width;
							break;
						case ChartImageAlign.Top:
						case ChartImageAlign.Bottom:
						case ChartImageAlign.Center:
							rectangleF2.X = (float)(rectangleF.X + (rectangleF.Width - rectangleF2.Width) / 2.0);
							break;
						}
					}
					if (rectangleF2.Height < rectangleF.Height)
					{
						switch (backImageAlign)
						{
						case ChartImageAlign.BottomRight:
						case ChartImageAlign.Bottom:
						case ChartImageAlign.BottomLeft:
							rectangleF2.Y = rectangleF.Bottom - rectangleF2.Height;
							break;
						case ChartImageAlign.Right:
						case ChartImageAlign.Left:
						case ChartImageAlign.Center:
							rectangleF2.Y = (float)(rectangleF.Y + (rectangleF.Height - rectangleF2.Height) / 2.0);
							break;
						}
					}
				}
				base.FillRectangle(brush, rect.X, rect.Y, (float)(rect.Width + 1.0), (float)(rect.Height + 1.0));
				base.DrawImage(image, new Rectangle((int)Math.Round((double)rectangleF2.X), (int)Math.Round((double)rectangleF2.Y), (int)Math.Round((double)rectangleF2.Width), (int)Math.Round((double)rectangleF2.Height)), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
			}
			else
			{
				if (brush2 != null && backImageTranspColor != Color.Empty)
				{
					base.FillRectangle(brush2, rect.X, rect.Y, (float)(rect.Width + 1.0), (float)(rect.Height + 1.0));
				}
				base.FillRectangle(brush, rect.X, rect.Y, (float)(rect.Width + 1.0), (float)(rect.Height + 1.0));
			}
			if (borderStyle != 0)
			{
				if (borderWidth > 1)
				{
					base.DrawRectangle(this.pen, rect.X, rect.Y, (float)(rect.Width + 1.0), (float)(rect.Height + 1.0));
				}
				else if (borderWidth == 1)
				{
					base.DrawRectangle(this.pen, rect.X, rect.Y, rect.Width, rect.Height);
				}
			}
			if (backGradientType != 0)
			{
				brush.Dispose();
			}
			if (backImage.Length > 0 && backImageMode != ChartImageWrapMode.Unscaled && backImageMode != ChartImageWrapMode.Scaled)
			{
				brush.Dispose();
			}
			if (backHatchStyle != 0)
			{
				brush.Dispose();
			}
			base.SmoothingMode = smoothingMode;
		}

		internal void DrawPathAbs(GraphicsPath path, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, PenAlignment penAlignment, int shadowOffset, Color shadowColor)
		{
			if (shadowOffset != 0 && shadowColor != Color.Transparent)
			{
				GraphicsState gstate = base.Save();
				base.TranslateTransform((float)shadowOffset, (float)shadowOffset);
				if (backColor == Color.Transparent && backGradientEndColor.IsEmpty)
				{
					this.DrawPathAbs(path, Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, shadowColor, borderWidth, borderStyle, PenAlignment.Center);
				}
				else
				{
					this.DrawPathAbs(path, shadowColor, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, Color.Transparent, 0, ChartDashStyle.NotSet, PenAlignment.Center);
				}
				base.Restore(gstate);
			}
			this.DrawPathAbs(path, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backGradientEndColor, borderColor, borderWidth, borderStyle, penAlignment);
		}

		internal void DrawPathAbs(GraphicsPath path, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, PenAlignment penAlignment)
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
				brush = this.GetHatchBrush(backHatchStyle, backColor, backGradientEndColor);
			}
			if (backImage.Length > 0 && backImageMode != ChartImageWrapMode.Unscaled && backImageMode != ChartImageWrapMode.Scaled)
			{
				brush2 = brush;
				brush = this.GetTextureBrush(backImage, backImageTranspColor, backImageMode, backColor);
			}
			RectangleF bounds2 = path.GetBounds();
			if (backImage.Length > 0 && (backImageMode == ChartImageWrapMode.Unscaled || backImageMode == ChartImageWrapMode.Scaled))
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
				if (backImageMode == ChartImageWrapMode.Unscaled)
				{
					SizeF sizeF = default(SizeF);
					ImageLoader.GetAdjustedImageSize(image, this.Graphics, ref sizeF);
					rectangleF.Width = sizeF.Width;
					rectangleF.Height = sizeF.Height;
					if (rectangleF.Width < bounds2.Width)
					{
						switch (backImageAlign)
						{
						case ChartImageAlign.TopRight:
						case ChartImageAlign.Right:
						case ChartImageAlign.BottomRight:
							rectangleF.X = bounds2.Right - rectangleF.Width;
							break;
						case ChartImageAlign.Top:
						case ChartImageAlign.Bottom:
						case ChartImageAlign.Center:
							rectangleF.X = (float)(bounds2.X + (bounds2.Width - rectangleF.Width) / 2.0);
							break;
						}
					}
					if (rectangleF.Height < bounds2.Height)
					{
						switch (backImageAlign)
						{
						case ChartImageAlign.BottomRight:
						case ChartImageAlign.Bottom:
						case ChartImageAlign.BottomLeft:
							rectangleF.Y = bounds2.Bottom - rectangleF.Height;
							break;
						case ChartImageAlign.Right:
						case ChartImageAlign.Left:
						case ChartImageAlign.Center:
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

		internal Brush CreateBrush(RectangleF rect, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor)
		{
			Brush result = new SolidBrush(backColor);
			if (backImage.Length > 0 && backImageMode != ChartImageWrapMode.Unscaled && backImageMode != ChartImageWrapMode.Scaled)
			{
				result = this.GetTextureBrush(backImage, backImageTranspColor, backImageMode, backColor);
			}
			else if (backHatchStyle != 0)
			{
				result = this.GetHatchBrush(backHatchStyle, backColor, backGradientEndColor);
			}
			else if (backGradientType != 0)
			{
				result = this.GetGradientBrush(rect, backColor, backGradientEndColor, backGradientType);
			}
			return result;
		}

		public RectangleF GetRelativeRectangle(RectangleF absolute)
		{
			RectangleF empty = RectangleF.Empty;
			empty.X = (float)(absolute.X * 100.0 / (float)(this.width - 1));
			empty.Y = (float)(absolute.Y * 100.0 / (float)(this.height - 1));
			empty.Width = (float)(absolute.Width * 100.0 / (float)(this.width - 1));
			empty.Height = (float)(absolute.Height * 100.0 / (float)(this.height - 1));
			return empty;
		}

		public PointF GetRelativePoint(PointF absolute)
		{
			PointF empty = PointF.Empty;
			empty.X = (float)(absolute.X * 100.0 / (float)(this.width - 1));
			empty.Y = (float)(absolute.Y * 100.0 / (float)(this.height - 1));
			return empty;
		}

		public SizeF GetRelativeSize(SizeF size)
		{
			SizeF empty = SizeF.Empty;
			empty.Width = (float)(size.Width * 100.0 / (float)(this.width - 1));
			empty.Height = (float)(size.Height * 100.0 / (float)(this.height - 1));
			return empty;
		}

		public PointF GetAbsolutePoint(PointF relative)
		{
			PointF empty = PointF.Empty;
			empty.X = (float)(relative.X * (float)(this.width - 1) / 100.0);
			empty.Y = (float)(relative.Y * (float)(this.height - 1) / 100.0);
			return empty;
		}

		public RectangleF GetAbsoluteRectangle(RectangleF relative)
		{
			RectangleF empty = RectangleF.Empty;
			empty.X = (float)(relative.X * (float)(this.width - 1) / 100.0);
			empty.Y = (float)(relative.Y * (float)(this.height - 1) / 100.0);
			empty.Width = (float)(relative.Width * (float)(this.width - 1) / 100.0);
			empty.Height = (float)(relative.Height * (float)(this.height - 1) / 100.0);
			return empty;
		}

		public SizeF GetAbsoluteSize(SizeF relative)
		{
			SizeF empty = SizeF.Empty;
			empty.Width = (float)(relative.Width * (float)(this.width - 1) / 100.0);
			empty.Height = (float)(relative.Height * (float)(this.height - 1) / 100.0);
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

		internal void DrawRoundedRectShadowAbs(RectangleF rect, float[] cornerRadius, float radius, Color centerColor, Color surroundColor, float shadowScale)
		{
			GraphicsPath graphicsPath = this.CreateRoundedRectPath(rect, cornerRadius);
			PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
			pathGradientBrush.CenterColor = centerColor;
			Color[] array = new Color[1]
			{
				surroundColor
			};
			Color[] array3 = pathGradientBrush.SurroundColors = array;
			pathGradientBrush.CenterPoint = new PointF((float)(rect.X + rect.Width / 2.0), (float)(rect.Y + rect.Height / 2.0));
			PointF pointF2 = pathGradientBrush.FocusScales = new PointF((float)(1.0 - shadowScale * radius / rect.Width), (float)(1.0 - shadowScale * radius / rect.Height));
			base.FillPath(pathGradientBrush, graphicsPath);
			if (graphicsPath != null)
			{
				graphicsPath.Dispose();
			}
		}

		internal void Draw3DBorderRel(BorderSkinAttributes borderSkin, RectangleF rect, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle)
		{
			this.Draw3DBorderAbs(borderSkin, this.GetAbsoluteRectangle(rect), backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backGradientEndColor, borderColor, borderWidth, borderStyle);
		}

		internal void Draw3DBorderAbs(BorderSkinAttributes borderSkin, RectangleF absRect, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle)
		{
			if (this.common != null && borderSkin.SkinStyle != 0 && absRect.Width != 0.0 && absRect.Height != 0.0)
			{
				IBorderType borderType = this.common.BorderTypeRegistry.GetBorderType(borderSkin.SkinStyle.ToString());
				if (borderType != null)
				{
					borderType.Resolution = this.Graphics.DpiX;
					borderType.DrawBorder(this, borderSkin, absRect, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backGradientEndColor, borderColor, borderWidth, borderStyle);
				}
			}
		}

		internal static PieDrawingStyle GetPieDrawingStyle(DataPoint point)
		{
			PieDrawingStyle result = PieDrawingStyle.Default;
			string text = ((DataPointAttributes)point)["PieDrawingStyle"];
			if (text != null)
			{
				if (string.Compare(text, "Default", StringComparison.OrdinalIgnoreCase) != 0)
				{
					if (string.Compare(text, "SoftEdge", StringComparison.OrdinalIgnoreCase) == 0)
					{
						result = PieDrawingStyle.SoftEdge;
						goto IL_0058;
					}
					if (string.Compare(text, "Concave", StringComparison.OrdinalIgnoreCase) == 0)
					{
						result = PieDrawingStyle.Concave;
						goto IL_0058;
					}
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, "PieDrawingStyle"));
				}
				result = PieDrawingStyle.Default;
			}
			goto IL_0058;
			IL_0058:
			return result;
		}

		internal void DrawPieRel(RectangleF rect, float startAngle, float sweepAngle, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, PenAlignment penAlignment, bool shadow, double shadowOffset, bool doughnut, float doughnutRadius, bool explodedShadow, PieDrawingStyle pieDrawingStyle, out GraphicsPath controlGraphicsPath)
		{
			controlGraphicsPath = null;
			Pen pen = null;
			RectangleF absoluteRectangle = this.GetAbsoluteRectangle(rect);
			if ((double)doughnutRadius == 100.0)
			{
				doughnut = false;
			}
			Brush brush;
			if ((double)doughnutRadius != 0.0)
			{
				if (backHatchStyle != 0)
				{
					brush = this.GetHatchBrush(backHatchStyle, backColor, backGradientEndColor);
					goto IL_00da;
				}
				if (!backGradientEndColor.IsEmpty)
				{
					switch (backGradientType)
					{
					case GradientType.Center:
						break;
					default:
						goto IL_0064;
					case GradientType.None:
						goto IL_00ac;
					}
					brush = this.GetPieGradientBrush(absoluteRectangle, backColor, backGradientEndColor);
					goto IL_00da;
				}
				goto IL_00ac;
			}
			return;
			IL_00ac:
			brush = ((backImage.Length <= 0 || backImageMode == ChartImageWrapMode.Unscaled || backImageMode == ChartImageWrapMode.Scaled) ? new SolidBrush(backColor) : this.GetTextureBrush(backImage, backImageTranspColor, backImageMode, backColor));
			goto IL_00da;
			IL_0064:
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddPie(absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height, startAngle, sweepAngle);
			brush = this.GetGradientBrush(graphicsPath.GetBounds(), backColor, backGradientEndColor, backGradientType);
			if (graphicsPath != null)
			{
				graphicsPath.Dispose();
			}
			goto IL_00da;
			IL_00da:
			pen = new Pen(borderColor, (float)borderWidth);
			pen.DashStyle = this.GetPenStyle(borderStyle);
			pen.LineJoin = LineJoin.Round;
			if (doughnut)
			{
				GraphicsPath graphicsPath2 = null;
				try
				{
					graphicsPath2 = new GraphicsPath();
					graphicsPath2.AddArc((float)(absoluteRectangle.X + absoluteRectangle.Width * doughnutRadius / 200.0 - 1.0), (float)(absoluteRectangle.Y + absoluteRectangle.Height * doughnutRadius / 200.0 - 1.0), (float)(absoluteRectangle.Width - absoluteRectangle.Width * doughnutRadius / 100.0 + 2.0), (float)(absoluteRectangle.Height - absoluteRectangle.Height * doughnutRadius / 100.0 + 2.0), startAngle, sweepAngle);
					graphicsPath2.AddArc(absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height, startAngle + sweepAngle, (float)(0.0 - sweepAngle));
					graphicsPath2.CloseFigure();
					base.FillPath(brush, graphicsPath2);
					this.DrawPieGradientEffects(pieDrawingStyle, absoluteRectangle, startAngle, sweepAngle, doughnutRadius, graphicsPath2);
					if (!shadow && borderWidth > 0 && borderStyle != 0)
					{
						base.DrawPath(pen, graphicsPath2);
					}
				}
				catch
				{
					if (graphicsPath2 != null)
					{
						graphicsPath2.Dispose();
						graphicsPath2 = null;
					}
				}
				finally
				{
					controlGraphicsPath = graphicsPath2;
				}
			}
			else
			{
				if (shadow && this.softShadows)
				{
					this.DrawPieSoftShadow(shadowOffset, startAngle, sweepAngle, explodedShadow, absoluteRectangle, backColor);
				}
				else
				{
					base.shadowDrawingMode = shadow;
					base.FillPie(brush, absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height, startAngle, sweepAngle);
					base.shadowDrawingMode = false;
					this.DrawPieGradientEffects(pieDrawingStyle, absoluteRectangle, startAngle, sweepAngle, -1f, null);
				}
				if (!shadow && borderWidth > 0 && borderStyle != 0)
				{
					base.DrawPie(pen, absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height, startAngle, sweepAngle);
				}
			}
			if (pen != null)
			{
				pen.Dispose();
			}
			if (brush != null)
			{
				brush.Dispose();
			}
		}

		internal void DrawPieRel(RectangleF rect, float startAngle, float sweepAngle, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, PenAlignment penAlignment, bool shadow, double shadowOffset, bool doughnut, float doughnutRadius, bool explodedShadow, PieDrawingStyle pieDrawingStyle)
		{
			GraphicsPath graphicsPath = null;
			try
			{
				this.DrawPieRel(rect, startAngle, sweepAngle, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backGradientEndColor, borderColor, borderWidth, borderStyle, penAlignment, shadow, shadowOffset, doughnut, doughnutRadius, explodedShadow, pieDrawingStyle, out graphicsPath);
			}
			finally
			{
				if (graphicsPath != null)
				{
					graphicsPath.Dispose();
				}
			}
		}

		private void DrawPieGradientEffects(PieDrawingStyle pieDrawingStyle, RectangleF position, float startAngle, float sweepAngle, float doughnutRadius, GraphicsPath doughnutPath)
		{
			switch (pieDrawingStyle)
			{
			case PieDrawingStyle.Concave:
			{
				float num3 = Math.Min(position.Width, position.Height);
				float num4 = (float)(num3 * 0.05000000074505806);
				RectangleF rectangleF = position;
				rectangleF.Inflate((float)(0.0 - num4), (float)(0.0 - num4));
				using (GraphicsPath graphicsPath5 = new GraphicsPath())
				{
					graphicsPath5.AddEllipse(rectangleF);
					using (GraphicsPath graphicsPath6 = new GraphicsPath())
					{
						if (doughnutRadius < 0.0)
						{
							graphicsPath6.AddPie(Rectangle.Round(rectangleF), startAngle, sweepAngle);
						}
						else
						{
							graphicsPath6.AddArc((float)(rectangleF.X + position.Width * doughnutRadius / 200.0 - 1.0 - num4), (float)(rectangleF.Y + position.Height * doughnutRadius / 200.0 - 1.0 - num4), (float)(rectangleF.Width - position.Width * doughnutRadius / 100.0 + 2.0 + 2.0 * num4), (float)(rectangleF.Height - position.Height * doughnutRadius / 100.0 + 2.0 + 2.0 * num4), startAngle, sweepAngle);
							graphicsPath6.AddArc(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height, startAngle + sweepAngle, (float)(0.0 - sweepAngle));
						}
						rectangleF.Inflate(1f, 1f);
						using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(rectangleF, Color.Red, Color.Green, LinearGradientMode.Vertical))
						{
							ColorBlend colorBlend = new ColorBlend(3);
							colorBlend.Colors[0] = Color.FromArgb(100, Color.Black);
							colorBlend.Colors[1] = Color.Transparent;
							colorBlend.Colors[2] = Color.FromArgb(140, Color.White);
							colorBlend.Positions[0] = 0f;
							colorBlend.Positions[1] = 0.5f;
							colorBlend.Positions[2] = 1f;
							linearGradientBrush.InterpolationColors = colorBlend;
							base.FillPath(linearGradientBrush, graphicsPath6);
						}
					}
				}
				break;
			}
			case PieDrawingStyle.SoftEdge:
			{
				float num = Math.Min(position.Width, position.Height);
				float num2 = (float)(num / 10.0);
				if (doughnutRadius > 0.0)
				{
					num2 = (float)(num * doughnutRadius / 100.0 / 8.0);
				}
				using (GraphicsPath graphicsPath = new GraphicsPath())
				{
					graphicsPath.AddEllipse(position);
					using (GraphicsPath graphicsPath2 = new GraphicsPath())
					{
						graphicsPath2.AddArc(position.X + num2, position.Y + num2, (float)(position.Width - num2 * 2.0), (float)(position.Height - num2 * 2.0), startAngle, sweepAngle);
						graphicsPath2.AddArc(position.X, position.Y, position.Width, position.Height, startAngle + sweepAngle, (float)(0.0 - sweepAngle));
						graphicsPath2.CloseFigure();
						using (PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath))
						{
							pathGradientBrush.CenterColor = Color.Transparent;
							pathGradientBrush.SurroundColors = new Color[1]
							{
								Color.FromArgb(100, Color.Black)
							};
							Blend blend = new Blend(3);
							blend.Positions[0] = 0f;
							blend.Factors[0] = 0f;
							blend.Positions[1] = (float)(num2 / (num / 2.0));
							blend.Factors[1] = 1f;
							blend.Positions[2] = 1f;
							blend.Factors[2] = 1f;
							pathGradientBrush.Blend = blend;
							base.FillPath(pathGradientBrush, graphicsPath2);
						}
					}
					if (doughnutRadius > 0.0)
					{
						using (GraphicsPath graphicsPath3 = new GraphicsPath())
						{
							RectangleF rect = position;
							rect.Inflate((float)((0.0 - position.Width) * doughnutRadius / 200.0 + num2), (float)((0.0 - position.Height) * doughnutRadius / 200.0 + num2));
							graphicsPath3.AddEllipse(rect);
							using (GraphicsPath graphicsPath4 = new GraphicsPath())
							{
								graphicsPath4.AddArc(rect.X + num2, rect.Y + num2, (float)(rect.Width - 2.0 * num2), (float)(rect.Height - 2.0 * num2), startAngle, sweepAngle);
								graphicsPath4.AddArc(rect.X, rect.Y, rect.Width, rect.Height, startAngle + sweepAngle, (float)(0.0 - sweepAngle));
								graphicsPath4.CloseFigure();
								using (PathGradientBrush pathGradientBrush2 = new PathGradientBrush(graphicsPath3))
								{
									pathGradientBrush2.CenterColor = Color.FromArgb(100, Color.Black);
									pathGradientBrush2.SurroundColors = new Color[1]
									{
										Color.Transparent
									};
									Blend blend2 = new Blend(3);
									blend2.Positions[0] = 0f;
									blend2.Factors[0] = 0f;
									blend2.Positions[1] = (float)(num2 / (rect.Width / 2.0));
									blend2.Factors[1] = 1f;
									blend2.Positions[2] = 1f;
									blend2.Factors[2] = 1f;
									pathGradientBrush2.Blend = blend2;
									base.FillPath(pathGradientBrush2, graphicsPath4);
								}
							}
						}
					}
				}
				break;
			}
			}
		}

		private void DrawPieSoftShadow(double shadowOffset, float startAngle, float sweepAngle, bool explodedShadow, RectangleF absRect, Color backColor)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddEllipse(absRect.X, absRect.Y, absRect.Width, absRect.Height);
			PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
			Color[] colors = new Color[3]
			{
				Color.FromArgb(0, backColor),
				Color.FromArgb(backColor.A, backColor),
				Color.FromArgb(backColor.A, backColor)
			};
			float[] positions = new float[3]
			{
				0f,
				0.05f,
				1f
			};
			ColorBlend colorBlend = new ColorBlend();
			colorBlend.Colors = colors;
			colorBlend.Positions = positions;
			pathGradientBrush.InterpolationColors = colorBlend;
			base.shadowDrawingMode = true;
			base.FillPie(pathGradientBrush, absRect.X, absRect.Y, absRect.Width, absRect.Height, startAngle, sweepAngle);
			base.shadowDrawingMode = false;
		}

		internal void DrawArrowRel(PointF position, ArrowOrientation orientation, ArrowsType type, Color color, int lineWidth, ChartDashStyle lineDashStyle, double shift, double size)
		{
			if (type != 0)
			{
				SolidBrush solidBrush = new SolidBrush(color);
				PointF pointF = PointF.Empty;
				PointF absolutePoint = this.GetAbsolutePoint(position);
				switch (type)
				{
				case ArrowsType.Triangle:
				{
					PointF[] arrowShape = this.GetArrowShape(absolutePoint, orientation, shift, size, lineWidth, type, ref pointF);
					pointF = this.GetRelativePoint(pointF);
					this.DrawLineRel(color, lineWidth, lineDashStyle, position, pointF);
					base.FillPolygon(solidBrush, arrowShape);
					break;
				}
				case ArrowsType.SharpTriangle:
				{
					PointF[] arrowShape = this.GetArrowShape(absolutePoint, orientation, shift, size, lineWidth, type, ref pointF);
					pointF = this.GetRelativePoint(pointF);
					this.DrawLineRel(color, lineWidth, lineDashStyle, position, pointF);
					base.FillPolygon(solidBrush, arrowShape);
					break;
				}
				case ArrowsType.Lines:
				{
					PointF[] arrowShape = this.GetArrowShape(absolutePoint, orientation, shift, size, lineWidth, type, ref pointF);
					arrowShape[0] = this.GetRelativePoint(arrowShape[0]);
					arrowShape[1] = this.GetRelativePoint(arrowShape[1]);
					arrowShape[2] = this.GetRelativePoint(arrowShape[2]);
					pointF = this.GetRelativePoint(pointF);
					this.DrawLineRel(color, lineWidth, lineDashStyle, position, pointF);
					this.DrawLineRel(color, lineWidth, lineDashStyle, arrowShape[0], arrowShape[2]);
					this.DrawLineRel(color, lineWidth, lineDashStyle, arrowShape[1], arrowShape[2]);
					break;
				}
				}
				if (solidBrush != null)
				{
					solidBrush.Dispose();
				}
			}
		}

		private PointF[] GetArrowShape(PointF position, ArrowOrientation orientation, double shift, double size, int lineWidth, ArrowsType type, ref PointF endPoint)
		{
			PointF[] array = new PointF[3];
			switch (orientation)
			{
			case ArrowOrientation.Top:
			{
				size = (double)this.GetAbsoluteSize(new SizeF((float)size, (float)size)).Width;
				shift = (double)this.GetAbsoluteSize(new SizeF((float)shift, (float)shift)).Height;
				double num = (type != ArrowsType.SharpTriangle) ? (size * 2.0) : (size * 4.0);
				array[0].X = position.X - (float)size;
				array[0].Y = position.Y - (float)shift;
				array[1].X = position.X + (float)size;
				array[1].Y = position.Y - (float)shift;
				array[2].X = position.X;
				array[2].Y = position.Y - (float)shift - (float)num;
				endPoint.X = position.X;
				if (type == ArrowsType.SharpTriangle || type == ArrowsType.Triangle)
				{
					endPoint.Y = array[1].Y;
				}
				else
				{
					endPoint.Y = array[2].Y;
				}
				break;
			}
			case ArrowOrientation.Bottom:
			{
				size = (double)this.GetAbsoluteSize(new SizeF((float)size, (float)size)).Width;
				shift = (double)this.GetAbsoluteSize(new SizeF((float)shift, (float)shift)).Height;
				double num = (type != ArrowsType.SharpTriangle) ? (size * 2.0) : (size * 4.0);
				array[0].X = position.X - (float)size;
				array[0].Y = position.Y + (float)shift;
				array[1].X = position.X + (float)size;
				array[1].Y = position.Y + (float)shift;
				array[2].X = position.X;
				array[2].Y = position.Y + (float)shift + (float)num;
				endPoint.X = position.X;
				if (type == ArrowsType.SharpTriangle || type == ArrowsType.Triangle)
				{
					endPoint.Y = array[1].Y;
				}
				else
				{
					endPoint.Y = array[2].Y;
				}
				break;
			}
			case ArrowOrientation.Left:
			{
				size = (double)this.GetAbsoluteSize(new SizeF((float)size, (float)size)).Width;
				shift = (double)this.GetAbsoluteSize(new SizeF((float)shift, (float)shift)).Width;
				double num = (type != ArrowsType.SharpTriangle) ? (size * 2.0) : (size * 4.0);
				array[0].Y = position.Y - (float)size;
				array[0].X = position.X - (float)shift;
				array[1].Y = position.Y + (float)size;
				array[1].X = position.X - (float)shift;
				array[2].Y = position.Y;
				array[2].X = position.X - (float)shift - (float)num;
				endPoint.Y = position.Y;
				if (type == ArrowsType.SharpTriangle || type == ArrowsType.Triangle)
				{
					endPoint.X = array[1].X;
				}
				else
				{
					endPoint.X = array[2].X;
				}
				break;
			}
			case ArrowOrientation.Right:
			{
				size = (double)this.GetAbsoluteSize(new SizeF((float)size, (float)size)).Width;
				shift = (double)this.GetAbsoluteSize(new SizeF((float)shift, (float)shift)).Width;
				double num = (type != ArrowsType.SharpTriangle) ? (size * 2.0) : (size * 4.0);
				array[0].Y = position.Y - (float)size;
				array[0].X = position.X + (float)shift;
				array[1].Y = position.Y + (float)size;
				array[1].X = position.X + (float)shift;
				array[2].Y = position.Y;
				array[2].X = position.X + (float)shift + (float)num;
				endPoint.Y = position.Y;
				if (type == ArrowsType.SharpTriangle || type == ArrowsType.Triangle)
				{
					endPoint.X = array[1].X;
				}
				else
				{
					endPoint.X = array[2].X;
				}
				break;
			}
			}
			return array;
		}

		internal static void Widen(GraphicsPath path, Pen pen)
		{
			try
			{
				path.Widen(pen);
			}
			catch (ThreadAbortException)
			{
				Thread.ResetAbort();
			}
			catch (OutOfMemoryException)
			{
			}
			catch (ArgumentException)
			{
			}
		}

		internal static BarDrawingStyle GetBarDrawingStyle(DataPoint point)
		{
			BarDrawingStyle result = BarDrawingStyle.Default;
			string text = ((DataPointAttributes)point)["DrawingStyle"];
			if (text != null)
			{
				if (string.Compare(text, "Default", StringComparison.OrdinalIgnoreCase) != 0)
				{
					if (string.Compare(text, "Cylinder", StringComparison.OrdinalIgnoreCase) == 0)
					{
						result = BarDrawingStyle.Cylinder;
						goto IL_007c;
					}
					if (string.Compare(text, "Emboss", StringComparison.OrdinalIgnoreCase) == 0)
					{
						result = BarDrawingStyle.Emboss;
						goto IL_007c;
					}
					if (string.Compare(text, "LightToDark", StringComparison.OrdinalIgnoreCase) == 0)
					{
						result = BarDrawingStyle.LightToDark;
						goto IL_007c;
					}
					if (string.Compare(text, "Wedge", StringComparison.OrdinalIgnoreCase) == 0)
					{
						result = BarDrawingStyle.Wedge;
						goto IL_007c;
					}
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, "DrawingStyle"));
				}
				result = BarDrawingStyle.Default;
			}
			goto IL_007c;
			IL_007c:
			return result;
		}

		internal RectangleF Round(RectangleF rect)
		{
			float num = (float)Math.Round((double)rect.Left);
			float num2 = (float)Math.Round((double)rect.Right);
			float num3 = (float)Math.Round((double)rect.Top);
			float num4 = (float)Math.Round((double)rect.Bottom);
			return new RectangleF(num, num3, num2 - num, num4 - num3);
		}

		public double GetPositionFromAxis(string chartAreaName, AxisName axis, double axisValue)
		{
			switch (axis)
			{
			case AxisName.X:
				return this.common.ChartPicture.ChartAreas[chartAreaName].AxisX.GetLinearPosition(axisValue);
			case AxisName.X2:
				return this.common.ChartPicture.ChartAreas[chartAreaName].AxisX2.GetLinearPosition(axisValue);
			case AxisName.Y:
				return this.common.ChartPicture.ChartAreas[chartAreaName].AxisY.GetLinearPosition(axisValue);
			case AxisName.Y2:
				return this.common.ChartPicture.ChartAreas[chartAreaName].AxisY2.GetLinearPosition(axisValue);
			default:
				return 0.0;
			}
		}

		internal void SetPictureSize(int width, int height)
		{
			this.width = width;
			this.height = height;
		}

		internal ChartGraphics(CommonElements common)
		{
			this.common = common;
			this.pen = new Pen(Color.Black);
			this.solidBrush = new SolidBrush(Color.Black);
		}

		internal void Dispose()
		{
			if (this.pen != null)
			{
				this.pen.Dispose();
			}
			if (this.solidBrush != null)
			{
				this.solidBrush.Dispose();
			}
		}

		internal new void SetClip(RectangleF region)
		{
			base.SetClip(this.GetAbsoluteRectangle(region));
		}

		internal void SetClipAbs(RectangleF region)
		{
			base.SetClip(region);
		}

		internal void StartAnimation()
		{
		}

		internal void StopAnimation()
		{
		}

		public static Color GetGradientColor(Color beginColor, Color endColor, double relativePosition)
		{
			if (!(relativePosition < 0.0) && !(relativePosition > 1.0) && !double.IsNaN(relativePosition))
			{
				int r = beginColor.R;
				int g = beginColor.G;
				int b = beginColor.B;
				int r2 = endColor.R;
				int g2 = endColor.G;
				int b2 = endColor.B;
				double num = (double)r + (double)(r2 - r) * relativePosition;
				double num2 = (double)g + (double)(g2 - g) * relativePosition;
				double num3 = (double)b + (double)(b2 - b) * relativePosition;
				if (num > 255.0)
				{
					num = 255.0;
				}
				if (num < 0.0)
				{
					num = 0.0;
				}
				if (num2 > 255.0)
				{
					num2 = 255.0;
				}
				if (num2 < 0.0)
				{
					num2 = 0.0;
				}
				if (num3 > 255.0)
				{
					num3 = 255.0;
				}
				if (num3 < 0.0)
				{
					num3 = 0.0;
				}
				return Color.FromArgb(beginColor.A, (int)num, (int)num2, (int)num3);
			}
			return beginColor;
		}

		private GraphicsPath GetLabelBackgroundGraphicsPath(RectangleF backPosition, int rotationAngle)
		{
			RectangleF rect = this.Round(this.GetAbsoluteRectangle(backPosition));
			PointF point = new PointF((float)(rect.X + rect.Width / 2.0), (float)(rect.Y + rect.Height / 2.0));
			this.myMatrix = base.Transform.Clone();
			this.myMatrix.RotateAt((float)rotationAngle, point);
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(rect);
			graphicsPath.Transform(this.myMatrix);
			return graphicsPath;
		}

		public bool CanLabelFitInSlice(GraphicsPath sliceGraphicsPath, RectangleF labelRelativeRect, int labelRotationAngle)
		{
			if (sliceGraphicsPath == null)
			{
				return false;
			}
			using (GraphicsPath path = this.GetLabelBackgroundGraphicsPath(labelRelativeRect, labelRotationAngle))
			{
				return sliceGraphicsPath.IsSuperSetOf(path, this.Graphics);
			}
		}

		public void DrawLabelBackground(int angle, PointF textPosition, RectangleF backPosition, Color backColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle)
		{
			RectangleF rect = this.Round(this.GetAbsoluteRectangle(backPosition));
			PointF empty = PointF.Empty;
			empty = ((!textPosition.IsEmpty) ? this.GetAbsolutePoint(textPosition) : new PointF((float)(rect.X + rect.Width / 2.0), (float)(rect.Y + rect.Height / 2.0)));
			this.myMatrix = base.Transform.Clone();
			this.myMatrix.RotateAt((float)angle, empty);
			GraphicsState gstate = base.Save();
			base.Transform = this.myMatrix;
			if (!backColor.IsEmpty || !borderColor.IsEmpty)
			{
				using (Brush brush = new SolidBrush(backColor))
				{
					base.FillRectangle(brush, rect);
				}
				if (borderWidth > 0 && !borderColor.IsEmpty && borderStyle != 0)
				{
					AntiAliasingTypes antiAliasingTypes = this.AntiAliasing;
					try
					{
						this.AntiAliasing = AntiAliasingTypes.None;
						using (Pen pen = new Pen(borderColor, (float)borderWidth))
						{
							pen.DashStyle = this.GetPenStyle(borderStyle);
							base.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
						}
					}
					finally
					{
						this.AntiAliasing = antiAliasingTypes;
					}
				}
			}
			else
			{
				using (Brush brush2 = new SolidBrush(Color.Transparent))
				{
					base.FillRectangle(brush2, rect);
				}
			}
			base.Restore(gstate);
		}

		public void MapCategoryNodeLabel(CommonElements common, CategoryNode node, RectangleF backPosition)
		{
			if (common != null && common.ProcessModeRegions)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				RectangleF rect = this.Round(this.GetAbsoluteRectangle(backPosition));
				graphicsPath.AddRectangle(rect);
				graphicsPath.Transform(this.myMatrix);
				common.HotRegionsList.AddHotRegion(this, graphicsPath, false, node.LabelToolTip, node.LabelHref, "", node, ChartElementType.Nothing);
				if (common.HotRegionsList.List != null)
				{
					((HotRegion)common.HotRegionsList.List[common.HotRegionsList.List.Count - 1]).Type = ChartElementType.Nothing;
				}
			}
		}

		public float GetAbsoluteWidth(float widthRelative)
		{
			return (float)(widthRelative * (float)(this.width - 1) / 100.0);
		}
	}
}
