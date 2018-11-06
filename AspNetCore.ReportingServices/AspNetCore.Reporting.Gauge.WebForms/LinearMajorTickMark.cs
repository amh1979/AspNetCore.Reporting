using System.ComponentModel;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class LinearMajorTickMark : TickMark
	{
		[SRCategory("CategoryAppearance")]
		[DefaultValue(MarkerStyle.Rectangle)]
		[SRDescription("DescriptionAttributeShape3")]
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
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeLength3")]
		[DefaultValue(15f)]
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

		[DefaultValue(4f)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeWidth7")]
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

		public LinearMajorTickMark()
			: this(null)
		{
		}

		public LinearMajorTickMark(object parent)
			: base(parent, MarkerStyle.Rectangle, 15f, 4f)
		{
		}
	}
}
