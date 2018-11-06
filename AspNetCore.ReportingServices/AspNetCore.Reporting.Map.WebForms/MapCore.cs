
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Text;
using System.Threading;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class MapCore : NamedElement, IDisposable
	{
		private struct PanelPair
		{
			private DockablePanel Panel1;

			private DockablePanel Panel2;

			public PanelPair(DockablePanel panel1, DockablePanel panel2)
			{
				this.Panel1 = panel1;
				this.Panel2 = panel2;
			}
		}

		internal class GridSection : IDisposable
		{
			public Point Origin;

			public BufferBitmap Bitmap;

			public HotRegionList HotRegions;

			public bool Dirty;

			public void Dispose()
			{
				if (this.HotRegions != null)
				{
					foreach (HotRegion item in this.HotRegions.List)
					{
						item.DoNotDispose = false;
					}
					this.HotRegions.Clear();
					this.HotRegions = null;
				}
				if (this.Bitmap != null)
				{
					this.Bitmap.Dispose();
					this.Bitmap = null;
				}
				GC.SuppressFinalize(this);
			}
		}

		private const double ProjectionLatitudeLimit = 89.0;

		private const double MercatorProjectionLatitudeLimit = 85.05112878;

		private const float VisibleContentMargin = 20f;

		internal const int MaximumResolvedKeywordLength = 80;

		private const int BufferSize = 4096;

		internal const string DefaultRuleLegendTest = "#FROMVALUE{N0} - #TOVALUE{N0}";

		internal const string NoneToken = "(none)";

		internal const string AllToken = "(all)";

		internal const string NameToken = "(Name)";

		private const double ClippingMargin = 1E-10;

		private static string[] _geoPoliticalBlockListLocales;

		private static Hashtable _geoPoliticalBlockHashtableLocales;

		private BorderTypeRegistry borderTypeRegistry;

		internal bool silentPaint;

		internal string loadedBuildNumber = string.Empty;

		internal bool skipPaint;

		private MapAreaCollection mapAreas;

		private Panel[] sortedPanels;

		private BufferBitmap bufferBitmap;

		private ImageLoader imageLoader;

		internal bool dirtyFlag;

		internal bool disableInvalidate;

		internal bool isInitializing;

		internal bool boundToDataSource;

		private bool invalidatingDataBind;

		private bool dataBinding;

		private bool isPrinting;

		private Size printSize = new Size(0, 0);

		internal ServiceContainer serviceContainer;

		private bool rulesDirty = true;

		private bool applyingRules;

		private bool cachedBoundsDirty = true;

		private bool updatingCachedBounds;

		private bool cachedPathsDirty = true;

		private bool resetingCachedPaths;

		private bool childSymbolsDirty = true;

		private bool resettingChildSymbols;

		private bool gridSectionsDirty = true;

		private bool recreatingGridSections;

		private int bufferedGridSectionCount;

		private MapBounds cachedBoundsAfterProjection;

		private Dictionary<RectangleF, RectangleF> cachedGeographicClipRectangles = new Dictionary<RectangleF, RectangleF>();

		private long openTileRequestCount;

		private EventWaitHandle requestsCompletedEvent = new EventWaitHandle(false, EventResetMode.ManualReset);

		private string[] userLocales;

		private FieldCollection groupFields;

		private FieldCollection shapeFields;

		private FieldCollection pathFields;

		private FieldCollection symbolFields;

		private GroupRuleCollection groupRules;

		private ShapeRuleCollection shapeRules;

		private PathRuleCollection pathRules;

		private SymbolRuleCollection symbolRules;

		private GroupCollection groups;

		private LayerCollection layers;

		private ShapeCollection shapes;

		private PathCollection paths;

		private SymbolCollection symbols;

		private NamedImageCollection namedImages;

		private MapImageCollection images;

		private MapLabelCollection labels;

		private LegendCollection legends;

		private DataBindingRulesCollection dataBindingRules;

		private Viewport viewport;

		private ZoomPanel zoomPanel;

		private NavigationPanel navigationPanel;

		private DistanceScalePanel distanceScalePanel;

		private ColorSwatchPanel colorSwatchPanel;

		private GridAttributes parallels;

		private GridAttributes meridians;

		private bool gridUnderContent;

		private AntiAliasing antiAliasing = AntiAliasing.All;

		private TextAntiAliasingQuality textAntiAliasingQuality = TextAntiAliasingQuality.High;

		private float shadowIntensity = 25f;

		private GradientType backGradientType;

		private Color backSecondaryColor = Color.Empty;

		private MapHatchStyle backHatchStyle;

		private Frame frame;

		private Projection projection;

		private Color borderLineColor = Color.DarkGray;

		private MapDashStyle borderLineStyle = MapDashStyle.Solid;

		private int borderLineWidth;

		private Color selectionMarkerColor = Color.LightBlue;

		private Color selectionBorderColor = Color.Black;

		private MapLimits mapLimits;

		private bool autoLimitsIgnoreSymbols;

		private ProjectionCenter projectionCenter;

		private bool geographyMode = true;

		private int tileServerTimeout = 10000;

		private int tileServerMaxConnections = 2;

		private CultureInfo tileCulture = CultureInfo.InvariantCulture;

		private RequestCacheLevel tileCacheLevel;

		private string tileServerAppId = "ArHDYkG4iBBgRo5ZKlvBPAXjj3tA13t5WMpr60l7m23-SSyZJoHsVVe2IecRKN88";

		private ImageType imageType = ImageType.Png;

		private int compression;

		private string mapImageUrl = "TempFiles/MapPic_#SEQ(300,3)";

		private bool imageMapEnabled = true;

		private RenderType renderType = RenderType.InteractiveImage;

		private ControlPersistence controlPersistence = ControlPersistence.SessionState;

		private string renderingImageUrl = "";

		private SerializationContent viewStateContent = SerializationContent.All;

		private string tagAttributes = "";

		private bool contentCachingEnabled;

		private double contentCachingTimeout;

		private object dataSource;

		private MapSerializer serializer;

		private CallbackManager callbackManager;

		private MapControl parent;

		private HotRegionList hotRegionList;

		private bool serializing;

		private MapPoint mapCenterPoint = new MapPoint(0.0, 0.0);

		private MapPoint minimumPoint = new MapPoint(-180.0, -90.0);

		private MapPoint maximumPoint = new MapPoint(180.0, 90.0);

		private RectangleF mapDockBounds = RectangleF.Empty;

		private ISelectable selectedDesignTimeElement;

		private bool useRSAccessibilityNames;

		private bool doPanelLayout = true;

		private RenderingMode renderingMode;

		private Panel panelToRender;

		private int singleGridSectionX;

		private int singleGridSectionY;

		private Hashtable tileImagesCache;

		private Image tileWaitImage;

		private bool uppercaseFieldKeywords = true;

		private int maxSpatialPointCount = 2147483647;

		private int maxSpatialElementCount = 2147483647;

		private double CurrentLatitudeLimit = 89.0;

		private int CurrentSrid = 2147483647;

		//private SqlGeography ClippingPolygon = SqlGeography.Parse("FULLGLOBE");

		internal LoadTilesHandler LoadTilesHandler;

		internal SaveTilesHandler SaveTilesHandler;

		public string licenseData = "";

		private double[,] A = new double[18, 4]
		{
			{
				-2.49E-06,
				0.0,
				-0.0001753,
				0.8487
			},
			{
				2.5E-07,
				-3.74E-05,
				-0.00036231,
				0.84751182
			},
			{
				-1.21E-06,
				-3.371E-05,
				-0.00071788,
				0.84479598
			},
			{
				3.22E-06,
				-5.18E-05,
				-0.00114546,
				0.840213
			},
			{
				-4.88E-06,
				-3.5E-06,
				-0.00142198,
				0.83359314
			},
			{
				2E-08,
				-7.677E-05,
				-0.00182334,
				0.8257851
			},
			{
				1.4E-06,
				-7.643E-05,
				-0.00258932,
				0.814752
			},
			{
				-2.22E-06,
				-5.546E-05,
				-0.00324874,
				0.80006949
			},
			{
				4.08E-06,
				-8.875E-05,
				-0.00396977,
				0.78216192
			},
			{
				-4.61E-06,
				-2.748E-05,
				-0.00455092,
				0.76060494
			},
			{
				2.82E-06,
				-9.667E-05,
				-0.00517168,
				0.73658673
			},
			{
				7.9E-07,
				-5.432E-05,
				-0.00592662,
				0.7086645
			},
			{
				8.2E-07,
				-4.252E-05,
				-0.0064108,
				0.67777182
			},
			{
				-2.03E-06,
				-3.021E-05,
				-0.00677444,
				0.64475739
			},
			{
				-6.94E-06,
				-6.071E-05,
				-0.00722903,
				0.60987582
			},
			{
				1.487E-05,
				-0.00016487,
				-0.00835697,
				0.57134484
			},
			{
				1.059E-05,
				5.822E-05,
				-0.00889021,
				0.52729731
			},
			{
				-1.448E-05,
				0.00021714,
				-0.0075134,
				0.48562614
			}
		};

		private double[,] B = new double[18, 4]
		{
			{
				-0.0,
				0.0,
				0.01676852,
				0.0
			},
			{
				0.0,
				-0.0,
				0.01676851,
				0.0838426
			},
			{
				-0.0,
				1E-08,
				0.01676854,
				0.1676852
			},
			{
				1E-08,
				-3E-08,
				0.01676845,
				0.2515278
			},
			{
				-3E-08,
				1E-07,
				0.0167688,
				0.3353704
			},
			{
				1.1E-07,
				-3.6E-07,
				0.01676749,
				0.419213
			},
			{
				-4.2E-07,
				1.34E-06,
				0.01677238,
				0.5030556
			},
			{
				-5.9E-07,
				-4.99E-06,
				0.01675411,
				0.5868982
			},
			{
				-4.7E-07,
				-1.383E-05,
				0.01666002,
				0.67047034
			},
			{
				-7.9E-07,
				-2.084E-05,
				0.01648669,
				0.75336633
			},
			{
				-7.1E-07,
				-3.264E-05,
				0.01621931,
				0.83518048
			},
			{
				-6.8E-07,
				-4.335E-05,
				0.0158394,
				0.91537187
			},
			{
				-8.7E-07,
				-5.362E-05,
				0.01535457,
				0.99339958
			},
			{
				-1.23E-06,
				-6.673E-05,
				0.01475283,
				1.06872269
			},
			{
				-7E-07,
				-8.515E-05,
				0.01399341,
				1.14066505
			},
			{
				-8.94E-06,
				-9.571E-05,
				0.01308909,
				1.20841528
			},
			{
				-1.547E-05,
				-0.00022979,
				0.01146158,
				1.27035062
			},
			{
				3.079E-05,
				-0.00046184,
				0.00800345,
				1.31998003
			}
		};

		private double[,] Phi = new double[18, 5]
		{
			{
				0.0,
				0.00144326,
				0.0,
				59.63554505,
				0.0
			},
			{
				5.0,
				-0.00721629,
				0.00036302,
				59.63557549,
				0.0838426
			},
			{
				10.0,
				0.0274219,
				-0.00145208,
				59.63548418,
				0.1676852
			},
			{
				15.0,
				-0.10247131,
				0.00544529,
				59.63581898,
				0.2515278
			},
			{
				20.0,
				0.38246336,
				-0.02032909,
				59.63457108,
				0.3353704
			},
			{
				25.0,
				-1.42738212,
				0.07587108,
				59.63922787,
				0.419213
			},
			{
				30.0,
				5.32706511,
				-0.28315521,
				59.62184863,
				0.5030556
			},
			{
				35.0,
				7.66388284,
				1.05674976,
				59.6867088,
				0.5868982
			},
			{
				40.0,
				6.65735123,
				2.97821103,
				60.02391911,
				0.67047034
			},
			{
				45.0,
				11.96458831,
				4.63381419,
				60.65492548,
				0.75336633
			},
			{
				50.0,
				14.04901782,
				7.57043206,
				61.65340551,
				0.83518048
			},
			{
				55.0,
				14.25180947,
				10.95026286,
				63.13860578,
				0.91537187
			},
			{
				60.0,
				34.69303182,
				14.28637103,
				65.10776253,
				0.99339958
			},
			{
				65.0,
				11.1754653,
				22.12593219,
				67.85045045,
				1.06872269
			},
			{
				70.0,
				202.71534041,
				24.53790023,
				71.20755668,
				1.14066505
			},
			{
				75.0,
				-173.6636948,
				65.73993304,
				77.32390065,
				1.20841528
			},
			{
				80.0,
				6340.38872653,
				33.47217309,
				83.46863618,
				1.27035062
			},
			{
				85.0,
				-10081.29471342,
				977.4814281,
				133.64166694,
				1.31998003
			}
		};

		private static double RadsInDegree;

		private static double DegreesInRad;

		private static double Cos35;

		private static double Eckert1Constant;

		private static double Eckert3ConstantA;

		private static double Eckert3ConstantB;

		private static double Eckert3ConstantC;

		private GridSection[,] gridSections;

		private int gridSectionsXCount;

		private int gridSectionsYCount;

		private int gridSectionsInViewportXCount;

		private int gridSectionsInViewportYCount;

		private Point[] gridSectionsArray;

		private Size gridSectionSize = Size.Empty;

		private Point gridSectionsOffset = Point.Empty;

		private int suspendUpdatesCount;

		private bool autoUpdates = true;

		public FieldCollection GroupFields
		{
			get
			{
				return this.groupFields;
			}
		}

		public FieldCollection ShapeFields
		{
			get
			{
				return this.shapeFields;
			}
		}

		public FieldCollection PathFields
		{
			get
			{
				return this.pathFields;
			}
		}

		public FieldCollection SymbolFields
		{
			get
			{
				return this.symbolFields;
			}
		}

		public GroupRuleCollection GroupRules
		{
			get
			{
				return this.groupRules;
			}
		}

		public ShapeRuleCollection ShapeRules
		{
			get
			{
				return this.shapeRules;
			}
		}

		public PathRuleCollection PathRules
		{
			get
			{
				return this.pathRules;
			}
		}

		public SymbolRuleCollection SymbolRules
		{
			get
			{
				return this.symbolRules;
			}
		}

		public GroupCollection Groups
		{
			get
			{
				return this.groups;
			}
		}

		public LayerCollection Layers
		{
			get
			{
				return this.layers;
			}
		}

		public ShapeCollection Shapes
		{
			get
			{
				return this.shapes;
			}
		}

		public PathCollection Paths
		{
			get
			{
				return this.paths;
			}
		}

		public SymbolCollection Symbols
		{
			get
			{
				return this.symbols;
			}
		}

		public NamedImageCollection NamedImages
		{
			get
			{
				return this.namedImages;
			}
		}

		public MapImageCollection Images
		{
			get
			{
				return this.images;
			}
		}

		public MapLabelCollection Labels
		{
			get
			{
				return this.labels;
			}
		}

		public LegendCollection Legends
		{
			get
			{
				return this.legends;
			}
		}

		public DataBindingRulesCollection DataBindingRules
		{
			get
			{
				return this.dataBindingRules;
			}
		}

		public MapAreaCollection MapAreas
		{
			get
			{
				return this.mapAreas;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Viewport Viewport
		{
			get
			{
				return this.viewport;
			}
			set
			{
				this.viewport = value;
				this.viewport.Common = this.Common;
				this.ZoomPanel.UpdateZoomRange();
				this.ZoomPanel.ZoomLevel = (double)this.viewport.Zoom;
				this.Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ZoomPanel ZoomPanel
		{
			get
			{
				return this.zoomPanel;
			}
			set
			{
				this.zoomPanel = value;
				this.zoomPanel.Common = this.Common;
				this.ZoomPanel.UpdateZoomRange();
				this.ZoomPanel.ZoomLevel = (double)this.viewport.Zoom;
				this.Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavigationPanel NavigationPanel
		{
			get
			{
				return this.navigationPanel;
			}
			set
			{
				this.navigationPanel = value;
				this.navigationPanel.Common = this.Common;
				this.Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DistanceScalePanel DistanceScalePanel
		{
			get
			{
				return this.distanceScalePanel;
			}
			set
			{
				this.distanceScalePanel = value;
				this.distanceScalePanel.Common = this.Common;
				this.Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColorSwatchPanel ColorSwatchPanel
		{
			get
			{
				return this.colorSwatchPanel;
			}
			set
			{
				this.colorSwatchPanel = value;
				this.colorSwatchPanel.Common = this.Common;
				this.Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridAttributes Parallels
		{
			get
			{
				return this.parallels;
			}
			set
			{
				this.parallels = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridAttributes Meridians
		{
			get
			{
				return this.meridians;
			}
			set
			{
				this.meridians = value;
			}
		}

		[DefaultValue(false)]
		public bool GridUnderContent
		{
			get
			{
				return this.gridUnderContent;
			}
			set
			{
				this.gridUnderContent = value;
				this.InvalidateViewport();
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
				this.InvalidateViewport();
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
				this.InvalidateViewport();
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
				this.InvalidateViewport();
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "White")]
		public Color BackColor
		{
			get
			{
				return this.MapControl.BackColor;
			}
			set
			{
				this.MapControl.BackColor = value;
				this.InvalidateViewport();
				this.Invalidate();
			}
		}

		[DefaultValue(GradientType.None)]
		public virtual GradientType BackGradientType
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

		[DefaultValue(typeof(Color), "")]
		public virtual Color BackSecondaryColor
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

		[DefaultValue(MapHatchStyle.None)]
		public virtual MapHatchStyle BackHatchStyle
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

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Frame Frame
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
					if (this.Viewport.ContentSize == 0)
					{
						this.InvalidateCachedPaths();
						this.InvalidateGridSections();
					}
					this.InvalidateAndLayout();
					this.printSize.Width = value;
				}
				else
				{
					this.MapControl.Width = value;
				}
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
					if (this.Viewport.ContentSize == 0)
					{
						this.InvalidateCachedPaths();
						this.InvalidateGridSections();
					}
					this.InvalidateAndLayout();
					this.printSize.Height = value;
				}
				else
				{
					this.MapControl.Height = value;
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string Name
		{
			get
			{
				return "MapControl";
			}
			set
			{
			}
		}

		[SRDescription("DescriptionAttributeMapCore_BuildNumber")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue("")]
		public string BuildNumber
		{
			get
			{
				string text = string.Empty;
				Assembly executingAssembly = Assembly.GetExecutingAssembly();
				if (executingAssembly != null)
				{
					text = executingAssembly.FullName.ToUpper(CultureInfo.CurrentCulture);
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeMapCore_ControlType")]
		[Browsable(false)]
		public string ControlType
		{
			get
			{
				return "DundasWebMap";
			}
			set
			{
			}
		}

		[DefaultValue(Projection.Equirectangular)]
		public Projection Projection
		{
			get
			{
				return this.projection;
			}
			set
			{
				this.projection = value;
				this.InvalidateCachedBounds();
				this.InvalidateCachedPaths();
				this.ResetCachedBoundsAfterProjection();
				this.InvalidateDistanceScalePanel();
				this.InvalidateViewport();
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeMapCore_BorderLineColor")]
		[DefaultValue(typeof(Color), "DarkGray")]
		[SRCategory("CategoryAttribute_Appearance")]
		public Color BorderLineColor
		{
			get
			{
				return this.borderLineColor;
			}
			set
			{
				this.borderLineColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(MapDashStyle.Solid)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapCore_BorderLineStyle")]
		public MapDashStyle BorderLineStyle
		{
			get
			{
				return this.borderLineStyle;
			}
			set
			{
				this.borderLineStyle = value;
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeMapCore_BorderLineWidth")]
		[DefaultValue(0)]
		[SRCategory("CategoryAttribute_Appearance")]
		public int BorderLineWidth
		{
			get
			{
				return this.borderLineWidth;
			}
			set
			{
				if (value >= 0 && value <= 100)
				{
					this.borderLineWidth = value;
					this.InvalidateAndLayout();
					return;
				}
				throw new ArgumentException(SR.must_in_range(0.0, 100.0));
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
				this.InvalidateViewport();
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
				this.InvalidateViewport();
				this.Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MapLimits MapLimits
		{
			get
			{
				return this.mapLimits;
			}
			set
			{
				this.mapLimits = value;
				this.mapLimits.Parent = this;
				this.InvalidateCachedPaths();
				this.ResetCachedBoundsAfterProjection();
				this.InvalidateDistanceScalePanel();
				this.InvalidateViewport();
			}
		}

		[DefaultValue(false)]
		public bool AutoLimitsIgnoreSymbols
		{
			get
			{
				return this.autoLimitsIgnoreSymbols;
			}
			set
			{
				this.autoLimitsIgnoreSymbols = value;
				this.InvalidateCachedPaths();
				this.ResetCachedBoundsAfterProjection();
				this.InvalidateDistanceScalePanel();
				this.InvalidateViewport();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ProjectionCenter ProjectionCenter
		{
			get
			{
				return this.projectionCenter;
			}
			set
			{
				this.projectionCenter = value;
				this.projectionCenter.Parent = this;
				this.InvalidateCachedPaths();
				this.ResetCachedBoundsAfterProjection();
				this.InvalidateDistanceScalePanel();
				this.InvalidateViewport();
			}
		}

		[DefaultValue(true)]
		public bool GeographyMode
		{
			get
			{
				return this.geographyMode;
			}
			set
			{
				this.geographyMode = value;
				this.InvalidateCachedBounds();
				this.InvalidateCachedPaths();
				this.ResetCachedBoundsAfterProjection();
				this.InvalidateDistanceScalePanel();
				this.InvalidateViewport();
			}
		}

		[DefaultValue(10000)]
		public int TileServerTimeout
		{
			get
			{
				return this.tileServerTimeout;
			}
			set
			{
				this.tileServerTimeout = value;
			}
		}

		[DefaultValue(2)]
		public int TileServerMaxConnections
		{
			get
			{
				return this.tileServerMaxConnections;
			}
			set
			{
				this.tileServerMaxConnections = value;
			}
		}

		[DefaultValue(typeof(CultureInfo), "Invariant Language (Invariant Country)")]
		public CultureInfo TileCulture
		{
			get
			{
				return this.tileCulture;
			}
			set
			{
				if (this.tileCulture != value)
				{
					this.tileCulture = value;
					this.InvalidateViewport();
				}
			}
		}

		[DefaultValue(RequestCacheLevel.Default)]
		public RequestCacheLevel TileCacheLevel
		{
			get
			{
				return this.tileCacheLevel;
			}
			set
			{
				if (this.tileCacheLevel != value)
				{
					this.tileCacheLevel = value;
					this.InvalidateViewport();
				}
			}
		}

		[DefaultValue("ArHDYkG4iBBgRo5ZKlvBPAXjj3tA13t5WMpr60l7m23-SSyZJoHsVVe2IecRKN88")]
		internal string TileServerAppId
		{
			get
			{
				return this.tileServerAppId;
			}
			set
			{
				if (this.tileServerAppId != value)
				{
					this.tileServerAppId = value;
					this.InvalidateViewport();
				}
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

		[DefaultValue("TempFiles/MapPic_#SEQ(300,3)")]
		public string ImageUrl
		{
			get
			{
				return this.mapImageUrl;
			}
			set
			{
				this.mapImageUrl = value;
			}
		}

		[DefaultValue(true)]
		public bool ImageMapEnabled
		{
			get
			{
				return this.imageMapEnabled;
			}
			set
			{
				this.imageMapEnabled = value;
			}
		}

		[DefaultValue(RenderType.InteractiveImage)]
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

		[DefaultValue(ControlPersistence.SessionState)]
		public ControlPersistence ControlPersistence
		{
			get
			{
				return this.controlPersistence;
			}
			set
			{
				this.controlPersistence = value;
			}
		}

		[DefaultValue("")]
		public string RenderingImageUrl
		{
			get
			{
				return this.renderingImageUrl;
			}
			set
			{
				this.renderingImageUrl = value;
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

		[DefaultValue(false)]
		public bool ContentCachingEnabled
		{
			get
			{
				return this.contentCachingEnabled;
			}
			set
			{
				this.contentCachingEnabled = value;
			}
		}

		[DefaultValue(0.0)]
		public double ContentCachingTimeout
		{
			get
			{
				return this.contentCachingTimeout;
			}
			set
			{
				this.contentCachingTimeout = value;
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
				if (!DataBindingHelper.IsValidDataSource(value))
				{
					throw new ArgumentException(SR.not_supported_DataSource(value.GetType().Name));
				}
				this.dataSource = value;
				this.InvalidateDataBinding();
				this.Invalidate();
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		internal MapSerializer Serializer
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

		internal MapControl MapControl
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

		internal MapPoint MapCenterPoint
		{
			get
			{
				MapPoint result = this.mapCenterPoint;
				if (!this.ProjectionCenter.IsXNaN())
				{
					result.X = this.ProjectionCenter.X;
				}
				if (!this.ProjectionCenter.IsYNaN())
				{
					result.Y = this.ProjectionCenter.Y;
				}
				return result;
			}
			set
			{
				this.mapCenterPoint = value;
				this.ResetCachedBoundsAfterProjection();
			}
		}

		internal MapPoint MinimumPoint
		{
			get
			{
				MapPoint result = this.minimumPoint;
				if (!this.MapLimits.IsMinimumXNaN())
				{
					result.X = this.MapLimits.MinimumX;
				}
				if (!this.MapLimits.IsMinimumYNaN())
				{
					result.Y = this.MapLimits.MinimumY;
				}
				return result;
			}
			set
			{
				this.minimumPoint = value;
				this.ResetCachedBoundsAfterProjection();
			}
		}

		internal MapPoint MaximumPoint
		{
			get
			{
				MapPoint result = this.maximumPoint;
				if (!this.MapLimits.IsMaximumXNaN())
				{
					result.X = this.MapLimits.MaximumX;
				}
				if (!this.MapLimits.IsMaximumYNaN())
				{
					result.Y = this.MapLimits.MaximumY;
				}
				return result;
			}
			set
			{
				this.maximumPoint = value;
				this.ResetCachedBoundsAfterProjection();
			}
		}

		internal RectangleF MapDockBounds
		{
			get
			{
				return this.mapDockBounds;
			}
			set
			{
				if (this.mapDockBounds != value)
				{
					this.mapDockBounds = value;
					this.InvalidateAndLayout();
				}
			}
		}

		internal ISelectable SelectedDesignTimeElement
		{
			get
			{
				NamedElement namedElement = this.selectedDesignTimeElement as NamedElement;
				if (namedElement != null && namedElement.Common == null)
				{
					this.selectedDesignTimeElement = null;
				}
				return this.selectedDesignTimeElement;
			}
			set
			{
				this.selectedDesignTimeElement = value;
				this.InvalidateViewport();
				this.Invalidate();
			}
		}

		internal bool UseRSAccessibilityNames
		{
			get
			{
				return this.useRSAccessibilityNames;
			}
			set
			{
				this.useRSAccessibilityNames = value;
				this.InvalidateViewport();
				this.Invalidate();
			}
		}

		internal bool DoPanelLayout
		{
			get
			{
				return this.doPanelLayout;
			}
			set
			{
				this.doPanelLayout = value;
			}
		}

		internal RenderingMode RenderingMode
		{
			get
			{
				return this.renderingMode;
			}
			set
			{
				this.renderingMode = value;
			}
		}

		internal Panel PanelToRender
		{
			get
			{
				return this.panelToRender;
			}
			set
			{
				this.panelToRender = value;
			}
		}

		internal int SingleGridSectionX
		{
			get
			{
				return this.singleGridSectionX;
			}
			set
			{
				this.singleGridSectionX = value;
			}
		}

		internal int SingleGridSectionY
		{
			get
			{
				return this.singleGridSectionY;
			}
			set
			{
				this.singleGridSectionY = value;
			}
		}

		internal Hashtable TileImagesCache
		{
			get
			{
				return this.tileImagesCache;
			}
			set
			{
				this.tileImagesCache = value;
			}
		}

		private Image TileWaitImage
		{
			get
			{
				if (this.tileWaitImage == null)
				{
					this.tileWaitImage = new Bitmap(256, 256);
					Utils.SetImageCustomProperty(this.tileWaitImage, CustomPropertyTag.ImageError, SR.DownloadingTile);
				}
				return this.tileWaitImage;
			}
		}

		internal bool UppercaseFieldKeywords
		{
			get
			{
				return this.uppercaseFieldKeywords;
			}
			set
			{
				this.uppercaseFieldKeywords = value;
			}
		}

		internal int MaxSpatialPointCount
		{
			get
			{
				return this.maxSpatialPointCount;
			}
			set
			{
				this.maxSpatialPointCount = value;
			}
		}

		internal int MaxSpatialElementCount
		{
			get
			{
				return this.maxSpatialElementCount;
			}
			set
			{
				this.maxSpatialElementCount = value;
			}
		}

		internal bool InvokeRequired
		{
			get
			{
				return false;
			}
		}

		internal GridSection[,] GridSections
		{
			get
			{
				return this.gridSections;
			}
			set
			{
				this.gridSections = value;
			}
		}

		internal int GridSectionsXCount
		{
			get
			{
				return this.gridSectionsXCount;
			}
			set
			{
				this.gridSectionsXCount = value;
			}
		}

		internal int GridSectionsYCount
		{
			get
			{
				return this.gridSectionsYCount;
			}
			set
			{
				this.gridSectionsYCount = value;
			}
		}

		internal int GridSectionsInViewportXCount
		{
			get
			{
				return this.gridSectionsInViewportXCount;
			}
			set
			{
				this.gridSectionsInViewportXCount = value;
			}
		}

		internal int GridSectionsInViewportYCount
		{
			get
			{
				return this.gridSectionsInViewportYCount;
			}
			set
			{
				this.gridSectionsInViewportYCount = value;
			}
		}

		internal Point[] GridSectionsArray
		{
			get
			{
				return this.gridSectionsArray;
			}
			set
			{
				this.gridSectionsArray = value;
			}
		}

		internal Size GridSectionSize
		{
			get
			{
				return this.gridSectionSize;
			}
			set
			{
				this.gridSectionSize = value;
			}
		}

		internal Point GridSectionsOffset
		{
			get
			{
				return this.gridSectionsOffset;
			}
			set
			{
				this.gridSectionsOffset = value;
			}
		}

		internal bool IsSuspended
		{
			get
			{
				return this.suspendUpdatesCount > 0;
			}
		}

		internal bool AutoUpdates
		{
			get
			{
				return this.autoUpdates;
			}
			set
			{
				this.autoUpdates = value;
			}
		}

		static MapCore()
		{
			MapCore._geoPoliticalBlockListLocales = new string[21]
			{
				"ES-AR",
				"ZH-CN",
				"ZH-HK",
				"AS-IN",
				"BN-IN",
				"EN-IN",
				"KOK-IN",
				"MR-IN",
				"HI-IN",
				"PA-IN",
				"GU-IN",
				"OR-IN",
				"TA-IN",
				"TE-IN",
				"KN-IN",
				"ML-IN",
				"SA-IN",
				"KO-KR",
				"ZH-MO",
				"AR-MA",
				"ES-VE"
			};
			MapCore._geoPoliticalBlockHashtableLocales = new Hashtable(StringComparer.OrdinalIgnoreCase);
			MapCore.RadsInDegree = 0.017453292519943295;
			MapCore.DegreesInRad = 57.295779513082323;
			MapCore.Cos35 = Math.Cos(0.6108652381980153);
			MapCore.Eckert1Constant = 2.0 * Math.Sqrt(0.21220659078919379);
			MapCore.Eckert3ConstantA = 2.0 / Math.Sqrt(22.43597501544853);
			MapCore.Eckert3ConstantB = 4.0 / Math.Sqrt(22.43597501544853);
			MapCore.Eckert3ConstantC = Math.Sqrt(22.43597501544853);
			for (int i = 0; i < MapCore._geoPoliticalBlockListLocales.Length; i++)
			{
				MapCore._geoPoliticalBlockHashtableLocales.Add(MapCore._geoPoliticalBlockListLocales[i], new object());
			}
		}

		public MapCore()
			: this(null)
		{
		}

		internal MapCore(MapControl parent)
		{
			this.parent = parent;
			this.serviceContainer = new ServiceContainer();
			this.serviceContainer.AddService(typeof(MapCore), this);
			this.serviceContainer.AddService(typeof(MapControl), this.parent);
			this.borderTypeRegistry = new BorderTypeRegistry(this.serviceContainer);
			this.serviceContainer.AddService(this.borderTypeRegistry.GetType(), this.borderTypeRegistry);
			base.common = new CommonElements(this.serviceContainer);
			this.shapeFields = new FieldCollection(this, base.common);
			this.pathFields = new FieldCollection(this, base.common);
			this.symbolFields = new FieldCollection(this, base.common);
			this.groupFields = new FieldCollection(this, base.common);
			this.shapes = new ShapeCollection(this, base.common);
			this.paths = new PathCollection(this, base.common);
			this.symbols = new SymbolCollection(this, base.common);
			this.shapeRules = new ShapeRuleCollection(this, base.common);
			this.pathRules = new PathRuleCollection(this, base.common);
			this.groupRules = new GroupRuleCollection(this, base.common);
			this.symbolRules = new SymbolRuleCollection(this, base.common);
			this.images = new MapImageCollection(this, base.common);
			this.labels = new MapLabelCollection(this, base.common);
			this.groups = new GroupCollection(this, base.common);
			this.layers = new LayerCollection(this, base.common);
			this.dataBindingRules = new DataBindingRulesCollection(this, base.common);
			this.legends = new LegendCollection(this, base.common);
			this.viewport = new Viewport(base.common);
			this.parallels = new GridAttributes(this, true);
			this.meridians = new GridAttributes(this, false);
			this.zoomPanel = new ZoomPanel(base.common);
			this.navigationPanel = new NavigationPanel(base.common);
			this.colorSwatchPanel = new ColorSwatchPanel(base.common);
			this.distanceScalePanel = new DistanceScalePanel(base.common);
			this.serializer = new MapSerializer(this.serviceContainer);
			this.frame = new Frame(this);
			this.mapLimits = new MapLimits(this);
			this.projectionCenter = new ProjectionCenter(this);
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
			this.mapAreas = new MapAreaCollection();
			this.callbackManager = new CallbackManager(this);
			this.namedImages = new NamedImageCollection(this, base.common);
			this.imageLoader = new ImageLoader(this.serviceContainer);
			this.tileImagesCache = new Hashtable(new CaseInsensitiveHashCodeProvider(CultureInfo.InvariantCulture), StringComparer.OrdinalIgnoreCase);
			this.serviceContainer.AddService(typeof(ImageLoader), this.imageLoader);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.DisposeGridSections();
				NamedCollection[] renderCollections = this.GetRenderCollections();
				foreach (NamedCollection namedCollection in renderCollections)
				{
					namedCollection.Dispose();
				}
				if (this.viewport != null)
				{
					this.viewport.Dispose();
				}
				if (this.parallels != null)
				{
					this.parallels.Dispose();
				}
				if (this.meridians != null)
				{
					this.meridians.Dispose();
				}
				if (this.zoomPanel != null)
				{
					this.zoomPanel.Dispose();
				}
				if (this.navigationPanel != null)
				{
					this.navigationPanel.Dispose();
				}
				if (this.colorSwatchPanel != null)
				{
					this.colorSwatchPanel.Dispose();
				}
				if (this.distanceScalePanel != null)
				{
					this.distanceScalePanel.Dispose();
				}
				if (this.frame != null)
				{
					this.frame.Dispose();
				}
				if (this.hotRegionList != null)
				{
					this.hotRegionList.Clear();
				}
				if (this.bufferBitmap != null)
				{
					this.bufferBitmap.Dispose();
				}
				if (this.namedImages != null)
				{
					this.namedImages.Dispose();
				}
				if (this.imageLoader != null)
				{
					this.imageLoader.Dispose();
				}
				if (this.tileImagesCache != null)
				{
					IDictionaryEnumerator enumerator = this.tileImagesCache.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
							if (dictionaryEntry.Value != null)
							{
								((Image)dictionaryEntry.Value).Dispose();
							}
						}
					}
					finally
					{
						IDisposable disposable = enumerator as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
					this.tileImagesCache = null;
				}
			}
			base.Dispose(disposing);
		}
        /*
		private static bool IsTypeInGeometry(Type type, SqlGeometry geometry)
		{
			string value = geometry.STGeometryType().Value;
			if (value == "GeometryCollection")
			{
				for (int i = 1; i <= geometry.STNumGeometries().Value; i++)
				{
					if (MapCore.IsTypeInGeometry(type, geometry.STGeometryN(i)))
					{
						return true;
					}
				}
				return false;
			}
			return MapCore.DetermineElementTypeFromGeometryType(value) == type;
		}

		private static bool IsTypeInGeography(Type type, SqlGeography geography)
		{
			string value = geography.STGeometryType().Value;
			if (value == "GeometryCollection")
			{
				for (int i = 1; i <= geography.STNumGeometries().Value; i++)
				{
					if (MapCore.IsTypeInGeography(type, geography.STGeometryN(i)))
					{
						return true;
					}
				}
				return false;
			}
			return MapCore.DetermineElementTypeFromGeometryType(value) == type;
		}
        */
		private static Type DetermineElementTypeFromGeometryType(string geometryType)
		{
			if (!(geometryType == "MultiPolygon") && !(geometryType == "Polygon") && !(geometryType == "CurvePolygon") && !(geometryType == "FullGlobe"))
			{
				if (!(geometryType == "MultiLineString") && !(geometryType == "LineString") && !(geometryType == "CircularString") && !(geometryType == "CompoundCurve"))
				{
					if (!(geometryType == "MultiPoint") && !(geometryType == "Point"))
					{
						return null;
					}
					return typeof(Symbol);
				}
				return typeof(Path);
			}
			return typeof(Shape);
		}

		internal static BasicMapElements? DetermineMapElementsFromSpatial(DataTable spatialTable, string spatialColumn)
		{
            /*
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (DataRow row in spatialTable.Rows)
			{
				SqlGeometry sqlGeometry = row[spatialColumn] as SqlGeometry;
				if (sqlGeometry == null || !sqlGeometry.IsNull)
				{
					SqlGeography sqlGeography = row[spatialColumn] as SqlGeography;
					object x = (SqlBoolean)(sqlGeography != null);
					if (!((SqlBoolean)x))
					{
						object sqlBoolean = (SqlBoolean)sqlGeography.IsNull;
						if (!((SqlBoolean)sqlBoolean))
						{
							sqlBoolean = ((SqlBoolean)sqlBoolean | sqlGeography.STIsEmpty());
						}
						x = ((SqlBoolean)x & (SqlBoolean)sqlBoolean);
					}
					if (!(SqlBoolean)x)
					{
						if (sqlGeometry != null)
						{
							sqlGeometry.MakeValid();
							if (MapCore.IsTypeInGeometry(typeof(Shape), sqlGeometry))
							{
								num++;
							}
							if (MapCore.IsTypeInGeometry(typeof(Path), sqlGeometry))
							{
								num2++;
							}
							if (MapCore.IsTypeInGeometry(typeof(Symbol), sqlGeometry))
							{
								num3++;
							}
						}
						else if (sqlGeography != null)
						{
							if (MapCore.IsTypeInGeography(typeof(Shape), sqlGeography))
							{
								num++;
							}
							if (MapCore.IsTypeInGeography(typeof(Path), sqlGeography))
							{
								num2++;
							}
							if (MapCore.IsTypeInGeography(typeof(Symbol), sqlGeography))
							{
								num3++;
							}
						}
					}
				}
			}
			if (num > 0 && num >= num2 && num >= num3)
			{
				return BasicMapElements.Shapes;
			}
			if (num2 > 0 && num2 >= num3)
			{
				return BasicMapElements.Paths;
			}
			if (num3 > 0)
			{
				return BasicMapElements.Symbols;
			}
            */
			return null;
		}

		internal void LoadFromSpatial(string connectionString, string sqlStatement, string nameColumn, string spatialColumn, BasicMapElements mapElementsToLoad, bool importAllData, string layer)
		{
			DataTable dataTable = null;
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				sqlConnection.Open();
				using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
				{
					using (SqlCommand selectCommand = new SqlCommand(sqlStatement, sqlConnection))
					{
						sqlDataAdapter.SelectCommand = selectCommand;
						DataSet dataSet = new DataSet();
						dataSet.Locale = CultureInfo.CurrentCulture;
						sqlDataAdapter.Fill(dataSet);
						dataTable = dataSet.Tables[0];
					}
				}
			}
			this.LoadFromSpatial(dataTable, nameColumn, spatialColumn, mapElementsToLoad, importAllData, layer);
		}

		internal void LoadFromSpatial(DataTable dataTable, string nameColumn, string spatialColumn, BasicMapElements mapElementsToLoad, bool importAllData, string layer)
		{
			if (!importAllData)
			{
				this.LoadFromSpatial(dataTable, nameColumn, spatialColumn, mapElementsToLoad, null, null, layer, string.Empty);
			}
			else
			{
				string[] array = new string[dataTable.Columns.Count];
				ColumnImportMode[] array2 = new ColumnImportMode[dataTable.Columns.Count];
				for (int i = 0; i < dataTable.Columns.Count; i++)
				{
					array[i] = dataTable.Columns[i].ColumnName;
					array2[i] = ColumnImportMode.FirstValue;
				}
				this.LoadFromSpatial(dataTable, nameColumn, spatialColumn, mapElementsToLoad, array, array2, layer, string.Empty);
			}
		}

		internal void LoadFromSpatial(DataTable dataTable, string nameColumn, string spatialColumn, BasicMapElements mapElementsToLoad, string[] additionalColumnsToImport, string layer)
		{
			ColumnImportMode[] array = new ColumnImportMode[additionalColumnsToImport.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = ColumnImportMode.FirstValue;
			}
			this.LoadFromSpatial(dataTable, nameColumn, spatialColumn, mapElementsToLoad, additionalColumnsToImport, array, layer, string.Empty);
		}

		internal void LoadFromSpatial(DataTable spatialTable, string nameColumn, string spatialColumn, BasicMapElements mapElementsToLoad, string[] columnsToImport, ColumnImportMode[] importModes, string layer, string category)
		{
			switch (mapElementsToLoad)
			{
			case BasicMapElements.Shapes:
				//this.CreateShapesFromSpatial(spatialTable, nameColumn, spatialColumn, columnsToImport, importModes, layer, category);
				break;
			case BasicMapElements.Paths:
				//this.CreatePathsFromSpatial(spatialTable, nameColumn, spatialColumn, columnsToImport, importModes, layer, category);
				break;
			case BasicMapElements.Symbols:
				//this.CreateSymbolsFromSpatial(spatialTable, nameColumn, spatialColumn, columnsToImport, layer, category);
				break;
			}
			this.InvalidateCachedBounds();
			this.InvalidateCachedPaths();
			this.InvalidateRules();
			this.InvalidateDataBinding();
		}
        /*
		private void CreateShapesFromSpatial(DataTable spatialTable, string nameColumn, string spatialColumn, string[] columnsToImport, ColumnImportMode[] importModes, string layer, string category)
		{
			if (columnsToImport != null)
			{
				foreach (string text in columnsToImport)
				{
					if (this.ShapeFields.GetByName(text) == null && text != spatialColumn)
					{
						Field field = this.ShapeFields.Add(text);
						field.Type = spatialTable.Columns[text].DataType;
					}
				}
			}
			foreach (DataRow row in spatialTable.Rows)
			{
				SqlGeometry sqlGeometry = row[spatialColumn] as SqlGeometry;
				if (sqlGeometry != null && sqlGeometry.IsNull)
				{
					continue;
				}
				SqlGeography sqlGeography = row[spatialColumn] as SqlGeography;
				if (sqlGeography != null && sqlGeography.IsNull)
				{
					continue;
				}
				if (sqlGeometry != null)
				{
					sqlGeometry = sqlGeometry.MakeValid();
					if (!sqlGeometry.STIsEmpty() && MapCore.IsTypeInGeometry(typeof(Shape), sqlGeometry))
					{
						goto IL_0119;
					}
					continue;
				}
				if (sqlGeography != null)
				{
					sqlGeography = sqlGeography.MakeValid();
					if (!sqlGeometry.STIsEmpty() && MapCore.IsTypeInGeography(typeof(Shape), sqlGeography))
					{
						goto IL_0119;
					}
					continue;
				}
				goto IL_0119;
				IL_0119:
				string name = "Shape" + (this.Shapes.Count + 1).ToString(CultureInfo.CurrentCulture);
				if (!string.IsNullOrEmpty(nameColumn))
				{
					try
					{
						name = row[nameColumn].ToString();
					}
					catch
					{
						name = "(null)";
					}
				}
				Shape shape = (Shape)this.Shapes.GetByName(name);
				bool flag = false;
				if (shape == null)
				{
					this.Shapes.SuppressAddedAndRemovedEvents = true;
					shape = this.Shapes.Add(name);
					flag = true;
				}
				if (columnsToImport != null)
				{
					foreach (string text2 in columnsToImport)
					{
						if (text2 != spatialColumn)
						{
							try
							{
								shape[text2] = row[text2];
							}
							catch
							{
							}
						}
					}
				}
				if (sqlGeometry != null)
				{
					shape.AddGeometry(sqlGeometry);
				}
				else if (sqlGeography != null)
				{
					shape.AddGeography(sqlGeography);
				}
				if (flag && (shape.ShapeData.Points == null || shape.ShapeData.Points.Length == 0))
				{
					this.Shapes.Remove(shape);
					if (this.Shapes.SuppressAddedAndRemovedEvents)
					{
						this.Shapes.SuppressAddedAndRemovedEvents = false;
					}
				}
				else
				{
					shape.Layer = layer;
					shape.Category = category;
					if (this.Shapes.SuppressAddedAndRemovedEvents)
					{
						this.Common.InvokeElementAdded(shape);
						this.Shapes.SuppressAddedAndRemovedEvents = false;
					}
				}
			}
		}

		private void CreatePathsFromSpatial(DataTable spatialTable, string nameColumn, string spatialColumn, string[] columnsToImport, ColumnImportMode[] importModes, string layer, string category)
		{
			if (columnsToImport != null)
			{
				foreach (string text in columnsToImport)
				{
					if (this.PathFields.GetByName(text) == null && text != spatialColumn)
					{
						Field field = this.PathFields.Add(text);
						field.Type = spatialTable.Columns[text].DataType;
					}
				}
			}
			foreach (DataRow row in spatialTable.Rows)
			{
				SqlGeometry sqlGeometry = row[spatialColumn] as SqlGeometry;
				if (sqlGeometry != null && sqlGeometry.IsNull)
				{
					continue;
				}
				SqlGeography sqlGeography = row[spatialColumn] as SqlGeography;
				if (sqlGeography != null && sqlGeography.IsNull)
				{
					continue;
				}
				if (sqlGeometry != null)
				{
					sqlGeometry = sqlGeometry.MakeValid();
					if (!sqlGeometry.STIsEmpty() && MapCore.IsTypeInGeometry(typeof(Path), sqlGeometry))
					{
						goto IL_0119;
					}
					continue;
				}
				if (sqlGeography != null)
				{
					sqlGeography = sqlGeography.MakeValid();
					if (!sqlGeometry.STIsEmpty() && MapCore.IsTypeInGeography(typeof(Path), sqlGeography))
					{
						goto IL_0119;
					}
					continue;
				}
				goto IL_0119;
				IL_0119:
				string name = "Path" + (this.Paths.Count + 1).ToString(CultureInfo.CurrentCulture);
				if (!string.IsNullOrEmpty(nameColumn))
				{
					try
					{
						name = row[nameColumn].ToString();
					}
					catch
					{
						name = "(null)";
					}
				}
				Path path = (Path)this.Paths.GetByName(name);
				bool flag = false;
				if (path == null)
				{
					this.Paths.SuppressAddedAndRemovedEvents = true;
					path = this.Paths.Add(name);
					flag = true;
				}
				if (columnsToImport != null)
				{
					foreach (string text2 in columnsToImport)
					{
						if (text2 != spatialColumn)
						{
							try
							{
								path[text2] = row[text2];
							}
							catch
							{
							}
						}
					}
				}
				if (sqlGeometry != null)
				{
					path.AddGeometry(sqlGeometry);
				}
				else if (sqlGeography != null)
				{
					path.AddGeography(sqlGeography);
				}
				if (flag && (path.PathData.Points == null || path.PathData.Points.Length == 0))
				{
					this.Paths.Remove(path);
					if (this.Paths.SuppressAddedAndRemovedEvents)
					{
						this.Paths.SuppressAddedAndRemovedEvents = false;
					}
				}
				else
				{
					path.Layer = layer;
					path.Category = category;
					if (this.Paths.SuppressAddedAndRemovedEvents)
					{
						this.Common.InvokeElementAdded(path);
						this.Paths.SuppressAddedAndRemovedEvents = false;
					}
				}
			}
		}

		private void CreateSymbolsFromSpatial(DataTable spatialTable, string nameColumn, string spatialColumn, string[] columnsToImport, string layer, string category)
		{
			if (columnsToImport != null)
			{
				foreach (string text in columnsToImport)
				{
					if (this.SymbolFields.GetByName(text) == null && text != spatialColumn)
					{
						Field field = this.SymbolFields.Add(text);
						field.Type = spatialTable.Columns[text].DataType;
					}
				}
			}
			foreach (DataRow row in spatialTable.Rows)
			{
				SqlGeometry sqlGeometry = row[spatialColumn] as SqlGeometry;
				if (sqlGeometry != null && sqlGeometry.IsNull)
				{
					continue;
				}
				SqlGeography sqlGeography = row[spatialColumn] as SqlGeography;
				if (sqlGeography != null && sqlGeography.IsNull)
				{
					continue;
				}
				if (sqlGeometry != null)
				{
					sqlGeometry = sqlGeometry.MakeValid();
					if (!sqlGeometry.STIsEmpty() && MapCore.IsTypeInGeometry(typeof(Symbol), sqlGeometry))
					{
						goto IL_0119;
					}
					continue;
				}
				if (sqlGeography != null)
				{
					sqlGeography = sqlGeography.MakeValid();
					if (!sqlGeometry.STIsEmpty() && MapCore.IsTypeInGeography(typeof(Symbol), sqlGeography))
					{
						goto IL_0119;
					}
					continue;
				}
				goto IL_0119;
				IL_0119:
				string name = "Symbol" + (this.Symbols.Count + 1).ToString(CultureInfo.CurrentCulture);
				if (!string.IsNullOrEmpty(nameColumn))
				{
					try
					{
						name = row[nameColumn].ToString();
					}
					catch
					{
						name = "(null)";
					}
				}
				Symbol symbol = (Symbol)this.Symbols.GetByName(name);
				bool flag = false;
				if (symbol == null)
				{
					this.Symbols.SuppressAddedAndRemovedEvents = true;
					symbol = this.Symbols.Add(name);
					flag = true;
				}
				if (columnsToImport != null)
				{
					foreach (string text2 in columnsToImport)
					{
						if (text2 != spatialColumn)
						{
							try
							{
								symbol[text2] = row[text2];
							}
							catch
							{
							}
						}
					}
				}
				bool flag2 = false;
				if (sqlGeometry != null)
				{
					flag2 = symbol.AddGeometry(sqlGeometry);
				}
				else if (sqlGeography != null)
				{
					flag2 = symbol.AddGeography(sqlGeography);
				}
				if (!flag2 && flag)
				{
					this.Symbols.Remove(symbol);
					if (this.Symbols.SuppressAddedAndRemovedEvents)
					{
						this.Symbols.SuppressAddedAndRemovedEvents = false;
					}
				}
				else
				{
					symbol.Layer = layer;
					symbol.Category = category;
					if (this.Symbols.SuppressAddedAndRemovedEvents)
					{
						this.Common.InvokeElementAdded(symbol);
						this.Symbols.SuppressAddedAndRemovedEvents = false;
					}
				}
			}
		}

		internal SqlGeography GetClippingPolygon(int srid)
		{
			double num = (this.Projection == Projection.Mercator) ? 85.05112878 : 89.0;
			if (srid != this.CurrentSrid || num != this.CurrentLatitudeLimit)
			{
				this.CurrentLatitudeLimit = num;
				this.CurrentSrid = srid;
				this.ConstructlippingPolygon();
			}
			return this.ClippingPolygon;
		}

		private void ConstructlippingPolygon()
		{
			double num = 1E-10 / Math.Cos(this.CurrentLatitudeLimit * 3.1415926535897931 / 180.0);
			double num2 = this.CurrentLatitudeLimit / 2.0;
			double num3 = 1E-10 / Math.Cos(num2 * 3.1415926535897931 / 180.0);
			SqlGeographyBuilder sqlGeographyBuilder = new SqlGeographyBuilder();
			sqlGeographyBuilder.SetSrid(this.CurrentSrid);
			sqlGeographyBuilder.BeginGeography(OpenGisGeographyType.CurvePolygon);
			sqlGeographyBuilder.BeginFigure(0.0 - this.CurrentLatitudeLimit, -180.0 + num);
			sqlGeographyBuilder.AddCircularArc(0.0 - this.CurrentLatitudeLimit, -90.0, 0.0 - this.CurrentLatitudeLimit, 0.0);
			sqlGeographyBuilder.AddCircularArc(0.0 - this.CurrentLatitudeLimit, 90.0, 0.0 - this.CurrentLatitudeLimit, 180.0 - num);
			sqlGeographyBuilder.AddLine(0.0 - num2, 180.0 - num3);
			sqlGeographyBuilder.AddLine(0.0, 179.9999999999);
			sqlGeographyBuilder.AddLine(num2, 180.0 - num3);
			sqlGeographyBuilder.AddLine(this.CurrentLatitudeLimit, 180.0 - num);
			sqlGeographyBuilder.AddCircularArc(this.CurrentLatitudeLimit, 90.0, this.CurrentLatitudeLimit, 0.0);
			sqlGeographyBuilder.AddCircularArc(this.CurrentLatitudeLimit, -90.0, this.CurrentLatitudeLimit, -180.0 + num);
			sqlGeographyBuilder.AddLine(num2, -180.0 + num3);
			sqlGeographyBuilder.AddLine(0.0, -179.9999999999);
			sqlGeographyBuilder.AddLine(0.0 - num2, -180.0 + num3);
			sqlGeographyBuilder.AddLine(0.0 - this.CurrentLatitudeLimit, -180.0 + num);
			sqlGeographyBuilder.EndFigure();
			sqlGeographyBuilder.EndGeography();
			this.ClippingPolygon = sqlGeographyBuilder.ConstructedGeography;
		}

		internal SqlGeography NormalizeLongitude(SqlGeography geography)
		{
			geography = geography.STCurveToLine();
			LongitudeNormalizer longitudeNormalizer = new LongitudeNormalizer();
			geography.Populate(longitudeNormalizer);
			return longitudeNormalizer.Result;
		}
        */
		internal SpatialLoadResult LoadFromShapeFileStreams(Stream shpStream, Stream dbfStream, string[] columnsToImport, string[] destinationFields, string layer, string category)
		{
			ShapeFileReader shapeFileReader = new ShapeFileReader();
			shapeFileReader.ShpStream = shpStream;
			shapeFileReader.DbfStream = dbfStream;
			shapeFileReader.Load();
			ColumnImportMode[] array = null;
			if (columnsToImport != null)
			{
				array = new ColumnImportMode[columnsToImport.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = ColumnImportMode.FirstValue;
				}
			}
			return this.LoadFromShapeReader(shapeFileReader, string.Empty, columnsToImport, destinationFields, array, layer, category);
		}

		internal SpatialLoadResult LoadFromShapeFileStreams(Stream shpStream, Stream dbfStream, string layer, string category)
		{
			ShapeFileReader shapeFileReader = new ShapeFileReader();
			shapeFileReader.ShpStream = shpStream;
			shapeFileReader.DbfStream = dbfStream;
			shapeFileReader.Load();
			string[] array = new string[shapeFileReader.Table.Columns.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = shapeFileReader.Table.Columns[i].ColumnName;
			}
			ColumnImportMode[] array2 = null;
			if (array != null)
			{
				array2 = new ColumnImportMode[array.Length];
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = ColumnImportMode.FirstValue;
				}
			}
			return this.LoadFromShapeReader(shapeFileReader, string.Empty, array, array, array2, layer, category);
		}

		internal void LoadFromShapeFile(string fileName, string nameColumn, bool importData)
		{
			ShapeFileReader shapeFileReader = new ShapeFileReader();
			shapeFileReader.FileName = fileName;
			shapeFileReader.Load();
			if (!importData)
			{
				this.LoadFromShapeReader(shapeFileReader, nameColumn, null, null);
			}
			else
			{
				string[] array = new string[shapeFileReader.Table.Columns.Count];
				ColumnImportMode[] array2 = new ColumnImportMode[shapeFileReader.Table.Columns.Count];
				for (int i = 0; i < shapeFileReader.Table.Columns.Count; i++)
				{
					array[i] = shapeFileReader.Table.Columns[i].ColumnName;
					array2[i] = ColumnImportMode.FirstValue;
				}
				this.LoadFromShapeReader(shapeFileReader, nameColumn, array, array2);
			}
		}

		internal void LoadFromShapeFile(string fileName, string nameColumn, string[] columnsToImport, ColumnImportMode[] importModes)
		{
			ShapeFileReader shapeFileReader = new ShapeFileReader();
			shapeFileReader.FileName = fileName;
			shapeFileReader.Load();
			this.LoadFromShapeReader(shapeFileReader, nameColumn, columnsToImport, importModes);
		}

		internal void LoadFromShapeReader(ShapeFileReader shapeReader, string nameColumn, string[] columnsToImport, ColumnImportMode[] importModes)
		{
			this.LoadFromShapeReader(shapeReader, nameColumn, columnsToImport, columnsToImport, importModes, string.Empty, string.Empty);
		}

		internal SpatialLoadResult LoadFromShapeReader(ShapeFileReader shapeReader, string nameColumn, string[] columnsToImport, string[] destinationFields, ColumnImportMode[] importModes, string layer, string category)
		{
			SpatialLoadResult result = SpatialLoadResult.AllSpatialElementsLoaded;
			if (shapeReader.ShapeType == ShapeType.Polygon)
			{
				result = this.CreateShapes(shapeReader, nameColumn, columnsToImport, destinationFields, importModes, layer, category);
			}
			else if (shapeReader.ShapeType == ShapeType.PolyLine)
			{
				result = this.CreatePaths(shapeReader, nameColumn, columnsToImport, destinationFields, importModes, layer, category);
			}
			else if (shapeReader.ShapeType == ShapeType.Point || shapeReader.ShapeType == ShapeType.MultiPoint)
			{
				result = this.CreateSymbols(shapeReader, nameColumn, columnsToImport, destinationFields, layer, category);
			}
			this.InvalidateCachedBounds();
			this.InvalidateCachedPaths();
			this.InvalidateRules();
			this.InvalidateDataBinding();
			return result;
		}

		private SpatialLoadResult CreateShapes(ShapeFileReader shapeReader, string nameColumn, string[] columnsToImport, string[] destinationFields, ColumnImportMode[] importModes, string layer, string category)
		{
			if (columnsToImport != null)
			{
				for (int i = 0; i < columnsToImport.Length; i++)
				{
					if (this.ShapeFields.GetByName(destinationFields[i]) == null)
					{
						Field field = this.ShapeFields.Add(destinationFields[i]);
						field.Type = shapeReader.Table.Columns[columnsToImport[i]].DataType;
					}
				}
			}
			int num = this.Shapes.Count;
			int num2 = 0;
			foreach (PolyLine polygon in shapeReader.Polygons)
			{
				if (this.GetSpatialElementCount() + 1 > this.MaxSpatialElementCount)
				{
					return SpatialLoadResult.MaxSpatialElementCountReached;
				}
				if (this.GetSpatialPointCount() + polygon.Points.Length > this.MaxSpatialPointCount)
				{
					return SpatialLoadResult.MaxSpatialPointCountReached;
				}
				int num3 = ++num;
				string name = "Shape" + num3.ToString(CultureInfo.CurrentCulture);
				if (!string.IsNullOrEmpty(nameColumn))
				{
					try
					{
						name = shapeReader.Table.Rows[num2][nameColumn].ToString();
					}
					catch
					{
						name = "(null)";
					}
				}
				Shape shape = (Shape)this.Shapes.GetByName(name);
				if (shape == null)
				{
					this.Shapes.SuppressAddedAndRemovedEvents = true;
					shape = this.Shapes.Add(name);
				}
				if (columnsToImport != null)
				{
					for (int j = 0; j < columnsToImport.Length; j++)
					{
						try
						{
							shape[destinationFields[j]] = shapeReader.Table.Rows[num2][columnsToImport[j]];
						}
						catch
						{
						}
					}
				}
				MapPoint[] array = new MapPoint[polygon.NumPoints];
				for (int k = 0; k < polygon.NumPoints; k++)
				{
					if (polygon.Points[k].Y < shapeReader.YMin)
					{
						polygon.Points[k].Y = shapeReader.YMin;
					}
					if (polygon.Points[k].Y > shapeReader.YMax)
					{
						polygon.Points[k].Y = shapeReader.YMax;
					}
					array[k].X = polygon.Points[k].X;
					array[k].Y = polygon.Points[k].Y;
				}
				ShapeSegment[] array2 = new ShapeSegment[polygon.NumParts];
				for (int l = 0; l < polygon.NumParts; l++)
				{
					array2[l].Type = SegmentType.Polygon;
					if (l + 1 < polygon.Parts.Length)
					{
						array2[l].Length = polygon.Parts[l + 1] - polygon.Parts[l];
					}
					else
					{
						array2[l].Length = polygon.NumPoints - polygon.Parts[l];
					}
				}
				shape.AddSegments(array, array2);
				shape.Layer = layer;
				shape.Category = category;
				if (this.Shapes.SuppressAddedAndRemovedEvents)
				{
					this.Common.InvokeElementAdded(shape);
					this.Shapes.SuppressAddedAndRemovedEvents = false;
				}
				num2++;
			}
			return SpatialLoadResult.AllSpatialElementsLoaded;
		}

		private SpatialLoadResult CreatePaths(ShapeFileReader shapeReader, string nameColumn, string[] columnsToImport, string[] destinationFields, ColumnImportMode[] importModes, string layer, string category)
		{
			if (columnsToImport != null)
			{
				for (int i = 0; i < columnsToImport.Length; i++)
				{
					if (this.PathFields.GetByName(destinationFields[i]) == null)
					{
						Field field = this.PathFields.Add(destinationFields[i]);
						field.Type = shapeReader.Table.Columns[columnsToImport[i]].DataType;
					}
				}
			}
			int num = this.Paths.Count;
			int num2 = 0;
			foreach (PolyLine polyLine in shapeReader.PolyLines)
			{
				if (this.GetSpatialElementCount() + 1 > this.MaxSpatialElementCount)
				{
					return SpatialLoadResult.MaxSpatialElementCountReached;
				}
				if (this.GetSpatialPointCount() + polyLine.Points.Length > this.MaxSpatialPointCount)
				{
					return SpatialLoadResult.MaxSpatialPointCountReached;
				}
				int num3 = ++num;
				string name = "Path" + num3.ToString(CultureInfo.CurrentCulture);
				if (!string.IsNullOrEmpty(nameColumn))
				{
					try
					{
						name = shapeReader.Table.Rows[num2][nameColumn].ToString();
					}
					catch
					{
						name = "(null)";
					}
				}
				Path path = (Path)this.Paths.GetByName(name);
				if (path == null)
				{
					this.Paths.SuppressAddedAndRemovedEvents = true;
					path = this.Paths.Add(name);
				}
				if (columnsToImport != null)
				{
					for (int j = 0; j < columnsToImport.Length; j++)
					{
						try
						{
							path[destinationFields[j]] = shapeReader.Table.Rows[num2][columnsToImport[j]];
						}
						catch
						{
						}
					}
				}
				MapPoint[] array = new MapPoint[polyLine.NumPoints];
				for (int k = 0; k < polyLine.NumPoints; k++)
				{
					if (polyLine.Points[k].Y < shapeReader.YMin)
					{
						polyLine.Points[k].Y = shapeReader.YMin;
					}
					if (polyLine.Points[k].Y > shapeReader.YMax)
					{
						polyLine.Points[k].Y = shapeReader.YMax;
					}
					array[k].X = polyLine.Points[k].X;
					array[k].Y = polyLine.Points[k].Y;
				}
				PathSegment[] array2 = new PathSegment[polyLine.NumParts];
				for (int l = 0; l < polyLine.NumParts; l++)
				{
					array2[l].Type = SegmentType.Polygon;
					if (l + 1 < polyLine.Parts.Length)
					{
						array2[l].Length = polyLine.Parts[l + 1] - polyLine.Parts[l];
					}
					else
					{
						array2[l].Length = polyLine.NumPoints - polyLine.Parts[l];
					}
				}
				path.AddSegments(array, array2);
				path.Layer = layer;
				path.Category = category;
				if (this.Paths.SuppressAddedAndRemovedEvents)
				{
					this.Common.InvokeElementAdded(path);
					this.Paths.SuppressAddedAndRemovedEvents = false;
				}
				num2++;
			}
			return SpatialLoadResult.AllSpatialElementsLoaded;
		}

		private SpatialLoadResult CreateSymbols(ShapeFileReader shapeReader, string nameColumn, string[] columnsToImport, string[] destinationFields, string layer, string category)
		{
			if (columnsToImport != null)
			{
				for (int i = 0; i < columnsToImport.Length; i++)
				{
					if (this.SymbolFields.GetByName(destinationFields[i]) == null)
					{
						Field field = this.SymbolFields.Add(destinationFields[i]);
						field.Type = shapeReader.Table.Columns[columnsToImport[i]].DataType;
					}
				}
			}
			int num = this.Symbols.Count;
			int num2 = 0;
			foreach (ShapePoint point in shapeReader.Points)
			{
				if (this.GetSpatialElementCount() + 1 > this.MaxSpatialElementCount)
				{
					return SpatialLoadResult.MaxSpatialElementCountReached;
				}
				if (this.GetSpatialPointCount() + 1 > this.MaxSpatialPointCount)
				{
					return SpatialLoadResult.MaxSpatialPointCountReached;
				}
				int num3 = ++num;
				string text = "Symbol" + num3.ToString(CultureInfo.CurrentCulture);
				if (!string.IsNullOrEmpty(nameColumn))
				{
					try
					{
						text = shapeReader.Table.Rows[num2][nameColumn].ToString();
					}
					catch
					{
						text = "(null)";
					}
				}
				Symbol symbol = null;
				this.Symbols.SuppressAddedAndRemovedEvents = true;
				int num4 = 0;
				while (symbol == null)
				{
					try
					{
						symbol = ((num4 != 0) ? this.Symbols.Add(text + num4.ToString(CultureInfo.CurrentCulture)) : this.Symbols.Add(text));
					}
					catch
					{
						num4++;
					}
				}
				if (columnsToImport != null)
				{
					for (int j = 0; j < columnsToImport.Length; j++)
					{
						try
						{
							symbol[destinationFields[j]] = shapeReader.Table.Rows[num2][columnsToImport[j]];
						}
						catch
						{
						}
					}
				}
				symbol.X = point.X;
				symbol.Y = point.Y;
				symbol.Layer = layer;
				symbol.Category = category;
				if (this.Symbols.SuppressAddedAndRemovedEvents)
				{
					this.Common.InvokeElementAdded(symbol);
					this.Symbols.SuppressAddedAndRemovedEvents = false;
				}
				num2++;
			}
			num2 = 0;
			foreach (MultiPoint multiPoint in shapeReader.MultiPoints)
			{
				if (this.GetSpatialElementCount() + 1 > this.MaxSpatialElementCount)
				{
					return SpatialLoadResult.MaxSpatialElementCountReached;
				}
				if (this.GetSpatialPointCount() + multiPoint.NumPoints > this.MaxSpatialPointCount)
				{
					return SpatialLoadResult.MaxSpatialPointCountReached;
				}
				string text2 = "Symbol" + num++.ToString(CultureInfo.CurrentCulture);
				if (!string.IsNullOrEmpty(nameColumn))
				{
					try
					{
						text2 = shapeReader.Table.Rows[num2][nameColumn].ToString();
					}
					catch
					{
						text2 = "(null)";
					}
				}
				Symbol symbol2 = null;
				this.Symbols.SuppressAddedAndRemovedEvents = true;
				int num6 = 0;
				while (symbol2 == null)
				{
					try
					{
						symbol2 = ((num6 != 0) ? this.Symbols.Add(text2 + num6.ToString(CultureInfo.CurrentCulture)) : this.Symbols.Add(text2));
					}
					catch
					{
						num6++;
					}
				}
				if (columnsToImport != null)
				{
					for (int k = 0; k < columnsToImport.Length; k++)
					{
						try
						{
							symbol2[destinationFields[k]] = shapeReader.Table.Rows[num2][columnsToImport[k]];
						}
						catch
						{
						}
					}
				}
				symbol2.X = multiPoint.Points[0].X;
				symbol2.Y = multiPoint.Points[0].Y;
				symbol2.Layer = layer;
				symbol2.Category = category;
				if (this.Symbols.SuppressAddedAndRemovedEvents)
				{
					this.Common.InvokeElementAdded(symbol2);
					this.Symbols.SuppressAddedAndRemovedEvents = false;
				}
				num2++;
			}
			return SpatialLoadResult.AllSpatialElementsLoaded;
		}

		public void Simplify(float factor)
		{
			double num = this.MaximumPoint.X - this.MinimumPoint.X;
			double resolution = num / 360.0 * (double)factor / 100.0 * 2.0;
			MapSimplifier mapSimplifier = new MapSimplifier();
			mapSimplifier.Simplify(this, resolution);
			this.InvalidateCachedPaths();
			this.InvalidateViewport();
		}

		internal void InvalidateCachedBounds()
		{
			if (!this.updatingCachedBounds)
			{
				this.cachedBoundsDirty = true;
			}
			this.InvalidateCachedPaths();
		}

		internal void UpdateCachedBounds()
		{
			if (this.cachedBoundsDirty)
			{
				this.updatingCachedBounds = true;
				this.cachedBoundsDirty = false;
				List<Shape> boundDeterminingShapes = this.GetBoundDeterminingShapes();
				List<Path> boundDeterminingPaths = this.GetBoundDeterminingPaths();
				List<Symbol> boundDeterminingSymbols = this.GetBoundDeterminingSymbols();
				double num = double.PositiveInfinity;
				double num2 = double.PositiveInfinity;
				double num3 = double.NegativeInfinity;
				double num4 = double.NegativeInfinity;
				foreach (Shape item in boundDeterminingShapes)
				{
					item.ShapeData.UpdateStoredParameters();
					num = Math.Min(num, item.ShapeData.MinimumExtent.X + item.OffsetInt.X);
					num2 = Math.Min(num2, item.ShapeData.MinimumExtent.Y + item.OffsetInt.Y);
					num3 = Math.Max(num3, item.ShapeData.MaximumExtent.X + item.OffsetInt.X);
					num4 = Math.Max(num4, item.ShapeData.MaximumExtent.Y + item.OffsetInt.Y);
				}
				foreach (Path item2 in boundDeterminingPaths)
				{
					item2.PathData.UpdateStoredParameters();
					num = Math.Min(num, item2.PathData.MinimumExtent.X + item2.OffsetInt.X);
					num2 = Math.Min(num2, item2.PathData.MinimumExtent.Y + item2.OffsetInt.Y);
					num3 = Math.Max(num3, item2.PathData.MaximumExtent.X + item2.OffsetInt.X);
					num4 = Math.Max(num4, item2.PathData.MaximumExtent.Y + item2.OffsetInt.Y);
				}
				foreach (Symbol item3 in boundDeterminingSymbols)
				{
					item3.SymbolData.UpdateStoredParameters();
					num = Math.Min(num, item3.SymbolData.MinimumExtent.X + item3.Offset.X);
					num2 = Math.Min(num2, item3.SymbolData.MinimumExtent.Y + item3.Offset.Y);
					num3 = Math.Max(num3, item3.SymbolData.MaximumExtent.X + item3.Offset.X);
					num4 = Math.Max(num4, item3.SymbolData.MaximumExtent.Y + item3.Offset.Y);
				}
				if (num == double.PositiveInfinity || num2 == double.PositiveInfinity || num3 == double.NegativeInfinity || num4 == double.NegativeInfinity)
				{
					MapBounds defaultEmptyBounds = this.GetDefaultEmptyBounds();
					this.MinimumPoint = defaultEmptyBounds.MinimumPoint;
					this.MaximumPoint = defaultEmptyBounds.MaximumPoint;
					this.MapCenterPoint = new MapPoint(0.0, 0.0);
				}
				else
				{
					double num5 = num3 - num;
					if (num5 < 1E-14)
					{
						num5 = ((!(Math.Abs(num3) < 1E-14)) ? (Math.Abs(num3) / 10.0) : 2.0);
						num -= num5 / 2.0;
						num3 += num5 / 2.0;
					}
					double num6 = num4 - num2;
					if (num6 < 1E-14)
					{
						num6 = ((!(Math.Abs(num4) < 1E-14)) ? (Math.Abs(num4) / 10.0) : 2.0);
						num2 -= num6 / 2.0;
						num4 += num6 / 2.0;
					}
					double latitudeLimit = this.GetLatitudeLimit();
					if (this.GeographyMode)
					{
						if (num4 < 0.0 - latitudeLimit)
						{
							num4 = 0.0 - latitudeLimit + num5;
							num6 = num4 - num2;
						}
						else if (num2 > latitudeLimit)
						{
							num2 = latitudeLimit - num5;
							num6 = num4 - num2;
						}
					}
					MapBounds boundsAfterProjection = this.GetBoundsAfterProjection(num, num2, num3, num4);
					double num7 = boundsAfterProjection.MaximumPoint.X - boundsAfterProjection.MinimumPoint.X;
					double num8 = boundsAfterProjection.MaximumPoint.Y - boundsAfterProjection.MinimumPoint.Y;
					if (num5 > num6 * 3.0 && num7 > num8 * 3.0)
					{
						double num9 = num6;
						num6 = num5 / 3.0;
						double num10 = (num6 - num9) / 2.0;
						num2 -= num10;
						num4 += num10;
					}
					else if (num6 > num5 * 3.0 && num8 > num7 * 3.0)
					{
						double num11 = num5;
						num5 = num6 / 3.0;
						double num12 = (num5 - num11) / 2.0;
						num -= num12;
						num3 += num12;
					}
					double num13 = num5 * 0.1;
					double num14 = num6 * 0.1;
					num -= num13;
					num2 -= num14;
					num3 += num13;
					num4 += num14;
					if (this.GeographyMode)
					{
						if (num2 < -89.0 && num2 > -90.1 && num4 < 90.0 && num4 > 81.0)
						{
							num4 = 0.0 - num2;
						}
						num = Math.Max(num, -180.0);
						num2 = Math.Max(num2, 0.0 - latitudeLimit);
						num3 = Math.Min(num3, 180.0);
						num4 = Math.Min(num4, latitudeLimit);
					}
					this.MinimumPoint = new MapPoint(num, num2);
					this.MaximumPoint = new MapPoint(num3, num4);
					this.MapCenterPoint = new MapPoint((this.MaximumPoint.X + this.MinimumPoint.X) / 2.0, (this.MaximumPoint.Y + this.MinimumPoint.Y) / 2.0);
				}
				this.updatingCachedBounds = false;
			}
		}

		internal MapBounds DetermineSpatialElementsBounds()
		{
			new MapBounds(new MapPoint(double.PositiveInfinity, double.PositiveInfinity), new MapPoint(double.NegativeInfinity, double.NegativeInfinity));
			List<Shape> boundDeterminingShapes = this.GetBoundDeterminingShapes();
			List<Path> boundDeterminingPaths = this.GetBoundDeterminingPaths();
			List<Symbol> boundDeterminingSymbols = this.GetBoundDeterminingSymbols();
			double num = double.PositiveInfinity;
			double num2 = double.PositiveInfinity;
			double num3 = double.NegativeInfinity;
			double num4 = double.NegativeInfinity;
			foreach (Shape item in boundDeterminingShapes)
			{
				item.ShapeData.UpdateStoredParameters();
				num = Math.Min(num, item.ShapeData.MinimumExtent.X + item.OffsetInt.X);
				num2 = Math.Min(num2, item.ShapeData.MinimumExtent.Y + item.OffsetInt.Y);
				num3 = Math.Max(num3, item.ShapeData.MaximumExtent.X + item.OffsetInt.X);
				num4 = Math.Max(num4, item.ShapeData.MaximumExtent.Y + item.OffsetInt.Y);
			}
			foreach (Path item2 in boundDeterminingPaths)
			{
				item2.PathData.UpdateStoredParameters();
				num = Math.Min(num, item2.PathData.MinimumExtent.X + item2.OffsetInt.X);
				num2 = Math.Min(num2, item2.PathData.MinimumExtent.Y + item2.OffsetInt.Y);
				num3 = Math.Max(num3, item2.PathData.MaximumExtent.X + item2.OffsetInt.X);
				num4 = Math.Max(num4, item2.PathData.MaximumExtent.Y + item2.OffsetInt.Y);
			}
			foreach (Symbol item3 in boundDeterminingSymbols)
			{
				item3.SymbolData.UpdateStoredParameters();
				num = Math.Min(num, item3.SymbolData.MinimumExtent.X + item3.Offset.X);
				num2 = Math.Min(num2, item3.SymbolData.MinimumExtent.Y + item3.Offset.Y);
				num3 = Math.Max(num3, item3.SymbolData.MaximumExtent.X + item3.Offset.X);
				num4 = Math.Max(num4, item3.SymbolData.MaximumExtent.Y + item3.Offset.Y);
			}
			if (num != double.PositiveInfinity && num2 != double.PositiveInfinity && num3 != double.NegativeInfinity && num4 != double.NegativeInfinity)
			{
				return new MapBounds(new MapPoint(num, num2), new MapPoint(num3, num4));
			}
			return null;
		}

		private List<Shape> GetBoundDeterminingShapes()
		{
			List<Shape> list = new List<Shape>();
			foreach (Shape shape in this.Shapes)
			{
				ILayerElement layerElement = shape;
				if ((layerElement.BelongsToAllLayers || !layerElement.BelongsToLayer || (layerElement.BelongsToLayer && (this.Layers[layerElement.Layer].Visibility == LayerVisibility.Shown || this.Layers[layerElement.Layer].Visibility == LayerVisibility.ZoomBased))) && shape.ShapeData.Points != null && shape.ShapeData.Points.Length > 0)
				{
					list.Add(shape);
				}
			}
			return list;
		}

		private List<Path> GetBoundDeterminingPaths()
		{
			List<Path> list = new List<Path>();
			foreach (Path path in this.Paths)
			{
				ILayerElement layerElement = path;
				if ((layerElement.BelongsToAllLayers || !layerElement.BelongsToLayer || (layerElement.BelongsToLayer && (this.Layers[layerElement.Layer].Visibility == LayerVisibility.Shown || this.Layers[layerElement.Layer].Visibility == LayerVisibility.ZoomBased))) && path.PathData.Points != null && path.PathData.Points.Length > 0)
				{
					list.Add(path);
				}
			}
			return list;
		}

		private List<Symbol> GetBoundDeterminingSymbols()
		{
			List<Symbol> list = new List<Symbol>();
			if (this.GeographyMode && this.AutoLimitsIgnoreSymbols)
			{
				return list;
			}
			foreach (Symbol symbol in this.Symbols)
			{
				ILayerElement layerElement = symbol;
				if ((layerElement.BelongsToAllLayers || !layerElement.BelongsToLayer || (layerElement.BelongsToLayer && (this.Layers[layerElement.Layer].Visibility == LayerVisibility.Shown || this.Layers[layerElement.Layer].Visibility == LayerVisibility.ZoomBased))) && symbol.ParentShape == "(none)" && symbol.SymbolData.Points != null && symbol.SymbolData.Points.Length > 0)
				{
					list.Add(symbol);
				}
			}
			return list;
		}

		private double GetLatitudeLimit()
		{
			if (this.Projection == Projection.Equirectangular)
			{
				return 90.0;
			}
			if (this.Projection == Projection.Mercator)
			{
				return 85.05112878;
			}
			return 89.0;
		}

		private MapBounds GetDefaultEmptyBounds()
		{
			MapPoint mapPoint;
			MapPoint mapPoint2;
			if (this.GeographyMode)
			{
				double latitudeLimit = this.GetLatitudeLimit();
				mapPoint = new MapPoint(-180.0, 0.0 - latitudeLimit);
				mapPoint2 = new MapPoint(180.0, latitudeLimit);
			}
			else
			{
				mapPoint = new MapPoint(-10.0, -10.0);
				mapPoint2 = new MapPoint(10.0, 10.0);
			}
			return new MapBounds(mapPoint, mapPoint2);
		}

		private string[,] DetermineTileUrls(Layer layer, int levelOfDetail, int xStartIndex, int yStartIndex, int horizontalCount, int verticalCount)
		{
			string[,] array = new string[horizontalCount, verticalCount];
			string text = layer.TileImageUriFormat.Replace("{subdomain}", "{0}");
			text = text.Replace("{quadkey}", "{1}");
			text = text.Replace("{culture}", "{2}");
			text = text.Replace("{token}", "{3}");
			for (int i = 0; i < verticalCount; i++)
			{
				for (int j = 0; j < horizontalCount; j++)
				{
					string text2 = VirtualEarthTileSystem.TileXYToQuadKey(j + xStartIndex, i + yStartIndex, levelOfDetail);
					int num = int.Parse(text2[text2.Length - 1].ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
					string text3 = layer.TileImageUriSubdomains[num % layer.TileImageUriSubdomains.Length];
					string[,] array2 = array;
					int num2 = j;
					int num3 = i;
					string text4 = string.Format(CultureInfo.InvariantCulture, text, text3, text2, this.TileCulture.IetfLanguageTag, this.TileServerAppId);
					array2[num2, num3] = text4;
				}
			}
			return array;
		}

		private Rectangle[,] DetermineTileRectangles(int horizontalCount, int verticalCount, int xOffset, int yOffset, int stretchedTileLength)
		{
			Rectangle[,] array = new Rectangle[horizontalCount, verticalCount];
			for (int i = 0; i < verticalCount; i++)
			{
				for (int j = 0; j < horizontalCount; j++)
				{
					array[j, i] = new Rectangle(xOffset + j * stretchedTileLength, yOffset + i * stretchedTileLength, stretchedTileLength, stretchedTileLength);
				}
			}
			return array;
		}

		private Image[,] LoadTileImages(string[,] tileImageUrls, Rectangle[,] tileRectangles, Layer layer)
		{
			int num = tileImageUrls.GetUpperBound(0) + 1;
			int num2 = tileImageUrls.GetUpperBound(1) + 1;
			Image[,] array = new Image[num, num2];
			this.openTileRequestCount = 0L;
			this.requestsCompletedEvent.Reset();
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					if (this.TileImagesCache.Contains(tileImageUrls[j, i]))
					{
						Image[,] array2 = array;
						int num3 = j;
						int num4 = i;
						Image obj = (Image)this.TileImagesCache[tileImageUrls[j, i]];
						array2[num3, num4] = obj;
					}
					else
					{
						if (this.Viewport.LoadTilesAsynchronously)
						{
							Image[,] array3 = array;
							int num5 = j;
							int num6 = i;
							Image image = this.TileWaitImage;
							array3[num5, num6] = image;
							lock (this.TileImagesCache)
							{
								this.TileImagesCache[tileImageUrls[j, i]] = this.TileWaitImage;
							}
						}
						else
						{
							Interlocked.Increment(ref this.openTileRequestCount);
						}
						try
						{
							HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(tileImageUrls[j, i]);
							httpWebRequest.KeepAlive = true;
							httpWebRequest.CachePolicy = new RequestCachePolicy(this.TileCacheLevel);
							TileRequestState state = new TileRequestState(httpWebRequest, tileImageUrls[j, i], array, j, i, tileRectangles[j, i], layer, this);
							IAsyncResult asyncResult = httpWebRequest.BeginGetResponse(this.WebResponseCallback, state);
							ThreadPool.RegisterWaitForSingleObject(asyncResult.AsyncWaitHandle, this.TimeoutCallback, state, this.TileServerTimeout, true);
						}
						catch
						{
							if (!this.Viewport.LoadTilesAsynchronously)
							{
								Interlocked.Decrement(ref this.openTileRequestCount);
							}
						}
					}
				}
			}
			if (!this.Viewport.LoadTilesAsynchronously)
			{
				while (Interlocked.Read(ref this.openTileRequestCount) > 0)
				{
					this.requestsCompletedEvent.WaitOne(10, false);
				}
			}
			return array;
		}

		private void TimeoutCallback(object state, bool timedOut)
		{
			if (timedOut)
			{
				TileRequestState tileRequestState = (TileRequestState)state;
				tileRequestState.Timeout = true;
				tileRequestState.Request.Abort();
			}
		}

		private void WebResponseCallback(IAsyncResult asyncResult)
		{
			TileRequestState tileRequestState = (TileRequestState)asyncResult.AsyncState;
			Image image = null;
			try
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)tileRequestState.Request.EndGetResponse(asyncResult);
				using (Stream stream = httpWebResponse.GetResponseStream())
				{
					MemoryStream memoryStream = new MemoryStream();
					byte[] buffer = new byte[4096];
					int count;
					while ((count = stream.Read(buffer, 0, 4096)) > 0)
					{
						memoryStream.Write(buffer, 0, count);
					}
					image = Image.FromStream(memoryStream);
					Utils.SetImageCustomProperty(image, CustomPropertyTag.ImageProviders, tileRequestState.Layer.GetAttributionStrings());
				}
				httpWebResponse.Close();
			}
			catch (Exception ex)
			{
				image = new Bitmap(256, 256);
				if (tileRequestState.Timeout)
				{
					Utils.SetImageCustomProperty(image, CustomPropertyTag.ImageError, SR.TileServerRequestTimeout(tileRequestState.MapCore.TileServerTimeout));
				}
				else
				{
					Utils.SetImageCustomProperty(image, CustomPropertyTag.ImageError, ex.Message);
				}
			}
			lock (tileRequestState.TileImages)
			{
				tileRequestState.TileImages[tileRequestState.X, tileRequestState.Y] = image;
			}
			if (tileRequestState.MapCore.Viewport.LoadTilesAsynchronously)
			{
				lock (tileRequestState.MapCore.TileImagesCache)
				{
					this.TileImagesCache[tileRequestState.Url] = image;
				}
				if (this.UseGridSectionRendering())
				{
					this.InvalidateGridSections(tileRequestState.Rectangle);
				}
				PointF contentOffsetInPixels = tileRequestState.MapCore.Viewport.GetContentOffsetInPixels();
				tileRequestState.Rectangle.Offset((int)(contentOffsetInPixels.X + (float)this.Viewport.Margins.Left), (int)(contentOffsetInPixels.Y + (float)this.Viewport.Margins.Top));
				if (tileRequestState.Rectangle.Width != 256)
				{
					tileRequestState.Rectangle.Inflate(3, 3);
				}
			}
			if (!tileRequestState.MapCore.Viewport.LoadTilesAsynchronously)
			{
				if (Interlocked.Read(ref this.openTileRequestCount) > 0)
				{
					Interlocked.Decrement(ref this.openTileRequestCount);
				}
				if (Interlocked.Read(ref this.openTileRequestCount) == 0)
				{
					this.requestsCompletedEvent.Set();
				}
			}
		}

		public void SetUserLocales(string[] userLocales)
		{
			this.userLocales = userLocales;
		}

		private bool IsLocaleAllowedToAccessBing()
		{
			if (this.userLocales != null)
			{
				string[] array = this.userLocales;
				foreach (string key in array)
				{
					if (MapCore._geoPoliticalBlockHashtableLocales.ContainsKey(key))
					{
						return false;
					}
				}
				return true;
			}
			return !MapCore._geoPoliticalBlockHashtableLocales.ContainsKey(CultureInfo.CurrentCulture.Name);
		}

		private void RenderTiles(MapGraphics g, Layer layer, RectangleF clipRect)
		{
			if (!this.IsLocaleAllowedToAccessBing())
			{
				MapCore.RenderTileImageError(g.Graphics, SR.Map_BackgroundNotAvailable, Rectangle.Round(clipRect));
			}
			else
			{
				if (!layer.IsVirtualEarthServiceQueried())
				{
					if (this.Viewport.LoadTilesAsynchronously || (this.Viewport.QueryVirtualEarthAsynchronously && string.IsNullOrEmpty(layer.TileError)))
					{
						layer.QueryVirtualEarthService(true);
						return;
					}
					if (!layer.QueryVirtualEarthService(false))
					{
						return;
					}
				}
				PointF location = this.GeographicToContent(new MapPoint(-180.0, 90.0));
				PointF pointF = this.GeographicToContent(new MapPoint(180.0, -90.0));
				RectangleF rect = new RectangleF(location, new SizeF(pointF.X - location.X, pointF.Y - location.Y));
				clipRect.Intersect(rect);
				if (!clipRect.IsEmpty)
				{
					GraphicsState gstate = g.Save();
					using (GraphicsPath graphicsPath = new GraphicsPath())
					{
						graphicsPath.AddRectangle(clipRect);
						g.SetClip(graphicsPath, CombineMode.Replace);
					}
					double num = VirtualEarthTileSystem.LevelOfDetail(this.Viewport.GetGroundResolutionAtEquator());
					int num2 = Math.Max((int)num, 1);
					double num3 = Math.Pow(2.0, num - (double)num2);
					double num4 = 256.0 * num3;
					int num5 = (int)Math.Round(num4);
					if (num5 == 512)
					{
						num4 = 256.0;
						num5 = 256;
						num3 = 1.0;
						num += 1.0;
						num2++;
					}
					double num6 = default(double);
					double num7 = default(double);
					VirtualEarthTileSystem.LongLatToPixelXY(this.MinimumPoint.X, this.MaximumPoint.Y, num, out num6, out num7);
					num6 = Math.Round(num6);
					num7 = Math.Round(num7);
					double num8 = num6 + (double)clipRect.X;
					double num9 = num7 + (double)clipRect.Y;
					int num10 = (int)Math.Max(num8 / num4, 0.0);
					int num11 = (int)Math.Max(num9 / num4, 0.0);
					double num12 = (double)num10 * num4;
					double num13 = (double)num11 * num4;
					float num14 = (float)(num12 - num8) + clipRect.X;
					float num15 = (float)(num13 - num9) + clipRect.Y;
					int num16 = (int)Math.Ceiling((double)(clipRect.Width + clipRect.X - num14) / num4);
					int num17 = (int)Math.Ceiling((double)(clipRect.Height + clipRect.Y - num15) / num4);
					float num18 = 1f;
					if (layer.Transparency > 0.0)
					{
						num18 = (float)((100.0 - layer.Transparency) / 100.0);
					}
					ImageAttributes imageAttributes = null;
					if (num18 < 1.0)
					{
						ColorMatrix colorMatrix = new ColorMatrix();
						colorMatrix.Matrix33 = num18;
						imageAttributes = new ImageAttributes();
						imageAttributes.SetColorMatrix(colorMatrix);
					}
					Bitmap bitmap = null;
					Graphics graphics = null;
					if (num5 > 256)
					{
						bitmap = new Bitmap(256 * num16, 256 * num17);
						graphics = Graphics.FromImage(bitmap);
					}
					string[,] array = this.DetermineTileUrls(layer, num2, num10, num11, num16, num17);
					Rectangle[,] tileRectangles = this.DetermineTileRectangles(num16, num17, (int)num14, (int)num15, num5);
					Image[,] array2 = null;
					if (this.LoadTilesHandler != null)
					{
						array2 = this.LoadTilesHandler(layer, array);
					}
					if (array2 == null)
					{
						array2 = this.LoadTileImages(array, tileRectangles, layer);
					}
					if (this.SaveTilesHandler != null)
					{
						this.SaveTilesHandler(layer, array, array2);
					}
					if (!this.Viewport.LoadTilesAsynchronously)
					{
						for (int i = 0; i < num17; i++)
						{
							for (int j = 0; j < num16; j++)
							{
								this.TileImagesCache[array[j, i]] = array2[j, i];
							}
						}
					}
					for (int k = 0; k < num17; k++)
					{
						for (int l = 0; l < num16; l++)
						{
							Image image = array2[l, k];
							if (image != null)
							{
								if (graphics != null)
								{
									Rectangle rect2 = new Rectangle(l * 256, k * 256, 256, 256);
									graphics.DrawImageUnscaledAndClipped(image, rect2);
								}
								else
								{
									Rectangle rectangle = new Rectangle(l * num5, k * num5, num5, num5);
									rectangle.Offset((int)num14, (int)num15);
									if (imageAttributes != null)
									{
										g.Graphics.DrawImage(image, rectangle, 0, 0, 256, 256, GraphicsUnit.Pixel, imageAttributes);
									}
									else if (num5 < 256)
									{
										g.Graphics.DrawImage(image, rectangle);
									}
									else
									{
										g.Graphics.DrawImageUnscaledAndClipped(image, rectangle);
									}
								}
							}
						}
					}
					if (bitmap != null)
					{
						Rectangle rectangle2 = new Rectangle((int)num14, (int)num15, num16 * num5, num17 * num5);
						if (imageAttributes != null)
						{
							g.Graphics.DrawImage(bitmap, rectangle2, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imageAttributes);
						}
						else
						{
							g.Graphics.DrawImageUnscaledAndClipped(bitmap, rectangle2);
						}
					}
					for (int m = 0; m < num17; m++)
					{
						for (int n = 0; n < num16; n++)
						{
							if (array2[n, m] != null)
							{
								string imageCustomProperty = Utils.GetImageCustomProperty(array2[n, m], CustomPropertyTag.ImageError);
								if (!string.IsNullOrEmpty(imageCustomProperty))
								{
									Rectangle rect3 = new Rectangle(n * num5, m * num5, num5, num5);
									rect3.Offset((int)num14, (int)num15);
									MapCore.RenderTileImageError(g.Graphics, imageCustomProperty, rect3);
								}
							}
						}
					}
					g.Restore(gstate);
				}
			}
		}

		internal void Paint(Graphics g)
		{
			this.Paint(g, RenderingType.Gdi, null, false);
		}

		internal void Paint(Graphics gdiGraph, RenderingType renderingType, Stream stream, bool buffered)
		{
			if (!this.skipPaint)
			{
				this.disableInvalidate = true;
				MapGraphics mapGraphics = null;
				try
				{
					if (this.AutoUpdates)
					{
						this.AutoDataBind(false);
						this.UpdateCachedBounds();
						this.ApplyAllRules();
						this.ResetCachedPaths();
						this.ResetChildSymbols();
					}
					mapGraphics = this.GetGraphics(renderingType, gdiGraph, stream);
					this.LayoutPanels(mapGraphics);
					if (this.RenderingMode == RenderingMode.GridSections)
					{
						this.RenderOneGridSection(mapGraphics, this.SingleGridSectionX, this.SingleGridSectionY);
					}
					else if (this.RenderingMode == RenderingMode.SinglePanel || this.RenderingMode == RenderingMode.ZoomThumb)
					{
						this.RenderOnePanel(mapGraphics);
					}
					else if (this.RenderingMode == RenderingMode.Background)
					{
						this.RenderFrame(mapGraphics);
					}
					else if (buffered)
					{
						this.RenderElementsBufered(mapGraphics);
					}
					else
					{
						this.RenderElements(mapGraphics);
					}
				}
				finally
				{
					this.disableInvalidate = false;
					if (mapGraphics != null)
					{
						mapGraphics.Close();
					}
				}
				this.dirtyFlag = false;
			}
		}

		internal void PrintPaint(Graphics g, Rectangle position)
		{
			this.Notify(MessageType.PrepareSnapShot, this, null);
			GraphicsState gstate = g.Save();
			try
			{
				this.isPrinting = true;
				this.Width = position.Width;
				this.Height = position.Height;
				g.TranslateTransform((float)position.X, (float)position.Y);
				this.Paint(g, RenderingType.Gdi, null, false);
			}
			finally
			{
				g.Restore(gstate);
				this.isPrinting = false;
			}
		}

		internal BufferBitmap InitBitmap(BufferBitmap bmp)
		{
			if (bmp == null)
			{
				bmp = new BufferBitmap();
			}
			else if (this.dirtyFlag)
			{
				bmp.Invalidate();
			}
			if (this.RenderingMode == RenderingMode.GridSections)
			{
				bmp.Size = new Size(this.GridSectionSize.Width, this.GridSectionSize.Height);
			}
			else if (this.RenderingMode == RenderingMode.SinglePanel || this.RenderingMode == RenderingMode.ZoomThumb)
			{
				SizeF absoluteSize = this.PanelToRender.GetAbsoluteSize();
				bmp.Size = new Size((int)Math.Round((double)absoluteSize.Width), (int)Math.Round((double)absoluteSize.Height));
			}
			else
			{
				bmp.Size = new Size(this.GetWidth(), this.GetHeight());
			}
			bmp.Graphics.SmoothingMode = this.GetSmootingMode();
			bmp.Graphics.TextRenderingHint = this.GetTextRenderingHint();
			bmp.Graphics.TextContrast = 2;
			return bmp;
		}

		private SmoothingMode GetSmootingMode()
		{
			if ((this.MapControl.AntiAliasing & AntiAliasing.Graphics) == AntiAliasing.Graphics)
			{
				return SmoothingMode.HighQuality;
			}
			return SmoothingMode.HighSpeed;
		}

		private TextRenderingHint GetTextRenderingHint()
		{
			TextRenderingHint textRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
			if ((this.MapControl.AntiAliasing & AntiAliasing.Text) == AntiAliasing.Text)
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

		internal MapGraphics GetGraphics(RenderingType renderingType, Graphics g, Stream outputStream)
		{
			MapGraphics mapGraphics = new MapGraphics(this.Common);
			this.Common.Height = this.GetHeight();
			this.Common.Width = this.GetWidth();
			mapGraphics.SetPictureSize(this.Common.Width, this.Common.Height);
			mapGraphics.ActiveRenderingType = renderingType;
			mapGraphics.Graphics = g;
			mapGraphics.AntiAliasing = this.Common.MapCore.AntiAliasing;
			mapGraphics.SmoothingMode = this.GetSmootingMode();
			mapGraphics.TextRenderingHint = this.GetTextRenderingHint();
			return mapGraphics;
		}

		internal Panel[] GetSortedPanels()
		{
			if (this.sortedPanels == null || this.dirtyFlag)
			{
				ArrayList arrayList = new ArrayList();
				arrayList.Add(this.Viewport);
				arrayList.AddRange(this.Labels);
				arrayList.AddRange(this.Legends);
				arrayList.AddRange(this.Images);
				arrayList.Add(this.ColorSwatchPanel);
				arrayList.Add(this.DistanceScalePanel);
				arrayList.Add(this.NavigationPanel);
				arrayList.Add(this.ZoomPanel);
				arrayList.Sort(new ZOrderSort(arrayList));
				this.sortedPanels = (Panel[])arrayList.ToArray(typeof(Panel));
			}
			return this.sortedPanels;
		}

		private string GetTileLayerAttributions()
		{
			Hashtable hashtable = new Hashtable();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Layer layer in this.Layers)
			{
				if (layer.Visible && layer.TileSystem != 0)
				{
					string[] array = layer.GetAttributionStrings().Split('|');
					foreach (string text in array)
					{
						if (!hashtable.ContainsKey(text))
						{
							hashtable.Add(text, null);
							stringBuilder.Append(text);
							stringBuilder.Append(", ");
						}
					}
				}
			}
			return stringBuilder.ToString().Trim(',', ' ');
		}

		private void RenderTileLayerAttributions(MapGraphics g)
		{
			string tileLayerAttributions = this.GetTileLayerAttributions();
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Far;
			RectangleF layoutRectangle = new RectangleF(this.Viewport.GetAbsoluteLocation(), this.Viewport.GetAbsoluteSize());
			using (Font font = new Font("Tahoma", 7f))
			{
				layoutRectangle.Offset(1f, 1f);
				g.DrawStringAbs(tileLayerAttributions, font, Brushes.Black, layoutRectangle, stringFormat);
				layoutRectangle.Offset(-1f, -1f);
				using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.White)))
				{
					g.DrawStringAbs(tileLayerAttributions, font, brush, layoutRectangle, stringFormat);
				}
			}
		}

		private string GetTileLayerError()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Layer layer in this.Layers)
			{
				if (layer.Visible && layer.TileSystem != 0)
				{
					lock (layer.TileError)
					{
						stringBuilder.Append(layer.TileError + "\n");
					}
				}
			}
			return stringBuilder.ToString().Trim();
		}

		internal static void RenderTileImageError(Graphics graphics, string error, Rectangle rect)
		{
			rect.Width++;
			rect.Height++;
			graphics.FillRectangle(Brushes.White, rect);
			rect.Inflate(-10, -10);
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			using (Font font = new Font("Tahoma", 10f))
			{
				graphics.MeasureString(error, font, rect.Width, stringFormat);
				graphics.DrawString(error, font, Brushes.Black, rect, stringFormat);
			}
		}

		internal void RenderErrorMessage(MapGraphics g)
		{
			string tileLayerError = this.GetTileLayerError();
			if (string.IsNullOrEmpty(this.Viewport.ErrorMessage) && string.IsNullOrEmpty(tileLayerError))
			{
				return;
			}
			string text = this.Viewport.ErrorMessage + "\n" + tileLayerError;
			text = text.Trim();
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Near;
			stringFormat.LineAlignment = StringAlignment.Near;
			RectangleF layoutRectangle = new RectangleF(this.Viewport.GetAbsoluteLocation(), this.Viewport.GetAbsoluteSize());
			using (Font font = new Font("Tahoma", 10f))
			{
				layoutRectangle.Offset(1f, 1f);
				g.DrawStringAbs(text, font, Brushes.Black, layoutRectangle, stringFormat);
				layoutRectangle.Offset(-1f, -1f);
				using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.White)))
				{
					g.DrawStringAbs(text, font, brush, layoutRectangle, stringFormat);
				}
			}
		}

		private bool IsGridSectionsArraySquare()
		{
			int num = 9999999;
			int num2 = -9999999;
			int num3 = 9999999;
			int num4 = -9999999;
			Point[] array = this.GridSectionsArray;
			for (int i = 0; i < array.Length; i++)
			{
				Point point = array[i];
				num = Math.Min(num, point.X);
				num2 = Math.Max(num2, point.X);
				num3 = Math.Min(num3, point.Y);
				num4 = Math.Max(num4, point.Y);
			}
			int num5 = num2 - num + 1;
			int num6 = num4 - num3 + 1;
			if (this.GridSectionsArray.Length == this.GridSectionsInViewportXCount * this.GridSectionsInViewportYCount && num5 == this.GridSectionsInViewportXCount && num6 == this.GridSectionsInViewportYCount)
			{
				return true;
			}
			return false;
		}

		internal void RenderElements(MapGraphics g)
		{
			this.HotRegionList.Clear();
			this.HotRegionList.ScaleFactorX = g.ScaleFactorX;
			this.HotRegionList.ScaleFactorY = g.ScaleFactorY;
			this.RenderFrame(g);
			using (GraphicsPath graphicsPath = this.Viewport.GetHotRegionPath(g))
			{
				this.HotRegionList.SetHotRegion(g, this.Viewport, graphicsPath);
			}
			this.Viewport.Render(g);
			GraphicsState gstate = g.Save();
			try
			{
				RectangleF rect = new RectangleF(this.Viewport.GetAbsoluteLocation(), this.Viewport.GetAbsoluteSize());
				rect.X = (float)Math.Round((double)rect.X);
				rect.Y = (float)Math.Round((double)rect.Y);
				rect.Width = (float)Math.Round((double)rect.Width);
				rect.Height = (float)Math.Round((double)rect.Height);
				if (rect.Width > 0.0 && rect.Height > 0.0)
				{
					g.Graphics.IntersectClip(rect);
					this.Common.InvokePrePaint(this.Viewport);
					if (this.UseGridSectionRendering())
					{
						this.RenderGridSections(g);
					}
					else
					{
						this.RenderContentElements(g, new PointF(0f, 0f), this.HotRegionList);
					}
					if (!this.GridUnderContent)
					{
						this.RenderGrid(g, Point.Empty);
					}
					this.RenderErrorMessage(g);
					this.RenderTileLayerAttributions(g);
					this.Common.InvokePostPaint(this.Viewport);
				}
			}
			finally
			{
				g.Restore(gstate);
			}
			this.Viewport.RenderBorder(g);
			this.RenderPanels(g);
		}

		internal void RenderLayer(MapGraphics g, Layer layer, bool allLayers, RectangleF clipRect, HotRegionList hotRegions, Hashtable visibleLabels)
		{
			if (layer != null && layer.TileSystem != 0 && this.Projection == Projection.Mercator)
			{
				this.RenderTiles(g, layer, clipRect);
			}
			ArrayList arrayList = new ArrayList();
			foreach (IContentElement group in this.Groups)
			{
				if (group.IsVisible(g, layer, allLayers, clipRect))
				{
					arrayList.Add(group);
					if (layer == null || layer.LabelVisible)
					{
						visibleLabels.Add(group, group);
					}
				}
				foreach (IContentElement shape in ((Group)group).Shapes)
				{
					if (shape.IsVisible(g, layer, allLayers, clipRect))
					{
						arrayList.Add(shape);
						if (layer == null || layer.LabelVisible)
						{
							visibleLabels.Add(shape, shape);
						}
					}
					((Shape)shape).ArrangeChildSymbols(g);
					foreach (IContentElement symbol in ((Shape)shape).Symbols)
					{
						if (symbol.IsVisible(g, layer, allLayers, clipRect))
						{
							arrayList.Add(symbol);
							if (layer == null || layer.LabelVisible)
							{
								visibleLabels.Add(symbol, symbol);
							}
						}
					}
				}
			}
			foreach (IContentElement shape2 in this.Shapes)
			{
				if (((Shape)shape2).ParentGroupObject == null)
				{
					if (shape2.IsVisible(g, layer, allLayers, clipRect))
					{
						arrayList.Add(shape2);
						if (layer == null || layer.LabelVisible)
						{
							visibleLabels.Add(shape2, shape2);
						}
					}
					((Shape)shape2).ArrangeChildSymbols(g);
					foreach (IContentElement symbol2 in ((Shape)shape2).Symbols)
					{
						if (symbol2.IsVisible(g, layer, allLayers, clipRect))
						{
							arrayList.Add(symbol2);
							if (layer == null || layer.LabelVisible)
							{
								visibleLabels.Add(symbol2, symbol2);
							}
						}
					}
				}
			}
			foreach (IContentElement path in this.Paths)
			{
				if (path.IsVisible(g, layer, allLayers, clipRect))
				{
					arrayList.Add(path);
					if (layer == null || layer.LabelVisible)
					{
						visibleLabels.Add(path, path);
					}
				}
			}
			foreach (IContentElement symbol3 in this.Symbols)
			{
				if (((Symbol)symbol3).ParentShapeObject == null && symbol3.IsVisible(g, layer, allLayers, clipRect))
				{
					arrayList.Add(symbol3);
					if (layer == null || layer.LabelVisible)
					{
						visibleLabels.Add(symbol3, symbol3);
					}
				}
			}
			foreach (IContentElement item in arrayList)
			{
				item.RenderShadow(g);
			}
			foreach (IContentElement item2 in arrayList)
			{
				base.common.InvokePrePaint((NamedElement)item2);
				item2.RenderBack(g, hotRegions);
			}
			foreach (IContentElement item3 in arrayList)
			{
				item3.RenderFront(g, hotRegions);
			}
		}

		internal void RenderContentElements(MapGraphics g, PointF gridSectionOffset, HotRegionList hotRegions)
		{
			if (this.GridUnderContent)
			{
				this.RenderGrid(g, gridSectionOffset);
			}
			try
			{
				g.CreateContentDrawRegion(this.Viewport, gridSectionOffset);
				RectangleF bounds = g.Clip.GetBounds(g.Graphics);
				this.ResetGeographicClipRectangles();
				Hashtable hashtable = new Hashtable();
				this.RenderLayer(g, null, false, bounds, hotRegions, hashtable);
				if (this.Layers.HasVisibleLayer())
				{
					foreach (Layer layer in this.Layers)
					{
						if (layer.Visible)
						{
							this.RenderLayer(g, layer, false, bounds, hotRegions, hashtable);
						}
					}
					this.RenderLayer(g, null, true, bounds, hotRegions, hashtable);
				}
				foreach (IContentElement key in hashtable.Keys)
				{
					key.RenderText(g, hotRegions);
					this.Common.InvokePostPaint((NamedElement)key);
				}
				this.RenderSelectedContentElements(g, bounds);
			}
			finally
			{
				g.RestoreDrawRegion();
			}
		}

		private void RenderOnePanel(MapGraphics g)
		{
			SizeF absoluteSize = this.PanelToRender.GetAbsoluteSize();
			if (!((double)absoluteSize.Width < 1.0) && !((double)absoluteSize.Height < 1.0))
			{
				try
				{
					using (GraphicsPath graphicsPath = this.PanelToRender.GetHotRegionPath(g))
					{
						this.HotRegionList.SetHotRegion(g, this.PanelToRender, graphicsPath);
					}
					RectangleF rect = new RectangleF((float)((float)(-this.PanelToRender.Margins.Left) / (float)this.GetWidth() * 100.0), (float)((float)(-this.PanelToRender.Margins.Top) / (float)this.GetHeight() * 100.0), (float)(absoluteSize.Width / (float)this.GetWidth() * 100.0), (float)(absoluteSize.Height / (float)this.GetHeight() * 100.0));
					g.CreateDrawRegion(rect);
					this.Common.InvokePrePaint(this.PanelToRender);
					this.PanelToRender.RenderPanel(g);
					this.Common.InvokePostPaint(this.PanelToRender);
				}
				finally
				{
					g.RestoreDrawRegion();
				}
			}
		}

		internal void RenderPanels(MapGraphics g)
		{
			RectangleF bounds = g.Clip.GetBounds(g.Graphics);
			Panel[] array = this.GetSortedPanels();
			Panel[] array2 = array;
			foreach (Panel panel in array2)
			{
				if (!(panel is Viewport))
				{
					RectangleF boundRect = panel.GetBoundRect(g);
					SizeF absoluteSize = g.GetAbsoluteSize(boundRect.Size);
					if (!((double)absoluteSize.Width < 1.0) && !((double)absoluteSize.Height < 1.0) && panel.IsRenderVisible(g, bounds))
					{
						try
						{
							using (GraphicsPath graphicsPath = panel.GetHotRegionPath(g))
							{
								this.HotRegionList.SetHotRegion(g, panel, graphicsPath);
							}
							g.CreateDrawRegion(boundRect);
							this.Common.InvokePrePaint(panel);
							panel.RenderPanel(g);
							this.Common.InvokePostPaint(panel);
						}
						finally
						{
							g.RestoreDrawRegion();
						}
					}
				}
			}
			this.RenderSelectedPanels(g, bounds);
		}

		private void RenderFrame(MapGraphics g)
		{
			if (this.Frame.FrameStyle != 0)
			{
				g.FillRectangleAbs(new RectangleF(0f, 0f, (float)this.Width, (float)this.Height), this.Frame.PageColor, MapHatchStyle.None, "", MapImageWrapMode.Tile, Color.Empty, MapImageAlign.Center, GradientType.None, Color.Empty, this.Frame.PageColor, 1, MapDashStyle.Solid, PenAlignment.Inset);
				g.Draw3DBorderAbs(this.Frame, new RectangleF(0f, 0f, (float)this.GetWidth(), (float)this.GetHeight()), this.BackColor, this.BackHatchStyle, "", MapImageWrapMode.Unscaled, Color.Empty, MapImageAlign.Center, this.BackGradientType, this.BackSecondaryColor, this.BorderLineColor, this.BorderLineWidth, this.BorderLineStyle);
			}
			else
			{
				AntiAliasing antiAliasing = g.AntiAliasing;
				g.AntiAliasing = AntiAliasing.None;
				try
				{
					RectangleF rect = new RectangleF(0f, 0f, (float)this.Width, (float)this.Height);
					g.FillRectangleAbs(rect, this.BackColor, this.BackHatchStyle, "", MapImageWrapMode.Unscaled, Color.Empty, MapImageAlign.Center, this.BackGradientType, this.BackSecondaryColor, Color.Empty, 0, MapDashStyle.Solid, PenAlignment.Inset);
					if (this.BorderLineWidth > 0 && !this.BorderLineColor.IsEmpty && this.BorderLineStyle != 0)
					{
						using (Pen pen = new Pen(this.BorderLineColor, (float)this.BorderLineWidth))
						{
							pen.DashStyle = MapGraphics.GetPenStyle(this.BorderLineStyle);
							pen.Alignment = PenAlignment.Inset;
							if (this.BorderLineWidth == 1)
							{
								rect.Width -= 1f;
								rect.Height -= 1f;
							}
							g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
						}
					}
				}
				finally
				{
					g.AntiAliasing = antiAliasing;
				}
			}
		}

		internal void RenderElementsBufered(MapGraphics g)
		{
			if (this.dirtyFlag || this.bufferBitmap == null || this.bufferBitmap.Graphics.ClipBounds != RectangleF.Union(g.Graphics.ClipBounds, this.bufferBitmap.Graphics.ClipBounds))
			{
				this.bufferBitmap = this.InitBitmap(this.bufferBitmap);
				Graphics graphics = g.Graphics;
				try
				{
					g.Graphics = this.bufferBitmap.Graphics;
					g.Graphics.SetClip(graphics);
					this.RenderElements(g);
				}
				finally
				{
					g.Graphics = graphics;
				}
			}
			if (!this.MapControl.Enabled)
			{
				ImageAttributes imageAttributes = new ImageAttributes();
				ColorMatrix colorMatrix = new ColorMatrix();
				if (!this.MapControl.Enabled)
				{
					colorMatrix.Matrix00 = 0.3f;
					colorMatrix.Matrix01 = 0.3f;
					colorMatrix.Matrix02 = 0.3f;
					colorMatrix.Matrix10 = 0.3f;
					colorMatrix.Matrix11 = 0.3f;
					colorMatrix.Matrix12 = 0.3f;
					colorMatrix.Matrix20 = 0.3f;
					colorMatrix.Matrix21 = 0.3f;
					colorMatrix.Matrix22 = 0.3f;
				}
				imageAttributes.SetColorMatrix(colorMatrix);
				g.Graphics.DrawImage(this.bufferBitmap.Bitmap, Rectangle.Round(g.Graphics.VisibleClipBounds), g.Graphics.VisibleClipBounds.X, g.Graphics.VisibleClipBounds.Y, g.Graphics.VisibleClipBounds.Width, g.Graphics.VisibleClipBounds.Height, GraphicsUnit.Pixel, imageAttributes);
			}
			else
			{
				g.Graphics.DrawImage(this.bufferBitmap.Bitmap, g.Graphics.VisibleClipBounds, g.Graphics.VisibleClipBounds, GraphicsUnit.Pixel);
			}
		}

		internal void RenderSelectedContentElements(MapGraphics g, RectangleF clipRect)
		{
			ISelectable[] selectedContentElements = this.GetSelectedContentElements();
			ISelectable[] array = selectedContentElements;
			foreach (ISelectable selectable in array)
			{
				selectable.DrawSelection(g, clipRect, false);
			}
			if (this.SelectedDesignTimeElement is IContentElement)
			{
				this.SelectedDesignTimeElement.DrawSelection(g, clipRect, true);
			}
			if (this.SelectedDesignTimeElement is Layer)
			{
				this.SelectedDesignTimeElement.DrawSelection(g, clipRect, true);
			}
		}

		internal void RenderSelectedPanels(MapGraphics g, RectangleF clipRect)
		{
			ISelectable[] selectedPanels = this.GetSelectedPanels();
			ISelectable[] array = selectedPanels;
			foreach (ISelectable selectable in array)
			{
				selectable.DrawSelection(g, clipRect, false);
			}
			if (this.SelectedDesignTimeElement is Panel)
			{
				this.SelectedDesignTimeElement.DrawSelection(g, clipRect, true);
			}
		}

		internal ISelectable[] GetSelectedContentElements()
		{
			ArrayList arrayList = new ArrayList();
			foreach (ISelectable group in this.Groups)
			{
				if (group.IsSelected())
				{
					arrayList.Add(group);
				}
			}
			foreach (ISelectable shape in this.Shapes)
			{
				if (shape.IsSelected())
				{
					arrayList.Add(shape);
				}
			}
			foreach (ISelectable path in this.Paths)
			{
				if (path.IsSelected())
				{
					arrayList.Add(path);
				}
			}
			foreach (ISelectable symbol in this.Symbols)
			{
				if (symbol.IsSelected())
				{
					arrayList.Add(symbol);
				}
			}
			return (ISelectable[])arrayList.ToArray(typeof(ISelectable));
		}

		internal ISelectable[] GetSelectedPanels()
		{
			ArrayList arrayList = new ArrayList();
			Panel[] array = this.GetSortedPanels();
			Panel[] array2 = array;
			foreach (ISelectable selectable in array2)
			{
				if (selectable.IsSelected())
				{
					arrayList.Add(selectable);
				}
			}
			return (ISelectable[])arrayList.ToArray(typeof(ISelectable));
		}

		internal double PixelsToKilometers(float pixels)
		{
			PointF absoluteLocation = this.Viewport.GetAbsoluteLocation();
			SizeF absoluteSize = this.Viewport.GetAbsoluteSize();
			PointF pointInPixels = new PointF((float)(absoluteLocation.X + absoluteSize.Width / 2.0), (float)(absoluteLocation.Y + absoluteSize.Height / 2.0));
			PointF pointInPixels2 = new PointF(pointInPixels.X + pixels, pointInPixels.Y);
			MapPoint point = this.PixelsToGeographic(pointInPixels);
			MapPoint point2 = this.PixelsToGeographic(pointInPixels2);
			return this.MeasureDistance(point, point2);
		}

		internal double MeasureDistance(MapPoint point1, MapPoint point2)
		{
			point1.X *= 0.017453292519943295;
			point1.Y *= 0.017453292519943295;
			point2.X *= 0.017453292519943295;
			point2.Y *= 0.017453292519943295;
			double num = point2.X - point1.X;
			double num2 = point2.Y - point1.Y;
			double num3 = Math.Sin(num2 / 2.0) * Math.Sin(num2 / 2.0) + Math.Cos(point2.Y) * Math.Cos(point1.Y) * Math.Sin(num / 2.0) * Math.Sin(num / 2.0);
			double num4 = 2.0 * Math.Atan2(Math.Sqrt(num3), Math.Sqrt(1.0 - num3));
			return 6378.137 * num4;
		}

		internal double GetMinimumAbsoluteLatitude()
		{
			return this.GetMinimumAbsoluteLatitude(this.MinimumPoint.Y, this.MaximumPoint.Y);
		}

		internal double GetMinimumAbsoluteLatitude(double minY, double maxY)
		{
			if (minY <= 0.0 && maxY >= 0.0)
			{
				return 0.0;
			}
			if (minY <= 0.0 && maxY <= 0.0)
			{
				return maxY;
			}
			return minY;
		}

		internal double LimitValue(double value, double lowerLimit, double upperLimit)
		{
			if (value < lowerLimit)
			{
				return lowerLimit;
			}
			if (value > upperLimit)
			{
				return upperLimit;
			}
			return value;
		}

		internal Point3D ApplyProjection(MapPoint mapPoint)
		{
			return this.ApplyProjection(mapPoint.X, mapPoint.Y);
		}

		internal Point3D ApplyProjection(double longitude, double latitude)
		{
			double num = longitude;
			double num2 = latitude;
			double z = 1.0;
			Projection projection = this.Projection;
			if (projection != Projection.Orthographic)
			{
				num -= this.MapCenterPoint.X;
			}
			if (projection != 0)
			{
				num = this.LimitValue(num, -180.0, 180.0);
				num2 = this.LimitValue(num2, -89.0, 89.0);
			}
			switch (projection)
			{
			case Projection.Mercator:
			{
				num2 = this.LimitValue(num2, -85.05112878, 85.05112878);
				double num8 = num2 * MapCore.RadsInDegree;
				num2 = Math.Log(Math.Tan(num8) + 1.0 / Math.Cos(num8));
				num2 *= MapCore.DegreesInRad;
				break;
			}
			case Projection.Wagner3:
			{
				double num9 = num * MapCore.RadsInDegree;
				double num10 = num2 * MapCore.RadsInDegree;
				num = Math.Cos(0.0) / Math.Cos(0.0) * num9 * Math.Cos(2.0 * num10 / 3.0);
				num *= MapCore.DegreesInRad;
				break;
			}
			case Projection.Fahey:
			{
				double num25 = num * MapCore.RadsInDegree;
				double num26 = num2 * MapCore.RadsInDegree;
				double num27 = Math.Tan(num26 / 2.0);
				num = num25 * MapCore.Cos35 * Math.Sqrt(1.0 - num27 * num27);
				num2 = (1.0 + MapCore.Cos35) * num27;
				num *= MapCore.DegreesInRad;
				num2 *= MapCore.DegreesInRad;
				break;
			}
			case Projection.Eckert1:
			{
				double num23 = num * MapCore.RadsInDegree;
				double num24 = num2 * MapCore.RadsInDegree;
				num = MapCore.Eckert1Constant * num23 * (1.0 - Math.Abs(num24) / 3.1415926535897931);
				num2 = MapCore.Eckert1Constant * num24;
				num *= MapCore.DegreesInRad;
				num2 *= MapCore.DegreesInRad;
				break;
			}
			case Projection.Eckert3:
			{
				double num21 = num * MapCore.RadsInDegree;
				double num22 = num2 * MapCore.RadsInDegree;
				num = MapCore.Eckert3ConstantA * num21 * (1.0 + Math.Sqrt(1.0 - 4.0 * (num22 / 3.1415926535897931) * (num22 / 3.1415926535897931)));
				num2 = MapCore.Eckert3ConstantB * num22;
				num *= MapCore.DegreesInRad;
				num2 *= MapCore.DegreesInRad;
				break;
			}
			case Projection.HammerAitoff:
			{
				double num18 = num * MapCore.RadsInDegree;
				double num19 = num2 * MapCore.RadsInDegree;
				double num20 = Math.Sqrt(1.0 + Math.Cos(num19) * Math.Cos(num18 / 2.0));
				num = 2.0 * Math.Cos(num19) * Math.Sin(num18 / 2.0) / num20;
				num2 = Math.Sin(num19) / num20;
				num *= MapCore.DegreesInRad;
				num2 *= MapCore.DegreesInRad;
				break;
			}
			case Projection.Robinson:
			{
				double num11 = Math.Abs(num2);
				int num12 = (int)(num11 / 5.0);
				double num13 = num11 - this.Phi[num12, 0];
				double num14 = num13 * num13;
				double num15 = num14 * num13;
				double num16 = this.A[num12, 0] * num15 + this.A[num12, 1] * num14 + this.A[num12, 2] * num13 + this.A[num12, 3];
				double num17 = this.B[num12, 0] * num15 + this.B[num12, 1] * num14 + this.B[num12, 2] * num13 + this.B[num12, 3];
				num *= num16;
				num2 = num17 * (double)Math.Sign(num2) * MapCore.DegreesInRad;
				break;
			}
			case Projection.Orthographic:
			{
				double xValueRad = num * MapCore.RadsInDegree;
				double yValueRad = num2 * MapCore.RadsInDegree;
				Point3D globePoint = this.GetGlobePoint(xValueRad, yValueRad);
				globePoint = this.RotateAndTilt(globePoint, this.MapCenterPoint.X, this.MapCenterPoint.Y);
				num = globePoint.X;
				num2 = globePoint.Y;
				z = globePoint.Z;
				num *= MapCore.DegreesInRad;
				num2 *= MapCore.DegreesInRad;
				break;
			}
			case Projection.Bonne:
			{
				double num3 = num * MapCore.RadsInDegree;
				double num4 = num2 * MapCore.RadsInDegree;
				double num5 = this.MapCenterPoint.Y;
				if (Math.Abs(num5) < 0.001)
				{
					num5 = 0.001;
				}
				num5 *= MapCore.RadsInDegree;
				double num6 = 1.0 / Math.Tan(num5) + num5 - num4;
				double num7 = num3 * Math.Cos(num4) / num6;
				num = num6 * Math.Sin(num7);
				num2 = 1.0 / Math.Tan(num5) - num6 * Math.Cos(num7);
				num *= MapCore.DegreesInRad;
				num2 *= MapCore.DegreesInRad;
				break;
			}
			}
			return new Point3D(num, num2, z);
		}

		private Point3D RotateAndTilt(Point3D point, double longitude, double latitude)
		{
			longitude = longitude * 3.1415926535897931 / 180.0;
			latitude = latitude * 3.1415926535897931 / 180.0;
			double num = Math.Sin(longitude);
			double num2 = Math.Cos(longitude);
			double num3 = Math.Sin(latitude);
			double num4 = Math.Cos(latitude);
			Matrix3x3 matrix3x = new Matrix3x3();
			matrix3x.Elements[0, 0] = num2;
			matrix3x.Elements[2, 0] = num;
			matrix3x.Elements[0, 2] = 0.0 - num;
			matrix3x.Elements[2, 2] = num2;
			Matrix3x3 matrix3x2 = new Matrix3x3();
			matrix3x2.Elements[1, 1] = num4;
			matrix3x2.Elements[2, 1] = num3;
			matrix3x2.Elements[1, 2] = 0.0 - num3;
			matrix3x2.Elements[2, 2] = num4;
			Matrix3x3 matrix3x3 = matrix3x2 * matrix3x;
			return matrix3x3.TransformPoint(point);
		}

		private Point3D GetGlobePoint(double xValueRad, double yValueRad)
		{
			double num = Math.Cos(yValueRad);
			Point3D result = default(Point3D);
			result.X = num * Math.Sin(xValueRad);
			result.Y = Math.Sin(yValueRad);
			result.Z = num * Math.Cos(xValueRad);
			return result;
		}

		internal MapPoint InverseProjection(double projectedX, double projectedY)
		{
			double num = projectedX;
			double num2 = projectedY;
			Projection projection = this.Projection;
			if (projection != 0)
			{
				MapBounds boundsAfterProjection = this.GetBoundsAfterProjection();
				if (num < boundsAfterProjection.MinimumPoint.X)
				{
					num = boundsAfterProjection.MinimumPoint.X;
				}
				else if (num > boundsAfterProjection.MaximumPoint.X)
				{
					num = boundsAfterProjection.MaximumPoint.X;
				}
				if (num2 < boundsAfterProjection.MinimumPoint.Y)
				{
					num2 = boundsAfterProjection.MinimumPoint.Y;
				}
				else if (num2 > boundsAfterProjection.MaximumPoint.Y)
				{
					num2 = boundsAfterProjection.MaximumPoint.Y;
				}
			}
			switch (projection)
			{
			case Projection.Mercator:
			{
				double value = num2 * MapCore.RadsInDegree;
				num2 = Math.Atan(Math.Sinh(value));
				num2 *= MapCore.DegreesInRad;
				break;
			}
			case Projection.Wagner3:
			{
				double num36 = num * MapCore.RadsInDegree;
				double num37 = num2 * MapCore.RadsInDegree;
				num = num36 / Math.Cos(2.0 * num37 / 3.0);
				num *= MapCore.DegreesInRad;
				break;
			}
			case Projection.Fahey:
			{
				double num34 = num * MapCore.RadsInDegree;
				double num35 = num2 * MapCore.RadsInDegree;
				num2 = 2.0 * Math.Atan(num35 / (1.0 + MapCore.Cos35));
				num = num34 / (MapCore.Cos35 * Math.Sqrt(1.0 - Math.Tan(num2 / 2.0) * Math.Tan(num2 / 2.0)));
				num *= MapCore.DegreesInRad;
				num2 *= MapCore.DegreesInRad;
				break;
			}
			case Projection.Eckert1:
			{
				double num32 = num * MapCore.RadsInDegree;
				double num33 = num2 * MapCore.RadsInDegree;
				num2 = num33 / MapCore.Eckert1Constant;
				num = (0.0 - num32) * 3.1415926535897931 / (MapCore.Eckert1Constant * (Math.Abs(num2) - 3.1415926535897931));
				num *= MapCore.DegreesInRad;
				num2 *= MapCore.DegreesInRad;
				break;
			}
			case Projection.Eckert3:
			{
				double num8 = num * MapCore.RadsInDegree;
				double num9 = num2 * MapCore.RadsInDegree;
				num = num8 * MapCore.Eckert3ConstantC * 3.1415926535897931 / (6.2831853071795862 + Math.Sqrt(39.478417604357432 - 4.0 * num9 * num9 * 3.1415926535897931 - num9 * num9 * 3.1415926535897931 * 3.1415926535897931));
				num2 = 0.25 * num9 * MapCore.Eckert3ConstantC;
				num *= MapCore.DegreesInRad;
				num2 *= MapCore.DegreesInRad;
				break;
			}
			case Projection.HammerAitoff:
			{
				double num10 = num * MapCore.RadsInDegree;
				double num11 = num2 * MapCore.RadsInDegree;
				double num12 = Math.Sqrt(1.0 - Math.Pow(Math.Sqrt(2.0) / 4.0 * num10, 2.0) - Math.Pow(Math.Sqrt(2.0) / 2.0 * num11, 2.0));
				num = 2.0 * Math.Atan(Math.Sqrt(2.0) * num12 * num10 / (2.0 * (2.0 * num12 * num12 - 1.0)));
				num2 = Math.Asin(Math.Sqrt(2.0) * num11 * num12);
				num *= MapCore.DegreesInRad;
				num2 *= MapCore.DegreesInRad;
				break;
			}
			case Projection.Robinson:
			{
				double num22 = Math.Abs(num2) * MapCore.RadsInDegree;
				int num23 = 0;
				while (num23 < 18)
				{
					if (!(this.Phi[num23, 4] > num22))
					{
						num23++;
						continue;
					}
					num23--;
					break;
				}
				if (num23 == 18)
				{
					num23--;
				}
				double num24 = num22 - this.Phi[num23, 4];
				double num25 = num24 * num24;
				double num26 = num25 * num24;
				double num27 = this.Phi[num23, 1] * num26 + this.Phi[num23, 2] * num25 + this.Phi[num23, 3] * num24 + this.Phi[num23, 0];
				double num28 = num27 - this.Phi[num23, 0];
				double num29 = num28 * num28;
				double num30 = num29 * num28;
				double num31 = this.A[num23, 0] * num30 + this.A[num23, 1] * num29 + this.A[num23, 2] * num28 + this.A[num23, 3];
				num /= num31;
				num2 = num27 * (double)Math.Sign(num2);
				break;
			}
			case Projection.Orthographic:
			{
				double num13 = num * MapCore.RadsInDegree;
				double num14 = num2 * MapCore.RadsInDegree;
				double num15 = this.MapCenterPoint.Y * MapCore.RadsInDegree;
				double num16 = Math.Sqrt(num13 * num13 + num14 * num14);
				double num17 = Math.Asin(num16);
				double num18 = Math.Sin(num17);
				double num19 = Math.Cos(num17);
				double num20 = Math.Sin(num15);
				double num21 = Math.Cos(num15);
				num = Math.Atan2(num13 * num18, num16 * num21 * num19 - num14 * num20 * num18);
				num2 = Math.Asin(num19 * num20 + num14 * num18 * num21 / num16);
				num *= MapCore.DegreesInRad;
				num2 *= MapCore.DegreesInRad;
				break;
			}
			case Projection.Bonne:
			{
				double num3 = num * MapCore.RadsInDegree;
				double num4 = num2 * MapCore.RadsInDegree;
				double num5 = this.MapCenterPoint.Y;
				if (Math.Abs(num5) < 0.001)
				{
					num5 = 0.001;
				}
				num5 *= MapCore.RadsInDegree;
				double num6 = 1.0 / Math.Tan(num5);
				double num7 = Math.Sqrt(num3 * num3 + (num6 - num4) * (num6 - num4));
				num7 *= (double)Math.Sign(num5);
				num2 = num6 + num5 - num7;
				num = num7 / Math.Cos(num2) * Math.Atan(num3 / (num6 - num4));
				num *= MapCore.DegreesInRad;
				num2 *= MapCore.DegreesInRad;
				break;
			}
			}
			num += this.MapCenterPoint.X;
			if (num < this.MinimumPoint.X)
			{
				num = this.MinimumPoint.X;
			}
			else if (num > this.MaximumPoint.X)
			{
				num = this.MaximumPoint.X;
			}
			if (num2 < this.MinimumPoint.Y)
			{
				num2 = this.MinimumPoint.Y;
			}
			else if (num2 > this.MaximumPoint.Y)
			{
				num2 = this.MaximumPoint.Y;
			}
			return new MapPoint(num, num2);
		}

		internal double CalculateAspectRatio()
		{
			MapBounds boundsAfterProjection = this.GetBoundsAfterProjection();
			double num = Math.Abs(boundsAfterProjection.MaximumPoint.X - boundsAfterProjection.MinimumPoint.X);
			double num2 = Math.Abs(boundsAfterProjection.MaximumPoint.Y - boundsAfterProjection.MinimumPoint.Y);
			if (num2 > 1E-07)
			{
				return num / num2;
			}
			return 1.0;
		}

		internal void ResetCachedBoundsAfterProjection()
		{
			this.cachedBoundsAfterProjection = null;
		}

		internal MapBounds GetBoundsAfterProjection()
		{
			if (this.cachedBoundsAfterProjection != null)
			{
				return this.cachedBoundsAfterProjection;
			}
			this.cachedBoundsAfterProjection = this.GetBoundsAfterProjection(this.MinimumPoint.X, this.MinimumPoint.Y, this.MaximumPoint.X, this.MaximumPoint.Y);
			return this.cachedBoundsAfterProjection;
		}

		internal MapBounds GetBoundsAfterProjection(double minX, double minY, double maxX, double maxY)
		{
			Point3D point3D = this.ApplyProjection(minX, minY);
			Point3D point3D2 = this.ApplyProjection(maxX, maxY);
			if (this.Projection != 0 && this.Projection != Projection.Mercator)
			{
				List<Point3D> list = new List<Point3D>();
				double num = (maxX - minX) / 250.0;
				if (minX + num == minX)
				{
					num = 1.7976931348623157E+308;
				}
				if (this.Projection == Projection.Bonne || this.Projection == Projection.Orthographic)
				{
					for (double num2 = minX; num2 <= maxX; num2 += num)
					{
						list.Add(this.ApplyProjection(num2, minY));
						list.Add(this.ApplyProjection(num2, maxY));
					}
				}
				num = (maxY - minY) / 250.0;
				if (minY + num == minY)
				{
					num = 1.7976931348623157E+308;
				}
				for (double num3 = minY; num3 <= maxY; num3 += num)
				{
					list.Add(this.ApplyProjection(minX, num3));
					list.Add(this.ApplyProjection(maxX, num3));
				}
				list.Add(this.ApplyProjection(maxX, maxY));
				double minimumAbsoluteLatitude = this.GetMinimumAbsoluteLatitude(minY, maxY);
				list.Add(this.ApplyProjection(minX, minimumAbsoluteLatitude));
				list.Add(this.ApplyProjection(maxX, minimumAbsoluteLatitude));
				list.Add(this.ApplyProjection(this.MapCenterPoint.X, minY));
				list.Add(this.ApplyProjection(this.MapCenterPoint.X, maxY));
				list.Add(this.ApplyProjection(minX, this.MapCenterPoint.Y));
				list.Add(this.ApplyProjection(maxX, this.MapCenterPoint.Y));
				if (this.Projection == Projection.Orthographic)
				{
					if (minX < -90.0 && maxX > -90.0)
					{
						list.Add(this.ApplyProjection(-90.0, minimumAbsoluteLatitude));
					}
					if (minX < 90.0 && maxX > 90.0)
					{
						list.Add(this.ApplyProjection(90.0, minimumAbsoluteLatitude));
					}
				}
				foreach (Point3D item in list)
				{
					Point3D current = item;
					if (current.Z >= 0.0)
					{
						point3D.X = Math.Min(point3D.X, current.X);
						point3D.Y = Math.Min(point3D.Y, current.Y);
						point3D2.X = Math.Max(point3D2.X, current.X);
						point3D2.Y = Math.Max(point3D2.Y, current.Y);
					}
				}
			}
			return new MapBounds(new MapPoint(point3D.X, point3D.Y), new MapPoint(point3D2.X, point3D2.Y));
		}

		internal RectangleF GetGeographicClipRectangle(RectangleF clipRectangle)
		{
			if (!this.cachedGeographicClipRectangles.ContainsKey(clipRectangle))
			{
				double step;
				switch (this.Projection)
				{
				case Projection.Equirectangular:
				case Projection.Mercator:
					step = 1000000.0;
					break;
				default:
					step = 5.0;
					break;
				}
				IEnumerable<PointF> points = Utils.DensifyPoints(Utils.GetRectangePoints(clipRectangle), step);
				this.cachedGeographicClipRectangles[clipRectangle] = this.GetGeographicRectangle(points);
			}
			return this.cachedGeographicClipRectangles[clipRectangle];
		}

		internal void ResetGeographicClipRectangles()
		{
			this.cachedGeographicClipRectangles.Clear();
		}

		internal RectangleF GetGeographicRectangle(IEnumerable<PointF> points)
		{
			float num = 3.40282347E+38f;
			float num2 = 3.40282347E+38f;
			float num3 = -3.40282347E+38f;
			float num4 = -3.40282347E+38f;
			foreach (PointF point in points)
			{
				MapPoint mapPoint = this.ContentToGeographic(point);
				if (!double.IsNaN(mapPoint.X))
				{
					num = Math.Min(num, (float)mapPoint.X);
					num2 = Math.Min(num2, (float)mapPoint.Y);
					num3 = Math.Max(num3, (float)mapPoint.X);
					num4 = Math.Max(num4, (float)mapPoint.Y);
					continue;
				}
				num = (float)this.minimumPoint.X;
				num2 = (float)this.minimumPoint.Y;
				num3 = (float)this.maximumPoint.X;
				num4 = (float)this.maximumPoint.Y;
				break;
			}
			RectangleF result = RectangleF.Empty;
			if (num != -3.4028234663852886E+38)
			{
				result = new RectangleF(num, num2, num3 - num, num4 - num2);
			}
			return result;
		}

		internal void PanBy(Point delta)
		{
			PointF contentOffsetInPixels = this.Viewport.GetContentOffsetInPixels();
			contentOffsetInPixels.X += (float)delta.X;
			contentOffsetInPixels.Y += (float)delta.Y;
			SizeF sizeInPixels = this.Viewport.GetSizeInPixels();
			contentOffsetInPixels.X = Math.Min(contentOffsetInPixels.X, (float)(sizeInPixels.Width - 20.0));
			contentOffsetInPixels.Y = Math.Min(contentOffsetInPixels.Y, (float)(sizeInPixels.Height - 20.0));
			SizeF contentSizeInPixels = this.Viewport.GetContentSizeInPixels();
			contentSizeInPixels.Height *= (float)(this.Viewport.Zoom / 100.0);
			contentSizeInPixels.Width *= (float)(this.Viewport.Zoom / 100.0);
			contentOffsetInPixels.X = Math.Max(contentOffsetInPixels.X, (float)(0.0 - contentSizeInPixels.Width + 20.0));
			contentOffsetInPixels.Y = Math.Max(contentOffsetInPixels.Y, (float)(0.0 - contentSizeInPixels.Height + 20.0));
			this.Viewport.SetContentOffsetInPixels(contentOffsetInPixels);
		}

		internal void EnsureContentIsVisible()
		{
			PointF contentOffsetInPixels = this.Viewport.GetContentOffsetInPixels();
			PointF right = contentOffsetInPixels;
			SizeF sizeInPixels = this.Viewport.GetSizeInPixels();
			if (contentOffsetInPixels.X > sizeInPixels.Width - 20.0)
			{
				contentOffsetInPixels.X = (float)(sizeInPixels.Width - 20.0);
			}
			if (contentOffsetInPixels.Y > sizeInPixels.Height - 20.0)
			{
				contentOffsetInPixels.Y = (float)(sizeInPixels.Height - 20.0);
			}
			SizeF contentSizeInPixels = this.Viewport.GetContentSizeInPixels();
			contentSizeInPixels.Height *= (float)(this.Viewport.Zoom / 100.0);
			contentSizeInPixels.Width *= (float)(this.Viewport.Zoom / 100.0);
			if (contentOffsetInPixels.X < 0.0 - contentSizeInPixels.Width + 20.0)
			{
				contentOffsetInPixels.X = (float)(0.0 - contentSizeInPixels.Width + 20.0);
			}
			if (contentOffsetInPixels.Y < 0.0 - contentSizeInPixels.Height + 20.0)
			{
				contentOffsetInPixels.Y = (float)(0.0 - contentSizeInPixels.Height + 20.0);
			}
			if (contentOffsetInPixels != right)
			{
				this.Viewport.SetContentOffsetInPixels(contentOffsetInPixels);
			}
		}

		internal void CenterView(MapPoint pointOnMap)
		{
			PointF pointF = this.GeographicToPercents(pointOnMap).ToPointF();
			this.Viewport.ViewCenter.X = pointF.X;
			this.Viewport.ViewCenter.Y = pointF.Y;
		}

		internal void Scroll(ScrollDirection direction, double scrollStep)
		{
			PointF pointF = (PointF)this.Viewport.ViewCenter;
			SizeF sizeInPixels = this.Viewport.GetSizeInPixels();
			sizeInPixels.Width *= (float)(scrollStep / 100.0);
			sizeInPixels.Height *= (float)(scrollStep / 100.0);
			Size size = Size.Round(sizeInPixels);
			Size size2 = default(Size);
			if ((direction & ScrollDirection.North) != 0)
			{
				size2.Height = size.Height;
			}
			if ((direction & ScrollDirection.South) != 0)
			{
				size2.Height = -size.Height;
			}
			if ((direction & ScrollDirection.East) != 0)
			{
				size2.Width = -size.Width;
			}
			if ((direction & ScrollDirection.West) != 0)
			{
				size2.Width = size.Width;
			}
			this.PanBy(new Point(size2.Width, size2.Height));
		}

		private void RenderGrid(MapGraphics g, PointF gridSectionOffset)
		{
			try
			{
				g.CreateContentDrawRegion(this.Viewport, gridSectionOffset);
				GraphicsState graphicsState = null;
				bool flag = this.Parallels.Visible && this.Parallels.LineColor != Color.Transparent && this.Parallels.Interval != 0.0 && this.Parallels.LineWidth != 0;
				bool flag2 = this.Meridians.Visible && this.Meridians.LineColor != Color.Transparent && this.Meridians.Interval != 0.0 && this.Meridians.LineWidth != 0;
				if (flag || flag2)
				{
					graphicsState = g.Save();
				}
				RectangleF bounds = g.Clip.GetBounds(g.Graphics);
				if (flag)
				{
					this.Parallels.GridLines = this.GetParallels(g);
					if (this.DrawParallelLabels())
					{
						GraphicsPath graphicsPath = new GraphicsPath();
						graphicsPath.FillMode = FillMode.Winding;
						GridLine[] gridLines = this.Parallels.GridLines;
						for (int i = 0; i < gridLines.Length; i++)
						{
							GridLine gridLine = gridLines[i];
							RectangleF labelRect = gridLine.LabelRect;
							if (!labelRect.IsEmpty && bounds.IntersectsWith(gridLine.LabelRect))
							{
								graphicsPath.AddRectangle(gridLine.LabelRect);
							}
						}
						g.SetClip(graphicsPath, CombineMode.Exclude);
					}
				}
				if (flag2)
				{
					this.Meridians.GridLines = this.GetMeridians(g, this.Parallels.GridLines);
					if (this.DrawMeridianLabels())
					{
						GraphicsPath graphicsPath2 = new GraphicsPath();
						graphicsPath2.FillMode = FillMode.Winding;
						GridLine[] gridLines2 = this.Meridians.GridLines;
						for (int j = 0; j < gridLines2.Length; j++)
						{
							GridLine gridLine2 = gridLines2[j];
							RectangleF labelRect2 = gridLine2.LabelRect;
							if (!labelRect2.IsEmpty && bounds.IntersectsWith(gridLine2.LabelRect))
							{
								graphicsPath2.AddRectangle(gridLine2.LabelRect);
							}
						}
						g.SetClip(graphicsPath2, CombineMode.Exclude);
					}
				}
				if (flag)
				{
					this.RenderParallels(g, this.Parallels.GridLines);
				}
				if (flag2)
				{
					this.RenderMeridians(g, this.Meridians.GridLines);
				}
				if (graphicsState != null)
				{
					g.Restore(graphicsState);
				}
				if (this.SelectedDesignTimeElement is GridAttributes)
				{
					this.SelectedDesignTimeElement.DrawSelection(g, RectangleF.Empty, true);
				}
			}
			finally
			{
				g.RestoreDrawRegion();
			}
		}

		internal bool DrawParallelLabels()
		{
			if (this.UseGridSectionRendering() && this.GridUnderContent)
			{
				return false;
			}
			return this.Parallels.ShowLabels;
		}

		internal bool DrawMeridianLabels()
		{
			if (this.UseGridSectionRendering() && this.GridUnderContent)
			{
				return false;
			}
			return this.Meridians.ShowLabels;
		}

		internal void RenderParallels(MapGraphics g, GridLine[] parallels)
		{
			AntiAliasing antiAliasing = g.AntiAliasing;
			if (!this.AreParallelsCurved())
			{
				g.AntiAliasing = AntiAliasing.None;
			}
			g.StartHotRegion(this.Parallels);
			GraphicsPath[] array = new GraphicsPath[parallels.Length];
			using (Pen pen = this.Parallels.GetPen())
			{
				int num = 0;
				for (int i = 0; i < parallels.Length; i++)
				{
					GridLine gridLine = parallels[i];
					if (gridLine.Path != null)
					{
						g.DrawPath(pen, gridLine.Path);
						array[num++] = gridLine.Path;
					}
				}
			}
			this.HotRegionList.SetHotRegion(g, this.Parallels, array);
			g.EndHotRegion();
			g.AntiAliasing = antiAliasing;
		}

		internal void RenderMeridians(MapGraphics g, GridLine[] meridians)
		{
			AntiAliasing antiAliasing = g.AntiAliasing;
			if (!this.AreMeridiansCurved() && this.Projection != Projection.Eckert1)
			{
				g.AntiAliasing = AntiAliasing.None;
			}
			g.StartHotRegion(this.Meridians);
			GraphicsPath[] array = new GraphicsPath[meridians.Length];
			using (Pen pen = this.Meridians.GetPen())
			{
				int num = 0;
				for (int i = 0; i < meridians.Length; i++)
				{
					GridLine gridLine = meridians[i];
					if (gridLine.Path != null)
					{
						g.DrawPath(pen, gridLine.Path);
						array[num++] = gridLine.Path;
					}
				}
			}
			this.HotRegionList.SetHotRegion(g, this.Meridians, array);
			g.EndHotRegion();
			g.AntiAliasing = antiAliasing;
		}

		internal GridLine[] GetParallels(MapGraphics g)
		{
			bool flag = this.AreParallelsCurved();
			int num = (!flag) ? 10 : 50;
			double[] parallelPositions = this.GetParallelPositions();
			GridLine[] array = new GridLine[parallelPositions.Length];
			for (int i = 0; i < parallelPositions.Length; i++)
			{
				PointF[] array2 = new PointF[num];
				double num2 = Math.Abs(this.MaximumPoint.X - this.MinimumPoint.X) / (double)(num - 1);
				double num3 = this.MinimumPoint.X;
				for (int j = 0; j < num; j++)
				{
					array2[j] = this.GeographicToPercents(num3, parallelPositions[i]).ToPointF();
					array2[j] = g.GetAbsolutePoint(array2[j]);
					num3 += num2;
				}
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.StartFigure();
				if (flag)
				{
					graphicsPath.AddCurve(array2);
				}
				else
				{
					graphicsPath.AddLines(array2);
				}
				array[i].GridType = GridType.Parallel;
				array[i].Points = array2;
				array[i].Path = graphicsPath;
				array[i].Coordinate = parallelPositions[i];
			}
			this.CalculateSelectionMarkerPositions(g, array);
			if (this.DrawParallelLabels())
			{
				string[] array3 = new string[parallelPositions.Length];
				SizeF[] array4 = new SizeF[parallelPositions.Length];
				float num4 = 0f;
				for (int k = 0; k < parallelPositions.Length; k++)
				{
					array3[k] = this.FormatParallelLabel(parallelPositions[k]);
					array4[k] = g.MeasureString(array3[k], this.Parallels.Font);
					num4 = Math.Max(num4, array4[k].Width);
				}
				MapPoint mapPoint = this.CalculateIdealLabelPointForParallels(g, num4);
				for (int l = 0; l < parallelPositions.Length; l++)
				{
					StringFormat stringFormat = new StringFormat();
					stringFormat.Alignment = StringAlignment.Center;
					stringFormat.LineAlignment = StringAlignment.Center;
					MapPoint mapPoint2 = new MapPoint(mapPoint.X, parallelPositions[l]);
					PointF absolutePoint = g.GetAbsolutePoint(this.GeographicToPercents(mapPoint2).ToPointF());
					RectangleF rectangleF = new RectangleF(absolutePoint.X, absolutePoint.Y, 0f, 0f);
					rectangleF.Inflate((float)(array4[l].Width / 2.0), (float)(array4[l].Height / 2.0));
					bool flag2 = false;
					int num5 = l - 1;
					while (num5 >= 0)
					{
						if (!(RectangleF.Intersect(array[num5].LabelRect, rectangleF) != RectangleF.Empty))
						{
							num5--;
							continue;
						}
						flag2 = true;
						break;
					}
					if (!flag2)
					{
						array[l].LabelRect = rectangleF;
						using (Brush brush = new SolidBrush(this.Parallels.LabelColor))
						{
							g.DrawString(array3[l], this.Parallels.Font, brush, absolutePoint, stringFormat);
						}
					}
				}
			}
			return array;
		}

		private string FormatParallelLabel(double value)
		{
			if (this.MapControl.FormatNumberHandler != null)
			{
				return this.MapControl.FormatNumberHandler(this.MapControl, value, this.Parallels.LabelFormatString);
			}
			return value.ToString(this.Parallels.LabelFormatString, CultureInfo.CurrentCulture);
		}

		private string FormatMeridianLabel(double value)
		{
			if (this.MapControl.FormatNumberHandler != null)
			{
				return this.MapControl.FormatNumberHandler(this.MapControl, value, this.Meridians.LabelFormatString);
			}
			return value.ToString(this.Meridians.LabelFormatString, CultureInfo.CurrentCulture);
		}

		private void CalculateSelectionMarkerPositions(MapGraphics g, GridLine[] gridLines)
		{
			if (gridLines.Length != 0)
			{
				RectangleF rectangleF = new RectangleF(this.Viewport.GetAbsoluteLocation(), this.Viewport.GetAbsoluteSize());
				rectangleF.Inflate((float)(-3.0 / g.ScaleFactorX), (float)(-3.0 / g.ScaleFactorY));
				rectangleF.Width -= 1f;
				rectangleF.Height -= 1f;
				rectangleF = this.PixelsToContent(rectangleF);
				for (int i = 0; i < gridLines.Length; i++)
				{
					gridLines[i].SelectionMarkerPositions = this.FindAllIntersectingPoints(gridLines[i], rectangleF);
				}
			}
		}

		private float CalculateYIntersect(PointF point1, PointF point2, float xIntersect)
		{
			if (point1.X == point2.X)
			{
				return point1.X;
			}
			float num = (point2.Y - point1.Y) / (point2.X - point1.X);
			return num * (xIntersect - point1.X) + point1.Y;
		}

		private float CalculateXIntersect(PointF point1, PointF point2, float yIntersect)
		{
			return this.CalculateYIntersect(new PointF(point1.Y, point1.X), new PointF(point2.Y, point2.X), yIntersect);
		}

		private PointF[] FindAllIntersectingPoints(GridLine gridLine, RectangleF rect)
		{
			Hashtable hashtable = new Hashtable();
			if (gridLine.GridType == GridType.Parallel)
			{
				this.FindIntersectingPoints(hashtable, gridLine.Points, rect.Left, rect.Top, rect.Left, rect.Bottom);
				this.FindIntersectingPoints(hashtable, gridLine.Points, rect.Right, rect.Top, rect.Right, rect.Bottom);
				this.FindIntersectingPoints(hashtable, gridLine.Points, rect.Left, rect.Top, rect.Right, rect.Top);
				this.FindIntersectingPoints(hashtable, gridLine.Points, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
			}
			else
			{
				this.FindIntersectingPoints(hashtable, gridLine.Points, rect.Left, rect.Top, rect.Right, rect.Top);
				this.FindIntersectingPoints(hashtable, gridLine.Points, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
				this.FindIntersectingPoints(hashtable, gridLine.Points, rect.Left, rect.Top, rect.Left, rect.Bottom);
				this.FindIntersectingPoints(hashtable, gridLine.Points, rect.Right, rect.Top, rect.Right, rect.Bottom);
			}
			if (rect.Contains(gridLine.Points[0]))
			{
				hashtable[gridLine.Points[0]] = null;
			}
			if (rect.Contains(gridLine.Points[gridLine.Points.Length - 1]))
			{
				hashtable[gridLine.Points[gridLine.Points.Length - 1]] = null;
			}
			PointF[] array = new PointF[hashtable.Keys.Count];
			int num = 0;
			foreach (PointF key in hashtable.Keys)
			{
				array[num++] = key;
			}
			return array;
		}

		private void FindIntersectingPoints(Hashtable resultSet, PointF[] points, float x1, float y1, float x2, float y2)
		{
			if (x1 == x2)
			{
				for (int i = 0; i < points.Length - 1; i++)
				{
					if (points[i].X <= x1 && x1 <= points[i + 1].X)
					{
						goto IL_004f;
					}
					if (points[i + 1].X <= x1 && x1 <= points[i].X)
					{
						goto IL_004f;
					}
					continue;
					IL_004f:
					float num = this.CalculateYIntersect(points[i], points[i + 1], x1);
					if (y1 <= num && num <= y2)
					{
						resultSet[new PointF(x1, num)] = null;
					}
				}
				return;
			}
			if (y1 == y2)
			{
				for (int j = 0; j < points.Length - 1; j++)
				{
					if (points[j].Y <= y1 && y1 <= points[j + 1].Y)
					{
						goto IL_00f2;
					}
					if (points[j + 1].Y <= y1 && y1 <= points[j].Y)
					{
						goto IL_00f2;
					}
					continue;
					IL_00f2:
					float num2 = this.CalculateXIntersect(points[j], points[j + 1], y1);
					if (x1 <= num2 && num2 <= x2)
					{
						resultSet[new PointF(num2, y1)] = null;
					}
				}
				return;
			}
			throw new NotSupportedException();
		}

		private MapPoint CalculateIdealLabelPointForParallels(MapGraphics g, float labelWidth)
		{
			PointF pointF = PointF.Empty;
			if (this.RenderingMode == RenderingMode.GridSections)
			{
				if (this.Parallels.LabelPosition == LabelPosition.Near)
				{
					pointF.X = 0f;
				}
				else if (this.Parallels.LabelPosition == LabelPosition.OneQuarter)
				{
					pointF.X = 25f;
				}
				else if (this.Parallels.LabelPosition == LabelPosition.Center)
				{
					pointF.X = 50f;
				}
				else if (this.Parallels.LabelPosition == LabelPosition.ThreeQuarters)
				{
					pointF.X = 75f;
				}
				else if (this.Parallels.LabelPosition == LabelPosition.Far)
				{
					pointF.X = 100f;
				}
				pointF.Y = 50f;
				return this.PercentsToGeographic((double)pointF.X, (double)pointF.Y);
			}
			RectangleF rectangleF = new RectangleF(this.Viewport.GetAbsoluteLocation(), this.Viewport.GetAbsoluteSize());
			float num = 8f;
			if (this.Parallels.LabelPosition == LabelPosition.Near)
			{
				pointF.X = (float)(rectangleF.X + labelWidth / 2.0 + num);
			}
			else if (this.Parallels.LabelPosition == LabelPosition.OneQuarter)
			{
				pointF.X = (float)(rectangleF.X + rectangleF.Width * 0.25);
			}
			else if (this.Parallels.LabelPosition == LabelPosition.Center)
			{
				pointF.X = (float)(rectangleF.X + rectangleF.Width * 0.5);
			}
			else if (this.Parallels.LabelPosition == LabelPosition.ThreeQuarters)
			{
				pointF.X = (float)(rectangleF.X + rectangleF.Width * 0.75);
			}
			else if (this.Parallels.LabelPosition == LabelPosition.Far)
			{
				pointF.X = (float)(rectangleF.X + rectangleF.Width - labelWidth / 2.0 - num);
			}
			pointF.Y = (float)(rectangleF.Y + rectangleF.Height / 2.0);
			pointF = this.PixelsToContent(pointF);
			PointF relativePoint = g.GetRelativePoint(pointF);
			return this.PercentsToGeographic((double)relativePoint.X, (double)relativePoint.Y);
		}

		private MapPoint CalculateIdealLabelPointForMeridians(MapGraphics g, float labelHeight)
		{
			PointF pointF = PointF.Empty;
			if (this.RenderingMode == RenderingMode.GridSections)
			{
				if (this.Meridians.LabelPosition == LabelPosition.Near)
				{
					pointF.Y = 0f;
				}
				else if (this.Meridians.LabelPosition == LabelPosition.OneQuarter)
				{
					pointF.Y = 25f;
				}
				else if (this.Meridians.LabelPosition == LabelPosition.Center)
				{
					pointF.Y = 50f;
				}
				else if (this.Meridians.LabelPosition == LabelPosition.ThreeQuarters)
				{
					pointF.Y = 75f;
				}
				else if (this.Meridians.LabelPosition == LabelPosition.Far)
				{
					pointF.Y = 100f;
				}
				pointF.X = 50f;
				return this.PercentsToGeographic((double)pointF.X, (double)pointF.Y);
			}
			RectangleF rectangleF = new RectangleF(this.Viewport.GetAbsoluteLocation(), this.Viewport.GetAbsoluteSize());
			float num = 8f;
			if (this.Meridians.LabelPosition == LabelPosition.Near)
			{
				pointF.Y = (float)(rectangleF.Y + labelHeight / 2.0 + num);
			}
			else if (this.Meridians.LabelPosition == LabelPosition.OneQuarter)
			{
				pointF.Y = (float)(rectangleF.Y + rectangleF.Height * 0.25);
			}
			else if (this.Meridians.LabelPosition == LabelPosition.Center)
			{
				pointF.Y = (float)(rectangleF.Y + rectangleF.Height * 0.5);
			}
			else if (this.Meridians.LabelPosition == LabelPosition.ThreeQuarters)
			{
				pointF.Y = (float)(rectangleF.Y + rectangleF.Height * 0.75);
			}
			else if (this.Meridians.LabelPosition == LabelPosition.Far)
			{
				pointF.Y = (float)(rectangleF.Y + rectangleF.Height - labelHeight / 2.0 - num);
			}
			pointF.X = (float)(rectangleF.X + rectangleF.Width / 2.0);
			pointF = this.PixelsToContent(pointF);
			PointF relativePoint = g.GetRelativePoint(pointF);
			return this.PercentsToGeographic((double)relativePoint.X, (double)relativePoint.Y);
		}

		internal GridLine[] GetMeridians(MapGraphics g, GridLine[] parallels)
		{
			bool flag = this.AreMeridiansCurved();
			int num = (!flag) ? 5 : 50;
			double[] meridianPositions = this.GetMeridianPositions();
			GridLine[] array = new GridLine[meridianPositions.Length];
			for (int i = 0; i < meridianPositions.Length; i++)
			{
				PointF[] array2 = new PointF[num];
				double num2 = Math.Abs(this.MaximumPoint.Y - this.MinimumPoint.Y) / (double)(num - 1);
				double num3 = this.MinimumPoint.Y;
				for (int j = 0; j < num; j++)
				{
					if (num == 3 && j == 1)
					{
						array2[j] = this.GeographicToPercents(meridianPositions[i], 0.0).ToPointF();
					}
					else
					{
						array2[j] = this.GeographicToPercents(meridianPositions[i], num3).ToPointF();
					}
					array2[j] = g.GetAbsolutePoint(array2[j]);
					num3 += num2;
				}
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.StartFigure();
				if (flag)
				{
					graphicsPath.AddCurve(array2);
				}
				else
				{
					graphicsPath.AddLines(array2);
				}
				array[i].GridType = GridType.Meridian;
				array[i].Points = array2;
				array[i].Path = graphicsPath;
				array[i].Coordinate = meridianPositions[i];
			}
			this.CalculateSelectionMarkerPositions(g, array);
			if (this.DrawMeridianLabels())
			{
				float height = g.MeasureString("5", this.Meridians.Font).Height;
				MapPoint mapPoint = this.CalculateIdealLabelPointForMeridians(g, height);
				for (int k = 0; k < meridianPositions.Length; k++)
				{
					StringFormat stringFormat = new StringFormat();
					stringFormat.Alignment = StringAlignment.Center;
					stringFormat.LineAlignment = StringAlignment.Center;
					MapPoint mapPoint2 = new MapPoint(meridianPositions[k], mapPoint.Y);
					PointF absolutePoint = g.GetAbsolutePoint(this.GeographicToPercents(mapPoint2).ToPointF());
					string text = this.FormatMeridianLabel(meridianPositions[k]);
					SizeF sizeF = g.MeasureString(text, this.Parallels.Font);
					RectangleF rectangleF = new RectangleF(absolutePoint.X, absolutePoint.Y, 0f, 0f);
					rectangleF.Inflate((float)(sizeF.Width / 2.0), (float)(sizeF.Height / 2.0));
					bool flag2 = false;
					if (parallels != null)
					{
						for (int l = 0; l < parallels.Length; l++)
						{
							GridLine gridLine = parallels[l];
							if (RectangleF.Intersect(gridLine.LabelRect, rectangleF) != RectangleF.Empty)
							{
								flag2 = true;
								break;
							}
						}
					}
					if (!flag2)
					{
						bool flag3 = false;
						int num4 = k - 1;
						while (num4 >= 0)
						{
							if (!(RectangleF.Intersect(array[num4].LabelRect, rectangleF) != RectangleF.Empty))
							{
								num4--;
								continue;
							}
							flag3 = true;
							break;
						}
						if (!flag3)
						{
							array[k].LabelRect = rectangleF;
							using (Brush brush = new SolidBrush(this.Meridians.LabelColor))
							{
								g.DrawString(text, this.Meridians.Font, brush, absolutePoint, stringFormat);
							}
						}
					}
				}
			}
			return array;
		}

		private double GetParallelInterval()
		{
			if (!double.IsNaN(this.Parallels.Interval))
			{
				return this.Parallels.Interval;
			}
			if (this.MaximumPoint.Y == 90.0 && this.MinimumPoint.Y == -90.0)
			{
				goto IL_00ba;
			}
			if (this.MaximumPoint.Y == 89.0 && this.MinimumPoint.Y == -89.0)
			{
				goto IL_00ba;
			}
			if (this.MaximumPoint.Y == 85.05112878 && this.MinimumPoint.Y == -85.05112878)
			{
				goto IL_00ba;
			}
			double num = this.MaximumPoint.Y - this.MinimumPoint.Y;
			double num2 = num / 20.0;
			int num3 = (int)Math.Log10(num2);
			double num4 = Math.Pow(10.0, (double)num3);
			return Math.Ceiling(num2 / num4) * num4;
			IL_00ba:
			return 10.0;
		}

		private double GetMeridianInterval()
		{
			if (!double.IsNaN(this.Meridians.Interval))
			{
				return this.Meridians.Interval;
			}
			double num = this.MaximumPoint.X - this.MinimumPoint.X;
			double num2 = num / 20.0;
			int num3 = (int)Math.Log10(num2);
			double num4 = Math.Pow(10.0, (double)num3);
			return Math.Ceiling(num2 / num4) * num4;
		}

		private double[] GetParallelPositions()
		{
			ArrayList arrayList = new ArrayList();
			double num = this.GetParallelInterval();
			double num2 = this.MaximumPoint.Y - this.MinimumPoint.Y;
			if (num2 / num > 1000.0)
			{
				num = num2 / 1000.0;
			}
			if (this.MinimumPoint.Y < 0.0 && this.MaximumPoint.Y > 0.0)
			{
				double num3;
				for (num3 = 0.0; num3 <= this.MaximumPoint.Y; num3 += num)
				{
					num3 = double.Parse(num3.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
					arrayList.Add(num3);
				}
				for (num3 = 0.0 - num; num3 >= this.MinimumPoint.Y; num3 -= num)
				{
					num3 = double.Parse(num3.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
					arrayList.Add(num3);
				}
			}
			else if (this.MaximumPoint.Y > 0.0)
			{
				for (double num4 = Math.Ceiling(this.MinimumPoint.Y / num) * num; num4 <= this.MaximumPoint.Y; num4 += num)
				{
					num4 = double.Parse(num4.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
					arrayList.Add(num4);
				}
			}
			else if (this.MinimumPoint.Y < 0.0)
			{
				for (double num5 = Math.Floor(this.MaximumPoint.Y / num) * num; num5 >= this.MinimumPoint.Y; num5 -= num)
				{
					num5 = double.Parse(num5.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
					arrayList.Add(num5);
				}
			}
			arrayList.Sort();
			return (double[])arrayList.ToArray(typeof(double));
		}

		private double[] GetMeridianPositions()
		{
			ArrayList arrayList = new ArrayList();
			double num = this.GetMeridianInterval();
			double num2 = this.MaximumPoint.X - this.MinimumPoint.X;
			if (num2 / num > 2000.0)
			{
				num = num2 / 2000.0;
			}
			if (this.MinimumPoint.X < 0.0 && this.MaximumPoint.X > 0.0)
			{
				double num3;
				for (num3 = 0.0; num3 <= this.MaximumPoint.X; num3 += num)
				{
					num3 = double.Parse(num3.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
					arrayList.Add(num3);
				}
				for (num3 = 0.0 - num; num3 >= this.MinimumPoint.X; num3 -= num)
				{
					num3 = double.Parse(num3.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
					arrayList.Add(num3);
				}
			}
			else if (this.MaximumPoint.X > 0.0)
			{
				for (double num4 = Math.Ceiling(this.MinimumPoint.X / num) * num; num4 <= this.MaximumPoint.X; num4 += num)
				{
					num4 = double.Parse(num4.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
					arrayList.Add(num4);
				}
			}
			else if (this.MinimumPoint.X < 0.0)
			{
				for (double num5 = Math.Floor(this.MaximumPoint.X / num) * num; num5 >= this.MinimumPoint.X; num5 -= num)
				{
					num5 = double.Parse(num5.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
					arrayList.Add(num5);
				}
			}
			arrayList.Sort();
			return (double[])arrayList.ToArray(typeof(double));
		}

		internal bool AreParallelsCurved()
		{
			if (this.Projection != Projection.HammerAitoff && this.Projection != Projection.Orthographic && this.Projection != Projection.Bonne)
			{
				return false;
			}
			return true;
		}

		internal bool AreMeridiansCurved()
		{
			if (this.Projection != 0 && this.Projection != Projection.Mercator && this.Projection != Projection.Eckert1)
			{
				return true;
			}
			return false;
		}

		internal void InvalidateRules()
		{
			if (!this.applyingRules)
			{
				this.rulesDirty = true;
			}
		}

		internal void ApplyAllRules()
		{
			if (this.rulesDirty)
			{
				if (this.Common != null && this.Common.MapControl != null)
				{
					this.Common.MapControl.OnBeforeApplyingRules(EventArgs.Empty);
				}
				this.applyingRules = true;
				this.rulesDirty = false;
				for (int i = 0; i < this.ColorSwatchPanel.Colors.Count; i++)
				{
					if (this.ColorSwatchPanel.Colors[i].AutomaticallyAdded)
					{
						this.ColorSwatchPanel.Colors.RemoveAt(i--);
					}
				}
				foreach (Legend legend in this.Legends)
				{
					for (int j = 0; j < legend.Items.Count; j++)
					{
						if (legend.Items[j].AutomaticallyAdded)
						{
							legend.Items.RemoveAt(j--);
						}
					}
				}
				foreach (GroupRule groupRule3 in this.GroupRules)
				{
					groupRule3.RegenerateColorRanges();
				}
				foreach (ShapeRule shapeRule3 in this.ShapeRules)
				{
					shapeRule3.RegenerateColorRanges();
				}
				foreach (PathRuleBase pathRule in this.PathRules)
				{
					pathRule.RegenerateRanges();
				}
				foreach (SymbolRule symbolRule3 in this.SymbolRules)
				{
					symbolRule3.UpdateAutoRanges();
				}
				foreach (Group group in this.Groups)
				{
					group.UseInternalProperties = false;
					foreach (GroupRule groupRule4 in this.GroupRules)
					{
						groupRule4.Apply(group);
					}
				}
				foreach (Shape shape in this.Shapes)
				{
					shape.UseInternalProperties = false;
					foreach (ShapeRule shapeRule4 in this.ShapeRules)
					{
						shapeRule4.Apply(shape);
					}
				}
				foreach (Path path in this.Paths)
				{
					path.UseInternalProperties = false;
					foreach (PathRuleBase pathRule2 in this.PathRules)
					{
						pathRule2.Apply(path);
					}
				}
				foreach (Symbol symbol in this.Symbols)
				{
					symbol.UseInternalProperties = false;
					foreach (SymbolRule symbolRule4 in this.SymbolRules)
					{
						symbolRule4.Apply(symbol);
					}
				}
				this.applyingRules = false;
				if (this.Common != null && this.Common.MapControl != null)
				{
					this.Common.MapControl.OnAllRulesApplied(EventArgs.Empty);
				}
			}
		}

		internal int GetWidth()
		{
			if (this.isPrinting)
			{
				return this.printSize.Width;
			}
			return this.MapControl.Width;
		}

		internal int GetHeight()
		{
			if (this.isPrinting)
			{
				return this.printSize.Height;
			}
			return this.MapControl.Height;
		}

		internal void ResetAutoValues()
		{
			if (this.SelectedDesignTimeElement != null)
			{
				this.SelectedDesignTimeElement = null;
			}
		}

		internal override void Invalidate()
		{
			this.dirtyFlag = true;
		}

		internal override void Invalidate(RectangleF rect)
		{
			this.dirtyFlag = true;
		}

		internal override void InvalidateViewport(bool invalidateGridSections)
		{
			if (this.Viewport != null && !this.disableInvalidate)
			{
				if (invalidateGridSections)
				{
					this.InvalidateGridSections();
				}
				RectangleF rect = new RectangleF(this.Viewport.GetAbsoluteLocation(), this.Viewport.GetAbsoluteSize());
				this.Invalidate(rect);
			}
		}

		internal override void InvalidateDistanceScalePanel()
		{
			if (this.DistanceScalePanel != null)
			{
				RectangleF rect = new RectangleF(this.DistanceScalePanel.GetAbsoluteLocation(), this.DistanceScalePanel.GetAbsoluteSize());
				this.Invalidate(rect);
			}
		}

		internal override void InvalidateAndLayout()
		{
			this.DoPanelLayout = true;
			this.Invalidate();
		}

		internal void InvalidateCachedPaths()
		{
			if (!this.resetingCachedPaths)
			{
				this.cachedPathsDirty = true;
			}
		}

		internal void ResetCachedPaths()
		{
			if (this.cachedPathsDirty)
			{
				this.resetingCachedPaths = true;
				this.cachedPathsDirty = false;
				foreach (Shape shape in this.Shapes)
				{
					shape.ResetCachedPaths();
				}
				foreach (Path path in this.Paths)
				{
					path.ResetCachedPaths();
				}
				foreach (Symbol symbol in this.Symbols)
				{
					symbol.ResetCachedPaths();
				}
				this.resetingCachedPaths = false;
			}
		}

		internal void OnFontChanged()
		{
		}

		internal NamedCollection[] GetRenderCollections()
		{
			ArrayList arrayList = new ArrayList();
			arrayList.Add(this.DataBindingRules);
			arrayList.Add(this.Symbols);
			arrayList.Add(this.SymbolRules);
			arrayList.Add(this.SymbolFields);
			arrayList.Add(this.Shapes);
			arrayList.Add(this.ShapeRules);
			arrayList.Add(this.ShapeFields);
			arrayList.Add(this.Paths);
			arrayList.Add(this.PathRules);
			arrayList.Add(this.PathFields);
			arrayList.Add(this.Groups);
			arrayList.Add(this.GroupRules);
			arrayList.Add(this.GroupFields);
			arrayList.Add(this.Layers);
			arrayList.Add(this.Images);
			arrayList.Add(this.Labels);
			arrayList.Add(this.Legends);
			return (NamedCollection[])arrayList.ToArray(typeof(NamedCollection));
		}

		internal override void BeginInit()
		{
			this.isInitializing = true;
			NamedCollection[] renderCollections = this.GetRenderCollections();
			foreach (NamedCollection namedCollection in renderCollections)
			{
				namedCollection.BeginInit();
			}
		}

		internal override void EndInit()
		{
			this.skipPaint = false;
			this.isInitializing = false;
			NamedCollection[] renderCollections = this.GetRenderCollections();
			foreach (NamedCollection namedCollection in renderCollections)
			{
				namedCollection.EndInit();
			}
			this.LoadFieldsFromBuffers();
			this.InvalidateCachedBounds();
			this.InvalidateRules();
			this.InvalidateCachedPaths();
			this.InvalidateChildSymbols();
			this.InvalidateDataBinding();
			this.InvalidateAndLayout();
			if (this.AutoUpdates)
			{
				this.UpdateCachedBounds();
				this.ResetCachedPaths();
				this.ResetChildSymbols();
			}
		}

		private void LoadFieldsFromBuffers()
		{
			foreach (Group group in this.Groups)
			{
				group.FieldDataFromBuffer();
			}
			foreach (Shape shape in this.Shapes)
			{
				shape.FieldDataFromBuffer();
			}
			foreach (Path path in this.Paths)
			{
				path.FieldDataFromBuffer();
			}
			foreach (Symbol symbol in this.Symbols)
			{
				symbol.FieldDataFromBuffer();
			}
		}

		internal override void ReconnectData(bool exact)
		{
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

		internal void SaveTo(string fileName, MapImageFormat imageFormat, int compression, Panel panel, bool zoomThumbOnly)
		{
			using (Stream stream = new FileStream(fileName, FileMode.Create))
			{
				this.SaveTo(stream, imageFormat, compression, panel, zoomThumbOnly);
				stream.Close();
			}
		}

		internal void SaveTo(Stream stream, MapImageFormat imageFormat, int compression, Panel panel, bool zoomThumbOnly)
		{
			if (panel != null)
			{
				if (panel is ZoomPanel && zoomThumbOnly)
				{
					this.RenderingMode = RenderingMode.ZoomThumb;
				}
				else
				{
					this.RenderingMode = RenderingMode.SinglePanel;
				}
				this.PanelToRender = panel;
			}
			bool flag = this.dirtyFlag;
			this.dirtyFlag = true;
			this.Notify(MessageType.PrepareSnapShot, this, null);
			if (imageFormat == MapImageFormat.Emf)
			{
				this.SaveIntoMetafile(stream);
				this.dirtyFlag = flag;
				if (panel != null)
				{
					this.RenderingMode = RenderingMode.All;
					this.PanelToRender = null;
				}
			}
			else
			{
				BufferBitmap bufferBitmap = this.InitBitmap(null);
				RenderingType renderingType = RenderingType.Gdi;
				ImageFormat imageFormat2 = null;
				ImageCodecInfo imageCodecInfo = null;
				EncoderParameter encoderParameter = null;
				EncoderParameters encoderParameters = null;
				string text = imageFormat.ToString();
				imageFormat2 = (ImageFormat)new ImageFormatConverter().ConvertFromString(text);
				this.Paint(bufferBitmap.Graphics, renderingType, stream, false);
				if (renderingType == RenderingType.Gdi && compression >= 0 && compression <= 100 && "JPEG,PNG,BMP".IndexOf(text.ToUpperInvariant(), StringComparison.Ordinal) != -1)
				{
					imageCodecInfo = MapCore.GetEncoderInfo("image/" + text.ToLowerInvariant());
					encoderParameters = new EncoderParameters(1);
					encoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L - (long)compression);
					encoderParameters.Param[0] = encoderParameter;
				}
				if (!this.MapControl.Enabled)
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
				if (this.RenderingMode == RenderingMode.SinglePanel && this.PanelToRender.IsMakeTransparentRequired())
				{
					bufferBitmap.Bitmap.MakeTransparent(this.PanelToRender.GetColorForMakeTransparent());
				}
				if (renderingType == RenderingType.Gdi && imageFormat != MapImageFormat.Emf)
				{
					if (imageCodecInfo == null)
					{
						bufferBitmap.Bitmap.Save(stream, imageFormat2);
					}
					else
					{
						bufferBitmap.Bitmap.Save(stream, imageCodecInfo, encoderParameters);
					}
				}
				this.dirtyFlag = flag;
				if (panel != null)
				{
					this.RenderingMode = RenderingMode.All;
					this.PanelToRender = null;
				}
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

		public void SaveIntoMetafile(Stream imageStream)
		{
			Bitmap bitmap = new Bitmap(this.GetWidth(), this.GetHeight());
			Graphics graphics = Graphics.FromImage(bitmap);
			IntPtr hdc = graphics.GetHdc();
			Metafile metafile = new Metafile(imageStream, hdc, new Rectangle(0, 0, this.GetWidth(), this.GetHeight()), MetafileFrameUnit.Pixel);
			Graphics graphics2 = Graphics.FromImage(metafile);
			graphics2.SmoothingMode = this.GetSmootingMode();
			graphics2.TextRenderingHint = this.GetTextRenderingHint();
			this.Paint(graphics2, RenderingType.Gdi, imageStream, false);
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

		internal void SavePanelAsImage(Panel panel, string fileName, MapImageFormat format, int compression, bool zoomThumbOnly)
		{
			this.SaveTo(fileName, format, compression, panel, zoomThumbOnly);
		}

		internal void SavePanelAsImage(Panel panel, Stream stream, MapImageFormat format, int compression, bool zoomThumbOnly)
		{
			this.SaveTo(stream, format, compression, panel, zoomThumbOnly);
		}

		internal void SavePanelAsImage(Panel panel, Stream stream, bool zoomThumbOnly)
		{
			MapImageFormat imageFormat = (MapImageFormat)Enum.Parse(typeof(MapImageFormat), ((Enum)(object)this.ImageType).ToString((IFormatProvider)CultureInfo.CurrentCulture), true);
			this.SaveTo(stream, imageFormat, this.Compression, panel, zoomThumbOnly);
		}

		internal void SavePanelAsImage(Panel panel, string fileName, bool zoomThumbOnly)
		{
			MapImageFormat imageFormat = (MapImageFormat)Enum.Parse(typeof(MapImageFormat), ((Enum)(object)this.ImageType).ToString((IFormatProvider)CultureInfo.CurrentCulture), true);
			this.SaveTo(fileName, imageFormat, this.Compression, panel, zoomThumbOnly);
		}

		internal int GetSpatialElementCount()
		{
			int num = 0;
			foreach (Symbol symbol in this.Symbols)
			{
				if (symbol.ParentShape == "(none)")
				{
					num++;
				}
			}
			return this.Shapes.Count + this.Paths.Count + num;
		}

		internal int GetSpatialPointCount()
		{
			int num = 0;
			foreach (ISpatialElement path in this.Paths)
			{
				if (path.Points != null)
				{
					num += path.Points.Length;
				}
			}
			foreach (ISpatialElement path2 in this.Paths)
			{
				if (path2.Points != null)
				{
					num += path2.Points.Length;
				}
			}
			foreach (Symbol symbol in this.Symbols)
			{
				if (symbol.ParentShape == "(none)")
				{
					ISpatialElement spatialElement3 = symbol;
					if (spatialElement3.Points != null)
					{
						num += spatialElement3.Points.Length;
					}
				}
			}
			return num;
		}

		private Region GetClipRegion()
		{
			Region region = null;
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				foreach (HotRegion item in this.HotRegionList.List)
				{
					if (item.SelectedObject is Panel)
					{
						GraphicsPath[] array = item.Paths;
						foreach (GraphicsPath addingPath in array)
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
			return this.MapControl.IsDesignMode();
		}

		internal void DrawException(Graphics graphics, Exception e)
		{
			graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, this.GetWidth(), this.GetHeight());
			graphics.DrawRectangle(new Pen(Color.Red, 4f), 0, 0, this.GetWidth(), this.GetHeight());
			string text = "Group Exception: " + e.Message;
			string text2 = "";
			if (e.TargetSite != null)
			{
				object obj = text2;
				text2 = obj + "Site: " + e.TargetSite + "\r\n\r\n";
			}
			if (e.StackTrace != string.Empty)
			{
				text2 = text2 + "Stack Trace: \r\n" + e.StackTrace;
			}
			RectangleF layoutRectangle = new RectangleF(3f, 3f, (float)(this.GetWidth() - 6), (float)(this.GetHeight() - 6));
			StringFormat format = new StringFormat();
			SizeF sizeF = graphics.MeasureString(text, new Font("MS Sans Serif", 10f, FontStyle.Bold), (int)layoutRectangle.Width, format);
			graphics.DrawString(text, new Font("MS Sans Serif", 10f, FontStyle.Bold), new SolidBrush(Color.Black), layoutRectangle, format);
			layoutRectangle.Y += (float)(sizeF.Height + 5.0);
			graphics.DrawString(text2, new Font("MS Sans Serif", 8f), new SolidBrush(Color.Black), layoutRectangle, format);
		}

		internal void PrepareHitTest()
		{
			if (this.HotRegionList.List.Count == 0)
			{
				using (MemoryStream stream = new MemoryStream())
				{
					this.SaveTo(stream, MapImageFormat.Bmp, -1, null, false);
				}
			}
		}

		internal HitTestResult[] HitTest(int x, int y, Type[] objectTypes, bool returnMultipleElements)
		{
			if (objectTypes == null)
			{
				objectTypes = new Type[0];
			}
			ArrayList arrayList = new ArrayList();
			HotRegion[] array = this.HotRegionList.CheckHotRegions(x, y, objectTypes, false);
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
			if (!(element is IContentElement) && element != this)
			{
				throw new ArgumentException(SR.hot_region_error_support(element.Name));
			}
			int num = this.HotRegionList.FindHotRegionOfObject(element);
			if (num != -1)
			{
				return (HotRegion)this.HotRegionList.List[num];
			}
			throw new ArgumentException(SR.hot_region_error_initialize(element.Name));
		}

		internal double GetMaximumSimplificationResolution()
		{
			if (this.Width != 0 && this.Height != 0)
			{
				double num = this.MaximumPoint.X - this.MinimumPoint.X;
				double num2 = this.MaximumPoint.Y - this.MinimumPoint.Y;
				double val = num / (double)this.Width;
				double val2 = num2 / (double)this.Height;
				return Math.Min(val, val2);
			}
			return 0.0;
		}

		internal PointF PercentsToPixels(PointF pointInPercents)
		{
			MapGraphics mapGraphics = new MapGraphics(null);
			mapGraphics.SetPictureSize(this.Width, this.Height);
			return mapGraphics.GetAbsolutePoint(pointInPercents);
		}

		internal PointF PixelsToPercents(PointF pointInPixels)
		{
			MapGraphics mapGraphics = new MapGraphics(null);
			mapGraphics.SetPictureSize(this.Width, this.Height);
			return mapGraphics.GetRelativePoint(pointInPixels);
		}

		internal SizeF PercentsToPixels(SizeF sizeInPercents)
		{
			MapGraphics mapGraphics = new MapGraphics(null);
			mapGraphics.SetPictureSize(this.Width, this.Height);
			return mapGraphics.GetAbsoluteSize(sizeInPercents);
		}

		internal SizeF PixelsToPercents(SizeF sizeInPixels)
		{
			MapGraphics mapGraphics = new MapGraphics(null);
			mapGraphics.SetPictureSize(this.Width, this.Height);
			return mapGraphics.GetRelativeSize(sizeInPixels);
		}

		internal Point3D GeographicToPercents(MapPoint mapPoint)
		{
			return this.GeographicToPercents(mapPoint.X, mapPoint.Y);
		}

		internal Point3D GeographicToPercents(double longtitude, double latitude)
		{
			Point3D result = this.ApplyProjection(longtitude, latitude);
			MapBounds boundsAfterProjection = this.GetBoundsAfterProjection();
			result.X -= boundsAfterProjection.MinimumPoint.X;
			result.Y -= boundsAfterProjection.MinimumPoint.Y;
			double num = boundsAfterProjection.MaximumPoint.X - boundsAfterProjection.MinimumPoint.X;
			double num2 = boundsAfterProjection.MaximumPoint.Y - boundsAfterProjection.MinimumPoint.Y;
			if (num > 0.0)
			{
				result.X *= 100.0 / num;
			}
			else
			{
				result.X = 0.0;
			}
			if (num2 > 0.0)
			{
				result.Y *= 100.0 / num2;
			}
			else
			{
				result.Y = 0.0;
			}
			result.Y = 100.0 - result.Y;
			return result;
		}

		internal MapPoint PercentsToGeographic(MapPoint point)
		{
			return this.PercentsToGeographic(point.X, point.Y);
		}

		internal MapPoint PercentsToGeographic(double pointX, double pointY)
		{
			double num = 100.0 - pointY;
			MapBounds boundsAfterProjection = this.GetBoundsAfterProjection();
			double num2 = pointX * ((boundsAfterProjection.MaximumPoint.X - boundsAfterProjection.MinimumPoint.X) / 100.0);
			num *= (boundsAfterProjection.MaximumPoint.Y - boundsAfterProjection.MinimumPoint.Y) / 100.0;
			num2 += boundsAfterProjection.MinimumPoint.X;
			num += boundsAfterProjection.MinimumPoint.Y;
			return this.InverseProjection(num2, num);
		}

		internal MapPoint PixelsToGeographic(PointF pointInPixels)
		{
			return this.ContentToGeographic(this.PixelsToContent(pointInPixels));
		}

		internal PointF GeographicToPixels(MapPoint pointOnMap)
		{
			return this.ContentToPixels(this.GeographicToContent(pointOnMap));
		}

		internal MapPoint ContentToGeographic(PointF contentPoint)
		{
			SizeF contentSizeInPixels = this.Viewport.GetContentSizeInPixels();
			contentPoint.X *= (float)(100.0 / contentSizeInPixels.Width);
			contentPoint.Y *= (float)(100.0 / contentSizeInPixels.Height);
			contentPoint.X *= (float)(100.0 / this.Viewport.Zoom);
			contentPoint.Y *= (float)(100.0 / this.Viewport.Zoom);
			return this.PercentsToGeographic((double)contentPoint.X, (double)contentPoint.Y);
		}

		internal PointF GeographicToContent(MapPoint pointOnMap)
		{
			SizeF contentSizeInPixels = this.Viewport.GetContentSizeInPixels();
			Point3D point3D = this.GeographicToPercents(pointOnMap);
			point3D.X *= this.Viewport.Zoom / 100.0;
			point3D.Y *= this.Viewport.Zoom / 100.0;
			point3D.X *= contentSizeInPixels.Width / 100.0;
			point3D.Y *= contentSizeInPixels.Height / 100.0;
			return point3D.ToPointF();
		}

		internal PointF PixelsToContent(PointF pointInPixels)
		{
			PointF contentOffsetInPixels = this.Viewport.GetContentOffsetInPixels();
			pointInPixels.X -= contentOffsetInPixels.X + (float)this.Viewport.Margins.Left;
			pointInPixels.Y -= contentOffsetInPixels.Y + (float)this.Viewport.Margins.Top;
			return pointInPixels;
		}

		internal RectangleF PixelsToContent(RectangleF pixelsRect)
		{
			PointF contentOffsetInPixels = this.Viewport.GetContentOffsetInPixels();
			pixelsRect.Offset((float)(0.0 - (contentOffsetInPixels.X + (float)this.Viewport.Margins.Left)), (float)(0.0 - (contentOffsetInPixels.Y + (float)this.Viewport.Margins.Top)));
			return pixelsRect;
		}

		internal PointF ContentToPixels(PointF contentPoint)
		{
			PointF contentOffsetInPixels = this.Viewport.GetContentOffsetInPixels();
			contentPoint.X += contentOffsetInPixels.X + (float)this.Viewport.Margins.Left;
			contentPoint.Y += contentOffsetInPixels.Y + (float)this.Viewport.Margins.Top;
			return contentPoint;
		}

		internal RectangleF ContentToPixels(RectangleF contentRectangle)
		{
			PointF pointF = this.ContentToPixels(contentRectangle.Location);
			PointF pointF2 = this.ContentToPixels(new PointF(contentRectangle.X + contentRectangle.Width, contentRectangle.Y + contentRectangle.Height));
			return new RectangleF(pointF.X, pointF.Y, pointF2.X - pointF.X, pointF2.Y - pointF.Y);
		}

		internal bool IsContentImageMapRequired()
		{
			if (!this.ImageMapEnabled)
			{
				return false;
			}
			foreach (IImageMapProvider shape in this.Shapes)
			{
				string toolTip = shape.GetToolTip();
				string href = shape.GetHref();
				string mapAreaAttributes = shape.GetMapAreaAttributes();
				if (!string.IsNullOrEmpty(toolTip) || !string.IsNullOrEmpty(href) || !string.IsNullOrEmpty(mapAreaAttributes))
				{
					return true;
				}
			}
			foreach (IImageMapProvider path in this.Paths)
			{
				string toolTip2 = path.GetToolTip();
				string href2 = path.GetHref();
				string mapAreaAttributes2 = path.GetMapAreaAttributes();
				if (!string.IsNullOrEmpty(toolTip2) || !string.IsNullOrEmpty(href2) || !string.IsNullOrEmpty(mapAreaAttributes2))
				{
					return true;
				}
			}
			foreach (IImageMapProvider symbol in this.Symbols)
			{
				string toolTip3 = symbol.GetToolTip();
				string href3 = symbol.GetHref();
				string mapAreaAttributes3 = symbol.GetMapAreaAttributes();
				if (!string.IsNullOrEmpty(toolTip3) || !string.IsNullOrEmpty(href3) || !string.IsNullOrEmpty(mapAreaAttributes3))
				{
					return true;
				}
			}
			return false;
		}

		internal MapAreaCollection GetMapAreasFromHotRegionList()
		{
			MapAreaCollection mapAreaCollection = new MapAreaCollection();
			for (int num = this.HotRegionList.List.Count - 1; num >= 0; num--)
			{
				HotRegion hotRegion = (HotRegion)this.HotRegionList.List[num];
				if (hotRegion.SelectedObject is IImageMapProvider)
				{
					IImageMapProvider imageMapProvider = (IImageMapProvider)hotRegion.SelectedObject;
					string toolTip = imageMapProvider.GetToolTip();
					string href = imageMapProvider.GetHref();
					string mapAreaAttributes = imageMapProvider.GetMapAreaAttributes();
					if (!string.IsNullOrEmpty(toolTip) || !string.IsNullOrEmpty(href) || !string.IsNullOrEmpty(mapAreaAttributes))
					{
						RectangleF rectangleF = new RectangleF(Point.Empty, this.GridSectionSize);
						if (hotRegion.SelectedObject is Path)
						{
							for (int i = 0; i < hotRegion.Paths.Length; i++)
							{
								if (hotRegion.Paths[i] != null)
								{
									using (Pen pen = ((Path)hotRegion.SelectedObject).GetBorderPen())
									{
										if (pen != null)
										{
											if (pen.Width < 7.0)
											{
												pen.Width = 7f;
											}
											GraphicsPath graphicsPath = (GraphicsPath)hotRegion.Paths[i].Clone();
											graphicsPath.Widen(pen);
											graphicsPath.Flatten(null, 1f);
											if (rectangleF.IntersectsWith(graphicsPath.GetBounds()))
											{
												mapAreaCollection.Add(toolTip, href, mapAreaAttributes, graphicsPath);
											}
											graphicsPath.Dispose();
										}
									}
								}
							}
						}
						else if (hotRegion.SelectedObject is Shape)
						{
							for (int j = 0; j < hotRegion.Paths.Length; j++)
							{
								if (hotRegion.Paths[j] != null && rectangleF.IntersectsWith(hotRegion.Paths[j].GetBounds()))
								{
									PointF[] pathPoints = hotRegion.Paths[j].PathPoints;
									if (pathPoints.Length > 0)
									{
										int[] array = new int[pathPoints.Length * 2];
										int num2 = 0;
										bool flag = true;
										PointF[] array2 = pathPoints;
										for (int k = 0; k < array2.Length; k++)
										{
											PointF pt = array2[k];
											array[num2++] = (int)Math.Round((double)pt.X);
											array[num2++] = (int)Math.Round((double)pt.Y);
											if (flag && rectangleF.Contains(pt))
											{
												flag = false;
											}
										}
										if (flag)
										{
											PointF point = new PointF((float)(rectangleF.Width / 2.0), (float)(rectangleF.Height / 2.0));
											if (hotRegion.Paths[j].IsVisible(point))
											{
												int[] coord = new int[4]
												{
													-1,
													-1,
													this.GridSectionSize.Width + 1,
													this.GridSectionSize.Height + 1
												};
												mapAreaCollection.Add(MapAreaShape.Rectangle, toolTip, href, mapAreaAttributes, coord);
											}
										}
										else
										{
											mapAreaCollection.Add(MapAreaShape.Polygon, toolTip, href, mapAreaAttributes, array);
										}
									}
								}
							}
						}
						else if (hotRegion.SelectedObject is IContentElement)
						{
							for (int l = 0; l < hotRegion.Paths.Length; l++)
							{
								if (hotRegion.Paths[l] != null)
								{
									mapAreaCollection.Add(toolTip, href, mapAreaAttributes, hotRegion.Paths[l]);
								}
							}
						}
					}
				}
			}
			return mapAreaCollection;
		}

		internal void PopulateImageMaps()
		{
			for (int num = this.HotRegionList.List.Count - 1; num >= 0; num--)
			{
				HotRegion hotRegion = (HotRegion)this.HotRegionList.List[num];
				if (hotRegion.SelectedObject is IImageMapProvider)
				{
					IImageMapProvider imageMapProvider = (IImageMapProvider)hotRegion.SelectedObject;
					string toolTip = imageMapProvider.GetToolTip();
					string href = imageMapProvider.GetHref();
					string mapAreaAttributes = imageMapProvider.GetMapAreaAttributes();
					if (!string.IsNullOrEmpty(toolTip) || !string.IsNullOrEmpty(href) || !string.IsNullOrEmpty(mapAreaAttributes))
					{
						object tag = imageMapProvider.Tag;
						for (int i = 0; i < hotRegion.Paths.Length; i++)
						{
							if (hotRegion.Paths[i] != null)
							{
								if (hotRegion.SelectedObject is Path)
								{
									using (Pen pen = ((Path)hotRegion.SelectedObject).GetBorderPen())
									{
										if (pen != null)
										{
											GraphicsPath graphicsPath = (GraphicsPath)hotRegion.Paths[i].Clone();
											graphicsPath.Widen(pen);
											graphicsPath.Flatten(null, 1f);
											this.MapAreas.Add(toolTip, href, mapAreaAttributes, graphicsPath, tag);
											graphicsPath.Dispose();
										}
									}
								}
								else
								{
									this.MapAreas.Add(toolTip, href, mapAreaAttributes, hotRegion.Paths[i], tag);
								}
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
			output.Write("\r\n</MAP>");
		}

		internal void InvalidateDataBinding()
		{
			if (!this.dataBinding && !this.invalidatingDataBind && !this.isInitializing && !this.IsSuspended)
			{
				try
				{
					this.invalidatingDataBind = true;
					this.ShapeFields.Purge();
					this.GroupFields.Purge();
					this.SymbolFields.Purge();
					this.boundToDataSource = false;
				}
				finally
				{
					this.invalidatingDataBind = false;
				}
			}
		}

		internal void AutoDataBind(bool forceBinding)
		{
			if (!this.boundToDataSource)
			{
				try
				{
					this.dataBinding = true;
					if (this.DataSource != null || forceBinding)
					{
						foreach (DataBindingRuleBase dataBindingRule in this.DataBindingRules)
						{
							dataBindingRule.DataBind();
						}
						this.boundToDataSource = true;
					}
				}
				finally
				{
					this.dataBinding = false;
				}
			}
		}

		internal void DataBindShapes(object dataSource, string dataMember, string bindingField)
		{
			try
			{
				this.dataBinding = true;
				this.ExecuteDataBind(BindingType.Shapes, null, dataSource, dataMember, bindingField);
			}
			finally
			{
				this.dataBinding = false;
			}
		}

		internal void DataBindGroups(object dataSource, string dataMember, string bindingField)
		{
			try
			{
				this.dataBinding = true;
				this.ExecuteDataBind(BindingType.Groups, null, dataSource, dataMember, bindingField);
			}
			finally
			{
				this.dataBinding = false;
			}
		}

		internal void DataBindPaths(object dataSource, string dataMember, string bindingField)
		{
			try
			{
				this.dataBinding = true;
				this.ExecuteDataBind(BindingType.Paths, null, dataSource, dataMember, bindingField);
			}
			finally
			{
				this.dataBinding = false;
			}
		}

		internal void DataBindSymbols(object dataSource, string dataMember, string bindingField, string category, string parentShapeField, string xCoordinateField, string yCoordinateField)
		{
			try
			{
				if (string.IsNullOrEmpty(parentShapeField) && string.IsNullOrEmpty(xCoordinateField))
				{
					throw new ArgumentException(SR.symboldatabind_bad_loc_arg);
				}
				this.dataBinding = true;
				this.ExecuteDataBind(BindingType.Symbols, null, dataSource, dataMember, bindingField, category, parentShapeField, xCoordinateField, yCoordinateField);
			}
			finally
			{
				this.dataBinding = false;
			}
		}

		internal void ExecuteDataBind(BindingType bindingType, DataBindingRuleBase dataBinding, object dataSource, string dataMember, string bindingField, params string[] auxFields)
		{
			bool flag = false;
			bool flag2 = false;
			IDbConnection dbConnection = null;
			bool dummyData = false;
			IEnumerable enumerable = null;
			try
			{
				if (!this.IsDesignMode() && (dataSource == null || bindingField == null || bindingField == string.Empty))
				{
					throw new ArgumentException(SR.bad_data_src_fields);
				}
				if (dataSource is IDesignTimeDataSource)
				{
					dummyData = true;
				}
				this.OnBeforeDataBind(new DataBindEventArgs(dataBinding));
				flag2 = true;
				DataBindingTargetResolver dataBindingTargetResolver = null;
				switch (bindingType)
				{
				case BindingType.Shapes:
					dataBindingTargetResolver = new DataBindingTargetResolver(this.ShapeFields, this.Shapes);
					break;
				case BindingType.Groups:
					dataBindingTargetResolver = new DataBindingTargetResolver(this.GroupFields, this.Groups);
					break;
				case BindingType.Paths:
					dataBindingTargetResolver = new DataBindingTargetResolver(this.PathFields, this.Paths);
					break;
				case BindingType.Symbols:
					if (auxFields.Length != 4)
					{
						if (this.IsDesignMode())
						{
							return;
						}
						throw new ArgumentException(SR.symbol_databinding_not_enough_params);
					}
					dataBindingTargetResolver = new DataBindingTargetResolver(this.SymbolFields, this.Symbols);
					break;
				}
				ArrayList arrayList = null;
				if (!string.IsNullOrEmpty(bindingField))
				{
					arrayList = DataBindingHelper.GetDataSourceDataFields(dataSource, dataMember, bindingField);
					foreach (DataBindingHelper.DataFieldDescriptor item in arrayList)
					{
						string name = item.Name;
						Type type = Field.ConvertToSupportedType(item.Type);
						if (name != bindingField && !dataBindingTargetResolver.ContainsField(name))
						{
							Field field = new Field();
							field.Name = name;
							field.Type = type;
							field.IsTemporary = true;
							dataBindingTargetResolver.AddField(field);
						}
					}
				}
				else
				{
					arrayList = new ArrayList();
				}
				enumerable = DataBindingHelper.GetDataSourceAsIEnumerable(dataSource, dataMember, out flag, out dbConnection);
				switch (bindingType)
				{
				case BindingType.Shapes:
				case BindingType.Groups:
				case BindingType.Paths:
					this.ExecuteFieldDataBind(enumerable, bindingField, arrayList, dataBindingTargetResolver, dummyData);
					break;
				case BindingType.Symbols:
					this.ExecuteSymbolDataBind(enumerable, bindingField, arrayList, dataBindingTargetResolver, auxFields[0], auxFields[1], auxFields[2], auxFields[3], dummyData);
					break;
				}
			}
			finally
			{
				if (flag && enumerable != null)
				{
					((IDataReader)enumerable).Close();
				}
				if (dbConnection != null)
				{
					dbConnection.Close();
				}
				if (flag2)
				{
					this.OnAfterDataBind(new DataBindEventArgs(dataBinding));
				}
				this.Invalidate();
			}
		}

		private void ExecuteFieldDataBind(IEnumerable dataEnumerator, string bindingField, ArrayList columnDescrs, DataBindingTargetResolver targetResolver, bool dummyData)
		{
			object obj = null;
			int num = 0;
			if (columnDescrs.Count != 0)
			{
				foreach (object item in dataEnumerator)
				{
					try
					{
						obj = DataBindingHelper.ConvertEnumerationItem(item, bindingField);
					}
					catch
					{
						if (!this.IsDesignMode())
						{
							throw;
						}
						obj = null;
					}
					NamedElement namedElement = null;
					if (!dummyData)
					{
						namedElement = targetResolver.GetItemById(obj);
					}
					else if (obj != null)
					{
						namedElement = targetResolver.GetItemByIndex(num++);
					}
					if (namedElement != null)
					{
						foreach (DataBindingHelper.DataFieldDescriptor columnDescr in columnDescrs)
						{
							object obj3 = null;
							Type type = Field.ConvertToSupportedType(columnDescr.Type);
							string name = columnDescr.Name;
							try
							{
								obj3 = DataBindingHelper.ConvertEnumerationItem(item, name);
							}
							catch
							{
								if (this.IsDesignMode())
								{
									goto end_IL_00a3;
								}
								throw;
								end_IL_00a3:;
							}
							if (obj3 != null && !Convert.IsDBNull(obj3))
							{
								obj3 = Field.ConvertToSupportedValue(obj3);
								obj3.GetType();
								Field fieldByName = targetResolver.GetFieldByName(name);
								if (!(name.ToUpper(CultureInfo.CurrentCulture) == "NAME") && fieldByName != null)
								{
									if (fieldByName.Type != type)
									{
										if (!this.IsDesignMode())
										{
											throw new InvalidOperationException(SR.field_duplication(name, ((Enum)(object)targetResolver.BindingType).ToString((IFormatProvider)CultureInfo.CurrentCulture), fieldByName.Type.Name, type.Name));
										}
									}
									else if (!dummyData || fieldByName.IsTemporary)
									{
										targetResolver.SetFieldValue(namedElement, name, obj3);
									}
								}
							}
						}
					}
				}
			}
		}

		private void ExecuteSymbolDataBind(IEnumerable dataEnumerator, string bindingField, ArrayList columnDescrs, DataBindingTargetResolver targetResolver, string category, string parentShapeField, string xCoordinateField, string yCoordinateField, bool dummyData)
		{
			if (string.IsNullOrEmpty(parentShapeField) && string.IsNullOrEmpty(xCoordinateField))
			{
				if (this.IsDesignMode())
				{
					return;
				}
				throw new ArgumentException(SR.symboldatabind_bad_loc_arg);
			}
			object obj = null;
			DataBindingTargetResolver dataBindingTargetResolver = null;
			int num = 0;
			if (columnDescrs.Count != 0)
			{
				if (!string.IsNullOrEmpty(parentShapeField))
				{
					dataBindingTargetResolver = new DataBindingTargetResolver(this.ShapeFields, this.Shapes);
				}
				foreach (object item in dataEnumerator)
				{
					try
					{
						obj = DataBindingHelper.ConvertEnumerationItem(item, bindingField);
					}
					catch
					{
						if (!this.IsDesignMode())
						{
							throw;
						}
						obj = null;
					}
					Symbol symbol = null;
					if (!dummyData)
					{
						symbol = (Symbol)targetResolver.GetItemById(obj);
					}
					else if (obj != null)
					{
						symbol = (Symbol)targetResolver.GetItemByIndex(num++);
					}
					if (symbol != null)
					{
						if (!this.IsDesignMode())
						{
							symbol.Category = category;
						}
						foreach (DataBindingHelper.DataFieldDescriptor columnDescr in columnDescrs)
						{
							object obj3 = null;
							Type type = Field.ConvertToSupportedType(columnDescr.Type);
							string name = columnDescr.Name;
							try
							{
								obj3 = DataBindingHelper.ConvertEnumerationItem(item, name);
							}
							catch
							{
								if (this.IsDesignMode())
								{
									goto end_IL_0105;
								}
								throw;
								end_IL_0105:;
							}
							if (obj3 != null && !Convert.IsDBNull(obj3))
							{
								if (columnDescr.Name == parentShapeField && !this.IsDesignMode())
								{
									Shape shape = dataBindingTargetResolver.GetItemById(obj3) as Shape;
									if (shape != null)
									{
										symbol.ParentShape = shape.Name;
									}
								}
								else if (columnDescr.Name == xCoordinateField && !this.IsDesignMode())
								{
									if (!DataBindingHelper.IsValidAsCoordinateType(columnDescr.Type))
									{
										if (this.IsDesignMode())
										{
											return;
										}
										throw new NotSupportedException(SR.symbol_coord_type(columnDescr.Type.Name));
									}
									if (columnDescr.Type == typeof(string))
									{
										if (xCoordinateField != yCoordinateField)
										{
											symbol.X = obj3.ToString();
										}
										else
										{
											symbol.SetCoordinates(obj3.ToString());
										}
									}
									else
									{
										try
										{
											symbol.X = Convert.ToDouble(obj3, CultureInfo.CurrentCulture);
										}
										catch (InvalidCastException)
										{
											if (this.IsDesignMode())
											{
												return;
											}
											throw new NotSupportedException(SR.symbol_coord_type(columnDescr.Type.Name));
										}
										catch
										{
											if (this.IsDesignMode())
											{
												return;
											}
											throw;
										}
									}
									continue;
								}
								if (columnDescr.Name == yCoordinateField && !this.IsDesignMode())
								{
									if (!DataBindingHelper.IsValidAsCoordinateType(columnDescr.Type))
									{
										if (this.IsDesignMode())
										{
											return;
										}
										throw new NotSupportedException(SR.symbol_coord_type(columnDescr.Type.Name));
									}
									if (columnDescr.Type == typeof(string))
									{
										if (xCoordinateField != yCoordinateField)
										{
											symbol.Y = obj3.ToString();
										}
									}
									else
									{
										try
										{
											symbol.Y = Convert.ToDouble(obj3, CultureInfo.InvariantCulture);
										}
										catch (InvalidCastException)
										{
											if (this.IsDesignMode())
											{
												return;
											}
											throw new NotSupportedException(SR.symbol_coord_type(columnDescr.Type.Name));
										}
										catch
										{
											if (this.IsDesignMode())
											{
												return;
											}
											throw;
										}
									}
								}
								else
								{
									obj3 = Field.ConvertToSupportedValue(obj3);
									obj3.GetType();
									Field fieldByName = targetResolver.GetFieldByName(name);
									if (!(name.ToUpper(CultureInfo.CurrentCulture) == "NAME") && fieldByName != null)
									{
										if (fieldByName.Type != type)
										{
											if (!this.IsDesignMode())
											{
												throw new InvalidOperationException(SR.field_duplication(name, ((Enum)(object)targetResolver.BindingType).ToString((IFormatProvider)CultureInfo.CurrentCulture), fieldByName.Type.Name, type.Name));
											}
										}
										else if (!dummyData || fieldByName.IsTemporary)
										{
											targetResolver.SetFieldValue(symbol, name, obj3);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private void OnBeforeDataBind(DataBindEventArgs e)
		{
			this.MapControl.OnBeforeDataBind(e);
		}

		private void OnAfterDataBind(DataBindEventArgs e)
		{
			this.MapControl.OnAfterDataBind(e);
		}

		internal string ResolveAllKeywords(string original, NamedElement element)
		{
			if (original.Length == 0)
			{
				return original;
			}
			string text = original.Replace("\\n", "\n");
			if (element is Group)
			{
				SortedList sortedList = new SortedList(new StringLengthReversedComparer());
				foreach (Field groupField in this.GroupFields)
				{
					sortedList.Add(groupField.GetKeyword(), groupField.Name);
				}
				IDictionaryEnumerator enumerator2 = sortedList.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator2.Current;
						object val = ((Group)element)[(string)dictionaryEntry.Value];
						text = this.ResolveKeyword(text, (string)dictionaryEntry.Key, val);
					}
				}
				finally
				{
					IDisposable disposable = enumerator2 as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				text = this.ResolveKeyword(text, "#NAME", element.Name);
			}
			else if (element is Shape)
			{
				SortedList sortedList2 = new SortedList(new StringLengthReversedComparer());
				foreach (Field shapeField in this.ShapeFields)
				{
					sortedList2[shapeField.GetKeyword()] = shapeField.Name;
				}
				IDictionaryEnumerator enumerator4 = sortedList2.GetEnumerator();
				try
				{
					while (enumerator4.MoveNext())
					{
						DictionaryEntry dictionaryEntry2 = (DictionaryEntry)enumerator4.Current;
						object val2 = ((Shape)element)[(string)dictionaryEntry2.Value];
						text = this.ResolveKeyword(text, (string)dictionaryEntry2.Key, val2);
					}
				}
				finally
				{
					IDisposable disposable2 = enumerator4 as IDisposable;
					if (disposable2 != null)
					{
						disposable2.Dispose();
					}
				}
				text = this.ResolveKeyword(text, "#NAME", element.Name);
			}
			else if (element is Path)
			{
				SortedList sortedList3 = new SortedList(new StringLengthReversedComparer());
				foreach (Field pathField in this.PathFields)
				{
					sortedList3.Add(pathField.GetKeyword(), pathField.Name);
				}
				IDictionaryEnumerator enumerator6 = sortedList3.GetEnumerator();
				try
				{
					while (enumerator6.MoveNext())
					{
						DictionaryEntry dictionaryEntry3 = (DictionaryEntry)enumerator6.Current;
						object val3 = ((Path)element)[(string)dictionaryEntry3.Value];
						text = this.ResolveKeyword(text, (string)dictionaryEntry3.Key, val3);
					}
				}
				finally
				{
					IDisposable disposable3 = enumerator6 as IDisposable;
					if (disposable3 != null)
					{
						disposable3.Dispose();
					}
				}
				text = this.ResolveKeyword(text, "#NAME", element.Name);
			}
			else if (element is Symbol)
			{
				SortedList sortedList4 = new SortedList(new StringLengthReversedComparer());
				foreach (Field symbolField in this.SymbolFields)
				{
					sortedList4.Add(symbolField.GetKeyword(), symbolField.Name);
				}
				IDictionaryEnumerator enumerator8 = sortedList4.GetEnumerator();
				try
				{
					while (enumerator8.MoveNext())
					{
						DictionaryEntry dictionaryEntry4 = (DictionaryEntry)enumerator8.Current;
						object val4 = ((Symbol)element)[(string)dictionaryEntry4.Value];
						text = this.ResolveKeyword(text, (string)dictionaryEntry4.Key, val4);
					}
				}
				finally
				{
					IDisposable disposable4 = enumerator8 as IDisposable;
					if (disposable4 != null)
					{
						disposable4.Dispose();
					}
				}
				text = this.ResolveKeyword(text, "#NAME", element.Name);
			}
			return text;
		}

		internal string ResolveKeyword(string original, string keyword, object val)
		{
			string text = original;
			if (val == null)
			{
				val = "(null)";
			}
			for (int num = text.IndexOf(keyword + "{", StringComparison.Ordinal); num != -1; num = text.IndexOf(keyword + "{", num + 1, StringComparison.Ordinal))
			{
				int num2 = text.IndexOf("{", num, StringComparison.Ordinal);
				int num3 = text.IndexOf("}", num2, StringComparison.Ordinal);
				if (num3 == -1)
				{
					throw new InvalidOperationException(SR.ExceptionInvalidKeywordFormat(text));
				}
				string text2 = text.Substring(num2, num3 - num2 + 1);
				string text3;
				if (this.MapControl.FormatNumberHandler != null)
				{
					text3 = this.MapControl.FormatNumberHandler(this.MapControl, val, text2.Trim('{', '}'));
				}
				else
				{
					string format = text2.Replace("{", "{0:");
					text3 = string.Format(CultureInfo.CurrentCulture, format, val);
				}
				if (text3.Length > 80)
				{
					text3 = text3.Substring(0, 80) + "...";
				}
				text = text.Replace(keyword + text2, text3);
			}
			string text4 = val.ToString();
			if (text4.Length > 80)
			{
				text4 = text4.Substring(0, 80) + "...";
			}
			return text.Replace(keyword, text4);
		}

		internal static bool CheckLicense()
		{
			return true;
		}

		private RectangleF PerformPanelLayout(MapGraphics g, RectangleF bounds, List<DockablePanel> sortedPanels)
		{
			if (sortedPanels.Count == 0)
			{
				return bounds;
			}
			RectangleF a = bounds;
			float top = bounds.Top;
			float bottom = bounds.Bottom;
			float top2 = bounds.Top;
			float bottom2 = bounds.Bottom;
			float left = bounds.Left;
			float right = bounds.Right;
			float left2 = bounds.Left;
			float right2 = bounds.Right;
			float top3 = bounds.Top;
			ArrayList arrayList = new ArrayList();
			float bottom3 = bounds.Bottom;
			ArrayList arrayList2 = new ArrayList();
			float left3 = bounds.Left;
			ArrayList arrayList3 = new ArrayList();
			float right3 = bounds.Right;
			ArrayList arrayList4 = new ArrayList();
			RectangleF b = bounds;
			foreach (DockablePanel sortedPanel in sortedPanels)
			{
				if (sortedPanel is AutoSizePanel)
				{
					((AutoSizePanel)sortedPanel).AdjustAutoSize(g);
				}
				RectangleF rectangleF = MapGraphics.Round(sortedPanel.GetBoundsInPixels());
				bool flag = rectangleF.Width <= rectangleF.Height;
				switch (sortedPanel.Dock)
				{
				case PanelDockStyle.Top:
					if (sortedPanel.DockAlignment == DockAlignment.Near)
					{
						this.LayoutAlignPanel(sortedPanel, bounds, ref a, left);
					}
					else if (sortedPanel.DockAlignment == DockAlignment.Far)
					{
						this.LayoutAlignPanel(sortedPanel, bounds, ref a, right);
					}
					else if (flag)
					{
						arrayList.Add(sortedPanel);
					}
					else
					{
						this.LayoutCenteredPanels(arrayList, true, bounds, ref b);
						arrayList.Clear();
						arrayList.Add(sortedPanel);
						this.LayoutCenteredPanels(arrayList, true, bounds, ref b);
						arrayList.Clear();
					}
					break;
				case PanelDockStyle.Bottom:
					if (sortedPanel.DockAlignment == DockAlignment.Near)
					{
						this.LayoutAlignPanel(sortedPanel, bounds, ref a, left2);
					}
					else if (sortedPanel.DockAlignment == DockAlignment.Far)
					{
						this.LayoutAlignPanel(sortedPanel, bounds, ref a, right2);
					}
					else if (flag)
					{
						arrayList2.Add(sortedPanel);
					}
					else
					{
						this.LayoutCenteredPanels(arrayList2, true, bounds, ref b);
						arrayList2.Clear();
						arrayList2.Add(sortedPanel);
						this.LayoutCenteredPanels(arrayList2, true, bounds, ref b);
						arrayList2.Clear();
					}
					break;
				case PanelDockStyle.Left:
					if (sortedPanel.DockAlignment == DockAlignment.Near)
					{
						this.LayoutAlignPanel(sortedPanel, bounds, ref a, top);
					}
					else if (sortedPanel.DockAlignment == DockAlignment.Far)
					{
						this.LayoutAlignPanel(sortedPanel, bounds, ref a, bottom);
					}
					else if (!flag)
					{
						arrayList3.Add(sortedPanel);
					}
					else
					{
						this.LayoutCenteredPanels(arrayList3, false, bounds, ref b);
						arrayList3.Clear();
						arrayList3.Add(sortedPanel);
						this.LayoutCenteredPanels(arrayList3, false, bounds, ref b);
						arrayList3.Clear();
					}
					break;
				case PanelDockStyle.Right:
					if (sortedPanel.DockAlignment == DockAlignment.Near)
					{
						this.LayoutAlignPanel(sortedPanel, bounds, ref a, top2);
					}
					else if (sortedPanel.DockAlignment == DockAlignment.Far)
					{
						this.LayoutAlignPanel(sortedPanel, bounds, ref a, bottom2);
					}
					else if (!flag)
					{
						arrayList4.Add(sortedPanel);
					}
					else
					{
						this.LayoutCenteredPanels(arrayList4, false, bounds, ref b);
						arrayList4.Clear();
						arrayList4.Add(sortedPanel);
						this.LayoutCenteredPanels(arrayList4, false, bounds, ref b);
						arrayList4.Clear();
					}
					break;
				}
			}
			this.LayoutCenteredPanels(arrayList, true, bounds, ref b);
			this.LayoutCenteredPanels(arrayList2, true, bounds, ref b);
			this.LayoutCenteredPanels(arrayList3, false, bounds, ref b);
			this.LayoutCenteredPanels(arrayList4, false, bounds, ref b);
			return RectangleF.Intersect(a, b);
		}

		private RectangleF LayoutAlignPanel(DockablePanel panel, RectangleF layoutBounds, ref RectangleF unoccupiedArea, float position)
		{
			bool flag = panel.Dock == PanelDockStyle.Top || panel.Dock == PanelDockStyle.Bottom;
			RectangleF result = MapGraphics.Round(panel.GetBoundsInPixels());
			switch (panel.Dock)
			{
			case PanelDockStyle.Top:
				result.Y = layoutBounds.Top;
				break;
			case PanelDockStyle.Bottom:
				result.Y = layoutBounds.Bottom - result.Height;
				break;
			case PanelDockStyle.Left:
				result.X = layoutBounds.Left;
				break;
			case PanelDockStyle.Right:
				result.X = layoutBounds.Right - result.Width;
				break;
			}
			switch (panel.DockAlignment)
			{
			case DockAlignment.Near:
			case DockAlignment.Center:
				if (flag)
				{
					result.X = position;
				}
				else
				{
					result.Y = position;
				}
				break;
			case DockAlignment.Far:
				if (flag)
				{
					result.X = position - result.Width;
				}
				else
				{
					result.Y = position - result.Height;
				}
				break;
			}
			panel.SetLocationInPixels(result.Location);
			switch (panel.Dock)
			{
			case PanelDockStyle.Top:
				if (result.Bottom > unoccupiedArea.Top)
				{
					unoccupiedArea.Height -= result.Bottom - unoccupiedArea.Top;
					unoccupiedArea.Y = result.Bottom;
				}
				break;
			case PanelDockStyle.Bottom:
				if (result.Top < unoccupiedArea.Bottom)
				{
					unoccupiedArea.Height -= unoccupiedArea.Bottom - result.Top;
				}
				break;
			case PanelDockStyle.Left:
				if (result.Right > unoccupiedArea.Left)
				{
					unoccupiedArea.Width -= result.Right - unoccupiedArea.Left;
					unoccupiedArea.X = result.Right;
				}
				break;
			case PanelDockStyle.Right:
				if (result.Left < unoccupiedArea.Right)
				{
					unoccupiedArea.Width -= unoccupiedArea.Right - result.Left;
				}
				break;
			}
			return result;
		}

		private void LayoutCenteredPanels(ArrayList panels, bool horizAlignment, RectangleF layoutBounds, ref RectangleF unoccupiedArea)
		{
			if (panels.Count != 0)
			{
				float num = 0f;
				RectangleF layoutBounds2 = layoutBounds;
				foreach (Panel panel3 in panels)
				{
					RectangleF rectangleF = MapGraphics.Round(panel3.GetBoundsInPixels());
					num += (horizAlignment ? rectangleF.Width : rectangleF.Height);
				}
				float num2 = 0f;
				num2 = (float)((!horizAlignment) ? (layoutBounds2.Y + (layoutBounds2.Height - num) / 2.0) : (layoutBounds2.X + (layoutBounds2.Width - num) / 2.0));
				foreach (DockablePanel panel4 in panels)
				{
					num2 = (float)(int)Math.Round((double)num2);
					this.LayoutAlignPanel(panel4, layoutBounds2, ref unoccupiedArea, num2);
				}
			}
		}

		internal void LayoutPanels(MapGraphics g)
		{
			if (this.DoPanelLayout)
			{
				this.MapDockBounds = this.CalculateMapDockBounds(g);
				try
				{
					if (!this.MapDockBounds.IsEmpty)
					{
						List<DockablePanel> list = new List<DockablePanel>();
						List<DockablePanel> list2 = new List<DockablePanel>();
						Panel[] array = this.GetSortedPanels();
						foreach (Panel panel in array)
						{
							DockablePanel dockablePanel = panel as DockablePanel;
							if (dockablePanel != null && dockablePanel.IsVisible())
							{
								if (dockablePanel.Dock != 0)
								{
									if (dockablePanel.DockedInsideViewport)
									{
										list.Add(dockablePanel);
									}
									else
									{
										list2.Add(dockablePanel);
									}
								}
								else if (panel is AutoSizePanel)
								{
									((AutoSizePanel)panel).AdjustAutoSize(g);
								}
							}
						}
						RectangleF rectangleF = MapGraphics.Round(this.MapDockBounds);
						RectangleF boundsInPixels = this.PerformPanelLayout(g, rectangleF, list2);
						this.SeparateOverlappingPanels(list2, rectangleF, ref boundsInPixels);
						if (this.Viewport.AutoSize)
						{
							this.Viewport.SetBoundsInPixels(boundsInPixels);
						}
						RectangleF rectangleF2 = MapGraphics.Round(this.Viewport.GetBoundsInPixels());
						rectangleF2.X += (float)this.Viewport.Margins.Left;
						rectangleF2.Y += (float)this.Viewport.Margins.Top;
						rectangleF2.Width -= (float)(this.Viewport.Margins.Left + this.Viewport.Margins.Right);
						rectangleF2.Height -= (float)(this.Viewport.Margins.Top + this.Viewport.Margins.Bottom);
						rectangleF2.Inflate((float)(-this.Viewport.BorderWidth), (float)(-this.Viewport.BorderWidth));
						this.PerformPanelLayout(g, rectangleF2, list);
						this.SeparateOverlappingPanels(list, rectangleF2, ref rectangleF2);
					}
				}
				finally
				{
					this.DoPanelLayout = false;
				}
			}
		}

		internal RectangleF CalculateMapDockBounds(MapGraphics g)
		{
			RectangleF result = RectangleF.Empty;
			if (this.Frame.FrameStyle != 0)
			{
				result = g.GetBorder3DAdjustedRect(this.Frame);
			}
			else if (this.BorderLineWidth > 0)
			{
				result = new RectangleF(0f, 0f, (float)this.Width, (float)this.Height);
				result.Inflate((float)(-this.BorderLineWidth), (float)(-this.BorderLineWidth));
			}
			else
			{
				result = new RectangleF(0f, 0f, (float)this.Width, (float)this.Height);
			}
			return result;
		}

		private void SeparateOverlappingPanels(List<DockablePanel> panels, RectangleF availableSpace, ref RectangleF viewportRect)
		{
			int num = 0;
			int num2 = panels.Count * panels.Count;
			Hashtable hashtable = new Hashtable();
			DockablePanel dockablePanel = default(DockablePanel);
			DockablePanel dockablePanel2 = default(DockablePanel);
			while (this.FindFirstOverlappingPanelPair(panels, hashtable, out dockablePanel, out dockablePanel2) && num++ < num2)
			{
				if (!this.MovePanelToAvoidOverlap(panels, hashtable, dockablePanel2, dockablePanel, availableSpace, ref viewportRect))
				{
					hashtable.Add(new PanelPair(dockablePanel, dockablePanel2), null);
				}
			}
		}

		private bool FindFirstOverlappingPanelPair(List<DockablePanel> panels, Hashtable unavoidableOverlaps, out DockablePanel intersectingPanel, out DockablePanel panelToMove)
		{
			for (int i = 0; i < panels.Count - 1; i++)
			{
				RectangleF rectangleF = MapGraphics.Round(panels[i].GetBoundsInPixels());
				for (int j = i + 1; j < panels.Count; j++)
				{
					RectangleF rect = MapGraphics.Round(panels[j].GetBoundsInPixels());
					if (rectangleF.IntersectsWith(rect) && !unavoidableOverlaps.ContainsKey(new PanelPair(panels[i], panels[j])))
					{
						intersectingPanel = panels[i];
						panelToMove = panels[j];
						return true;
					}
				}
			}
			intersectingPanel = null;
			panelToMove = null;
			return false;
		}

		private bool MovePanelToAvoidOverlap(List<DockablePanel> panels, Hashtable unavoidableOverlaps, DockablePanel panelToMove, DockablePanel intersectingPanel, RectangleF availableSpace, ref RectangleF viewportRect)
		{
			RectangleF rectangleF = MapGraphics.Round(panelToMove.GetBoundsInPixels());
			RectangleF rectangleF2 = MapGraphics.Round(intersectingPanel.GetBoundsInPixels());
			RectangleF newRect = rectangleF;
			if (panelToMove.Dock == PanelDockStyle.Left || panelToMove.Dock == PanelDockStyle.Right)
			{
				newRect.Y = rectangleF2.Y + rectangleF2.Height;
				if (this.ValidateNewPosition(panels, unavoidableOverlaps, panelToMove, intersectingPanel, newRect, availableSpace, true))
				{
					this.SetNewPanelPosition(panelToMove, newRect, ref viewportRect);
					return true;
				}
				newRect = rectangleF;
				newRect.Y = rectangleF2.Y - rectangleF.Height;
				if (this.ValidateNewPosition(panels, unavoidableOverlaps, panelToMove, intersectingPanel, newRect, availableSpace, true))
				{
					this.SetNewPanelPosition(panelToMove, newRect, ref viewportRect);
					return true;
				}
				newRect = rectangleF;
				if (panelToMove.Dock == PanelDockStyle.Left)
				{
					newRect.X = rectangleF2.X + rectangleF2.Width;
				}
				else
				{
					newRect.X = rectangleF2.X - rectangleF.Width;
				}
				if (this.ValidateNewPosition(panels, unavoidableOverlaps, panelToMove, intersectingPanel, newRect, availableSpace, false))
				{
					this.SetNewPanelPosition(panelToMove, newRect, ref viewportRect);
					return true;
				}
			}
			else if (panelToMove.Dock == PanelDockStyle.Top || panelToMove.Dock == PanelDockStyle.Bottom)
			{
				newRect.X = rectangleF2.X + rectangleF2.Width;
				if (this.ValidateNewPosition(panels, unavoidableOverlaps, panelToMove, intersectingPanel, newRect, availableSpace, true))
				{
					this.SetNewPanelPosition(panelToMove, newRect, ref viewportRect);
					return true;
				}
				newRect = rectangleF;
				newRect.X = rectangleF2.X - rectangleF.Width;
				if (this.ValidateNewPosition(panels, unavoidableOverlaps, panelToMove, intersectingPanel, newRect, availableSpace, true))
				{
					this.SetNewPanelPosition(panelToMove, newRect, ref viewportRect);
					return true;
				}
				newRect = rectangleF;
				if (panelToMove.Dock == PanelDockStyle.Top)
				{
					newRect.Y = rectangleF2.Y + rectangleF2.Height;
				}
				else
				{
					newRect.Y = rectangleF2.Y - rectangleF.Height;
				}
				if (this.ValidateNewPosition(panels, unavoidableOverlaps, panelToMove, intersectingPanel, newRect, availableSpace, false))
				{
					this.SetNewPanelPosition(panelToMove, newRect, ref viewportRect);
					return true;
				}
			}
			return false;
		}

		private bool ValidateNewPosition(List<DockablePanel> panels, Hashtable unavoidableOverlaps, DockablePanel panelToMove, DockablePanel intersectingPanel, RectangleF newRect, RectangleF availableSpace, bool checkPreviousPanels)
		{
			if (!MapGraphics.Round(availableSpace).Contains(newRect))
			{
				return false;
			}
			if (checkPreviousPanels)
			{
				foreach (DockablePanel panel in panels)
				{
					if (panel == panelToMove)
					{
						return true;
					}
					if (!unavoidableOverlaps.ContainsKey(new PanelPair(panel, panelToMove)) && newRect.IntersectsWith(MapGraphics.Round(panel.GetBoundsInPixels())))
					{
						return false;
					}
				}
			}
			return true;
		}

		private void SetNewPanelPosition(DockablePanel panelToMove, RectangleF newRect, ref RectangleF viewportRect)
		{
			panelToMove.SetLocationInPixels(newRect.Location);
			RectangleF rectangleF = RectangleF.Intersect(newRect, viewportRect);
			if (panelToMove.Dock == PanelDockStyle.Left || panelToMove.Dock == PanelDockStyle.Right)
			{
				if (rectangleF.Width > 0.0)
				{
					float num;
					if (panelToMove.Dock == PanelDockStyle.Left)
					{
						num = newRect.Right - viewportRect.Left;
						viewportRect.X += num;
					}
					else
					{
						num = viewportRect.Right - newRect.Left;
					}
					viewportRect.Width -= num;
					viewportRect.Width = Math.Max(viewportRect.Width, 0f);
				}
			}
			else if (rectangleF.Height > 0.0)
			{
				float num2;
				if (panelToMove.Dock == PanelDockStyle.Top)
				{
					num2 = newRect.Bottom - viewportRect.Top;
					viewportRect.Y += num2;
				}
				else
				{
					num2 = viewportRect.Bottom - newRect.Top;
				}
				viewportRect.Height -= num2;
				viewportRect.Height = Math.Max(viewportRect.Height, 0f);
			}
		}

		internal void LoadShapesFromStream(Stream stream)
		{
			stream.Seek(0L, SeekOrigin.Begin);
			foreach (Shape shape in this.Shapes)
			{
				shape.ShapeData.LoadFromStream(stream);
			}
			this.InvalidateCachedBounds();
		}

		internal void SaveShapesToStream(Stream stream)
		{
			foreach (Shape shape in this.Shapes)
			{
				shape.ShapeData.SaveToStream(stream);
			}
		}

		internal void LoadPathsFromStream(Stream stream)
		{
			stream.Seek(0L, SeekOrigin.Begin);
			foreach (Path path in this.Paths)
			{
				path.PathData.LoadFromStream(stream);
			}
			this.InvalidateCachedBounds();
		}

		internal void SavePathsToStream(Stream stream)
		{
			foreach (Path path in this.Paths)
			{
				path.PathData.SaveToStream(stream);
			}
		}

		internal void LoadSymbolsFromStream(Stream stream)
		{
			stream.Seek(0L, SeekOrigin.Begin);
			foreach (Symbol symbol in this.Symbols)
			{
				symbol.SymbolData.LoadFromStream(stream);
			}
			this.InvalidateCachedBounds();
		}

		internal void SaveSymbolsToStream(Stream stream)
		{
			foreach (Symbol symbol in this.Symbols)
			{
				symbol.SymbolData.SaveToStream(stream);
			}
		}

		internal void CreateGroups(string shapeFieldName)
		{
			this.CreateGroups(shapeFieldName, string.Empty, string.Empty);
		}

		internal void CreateGroups(string shapeFieldName, string layer, string category)
		{
			Field field = (Field)this.ShapeFields.GetByName(shapeFieldName);
			if (field == null)
			{
				throw new ArgumentException(SR.ExceptionShapeFieldNotFound(shapeFieldName));
			}
			Hashtable hashtable = new Hashtable();
			foreach (Shape shape in this.Shapes)
			{
				object obj = shape[field.Name];
				if (obj != null)
				{
					string text = Field.ToStringInvariant(obj);
					if (!string.IsNullOrEmpty(text))
					{
						hashtable[text] = 0;
						shape.ParentGroup = text;
					}
				}
			}
			foreach (string key in hashtable.Keys)
			{
				if (this.Groups.GetByName(key) == null)
				{
					Group group = this.Groups.Add(key);
					group.Layer = layer;
					group.Category = category;
				}
			}
			this.InvalidateRules();
			this.InvalidateDataBinding();
			this.InvalidateCachedPaths();
			this.InvalidateCachedBounds();
			this.InvalidateChildSymbols();
			this.InvalidateDistanceScalePanel();
			this.InvalidateGridSections();
		}

		internal void InvalidateChildSymbols()
		{
			if (!this.resettingChildSymbols)
			{
				this.childSymbolsDirty = true;
			}
		}

		internal void ResetChildSymbols()
		{
			if (this.childSymbolsDirty)
			{
				this.resettingChildSymbols = true;
				this.childSymbolsDirty = false;
				foreach (Shape shape in this.Shapes)
				{
					shape.Symbols = null;
				}
				this.resettingChildSymbols = false;
			}
		}

		internal void InvalidateGridSections()
		{
			if (!this.recreatingGridSections)
			{
				this.gridSectionsDirty = true;
			}
		}

		internal void DisposeGridSections()
		{
			if (this.GridSections != null)
			{
				GridSection[,] array = this.GridSections;
				foreach (GridSection gridSection in array)
				{
					if (gridSection != null)
					{
						gridSection.Dispose();
					}
				}
				this.GridSections = null;
			}
		}

		internal void RecreateGridSections()
		{
			if (this.gridSectionsDirty)
			{
				this.recreatingGridSections = true;
				this.gridSectionsDirty = false;
				this.DisposeGridSections();
				this.bufferedGridSectionCount = 0;
				this.DetermineGridSectionSizeAndCount();
				this.GridSections = new GridSection[this.GridSectionsXCount, this.GridSectionsYCount];
				this.recreatingGridSections = false;
			}
		}

		internal void DetermineGridSectionSizeAndCount()
		{
			SizeF absoluteSize = this.Viewport.GetAbsoluteSize();
			int val = (int)absoluteSize.Width / 160;
			val = Math.Max(val, 1);
			val = Math.Min(val, 4);
			int val2 = (int)Math.Ceiling((double)(absoluteSize.Width / (float)val));
			val2 = Math.Max(val2, 1);
			int num = (int)Math.Ceiling((double)(absoluteSize.Height / (float)val2));
			if (num == 0)
			{
				num = 1;
			}
			int val3 = (int)Math.Ceiling((double)(absoluteSize.Height / (float)num));
			val3 = Math.Max(val3, 1);
			this.GridSectionSize = new Size(val2, val3);
			float num2 = (float)(this.Viewport.Zoom / 100.0);
			SizeF contentSizeInPixels = this.Viewport.GetContentSizeInPixels();
			contentSizeInPixels.Width *= num2;
			contentSizeInPixels.Height *= num2;
			this.GridSectionsXCount = (int)Math.Ceiling((double)(contentSizeInPixels.Width / (float)this.GridSectionSize.Width)) + 2;
			this.GridSectionsYCount = (int)Math.Ceiling((double)(contentSizeInPixels.Height / (float)this.GridSectionSize.Height)) + 2;
			this.GridSectionsInViewportXCount = Math.Min(this.GridSectionsXCount, val);
			this.GridSectionsInViewportYCount = Math.Min(this.GridSectionsYCount, num);
			PointF contentOffsetInPixels = this.Viewport.GetContentOffsetInPixels();
			this.GridSectionsOffset = new Point(MapCore.SimpleRound(contentOffsetInPixels.X % (float)this.GridSectionSize.Width), MapCore.SimpleRound(contentOffsetInPixels.Y % (float)this.GridSectionSize.Height));
		}

		internal GridSection[] GetVisibleSections()
		{
			ArrayList arrayList = new ArrayList();
			PointF contentOffsetInPixels = this.Viewport.GetContentOffsetInPixels();
			contentOffsetInPixels.X -= (float)this.GridSectionsOffset.X;
			contentOffsetInPixels.Y -= (float)this.GridSectionsOffset.Y;
			PointF locationInPixels = this.Viewport.GetLocationInPixels();
			SizeF absoluteSize = this.Viewport.GetAbsoluteSize();
			int val = (int)Math.Floor((0.0 - contentOffsetInPixels.X) / (float)this.GridSectionSize.Width) + 1;
			val = Math.Max(0, val);
			val = Math.Min(this.GridSectionsXCount - 1, val);
			int val2 = (int)Math.Floor((0.0 - contentOffsetInPixels.Y) / (float)this.GridSectionSize.Height) + 1;
			val2 = Math.Max(0, val2);
			val2 = Math.Min(this.GridSectionsYCount - 1, val2);
			int val3 = (int)Math.Floor((0.0 - contentOffsetInPixels.X + absoluteSize.Width) / (float)this.GridSectionSize.Width) + 1;
			val3 = Math.Max(0, val3);
			val3 = Math.Min(this.GridSectionsXCount - 1, val3);
			int val4 = (int)Math.Floor((0.0 - contentOffsetInPixels.Y + absoluteSize.Height) / (float)this.GridSectionSize.Height) + 1;
			val4 = Math.Max(0, val4);
			val4 = Math.Min(this.GridSectionsYCount - 1, val4);
			for (int i = val; i <= val3; i++)
			{
				for (int j = val2; j <= val4; j++)
				{
					if (this.GridSections[i, j] == null)
					{
						GridSection[,] array = this.GridSections;
						int num = i;
						int num2 = j;
						GridSection gridSection = new GridSection();
						array[num, num2] = gridSection;
						this.GridSections[i, j].Origin.X = (i - 1) * this.GridSectionSize.Width - this.GridSectionsOffset.X + MapCore.SimpleRound(locationInPixels.X);
						this.GridSections[i, j].Origin.Y = (j - 1) * this.GridSectionSize.Height - this.GridSectionsOffset.Y + MapCore.SimpleRound(locationInPixels.Y);
					}
					arrayList.Add(this.GridSections[i, j]);
				}
			}
			return (GridSection[])arrayList.ToArray(typeof(GridSection));
		}

		private void InvalidateGridSections(Rectangle rectangle)
		{
			for (int i = 0; i < this.GridSectionsXCount; i++)
			{
				for (int j = 0; j < this.GridSectionsYCount; j++)
				{
					if (this.GridSections[i, j] != null)
					{
						this.ContentToPixels(this.GridSections[i, j].Origin);
						Rectangle rectangle2 = new Rectangle(this.GridSections[i, j].Origin.X, this.GridSections[i, j].Origin.Y, this.GridSectionSize.Width, this.GridSectionSize.Height);
						if (rectangle2.IntersectsWith(rectangle))
						{
							this.GridSections[i, j].Dirty = true;
						}
					}
				}
			}
		}

		internal void RenderOneGridSection(MapGraphics g, int xIndex, int yIndex)
		{
			this.HotRegionList.Clear();
			this.Common.InvokePrePaint(this.Viewport);
			GridSection gridSection = new GridSection();
			gridSection.Origin.X = (xIndex - 1) * this.GridSectionSize.Width - this.GridSectionsOffset.X;
			gridSection.Origin.Y = (yIndex - 1) * this.GridSectionSize.Height - this.GridSectionsOffset.Y;
			gridSection.HotRegions = new HotRegionList(this);
			PointF gridSectionOffset = this.ContentToPixels(gridSection.Origin);
			new Rectangle((int)gridSectionOffset.X, (int)gridSectionOffset.Y, this.GridSectionSize.Width, this.GridSectionSize.Height);
			Graphics graphics = g.Graphics;
			try
			{
				using (Brush brush = new SolidBrush(this.GetGridSectionBackColor()))
				{
					g.Graphics.FillRectangle(brush, -1, -1, this.GridSectionSize.Width + 1, this.GridSectionSize.Height + 1);
				}
				RectangleF rect = new RectangleF(0f, 0f, (float)this.GridSectionSize.Width, (float)this.GridSectionSize.Height);
				g.Graphics.SetClip(rect, CombineMode.Replace);
				this.RenderContentElements(g, gridSectionOffset, gridSection.HotRegions);
				if (!this.GridUnderContent)
				{
					this.RenderGrid(g, gridSectionOffset);
				}
			}
			finally
			{
				g.Graphics = graphics;
			}
			foreach (HotRegion item in gridSection.HotRegions.List)
			{
				if (this.HotRegionList.FindHotRegionOfObject(item.SelectedObject) == -1)
				{
					item.DoNotDispose = true;
					this.HotRegionList.List.Add(item);
				}
			}
			this.Common.InvokePostPaint(this.Viewport);
		}

		internal static int SimpleRound(float number)
		{
			return (int)Math.Round((double)number);
		}

		internal void RenderGridSections(MapGraphics g)
		{
			this.RecreateGridSections();
			GridSection[] visibleSections = this.GetVisibleSections();
			for (int i = 0; i < visibleSections.Length; i++)
			{
				PointF pointF = this.ContentToPixels(visibleSections[i].Origin);
				Rectangle rect = new Rectangle(MapCore.SimpleRound(pointF.X), MapCore.SimpleRound(pointF.Y), this.GridSectionSize.Width, this.GridSectionSize.Height);
				if (visibleSections[i].Dirty)
				{
					visibleSections[i].Dispose();
					visibleSections[i].Dirty = false;
					this.bufferedGridSectionCount--;
				}
				if (visibleSections[i].Bitmap == null)
				{
					this.bufferedGridSectionCount++;
					visibleSections[i].Bitmap = new BufferBitmap();
					visibleSections[i].Bitmap.Size = this.GridSectionSize;
					visibleSections[i].Bitmap.Graphics.SmoothingMode = this.GetSmootingMode();
					visibleSections[i].Bitmap.Graphics.TextRenderingHint = this.GetTextRenderingHint();
					visibleSections[i].Bitmap.Graphics.TextContrast = 2;
					visibleSections[i].HotRegions = new HotRegionList(this);
					Graphics graphics = g.Graphics;
					try
					{
						g.Graphics = visibleSections[i].Bitmap.Graphics;
						using (Brush brush = new SolidBrush(this.GetGridSectionBackColor()))
						{
							g.Graphics.FillRectangle(brush, -1, -1, this.GridSectionSize.Width + 1, this.GridSectionSize.Height + 1);
						}
						RectangleF rect2 = new RectangleF(0f, 0f, (float)this.GridSectionSize.Width, (float)this.GridSectionSize.Height);
						g.Graphics.SetClip(rect2, CombineMode.Intersect);
						this.RenderContentElements(g, pointF, visibleSections[i].HotRegions);
					}
					finally
					{
						g.Graphics = graphics;
					}
				}
				foreach (HotRegion item in visibleSections[i].HotRegions.List)
				{
					if (this.HotRegionList.FindHotRegionOfObject(item.SelectedObject) == -1)
					{
						item.DoNotDispose = true;
						item.OffsetBy(pointF);
						this.HotRegionList.List.Add(item);
					}
				}
				AntiAliasing antiAliasing = g.AntiAliasing;
				g.AntiAliasing = AntiAliasing.None;
				g.Graphics.DrawImageUnscaledAndClipped(visibleSections[i].Bitmap.Bitmap, rect);
				g.AntiAliasing = antiAliasing;
			}
		}

		internal Color GetGridSectionBackColor()
		{
			if (this.Viewport.BackColor.A == 0)
			{
				return Color.FromArgb(255, this.MapControl.BackColor);
			}
			return this.Viewport.BackColor;
		}

		internal bool UseGridSectionRendering()
		{
			if (this.Viewport.OptimizeForPanning)
			{
				if (!this.IsDesignMode() && this.Viewport.EnablePanning)
				{
					return !this.IsTileLayerVisible();
				}
				return false;
			}
			return false;
		}

		internal bool IsTileLayerVisible()
		{
			foreach (Layer layer in this.Layers)
			{
				if (layer.TileSystem != 0 && layer.Visible)
				{
					return true;
				}
			}
			return false;
		}

		internal ArrayList Find(string searchFor, bool ignoreCase, bool exactSearch)
		{
			ArrayList arrayList = new ArrayList();
			arrayList.AddRange(this.Shapes.Find(searchFor, ignoreCase, exactSearch, true));
			arrayList.AddRange(this.Groups.Find(searchFor, ignoreCase, exactSearch, true));
			arrayList.AddRange(this.Symbols.Find(searchFor, ignoreCase, exactSearch, true));
			arrayList.AddRange(this.Paths.Find(searchFor, ignoreCase, exactSearch, true));
			return arrayList;
		}

		internal void SuspendUpdates()
		{
			this.suspendUpdatesCount++;
			this.disableInvalidate = true;
			if (this.suspendUpdatesCount == 1)
			{
				NamedCollection[] renderCollections = this.GetRenderCollections();
				foreach (NamedCollection namedCollection in renderCollections)
				{
					namedCollection.SuspendUpdates();
				}
			}
		}

		internal void ResumeUpdates()
		{
			if (this.suspendUpdatesCount > 0)
			{
				this.suspendUpdatesCount--;
			}
			if (this.suspendUpdatesCount == 0)
			{
				this.disableInvalidate = false;
				NamedCollection[] renderCollections = this.GetRenderCollections();
				foreach (NamedCollection namedCollection in renderCollections)
				{
					namedCollection.ResumeUpdates();
				}
			}
		}
	}
}
