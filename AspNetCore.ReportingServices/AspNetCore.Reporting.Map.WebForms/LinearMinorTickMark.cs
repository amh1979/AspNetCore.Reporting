using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class LinearMinorTickMark : TickMark
	{
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(MarkerStyle.Rectangle)]
		[SRDescription("DescriptionAttributeLinearMinorTickMark_Shape")]
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

		[SRDescription("DescriptionAttributeLinearMinorTickMark_Length")]
		[NotifyParentProperty(true)]
		[DefaultValue(9f)]
		[SRCategory("CategoryAttribute_Appearance")]
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

		[DefaultValue(3f)]
		[SRDescription("DescriptionAttributeLinearMinorTickMark_Width")]
		[SRCategory("CategoryAttribute_Appearance")]
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

		public LinearMinorTickMark()
			: this(null)
		{
		}

		public LinearMinorTickMark(object parent)
			: base(parent)
		{
			this.Shape = MarkerStyle.Rectangle;
			this.Width = 3f;
			this.Length = 9f;
		}
	}
}
