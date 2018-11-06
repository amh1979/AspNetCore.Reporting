using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(CustomColorConverter))]
	internal class CustomColor : NamedElement
	{
		private Color borderColor = Color.DarkGray;

		private Color color = Color.Green;

		private Color secondaryColor = Color.Empty;

		private GradientType gradientType;

		private MapHatchStyle hatchStyle;

		private string fromValue = string.Empty;

		private string toValue = "";

		private string legendText = "";

		private string text = "#NAME";

		private string toolTip = "";

		private ArrayList affectedElements;

		private string fromValueInt = string.Empty;

		private string toValueInt = "";

		private bool visibleInt = true;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object Tag
		{
			get
			{
				return base.Tag;
			}
			set
			{
				base.Tag = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(typeof(Color), "DarkGray")]
		[SRDescription("DescriptionAttributeCustomColor_BorderColor")]
		public Color BorderColor
		{
			get
			{
				return this.borderColor;
			}
			set
			{
				this.borderColor = value;
				this.InvalidateRules();
			}
		}

		[DefaultValue(typeof(Color), "Green")]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeCustomColor_Color")]
		public Color Color
		{
			get
			{
				return this.color;
			}
			set
			{
				this.color = value;
				this.InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeCustomColor_SecondaryColor")]
		[DefaultValue(typeof(Color), "")]
		public Color SecondaryColor
		{
			get
			{
				return this.secondaryColor;
			}
			set
			{
				this.secondaryColor = value;
				this.InvalidateRules();
			}
		}

		[SRDescription("DescriptionAttributeCustomColor_GradientType")]
		[Editor(typeof(GradientEditor), typeof(UITypeEditor))]
		[DefaultValue(GradientType.None)]
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
				this.InvalidateRules();
			}
		}

		[DefaultValue(MapHatchStyle.None)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeCustomColor_HatchStyle")]
		[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		public MapHatchStyle HatchStyle
		{
			get
			{
				return this.hatchStyle;
			}
			set
			{
				this.hatchStyle = value;
				this.InvalidateRules();
			}
		}

		[SRDescription("DescriptionAttributeCustomColor_FromValue")]
		[TypeConverter(typeof(StringConverter))]
		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Values")]
		public string FromValue
		{
			get
			{
				return this.fromValue;
			}
			set
			{
				this.fromValue = value;
				this.InvalidateRules();
			}
		}

		[SRDescription("DescriptionAttributeCustomColor_ToValue")]
		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Values")]
		[TypeConverter(typeof(StringConverter))]
		public string ToValue
		{
			get
			{
				return this.toValue;
			}
			set
			{
				this.toValue = value;
				this.InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeCustomColor_LegendText")]
		public string LegendText
		{
			get
			{
				return this.legendText;
			}
			set
			{
				this.legendText = value;
				this.InvalidateRules();
			}
		}

		[DefaultValue("#NAME")]
		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeCustomColor_Text")]
		[Localizable(true)]
		[TypeConverter(typeof(KeywordConverter))]
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
				this.InvalidateRules();
			}
		}

		[TypeConverter(typeof(KeywordConverter))]
		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeCustomColor_ToolTip")]
		[Localizable(true)]
		public string ToolTip
		{
			get
			{
				return this.toolTip;
			}
			set
			{
				this.toolTip = value;
				this.InvalidateRules();
			}
		}

		internal ArrayList AffectedElements
		{
			get
			{
				return this.affectedElements;
			}
			set
			{
				this.affectedElements = value;
			}
		}

		internal string FromValueInt
		{
			get
			{
				if (string.IsNullOrEmpty(this.fromValue))
				{
					return this.fromValueInt;
				}
				return this.fromValue;
			}
			set
			{
				this.fromValueInt = value;
			}
		}

		internal string ToValueInt
		{
			get
			{
				if (string.IsNullOrEmpty(this.toValue))
				{
					return this.toValueInt;
				}
				return this.toValue;
			}
			set
			{
				this.toValueInt = value;
			}
		}

		public bool VisibleInt
		{
			get
			{
				return this.visibleInt;
			}
			set
			{
				this.visibleInt = value;
			}
		}

		public CustomColor()
			: this(null)
		{
		}

		internal CustomColor(CommonElements common)
			: base(common)
		{
			this.affectedElements = new ArrayList();
		}

		public override string ToString()
		{
			return this.Name;
		}

		public ArrayList GetAffectedElements()
		{
			return this.AffectedElements;
		}

		internal RuleBase GetRule()
		{
			return (RuleBase)this.ParentElement;
		}

		internal override void OnAdded()
		{
			base.OnAdded();
			this.InvalidateRules();
		}

		internal override void OnRemove()
		{
			base.OnRemove();
			this.InvalidateRules();
		}

		internal void InvalidateRules()
		{
			MapCore mapCore = this.GetMapCore();
			if (mapCore != null)
			{
				mapCore.InvalidateRules();
				mapCore.Invalidate();
			}
		}

		internal MapCore GetMapCore()
		{
			RuleBase rule = this.GetRule();
			if (rule != null)
			{
				return rule.GetMapCore();
			}
			return null;
		}
	}
}
