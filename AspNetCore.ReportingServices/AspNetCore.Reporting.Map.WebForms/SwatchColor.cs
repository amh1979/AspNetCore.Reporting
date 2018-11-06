using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class SwatchColor : NamedElement
	{
		private Color color = Color.White;

		private GradientType gradientType;

		private Color secondaryColor = Color.Empty;

		private MapHatchStyle hatchStyle;

		private bool noData;

		private double fromValue;

		private double toValue;

		private string textValue = "";

		internal bool automaticallyAdded;

		private ColorSwatchPanel owner;

		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeSwatchColor_Color")]
		[DefaultValue(typeof(Color), "White")]
		public Color Color
		{
			get
			{
				return this.color;
			}
			set
			{
				this.color = value;
				this.Invalidate(false);
			}
		}

		//[Editor(typeof(GradientEditor), typeof(UITypeEditor))]
		[DefaultValue(GradientType.None)]
		[SRDescription("DescriptionAttributeSwatchColor_GradientType")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		public GradientType GradientType
		{
			get
			{
				return this.gradientType;
			}
			set
			{
				this.gradientType = value;
				this.Invalidate(false);
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeSwatchColor_SecondaryColor")]
		[NotifyParentProperty(true)]
		public Color SecondaryColor
		{
			get
			{
				return this.secondaryColor;
			}
			set
			{
				this.secondaryColor = value;
				this.Invalidate(false);
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		//[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		[NotifyParentProperty(true)]
		[DefaultValue(MapHatchStyle.None)]
		[SRDescription("DescriptionAttributeSwatchColor_HatchStyle")]
		public MapHatchStyle HatchStyle
		{
			get
			{
				return this.hatchStyle;
			}
			set
			{
				this.hatchStyle = value;
				this.Invalidate(false);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSwatchColor_NoData")]
		[DefaultValue(false)]
		public bool NoData
		{
			get
			{
				return this.noData;
			}
			set
			{
				this.noData = value;
				this.Invalidate(true);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Data")]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeSwatchColor_FromValue")]
		public double FromValue
		{
			get
			{
				return this.fromValue;
			}
			set
			{
				this.fromValue = value;
				this.Invalidate(true);
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeSwatchColor_ToValue")]
		[DefaultValue(0.0)]
		public double ToValue
		{
			get
			{
				return this.toValue;
			}
			set
			{
				this.toValue = value;
				this.Invalidate(true);
			}
		}

		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSwatchColor_TextValue")]
		[NotifyParentProperty(true)]
		public string TextValue
		{
			get
			{
				return this.textValue;
			}
			set
			{
				this.textValue = value;
				this.Invalidate(true);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public bool HasTextValue
		{
			get
			{
				return !string.IsNullOrEmpty(this.TextValue);
			}
		}

		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeSwatchColor_AutomaticallyAdded")]
		[DefaultValue(false)]
		[NotifyParentProperty(true)]
		public bool AutomaticallyAdded
		{
			get
			{
				return this.automaticallyAdded;
			}
			set
			{
				this.automaticallyAdded = value;
			}
		}

		internal ColorSwatchPanel Owner
		{
			get
			{
				return this.owner;
			}
			set
			{
				this.owner = value;
			}
		}

		public SwatchColor()
			: this(null, "", 0.0, 0.0, "")
		{
		}

		public SwatchColor(string name)
			: this(null, name, 0.0, 0.0, "")
		{
		}

		public SwatchColor(string name, double fromValue, double toValue)
			: this(null, name, fromValue, toValue, "")
		{
		}

		public SwatchColor(string name, string textValue)
			: this(null, name, 0.0, 0.0, textValue)
		{
		}

		public SwatchColor(string name, double fromValue, double toValue, string textValue)
			: this(null, name, fromValue, toValue, textValue)
		{
		}

		internal SwatchColor(CommonElements common, string name, double fromValue, double toValue, string textValue)
			: base(common)
		{
			this.Name = name;
			this.fromValue = fromValue;
			this.toValue = toValue;
			this.textValue = textValue;
		}

		private void Invalidate(bool layout)
		{
			if (layout)
			{
				base.InvalidateAndLayout();
			}
			else
			{
				base.Invalidate();
			}
		}
	}
}
