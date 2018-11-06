using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class ChartGraphics3D : ChartRenderingEngine
	{
		private int oppLeftBottomPoint = -1;

		private int oppRigthTopPoint = -1;

		internal PointF frontLinePoint1 = PointF.Empty;

		internal PointF frontLinePoint2 = PointF.Empty;

		internal Pen frontLinePen;

		internal void Draw3DGridLine(ChartArea area, Color color, int width, ChartDashStyle style, PointF point1, PointF point2, bool horizontal, CommonElements common, object obj)
		{
			this.Draw3DGridLine(area, color, width, style, point1, point2, horizontal, common, obj, 0, 0);
		}

		internal void Draw3DGridLine(ChartArea area, Color color, int width, ChartDashStyle style, PointF point1, PointF point2, bool horizontal, CommonElements common, object obj, int numberOfElements, int elementIndex)
		{
			float z = (float)(area.IsMainSceneWallOnFront() ? area.areaSceneDepth : 0.0);
			PointF pointF = new PointF(point2.X, point2.Y);
			if (horizontal)
			{
				pointF.X = point1.X;
				pointF.Y = point1.Y;
			}
			this.InitGridAnimation3D(common, new Point3D(pointF.X, pointF.Y, z), (ChartGraphics)this, numberOfElements, elementIndex, area, obj);
			((ChartGraphics)this).StartAnimation();
			((ChartGraphics)this).Draw3DLine(area.matrix3D, color, width, style, new Point3D(point1.X, point1.Y, z), new Point3D(point2.X, point2.Y, z), common, obj, (ChartElementType)((obj is StripLine) ? 7 : 6));
			((ChartGraphics)this).StopAnimation();
			if (horizontal)
			{
				if (area.IsSideSceneWallOnLeft())
				{
					point1.X = Math.Min(point1.X, point2.X);
				}
				else
				{
					point1.X = Math.Max(point1.X, point2.X);
				}
				this.InitGridAnimation3D(common, new Point3D(point1.X, point1.Y, z), (ChartGraphics)this, numberOfElements, elementIndex, area, obj);
				((ChartGraphics)this).StartAnimation();
				((ChartGraphics)this).Draw3DLine(area.matrix3D, color, width, style, new Point3D(point1.X, point1.Y, 0f), new Point3D(point1.X, point1.Y, area.areaSceneDepth), common, obj, (ChartElementType)((obj is StripLine) ? 7 : 6));
				((ChartGraphics)this).StopAnimation();
			}
			else if (area.IsBottomSceneWallVisible())
			{
				point1.Y = Math.Max(point1.Y, point2.Y);
				this.InitGridAnimation3D(common, new Point3D(point1.X, point1.Y, z), (ChartGraphics)this, numberOfElements, elementIndex, area, obj);
				((ChartGraphics)this).StartAnimation();
				((ChartGraphics)this).Draw3DLine(area.matrix3D, color, width, style, new Point3D(point1.X, point1.Y, 0f), new Point3D(point1.X, point1.Y, area.areaSceneDepth), common, obj, (ChartElementType)((obj is StripLine) ? 7 : 6));
				((ChartGraphics)this).StopAnimation();
			}
		}

		private void InitGridAnimation3D(CommonElements common, Point3D point, ChartGraphics graph, int numberOfElements, int index, ChartArea area, object obj)
		{
		}

		internal void Draw3DLine(Matrix3D matrix, Color color, int width, ChartDashStyle style, Point3D firstPoint, Point3D secondPoint, CommonElements common, object obj, ChartElementType type)
		{
			Point3D[] array = new Point3D[2]
			{
				firstPoint,
				secondPoint
			};
			matrix.TransformPoints(array);
			if (common.ProcessModeRegions)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				if (Math.Abs(array[0].X - array[1].X) > Math.Abs(array[0].Y - array[1].Y))
				{
					graphicsPath.AddLine(array[0].X, (float)(array[0].Y - 1.0), array[1].X, (float)(array[1].Y - 1.0));
					graphicsPath.AddLine(array[1].X, (float)(array[1].Y + 1.0), array[0].X, (float)(array[0].Y + 1.0));
					graphicsPath.CloseAllFigures();
				}
				else
				{
					graphicsPath.AddLine((float)(array[0].X - 1.0), array[0].Y, (float)(array[1].X - 1.0), array[1].Y);
					graphicsPath.AddLine((float)(array[1].X + 1.0), array[1].Y, (float)(array[0].X + 1.0), array[0].Y);
					graphicsPath.CloseAllFigures();
				}
				common.HotRegionsList.AddHotRegion(graphicsPath, true, (ChartGraphics)this, type, obj);
			}
			if (common.ProcessModePaint)
			{
				((ChartGraphics)this).DrawLineRel(color, width, style, array[0].PointF, array[1].PointF);
			}
		}

		internal void FillPieSides(ChartArea area, float xAngle, float startAngle, float sweepAngle, PointF[] points, SolidBrush brush, Pen pen, bool doughnut)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			PointF pointF = points[8];
			PointF pointF2 = points[9];
			PointF pointF3 = points[4];
			PointF pointF4 = points[6];
			PointF pointF5 = points[5];
			PointF pointF6 = points[7];
			PointF pointF7 = PointF.Empty;
			PointF pointF8 = PointF.Empty;
			PointF pointF9 = PointF.Empty;
			PointF pointF10 = PointF.Empty;
			if (doughnut)
			{
				pointF7 = points[21];
				pointF8 = points[23];
				pointF9 = points[22];
				pointF10 = points[24];
			}
			bool flag = false;
			bool flag2 = false;
			float num = startAngle + sweepAngle;
			if (xAngle > 0.0)
			{
				if (startAngle > -90.0 && startAngle < 90.0)
				{
					goto IL_00f2;
				}
				if (startAngle > 270.0 && startAngle < 450.0)
				{
					goto IL_00f2;
				}
				goto IL_00f5;
			}
			if (startAngle >= -180.0 && startAngle < -90.0)
			{
				goto IL_0160;
			}
			if (startAngle > 90.0 && startAngle < 270.0)
			{
				goto IL_0160;
			}
			if (startAngle > 450.0 && startAngle <= 540.0)
			{
				goto IL_0160;
			}
			goto IL_0163;
			IL_0160:
			flag = true;
			goto IL_0163;
			IL_00f5:
			if (num >= -180.0 && num < -90.0)
			{
				goto IL_012b;
			}
			if (num > 90.0 && num < 270.0)
			{
				goto IL_012b;
			}
			if (num > 450.0 && num <= 540.0)
			{
				goto IL_012b;
			}
			goto IL_018a;
			IL_0187:
			flag2 = true;
			goto IL_018a;
			IL_0163:
			if (num > -90.0 && num < 90.0)
			{
				goto IL_0187;
			}
			if (num > 270.0 && num < 450.0)
			{
				goto IL_0187;
			}
			goto IL_018a;
			IL_018a:
			if (flag)
			{
				graphicsPath = new GraphicsPath();
				if (doughnut)
				{
					graphicsPath.AddLine(pointF7, pointF3);
					graphicsPath.AddLine(pointF3, pointF4);
					graphicsPath.AddLine(pointF4, pointF8);
					graphicsPath.AddLine(pointF8, pointF7);
				}
				else
				{
					graphicsPath.AddLine(pointF, pointF3);
					graphicsPath.AddLine(pointF3, pointF4);
					graphicsPath.AddLine(pointF4, pointF2);
					graphicsPath.AddLine(pointF2, pointF);
				}
				Color color = default(Color);
				Color color2 = default(Color);
				Color color3 = default(Color);
				Color color4 = default(Color);
				Color color5 = default(Color);
				Color color6 = default(Color);
				area.matrix3D.GetLight(brush.Color, out color, out color2, out color3, out color4, out color5, out color6);
				Color color7 = (area.Area3DStyle.XAngle >= 0) ? color5 : color6;
				((ChartGraphics)this).FillPath(new SolidBrush(color7), graphicsPath);
				this.DrawGraphicsPath(pen, graphicsPath);
			}
			if (flag2)
			{
				graphicsPath = new GraphicsPath();
				if (doughnut)
				{
					graphicsPath.AddLine(pointF9, pointF5);
					graphicsPath.AddLine(pointF5, pointF6);
					graphicsPath.AddLine(pointF6, pointF10);
					graphicsPath.AddLine(pointF10, pointF9);
				}
				else
				{
					graphicsPath.AddLine(pointF, pointF5);
					graphicsPath.AddLine(pointF5, pointF6);
					graphicsPath.AddLine(pointF6, pointF2);
					graphicsPath.AddLine(pointF2, pointF);
				}
				Color color8 = default(Color);
				Color color9 = default(Color);
				Color color10 = default(Color);
				Color color11 = default(Color);
				Color color12 = default(Color);
				Color color13 = default(Color);
				area.matrix3D.GetLight(brush.Color, out color8, out color9, out color10, out color11, out color12, out color13);
				Color color14 = (area.Area3DStyle.XAngle >= 0) ? color12 : color13;
				((ChartGraphics)this).FillPath(new SolidBrush(color14), graphicsPath);
				this.DrawGraphicsPath(pen, graphicsPath);
			}
			return;
			IL_012b:
			flag2 = true;
			goto IL_018a;
			IL_00f2:
			flag = true;
			goto IL_00f5;
		}

		internal void FillPieCurve(ChartArea area, DataPoint point, Brush brush, Pen pen, PointF topFirstRectPoint, PointF topSecondRectPoint, PointF bottomFirstRectPoint, PointF bottomSecondRectPoint, PointF topFirstPoint, PointF topSecondPoint, PointF bottomFirstPoint, PointF bottomSecondPoint, float startAngle, float sweepAngle, int pointIndex)
		{
			CommonElements common = area.Common;
			GraphicsPath graphicsPath = new GraphicsPath();
			RectangleF rectangleF = default(RectangleF);
			rectangleF.X = topFirstRectPoint.X;
			rectangleF.Y = topFirstRectPoint.Y;
			rectangleF.Height = topSecondRectPoint.Y - topFirstRectPoint.Y;
			rectangleF.Width = topSecondRectPoint.X - topFirstRectPoint.X;
			RectangleF rectangleF2 = default(RectangleF);
			rectangleF2.X = bottomFirstRectPoint.X;
			rectangleF2.Y = bottomFirstRectPoint.Y;
			rectangleF2.Height = bottomSecondRectPoint.Y - bottomFirstRectPoint.Y;
			rectangleF2.Width = bottomSecondRectPoint.X - bottomFirstRectPoint.X;
			double correction = (double)(rectangleF.Height / rectangleF.Width);
			float num = this.AngleCorrection(startAngle + sweepAngle, correction, (float)area.Area3DStyle.XAngle);
			startAngle = this.AngleCorrection(startAngle, correction, (float)area.Area3DStyle.XAngle);
			sweepAngle = num - startAngle;
			graphicsPath.AddLine(topFirstPoint, bottomFirstPoint);
			if (rectangleF2.Height <= 0.0)
			{
				graphicsPath.AddLine(bottomFirstPoint.X, bottomFirstPoint.Y, bottomSecondPoint.X, bottomSecondPoint.Y);
			}
			else
			{
				graphicsPath.AddArc(rectangleF2.X, rectangleF2.Y, rectangleF2.Width, rectangleF2.Height, startAngle, sweepAngle);
			}
			graphicsPath.AddLine(bottomSecondPoint, topSecondPoint);
			if (rectangleF.Height <= 0.0)
			{
				graphicsPath.AddLine(topFirstPoint.X, topFirstPoint.Y, topSecondPoint.X, topSecondPoint.Y);
			}
			else
			{
				graphicsPath.AddArc(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height, startAngle + sweepAngle, (float)(0.0 - sweepAngle));
			}
			if (common.ProcessModePaint)
			{
				((ChartGraphics)this).FillPath(brush, graphicsPath);
				if (point.BorderColor != Color.Empty && point.BorderWidth > 0 && point.BorderStyle != 0)
				{
					this.DrawGraphicsPath(pen, graphicsPath);
				}
			}
			if (common.ProcessModeRegions)
			{
				if (point.IsAttributeSet("_COLLECTED_DATA_POINT"))
				{
					common.HotRegionsList.AddHotRegion((ChartGraphics)this, graphicsPath, false, point.ReplaceKeywords(point.ToolTip), point.ReplaceKeywords(point.Href), point.ReplaceKeywords(point.MapAreaAttributes), point, ChartElementType.DataPoint);
				}
				else
				{
					common.HotRegionsList.AddHotRegion(graphicsPath, false, (ChartGraphics)this, point, point.series.Name, pointIndex);
				}
			}
		}

		internal void FillPieSlice(ChartArea area, DataPoint point, SolidBrush brush, Pen pen, PointF firstRectPoint, PointF firstPoint, PointF secondRectPoint, PointF secondPoint, PointF center, float startAngle, float sweepAngle, bool fill, int pointIndex)
		{
			CommonElements common = area.Common;
			GraphicsPath graphicsPath = new GraphicsPath();
			RectangleF rectangleF = default(RectangleF);
			rectangleF.X = firstRectPoint.X;
			rectangleF.Y = firstRectPoint.Y;
			rectangleF.Height = secondRectPoint.Y - firstRectPoint.Y;
			rectangleF.Width = secondRectPoint.X - firstRectPoint.X;
			double correction = (double)(rectangleF.Height / rectangleF.Width);
			float num = this.AngleCorrection(startAngle + sweepAngle, correction, (float)area.Area3DStyle.XAngle);
			startAngle = this.AngleCorrection(startAngle, correction, (float)area.Area3DStyle.XAngle);
			sweepAngle = num - startAngle;
			graphicsPath.AddLine(center, firstPoint);
			if (rectangleF.Height > 0.0)
			{
				graphicsPath.AddArc(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height, startAngle, sweepAngle);
			}
			graphicsPath.AddLine(secondPoint, center);
			if (common.ProcessModePaint)
			{
				Color color = default(Color);
				Color color2 = default(Color);
				Color color3 = default(Color);
				Color color4 = default(Color);
				Color color5 = default(Color);
				Color color6 = default(Color);
				area.matrix3D.GetLight(brush.Color, out color, out color2, out color3, out color4, out color5, out color6);
				Pen pen2 = (Pen)pen.Clone();
				if (area.Area3DStyle.Light == LightStyle.Realistic && point.BorderColor == Color.Empty)
				{
					pen2.Color = color;
				}
				if (fill)
				{
					((ChartGraphics)this).FillPath(new SolidBrush(color), graphicsPath);
				}
				if (point.BorderColor != Color.Empty && point.BorderWidth > 0 && point.BorderStyle != 0)
				{
					this.DrawGraphicsPath(pen2, graphicsPath);
				}
			}
			if (common.ProcessModeRegions && fill)
			{
				if (point.IsAttributeSet("_COLLECTED_DATA_POINT"))
				{
					common.HotRegionsList.AddHotRegion((ChartGraphics)this, graphicsPath, false, point.ReplaceKeywords(point.ToolTip), point.ReplaceKeywords(point.Href), point.ReplaceKeywords(point.MapAreaAttributes), point, ChartElementType.DataPoint);
				}
				else
				{
					common.HotRegionsList.AddHotRegion(graphicsPath, false, (ChartGraphics)this, point, point.series.Name, pointIndex);
				}
			}
		}

		internal void FillDoughnutSlice(ChartArea area, DataPoint point, SolidBrush brush, Pen pen, PointF firstRectPoint, PointF firstPoint, PointF secondRectPoint, PointF secondPoint, PointF threePoint, PointF fourPoint, PointF center, float startAngle, float sweepAngle, bool fill, float doughnutRadius, int pointIndex)
		{
			CommonElements common = area.Common;
			doughnutRadius = (float)(1.0 - doughnutRadius / 100.0);
			GraphicsPath graphicsPath = new GraphicsPath();
			RectangleF rectangleF = default(RectangleF);
			rectangleF.X = firstRectPoint.X;
			rectangleF.Y = firstRectPoint.Y;
			rectangleF.Height = secondRectPoint.Y - firstRectPoint.Y;
			rectangleF.Width = secondRectPoint.X - firstRectPoint.X;
			RectangleF rectangleF2 = default(RectangleF);
			rectangleF2.X = (float)(rectangleF.X + rectangleF.Width * (1.0 - doughnutRadius) / 2.0);
			rectangleF2.Y = (float)(rectangleF.Y + rectangleF.Height * (1.0 - doughnutRadius) / 2.0);
			rectangleF2.Height = rectangleF.Height * doughnutRadius;
			rectangleF2.Width = rectangleF.Width * doughnutRadius;
			double correction = (double)(rectangleF.Height / rectangleF.Width);
			float num = this.AngleCorrection(startAngle + sweepAngle, correction, (float)area.Area3DStyle.XAngle);
			startAngle = this.AngleCorrection(startAngle, correction, (float)area.Area3DStyle.XAngle);
			sweepAngle = num - startAngle;
			graphicsPath.AddLine(fourPoint, firstPoint);
			if (rectangleF.Height > 0.0)
			{
				graphicsPath.AddArc(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height, startAngle, sweepAngle);
			}
			graphicsPath.AddLine(secondPoint, threePoint);
			if (rectangleF2.Height > 0.0)
			{
				graphicsPath.AddArc(rectangleF2.X, rectangleF2.Y, rectangleF2.Width, rectangleF2.Height, startAngle + sweepAngle, (float)(0.0 - sweepAngle));
			}
			if (common.ProcessModePaint)
			{
				Color color = default(Color);
				Color color2 = default(Color);
				Color color3 = default(Color);
				Color color4 = default(Color);
				Color color5 = default(Color);
				Color color6 = default(Color);
				area.matrix3D.GetLight(brush.Color, out color, out color2, out color3, out color4, out color5, out color6);
				Pen pen2 = (Pen)pen.Clone();
				if (area.Area3DStyle.Light == LightStyle.Realistic && point.BorderColor == Color.Empty)
				{
					pen2.Color = color;
				}
				if (fill)
				{
					((ChartGraphics)this).FillPath(new SolidBrush(color), graphicsPath);
				}
				if (point.BorderColor != Color.Empty && point.BorderWidth > 0 && point.BorderStyle != 0)
				{
					this.DrawGraphicsPath(pen2, graphicsPath);
				}
			}
			if (common.ProcessModeRegions && fill)
			{
				if (point.IsAttributeSet("_COLLECTED_DATA_POINT"))
				{
					common.HotRegionsList.AddHotRegion((ChartGraphics)this, graphicsPath, false, point.ReplaceKeywords(point.ToolTip), point.ReplaceKeywords(point.Href), point.ReplaceKeywords(point.MapAreaAttributes), point, ChartElementType.DataPoint);
				}
				else
				{
					common.HotRegionsList.AddHotRegion(graphicsPath, false, (ChartGraphics)this, point, point.series.Name, pointIndex);
				}
			}
		}

		private void DrawGraphicsPath(Pen pen, GraphicsPath path)
		{
			if (pen.Width < 2.0)
			{
				((ChartGraphics)this).DrawPath(pen, path);
			}
			else
			{
				path.Flatten();
				pen.EndCap = LineCap.Round;
				pen.StartCap = LineCap.Round;
				PointF[] pathPoints = path.PathPoints;
				for (int i = 0; i < path.PathPoints.Length - 1; i++)
				{
					PointF[] array = new PointF[2]
					{
						pathPoints[i],
						pathPoints[i + 1]
					};
					((ChartGraphics)this).DrawLine(pen, array[0], array[1]);
				}
			}
		}

		private float AngleCorrection(float angle, double correction, float xAngle)
		{
			if (angle > -90.0 && angle < 90.0)
			{
				angle = (float)(Math.Atan(Math.Tan((double)angle * 3.1415926535897931 / 180.0) * correction) * 180.0 / 3.1415926535897931);
			}
			else if (angle > -270.0 && angle < -90.0)
			{
				angle = (float)(angle + 180.0);
				angle = (float)(Math.Atan(Math.Tan((double)angle * 3.1415926535897931 / 180.0) * correction) * 180.0 / 3.1415926535897931);
				angle = (float)(angle - 180.0);
			}
			else if (angle > 90.0 && angle < 270.0)
			{
				angle = (float)(angle - 180.0);
				angle = (float)(Math.Atan(Math.Tan((double)angle * 3.1415926535897931 / 180.0) * correction) * 180.0 / 3.1415926535897931);
				angle = (float)(angle + 180.0);
			}
			else if (angle > 270.0 && angle < 450.0)
			{
				angle = (float)(angle - 360.0);
				angle = (float)(Math.Atan(Math.Tan((double)angle * 3.1415926535897931 / 180.0) * correction) * 180.0 / 3.1415926535897931);
				angle = (float)(angle + 360.0);
			}
			else if (angle > 450.0)
			{
				angle = (float)(angle - 540.0);
				angle = (float)(Math.Atan(Math.Tan((double)angle * 3.1415926535897931 / 180.0) * correction) * 180.0 / 3.1415926535897931);
				angle = (float)(angle + 540.0);
			}
			return angle;
		}

		internal GraphicsPath Draw3DPolygon(ChartArea area, Matrix3D matrix, LightStyle lightStyle, SurfaceNames surfaceName, float positionZ, Color backColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, DataPoint3D firstPoint, DataPoint3D secondPoint, DataPoint3D thirdPoint, DataPoint3D fourthPoint, ArrayList points, int pointIndex, float tension, DrawingOperationTypes operationType, LineSegmentType lineSegmentType, SurfaceNames thinBorders)
		{
			bool flag = (operationType & DrawingOperationTypes.DrawElement) == DrawingOperationTypes.DrawElement;
			GraphicsPath graphicsPath = ((operationType & DrawingOperationTypes.CalcElementPath) == DrawingOperationTypes.CalcElementPath) ? new GraphicsPath() : null;
			Point3D[] array = new Point3D[4]
			{
				new Point3D((float)firstPoint.xPosition, (float)firstPoint.yPosition, positionZ),
				new Point3D((float)secondPoint.xPosition, (float)secondPoint.yPosition, positionZ),
				new Point3D((float)thirdPoint.xPosition, (float)thirdPoint.yPosition, positionZ),
				new Point3D((float)fourthPoint.xPosition, (float)fourthPoint.yPosition, positionZ)
			};
			matrix.TransformPoints(array);
			PointF[] array2 = new PointF[4]
			{
				((ChartGraphics)this).GetAbsolutePoint(array[0].PointF),
				((ChartGraphics)this).GetAbsolutePoint(array[1].PointF),
				((ChartGraphics)this).GetAbsolutePoint(array[2].PointF),
				((ChartGraphics)this).GetAbsolutePoint(array[3].PointF)
			};
			bool visiblePolygon = ChartGraphics3D.IsSurfaceVisible(array[0], array[1], array[2]);
			Color polygonLight = matrix.GetPolygonLight(array, backColor, visiblePolygon, (float)area.Area3DStyle.YAngle, surfaceName, area.reverseSeriesOrder);
			Color color = borderColor;
			if (color == Color.Empty)
			{
				color = ChartGraphics.GetGradientColor(backColor, Color.Black, 0.2);
			}
			Pen pen = null;
			if (flag)
			{
				SmoothingMode smoothingMode = ((ChartGraphics)this).SmoothingMode;
				((ChartGraphics)this).SmoothingMode = SmoothingMode.Default;
				((ChartGraphics)this).FillPolygon(new SolidBrush(polygonLight), array2);
				((ChartGraphics)this).SmoothingMode = smoothingMode;
				if (thinBorders != 0)
				{
					Pen pen2 = new Pen(color, 1f);
					if ((thinBorders & SurfaceNames.Left) != 0)
					{
						((ChartGraphics)this).DrawLine(pen2, array2[3], array2[0]);
					}
					if ((thinBorders & SurfaceNames.Right) != 0)
					{
						((ChartGraphics)this).DrawLine(pen2, array2[1], array2[2]);
					}
					if ((thinBorders & SurfaceNames.Top) != 0)
					{
						((ChartGraphics)this).DrawLine(pen2, array2[0], array2[1]);
					}
					if ((thinBorders & SurfaceNames.Bottom) != 0)
					{
						((ChartGraphics)this).DrawLine(pen2, array2[2], array2[3]);
					}
				}
				else if (polygonLight.A == 255)
				{
					((ChartGraphics)this).DrawPolygon(new Pen(polygonLight, 1f), array2);
				}
				pen = new Pen(color, (float)borderWidth);
				pen.StartCap = LineCap.Round;
				pen.EndCap = LineCap.Round;
				((ChartGraphics)this).DrawLine(pen, array2[0], array2[1]);
				((ChartGraphics)this).DrawLine(pen, array2[2], array2[3]);
				switch (lineSegmentType)
				{
				case LineSegmentType.First:
					((ChartGraphics)this).DrawLine(pen, array2[3], array2[0]);
					break;
				case LineSegmentType.Last:
					((ChartGraphics)this).DrawLine(pen, array2[1], array2[2]);
					break;
				}
			}
			if (area.Area3DStyle.Perspective == 0)
			{
				if (this.frontLinePoint1 != PointF.Empty && this.frontLinePen != null)
				{
					if ((this.frontLinePoint1.X != array2[0].X || this.frontLinePoint1.Y != array2[0].Y) && (this.frontLinePoint2.X != array2[1].X || this.frontLinePoint2.Y != array2[1].Y) && (this.frontLinePoint1.X != array2[1].X || this.frontLinePoint1.Y != array2[1].Y) && (this.frontLinePoint2.X != array2[0].X || this.frontLinePoint2.Y != array2[0].Y) && (this.frontLinePoint1.X != array2[3].X || this.frontLinePoint1.Y != array2[3].Y) && (this.frontLinePoint2.X != array2[2].X || this.frontLinePoint2.Y != array2[2].Y) && (this.frontLinePoint1.X != array2[2].X || this.frontLinePoint1.Y != array2[2].Y) && (this.frontLinePoint2.X != array2[3].X || this.frontLinePoint2.Y != array2[3].Y))
					{
						((ChartGraphics)this).DrawLine(this.frontLinePen, (float)Math.Round((double)this.frontLinePoint1.X), (float)Math.Round((double)this.frontLinePoint1.Y), (float)Math.Round((double)this.frontLinePoint2.X), (float)Math.Round((double)this.frontLinePoint2.Y));
					}
					this.frontLinePen = null;
					this.frontLinePoint1 = PointF.Empty;
					this.frontLinePoint2 = PointF.Empty;
				}
				if (flag)
				{
					this.frontLinePen = pen;
					this.frontLinePoint1 = array2[0];
					this.frontLinePoint2 = array2[1];
				}
			}
			if (graphicsPath != null)
			{
				graphicsPath.AddPolygon(array2);
			}
			return graphicsPath;
		}

		internal GraphicsPath GetSplineFlattenPath(ChartArea area, Matrix3D matrix, float positionZ, float depth, DataPoint3D firstPoint, DataPoint3D secondPoint, ArrayList points, int pointIndex, float tension, bool flatten, bool translateCoordinates, int yValueIndex)
		{
			int num = (firstPoint.index < secondPoint.index) ? firstPoint.index : secondPoint.index;
			num--;
			if (num >= points.Count - 2)
			{
				num--;
			}
			if (num < 1)
			{
				num = 1;
			}
			int num2 = -2147483648;
			DataPoint3D[] array = new DataPoint3D[4]
			{
				ChartGraphics3D.FindPointByIndex(points, num, null, ref num2),
				ChartGraphics3D.FindPointByIndex(points, num + 1, null, ref num2),
				ChartGraphics3D.FindPointByIndex(points, num + 2, null, ref num2),
				ChartGraphics3D.FindPointByIndex(points, num + 3, null, ref num2)
			};
			int i;
			for (i = 0; i < 4 && array[i].index != firstPoint.index && array[i].index != secondPoint.index; i++)
			{
			}
			int num3 = 2;
			if (array[2] != null)
			{
				num3++;
			}
			if (array[3] != null)
			{
				num3++;
			}
			PointF[] array2 = new PointF[num3];
			if (yValueIndex == 0)
			{
				array2[0] = new PointF((float)array[0].xPosition, (float)array[0].yPosition);
				array2[1] = new PointF((float)array[1].xPosition, (float)array[1].yPosition);
				if (num3 > 2)
				{
					array2[2] = new PointF((float)array[2].xPosition, (float)array[2].yPosition);
				}
				if (num3 > 3)
				{
					array2[3] = new PointF((float)array[3].xPosition, (float)array[3].yPosition);
				}
			}
			else
			{
				Axis axis = (firstPoint.dataPoint.series.YAxisType == AxisType.Primary) ? area.AxisY : area.AxisY2;
				float y = (float)axis.GetPosition(array[0].dataPoint.YValues[yValueIndex]);
				array2[0] = new PointF((float)array[0].xPosition, y);
				y = (float)axis.GetPosition(array[1].dataPoint.YValues[yValueIndex]);
				array2[1] = new PointF((float)array[1].xPosition, y);
				if (num3 > 2)
				{
					y = (float)axis.GetPosition(array[2].dataPoint.YValues[yValueIndex]);
					array2[2] = new PointF((float)array[2].xPosition, y);
				}
				if (num3 > 3)
				{
					y = (float)axis.GetPosition(array[3].dataPoint.YValues[yValueIndex]);
					array2[3] = new PointF((float)array[3].xPosition, y);
				}
			}
			if (translateCoordinates)
			{
				Point3D[] array3 = new Point3D[num3];
				for (int j = 0; j < num3; j++)
				{
					array3[j] = new Point3D(array2[j].X, array2[j].Y, positionZ);
				}
				area.matrix3D.TransformPoints(array3);
				for (int k = 0; k < num3; k++)
				{
					array2[k] = ((ChartGraphics)this).GetAbsolutePoint(array3[k].PointF);
				}
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddCurve(array2, i, 1, tension);
			if (flatten)
			{
				graphicsPath.Flatten();
			}
			if (firstPoint.index > secondPoint.index)
			{
				graphicsPath.Reverse();
			}
			return graphicsPath;
		}

		internal GraphicsPath Draw3DSplineSurface(ChartArea area, Matrix3D matrix, LightStyle lightStyle, SurfaceNames surfaceName, float positionZ, float depth, Color backColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, DataPoint3D firstPoint, DataPoint3D secondPoint, ArrayList points, int pointIndex, float tension, DrawingOperationTypes operationType, bool forceThinBorder, bool forceThickBorder, bool reversedSeriesOrder, bool multiSeries, int yValueIndex, bool clipInsideArea)
		{
			if (tension == 0.0)
			{
				return this.Draw3DSurface(area, matrix, lightStyle, surfaceName, positionZ, depth, backColor, borderColor, borderWidth, borderStyle, firstPoint, secondPoint, points, pointIndex, tension, operationType, LineSegmentType.Single, forceThinBorder, forceThickBorder, reversedSeriesOrder, multiSeries, yValueIndex, clipInsideArea);
			}
			GraphicsPath graphicsPath = ((operationType & DrawingOperationTypes.CalcElementPath) == DrawingOperationTypes.CalcElementPath) ? new GraphicsPath() : null;
			GraphicsPath splineFlattenPath = this.GetSplineFlattenPath(area, matrix, positionZ, depth, firstPoint, secondPoint, points, pointIndex, tension, true, false, yValueIndex);
			bool flag = false;
			if (pointIndex + 1 < points.Count)
			{
				DataPoint3D dataPoint3D = (DataPoint3D)points[pointIndex + 1];
				if (dataPoint3D.index == firstPoint.index)
				{
					flag = true;
				}
			}
			if (flag)
			{
				splineFlattenPath.Reverse();
			}
			PointF[] pathPoints = splineFlattenPath.PathPoints;
			DataPoint3D dataPoint3D2 = new DataPoint3D();
			DataPoint3D dataPoint3D3 = new DataPoint3D();
			LineSegmentType lineSegmentType = LineSegmentType.Middle;
			for (int i = 1; i < pathPoints.Length; i++)
			{
				bool forceThinBorder2 = false;
				bool forceThickBorder2 = false;
				if (!flag)
				{
					dataPoint3D2.index = firstPoint.index;
					dataPoint3D2.dataPoint = firstPoint.dataPoint;
					dataPoint3D2.xPosition = (double)pathPoints[i - 1].X;
					dataPoint3D2.yPosition = (double)pathPoints[i - 1].Y;
					dataPoint3D3.index = secondPoint.index;
					dataPoint3D3.index = secondPoint.index;
					dataPoint3D3.xPosition = (double)pathPoints[i].X;
					dataPoint3D3.yPosition = (double)pathPoints[i].Y;
				}
				else
				{
					dataPoint3D3.index = firstPoint.index;
					dataPoint3D3.dataPoint = firstPoint.dataPoint;
					dataPoint3D3.xPosition = (double)pathPoints[i - 1].X;
					dataPoint3D3.yPosition = (double)pathPoints[i - 1].Y;
					dataPoint3D2.index = secondPoint.index;
					dataPoint3D2.dataPoint = secondPoint.dataPoint;
					dataPoint3D2.xPosition = (double)pathPoints[i].X;
					dataPoint3D2.yPosition = (double)pathPoints[i].Y;
				}
				lineSegmentType = LineSegmentType.Middle;
				if (i == 1)
				{
					lineSegmentType = (LineSegmentType)((!flag) ? 1 : 3);
					forceThinBorder2 = forceThinBorder;
					forceThickBorder2 = forceThickBorder;
				}
				else if (i == pathPoints.Length - 1)
				{
					lineSegmentType = (LineSegmentType)(flag ? 1 : 3);
					forceThinBorder2 = forceThinBorder;
					forceThickBorder2 = forceThickBorder;
				}
				GraphicsPath graphicsPath2 = this.Draw3DSurface(area, matrix, lightStyle, surfaceName, positionZ, depth, backColor, borderColor, borderWidth, borderStyle, dataPoint3D2, dataPoint3D3, points, pointIndex, 0f, operationType, lineSegmentType, forceThinBorder2, forceThickBorder2, reversedSeriesOrder, multiSeries, yValueIndex, clipInsideArea);
				if (graphicsPath != null && graphicsPath2 != null && graphicsPath2.PointCount > 0)
				{
					graphicsPath.AddPath(graphicsPath2, true);
				}
			}
			return graphicsPath;
		}

		internal GraphicsPath Draw3DSurface(ChartArea area, Matrix3D matrix, LightStyle lightStyle, SurfaceNames surfaceName, float positionZ, float depth, Color backColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, DataPoint3D firstPoint, DataPoint3D secondPoint, ArrayList points, int pointIndex, float tension, DrawingOperationTypes operationType, LineSegmentType lineSegmentType, bool forceThinBorder, bool forceThickBorder, bool reversedSeriesOrder, bool multiSeries, int yValueIndex, bool clipInsideArea)
		{
			if (tension != 0.0)
			{
				return this.Draw3DSplineSurface(area, matrix, lightStyle, surfaceName, positionZ, depth, backColor, borderColor, borderWidth, borderStyle, firstPoint, secondPoint, points, pointIndex, tension, operationType, forceThinBorder, forceThickBorder, reversedSeriesOrder, multiSeries, yValueIndex, clipInsideArea);
			}
			bool flag = (operationType & DrawingOperationTypes.DrawElement) == DrawingOperationTypes.DrawElement;
			GraphicsPath graphicsPath = ((operationType & DrawingOperationTypes.CalcElementPath) == DrawingOperationTypes.CalcElementPath) ? new GraphicsPath() : null;
			if ((decimal)firstPoint.xPosition == (decimal)secondPoint.xPosition && (decimal)firstPoint.yPosition == (decimal)secondPoint.yPosition)
			{
				return graphicsPath;
			}
			decimal d2;
			decimal d4;
			double yPosition;
			double yPosition2;
			Color gradientColor;
			Color gradientColor2;
			DataPoint3D dataPoint3D;
			int num;
			DataPoint3D dataPoint3D2;
			if (clipInsideArea)
			{
				int decimals = 3;
				decimal d = Math.Round((decimal)area.PlotAreaPosition.X, decimals);
				d2 = Math.Round((decimal)area.PlotAreaPosition.Y, decimals);
				decimal d3 = Math.Round((decimal)area.PlotAreaPosition.Right(), decimals);
				d4 = Math.Round((decimal)area.PlotAreaPosition.Bottom(), decimals);
				d -= 0.001m;
				d2 -= 0.001m;
				d3 += 0.001m;
				d4 += 0.001m;
				if ((decimal)firstPoint.xPosition < d || (decimal)firstPoint.xPosition > d3 || (decimal)secondPoint.xPosition < d || (decimal)secondPoint.xPosition > d3)
				{
					if ((decimal)firstPoint.xPosition < d && (decimal)secondPoint.xPosition < d)
					{
						return graphicsPath;
					}
					if ((decimal)firstPoint.xPosition > d3 && (decimal)secondPoint.xPosition > d3)
					{
						return graphicsPath;
					}
					if ((decimal)firstPoint.xPosition < d)
					{
						firstPoint.yPosition = ((double)d - secondPoint.xPosition) / (firstPoint.xPosition - secondPoint.xPosition) * (firstPoint.yPosition - secondPoint.yPosition) + secondPoint.yPosition;
						firstPoint.xPosition = (double)d;
					}
					else if ((decimal)firstPoint.xPosition > d3)
					{
						firstPoint.yPosition = ((double)d3 - secondPoint.xPosition) / (firstPoint.xPosition - secondPoint.xPosition) * (firstPoint.yPosition - secondPoint.yPosition) + secondPoint.yPosition;
						firstPoint.xPosition = (double)d3;
					}
					if ((decimal)secondPoint.xPosition < d)
					{
						secondPoint.yPosition = ((double)d - secondPoint.xPosition) / (firstPoint.xPosition - secondPoint.xPosition) * (firstPoint.yPosition - secondPoint.yPosition) + secondPoint.yPosition;
						secondPoint.xPosition = (double)d;
					}
					else if ((decimal)secondPoint.xPosition > d3)
					{
						secondPoint.yPosition = ((double)d3 - secondPoint.xPosition) / (firstPoint.xPosition - secondPoint.xPosition) * (firstPoint.yPosition - secondPoint.yPosition) + secondPoint.yPosition;
						secondPoint.xPosition = (double)d3;
					}
				}
				if (!((decimal)firstPoint.yPosition < d2) && !((decimal)firstPoint.yPosition > d4) && !((decimal)secondPoint.yPosition < d2) && !((decimal)secondPoint.yPosition > d4))
				{
					goto IL_096c;
				}
				yPosition = firstPoint.yPosition;
				yPosition2 = secondPoint.yPosition;
				bool flag2 = false;
				if ((decimal)firstPoint.yPosition < d2 && (decimal)secondPoint.yPosition < d2)
				{
					flag2 = true;
					firstPoint.yPosition = (double)d2;
					secondPoint.yPosition = (double)d2;
				}
				if ((decimal)firstPoint.yPosition > d4 && (decimal)secondPoint.yPosition > d4)
				{
					flag2 = true;
					firstPoint.yPosition = (double)d4;
					secondPoint.yPosition = (double)d4;
				}
				gradientColor = ChartGraphics.GetGradientColor(backColor, Color.Black, 0.5);
				gradientColor2 = ChartGraphics.GetGradientColor(borderColor, Color.Black, 0.5);
				if (flag2)
				{
					graphicsPath = this.Draw3DSurface(area, matrix, lightStyle, surfaceName, positionZ, depth, gradientColor, gradientColor2, borderWidth, borderStyle, firstPoint, secondPoint, points, pointIndex, tension, operationType, lineSegmentType, forceThinBorder, forceThickBorder, reversedSeriesOrder, multiSeries, yValueIndex, clipInsideArea);
					firstPoint.yPosition = yPosition;
					secondPoint.yPosition = yPosition2;
					return graphicsPath;
				}
				dataPoint3D = new DataPoint3D();
				dataPoint3D.yPosition = (double)d2;
				if ((decimal)firstPoint.yPosition > d4 || (decimal)secondPoint.yPosition > d4)
				{
					dataPoint3D.yPosition = (double)d4;
				}
				dataPoint3D.xPosition = (dataPoint3D.yPosition - secondPoint.yPosition) * (firstPoint.xPosition - secondPoint.xPosition) / (firstPoint.yPosition - secondPoint.yPosition) + secondPoint.xPosition;
				num = 2;
				dataPoint3D2 = null;
				if ((decimal)firstPoint.yPosition < d2 && (decimal)secondPoint.yPosition > d4)
				{
					goto IL_0601;
				}
				if ((decimal)firstPoint.yPosition > d4 && (decimal)secondPoint.yPosition < d2)
				{
					goto IL_0601;
				}
				goto IL_06f0;
			}
			goto IL_096c;
			IL_0601:
			num = 3;
			dataPoint3D2 = new DataPoint3D();
			if ((decimal)dataPoint3D.yPosition == d2)
			{
				dataPoint3D2.yPosition = (double)d4;
			}
			else
			{
				dataPoint3D2.yPosition = (double)d2;
			}
			dataPoint3D2.xPosition = (dataPoint3D2.yPosition - secondPoint.yPosition) * (firstPoint.xPosition - secondPoint.xPosition) / (firstPoint.yPosition - secondPoint.yPosition) + secondPoint.xPosition;
			if ((decimal)firstPoint.yPosition > d4)
			{
				DataPoint3D dataPoint3D3 = new DataPoint3D();
				dataPoint3D3.xPosition = dataPoint3D.xPosition;
				dataPoint3D3.yPosition = dataPoint3D.yPosition;
				dataPoint3D.xPosition = dataPoint3D2.xPosition;
				dataPoint3D.yPosition = dataPoint3D2.yPosition;
				dataPoint3D2.xPosition = dataPoint3D3.xPosition;
				dataPoint3D2.yPosition = dataPoint3D3.yPosition;
			}
			goto IL_06f0;
			IL_06f0:
			bool flag3 = true;
			if ((decimal)firstPoint.yPosition < d2)
			{
				flag3 = false;
				firstPoint.yPosition = (double)d2;
			}
			else if ((decimal)firstPoint.yPosition > d4)
			{
				flag3 = false;
				firstPoint.yPosition = (double)d4;
			}
			if ((decimal)secondPoint.yPosition < d2)
			{
				secondPoint.yPosition = (double)d2;
			}
			else if ((decimal)secondPoint.yPosition > d4)
			{
				secondPoint.yPosition = (double)d4;
			}
			bool flag4 = false;
			if (pointIndex + 1 < points.Count)
			{
				DataPoint3D dataPoint3D4 = (DataPoint3D)points[pointIndex + 1];
				if (dataPoint3D4.index == firstPoint.index)
				{
					flag4 = true;
				}
			}
			int num2 = 0;
			while (num2 < 3)
			{
				GraphicsPath graphicsPath2 = null;
				if (num2 == 0 && !flag4)
				{
					goto IL_07e2;
				}
				if (num2 == 2 && flag4)
				{
					goto IL_07e2;
				}
				goto IL_0853;
				IL_07e2:
				if (dataPoint3D2 == null)
				{
					dataPoint3D2 = dataPoint3D;
				}
				dataPoint3D2.dataPoint = secondPoint.dataPoint;
				dataPoint3D2.index = secondPoint.index;
				graphicsPath2 = this.Draw3DSurface(area, matrix, lightStyle, surfaceName, positionZ, depth, (flag3 && num != 3) ? backColor : gradientColor, (flag3 && num != 3) ? borderColor : gradientColor2, borderWidth, borderStyle, firstPoint, dataPoint3D2, points, pointIndex, tension, operationType, lineSegmentType, forceThinBorder, forceThickBorder, reversedSeriesOrder, multiSeries, yValueIndex, clipInsideArea);
				goto IL_0853;
				IL_0853:
				if (num2 == 1 && dataPoint3D2 != null && num == 3)
				{
					dataPoint3D2.dataPoint = secondPoint.dataPoint;
					dataPoint3D2.index = secondPoint.index;
					graphicsPath2 = this.Draw3DSurface(area, matrix, lightStyle, surfaceName, positionZ, depth, backColor, borderColor, borderWidth, borderStyle, dataPoint3D, dataPoint3D2, points, pointIndex, tension, operationType, lineSegmentType, forceThinBorder, forceThickBorder, reversedSeriesOrder, multiSeries, yValueIndex, clipInsideArea);
				}
				if (num2 == 2 && !flag4)
				{
					goto IL_08c1;
				}
				if (num2 == 0 && flag4)
				{
					goto IL_08c1;
				}
				goto IL_092a;
				IL_092a:
				if (graphicsPath != null && graphicsPath2 != null && graphicsPath2.PointCount > 0)
				{
					graphicsPath.SetMarkers();
					graphicsPath.AddPath(graphicsPath2, true);
				}
				num2++;
				continue;
				IL_08c1:
				dataPoint3D.dataPoint = firstPoint.dataPoint;
				dataPoint3D.index = firstPoint.index;
				graphicsPath2 = this.Draw3DSurface(area, matrix, lightStyle, surfaceName, positionZ, depth, (!flag3 && num != 3) ? backColor : gradientColor, (!flag3 && num != 3) ? borderColor : gradientColor2, borderWidth, borderStyle, dataPoint3D, secondPoint, points, pointIndex, tension, operationType, lineSegmentType, forceThinBorder, forceThickBorder, reversedSeriesOrder, multiSeries, yValueIndex, clipInsideArea);
				goto IL_092a;
			}
			firstPoint.yPosition = yPosition;
			secondPoint.yPosition = yPosition2;
			return graphicsPath;
			IL_096c:
			Point3D[] array = new Point3D[4]
			{
				new Point3D((float)firstPoint.xPosition, (float)firstPoint.yPosition, positionZ + depth),
				new Point3D((float)secondPoint.xPosition, (float)secondPoint.yPosition, positionZ + depth),
				new Point3D((float)secondPoint.xPosition, (float)secondPoint.yPosition, positionZ),
				new Point3D((float)firstPoint.xPosition, (float)firstPoint.yPosition, positionZ)
			};
			matrix.TransformPoints(array);
			PointF[] array2 = new PointF[4]
			{
				((ChartGraphics)this).GetAbsolutePoint(array[0].PointF),
				((ChartGraphics)this).GetAbsolutePoint(array[1].PointF),
				((ChartGraphics)this).GetAbsolutePoint(array[2].PointF),
				((ChartGraphics)this).GetAbsolutePoint(array[3].PointF)
			};
			bool visiblePolygon = ChartGraphics3D.IsSurfaceVisible(array[0], array[1], array[2]);
			Color polygonLight = matrix.GetPolygonLight(array, backColor, visiblePolygon, (float)area.Area3DStyle.YAngle, surfaceName, area.reverseSeriesOrder);
			Color color = borderColor;
			if (color == Color.Empty)
			{
				color = ChartGraphics.GetGradientColor(backColor, Color.Black, 0.2);
			}
			Pen pen = new Pen(color, 1f);
			if (flag)
			{
				if (backColor != Color.Transparent)
				{
					SmoothingMode smoothingMode = ((ChartGraphics)this).SmoothingMode;
					((ChartGraphics)this).SmoothingMode = SmoothingMode.Default;
					((ChartGraphics)this).FillPolygon(new SolidBrush(polygonLight), array2);
					((ChartGraphics)this).SmoothingMode = smoothingMode;
				}
				if (forceThinBorder || forceThickBorder)
				{
					if (forceThickBorder)
					{
						Pen pen2 = new Pen(color, (float)borderWidth);
						pen2.StartCap = LineCap.Round;
						pen2.EndCap = LineCap.Round;
						((ChartGraphics)this).DrawLine(pen2, array2[0], array2[1]);
						((ChartGraphics)this).DrawLine(pen2, array2[2], array2[3]);
						((ChartGraphics)this).DrawLine(pen2, array2[3], array2[0]);
						((ChartGraphics)this).DrawLine(pen2, array2[1], array2[2]);
					}
					else
					{
						((ChartGraphics)this).DrawLine(pen, array2[0], array2[1]);
						((ChartGraphics)this).DrawLine(pen, array2[2], array2[3]);
						switch (lineSegmentType)
						{
						case LineSegmentType.First:
							((ChartGraphics)this).DrawLine(pen, array2[3], array2[0]);
							break;
						case LineSegmentType.Last:
							((ChartGraphics)this).DrawLine(pen, array2[1], array2[2]);
							break;
						default:
							((ChartGraphics)this).DrawLine(pen, array2[3], array2[0]);
							((ChartGraphics)this).DrawLine(pen, array2[1], array2[2]);
							break;
						}
					}
				}
				else
				{
					if (polygonLight.A == 255)
					{
						((ChartGraphics)this).DrawPolygon(new Pen(polygonLight, 1f), array2);
					}
					((ChartGraphics)this).DrawLine(pen, array2[0], array2[1]);
					((ChartGraphics)this).DrawLine(pen, array2[2], array2[3]);
				}
			}
			Pen pen3 = null;
			if (borderWidth > 1 && !forceThickBorder)
			{
				pen3 = new Pen(color, (float)borderWidth);
				pen3.StartCap = LineCap.Round;
				pen3.EndCap = LineCap.Round;
				bool reversed = false;
				if (firstPoint.index > secondPoint.index)
				{
					DataPoint3D dataPoint3D5 = firstPoint;
					firstPoint = secondPoint;
					secondPoint = dataPoint3D5;
					reversed = true;
				}
				float num3 = Math.Min(array[0].X, array[1].X);
				float num4 = Math.Min(array[0].Y, array[1].Y);
				float num5 = Math.Max(array[0].X, array[1].X);
				float num6 = Math.Max(array[0].Y, array[1].Y);
				RectangleF position = new RectangleF(num3, num4, num5 - num3, num6 - num4);
				SurfaceNames visibleSurfaces = this.GetVisibleSurfaces(position, positionZ, depth, matrix);
				bool flag5 = false;
				bool flag6 = false;
				if (lineSegmentType != LineSegmentType.Middle)
				{
					LineSegmentType lineSegmentType2 = LineSegmentType.Single;
					flag5 = (ChartGraphics3D.ShouldDrawLineChartSurface(area, reversedSeriesOrder, SurfaceNames.Left, visibleSurfaces, polygonLight, points, firstPoint, secondPoint, multiSeries, reversed, ref lineSegmentType2) == 2);
					flag6 = (ChartGraphics3D.ShouldDrawLineChartSurface(area, reversedSeriesOrder, SurfaceNames.Right, visibleSurfaces, polygonLight, points, firstPoint, secondPoint, multiSeries, reversed, ref lineSegmentType2) == 2);
				}
				if (reversedSeriesOrder)
				{
					bool flag7 = flag5;
					flag5 = flag6;
					flag6 = flag7;
				}
				if (lineSegmentType != LineSegmentType.First && lineSegmentType != 0)
				{
					flag5 = false;
				}
				if (lineSegmentType != LineSegmentType.Last && lineSegmentType != 0)
				{
					flag6 = false;
				}
				if (matrix.perspective != 0.0 || (matrix.angleX != 90.0 && matrix.angleX != -90.0 && matrix.angleY != 90.0 && matrix.angleY != -90.0 && matrix.angleY != 180.0 && matrix.angleY != -180.0))
				{
					if (flag)
					{
						((ChartGraphics)this).DrawLine(pen3, (float)Math.Round((double)array2[0].X), (float)Math.Round((double)array2[0].Y), (float)Math.Round((double)array2[1].X), (float)Math.Round((double)array2[1].Y));
					}
					if (graphicsPath != null)
					{
						graphicsPath.AddLine((float)Math.Round((double)array2[0].X), (float)Math.Round((double)array2[0].Y), (float)Math.Round((double)array2[1].X), (float)Math.Round((double)array2[1].Y));
					}
				}
				pen3.EndCap = LineCap.Flat;
				if ((matrix.perspective != 0.0 || (matrix.angleX != 90.0 && matrix.angleX != -90.0)) && flag5)
				{
					if (flag)
					{
						((ChartGraphics)this).DrawLine(pen3, (float)Math.Round((double)array2[3].X), (float)Math.Round((double)array2[3].Y), (float)Math.Round((double)array2[0].X), (float)Math.Round((double)array2[0].Y));
					}
					if (graphicsPath != null)
					{
						graphicsPath.AddLine((float)Math.Round((double)array2[3].X), (float)Math.Round((double)array2[3].Y), (float)Math.Round((double)array2[0].X), (float)Math.Round((double)array2[0].Y));
					}
				}
				if ((matrix.perspective != 0.0 || (matrix.angleX != 90.0 && matrix.angleX != -90.0)) && flag6)
				{
					if (flag)
					{
						((ChartGraphics)this).DrawLine(pen3, (float)Math.Round((double)array2[1].X), (float)Math.Round((double)array2[1].Y), (float)Math.Round((double)array2[2].X), (float)Math.Round((double)array2[2].Y));
					}
					if (graphicsPath != null)
					{
						graphicsPath.AddLine((float)Math.Round((double)array2[1].X), (float)Math.Round((double)array2[1].Y), (float)Math.Round((double)array2[2].X), (float)Math.Round((double)array2[2].Y));
					}
				}
			}
			if (area.Area3DStyle.Perspective == 0)
			{
				if (this.frontLinePoint1 != PointF.Empty && this.frontLinePen != null)
				{
					((ChartGraphics)this).DrawLine(this.frontLinePen, (float)Math.Round((double)this.frontLinePoint1.X), (float)Math.Round((double)this.frontLinePoint1.Y), (float)Math.Round((double)this.frontLinePoint2.X), (float)Math.Round((double)this.frontLinePoint2.Y));
					this.frontLinePen = null;
					this.frontLinePoint1 = PointF.Empty;
					this.frontLinePoint2 = PointF.Empty;
				}
				if (flag)
				{
					this.frontLinePen = ((borderWidth > 1) ? pen3 : pen);
					this.frontLinePoint1 = array2[0];
					this.frontLinePoint2 = array2[1];
				}
			}
			if (graphicsPath != null)
			{
				if (pen3 != null)
				{
					ChartGraphics.Widen(graphicsPath, pen3);
				}
				graphicsPath.AddPolygon(array2);
			}
			return graphicsPath;
		}

		internal static int ShouldDrawLineChartSurface(ChartArea area, bool reversedSeriesOrder, SurfaceNames surfaceName, SurfaceNames boundaryRectVisibleSurfaces, Color color, ArrayList points, DataPoint3D firstPoint, DataPoint3D secondPoint, bool multiSeries, bool reversed, ref LineSegmentType lineSegmentType)
		{
			int num = 0;
			Series series = firstPoint.dataPoint.series;
			Axis axis = (series.XAxisType == AxisType.Primary) ? area.AxisX : area.AxisX2;
			double viewMinimum = axis.GetViewMinimum();
			double viewMaximum = axis.GetViewMaximum();
			bool flag = color.A != 255;
			bool flag2 = false;
			bool flag3 = false;
			if (surfaceName == SurfaceNames.Left)
			{
				DataPoint3D dataPoint3D = null;
				DataPoint3D dataPoint3D2 = null;
				int num2 = -2147483648;
				if (!reversedSeriesOrder)
				{
					dataPoint3D = ChartGraphics3D.FindPointByIndex(points, Math.Min(firstPoint.index, secondPoint.index) - 1, multiSeries ? secondPoint : null, ref num2);
					dataPoint3D2 = ChartGraphics3D.FindPointByIndex(points, Math.Min(firstPoint.index, secondPoint.index), multiSeries ? secondPoint : null, ref num2);
				}
				else
				{
					dataPoint3D = ChartGraphics3D.FindPointByIndex(points, Math.Max(firstPoint.index, secondPoint.index) + 1, multiSeries ? secondPoint : null, ref num2);
					dataPoint3D2 = dataPoint3D;
				}
				if (dataPoint3D != null)
				{
					if (dataPoint3D2.dataPoint.Empty)
					{
						if (dataPoint3D2.dataPoint.series.EmptyPointStyle.Color == color || dataPoint3D2.dataPoint.series.EmptyPointStyle.Color.A == 255)
						{
							flag2 = true;
						}
					}
					else if (dataPoint3D2.dataPoint.Color == color || dataPoint3D2.dataPoint.Color.A == 255)
					{
						flag2 = true;
					}
					double num3 = dataPoint3D.indexedSeries ? ((double)dataPoint3D.index) : dataPoint3D.dataPoint.XValue;
					if (num3 > viewMaximum || num3 < viewMinimum)
					{
						DataPoint3D dataPoint3D3 = null;
						dataPoint3D3 = ((!reversedSeriesOrder) ? ((firstPoint.index < secondPoint.index) ? firstPoint : secondPoint) : ((firstPoint.index > secondPoint.index) ? firstPoint : secondPoint));
						double num4 = dataPoint3D3.indexedSeries ? ((double)dataPoint3D3.index) : dataPoint3D3.dataPoint.XValue;
						if (num4 > viewMaximum || num4 < viewMinimum)
						{
							flag2 = false;
						}
					}
				}
			}
			if (surfaceName == SurfaceNames.Right)
			{
				DataPoint3D dataPoint3D4 = null;
				DataPoint3D dataPoint3D5 = null;
				int num5 = -2147483648;
				if (!reversedSeriesOrder)
				{
					dataPoint3D4 = ChartGraphics3D.FindPointByIndex(points, Math.Max(firstPoint.index, secondPoint.index) + 1, multiSeries ? secondPoint : null, ref num5);
					dataPoint3D5 = dataPoint3D4;
				}
				else
				{
					dataPoint3D4 = ChartGraphics3D.FindPointByIndex(points, Math.Min(firstPoint.index, secondPoint.index) - 1, multiSeries ? secondPoint : null, ref num5);
					dataPoint3D5 = ChartGraphics3D.FindPointByIndex(points, Math.Min(firstPoint.index, secondPoint.index), multiSeries ? secondPoint : null, ref num5);
				}
				if (dataPoint3D4 != null)
				{
					if (dataPoint3D5.dataPoint.Empty)
					{
						if (dataPoint3D5.dataPoint.series.EmptyPointStyle.Color == color || dataPoint3D5.dataPoint.series.EmptyPointStyle.Color.A == 255)
						{
							flag3 = true;
						}
					}
					else if (dataPoint3D5.dataPoint.Color == color || dataPoint3D5.dataPoint.Color.A == 255)
					{
						flag3 = true;
					}
					double num6 = dataPoint3D4.indexedSeries ? ((double)dataPoint3D4.index) : dataPoint3D4.dataPoint.XValue;
					if (num6 > viewMaximum || num6 < viewMinimum)
					{
						DataPoint3D dataPoint3D6 = null;
						dataPoint3D6 = ((!reversedSeriesOrder) ? ((firstPoint.index < secondPoint.index) ? firstPoint : secondPoint) : ((firstPoint.index > secondPoint.index) ? firstPoint : secondPoint));
						double num7 = dataPoint3D6.indexedSeries ? ((double)dataPoint3D6.index) : dataPoint3D6.dataPoint.XValue;
						if (num7 > viewMaximum || num7 < viewMinimum)
						{
							flag3 = false;
						}
					}
				}
			}
			if (surfaceName == SurfaceNames.Left && !flag2)
			{
				if (lineSegmentType == LineSegmentType.Middle)
				{
					lineSegmentType = LineSegmentType.First;
				}
				else if (lineSegmentType == LineSegmentType.Last)
				{
					lineSegmentType = LineSegmentType.Single;
				}
			}
			if (surfaceName == SurfaceNames.Right && !flag3)
			{
				if (lineSegmentType == LineSegmentType.Middle)
				{
					lineSegmentType = LineSegmentType.Last;
				}
				else if (lineSegmentType == LineSegmentType.First)
				{
					lineSegmentType = LineSegmentType.Single;
				}
			}
			if (surfaceName == SurfaceNames.Top)
			{
				num = (((boundaryRectVisibleSurfaces & SurfaceNames.Top) != SurfaceNames.Top) ? 1 : 2);
			}
			if (surfaceName == SurfaceNames.Bottom)
			{
				num = (((boundaryRectVisibleSurfaces & SurfaceNames.Bottom) != SurfaceNames.Bottom) ? 1 : 2);
				if (num == 1 && !flag)
				{
					num = 0;
				}
			}
			if (surfaceName == SurfaceNames.Front)
			{
				num = (((boundaryRectVisibleSurfaces & SurfaceNames.Front) != SurfaceNames.Front) ? 1 : 2);
				if (num == 1 && !flag)
				{
					num = 0;
				}
			}
			if (surfaceName == SurfaceNames.Back)
			{
				num = (((boundaryRectVisibleSurfaces & SurfaceNames.Back) != SurfaceNames.Back) ? 1 : 2);
				if (num == 1 && !flag)
				{
					num = 0;
				}
			}
			if (surfaceName == SurfaceNames.Left)
			{
				num = (((boundaryRectVisibleSurfaces & SurfaceNames.Left) != SurfaceNames.Left) ? 1 : 2);
				if (flag2)
				{
					num = 0;
				}
			}
			if (surfaceName == SurfaceNames.Right)
			{
				num = (((boundaryRectVisibleSurfaces & SurfaceNames.Right) != SurfaceNames.Right) ? 1 : 2);
				if (flag3)
				{
					num = 0;
				}
			}
			return num;
		}

		internal static DataPoint3D FindPointByIndex(ArrayList points, int index, DataPoint3D neighborDataPoint, ref int neighborPointIndex)
		{
			if (neighborPointIndex != -2147483648)
			{
				if (neighborPointIndex < points.Count - 2)
				{
					DataPoint3D dataPoint3D = (DataPoint3D)points[neighborPointIndex + 1];
					if (dataPoint3D.index == index && (neighborDataPoint == null || string.Compare(neighborDataPoint.dataPoint.series.Name, dataPoint3D.dataPoint.series.Name, StringComparison.Ordinal) == 0))
					{
						neighborPointIndex++;
						return dataPoint3D;
					}
				}
				if (neighborPointIndex > 0)
				{
					DataPoint3D dataPoint3D2 = (DataPoint3D)points[neighborPointIndex - 1];
					if (dataPoint3D2.index == index && (neighborDataPoint == null || string.Compare(neighborDataPoint.dataPoint.series.Name, dataPoint3D2.dataPoint.series.Name, StringComparison.Ordinal) == 0))
					{
						neighborPointIndex--;
						return dataPoint3D2;
					}
				}
			}
			neighborPointIndex = 0;
			foreach (object point in points)
			{
				if (((DataPoint3D)point).index == index)
				{
					if (neighborDataPoint != null && string.Compare(neighborDataPoint.dataPoint.series.Name, ((DataPoint3D)point).dataPoint.series.Name, StringComparison.Ordinal) != 0)
					{
						neighborPointIndex++;
						continue;
					}
					return (DataPoint3D)point;
				}
				neighborPointIndex++;
			}
			return null;
		}

		internal GraphicsPath Fill3DRectangle(RectangleF position, float positionZ, float depth, Matrix3D matrix, LightStyle lightStyle, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, PenAlignment penAlignment, DrawingOperationTypes operationType)
		{
			return this.Fill3DRectangle(position, positionZ, depth, matrix, lightStyle, backColor, 0f, 0f, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backGradientEndColor, borderColor, borderWidth, borderStyle, penAlignment, BarDrawingStyle.Default, false, operationType);
		}

		internal GraphicsPath Fill3DRectangle(RectangleF position, float positionZ, float depth, Matrix3D matrix, LightStyle lightStyle, Color backColor, float topRightDarkening, float bottomLeftDarkening, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, PenAlignment penAlignment, BarDrawingStyle barDrawingStyle, bool veticalOrientation, DrawingOperationTypes operationType)
		{
			if (barDrawingStyle == BarDrawingStyle.Cylinder && base.ActiveRenderingType != RenderingType.Svg)
			{
				return this.Fill3DRectangleAsCylinder(position, positionZ, depth, matrix, lightStyle, backColor, topRightDarkening, bottomLeftDarkening, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backGradientEndColor, borderColor, borderWidth, borderStyle, penAlignment, veticalOrientation, operationType);
			}
			Point3D[] array = new Point3D[8];
			GraphicsPath graphicsPath = ((operationType & DrawingOperationTypes.CalcElementPath) == DrawingOperationTypes.CalcElementPath) ? new GraphicsPath() : null;
			array[0] = new Point3D(position.X, position.Y, positionZ + depth);
			array[1] = new Point3D(position.X, position.Bottom, positionZ + depth);
			array[2] = new Point3D(position.Right, position.Bottom, positionZ + depth);
			array[3] = new Point3D(position.Right, position.Y, positionZ + depth);
			array[4] = new Point3D(position.X, position.Y, positionZ);
			array[5] = new Point3D(position.X, position.Bottom, positionZ);
			array[6] = new Point3D(position.Right, position.Bottom, positionZ);
			array[7] = new Point3D(position.Right, position.Y, positionZ);
			matrix.TransformPoints(array);
			if (lightStyle == LightStyle.None && (borderWidth == 0 || borderStyle == ChartDashStyle.NotSet || borderColor == Color.Empty))
			{
				borderColor = ChartGraphics.GetGradientColor(backColor, Color.Black, 0.5);
			}
			Color color = default(Color);
			Color color2 = default(Color);
			Color gradientColor = default(Color);
			Color gradientColor2 = default(Color);
			Color gradientColor3 = default(Color);
			Color gradientColor4 = default(Color);
			matrix.GetLight(backColor, out color, out color2, out gradientColor, out gradientColor2, out gradientColor3, out gradientColor4);
			if (topRightDarkening != 0.0)
			{
				if (veticalOrientation)
				{
					gradientColor3 = ChartGraphics.GetGradientColor(gradientColor3, Color.Black, (double)topRightDarkening);
				}
				else
				{
					gradientColor2 = ChartGraphics.GetGradientColor(gradientColor2, Color.Black, (double)topRightDarkening);
				}
			}
			if (bottomLeftDarkening != 0.0)
			{
				if (veticalOrientation)
				{
					gradientColor4 = ChartGraphics.GetGradientColor(gradientColor4, Color.Black, (double)bottomLeftDarkening);
				}
				else
				{
					gradientColor = ChartGraphics.GetGradientColor(gradientColor, Color.Black, (double)bottomLeftDarkening);
				}
			}
			SurfaceNames visibleSurfacesWithPerspective = this.GetVisibleSurfacesWithPerspective(position, positionZ, depth, matrix);
			for (int i = 0; i <= 1; i++)
			{
				if (i != 0 || backColor.A != 255)
				{
					for (int num = 1; num <= 32; num *= 2)
					{
						SurfaceNames surfaceNames = (SurfaceNames)num;
						if (((double)depth != 0.0 || surfaceNames == SurfaceNames.Front) && ((double)position.Width != 0.0 || surfaceNames == SurfaceNames.Left || surfaceNames == SurfaceNames.Right) && ((double)position.Height != 0.0 || surfaceNames == SurfaceNames.Top || surfaceNames == SurfaceNames.Bottom))
						{
							bool flag = (visibleSurfacesWithPerspective & surfaceNames) != (SurfaceNames)0;
							if (flag && i == 1)
							{
								goto IL_0272;
							}
							if (!flag && i == 0)
							{
								goto IL_0272;
							}
						}
						continue;
						IL_0272:
						PointF[] array2 = new PointF[4];
						Color color3 = backColor;
						switch (surfaceNames)
						{
						case SurfaceNames.Front:
							color3 = color;
							array2[0] = new PointF(array[0].X, array[0].Y);
							array2[1] = new PointF(array[1].X, array[1].Y);
							array2[2] = new PointF(array[2].X, array[2].Y);
							array2[3] = new PointF(array[3].X, array[3].Y);
							break;
						case SurfaceNames.Back:
							color3 = color2;
							array2[0] = new PointF(array[4].X, array[4].Y);
							array2[1] = new PointF(array[5].X, array[5].Y);
							array2[2] = new PointF(array[6].X, array[6].Y);
							array2[3] = new PointF(array[7].X, array[7].Y);
							break;
						case SurfaceNames.Left:
							color3 = gradientColor;
							array2[0] = new PointF(array[0].X, array[0].Y);
							array2[1] = new PointF(array[1].X, array[1].Y);
							array2[2] = new PointF(array[5].X, array[5].Y);
							array2[3] = new PointF(array[4].X, array[4].Y);
							break;
						case SurfaceNames.Right:
							color3 = gradientColor2;
							array2[0] = new PointF(array[3].X, array[3].Y);
							array2[1] = new PointF(array[2].X, array[2].Y);
							array2[2] = new PointF(array[6].X, array[6].Y);
							array2[3] = new PointF(array[7].X, array[7].Y);
							break;
						case SurfaceNames.Top:
							color3 = gradientColor3;
							array2[0] = new PointF(array[0].X, array[0].Y);
							array2[1] = new PointF(array[3].X, array[3].Y);
							array2[2] = new PointF(array[7].X, array[7].Y);
							array2[3] = new PointF(array[4].X, array[4].Y);
							break;
						case SurfaceNames.Bottom:
							color3 = gradientColor4;
							array2[0] = new PointF(array[1].X, array[1].Y);
							array2[1] = new PointF(array[2].X, array[2].Y);
							array2[2] = new PointF(array[6].X, array[6].Y);
							array2[3] = new PointF(array[5].X, array[5].Y);
							break;
						}
						for (int j = 0; j < array2.Length; j++)
						{
							array2[j] = ((ChartGraphics)this).GetAbsolutePoint(array2[j]);
						}
						if ((operationType & DrawingOperationTypes.DrawElement) == DrawingOperationTypes.DrawElement)
						{
							if ((visibleSurfacesWithPerspective & surfaceNames) != 0)
							{
								((ChartGraphics)this).FillPolygon(new SolidBrush(color3), array2);
								if (surfaceNames == SurfaceNames.Front && barDrawingStyle != 0 && barDrawingStyle != BarDrawingStyle.Cylinder)
								{
									this.DrawBarStyleGradients(matrix, barDrawingStyle, position, positionZ, depth, veticalOrientation);
								}
							}
							Pen pen = new Pen(borderColor, (float)borderWidth);
							pen.DashStyle = ((ChartGraphics)this).GetPenStyle(borderStyle);
							if (lightStyle != 0 && (borderWidth == 0 || borderStyle == ChartDashStyle.NotSet || borderColor == Color.Empty))
							{
								pen = new Pen(color3, 1f);
								pen.Alignment = PenAlignment.Inset;
							}
							pen.StartCap = LineCap.Round;
							pen.EndCap = LineCap.Round;
							((ChartGraphics)this).DrawLine(pen, array2[0], array2[1]);
							((ChartGraphics)this).DrawLine(pen, array2[1], array2[2]);
							((ChartGraphics)this).DrawLine(pen, array2[2], array2[3]);
							((ChartGraphics)this).DrawLine(pen, array2[3], array2[0]);
						}
						if ((operationType & DrawingOperationTypes.CalcElementPath) == DrawingOperationTypes.CalcElementPath && (visibleSurfacesWithPerspective & surfaceNames) != 0)
						{
							graphicsPath.SetMarkers();
							graphicsPath.AddPolygon(array2);
						}
					}
				}
			}
			return graphicsPath;
		}

		private void DrawBarStyleGradients(Matrix3D matrix, BarDrawingStyle barDrawingStyle, RectangleF position, float positionZ, float depth, bool isVertical)
		{
			switch (barDrawingStyle)
			{
			case BarDrawingStyle.Wedge:
			{
				RectangleF absoluteRectangle2 = ((ChartGraphics)this).GetAbsoluteRectangle(position);
				float num2 = (float)(isVertical ? (absoluteRectangle2.Width / 2.0) : (absoluteRectangle2.Height / 2.0));
				if (isVertical && 2.0 * num2 > absoluteRectangle2.Height)
				{
					num2 = (float)(absoluteRectangle2.Height / 2.0);
				}
				if (!isVertical && 2.0 * num2 > absoluteRectangle2.Width)
				{
					num2 = (float)(absoluteRectangle2.Width / 2.0);
				}
				SizeF relativeSize2 = ((ChartGraphics)this).GetRelativeSize(new SizeF(num2, num2));
				Point3D[] array3 = new Point3D[6]
				{
					new Point3D(position.Left, position.Top, positionZ + depth),
					new Point3D(position.Left, position.Bottom, positionZ + depth),
					new Point3D(position.Right, position.Bottom, positionZ + depth),
					new Point3D(position.Right, position.Top, positionZ + depth),
					null,
					null
				};
				if (isVertical)
				{
					array3[4] = new Point3D((float)(position.X + position.Width / 2.0), position.Top + relativeSize2.Height, positionZ + depth);
					array3[5] = new Point3D((float)(position.X + position.Width / 2.0), position.Bottom - relativeSize2.Height, positionZ + depth);
				}
				else
				{
					array3[4] = new Point3D(position.X + relativeSize2.Width, (float)(position.Top + position.Height / 2.0), positionZ + depth);
					array3[5] = new Point3D(position.Right - relativeSize2.Width, (float)(position.Top + position.Height / 2.0), positionZ + depth);
				}
				matrix.TransformPoints(array3);
				PointF[] array4 = new PointF[6];
				for (int k = 0; k < array3.Length; k++)
				{
					array4[k] = ((ChartGraphics)this).GetAbsolutePoint(array3[k].PointF);
				}
				using (GraphicsPath graphicsPath3 = new GraphicsPath())
				{
					if (isVertical)
					{
						graphicsPath3.AddLine(array4[4], array4[5]);
						graphicsPath3.AddLine(array4[5], array4[2]);
						graphicsPath3.AddLine(array4[2], array4[3]);
					}
					else
					{
						graphicsPath3.AddLine(array4[4], array4[5]);
						graphicsPath3.AddLine(array4[5], array4[2]);
						graphicsPath3.AddLine(array4[2], array4[1]);
					}
					graphicsPath3.CloseAllFigures();
					using (SolidBrush brush3 = new SolidBrush(Color.FromArgb(90, Color.Black)))
					{
						base.FillPath(brush3, graphicsPath3);
					}
				}
				using (GraphicsPath graphicsPath4 = new GraphicsPath())
				{
					if (isVertical)
					{
						graphicsPath4.AddLine(array4[0], array4[4]);
						graphicsPath4.AddLine(array4[4], array4[3]);
					}
					else
					{
						graphicsPath4.AddLine(array4[3], array4[5]);
						graphicsPath4.AddLine(array4[5], array4[2]);
					}
					using (SolidBrush brush4 = new SolidBrush(Color.FromArgb(50, Color.Black)))
					{
						base.FillPath(brush4, graphicsPath4);
						using (Pen pen = new Pen(Color.FromArgb(20, Color.Black), 1f))
						{
							base.DrawPath(pen, graphicsPath4);
							base.DrawLine(pen, array4[4], array4[5]);
						}
						using (Pen pen2 = new Pen(Color.FromArgb(40, Color.White), 1f))
						{
							base.DrawPath(pen2, graphicsPath4);
							base.DrawLine(pen2, array4[4], array4[5]);
						}
					}
				}
				using (GraphicsPath graphicsPath5 = new GraphicsPath())
				{
					if (isVertical)
					{
						graphicsPath5.AddLine(array4[1], array4[5]);
						graphicsPath5.AddLine(array4[5], array4[2]);
					}
					else
					{
						graphicsPath5.AddLine(array4[0], array4[4]);
						graphicsPath5.AddLine(array4[4], array4[1]);
					}
					using (SolidBrush brush5 = new SolidBrush(Color.FromArgb(50, Color.Black)))
					{
						base.FillPath(brush5, graphicsPath5);
						using (Pen pen3 = new Pen(Color.FromArgb(20, Color.Black), 1f))
						{
							base.DrawPath(pen3, graphicsPath5);
						}
						using (Pen pen4 = new Pen(Color.FromArgb(40, Color.White), 1f))
						{
							base.DrawPath(pen4, graphicsPath5);
						}
					}
				}
				break;
			}
			case BarDrawingStyle.LightToDark:
			{
				RectangleF absoluteRectangle3 = ((ChartGraphics)this).GetAbsoluteRectangle(position);
				float num3 = 5f;
				if (absoluteRectangle3.Width < 6.0 || absoluteRectangle3.Height < 6.0)
				{
					num3 = 2f;
				}
				else if (absoluteRectangle3.Width < 15.0 || absoluteRectangle3.Height < 15.0)
				{
					num3 = 3f;
				}
				SizeF relativeSize3 = ((ChartGraphics)this).GetRelativeSize(new SizeF(num3, num3));
				RectangleF rectangleF = position;
				rectangleF.Inflate((float)(0.0 - relativeSize3.Width), (float)(0.0 - relativeSize3.Height));
				if (isVertical)
				{
					rectangleF.Height = (float)Math.Floor(rectangleF.Height / 3.0);
				}
				else
				{
					rectangleF.X = rectangleF.Right - (float)Math.Floor(rectangleF.Width / 3.0);
					rectangleF.Width = (float)Math.Floor(rectangleF.Width / 3.0);
				}
				Point3D[] array5 = new Point3D[4]
				{
					new Point3D(rectangleF.Left, rectangleF.Top, positionZ + depth),
					new Point3D(rectangleF.Left, rectangleF.Bottom, positionZ + depth),
					new Point3D(rectangleF.Right, rectangleF.Bottom, positionZ + depth),
					new Point3D(rectangleF.Right, rectangleF.Top, positionZ + depth)
				};
				matrix.TransformPoints(array5);
				PointF[] array6 = new PointF[4];
				for (int l = 0; l < array5.Length; l++)
				{
					array6[l] = ((ChartGraphics)this).GetAbsolutePoint(array5[l].PointF);
				}
				using (GraphicsPath graphicsPath6 = new GraphicsPath())
				{
					graphicsPath6.AddPolygon(array6);
					RectangleF bounds = graphicsPath6.GetBounds();
					bounds.Width += 1f;
					bounds.Height += 1f;
					if (bounds.Width > 0.0 && bounds.Height > 0.0)
					{
						using (LinearGradientBrush brush6 = new LinearGradientBrush(bounds, (!isVertical) ? Color.Transparent : Color.FromArgb(120, Color.White), (!isVertical) ? Color.FromArgb(120, Color.White) : Color.Transparent, (LinearGradientMode)(isVertical ? 1 : 0)))
						{
							base.FillPath(brush6, graphicsPath6);
						}
					}
				}
				rectangleF = position;
				rectangleF.Inflate((float)(0.0 - relativeSize3.Width), (float)(0.0 - relativeSize3.Height));
				if (isVertical)
				{
					rectangleF.Y = rectangleF.Bottom - (float)Math.Floor(rectangleF.Height / 3.0);
					rectangleF.Height = (float)Math.Floor(rectangleF.Height / 3.0);
				}
				else
				{
					rectangleF.Width = (float)Math.Floor(rectangleF.Width / 3.0);
				}
				array5 = new Point3D[4]
				{
					new Point3D(rectangleF.Left, rectangleF.Top, positionZ + depth),
					new Point3D(rectangleF.Left, rectangleF.Bottom, positionZ + depth),
					new Point3D(rectangleF.Right, rectangleF.Bottom, positionZ + depth),
					new Point3D(rectangleF.Right, rectangleF.Top, positionZ + depth)
				};
				matrix.TransformPoints(array5);
				array6 = new PointF[4];
				for (int m = 0; m < array5.Length; m++)
				{
					array6[m] = ((ChartGraphics)this).GetAbsolutePoint(array5[m].PointF);
				}
				using (GraphicsPath graphicsPath7 = new GraphicsPath())
				{
					graphicsPath7.AddPolygon(array6);
					RectangleF bounds2 = graphicsPath7.GetBounds();
					bounds2.Width += 1f;
					bounds2.Height += 1f;
					if (bounds2.Width > 0.0 && bounds2.Height > 0.0)
					{
						using (LinearGradientBrush brush7 = new LinearGradientBrush(bounds2, isVertical ? Color.Transparent : Color.FromArgb(80, Color.Black), isVertical ? Color.FromArgb(80, Color.Black) : Color.Transparent, (LinearGradientMode)(isVertical ? 1 : 0)))
						{
							base.FillPath(brush7, graphicsPath7);
						}
					}
				}
				break;
			}
			case BarDrawingStyle.Emboss:
			{
				RectangleF absoluteRectangle = ((ChartGraphics)this).GetAbsoluteRectangle(position);
				float num = 4f;
				if (absoluteRectangle.Width < 6.0 || absoluteRectangle.Height < 6.0)
				{
					num = 2f;
				}
				else if (absoluteRectangle.Width < 15.0 || absoluteRectangle.Height < 15.0)
				{
					num = 3f;
				}
				SizeF relativeSize = ((ChartGraphics)this).GetRelativeSize(new SizeF(num, num));
				Point3D[] array = new Point3D[6]
				{
					new Point3D(position.Left, position.Bottom, positionZ + depth),
					new Point3D(position.Left, position.Top, positionZ + depth),
					new Point3D(position.Right, position.Top, positionZ + depth),
					new Point3D(position.Right - relativeSize.Width, position.Top + relativeSize.Height, positionZ + depth),
					new Point3D(position.Left + relativeSize.Width, position.Top + relativeSize.Height, positionZ + depth),
					new Point3D(position.Left + relativeSize.Width, position.Bottom - relativeSize.Height, positionZ + depth)
				};
				matrix.TransformPoints(array);
				PointF[] array2 = new PointF[6];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = ((ChartGraphics)this).GetAbsolutePoint(array[i].PointF);
				}
				using (GraphicsPath graphicsPath = new GraphicsPath())
				{
					graphicsPath.AddPolygon(array2);
					using (SolidBrush brush = new SolidBrush(Color.FromArgb(100, Color.White)))
					{
						base.FillPath(brush, graphicsPath);
					}
				}
				array[0] = new Point3D(position.Right, position.Top, positionZ + depth);
				array[1] = new Point3D(position.Right, position.Bottom, positionZ + depth);
				array[2] = new Point3D(position.Left, position.Bottom, positionZ + depth);
				array[3] = new Point3D(position.Left + relativeSize.Width, position.Bottom - relativeSize.Height, positionZ + depth);
				array[4] = new Point3D(position.Right - relativeSize.Width, position.Bottom - relativeSize.Height, positionZ + depth);
				array[5] = new Point3D(position.Right - relativeSize.Width, position.Top + relativeSize.Height, positionZ + depth);
				matrix.TransformPoints(array);
				for (int j = 0; j < array.Length; j++)
				{
					array2[j] = ((ChartGraphics)this).GetAbsolutePoint(array[j].PointF);
				}
				using (GraphicsPath graphicsPath2 = new GraphicsPath())
				{
					graphicsPath2.AddPolygon(array2);
					using (SolidBrush brush2 = new SolidBrush(Color.FromArgb(80, Color.Black)))
					{
						base.FillPath(brush2, graphicsPath2);
					}
				}
				break;
			}
			}
		}

		internal GraphicsPath DrawMarker3D(Matrix3D matrix, LightStyle lightStyle, float positionZ, PointF point, MarkerStyle markerStyle, int markerSize, Color markerColor, Color markerBorderColor, int markerBorderSize, string markerImage, Color markerImageTranspColor, int shadowSize, Color shadowColor, RectangleF imageScaleRect, DrawingOperationTypes operationType)
		{
			ChartGraphics chartGraphics = (ChartGraphics)this;
			GraphicsPath graphicsPath = ((operationType & DrawingOperationTypes.CalcElementPath) == DrawingOperationTypes.CalcElementPath) ? new GraphicsPath() : null;
			Point3D[] array = new Point3D[1]
			{
				new Point3D(point.X, point.Y, positionZ)
			};
			matrix.TransformPoints(array);
			PointF pointF = array[0].PointF;
			pointF = chartGraphics.GetAbsolutePoint(pointF);
			if (markerImage.Length <= 0 && (markerStyle == MarkerStyle.Circle || markerStyle == MarkerStyle.Square))
			{
				if (markerStyle != 0 && markerSize > 0 && markerColor != Color.Empty)
				{
					RectangleF empty = RectangleF.Empty;
					empty.X = (float)(pointF.X - (float)markerSize / 2.0);
					empty.Y = (float)(pointF.Y - (float)markerSize / 2.0);
					empty.Width = (float)markerSize;
					empty.Height = (float)markerSize;
					SizeF relativeSize = chartGraphics.GetRelativeSize(new SizeF((float)markerSize, (float)markerSize));
					switch (markerStyle)
					{
					case MarkerStyle.Circle:
						if ((operationType & DrawingOperationTypes.DrawElement) == DrawingOperationTypes.DrawElement)
						{
							if (shadowSize != 0 && shadowColor != Color.Empty)
							{
								if (!chartGraphics.softShadows)
								{
									SolidBrush brush = new SolidBrush((shadowColor.A != 255) ? shadowColor : Color.FromArgb((int)markerColor.A / 2, shadowColor));
									RectangleF rect = empty;
									rect.X += (float)shadowSize;
									rect.Y += (float)shadowSize;
									chartGraphics.FillEllipse(brush, rect);
								}
								else
								{
									GraphicsPath graphicsPath2 = new GraphicsPath();
									graphicsPath2.AddEllipse((float)(empty.X + (float)shadowSize - 1.0), (float)(empty.Y + (float)shadowSize - 1.0), (float)(empty.Width + 2.0), (float)(empty.Height + 2.0));
									PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath2);
									pathGradientBrush.CenterColor = shadowColor;
									Color[] array2 = new Color[1]
									{
										Color.Transparent
									};
									Color[] array4 = pathGradientBrush.SurroundColors = array2;
									pathGradientBrush.CenterPoint = new PointF(pointF.X, pointF.Y);
									PointF focusScales = new PointF((float)(1.0 - 2.0 * (float)shadowSize / empty.Width), (float)(1.0 - 2.0 * (float)shadowSize / empty.Height));
									if (focusScales.X < 0.0)
									{
										focusScales.X = 0f;
									}
									if (focusScales.Y < 0.0)
									{
										focusScales.Y = 0f;
									}
									pathGradientBrush.FocusScales = focusScales;
									chartGraphics.FillPath(pathGradientBrush, graphicsPath2);
								}
							}
							GraphicsPath graphicsPath3 = new GraphicsPath();
							RectangleF rect2 = new RectangleF(empty.Location, empty.Size);
							rect2.Inflate((float)(rect2.Width / 4.0), (float)(rect2.Height / 4.0));
							graphicsPath3.AddEllipse(rect2);
							PathGradientBrush pathGradientBrush2 = new PathGradientBrush(graphicsPath3);
							pathGradientBrush2.CenterColor = ChartGraphics.GetGradientColor(markerColor, Color.White, 0.85);
							pathGradientBrush2.SurroundColors = new Color[1]
							{
								markerColor
							};
							Point3D[] array5 = new Point3D[1]
							{
								new Point3D(point.X, point.Y, positionZ + relativeSize.Width)
							};
							matrix.TransformPoints(array5);
							array5[0].PointF = chartGraphics.GetAbsolutePoint(array5[0].PointF);
							pathGradientBrush2.CenterPoint = array5[0].PointF;
							chartGraphics.FillEllipse(pathGradientBrush2, empty);
							chartGraphics.DrawEllipse(new Pen(markerBorderColor, (float)markerBorderSize), empty);
						}
						if ((operationType & DrawingOperationTypes.CalcElementPath) == DrawingOperationTypes.CalcElementPath)
						{
							graphicsPath.AddEllipse(empty);
						}
						break;
					case MarkerStyle.Square:
					{
						RectangleF empty2 = RectangleF.Empty;
						empty2.X = (float)(point.X - relativeSize.Width / 2.0);
						empty2.Y = (float)(point.Y - relativeSize.Height / 2.0);
						empty2.Width = relativeSize.Width;
						empty2.Height = relativeSize.Height;
						graphicsPath = this.Fill3DRectangle(empty2, (float)(positionZ - relativeSize.Width / 2.0), relativeSize.Width, matrix, lightStyle, markerColor, ChartHatchStyle.None, "", ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, markerBorderColor, markerBorderSize, ChartDashStyle.Solid, PenAlignment.Outset, operationType);
						break;
					}
					default:
						throw new InvalidOperationException(SR.ExceptionGraphics3DMarkerStyleUnknown);
					}
				}
				return graphicsPath;
			}
			if ((operationType & DrawingOperationTypes.DrawElement) == DrawingOperationTypes.DrawElement)
			{
				chartGraphics.DrawMarkerAbs(pointF, markerStyle, markerSize, markerColor, markerBorderColor, markerBorderSize, markerImage, markerImageTranspColor, shadowSize, shadowColor, imageScaleRect, false);
			}
			if ((operationType & DrawingOperationTypes.CalcElementPath) == DrawingOperationTypes.CalcElementPath)
			{
				RectangleF empty3 = RectangleF.Empty;
				empty3.X = (float)(pointF.X - (float)markerSize / 2.0);
				empty3.Y = (float)(pointF.Y - (float)markerSize / 2.0);
				empty3.Width = (float)markerSize;
				empty3.Height = (float)markerSize;
				graphicsPath.AddRectangle(empty3);
			}
			return graphicsPath;
		}

		internal SurfaceNames GetVisibleSurfaces(RectangleF position, float positionZ, float depth, Matrix3D matrix)
		{
			if (matrix.perspective != 0.0)
			{
				return this.GetVisibleSurfacesWithPerspective(position, positionZ, depth, matrix);
			}
			SurfaceNames surfaceNames = SurfaceNames.Front;
			if (matrix.angleY > 0.0)
			{
				surfaceNames |= SurfaceNames.Right;
			}
			else if (matrix.angleY < 0.0)
			{
				surfaceNames |= SurfaceNames.Left;
			}
			if (matrix.angleX > 0.0)
			{
				surfaceNames |= SurfaceNames.Top;
			}
			else if (matrix.angleX < 0.0)
			{
				surfaceNames |= SurfaceNames.Bottom;
			}
			return surfaceNames;
		}

		internal SurfaceNames GetVisibleSurfacesWithPerspective(RectangleF position, float positionZ, float depth, Matrix3D matrix)
		{
			Point3D[] array = new Point3D[8]
			{
				new Point3D(position.X, position.Y, positionZ + depth),
				new Point3D(position.X, position.Bottom, positionZ + depth),
				new Point3D(position.Right, position.Bottom, positionZ + depth),
				new Point3D(position.Right, position.Y, positionZ + depth),
				new Point3D(position.X, position.Y, positionZ),
				new Point3D(position.X, position.Bottom, positionZ),
				new Point3D(position.Right, position.Bottom, positionZ),
				new Point3D(position.Right, position.Y, positionZ)
			};
			matrix.TransformPoints(array);
			return this.GetVisibleSurfacesWithPerspective(array, matrix);
		}

		internal SurfaceNames GetVisibleSurfacesWithPerspective(Point3D[] cubePoints, Matrix3D matrix)
		{
			if (cubePoints.Length != 8)
			{
				throw new ArgumentException(SR.ExceptionGraphics3DCoordinatesInvalid, "cubePoints");
			}
			SurfaceNames surfaceNames = (SurfaceNames)0;
			if (ChartGraphics3D.IsSurfaceVisible(cubePoints[0], cubePoints[3], cubePoints[2]))
			{
				surfaceNames |= SurfaceNames.Front;
			}
			if (ChartGraphics3D.IsSurfaceVisible(cubePoints[4], cubePoints[5], cubePoints[6]))
			{
				surfaceNames |= SurfaceNames.Back;
			}
			if (ChartGraphics3D.IsSurfaceVisible(cubePoints[0], cubePoints[1], cubePoints[5]))
			{
				surfaceNames |= SurfaceNames.Left;
			}
			if (ChartGraphics3D.IsSurfaceVisible(cubePoints[3], cubePoints[7], cubePoints[6]))
			{
				surfaceNames |= SurfaceNames.Right;
			}
			if (ChartGraphics3D.IsSurfaceVisible(cubePoints[4], cubePoints[7], cubePoints[3]))
			{
				surfaceNames |= SurfaceNames.Top;
			}
			if (ChartGraphics3D.IsSurfaceVisible(cubePoints[1], cubePoints[2], cubePoints[6]))
			{
				surfaceNames |= SurfaceNames.Bottom;
			}
			return surfaceNames;
		}

		internal static bool IsSurfaceVisible(Point3D first, Point3D second, Point3D tree)
		{
			float num = (first.Y - second.Y) / (first.X - second.X);
			float num2 = first.Y - num * first.X;
			if (first.X == second.X)
			{
				if (first.Y > second.Y)
				{
					if (tree.X > first.X)
					{
						return true;
					}
					return false;
				}
				if (tree.X > first.X)
				{
					return false;
				}
				return true;
			}
			if (first.X < second.X)
			{
				if (tree.Y < num * tree.X + num2)
				{
					return false;
				}
				return true;
			}
			if (tree.Y <= num * tree.X + num2)
			{
				return true;
			}
			return false;
		}

		internal static PointF GetLinesIntersection(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
		{
			PointF empty = PointF.Empty;
			if (x1 == x2 && y3 == y4)
			{
				empty.X = x1;
				empty.Y = y3;
				return empty;
			}
			if (y1 == y2 && x3 == x4)
			{
				empty.X = x3;
				empty.Y = y1;
				return empty;
			}
			if (x1 == x2)
			{
				empty.X = x1;
				empty.Y = (empty.X - x3) * (y4 - y3);
				empty.Y /= x4 - x3;
				empty.Y += y3;
				return empty;
			}
			if (x3 == x4)
			{
				empty.X = x3;
				empty.Y = (empty.X - x1) * (y2 - y1);
				empty.Y /= x2 - x1;
				empty.Y += y1;
				return empty;
			}
			float num = (y1 - y2) / (x1 - x2);
			float num2 = y1 - num * x1;
			float num3 = (y3 - y4) / (x3 - x4);
			float num4 = y3 - num3 * x3;
			empty.X = (num4 - num2) / (num - num3);
			empty.Y = num * empty.X + num2;
			return empty;
		}

		internal GraphicsPath Fill3DRectangleAsCylinder(RectangleF position, float positionZ, float depth, Matrix3D matrix, LightStyle lightStyle, Color backColor, float topRightDarkening, float bottomLeftDarkening, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, PenAlignment penAlignment, bool veticalOrientation, DrawingOperationTypes operationType)
		{
			Point3D[] array = new Point3D[8];
			GraphicsPath graphicsPath = ((operationType & DrawingOperationTypes.CalcElementPath) == DrawingOperationTypes.CalcElementPath) ? new GraphicsPath() : null;
			if (veticalOrientation)
			{
				array[0] = new Point3D(position.X, position.Y, (float)(positionZ + depth / 2.0));
				array[1] = new Point3D(position.X, position.Bottom, (float)(positionZ + depth / 2.0));
				array[2] = new Point3D(position.Right, position.Bottom, (float)(positionZ + depth / 2.0));
				array[3] = new Point3D(position.Right, position.Y, (float)(positionZ + depth / 2.0));
				float x = (float)(position.X + position.Width / 2.0);
				array[4] = new Point3D(x, position.Y, positionZ + depth);
				array[5] = new Point3D(x, position.Bottom, positionZ + depth);
				array[6] = new Point3D(x, position.Bottom, positionZ);
				array[7] = new Point3D(x, position.Y, positionZ);
			}
			else
			{
				array[0] = new Point3D(position.Right, position.Y, (float)(positionZ + depth / 2.0));
				array[1] = new Point3D(position.X, position.Y, (float)(positionZ + depth / 2.0));
				array[2] = new Point3D(position.X, position.Bottom, (float)(positionZ + depth / 2.0));
				array[3] = new Point3D(position.Right, position.Bottom, (float)(positionZ + depth / 2.0));
				float y = (float)(position.Y + position.Height / 2.0);
				array[4] = new Point3D(position.Right, y, positionZ + depth);
				array[5] = new Point3D(position.X, y, positionZ + depth);
				array[6] = new Point3D(position.X, y, positionZ);
				array[7] = new Point3D(position.Right, y, positionZ);
			}
			matrix.TransformPoints(array);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].PointF = ((ChartGraphics)this).GetAbsolutePoint(array[i].PointF);
			}
			if (lightStyle == LightStyle.None && (borderWidth == 0 || borderStyle == ChartDashStyle.NotSet || borderColor == Color.Empty))
			{
				borderColor = ChartGraphics.GetGradientColor(backColor, Color.Black, 0.5);
			}
			Color color = default(Color);
			Color color2 = default(Color);
			Color gradientColor = default(Color);
			Color gradientColor2 = default(Color);
			Color gradientColor3 = default(Color);
			Color gradientColor4 = default(Color);
			matrix.GetLight(backColor, out color, out color2, out gradientColor, out gradientColor2, out gradientColor3, out gradientColor4);
			if (topRightDarkening != 0.0)
			{
				if (veticalOrientation)
				{
					gradientColor3 = ChartGraphics.GetGradientColor(gradientColor3, Color.Black, (double)topRightDarkening);
				}
				else
				{
					gradientColor2 = ChartGraphics.GetGradientColor(gradientColor2, Color.Black, (double)topRightDarkening);
				}
			}
			if (bottomLeftDarkening != 0.0)
			{
				if (veticalOrientation)
				{
					gradientColor4 = ChartGraphics.GetGradientColor(gradientColor4, Color.Black, (double)bottomLeftDarkening);
				}
				else
				{
					gradientColor = ChartGraphics.GetGradientColor(gradientColor, Color.Black, (double)bottomLeftDarkening);
				}
			}
			SurfaceNames surfaceNames = this.GetVisibleSurfacesWithPerspective(position, positionZ, depth, matrix);
			if ((surfaceNames & SurfaceNames.Front) != SurfaceNames.Front)
			{
				surfaceNames |= SurfaceNames.Front;
			}
			PointF[] array2 = new PointF[4]
			{
				array[6].PointF,
				array[1].PointF,
				array[5].PointF,
				array[2].PointF
			};
			GraphicsPath graphicsPath2 = new GraphicsPath();
			graphicsPath2.AddClosedCurve(array2, 0.8f);
			graphicsPath2.Flatten();
			array2[0] = array[7].PointF;
			array2[1] = array[0].PointF;
			array2[2] = array[4].PointF;
			array2[3] = array[3].PointF;
			GraphicsPath graphicsPath3 = new GraphicsPath();
			graphicsPath3.AddClosedCurve(array2, 0.8f);
			graphicsPath3.Flatten();
			float num = 90f;
			if (array[5].PointF.Y != array[4].PointF.Y)
			{
				num = (float)Math.Atan((double)((array[4].PointF.X - array[5].PointF.X) / (array[5].PointF.Y - array[4].PointF.Y)));
				num = (float)Math.Round(num * 180.0 / 3.1415927410125732);
			}
			for (int j = 0; j <= 1; j++)
			{
				if (j != 0 || backColor.A != 255)
				{
					for (int num2 = 1; num2 <= 32; num2 *= 2)
					{
						SurfaceNames surfaceNames2 = (SurfaceNames)num2;
						bool flag = (surfaceNames & surfaceNames2) != (SurfaceNames)0;
						if (flag && j == 1)
						{
							goto IL_0497;
						}
						if (!flag && j == 0)
						{
							goto IL_0497;
						}
						continue;
						IL_0497:
						GraphicsPath graphicsPath4 = null;
						Color color3 = backColor;
						Brush brush = null;
						switch (surfaceNames2)
						{
						case SurfaceNames.Front:
						{
							color3 = backColor;
							graphicsPath4 = new GraphicsPath();
							PointF empty = PointF.Empty;
							PointF empty2 = PointF.Empty;
							this.AddEllipseSegment(graphicsPath4, graphicsPath3, graphicsPath2, matrix.perspective == 0.0 && veticalOrientation, num, out empty, out empty2);
							graphicsPath4.Reverse();
							PointF empty3 = PointF.Empty;
							PointF empty4 = PointF.Empty;
							this.AddEllipseSegment(graphicsPath4, graphicsPath2, graphicsPath3, matrix.perspective == 0.0 && veticalOrientation, num, out empty3, out empty4);
							graphicsPath4.CloseAllFigures();
							this.oppLeftBottomPoint = -1;
							this.oppRigthTopPoint = -1;
							if (lightStyle != 0)
							{
								RectangleF bounds = graphicsPath4.GetBounds();
								if (bounds.Height > 0.0 && bounds.Width > 0.0)
								{
									Color gradientColor5 = ChartGraphics.GetGradientColor(backColor, Color.White, 0.3);
									Color gradientColor6 = ChartGraphics.GetGradientColor(backColor, Color.Black, 0.3);
									if (!empty.IsEmpty && !empty2.IsEmpty && !empty3.IsEmpty && !empty4.IsEmpty)
									{
										PointF empty5 = PointF.Empty;
										empty5.X = (float)(bounds.X + bounds.Width / 2.0);
										empty5.Y = (float)(bounds.Y + bounds.Height / 2.0);
										PointF empty6 = PointF.Empty;
										double a = (double)num * 3.1415926535897931 / 180.0;
										if (num == 0.0 || num == 180.0 || num == -180.0)
										{
											empty6.X = (float)(empty5.X + 100.0);
											empty6.Y = empty5.Y;
										}
										else if (num == 90.0 || num == -90.0)
										{
											empty6.X = empty5.X;
											empty6.Y = (float)(empty5.Y + 100.0);
										}
										else if (num > -45.0 && num < 45.0)
										{
											empty6.X = (float)(empty5.X + 100.0);
											empty6.Y = (float)(Math.Tan(a) * (double)empty6.X);
											empty6.Y += (float)((double)empty5.Y - Math.Tan(a) * (double)empty5.X);
										}
										else
										{
											empty6.Y = (float)(empty5.Y + 100.0);
											empty6.X = (float)((double)empty6.Y - ((double)empty5.Y - Math.Tan(a) * (double)empty5.X));
											empty6.X /= (float)Math.Tan(a);
										}
										PointF linesIntersection = ChartGraphics3D.GetLinesIntersection(empty5.X, empty5.Y, empty6.X, empty6.Y, empty.X, empty.Y, empty3.X, empty3.Y);
										PointF linesIntersection2 = ChartGraphics3D.GetLinesIntersection(empty5.X, empty5.Y, empty6.X, empty6.Y, empty2.X, empty2.Y, empty4.X, empty4.Y);
										if (linesIntersection.X == linesIntersection2.X && linesIntersection.Y == linesIntersection2.Y)
										{
											break;
										}
										brush = new LinearGradientBrush(linesIntersection, linesIntersection2, gradientColor5, gradientColor6);
										ColorBlend colorBlend = new ColorBlend(5);
										colorBlend.Colors[0] = gradientColor6;
										colorBlend.Colors[1] = gradientColor6;
										colorBlend.Colors[2] = gradientColor5;
										colorBlend.Colors[3] = gradientColor6;
										colorBlend.Colors[4] = gradientColor6;
										colorBlend.Positions[0] = 0f;
										colorBlend.Positions[1] = 0f;
										colorBlend.Positions[2] = 0.5f;
										colorBlend.Positions[3] = 1f;
										colorBlend.Positions[4] = 1f;
										((LinearGradientBrush)brush).InterpolationColors = colorBlend;
									}
								}
							}
							break;
						}
						case SurfaceNames.Top:
							if (veticalOrientation)
							{
								color3 = gradientColor3;
								graphicsPath4 = graphicsPath3;
							}
							break;
						case SurfaceNames.Bottom:
							if (veticalOrientation)
							{
								color3 = gradientColor4;
								graphicsPath4 = graphicsPath2;
							}
							break;
						case SurfaceNames.Right:
							if (!veticalOrientation)
							{
								color3 = gradientColor2;
								graphicsPath4 = graphicsPath3;
							}
							break;
						case SurfaceNames.Left:
							if (!veticalOrientation)
							{
								color3 = gradientColor;
								graphicsPath4 = graphicsPath2;
							}
							break;
						}
						if (graphicsPath4 != null)
						{
							if ((operationType & DrawingOperationTypes.DrawElement) == DrawingOperationTypes.DrawElement)
							{
								if ((surfaceNames & surfaceNames2) != 0)
								{
									((ChartGraphics)this).FillPath((brush == null) ? new SolidBrush(color3) : brush, graphicsPath4);
								}
								Pen pen = new Pen(borderColor, (float)borderWidth);
								pen.DashStyle = ((ChartGraphics)this).GetPenStyle(borderStyle);
								if (lightStyle != 0 && (borderWidth == 0 || borderStyle == ChartDashStyle.NotSet || borderColor == Color.Empty))
								{
									pen = new Pen((brush == null) ? color3 : ChartGraphics.GetGradientColor(backColor, Color.Black, 0.3), 1f);
									pen.Alignment = PenAlignment.Inset;
								}
								pen.StartCap = LineCap.Round;
								pen.EndCap = LineCap.Round;
								pen.LineJoin = LineJoin.Bevel;
								((ChartGraphics)this).DrawPath(pen, graphicsPath4);
							}
							if ((operationType & DrawingOperationTypes.CalcElementPath) == DrawingOperationTypes.CalcElementPath && (surfaceNames & surfaceNames2) != 0 && graphicsPath4 != null && graphicsPath4.PointCount > 0)
							{
								graphicsPath.AddPath(graphicsPath4, true);
								graphicsPath.SetMarkers();
							}
						}
					}
				}
			}
			return graphicsPath;
		}

		internal void AddEllipseSegment(GraphicsPath resultPath, GraphicsPath ellipseFlattenPath, GraphicsPath oppositeEllipseFlattenPath, bool veticalOrientation, float cylinderAngle, out PointF leftSideLinePoint, out PointF rightSideLinePoint)
		{
			leftSideLinePoint = PointF.Empty;
			rightSideLinePoint = PointF.Empty;
			if (ellipseFlattenPath.PointCount != 0)
			{
				int num = 0;
				int num2 = 0;
				PointF[] pathPoints = ellipseFlattenPath.PathPoints;
				if (veticalOrientation)
				{
					for (int i = 1; i < pathPoints.Length; i++)
					{
						if (pathPoints[num].X > pathPoints[i].X)
						{
							num = i;
						}
						if (pathPoints[num2].X < pathPoints[i].X)
						{
							num2 = i;
						}
					}
				}
				else
				{
					bool flag = false;
					num = -1;
					num2 = -1;
					if (this.oppLeftBottomPoint != -1 && this.oppRigthTopPoint != -1)
					{
						num = this.oppLeftBottomPoint;
						num2 = this.oppRigthTopPoint;
					}
					else
					{
						PointF[] pathPoints2 = oppositeEllipseFlattenPath.PathPoints;
						int num3 = 0;
						while (!flag && num3 < pathPoints.Length)
						{
							for (int j = 0; !flag && j < pathPoints2.Length; j++)
							{
								bool flag2 = false;
								bool flag3 = false;
								bool flag4 = false;
								if (cylinderAngle > -30.0 && cylinderAngle < 30.0)
								{
									flag2 = true;
								}
								if (flag2)
								{
									if (pathPoints2[j].Y != pathPoints[num3].Y)
									{
										float num4 = pathPoints2[j].X - pathPoints[num3].X;
										num4 /= pathPoints2[j].Y - pathPoints[num3].Y;
										for (int k = 0; k < pathPoints.Length; k++)
										{
											if (k != num3)
											{
												float num5 = num4;
												num5 *= pathPoints[k].Y - pathPoints[num3].Y;
												num5 += pathPoints[num3].X;
												if (num5 > pathPoints[k].X)
												{
													flag3 = true;
												}
												if (num5 < pathPoints[k].X)
												{
													flag4 = true;
												}
												if (flag3 && flag4)
												{
													break;
												}
											}
										}
										if (!flag3 || !flag4)
										{
											for (int l = 0; l < pathPoints2.Length; l++)
											{
												if (l != j)
												{
													float num6 = num4;
													num6 *= pathPoints2[l].Y - pathPoints[num3].Y;
													num6 += pathPoints[num3].X;
													if (num6 > pathPoints2[l].X)
													{
														flag3 = true;
													}
													if (num6 < pathPoints2[l].X)
													{
														flag4 = true;
													}
													if (flag3 && flag4)
													{
														break;
													}
												}
											}
										}
										goto IL_03c0;
									}
								}
								else if (pathPoints2[j].X != pathPoints[num3].X)
								{
									float num7 = pathPoints2[j].Y - pathPoints[num3].Y;
									num7 /= pathPoints2[j].X - pathPoints[num3].X;
									for (int m = 0; m < pathPoints.Length; m++)
									{
										if (m != num3)
										{
											float num8 = num7;
											num8 *= pathPoints[m].X - pathPoints[num3].X;
											num8 += pathPoints[num3].Y;
											if (num8 > pathPoints[m].Y)
											{
												flag3 = true;
											}
											if (num8 < pathPoints[m].Y)
											{
												flag4 = true;
											}
											if (flag3 && flag4)
											{
												break;
											}
										}
									}
									if (!flag3 || !flag4)
									{
										for (int n = 0; n < pathPoints2.Length; n++)
										{
											if (n != j)
											{
												float num9 = num7;
												num9 *= pathPoints2[n].X - pathPoints[num3].X;
												num9 += pathPoints[num3].Y;
												if (num9 > pathPoints2[n].Y)
												{
													flag3 = true;
												}
												if (num9 < pathPoints2[n].Y)
												{
													flag4 = true;
												}
												if (flag3 && flag4)
												{
													break;
												}
											}
										}
									}
									goto IL_03c0;
								}
								continue;
								IL_03c0:
								if (!flag3 && num == -1)
								{
									num = num3;
									this.oppLeftBottomPoint = j;
								}
								if (!flag4 && num2 == -1)
								{
									num2 = num3;
									this.oppRigthTopPoint = j;
								}
								if (num >= 0 && num2 >= 0)
								{
									flag = true;
									if (flag2 && pathPoints[num].Y > pathPoints2[this.oppLeftBottomPoint].Y)
									{
										int num10 = num;
										num = num2;
										num2 = num10;
										num10 = this.oppLeftBottomPoint;
										this.oppLeftBottomPoint = this.oppRigthTopPoint;
										this.oppRigthTopPoint = num10;
									}
								}
							}
							num3++;
						}
					}
				}
				if (num != num2 && num2 != -1 && num != -1)
				{
					leftSideLinePoint = pathPoints[num];
					rightSideLinePoint = pathPoints[num2];
					for (int num11 = num + 1; num11 != num2 + 1; num11++)
					{
						if (num11 > pathPoints.Length - 1)
						{
							resultPath.AddLine(pathPoints[pathPoints.Length - 1], pathPoints[0]);
							num11 = 0;
						}
						else
						{
							resultPath.AddLine(pathPoints[num11 - 1], pathPoints[num11]);
						}
					}
				}
			}
		}
	}
}
