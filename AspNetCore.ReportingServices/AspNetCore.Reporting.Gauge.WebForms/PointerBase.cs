using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class PointerBase : NamedElement, IToolTipProvider, IPointerProvider, IImageMapProvider
	{
		private double position;

		private DataAttributes data;

		internal bool dragging;

		private string scaleName = "Default";

		private float distanceFromScale;

		private string image = "";

		private Color imageTransColor = Color.Empty;

		private float imageTransparency;

		private Color imageHueColor = Color.Empty;

		private Point imageOrigin = Point.Empty;

		private bool snappingEnabled;

		private double snappingInterval;

		private bool dampeningEnabled;

		private double dampeningSweepTime = 1.0;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private bool interactive;

		private bool visible = true;

		private BarStyle barStyle;

		private BarStart barStart = BarStart.ScaleStart;

		private float shadowOffset = 2f;

		private Color borderColor = Color.Black;

		private GaugeDashStyle borderStyle;

		private int borderWidth = 1;

		private Color fillColor = Color.White;

		private Color fillGradientEndColor = Color.Red;

		private GaugeHatchStyle fillHatchStyle;

		private GradientType fillGradientType = GradientType.DiagonalLeft;

		private MarkerStyle markerStyle;

		private float markerLength;

		private float width;

		private GaugeCursor cursor = GaugeCursor.Default;

		private object imageMapProviderTag;

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeName9")]
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

		[SRDescription("DescriptionAttributeScaleName4")]
		[SRCategory("CategoryMisc")]
		[DefaultValue("Default")]
		[TypeConverter(typeof(ScaleSourceConverter))]
		public virtual string ScaleName
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

		[DefaultValue(0f)]
		[SRDescription("DescriptionAttributeDistanceFromScale3")]
		[ValidateBound(0.0, 100.0)]
		[SRCategory("CategoryLayout")]
		public virtual float DistanceFromScale
		{
			get
			{
				return this.distanceFromScale;
			}
			set
			{
				if (!(value < -100.0) && !(value > 100.0))
				{
					this.distanceFromScale = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
			}
		}

		[SRDescription("DescriptionAttributeImage7")]
		[SRCategory("CategoryImage")]
		[DefaultValue("")]
		public virtual string Image
		{
			get
			{
				return this.image;
			}
			set
			{
				this.image = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeImageTransColor5")]
		[DefaultValue(typeof(Color), "")]
		public virtual Color ImageTransColor
		{
			get
			{
				return this.imageTransColor;
			}
			set
			{
				this.imageTransColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(float), "0")]
		[SRDescription("DescriptionAttributeImageTransparency5")]
		[SRCategory("CategoryImage")]
		public virtual float ImageTransparency
		{
			get
			{
				return this.imageTransparency;
			}
			set
			{
				if (!(value < 0.0) && !(value > 100.0))
				{
					this.imageTransparency = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 100));
			}
		}

		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeImageHueColor")]
		[SRCategory("CategoryImage")]
		public virtual Color ImageHueColor
		{
			get
			{
				return this.imageHueColor;
			}
			set
			{
				this.imageHueColor = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeCircularPointer_CapImageOrigin")]
		[TypeConverter(typeof(EmptyPointConverter))]
		[DefaultValue(typeof(Point), "0, 0")]
		public virtual Point ImageOrigin
		{
			get
			{
				return this.imageOrigin;
			}
			set
			{
				this.imageOrigin = value;
				this.Invalidate();
			}
		}

		[DefaultValue(double.NaN)]
		[SRCategory("CategoryData")]
		[SRDescription("DescriptionAttributeValue6")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.Repaint)]
		[Browsable(false)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		public virtual double Value
		{
			get
			{
				return this.data.Value;
			}
			set
			{
				if (this.data.Value != value)
				{
					this.data.Value = value;
				}
			}
		}

		[SRDescription("DescriptionAttributeValueSource")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryData")]
		[Browsable(false)]
		[TypeConverter(typeof(ValueSourceConverter))]
		[RefreshProperties(RefreshProperties.Repaint)]
		[NotifyParentProperty(true)]
		[DefaultValue("")]
		public virtual string ValueSource
		{
			get
			{
				return this.data.ValueSource;
			}
			set
			{
				this.data.ValueSource = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeSnappingEnabled4")]
		[DefaultValue(false)]
		public virtual bool SnappingEnabled
		{
			get
			{
				return this.snappingEnabled;
			}
			set
			{
				this.snappingEnabled = value;
				((IPointerProvider)this).DataValueChanged(true);
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeSnappingInterval")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[DefaultValue(0.0)]
		[Browsable(false)]
		public double SnappingInterval
		{
			get
			{
				return this.snappingInterval;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionNegativeValue"));
				}
				this.snappingInterval = value;
				((IPointerProvider)this).DataValueChanged(true);
				this.Invalidate();
			}
		}

		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeDampeningEnabled3")]
		[SRCategory("CategoryBehavior")]
		public virtual bool DampeningEnabled
		{
			get
			{
				return this.dampeningEnabled;
			}
			set
			{
				this.dampeningEnabled = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeDampeningSweepTime")]
		[DefaultValue(1.0)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public double DampeningSweepTime
		{
			get
			{
				return this.dampeningSweepTime;
			}
			set
			{
				this.dampeningSweepTime = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeToolTip5")]
		[Localizable(true)]
		[DefaultValue("")]
		[Browsable(false)]
		public virtual string ToolTip
		{
			get
			{
				return this.toolTip;
			}
			set
			{
				this.toolTip = value;
				this.Invalidate();
			}
		}

		[Browsable(false)]
		[SRDescription("DescriptionAttributeHref10")]
		[SRCategory("CategoryBehavior")]
		[DefaultValue("")]
		[Localizable(true)]
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

		[Browsable(false)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeInteractive3")]
		[DefaultValue(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool Interactive
		{
			get
			{
				return this.interactive;
			}
			set
			{
				this.interactive = value;
				this.Invalidate();
			}
		}

		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeVisible7")]
		[SRCategory("CategoryAppearance")]
		[ParenthesizePropertyName(true)]
		public virtual bool Visible
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

		[DefaultValue(BarStyle.Style1)]
		[SRDescription("DescriptionAttributeBarStyle")]
		[SRCategory("CategoryTypeSpecific")]
		internal BarStyle BarStyle
		{
			get
			{
				return this.barStyle;
			}
			set
			{
				this.barStyle = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeBarStart")]
		[DefaultValue(BarStart.ScaleStart)]
		[SRCategory("CategoryTypeSpecific")]
		public virtual BarStart BarStart
		{
			get
			{
				return this.barStart;
			}
			set
			{
				this.barStart = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[ValidateBound(-5.0, 5.0)]
		[DefaultValue(2f)]
		[SRDescription("DescriptionAttributeShadowOffset")]
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

		[SRCategory("CategoryAppearance")]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeBorderColor5")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBorderStyle3")]
		[DefaultValue(GaugeDashStyle.NotSet)]
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

		[SRDescription("DescriptionAttributeBorderWidth9")]
		[DefaultValue(1)]
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

		[SRDescription("DescriptionAttributeFillColor3")]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(typeof(Color), "White")]
		public virtual Color FillColor
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFillGradientEndColor")]
		[DefaultValue(typeof(Color), "Red")]
		public virtual Color FillGradientEndColor
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFillHatchStyle3")]
		[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		[DefaultValue(GaugeHatchStyle.None)]
		public virtual GaugeHatchStyle FillHatchStyle
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFillGradientType6")]
		[DefaultValue(GradientType.DiagonalLeft)]
		public virtual GradientType FillGradientType
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

		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributePointerBase_MarkerStyle")]
		public virtual MarkerStyle MarkerStyle
		{
			get
			{
				return this.markerStyle;
			}
			set
			{
				this.markerStyle = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributePointerBase_MarkerLength")]
		[SRCategory("CategoryTypeSpecific")]
		public virtual float MarkerLength
		{
			get
			{
				return this.markerLength;
			}
			set
			{
				this.markerLength = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributePointerBase_Width")]
		public virtual float Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeCursor")]
		[SRCategory("CategoryBehavior")]
		[DefaultValue(GaugeCursor.Default)]
		public virtual GaugeCursor Cursor
		{
			get
			{
				return this.cursor;
			}
			set
			{
				this.cursor = value;
			}
		}

		internal double Position
		{
			get
			{
				if (double.IsNaN(this.position))
				{
					return this.GetScaleBase().GetValueLimit(this.position);
				}
				return this.position;
			}
			set
			{
				this.position = value;
				if (this.dragging)
				{
					IValueProvider provider = ((IValueConsumer)this.Data).GetProvider();
					if (provider is InputValue && ((InputValue)provider).IntState != 0)
					{
						((InputValue)provider).Value = value;
					}
					if (!this.DampeningEnabled)
					{
						this.dragging = false;
					}
				}
				this.Refresh();
			}
		}

		internal override CommonElements Common
		{
			get
			{
				return base.Common;
			}
			set
			{
				base.Common = value;
				this.data.Common = value;
			}
		}

		internal DataAttributes Data
		{
			get
			{
				return this.data;
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

		double IPointerProvider.Position
		{
			get
			{
				return this.Position;
			}
			set
			{
				this.Position = value;
			}
		}

		public PointerBase()
		{
			this.data = new DataAttributes(this);
		}

		protected PointerBase(MarkerStyle markerStyle, float markerLength, float width, GradientType fillGradientType)
			: this()
		{
			this.markerStyle = markerStyle;
			this.markerLength = markerLength;
			this.width = width;
			this.fillGradientType = fillGradientType;
		}

		protected PointerBase(MarkerStyle markerStyle, float markerLength, float width, GradientType fillGradientType, Color fillColor, Color fillGradientEndColor, bool interactive)
			: this(markerStyle, markerLength, width, fillGradientType)
		{
			this.fillColor = fillColor;
			this.fillGradientEndColor = fillGradientEndColor;
			this.interactive = interactive;
		}

		internal virtual void DragTo(int x, int y, PointF refPoint)
		{
			ScaleBase scaleBase = this.GetScaleBase();
			double value = scaleBase.GetValue(refPoint, new PointF((float)x, (float)y));
			value = scaleBase.GetValueLimit(value, this.SnappingEnabled, this.SnappingInterval);
			PointerPositionChangeEventArgs pointerPositionChangeEventArgs = new PointerPositionChangeEventArgs(value, DateTime.Now, this.Name, false);
			if (this.Common != null)
			{
				this.Common.GaugeContainer.OnPointerPositionChange(this, pointerPositionChangeEventArgs);
				if (pointerPositionChangeEventArgs.Accept)
				{
					this.dragging = true;
					this.Value = value;
				}
			}
		}

		internal virtual void Render(GaugeGraphics g)
		{
		}

		internal virtual void RenderShadow(GaugeGraphics g)
		{
		}

		internal GaugeBase GetGaugeBase()
		{
			if (this.Common == null)
			{
				return null;
			}
			return (GaugeBase)this.Collection.ParentElement;
		}

		internal ScaleBase GetScaleBase()
		{
			if (this.Common == null)
			{
				return null;
			}
			if (this.scaleName == string.Empty)
			{
				return null;
			}
			GaugeBase gaugeBase = this.GetGaugeBase();
			NamedCollection namedCollection = null;
			if (gaugeBase is CircularGauge)
			{
				namedCollection = ((CircularGauge)gaugeBase).Scales;
			}
			if (gaugeBase is LinearGauge)
			{
				namedCollection = ((LinearGauge)gaugeBase).Scales;
			}
			if (namedCollection == null)
			{
				return null;
			}
			return (ScaleBase)namedCollection.GetByName(this.scaleName);
		}

		internal override void BeginInit()
		{
			base.BeginInit();
			this.data.BeginInit();
		}

		internal override void EndInit()
		{
			base.EndInit();
			this.data.EndInit();
		}

		internal override void OnAdded()
		{
			base.OnAdded();
			this.data.ReconnectData(true);
			this.scaleName = this.GetGaugeBase().GetDefaultScaleName(this.scaleName);
		}

		internal override void ReconnectData(bool exact)
		{
			this.data.ReconnectData(exact);
		}

		protected override void OnDispose()
		{
			this.Data.Dispose();
			base.OnDispose();
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
			this.Data.Notify(msg, element, param);
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

		void IPointerProvider.DataValueChanged(bool initialize)
		{
			if (!initialize)
			{
				GaugeBase gaugeBase = this.GetGaugeBase();
				if (gaugeBase != null)
				{
					gaugeBase.PointerValueChanged(this);
				}
			}
			ScaleBase scaleBase = this.GetScaleBase();
			if (scaleBase != null)
			{
				double valueLimit = scaleBase.GetValueLimit(this.data.Value, this.snappingEnabled, this.snappingInterval);
				if (!initialize && this.dampeningEnabled && this.Data.StartDampening(valueLimit, scaleBase.MinimumLog, scaleBase.Maximum, this.dampeningSweepTime, this.Common.GaugeCore.RefreshRate))
				{
					return;
				}
				this.Position = valueLimit;
			}
		}
	}
}
