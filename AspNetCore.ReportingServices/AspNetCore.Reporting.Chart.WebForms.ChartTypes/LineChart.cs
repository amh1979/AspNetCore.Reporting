using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class LineChart : PointChart
	{
		protected float lineTension;

		protected int centerPointIndex = 2147483647;

		protected bool useBorderColor;

		protected bool disableShadow;

		protected bool drawShadowOnly;

		private Pen linePen = new Pen(Color.Black);

		protected double hAxisMin;

		protected double hAxisMax;

		protected double vAxisMin;

		protected double vAxisMax;

		protected bool clipRegionSet;

		protected bool multiSeries;

		protected COPCoordinates COPCoordinatesToCheck = COPCoordinates.X;

		protected int allPointsLoopsNumber = 1;

		protected bool showPointLines;

		protected bool drawOutsideLines;

		private bool processBaseChart;

		public override string Name
		{
			get
			{
				return "Line";
			}
		}

		public override bool Stacked
		{
			get
			{
				return false;
			}
		}

		public override bool RequireAxes
		{
			get
			{
				return true;
			}
		}

		public override bool SupportLogarithmicAxes
		{
			get
			{
				return true;
			}
		}

		public override bool SwitchValueAxes
		{
			get
			{
				return false;
			}
		}

		public override bool SideBySideSeries
		{
			get
			{
				return false;
			}
		}

		public override bool ZeroCrossing
		{
			get
			{
				return true;
			}
		}

		public override bool DataPointsInLegend
		{
			get
			{
				return false;
			}
		}

		public override bool ApplyPaletteColorsToPoints
		{
			get
			{
				return false;
			}
		}

		public override int YValuesPerPoint
		{
			get
			{
				return 1;
			}
		}

		public LineChart()
			: base(false)
		{
			base.middleMarker = false;
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}

		public override LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Line;
		}

		public override void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			base.area = area;
			this.processBaseChart = false;
			this.ProcessChartType(false, graph, common, area, seriesToDraw);
			if (this.processBaseChart)
			{
				base.ProcessChartType(false, graph, common, area, seriesToDraw);
			}
		}

		protected override void ProcessChartType(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			if (area.Area3DStyle.Enable3D)
			{
				this.processBaseChart = true;
				this.ProcessLineChartType3D(selection, graph, common, area, seriesToDraw);
			}
			else
			{
				ArrayList seriesFromChartType = area.GetSeriesFromChartType(this.Name);
				bool flag = area.IndexedSeries((string[])seriesFromChartType.ToArray(typeof(string)));
				foreach (Series item in common.DataManager.Series)
				{
					if (string.Compare(item.ChartTypeName, this.Name, true, CultureInfo.CurrentCulture) == 0 && !(item.ChartArea != area.Name) && item.IsVisible() && (seriesToDraw == null || !(seriesToDraw.Name != item.Name)))
					{
						base.hAxis = area.GetAxis(AxisName.X, item.XAxisType, item.XSubAxisName);
						base.vAxis = area.GetAxis(AxisName.Y, item.YAxisType, item.YSubAxisName);
						this.hAxisMin = base.hAxis.GetViewMinimum();
						this.hAxisMax = base.hAxis.GetViewMaximum();
						this.vAxisMin = base.vAxis.GetViewMinimum();
						this.vAxisMax = base.vAxis.GetViewMaximum();
						float num = (float)((float)(graph.common.ChartPicture.Width - 1) / 100.0);
						float num2 = (float)((float)(graph.common.ChartPicture.Height - 1) / 100.0);
						if (!selection)
						{
							common.EventsManager.OnBackPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
						}
						bool flag2 = false;
						PointF[] array = null;
						if (this.lineTension == 0.0 && !common.ProcessModeRegions)
						{
							array = new PointF[item.Points.Count];
						}
						else
						{
							flag2 = true;
							array = this.GetPointsPosition(graph, item, flag);
							if (this.lineTension != 0.0)
							{
								float num3 = 0.1f;
								for (int i = 1; i < array.Length; i++)
								{
									if (Math.Abs(array[i - 1].X - array[i].X) < num3)
									{
										if (array[i].X > array[i - 1].X)
										{
											array[i].X = array[i - 1].X + num3;
										}
										else
										{
											array[i].X = array[i - 1].X - num3;
										}
									}
									if (Math.Abs(array[i - 1].Y - array[i].Y) < num3)
									{
										if (array[i].Y > array[i - 1].Y)
										{
											array[i].Y = array[i - 1].Y + num3;
										}
										else
										{
											array[i].Y = array[i - 1].Y - num3;
										}
									}
								}
							}
						}
						if (array.Length > 1)
						{
							int num4 = 0;
							DataPoint dataPoint = null;
							double num5 = 0.0;
							double num6 = 0.0;
							bool showLabelAsValue = item.ShowLabelAsValue;
							bool flag3 = false;
							foreach (DataPoint point in item.Points)
							{
								flag3 = false;
								point.positionRel = new PointF(float.NaN, float.NaN);
								if (!this.processBaseChart)
								{
									int markerSize = point.MarkerSize;
									string markerImage = point.MarkerImage;
									MarkerStyle markerStyle = point.MarkerStyle;
									if (base.alwaysDrawMarkers || markerStyle != 0 || markerImage.Length > 0 || showLabelAsValue || point.ShowLabelAsValue || point.Label.Length > 0)
									{
										this.processBaseChart = true;
									}
								}
								double yValue = this.GetYValue(common, area, item, point, num4, base.yValueIndex);
								double yValue2 = flag ? ((double)(num4 + 1)) : point.XValue;
								if (num4 != 0)
								{
									yValue = base.vAxis.GetLogValue(yValue);
									yValue2 = base.hAxis.GetLogValue(yValue2);
									if (yValue2 <= this.hAxisMin && num6 < this.hAxisMin)
									{
										goto IL_0486;
									}
									if (yValue2 >= this.hAxisMax && num6 > this.hAxisMax)
									{
										goto IL_0486;
									}
									if (yValue <= this.vAxisMin && num5 < this.vAxisMin)
									{
										goto IL_0486;
									}
									if (yValue >= this.vAxisMax && num5 > this.vAxisMax)
									{
										goto IL_0486;
									}
									goto IL_0563;
								}
								dataPoint = point;
								num5 = this.GetYValue(common, area, item, point, num4, 0);
								num6 = (flag ? ((double)(num4 + 1)) : point.XValue);
								num5 = base.vAxis.GetLogValue(num5);
								num6 = base.hAxis.GetLogValue(num6);
								point.positionRel = new PointF((float)base.hAxis.GetPosition(num6), (float)base.vAxis.GetPosition(num5));
								goto IL_0795;
								IL_0502:
								bool flag4;
								DataPoint dataPoint3;
								double num7;
								if (flag4)
								{
									this.GetYValue(common, area, item, dataPoint3, num4 + 1, base.yValueIndex);
									if (yValue < this.vAxisMin && num7 > this.vAxisMin)
									{
										goto IL_0545;
									}
									if (yValue > this.vAxisMax && num7 < this.vAxisMax)
									{
										goto IL_0545;
									}
								}
								goto IL_0548;
								IL_0548:
								if (flag4)
								{
									num4++;
									dataPoint = point;
									num5 = yValue;
									num6 = yValue2;
									continue;
								}
								goto IL_0563;
								IL_04ff:
								flag4 = false;
								goto IL_0502;
								IL_0486:
								if (!this.drawOutsideLines)
								{
									flag4 = true;
									if (common.ProcessModeRegions && num4 + 1 < item.Points.Count)
									{
										dataPoint3 = item.Points[num4 + 1];
										num7 = (flag ? ((double)(num4 + 2)) : dataPoint3.XValue);
										if (yValue2 < this.hAxisMin && num7 > this.hAxisMin)
										{
											goto IL_04ff;
										}
										if (yValue2 > this.hAxisMax && num7 < this.hAxisMax)
										{
											goto IL_04ff;
										}
										goto IL_0502;
									}
									goto IL_0548;
								}
								goto IL_0563;
								IL_0545:
								flag4 = false;
								goto IL_0548;
								IL_0795:
								if (num4 == 0)
								{
									this.DrawLine(graph, common, point, item, array, num4, this.lineTension);
								}
								num4++;
								continue;
								IL_0563:
								this.clipRegionSet = false;
								if ((double)this.lineTension != 0.0 || num6 < this.hAxisMin || num6 > this.hAxisMax || yValue2 > this.hAxisMax || yValue2 < this.hAxisMin || num5 < this.vAxisMin || num5 > this.vAxisMax || yValue < this.vAxisMin || yValue > this.vAxisMax)
								{
									graph.SetClip(area.PlotAreaPosition.ToRectangleF());
									this.clipRegionSet = true;
								}
								if (this.lineTension == 0.0 && !flag2)
								{
									float num8 = 0f;
									float num9 = 0f;
									if (!flag3)
									{
										num8 = (float)base.vAxis.GetLinearPosition(num5);
										num9 = (float)base.hAxis.GetLinearPosition(num6);
										array[num4 - 1] = new PointF(num9 * num, num8 * num2);
									}
									num8 = (float)base.vAxis.GetLinearPosition(yValue);
									num9 = (float)base.hAxis.GetLinearPosition(yValue2);
									array[num4] = new PointF(num9 * num, num8 * num2);
									flag3 = true;
								}
								point.positionRel = graph.GetRelativePoint(array[num4]);
								this.Init2DAnimation(common, point, num4, yValue2, num6, graph, array);
								graph.StartHotRegion(point);
								graph.StartAnimation();
								if (num4 != 0 && dataPoint.Empty)
								{
									this.DrawLine(graph, common, dataPoint, item, array, num4, this.lineTension);
								}
								else
								{
									this.DrawLine(graph, common, point, item, array, num4, this.lineTension);
								}
								graph.StopAnimation();
								graph.EndHotRegion();
								if (this.clipRegionSet)
								{
									graph.ResetClip();
								}
								dataPoint = point;
								num5 = yValue;
								num6 = yValue2;
								goto IL_0795;
							}
						}
						else if (array.Length == 1 && item.Points.Count == 1 && !this.processBaseChart && (base.alwaysDrawMarkers || item.Points[0].MarkerStyle != 0 || item.Points[0].MarkerImage.Length > 0 || item.ShowLabelAsValue || item.Points[0].ShowLabelAsValue || item.Points[0].Label.Length > 0))
						{
							this.processBaseChart = true;
						}
						array = null;
						if (!selection)
						{
							common.EventsManager.OnPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
						}
					}
				}
			}
		}

		protected virtual void DrawLine(ChartGraphics graph, CommonElements common, DataPoint point, Series series, PointF[] points, int pointIndex, float tension)
		{
			int borderWidth = point.BorderWidth;
			if (common.ProcessModePaint && pointIndex > 0)
			{
				Color color = this.useBorderColor ? point.BorderColor : point.Color;
				ChartDashStyle borderStyle = point.BorderStyle;
				if (!this.disableShadow && series.ShadowOffset != 0 && series.ShadowColor != Color.Empty)
				{
					graph.shadowDrawingMode = true;
					if (color != Color.Empty && color != Color.Transparent && borderWidth > 0 && borderStyle != 0)
					{
						Pen pen = new Pen((series.ShadowColor.A != 255) ? series.ShadowColor : Color.FromArgb(this.useBorderColor ? ((int)point.BorderColor.A / 2) : ((int)point.Color.A / 2), series.ShadowColor), (float)borderWidth);
						pen.DashStyle = graph.GetPenStyle(point.BorderStyle);
						pen.StartCap = LineCap.Round;
						pen.EndCap = LineCap.Round;
						GraphicsState gstate = graph.Save();
						Matrix matrix = graph.Transform.Clone();
						matrix.Translate((float)series.ShadowOffset, (float)series.ShadowOffset);
						graph.Transform = matrix;
						if (this.lineTension == 0.0)
						{
							try
							{
								graph.DrawLine(pen, points[pointIndex - 1], points[pointIndex]);
							}
							catch (OverflowException)
							{
								this.DrawTruncatedLine(graph, pen, points[pointIndex - 1], points[pointIndex]);
							}
						}
						else
						{
							graph.DrawCurve(pen, points, pointIndex - 1, 1, tension);
						}
						graph.Restore(gstate);
					}
					graph.shadowDrawingMode = false;
				}
				if (!this.drawShadowOnly)
				{
					if (color != Color.Empty && borderWidth > 0 && borderStyle != 0)
					{
						if (this.linePen.Color != color)
						{
							this.linePen.Color = color;
						}
						if (this.linePen.Width != (float)borderWidth)
						{
							this.linePen.Width = (float)borderWidth;
						}
						if (this.linePen.DashStyle != graph.GetPenStyle(borderStyle))
						{
							this.linePen.DashStyle = graph.GetPenStyle(borderStyle);
						}
						if (this.linePen.StartCap != LineCap.Round)
						{
							this.linePen.StartCap = LineCap.Round;
						}
						if (this.linePen.EndCap != LineCap.Round)
						{
							this.linePen.EndCap = LineCap.Round;
						}
						if (tension == 0.0)
						{
							try
							{
								graph.DrawLine(this.linePen, points[pointIndex - 1], points[pointIndex]);
							}
							catch (OverflowException)
							{
								this.DrawTruncatedLine(graph, this.linePen, points[pointIndex - 1], points[pointIndex]);
							}
						}
						else
						{
							graph.DrawCurve(this.linePen, points, pointIndex - 1, 1, tension);
						}
					}
					goto IL_02fc;
				}
				return;
			}
			goto IL_02fc;
			IL_02fc:
			if (common.ProcessModeRegions)
			{
				int num = borderWidth + 2;
				GraphicsPath graphicsPath = new GraphicsPath();
				if (this.lineTension == 0.0)
				{
					if (pointIndex > 0)
					{
						PointF pointF = points[pointIndex - 1];
						PointF pointF2 = points[pointIndex];
						pointF.X = (float)((pointF.X + pointF2.X) / 2.0);
						pointF.Y = (float)((pointF.Y + pointF2.Y) / 2.0);
						if (Math.Abs(pointF.X - pointF2.X) > Math.Abs(pointF.Y - pointF2.Y))
						{
							graphicsPath.AddLine(pointF.X, pointF.Y - (float)num, pointF2.X, pointF2.Y - (float)num);
							graphicsPath.AddLine(pointF2.X, pointF2.Y + (float)num, pointF.X, pointF.Y + (float)num);
							graphicsPath.CloseAllFigures();
						}
						else
						{
							graphicsPath.AddLine(pointF.X - (float)num, pointF.Y, pointF2.X - (float)num, pointF2.Y);
							graphicsPath.AddLine(pointF2.X + (float)num, pointF2.Y, pointF.X + (float)num, pointF.Y);
							graphicsPath.CloseAllFigures();
						}
					}
					if (pointIndex + 1 < points.Length)
					{
						PointF pointF3 = points[pointIndex];
						PointF pointF4 = points[pointIndex + 1];
						pointF4.X = (float)((pointF3.X + pointF4.X) / 2.0);
						pointF4.Y = (float)((pointF3.Y + pointF4.Y) / 2.0);
						if (pointIndex > 0)
						{
							graphicsPath.SetMarkers();
						}
						if (Math.Abs(pointF3.X - pointF4.X) > Math.Abs(pointF3.Y - pointF4.Y))
						{
							graphicsPath.AddLine(pointF3.X, pointF3.Y - (float)num, pointF4.X, pointF4.Y - (float)num);
							graphicsPath.AddLine(pointF4.X, pointF4.Y + (float)num, pointF3.X, pointF3.Y + (float)num);
							graphicsPath.CloseAllFigures();
						}
						else
						{
							graphicsPath.AddLine(pointF3.X - (float)num, pointF3.Y, pointF4.X - (float)num, pointF4.Y);
							graphicsPath.AddLine(pointF4.X + (float)num, pointF4.Y, pointF3.X + (float)num, pointF3.Y);
							graphicsPath.CloseAllFigures();
						}
					}
				}
				else if (pointIndex > 0)
				{
					try
					{
						graphicsPath.AddCurve(points, pointIndex - 1, 1, this.lineTension);
						ChartGraphics.Widen(graphicsPath, new Pen(point.Color, (float)(borderWidth + 2)));
						graphicsPath.Flatten();
					}
					catch
					{
					}
				}
				if (graphicsPath.PointCount != 0)
				{
					PointF pointF5 = PointF.Empty;
					float[] array = new float[graphicsPath.PointCount * 2];
					PointF[] pathPoints = graphicsPath.PathPoints;
					for (int i = 0; i < graphicsPath.PointCount; i++)
					{
						pointF5 = graph.GetRelativePoint(pathPoints[i]);
						array[2 * i] = pointF5.X;
						array[2 * i + 1] = pointF5.Y;
					}
					common.HotRegionsList.AddHotRegion(graph, graphicsPath, false, array, point, series.Name, pointIndex);
				}
			}
		}

		private void DrawTruncatedLine(ChartGraphics graph, Pen pen, PointF pt1, PointF pt2)
		{
			PointF empty = PointF.Empty;
			PointF empty2 = PointF.Empty;
			if (Math.Abs(pt2.Y - pt1.Y) > Math.Abs(pt2.X - pt1.X))
			{
				empty = LineChart.GetIntersectionY(pt1, pt2, 0f);
				empty2 = LineChart.GetIntersectionY(pt1, pt2, (float)graph.common.ChartPicture.Height);
			}
			else
			{
				empty = LineChart.GetIntersectionX(pt1, pt2, 0f);
				empty2 = LineChart.GetIntersectionX(pt1, pt2, (float)graph.common.ChartPicture.Width);
			}
			graph.DrawLine(pen, empty, empty2);
		}

		internal static PointF GetIntersectionY(PointF firstPoint, PointF secondPoint, float pointY)
		{
			PointF result = default(PointF);
			result.Y = pointY;
			result.X = (pointY - firstPoint.Y) * (secondPoint.X - firstPoint.X) / (secondPoint.Y - firstPoint.Y) + firstPoint.X;
			return result;
		}

		internal static PointF GetIntersectionX(PointF firstPoint, PointF secondPoint, float pointX)
		{
			PointF result = default(PointF);
			result.X = pointX;
			result.Y = (pointX - firstPoint.X) * (secondPoint.Y - firstPoint.Y) / (secondPoint.X - firstPoint.X) + firstPoint.Y;
			return result;
		}

		protected void DrawLine(ChartGraphics graph, DataPoint point, Series series, PointF firstPoint, PointF secondPoint)
		{
			graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, firstPoint, secondPoint, series.ShadowColor, series.ShadowOffset);
		}

		protected virtual bool IsLineTensionSupported()
		{
			return false;
		}

		private void Init2DAnimation(CommonElements common, DataPoint point, int index, double xValue, double xValuePrev, ChartGraphics graph, PointF[] dataPointPos)
		{
		}

		protected virtual float GetDefaultTension()
		{
			return 0f;
		}

		protected override LabelAlignmentTypes GetAutoLabelPosition(Series series, int pointIndex)
		{
			int count = series.Points.Count;
			if (count == 1)
			{
				return LabelAlignmentTypes.Top;
			}
			double yValue = this.GetYValue(base.common, base.area, series, series.Points[pointIndex], pointIndex, 0);
			if (pointIndex < count - 1 && pointIndex > 0)
			{
				double yValue2 = this.GetYValue(base.common, base.area, series, series.Points[pointIndex - 1], pointIndex - 1, 0);
				double yValue3 = this.GetYValue(base.common, base.area, series, series.Points[pointIndex + 1], pointIndex + 1, 0);
				if (yValue2 > yValue && yValue3 > yValue)
				{
					return LabelAlignmentTypes.Bottom;
				}
			}
			if (pointIndex == count - 1)
			{
				double yValue2 = this.GetYValue(base.common, base.area, series, series.Points[pointIndex - 1], pointIndex - 1, 0);
				if (yValue2 > yValue)
				{
					return LabelAlignmentTypes.Bottom;
				}
			}
			if (pointIndex == 0)
			{
				double yValue3 = this.GetYValue(base.common, base.area, series, series.Points[pointIndex + 1], pointIndex + 1, 0);
				if (yValue3 > yValue)
				{
					return LabelAlignmentTypes.Bottom;
				}
			}
			return LabelAlignmentTypes.Top;
		}

		protected virtual PointF[] GetPointsPosition(ChartGraphics graph, Series series, bool indexedSeries)
		{
			PointF[] array = new PointF[series.Points.Count];
			int num = 0;
			foreach (DataPoint point in series.Points)
			{
				double yValue = this.GetYValue(base.common, base.area, series, point, num, base.yValueIndex);
				double position = base.vAxis.GetPosition(yValue);
				double position2 = base.hAxis.GetPosition(point.XValue);
				if (indexedSeries)
				{
					position2 = base.hAxis.GetPosition((double)(num + 1));
				}
				array[num] = new PointF((float)((float)position2 * (float)(graph.common.ChartPicture.Width - 1) / 100.0), (float)((float)position * (float)(graph.common.ChartPicture.Height - 1) / 100.0));
				num++;
			}
			return array;
		}

		protected void ProcessLineChartType3D(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			graph.frontLinePen = null;
			graph.frontLinePoint1 = PointF.Empty;
			graph.frontLinePoint2 = PointF.Empty;
			ArrayList arrayList = null;
			if (area.Area3DStyle.Clustered && this.SideBySideSeries)
			{
				goto IL_003d;
			}
			if (this.Stacked)
			{
				goto IL_003d;
			}
			arrayList = new ArrayList();
			arrayList.Add(seriesToDraw.Name);
			goto IL_0061;
			IL_0061:
			foreach (string item in arrayList)
			{
				Series series = common.DataManager.Series[item];
				if (!series.XValueIndexed)
				{
					bool flag = true;
					int num = 2147483647;
					double num2 = double.NaN;
					foreach (DataPoint point in series.Points)
					{
						if (!flag || point.XValue != 0.0)
						{
							flag = false;
							bool flag2 = true;
							if (!double.IsNaN(num2) && point.XValue != num2)
							{
								if (num == 2147483647)
								{
									num = ((!(point.XValue > num2)) ? 1 : 0);
								}
								if (point.XValue > num2 && num == 1)
								{
									flag2 = false;
								}
								if (point.XValue < num2 && num == 0)
								{
									flag2 = false;
								}
							}
							if (!flag2)
							{
								throw new InvalidOperationException(SR.Exception3DChartPointsXValuesUnsorted);
							}
							num2 = point.XValue;
						}
					}
				}
			}
			ArrayList dataPointDrawingOrder = area.GetDataPointDrawingOrder(arrayList, this, selection, this.COPCoordinatesToCheck, null, 0, false);
			this.lineTension = this.GetDefaultTension();
			if (dataPointDrawingOrder.Count > 0)
			{
				Series series2 = series2 = ((DataPoint3D)dataPointDrawingOrder[0]).dataPoint.series;
				if (this.IsLineTensionSupported() && series2.IsAttributeSet("LineTension"))
				{
					this.lineTension = CommonElements.ParseFloat(((DataPointAttributes)series2)["LineTension"]);
				}
			}
			this.allPointsLoopsNumber = this.GetPointLoopNumber(selection, dataPointDrawingOrder);
			for (int i = 0; i < this.allPointsLoopsNumber; i++)
			{
				int num3 = 0;
				this.centerPointIndex = 2147483647;
				foreach (object item2 in dataPointDrawingOrder)
				{
					DataPoint3D dataPoint3D = (DataPoint3D)item2;
					DataPoint dataPoint2 = dataPoint3D.dataPoint;
					Series series3 = dataPoint2.series;
					base.hAxis = area.GetAxis(AxisName.X, series3.XAxisType, series3.XSubAxisName);
					base.vAxis = area.GetAxis(AxisName.Y, series3.YAxisType, series3.YSubAxisName);
					this.hAxisMin = base.hAxis.GetViewMinimum();
					this.hAxisMax = base.hAxis.GetViewMaximum();
					this.vAxisMin = base.vAxis.GetViewMinimum();
					this.vAxisMax = base.vAxis.GetViewMaximum();
					if (dataPoint3D.index > 1)
					{
						int num4 = num3;
						DataPoint3D dataPoint3D2 = ChartGraphics3D.FindPointByIndex(dataPointDrawingOrder, dataPoint3D.index - 1, this.multiSeries ? dataPoint3D : null, ref num4);
						GraphicsPath graphicsPath = null;
						double yValue = this.GetYValue(common, area, series3, dataPoint3D.dataPoint, dataPoint3D.index - 1, 0);
						double yValue2 = this.GetYValue(common, area, series3, dataPoint3D2.dataPoint, dataPoint3D2.index - 1, 0);
						double yValue3 = dataPoint3D.indexedSeries ? ((double)dataPoint3D.index) : dataPoint3D.dataPoint.XValue;
						double yValue4 = dataPoint3D2.indexedSeries ? ((double)dataPoint3D2.index) : dataPoint3D2.dataPoint.XValue;
						yValue = base.vAxis.GetLogValue(yValue);
						yValue2 = base.vAxis.GetLogValue(yValue2);
						yValue3 = base.hAxis.GetLogValue(yValue3);
						yValue4 = base.hAxis.GetLogValue(yValue4);
						DataPoint3D dataPoint3D3 = dataPoint3D2.dataPoint.Empty ? dataPoint3D2 : dataPoint3D;
						if (dataPoint3D3.dataPoint.Color != Color.Empty)
						{
							DrawingOperationTypes drawingOperationTypes = DrawingOperationTypes.DrawElement;
							if (common.ProcessModeRegions)
							{
								drawingOperationTypes |= DrawingOperationTypes.CalcElementPath;
							}
							this.showPointLines = false;
							if (dataPoint3D3.dataPoint.IsAttributeSet("ShowMarkerLines"))
							{
								if (string.Compare(((DataPointAttributes)dataPoint3D3.dataPoint)["ShowMarkerLines"], "TRUE", StringComparison.OrdinalIgnoreCase) == 0)
								{
									this.showPointLines = true;
								}
							}
							else if (dataPoint3D3.dataPoint.series.IsAttributeSet("ShowMarkerLines") && string.Compare(((DataPointAttributes)dataPoint3D3.dataPoint.series)["ShowMarkerLines"], "TRUE", StringComparison.OrdinalIgnoreCase) == 0)
							{
								this.showPointLines = true;
							}
							graph.StartHotRegion(dataPoint2);
							this.Init3DAnimation(common, yValue4, yValue3, yValue2, yValue, base.vAxis, base.hAxis, dataPoint3D3, graph, dataPoint3D2.dataPoint, seriesToDraw);
							graph.StartAnimation();
							area.IterationCounter = 0;
							graphicsPath = this.Draw3DSurface(area, graph, area.matrix3D, area.Area3DStyle.Light, dataPoint3D2, dataPoint3D3.zPosition, dataPoint3D3.depth, dataPointDrawingOrder, num3, i, this.lineTension, drawingOperationTypes, 0f, 0f, new PointF(float.NaN, float.NaN), new PointF(float.NaN, float.NaN), false);
							graph.StopAnimation();
							graph.EndHotRegion();
						}
						if (common.ProcessModeRegions && graphicsPath != null)
						{
							common.HotRegionsList.AddHotRegion(graphicsPath, false, graph, dataPoint2, series3.Name, dataPoint3D.index - 1);
						}
					}
					num3++;
				}
			}
			return;
			IL_003d:
			arrayList = area.GetSeriesFromChartType(this.Name);
			goto IL_0061;
		}

		private void Init3DAnimation(CommonElements common, double xValuePrev, double xValue, double yValuePrev, double yValue, Axis vAxis, Axis hAxis, DataPoint3D pointAttr, ChartGraphics graph, DataPoint point, Series series)
		{
		}

		protected virtual GraphicsPath Draw3DSurface(ChartArea area, ChartGraphics graph, Matrix3D matrix, LightStyle lightStyle, DataPoint3D prevDataPointEx, float positionZ, float depth, ArrayList points, int pointIndex, int pointLoopIndex, float tension, DrawingOperationTypes operationType, float topDarkening, float bottomDarkening, PointF thirdPointPosition, PointF fourthPointPosition, bool clippedSegment)
		{
			if (this.centerPointIndex == 2147483647)
			{
				this.centerPointIndex = this.GetCenterPointIndex(points);
			}
			DataPoint3D dataPoint3D = (DataPoint3D)points[pointIndex];
			int num = pointIndex;
			DataPoint3D dataPoint3D2 = ChartGraphics3D.FindPointByIndex(points, dataPoint3D.index - 1, this.multiSeries ? dataPoint3D : null, ref num);
			DataPoint3D dataPoint3D3 = dataPoint3D;
			if (prevDataPointEx.dataPoint.Empty)
			{
				dataPoint3D3 = prevDataPointEx;
			}
			else if (dataPoint3D2.index > dataPoint3D.index)
			{
				dataPoint3D3 = dataPoint3D2;
			}
			Color backColor = this.useBorderColor ? dataPoint3D3.dataPoint.BorderColor : dataPoint3D3.dataPoint.Color;
			ChartDashStyle borderStyle = dataPoint3D3.dataPoint.BorderStyle;
			if (dataPoint3D3.dataPoint.Empty && dataPoint3D3.dataPoint.Color == Color.Empty)
			{
				backColor = Color.Gray;
			}
			if (dataPoint3D3.dataPoint.Empty && dataPoint3D3.dataPoint.BorderStyle == ChartDashStyle.NotSet)
			{
				borderStyle = ChartDashStyle.Solid;
			}
			return graph.Draw3DSurface(area, matrix, lightStyle, SurfaceNames.Top, positionZ, depth, backColor, dataPoint3D3.dataPoint.BorderColor, dataPoint3D3.dataPoint.BorderWidth, borderStyle, dataPoint3D2, dataPoint3D, points, pointIndex, tension, operationType, LineSegmentType.Single, (byte)(this.showPointLines ? 1 : 0) != 0, false, area.reverseSeriesOrder, this.multiSeries, 0, true);
		}

		protected int GetCenterPointIndex(ArrayList points)
		{
			for (int i = 1; i < points.Count; i++)
			{
				DataPoint3D dataPoint3D = (DataPoint3D)points[i - 1];
				DataPoint3D dataPoint3D2 = (DataPoint3D)points[i];
				if (Math.Abs(dataPoint3D2.index - dataPoint3D.index) != 1)
				{
					return i - 1;
				}
			}
			return 2147483647;
		}

		protected virtual int GetPointLoopNumber(bool selection, ArrayList pointsArray)
		{
			return 1;
		}

		protected bool ClipTopPoints(GraphicsPath resultPath, ref DataPoint3D firstPoint, ref DataPoint3D secondPoint, bool reversed, ChartArea area, ChartGraphics graph, Matrix3D matrix, LightStyle lightStyle, DataPoint3D prevDataPointEx, float positionZ, float depth, ArrayList points, int pointIndex, int pointLoopIndex, float tension, DrawingOperationTypes operationType, LineSegmentType surfaceSegmentType, float topDarkening, float bottomDarkening)
		{
			area.IterationCounter++;
			if (area.IterationCounter > 20)
			{
				area.IterationCounter = 0;
				return true;
			}
			decimal d2;
			decimal d4;
			double yPosition;
			double yPosition2;
			DataPoint3D dataPoint3D;
			int num2;
			DataPoint3D dataPoint3D2;
			if (!double.IsNaN(firstPoint.xPosition) && !double.IsNaN(firstPoint.yPosition) && !double.IsNaN(secondPoint.xPosition) && !double.IsNaN(secondPoint.yPosition))
			{
				int num = 3;
				decimal d = Math.Round((decimal)area.PlotAreaPosition.X, num);
				d2 = Math.Round((decimal)area.PlotAreaPosition.Y, num);
				decimal d3 = Math.Round((decimal)area.PlotAreaPosition.Right(), num);
				d4 = Math.Round((decimal)area.PlotAreaPosition.Bottom(), num);
				d -= 0.001m;
				d2 -= 0.001m;
				d3 += 0.001m;
				d4 += 0.001m;
				firstPoint.xPosition = Math.Round(firstPoint.xPosition, num);
				firstPoint.yPosition = Math.Round(firstPoint.yPosition, num);
				secondPoint.xPosition = Math.Round(secondPoint.xPosition, num);
				secondPoint.yPosition = Math.Round(secondPoint.yPosition, num);
				if ((decimal)firstPoint.xPosition < d || (decimal)firstPoint.xPosition > d3 || (decimal)secondPoint.xPosition < d || (decimal)secondPoint.xPosition > d3)
				{
					if ((decimal)firstPoint.xPosition < d && (decimal)secondPoint.xPosition < d)
					{
						return true;
					}
					if ((decimal)firstPoint.xPosition > d3 && (decimal)secondPoint.xPosition > d3)
					{
						return true;
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
					return false;
				}
				yPosition = firstPoint.yPosition;
				yPosition2 = secondPoint.yPosition;
				bool flag = false;
				bool clippedSegment = false;
				if ((decimal)firstPoint.yPosition < d2 && (decimal)secondPoint.yPosition < d2)
				{
					flag = true;
					firstPoint.yPosition = (double)d2;
					secondPoint.yPosition = (double)d2;
				}
				if ((decimal)firstPoint.yPosition > d4 && (decimal)secondPoint.yPosition > d4)
				{
					flag = true;
					clippedSegment = true;
					firstPoint.yPosition = (double)d4;
					secondPoint.yPosition = (double)d4;
				}
				if (flag)
				{
					resultPath = this.Draw3DSurface(firstPoint, secondPoint, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, surfaceSegmentType, 0.5f, 0f, new PointF(float.NaN, float.NaN), new PointF(float.NaN, float.NaN), clippedSegment, false, true);
					firstPoint.yPosition = yPosition;
					secondPoint.yPosition = yPosition2;
					return true;
				}
				dataPoint3D = new DataPoint3D();
				dataPoint3D.yPosition = (double)d2;
				if ((decimal)firstPoint.yPosition > d4 || (decimal)secondPoint.yPosition > d4)
				{
					dataPoint3D.yPosition = (double)d4;
				}
				dataPoint3D.xPosition = (dataPoint3D.yPosition - secondPoint.yPosition) * (firstPoint.xPosition - secondPoint.xPosition) / (firstPoint.yPosition - secondPoint.yPosition) + secondPoint.xPosition;
				if (!double.IsNaN(dataPoint3D.xPosition) && !double.IsInfinity(dataPoint3D.xPosition) && !double.IsNaN(dataPoint3D.yPosition) && !double.IsInfinity(dataPoint3D.yPosition))
				{
					num2 = 2;
					dataPoint3D2 = null;
					if ((decimal)firstPoint.yPosition < d2 && (decimal)secondPoint.yPosition > d4)
					{
						goto IL_0632;
					}
					if ((decimal)firstPoint.yPosition > d4 && (decimal)secondPoint.yPosition < d2)
					{
						goto IL_0632;
					}
					goto IL_0759;
				}
				return true;
			}
			return true;
			IL_0632:
			num2 = 3;
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
			if (!double.IsNaN(dataPoint3D2.xPosition) && !double.IsInfinity(dataPoint3D2.xPosition) && !double.IsNaN(dataPoint3D2.yPosition) && !double.IsInfinity(dataPoint3D2.yPosition))
			{
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
				goto IL_0759;
			}
			return true;
			IL_0759:
			bool flag2 = true;
			bool clippedSegment2 = false;
			bool clippedSegment3 = false;
			if ((decimal)firstPoint.yPosition < d2)
			{
				flag2 = false;
				firstPoint.yPosition = (double)d2;
			}
			else if ((decimal)firstPoint.yPosition > d4)
			{
				clippedSegment2 = true;
				flag2 = false;
				firstPoint.yPosition = (double)d4;
			}
			if ((decimal)secondPoint.yPosition < d2)
			{
				secondPoint.yPosition = (double)d2;
			}
			else if ((decimal)secondPoint.yPosition > d4)
			{
				clippedSegment3 = true;
				secondPoint.yPosition = (double)d4;
			}
			int num3 = 0;
			while (num3 < 3)
			{
				GraphicsPath graphicsPath = null;
				if (num3 == 0 && !reversed)
				{
					goto IL_0824;
				}
				if (num3 == 2 && reversed)
				{
					goto IL_0824;
				}
				goto IL_08c4;
				IL_0824:
				if (dataPoint3D2 == null)
				{
					dataPoint3D2 = dataPoint3D;
				}
				dataPoint3D2.dataPoint = secondPoint.dataPoint;
				dataPoint3D2.index = secondPoint.index;
				dataPoint3D2.xCenterVal = secondPoint.xCenterVal;
				graphicsPath = this.Draw3DSurface(firstPoint, dataPoint3D2, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, (LineSegmentType)((surfaceSegmentType != LineSegmentType.Middle) ? 1 : 2), (float)((flag2 && num2 != 3) ? 0.0 : 0.5), 0f, new PointF(float.NaN, float.NaN), new PointF((float)dataPoint3D2.xPosition, float.NaN), clippedSegment2, false, true);
				goto IL_08c4;
				IL_0998:
				dataPoint3D.dataPoint = firstPoint.dataPoint;
				dataPoint3D.index = firstPoint.index;
				dataPoint3D.xCenterVal = firstPoint.xCenterVal;
				graphicsPath = this.Draw3DSurface(dataPoint3D, secondPoint, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, (LineSegmentType)((surfaceSegmentType == LineSegmentType.Middle) ? 2 : 3), (float)((!flag2 && num2 != 3) ? 0.0 : 0.5), 0f, new PointF((float)dataPoint3D.xPosition, float.NaN), new PointF(float.NaN, float.NaN), clippedSegment3, false, true);
				goto IL_0a30;
				IL_0a30:
				if (resultPath != null && graphicsPath != null && graphicsPath.PointCount > 0)
				{
					resultPath.AddPath(graphicsPath, true);
				}
				num3++;
				continue;
				IL_08c4:
				if (num3 == 1 && dataPoint3D2 != null && num2 == 3)
				{
					dataPoint3D2.dataPoint = secondPoint.dataPoint;
					dataPoint3D2.index = secondPoint.index;
					dataPoint3D2.xCenterVal = secondPoint.xCenterVal;
					dataPoint3D.xCenterVal = firstPoint.xCenterVal;
					dataPoint3D.index = firstPoint.index;
					dataPoint3D.dataPoint = firstPoint.dataPoint;
					graphicsPath = this.Draw3DSurface(dataPoint3D, dataPoint3D2, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, LineSegmentType.Middle, topDarkening, bottomDarkening, new PointF((float)dataPoint3D.xPosition, float.NaN), new PointF((float)dataPoint3D2.xPosition, float.NaN), false, false, true);
				}
				if (num3 == 2 && !reversed)
				{
					goto IL_0998;
				}
				if (num3 == 0 && reversed)
				{
					goto IL_0998;
				}
				goto IL_0a30;
			}
			firstPoint.yPosition = yPosition;
			secondPoint.yPosition = yPosition2;
			return true;
		}

		protected bool ClipBottomPoints(GraphicsPath resultPath, ref DataPoint3D firstPoint, ref DataPoint3D secondPoint, ref PointF thirdPoint, ref PointF fourthPoint, bool reversed, ChartArea area, ChartGraphics graph, Matrix3D matrix, LightStyle lightStyle, DataPoint3D prevDataPointEx, float positionZ, float depth, ArrayList points, int pointIndex, int pointLoopIndex, float tension, DrawingOperationTypes operationType, LineSegmentType surfaceSegmentType, float topDarkening, float bottomDarkening)
		{
			area.IterationCounter++;
			if (area.IterationCounter > 20)
			{
				area.IterationCounter = 0;
				return true;
			}
			int num = 3;
			decimal d = Math.Round((decimal)area.PlotAreaPosition.X, num);
			decimal d2 = Math.Round((decimal)area.PlotAreaPosition.Y, num);
			decimal d3 = Math.Round((decimal)area.PlotAreaPosition.Right(), num);
			decimal d4 = Math.Round((decimal)area.PlotAreaPosition.Bottom(), num);
			d -= 0.001m;
			d2 -= 0.001m;
			d3 += 0.001m;
			d4 += 0.001m;
			firstPoint.xPosition = Math.Round(firstPoint.xPosition, num);
			firstPoint.yPosition = Math.Round(firstPoint.yPosition, num);
			secondPoint.xPosition = Math.Round(secondPoint.xPosition, num);
			secondPoint.yPosition = Math.Round(secondPoint.yPosition, num);
			thirdPoint.X = (float)Math.Round((double)thirdPoint.X, num);
			thirdPoint.Y = (float)Math.Round((double)thirdPoint.Y, num);
			fourthPoint.X = (float)Math.Round((double)fourthPoint.X, num);
			fourthPoint.Y = (float)Math.Round((double)fourthPoint.Y, num);
			if (!((decimal)thirdPoint.Y < d2) && !((decimal)thirdPoint.Y > d4) && !((decimal)fourthPoint.Y < d2) && !((decimal)fourthPoint.Y > d4))
			{
				return false;
			}
			PointF pointF = new PointF(thirdPoint.X, thirdPoint.Y);
			PointF pointF2 = new PointF(fourthPoint.X, fourthPoint.Y);
			bool flag = false;
			bool clippedSegment = false;
			if ((decimal)thirdPoint.Y < d2 && (decimal)fourthPoint.Y < d2)
			{
				clippedSegment = true;
				flag = true;
				thirdPoint.Y = area.PlotAreaPosition.Y;
				fourthPoint.Y = area.PlotAreaPosition.Y;
			}
			if ((decimal)thirdPoint.Y > d4 && (decimal)fourthPoint.Y > d4)
			{
				flag = true;
				thirdPoint.Y = area.PlotAreaPosition.Bottom();
				fourthPoint.Y = area.PlotAreaPosition.Bottom();
			}
			if (flag)
			{
				resultPath = this.Draw3DSurface(firstPoint, secondPoint, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, surfaceSegmentType, topDarkening, 0.5f, new PointF(thirdPoint.X, thirdPoint.Y), new PointF(fourthPoint.X, fourthPoint.Y), clippedSegment, false, false);
				thirdPoint = new PointF(pointF.X, pointF.Y);
				fourthPoint = new PointF(pointF2.X, pointF2.Y);
				return true;
			}
			DataPoint3D dataPoint3D = new DataPoint3D();
			bool flag2 = false;
			dataPoint3D.yPosition = (double)d2;
			if ((decimal)thirdPoint.Y > d4 || (decimal)fourthPoint.Y > d4)
			{
				dataPoint3D.yPosition = (double)area.PlotAreaPosition.Bottom();
				flag2 = true;
			}
			dataPoint3D.xPosition = (dataPoint3D.yPosition - (double)fourthPoint.Y) * (double)(thirdPoint.X - fourthPoint.X) / (double)(thirdPoint.Y - fourthPoint.Y) + (double)fourthPoint.X;
			dataPoint3D.yPosition = (dataPoint3D.xPosition - secondPoint.xPosition) / (firstPoint.xPosition - secondPoint.xPosition) * (firstPoint.yPosition - secondPoint.yPosition) + secondPoint.yPosition;
			int num2;
			DataPoint3D dataPoint3D2;
			bool flag3;
			if (!double.IsNaN(dataPoint3D.xPosition) && !double.IsInfinity(dataPoint3D.xPosition) && !double.IsNaN(dataPoint3D.yPosition) && !double.IsInfinity(dataPoint3D.yPosition))
			{
				num2 = 2;
				dataPoint3D2 = null;
				flag3 = false;
				if ((decimal)thirdPoint.Y < d2 && (decimal)fourthPoint.Y > d4)
				{
					goto IL_04bd;
				}
				if ((decimal)thirdPoint.Y > d4 && (decimal)fourthPoint.Y < d2)
				{
					goto IL_04bd;
				}
				goto IL_05c8;
			}
			return true;
			IL_05c8:
			bool flag4 = true;
			float bottomDarkening2 = bottomDarkening;
			bool clippedSegment2 = false;
			bool flag5 = false;
			if ((decimal)thirdPoint.Y < d2)
			{
				clippedSegment2 = true;
				flag4 = false;
				thirdPoint.Y = area.PlotAreaPosition.Y;
				bottomDarkening2 = 0.5f;
			}
			else if ((decimal)thirdPoint.Y > d4)
			{
				flag4 = false;
				thirdPoint.Y = area.PlotAreaPosition.Bottom();
				if (firstPoint.yPosition >= (double)thirdPoint.Y)
				{
					bottomDarkening2 = 0.5f;
				}
			}
			if ((decimal)fourthPoint.Y < d2)
			{
				flag5 = true;
				fourthPoint.Y = area.PlotAreaPosition.Y;
				bottomDarkening2 = 0.5f;
			}
			else if ((decimal)fourthPoint.Y > d4)
			{
				fourthPoint.Y = area.PlotAreaPosition.Bottom();
				if ((double)fourthPoint.Y <= secondPoint.yPosition)
				{
					bottomDarkening2 = 0.5f;
				}
			}
			int num3 = 0;
			while (num3 < 3)
			{
				GraphicsPath graphicsPath = null;
				if (num3 == 0 && !reversed)
				{
					goto IL_06e7;
				}
				if (num3 == 2 && reversed)
				{
					goto IL_06e7;
				}
				goto IL_0845;
				IL_06e7:
				if (dataPoint3D2 == null)
				{
					dataPoint3D2 = dataPoint3D;
				}
				if (flag3)
				{
					DataPoint3D dataPoint3D3 = new DataPoint3D();
					dataPoint3D3.xPosition = dataPoint3D.xPosition;
					dataPoint3D3.yPosition = dataPoint3D.yPosition;
					dataPoint3D.xPosition = dataPoint3D2.xPosition;
					dataPoint3D.yPosition = dataPoint3D2.yPosition;
					dataPoint3D2.xPosition = dataPoint3D3.xPosition;
					dataPoint3D2.yPosition = dataPoint3D3.yPosition;
				}
				dataPoint3D2.dataPoint = secondPoint.dataPoint;
				dataPoint3D2.index = secondPoint.index;
				dataPoint3D2.xCenterVal = secondPoint.xCenterVal;
				graphicsPath = this.Draw3DSurface(firstPoint, dataPoint3D2, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, (LineSegmentType)((surfaceSegmentType != LineSegmentType.Middle) ? 1 : 2), topDarkening, bottomDarkening2, new PointF(float.NaN, thirdPoint.Y), new PointF((float)dataPoint3D2.xPosition, (!flag4 || num2 == 3) ? thirdPoint.Y : fourthPoint.Y), clippedSegment2, false, false);
				if (flag3)
				{
					DataPoint3D dataPoint3D4 = new DataPoint3D();
					dataPoint3D4.xPosition = dataPoint3D.xPosition;
					dataPoint3D4.yPosition = dataPoint3D.yPosition;
					dataPoint3D.xPosition = dataPoint3D2.xPosition;
					dataPoint3D.yPosition = dataPoint3D2.yPosition;
					dataPoint3D2.xPosition = dataPoint3D4.xPosition;
					dataPoint3D2.yPosition = dataPoint3D4.yPosition;
				}
				goto IL_0845;
				IL_09db:
				if (flag3)
				{
					DataPoint3D dataPoint3D5 = new DataPoint3D();
					dataPoint3D5.xPosition = dataPoint3D.xPosition;
					dataPoint3D5.yPosition = dataPoint3D.yPosition;
					dataPoint3D.xPosition = dataPoint3D2.xPosition;
					dataPoint3D.yPosition = dataPoint3D2.yPosition;
					dataPoint3D2.xPosition = dataPoint3D5.xPosition;
					dataPoint3D2.yPosition = dataPoint3D5.yPosition;
				}
				dataPoint3D.dataPoint = firstPoint.dataPoint;
				dataPoint3D.index = firstPoint.index;
				dataPoint3D.xCenterVal = firstPoint.xCenterVal;
				float y = (!flag4 || num2 == 3) ? thirdPoint.Y : fourthPoint.Y;
				if (num2 == 3)
				{
					y = (flag5 ? thirdPoint.Y : fourthPoint.Y);
				}
				graphicsPath = this.Draw3DSurface(dataPoint3D, secondPoint, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, (LineSegmentType)((surfaceSegmentType == LineSegmentType.Middle) ? 2 : 3), topDarkening, bottomDarkening2, new PointF((float)dataPoint3D.xPosition, y), new PointF(float.NaN, fourthPoint.Y), flag5, false, false);
				if (flag3)
				{
					DataPoint3D dataPoint3D6 = new DataPoint3D();
					dataPoint3D6.xPosition = dataPoint3D.xPosition;
					dataPoint3D6.yPosition = dataPoint3D.yPosition;
					dataPoint3D.xPosition = dataPoint3D2.xPosition;
					dataPoint3D.yPosition = dataPoint3D2.yPosition;
					dataPoint3D2.xPosition = dataPoint3D6.xPosition;
					dataPoint3D2.yPosition = dataPoint3D6.yPosition;
				}
				goto IL_0b50;
				IL_0b50:
				if (resultPath != null && graphicsPath != null && graphicsPath.PointCount > 0)
				{
					resultPath.AddPath(graphicsPath, true);
				}
				num3++;
				continue;
				IL_0845:
				if (num3 == 1 && dataPoint3D2 != null && num2 == 3)
				{
					if (!flag3)
					{
						DataPoint3D dataPoint3D7 = new DataPoint3D();
						dataPoint3D7.xPosition = dataPoint3D.xPosition;
						dataPoint3D7.yPosition = dataPoint3D.yPosition;
						dataPoint3D.xPosition = dataPoint3D2.xPosition;
						dataPoint3D.yPosition = dataPoint3D2.yPosition;
						dataPoint3D2.xPosition = dataPoint3D7.xPosition;
						dataPoint3D2.yPosition = dataPoint3D7.yPosition;
					}
					dataPoint3D2.dataPoint = secondPoint.dataPoint;
					dataPoint3D2.index = secondPoint.index;
					dataPoint3D2.xCenterVal = secondPoint.xCenterVal;
					dataPoint3D.xCenterVal = firstPoint.xCenterVal;
					dataPoint3D.index = firstPoint.index;
					dataPoint3D.dataPoint = firstPoint.dataPoint;
					graphicsPath = this.Draw3DSurface(dataPoint3D, dataPoint3D2, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, LineSegmentType.Middle, topDarkening, bottomDarkening, new PointF((float)dataPoint3D.xPosition, thirdPoint.Y), new PointF((float)dataPoint3D2.xPosition, fourthPoint.Y), false, false, false);
					if (!flag3)
					{
						DataPoint3D dataPoint3D8 = new DataPoint3D();
						dataPoint3D8.xPosition = dataPoint3D.xPosition;
						dataPoint3D8.yPosition = dataPoint3D.yPosition;
						dataPoint3D.xPosition = dataPoint3D2.xPosition;
						dataPoint3D.yPosition = dataPoint3D2.yPosition;
						dataPoint3D2.xPosition = dataPoint3D8.xPosition;
						dataPoint3D2.yPosition = dataPoint3D8.yPosition;
					}
				}
				if (num3 == 2 && !reversed)
				{
					goto IL_09db;
				}
				if (num3 == 0 && reversed)
				{
					goto IL_09db;
				}
				goto IL_0b50;
			}
			thirdPoint = new PointF(pointF.X, pointF.Y);
			fourthPoint = new PointF(pointF2.X, pointF2.Y);
			return true;
			IL_04bd:
			num2 = 3;
			dataPoint3D2 = new DataPoint3D();
			if (!flag2)
			{
				dataPoint3D2.yPosition = (double)area.PlotAreaPosition.Bottom();
			}
			else
			{
				dataPoint3D2.yPosition = (double)area.PlotAreaPosition.Y;
			}
			dataPoint3D2.xPosition = (dataPoint3D2.yPosition - (double)fourthPoint.Y) * (double)(thirdPoint.X - fourthPoint.X) / (double)(thirdPoint.Y - fourthPoint.Y) + (double)fourthPoint.X;
			dataPoint3D2.yPosition = (dataPoint3D2.xPosition - secondPoint.xPosition) / (firstPoint.xPosition - secondPoint.xPosition) * (firstPoint.yPosition - secondPoint.yPosition) + secondPoint.yPosition;
			if (!double.IsNaN(dataPoint3D2.xPosition) && !double.IsInfinity(dataPoint3D2.xPosition) && !double.IsNaN(dataPoint3D2.yPosition) && !double.IsInfinity(dataPoint3D2.yPosition))
			{
				if ((decimal)thirdPoint.Y > d4)
				{
					flag3 = true;
				}
				goto IL_05c8;
			}
			return true;
		}

		protected virtual GraphicsPath Draw3DSurface(DataPoint3D firstPoint, DataPoint3D secondPoint, bool reversed, ChartArea area, ChartGraphics graph, Matrix3D matrix, LightStyle lightStyle, DataPoint3D prevDataPointEx, float positionZ, float depth, ArrayList points, int pointIndex, int pointLoopIndex, float tension, DrawingOperationTypes operationType, LineSegmentType surfaceSegmentType, float topDarkening, float bottomDarkening, PointF thirdPointPosition, PointF fourthPointPosition, bool clippedSegment, bool clipOnTop, bool clipOnBottom)
		{
			return null;
		}
	}
}
