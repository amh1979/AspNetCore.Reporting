using AspNetCore.Reporting.Chart.WebForms.Design;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class AxisScale : ChartElement
	{
		internal double margin = 100.0;

		internal double marginView;

		internal bool offsetTempSet;

		internal double marginTemp;

		private ArrayList stripLineOffsets = new ArrayList();

		private bool logarithmic;

		internal double logarithmBase = 10.0;

		internal bool reverse;

		internal bool startFromZero = true;

		internal TickMark minorTickMark;

		internal TickMark majorTickMark;

		internal Grid minorGrid;

		internal Grid majorGrid;

		internal bool enabled;

		internal bool autoEnabled = true;

		internal Label labelStyle;

		private DateTimeIntervalType intervalType;

		internal double maximum = double.NaN;

		internal double crossing = double.NaN;

		internal double minimum = double.NaN;

		internal double tempMaximum = double.NaN;

		internal double tempMinimum = double.NaN;

		internal double tempCrossing = double.NaN;

		internal CustomLabelsCollection tempLabels;

		internal bool tempAutoMaximum = true;

		internal bool tempAutoMinimum = true;

		internal double tempMajorGridInterval = double.NaN;

		internal double tempMinorGridInterval = double.NaN;

		internal double tempMajorTickMarkInterval = double.NaN;

		internal double tempMinorTickMarkInterval = double.NaN;

		internal double tempLabelInterval = double.NaN;

		internal DateTimeIntervalType tempGridIntervalType = DateTimeIntervalType.NotSet;

		internal DateTimeIntervalType tempTickMarkIntervalType = DateTimeIntervalType.NotSet;

		internal DateTimeIntervalType tempLabelIntervalType = DateTimeIntervalType.NotSet;

		internal bool paintMode;

		internal AxisName axisType;

		internal ChartArea chartArea;

		internal bool autoMaximum = true;

		internal bool autoMinimum = true;

		private AxisPosition axisPosition;

		internal Axis oppositeAxis;

		private AxisDataView view;

		internal AxisScrollBar scrollBar;

		internal bool roundedXValues;

		internal bool logarithmicConvertedToLinear;

		internal double logarithmicMinimum;

		internal double logarithmicMaximum;

		internal double logarithmicCrossing;

		internal double interval3DCorrection = double.NaN;

		internal bool optimizedGetPosition;

		internal double paintViewMax;

		internal double paintViewMin;

		internal double paintRange;

		internal double valueMultiplier;

		internal RectangleF paintAreaPosition = RectangleF.Empty;

		internal double paintAreaPositionBottom;

		internal double paintAreaPositionRight;

		internal double paintChartAreaSize;

		private IntervalAutoMode intervalAutoMode;

		internal bool scaleSegmentsUsed;

		internal int prefferedNumberofIntervals = 5;

		private Stack<double> intervalsStore = new Stack<double>();

		internal AxisScaleBreakStyle axisScaleBreakStyle;

		internal AxisScaleSegmentCollection scaleSegments;

		[Bindable(true)]
		[SRDescription("DescriptionAttributeReverse")]
		[DefaultValue(AxisPosition.Left)]
		[NotifyParentProperty(true)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal virtual AxisPosition AxisPosition
		{
			get
			{
				return this.axisPosition;
			}
			set
			{
				this.axisPosition = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeIntervalAutoMode")]
		[SRCategory("CategoryAttributeInterval")]
		[DefaultValue(IntervalAutoMode.FixedCount)]
		public IntervalAutoMode IntervalAutoMode
		{
			get
			{
				return this.intervalAutoMode;
			}
			set
			{
				this.intervalAutoMode = value;
				this.Invalidate();
			}
		}

		[DefaultValue(false)]
		[SRCategory("CategoryAttributeScale")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeReverse")]
		public bool Reverse
		{
			get
			{
				return this.reverse;
			}
			set
			{
				this.reverse = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeStartFromZero3")]
		[SRCategory("CategoryAttributeScale")]
		[Bindable(true)]
		[DefaultValue(true)]
		[NotifyParentProperty(true)]
		public bool StartFromZero
		{
			get
			{
				return this.startFromZero;
			}
			set
			{
				this.startFromZero = value;
				this.Invalidate();
			}
		}

		[DefaultValue(true)]
		[SRCategory("CategoryAttributeScale")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeMargin")]
		public bool Margin
		{
			get
			{
				if (this.margin > 0.0)
				{
					return true;
				}
				return false;
			}
			set
			{
				if (value)
				{
					this.margin = 100.0;
				}
				else
				{
					this.margin = 0.0;
				}
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeScale")]
		[RefreshProperties(RefreshProperties.All)]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeInternalIntervalType")]
		internal DateTimeIntervalType InternalIntervalType
		{
			get
			{
				return this.intervalType;
			}
			set
			{
				if (this.tempMajorGridInterval <= 0.0 || (double.IsNaN(this.tempMajorGridInterval) && ((Axis)this).Interval <= 0.0))
				{
					this.majorGrid.intervalType = value;
				}
				if (this.tempMajorTickMarkInterval <= 0.0 || (double.IsNaN(this.tempMajorTickMarkInterval) && ((Axis)this).Interval <= 0.0))
				{
					this.majorTickMark.intervalType = value;
				}
				if (this.tempLabelInterval <= 0.0 || (double.IsNaN(this.tempLabelInterval) && ((Axis)this).Interval <= 0.0))
				{
					this.labelStyle.intervalType = value;
				}
				this.intervalType = value;
				this.Invalidate();
			}
		}

		internal double SetInterval
		{
			set
			{
				if (this.tempMajorGridInterval <= 0.0 || (double.IsNaN(this.tempMajorGridInterval) && ((Axis)this).Interval <= 0.0))
				{
					this.majorGrid.interval = value;
				}
				if (this.tempMajorTickMarkInterval <= 0.0 || (double.IsNaN(this.tempMajorTickMarkInterval) && ((Axis)this).Interval <= 0.0))
				{
					this.majorTickMark.interval = value;
				}
				if (this.tempLabelInterval <= 0.0 || (double.IsNaN(this.tempLabelInterval) && ((Axis)this).Interval <= 0.0))
				{
					this.labelStyle.interval = value;
				}
				this.Invalidate();
			}
		}

		[DefaultValue(double.NaN)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeScale")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeMaximum")]
		[TypeConverter(typeof(AxisMinMaxAutoValueConverter))]
		public double Maximum
		{
			get
			{
				if (this.logarithmic && this.logarithmicConvertedToLinear && !double.IsNaN(this.maximum))
				{
					return this.logarithmicMaximum;
				}
				return this.maximum;
			}
			set
			{
				if (double.IsNaN(value))
				{
					this.autoMaximum = true;
					this.maximum = double.NaN;
				}
				else
				{
					this.maximum = value;
					this.logarithmicMaximum = value;
					this.autoMaximum = false;
				}
				((Axis)this).tempMaximum = this.maximum;
				((Axis)this).tempAutoMaximum = this.autoMaximum;
				this.Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeScale")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeMinimum")]
		[TypeConverter(typeof(AxisMinMaxAutoValueConverter))]
		public double Minimum
		{
			get
			{
				if (this.logarithmic && this.logarithmicConvertedToLinear && !double.IsNaN(this.maximum))
				{
					return this.logarithmicMinimum;
				}
				return this.minimum;
			}
			set
			{
				if (double.IsNaN(value))
				{
					this.autoMinimum = true;
					this.minimum = double.NaN;
				}
				else
				{
					this.minimum = value;
					this.autoMinimum = false;
					this.logarithmicMinimum = value;
				}
				((Axis)this).tempMinimum = this.minimum;
				((Axis)this).tempAutoMinimum = this.autoMinimum;
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[SRCategory("CategoryAttributeScale")]
		[SRDescription("DescriptionAttributeCrossing")]
		[TypeConverter(typeof(AxisCrossingValueConverter))]
		public virtual double Crossing
		{
			get
			{
				if (this.paintMode)
				{
					if (this.logarithmic)
					{
						return Math.Pow(this.logarithmBase, this.GetCrossing());
					}
					return this.GetCrossing();
				}
				return this.crossing;
			}
			set
			{
				this.crossing = value;
				((Axis)this).tempCrossing = this.crossing;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeEnabled7")]
		[Bindable(true)]
		[DefaultValue(typeof(AxisEnabled), "Auto")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeMisc")]
		public AxisEnabled Enabled
		{
			get
			{
				if (this.autoEnabled)
				{
					return AxisEnabled.Auto;
				}
				if (this.enabled)
				{
					return AxisEnabled.True;
				}
				return AxisEnabled.False;
			}
			set
			{
				switch (value)
				{
				case AxisEnabled.Auto:
					this.autoEnabled = true;
					break;
				case AxisEnabled.True:
					this.enabled = true;
					this.autoEnabled = false;
					break;
				default:
					this.enabled = false;
					this.autoEnabled = false;
					break;
				}
				this.Invalidate();
			}
		}

		[DefaultValue(false)]
		[Bindable(true)]
		[SRCategory("CategoryAttributeScale")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLogarithmic")]
		public bool Logarithmic
		{
			get
			{
				return this.logarithmic;
			}
			set
			{
				this.logarithmic = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeScale")]
		[Bindable(true)]
		[DefaultValue(10.0)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLogarithmBase")]
		public double LogarithmBase
		{
			get
			{
				return this.logarithmBase;
			}
			set
			{
				if (value < 2.0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionAxisScaleLogarithmBaseInvalid);
				}
				this.logarithmBase = value;
				this.Invalidate();
			}
		}

		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[SRCategory("CategoryAttributeScale")]
		[SRDescription("DescriptionAttributeScaleBreakStyle")]
		[NotifyParentProperty(true)]
		public virtual AxisScaleBreakStyle ScaleBreakStyle
		{
			get
			{
				return this.axisScaleBreakStyle;
			}
			set
			{
				this.axisScaleBreakStyle = value;
				this.axisScaleBreakStyle.axis = (Axis)this;
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeAxisScaleSegmentCollection_AxisScaleSegmentCollection")]
		[SRCategory("CategoryAttributeScale")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AxisScaleSegmentCollection ScaleSegments
		{
			get
			{
				return this.scaleSegments;
			}
		}

		[SRCategory("CategoryAttributeDataView")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeView")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		internal AxisDataView View
		{
			get
			{
				return this.view;
			}
			set
			{
				this.view = value;
				this.view.axis = (Axis)this;
				this.Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeDataView")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeScrollBar")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		internal AxisScrollBar ScrollBar
		{
			get
			{
				return this.scrollBar;
			}
			set
			{
				this.scrollBar = value;
				this.scrollBar.axis = (Axis)this;
				this.Invalidate();
			}
		}

		internal void SetIntervalAndType(double newInterval, DateTimeIntervalType newIntervalType)
		{
			if (this.tempMajorGridInterval <= 0.0 || (double.IsNaN(this.tempMajorGridInterval) && ((Axis)this).Interval <= 0.0))
			{
				this.majorGrid.interval = newInterval;
				this.majorGrid.intervalType = newIntervalType;
			}
			if (this.tempMajorTickMarkInterval <= 0.0 || (double.IsNaN(this.tempMajorTickMarkInterval) && ((Axis)this).Interval <= 0.0))
			{
				this.majorTickMark.interval = newInterval;
				this.majorTickMark.intervalType = newIntervalType;
			}
			if (this.tempLabelInterval <= 0.0 || (double.IsNaN(this.tempLabelInterval) && ((Axis)this).Interval <= 0.0))
			{
				this.labelStyle.interval = newInterval;
				this.labelStyle.intervalType = newIntervalType;
			}
			this.Invalidate();
		}

		internal double GetViewMinimum()
		{
			return this.view.GetViewMinimum();
		}

		internal double GetViewMaximum()
		{
			return this.view.GetViewMaximum();
		}

		public double GetPosition(double axisValue)
		{
			if (this.logarithmic && axisValue != 0.0)
			{
				axisValue = Math.Log(axisValue, this.logarithmBase);
			}
			return this.GetLinearPosition(axisValue);
		}

		public double ValueToPosition(double axisValue)
		{
			return this.GetPosition(axisValue);
		}

		public double ValueToPixelPosition(double axisValue)
		{
			double num = this.ValueToPosition(axisValue);
			if (this.AxisPosition != AxisPosition.Top && this.AxisPosition != AxisPosition.Bottom)
			{
				return num * ((float)(base.Common.ChartPicture.Height - 1) / 100.0);
			}
			return num * ((float)(base.Common.ChartPicture.Width - 1) / 100.0);
		}

		public double PositionToValue(double position)
		{
			return this.PositionToValue(position, true);
		}

		internal double PositionToValue(double position, bool validateInput)
		{
			if (validateInput && (position < 0.0 || position > 100.0))
			{
				throw new ArgumentException(SR.ExceptionAxisScalePositionInvalid, "position");
			}
			if (base.PlotAreaPosition == null)
			{
				throw new InvalidOperationException(SR.ExceptionAxisScalePositionToValueCallFailed);
			}
			position = ((this.AxisPosition != AxisPosition.Top && this.AxisPosition != AxisPosition.Bottom) ? ((double)base.PlotAreaPosition.Bottom() - position) : (position - (double)base.PlotAreaPosition.X));
			double num = (this.AxisPosition != AxisPosition.Top && this.AxisPosition != AxisPosition.Bottom) ? ((double)base.PlotAreaPosition.Height) : ((double)base.PlotAreaPosition.Width);
			double viewMaximum = this.GetViewMaximum();
			double viewMinimum = this.GetViewMinimum();
			double num2 = viewMaximum - viewMinimum;
			double num3 = 0.0;
			if (num2 != 0.0)
			{
				num3 = num2 / num * position;
			}
			if (this.reverse)
			{
				return viewMaximum - num3;
			}
			return viewMinimum + num3;
		}

		public double PixelPositionToValue(double position)
		{
			double position2 = (this.AxisPosition != AxisPosition.Top && this.AxisPosition != AxisPosition.Bottom) ? (position * (100.0 / (float)(base.Common.ChartPicture.Height - 1))) : (position * (100.0 / (float)(base.Common.ChartPicture.Width - 1)));
			return this.PositionToValue(position2);
		}

		internal double PixelPositionToValue(double position, bool validate)
		{
			double position2 = (this.AxisPosition != AxisPosition.Top && this.AxisPosition != AxisPosition.Bottom) ? (position * (100.0 / (float)(base.Common.ChartPicture.Height - 1))) : (position * (100.0 / (float)(base.Common.ChartPicture.Width - 1)));
			return this.PositionToValue(position2, validate);
		}

		public AxisScale()
		{
			this.view = new AxisDataView((Axis)this);
			this.scrollBar = new AxisScrollBar((Axis)this);
		}

		internal void SetAxisPosition()
		{
			if (this.GetOppositeAxis().reverse)
			{
				if (this.AxisPosition == AxisPosition.Left)
				{
					this.AxisPosition = AxisPosition.Right;
				}
				else if (this.AxisPosition == AxisPosition.Right)
				{
					this.AxisPosition = AxisPosition.Left;
				}
				else if (this.AxisPosition == AxisPosition.Top)
				{
					this.AxisPosition = AxisPosition.Bottom;
				}
				else if (this.AxisPosition == AxisPosition.Bottom)
				{
					this.AxisPosition = AxisPosition.Top;
				}
			}
		}

		internal void SetTempAxisOffset()
		{
			if (this.chartArea.Series.Count != 0)
			{
				Series firstSeries = this.chartArea.GetFirstSeries();
				if (firstSeries.ChartType != SeriesChartType.Column && firstSeries.ChartType != SeriesChartType.StackedColumn && firstSeries.ChartType != SeriesChartType.StackedColumn100 && firstSeries.ChartType != SeriesChartType.Bar && firstSeries.ChartType != SeriesChartType.Gantt && firstSeries.ChartType != SeriesChartType.RangeColumn && firstSeries.ChartType != SeriesChartType.StackedBar && firstSeries.ChartType != SeriesChartType.StackedBar100)
				{
					return;
				}
				if (this.margin != 100.0 && !this.offsetTempSet)
				{
					this.marginTemp = this.margin;
					string text = ((DataPointAttributes)firstSeries)["PointWidth"];
					double num = (text == null) ? 0.8 : CommonElements.ParseDouble(text);
					this.margin = num / 2.0 * 100.0;
					double num2 = this.margin / 100.0;
					double num3 = (100.0 - this.margin) / 100.0;
					if (this.intervalsStore.Count == 0)
					{
						this.intervalsStore.Push(this.labelStyle.intervalOffset);
						this.intervalsStore.Push(this.majorGrid.intervalOffset);
						this.intervalsStore.Push(this.majorTickMark.intervalOffset);
						this.intervalsStore.Push(this.minorGrid.intervalOffset);
						this.intervalsStore.Push(this.minorTickMark.intervalOffset);
					}
					this.labelStyle.intervalOffset = (double.IsNaN(this.labelStyle.intervalOffset) ? num2 : (this.labelStyle.intervalOffset + num2));
					this.majorGrid.intervalOffset = (double.IsNaN(this.majorGrid.intervalOffset) ? num2 : (this.majorGrid.intervalOffset + num2));
					this.majorTickMark.intervalOffset = (double.IsNaN(this.majorTickMark.intervalOffset) ? num2 : (this.majorTickMark.intervalOffset + num2));
					this.minorGrid.intervalOffset = (double.IsNaN(this.minorGrid.intervalOffset) ? num2 : (this.minorGrid.intervalOffset + num2));
					this.minorTickMark.intervalOffset = (double.IsNaN(this.minorTickMark.intervalOffset) ? num2 : (this.minorTickMark.intervalOffset + num2));
					foreach (StripLine stripLine in ((Axis)this).StripLines)
					{
						this.stripLineOffsets.Add(stripLine.IntervalOffset);
						stripLine.IntervalOffset -= num3;
					}
					this.offsetTempSet = true;
				}
			}
		}

		internal void ResetTempAxisOffset()
		{
			if (this.offsetTempSet)
			{
				this.minorTickMark.intervalOffset = this.intervalsStore.Pop();
				this.minorGrid.intervalOffset = this.intervalsStore.Pop();
				this.majorTickMark.intervalOffset = this.intervalsStore.Pop();
				this.majorGrid.intervalOffset = this.intervalsStore.Pop();
				this.labelStyle.intervalOffset = this.intervalsStore.Pop();
				int num = 0;
				foreach (StripLine stripLine in ((Axis)this).StripLines)
				{
					if (this.stripLineOffsets.Count > num)
					{
						stripLine.IntervalOffset = (double)this.stripLineOffsets[num];
					}
					num++;
				}
				this.stripLineOffsets.Clear();
				this.offsetTempSet = false;
				this.margin = this.marginTemp;
			}
		}

		internal double RoundedValues(double inter, bool shouldStartFromZero, bool autoMax, bool autoMin, ref double min, ref double max)
		{
			if (this.axisType == AxisName.X || this.axisType == AxisName.X2)
			{
				if (this.margin == 0.0 && !this.roundedXValues)
				{
					return inter;
				}
			}
			else if (this.margin == 0.0)
			{
				return inter;
			}
			if (autoMin)
			{
				if (min < 0.0 || (!shouldStartFromZero && !this.chartArea.stacked))
				{
					min = (Axis.RemoveNoiseFromDoubleMath(Math.Ceiling(min / inter)) - 1.0) * Axis.RemoveNoiseFromDoubleMath(inter);
				}
				else
				{
					min = 0.0;
				}
			}
			if (autoMax)
			{
				if (max <= 0.0 && shouldStartFromZero)
				{
					max = 0.0;
				}
				else
				{
					max = (Axis.RemoveNoiseFromDoubleMath(Math.Floor(max / inter)) + 1.0) * Axis.RemoveNoiseFromDoubleMath(inter);
				}
			}
			return inter;
		}

		internal double CalcInterval(double diff)
		{
			if (diff == 0.0)
			{
				throw new ArgumentOutOfRangeException("diff", SR.ExceptionAxisScaleIntervalIsZero);
			}
			double num = -1.0;
			double num2 = diff;
			while (num2 > 1.0)
			{
				num += 1.0;
				num2 /= 10.0;
				if (num > 1000.0)
				{
					throw new InvalidOperationException(SR.ExceptionAxisScaleMinimumMaximumInvalid);
				}
			}
			num2 = diff;
			if (num2 < 1.0)
			{
				num = 0.0;
			}
			while (num2 < 1.0)
			{
				num -= 1.0;
				num2 *= 10.0;
				if (num < -1000.0)
				{
					throw new InvalidOperationException(SR.ExceptionAxisScaleMinimumMaximumInvalid);
				}
			}
			double x = this.Logarithmic ? this.logarithmBase : 10.0;
			double num3 = diff / Math.Pow(x, num);
			num3 = ((!(num3 < 3.0)) ? ((!(num3 < 7.0)) ? 10.0 : 5.0) : 2.0);
			return num3 * Math.Pow(x, num);
		}

		private double CalcInterval(double min, double max)
		{
			return this.CalcInterval((max - min) / 5.0);
		}

		internal double CalcInterval(double min, double max, bool date, out DateTimeIntervalType type, ChartValueTypes valuesType)
		{
			if (date)
			{
				DateTime value = DateTime.FromOADate(min);
				TimeSpan timeSpan = DateTime.FromOADate(max).Subtract(value);
				double totalMinutes = timeSpan.TotalMinutes;
				if (totalMinutes <= 1.0 && valuesType != ChartValueTypes.Date)
				{
					double totalMilliseconds = timeSpan.TotalMilliseconds;
					if (totalMilliseconds <= 10.0)
					{
						type = DateTimeIntervalType.Milliseconds;
						return 1.0;
					}
					if (totalMilliseconds <= 50.0)
					{
						type = DateTimeIntervalType.Milliseconds;
						return 4.0;
					}
					if (totalMilliseconds <= 200.0)
					{
						type = DateTimeIntervalType.Milliseconds;
						return 20.0;
					}
					if (totalMilliseconds <= 500.0)
					{
						type = DateTimeIntervalType.Milliseconds;
						return 50.0;
					}
					double totalSeconds = timeSpan.TotalSeconds;
					if (totalSeconds <= 7.0)
					{
						type = DateTimeIntervalType.Seconds;
						return 1.0;
					}
					if (totalSeconds <= 15.0)
					{
						type = DateTimeIntervalType.Seconds;
						return 2.0;
					}
					if (totalSeconds <= 30.0)
					{
						type = DateTimeIntervalType.Seconds;
						return 5.0;
					}
					if (totalSeconds <= 60.0)
					{
						type = DateTimeIntervalType.Seconds;
						return 10.0;
					}
				}
				else
				{
					if (totalMinutes <= 2.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Seconds;
						return 20.0;
					}
					if (totalMinutes <= 3.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Seconds;
						return 30.0;
					}
					if (totalMinutes <= 10.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Minutes;
						return 1.0;
					}
					if (totalMinutes <= 20.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Minutes;
						return 2.0;
					}
					if (totalMinutes <= 60.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Minutes;
						return 5.0;
					}
					if (totalMinutes <= 120.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Minutes;
						return 10.0;
					}
					if (totalMinutes <= 180.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Minutes;
						return 30.0;
					}
					if (totalMinutes <= 720.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Hours;
						return 1.0;
					}
					if (totalMinutes <= 1440.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Hours;
						return 4.0;
					}
					if (totalMinutes <= 2880.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Hours;
						return 6.0;
					}
					if (totalMinutes <= 4320.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Hours;
						return 12.0;
					}
					if (totalMinutes <= 14400.0)
					{
						type = DateTimeIntervalType.Days;
						return 1.0;
					}
					if (totalMinutes <= 28800.0)
					{
						type = DateTimeIntervalType.Days;
						return 2.0;
					}
					if (totalMinutes <= 43200.0)
					{
						type = DateTimeIntervalType.Days;
						return 3.0;
					}
					if (totalMinutes <= 87840.0)
					{
						type = DateTimeIntervalType.Weeks;
						return 1.0;
					}
					if (totalMinutes <= 219600.0)
					{
						type = DateTimeIntervalType.Weeks;
						return 2.0;
					}
					if (totalMinutes <= 527040.0)
					{
						type = DateTimeIntervalType.Months;
						return 1.0;
					}
					if (totalMinutes <= 1054080.0)
					{
						type = DateTimeIntervalType.Months;
						return 3.0;
					}
					if (totalMinutes <= 2108160.0)
					{
						type = DateTimeIntervalType.Months;
						return 6.0;
					}
					if (totalMinutes >= 2108160.0)
					{
						type = DateTimeIntervalType.Years;
						return this.CalcYearInterval(totalMinutes / 60.0 / 24.0 / 365.0);
					}
				}
			}
			type = DateTimeIntervalType.Number;
			return this.CalcInterval(min, max);
		}

		private double CalcYearInterval(double years)
		{
			if (years <= 1.0)
			{
				throw new ArgumentOutOfRangeException("years", SR.ExceptionAxisScaleIntervalIsLessThen1Year);
			}
			if (years < 5.0)
			{
				return 1.0;
			}
			if (years < 10.0)
			{
				return 2.0;
			}
			return Math.Floor(years / 5.0);
		}

		private int GetNumOfUnits(double min, double max, DateTimeIntervalType type)
		{
			double intervalSize = base.GetIntervalSize(min, 1.0, type);
			return (int)Math.Round((max - min) / intervalSize);
		}

		internal ChartValueTypes GetDateTimeType()
		{
			ArrayList arrayList = null;
			ChartValueTypes result = ChartValueTypes.Auto;
			if (this.axisType == AxisName.X)
			{
				arrayList = this.chartArea.GetXAxesSeries(AxisType.Primary, ((Axis)this).SubAxisName);
				if (arrayList.Count == 0)
				{
					return ChartValueTypes.Auto;
				}
				if (base.Common.DataManager.Series[arrayList[0]].IsXValueDateTime())
				{
					result = base.Common.DataManager.Series[arrayList[0]].XValueType;
				}
			}
			else if (this.axisType == AxisName.X2)
			{
				arrayList = this.chartArea.GetXAxesSeries(AxisType.Secondary, ((Axis)this).SubAxisName);
				if (arrayList.Count == 0)
				{
					return ChartValueTypes.Auto;
				}
				if (base.Common.DataManager.Series[arrayList[0]].IsXValueDateTime())
				{
					result = base.Common.DataManager.Series[arrayList[0]].XValueType;
				}
			}
			else if (this.axisType == AxisName.Y)
			{
				arrayList = this.chartArea.GetYAxesSeries(AxisType.Primary, ((Axis)this).SubAxisName);
				if (arrayList.Count == 0)
				{
					return ChartValueTypes.Auto;
				}
				if (base.Common.DataManager.Series[arrayList[0]].IsYValueDateTime())
				{
					result = base.Common.DataManager.Series[arrayList[0]].YValueType;
				}
			}
			else if (this.axisType == AxisName.Y2)
			{
				arrayList = this.chartArea.GetYAxesSeries(AxisType.Secondary, ((Axis)this).SubAxisName);
				if (arrayList.Count == 0)
				{
					return ChartValueTypes.Auto;
				}
				if (base.Common.DataManager.Series[arrayList[0]].IsYValueDateTime())
				{
					result = base.Common.DataManager.Series[arrayList[0]].YValueType;
				}
			}
			return result;
		}

		private double GetCrossing()
		{
			if (double.IsNaN(this.crossing))
			{
				if (base.Common.ChartTypeRegistry.GetChartType((string)this.chartArea.ChartTypes[0]).ZeroCrossing)
				{
					if (this.GetViewMinimum() > 0.0)
					{
						return this.GetViewMinimum();
					}
					if (this.GetViewMaximum() < 0.0)
					{
						return this.GetViewMaximum();
					}
					return 0.0;
				}
				return this.GetViewMinimum();
			}
			if (this.crossing == 1.7976931348623157E+308)
			{
				return this.GetViewMaximum();
			}
			if (this.crossing == -1.7976931348623157E+308)
			{
				return this.GetViewMinimum();
			}
			return this.crossing;
		}

		internal void SetAutoMinimum(double min)
		{
			if (this.autoMinimum)
			{
				this.minimum = min;
			}
		}

		internal void SetAutoMaximum(double max)
		{
			if (this.autoMaximum)
			{
				this.maximum = max;
			}
		}

		internal Axis GetOppositeAxis()
		{
			if (this.oppositeAxis != null)
			{
				return this.oppositeAxis;
			}
			switch (this.axisType)
			{
			case AxisName.X:
			{
				ArrayList yAxesSeries = this.chartArea.GetXAxesSeries(AxisType.Primary, ((Axis)this).SubAxisName);
				if (yAxesSeries.Count == 0)
				{
					this.oppositeAxis = this.chartArea.AxisY;
				}
				else if (base.Common.DataManager.Series[yAxesSeries[0]].YAxisType == AxisType.Primary)
				{
					this.oppositeAxis = this.chartArea.AxisY.GetSubAxis(base.Common.DataManager.Series[yAxesSeries[0]].YSubAxisName);
				}
				else
				{
					this.oppositeAxis = this.chartArea.AxisY2.GetSubAxis(base.Common.DataManager.Series[yAxesSeries[0]].YSubAxisName);
				}
				break;
			}
			case AxisName.X2:
			{
				ArrayList yAxesSeries = this.chartArea.GetXAxesSeries(AxisType.Secondary, ((Axis)this).SubAxisName);
				if (yAxesSeries.Count == 0)
				{
					this.oppositeAxis = this.chartArea.AxisY2;
				}
				else if (base.Common.DataManager.Series[yAxesSeries[0]].YAxisType == AxisType.Primary)
				{
					this.oppositeAxis = this.chartArea.AxisY.GetSubAxis(base.Common.DataManager.Series[yAxesSeries[0]].YSubAxisName);
				}
				else
				{
					this.oppositeAxis = this.chartArea.AxisY2.GetSubAxis(base.Common.DataManager.Series[yAxesSeries[0]].YSubAxisName);
				}
				break;
			}
			case AxisName.Y:
			{
				ArrayList yAxesSeries = this.chartArea.GetYAxesSeries(AxisType.Primary, ((Axis)this).SubAxisName);
				if (yAxesSeries.Count == 0)
				{
					this.oppositeAxis = this.chartArea.AxisX;
				}
				else if (base.Common.DataManager.Series[yAxesSeries[0]].XAxisType == AxisType.Primary)
				{
					this.oppositeAxis = this.chartArea.AxisX.GetSubAxis(base.Common.DataManager.Series[yAxesSeries[0]].XSubAxisName);
				}
				else
				{
					this.oppositeAxis = this.chartArea.AxisX2.GetSubAxis(base.Common.DataManager.Series[yAxesSeries[0]].XSubAxisName);
				}
				break;
			}
			case AxisName.Y2:
			{
				ArrayList yAxesSeries = this.chartArea.GetYAxesSeries(AxisType.Secondary, ((Axis)this).SubAxisName);
				if (yAxesSeries.Count == 0)
				{
					this.oppositeAxis = this.chartArea.AxisX2;
				}
				else if (base.Common.DataManager.Series[yAxesSeries[0]].XAxisType == AxisType.Primary)
				{
					this.oppositeAxis = this.chartArea.AxisX.GetSubAxis(base.Common.DataManager.Series[yAxesSeries[0]].XSubAxisName);
				}
				else
				{
					this.oppositeAxis = this.chartArea.AxisX2.GetSubAxis(base.Common.DataManager.Series[yAxesSeries[0]].XSubAxisName);
				}
				break;
			}
			}
			return this.oppositeAxis;
		}

		internal void Invalidate()
		{
		}

		internal double GetLinearPosition(double axisValue)
		{
			bool flag = (byte)((this.chartArea != null && this.chartArea.chartAreaIsCurcular) ? 1 : 0) != 0;
			if (!this.optimizedGetPosition)
			{
				this.paintViewMax = this.GetViewMaximum();
				this.paintViewMin = this.GetViewMinimum();
				this.paintRange = this.paintViewMax - this.paintViewMin;
				this.paintAreaPosition = base.PlotAreaPosition.ToRectangleF();
				if (flag)
				{
					this.paintAreaPosition.Width /= 2f;
					this.paintAreaPosition.Height /= 2f;
				}
				this.paintAreaPositionBottom = (double)(this.paintAreaPosition.Y + this.paintAreaPosition.Height);
				this.paintAreaPositionRight = (double)(this.paintAreaPosition.X + this.paintAreaPosition.Width);
				if (this.AxisPosition == AxisPosition.Top || this.AxisPosition == AxisPosition.Bottom)
				{
					this.paintChartAreaSize = (double)this.paintAreaPosition.Width;
				}
				else
				{
					this.paintChartAreaSize = (double)this.paintAreaPosition.Height;
				}
				this.valueMultiplier = 0.0;
				if (this.paintRange != 0.0)
				{
					this.valueMultiplier = this.paintChartAreaSize / this.paintRange;
				}
			}
			double num = this.valueMultiplier * (axisValue - this.paintViewMin);
			if (this.scaleSegmentsUsed)
			{
				AxisScaleSegment axisScaleSegment = this.ScaleSegments.FindScaleSegmentForAxisValue(axisValue);
				if (axisScaleSegment != null)
				{
					double num2 = 0.0;
					double num3 = 0.0;
					axisScaleSegment.GetScalePositionAndSize(this.paintChartAreaSize, out num3, out num2);
					if (!this.ScaleSegments.AllowOutOfScaleValues)
					{
						if (axisValue > axisScaleSegment.ScaleMaximum)
						{
							axisValue = axisScaleSegment.ScaleMaximum;
						}
						else if (axisValue < axisScaleSegment.ScaleMinimum)
						{
							axisValue = axisScaleSegment.ScaleMinimum;
						}
					}
					double num4 = axisScaleSegment.ScaleMaximum - axisScaleSegment.ScaleMinimum;
					num = num2 / num4 * (axisValue - axisScaleSegment.ScaleMinimum);
					num += num3;
				}
			}
			if (this.reverse)
			{
				if (this.AxisPosition != AxisPosition.Top && this.AxisPosition != AxisPosition.Bottom)
				{
					return (double)this.paintAreaPosition.Y + num;
				}
				return this.paintAreaPositionRight - num;
			}
			if (this.AxisPosition != AxisPosition.Top && this.AxisPosition != AxisPosition.Bottom)
			{
				return this.paintAreaPositionBottom - num;
			}
			return (double)this.paintAreaPosition.X + num;
		}

		internal void EstimateAxis()
		{
			if (!double.IsNaN(this.View.Size) && double.IsNaN(this.View.Position))
			{
				this.View.Position = this.Minimum;
			}
			double num;
			if (!double.IsNaN(this.view.Position) && !double.IsNaN(this.view.Size))
			{
				double y = this.GetViewMaximum();
				double y2 = this.GetViewMinimum();
				if (this.logarithmic)
				{
					y = Math.Pow(this.logarithmBase, y);
					y2 = Math.Pow(this.logarithmBase, y2);
				}
				else
				{
					this.EstimateAxis(ref this.minimum, ref this.maximum, this.autoMaximum, this.autoMinimum);
				}
				num = this.EstimateAxis(ref y2, ref y, true, true);
			}
			else
			{
				num = this.EstimateAxis(ref this.minimum, ref this.maximum, this.autoMaximum, this.autoMinimum);
			}
			if (num <= 0.0)
			{
				throw new InvalidOperationException(SR.ExceptionAxisScaleAutoIntervalInvalid);
			}
			if (this.chartArea.SeriesIntegerType(this.axisType, string.Empty))
			{
				num = Math.Round(num);
				if (num == 0.0)
				{
					num = 1.0;
				}
				this.minimum = Math.Floor(this.minimum);
			}
			this.SetInterval = num;
		}

		internal double EstimateAxis(ref double minimum, ref double maximum, bool autoMaximum, bool autoMinimum)
		{
			if (maximum < minimum)
			{
				if (!base.Common.ChartPicture.SuppressExceptions)
				{
					throw new InvalidOperationException(SR.ExceptionAxisScaleMinimumValueIsGreaterThenMaximumDataPoint);
				}
				double num = maximum;
				maximum = minimum;
				minimum = num;
			}
			ChartValueTypes dateTimeType = this.GetDateTimeType();
			double num2 = (!this.logarithmic) ? ((dateTimeType == ChartValueTypes.Auto) ? this.EstimateNumberAxis(ref minimum, ref maximum, this.StartFromZero, this.prefferedNumberofIntervals, this.crossing, autoMaximum, autoMinimum) : this.EstimateDateAxis(ref minimum, ref maximum, this.crossing, autoMaximum, autoMinimum, dateTimeType)) : this.EstimateLogarithmicAxis(ref minimum, ref maximum, this.crossing, autoMaximum, autoMinimum);
			if (num2 <= 0.0)
			{
				throw new InvalidOperationException(SR.ExceptionAxisScaleAutoIntervalInvalid);
			}
			this.SetInterval = num2;
			return num2;
		}

		private double EstimateLogarithmicAxis(ref double minimum, ref double maximum, double crossing, bool autoMaximum, bool autoMinimum)
		{
			if (!this.logarithmicConvertedToLinear)
			{
				this.logarithmicMinimum = this.minimum;
				this.logarithmicMaximum = this.maximum;
				this.logarithmicCrossing = this.crossing;
			}
			this.margin = 100.0;
			if (base.Common != null && base.Common.Chart != null && base.Common.Chart.chartPicture.SuppressExceptions)
			{
				if (minimum <= 0.0)
				{
					minimum = 1.0;
				}
				if (maximum <= 0.0)
				{
					maximum = 1.0;
				}
				if (crossing <= 0.0 && crossing != -1.7976931348623157E+308)
				{
					crossing = 1.0;
				}
			}
			if (minimum <= 0.0 || maximum <= 0.0 || crossing <= 0.0)
			{
				if (minimum <= 0.0)
				{
					throw new ArgumentOutOfRangeException("minimum", SR.ExceptionAxisScaleLogarithmicNegativeValues);
				}
				if (maximum <= 0.0)
				{
					throw new ArgumentOutOfRangeException("maximum", SR.ExceptionAxisScaleLogarithmicNegativeValues);
				}
			}
			crossing = Math.Log(crossing, this.logarithmBase);
			minimum = Math.Log(minimum, this.logarithmBase);
			maximum = Math.Log(maximum, this.logarithmBase);
			this.logarithmicConvertedToLinear = true;
			double d = (maximum - minimum) / 5.0;
			double num = Math.Floor(d);
			if (num == 0.0)
			{
				num = 1.0;
			}
			if (autoMinimum && autoMaximum)
			{
				this.RoundedValues(num, this.StartFromZero, autoMaximum, autoMinimum, ref minimum, ref maximum);
			}
			if (this.chartArea.hundredPercent)
			{
				if (autoMinimum && minimum < 0.0)
				{
					minimum = 0.0;
				}
				if (autoMaximum && maximum > 2.0)
				{
					maximum = 2.0;
				}
			}
			return num;
		}

		private double EstimateDateAxis(ref double minimum, ref double maximum, double crossing, bool autoMaximum, bool autoMinimum, ChartValueTypes valuesType)
		{
			double num = minimum;
			double num2 = maximum;
			double num3 = this.CalcInterval(num, num2, true, out this.intervalType, valuesType);
			if (!double.IsNaN(this.interval3DCorrection) && this.chartArea.Area3DStyle.Enable3D && !this.chartArea.chartAreaIsCurcular)
			{
				num3 = Math.Floor(num3 / this.interval3DCorrection);
				this.interval3DCorrection = double.NaN;
			}
			int numOfUnits = this.GetNumOfUnits(num, num2, this.intervalType);
			if (this.axisType == AxisName.Y || this.axisType == AxisName.Y2)
			{
				if (autoMinimum && minimum > base.GetIntervalSize(num, num3, this.intervalType))
				{
					minimum += base.GetIntervalSize(num, (0.0 - num3 / 2.0) * this.margin / 100.0, this.intervalType, null, 0.0, DateTimeIntervalType.Number, false, false);
					minimum = base.AlignIntervalStart(minimum, num3 * this.margin / 100.0, this.intervalType);
				}
				if (autoMaximum && num2 > 0.0 && this.margin != 0.0)
				{
					maximum = minimum + base.GetIntervalSize(minimum, (Math.Floor((double)numOfUnits / num3 / this.margin * 100.0) + 2.0) * num3 * this.margin / 100.0, this.intervalType);
				}
			}
			this.InternalIntervalType = this.intervalType;
			return num3;
		}

		internal double EstimateNumberAxis(ref double minimum, ref double maximum, bool shouldStartFromZero, int preferredNumberOfIntervals, double crossing, bool autoMaximum, bool autoMinimum)
		{
			double num = minimum;
			double num2 = maximum;
			double num3;
			if (!this.roundedXValues && (this.axisType == AxisName.X || this.axisType == AxisName.X2))
			{
				num3 = this.chartArea.GetPointsInterval(false, 10.0);
				if (num3 == 0.0 || (num2 - num) / num3 > 20.0)
				{
					num3 = (num2 - num) / (double)preferredNumberOfIntervals;
				}
			}
			else
			{
				num3 = (num2 - num) / (double)preferredNumberOfIntervals;
			}
			if (!double.IsNaN(this.interval3DCorrection) && this.chartArea.Area3DStyle.Enable3D && !this.chartArea.chartAreaIsCurcular)
			{
				num3 /= this.interval3DCorrection;
				if (num2 - num < num3)
				{
					num3 = num2 - num;
				}
				this.interval3DCorrection = double.NaN;
				if (num3 != 0.0)
				{
					num3 = this.CalcInterval(num3);
				}
			}
			double num4;
			if (autoMaximum || autoMinimum)
			{
				if (num3 == 0.0)
				{
					num2 = num + 1.0;
					num3 = 0.2;
					num4 = 0.2;
				}
				else
				{
					num4 = this.CalcInterval(num3);
				}
			}
			else
			{
				num4 = num3;
			}
			if (((Axis)this).interval != 0.0 && ((Axis)this).interval > num4 && minimum + ((Axis)this).interval > maximum)
			{
				num4 = ((Axis)this).interval;
				if (autoMaximum)
				{
					maximum = minimum + num4;
				}
				if (autoMinimum)
				{
					minimum = maximum - num4;
				}
			}
			if (this.axisType == AxisName.Y || this.axisType == AxisName.Y2 || (this.roundedXValues && (this.axisType == AxisName.X || this.axisType == AxisName.X2)))
			{
				bool flag = false;
				bool flag2 = false;
				if (this.chartArea.hundredPercent)
				{
					flag = (minimum == 0.0);
					flag2 = (maximum == 0.0);
				}
				this.RoundedValues(num4, shouldStartFromZero, autoMaximum, autoMinimum, ref minimum, ref maximum);
				if (this.chartArea.hundredPercent)
				{
					if (autoMinimum)
					{
						if (minimum < -100.0)
						{
							minimum = -100.0;
						}
						if (flag)
						{
							minimum = 0.0;
						}
					}
					if (autoMaximum)
					{
						if (maximum > 100.0)
						{
							maximum = 100.0;
						}
						if (flag2)
						{
							maximum = 0.0;
						}
					}
				}
			}
			return num4;
		}
	}
}
