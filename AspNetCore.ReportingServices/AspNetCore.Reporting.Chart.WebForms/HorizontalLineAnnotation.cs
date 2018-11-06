using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeHorizontalLineAnnotation_HorizontalLineAnnotation")]
	internal class HorizontalLineAnnotation : LineAnnotation
	{
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAnnotationType4")]
		public override string AnnotationType
		{
			get
			{
				return "HorizontalLine";
			}
		}

		internal override void AdjustLineCoordinates(ref PointF point1, ref PointF point2, ref RectangleF selectionRect)
		{
			point2.Y = point1.Y;
			selectionRect.Height = 0f;
			base.AdjustLineCoordinates(ref point1, ref point2, ref selectionRect);
		}

		internal override RectangleF GetContentPosition()
		{
			return new RectangleF(float.NaN, float.NaN, float.NaN, 0f);
		}
	}
}
