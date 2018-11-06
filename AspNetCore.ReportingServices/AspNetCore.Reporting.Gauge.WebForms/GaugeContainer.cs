using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[SRDescription("DescriptionAttributeGaugeContainer_GaugeContainer")]
	[DisplayName("Dundas Gauge")]
	internal class GaugeContainer : IDisposable
	{
		private const string smartClientDll = "DundasWinGauge.dll";

		private const string jsFilename = "DundasGauge1.js";

		internal GaugeCore gauge;

		internal string webFormDocumentURL = "";

		internal string applicationDocumentURL = "";

		internal static ITypeDescriptorContext controlCurrentContext = null;

		private string cachedImageUrl = string.Empty;

		internal static string productID = "DG-WC";

		private bool pollServer;

		private Color backColor = Color.White;

		private int width = 320;

		private int height = 240;

		private bool enabled = true;

		public FormatNumberHandler FormatNumberHandler;

		[SRDescription("DescriptionAttributeGaugeContainer_Values")]
		[Category("Data")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public InputValueCollection Values
		{
			get
			{
				return this.gauge.Values;
			}
		}

		[Category("Gauge Container")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeGaugeContainer_CircularGauges")]
		public CircularGaugeCollection CircularGauges
		{
			get
			{
				return this.gauge.CircularGauges;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Category("Gauge Container")]
		[SRDescription("DescriptionAttributeGaugeContainer_LinearGauges")]
		public LinearGaugeCollection LinearGauges
		{
			get
			{
				return this.gauge.LinearGauges;
			}
		}

		[SRDescription("DescriptionAttributeGaugeContainer_NumericIndicators")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Category("Gauge Container")]
		public NumericIndicatorCollection NumericIndicators
		{
			get
			{
				return this.gauge.NumericIndicators;
			}
		}

		[Category("Gauge Container")]
		[SRDescription("DescriptionAttributeGaugeContainer_StateIndicators")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public StateIndicatorCollection StateIndicators
		{
			get
			{
				return this.gauge.StateIndicators;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeGaugeContainer_Images")]
		[Category("Gauge Container")]
		public GaugeImageCollection Images
		{
			get
			{
				return this.gauge.Images;
			}
		}

		[Category("Gauge Container")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeGaugeContainer_Labels")]
		public GaugeLabelCollection Labels
		{
			get
			{
				return this.gauge.Labels;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeGaugeContainer_NamedImages")]
		[Browsable(false)]
		[Category("Gauge Container")]
		public NamedImageCollection NamedImages
		{
			get
			{
				return this.gauge.NamedImages;
			}
		}

		[Category("Image")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeGaugeContainer_MapAreas")]
		public MapAreaCollection MapAreas
		{
			get
			{
				return this.gauge.MapAreas;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public string LicenseData
		{
			get
			{
				return this.gauge.licenseData;
			}
			set
			{
				this.gauge.licenseData = value;
			}
		}

		[Description("Separator to be used with the multiple value parameters.")]
		public string MultiValueSeparator
		{
			get
			{
				return this.gauge.MultiValueSeparator;
			}
			set
			{
				this.gauge.MultiValueSeparator = value;
			}
		}

		public float ImageResolution
		{
			get
			{
				return this.gauge.ImageResolution;
			}
			set
			{
				this.gauge.ImageResolution = value;
			}
		}

		[TypeConverter(typeof(DoubleNanValueConverter))]
		[SRDescription("DescriptionAttributeGaugeContainer_RefreshRate")]
		[Category("Gauge Behavior")]
		[DefaultValue(30.0)]
		public double RefreshRate
		{
			get
			{
				return this.gauge.RefreshRate;
			}
			set
			{
				if (!(value < 0.0) && !(value > 100.0))
				{
					this.gauge.RefreshRate = value;
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
			}
		}

		[Category("Image")]
		[SRDescription("DescriptionAttributeGaugeContainer_MapEnabled")]
		[DefaultValue(true)]
		public bool MapEnabled
		{
			get
			{
				return this.gauge.MapEnabled;
			}
			set
			{
				this.gauge.MapEnabled = value;
			}
		}

		[Category("Gauge Behavior")]
		[SRDescription("DescriptionAttributeGaugeContainer_AutoLayout")]
		[DefaultValue(true)]
		public bool AutoLayout
		{
			get
			{
				return this.gauge.AutoLayout;
			}
			set
			{
				this.gauge.AutoLayout = value;
			}
		}

		[Category("Appearance")]
		[SRDescription("DescriptionAttributeGaugeContainer_ShadowIntensity")]
		[ValidateBound(0.0, 100.0)]
		[DefaultValue(25f)]
		public float ShadowIntensity
		{
			get
			{
				return this.gauge.ShadowIntensity;
			}
			set
			{
				if (!(value > 100.0) && !(value < 0.0))
				{
					this.gauge.ShadowIntensity = value;
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 100));
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Category("Appearance")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeGaugeContainer_BackFrame")]
		public BackFrame BackFrame
		{
			get
			{
				return this.gauge.BackFrame;
			}
			set
			{
				this.gauge.BackFrame = value;
			}
		}

		[Category("Appearance")]
		[SRDescription("DescriptionAttributeGaugeContainer_TopImage")]
		[DefaultValue("")]
		public string TopImage
		{
			get
			{
				return this.gauge.TopImage;
			}
			set
			{
				this.gauge.TopImage = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeTopImageTransColor")]
		public Color TopImageTransColor
		{
			get
			{
				return this.gauge.TopImageTransColor;
			}
			set
			{
				this.gauge.TopImageTransColor = value;
			}
		}

		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeTopImageHueColor")]
		[Category("Appearance")]
		public Color TopImageHueColor
		{
			get
			{
				return this.gauge.TopImageHueColor;
			}
			set
			{
				this.gauge.TopImageHueColor = value;
			}
		}

		[Category("Appearance")]
		[SRDescription("DescriptionAttributeGaugeContainer_AntiAliasing")]
		[DefaultValue(typeof(AntiAliasing), "All")]
		public AntiAliasing AntiAliasing
		{
			get
			{
				return this.gauge.AntiAliasing;
			}
			set
			{
				this.gauge.AntiAliasing = value;
			}
		}

		[DefaultValue(TextAntiAliasingQuality.High)]
		[SRDescription("DescriptionAttributeGaugeContainer_TextAntiAliasingQuality")]
		[Category("Appearance")]
		public TextAntiAliasingQuality TextAntiAliasingQuality
		{
			get
			{
				return this.gauge.TextAntiAliasingQuality;
			}
			set
			{
				this.gauge.TextAntiAliasingQuality = value;
			}
		}

		[DefaultValue(RightToLeft.No)]
		[Category("Appearance")]
		[SRDescription("DescriptionAttributeGaugeContainer_RightToLeft")]
		public RightToLeft RightToLeft
		{
			get
			{
				return this.gauge.RightToLeft;
			}
			set
			{
				this.gauge.RightToLeft = value;
			}
		}

		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GaugeSerializer Serializer
		{
			get
			{
				return this.gauge.Serializer;
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CallbackManager CallbackManager
		{
			get
			{
				return this.gauge.CallbackManager;
			}
		}

		[SRDescription("DescriptionAttributeGaugeContainer_RealTimeDataInterval")]
		[Category("Data")]
		[DefaultValue(1f)]
		public float RealTimeDataInterval
		{
			get
			{
				return this.gauge.RealTimeDataInterval;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionNegativeValue"));
				}
				this.gauge.RealTimeDataInterval = value;
			}
		}

		[Category("ViewState")]
		[DefaultValue(SerializationContent.All)]
		[SRDescription("DescriptionAttributeGaugeContainer_ViewStateContent")]
		public SerializationContent ViewStateContent
		{
			get
			{
				return this.gauge.ViewStateContent;
			}
			set
			{
				this.gauge.ViewStateContent = value;
			}
		}

		[Category("Image")]
		[SRDescription("DescriptionAttributeGaugeContainer_ImageType")]
		[DefaultValue(ImageType.Png)]
		public ImageType ImageType
		{
			get
			{
				return this.gauge.ImageType;
			}
			set
			{
				this.gauge.ImageType = value;
			}
		}

		[Category("Image")]
		[SRDescription("DescriptionAttributeGaugeContainer_Compression")]
		[DefaultValue(0)]
		public int Compression
		{
			get
			{
				return this.gauge.Compression;
			}
			set
			{
				this.gauge.Compression = value;
			}
		}

		[SRDescription("DescriptionAttributeGaugeContainer_ImageUrl")]
		[Category("Image")]
		[DefaultValue("TempFiles/GaugePic_#SEQ(300,3)")]
		public string ImageUrl
		{
			get
			{
				return this.gauge.ImageUrl;
			}
			set
			{
				int num = value.IndexOf("#SEQ", StringComparison.Ordinal);
				if (num > 0)
				{
					this.CheckImageUrlSeqFormat(value);
				}
				this.gauge.ImageUrl = value;
			}
		}

		[SRDescription("DescriptionAttributeGaugeContainer_RenderType")]
		[Category("Image")]
		[DefaultValue(RenderType.ImageTag)]
		public RenderType RenderType
		{
			get
			{
				return this.gauge.RenderType;
			}
			set
			{
				this.gauge.RenderType = value;
			}
		}

		[Category("Image")]
		[SRDescription("DescriptionAttributeGaugeContainer_TransparentColor")]
		[DefaultValue(typeof(Color), "")]
		public Color TransparentColor
		{
			get
			{
				return this.gauge.TransparentColor;
			}
			set
			{
				this.gauge.TransparentColor = value;
			}
		}

		[Category("Smart Client")]
		[SRDescription("DescriptionAttributeGaugeContainer_RenderAsControl")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(AutoBool.False)]
		public AutoBool RenderAsControl
		{
			get
			{
				return this.gauge.RenderAsControl;
			}
			set
			{
				this.gauge.RenderAsControl = value;
			}
		}

		[Category("Smart Client")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeGaugeContainer_WinControlUrl")]
		public string WinControlUrl
		{
			get
			{
				return this.gauge.WinControlUrl;
			}
			set
			{
				this.gauge.WinControlUrl = value;
			}
		}

		[Category("Smart Client")]
		[DefaultValue("Loading...")]
		[SRDescription("DescriptionAttributeGaugeContainer_LoadingDataText")]
		[Localizable(true)]
		public string LoadingDataText
		{
			get
			{
				return this.gauge.LoadingDataText;
			}
			set
			{
				this.gauge.LoadingDataText = value;
			}
		}

		[SRDescription("DescriptionAttributeGaugeContainer_LoadingDataImage")]
		[Category("Smart Client")]
		[DefaultValue("")]
		public string LoadingDataImage
		{
			get
			{
				return this.gauge.LoadingDataImage;
			}
			set
			{
				this.gauge.LoadingDataImage = value;
			}
		}

		[Category("Smart Client")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeGaugeContainer_LoadingDataImage")]
		public string LoadingControlImage
		{
			get
			{
				return this.gauge.LoadingControlImage;
			}
			set
			{
				this.gauge.LoadingControlImage = value;
			}
		}

		[DefaultValue("")]
		[Category("Image")]
		[SRDescription("DescriptionAttributeGaugeContainer_TagAttributes")]
		public string TagAttributes
		{
			get
			{
				return this.gauge.TagAttributes;
			}
			set
			{
				this.gauge.TagAttributes = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue(typeof(Color), "LightBlue")]
		[SRDescription("DescriptionAttributeGaugeContainer_SelectionMarkerColor")]
		public Color SelectionMarkerColor
		{
			get
			{
				return this.gauge.SelectionMarkerColor;
			}
			set
			{
				this.gauge.SelectionMarkerColor = value;
			}
		}

		[DefaultValue(typeof(Color), "Black")]
		[Category("Appearance")]
		[SRDescription("DescriptionAttributeGaugeContainer_SelectionBorderColor")]
		public Color SelectionBorderColor
		{
			get
			{
				return this.gauge.SelectionBorderColor;
			}
			set
			{
				this.gauge.SelectionBorderColor = value;
			}
		}

		[SRCategory("CategoryAppearance")]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeGaugeContainer_BorderColor")]
		public Color BorderColor
		{
			get
			{
				return this.gauge.BorderColor;
			}
			set
			{
				this.gauge.BorderColor = value;
			}
		}

		[SRCategory("CategoryAppearance")]
		[DefaultValue(GaugeDashStyle.NotSet)]
		[SRDescription("DescriptionAttributeGaugeContainer_BorderStyle")]
		public GaugeDashStyle BorderStyle
		{
			get
			{
				return this.gauge.BorderStyle;
			}
			set
			{
				this.gauge.BorderStyle = value;
			}
		}

		[DefaultValue(1)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeContainer_BorderWidth")]
		public int BorderWidth
		{
			get
			{
				return this.gauge.BorderWidth;
			}
			set
			{
				this.gauge.BorderWidth = value;
			}
		}

		[DefaultValue(typeof(Color), "White")]
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

		[DefaultValue(320)]
		[Category("Image")]
		[SRDescription("DescriptionAttributeGaugeContainer_Width")]
		public int Width
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

		[SRDescription("DescriptionAttributeGaugeContainer_Height")]
		[Category("Image")]
		[DefaultValue(240)]
		public int Height
		{
			get
			{
				return this.height;
			}
			set
			{
				this.height = value;
				this.Invalidate();
			}
		}

		[DefaultValue(true)]
		[Category("Image")]
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
				this.Invalidate();
			}
		}

		internal ISelectable SelectedDesignTimeElement
		{
			get
			{
				return this.gauge.SelectedDesignTimeElement;
			}
			set
			{
				this.gauge.SelectedDesignTimeElement = value;
			}
		}

		[Category("Behavior")]
		[Description("Provides a tooltip to be displayed on the rendered image.")]
		[DefaultValue("")]
		public string ToolTip
		{
			get
			{
				return this.gauge.ToolTip;
			}
			set
			{
				this.gauge.ToolTip = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(false)]
		[Browsable(false)]
		public bool EnableTheming
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue("")]
		public string SkinID
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeGaugeContainerEvent_PrePaint")]
		[Category("Appearance")]
		public event GaugePaintEventHandler PrePaint;

		[SRDescription("DescriptionAttributeGaugeContainerEvent_PostPaint")]
		[Category("Appearance")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public event GaugePaintEventHandler PostPaint;

		[Browsable(false)]
		[Category("Data")]
		[SRDescription("DescriptionAttributeGaugeContainerEvent_RealTimeData")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public event RealTimeDataEventHandler RealTimeData;

		[SRDescription("DescriptionAttributeGaugeContainerEvent_Callback")]
		[Category("Action")]
		public event CallbackEventHandler Callback;

		public GaugeContainer()
		{
			this.gauge = new GaugeCore(this);
			this.gauge.BeginInit();
			this.BackColor = Color.White;
			this.Width = 320;
			this.Height = 240;
			this.BackColor = Color.White;
		}

		~GaugeContainer()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			this.gauge.Dispose();
			GC.SuppressFinalize(this);
		}

		private void SaveAsImage(string fileName, GaugeImageFormat format)
		{
			this.SaveAsImage(fileName, format, this.Compression);
		}

		private void SaveAsImage(string fileName, GaugeImageFormat format, int compression)
		{
			this.gauge.SaveTo(fileName, format, compression, this.ImageResolution, this.ImageResolution);
		}

		internal void SaveAsImage(Stream stream, GaugeImageFormat format)
		{
			this.SaveAsImage(stream, format, this.Compression);
		}

		private void SaveAsImage(Stream stream, GaugeImageFormat format, int compression)
		{
			this.gauge.SaveTo(stream, format, compression, this.ImageResolution, this.ImageResolution);
			if (this.gauge.MapEnabled)
			{
				this.gauge.PopulateImageMaps();
			}
		}

		public void SaveAsImage(Stream stream)
		{
			GaugeImageFormat imageFormat = (GaugeImageFormat)Enum.Parse(typeof(GaugeImageFormat), this.ImageType.ToString(), true);
			this.gauge.SaveTo(stream, imageFormat, this.Compression, this.ImageResolution, this.ImageResolution);
		}

		private void SaveAsImage(string fileName)
		{
			GaugeImageFormat imageFormat = (GaugeImageFormat)Enum.Parse(typeof(GaugeImageFormat), this.ImageType.ToString(), true);
			this.gauge.SaveTo(fileName, imageFormat, this.Compression, this.ImageResolution, this.ImageResolution);
		}

		private void PrintPaint(Graphics graphics, Rectangle position)
		{
			this.gauge.PrintPaint(graphics, position);
		}

		public RectangleF GetAbsoluteRectangle(NamedElement element, RectangleF relativeRect)
		{
			return this.gauge.GetHotRegion(element).GetAbsoluteRectangle(relativeRect);
		}

		public RectangleF GetRelativeRectangle(NamedElement element, RectangleF absoluteRect)
		{
			return this.gauge.GetHotRegion(element).GetRelativeRectangle(absoluteRect);
		}

		public PointF GetAbsolutePoint(NamedElement element, PointF relativePoint)
		{
			return this.gauge.GetHotRegion(element).GetAbsolutePoint(relativePoint);
		}

		public PointF GetRelativePoint(NamedElement element, PointF absolutePoint)
		{
			return this.gauge.GetHotRegion(element).GetRelativePoint(absolutePoint);
		}

		public SizeF GetAbsoluteSize(NamedElement element, SizeF relativeSize)
		{
			return this.gauge.GetHotRegion(element).GetAbsoluteSize(relativeSize);
		}

		public SizeF GetRelativeSize(NamedElement element, SizeF absoluteSize)
		{
			return this.gauge.GetHotRegion(element).GetRelativeSize(absoluteSize);
		}

		public NamedElement[] GetSelectedElements()
		{
			return this.gauge.GetSelectedElements();
		}

		private string GetImageRefreshScript(string gaugeContainerID, float refreshInterval)
		{
			string text = "\n<script language=\"javascript\">\n<!--\nvar GaugeContainer1 = new Object();\nGaugeContainer1.refreshInterval = " + ((int)(refreshInterval * 1000.0)).ToString(CultureInfo.InvariantCulture) + ";\nif (navigator.appName == \"Microsoft Internet Explorer\")\n    GaugeContainer1.bufferImage = document.images[\"GaugeContainer1\"].cloneNode();\nelse\n    GaugeContainer1.bufferImage = new Image();\nGaugeContainer1.bufferImage.src = GetNewGaugeContainer1Url();\nUpdateGaugeContainer1();\n\nfunction GetNewGaugeContainer1Url()\n{\n    var now = new Date();\n    if (location.href.indexOf(\"?\") == -1)\n        return location.href + \"?_gaugeContainerID=GaugeContainer1&_time=\" + now.getTime() + now.getMilliseconds();\n    else\n        return location.href + \"&_gaugeContainerID=GaugeContainer1&_time=\" + now.getTime() + now.getMilliseconds();\n}\n\nfunction UpdateGaugeContainer1()\n{\n    var isExplorer = navigator.appName == \"Microsoft Internet Explorer\";\n    var isBufferLoaded;\n    if (isExplorer)\n        isBufferLoaded = GaugeContainer1.bufferImage.readyState == \"complete\";\n    else\n        isBufferLoaded = GaugeContainer1.bufferImage.complete;\n    if (isBufferLoaded)\n    {\n        if (isExplorer)\n        {\n            GaugeContainer1.bufferImage.swapNode(document.images[\"GaugeContainer1\"]);\n            GaugeContainer1.bufferImage = document.images[\"GaugeContainer1\"].cloneNode();\n        }\n        else\n        {\n            document.images[\"GaugeContainer1\"].src = GaugeContainer1.bufferImage.src;\n            GaugeContainer1.bufferImage = new Image();\n        }\n\n        GaugeContainer1.bufferImage.src = GetNewGaugeContainer1Url();\n    }\n\n    setTimeout(\"UpdateGaugeContainer1()\", GaugeContainer1.refreshInterval);\n}\n-->\n</script>\n";
			return text.Replace("GaugeContainer1", gaugeContainerID);
		}

		private string GetFlashRefreshScript(string gaugeContainerID, float refreshInterval, bool isIE)
		{
			int num = (int)(refreshInterval * 1000.0);
			string text = (!isIE) ? ("\n<script language=\"javascript\">\n<!--\nvar GaugeContainer1 = new Object();\nGaugeContainer1.refreshInterval = " + num.ToString(CultureInfo.InvariantCulture) + ";\nGaugeContainer1.bufferImage = new Image();\nGaugeContainer1.bufferImage.src = GetNewGaugeContainer1Url();\nif (document.GaugeContainer1.PercentLoaded() == 100)\n    document.GaugeContainer1.LoadMovie(1, GaugeContainer1.bufferImage.src); \nif (document.GaugeContainer1Buffer.PercentLoaded() == 100)\n    document.GaugeContainer1Buffer.LoadMovie(1, GaugeContainer1.bufferImage.src);\nUpdateGaugeContainer1();\n\nfunction GetNewGaugeContainer1Url()\n{\n    var now = new Date();\n    if (location.href.indexOf(\"?\") == -1)\n        return location.href + \"?___gaugeContainerID=GaugeContainer1&_time=\" + now.getTime() + now.getMilliseconds();\n    else\n        return location.href + \"&___gaugeContainerID=GaugeContainer1&_time=\" + now.getTime() + now.getMilliseconds();\n}\n\nfunction UpdateGaugeContainer1()\n{\n    if (GaugeContainer1.bufferImage.complete)\n    {\n        if (document.GaugeContainer1.width == 0)\n        {\n            document.GaugeContainer1.width = document.GaugeContainer1Buffer.width;\n            document.GaugeContainer1.height = document.GaugeContainer1Buffer.height;\n            document.GaugeContainer1Buffer.width = 0;\n            document.GaugeContainer1Buffer.height = 0;\n            if (document.GaugeContainer1Buffer.PercentLoaded() == 100)\n                document.GaugeContainer1Buffer.LoadMovie(1, GaugeContainer1.bufferImage.src);\n        }\n        else\n        {\n            document.GaugeContainer1Buffer.width = document.GaugeContainer1.width;\n            document.GaugeContainer1Buffer.height = document.GaugeContainer1.height;\n            document.GaugeContainer1.width = 0;\n            document.GaugeContainer1.height = 0;\n            if (document.GaugeContainer1.PercentLoaded() == 100)\n                document.GaugeContainer1.LoadMovie(1, GaugeContainer1.bufferImage.src);\n        }\n    }\n    setTimeout(\"UpdateGaugeContainer1()\", GaugeContainer1.refreshInterval);\n}\n-->\n</script>\n") : ("\n<script language=\"javascript\">\n<!--\nvar GaugeContainer1 = new Object();\nInitializeGaugeContainer1();\n\nfunction InitializeGaugeContainer1()\n{\n    if (!GaugeContainer1.initialized)\n    {\n        if (document.GaugeContainer1.ReadyState == 4 && document._GaugeContainer1.ReadyState == 4)\n        {\n            GaugeContainer1.refreshInterval = " + num.ToString(CultureInfo.InvariantCulture) + ";\n            document.GaugeContainer1.LoadMovie(1, GetNewGaugeContainer1Url());\n            document._GaugeContainer1.LoadMovie(1, GetNewGaugeContainer1Url());\n            document.GaugeContainer1Buffer.Movie = GetNewGaugeContainer1Url();\n            GaugeContainer1.initialized = true;\n            UpdateGaugeContainer1();\n        }\n        else\n        {\n            setTimeout(\"InitializeGaugeContainer1()\", " + num.ToString(CultureInfo.InvariantCulture) + ");\n        }\n    }\n}\n\nfunction GetNewGaugeContainer1Url()\n{\n    var now = new Date();\n    if (location.href.indexOf(\"?\") == -1)\n        return location.href + \"?___gaugeContainerID=GaugeContainer1&_time=\" + now.getTime() + now.getMilliseconds();\n    else\n        return location.href + \"&___gaugeContainerID=GaugeContainer1&_time=\" + now.getTime() + now.getMilliseconds();\n}\n\nfunction UpdateGaugeContainer1()\n{\n    if (document.GaugeContainer1Buffer.ReadyState == 4)\n    {\n        if (document.GaugeContainer1.style.display == \"none\")\n        {\n            document.GaugeContainer1.style.width = document._GaugeContainer1.style.width;\n            document.GaugeContainer1.style.heigth = document._GaugeContainer1.style.heigth;\n            document._GaugeContainer1.style.width = 0;\n            document._GaugeContainer1.style.heigth = 0;\n            document._GaugeContainer1.style.display = \"none\";\n            document.GaugeContainer1.style.display = \"\";\n            document._GaugeContainer1.LoadMovie(1, document.GaugeContainer1Buffer.Movie);\n        }\n        else\n        {\n            document._GaugeContainer1.style.width = document.GaugeContainer1.style.width;\n            document._GaugeContainer1.style.heigth = document.GaugeContainer1.style.heigth;\n            document.GaugeContainer1.style.width = 0;\n            document.GaugeContainer1.style.heigth = 0;\n            document.GaugeContainer1.style.display = \"none\";\n            document._GaugeContainer1.style.display = \"\";\n            document.GaugeContainer1.LoadMovie(1, document.GaugeContainer1Buffer.Movie);\n        }\n        document.GaugeContainer1Buffer.Movie = GetNewGaugeContainer1Url();\n    }\n    setTimeout(\"UpdateGaugeContainer1()\", GaugeContainer1.refreshInterval);\n}\n-->\n</script>\n");
			return text.Replace("GaugeContainer1", gaugeContainerID);
		}

		internal void SaveFiles(string fullImagePath)
		{
			string directoryName = Path.GetDirectoryName(fullImagePath);
			DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
			if (!directoryInfo.Exists)
			{
				Directory.CreateDirectory(directoryName);
			}
			string text = directoryName + "\\DundasGauge1.js";
			FileInfo fileInfo = new FileInfo(text);
			if (!fileInfo.Exists)
			{
				Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(GaugeContainer).Namespace + ".DundasGauge.js");
				byte[] array = new byte[manifestResourceStream.Length];
				manifestResourceStream.Read(array, 0, array.Length);
				string value = this.ObfuscateJavaScript(Encoding.UTF8.GetString(array));
				StreamWriter streamWriter = File.CreateText(text);
				streamWriter.Write(value);
				streamWriter.Close();
			}
			this.SaveAsImage(fullImagePath);
		}

		private string ObfuscateJavaScript(string text)
		{
			string text2 = "";
			text2 = Regex.Replace(text, "\\/\\/.*\\n", "");
			text2 = Regex.Replace(text2, "[\\t\\v\\f]", "");
			text2 = Regex.Replace(text2, "\\x20?([\\+\\-\\*\\/\\=\\?\\:])\\x20?", "$1");
			text2 = Regex.Replace(text2, "(if|for|while)(\\([^\\)]+\\))[\\n\\r]+", "$1$2");
			text2 = Regex.Replace(text2, "(for\\()\\x20*(.+;)\\x20*(.+;)\\x20*(.+)\\x20*(\\))", "$1$2$3$4$5'");
			text2 = Regex.Replace(text2, ";{2,}", ";");
			text2 = text2.Replace("  ", "");
			text2 = text2.Replace("\r\n{", "{");
			text2 = text2.Replace("\r\n}", "}");
			text2 = text2.Replace("{\r\n", "{");
			text2 = text2.Replace("}\r\n", "}");
			text2 = text2.Replace("\r\n\r\n", "\r\n");
			text2 = text2.Replace(";\r\n", ";");
			return text2.Replace("\r\nvar", "var");
		}

		internal void Invalidate()
		{
		}

		internal void Refresh()
		{
		}

		private void CheckImageUrlSeqFormat(string imageUrl)
		{
			int num = imageUrl.IndexOf("#SEQ", StringComparison.Ordinal);
			num += 4;
			if (imageUrl[num] != '(')
			{
				throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidImageFormat"), "imageUrl");
			}
			int num2 = imageUrl.IndexOf(')', 1);
			if (num2 < 0)
			{
				throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidImageFormat"), "imageUrl");
			}
			string[] array = imageUrl.Substring(num + 1, num2 - num - 1).Split(',');
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
							throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidImageFormat"), "imageUrl");
						}
					}
				}
				return;
			}
			throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidImageFormat"), "imageUrl");
		}

		internal bool IsDesignMode()
		{
			return false;
		}

		public void SaveXml(string fileName)
		{
			try
			{
				this.Serializer.Save(fileName);
			}
			catch
			{
			}
		}

		internal void OnPrePaint(object sender, GaugePaintEventArgs e)
		{
			if (this.PrePaint != null)
			{
				this.PrePaint(sender, e);
			}
		}

		internal void OnPostPaint(object sender, GaugePaintEventArgs e)
		{
			if (this.PostPaint != null)
			{
				this.PostPaint(sender, e);
			}
		}

		public void RaisePostBackEvent(string eventArgument)
		{
		}

		internal void OnRealTimeData(object sender, RealTimeDataEventArgs e)
		{
			if (this.RealTimeData != null)
			{
				this.RealTimeData(sender, e);
			}
		}

		internal void OnCallback(object sender, CallbackEventArgs e)
		{
			if (this.Callback != null)
			{
				this.Callback(sender, e);
			}
		}

		internal void OnValueChanged(object sender, ValueChangedEventArgs e)
		{
		}

		internal void OnPlaybackStateChanged(object sender, PlaybackStateChangedEventArgs e)
		{
		}

		internal void OnValueLimitOverflow(object sender, ValueChangedEventArgs e)
		{
		}

		internal void OnValueRateOfChangeExceed(object sender, ValueChangedEventArgs e)
		{
		}

		internal void OnValueRangeEnter(object sender, ValueRangeEventArgs e)
		{
		}

		internal void OnValueRangeLeave(object sender, ValueRangeEventArgs e)
		{
		}

		internal void OnValueRangeTimeOut(object sender, ValueRangeEventArgs e)
		{
		}

		internal void OnValueScaleEnter(object sender, ValueRangeEventArgs e)
		{
		}

		internal void OnValueScaleLeave(object sender, ValueRangeEventArgs e)
		{
		}

		internal void OnPointerPositionChange(object sender, PointerPositionChangeEventArgs e)
		{
		}
	}
}
