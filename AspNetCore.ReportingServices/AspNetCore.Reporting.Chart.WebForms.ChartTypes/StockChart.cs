using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class StockChart : IChartType
	{
		protected Axis vAxis;

		protected Axis hAxis;

		protected StockOpenCloseMarkStyle openCloseStyle;

		protected bool forceCandleStick;

		public virtual string Name
		{
			get
			{
				return "Stock";
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
				return 4;
			}
		}

		public StockChart()
		{
		}

		public StockChart(StockOpenCloseMarkStyle style)
		{
			this.openCloseStyle = style;
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
				foreach (Series item in common.DataManager.Series)
				{
					if (string.Compare(item.ChartTypeName, this.Name, StringComparison.OrdinalIgnoreCase) == 0 && !(item.ChartArea != area.Name) && item.IsVisible())
					{
						if (item.YValuesPerPoint < 4)
						{
							throw new ArgumentException(SR.ExceptionChartTypeRequiresYValues("StockChart", "4"));
						}
						this.hAxis = area.GetAxis(AxisName.X, item.XAxisType, item.XSubAxisName);
						this.vAxis = area.GetAxis(AxisName.Y, item.YAxisType, item.YSubAxisName);
						double interval = flag ? 1.0 : area.GetPointsInterval(this.hAxis.Logarithmic, this.hAxis.logarithmBase);
						float num = (float)item.GetPointWidth(graph, this.hAxis, interval, 0.8);
						if (!selection)
						{
							common.EventsManager.OnBackPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
						}
						int num2 = 1;
						foreach (DataPoint point in item.Points)
						{
							point.positionRel = new PointF(float.NaN, float.NaN);
							double num3 = point.XValue;
							if (flag)
							{
								num3 = (double)num2;
							}
							float num4 = (float)this.hAxis.GetPosition(num3);
							double logValue = this.vAxis.GetLogValue(point.YValues[0]);
							double logValue2 = this.vAxis.GetLogValue(point.YValues[1]);
							num3 = this.hAxis.GetLogValue(num3);
							if (num3 < this.hAxis.GetViewMinimum() || num3 > this.hAxis.GetViewMaximum() || (logValue < this.vAxis.GetViewMinimum() && logValue2 < this.vAxis.GetViewMinimum()) || (logValue > this.vAxis.GetViewMaximum() && logValue2 > this.vAxis.GetViewMaximum()))
							{
								num2++;
							}
							else
							{
								double num5 = this.vAxis.GetLogValue(point.YValues[0]);
								double num6 = this.vAxis.GetLogValue(point.YValues[1]);
								if (num5 > this.vAxis.GetViewMaximum())
								{
									num5 = this.vAxis.GetViewMaximum();
								}
								if (num5 < this.vAxis.GetViewMinimum())
								{
									num5 = this.vAxis.GetViewMinimum();
								}
								num5 = (double)(float)this.vAxis.GetLinearPosition(num5);
								if (num6 > this.vAxis.GetViewMaximum())
								{
									num6 = this.vAxis.GetViewMaximum();
								}
								if (num6 < this.vAxis.GetViewMinimum())
								{
									num6 = this.vAxis.GetViewMinimum();
								}
								num6 = this.vAxis.GetLinearPosition(num6);
								point.positionRel = new PointF(num4, (float)num5);
								if (common.ProcessModePaint)
								{
									bool flag2 = false;
									if (num3 == this.hAxis.GetViewMinimum() || num3 == this.hAxis.GetViewMaximum())
									{
										graph.SetClip(area.PlotAreaPosition.ToRectangleF());
										flag2 = true;
									}
									graph.StartHotRegion(point);
									graph.StartAnimation();
									graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, new PointF(num4, (float)num5), new PointF(num4, (float)num6), item.ShadowColor, item.ShadowOffset);
									this.DrawOpenCloseMarks(graph, area, item, point, num4, num);
									graph.StopAnimation();
									graph.EndHotRegion();
									if (flag2)
									{
										graph.ResetClip();
									}
								}
								if (common.ProcessModeRegions)
								{
									RectangleF empty = RectangleF.Empty;
									empty.X = (float)(num4 - num / 2.0);
									empty.Y = (float)Math.Min(num5, num6);
									empty.Width = num;
									empty.Height = (float)Math.Max(num5, num6) - empty.Y;
									common.HotRegionsList.AddHotRegion(graph, empty, point, item.Name, num2 - 1);
								}
								num2++;
							}
						}
						int num7 = 0;
						num2 = 1;
						foreach (DataPoint point2 in item.Points)
						{
							double num8 = point2.XValue;
							if (flag)
							{
								num8 = (double)num2;
							}
							float x = (float)this.hAxis.GetPosition(num8);
							double logValue3 = this.vAxis.GetLogValue(point2.YValues[0]);
							double logValue4 = this.vAxis.GetLogValue(point2.YValues[1]);
							num8 = this.hAxis.GetLogValue(num8);
							if (num8 < this.hAxis.GetViewMinimum() || num8 > this.hAxis.GetViewMaximum() || (logValue3 < this.vAxis.GetViewMinimum() && logValue4 < this.vAxis.GetViewMinimum()) || (logValue3 > this.vAxis.GetViewMaximum() && logValue4 > this.vAxis.GetViewMaximum()))
							{
								num2++;
							}
							else
							{
								double num9 = this.vAxis.GetLogValue(point2.YValues[0]);
								double num10 = this.vAxis.GetLogValue(point2.YValues[1]);
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
								if (point2.MarkerStyle != 0 || point2.MarkerImage.Length > 0)
								{
									SizeF empty2 = SizeF.Empty;
									empty2.Width = (float)point2.MarkerSize;
									empty2.Height = (float)point2.MarkerSize;
									if (point2.MarkerImage.Length > 0)
									{
										common.ImageLoader.GetAdjustedImageSize(point2.MarkerImage, graph.Graphics, ref empty2);
									}
									PointF empty3 = PointF.Empty;
									empty3.X = x;
									empty3.Y = (float)((float)num9 - graph.GetRelativeSize(empty2).Height / 2.0);
									if (num7 == 0)
									{
										graph.StartAnimation();
										graph.DrawMarkerRel(empty3, point2.MarkerStyle, (int)empty2.Height, (point2.MarkerColor == Color.Empty) ? point2.Color : point2.MarkerColor, (point2.MarkerBorderColor == Color.Empty) ? point2.BorderColor : point2.MarkerBorderColor, point2.MarkerBorderWidth, point2.MarkerImage, point2.MarkerImageTransparentColor, (point2.series != null) ? point2.series.ShadowOffset : 0, (point2.series != null) ? point2.series.ShadowColor : Color.Empty, new RectangleF(empty3.X, empty3.Y, empty2.Width, empty2.Height));
										graph.StopAnimation();
										if (common.ProcessModeRegions)
										{
											SizeF relativeSize = graph.GetRelativeSize(empty2);
											int insertIndex = common.HotRegionsList.FindInsertIndex();
											common.HotRegionsList.FindInsertIndex();
											if (point2.MarkerStyle == MarkerStyle.Circle)
											{
												float[] array = new float[3]
												{
													empty3.X,
													empty3.Y,
													(float)(relativeSize.Width / 2.0)
												};
												common.HotRegionsList.AddHotRegion(insertIndex, graph, array[0], array[1], array[2], point2, item.Name, num2 - 1);
											}
											else
											{
												common.HotRegionsList.AddHotRegion(graph, new RectangleF((float)(empty3.X - relativeSize.Width / 2.0), (float)(empty3.Y - relativeSize.Height / 2.0), relativeSize.Width, relativeSize.Height), point2, item.Name, num2 - 1);
											}
										}
									}
									num7++;
									if (item.MarkerStep == num7)
									{
										num7 = 0;
									}
								}
								graph.StartAnimation();
								this.DrawLabel(common, area, graph, item, point2, new PointF(x, (float)Math.Min(num9, num10)), num2);
								graph.StopAnimation();
								num2++;
							}
						}
						if (!selection)
						{
							common.EventsManager.OnPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
						}
					}
				}
			}
		}

		protected virtual void DrawOpenCloseMarks(ChartGraphics graph, ChartArea area, Series ser, DataPoint point, float xPosition, float width)
		{
			double logValue = this.vAxis.GetLogValue(point.YValues[2]);
			double logValue2 = this.vAxis.GetLogValue(point.YValues[3]);
			if ((logValue > this.vAxis.GetViewMaximum() || logValue < this.vAxis.GetViewMinimum()) && !(logValue2 > this.vAxis.GetViewMaximum()))
			{
				this.vAxis.GetViewMinimum();
			}
			float num = (float)this.vAxis.GetLinearPosition(logValue);
			float num2 = (float)this.vAxis.GetLinearPosition(logValue2);
			SizeF absoluteSize = graph.GetAbsoluteSize(new SizeF(width, width));
			float height = graph.GetRelativeSize(absoluteSize).Height;
			StockOpenCloseMarkStyle stockOpenCloseMarkStyle = this.openCloseStyle;
			string text = "";
			if (point.IsAttributeSet("OpenCloseStyle"))
			{
				text = ((DataPointAttributes)point)["OpenCloseStyle"];
			}
			else if (ser.IsAttributeSet("OpenCloseStyle"))
			{
				text = ((DataPointAttributes)ser)["OpenCloseStyle"];
			}
			if (text != null && text.Length > 0)
			{
				if (string.Compare(text, "Candlestick", StringComparison.OrdinalIgnoreCase) == 0)
				{
					stockOpenCloseMarkStyle = StockOpenCloseMarkStyle.Candlestick;
				}
				else if (string.Compare(text, "Triangle", StringComparison.OrdinalIgnoreCase) == 0)
				{
					stockOpenCloseMarkStyle = StockOpenCloseMarkStyle.Triangle;
				}
				else if (string.Compare(text, "Line", StringComparison.OrdinalIgnoreCase) == 0)
				{
					stockOpenCloseMarkStyle = StockOpenCloseMarkStyle.Line;
				}
			}
			bool flag = true;
			bool flag2 = true;
			string text2 = "";
			if (point.IsAttributeSet("ShowOpenClose"))
			{
				text2 = ((DataPointAttributes)point)["ShowOpenClose"];
			}
			else if (ser.IsAttributeSet("ShowOpenClose"))
			{
				text2 = ((DataPointAttributes)ser)["ShowOpenClose"];
			}
			if (text2 != null && text2.Length > 0)
			{
				if (string.Compare(text2, "Both", StringComparison.OrdinalIgnoreCase) == 0)
				{
					flag = true;
					flag2 = true;
				}
				else if (string.Compare(text2, "Open", StringComparison.OrdinalIgnoreCase) == 0)
				{
					flag = true;
					flag2 = false;
				}
				else if (string.Compare(text2, "Close", StringComparison.OrdinalIgnoreCase) == 0)
				{
					flag = false;
					flag2 = true;
				}
			}
			bool flag3 = false;
			if (stockOpenCloseMarkStyle == StockOpenCloseMarkStyle.Candlestick || xPosition - width / 2.0 < area.PlotAreaPosition.X || xPosition + width / 2.0 > area.PlotAreaPosition.Right())
			{
				graph.SetClip(area.PlotAreaPosition.ToRectangleF());
				flag3 = true;
			}
			if (!this.forceCandleStick)
			{
				switch (stockOpenCloseMarkStyle)
				{
				case StockOpenCloseMarkStyle.Candlestick:
					break;
				case StockOpenCloseMarkStyle.Triangle:
					goto IL_0428;
				default:
					goto IL_05a4;
				}
			}
			ColorConverter colorConverter = new ColorConverter();
			Color color = point.Color;
			Color color2 = point.BackGradientEndColor;
			string text3 = ((DataPointAttributes)point)["PriceUpColor"];
			if (text3 != null && text3.Length > 0)
			{
				try
				{
					color = (Color)colorConverter.ConvertFromString(text3);
				}
				catch
				{
					color = (Color)colorConverter.ConvertFromInvariantString(text3);
				}
			}
			text3 = ((DataPointAttributes)point)["PriceDownColor"];
			if (text3 != null && text3.Length > 0)
			{
				try
				{
					color2 = (Color)colorConverter.ConvertFromString(text3);
				}
				catch
				{
					color2 = (Color)colorConverter.ConvertFromInvariantString(text3);
				}
			}
			RectangleF empty = RectangleF.Empty;
			empty.Y = Math.Min(num, num2);
			empty.X = (float)(xPosition - width / 2.0);
			empty.Height = Math.Max(num, num2) - empty.Y;
			empty.Width = width;
			Color color3 = (num > num2) ? color : color2;
			Color color4 = (point.BorderColor == Color.Empty) ? ((color3 == Color.Empty) ? point.Color : color3) : point.BorderColor;
			SizeF relative = new SizeF(empty.Height, empty.Height);
			relative = graph.GetAbsoluteSize(relative);
			if (relative.Height > 1.0)
			{
				graph.FillRectangleRel(empty, color3, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, point.BackGradientEndColor, color4, point.BorderWidth, point.BorderStyle, ser.ShadowColor, ser.ShadowOffset, PenAlignment.Inset);
			}
			else
			{
				graph.DrawLineRel(color4, point.BorderWidth, point.BorderStyle, new PointF(empty.X, empty.Y), new PointF(empty.Right, empty.Y), ser.ShadowColor, ser.ShadowOffset);
			}
			goto IL_0664;
			IL_05a4:
			if (flag && logValue <= this.vAxis.GetViewMaximum() && logValue >= this.vAxis.GetViewMinimum())
			{
				graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, new PointF((float)(xPosition - width / 2.0), num), new PointF(xPosition, num), ser.ShadowColor, ser.ShadowOffset);
			}
			if (flag2 && logValue2 <= this.vAxis.GetViewMaximum() && logValue2 >= this.vAxis.GetViewMinimum())
			{
				graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, new PointF(xPosition, num2), new PointF((float)(xPosition + width / 2.0), num2), ser.ShadowColor, ser.ShadowOffset);
			}
			goto IL_0664;
			IL_0428:
			GraphicsPath graphicsPath = new GraphicsPath();
			PointF absolutePoint = graph.GetAbsolutePoint(new PointF(xPosition, num));
			PointF absolutePoint2 = graph.GetAbsolutePoint(new PointF((float)(xPosition - width / 2.0), (float)(num + height / 2.0)));
			PointF absolutePoint3 = graph.GetAbsolutePoint(new PointF((float)(xPosition - width / 2.0), (float)(num - height / 2.0)));
			if (flag && logValue <= this.vAxis.GetViewMaximum() && logValue >= this.vAxis.GetViewMinimum())
			{
				graphicsPath.AddLine(absolutePoint2, absolutePoint);
				graphicsPath.AddLine(absolutePoint, absolutePoint3);
				graphicsPath.AddLine(absolutePoint3, absolutePoint3);
				graph.FillPath(new SolidBrush(point.Color), graphicsPath);
			}
			if (flag2 && logValue2 <= this.vAxis.GetViewMaximum() && logValue2 >= this.vAxis.GetViewMinimum())
			{
				graphicsPath.Reset();
				absolutePoint = graph.GetAbsolutePoint(new PointF(xPosition, num2));
				absolutePoint2 = graph.GetAbsolutePoint(new PointF((float)(xPosition + width / 2.0), (float)(num2 + height / 2.0)));
				absolutePoint3 = graph.GetAbsolutePoint(new PointF((float)(xPosition + width / 2.0), (float)(num2 - height / 2.0)));
				graphicsPath.AddLine(absolutePoint2, absolutePoint);
				graphicsPath.AddLine(absolutePoint, absolutePoint3);
				graphicsPath.AddLine(absolutePoint3, absolutePoint3);
				graph.FillPath(new SolidBrush(point.Color), graphicsPath);
			}
			if (graphicsPath != null)
			{
				graphicsPath.Dispose();
			}
			goto IL_0664;
			IL_0664:
			if (flag3)
			{
				graph.ResetClip();
			}
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
				int num = 3;
				string strA = "";
				if (point.IsAttributeSet("LabelValueType"))
				{
					strA = ((DataPointAttributes)point)["LabelValueType"];
				}
				else if (ser.IsAttributeSet("LabelValueType"))
				{
					strA = ((DataPointAttributes)ser)["LabelValueType"];
				}
				if (string.Compare(strA, "High", StringComparison.OrdinalIgnoreCase) == 0)
				{
					num = 0;
				}
				else if (string.Compare(strA, "Low", StringComparison.OrdinalIgnoreCase) == 0)
				{
					num = 1;
				}
				else if (string.Compare(strA, "Open", StringComparison.OrdinalIgnoreCase) == 0)
				{
					num = 2;
				}
				text = ValueConverter.FormatValue(ser.chart, point, point.YValues[num], point.LabelFormat, ser.YValueType, ChartElementType.DataPoint);
			}
			else
			{
				text = point.ReplaceKeywords(point.Label);
				if (ser.chart != null && ser.chart.LocalizeTextHandler != null)
				{
					text = ser.chart.LocalizeTextHandler(point, text, point.ElementId, ChartElementType.DataPoint);
				}
			}
			int angle = point.FontAngle;
			if (text.Trim().Length != 0)
			{
				SizeF labelSize = SizeF.Empty;
				if (ser.SmartLabels.Enabled)
				{
					SizeF sizeF = SizeF.Empty;
					sizeF.Width = (float)point.MarkerSize;
					sizeF.Height = (float)point.MarkerSize;
					if (point.MarkerImage.Length > 0)
					{
						common.ImageLoader.GetAdjustedImageSize(point.MarkerImage, graph.Graphics, ref sizeF);
					}
					sizeF = graph.GetRelativeSize(sizeF);
					labelSize = graph.GetRelativeSize(graph.MeasureString(text, point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
					position = area.smartLabels.AdjustSmartLabelPosition(common, graph, area, ser.SmartLabels, position, labelSize, ref stringFormat, position, sizeF, LabelAlignmentTypes.Top);
					angle = 0;
				}
				if (!position.IsEmpty)
				{
					RectangleF backPosition = RectangleF.Empty;
					if (!point.LabelBackColor.IsEmpty || point.LabelBorderWidth > 0 || !point.LabelBorderColor.IsEmpty)
					{
						if (labelSize.IsEmpty)
						{
							labelSize = graph.GetRelativeSize(graph.MeasureString(text, point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
						}
						position.Y -= (float)(labelSize.Height / 8.0);
						SizeF size = new SizeF(labelSize.Width, labelSize.Height);
						size.Height += (float)(labelSize.Height / 8.0);
						size.Width += size.Width / (float)text.Length;
						backPosition = PointChart.GetLabelPosition(graph, position, size, stringFormat, true);
					}
					graph.DrawPointLabelStringRel(common, text, point.Font, new SolidBrush(point.FontColor), position, stringFormat, angle, backPosition, point.LabelBackColor, point.LabelBorderColor, point.LabelBorderWidth, point.LabelBorderStyle, ser, point, pointIndex - 1);
				}
			}
		}

		protected virtual void ProcessChartType3D(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			ArrayList seriesFromChartType = area.GetSeriesFromChartType(this.Name);
			bool flag = area.IndexedSeries((string[])seriesFromChartType.ToArray(typeof(string)));
			foreach (Series item in common.DataManager.Series)
			{
				if (string.Compare(item.ChartTypeName, this.Name, StringComparison.OrdinalIgnoreCase) == 0 && !(item.ChartArea != area.Name) && item.IsVisible() && (seriesToDraw == null || !(seriesToDraw.Name != item.Name)))
				{
					if (item.YValuesPerPoint < 4)
					{
						throw new ArgumentException(SR.ExceptionChartTypeRequiresYValues("StockChart", "4"));
					}
					this.hAxis = area.GetAxis(AxisName.X, item.XAxisType, item.XSubAxisName);
					this.vAxis = area.GetAxis(AxisName.Y, item.YAxisType, item.YSubAxisName);
					double interval = flag ? 1.0 : area.GetPointsInterval(this.hAxis.Logarithmic, this.hAxis.logarithmBase);
					float num = (float)item.GetPointWidth(graph, this.hAxis, interval, 0.8);
					if (!selection)
					{
						common.EventsManager.OnBackPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
					}
					float num2 = default(float);
					float num3 = default(float);
					((ChartArea3D)area).GetSeriesZPositionAndDepth(item, out num2, out num3);
					int num4 = 1;
					foreach (DataPoint point in item.Points)
					{
						point.positionRel = new PointF(float.NaN, float.NaN);
						double num5 = point.XValue;
						if (flag)
						{
							num5 = (double)num4;
						}
						float num6 = (float)this.hAxis.GetPosition(num5);
						double logValue = this.vAxis.GetLogValue(point.YValues[0]);
						double logValue2 = this.vAxis.GetLogValue(point.YValues[1]);
						num5 = this.hAxis.GetLogValue(num5);
						if (num5 < this.hAxis.GetViewMinimum() || num5 > this.hAxis.GetViewMaximum() || (logValue < this.vAxis.GetViewMinimum() && logValue2 < this.vAxis.GetViewMinimum()) || (logValue > this.vAxis.GetViewMaximum() && logValue2 > this.vAxis.GetViewMaximum()))
						{
							num4++;
						}
						else
						{
							bool flag2 = false;
							if (num5 == this.hAxis.GetViewMinimum() || num5 == this.hAxis.GetViewMaximum())
							{
								graph.SetClip(area.PlotAreaPosition.ToRectangleF());
								flag2 = true;
							}
							double num7 = this.vAxis.GetLogValue(point.YValues[0]);
							double num8 = this.vAxis.GetLogValue(point.YValues[1]);
							if (num7 > this.vAxis.GetViewMaximum())
							{
								num7 = this.vAxis.GetViewMaximum();
							}
							if (num7 < this.vAxis.GetViewMinimum())
							{
								num7 = this.vAxis.GetViewMinimum();
							}
							num7 = (double)(float)this.vAxis.GetLinearPosition(num7);
							if (num8 > this.vAxis.GetViewMaximum())
							{
								num8 = this.vAxis.GetViewMaximum();
							}
							if (num8 < this.vAxis.GetViewMinimum())
							{
								num8 = this.vAxis.GetViewMinimum();
							}
							num8 = this.vAxis.GetLinearPosition(num8);
							point.positionRel = new PointF(num6, (float)num7);
							Point3D[] array = new Point3D[2]
							{
								new Point3D(num6, (float)num7, (float)(num3 + num2 / 2.0)),
								new Point3D(num6, (float)num8, (float)(num3 + num2 / 2.0))
							};
							area.matrix3D.TransformPoints(array);
							graph.StartHotRegion(point);
							graph.StartAnimation();
							graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, array[0].PointF, array[1].PointF, item.ShadowColor, item.ShadowOffset);
							this.DrawOpenCloseMarks3D(graph, area, item, point, num6, num, num3, num2);
							num6 = array[0].X;
							num7 = (double)array[0].Y;
							num8 = (double)array[1].Y;
							graph.StopAnimation();
							graph.EndHotRegion();
							if (flag2)
							{
								graph.ResetClip();
							}
							if (common.ProcessModeRegions)
							{
								RectangleF empty = RectangleF.Empty;
								empty.X = (float)(num6 - num / 2.0);
								empty.Y = (float)Math.Min(num7, num8);
								empty.Width = num;
								empty.Height = (float)Math.Max(num7, num8) - empty.Y;
								common.HotRegionsList.AddHotRegion(graph, empty, point, item.Name, num4 - 1);
							}
							num4++;
						}
					}
					int num9 = 0;
					num4 = 1;
					foreach (DataPoint point2 in item.Points)
					{
						double num10 = point2.XValue;
						if (flag)
						{
							num10 = (double)num4;
						}
						float x = (float)this.hAxis.GetPosition(num10);
						double logValue3 = this.vAxis.GetLogValue(point2.YValues[0]);
						double logValue4 = this.vAxis.GetLogValue(point2.YValues[1]);
						num10 = this.hAxis.GetLogValue(num10);
						if (num10 < this.hAxis.GetViewMinimum() || num10 > this.hAxis.GetViewMaximum() || (logValue3 < this.vAxis.GetViewMinimum() && logValue4 < this.vAxis.GetViewMinimum()) || (logValue3 > this.vAxis.GetViewMaximum() && logValue4 > this.vAxis.GetViewMaximum()))
						{
							num4++;
						}
						else
						{
							double num11 = this.vAxis.GetLogValue(point2.YValues[0]);
							double num12 = this.vAxis.GetLogValue(point2.YValues[1]);
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
							Point3D[] array2 = new Point3D[2]
							{
								new Point3D(x, (float)num11, (float)(num3 + num2 / 2.0)),
								new Point3D(x, (float)num12, (float)(num3 + num2 / 2.0))
							};
							area.matrix3D.TransformPoints(array2);
							x = array2[0].X;
							num11 = (double)array2[0].Y;
							num12 = (double)array2[1].Y;
							graph.StartAnimation();
							this.DrawLabel(common, area, graph, item, point2, new PointF(x, (float)Math.Min(num11, num12)), num4);
							graph.StopAnimation();
							if (point2.MarkerStyle != 0 || point2.MarkerImage.Length > 0)
							{
								SizeF empty2 = SizeF.Empty;
								empty2.Width = (float)point2.MarkerSize;
								empty2.Height = (float)point2.MarkerSize;
								if (point2.MarkerImage.Length > 0)
								{
									common.ImageLoader.GetAdjustedImageSize(point2.MarkerImage, graph.Graphics, ref empty2);
								}
								PointF empty3 = PointF.Empty;
								empty3.X = x;
								empty3.Y = (float)((float)num11 - graph.GetRelativeSize(empty2).Height / 2.0);
								if (num9 == 0)
								{
									graph.StartAnimation();
									graph.DrawMarkerRel(empty3, point2.MarkerStyle, (int)empty2.Height, (point2.MarkerColor == Color.Empty) ? point2.Color : point2.MarkerColor, (point2.MarkerBorderColor == Color.Empty) ? point2.BorderColor : point2.MarkerBorderColor, point2.MarkerBorderWidth, point2.MarkerImage, point2.MarkerImageTransparentColor, (point2.series != null) ? point2.series.ShadowOffset : 0, (point2.series != null) ? point2.series.ShadowColor : Color.Empty, new RectangleF(empty3.X, empty3.Y, empty2.Width, empty2.Height));
									graph.StopAnimation();
									if (common.ProcessModeRegions)
									{
										SizeF relativeSize = graph.GetRelativeSize(empty2);
										int insertIndex = common.HotRegionsList.FindInsertIndex();
										common.HotRegionsList.FindInsertIndex();
										if (point2.MarkerStyle == MarkerStyle.Circle)
										{
											float[] array3 = new float[3]
											{
												empty3.X,
												empty3.Y,
												(float)(relativeSize.Width / 2.0)
											};
											common.HotRegionsList.AddHotRegion(insertIndex, graph, array3[0], array3[1], array3[2], point2, item.Name, num4 - 1);
										}
										else
										{
											common.HotRegionsList.AddHotRegion(graph, new RectangleF((float)(empty3.X - relativeSize.Width / 2.0), (float)(empty3.Y - relativeSize.Height / 2.0), relativeSize.Width, relativeSize.Height), point2, item.Name, num4 - 1);
										}
									}
								}
								num9++;
								if (item.MarkerStep == num9)
								{
									num9 = 0;
								}
							}
							num4++;
						}
					}
					if (!selection)
					{
						common.EventsManager.OnPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
					}
				}
			}
		}

		protected virtual void DrawOpenCloseMarks3D(ChartGraphics graph, ChartArea area, Series ser, DataPoint point, float xPosition, float width, float zPosition, float depth)
		{
			double logValue = this.vAxis.GetLogValue(point.YValues[2]);
			double logValue2 = this.vAxis.GetLogValue(point.YValues[3]);
			if ((logValue > this.vAxis.GetViewMaximum() || logValue < this.vAxis.GetViewMinimum()) && !(logValue2 > this.vAxis.GetViewMaximum()))
			{
				this.vAxis.GetViewMinimum();
			}
			float num = (float)this.vAxis.GetLinearPosition(logValue);
			float num2 = (float)this.vAxis.GetLinearPosition(logValue2);
			SizeF absoluteSize = graph.GetAbsoluteSize(new SizeF(width, width));
			float height = graph.GetRelativeSize(absoluteSize).Height;
			StockOpenCloseMarkStyle stockOpenCloseMarkStyle = this.openCloseStyle;
			string text = "";
			if (point.IsAttributeSet("OpenCloseStyle"))
			{
				text = ((DataPointAttributes)point)["OpenCloseStyle"];
			}
			else if (ser.IsAttributeSet("OpenCloseStyle"))
			{
				text = ((DataPointAttributes)ser)["OpenCloseStyle"];
			}
			if (text != null && text.Length > 0)
			{
				if (string.Compare(text, "Candlestick", StringComparison.OrdinalIgnoreCase) == 0)
				{
					stockOpenCloseMarkStyle = StockOpenCloseMarkStyle.Candlestick;
				}
				else if (string.Compare(text, "Triangle", StringComparison.OrdinalIgnoreCase) == 0)
				{
					stockOpenCloseMarkStyle = StockOpenCloseMarkStyle.Triangle;
				}
				else if (string.Compare(text, "Line", StringComparison.OrdinalIgnoreCase) == 0)
				{
					stockOpenCloseMarkStyle = StockOpenCloseMarkStyle.Line;
				}
			}
			bool flag = true;
			bool flag2 = true;
			string text2 = "";
			if (point.IsAttributeSet("ShowOpenClose"))
			{
				text2 = ((DataPointAttributes)point)["ShowOpenClose"];
			}
			else if (ser.IsAttributeSet("ShowOpenClose"))
			{
				text2 = ((DataPointAttributes)ser)["ShowOpenClose"];
			}
			if (text2 != null && text2.Length > 0)
			{
				if (string.Compare(text2, "Both", StringComparison.OrdinalIgnoreCase) == 0)
				{
					flag = true;
					flag2 = true;
				}
				else if (string.Compare(text2, "Open", StringComparison.OrdinalIgnoreCase) == 0)
				{
					flag = true;
					flag2 = false;
				}
				else if (string.Compare(text2, "Close", StringComparison.OrdinalIgnoreCase) == 0)
				{
					flag = false;
					flag2 = true;
				}
			}
			bool flag3 = false;
			if (xPosition - width / 2.0 < area.PlotAreaPosition.X || xPosition + width / 2.0 > area.PlotAreaPosition.Right())
			{
				graph.SetClip(area.PlotAreaPosition.ToRectangleF());
				flag3 = true;
			}
			if (!this.forceCandleStick)
			{
				switch (stockOpenCloseMarkStyle)
				{
				case StockOpenCloseMarkStyle.Candlestick:
					break;
				case StockOpenCloseMarkStyle.Triangle:
					goto IL_04ad;
				default:
					goto IL_0759;
				}
			}
			ColorConverter colorConverter = new ColorConverter();
			Color color = point.Color;
			Color color2 = point.BackGradientEndColor;
			string text3 = ((DataPointAttributes)point)["PriceUpColor"];
			if (text3 != null && text3.Length > 0)
			{
				try
				{
					color = (Color)colorConverter.ConvertFromString(text3);
				}
				catch
				{
					color = (Color)colorConverter.ConvertFromInvariantString(text3);
				}
			}
			text3 = ((DataPointAttributes)point)["PriceDownColor"];
			if (text3 != null && text3.Length > 0)
			{
				try
				{
					color2 = (Color)colorConverter.ConvertFromString(text3);
				}
				catch
				{
					color2 = (Color)colorConverter.ConvertFromInvariantString(text3);
				}
			}
			RectangleF empty = RectangleF.Empty;
			empty.Y = Math.Min(num, num2);
			empty.X = (float)(xPosition - width / 2.0);
			empty.Height = Math.Max(num, num2) - empty.Y;
			empty.Width = width;
			Color color3 = (num > num2) ? color : color2;
			Color color4 = (point.BorderColor == Color.Empty) ? ((color3 == Color.Empty) ? point.Color : color3) : point.BorderColor;
			Point3D[] array = new Point3D[2]
			{
				new Point3D(empty.X, empty.Y, (float)(zPosition + depth / 2.0)),
				new Point3D(empty.Right, empty.Bottom, (float)(zPosition + depth / 2.0))
			};
			area.matrix3D.TransformPoints(array);
			empty.Location = array[0].PointF;
			empty.Width = Math.Abs(array[1].X - array[0].X);
			empty.Height = Math.Abs(array[1].Y - array[0].Y);
			if (empty.Height > 1.0)
			{
				graph.FillRectangleRel(empty, color3, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, point.BackGradientEndColor, color4, point.BorderWidth, point.BorderStyle, ser.ShadowColor, ser.ShadowOffset, PenAlignment.Inset);
			}
			else
			{
				graph.DrawLineRel(color4, point.BorderWidth, point.BorderStyle, new PointF(empty.X, empty.Y), new PointF(empty.Right, empty.Y), ser.ShadowColor, ser.ShadowOffset);
			}
			goto IL_08b5;
			IL_04ad:
			GraphicsPath graphicsPath = new GraphicsPath();
			Point3D[] array2 = new Point3D[3]
			{
				new Point3D(xPosition, num, (float)(zPosition + depth / 2.0)),
				new Point3D((float)(xPosition - width / 2.0), (float)(num + height / 2.0), (float)(zPosition + depth / 2.0)),
				new Point3D((float)(xPosition - width / 2.0), (float)(num - height / 2.0), (float)(zPosition + depth / 2.0))
			};
			area.matrix3D.TransformPoints(array2);
			array2[0].PointF = graph.GetAbsolutePoint(array2[0].PointF);
			array2[1].PointF = graph.GetAbsolutePoint(array2[1].PointF);
			array2[2].PointF = graph.GetAbsolutePoint(array2[2].PointF);
			if (flag && logValue <= this.vAxis.GetViewMaximum() && logValue >= this.vAxis.GetViewMinimum())
			{
				graphicsPath.AddLine(array2[1].PointF, array2[0].PointF);
				graphicsPath.AddLine(array2[0].PointF, array2[2].PointF);
				graphicsPath.AddLine(array2[2].PointF, array2[2].PointF);
				graph.FillPath(new SolidBrush(point.Color), graphicsPath);
			}
			if (flag2 && logValue2 <= this.vAxis.GetViewMaximum() && logValue2 >= this.vAxis.GetViewMinimum())
			{
				array2[0] = new Point3D(xPosition, num2, (float)(zPosition + depth / 2.0));
				array2[1] = new Point3D((float)(xPosition + width / 2.0), (float)(num2 + height / 2.0), (float)(zPosition + depth / 2.0));
				array2[2] = new Point3D((float)(xPosition + width / 2.0), (float)(num2 - height / 2.0), (float)(zPosition + depth / 2.0));
				area.matrix3D.TransformPoints(array2);
				array2[0].PointF = graph.GetAbsolutePoint(array2[0].PointF);
				array2[1].PointF = graph.GetAbsolutePoint(array2[1].PointF);
				array2[2].PointF = graph.GetAbsolutePoint(array2[2].PointF);
				graphicsPath.Reset();
				graphicsPath.AddLine(array2[1].PointF, array2[0].PointF);
				graphicsPath.AddLine(array2[0].PointF, array2[2].PointF);
				graphicsPath.AddLine(array2[2].PointF, array2[2].PointF);
				graph.FillPath(new SolidBrush(point.Color), graphicsPath);
			}
			if (graphicsPath != null)
			{
				graphicsPath.Dispose();
			}
			goto IL_08b5;
			IL_0759:
			if (flag && logValue <= this.vAxis.GetViewMaximum() && logValue >= this.vAxis.GetViewMinimum())
			{
				Point3D[] array3 = new Point3D[2]
				{
					new Point3D((float)(xPosition - width / 2.0), num, (float)(zPosition + depth / 2.0)),
					new Point3D(xPosition, num, (float)(zPosition + depth / 2.0))
				};
				area.matrix3D.TransformPoints(array3);
				graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, array3[0].PointF, array3[1].PointF, ser.ShadowColor, ser.ShadowOffset);
			}
			if (flag2 && logValue2 <= this.vAxis.GetViewMaximum() && logValue2 >= this.vAxis.GetViewMinimum())
			{
				Point3D[] array4 = new Point3D[2]
				{
					new Point3D(xPosition, num2, (float)(zPosition + depth / 2.0)),
					new Point3D((float)(xPosition + width / 2.0), num2, (float)(zPosition + depth / 2.0))
				};
				area.matrix3D.TransformPoints(array4);
				graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, array4[0].PointF, array4[1].PointF, ser.ShadowColor, ser.ShadowOffset);
			}
			goto IL_08b5;
			IL_08b5:
			if (flag3)
			{
				graph.ResetClip();
			}
		}

		public virtual double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			return point.YValues[yValueIndex];
		}

		public void AddSmartLabelMarkerPositions(CommonElements common, ChartArea area, Series series, ArrayList list)
		{
			bool flag = area.IndexedSeries((string[])area.GetSeriesFromChartType(this.Name).ToArray(typeof(string)));
			Axis axis = area.GetAxis(AxisName.X, series.XAxisType, series.XSubAxisName);
			Axis axis2 = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName);
			int num = 0;
			int num2 = 1;
			foreach (DataPoint point in series.Points)
			{
				double yValue = this.GetYValue(common, area, series, point, num2 - 1, 0);
				yValue = axis2.GetLogValue(yValue);
				if (yValue > axis2.GetViewMaximum() || yValue < axis2.GetViewMinimum())
				{
					num2++;
				}
				else
				{
					double yValue2 = flag ? ((double)num2) : point.XValue;
					yValue2 = axis.GetLogValue(yValue2);
					if (yValue2 > axis.GetViewMaximum() || yValue2 < axis.GetViewMinimum())
					{
						num2++;
					}
					else
					{
						PointF pointF = PointF.Empty;
						pointF.Y = (float)axis2.GetLinearPosition(yValue);
						if (flag)
						{
							pointF.X = (float)axis.GetPosition((double)num2);
						}
						else
						{
							pointF.X = (float)axis.GetPosition(point.XValue);
						}
						int markerSize = point.MarkerSize;
						string markerImage = point.MarkerImage;
						MarkerStyle markerStyle = point.MarkerStyle;
						SizeF size = SizeF.Empty;
						size.Width = (float)point.MarkerSize;
						size.Height = (float)point.MarkerSize;
						if (point.MarkerImage.Length > 0 && common.graph != null)
						{
							common.ImageLoader.GetAdjustedImageSize(point.MarkerImage, common.graph.Graphics, ref size);
						}
						if (area.Area3DStyle.Enable3D)
						{
							float num3 = default(float);
							float num4 = default(float);
							((ChartArea3D)area).GetSeriesZPositionAndDepth(series, out num3, out num4);
							Point3D[] array = new Point3D[1]
							{
								new Point3D(pointF.X, pointF.Y, (float)(num4 + num3 / 2.0))
							};
							area.matrix3D.TransformPoints(array);
							pointF = array[0].PointF;
						}
						if (markerStyle != 0 || markerImage.Length > 0)
						{
							if (num == 0)
							{
								size = common.graph.GetRelativeSize(size);
								RectangleF rectangleF = new RectangleF((float)(pointF.X - size.Width / 2.0), pointF.Y - size.Height, size.Width, size.Height);
								list.Add(rectangleF);
							}
							num++;
							if (series.MarkerStep == num)
							{
								num = 0;
							}
						}
						num2++;
					}
				}
			}
		}
	}
}
