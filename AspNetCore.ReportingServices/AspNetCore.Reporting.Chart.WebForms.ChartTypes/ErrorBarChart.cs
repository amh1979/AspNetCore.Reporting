using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class ErrorBarChart : IChartType
	{
		protected Axis vAxis;

		protected Axis hAxis;

		public virtual string Name
		{
			get
			{
				return "ErrorBar";
			}
		}

		public virtual bool Stacked
		{
			get
			{
				return false;
			}
		}

		public virtual bool SupportStackedGroups
		{
			get
			{
				return false;
			}
		}

		public bool StackSign
		{
			get
			{
				return false;
			}
		}

		public virtual bool RequireAxes
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

		public virtual bool ZeroCrossing
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

		public virtual bool ExtraYValuesConnectedToYAxis
		{
			get
			{
				return true;
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

		public virtual int YValuesPerPoint
		{
			get
			{
				return 3;
			}
		}

		public virtual LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Line;
		}

		public virtual Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}

		public virtual void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			this.ProcessChartType(false, graph, common, area, seriesToDraw);
		}

		protected virtual void ProcessChartType(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			if (area.Area3DStyle.Enable3D)
			{
				this.ProcessChartType3D(selection, graph, common, area, seriesToDraw);
			}
			else
			{
				ArrayList seriesFromChartType = area.GetSeriesFromChartType(this.Name);
				bool flag = area.IndexedSeries((string[])seriesFromChartType.ToArray(typeof(string)));
				int num = 0;
				foreach (Series item in common.DataManager.Series)
				{
					if (string.Compare(item.ChartTypeName, this.Name, StringComparison.OrdinalIgnoreCase) == 0 && !(item.ChartArea != area.Name) && item.IsVisible())
					{
						this.hAxis = area.GetAxis(AxisName.X, item.XAxisType, item.XSubAxisName);
						this.vAxis = area.GetAxis(AxisName.Y, item.YAxisType, item.YSubAxisName);
						double interval = flag ? 1.0 : area.GetPointsInterval(this.hAxis.Logarithmic, this.hAxis.logarithmBase);
						float num2 = (float)item.GetPointWidth(graph, this.hAxis, interval, 0.4);
						float num3 = num2;
						int num4 = 1;
						int num5 = 0;
						bool flag2 = false;
						string empty = string.Empty;
						bool flag3 = false;
						if (item.IsAttributeSet("ErrorBarSeries"))
						{
							empty = ((DataPointAttributes)item)["ErrorBarSeries"];
							int num6 = empty.IndexOf(":", StringComparison.Ordinal);
							if (num6 >= 0)
							{
								empty = empty.Substring(0, num6);
							}
							string chartTypeName = common.DataManager.Series[empty].ChartTypeName;
							ChartArea chartArea = common.chartAreaCollection[common.DataManager.Series[empty].ChartArea];
							ArrayList seriesFromChartType2 = chartArea.GetSeriesFromChartType(chartTypeName);
							foreach (string item2 in seriesFromChartType2)
							{
								if (item2 == empty)
								{
									break;
								}
								num5++;
							}
							flag3 = false;
							if (string.Compare(chartTypeName, "Column", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(chartTypeName, "RangeColumn", StringComparison.OrdinalIgnoreCase) == 0)
							{
								flag3 = true;
							}
							foreach (string item3 in seriesFromChartType2)
							{
								if (common.DataManager.Series[item3].IsAttributeSet("DrawSideBySide"))
								{
									string strA = ((DataPointAttributes)common.DataManager.Series[item3])["DrawSideBySide"];
									if (string.Compare(strA, "False", StringComparison.OrdinalIgnoreCase) == 0)
									{
										flag3 = false;
									}
									else if (string.Compare(strA, "True", StringComparison.OrdinalIgnoreCase) == 0)
									{
										flag3 = true;
									}
									else if (string.Compare(strA, "Auto", StringComparison.OrdinalIgnoreCase) != 0)
									{
										throw new InvalidOperationException(SR.ExceptionAttributeDrawSideBySideInvalid);
									}
								}
							}
							if (flag3)
							{
								num4 = seriesFromChartType2.Count;
								num2 /= (float)num4;
								flag2 = true;
								if (!flag)
								{
									((ChartAreaAxes)area).GetPointsInterval(seriesFromChartType2, this.hAxis.Logarithmic, this.hAxis.logarithmBase, true, out flag2);
								}
								num3 = (float)common.DataManager.Series[empty].GetPointWidth(graph, this.hAxis, interval, 0.8) / (float)num4;
							}
						}
						if (!flag3 && item.IsAttributeSet("DrawSideBySide"))
						{
							string strA2 = ((DataPointAttributes)item)["DrawSideBySide"];
							if (string.Compare(strA2, "False", StringComparison.OrdinalIgnoreCase) == 0)
							{
								flag2 = false;
							}
							else if (string.Compare(strA2, "True", StringComparison.OrdinalIgnoreCase) == 0)
							{
								flag2 = true;
								num4 = seriesFromChartType.Count;
								num5 = num;
								num2 /= (float)num4;
								num3 = (float)item.GetPointWidth(graph, this.hAxis, interval, 0.8) / (float)num4;
							}
							else if (string.Compare(strA2, "Auto", StringComparison.OrdinalIgnoreCase) != 0)
							{
								throw new InvalidOperationException(SR.ExceptionAttributeDrawSideBySideInvalid);
							}
						}
						if (!selection)
						{
							common.EventsManager.OnBackPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
						}
						int num7 = 1;
						foreach (DataPoint point in item.Points)
						{
							if (point.YValues.Length < this.YValuesPerPoint)
							{
								throw new InvalidOperationException(SR.ExceptionChartTypeRequiresYValues(this.Name, this.YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
							}
							point.positionRel = new PointF(float.NaN, float.NaN);
							float num8 = 0f;
							double num9 = point.XValue;
							if (flag)
							{
								num9 = (double)num7;
							}
							num8 = ((!flag2) ? ((float)this.hAxis.GetPosition(num9)) : ((float)(this.hAxis.GetPosition(num9) - (double)num3 * (double)num4 / 2.0 + num3 / 2.0 + (double)((float)num5 * num3))));
							double logValue = this.vAxis.GetLogValue(point.YValues[1]);
							double logValue2 = this.vAxis.GetLogValue(point.YValues[2]);
							num9 = this.hAxis.GetLogValue(num9);
							ErrorBarStyle barStyle;
							double num11;
							double num10;
							if (!(num9 < this.hAxis.GetViewMinimum()) && !(num9 > this.hAxis.GetViewMaximum()) && (!(logValue < this.vAxis.GetViewMinimum()) || !(logValue2 < this.vAxis.GetViewMinimum())) && (!(logValue > this.vAxis.GetViewMaximum()) || !(logValue2 > this.vAxis.GetViewMaximum())))
							{
								num10 = this.vAxis.GetLogValue(point.YValues[1]);
								num11 = this.vAxis.GetLogValue(point.YValues[2]);
								barStyle = ErrorBarStyle.Both;
								if (point.IsAttributeSet("ErrorBarStyle") || item.IsAttributeSet("ErrorBarStyle"))
								{
									string strA3 = ((DataPointAttributes)item)["ErrorBarStyle"];
									if (point.IsAttributeSet("ErrorBarStyle"))
									{
										strA3 = ((DataPointAttributes)point)["ErrorBarStyle"];
									}
									if (string.Compare(strA3, "Both", StringComparison.OrdinalIgnoreCase) != 0)
									{
										if (string.Compare(strA3, "UpperError", StringComparison.OrdinalIgnoreCase) != 0)
										{
											if (string.Compare(strA3, "LowerError", StringComparison.OrdinalIgnoreCase) == 0)
											{
												barStyle = ErrorBarStyle.LowerError;
												num10 = this.vAxis.GetLogValue(point.YValues[1]);
												num11 = this.vAxis.GetLogValue(point.YValues[0]);
												goto IL_06a1;
											}
											throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(((DataPointAttributes)point)["ErrorBarStyle"], "ErrorBarStyle"));
										}
										barStyle = ErrorBarStyle.UpperError;
										num10 = this.vAxis.GetLogValue(point.YValues[0]);
										num11 = this.vAxis.GetLogValue(point.YValues[2]);
									}
								}
								goto IL_06a1;
							}
							num7++;
							continue;
							IL_06a1:
							if (num11 > this.vAxis.GetViewMaximum())
							{
								num11 = this.vAxis.GetViewMaximum();
							}
							if (num11 < this.vAxis.GetViewMinimum())
							{
								num11 = this.vAxis.GetViewMinimum();
							}
							num11 = (double)(float)this.vAxis.GetLinearPosition(num11);
							if (num10 > this.vAxis.GetViewMaximum())
							{
								num10 = this.vAxis.GetViewMaximum();
							}
							if (num10 < this.vAxis.GetViewMinimum())
							{
								num10 = this.vAxis.GetViewMinimum();
							}
							num10 = this.vAxis.GetLinearPosition(num10);
							point.positionRel = new PointF(num8, (float)Math.Min(num11, num10));
							if (common.ProcessModePaint)
							{
								bool flag4 = false;
								if (num9 == this.hAxis.GetViewMinimum() || num9 == this.hAxis.GetViewMaximum())
								{
									graph.SetClip(area.PlotAreaPosition.ToRectangleF());
									flag4 = true;
								}
								graph.StartAnimation();
								graph.StartHotRegion(point);
								graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, new PointF(num8, (float)num11), new PointF(num8, (float)num10), item.ShadowColor, item.ShadowOffset);
								this.DrawErrorBarMarks(graph, barStyle, area, item, point, num8, num2);
								graph.StopAnimation();
								graph.EndHotRegion();
								if (flag4)
								{
									graph.ResetClip();
								}
							}
							if (common.ProcessModeRegions)
							{
								RectangleF empty2 = RectangleF.Empty;
								empty2.X = (float)(num8 - num2 / 2.0);
								empty2.Y = (float)Math.Min(num11, num10);
								empty2.Width = num2;
								empty2.Height = (float)Math.Max(num11, num10) - empty2.Y;
								common.HotRegionsList.AddHotRegion(graph, empty2, point, item.Name, num7 - 1);
							}
							num7++;
						}
						if (!selection)
						{
							num7 = 1;
							foreach (DataPoint point2 in item.Points)
							{
								float num12 = 0f;
								double num13 = point2.XValue;
								if (flag)
								{
									num13 = (double)num7;
									num12 = (float)(this.hAxis.GetPosition((double)num7) - (double)num3 * (double)num4 / 2.0 + num3 / 2.0 + (double)((float)num5 * num3));
								}
								else
								{
									num12 = ((!flag2) ? ((float)this.hAxis.GetPosition(num13)) : ((float)(this.hAxis.GetPosition(num13) - (double)num3 * (double)num4 / 2.0 + num3 / 2.0 + (double)((float)num5 * num3))));
								}
								double logValue3 = this.vAxis.GetLogValue(point2.YValues[1]);
								double logValue4 = this.vAxis.GetLogValue(point2.YValues[2]);
								num13 = this.hAxis.GetLogValue(num13);
								double num14;
								double num15;
								if (!(num13 < this.hAxis.GetViewMinimum()) && !(num13 > this.hAxis.GetViewMaximum()) && (!(logValue3 < this.vAxis.GetViewMinimum()) || !(logValue4 < this.vAxis.GetViewMinimum())) && (!(logValue3 > this.vAxis.GetViewMaximum()) || !(logValue4 > this.vAxis.GetViewMaximum())))
								{
									num14 = this.vAxis.GetLogValue(point2.YValues[1]);
									num15 = this.vAxis.GetLogValue(point2.YValues[2]);
									if (point2.IsAttributeSet("ErrorBarStyle") || item.IsAttributeSet("ErrorBarStyle"))
									{
										string strA4 = ((DataPointAttributes)item)["ErrorBarStyle"];
										if (point2.IsAttributeSet("ErrorBarStyle"))
										{
											strA4 = ((DataPointAttributes)point2)["ErrorBarStyle"];
										}
										if (string.Compare(strA4, "Both", StringComparison.OrdinalIgnoreCase) != 0)
										{
											if (string.Compare(strA4, "UpperError", StringComparison.OrdinalIgnoreCase) != 0)
											{
												if (string.Compare(strA4, "LowerError", StringComparison.OrdinalIgnoreCase) == 0)
												{
													num15 = this.vAxis.GetLogValue(point2.YValues[1]);
													num14 = this.vAxis.GetLogValue(point2.YValues[0]);
													goto IL_0b10;
												}
												throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(((DataPointAttributes)point2)["ErrorBarStyle"], "ErrorBarStyle"));
											}
											num15 = this.vAxis.GetLogValue(point2.YValues[0]);
											num14 = this.vAxis.GetLogValue(point2.YValues[2]);
										}
									}
									goto IL_0b10;
								}
								num7++;
								continue;
								IL_0b10:
								if (num14 > this.vAxis.GetViewMaximum())
								{
									num14 = this.vAxis.GetViewMaximum();
								}
								if (num14 < this.vAxis.GetViewMinimum())
								{
									num14 = this.vAxis.GetViewMinimum();
								}
								num14 = (double)(float)this.vAxis.GetLinearPosition(num14);
								if (num15 > this.vAxis.GetViewMaximum())
								{
									num15 = this.vAxis.GetViewMaximum();
								}
								if (num15 < this.vAxis.GetViewMinimum())
								{
									num15 = this.vAxis.GetViewMinimum();
								}
								num15 = this.vAxis.GetLinearPosition(num15);
								graph.StartAnimation();
								graph.StartHotRegion(point2, true);
								this.DrawLabel(common, area, graph, item, point2, new PointF(num12, (float)Math.Min(num14, num15)), num7);
								graph.EndHotRegion();
								graph.StopAnimation();
								num7++;
							}
						}
						if (!selection)
						{
							common.EventsManager.OnPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
						}
						num++;
					}
				}
			}
		}

		protected virtual void DrawErrorBarMarks(ChartGraphics graph, ErrorBarStyle barStyle, ChartArea area, Series ser, DataPoint point, float xPosition, float width)
		{
			double num = 0.0;
			string empty = string.Empty;
			if (barStyle == ErrorBarStyle.Both || barStyle == ErrorBarStyle.LowerError)
			{
				num = this.vAxis.GetLogValue(point.YValues[1]);
				empty = "LINE";
				if (point.MarkerStyle != 0)
				{
					empty = point.MarkerStyle.ToString();
				}
				this.DrawErrorBarSingleMarker(graph, area, point, empty, xPosition, (float)num, 0f, width, false);
			}
			if (barStyle == ErrorBarStyle.Both || barStyle == ErrorBarStyle.UpperError)
			{
				num = this.vAxis.GetLogValue(point.YValues[2]);
				empty = "LINE";
				if (point.MarkerStyle != 0)
				{
					empty = point.MarkerStyle.ToString();
				}
				this.DrawErrorBarSingleMarker(graph, area, point, empty, xPosition, (float)num, 0f, width, false);
			}
			if (!point.IsAttributeSet("ErrorBarCenterMarkerStyle") && !point.series.IsAttributeSet("ErrorBarCenterMarkerStyle"))
			{
				return;
			}
			num = this.vAxis.GetLogValue(point.YValues[0]);
			empty = ((DataPointAttributes)point.series)["ErrorBarCenterMarkerStyle"];
			if (point.IsAttributeSet("ErrorBarCenterMarkerStyle"))
			{
				empty = ((DataPointAttributes)point)["ErrorBarCenterMarkerStyle"];
			}
			empty = empty.ToUpper(CultureInfo.InvariantCulture);
			this.DrawErrorBarSingleMarker(graph, area, point, empty, xPosition, (float)num, 0f, width, false);
		}

		private void DrawErrorBarSingleMarker(ChartGraphics graph, ChartArea area, DataPoint point, string markerStyle, float xPosition, float yPosition, float zPosition, float width, bool draw3D)
		{
			markerStyle = markerStyle.ToUpper(CultureInfo.InvariantCulture);
			if (markerStyle.Length > 0 && string.Compare(markerStyle, "None", StringComparison.OrdinalIgnoreCase) != 0 && !((double)yPosition > this.vAxis.GetViewMaximum()) && !((double)yPosition < this.vAxis.GetViewMinimum()))
			{
				yPosition = (float)this.vAxis.GetLinearPosition((double)yPosition);
				if (draw3D)
				{
					Point3D[] array = new Point3D[1]
					{
						new Point3D(xPosition, yPosition, zPosition)
					};
					area.matrix3D.TransformPoints(array);
					xPosition = array[0].X;
					yPosition = array[0].Y;
				}
				if (string.Compare(markerStyle, "Line", StringComparison.OrdinalIgnoreCase) == 0)
				{
					graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, new PointF((float)(xPosition - width / 2.0), yPosition), new PointF((float)(xPosition + width / 2.0), yPosition), (point.series != null) ? point.series.ShadowColor : Color.Empty, (point.series != null) ? point.series.ShadowOffset : 0);
				}
				else
				{
					MarkerStyle markerStyle2 = (MarkerStyle)Enum.Parse(typeof(MarkerStyle), markerStyle, true);
					SizeF markerSize = this.GetMarkerSize(graph, area.Common, area, point, point.MarkerSize, point.MarkerImage);
					Color markerColor = (point.MarkerColor == Color.Empty) ? point.Color : point.MarkerColor;
					graph.DrawMarkerRel(new PointF(xPosition, yPosition), markerStyle2, point.MarkerSize, markerColor, point.MarkerBorderColor, point.MarkerBorderWidth, point.MarkerImage, point.MarkerImageTransparentColor, (point.series != null) ? point.series.ShadowOffset : 0, (point.series != null) ? point.series.ShadowColor : Color.Empty, new RectangleF(xPosition, yPosition, markerSize.Width, markerSize.Height));
				}
			}
		}

		protected virtual SizeF GetMarkerSize(ChartGraphics graph, CommonElements common, ChartArea area, DataPoint point, int markerSize, string markerImage)
		{
			SizeF result = new SizeF((float)markerSize, (float)markerSize);
			if (markerImage.Length > 0)
			{
				common.ImageLoader.GetAdjustedImageSize(markerImage, graph.Graphics, ref result);
			}
			return result;
		}

		protected virtual void DrawLabel(CommonElements common, ChartArea area, ChartGraphics graph, Series ser, DataPoint point, PointF position, int pointIndex)
		{
			if (!ser.ShowLabelAsValue && !point.ShowLabelAsValue && point.Label.Length <= 0)
			{
				return;
			}
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Near;
			stringFormat.LineAlignment = StringAlignment.Center;
			if (point.FontAngle == 0)
			{
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Far;
			}
			string text;
			if (point.Label.Length == 0)
			{
				text = ValueConverter.FormatValue(ser.chart, point, point.YValues[0], point.LabelFormat, ser.YValueType, ChartElementType.DataPoint);
			}
			else
			{
				text = point.ReplaceKeywords(point.Label);
				if (ser.chart != null && ser.chart.LocalizeTextHandler != null)
				{
					text = ser.chart.LocalizeTextHandler(point, text, point.ElementId, ChartElementType.DataPoint);
				}
			}
			SizeF markerSize = new SizeF(0f, 0f);
			if (point.MarkerStyle != 0)
			{
				markerSize = graph.GetRelativeSize(new SizeF((float)point.MarkerSize, (float)point.MarkerSize));
				position.Y -= (float)(markerSize.Height / 2.0);
			}
			int angle = point.FontAngle;
			if (text.Trim().Length != 0)
			{
				SizeF labelSize = SizeF.Empty;
				if (ser.SmartLabels.Enabled)
				{
					labelSize = graph.GetRelativeSize(graph.MeasureString(text, point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
					position = area.smartLabels.AdjustSmartLabelPosition(common, graph, area, ser.SmartLabels, position, labelSize, ref stringFormat, position, markerSize, LabelAlignmentTypes.Top);
					angle = 0;
				}
				if (!position.IsEmpty)
				{
					if (labelSize.IsEmpty)
					{
						labelSize = graph.GetRelativeSize(graph.MeasureString(text, point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
					}
					RectangleF empty = RectangleF.Empty;
					SizeF size = new SizeF(labelSize.Width, labelSize.Height);
					size.Height += (float)(labelSize.Height / 8.0);
					size.Width += size.Width / (float)text.Length;
					empty = PointChart.GetLabelPosition(graph, position, size, stringFormat, true);
					graph.DrawPointLabelStringRel(common, text, point.Font, new SolidBrush(point.FontColor), position, stringFormat, angle, empty, point.LabelBackColor, point.LabelBorderColor, point.LabelBorderWidth, point.LabelBorderStyle, ser, point, pointIndex - 1);
				}
			}
		}

		protected virtual void ProcessChartType3D(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			ArrayList seriesFromChartType = area.GetSeriesFromChartType(this.Name);
			bool flag = area.IndexedSeries((string[])seriesFromChartType.ToArray(typeof(string)));
			foreach (Series item in common.DataManager.Series)
			{
				if (string.Compare(item.ChartTypeName, this.Name, StringComparison.OrdinalIgnoreCase) == 0 && !(item.ChartArea != area.Name) && item.IsVisible())
				{
					if (item.YValuesPerPoint < 3)
					{
						throw new ArgumentException(SR.ExceptionChartTypeRequiresYValues("ErrorBar", 3.ToString(CultureInfo.CurrentCulture)));
					}
					this.hAxis = area.GetAxis(AxisName.X, item.XAxisType, item.XSubAxisName);
					this.vAxis = area.GetAxis(AxisName.Y, item.YAxisType, item.YSubAxisName);
					double interval = flag ? 1.0 : area.GetPointsInterval(this.hAxis.Logarithmic, this.hAxis.logarithmBase);
					float num = (float)item.GetPointWidth(graph, this.hAxis, interval, 0.4);
					float num2 = num;
					int num3 = 1;
					int num4 = 0;
					bool flag2 = false;
					if (item.IsAttributeSet("ErrorBarSeries"))
					{
						string text = ((DataPointAttributes)item)["ErrorBarSeries"];
						int num5 = text.IndexOf(":", StringComparison.Ordinal);
						if (num5 >= 0)
						{
							text = text.Substring(0, num5);
						}
						string chartTypeName = common.DataManager.Series[text].ChartTypeName;
						ArrayList seriesFromChartType2 = area.GetSeriesFromChartType(chartTypeName);
						foreach (string item2 in seriesFromChartType2)
						{
							if (item2 == text)
							{
								break;
							}
							num4++;
						}
						bool flag3 = false;
						if (string.Compare(chartTypeName, "Column", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(chartTypeName, "RangeColumn", StringComparison.OrdinalIgnoreCase) == 0)
						{
							flag3 = true;
						}
						foreach (string item3 in seriesFromChartType2)
						{
							if (common.DataManager.Series[item3].IsAttributeSet("DrawSideBySide"))
							{
								text = ((DataPointAttributes)common.DataManager.Series[item3])["DrawSideBySide"];
								if (string.Compare(text, "False", StringComparison.OrdinalIgnoreCase) == 0)
								{
									flag3 = false;
								}
								else if (string.Compare(text, "True", StringComparison.OrdinalIgnoreCase) == 0)
								{
									flag3 = true;
								}
								else if (string.Compare(text, "Auto", StringComparison.OrdinalIgnoreCase) != 0)
								{
									throw new InvalidOperationException(SR.ExceptionAttributeDrawSideBySideInvalid);
								}
							}
						}
						if (flag3)
						{
							num3 = seriesFromChartType2.Count;
							num /= (float)num3;
							if (!flag)
							{
								((ChartAreaAxes)area).GetPointsInterval(seriesFromChartType2, this.hAxis.Logarithmic, this.hAxis.logarithmBase, true, out flag2);
							}
						}
					}
					if (!selection)
					{
						common.EventsManager.OnBackPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
					}
					float num6 = default(float);
					float num7 = default(float);
					((ChartArea3D)area).GetSeriesZPositionAndDepth(item, out num6, out num7);
					int num8 = 1;
					foreach (DataPoint point in item.Points)
					{
						if (point.YValues.Length < this.YValuesPerPoint)
						{
							throw new InvalidOperationException(SR.ExceptionChartTypeRequiresYValues(this.Name, this.YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
						}
						point.positionRel = new PointF(float.NaN, float.NaN);
						float num9 = 0f;
						double num10 = point.XValue;
						if (flag)
						{
							num10 = (double)num8;
							num9 = (float)(this.hAxis.GetPosition((double)num8) - (double)num2 * (double)num3 / 2.0 + num2 / 2.0 + (double)((float)num4 * num2));
						}
						else
						{
							num9 = ((!flag2) ? ((float)this.hAxis.GetPosition(num10)) : ((float)(this.hAxis.GetPosition(num10) - (double)num2 * (double)num3 / 2.0 + num2 / 2.0 + (double)((float)num4 * num2))));
						}
						double logValue = this.vAxis.GetLogValue(point.YValues[1]);
						double logValue2 = this.vAxis.GetLogValue(point.YValues[2]);
						num10 = this.hAxis.GetLogValue(num10);
						ErrorBarStyle barStyle;
						double num11;
						double num12;
						if (!(num10 < this.hAxis.GetViewMinimum()) && !(num10 > this.hAxis.GetViewMaximum()) && (!(logValue < this.vAxis.GetViewMinimum()) || !(logValue2 < this.vAxis.GetViewMinimum())) && (!(logValue > this.vAxis.GetViewMaximum()) || !(logValue2 > this.vAxis.GetViewMaximum())))
						{
							num11 = this.vAxis.GetLogValue(point.YValues[2]);
							num12 = this.vAxis.GetLogValue(point.YValues[1]);
							barStyle = ErrorBarStyle.Both;
							if (point.IsAttributeSet("ErrorBarStyle") || item.IsAttributeSet("ErrorBarStyle"))
							{
								string strA = ((DataPointAttributes)item)["ErrorBarStyle"];
								if (point.IsAttributeSet("ErrorBarStyle"))
								{
									strA = ((DataPointAttributes)point)["ErrorBarStyle"];
								}
								if (string.Compare(strA, "Both", StringComparison.OrdinalIgnoreCase) != 0)
								{
									if (string.Compare(strA, "UpperError", StringComparison.OrdinalIgnoreCase) != 0)
									{
										if (string.Compare(strA, "LowerError", StringComparison.OrdinalIgnoreCase) == 0)
										{
											barStyle = ErrorBarStyle.LowerError;
											num12 = this.vAxis.GetLogValue(point.YValues[1]);
											num11 = this.vAxis.GetLogValue(point.YValues[0]);
											goto IL_05f3;
										}
										throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(((DataPointAttributes)point)["ErrorBarStyle"], "ErrorBarStyle"));
									}
									barStyle = ErrorBarStyle.UpperError;
									num12 = this.vAxis.GetLogValue(point.YValues[0]);
									num11 = this.vAxis.GetLogValue(point.YValues[2]);
								}
							}
							goto IL_05f3;
						}
						num8++;
						continue;
						IL_05f3:
						if (num11 > this.vAxis.GetViewMaximum())
						{
							num11 = this.vAxis.GetViewMaximum();
						}
						if (num11 < this.vAxis.GetViewMinimum())
						{
							num11 = this.vAxis.GetViewMinimum();
						}
						num11 = (double)(float)this.vAxis.GetLinearPosition(num11);
						if (num12 > this.vAxis.GetViewMaximum())
						{
							num12 = this.vAxis.GetViewMaximum();
						}
						if (num12 < this.vAxis.GetViewMinimum())
						{
							num12 = this.vAxis.GetViewMinimum();
						}
						num12 = this.vAxis.GetLinearPosition(num12);
						point.positionRel = new PointF(num9, (float)Math.Min(num11, num12));
						Point3D[] array = new Point3D[2]
						{
							new Point3D(num9, (float)num11, (float)(num7 + num6 / 2.0)),
							new Point3D(num9, (float)num12, (float)(num7 + num6 / 2.0))
						};
						area.matrix3D.TransformPoints(array);
						if (common.ProcessModePaint)
						{
							bool flag4 = false;
							if (num10 == this.hAxis.GetViewMinimum() || num10 == this.hAxis.GetViewMaximum())
							{
								graph.SetClip(area.PlotAreaPosition.ToRectangleF());
								flag4 = true;
							}
							graph.StartAnimation();
							graph.StartHotRegion(point);
							graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, array[0].PointF, array[1].PointF, item.ShadowColor, item.ShadowOffset);
							this.DrawErrorBarMarks3D(graph, barStyle, area, item, point, num9, num, num7, num6);
							num9 = array[0].X;
							num11 = (double)array[0].Y;
							num12 = (double)array[1].Y;
							graph.EndHotRegion();
							graph.StopAnimation();
							if (flag4)
							{
								graph.ResetClip();
							}
						}
						if (common.ProcessModeRegions)
						{
							num9 = array[0].X;
							num11 = (double)array[0].Y;
							num12 = (double)array[1].Y;
							RectangleF empty = RectangleF.Empty;
							empty.X = (float)(num9 - num / 2.0);
							empty.Y = (float)Math.Min(num11, num12);
							empty.Width = num;
							empty.Height = (float)Math.Max(num11, num12) - empty.Y;
							common.HotRegionsList.AddHotRegion(graph, empty, point, item.Name, num8 - 1);
						}
						num8++;
					}
					if (!selection)
					{
						num8 = 1;
						foreach (DataPoint point2 in item.Points)
						{
							float num13 = 0f;
							double num14 = point2.XValue;
							if (flag)
							{
								num14 = (double)num8;
								num13 = (float)(this.hAxis.GetPosition((double)num8) - (double)num2 * (double)num3 / 2.0 + num2 / 2.0 + (double)((float)num4 * num2));
							}
							else
							{
								num13 = ((!flag2) ? ((float)this.hAxis.GetPosition(num14)) : ((float)(this.hAxis.GetPosition(num14) - (double)num2 * (double)num3 / 2.0 + num2 / 2.0 + (double)((float)num4 * num2))));
							}
							double logValue3 = this.vAxis.GetLogValue(point2.YValues[1]);
							double logValue4 = this.vAxis.GetLogValue(point2.YValues[2]);
							num14 = this.hAxis.GetLogValue(num14);
							double num15;
							double num16;
							if (!(num14 < this.hAxis.GetViewMinimum()) && !(num14 > this.hAxis.GetViewMaximum()) && (!(logValue3 < this.vAxis.GetViewMinimum()) || !(logValue4 < this.vAxis.GetViewMinimum())) && (!(logValue3 > this.vAxis.GetViewMaximum()) || !(logValue4 > this.vAxis.GetViewMaximum())))
							{
								num15 = this.vAxis.GetLogValue(point2.YValues[2]);
								num16 = this.vAxis.GetLogValue(point2.YValues[1]);
								if (point2.IsAttributeSet("ErrorBarStyle") || item.IsAttributeSet("ErrorBarStyle"))
								{
									string strA2 = ((DataPointAttributes)item)["ErrorBarStyle"];
									if (point2.IsAttributeSet("ErrorBarStyle"))
									{
										strA2 = ((DataPointAttributes)point2)["ErrorBarStyle"];
									}
									if (string.Compare(strA2, "Both", StringComparison.OrdinalIgnoreCase) != 0)
									{
										if (string.Compare(strA2, "UpperError", StringComparison.OrdinalIgnoreCase) != 0)
										{
											if (string.Compare(strA2, "LowerError", StringComparison.OrdinalIgnoreCase) == 0)
											{
												num16 = this.vAxis.GetLogValue(point2.YValues[1]);
												num15 = this.vAxis.GetLogValue(point2.YValues[0]);
												goto IL_0af5;
											}
											throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(((DataPointAttributes)point2)["ErrorBarStyle"], "ErrorBarStyle"));
										}
										num16 = this.vAxis.GetLogValue(point2.YValues[0]);
										num15 = this.vAxis.GetLogValue(point2.YValues[2]);
									}
								}
								goto IL_0af5;
							}
							num8++;
							continue;
							IL_0af5:
							if (num15 > this.vAxis.GetViewMaximum())
							{
								num15 = this.vAxis.GetViewMaximum();
							}
							if (num15 < this.vAxis.GetViewMinimum())
							{
								num15 = this.vAxis.GetViewMinimum();
							}
							num15 = (double)(float)this.vAxis.GetLinearPosition(num15);
							if (num16 > this.vAxis.GetViewMaximum())
							{
								num16 = this.vAxis.GetViewMaximum();
							}
							if (num16 < this.vAxis.GetViewMinimum())
							{
								num16 = this.vAxis.GetViewMinimum();
							}
							num16 = this.vAxis.GetLinearPosition(num16);
							Point3D[] array2 = new Point3D[2]
							{
								new Point3D(num13, (float)num15, (float)(num7 + num6 / 2.0)),
								new Point3D(num13, (float)num16, (float)(num7 + num6 / 2.0))
							};
							area.matrix3D.TransformPoints(array2);
							num13 = array2[0].X;
							num15 = (double)array2[0].Y;
							num16 = (double)array2[1].Y;
							graph.StartAnimation();
							this.DrawLabel(common, area, graph, item, point2, new PointF(num13, (float)Math.Min(num15, num16)), num8);
							graph.StopAnimation();
							num8++;
						}
					}
					if (!selection)
					{
						common.EventsManager.OnPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
					}
				}
			}
		}

		protected virtual void DrawErrorBarMarks3D(ChartGraphics graph, ErrorBarStyle barStyle, ChartArea area, Series ser, DataPoint point, float xPosition, float width, float zPosition, float depth)
		{
			float num = 0f;
			string empty = string.Empty;
			if (barStyle == ErrorBarStyle.Both || barStyle == ErrorBarStyle.LowerError)
			{
				num = (float)this.vAxis.GetLogValue(point.YValues[1]);
				empty = "LINE";
				if (point.MarkerStyle != 0)
				{
					empty = point.MarkerStyle.ToString();
				}
				this.DrawErrorBarSingleMarker(graph, area, point, empty, xPosition, num, (float)(zPosition + depth / 2.0), width, true);
			}
			if (barStyle == ErrorBarStyle.Both || barStyle == ErrorBarStyle.UpperError)
			{
				num = (float)this.vAxis.GetLogValue(point.YValues[2]);
				empty = "LINE";
				if (point.MarkerStyle != 0)
				{
					empty = point.MarkerStyle.ToString();
				}
				this.DrawErrorBarSingleMarker(graph, area, point, empty, xPosition, num, (float)(zPosition + depth / 2.0), width, true);
			}
			if (!point.IsAttributeSet("ErrorBarCenterMarkerStyle") && !point.series.IsAttributeSet("ErrorBarCenterMarkerStyle"))
			{
				return;
			}
			num = (float)this.vAxis.GetLogValue(point.YValues[0]);
			empty = ((DataPointAttributes)point.series)["ErrorBarCenterMarkerStyle"];
			if (point.IsAttributeSet("ErrorBarCenterMarkerStyle"))
			{
				empty = ((DataPointAttributes)point)["ErrorBarCenterMarkerStyle"];
			}
			empty = empty.ToUpper(CultureInfo.InvariantCulture);
			this.DrawErrorBarSingleMarker(graph, area, point, empty, xPosition, num, (float)(zPosition + depth / 2.0), width, true);
		}

		public virtual double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			return point.YValues[yValueIndex];
		}

		internal static void CalculateErrorAmount(Series errorBarSeries)
		{
			double num;
			ErrorBarType errorBarType;
			string text;
			if (string.Compare(errorBarSeries.ChartTypeName, "ErrorBar", StringComparison.OrdinalIgnoreCase) == 0 && (errorBarSeries.IsAttributeSet("ErrorBarType") || errorBarSeries.IsAttributeSet("ErrorBarSeries")))
			{
				num = double.NaN;
				errorBarType = ErrorBarType.StandardError;
				if (errorBarSeries.IsAttributeSet("ErrorBarType"))
				{
					text = ((DataPointAttributes)errorBarSeries)["ErrorBarType"];
					if (text.StartsWith("FixedValue", StringComparison.OrdinalIgnoreCase))
					{
						errorBarType = ErrorBarType.FixedValue;
						goto IL_00c4;
					}
					if (text.StartsWith("Percentage", StringComparison.OrdinalIgnoreCase))
					{
						errorBarType = ErrorBarType.Percentage;
						goto IL_00c4;
					}
					if (text.StartsWith("StandardDeviation", StringComparison.OrdinalIgnoreCase))
					{
						errorBarType = ErrorBarType.StandardDeviation;
						goto IL_00c4;
					}
					if (text.StartsWith("StandardError", StringComparison.OrdinalIgnoreCase))
					{
						errorBarType = ErrorBarType.StandardError;
						goto IL_00c4;
					}
					if (!text.StartsWith("None", StringComparison.OrdinalIgnoreCase))
					{
						throw new InvalidOperationException(SR.ExceptionErrorBarTypeInvalid(((DataPointAttributes)errorBarSeries)["ErrorBarType"]));
					}
					return;
				}
				goto IL_013b;
			}
			return;
			IL_013b:
			int count = errorBarSeries.Points.Count;
			int num2 = 0;
			foreach (DataPoint point in errorBarSeries.Points)
			{
				if (point.Empty)
				{
					num2++;
				}
			}
			count -= num2;
			if (double.IsNaN(num))
			{
				num = ErrorBarChart.DefaultErrorBarTypeValue(errorBarType);
			}
			double num3 = 0.0;
			switch (errorBarType)
			{
			case ErrorBarType.FixedValue:
				num3 = num;
				break;
			case ErrorBarType.StandardDeviation:
				if (count > 1)
				{
					double num4 = 0.0;
					foreach (DataPoint point2 in errorBarSeries.Points)
					{
						num4 += point2.YValues[0];
					}
					num4 /= (double)count;
					num3 = 0.0;
					foreach (DataPoint point3 in errorBarSeries.Points)
					{
						if (!point3.Empty)
						{
							num3 += Math.Pow(point3.YValues[0] - num4, 2.0);
						}
					}
					num3 = num * Math.Sqrt(num3 / (double)(count - 1));
				}
				else
				{
					num3 = 0.0;
				}
				break;
			case ErrorBarType.StandardError:
				if (count > 1)
				{
					num3 = 0.0;
					foreach (DataPoint point4 in errorBarSeries.Points)
					{
						if (!point4.Empty)
						{
							num3 += Math.Pow(point4.YValues[0], 2.0);
						}
					}
					num3 = num * Math.Sqrt(num3 / (double)(count * (count - 1))) / 2.0;
				}
				else
				{
					num3 = 0.0;
				}
				break;
			}
			foreach (DataPoint point5 in errorBarSeries.Points)
			{
				if (errorBarType == ErrorBarType.Percentage)
				{
					point5.YValues[1] = point5.YValues[0] - point5.YValues[0] * num / 100.0;
					point5.YValues[2] = point5.YValues[0] + point5.YValues[0] * num / 100.0;
				}
				else
				{
					point5.YValues[1] = point5.YValues[0] - num3;
					point5.YValues[2] = point5.YValues[0] + num3;
				}
			}
			return;
			IL_00c4:
			text = text.Substring(errorBarType.ToString().Length);
			if (text.Length > 0)
			{
				if (!text.StartsWith("(", StringComparison.Ordinal) || !text.EndsWith(")", StringComparison.Ordinal))
				{
					throw new InvalidOperationException(SR.ExceptionErrorBarTypeFormatInvalid(((DataPointAttributes)errorBarSeries)["ErrorBarType"]));
				}
				text = text.Substring(1, text.Length - 2);
				if (text.Length > 0)
				{
					num = double.Parse(text, CultureInfo.InvariantCulture);
				}
			}
			goto IL_013b;
		}

		internal static double DefaultErrorBarTypeValue(ErrorBarType errorBarType)
		{
			switch (errorBarType)
			{
			case ErrorBarType.FixedValue:
			case ErrorBarType.Percentage:
				return 10.0;
			case ErrorBarType.StandardDeviation:
			case ErrorBarType.StandardError:
				return 1.0;
			default:
				return 10.0;
			}
		}

		internal static void GetDataFromLinkedSeries(Series errorBarSeries, IServiceContainer serviceContainer)
		{
			if (string.Compare(errorBarSeries.ChartTypeName, "ErrorBar", StringComparison.OrdinalIgnoreCase) == 0 && serviceContainer != null && errorBarSeries.IsAttributeSet("ErrorBarSeries"))
			{
				string text = ((DataPointAttributes)errorBarSeries)["ErrorBarSeries"];
				string valueName = "Y";
				int num = text.IndexOf(":", StringComparison.Ordinal);
				if (num >= 0)
				{
					valueName = text.Substring(num + 1);
					text = text.Substring(0, num);
				}
				Chart chart = (Chart)serviceContainer.GetService(typeof(Chart));
				if (chart != null)
				{
					if (chart.Series.GetIndex(text) == -1)
					{
						throw new InvalidOperationException(SR.ExceptionDataSeriesNameNotFound(text));
					}
					Series series = chart.Series[text];
					errorBarSeries.XAxisType = series.XAxisType;
					errorBarSeries.YAxisType = series.YAxisType;
					errorBarSeries.Points.Clear();
					foreach (DataPoint point in series.Points)
					{
						errorBarSeries.Points.AddXY(point.XValue, point.GetValueByName(valueName));
					}
				}
			}
		}

		public void AddSmartLabelMarkerPositions(CommonElements common, ChartArea area, Series series, ArrayList list)
		{
		}
	}
}
