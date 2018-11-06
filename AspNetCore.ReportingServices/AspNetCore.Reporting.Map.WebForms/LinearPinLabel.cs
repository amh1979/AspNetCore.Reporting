using System;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class LinearPinLabel : MapObject
	{
		private string text = "";

		private Font font = new Font("Microsoft Sans Serif", 12f);

		private FontUnit fontUnit;

		private Color textColor = Color.Black;

		private Placement placement;

		private float fontAngle;

		private float scaleOffset = 2f;

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLinearPinLabel_Text")]
		[DefaultValue("")]
		[Localizable(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLinearPinLabel_Font")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 12pt")]
		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
		public Font Font
		{
			get
			{
				return this.font;
			}
			set
			{
				this.font = value;
				this.Invalidate();
			}
		}

		[DefaultValue(FontUnit.Percent)]
		[SRDescription("DescriptionAttributeLinearPinLabel_FontUnit")]
		[SRCategory("CategoryAttribute_Appearance")]
		public FontUnit FontUnit
		{
			get
			{
				return this.fontUnit;
			}
			set
			{
				this.fontUnit = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearPinLabel_TextColor")]
		[DefaultValue(typeof(Color), "Black")]
		[NotifyParentProperty(true)]
		public Color TextColor
		{
			get
			{
				return this.textColor;
			}
			set
			{
				this.textColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(Placement.Inside)]
		[SRDescription("DescriptionAttributeLinearPinLabel_Placement")]
		[SRCategory("CategoryAttribute_Appearance")]
		public Placement Placement
		{
			get
			{
				return this.placement;
			}
			set
			{
				this.placement = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLinearPinLabel_FontAngle")]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(0f)]
		public float FontAngle
		{
			get
			{
				return this.fontAngle;
			}
			set
			{
				if (!(value > 360.0) && !(value < 0.0))
				{
					this.fontAngle = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentOutOfRangeException(SR.out_of_range(0.0, 360.0));
			}
		}

		[DefaultValue(2f)]
		[SRDescription("DescriptionAttributeLinearPinLabel_DistanceFromScale")]
		[SRCategory("CategoryAttribute_Appearance")]
		public float DistanceFromScale
		{
			get
			{
				return this.scaleOffset;
			}
			set
			{
				if (!(value < -100.0) && !(value > 100.0))
				{
					this.scaleOffset = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
			}
		}

		public LinearPinLabel()
			: this(null)
		{
		}

		public LinearPinLabel(object parent)
			: base(parent)
		{
		}
	}
}
