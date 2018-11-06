using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace AspNetCore.Reporting.Map.WebForms
{
	[Description("Drawing attributes for the 3D frames.")]
	[TypeConverter(typeof(FrameAttributesConverter))]
	[DefaultProperty("FrameStyle")]
	internal class Frame : MapObject
	{
		private Color pageColor = Color.White;

		private FrameStyle frameStyle;

		private GradientType backGradientType;

		private Color backSecondaryColor = Color.Empty;

		private Color backColor = Color.Gray;

		private string backImage = "";

		private MapImageWrapMode backImageMode;

		private Color backImageTranspColor = Color.Empty;

		private MapImageAlign backImageAlign;

		private Color borderColor = Color.DarkGray;

		private int borderWidth = 1;

		private MapDashStyle borderStyle;

		private MapHatchStyle backHatchStyle;

		internal object ownerElement;

		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "White")]
		[SRDescription("DescriptionAttributeFrame_PageColor")]
		public Color PageColor
		{
			get
			{
				return this.pageColor;
			}
			set
			{
				this.pageColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(FrameStyle.None)]
		[RefreshProperties(RefreshProperties.All)]
		[ParenthesizePropertyName(true)]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeFrame_FrameStyle")]
		public FrameStyle FrameStyle
		{
			get
			{
				return this.frameStyle;
			}
			set
			{
				this.frameStyle = value;
				this.InvalidateAndLayout();
			}
		}

		[Browsable(false)]
		[Bindable(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(typeof(Color), "Gray")]
		[SRDescription("DescriptionAttributeFrame_BackColor")]
		[NotifyParentProperty(true)]
		public Color BackColor
		{
			get
			{
				return this.backColor;
			}
			set
			{
				this.backColor = value;
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeFrame_BorderColor")]
		[DefaultValue(typeof(Color), "DarkGray")]
		[Bindable(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[Browsable(false)]
		public Color BorderColor
		{
			get
			{
				return this.borderColor;
			}
			set
			{
				this.borderColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(MapHatchStyle.None)]
		[SRDescription("DescriptionAttributeFrame_BackHatchStyle")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[Browsable(false)]
		[Bindable(true)]
		[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		public MapHatchStyle BackHatchStyle
		{
			get
			{
				return this.backHatchStyle;
			}
			set
			{
				this.backHatchStyle = value;
				this.Invalidate();
			}
		}

		[Bindable(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeFrame_BackImage")]
		[Browsable(false)]
		[NotifyParentProperty(true)]
		public string BackImage
		{
			get
			{
				return this.backImage;
			}
			set
			{
				this.backImage = value;
				this.Invalidate();
			}
		}

		[DefaultValue(MapImageWrapMode.Tile)]
		[SRCategory("CategoryAttribute_Appearance")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeFrame_BackImageMode")]
		[Bindable(true)]
		public MapImageWrapMode BackImageMode
		{
			get
			{
				return this.backImageMode;
			}
			set
			{
				this.backImageMode = value;
				this.Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeFrame_BackImageTranspColor")]
		[DefaultValue(typeof(Color), "")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Color BackImageTranspColor
		{
			get
			{
				return this.backImageTranspColor;
			}
			set
			{
				this.backImageTranspColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(MapImageAlign.TopLeft)]
		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeFrame_BackImageAlign")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Bindable(true)]
		public MapImageAlign BackImageAlign
		{
			get
			{
				return this.backImageAlign;
			}
			set
			{
				this.backImageAlign = value;
				this.Invalidate();
			}
		}

		[DefaultValue(GradientType.None)]
		[Editor(typeof(GradientEditor), typeof(UITypeEditor))]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeFrame_BackGradientType")]
		[NotifyParentProperty(true)]
		[Bindable(true)]
		public GradientType BackGradientType
		{
			get
			{
				return this.backGradientType;
			}
			set
			{
				this.backGradientType = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[Browsable(false)]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeFrame_BackSecondaryColor")]
		public Color BackSecondaryColor
		{
			get
			{
				return this.backSecondaryColor;
			}
			set
			{
				this.backSecondaryColor = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeFrame_BorderWidth")]
		[Browsable(false)]
		[NotifyParentProperty(true)]
		[DefaultValue(1)]
		[Bindable(true)]
		public int BorderWidth
		{
			get
			{
				return this.borderWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("BorderWidth", SR.ExceptionBorderWidthMustBeGreaterThanZero);
				}
				this.borderWidth = value;
				this.Invalidate();
			}
		}

		[Bindable(true)]
		[DefaultValue(MapDashStyle.None)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeFrame_BorderStyle")]
		[NotifyParentProperty(true)]
		[Browsable(false)]
		public MapDashStyle BorderStyle
		{
			get
			{
				return this.borderStyle;
			}
			set
			{
				this.borderStyle = value;
				this.Invalidate();
			}
		}

		public Frame()
			: this(null)
		{
		}

		public Frame(object parent)
			: base(parent)
		{
		}

		internal MapCore GetMapCore()
		{
			return (MapCore)this.Parent;
		}

		internal override void Invalidate()
		{
			MapCore mapCore = this.GetMapCore();
			if (mapCore != null)
			{
				mapCore.Invalidate();
			}
		}

		internal void InvalidateAndLayout()
		{
			MapCore mapCore = this.GetMapCore();
			if (mapCore != null)
			{
				mapCore.InvalidateAndLayout();
			}
		}

		internal bool ShouldRenderReadOnly()
		{
			if (this.FrameStyle != 0 && this.FrameStyle != FrameStyle.Raised && this.FrameStyle != FrameStyle.Sunken)
			{
				return this.FrameStyle == FrameStyle.Emboss;
			}
			return true;
		}
	}
}
