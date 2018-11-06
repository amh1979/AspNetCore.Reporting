using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class RangeChart : SplineChart
	{
		protected bool gradientFill;

		protected GraphicsPath areaBottomPath = new GraphicsPath();

		protected GraphicsPath areaPath;

		protected Series series;

		protected PointF[] lowPoints;

		protected bool indexedBasedX;

		private float thirdPointY2Value = float.NaN;

		private float fourthPointY2Value = float.NaN;

		public override string Name
		{
			get
			{
				return "Range";
			}
		}

		public override int YValuesPerPoint
		{
			get
			{
				return 2;
			}
		}

		public override bool ExtraYValuesConnectedToYAxis
		{
			get
			{
				return true;
			}
		}

		public RangeChart()
		{
			base.drawOutsideLines = true;
		}

		public override LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Rectangle;
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}

		protected override float GetDefaultTension()
		{
			return 0f;
		}

		protected override bool IsLineTensionSupported()
		{
			return false;
		}

		private void FillLastSeriesGradient(ChartGraphics graph)
		{
			if (this.areaPath != null)
			{
				this.areaPath.AddLine(this.areaPath.GetLastPoint().X, this.areaPath.GetLastPoint().Y, this.areaPath.GetLastPoint().X, this.areaBottomPath.GetLastPoint().Y);
			}
			if (this.gradientFill && this.areaPath != null)
			{
				graph.SetClip(base.area.PlotAreaPosition.ToRectangleF());
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddPath(this.areaPath, true);
				this.areaBottomPath.Reverse();
				graphicsPath.AddPath(this.areaBottomPath, true);
				Brush gradientBrush = graph.GetGradientBrush(graphicsPath.GetBounds(), this.series.Color, this.series.BackGradientEndColor, this.series.BackGradientType);
				graph.FillPath(gradientBrush, graphicsPath);
				this.gradientFill = false;
				graph.ResetClip();
			}
			if (this.areaPath != null)
			{
				this.areaPath.Dispose();
				this.areaPath = null;
			}
			this.areaBottomPath.Reset();
		}

		protected override void ProcessChartType(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			this.gradientFill = false;
			this.lowPoints = null;
			this.indexedBasedX = area.IndexedSeries((string[])area.GetSeriesFromChartType(this.Name).ToArray(typeof(string)));
			base.ProcessChartType(selection, graph, common, area, seriesToDraw);
			this.FillLastSeriesGradient(graph);
		}

		protected override void DrawLine(ChartGraphics graph, CommonElements common, DataPoint point, Series series, PointF[] points, int pointIndex, float tension)
		{
			if (point.YValues.Length < 2)
			{
				throw new InvalidOperationException(SR.ExceptionChartTypeRequiresYValues(this.Name, "2"));
			}
			PointF pointF;
			PointF pointF2;
			PointF pointF3;
			PointF pointF4;
			GraphicsPath graphicsPath;
			if (pointIndex > 0 && base.yValueIndex != 1)
			{
				if (this.series != null)
				{
					if (this.series.Name != series.Name)
					{
						this.FillLastSeriesGradient(graph);
						this.series = series;
						this.lowPoints = null;
						this.areaBottomPath.Reset();
					}
				}
				else
				{
					this.series = series;
				}
				if (this.lowPoints == null)
				{
					base.yValueIndex = 1;
					this.lowPoints = this.GetPointsPosition(graph, series, this.indexedBasedX);
					base.yValueIndex = 0;
				}
				pointF = points[pointIndex - 1];
				pointF2 = points[pointIndex];
				pointF3 = this.lowPoints[pointIndex - 1];
				pointF4 = this.lowPoints[pointIndex];
				Brush brush = null;
				if (point.BackHatchStyle != 0)
				{
					brush = graph.GetHatchBrush(point.BackHatchStyle, point.Color, point.BackGradientEndColor);
				}
				else if (point.BackGradientType != 0)
				{
					this.gradientFill = true;
					this.series = point.series;
				}
				else
				{
					brush = ((point.BackImage.Length <= 0 || point.BackImageMode == ChartImageWrapMode.Unscaled || point.BackImageMode == ChartImageWrapMode.Scaled) ? new SolidBrush(point.Color) : graph.GetTextureBrush(point.BackImage, point.BackImageTransparentColor, point.BackImageMode, point.Color));
				}
				graphicsPath = new GraphicsPath();
				graphicsPath.AddLine(pointF.X, pointF3.Y, pointF.X, pointF.Y);
				if (base.lineTension == 0.0)
				{
					graphicsPath.AddLine(points[pointIndex - 1], points[pointIndex]);
				}
				else
				{
					graphicsPath.AddCurve(points, pointIndex - 1, 1, base.lineTension);
				}
				graphicsPath.AddLine(pointF2.X, pointF2.Y, pointF2.X, pointF4.Y);
				if (graph.ActiveRenderingType == RenderingType.Svg)
				{
					GraphicsPath graphicsPath2 = new GraphicsPath();
					if (base.lineTension == 0.0)
					{
						graphicsPath.AddLine(this.lowPoints[pointIndex - 1], this.lowPoints[pointIndex]);
					}
					else
					{
						graphicsPath2.AddCurve(this.lowPoints, pointIndex - 1, 1, base.lineTension);
						graphicsPath2.Flatten();
						PointF[] pathPoints = graphicsPath2.PathPoints;
						PointF[] array = new PointF[pathPoints.Length];
						int num = pathPoints.Length - 1;
						PointF[] array2 = pathPoints;
						foreach (PointF pointF5 in array2)
						{
							array[num] = pointF5;
							num--;
						}
						if (array.Length == 2)
						{
							array = new PointF[3]
							{
								array[0],
								array[1],
								array[1]
							};
						}
						graphicsPath.AddPolygon(array);
					}
				}
				else if (base.lineTension == 0.0)
				{
					graphicsPath.AddLine(this.lowPoints[pointIndex - 1], this.lowPoints[pointIndex]);
				}
				else
				{
					graphicsPath.AddCurve(this.lowPoints, pointIndex - 1, 1, base.lineTension);
				}
				if (!base.clipRegionSet)
				{
					double num2 = base.indexedSeries ? ((double)(pointIndex + 1)) : series.Points[pointIndex].XValue;
					double num3 = base.indexedSeries ? ((double)pointIndex) : series.Points[pointIndex - 1].XValue;
					if (num3 < base.hAxisMin || num3 > base.hAxisMax || num2 > base.hAxisMax || num2 < base.hAxisMin || series.Points[pointIndex - 1].YValues[1] < base.vAxisMin || series.Points[pointIndex - 1].YValues[1] > base.vAxisMax || series.Points[pointIndex].YValues[1] < base.vAxisMin || series.Points[pointIndex].YValues[1] > base.vAxisMax)
					{
						graph.SetClip(base.area.PlotAreaPosition.ToRectangleF());
						base.clipRegionSet = true;
					}
				}
				if (series.ShadowColor != Color.Empty && series.ShadowOffset != 0 && point.Color != Color.Empty && point.Color != Color.Transparent)
				{
					Matrix matrix = graph.Transform.Clone();
					matrix.Translate((float)series.ShadowOffset, (float)series.ShadowOffset);
					Matrix transform = graph.Transform;
					graph.Transform = matrix;
					Region region = new Region(graphicsPath);
					Brush brush2 = new SolidBrush((series.ShadowColor.A != 255) ? series.ShadowColor : Color.FromArgb((int)point.Color.A / 2, series.ShadowColor));
					Region region2 = null;
					if (!graph.IsClipEmpty && !graph.Clip.IsInfinite(graph.Graphics))
					{
						region2 = graph.Clip;
						region2.Translate(series.ShadowOffset + 1, series.ShadowOffset + 1);
						graph.Clip = region2;
					}
					graph.FillRegion(brush2, region);
					Pen pen = new Pen(brush2, 1f);
					if (pointIndex == 0)
					{
						graph.DrawLine(pen, pointF.X, pointF3.Y, pointF.X, pointF.Y);
					}
					if (pointIndex == series.Points.Count - 1)
					{
						graph.DrawLine(pen, pointF2.X, pointF2.Y, pointF2.X, pointF4.Y);
					}
					graph.Transform = transform;
					graph.shadowDrawingMode = true;
					base.drawShadowOnly = true;
					base.DrawLine(graph, common, point, series, points, pointIndex, tension);
					base.yValueIndex = 1;
					base.DrawLine(graph, common, point, series, this.lowPoints, pointIndex, tension);
					base.yValueIndex = 0;
					base.drawShadowOnly = false;
					graph.shadowDrawingMode = false;
					if (region2 != null)
					{
						region2 = graph.Clip;
						region2.Translate(-(series.ShadowOffset + 1), -(series.ShadowOffset + 1));
						graph.Clip = region2;
					}
				}
				if (!this.gradientFill)
				{
					SmoothingMode smoothingMode = graph.SmoothingMode;
					graph.SmoothingMode = SmoothingMode.None;
					graphicsPath.CloseAllFigures();
					graph.FillPath(brush, graphicsPath);
					graph.SmoothingMode = smoothingMode;
					if (graph.SmoothingMode != SmoothingMode.None)
					{
						Pen pen2 = new Pen(brush, 1f);
						if (brush is HatchBrush)
						{
							pen2.Color = ((HatchBrush)brush).ForegroundColor;
						}
						if (pointIndex == 0)
						{
							graph.DrawLine(pen2, pointF.X, pointF3.Y, pointF.X, pointF.Y);
						}
						if (pointIndex == series.Points.Count - 1)
						{
							graph.DrawLine(pen2, pointF2.X, pointF2.Y, pointF2.X, pointF4.Y);
						}
						if (base.lineTension == 0.0)
						{
							graph.DrawLine(pen2, points[pointIndex - 1], points[pointIndex]);
						}
						else
						{
							graph.DrawCurve(pen2, points, pointIndex - 1, 1, base.lineTension);
						}
						if (base.lineTension == 0.0)
						{
							graph.DrawLine(pen2, this.lowPoints[pointIndex - 1], this.lowPoints[pointIndex]);
						}
						else
						{
							graph.DrawCurve(pen2, this.lowPoints, pointIndex - 1, 1, base.lineTension);
						}
					}
				}
				if (this.areaPath == null)
				{
					this.areaPath = new GraphicsPath();
					this.areaPath.AddLine(pointF.X, pointF3.Y, pointF.X, pointF.Y);
				}
				if (base.lineTension == 0.0)
				{
					this.areaPath.AddLine(points[pointIndex - 1], points[pointIndex]);
				}
				else
				{
					this.areaPath.AddCurve(points, pointIndex - 1, 1, base.lineTension);
				}
				if (base.lineTension == 0.0)
				{
					this.areaBottomPath.AddLine(this.lowPoints[pointIndex - 1], this.lowPoints[pointIndex]);
				}
				else
				{
					this.areaBottomPath.AddCurve(this.lowPoints, pointIndex - 1, 1, base.lineTension);
				}
				if (point.BorderWidth > 0 && point.BorderStyle != 0 && point.BorderColor != Color.Empty)
				{
					goto IL_0946;
				}
				if (brush is SolidBrush)
				{
					goto IL_0946;
				}
				goto IL_0996;
			}
			return;
			IL_0996:
			if (common.ProcessModeRegions)
			{
				graphicsPath.AddLine(pointF.X, pointF3.Y, pointF.X, pointF.Y);
				if (base.lineTension == 0.0)
				{
					graphicsPath.AddLine(points[pointIndex - 1], points[pointIndex]);
				}
				else
				{
					graphicsPath.AddCurve(points, pointIndex - 1, 1, base.lineTension);
				}
				graphicsPath.AddLine(pointF2.X, pointF2.Y, pointF2.X, pointF4.Y);
				if (base.lineTension == 0.0)
				{
					graphicsPath.AddLine(this.lowPoints[pointIndex - 1], this.lowPoints[pointIndex]);
				}
				else
				{
					graphicsPath.AddCurve(this.lowPoints, pointIndex - 1, 1, base.lineTension);
				}
				GraphicsPath graphicsPath3 = new GraphicsPath();
				graphicsPath3.AddLine(pointF.X, pointF3.Y, pointF.X, pointF.Y);
				if (base.lineTension == 0.0)
				{
					graphicsPath3.AddLine(points[pointIndex - 1], points[pointIndex]);
				}
				else
				{
					graphicsPath3.AddCurve(points, pointIndex - 1, 1, base.lineTension);
					graphicsPath3.Flatten();
				}
				graphicsPath3.AddLine(pointF2.X, pointF2.Y, pointF2.X, pointF4.Y);
				if (base.lineTension == 0.0)
				{
					graphicsPath3.AddLine(this.lowPoints[pointIndex - 1], this.lowPoints[pointIndex]);
				}
				else
				{
					graphicsPath3.AddCurve(this.lowPoints, pointIndex - 1, 1, base.lineTension);
					graphicsPath3.Flatten();
				}
				PointF pointF6 = PointF.Empty;
				float[] array3 = new float[graphicsPath3.PointCount * 2];
				PointF[] pathPoints2 = graphicsPath3.PathPoints;
				for (int j = 0; j < graphicsPath3.PointCount; j++)
				{
					pointF6 = graph.GetRelativePoint(pathPoints2[j]);
					array3[2 * j] = pointF6.X;
					array3[2 * j + 1] = pointF6.Y;
				}
				common.HotRegionsList.AddHotRegion(graph, graphicsPath3, false, array3, point, series.Name, pointIndex);
			}
			return;
			IL_0946:
			base.useBorderColor = true;
			base.disableShadow = true;
			base.DrawLine(graph, common, point, series, points, pointIndex, tension);
			base.yValueIndex = 1;
			base.DrawLine(graph, common, point, series, this.lowPoints, pointIndex, tension);
			base.yValueIndex = 0;
			base.useBorderColor = false;
			base.disableShadow = false;
			goto IL_0996;
		}

		protected override GraphicsPath Draw3DSurface(ChartArea area, ChartGraphics graph, Matrix3D matrix, LightStyle lightStyle, DataPoint3D prevDataPointEx, float positionZ, float depth, ArrayList points, int pointIndex, int pointLoopIndex, float tension, DrawingOperationTypes operationType, float topDarkening, float bottomDarkening, PointF thirdPointPosition, PointF fourthPointPosition, bool clippedSegment)
		{
			GraphicsPath result = ((operationType & DrawingOperationTypes.CalcElementPath) == DrawingOperationTypes.CalcElementPath) ? new GraphicsPath() : null;
			if (base.centerPointIndex == 2147483647)
			{
				base.centerPointIndex = base.GetCenterPointIndex(points);
			}
			DataPoint3D dataPoint3D = (DataPoint3D)points[pointIndex];
			int num = pointIndex;
			DataPoint3D dataPoint3D2 = ChartGraphics3D.FindPointByIndex(points, dataPoint3D.index - 1, base.multiSeries ? dataPoint3D : null, ref num);
			bool reversed = false;
			if (dataPoint3D2.index > dataPoint3D.index)
			{
				DataPoint3D dataPoint3D3 = dataPoint3D2;
				dataPoint3D2 = dataPoint3D;
				dataPoint3D = dataPoint3D3;
				reversed = true;
			}
			if (matrix.perspective != 0.0 && base.centerPointIndex != 2147483647)
			{
				num = pointIndex;
				if (pointIndex != base.centerPointIndex + 1)
				{
					dataPoint3D2 = ChartGraphics3D.FindPointByIndex(points, dataPoint3D.index - 1, base.multiSeries ? dataPoint3D : null, ref num);
				}
				else if (!area.reverseSeriesOrder)
				{
					dataPoint3D = ChartGraphics3D.FindPointByIndex(points, dataPoint3D2.index + 1, base.multiSeries ? dataPoint3D : null, ref num);
				}
				else
				{
					dataPoint3D2 = dataPoint3D;
					dataPoint3D = ChartGraphics3D.FindPointByIndex(points, dataPoint3D.index - 1, base.multiSeries ? dataPoint3D : null, ref num);
				}
			}
			if (dataPoint3D2 != null && dataPoint3D != null)
			{
				DataPoint3D dataPoint3D4 = dataPoint3D;
				if (prevDataPointEx.dataPoint.Empty)
				{
					dataPoint3D4 = prevDataPointEx;
				}
				else if (dataPoint3D2.index > dataPoint3D.index)
				{
					dataPoint3D4 = dataPoint3D2;
				}
				if (!base.useBorderColor)
				{
					Color color = dataPoint3D4.dataPoint.Color;
				}
				else
				{
					Color borderColor = dataPoint3D4.dataPoint.BorderColor;
				}
				ChartDashStyle borderStyle = dataPoint3D4.dataPoint.BorderStyle;
				if (dataPoint3D4.dataPoint.Empty && dataPoint3D4.dataPoint.Color == Color.Empty)
				{
					Color gray = Color.Gray;
				}
				if (dataPoint3D4.dataPoint.Empty)
				{
					ChartDashStyle borderStyle2 = dataPoint3D4.dataPoint.BorderStyle;
				}
				return this.Draw3DSurface(dataPoint3D2, dataPoint3D, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, LineSegmentType.Single, topDarkening, bottomDarkening, thirdPointPosition, fourthPointPosition, clippedSegment, true, true);
			}
			return result;
		}

		protected override GraphicsPath Draw3DSurface(DataPoint3D firstPoint, DataPoint3D secondPoint, bool reversed, ChartArea area, ChartGraphics graph, Matrix3D matrix, LightStyle lightStyle, DataPoint3D prevDataPointEx, float positionZ, float depth, ArrayList points, int pointIndex, int pointLoopIndex, float tension, DrawingOperationTypes operationType, LineSegmentType surfaceSegmentType, float topDarkening, float bottomDarkening, PointF thirdPointPosition, PointF fourthPointPosition, bool clippedSegment, bool clipOnTop, bool clipOnBottom)
		{
			GraphicsPath graphicsPath = ((operationType & DrawingOperationTypes.CalcElementPath) == DrawingOperationTypes.CalcElementPath) ? new GraphicsPath() : null;
			DataPoint3D dataPoint3D = secondPoint;
			if (prevDataPointEx.dataPoint.Empty)
			{
				dataPoint3D = prevDataPointEx;
			}
			else if (firstPoint.index > secondPoint.index)
			{
				dataPoint3D = firstPoint;
			}
			Color color = base.useBorderColor ? dataPoint3D.dataPoint.BorderColor : dataPoint3D.dataPoint.Color;
			ChartDashStyle borderStyle = dataPoint3D.dataPoint.BorderStyle;
			if (dataPoint3D.dataPoint.Empty && dataPoint3D.dataPoint.Color == Color.Empty)
			{
				color = Color.Gray;
			}
			if (dataPoint3D.dataPoint.Empty && dataPoint3D.dataPoint.BorderStyle == ChartDashStyle.NotSet)
			{
				borderStyle = ChartDashStyle.Solid;
			}
			float num = (float)base.vAxis.GetPosition(base.vAxis.Crossing);
			PointF pointF = default(PointF);
			PointF pointF2 = default(PointF);
			this.GetBottomPointsPosition(base.common, area, num, ref firstPoint, ref secondPoint, out pointF, out pointF2);
			if (!float.IsNaN(thirdPointPosition.Y))
			{
				pointF.Y = thirdPointPosition.Y;
			}
			if (!float.IsNaN(fourthPointPosition.Y))
			{
				pointF2.Y = fourthPointPosition.Y;
			}
			if (firstPoint.yPosition > (double)pointF.Y && secondPoint.yPosition < (double)pointF2.Y)
			{
				goto IL_0157;
			}
			if (firstPoint.yPosition < (double)pointF.Y && secondPoint.yPosition > (double)pointF2.Y)
			{
				goto IL_0157;
			}
			goto IL_040a;
			IL_0157:
			if (tension != 0.0)
			{
				throw new InvalidOperationException(SR.Exception3DSplineY1ValueIsLessThenY2);
			}
			PointF linesIntersection = ChartGraphics3D.GetLinesIntersection((float)firstPoint.xPosition, (float)firstPoint.yPosition, (float)secondPoint.xPosition, (float)secondPoint.yPosition, pointF.X, pointF.Y, pointF2.X, pointF2.Y);
			DataPoint3D dataPoint3D2 = new DataPoint3D();
			dataPoint3D2.xPosition = (double)linesIntersection.X;
			dataPoint3D2.yPosition = (double)linesIntersection.Y;
			bool flag = true;
			if (double.IsNaN((double)linesIntersection.X) || double.IsNaN((double)linesIntersection.Y))
			{
				flag = false;
			}
			else
			{
				if ((decimal)linesIntersection.X == (decimal)firstPoint.xPosition && (decimal)linesIntersection.Y == (decimal)firstPoint.yPosition)
				{
					flag = false;
				}
				if ((decimal)linesIntersection.X == (decimal)secondPoint.xPosition && (decimal)linesIntersection.Y == (decimal)secondPoint.yPosition)
				{
					flag = false;
				}
			}
			if (flag)
			{
				reversed = false;
				if (pointIndex + 1 < points.Count)
				{
					DataPoint3D dataPoint3D3 = (DataPoint3D)points[pointIndex + 1];
					if (dataPoint3D3.index == firstPoint.index)
					{
						reversed = true;
					}
				}
				for (int num2 = 0; num2 <= 1; num2++)
				{
					GraphicsPath graphicsPath2 = null;
					if (num2 == 0 && !reversed)
					{
						goto IL_02d3;
					}
					if (num2 == 1 && reversed)
					{
						goto IL_02d3;
					}
					goto IL_0347;
					IL_02d3:
					this.fourthPointY2Value = (float)dataPoint3D2.yPosition;
					dataPoint3D2.dataPoint = secondPoint.dataPoint;
					dataPoint3D2.index = secondPoint.index;
					graphicsPath2 = this.Draw3DSurface(firstPoint, dataPoint3D2, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, surfaceSegmentType, topDarkening, bottomDarkening, new PointF(float.NaN, float.NaN), new PointF(float.NaN, float.NaN), clippedSegment, true, true);
					goto IL_0347;
					IL_0356:
					this.thirdPointY2Value = (float)dataPoint3D2.yPosition;
					dataPoint3D2.dataPoint = firstPoint.dataPoint;
					dataPoint3D2.index = firstPoint.index;
					graphicsPath2 = this.Draw3DSurface(dataPoint3D2, secondPoint, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, surfaceSegmentType, topDarkening, bottomDarkening, new PointF(float.NaN, float.NaN), new PointF(float.NaN, float.NaN), clippedSegment, true, true);
					goto IL_03ca;
					IL_0347:
					if (num2 == 1 && !reversed)
					{
						goto IL_0356;
					}
					if (num2 == 0 && reversed)
					{
						goto IL_0356;
					}
					goto IL_03ca;
					IL_03ca:
					if (graphicsPath != null && graphicsPath2 != null && graphicsPath2.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath2, true);
					}
					this.thirdPointY2Value = float.NaN;
					this.fourthPointY2Value = float.NaN;
				}
				return graphicsPath;
			}
			goto IL_040a;
			IL_040a:
			float num3 = (float)Math.Min(firstPoint.xPosition, secondPoint.xPosition);
			float val = (float)Math.Min(firstPoint.yPosition, secondPoint.yPosition);
			val = Math.Min(val, num);
			float num4 = (float)Math.Max(firstPoint.xPosition, secondPoint.xPosition);
			float val2 = (float)Math.Max(firstPoint.yPosition, secondPoint.yPosition);
			val2 = Math.Max(val2, num);
			RectangleF position = new RectangleF(num3, val, num4 - num3, val2 - val);
			SurfaceNames surfaceNames = graph.GetVisibleSurfaces(position, positionZ, depth, matrix);
			bool upSideDown = false;
			if (firstPoint.yPosition >= (double)pointF.Y && secondPoint.yPosition >= (double)pointF2.Y)
			{
				upSideDown = true;
				bool flag2 = (surfaceNames & SurfaceNames.Top) == SurfaceNames.Top;
				bool flag3 = (surfaceNames & SurfaceNames.Bottom) == SurfaceNames.Bottom;
				surfaceNames ^= SurfaceNames.Bottom;
				surfaceNames ^= SurfaceNames.Top;
				if (flag2)
				{
					surfaceNames |= SurfaceNames.Bottom;
				}
				if (flag3)
				{
					surfaceNames |= SurfaceNames.Top;
				}
			}
			this.GetTopSurfaceVisibility(area, firstPoint, secondPoint, upSideDown, positionZ, depth, matrix, ref surfaceNames);
			bool flag4 = true;
			if (tension != 0.0)
			{
				if ((surfaceNames & SurfaceNames.Bottom) == SurfaceNames.Bottom)
				{
					flag4 = false;
				}
				if ((surfaceNames & SurfaceNames.Bottom) == (SurfaceNames)0 && (surfaceNames & SurfaceNames.Top) == (SurfaceNames)0)
				{
					flag4 = false;
				}
				surfaceNames |= SurfaceNames.Bottom;
				surfaceNames |= SurfaceNames.Top;
			}
			firstPoint.xPosition = Math.Round(firstPoint.xPosition, 5);
			firstPoint.yPosition = Math.Round(firstPoint.yPosition, 5);
			secondPoint.xPosition = Math.Round(secondPoint.xPosition, 5);
			secondPoint.yPosition = Math.Round(secondPoint.yPosition, 5);
			if (base.ClipTopPoints(graphicsPath, ref firstPoint, ref secondPoint, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, surfaceSegmentType, topDarkening, bottomDarkening))
			{
				return graphicsPath;
			}
			if (base.ClipBottomPoints(graphicsPath, ref firstPoint, ref secondPoint, ref pointF, ref pointF2, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, surfaceSegmentType, topDarkening, bottomDarkening))
			{
				return graphicsPath;
			}
			for (int i = 1; i <= 2; i++)
			{
				SurfaceNames[] array = null;
				array = ((!flag4) ? new SurfaceNames[6]
				{
					SurfaceNames.Back,
					SurfaceNames.Top,
					SurfaceNames.Bottom,
					SurfaceNames.Left,
					SurfaceNames.Right,
					SurfaceNames.Front
				} : new SurfaceNames[6]
				{
					SurfaceNames.Back,
					SurfaceNames.Bottom,
					SurfaceNames.Top,
					SurfaceNames.Left,
					SurfaceNames.Right,
					SurfaceNames.Front
				});
				LineSegmentType lineSegmentType = LineSegmentType.Middle;
				SurfaceNames[] array2 = array;
				foreach (SurfaceNames surfaceNames2 in array2)
				{
					if (ChartGraphics3D.ShouldDrawLineChartSurface(area, area.reverseSeriesOrder, surfaceNames2, surfaceNames, color, points, firstPoint, secondPoint, base.multiSeries, reversed, ref lineSegmentType) == i)
					{
						Color backColor = color;
						Color color2 = dataPoint3D.dataPoint.BorderColor;
						if (i == 1)
						{
							if (backColor.A == 255)
							{
								continue;
							}
							backColor = Color.Transparent;
							if (color2 == Color.Empty)
							{
								color2 = ChartGraphics.GetGradientColor(color, Color.Black, 0.2);
							}
						}
						GraphicsPath graphicsPath3 = null;
						switch (surfaceNames2)
						{
						case SurfaceNames.Top:
							graphicsPath3 = graph.Draw3DSurface(area, matrix, lightStyle, surfaceNames2, positionZ, depth, backColor, color2, dataPoint3D.dataPoint.BorderWidth, borderStyle, firstPoint, secondPoint, points, pointIndex, tension, operationType, LineSegmentType.Middle, (byte)(base.showPointLines ? 1 : 0) != 0, false, area.reverseSeriesOrder, base.multiSeries, 0, true);
							break;
						case SurfaceNames.Bottom:
						{
							DataPoint3D dataPoint3D14 = new DataPoint3D();
							dataPoint3D14.dataPoint = firstPoint.dataPoint;
							dataPoint3D14.index = firstPoint.index;
							dataPoint3D14.xPosition = firstPoint.xPosition;
							dataPoint3D14.yPosition = (double)pointF.Y;
							DataPoint3D dataPoint3D15 = new DataPoint3D();
							dataPoint3D15.dataPoint = secondPoint.dataPoint;
							dataPoint3D15.index = secondPoint.index;
							dataPoint3D15.xPosition = secondPoint.xPosition;
							dataPoint3D15.yPosition = (double)pointF2.Y;
							graphicsPath3 = graph.Draw3DSurface(area, matrix, lightStyle, surfaceNames2, positionZ, depth, backColor, color2, dataPoint3D.dataPoint.BorderWidth, borderStyle, dataPoint3D14, dataPoint3D15, points, pointIndex, tension, operationType, LineSegmentType.Middle, (byte)(base.showPointLines ? 1 : 0) != 0, false, area.reverseSeriesOrder, base.multiSeries, 1, true);
							break;
						}
						case SurfaceNames.Left:
						{
							if (surfaceSegmentType != 0 && (area.reverseSeriesOrder || surfaceSegmentType != LineSegmentType.First))
							{
								if (!area.reverseSeriesOrder)
								{
									break;
								}
								if (surfaceSegmentType != LineSegmentType.Last)
								{
									break;
								}
							}
							DataPoint3D dataPoint3D8 = (firstPoint.xPosition <= secondPoint.xPosition) ? firstPoint : secondPoint;
							DataPoint3D dataPoint3D9 = new DataPoint3D();
							dataPoint3D9.xPosition = dataPoint3D8.xPosition;
							dataPoint3D9.yPosition = (double)((firstPoint.xPosition <= secondPoint.xPosition) ? pointF.Y : pointF2.Y);
							DataPoint3D dataPoint3D10 = new DataPoint3D();
							dataPoint3D10.xPosition = dataPoint3D8.xPosition;
							dataPoint3D10.yPosition = dataPoint3D8.yPosition;
							graphicsPath3 = graph.Draw3DSurface(area, matrix, lightStyle, surfaceNames2, positionZ, depth, backColor, color2, dataPoint3D.dataPoint.BorderWidth, borderStyle, dataPoint3D9, dataPoint3D10, points, pointIndex, 0f, operationType, LineSegmentType.Single, false, true, area.reverseSeriesOrder, base.multiSeries, 0, true);
							break;
						}
						case SurfaceNames.Right:
						{
							if (surfaceSegmentType != 0 && (area.reverseSeriesOrder || surfaceSegmentType != LineSegmentType.Last))
							{
								if (!area.reverseSeriesOrder)
								{
									break;
								}
								if (surfaceSegmentType != LineSegmentType.First)
								{
									break;
								}
							}
							DataPoint3D dataPoint3D11 = (secondPoint.xPosition >= firstPoint.xPosition) ? secondPoint : firstPoint;
							DataPoint3D dataPoint3D12 = new DataPoint3D();
							dataPoint3D12.xPosition = dataPoint3D11.xPosition;
							dataPoint3D12.yPosition = (double)((secondPoint.xPosition >= firstPoint.xPosition) ? pointF2.Y : pointF.Y);
							DataPoint3D dataPoint3D13 = new DataPoint3D();
							dataPoint3D13.xPosition = dataPoint3D11.xPosition;
							dataPoint3D13.yPosition = dataPoint3D11.yPosition;
							graphicsPath3 = graph.Draw3DSurface(area, matrix, lightStyle, surfaceNames2, positionZ, depth, backColor, color2, dataPoint3D.dataPoint.BorderWidth, borderStyle, dataPoint3D12, dataPoint3D13, points, pointIndex, 0f, operationType, LineSegmentType.Single, false, true, area.reverseSeriesOrder, base.multiSeries, 0, true);
							break;
						}
						case SurfaceNames.Back:
						{
							DataPoint3D dataPoint3D6 = new DataPoint3D();
							dataPoint3D6.dataPoint = firstPoint.dataPoint;
							dataPoint3D6.index = firstPoint.index;
							dataPoint3D6.xPosition = firstPoint.xPosition;
							dataPoint3D6.yPosition = (double)pointF.Y;
							DataPoint3D dataPoint3D7 = new DataPoint3D();
							dataPoint3D7.dataPoint = secondPoint.dataPoint;
							dataPoint3D7.index = secondPoint.index;
							dataPoint3D7.xPosition = secondPoint.xPosition;
							dataPoint3D7.yPosition = (double)pointF2.Y;
							graphicsPath3 = this.Draw3DSplinePolygon(graph, area, positionZ, backColor, color2, dataPoint3D.dataPoint.BorderWidth, borderStyle, firstPoint, secondPoint, dataPoint3D7, dataPoint3D6, points, pointIndex, tension, operationType, lineSegmentType, (byte)(base.showPointLines ? 1 : 0) != 0);
							break;
						}
						case SurfaceNames.Front:
						{
							DataPoint3D dataPoint3D4 = new DataPoint3D();
							dataPoint3D4.dataPoint = firstPoint.dataPoint;
							dataPoint3D4.index = firstPoint.index;
							dataPoint3D4.xPosition = firstPoint.xPosition;
							dataPoint3D4.yPosition = (double)pointF.Y;
							DataPoint3D dataPoint3D5 = new DataPoint3D();
							dataPoint3D5.dataPoint = secondPoint.dataPoint;
							dataPoint3D5.index = secondPoint.index;
							dataPoint3D5.xPosition = secondPoint.xPosition;
							dataPoint3D5.yPosition = (double)pointF2.Y;
							if (area.reverseSeriesOrder)
							{
								switch (lineSegmentType)
								{
								case LineSegmentType.First:
									lineSegmentType = LineSegmentType.Last;
									break;
								case LineSegmentType.Last:
									lineSegmentType = LineSegmentType.First;
									break;
								}
							}
							switch (surfaceSegmentType)
							{
							case LineSegmentType.First:
								if (lineSegmentType == LineSegmentType.First)
								{
									goto default;
								}
								goto case LineSegmentType.Middle;
							default:
								if (surfaceSegmentType == LineSegmentType.Last && lineSegmentType != LineSegmentType.Last)
								{
									goto case LineSegmentType.Middle;
								}
								goto IL_0b77;
							case LineSegmentType.Middle:
								lineSegmentType = LineSegmentType.Middle;
								goto IL_0b77;
							case LineSegmentType.Single:
								break;
								IL_0b77:
								if (reversed)
								{
									switch (lineSegmentType)
									{
									case LineSegmentType.First:
										lineSegmentType = LineSegmentType.Last;
										break;
									case LineSegmentType.Last:
										lineSegmentType = LineSegmentType.First;
										break;
									}
								}
								break;
							}
							graphicsPath3 = this.Draw3DSplinePolygon(graph, area, positionZ + depth, backColor, color2, dataPoint3D.dataPoint.BorderWidth, borderStyle, firstPoint, secondPoint, dataPoint3D5, dataPoint3D4, points, pointIndex, tension, operationType, lineSegmentType, (byte)(base.showPointLines ? 1 : 0) != 0);
							break;
						}
						}
						if (i == 2 && graphicsPath != null && graphicsPath3 != null && graphicsPath3.PointCount > 0)
						{
							graphicsPath.CloseFigure();
							graphicsPath.AddPath(graphicsPath3, true);
						}
					}
				}
			}
			return graphicsPath;
		}

		protected virtual void GetTopSurfaceVisibility(ChartArea area, DataPoint3D firstPoint, DataPoint3D secondPoint, bool upSideDown, float positionZ, float depth, Matrix3D matrix, ref SurfaceNames visibleSurfaces)
		{
			if ((visibleSurfaces & SurfaceNames.Top) == SurfaceNames.Top)
			{
				visibleSurfaces ^= SurfaceNames.Top;
			}
			Point3D[] array = new Point3D[3];
			if (!area.reverseSeriesOrder)
			{
				if (!upSideDown && firstPoint.xPosition < secondPoint.xPosition)
				{
					goto IL_0048;
				}
				if (upSideDown && firstPoint.xPosition > secondPoint.xPosition)
				{
					goto IL_0048;
				}
				array[0] = new Point3D((float)secondPoint.xPosition, (float)secondPoint.yPosition, positionZ + depth);
				array[1] = new Point3D((float)secondPoint.xPosition, (float)secondPoint.yPosition, positionZ);
				array[2] = new Point3D((float)firstPoint.xPosition, (float)firstPoint.yPosition, positionZ);
			}
			else
			{
				if (!upSideDown && secondPoint.xPosition < firstPoint.xPosition)
				{
					goto IL_010c;
				}
				if (upSideDown && secondPoint.xPosition > firstPoint.xPosition)
				{
					goto IL_010c;
				}
				array[0] = new Point3D((float)firstPoint.xPosition, (float)firstPoint.yPosition, positionZ + depth);
				array[1] = new Point3D((float)firstPoint.xPosition, (float)firstPoint.yPosition, positionZ);
				array[2] = new Point3D((float)secondPoint.xPosition, (float)secondPoint.yPosition, positionZ);
			}
			goto IL_01a4;
			IL_010c:
			array[0] = new Point3D((float)secondPoint.xPosition, (float)secondPoint.yPosition, positionZ + depth);
			array[1] = new Point3D((float)secondPoint.xPosition, (float)secondPoint.yPosition, positionZ);
			array[2] = new Point3D((float)firstPoint.xPosition, (float)firstPoint.yPosition, positionZ);
			goto IL_01a4;
			IL_0048:
			array[0] = new Point3D((float)firstPoint.xPosition, (float)firstPoint.yPosition, positionZ + depth);
			array[1] = new Point3D((float)firstPoint.xPosition, (float)firstPoint.yPosition, positionZ);
			array[2] = new Point3D((float)secondPoint.xPosition, (float)secondPoint.yPosition, positionZ);
			goto IL_01a4;
			IL_0226:
			PointF pointF = default(PointF);
			array[0] = new Point3D((float)firstPoint.xPosition, pointF.Y, positionZ + depth);
			array[1] = new Point3D((float)firstPoint.xPosition, pointF.Y, positionZ);
			PointF pointF2 = default(PointF);
			array[2] = new Point3D((float)secondPoint.xPosition, pointF2.Y, positionZ);
			goto IL_038e;
			IL_02f0:
			array[0] = new Point3D((float)secondPoint.xPosition, pointF2.Y, positionZ + depth);
			array[1] = new Point3D((float)secondPoint.xPosition, pointF2.Y, positionZ);
			array[2] = new Point3D((float)firstPoint.xPosition, pointF.Y, positionZ);
			goto IL_038e;
			IL_01a4:
			matrix.TransformPoints(array);
			if (ChartGraphics3D.IsSurfaceVisible(array[0], array[1], array[2]))
			{
				visibleSurfaces |= SurfaceNames.Top;
			}
			this.GetBottomPointsPosition(area.Common, area, 0f, ref firstPoint, ref secondPoint, out pointF, out pointF2);
			if ((visibleSurfaces & SurfaceNames.Bottom) == SurfaceNames.Bottom)
			{
				visibleSurfaces ^= SurfaceNames.Bottom;
			}
			array = new Point3D[3];
			if (!area.reverseSeriesOrder)
			{
				if (!upSideDown && firstPoint.xPosition < secondPoint.xPosition)
				{
					goto IL_0226;
				}
				if (upSideDown && firstPoint.xPosition > secondPoint.xPosition)
				{
					goto IL_0226;
				}
				array[0] = new Point3D((float)secondPoint.xPosition, pointF2.Y, positionZ + depth);
				array[1] = new Point3D((float)secondPoint.xPosition, pointF2.Y, positionZ);
				array[2] = new Point3D((float)firstPoint.xPosition, pointF.Y, positionZ);
			}
			else
			{
				if (!upSideDown && secondPoint.xPosition < firstPoint.xPosition)
				{
					goto IL_02f0;
				}
				if (upSideDown && secondPoint.xPosition > firstPoint.xPosition)
				{
					goto IL_02f0;
				}
				array[0] = new Point3D((float)firstPoint.xPosition, pointF.Y, positionZ + depth);
				array[1] = new Point3D((float)firstPoint.xPosition, pointF.Y, positionZ);
				array[2] = new Point3D((float)secondPoint.xPosition, pointF2.Y, positionZ);
			}
			goto IL_038e;
			IL_038e:
			matrix.TransformPoints(array);
			if (ChartGraphics3D.IsSurfaceVisible(array[2], array[1], array[0]))
			{
				visibleSurfaces |= SurfaceNames.Bottom;
			}
		}

		protected virtual void GetBottomPointsPosition(CommonElements common, ChartArea area, float axisPosition, ref DataPoint3D firstPoint, ref DataPoint3D secondPoint, out PointF thirdPoint, out PointF fourthPoint)
		{
			Axis axis = area.GetAxis(AxisName.Y, firstPoint.dataPoint.series.YAxisType, firstPoint.dataPoint.series.YSubAxisName);
			float y = (float)axis.GetPosition(firstPoint.dataPoint.YValues[1]);
			thirdPoint = new PointF((float)firstPoint.xPosition, y);
			y = (float)axis.GetPosition(secondPoint.dataPoint.YValues[1]);
			fourthPoint = new PointF((float)secondPoint.xPosition, y);
			if (!float.IsNaN(this.thirdPointY2Value))
			{
				thirdPoint.Y = this.thirdPointY2Value;
			}
			if (!float.IsNaN(this.fourthPointY2Value))
			{
				fourthPoint.Y = this.fourthPointY2Value;
			}
		}

		internal GraphicsPath Draw3DSplinePolygon(ChartGraphics graph, ChartArea area, float positionZ, Color backColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, DataPoint3D firstPoint, DataPoint3D secondPoint, DataPoint3D thirdPoint, DataPoint3D fourthPoint, ArrayList points, int pointIndex, float tension, DrawingOperationTypes operationType, LineSegmentType lineSegmentType, bool forceThinBorder)
		{
			if (tension == 0.0)
			{
				SurfaceNames thinBorders = (SurfaceNames)0;
				if (forceThinBorder)
				{
					thinBorders = (SurfaceNames.Left | SurfaceNames.Right);
				}
				return graph.Draw3DPolygon(area, area.matrix3D, area.Area3DStyle.Light, SurfaceNames.Front, positionZ, backColor, borderColor, borderWidth, borderStyle, firstPoint, secondPoint, thirdPoint, fourthPoint, points, pointIndex, tension, operationType, lineSegmentType, thinBorders);
			}
			bool flag = (operationType & DrawingOperationTypes.DrawElement) == DrawingOperationTypes.DrawElement;
			GraphicsPath graphicsPath = new GraphicsPath();
			GraphicsPath splineFlattenPath = graph.GetSplineFlattenPath(area, area.matrix3D, positionZ, 0f, firstPoint, secondPoint, points, pointIndex, tension, false, true, 0);
			GraphicsPath splineFlattenPath2 = graph.GetSplineFlattenPath(area, area.matrix3D, positionZ, 0f, thirdPoint, fourthPoint, points, pointIndex, tension, false, true, 1);
			graphicsPath.AddPath(splineFlattenPath, true);
			graphicsPath.AddPath(splineFlattenPath2, true);
			graphicsPath.CloseAllFigures();
			Point3D[] array = new Point3D[3]
			{
				new Point3D((float)firstPoint.xPosition, (float)firstPoint.yPosition, positionZ),
				new Point3D((float)secondPoint.xPosition, (float)secondPoint.yPosition, positionZ),
				new Point3D((float)thirdPoint.xPosition, (float)thirdPoint.yPosition, positionZ)
			};
			area.matrix3D.TransformPoints(array);
			bool visiblePolygon = ChartGraphics3D.IsSurfaceVisible(array[0], array[1], array[2]);
			Color polygonLight = area.matrix3D.GetPolygonLight(array, backColor, visiblePolygon, (float)area.Area3DStyle.YAngle, SurfaceNames.Front, area.reverseSeriesOrder);
			Color color = borderColor;
			if (color == Color.Empty)
			{
				color = ChartGraphics.GetGradientColor(backColor, Color.Black, 0.2);
			}
			Pen pen = null;
			if (flag)
			{
				SmoothingMode smoothingMode = graph.SmoothingMode;
				graph.SmoothingMode = SmoothingMode.Default;
				graph.FillPath(new SolidBrush(polygonLight), graphicsPath);
				graph.SmoothingMode = smoothingMode;
				if (forceThinBorder)
				{
					graph.DrawPath(new Pen(color, 1f), graphicsPath);
				}
				else if (polygonLight.A == 255)
				{
					graph.DrawPath(new Pen(polygonLight, 1f), graphicsPath);
				}
				pen = new Pen(color, (float)borderWidth);
				pen.StartCap = LineCap.Round;
				pen.EndCap = LineCap.Round;
				graph.DrawPath(pen, splineFlattenPath);
				graph.DrawPath(pen, splineFlattenPath2);
				switch (lineSegmentType)
				{
				case LineSegmentType.First:
					graph.DrawLine(pen, splineFlattenPath.PathPoints[0], splineFlattenPath2.GetLastPoint());
					break;
				case LineSegmentType.Last:
					graph.DrawLine(pen, splineFlattenPath.GetLastPoint(), splineFlattenPath2.PathPoints[0]);
					break;
				}
			}
			if (graphicsPath != null && pen != null)
			{
				ChartGraphics.Widen(graphicsPath, pen);
			}
			return graphicsPath;
		}
	}
}
