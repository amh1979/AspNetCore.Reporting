using System.ComponentModel;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class SpecialPosition : PinMajorTickMark
	{
		private bool enable;

		private float location = 5f;

		[SRCategory("CategoryAppearance")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeEnable")]
		[NotifyParentProperty(true)]
		[ParenthesizePropertyName(true)]
		public bool Enable
		{
			get
			{
				return this.enable;
			}
			set
			{
				this.enable = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLocation6")]
		[DefaultValue(5f)]
		[ValidateBound(-50.0, 50.0)]
		[SRCategory("CategoryAppearance")]
		public virtual float Location
		{
			get
			{
				return this.location;
			}
			set
			{
				this.location = value;
				this.Invalidate();
			}
		}

		public SpecialPosition()
			: this(null)
		{
		}

		public SpecialPosition(object parent)
			: base(parent)
		{
		}
	}
}
