using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class StackedAreaChart : AreaChart
	{
		private ArrayList stackedData = new ArrayList();

		protected GraphicsPath areaBottomPath = new GraphicsPath();

		protected double prevPosY = double.NaN;

		protected double prevNegY = double.NaN;

		protected double prevPositionX = double.NaN;

		protected bool hundredPercentStacked;

		public override string Name
		{
			get
			{
				return "StackedArea";
			}
		}

		public override bool Stacked
		{
			get
			{
				return true;
			}
		}

		public StackedAreaChart()
		{
			base.multiSeries = true;
			base.COPCoordinatesToCheck = (COPCoordinates.X | COPCoordinates.Y);
		}

		protected override float GetDefaultTension()
		{
			return 0f;
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}

		public override void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			graph.SetClip(area.PlotAreaPosition.ToRectangleF());
			this.ProcessChartType(false, graph, common, area, seriesToDraw);
			graph.ResetClip();
		}

		protected override void ProcessChartType(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			ArrayList arrayList = null;
			ArrayList arrayList2 = null;
			if (area.Area3DStyle.Enable3D)
			{
				base.ProcessChartType(selection, graph, common, area, seriesToDraw);
			}
			else
			{
				bool flag = area.IndexedSeries((string[])area.GetSeriesFromChartType(this.Name).ToArray(typeof(string)));
				bool flag2 = false;
				bool flag3 = false;
				int num = -1;
				PointF absolutePoint;
				foreach (Series item in common.DataManager.Series)
				{
					if (string.Compare(item.ChartTypeName, this.Name, StringComparison.OrdinalIgnoreCase) == 0 && !(item.ChartArea != area.Name) && item.IsVisible())
					{
						if (base.areaPath != null)
						{
							base.areaPath.Dispose();
							base.areaPath = null;
						}
						this.areaBottomPath.Reset();
						if (num == -1)
						{
							num = item.Points.Count;
						}
						else if (num != item.Points.Count)
						{
							throw new ArgumentException(SR.ExceptionStackedAreaChartSeriesDataPointsNumberMismatch);
						}
						base.hAxis = area.GetAxis(AxisName.X, item.XAxisType, item.XSubAxisName);
						base.vAxis = area.GetAxis(AxisName.Y, item.YAxisType, item.YSubAxisName);
						base.hAxisMin = base.hAxis.GetViewMinimum();
						base.hAxisMax = base.hAxis.GetViewMaximum();
						base.vAxisMin = base.vAxis.GetViewMinimum();
						base.vAxisMax = base.vAxis.GetViewMaximum();
						base.axisPos.X = (float)base.vAxis.GetPosition(base.vAxis.Crossing);
						base.axisPos.Y = (float)base.vAxis.GetPosition(base.vAxis.Crossing);
						base.axisPos = graph.GetAbsolutePoint(base.axisPos);
						if (arrayList2 == null)
						{
							arrayList2 = new ArrayList(item.Points.Count);
						}
						else
						{
							arrayList = arrayList2;
							arrayList2 = new ArrayList(item.Points.Count);
						}
						if (!selection)
						{
							common.EventsManager.OnBackPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
						}
						int num2 = 0;
						float num3 = base.axisPos.Y;
						float y = base.axisPos.Y;
						PointF pointF = PointF.Empty;
						PointF pointF2 = PointF.Empty;
						foreach (DataPoint point in item.Points)
						{
							point.positionRel = new PointF(float.NaN, float.NaN);
							double num4 = point.Empty ? 0.0 : this.GetYValue(common, area, item, point, num2, 0);
							double num5 = flag ? ((double)num2 + 1.0) : point.XValue;
							if (arrayList != null && num2 < arrayList.Count)
							{
								num4 += (double)arrayList[num2];
							}
							arrayList2.Insert(num2, num4);
							float y2 = (float)base.vAxis.GetPosition(num4);
							float x = (float)base.hAxis.GetPosition(num5);
							point.positionRel = new PointF(x, y2);
							num4 = base.vAxis.GetLogValue(num4);
							num5 = base.hAxis.GetLogValue(num5);
							if (pointF == PointF.Empty)
							{
								pointF.X = x;
								pointF.Y = y2;
								if (arrayList != null && num2 < arrayList.Count)
								{
									num3 = (float)base.vAxis.GetPosition((double)arrayList[num2]);
									absolutePoint = graph.GetAbsolutePoint(new PointF(num3, num3));
									num3 = absolutePoint.Y;
								}
								pointF = graph.GetAbsolutePoint(pointF);
								num2++;
							}
							else
							{
								pointF2.X = x;
								pointF2.Y = y2;
								if (arrayList != null && num2 < arrayList.Count)
								{
									y = (float)base.vAxis.GetPosition((double)arrayList[num2]);
									absolutePoint = graph.GetAbsolutePoint(new PointF(y, y));
									y = absolutePoint.Y;
								}
								pointF2 = graph.GetAbsolutePoint(pointF2);
								pointF.X = (float)Math.Round((double)pointF.X);
								pointF2.X = (float)Math.Round((double)pointF2.X);
								GraphicsPath graphicsPath = new GraphicsPath();
								graphicsPath.AddLine(pointF.X, pointF.Y, pointF2.X, pointF2.Y);
								graphicsPath.AddLine(pointF2.X, pointF2.Y, pointF2.X, y);
								graphicsPath.AddLine(pointF2.X, y, pointF.X, num3);
								graphicsPath.AddLine(pointF.X, num3, pointF.X, pointF.Y);
								if (common.ProcessModePaint)
								{
									if (!point.Empty)
									{
										this.GetYValue(common, area, item, item.Points[num2 - 1], num2 - 1, 0);
									}
									double num6 = flag ? ((double)num2) : item.Points[num2 - 1].XValue;
									if (num5 <= base.hAxisMin && num6 <= base.hAxisMin)
									{
										goto IL_053b;
									}
									if (num5 >= base.hAxisMax && num6 >= base.hAxisMax)
									{
										goto IL_053b;
									}
									Brush brush = null;
									if (point.BackHatchStyle != 0)
									{
										brush = graph.GetHatchBrush(point.BackHatchStyle, point.Color, point.BackGradientEndColor);
									}
									else if (point.BackGradientType != 0)
									{
										base.gradientFill = true;
										base.series = point.series;
									}
									else
									{
										brush = ((point.BackImage.Length <= 0 || point.BackImageMode == ChartImageWrapMode.Unscaled || point.BackImageMode == ChartImageWrapMode.Scaled) ? ((!point.Empty || !(point.Color == Color.Empty)) ? new SolidBrush(point.Color) : new SolidBrush(item.Color)) : graph.GetTextureBrush(point.BackImage, point.BackImageTransparentColor, point.BackImageMode, point.Color));
									}
									if (point.BorderColor != Color.Empty && point.BorderWidth > 0)
									{
										flag2 = true;
									}
									if (point.Label.Length > 0 || point.ShowLabelAsValue)
									{
										flag3 = true;
									}
									if (!base.gradientFill)
									{
										graph.StartAnimation();
										graph.StartHotRegion(point);
										SmoothingMode smoothingMode = graph.SmoothingMode;
										graph.SmoothingMode = SmoothingMode.None;
										graph.FillPath(brush, graphicsPath);
										graph.SmoothingMode = smoothingMode;
										Pen pen = new Pen(brush, 1f);
										if (pointF.X != pointF2.X && pointF.Y != pointF2.Y)
										{
											graph.DrawLine(pen, pointF.X, pointF.Y, pointF2.X, pointF2.Y);
										}
										if (pointF.X != pointF2.X && y != num3)
										{
											graph.DrawLine(pen, pointF2.X, y, pointF.X, num3);
										}
										graph.EndHotRegion();
										graph.StopAnimation();
									}
									if (base.areaPath == null)
									{
										base.areaPath = new GraphicsPath();
									}
									base.areaPath.AddLine(pointF.X, pointF.Y, pointF2.X, pointF2.Y);
									this.areaBottomPath.AddLine(pointF.X, num3, pointF2.X, y);
								}
								if (common.ProcessModeRegions)
								{
									PointF pointF3 = PointF.Empty;
									float[] array = new float[graphicsPath.PointCount * 2];
									PointF[] pathPoints = graphicsPath.PathPoints;
									for (int i = 0; i < graphicsPath.PointCount; i++)
									{
										pointF3 = graph.GetRelativePoint(pathPoints[i]);
										array[2 * i] = pointF3.X;
										array[2 * i + 1] = pointF3.Y;
									}
									common.HotRegionsList.AddHotRegion(graph, graphicsPath, false, array, point, item.Name, num2);
									if (point.BorderWidth > 1 && point.BorderStyle != 0 && point.BorderColor != Color.Empty)
									{
										GraphicsPath graphicsPath2 = new GraphicsPath();
										graphicsPath2.AddLine(pointF.X, pointF.Y, pointF2.X, pointF2.Y);
										ChartGraphics.Widen(graphicsPath2, new Pen(point.Color, (float)(point.BorderWidth + 2)));
										pointF3 = PointF.Empty;
										array = new float[graphicsPath2.PointCount * 2];
										for (int j = 0; j < graphicsPath2.PointCount; j++)
										{
											pointF3 = graph.GetRelativePoint(graphicsPath2.PathPoints[j]);
											array[2 * j] = pointF3.X;
											array[2 * j + 1] = pointF3.Y;
										}
										common.HotRegionsList.AddHotRegion(graph, graphicsPath2, false, array, point, item.Name, num2);
									}
								}
								pointF = pointF2;
								num3 = y;
								num2++;
							}
							continue;
							IL_053b:
							pointF = pointF2;
							num3 = y;
							num2++;
						}
						if (base.gradientFill && base.areaPath != null)
						{
							GraphicsPath graphicsPath3 = new GraphicsPath();
							graphicsPath3.AddPath(base.areaPath, true);
							this.areaBottomPath.Reverse();
							graphicsPath3.AddPath(this.areaBottomPath, true);
							Brush gradientBrush = graph.GetGradientBrush(graphicsPath3.GetBounds(), base.series.Color, base.series.BackGradientEndColor, base.series.BackGradientType);
							graph.FillPath(gradientBrush, graphicsPath3);
							base.areaPath.Dispose();
							base.areaPath = null;
							base.gradientFill = false;
						}
						this.areaBottomPath.Reset();
						if (!selection)
						{
							common.EventsManager.OnPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
						}
					}
				}
				if (flag2)
				{
					arrayList = null;
					arrayList2 = null;
					foreach (Series item2 in common.DataManager.Series)
					{
						if (string.Compare(item2.ChartTypeName, this.Name, StringComparison.OrdinalIgnoreCase) == 0 && !(item2.ChartArea != area.Name) && item2.IsVisible())
						{
							base.hAxis = area.GetAxis(AxisName.X, item2.XAxisType, item2.XSubAxisName);
							base.vAxis = area.GetAxis(AxisName.Y, item2.YAxisType, item2.YSubAxisName);
							base.axisPos.X = (float)base.vAxis.GetPosition(base.vAxis.Crossing);
							base.axisPos.Y = (float)base.vAxis.GetPosition(base.vAxis.Crossing);
							base.axisPos = graph.GetAbsolutePoint(base.axisPos);
							if (arrayList2 == null)
							{
								arrayList2 = new ArrayList(item2.Points.Count);
							}
							else
							{
								arrayList = arrayList2;
								arrayList2 = new ArrayList(item2.Points.Count);
							}
							int num7 = 0;
							float num8 = base.axisPos.Y;
							float num9 = base.axisPos.Y;
							PointF pointF4 = PointF.Empty;
							PointF pointF5 = PointF.Empty;
							foreach (DataPoint point2 in item2.Points)
							{
								double num10 = point2.Empty ? 0.0 : this.GetYValue(common, area, item2, point2, num7, 0);
								double axisValue = flag ? ((double)num7 + 1.0) : point2.XValue;
								if (arrayList != null && num7 < arrayList.Count)
								{
									num10 += (double)arrayList[num7];
								}
								arrayList2.Insert(num7, num10);
								float y3 = (float)base.vAxis.GetPosition(num10);
								float x2 = (float)base.hAxis.GetPosition(axisValue);
								if (pointF4 == PointF.Empty)
								{
									pointF4.X = x2;
									pointF4.Y = y3;
									if (arrayList != null && num7 < arrayList.Count)
									{
										num8 = (float)base.vAxis.GetPosition((double)arrayList[num7]);
										absolutePoint = graph.GetAbsolutePoint(new PointF(num8, num8));
										num8 = absolutePoint.Y;
									}
									pointF4 = graph.GetAbsolutePoint(pointF4);
									pointF5 = pointF4;
									num9 = num8;
								}
								else
								{
									pointF5.X = x2;
									pointF5.Y = y3;
									if (arrayList != null && num7 < arrayList.Count)
									{
										num9 = (float)base.vAxis.GetPosition((double)arrayList[num7]);
										absolutePoint = graph.GetAbsolutePoint(new PointF(num9, num9));
										num9 = absolutePoint.Y;
									}
									pointF5 = graph.GetAbsolutePoint(pointF5);
								}
								if (num7 != 0)
								{
									pointF4.X = (float)Math.Round((double)pointF4.X);
									pointF5.X = (float)Math.Round((double)pointF5.X);
									graph.StartAnimation();
									graph.DrawLineRel(point2.BorderColor, point2.BorderWidth, point2.BorderStyle, graph.GetRelativePoint(pointF4), graph.GetRelativePoint(pointF5), point2.series.ShadowColor, point2.series.ShadowOffset);
									graph.StopAnimation();
								}
								pointF4 = pointF5;
								num8 = num9;
								num7++;
							}
						}
					}
				}
				if (flag3)
				{
					arrayList = null;
					arrayList2 = null;
					foreach (Series item3 in common.DataManager.Series)
					{
						if (string.Compare(item3.ChartTypeName, this.Name, StringComparison.OrdinalIgnoreCase) == 0 && !(item3.ChartArea != area.Name) && item3.IsVisible())
						{
							base.hAxis = area.GetAxis(AxisName.X, item3.XAxisType, item3.XSubAxisName);
							base.vAxis = area.GetAxis(AxisName.Y, item3.YAxisType, item3.YSubAxisName);
							base.axisPos.X = (float)base.vAxis.GetPosition(base.vAxis.Crossing);
							base.axisPos.Y = (float)base.vAxis.GetPosition(base.vAxis.Crossing);
							base.axisPos = graph.GetAbsolutePoint(base.axisPos);
							if (arrayList2 == null)
							{
								arrayList2 = new ArrayList(item3.Points.Count);
							}
							else
							{
								arrayList = arrayList2;
								arrayList2 = new ArrayList(item3.Points.Count);
							}
							int num11 = 0;
							float num12 = base.axisPos.Y;
							float num13 = base.axisPos.Y;
							PointF pointF6 = PointF.Empty;
							PointF pointF7 = PointF.Empty;
							foreach (DataPoint point3 in item3.Points)
							{
								double num14 = point3.Empty ? 0.0 : this.GetYValue(common, area, item3, point3, num11, 0);
								double axisValue2 = flag ? ((double)num11 + 1.0) : point3.XValue;
								if (arrayList != null && num11 < arrayList.Count)
								{
									num14 += (double)arrayList[num11];
								}
								arrayList2.Insert(num11, num14);
								float y4 = (float)base.vAxis.GetPosition(num14);
								float x3 = (float)base.hAxis.GetPosition(axisValue2);
								if (pointF6 == PointF.Empty)
								{
									pointF6.X = x3;
									pointF6.Y = y4;
									if (arrayList != null && num11 < arrayList.Count)
									{
										num12 = (float)base.vAxis.GetPosition((double)arrayList[num11]);
										absolutePoint = graph.GetAbsolutePoint(new PointF(num12, num12));
										num12 = absolutePoint.Y;
									}
									pointF6 = graph.GetAbsolutePoint(pointF6);
									pointF7 = pointF6;
									num13 = num12;
								}
								else
								{
									pointF7.X = x3;
									pointF7.Y = y4;
									if (arrayList != null && num11 < arrayList.Count)
									{
										num13 = (float)base.vAxis.GetPosition((double)arrayList[num11]);
										absolutePoint = graph.GetAbsolutePoint(new PointF(num13, num13));
										num13 = absolutePoint.Y;
									}
									pointF7 = graph.GetAbsolutePoint(pointF7);
								}
								if (!point3.Empty && (item3.ShowLabelAsValue || point3.ShowLabelAsValue || point3.Label.Length > 0))
								{
									StringFormat stringFormat = new StringFormat();
									stringFormat.Alignment = StringAlignment.Center;
									stringFormat.LineAlignment = StringAlignment.Center;
									string text;
									if (point3.Label.Length == 0)
									{
										double value = this.GetYValue(common, area, item3, point3, num11, 0);
										if (this.hundredPercentStacked && point3.LabelFormat.Length == 0)
										{
											value = Math.Round(value, 2);
										}
										text = ValueConverter.FormatValue(item3.chart, point3, value, point3.LabelFormat, item3.YValueType, ChartElementType.DataPoint);
									}
									else
									{
										text = point3.ReplaceKeywords(point3.Label);
										if (item3.chart != null && item3.chart.LocalizeTextHandler != null)
										{
											text = item3.chart.LocalizeTextHandler(point3, text, point3.ElementId, ChartElementType.DataPoint);
										}
									}
									Region clip = graph.Clip;
									graph.Clip = new Region();
									graph.StartAnimation();
									PointF pointF8 = PointF.Empty;
									pointF8.X = pointF7.X;
									pointF8.Y = (float)(pointF7.Y - (pointF7.Y - num13) / 2.0);
									pointF8 = graph.GetRelativePoint(pointF8);
									SizeF relativeSize = graph.GetRelativeSize(graph.MeasureString(text, point3.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
									RectangleF empty = RectangleF.Empty;
									SizeF sizeF = new SizeF(relativeSize.Width, relativeSize.Height);
									sizeF.Height += (float)(relativeSize.Height / 8.0);
									sizeF.Width += sizeF.Width / (float)text.Length;
									empty = new RectangleF((float)(pointF8.X - sizeF.Width / 2.0), (float)(pointF8.Y - sizeF.Height / 2.0 - relativeSize.Height / 10.0), sizeF.Width, sizeF.Height);
									graph.DrawPointLabelStringRel(common, text, point3.Font, new SolidBrush(point3.FontColor), pointF8, stringFormat, point3.FontAngle, empty, point3.LabelBackColor, point3.LabelBorderColor, point3.LabelBorderWidth, point3.LabelBorderStyle, item3, point3, num11);
									graph.StopAnimation();
									graph.Clip = clip;
								}
								pointF6 = pointF7;
								num12 = num13;
								num11++;
							}
						}
					}
				}
			}
		}

		protected override GraphicsPath Draw3DSurface(ChartArea area, ChartGraphics graph, Matrix3D matrix, LightStyle lightStyle, DataPoint3D prevDataPointEx, float positionZ, float depth, ArrayList points, int pointIndex, int pointLoopIndex, float tension, DrawingOperationTypes operationType, float topDarkening, float bottomDarkening, PointF thirdPointPosition, PointF fourthPointPosition, bool clippedSegment)
		{
			if (pointLoopIndex != 2)
			{
				return base.Draw3DSurface(area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, topDarkening, bottomDarkening, thirdPointPosition, fourthPointPosition, clippedSegment);
			}
			DataPoint3D dataPoint3D = (DataPoint3D)points[pointIndex];
			if (dataPoint3D.index == 2)
			{
				int num = 0;
				DataPoint3D pointEx = ChartGraphics3D.FindPointByIndex(points, dataPoint3D.index - 1, dataPoint3D, ref num);
				this.DrawLabels3D(area, graph, area.Common, pointEx, positionZ, depth);
			}
			this.DrawLabels3D(area, graph, area.Common, dataPoint3D, positionZ, depth);
			return new GraphicsPath();
		}

		protected override void GetTopSurfaceVisibility(ChartArea area, DataPoint3D firstPoint, DataPoint3D secondPoint, bool upSideDown, float positionZ, float depth, Matrix3D matrix, ref SurfaceNames visibleSurfaces)
		{
			base.GetTopSurfaceVisibility(area, firstPoint, secondPoint, upSideDown, positionZ, depth, matrix, ref visibleSurfaces);
			if ((visibleSurfaces & SurfaceNames.Top) == SurfaceNames.Top)
			{
				bool flag = false;
				foreach (Series item in area.Common.DataManager.Series)
				{
					if (string.Compare(item.ChartTypeName, secondPoint.dataPoint.series.ChartTypeName, true, CultureInfo.CurrentCulture) == 0)
					{
						if (flag)
						{
							DataPointAttributes dataPointAttributes = item.Points[secondPoint.index - 1];
							if (item.Points[secondPoint.index - 1].Empty)
							{
								dataPointAttributes = item.EmptyPointStyle;
							}
							if (dataPointAttributes.Color.A == 255)
							{
								visibleSurfaces ^= SurfaceNames.Top;
							}
							break;
						}
						if (string.Compare(item.Name, secondPoint.dataPoint.series.Name, StringComparison.Ordinal) == 0)
						{
							flag = true;
						}
					}
				}
			}
			if ((visibleSurfaces & SurfaceNames.Bottom) != SurfaceNames.Bottom)
			{
				DataPointAttributes dataPointAttributes2 = null;
				foreach (Series item2 in area.Common.DataManager.Series)
				{
					if (string.Compare(item2.ChartTypeName, secondPoint.dataPoint.series.ChartTypeName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						if (dataPointAttributes2 != null && string.Compare(item2.Name, secondPoint.dataPoint.series.Name, StringComparison.Ordinal) == 0)
						{
							if (dataPointAttributes2.Color.A != 255)
							{
								visibleSurfaces |= SurfaceNames.Bottom;
							}
							break;
						}
						dataPointAttributes2 = item2.Points[secondPoint.index - 1];
						if (item2.Points[secondPoint.index - 1].Empty)
						{
							dataPointAttributes2 = item2.EmptyPointStyle;
						}
					}
				}
			}
		}

		protected override void GetBottomPointsPosition(CommonElements common, ChartArea area, float axisPosition, ref DataPoint3D firstPoint, ref DataPoint3D secondPoint, PointF thirdPointPosition, PointF fourthPointPosition, out PointF thirdPoint, out PointF fourthPoint)
		{
			Axis axis = area.GetAxis(AxisName.Y, firstPoint.dataPoint.series.YAxisType, firstPoint.dataPoint.series.YSubAxisName);
			Axis axis2 = area.GetAxis(AxisName.X, firstPoint.dataPoint.series.XAxisType, firstPoint.dataPoint.series.XSubAxisName);
			double yValue = this.GetYValue(area.Common, area, firstPoint.dataPoint.series, firstPoint.dataPoint, firstPoint.index - 1, 0);
			double num = (double)(float)firstPoint.xPosition;
			if (yValue >= 0.0)
			{
				if (double.IsNaN(this.prevPosY))
				{
					yValue = (double)axisPosition;
				}
				else
				{
					yValue = axis.GetPosition(this.prevPosY);
					num = axis2.GetPosition(this.prevPositionX);
				}
			}
			else if (double.IsNaN(this.prevNegY))
			{
				yValue = (double)axisPosition;
			}
			else
			{
				yValue = axis.GetPosition(this.prevNegY);
				num = axis2.GetPosition(this.prevPositionX);
			}
			thirdPoint = new PointF((float)num, (float)yValue);
			yValue = this.GetYValue(area.Common, area, secondPoint.dataPoint.series, secondPoint.dataPoint, secondPoint.index - 1, 0);
			num = (double)(float)secondPoint.xPosition;
			if (yValue >= 0.0)
			{
				if (double.IsNaN(this.prevPosY))
				{
					yValue = (double)axisPosition;
				}
				else
				{
					yValue = axis.GetPosition(this.prevPosY);
					num = axis2.GetPosition(this.prevPositionX);
				}
			}
			else if (double.IsNaN(this.prevNegY))
			{
				yValue = (double)axisPosition;
			}
			else
			{
				yValue = axis.GetPosition(this.prevNegY);
				num = axis2.GetPosition(this.prevPositionX);
			}
			fourthPoint = new PointF((float)num, (float)yValue);
			if (!float.IsNaN(thirdPointPosition.X))
			{
				thirdPoint.X = (float)((firstPoint.xCenterVal == 0.0) ? firstPoint.xPosition : firstPoint.xCenterVal);
				thirdPoint.Y = (thirdPointPosition.X - fourthPoint.X) / (thirdPoint.X - fourthPoint.X) * (thirdPoint.Y - fourthPoint.Y) + fourthPoint.Y;
				thirdPoint.X = thirdPointPosition.X;
			}
			if (!float.IsNaN(thirdPointPosition.Y))
			{
				thirdPoint.Y = thirdPointPosition.Y;
			}
			if (!float.IsNaN(fourthPointPosition.X))
			{
				fourthPoint.X = (float)((secondPoint.xCenterVal == 0.0) ? secondPoint.xPosition : secondPoint.xCenterVal);
				fourthPoint.Y = (fourthPointPosition.X - fourthPoint.X) / (thirdPoint.X - fourthPoint.X) * (thirdPoint.Y - fourthPoint.Y) + fourthPoint.Y;
				fourthPoint.X = fourthPointPosition.X;
			}
			if (!float.IsNaN(fourthPointPosition.Y))
			{
				fourthPoint.Y = fourthPointPosition.Y;
			}
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
				if (dataPoint3D.dataPoint.Label.Length > 0 || dataPoint3D.dataPoint.ShowLabelAsValue || dataPoint3D.dataPoint.series.ShowLabelAsValue)
				{
					return 3;
				}
			}
			return result;
		}

		private void DrawLabels3D(ChartArea area, ChartGraphics graph, CommonElements common, DataPoint3D pointEx, float positionZ, float depth)
		{
			string label = pointEx.dataPoint.Label;
			bool showLabelAsValue = pointEx.dataPoint.ShowLabelAsValue;
			if ((pointEx.dataPoint.Empty || (!pointEx.dataPoint.series.ShowLabelAsValue && !showLabelAsValue && label.Length <= 0)) && !showLabelAsValue && label.Length <= 0)
			{
				return;
			}
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			string text;
			if (label.Length == 0)
			{
				double value = pointEx.dataPoint.YValues[(base.labelYValueIndex == -1) ? base.yValueIndex : base.labelYValueIndex];
				if (this.hundredPercentStacked && pointEx.dataPoint.LabelFormat.Length == 0)
				{
					value = Math.Round(value, 2);
				}
				text = ValueConverter.FormatValue(pointEx.dataPoint.series.chart, pointEx.dataPoint, value, pointEx.dataPoint.LabelFormat, pointEx.dataPoint.series.YValueType, ChartElementType.DataPoint);
			}
			else
			{
				text = pointEx.dataPoint.ReplaceKeywords(label);
				if (pointEx.dataPoint.series.chart != null && pointEx.dataPoint.series.chart.LocalizeTextHandler != null)
				{
					text = pointEx.dataPoint.series.chart.LocalizeTextHandler(pointEx.dataPoint, text, pointEx.dataPoint.ElementId, ChartElementType.DataPoint);
				}
			}
			Point3D[] array = new Point3D[1]
			{
				new Point3D((float)pointEx.xPosition, (float)((float)(pointEx.yPosition + pointEx.height) / 2.0), positionZ + depth)
			};
			area.matrix3D.TransformPoints(array);
			SizeF relativeSize = graph.GetRelativeSize(graph.MeasureString(text, pointEx.dataPoint.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
			RectangleF backPosition = RectangleF.Empty;
			SizeF sizeF = new SizeF(relativeSize.Width, relativeSize.Height);
			sizeF.Height += (float)(relativeSize.Height / 8.0);
			sizeF.Width += sizeF.Width / (float)text.Length;
			backPosition = new RectangleF((float)(array[0].PointF.X - sizeF.Width / 2.0), (float)(array[0].PointF.Y - sizeF.Height / 2.0 - relativeSize.Height / 10.0), sizeF.Width, sizeF.Height);
			graph.DrawPointLabelStringRel(common, text, pointEx.dataPoint.Font, new SolidBrush(pointEx.dataPoint.FontColor), array[0].PointF, stringFormat, pointEx.dataPoint.FontAngle, backPosition, pointEx.dataPoint.LabelBackColor, pointEx.dataPoint.LabelBorderColor, pointEx.dataPoint.LabelBorderWidth, pointEx.dataPoint.LabelBorderStyle, pointEx.dataPoint.series, pointEx.dataPoint, pointEx.index - 1);
		}

		public override double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			double num = double.NaN;
			if (!area.Area3DStyle.Enable3D)
			{
				return point.YValues[0];
			}
			if (yValueIndex == -1)
			{
				Axis axis = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName);
				double crossing = axis.Crossing;
				num = this.GetYValue(common, area, series, point, pointIndex, 0);
				if (area.Area3DStyle.Enable3D && num < 0.0)
				{
					num = 0.0 - num;
				}
				if (num >= 0.0)
				{
					if (!double.IsNaN(this.prevPosY))
					{
						crossing = this.prevPosY;
					}
				}
				else if (!double.IsNaN(this.prevNegY))
				{
					crossing = this.prevNegY;
				}
				return num - crossing;
			}
			this.prevPosY = double.NaN;
			this.prevNegY = double.NaN;
			this.prevPositionX = double.NaN;
			foreach (Series item in common.DataManager.Series)
			{
				if (string.Compare(series.ChartArea, item.ChartArea, StringComparison.Ordinal) == 0 && string.Compare(series.ChartTypeName, item.ChartTypeName, StringComparison.OrdinalIgnoreCase) == 0 && item.IsVisible())
				{
					num = item.Points[pointIndex].YValues[0];
					if (area.Area3DStyle.Enable3D && num < 0.0)
					{
						num = 0.0 - num;
					}
					if (!double.IsNaN(num))
					{
						if (num >= 0.0 && !double.IsNaN(this.prevPosY))
						{
							num += this.prevPosY;
						}
						if (num < 0.0 && !double.IsNaN(this.prevNegY))
						{
							num += this.prevNegY;
						}
					}
					if (string.Compare(series.Name, item.Name, StringComparison.Ordinal) != 0)
					{
						if (num >= 0.0)
						{
							this.prevPosY = num;
						}
						if (num < 0.0)
						{
							this.prevNegY = num;
						}
						this.prevPositionX = item.Points[pointIndex].XValue;
						if (this.prevPositionX == 0.0 && ChartElement.IndexedSeries(series))
						{
							this.prevPositionX = (double)(pointIndex + 1);
						}
						continue;
					}
					return num;
				}
			}
			return num;
		}
	}
}
