using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(StateIndicatorConverter))]
	internal class StateIndicator : NamedElement, IRenderable, IToolTipProvider, IPointerProvider, ISelectable, IImageMapProvider
	{
		private DataAttributes data;

		private double internalValue;

		private StateCollection states;

		private NamedElement parentSystem;

		private string parent = string.Empty;

		private int zOrder;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private StateIndicatorStyle style = StateIndicatorStyle.CircularLed;

		private GaugeLocation location;

		private GaugeSize size;

		private bool visible = true;

		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private Color borderColor = Color.Black;

		private GaugeDashStyle borderStyle;

		private int borderWidth = 1;

		private Color fillColor = Color.LawnGreen;

		private GradientType fillGradientType = GradientType.Center;

		private Color fillGradientEndColor = Color.DarkGreen;

		private GaugeHatchStyle fillHatchStyle;

		private string text = "Text";

		private string image = "";

		private Color imageTransColor = Color.Empty;

		private Color imageHueColor = Color.Empty;

		private float shadowOffset = 1f;

		private bool showBorder;

		private float angle;

		private float scaleFactor = 1f;

		private ResizeMode resizeMode = ResizeMode.AutoFit;

		private float imageTransparency;

		private FontUnit fontUnit = FontUnit.Default;

		private bool selected;

		private bool defaultParent = true;

		private object imageMapProviderTag;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public StateCollection States
		{
			get
			{
				return this.states;
			}
		}

		[Browsable(false)]
		[Bindable(false)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeParentObject3")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public NamedElement ParentObject
		{
			get
			{
				return this.parentSystem;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeStateIndicator_Parent")]
		[TypeConverter(typeof(ParentSourceConverter))]
		[NotifyParentProperty(true)]
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

		[DefaultValue(0)]
		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeStateIndicator_ZOrder")]
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

		[SRDescription("DescriptionAttributeStateIndicator_ToolTip")]
		[SRCategory("CategoryBehavior")]
		[Localizable(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DefaultValue("")]
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

		[DefaultValue("")]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeStateIndicator_Href")]
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

		[DefaultValue("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeMapAreaAttributes4")]
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

		[SRDescription("DescriptionAttributeStateIndicator_Name")]
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

		[Browsable(false)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeStateIndicator_IndicatorStyle")]
		[Obsolete("This property is obsolete in Dundas Gauge 2.0. IndicatorStyle is supposed to be used instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[ParenthesizePropertyName(true)]
		[DefaultValue(StateIndicatorStyle.CircularLed)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public StateIndicatorStyle Style
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

		[SRCategory("CategoryBehavior")]
		[DefaultValue(StateIndicatorStyle.CircularLed)]
		[SRDescription("DescriptionAttributeStateIndicator_IndicatorStyle")]
		[ParenthesizePropertyName(true)]
		public StateIndicatorStyle IndicatorStyle
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

		[SRCategory("CategoryPosition")]
		[ValidateBound(100.0, 100.0)]
		[SRDescription("DescriptionAttributeStateIndicator_Location")]
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
		[ValidateBound(100.0, 100.0)]
		[SRDescription("DescriptionAttributeStateIndicator_Size")]
		[TypeConverter(typeof(SizeConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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

		[SRCategory("CategoryAppearance")]
		[ParenthesizePropertyName(true)]
		[SRDescription("DescriptionAttributeStateIndicator_Visible")]
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

		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8.25pt")]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeStateIndicator_Font")]
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

		[DefaultValue(typeof(Color), "Black")]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeStateIndicator_BorderColor")]
		[NotifyParentProperty(true)]
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

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeStateIndicator_BorderStyle")]
		[SRCategory("CategoryAppearance")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeStateIndicator_BorderWidth")]
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

		[SRDescription("DescriptionAttributeStateIndicator_FillColor")]
		[SRCategory("CategoryAppearance")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "LawnGreen")]
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

		[DefaultValue(GradientType.Center)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeStateIndicator_FillGradientType")]
		[NotifyParentProperty(true)]
		public GradientType FillGradientType
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

		[SRDescription("DescriptionAttributeStateIndicator_FillGradientEndColor")]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(typeof(Color), "DarkGreen")]
		[NotifyParentProperty(true)]
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

		[DefaultValue(GaugeHatchStyle.None)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeStateIndicator_FillHatchStyle")]
		[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		[NotifyParentProperty(true)]
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

		[Browsable(false)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		[SRCategory("CategoryData")]
		[SRDescription("DescriptionAttributeStateIndicator_Value")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.Repaint)]
		[DefaultValue(double.NaN)]
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
					this.Invalidate();
				}
			}
		}

		[SRDescription("DescriptionAttributeStateIndicator_Visible")]
		[SRCategory("CategoryData")]
		[DefaultValue(true)]
		public bool IsPercentBased
		{
			get
			{
				return this.data.IsPercentBased;
			}
			set
			{
				this.data.IsPercentBased = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryData")]
		[Browsable(false)]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeStateIndicator_Minimum")]
		public double Minimum
		{
			get
			{
				return this.data.Minimum;
			}
			set
			{
				if (this.Common != null)
				{
					if (!(value > this.data.Maximum) && (value != this.data.Maximum || value == 0.0))
					{
						if (!double.IsNaN(value) && !double.IsInfinity(value))
						{
							goto IL_006c;
						}
						throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidValue"));
					}
					throw new ArgumentException(Utils.SRGetStr("ExceptionMinMax"));
				}
				goto IL_006c;
				IL_006c:
				this.data.Minimum = value;
				this.Invalidate();
			}
		}

		[Browsable(false)]
		[DefaultValue(100.0)]
		[SRCategory("CategoryData")]
		[SRDescription("DescriptionAttributeStateIndicator_Maximum")]
		public double Maximum
		{
			get
			{
				return this.data.Maximum;
			}
			set
			{
				if (this.Common != null)
				{
					if (!(value < this.data.Minimum) && (value != this.data.Minimum || value == 0.0))
					{
						if (!double.IsNaN(value) && !double.IsInfinity(value))
						{
							goto IL_006c;
						}
						throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidValue"));
					}
					throw new ArgumentException(Utils.SRGetStr("ExceptionMaxMin"));
				}
				goto IL_006c;
				IL_006c:
				this.data.Maximum = value;
				this.Invalidate();
			}
		}

		[DefaultValue("")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryData")]
		[SRDescription("DescriptionAttributeValueSource")]
		[TypeConverter(typeof(ValueSourceConverter))]
		[NotifyParentProperty(true)]
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

		[SRCategory("CategoryBehavior")]
		[Localizable(true)]
		[SRDescription("DescriptionAttributeStateIndicator_Text")]
		[DefaultValue("Text")]
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

		[DefaultValue("")]
		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeStateIndicator_Image")]
		public string Image
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

		[SRDescription("DescriptionAttributeImageTransColor6")]
		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryImage")]
		public Color ImageTransColor
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

		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeImageHueColor4")]
		public Color ImageHueColor
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

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeStateIndicator_CurrentState")]
		[Browsable(false)]
		public string CurrentState
		{
			get
			{
				State currentState = this.GetCurrentState();
				if (currentState != null)
				{
					return currentState.Name;
				}
				return string.Empty;
			}
		}

		[SRDescription("DescriptionAttributeShadowOffset4")]
		[ValidateBound(-5.0, 5.0)]
		[DefaultValue(1f)]
		[NotifyParentProperty(true)]
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

		[SRCategory("CategoryAppearance")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeShowBorder")]
		[NotifyParentProperty(true)]
		public bool ShowBorder
		{
			get
			{
				return this.showBorder;
			}
			set
			{
				this.showBorder = value;
				this.Invalidate();
			}
		}

		[ValidateBound(0.0, 360.0)]
		[DefaultValue(0f)]
		[SRCategory("CategoryPosition")]
		[SRDescription("DescriptionAttributeStateIndicator_Angle")]
		public float Angle
		{
			get
			{
				return this.angle;
			}
			set
			{
				if (!(value > 360.0) && !(value < 0.0))
				{
					this.angle = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 360));
			}
		}

		[DefaultValue(1f)]
		[ValidateBound(0.0, 1.0)]
		[SRCategory("CategoryPosition")]
		[SRDescription("DescriptionAttributeStateIndicator_ScaleFactor")]
		public float ScaleFactor
		{
			get
			{
				return this.scaleFactor;
			}
			set
			{
				if (!(value > 1.0) && !(value < 0.0))
				{
					this.scaleFactor = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 1));
			}
		}

		[SRDescription("DescriptionAttributeStateIndicator_ResizeMode")]
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

		[DefaultValue(0f)]
		[SRCategory("CategoryImage")]
		[ValidateBound(0.0, 100.0)]
		[SRDescription("DescriptionAttributeStateIndicator_ImageTransparency")]
		public float ImageTransparency
		{
			get
			{
				return this.imageTransparency;
			}
			set
			{
				if (!(value > 100.0) && !(value < 0.0))
				{
					this.imageTransparency = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 100));
			}
		}

		[SRDescription("DescriptionAttributeFontUnit3")]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(FontUnit.Default)]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeStateIndicator_Selected")]
		[DefaultValue(false)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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

		internal override CommonElements Common
		{
			get
			{
				return base.Common;
			}
			set
			{
				this.data.Common = value;
				this.States.Common = value;
				base.Common = value;
			}
		}

		internal DataAttributes Data
		{
			get
			{
				return this.data;
			}
		}

		internal double InternalValue
		{
			get
			{
				return this.internalValue;
			}
			set
			{
				this.internalValue = value;
				this.Refresh();
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
				return this.InternalValue;
			}
			set
			{
				this.InternalValue = value;
			}
		}

		public StateIndicator()
		{
			this.location = new GaugeLocation(this, 26f, 47f);
			this.size = new GaugeSize(this, 12f, 6f);
			this.location.DefaultValues = true;
			this.size.DefaultValues = true;
			this.states = new StateCollection(this, base.common);
			this.data = new DataAttributes(this);
		}

		protected bool ShouldSerializeStyle()
		{
			return false;
		}

		public double GetValueInPercents()
		{
			return this.data.GetValueInPercents();
		}

		internal static XamlRenderer CreateXamlRenderer(StateIndicatorStyle style, Color color, RectangleF rect, bool addBorder)
		{
			XamlRenderer xamlRenderer = new XamlRenderer(style.ToString() + ".xaml");
			xamlRenderer.AllowPathGradientTransform = false;
			xamlRenderer.ParseXaml(rect, new Color[5]
			{
				Color.Transparent,
				Color.Transparent,
				color,
				Color.Transparent,
				Color.Transparent
			}, addBorder ? 5 : 4);
			return xamlRenderer;
		}

		internal XamlRenderer GetXamlRenderer(GaugeGraphics g, State currentState)
		{
			GraphicsPath path = this.GetPath(g, currentState);
			XamlRenderer xamlRenderer = null;
			if (path != null)
			{
				RectangleF squareScaledAbsoluteRectangle = this.GetSquareScaledAbsoluteRectangle(g, currentState.ScaleFactor, new RectangleF(0f, 0f, 100f, 100f));
				if (squareScaledAbsoluteRectangle.Width != 0.0 && squareScaledAbsoluteRectangle.Height != 0.0)
				{
					float num = (float)(squareScaledAbsoluteRectangle.Width / 2.0);
					RectangleF rect = StateIndicator.CalculateXamlMarkerBounds(currentState.IndicatorStyle, new PointF(squareScaledAbsoluteRectangle.X + num, squareScaledAbsoluteRectangle.Y + num), num, num);
					xamlRenderer = StateIndicator.CreateXamlRenderer(currentState.IndicatorStyle, currentState.FillColor, rect, this.showBorder);
					XamlLayer[] layers = xamlRenderer.Layers;
					foreach (XamlLayer xamlLayer in layers)
					{
						GraphicsPath[] paths = xamlLayer.Paths;
						foreach (GraphicsPath graphicsPath in paths)
						{
							if (this.Angle != 0.0)
							{
								using (Matrix matrix = new Matrix())
								{
									PointF point = new PointF((float)(squareScaledAbsoluteRectangle.X + squareScaledAbsoluteRectangle.Width / 2.0), (float)(squareScaledAbsoluteRectangle.Y + squareScaledAbsoluteRectangle.Height / 2.0));
									matrix.RotateAt(this.Angle, point);
									graphicsPath.Transform(matrix);
								}
							}
						}
					}
					goto IL_015a;
				}
				return null;
			}
			goto IL_015a;
			IL_015a:
			return xamlRenderer;
		}

		internal static bool IsXamlMarker(StateIndicatorStyle style)
		{
			switch (style)
			{
			case StateIndicatorStyle.RectangularLed:
			case StateIndicatorStyle.CircularLed:
			case StateIndicatorStyle.RoundedRectangularLed:
			case StateIndicatorStyle.Text:
				return false;
			default:
				return true;
			}
		}

		internal static RectangleF CalculateXamlMarkerBounds(StateIndicatorStyle markerStyle, PointF centerPoint, float width, float height)
		{
			RectangleF result = RectangleF.Empty;
			if (StateIndicator.IsXamlMarker(markerStyle))
			{
				result = new RectangleF(centerPoint.X, centerPoint.Y, 0f, 0f);
				result.Inflate(width, height);
			}
			return result;
		}

		public override string ToString()
		{
			return this.Name;
		}

		internal GraphicsPath GetPath(GaugeGraphics g, State currentState)
		{
			if (!this.Visible)
			{
				return null;
			}
			RectangleF rectangleF = RectangleF.Empty;
			GraphicsPath graphicsPath = new GraphicsPath();
			if (StateIndicator.IsXamlMarker(this.IndicatorStyle))
			{
				rectangleF = this.GetSquareScaledAbsoluteRectangle(g, currentState.ScaleFactor, new RectangleF(0f, 0f, 100f, 100f));
				graphicsPath.AddRectangle(rectangleF);
			}
			else
			{
				rectangleF = this.GetScaledAbsoluteRectangle(g, currentState.ScaleFactor, new RectangleF(0f, 0f, 100f, 100f));
				if (this.IndicatorStyle == StateIndicatorStyle.RectangularLed)
				{
					graphicsPath.AddRectangle(rectangleF);
				}
				else if (this.IndicatorStyle == StateIndicatorStyle.CircularLed)
				{
					if (rectangleF.Width != rectangleF.Height)
					{
						if (rectangleF.Width > rectangleF.Height)
						{
							rectangleF.Offset((float)((rectangleF.Width - rectangleF.Height) / 2.0), 0f);
							rectangleF.Width = rectangleF.Height;
						}
						else if (rectangleF.Width < rectangleF.Height)
						{
							rectangleF.Offset(0f, (float)((rectangleF.Height - rectangleF.Width) / 2.0));
							rectangleF.Height = rectangleF.Width;
						}
					}
					graphicsPath.AddEllipse(rectangleF);
				}
				else if (this.IndicatorStyle == StateIndicatorStyle.RoundedRectangularLed)
				{
					float num = (float)((rectangleF.Width < rectangleF.Height) ? (rectangleF.Width / 2.0) : (rectangleF.Height / 2.0));
					float[] cornerRadius = new float[8]
					{
						num,
						num,
						num,
						num,
						num,
						num,
						num,
						num
					};
					graphicsPath.AddPath(g.CreateRoundedRectPath(rectangleF, cornerRadius), false);
				}
				else if (this.IndicatorStyle == StateIndicatorStyle.Text)
				{
					Font font;
					string text;
					if (currentState != null)
					{
						text = currentState.Text;
						font = currentState.Font;
					}
					else
					{
						text = this.Text;
						font = this.Font;
					}
					if (text.Length == 0)
					{
						return null;
					}
					text = text.Replace("\\n", "\n");
					float emSize;
					if (this.ResizeMode == ResizeMode.AutoFit)
					{
						SizeF sizeF = g.MeasureString(text, font);
						SizeF absoluteSize = g.GetAbsoluteSize(new SizeF(100f, 100f));
						float num2 = absoluteSize.Width / sizeF.Width;
						float num3 = absoluteSize.Height / sizeF.Height;
						emSize = (float)((!(num2 < num3)) ? (font.SizeInPoints * num3 * 1.2999999523162842) : (font.SizeInPoints * num2 * 1.2999999523162842));
					}
					else
					{
						if (this.FontUnit == FontUnit.Percent)
						{
							g.RestoreDrawRegion();
							emSize = g.GetAbsoluteDimension(font.Size);
							RectangleF boundRect = ((IRenderable)this).GetBoundRect(g);
							g.CreateDrawRegion(boundRect);
						}
						else
						{
							emSize = font.Size;
						}
						emSize = (float)(emSize * 1.2999999523162842);
					}
					StringFormat stringFormat = new StringFormat();
					stringFormat.Alignment = StringAlignment.Center;
					stringFormat.LineAlignment = StringAlignment.Center;
					graphicsPath.AddString(text, font.FontFamily, (int)font.Style, emSize, rectangleF, stringFormat);
				}
			}
			if (this.Angle != 0.0)
			{
				PointF point = new PointF((float)(rectangleF.X + rectangleF.Width / 2.0), (float)(rectangleF.Y + rectangleF.Height / 2.0));
				using (Matrix matrix = new Matrix())
				{
					matrix.RotateAt(this.Angle, point);
					graphicsPath.Transform(matrix);
					return graphicsPath;
				}
			}
			return graphicsPath;
		}

		internal Brush GetBrush(GaugeGraphics g, State currentState, RectangleF rect)
		{
			if (!this.IsCircular())
			{
				rect = this.GetScaledAbsoluteRectangle(g, currentState.ScaleFactor, new RectangleF(0f, 0f, 100f, 100f));
			}
			rect.Inflate(1f, 1f);
			Brush brush = null;
			Color color;
			Color color2;
			GradientType gradientType;
			GaugeHatchStyle gaugeHatchStyle;
			if (currentState != null)
			{
				color = currentState.FillColor;
				color2 = currentState.FillGradientEndColor;
				gradientType = currentState.FillGradientType;
				gaugeHatchStyle = currentState.FillHatchStyle;
			}
			else
			{
				color = this.FillColor;
				color2 = this.FillGradientEndColor;
				gradientType = this.FillGradientType;
				gaugeHatchStyle = this.FillHatchStyle;
			}
			if (this.IndicatorStyle == StateIndicatorStyle.Text)
			{
				gaugeHatchStyle = GaugeHatchStyle.None;
				gradientType = GradientType.None;
			}
			if (gaugeHatchStyle != 0)
			{
				brush = GaugeGraphics.GetHatchBrush(gaugeHatchStyle, color, color2);
			}
			else if (gradientType != 0)
			{
				if (this.IsCircular() && gradientType == GradientType.DiagonalLeft)
				{
					brush = g.GetGradientBrush(rect, color, color2, GradientType.LeftRight);
					Matrix matrix = new Matrix();
					matrix.RotateAt(45f, new PointF((float)(rect.X + rect.Width / 2.0), (float)(rect.Y + rect.Height / 2.0)));
					((LinearGradientBrush)brush).Transform = matrix;
				}
				else if (this.IsCircular() && gradientType == GradientType.DiagonalRight)
				{
					brush = g.GetGradientBrush(rect, color, color2, GradientType.TopBottom);
					Matrix matrix2 = new Matrix();
					matrix2.RotateAt(135f, new PointF((float)(rect.X + rect.Width / 2.0), (float)(rect.Y + rect.Height / 2.0)));
					((LinearGradientBrush)brush).Transform = matrix2;
				}
				else if (gradientType == GradientType.Center)
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					if (this.IsCircular())
					{
						graphicsPath.AddArc((float)(rect.X - 1.0), (float)(rect.Y - 1.0), (float)(rect.Width + 2.0), (float)(rect.Height + 2.0), 0f, 360f);
					}
					else
					{
						graphicsPath.AddRectangle(rect);
					}
					PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
					pathGradientBrush.CenterColor = color;
					pathGradientBrush.CenterPoint = new PointF((float)(rect.X + rect.Width / 2.0), (float)(rect.Y + rect.Height / 2.0));
					pathGradientBrush.SurroundColors = new Color[1]
					{
						color2
					};
					brush = pathGradientBrush;
				}
				else
				{
					brush = g.GetGradientBrush(rect, color, color2, gradientType);
				}
				if (!this.IsCircular() && this.Angle != 0.0)
				{
					PointF pointF = new PointF((float)(rect.X + rect.Width / 2.0), (float)(rect.Y + rect.Height / 2.0));
					if (brush is LinearGradientBrush)
					{
						((LinearGradientBrush)brush).TranslateTransform((float)(0.0 - pointF.X), (float)(0.0 - pointF.Y), MatrixOrder.Append);
						((LinearGradientBrush)brush).RotateTransform(this.Angle, MatrixOrder.Append);
						((LinearGradientBrush)brush).TranslateTransform(pointF.X, pointF.Y, MatrixOrder.Append);
					}
					else if (brush is PathGradientBrush)
					{
						((PathGradientBrush)brush).TranslateTransform((float)(0.0 - pointF.X), (float)(0.0 - pointF.Y), MatrixOrder.Append);
						((PathGradientBrush)brush).RotateTransform(this.Angle, MatrixOrder.Append);
						((PathGradientBrush)brush).TranslateTransform(pointF.X, pointF.Y, MatrixOrder.Append);
					}
				}
			}
			else
			{
				brush = new SolidBrush(color);
			}
			return brush;
		}

		internal Pen GetPen(GaugeGraphics g, State currentState)
		{
			Color color;
			int num;
			GaugeDashStyle gaugeDashStyle;
			if (currentState != null)
			{
				if (currentState.BorderWidth <= 0 && currentState.BorderStyle != 0)
				{
					return null;
				}
				color = currentState.BorderColor;
				num = currentState.BorderWidth;
				gaugeDashStyle = currentState.BorderStyle;
			}
			else
			{
				if (this.BorderWidth <= 0 && this.BorderStyle != 0)
				{
					return null;
				}
				color = this.BorderColor;
				num = this.BorderWidth;
				gaugeDashStyle = this.BorderStyle;
			}
			Pen pen = new Pen(color, (float)num);
			pen.DashStyle = g.GetPenStyle(gaugeDashStyle);
			pen.Alignment = PenAlignment.Center;
			return pen;
		}

		private RectangleF GetSquareScaledAbsoluteRectangle(GaugeGraphics g, float scaleFactor, RectangleF rect)
		{
			RectangleF scaledAbsoluteRectangle = this.GetScaledAbsoluteRectangle(g, scaleFactor, rect);
			if (scaledAbsoluteRectangle.Width != scaledAbsoluteRectangle.Height)
			{
				if (scaledAbsoluteRectangle.Width > scaledAbsoluteRectangle.Height)
				{
					scaledAbsoluteRectangle.Offset((float)((scaledAbsoluteRectangle.Width - scaledAbsoluteRectangle.Height) / 2.0), 0f);
					scaledAbsoluteRectangle.Width = scaledAbsoluteRectangle.Height;
				}
				else if (scaledAbsoluteRectangle.Width < scaledAbsoluteRectangle.Height)
				{
					scaledAbsoluteRectangle.Offset(0f, (float)((scaledAbsoluteRectangle.Height - scaledAbsoluteRectangle.Width) / 2.0));
					scaledAbsoluteRectangle.Height = scaledAbsoluteRectangle.Width;
				}
			}
			return scaledAbsoluteRectangle;
		}

		private RectangleF GetScaledAbsoluteRectangle(GaugeGraphics g, float scaleFactor, RectangleF rect)
		{
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			if (scaleFactor != 1.0)
			{
				absoluteRectangle.Inflate((float)((absoluteRectangle.Width * scaleFactor - absoluteRectangle.Width) / 2.0), (float)((absoluteRectangle.Height * scaleFactor - absoluteRectangle.Height) / 2.0));
			}
			return absoluteRectangle;
		}

		internal void DrawImage(GaugeGraphics g, string imageName, float scaleFactor, Color imageTransColor, Color imageHueColor, bool drawShadow)
		{
			if (drawShadow && this.ShadowOffset == 0.0)
			{
				return;
			}
			Image image = this.Common.ImageLoader.LoadImage(imageName);
			if (image.Width != 0 && image.Height != 0)
			{
				RectangleF scaledAbsoluteRectangle = this.GetScaledAbsoluteRectangle(g, scaleFactor, new RectangleF(0f, 0f, 100f, 100f));
				Rectangle rectangle = Rectangle.Empty;
				if (this.ResizeMode == ResizeMode.AutoFit)
				{
					rectangle = new Rectangle((int)scaledAbsoluteRectangle.X, (int)scaledAbsoluteRectangle.Y, (int)scaledAbsoluteRectangle.Width, (int)scaledAbsoluteRectangle.Height);
				}
				else
				{
					rectangle = new Rectangle(0, 0, image.Width, image.Height);
					PointF absolutePoint = g.GetAbsolutePoint(new PointF(50f, 50f));
					rectangle.X = (int)(absolutePoint.X - (float)(rectangle.Size.Width / 2));
					rectangle.Y = (int)(absolutePoint.Y - (float)(rectangle.Size.Height / 2));
				}
				ImageAttributes imageAttributes = new ImageAttributes();
				if (this.ImageTransColor != Color.Empty)
				{
					imageAttributes.SetColorKey(imageTransColor, imageTransColor, ColorAdjustType.Default);
				}
				float num = (float)((100.0 - this.ImageTransparency) / 100.0);
				float num2 = (float)(this.Common.GaugeCore.ShadowIntensity / 100.0);
				if (drawShadow)
				{
					ColorMatrix colorMatrix = new ColorMatrix();
					colorMatrix.Matrix00 = 0f;
					colorMatrix.Matrix11 = 0f;
					colorMatrix.Matrix22 = 0f;
					colorMatrix.Matrix33 = num2 * num;
					imageAttributes.SetColorMatrix(colorMatrix);
				}
				else if (this.ImageTransparency > 0.0)
				{
					ColorMatrix colorMatrix2 = new ColorMatrix();
					colorMatrix2.Matrix33 = num;
					imageAttributes.SetColorMatrix(colorMatrix2);
				}
				if (this.Angle != 0.0)
				{
					PointF point = new PointF((float)(scaledAbsoluteRectangle.X + scaledAbsoluteRectangle.Width / 2.0), (float)(scaledAbsoluteRectangle.Y + scaledAbsoluteRectangle.Height / 2.0));
					Matrix transform = g.Transform;
					Matrix matrix = g.Transform.Clone();
					float offsetX = matrix.OffsetX;
					float offsetY = matrix.OffsetY;
					point.X += offsetX;
					point.Y += offsetY;
					matrix.RotateAt(this.Angle, point, MatrixOrder.Append);
					if (drawShadow)
					{
						matrix.Translate(this.ShadowOffset, this.ShadowOffset, MatrixOrder.Append);
					}
					else if (!imageHueColor.IsEmpty)
					{
						Color color = g.TransformHueColor(imageHueColor);
						ColorMatrix colorMatrix3 = new ColorMatrix();
						colorMatrix3.Matrix00 = (float)((float)(int)color.R / 255.0);
						colorMatrix3.Matrix11 = (float)((float)(int)color.G / 255.0);
						colorMatrix3.Matrix22 = (float)((float)(int)color.B / 255.0);
						imageAttributes.SetColorMatrix(colorMatrix3);
					}
					g.Transform = matrix;
					ImageSmoothingState imageSmoothingState = new ImageSmoothingState(g);
					imageSmoothingState.Set();
					g.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
					imageSmoothingState.Restore();
					g.Transform = transform;
				}
				else
				{
					if (drawShadow)
					{
						rectangle.X += (int)this.ShadowOffset;
						rectangle.Y += (int)this.ShadowOffset;
					}
					else if (!imageHueColor.IsEmpty)
					{
						Color color2 = g.TransformHueColor(imageHueColor);
						ColorMatrix colorMatrix4 = new ColorMatrix();
						colorMatrix4.Matrix00 = (float)((float)(int)color2.R / 255.0);
						colorMatrix4.Matrix11 = (float)((float)(int)color2.G / 255.0);
						colorMatrix4.Matrix22 = (float)((float)(int)color2.B / 255.0);
						imageAttributes.SetColorMatrix(colorMatrix4);
					}
					ImageSmoothingState imageSmoothingState2 = new ImageSmoothingState(g);
					imageSmoothingState2.Set();
					g.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
					imageSmoothingState2.Restore();
				}
				if (!drawShadow)
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					graphicsPath.AddRectangle(rectangle);
					if (this.Angle != 0.0)
					{
						PointF point2 = new PointF((float)(scaledAbsoluteRectangle.X + scaledAbsoluteRectangle.Width / 2.0), (float)(scaledAbsoluteRectangle.Y + scaledAbsoluteRectangle.Height / 2.0));
						using (Matrix matrix2 = new Matrix())
						{
							matrix2.RotateAt(this.Angle, point2, MatrixOrder.Append);
							graphicsPath.Transform(matrix2);
						}
					}
					this.Common.GaugeCore.HotRegionList.SetHotRegion(this, Point.Empty, graphicsPath);
				}
			}
		}

		internal State GetCurrentState()
		{
			foreach (State state in this.States)
			{
				if (state.GetDataState(this.Data).IsRangeActive)
				{
					return state;
				}
			}
			return null;
		}

		internal bool IsCircular()
		{
			if (this.IndicatorStyle == StateIndicatorStyle.CircularLed)
			{
				return true;
			}
			return false;
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
		}

		internal override void ReconnectData(bool exact)
		{
			this.data.ReconnectData(exact);
		}

		protected override void OnDispose()
		{
			this.States.Dispose();
			this.Data.Dispose();
			base.OnDispose();
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
			}
			this.Data.Notify(msg, element, param);
			this.States.Notify(msg, element, param);
		}

		internal override void OnAdded()
		{
			base.OnAdded();
			this.ConnectToParent(true);
			this.data.ReconnectData(true);
		}

		void IRenderable.RenderStaticElements(GaugeGraphics g)
		{
		}

		void IRenderable.RenderDynamicElements(GaugeGraphics g)
		{
			if (this.Visible)
			{
				g.StartHotRegion(this);
				State state = this.GetCurrentState();
				if (state == null)
				{
					state = new State();
					state.IndicatorStyle = this.IndicatorStyle;
					state.ImageTransColor = this.ImageTransColor;
					state.ImageHueColor = this.ImageHueColor;
					state.Image = this.Image;
					state.ScaleFactor = this.ScaleFactor;
					state.FillColor = this.FillColor;
				}
				GraphicsPath graphicsPath = null;
				Brush brush = null;
				Brush brush2 = null;
				Pen pen = null;
				try
				{
					switch (state.IndicatorStyle)
					{
					case StateIndicatorStyle.None:
						break;
					case StateIndicatorStyle.Image:
						if (state.Image.Length != 0)
						{
							this.DrawImage(g, state.Image, state.ScaleFactor, state.ImageTransColor, state.ImageHueColor, true);
							this.DrawImage(g, state.Image, state.ScaleFactor, state.ImageTransColor, state.ImageHueColor, false);
							g.EndHotRegion();
						}
						break;
					case StateIndicatorStyle.Text:
						graphicsPath = this.GetPath(g, state);
						if (graphicsPath != null)
						{
							brush = this.GetBrush(g, state, graphicsPath.GetBounds());
							if (this.ShadowOffset != 0.0)
							{
								using (Matrix matrix2 = new Matrix())
								{
									brush2 = g.GetShadowBrush();
									matrix2.Translate(this.ShadowOffset, this.ShadowOffset, MatrixOrder.Append);
									graphicsPath.Transform(matrix2);
									g.FillPath(brush2, graphicsPath);
									matrix2.Reset();
									matrix2.Translate((float)(0.0 - this.ShadowOffset), (float)(0.0 - this.ShadowOffset), MatrixOrder.Append);
									graphicsPath.Transform(matrix2);
								}
							}
							AntiAliasing antiAliasing = this.Common.GaugeContainer.AntiAliasing;
							AntiAliasing antiAliasing2 = g.AntiAliasing;
							if (this.Common.GaugeContainer.AntiAliasing == AntiAliasing.Text)
							{
								antiAliasing = AntiAliasing.Graphics;
							}
							else if (this.Common.GaugeContainer.AntiAliasing == AntiAliasing.Graphics)
							{
								antiAliasing = AntiAliasing.None;
							}
							g.AntiAliasing = antiAliasing;
							g.FillPath(brush, graphicsPath);
							g.AntiAliasing = antiAliasing2;
							using (GraphicsPath graphicsPath2 = new GraphicsPath())
							{
								graphicsPath2.AddRectangle(graphicsPath.GetBounds());
								this.Common.GaugeCore.HotRegionList.SetHotRegion(this, Point.Empty, (GraphicsPath)graphicsPath2.Clone());
							}
						}
						break;
					case StateIndicatorStyle.RectangularLed:
					case StateIndicatorStyle.CircularLed:
					case StateIndicatorStyle.RoundedRectangularLed:
						graphicsPath = this.GetPath(g, state);
						if (graphicsPath != null)
						{
							brush = this.GetBrush(g, state, graphicsPath.GetBounds());
							if (this.ShadowOffset != 0.0)
							{
								using (Matrix matrix = new Matrix())
								{
									brush2 = g.GetShadowBrush();
									matrix.Translate(this.ShadowOffset, this.ShadowOffset, MatrixOrder.Append);
									graphicsPath.Transform(matrix);
									g.FillPath(brush2, graphicsPath);
									matrix.Reset();
									matrix.Translate((float)(0.0 - this.ShadowOffset), (float)(0.0 - this.ShadowOffset), MatrixOrder.Append);
									graphicsPath.Transform(matrix);
								}
							}
							g.FillPath(brush, graphicsPath);
							if (state.IndicatorStyle != StateIndicatorStyle.Text && !StateIndicator.IsXamlMarker(state.IndicatorStyle))
							{
								pen = this.GetPen(g, state);
								if (pen != null)
								{
									g.DrawPath(pen, graphicsPath);
								}
							}
							this.Common.GaugeCore.HotRegionList.SetHotRegion(this, Point.Empty, (GraphicsPath)graphicsPath.Clone());
						}
						break;
					default:
						graphicsPath = this.GetPath(g, state);
						if (graphicsPath != null)
						{
							XamlRenderer xamlRenderer = this.GetXamlRenderer(g, state);
							if (xamlRenderer != null)
							{
								for (int i = 0; i < xamlRenderer.Layers.Length; i++)
								{
									if (i == 0)
									{
										if (this.ShadowOffset != 0.0)
										{
											brush2 = g.GetShadowBrush();
											xamlRenderer.Layers[i].SetSingleBrush(brush2);
											xamlRenderer.Layers[i].Render(g);
										}
									}
									else
									{
										xamlRenderer.Layers[i].Render(g);
									}
								}
							}
							this.Common.GaugeCore.HotRegionList.SetHotRegion(this, Point.Empty, (GraphicsPath)graphicsPath.Clone());
						}
						break;
					}
				}
				finally
				{
					if (graphicsPath != null)
					{
						graphicsPath.Dispose();
					}
					if (brush2 != null)
					{
						brush2.Dispose();
					}
					if (brush != null)
					{
						brush.Dispose();
					}
					if (pen != null)
					{
						pen.Dispose();
					}
					g.EndHotRegion();
				}
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
			foreach (Range state in this.States)
			{
				state.PointerValueChanged(this.Data);
			}
			this.InternalValue = this.Data.Value;
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
			g.DrawSelection(g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f)), (float)(-3.0 / g.Graphics.PageScale), designTimeSelection, this.Common.GaugeCore.SelectionBorderColor, this.Common.GaugeCore.SelectionMarkerColor);
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
			StateIndicator stateIndicator = new StateIndicator();
			binaryFormatSerializer.Deserialize(stateIndicator, stream);
			return stateIndicator;
		}
	}
}
