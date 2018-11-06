using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class FastPointChart : IChartType
	{
		internal bool chartArea3DEnabled;

		internal ChartGraphics graph;

		internal float seriesZCoordinate;

		internal Matrix3D matrix3D;

		internal CommonElements common;

		public virtual string Name
		{
			get
			{
				return "FastPoint";
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
			this.graph = graph;
			if (area.Area3DStyle.Enable3D)
			{
				this.chartArea3DEnabled = true;
				this.matrix3D = area.matrix3D;
			}
			else
			{
				this.chartArea3DEnabled = false;
			}
			foreach (Series item in common.DataManager.Series)
			{
				Axis axis;
				Axis axis2;
				double viewMinimum;
				double viewMaximum;
				double viewMinimum2;
				double viewMaximum2;
				float num2;
				if (string.Compare(item.ChartTypeName, this.Name, true, CultureInfo.CurrentCulture) == 0 && !(item.ChartArea != area.Name) && item.IsVisible())
				{
					if (this.chartArea3DEnabled)
					{
						float num = default(float);
						((ChartArea3D)area).GetSeriesZPositionAndDepth(item, out num, out this.seriesZCoordinate);
						this.seriesZCoordinate += (float)(num / 2.0);
					}
					axis = area.GetAxis(AxisName.X, item.XAxisType, area.Area3DStyle.Enable3D ? string.Empty : item.XSubAxisName);
					axis2 = area.GetAxis(AxisName.Y, item.YAxisType, area.Area3DStyle.Enable3D ? string.Empty : item.YSubAxisName);
					viewMinimum = axis.GetViewMinimum();
					viewMaximum = axis.GetViewMaximum();
					viewMinimum2 = axis2.GetViewMinimum();
					viewMaximum2 = axis2.GetViewMaximum();
					num2 = (float)((float)item.MarkerSize / 3.0);
					if (item.IsAttributeSet("PixelPointGapDepth"))
					{
						string s = ((DataPointAttributes)item)["PixelPointGapDepth"];
						try
						{
							num2 = float.Parse(s, CultureInfo.CurrentCulture);
						}
						catch
						{
							throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid2("PermittedPixelError"));
						}
						if (!(num2 < 0.0) && !(num2 > 50.0))
						{
							goto IL_019f;
						}
						throw new InvalidOperationException(SR.ExceptionCustomAttributeIsNotInRange0to50("PermittedPixelError"));
					}
					goto IL_019f;
				}
				continue;
				IL_019f:
				SizeF relativeSize = graph.GetRelativeSize(new SizeF(num2, num2));
				SizeF relativeSize2 = graph.GetRelativeSize(new SizeF((float)viewMinimum, (float)viewMinimum2));
				double num3 = Math.Abs(axis.PositionToValue((double)(relativeSize2.Width + relativeSize.Width), false) - axis.PositionToValue((double)relativeSize2.Width, false));
				double num4 = Math.Abs(axis2.PositionToValue((double)(relativeSize2.Height + relativeSize.Height), false) - axis2.PositionToValue((double)relativeSize2.Height, false));
				SolidBrush solidBrush = new SolidBrush(item.MarkerColor.IsEmpty ? item.Color : item.MarkerColor);
				SolidBrush solidBrush2 = new SolidBrush(item.EmptyPointStyle.MarkerColor.IsEmpty ? item.EmptyPointStyle.Color : item.EmptyPointStyle.MarkerColor);
				Pen pen = null;
				Pen pen2 = null;
				if (!item.MarkerBorderColor.IsEmpty && item.MarkerBorderWidth > 0)
				{
					pen = new Pen(item.MarkerBorderColor, (float)item.MarkerBorderWidth);
				}
				if (!item.EmptyPointStyle.MarkerBorderColor.IsEmpty && item.EmptyPointStyle.MarkerBorderWidth > 0)
				{
					pen2 = new Pen(item.EmptyPointStyle.MarkerBorderColor, (float)item.EmptyPointStyle.MarkerBorderWidth);
				}
				bool flag = area.IndexedSeries(item.Name);
				int num5 = 0;
				double num6 = 0.0;
				double num7 = 0.0;
				double num8 = 0.0;
				double num9 = 0.0;
				PointF empty = PointF.Empty;
				bool flag2 = false;
				double num10 = ((double)graph.common.ChartPicture.Width - 1.0) / 100.0;
				double num11 = ((double)graph.common.ChartPicture.Height - 1.0) / 100.0;
				int markerSize = item.MarkerSize;
				MarkerStyle markerStyle = item.MarkerStyle;
				MarkerStyle markerStyle2 = item.EmptyPointStyle.MarkerStyle;
				foreach (DataPoint point in item.Points)
				{
					num6 = (flag ? ((double)(num5 + 1)) : point.XValue);
					num6 = axis.GetLogValue(num6);
					num7 = axis2.GetLogValue(point.YValues[0]);
					flag2 = point.Empty;
					if (num6 < viewMinimum || num6 > viewMaximum || num7 < viewMinimum2 || num7 > viewMaximum2)
					{
						num8 = num6;
						num9 = num7;
						num5++;
					}
					else if (num5 > 0 && Math.Abs(num6 - num8) < num3 && Math.Abs(num7 - num9) < num4)
					{
						num5++;
					}
					else
					{
						empty.X = (float)(axis.GetLinearPosition(num6) * num10);
						empty.Y = (float)(axis2.GetLinearPosition(num7) * num11);
						this.DrawMarker(graph, point, num5, empty, flag2 ? markerStyle2 : markerStyle, markerSize, flag2 ? solidBrush2 : solidBrush, flag2 ? pen2 : pen);
						num8 = num6;
						num9 = num7;
						num5++;
					}
				}
				solidBrush.Dispose();
				solidBrush2.Dispose();
				if (pen != null)
				{
					pen.Dispose();
				}
				if (pen2 != null)
				{
					pen2.Dispose();
				}
			}
		}

		protected virtual void DrawMarker(ChartGraphics graph, DataPoint point, int pointIndex, PointF location, MarkerStyle markerStyle, int markerSize, Brush brush, Pen borderPen)
		{
			if (this.chartArea3DEnabled)
			{
				Point3D[] array = new Point3D[1];
				location = graph.GetRelativePoint(location);
				array[0] = new Point3D(location.X, location.Y, this.seriesZCoordinate);
				this.matrix3D.TransformPoints(array);
				location.X = array[0].X;
				location.Y = array[0].Y;
				location = graph.GetAbsolutePoint(location);
			}
			RectangleF rectangleF = new RectangleF((float)(location.X - (float)markerSize / 2.0), (float)(location.Y - (float)markerSize / 2.0), (float)markerSize, (float)markerSize);
			switch (markerStyle)
			{
			case MarkerStyle.Star4:
			case MarkerStyle.Star5:
			case MarkerStyle.Star6:
			case MarkerStyle.Star10:
			{
				int numberOfCorners = 4;
				switch (markerStyle)
				{
				case MarkerStyle.Star5:
					numberOfCorners = 5;
					break;
				case MarkerStyle.Star6:
					numberOfCorners = 6;
					break;
				case MarkerStyle.Star10:
					numberOfCorners = 10;
					break;
				}
				PointF[] points = graph.CreateStarPolygon(rectangleF, numberOfCorners);
				graph.FillPolygon(brush, points);
				if (borderPen != null)
				{
					graph.DrawPolygon(borderPen, points);
				}
				break;
			}
			case MarkerStyle.Circle:
				graph.FillEllipse(brush, rectangleF);
				if (borderPen != null)
				{
					graph.DrawEllipse(borderPen, rectangleF);
				}
				break;
			case MarkerStyle.Square:
				graph.FillRectangle(brush, rectangleF);
				if (borderPen != null)
				{
					graph.DrawRectangle(borderPen, (int)Math.Round((double)rectangleF.X, 0), (int)Math.Round((double)rectangleF.Y, 0), (int)Math.Round((double)rectangleF.Width, 0), (int)Math.Round((double)rectangleF.Height, 0));
				}
				break;
			case MarkerStyle.Cross:
			{
				float num = (float)Math.Ceiling((float)markerSize / 4.0);
				float num2 = (float)markerSize;
				PointF[] array4 = new PointF[12];
				array4[0].X = (float)(location.X - num2 / 2.0);
				array4[0].Y = (float)(location.Y + num / 2.0);
				array4[1].X = (float)(location.X - num2 / 2.0);
				array4[1].Y = (float)(location.Y - num / 2.0);
				array4[2].X = (float)(location.X - num / 2.0);
				array4[2].Y = (float)(location.Y - num / 2.0);
				array4[3].X = (float)(location.X - num / 2.0);
				array4[3].Y = (float)(location.Y - num2 / 2.0);
				array4[4].X = (float)(location.X + num / 2.0);
				array4[4].Y = (float)(location.Y - num2 / 2.0);
				array4[5].X = (float)(location.X + num / 2.0);
				array4[5].Y = (float)(location.Y - num / 2.0);
				array4[6].X = (float)(location.X + num2 / 2.0);
				array4[6].Y = (float)(location.Y - num / 2.0);
				array4[7].X = (float)(location.X + num2 / 2.0);
				array4[7].Y = (float)(location.Y + num / 2.0);
				array4[8].X = (float)(location.X + num / 2.0);
				array4[8].Y = (float)(location.Y + num / 2.0);
				array4[9].X = (float)(location.X + num / 2.0);
				array4[9].Y = (float)(location.Y + num2 / 2.0);
				array4[10].X = (float)(location.X - num / 2.0);
				array4[10].Y = (float)(location.Y + num2 / 2.0);
				array4[11].X = (float)(location.X - num / 2.0);
				array4[11].Y = (float)(location.Y + num / 2.0);
				Matrix matrix = new Matrix();
				matrix.RotateAt(45f, location);
				matrix.TransformPoints(array4);
				matrix.Dispose();
				graph.FillPolygon(brush, array4);
				if (borderPen != null)
				{
					graph.DrawPolygon(borderPen, array4);
				}
				break;
			}
			case MarkerStyle.Diamond:
			{
				PointF[] array3 = new PointF[4];
				array3[0].X = rectangleF.X;
				array3[0].Y = (float)(rectangleF.Y + rectangleF.Height / 2.0);
				array3[1].X = (float)(rectangleF.X + rectangleF.Width / 2.0);
				array3[1].Y = rectangleF.Top;
				array3[2].X = rectangleF.Right;
				array3[2].Y = (float)(rectangleF.Y + rectangleF.Height / 2.0);
				array3[3].X = (float)(rectangleF.X + rectangleF.Width / 2.0);
				array3[3].Y = rectangleF.Bottom;
				graph.FillPolygon(brush, array3);
				if (borderPen != null)
				{
					graph.DrawPolygon(borderPen, array3);
				}
				break;
			}
			case MarkerStyle.Triangle:
			{
				PointF[] array2 = new PointF[3];
				array2[0].X = rectangleF.X;
				array2[0].Y = rectangleF.Bottom;
				array2[1].X = (float)(rectangleF.X + rectangleF.Width / 2.0);
				array2[1].Y = rectangleF.Top;
				array2[2].X = rectangleF.Right;
				array2[2].Y = rectangleF.Bottom;
				graph.FillPolygon(brush, array2);
				if (borderPen != null)
				{
					graph.DrawPolygon(borderPen, array2);
				}
				break;
			}
			default:
				throw new InvalidOperationException(SR.ExceptionFastPointMarkerStyleUnknown);
			}
			if (this.common.ProcessModeRegions)
			{
				this.common.HotRegionsList.AddHotRegion(graph, graph.GetRelativeRectangle(rectangleF), point, point.series.Name, pointIndex);
			}
		}

		public virtual double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			return point.YValues[yValueIndex];
		}

		public void AddSmartLabelMarkerPositions(CommonElements common, ChartArea area, Series series, ArrayList list)
		{
		}
	}
}
