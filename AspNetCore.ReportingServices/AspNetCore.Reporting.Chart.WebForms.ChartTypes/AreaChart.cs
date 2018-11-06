using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class AreaChart : SplineChart
	{
		protected bool gradientFill;

		protected GraphicsPath areaPath;

		protected Series series;

		protected PointF axisPos = PointF.Empty;

		public override string Name
		{
			get
			{
				return "Area";
			}
		}

		public override bool ZeroCrossing
		{
			get
			{
				return true;
			}
		}

		public AreaChart()
		{
			base.drawOutsideLines = true;
			base.lineTension = 0f;
			this.axisPos = PointF.Empty;
		}

		protected override float GetDefaultTension()
		{
			return 0f;
		}

		public override LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Rectangle;
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}

		protected override void ProcessChartType(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			this.gradientFill = false;
			this.axisPos = PointF.Empty;
			base.ProcessChartType(selection, graph, common, area, seriesToDraw);
			if (!area.Area3DStyle.Enable3D)
			{
				this.FillLastSeriesGradient(graph);
			}
		}

		protected override void DrawLine(ChartGraphics graph, CommonElements common, DataPoint point, Series series, PointF[] points, int pointIndex, float tension)
		{
			if (pointIndex > 0)
			{
				if (this.series != null)
				{
					if (this.series.Name != series.Name)
					{
						this.FillLastSeriesGradient(graph);
						this.series = series;
					}
				}
				else
				{
					this.series = series;
				}
				PointF pointF = points[pointIndex - 1];
				PointF pointF2 = points[pointIndex];
				pointF.X = (float)Math.Round((double)pointF.X);
				pointF.Y = (float)Math.Round((double)pointF.Y);
				pointF2.X = (float)Math.Round((double)pointF2.X);
				pointF2.Y = (float)Math.Round((double)pointF2.Y);
				if (this.axisPos == PointF.Empty)
				{
					this.axisPos.X = (float)base.vAxis.GetPosition(base.vAxis.Crossing);
					this.axisPos.Y = (float)base.vAxis.GetPosition(base.vAxis.Crossing);
					this.axisPos = graph.GetAbsolutePoint(this.axisPos);
					this.axisPos.X = (float)Math.Round((double)this.axisPos.X);
					this.axisPos.Y = (float)Math.Round((double)this.axisPos.Y);
				}
				Color color = point.Color;
				Color borderColor = point.BorderColor;
				int borderWidth = point.BorderWidth;
				ChartDashStyle borderStyle = point.BorderStyle;
				Brush brush = null;
				if (point.BackHatchStyle != 0)
				{
					brush = graph.GetHatchBrush(point.BackHatchStyle, color, point.BackGradientEndColor);
				}
				else if (point.BackGradientType != 0)
				{
					this.gradientFill = true;
					this.series = point.series;
				}
				else
				{
					brush = ((point.BackImage.Length <= 0 || point.BackImageMode == ChartImageWrapMode.Unscaled || point.BackImageMode == ChartImageWrapMode.Scaled) ? new SolidBrush(color) : graph.GetTextureBrush(point.BackImage, point.BackImageTransparentColor, point.BackImageMode, point.Color));
				}
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddLine(pointF.X, this.axisPos.Y, pointF.X, pointF.Y);
				if (base.lineTension == 0.0)
				{
					graphicsPath.AddLine(points[pointIndex - 1], points[pointIndex]);
				}
				else
				{
					graphicsPath.AddCurve(points, pointIndex - 1, 1, base.lineTension);
				}
				graphicsPath.AddLine(pointF2.X, pointF2.Y, pointF2.X, this.axisPos.Y);
				if (series.ShadowColor != Color.Empty && series.ShadowOffset != 0)
				{
					graph.shadowDrawingMode = true;
					if (color != Color.Empty && color != Color.Transparent)
					{
						Region region = new Region(graphicsPath);
						Brush brush2 = new SolidBrush((series.ShadowColor.A != 255) ? series.ShadowColor : Color.FromArgb((int)color.A / 2, series.ShadowColor));
						GraphicsState gstate = graph.Save();
						Region region2 = null;
						Region region3 = null;
						if (!graph.IsClipEmpty && !graph.Clip.IsInfinite(graph.Graphics))
						{
							region3 = graph.Clip.Clone();
							region2 = graph.Clip;
							region2.Translate(series.ShadowOffset, series.ShadowOffset);
							graph.Clip = region2;
						}
						graph.TranslateTransform((float)series.ShadowOffset, (float)series.ShadowOffset);
						if (graph.SmoothingMode != SmoothingMode.None)
						{
							Pen pen = new Pen(brush2, 1f);
							if (base.lineTension == 0.0)
							{
								graph.DrawLine(pen, points[pointIndex - 1], points[pointIndex]);
							}
							else
							{
								graph.DrawCurve(pen, points, pointIndex - 1, 1, base.lineTension);
							}
						}
						graph.FillRegion(brush2, region);
						graph.Restore(gstate);
						if (region2 != null && region3 != null)
						{
							graph.Clip = region3;
						}
					}
					graph.shadowDrawingMode = false;
				}
				if (!this.gradientFill)
				{
					SmoothingMode smoothingMode = graph.SmoothingMode;
					graph.SmoothingMode = SmoothingMode.None;
					graph.FillPath(brush, graphicsPath);
					graph.SmoothingMode = smoothingMode;
					if (graph.SmoothingMode != SmoothingMode.None)
					{
						graph.StartHotRegion(point);
						Pen pen2 = new Pen(brush, 1f);
						if (base.lineTension == 0.0)
						{
							if (points[pointIndex - 1].X != points[pointIndex].X && points[pointIndex - 1].Y != points[pointIndex].Y)
							{
								graph.DrawLine(pen2, points[pointIndex - 1], points[pointIndex]);
							}
						}
						else
						{
							graph.DrawCurve(pen2, points, pointIndex - 1, 1, base.lineTension);
						}
						graph.EndHotRegion();
					}
				}
				if (this.areaPath == null)
				{
					this.areaPath = new GraphicsPath();
					this.areaPath.AddLine(pointF.X, this.axisPos.Y, pointF.X, pointF.Y);
				}
				if (base.lineTension == 0.0)
				{
					this.areaPath.AddLine(points[pointIndex - 1], points[pointIndex]);
				}
				else
				{
					this.areaPath.AddCurve(points, pointIndex - 1, 1, base.lineTension);
				}
				if (borderWidth > 0 && borderColor != Color.Empty)
				{
					Pen pen3 = new Pen((borderColor != Color.Empty) ? borderColor : color, (float)borderWidth);
					pen3.DashStyle = graph.GetPenStyle(borderStyle);
					pen3.StartCap = LineCap.Round;
					pen3.EndCap = LineCap.Round;
					if (base.lineTension == 0.0)
					{
						graph.DrawLine(pen3, points[pointIndex - 1], points[pointIndex]);
					}
					else
					{
						graph.DrawCurve(pen3, points, pointIndex - 1, 1, base.lineTension);
					}
				}
				if (common.ProcessModeRegions)
				{
					GraphicsPath graphicsPath2 = new GraphicsPath();
					graphicsPath2.AddLine(pointF.X, this.axisPos.Y, pointF.X, pointF.Y);
					if (base.lineTension == 0.0)
					{
						graphicsPath2.AddLine(points[pointIndex - 1], points[pointIndex]);
					}
					else
					{
						graphicsPath2.AddCurve(points, pointIndex - 1, 1, base.lineTension);
						graphicsPath2.Flatten();
					}
					graphicsPath2.AddLine(pointF2.X, pointF2.Y, pointF2.X, this.axisPos.Y);
					graphicsPath2.AddLine(pointF2.X, this.axisPos.Y, pointF.X, this.axisPos.Y);
					PointF pointF3 = PointF.Empty;
					float[] array = new float[graphicsPath2.PointCount * 2];
					PointF[] pathPoints = graphicsPath2.PathPoints;
					for (int i = 0; i < graphicsPath2.PointCount; i++)
					{
						pointF3 = graph.GetRelativePoint(pathPoints[i]);
						array[2 * i] = pointF3.X;
						array[2 * i + 1] = pointF3.Y;
					}
					common.HotRegionsList.AddHotRegion(graph, graphicsPath2, false, array, point, series.Name, pointIndex);
					if (borderWidth > 1 && borderStyle != 0 && borderColor != Color.Empty)
					{
						try
						{
							graphicsPath2 = new GraphicsPath();
							if (base.lineTension == 0.0)
							{
								graphicsPath2.AddLine(points[pointIndex - 1], points[pointIndex]);
							}
							else
							{
								graphicsPath2.AddCurve(points, pointIndex - 1, 1, base.lineTension);
								graphicsPath2.Flatten();
							}
							ChartGraphics.Widen(graphicsPath2, new Pen(color, (float)(borderWidth + 2)));
						}
						catch (Exception)
						{
						}
						pointF3 = PointF.Empty;
						array = new float[graphicsPath2.PointCount * 2];
						PointF[] pathPoints2 = graphicsPath2.PathPoints;
						for (int j = 0; j < pathPoints2.Length; j++)
						{
							pointF3 = graph.GetRelativePoint(pathPoints2[j]);
							array[2 * j] = pointF3.X;
							array[2 * j + 1] = pointF3.Y;
						}
						common.HotRegionsList.AddHotRegion(graph, graphicsPath2, false, array, point, series.Name, pointIndex);
					}
				}
			}
		}

		private void FillLastSeriesGradient(ChartGraphics graph)
		{
			if (this.areaPath != null)
			{
				this.areaPath.AddLine(this.areaPath.GetLastPoint().X, this.areaPath.GetLastPoint().Y, this.areaPath.GetLastPoint().X, this.axisPos.Y);
			}
			if (this.gradientFill && this.areaPath != null)
			{
				graph.SetClip(base.area.PlotAreaPosition.ToRectangleF());
				Brush gradientBrush = graph.GetGradientBrush(this.areaPath.GetBounds(), this.series.Color, this.series.BackGradientEndColor, this.series.BackGradientType);
				graph.FillPath(gradientBrush, this.areaPath);
				this.gradientFill = false;
				graph.ResetClip();
			}
			if (this.areaPath != null)
			{
				this.areaPath.Dispose();
				this.areaPath = null;
			}
		}

		protected override bool IsLineTensionSupported()
		{
			return false;
		}

		protected override GraphicsPath Draw3DSurface(ChartArea area, ChartGraphics graph, Matrix3D matrix, LightStyle lightStyle, DataPoint3D prevDataPointEx, float positionZ, float depth, ArrayList points, int pointIndex, int pointLoopIndex, float tension, DrawingOperationTypes operationType, float topDarkening, float bottomDarkening, PointF thirdPointPosition, PointF fourthPointPosition, bool clippedSegment)
		{
			GraphicsPath graphicsPath = ((operationType & DrawingOperationTypes.CalcElementPath) == DrawingOperationTypes.CalcElementPath) ? new GraphicsPath() : null;
			if (base.centerPointIndex == 2147483647)
			{
				base.centerPointIndex = base.GetCenterPointIndex(points);
			}
			DataPoint3D dataPoint3D = (DataPoint3D)points[pointIndex];
			int num = pointIndex;
			DataPoint3D dataPoint3D2 = ChartGraphics3D.FindPointByIndex(points, dataPoint3D.index - 1, base.multiSeries ? dataPoint3D : null, ref num);
			bool flag = false;
			if (dataPoint3D2.index > dataPoint3D.index)
			{
				DataPoint3D dataPoint3D3 = dataPoint3D2;
				dataPoint3D2 = dataPoint3D;
				dataPoint3D = dataPoint3D3;
				flag = true;
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
				flag = false;
				for (int i = 1; pointIndex + i < points.Count; i++)
				{
					DataPoint3D dataPoint3D5 = (DataPoint3D)points[pointIndex + i];
					if (dataPoint3D5.dataPoint.series.Name == dataPoint3D2.dataPoint.series.Name)
					{
						if (dataPoint3D5.index == dataPoint3D2.index)
						{
							flag = true;
						}
						break;
					}
				}
				if (tension != 0.0)
				{
					GraphicsPath splineFlattenPath = graph.GetSplineFlattenPath(area, matrix, positionZ, depth, dataPoint3D2, dataPoint3D, points, pointIndex, tension, true, false, 0);
					PointF[] array = null;
					flag = (pointIndex < num);
					if (flag)
					{
						splineFlattenPath.Reverse();
					}
					array = splineFlattenPath.PathPoints;
					DataPoint3D dataPoint3D6 = new DataPoint3D();
					DataPoint3D dataPoint3D7 = new DataPoint3D();
					LineSegmentType lineSegmentType = LineSegmentType.Middle;
					for (int j = 1; j < array.Length; j++)
					{
						if (!flag)
						{
							dataPoint3D6.dataPoint = dataPoint3D2.dataPoint;
							dataPoint3D6.index = dataPoint3D2.index;
							dataPoint3D6.xPosition = (double)array[j - 1].X;
							dataPoint3D6.yPosition = (double)array[j - 1].Y;
							dataPoint3D7.dataPoint = dataPoint3D.dataPoint;
							dataPoint3D7.index = dataPoint3D.index;
							dataPoint3D7.xPosition = (double)array[j].X;
							dataPoint3D7.yPosition = (double)array[j].Y;
						}
						else
						{
							dataPoint3D7.dataPoint = dataPoint3D2.dataPoint;
							dataPoint3D7.index = dataPoint3D2.index;
							dataPoint3D7.xPosition = (double)array[j - 1].X;
							dataPoint3D7.yPosition = (double)array[j - 1].Y;
							dataPoint3D6.dataPoint = dataPoint3D.dataPoint;
							dataPoint3D6.index = dataPoint3D.index;
							dataPoint3D6.xPosition = (double)array[j].X;
							dataPoint3D6.yPosition = (double)array[j].Y;
						}
						lineSegmentType = LineSegmentType.Middle;
						if (j == 1)
						{
							lineSegmentType = (LineSegmentType)((!flag) ? 1 : 3);
						}
						else if (j == array.Length - 1)
						{
							lineSegmentType = (LineSegmentType)(flag ? 1 : 3);
						}
						area.IterationCounter = 0;
						GraphicsPath graphicsPath2 = this.Draw3DSurface(dataPoint3D6, dataPoint3D7, flag, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, 0f, operationType, lineSegmentType, topDarkening, bottomDarkening, new PointF(float.NaN, float.NaN), new PointF(float.NaN, float.NaN), clippedSegment, true, true);
						if (graphicsPath != null && graphicsPath2 != null && graphicsPath2.PointCount > 0)
						{
							graphicsPath.AddPath(graphicsPath2, true);
						}
					}
					return graphicsPath;
				}
				return this.Draw3DSurface(dataPoint3D2, dataPoint3D, flag, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, LineSegmentType.Single, topDarkening, bottomDarkening, thirdPointPosition, fourthPointPosition, clippedSegment, true, true);
			}
			return graphicsPath;
		}

		protected override GraphicsPath Draw3DSurface(DataPoint3D firstPoint, DataPoint3D secondPoint, bool reversed, ChartArea area, ChartGraphics graph, Matrix3D matrix, LightStyle lightStyle, DataPoint3D prevDataPointEx, float positionZ, float depth, ArrayList points, int pointIndex, int pointLoopIndex, float tension, DrawingOperationTypes operationType, LineSegmentType surfaceSegmentType, float topDarkening, float bottomDarkening, PointF thirdPointPosition, PointF fourthPointPosition, bool clippedSegment, bool clipOnTop, bool clipOnBottom)
		{
			GraphicsPath graphicsPath = ((operationType & DrawingOperationTypes.CalcElementPath) == DrawingOperationTypes.CalcElementPath) ? new GraphicsPath() : null;
			if (Math.Round(firstPoint.xPosition, 3) == Math.Round(secondPoint.xPosition, 3) && Math.Round(firstPoint.yPosition, 3) == Math.Round(secondPoint.yPosition, 3))
			{
				return graphicsPath;
			}
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
			float num = (float)Math.Round(base.vAxis.GetPosition(base.vAxis.Crossing), 3);
			float num2 = (float)Math.Min(firstPoint.xPosition, secondPoint.xPosition);
			float val = (float)Math.Min(firstPoint.yPosition, secondPoint.yPosition);
			val = Math.Min(val, num);
			float num3 = (float)Math.Max(firstPoint.xPosition, secondPoint.xPosition);
			float val2 = (float)Math.Max(firstPoint.yPosition, secondPoint.yPosition);
			val2 = Math.Max(val2, num);
			RectangleF position = new RectangleF(num2, val, num3 - num2, val2 - val);
			SurfaceNames surfaceNames = graph.GetVisibleSurfaces(position, positionZ, depth, matrix);
			bool upSideDown = false;
			if ((decimal)firstPoint.yPosition >= (decimal)num && (decimal)secondPoint.yPosition >= (decimal)num)
			{
				upSideDown = true;
				bool flag = (surfaceNames & SurfaceNames.Top) == SurfaceNames.Top;
				bool flag2 = (surfaceNames & SurfaceNames.Bottom) == SurfaceNames.Bottom;
				surfaceNames ^= SurfaceNames.Bottom;
				surfaceNames ^= SurfaceNames.Top;
				if (flag)
				{
					surfaceNames |= SurfaceNames.Bottom;
				}
				if (flag2)
				{
					surfaceNames |= SurfaceNames.Top;
				}
			}
			this.GetTopSurfaceVisibility(area, firstPoint, secondPoint, upSideDown, positionZ, depth, matrix, ref surfaceNames);
			PointF pointF = default(PointF);
			PointF pointF2 = default(PointF);
			this.GetBottomPointsPosition(base.common, area, num, ref firstPoint, ref secondPoint, thirdPointPosition, fourthPointPosition, out pointF, out pointF2);
			if (!float.IsNaN(thirdPointPosition.Y))
			{
				pointF.Y = thirdPointPosition.Y;
			}
			if (!float.IsNaN(fourthPointPosition.Y))
			{
				pointF2.Y = fourthPointPosition.Y;
			}
			if (!float.IsNaN(pointF.X) && !float.IsNaN(pointF.Y) && !float.IsNaN(pointF2.X) && !float.IsNaN(pointF2.Y))
			{
				if (clipOnTop && base.ClipTopPoints(graphicsPath, ref firstPoint, ref secondPoint, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, surfaceSegmentType, topDarkening, bottomDarkening))
				{
					return graphicsPath;
				}
				if (clipOnBottom && base.ClipBottomPoints(graphicsPath, ref firstPoint, ref secondPoint, ref pointF, ref pointF2, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, surfaceSegmentType, topDarkening, bottomDarkening))
				{
					return graphicsPath;
				}
				if (Math.Round((decimal)firstPoint.yPosition, 3) > (decimal)num + 0.001m && Math.Round((decimal)secondPoint.yPosition, 3) < (decimal)num - 0.001m)
				{
					goto IL_03d1;
				}
				if (Math.Round((decimal)firstPoint.yPosition, 3) < (decimal)num - 0.001m && Math.Round((decimal)secondPoint.yPosition, 3) > (decimal)num + 0.001m)
				{
					goto IL_03d1;
				}
				if (Math.Round(firstPoint.xPosition, 3) == Math.Round(secondPoint.xPosition, 3) && Math.Round(firstPoint.yPosition, 3) == Math.Round(secondPoint.yPosition, 3))
				{
					return graphicsPath;
				}
				for (int i = 1; i <= 2; i++)
				{
					SurfaceNames[] array = new SurfaceNames[6]
					{
						SurfaceNames.Back,
						SurfaceNames.Bottom,
						SurfaceNames.Top,
						SurfaceNames.Left,
						SurfaceNames.Right,
						SurfaceNames.Front
					};
					LineSegmentType lineSegmentType = LineSegmentType.Middle;
					SurfaceNames[] array2 = array;
					foreach (SurfaceNames surfaceNames2 in array2)
					{
						if (ChartGraphics3D.ShouldDrawLineChartSurface(area, area.reverseSeriesOrder, surfaceNames2, surfaceNames, color, points, firstPoint, secondPoint, base.multiSeries, reversed, ref lineSegmentType) == i)
						{
							if (base.allPointsLoopsNumber == 2 && (operationType & DrawingOperationTypes.DrawElement) == DrawingOperationTypes.DrawElement)
							{
								if (pointLoopIndex == 0 && (surfaceNames2 == SurfaceNames.Front || (i == 2 && (surfaceNames2 == SurfaceNames.Left || surfaceNames2 == SurfaceNames.Right))))
								{
									continue;
								}
								if (pointLoopIndex == 1)
								{
									switch (surfaceNames2)
									{
									case SurfaceNames.Front:
										goto IL_05fc;
									}
									if (i != 1 && (surfaceNames2 == SurfaceNames.Left || surfaceNames2 == SurfaceNames.Right))
									{
										goto IL_05fc;
									}
									continue;
								}
							}
							goto IL_05fc;
						}
						continue;
						IL_05fc:
						Color color2 = color;
						Color color3 = dataPoint3D.dataPoint.BorderColor;
						if (i == 1)
						{
							if (color2.A == 255)
							{
								continue;
							}
							color2 = Color.Transparent;
							if (color3 == Color.Empty)
							{
								color3 = ChartGraphics.GetGradientColor(color, Color.Black, 0.2);
							}
						}
						bool flag3 = base.showPointLines;
						if (surfaceSegmentType == LineSegmentType.Middle)
						{
							flag3 = false;
						}
						if (!clippedSegment || surfaceNames2 == SurfaceNames.Top || surfaceNames2 == SurfaceNames.Bottom)
						{
							GraphicsPath graphicsPath2 = null;
							switch (surfaceNames2)
							{
							case SurfaceNames.Top:
							{
								Color backColor = (topDarkening == 0.0) ? color2 : ChartGraphics.GetGradientColor(color2, Color.Black, (double)topDarkening);
								Color borderColor = (topDarkening == 0.0) ? color3 : ChartGraphics.GetGradientColor(color3, Color.Black, (double)topDarkening);
								graphicsPath2 = graph.Draw3DSurface(area, matrix, lightStyle, surfaceNames2, positionZ, depth, backColor, borderColor, dataPoint3D.dataPoint.BorderWidth, borderStyle, firstPoint, secondPoint, points, pointIndex, 0f, operationType, surfaceSegmentType, flag3, false, area.reverseSeriesOrder, base.multiSeries, 0, true);
								break;
							}
							case SurfaceNames.Bottom:
							{
								DataPoint3D dataPoint3D9 = new DataPoint3D();
								dataPoint3D9.index = firstPoint.index;
								dataPoint3D9.dataPoint = firstPoint.dataPoint;
								dataPoint3D9.xPosition = firstPoint.xPosition;
								dataPoint3D9.yPosition = (double)pointF.Y;
								DataPoint3D dataPoint3D10 = new DataPoint3D();
								dataPoint3D10.index = secondPoint.index;
								dataPoint3D10.dataPoint = secondPoint.dataPoint;
								dataPoint3D10.xPosition = secondPoint.xPosition;
								dataPoint3D10.yPosition = (double)pointF2.Y;
								Color backColor2 = (bottomDarkening == 0.0) ? color2 : ChartGraphics.GetGradientColor(color2, Color.Black, (double)topDarkening);
								Color borderColor2 = (bottomDarkening == 0.0) ? color3 : ChartGraphics.GetGradientColor(color3, Color.Black, (double)topDarkening);
								graphicsPath2 = graph.Draw3DSurface(area, matrix, lightStyle, surfaceNames2, positionZ, depth, backColor2, borderColor2, dataPoint3D.dataPoint.BorderWidth, borderStyle, dataPoint3D9, dataPoint3D10, points, pointIndex, 0f, operationType, surfaceSegmentType, flag3, false, area.reverseSeriesOrder, base.multiSeries, 0, true);
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
								DataPoint3D dataPoint3D4 = (firstPoint.xPosition <= secondPoint.xPosition) ? firstPoint : secondPoint;
								DataPoint3D dataPoint3D5 = new DataPoint3D();
								dataPoint3D5.index = dataPoint3D4.index;
								dataPoint3D5.dataPoint = dataPoint3D4.dataPoint;
								dataPoint3D5.xPosition = dataPoint3D4.xPosition;
								dataPoint3D5.yPosition = (double)((firstPoint.xPosition <= secondPoint.xPosition) ? pointF.Y : pointF2.Y);
								DataPoint3D dataPoint3D6 = new DataPoint3D();
								dataPoint3D6.index = dataPoint3D4.index;
								dataPoint3D6.dataPoint = dataPoint3D4.dataPoint;
								dataPoint3D6.xPosition = dataPoint3D4.xPosition;
								dataPoint3D6.yPosition = dataPoint3D4.yPosition;
								graphicsPath2 = graph.Draw3DSurface(area, matrix, lightStyle, surfaceNames2, positionZ, depth, color2, color3, dataPoint3D.dataPoint.BorderWidth, borderStyle, dataPoint3D5, dataPoint3D6, points, pointIndex, 0f, operationType, LineSegmentType.Single, true, true, area.reverseSeriesOrder, base.multiSeries, 0, true);
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
								dataPoint3D12.index = dataPoint3D11.index;
								dataPoint3D12.dataPoint = dataPoint3D11.dataPoint;
								dataPoint3D12.xPosition = dataPoint3D11.xPosition;
								dataPoint3D12.yPosition = (double)((secondPoint.xPosition >= firstPoint.xPosition) ? pointF2.Y : pointF.Y);
								DataPoint3D dataPoint3D13 = new DataPoint3D();
								dataPoint3D13.index = dataPoint3D11.index;
								dataPoint3D13.dataPoint = dataPoint3D11.dataPoint;
								dataPoint3D13.xPosition = dataPoint3D11.xPosition;
								dataPoint3D13.yPosition = dataPoint3D11.yPosition;
								graphicsPath2 = graph.Draw3DSurface(area, matrix, lightStyle, surfaceNames2, positionZ, depth, color2, color3, dataPoint3D.dataPoint.BorderWidth, borderStyle, dataPoint3D12, dataPoint3D13, points, pointIndex, 0f, operationType, LineSegmentType.Single, true, true, area.reverseSeriesOrder, base.multiSeries, 0, true);
								break;
							}
							case SurfaceNames.Back:
							{
								DataPoint3D dataPoint3D7 = new DataPoint3D();
								dataPoint3D7.index = firstPoint.index;
								dataPoint3D7.dataPoint = firstPoint.dataPoint;
								dataPoint3D7.xPosition = firstPoint.xPosition;
								dataPoint3D7.yPosition = (double)pointF.Y;
								DataPoint3D dataPoint3D8 = new DataPoint3D();
								dataPoint3D8.index = secondPoint.index;
								dataPoint3D8.dataPoint = secondPoint.dataPoint;
								dataPoint3D8.xPosition = secondPoint.xPosition;
								dataPoint3D8.yPosition = (double)pointF2.Y;
								SurfaceNames thinBorders2 = (SurfaceNames)0;
								if (flag3)
								{
									switch (surfaceSegmentType)
									{
									case LineSegmentType.Single:
										thinBorders2 = (SurfaceNames.Left | SurfaceNames.Right);
										break;
									case LineSegmentType.First:
										thinBorders2 = SurfaceNames.Left;
										break;
									case LineSegmentType.Last:
										thinBorders2 = SurfaceNames.Right;
										break;
									}
								}
								graphicsPath2 = graph.Draw3DPolygon(area, matrix, lightStyle, surfaceNames2, positionZ, color2, color3, dataPoint3D.dataPoint.BorderWidth, borderStyle, firstPoint, secondPoint, dataPoint3D8, dataPoint3D7, points, pointIndex, 0f, operationType, lineSegmentType, thinBorders2);
								break;
							}
							case SurfaceNames.Front:
							{
								DataPoint3D dataPoint3D2 = new DataPoint3D();
								dataPoint3D2.index = firstPoint.index;
								dataPoint3D2.dataPoint = firstPoint.dataPoint;
								dataPoint3D2.xPosition = firstPoint.xPosition;
								dataPoint3D2.yPosition = (double)pointF.Y;
								DataPoint3D dataPoint3D3 = new DataPoint3D();
								dataPoint3D3.index = secondPoint.index;
								dataPoint3D3.dataPoint = secondPoint.dataPoint;
								dataPoint3D3.xPosition = secondPoint.xPosition;
								dataPoint3D3.yPosition = (double)pointF2.Y;
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
									if (surfaceSegmentType != LineSegmentType.Last)
									{
										break;
									}
									if (lineSegmentType == LineSegmentType.Last)
									{
										break;
									}
									goto case LineSegmentType.Middle;
								case LineSegmentType.Middle:
									lineSegmentType = LineSegmentType.Middle;
									break;
								case LineSegmentType.Single:
									break;
								}
								SurfaceNames thinBorders = (SurfaceNames)0;
								if (flag3)
								{
									switch (surfaceSegmentType)
									{
									case LineSegmentType.Single:
										thinBorders = (SurfaceNames.Left | SurfaceNames.Right);
										break;
									case LineSegmentType.First:
										thinBorders = SurfaceNames.Left;
										break;
									case LineSegmentType.Last:
										thinBorders = SurfaceNames.Right;
										break;
									}
								}
								graphicsPath2 = graph.Draw3DPolygon(area, matrix, lightStyle, surfaceNames2, positionZ + depth, color2, color3, dataPoint3D.dataPoint.BorderWidth, borderStyle, firstPoint, secondPoint, dataPoint3D3, dataPoint3D2, points, pointIndex, 0f, operationType, lineSegmentType, thinBorders);
								break;
							}
							}
							if (i == 2 && graphicsPath != null && graphicsPath2 != null && graphicsPath2.PointCount > 0)
							{
								graphicsPath.CloseFigure();
								graphicsPath.SetMarkers();
								graphicsPath.AddPath(graphicsPath2, true);
							}
						}
					}
				}
				return graphicsPath;
			}
			return graphicsPath;
			IL_03d1:
			DataPoint3D axisIntersection = this.GetAxisIntersection(firstPoint, secondPoint, num);
			for (int num4 = 0; num4 <= 1; num4++)
			{
				GraphicsPath graphicsPath3 = null;
				if (num4 == 0 && !reversed)
				{
					goto IL_03f7;
				}
				if (num4 == 1 && reversed)
				{
					goto IL_03f7;
				}
				goto IL_045f;
				IL_03f7:
				axisIntersection.dataPoint = secondPoint.dataPoint;
				axisIntersection.index = secondPoint.index;
				graphicsPath3 = this.Draw3DSurface(firstPoint, axisIntersection, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, surfaceSegmentType, topDarkening, bottomDarkening, new PointF(float.NaN, float.NaN), new PointF(float.NaN, float.NaN), clippedSegment, clipOnTop, clipOnBottom);
				goto IL_045f;
				IL_046e:
				axisIntersection.dataPoint = firstPoint.dataPoint;
				axisIntersection.index = firstPoint.index;
				graphicsPath3 = this.Draw3DSurface(axisIntersection, secondPoint, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, surfaceSegmentType, topDarkening, bottomDarkening, new PointF(float.NaN, float.NaN), new PointF(float.NaN, float.NaN), clippedSegment, clipOnTop, clipOnBottom);
				goto IL_04d6;
				IL_045f:
				if (num4 == 1 && !reversed)
				{
					goto IL_046e;
				}
				if (num4 == 0 && reversed)
				{
					goto IL_046e;
				}
				goto IL_04d6;
				IL_04d6:
				if (graphicsPath != null && graphicsPath3 != null && graphicsPath3.PointCount > 0)
				{
					graphicsPath.AddPath(graphicsPath3, true);
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
				if (!upSideDown && firstPoint.xPosition <= secondPoint.xPosition)
				{
					goto IL_0048;
				}
				if (upSideDown && firstPoint.xPosition >= secondPoint.xPosition)
				{
					goto IL_0048;
				}
				array[0] = new Point3D((float)secondPoint.xPosition, (float)secondPoint.yPosition, positionZ + depth);
				array[1] = new Point3D((float)secondPoint.xPosition, (float)secondPoint.yPosition, positionZ);
				array[2] = new Point3D((float)firstPoint.xPosition, (float)firstPoint.yPosition, positionZ);
			}
			else
			{
				if (!upSideDown && secondPoint.xPosition <= firstPoint.xPosition)
				{
					goto IL_010c;
				}
				if (upSideDown && secondPoint.xPosition >= firstPoint.xPosition)
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
			IL_01a4:
			matrix.TransformPoints(array);
			if (ChartGraphics3D.IsSurfaceVisible(array[0], array[1], array[2]))
			{
				visibleSurfaces |= SurfaceNames.Top;
			}
			return;
			IL_0048:
			array[0] = new Point3D((float)firstPoint.xPosition, (float)firstPoint.yPosition, positionZ + depth);
			array[1] = new Point3D((float)firstPoint.xPosition, (float)firstPoint.yPosition, positionZ);
			array[2] = new Point3D((float)secondPoint.xPosition, (float)secondPoint.yPosition, positionZ);
			goto IL_01a4;
		}

		internal DataPoint3D GetAxisIntersection(DataPoint3D firstPoint, DataPoint3D secondPoint, float axisPosition)
		{
			DataPoint3D dataPoint3D = new DataPoint3D();
			dataPoint3D.yPosition = (double)axisPosition;
			dataPoint3D.xPosition = ((double)axisPosition - firstPoint.yPosition) * (secondPoint.xPosition - firstPoint.xPosition) / (secondPoint.yPosition - firstPoint.yPosition) + firstPoint.xPosition;
			return dataPoint3D;
		}

		protected virtual void GetBottomPointsPosition(CommonElements common, ChartArea area, float axisPosition, ref DataPoint3D firstPoint, ref DataPoint3D secondPoint, PointF thirdPointPosition, PointF fourthPointPosition, out PointF thirdPoint, out PointF fourthPoint)
		{
			thirdPoint = new PointF((float)firstPoint.xPosition, axisPosition);
			fourthPoint = new PointF((float)secondPoint.xPosition, axisPosition);
		}

		protected override int GetPointLoopNumber(bool selection, ArrayList pointsArray)
		{
			if (selection)
			{
				return 1;
			}
			int result = 1;
			foreach (object item in pointsArray)
			{
				DataPoint3D dataPoint3D = (DataPoint3D)item;
				if (dataPoint3D.dataPoint.Color.A != 255)
				{
					result = 2;
				}
			}
			return result;
		}
	}
}
