using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class PointChart : IChartType
	{
		internal class Label3DInfo
		{
			internal DataPoint3D PointEx;

			internal PointF MarkerPosition = PointF.Empty;

			internal SizeF MarkerSize = SizeF.Empty;

			internal PointF AnimatedPoint = PointF.Empty;
		}

		internal bool alwaysDrawMarkers = true;

		internal int yValueIndex;

		internal int labelYValueIndex = -1;

		internal bool autoLabelPosition = true;

		internal LabelAlignmentTypes labelPosition = LabelAlignmentTypes.Top;

		internal Axis vAxis;

		internal Axis hAxis;

		internal bool indexedSeries;

		internal CommonElements common;

		internal ChartArea area;

		internal bool middleMarker = true;

		internal ArrayList label3DInfoList;

		public virtual string Name
		{
			get
			{
				return "Point";
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

		public virtual bool SideBySideSeries
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

		public virtual int YValuesPerPoint
		{
			get
			{
				return 1;
			}
		}

		public virtual double ShiftedX
		{
			get
			{
				return 0.0;
			}
			set
			{
			}
		}

		public virtual string ShiftedSerName
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public PointChart()
		{
		}

		public PointChart(bool alwaysDrawMarkers)
		{
			this.alwaysDrawMarkers = alwaysDrawMarkers;
		}

		public virtual LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Marker;
		}

		public virtual Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}

		public virtual void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			this.common = common;
			this.area = area;
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
				if (this.ShiftedSerName.Length == 0)
				{
					this.indexedSeries = area.IndexedSeries((string[])area.GetSeriesFromChartType(this.Name).ToArray(typeof(string)));
				}
				else
				{
					this.indexedSeries = ChartElement.IndexedSeries(common.DataManager.Series[this.ShiftedSerName]);
				}
				foreach (Series item in common.DataManager.Series)
				{
					bool flag = false;
					if (this.ShiftedSerName.Length > 0)
					{
						if (this.ShiftedSerName != item.Name)
						{
							continue;
						}
						flag = true;
					}
					if (string.Compare(item.ChartTypeName, this.Name, true, CultureInfo.CurrentCulture) == 0 && !(item.ChartArea != area.Name) && item.IsVisible() && (seriesToDraw == null || !(seriesToDraw.Name != item.Name)))
					{
						this.hAxis = area.GetAxis(AxisName.X, item.XAxisType, item.XSubAxisName);
						this.vAxis = area.GetAxis(AxisName.Y, item.YAxisType, item.YSubAxisName);
						double viewMaximum = this.hAxis.GetViewMaximum();
						double viewMinimum = this.hAxis.GetViewMinimum();
						double viewMaximum2 = this.vAxis.GetViewMaximum();
						double viewMinimum2 = this.vAxis.GetViewMinimum();
						if (!selection)
						{
							common.EventsManager.OnBackPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
						}
						int num = 0;
						int num2 = 1;
						foreach (DataPoint point in item.Points)
						{
							point.positionRel = new PointF(float.NaN, float.NaN);
							double yValue = this.indexedSeries ? ((double)num2) : point.XValue;
							yValue = this.hAxis.GetLogValue(yValue);
							if (yValue > viewMaximum || yValue < viewMinimum)
							{
								num2++;
							}
							else
							{
								double yValue2 = this.GetYValue(common, area, item, point, num2 - 1, this.yValueIndex);
								yValue2 = this.vAxis.GetLogValue(yValue2);
								if (yValue2 > viewMaximum2 || yValue2 < viewMinimum2)
								{
									num2++;
								}
								else
								{
									bool flag2 = false;
									if (!this.ShouldDrawMarkerOnViewEdgeX())
									{
										if (yValue == viewMaximum && this.ShiftedX >= 0.0)
										{
											flag2 = true;
										}
										if (yValue == viewMinimum && this.ShiftedX <= 0.0)
										{
											flag2 = true;
										}
									}
									int markerSize = point.MarkerSize;
									string markerImage = point.MarkerImage;
									MarkerStyle markerStyle = point.MarkerStyle;
									PointF empty = PointF.Empty;
									empty.Y = (float)this.vAxis.GetLinearPosition(yValue2);
									if (this.indexedSeries)
									{
										empty.X = (float)this.hAxis.GetPosition((double)num2);
									}
									else
									{
										empty.X = (float)this.hAxis.GetPosition(point.XValue);
									}
									empty.X += (float)this.ShiftedX;
									point.positionRel = new PointF(empty.X, empty.Y);
									SizeF markerSize2 = this.GetMarkerSize(graph, common, area, point, markerSize, markerImage);
									if (flag2)
									{
										num2++;
									}
									else
									{
										if (this.alwaysDrawMarkers || markerStyle != 0 || markerImage.Length > 0)
										{
											if (common.ProcessModePaint)
											{
												if (num == 0)
												{
													graph.StartHotRegion(point);
													graph.StartAnimation();
													this.DrawPointMarker(graph, point.series, point, empty, (markerStyle == MarkerStyle.None) ? MarkerStyle.Circle : markerStyle, (int)markerSize2.Height, (point.MarkerColor == Color.Empty) ? point.Color : point.MarkerColor, (point.MarkerBorderColor == Color.Empty) ? point.BorderColor : point.MarkerBorderColor, this.GetMarkerBorderSize(point), markerImage, point.MarkerImageTransparentColor, (point.series != null) ? point.series.ShadowOffset : 0, (point.series != null) ? point.series.ShadowColor : Color.Empty, new RectangleF(empty.X, empty.Y, markerSize2.Width, markerSize2.Height));
													graph.StopAnimation();
													graph.EndHotRegion();
												}
												if (common.ProcessModeRegions)
												{
													this.SetHotRegions(common, graph, point, markerSize2, point.series.Name, num2 - 1, markerStyle, empty);
												}
											}
											num++;
											if (item.MarkerStep == num)
											{
												num = 0;
											}
										}
										graph.StartHotRegion(point, true);
										graph.StartAnimation();
										this.DrawLabels(area, graph, common, empty, (int)markerSize2.Height, point, item, num2 - 1);
										graph.StopAnimation();
										graph.EndHotRegion();
										num2++;
									}
								}
							}
						}
						if (!selection)
						{
							common.EventsManager.OnPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
						}
						if (flag)
						{
							break;
						}
					}
				}
			}
		}

		protected virtual void DrawPointMarker(ChartGraphics graph, Series series, DataPoint dataPoint, PointF point, MarkerStyle markerStyle, int markerSize, Color markerColor, Color markerBorderColor, int markerBorderSize, string markerImage, Color markerImageTransparentColor, int shadowSize, Color shadowColor, RectangleF imageScaleRect)
		{
			graph.DrawMarkerRel(point, markerStyle, markerSize, markerColor, markerBorderColor, markerBorderSize, markerImage, markerImageTransparentColor, shadowSize, shadowColor, imageScaleRect);
		}

		private void SetHotRegions(CommonElements common, ChartGraphics graph, DataPoint point, SizeF markerSize, string seriesName, int pointIndex, MarkerStyle pointMarkerStyle, PointF markerPosition)
		{
			SizeF relativeSize = graph.GetRelativeSize(markerSize);
			int insertIndex = common.HotRegionsList.FindInsertIndex();
			if (pointMarkerStyle == MarkerStyle.Circle)
			{
				common.HotRegionsList.AddHotRegion(insertIndex, graph, markerPosition.X, markerPosition.Y, (float)(relativeSize.Width / 2.0), point, seriesName, pointIndex);
			}
			else
			{
				common.HotRegionsList.AddHotRegion(graph, new RectangleF((float)(markerPosition.X - relativeSize.Width / 2.0), (float)(markerPosition.Y - relativeSize.Height / 2.0), relativeSize.Width, relativeSize.Height), point, seriesName, pointIndex);
			}
		}

		private void DrawLabels(ChartArea area, ChartGraphics graph, CommonElements common, PointF markerPosition, int markerSize, DataPoint point, Series ser, int pointIndex)
		{
			string label = point.Label;
			bool showLabelAsValue = point.ShowLabelAsValue;
			if ((point.Empty || (!ser.ShowLabelAsValue && !showLabelAsValue && label.Length <= 0)) && !showLabelAsValue && label.Length <= 0)
			{
				return;
			}
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Near;
			stringFormat.LineAlignment = StringAlignment.Center;
			string text;
			if (label.Length == 0)
			{
				text = ValueConverter.FormatValue(ser.chart, point, point.YValues[(this.labelYValueIndex == -1) ? this.yValueIndex : this.labelYValueIndex], point.LabelFormat, ser.YValueType, ChartElementType.DataPoint);
			}
			else
			{
				text = point.ReplaceKeywords(label);
				if (ser.chart != null && ser.chart.LocalizeTextHandler != null)
				{
					text = ser.chart.LocalizeTextHandler(point, text, point.ElementId, ChartElementType.DataPoint);
				}
			}
			SizeF relativeSize = graph.GetRelativeSize(new SizeF((float)markerSize, (float)markerSize));
			SizeF relativeSize2 = graph.GetRelativeSize(graph.MeasureString(text, point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
			SizeF relativeSize3 = graph.GetRelativeSize(graph.MeasureString("W", point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
			SizeF size = new SizeF(relativeSize2.Width, relativeSize2.Height);
			float num = size.Width / (float)text.Length;
			size.Height += (float)(relativeSize3.Height / 2.0);
			size.Width += num;
			string text2 = ((DataPointAttributes)point)["LabelStyle"];
			if (text2 == null || text2.Length == 0)
			{
				text2 = ((DataPointAttributes)ser)["LabelStyle"];
			}
			this.autoLabelPosition = true;
			if (text2 != null && text2.Length > 0)
			{
				this.autoLabelPosition = false;
				if (string.Compare(text2, "Auto", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.autoLabelPosition = true;
					goto IL_0302;
				}
				if (string.Compare(text2, "Center", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.labelPosition = LabelAlignmentTypes.Center;
					goto IL_0302;
				}
				if (string.Compare(text2, "Bottom", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.labelPosition = LabelAlignmentTypes.Bottom;
					goto IL_0302;
				}
				if (string.Compare(text2, "TopLeft", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.labelPosition = LabelAlignmentTypes.TopLeft;
					goto IL_0302;
				}
				if (string.Compare(text2, "TopRight", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.labelPosition = LabelAlignmentTypes.TopRight;
					goto IL_0302;
				}
				if (string.Compare(text2, "BottomLeft", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.labelPosition = LabelAlignmentTypes.BottomLeft;
					goto IL_0302;
				}
				if (string.Compare(text2, "BottomRight", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.labelPosition = LabelAlignmentTypes.BottomRight;
					goto IL_0302;
				}
				if (string.Compare(text2, "Left", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.labelPosition = LabelAlignmentTypes.Left;
					goto IL_0302;
				}
				if (string.Compare(text2, "Right", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.labelPosition = LabelAlignmentTypes.Right;
					goto IL_0302;
				}
				if (string.Compare(text2, "Top", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.labelPosition = LabelAlignmentTypes.Top;
					goto IL_0302;
				}
				throw new ArgumentException(SR.ExceptionCustomAttributeValueInvalid(text2, "LabelStyle"));
			}
			goto IL_0302;
			IL_0302:
			if (this.autoLabelPosition)
			{
				this.labelPosition = this.GetAutoLabelPosition(ser, pointIndex);
			}
			PointF position = new PointF(markerPosition.X, markerPosition.Y);
			switch (this.labelPosition)
			{
			case LabelAlignmentTypes.Center:
				stringFormat.Alignment = StringAlignment.Center;
				break;
			case LabelAlignmentTypes.Bottom:
				stringFormat.Alignment = StringAlignment.Center;
				position.Y += (float)(relativeSize.Height / 1.75);
				position.Y += (float)(size.Height / 2.0);
				break;
			case LabelAlignmentTypes.Top:
				stringFormat.Alignment = StringAlignment.Center;
				position.Y -= (float)(relativeSize.Height / 1.75);
				position.Y -= (float)(size.Height / 2.0);
				break;
			case LabelAlignmentTypes.Left:
				stringFormat.Alignment = StringAlignment.Far;
				position.X -= (float)(relativeSize.Height / 1.75 + num / 2.0);
				break;
			case LabelAlignmentTypes.TopLeft:
				stringFormat.Alignment = StringAlignment.Far;
				position.X -= (float)(relativeSize.Height / 1.75 + num / 2.0);
				position.Y -= (float)(relativeSize.Height / 1.75);
				position.Y -= (float)(size.Height / 2.0);
				break;
			case LabelAlignmentTypes.BottomLeft:
				stringFormat.Alignment = StringAlignment.Far;
				position.X -= (float)(relativeSize.Height / 1.75 + num / 2.0);
				position.Y += (float)(relativeSize.Height / 1.75);
				position.Y += (float)(size.Height / 2.0);
				break;
			case LabelAlignmentTypes.Right:
				position.X += (float)(relativeSize.Height / 1.75 + num / 2.0);
				break;
			case LabelAlignmentTypes.TopRight:
				position.X += (float)(relativeSize.Height / 1.75 + num / 2.0);
				position.Y -= (float)(relativeSize.Height / 1.75);
				position.Y -= (float)(size.Height / 2.0);
				break;
			case LabelAlignmentTypes.BottomRight:
				position.X += (float)(relativeSize.Height / 1.75 + num / 2.0);
				position.Y += (float)(relativeSize.Height / 1.75);
				position.Y += (float)(size.Height / 2.0);
				break;
			}
			int num2 = point.FontAngle;
			if (text.Trim().Length != 0)
			{
				if (ser.SmartLabels.Enabled)
				{
					position = area.smartLabels.AdjustSmartLabelPosition(common, graph, area, ser.SmartLabels, position, relativeSize2, ref stringFormat, markerPosition, relativeSize, this.labelPosition);
					num2 = 0;
				}
				if (num2 == 90 || num2 == -90)
				{
					switch (this.labelPosition)
					{
					case LabelAlignmentTypes.Top:
						stringFormat.Alignment = StringAlignment.Near;
						position.Y += (float)(size.Height / 2.0);
						break;
					case LabelAlignmentTypes.Bottom:
						stringFormat.Alignment = StringAlignment.Far;
						position.Y -= (float)(size.Height / 2.0);
						break;
					case LabelAlignmentTypes.Right:
						stringFormat.Alignment = StringAlignment.Center;
						stringFormat.LineAlignment = StringAlignment.Near;
						break;
					case LabelAlignmentTypes.Left:
						stringFormat.Alignment = StringAlignment.Center;
						stringFormat.LineAlignment = StringAlignment.Center;
						break;
					case LabelAlignmentTypes.TopLeft:
						stringFormat.Alignment = StringAlignment.Near;
						break;
					case LabelAlignmentTypes.BottomRight:
						stringFormat.Alignment = StringAlignment.Far;
						break;
					}
				}
				if (!position.IsEmpty)
				{
					RectangleF empty = RectangleF.Empty;
					size.Height -= (float)(relativeSize2.Height / 2.0);
					size.Height += (float)(relativeSize2.Height / 8.0);
					empty = PointChart.GetLabelPosition(graph, position, size, stringFormat, true);
					switch (this.labelPosition)
					{
					case LabelAlignmentTypes.Left:
						empty.X += (float)(num / 2.0);
						break;
					case LabelAlignmentTypes.TopLeft:
						empty.X += (float)(num / 2.0);
						break;
					case LabelAlignmentTypes.BottomLeft:
						empty.X += (float)(num / 2.0);
						break;
					case LabelAlignmentTypes.Right:
						empty.X -= (float)(num / 2.0);
						break;
					case LabelAlignmentTypes.TopRight:
						empty.X -= (float)(num / 2.0);
						break;
					case LabelAlignmentTypes.BottomRight:
						empty.X -= (float)(num / 2.0);
						break;
					}
					graph.DrawPointLabelStringRel(common, text, point.Font, new SolidBrush(point.FontColor), position, stringFormat, num2, empty, point.LabelBackColor, point.LabelBorderColor, point.LabelBorderWidth, point.LabelBorderStyle, ser, point, pointIndex);
				}
			}
		}

		internal static RectangleF GetLabelPosition(ChartGraphics graph, PointF position, SizeF size, StringFormat format, bool adjustForDrawing)
		{
			RectangleF empty = RectangleF.Empty;
			empty.Width = size.Width;
			empty.Height = size.Height;
			SizeF sizeF = SizeF.Empty;
			if (graph != null)
			{
				sizeF = graph.GetRelativeSize(new SizeF(1f, 1f));
			}
			if (format.Alignment == StringAlignment.Far)
			{
				empty.X = position.X - size.Width;
				if (adjustForDrawing && !sizeF.IsEmpty)
				{
					empty.X -= (float)(4.0 * sizeF.Width);
					empty.Width += (float)(4.0 * sizeF.Width);
				}
			}
			else if (format.Alignment == StringAlignment.Near)
			{
				empty.X = position.X;
				if (adjustForDrawing && !sizeF.IsEmpty)
				{
					empty.Width += (float)(4.0 * sizeF.Width);
				}
			}
			else if (format.Alignment == StringAlignment.Center)
			{
				empty.X = (float)(position.X - size.Width / 2.0);
				if (adjustForDrawing && !sizeF.IsEmpty)
				{
					empty.X -= (float)(2.0 * sizeF.Width);
					empty.Width += (float)(4.0 * sizeF.Width);
				}
			}
			if (format.LineAlignment == StringAlignment.Far)
			{
				empty.Y = position.Y - size.Height;
			}
			else if (format.LineAlignment == StringAlignment.Near)
			{
				empty.Y = position.Y;
			}
			else if (format.LineAlignment == StringAlignment.Center)
			{
				empty.Y = (float)(position.Y - size.Height / 2.0);
			}
			empty.Y -= (float)(1.0 * sizeF.Height);
			return empty;
		}

		protected virtual void ProcessChartType3D(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			ArrayList arrayList = null;
			if (area.Area3DStyle.Clustered && this.SideBySideSeries)
			{
				goto IL_0020;
			}
			if (this.Stacked)
			{
				goto IL_0020;
			}
			arrayList = new ArrayList();
			arrayList.Add(seriesToDraw.Name);
			goto IL_0044;
			IL_0044:
			ArrayList dataPointDrawingOrder = area.GetDataPointDrawingOrder(arrayList, this, selection, COPCoordinates.X, null, this.yValueIndex, false);
			foreach (object item in dataPointDrawingOrder)
			{
				this.ProcessSinglePoint3D((DataPoint3D)item, selection, graph, common, area);
			}
			this.DrawAccumulated3DLabels(graph, common, area);
			return;
			IL_0020:
			arrayList = area.GetSeriesFromChartType(this.Name);
			goto IL_0044;
		}

		internal void ProcessSinglePoint3D(DataPoint3D pointEx, bool selection, ChartGraphics graph, CommonElements common, ChartArea area)
		{
			DataPoint dataPoint = pointEx.dataPoint;
			Series series = dataPoint.series;
			dataPoint.positionRel = new PointF(float.NaN, float.NaN);
			this.hAxis = area.GetAxis(AxisName.X, series.XAxisType, series.XSubAxisName);
			this.vAxis = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName);
			double yValue = this.GetYValue(common, area, series, pointEx.dataPoint, pointEx.index - 1, this.yValueIndex);
			yValue = this.vAxis.GetLogValue(yValue);
			if (!(yValue > this.vAxis.GetViewMaximum()) && !(yValue < this.vAxis.GetViewMinimum()))
			{
				double yValue2 = pointEx.indexedSeries ? ((double)pointEx.index) : dataPoint.XValue;
				yValue2 = this.hAxis.GetLogValue(yValue2);
				if (!(yValue2 > this.hAxis.GetViewMaximum()) && !(yValue2 < this.hAxis.GetViewMinimum()))
				{
					if (!this.ShouldDrawMarkerOnViewEdgeX())
					{
						if (yValue2 == this.hAxis.GetViewMaximum() && this.ShiftedX >= 0.0)
						{
							return;
						}
						if (yValue2 == this.hAxis.GetViewMinimum() && this.ShiftedX <= 0.0)
						{
							return;
						}
					}
					PointF empty = PointF.Empty;
					empty.Y = (float)pointEx.yPosition;
					empty.X = (float)this.hAxis.GetLinearPosition(yValue2);
					empty.X += (float)this.ShiftedX;
					dataPoint.positionRel = new PointF(empty.X, empty.Y);
					int markerSize = dataPoint.MarkerSize;
					string markerImage = dataPoint.MarkerImage;
					MarkerStyle markerStyle = dataPoint.MarkerStyle;
					SizeF markerSize2 = this.GetMarkerSize(graph, common, area, dataPoint, markerSize, markerImage);
					Point3D[] array = new Point3D[1]
					{
						new Point3D(empty.X, empty.Y, (float)(pointEx.zPosition + (this.middleMarker ? (pointEx.depth / 2.0) : pointEx.depth)))
					};
					area.matrix3D.TransformPoints(array);
					PointF pointF = array[0].PointF;
					GraphicsPath path = null;
					if ((this.alwaysDrawMarkers || markerStyle != 0 || markerImage.Length > 0) && pointEx.index % series.MarkerStep == 0)
					{
						DrawingOperationTypes drawingOperationTypes = DrawingOperationTypes.DrawElement;
						if (common.ProcessModeRegions)
						{
							drawingOperationTypes |= DrawingOperationTypes.CalcElementPath;
						}
						graph.StartHotRegion(dataPoint);
						graph.StartAnimation();
						path = graph.DrawMarker3D(area.matrix3D, area.Area3DStyle.Light, (float)(pointEx.zPosition + (this.middleMarker ? (pointEx.depth / 2.0) : pointEx.depth)), empty, (markerStyle == MarkerStyle.None) ? MarkerStyle.Circle : markerStyle, (int)markerSize2.Height, (dataPoint.MarkerColor == Color.Empty) ? dataPoint.Color : dataPoint.MarkerColor, (dataPoint.MarkerBorderColor == Color.Empty) ? dataPoint.BorderColor : dataPoint.MarkerBorderColor, this.GetMarkerBorderSize(dataPoint), markerImage, dataPoint.MarkerImageTransparentColor, (dataPoint.series != null) ? dataPoint.series.ShadowOffset : 0, (dataPoint.series != null) ? dataPoint.series.ShadowColor : Color.Empty, new RectangleF(pointF.X, pointF.Y, markerSize2.Width, markerSize2.Height), drawingOperationTypes);
						graph.StopAnimation();
						graph.EndHotRegion();
					}
					PointF empty2 = PointF.Empty;
					if (this.label3DInfoList != null && this.label3DInfoList.Count > 0 && ((Label3DInfo)this.label3DInfoList[this.label3DInfoList.Count - 1]).PointEx.zPosition != pointEx.zPosition)
					{
						this.DrawAccumulated3DLabels(graph, common, area);
					}
					if (this.label3DInfoList == null)
					{
						this.label3DInfoList = new ArrayList();
					}
					Label3DInfo label3DInfo = new Label3DInfo();
					label3DInfo.PointEx = pointEx;
					label3DInfo.MarkerPosition = pointF;
					label3DInfo.MarkerSize = markerSize2;
					label3DInfo.AnimatedPoint = empty2;
					this.label3DInfoList.Add(label3DInfo);
					if (common.ProcessModeRegions)
					{
						SizeF relativeSize = graph.GetRelativeSize(markerSize2);
						int insertIndex = common.HotRegionsList.FindInsertIndex();
						if (markerStyle == MarkerStyle.Circle)
						{
							float[] array2 = new float[3]
							{
								pointF.X,
								pointF.Y,
								(float)(relativeSize.Width / 2.0)
							};
							common.HotRegionsList.AddHotRegion(insertIndex, graph, array2[0], array2[1], array2[2], dataPoint, series.Name, pointEx.index - 1);
						}
						if (markerStyle == MarkerStyle.Square)
						{
							common.HotRegionsList.AddHotRegion(path, false, graph, dataPoint, series.Name, pointEx.index - 1);
						}
						else
						{
							common.HotRegionsList.AddHotRegion(graph, new RectangleF((float)(pointF.X - relativeSize.Width / 2.0), (float)(pointF.Y - relativeSize.Height / 2.0), relativeSize.Width, relativeSize.Height), dataPoint, series.Name, pointEx.index - 1);
						}
					}
				}
			}
		}

		internal void DrawAccumulated3DLabels(ChartGraphics graph, CommonElements common, ChartArea area)
		{
			if (this.label3DInfoList != null)
			{
				foreach (Label3DInfo label3DInfo in this.label3DInfoList)
				{
					graph.StartAnimation();
					this.DrawLabels(area, graph, common, label3DInfo.MarkerPosition, (int)label3DInfo.MarkerSize.Height, label3DInfo.PointEx.dataPoint, label3DInfo.PointEx.dataPoint.series, label3DInfo.PointEx.index - 1);
					graph.StopAnimation();
				}
				this.label3DInfoList.Clear();
			}
		}

		protected virtual bool ShouldDrawMarkerOnViewEdgeX()
		{
			return true;
		}

		protected virtual int GetMarkerBorderSize(DataPointAttributes point)
		{
			return point.MarkerBorderWidth;
		}

		protected virtual LabelAlignmentTypes GetAutoLabelPosition(Series series, int pointIndex)
		{
			return LabelAlignmentTypes.Top;
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
			double num = this.GetEmptyPointValue(point, pointIndex);
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

		internal double GetEmptyPointValue(DataPoint point, int pointIndex)
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
				num = series.Points[num3].YValues[this.yValueIndex];
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
				num2 = series.Points[num4].YValues[this.yValueIndex];
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

		public void AddSmartLabelMarkerPositions(CommonElements common, ChartArea area, Series series, ArrayList list)
		{
			this.indexedSeries = area.IndexedSeries((string[])area.GetSeriesFromChartType(this.Name).ToArray(typeof(string)));
			Axis axis = area.GetAxis(AxisName.X, series.XAxisType, series.XSubAxisName);
			Axis axis2 = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName);
			int num = 0;
			int num2 = 1;
			foreach (DataPoint point in series.Points)
			{
				double yValue = this.GetYValue(common, area, series, point, num2 - 1, this.yValueIndex);
				yValue = axis2.GetLogValue(yValue);
				if (yValue > axis2.GetViewMaximum() || yValue < axis2.GetViewMinimum())
				{
					num2++;
				}
				else
				{
					double yValue2 = this.indexedSeries ? ((double)num2) : point.XValue;
					yValue2 = axis.GetLogValue(yValue2);
					if (yValue2 > axis.GetViewMaximum() || yValue2 < axis.GetViewMinimum())
					{
						num2++;
					}
					else
					{
						if (!this.ShouldDrawMarkerOnViewEdgeX())
						{
							if (yValue2 == axis.GetViewMaximum() && this.ShiftedX >= 0.0)
							{
								num2++;
								continue;
							}
							if (yValue2 == axis.GetViewMinimum() && this.ShiftedX <= 0.0)
							{
								num2++;
								continue;
							}
						}
						PointF pointF = PointF.Empty;
						pointF.Y = (float)axis2.GetLinearPosition(yValue);
						if (this.indexedSeries)
						{
							pointF.X = (float)axis.GetPosition((double)num2);
						}
						else
						{
							pointF.X = (float)axis.GetPosition(point.XValue);
						}
						pointF.X += (float)this.ShiftedX;
						int markerSize = point.MarkerSize;
						string markerImage = point.MarkerImage;
						MarkerStyle markerStyle = point.MarkerStyle;
						SizeF size = this.GetMarkerSize(common.graph, common, area, point, markerSize, markerImage);
						if (area.Area3DStyle.Enable3D)
						{
							float num3 = default(float);
							float num4 = default(float);
							((ChartArea3D)area).GetSeriesZPositionAndDepth(series, out num3, out num4);
							Point3D[] array = new Point3D[1]
							{
								new Point3D(pointF.X, pointF.Y, (float)(num4 + (this.middleMarker ? (num3 / 2.0) : num3)))
							};
							area.matrix3D.TransformPoints(array);
							pointF = array[0].PointF;
						}
						if (this.alwaysDrawMarkers || markerStyle != 0 || markerImage.Length > 0)
						{
							if (num == 0)
							{
								size = common.graph.GetRelativeSize(size);
								RectangleF rectangleF = new RectangleF((float)(pointF.X - size.Width / 2.0), (float)(pointF.Y - size.Height / 2.0), size.Width, size.Height);
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
