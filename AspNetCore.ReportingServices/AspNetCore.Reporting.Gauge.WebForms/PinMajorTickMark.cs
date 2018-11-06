using System.ComponentModel;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class PinMajorTickMark : CustomTickMark
	{
		[NotifyParentProperty(true)]
		[DefaultValue(MarkerStyle.Circle)]
		[SRDescription("DescriptionAttributeShape3")]
		[SRCategory("CategoryAppearance")]
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

		[DefaultValue(6f)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeLength3")]
		[NotifyParentProperty(true)]
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

		[SRDescription("DescriptionAttributeWidth7")]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(6f)]
		[NotifyParentProperty(true)]
		[ValidateBound(0.0, 30.0)]
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

		public PinMajorTickMark()
			: this(null)
		{
		}

		public PinMajorTickMark(object parent)
			: base(parent, MarkerStyle.Circle, 6f, 6f)
		{
		}

		public PinMajorTickMark(object parent, MarkerStyle shape, float length, float width)
			: base(parent, shape, length, width)
		{
		}
	}
}
