using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class PointAndFigureChart : RangeColumnChart
	{
		private static bool customizeSubscribed;

		public override string Name
		{
			get
			{
				return "PointAndFigure";
			}
		}

		internal static void PrepareData(Series series, IServiceContainer serviceContainer)
		{
			if (string.Compare(series.ChartTypeName, "PointAndFigure", StringComparison.OrdinalIgnoreCase) == 0 && series.IsVisible())
			{
				Chart chart = (Chart)serviceContainer.GetService(typeof(Chart));
				if (chart == null)
				{
					throw new InvalidOperationException(SR.ExceptionPointAndFigureNullReference);
				}
				ChartArea chartArea = chart.ChartAreas[series.ChartArea];
				foreach (Series item in chart.Series)
				{
					if (item.IsVisible() && item != series && chartArea == chart.ChartAreas[item.ChartArea])
					{
						throw new InvalidOperationException(SR.ExceptionPointAndFigureCanNotCombine);
					}
				}
				if (!PointAndFigureChart.customizeSubscribed)
				{
					PointAndFigureChart.customizeSubscribed = true;
					chart.Customize += PointAndFigureChart.OnCustomize;
				}
				string name = "POINTANDFIGURE_ORIGINAL_DATA_" + series.Name;
				if (chart.Series.GetIndex(name) == -1)
				{
					Series series3 = new Series(name, series.YValuesPerPoint);
					series3.Enabled = false;
					series3.ShowInLegend = false;
					series3.YValuesPerPoint = series.YValuesPerPoint;
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
					PointAndFigureChart.FillPointAndFigureData(series, series3);
				}
			}
		}

		internal static bool UnPrepareData(Series series, IServiceContainer serviceContainer)
		{
			if (series.Name.StartsWith("POINTANDFIGURE_ORIGINAL_DATA_", StringComparison.Ordinal))
			{
				Chart chart = (Chart)serviceContainer.GetService(typeof(Chart));
				if (chart == null)
				{
					throw new InvalidOperationException(SR.ExceptionPointAndFigureNullReference);
				}
				if (PointAndFigureChart.customizeSubscribed)
				{
					PointAndFigureChart.customizeSubscribed = false;
					chart.Customize -= PointAndFigureChart.OnCustomize;
				}
				Series series2 = chart.Series[series.Name.Substring(29)];
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
				series2.DeleteAttribute("EmptyPointValue");
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

		private static void GetPriceRange(Series originalData, int yValueHighIndex, int yValueLowIndex, out double minPrice, out double maxPrice)
		{
			maxPrice = -1.7976931348623157E+308;
			minPrice = 1.7976931348623157E+308;
			foreach (DataPoint point in originalData.Points)
			{
				if (!point.Empty)
				{
					if (point.YValues.Length < 2)
					{
						throw new InvalidOperationException(SR.ExceptionChartTypeRequiresYValues("PointAndFigure", 2.ToString(CultureInfo.CurrentCulture)));
					}
					if (point.YValues[yValueHighIndex] > maxPrice)
					{
						maxPrice = point.YValues[yValueHighIndex];
					}
					else if (point.YValues[yValueLowIndex] > maxPrice)
					{
						maxPrice = point.YValues[yValueLowIndex];
					}
					if (point.YValues[yValueHighIndex] < minPrice)
					{
						minPrice = point.YValues[yValueHighIndex];
					}
					else if (point.YValues[yValueLowIndex] < minPrice)
					{
						minPrice = point.YValues[yValueLowIndex];
					}
				}
			}
		}

		private static double GetBoxSize(Series series, Series originalData, int yValueHighIndex, int yValueLowIndex, double minPrice, double maxPrice)
		{
			double num = 1.0;
			double num2 = 4.0;
			bool flag = true;
			if (series.IsAttributeSet("BoxSize"))
			{
				string text = ((DataPointAttributes)series)["BoxSize"].Trim();
				bool flag2 = text.EndsWith("%", StringComparison.Ordinal);
				if (flag2)
				{
					text = text.Substring(0, text.Length - 1);
				}
				try
				{
					if (flag2)
					{
						num2 = double.Parse(text, CultureInfo.InvariantCulture);
						flag = false;
					}
					else
					{
						num = double.Parse(text, CultureInfo.InvariantCulture);
						num2 = 0.0;
					}
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionRenkoBoxSizeFormatInvalid);
				}
			}
			if (num2 > 0.0)
			{
				num = 1.0;
				num = ((minPrice != maxPrice) ? ((!(maxPrice - minPrice < 1E-06)) ? ((maxPrice - minPrice) * (num2 / 100.0)) : 1E-06) : 1.0);
				if (flag)
				{
					double[] array = new double[30]
					{
						1E-06,
						1E-05,
						0.0001,
						0.001,
						0.01,
						0.1,
						0.25,
						0.5,
						1.0,
						2.0,
						2.5,
						3.0,
						4.0,
						5.0,
						7.5,
						10.0,
						15.0,
						20.0,
						25.0,
						50.0,
						100.0,
						200.0,
						500.0,
						1000.0,
						5000.0,
						10000.0,
						50000.0,
						100000.0,
						1000000.0,
						1000000.0
					};
					for (int i = 1; i < array.Length; i++)
					{
						if (num > array[i - 1] && num < array[i])
						{
							num = array[i];
						}
					}
				}
			}
			((DataPointAttributes)series)["CurrentBoxSize"] = num.ToString(CultureInfo.InvariantCulture);
			return num;
		}

		private static double GetReversalAmount(Series series, Series originalData, int yValueHighIndex, int yValueLowIndex, double minPrice, double maxPrice)
		{
			double result = 3.0;
			if (series.IsAttributeSet("ReversalAmount"))
			{
				string s = ((DataPointAttributes)series)["ReversalAmount"].Trim();
				try
				{
					return double.Parse(s, CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionPointAndFigureReversalAmountInvalidFormat);
				}
			}
			return result;
		}

		private static void FillPointAndFigureData(Series series, Series originalData)
		{
			int num = 0;
			if (series.IsAttributeSet("UsedYValueHigh"))
			{
				try
				{
					num = int.Parse(((DataPointAttributes)series)["UsedYValueHigh"], CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionPointAndFigureUsedYValueHighInvalidFormat);
				}
				if (num >= series.YValuesPerPoint)
				{
					throw new InvalidOperationException(SR.ExceptionPointAndFigureUsedYValueHighOutOfRange);
				}
			}
			int num2 = 1;
			if (series.IsAttributeSet("UsedYValueLow"))
			{
				try
				{
					num2 = int.Parse(((DataPointAttributes)series)["UsedYValueLow"], CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionPointAndFigureUsedYValueLowInvalidFormat);
				}
				if (num2 >= series.YValuesPerPoint)
				{
					throw new InvalidOperationException(SR.ExceptionPointAndFigureUsedYValueLowOutOfrange);
				}
			}
			Color color = ChartGraphics.GetGradientColor(series.Color, Color.Black, 0.5);
			string text = ((DataPointAttributes)series)["PriceUpColor"];
			if (text != null)
			{
				try
				{
					ColorConverter colorConverter = new ColorConverter();
					color = (Color)colorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, text);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionPointAndFigureUpBrickColorInvalidFormat);
				}
			}
			double minPrice = default(double);
			double maxPrice = default(double);
			PointAndFigureChart.GetPriceRange(originalData, num, num2, out minPrice, out maxPrice);
			double boxSize = PointAndFigureChart.GetBoxSize(series, originalData, num, num2, minPrice, maxPrice);
			double reversalAmount = PointAndFigureChart.GetReversalAmount(series, originalData, num, num2, minPrice, maxPrice);
			double num3 = double.NaN;
			double num4 = double.NaN;
			int num5 = 0;
			int num6 = 0;
			foreach (DataPoint point in originalData.Points)
			{
				if (!point.Empty)
				{
					bool flag = false;
					int num7 = 0;
					if (double.IsNaN(num3))
					{
						num3 = point.YValues[num];
						num4 = point.YValues[num2];
						num6++;
						continue;
					}
					int num8 = 0;
					if (num5 == 1 || num5 == 0)
					{
						if (point.YValues[num] >= num3 + boxSize)
						{
							num8 = 1;
							num7 = (int)Math.Floor((point.YValues[num] - num3) / boxSize);
						}
						else if (point.YValues[num2] <= num3 - boxSize * reversalAmount)
						{
							num8 = -1;
							num7 = (int)Math.Floor((num3 - point.YValues[num2]) / boxSize);
						}
						else if (point.YValues[num] <= num4 - boxSize)
						{
							flag = true;
							num7 = (int)Math.Floor((num4 - point.YValues[num]) / boxSize);
							if (series.Points.Count > 0)
							{
								series.Points[series.Points.Count - 1].YValues[0] -= (double)num7 * boxSize;
							}
							num4 -= (double)num7 * boxSize;
						}
					}
					if (num8 == 0 && (num5 == -1 || num5 == 0))
					{
						if (point.YValues[num2] <= num4 - boxSize)
						{
							num8 = -1;
							num7 = (int)Math.Floor((num4 - point.YValues[num2]) / boxSize);
						}
						else if (point.YValues[num] >= num4 + boxSize * reversalAmount)
						{
							num8 = 1;
							num7 = (int)Math.Floor((point.YValues[num] - num4) / boxSize);
						}
						else if (point.YValues[num2] >= num3 + boxSize)
						{
							flag = true;
							num7 = (int)Math.Floor((num3 - point.YValues[num2]) / boxSize);
							if (series.Points.Count > 0)
							{
								series.Points[series.Points.Count - 1].YValues[1] += (double)num7 * boxSize;
							}
							num3 += (double)num7 * boxSize;
						}
					}
					if (num8 != 0 && !flag)
					{
						if (num8 == num5)
						{
							if (num8 == 1)
							{
								series.Points[series.Points.Count - 1].YValues[1] += (double)num7 * boxSize;
								num3 += (double)num7 * boxSize;
								((DataPointAttributes)series.Points[series.Points.Count - 1])["OriginalPointIndex"] = num6.ToString(CultureInfo.InvariantCulture);
							}
							else
							{
								series.Points[series.Points.Count - 1].YValues[0] -= (double)num7 * boxSize;
								num4 -= (double)num7 * boxSize;
								((DataPointAttributes)series.Points[series.Points.Count - 1])["OriginalPointIndex"] = num6.ToString(CultureInfo.InvariantCulture);
							}
						}
						else
						{
							DataPoint dataPoint2 = point.Clone();
							((DataPointAttributes)dataPoint2)["OriginalPointIndex"] = num6.ToString(CultureInfo.InvariantCulture);
							dataPoint2.series = series;
							dataPoint2.XValue = point.XValue;
							if (num8 == 1)
							{
								dataPoint2.Color = color;
								((DataPointAttributes)dataPoint2)["PriceUpPoint"] = "true";
								dataPoint2.YValues[0] = num4 + ((num5 != 0) ? boxSize : 0.0);
								dataPoint2.YValues[1] = dataPoint2.YValues[0] + (double)num7 * boxSize - ((num5 != 0) ? boxSize : 0.0);
							}
							else
							{
								dataPoint2.YValues[1] = num3 - ((num5 != 0) ? boxSize : 0.0);
								dataPoint2.YValues[0] = dataPoint2.YValues[1] - (double)num7 * boxSize;
							}
							num3 = dataPoint2.YValues[1];
							num4 = dataPoint2.YValues[0];
							series.Points.Add(dataPoint2);
						}
						num5 = num8;
					}
				}
				num6++;
			}
		}

		private static void OnCustomize(object sender, EventArgs e)
		{
			bool flag = false;
			Chart chart = (Chart)sender;
			foreach (Series item in chart.Series)
			{
				if (item.Name.StartsWith("POINTANDFIGURE_ORIGINAL_DATA_", StringComparison.Ordinal))
				{
					Series series2 = chart.Series[item.Name.Substring(29)];
					bool flag2 = true;
					string text = ((DataPointAttributes)series2)["ProportionalSymbols"];
					if (text != null && string.Compare(text, "True", StringComparison.OrdinalIgnoreCase) != 0)
					{
						flag2 = false;
					}
					if (flag2 && series2.Enabled && series2.ChartArea.Length > 0)
					{
						if (!flag)
						{
							flag = true;
							chart.chartPicture.Resize(chart.chartPicture.chartGraph, false);
						}
						ChartArea chartArea = chart.ChartAreas[series2.ChartArea];
						Axis axis = chartArea.GetAxis(AxisName.X, series2.XAxisType, series2.XSubAxisName);
						Axis axis2 = chartArea.GetAxis(AxisName.Y, series2.YAxisType, series2.YSubAxisName);
						if (!chartArea.Area3DStyle.Enable3D)
						{
							double num = double.Parse(((DataPointAttributes)series2)["CurrentBoxSize"], CultureInfo.InvariantCulture);
							double num2 = Math.Abs(axis2.GetPosition(axis2.Minimum) - axis2.GetPosition(axis2.Minimum + num));
							double num3 = Math.Abs(axis.GetPosition(1.0) - axis.GetPosition(0.0));
							num3 *= 0.8;
							SizeF absoluteSize = chart.chartPicture.chartGraph.GetAbsoluteSize(new SizeF((float)num3, (float)num2));
							int num4 = 0;
							if (absoluteSize.Width > absoluteSize.Height)
							{
								num4 = (int)((float)series2.Points.Count * (absoluteSize.Width / absoluteSize.Height));
							}
							DataPoint dataPoint = new DataPoint(series2);
							dataPoint.Empty = true;
							dataPoint.AxisLabel = " ";
							while (series2.Points.Count < num4)
							{
								series2.Points.Add(dataPoint);
							}
							((DataPointAttributes)series2)["EmptyPointValue"] = "Zero";
							chartArea.ReCalcInternal();
						}
					}
				}
			}
		}

		protected override void DrawColumn2D(ChartGraphics graph, Axis vAxis, RectangleF rectSize, DataPoint point, Series ser)
		{
			double num = double.Parse(((DataPointAttributes)ser)["CurrentBoxSize"], CultureInfo.InvariantCulture);
			double logValue = vAxis.GetLogValue(vAxis.GetViewMinimum());
			logValue = vAxis.GetLinearPosition(logValue);
			logValue = Math.Abs(logValue - vAxis.GetLinearPosition(vAxis.GetLogValue(vAxis.GetViewMinimum() + num)));
			Math.Floor((double)graph.GetAbsoluteSize(new SizeF((float)logValue, (float)logValue)).Height);
			for (float num2 = rectSize.Y; num2 < rectSize.Bottom - (float)(logValue - logValue / 4.0); num2 += (float)logValue)
			{
				RectangleF rectangleF = RectangleF.Empty;
				rectangleF.X = rectSize.X;
				rectangleF.Y = num2;
				rectangleF.Width = rectSize.Width;
				rectangleF.Height = (float)logValue;
				rectangleF = graph.GetAbsoluteRectangle(rectangleF);
				int num3 = 1 + point.BorderWidth / 2;
				rectangleF.Y += (float)num3;
				rectangleF.Height -= (float)(2 * num3);
				RectangleF position = new RectangleF(rectangleF.Location, rectangleF.Size);
				position.Offset((float)ser.ShadowOffset, (float)ser.ShadowOffset);
				if (point.IsAttributeSet("PriceUpPoint"))
				{
					if (ser.ShadowOffset != 0)
					{
						graph.shadowDrawingMode = true;
						graph.DrawLineAbs(ser.ShadowColor, point.BorderWidth, ChartDashStyle.Solid, new PointF(position.Left, position.Top), new PointF(position.Right, position.Bottom));
						graph.DrawLineAbs(ser.ShadowColor, point.BorderWidth, ChartDashStyle.Solid, new PointF(position.Left, position.Bottom), new PointF(position.Right, position.Top));
						graph.shadowDrawingMode = false;
					}
					graph.DrawLineAbs(point.Color, point.BorderWidth, ChartDashStyle.Solid, new PointF(rectangleF.Left, rectangleF.Top), new PointF(rectangleF.Right, rectangleF.Bottom));
					graph.DrawLineAbs(point.Color, point.BorderWidth, ChartDashStyle.Solid, new PointF(rectangleF.Left, rectangleF.Bottom), new PointF(rectangleF.Right, rectangleF.Top));
				}
				else
				{
					if (ser.ShadowOffset != 0)
					{
						graph.shadowDrawingMode = true;
						graph.DrawCircleAbs(new Pen(ser.ShadowColor, (float)point.BorderWidth), null, position, 1, false);
						graph.shadowDrawingMode = false;
					}
					graph.DrawCircleAbs(new Pen(point.Color, (float)point.BorderWidth), null, rectangleF, 1, false);
				}
			}
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}
	}
}
