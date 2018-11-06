using AspNetCore.Reporting.Chart.WebForms.Design;
using System;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[DefaultProperty("Enabled")]
	[SRDescription("DescriptionAttributeSmartLabelsStyle_SmartLabelsStyle")]
	[TypeConverter(typeof(NoNameExpandableObjectConverter))]
	internal class SmartLabelsStyle
	{
		internal object chartElement;

		private bool enabled;

		private bool markerOverlapping;

		private bool hideOverlapped = true;

		private LabelAlignmentTypes movingDirection = LabelAlignmentTypes.Top | LabelAlignmentTypes.Bottom | LabelAlignmentTypes.Right | LabelAlignmentTypes.Left | LabelAlignmentTypes.TopLeft | LabelAlignmentTypes.TopRight | LabelAlignmentTypes.BottomLeft | LabelAlignmentTypes.BottomRight;

		private double minMovingDistance;

		private double maxMovingDistance = 30.0;

		private LabelOutsidePlotAreaStyle allowOutsidePlotArea = LabelOutsidePlotAreaStyle.Partial;

		private LabelCalloutStyle calloutStyle = LabelCalloutStyle.Underlined;

		private Color calloutLineColor = Color.Black;

		private ChartDashStyle calloutLineStyle = ChartDashStyle.Solid;

		private Color calloutBackColor = Color.Transparent;

		private int calloutLineWidth = 1;

		private LineAnchorCap calloutLineAnchorCap = LineAnchorCap.Arrow;

		[DefaultValue(true)]
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeEnabled13")]
		[ParenthesizePropertyName(true)]
		public virtual bool Enabled
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

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeMarkerOverlapping")]
		public virtual bool MarkerOverlapping
		{
			get
			{
				return this.markerOverlapping;
			}
			set
			{
				this.markerOverlapping = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeHideOverlapped")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRCategory("CategoryAttributeMisc")]
		public virtual bool HideOverlapped
		{
			get
			{
				return this.hideOverlapped;
			}
			set
			{
				this.hideOverlapped = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue(typeof(LabelAlignmentTypes), "Top, Bottom, Right, Left, TopLeft, TopRight, BottomLeft, BottomRight")]
		[SRDescription("DescriptionAttributeMovingDirection")]
		public virtual LabelAlignmentTypes MovingDirection
		{
			get
			{
				return this.movingDirection;
			}
			set
			{
				if (value == (LabelAlignmentTypes)0)
				{
					Series series = this.chartElement as Series;
					if (series != null && series.chart != null && series.chart.SuppressExceptions)
					{
						goto IL_0032;
					}
					throw new InvalidOperationException(SR.ExceptionSmartLabelsDirectionUndefined);
				}
				goto IL_0032;
				IL_0032:
				this.movingDirection = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeMinMovingDistance")]
		public virtual double MinMovingDistance
		{
			get
			{
				return this.minMovingDistance;
			}
			set
			{
				if (value < 0.0)
				{
					throw new InvalidOperationException(SR.ExceptionSmartLabelsMinMovingDistanceIsNegative);
				}
				this.minMovingDistance = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue(30.0)]
		[SRDescription("DescriptionAttributeMaxMovingDistance")]
		public virtual double MaxMovingDistance
		{
			get
			{
				return this.maxMovingDistance;
			}
			set
			{
				if (value < 0.0)
				{
					throw new InvalidOperationException(SR.ExceptionSmartLabelsMaxMovingDistanceIsNegative);
				}
				this.maxMovingDistance = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[SRDescription("DescriptionAttributeAllowOutsidePlotArea")]
		[Bindable(true)]
		[DefaultValue(LabelOutsidePlotAreaStyle.Partial)]
		public virtual LabelOutsidePlotAreaStyle AllowOutsidePlotArea
		{
			get
			{
				return this.allowOutsidePlotArea;
			}
			set
			{
				this.allowOutsidePlotArea = value;
				this.Invalidate();
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeMisc")]
		[SRDescription("DescriptionAttributeCalloutStyle3")]
		[DefaultValue(LabelCalloutStyle.Underlined)]
		public virtual LabelCalloutStyle CalloutStyle
		{
			get
			{
				return this.calloutStyle;
			}
			set
			{
				this.calloutStyle = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeCalloutLineColor")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Black")]
		public virtual Color CalloutLineColor
		{
			get
			{
				return this.calloutLineColor;
			}
			set
			{
				this.calloutLineColor = value;
				this.Invalidate();
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeCalloutLineStyle")]
		public virtual ChartDashStyle CalloutLineStyle
		{
			get
			{
				return this.calloutLineStyle;
			}
			set
			{
				this.calloutLineStyle = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeCalloutBackColor")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Transparent")]
		public virtual Color CalloutBackColor
		{
			get
			{
				return this.calloutBackColor;
			}
			set
			{
				this.calloutBackColor = value;
				this.Invalidate();
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeCalloutLineWidth")]
		[DefaultValue(1)]
		public virtual int CalloutLineWidth
		{
			get
			{
				return this.calloutLineWidth;
			}
			set
			{
				this.calloutLineWidth = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeCalloutLineAnchorCap")]
		[Bindable(true)]
		[DefaultValue(LineAnchorCap.Arrow)]
		public virtual LineAnchorCap CalloutLineAnchorCap
		{
			get
			{
				return this.calloutLineAnchorCap;
			}
			set
			{
				this.calloutLineAnchorCap = value;
				this.Invalidate();
			}
		}

		public SmartLabelsStyle()
		{
			this.chartElement = null;
		}

		public SmartLabelsStyle(object chartElement)
		{
			this.chartElement = chartElement;
		}

		private void Invalidate()
		{
			if (this.chartElement != null)
			{
				if (this.chartElement is Series)
				{
					((Series)this.chartElement).Invalidate(false, false);
				}
				else if (this.chartElement is Annotation)
				{
					((Annotation)this.chartElement).Invalidate();
				}
			}
		}
	}
}
