using System.ComponentModel;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class LinearMinorTickMark : TickMark
	{
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeShape3")]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(MarkerStyle.Rectangle)]
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
		[SRDescription("DescriptionAttributeLength3")]
		[NotifyParentProperty(true)]
		[ValidateBound(0.0, 50.0)]
		[DefaultValue(9f)]
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

		[NotifyParentProperty(true)]
		[DefaultValue(3f)]
		[SRDescription("DescriptionAttributeWidth7")]
		[SRCategory("CategoryAppearance")]
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

		public LinearMinorTickMark()
			: this(null)
		{
		}

		public LinearMinorTickMark(object parent)
			: base(parent, MarkerStyle.Rectangle, 9f, 3f)
		{
		}
	}
}
