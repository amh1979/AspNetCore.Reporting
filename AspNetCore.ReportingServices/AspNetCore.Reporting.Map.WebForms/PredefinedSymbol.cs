using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(PredefinedSymbolConverter))]
	internal class PredefinedSymbol : NamedElement
	{
		private TextAlignment textAlignment = TextAlignment.Bottom;

		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private Color borderColor = Color.DarkGray;

		private MapDashStyle borderStyle = MapDashStyle.Solid;

		private int borderWidth = 1;

		private Color color = Color.Red;

		private Color textColor = Color.Black;

		private Color secondaryColor = Color.Empty;

		private GradientType gradientType;

		private MapHatchStyle hatchStyle;

		private string fromValue = string.Empty;

		private string toValue = "";

		private string legendText = "";

		private string text = "";

		private int shadowOffset;

		private int textShadowOffset;

		private MarkerStyle markerStyle = MarkerStyle.Circle;

		private float width = 7f;

		private float height = 7f;

		private ResizeMode imageResizeMode = ResizeMode.AutoFit;

		private string image = "";

		private Color imageTransColor = Color.Empty;

		private string category = string.Empty;

		private string toolTip = "";

		private bool visible = true;

		private ArrayList affectedSymbols;

		private string fromValueInt = string.Empty;

		private string toValueInt = "";

		private bool visibleInt = true;

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributePredefinedSymbol_Name")]
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

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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

		[SRCategory("CategoryAttribute_SymbolText")]
		[SRDescription("DescriptionAttributePredefinedSymbol_TextAlignment")]
		[DefaultValue(TextAlignment.Bottom)]
		public TextAlignment TextAlignment
		{
			get
			{
				return this.textAlignment;
			}
			set
			{
				this.textAlignment = value;
				this.InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_SymbolText")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8.25pt")]
		[SRDescription("DescriptionAttributePredefinedSymbol_Font")]
		public Font Font
		{
			get
			{
				return this.font;
			}
			set
			{
				this.font = value;
				this.InvalidateRules();
			}
		}

		[DefaultValue(typeof(Color), "DarkGray")]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePredefinedSymbol_BorderColor")]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(MapDashStyle.Solid)]
		[SRDescription("DescriptionAttributePredefinedSymbol_BorderStyle")]
		public MapDashStyle BorderStyle
		{
			get
			{
				return this.borderStyle;
			}
			set
			{
				this.borderStyle = value;
				this.InvalidateRules();
			}
		}

		[DefaultValue(1)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePredefinedSymbol_BorderWidth")]
		public int BorderWidth
		{
			get
			{
				return this.borderWidth;
			}
			set
			{
				if (value >= 0 && value <= 100)
				{
					this.borderWidth = value;
					this.InvalidateRules();
					return;
				}
				throw new ArgumentException(SR.must_in_range(0.0, 100.0));
			}
		}

		[SRDescription("DescriptionAttributePredefinedSymbol_Color")]
		[DefaultValue(typeof(Color), "Red")]
		[SRCategory("CategoryAttribute_Appearance")]
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

		[DefaultValue(typeof(Color), "Black")]
		[SRCategory("CategoryAttribute_SymbolText")]
		[SRDescription("DescriptionAttributePredefinedSymbol_TextColor")]
		public Color TextColor
		{
			get
			{
				return this.textColor;
			}
			set
			{
				this.textColor = value;
				this.InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributePredefinedSymbol_SecondaryColor")]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(GradientType.None)]
		[SRDescription("DescriptionAttributePredefinedSymbol_GradientType")]
		//[Editor(typeof(GradientEditor), typeof(UITypeEditor))]
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
		[SRDescription("DescriptionAttributePredefinedSymbol_HatchStyle")]
		//[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
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

		[SRDescription("DescriptionAttributePredefinedSymbol_FromValue")]
		[SRCategory("CategoryAttribute_Values")]
		[DefaultValue("")]
		[TypeConverter(typeof(StringConverter))]
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

		[SRCategory("CategoryAttribute_Values")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributePredefinedSymbol_ToValue")]
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

		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePredefinedSymbol_LegendText")]
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

		[SRDescription("DescriptionAttributePredefinedSymbol_Text")]
		[SRCategory("CategoryAttribute_Behavior")]
		[Localizable(true)]
		[TypeConverter(typeof(KeywordConverter))]
		[DefaultValue("")]
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

		[DefaultValue(0)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePredefinedSymbol_ShadowOffset")]
		public int ShadowOffset
		{
			get
			{
				return this.shadowOffset;
			}
			set
			{
				if (value >= -100 && value <= 100)
				{
					this.shadowOffset = value;
					this.InvalidateRules();
					return;
				}
				throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
			}
		}

		[SRCategory("CategoryAttribute_SymbolText")]
		[SRDescription("DescriptionAttributePredefinedSymbol_TextShadowOffset")]
		[DefaultValue(0)]
		public int TextShadowOffset
		{
			get
			{
				return this.textShadowOffset;
			}
			set
			{
				if (value >= -100 && value <= 100)
				{
					this.textShadowOffset = value;
					this.InvalidateRules();
					return;
				}
				throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(MarkerStyle.Circle)]
		[SRDescription("DescriptionAttributePredefinedSymbol_MarkerStyle")]
		public MarkerStyle MarkerStyle
		{
			get
			{
				return this.markerStyle;
			}
			set
			{
				this.markerStyle = value;
				this.InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Size")]
		[DefaultValue(7f)]
		[SRDescription("DescriptionAttributePredefinedSymbol_Width")]
		public float Width
		{
			get
			{
				return this.width;
			}
			set
			{
				if (!(value < 0.0) && !(value > 1000.0))
				{
					this.width = value;
					this.InvalidateRules();
					return;
				}
				throw new ArgumentException(SR.must_in_range(0.0, 1000.0));
			}
		}

		[DefaultValue(7f)]
		[SRCategory("CategoryAttribute_Size")]
		[SRDescription("DescriptionAttributePredefinedSymbol_Height")]
		public float Height
		{
			get
			{
				return this.height;
			}
			set
			{
				if (!(value < 0.0) && !(value > 1000.0))
				{
					this.height = value;
					this.InvalidateRules();
					return;
				}
				throw new ArgumentException(SR.must_in_range(0.0, 1000.0));
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[DefaultValue(ResizeMode.AutoFit)]
		[SRDescription("DescriptionAttributePredefinedSymbol_ImageResizeMode")]
		public ResizeMode ImageResizeMode
		{
			get
			{
				return this.imageResizeMode;
			}
			set
			{
				this.imageResizeMode = value;
				this.InvalidateRules();
			}
		}

		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributePredefinedSymbol_Image")]
		public string Image
		{
			get
			{
				return this.image;
			}
			set
			{
				this.image = value;
				this.InvalidateRules();
			}
		}

		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributePredefinedSymbol_ImageTransColor")]
		public Color ImageTransColor
		{
			get
			{
				return this.imageTransColor;
			}
			set
			{
				this.imageTransColor = value;
				this.InvalidateRules();
			}
		}

		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributePredefinedSymbol_Category")]
		public string Category
		{
			get
			{
				return this.category;
			}
			set
			{
				this.category = value;
				this.InvalidateRules();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributePredefinedSymbol_ToolTip")]
		[Localizable(true)]
		[TypeConverter(typeof(KeywordConverter))]
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

		[SRDescription("DescriptionAttributePredefinedSymbol_Visible")]
		[SRCategory("CategoryAttribute_Appearance")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(true)]
		public bool Visible
		{
			get
			{
				if (this.visible)
				{
					return this.VisibleInt;
				}
				return false;
			}
			set
			{
				this.visible = value;
				this.InvalidateRules();
			}
		}

		internal ArrayList AffectedSymbols
		{
			get
			{
				return this.affectedSymbols;
			}
			set
			{
				this.affectedSymbols = value;
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

		internal bool VisibleInt
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

		public PredefinedSymbol()
			: this(null)
		{
		}

		internal PredefinedSymbol(CommonElements common)
			: base(common)
		{
			this.affectedSymbols = new ArrayList();
		}

		public override string ToString()
		{
			return this.Name;
		}

		public ArrayList GetAffectedSymbols()
		{
			return this.AffectedSymbols;
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
