using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeEllipseAnnotation_EllipseAnnotation")]
	internal class EllipseAnnotation : RectangleAnnotation
	{
		[Browsable(false)]
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAnnotationType4")]
		public override string AnnotationType
		{
			get
			{
				return "Ellipse";
			}
		}

		public EllipseAnnotation()
		{
			base.isEllipse = true;
		}
	}
}
