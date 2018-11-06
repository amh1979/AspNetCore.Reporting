using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class SpecialPosition : PinMajorTickMark
	{
		private bool enable;

		private float location = 5f;

		[SRDescription("DescriptionAttributeSpecialPosition_Enable")]
		[ParenthesizePropertyName(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(false)]
		[NotifyParentProperty(true)]
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

		[SRDescription("DescriptionAttributeSpecialPosition_Location")]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(5f)]
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
