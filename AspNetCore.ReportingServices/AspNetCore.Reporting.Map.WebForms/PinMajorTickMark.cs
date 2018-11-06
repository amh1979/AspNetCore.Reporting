using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class PinMajorTickMark : CustomTickMark
	{
		[DefaultValue(MarkerStyle.Circle)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePinMajorTickMark_Shape")]
		[NotifyParentProperty(true)]
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

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(6f)]
		[SRDescription("DescriptionAttributePinMajorTickMark_Length")]
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

		[SRDescription("DescriptionAttributePinMajorTickMark_Width")]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(6f)]
		[NotifyParentProperty(true)]
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
			: base(parent)
		{
			this.Shape = MarkerStyle.Circle;
			this.Width = 6f;
			this.Length = 6f;
		}

		public PinMajorTickMark(object parent, MarkerStyle shape, float length, float width)
			: base(parent, shape, length, width)
		{
		}
	}
}
