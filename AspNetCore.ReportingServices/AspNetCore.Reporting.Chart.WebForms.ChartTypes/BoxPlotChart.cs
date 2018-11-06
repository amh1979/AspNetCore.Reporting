using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class BoxPlotChart : IChartType
	{
		protected Axis vAxis;

		protected Axis hAxis;

		protected bool showSideBySide = true;

		public virtual string Name
		{
			get
			{
				return "BoxPlot";
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
				return 6;
			}
		}

		public virtual LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Rectangle;
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
						bool flag2 = this.showSideBySide;
						if (item.IsAttributeSet("DrawSideBySide"))
						{
							string strA = ((DataPointAttributes)item)["DrawSideBySide"];
							if (string.Compare(strA, "False", StringComparison.OrdinalIgnoreCase) == 0)
							{
								flag2 = false;
							}
							else if (string.Compare(strA, "True", StringComparison.OrdinalIgnoreCase) == 0)
							{
								flag2 = true;
							}
							else if (string.Compare(strA, "Auto", StringComparison.OrdinalIgnoreCase) != 0)
							{
								throw new InvalidOperationException(SR.ExceptionAttributeDrawSideBySideInvalid);
							}
						}
						double num2 = (double)seriesFromChartType.Count;
						if (!flag2)
						{
							num2 = 1.0;
						}
						float num3 = (float)(item.GetPointWidth(graph, this.hAxis, interval, 0.8) / num2);
						if (!selection)
						{
							common.EventsManager.OnBackPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
						}
						int num4 = 1;
						foreach (DataPoint point in item.Points)
						{
							if (point.YValues.Length < this.YValuesPerPoint)
							{
								throw new InvalidOperationException(SR.ExceptionChartTypeRequiresYValues(this.Name, this.YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
							}
							point.positionRel = new PointF(float.NaN, float.NaN);
							float num5 = 0f;
							double num6 = point.XValue;
							if (flag)
							{
								num6 = (double)num4;
								num5 = (float)(this.hAxis.GetPosition((double)num4) - (double)num3 * num2 / 2.0 + num3 / 2.0 + (double)((float)num * num3));
							}
							else
							{
								num5 = ((!flag2) ? ((float)this.hAxis.GetPosition(num6)) : ((float)(this.hAxis.GetPosition(num6) - (double)num3 * num2 / 2.0 + num3 / 2.0 + (double)((float)num * num3))));
							}
							double logValue = this.vAxis.GetLogValue(point.YValues[0]);
							double logValue2 = this.vAxis.GetLogValue(point.YValues[1]);
							num6 = this.hAxis.GetLogValue(num6);
							if (num6 < this.hAxis.GetViewMinimum() || num6 > this.hAxis.GetViewMaximum() || (logValue < this.vAxis.GetViewMinimum() && logValue2 < this.vAxis.GetViewMinimum()) || (logValue > this.vAxis.GetViewMaximum() && logValue2 > this.vAxis.GetViewMaximum()))
							{
								num4++;
							}
							else
							{
								double num7 = this.vAxis.GetLogValue(point.YValues[0]);
								double num8 = this.vAxis.GetLogValue(point.YValues[1]);
								if (num8 > this.vAxis.GetViewMaximum())
								{
									num8 = this.vAxis.GetViewMaximum();
								}
								if (num8 < this.vAxis.GetViewMinimum())
								{
									num8 = this.vAxis.GetViewMinimum();
								}
								num8 = (double)(float)this.vAxis.GetLinearPosition(num8);
								if (num7 > this.vAxis.GetViewMaximum())
								{
									num7 = this.vAxis.GetViewMaximum();
								}
								if (num7 < this.vAxis.GetViewMinimum())
								{
									num7 = this.vAxis.GetViewMinimum();
								}
								num7 = this.vAxis.GetLinearPosition(num7);
								point.positionRel = new PointF(num5, (float)Math.Min(num8, num7));
								if (common.ProcessModePaint)
								{
									bool flag3 = false;
									if (num6 == this.hAxis.GetViewMinimum() || num6 == this.hAxis.GetViewMaximum())
									{
										graph.SetClip(area.PlotAreaPosition.ToRectangleF());
										flag3 = true;
									}
									Color color = point.BorderColor;
									if (color == Color.Empty)
									{
										color = point.Color;
									}
									graph.StartHotRegion(point);
									graph.StartAnimation();
									graph.DrawLineRel(color, point.BorderWidth, point.BorderStyle, new PointF(num5, (float)num7), new PointF(num5, (float)this.vAxis.GetPosition(point.YValues[2])), item.ShadowColor, item.ShadowOffset);
									graph.DrawLineRel(color, point.BorderWidth, point.BorderStyle, new PointF(num5, (float)num8), new PointF(num5, (float)this.vAxis.GetPosition(point.YValues[3])), item.ShadowColor, item.ShadowOffset);
									RectangleF empty = RectangleF.Empty;
									empty.X = (float)(num5 - num3 / 2.0);
									empty.Width = num3;
									empty.Y = (float)this.vAxis.GetPosition(point.YValues[3]);
									empty.Height = (float)Math.Abs((double)empty.Y - this.vAxis.GetPosition(point.YValues[2]));
									graph.FillRectangleRel(empty, point.Color, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, point.BackGradientEndColor, point.BorderColor, point.BorderWidth, point.BorderStyle, item.ShadowColor, item.ShadowOffset, PenAlignment.Inset);
									bool flag4 = true;
									if (point.IsAttributeSet("BoxPlotShowAverage") || item.IsAttributeSet("BoxPlotShowAverage"))
									{
										string strA2 = ((DataPointAttributes)item)["BoxPlotShowAverage"];
										if (point.IsAttributeSet("BoxPlotShowAverage"))
										{
											strA2 = ((DataPointAttributes)point)["BoxPlotShowAverage"];
										}
										if (string.Compare(strA2, "True", StringComparison.OrdinalIgnoreCase) != 0)
										{
											if (string.Compare(strA2, "False", StringComparison.OrdinalIgnoreCase) != 0)
											{
												throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(((DataPointAttributes)point)["BoxPlotShowAverage"], "BoxPlotShowAverage"));
											}
											flag4 = false;
										}
									}
									SizeF relativeSize = graph.GetRelativeSize(new SizeF((float)point.BorderWidth, (float)point.BorderWidth));
									if (point.BorderColor == Color.Empty)
									{
										relativeSize.Height = 0f;
										relativeSize.Width = 0f;
									}
									Color color2 = color;
									if (color2 == point.Color)
									{
										double num9 = Math.Sqrt((double)(point.Color.R * point.Color.R + point.Color.G * point.Color.G + point.Color.B * point.Color.B));
										color2 = ((!(num9 > 220.0)) ? ChartGraphics.GetGradientColor(point.Color, Color.White, 0.4) : ChartGraphics.GetGradientColor(point.Color, Color.Black, 0.4));
									}
									if (!double.IsNaN(point.YValues[4]) && flag4)
									{
										graph.DrawLineRel(color2, 1, ChartDashStyle.Solid, new PointF(empty.Left + relativeSize.Width, (float)this.vAxis.GetPosition(point.YValues[4])), new PointF(empty.Right - relativeSize.Width, (float)this.vAxis.GetPosition(point.YValues[4])), Color.Empty, 0);
									}
									bool flag5 = true;
									if (point.IsAttributeSet("BoxPlotShowMedian") || item.IsAttributeSet("BoxPlotShowMedian"))
									{
										string strA3 = ((DataPointAttributes)item)["BoxPlotShowMedian"];
										if (point.IsAttributeSet("BoxPlotShowMedian"))
										{
											strA3 = ((DataPointAttributes)point)["BoxPlotShowMedian"];
										}
										if (string.Compare(strA3, "True", StringComparison.OrdinalIgnoreCase) != 0)
										{
											if (string.Compare(strA3, "False", StringComparison.OrdinalIgnoreCase) != 0)
											{
												throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(((DataPointAttributes)point)["BoxPlotShowMedian"], "BoxPlotShowMedian"));
											}
											flag5 = false;
										}
									}
									if (!double.IsNaN(point.YValues[5]) && flag5)
									{
										float y = (float)this.vAxis.GetPosition(point.YValues[5]);
										float val = (float)((empty.Width - relativeSize.Width * 2.0) / 9.0);
										val = Math.Max(val, graph.GetRelativeSize(new SizeF(2f, 2f)).Width);
										for (float num10 = empty.Left + relativeSize.Width; num10 < empty.Right - relativeSize.Width; num10 = (float)(num10 + val * 2.0))
										{
											graph.DrawLineRel(color2, 1, ChartDashStyle.Solid, new PointF(num10, y), new PointF(Math.Min(empty.Right, num10 + val), y), Color.Empty, 0);
										}
									}
									this.DrawBoxPlotMarks(graph, area, item, point, num5, num3);
									graph.StopAnimation();
									graph.EndHotRegion();
									if (flag3)
									{
										graph.ResetClip();
									}
								}
								if (common.ProcessModeRegions)
								{
									RectangleF empty2 = RectangleF.Empty;
									empty2.X = (float)(num5 - num3 / 2.0);
									empty2.Y = (float)Math.Min(num8, num7);
									empty2.Width = num3;
									empty2.Height = (float)Math.Max(num8, num7) - empty2.Y;
									common.HotRegionsList.AddHotRegion(graph, empty2, point, item.Name, num4 - 1);
								}
								num4++;
							}
						}
						if (!selection)
						{
							num4 = 1;
							foreach (DataPoint point2 in item.Points)
							{
								float num11 = 0f;
								double num12 = point2.XValue;
								if (flag)
								{
									num12 = (double)num4;
									num11 = (float)(this.hAxis.GetPosition((double)num4) - (double)num3 * num2 / 2.0 + num3 / 2.0 + (double)((float)num * num3));
								}
								else
								{
									num11 = ((!flag2) ? ((float)this.hAxis.GetPosition(num12)) : ((float)(this.hAxis.GetPosition(num12) - (double)num3 * num2 / 2.0 + num3 / 2.0 + (double)((float)num * num3))));
								}
								double logValue3 = this.vAxis.GetLogValue(point2.YValues[0]);
								double logValue4 = this.vAxis.GetLogValue(point2.YValues[1]);
								num12 = this.hAxis.GetLogValue(num12);
								if (num12 < this.hAxis.GetViewMinimum() || num12 > this.hAxis.GetViewMaximum() || (logValue3 < this.vAxis.GetViewMinimum() && logValue4 < this.vAxis.GetViewMinimum()) || (logValue3 > this.vAxis.GetViewMaximum() && logValue4 > this.vAxis.GetViewMaximum()))
								{
									num4++;
								}
								else
								{
									double num13 = 1.7976931348623157E+308;
									for (int i = 0; i < point2.YValues.Length; i++)
									{
										if (!double.IsNaN(point2.YValues[i]))
										{
											double num14 = this.vAxis.GetLogValue(point2.YValues[i]);
											if (num14 > this.vAxis.GetViewMaximum())
											{
												num14 = this.vAxis.GetViewMaximum();
											}
											if (num14 < this.vAxis.GetViewMinimum())
											{
												num14 = this.vAxis.GetViewMinimum();
											}
											num14 = (double)(float)this.vAxis.GetLinearPosition(num14);
											num13 = Math.Min(num13, num14);
										}
									}
									num13 -= graph.GetRelativeSize(new SizeF((float)point2.MarkerSize, (float)point2.MarkerSize)).Height / 2.0;
									graph.StartHotRegion(point2, true);
									graph.StartAnimation();
									this.DrawLabel(common, area, graph, item, point2, new PointF(num11, (float)num13), num4);
									graph.StopAnimation();
									graph.EndHotRegion();
									num4++;
								}
							}
						}
						if (!selection)
						{
							common.EventsManager.OnPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
						}
						if (flag2)
						{
							num++;
						}
					}
				}
			}
		}

		protected virtual void DrawBoxPlotMarks(ChartGraphics graph, ChartArea area, Series ser, DataPoint point, float xPosition, float width)
		{
			string markerStyle = "LINE";
			if (point.MarkerStyle != 0)
			{
				markerStyle = point.MarkerStyle.ToString();
			}
			double logValue = this.vAxis.GetLogValue(point.YValues[0]);
			this.DrawBoxPlotSingleMarker(graph, area, point, markerStyle, xPosition, (float)logValue, 0f, width, false);
			logValue = this.vAxis.GetLogValue(point.YValues[1]);
			this.DrawBoxPlotSingleMarker(graph, area, point, markerStyle, xPosition, (float)logValue, 0f, width, false);
			markerStyle = "CIRCLE";
			if (point.MarkerStyle != 0)
			{
				markerStyle = point.MarkerStyle.ToString();
			}
			for (int i = 6; i < point.YValues.Length; i++)
			{
				if (!double.IsNaN(point.YValues[i]))
				{
					logValue = this.vAxis.GetLogValue(point.YValues[i]);
					this.DrawBoxPlotSingleMarker(graph, area, point, markerStyle, xPosition, (float)logValue, 0f, width, false);
				}
			}
		}

		private void DrawBoxPlotSingleMarker(ChartGraphics graph, ChartArea area, DataPoint point, string markerStyle, float xPosition, float yPosition, float zPosition, float width, bool draw3D)
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
				Color color = point.BorderColor;
				if (color == Color.Empty)
				{
					color = point.Color;
				}
				if (string.Compare(markerStyle, "Line", StringComparison.OrdinalIgnoreCase) == 0)
				{
					graph.DrawLineRel(color, point.BorderWidth, point.BorderStyle, new PointF((float)(xPosition - width / 4.0), yPosition), new PointF((float)(xPosition + width / 4.0), yPosition), (point.series != null) ? point.series.ShadowColor : Color.Empty, (point.series != null) ? point.series.ShadowOffset : 0);
				}
				else
				{
					MarkerStyle markerStyle2 = (MarkerStyle)Enum.Parse(typeof(MarkerStyle), markerStyle, true);
					SizeF markerSize = this.GetMarkerSize(graph, area.Common, area, point, point.MarkerSize, point.MarkerImage);
					Color color2 = (point.MarkerColor == Color.Empty) ? point.BorderColor : point.MarkerColor;
					if (color2 == Color.Empty)
					{
						color2 = point.Color;
					}
					graph.DrawMarkerRel(new PointF(xPosition, yPosition), markerStyle2, point.MarkerSize, color2, point.MarkerBorderColor, point.MarkerBorderWidth, point.MarkerImage, point.MarkerImageTransparentColor, (point.series != null) ? point.series.ShadowOffset : 0, (point.series != null) ? point.series.ShadowColor : Color.Empty, new RectangleF(xPosition, yPosition, markerSize.Width, markerSize.Height));
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
			int num = 0;
			foreach (Series item in common.DataManager.Series)
			{
				if (string.Compare(item.ChartTypeName, this.Name, StringComparison.OrdinalIgnoreCase) == 0 && !(item.ChartArea != area.Name) && item.IsVisible())
				{
					if (item.YValuesPerPoint < 6)
					{
						throw new ArgumentException(SR.ExceptionChartTypeRequiresYValues("BoxPlot", "6"));
					}
					bool flag2 = this.showSideBySide;
					if (item.IsAttributeSet("DrawSideBySide"))
					{
						string strA = ((DataPointAttributes)item)["DrawSideBySide"];
						if (string.Compare(strA, "False", StringComparison.OrdinalIgnoreCase) == 0)
						{
							flag2 = false;
						}
						else if (string.Compare(strA, "True", StringComparison.OrdinalIgnoreCase) == 0)
						{
							flag2 = true;
						}
						else if (string.Compare(strA, "Auto", StringComparison.OrdinalIgnoreCase) != 0)
						{
							throw new InvalidOperationException(SR.ExceptionAttributeDrawSideBySideInvalid);
						}
					}
					double num2 = (double)seriesFromChartType.Count;
					if (!flag2)
					{
						num2 = 1.0;
					}
					this.hAxis = area.GetAxis(AxisName.X, item.XAxisType, item.XSubAxisName);
					this.vAxis = area.GetAxis(AxisName.Y, item.YAxisType, item.YSubAxisName);
					double interval = flag ? 1.0 : area.GetPointsInterval(this.hAxis.Logarithmic, this.hAxis.logarithmBase);
					float num3 = (float)(item.GetPointWidth(graph, this.hAxis, interval, 0.8) / num2);
					if (!selection)
					{
						common.EventsManager.OnBackPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
					}
					float num4 = default(float);
					float num5 = default(float);
					((ChartArea3D)area).GetSeriesZPositionAndDepth(item, out num4, out num5);
					int num6 = 1;
					foreach (DataPoint point in item.Points)
					{
						if (point.YValues.Length < this.YValuesPerPoint)
						{
							throw new InvalidOperationException(SR.ExceptionChartTypeRequiresYValues(this.Name, this.YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
						}
						point.positionRel = new PointF(float.NaN, float.NaN);
						float num7 = 0f;
						double num8 = point.XValue;
						if (flag)
						{
							num8 = (double)num6;
							num7 = (float)(this.hAxis.GetPosition((double)num6) - (double)num3 * num2 / 2.0 + num3 / 2.0 + (double)((float)num * num3));
						}
						else
						{
							num7 = ((!flag2) ? ((float)this.hAxis.GetPosition(num8)) : ((float)(this.hAxis.GetPosition(num8) - (double)num3 * num2 / 2.0 + num3 / 2.0 + (double)((float)num * num3))));
						}
						double logValue = this.vAxis.GetLogValue(point.YValues[0]);
						double logValue2 = this.vAxis.GetLogValue(point.YValues[1]);
						num8 = this.hAxis.GetLogValue(num8);
						if (num8 < this.hAxis.GetViewMinimum() || num8 > this.hAxis.GetViewMaximum() || (logValue < this.vAxis.GetViewMinimum() && logValue2 < this.vAxis.GetViewMinimum()) || (logValue > this.vAxis.GetViewMaximum() && logValue2 > this.vAxis.GetViewMaximum()))
						{
							num6++;
						}
						else
						{
							double num9 = this.vAxis.GetLogValue(point.YValues[1]);
							double num10 = this.vAxis.GetLogValue(point.YValues[0]);
							if (num9 > this.vAxis.GetViewMaximum())
							{
								num9 = this.vAxis.GetViewMaximum();
							}
							if (num9 < this.vAxis.GetViewMinimum())
							{
								num9 = this.vAxis.GetViewMinimum();
							}
							num9 = (double)(float)this.vAxis.GetLinearPosition(num9);
							if (num10 > this.vAxis.GetViewMaximum())
							{
								num10 = this.vAxis.GetViewMaximum();
							}
							if (num10 < this.vAxis.GetViewMinimum())
							{
								num10 = this.vAxis.GetViewMinimum();
							}
							num10 = this.vAxis.GetLinearPosition(num10);
							point.positionRel = new PointF(num7, (float)Math.Min(num9, num10));
							Point3D[] array = new Point3D[6]
							{
								new Point3D(num7, (float)num10, (float)(num5 + num4 / 2.0)),
								new Point3D(num7, (float)num9, (float)(num5 + num4 / 2.0)),
								new Point3D(num7, (float)this.vAxis.GetPosition(point.YValues[2]), (float)(num5 + num4 / 2.0)),
								new Point3D(num7, (float)this.vAxis.GetPosition(point.YValues[3]), (float)(num5 + num4 / 2.0)),
								new Point3D(num7, (float)this.vAxis.GetPosition(point.YValues[4]), (float)(num5 + num4 / 2.0)),
								new Point3D(num7, (float)this.vAxis.GetPosition(point.YValues[5]), (float)(num5 + num4 / 2.0))
							};
							area.matrix3D.TransformPoints(array);
							if (common.ProcessModePaint)
							{
								bool flag3 = false;
								if (num8 == this.hAxis.GetViewMinimum() || num8 == this.hAxis.GetViewMaximum())
								{
									graph.SetClip(area.PlotAreaPosition.ToRectangleF());
									flag3 = true;
								}
								Color color = point.BorderColor;
								if (color == Color.Empty)
								{
									color = point.Color;
								}
								graph.StartAnimation();
								graph.StartHotRegion(point);
								graph.DrawLineRel(color, point.BorderWidth, point.BorderStyle, array[0].PointF, array[2].PointF, item.ShadowColor, item.ShadowOffset);
								graph.DrawLineRel(color, point.BorderWidth, point.BorderStyle, array[1].PointF, array[3].PointF, item.ShadowColor, item.ShadowOffset);
								RectangleF empty = RectangleF.Empty;
								empty.X = (float)(array[0].X - num3 / 2.0);
								empty.Width = num3;
								empty.Y = array[3].Y;
								empty.Height = Math.Abs(empty.Y - array[2].Y);
								graph.FillRectangleRel(empty, point.Color, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, point.BackGradientEndColor, point.BorderColor, point.BorderWidth, point.BorderStyle, item.ShadowColor, item.ShadowOffset, PenAlignment.Inset);
								bool flag4 = true;
								if (point.IsAttributeSet("BoxPlotShowAverage") || item.IsAttributeSet("BoxPlotShowAverage"))
								{
									string strA2 = ((DataPointAttributes)item)["BoxPlotShowAverage"];
									if (point.IsAttributeSet("BoxPlotShowAverage"))
									{
										strA2 = ((DataPointAttributes)point)["BoxPlotShowAverage"];
									}
									if (string.Compare(strA2, "True", StringComparison.OrdinalIgnoreCase) != 0)
									{
										if (string.Compare(strA2, "False", StringComparison.OrdinalIgnoreCase) != 0)
										{
											throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(((DataPointAttributes)point)["BoxPlotShowAverage"], "BoxPlotShowAverage"));
										}
										flag4 = false;
									}
								}
								Color color2 = color;
								if (color2 == point.Color)
								{
									double num11 = Math.Sqrt((double)(point.Color.R * point.Color.R + point.Color.G * point.Color.G + point.Color.B * point.Color.B));
									color2 = ((!(num11 > 220.0)) ? ChartGraphics.GetGradientColor(point.Color, Color.White, 0.4) : ChartGraphics.GetGradientColor(point.Color, Color.Black, 0.4));
								}
								if (!double.IsNaN(point.YValues[4]) && flag4)
								{
									graph.DrawLineRel(color2, 1, ChartDashStyle.Solid, new PointF(empty.Left, array[4].Y), new PointF(empty.Right, array[4].Y), Color.Empty, 0);
								}
								bool flag5 = true;
								if (point.IsAttributeSet("BoxPlotShowMedian") || item.IsAttributeSet("BoxPlotShowMedian"))
								{
									string strA3 = ((DataPointAttributes)item)["BoxPlotShowMedian"];
									if (point.IsAttributeSet("BoxPlotShowMedian"))
									{
										strA3 = ((DataPointAttributes)point)["BoxPlotShowMedian"];
									}
									if (string.Compare(strA3, "True", StringComparison.OrdinalIgnoreCase) != 0)
									{
										if (string.Compare(strA3, "False", StringComparison.OrdinalIgnoreCase) != 0)
										{
											throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(((DataPointAttributes)point)["BoxPlotShowMedian"], "BoxPlotShowMedian"));
										}
										flag5 = false;
									}
								}
								if (!double.IsNaN(point.YValues[5]) && flag5)
								{
									float y = array[5].Y;
									float val = (float)(empty.Width / 9.0);
									val = Math.Max(val, graph.GetRelativeSize(new SizeF(2f, 2f)).Width);
									for (float num12 = empty.Left; num12 < empty.Right; num12 = (float)(num12 + val * 2.0))
									{
										graph.DrawLineRel(color2, 1, ChartDashStyle.Solid, new PointF(num12, y), new PointF(Math.Min(empty.Right, num12 + val), y), Color.Empty, 0);
									}
								}
								this.DrawBoxPlotMarks3D(graph, area, item, point, num7, num3, num5, num4);
								num7 = array[0].X;
								num9 = (double)array[0].Y;
								num10 = (double)array[1].Y;
								graph.StopAnimation();
								graph.EndHotRegion();
								if (flag3)
								{
									graph.ResetClip();
								}
							}
							if (common.ProcessModeRegions)
							{
								num7 = array[0].X;
								num9 = (double)array[0].Y;
								num10 = (double)array[1].Y;
								RectangleF empty2 = RectangleF.Empty;
								empty2.X = (float)(num7 - num3 / 2.0);
								empty2.Y = (float)Math.Min(num9, num10);
								empty2.Width = num3;
								empty2.Height = (float)Math.Max(num9, num10) - empty2.Y;
								common.HotRegionsList.AddHotRegion(graph, empty2, point, item.Name, num6 - 1);
							}
							num6++;
						}
					}
					if (!selection)
					{
						num6 = 1;
						foreach (DataPoint point2 in item.Points)
						{
							float num13 = 0f;
							double num14 = point2.XValue;
							if (flag)
							{
								num14 = (double)num6;
								num13 = (float)(this.hAxis.GetPosition((double)num6) - (double)num3 * num2 / 2.0 + num3 / 2.0 + (double)((float)num * num3));
							}
							else
							{
								num13 = ((!flag2) ? ((float)this.hAxis.GetPosition(num14)) : ((float)(this.hAxis.GetPosition(num14) - (double)num3 * num2 / 2.0 + num3 / 2.0 + (double)((float)num * num3))));
							}
							double logValue3 = this.vAxis.GetLogValue(point2.YValues[0]);
							double logValue4 = this.vAxis.GetLogValue(point2.YValues[1]);
							num14 = this.hAxis.GetLogValue(num14);
							if (num14 < this.hAxis.GetViewMinimum() || num14 > this.hAxis.GetViewMaximum() || (logValue3 < this.vAxis.GetViewMinimum() && logValue4 < this.vAxis.GetViewMinimum()) || (logValue3 > this.vAxis.GetViewMaximum() && logValue4 > this.vAxis.GetViewMaximum()))
							{
								num6++;
							}
							else
							{
								double num15 = this.vAxis.GetLogValue(point2.YValues[1]);
								double num16 = this.vAxis.GetLogValue(point2.YValues[0]);
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
									new Point3D(num13, (float)num15, (float)(num5 + num4 / 2.0)),
									new Point3D(num13, (float)num16, (float)(num5 + num4 / 2.0))
								};
								area.matrix3D.TransformPoints(array2);
								num13 = array2[0].X;
								num15 = (double)array2[0].Y;
								num16 = (double)array2[1].Y;
								graph.StartAnimation();
								this.DrawLabel(common, area, graph, item, point2, new PointF(num13, (float)Math.Min(num15, num16)), num6);
								graph.StopAnimation();
								num6++;
							}
						}
					}
					if (!selection)
					{
						common.EventsManager.OnPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
					}
				}
			}
		}

		protected virtual void DrawBoxPlotMarks3D(ChartGraphics graph, ChartArea area, Series ser, DataPoint point, float xPosition, float width, float zPosition, float depth)
		{
			string markerStyle = "LINE";
			if (point.MarkerStyle != 0)
			{
				markerStyle = point.MarkerStyle.ToString();
			}
			double logValue = this.vAxis.GetLogValue(point.YValues[0]);
			this.DrawBoxPlotSingleMarker(graph, area, point, markerStyle, xPosition, (float)logValue, (float)(zPosition + depth / 2.0), width, true);
			logValue = this.vAxis.GetLogValue(point.YValues[1]);
			this.DrawBoxPlotSingleMarker(graph, area, point, markerStyle, xPosition, (float)logValue, (float)(zPosition + depth / 2.0), width, true);
			markerStyle = "CIRCLE";
			if (point.MarkerStyle != 0)
			{
				markerStyle = point.MarkerStyle.ToString();
			}
			for (int i = 6; i < point.YValues.Length; i++)
			{
				if (!double.IsNaN(point.YValues[i]))
				{
					logValue = this.vAxis.GetLogValue(point.YValues[i]);
					this.DrawBoxPlotSingleMarker(graph, area, point, markerStyle, xPosition, (float)logValue, (float)(zPosition + depth / 2.0), width, true);
				}
			}
		}

		public virtual double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			return point.YValues[yValueIndex];
		}

		internal static void CalculateBoxPlotFromLinkedSeries(Series boxPlotSeries, IServiceContainer serviceContainer)
		{
			if (string.Compare(boxPlotSeries.ChartTypeName, "BoxPlot", StringComparison.OrdinalIgnoreCase) == 0 && serviceContainer != null)
			{
				if (boxPlotSeries.IsAttributeSet("BoxPlotSeries"))
				{
					string[] array = ((DataPointAttributes)boxPlotSeries)["BoxPlotSeries"].Split(';');
					boxPlotSeries.Points.Clear();
					int num = 0;
					string[] array2 = array;
					foreach (string text in array2)
					{
						boxPlotSeries.Points.AddY(0.0);
						((DataPointAttributes)boxPlotSeries.Points[num++])["BoxPlotSeries"] = text.Trim();
					}
				}
				int num3 = 0;
				string text2;
				while (true)
				{
					if (num3 < boxPlotSeries.Points.Count)
					{
						DataPoint dataPoint = boxPlotSeries.Points[num3];
						if (dataPoint.IsAttributeSet("BoxPlotSeries"))
						{
							text2 = ((DataPointAttributes)dataPoint)["BoxPlotSeries"];
							string valueName = "Y";
							int num4 = text2.IndexOf(":", StringComparison.OrdinalIgnoreCase);
							if (num4 >= 0)
							{
								valueName = text2.Substring(num4 + 1);
								text2 = text2.Substring(0, num4);
							}
							Chart chart = (Chart)serviceContainer.GetService(typeof(Chart));
							if (chart != null)
							{
								if (chart.Series.GetIndex(text2) == -1)
								{
									break;
								}
								Series linkedSeries = chart.Series[text2];
								BoxPlotChart.CalculateBoxPlotValues(ref dataPoint, linkedSeries, valueName);
							}
						}
						num3++;
						continue;
					}
					return;
				}
				throw new InvalidOperationException(SR.ExceptionCustomAttributeSeriesNameNotFound("BoxPlotSeries", text2));
			}
		}

		private static void CalculateBoxPlotValues(ref DataPoint boxPoint, Series linkedSeries, string valueName)
		{
			double num;
			double[] array;
			double[] array2;
			string text2;
			if (linkedSeries.Points.Count != 0)
			{
				num = 0.0;
				int num2 = 0;
				foreach (DataPoint point in linkedSeries.Points)
				{
					if (!point.Empty)
					{
						num += point.GetValueByName(valueName);
						num2++;
					}
				}
				num /= (double)num2;
				array = new double[num2];
				int num3 = 0;
				foreach (DataPoint point2 in linkedSeries.Points)
				{
					if (!point2.Empty)
					{
						array[num3++] = (point2.Empty ? double.NaN : point2.GetValueByName(valueName));
					}
				}
				array2 = new double[5]
				{
					10.0,
					90.0,
					25.0,
					75.0,
					50.0
				};
				string text = boxPoint.IsAttributeSet("BoxPlotPercentile") ? ((DataPointAttributes)boxPoint)["BoxPlotPercentile"] : string.Empty;
				if (text.Length == 0 && boxPoint.series != null && boxPoint.series.IsAttributeSet("BoxPlotPercentile"))
				{
					text = ((DataPointAttributes)boxPoint.series)["BoxPlotPercentile"];
				}
				text2 = (boxPoint.IsAttributeSet("BoxPlotWhiskerPercentile") ? ((DataPointAttributes)boxPoint)["BoxPlotWhiskerPercentile"] : string.Empty);
				if (text2.Length == 0 && boxPoint.series != null && boxPoint.series.IsAttributeSet("BoxPlotWhiskerPercentile"))
				{
					text2 = ((DataPointAttributes)boxPoint.series)["BoxPlotWhiskerPercentile"];
				}
				if (text.Length > 0)
				{
					try
					{
						array2[2] = double.Parse(text, CultureInfo.InvariantCulture);
					}
					catch
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeIsNotInRange0to50("BoxPlotPercentile"));
					}
					if (!(array2[2] < 0.0) && !(array2[2] > 50.0))
					{
						array2[3] = 100.0 - array2[2];
						goto IL_021c;
					}
					throw new InvalidOperationException(SR.ExceptionCustomAttributeIsNotInRange0to50("BoxPlotPercentile"));
				}
				goto IL_021c;
			}
			return;
			IL_037e:
			bool flag;
			if (flag)
			{
				BoxPlotChart.BoxPlotAddUnusual(ref boxPoint, array);
			}
			return;
			IL_021c:
			if (text2.Length > 0)
			{
				try
				{
					array2[0] = double.Parse(text2, CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeIsNotInRange0to50("BoxPlotWhiskerPercentile"));
				}
				if (!(array2[0] < 0.0) && !(array2[0] > 50.0))
				{
					array2[1] = 100.0 - array2[0];
					goto IL_0289;
				}
				throw new InvalidOperationException(SR.ExceptionCustomAttributeIsNotInRange0to50("BoxPlotPercentile"));
			}
			goto IL_0289;
			IL_0289:
			double[] array3 = BoxPlotChart.CalculatePercentileValues(array, array2);
			boxPoint.YValues[0] = array3[0];
			boxPoint.YValues[1] = array3[1];
			boxPoint.YValues[2] = array3[2];
			boxPoint.YValues[3] = array3[3];
			boxPoint.YValues[4] = num;
			boxPoint.YValues[5] = array3[4];
			flag = false;
			string text3 = boxPoint.IsAttributeSet("BoxPlotShowUnusualValues") ? ((DataPointAttributes)boxPoint)["BoxPlotShowUnusualValues"] : string.Empty;
			if (text3.Length == 0 && boxPoint.series != null && boxPoint.series.IsAttributeSet("BoxPlotShowUnusualValues"))
			{
				text3 = ((DataPointAttributes)boxPoint.series)["BoxPlotShowUnusualValues"];
			}
			if (text3.Length > 0)
			{
				if (string.Compare(text3, "True", StringComparison.OrdinalIgnoreCase) != 0)
				{
					if (string.Compare(text3, "False", StringComparison.OrdinalIgnoreCase) == 0)
					{
						flag = false;
						goto IL_037e;
					}
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid2("BoxPlotShowUnusualValues"));
				}
				flag = true;
			}
			goto IL_037e;
		}

		private static void BoxPlotAddUnusual(ref DataPoint boxPoint, double[] yValues)
		{
			ArrayList arrayList = new ArrayList();
			foreach (double num in yValues)
			{
				if (num < boxPoint.YValues[0] || num > boxPoint.YValues[1])
				{
					arrayList.Add(num);
				}
			}
			if (arrayList.Count > 0)
			{
				double[] array = new double[6 + arrayList.Count];
				for (int j = 0; j < 6; j++)
				{
					array[j] = boxPoint.YValues[j];
				}
				for (int k = 0; k < arrayList.Count; k++)
				{
					array[6 + k] = (double)arrayList[k];
				}
				boxPoint.YValues = array;
			}
		}

		private static double[] CalculatePercentileValues(double[] yValues, double[] requiredPercentile)
		{
			double[] array = new double[5];
			Array.Sort(yValues);
			int num = 0;
			foreach (double num2 in requiredPercentile)
			{
				double num3 = ((double)yValues.Length - 1.0) / 100.0 * num2;
				double num4 = Math.Floor(num3);
				double num5 = num3 - num4;
				array[num] = 0.0;
				if ((int)num4 < yValues.Length)
				{
					array[num] += (1.0 - num5) * yValues[(int)num4];
				}
				if ((int)(num4 + 1.0) < yValues.Length)
				{
					array[num] += num5 * yValues[(int)num4 + 1];
				}
				num++;
			}
			return array;
		}

		public void AddSmartLabelMarkerPositions(CommonElements common, ChartArea area, Series series, ArrayList list)
		{
		}
	}
}
