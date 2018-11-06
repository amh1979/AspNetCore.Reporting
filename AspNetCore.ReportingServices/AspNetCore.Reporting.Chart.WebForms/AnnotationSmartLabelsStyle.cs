using AspNetCore.Reporting.Chart.WebForms.Design;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[DefaultProperty("Enabled")]
	[TypeConverter(typeof(NoNameExpandableObjectConverter))]
	[SRDescription("DescriptionAttributeAnnotationSmartLabelsStyle_AnnotationSmartLabelsStyle")]
	internal class AnnotationSmartLabelsStyle : SmartLabelsStyle
	{
		[SRCategory("CategoryAttributeMisc")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(LabelCalloutStyle.Underlined)]
		[SRDescription("DescriptionAttributeCalloutStyle3")]
		[Browsable(false)]
		public override LabelCalloutStyle CalloutStyle
		{
			get
			{
				return base.CalloutStyle;
			}
			set
			{
				base.CalloutStyle = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeCalloutLineColor")]
		[SRCategory("CategoryAttributeAppearance")]
		public override Color CalloutLineColor
		{
			get
			{
				return base.CalloutLineColor;
			}
			set
			{
				base.CalloutLineColor = value;
			}
		}

		[DefaultValue(ChartDashStyle.Solid)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeCalloutLineStyle")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override ChartDashStyle CalloutLineStyle
		{
			get
			{
				return base.CalloutLineStyle;
			}
			set
			{
				base.CalloutLineStyle = value;
			}
		}

		[DefaultValue(typeof(Color), "Transparent")]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeCalloutBackColor")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Color CalloutBackColor
		{
			get
			{
				return base.CalloutBackColor;
			}
			set
			{
				base.CalloutBackColor = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeCalloutLineWidth")]
		[SRCategory("CategoryAttributeAppearance")]
		public override int CalloutLineWidth
		{
			get
			{
				return base.CalloutLineWidth;
			}
			set
			{
				base.CalloutLineWidth = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(LineAnchorCap.Arrow)]
		[SRDescription("DescriptionAttributeCalloutLineAnchorCap")]
		[Browsable(false)]
		public override LineAnchorCap CalloutLineAnchorCap
		{
			get
			{
				return base.CalloutLineAnchorCap;
			}
			set
			{
				base.CalloutLineAnchorCap = value;
			}
		}

		public AnnotationSmartLabelsStyle()
		{
			base.chartElement = null;
		}

		public AnnotationSmartLabelsStyle(object chartElement)
			: base(chartElement)
		{
		}
	}
}
