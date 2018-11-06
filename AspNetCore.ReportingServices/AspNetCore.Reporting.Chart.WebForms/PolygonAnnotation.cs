using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributePolygonAnnotation_PolygonAnnotation")]
	internal class PolygonAnnotation : PolylineAnnotation
	{
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		[DefaultValue(LineAnchorCap.None)]
		public override LineAnchorCap StartCap
		{
			get
			{
				return base.StartCap;
			}
			set
			{
				base.StartCap = value;
			}
		}

		[Browsable(false)]
		[DefaultValue(LineAnchorCap.None)]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		[SRCategory("CategoryAttributeAppearance")]
		public override LineAnchorCap EndCap
		{
			get
			{
				return base.EndCap;
			}
			set
			{
				base.EndCap = value;
			}
		}

		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeBackColor8")]
		[NotifyParentProperty(true)]
		[Browsable(true)]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}

		[Browsable(true)]
		[DefaultValue(ChartHatchStyle.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackHatchStyle")]
		[SRCategory("CategoryAttributeAppearance")]
		public override ChartHatchStyle BackHatchStyle
		{
			get
			{
				return base.BackHatchStyle;
			}
			set
			{
				base.BackHatchStyle = value;
			}
		}

		[SRDescription("DescriptionAttributeBackGradientType8")]
		[Browsable(true)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(GradientType.None)]
		public override GradientType BackGradientType
		{
			get
			{
				return base.BackGradientType;
			}
			set
			{
				base.BackGradientType = value;
			}
		}

		[SRDescription("DescriptionAttributePolygonAnnotation_BackGradientEndColor")]
		[Browsable(true)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "")]
		public override Color BackGradientEndColor
		{
			get
			{
				return base.BackGradientEndColor;
			}
			set
			{
				base.BackGradientEndColor = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeMisc")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAnnotationType4")]
		[Bindable(true)]
		public override string AnnotationType
		{
			get
			{
				return "Polygon";
			}
		}

		[DefaultValue(SelectionPointsStyle.Rectangle)]
		[SRCategory("CategoryAttributeAppearance")]
		[ParenthesizePropertyName(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeSelectionPointsStyle3")]
		internal override SelectionPointsStyle SelectionPointsStyle
		{
			get
			{
				return SelectionPointsStyle.Rectangle;
			}
		}

		public PolygonAnnotation()
		{
			base.isPolygon = true;
		}
	}
}
