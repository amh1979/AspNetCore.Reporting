using System.ComponentModel;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class CircularPinLabel : LinearPinLabel
	{
		private bool rotateLabels;

		private bool allowUpsideDown;

		[SRCategory("CategoryAppearance")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeCircularPinLabel_RotateLabel")]
		public bool RotateLabel
		{
			get
			{
				return this.rotateLabels;
			}
			set
			{
				this.rotateLabels = value;
				this.Invalidate();
			}
		}

		[DefaultValue(false)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeCircularPinLabel_AllowUpsideDown")]
		public bool AllowUpsideDown
		{
			get
			{
				return this.allowUpsideDown;
			}
			set
			{
				this.allowUpsideDown = value;
				this.Invalidate();
			}
		}

		public CircularPinLabel()
			: this(null)
		{
		}

		public CircularPinLabel(object parent)
			: base(parent)
		{
		}
	}
}
