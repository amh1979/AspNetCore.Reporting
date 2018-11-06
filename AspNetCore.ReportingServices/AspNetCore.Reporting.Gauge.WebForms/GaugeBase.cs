using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal abstract class GaugeBase : NamedElement, IRenderable, IToolTipProvider, IImageMapProvider
	{
		private string parent = string.Empty;

		private NamedElement parentSystem;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private GaugeLocation location;

		private GaugeSize size;

		private int zOrder;

		private bool visible = true;

		private BackFrame frame;

		private bool clipContent = true;

		private bool selected;

		private string topImage = "";

		private Color topImageTransColor = Color.Empty;

		private Color topImageHueColor = Color.Empty;

		private float aspectRatio = float.NaN;

		private object imagMapProviderTag;

		[SRDescription("DescriptionAttributeParent5")]
		[DefaultValue("")]
		[NotifyParentProperty(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryLayout")]
		[TypeConverter(typeof(ParentSourceConverter))]
		public string Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				string text = this.parent;
				if (value == "(none)")
				{
					value = string.Empty;
				}
				this.parent = value;
				try
				{
					this.ConnectToParent(true);
				}
				catch
				{
					this.parent = text;
					throw;
				}
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeParentObject")]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Bindable(false)]
		[Browsable(false)]
		[DefaultValue(null)]
		public NamedElement ParentObject
		{
			get
			{
				return this.parentSystem;
			}
		}

		[SRDescription("DescriptionAttributeName3")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Localizable(true)]
		[DefaultValue("")]
		[Browsable(false)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeToolTip8")]
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

		[SRDescription("DescriptionAttributeHref3")]
		[SRCategory("CategoryBehavior")]
		[Localizable(true)]
		[DefaultValue("")]
		[Browsable(false)]
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeMapAreaAttributes4")]
		[DefaultValue("")]
		[Browsable(false)]
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

		[ValidateBound(100.0, 100.0)]
		[SRDescription("DescriptionAttributeLocation5")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryPosition")]
		[TypeConverter(typeof(LocationConverter))]
		public GaugeLocation Location
		{
			get
			{
				return this.location;
			}
			set
			{
				if (this.location.X != value.X || this.location.Y != value.Y)
				{
					this.RemoveAutoLayout();
				}
				this.location = value;
				this.location.Parent = this;
				this.Invalidate();
			}
		}

		[ValidateBound(100.0, 100.0)]
		[SRDescription("DescriptionAttributeSize5")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryPosition")]
		[TypeConverter(typeof(SizeConverter))]
		public GaugeSize Size
		{
			get
			{
				return this.size;
			}
			set
			{
				if (this.size.Width != value.Width || this.size.Height != value.Height)
				{
					this.RemoveAutoLayout();
				}
				this.size = value;
				this.size.Parent = this;
				this.Invalidate();
			}
		}

		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeZOrder4")]
		[SRCategory("CategoryLayout")]
		public int ZOrder
		{
			get
			{
				return this.zOrder;
			}
			set
			{
				this.zOrder = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeVisible3")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(true)]
		[SRCategory("CategoryAppearance")]
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

		[SRDescription("DescriptionAttributeBackFrame")]
		[SRCategory("CategoryAppearance")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		public BackFrame BackFrame
		{
			get
			{
				return this.frame;
			}
			set
			{
				this.frame = value;
				this.frame.Parent = this;
				this.Invalidate();
			}
		}

		protected BackFrame Frame
		{
			get
			{
				return this.frame;
			}
			set
			{
				this.frame = value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeClipContent")]
		public bool ClipContent
		{
			get
			{
				return this.clipContent;
			}
			set
			{
				this.clipContent = value;
				this.Invalidate();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeSelected7")]
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

		[SRCategory("CategoryAppearance")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeTopImage4")]
		public string TopImage
		{
			get
			{
				return this.topImage;
			}
			set
			{
				this.topImage = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeTopImageTransColor")]
		public Color TopImageTransColor
		{
			get
			{
				return this.topImageTransColor;
			}
			set
			{
				this.topImageTransColor = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeTopImageHueColor")]
		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAppearance")]
		public Color TopImageHueColor
		{
			get
			{
				return this.topImageHueColor;
			}
			set
			{
				this.topImageHueColor = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeGauge_AspectRatio")]
		[SRCategory("CategoryLayout")]
		[DefaultValue(float.NaN)]
		[TypeConverter(typeof(FloatAutoValueConverter))]
		public float AspectRatio
		{
			get
			{
				return this.aspectRatio;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange_min_open", 0));
				}
				if (value == 0.0)
				{
					this.aspectRatio = float.NaN;
				}
				else
				{
					this.aspectRatio = value;
				}
				this.Invalidate();
			}
		}

		internal Position Position
		{
			get
			{
				return new Position(this.Location, this.Size, ContentAlignment.TopLeft);
			}
		}

		object IImageMapProvider.Tag
		{
			get
			{
				return this.imagMapProviderTag;
			}
			set
			{
				this.imagMapProviderTag = value;
			}
		}

		protected GaugeBase()
		{
			this.location = new GaugeLocation(this, 0f, 0f);
			this.size = new GaugeSize(this, 100f, 100f);
			this.location.DefaultValues = true;
			this.size.DefaultValues = true;
		}

		private void RemoveAutoLayout()
		{
			if (this.Parent == string.Empty && this.Common != null && this.Common.GaugeContainer != null && this.Common.GaugeContainer.AutoLayout)
			{
				this.Common.GaugeContainer.AutoLayout = false;
			}
		}

		internal abstract IEnumerable GetRanges();

		internal string GetDefaultScaleName(string scaleName)
		{
			NamedCollection namedCollection = null;
			if (this is CircularGauge)
			{
				namedCollection = ((CircularGauge)this).Scales;
			}
			if (this is LinearGauge)
			{
				namedCollection = ((LinearGauge)this).Scales;
			}
			if (namedCollection.GetIndex(scaleName) != -1)
			{
				return scaleName;
			}
			if (scaleName == "Default" && namedCollection.Count > 0)
			{
				return namedCollection.GetByIndex(0).Name;
			}
			return scaleName;
		}

		private void ConnectToParent(bool exact)
		{
			if (this.Common != null && !this.Common.GaugeCore.isInitializing)
			{
				if (this.parent == string.Empty)
				{
					this.parentSystem = null;
				}
				else
				{
					this.Common.ObjectLinker.IsParentElementValid(this, this, exact);
					this.parentSystem = this.Common.ObjectLinker.GetElement(this.parent);
				}
			}
		}

		internal override void OnAdded()
		{
			base.OnAdded();
			this.ConnectToParent(true);
		}

		internal override void EndInit()
		{
			base.EndInit();
			this.ConnectToParent(true);
		}

		internal abstract RectangleF GetAspectRatioBounds();

		internal virtual RectangleF GetBoundRect(GaugeGraphics g)
		{
			return this.Position.Rectangle;
		}

		internal virtual void RenderStaticElements(GaugeGraphics g)
		{
		}

		internal virtual void RenderDynamicElements(GaugeGraphics g)
		{
		}

		void IRenderable.RenderStaticElements(GaugeGraphics g)
		{
			this.RenderStaticElements(g);
		}

		void IRenderable.RenderDynamicElements(GaugeGraphics g)
		{
			this.RenderDynamicElements(g);
		}

		int IRenderable.GetZOrder()
		{
			return this.ZOrder;
		}

		RectangleF IRenderable.GetBoundRect(GaugeGraphics g)
		{
			return this.GetBoundRect(g);
		}

		object IRenderable.GetParentRenderable()
		{
			return this.parentSystem;
		}

		string IRenderable.GetParentRenderableName()
		{
			return this.parent;
		}

		internal abstract void PointerValueChanged(PointerBase sender);

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
