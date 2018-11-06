using AspNetCore.Reporting.Chart.WebForms.Design;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeAxisScaleSegment_AxisScaleSegment")]
	internal class AxisScaleSegment
	{
		internal Axis axis;

		private double position;

		private double size;

		private double spacing;

		private double scaleMinimum;

		private double scaleMaximum;

		private double intervalOffset;

		private double interval;

		private DateTimeIntervalType intervalType;

		private DateTimeIntervalType intervalOffsetType;

		private object tag;

		private Stack oldAxisSettings = new Stack();

		[DefaultValue(0.0)]
		[SRCategory("CategoryAttributeMisc")]
		[SRDescription("DescriptionAttributeAxisScaleSegment_Position")]
		public double Position
		{
			get
			{
				return this.position;
			}
			set
			{
				if (!(value < 0.0) && !(value > 100.0))
				{
					this.position = value;
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionAxisScaleSegmentsPositionInvalid);
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeAxisScaleSegment_Size")]
		public double Size
		{
			get
			{
				return this.size;
			}
			set
			{
				if (!(value < 0.0) && !(value > 100.0))
				{
					this.size = value;
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionAxisScaleSegmentsSizeInvalid);
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeAxisScaleSegment_Spacing")]
		public double Spacing
		{
			get
			{
				return this.spacing;
			}
			set
			{
				if (!(value < 0.0) && !(value > 100.0))
				{
					this.spacing = value;
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionAxisScaleSegmentsSpacingInvalid);
			}
		}

		[DefaultValue(0.0)]
		[SRCategory("CategoryAttributeMisc")]
		[SRDescription("DescriptionAttributeAxisScaleSegment_ScaleMaximum")]
		public double ScaleMaximum
		{
			get
			{
				return this.scaleMaximum;
			}
			set
			{
				this.scaleMaximum = value;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeAxisScaleSegment_ScaleMinimum")]
		public double ScaleMinimum
		{
			get
			{
				return this.scaleMinimum;
			}
			set
			{
				this.scaleMinimum = value;
			}
		}

		[DefaultValue(0.0)]
		[SRCategory("CategoryAttributeInterval")]
		[SRDescription("DescriptionAttributeAxisScaleSegment_Interval")]
		[TypeConverter(typeof(AxisIntervalValueConverter))]
		public double Interval
		{
			get
			{
				return this.interval;
			}
			set
			{
				if (double.IsNaN(value))
				{
					this.interval = 0.0;
				}
				else
				{
					this.interval = value;
				}
			}
		}

		[SRCategory("CategoryAttributeInterval")]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeAxisScaleSegment_IntervalOffset")]
		[TypeConverter(typeof(AxisIntervalValueConverter))]
		public double IntervalOffset
		{
			get
			{
				return this.intervalOffset;
			}
			set
			{
				if (double.IsNaN(value))
				{
					this.intervalOffset = 0.0;
				}
				else
				{
					this.intervalOffset = value;
				}
			}
		}

		[SRDescription("DescriptionAttributeAxisScaleSegment_IntervalType")]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRCategory("CategoryAttributeInterval")]
		public DateTimeIntervalType IntervalType
		{
			get
			{
				return this.intervalType;
			}
			set
			{
				if (value == DateTimeIntervalType.NotSet)
				{
					this.intervalType = DateTimeIntervalType.Auto;
				}
				else
				{
					this.intervalType = value;
				}
			}
		}

		[SRCategory("CategoryAttributeInterval")]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeAxisScaleSegment_IntervalOffsetType")]
		public DateTimeIntervalType IntervalOffsetType
		{
			get
			{
				return this.intervalOffsetType;
			}
			set
			{
				if (value == DateTimeIntervalType.NotSet)
				{
					this.intervalOffsetType = DateTimeIntervalType.Auto;
				}
				else
				{
					this.intervalOffsetType = value;
				}
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Browsable(false)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeAxisScaleSegment_Tag")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public object Tag
		{
			get
			{
				return this.tag;
			}
			set
			{
				this.tag = value;
			}
		}

		internal void PaintBreakLine(ChartGraphics graph, AxisScaleSegment nextSegment)
		{
			RectangleF breakLinePosition = this.GetBreakLinePosition(graph, nextSegment);
			GraphicsPath breakLinePath = this.GetBreakLinePath(breakLinePosition, true);
			GraphicsPath graphicsPath = null;
			if (breakLinePosition.Width > 0.0 && breakLinePosition.Height > 0.0)
			{
				graphicsPath = this.GetBreakLinePath(breakLinePosition, false);
				using (GraphicsPath graphicsPath2 = new GraphicsPath())
				{
					graphicsPath2.AddPath(breakLinePath, true);
					graphicsPath2.Reverse();
					graphicsPath2.AddPath(graphicsPath, true);
					graphicsPath2.CloseAllFigures();
					using (Brush brush = this.GetChartFillBrush(graph))
					{
						graph.FillPath(brush, graphicsPath2);
						if (this.axis.chartArea.ShadowOffset != 0 && !this.axis.chartArea.ShadowColor.IsEmpty)
						{
							RectangleF rect = breakLinePosition;
							if (this.axis.AxisPosition == AxisPosition.Right || this.axis.AxisPosition == AxisPosition.Left)
							{
								rect.Y += (float)this.axis.chartArea.ShadowOffset;
								rect.Height -= (float)this.axis.chartArea.ShadowOffset;
								rect.X = (float)(rect.Right - 1.0);
								rect.Width = (float)(this.axis.chartArea.ShadowOffset + 2);
							}
							else
							{
								rect.X += (float)this.axis.chartArea.ShadowOffset;
								rect.Width -= (float)this.axis.chartArea.ShadowOffset;
								rect.Y = (float)(rect.Bottom - 1.0);
								rect.Height = (float)(this.axis.chartArea.ShadowOffset + 2);
							}
							graph.FillRectangle(brush, rect);
							using (GraphicsPath graphicsPath3 = new GraphicsPath())
							{
								graphicsPath3.AddPath(breakLinePath, false);
								float val = (float)this.axis.chartArea.ShadowOffset;
								val = ((this.axis.AxisPosition != AxisPosition.Right && this.axis.AxisPosition != 0) ? Math.Min(val, breakLinePosition.Width) : Math.Min(val, breakLinePosition.Height));
								int num = (int)((float)(int)this.axis.chartArea.ShadowColor.A / val);
								RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(this.axis.PlotAreaPosition.ToRectangleF());
								if (this.axis.AxisPosition == AxisPosition.Right || this.axis.AxisPosition == AxisPosition.Left)
								{
									absoluteRectangle.X += (float)this.axis.chartArea.ShadowOffset;
									absoluteRectangle.Width += (float)this.axis.chartArea.ShadowOffset;
								}
								else
								{
									absoluteRectangle.Y += (float)this.axis.chartArea.ShadowOffset;
									absoluteRectangle.Height += (float)this.axis.chartArea.ShadowOffset;
								}
								graph.SetClip(graph.GetRelativeRectangle(absoluteRectangle));
								for (int i = 0; (float)i < val; i++)
								{
									using (Matrix matrix = new Matrix())
									{
										if (this.axis.AxisPosition == AxisPosition.Right || this.axis.AxisPosition == AxisPosition.Left)
										{
											matrix.Translate(0f, 1f);
										}
										else
										{
											matrix.Translate(1f, 0f);
										}
										graphicsPath3.Transform(matrix);
									}
									Color color = Color.FromArgb(this.axis.chartArea.ShadowColor.A - num * i, this.axis.chartArea.ShadowColor);
									using (Pen pen = new Pen(color, 1f))
									{
										graph.DrawPath(pen, graphicsPath3);
									}
								}
								graph.ResetClip();
							}
						}
					}
				}
			}
			if (this.axis.ScaleBreakStyle.BreakLineType != 0)
			{
				using (Pen pen2 = new Pen(this.axis.ScaleBreakStyle.LineColor, (float)this.axis.ScaleBreakStyle.LineWidth))
				{
					pen2.DashStyle = graph.GetPenStyle(this.axis.ScaleBreakStyle.LineStyle);
					graph.DrawPath(pen2, breakLinePath);
					if (breakLinePosition.Width > 0.0 && breakLinePosition.Height > 0.0)
					{
						graph.DrawPath(pen2, graphicsPath);
					}
				}
			}
			breakLinePath.Dispose();
			breakLinePath = null;
			if (graphicsPath != null)
			{
				graphicsPath.Dispose();
				graphicsPath = null;
			}
		}

		private Brush GetChartFillBrush(ChartGraphics graph)
		{
			Chart chart = this.axis.chartArea.Common.Chart;
			Brush brush = null;
			brush = ((chart.BackGradientType != 0) ? graph.GetGradientBrush(new RectangleF(0f, 0f, (float)(chart.chartPicture.Width - 1), (float)(chart.chartPicture.Height - 1)), chart.BackColor, chart.BackGradientEndColor, chart.BackGradientType) : new SolidBrush(chart.BackColor));
			if (chart.BackHatchStyle != 0)
			{
				brush = graph.GetHatchBrush(chart.BackHatchStyle, chart.BackColor, chart.BackGradientEndColor);
			}
			if (chart.BackImage.Length > 0 && chart.BackImageMode != ChartImageWrapMode.Unscaled && chart.BackImageMode != ChartImageWrapMode.Scaled)
			{
				brush = graph.GetTextureBrush(chart.BackImage, chart.BackImageTransparentColor, chart.BackImageMode, chart.BackColor);
			}
			return brush;
		}

		private GraphicsPath GetBreakLinePath(RectangleF breakLinePosition, bool top)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			if (this.axis.ScaleBreakStyle.BreakLineType == BreakLineType.Wave)
			{
				PointF[] array = null;
				int num = 0;
				if (this.axis.AxisPosition == AxisPosition.Right || this.axis.AxisPosition == AxisPosition.Left)
				{
					float x = breakLinePosition.X;
					float right = breakLinePosition.Right;
					float num2 = top ? breakLinePosition.Y : breakLinePosition.Bottom;
					num = (int)(right - x) / 40 * 2;
					if (num < 2)
					{
						num = 2;
					}
					float num3 = (right - x) / (float)num;
					array = new PointF[num + 1];
					for (int i = 1; i < num + 1; i++)
					{
						array[i] = new PointF(x + (float)i * num3, (float)(num2 + ((i % 2 == 0) ? -2.0 : 2.0)));
					}
					array[0] = new PointF(x, num2);
					array[array.Length - 1] = new PointF(right, num2);
				}
				else
				{
					float y = breakLinePosition.Y;
					float bottom = breakLinePosition.Bottom;
					float num4 = top ? breakLinePosition.X : breakLinePosition.Right;
					num = (int)(bottom - y) / 40 * 2;
					if (num < 2)
					{
						num = 2;
					}
					float num5 = (bottom - y) / (float)num;
					array = new PointF[num + 1];
					for (int j = 1; j < num + 1; j++)
					{
						array[j] = new PointF((float)(num4 + ((j % 2 == 0) ? -2.0 : 2.0)), y + (float)j * num5);
					}
					array[0] = new PointF(num4, y);
					array[array.Length - 1] = new PointF(num4, bottom);
				}
				graphicsPath.AddCurve(array, 0, num, 0.8f);
			}
			else if (this.axis.ScaleBreakStyle.BreakLineType == BreakLineType.Ragged)
			{
				PointF[] array2 = null;
				Random random = new Random(435657);
				if (this.axis.AxisPosition == AxisPosition.Right || this.axis.AxisPosition == AxisPosition.Left)
				{
					float x2 = breakLinePosition.X;
					float right2 = breakLinePosition.Right;
					float num6 = top ? breakLinePosition.Y : breakLinePosition.Bottom;
					float num7 = 10f;
					int num8 = (int)((right2 - x2) / num7);
					if (num8 < 2)
					{
						num8 = 2;
					}
					array2 = new PointF[num8];
					for (int k = 1; k < num8 - 1; k++)
					{
						array2[k] = new PointF(x2 + (float)k * num7, num6 + (float)random.Next(-3, 3));
					}
					array2[0] = new PointF(x2, num6);
					array2[array2.Length - 1] = new PointF(right2, num6);
				}
				else
				{
					float y2 = breakLinePosition.Y;
					float bottom2 = breakLinePosition.Bottom;
					float num9 = top ? breakLinePosition.X : breakLinePosition.Right;
					float num10 = 10f;
					int num11 = (int)((bottom2 - y2) / num10);
					if (num11 < 2)
					{
						num11 = 2;
					}
					array2 = new PointF[num11];
					for (int l = 1; l < num11 - 1; l++)
					{
						array2[l] = new PointF(num9 + (float)random.Next(-3, 3), y2 + (float)l * num10);
					}
					array2[0] = new PointF(num9, y2);
					array2[array2.Length - 1] = new PointF(num9, bottom2);
				}
				graphicsPath.AddLines(array2);
			}
			else if (this.axis.AxisPosition == AxisPosition.Right || this.axis.AxisPosition == AxisPosition.Left)
			{
				if (top)
				{
					graphicsPath.AddLine(breakLinePosition.X, breakLinePosition.Y, breakLinePosition.Right, breakLinePosition.Y);
				}
				else
				{
					graphicsPath.AddLine(breakLinePosition.X, breakLinePosition.Bottom, breakLinePosition.Right, breakLinePosition.Bottom);
				}
			}
			else if (top)
			{
				graphicsPath.AddLine(breakLinePosition.X, breakLinePosition.Y, breakLinePosition.X, breakLinePosition.Bottom);
			}
			else
			{
				graphicsPath.AddLine(breakLinePosition.Right, breakLinePosition.Y, breakLinePosition.Right, breakLinePosition.Bottom);
			}
			return graphicsPath;
		}

		internal RectangleF GetBreakLinePosition(ChartGraphics graph, AxisScaleSegment nextSegment)
		{
			RectangleF rectangleF = this.axis.PlotAreaPosition.ToRectangleF();
			double linearPosition = this.axis.GetLinearPosition(nextSegment.ScaleMinimum);
			double linearPosition2 = this.axis.GetLinearPosition(this.ScaleMaximum);
			if (this.axis.AxisPosition == AxisPosition.Right || this.axis.AxisPosition == AxisPosition.Left)
			{
				rectangleF.Y = (float)Math.Min(linearPosition, linearPosition2);
				rectangleF.Height = (float)Math.Max(linearPosition, linearPosition2);
			}
			else
			{
				rectangleF.X = (float)Math.Min(linearPosition, linearPosition2);
				rectangleF.Width = (float)Math.Max(linearPosition, linearPosition2);
			}
			rectangleF = Rectangle.Round(graph.GetAbsoluteRectangle(rectangleF));
			if (this.axis.AxisPosition == AxisPosition.Right || this.axis.AxisPosition == AxisPosition.Left)
			{
				rectangleF.Height = Math.Abs(rectangleF.Y - rectangleF.Height);
				rectangleF.X -= (float)this.axis.chartArea.BorderWidth;
				rectangleF.Width += (float)(2 * this.axis.chartArea.BorderWidth);
			}
			else
			{
				rectangleF.Width = Math.Abs(rectangleF.X - rectangleF.Width);
				rectangleF.Y -= (float)this.axis.chartArea.BorderWidth;
				rectangleF.Height += (float)(2 * this.axis.chartArea.BorderWidth);
			}
			return rectangleF;
		}

		internal void GetScalePositionAndSize(double plotAreaSize, out double scalePosition, out double scaleSize)
		{
			scaleSize = (this.Size - this.Spacing) * (plotAreaSize / 100.0);
			scalePosition = this.Position * (plotAreaSize / 100.0);
		}

		internal void SetTempAxisScaleAndInterval()
		{
			if (this.oldAxisSettings.Count == 0)
			{
				this.oldAxisSettings.Push(this.axis.maximum);
				this.oldAxisSettings.Push(this.axis.minimum);
				this.oldAxisSettings.Push(this.axis.majorGrid.interval);
				this.oldAxisSettings.Push(this.axis.majorGrid.intervalType);
				this.oldAxisSettings.Push(this.axis.majorGrid.intervalOffset);
				this.oldAxisSettings.Push(this.axis.majorGrid.intervalOffsetType);
				this.oldAxisSettings.Push(this.axis.majorTickMark.interval);
				this.oldAxisSettings.Push(this.axis.majorTickMark.intervalType);
				this.oldAxisSettings.Push(this.axis.majorTickMark.intervalOffset);
				this.oldAxisSettings.Push(this.axis.majorTickMark.intervalOffsetType);
				this.oldAxisSettings.Push(this.axis.LabelStyle.interval);
				this.oldAxisSettings.Push(this.axis.LabelStyle.intervalType);
				this.oldAxisSettings.Push(this.axis.LabelStyle.intervalOffset);
				this.oldAxisSettings.Push(this.axis.LabelStyle.intervalOffsetType);
			}
			this.axis.maximum = this.ScaleMaximum;
			this.axis.minimum = this.ScaleMinimum;
			this.axis.majorGrid.interval = this.Interval;
			this.axis.majorGrid.intervalType = this.IntervalType;
			this.axis.majorGrid.intervalOffset = this.IntervalOffset;
			this.axis.majorGrid.intervalOffsetType = this.IntervalOffsetType;
			this.axis.majorTickMark.interval = this.Interval;
			this.axis.majorTickMark.intervalType = this.IntervalType;
			this.axis.majorTickMark.intervalOffset = this.IntervalOffset;
			this.axis.majorTickMark.intervalOffsetType = this.IntervalOffsetType;
			this.axis.LabelStyle.interval = this.Interval;
			this.axis.LabelStyle.intervalType = this.IntervalType;
			this.axis.LabelStyle.intervalOffset = this.IntervalOffset;
			this.axis.LabelStyle.intervalOffsetType = this.IntervalOffsetType;
		}

		internal void RestoreAxisScaleAndInterval()
		{
			if (this.oldAxisSettings.Count > 0)
			{
				this.axis.LabelStyle.intervalOffsetType = (DateTimeIntervalType)this.oldAxisSettings.Pop();
				this.axis.LabelStyle.intervalOffset = (double)this.oldAxisSettings.Pop();
				this.axis.LabelStyle.intervalType = (DateTimeIntervalType)this.oldAxisSettings.Pop();
				this.axis.LabelStyle.interval = (double)this.oldAxisSettings.Pop();
				this.axis.majorTickMark.intervalOffsetType = (DateTimeIntervalType)this.oldAxisSettings.Pop();
				this.axis.majorTickMark.intervalOffset = (double)this.oldAxisSettings.Pop();
				this.axis.majorTickMark.intervalType = (DateTimeIntervalType)this.oldAxisSettings.Pop();
				this.axis.majorTickMark.interval = (double)this.oldAxisSettings.Pop();
				this.axis.majorGrid.intervalOffsetType = (DateTimeIntervalType)this.oldAxisSettings.Pop();
				this.axis.majorGrid.intervalOffset = (double)this.oldAxisSettings.Pop();
				this.axis.majorGrid.intervalType = (DateTimeIntervalType)this.oldAxisSettings.Pop();
				this.axis.majorGrid.interval = (double)this.oldAxisSettings.Pop();
				this.axis.minimum = (double)this.oldAxisSettings.Pop();
				this.axis.maximum = (double)this.oldAxisSettings.Pop();
			}
		}
	}
}
