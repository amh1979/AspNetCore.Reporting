using AspNetCore.Reporting.Chart.WebForms.Design;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[DefaultProperty("Position")]
	[SRDescription("DescriptionAttributeAxisDataView_AxisDataView")]
	internal class AxisDataView
	{
		internal Axis axis;

		private double position = double.NaN;

		private double size = double.NaN;

		private DateTimeIntervalType sizeType;

		private double minSize = double.NaN;

		private DateTimeIntervalType minSizeType;

		private bool zoomable = true;

		private double smallScrollSize = double.NaN;

		private DateTimeIntervalType smallScrollSizeType;

		private double smallScrollMinSize = 1.0;

		private DateTimeIntervalType smallScrollMinSizeType;

		private bool ignoreValidation;

		[Bindable(true)]
		[SRCategory("CategoryAttributeAxisView")]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeAxisDataView_Position")]
		[TypeConverter(typeof(DoubleDateNanValueConverter))]
		[ParenthesizePropertyName(true)]
		public double Position
		{
			get
			{
				if (this.axis != null && this.axis.chartArea != null && this.axis.chartArea.chartAreaIsCurcular)
				{
					return double.NaN;
				}
				return this.position;
			}
			set
			{
				if (this.axis != null && this.axis.chartArea != null && this.axis.chartArea.chartAreaIsCurcular)
				{
					return;
				}
				if (this.position != value)
				{
					this.position = value;
					if (this.axis != null && this.axis.chartArea != null && this.axis.Common != null && this.axis.Common.ChartPicture != null && !this.axis.chartArea.alignmentInProcess)
					{
						AreaAlignOrientations orientation = (AreaAlignOrientations)((this.axis.axisType == AxisName.X || this.axis.axisType == AxisName.X2) ? 1 : 2);
						this.axis.Common.ChartPicture.AlignChartAreasAxesView(this.axis.chartArea, orientation);
					}
					if (!this.ignoreValidation && this.axis != null)
					{
						this.axis.Invalidate();
					}
				}
			}
		}

		[DefaultValue(double.NaN)]
		[Bindable(true)]
		[SRCategory("CategoryAttributeAxisView")]
		[SRDescription("DescriptionAttributeAxisDataView_Size")]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		[ParenthesizePropertyName(true)]
		public double Size
		{
			get
			{
				if (this.axis != null && this.axis.chartArea != null && this.axis.chartArea.chartAreaIsCurcular)
				{
					return double.NaN;
				}
				return this.size;
			}
			set
			{
				if (this.axis != null && this.axis.chartArea != null && this.axis.chartArea.chartAreaIsCurcular)
				{
					return;
				}
				if (this.size != value)
				{
					this.size = value;
					if (this.axis != null && this.axis.chartArea != null && this.axis.Common != null && this.axis.Common.ChartPicture != null && !this.axis.chartArea.alignmentInProcess)
					{
						AreaAlignOrientations orientation = (AreaAlignOrientations)((this.axis.axisType == AxisName.X || this.axis.axisType == AxisName.X2) ? 1 : 2);
						this.axis.Common.ChartPicture.AlignChartAreasAxesView(this.axis.chartArea, orientation);
					}
					if (!this.ignoreValidation && this.axis != null)
					{
						this.axis.Invalidate();
					}
				}
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeAxisDataView_SizeType")]
		[ParenthesizePropertyName(true)]
		public DateTimeIntervalType SizeType
		{
			get
			{
				return this.sizeType;
			}
			set
			{
				if (this.sizeType != value)
				{
					this.sizeType = ((value != DateTimeIntervalType.NotSet) ? value : DateTimeIntervalType.Auto);
					if (this.axis != null && this.axis.chartArea != null && this.axis.Common != null && this.axis.Common.ChartPicture != null && !this.axis.chartArea.alignmentInProcess)
					{
						AreaAlignOrientations orientation = (AreaAlignOrientations)((this.axis.axisType == AxisName.X || this.axis.axisType == AxisName.X2) ? 1 : 2);
						this.axis.Common.ChartPicture.AlignChartAreasAxesView(this.axis.chartArea, orientation);
					}
					if (!this.ignoreValidation && this.axis != null)
					{
						this.axis.Invalidate();
					}
				}
			}
		}

		[DefaultValue(double.NaN)]
		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeAxisDataView_MinSize")]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		public double MinSize
		{
			get
			{
				return this.minSize;
			}
			set
			{
				this.minSize = value;
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeAxisDataView_MinSizeType")]
		public DateTimeIntervalType MinSizeType
		{
			get
			{
				return this.minSizeType;
			}
			set
			{
				this.minSizeType = ((value != DateTimeIntervalType.NotSet) ? value : DateTimeIntervalType.Auto);
			}
		}

		[DefaultValue(true)]
		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeAxisDataView_Zoomable")]
		public bool Zoomable
		{
			get
			{
				return this.zoomable;
			}
			set
			{
				this.zoomable = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAxisDataView_IsZoomed")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsZoomed
		{
			get
			{
				if (!double.IsNaN(this.Size) && this.Size != 0.0)
				{
					return !double.IsNaN(this.Position);
				}
				return false;
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[TypeConverter(typeof(AxisMinMaxAutoValueConverter))]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeAxisDataView_SmallScrollSize")]
		public double SmallScrollSize
		{
			get
			{
				return this.smallScrollSize;
			}
			set
			{
				if (this.smallScrollSize != value)
				{
					this.smallScrollSize = value;
					if (!this.ignoreValidation && this.axis != null)
					{
						this.axis.Invalidate();
					}
				}
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[SRDescription("DescriptionAttributeAxisDataView_SmallScrollSizeType")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		public DateTimeIntervalType SmallScrollSizeType
		{
			get
			{
				return this.smallScrollSizeType;
			}
			set
			{
				if (this.smallScrollSizeType != value)
				{
					this.smallScrollSizeType = ((value != DateTimeIntervalType.NotSet) ? value : DateTimeIntervalType.Auto);
					if (!this.ignoreValidation && this.axis != null)
					{
						this.axis.Invalidate();
					}
				}
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAxisView")]
		[SRDescription("DescriptionAttributeAxisDataView_SmallScrollMinSize")]
		[DefaultValue(1.0)]
		public double SmallScrollMinSize
		{
			get
			{
				return this.smallScrollMinSize;
			}
			set
			{
				if (this.smallScrollMinSize != value)
				{
					this.smallScrollMinSize = value;
					if (!this.ignoreValidation && this.axis != null)
					{
						this.axis.Invalidate();
					}
				}
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeAxisDataView_SmallScrollMinSizeType")]
		public DateTimeIntervalType SmallScrollMinSizeType
		{
			get
			{
				return this.smallScrollMinSizeType;
			}
			set
			{
				if (this.smallScrollMinSizeType != value)
				{
					this.smallScrollMinSizeType = ((value != DateTimeIntervalType.NotSet) ? value : DateTimeIntervalType.Auto);
					if (!this.ignoreValidation && this.axis != null)
					{
						this.axis.Invalidate();
					}
				}
			}
		}

		public AxisDataView()
		{
			this.axis = null;
		}

		public AxisDataView(Axis axis)
		{
			this.axis = axis;
		}

		public double GetViewMinimum()
		{
			if (!double.IsNaN(this.Size))
			{
				if (!double.IsNaN(this.Position))
				{
					if (this.Position <= this.axis.minimum)
					{
						return this.Position;
					}
					return this.Position - this.axis.marginView;
				}
				this.Position = this.axis.Minimum;
			}
			return this.axis.minimum;
		}

		public double GetViewMaximum()
		{
			if (!double.IsNaN(this.Size))
			{
				if (!double.IsNaN(this.Position))
				{
					double intervalSize = this.axis.GetIntervalSize(this.Position, this.Size, this.SizeType);
					if (this.Position + intervalSize >= this.axis.maximum)
					{
						return this.Position + intervalSize;
					}
					return this.Position + intervalSize + this.axis.marginView;
				}
				this.Position = this.axis.Minimum;
			}
			return this.axis.maximum;
		}

		internal Chart GetChartObject()
		{
			if (this.axis != null)
			{
				if (this.axis.chart != null)
				{
					return this.axis.chart;
				}
				if (this.axis.Common != null && this.axis.Common.container != null)
				{
					this.axis.chart = (Chart)this.axis.Common.container.GetService(typeof(Chart));
				}
				return this.axis.chart;
			}
			return null;
		}
	}
}
