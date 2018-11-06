using AspNetCore.Reporting.Chart.WebForms.Borders3D;
using AspNetCore.Reporting.Chart.WebForms.ChartTypes;
using AspNetCore.Reporting.Chart.WebForms.Data;
using AspNetCore.Reporting.Chart.WebForms.Design;
using AspNetCore.Reporting.Chart.WebForms.Formulas;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[LicenseProvider(typeof(LicFileLicenseProvider))]
	[DisplayName("Dundas Chart Enterprise")]
	internal class Chart : IDisposable, IChart
	{
		private float imageResolution = 96f;

		private string multiValueSeparator = "\\#\\";

		private Title noDataMessage = new Title("*****  No Data  *****");

		private bool reverseSeriesOrder;

		private bool suppressCodeExceptions = true;

		private string codeException = "";

		internal bool ShowDebugMarkings;

		private ChartTypeRegistry chartTypeRegistry;

		private BorderTypeRegistry borderTypeRegistry;

		private CustomAttributeRegistry customAttributeRegistry;

		private DataManager dataManager;

		internal ChartImage chartPicture;

		private ImageLoader imageLoader;

		internal static ITypeDescriptorContext controlCurrentContext = null;

		internal string webFormDocumentURL = "";

		internal ServiceContainer serviceContainer;

		private EventsManager eventsManager;

		private TraceManager traceManager;

		private NamedImagesCollection namedImages;

		private FormulaRegistry formulaRegistry;

		internal static string productID = "DC-WCE-42";

		private License license;

		private RenderType renderType;

		private string chartImageUrl = "ChartPic_#SEQ(300,3)";

		internal bool serializing;

		internal SerializationStatus serializationStatus;

		private ChartSerializer chartSerializer;

		private string windowsFormsControlURL = string.Empty;

		private string currentChartImageUrl = string.Empty;

		private KeywordsRegistry keywordsRegistry;

		internal static double renderingDpiX = 96.0;

		internal static double renderingDpiY = 96.0;

		private string lastUpdatedDesignTimeHtmlValue = string.Empty;

		public LocalizeTextHandler LocalizeTextHandler;

		public FormatNumberHandler FormatNumberHandler;

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

		[SRDescription("DescriptionAttributeMultiValueSeparator")]
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

		[SRDescription("DescriptionAttributeNoDataMessage")]
		public Title NoDataMessage
		{
			get
			{
				if (this.noDataMessage != null && this.noDataMessage.Text == string.Empty)
				{
					this.noDataMessage.Text = "*****  No Data  *****";
				}
				return this.noDataMessage;
			}
			set
			{
				this.noDataMessage = value;
			}
		}

		[SRDescription("DescriptionAttributeReverseSeriesOrder")]
		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[DefaultValue(false)]
		public bool ReverseSeriesOrder
		{
			get
			{
				return this.reverseSeriesOrder;
			}
			set
			{
				this.reverseSeriesOrder = value;
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeChart")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeSuppressCodeExceptions")]
		[RefreshProperties(RefreshProperties.All)]
		public bool SuppressCodeExceptions
		{
			get
			{
				return this.suppressCodeExceptions;
			}
			set
			{
				this.suppressCodeExceptions = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string CodeException
		{
			get
			{
				return this.codeException;
			}
			set
			{
				this.codeException = value;
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(96.0)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public double RenderingDpiY
		{
			get
			{
				return Chart.renderingDpiY;
			}
			set
			{
				Chart.renderingDpiY = value;
			}
		}

		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(96.0)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public double RenderingDpiX
		{
			get
			{
				return Chart.renderingDpiX;
			}
			set
			{
				Chart.renderingDpiX = value;
			}
		}

		[SRDescription("DescriptionAttributeSuppressExceptions")]
		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(false)]
		public bool SuppressExceptions
		{
			get
			{
				return this.chartPicture.SuppressExceptions;
			}
			set
			{
				this.chartPicture.SuppressExceptions = value;
			}
		}

		[SRDescription("DescriptionAttributeChart_Images")]
		[SRCategory("CategoryAttributeChart")]
		[Bindable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public NamedImagesCollection Images
		{
			get
			{
				return this.namedImages;
			}
		}

		[DefaultValue(RenderType.ImageTag)]
		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeChart_RenderType")]
		internal RenderType RenderType
		{
			get
			{
				return this.renderType;
			}
			set
			{
				this.renderType = value;
				if (this.renderType == RenderType.ImageMap && !this.MapEnabled)
				{
					this.MapEnabled = true;
				}
			}
		}

		[SRCategory("CategoryAttributeImage")]
		[DefaultValue("ChartPic_#SEQ(300,3)")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeChart_ImageUrl")]
		public string ImageUrl
		{
			get
			{
				return this.chartImageUrl;
			}
			set
			{
				int num = value.IndexOf("#SEQ", StringComparison.Ordinal);
				if (num > 0)
				{
					this.CheckImageURLSeqFormat(value);
				}
				this.chartImageUrl = value;
			}
		}

		[Category("Appearance")]
		[SRDescription("DescriptionAttribute_RightToLeft")]
		[DefaultValue(RightToLeft.No)]
		public RightToLeft RightToLeft
		{
			get
			{
				return this.chartPicture.RightToLeft;
			}
			set
			{
				this.chartPicture.RightToLeft = value;
			}
		}

		[SRDescription("DescriptionAttributeChart_Series")]
		[SRCategory("CategoryAttributeChart")]
		public SeriesCollection Series
		{
			get
			{
				return this.dataManager.Series;
			}
		}

		[Bindable(true)]
		[DefaultValue(ChartColorPalette.BrightPastel)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributePalette")]
		public ChartColorPalette Palette
		{
			get
			{
				return this.dataManager.Palette;
			}
			set
			{
				this.dataManager.Palette = value;
			}
		}

		[SerializationVisibility(SerializationVisibility.Attribute)]
		[TypeConverter(typeof(ColorArrayConverter))]
		[SRDescription("DescriptionAttributeChart_PaletteCustomColors")]
		[SRCategory("CategoryAttributeAppearance")]
		public Color[] PaletteCustomColors
		{
			get
			{
				return this.dataManager.PaletteCustomColors;
			}
			set
			{
				this.dataManager.PaletteCustomColors = value;
			}
		}

		[SRDescription("DescriptionAttributeChart_BuildNumber")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string BuildNumber
		{
			get
			{
				string text = string.Empty;
				Assembly executingAssembly = Assembly.GetExecutingAssembly();
				if (executingAssembly != null)
				{
					text = executingAssembly.FullName.ToUpper(CultureInfo.InvariantCulture);
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
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRCategory("CategoryAttributeSerializer")]
		[SRDescription("DescriptionAttributeChart_Serializer")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal ChartSerializer Serializer
		{
			get
			{
				return this.chartSerializer;
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeImage")]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(ChartImageType.Png)]
		[SRDescription("DescriptionAttributeChartImageType")]
		public ChartImageType ImageType
		{
			get
			{
				return this.chartPicture.ImageType;
			}
			set
			{
				this.chartPicture.ImageType = value;
			}
		}

		[SRDescription("DescriptionAttributeChart_Compression")]
		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[DefaultValue(0)]
		internal int Compression
		{
			get
			{
				return this.chartPicture.Compression;
			}
			set
			{
				this.chartPicture.Compression = value;
			}
		}

		[DefaultValue(true)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMapEnabled")]
		[SRCategory("CategoryAttributeMap")]
		internal bool MapEnabled
		{
			get
			{
				return this.chartPicture.MapEnabled;
			}
			set
			{
				this.chartPicture.MapEnabled = value;
			}
		}

		[SRDescription("DescriptionAttributeMapAreas")]
		[SRCategory("CategoryAttributeMap")]
		public MapAreasCollection MapAreas
		{
			get
			{
				return this.chartPicture.MapAreas;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRCategory("CategoryAttributeImage")]
		[Browsable(false)]
		[Bindable(false)]
		[DefaultValue(typeof(AntiAlias), "On")]
		[SRDescription("DescriptionAttributeAntiAlias")]
		public AntiAlias AntiAlias
		{
			get
			{
				return this.chartPicture.AntiAlias;
			}
			set
			{
				this.chartPicture.AntiAlias = value;
			}
		}

		[Bindable(true)]
		[SRDescription("DescriptionAttributeAntiAlias")]
		[SRCategory("CategoryAttributeImage")]
		[DefaultValue(typeof(AntiAliasingTypes), "All")]
		public AntiAliasingTypes AntiAliasing
		{
			get
			{
				return this.chartPicture.AntiAliasing;
			}
			set
			{
				this.chartPicture.AntiAliasing = value;
			}
		}

		[Bindable(true)]
		[SRDescription("DescriptionAttributeTextAntiAliasingQuality")]
		[DefaultValue(typeof(TextAntiAliasingQuality), "High")]
		[SRCategory("CategoryAttributeImage")]
		public TextAntiAliasingQuality TextAntiAliasingQuality
		{
			get
			{
				return this.chartPicture.TextAntiAliasingQuality;
			}
			set
			{
				this.chartPicture.TextAntiAliasingQuality = value;
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeImage")]
		[SRDescription("DescriptionAttributeChart_SoftShadows")]
		[DefaultValue(true)]
		public bool SoftShadows
		{
			get
			{
				return this.chartPicture.SoftShadows;
			}
			set
			{
				this.chartPicture.SoftShadows = value;
			}
		}

		[SRDescription("DescriptionAttributeChartAreas")]
		[SRCategory("CategoryAttributeChart")]
		[Bindable(true)]
		public ChartAreaCollection ChartAreas
		{
			get
			{
				return this.chartPicture.ChartAreas;
			}
		}

		[DefaultValue(typeof(Color), "White")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBackColor5")]
		public Color BackColor
		{
			get
			{
				return this.chartPicture.BackColor;
			}
			set
			{
				this.chartPicture.BackColor = value;
			}
		}

		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(false)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeChart_ForeColor")]
		public Color ForeColor
		{
			get
			{
				return Color.Empty;
			}
			set
			{
			}
		}

		[SRCategory("CategoryAttributeImage")]
		[SRDescription("DescriptionAttributeHeight3")]
		[Bindable(true)]
		[DefaultValue(300)]
		public int Height
		{
			get
			{
				return this.chartPicture.Height;
			}
			set
			{
				this.chartPicture.Height = value;
			}
		}

		[SRDescription("DescriptionAttributeWidth")]
		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[DefaultValue(300)]
		public int Width
		{
			get
			{
				return this.chartPicture.Width;
			}
			set
			{
				this.chartPicture.Width = value;
			}
		}

		[SRCategory("CategoryAttributeChart")]
		[NotifyParentProperty(true)]
		[DefaultValue(null)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeLegend")]
		[Bindable(false)]
		[TypeConverter(typeof(LegendConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public Legend Legend
		{
			get
			{
				if (this.serializing)
				{
					return null;
				}
				return this.chartPicture.Legend;
			}
			set
			{
				this.chartPicture.Legend = value;
			}
		}

		[SRDescription("DescriptionAttributeLegends")]
		[SRCategory("CategoryAttributeChart")]
		public LegendCollection Legends
		{
			get
			{
				return this.chartPicture.Legends;
			}
		}

		[SRDescription("DescriptionAttributeTitles")]
		[SRCategory("CategoryAttributeChart")]
		public TitleCollection Titles
		{
			get
			{
				return this.chartPicture.Titles;
			}
		}

		[SRDescription("DescriptionAttributeAnnotations3")]
		[SRCategory("CategoryAttributeChart")]
		public AnnotationCollection Annotations
		{
			get
			{
				return this.chartPicture.Annotations;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRCategory("CategoryAttributeData")]
		[SRDescription("DescriptionAttributeDataManipulator")]
		[Browsable(false)]
		public DataManipulator DataManipulator
		{
			get
			{
				return this.chartPicture.DataManipulator;
			}
		}

		[SRCategory("CategoryAttributeCharttitle")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Bindable(false)]
		[Browsable(false)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeTitle5")]
		public string Title
		{
			get
			{
				return this.chartPicture.Title;
			}
			set
			{
				this.chartPicture.Title = value;
			}
		}

		[SRDescription("DescriptionAttributeTitleFontColor")]
		[SRCategory("CategoryAttributeCharttitle")]
		[Bindable(false)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(typeof(Color), "Black")]
		public Color TitleFontColor
		{
			get
			{
				return this.chartPicture.TitleFontColor;
			}
			set
			{
				this.chartPicture.TitleFontColor = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttributeCharttitle")]
		[Bindable(false)]
		[Browsable(false)]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeTitleFont4")]
		public Font TitleFont
		{
			get
			{
				return this.chartPicture.TitleFont;
			}
			set
			{
				this.chartPicture.TitleFont = value;
			}
		}

		[SRDescription("DescriptionAttributeBackHatchStyle9")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartHatchStyle.None)]
		public ChartHatchStyle BackHatchStyle
		{
			get
			{
				return this.chartPicture.BackHatchStyle;
			}
			set
			{
				this.chartPicture.BackHatchStyle = value;
			}
		}

		[DefaultValue("")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackImage3")]
		public string BackImage
		{
			get
			{
				return this.chartPicture.BackImage;
			}
			set
			{
				this.chartPicture.BackImage = value;
			}
		}

		[SRDescription("DescriptionAttributeBackImageMode3")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartImageWrapMode.Tile)]
		[NotifyParentProperty(true)]
		public ChartImageWrapMode BackImageMode
		{
			get
			{
				return this.chartPicture.BackImageMode;
			}
			set
			{
				this.chartPicture.BackImageMode = value;
			}
		}

		[Bindable(true)]
		[SRDescription("DescriptionAttributeBackImageTransparentColor6")]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		public Color BackImageTransparentColor
		{
			get
			{
				return this.chartPicture.BackImageTransparentColor;
			}
			set
			{
				this.chartPicture.BackImageTransparentColor = value;
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeBackImageAlign")]
		[DefaultValue(ChartImageAlign.TopLeft)]
		[NotifyParentProperty(true)]
		public ChartImageAlign BackImageAlign
		{
			get
			{
				return this.chartPicture.BackImageAlign;
			}
			set
			{
				this.chartPicture.BackImageAlign = value;
			}
		}

		[SRDescription("DescriptionAttributeBackGradientType3")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(GradientType.None)]
		public GradientType BackGradientType
		{
			get
			{
				return this.chartPicture.BackGradientType;
			}
			set
			{
				this.chartPicture.BackGradientType = value;
			}
		}

		[SRDescription("DescriptionAttributeBackGradientEndColor4")]
		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		public Color BackGradientEndColor
		{
			get
			{
				return this.chartPicture.BackGradientEndColor;
			}
			set
			{
				this.chartPicture.BackGradientEndColor = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(false)]
		[Browsable(false)]
		[DefaultValue(typeof(Color), "White")]
		[SRDescription("DescriptionAttributeBorderColor")]
		public Color BorderColor
		{
			get
			{
				return this.chartPicture.BorderColor;
			}
			set
			{
				this.chartPicture.BorderColor = value;
			}
		}

		[Bindable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeChart_BorderlineWidth")]
		public int BorderWidth
		{
			get
			{
				return this.chartPicture.BorderWidth;
			}
			set
			{
				this.chartPicture.BorderWidth = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Bindable(false)]
		[Browsable(false)]
		[DefaultValue(ChartDashStyle.NotSet)]
		[SRDescription("DescriptionAttributeBorderStyle8")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ChartDashStyle BorderStyle
		{
			get
			{
				return this.chartPicture.BorderStyle;
			}
			set
			{
				this.chartPicture.BorderStyle = value;
			}
		}

		[SRDescription("DescriptionAttributeBorderColor")]
		[DefaultValue(typeof(Color), "White")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		public Color BorderlineColor
		{
			get
			{
				return this.chartPicture.BorderColor;
			}
			set
			{
				this.chartPicture.BorderColor = value;
			}
		}

		[SRDescription("DescriptionAttributeChart_BorderlineWidth")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(1)]
		public int BorderlineWidth
		{
			get
			{
				return this.chartPicture.BorderWidth;
			}
			set
			{
				this.chartPicture.BorderWidth = value;
			}
		}

		[Bindable(true)]
		[SRDescription("DescriptionAttributeBorderStyle8")]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(ChartDashStyle.NotSet)]
		public ChartDashStyle BorderlineStyle
		{
			get
			{
				return this.chartPicture.BorderStyle;
			}
			set
			{
				this.chartPicture.BorderStyle = value;
			}
		}

		[Bindable(true)]
		[SRDescription("DescriptionAttributeBorderSkinAttributes")]
		[NotifyParentProperty(true)]
		[TypeConverter(typeof(LegendConverter))]
		[DefaultValue(BorderSkinStyle.None)]
		[SRCategory("CategoryAttributeAppearance")]
		public BorderSkinAttributes BorderSkin
		{
			get
			{
				return this.chartPicture.BorderSkinAttributes;
			}
			set
			{
				this.chartPicture.BorderSkinAttributes = value;
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRCategory("CategoryAttributeMisc")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Bindable(false)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeChart_Edition")]
		public ChartEdition Edition
		{
			get
			{
				return ChartEdition.Enterprise;
			}
		}

		[SRDescription("DescriptionAttributeChartEvent_PrePaint")]
		internal event PaintEventHandler PrePaint;

		[SRDescription("DescriptionAttributeChartEvent_PostPaint")]
		internal event PaintEventHandler PostPaint;

		[SRDescription("DescriptionAttributeChartEvent_BackPaint")]
		[Browsable(false)]
		internal event PaintEventHandler BackPaint;

		[SRDescription("DescriptionAttributeChartEvent_Paint")]
		[Browsable(false)]
		internal event PaintEventHandler Paint;

		[SRDescription("DescriptionAttributeChartEvent_CustomizeMapAreas")]
		internal event CustomizeMapAreasEventHandler CustomizeMapAreas;

		[SRDescription("DescriptionAttributeChartEvent_Customize")]
		internal event CustomizeEventHandler Customize;

		[SRDescription("DescriptionAttributeChartEvent_CustomizeLegend")]
		internal event CustomizeLegendEventHandler CustomizeLegend;

		public Chart()
		{
			this.serviceContainer = new ServiceContainer();
			this.eventsManager = new EventsManager(this.serviceContainer);
			this.traceManager = new TraceManager(this.serviceContainer);
			this.chartTypeRegistry = new ChartTypeRegistry(this.serviceContainer);
			this.borderTypeRegistry = new BorderTypeRegistry(this.serviceContainer);
			this.customAttributeRegistry = new CustomAttributeRegistry(this.serviceContainer);
			this.keywordsRegistry = new KeywordsRegistry(this.serviceContainer);
			this.dataManager = new DataManager(this.serviceContainer);
			this.imageLoader = new ImageLoader(this.serviceContainer);
			this.chartPicture = new ChartImage(this.serviceContainer);
			this.chartSerializer = new ChartSerializer(this.serviceContainer);
			this.formulaRegistry = new FormulaRegistry(this.serviceContainer);
			this.serviceContainer.AddService(typeof(Chart), this);
			this.serviceContainer.AddService(this.eventsManager.GetType(), this.eventsManager);
			this.serviceContainer.AddService(this.traceManager.GetType(), this.traceManager);
			this.serviceContainer.AddService(this.chartTypeRegistry.GetType(), this.chartTypeRegistry);
			this.serviceContainer.AddService(this.borderTypeRegistry.GetType(), this.borderTypeRegistry);
			this.serviceContainer.AddService(this.customAttributeRegistry.GetType(), this.customAttributeRegistry);
			this.serviceContainer.AddService(this.dataManager.GetType(), this.dataManager);
			this.serviceContainer.AddService(this.imageLoader.GetType(), this.imageLoader);
			this.serviceContainer.AddService(this.chartPicture.GetType(), this.chartPicture);
			this.serviceContainer.AddService(this.chartSerializer.GetType(), this.chartSerializer);
			this.serviceContainer.AddService(this.formulaRegistry.GetType(), this.formulaRegistry);
			this.serviceContainer.AddService(this.keywordsRegistry.GetType(), this.keywordsRegistry);
			this.dataManager.Initialize();
			this.chartTypeRegistry.Register("Bar", typeof(BarChart));
			this.chartTypeRegistry.Register("Column", typeof(ColumnChart));
			this.chartTypeRegistry.Register("Point", typeof(PointChart));
			this.chartTypeRegistry.Register("Bubble", typeof(BubbleChart));
			this.chartTypeRegistry.Register("Line", typeof(LineChart));
			this.chartTypeRegistry.Register("Spline", typeof(SplineChart));
			this.chartTypeRegistry.Register("StepLine", typeof(StepLineChart));
			this.chartTypeRegistry.Register("Area", typeof(AreaChart));
			this.chartTypeRegistry.Register("SplineArea", typeof(SplineAreaChart));
			this.chartTypeRegistry.Register("StackedArea", typeof(StackedAreaChart));
			this.chartTypeRegistry.Register("Pie", typeof(PieChart));
			this.chartTypeRegistry.Register("Stock", typeof(StockChart));
			this.chartTypeRegistry.Register("Candlestick", typeof(CandleStickChart));
			this.chartTypeRegistry.Register("Doughnut", typeof(DoughnutChart));
			this.chartTypeRegistry.Register("StackedBar", typeof(StackedBarChart));
			this.chartTypeRegistry.Register("StackedColumn", typeof(StackedColumnChart));
			this.chartTypeRegistry.Register("100%StackedColumn", typeof(HundredPercentStackedColumnChart));
			this.chartTypeRegistry.Register("100%StackedBar", typeof(HundredPercentStackedBarChart));
			this.chartTypeRegistry.Register("100%StackedArea", typeof(HundredPercentStackedAreaChart));
			this.chartTypeRegistry.Register("Range", typeof(RangeChart));
			this.chartTypeRegistry.Register("SplineRange", typeof(SplineRangeChart));
			this.chartTypeRegistry.Register("Gantt", typeof(GanttChart));
			this.chartTypeRegistry.Register("RangeColumn", typeof(RangeColumnChart));
			this.chartTypeRegistry.Register("ErrorBar", typeof(ErrorBarChart));
			this.chartTypeRegistry.Register("BoxPlot", typeof(BoxPlotChart));
			this.chartTypeRegistry.Register("Radar", typeof(RadarChart));
			this.chartTypeRegistry.Register("Renko", typeof(RenkoChart));
			this.chartTypeRegistry.Register("ThreeLineBreak", typeof(ThreeLineBreakChart));
			this.chartTypeRegistry.Register("Kagi", typeof(KagiChart));
			this.chartTypeRegistry.Register("PointAndFigure", typeof(PointAndFigureChart));
			this.chartTypeRegistry.Register("Polar", typeof(PolarChart));
			this.chartTypeRegistry.Register("FastLine", typeof(FastLineChart));
			this.chartTypeRegistry.Register("Funnel", typeof(FunnelChart));
			this.chartTypeRegistry.Register("Pyramid", typeof(PyramidChart));
			this.chartTypeRegistry.Register("FastPoint", typeof(FastPointChart));
			this.chartTypeRegistry.Register("TreeMap", typeof(TreeMapChart));
			this.chartTypeRegistry.Register("Sunburst", typeof(SunburstChart));
			this.formulaRegistry.Register(SR.FormulaNamePriceIndicators, typeof(PriceIndicators));
			this.formulaRegistry.Register(SR.FormulaNameGeneralTechnicalIndicators, typeof(GeneralTechnicalIndicators));
			this.formulaRegistry.Register(SR.FormulaNameTechnicalVolumeIndicators, typeof(VolumeIndicators));
			this.formulaRegistry.Register(SR.FormulaNameOscillator, typeof(Oscillators));
			this.formulaRegistry.Register(SR.FormulaNameGeneralFormulas, typeof(GeneralFormulas));
			this.formulaRegistry.Register(SR.FormulaNameTimeSeriesAndForecasting, typeof(TimeSeriesAndForecasting));
			this.formulaRegistry.Register(SR.FormulaNameStatisticalAnalysis, typeof(StatisticalAnalysis));
			this.borderTypeRegistry.Register("Emboss", typeof(EmbossBorder));
			this.borderTypeRegistry.Register("Raised", typeof(RaisedBorder));
			this.borderTypeRegistry.Register("Sunken", typeof(SunkenBorder));
			this.borderTypeRegistry.Register("FrameThin1", typeof(FrameThin1Border));
			this.borderTypeRegistry.Register("FrameThin2", typeof(FrameThin2Border));
			this.borderTypeRegistry.Register("FrameThin3", typeof(FrameThin3Border));
			this.borderTypeRegistry.Register("FrameThin4", typeof(FrameThin4Border));
			this.borderTypeRegistry.Register("FrameThin5", typeof(FrameThin5Border));
			this.borderTypeRegistry.Register("FrameThin6", typeof(FrameThin6Border));
			this.borderTypeRegistry.Register("FrameTitle1", typeof(FrameTitle1Border));
			this.borderTypeRegistry.Register("FrameTitle2", typeof(FrameTitle2Border));
			this.borderTypeRegistry.Register("FrameTitle3", typeof(FrameTitle3Border));
			this.borderTypeRegistry.Register("FrameTitle4", typeof(FrameTitle4Border));
			this.borderTypeRegistry.Register("FrameTitle5", typeof(FrameTitle5Border));
			this.borderTypeRegistry.Register("FrameTitle6", typeof(FrameTitle6Border));
			this.borderTypeRegistry.Register("FrameTitle7", typeof(FrameTitle7Border));
			this.borderTypeRegistry.Register("FrameTitle8", typeof(FrameTitle8Border));
			this.namedImages = new NamedImagesCollection(this);
			this.chartPicture.Initialize(this);
		}

		~Chart()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			if (this.imageLoader != null)
			{
				this.imageLoader.Dispose();
			}
			if (this.license != null)
			{
				this.license.Dispose();
				this.license = null;
			}
			if (this.serviceContainer != null)
			{
				this.serviceContainer.Dispose();
			}
			GC.SuppressFinalize(this);
		}

		private void CheckImageURLSeqFormat(string imageURL)
		{
			int num = imageURL.IndexOf("#SEQ", StringComparison.Ordinal);
			num += 4;
			if (imageURL[num] != '(')
			{
				throw new ArgumentException(SR.ExceptionImageUrlInvalidFormat, "imageURL");
			}
			int num2 = imageURL.IndexOf(')', 1);
			if (num2 < 0)
			{
				throw new ArgumentException(SR.ExceptionImageUrlInvalidFormat, "imageURL");
			}
			string[] array = imageURL.Substring(num + 1, num2 - num - 1).Split(',');
			if (array != null && array.Length == 2)
			{
				string[] array2 = array;
				foreach (string text in array2)
				{
					string text2 = text;
					foreach (char c in text2)
					{
						if (!char.IsDigit(c))
						{
							throw new ArgumentException(SR.ExceptionImageUrlInvalidFormat, "imageURL");
						}
					}
				}
				return;
			}
			throw new ArgumentException(SR.ExceptionImageUrlInvalidFormat, "imageURL");
		}

		private string GetNewSeqImageUrl(string imageUrl)
		{
			int num = 0;
			string text = "";
			int num2 = imageUrl.IndexOf("#SEQ", StringComparison.Ordinal);
			if (num2 < 0)
			{
				throw new ArgumentException(SR.ExceptionImageUrlMissedFormatter, "imageUrl");
			}
			this.CheckImageURLSeqFormat(imageUrl);
			text = imageUrl.Substring(0, num2);
			num2 += 4;
			int num3 = imageUrl.IndexOf(')', 1);
			text += "{0:D6}";
			text += imageUrl.Substring(num3 + 1);
			string[] array = imageUrl.Substring(num2 + 1, num3 - num2 - 1).Split(',');
			int.Parse(array[0], CultureInfo.InvariantCulture);
			num = int.Parse(array[1], CultureInfo.InvariantCulture);
			int num4 = 1;
			text = string.Format(CultureInfo.InvariantCulture, text, num4);
			if (num > 0)
			{
				this.CheckChartFileTime(text, num);
			}
			return text;
		}

		private void CheckChartFileTime(string fileName, int imageTimeToLive)
		{
		}

		public void Select(int x, int y, out string series, out int point)
		{
			((ChartPicture)this.chartPicture).Select(x, y, out series, out point);
		}

		public HitTestResult HitTest(int x, int y)
		{
			string seriesName = default(string);
			int pointIndex = default(int);
			ChartElementType type = default(ChartElementType);
			object obj = default(object);
			((ChartPicture)this.chartPicture).Select(x, y, ChartElementType.Nothing, false, out seriesName, out pointIndex, out type, out obj);
			return this.GetHitTestResult(seriesName, pointIndex, type, obj);
		}

		public HitTestResult HitTest(int x, int y, bool ignoreTransparent)
		{
			string seriesName = default(string);
			int pointIndex = default(int);
			ChartElementType type = default(ChartElementType);
			object obj = default(object);
			((ChartPicture)this.chartPicture).Select(x, y, ChartElementType.Nothing, ignoreTransparent, out seriesName, out pointIndex, out type, out obj);
			return this.GetHitTestResult(seriesName, pointIndex, type, obj);
		}

		public HitTestResult HitTest(int x, int y, ChartElementType requestedElement)
		{
			string seriesName = default(string);
			int pointIndex = default(int);
			ChartElementType type = default(ChartElementType);
			object obj = default(object);
			((ChartPicture)this.chartPicture).Select(x, y, requestedElement, false, out seriesName, out pointIndex, out type, out obj);
			return this.GetHitTestResult(seriesName, pointIndex, type, obj);
		}

		internal HitTestResult GetHitTestResult(string seriesName, int pointIndex, ChartElementType type, object obj)
		{
			HitTestResult hitTestResult = new HitTestResult();
			if (seriesName.Length > 0)
			{
				hitTestResult.Series = this.Series[seriesName];
			}
			hitTestResult.Object = obj;
			hitTestResult.PointIndex = pointIndex;
			hitTestResult.ChartElementType = type;
			switch (type)
			{
			case ChartElementType.Axis:
			{
				Axis axis2 = hitTestResult.Axis = (Axis)obj;
				if (axis2 != null)
				{
					hitTestResult.ChartArea = axis2.chartArea;
				}
				break;
			}
			case ChartElementType.DataPoint:
			{
				DataPoint dataPoint = this.Series[seriesName].Points[pointIndex];
				hitTestResult.Axis = null;
				hitTestResult.ChartArea = this.ChartAreas[dataPoint.series.ChartArea];
				break;
			}
			case ChartElementType.Gridlines:
			{
				Grid grid = (Grid)obj;
				hitTestResult.Axis = grid.axis;
				if (grid.axis != null)
				{
					hitTestResult.ChartArea = grid.axis.chartArea;
				}
				break;
			}
			case ChartElementType.LegendArea:
				hitTestResult.Axis = null;
				hitTestResult.ChartArea = null;
				break;
			case ChartElementType.LegendItem:
				hitTestResult.Axis = null;
				hitTestResult.ChartArea = null;
				break;
			case ChartElementType.PlottingArea:
			{
				ChartArea chartArea = (ChartArea)obj;
				hitTestResult.Axis = null;
				hitTestResult.ChartArea = chartArea;
				break;
			}
			case ChartElementType.StripLines:
			{
				StripLine stripLine = (StripLine)obj;
				hitTestResult.Axis = stripLine.axis;
				if (stripLine.axis != null)
				{
					hitTestResult.ChartArea = stripLine.axis.chartArea;
				}
				break;
			}
			case ChartElementType.TickMarks:
			{
				TickMark tickMark = (TickMark)obj;
				hitTestResult.Axis = tickMark.axis;
				if (tickMark.axis != null)
				{
					hitTestResult.ChartArea = tickMark.axis.chartArea;
				}
				break;
			}
			case ChartElementType.Title:
				hitTestResult.Axis = null;
				hitTestResult.ChartArea = null;
				break;
			case ChartElementType.SBLargeDecrement:
			{
				AxisScrollBar axisScrollBar = (AxisScrollBar)obj;
				hitTestResult.Axis = axisScrollBar.axis;
				if (axisScrollBar.axis != null)
				{
					hitTestResult.ChartArea = axisScrollBar.axis.chartArea;
				}
				break;
			}
			case ChartElementType.SBLargeIncrement:
			{
				AxisScrollBar axisScrollBar = (AxisScrollBar)obj;
				hitTestResult.Axis = axisScrollBar.axis;
				if (axisScrollBar.axis != null)
				{
					hitTestResult.ChartArea = axisScrollBar.axis.chartArea;
				}
				break;
			}
			case ChartElementType.SBSmallDecrement:
			{
				AxisScrollBar axisScrollBar = (AxisScrollBar)obj;
				hitTestResult.Axis = axisScrollBar.axis;
				if (axisScrollBar.axis != null)
				{
					hitTestResult.ChartArea = axisScrollBar.axis.chartArea;
				}
				break;
			}
			case ChartElementType.SBSmallIncrement:
			{
				AxisScrollBar axisScrollBar = (AxisScrollBar)obj;
				hitTestResult.Axis = axisScrollBar.axis;
				if (axisScrollBar.axis != null)
				{
					hitTestResult.ChartArea = axisScrollBar.axis.chartArea;
				}
				break;
			}
			case ChartElementType.SBThumbTracker:
			{
				AxisScrollBar axisScrollBar = (AxisScrollBar)obj;
				hitTestResult.Axis = axisScrollBar.axis;
				if (axisScrollBar.axis != null)
				{
					hitTestResult.ChartArea = axisScrollBar.axis.chartArea;
				}
				break;
			}
			}
			return hitTestResult;
		}

		public void Save(string imageFileName, ChartImageFormat format)
		{
			FileStream fileStream = new FileStream(imageFileName, FileMode.Create);
			try
			{
				this.Save(fileStream, format);
			}
			finally
			{
				fileStream.Close();
			}
		}

		public void Save(Stream imageStream, ChartImageFormat format)
		{
			this.chartPicture.isSavingAsImage = true;
			this.chartPicture.isPrinting = true;
			try
			{
				if (format == ChartImageFormat.Emf || format == ChartImageFormat.EmfDual || format == ChartImageFormat.EmfPlus)
				{
					EmfType emfType = EmfType.EmfOnly;
					switch (format)
					{
					case ChartImageFormat.EmfDual:
						emfType = EmfType.EmfPlusDual;
						break;
					case ChartImageFormat.EmfPlus:
						emfType = EmfType.EmfPlusOnly;
						break;
					}
					this.chartPicture.SaveIntoMetafile(imageStream, emfType);
				}
				else
				{
					Image image = this.chartPicture.GetImage(this.ImageResolution);
					ImageFormat format2 = ImageFormat.Png;
					switch (format)
					{
					case ChartImageFormat.Bmp:
						format2 = ImageFormat.Bmp;
						break;
					case ChartImageFormat.Jpeg:
						format2 = ImageFormat.Jpeg;
						break;
					case ChartImageFormat.Png:
						format2 = ImageFormat.Png;
						break;
					case ChartImageFormat.Emf:
						format2 = ImageFormat.Emf;
						break;
					}
					image.Save(imageStream, format2);
					image.Dispose();
				}
			}
			finally
			{
				this.chartPicture.isSavingAsImage = false;
				this.chartPicture.isPrinting = false;
			}
		}

		public void Save(Stream imageStream)
		{
			this.chartPicture.isSavingAsImage = true;
			this.chartPicture.isPrinting = true;
			try
			{
				if (this.ImageType == ChartImageType.Emf)
				{
					this.chartPicture.SaveIntoMetafile(imageStream, EmfType.EmfOnly);
				}
				else
				{
					Image image = this.chartPicture.GetImage(this.ImageResolution);
					ImageCodecInfo imageCodecInfo = null;
					EncoderParameter encoderParameter = null;
					EncoderParameters encoderParameters = new EncoderParameters(1);
					if (this.ImageType == ChartImageType.Bmp)
					{
						imageCodecInfo = Chart.GetEncoderInfo("image/bmp");
					}
					else if (this.ImageType == ChartImageType.Jpeg)
					{
						imageCodecInfo = Chart.GetEncoderInfo("image/jpeg");
					}
					else if (this.ImageType == ChartImageType.Png)
					{
						imageCodecInfo = Chart.GetEncoderInfo("image/png");
					}
					encoderParameter = new EncoderParameter(Encoder.Quality, 100L - (long)this.Compression);
					encoderParameters.Param[0] = encoderParameter;
					if (imageCodecInfo == null)
					{
						ImageFormat format = (ImageFormat)new ImageFormatConverter().ConvertFromString(this.ImageType.ToString());
						image.Save(imageStream, format);
					}
					else
					{
						image.Save(imageStream, imageCodecInfo, encoderParameters);
					}
					image.Dispose();
				}
			}
			finally
			{
				this.chartPicture.isSavingAsImage = false;
				this.chartPicture.isPrinting = false;
			}
		}

		private static ImageCodecInfo GetEncoderInfo(string mimeType)
		{
			ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
			for (int i = 0; i < imageEncoders.Length; i++)
			{
				if (imageEncoders[i].MimeType == mimeType)
				{
					return imageEncoders[i];
				}
			}
			return null;
		}

		public void RaisePostBackEvent(string eventArgument)
		{
		}

		[SRDescription("DescriptionAttributeChart_OnBackPaint")]
		protected virtual void OnBackPaint(object caller, ChartPaintEventArgs e)
		{
			if (this.BackPaint != null)
			{
				this.BackPaint(caller, e);
			}
			if (this.PrePaint != null)
			{
				this.PrePaint(caller, e);
			}
		}

		[SRDescription("DescriptionAttributeChart_OnPaint")]
		protected virtual void OnPaint(object caller, ChartPaintEventArgs e)
		{
			if (this.Paint != null)
			{
				this.Paint(caller, e);
			}
			if (this.PostPaint != null)
			{
				this.PostPaint(caller, e);
			}
		}

		[SRDescription("DescriptionAttributeChart_OnCustomizeMapAreas")]
		protected virtual void OnCustomizeMapAreas(MapAreasCollection areaItems)
		{
			if (this.CustomizeMapAreas != null)
			{
				this.CustomizeMapAreas(this, new CustomizeMapAreasEventArgs(areaItems));
			}
		}

		[SRDescription("DescriptionAttributeChart_OnCustomize")]
		protected virtual void OnCustomize()
		{
			if (this.Customize != null)
			{
				this.Customize(this, EventArgs.Empty);
			}
		}

		[SRDescription("DescriptionAttributeChart_OnCustomizeLegend")]
		protected virtual void OnCustomizeLegend(LegendItemsCollection legendItems, string legendName)
		{
			if (this.CustomizeLegend != null)
			{
				this.CustomizeLegend(this, new CustomizeLegendEventArgs(legendItems, legendName));
			}
		}

		internal void CallBackPaint(object caller, ChartPaintEventArgs e)
		{
			this.OnBackPaint(caller, e);
		}

		internal void CallPaint(object caller, ChartPaintEventArgs e)
		{
			this.OnPaint(caller, e);
		}

		internal void CallCustomizeMapAreas(MapAreasCollection areaItems)
		{
			this.OnCustomizeMapAreas(areaItems);
		}

		internal void CallCustomize()
		{
			this.OnCustomize();
		}

		internal void CallCustomizeLegend(LegendItemsCollection legendItems, string legendName)
		{
			this.OnCustomizeLegend(legendItems, legendName);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetPaletteCustomColors()
		{
			this.PaletteCustomColors = new Color[0];
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerializePaletteCustomColors()
		{
			if (this.PaletteCustomColors != null && this.PaletteCustomColors.Length != 0)
			{
				return true;
			}
			return false;
		}

		public void SaveXml(string name)
		{
			try
			{
				this.Serializer.Save(name);
			}
			catch
			{
			}
		}

		public void ApplyPaletteColors()
		{
			this.dataManager.ApplyPaletteColors();
			foreach (Series item in this.Series)
			{
				bool flag = false;
				if (item.Palette != 0)
				{
					flag = true;
				}
				else
				{
					IChartType chartType = this.chartTypeRegistry.GetChartType(item.ChartTypeName);
					flag = chartType.ApplyPaletteColorsToPoints;
				}
				if (flag)
				{
					item.ApplyPaletteColors();
				}
			}
		}

		internal bool IsDesignMode()
		{
			return false;
		}

		public void ResetAutoValues()
		{
			foreach (Series item in this.Series)
			{
				item.ResetAutoValues();
			}
			foreach (ChartArea chartArea in this.ChartAreas)
			{
				chartArea.ResetAutoValues();
			}
		}

		public void AlignDataPointsByAxisLabel()
		{
			this.chartPicture.AlignDataPointsByAxisLabel(false, PointsSortOrder.Ascending);
		}

		public void AlignDataPointsByAxisLabel(string series)
		{
			ArrayList arrayList = new ArrayList();
			string[] array = series.Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				arrayList.Add(this.Series[text.Trim()]);
			}
			this.chartPicture.AlignDataPointsByAxisLabel(arrayList, false, PointsSortOrder.Ascending);
		}

		public void AlignDataPointsByAxisLabel(string series, PointsSortOrder sortingOrder)
		{
			ArrayList arrayList = new ArrayList();
			string[] array = series.Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				arrayList.Add(this.Series[text.Trim()]);
			}
			this.chartPicture.AlignDataPointsByAxisLabel(arrayList, true, sortingOrder);
		}

		public void AlignDataPointsByAxisLabel(PointsSortOrder sortingOrder)
		{
			this.chartPicture.AlignDataPointsByAxisLabel(true, sortingOrder);
		}

		public object GetService(Type serviceType)
		{
			object result = null;
			if (this.serviceContainer != null)
			{
				result = this.serviceContainer.GetService(serviceType);
			}
			return result;
		}
	}
}
