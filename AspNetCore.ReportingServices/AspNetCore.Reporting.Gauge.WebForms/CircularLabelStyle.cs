using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class CircularLabelStyle : LinearLabelStyle
	{
		private bool rotateLabels = true;

		private bool allowUpsideDown;

		[DefaultValue(true)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeCircularLabelStyle_RotateLabels")]
		public bool RotateLabels
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeCircularLabelStyle_AllowUpsideDown")]
		[DefaultValue(false)]
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

		[SRDescription("DescriptionAttributeFont3")]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 14pt")]
		public override Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				base.Font = value;
			}
		}

		public CircularLabelStyle()
			: this(null)
		{
		}

		public CircularLabelStyle(object parent)
			: base(parent, new Font("Microsoft Sans Serif", 14f))
		{
		}
	}
}
