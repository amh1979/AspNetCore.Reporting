using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class LinearMajorTickMark : TickMark
	{
		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
		[DefaultValue(MarkerStyle.Rectangle)]
		[SRDescription("DescriptionAttributeLinearMajorTickMark_Shape")]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearMajorTickMark_Length")]
		[DefaultValue(15f)]
		[NotifyParentProperty(true)]
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

		[SRDescription("DescriptionAttributeLinearMajorTickMark_Width")]
		[NotifyParentProperty(true)]
		[DefaultValue(4f)]
		[SRCategory("CategoryAttribute_Appearance")]
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

		public LinearMajorTickMark()
			: this(null)
		{
		}

		public LinearMajorTickMark(object parent)
			: base(parent)
		{
			this.Shape = MarkerStyle.Rectangle;
			this.Width = 4f;
			this.Length = 15f;
		}
	}
}
