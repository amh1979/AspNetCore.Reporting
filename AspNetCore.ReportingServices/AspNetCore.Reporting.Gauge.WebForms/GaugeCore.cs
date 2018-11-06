using Microsoft.Win32;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Timers;
using System.Windows.Forms;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class GaugeCore : NamedElement, IDisposable, ISelectable
	{
		private InputValueCollection inputValues;

		private CircularGaugeCollection circularGauges;

		private LinearGaugeCollection linearGauges;

		private NumericIndicatorCollection numericIndicators;

		private StateIndicatorCollection stateIndicators;

		private GaugeImageCollection images;

		private GaugeLabelCollection labels;

		private NamedImageCollection namedImages;

		internal bool silentPaint;

		internal RenderContent renderContent;

		internal string loadedBuildNumber = string.Empty;

		internal bool hasTransparentBackground;

		private MapAreaCollection mapAreas;

		private IRenderable[] renderableElements;

		private BufferBitmap bmpGauge;

		private BufferBitmap bmpFaces;

		private ImageLoader imageLoader;

		internal bool dirtyFlag;

		internal bool disableInvalidate;

		internal bool layoutFlag;

		internal bool isInitializing;

		internal bool refreshPending;

		internal bool boundToDataSource;

		private bool isPrinting;

		private Size printSize = new Size(0, 0);

		internal ServiceContainer serviceContainer;

		private NamedCollection[] elementCollections;

		private TraceManager traceManager;

		public string licenseData = "";

		private string valueExpression = "";

		private string toolTip = "";

		private string multiValueSeparator = "\\#\\";

		private float imageResolution = 96f;

		private GaugeThemes gaugeTheme = GaugeThemes.Default;

		private bool autoLayout = true;

		private double refreshRate = 30.0;

		private BackFrame frame;

		private string topImage = "";

		private Color topImageTransColor = Color.Empty;

		private Color topImageHueColor = Color.Empty;

		private AntiAliasing antiAliasing = AntiAliasing.All;

		private TextAntiAliasingQuality textAntiAliasingQuality = TextAntiAliasingQuality.High;

		private float shadowIntensity = 25f;

		private RightToLeft rightToLeft;

		private Color selectionMarkerColor = Color.LightBlue;

		private Color selectionBorderColor = Color.Black;

		private Color borderColor = Color.Black;

		private GaugeDashStyle borderStyle;

		private int borderWidth = 1;

		private float realTimeDataInterval = 1f;

		private ImageType imageType = ImageType.Png;

		private int compression;

		private string gaugeImageUrl = "TempFiles/GaugePic_#SEQ(300,3)";

		private bool mapEnabled = true;

		private RenderType renderType;

		private Color transparentColor = Color.Empty;

		private SerializationContent viewStateContent = SerializationContent.All;

		private AutoBool renderAsControl = AutoBool.False;

		private string winControlUrl = "";

		private string loadingDataText = "Loading...";

		private string loadingDataImage = "";

		private string loadingControlImage = "";

		private string tagAttributes = "";

		private object dataSource;

		private GaugeSerializer serializer;

		private CallbackManager callbackManager;

		private GaugeContainer parent;

		private HotRegionList hotRegionList;

		private bool serializing;

		private ISelectable selectedDesignTimeElement;

		private bool savingToMetafile;

		public InputValueCollection Values
		{
			get
			{
				return this.inputValues;
			}
		}

		public NamedImageCollection NamedImages
		{
			get
			{
				return this.namedImages;
			}
		}

		public CircularGaugeCollection CircularGauges
		{
			get
			{
				return this.circularGauges;
			}
		}

		public LinearGaugeCollection LinearGauges
		{
			get
			{
				return this.linearGauges;
			}
		}

		public NumericIndicatorCollection NumericIndicators
		{
			get
			{
				return this.numericIndicators;
			}
		}

		public StateIndicatorCollection StateIndicators
		{
			get
			{
				return this.stateIndicators;
			}
		}

		public GaugeImageCollection Images
		{
			get
			{
				return this.images;
			}
		}

		public GaugeLabelCollection Labels
		{
			get
			{
				return this.labels;
			}
		}

		public MapAreaCollection MapAreas
		{
			get
			{
				return this.mapAreas;
			}
		}

		[Browsable(false)]
		[DefaultValue("")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string ValueExpression
		{
			get
			{
				return this.valueExpression;
			}
			set
			{
				if (this.Values.GetByName("Default") == null)
				{
					this.Values.Add("Default");
				}
				this.valueExpression = value;
				double value2 = 0.0;
				if (double.TryParse(value, out value2))
				{
					this.Values["Default"].Value = value2;
				}
			}
		}

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
			}
		}

		[SRDescription("DescriptionAttributeMultiValueSeparator")]
		[DefaultValue("\\#\\")]
		[SRCategory("CategoryGaugeContainer")]
		public string MultiValueSeparator
		{
			get
			{
				return this.multiValueSeparator;
			}
			set
			{
				this.multiValueSeparator = value;
			}
		}

		[SRDescription("DescriptionAttributeImageResolution")]
		[SRCategory("CategoryImage")]
		[DefaultValue(96f)]
		public float ImageResolution
		{
			get
			{
				return this.imageResolution;
			}
			set
			{
				this.imageResolution = value;
			}
		}

		[DefaultValue(GaugeThemes.Default)]
		public GaugeThemes GaugeTheme
		{
			get
			{
				return this.gaugeTheme;
			}
			set
			{
				this.gaugeTheme = value;
			}
		}

		[DefaultValue(true)]
		public bool AutoLayout
		{
			get
			{
				return this.autoLayout;
			}
			set
			{
				this.autoLayout = value;
				this.DoAutoLayout();
				this.Invalidate();
			}
		}

		[DefaultValue(30.0)]
		public double RefreshRate
		{
			get
			{
				return this.refreshRate;
			}
			set
			{
				this.refreshRate = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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

		[DefaultValue("")]
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

		[DefaultValue(typeof(Color), "")]
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

		[DefaultValue(typeof(Color), "")]
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

		[DefaultValue(typeof(AntiAliasing), "All")]
		public AntiAliasing AntiAliasing
		{
			get
			{
				return this.antiAliasing;
			}
			set
			{
				this.antiAliasing = value;
				this.Invalidate();
			}
		}

		[DefaultValue(TextAntiAliasingQuality.High)]
		public TextAntiAliasingQuality TextAntiAliasingQuality
		{
			get
			{
				return this.textAntiAliasingQuality;
			}
			set
			{
				this.textAntiAliasingQuality = value;
				this.Invalidate();
			}
		}

		[DefaultValue(25f)]
		public float ShadowIntensity
		{
			get
			{
				return this.shadowIntensity;
			}
			set
			{
				this.shadowIntensity = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "White")]
		public Color BackColor
		{
			get
			{
				return this.GaugeContainer.BackColor;
			}
			set
			{
				this.GaugeContainer.BackColor = value;
				if (value.A != 255 || value == Color.Empty)
				{
					this.hasTransparentBackground = true;
				}
				else
				{
					this.hasTransparentBackground = false;
				}
				this.Invalidate();
			}
		}

		[DefaultValue(RightToLeft.No)]
		public RightToLeft RightToLeft
		{
			get
			{
				return this.rightToLeft;
			}
			set
			{
				this.rightToLeft = value;
				this.Invalidate();
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		public int Width
		{
			get
			{
				return this.GetWidth();
			}
			set
			{
				if (this.isPrinting)
				{
					this.printSize.Width = value;
				}
				else
				{
					this.GaugeContainer.Width = value;
				}
				this.Invalidate();
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		public int Height
		{
			get
			{
				return this.GetHeight();
			}
			set
			{
				if (this.isPrinting)
				{
					this.printSize.Height = value;
				}
				else
				{
					this.GaugeContainer.Height = value;
				}
				this.Invalidate();
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Name
		{
			get
			{
				return "GaugeContainer";
			}
			set
			{
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeGaugeCore_BuildNumber")]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public string BuildNumber
		{
			get
			{
				string text = string.Empty;
				Assembly executingAssembly = Assembly.GetExecutingAssembly();
				if (executingAssembly != null)
				{
					text = executingAssembly.FullName.ToUpperInvariant();
					int num = text.IndexOf("VERSION=", StringComparison.Ordinal);
					if (num >= 0)
					{
						text = text.Substring(num + 8);
					}
					num = text.IndexOf(",", StringComparison.Ordinal);
					if (num >= 0)
					{
						text = text.Substring(0, num);
					}
				}
				return text;
			}
			set
			{
				this.loadedBuildNumber = value;
			}
		}

		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeGaugeCore_ControlType")]
		public string ControlType
		{
			get
			{
				return "DundasWebGauge";
			}
			set
			{
			}
		}

		[DefaultValue(typeof(Color), "LightBlue")]
		public Color SelectionMarkerColor
		{
			get
			{
				return this.selectionMarkerColor;
			}
			set
			{
				this.selectionMarkerColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "Black")]
		public Color SelectionBorderColor
		{
			get
			{
				return this.selectionBorderColor;
			}
			set
			{
				this.selectionBorderColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "Black")]
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

		[DefaultValue(1f)]
		public float RealTimeDataInterval
		{
			get
			{
				return this.realTimeDataInterval;
			}
			set
			{
				this.realTimeDataInterval = value;
			}
		}

		[DefaultValue(ImageType.Png)]
		public ImageType ImageType
		{
			get
			{
				return this.imageType;
			}
			set
			{
				this.imageType = value;
			}
		}

		[DefaultValue(0)]
		public int Compression
		{
			get
			{
				return this.compression;
			}
			set
			{
				this.compression = value;
			}
		}

		[DefaultValue("TempFiles/GaugePic_#SEQ(300,3)")]
		public string ImageUrl
		{
			get
			{
				return this.gaugeImageUrl;
			}
			set
			{
				this.gaugeImageUrl = value;
			}
		}

		[DefaultValue(true)]
		public bool MapEnabled
		{
			get
			{
				return this.mapEnabled;
			}
			set
			{
				this.mapEnabled = value;
			}
		}

		[DefaultValue(RenderType.ImageTag)]
		public RenderType RenderType
		{
			get
			{
				return this.renderType;
			}
			set
			{
				this.renderType = value;
			}
		}

		[DefaultValue(typeof(Color), "")]
		public Color TransparentColor
		{
			get
			{
				return this.transparentColor;
			}
			set
			{
				this.transparentColor = value;
			}
		}

		[DefaultValue(SerializationContent.All)]
		public SerializationContent ViewStateContent
		{
			get
			{
				return this.viewStateContent;
			}
			set
			{
				this.viewStateContent = value;
			}
		}

		[DefaultValue(AutoBool.False)]
		public AutoBool RenderAsControl
		{
			get
			{
				return this.renderAsControl;
			}
			set
			{
				this.renderAsControl = value;
			}
		}

		[DefaultValue("")]
		public string WinControlUrl
		{
			get
			{
				return this.winControlUrl;
			}
			set
			{
				this.winControlUrl = value;
			}
		}

		[DefaultValue("Loading...")]
		public string LoadingDataText
		{
			get
			{
				return this.loadingDataText;
			}
			set
			{
				this.loadingDataText = value;
			}
		}

		[DefaultValue("")]
		public string LoadingDataImage
		{
			get
			{
				return this.loadingDataImage;
			}
			set
			{
				this.loadingDataImage = value;
			}
		}

		[DefaultValue("")]
		public string LoadingControlImage
		{
			get
			{
				return this.loadingControlImage;
			}
			set
			{
				this.loadingControlImage = value;
			}
		}

		[DefaultValue("")]
		public string TagAttributes
		{
			get
			{
				return this.tagAttributes;
			}
			set
			{
				this.tagAttributes = value;
			}
		}

		internal object DataSource
		{
			get
			{
				return this.dataSource;
			}
			set
			{
				this.dataSource = value;
				this.boundToDataSource = false;
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		internal GaugeSerializer Serializer
		{
			get
			{
				return this.serializer;
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		internal CallbackManager CallbackManager
		{
			get
			{
				return this.callbackManager;
			}
		}

		internal GaugeContainer GaugeContainer
		{
			get
			{
				return this.parent;
			}
		}

		internal HotRegionList HotRegionList
		{
			get
			{
				if (this.hotRegionList == null)
				{
					this.hotRegionList = new HotRegionList(this);
				}
				return this.hotRegionList;
			}
		}

		internal bool Serializing
		{
			get
			{
				return this.serializing;
			}
			set
			{
				this.serializing = value;
			}
		}

		internal ISelectable SelectedDesignTimeElement
		{
			get
			{
				return this.selectedDesignTimeElement;
			}
			set
			{
				this.selectedDesignTimeElement = value;
				this.Invalidate();
			}
		}

		internal bool SavingToMetafile
		{
			get
			{
				return this.savingToMetafile;
			}
			set
			{
				this.savingToMetafile = value;
			}
		}

		internal bool InvokeRequired
		{
			get
			{
				return false;
			}
		}

		public GaugeCore()
		{
			this.parent = null;
			this.inputValues = new InputValueCollection(this, null);
			this.circularGauges = new CircularGaugeCollection(this, null);
			this.linearGauges = new LinearGaugeCollection(this, null);
			this.numericIndicators = new NumericIndicatorCollection(this, null);
			this.stateIndicators = new StateIndicatorCollection(this, null);
			this.images = new GaugeImageCollection(this, null);
			this.labels = new GaugeLabelCollection(this, null);
			this.frame = new BackFrame(this, BackFrameStyle.None, BackFrameShape.Rectangular);
			this.mapAreas = new MapAreaCollection();
			this.namedImages = new NamedImageCollection(this, base.common);
		}

		internal GaugeCore(GaugeContainer parent)
		{
			this.parent = parent;
			this.serviceContainer = new ServiceContainer();
			this.serviceContainer.AddService(typeof(GaugeCore), this);
			this.serviceContainer.AddService(typeof(GaugeContainer), parent);
			base.common = new CommonElements(this.serviceContainer);
			this.inputValues = new InputValueCollection(this, base.common);
			this.circularGauges = new CircularGaugeCollection(this, base.common);
			this.linearGauges = new LinearGaugeCollection(this, base.common);
			this.numericIndicators = new NumericIndicatorCollection(this, base.common);
			this.stateIndicators = new StateIndicatorCollection(this, base.common);
			this.images = new GaugeImageCollection(this, base.common);
			this.labels = new GaugeLabelCollection(this, base.common);
			this.frame = new BackFrame(this, BackFrameStyle.None, BackFrameShape.Rectangular);
			this.serializer = new GaugeSerializer(this.serviceContainer);
			this.mapAreas = new MapAreaCollection();
			this.callbackManager = new CallbackManager();
			this.traceManager = new TraceManager(this.serviceContainer);
			this.serviceContainer.AddService(this.traceManager.GetType(), this.traceManager);
			this.namedImages = new NamedImageCollection(this, base.common);
			this.imageLoader = new ImageLoader(this.serviceContainer);
			this.serviceContainer.AddService(typeof(ImageLoader), this.imageLoader);
		}

		internal void DoAutoLayout()
		{
			if (this.AutoLayout && this.GetWidth() != 0 && this.GetHeight() != 0)
			{
				CircularGauge[] circularAutoLayoutGauges = this.GetCircularAutoLayoutGauges();
				LinearGauge[] linearAutoLayoutGauges = this.GetLinearAutoLayoutGauges();
				StateIndicator[] autoLayoutStateIndicators = this.GetAutoLayoutStateIndicators();
				int num = circularAutoLayoutGauges.Length + linearAutoLayoutGauges.Length + autoLayoutStateIndicators.Length;
				if (num != 0)
				{
					this.layoutFlag = true;
					float num2 = (float)((float)circularAutoLayoutGauges.Length / (float)num * 100.0);
					float num3 = (float)((float)linearAutoLayoutGauges.Length / (float)num * 100.0);
					float num4 = (float)((float)autoLayoutStateIndicators.Length / (float)num * 100.0);
					RectangleF rect = RectangleF.Empty;
					RectangleF rectangleF = RectangleF.Empty;
					RectangleF rect2 = RectangleF.Empty;
					if (this.GetWidth() > this.GetHeight())
					{
						rect = new RectangleF(0f, 0f, num2, 100f);
						rectangleF = new RectangleF(num2, 0f, (float)(100.0 - num2 - num4), 100f);
						rect2 = new RectangleF(num2 + num3, 0f, (float)(100.0 - num2 - num3), 100f);
					}
					else
					{
						rect = new RectangleF(0f, 0f, 100f, num2);
						rectangleF = new RectangleF(0f, num2, 100f, (float)(100.0 - num2 - num4));
						rect2 = new RectangleF(0f, num2 + num3, 100f, (float)(100.0 - num2 - num3));
					}
					if (circularAutoLayoutGauges.Length != 0)
					{
						int num5 = (int)((float)this.GetWidth() * rect.Width / 100.0);
						int num6 = (int)((float)this.GetHeight() * rect.Height / 100.0);
						bool stackHorizontally;
						float num7;
						if (num5 > num6)
						{
							stackHorizontally = true;
							num7 = (float)num6 / (float)num5;
						}
						else
						{
							stackHorizontally = false;
							num7 = (float)num5 / (float)num6;
						}
						if (circularAutoLayoutGauges.Length % 3 == 0 && circularAutoLayoutGauges.Length > 6 && num7 > 1.0 / (float)(circularAutoLayoutGauges.Length / 3))
						{
							this.LayoutMatrix(circularAutoLayoutGauges, rect, stackHorizontally, 3);
						}
						else if (circularAutoLayoutGauges.Length % 2 == 0 && circularAutoLayoutGauges.Length > 2 && num7 > 1.0 / (float)(circularAutoLayoutGauges.Length / 2))
						{
							this.LayoutMatrix(circularAutoLayoutGauges, rect, stackHorizontally, 2);
						}
						else
						{
							this.LayoutSingleLine(circularAutoLayoutGauges, rect, stackHorizontally);
						}
					}
					if (linearAutoLayoutGauges.Length != 0)
					{
						float num8 = (float)((float)this.GetWidth() * rectangleF.Width / 100.0);
						float num9 = (float)((float)this.GetHeight() * rectangleF.Height / 100.0);
						LinearGauge[] array = default(LinearGauge[]);
						LinearGauge[] array2 = default(LinearGauge[]);
						this.SplitAutoLayoutGauges(linearAutoLayoutGauges, num8, num9, out array, out array2);
						if (array.Length == 0 || array2.Length == 0)
						{
							bool stackHorizontally2 = this.ShouldStackHorizontally(linearAutoLayoutGauges, rectangleF);
							this.LayoutSingleLine(linearAutoLayoutGauges, rectangleF, stackHorizontally2);
						}
						else if (num8 > num9)
						{
							RectangleF rectangleF2 = rectangleF;
							rectangleF2.Width /= 2f;
							RectangleF rectangleF3 = rectangleF;
							rectangleF3.X = rectangleF2.Width;
							rectangleF3.Width -= rectangleF2.Width;
							bool stackHorizontally3 = this.ShouldStackHorizontally(array, rectangleF2);
							this.LayoutSingleLine(array, rectangleF2, stackHorizontally3);
							bool flag = this.ShouldStackHorizontally(array2, rectangleF3);
							this.LayoutSingleLine(array2, rectangleF3, flag);
							float num10 = 0f;
							LinearGauge[] array3 = array2;
							foreach (LinearGauge linearGauge in array3)
							{
								num10 = ((!flag) ? Math.Max(num10, linearGauge.GetAspectRatioBounds().Width) : (num10 + linearGauge.GetAspectRatioBounds().Width));
							}
							rectangleF2.Width = rectangleF.Width - num10;
							rectangleF3.X = rectangleF2.Width;
							rectangleF3.Width = num10;
							this.LayoutSingleLine(array, rectangleF2, stackHorizontally3);
							this.LayoutSingleLine(array2, rectangleF3, flag);
						}
						else
						{
							RectangleF rectangleF4 = rectangleF;
							rectangleF4.Height /= 2f;
							RectangleF rectangleF5 = rectangleF;
							rectangleF5.Y = rectangleF4.Height;
							rectangleF5.Height -= rectangleF4.Height;
							bool flag2 = this.ShouldStackHorizontally(array, rectangleF4);
							this.LayoutSingleLine(array, rectangleF4, flag2);
							bool stackHorizontally4 = this.ShouldStackHorizontally(array2, rectangleF5);
							this.LayoutSingleLine(array2, rectangleF5, stackHorizontally4);
							float num11 = 0f;
							LinearGauge[] array4 = array;
							foreach (LinearGauge linearGauge2 in array4)
							{
								num11 = (flag2 ? Math.Max(num11, linearGauge2.GetAspectRatioBounds().Height) : (num11 + linearGauge2.GetAspectRatioBounds().Height));
							}
							rectangleF4.Height = num11;
							rectangleF5.Y = num11;
							rectangleF5.Height = rectangleF.Height - num11;
							this.LayoutSingleLine(array, rectangleF4, flag2);
							this.LayoutSingleLine(array2, rectangleF5, stackHorizontally4);
						}
					}
					if (autoLayoutStateIndicators.Length != 0)
					{
						rect2.Inflate(-4f, -4f);
						int num12 = (int)((float)this.GetWidth() * rect2.Width / 100.0);
						int num13 = (int)((float)this.GetHeight() * rect2.Height / 100.0);
						bool stackHorizontally5 = (byte)((num12 > num13) ? 1 : 0) != 0;
						this.LayoutSingleLine(autoLayoutStateIndicators, rect2, stackHorizontally5);
					}
					this.layoutFlag = false;
				}
			}
		}

		private bool ShouldStackHorizontally(LinearGauge[] linearGauges, RectangleF availableRect)
		{
			float num = (float)((float)this.GetWidth() * availableRect.Width / 100.0);
			float num2 = (float)((float)this.GetHeight() * availableRect.Height / 100.0);
			float horizontalStackAspectRatio = this.GetHorizontalStackAspectRatio(linearGauges, num, num2);
			float verticalStackAspectRatio = this.GetVerticalStackAspectRatio(linearGauges, num, num2);
			float num3 = num / num2;
			float num4 = Math.Abs(num3 - horizontalStackAspectRatio);
			float num5 = Math.Abs(num3 - verticalStackAspectRatio);
			return num4 > num5;
		}

		private float GetHorizontalStackAspectRatio(LinearGauge[] horizontalGauges, float pixelWidth, float pixelHeight)
		{
			if (horizontalGauges.Length == 0)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < horizontalGauges.Length; i++)
			{
				num = (float)(num + 1.0 / this.GetPreferredAspectRatio(horizontalGauges[i], pixelWidth, pixelHeight));
			}
			return (float)(1.0 / num);
		}

		private float GetVerticalStackAspectRatio(LinearGauge[] verticalGauges, float pixelWidth, float pixelHeight)
		{
			if (verticalGauges.Length == 0)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < verticalGauges.Length; i++)
			{
				num += this.GetPreferredAspectRatio(verticalGauges[i], pixelWidth, pixelHeight);
			}
			return num;
		}

		private void SplitAutoLayoutGauges(LinearGauge[] autoLayoutGauges, float pixelWidth, float pixelHeight, out LinearGauge[] horizontalGauges, out LinearGauge[] verticalGauges)
		{
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			foreach (LinearGauge linearGauge in autoLayoutGauges)
			{
				if (this.GetPreferredAspectRatio(linearGauge, pixelWidth, pixelHeight) >= 1.0)
				{
					arrayList.Add(linearGauge);
				}
				else
				{
					arrayList2.Add(linearGauge);
				}
			}
			horizontalGauges = (LinearGauge[])arrayList.ToArray(typeof(LinearGauge));
			verticalGauges = (LinearGauge[])arrayList2.ToArray(typeof(LinearGauge));
		}

		private float GetPreferredAspectRatio(GaugeBase gauge, float pixelWidth, float pixelHeight)
		{
			if (!float.IsNaN(gauge.AspectRatio))
			{
				return gauge.AspectRatio;
			}
			if (gauge is CircularGauge)
			{
				return 1f;
			}
			LinearGauge linearGauge = (LinearGauge)gauge;
			if (linearGauge.Orientation == GaugeOrientation.Horizontal)
			{
				return 5f;
			}
			if (linearGauge.Orientation == GaugeOrientation.Vertical)
			{
				return 0.2f;
			}
			return pixelWidth / pixelHeight;
		}

		internal CircularGauge[] GetCircularAutoLayoutGauges()
		{
			ArrayList arrayList = new ArrayList();
			foreach (CircularGauge circularGauge in this.CircularGauges)
			{
				if (circularGauge.Parent == string.Empty)
				{
					arrayList.Add(circularGauge);
				}
			}
			return (CircularGauge[])arrayList.ToArray(typeof(CircularGauge));
		}

		internal LinearGauge[] GetLinearAutoLayoutGauges()
		{
			ArrayList arrayList = new ArrayList();
			foreach (LinearGauge linearGauge in this.LinearGauges)
			{
				if (linearGauge.Parent == string.Empty)
				{
					arrayList.Add(linearGauge);
				}
			}
			return (LinearGauge[])arrayList.ToArray(typeof(LinearGauge));
		}

		internal StateIndicator[] GetAutoLayoutStateIndicators()
		{
			ArrayList arrayList = new ArrayList();
			foreach (StateIndicator stateIndicator in this.StateIndicators)
			{
				if (stateIndicator.Parent == string.Empty)
				{
					arrayList.Add(stateIndicator);
				}
			}
			return (StateIndicator[])arrayList.ToArray(typeof(StateIndicator));
		}

		private void LayoutSingleLine(GaugeBase[] gauges, RectangleF rect, bool stackHorizontally)
		{
			if (stackHorizontally)
			{
				float num = rect.Width / (float)gauges.Length;
				float num2 = rect.X;
				foreach (GaugeBase gaugeBase in gauges)
				{
					gaugeBase.Location.X = num2;
					gaugeBase.Location.Y = rect.Y;
					gaugeBase.Size.Width = num;
					gaugeBase.Size.Height = rect.Height;
					this.CompensateForShadowAndBorder(gaugeBase);
					num2 += num;
				}
			}
			else
			{
				float num3 = rect.Height / (float)gauges.Length;
				float num4 = rect.Y;
				foreach (GaugeBase gaugeBase2 in gauges)
				{
					gaugeBase2.Location.X = rect.X;
					gaugeBase2.Location.Y = num4;
					gaugeBase2.Size.Width = rect.Width;
					gaugeBase2.Size.Height = num3;
					this.CompensateForShadowAndBorder(gaugeBase2);
					num4 += num3;
				}
			}
		}

		private void CompensateForShadowAndBorder(GaugeBase gauge)
		{
			if (gauge.BackFrame.BorderWidth > 1 && gauge.BackFrame.BorderStyle != 0 && gauge.BackFrame.BorderColor.A != 0)
			{
				float num = (float)((float)gauge.BackFrame.BorderWidth / (float)this.GetWidth() * 100.0);
				float num2 = (float)((float)gauge.BackFrame.BorderWidth / (float)this.GetHeight() * 100.0);
				gauge.Location.X += (float)(num / 2.0);
				gauge.Location.Y += (float)(num2 / 2.0);
				gauge.Size.Width -= num;
				gauge.Size.Height -= num2;
			}
			if (gauge.BackFrame.ShadowOffset != 0.0)
			{
				float num3 = (float)(gauge.BackFrame.ShadowOffset / (float)this.GetWidth() * 100.0);
				float num4 = (float)(gauge.BackFrame.ShadowOffset / (float)this.GetHeight() * 100.0);
				if (gauge.BackFrame.ShadowOffset < 0.0)
				{
					gauge.Location.X -= num3;
					gauge.Location.Y -= num4;
					gauge.Size.Width += num3;
					gauge.Size.Height += num4;
				}
				else
				{
					gauge.Size.Width -= num3;
					gauge.Size.Height -= num4;
				}
			}
		}

		private void LayoutMatrix(GaugeBase[] gauges, RectangleF rect, bool stackHorizontally, int lineCount)
		{
			if (stackHorizontally)
			{
				float num = rect.Height / (float)lineCount;
				float num2 = rect.Y;
				int num3 = 0;
				for (int i = 0; i < lineCount; i++)
				{
					RectangleF rect2 = new RectangleF(rect.X, num2, rect.Width, num);
					int num4 = gauges.Length / lineCount;
					GaugeBase[] array = new GaugeBase[num4];
					for (int j = 0; j < num4; j++)
					{
						array[j] = gauges[num3++];
					}
					this.LayoutSingleLine(array, rect2, stackHorizontally);
					num2 += num;
				}
			}
			else
			{
				float num6 = rect.Width / (float)lineCount;
				float num7 = rect.X;
				int num8 = 0;
				for (int k = 0; k < lineCount; k++)
				{
					RectangleF rect3 = new RectangleF(num7, rect.Y, num6, rect.Height);
					int num9 = gauges.Length / lineCount;
					GaugeBase[] array2 = new GaugeBase[num9];
					for (int l = 0; l < num9; l++)
					{
						array2[l] = gauges[num8++];
					}
					this.LayoutSingleLine(array2, rect3, stackHorizontally);
					num7 += num6;
				}
			}
		}

		private void LayoutSingleLine(StateIndicator[] indicators, RectangleF rect, bool stackHorizontally)
		{
			if (stackHorizontally)
			{
				float num = rect.Width / (float)indicators.Length;
				float num2 = rect.X;
				foreach (StateIndicator stateIndicator in indicators)
				{
					stateIndicator.Location.X = num2;
					stateIndicator.Location.Y = rect.Y;
					stateIndicator.Size.Width = num;
					stateIndicator.Size.Height = rect.Height;
					num2 += num;
				}
			}
			else
			{
				float num3 = rect.Height / (float)indicators.Length;
				float num4 = rect.Y;
				foreach (StateIndicator stateIndicator2 in indicators)
				{
					stateIndicator2.Location.X = rect.X;
					stateIndicator2.Location.Y = num4;
					stateIndicator2.Size.Width = rect.Width;
					stateIndicator2.Size.Height = num3;
					num4 += num3;
				}
			}
		}

		internal int GetWidth()
		{
			if (this.isPrinting)
			{
				return this.printSize.Width;
			}
			return this.GaugeContainer.Width;
		}

		internal int GetHeight()
		{
			if (this.isPrinting)
			{
				return this.printSize.Height;
			}
			return this.GaugeContainer.Height;
		}

		internal void ResetAutoValues()
		{
			if (this.selectedDesignTimeElement != null)
			{
				this.selectedDesignTimeElement = null;
			}
		}

		internal override void Invalidate()
		{
			if (!this.layoutFlag)
			{
				this.dirtyFlag = true;
				base.common.ObjectLinker.Invalidate();
				if (!this.disableInvalidate)
				{
					this.Refresh();
				}
			}
		}

		internal void OnFontChanged()
		{
		}

		private void refreshTimer_Elapsed(object source, ElapsedEventArgs e)
		{
			if (this.refreshPending)
			{
				this.refreshPending = false;
				this.GaugeContainer.Invalidate();
			}
		}

		internal override void Refresh()
		{
		}

		internal NamedCollection[] GetRenderCollections()
		{
			if (this.elementCollections == null)
			{
				this.elementCollections = new NamedCollection[6];
				this.elementCollections[0] = this.CircularGauges;
				this.elementCollections[1] = this.LinearGauges;
				this.elementCollections[2] = this.Labels;
				this.elementCollections[3] = this.NumericIndicators;
				this.elementCollections[4] = this.StateIndicators;
				this.elementCollections[5] = this.Images;
			}
			return this.elementCollections;
		}

		internal override void BeginInit()
		{
			this.isInitializing = true;
			this.Values.BeginInit();
			NamedCollection[] renderCollections = this.GetRenderCollections();
			foreach (NamedCollection namedCollection in renderCollections)
			{
				namedCollection.BeginInit();
			}
		}

		internal override void EndInit()
		{
			this.isInitializing = false;
			this.Values.EndInit();
			NamedCollection[] renderCollections = this.GetRenderCollections();
			foreach (NamedCollection namedCollection in renderCollections)
			{
				namedCollection.EndInit();
			}
		}

		internal override void ReconnectData(bool exact)
		{
			this.Values.ReconnectData(exact);
			NamedCollection[] renderCollections = this.GetRenderCollections();
			foreach (NamedCollection namedCollection in renderCollections)
			{
				namedCollection.ReconnectData(exact);
			}
		}

		internal override void Notify(MessageType msg, NamedElement element, object param)
		{
			NamedCollection[] renderCollections = this.GetRenderCollections();
			foreach (NamedCollection namedCollection in renderCollections)
			{
				namedCollection.Notify(msg, element, param);
			}
		}

		private BufferBitmap InitBitmap(BufferBitmap bmp, float dpiX, float dpiY)
		{
			return this.InitBitmap(bmp, new Size(this.GetWidth(), this.GetHeight()), dpiX, dpiY);
		}

		internal BufferBitmap InitBitmap(BufferBitmap bmp, Size size, float dpiX, float dpiY)
		{
			if (bmp == null)
			{
				bmp = new BufferBitmap(dpiX, dpiY);
			}
			else if (this.dirtyFlag)
			{
				bmp.Invalidate();
			}
			bmp.Size = size;
			bmp.Graphics.SmoothingMode = this.GetSmootingMode();
			bmp.Graphics.TextRenderingHint = this.GetTextRenderingHint();
			bmp.Graphics.TextContrast = 2;
			return bmp;
		}

		private SmoothingMode GetSmootingMode()
		{
			if ((this.GaugeContainer.AntiAliasing & AntiAliasing.Graphics) == AntiAliasing.Graphics)
			{
				return SmoothingMode.HighQuality;
			}
			return SmoothingMode.HighSpeed;
		}

		private TextRenderingHint GetTextRenderingHint()
		{
			TextRenderingHint textRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
			if ((this.GaugeContainer.AntiAliasing & AntiAliasing.Text) == AntiAliasing.Text)
			{
				textRenderingHint = TextRenderingHint.ClearTypeGridFit;
				if (this.TextAntiAliasingQuality == TextAntiAliasingQuality.Normal)
				{
					textRenderingHint = TextRenderingHint.AntiAlias;
				}
				else if (this.TextAntiAliasingQuality == TextAntiAliasingQuality.SystemDefault)
				{
					textRenderingHint = TextRenderingHint.SystemDefault;
				}
			}
			else
			{
				textRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
			}
			return textRenderingHint;
		}

		public GaugeGraphics GetGraphics(RenderingType renderingType, Graphics g, Stream outputStream)
		{
			GaugeGraphics gaugeGraphics = new GaugeGraphics(this.Common);
			this.Common.Height = this.GetHeight();
			this.Common.Width = this.GetWidth();
			gaugeGraphics.SetPictureSize((float)this.Common.Width, (float)this.Common.Height);
			gaugeGraphics.ActiveRenderingType = renderingType;
			gaugeGraphics.Graphics = g;
			gaugeGraphics.SmoothingMode = this.GetSmootingMode();
			gaugeGraphics.TextRenderingHint = this.GetTextRenderingHint();
			return gaugeGraphics;
		}

		internal IRenderable[] GetGraphElements()
		{
			if (this.renderableElements == null || this.dirtyFlag)
			{
				ArrayList arrayList = new ArrayList();
				NamedCollection[] renderCollections = this.GetRenderCollections();
				foreach (NamedCollection c in renderCollections)
				{
					arrayList.AddRange(c);
				}
				ArrayList collection = (ArrayList)arrayList.Clone();
				arrayList.Sort(new ZOrderSort(collection));
				this.renderableElements = (IRenderable[])arrayList.ToArray(typeof(IRenderable));
			}
			return this.renderableElements;
		}

		private void RenderWaterMark(GaugeGraphics g)
		{
		}

		internal void RenderOneDynamicElement(GaugeGraphics g, IRenderable element, bool renderChildrenFirst)
		{
			try
			{
				RectangleF rectangleF = element.GetBoundRect(g);
				g.CreateDrawRegion(rectangleF);
				rectangleF = g.GetAbsoluteRectangle(rectangleF);
				float num = (float)((element is GaugeBase) ? 0.10000000149011612 : 0.0099999997764825821);
				if (rectangleF.Width > num && rectangleF.Height > num)
				{
					if (renderChildrenFirst)
					{
						this.RenderElements(g, element, false);
					}
					element.RenderDynamicElements(g);
					if (!renderChildrenFirst)
					{
						this.RenderElements(g, element, false);
					}
					base.common.InvokePostPaint(element);
				}
			}
			finally
			{
				g.RestoreDrawRegion();
			}
		}

		internal void RenderElements(GaugeGraphics g, IRenderable parentElement, bool renderStaticElements)
		{
			IRenderable[] graphElements = this.GetGraphElements();
			ArrayList arrayList = new ArrayList();
			IRenderable[] array = graphElements;
			foreach (IRenderable renderable in array)
			{
				if (renderable.GetParentRenderable() == parentElement)
				{
					arrayList.Add(renderable);
				}
			}
			if (arrayList.Count > 0)
			{
				arrayList.Sort(new ZOrderSort(arrayList));
				if (renderStaticElements)
				{
					this.TraceWrite("GaugePaint", SR.TraceStartingRenderingStaticElements);
					for (int j = 0; j < arrayList.Count; j++)
					{
						try
						{
							RectangleF rectangleF = ((IRenderable)arrayList[j]).GetBoundRect(g);
							g.CreateDrawRegion(rectangleF);
							rectangleF = g.GetAbsoluteRectangle(rectangleF);
							if ((double)rectangleF.Width > 0.1 && (double)rectangleF.Height > 0.1)
							{
								((IRenderable)arrayList[j]).RenderStaticElements(g);
								base.common.InvokePrePaint(arrayList[j]);
								this.RenderElements(g, (IRenderable)arrayList[j], renderStaticElements);
							}
						}
						finally
						{
							g.RestoreDrawRegion();
						}
					}
					this.TraceWrite("GaugePaint", SR.TraceFinishedRenderingStaticElements);
				}
				else
				{
					this.TraceWrite("GaugePaint", SR.TraceStartingRenderingDynamicElements);
					for (int k = 0; k < arrayList.Count; k++)
					{
						IRenderable renderable2 = (IRenderable)arrayList[k];
						if (renderable2 is NumericIndicator || renderable2 is StateIndicator || renderable2 is GaugeLabel)
						{
							this.RenderOneDynamicElement(g, renderable2, false);
						}
					}
					for (int num = arrayList.Count - 1; num >= 0; num--)
					{
						IRenderable renderable3 = (IRenderable)arrayList[num];
						if (!(renderable3 is NumericIndicator) && !(renderable3 is StateIndicator) && !(renderable3 is GaugeLabel))
						{
							this.RenderOneDynamicElement(g, renderable3, true);
						}
					}
					this.TraceWrite("GaugePaint", SR.TraceFinishedRenderingDynamicElements);
				}
			}
		}

		internal void RenderSelection(GaugeGraphics g)
		{
			NamedElement[] selectedElements = this.GetSelectedElements();
			if (selectedElements != null)
			{
				NamedElement[] array = selectedElements;
				foreach (NamedElement namedElement in array)
				{
					ISelectable selectable = namedElement as ISelectable;
					if (selectable != null)
					{
						selectable.DrawSelection(g, false);
					}
				}
			}
			if (this.selectedDesignTimeElement != null)
			{
				this.selectedDesignTimeElement.DrawSelection(g, true);
			}
		}

		internal NamedElement[] GetSelectedElements()
		{
			ArrayList arrayList = new ArrayList();
			foreach (CircularGauge circularGauge in this.CircularGauges)
			{
				if (circularGauge.Selected)
				{
					arrayList.Add(circularGauge);
				}
				foreach (CircularScale scale in circularGauge.Scales)
				{
					if (scale.Selected)
					{
						arrayList.Add(scale);
					}
				}
				foreach (CircularRange range in circularGauge.Ranges)
				{
					if (range.Selected)
					{
						arrayList.Add(range);
					}
				}
				foreach (CircularPointer pointer in circularGauge.Pointers)
				{
					if (pointer.Selected)
					{
						arrayList.Add(pointer);
					}
				}
				foreach (Knob knob in circularGauge.Knobs)
				{
					if (knob.Selected)
					{
						arrayList.Add(knob);
					}
				}
			}
			foreach (LinearGauge linearGauge in this.LinearGauges)
			{
				if (linearGauge.Selected)
				{
					arrayList.Add(linearGauge);
				}
				foreach (LinearScale scale2 in linearGauge.Scales)
				{
					if (scale2.Selected)
					{
						arrayList.Add(scale2);
					}
				}
				foreach (LinearRange range2 in linearGauge.Ranges)
				{
					if (range2.Selected)
					{
						arrayList.Add(range2);
					}
				}
				foreach (LinearPointer pointer2 in linearGauge.Pointers)
				{
					if (pointer2.Selected)
					{
						arrayList.Add(pointer2);
					}
				}
			}
			foreach (NumericIndicator numericIndicator in this.NumericIndicators)
			{
				if (numericIndicator.Selected)
				{
					arrayList.Add(numericIndicator);
				}
			}
			foreach (StateIndicator stateIndicator in this.StateIndicators)
			{
				if (stateIndicator.Selected)
				{
					arrayList.Add(stateIndicator);
				}
			}
			foreach (GaugeImage image in this.Images)
			{
				if (image.Selected)
				{
					arrayList.Add(image);
				}
			}
			foreach (GaugeLabel label in this.Labels)
			{
				if (label.Selected)
				{
					arrayList.Add(label);
				}
			}
			if (arrayList.Count == 0)
			{
				return null;
			}
			return (NamedElement[])arrayList.ToArray(typeof(NamedElement));
		}

		internal void RenderTopImage(GaugeGraphics g)
		{
			if (this.TopImage != "")
			{
				ImageAttributes imageAttributes = new ImageAttributes();
				if (this.TopImageTransColor != Color.Empty)
				{
					imageAttributes.SetColorKey(this.TopImageTransColor, this.TopImageTransColor, ColorAdjustType.Default);
				}
				Image image = this.Common.ImageLoader.LoadImage(this.TopImage);
				Rectangle destRect = new Rectangle(0, 0, this.GetWidth(), this.GetHeight());
				if (!this.TopImageHueColor.IsEmpty)
				{
					Color color = g.TransformHueColor(this.TopImageHueColor);
					ColorMatrix colorMatrix = new ColorMatrix();
					colorMatrix.Matrix00 = (float)((float)(int)color.R / 255.0);
					colorMatrix.Matrix11 = (float)((float)(int)color.G / 255.0);
					colorMatrix.Matrix22 = (float)((float)(int)color.B / 255.0);
					imageAttributes.SetColorMatrix(colorMatrix);
				}
				ImageSmoothingState imageSmoothingState = new ImageSmoothingState(g);
				imageSmoothingState.Set();
				g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				imageSmoothingState.Restore();
			}
		}

		internal void RenderStaticElements(GaugeGraphics g)
		{
			if (this.renderContent != RenderContent.Dynamic)
			{
				Color backColor = this.GaugeContainer.BackColor;
				using (Brush brush = new SolidBrush(backColor))
				{
					g.FillRectangle(brush, new Rectangle(new Point(-1, -1), new Size(this.GetWidth() + 2, this.GetHeight() + 2)));
				}
				this.HotRegionList.Clear();
				this.HotRegionList.SetHotRegion(this);
				this.BackFrame.RenderFrame(g);
				this.RenderBorder(g);
				this.RenderElements(g, null, true);
			}
		}

		private void RenderBorder(GaugeGraphics g)
		{
			if (this.BorderStyle != 0 && this.BorderColor.A != 0 && this.BorderWidth != 0)
			{
				using (Pen pen = new Pen(this.BorderColor, (float)this.BorderWidth))
				{
					pen.DashStyle = g.GetPenStyle(this.BorderStyle);
					pen.Alignment = PenAlignment.Inset;
					using (GraphicsPath graphicsPath = new GraphicsPath())
					{
						if (g.Graphics.PageScale > 1.0)
						{
							graphicsPath.AddRectangle(new RectangleF(0f, 0f, (float)this.GetWidth(), (float)this.GetHeight()));
						}
						else
						{
							graphicsPath.AddRectangle(new RectangleF(0f, 0f, (float)(this.GetWidth() - 1), (float)(this.GetHeight() - 1)));
						}
						AntiAliasing antiAliasing = g.AntiAliasing;
						g.AntiAliasing = AntiAliasing.None;
						g.DrawPath(pen, graphicsPath);
						g.AntiAliasing = antiAliasing;
					}
				}
			}
		}

		internal void RenderStaticElementsBufered(GaugeGraphics g)
		{
			if (this.dirtyFlag || this.bmpFaces == null)
			{
				this.bmpFaces = this.InitBitmap(this.bmpFaces, g.Graphics.DpiX, g.Graphics.DpiY);
				Graphics graphics = g.Graphics;
				try
				{
					g.Graphics = this.bmpFaces.Graphics;
					this.HotRegionList.Clear();
					this.RenderStaticElements(g);
				}
				finally
				{
					g.Graphics = graphics;
				}
			}
			g.DrawImage(this.bmpFaces.Bitmap, new Rectangle(new Point(0, 0), this.bmpFaces.Size));
		}

		internal void RenderDynamicElements(GaugeGraphics g)
		{
			if (this.renderContent != RenderContent.Static)
			{
				this.RenderElements(g, null, false);
				this.BackFrame.RenderGlassEffect(g);
				this.RenderTopImage(g);
				this.RenderSelection(g);
			}
		}

		internal void Paint(Graphics gdiGraph, RenderingType renderingType, Stream stream, bool buffered)
		{
			this.TraceWrite("GaugePaint", SR.TraceStartingPaint);
			this.disableInvalidate = true;
			GaugeGraphics graphics = this.GetGraphics(renderingType, gdiGraph, stream);
			try
			{
				this.AutoDataBind(false);
				this.DoAutoLayout();
				if (buffered)
				{
					this.RenderStaticElementsBufered(graphics);
				}
				else
				{
					this.RenderStaticElements(graphics);
				}
				this.RenderDynamicElements(graphics);
				this.RenderWaterMark(graphics);
			}
			finally
			{
				this.disableInvalidate = false;
				graphics.Close();
			}
			this.dirtyFlag = false;
			this.TraceWrite("GaugePaint", SR.TracePaintComplete);
		}

		internal void PrintPaint(Graphics g, Rectangle position)
		{
			this.Notify(MessageType.PrepareSnapShot, this, null);
			this.printSize = position.Size;
			GraphicsState gstate = g.Save();
			try
			{
				this.isPrinting = true;
				g.TranslateTransform((float)position.X, (float)position.Y);
				this.Paint(g, RenderingType.Gdi, null, false);
			}
			finally
			{
				g.Restore(gstate);
				this.isPrinting = false;
			}
		}

		internal void Paint(Graphics g)
		{
			this.disableInvalidate = true;
			this.bmpGauge = this.InitBitmap(this.bmpGauge, g.DpiX, g.DpiY);
			this.Paint(this.bmpGauge.Graphics, RenderingType.Gdi, null, true);
			if (!this.GaugeContainer.Enabled)
			{
				ImageAttributes imageAttributes = new ImageAttributes();
				ColorMatrix colorMatrix = new ColorMatrix();
				colorMatrix.Matrix00 = 0.3f;
				colorMatrix.Matrix01 = 0.3f;
				colorMatrix.Matrix02 = 0.3f;
				colorMatrix.Matrix10 = 0.3f;
				colorMatrix.Matrix11 = 0.3f;
				colorMatrix.Matrix12 = 0.3f;
				colorMatrix.Matrix20 = 0.3f;
				colorMatrix.Matrix21 = 0.3f;
				colorMatrix.Matrix22 = 0.3f;
				imageAttributes.SetColorMatrix(colorMatrix);
				Region clip = this.bmpGauge.Graphics.Clip;
				this.bmpGauge.Graphics.Clip = this.GetClipRegion();
				this.bmpGauge.Graphics.DrawImage(this.bmpGauge.Bitmap, new Rectangle(0, 0, this.GetWidth(), this.GetHeight()), 0, 0, this.GetWidth(), this.GetHeight(), GraphicsUnit.Pixel, imageAttributes);
				this.bmpGauge.Graphics.Clip.Dispose();
				this.bmpGauge.Graphics.Clip = clip;
			}
			g.DrawImageUnscaled(this.bmpGauge.Bitmap, 0, 0);
			this.dirtyFlag = false;
		}

		internal void SaveTo(Stream stream, GaugeImageFormat imageFormat, int compression, float dpiX, float dpiY)
		{
			if (this.isInitializing)
			{
				this.EndInit();
			}
			bool flag = this.dirtyFlag;
			this.dirtyFlag = true;
			this.Notify(MessageType.PrepareSnapShot, this, null);
			if (imageFormat == GaugeImageFormat.Emf)
			{
				this.SaveIntoMetafile(stream);
				this.dirtyFlag = flag;
			}
			else
			{
				BufferBitmap bufferBitmap = this.InitBitmap(null, dpiX, dpiY);
				RenderingType renderingType = RenderingType.Gdi;
				ImageFormat imageFormat2 = null;
				ImageCodecInfo imageCodecInfo = null;
				EncoderParameter encoderParameter = null;
				EncoderParameters encoderParameters = null;
				string text = ((Enum)(object)imageFormat).ToString((IFormatProvider)CultureInfo.InvariantCulture);
				imageFormat2 = (ImageFormat)new ImageFormatConverter().ConvertFromString(text);
				Color color = (!(this.BackColor != Color.Empty)) ? Color.White : this.BackColor;
				Pen pen = new Pen(color);
				SmoothingMode smoothingMode = bufferBitmap.Graphics.SmoothingMode;
				bufferBitmap.Graphics.SmoothingMode = SmoothingMode.None;
				bufferBitmap.Graphics.DrawRectangle(pen, 0, 0, bufferBitmap.Size.Width, bufferBitmap.Size.Height);
				bufferBitmap.Graphics.SmoothingMode = smoothingMode;
				pen.Dispose();
				this.Paint(bufferBitmap.Graphics, renderingType, stream, false);
				if (renderingType == RenderingType.Gdi && compression >= 0 && compression <= 100 && "jpeg,png,bmp".IndexOf(text, StringComparison.OrdinalIgnoreCase) != -1)
				{
					imageCodecInfo = GaugeCore.GetEncoderInfo("image/" + text);
					encoderParameters = new EncoderParameters(1);
					encoderParameter = new EncoderParameter(Encoder.Quality, 100L - (long)compression);
					encoderParameters.Param[0] = encoderParameter;
				}
				if (!this.GaugeContainer.Enabled)
				{
					ImageAttributes imageAttributes = new ImageAttributes();
					ColorMatrix colorMatrix = new ColorMatrix();
					colorMatrix.Matrix00 = 0.3f;
					colorMatrix.Matrix01 = 0.3f;
					colorMatrix.Matrix02 = 0.3f;
					colorMatrix.Matrix10 = 0.3f;
					colorMatrix.Matrix11 = 0.3f;
					colorMatrix.Matrix12 = 0.3f;
					colorMatrix.Matrix20 = 0.3f;
					colorMatrix.Matrix21 = 0.3f;
					colorMatrix.Matrix22 = 0.3f;
					imageAttributes.SetColorMatrix(colorMatrix);
					Region clip = bufferBitmap.Graphics.Clip;
					bufferBitmap.Graphics.Clip = this.GetClipRegion();
					bufferBitmap.Graphics.DrawImage(bufferBitmap.Bitmap, new Rectangle(0, 0, this.GetWidth(), this.GetHeight()), 0, 0, this.GetWidth(), this.GetHeight(), GraphicsUnit.Pixel, imageAttributes);
					bufferBitmap.Graphics.Clip.Dispose();
					bufferBitmap.Graphics.Clip = clip;
				}
				if (renderingType == RenderingType.Gdi && imageFormat != GaugeImageFormat.Emf)
				{
					if (imageCodecInfo == null)
					{
						bufferBitmap.Bitmap.Save(stream, imageFormat2);
					}
					else
					{
						if (this.TransparentColor != Color.Empty)
						{
							bufferBitmap.Bitmap.MakeTransparent(this.TransparentColor);
						}
						bufferBitmap.Bitmap.Save(stream, imageCodecInfo, encoderParameters);
					}
				}
				this.dirtyFlag = flag;
			}
		}

		internal void SaveTo(Stream stream, GaugeImageFormat imageFormat, float dpiX, float dpiY)
		{
			this.SaveTo(stream, imageFormat, -1, dpiX, dpiY);
		}

		internal void SaveTo(string fileName, GaugeImageFormat imageFormat, float dpiX, float dpiY)
		{
			this.SaveTo(fileName, imageFormat, -1, dpiX, dpiY);
		}

		internal void SaveTo(string fileName, GaugeImageFormat imageFormat, int compression, float dpiX, float dpiY)
		{
			using (Stream stream = new FileStream(fileName, FileMode.Create))
			{
				this.SaveTo(stream, imageFormat, compression, dpiX, dpiY);
			}
		}

		private static ImageCodecInfo GetEncoderInfo(string mimeType)
		{
			ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
			for (int i = 0; i < imageEncoders.Length; i++)
			{
				if (imageEncoders[i].MimeType.Equals(mimeType, StringComparison.OrdinalIgnoreCase))
				{
					return imageEncoders[i];
				}
			}
			return null;
		}

		public void SaveIntoMetafile(Stream imageStream)
		{
			Bitmap bitmap = new Bitmap(this.GetWidth(), this.GetHeight());
			Graphics graphics = Graphics.FromImage(bitmap);
			IntPtr hdc = graphics.GetHdc();
			Metafile metafile = new Metafile(imageStream, hdc, new Rectangle(0, 0, this.GetWidth(), this.GetHeight()), MetafileFrameUnit.Pixel, EmfType.EmfPlusOnly);
			Graphics graphics2 = Graphics.FromImage(metafile);
			graphics2.SmoothingMode = this.GetSmootingMode();
			graphics2.TextRenderingHint = this.GetTextRenderingHint();
			this.SavingToMetafile = true;
			this.Paint(graphics2, RenderingType.Gdi, imageStream, false);
			this.SavingToMetafile = false;
			byte[] data = new byte[12]
			{
				68,
				117,
				110,
				100,
				97,
				115,
				32,
				67,
				104,
				97,
				114,
				116
			};
			graphics2.AddMetafileComment(data);
			graphics2.Dispose();
			metafile.Dispose();
			graphics.ReleaseHdc(hdc);
			graphics.Dispose();
			bitmap.Dispose();
		}

		private Region GetClipRegion()
		{
			Region region = null;
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				foreach (HotRegion item in this.HotRegionList.List)
				{
					if (item.SelectedObject is IRenderable)
					{
						GraphicsPath[] paths = item.Paths;
						foreach (GraphicsPath addingPath in paths)
						{
							graphicsPath.AddPath(addingPath, false);
						}
					}
				}
				return new Region(graphicsPath);
			}
		}

		internal bool IsDesignMode()
		{
			return this.GaugeContainer.IsDesignMode();
		}

		internal void DrawException(Graphics graphics, Exception e)
		{
			graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, this.GetWidth(), this.GetHeight());
			graphics.DrawRectangle(new Pen(Color.Red, 4f), 0, 0, this.GetWidth(), this.GetHeight());
			string text = SR.GaugeDesignException + e.Message;
			string text2 = "";
			if (e.TargetSite != null)
			{
				text2 += SR.GaugeDesignSite(e.TargetSite.ToString());
			}
			if (e.StackTrace != string.Empty)
			{
				text2 = text2 + SR.GaugeDesignStack + e.StackTrace;
			}
			RectangleF layoutRectangle = new RectangleF(3f, 3f, (float)(this.GetWidth() - 6), (float)(this.GetHeight() - 6));
			StringFormat format = new StringFormat();
			SizeF sizeF = graphics.MeasureString(text, new Font("Microsoft Sans Serif", 10f, FontStyle.Bold), (int)layoutRectangle.Width, format);
			graphics.DrawString(text, new Font("Microsoft Sans Serif", 10f, FontStyle.Bold), new SolidBrush(Color.Black), layoutRectangle, format);
			layoutRectangle.Y += (float)(sizeF.Height + 5.0);
			graphics.DrawString(text2, new Font("Microsoft Sans Serif", 8f), new SolidBrush(Color.Black), layoutRectangle, format);
		}

		internal HitTestResult[] HitTest(int x, int y, Type[] objectTypes, bool returnMultipleElements)
		{
			if (objectTypes == null)
			{
				objectTypes = new Type[0];
			}
			ArrayList arrayList = new ArrayList();
			HotRegion[] array = this.HotRegionList.CheckHotRegions(x, y, objectTypes);
			PointF hitTestPoint = new PointF((float)x, (float)y);
			if (array == null)
			{
				arrayList.Add(new HitTestResult(null, hitTestPoint));
			}
			else if (!returnMultipleElements)
			{
				arrayList.Add(new HitTestResult(array[0], hitTestPoint));
			}
			else
			{
				object obj = null;
				HotRegion[] array2 = array;
				foreach (HotRegion hotRegion in array2)
				{
					if (obj != hotRegion.SelectedObject)
					{
						arrayList.Add(new HitTestResult(hotRegion, hitTestPoint));
						obj = hotRegion.SelectedObject;
					}
				}
			}
			return (HitTestResult[])arrayList.ToArray(typeof(HitTestResult));
		}

		internal HotRegion GetHotRegion(NamedElement element)
		{
			if (element == null)
			{
				element = this;
			}
			if (!(element is IRenderable) && element != this)
			{
				throw new ArgumentException(Utils.SRGetStr("ExceptionHotRegionSupport", element.Name));
			}
			int num = this.HotRegionList.LocateObject(element);
			if (num != -1)
			{
				return (HotRegion)this.HotRegionList.List[num];
			}
			throw new ArgumentException(Utils.SRGetStr("ExceptionHotRegionInitialize", element.Name));
		}

		internal void PopulateImageMaps()
		{
			for (int num = this.HotRegionList.List.Count - 1; num >= 0; num--)
			{
				HotRegion hotRegion = (HotRegion)this.HotRegionList.List[num];
				if (hotRegion.SelectedObject is IImageMapProvider)
				{
					IImageMapProvider imageMapProvider = (IImageMapProvider)hotRegion.SelectedObject;
					string a = imageMapProvider.GetToolTip();
					string href = imageMapProvider.GetHref();
					string mapAreaAttributes = imageMapProvider.GetMapAreaAttributes();
					if (a != "" || href != "" || mapAreaAttributes != "")
					{
						object tag = imageMapProvider.Tag;
						for (int i = 0; i < hotRegion.Paths.Length; i++)
						{
							if (hotRegion.Paths[i] != null)
							{
								this.mapAreas.Add(a, href, mapAreaAttributes, hotRegion.Paths[i], tag);
							}
						}
					}
				}
			}
		}

		internal void WriteMapTag(TextWriter output, string mapName)
		{
			output.Write("\r\n<MAP NAME=\"" + mapName + "\">");
			foreach (MapArea mapArea in this.mapAreas)
			{
				output.Write(mapArea.GetTag());
			}
			output.Write("\r\n</MAP>\r\n");
		}

		internal void PerformDataBinding(IEnumerable data)
		{
			foreach (InputValue value in this.Values)
			{
				value.PerformDataBinding(data);
				this.boundToDataSource = true;
			}
		}

		internal void AutoDataBind(bool forceBinding)
		{
			if ((this.boundToDataSource || this.IsDesignMode()) && !forceBinding)
			{
				return;
			}
			this.boundToDataSource = true;
			foreach (InputValue value in this.Values)
			{
				value.AutoDataBind();
			}
		}

		internal static bool IsValidDataSource(object dataSource)
		{
			if (!(dataSource is IEnumerable) && !(dataSource is DataSet) && !(dataSource is DataView) && !(dataSource is DataTable) && !(dataSource is SqlCommand) && !(dataSource is SqlDataAdapter) && dataSource.GetType().GetInterface("IDataSource") == null)
			{
				return false;
			}
			return true;
		}

		internal string ResolveAllKeywords(string original, NamedElement element)
		{
			if (original.Length == 0)
			{
				return original;
			}
			string text = original.Replace("\\n", "\n");
			int num = 0;
			foreach (InputValue value in this.Values)
			{
				string keyword = "#INPUTVALUE" + num.ToString(CultureInfo.InvariantCulture);
				text = this.ResolveKeyword(text, keyword, value.Value);
				num++;
			}
			if (element is CircularGauge)
			{
				CircularGauge circularGauge = (CircularGauge)element;
				num = 0;
				foreach (CircularPointer pointer in circularGauge.Pointers)
				{
					string keyword2 = "#POINTERVALUE" + num.ToString(CultureInfo.InvariantCulture);
					text = this.ResolveKeyword(text, keyword2, pointer.Value);
					num++;
				}
			}
			else if (element is LinearGauge)
			{
				LinearGauge linearGauge = (LinearGauge)element;
				num = 0;
				foreach (LinearPointer pointer2 in linearGauge.Pointers)
				{
					string keyword3 = "#POINTERVALUE" + num.ToString(CultureInfo.InvariantCulture);
					text = this.ResolveKeyword(text, keyword3, pointer2.Value);
					num++;
				}
			}
			if (element is CircularPointer)
			{
				text = this.ResolveKeyword(text, "#VALUE", ((CircularPointer)element).Value);
			}
			else if (element is LinearPointer)
			{
				text = this.ResolveKeyword(text, "#VALUE", ((LinearPointer)element).Value);
			}
			else if (element is NumericIndicator)
			{
				text = this.ResolveKeyword(text, "#VALUE", ((NumericIndicator)element).Value);
			}
			else if (element is StateIndicator)
			{
				text = this.ResolveKeyword(text, "#VALUE", ((StateIndicator)element).Value);
			}
			if (element is StateIndicator)
			{
				text = text.Replace("#STATE", ((StateIndicator)element).CurrentState);
			}
			return text;
		}

		internal string ResolveKeyword(string original, string keyword, double val)
		{
			string text = original;
			int num = -1;
			while ((num = text.IndexOf(keyword, StringComparison.Ordinal)) != -1)
			{
				int num2 = num + keyword.Length;
				string format = string.Empty;
				if (text.Length > num2 && text[num2] == '{')
				{
					int num3 = text.IndexOf('}', num2);
					if (num3 == -1)
					{
						throw new InvalidOperationException(Utils.SRGetStr("ExceptionInvalidKeywordFormat", text));
					}
					format = text.Substring(num2, num3 - num2).Trim('{', '}');
					num2 = num3 + 1;
				}
				text = text.Remove(num, num2 - num);
				string empty = string.Empty;
				empty = ((this.GaugeContainer.FormatNumberHandler == null) ? val.ToString(format, CultureInfo.CurrentCulture) : this.GaugeContainer.FormatNumberHandler(this.GaugeContainer, val, format));
				text = text.Insert(num, empty);
			}
			return text;
		}

		internal void TraceWrite(string category, string message)
		{
			if (this.serviceContainer != null)
			{
				TraceManager traceManager = (TraceManager)this.serviceContainer.GetService(typeof(TraceManager));
				if (traceManager != null)
				{
					traceManager.Write(category, message);
				}
			}
		}

		internal object GetService(Type serviceType)
		{
			object result = null;
			if (this.serviceContainer != null)
			{
				result = this.serviceContainer.GetService(serviceType);
			}
			return result;
		}

		protected override void OnDispose()
		{
			if (this.inputValues != null)
			{
				this.inputValues.Dispose();
			}
			NamedCollection[] renderCollections = this.GetRenderCollections();
			foreach (NamedCollection namedCollection in renderCollections)
			{
				namedCollection.Dispose();
			}
			if (this.bmpGauge != null)
			{
				this.bmpGauge.Dispose();
			}
			if (this.bmpFaces != null)
			{
				this.bmpFaces.Dispose();
			}
			if (this.namedImages != null)
			{
				this.namedImages.Dispose();
			}
			if (this.imageLoader != null)
			{
				this.imageLoader.Dispose();
			}
		}

		internal static bool CheckLicense()
		{
			bool result = false;
			try
			{
				string str = "SOFTWARE\\Dundas Software\\Gauges\\WebControl";
				str += "VS2005";
				string str2 = "AspNetCore.Reporting.Gauge.WebForms.GaugeContainer.lic";
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(str);
				if (registryKey != null)
				{
					string text = (string)registryKey.GetValue("InstallDir");
					if (!text.EndsWith("\\", StringComparison.Ordinal))
					{
						text += "\\";
					}
					text += "Bin\\";
					if (File.Exists(text + str2))
					{
						result = true;
						return result;
					}
					return result;
				}
				return result;
			}
			catch
			{
				return result;
			}
		}

		void ISelectable.DrawSelection(GaugeGraphics g, bool designTimeSelection)
		{
			g.DrawSelection(new RectangleF(0f, 0f, (float)(this.GetWidth() - 1), (float)(this.GetHeight() - 1)), (float)(-3.0 / g.Graphics.PageScale), designTimeSelection, this.Common.GaugeCore.SelectionBorderColor, this.Common.GaugeCore.SelectionMarkerColor);
		}
	}
}
