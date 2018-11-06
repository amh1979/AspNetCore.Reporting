using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net.Cache;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace AspNetCore.Reporting.Map.WebForms
{
	[Description("Map for ASP.NET is a fully managed .NET control that lets you add fantastic looking maps to your applications with ease.")]
	[ToolboxBitmap(typeof(MapControl), "Map.ico")]
	[DisplayName("Map")]
	internal class MapControl : IDisposable
	{
		internal const string ResKeyFormat = "#MapControlResKey#{0}#";

		private const string jsFilename = "DundasMap8.js";

		private const string imagemapExt = ".imagemap.txt";

		internal MapCore mapCore;

		internal string webFormDocumentURL = "";

		internal string applicationDocumentURL = "";

		internal static ITypeDescriptorContext controlCurrentContext = null;

		internal bool sessionExpired;

		internal bool generatingCachedContent;

		internal static string productID = "DG-WC";

		private bool doNotDispose;

		private bool isCallback;

		private Color backColor = Color.White;

		private bool enabled = true;

		public FormatNumberHandler FormatNumberHandler;

		private int width = 500;

		private int height = 375;

		private string resourceKey = "";

		private bool autoRunWizard;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryAttribute_Groups")]
		[SRDescription("DescriptionAttributeMapControl_GroupFields")]
		public FieldCollection GroupFields
		{
			get
			{
				return this.mapCore.GroupFields;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryAttribute_Paths")]
		[SRDescription("DescriptionAttributeMapControl_PathFields")]
		public FieldCollection PathFields
		{
			get
			{
				return this.mapCore.PathFields;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryAttribute_Shapes")]
		[SRDescription("DescriptionAttributeMapControl_ShapeFields")]
		public FieldCollection ShapeFields
		{
			get
			{
				return this.mapCore.ShapeFields;
			}
		}

		[SRCategory("CategoryAttribute_Symbols")]
		[SRDescription("DescriptionAttributeMapControl_SymbolFields")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FieldCollection SymbolFields
		{
			get
			{
				return this.mapCore.SymbolFields;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryAttribute_Groups")]
		[SRDescription("DescriptionAttributeMapControl_GroupRules")]
		public GroupRuleCollection GroupRules
		{
			get
			{
				return this.mapCore.GroupRules;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryAttribute_Shapes")]
		[SRDescription("DescriptionAttributeMapControl_ShapeRules")]
		public ShapeRuleCollection ShapeRules
		{
			get
			{
				return this.mapCore.ShapeRules;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeMapControl_PathRules")]
		[SRCategory("CategoryAttribute_Paths")]
		public PathRuleCollection PathRules
		{
			get
			{
				return this.mapCore.PathRules;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeMapControl_SymbolRules")]
		[SRCategory("CategoryAttribute_Symbols")]
		public SymbolRuleCollection SymbolRules
		{
			get
			{
				return this.mapCore.SymbolRules;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeMapControl_Groups")]
		[SRCategory("CategoryAttribute_Groups")]
		public GroupCollection Groups
		{
			get
			{
				return this.mapCore.Groups;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryAttribute_Layers")]
		[SRDescription("DescriptionAttributeMapControl_Layers")]
		public LayerCollection Layers
		{
			get
			{
				return this.mapCore.Layers;
			}
		}

		[SRCategory("CategoryAttribute_Shapes")]
		[SRDescription("DescriptionAttributeMapControl_Shapes")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ShapeCollection Shapes
		{
			get
			{
				return this.mapCore.Shapes;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryAttribute_Paths")]
		[SRDescription("DescriptionAttributeMapControl_Paths")]
		public PathCollection Paths
		{
			get
			{
				return this.mapCore.Paths;
			}
		}

		[SRCategory("CategoryAttribute_Symbols")]
		[SRDescription("DescriptionAttributeMapControl_Shapes")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SymbolCollection Symbols
		{
			get
			{
				return this.mapCore.Symbols;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeMapControl_Images")]
		[SRCategory("CategoryAttribute_Panels")]
		public MapImageCollection Images
		{
			get
			{
				return this.mapCore.Images;
			}
		}

		[SRDescription("DescriptionAttributeMapControl_Labels")]
		[SRCategory("CategoryAttribute_Panels")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MapLabelCollection Labels
		{
			get
			{
				return this.mapCore.Labels;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SRCategory("CategoryAttribute_MapControl")]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeMapControl_NamedImages")]
		public NamedImageCollection NamedImages
		{
			get
			{
				return this.mapCore.NamedImages;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeMapControl_DataBindingRules")]
		public DataBindingRulesCollection DataBindingRules
		{
			get
			{
				return this.mapCore.DataBindingRules;
			}
		}

		[SRDescription("DescriptionAttributeMapControl_MapAreas")]
		[SRCategory("CategoryAttribute_Image")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MapAreaCollection MapAreas
		{
			get
			{
				return this.mapCore.MapAreas;
			}
		}

		[DefaultValue(true)]
		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeMapControl_ImageMapEnabled")]
		public bool ImageMapEnabled
		{
			get
			{
				return this.mapCore.ImageMapEnabled;
			}
			set
			{
				this.mapCore.ImageMapEnabled = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(25f)]
		[SRDescription("DescriptionAttributeMapControl_ShadowIntensity")]
		public float ShadowIntensity
		{
			get
			{
				return this.mapCore.ShadowIntensity;
			}
			set
			{
				if (!(value > 100.0) && !(value < 0.0))
				{
					this.mapCore.ShadowIntensity = value;
					return;
				}
				throw new ArgumentException(SR.must_in_range(0.0, 100.0));
			}
		}

		[DefaultValue(typeof(AntiAliasing), "All")]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapControl_AntiAliasing")]
		public AntiAliasing AntiAliasing
		{
			get
			{
				return this.mapCore.AntiAliasing;
			}
			set
			{
				this.mapCore.AntiAliasing = value;
			}
		}

		[SRDescription("DescriptionAttributeMapControl_TextAntiAliasingQuality")]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(TextAntiAliasingQuality.High)]
		public TextAntiAliasingQuality TextAntiAliasingQuality
		{
			get
			{
				return this.mapCore.TextAntiAliasingQuality;
			}
			set
			{
				this.mapCore.TextAntiAliasingQuality = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public MapSerializer Serializer
		{
			get
			{
				return this.mapCore.Serializer;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public CallbackManager CallbackManager
		{
			get
			{
				return this.mapCore.CallbackManager;
			}
		}

		[SRDescription("DescriptionAttributeMapControl_ViewStateContent")]
		[DefaultValue(SerializationContent.All)]
		[SRCategory("CategoryAttribute_ViewState")]
		public SerializationContent ViewStateContent
		{
			get
			{
				return this.mapCore.ViewStateContent;
			}
			set
			{
				this.mapCore.ViewStateContent = value;
			}
		}

		[SRDescription("DescriptionAttributeMapControl_ImageType")]
		[SRCategory("CategoryAttribute_Image")]
		[DefaultValue(ImageType.Png)]
		public ImageType ImageType
		{
			get
			{
				return this.mapCore.ImageType;
			}
			set
			{
				this.mapCore.ImageType = value;
			}
		}

		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeMapControl_Compression")]
		[SRCategory("CategoryAttribute_Image")]
		public int Compression
		{
			get
			{
				return this.mapCore.Compression;
			}
			set
			{
				this.mapCore.Compression = value;
			}
		}

		[SRDescription("DescriptionAttributeMapControl_ImageUrl")]
		[DefaultValue("TempFiles/MapPic_#SEQ(300,3)")]
		[SRCategory("CategoryAttribute_Image")]
		public string ImageUrl
		{
			get
			{
				return this.mapCore.ImageUrl;
			}
			set
			{
				int num = value.IndexOf("#SEQ", StringComparison.Ordinal);
				if (num > 0)
				{
					this.CheckImageURLSeqFormat(value);
				}
				this.mapCore.ImageUrl = value;
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeMapControl_RenderType")]
		[DefaultValue(RenderType.InteractiveImage)]
		public RenderType RenderType
		{
			get
			{
				return this.mapCore.RenderType;
			}
			set
			{
				this.mapCore.RenderType = value;
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeMapControl_ControlPersistence")]
		[DefaultValue(ControlPersistence.SessionState)]
		public ControlPersistence ControlPersistence
		{
			get
			{
				return this.mapCore.ControlPersistence;
			}
			set
			{
				this.mapCore.ControlPersistence = value;
			}
		}

		[SRDescription("DescriptionAttributeMapControl_RenderingImageUrl")]
		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Image")]
		public string RenderingImageUrl
		{
			get
			{
				return this.mapCore.RenderingImageUrl;
			}
			set
			{
				this.mapCore.RenderingImageUrl = value;
			}
		}

		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsCallback
		{
			get
			{
				return this.isCallback;
			}
		}

		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeMapControl_TagAttributes")]
		public string TagAttributes
		{
			get
			{
				return this.mapCore.TagAttributes;
			}
			set
			{
				this.mapCore.TagAttributes = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapControl_BorderLineColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "DarkGray")]
		public Color BorderLineColor
		{
			get
			{
				return this.mapCore.BorderLineColor;
			}
			set
			{
				this.mapCore.BorderLineColor = value;
			}
		}

		[SRDescription("DescriptionAttributeMapControl_BorderLineStyle")]
		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
		[DefaultValue(MapDashStyle.Solid)]
		public MapDashStyle BorderLineStyle
		{
			get
			{
				return this.mapCore.BorderLineStyle;
			}
			set
			{
				this.mapCore.BorderLineStyle = value;
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeMapControl_BorderLineWidth")]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(0)]
		public int BorderLineWidth
		{
			get
			{
				return this.mapCore.BorderLineWidth;
			}
			set
			{
				this.mapCore.BorderLineWidth = value;
			}
		}

		[NotifyParentProperty(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeMapControl_Frame")]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(FrameStyle.None)]
		public Frame Frame
		{
			get
			{
				return this.mapCore.Frame;
			}
			set
			{
				this.mapCore.Frame = value;
			}
		}

		[SRCategory("CategoryAttribute_Panels")]
		[SRDescription("DescriptionAttributeMapControl_Viewport")]
		[TypeConverter(typeof(DockablePanelConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Viewport Viewport
		{
			get
			{
				return this.mapCore.Viewport;
			}
			set
			{
				this.mapCore.Viewport = value;
			}
		}

		[SRCategory("CategoryAttribute_Panels")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeMapControl_ZoomPanel")]
		[TypeConverter(typeof(DockablePanelConverter))]
		public ZoomPanel ZoomPanel
		{
			get
			{
				return this.mapCore.ZoomPanel;
			}
			set
			{
				this.mapCore.ZoomPanel = value;
			}
		}

		[SRCategory("CategoryAttribute_Panels")]
		[SRDescription("DescriptionAttributeMapControl_NavigationPanel")]
		[TypeConverter(typeof(DockablePanelConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavigationPanel NavigationPanel
		{
			get
			{
				return this.mapCore.NavigationPanel;
			}
			set
			{
				this.mapCore.NavigationPanel = value;
			}
		}

		[SRCategory("CategoryAttribute_Panels")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeMapControl_DistanceScalePanel")]
		[TypeConverter(typeof(DockablePanelConverter))]
		public DistanceScalePanel DistanceScalePanel
		{
			get
			{
				return this.mapCore.DistanceScalePanel;
			}
			set
			{
				this.mapCore.DistanceScalePanel = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryAttribute_Panels")]
		[SRDescription("DescriptionAttributeMapControl_ColorSwatchPanel")]
		[TypeConverter(typeof(DockablePanelConverter))]
		public ColorSwatchPanel ColorSwatchPanel
		{
			get
			{
				return this.mapCore.ColorSwatchPanel;
			}
			set
			{
				this.mapCore.ColorSwatchPanel = value;
			}
		}

		[SRCategory("CategoryAttribute_Panels")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeMapControl_Legends")]
		public LegendCollection Legends
		{
			get
			{
				return this.mapCore.Legends;
			}
		}

		[SRDescription("DescriptionAttributeMapControl_Parallels")]
		[SRCategory("CategoryAttribute_ParallelsAndMeridians")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public GridAttributes Parallels
		{
			get
			{
				return this.mapCore.Parallels;
			}
			set
			{
				this.mapCore.Parallels = value;
			}
		}

		[SRCategory("CategoryAttribute_ParallelsAndMeridians")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeMapControl_Meridians")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public GridAttributes Meridians
		{
			get
			{
				return this.mapCore.Meridians;
			}
			set
			{
				this.mapCore.Meridians = value;
			}
		}

		[SRCategory("CategoryAttribute_ParallelsAndMeridians")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeMapControl_GridUnderContent")]
		[NotifyParentProperty(true)]
		public bool GridUnderContent
		{
			get
			{
				return this.mapCore.GridUnderContent;
			}
			set
			{
				this.mapCore.GridUnderContent = value;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue(typeof(Projection), "Equirectangular")]
		[SRDescription("DescriptionAttributeMapControl_Projection")]
		public Projection Projection
		{
			get
			{
				return this.mapCore.Projection;
			}
			set
			{
				this.mapCore.Projection = value;
			}
		}

		[DefaultValue(typeof(Color), "LightBlue")]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapControl_SelectionMarkerColor")]
		public Color SelectionMarkerColor
		{
			get
			{
				return this.mapCore.SelectionMarkerColor;
			}
			set
			{
				this.mapCore.SelectionMarkerColor = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapControl_SelectionBorderColor")]
		[DefaultValue(typeof(Color), "Black")]
		public Color SelectionBorderColor
		{
			get
			{
				return this.mapCore.SelectionBorderColor;
			}
			set
			{
				this.mapCore.SelectionBorderColor = value;
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
			}
		}

		[DefaultValue(true)]
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		//[Editor(typeof(GradientEditor), typeof(UITypeEditor))]
		[SRDescription("DescriptionAttributeMapControl_BackGradientType")]
		[DefaultValue(GradientType.None)]
		public GradientType BackGradientType
		{
			get
			{
				return this.mapCore.BackGradientType;
			}
			set
			{
				this.mapCore.BackGradientType = value;
			}
		}

		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapControl_BackSecondaryColor")]
		public Color BackSecondaryColor
		{
			get
			{
				return this.mapCore.BackSecondaryColor;
			}
			set
			{
				this.mapCore.BackSecondaryColor = value;
			}
		}

		[SRDescription("DescriptionAttributeMapControl_BackHatchStyle")]
		[DefaultValue(MapHatchStyle.None)]
		[SRCategory("CategoryAttribute_Appearance")]
		//[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		public MapHatchStyle BackHatchStyle
		{
			get
			{
				return this.mapCore.BackHatchStyle;
			}
			set
			{
				this.mapCore.BackHatchStyle = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapControl_MapLimits")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public MapLimits MapLimits
		{
			get
			{
				return this.mapCore.MapLimits;
			}
			set
			{
				this.mapCore.MapLimits = value;
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapControl_AutoLimitsIgnoreSymbols")]
		[DefaultValue(false)]
		public bool AutoLimitsIgnoreSymbols
		{
			get
			{
				return this.mapCore.AutoLimitsIgnoreSymbols;
			}
			set
			{
				this.mapCore.AutoLimitsIgnoreSymbols = value;
			}
		}

		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapControl_ProjectionCenter")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ProjectionCenter ProjectionCenter
		{
			get
			{
				return this.mapCore.ProjectionCenter;
			}
			set
			{
				this.mapCore.ProjectionCenter = value;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeMapControl_GeographyMode")]
		public bool GeographyMode
		{
			get
			{
				return this.mapCore.GeographyMode;
			}
			set
			{
				this.mapCore.GeographyMode = value;
			}
		}

		[DefaultValue(10000)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapControl_TileServerTimeout")]
		public int TileServerTimeout
		{
			get
			{
				return this.mapCore.TileServerTimeout;
			}
			set
			{
				this.mapCore.TileServerTimeout = value;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue(2)]
		[SRDescription("DescriptionAttributeMapControl_TileServerMaxConnections")]
		public int TileServerMaxConnections
		{
			get
			{
				return this.mapCore.TileServerMaxConnections;
			}
			set
			{
				this.mapCore.TileServerMaxConnections = value;
			}
		}

		[SRCategory("CategoryAttribute_VirtualEarth")]
		[DefaultValue(typeof(CultureInfo), "Invariant Language (Invariant Country)")]
		[SRDescription("DescriptionAttributeMapControl_TileCulture")]
		public CultureInfo TileCulture
		{
			get
			{
				return this.mapCore.TileCulture;
			}
			set
			{
				this.mapCore.TileCulture = value;
			}
		}

		[DefaultValue(RequestCacheLevel.Default)]
		[SRCategory("CategoryAttribute_VirtualEarth")]
		[SRDescription("DescriptionAttributeMapControl_TileCacheLevel")]
		public RequestCacheLevel TileCacheLevel
		{
			get
			{
				return this.mapCore.TileCacheLevel;
			}
			set
			{
				this.mapCore.TileCacheLevel = value;
			}
		}

		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRCategory("CategoryAttribute_VirtualEarth")]
		[SRDescription("DescriptionAttributeMapControl_TileServerAppId")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string TileServerAppId
		{
			set
			{
				this.mapCore.TileServerAppId = value;
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeMapControl_ContentCachingEnabled")]
		public bool ContentCachingEnabled
		{
			get
			{
				return this.mapCore.ContentCachingEnabled;
			}
			set
			{
				this.mapCore.ContentCachingEnabled = value;
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeMapControl_ContentCachingTimeout")]
		public double ContentCachingTimeout
		{
			get
			{
				return this.mapCore.ContentCachingTimeout;
			}
			set
			{
				this.mapCore.ContentCachingTimeout = value;
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[DefaultValue(500)]
		[SRDescription("DescriptionAttributeMapControl_Width")]
		public int Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
				if (this.mapCore.Viewport.ContentSize == 0)
				{
					this.mapCore.ResetCachedPaths();
				}
				this.mapCore.InvalidateAndLayout();
				this.Invalidate();
			}
		}

		[DefaultValue(375)]
		[SRDescription("DescriptionAttributeMapControl_Height")]
		[SRCategory("CategoryAttribute_Image")]
		public int Height
		{
			get
			{
				return this.height;
			}
			set
			{
				this.height = value;
				if (this.mapCore.Viewport.ContentSize == 0)
				{
					this.mapCore.ResetCachedPaths();
				}
				this.mapCore.InvalidateAndLayout();
				this.Invalidate();
			}
		}

		internal ISelectable SelectedDesignTimeElement
		{
			get
			{
				return this.mapCore.SelectedDesignTimeElement;
			}
			set
			{
				this.mapCore.SelectedDesignTimeElement = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public string ResourceKey
		{
			get
			{
				return this.resourceKey;
			}
			set
			{
				this.resourceKey = value;
			}
		}

		[DefaultValue(false)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool InternalAutoRunWizard
		{
			get
			{
				return this.autoRunWizard;
			}
			set
			{
				this.autoRunWizard = value;
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[Description("Called just before to databind the control. This event will be called once for each databinding object in the DataBindingRules collection.")]
		public event DataBindEventHandler BeforeDataBind;

		[Description("Called right after databinding the control. This event will be called once for each databinding object in the DataBindingRules collection.")]
		[SRCategory("CategoryAttribute_Data")]
		public event DataBindEventHandler AfterDataBind;

		internal event EventHandler BeforeApplyingRules;

		[Category("Action")]
		[Description("Fires when all defined rules for the control are applied.")]
		public event EventHandler AllRulesApplied;

		[Description("Called just before a map element's background is painted.")]
		[SRCategory("CategoryAttribute_Appearance")]
		public event MapPaintEvent PrePaint;

		[Description("Called just after a map element's background is painted.")]
		[SRCategory("CategoryAttribute_Appearance")]
		public event MapPaintEvent PostPaint;

		[Description("Called after a map element is added to a collection.")]
		[SRCategory("CategoryAttribute_Behavior")]
		public event ElementEvent ElementAdded;

		[SRCategory("CategoryAttribute_Behavior")]
		[Description("Called after a map element is removed from a collection.")]
		public event ElementEvent ElementRemoved;

		[Description("Fires when the user clicks on the control.")]
		[Category("Action")]
		public event ClickEvent Click;

		[Description("Fires during a user callback.")]
		[Category("Action")]
		public event CallbackEvent Callback;

		public MapControl()
		{
			this.mapCore = new MapCore(this);
			this.mapCore.BeginInit();
			this.BackColor = Color.White;
			this.Width = 500;
			this.Height = 375;
			this.BackColor = Color.White;
		}

		~MapControl()
		{
			this.doNotDispose = false;
			this.Dispose();
		}

		public void Dispose()
		{
			if (this.mapCore != null)
			{
				this.mapCore.Dispose();
				this.mapCore = null;
			}
		}

		public HitTestResult HitTest(int x, int y)
		{
			return this.mapCore.HitTest(x, y, new Type[0], false)[0];
		}

		public HitTestResult HitTest(int x, int y, Type objectType)
		{
			return this.mapCore.HitTest(x, y, new Type[1]
			{
				objectType
			}, false)[0];
		}

		public HitTestResult HitTest(int x, int y, Type[] objectTypes)
		{
			return this.mapCore.HitTest(x, y, objectTypes, false)[0];
		}

		public HitTestResult[] HitTest(int x, int y, Type[] objectTypes, bool returnMultipleElements)
		{
			return this.mapCore.HitTest(x, y, objectTypes, returnMultipleElements);
		}

		public void SaveAsImage(string fileName, MapImageFormat format)
		{
			this.SaveAsImage(fileName, format, this.Compression);
		}

		public void SaveAsImage(string fileName, MapImageFormat format, int compression)
		{
			this.mapCore.SaveTo(fileName, format, compression, null, false);
		}

		public void SaveAsImage(Stream stream, MapImageFormat format)
		{
			this.SaveAsImage(stream, format, this.Compression);
		}

		public void SaveAsImage(Stream stream, MapImageFormat format, int compression)
		{
			this.mapCore.SaveTo(stream, format, compression, null, false);
		}

		public void SaveAsImage(Stream stream)
		{
			MapImageFormat imageFormat = (MapImageFormat)Enum.Parse(typeof(MapImageFormat), ((Enum)(object)this.ImageType).ToString((IFormatProvider)CultureInfo.CurrentCulture), true);
			this.mapCore.SaveTo(stream, imageFormat, this.Compression, null, false);
		}

		public void SaveAsImage(string fileName)
		{
			MapImageFormat imageFormat = (MapImageFormat)Enum.Parse(typeof(MapImageFormat), ((Enum)(object)this.ImageType).ToString((IFormatProvider)CultureInfo.CurrentCulture), true);
			this.mapCore.SaveTo(fileName, imageFormat, this.Compression, null, false);
		}

		public void DataBindShapes(object dataSource, string dataMember, string bindingField)
		{
			this.mapCore.DataBindShapes(dataSource, dataMember, bindingField);
		}

		public void DataBindGroups(object dataSource, string dataMember, string bindingField)
		{
			this.mapCore.DataBindGroups(dataSource, dataMember, bindingField);
		}

		public void DataBindPaths(object dataSource, string dataMember, string bindingField)
		{
			this.mapCore.DataBindPaths(dataSource, dataMember, bindingField);
		}

		public void DataBindSymbols(object dataSource, string dataMember, string bindingField, string category, string parentShapeField)
		{
			this.mapCore.DataBindSymbols(dataSource, dataMember, bindingField, category, parentShapeField, string.Empty, string.Empty);
		}

		public void DataBindSymbols(object dataSource, string dataMember, string bindingField, string category, string xCoordinateField, string yCoordinateField)
		{
			this.mapCore.DataBindSymbols(dataSource, dataMember, bindingField, category, string.Empty, xCoordinateField, yCoordinateField);
		}

		public void PrintPaint(Graphics g, Rectangle position)
		{
			this.mapCore.PrintPaint(g, position);
		}

		public void LoadFromShapeFile(string fileName, string nameColumn, bool importData)
		{
			if (nameColumn == null)
			{
				nameColumn = "";
			}
			this.mapCore.LoadFromShapeFile(fileName, nameColumn, importData);
		}

		public void LoadFromSQLServerSpatial(string connectionString, string sqlStatement, string nameColumn, string spatialColumn, BasicMapElements mapElementsToLoad, bool importAllData, string layer)
		{
			if (nameColumn == null)
			{
				nameColumn = string.Empty;
			}
			if (layer == null)
			{
				layer = string.Empty;
			}
			this.mapCore.LoadFromSpatial(connectionString, sqlStatement, nameColumn, spatialColumn, mapElementsToLoad, importAllData, layer);
		}

		public void LoadFromSQLServerSpatial(DataTable dataTable, string nameColumn, string spatialColumn, BasicMapElements mapElementsToLoad, bool importAllData, string layer)
		{
			if (nameColumn == null)
			{
				nameColumn = string.Empty;
			}
			if (layer == null)
			{
				layer = string.Empty;
			}
			this.mapCore.LoadFromSpatial(dataTable, nameColumn, spatialColumn, mapElementsToLoad, importAllData, layer);
		}

		public void LoadFromSQLServerSpatial(DataTable dataTable, string nameColumn, string spatialColumn, BasicMapElements mapElementsToLoad, string[] additionalColumnsToImport, string layer)
		{
			if (nameColumn == null)
			{
				nameColumn = string.Empty;
			}
			if (layer == null)
			{
				layer = string.Empty;
			}
			if (additionalColumnsToImport == null)
			{
				additionalColumnsToImport = new string[0];
			}
			this.mapCore.LoadFromSpatial(dataTable, nameColumn, spatialColumn, mapElementsToLoad, additionalColumnsToImport, layer);
		}

		public void Simplify(float factor)
		{
			this.mapCore.Simplify(factor);
		}

		public PointF PixelsToPercents(PointF pointInPixels)
		{
			return this.mapCore.PixelsToPercents(pointInPixels);
		}

		public PointF PercentsToPixels(PointF pointInPercents)
		{
			return this.mapCore.PercentsToPixels(pointInPercents);
		}

		public SizeF PixelsToPercents(SizeF sizeInPixels)
		{
			return this.mapCore.PixelsToPercents(sizeInPixels);
		}

		public SizeF PercentsToPixels(SizeF sizeInPercents)
		{
			return this.mapCore.PercentsToPixels(sizeInPercents);
		}

		public MapPoint PixelsToGeographic(PointF pointInPixels)
		{
			return this.mapCore.PixelsToGeographic(pointInPixels);
		}

		public PointF GeographicToPixels(MapPoint pointOnMap)
		{
			return this.mapCore.GeographicToPixels(pointOnMap);
		}

		public MapPoint PercentsToGeographic(MapPoint pointInPercents)
		{
			return this.mapCore.PercentsToGeographic(pointInPercents);
		}

		public MapPoint GeographicToPercents(MapPoint pointOnMap)
		{
			Point3D point3D = this.mapCore.GeographicToPercents(pointOnMap);
			return new MapPoint(point3D.X, point3D.Y);
		}

		public double MeasureDistance(MapPoint point1, MapPoint point2)
		{
			return this.mapCore.MeasureDistance(point1, point2);
		}

		public void CenterView(MapPoint pointOnMap)
		{
			this.mapCore.CenterView(pointOnMap);
		}

		public void CreateGroups(string shapeFieldName)
		{
			this.mapCore.CreateGroups(shapeFieldName);
		}

		public void ApplyRules()
		{
			this.mapCore.ApplyAllRules();
		}

		public ArrayList Find(string searchFor, bool ignoreCase, bool exactSearch)
		{
			return this.mapCore.Find(searchFor, ignoreCase, exactSearch);
		}

		protected internal virtual void OnBeforeDataBind(DataBindEventArgs e)
		{
			if (this.BeforeDataBind != null)
			{
				this.BeforeDataBind(this, e);
			}
		}

		protected internal virtual void OnAfterDataBind(DataBindEventArgs e)
		{
			if (this.AfterDataBind != null)
			{
				this.AfterDataBind(this, e);
			}
		}

		internal void OnBeforeApplyingRules(EventArgs e)
		{
			if (this.BeforeApplyingRules != null)
			{
				this.BeforeApplyingRules(this, e);
			}
		}

		internal void OnAllRulesApplied(EventArgs e)
		{
			if (this.AllRulesApplied != null)
			{
				this.AllRulesApplied(this, e);
			}
		}

		internal string GetHtmlColor(Color color, bool allowTransparent)
		{
			if (color.A == 0)
			{
				if (allowTransparent)
				{
					return "transparent";
				}
				color = Color.White;
			}
			else if (color.A < 255)
			{
				color = Color.FromArgb(color.R, color.G, color.B);
			}
			return string.Format(CultureInfo.InvariantCulture, "rgb({0}, {1}, {2})", color.R, color.G, color.B);
		}

		internal string GetHtmlBorderStyle(Panel panel)
		{
			string empty = string.Empty;
			if (panel.BorderWidth == 0)
			{
				return empty;
			}
			empty = ((panel.BorderStyle == MapDashStyle.Dash || panel.BorderStyle == MapDashStyle.DashDot || panel.BorderStyle == MapDashStyle.DashDotDot) ? (empty + "border-style: dashed;") : ((panel.BorderStyle != MapDashStyle.Dot) ? ((panel.BorderStyle != MapDashStyle.Solid) ? (empty + "border-style: none;") : (empty + "border-style: solid;")) : (empty + "border-style: dotted;")));
			empty = empty + "border-width: " + panel.BorderWidth.ToString(CultureInfo.CurrentCulture) + "px;";
			return empty + "border-color: " + this.GetHtmlColor(panel.BorderColor, true) + ";";
		}

		internal string GetPanelHref(Panel panel)
		{
			if (panel.Href == "")
			{
				return "";
			}
			return "onclick=\"window.location='" + panel.Href + "';\" ";
		}

		internal string GetPanelHrefStyle(Panel panel)
		{
			if (panel.Href == "")
			{
				return "";
			}
			return "cursor: pointer; ";
		}

		internal string GetPanelImageUrl(string imageUrl, string panelName)
		{
			int startIndex = imageUrl.LastIndexOf('.');
			return imageUrl.Insert(startIndex, panelName);
		}

		internal string GetUrl(string imageUrl, string filename)
		{
			int num = imageUrl.LastIndexOf('/');
			if (num == -1)
			{
				return filename;
			}
			return imageUrl.Substring(0, num + 1) + filename;
		}

		internal void SaveFiles(string fullImagePath)
		{
			string directoryName = System.IO.Path.GetDirectoryName(fullImagePath);
			DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
			if (!directoryInfo.Exists)
			{
				Directory.CreateDirectory(directoryName);
			}
			string text = directoryName + "\\DundasMap8.js";
			FileInfo fileInfo = new FileInfo(text);
			if (!fileInfo.Exists)
			{
				Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(MapControl).Namespace + ".DundasMap.js");
				byte[] array = new byte[manifestResourceStream.Length];
				manifestResourceStream.Read(array, 0, array.Length);
				string value = this.ObfuscateJavaScript(Encoding.UTF8.GetString(array));
				try
				{
					StreamWriter streamWriter = File.CreateText(text);
					streamWriter.Write(value);
					streamWriter.Close();
				}
				catch
				{
				}
			}
			if (this.RenderType != RenderType.InteractiveImage)
			{
				if (this.IsImageCached(fullImagePath) == 0)
				{
					try
					{
						this.SaveAsImage(fullImagePath);
					}
					catch
					{
					}
				}
			}
			else
			{
				if (this.IsImageCached(fullImagePath) == 0)
				{
					try
					{
						this.mapCore.RenderingMode = RenderingMode.Background;
						this.SaveAsImage(fullImagePath);
						this.mapCore.RenderingMode = RenderingMode.All;
					}
					catch
					{
					}
				}
				Panel[] sortedPanels = this.mapCore.GetSortedPanels();
				Panel[] array2 = sortedPanels;
				foreach (Panel panel in array2)
				{
					if (!(panel is Viewport) && panel.Visible)
					{
						string panelImageUrl = this.GetPanelImageUrl(fullImagePath, panel.Name);
						if (this.IsImageCached(panelImageUrl) == 0)
						{
							try
							{
								this.mapCore.SavePanelAsImage(panel, panelImageUrl, false);
							}
							catch
							{
							}
						}
						if (panel is ZoomPanel)
						{
							string panelImageUrl2 = this.GetPanelImageUrl(fullImagePath, panel.Name + "Thumb");
							if (this.IsImageCached(panelImageUrl2) == 0)
							{
								try
								{
									this.mapCore.SavePanelAsImage(panel, panelImageUrl2, true);
								}
								catch
								{
								}
							}
						}
					}
				}
				string panelImageUrl3 = this.GetPanelImageUrl(fullImagePath, "Background");
				if (this.IsImageCached(panelImageUrl3) == 0)
				{
					try
					{
						Bitmap bitmap = new Bitmap(1, 1);
						bitmap.SetPixel(0, 0, this.mapCore.GetGridSectionBackColor());
						bitmap.Save(panelImageUrl3, ImageFormat.Png);
					}
					catch
					{
					}
				}
				string text2 = directoryName + "\\Empty.gif";
				FileInfo fileInfo2 = new FileInfo(text2);
				if (!fileInfo2.Exists)
				{
					Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(MapControl).Namespace + ".Empty.gif");
					byte[] array3 = new byte[manifestResourceStream2.Length];
					manifestResourceStream2.Read(array3, 0, array3.Length);
					try
					{
						FileStream fileStream = File.Create(text2);
						fileStream.Write(array3, 0, array3.Length);
						fileStream.Close();
					}
					catch
					{
					}
				}
				if (this.RenderingImageUrl == "")
				{
					string text3 = directoryName + "\\Rendering.gif";
					FileInfo fileInfo3 = new FileInfo(text3);
					if (!fileInfo3.Exists)
					{
						Stream manifestResourceStream3 = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(MapControl).Namespace + ".Rendering.gif");
						byte[] array4 = new byte[manifestResourceStream3.Length];
						manifestResourceStream3.Read(array4, 0, array4.Length);
						try
						{
							FileStream fileStream2 = File.Create(text3);
							fileStream2.Write(array4, 0, array4.Length);
							fileStream2.Close();
						}
						catch
						{
						}
					}
				}
			}
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

		private bool SaveGridSectionImageMap(string fullImageMapPath)
		{
			bool result = false;
			MapAreaCollection mapAreasFromHotRegionList = this.mapCore.GetMapAreasFromHotRegionList();
			if (mapAreasFromHotRegionList.Count > 0)
			{
				try
				{
					StreamWriter streamWriter = File.CreateText(fullImageMapPath);
					foreach (MapArea item in mapAreasFromHotRegionList)
					{
						string tag = item.GetTag();
						if (tag.Length > 0)
						{
							streamWriter.Write(tag);
							result = true;
						}
					}
					streamWriter.Close();
					return result;
				}
				catch
				{
					return false;
				}
			}
			return result;
		}

		private long GetLastWriteTime(string fullImagePath)
		{
			FileInfo fileInfo = new FileInfo(fullImagePath);
			return fileInfo.LastWriteTime.Ticks;
		}

		private long IsImageCached(string fullImagePath)
		{
			if (this.ContentCachingEnabled && !this.IsDesignMode() && !this.generatingCachedContent)
			{
				FileInfo fileInfo = new FileInfo(fullImagePath);
				if (!fileInfo.Exists)
				{
					return 0L;
				}
				if (this.ContentCachingTimeout == 0.0)
				{
					return fileInfo.LastWriteTime.Ticks;
				}
				if ((DateTime.Now - fileInfo.LastWriteTime).TotalMinutes < this.ContentCachingTimeout)
				{
					return fileInfo.LastWriteTime.Ticks;
				}
				return 0L;
			}
			return 0L;
		}

		private long IsFileCached(string fullImagePath, string fileExtension)
		{
			long num = this.IsImageCached(fullImagePath);
			if (num == 0)
			{
				return 0L;
			}
			return this.IsImageCached(fullImagePath + fileExtension);
		}

		private Point[] DecodeGridSectionIndexes(string gridXParam, string gridYParam)
		{
			char[] array = new char[1]
			{
				';'
			};
			gridXParam = gridXParam.TrimEnd(array);
			gridYParam = gridYParam.TrimEnd(array);
			string[] array2 = gridXParam.Split(array);
			string[] array3 = gridYParam.Split(array);
			Point[] array4 = new Point[array2.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array4[i].X = int.Parse(array2[i], CultureInfo.InvariantCulture);
				array4[i].Y = int.Parse(array3[i], CultureInfo.InvariantCulture);
			}
			return array4;
		}

		internal void Invalidate()
		{
		}

		internal void Refresh()
		{
		}

		private void CheckImageURLSeqFormat(string imageURL)
		{
			int num = imageURL.IndexOf("#SEQ", StringComparison.Ordinal);
			num += 4;
			if (imageURL[num] != '(')
			{
				throw new ArgumentException("Invalid image URL format. #SEQ formatter must be followed by (300,3), where 300 is a max sequence number and 3 is an image file time to live.", "ImageURL");
			}
			int num2 = imageURL.IndexOf(')', 1);
			if (num2 < 0)
			{
				throw new ArgumentException("Invalid image URL format. #SEQ formatter must be followed by (300,3), where 300 is a max sequence number and 3 is an image file time to live.", "ImageURL");
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
							throw new ArgumentException("Invalid image URL format. #SEQ formatter must be followed by (300,3), where 300 is a max sequence number and 3 is an image file time to live.", "ImageURL");
						}
					}
				}
				return;
			}
			throw new ArgumentException("Invalid image URL format. #SEQ formatter must be followed by (300,3), where 300 is a max sequence number and 3 is an image file time to live.", "ImageURL");
		}

		internal bool IsDesignMode()
		{
			return false;
		}

		internal void OnPrePaint(object sender, MapPaintEventArgs e)
		{
			if (this.PrePaint != null)
			{
				this.PrePaint(sender, e);
			}
		}

		internal void OnPostPaint(object sender, MapPaintEventArgs e)
		{
			if (this.PostPaint != null)
			{
				this.PostPaint(sender, e);
			}
		}

		internal void OnElementAdded(object sender, ElementEventArgs e)
		{
			if (this.ElementAdded != null)
			{
				this.ElementAdded(sender, e);
			}
		}

		internal void OnElementRemoved(object sender, ElementEventArgs e)
		{
			if (this.ElementRemoved != null)
			{
				this.ElementRemoved(sender, e);
			}
		}

		internal void OnClick(object sender, ClickEventArgs e)
		{
			MapControl mapControl = this;
			if (sender is MapControl && sender != this)
			{
				mapControl = (sender as MapControl);
			}
			if (mapControl.Click != null)
			{
				mapControl.Click(sender, e);
			}
		}

		internal void OnCallback(object sender, CallbackEventArgs e)
		{
			MapControl mapControl = this;
			if (sender is MapControl && sender != this)
			{
				mapControl = (sender as MapControl);
			}
			if (mapControl.Callback != null)
			{
				mapControl.Callback(sender, e);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This method is obsolete in VS2005 map control. The method doesn't do anything and is\tpresent for backward compatibility only")]
		public void LoadResourceData(Type rootType, string shapeResourceKey, string pathsResourceKey)
		{
		}
	}
}
