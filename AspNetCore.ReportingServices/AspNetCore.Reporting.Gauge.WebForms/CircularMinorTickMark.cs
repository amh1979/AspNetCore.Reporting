using System.ComponentModel;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class CircularMinorTickMark : TickMark
	{
		[DefaultValue(MarkerStyle.Rectangle)]
		[SRCategory("CategoryAppearance")]
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
		[DefaultValue(8f)]
		[SRDescription("DescriptionAttributeLength3")]
		[SRCategory("CategoryAppearance")]
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

		[DefaultValue(3f)]
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

		public CircularMinorTickMark()
			: this(null)
		{
		}

		public CircularMinorTickMark(object parent)
			: base(parent, MarkerStyle.Rectangle, 8f, 3f)
		{
		}
	}
}
