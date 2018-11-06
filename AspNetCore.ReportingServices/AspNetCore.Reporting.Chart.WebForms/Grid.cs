using AspNetCore.Reporting.Chart.WebForms.ChartTypes;
using AspNetCore.Reporting.Chart.WebForms.Design;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeGrid_Grid")]
	[DefaultProperty("Enabled")]
	internal class Grid
	{
		protected const double NumberOfIntervals = 5.0;

		protected const double NumberOfDateTimeIntervals = 4.0;

		internal Axis axis;

		internal bool intervalOffsetChanged;

		internal bool intervalChanged;

		internal bool intervalTypeChanged;

		internal bool intervalOffsetTypeChanged;

		internal bool enabledChanged;

		internal double intervalOffset;

		internal double interval;

		internal DateTimeIntervalType intervalType;

		internal DateTimeIntervalType intervalOffsetType;

		internal Color borderColor = Color.Black;

		internal int borderWidth = 1;

		internal ChartDashStyle borderStyle = ChartDashStyle.Solid;

		internal bool enabled = true;

		internal bool majorGridTick;

		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeIntervalOffset3")]
		[TypeConverter(typeof(AxisElementIntervalValueConverter))]
		[Bindable(true)]
		[SRCategory("CategoryAttributeData")]
		public double IntervalOffset
		{
			get
			{
				if (this.majorGridTick && double.IsNaN(this.intervalOffset) && this.axis != null)
				{
					if (this.axis.IsSerializing())
					{
						return double.NaN;
					}
					return this.axis.IntervalOffset;
				}
				if (!this.majorGridTick && this.intervalOffset == 0.0 && this.axis.IsSerializing())
				{
					return double.NaN;
				}
				return this.intervalOffset;
			}
			set
			{
				this.intervalOffset = value;
				this.intervalOffsetChanged = true;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeData")]
		[DefaultValue(DateTimeIntervalType.NotSet)]
		[RefreshProperties(RefreshProperties.All)]
		[SRDescription("DescriptionAttributeIntervalOffsetType6")]
		[Bindable(true)]
		public DateTimeIntervalType IntervalOffsetType
		{
			get
			{
				if (this.majorGridTick && this.intervalOffsetType == DateTimeIntervalType.NotSet && this.axis != null)
				{
					if (this.axis.IsSerializing())
					{
						return DateTimeIntervalType.NotSet;
					}
					return this.axis.IntervalOffsetType;
				}
				if (!this.majorGridTick && this.intervalOffsetType == DateTimeIntervalType.Auto && this.axis.IsSerializing())
				{
					return DateTimeIntervalType.NotSet;
				}
				return this.intervalOffsetType;
			}
			set
			{
				this.intervalOffsetType = value;
				this.intervalOffsetTypeChanged = true;
				this.Invalidate();
			}
		}

		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeInterval6")]
		[TypeConverter(typeof(AxisElementIntervalValueConverter))]
		[RefreshProperties(RefreshProperties.All)]
		[Bindable(true)]
		[SRCategory("CategoryAttributeData")]
		public double Interval
		{
			get
			{
				if (this.majorGridTick && double.IsNaN(this.interval) && this.axis != null)
				{
					if (this.axis.IsSerializing())
					{
						return double.NaN;
					}
					return this.axis.Interval;
				}
				if (!this.majorGridTick && this.interval == 0.0 && this.axis.IsSerializing())
				{
					return double.NaN;
				}
				return this.interval;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(SR.ExceptionTickMarksIntervalIsNegative, "value");
				}
				this.interval = value;
				this.intervalChanged = true;
				if (!this.majorGridTick && value != 0.0 && !double.IsNaN(value) && this.axis != null && this.axis.chart != null && this.axis.chart.serializing)
				{
					this.Enabled = true;
				}
				if (this.axis != null)
				{
					if (this is TickMark)
					{
						if (this.majorGridTick)
						{
							this.axis.tempMajorTickMarkInterval = this.interval;
						}
						else
						{
							this.axis.tempMinorTickMarkInterval = this.interval;
						}
					}
					else if (this.majorGridTick)
					{
						this.axis.tempMajorGridInterval = this.interval;
					}
					else
					{
						this.axis.tempMinorGridInterval = this.interval;
					}
				}
				this.Invalidate();
			}
		}

		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.NotSet)]
		[RefreshProperties(RefreshProperties.All)]
		[SRDescription("DescriptionAttributeIntervalType3")]
		[SRCategory("CategoryAttributeData")]
		public DateTimeIntervalType IntervalType
		{
			get
			{
				if (this.majorGridTick && this.intervalType == DateTimeIntervalType.NotSet && this.axis != null)
				{
					if (this.axis.IsSerializing())
					{
						return DateTimeIntervalType.NotSet;
					}
					return this.axis.IntervalType;
				}
				if (!this.majorGridTick && this.intervalType == DateTimeIntervalType.Auto && this.axis.IsSerializing())
				{
					return DateTimeIntervalType.NotSet;
				}
				return this.intervalType;
			}
			set
			{
				this.intervalType = value;
				this.intervalTypeChanged = true;
				if (this.axis != null)
				{
					if (this is TickMark)
					{
						this.axis.tempTickMarkIntervalType = this.intervalType;
					}
					else
					{
						this.axis.tempGridIntervalType = this.intervalType;
					}
				}
				this.Invalidate();
			}
		}

		[Bindable(true)]
		[SRDescription("DescriptionAttributeLineColor5")]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "Black")]
		public Color LineColor
		{
			get
			{
				return this.borderColor;
			}
			set
			{
				this.borderColor = value;
				this.Invalidate();
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeLineStyle9")]
		public ChartDashStyle LineStyle
		{
			get
			{
				return this.borderStyle;
			}
			set
			{
				this.borderStyle = value;
				this.Invalidate();
			}
		}

		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeLineWidth8")]
		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		public int LineWidth
		{
			get
			{
				return this.borderWidth;
			}
			set
			{
				this.borderWidth = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeEnabled5")]
		[DefaultValue(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		public bool Enabled
		{
			get
			{
				if (this.axis != null && this.axis.IsSerializing() && !this.majorGridTick)
				{
					return true;
				}
				return this.enabled;
			}
			set
			{
				this.enabled = value;
				this.enabledChanged = true;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeDisabled")]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Bindable(false)]
		[DefaultValue(true)]
		public bool Disabled
		{
			get
			{
				if (this.axis != null && this.axis.IsSerializing() && this.majorGridTick)
				{
					return true;
				}
				return !this.enabled;
			}
			set
			{
				this.enabled = !value;
				this.enabledChanged = true;
				this.Invalidate();
			}
		}

		public Grid()
		{
		}

		public Grid(Axis axis, bool major)
		{
			this.Initialize(axis, major);
		}

		internal void Initialize(Axis axis, bool major)
		{
			if (!this.enabledChanged && this.axis == null && !major)
			{
				this.enabled = false;
			}
			if (this.axis == null)
			{
				if (this.interval != 0.0)
				{
					if (this is TickMark)
					{
						if (major)
						{
							axis.tempMajorTickMarkInterval = this.interval;
						}
						else
						{
							axis.tempMinorTickMarkInterval = this.interval;
						}
					}
					else if (major)
					{
						axis.tempMajorGridInterval = this.interval;
					}
					else
					{
						axis.tempMinorGridInterval = this.interval;
					}
				}
				if (this.intervalType != 0)
				{
					if (this is TickMark)
					{
						if (major)
						{
							axis.tempTickMarkIntervalType = this.intervalType;
						}
					}
					else if (major)
					{
						axis.tempGridIntervalType = this.intervalType;
					}
				}
			}
			this.axis = axis;
			this.majorGridTick = major;
		}

		internal void Invalidate()
		{
		}

		internal void Paint(ChartGraphics graph)
		{
			if (this.enabled)
			{
				if (this.axis.IsCustomGridLines())
				{
					this.PaintCustom(graph);
				}
				else
				{
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
					double num = this.interval;
					DateTimeIntervalType dateTimeIntervalType = this.intervalType;
					double num2 = this.intervalOffset;
					DateTimeIntervalType dateTimeIntervalType2 = this.intervalOffsetType;
					if (!this.majorGridTick && (this.interval == 0.0 || double.IsNaN(this.interval)))
					{
						if (this.axis.majorGrid.IntervalType == DateTimeIntervalType.Auto)
						{
							this.interval = this.axis.majorGrid.Interval / 5.0;
						}
						else
						{
							DateTimeIntervalType dateTimeIntervalType3 = this.axis.majorGrid.IntervalType;
							this.interval = ((AxisScale)this.axis).CalcInterval(this.axis.minimum, this.axis.minimum + (this.axis.maximum - this.axis.minimum) / 4.0, true, out dateTimeIntervalType3, ChartValueTypes.DateTime);
							this.intervalType = dateTimeIntervalType3;
							this.intervalOffsetType = this.axis.majorGrid.IntervalOffsetType;
							this.intervalOffset = this.axis.majorGrid.IntervalOffset;
						}
					}
					double num3 = this.axis.GetViewMinimum();
					if (!this.axis.chartArea.chartAreaIsCurcular || this.axis.axisType == AxisName.Y || this.axis.axisType == AxisName.Y2)
					{
						num3 = this.axis.AlignIntervalStart(num3, this.Interval, this.IntervalType, series, this.majorGridTick);
					}
					DateTimeIntervalType type = (this.IntervalOffsetType == DateTimeIntervalType.Auto) ? this.IntervalType : this.IntervalOffsetType;
					if (this.IntervalOffset != 0.0 && !double.IsNaN(this.IntervalOffset) && series == null)
					{
						num3 += this.axis.GetIntervalSize(num3, this.IntervalOffset, type, series, 0.0, DateTimeIntervalType.Number, true, false);
					}
					if (!((this.axis.GetViewMaximum() - this.axis.GetViewMinimum()) / this.axis.GetIntervalSize(num3, this.Interval, this.IntervalType, series, 0.0, DateTimeIntervalType.Number, true) > 10000.0) && !(this.axis.GetViewMaximum() <= this.axis.GetViewMinimum()) && !(this.Interval <= 0.0))
					{
						int num4 = 0;
						int num5 = 1;
						double num6 = num3;
						decimal d = (decimal)this.axis.GetViewMaximum();
						while ((decimal)num3 <= d)
						{
							if (!this.majorGridTick && this.axis.Logarithmic)
							{
								double logMinimum = this.GetLogMinimum(num3, series);
								if (num6 != logMinimum)
								{
									num6 = logMinimum;
									num5 = 1;
								}
								double num7 = Math.Log(1.0 + this.interval * (double)num5, this.axis.logarithmBase);
								num3 = num6;
								num3 += num7;
								num5++;
								if (this.GetLogMinimum(num3, series) == logMinimum)
								{
									if (num7 == 0.0)
									{
										throw new InvalidOperationException(SR.ExceptionTickMarksIntervalIsZero);
									}
									if ((decimal)num3 >= (decimal)this.axis.GetViewMinimum() && (decimal)num3 <= (decimal)this.axis.GetViewMaximum())
									{
										this.DrawGrid(graph, num3, (int)((this.axis.GetViewMaximum() - this.axis.GetViewMinimum()) / num7), num4);
									}
									goto IL_0478;
								}
								continue;
							}
							double num8 = this.Interval;
							double intervalSize = this.axis.GetIntervalSize(num3, num8, this.IntervalType, series, this.IntervalOffset, type, true);
							if (intervalSize == 0.0)
							{
								throw new InvalidOperationException(SR.ExceptionTickMarksIntervalIsZero);
							}
							if ((decimal)num3 >= (decimal)this.axis.GetViewMinimum())
							{
								this.DrawGrid(graph, num3, (int)((this.axis.GetViewMaximum() - this.axis.GetViewMinimum()) / intervalSize), num4);
							}
							num3 += intervalSize;
							goto IL_0478;
							IL_0478:
							if (num4++ > 10000)
							{
								break;
							}
						}
						if (!this.majorGridTick)
						{
							this.interval = num;
							this.intervalType = dateTimeIntervalType;
							this.intervalOffset = num2;
							this.intervalOffsetType = dateTimeIntervalType2;
						}
					}
				}
			}
		}

		private double GetLogMinimum(double current, Series axisSeries)
		{
			double num = this.axis.GetViewMinimum();
			DateTimeIntervalType type = (this.IntervalOffsetType == DateTimeIntervalType.Auto) ? this.IntervalType : this.IntervalOffsetType;
			if (this.IntervalOffset != 0.0 && !double.IsNaN(this.IntervalOffset) && axisSeries == null)
			{
				num += this.axis.GetIntervalSize(num, this.IntervalOffset, type, axisSeries, 0.0, DateTimeIntervalType.Number, true, false);
			}
			return num + Math.Floor(current - num);
		}

		private void DrawGrid(ChartGraphics graph, double current, int numberOfElements, int index)
		{
			CommonElements common = this.axis.Common;
			PointF point = PointF.Empty;
			PointF empty = PointF.Empty;
			PointF empty2 = PointF.Empty;
			RectangleF rectangleF = this.axis.PlotAreaPosition.ToRectangleF();
			if (this.axis.AxisPosition == AxisPosition.Left || this.axis.AxisPosition == AxisPosition.Right)
			{
				empty.X = rectangleF.X;
				empty2.X = rectangleF.Right;
				empty.Y = (float)this.axis.GetLinearPosition(current);
				empty2.Y = empty.Y;
				point = empty;
			}
			if (this.axis.AxisPosition == AxisPosition.Top || this.axis.AxisPosition == AxisPosition.Bottom)
			{
				empty.Y = rectangleF.Y;
				empty2.Y = rectangleF.Bottom;
				empty.X = (float)this.axis.GetLinearPosition(current);
				empty2.X = empty.X;
				point = empty2;
			}
			if (common.ProcessModeRegions)
			{
				if (this.axis.chartArea.Area3DStyle.Enable3D && !this.axis.chartArea.chartAreaIsCurcular)
				{
					graph.Draw3DGridLine(this.axis.chartArea, this.borderColor, this.borderWidth, this.borderStyle, empty, empty2, this.axis.AxisPosition == AxisPosition.Left || this.axis.AxisPosition == AxisPosition.Right, common, this);
				}
				else if (!this.axis.chartArea.chartAreaIsCurcular)
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					if (Math.Abs(empty.X - empty2.X) > Math.Abs(empty.Y - empty2.Y))
					{
						graphicsPath.AddLine(empty.X, (float)(empty.Y - 1.0), empty2.X, (float)(empty2.Y - 1.0));
						graphicsPath.AddLine(empty2.X, (float)(empty2.Y + 1.0), empty.X, (float)(empty.Y + 1.0));
						graphicsPath.CloseAllFigures();
					}
					else
					{
						graphicsPath.AddLine((float)(empty.X - 1.0), empty.Y, (float)(empty2.X - 1.0), empty2.Y);
						graphicsPath.AddLine((float)(empty2.X + 1.0), empty2.Y, (float)(empty.X + 1.0), empty.Y);
						graphicsPath.CloseAllFigures();
					}
					common.HotRegionsList.AddHotRegion(graphicsPath, true, graph, ChartElementType.Gridlines, this);
				}
			}
			if (common.ProcessModePaint)
			{
				if (this.axis.chartArea.chartAreaIsCurcular)
				{
					this.InitAnimation(this.axis.Common, point, graph, numberOfElements, index);
					graph.StartAnimation();
					if (this.axis.axisType == AxisName.Y)
					{
						this.axis.DrawCircularLine(this, graph, this.borderColor, this.borderWidth, this.borderStyle, empty.Y);
					}
					if (this.axis.axisType == AxisName.X)
					{
						ICircularChartType circularChartType = this.axis.chartArea.GetCircularChartType();
						if (circularChartType != null && circularChartType.RadialGridLinesSupported())
						{
							this.axis.DrawRadialLine(this, graph, this.borderColor, this.borderWidth, this.borderStyle, current);
						}
					}
					graph.StopAnimation();
				}
				else if (!this.axis.chartArea.Area3DStyle.Enable3D || this.axis.chartArea.chartAreaIsCurcular)
				{
					this.InitAnimation(this.axis.Common, point, graph, numberOfElements, index);
					graph.StartAnimation();
					graph.DrawLineRel(this.borderColor, this.borderWidth, this.borderStyle, empty, empty2);
					graph.StopAnimation();
				}
				else
				{
					graph.Draw3DGridLine(this.axis.chartArea, this.borderColor, this.borderWidth, this.borderStyle, empty, empty2, this.axis.AxisPosition == AxisPosition.Left || this.axis.AxisPosition == AxisPosition.Right, this.axis.Common, this, numberOfElements, index);
				}
			}
		}

		private void InitAnimation(CommonElements common, PointF point, ChartGraphics graph, int numberOfElements, int index)
		{
		}

		internal void PaintCustom(ChartGraphics graph)
		{
			CommonElements common = this.axis.Common;
			PointF empty = PointF.Empty;
			PointF empty2 = PointF.Empty;
			RectangleF rectangleF = this.axis.PlotAreaPosition.ToRectangleF();
			foreach (CustomLabel customLabel in this.axis.CustomLabels)
			{
				if ((customLabel.GridTicks & GridTicks.Gridline) == GridTicks.Gridline)
				{
					double num = (customLabel.To + customLabel.From) / 2.0;
					if (num >= this.axis.GetViewMinimum() && num <= this.axis.GetViewMaximum())
					{
						if (this.axis.AxisPosition == AxisPosition.Left || this.axis.AxisPosition == AxisPosition.Right)
						{
							empty.X = rectangleF.X;
							empty2.X = rectangleF.Right;
							empty.Y = (float)this.axis.GetLinearPosition(num);
							empty2.Y = empty.Y;
						}
						if (this.axis.AxisPosition == AxisPosition.Top || this.axis.AxisPosition == AxisPosition.Bottom)
						{
							empty.Y = rectangleF.Y;
							empty2.Y = rectangleF.Bottom;
							empty.X = (float)this.axis.GetLinearPosition(num);
							empty2.X = empty.X;
						}
						if (common.ProcessModeRegions)
						{
							if (!this.axis.chartArea.Area3DStyle.Enable3D || this.axis.chartArea.chartAreaIsCurcular)
							{
								GraphicsPath graphicsPath = new GraphicsPath();
								if (Math.Abs(empty.X - empty2.X) > Math.Abs(empty.Y - empty2.Y))
								{
									graphicsPath.AddLine(empty.X, (float)(empty.Y - 1.0), empty2.X, (float)(empty2.Y - 1.0));
									graphicsPath.AddLine(empty2.X, (float)(empty2.Y + 1.0), empty.X, (float)(empty.Y + 1.0));
									graphicsPath.CloseAllFigures();
								}
								else
								{
									graphicsPath.AddLine((float)(empty.X - 1.0), empty.Y, (float)(empty2.X - 1.0), empty2.Y);
									graphicsPath.AddLine((float)(empty2.X + 1.0), empty2.Y, (float)(empty.X + 1.0), empty.Y);
									graphicsPath.CloseAllFigures();
								}
								common.HotRegionsList.AddHotRegion(graphicsPath, true, graph, ChartElementType.Gridlines, this);
							}
							else
							{
								graph.Draw3DGridLine(this.axis.chartArea, this.borderColor, this.borderWidth, this.borderStyle, empty, empty2, this.axis.AxisPosition == AxisPosition.Left || this.axis.AxisPosition == AxisPosition.Right, common, this);
							}
						}
						if (common.ProcessModePaint)
						{
							if (!this.axis.chartArea.Area3DStyle.Enable3D || this.axis.chartArea.chartAreaIsCurcular)
							{
								graph.DrawLineRel(this.borderColor, this.borderWidth, this.borderStyle, empty, empty2);
							}
							else
							{
								graph.Draw3DGridLine(this.axis.chartArea, this.borderColor, this.borderWidth, this.borderStyle, empty, empty2, this.axis.AxisPosition == AxisPosition.Left || this.axis.AxisPosition == AxisPosition.Right, this.axis.Common, this);
							}
						}
					}
				}
			}
		}
	}
}
