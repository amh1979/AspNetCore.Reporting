using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class BarChart : IChartType
	{
		protected bool useTwoValues;

		protected bool drawSeriesSideBySide = true;

		protected BarValueLabelDrawingStyle defLabelDrawingStyle;

		protected bool pointLabelsMarkersPresent;

		public virtual string Name
		{
			get
			{
				return "Bar";
			}
		}

		public bool Stacked
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

		public virtual bool SideBySideSeries
		{
			get
			{
				return true;
			}
		}

		public virtual bool ZeroCrossing
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

		public virtual int YValuesPerPoint
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

		public void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			this.pointLabelsMarkersPresent = false;
			this.ProcessChartType(false, false, graph, common, area, seriesToDraw);
			if (this.pointLabelsMarkersPresent)
			{
				this.ProcessChartType(true, false, graph, common, area, seriesToDraw);
			}
		}

		private void ProcessChartType(bool labels, bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			int num = 0;
			bool flag = false;
			SizeF relativeSize = graph.GetRelativeSize(new SizeF(1.1f, 1.1f));
			if (area.Area3DStyle.Enable3D)
			{
				this.ProcessChartType3D(selection, graph, common, area, seriesToDraw);
			}
			else
			{
				ArrayList seriesFromChartType = area.GetSeriesFromChartType(this.Name);
				bool flag2 = this.drawSeriesSideBySide;
				foreach (string item in seriesFromChartType)
				{
					if (common.DataManager.Series[item].IsAttributeSet("DrawSideBySide"))
					{
						string strA = ((DataPointAttributes)common.DataManager.Series[item])["DrawSideBySide"];
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
				}
				int num2 = seriesFromChartType.Count;
				if (!flag2)
				{
					num2 = 1;
				}
				bool flag3 = area.IndexedSeries((string[])seriesFromChartType.ToArray(typeof(string)));
				foreach (Series item2 in common.DataManager.Series)
				{
					if (string.Compare(item2.ChartTypeName, this.Name, true, CultureInfo.CurrentCulture) == 0 && !(item2.ChartArea != area.Name) && item2.Points.Count != 0 && item2.IsVisible())
					{
						Axis axis = area.GetAxis(AxisName.X, item2.XAxisType, item2.XSubAxisName);
						double viewMaximum = axis.GetViewMaximum();
						double viewMinimum = axis.GetViewMinimum();
						Axis axis2 = area.GetAxis(AxisName.Y, item2.YAxisType, item2.YSubAxisName);
						double viewMaximum2 = axis2.GetViewMaximum();
						double viewMinimum2 = axis2.GetViewMinimum();
						double interval = 1.0;
						if (!flag3)
						{
							if (item2.Points.Count == 1 && (item2.XValueType == ChartValueTypes.Date || item2.XValueType == ChartValueTypes.DateTime || item2.XValueType == ChartValueTypes.Time || item2.XValueType == ChartValueTypes.DateTimeOffset))
							{
								((ChartAreaAxes)area).GetPointsInterval(seriesFromChartType, axis.Logarithmic, axis.logarithmBase, true, out flag);
								interval = ((double.IsNaN(axis.majorGrid.Interval) || axis.majorGrid.IntervalType == DateTimeIntervalType.NotSet) ? axis.GetIntervalSize(axis.minimum, axis.Interval, axis.IntervalType) : axis.GetIntervalSize(axis.minimum, axis.majorGrid.Interval, axis.majorGrid.IntervalType));
							}
							else
							{
								interval = ((ChartAreaAxes)area).GetPointsInterval(seriesFromChartType, axis.Logarithmic, axis.logarithmBase, true, out flag);
							}
						}
						double num3 = item2.GetPointWidth(graph, axis, interval, 0.8) / (double)num2;
						if (!selection)
						{
							common.EventsManager.OnBackPaint(item2, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
						}
						int num4 = 0;
						int num5 = 0;
						foreach (DataPoint point in item2.Points)
						{
							if (point.YValues.Length < this.YValuesPerPoint)
							{
								throw new InvalidOperationException(SR.ExceptionChartTypeRequiresYValues(this.Name, this.YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
							}
							point.positionRel = new PointF(float.NaN, float.NaN);
							double num6 = axis2.GetLogValue(this.GetYValue(common, area, item2, point, num4, this.useTwoValues ? 1 : 0));
							bool flag4 = false;
							bool flag5 = true;
							if (num6 > viewMaximum2)
							{
								num6 = viewMaximum2;
								flag4 = true;
							}
							else if (num6 < viewMinimum2)
							{
								num6 = viewMinimum2;
								flag4 = true;
							}
							double num7 = axis2.GetLinearPosition(num6);
							double num8 = 0.0;
							if (this.useTwoValues)
							{
								double num9 = axis2.GetLogValue(this.GetYValue(common, area, item2, point, num4, 0));
								flag5 = false;
								if (num9 > viewMaximum2)
								{
									num9 = viewMaximum2;
									flag5 = true;
								}
								else if (num9 < viewMinimum2)
								{
									num9 = viewMinimum2;
									flag5 = true;
								}
								num8 = axis2.GetLinearPosition(num9);
							}
							else
							{
								num8 = axis2.GetPosition(axis2.Crossing);
							}
							double num10 = 0.0;
							num10 = ((!flag3) ? ((!flag) ? axis.GetPosition(point.XValue) : (axis.GetPosition(point.XValue) - num3 * (double)num2 / 2.0 + num3 / 2.0 + (double)num * num3)) : (axis.GetPosition((double)num4 + 1.0) - num3 * (double)num2 / 2.0 + num3 / 2.0 + (double)num * num3));
							if (num7 < num8 && num8 - num7 < (double)relativeSize.Width)
							{
								num7 = num8 - (double)relativeSize.Width;
							}
							if (num7 > num8 && num7 - num8 < (double)relativeSize.Width)
							{
								num7 = num8 + (double)relativeSize.Width;
							}
							RectangleF empty = RectangleF.Empty;
							try
							{
								empty.Y = (float)(num10 - num3 / 2.0);
								empty.Height = (float)num3;
								if (num8 < num7)
								{
									empty.X = (float)num8;
									empty.Width = (float)num7 - empty.X;
								}
								else
								{
									empty.X = (float)num7;
									empty.Width = (float)num8 - empty.X;
								}
							}
							catch (Exception)
							{
								num4++;
								continue;
							}
							point.positionRel = new PointF((num8 < num7) ? empty.Right : empty.X, (float)num10);
							if (common.ProcessModePaint)
							{
								if (!point.Empty && !labels)
								{
									double yValue = flag3 ? ((double)(num4 + 1)) : point.XValue;
									yValue = axis.GetLogValue(yValue);
									if (!(yValue < viewMinimum) && !(yValue > viewMaximum))
									{
										bool flag6 = false;
										if (empty.Y < area.PlotAreaPosition.Y || empty.Bottom > area.PlotAreaPosition.Bottom())
										{
											graph.SetClip(area.PlotAreaPosition.ToRectangleF());
											flag6 = true;
										}
										graph.StartHotRegion(point);
										graph.StartAnimation();
										graph.FillRectangleRel(empty, point.Color, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, point.BackGradientEndColor, point.BorderColor, point.BorderWidth, point.BorderStyle, item2.ShadowColor, item2.ShadowOffset, PenAlignment.Inset, ChartGraphics.GetBarDrawingStyle(point), false);
										graph.StopAnimation();
										graph.EndHotRegion();
										if (flag6)
										{
											graph.ResetClip();
										}
										goto IL_06fa;
									}
									num4++;
									continue;
								}
								goto IL_06fa;
							}
							goto IL_07c7;
							IL_07c7:
							if (common.ProcessModeRegions)
							{
								if (!labels)
								{
									common.HotRegionsList.AddHotRegion(graph, empty, point, item2.Name, num4);
								}
								else if (!common.ProcessModePaint)
								{
									this.DrawLabelsAndMarkers(area, axis2, graph, common, empty, point, item2, num8, num7, num3, num4, ref num5);
								}
							}
							num4++;
							continue;
							IL_06fa:
							if ((!flag4 || !flag5 || empty.Width != 0.0) && empty.Y + empty.Height / 2.0 >= area.PlotAreaPosition.Y && empty.Y + empty.Height / 2.0 <= area.PlotAreaPosition.Bottom())
							{
								if (labels)
								{
									this.DrawLabelsAndMarkers(area, axis2, graph, common, empty, point, item2, num8, num7, num3, num4, ref num5);
								}
								else if (point.MarkerStyle != 0 || point.MarkerImage.Length > 0)
								{
									this.pointLabelsMarkersPresent = true;
								}
								else if (item2.ShowLabelAsValue || point.ShowLabelAsValue || point.Label.Length > 0)
								{
									this.pointLabelsMarkersPresent = true;
								}
							}
							goto IL_07c7;
						}
						if (!selection)
						{
							common.EventsManager.OnPaint(item2, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
						}
						if (flag2)
						{
							num++;
						}
					}
				}
			}
		}

		private void DrawLabelsAndMarkers(ChartArea area, Axis hAxis, ChartGraphics graph, CommonElements common, RectangleF rectSize, DataPoint point, Series ser, double barStartPosition, double barSize, double width, int pointIndex, ref int markerIndex)
		{
			SizeF sizeF = SizeF.Empty;
			if (point.MarkerStyle != 0 || point.MarkerImage.Length > 0)
			{
				if (markerIndex == 0)
				{
					if (point.MarkerImage.Length == 0)
					{
						sizeF.Width = (float)point.MarkerSize;
						sizeF.Height = (float)point.MarkerSize;
					}
					else
					{
						common.ImageLoader.GetAdjustedImageSize(point.MarkerImage, graph.Graphics, ref sizeF);
					}
					sizeF = graph.GetRelativeSize(sizeF);
					PointF empty = PointF.Empty;
					if (barStartPosition < barSize)
					{
						empty.X = rectSize.Right;
					}
					else
					{
						empty.X = rectSize.X;
					}
					empty.Y = (float)(rectSize.Y + rectSize.Height / 2.0);
					if (common.ProcessModePaint)
					{
						graph.StartAnimation();
						graph.DrawMarkerRel(empty, point.MarkerStyle, point.MarkerSize, (point.MarkerColor == Color.Empty) ? point.Color : point.MarkerColor, point.MarkerBorderColor, point.MarkerBorderWidth, point.MarkerImage, point.MarkerImageTransparentColor, (point.series != null) ? point.series.ShadowOffset : 0, (point.series != null) ? point.series.ShadowColor : Color.Empty, RectangleF.Empty);
						graph.StopAnimation();
					}
					if (common.ProcessModeRegions)
					{
						this.SetHotRegions(common, graph, point, sizeF, point.series.Name, pointIndex, point.MarkerStyle, empty);
					}
				}
				markerIndex++;
				if (ser.MarkerStep == markerIndex)
				{
					markerIndex = 0;
				}
			}
			if (point.Label.Length <= 0)
			{
				if (point.Empty)
				{
					return;
				}
				if (!ser.ShowLabelAsValue && !point.ShowLabelAsValue)
				{
					return;
				}
			}
			RectangleF position = RectangleF.Empty;
			StringFormat stringFormat = new StringFormat();
			string text;
			if (point.Label.Length == 0)
			{
				text = ValueConverter.FormatValue(ser.chart, point, this.GetYValue(common, area, ser, point, pointIndex, 0), point.LabelFormat, ser.YValueType, ChartElementType.DataPoint);
			}
			else
			{
				text = point.ReplaceKeywords(point.Label);
				if (ser.chart != null && ser.chart.LocalizeTextHandler != null)
				{
					text = ser.chart.LocalizeTextHandler(point, text, point.ElementId, ChartElementType.DataPoint);
				}
			}
			BarValueLabelDrawingStyle barValueLabelDrawingStyle = this.defLabelDrawingStyle;
			string text2 = "";
			if (point.IsAttributeSet("BarLabelStyle"))
			{
				text2 = ((DataPointAttributes)point)["BarLabelStyle"];
			}
			else if (ser.IsAttributeSet("BarLabelStyle"))
			{
				text2 = ((DataPointAttributes)ser)["BarLabelStyle"];
			}
			if (text2.Length > 0)
			{
				if (string.Compare(text2, "Left", StringComparison.OrdinalIgnoreCase) == 0)
				{
					barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Left;
				}
				if (string.Compare(text2, "Right", StringComparison.OrdinalIgnoreCase) == 0)
				{
					barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Right;
				}
				if (string.Compare(text2, "Center", StringComparison.OrdinalIgnoreCase) == 0)
				{
					barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Center;
				}
				else if (string.Compare(text2, "Outside", StringComparison.OrdinalIgnoreCase) == 0)
				{
					barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Outside;
				}
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			float num = 0f;
			while (!flag)
			{
				stringFormat.Alignment = StringAlignment.Near;
				stringFormat.LineAlignment = StringAlignment.Center;
				if (barStartPosition <= barSize)
				{
					position.X = rectSize.Right;
					position.Width = area.PlotAreaPosition.Right() - rectSize.Right;
					if (position.Width < 0.0010000000474974513 && barStartPosition == barSize)
					{
						position.Width = rectSize.X - area.PlotAreaPosition.X;
						position.X = area.PlotAreaPosition.X;
						stringFormat.Alignment = StringAlignment.Far;
					}
				}
				else
				{
					position.X = area.PlotAreaPosition.X;
					position.Width = rectSize.X - area.PlotAreaPosition.X;
				}
				position.Y = (float)(rectSize.Y - (float)width / 2.0);
				position.Height = rectSize.Height + (float)width;
				switch (barValueLabelDrawingStyle)
				{
				case BarValueLabelDrawingStyle.Outside:
					if (!sizeF.IsEmpty)
					{
						position.Width -= Math.Min(position.Width, (float)(sizeF.Width / 2.0));
						if (barStartPosition < barSize)
						{
							position.X += Math.Min(position.Width, (float)(sizeF.Width / 2.0));
						}
					}
					break;
				case BarValueLabelDrawingStyle.Left:
					position = rectSize;
					stringFormat.Alignment = StringAlignment.Near;
					break;
				case BarValueLabelDrawingStyle.Center:
					position = rectSize;
					stringFormat.Alignment = StringAlignment.Center;
					break;
				case BarValueLabelDrawingStyle.Right:
					position = rectSize;
					stringFormat.Alignment = StringAlignment.Far;
					if (!sizeF.IsEmpty)
					{
						position.Width -= Math.Min(position.Width, (float)(sizeF.Width / 2.0));
						if (barStartPosition >= barSize)
						{
							position.X += Math.Min(position.Width, (float)(sizeF.Width / 2.0));
						}
					}
					break;
				}
				if (barStartPosition > barSize)
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
				SizeF sizeF2 = graph.MeasureStringRel(text, point.Font);
				if (!flag2 && !flag3 && sizeF2.Width > position.Width - 1.0)
				{
					flag2 = true;
					num = position.Width;
					barValueLabelDrawingStyle = (BarValueLabelDrawingStyle)((barValueLabelDrawingStyle == BarValueLabelDrawingStyle.Outside) ? 3 : 0);
				}
				else if (flag2 && !flag3 && sizeF2.Width > position.Width - 1.0 && num > position.Width)
				{
					barValueLabelDrawingStyle = (BarValueLabelDrawingStyle)((barValueLabelDrawingStyle == BarValueLabelDrawingStyle.Outside) ? 3 : 0);
				}
				else
				{
					flag = true;
				}
			}
			graph.StartAnimation();
			RectangleF backPosition = RectangleF.Empty;
			if ((common.ProcessModeRegions || !point.LabelBackColor.IsEmpty || !point.LabelBorderColor.IsEmpty) && position.Width > 0.0 && position.Height > 0.0)
			{
				SizeF sizeF3 = graph.MeasureStringRel(text, point.Font);
				sizeF3.Height += (float)(sizeF3.Height / 8.0);
				float num2 = (float)(sizeF3.Width / (float)text.Length / 2.0);
				sizeF3.Width += num2;
				backPosition = new RectangleF(position.X, (float)(position.Y + (position.Height - sizeF3.Height) / 2.0), sizeF3.Width, sizeF3.Height);
				if (stringFormat.Alignment == StringAlignment.Near)
				{
					backPosition.X += (float)(num2 / 2.0);
					position.X += num2;
				}
				else if (stringFormat.Alignment == StringAlignment.Center)
				{
					backPosition.X = (float)(position.X + (position.Width - sizeF3.Width) / 2.0);
				}
				else if (stringFormat.Alignment == StringAlignment.Far)
				{
					backPosition.X = (float)(position.Right - sizeF3.Width - num2 / 2.0);
					position.X -= num2;
				}
			}
			SizeF sizeF4 = graph.MeasureStringRel(text, point.Font);
			if (sizeF4.Height > position.Height)
			{
				position.Y -= (float)((sizeF4.Height - position.Height) / 2.0);
				position.Height = sizeF4.Height;
			}
			graph.DrawPointLabelStringRel(common, text, point.Font, new SolidBrush(point.FontColor), position, stringFormat, point.FontAngle, backPosition, point.LabelBackColor, point.LabelBorderColor, point.LabelBorderWidth, point.LabelBorderStyle, ser, point, pointIndex);
			graph.StopAnimation();
		}

		private void SetHotRegions(CommonElements common, ChartGraphics graph, DataPoint point, SizeF markerSize, string seriesName, int pointIndex, MarkerStyle pointMarkerStyle, PointF markerPosition)
		{
			SizeF sizeF = markerSize;
			int insertIndex = common.HotRegionsList.FindInsertIndex();
			if (pointMarkerStyle == MarkerStyle.Circle)
			{
				common.HotRegionsList.AddHotRegion(insertIndex, graph, markerPosition.X, markerPosition.Y, (float)(sizeF.Width / 2.0), point, seriesName, pointIndex);
			}
			else
			{
				common.HotRegionsList.AddHotRegion(graph, new RectangleF((float)(markerPosition.X - sizeF.Width / 2.0), (float)(markerPosition.Y - sizeF.Height / 2.0), sizeF.Width, sizeF.Height), point, seriesName, pointIndex);
			}
		}

		public virtual double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			if (yValueIndex == -1)
			{
				return 0.0;
			}
			if (point.YValues.Length <= yValueIndex)
			{
				throw new InvalidOperationException(SR.ExceptionChartTypeRequiresYValues(this.Name, this.YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
			}
			if (!point.Empty && !double.IsNaN(point.YValues[yValueIndex]))
			{
				return point.YValues[yValueIndex];
			}
			double num = this.GetEmptyPointValue(point, pointIndex, yValueIndex);
			if (num == 0.0)
			{
				Axis axis = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName);
				double maximum = axis.maximum;
				double minimum = axis.minimum;
				if (num < minimum)
				{
					num = minimum;
				}
				else if (num > maximum)
				{
					num = maximum;
				}
			}
			return num;
		}

		internal double GetEmptyPointValue(DataPoint point, int pointIndex, int yValueIndex)
		{
			Series series = point.series;
			double num = 0.0;
			double num2 = 0.0;
			int index = 0;
			int index2 = series.Points.Count - 1;
			string strA = "";
			if (series.EmptyPointStyle.IsAttributeSet("EmptyPointValue"))
			{
				strA = series.EmptyPointStyle["EmptyPointValue"];
			}
			else if (series.IsAttributeSet("EmptyPointValue"))
			{
				strA = ((DataPointAttributes)series)["EmptyPointValue"];
			}
			if (string.Compare(strA, "Zero", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return 0.0;
			}
			int num3 = pointIndex;
			while (num3 >= 0)
			{
				if (series.Points[num3].Empty)
				{
					num = double.NaN;
					num3--;
					continue;
				}
				num = series.Points[num3].YValues[yValueIndex];
				index = num3;
				break;
			}
			int num4 = pointIndex;
			while (num4 < series.Points.Count)
			{
				if (series.Points[num4].Empty)
				{
					num2 = double.NaN;
					num4++;
					continue;
				}
				num2 = series.Points[num4].YValues[yValueIndex];
				index2 = num4;
				break;
			}
			if (double.IsNaN(num))
			{
				num = ((!double.IsNaN(num2)) ? num2 : 0.0);
			}
			if (double.IsNaN(num2))
			{
				num2 = num;
			}
			if (series.Points[index2].XValue == series.Points[index].XValue)
			{
				return (num + num2) / 2.0;
			}
			double num5 = (num - num2) / (series.Points[index2].XValue - series.Points[index].XValue);
			return (0.0 - num5) * (point.XValue - series.Points[index].XValue) + num;
		}

		private void ProcessChartType3D(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			SizeF relativeSize = graph.GetRelativeSize(new SizeF(1.1f, 1.1f));
			double num = 0.0;
			ArrayList arrayList = null;
			bool sideBySide = this.drawSeriesSideBySide;
			if (area.Area3DStyle.Clustered && this.SideBySideSeries)
			{
				goto IL_004a;
			}
			if (this.Stacked)
			{
				goto IL_004a;
			}
			arrayList = new ArrayList();
			arrayList.Add(seriesToDraw.Name);
			goto IL_0126;
			IL_0126:
			ArrayList dataPointDrawingOrder = area.GetDataPointDrawingOrder(arrayList, this, selection, COPCoordinates.X | COPCoordinates.Y, new BarPointsDrawingOrderComparer(area, selection, COPCoordinates.X | COPCoordinates.Y), 0, sideBySide);
			bool flag = false;
			foreach (object item in dataPointDrawingOrder)
			{
				DataPoint3D dataPoint3D = (DataPoint3D)item;
				DataPoint dataPoint = dataPoint3D.dataPoint;
				Series series = dataPoint.series;
				if (dataPoint.YValues.Length < this.YValuesPerPoint)
				{
					throw new InvalidOperationException(SR.ExceptionChartTypeRequiresYValues(this.Name, this.YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
				}
				dataPoint.positionRel = new PointF(float.NaN, float.NaN);
				Axis axis = area.GetAxis(AxisName.X, series.XAxisType, series.XSubAxisName);
				Axis axis2 = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName);
				BarDrawingStyle barDrawingStyle = ChartGraphics.GetBarDrawingStyle(dataPoint);
				float num2 = 0f;
				float num3 = 0f;
				double num4 = axis2.GetLogValue(this.GetYValue(common, area, series, dataPoint3D.dataPoint, dataPoint3D.index - 1, this.useTwoValues ? 1 : 0));
				if (num4 > axis2.GetViewMaximum())
				{
					num2 = 0.5f;
					num4 = axis2.GetViewMaximum();
				}
				else if (num4 < axis2.GetViewMinimum())
				{
					num2 = 0.5f;
					num4 = axis2.GetViewMinimum();
				}
				double num5 = axis2.GetLinearPosition(num4);
				double num6 = 0.0;
				if (this.useTwoValues)
				{
					double num7 = axis2.GetLogValue(this.GetYValue(common, area, series, dataPoint3D.dataPoint, dataPoint3D.index - 1, 0));
					if (num7 > axis2.GetViewMaximum())
					{
						num3 = 0.5f;
						num7 = axis2.GetViewMaximum();
					}
					else if (num7 < axis2.GetViewMinimum())
					{
						num3 = 0.5f;
						num7 = axis2.GetViewMinimum();
					}
					num6 = axis2.GetLinearPosition(num7);
				}
				else
				{
					num6 = axis2.GetPosition(axis2.Crossing);
				}
				double xPosition = dataPoint3D.xPosition;
				if (num5 < num6 && num6 - num5 < (double)relativeSize.Width)
				{
					num5 = num6 - (double)relativeSize.Width;
				}
				if (num5 > num6 && num5 - num6 < (double)relativeSize.Width)
				{
					num5 = num6 + (double)relativeSize.Width;
				}
				RectangleF empty = RectangleF.Empty;
				try
				{
					empty.Y = (float)(xPosition - dataPoint3D.width / 2.0);
					empty.Height = (float)dataPoint3D.width;
					if (num6 < num5)
					{
						empty.X = (float)num6;
						empty.Width = (float)num5 - empty.X;
					}
					else
					{
						float num8 = num2;
						num2 = num3;
						num3 = num8;
						empty.X = (float)num5;
						empty.Width = (float)num6 - empty.X;
					}
				}
				catch (Exception)
				{
					continue;
				}
				dataPoint.positionRel = new PointF(empty.Right, (float)xPosition);
				GraphicsPath path = null;
				if (!dataPoint.Empty)
				{
					num = (dataPoint3D.indexedSeries ? ((double)dataPoint3D.index) : dataPoint.XValue);
					num = axis.GetLogValue(num);
					if (!(num < axis.GetViewMinimum()) && !(num > axis.GetViewMaximum()))
					{
						bool flag2 = false;
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
								path = graph.Fill3DRectangle(empty, dataPoint3D.zPosition, dataPoint3D.depth, area.matrix3D, area.Area3DStyle.Light, dataPoint.Color, num2, num3, dataPoint.BackHatchStyle, dataPoint.BackImage, dataPoint.BackImageMode, dataPoint.BackImageTransparentColor, dataPoint.BackImageAlign, dataPoint.BackGradientType, dataPoint.BackGradientEndColor, dataPoint.BorderColor, dataPoint.BorderWidth, dataPoint.BorderStyle, PenAlignment.Inset, barDrawingStyle, false, drawingOperationTypes);
								graph.StopAnimation();
								graph.EndHotRegion();
								if (flag2)
								{
									graph.ResetClip();
								}
								goto IL_05e3;
							}
						}
					}
					continue;
				}
				goto IL_05e3;
				IL_05e3:
				graph.StartAnimation();
				this.DrawMarkers3D(area, axis2, graph, common, empty, dataPoint3D, series, num6, num5, dataPoint3D.width, dataPoint3D.index);
				graph.StopAnimation();
				if (dataPoint.ShowLabelAsValue || dataPoint.Label.Length > 0)
				{
					flag = true;
				}
				if (common.ProcessModeRegions)
				{
					common.HotRegionsList.AddHotRegion(path, false, graph, dataPoint, series.Name, dataPoint3D.index - 1);
				}
			}
			if (flag)
			{
				foreach (object item2 in dataPointDrawingOrder)
				{
					DataPoint3D dataPoint3D2 = (DataPoint3D)item2;
					DataPoint dataPoint2 = dataPoint3D2.dataPoint;
					Series series2 = dataPoint2.series;
					Axis axis3 = area.GetAxis(AxisName.X, series2.XAxisType, series2.XSubAxisName);
					Axis axis4 = area.GetAxis(AxisName.Y, series2.YAxisType, series2.YSubAxisName);
					double num9 = axis4.GetLogValue(this.GetYValue(common, area, series2, dataPoint3D2.dataPoint, dataPoint3D2.index - 1, this.useTwoValues ? 1 : 0));
					if (num9 > axis4.GetViewMaximum())
					{
						num9 = axis4.GetViewMaximum();
					}
					else if (num9 < axis4.GetViewMinimum())
					{
						num9 = axis4.GetViewMinimum();
					}
					double linearPosition = axis4.GetLinearPosition(num9);
					double num10 = 0.0;
					if (this.useTwoValues)
					{
						double num11 = axis4.GetLogValue(this.GetYValue(common, area, series2, dataPoint3D2.dataPoint, dataPoint3D2.index - 1, 0));
						if (num11 > axis4.GetViewMaximum())
						{
							num11 = axis4.GetViewMaximum();
						}
						else if (num11 < axis4.GetViewMinimum())
						{
							num11 = axis4.GetViewMinimum();
						}
						num10 = axis4.GetLinearPosition(num11);
					}
					else
					{
						num10 = axis4.GetPosition(axis4.Crossing);
					}
					double xPosition2 = dataPoint3D2.xPosition;
					RectangleF empty2 = RectangleF.Empty;
					try
					{
						empty2.Y = (float)(xPosition2 - dataPoint3D2.width / 2.0);
						empty2.Height = (float)dataPoint3D2.width;
						if (num10 < linearPosition)
						{
							empty2.X = (float)num10;
							empty2.Width = (float)linearPosition - empty2.X;
						}
						else
						{
							empty2.X = (float)linearPosition;
							empty2.Width = (float)num10 - empty2.X;
						}
					}
					catch (Exception)
					{
						continue;
					}
					if (!dataPoint2.Empty)
					{
						num = (dataPoint3D2.indexedSeries ? ((double)dataPoint3D2.index) : dataPoint2.XValue);
						num = axis3.GetLogValue(num);
						if (!(num < axis3.GetViewMinimum()) && !(num > axis3.GetViewMaximum()) && (decimal)empty2.Y >= (decimal)area.PlotAreaPosition.Y && (decimal)empty2.Bottom <= (decimal)area.PlotAreaPosition.Bottom())
						{
							graph.StartAnimation();
							this.DrawLabels3D(area, axis4, graph, common, empty2, dataPoint3D2, series2, num10, linearPosition, dataPoint3D2.width, dataPoint3D2.index - 1);
							graph.StopAnimation();
						}
					}
				}
			}
			return;
			IL_004a:
			arrayList = area.GetSeriesFromChartType(this.Name);
			foreach (string item3 in arrayList)
			{
				if (common.DataManager.Series[item3].IsAttributeSet("DrawSideBySide"))
				{
					string strA = ((DataPointAttributes)common.DataManager.Series[item3])["DrawSideBySide"];
					if (string.Compare(strA, "False", StringComparison.OrdinalIgnoreCase) == 0)
					{
						sideBySide = false;
					}
					else if (string.Compare(strA, "True", StringComparison.OrdinalIgnoreCase) == 0)
					{
						sideBySide = true;
					}
					else if (string.Compare(strA, "Auto", StringComparison.OrdinalIgnoreCase) != 0)
					{
						throw new InvalidOperationException(SR.ExceptionAttributeDrawSideBySideInvalid);
					}
				}
			}
			goto IL_0126;
		}

		private void DrawMarkers3D(ChartArea area, Axis hAxis, ChartGraphics graph, CommonElements common, RectangleF rectSize, DataPoint3D pointEx, Series ser, double barStartPosition, double barSize, double width, int pointIndex)
		{
			DataPoint dataPoint = pointEx.dataPoint;
			SizeF size = SizeF.Empty;
			if (dataPoint.MarkerStyle == MarkerStyle.None && dataPoint.MarkerImage.Length <= 0)
			{
				return;
			}
			if (pointEx.index % ser.MarkerStep == 0)
			{
				if (dataPoint.MarkerImage.Length == 0)
				{
					size.Width = (float)dataPoint.MarkerSize;
					size.Height = (float)dataPoint.MarkerSize;
				}
				else
				{
					common.ImageLoader.GetAdjustedImageSize(dataPoint.MarkerImage, graph.Graphics, ref size);
				}
				size = graph.GetRelativeSize(size);
				PointF empty = PointF.Empty;
				if (barStartPosition < barSize)
				{
					empty.X = rectSize.Right;
				}
				else
				{
					empty.X = rectSize.X;
				}
				empty.Y = (float)(rectSize.Y + rectSize.Height / 2.0);
				Point3D[] array = new Point3D[1]
				{
					new Point3D(empty.X, empty.Y, (float)(pointEx.zPosition + pointEx.depth / 2.0))
				};
				area.matrix3D.TransformPoints(array);
				PointF pointF = array[0].PointF;
				graph.DrawMarker3D(area.matrix3D, area.Area3DStyle.Light, (float)(pointEx.zPosition + pointEx.depth / 2.0), empty, dataPoint.MarkerStyle, dataPoint.MarkerSize, dataPoint.MarkerColor.IsEmpty ? dataPoint.series.Color : dataPoint.MarkerColor, dataPoint.MarkerBorderColor, dataPoint.MarkerBorderWidth, dataPoint.MarkerImage, dataPoint.MarkerImageTransparentColor, (dataPoint.series != null) ? dataPoint.series.ShadowOffset : 0, (dataPoint.series != null) ? dataPoint.series.ShadowColor : Color.Empty, RectangleF.Empty, DrawingOperationTypes.DrawElement);
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
				text = ValueConverter.FormatValue(ser.chart, dataPoint, this.GetYValue(common, area, ser, dataPoint, pointIndex, 0), dataPoint.LabelFormat, ser.YValueType, ChartElementType.DataPoint);
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
					common.ImageLoader.GetAdjustedImageSize(dataPoint.MarkerImage, graph.Graphics, ref size);
				}
				size = graph.GetRelativeSize(size);
			}
			BarValueLabelDrawingStyle barValueLabelDrawingStyle = this.defLabelDrawingStyle;
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
			bool flag2 = false;
			bool flag3 = false;
			float num = 0f;
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
				SizeF sizeF = graph.MeasureStringRel(text, dataPoint.Font);
				if (!flag2 && !flag3 && sizeF.Width > rectangleF.Width)
				{
					flag2 = true;
					num = rectangleF.Width;
					barValueLabelDrawingStyle = (BarValueLabelDrawingStyle)((barValueLabelDrawingStyle == BarValueLabelDrawingStyle.Outside) ? 3 : 0);
				}
				else if (flag2 && !flag3 && sizeF.Width > rectangleF.Width - 1.0 && num > rectangleF.Width)
				{
					barValueLabelDrawingStyle = (BarValueLabelDrawingStyle)((barValueLabelDrawingStyle == BarValueLabelDrawingStyle.Outside) ? 3 : 0);
				}
				else
				{
					flag = true;
				}
			}
			SizeF sizeF2 = graph.MeasureStringRel(text, dataPoint.Font, new SizeF(rectangleF.Width, rectangleF.Height), stringFormat);
			PointF position = PointF.Empty;
			if (stringFormat.Alignment == StringAlignment.Near)
			{
				position.X = (float)(rectangleF.X + sizeF2.Width / 2.0);
			}
			else if (stringFormat.Alignment == StringAlignment.Far)
			{
				position.X = (float)(rectangleF.Right - sizeF2.Width / 2.0);
			}
			else
			{
				position.X = (float)((rectangleF.Left + rectangleF.Right) / 2.0);
			}
			if (stringFormat.LineAlignment == StringAlignment.Near)
			{
				position.Y = (float)(rectangleF.Top + sizeF2.Height / 2.0);
			}
			else if (stringFormat.LineAlignment == StringAlignment.Far)
			{
				position.Y = (float)(rectangleF.Bottom - sizeF2.Height / 2.0);
			}
			else
			{
				position.Y = (float)((rectangleF.Bottom + rectangleF.Top) / 2.0);
			}
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			int num2 = dataPoint.FontAngle;
			Point3D[] array = new Point3D[2]
			{
				new Point3D(position.X, position.Y, pointEx.zPosition + pointEx.depth),
				new Point3D((float)(position.X - 20.0), position.Y, pointEx.zPosition + pointEx.depth)
			};
			area.matrix3D.TransformPoints(array);
			position = array[0].PointF;
			if (num2 == 0 || num2 == 180)
			{
				array[0].PointF = graph.GetAbsolutePoint(array[0].PointF);
				array[1].PointF = graph.GetAbsolutePoint(array[1].PointF);
				float num3 = (float)Math.Atan((double)((array[1].Y - array[0].Y) / (array[1].X - array[0].X)));
				num3 = (float)Math.Round(num3 * 180.0 / 3.1415927410125732);
				num2 += (int)num3;
			}
			RectangleF backPosition = RectangleF.Empty;
			if (common.ProcessModeRegions || !dataPoint.LabelBackColor.IsEmpty || !dataPoint.LabelBorderColor.IsEmpty)
			{
				SizeF sizeF3 = new SizeF(sizeF2.Width, sizeF2.Height);
				sizeF3.Height += (float)(sizeF3.Height / 8.0);
				sizeF3.Width += sizeF3.Width / (float)text.Length;
				backPosition = new RectangleF((float)(position.X - sizeF3.Width / 2.0), (float)(position.Y - sizeF3.Height / 2.0), sizeF3.Width, sizeF3.Height);
			}
			graph.DrawPointLabelStringRel(common, text, dataPoint.Font, new SolidBrush(dataPoint.FontColor), position, stringFormat, num2, backPosition, dataPoint.LabelBackColor, dataPoint.LabelBorderColor, dataPoint.LabelBorderWidth, dataPoint.LabelBorderStyle, ser, dataPoint, pointIndex);
		}

		public void AddSmartLabelMarkerPositions(CommonElements common, ChartArea area, Series series, ArrayList list)
		{
		}
	}
}
