using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class RadarChart : IChartType, ICircularChartType
	{
		protected CommonElements common;

		protected ChartArea area;

		protected bool autoLabelPosition = true;

		protected LabelAlignmentTypes labelPosition = LabelAlignmentTypes.Top;

		public virtual string Name
		{
			get
			{
				return "Radar";
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
				return true;
			}
		}

		public virtual bool SupportLogarithmicAxes
		{
			get
			{
				return false;
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

		public virtual LegendImageStyle GetLegendImageStyle(Series series)
		{
			if (series != null)
			{
				switch (this.GetDrawingStyle(series, new DataPoint(series)))
				{
				case RadarDrawingStyle.Line:
					return LegendImageStyle.Line;
				case RadarDrawingStyle.Marker:
					return LegendImageStyle.Marker;
				}
			}
			return LegendImageStyle.Rectangle;
		}

		public virtual Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}

		public virtual bool RequireClosedFigure()
		{
			return true;
		}

		public virtual bool XAxisCrossingSupported()
		{
			return false;
		}

		public virtual bool XAxisLabelsSupported()
		{
			return false;
		}

		public virtual bool RadialGridLinesSupported()
		{
			return false;
		}

		public virtual int GetNumerOfSectors(ChartArea area, SeriesCollection seriesCollection)
		{
			int num = 0;
			foreach (Series item in seriesCollection)
			{
				if (item.IsVisible() && item.ChartArea == area.Name)
				{
					num = Math.Max(item.Points.Count, num);
				}
			}
			return num;
		}

		public virtual float[] GetYAxisLocations(ChartArea area)
		{
			float[] array = new float[area.CircularSectorsNumber];
			float num = (float)(360.0 / (float)array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = num * (float)i;
			}
			return array;
		}

		public virtual void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			this.common = common;
			this.area = area;
			this.ProcessChartType(false, graph, common, area, seriesToDraw);
		}

		protected virtual void ProcessChartType(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			foreach (Series item in common.DataManager.Series)
			{
				if (!(item.ChartArea != area.Name) && item.IsVisible())
				{
					if (string.Compare(item.ChartTypeName, this.Name, true, CultureInfo.CurrentCulture) != 0)
					{
						throw new InvalidOperationException(SR.ExceptionChartTypeCanNotCombine(item.ChartTypeName, this.Name));
					}
					if (!selection)
					{
						common.EventsManager.OnBackPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
					}
					Axis axis = area.GetAxis(AxisName.Y, AxisType.Primary, item.YSubAxisName);
					double viewMinimum = axis.GetViewMinimum();
					double viewMaximum = axis.GetViewMaximum();
					PointF[] pointsPosition = this.GetPointsPosition(graph, area, item);
					int num = 0;
					if (item.ShadowOffset != 0 && !selection)
					{
						graph.shadowDrawingMode = true;
						foreach (DataPoint point in item.Points)
						{
							int num2 = num + 1;
							if (num2 >= item.Points.Count)
							{
								num2 = 0;
							}
							DataPointAttributes dataPointAttributes = point;
							if (item.Points[num2].Empty)
							{
								dataPointAttributes = item.Points[num2];
							}
							Color left = dataPointAttributes.Color;
							Color borderColor = dataPointAttributes.BorderColor;
							int borderWidth = dataPointAttributes.BorderWidth;
							ChartDashStyle borderStyle = dataPointAttributes.BorderStyle;
							RadarDrawingStyle drawingStyle = this.GetDrawingStyle(item, point);
							if (!(axis.GetLogValue(point.YValues[0]) > viewMaximum) && !(axis.GetLogValue(point.YValues[0]) < viewMinimum) && !(axis.GetLogValue(item.Points[num2].YValues[0]) > viewMaximum) && !(axis.GetLogValue(item.Points[num2].YValues[0]) < viewMinimum))
							{
								switch (drawingStyle)
								{
								case RadarDrawingStyle.Line:
								{
									Color color3 = dataPointAttributes.Color;
									borderWidth = ((borderWidth < 1) ? 1 : borderWidth);
									borderStyle = ((borderStyle == ChartDashStyle.NotSet) ? ChartDashStyle.Solid : borderStyle);
									left = Color.Transparent;
									break;
								}
								case RadarDrawingStyle.Marker:
									left = Color.Transparent;
									break;
								}
								if (num2 == 0 && !this.RequireClosedFigure() && drawingStyle != 0)
								{
									break;
								}
								if (left != Color.Transparent && left != Color.Empty && item.ShadowOffset != 0)
								{
									GraphicsPath graphicsPath = new GraphicsPath();
									graphicsPath.AddLine(graph.GetAbsolutePoint(area.circularCenter), pointsPosition[num]);
									graphicsPath.AddLine(pointsPosition[num], pointsPosition[num2]);
									graphicsPath.AddLine(pointsPosition[num2], graph.GetAbsolutePoint(area.circularCenter));
									Matrix matrix = new Matrix();
									matrix.Translate((float)item.ShadowOffset, (float)item.ShadowOffset);
									graphicsPath.Transform(matrix);
									graph.FillPath(new SolidBrush(item.ShadowColor), graphicsPath);
								}
								num++;
							}
							else
							{
								num++;
							}
						}
						graph.shadowDrawingMode = false;
					}
					num = 0;
					foreach (DataPoint point2 in item.Points)
					{
						point2.positionRel = graph.GetRelativePoint(pointsPosition[num]);
						int num3 = num + 1;
						if (num3 >= item.Points.Count)
						{
							num3 = 0;
						}
						DataPointAttributes dataPointAttributes2 = point2;
						if (item.Points[num3].Empty)
						{
							dataPointAttributes2 = item.Points[num3];
						}
						Color color = dataPointAttributes2.Color;
						Color color2 = dataPointAttributes2.BorderColor;
						int num4 = dataPointAttributes2.BorderWidth;
						ChartDashStyle chartDashStyle = dataPointAttributes2.BorderStyle;
						RadarDrawingStyle drawingStyle2 = this.GetDrawingStyle(item, point2);
						if (!(axis.GetLogValue(point2.YValues[0]) > viewMaximum) && !(axis.GetLogValue(point2.YValues[0]) < viewMinimum) && !(axis.GetLogValue(item.Points[num3].YValues[0]) > viewMaximum) && !(axis.GetLogValue(item.Points[num3].YValues[0]) < viewMinimum))
						{
							switch (drawingStyle2)
							{
							case RadarDrawingStyle.Line:
								color2 = dataPointAttributes2.Color;
								num4 = ((num4 < 1) ? 1 : num4);
								chartDashStyle = ((chartDashStyle == ChartDashStyle.NotSet) ? ChartDashStyle.Solid : chartDashStyle);
								color = Color.Transparent;
								break;
							case RadarDrawingStyle.Marker:
								color = Color.Transparent;
								break;
							}
							GraphicsPath graphicsPath2 = new GraphicsPath();
							if (num3 == 0 && !this.RequireClosedFigure() && drawingStyle2 != 0)
							{
								if (common.ProcessModeRegions)
								{
									this.AddSelectionPath(area, graphicsPath2, pointsPosition, num, num3, graph.GetAbsolutePoint(area.circularCenter), 0);
									int insertIndex = common.HotRegionsList.FindInsertIndex();
									common.HotRegionsList.AddHotRegion(insertIndex, graphicsPath2, false, graph, point2, item.Name, num);
								}
								break;
							}
							if (color != Color.Transparent && color != Color.Empty)
							{
								GraphicsPath graphicsPath3 = new GraphicsPath();
								graphicsPath3.AddLine(graph.GetAbsolutePoint(area.circularCenter), pointsPosition[num]);
								graphicsPath3.AddLine(pointsPosition[num], pointsPosition[num3]);
								graphicsPath3.AddLine(pointsPosition[num3], graph.GetAbsolutePoint(area.circularCenter));
								if (common.ProcessModePaint)
								{
									Brush brush = graph.CreateBrush(graphicsPath3.GetBounds(), color, dataPointAttributes2.BackHatchStyle, dataPointAttributes2.BackImage, dataPointAttributes2.BackImageMode, dataPointAttributes2.BackImageTransparentColor, dataPointAttributes2.BackImageAlign, dataPointAttributes2.BackGradientType, dataPointAttributes2.BackGradientEndColor);
									graph.StartHotRegion(point2);
									graph.StartAnimation();
									graph.FillPath(brush, graphicsPath3);
									graph.StopAnimation();
									graph.EndHotRegion();
								}
								if (common.ProcessModeRegions)
								{
									this.AddSelectionPath(area, graphicsPath2, pointsPosition, num, num3, graph.GetAbsolutePoint(area.circularCenter), 0);
								}
							}
							if (color2 != Color.Empty && num4 > 0 && chartDashStyle != 0 && num3 < item.Points.Count)
							{
								if (common.ProcessModePaint)
								{
									graph.StartHotRegion(point2);
									graph.StartAnimation();
									graph.DrawLineAbs(color2, num4, chartDashStyle, pointsPosition[num], pointsPosition[num3], item.ShadowColor, (color == Color.Transparent || color == Color.Empty) ? item.ShadowOffset : 0);
									graph.StopAnimation();
									graph.EndHotRegion();
								}
								if (common.ProcessModeRegions)
								{
									this.AddSelectionPath(area, graphicsPath2, pointsPosition, num, num3, PointF.Empty, num4);
								}
							}
							if (common.ProcessModeRegions)
							{
								int insertIndex2 = common.HotRegionsList.FindInsertIndex();
								common.HotRegionsList.AddHotRegion(insertIndex2, graphicsPath2, false, graph, point2, item.Name, num);
							}
							num++;
						}
						else
						{
							num++;
						}
					}
					int num5 = 0;
					num = 0;
					foreach (DataPoint point3 in item.Points)
					{
						Color markerColor = point3.MarkerColor;
						MarkerStyle markerStyle = point3.MarkerStyle;
						RadarDrawingStyle drawingStyle3 = this.GetDrawingStyle(item, point3);
						if (axis.GetLogValue(point3.YValues[0]) > viewMaximum || axis.GetLogValue(point3.YValues[0]) < viewMinimum)
						{
							num++;
						}
						else
						{
							if (drawingStyle3 == RadarDrawingStyle.Marker && markerColor.IsEmpty)
							{
								markerColor = point3.Color;
							}
							SizeF markerSize = this.GetMarkerSize(graph, common, area, point3, point3.MarkerSize, point3.MarkerImage);
							if (common.ProcessModePaint)
							{
								if (markerStyle != 0 || point3.MarkerImage.Length > 0)
								{
									if (markerColor.IsEmpty)
									{
										markerColor = point3.Color;
									}
									if (num5 == 0)
									{
										graph.StartHotRegion(point3);
										graph.StartAnimation();
										graph.DrawMarkerAbs(pointsPosition[num], markerStyle, (int)markerSize.Height, markerColor, point3.MarkerBorderColor, point3.MarkerBorderWidth, point3.MarkerImage, point3.MarkerImageTransparentColor, (point3.series != null) ? point3.series.ShadowOffset : 0, (point3.series != null) ? point3.series.ShadowColor : Color.Empty, new RectangleF(pointsPosition[num].X, pointsPosition[num].Y, markerSize.Width, markerSize.Height), false);
										graph.StopAnimation();
										graph.EndHotRegion();
									}
									num5++;
									if (item.MarkerStep == num5)
									{
										num5 = 0;
									}
								}
								graph.StartAnimation();
								this.DrawLabels(area, graph, common, pointsPosition[num], (int)markerSize.Height, point3, item, num);
								graph.StopAnimation();
							}
							if (common.ProcessModeRegions)
							{
								SizeF relativeSize = graph.GetRelativeSize(markerSize);
								PointF relativePoint = graph.GetRelativePoint(pointsPosition[num]);
								int insertIndex3 = common.HotRegionsList.FindInsertIndex();
								if (point3.MarkerStyle == MarkerStyle.Circle)
								{
									float[] array = new float[3]
									{
										relativePoint.X,
										relativePoint.Y,
										(float)(relativeSize.Width / 2.0)
									};
									common.HotRegionsList.AddHotRegion(insertIndex3, graph, array[0], array[1], array[2], point3, item.Name, num);
								}
								else
								{
									common.HotRegionsList.AddHotRegion(insertIndex3, graph, new RectangleF((float)(relativePoint.X - relativeSize.Width / 2.0), (float)(relativePoint.Y - relativeSize.Height / 2.0), relativeSize.Width, relativeSize.Height), point3, item.Name, num);
								}
							}
							num++;
						}
					}
					if (!selection)
					{
						common.EventsManager.OnPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
					}
				}
			}
		}

		internal void AddSelectionPath(ChartArea area, GraphicsPath selectionPath, PointF[] dataPointPos, int firstPointIndex, int secondPointIndex, PointF centerPoint, int borderWidth)
		{
			PointF middlePoint = this.GetMiddlePoint(dataPointPos[firstPointIndex], dataPointPos[secondPointIndex]);
			PointF pointF = PointF.Empty;
			if (firstPointIndex > 0)
			{
				pointF = this.GetMiddlePoint(dataPointPos[firstPointIndex], dataPointPos[firstPointIndex - 1]);
			}
			else if (firstPointIndex == 0 && area.CircularSectorsNumber == dataPointPos.Length - 1)
			{
				pointF = this.GetMiddlePoint(dataPointPos[firstPointIndex], dataPointPos[dataPointPos.Length - 2]);
			}
			if (!centerPoint.IsEmpty)
			{
				selectionPath.AddLine(centerPoint, middlePoint);
				selectionPath.AddLine(middlePoint, dataPointPos[firstPointIndex]);
				if (pointF.IsEmpty)
				{
					selectionPath.AddLine(dataPointPos[firstPointIndex], centerPoint);
				}
				else
				{
					selectionPath.AddLine(dataPointPos[firstPointIndex], pointF);
					selectionPath.AddLine(pointF, centerPoint);
				}
			}
			else
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				if (!pointF.IsEmpty)
				{
					graphicsPath.AddLine(pointF, dataPointPos[firstPointIndex]);
				}
				graphicsPath.AddLine(dataPointPos[firstPointIndex], middlePoint);
				try
				{
					ChartGraphics.Widen(graphicsPath, new Pen(Color.Black, (float)(borderWidth + 2)));
					graphicsPath.Flatten();
				}
				catch
				{
				}
				selectionPath.AddPath(graphicsPath, false);
			}
		}

		private PointF GetMiddlePoint(PointF p1, PointF p2)
		{
			PointF empty = PointF.Empty;
			empty.X = (float)((p1.X + p2.X) / 2.0);
			empty.Y = (float)((p1.Y + p2.Y) / 2.0);
			return empty;
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

		protected virtual PointF[] GetPointsPosition(ChartGraphics graph, ChartArea area, Series series)
		{
			PointF[] array = new PointF[series.Points.Count + 1];
			int num = 0;
			foreach (DataPoint point in series.Points)
			{
				double yValue = this.GetYValue(this.common, area, series, point, num, 0);
				double position = area.AxisY.GetPosition(yValue);
				double num2 = (double)area.circularCenter.X;
				array[num] = graph.GetAbsolutePoint(new PointF((float)num2, (float)position));
				float angle = (float)(360.0 / (float)area.CircularSectorsNumber * (float)num);
				Matrix matrix = new Matrix();
				matrix.RotateAt(angle, graph.GetAbsolutePoint(area.circularCenter));
				PointF[] array2 = new PointF[1]
				{
					array[num]
				};
				matrix.TransformPoints(array2);
				array[num] = array2[0];
				num++;
			}
			array[num] = graph.GetAbsolutePoint(area.circularCenter);
			return array;
		}

		internal void DrawLabels(ChartArea area, ChartGraphics graph, CommonElements common, PointF markerPosition, int markerSize, DataPoint point, Series ser, int pointIndex)
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
				text = ValueConverter.FormatValue(ser.chart, point, point.YValues[0], point.LabelFormat, ser.YValueType, ChartElementType.DataPoint);
			}
			else
			{
				text = point.ReplaceKeywords(label);
				if (ser.chart != null && ser.chart.LocalizeTextHandler != null)
				{
					text = ser.chart.LocalizeTextHandler(point, text, point.ElementId, ChartElementType.DataPoint);
				}
			}
			SizeF sizeF = new SizeF((float)markerSize, (float)markerSize);
			SizeF sizeF2 = graph.MeasureString(text, point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic));
			SizeF size = new SizeF(sizeF2.Width, sizeF2.Height);
			size.Height += (float)(size.Height / 2.0);
			size.Width += size.Width / (float)text.Length;
			this.autoLabelPosition = true;
			string text2 = ((DataPointAttributes)point)["LabelStyle"];
			if (text2 == null || text2.Length == 0)
			{
				text2 = ((DataPointAttributes)ser)["LabelStyle"];
			}
			if (text2 != null && text2.Length > 0)
			{
				this.autoLabelPosition = false;
				if (string.Compare(text2, "Auto", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.autoLabelPosition = true;
					goto IL_02a9;
				}
				if (string.Compare(text2, "Center", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.labelPosition = LabelAlignmentTypes.Center;
					goto IL_02a9;
				}
				if (string.Compare(text2, "Bottom", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.labelPosition = LabelAlignmentTypes.Bottom;
					goto IL_02a9;
				}
				if (string.Compare(text2, "TopLeft", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.labelPosition = LabelAlignmentTypes.TopLeft;
					goto IL_02a9;
				}
				if (string.Compare(text2, "TopRight", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.labelPosition = LabelAlignmentTypes.TopRight;
					goto IL_02a9;
				}
				if (string.Compare(text2, "BottomLeft", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.labelPosition = LabelAlignmentTypes.BottomLeft;
					goto IL_02a9;
				}
				if (string.Compare(text2, "BottomRight", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.labelPosition = LabelAlignmentTypes.BottomRight;
					goto IL_02a9;
				}
				if (string.Compare(text2, "Left", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.labelPosition = LabelAlignmentTypes.Left;
					goto IL_02a9;
				}
				if (string.Compare(text2, "Right", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.labelPosition = LabelAlignmentTypes.Right;
					goto IL_02a9;
				}
				if (string.Compare(text2, "Top", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.labelPosition = LabelAlignmentTypes.Top;
					goto IL_02a9;
				}
				throw new ArgumentException(SR.ExceptionCustomAttributeValueInvalid(text2, "LabelStyle"));
			}
			goto IL_02a9;
			IL_02a9:
			if (this.autoLabelPosition)
			{
				this.labelPosition = this.GetAutoLabelPosition(area, ser, pointIndex);
			}
			PointF pointF = new PointF(markerPosition.X, markerPosition.Y);
			switch (this.labelPosition)
			{
			case LabelAlignmentTypes.Center:
				stringFormat.Alignment = StringAlignment.Center;
				break;
			case LabelAlignmentTypes.Bottom:
				stringFormat.Alignment = StringAlignment.Center;
				pointF.Y += (float)(sizeF.Height / 1.75);
				pointF.Y += (float)(size.Height / 2.0);
				break;
			case LabelAlignmentTypes.Top:
				stringFormat.Alignment = StringAlignment.Center;
				pointF.Y -= (float)(sizeF.Height / 1.75);
				pointF.Y -= (float)(size.Height / 2.0);
				break;
			case LabelAlignmentTypes.Left:
				stringFormat.Alignment = StringAlignment.Far;
				pointF.X -= (float)(sizeF.Height / 1.75);
				break;
			case LabelAlignmentTypes.TopLeft:
				stringFormat.Alignment = StringAlignment.Far;
				pointF.X -= (float)(sizeF.Height / 1.75);
				pointF.Y -= (float)(sizeF.Height / 1.75);
				pointF.Y -= (float)(size.Height / 2.0);
				break;
			case LabelAlignmentTypes.BottomLeft:
				stringFormat.Alignment = StringAlignment.Far;
				pointF.X -= (float)(sizeF.Height / 1.75);
				pointF.Y += (float)(sizeF.Height / 1.75);
				pointF.Y += (float)(size.Height / 2.0);
				break;
			case LabelAlignmentTypes.Right:
				pointF.X += (float)(sizeF.Height / 1.75);
				break;
			case LabelAlignmentTypes.TopRight:
				pointF.X += (float)(sizeF.Height / 1.75);
				pointF.Y -= (float)(sizeF.Height / 1.75);
				pointF.Y -= (float)(size.Height / 2.0);
				break;
			case LabelAlignmentTypes.BottomRight:
				pointF.X += (float)(sizeF.Height / 1.75);
				pointF.Y += (float)(sizeF.Height / 1.75);
				pointF.Y += (float)(size.Height / 2.0);
				break;
			}
			int angle = point.FontAngle;
			if (text.Trim().Length != 0)
			{
				if (ser.SmartLabels.Enabled)
				{
					pointF = graph.GetRelativePoint(pointF);
					markerPosition = graph.GetRelativePoint(markerPosition);
					sizeF2 = graph.GetRelativeSize(sizeF2);
					sizeF = graph.GetRelativeSize(sizeF);
					pointF = area.smartLabels.AdjustSmartLabelPosition(common, graph, area, ser.SmartLabels, pointF, sizeF2, ref stringFormat, markerPosition, sizeF, this.labelPosition);
					if (!pointF.IsEmpty)
					{
						pointF = graph.GetAbsolutePoint(pointF);
					}
					sizeF2 = graph.GetAbsoluteSize(sizeF2);
					angle = 0;
				}
				if (!pointF.IsEmpty)
				{
					pointF = graph.GetRelativePoint(pointF);
					RectangleF empty = RectangleF.Empty;
					size = graph.GetRelativeSize(sizeF2);
					size.Height += (float)(size.Height / 8.0);
					empty = PointChart.GetLabelPosition(graph, pointF, size, stringFormat, true);
					graph.DrawPointLabelStringRel(common, text, point.Font, new SolidBrush(point.FontColor), pointF, stringFormat, angle, empty, point.LabelBackColor, point.LabelBorderColor, point.LabelBorderWidth, point.LabelBorderStyle, ser, point, pointIndex);
				}
			}
		}

		protected virtual LabelAlignmentTypes GetAutoLabelPosition(ChartArea area, Series series, int pointIndex)
		{
			LabelAlignmentTypes result = LabelAlignmentTypes.Top;
			float num = (float)(360.0 / (float)area.CircularSectorsNumber * (float)pointIndex);
			if (num == 0.0)
			{
				result = LabelAlignmentTypes.TopRight;
			}
			else if (num >= 0.0 && num <= 45.0)
			{
				result = LabelAlignmentTypes.Top;
			}
			else if (num >= 45.0 && num <= 90.0)
			{
				result = LabelAlignmentTypes.TopRight;
			}
			else if (num >= 90.0 && num <= 135.0)
			{
				result = LabelAlignmentTypes.BottomRight;
			}
			else if (num >= 135.0 && num <= 180.0)
			{
				result = LabelAlignmentTypes.BottomRight;
			}
			else if (num >= 180.0 && num <= 225.0)
			{
				result = LabelAlignmentTypes.BottomLeft;
			}
			else if (num >= 225.0 && num <= 270.0)
			{
				result = LabelAlignmentTypes.BottomLeft;
			}
			else if (num >= 270.0 && num <= 315.0)
			{
				result = LabelAlignmentTypes.TopLeft;
			}
			else if (num >= 315.0 && num <= 360.0)
			{
				result = LabelAlignmentTypes.TopLeft;
			}
			return result;
		}

		protected virtual RadarDrawingStyle GetDrawingStyle(Series ser, DataPoint point)
		{
			RadarDrawingStyle result = RadarDrawingStyle.Area;
			if (!point.IsAttributeSet("RadarDrawingStyle") && !ser.IsAttributeSet("RadarDrawingStyle"))
			{
				goto IL_0089;
			}
			string text = point.IsAttributeSet("RadarDrawingStyle") ? ((DataPointAttributes)point)["RadarDrawingStyle"] : ((DataPointAttributes)ser)["RadarDrawingStyle"];
			if (string.Compare(text, "Area", StringComparison.OrdinalIgnoreCase) == 0)
			{
				result = RadarDrawingStyle.Area;
				goto IL_0089;
			}
			if (string.Compare(text, "Line", StringComparison.OrdinalIgnoreCase) == 0)
			{
				result = RadarDrawingStyle.Line;
				goto IL_0089;
			}
			if (string.Compare(text, "Marker", StringComparison.OrdinalIgnoreCase) == 0)
			{
				result = RadarDrawingStyle.Marker;
				goto IL_0089;
			}
			throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, "RadarDrawingStyle"));
			IL_0089:
			return result;
		}

		public virtual double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			if (yValueIndex == -1)
			{
				return 0.0;
			}
			if (point.Empty)
			{
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
			return point.YValues[yValueIndex];
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
				num = series.Points[num3].YValues[0];
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
				num2 = series.Points[num4].YValues[0];
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
			PointF[] pointsPosition = this.GetPointsPosition(common.graph, area, series);
			int num = 0;
			int num2 = 0;
			foreach (DataPoint point in series.Points)
			{
				Color color = point.MarkerColor;
				MarkerStyle markerStyle = point.MarkerStyle;
				RadarDrawingStyle drawingStyle = this.GetDrawingStyle(series, point);
				if (drawingStyle == RadarDrawingStyle.Marker)
				{
					color = point.Color;
				}
				SizeF size = this.GetMarkerSize(common.graph, common, area, point, point.MarkerSize, point.MarkerImage);
				if (markerStyle != 0 || point.MarkerImage.Length > 0)
				{
					if (color.IsEmpty)
					{
						color = point.Color;
					}
					if (num == 0)
					{
						PointF relativePoint = common.graph.GetRelativePoint(pointsPosition[num2]);
						size = common.graph.GetRelativeSize(size);
						RectangleF rectangleF = new RectangleF((float)(relativePoint.X - size.Width / 2.0), (float)(relativePoint.Y - size.Height / 2.0), size.Width, size.Height);
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
