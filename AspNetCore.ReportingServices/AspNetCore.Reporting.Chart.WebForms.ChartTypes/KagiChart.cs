using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class KagiChart : StepLineChart
	{
		internal bool kagiChart;

		internal Color kagiUpColor = Color.Empty;

		internal int currentKagiDirection;

		public override string Name
		{
			get
			{
				return "Kagi";
			}
		}

		internal static void PrepareData(Series series, IServiceContainer serviceContainer)
		{
			if (string.Compare(series.ChartTypeName, "Kagi", StringComparison.OrdinalIgnoreCase) == 0 && series.IsVisible())
			{
				Chart chart = (Chart)serviceContainer.GetService(typeof(Chart));
				if (chart == null)
				{
					throw new InvalidOperationException(SR.ExceptionKagiNullReference);
				}
				ChartArea chartArea = chart.ChartAreas[series.ChartArea];
				foreach (Series item in chart.Series)
				{
					if (item.IsVisible() && item != series && chartArea == chart.ChartAreas[item.ChartArea])
					{
						throw new InvalidOperationException(SR.ExceptionKagiCanNotCombine);
					}
				}
				string name = "KAGI_ORIGINAL_DATA_" + series.Name;
				if (chart.Series.GetIndex(name) == -1)
				{
					Series series3 = new Series(name, series.YValuesPerPoint);
					series3.Enabled = false;
					series3.ShowInLegend = false;
					chart.Series.Add(series3);
					foreach (DataPoint point in series.Points)
					{
						series3.Points.Add(point);
					}
					series.Points.Clear();
					if (series.IsAttributeSet("TempDesignData"))
					{
						((DataPointAttributes)series3)["TempDesignData"] = "true";
					}
					((DataPointAttributes)series)["OldXValueIndexed"] = series.XValueIndexed.ToString(CultureInfo.InvariantCulture);
					((DataPointAttributes)series)["OldYValuesPerPoint"] = series.YValuesPerPoint.ToString(CultureInfo.InvariantCulture);
					series.XValueIndexed = true;
					if (series.ChartArea.Length > 0 && series.IsXValueDateTime())
					{
						Axis axis = chartArea.GetAxis(AxisName.X, series.XAxisType, series.XSubAxisName);
						if (axis.Interval == 0.0 && axis.IntervalType == DateTimeIntervalType.Auto)
						{
							bool flag = false;
							double num = 1.7976931348623157E+308;
							double num2 = -1.7976931348623157E+308;
							foreach (DataPoint point2 in series3.Points)
							{
								if (!point2.Empty)
								{
									if (point2.XValue != 0.0)
									{
										flag = true;
									}
									if (point2.XValue > num2)
									{
										num2 = point2.XValue;
									}
									if (point2.XValue < num)
									{
										num = point2.XValue;
									}
								}
							}
							if (flag)
							{
								((DataPointAttributes)series)["OldAutomaticXAxisInterval"] = "true";
								DateTimeIntervalType intervalType = DateTimeIntervalType.Auto;
								axis.interval = ((AxisScale)axis).CalcInterval(num, num2, true, out intervalType, series.XValueType);
								axis.intervalType = intervalType;
							}
						}
					}
					KagiChart.FillKagiData(series, series3);
				}
			}
		}

		internal static bool UnPrepareData(Series series, IServiceContainer serviceContainer)
		{
			if (series.Name.StartsWith("KAGI_ORIGINAL_DATA_", StringComparison.Ordinal))
			{
				Chart chart = (Chart)serviceContainer.GetService(typeof(Chart));
				if (chart == null)
				{
					throw new InvalidOperationException(SR.ExceptionKagiNullReference);
				}
				Series series2 = chart.Series[series.Name.Substring(19)];
				series2.Points.Clear();
				if (!series.IsAttributeSet("TempDesignData"))
				{
					foreach (DataPoint point in series.Points)
					{
						series2.Points.Add(point);
					}
				}
				try
				{
					series2.XValueIndexed = bool.Parse(((DataPointAttributes)series2)["OldXValueIndexed"]);
					series2.YValuesPerPoint = int.Parse(((DataPointAttributes)series2)["OldYValuesPerPoint"], CultureInfo.InvariantCulture);
				}
				catch
				{
				}
				series2.DeleteAttribute("OldXValueIndexed");
				series2.DeleteAttribute("OldYValuesPerPoint");
				((DataPointAttributes)series)["OldAutomaticXAxisInterval"] = "true";
				if (series2.IsAttributeSet("OldAutomaticXAxisInterval"))
				{
					series2.DeleteAttribute("OldAutomaticXAxisInterval");
					if (series2.ChartArea.Length > 0)
					{
						ChartArea chartArea = chart.ChartAreas[series2.ChartArea];
						Axis axis = chartArea.GetAxis(AxisName.X, series2.XAxisType, series2.XSubAxisName);
						axis.interval = 0.0;
						axis.intervalType = DateTimeIntervalType.Auto;
					}
				}
				chart.Series.Remove(series);
				return true;
			}
			return false;
		}

		private static double GetReversalAmount(Series series, Series originalData, int yValueIndex, out double percentOfPrice)
		{
			double result = 1.0;
			percentOfPrice = 3.0;
			if (series.IsAttributeSet("ReversalAmount"))
			{
				string text = ((DataPointAttributes)series)["ReversalAmount"].Trim();
				bool flag = text.EndsWith("%", StringComparison.Ordinal);
				if (flag)
				{
					text = text.Substring(0, text.Length - 1);
				}
				try
				{
					if (flag)
					{
						percentOfPrice = double.Parse(text, CultureInfo.InvariantCulture);
						return result;
					}
					result = double.Parse(text, CultureInfo.InvariantCulture);
					percentOfPrice = 0.0;
					return result;
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionKagiAttributeFormatInvalid("ReversalAmount"));
				}
			}
			return result;
		}

		private static void FillKagiData(Series series, Series originalData)
		{
			int num = 0;
			if (series.IsAttributeSet("UsedYValue"))
			{
				try
				{
					num = int.Parse(((DataPointAttributes)series)["UsedYValue"], CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionKagiAttributeFormatInvalid("UsedYValue"));
				}
				if (num >= series.YValuesPerPoint)
				{
					throw new InvalidOperationException(SR.ExceptionKagiAttributeOutOfRange("UsedYValue"));
				}
			}
			double num2 = 0.0;
			double num3 = KagiChart.GetReversalAmount(series, originalData, num, out num2);
			double num4 = double.NaN;
			int num5 = 0;
			int num6 = 0;
			foreach (DataPoint point in originalData.Points)
			{
				if (double.IsNaN(num4))
				{
					num4 = point.YValues[num];
					DataPoint dataPoint2 = point.Clone();
					dataPoint2.series = series;
					dataPoint2.XValue = point.XValue;
					dataPoint2.YValues[0] = point.YValues[num];
					series.Points.Add(dataPoint2);
					num6++;
				}
				else
				{
					if (num2 != 0.0)
					{
						num3 = num4 / 100.0 * num2;
					}
					int num7 = 0;
					num7 = ((point.YValues[num] > num4) ? 1 : ((point.YValues[num] < num4) ? (-1) : 0));
					if (num7 != 0)
					{
						if (num7 == num5)
						{
							series.Points[series.Points.Count - 1].YValues[0] = point.YValues[num];
							((DataPointAttributes)series.Points[series.Points.Count - 1])["OriginalPointIndex"] = num6.ToString(CultureInfo.InvariantCulture);
						}
						else
						{
							if (Math.Abs(point.YValues[num] - num4) < num3)
							{
								num6++;
								continue;
							}
							DataPoint dataPoint3 = point.Clone();
							((DataPointAttributes)dataPoint3)["OriginalPointIndex"] = num6.ToString(CultureInfo.InvariantCulture);
							dataPoint3.series = series;
							dataPoint3.XValue = point.XValue;
							dataPoint3.YValues[0] = point.YValues[num];
							series.Points.Add(dataPoint3);
						}
						num4 = point.YValues[num];
						num5 = num7;
					}
					num6++;
				}
			}
		}

		protected override void DrawLine(ChartGraphics graph, CommonElements common, DataPoint point, Series series, PointF[] points, int pointIndex, float tension)
		{
			if (pointIndex > 0)
			{
				if (this.currentKagiDirection == 0)
				{
					this.kagiUpColor = ChartGraphics.GetGradientColor(series.Color, Color.Black, 0.5);
					string text = ((DataPointAttributes)series)["PriceUpColor"];
					ColorConverter colorConverter = new ColorConverter();
					if (text != null)
					{
						try
						{
							this.kagiUpColor = (Color)colorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, text);
						}
						catch
						{
							throw new InvalidOperationException(SR.ExceptionKagiAttributeFormatInvalid("Up Brick color"));
						}
					}
					this.currentKagiDirection = ((points[pointIndex - 1].Y > points[pointIndex].Y) ? 1 : (-1));
				}
				Color color = (this.currentKagiDirection == 1) ? this.kagiUpColor : point.Color;
				PointF pointF = points[pointIndex - 1];
				PointF pointF2 = new PointF(points[pointIndex].X, points[pointIndex - 1].Y);
				PointF pointF3 = points[pointIndex];
				PointF empty = PointF.Empty;
				if (pointIndex >= 2)
				{
					int num = (points[pointIndex - 1].Y > points[pointIndex].Y) ? 1 : (-1);
					if (num != this.currentKagiDirection)
					{
						PointF pointF4 = points[pointIndex - 2];
						bool flag = false;
						if (pointF.Y > pointF4.Y && pointF.Y > pointF3.Y && pointF4.Y > pointF3.Y)
						{
							flag = true;
						}
						else if (pointF.Y < pointF4.Y && pointF.Y < pointF3.Y && pointF4.Y < pointF3.Y)
						{
							flag = true;
						}
						if (flag)
						{
							empty.Y = pointF4.Y;
							empty.X = pointF2.X;
						}
					}
				}
				pointF.X = (float)Math.Round((double)pointF.X);
				pointF.Y = (float)Math.Round((double)pointF.Y);
				pointF2.X = (float)Math.Round((double)pointF2.X);
				pointF2.Y = (float)Math.Round((double)pointF2.Y);
				pointF3.X = (float)Math.Round((double)pointF3.X);
				pointF3.Y = (float)Math.Round((double)pointF3.Y);
				if (!empty.IsEmpty)
				{
					empty.X = (float)Math.Round((double)empty.X);
					empty.Y = (float)Math.Round((double)empty.Y);
				}
				graph.DrawLineRel(color, point.BorderWidth, point.BorderStyle, graph.GetRelativePoint(pointF), graph.GetRelativePoint(pointF2), series.ShadowColor, series.ShadowOffset);
				if (empty.IsEmpty)
				{
					graph.DrawLineRel(color, point.BorderWidth, point.BorderStyle, graph.GetRelativePoint(pointF2), graph.GetRelativePoint(pointF3), series.ShadowColor, series.ShadowOffset);
				}
				else
				{
					graph.DrawLineRel(color, point.BorderWidth, point.BorderStyle, graph.GetRelativePoint(pointF2), graph.GetRelativePoint(empty), series.ShadowColor, series.ShadowOffset);
					this.currentKagiDirection = ((this.currentKagiDirection != 1) ? 1 : (-1));
					color = ((this.currentKagiDirection == 1) ? this.kagiUpColor : point.Color);
					graph.DrawLineRel(color, point.BorderWidth, point.BorderStyle, graph.GetRelativePoint(empty), graph.GetRelativePoint(pointF3), series.ShadowColor, series.ShadowOffset);
				}
				if (common.ProcessModeRegions)
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					graphicsPath.AddLine(pointF, pointF2);
					graphicsPath.AddLine(pointF2, pointF3);
					ChartGraphics.Widen(graphicsPath, new Pen(point.Color, (float)(point.BorderWidth + 2)));
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

		protected override PointF[] GetPointsPosition(ChartGraphics graph, Series series, bool indexedSeries)
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
				array[num] = new PointF((float)(position2 * (double)(graph.common.ChartPicture.Width - 1) / 100.0), (float)(position * (double)(graph.common.ChartPicture.Height - 1) / 100.0));
				num++;
			}
			return array;
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
			DataPoint3D dataPoint3D3 = dataPoint3D;
			if (prevDataPointEx.dataPoint.Empty)
			{
				dataPoint3D3 = prevDataPointEx;
			}
			else if (dataPoint3D2.index > dataPoint3D.index)
			{
				dataPoint3D3 = dataPoint3D2;
			}
			Color color = base.useBorderColor ? dataPoint3D3.dataPoint.BorderColor : dataPoint3D3.dataPoint.Color;
			ChartDashStyle borderStyle = dataPoint3D3.dataPoint.BorderStyle;
			if (dataPoint3D3.dataPoint.Empty && dataPoint3D3.dataPoint.Color == Color.Empty)
			{
				color = Color.Gray;
			}
			if (dataPoint3D3.dataPoint.Empty && dataPoint3D3.dataPoint.BorderStyle == ChartDashStyle.NotSet)
			{
				borderStyle = ChartDashStyle.Solid;
			}
			if (this.currentKagiDirection == 0)
			{
				this.kagiUpColor = dataPoint3D.dataPoint.series.Color;
				string text = ((DataPointAttributes)dataPoint3D.dataPoint.series)["PriceUpColor"];
				ColorConverter colorConverter = new ColorConverter();
				if (text != null)
				{
					try
					{
						this.kagiUpColor = (Color)colorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, text);
					}
					catch
					{
						throw new InvalidOperationException(SR.ExceptionKagiAttributeFormatInvalid("Up Brick color"));
					}
				}
				this.currentKagiDirection = ((dataPoint3D2.yPosition > dataPoint3D.yPosition) ? 1 : (-1));
			}
			Color backColor = (this.currentKagiDirection == 1) ? this.kagiUpColor : color;
			DataPoint3D dataPoint3D4 = new DataPoint3D();
			dataPoint3D4.xPosition = dataPoint3D.xPosition;
			dataPoint3D4.yPosition = dataPoint3D2.yPosition;
			bool flag = true;
			if (pointIndex + 1 < points.Count)
			{
				DataPoint3D dataPoint3D5 = (DataPoint3D)points[pointIndex + 1];
				if (dataPoint3D5.index == dataPoint3D2.index)
				{
					flag = false;
				}
			}
			if (base.centerPointIndex != 2147483647 && pointIndex >= base.centerPointIndex)
			{
				flag = false;
			}
			DataPoint3D dataPoint3D6 = null;
			if (dataPoint3D.index >= 2)
			{
				int num2 = (dataPoint3D2.yPosition > dataPoint3D.yPosition) ? 1 : (-1);
				if (num2 != this.currentKagiDirection)
				{
					DataPoint3D dataPoint3D7 = ChartGraphics3D.FindPointByIndex(points, dataPoint3D.index - 2, base.multiSeries ? dataPoint3D : null, ref num);
					bool flag2 = false;
					if (dataPoint3D2.yPosition > dataPoint3D7.yPosition && dataPoint3D2.yPosition > dataPoint3D.yPosition && dataPoint3D7.yPosition > dataPoint3D.yPosition)
					{
						flag2 = true;
					}
					else if (dataPoint3D2.yPosition < dataPoint3D7.yPosition && dataPoint3D2.yPosition < dataPoint3D.yPosition && dataPoint3D7.yPosition < dataPoint3D.yPosition)
					{
						flag2 = true;
					}
					if (flag2)
					{
						dataPoint3D6 = new DataPoint3D();
						dataPoint3D6.xPosition = dataPoint3D.xPosition;
						dataPoint3D6.yPosition = dataPoint3D7.yPosition;
						dataPoint3D6.dataPoint = dataPoint3D.dataPoint;
					}
				}
			}
			GraphicsPath[] array = new GraphicsPath[3];
			for (int i = 0; i < 2; i++)
			{
				DataPoint3D firstPoint = dataPoint3D2;
				DataPoint3D secondPoint = dataPoint3D;
				LineSegmentType lineSegmentType = LineSegmentType.First;
				switch (i)
				{
				case 0:
					lineSegmentType = (LineSegmentType)(flag ? 1 : 3);
					dataPoint3D4.dataPoint = (flag ? dataPoint3D.dataPoint : dataPoint3D2.dataPoint);
					firstPoint = (flag ? dataPoint3D2 : dataPoint3D4);
					secondPoint = (flag ? dataPoint3D4 : dataPoint3D);
					break;
				case 1:
					lineSegmentType = (LineSegmentType)((!flag) ? 1 : 3);
					dataPoint3D4.dataPoint = ((!flag) ? dataPoint3D.dataPoint : dataPoint3D.dataPoint);
					firstPoint = ((!flag) ? dataPoint3D2 : dataPoint3D4);
					secondPoint = ((!flag) ? dataPoint3D4 : dataPoint3D);
					break;
				}
				if (lineSegmentType == LineSegmentType.First || dataPoint3D6 == null)
				{
					array[i] = new GraphicsPath();
					array[i] = graph.Draw3DSurface(area, matrix, lightStyle, SurfaceNames.Top, positionZ, depth, backColor, dataPoint3D3.dataPoint.BorderColor, dataPoint3D3.dataPoint.BorderWidth, borderStyle, firstPoint, secondPoint, points, pointIndex, 0f, operationType, lineSegmentType, (byte)(base.showPointLines ? 1 : 0) != 0, false, area.reverseSeriesOrder, base.multiSeries, 0, true);
				}
				else
				{
					if (!flag)
					{
						backColor = ((this.currentKagiDirection == -1) ? this.kagiUpColor : color);
					}
					array[i] = new GraphicsPath();
					array[i] = graph.Draw3DSurface(area, matrix, lightStyle, SurfaceNames.Top, positionZ, depth, backColor, dataPoint3D3.dataPoint.BorderColor, dataPoint3D3.dataPoint.BorderWidth, borderStyle, firstPoint, dataPoint3D6, points, pointIndex, 0f, operationType, LineSegmentType.Middle, (byte)(base.showPointLines ? 1 : 0) != 0, false, area.reverseSeriesOrder, base.multiSeries, 0, true);
					graph.frontLinePen = null;
					this.currentKagiDirection = ((this.currentKagiDirection != 1) ? 1 : (-1));
					backColor = ((!flag) ? ((this.currentKagiDirection == -1) ? this.kagiUpColor : color) : ((this.currentKagiDirection == 1) ? this.kagiUpColor : color));
					array[2] = new GraphicsPath();
					array[2] = graph.Draw3DSurface(area, matrix, lightStyle, SurfaceNames.Top, positionZ, depth, backColor, dataPoint3D3.dataPoint.BorderColor, dataPoint3D3.dataPoint.BorderWidth, borderStyle, dataPoint3D6, secondPoint, points, pointIndex, 0f, operationType, lineSegmentType, (byte)(base.showPointLines ? 1 : 0) != 0, false, area.reverseSeriesOrder, base.multiSeries, 0, true);
					if (!flag)
					{
						backColor = ((this.currentKagiDirection == 1) ? this.kagiUpColor : color);
					}
				}
				graph.frontLinePen = null;
			}
			if (graphicsPath != null)
			{
				if (array[0] != null)
				{
					graphicsPath.AddPath(array[0], true);
				}
				if (array[1] != null)
				{
					graphicsPath.AddPath(array[1], true);
				}
				if (array[2] != null)
				{
					graphicsPath.AddPath(array[2], true);
				}
			}
			return graphicsPath;
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}

		public override void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			this.currentKagiDirection = 0;
			base.Paint(graph, common, area, seriesToDraw);
		}
	}
}
