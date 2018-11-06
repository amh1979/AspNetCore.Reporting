using System.Collections;
using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(CustomWidthConverter))]
	internal class CustomWidth : NamedElement
	{
		private float width = 5f;

		private string fromValue = string.Empty;

		private string toValue = "";

		private string legendText = "";

		private string text = "#NAME";

		private string toolTip = "";

		private ArrayList affectedElements;

		private string fromValueInt = string.Empty;

		private string toValueInt = "";

		private bool visibleInt = true;

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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

		[SRDescription("DescriptionAttributeCustomWidth_Width")]
		[DefaultValue(5f)]
		[SRCategory("CategoryAttribute_Appearance")]
		public float Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
				this.InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Values")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeCustomWidth_FromValue")]
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

		[DefaultValue("")]
		[TypeConverter(typeof(StringConverter))]
		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributeCustomWidth_ToValue")]
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

		[SRDescription("DescriptionAttributeCustomWidth_LegendText")]
		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Appearance")]
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

		[SRCategory("CategoryAttribute_Misc")]
		[Localizable(true)]
		[DefaultValue("#NAME")]
		[SRDescription("DescriptionAttributeCustomWidth_Text")]
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

		[SRDescription("DescriptionAttributeCustomWidth_ToolTip")]
		[Localizable(true)]
		[DefaultValue("")]
		[TypeConverter(typeof(KeywordConverter))]
		[SRCategory("CategoryAttribute_Misc")]
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

		public CustomWidth()
			: this(null)
		{
		}

		internal CustomWidth(CommonElements common)
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
