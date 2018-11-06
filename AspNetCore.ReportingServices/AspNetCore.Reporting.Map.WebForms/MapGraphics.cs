using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class MapGraphics : RenderingEngine
	{
		internal CommonElements common;

		private Pen pen;

		private SolidBrush solidBrush;

		private Matrix myMatrix;

		private int width;

		private int height;

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

		internal float ScaleFactorX
		{
			get
			{
				return this.Graphics.Transform.Elements[0];
			}
		}

		internal float ScaleFactorY
		{
			get
			{
				return this.Graphics.Transform.Elements[3];
			}
		}

		internal MapGraphics(CommonElements common)
		{
			if (common != null)
			{
				this.common = common;
				common.Graph = this;
			}
			this.pen = new Pen(Color.Black);
			this.solidBrush = new SolidBrush(Color.Black);
		}

		internal void DrawLineRel(Color color, int width, MapDashStyle style, PointF firstPointF, PointF secondPointF)
		{
			this.DrawLineAbs(color, width, style, this.GetAbsolutePoint(firstPointF), this.GetAbsolutePoint(secondPointF));
		}

		internal void DrawLineAbs(Color color, int width, MapDashStyle style, PointF firstPoint, PointF secondPoint)
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
				if (this.pen.DashStyle != MapGraphics.GetPenStyle(style))
				{
					this.pen.DashStyle = MapGraphics.GetPenStyle(style);
				}
				SmoothingMode smoothingMode = base.SmoothingMode;
				if (width <= 1 && style != MapDashStyle.Solid && (firstPoint.X == secondPoint.X || firstPoint.Y == secondPoint.Y))
				{
					base.SmoothingMode = SmoothingMode.Default;
				}
				base.DrawLine(this.pen, (float)Math.Round((double)firstPoint.X), (float)Math.Round((double)firstPoint.Y), (float)Math.Round((double)secondPoint.X), (float)Math.Round((double)secondPoint.Y));
				base.SmoothingMode = smoothingMode;
			}
		}

		internal void DrawLineRel(Color color, int width, MapDashStyle style, PointF firstPoint, PointF secondPoint, Color shadowColor, int shadowOffset)
		{
			this.DrawLineAbs(color, width, style, this.GetAbsolutePoint(firstPoint), this.GetAbsolutePoint(secondPoint), shadowColor, shadowOffset);
		}

		internal void DrawLineAbs(Color color, int width, MapDashStyle style, PointF firstPoint, PointF secondPoint, Color shadowColor, int shadowOffset)
		{
			Color color2 = (shadowColor.A == 255) ? Color.FromArgb((int)color.A / 2, shadowColor) : shadowColor;
			PointF firstPoint2 = new PointF(firstPoint.X + (float)shadowOffset, firstPoint.Y + (float)shadowOffset);
			PointF secondPoint2 = new PointF(secondPoint.X + (float)shadowOffset, secondPoint.Y + (float)shadowOffset);
			base.shadowDrawingMode = true;
			this.DrawLineAbs(color2, width, style, firstPoint2, secondPoint2);
			base.shadowDrawingMode = false;
			this.DrawLineAbs(color, width, style, firstPoint, secondPoint);
		}

		internal static Brush GetHatchBrush(MapHatchStyle hatchStyle, Color backColor, Color foreColor)
		{
			HatchStyle hatchstyle = (HatchStyle)Enum.Parse(typeof(HatchStyle), ((Enum)(object)hatchStyle).ToString((IFormatProvider)CultureInfo.InvariantCulture));
			return new HatchBrush(hatchstyle, foreColor, backColor);
		}

		internal Brush GetTextureBrush(string name, Color backImageTranspColor, MapImageWrapMode mode)
		{
			Image image = this.common.ImageLoader.LoadImage(name);
			ImageAttributes imageAttributes = new ImageAttributes();
			imageAttributes.SetWrapMode((WrapMode)((mode == MapImageWrapMode.Unscaled) ? MapImageWrapMode.Scaled : mode));
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
			int alpha = (int)(255.0 * this.common.MapCore.ShadowIntensity / 100.0);
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
				case GradientType.RightLeft:
				{
					type = GradientType.LeftRight;
					Color color = firstColor;
					firstColor = secondColor;
					secondColor = color;
					break;
				}
				case GradientType.BottomTop:
				{
					type = GradientType.TopBottom;
					Color color = firstColor;
					firstColor = secondColor;
					secondColor = color;
					break;
				}
				case GradientType.ReversedCenter:
				{
					type = GradientType.Center;
					Color color = firstColor;
					firstColor = secondColor;
					secondColor = color;
					break;
				}
				case GradientType.ReversedDiagonalLeft:
				{
					type = GradientType.DiagonalLeft;
					Color color = firstColor;
					firstColor = secondColor;
					secondColor = color;
					break;
				}
				case GradientType.ReversedDiagonalRight:
				{
					type = GradientType.DiagonalRight;
					Color color = firstColor;
					firstColor = secondColor;
					secondColor = color;
					break;
				}
				case GradientType.ReversedHorizontalCenter:
				{
					type = GradientType.HorizontalCenter;
					Color color = firstColor;
					firstColor = secondColor;
					secondColor = color;
					break;
				}
				case GradientType.ReversedVerticalCenter:
				{
					type = GradientType.VerticalCenter;
					Color color = firstColor;
					firstColor = secondColor;
					secondColor = color;
					break;
				}
				}
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
				LinearGradientBrush linearGradientBrush = null;
				RectangleF rect = new RectangleF(0f, 0f, rectangle.Width, rectangle.Height);
				switch (type)
				{
				case GradientType.HorizontalCenter:
					rect.Height /= 2f;
					linearGradientBrush = new LinearGradientBrush(rect, firstColor, secondColor, angle);
					linearGradientBrush.WrapMode = WrapMode.TileFlipX;
					break;
				case GradientType.VerticalCenter:
					rect.Width /= 2f;
					linearGradientBrush = new LinearGradientBrush(rect, firstColor, secondColor, angle);
					linearGradientBrush.WrapMode = WrapMode.TileFlipX;
					break;
				default:
					linearGradientBrush = new LinearGradientBrush(rect, firstColor, secondColor, angle);
					break;
				}
				linearGradientBrush.TranslateTransform(rectangle.X, rectangle.Y, MatrixOrder.Append);
				return linearGradientBrush;
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

		internal Brush CreateBrush(RectangleF rect, Color backColor, MapHatchStyle backHatchStyle, string backImage, MapImageWrapMode backImageMode, Color backImageTranspColor, MapImageAlign backImageAlign, GradientType backGradientType, Color backSecondaryColor)
		{
			Brush brush = new SolidBrush(backColor);
			if (!string.IsNullOrEmpty(backImage) && backImageMode != MapImageWrapMode.Unscaled && backImageMode != MapImageWrapMode.Scaled)
			{
				return this.GetTextureBrush(backImage, backImageTranspColor, backImageMode);
			}
			if (backHatchStyle != 0)
			{
				return MapGraphics.GetHatchBrush(backHatchStyle, backColor, backSecondaryColor);
			}
			if (backGradientType != 0)
			{
				return this.GetGradientBrush(rect, backColor, backSecondaryColor, backGradientType);
			}
			return new SolidBrush(backColor);
		}

		internal static DashStyle GetPenStyle(MapDashStyle style)
		{
			switch (style)
			{
			case MapDashStyle.Dash:
				return DashStyle.Dash;
			case MapDashStyle.DashDot:
				return DashStyle.DashDot;
			case MapDashStyle.DashDotDot:
				return DashStyle.DashDotDot;
			case MapDashStyle.Dot:
				return DashStyle.Dot;
			default:
				return DashStyle.Solid;
			}
		}

		internal GraphicsPath WidenPath(GraphicsPath path, float amount)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			GraphicsPath graphicsPath2 = (GraphicsPath)path.Clone();
			GraphicsPath graphicsPath3 = (GraphicsPath)path.Clone();
			try
			{
				using (Pen pen = new Pen(Color.Empty, amount))
				{
					pen.LineJoin = LineJoin.Round;
					graphicsPath2.Widen(pen, null, 0.3f);
				}
				graphicsPath2.Flatten(null, 0.25f);
				using (Pen pen2 = new Pen(Color.Empty, (float)(amount * 0.89999997615814209)))
				{
					pen2.LineJoin = LineJoin.Round;
					graphicsPath3.Widen(pen2, null, 0.3f);
				}
				graphicsPath3.Flatten(null, 0.25f);
				ArrayList arrayList = new ArrayList();
				PointF[] pathPoints = graphicsPath2.PathPoints;
				foreach (PointF pointF in pathPoints)
				{
					if (!path.IsVisible(pointF) && !graphicsPath3.IsVisible(pointF))
					{
						arrayList.Add(pointF);
					}
				}
				graphicsPath.AddPolygon((PointF[])arrayList.ToArray(typeof(PointF)));
				return graphicsPath;
			}
			finally
			{
				graphicsPath2.Dispose();
				graphicsPath3.Dispose();
			}
		}

		internal GraphicsPath Union(GraphicsPath pathA, GraphicsPath pathB)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			pathA.Flatten();
			pathB.Flatten();
			int firstPointOf1Inside = this.GetFirstPointOf1Inside2(pathA, pathB);
			if (firstPointOf1Inside == -1)
			{
				return (GraphicsPath)pathA.Clone();
			}
			int firstPointOf1Inside2 = this.GetFirstPointOf1Inside2(pathB, pathA);
			if (firstPointOf1Inside2 == -1)
			{
				return (GraphicsPath)pathB.Clone();
			}
			GraphicsPath[] array = this.SplitIntoSegments(pathA, firstPointOf1Inside, pathB);
			GraphicsPath[] array2 = this.SplitIntoSegments(pathB, firstPointOf1Inside2, pathA);
			int num = 0;
			int num2 = 0;
			while (num < array.Length && num2 < array2.Length)
			{
				graphicsPath.AddPath(array[num], true);
				graphicsPath.AddPath(array2[num2], true);
			}
			return graphicsPath;
		}

		private GraphicsPath[] SplitIntoSegments(GraphicsPath pathToBeSplit, int startingPoint, GraphicsPath splitterPath)
		{
			if (startingPoint == -1)
			{
				return null;
			}
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = null;
			for (int i = startingPoint + 1; i != startingPoint; i++)
			{
				if (i == pathToBeSplit.PointCount)
				{
					i = 0;
				}
				if (!splitterPath.IsVisible(pathToBeSplit.PathPoints[i]))
				{
					if (arrayList2 == null)
					{
						arrayList2 = new ArrayList();
					}
					arrayList2.Add(pathToBeSplit.PathPoints[i]);
				}
				else if (arrayList2 != null)
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					graphicsPath.AddPolygon((PointF[])arrayList2.ToArray(typeof(PointF)));
					arrayList.Add(graphicsPath);
					arrayList2.RemoveRange(0, arrayList2.Count);
					arrayList2 = null;
				}
			}
			return (GraphicsPath[])arrayList.ToArray(typeof(GraphicsPath));
		}

		private int GetFirstPointOf1Inside2(GraphicsPath path1, GraphicsPath path2)
		{
			for (int i = 0; i < path1.PathPoints.Length; i++)
			{
				if (path2.IsVisible(path1.PathPoints[i]))
				{
					return i;
				}
			}
			return -1;
		}

		internal Brush GetMarkerBrush(GraphicsPath path, MarkerStyle markerStyle, PointF pointOrigin, float angle, Color fillColor, GradientType fillGradientType, Color fillSecondaryColor, MapHatchStyle fillHatchStyle)
		{
			Brush brush = null;
			if (fillHatchStyle != 0)
			{
				brush = MapGraphics.GetHatchBrush(fillHatchStyle, fillSecondaryColor, fillColor);
			}
			else if (fillGradientType != 0)
			{
				RectangleF bounds = path.GetBounds();
				if (markerStyle == MarkerStyle.Circle && fillGradientType == GradientType.DiagonalLeft)
				{
					brush = this.GetGradientBrush(bounds, fillColor, fillSecondaryColor, GradientType.LeftRight);
					Matrix matrix = new Matrix();
					matrix.RotateAt(45f, new PointF((float)(bounds.X + bounds.Width / 2.0), (float)(bounds.Y + bounds.Height / 2.0)));
					((LinearGradientBrush)brush).Transform = matrix;
				}
				else if (markerStyle == MarkerStyle.Circle && fillGradientType == GradientType.DiagonalRight)
				{
					brush = this.GetGradientBrush(bounds, fillColor, fillSecondaryColor, GradientType.TopBottom);
					Matrix matrix2 = new Matrix();
					matrix2.RotateAt(135f, new PointF((float)(bounds.X + bounds.Width / 2.0), (float)(bounds.Y + bounds.Height / 2.0)));
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
							fillSecondaryColor
						};
						brush = pathGradientBrush;
					}
				}
				else
				{
					brush = this.GetGradientBrush(path.GetBounds(), fillColor, fillSecondaryColor, fillGradientType);
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
			{
				PointF[] array = new PointF[4];
				array[0].X = point.X;
				array[0].Y = point.Y;
				array[1].X = (float)(point.X + 1.0);
				array[1].Y = point.Y;
				array[2].X = (float)(point.X + 1.0);
				array[2].Y = (float)(point.Y + 1.0);
				array[3].X = point.X;
				array[3].Y = (float)(point.Y + 1.0);
				graphicsPath.AddPolygon(array);
				break;
			}
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
					float num24 = (float)Math.Pow(Math.Pow((double)empty.Width, 2.0) - empty.Width / 2.0 * (empty.Width / 2.0), 0.5);
					PointF[] array = new PointF[5];
					array[0].X = empty.X;
					array[0].Y = empty.Y + num24;
					array[1].X = (float)(empty.X + empty.Width / 2.0);
					array[1].Y = empty.Y;
					array[2].X = empty.X + empty.Width;
					array[2].Y = empty.Y + num24;
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
				float num21 = (float)Math.Cos(0.62831853071795862);
				float num22 = (float)Math.Sin(1.2566370614359172);
				float num23 = (float)Math.Sin(2.5132741228718345);
				PointF[] array = new PointF[5];
				array[0].X = 0f;
				array[0].Y = 1f;
				array[1].X = num22;
				array[1].Y = y;
				array[2].X = num23;
				array[2].Y = (float)(0.0 - num21);
				array[3].X = (float)(0.0 - num23);
				array[3].Y = (float)(0.0 - num21);
				array[4].X = (float)(0.0 - num22);
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
			case MarkerStyle.PushPin:
			{
				PointF[] array = new PointF[11];
				float num = (float)(empty.Width / 5.0);
				float num2 = (float)(empty.Height / 5.0);
				float num3 = (float)(empty.X + empty.Width / 2.0);
				float num4 = (float)(empty.Y + empty.Height / 2.0);
				array[0].X = (float)(num3 - 3.0 * num);
				array[1].X = (float)(num3 + 3.0 * num);
				array[2].X = (float)(num3 + 1.5 * num);
				array[10].X = (float)(num3 - 1.5 * num);
				array[3].X = (float)(num3 + 2.5 * num);
				array[9].X = (float)(num3 - 2.5 * num);
				array[4].X = (float)(num3 + 4.5 * num);
				array[8].X = (float)(num3 - 4.5 * num);
				array[5].X = (float)(num3 + 1.0 * num);
				array[7].X = (float)(num3 - 1.0 * num);
				array[6].X = num3;
				ref PointF val = ref array[0];
				ref PointF val2 = ref array[1];
				float num7 = val.Y = (val2.Y = (float)(num4 - 20.0 * num2));
				ref PointF val3 = ref array[2];
				ref PointF val4 = ref array[10];
				float num10 = val3.Y = (val4.Y = (float)(num4 - 18.0 * num2));
				ref PointF val5 = ref array[3];
				ref PointF val6 = ref array[9];
				float num13 = val5.Y = (val6.Y = (float)(num4 - 12.0 * num2));
				ref PointF val7 = ref array[4];
				ref PointF val8 = ref array[5];
				ref PointF val9 = ref array[7];
				ref PointF val10 = ref array[8];
				float num15 = val10.Y = (float)(num4 - 10.0 * num2);
				float num17 = val9.Y = num15;
				float num20 = val7.Y = (val8.Y = num17);
				array[6].Y = num4;
				PointF[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					PointF pointF = array2[i];
					point.X = (float)Math.Round((double)point.X);
					point.Y = (float)Math.Round((double)point.Y);
				}
				graphicsPath.AddPolygon(array);
				break;
			}
			default:
				throw new InvalidOperationException(SR.invalid_marker_type);
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

		internal void DrawMarkerRel(PointF point, MarkerStyle markerStyle, int markerSize, Color markerColor, GradientType markerGradientType, MapHatchStyle markerHatchStyle, Color markerSecondaryColor, MapDashStyle markerBorderStyle, Color markerBorderColor, int markerBorderSize, string markerImage, Color markerImageTranspColor, int shadowSize, Color shadowColor, RectangleF imageScaleRect)
		{
			this.DrawMarkerAbs(this.GetAbsolutePoint(point), markerStyle, markerSize, markerColor, markerGradientType, markerHatchStyle, markerSecondaryColor, markerBorderStyle, markerBorderColor, markerBorderSize, markerImage, markerImageTranspColor, shadowSize, shadowColor, imageScaleRect, false, 0f);
		}

		internal void DrawMarkerAbs(PointF point, MarkerStyle markerStyle, int markerSize, Color markerColor, GradientType markerGradientType, MapHatchStyle markerHatchStyle, Color markerSecondaryColor, MapDashStyle markerBorderStyle, Color markerBorderColor, int markerBorderSize, string markerImage, Color markerImageTranspColor, int shadowSize, Color shadowColor, RectangleF imageScaleRect, bool forceAntiAlias, float angle)
		{
			if (!string.IsNullOrEmpty(markerImage))
			{
				Image image = this.common.ImageLoader.LoadImage(markerImage);
				RectangleF empty = RectangleF.Empty;
				if (imageScaleRect == RectangleF.Empty)
				{
					imageScaleRect.Height = (float)image.Height;
					imageScaleRect.Width = (float)image.Width;
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
			else if (markerStyle != 0 && markerSize > 0)
			{
				SmoothingMode smoothingMode = base.SmoothingMode;
				if (forceAntiAlias)
				{
					base.SmoothingMode = SmoothingMode.AntiAlias;
				}
				RectangleF empty2 = RectangleF.Empty;
				empty2.X = (float)(point.X - (float)markerSize / 2.0);
				empty2.Y = (float)(point.Y - (float)markerSize / 2.0);
				empty2.Width = (float)markerSize;
				empty2.Height = (float)markerSize;
				Brush brush = null;
				GraphicsPath graphicsPath = null;
				Pen pen = null;
				Brush brush2 = null;
				try
				{
					brush = this.CreateBrush(empty2, markerColor, markerHatchStyle, string.Empty, MapImageWrapMode.Unscaled, Color.Empty, MapImageAlign.Center, markerGradientType, markerSecondaryColor);
					if (markerBorderSize > 0)
					{
						pen = new Pen(markerBorderColor, (float)markerBorderSize);
						pen.DashStyle = MapGraphics.GetPenStyle(markerBorderStyle);
						pen.Alignment = PenAlignment.Center;
						pen.LineJoin = LineJoin.Round;
					}
					if (shadowSize > 0)
					{
						brush2 = this.GetShadowBrush();
					}
					graphicsPath = this.CreateMarker(point, (float)markerSize, (float)markerSize, markerStyle);
					if (brush2 != null)
					{
						using (Matrix matrix = new Matrix())
						{
							matrix.Translate((float)shadowSize, (float)shadowSize);
							graphicsPath.Transform(matrix);
							base.FillPath(brush2, graphicsPath);
							matrix.Reset();
							matrix.Translate((float)(-shadowSize), (float)(-shadowSize));
							graphicsPath.Transform(matrix);
						}
					}
					base.FillPath(brush, graphicsPath);
					if (pen != null)
					{
						base.DrawPath(pen, graphicsPath);
					}
				}
				finally
				{
					if (brush != null)
					{
						brush.Dispose();
					}
					if (graphicsPath != null)
					{
						graphicsPath.Dispose();
					}
					if (pen != null)
					{
						pen.Dispose();
					}
					if (brush2 != null)
					{
						brush2.Dispose();
					}
				}
				if (forceAntiAlias)
				{
					base.SmoothingMode = smoothingMode;
				}
			}
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

		internal void DrawStringAbs(string text, Font font, Brush brush, PointF absPosition, StringFormat format, int angle)
		{
			this.myMatrix = base.Transform.Clone();
			this.myMatrix.RotateAt((float)angle, absPosition);
			GraphicsState gstate = base.Save();
			base.Transform = this.myMatrix;
			base.DrawString(text, font, brush, absPosition, format);
			base.Restore(gstate);
		}

		internal void DrawStringAbs(string text, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
		{
			if (layoutRectangle.Width != 0.0 && layoutRectangle.Height != 0.0)
			{
				base.DrawString(text, font, brush, layoutRectangle, format);
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

		internal void DrawStringRel(string text, Font font, Brush brush, PointF position, StringFormat format, int angle)
		{
			this.DrawStringAbs(text, font, brush, this.GetAbsolutePoint(position), format, angle);
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

		internal void FillRectangleRel(RectangleF rectF, Color backColor, MapHatchStyle backHatchStyle, string backImage, MapImageWrapMode backImageMode, Color backImageTranspColor, MapImageAlign backImageAlign, GradientType backGradientType, Color backSecondaryColor, Color borderColor, int borderWidth, MapDashStyle borderStyle, Color shadowColor, int shadowOffset, PenAlignment penAlignment)
		{
			this.FillRectangleRel(rectF, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backSecondaryColor, borderColor, borderWidth, borderStyle, shadowColor, shadowOffset, penAlignment, false, 0, false);
		}

		internal void FillRectangleRel(RectangleF rectF, Color backColor, MapHatchStyle backHatchStyle, string backImage, MapImageWrapMode backImageMode, Color backImageTranspColor, MapImageAlign backImageAlign, GradientType backGradientType, Color backSecondaryColor, Color borderColor, int borderWidth, MapDashStyle borderStyle, Color shadowColor, int shadowOffset, PenAlignment penAlignment, bool circular, int circularSectorsCount, bool circle3D)
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
			if (backSecondaryColor.IsEmpty)
			{
				backSecondaryColor = Color.White;
			}
			if (borderColor.IsEmpty)
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
			rectangleF = MapGraphics.Round(rectangleF);
			RectangleF rectangleF2 = (penAlignment != PenAlignment.Inset || borderWidth <= 0) ? rectangleF : ((base.ActiveRenderingType == RenderingType.Svg || this.IsMetafile) ? new RectangleF(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height) : new RectangleF(rectangleF.X + (float)borderWidth, rectangleF.Y + (float)borderWidth, (float)(rectangleF.Width - (float)borderWidth * 2.0 + 1.0), (float)(rectangleF.Height - (float)borderWidth * 2.0 + 1.0)));
			if (!string.IsNullOrEmpty(backImage) && backImageMode != MapImageWrapMode.Unscaled && backImageMode != MapImageWrapMode.Scaled)
			{
				brush2 = brush;
				brush = this.GetTextureBrush(backImage, backImageTranspColor, backImageMode);
			}
			else
			{
				brush = ((backHatchStyle == MapHatchStyle.None) ? ((backGradientType == GradientType.None) ? ((!(backColor == Color.Empty) && !(backColor == Color.Transparent)) ? new SolidBrush(backColor) : null) : this.GetGradientBrush(rectangleF, backColor, backSecondaryColor, backGradientType)) : MapGraphics.GetHatchBrush(backHatchStyle, backColor, backSecondaryColor));
			}
			this.FillRectangleShadowAbs(rectangleF, shadowColor, (float)shadowOffset, backColor, circular, circularSectorsCount);
			if (!string.IsNullOrEmpty(backImage) && (backImageMode == MapImageWrapMode.Unscaled || backImageMode == MapImageWrapMode.Scaled))
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
				if (backImageMode == MapImageWrapMode.Unscaled)
				{
					rectangleF3.Width = Math.Min(rectangleF2.Width, (float)image.Width);
					rectangleF3.Height = Math.Min(rectangleF2.Height, (float)image.Height);
					if (rectangleF3.Width < rectangleF2.Width)
					{
						switch (backImageAlign)
						{
						case MapImageAlign.TopRight:
						case MapImageAlign.Right:
						case MapImageAlign.BottomRight:
							rectangleF3.X = rectangleF2.Right - rectangleF3.Width;
							break;
						case MapImageAlign.Top:
						case MapImageAlign.Bottom:
						case MapImageAlign.Center:
							rectangleF3.X = (float)(rectangleF2.X + (rectangleF2.Width - rectangleF3.Width) / 2.0);
							break;
						}
					}
					if (rectangleF3.Height < rectangleF2.Height)
					{
						switch (backImageAlign)
						{
						case MapImageAlign.BottomRight:
						case MapImageAlign.Bottom:
						case MapImageAlign.BottomLeft:
							rectangleF3.Y = rectangleF2.Bottom - rectangleF3.Height;
							break;
						case MapImageAlign.Right:
						case MapImageAlign.Left:
						case MapImageAlign.Center:
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
				base.DrawImage(image, new Rectangle((int)Math.Round((double)rectangleF3.X), (int)Math.Round((double)rectangleF3.Y), (int)Math.Round((double)rectangleF3.Width), (int)Math.Round((double)rectangleF3.Height)), 0f, 0f, (backImageMode == MapImageWrapMode.Unscaled) ? rectangleF3.Width : ((float)image.Width), (backImageMode == MapImageWrapMode.Unscaled) ? rectangleF3.Height : ((float)image.Height), GraphicsUnit.Pixel, imageAttributes);
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
				if (this.pen.DashStyle != MapGraphics.GetPenStyle(borderStyle))
				{
					this.pen.DashStyle = MapGraphics.GetPenStyle(borderStyle);
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

		internal void FillRectangleShadowAbs(RectangleF rect, Color shadowColor, float shadowOffset, Color backColor)
		{
			this.FillRectangleShadowAbs(rect, shadowColor, shadowOffset, backColor, false, 0);
		}

		internal void FillRectangleShadowAbs(RectangleF rect, Color shadowColor, float shadowOffset, Color backColor, bool circular, int circularSectorsCount)
		{
			if (rect.Height != 0.0 && rect.Width != 0.0 && shadowOffset != 0.0)
			{
				if (!this.softShadows || circularSectorsCount > 2)
				{
					RectangleF empty = RectangleF.Empty;
					if (shadowOffset != 0.0 && !(shadowColor == Color.Empty))
					{
						RectangleF rectangleF = MapGraphics.Round(rect);
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
				}
				else
				{
					RectangleF empty2 = RectangleF.Empty;
					if (shadowOffset != 0.0 && !(shadowColor == Color.Empty))
					{
						RectangleF rectangleF2 = MapGraphics.Round(rect);
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
						graphicsPath.AddLine(empty2.X + val, empty2.Y, empty2.Right - val, empty2.Y);
						graphicsPath.AddArc((float)(empty2.Right - 2.0 * val), empty2.Y, (float)(2.0 * val), (float)(2.0 * val), 270f, 90f);
						graphicsPath.AddLine(empty2.Right, empty2.Y + val, empty2.Right, empty2.Bottom - val);
						graphicsPath.AddArc((float)(empty2.Right - 2.0 * val), (float)(empty2.Bottom - 2.0 * val), (float)(2.0 * val), (float)(2.0 * val), 0f, 90f);
						graphicsPath.AddLine(empty2.Right - val, empty2.Bottom, empty2.X + val, empty2.Bottom);
						graphicsPath.AddArc(empty2.X, (float)(empty2.Bottom - 2.0 * val), (float)(2.0 * val), (float)(2.0 * val), 90f, 90f);
						graphicsPath.AddLine(empty2.X, empty2.Bottom - val, empty2.X, empty2.Y + val);
						graphicsPath.AddArc(empty2.X, empty2.Y, (float)(2.0 * val), (float)(2.0 * val), 180f, 90f);
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
				}
			}
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

		internal void DrawRectangleRel(Pen pen, RectangleF rect)
		{
			RectangleF absoluteRectangle = this.GetAbsoluteRectangle(rect);
			base.DrawRectangle(pen, absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height);
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
				return MapGraphics.GetGradientColor(Color.FromArgb(beginColor.A, 255, 255, 255), beginColor, 1.0 - num + position);
			}
			if (0.0 - num + position < 1.0)
			{
				return MapGraphics.GetGradientColor(beginColor, Color.Black, 0.0 - num + position);
			}
			return Color.FromArgb(beginColor.A, 0, 0, 0);
		}

		internal void FillRectangleAbs(RectangleF rect, Color backColor, MapHatchStyle backHatchStyle, string backImage, MapImageWrapMode backImageMode, Color backImageTranspColor, MapImageAlign backImageAlign, GradientType backGradientType, Color backSecondaryColor, Color borderColor, int borderWidth, MapDashStyle borderStyle, PenAlignment penAlignment)
		{
			Brush brush = null;
			Brush brush2 = null;
			SmoothingMode smoothingMode = base.SmoothingMode;
			base.SmoothingMode = SmoothingMode.None;
			if (backColor.IsEmpty)
			{
				backColor = Color.White;
			}
			if (backSecondaryColor.IsEmpty)
			{
				backSecondaryColor = Color.White;
			}
			if (borderColor.IsEmpty)
			{
				borderColor = Color.White;
				borderWidth = 0;
			}
			this.pen.Color = borderColor;
			this.pen.Width = (float)borderWidth;
			this.pen.Alignment = penAlignment;
			this.pen.DashStyle = MapGraphics.GetPenStyle(borderStyle);
			if (backGradientType == GradientType.None)
			{
				this.solidBrush.Color = backColor;
				brush = this.solidBrush;
			}
			else
			{
				brush = this.GetGradientBrush(rect, backColor, backSecondaryColor, backGradientType);
			}
			if (backHatchStyle != 0)
			{
				brush = MapGraphics.GetHatchBrush(backHatchStyle, backColor, backSecondaryColor);
			}
			if (!string.IsNullOrEmpty(backImage) && backImageMode != MapImageWrapMode.Unscaled && backImageMode != MapImageWrapMode.Scaled)
			{
				brush2 = brush;
				brush = this.GetTextureBrush(backImage, backImageTranspColor, backImageMode);
			}
			RectangleF rectangleF = new RectangleF(rect.X + (float)borderWidth, rect.Y + (float)borderWidth, rect.Width - (float)(borderWidth * 2), rect.Height - (float)(borderWidth * 2));
			rectangleF.Width += 1f;
			rectangleF.Height += 1f;
			if (!string.IsNullOrEmpty(backImage) && (backImageMode == MapImageWrapMode.Unscaled || backImageMode == MapImageWrapMode.Scaled))
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
				if (backImageMode == MapImageWrapMode.Unscaled)
				{
					rectangleF2.Width = (float)image.Width;
					rectangleF2.Height = (float)image.Height;
					if (rectangleF2.Width < rectangleF.Width)
					{
						switch (backImageAlign)
						{
						case MapImageAlign.TopRight:
						case MapImageAlign.Right:
						case MapImageAlign.BottomRight:
							rectangleF2.X = rectangleF.Right - rectangleF2.Width;
							break;
						case MapImageAlign.Top:
						case MapImageAlign.Bottom:
						case MapImageAlign.Center:
							rectangleF2.X = (float)(rectangleF.X + (rectangleF.Width - rectangleF2.Width) / 2.0);
							break;
						}
					}
					if (rectangleF2.Height < rectangleF.Height)
					{
						switch (backImageAlign)
						{
						case MapImageAlign.BottomRight:
						case MapImageAlign.Bottom:
						case MapImageAlign.BottomLeft:
							rectangleF2.Y = rectangleF.Bottom - rectangleF2.Height;
							break;
						case MapImageAlign.Right:
						case MapImageAlign.Left:
						case MapImageAlign.Center:
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
			if (!string.IsNullOrEmpty(backImage) && backImageMode != MapImageWrapMode.Unscaled && backImageMode != MapImageWrapMode.Scaled)
			{
				brush.Dispose();
			}
			if (backHatchStyle != 0)
			{
				brush.Dispose();
			}
			base.SmoothingMode = smoothingMode;
		}

		internal void DrawPathShadowAbs(GraphicsPath path, Color shadowColor, float shadowWidth)
		{
			if (shadowWidth != 0.0)
			{
				if (shadowColor == Color.Empty)
				{
					shadowColor = Color.FromArgb(128, 128, 128, 128);
				}
				if (shadowColor.A == 255)
				{
					shadowColor = Color.FromArgb((int)shadowColor.A / 2, shadowColor);
				}
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

		internal void DrawPathAbs(GraphicsPath path, Color backColor, MapHatchStyle backHatchStyle, string backImage, MapImageWrapMode backImageMode, Color backImageTranspColor, MapImageAlign backImageAlign, GradientType backGradientType, Color backSecondaryColor, Color borderColor, int borderWidth, MapDashStyle borderStyle, PenAlignment penAlignment)
		{
			Brush brush = null;
			Brush brush2 = null;
			if (backColor.IsEmpty)
			{
				backColor = Color.White;
			}
			if (backSecondaryColor.IsEmpty)
			{
				backSecondaryColor = Color.White;
			}
			if (borderColor.IsEmpty)
			{
				borderColor = Color.White;
				borderWidth = 0;
			}
			this.pen.Color = borderColor;
			this.pen.Width = (float)borderWidth;
			this.pen.Alignment = penAlignment;
			this.pen.DashStyle = MapGraphics.GetPenStyle(borderStyle);
			if (backGradientType == GradientType.None)
			{
				this.solidBrush.Color = backColor;
				brush = this.solidBrush;
			}
			else
			{
				RectangleF bounds = path.GetBounds();
				bounds.Inflate(new SizeF(2f, 2f));
				brush = this.GetGradientBrush(bounds, backColor, backSecondaryColor, backGradientType);
			}
			if (backHatchStyle != 0)
			{
				brush = MapGraphics.GetHatchBrush(backHatchStyle, backColor, backSecondaryColor);
			}
			if (!string.IsNullOrEmpty(backImage) && backImageMode != MapImageWrapMode.Unscaled && backImageMode != MapImageWrapMode.Scaled)
			{
				brush2 = brush;
				brush = this.GetTextureBrush(backImage, backImageTranspColor, backImageMode);
			}
			RectangleF bounds2 = path.GetBounds();
			if (!string.IsNullOrEmpty(backImage) && (backImageMode == MapImageWrapMode.Unscaled || backImageMode == MapImageWrapMode.Scaled))
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
				if (backImageMode == MapImageWrapMode.Unscaled)
				{
					rectangleF.Width = (float)image.Width;
					rectangleF.Height = (float)image.Height;
					if (rectangleF.Width < bounds2.Width)
					{
						switch (backImageAlign)
						{
						case MapImageAlign.TopRight:
						case MapImageAlign.Right:
						case MapImageAlign.BottomRight:
							rectangleF.X = bounds2.Right - rectangleF.Width;
							break;
						case MapImageAlign.Top:
						case MapImageAlign.Bottom:
						case MapImageAlign.Center:
							rectangleF.X = (float)(bounds2.X + (bounds2.Width - rectangleF.Width) / 2.0);
							break;
						}
					}
					if (rectangleF.Height < bounds2.Height)
					{
						switch (backImageAlign)
						{
						case MapImageAlign.BottomRight:
						case MapImageAlign.Bottom:
						case MapImageAlign.BottomLeft:
							rectangleF.Y = bounds2.Bottom - rectangleF.Height;
							break;
						case MapImageAlign.Right:
						case MapImageAlign.Left:
						case MapImageAlign.Center:
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

		public MapPoint PixelsToGeographic(PointF pointInPixels)
		{
			PointF relativePoint = this.GetRelativePoint(pointInPixels);
			return this.common.MapCore.PercentsToGeographic((double)relativePoint.X, (double)relativePoint.Y);
		}

		public PointF GeographicToPixels(MapPoint pointOnMap)
		{
			PointF relative = this.common.MapCore.GeographicToPercents(pointOnMap).ToPointF();
			return this.GetAbsolutePoint(relative);
		}

		internal float GetRelativeX(float absoluteX)
		{
			return (float)(absoluteX * 100.0 / (float)(this.width - 1));
		}

		internal float GetRelativeY(float absoluteY)
		{
			return (float)(absoluteY * 100.0 / (float)(this.height - 1));
		}

		internal float GetRelativeWidth(float absoluteWidth)
		{
			return (float)(absoluteWidth * 100.0 / (float)this.width);
		}

		internal float GetRelativeHeight(float absoluteHeight)
		{
			return (float)(absoluteHeight * 100.0 / (float)this.height);
		}

		internal float GetAbsoluteX(float relativeX)
		{
			return (float)(relativeX * (float)(this.width - 1) / 100.0);
		}

		internal float GetAbsoluteY(float relativeY)
		{
			return (float)(relativeY * (float)(this.height - 1) / 100.0);
		}

		internal float GetAbsoluteWidth(float relativeWidth)
		{
			return (float)(relativeWidth * (float)this.width / 100.0);
		}

		internal float GetAbsoluteHeight(float relativeHeight)
		{
			return (float)(relativeHeight * (float)this.height / 100.0);
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

		internal RectangleF GetBorder3DAdjustedRect(Frame frameAttr)
		{
			RectangleF relative = new RectangleF(0f, 0f, 100f, 100f);
			if (frameAttr.FrameStyle != 0)
			{
				IBorderType borderType = this.common.BorderTypeRegistry.GetBorderType(((Enum)(object)frameAttr.FrameStyle).ToString((IFormatProvider)CultureInfo.InvariantCulture));
				if (borderType != null)
				{
					borderType.AdjustAreasPosition(this, ref relative);
				}
			}
			RectangleF absoluteRectangle = this.GetAbsoluteRectangle(relative);
			absoluteRectangle.Inflate(-5f, -5f);
			absoluteRectangle.Height -= 3f;
			absoluteRectangle.Width -= 2f;
			return absoluteRectangle;
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

		internal void Draw3DBorderRel(Frame borderSkin, RectangleF rect, Color borderColor, Color backColor)
		{
			this.Draw3DBorderAbs(borderSkin, this.GetAbsoluteRectangle(rect), borderColor, backColor);
		}

		internal void Draw3DBorderAbs(Frame borderSkin, RectangleF absRect, Color borderColor, Color backColor)
		{
			this.Draw3DBorderAbs(borderSkin, absRect, backColor, MapHatchStyle.None, "", MapImageWrapMode.Tile, Color.Empty, MapImageAlign.Center, GradientType.None, Color.Empty, borderColor, 1, MapDashStyle.Dot);
		}

		internal void Draw3DBorderRel(Frame borderSkin, RectangleF rect, Color backColor, MapHatchStyle backHatchStyle, string backImage, MapImageWrapMode backImageMode, Color backImageTranspColor, MapImageAlign backImageAlign, GradientType backGradientType, Color backSecondaryColor, Color borderColor, int borderWidth, MapDashStyle borderStyle)
		{
			this.Draw3DBorderAbs(borderSkin, this.GetAbsoluteRectangle(rect), backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backSecondaryColor, borderColor, borderWidth, borderStyle);
		}

		internal void Draw3DBorderAbs(Frame borderSkin, RectangleF absRect, Color backColor, MapHatchStyle backHatchStyle, string backImage, MapImageWrapMode backImageMode, Color backImageTranspColor, MapImageAlign backImageAlign, GradientType backGradientType, Color backSecondaryColor, Color borderColor, int borderWidth, MapDashStyle borderStyle)
		{
			if (this.common != null && borderSkin.FrameStyle != 0 && absRect.Width != 0.0 && absRect.Height != 0.0)
			{
				IBorderType borderType = this.common.BorderTypeRegistry.GetBorderType(((Enum)(object)borderSkin.FrameStyle).ToString((IFormatProvider)CultureInfo.InvariantCulture));
				if (borderType != null && borderType.IsVisible(this))
				{
					borderType.DrawBorder(this, borderSkin, absRect, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backSecondaryColor, borderColor, borderWidth, borderStyle);
				}
				else
				{
					this.FillRectangleAbs(absRect, backColor, backHatchStyle, "", MapImageWrapMode.Unscaled, Color.Empty, MapImageAlign.Center, backGradientType, backSecondaryColor, Color.Empty, 0, MapDashStyle.None, PenAlignment.Inset);
				}
			}
		}

		internal void DrawPieRel(RectangleF rect, float startAngle, float sweepAngle, Color backColor, MapHatchStyle backHatchStyle, string backImage, MapImageWrapMode backImageMode, Color backImageTranspColor, MapImageAlign backImageAlign, GradientType backGradientType, Color backSecondaryColor, Color borderColor, int borderWidth, MapDashStyle borderStyle, PenAlignment penAlignment, bool shadow, double shadowOffset, bool doughnut, float doughnutRadius, bool explodedShadow)
		{
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
					brush = MapGraphics.GetHatchBrush(backHatchStyle, backColor, backSecondaryColor);
					goto IL_00d2;
				}
				if (!backSecondaryColor.IsEmpty)
				{
					switch (backGradientType)
					{
					case GradientType.Center:
						break;
					default:
						goto IL_005f;
					case GradientType.None:
						goto IL_00a7;
					}
					brush = this.GetPieGradientBrush(absoluteRectangle, backColor, backSecondaryColor);
					goto IL_00d2;
				}
				goto IL_00a7;
			}
			return;
			IL_00a7:
			brush = ((string.IsNullOrEmpty(backImage) || backImageMode == MapImageWrapMode.Unscaled || backImageMode == MapImageWrapMode.Scaled) ? new SolidBrush(backColor) : this.GetTextureBrush(backImage, backImageTranspColor, backImageMode));
			goto IL_00d2;
			IL_005f:
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddPie(absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height, startAngle, sweepAngle);
			brush = this.GetGradientBrush(graphicsPath.GetBounds(), backColor, backSecondaryColor, backGradientType);
			if (graphicsPath != null)
			{
				graphicsPath.Dispose();
			}
			goto IL_00d2;
			IL_00d2:
			pen = new Pen(borderColor, (float)borderWidth);
			pen.DashStyle = MapGraphics.GetPenStyle(borderStyle);
			if (doughnut)
			{
				GraphicsPath graphicsPath2 = new GraphicsPath();
				graphicsPath2.AddArc((float)(absoluteRectangle.X + absoluteRectangle.Width * doughnutRadius / 200.0 - 1.0), (float)(absoluteRectangle.Y + absoluteRectangle.Height * doughnutRadius / 200.0 - 1.0), (float)(absoluteRectangle.Width - absoluteRectangle.Width * doughnutRadius / 100.0 + 2.0), (float)(absoluteRectangle.Height - absoluteRectangle.Height * doughnutRadius / 100.0 + 2.0), startAngle, sweepAngle);
				graphicsPath2.AddArc(absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height, startAngle + sweepAngle, (float)(0.0 - sweepAngle));
				graphicsPath2.CloseFigure();
				base.FillPath(brush, graphicsPath2);
				if (!shadow)
				{
					base.DrawPath(pen, graphicsPath2);
				}
				if (graphicsPath2 != null)
				{
					graphicsPath2.Dispose();
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
				}
				if (!shadow)
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

		internal void DrawImageRel(string name, RectangleF position)
		{
			RectangleF absoluteRectangle = this.GetAbsoluteRectangle(position);
			ImageLoader imageLoader = this.common.ImageLoader;
			Image image = imageLoader.LoadImage(name);
			base.DrawImage(image, absoluteRectangle);
		}

		internal static RectangleF Round(RectangleF rect)
		{
			return new RectangleF((float)Math.Round((double)rect.X), (float)Math.Round((double)rect.Y), (float)Math.Round((double)rect.Width), (float)Math.Round((double)rect.Height));
		}

		internal void SetPictureSize(int width, int height)
		{
			this.width = width;
			this.height = height;
		}

		internal void CreateDrawRegion(RectangleF rect)
		{
			this.graphicStates.Push(new MapGraphState(base.Save(), this.width, this.height));
			RectangleF absoluteRectangle = this.GetAbsoluteRectangle(rect);
			if (base.Transform == null)
			{
				base.Transform = new Matrix();
			}
			base.TranslateTransform((float)Math.Round((double)absoluteRectangle.Location.X), (float)Math.Round((double)absoluteRectangle.Location.Y));
			this.SetPictureSize((int)Math.Round((double)absoluteRectangle.Size.Width), (int)Math.Round((double)absoluteRectangle.Size.Height));
		}

		internal void CreateContentDrawRegion(Viewport viewport, PointF gridSectionOffset)
		{
			this.graphicStates.Push(new MapGraphState(base.Save(), this.width, this.height));
			if (base.Transform == null)
			{
				base.Transform = new Matrix();
			}
			PointF contentOffsetInPixels = viewport.GetContentOffsetInPixels();
			contentOffsetInPixels.X += (float)viewport.Margins.Left;
			contentOffsetInPixels.Y += (float)viewport.Margins.Top;
			SizeF contentSizeInPixels = viewport.GetContentSizeInPixels();
			contentOffsetInPixels.X -= gridSectionOffset.X;
			contentOffsetInPixels.Y -= gridSectionOffset.Y;
			base.TranslateTransform(contentOffsetInPixels.X, contentOffsetInPixels.Y);
			this.SetPictureSize((int)(contentSizeInPixels.Width * viewport.Zoom / 100.0), (int)(contentSizeInPixels.Height * viewport.Zoom / 100.0));
		}

		internal void RestoreDrawRegion()
		{
			MapGraphState mapGraphState = (MapGraphState)this.graphicStates.Pop();
			base.Restore(mapGraphState.state);
			this.SetPictureSize(mapGraphState.width, mapGraphState.height);
		}

		public override void Close()
		{
			this.common.Graph = null;
			base.Close();
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

		internal static Color GetGradientColor(Color beginColor, Color endColor, double dPosition)
		{
			if (!(dPosition < 0.0) && !(dPosition > 1.0) && !double.IsNaN(dPosition))
			{
				int r = beginColor.R;
				int g = beginColor.G;
				int b = beginColor.B;
				int r2 = endColor.R;
				int g2 = endColor.G;
				int b2 = endColor.B;
				double num = (double)r + (double)(r2 - r) * dPosition;
				double num2 = (double)g + (double)(g2 - g) * dPosition;
				double num3 = (double)b + (double)(b2 - b) * dPosition;
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
				pen.Width = (float)(1.0 / this.ScaleFactorX);
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
			pen.Width = (float)(1.0 / this.ScaleFactorX);
			return pen;
		}

		internal void DrawSelection(RectangleF rect, bool designTimeSelection, Color borderColor, Color markerColor)
		{
			this.DrawSelection(rect, (float)(3.0 / this.ScaleFactorX), designTimeSelection, borderColor, markerColor);
		}

		internal void DrawSelection(RectangleF rect, float inflateBy, bool designTimeSelection, Color borderColor, Color markerColor)
		{
			rect.Inflate(inflateBy, inflateBy);
			RectangleF visibleClipBounds = this.Graphics.VisibleClipBounds;
			visibleClipBounds.Inflate((float)(-3.0 / this.ScaleFactorX), (float)(-3.0 / this.ScaleFactorY));
			visibleClipBounds.Width -= 1f;
			visibleClipBounds.Height -= 1f;
			rect = RectangleF.Intersect(rect, visibleClipBounds);
			PointF pointF = new PointF((float)(rect.X + rect.Width / 2.0), (float)(rect.Y + rect.Height / 2.0));
			using (Pen pen = this.GetSelectionPen(designTimeSelection, borderColor))
			{
				base.DrawLine(pen, new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Top));
				base.DrawLine(pen, new PointF(rect.Left, rect.Bottom), new PointF(rect.Right, rect.Bottom));
				base.DrawLine(pen, new PointF(rect.Left, rect.Top), new PointF(rect.Left, rect.Bottom));
				base.DrawLine(pen, new PointF(rect.Right, rect.Top), new PointF(rect.Right, rect.Bottom));
			}
			ArrayList arrayList = new ArrayList();
			arrayList.Add(new PointF(rect.X, rect.Y));
			if (rect.Width >= 20.0)
			{
				arrayList.Add(new PointF(pointF.X, rect.Y));
			}
			arrayList.Add(new PointF(rect.X + rect.Width, rect.Y));
			if (rect.Height >= 20.0)
			{
				arrayList.Add(new PointF(rect.X, pointF.Y));
			}
			if (rect.Height >= 20.0)
			{
				arrayList.Add(new PointF(rect.X + rect.Width, pointF.Y));
			}
			arrayList.Add(new PointF(rect.X, rect.Y + rect.Height));
			if (rect.Width >= 20.0)
			{
				arrayList.Add(new PointF(pointF.X, rect.Y + rect.Height));
			}
			arrayList.Add(new PointF(rect.X + rect.Width, rect.Y + rect.Height));
			this.DrawSelectionMarkers((PointF[])arrayList.ToArray(typeof(PointF)), designTimeSelection, borderColor, markerColor);
		}

		internal void DrawSelectionMarkers(PointF[] markerPositions, bool designTimeSelection, Color borderColor, Color markerColor)
		{
			float num = (float)(6.0 / this.ScaleFactorX);
			float num2 = (float)(6.0 / this.ScaleFactorY);
			Brush brush;
			Pen pen;
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
				base.FillEllipse(brush, new RectangleF((float)(markerPositions[i].X - num / 2.0), (float)(markerPositions[i].Y - num2 / 2.0), num, num2));
				base.DrawEllipse(pen, new RectangleF((float)(markerPositions[i].X - num / 2.0), (float)(markerPositions[i].Y - num2 / 2.0), num, num2));
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
