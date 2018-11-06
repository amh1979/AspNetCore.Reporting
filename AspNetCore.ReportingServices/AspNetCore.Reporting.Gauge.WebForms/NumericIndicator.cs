using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Timers;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(NumericIndicatorConverter))]
	internal class NumericIndicator : NamedElement, IRenderable, IToolTipProvider, IPointerProvider, ISelectable, IImageMapProvider
	{
		private double numberPosition = double.NaN;

		private DataAttributes data;

		private bool refreshPending;

		private double pendingNumberPosition;

		private SegmentsCache segmentsCache = new SegmentsCache();

		private NumericRangeCollection ranges;

		private NamedElement parentSystem;

		private string parent = string.Empty;

		private int zOrder;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private NumericIndicatorStyle style = NumericIndicatorStyle.Mechanical;

		private int digits = 6;

		private int decimals = 1;

		private FontUnit fontUnit;

		private ResizeMode resizeMode = ResizeMode.AutoFit;

		private float shadowOffset;

		private string formatString = string.Empty;

		private bool showDecimal;

		private bool showLeadingZeros = true;

		private float refreshRate = 10f;

		private string offString = "-";

		private string outOfRangeString = "Error";

		private ShowSign showSign = ShowSign.NegativeOnly;

		private double minimum = double.NegativeInfinity;

		private double maximum = double.PositiveInfinity;

		private double multiplier = 1.0;

		private bool snappingEnabled;

		private double snappingInterval;

		private bool dampeningEnabled;

		private double dampeningSweepTime = 1.0;

		private GaugeLocation location;

		private GaugeSize size;

		private bool visible = true;

		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private Color borderColor = Color.DimGray;

		private GaugeDashStyle borderStyle = GaugeDashStyle.Solid;

		private int borderWidth = 1;

		private Color separatorColor = Color.DimGray;

		private float separatorWidth = 1f;

		private Color backColor = Color.DimGray;

		private GradientType backGradientType = GradientType.HorizontalCenter;

		private Color backGradientEndColor = Color.White;

		private GaugeHatchStyle backHatchStyle;

		private Color digitColor = Color.SteelBlue;

		private Color decimalColor = Color.Firebrick;

		private Color ledDimColor = Color.Empty;

		private bool selected;

		private bool defaultParent = true;

		private object imageMapProviderTag;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NumericRangeCollection Ranges
		{
			get
			{
				return this.ranges;
			}
		}

		[Bindable(false)]
		[Browsable(false)]
		[DefaultValue(null)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeParentObject3")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public NamedElement ParentObject
		{
			get
			{
				return this.parentSystem;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeNumericIndicator_Parent")]
		[NotifyParentProperty(true)]
		[Browsable(false)]
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
				this.DefaultParent = false;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeNumericIndicator_ZOrder")]
		[SRCategory("CategoryLayout")]
		[DefaultValue(0)]
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

		[SRCategory("CategoryBehavior")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeNumericIndicator_ToolTip")]
		[Localizable(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public string ToolTip
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

		[SRDescription("DescriptionAttributeNumericIndicator_Href")]
		[DefaultValue("")]
		[SRCategory("CategoryBehavior")]
		[Localizable(true)]
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

		[SRDescription("DescriptionAttributeNumericIndicator_MapAreaAttributes")]
		[DefaultValue("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeNumericIndicator_Name")]
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

		[Obsolete("This property is obsolete in Dundas Gauge 2.0. IndicatorStyle is supposed to be used instead.")]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRCategory("CategoryStyleSpecific")]
		[SRDescription("DescriptionAttributeNumericIndicator_Style")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[ParenthesizePropertyName(true)]
		[DefaultValue(NumericIndicatorStyle.Mechanical)]
		public NumericIndicatorStyle Style
		{
			get
			{
				return this.style;
			}
			set
			{
				this.style = value;
				this.Invalidate();
			}
		}

		[DefaultValue(NumericIndicatorStyle.Mechanical)]
		[SRDescription("DescriptionAttributeNumericIndicator_Style")]
		[ParenthesizePropertyName(true)]
		[SRCategory("CategoryStyleSpecific")]
		public NumericIndicatorStyle IndicatorStyle
		{
			get
			{
				return this.style;
			}
			set
			{
				this.style = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeNumericIndicator_Digits")]
		[DefaultValue(6)]
		[SRCategory("CategoryBehavior")]
		[ParenthesizePropertyName(true)]
		public int Digits
		{
			get
			{
				return this.digits;
			}
			set
			{
				if (value < this.Decimals && this.Common != null && base.initialized)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionDigitsDecimals"));
				}
				if (value < 0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfRangeMinClose", 0));
				}
				this.digits = value;
				this.RefreshIndicator();
			}
		}

		[DefaultValue(1)]
		[ParenthesizePropertyName(true)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_Decimals")]
		public int Decimals
		{
			get
			{
				return this.decimals;
			}
			set
			{
				if (value > this.Digits && this.Common != null && base.initialized)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionDecimalsDigitsDrror"));
				}
				if (value < 0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionPropertyNegative", "Decimals"));
				}
				this.decimals = value;
				this.RefreshIndicator();
			}
		}

		[DefaultValue(FontUnit.Percent)]
		[SRDescription("DescriptionAttributeNumericIndicator_FontUnit")]
		[SRCategory("CategoryAppearance")]
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

		[SRDescription("DescriptionAttributeResizeMode")]
		[SRCategory("CategoryBehavior")]
		[DefaultValue(ResizeMode.AutoFit)]
		public ResizeMode ResizeMode
		{
			get
			{
				return this.resizeMode;
			}
			set
			{
				this.resizeMode = value;
				this.Invalidate();
			}
		}

		[ValidateBound(-5.0, 5.0)]
		[DefaultValue(0f)]
		[SRDescription("DescriptionAttributeShadowOffset")]
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

		[SRDescription("DescriptionAttributeNumericIndicator_FormatString")]
		[Editor(typeof(LabelFormatEditor), typeof(UITypeEditor))]
		[Localizable(true)]
		[DefaultValue("")]
		[SRCategory("CategoryBehavior")]
		public string FormatString
		{
			get
			{
				return this.formatString;
			}
			set
			{
				this.formatString = value;
				this.RefreshIndicator();
			}
		}

		[DefaultValue(false)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_ShowDecimalPoint")]
		public bool ShowDecimalPoint
		{
			get
			{
				return this.showDecimal;
			}
			set
			{
				this.showDecimal = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeNumericIndicator_ShowLeadingZeros")]
		public bool ShowLeadingZeros
		{
			get
			{
				return this.showLeadingZeros;
			}
			set
			{
				this.showLeadingZeros = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeNumericIndicator_RefreshRate")]
		[SRCategory("CategoryBehavior")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(10f)]
		public float RefreshRate
		{
			get
			{
				return this.refreshRate;
			}
			set
			{
				if (!(value < 0.0) && !(value > 100.0))
				{
					this.refreshRate = value;
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 100));
			}
		}

		[DefaultValue("-")]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_OffString")]
		[Localizable(true)]
		public string OffString
		{
			get
			{
				return this.offString;
			}
			set
			{
				this.offString = value;
				this.Invalidate();
			}
		}

		[DefaultValue("Error")]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_OutOfRangeString")]
		[Localizable(true)]
		public string OutOfRangeString
		{
			get
			{
				return this.outOfRangeString;
			}
			set
			{
				this.outOfRangeString = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeNumericIndicator_ShowSign")]
		[SRCategory("CategoryBehavior")]
		[DefaultValue(ShowSign.NegativeOnly)]
		public ShowSign ShowSign
		{
			get
			{
				return this.showSign;
			}
			set
			{
				this.showSign = value;
				this.Invalidate();
			}
		}

		[DoubleConverterHint(double.NegativeInfinity)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_Minimum")]
		[DefaultValue(double.NegativeInfinity)]
		[TypeConverter(typeof(DoubleInfinityConverter))]
		public double Minimum
		{
			get
			{
				return this.minimum;
			}
			set
			{
				this.minimum = value;
				this.Invalidate();
			}
		}

		[DefaultValue(double.PositiveInfinity)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_Maximum")]
		[DoubleConverterHint(double.PositiveInfinity)]
		[TypeConverter(typeof(DoubleInfinityConverter))]
		public double Maximum
		{
			get
			{
				return this.maximum;
			}
			set
			{
				this.maximum = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[DefaultValue(1.0)]
		[SRDescription("DescriptionAttributeNumericIndicator_Multiplier")]
		public double Multiplier
		{
			get
			{
				return this.multiplier;
			}
			set
			{
				this.multiplier = value;
				this.Invalidate();
			}
		}

		[DefaultValue(false)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_SnappingEnabled")]
		public bool SnappingEnabled
		{
			get
			{
				return this.snappingEnabled;
			}
			set
			{
				this.snappingEnabled = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeNumericIndicator_SnappingInterval")]
		[DefaultValue(0.0)]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionPropertyNegative", "SnappingInterval"));
				}
				this.snappingInterval = value;
				this.Invalidate();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DefaultValue(false)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_DampeningEnabled")]
		public bool DampeningEnabled
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

		[Browsable(false)]
		[DefaultValue(1.0)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeDampeningSweepTime")]
		public double DampeningSweepTime
		{
			get
			{
				return this.dampeningSweepTime;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionPropertyNegative", "DampeningSweepTime"));
				}
				this.dampeningSweepTime = value;
				this.Invalidate();
			}
		}

		[ValidateBound(100.0, 100.0)]
		[SRCategory("CategoryPosition")]
		[SRDescription("DescriptionAttributeNumericIndicator_Location")]
		[TypeConverter(typeof(LocationConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GaugeLocation Location
		{
			get
			{
				return this.location;
			}
			set
			{
				this.location = value;
				this.location.Parent = this;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryPosition")]
		[SRDescription("DescriptionAttributeNumericIndicator_Size")]
		[TypeConverter(typeof(SizeConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[ValidateBound(100.0, 100.0)]
		public GaugeSize Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
				this.size.Parent = this;
				this.Invalidate();
			}
		}

		[ParenthesizePropertyName(true)]
		[SRDescription("DescriptionAttributeNumericIndicator_Visible")]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(true)]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeNumericIndicator_Font")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8.25pt")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeNumericIndicator_BorderColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "DimGray")]
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
		[SRDescription("DescriptionAttributeNumericIndicator_BorderStyle")]
		[NotifyParentProperty(true)]
		[DefaultValue(GaugeDashStyle.Solid)]
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

		[SRDescription("DescriptionAttributeNumericIndicator_BorderWidth")]
		[SRCategory("CategoryAppearance")]
		[NotifyParentProperty(true)]
		[DefaultValue(1)]
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

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeNumericIndicator_SeparatorColor")]
		[SRCategory("CategoryStyleSpecific")]
		[DefaultValue(typeof(Color), "DimGray")]
		public Color SeparatorColor
		{
			get
			{
				return this.separatorColor;
			}
			set
			{
				this.separatorColor = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryStyleSpecific")]
		[SRDescription("DescriptionAttributeNumericIndicator_SeparatorWidth")]
		[NotifyParentProperty(true)]
		[DefaultValue(1f)]
		public float SeparatorWidth
		{
			get
			{
				return this.separatorWidth;
			}
			set
			{
				if (!(value < 0.0) && !(value > 100.0))
				{
					this.separatorWidth = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
			}
		}

		[SRDescription("DescriptionAttributeNumericIndicator_BackColor")]
		[SRCategory("CategoryAppearance")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "DimGray")]
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

		[DefaultValue(GradientType.HorizontalCenter)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeNumericIndicator_BackGradientType")]
		[NotifyParentProperty(true)]
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

		[SRDescription("DescriptionAttributeNumericIndicator_BackGradientEndColor")]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(typeof(Color), "White")]
		[NotifyParentProperty(true)]
		public Color BackGradientEndColor
		{
			get
			{
				return this.backGradientEndColor;
			}
			set
			{
				this.backGradientEndColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(GaugeHatchStyle.None)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeNumericIndicator_BackHatchStyle")]
		[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		[NotifyParentProperty(true)]
		public GaugeHatchStyle BackHatchStyle
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

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(typeof(Color), "SteelBlue")]
		[SRDescription("DescriptionAttributeNumericIndicator_DigitColor")]
		public Color DigitColor
		{
			get
			{
				return this.digitColor;
			}
			set
			{
				this.digitColor = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeNumericIndicator_DecimalColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Firebrick")]
		[SRCategory("CategoryAppearance")]
		public Color DecimalColor
		{
			get
			{
				return this.decimalColor;
			}
			set
			{
				this.decimalColor = value;
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeNumericIndicator_LedDimColor")]
		[DefaultValue(typeof(Color), "Empty")]
		[SRCategory("CategoryStyleSpecific")]
		public Color LedDimColor
		{
			get
			{
				return this.ledDimColor;
			}
			set
			{
				this.ledDimColor = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeNumericIndicator_Value")]
		[Browsable(false)]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.Repaint)]
		[DefaultValue(double.NaN)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		[SRCategory("CategoryData")]
		public double Value
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
					this.data.ValueSource = string.Empty;
				}
			}
		}

		[DefaultValue("")]
		[SRCategory("CategoryData")]
		[SRDescription("DescriptionAttributeValueSource")]
		[TypeConverter(typeof(ValueSourceConverter))]
		[RefreshProperties(RefreshProperties.Repaint)]
		[NotifyParentProperty(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string ValueSource
		{
			get
			{
				return this.data.ValueSource;
			}
			set
			{
				this.data.ValueSource = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeNumericIndicator_Selected")]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(false)]
		[Browsable(false)]
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

		internal Position Position
		{
			get
			{
				return new Position(this.Location, this.Size, ContentAlignment.TopLeft);
			}
		}

		internal double NumberPosition
		{
			get
			{
				return this.numberPosition;
			}
			set
			{
				this.numberPosition = value;
				this.pendingNumberPosition = value;
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
				this.ranges.Common = value;
			}
		}

		internal DataAttributes Data
		{
			get
			{
				return this.data;
			}
		}

		internal bool DefaultParent
		{
			get
			{
				return this.defaultParent;
			}
			set
			{
				this.defaultParent = value;
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
				return this.NumberPosition;
			}
			set
			{
				this.NumberPosition = value;
			}
		}

		public NumericIndicator()
		{
			this.location = new GaugeLocation(this, 35f, 30f);
			this.size = new GaugeSize(this, 30f, 10f);
			this.location.DefaultValues = true;
			this.size.DefaultValues = true;
			this.ranges = new NumericRangeCollection(this, base.common);
			this.data = new DataAttributes(this);
		}

		protected bool ShouldSerializeStyle()
		{
			return false;
		}

		public override string ToString()
		{
			return this.Name;
		}

		private Color GetRangeColor(Color color, bool decimalColor)
		{
			foreach (NumericRange range in this.ranges)
			{
				double num = Math.Min(range.StartValue, range.EndValue);
				double num2 = Math.Max(range.StartValue, range.EndValue);
				if (this.numberPosition >= num && this.numberPosition <= num2)
				{
					return decimalColor ? range.DecimalColor : range.DigitColor;
				}
			}
			return color;
		}

		private Brush GetFontBrush(GaugeGraphics g, Color color)
		{
			if (this.BackGradientType == GradientType.HorizontalCenter && this.IndicatorStyle == NumericIndicatorStyle.Mechanical)
			{
				HSV hSV = ColorHandler.ColorToHSV(this.backGradientEndColor);
				HSV hSV2 = ColorHandler.ColorToHSV(this.backColor);
				HSV hsv = ColorHandler.ColorToHSV(color);
				HSV hsv2 = ColorHandler.ColorToHSV(color);
				hsv.value = Math.Min(Math.Max(hsv.value - (hSV.value - hSV2.value), 0), 255);
				RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
				absoluteRectangle.Inflate(2f, 2f);
				Color firstColor = ColorHandler.HSVtoColor(hsv);
				Color secondColor = ColorHandler.HSVtoColor(hsv2);
				firstColor = Color.FromArgb(color.A, firstColor.R, firstColor.G, firstColor.B);
				secondColor = Color.FromArgb(color.A, secondColor.R, secondColor.G, secondColor.B);
				return g.GetGradientBrush(absoluteRectangle, firstColor, secondColor, this.backGradientType);
			}
			return new SolidBrush(color);
		}

		private double GetNumber()
		{
			return this.numberPosition * this.Multiplier;
		}

		private double GetNumber(double number)
		{
			return number * this.Multiplier;
		}

		private string GetDefaultFormat()
		{
			string text = "F0";
			if (this.formatString != string.Empty)
			{
				text = this.formatString;
				if (this.formatString.IndexOf(";", StringComparison.Ordinal) == -1 && this.ShowSign == ShowSign.None && !this.IsStandardFormat())
				{
					text = text + ";" + text;
				}
			}
			else
			{
				int num = this.Digits - this.Decimals;
				string text2 = "000000000000000000000000000000000000000".Substring(0, num % 40);
				for (int i = 0; i < num / 40; i++)
				{
					text2 += "000000000000000000000000000000000000000";
				}
				if (this.Decimals > 0)
				{
					text2 += ".";
					text2 += "000000000000000000000000000000000000000".Substring(0, this.Decimals % 40);
					for (int j = 0; j < this.Decimals / 40; j++)
					{
						text2 += "000000000000000000000000000000000000000";
					}
				}
				string text3 = text2;
				if (this.ShowSign == ShowSign.Both)
				{
					text2 = ((CultureInfo.CurrentCulture.NumberFormat.NumberNegativePattern <= 2) ? (CultureInfo.CurrentCulture.NumberFormat.PositiveSign + text2) : (text2 + CultureInfo.CurrentCulture.NumberFormat.PositiveSign));
				}
				if (this.ShowSign != 0)
				{
					text3 = ((CultureInfo.CurrentCulture.NumberFormat.NumberNegativePattern <= 2) ? (CultureInfo.CurrentCulture.NumberFormat.NegativeSign + text3) : (text3 + CultureInfo.CurrentCulture.NumberFormat.NegativeSign));
				}
				text = text2 + ";" + text3;
			}
			return text;
		}

		private string GetLabel(ref bool digitsPrinted)
		{
			digitsPrinted = false;
			string text = "";
			if (double.IsNaN(this.numberPosition))
			{
				text = this.offString;
			}
			else if (this.numberPosition > this.maximum || this.numberPosition < this.minimum)
			{
				text = this.outOfRangeString;
			}
			else
			{
				string defaultFormat = this.GetDefaultFormat();
				text = this.GetNumber().ToString(defaultFormat, CultureInfo.CurrentCulture);
				int num = this.Digits;
				string numberDecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
				if (text.IndexOf(numberDecimalSeparator, StringComparison.CurrentCulture) != -1)
				{
					num++;
				}
				switch (this.ShowSign)
				{
				case ShowSign.NegativeOnly:
					if (this.GetNumber() < 0.0)
					{
						num++;
					}
					break;
				case ShowSign.Both:
					num++;
					break;
				}
				if (text.Length > num && this.formatString == string.Empty)
				{
					text = this.outOfRangeString;
				}
				else
				{
					digitsPrinted = true;
				}
			}
			return text;
		}

		private void DrawSymbol(GaugeGraphics g, string symbol, RectangleF rect, Font font, Brush brush, StringFormat format, bool decDot, bool comma)
		{
			PointF pointF = rect.Location;
			pointF.X += (float)(rect.Width / 2.0);
			pointF.Y += (float)(rect.Height / 2.0);
			if (this.style == NumericIndicatorStyle.Mechanical)
			{
				pointF.Y += (float)(rect.Height / 20.0);
				if (g.TextRenderingHint == TextRenderingHint.ClearTypeGridFit)
				{
					using (GraphicsPath graphicsPath = new GraphicsPath())
					{
						graphicsPath.AddString(symbol, font.FontFamily, (int)font.Style, (float)(font.SizeInPoints * 1.3999999761581421), pointF, format);
						g.FillPath(brush, graphicsPath);
					}
				}
				else
				{
					StringFormat stringFormat = new StringFormat();
					stringFormat.Alignment = StringAlignment.Far;
					stringFormat.LineAlignment = StringAlignment.Far;
					g.DrawString(symbol, font, brush, rect, stringFormat);
				}
			}
			else if (this.style == NumericIndicatorStyle.Digital7Segment)
			{
				float num = (float)((double)rect.Height * 0.65);
				if (symbol == string.Empty)
				{
					if (this.LedDimColor != Color.Empty)
					{
						using (Brush brush2 = this.GetFontBrush(g, this.LedDimColor))
						{
							using (GraphicsPath path = DigitalSegment.GetOrientedSegments(LEDSegment7.All, pointF, num, this.segmentsCache))
							{
								g.FillPath(brush2, path);
							}
						}
					}
				}
				else
				{
					using (GraphicsPath path2 = DigitalSegment.GetSymbol7(symbol[0], pointF, num, decDot, comma, false, this.segmentsCache))
					{
						g.FillPath(brush, path2);
					}
				}
			}
			else if (this.style == NumericIndicatorStyle.Digital14Segment)
			{
				float num2 = (float)((double)rect.Height * 0.65);
				symbol = symbol.ToUpper(CultureInfo.InvariantCulture);
				if (symbol == string.Empty)
				{
					if (this.LedDimColor != Color.Empty)
					{
						using (Brush brush3 = this.GetFontBrush(g, this.LedDimColor))
						{
							using (GraphicsPath path3 = DigitalSegment.GetOrientedSegments(LEDSegment14.All, pointF, num2, this.segmentsCache))
							{
								g.FillPath(brush3, path3);
							}
						}
					}
				}
				else
				{
					using (GraphicsPath path4 = DigitalSegment.GetSymbol14(symbol[0], pointF, num2, decDot, comma, false, this.segmentsCache))
					{
						g.FillPath(brush, path4);
					}
				}
			}
		}

		private void DrawSeparator(GaugeGraphics g, Brush brush, float digitsCount, float rectPosition, RectangleF gaugeRect, float separatorWidth)
		{
			float x = gaugeRect.X + gaugeRect.Width / digitsCount * rectPosition;
			RectangleF rect = new RectangleF(x, gaugeRect.Y, 0f, gaugeRect.Height);
			rect.Inflate((float)(separatorWidth / 2.0), 0f);
			using (new GraphicsPath())
			{
				g.FillRectangle(brush, rect);
			}
		}

		private bool IsStandardFormat()
		{
			if (this.formatString != string.Empty && this.formatString.Length < 4)
			{
				if (this.FormatString.Length == 1)
				{
					return true;
				}
				if ("CDEFGNPRX".IndexOf(this.formatString.ToUpper(CultureInfo.InvariantCulture)[0]) != -1)
				{
					bool result = false;
					for (int i = 1; i < this.formatString.Length; i++)
					{
						if (!char.IsDigit(this.formatString[i]))
						{
							return false;
						}
						result = true;
					}
					return result;
				}
			}
			return false;
		}

		private void RenderIndicator(GaugeGraphics g)
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			bool flag = false;
			string label = this.GetLabel(ref flag);
			string format = this.GetDefaultFormat().Replace("#", "0");
			string text;
			string text2;
			if (this.IsStandardFormat())
			{
				double num = Math.Pow(10.0, (double)(this.Digits - this.Decimals - 1));
				text = this.GetNumber(Math.Min(num, this.Maximum)).ToString(format, CultureInfo.CurrentCulture);
				text2 = ((this.ShowSign == ShowSign.None) ? text : this.GetNumber(Math.Max(0.0 - num, this.Minimum)).ToString(format, CultureInfo.CurrentCulture));
			}
			else
			{
				text2 = this.GetNumber(1.0).ToString(format, CultureInfo.CurrentCulture);
				text = this.GetNumber(-1.0).ToString(format, CultureInfo.CurrentCulture);
			}
			int num2 = Math.Max(text2.Length, text.Length);
			this.GetNumber();
			string numberDecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
			bool flag2 = this.showDecimal & this.style == NumericIndicatorStyle.Mechanical;
			if (!flag2 && (text2.IndexOf(numberDecimalSeparator, StringComparison.CurrentCulture) != -1 || text.IndexOf(numberDecimalSeparator, StringComparison.CurrentCulture) != -1))
			{
				num2--;
			}
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			SizeF insetSize = g.MeasureString(SR.DefaultMeazureStringSimbol, this.font);
			RectangleF rectangleF = absoluteRectangle;
			rectangleF.Width /= (float)num2;
			RectangleF boundRect = rectangleF;
			float num3 = g.GetAbsoluteSize(new SizeF(this.SeparatorWidth, 0f)).Width;
			if (num3 > boundRect.Width - boundRect.Width / 10.0)
			{
				num3 = (float)(boundRect.Width - boundRect.Width / 10.0);
			}
			boundRect.Width -= num3;
			RectangleF rect = Utils.NormalizeRectangle(boundRect, insetSize, this.ResizeMode == ResizeMode.AutoFit);
			Font font;
			if (this.ResizeMode == ResizeMode.AutoFit)
			{
				float num4 = this.font.Size * (rect.Height / insetSize.Height);
				if (!(num4 <= 0.0))
				{
					font = new Font(this.font.Name, num4, this.font.Style, this.font.Unit);
					goto IL_02a8;
				}
				return;
			}
			font = ((this.FontUnit != FontUnit.Default) ? ((Font)this.font.Clone()) : ((Font)this.font.Clone()));
			goto IL_02a8;
			IL_02a8:
			Brush fontBrush = this.GetFontBrush(g, this.GetRangeColor(this.digitColor, false));
			Brush fontBrush2 = this.GetFontBrush(g, this.GetRangeColor(this.decimalColor, true));
			Brush fontBrush3 = this.GetFontBrush(g, this.separatorColor);
			float num5 = (float)num2;
			try
			{
				string[] array = new string[2]
				{
					string.Empty,
					string.Empty
				};
				if (flag && label.IndexOf(numberDecimalSeparator, StringComparison.CurrentCulture) != -1)
				{
					int num6 = label.IndexOf(numberDecimalSeparator, StringComparison.CurrentCulture);
					array[1] = label.Substring(num6 + numberDecimalSeparator.Length);
					if (!flag2)
					{
						array[0] = label.Substring(0, num6);
					}
					else
					{
						array[0] = label.Substring(0, num6 + numberDecimalSeparator.Length);
					}
				}
				else
				{
					array[0] = label.Substring(0, Math.Min(label.Length, num2));
				}
				if (array[0].Length + array[1].Length > num2)
				{
					array[0] = this.outOfRangeString.Substring(0, Math.Min(this.outOfRangeString.Length, num2));
					array[1] = "";
				}
				if (this.style == NumericIndicatorStyle.Mechanical)
				{
					if (this.separatorWidth > 0.0)
					{
						for (int i = 1; i < num2; i++)
						{
							this.DrawSeparator(g, fontBrush3, (float)num2, (float)i, absoluteRectangle, num3);
						}
					}
				}
				else if (this.style == NumericIndicatorStyle.Digital7Segment || this.style == NumericIndicatorStyle.Digital14Segment)
				{
					num5 = (float)num2;
					for (int j = 0; j < num2; j++)
					{
						rectangleF.X = absoluteRectangle.X + (num5 = (float)(num5 - 1.0)) * rectangleF.Width;
						rect = Utils.NormalizeRectangle(rectangleF, insetSize, this.ResizeMode == ResizeMode.AutoFit);
						this.DrawSymbol(g, string.Empty, rect, font, fontBrush2, stringFormat, false, false);
					}
				}
				num5 = (float)num2;
				if (array[1].Length > 0)
				{
					for (int num7 = array[1].Length - 1; num7 >= 0; num7--)
					{
						rectangleF.X = absoluteRectangle.X + (num5 = (float)(num5 - 1.0)) * rectangleF.Width;
						rect = Utils.NormalizeRectangle(rectangleF, insetSize, this.ResizeMode == ResizeMode.AutoFit);
						this.DrawSymbol(g, array[1].Substring(num7, 1), rect, font, fontBrush2, stringFormat, false, false);
					}
				}
				if (array[0].Length > 0)
				{
					bool decDot = array[1].Length > 0 && this.ShowDecimalPoint;
					bool comma = false;
					string text3 = "";
					if (!this.ShowLeadingZeros)
					{
						bool flag3 = true;
						for (int k = 0; k < array[0].Length; k++)
						{
							char c = array[0][k];
							if (char.IsDigit(c) && flag3)
							{
								if (c == '0' && k != array[0].Length - 1)
								{
									c = ' ';
								}
								else
								{
									flag3 = false;
								}
							}
							text3 += c;
						}
					}
					else
					{
						text3 = array[0];
					}
					for (int num8 = text3.Length - 1; num8 >= 0; num8--)
					{
						string a = text3.Substring(num8, 1);
						if (this.IndicatorStyle != NumericIndicatorStyle.Mechanical && (a == "." || a == ",") && num8 > 0)
						{
							num8--;
							decDot = (a == ".");
							comma = (a == ",");
							a = text3.Substring(num8, 1);
						}
						rectangleF.X = absoluteRectangle.X + (num5 = (float)(num5 - 1.0)) * rectangleF.Width;
						rect = Utils.NormalizeRectangle(rectangleF, insetSize, this.ResizeMode == ResizeMode.AutoFit);
						this.DrawSymbol(g, text3.Substring(num8, 1), rect, font, fontBrush, stringFormat, decDot, comma);
						decDot = false;
						comma = false;
					}
					for (int num9 = text3.Length - 1; num9 >= 0; num9--)
					{
					}
				}
			}
			finally
			{
				font.Dispose();
				fontBrush.Dispose();
				fontBrush2.Dispose();
			}
		}

		private void RenderBackground(GaugeGraphics g)
		{
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			if (this.ShadowOffset != 0.0)
			{
				RectangleF rect = absoluteRectangle;
				if (this.BorderWidth > 0)
				{
					rect.Inflate((float)this.BorderWidth, (float)this.BorderWidth);
				}
				rect.Offset(this.ShadowOffset, this.ShadowOffset);
				using (Brush brush = g.GetShadowBrush())
				{
					g.FillRectangle(brush, rect);
				}
			}
			if (this.BackColor != Color.Empty)
			{
				using (Brush brush2 = g.CreateBrush(absoluteRectangle, this.BackColor, this.BackHatchStyle, "", GaugeImageWrapMode.Unscaled, Color.Empty, GaugeImageAlign.Center, this.BackGradientType, this.BackGradientEndColor))
				{
					g.FillRectangle(brush2, absoluteRectangle);
				}
			}
			if (this.BorderWidth > 0)
			{
				using (Pen pen = new Pen(this.BorderColor, (float)this.BorderWidth))
				{
					absoluteRectangle.Inflate((float)(this.BorderWidth / 2), (float)(this.BorderWidth / 2));
					if (this.BorderStyle != 0)
					{
						pen.DashStyle = g.GetPenStyle(this.BorderStyle);
					}
					g.DrawRectangle(pen, absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height);
				}
			}
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

		internal override void Notify(MessageType msg, NamedElement element, object param)
		{
			base.Notify(msg, element, param);
			switch (msg)
			{
			case MessageType.NamedElementRemove:
				if (this.parentSystem == element)
				{
					this.Parent = string.Empty;
				}
				break;
			case MessageType.NamedElementRename:
				if (this.parentSystem == element)
				{
					this.parent = element.GetNameAsParent((string)param);
				}
				break;
			case MessageType.PrepareSnapShot:
				this.refreshTimer_Elapsed(this, null);
				break;
			}
			this.Data.Notify(msg, element, param);
			this.ranges.Notify(msg, element, param);
		}

		internal override void OnAdded()
		{
			this.RefreshRate = this.RefreshRate;
			base.OnAdded();
			this.ConnectToParent(true);
			this.data.ReconnectData(true);
			this.refreshTimer_Elapsed(this, null);
			this.Decimals = this.Decimals;
			this.Digits = this.Digits;
		}

		internal override void BeginInit()
		{
			base.BeginInit();
			this.data.BeginInit();
		}

		internal override void EndInit()
		{
			base.EndInit();
			this.ConnectToParent(true);
			this.data.EndInit();
			this.refreshTimer_Elapsed(this, null);
			this.Decimals = this.Decimals;
			this.Digits = this.Digits;
		}

		internal override void ReconnectData(bool exact)
		{
			base.ReconnectData(exact);
			this.data.ReconnectData(exact);
		}

		protected override void OnDispose()
		{
			this.ranges.Dispose();
			this.data.Dispose();
			this.segmentsCache.Reset();
			base.OnDispose();
		}

		internal override void Invalidate()
		{
			this.segmentsCache.Reset();
			base.Invalidate();
		}

		internal virtual void PointerValueChanged()
		{
			bool playbackMode;
			if (this.Common != null && !double.IsNaN(this.Data.OldValue))
			{
				playbackMode = false;
				if (((IValueConsumer)this.Data).GetProvider() != null)
				{
					playbackMode = ((IValueConsumer)this.Data).GetProvider().GetPlayBackMode();
				}
				if (this.Data.OldValue >= this.minimum && this.Data.Value < this.minimum)
				{
					goto IL_0088;
				}
				if (this.Data.OldValue <= this.maximum && this.Data.Value > this.maximum)
				{
					goto IL_0088;
				}
				goto IL_00bc;
			}
			return;
			IL_0108:
			this.Common.GaugeContainer.OnValueScaleEnter(this, new ValueRangeEventArgs(this.Data.Value, this.Data.DateValueStamp, this.Name, playbackMode, this));
			goto IL_013c;
			IL_00bc:
			if (this.Data.OldValue < this.minimum && this.Data.Value >= this.minimum)
			{
				goto IL_0108;
			}
			if (this.Data.OldValue > this.maximum && this.Data.Value <= this.maximum)
			{
				goto IL_0108;
			}
			goto IL_013c;
			IL_0088:
			this.Common.GaugeContainer.OnValueScaleLeave(this, new ValueRangeEventArgs(this.Data.Value, this.Data.DateValueStamp, this.Name, playbackMode, this));
			goto IL_00bc;
			IL_013c:
			foreach (Range range in this.Ranges)
			{
				range.PointerValueChanged(this.Data);
			}
		}

		private void refreshTimer_Elapsed(object source, ElapsedEventArgs e)
		{
			if (this.refreshPending)
			{
				this.refreshPending = false;
				this.numberPosition = this.pendingNumberPosition;
				base.Refresh();
			}
		}

		private void RefreshIndicator()
		{
			this.segmentsCache.Reset();
			this.Refresh();
		}

		private double GetSnapValue(double value)
		{
			if (this.snappingEnabled && this.snappingInterval > 0.0)
			{
				return this.snappingInterval * Math.Round(value / this.snappingInterval);
			}
			return value;
		}

		void IRenderable.RenderStaticElements(GaugeGraphics g)
		{
			if (this.Visible)
			{
				RectangleF rectangle = this.Position.Rectangle;
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddRectangle(g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f)));
				this.Common.GaugeCore.HotRegionList.SetHotRegion(this, graphicsPath);
			}
		}

		void IRenderable.RenderDynamicElements(GaugeGraphics g)
		{
			if (this.Visible)
			{
				g.StartHotRegion(this);
				this.RenderBackground(g);
				this.RenderIndicator(g);
				g.EndHotRegion();
			}
		}

		int IRenderable.GetZOrder()
		{
			return this.ZOrder;
		}

		RectangleF IRenderable.GetBoundRect(GaugeGraphics g)
		{
			return this.Position.Rectangle;
		}

		object IRenderable.GetParentRenderable()
		{
			return this.ParentObject;
		}

		string IRenderable.GetParentRenderableName()
		{
			return this.parent;
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
			this.PointerValueChanged();
			double num = this.NumberPosition = this.GetSnapValue(this.Data.Value);
		}

		void ISelectable.DrawSelection(GaugeGraphics g, bool designTimeSelection)
		{
			Stack stack = new Stack();
			for (NamedElement namedElement = this.ParentObject; namedElement != null; namedElement = (NamedElement)((IRenderable)namedElement).GetParentRenderable())
			{
				stack.Push(namedElement);
			}
			foreach (IRenderable item in stack)
			{
				g.CreateDrawRegion(item.GetBoundRect(g));
			}
			g.CreateDrawRegion(((IRenderable)this).GetBoundRect(g));
			g.DrawSelection(g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f)), designTimeSelection, this.Common.GaugeCore.SelectionBorderColor, this.Common.GaugeCore.SelectionMarkerColor);
			g.RestoreDrawRegion();
			foreach (IRenderable item2 in stack)
			{
				IRenderable renderable = item2;
				g.RestoreDrawRegion();
			}
		}

		public override object Clone()
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatSerializer binaryFormatSerializer = new BinaryFormatSerializer();
			binaryFormatSerializer.Serialize(this, stream);
			NumericIndicator numericIndicator = new NumericIndicator();
			binaryFormatSerializer.Deserialize(numericIndicator, stream);
			return numericIndicator;
		}
	}
}
