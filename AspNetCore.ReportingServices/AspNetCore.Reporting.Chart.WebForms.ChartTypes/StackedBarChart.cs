using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class StackedBarChart : IChartType
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
				return "StackedBar";
			}
		}

		public bool Stacked
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

		public bool RequireAxes
		{
			get
			{
				return true;
			}
		}

		public bool SecondYScale
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

		public bool SupportLogarithmicAxes
		{
			get
			{
				return true;
			}
		}

		public bool SwitchValueAxes
		{
			get
			{
				return true;
			}
		}

		public bool SideBySideSeries
		{
			get
			{
				return false;
			}
		}

		public bool ZeroCrossing
		{
			get
			{
				return true;
			}
		}

		public bool DataPointsInLegend
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

		public bool ApplyPaletteColorsToPoints
		{
			get
			{
				return false;
			}
		}

		public int YValuesPerPoint
		{
			get
			{
				return 1;
			}
		}

		public virtual Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}

		public LegendImageStyle GetLegendImageStyle(Series series)
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
									Axis axis = area.GetAxis(AxisName.X, item2.XAxisType, item2.XSubAxisName);
									Axis axis2 = area.GetAxis(AxisName.Y, item2.YAxisType, item2.YSubAxisName);
									double interval = 1.0;
									if (!flag2)
									{
										if (item2.Points.Count == 1 && (item2.XValueType == ChartValueTypes.Date || item2.XValueType == ChartValueTypes.DateTime || item2.XValueType == ChartValueTypes.Time || item2.XValueType == ChartValueTypes.DateTimeOffset))
										{
											bool flag3 = false;
											ArrayList seriesFromChartType = area.GetSeriesFromChartType(this.Name);
											((ChartAreaAxes)area).GetPointsInterval(seriesFromChartType, axis.Logarithmic, axis.logarithmBase, true, out flag3);
											interval = ((double.IsNaN(axis.majorGrid.Interval) || axis.majorGrid.IntervalType == DateTimeIntervalType.NotSet) ? axis.GetIntervalSize(axis.minimum, axis.Interval, axis.IntervalType) : axis.GetIntervalSize(axis.minimum, axis.majorGrid.Interval, axis.majorGrid.IntervalType));
										}
										else
										{
											interval = area.GetPointsInterval(axis.Logarithmic, axis.logarithmBase);
										}
									}
									double pointWidth = item2.GetPointWidth(graph, axis, interval, 0.8);
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
									if (axis2.Logarithmic)
									{
										num4 = Math.Log(num4, axis2.logarithmBase);
									}
									double linearPosition = axis2.GetLinearPosition(num4);
									double num6 = dataPoint.XValue;
									if (flag2)
									{
										num6 = (double)k + 1.0;
									}
									double num7 = axis.GetPosition(num6);
									if (this.stackGroupNames.Count > 1)
									{
										num7 = num7 - pointWidth * (double)this.stackGroupNames.Count / 2.0 + pointWidth / 2.0 + (double)l * pointWidth;
									}
									num6 = axis.GetLogValue(num6);
									double num8 = (num != 0) ? ((!(this.GetYValue(common, area, item2, dataPoint, k, 0) >= 0.0)) ? num3 : num2) : ((!flag4 || !labels) ? axis2.Crossing : 0.0);
									double position = axis2.GetPosition(num8);
									RectangleF empty = RectangleF.Empty;
									try
									{
										empty.Y = (float)(num7 - pointWidth / 2.0);
										empty.Height = (float)pointWidth;
										if (position < linearPosition)
										{
											empty.X = (float)position;
											empty.Width = (float)linearPosition - empty.X;
										}
										else
										{
											empty.X = (float)linearPosition;
											empty.Width = (float)position - empty.X;
										}
									}
									catch (Exception)
									{
										continue;
									}
									dataPoint.positionRel = new PointF(empty.Right, (float)num7);
									if (!dataPoint.Empty)
									{
										if (axis2.Logarithmic)
										{
											num8 = Math.Log(num8, axis2.logarithmBase);
										}
										bool flag5 = false;
										if (num6 < axis.GetViewMinimum() || num6 > axis.GetViewMaximum() || (num4 < axis2.GetViewMinimum() && num8 < axis2.GetViewMinimum()) || (num4 > axis2.GetViewMaximum() && num8 > axis2.GetViewMaximum()))
										{
											flag5 = true;
										}
										if (!flag5)
										{
											if (common.ProcessModePaint)
											{
												bool flag6 = false;
												if (empty.Y < area.PlotAreaPosition.Y || empty.Bottom > area.PlotAreaPosition.Bottom() || empty.X < area.PlotAreaPosition.X || empty.Right > area.PlotAreaPosition.Right())
												{
													graph.SetClip(area.PlotAreaPosition.ToRectangleF());
													flag6 = true;
												}
												int shadowOffset = 0;
												if (shadow)
												{
													shadowOffset = item2.ShadowOffset;
												}
												if (!labels)
												{
													graph.StartHotRegion(dataPoint);
													graph.StartAnimation();
													graph.FillRectangleRel(empty, (!shadow) ? dataPoint.Color : Color.Transparent, dataPoint.BackHatchStyle, dataPoint.BackImage, dataPoint.BackImageMode, dataPoint.BackImageTransparentColor, dataPoint.BackImageAlign, dataPoint.BackGradientType, (!shadow) ? dataPoint.BackGradientEndColor : Color.Transparent, dataPoint.BorderColor, dataPoint.BorderWidth, dataPoint.BorderStyle, item2.ShadowColor, shadowOffset, PenAlignment.Inset, (!shadow) ? ChartGraphics.GetBarDrawingStyle(dataPoint) : BarDrawingStyle.Default, false);
													graph.StopAnimation();
													graph.EndHotRegion();
												}
												else
												{
													graph.StartAnimation();
													RectangleF rectangle = new RectangleF(empty.Location, empty.Size);
													if (flag6 && !flag4)
													{
														rectangle.Intersect(area.PlotAreaPosition.ToRectangleF());
													}
													this.DrawLabels(common, graph, area, dataPoint, k, item2, rectangle);
													graph.StopAnimation();
												}
												if (flag6)
												{
													graph.ResetClip();
												}
											}
											if (common.ProcessModeRegions && !shadow && !labels)
											{
												common.HotRegionsList.AddHotRegion(graph, empty, dataPoint, item2.Name, k);
												if (labels && !common.ProcessModePaint)
												{
													this.DrawLabels(common, graph, area, dataPoint, k, item2, empty);
												}
											}
											if (!selection)
											{
												common.EventsManager.OnPaint(item2, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
											}
										}
										if (axis2.Logarithmic)
										{
											num4 = Math.Pow(axis2.logarithmBase, num4);
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
							int num9 = text2.IndexOf("__", StringComparison.Ordinal);
							if (num9 >= 0)
							{
								text2 = text2.Substring(num9 + 2);
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

		public void DrawLabels(CommonElements common, ChartGraphics graph, ChartArea area, DataPoint point, int pointIndex, Series series, RectangleF rectangle)
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			Region clip = graph.Clip;
			graph.Clip = new Region();
			if (point.ShowLabelAsValue || point.Label.Length > 0)
			{
				double value = this.GetYValue(common, area, series, point, pointIndex, 0);
				if (this.hundredPercentStacked && point.LabelFormat.Length == 0)
				{
					value = Math.Round(value, 2);
				}
				string text;
				if (point.Label.Length == 0)
				{
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
					SizeF relativeSize = graph.GetRelativeSize(graph.MeasureString(text, point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
					BarValueLabelDrawingStyle barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Center;
					string text2 = "";
					if (point.IsAttributeSet("BarLabelStyle"))
					{
						text2 = ((DataPointAttributes)point)["BarLabelStyle"];
					}
					else if (series.IsAttributeSet("BarLabelStyle"))
					{
						text2 = ((DataPointAttributes)series)["BarLabelStyle"];
					}
					if (text2 != null && text2.Length > 0)
					{
						if (string.Compare(text2, "Left", StringComparison.OrdinalIgnoreCase) == 0)
						{
							barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Left;
						}
						else if (string.Compare(text2, "Right", StringComparison.OrdinalIgnoreCase) == 0)
						{
							barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Right;
						}
						else if (string.Compare(text2, "Center", StringComparison.OrdinalIgnoreCase) == 0)
						{
							barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Center;
						}
						else if (string.Compare(text2, "Outside", StringComparison.OrdinalIgnoreCase) == 0)
						{
							barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Outside;
						}
					}
					switch (barValueLabelDrawingStyle)
					{
					case BarValueLabelDrawingStyle.Left:
						pointF.X = (float)(rectangle.X + relativeSize.Width / 2.0);
						break;
					case BarValueLabelDrawingStyle.Right:
						pointF.X = (float)(rectangle.Right - relativeSize.Width / 2.0);
						break;
					case BarValueLabelDrawingStyle.Outside:
						pointF.X = (float)(rectangle.Right + relativeSize.Width / 2.0);
						break;
					}
					if (series.SmartLabels.Enabled)
					{
						bool markerOverlapping = series.SmartLabels.MarkerOverlapping;
						LabelAlignmentTypes movingDirection = series.SmartLabels.MovingDirection;
						series.SmartLabels.MarkerOverlapping = true;
						if (series.SmartLabels.MovingDirection == (LabelAlignmentTypes.Top | LabelAlignmentTypes.Bottom | LabelAlignmentTypes.Right | LabelAlignmentTypes.Left | LabelAlignmentTypes.TopLeft | LabelAlignmentTypes.TopRight | LabelAlignmentTypes.BottomLeft | LabelAlignmentTypes.BottomRight))
						{
							series.SmartLabels.MovingDirection = (LabelAlignmentTypes.Right | LabelAlignmentTypes.Left);
						}
						pointF = area.smartLabels.AdjustSmartLabelPosition(common, graph, area, series.SmartLabels, pointF, relativeSize, ref stringFormat, pointF, new SizeF(0f, 0f), LabelAlignmentTypes.Center);
						series.SmartLabels.MarkerOverlapping = markerOverlapping;
						series.SmartLabels.MovingDirection = movingDirection;
						angle = 0;
					}
					if (!pointF.IsEmpty)
					{
						RectangleF empty = RectangleF.Empty;
						SizeF size = new SizeF(relativeSize.Width, relativeSize.Height);
						size.Height += (float)(relativeSize.Height / 8.0);
						size.Width += size.Width / (float)text.Length;
						empty = new RectangleF((float)(pointF.X - size.Width / 2.0), (float)(pointF.Y - size.Height / 2.0 - relativeSize.Height / 10.0), size.Width, size.Height);
						empty = area.smartLabels.GetLabelPosition(graph, pointF, size, stringFormat, true);
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

		private void ProcessChartType3D(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, bool drawLabels, Series seriesToDraw)
		{
			ArrayList arrayList = null;
			arrayList = area.GetClusterSeriesNames(seriesToDraw.Name);
			common.DataManager.GetNumberOfPoints((string[])arrayList.ToArray(typeof(string)));
			ArrayList dataPointDrawingOrder = area.GetDataPointDrawingOrder(arrayList, this, selection, COPCoordinates.X | COPCoordinates.Y, new BarPointsDrawingOrderComparer(area, selection, COPCoordinates.X | COPCoordinates.Y), 0, false);
			if (!drawLabels)
			{
				foreach (object item in dataPointDrawingOrder)
				{
					DataPoint3D dataPoint3D = (DataPoint3D)item;
					DataPoint dataPoint = dataPoint3D.dataPoint;
					Series series = dataPoint.series;
					this.currentStackGroup = StackedColumnChart.GetSeriesStackGroupName(series);
					dataPoint.positionRel = new PointF(float.NaN, float.NaN);
					Axis axis = area.GetAxis(AxisName.X, series.XAxisType, series.XSubAxisName);
					Axis axis2 = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName);
					BarDrawingStyle barDrawingStyle = ChartGraphics.GetBarDrawingStyle(dataPoint);
					float num = 0.5f;
					float num2 = 0.5f;
					bool flag = true;
					bool flag2 = false;
					for (int i = 0; i < arrayList.Count; i++)
					{
						Series series2 = common.DataManager.Series[i];
						if (flag && dataPoint3D.index <= series2.Points.Count && series2.Points[dataPoint3D.index - 1].YValues[0] != 0.0)
						{
							flag = false;
							if (series2.Name == series.Name)
							{
								num = 0f;
							}
						}
						if (series2.Name == series.Name)
						{
							flag2 = true;
						}
						else if (dataPoint3D.index <= series2.Points.Count && series2.Points[dataPoint3D.index - 1].YValues[0] != 0.0)
						{
							flag2 = false;
						}
					}
					if (flag2)
					{
						num2 = 0f;
					}
					if (area.stackGroupNames != null && area.stackGroupNames.Count > 1 && area.Area3DStyle.Clustered)
					{
						string seriesStackGroupName = StackedColumnChart.GetSeriesStackGroupName(series);
						bool flag3 = true;
						bool flag4 = false;
						foreach (string item2 in arrayList)
						{
							Series series3 = common.DataManager.Series[item2];
							if (StackedColumnChart.GetSeriesStackGroupName(series3) == seriesStackGroupName)
							{
								if (flag3 && dataPoint3D.index < series3.Points.Count && series3.Points[dataPoint3D.index - 1].YValues[0] != 0.0)
								{
									flag3 = false;
									if (item2 == series.Name)
									{
										num = 0f;
									}
								}
								if (item2 == series.Name)
								{
									flag4 = true;
								}
								else if (dataPoint3D.index < series3.Points.Count && series3.Points[dataPoint3D.index - 1].YValues[0] != 0.0)
								{
									flag4 = false;
								}
							}
						}
						if (flag4)
						{
							num2 = 0f;
						}
					}
					double yValue = this.GetYValue(common, area, series, dataPoint3D.dataPoint, dataPoint3D.index - 1, 0);
					double yValue2 = yValue - this.GetYValue(common, area, series, dataPoint3D.dataPoint, dataPoint3D.index - 1, -1);
					yValue = axis2.GetLogValue(yValue);
					yValue2 = axis2.GetLogValue(yValue2);
					if (yValue2 > axis2.GetViewMaximum())
					{
						num2 = 0.5f;
						yValue2 = axis2.GetViewMaximum();
					}
					else if (yValue2 < axis2.GetViewMinimum())
					{
						num = 0.5f;
						yValue2 = axis2.GetViewMinimum();
					}
					if (yValue > axis2.GetViewMaximum())
					{
						num2 = 0.5f;
						yValue = axis2.GetViewMaximum();
					}
					else if (yValue < axis2.GetViewMinimum())
					{
						num = 0.5f;
						yValue = axis2.GetViewMinimum();
					}
					double linearPosition = axis2.GetLinearPosition(yValue);
					double linearPosition2 = axis2.GetLinearPosition(yValue2);
					double yValue3 = dataPoint3D.indexedSeries ? ((double)dataPoint3D.index) : dataPoint.XValue;
					yValue3 = axis.GetLogValue(yValue3);
					RectangleF empty = RectangleF.Empty;
					try
					{
						empty.Y = (float)(dataPoint3D.xPosition - dataPoint3D.width / 2.0);
						empty.Height = (float)dataPoint3D.width;
						if (linearPosition2 < linearPosition)
						{
							float num3 = num2;
							num2 = num;
							num = num3;
							empty.X = (float)linearPosition2;
							empty.Width = (float)linearPosition - empty.X;
						}
						else
						{
							empty.X = (float)linearPosition;
							empty.Width = (float)linearPosition2 - empty.X;
						}
					}
					catch (Exception)
					{
						continue;
					}
					dataPoint.positionRel = new PointF(empty.Right, (float)dataPoint3D.xPosition);
					if (!dataPoint.Empty)
					{
						GraphicsPath graphicsPath = null;
						if (!(yValue3 < axis.GetViewMinimum()) && !(yValue3 > axis.GetViewMaximum()) && (!(yValue < axis2.GetViewMinimum()) || !(yValue2 < axis2.GetViewMinimum())) && (!(yValue > axis2.GetViewMaximum()) || !(yValue2 > axis2.GetViewMaximum())))
						{
							bool flag5 = false;
							if (!(empty.Bottom <= area.PlotAreaPosition.Y) && !(empty.Y >= area.PlotAreaPosition.Bottom()))
							{
								if (empty.Y < area.PlotAreaPosition.Y)
								{
									empty.Height -= area.PlotAreaPosition.Y - empty.Y;
									empty.Y = area.PlotAreaPosition.Y;
								}
								if (empty.Bottom > area.PlotAreaPosition.Bottom())
								{
									empty.Height -= empty.Bottom - area.PlotAreaPosition.Bottom();
								}
								if (empty.Height < 0.0)
								{
									empty.Height = 0f;
								}
								if (empty.Height != 0.0 && empty.Width != 0.0)
								{
									DrawingOperationTypes drawingOperationTypes = DrawingOperationTypes.DrawElement;
									if (common.ProcessModeRegions)
									{
										drawingOperationTypes |= DrawingOperationTypes.CalcElementPath;
									}
									graph.StartHotRegion(dataPoint);
									graph.StartAnimation();
									graphicsPath = graph.Fill3DRectangle(empty, dataPoint3D.zPosition, dataPoint3D.depth, area.matrix3D, area.Area3DStyle.Light, dataPoint.Color, num, num2, dataPoint.BackHatchStyle, dataPoint.BackImage, dataPoint.BackImageMode, dataPoint.BackImageTransparentColor, dataPoint.BackImageAlign, dataPoint.BackGradientType, dataPoint.BackGradientEndColor, dataPoint.BorderColor, dataPoint.BorderWidth, dataPoint.BorderStyle, PenAlignment.Inset, barDrawingStyle, false, drawingOperationTypes);
									graph.StopAnimation();
									graph.EndHotRegion();
									if (flag5)
									{
										graph.ResetClip();
									}
									if (common.ProcessModeRegions && !drawLabels)
									{
										common.HotRegionsList.AddHotRegion(graphicsPath, false, graph, dataPoint, series.Name, dataPoint3D.index - 1);
									}
								}
							}
						}
					}
				}
			}
			if (drawLabels)
			{
				foreach (object item3 in dataPointDrawingOrder)
				{
					DataPoint3D dataPoint3D2 = (DataPoint3D)item3;
					DataPoint dataPoint2 = dataPoint3D2.dataPoint;
					Series series4 = dataPoint2.series;
					Axis axis3 = area.GetAxis(AxisName.X, series4.XAxisType, series4.XSubAxisName);
					Axis axis4 = area.GetAxis(AxisName.Y, series4.YAxisType, series4.YSubAxisName);
					double num4 = this.GetYValue(common, area, series4, dataPoint3D2.dataPoint, dataPoint3D2.index - 1, 0);
					if (axis4.Logarithmic)
					{
						num4 = Math.Log(num4, axis4.logarithmBase);
					}
					double yPosition = dataPoint3D2.yPosition;
					double num5 = dataPoint3D2.indexedSeries ? ((double)dataPoint3D2.index) : dataPoint2.XValue;
					double num6 = num4 - this.GetYValue(common, area, series4, dataPoint3D2.dataPoint, dataPoint3D2.index - 1, -1);
					double height = dataPoint3D2.height;
					RectangleF empty2 = RectangleF.Empty;
					try
					{
						empty2.Y = (float)(dataPoint3D2.xPosition - dataPoint3D2.width / 2.0);
						empty2.Height = (float)dataPoint3D2.width;
						if (height < yPosition)
						{
							empty2.X = (float)height;
							empty2.Width = (float)yPosition - empty2.X;
						}
						else
						{
							empty2.X = (float)yPosition;
							empty2.Width = (float)height - empty2.X;
						}
					}
					catch (Exception)
					{
						continue;
					}
					if (!dataPoint2.Empty)
					{
						if (axis4.Logarithmic)
						{
							num6 = Math.Log(num6, axis4.logarithmBase);
						}
						if (!(num5 < axis3.GetViewMinimum()) && !(num5 > axis3.GetViewMaximum()) && (!(num4 < axis4.GetViewMinimum()) || !(num6 < axis4.GetViewMinimum())) && (!(num4 > axis4.GetViewMaximum()) || !(num6 > axis4.GetViewMaximum())))
						{
							graph.StartAnimation();
							this.DrawLabels3D(area, axis4, graph, common, empty2, dataPoint3D2, series4, num6, yPosition, dataPoint3D2.width, dataPoint3D2.index - 1);
							graph.StopAnimation();
						}
					}
				}
			}
		}

		private void DrawLabels3D(ChartArea area, Axis hAxis, ChartGraphics graph, CommonElements common, RectangleF rectSize, DataPoint3D pointEx, Series ser, double barStartPosition, double barSize, double width, int pointIndex)
		{
			DataPoint dataPoint = pointEx.dataPoint;
			if (!ser.ShowLabelAsValue && !dataPoint.ShowLabelAsValue && dataPoint.Label.Length <= 0)
			{
				return;
			}
			RectangleF rectangleF = RectangleF.Empty;
			StringFormat stringFormat = new StringFormat();
			string text;
			if (dataPoint.Label.Length == 0)
			{
				double value = this.GetYValue(common, area, ser, dataPoint, pointIndex, -2);
				if (this.hundredPercentStacked && dataPoint.LabelFormat.Length == 0)
				{
					value = Math.Round(value, 2);
				}
				text = ValueConverter.FormatValue(ser.chart, dataPoint, value, dataPoint.LabelFormat, ser.YValueType, ChartElementType.DataPoint);
			}
			else
			{
				text = dataPoint.ReplaceKeywords(dataPoint.Label);
				if (ser.chart != null && ser.chart.LocalizeTextHandler != null)
				{
					text = ser.chart.LocalizeTextHandler(dataPoint, text, dataPoint.ElementId, ChartElementType.DataPoint);
				}
			}
			SizeF size = SizeF.Empty;
			if ((dataPoint.MarkerStyle != 0 || dataPoint.MarkerImage.Length > 0) && pointEx.index % ser.MarkerStep == 0)
			{
				if (dataPoint.MarkerImage.Length == 0)
				{
					size.Width = (float)dataPoint.MarkerSize;
					size.Height = (float)dataPoint.MarkerSize;
				}
				else
				{
					Image image = common.ImageLoader.LoadImage(dataPoint.MarkerImage);
					size.Width = (float)image.Width;
					size.Height = (float)image.Height;
				}
				size = graph.GetRelativeSize(size);
			}
			BarValueLabelDrawingStyle barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Center;
			string text2 = "";
			if (dataPoint.IsAttributeSet("BarLabelStyle"))
			{
				text2 = ((DataPointAttributes)dataPoint)["BarLabelStyle"];
			}
			else if (ser.IsAttributeSet("BarLabelStyle"))
			{
				text2 = ((DataPointAttributes)ser)["BarLabelStyle"];
			}
			if (text2 != null && text2.Length > 0)
			{
				if (string.Compare(text2, "Left", StringComparison.OrdinalIgnoreCase) == 0)
				{
					barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Left;
				}
				else if (string.Compare(text2, "Right", StringComparison.OrdinalIgnoreCase) == 0)
				{
					barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Right;
				}
				else if (string.Compare(text2, "Center", StringComparison.OrdinalIgnoreCase) == 0)
				{
					barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Center;
				}
				else if (string.Compare(text2, "Outside", StringComparison.OrdinalIgnoreCase) == 0)
				{
					barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Outside;
				}
			}
			bool flag = false;
			while (!flag)
			{
				stringFormat.Alignment = StringAlignment.Near;
				stringFormat.LineAlignment = StringAlignment.Center;
				if (barStartPosition < barSize)
				{
					rectangleF.X = rectSize.Right;
					rectangleF.Width = area.PlotAreaPosition.Right() - rectSize.Right;
				}
				else
				{
					rectangleF.X = area.PlotAreaPosition.X;
					rectangleF.Width = rectSize.X - area.PlotAreaPosition.X;
				}
				rectangleF.Y = (float)(rectSize.Y - (float)width / 2.0);
				rectangleF.Height = rectSize.Height + (float)width;
				switch (barValueLabelDrawingStyle)
				{
				case BarValueLabelDrawingStyle.Outside:
					if (!size.IsEmpty)
					{
						rectangleF.Width -= Math.Min(rectangleF.Width, (float)(size.Width / 2.0));
						if (barStartPosition < barSize)
						{
							rectangleF.X += Math.Min(rectangleF.Width, (float)(size.Width / 2.0));
						}
					}
					break;
				case BarValueLabelDrawingStyle.Left:
					rectangleF = rectSize;
					stringFormat.Alignment = StringAlignment.Near;
					break;
				case BarValueLabelDrawingStyle.Center:
					rectangleF = rectSize;
					stringFormat.Alignment = StringAlignment.Center;
					break;
				case BarValueLabelDrawingStyle.Right:
					rectangleF = rectSize;
					stringFormat.Alignment = StringAlignment.Far;
					if (!size.IsEmpty)
					{
						rectangleF.Width -= Math.Min(rectangleF.Width, (float)(size.Width / 2.0));
						if (barStartPosition >= barSize)
						{
							rectangleF.X += Math.Min(rectangleF.Width, (float)(size.Width / 2.0));
						}
					}
					break;
				}
				if (barStartPosition >= barSize)
				{
					if (stringFormat.Alignment == StringAlignment.Far)
					{
						stringFormat.Alignment = StringAlignment.Near;
					}
					else if (stringFormat.Alignment == StringAlignment.Near)
					{
						stringFormat.Alignment = StringAlignment.Far;
					}
				}
				flag = true;
			}
			SizeF sizeF = graph.MeasureStringRel(text, dataPoint.Font, new SizeF(rectangleF.Width, rectangleF.Height), stringFormat);
			PointF pointF = PointF.Empty;
			if (stringFormat.Alignment == StringAlignment.Near)
			{
				pointF.X = (float)(rectangleF.X + sizeF.Width / 2.0);
			}
			else if (stringFormat.Alignment == StringAlignment.Far)
			{
				pointF.X = (float)(rectangleF.Right - sizeF.Width / 2.0);
			}
			else
			{
				pointF.X = (float)((rectangleF.Left + rectangleF.Right) / 2.0);
			}
			if (stringFormat.LineAlignment == StringAlignment.Near)
			{
				pointF.Y = (float)(rectangleF.Top + sizeF.Height / 2.0);
			}
			else if (stringFormat.LineAlignment == StringAlignment.Far)
			{
				pointF.Y = (float)(rectangleF.Bottom - sizeF.Height / 2.0);
			}
			else
			{
				pointF.Y = (float)((rectangleF.Bottom + rectangleF.Top) / 2.0);
			}
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			int num = dataPoint.FontAngle;
			Point3D[] array = new Point3D[2]
			{
				new Point3D(pointF.X, pointF.Y, pointEx.zPosition + pointEx.depth),
				new Point3D((float)(pointF.X - 20.0), pointF.Y, pointEx.zPosition + pointEx.depth)
			};
			area.matrix3D.TransformPoints(array);
			pointF = array[0].PointF;
			if (num == 0 || num == 180)
			{
				array[0].PointF = graph.GetAbsolutePoint(array[0].PointF);
				array[1].PointF = graph.GetAbsolutePoint(array[1].PointF);
				float num2 = (float)Math.Atan((double)((array[1].Y - array[0].Y) / (array[1].X - array[0].X)));
				num2 = (float)Math.Round(num2 * 180.0 / 3.1415927410125732);
				num += (int)num2;
			}
			SizeF labelSize = SizeF.Empty;
			if (ser.SmartLabels.Enabled)
			{
				labelSize = graph.GetRelativeSize(graph.MeasureString(text, dataPoint.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
				bool markerOverlapping = ser.SmartLabels.MarkerOverlapping;
				LabelAlignmentTypes movingDirection = ser.SmartLabels.MovingDirection;
				ser.SmartLabels.MarkerOverlapping = true;
				if (ser.SmartLabels.MovingDirection == (LabelAlignmentTypes.Top | LabelAlignmentTypes.Bottom | LabelAlignmentTypes.Right | LabelAlignmentTypes.Left | LabelAlignmentTypes.TopLeft | LabelAlignmentTypes.TopRight | LabelAlignmentTypes.BottomLeft | LabelAlignmentTypes.BottomRight))
				{
					ser.SmartLabels.MovingDirection = (LabelAlignmentTypes.Right | LabelAlignmentTypes.Left);
				}
				pointF = area.smartLabels.AdjustSmartLabelPosition(common, graph, area, ser.SmartLabels, pointF, labelSize, ref stringFormat, pointF, new SizeF(0f, 0f), LabelAlignmentTypes.Center);
				ser.SmartLabels.MarkerOverlapping = markerOverlapping;
				ser.SmartLabels.MovingDirection = movingDirection;
				num = 0;
			}
			if (!pointF.IsEmpty)
			{
				if (labelSize.IsEmpty)
				{
					labelSize = graph.GetRelativeSize(graph.MeasureString(text, dataPoint.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
				}
				RectangleF empty = RectangleF.Empty;
				SizeF sizeF2 = new SizeF(labelSize.Width, labelSize.Height);
				sizeF2.Height += (float)(labelSize.Height / 8.0);
				sizeF2.Width += sizeF2.Width / (float)text.Length;
				empty = new RectangleF((float)(pointF.X - sizeF2.Width / 2.0), (float)(pointF.Y - sizeF2.Height / 2.0 - labelSize.Height / 10.0), sizeF2.Width, sizeF2.Height);
				graph.DrawPointLabelStringRel(common, text, dataPoint.Font, new SolidBrush(dataPoint.FontColor), pointF, stringFormat, num, empty, dataPoint.LabelBackColor, dataPoint.LabelBorderColor, dataPoint.LabelBorderWidth, dataPoint.LabelBorderStyle, ser, dataPoint, pointIndex);
			}
		}

		public void AddSmartLabelMarkerPositions(CommonElements common, ChartArea area, Series series, ArrayList list)
		{
		}
	}
}
