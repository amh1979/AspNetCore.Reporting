using AspNetCore.Reporting.Chart.WebForms.ChartTypes;
using AspNetCore.Reporting.Chart.WebForms.Design;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[DefaultProperty("Enabled")]
	[SRDescription("DescriptionAttributeLabel_Label")]
	internal class Label : ChartElement
	{
		internal Axis axis;

		private bool enabled = true;

		internal double intervalOffset = double.NaN;

		internal double interval = double.NaN;

		internal DateTimeIntervalType intervalType = DateTimeIntervalType.NotSet;

		internal DateTimeIntervalType intervalOffsetType = DateTimeIntervalType.NotSet;

		internal Font font = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);

		private Color fontColor = Color.Black;

		internal int fontAngle;

		internal bool offsetLabels;

		private bool showEndLabels = true;

		private bool truncatedLabels;

		private string format = "";

		[DefaultValue(double.NaN)]
		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLabel_IntervalOffset")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(AxisElementIntervalValueConverter))]
		public double IntervalOffset
		{
			get
			{
				if (double.IsNaN(this.intervalOffset) && this.axis != null)
				{
					if (this.axis.IsSerializing())
					{
						return double.NaN;
					}
					return this.axis.IntervalOffset;
				}
				return this.intervalOffset;
			}
			set
			{
				this.intervalOffset = value;
				this.Invalidate();
			}
		}

		[DefaultValue(DateTimeIntervalType.NotSet)]
		[Bindable(true)]
		[SRCategory("CategoryAttributeData")]
		[SRDescription("DescriptionAttributeLabel_IntervalOffsetType")]
		[RefreshProperties(RefreshProperties.All)]
		public DateTimeIntervalType IntervalOffsetType
		{
			get
			{
				if (this.intervalOffsetType == DateTimeIntervalType.NotSet && this.axis != null)
				{
					if (this.axis.IsSerializing())
					{
						return DateTimeIntervalType.NotSet;
					}
					return this.axis.IntervalOffsetType;
				}
				return this.intervalOffsetType;
			}
			set
			{
				this.intervalOffsetType = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeLabel_Interval")]
		[TypeConverter(typeof(AxisElementIntervalValueConverter))]
		public double Interval
		{
			get
			{
				if (double.IsNaN(this.interval) && this.axis != null)
				{
					if (this.axis.IsSerializing())
					{
						return double.NaN;
					}
					return this.axis.Interval;
				}
				return this.interval;
			}
			set
			{
				this.interval = value;
				if (this.axis != null)
				{
					this.axis.tempLabelInterval = this.interval;
				}
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLabel_IntervalType")]
		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.NotSet)]
		[RefreshProperties(RefreshProperties.All)]
		public DateTimeIntervalType IntervalType
		{
			get
			{
				if (this.intervalType == DateTimeIntervalType.NotSet && this.axis != null)
				{
					if (this.axis.IsSerializing())
					{
						return DateTimeIntervalType.NotSet;
					}
					return this.axis.IntervalType;
				}
				return this.intervalType;
			}
			set
			{
				this.intervalType = value;
				if (this.axis != null)
				{
					this.axis.tempLabelIntervalType = this.intervalType;
				}
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeLabel_Font")]
		public Font Font
		{
			get
			{
				return this.font;
			}
			set
			{
				if (this.axis != null && this.axis.chart != null && !this.axis.chart.serializing)
				{
					this.axis.LabelsAutoFit = false;
				}
				this.font = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "Black")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLabel_FontColor")]
		[NotifyParentProperty(true)]
		public Color FontColor
		{
			get
			{
				return this.fontColor;
			}
			set
			{
				this.fontColor = value;
				this.Invalidate();
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeLabel_FontAngle")]
		public int FontAngle
		{
			get
			{
				return this.fontAngle;
			}
			set
			{
				if (value >= -90 && value <= 90)
				{
					if (this.OffsetLabels && value != 0 && value != -90 && value != 90)
					{
						this.OffsetLabels = false;
					}
					if (this.axis != null && this.axis.chart != null && !this.axis.chart.serializing)
					{
						this.axis.LabelsAutoFit = false;
					}
					this.fontAngle = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionAxisLabelFontAngleInvalid);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[RefreshProperties(RefreshProperties.All)]
		[Bindable(true)]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeLabel_OffsetLabels")]
		public bool OffsetLabels
		{
			get
			{
				return this.offsetLabels;
			}
			set
			{
				if (value && (this.FontAngle != 0 || this.FontAngle != -90 || this.FontAngle != 90))
				{
					this.FontAngle = 0;
				}
				if (this.axis != null && this.axis.chart != null && !this.axis.chart.serializing)
				{
					this.axis.LabelsAutoFit = false;
				}
				this.offsetLabels = value;
				this.Invalidate();
			}
		}

		[DefaultValue(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLabel_ShowEndLabels")]
		public bool ShowEndLabels
		{
			get
			{
				return this.showEndLabels;
			}
			set
			{
				this.showEndLabels = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeLabel_TruncatedLabels")]
		public bool TruncatedLabels
		{
			get
			{
				return this.truncatedLabels;
			}
			set
			{
				this.truncatedLabels = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeLabel_Format")]
		public string Format
		{
			get
			{
				return this.format;
			}
			set
			{
				this.format = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeLabel_Enabled")]
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
				this.Invalidate();
			}
		}

		public Label()
		{
			this.axis = null;
		}

		public Label(Axis axis)
		{
			this.axis = axis;
		}

		internal void PaintCircular(ChartGraphics graph)
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
			if (this.axis.LabelStyle.Enabled)
			{
				CircularAxisLabelsStyle circularAxisLabelsStyle = this.axis.chartArea.GetCircularAxisLabelsStyle();
				ArrayList circularAxisList = this.axis.chartArea.GetCircularAxisList();
				int num = 0;
				foreach (CircularChartAreaAxis item in circularAxisList)
				{
					if (item.Title.Length > 0)
					{
						PointF relative = new PointF(this.axis.chartArea.circularCenter.X, this.axis.chartArea.PlotAreaPosition.Y);
						relative.Y -= (float)(this.axis.markSize + 1.0);
						PointF[] array = new PointF[1]
						{
							graph.GetAbsolutePoint(relative)
						};
						float num2 = item.AxisPosition;
						ICircularChartType circularChartType = this.axis.chartArea.GetCircularChartType();
						if (circularChartType != null && circularChartType.XAxisCrossingSupported() && !double.IsNaN(this.axis.chartArea.AxisX.Crossing))
						{
							num2 += (float)this.axis.chartArea.AxisX.Crossing;
						}
						while (num2 < 0.0)
						{
							num2 = (float)(360.0 + num2);
						}
						Matrix matrix = new Matrix();
						matrix.RotateAt(num2, graph.GetAbsolutePoint(this.axis.chartArea.circularCenter));
						matrix.TransformPoints(array);
						stringFormat.LineAlignment = StringAlignment.Center;
						stringFormat.Alignment = StringAlignment.Near;
						if (circularAxisLabelsStyle != CircularAxisLabelsStyle.Radial)
						{
							if (num2 < 5.0 || num2 > 355.0)
							{
								stringFormat.Alignment = StringAlignment.Center;
								stringFormat.LineAlignment = StringAlignment.Far;
							}
							if (num2 < 185.0 && num2 > 175.0)
							{
								stringFormat.Alignment = StringAlignment.Center;
								stringFormat.LineAlignment = StringAlignment.Near;
							}
							if (num2 > 185.0 && num2 < 355.0)
							{
								stringFormat.Alignment = StringAlignment.Far;
							}
						}
						else if (num2 > 180.0)
						{
							stringFormat.Alignment = StringAlignment.Far;
						}
						float num3 = num2;
						switch (circularAxisLabelsStyle)
						{
						case CircularAxisLabelsStyle.Radial:
							num3 = (float)((!(num2 > 180.0)) ? (num3 - 90.0) : (num3 + 90.0));
							break;
						case CircularAxisLabelsStyle.Circular:
							stringFormat.Alignment = StringAlignment.Center;
							stringFormat.LineAlignment = StringAlignment.Far;
							break;
						}
						Matrix transform = graph.Transform;
						if (circularAxisLabelsStyle == CircularAxisLabelsStyle.Radial || circularAxisLabelsStyle == CircularAxisLabelsStyle.Circular)
						{
							Matrix matrix2 = transform.Clone();
							matrix2.RotateAt(num3, array[0]);
							graph.Transform = matrix2;
						}
						this.InitAnimation(graph, circularAxisList.Count, num);
						graph.StartAnimation();
						Color titleColor = this.fontColor;
						if (!item.TitleColor.IsEmpty)
						{
							titleColor = item.TitleColor;
						}
						graph.DrawString(item.Title.Replace("\\n", "\n"), (this.axis.autoLabelFont == null) ? this.font : this.axis.autoLabelFont, new SolidBrush(titleColor), array[0], stringFormat);
						graph.StopAnimation();
						if (this.axis.Common.ProcessModeRegions)
						{
							SizeF size = graph.MeasureString(item.Title.Replace("\\n", "\n"), (this.axis.autoLabelFont == null) ? this.font : this.axis.autoLabelFont);
							RectangleF labelPosition = Label.GetLabelPosition(graph, array[0], size, stringFormat);
							PointF[] points = new PointF[4]
							{
								labelPosition.Location,
								new PointF(labelPosition.Right, labelPosition.Y),
								new PointF(labelPosition.Right, labelPosition.Bottom),
								new PointF(labelPosition.X, labelPosition.Bottom)
							};
							GraphicsPath graphicsPath = new GraphicsPath();
							graphicsPath.AddPolygon(points);
							graphicsPath.CloseAllFigures();
							graphicsPath.Transform(graph.Transform);
							try
							{
								this.axis.Common.HotRegionsList.AddHotRegion(graphicsPath, false, graph, ChartElementType.AxisLabels, item.Title);
							}
							catch
							{
							}
						}
						if (circularAxisLabelsStyle == CircularAxisLabelsStyle.Radial || circularAxisLabelsStyle == CircularAxisLabelsStyle.Circular)
						{
							graph.Transform = transform;
						}
					}
					num++;
				}
			}
		}

		internal static RectangleF GetLabelPosition(ChartGraphics graph, PointF position, SizeF size, StringFormat format)
		{
			RectangleF empty = RectangleF.Empty;
			empty.Width = size.Width;
			empty.Height = size.Height;
			if (format.Alignment == StringAlignment.Far)
			{
				empty.X = position.X - size.Width;
			}
			else if (format.Alignment == StringAlignment.Near)
			{
				empty.X = position.X;
			}
			else if (format.Alignment == StringAlignment.Center)
			{
				empty.X = (float)(position.X - size.Width / 2.0);
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
			return empty;
		}

		internal void Paint(ChartGraphics graph, bool backElements)
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
			if (this.axis.LabelStyle.Enabled && !double.IsNaN(this.axis.GetViewMinimum()) && !double.IsNaN(this.axis.GetViewMaximum()))
			{
				if (this.axis.chartArea.Area3DStyle.Enable3D && !this.axis.chartArea.chartAreaIsCurcular)
				{
					this.Paint3D(graph, backElements);
				}
				else
				{
					this.axis.PlotAreaPosition.ToRectangleF();
					RectangleF rectangleF = this.axis.chartArea.Position.ToRectangleF();
					float labelSize = this.axis.labelSize;
					if (this.axis.AxisPosition == AxisPosition.Left)
					{
						rectangleF.Width = labelSize;
						if (this.axis.IsMarksNextToAxis())
						{
							rectangleF.X = (float)this.axis.GetAxisPosition();
						}
						else
						{
							rectangleF.X = this.axis.PlotAreaPosition.X;
						}
						rectangleF.X -= labelSize + this.axis.markSize;
						stringFormat.Alignment = StringAlignment.Far;
						stringFormat.LineAlignment = StringAlignment.Center;
					}
					else if (this.axis.AxisPosition == AxisPosition.Right)
					{
						rectangleF.Width = labelSize;
						if (this.axis.IsMarksNextToAxis())
						{
							rectangleF.X = (float)this.axis.GetAxisPosition();
						}
						else
						{
							rectangleF.X = this.axis.PlotAreaPosition.Right();
						}
						rectangleF.X += this.axis.markSize;
						stringFormat.Alignment = StringAlignment.Near;
						stringFormat.LineAlignment = StringAlignment.Center;
					}
					else if (this.axis.AxisPosition == AxisPosition.Top)
					{
						rectangleF.Height = labelSize;
						if (this.axis.IsMarksNextToAxis())
						{
							rectangleF.Y = (float)this.axis.GetAxisPosition();
						}
						else
						{
							rectangleF.Y = this.axis.PlotAreaPosition.Y;
						}
						rectangleF.Y -= labelSize + this.axis.markSize;
						stringFormat.Alignment = StringAlignment.Center;
						stringFormat.LineAlignment = StringAlignment.Far;
					}
					else if (this.axis.AxisPosition == AxisPosition.Bottom)
					{
						rectangleF.Height = labelSize;
						if (this.axis.IsMarksNextToAxis())
						{
							rectangleF.Y = (float)this.axis.GetAxisPosition();
						}
						else
						{
							rectangleF.Y = this.axis.PlotAreaPosition.Bottom();
						}
						rectangleF.Y += this.axis.markSize;
						stringFormat.Alignment = StringAlignment.Center;
						stringFormat.LineAlignment = StringAlignment.Near;
					}
					RectangleF rectangleF2 = rectangleF;
					if (rectangleF2 != RectangleF.Empty && this.axis.totlaGroupingLabelsSize > 0.0)
					{
						if (this.axis.AxisPosition == AxisPosition.Left)
						{
							rectangleF2.X += this.axis.totlaGroupingLabelsSize;
							rectangleF2.Width -= this.axis.totlaGroupingLabelsSize;
						}
						else if (this.axis.AxisPosition == AxisPosition.Right)
						{
							rectangleF2.Width -= this.axis.totlaGroupingLabelsSize;
						}
						else if (this.axis.AxisPosition == AxisPosition.Top)
						{
							rectangleF2.Y += this.axis.totlaGroupingLabelsSize;
							rectangleF2.Height -= this.axis.totlaGroupingLabelsSize;
						}
						else if (this.axis.AxisPosition == AxisPosition.Bottom)
						{
							rectangleF2.Height -= this.axis.totlaGroupingLabelsSize;
						}
					}
					bool flag = false;
					bool flag2 = true;
					bool flag3 = true;
					int num = 0;
					foreach (CustomLabel customLabel in this.axis.CustomLabels)
					{
						bool truncatedLeft = false;
						bool truncatedRight = false;
						double axisValue = customLabel.From;
						double axisValue2 = customLabel.To;
						bool flag4 = false;
						double num2 = double.NaN;
						double num3 = double.NaN;
						double num4;
						decimal d;
						decimal d2;
						if (customLabel.RowIndex == 0)
						{
							num4 = (customLabel.From + customLabel.To) / 2.0;
							d = (decimal)this.axis.GetViewMinimum();
							d2 = (decimal)this.axis.GetViewMaximum();
							if (flag)
							{
								if ((!flag2 || !((decimal)customLabel.From < (decimal)this.axis.Minimum)) && (!flag3 || !((decimal)customLabel.To > (decimal)this.axis.Maximum)) && !((decimal)customLabel.To < d) && !((decimal)customLabel.From > d2))
								{
									if ((this.axis.autoLabelOffset == -1) ? this.OffsetLabels : (this.axis.autoLabelOffset == 1))
									{
										num = 0;
										Series series = null;
										if (this.axis.axisType == AxisName.X || this.axis.axisType == AxisName.X2)
										{
											ArrayList xAxesSeries = this.axis.chartArea.GetXAxesSeries((AxisType)((this.axis.axisType != 0) ? 1 : 0), this.axis.SubAxisName);
											if (xAxesSeries.Count > 0)
											{
												series = this.axis.Common.DataManager.Series[xAxesSeries[0]];
												if (series != null && !series.XValueIndexed)
												{
													series = null;
												}
											}
										}
										double num5 = this.axis.Minimum;
										while (num5 < this.axis.Maximum && !(num5 >= num4))
										{
											num5 += base.GetIntervalSize(num5, this.axis.LabelStyle.Interval, this.axis.LabelStyle.IntervalType, series, 0.0, DateTimeIntervalType.Number, true);
											num++;
										}
									}
									goto IL_0624;
								}
							}
							else if (!((decimal)num4 < d) && !((decimal)num4 > d2))
							{
								goto IL_0624;
							}
						}
						else if (!(customLabel.To <= this.axis.GetViewMinimum()) && !(customLabel.From >= this.axis.GetViewMaximum()))
						{
							if (!flag && this.axis.View.IsZoomed)
							{
								if (customLabel.From < this.axis.GetViewMinimum())
								{
									truncatedLeft = true;
									axisValue = this.axis.GetViewMinimum();
								}
								if (customLabel.To > this.axis.GetViewMaximum())
								{
									truncatedRight = true;
									axisValue2 = this.axis.GetViewMaximum();
								}
							}
							goto IL_075b;
						}
						continue;
						IL_0be8:
						double val = this.axis.GetLinearPosition(axisValue);
						double val2 = this.axis.GetLinearPosition(axisValue2);
						if (flag4)
						{
							flag4 = false;
							val = num2;
							val2 = num3;
						}
						RectangleF position;
						if (this.axis.AxisPosition == AxisPosition.Top || this.axis.AxisPosition == AxisPosition.Bottom)
						{
							position.X = (float)Math.Min(val, val2);
							position.Width = (float)Math.Max(val, val2) - position.X;
							if (customLabel.RowIndex == 0 && ((this.axis.autoLabelOffset == -1) ? this.OffsetLabels : (this.axis.autoLabelOffset == 1)))
							{
								position.X -= (float)(position.Width / 2.0);
								position.Width *= 2f;
							}
						}
						else
						{
							position.Y = (float)Math.Min(val, val2);
							position.Height = (float)Math.Max(val, val2) - position.Y;
							if (customLabel.RowIndex == 0 && ((this.axis.autoLabelOffset == -1) ? this.OffsetLabels : (this.axis.autoLabelOffset == 1)))
							{
								position.Y -= (float)(position.Height / 2.0);
								position.Height *= 2f;
							}
						}
						this.InitAnimation(graph, this.axis.CustomLabels.Count, num);
						graph.StartAnimation();
						graph.DrawLabelStringRel(this.axis, customLabel.RowIndex, customLabel.LabelMark, customLabel.MarkColor, customLabel.Text, customLabel.Image, customLabel.ImageTransparentColor, (this.axis.autoLabelFont == null) ? this.font : this.axis.autoLabelFont, new SolidBrush(customLabel.TextColor.IsEmpty ? this.fontColor : customLabel.TextColor), position, stringFormat, (this.axis.autoLabelAngle < -90) ? this.fontAngle : this.axis.autoLabelAngle, (!this.TruncatedLabels || customLabel.RowIndex > 0) ? RectangleF.Empty : rectangleF2, customLabel, truncatedLeft, truncatedRight);
						graph.StopAnimation();
						this.axis.ScaleSegments.EnforceSegment(null);
						this.axis.ScaleSegments.AllowOutOfScaleValues = false;
						continue;
						IL_0624:
						if (this.axis.ScaleSegments.Count > 0)
						{
							AxisScaleSegment segment = this.axis.ScaleSegments.FindScaleSegmentForAxisValue(num4);
							this.axis.ScaleSegments.AllowOutOfScaleValues = true;
							this.axis.ScaleSegments.EnforceSegment(segment);
						}
						if ((decimal)customLabel.From < d && (decimal)customLabel.To > d2)
						{
							flag4 = true;
							num2 = this.axis.GetLinearPosition(num4) - 50.0;
							num3 = num2 + 100.0;
						}
						goto IL_075b;
						IL_075b:
						position = rectangleF;
						if (customLabel.RowIndex == 0)
						{
							if (this.axis.AxisPosition == AxisPosition.Left)
							{
								position.X = rectangleF.Right - this.axis.unRotatedLabelSize;
								position.Width = this.axis.unRotatedLabelSize;
								if ((this.axis.autoLabelOffset == -1) ? this.OffsetLabels : (this.axis.autoLabelOffset == 1))
								{
									position.Width /= 2f;
									if ((float)(num % 2) != 0.0)
									{
										position.X += position.Width;
									}
								}
							}
							else if (this.axis.AxisPosition == AxisPosition.Right)
							{
								position.Width = this.axis.unRotatedLabelSize;
								if ((this.axis.autoLabelOffset == -1) ? this.OffsetLabels : (this.axis.autoLabelOffset == 1))
								{
									position.Width /= 2f;
									if ((float)(num % 2) != 0.0)
									{
										position.X += position.Width;
									}
								}
							}
							else if (this.axis.AxisPosition == AxisPosition.Top)
							{
								position.Y = rectangleF.Bottom - this.axis.unRotatedLabelSize;
								position.Height = this.axis.unRotatedLabelSize;
								if ((this.axis.autoLabelOffset == -1) ? this.OffsetLabels : (this.axis.autoLabelOffset == 1))
								{
									position.Height /= 2f;
									if ((float)(num % 2) != 0.0)
									{
										position.Y += position.Height;
									}
								}
							}
							else if (this.axis.AxisPosition == AxisPosition.Bottom)
							{
								position.Height = this.axis.unRotatedLabelSize;
								if ((this.axis.autoLabelOffset == -1) ? this.OffsetLabels : (this.axis.autoLabelOffset == 1))
								{
									position.Height /= 2f;
									if ((float)(num % 2) != 0.0)
									{
										position.Y += position.Height;
									}
								}
							}
							num++;
							goto IL_0be8;
						}
						if (customLabel.RowIndex > 0)
						{
							if (this.axis.AxisPosition == AxisPosition.Left)
							{
								position.X += this.axis.totlaGroupingLabelsSizeAdjustment;
								for (int num6 = this.axis.groupingLabelSizes.Length; num6 > customLabel.RowIndex; num6--)
								{
									position.X += this.axis.groupingLabelSizes[num6 - 1];
								}
								position.Width = this.axis.groupingLabelSizes[customLabel.RowIndex - 1];
							}
							else if (this.axis.AxisPosition == AxisPosition.Right)
							{
								position.X = position.Right - this.axis.totlaGroupingLabelsSize - this.axis.totlaGroupingLabelsSizeAdjustment;
								for (int i = 1; i < customLabel.RowIndex; i++)
								{
									position.X += this.axis.groupingLabelSizes[i - 1];
								}
								position.Width = this.axis.groupingLabelSizes[customLabel.RowIndex - 1];
							}
							else if (this.axis.AxisPosition == AxisPosition.Top)
							{
								position.Y += this.axis.totlaGroupingLabelsSizeAdjustment;
								for (int num7 = this.axis.groupingLabelSizes.Length; num7 > customLabel.RowIndex; num7--)
								{
									position.Y += this.axis.groupingLabelSizes[num7 - 1];
								}
								position.Height = this.axis.groupingLabelSizes[customLabel.RowIndex - 1];
							}
							if (this.axis.AxisPosition == AxisPosition.Bottom)
							{
								position.Y = position.Bottom - this.axis.totlaGroupingLabelsSize - this.axis.totlaGroupingLabelsSizeAdjustment;
								for (int j = 1; j < customLabel.RowIndex; j++)
								{
									position.Y += this.axis.groupingLabelSizes[j - 1];
								}
								position.Height = this.axis.groupingLabelSizes[customLabel.RowIndex - 1];
							}
							goto IL_0be8;
						}
						throw new InvalidOperationException(SR.ExceptionAxisLabelIndexIsNegative);
					}
				}
			}
		}

		private void InitAnimation(ChartGraphics graph, int numberOfElements, int index)
		{
		}

		private RectangleF GetAllLabelsRect(ChartArea area, AxisPosition position, ref StringFormat stringFormat)
		{
			Axis axis = null;
			Axis[] axes = area.Axes;
			foreach (Axis axis2 in axes)
			{
				if (axis2.AxisPosition == position)
				{
					axis = axis2;
					break;
				}
			}
			if (axis == null)
			{
				return RectangleF.Empty;
			}
			RectangleF result = area.Position.ToRectangleF();
			switch (position)
			{
			case AxisPosition.Left:
				result.Width = axis.labelSize;
				if (axis.IsMarksNextToAxis())
				{
					result.X = (float)axis.GetAxisPosition();
					result.Width = Math.Max(result.Width, result.X - axis.PlotAreaPosition.X);
				}
				else
				{
					result.X = axis.PlotAreaPosition.X;
				}
				result.X -= result.Width;
				if (area.IsSideSceneWallOnLeft() || area.Area3DStyle.WallWidth == 0)
				{
					result.X -= axis.markSize;
				}
				stringFormat.Alignment = StringAlignment.Far;
				stringFormat.LineAlignment = StringAlignment.Center;
				break;
			case AxisPosition.Right:
				result.Width = axis.labelSize;
				if (axis.IsMarksNextToAxis())
				{
					result.X = (float)axis.GetAxisPosition();
					result.Width = Math.Max(result.Width, axis.PlotAreaPosition.Right() - result.X);
				}
				else
				{
					result.X = axis.PlotAreaPosition.Right();
				}
				if (!area.IsSideSceneWallOnLeft() || area.Area3DStyle.WallWidth == 0)
				{
					result.X += axis.markSize;
				}
				stringFormat.Alignment = StringAlignment.Near;
				stringFormat.LineAlignment = StringAlignment.Center;
				break;
			case AxisPosition.Top:
				result.Height = axis.labelSize;
				if (axis.IsMarksNextToAxis())
				{
					result.Y = (float)axis.GetAxisPosition();
					result.Height = Math.Max(result.Height, result.Y - axis.PlotAreaPosition.Y);
				}
				else
				{
					result.Y = axis.PlotAreaPosition.Y;
				}
				result.Y -= result.Height;
				if (area.Area3DStyle.WallWidth == 0)
				{
					result.Y -= axis.markSize;
				}
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Far;
				break;
			case AxisPosition.Bottom:
				result.Height = axis.labelSize;
				if (axis.IsMarksNextToAxis())
				{
					result.Y = (float)axis.GetAxisPosition();
					result.Height = Math.Max(result.Height, axis.PlotAreaPosition.Bottom() - result.Y);
				}
				else
				{
					result.Y = axis.PlotAreaPosition.Bottom();
				}
				result.Y += axis.markSize;
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Near;
				break;
			}
			return result;
		}

		private AxisPosition GetLabelsPosition(ChartArea area, Axis axis)
		{
			double axisProjectionAngle = axis.GetAxisProjectionAngle();
			if (axis.AxisPosition == AxisPosition.Bottom)
			{
				if (axisProjectionAngle <= -25.0)
				{
					return AxisPosition.Right;
				}
				if (axisProjectionAngle >= 25.0)
				{
					return AxisPosition.Left;
				}
			}
			else if (axis.AxisPosition == AxisPosition.Top)
			{
				if (axisProjectionAngle <= -25.0)
				{
					return AxisPosition.Left;
				}
				if (axisProjectionAngle >= 25.0)
				{
					return AxisPosition.Right;
				}
			}
			return axis.AxisPosition;
		}

		internal void Paint3D(ChartGraphics graph, bool backElements)
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
			this.axis.PlotAreaPosition.ToRectangleF();
			SizeF relativeSize = graph.GetRelativeSize(new SizeF(1f, 1f));
			AxisPosition labelsPosition = this.GetLabelsPosition(this.axis.chartArea, this.axis);
			bool flag = default(bool);
			float num = this.axis.GetMarksZPosition(out flag);
			bool flag2 = false;
			if (this.axis.AxisPosition == AxisPosition.Top && !this.axis.chartArea.ShouldDrawOnSurface(SurfaceNames.Top, backElements, false))
			{
				flag2 = true;
			}
			if (this.axis.AxisPosition == AxisPosition.Left && !this.axis.chartArea.ShouldDrawOnSurface(SurfaceNames.Left, backElements, false))
			{
				flag2 = true;
			}
			if (this.axis.AxisPosition == AxisPosition.Right && !this.axis.chartArea.ShouldDrawOnSurface(SurfaceNames.Right, backElements, false))
			{
				flag2 = true;
			}
			if (flag2 && this.axis.chartArea.Area3DStyle.WallWidth > 0)
			{
				if (this.axis.MajorTickMark.Style == TickMarkStyle.Inside)
				{
					num -= this.axis.chartArea.areaSceneWallWidth.Width;
				}
				else if (this.axis.MajorTickMark.Style == TickMarkStyle.Outside)
				{
					num -= this.axis.MajorTickMark.Size + this.axis.chartArea.areaSceneWallWidth.Width;
				}
				else if (this.axis.MajorTickMark.Style == TickMarkStyle.Cross)
				{
					num = (float)(num - (this.axis.MajorTickMark.Size / 2.0 + this.axis.chartArea.areaSceneWallWidth.Width));
				}
			}
			bool flag3 = this.axis.IsMarksNextToAxis() && !flag;
			if (backElements != flag3)
			{
				RectangleF allLabelsRect = this.GetAllLabelsRect(this.axis.chartArea, this.axis.AxisPosition, ref stringFormat);
				RectangleF rectangleF = allLabelsRect;
				if (rectangleF != RectangleF.Empty && this.axis.totlaGroupingLabelsSize > 0.0)
				{
					if (this.axis.AxisPosition == AxisPosition.Left)
					{
						rectangleF.X += this.axis.totlaGroupingLabelsSize;
						rectangleF.Width -= this.axis.totlaGroupingLabelsSize;
					}
					else if (this.axis.AxisPosition == AxisPosition.Right)
					{
						rectangleF.Width -= this.axis.totlaGroupingLabelsSize;
					}
					else if (this.axis.AxisPosition == AxisPosition.Top)
					{
						rectangleF.Y += this.axis.totlaGroupingLabelsSize;
						rectangleF.Height -= this.axis.totlaGroupingLabelsSize;
					}
					else if (this.axis.AxisPosition == AxisPosition.Bottom)
					{
						rectangleF.Height -= this.axis.totlaGroupingLabelsSize;
					}
				}
				float num2 = -1f;
				for (int i = 0; i <= this.axis.GetGroupLabelLevelCount(); i++)
				{
					int num3 = 0;
					foreach (CustomLabel customLabel in this.axis.CustomLabels)
					{
						bool truncatedLeft = false;
						bool truncatedRight = false;
						double axisValue = customLabel.From;
						double axisValue2 = customLabel.To;
						if (customLabel.RowIndex == i)
						{
							if (customLabel.RowIndex == 0)
							{
								double num4 = (customLabel.From + customLabel.To) / 2.0;
								if (!((decimal)num4 < (decimal)this.axis.GetViewMinimum()) && !((decimal)num4 > (decimal)this.axis.GetViewMaximum()))
								{
									goto IL_0438;
								}
							}
							else if (!(customLabel.To <= this.axis.GetViewMinimum()) && !(customLabel.From >= this.axis.GetViewMaximum()))
							{
								if (this.axis.View.IsZoomed)
								{
									if (customLabel.From < this.axis.GetViewMinimum())
									{
										truncatedLeft = true;
										axisValue = this.axis.GetViewMinimum();
									}
									if (customLabel.To > this.axis.GetViewMaximum())
									{
										truncatedRight = true;
										axisValue2 = this.axis.GetViewMaximum();
									}
								}
								goto IL_0438;
							}
						}
						continue;
						IL_0911:
						double linearPosition = this.axis.GetLinearPosition(axisValue);
						double linearPosition2 = this.axis.GetLinearPosition(axisValue2);
						RectangleF position;
						if (this.axis.AxisPosition == AxisPosition.Top || this.axis.AxisPosition == AxisPosition.Bottom)
						{
							position.X = (float)Math.Min(linearPosition, linearPosition2);
							position.Width = (float)Math.Max(linearPosition, linearPosition2) - position.X;
							if (position.Width < relativeSize.Width)
							{
								position.Width = relativeSize.Width;
							}
							if (customLabel.RowIndex == 0 && ((this.axis.autoLabelOffset == -1) ? this.OffsetLabels : (this.axis.autoLabelOffset == 1)))
							{
								position.X -= (float)(position.Width / 2.0);
								position.Width *= 2f;
							}
						}
						else
						{
							position.Y = (float)Math.Min(linearPosition, linearPosition2);
							position.Height = (float)Math.Max(linearPosition, linearPosition2) - position.Y;
							if (position.Height < relativeSize.Height)
							{
								position.Height = relativeSize.Height;
							}
							if (customLabel.RowIndex == 0 && ((this.axis.autoLabelOffset == -1) ? this.OffsetLabels : (this.axis.autoLabelOffset == 1)))
							{
								position.Y -= (float)(position.Height / 2.0);
								position.Height *= 2f;
							}
						}
						RectangleF rectangleF2 = new RectangleF(position.Location, position.Size);
						Point3D[] array = new Point3D[3];
						if (this.axis.AxisPosition == AxisPosition.Left)
						{
							array[0] = new Point3D(position.Right, position.Y, num);
							array[1] = new Point3D(position.Right, (float)(position.Y + position.Height / 2.0), num);
							array[2] = new Point3D(position.Right, position.Bottom, num);
							this.axis.chartArea.matrix3D.TransformPoints(array);
							position.Y = array[0].Y;
							position.Height = array[2].Y - position.Y;
							position.Width = array[1].X - position.X;
						}
						else if (this.axis.AxisPosition == AxisPosition.Right)
						{
							array[0] = new Point3D(position.X, position.Y, num);
							array[1] = new Point3D(position.X, (float)(position.Y + position.Height / 2.0), num);
							array[2] = new Point3D(position.X, position.Bottom, num);
							this.axis.chartArea.matrix3D.TransformPoints(array);
							position.Y = array[0].Y;
							position.Height = array[2].Y - position.Y;
							position.Width = position.Right - array[1].X;
							position.X = array[1].X;
						}
						else if (this.axis.AxisPosition == AxisPosition.Top)
						{
							array[0] = new Point3D(position.X, position.Bottom, num);
							array[1] = new Point3D((float)(position.X + position.Width / 2.0), position.Bottom, num);
							array[2] = new Point3D(position.Right, position.Bottom, num);
							this.axis.chartArea.matrix3D.TransformPoints(array);
							switch (labelsPosition)
							{
							case AxisPosition.Top:
								position.X = array[0].X;
								position.Width = array[2].X - position.X;
								position.Height = array[1].Y - position.Y;
								break;
							case AxisPosition.Right:
							{
								RectangleF allLabelsRect3 = this.GetAllLabelsRect(this.axis.chartArea, labelsPosition, ref stringFormat);
								position.Y = array[0].Y;
								position.Height = array[2].Y - position.Y;
								position.X = array[1].X;
								position.Width = allLabelsRect3.Right - position.X;
								break;
							}
							case AxisPosition.Left:
							{
								RectangleF allLabelsRect2 = this.GetAllLabelsRect(this.axis.chartArea, labelsPosition, ref stringFormat);
								position.Y = array[2].Y;
								position.Height = array[0].Y - position.Y;
								position.X = allLabelsRect2.X;
								position.Width = array[1].X - allLabelsRect2.X;
								break;
							}
							}
						}
						else if (this.axis.AxisPosition == AxisPosition.Bottom)
						{
							array[0] = new Point3D(position.X, position.Y, num);
							array[1] = new Point3D((float)(position.X + position.Width / 2.0), position.Y, num);
							array[2] = new Point3D(position.Right, position.Y, num);
							this.axis.chartArea.matrix3D.TransformPoints(array);
							switch (labelsPosition)
							{
							case AxisPosition.Bottom:
								position.X = array[0].X;
								position.Width = array[2].X - position.X;
								position.Height = position.Bottom - array[1].Y;
								position.Y = array[1].Y;
								break;
							case AxisPosition.Right:
							{
								RectangleF allLabelsRect5 = this.GetAllLabelsRect(this.axis.chartArea, labelsPosition, ref stringFormat);
								position.Y = array[2].Y;
								position.Height = array[0].Y - position.Y;
								position.X = array[1].X;
								position.Width = allLabelsRect5.Right - position.X;
								if (this.axis.autoLabelAngle == 0)
								{
									position.Y += (float)(this.axis.markSize / 4.0);
								}
								break;
							}
							case AxisPosition.Left:
							{
								RectangleF allLabelsRect4 = this.GetAllLabelsRect(this.axis.chartArea, labelsPosition, ref stringFormat);
								position.Y = array[0].Y;
								position.Height = array[2].Y - position.Y;
								position.X = allLabelsRect4.X;
								position.Width = array[1].X - allLabelsRect4.X;
								if (this.axis.autoLabelAngle == 0)
								{
									position.Y += (float)(this.axis.markSize / 4.0);
								}
								break;
							}
							}
						}
						Axis axis = null;
						Axis[] axes = this.axis.chartArea.Axes;
						foreach (Axis axis2 in axes)
						{
							if (axis2.AxisPosition == labelsPosition)
							{
								axis = axis2;
								break;
							}
						}
						int num5 = (this.axis.autoLabelAngle < -90) ? this.fontAngle : this.axis.autoLabelAngle;
						if (labelsPosition != this.axis.AxisPosition)
						{
							if (this.axis.AxisPosition != AxisPosition.Top && this.axis.AxisPosition != AxisPosition.Bottom)
							{
								goto IL_10b1;
							}
							if (num5 != 90 && num5 != -90)
							{
								goto IL_10b1;
							}
							num5 = 0;
						}
						goto IL_1109;
						IL_10b1:
						if (this.axis.AxisPosition == AxisPosition.Bottom)
						{
							if (labelsPosition == AxisPosition.Left && num5 > 0)
							{
								num5 = -num5;
							}
							else if (labelsPosition == AxisPosition.Right && num5 < 0)
							{
								num5 = -num5;
							}
						}
						else if (this.axis.AxisPosition == AxisPosition.Top)
						{
							if (labelsPosition == AxisPosition.Left && num5 < 0)
							{
								num5 = -num5;
							}
							else if (labelsPosition == AxisPosition.Right && num5 > 0)
							{
								num5 = -num5;
							}
						}
						goto IL_1109;
						IL_0438:
						position = allLabelsRect;
						if (customLabel.RowIndex == 0)
						{
							if (this.axis.AxisPosition == AxisPosition.Left)
							{
								if (!this.axis.IsMarksNextToAxis())
								{
									position.X = allLabelsRect.Right - this.axis.unRotatedLabelSize;
									position.Width = this.axis.unRotatedLabelSize;
								}
								if ((this.axis.autoLabelOffset == -1) ? this.OffsetLabels : (this.axis.autoLabelOffset == 1))
								{
									position.Width /= 2f;
									if ((float)(num3 % 2) != 0.0)
									{
										position.X += position.Width;
									}
								}
							}
							else if (this.axis.AxisPosition == AxisPosition.Right)
							{
								if (!this.axis.IsMarksNextToAxis())
								{
									position.Width = this.axis.unRotatedLabelSize;
								}
								if ((this.axis.autoLabelOffset == -1) ? this.OffsetLabels : (this.axis.autoLabelOffset == 1))
								{
									position.Width /= 2f;
									if ((float)(num3 % 2) != 0.0)
									{
										position.X += position.Width;
									}
								}
							}
							else if (this.axis.AxisPosition == AxisPosition.Top)
							{
								if (!this.axis.IsMarksNextToAxis())
								{
									position.Y = allLabelsRect.Bottom - this.axis.unRotatedLabelSize;
									position.Height = this.axis.unRotatedLabelSize;
								}
								if ((this.axis.autoLabelOffset == -1) ? this.OffsetLabels : (this.axis.autoLabelOffset == 1))
								{
									position.Height /= 2f;
									if ((float)(num3 % 2) != 0.0)
									{
										position.Y += position.Height;
									}
								}
							}
							else if (this.axis.AxisPosition == AxisPosition.Bottom)
							{
								if (!this.axis.IsMarksNextToAxis())
								{
									position.Height = this.axis.unRotatedLabelSize;
								}
								if ((this.axis.autoLabelOffset == -1) ? this.OffsetLabels : (this.axis.autoLabelOffset == 1))
								{
									position.Height /= 2f;
									if ((float)(num3 % 2) != 0.0)
									{
										position.Y += position.Height;
									}
								}
							}
							num3++;
							goto IL_0911;
						}
						if (customLabel.RowIndex > 0)
						{
							if (labelsPosition == this.axis.AxisPosition)
							{
								if (this.axis.AxisPosition == AxisPosition.Left)
								{
									position.X += this.axis.totlaGroupingLabelsSizeAdjustment;
									for (int num6 = this.axis.groupingLabelSizes.Length; num6 > customLabel.RowIndex; num6--)
									{
										position.X += this.axis.groupingLabelSizes[num6 - 1];
									}
									position.Width = this.axis.groupingLabelSizes[customLabel.RowIndex - 1];
								}
								else if (this.axis.AxisPosition == AxisPosition.Right)
								{
									position.X = position.Right - this.axis.totlaGroupingLabelsSize - this.axis.totlaGroupingLabelsSizeAdjustment;
									for (int k = 1; k < customLabel.RowIndex; k++)
									{
										position.X += this.axis.groupingLabelSizes[k - 1];
									}
									position.Width = this.axis.groupingLabelSizes[customLabel.RowIndex - 1];
								}
								else if (this.axis.AxisPosition == AxisPosition.Top)
								{
									position.Y += this.axis.totlaGroupingLabelsSizeAdjustment;
									for (int num7 = this.axis.groupingLabelSizes.Length; num7 > customLabel.RowIndex; num7--)
									{
										position.Y += this.axis.groupingLabelSizes[num7 - 1];
									}
									position.Height = this.axis.groupingLabelSizes[customLabel.RowIndex - 1];
								}
								if (this.axis.AxisPosition == AxisPosition.Bottom)
								{
									position.Y = position.Bottom - this.axis.totlaGroupingLabelsSize - this.axis.totlaGroupingLabelsSizeAdjustment;
									for (int l = 1; l < customLabel.RowIndex; l++)
									{
										position.Y += this.axis.groupingLabelSizes[l - 1];
									}
									position.Height = this.axis.groupingLabelSizes[customLabel.RowIndex - 1];
								}
								goto IL_0911;
							}
							continue;
						}
						throw new InvalidOperationException(SR.ExceptionAxisLabelRowIndexMustBe1Or2);
						IL_1109:
						StringFormat stringFormat2 = null;
						if (customLabel.RowIndex == 0 && num5 == 0 && this.axis.groupingLabelSizes != null && this.axis.groupingLabelSizes.Length > 0 && this.axis.AxisPosition == AxisPosition.Bottom && labelsPosition == AxisPosition.Bottom && !((this.axis.autoLabelOffset == -1) ? this.OffsetLabels : (this.axis.autoLabelOffset == 1)))
						{
							if (num2 == -1.0)
							{
								Point3D[] array2 = new Point3D[1]
								{
									new Point3D(rectangleF2.X, rectangleF2.Bottom - this.axis.totlaGroupingLabelsSize - this.axis.totlaGroupingLabelsSizeAdjustment, num)
								};
								this.axis.chartArea.matrix3D.TransformPoints(array2);
								float num8 = array2[0].Y - position.Y;
								num2 = ((num8 > 0.0) ? num8 : position.Height);
							}
							position.Height = num2;
							stringFormat2 = (StringFormat)stringFormat.Clone();
							if ((stringFormat.FormatFlags & StringFormatFlags.LineLimit) == (StringFormatFlags)0)
							{
								stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
							}
						}
						this.InitAnimation(graph, this.axis.CustomLabels.Count, num3);
						graph.StartAnimation();
						graph.DrawLabelStringRel(axis, customLabel.RowIndex, customLabel.LabelMark, customLabel.MarkColor, customLabel.Text, customLabel.Image, customLabel.ImageTransparentColor, (this.axis.autoLabelFont == null) ? this.font : this.axis.autoLabelFont, new SolidBrush(customLabel.TextColor.IsEmpty ? this.fontColor : customLabel.TextColor), position, stringFormat, num5, (!this.TruncatedLabels || customLabel.Row > LabelRow.First) ? RectangleF.Empty : rectangleF, customLabel, truncatedLeft, truncatedRight);
						graph.StopAnimation();
						if (stringFormat2 != null)
						{
							if (stringFormat != null)
							{
								stringFormat.Dispose();
							}
							stringFormat = stringFormat2;
							stringFormat2 = null;
						}
					}
				}
			}
		}

		internal Axis GetAxis()
		{
			return this.axis;
		}

		internal void Invalidate()
		{
		}
	}
}
