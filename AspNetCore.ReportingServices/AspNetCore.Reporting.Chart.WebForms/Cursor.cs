using AspNetCore.Reporting.Chart.WebForms.Design;
using System;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeCursor_Cursor")]
	[DefaultProperty("Enabled")]
	internal class Cursor
	{
		private ChartArea chartArea;

		private AxisName attachedToXAxis;

		private bool userEnabled;

		private bool userSelection;

		private bool autoScroll = true;

		private Color lineColor = Color.Red;

		private int lineWidth = 1;

		private ChartDashStyle lineStyle = ChartDashStyle.Solid;

		private Color selectionColor = Color.LightGray;

		private AxisType axisType;

		private double position = double.NaN;

		private double selectionStart = double.NaN;

		private double selectionEnd = double.NaN;

		private double interval = 1.0;

		private DateTimeIntervalType intervalType;

		private double intervalOffset;

		private DateTimeIntervalType intervalOffsetType;

		private Axis axis;

		private PointF userSelectionStart = PointF.Empty;

		[SRCategory("CategoryAttributeBehavior")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeCursor_Position")]
		[ParenthesizePropertyName(true)]
		[TypeConverter(typeof(DoubleDateNanValueConverter))]
		public double Position
		{
			get
			{
				return this.position;
			}
			set
			{
				if (this.position != value)
				{
					this.position = value;
					if (this.chartArea != null && this.chartArea.Common != null && this.chartArea.Common.ChartPicture != null && !this.chartArea.alignmentInProcess)
					{
						AreaAlignOrientations orientation = (AreaAlignOrientations)((this.attachedToXAxis == AxisName.X || this.attachedToXAxis == AxisName.X2) ? 1 : 2);
						this.chartArea.Common.ChartPicture.AlignChartAreasCursor(this.chartArea, orientation, false);
					}
					if (this.chartArea != null && !this.chartArea.alignmentInProcess)
					{
						this.Invalidate(false);
					}
				}
			}
		}

		[DefaultValue(double.NaN)]
		[Bindable(true)]
		[SRCategory("CategoryAttributeBehavior")]
		[SRDescription("DescriptionAttributeCursor_SelectionStart")]
		[TypeConverter(typeof(DoubleDateNanValueConverter))]
		public double SelectionStart
		{
			get
			{
				return this.selectionStart;
			}
			set
			{
				if (this.selectionStart != value)
				{
					this.selectionStart = value;
					if (this.chartArea != null && this.chartArea.Common != null && this.chartArea.Common.ChartPicture != null && !this.chartArea.alignmentInProcess)
					{
						AreaAlignOrientations orientation = (AreaAlignOrientations)((this.attachedToXAxis == AxisName.X || this.attachedToXAxis == AxisName.X2) ? 1 : 2);
						this.chartArea.Common.ChartPicture.AlignChartAreasCursor(this.chartArea, orientation, false);
					}
					if (this.chartArea != null && !this.chartArea.alignmentInProcess)
					{
						this.Invalidate(false);
					}
				}
			}
		}

		[SRCategory("CategoryAttributeBehavior")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeCursor_SelectionEnd")]
		[TypeConverter(typeof(DoubleDateNanValueConverter))]
		public double SelectionEnd
		{
			get
			{
				return this.selectionEnd;
			}
			set
			{
				if (this.selectionEnd != value)
				{
					this.selectionEnd = value;
					if (this.chartArea != null && this.chartArea.Common != null && this.chartArea.Common.ChartPicture != null && !this.chartArea.alignmentInProcess)
					{
						AreaAlignOrientations orientation = (AreaAlignOrientations)((this.attachedToXAxis == AxisName.X || this.attachedToXAxis == AxisName.X2) ? 1 : 2);
						this.chartArea.Common.ChartPicture.AlignChartAreasCursor(this.chartArea, orientation, false);
					}
					if (this.chartArea != null && !this.chartArea.alignmentInProcess)
					{
						this.Invalidate(false);
					}
				}
			}
		}

		[DefaultValue(false)]
		[Bindable(true)]
		[SRCategory("CategoryAttributeBehavior")]
		[SRDescription("DescriptionAttributeCursor_UserEnabled")]
		public bool UserEnabled
		{
			get
			{
				return this.userEnabled;
			}
			set
			{
				this.userEnabled = value;
			}
		}

		[SRCategory("CategoryAttributeBehavior")]
		[Bindable(true)]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeCursor_UserSelection")]
		public bool UserSelection
		{
			get
			{
				return this.userSelection;
			}
			set
			{
				this.userSelection = value;
			}
		}

		[DefaultValue(true)]
		[Bindable(true)]
		[SRCategory("CategoryAttributeBehavior")]
		[SRDescription("DescriptionAttributeCursor_AutoScroll")]
		public bool AutoScroll
		{
			get
			{
				return this.autoScroll;
			}
			set
			{
				this.autoScroll = value;
			}
		}

		[SRDescription("DescriptionAttributeCursor_AxisType")]
		[Bindable(true)]
		[SRCategory("CategoryAttributeBehavior")]
		[DefaultValue(AxisType.Primary)]
		public AxisType AxisType
		{
			get
			{
				return this.axisType;
			}
			set
			{
				this.axisType = value;
				this.axis = null;
				this.Invalidate(true);
			}
		}

		[SRCategory("CategoryAttributeBehavior")]
		[Bindable(true)]
		[DefaultValue(1.0)]
		[SRDescription("DescriptionAttributeCursor_Interval")]
		public double Interval
		{
			get
			{
				return this.interval;
			}
			set
			{
				this.interval = value;
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeBehavior")]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeCursor_IntervalType")]
		public DateTimeIntervalType IntervalType
		{
			get
			{
				return this.intervalType;
			}
			set
			{
				this.intervalType = ((value != DateTimeIntervalType.NotSet) ? value : DateTimeIntervalType.Auto);
			}
		}

		[SRCategory("CategoryAttributeBehavior")]
		[Bindable(true)]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeCursor_IntervalOffset")]
		public double IntervalOffset
		{
			get
			{
				return this.intervalOffset;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(SR.ExceptionCursorIntervalOffsetIsNegative, "value");
				}
				this.intervalOffset = value;
			}
		}

		[SRCategory("CategoryAttributeBehavior")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeCursor_IntervalOffsetType")]
		public DateTimeIntervalType IntervalOffsetType
		{
			get
			{
				return this.intervalOffsetType;
			}
			set
			{
				this.intervalOffsetType = ((value != DateTimeIntervalType.NotSet) ? value : DateTimeIntervalType.Auto);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Red")]
		[SRDescription("DescriptionAttributeCursor_LineColor")]
		public Color LineColor
		{
			get
			{
				return this.lineColor;
			}
			set
			{
				this.lineColor = value;
				this.Invalidate(false);
			}
		}

		[SRDescription("DescriptionAttributeCursor_LineStyle")]
		[Bindable(true)]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRCategory("CategoryAttributeAppearance")]
		public ChartDashStyle LineStyle
		{
			get
			{
				return this.lineStyle;
			}
			set
			{
				this.lineStyle = value;
				this.Invalidate(false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeCursor_LineWidth")]
		public int LineWidth
		{
			get
			{
				return this.lineWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionCursorLineWidthIsNegative);
				}
				this.lineWidth = value;
				this.Invalidate(true);
			}
		}

		[SRDescription("DescriptionAttributeCursor_SelectionColor")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "LightGray")]
		public Color SelectionColor
		{
			get
			{
				return this.selectionColor;
			}
			set
			{
				this.selectionColor = value;
				this.Invalidate(false);
			}
		}

		internal void Initialize(ChartArea chartArea, AxisName attachedToXAxis)
		{
			this.chartArea = chartArea;
			this.attachedToXAxis = attachedToXAxis;
		}

		private void Invalidate(bool invalidateArea)
		{
		}

		internal Axis GetAxis()
		{
			if (this.axis == null && this.chartArea != null)
			{
				if (this.attachedToXAxis == AxisName.X)
				{
					this.axis = ((this.axisType == AxisType.Primary) ? this.chartArea.AxisX : this.chartArea.AxisX2);
				}
				else
				{
					this.axis = ((this.axisType == AxisType.Primary) ? this.chartArea.AxisY : this.chartArea.AxisY2);
				}
			}
			return this.axis;
		}
	}
}
