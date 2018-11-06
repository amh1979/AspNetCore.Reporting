using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class StackedColumnChart : IChartType
	{
		protected double prevPosY = double.NaN;

		protected double prevNegY = double.NaN;

		protected bool hundredPercentStacked;

		internal bool stackGroupNameUsed;

		internal ArrayList stackGroupNames;

		internal string currentStackGroup = string.Empty;

		public virtual string Name
		{
			get
			{
				return "StackedColumn";
			}
		}

		public virtual bool Stacked
		{
			get
			{
				return true;
			}
		}

		public virtual bool SupportStackedGroups
		{
			get
			{
				return true;
			}
		}

		public bool StackSign
		{
			get
			{
				return true;
			}
		}

		public virtual bool RequireAxes
		{
			get
			{
				return true;
			}
		}

		public virtual bool SecondYScale
		{
			get
			{
				return false;
			}
		}

		public bool CircularChartArea
		{
			get
			{
				return false;
			}
		}

		public virtual bool SupportLogarithmicAxes
		{
			get
			{
				return true;
			}
		}

		public virtual bool SwitchValueAxes
		{
			get
			{
				return false;
			}
		}

		public bool SideBySideSeries
		{
			get
			{
				return false;
			}
		}

		public virtual bool DataPointsInLegend
		{
			get
			{
				return false;
			}
		}

		public virtual bool ExtraYValuesConnectedToYAxis
		{
			get
			{
				return false;
			}
		}

		public virtual bool HundredPercent
		{
			get
			{
				return false;
			}
		}

		public virtual bool HundredPercentSupportNegative
		{
			get
			{
				return false;
			}
		}

		public virtual bool ApplyPaletteColorsToPoints
		{
			get
			{
				return false;
			}
		}

		public virtual int YValuesPerPoint
		{
			get
			{
				return 1;
			}
		}

		public virtual bool ZeroCrossing
		{
			get
			{
				return true;
			}
		}

		public virtual Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}

		public virtual LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Rectangle;
		}

		public virtual void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			this.stackGroupNameUsed = true;
			RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(area.PlotAreaPosition.ToRectangleF());
			float num = (float)Math.Ceiling((double)absoluteRectangle.Right);
			float num2 = (float)Math.Ceiling((double)absoluteRectangle.Bottom);
			absoluteRectangle.X = (float)Math.Floor((double)absoluteRectangle.X);
			absoluteRectangle.Width = num - absoluteRectangle.X;
			absoluteRectangle.Y = (float)Math.Floor((double)absoluteRectangle.Y);
			absoluteRectangle.Height = num2 - absoluteRectangle.Y;
			graph.SetClipAbs(absoluteRectangle);
			this.ProcessChartType(false, graph, common, area, true, false, seriesToDraw);
			this.ProcessChartType(false, graph, common, area, false, false, seriesToDraw);
			this.ProcessChartType(false, graph, common, area, false, true, seriesToDraw);
			graph.ResetClip();
		}

		private void ProcessChartType(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, bool shadow, bool labels, Series seriesToDraw)
		{
			bool flag = false;
			AxisType axisType = AxisType.Primary;
			AxisType axisType2 = AxisType.Primary;
			string a = string.Empty;
			string a2 = string.Empty;
			for (int i = 0; i < common.DataManager.Series.Count; i++)
			{
				Series series = common.DataManager.Series[i];
				if (string.Compare(series.ChartTypeName, this.Name, StringComparison.OrdinalIgnoreCase) == 0 && !(series.ChartArea != area.Name) && series.IsVisible())
				{
					if (i != 0)
					{
						if (axisType == series.XAxisType && axisType2 == series.YAxisType && !(a != series.XSubAxisName) && !(a2 != series.YSubAxisName))
						{
							continue;
						}
						flag = true;
						break;
					}
					axisType = series.XAxisType;
					axisType2 = series.YAxisType;
					a = series.XSubAxisName;
					a2 = series.YSubAxisName;
				}
			}
			if (flag)
			{
				for (int j = 0; j < common.DataManager.Series.Count; j++)
				{
					Series series2 = common.DataManager.Series[j];
					if (string.Compare(series2.ChartTypeName, this.Name, StringComparison.OrdinalIgnoreCase) == 0 && !(series2.ChartArea != area.Name) && series2.IsVisible())
					{
						string seriesStackGroupName = StackedColumnChart.GetSeriesStackGroupName(series2);
						string[] values = new string[7]
						{
							"_X_",
							series2.XAxisType.ToString(),
							series2.XSubAxisName,
							"_Y_",
							series2.YAxisType.ToString(),
							series2.YSubAxisName,
							"__"
						};
						seriesStackGroupName = (((DataPointAttributes)series2)["StackedGroupName"] = string.Concat(values));
					}
				}
			}
			this.stackGroupNames = new ArrayList();
			foreach (Series item in common.DataManager.Series)
			{
				if (string.Compare(item.ChartTypeName, this.Name, StringComparison.OrdinalIgnoreCase) == 0 && !(item.ChartArea != area.Name) && item.IsVisible())
				{
					string seriesStackGroupName2 = StackedColumnChart.GetSeriesStackGroupName(item);
					if (!this.stackGroupNames.Contains(seriesStackGroupName2))
					{
						this.stackGroupNames.Add(seriesStackGroupName2);
					}
				}
			}
			if (area.Area3DStyle.Enable3D)
			{
				if (!shadow)
				{
					this.ProcessChartType3D(selection, graph, common, area, labels, seriesToDraw);
				}
			}
			else
			{
				string[] series4 = (string[])area.GetSeriesFromChartType(this.Name).ToArray(typeof(string));
				int numberOfPoints = common.DataManager.GetNumberOfPoints(series4);
				bool flag2 = area.IndexedSeries(series4);
				for (int k = 0; k < numberOfPoints; k++)
				{
					for (int l = 0; l < this.stackGroupNames.Count; l++)
					{
						this.currentStackGroup = (string)this.stackGroupNames[l];
						int num = 0;
						double num2 = 0.0;
						double num3 = 0.0;
						foreach (Series item2 in common.DataManager.Series)
						{
							if (string.Compare(item2.ChartTypeName, this.Name, StringComparison.OrdinalIgnoreCase) == 0 && !(item2.ChartArea != area.Name) && item2.IsVisible() && k < item2.Points.Count)
							{
								string seriesStackGroupName3 = StackedColumnChart.GetSeriesStackGroupName(item2);
								if (!(seriesStackGroupName3 != this.currentStackGroup))
								{
									DataPoint dataPoint = item2.Points[k];
									dataPoint.positionRel = new PointF(float.NaN, float.NaN);
									Axis axis = area.GetAxis(AxisName.Y, item2.YAxisType, item2.YSubAxisName);
									Axis axis2 = area.GetAxis(AxisName.X, item2.XAxisType, item2.XSubAxisName);
									bool flag3 = false;
									double interval = 1.0;
									if (!flag2)
									{
										if (item2.Points.Count == 1 && (item2.XValueType == ChartValueTypes.Date || item2.XValueType == ChartValueTypes.DateTime || item2.XValueType == ChartValueTypes.Time || item2.XValueType == ChartValueTypes.DateTimeOffset))
										{
											ArrayList seriesFromChartType = area.GetSeriesFromChartType(this.Name);
											((ChartAreaAxes)area).GetPointsInterval(seriesFromChartType, axis2.Logarithmic, axis2.logarithmBase, true, out flag3);
											interval = ((double.IsNaN(axis2.majorGrid.Interval) || axis2.majorGrid.IntervalType == DateTimeIntervalType.NotSet) ? axis2.GetIntervalSize(axis2.minimum, axis2.Interval, axis2.IntervalType) : axis2.GetIntervalSize(axis2.minimum, axis2.majorGrid.Interval, axis2.majorGrid.IntervalType));
										}
										else
										{
											interval = area.GetPointsInterval(axis2.Logarithmic, axis2.logarithmBase);
										}
									}
									double pointWidth = item2.GetPointWidth(graph, axis2, interval, 0.8);
									pointWidth /= (double)this.stackGroupNames.Count;
									if (!selection)
									{
										common.EventsManager.OnBackPaint(item2, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
									}
									double num4 = this.GetYValue(common, area, item2, dataPoint, k, 0);
									if (num != 0)
									{
										num4 = ((!(num4 >= 0.0)) ? (num4 + num3) : (num4 + num2));
									}
									bool flag4 = false;
									double num5 = num4;
									num4 = axis.GetLogValue(num4);
									if (!flag4 || !labels)
									{
										if (num4 > axis.GetViewMaximum())
										{
											num4 = axis.GetViewMaximum();
										}
										if (num4 < axis.GetViewMinimum())
										{
											num4 = axis.GetViewMinimum();
										}
									}
									double linearPosition = axis.GetLinearPosition(num4);
									double num6 = 0.0;
									num6 = ((num != 0) ? ((!(this.GetYValue(common, area, item2, dataPoint, k, 0) >= 0.0)) ? num3 : num2) : ((!flag4 || !labels) ? axis.Crossing : 0.0));
									double position = axis.GetPosition(num6);
									double num7 = dataPoint.XValue;
									if (flag2)
									{
										num7 = (double)k + 1.0;
									}
									double num8 = axis2.GetPosition(num7);
									if (this.stackGroupNames.Count > 1)
									{
										num8 = num8 - pointWidth * (double)this.stackGroupNames.Count / 2.0 + pointWidth / 2.0 + (double)l * pointWidth;
									}
									num7 = axis2.GetLogValue(num7);
									RectangleF empty = RectangleF.Empty;
									try
									{
										empty.X = (float)(num8 - pointWidth / 2.0);
										empty.Width = (float)pointWidth;
										if (position < linearPosition)
										{
											empty.Y = (float)position;
											empty.Height = (float)linearPosition - empty.Y;
										}
										else
										{
											empty.Y = (float)linearPosition;
											empty.Height = (float)position - empty.Y;
										}
									}
									catch (Exception)
									{
										num++;
										continue;
									}
									dataPoint.positionRel = new PointF((float)num8, empty.Top);
									if (dataPoint.Empty)
									{
										num++;
									}
									else
									{
										if (common.ProcessModePaint)
										{
											bool flag5 = false;
											if (num7 < axis2.GetViewMinimum() || num7 > axis2.GetViewMaximum() || (num4 < axis.GetViewMinimum() && num6 < axis.GetViewMinimum()) || (num4 > axis.GetViewMaximum() && num6 > axis.GetViewMaximum()))
											{
												flag5 = true;
											}
											if (!flag5)
											{
												int num9 = 0;
												if (shadow)
												{
													num9 = item2.ShadowOffset;
												}
												if (!labels)
												{
													bool flag6 = false;
													if (empty.X < area.PlotAreaPosition.X || empty.Right > area.PlotAreaPosition.Right() || empty.Y < area.PlotAreaPosition.Y || empty.Bottom > area.PlotAreaPosition.Bottom())
													{
														graph.SetClip(area.PlotAreaPosition.ToRectangleF());
														flag6 = true;
													}
													graph.StartHotRegion(dataPoint);
													graph.StartAnimation();
													if (!shadow || num9 != 0)
													{
														graph.FillRectangleRel(empty, (!shadow) ? dataPoint.Color : Color.Transparent, dataPoint.BackHatchStyle, dataPoint.BackImage, dataPoint.BackImageMode, dataPoint.BackImageTransparentColor, dataPoint.BackImageAlign, dataPoint.BackGradientType, (!shadow) ? dataPoint.BackGradientEndColor : Color.Transparent, dataPoint.BorderColor, dataPoint.BorderWidth, dataPoint.BorderStyle, item2.ShadowColor, num9, PenAlignment.Inset, (!shadow) ? ChartGraphics.GetBarDrawingStyle(dataPoint) : BarDrawingStyle.Default, true);
													}
													graph.StopAnimation();
													graph.EndHotRegion();
													if (flag6)
													{
														graph.ResetClip();
													}
												}
												else
												{
													graph.StartAnimation();
													this.DrawLabels(common, graph, area, dataPoint, k, item2, empty);
													graph.StopAnimation();
												}
											}
										}
										if (common.ProcessModeRegions && !shadow && !labels)
										{
											common.HotRegionsList.AddHotRegion(graph, empty, dataPoint, item2.Name, k);
										}
										if (!selection)
										{
											common.EventsManager.OnPaint(item2, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
										}
										if (axis.Logarithmic)
										{
											num4 = Math.Pow(axis.logarithmBase, num4);
										}
										num++;
										if (this.GetYValue(common, area, item2, dataPoint, k, 0) >= 0.0)
										{
											num2 = num5;
										}
										else
										{
											num3 = num5;
										}
									}
								}
							}
						}
					}
				}
				if (flag)
				{
					for (int m = 0; m < common.DataManager.Series.Count; m++)
					{
						Series series6 = common.DataManager.Series[m];
						if (string.Compare(series6.ChartTypeName, this.Name, StringComparison.OrdinalIgnoreCase) == 0 && !(series6.ChartArea != area.Name) && series6.IsVisible())
						{
							string text2 = StackedColumnChart.GetSeriesStackGroupName(series6);
							int num10 = text2.IndexOf("__", StringComparison.Ordinal);
							if (num10 >= 0)
							{
								text2 = text2.Substring(num10 + 2);
							}
							if (text2.Length > 0)
							{
								((DataPointAttributes)series6)["StackedGroupName"] = text2;
							}
							else
							{
								series6.DeleteAttribute("StackedGroupName");
							}
						}
					}
				}
			}
		}

		internal static Series[] GetSeriesByStackedGroupName(CommonElements common, string groupName, string chartTypeName, string chartAreaName)
		{
			ArrayList arrayList = new ArrayList();
			foreach (Series item in common.DataManager.Series)
			{
				if (string.Compare(item.ChartTypeName, chartTypeName, StringComparison.OrdinalIgnoreCase) == 0 && chartAreaName == item.ChartArea && item.IsVisible() && StackedColumnChart.GetSeriesStackGroupName(item) == groupName)
				{
					arrayList.Add(item);
				}
			}
			int num = 0;
			Series[] array = new Series[arrayList.Count];
			foreach (Series item2 in arrayList)
			{
				array[num++] = item2;
			}
			return array;
		}

		internal static string GetSeriesStackGroupName(Series series)
		{
			string result = string.Empty;
			if (series.IsAttributeSet("StackedGroupName"))
			{
				result = ((DataPointAttributes)series)["StackedGroupName"];
			}
			return result;
		}

		internal static bool IsSeriesStackGroupNameSupported(Series series)
		{
			if (series.ChartType != SeriesChartType.StackedColumn && series.ChartType != SeriesChartType.StackedColumn100 && series.ChartType != SeriesChartType.StackedBar && series.ChartType != SeriesChartType.StackedBar100)
			{
				return false;
			}
			return true;
		}

		public void DrawLabels(CommonElements common, ChartGraphics graph, ChartArea area, DataPoint point, int pointIndex, Series series, RectangleF rectangle)
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			Region clip = graph.Clip;
			graph.Clip = new Region();
			if (point.ShowLabelAsValue || point.Label.Length > 0)
			{
				string text;
				if (point.Label.Length == 0)
				{
					double value = this.GetYValue(common, area, series, point, pointIndex, 0);
					if (this.hundredPercentStacked && point.LabelFormat.Length == 0)
					{
						value = Math.Round(value, 2);
					}
					text = ValueConverter.FormatValue(series.chart, point, value, point.LabelFormat, series.YValueType, ChartElementType.DataPoint);
				}
				else
				{
					text = point.ReplaceKeywords(point.Label);
					if (series.chart != null && series.chart.LocalizeTextHandler != null)
					{
						text = series.chart.LocalizeTextHandler(point, text, point.ElementId, ChartElementType.DataPoint);
					}
				}
				PointF pointF = PointF.Empty;
				pointF.X = (float)(rectangle.X + rectangle.Width / 2.0);
				pointF.Y = (float)(rectangle.Y + rectangle.Height / 2.0);
				int angle = point.FontAngle;
				if (text.Trim().Length != 0)
				{
					SizeF labelSize = SizeF.Empty;
					if (series.SmartLabels.Enabled)
					{
						labelSize = graph.GetRelativeSize(graph.MeasureString(text, point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
						bool markerOverlapping = series.SmartLabels.MarkerOverlapping;
						LabelAlignmentTypes movingDirection = series.SmartLabels.MovingDirection;
						series.SmartLabels.MarkerOverlapping = true;
						if (series.SmartLabels.MovingDirection == (LabelAlignmentTypes.Top | LabelAlignmentTypes.Bottom | LabelAlignmentTypes.Right | LabelAlignmentTypes.Left | LabelAlignmentTypes.TopLeft | LabelAlignmentTypes.TopRight | LabelAlignmentTypes.BottomLeft | LabelAlignmentTypes.BottomRight))
						{
							series.SmartLabels.MovingDirection = (LabelAlignmentTypes.Top | LabelAlignmentTypes.Bottom);
						}
						pointF = area.smartLabels.AdjustSmartLabelPosition(common, graph, area, series.SmartLabels, pointF, labelSize, ref stringFormat, pointF, new SizeF(0f, 0f), LabelAlignmentTypes.Center);
						series.SmartLabels.MarkerOverlapping = markerOverlapping;
						series.SmartLabels.MovingDirection = movingDirection;
						angle = 0;
					}
					if (!pointF.IsEmpty)
					{
						PointF absolutePoint = graph.GetAbsolutePoint(pointF);
						if (graph.TextRenderingHint != TextRenderingHint.AntiAlias)
						{
							absolutePoint.X = (float)((float)Math.Ceiling((double)absolutePoint.X) + 1.0);
							pointF = graph.GetRelativePoint(absolutePoint);
						}
						if (labelSize.IsEmpty)
						{
							labelSize = graph.GetRelativeSize(graph.MeasureString(text, point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
						}
						RectangleF empty = RectangleF.Empty;
						SizeF sizeF = new SizeF(labelSize.Width, labelSize.Height);
						sizeF.Height += (float)(labelSize.Height / 8.0);
						sizeF.Width += sizeF.Width / (float)text.Length;
						empty = new RectangleF((float)(pointF.X - sizeF.Width / 2.0), (float)(pointF.Y - sizeF.Height / 2.0 - labelSize.Height / 10.0), sizeF.Width, sizeF.Height);
						graph.DrawPointLabelStringRel(common, text, point.Font, new SolidBrush(point.FontColor), pointF, stringFormat, angle, empty, point.LabelBackColor, point.LabelBorderColor, point.LabelBorderWidth, point.LabelBorderStyle, series, point, pointIndex);
					}
				}
			}
			graph.Clip = clip;
		}

		public virtual double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			double num = double.NaN;
			if (area.Area3DStyle.Enable3D)
			{
				switch (yValueIndex)
				{
				case -2:
					break;
				case -1:
				{
					Axis axis = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName);
					double crossing = axis.Crossing;
					num = this.GetYValue(common, area, series, point, pointIndex, 0);
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
				default:
					this.prevPosY = double.NaN;
					this.prevNegY = double.NaN;
					{
						foreach (Series item in common.DataManager.Series)
						{
							if (string.Compare(series.ChartArea, item.ChartArea, StringComparison.Ordinal) == 0 && string.Compare(series.ChartTypeName, item.ChartTypeName, StringComparison.OrdinalIgnoreCase) == 0 && item.IsVisible())
							{
								string seriesStackGroupName = StackedColumnChart.GetSeriesStackGroupName(item);
								if (this.stackGroupNameUsed && seriesStackGroupName != this.currentStackGroup)
								{
									continue;
								}
								if (double.IsNaN(num))
								{
									num = item.Points[pointIndex].YValues[0];
								}
								else
								{
									num = item.Points[pointIndex].YValues[0];
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
									continue;
								}
								return num;
							}
						}
						return num;
					}
				}
			}
			return point.YValues[0];
		}

		private void ProcessChartType3D(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, bool labels, Series seriesToDraw)
		{
			if (labels && !selection)
			{
				return;
			}
			ArrayList arrayList = null;
			arrayList = area.GetClusterSeriesNames(seriesToDraw.Name);
			common.DataManager.GetNumberOfPoints((string[])arrayList.ToArray(typeof(string)));
			ArrayList dataPointDrawingOrder = area.GetDataPointDrawingOrder(arrayList, this, selection, COPCoordinates.X | COPCoordinates.Y, null, 0, false);
			bool flag = false;
			foreach (object item in dataPointDrawingOrder)
			{
				DataPoint3D dataPoint3D = (DataPoint3D)item;
				DataPoint dataPoint = dataPoint3D.dataPoint;
				Series series = dataPoint.series;
				this.currentStackGroup = StackedColumnChart.GetSeriesStackGroupName(series);
				dataPoint.positionRel = new PointF(float.NaN, float.NaN);
				Axis axis = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName);
				Axis axis2 = area.GetAxis(AxisName.X, series.XAxisType, series.XSubAxisName);
				BarDrawingStyle barDrawingStyle = ChartGraphics.GetBarDrawingStyle(dataPoint);
				float num = 0.5f;
				float num2 = 0.5f;
				bool flag2 = true;
				bool flag3 = false;
				for (int i = 0; i < arrayList.Count; i++)
				{
					Series series2 = common.DataManager.Series[i];
					if (flag2 && dataPoint3D.index <= series2.Points.Count && series2.Points[dataPoint3D.index - 1].YValues[0] != 0.0)
					{
						flag2 = false;
						if (series2.Name == series.Name)
						{
							num2 = 0f;
						}
					}
					if (series2.Name == series.Name)
					{
						flag3 = true;
					}
					else if (dataPoint3D.index <= series2.Points.Count && series2.Points[dataPoint3D.index - 1].YValues[0] != 0.0)
					{
						flag3 = false;
					}
				}
				if (flag3)
				{
					num = 0f;
				}
				if (area.stackGroupNames != null && area.stackGroupNames.Count > 1 && area.Area3DStyle.Clustered)
				{
					string seriesStackGroupName = StackedColumnChart.GetSeriesStackGroupName(series);
					bool flag4 = true;
					bool flag5 = false;
					foreach (string item2 in arrayList)
					{
						Series series3 = common.DataManager.Series[item2];
						if (StackedColumnChart.GetSeriesStackGroupName(series3) == seriesStackGroupName)
						{
							if (flag4 && dataPoint3D.index < series3.Points.Count && series3.Points[dataPoint3D.index - 1].YValues[0] != 0.0)
							{
								flag4 = false;
								if (item2 == series.Name)
								{
									num2 = 0f;
								}
							}
							if (item2 == series.Name)
							{
								flag5 = true;
							}
							else if (dataPoint3D.index < series3.Points.Count && series3.Points[dataPoint3D.index - 1].YValues[0] != 0.0)
							{
								flag5 = false;
							}
						}
					}
					if (flag5)
					{
						num = 0f;
					}
				}
				double yValue = this.GetYValue(common, area, series, dataPoint3D.dataPoint, dataPoint3D.index - 1, 0);
				double yValue2 = yValue - this.GetYValue(common, area, series, dataPoint3D.dataPoint, dataPoint3D.index - 1, -1);
				yValue = axis.GetLogValue(yValue);
				yValue2 = axis.GetLogValue(yValue2);
				if (yValue2 > axis.GetViewMaximum())
				{
					num = 0.5f;
					yValue2 = axis.GetViewMaximum();
				}
				else if (yValue2 < axis.GetViewMinimum())
				{
					num2 = 0.5f;
					yValue2 = axis.GetViewMinimum();
				}
				if (yValue > axis.GetViewMaximum())
				{
					num = 0.5f;
					yValue = axis.GetViewMaximum();
				}
				else if (yValue < axis.GetViewMinimum())
				{
					num2 = 0.5f;
					yValue = axis.GetViewMinimum();
				}
				double linearPosition = axis.GetLinearPosition(yValue);
				double linearPosition2 = axis.GetLinearPosition(yValue2);
				RectangleF empty = RectangleF.Empty;
				try
				{
					empty.X = (float)(dataPoint3D.xPosition - dataPoint3D.width / 2.0);
					empty.Width = (float)dataPoint3D.width;
					if (linearPosition2 < linearPosition)
					{
						float num3 = num2;
						num2 = num;
						num = num3;
						empty.Y = (float)linearPosition2;
						empty.Height = (float)linearPosition - empty.Y;
					}
					else
					{
						empty.Y = (float)linearPosition;
						empty.Height = (float)linearPosition2 - empty.Y;
					}
				}
				catch (Exception)
				{
					continue;
				}
				dataPoint.positionRel = new PointF((float)dataPoint3D.xPosition, empty.Top);
				if (!dataPoint.Empty)
				{
					double yValue3 = dataPoint3D.indexedSeries ? ((double)dataPoint3D.index) : dataPoint.XValue;
					yValue3 = axis2.GetLogValue(yValue3);
					if (!(yValue3 < axis2.GetViewMinimum()) && !(yValue3 > axis2.GetViewMaximum()) && (!(yValue < axis.GetViewMinimum()) || !(yValue2 < axis.GetViewMinimum())) && (!(yValue > axis.GetViewMaximum()) || !(yValue2 > axis.GetViewMaximum())))
					{
						bool flag6 = false;
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
							if (empty.Height != 0.0 && empty.Width != 0.0)
							{
								DrawingOperationTypes drawingOperationTypes = DrawingOperationTypes.DrawElement;
								if (common.ProcessModeRegions)
								{
									drawingOperationTypes |= DrawingOperationTypes.CalcElementPath;
								}
								graph.StartHotRegion(dataPoint);
								GraphicsPath path = graph.Fill3DRectangle(empty, dataPoint3D.zPosition, dataPoint3D.depth, area.matrix3D, area.Area3DStyle.Light, dataPoint.Color, num, num2, dataPoint.BackHatchStyle, dataPoint.BackImage, dataPoint.BackImageMode, dataPoint.BackImageTransparentColor, dataPoint.BackImageAlign, dataPoint.BackGradientType, dataPoint.BackGradientEndColor, dataPoint.BorderColor, dataPoint.BorderWidth, dataPoint.BorderStyle, PenAlignment.Inset, barDrawingStyle, true, drawingOperationTypes);
								graph.StopAnimation();
								graph.EndHotRegion();
								if (flag6)
								{
									graph.ResetClip();
								}
								if (common.ProcessModeRegions && !labels)
								{
									common.HotRegionsList.AddHotRegion(path, false, graph, dataPoint, series.Name, dataPoint3D.index - 1);
								}
								if (dataPoint.ShowLabelAsValue || dataPoint.Label.Length > 0)
								{
									flag = true;
								}
							}
						}
					}
				}
			}
			if (flag)
			{
				foreach (object item3 in dataPointDrawingOrder)
				{
					DataPoint3D dataPoint3D2 = (DataPoint3D)item3;
					DataPoint dataPoint2 = dataPoint3D2.dataPoint;
					Series series4 = dataPoint2.series;
					Axis axis3 = area.GetAxis(AxisName.Y, series4.YAxisType, series4.YSubAxisName);
					Axis axis4 = area.GetAxis(AxisName.X, series4.XAxisType, series4.XSubAxisName);
					double num4 = this.GetYValue(common, area, series4, dataPoint3D2.dataPoint, dataPoint3D2.index - 1, 0);
					if (num4 > axis3.GetViewMaximum())
					{
						num4 = axis3.GetViewMaximum();
					}
					if (num4 < axis3.GetViewMinimum())
					{
						num4 = axis3.GetViewMinimum();
					}
					num4 = axis3.GetLogValue(num4);
					double yPosition = dataPoint3D2.yPosition;
					double num5 = num4 - axis3.GetLogValue(this.GetYValue(common, area, series4, dataPoint3D2.dataPoint, dataPoint3D2.index - 1, -1));
					double height = dataPoint3D2.height;
					RectangleF empty2 = RectangleF.Empty;
					try
					{
						empty2.X = (float)(dataPoint3D2.xPosition - dataPoint3D2.width / 2.0);
						empty2.Width = (float)dataPoint3D2.width;
						if (height < yPosition)
						{
							empty2.Y = (float)height;
							empty2.Height = (float)yPosition - empty2.Y;
						}
						else
						{
							empty2.Y = (float)yPosition;
							empty2.Height = (float)height - empty2.Y;
						}
					}
					catch (Exception)
					{
						continue;
					}
					if (!dataPoint2.Empty && !selection)
					{
						double yValue4 = dataPoint3D2.indexedSeries ? ((double)dataPoint3D2.index) : dataPoint2.XValue;
						yValue4 = axis4.GetLogValue(yValue4);
						if (!(yValue4 < axis4.GetViewMinimum()) && !(yValue4 > axis4.GetViewMaximum()) && (!(num4 < axis3.GetViewMinimum()) || !(num5 < axis3.GetViewMinimum())) && (!(num4 > axis3.GetViewMaximum()) || !(num5 > axis3.GetViewMaximum())))
						{
							graph.StartAnimation();
							this.DrawLabels3D(common, graph, area, dataPoint3D2, dataPoint3D2.index - 1, series4, empty2);
							graph.StopAnimation();
						}
					}
				}
			}
		}

		internal void DrawLabels3D(CommonElements common, ChartGraphics graph, ChartArea area, DataPoint3D pointEx, int pointIndex, Series series, RectangleF rectangle)
		{
			DataPoint dataPoint = pointEx.dataPoint;
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			Region clip = graph.Clip;
			graph.Clip = new Region();
			if (dataPoint.ShowLabelAsValue || dataPoint.Label.Length > 0)
			{
				string text;
				if (dataPoint.Label.Length == 0)
				{
					double value = this.GetYValue(common, area, series, dataPoint, pointIndex, -2);
					if (this.hundredPercentStacked && dataPoint.LabelFormat.Length == 0)
					{
						value = Math.Round(value, 2);
					}
					text = ValueConverter.FormatValue(series.chart, dataPoint, value, dataPoint.LabelFormat, series.YValueType, ChartElementType.DataPoint);
				}
				else
				{
					text = dataPoint.ReplaceKeywords(dataPoint.Label);
					if (series.chart != null && series.chart.LocalizeTextHandler != null)
					{
						text = series.chart.LocalizeTextHandler(dataPoint, text, dataPoint.ElementId, ChartElementType.DataPoint);
					}
				}
				PointF pointF = PointF.Empty;
				pointF.X = (float)(rectangle.X + rectangle.Width / 2.0);
				pointF.Y = (float)(rectangle.Y + rectangle.Height / 2.0);
				Point3D[] array = new Point3D[1]
				{
					new Point3D(pointF.X, pointF.Y, pointEx.zPosition + pointEx.depth)
				};
				area.matrix3D.TransformPoints(array);
				pointF.X = array[0].X;
				pointF.Y = array[0].Y;
				int angle = dataPoint.FontAngle;
				SizeF labelSize = SizeF.Empty;
				if (series.SmartLabels.Enabled)
				{
					labelSize = graph.GetRelativeSize(graph.MeasureString(text, dataPoint.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
					bool markerOverlapping = series.SmartLabels.MarkerOverlapping;
					LabelAlignmentTypes movingDirection = series.SmartLabels.MovingDirection;
					series.SmartLabels.MarkerOverlapping = true;
					if (series.SmartLabels.MovingDirection == (LabelAlignmentTypes.Top | LabelAlignmentTypes.Bottom | LabelAlignmentTypes.Right | LabelAlignmentTypes.Left | LabelAlignmentTypes.TopLeft | LabelAlignmentTypes.TopRight | LabelAlignmentTypes.BottomLeft | LabelAlignmentTypes.BottomRight))
					{
						series.SmartLabels.MovingDirection = (LabelAlignmentTypes.Top | LabelAlignmentTypes.Bottom);
					}
					pointF = area.smartLabels.AdjustSmartLabelPosition(common, graph, area, series.SmartLabels, pointF, labelSize, ref stringFormat, pointF, new SizeF(0f, 0f), LabelAlignmentTypes.Center);
					series.SmartLabels.MarkerOverlapping = markerOverlapping;
					series.SmartLabels.MovingDirection = movingDirection;
					angle = 0;
				}
				if (!pointF.IsEmpty)
				{
					if (labelSize.IsEmpty)
					{
						labelSize = graph.GetRelativeSize(graph.MeasureString(text, dataPoint.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
					}
					RectangleF empty = RectangleF.Empty;
					SizeF sizeF = new SizeF(labelSize.Width, labelSize.Height);
					sizeF.Height += (float)(labelSize.Height / 8.0);
					sizeF.Width += sizeF.Width / (float)text.Length;
					empty = new RectangleF((float)(pointF.X - sizeF.Width / 2.0), (float)(pointF.Y - sizeF.Height / 2.0 - labelSize.Height / 10.0), sizeF.Width, sizeF.Height);
					graph.DrawPointLabelStringRel(common, text, dataPoint.Font, new SolidBrush(dataPoint.FontColor), pointF, stringFormat, angle, empty, dataPoint.LabelBackColor, dataPoint.LabelBorderColor, dataPoint.LabelBorderWidth, dataPoint.LabelBorderStyle, series, dataPoint, pointIndex);
				}
			}
			graph.Clip = clip;
		}

		public void AddSmartLabelMarkerPositions(CommonElements common, ChartArea area, Series series, ArrayList list)
		{
		}
	}
}
