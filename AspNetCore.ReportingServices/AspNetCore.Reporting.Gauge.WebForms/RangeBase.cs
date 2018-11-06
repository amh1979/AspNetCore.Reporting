using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal abstract class RangeBase : Range, IToolTipProvider, IImageMapProvider
	{
		private Placement placement;

		private float distanceFromScale = 10f;

		private string scaleName = "Default";

		private Color inRangeTickMarkColor = Color.Empty;

		private Color inRangeLabelColor = Color.Empty;

		private Color inRangeBarPointerColor = Color.Empty;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private bool visible = true;

		private Color borderColor = Color.Black;

		private float shadowOffset;

		private GaugeDashStyle borderStyle = GaugeDashStyle.Solid;

		private int borderWidth = 1;

		private Color fillColor = Color.Lime;

		private RangeGradientType fillGradientType = RangeGradientType.StartToEnd;

		private Color fillGradientEndColor = Color.Red;

		private GaugeHatchStyle fillHatchStyle;

		private bool selected;

		private float startWidth;

		private float endWidth;

		private object imageMapProviderTag;

		[SRDescription("DescriptionAttributeName13")]
		[SRCategory("CategoryMisc")]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeRangeBase_Placement")]
		public virtual Placement Placement
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

		[ValidateBound(0.0, 100.0)]
		[SRDescription("DescriptionAttributeRangeBase_DistanceFromScale")]
		[DefaultValue(10f)]
		[SRCategory("CategoryLayout")]
		public virtual float DistanceFromScale
		{
			get
			{
				return this.distanceFromScale;
			}
			set
			{
				this.distanceFromScale = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeScaleName3")]
		[DefaultValue("Default")]
		[TypeConverter(typeof(ScaleSourceConverter))]
		[SRCategory("CategoryMisc")]
		public string ScaleName
		{
			get
			{
				return this.scaleName;
			}
			set
			{
				this.scaleName = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeInRangeTickMarkColor")]
		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryBehavior")]
		public Color InRangeTickMarkColor
		{
			get
			{
				return this.inRangeTickMarkColor;
			}
			set
			{
				this.inRangeTickMarkColor = value;
				base.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeInRangeLabelColor")]
		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryBehavior")]
		public Color InRangeLabelColor
		{
			get
			{
				return this.inRangeLabelColor;
			}
			set
			{
				this.inRangeLabelColor = value;
				base.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeInRangeBarPointerColor")]
		[SRCategory("CategoryBehavior")]
		public Color InRangeBarPointerColor
		{
			get
			{
				return this.inRangeBarPointerColor;
			}
			set
			{
				this.inRangeBarPointerColor = value;
				base.Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeToolTip9")]
		[Localizable(true)]
		[DefaultValue("")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string ToolTip
		{
			get
			{
				return this.toolTip;
			}
			set
			{
				this.toolTip = value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[Localizable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeHref8")]
		public string Href
		{
			get
			{
				return this.href;
			}
			set
			{
				this.href = value;
			}
		}

		[SRDescription("DescriptionAttributeMapAreaAttributes4")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DefaultValue("")]
		[SRCategory("CategoryBehavior")]
		public string MapAreaAttributes
		{
			get
			{
				return this.mapAreaAttributes;
			}
			set
			{
				this.mapAreaAttributes = value;
			}
		}

		[SRCategory("CategoryAppearance")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeVisible9")]
		public bool Visible
		{
			get
			{
				return this.visible;
			}
			set
			{
				this.visible = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "Black")]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBorderColor4")]
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

		[SRDescription("DescriptionAttributeShadowOffset4")]
		[ValidateBound(-5.0, 5.0)]
		[DefaultValue(0f)]
		[SRCategory("CategoryAppearance")]
		public float ShadowOffset
		{
			get
			{
				return this.shadowOffset;
			}
			set
			{
				if (!(value < -100.0) && !(value > 100.0))
				{
					this.shadowOffset = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
			}
		}

		[DefaultValue(GaugeDashStyle.Solid)]
		[SRDescription("DescriptionAttributeBorderStyle8")]
		[SRCategory("CategoryAppearance")]
		public GaugeDashStyle BorderStyle
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

		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeBorderWidth5")]
		[SRCategory("CategoryAppearance")]
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
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
			}
		}

		[SRDescription("DescriptionAttributeFillColor8")]
		[DefaultValue(typeof(Color), "Lime")]
		[SRCategory("CategoryAppearance")]
		public Color FillColor
		{
			get
			{
				return this.fillColor;
			}
			set
			{
				this.fillColor = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeFillGradientType5")]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(RangeGradientType.StartToEnd)]
		public RangeGradientType FillGradientType
		{
			get
			{
				return this.fillGradientType;
			}
			set
			{
				this.fillGradientType = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeFillGradientEndColor5")]
		[DefaultValue(typeof(Color), "Red")]
		[SRCategory("CategoryAppearance")]
		public Color FillGradientEndColor
		{
			get
			{
				return this.fillGradientEndColor;
			}
			set
			{
				this.fillGradientEndColor = value;
				this.Invalidate();
			}
		}

		[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		[SRDescription("DescriptionAttributeFillHatchStyle7")]
		[DefaultValue(GaugeHatchStyle.None)]
		[SRCategory("CategoryAppearance")]
		public GaugeHatchStyle FillHatchStyle
		{
			get
			{
				return this.fillHatchStyle;
			}
			set
			{
				this.fillHatchStyle = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeInRangeTimeout")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(0.0)]
		[SRCategory("CategoryBehavior")]
		public override double InRangeTimeout
		{
			get
			{
				return base.InRangeTimeout;
			}
			set
			{
				base.InRangeTimeout = value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeInRangeTimeoutType")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(PeriodType.Seconds)]
		[Browsable(false)]
		public override PeriodType InRangeTimeoutType
		{
			get
			{
				return base.InRangeTimeoutType;
			}
			set
			{
				base.InRangeTimeoutType = value;
			}
		}

		[SRDescription("DescriptionAttributeSelected4")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DefaultValue(false)]
		[SRCategory("CategoryAppearance")]
		public bool Selected
		{
			get
			{
				return this.selected;
			}
			set
			{
				this.selected = value;
				this.Invalidate();
			}
		}

		public virtual float StartWidth
		{
			get
			{
				return this.startWidth;
			}
			set
			{
				this.startWidth = value;
				this.Invalidate();
			}
		}

		public virtual float EndWidth
		{
			get
			{
				return this.endWidth;
			}
			set
			{
				this.endWidth = value;
				this.Invalidate();
			}
		}

		object IImageMapProvider.Tag
		{
			get
			{
				return this.imageMapProviderTag;
			}
			set
			{
				this.imageMapProviderTag = value;
			}
		}

		internal abstract void Render(GaugeGraphics g);

		internal GaugeBase GetGaugeBase()
		{
			if (this.Common == null)
			{
				return null;
			}
			return (GaugeBase)this.Collection.ParentElement;
		}

		internal override void OnAdded()
		{
			base.OnAdded();
			this.scaleName = this.GetGaugeBase().GetDefaultScaleName(this.scaleName);
		}

		internal override void Notify(MessageType msg, NamedElement element, object param)
		{
			base.Notify(msg, element, param);
			switch (msg)
			{
			case MessageType.NamedElementRemove:
				if (element is ScaleBase)
				{
					ScaleBase scaleBase2 = (ScaleBase)element;
					if (scaleBase2.Name == this.scaleName && scaleBase2.ParentElement == this.ParentElement && this.scaleName != "Default")
					{
						this.scaleName = string.Empty;
					}
				}
				break;
			case MessageType.NamedElementRename:
				if (element is ScaleBase)
				{
					ScaleBase scaleBase = (ScaleBase)element;
					if (scaleBase.Name == this.scaleName && scaleBase.ParentElement == this.ParentElement)
					{
						this.scaleName = (string)param;
					}
				}
				break;
			}
		}

		string IToolTipProvider.GetToolTip(HitTestResult ht)
		{
			if (this.Common != null && this.Common.GaugeCore != null)
			{
				return this.Common.GaugeCore.ResolveAllKeywords(this.ToolTip, this);
			}
			return this.ToolTip;
		}

		string IImageMapProvider.GetToolTip()
		{
			return ((IToolTipProvider)this).GetToolTip((HitTestResult)null);
		}

		string IImageMapProvider.GetHref()
		{
			if (this.Common != null && this.Common.GaugeCore != null)
			{
				return this.Common.GaugeCore.ResolveAllKeywords(this.Href, this);
			}
			return this.Href;
		}

		string IImageMapProvider.GetMapAreaAttributes()
		{
			if (this.Common != null && this.Common.GaugeCore != null)
			{
				return this.Common.GaugeCore.ResolveAllKeywords(this.MapAreaAttributes, this);
			}
			return this.MapAreaAttributes;
		}
	}
}
