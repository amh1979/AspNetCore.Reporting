using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[DefaultProperty("Enabled")]
	[SRDescription("DescriptionAttributeTickMark_TickMark")]
	internal class TickMark : Grid
	{
		private TickMarkStyle style = TickMarkStyle.Outside;

		private float size = 1f;

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeTickMark_Style")]
		[Bindable(true)]
		[DefaultValue(TickMarkStyle.Outside)]
		public TickMarkStyle Style
		{
			get
			{
				return this.style;
			}
			set
			{
				this.style = value;
				base.Invalidate();
			}
		}

		[DefaultValue(1f)]
		[SRDescription("DescriptionAttributeTickMark_Size")]
		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		public float Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
				base.Invalidate();
			}
		}

		public TickMark()
			: base(null, true)
		{
		}

		public TickMark(Axis axis, bool major)
			: base(axis, major)
		{
		}

		internal void Paint(ChartGraphics graph, bool backElements)
		{
			PointF empty = PointF.Empty;
			PointF empty2 = PointF.Empty;
			if (base.enabled)
			{
				double interval = base.interval;
				DateTimeIntervalType intervalType = base.intervalType;
				double intervalOffset = base.intervalOffset;
				DateTimeIntervalType intervalOffsetType = base.intervalOffsetType;
				if (!base.majorGridTick && (base.interval == 0.0 || double.IsNaN(base.interval)))
				{
					if (base.axis.majorGrid.IntervalType == DateTimeIntervalType.Auto)
					{
						base.interval = base.axis.majorGrid.Interval / 5.0;
					}
					else
					{
						DateTimeIntervalType intervalType2 = base.axis.majorGrid.IntervalType;
						base.interval = ((AxisScale)base.axis).CalcInterval(base.axis.GetViewMinimum(), base.axis.GetViewMinimum() + (base.axis.GetViewMaximum() - base.axis.GetViewMinimum()) / 4.0, true, out intervalType2, ChartValueTypes.DateTime);
						base.intervalType = intervalType2;
						base.intervalOffsetType = base.axis.majorGrid.IntervalOffsetType;
						base.intervalOffset = base.axis.majorGrid.IntervalOffset;
					}
				}
				if (this.style != 0)
				{
					if (base.axis.IsCustomTickMarks())
					{
						this.PaintCustom(graph, backElements);
					}
					else
					{
						base.axis.PlotAreaPosition.ToRectangleF();
						Series series = null;
						if (base.axis.axisType == AxisName.X || base.axis.axisType == AxisName.X2)
						{
							ArrayList xAxesSeries = base.axis.chartArea.GetXAxesSeries((AxisType)((base.axis.axisType != 0) ? 1 : 0), base.axis.SubAxisName);
							if (xAxesSeries.Count > 0)
							{
								series = base.axis.Common.DataManager.Series[xAxesSeries[0]];
								if (series != null && !series.XValueIndexed)
								{
									series = null;
								}
							}
						}
						double num = base.axis.GetViewMinimum();
						DateTimeIntervalType dateTimeIntervalType = (base.IntervalOffsetType == DateTimeIntervalType.Auto) ? base.IntervalType : base.IntervalOffsetType;
						if (!base.axis.chartArea.chartAreaIsCurcular || base.axis.axisType == AxisName.Y || base.axis.axisType == AxisName.Y2)
						{
							num = base.axis.AlignIntervalStart(num, base.Interval, base.IntervalType, series, base.majorGridTick);
						}
						if (base.IntervalOffset != 0.0 && !double.IsNaN(base.IntervalOffset) && series == null)
						{
							num += base.axis.GetIntervalSize(num, base.IntervalOffset, dateTimeIntervalType, series, 0.0, DateTimeIntervalType.Number, true, false);
						}
						if (!((base.axis.GetViewMaximum() - base.axis.GetViewMinimum()) / base.axis.GetIntervalSize(num, base.Interval, base.IntervalType, series, 0.0, DateTimeIntervalType.Number, true) > 10000.0) && !(base.axis.GetViewMaximum() <= base.axis.GetViewMinimum()))
						{
							float num2 = 0f;
							if (base.axis.ScrollBar.IsVisible() && base.axis.ScrollBar.PositionInside && (base.axis.IsAxisOnAreaEdge() || !base.axis.MarksNextToAxis))
							{
								num2 = (float)base.axis.ScrollBar.GetScrollBarRelativeSize();
							}
							if (base.axis.AxisPosition == AxisPosition.Left)
							{
								float num3 = (!base.axis.IsMarksNextToAxis()) ? base.axis.PlotAreaPosition.X : ((float)base.axis.GetAxisPosition());
								if (this.style == TickMarkStyle.Inside)
								{
									empty.X = num3;
									empty2.X = num3 + this.size;
								}
								else if (this.style == TickMarkStyle.Outside)
								{
									empty.X = num3 - this.size - num2;
									empty2.X = num3;
								}
								else if (this.style == TickMarkStyle.Cross)
								{
									empty.X = (float)(num3 - this.size / 2.0 - num2);
									empty2.X = (float)(num3 + this.size / 2.0);
								}
							}
							else if (base.axis.AxisPosition == AxisPosition.Right)
							{
								float num3 = (!base.axis.IsMarksNextToAxis()) ? base.axis.PlotAreaPosition.Right() : ((float)base.axis.GetAxisPosition());
								if (this.style == TickMarkStyle.Inside)
								{
									empty.X = num3 - this.size;
									empty2.X = num3;
								}
								else if (this.style == TickMarkStyle.Outside)
								{
									empty.X = num3;
									empty2.X = num3 + this.size + num2;
								}
								else if (this.style == TickMarkStyle.Cross)
								{
									empty.X = (float)(num3 - this.size / 2.0);
									empty2.X = (float)(num3 + this.size / 2.0 + num2);
								}
							}
							else if (base.axis.AxisPosition == AxisPosition.Top)
							{
								float num3 = (!base.axis.IsMarksNextToAxis()) ? base.axis.PlotAreaPosition.Y : ((float)base.axis.GetAxisPosition());
								if (this.style == TickMarkStyle.Inside)
								{
									empty.Y = num3;
									empty2.Y = num3 + this.size;
								}
								else if (this.style == TickMarkStyle.Outside)
								{
									empty.Y = num3 - this.size - num2;
									empty2.Y = num3;
								}
								else if (this.style == TickMarkStyle.Cross)
								{
									empty.Y = (float)(num3 - this.size / 2.0 - num2);
									empty2.Y = (float)(num3 + this.size / 2.0);
								}
							}
							else if (base.axis.AxisPosition == AxisPosition.Bottom)
							{
								float num3 = (!base.axis.IsMarksNextToAxis()) ? base.axis.PlotAreaPosition.Bottom() : ((float)base.axis.GetAxisPosition());
								if (this.style == TickMarkStyle.Inside)
								{
									empty.Y = num3 - this.size;
									empty2.Y = num3;
								}
								else if (this.style == TickMarkStyle.Outside)
								{
									empty.Y = num3;
									empty2.Y = num3 + this.size + num2;
								}
								else if (this.style == TickMarkStyle.Cross)
								{
									empty.Y = (float)(num3 - this.size / 2.0);
									empty2.Y = (float)(num3 + this.size / 2.0 + num2);
								}
							}
							int num4 = 0;
							int num5 = 1;
							double num6 = num;
							double num7 = 0.0;
							while (num <= base.axis.GetViewMaximum())
							{
								double num8 = 0.0;
								if (base.majorGridTick || !base.axis.Logarithmic)
								{
									num7 = base.axis.GetIntervalSize(num, base.Interval, base.IntervalType, series, base.IntervalOffset, dateTimeIntervalType, true);
								}
								else
								{
									double logMinimum = this.GetLogMinimum(num, series);
									if (num6 != logMinimum)
									{
										num6 = logMinimum;
										num5 = 1;
									}
									num8 = Math.Log(1.0 + base.interval * (double)num5, base.axis.logarithmBase);
									num = num6;
									num7 = num8;
									num5++;
									if (this.GetLogMinimum(num + num8, series) != logMinimum)
									{
										num += num8;
										continue;
									}
								}
								if (num == base.axis.GetViewMaximum() && series != null)
								{
									num += num7;
								}
								else
								{
									if (num7 == 0.0)
									{
										throw new InvalidOperationException(SR.ExceptionTickMarksIntervalIsZero);
									}
									if (num4++ > 10000)
									{
										break;
									}
									if (base.axis != null && base.axis.chartArea != null && base.axis.chartArea.chartAreaIsCurcular)
									{
										if (!base.axis.Reverse && num == base.axis.GetViewMinimum())
										{
											goto IL_07e7;
										}
										if (base.axis.Reverse && num == base.axis.GetViewMaximum())
										{
											goto IL_07e7;
										}
									}
									if (!base.majorGridTick && base.axis.Logarithmic)
									{
										num += num8;
										if (num > base.axis.GetViewMaximum())
										{
											break;
										}
									}
									if ((decimal)num >= (decimal)base.axis.GetViewMinimum())
									{
										if (base.axis.AxisPosition == AxisPosition.Left)
										{
											empty.Y = (float)base.axis.GetLinearPosition(num);
											empty2.Y = empty.Y;
										}
										else if (base.axis.AxisPosition == AxisPosition.Right)
										{
											empty.Y = (float)base.axis.GetLinearPosition(num);
											empty2.Y = empty.Y;
										}
										else if (base.axis.AxisPosition == AxisPosition.Top)
										{
											empty.X = (float)base.axis.GetLinearPosition(num);
											empty2.X = empty.X;
										}
										else if (base.axis.AxisPosition == AxisPosition.Bottom)
										{
											empty.X = (float)base.axis.GetLinearPosition(num);
											empty2.X = empty.X;
										}
										if (base.axis.Common.ProcessModeRegions)
										{
											if (base.axis.chartArea.chartAreaIsCurcular)
											{
												RectangleF relative = new RectangleF((float)(empty.X - 0.5), (float)(empty.Y - 0.5), (float)(Math.Abs(empty2.X - empty.X) + 1.0), (float)(Math.Abs(empty2.Y - empty.Y) + 1.0));
												GraphicsPath graphicsPath = new GraphicsPath();
												graphicsPath.AddRectangle(graph.GetAbsoluteRectangle(relative));
												graphicsPath.Transform(graph.Transform);
												base.axis.Common.HotRegionsList.AddHotRegion(graphicsPath, false, graph, ChartElementType.TickMarks, this);
											}
											else if (!base.axis.chartArea.Area3DStyle.Enable3D || base.axis.chartArea.chartAreaIsCurcular)
											{
												RectangleF rectArea = new RectangleF((float)(empty.X - 0.5), (float)(empty.Y - 0.5), (float)(Math.Abs(empty2.X - empty.X) + 1.0), (float)(Math.Abs(empty2.Y - empty.Y) + 1.0));
												base.axis.Common.HotRegionsList.AddHotRegion(rectArea, this, ChartElementType.TickMarks, true);
											}
											else
											{
												this.Draw3DTickLine(graph, empty, empty2, base.axis.AxisPosition == AxisPosition.Left || base.axis.AxisPosition == AxisPosition.Right, num2, backElements);
											}
										}
										if (base.axis.Common.ProcessModePaint)
										{
											if (!base.axis.chartArea.Area3DStyle.Enable3D || base.axis.chartArea.chartAreaIsCurcular)
											{
												graph.StartAnimation();
												graph.DrawLineRel(base.borderColor, base.borderWidth, base.borderStyle, empty, empty2);
												graph.StopAnimation();
											}
											else
											{
												graph.StartAnimation();
												this.Draw3DTickLine(graph, empty, empty2, base.axis.AxisPosition == AxisPosition.Left || base.axis.AxisPosition == AxisPosition.Right, num2, backElements);
												graph.StopAnimation();
											}
										}
									}
									if (base.majorGridTick || !base.axis.Logarithmic)
									{
										num += num7;
									}
								}
								continue;
								IL_07e7:
								num += num7;
							}
							if (!base.majorGridTick)
							{
								base.interval = interval;
								base.intervalType = intervalType;
								base.intervalOffset = intervalOffset;
								base.intervalOffsetType = intervalOffsetType;
							}
						}
					}
				}
			}
		}

		private double GetLogMinimum(double current, Series axisSeries)
		{
			double num = base.axis.GetViewMinimum();
			DateTimeIntervalType type = (base.IntervalOffsetType == DateTimeIntervalType.Auto) ? base.IntervalType : base.IntervalOffsetType;
			if (base.IntervalOffset != 0.0 && !double.IsNaN(base.IntervalOffset) && axisSeries == null)
			{
				num += base.axis.GetIntervalSize(num, base.IntervalOffset, type, axisSeries, 0.0, DateTimeIntervalType.Number, true, false);
			}
			return num + Math.Floor(current - num);
		}

		internal void PaintCustom(ChartGraphics graph, bool backElements)
		{
			PointF empty = PointF.Empty;
			PointF empty2 = PointF.Empty;
			base.axis.PlotAreaPosition.ToRectangleF();
			float num = 0f;
			if (base.axis.ScrollBar.IsVisible() && base.axis.ScrollBar.PositionInside && base.axis.IsAxisOnAreaEdge())
			{
				num = (float)base.axis.ScrollBar.GetScrollBarRelativeSize();
			}
			if (base.axis.AxisPosition == AxisPosition.Left)
			{
				float num2 = (!base.axis.IsMarksNextToAxis()) ? base.axis.PlotAreaPosition.X : ((float)base.axis.GetAxisPosition());
				if (this.style == TickMarkStyle.Inside)
				{
					empty.X = num2;
					empty2.X = num2 + this.size;
				}
				else if (this.style == TickMarkStyle.Outside)
				{
					empty.X = num2 - this.size - num;
					empty2.X = num2;
				}
				else if (this.style == TickMarkStyle.Cross)
				{
					empty.X = (float)(num2 - this.size / 2.0 - num);
					empty2.X = (float)(num2 + this.size / 2.0);
				}
			}
			else if (base.axis.AxisPosition == AxisPosition.Right)
			{
				float num2 = (!base.axis.IsMarksNextToAxis()) ? base.axis.PlotAreaPosition.Right() : ((float)base.axis.GetAxisPosition());
				if (this.style == TickMarkStyle.Inside)
				{
					empty.X = num2 - this.size;
					empty2.X = num2;
				}
				else if (this.style == TickMarkStyle.Outside)
				{
					empty.X = num2;
					empty2.X = num2 + this.size + num;
				}
				else if (this.style == TickMarkStyle.Cross)
				{
					empty.X = (float)(num2 - this.size / 2.0);
					empty2.X = (float)(num2 + this.size / 2.0 + num);
				}
			}
			else if (base.axis.AxisPosition == AxisPosition.Top)
			{
				float num2 = (!base.axis.IsMarksNextToAxis()) ? base.axis.PlotAreaPosition.Y : ((float)base.axis.GetAxisPosition());
				if (this.style == TickMarkStyle.Inside)
				{
					empty.Y = num2;
					empty2.Y = num2 + this.size;
				}
				else if (this.style == TickMarkStyle.Outside)
				{
					empty.Y = num2 - this.size - num;
					empty2.Y = num2;
				}
				else if (this.style == TickMarkStyle.Cross)
				{
					empty.Y = (float)(num2 - this.size / 2.0 - num);
					empty2.Y = (float)(num2 + this.size / 2.0);
				}
			}
			else if (base.axis.AxisPosition == AxisPosition.Bottom)
			{
				float num2 = (!base.axis.IsMarksNextToAxis()) ? base.axis.PlotAreaPosition.Bottom() : ((float)base.axis.GetAxisPosition());
				if (this.style == TickMarkStyle.Inside)
				{
					empty.Y = num2 - this.size;
					empty2.Y = num2;
				}
				else if (this.style == TickMarkStyle.Outside)
				{
					empty.Y = num2;
					empty2.Y = num2 + this.size + num;
				}
				else if (this.style == TickMarkStyle.Cross)
				{
					empty.Y = (float)(num2 - this.size / 2.0);
					empty2.Y = (float)(num2 + this.size / 2.0 + num);
				}
			}
			foreach (CustomLabel customLabel in base.axis.CustomLabels)
			{
				if ((customLabel.GridTicks & GridTicks.TickMark) == GridTicks.TickMark)
				{
					double num3 = (customLabel.To + customLabel.From) / 2.0;
					if (num3 >= base.axis.GetViewMinimum() && num3 <= base.axis.GetViewMaximum())
					{
						if (base.axis.AxisPosition == AxisPosition.Left)
						{
							empty.Y = (float)base.axis.GetLinearPosition(num3);
							empty2.Y = empty.Y;
						}
						else if (base.axis.AxisPosition == AxisPosition.Right)
						{
							empty.Y = (float)base.axis.GetLinearPosition(num3);
							empty2.Y = empty.Y;
						}
						else if (base.axis.AxisPosition == AxisPosition.Top)
						{
							empty.X = (float)base.axis.GetLinearPosition(num3);
							empty2.X = empty.X;
						}
						else if (base.axis.AxisPosition == AxisPosition.Bottom)
						{
							empty.X = (float)base.axis.GetLinearPosition(num3);
							empty2.X = empty.X;
						}
						if (base.axis.Common.ProcessModeRegions)
						{
							if (!base.axis.chartArea.Area3DStyle.Enable3D || base.axis.chartArea.chartAreaIsCurcular)
							{
								RectangleF rectArea = new RectangleF((float)(empty.X - 0.5), (float)(empty.Y - 0.5), (float)(Math.Abs(empty2.X - empty.X) + 1.0), (float)(Math.Abs(empty2.Y - empty.Y) + 1.0));
								base.axis.Common.HotRegionsList.AddHotRegion(rectArea, this, ChartElementType.TickMarks, true);
							}
							else
							{
								this.Draw3DTickLine(graph, empty, empty2, base.axis.AxisPosition == AxisPosition.Left || base.axis.AxisPosition == AxisPosition.Right, num, backElements);
							}
						}
						if (base.axis.Common.ProcessModePaint)
						{
							if (!base.axis.chartArea.Area3DStyle.Enable3D || base.axis.chartArea.chartAreaIsCurcular)
							{
								graph.DrawLineRel(base.borderColor, base.borderWidth, base.borderStyle, empty, empty2);
							}
							else
							{
								this.Draw3DTickLine(graph, empty, empty2, base.axis.AxisPosition == AxisPosition.Left || base.axis.AxisPosition == AxisPosition.Right, num, backElements);
							}
						}
					}
				}
			}
		}

		internal void Draw3DTickLine(ChartGraphics graph, PointF point1, PointF point2, bool horizontal, float scrollBarSize, bool backElements)
		{
			this.Draw3DTickLine(graph, point1, point2, horizontal, scrollBarSize, backElements, false);
		}

		internal void Draw3DTickLine(ChartGraphics graph, PointF point1, PointF point2, bool horizontal, float scrollBarSize, bool backElements, bool selectionMode)
		{
			ChartArea chartArea = base.axis.chartArea;
			bool flag = default(bool);
			float marksZPosition = base.axis.GetMarksZPosition(out flag);
			bool flag2 = flag;
			if ((!flag2 || base.axis.MajorTickMark.Style != TickMarkStyle.Cross) && base.axis.MajorTickMark.Style != TickMarkStyle.Inside && base.axis.MinorTickMark.Style != TickMarkStyle.Cross && base.axis.MinorTickMark.Style != TickMarkStyle.Inside)
			{
				goto IL_006d;
			}
			flag2 = false;
			goto IL_006d;
			IL_006d:
			SurfaceNames surfaceName = (SurfaceNames)((marksZPosition != 0.0) ? 1 : 2);
			if (chartArea.ShouldDrawOnSurface(surfaceName, backElements, flag2))
			{
				if (base.axis.AxisPosition == AxisPosition.Bottom && (!base.axis.IsMarksNextToAxis() || flag) && chartArea.IsBottomSceneWallVisible())
				{
					point2.Y += chartArea.areaSceneWallWidth.Height;
				}
				else if (base.axis.AxisPosition == AxisPosition.Left && (!base.axis.IsMarksNextToAxis() || flag) && chartArea.IsSideSceneWallOnLeft())
				{
					point1.X -= chartArea.areaSceneWallWidth.Width;
				}
				else if (base.axis.AxisPosition == AxisPosition.Right && (!base.axis.IsMarksNextToAxis() || flag) && !chartArea.IsSideSceneWallOnLeft())
				{
					point2.X += chartArea.areaSceneWallWidth.Width;
				}
				else if (base.axis.AxisPosition == AxisPosition.Top && (!base.axis.IsMarksNextToAxis() || flag))
				{
					point1.Y -= chartArea.areaSceneWallWidth.Height;
				}
				Point3D point3D = null;
				Point3D point3D2 = null;
				if (flag && chartArea.areaSceneWallWidth.Width != 0.0)
				{
					if (base.axis.AxisPosition == AxisPosition.Top)
					{
						float y = base.axis.PlotAreaPosition.Y;
						if (this.style == TickMarkStyle.Inside)
						{
							point1.Y = y;
							point2.Y = y + this.size;
							point3D = new Point3D(point1.X, point1.Y, (float)(0.0 - chartArea.areaSceneWallWidth.Width));
							point3D2 = new Point3D(point1.X, point1.Y, 0f);
						}
						else if (this.style == TickMarkStyle.Outside)
						{
							point1.Y = y;
							point2.Y = y;
							point3D = new Point3D(point1.X, y, marksZPosition);
							point3D2 = new Point3D(point1.X, point1.Y, (float)(0.0 - this.size - chartArea.areaSceneWallWidth.Width));
						}
						else if (this.style == TickMarkStyle.Cross)
						{
							point1.Y = y;
							point2.Y = (float)(y + this.size / 2.0);
							point3D = new Point3D(point1.X, y, marksZPosition);
							point3D2 = new Point3D(point1.X, point1.Y, (float)((0.0 - this.size) / 2.0 - chartArea.areaSceneWallWidth.Width));
						}
						if (chartArea.ShouldDrawOnSurface(SurfaceNames.Top, backElements, false))
						{
							point3D = null;
							point3D2 = null;
						}
					}
					if (base.axis.AxisPosition == AxisPosition.Left && !chartArea.IsSideSceneWallOnLeft())
					{
						float x = base.axis.PlotAreaPosition.X;
						if (this.style == TickMarkStyle.Inside)
						{
							point1.X = x;
							point2.X = x + this.size;
							point3D = new Point3D(point1.X, point1.Y, (float)(0.0 - chartArea.areaSceneWallWidth.Width));
							point3D2 = new Point3D(point1.X, point1.Y, 0f);
						}
						else if (this.style == TickMarkStyle.Outside)
						{
							point1.X = x;
							point2.X = x;
							point3D = new Point3D(x, point1.Y, marksZPosition);
							point3D2 = new Point3D(x, point1.Y, (float)(0.0 - this.size - chartArea.areaSceneWallWidth.Width));
						}
						else if (this.style == TickMarkStyle.Cross)
						{
							point1.X = x;
							point2.X = (float)(x + this.size / 2.0);
							point3D = new Point3D(x, point1.Y, marksZPosition);
							point3D2 = new Point3D(x, point1.Y, (float)((0.0 - this.size) / 2.0 - chartArea.areaSceneWallWidth.Width));
						}
						if (chartArea.ShouldDrawOnSurface(SurfaceNames.Left, backElements, false))
						{
							point3D = null;
							point3D2 = null;
						}
					}
					else if (base.axis.AxisPosition == AxisPosition.Right && chartArea.IsSideSceneWallOnLeft())
					{
						float num = base.axis.PlotAreaPosition.Right();
						if (this.style == TickMarkStyle.Inside)
						{
							point1.X = num - this.size;
							point2.X = num;
							point3D = new Point3D(point2.X, point2.Y, (float)(0.0 - chartArea.areaSceneWallWidth.Width));
							point3D2 = new Point3D(point2.X, point2.Y, 0f);
						}
						else if (this.style == TickMarkStyle.Outside)
						{
							point1.X = num;
							point2.X = num;
							point3D = new Point3D(num, point1.Y, marksZPosition);
							point3D2 = new Point3D(num, point1.Y, (float)(0.0 - this.size - chartArea.areaSceneWallWidth.Width));
						}
						else if (this.style == TickMarkStyle.Cross)
						{
							point1.X = (float)(num - this.size / 2.0);
							point2.X = num;
							point3D = new Point3D(num, point1.Y, marksZPosition);
							point3D2 = new Point3D(num, point1.Y, (float)((0.0 - this.size) / 2.0 - chartArea.areaSceneWallWidth.Width));
						}
						if (chartArea.ShouldDrawOnSurface(SurfaceNames.Right, backElements, false))
						{
							point3D = null;
							point3D2 = null;
						}
					}
				}
				graph.Draw3DLine(chartArea.matrix3D, base.borderColor, base.borderWidth, base.borderStyle, new Point3D(point1.X, point1.Y, marksZPosition), new Point3D(point2.X, point2.Y, marksZPosition), base.axis.Common, this, ChartElementType.TickMarks);
				if (point3D != null && point3D2 != null)
				{
					graph.Draw3DLine(chartArea.matrix3D, base.borderColor, base.borderWidth, base.borderStyle, point3D, point3D2, base.axis.Common, this, ChartElementType.TickMarks);
				}
			}
		}
	}
}
