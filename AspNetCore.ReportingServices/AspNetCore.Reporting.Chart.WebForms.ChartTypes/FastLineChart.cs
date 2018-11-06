using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class FastLineChart : IChartType
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
				return "FastLine";
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
			return LegendImageStyle.Line;
		}

		public virtual Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}

		public virtual void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			this.common = common;
			this.graph = graph;
			bool flag = false;
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
					num2 = 1f;
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
						if (!(num2 < 0.0) && !(num2 > 1.0))
						{
							goto IL_019c;
						}
						throw new InvalidOperationException(SR.ExceptionCustomAttributeIsNotInRange0to1("PermittedPixelError"));
					}
					goto IL_019c;
				}
				continue;
				IL_019c:
				SizeF relativeSize = graph.GetRelativeSize(new SizeF(num2, num2));
				SizeF relativeSize2 = graph.GetRelativeSize(new SizeF((float)viewMinimum, (float)viewMinimum2));
				double num3 = Math.Abs(axis.PositionToValue((double)(relativeSize2.Width + relativeSize.Width), false) - axis.PositionToValue((double)relativeSize2.Width, false));
				Math.Abs(axis2.PositionToValue((double)(relativeSize2.Height + relativeSize.Height), false) - axis2.PositionToValue((double)relativeSize2.Height, false));
				Pen pen = new Pen(item.Color, (float)item.BorderWidth);
				pen.DashStyle = graph.GetPenStyle(item.BorderStyle);
				pen.StartCap = LineCap.Round;
				pen.EndCap = LineCap.Round;
				Pen pen2 = new Pen(item.EmptyPointStyle.Color, (float)item.EmptyPointStyle.BorderWidth);
				pen2.DashStyle = graph.GetPenStyle(item.EmptyPointStyle.BorderStyle);
				pen2.StartCap = LineCap.Round;
				pen2.EndCap = LineCap.Round;
				bool flag2 = area.IndexedSeries(item.Name);
				int num4 = 0;
				double num5 = double.NaN;
				double num6 = double.NaN;
				DataPoint pointMin = null;
				DataPoint pointMax = null;
				double num7 = 0.0;
				double num8 = 0.0;
				double num9 = 0.0;
				double num10 = 0.0;
				DataPoint dataPoint = null;
				PointF empty = PointF.Empty;
				PointF pointF = PointF.Empty;
				PointF empty2 = PointF.Empty;
				bool flag3 = false;
				bool flag4 = false;
				bool flag5 = false;
				bool flag6 = false;
				bool flag7 = false;
				double num11 = ((double)graph.common.ChartPicture.Width - 1.0) / 100.0;
				double num12 = ((double)graph.common.ChartPicture.Height - 1.0) / 100.0;
				foreach (DataPoint point in item.Points)
				{
					num7 = (flag2 ? ((double)(num4 + 1)) : point.XValue);
					num7 = axis.GetLogValue(num7);
					num8 = axis2.GetLogValue(point.YValues[0]);
					flag6 = point.Empty;
					if (flag5 && !flag6 && !flag7)
					{
						flag7 = true;
						flag6 = true;
					}
					else
					{
						flag7 = false;
					}
					if (!flag4)
					{
						if (num7 < viewMinimum && num9 < viewMinimum)
						{
							goto IL_041a;
						}
						if (num7 > viewMaximum && num9 > viewMaximum)
						{
							goto IL_041a;
						}
						if (num8 < viewMinimum2 && num10 < viewMinimum2)
						{
							goto IL_041a;
						}
						if (num8 > viewMaximum2 && num10 > viewMaximum2)
						{
							goto IL_041a;
						}
					}
					if (!flag && (num9 < viewMinimum || num9 > viewMaximum || num7 > viewMaximum || num7 < viewMinimum || num10 < viewMinimum2 || num10 > viewMaximum2 || num8 < viewMinimum2 || num8 > viewMaximum2))
					{
						graph.SetClip(area.PlotAreaPosition.ToRectangleF());
						flag = true;
					}
					if (num4 > 0 && flag6 == flag5 && Math.Abs(num7 - num9) < num3)
					{
						if (!flag4)
						{
							flag4 = true;
							if (num8 > num10)
							{
								num6 = num8;
								num5 = num10;
								pointMax = point;
								pointMin = dataPoint;
							}
							else
							{
								num6 = num10;
								num5 = num8;
								pointMax = dataPoint;
								pointMin = point;
							}
						}
						else if (num8 > num6)
						{
							num6 = num8;
							pointMax = point;
						}
						else if (num8 < num5)
						{
							num5 = num8;
							pointMin = point;
						}
						dataPoint = point;
						empty.Y = (float)num8;
						num4++;
					}
					else
					{
						empty2.X = (float)(axis.GetLinearPosition(num7) * num11);
						empty2.Y = (float)(axis2.GetLinearPosition(num8) * num12);
						if (flag3)
						{
							pointF.X = (float)(axis.GetLinearPosition(num9) * num11);
							pointF.Y = (float)(axis2.GetLinearPosition(num10) * num12);
						}
						if (flag4)
						{
							num5 = axis2.GetLinearPosition(num5) * num12;
							num6 = axis2.GetLinearPosition(num6) * num12;
							this.DrawLine(item, dataPoint, pointMin, pointMax, num4, flag5 ? pen2 : pen, pointF.X, (float)num5, pointF.X, (float)num6);
							flag4 = false;
							pointF.Y = (float)(axis2.GetLinearPosition((double)empty.Y) * num12);
						}
						if (num4 > 0)
						{
							this.DrawLine(item, point, pointMin, pointMax, num4, flag6 ? pen2 : pen, pointF.X, pointF.Y, empty2.X, empty2.Y);
						}
						num9 = num7;
						num10 = num8;
						dataPoint = point;
						pointF = empty2;
						flag3 = false;
						flag5 = flag6;
						num4++;
					}
					continue;
					IL_041a:
					num9 = num7;
					num10 = num8;
					flag3 = true;
					num4++;
				}
				if (flag4)
				{
					if (flag3)
					{
						pointF.X = (float)(axis.GetLinearPosition(num9) * num11);
						pointF.Y = (float)(axis2.GetLinearPosition(num10) * num12);
					}
					num5 = axis2.GetLinearPosition(num5) * num12;
					num6 = axis2.GetLinearPosition(num6) * num12;
					this.DrawLine(item, dataPoint, pointMin, pointMax, num4 - 1, flag5 ? pen2 : pen, pointF.X, (float)num5, pointF.X, (float)num6);
					flag4 = false;
					num5 = double.NaN;
					num6 = double.NaN;
					pointMin = null;
					pointMax = null;
				}
			}
			if (flag)
			{
				graph.ResetClip();
			}
		}

		public virtual void DrawLine(Series series, DataPoint point, DataPoint pointMin, DataPoint pointMax, int pointIndex, Pen pen, float firstPointX, float firstPointY, float secondPointX, float secondPointY)
		{
			if (this.chartArea3DEnabled)
			{
				Point3D[] array = new Point3D[2];
				PointF relativePoint = this.graph.GetRelativePoint(new PointF(firstPointX, firstPointY));
				PointF relativePoint2 = this.graph.GetRelativePoint(new PointF(secondPointX, secondPointY));
				array[0] = new Point3D(relativePoint.X, relativePoint.Y, this.seriesZCoordinate);
				array[1] = new Point3D(relativePoint2.X, relativePoint2.Y, this.seriesZCoordinate);
				this.matrix3D.TransformPoints(array);
				array[0].PointF = this.graph.GetAbsolutePoint(array[0].PointF);
				array[1].PointF = this.graph.GetAbsolutePoint(array[1].PointF);
				firstPointX = array[0].X;
				firstPointY = array[0].Y;
				secondPointX = array[1].X;
				secondPointY = array[1].Y;
			}
			this.graph.DrawLine(pen, firstPointX, firstPointY, secondPointX, secondPointY);
			if (this.common.ProcessModeRegions)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				float num = (float)(pen.Width + 2.0);
				if (Math.Abs(firstPointX - secondPointX) > Math.Abs(firstPointY - secondPointY))
				{
					graphicsPath.AddLine(firstPointX, firstPointY - num, secondPointX, secondPointY - num);
					graphicsPath.AddLine(secondPointX, secondPointY + num, firstPointX, firstPointY + num);
					graphicsPath.CloseAllFigures();
				}
				else
				{
					graphicsPath.AddLine(firstPointX - num, firstPointY, secondPointX - num, secondPointY);
					graphicsPath.AddLine(secondPointX + num, secondPointY, firstPointX + num, firstPointY);
					graphicsPath.CloseAllFigures();
				}
				RectangleF bounds = graphicsPath.GetBounds();
				if ((double)bounds.Width <= 2.0 || (double)bounds.Height <= 2.0)
				{
					bounds.Inflate(pen.Width, pen.Width);
					this.common.HotRegionsList.AddHotRegion(this.graph, this.graph.GetRelativeRectangle(bounds), point, point.series.Name, pointIndex);
				}
				else
				{
					this.common.HotRegionsList.AddHotRegion(graphicsPath, false, this.graph, point, point.series.Name, pointIndex);
				}
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
