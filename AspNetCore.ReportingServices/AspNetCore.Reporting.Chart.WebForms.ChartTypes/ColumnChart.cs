using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class ColumnChart : PointChart
	{
		private double shiftedX;

		private string shiftedSerName = "";

		protected bool useTwoValues;

		protected bool drawSeriesSideBySide = true;

		protected COPCoordinates coordinates = COPCoordinates.X;

		public override string Name
		{
			get
			{
				return "Column";
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

		public override bool ExtraYValuesConnectedToYAxis
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

		public override bool ZeroCrossing
		{
			get
			{
				return true;
			}
		}

		public override double ShiftedX
		{
			get
			{
				return this.shiftedX;
			}
			set
			{
				this.shiftedX = value;
			}
		}

		public override string ShiftedSerName
		{
			get
			{
				return this.shiftedSerName;
			}
			set
			{
				this.shiftedSerName = value;
			}
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}

		public override LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Rectangle;
		}

		public ColumnChart()
			: base(false)
		{
		}

		public override void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			this.ProcessChartType(false, false, graph, common, area, seriesToDraw);
			this.ProcessChartType(true, false, graph, common, area, seriesToDraw);
		}

		private void ProcessChartType(bool labels, bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			if (area.Area3DStyle.Enable3D)
			{
				this.ProcessChartType3D(labels, selection, graph, common, area, seriesToDraw);
			}
			else
			{
				SizeF relativeSize = graph.GetRelativeSize(new SizeF(1.1f, 1.1f));
				ArrayList seriesFromChartType = area.GetSeriesFromChartType(this.Name);
				bool flag = this.drawSeriesSideBySide;
				foreach (string item in seriesFromChartType)
				{
					if (common.DataManager.Series[item].IsAttributeSet("DrawSideBySide"))
					{
						string strA = ((DataPointAttributes)common.DataManager.Series[item])["DrawSideBySide"];
						if (string.Compare(strA, "False", StringComparison.OrdinalIgnoreCase) == 0)
						{
							flag = false;
						}
						else if (string.Compare(strA, "True", StringComparison.OrdinalIgnoreCase) == 0)
						{
							flag = true;
						}
						else if (string.Compare(strA, "Auto", StringComparison.OrdinalIgnoreCase) != 0)
						{
							throw new InvalidOperationException(SR.ExceptionAttributeDrawSideBySideInvalid);
						}
					}
				}
				double num = (double)seriesFromChartType.Count;
				if (!flag)
				{
					num = 1.0;
				}
				bool flag2 = area.IndexedSeries((string[])area.GetSeriesFromChartType(this.Name).ToArray(typeof(string)));
				int num2 = 0;
				foreach (Series item2 in common.DataManager.Series)
				{
					if (string.Compare(item2.ChartTypeName, this.Name, true, CultureInfo.CurrentCulture) == 0 && !(item2.ChartArea != area.Name) && item2.Points.Count != 0 && item2.IsVisible())
					{
						this.ShiftedSerName = item2.Name;
						Axis axis = area.GetAxis(AxisName.Y, item2.YAxisType, item2.YSubAxisName);
						Axis axis2 = area.GetAxis(AxisName.X, item2.XAxisType, item2.XSubAxisName);
						double viewMaximum = axis2.GetViewMaximum();
						double viewMinimum = axis2.GetViewMinimum();
						double viewMaximum2 = axis.GetViewMaximum();
						double viewMinimum2 = axis.GetViewMinimum();
						double position = axis.GetPosition(axis.Crossing);
						bool flag3 = false;
						double interval = 1.0;
						if (!flag2)
						{
							if (item2.Points.Count == 1 && (item2.XValueType == ChartValueTypes.Date || item2.XValueType == ChartValueTypes.DateTime || item2.XValueType == ChartValueTypes.Time || item2.XValueType == ChartValueTypes.DateTimeOffset))
							{
								((ChartAreaAxes)area).GetPointsInterval(seriesFromChartType, axis2.Logarithmic, axis2.logarithmBase, true, out flag3);
								interval = ((double.IsNaN(axis2.majorGrid.Interval) || axis2.majorGrid.IntervalType == DateTimeIntervalType.NotSet) ? axis2.GetIntervalSize(axis2.minimum, axis2.Interval, axis2.IntervalType) : axis2.GetIntervalSize(axis2.minimum, axis2.majorGrid.Interval, axis2.majorGrid.IntervalType));
							}
							else
							{
								interval = ((ChartAreaAxes)area).GetPointsInterval(seriesFromChartType, axis2.Logarithmic, axis2.logarithmBase, true, out flag3);
							}
						}
						double num3 = item2.GetPointWidth(graph, axis2, interval, 0.8) / num;
						if (!selection)
						{
							common.EventsManager.OnBackPaint(item2, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
						}
						int num4 = 0;
						foreach (DataPoint point in item2.Points)
						{
							double num5 = axis.GetLogValue(this.GetYValue(common, area, item2, point, num4, this.useTwoValues ? 1 : 0));
							if (num5 > viewMaximum2)
							{
								num5 = viewMaximum2;
							}
							if (num5 < viewMinimum2)
							{
								num5 = viewMinimum2;
							}
							double num6 = axis.GetLinearPosition(num5);
							double num7 = 0.0;
							if (this.useTwoValues)
							{
								double num8 = axis.GetLogValue(this.GetYValue(common, area, item2, point, num4, 0));
								if (num8 > viewMaximum2)
								{
									num8 = viewMaximum2;
								}
								else if (num8 < viewMinimum2)
								{
									num8 = viewMinimum2;
								}
								num7 = axis.GetLinearPosition(num8);
							}
							else
							{
								num7 = position;
							}
							num4++;
							double num9;
							double position2;
							if (flag2)
							{
								num9 = axis2.GetPosition((double)num4) - num3 * num / 2.0 + num3 / 2.0 + (double)num2 * num3;
								position2 = axis2.GetPosition((double)num4);
							}
							else if (flag3)
							{
								num9 = axis2.GetPosition(point.XValue) - num3 * num / 2.0 + num3 / 2.0 + (double)num2 * num3;
								position2 = axis2.GetPosition(point.XValue);
							}
							else
							{
								num9 = axis2.GetPosition(point.XValue);
								position2 = axis2.GetPosition(point.XValue);
							}
							this.ShiftedX = num9 - position2;
							if (num6 < num7 && num7 - num6 < (double)relativeSize.Height)
							{
								num6 = num7 - (double)relativeSize.Height;
							}
							if (num6 > num7 && num6 - num7 < (double)relativeSize.Height)
							{
								num6 = num7 + (double)relativeSize.Height;
							}
							RectangleF empty = RectangleF.Empty;
							try
							{
								empty.X = (float)(num9 - num3 / 2.0);
								empty.Width = (float)num3;
								if (num7 < num6)
								{
									empty.Y = (float)num7;
									empty.Height = (float)num6 - empty.Y;
								}
								else
								{
									empty.Y = (float)num6;
									empty.Height = (float)num7 - empty.Y;
								}
							}
							catch (Exception)
							{
								continue;
							}
							if (!point.Empty)
							{
								if (common.ProcessModePaint)
								{
									if (!labels)
									{
										double yValue = flag2 ? ((double)num4) : point.XValue;
										yValue = axis2.GetLogValue(yValue);
										if (!(yValue < viewMinimum) && !(yValue > viewMaximum))
										{
											bool flag4 = false;
											if (empty.X < area.PlotAreaPosition.X || empty.Right > area.PlotAreaPosition.Right())
											{
												graph.SetClip(area.PlotAreaPosition.ToRectangleF());
												flag4 = true;
											}
											graph.StartHotRegion(point);
											graph.StartAnimation();
											this.DrawColumn2D(graph, axis, empty, point, item2);
											graph.StopAnimation();
											graph.EndHotRegion();
											if (flag4)
											{
												graph.ResetClip();
											}
											goto IL_066e;
										}
										continue;
									}
									if (this.useTwoValues)
									{
										this.DrawLabel(area, graph, common, empty, point, item2, num4);
									}
								}
								goto IL_066e;
							}
							continue;
							IL_066e:
							if (common.ProcessModeRegions && !labels)
							{
								common.HotRegionsList.AddHotRegion(graph, empty, point, item2.Name, num4 - 1);
							}
						}
						if (!selection)
						{
							common.EventsManager.OnPaint(item2, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
						}
						if (flag)
						{
							num2++;
						}
						if (labels && !this.useTwoValues)
						{
							base.ProcessChartType(false, graph, common, area, seriesToDraw);
						}
					}
				}
			}
		}

		protected virtual void DrawColumn2D(ChartGraphics graph, Axis vAxis, RectangleF rectSize, DataPoint point, Series ser)
		{
			graph.FillRectangleRel(rectSize, point.Color, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, point.BackGradientEndColor, point.BorderColor, point.BorderWidth, point.BorderStyle, ser.ShadowColor, ser.ShadowOffset, PenAlignment.Inset, ChartGraphics.GetBarDrawingStyle(point), true);
		}

		protected override LabelAlignmentTypes GetAutoLabelPosition(Series series, int pointIndex)
		{
			if (series.Points[pointIndex].YValues[0] >= 0.0)
			{
				return LabelAlignmentTypes.Top;
			}
			return LabelAlignmentTypes.Bottom;
		}

		protected override bool ShouldDrawMarkerOnViewEdgeX()
		{
			return false;
		}

		private void ProcessChartType3D(bool labels, bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			if (labels && !selection)
			{
				return;
			}
			SizeF relativeSize = graph.GetRelativeSize(new SizeF(1.1f, 1.1f));
			ArrayList arrayList = null;
			bool flag = this.drawSeriesSideBySide;
			if (area.Area3DStyle.Clustered && this.SideBySideSeries)
			{
				goto IL_0047;
			}
			if (this.Stacked)
			{
				goto IL_0047;
			}
			arrayList = new ArrayList();
			arrayList.Add(seriesToDraw.Name);
			goto IL_0122;
			IL_0047:
			arrayList = area.GetSeriesFromChartType(this.Name);
			foreach (string item in arrayList)
			{
				if (common.DataManager.Series[item].IsAttributeSet("DrawSideBySide"))
				{
					string strA = ((DataPointAttributes)common.DataManager.Series[item])["DrawSideBySide"];
					if (string.Compare(strA, "False", StringComparison.OrdinalIgnoreCase) == 0)
					{
						flag = false;
					}
					else if (string.Compare(strA, "True", StringComparison.OrdinalIgnoreCase) == 0)
					{
						flag = true;
					}
					else if (string.Compare(strA, "Auto", StringComparison.OrdinalIgnoreCase) != 0)
					{
						throw new InvalidOperationException(SR.ExceptionAttributeDrawSideBySideInvalid);
					}
				}
			}
			goto IL_0122;
			IL_0122:
			ArrayList dataPointDrawingOrder = area.GetDataPointDrawingOrder(arrayList, this, selection, this.coordinates, null, base.yValueIndex, flag);
			foreach (object item2 in dataPointDrawingOrder)
			{
				DataPoint3D dataPoint3D = (DataPoint3D)item2;
				DataPoint dataPoint = dataPoint3D.dataPoint;
				Series series = dataPoint.series;
				BarDrawingStyle barDrawingStyle = ChartGraphics.GetBarDrawingStyle(dataPoint);
				Axis axis = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName);
				Axis axis2 = area.GetAxis(AxisName.X, series.XAxisType, series.XSubAxisName);
				float num = 0f;
				float num2 = 0f;
				double yValue = this.GetYValue(common, area, series, dataPoint3D.dataPoint, dataPoint3D.index - 1, this.useTwoValues ? 1 : 0);
				yValue = axis.GetLogValue(yValue);
				if (yValue > axis.GetViewMaximum())
				{
					num = 0.5f;
					yValue = axis.GetViewMaximum();
				}
				if (yValue < axis.GetViewMinimum())
				{
					num = 0.5f;
					yValue = axis.GetViewMinimum();
				}
				double num3 = axis.GetLinearPosition(yValue);
				double num4 = 0.0;
				if (this.useTwoValues)
				{
					double num5 = axis.GetLogValue(this.GetYValue(common, area, series, dataPoint, dataPoint3D.index - 1, 0));
					if (num5 > axis.GetViewMaximum())
					{
						num2 = 0.5f;
						num5 = axis.GetViewMaximum();
					}
					else if (num5 < axis.GetViewMinimum())
					{
						num2 = 0.5f;
						num5 = axis.GetViewMinimum();
					}
					num4 = axis.GetLinearPosition(num5);
				}
				else
				{
					num4 = axis.GetPosition(axis.Crossing);
				}
				if (!flag)
				{
					dataPoint3D.xPosition = dataPoint3D.xCenterVal;
				}
				this.ShiftedX = dataPoint3D.xPosition - dataPoint3D.xCenterVal;
				if (num3 < num4 && num4 - num3 < (double)relativeSize.Height)
				{
					num3 = num4 - (double)relativeSize.Height;
				}
				if (num3 > num4 && num3 - num4 < (double)relativeSize.Height)
				{
					num3 = num4 + (double)relativeSize.Height;
				}
				RectangleF empty = RectangleF.Empty;
				try
				{
					empty.X = (float)(dataPoint3D.xPosition - dataPoint3D.width / 2.0);
					empty.Width = (float)dataPoint3D.width;
					if (num4 < num3)
					{
						float num6 = num2;
						num2 = num;
						num = num6;
						empty.Y = (float)num4;
						empty.Height = (float)num3 - empty.Y;
					}
					else
					{
						empty.Y = (float)num3;
						empty.Height = (float)num4 - empty.Y;
					}
				}
				catch (Exception)
				{
					continue;
				}
				GraphicsPath graphicsPath = null;
				double yValue2 = dataPoint3D.indexedSeries ? ((double)dataPoint3D.index) : dataPoint.XValue;
				yValue2 = axis2.GetLogValue(yValue2);
				if (!(yValue2 < axis2.GetViewMinimum()) && !(yValue2 > axis2.GetViewMaximum()))
				{
					bool flag2 = false;
					if (!(empty.Right <= area.PlotAreaPosition.X) && !(empty.X >= area.PlotAreaPosition.Right()))
					{
						if (empty.X < area.PlotAreaPosition.X)
						{
							empty.Width -= area.PlotAreaPosition.X - empty.X;
							empty.X = area.PlotAreaPosition.X;
						}
						if (empty.Right > area.PlotAreaPosition.Right())
						{
							empty.Width -= empty.Right - area.PlotAreaPosition.Right();
						}
						if (empty.Width < 0.0)
						{
							empty.Width = 0f;
						}
						DrawingOperationTypes drawingOperationTypes = DrawingOperationTypes.DrawElement;
						if (common.ProcessModeRegions)
						{
							drawingOperationTypes |= DrawingOperationTypes.CalcElementPath;
						}
						if (!dataPoint.Empty && empty.Height > 0.0 && empty.Width > 0.0)
						{
							graph.StartHotRegion(dataPoint);
							this.Init3DAnimation(common, empty, dataPoint3D.zPosition, dataPoint3D.depth, area.matrix3D, graph, yValue < axis2.Crossing, dataPoint);
							graph.StartAnimation();
							graphicsPath = graph.Fill3DRectangle(empty, dataPoint3D.zPosition, dataPoint3D.depth, area.matrix3D, area.Area3DStyle.Light, dataPoint.Color, num, num2, dataPoint.BackHatchStyle, dataPoint.BackImage, dataPoint.BackImageMode, dataPoint.BackImageTransparentColor, dataPoint.BackImageAlign, dataPoint.BackGradientType, dataPoint.BackGradientEndColor, dataPoint.BorderColor, dataPoint.BorderWidth, dataPoint.BorderStyle, PenAlignment.Inset, barDrawingStyle, true, drawingOperationTypes);
							graph.StopAnimation();
							graph.EndHotRegion();
							if (common.ProcessModeRegions && !labels)
							{
								common.HotRegionsList.AddHotRegion(graphicsPath, false, graph, dataPoint, series.Name, dataPoint3D.index - 1);
							}
						}
						if (flag2)
						{
							graph.ResetClip();
						}
						this.ProcessSinglePoint3D(dataPoint3D, selection, graph, common, area, empty, dataPoint3D.index - 1);
					}
				}
			}
			base.DrawAccumulated3DLabels(graph, common, area);
		}

		private void Init3DAnimation(CommonElements common, RectangleF position, float positionZ, float depth, Matrix3D matrix, ChartGraphics graph, bool negativeValue, DataPoint point)
		{
		}

		protected virtual void DrawLabel(ChartArea area, ChartGraphics graph, CommonElements common, RectangleF columnPosition, DataPoint point, Series ser, int pointIndex)
		{
		}

		protected virtual void ProcessSinglePoint3D(DataPoint3D pointEx, bool selection, ChartGraphics graph, CommonElements common, ChartArea area, RectangleF columnPosition, int pointIndex)
		{
			base.ProcessSinglePoint3D(pointEx, selection, graph, common, area);
		}
	}
}
