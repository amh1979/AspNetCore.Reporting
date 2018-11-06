using System.ComponentModel;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class CircularMajorTickMark : TickMark
	{
		[SRCategory("CategoryAppearance")]
		[DefaultValue(MarkerStyle.Trapezoid)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeShape3")]
		public override MarkerStyle Shape
		{
			get
			{
				return base.Shape;
			}
			set
			{
				base.Shape = value;
			}
		}

		[SRCategory("CategoryAppearance")]
		[NotifyParentProperty(true)]
		[DefaultValue(14f)]
		[SRDescription("DescriptionAttributeLength3")]
		[ValidateBound(0.0, 50.0)]
		public override float Length
		{
			get
			{
				return base.Length;
			}
			set
			{
				base.Length = value;
			}
		}

		[DefaultValue(8f)]
		[NotifyParentProperty(true)]
		[ValidateBound(0.0, 30.0)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeWidth7")]
		public override float Width
		{
			get
			{
				return base.Width;
			}
			set
			{
				base.Width = value;
			}
		}

		public CircularMajorTickMark()
			: this(null)
		{
		}

		public CircularMajorTickMark(object parent)
			: base(parent, MarkerStyle.Trapezoid, 14f, 8f)
		{
		}
	}
}
