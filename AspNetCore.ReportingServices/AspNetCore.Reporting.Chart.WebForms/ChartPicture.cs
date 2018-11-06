using AspNetCore.Reporting.Chart.WebForms.Borders3D;
using AspNetCore.Reporting.Chart.WebForms.Svg;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class ChartPicture : IServiceProvider
	{
		internal const float elementSpacing = 3f;

		internal const float maxTitleSize = 15f;

		private bool suppressExceptions;

		internal ChartGraphics chartGraph;

		internal bool backgroundRestored;

		private IServiceContainer serviceContainer;

		private ChartAreaCollection chartAreas;

		internal Legend legend = new Legend();

		private Color titleFontColor = Color.Black;

		internal static FontCache fontCache = new FontCache();

		private Font titleFont = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);

		internal CommonElements common;

		private GradientType backGradientType;

		private Color backGradientEndColor = Color.Empty;

		private Color backColor = Color.White;

		private string backImage = "";

		private ChartImageWrapMode backImageMode;

		private Color backImageTranspColor = Color.Empty;

		private ChartImageAlign backImageAlign;

		private Color borderColor = Color.White;

		private int borderWidth = 1;

		private ChartDashStyle borderStyle;

		private ChartHatchStyle backHatchStyle;

		private AntiAlias antiAlias;

		private AntiAliasingTypes antiAliasing = AntiAliasingTypes.All;

		private TextAntiAliasingQuality textAntiAliasingQuality = TextAntiAliasingQuality.High;

		private bool softShadows = true;

		private int width = 300;

		private int height = 300;

		private DataManipulator dataManipulator = new DataManipulator();

		internal HotRegionsList hotRegionsList;

		private BorderSkinAttributes borderSkin;

		private bool mapEnabled = true;

		private MapAreasCollection mapAreas;

		private LegendCollection legends;

		private TitleCollection titles;

		private AnnotationCollection annotations;

		internal AnnotationSmartLabels annotationSmartLabels = new AnnotationSmartLabels();

		internal bool showWaterMark;

		private RectangleF titlePosition = RectangleF.Empty;

		internal float legendMaxAutoSize = 50f;

		internal bool isPrinting;

		internal bool isSavingAsImage;

		internal bool isSelectionMode;

		private static string defaultFontFamilyName = string.Empty;

		private RectangleF chartBorderPosition = RectangleF.Empty;

		private SelectionManager selectorManager;

		private RightToLeft rightToLeft;

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
			}
		}

		[SRDescription("DescriptionAttributeSuppressExceptions")]
		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(false)]
		internal bool SuppressExceptions
		{
			get
			{
				return this.suppressExceptions;
			}
			set
			{
				this.suppressExceptions = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(BorderSkinStyle.None)]
		[SRDescription("DescriptionAttributeBorderSkinAttributes")]
		public BorderSkinAttributes BorderSkinAttributes
		{
			get
			{
				return this.borderSkin;
			}
			set
			{
				this.borderSkin = value;
			}
		}

		[SRDescription("DescriptionAttributeMapEnabled")]
		[SRCategory("CategoryAttributeMap")]
		[Bindable(true)]
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

		[SRDescription("DescriptionAttributeMapAreas")]
		[SRCategory("CategoryAttributeMap")]
		public MapAreasCollection MapAreas
		{
			get
			{
				return this.mapAreas;
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeChartAreas")]
		public ChartAreaCollection ChartAreas
		{
			get
			{
				return this.chartAreas;
			}
		}

		[SRCategory("CategoryAttributeChart")]
		[SRDescription("DescriptionAttributeLegends")]
		public LegendCollection Legends
		{
			get
			{
				return this.legends;
			}
		}

		[SRDescription("DescriptionAttributeTitles")]
		[SRCategory("CategoryAttributeCharttitle")]
		public TitleCollection Titles
		{
			get
			{
				return this.titles;
			}
		}

		[SRDescription("DescriptionAttributeAnnotations3")]
		[SRCategory("CategoryAttributeChart")]
		public AnnotationCollection Annotations
		{
			get
			{
				return this.annotations;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[Bindable(true)]
		[DefaultValue("White")]
		[SRDescription("DescriptionAttributeBackColor5")]
		public Color BackColor
		{
			get
			{
				return this.backColor;
			}
			set
			{
				if (!(value == Color.Empty) && value.A == 255)
				{
					bool flag = value == Color.Transparent;
				}
				this.backColor = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[Bindable(true)]
		[DefaultValue("White")]
		[SRDescription("DescriptionAttributeBorderColor")]
		public Color BorderColor
		{
			get
			{
				return this.borderColor;
			}
			set
			{
				this.borderColor = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(300)]
		[SRDescription("DescriptionAttributeWidth")]
		public int Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
				this.common.Width = this.width;
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeLegend")]
		public Legend Legend
		{
			get
			{
				int index = this.legends.GetIndex("Default");
				if (index >= 0)
				{
					return this.legends[index];
				}
				return this.legend;
			}
			set
			{
				value.Name = "Default";
				value.Common = this.common;
				value.CustomItems.common = this.common;
				int index = this.legends.GetIndex("Default");
				if (index >= 0)
				{
					this.legends[index] = value;
				}
				else
				{
					this.legends.Insert(0, value);
				}
			}
		}

		[SRCategory("CategoryAttributeData")]
		[SRDescription("DescriptionAttributeDataManipulator")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public DataManipulator DataManipulator
		{
			get
			{
				return this.dataManipulator;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeHeight3")]
		[Bindable(true)]
		[DefaultValue(300)]
		public int Height
		{
			get
			{
				return this.height;
			}
			set
			{
				this.height = value;
				this.common.Height = value;
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeTitle5")]
		[DefaultValue("")]
		public string Title
		{
			get
			{
				Title defaultTitle = this.GetDefaultTitle(false);
				if (defaultTitle != null && (this.common == null || !this.common.Chart.serializing))
				{
					return defaultTitle.Text;
				}
				return "";
			}
			set
			{
				Title defaultTitle = this.GetDefaultTitle(true);
				defaultTitle.Text = value;
				defaultTitle.Color = this.titleFontColor;
				defaultTitle.Font = this.titleFont;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeTitleFontColor")]
		[Bindable(true)]
		[DefaultValue("Black")]
		public Color TitleFontColor
		{
			get
			{
				Title defaultTitle = this.GetDefaultTitle(false);
				if (defaultTitle != null && (this.common == null || !this.common.Chart.serializing))
				{
					return defaultTitle.Color;
				}
				return Color.Black;
			}
			set
			{
				this.titleFontColor = value;
				Title defaultTitle = this.GetDefaultTitle(false);
				if (defaultTitle != null)
				{
					defaultTitle.Color = value;
				}
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeTitleFont4")]
		public Font TitleFont
		{
			get
			{
				Title defaultTitle = this.GetDefaultTitle(false);
				if (defaultTitle != null && (this.common == null || !this.common.Chart.serializing))
				{
					return defaultTitle.Font;
				}
				return new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);
			}
			set
			{
				this.titleFont = value;
				Title defaultTitle = this.GetDefaultTitle(false);
				if (defaultTitle != null)
				{
					defaultTitle.Font = value;
				}
			}
		}

		[DefaultValue(ChartHatchStyle.None)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeBackHatchStyle9")]
		public ChartHatchStyle BackHatchStyle
		{
			get
			{
				return this.backHatchStyle;
			}
			set
			{
				this.backHatchStyle = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeBackImage16")]
		[NotifyParentProperty(true)]
		public string BackImage
		{
			get
			{
				return this.backImage;
			}
			set
			{
				this.backImage = value;
			}
		}

		[DefaultValue(ChartImageWrapMode.Tile)]
		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackImageMode3")]
		public ChartImageWrapMode BackImageMode
		{
			get
			{
				return this.backImageMode;
			}
			set
			{
				this.backImageMode = value;
			}
		}

		[SRDescription("DescriptionAttributeBackImageTransparentColor6")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeAppearance")]
		public Color BackImageTransparentColor
		{
			get
			{
				return this.backImageTranspColor;
			}
			set
			{
				this.backImageTranspColor = value;
			}
		}

		[DefaultValue(ChartImageAlign.TopLeft)]
		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackImageAlign")]
		public ChartImageAlign BackImageAlign
		{
			get
			{
				return this.backImageAlign;
			}
			set
			{
				this.backImageAlign = value;
			}
		}

		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeSoftShadows3")]
		public bool SoftShadows
		{
			get
			{
				return this.softShadows;
			}
			set
			{
				this.softShadows = value;
			}
		}

		[DefaultValue(typeof(AntiAlias), "On")]
		[SRCategory("CategoryAttributeImage")]
		[Browsable(false)]
		[Bindable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeAntiAlias")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public AntiAlias AntiAlias
		{
			get
			{
				return this.antiAlias;
			}
			set
			{
				this.antiAlias = value;
				if (this.antiAlias == AntiAlias.Off)
				{
					this.AntiAliasing = AntiAliasingTypes.None;
				}
				else if (this.antiAlias == AntiAlias.On)
				{
					this.AntiAliasing = AntiAliasingTypes.All;
				}
			}
		}

		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[DefaultValue(typeof(AntiAliasingTypes), "All")]
		[SRDescription("DescriptionAttributeAntiAlias")]
		public AntiAliasingTypes AntiAliasing
		{
			get
			{
				return this.antiAliasing;
			}
			set
			{
				this.antiAliasing = value;
			}
		}

		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[DefaultValue(typeof(TextAntiAliasingQuality), "High")]
		[SRDescription("DescriptionAttributeTextAntiAliasingQuality")]
		public TextAntiAliasingQuality TextAntiAliasingQuality
		{
			get
			{
				return this.textAntiAliasingQuality;
			}
			set
			{
				this.textAntiAliasingQuality = value;
			}
		}

		[SRDescription("DescriptionAttributeBackGradientType3")]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(GradientType.None)]
		public GradientType BackGradientType
		{
			get
			{
				return this.backGradientType;
			}
			set
			{
				this.backGradientType = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeBackGradientEndColor4")]
		public Color BackGradientEndColor
		{
			get
			{
				return this.backGradientEndColor;
			}
			set
			{
				if (value != Color.Empty && (value.A != 255 || value == Color.Transparent))
				{
					throw new ArgumentException(SR.ExceptionChartBackGradientEndColorIsTransparent);
				}
				this.backGradientEndColor = value;
			}
		}

		[Bindable(true)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeChart_BorderlineWidth")]
		public int BorderWidth
		{
			get
			{
				return this.borderWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionChartBorderIsNegative);
				}
				this.borderWidth = value;
			}
		}

		[Bindable(true)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(ChartDashStyle.NotSet)]
		[SRDescription("DescriptionAttributeBorderStyle8")]
		public ChartDashStyle BorderStyle
		{
			get
			{
				return this.borderStyle;
			}
			set
			{
				this.borderStyle = value;
			}
		}

		internal SelectionManager SelectorManager
		{
			get
			{
				return this.selectorManager;
			}
		}

		internal bool IsSelectorManagerEnabled
		{
			get
			{
				if (this.selectorManager.Enabled)
				{
					return true;
				}
				return false;
			}
		}

		internal event PaintEventHandler BeforePaint;

		internal event PaintEventHandler AfterPaint;

		private ChartPicture()
		{
		}

		public ChartPicture(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(SR.ExceptionInvalidServiceContainer);
			}
			this.common = new CommonElements(container);
			this.chartGraph = new ChartGraphics(this.common);
			this.hotRegionsList = new HotRegionsList(this.common);
			this.borderSkin = new BorderSkinAttributes(container);
			this.serviceContainer = container;
			this.chartAreas = new ChartAreaCollection(this.common);
			this.legend.Common = this.common;
			this.legend.CustomItems.common = this.common;
			this.legend.Position.common = this.common;
			this.legends = new LegendCollection(this.common);
			this.legend.Name = "Default";
			this.legends.Add(this.legend);
			this.titles = new TitleCollection(this.serviceContainer);
			this.annotations = new AnnotationCollection(this.serviceContainer);
			this.dataManipulator.Common = this.common;
			this.mapAreas = new MapAreasCollection();
			this.selectorManager = new SelectionManager(this.serviceContainer);
		}

		internal void Initialize(Chart chart)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(ChartPicture))
			{
				return this;
			}
			throw new ArgumentException(SR.ExceptionChartPictureUnsupportedType(serviceType.ToString()));
		}

		internal void Dispose()
		{
			if (this.chartGraph != null)
			{
				this.chartGraph.Dispose();
			}
		}

		public void Select(int x, int y, ChartElementType requestedElement, bool ignoreTransparent, out string series, out int point, out ChartElementType type, out object obj)
		{
			object obj2 = null;
			this.Select(x, y, requestedElement, ignoreTransparent, out series, out point, out type, out obj, out obj2);
		}

		public void Select(int x, int y, ChartElementType requestedElement, bool ignoreTransparent, out string series, out int point, out ChartElementType type, out object obj, out object subObj)
		{
			point = -1;
			series = "";
			obj = null;
			subObj = null;
			type = ChartElementType.Nothing;
			if (this.Width > 0 && this.Height > 0)
			{
				this.common.HotRegionsList.ProcessChartMode |= ProcessMode.HotRegions;
				this.common.HotRegionsList.hitTestCalled = true;
				if (this.common.HotRegionsList.List != null)
				{
					this.common.HotRegionsList.CheckHotRegions(x, y, requestedElement, ignoreTransparent, out series, out point, out type, out obj, out subObj);
					if (this.common.HotRegionsList.List.Count != 0)
					{
						return;
					}
				}
				this.isSelectionMode = true;
				if (this.common.HotRegionsList.List != null)
				{
					this.common.HotRegionsList.List.Clear();
				}
				else
				{
					this.common.HotRegionsList.List = new ArrayList();
				}
				Bitmap bitmap = new Bitmap(this.Width, this.Height);
				Graphics graphics = Graphics.FromImage(bitmap);
				this.chartGraph.Graphics = graphics;
				this.Paint(this.chartGraph.Graphics, false);
				bitmap.Dispose();
				this.isSelectionMode = false;
				this.common.HotRegionsList.ProcessChartMode |= ProcessMode.HotRegions;
				if (this.common.HotRegionsList.List != null)
				{
					this.common.HotRegionsList.CheckHotRegions(x, y, requestedElement, ignoreTransparent, out series, out point, out type, out obj, out subObj);
					int count = this.common.HotRegionsList.List.Count;
				}
			}
		}

		public void Select(int x, int y, out string series, out int point)
		{
			ChartElementType chartElementType = default(ChartElementType);
			object obj = default(object);
			this.Select(x, y, ChartElementType.Nothing, false, out series, out point, out chartElementType, out obj);
		}

		public void Paint(Graphics graph, bool paintTopLevelElementOnly)
		{
			this.Paint(graph, paintTopLevelElementOnly, RenderingType.Gdi, null, null, string.Empty, false, false);
		}

		internal TextRenderingHint GetTextRenderingHint()
		{
			TextRenderingHint textRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
			if ((this.AntiAliasing & AntiAliasingTypes.Text) == AntiAliasingTypes.Text)
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

		public void Paint(Graphics graph, bool paintTopLevelElementOnly, RenderingType renderingType, XmlTextWriter svgTextWriter, Stream flashStream, string documentTitle, bool resizable, bool preserveAspectRatio)
		{
			this.backgroundRestored = false;
			this.annotationSmartLabels.Reset(this.common, null);
			if (this.MapEnabled)
			{
				this.common.HotRegionsList.ProcessChartMode |= (ProcessMode.Paint | ProcessMode.ImageMaps);
				for (int i = 0; i < this.MapAreas.Count; i++)
				{
					MapArea mapArea = this.MapAreas[i];
					if (!mapArea.Custom)
					{
						this.MapAreas.RemoveAt(i);
						i--;
					}
				}
			}
			if (renderingType == RenderingType.Svg)
			{
				this.chartGraph.ActiveRenderingType = RenderingType.Svg;
				this.chartGraph.SetTitle(documentTitle);
				this.chartGraph.Open(svgTextWriter, new Size(this.width, this.height), new SvgOpenParameters(this.IsToolTipsEnabled(), resizable, preserveAspectRatio));
			}
			this.chartGraph.Graphics = graph;
			this.common.graph = this.chartGraph;
			this.chartGraph.AntiAliasing = this.antiAliasing;
			this.chartGraph.softShadows = this.softShadows;
			this.chartGraph.TextRenderingHint = this.GetTextRenderingHint();
			try
			{
				if (!paintTopLevelElementOnly)
				{
					this.OnBeforePaint(new ChartPaintEventArgs(this.chartGraph, this.common, new ElementPosition(0f, 0f, 100f, 100f)));
					bool flag = false;
					foreach (ChartArea chartArea8 in this.chartAreas)
					{
						if (chartArea8.Visible)
						{
							chartArea8.Set3DAnglesAndReverseMode();
							chartArea8.SetTempValues();
							chartArea8.ReCalcInternal();
							if (chartArea8.Area3DStyle.Enable3D && !chartArea8.chartAreaIsCurcular)
							{
								flag = true;
							}
						}
					}
					this.common.EventsManager.OnCustomize();
					this.Resize(this.chartGraph, flag);
					bool flag2 = false;
					foreach (ChartArea chartArea9 in this.chartAreas)
					{
						if (chartArea9.Area3DStyle.Enable3D && !chartArea9.chartAreaIsCurcular && chartArea9.Visible)
						{
							flag2 = true;
							chartArea9.Estimate3DInterval(this.chartGraph);
							chartArea9.ReCalcInternal();
						}
					}
					if (flag)
					{
						if (flag2)
						{
							this.common.EventsManager.OnCustomize();
						}
						this.Resize(this.chartGraph);
					}
					if (this.borderSkin.SkinStyle != 0 && this.Width > 20 && this.Height > 20)
					{
						this.chartGraph.FillRectangleAbs(new RectangleF(0f, 0f, (float)(this.Width - 1), (float)(this.Height - 1)), this.borderSkin.PageColor, ChartHatchStyle.None, "", ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, this.borderSkin.PageColor, 1, ChartDashStyle.Solid, PenAlignment.Inset);
						if (this.chartGraph.ActiveRenderingType == RenderingType.Svg)
						{
							Bitmap bitmap = new Bitmap(this.Width, this.Height);
							Graphics graphics = Graphics.FromImage(bitmap);
							graphics.SmoothingMode = this.chartGraph.Graphics.SmoothingMode;
							ChartGraphics chartGraphics = new ChartGraphics(this.common);
							chartGraphics.Graphics = graphics;
							chartGraphics.Draw3DBorderAbs(this.borderSkin, this.chartBorderPosition, this.BackColor, this.BackHatchStyle, this.BackImage, this.BackImageMode, this.BackImageTransparentColor, this.BackImageAlign, this.BackGradientType, this.BackGradientEndColor, this.BorderColor, this.BorderWidth, this.BorderStyle);
							this.chartGraph.DrawImage(bitmap, new RectangleF(0f, 0f, (float)(this.Width - 1), (float)(this.Height - 1)));
							chartGraphics.Dispose();
							graphics.Dispose();
							bitmap.Dispose();
						}
						else
						{
							this.chartGraph.Draw3DBorderAbs(this.borderSkin, this.chartBorderPosition, this.BackColor, this.BackHatchStyle, this.BackImage, this.BackImageMode, this.BackImageTransparentColor, this.BackImageAlign, this.BackGradientType, this.BackGradientEndColor, this.BorderColor, this.BorderWidth, this.BorderStyle);
						}
					}
					else
					{
						this.chartGraph.FillRectangleAbs(new RectangleF(0f, 0f, (float)(this.Width - 1), (float)(this.Height - 1)), this.BackColor, this.BackHatchStyle, this.BackImage, this.BackImageMode, this.BackImageTransparentColor, this.BackImageAlign, this.BackGradientType, this.BackGradientEndColor, this.BorderColor, this.BorderWidth, this.BorderStyle, PenAlignment.Inset);
					}
					this.common.EventsManager.OnBackPaint(this, new ChartPaintEventArgs(this.chartGraph, this.common, new ElementPosition(0f, 0f, 100f, 100f)));
					foreach (ChartArea chartArea10 in this.chartAreas)
					{
						if (chartArea10.Visible)
						{
							chartArea10.Paint(this.chartGraph);
						}
					}
					foreach (ChartArea chartArea11 in this.chartAreas)
					{
						chartArea11.intervalData = double.NaN;
					}
					foreach (Legend legend2 in this.Legends)
					{
						legend2.Paint(this.chartGraph);
					}
					foreach (Title title in this.Titles)
					{
						title.Paint(this.chartGraph);
					}
					this.common.EventsManager.OnPaint(this, new ChartPaintEventArgs(this.chartGraph, this.common, new ElementPosition(0f, 0f, 100f, 100f)));
				}
				if (this.common.Chart != null)
				{
					foreach (Series item in this.common.Chart.Series)
					{
						item.financialMarkers.DrawMarkers(this.chartGraph, this);
					}
				}
				this.Annotations.Paint(this.chartGraph, paintTopLevelElementOnly);
				if (!this.isSelectionMode)
				{
					foreach (ChartArea chartArea12 in this.chartAreas)
					{
						if (chartArea12.Visible)
						{
							chartArea12.PaintCursors(this.chartGraph, paintTopLevelElementOnly);
						}
					}
				}
				foreach (ChartArea chartArea13 in this.chartAreas)
				{
					if (chartArea13.Visible)
					{
						chartArea13.Restore3DAnglesAndReverseMode();
						chartArea13.GetTempValues();
					}
				}
				if (!this.isSelectionMode)
				{
					this.SelectorManager.DrawSelection();
				}
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				this.OnAfterPaint(new ChartPaintEventArgs(this.chartGraph, this.common, new ElementPosition(0f, 0f, 100f, 100f)));
				foreach (ChartArea chartArea14 in this.chartAreas)
				{
					if (chartArea14.Visible)
					{
						chartArea14.Restore3DAnglesAndReverseMode();
						chartArea14.GetTempValues();
					}
				}
				this.showWaterMark = !this.common.Chart.IsDesignMode();
				if (renderingType == RenderingType.Svg)
				{
					this.chartGraph.Close();
					this.chartGraph.ActiveRenderingType = RenderingType.Gdi;
				}
			}
		}

		private void DrawTitle(ChartGraphics graph)
		{
			object obj = null;
			this.DrawTitle(graph, false, 0, 0, out obj);
		}

		private void DrawTitle(ChartGraphics graph, bool selectionMode, int x, int y, out object obj)
		{
			obj = null;
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			if (selectionMode)
			{
				RectangleF absolute = new RectangleF((float)(x - 1), (float)(y - 1), 2f, 2f);
				absolute = graph.GetRelativeRectangle(absolute);
				if (this.titlePosition.IntersectsWith(absolute))
				{
					obj = this;
				}
			}
			else
			{
				graph.DrawStringRel(this.Title.Replace("\\n", "\n"), this.TitleFont, new SolidBrush(this.TitleFontColor), this.titlePosition, stringFormat);
			}
		}

		protected virtual void OnBeforePaint(ChartPaintEventArgs e)
		{
			if (this.BeforePaint != null)
			{
				this.BeforePaint(this, e);
			}
		}

		protected virtual void OnAfterPaint(ChartPaintEventArgs e)
		{
			if (this.AfterPaint != null)
			{
				this.AfterPaint(this, e);
			}
		}

		public void Resize(ChartGraphics chartGraph)
		{
			this.Resize(chartGraph, false);
		}

		public void Resize(ChartGraphics chartGraph, bool calcAreaPositionOnly)
		{
			this.common.Width = this.width;
			this.common.Height = this.height;
			chartGraph.SetPictureSize(this.width, this.height);
			RectangleF rectangleF = new RectangleF(0f, 0f, (float)(this.width - 1), (float)(this.height - 1));
			rectangleF = chartGraph.GetRelativeRectangle(rectangleF);
			this.titlePosition = RectangleF.Empty;
			IBorderType borderType = null;
			bool flag = false;
			if (this.borderSkin.SkinStyle != 0)
			{
				this.chartBorderPosition = chartGraph.GetAbsoluteRectangle(rectangleF);
				borderType = this.common.BorderTypeRegistry.GetBorderType(this.borderSkin.SkinStyle.ToString());
				if (borderType != null)
				{
					borderType.Resolution = chartGraph.Graphics.DpiX;
					flag = (borderType.GetTitlePositionInBorder() != RectangleF.Empty);
					this.titlePosition = chartGraph.GetRelativeRectangle(borderType.GetTitlePositionInBorder());
					this.titlePosition.Width = rectangleF.Width - this.titlePosition.Width;
					borderType.AdjustAreasPosition(chartGraph, ref rectangleF);
				}
			}
			RectangleF rectangleF2 = RectangleF.Empty;
			if (flag)
			{
				rectangleF2 = new RectangleF(this.titlePosition.Location, this.titlePosition.Size);
			}
			foreach (Title title in this.Titles)
			{
				if (title.DockToChartArea == "NotSet" && title.Position.Auto && title.Visible)
				{
					title.CalcTitlePosition(chartGraph, ref rectangleF, ref rectangleF2, 3f);
				}
			}
			this.Legends.CalcLegendPosition(chartGraph, ref rectangleF, this.legendMaxAutoSize, 3f);
			rectangleF.Width -= 3f;
			rectangleF.Height -= 3f;
			RectangleF rectangleF3 = default(RectangleF);
			int num = 0;
			foreach (ChartArea chartArea4 in this.chartAreas)
			{
				if (chartArea4.Visible && chartArea4.Position.Auto)
				{
					num++;
				}
			}
			int num2 = (int)Math.Floor(Math.Sqrt((double)num));
			if (num2 < 1)
			{
				num2 = 1;
			}
			int num3 = (int)Math.Ceiling((double)((float)num / (float)num2));
			int num4 = 0;
			int num5 = 0;
			foreach (ChartArea chartArea5 in this.chartAreas)
			{
				if (chartArea5.Visible)
				{
					if (chartArea5.Position.Auto)
					{
						rectangleF3.Width = (float)(rectangleF.Width / (float)num2 - 3.0);
						rectangleF3.Height = (float)(rectangleF.Height / (float)num3 - 3.0);
						rectangleF3.X = (float)(rectangleF.X + (float)num4 * (rectangleF.Width / (float)num2) + 3.0);
						rectangleF3.Y = (float)(rectangleF.Y + (float)num5 * (rectangleF.Height / (float)num3) + 3.0);
						TitleCollection.CalcOutsideTitlePosition(this, chartGraph, chartArea5, ref rectangleF3, 3f);
						this.Legends.CalcOutsideLegendPosition(chartGraph, chartArea5, ref rectangleF3, this.legendMaxAutoSize, 3f);
						chartArea5.Position.SetPositionNoAuto(rectangleF3.X, rectangleF3.Y, rectangleF3.Width, rectangleF3.Height);
						num5++;
						if (num5 >= num3)
						{
							num5 = 0;
							num4++;
						}
					}
					else
					{
						RectangleF rectangleF4 = chartArea5.Position.ToRectangleF();
						TitleCollection.CalcOutsideTitlePosition(this, chartGraph, chartArea5, ref rectangleF4, 3f);
						this.Legends.CalcOutsideLegendPosition(chartGraph, chartArea5, ref rectangleF4, this.legendMaxAutoSize, 3f);
					}
				}
			}
			this.AlignChartAreasPosition();
			if (!calcAreaPositionOnly)
			{
				foreach (ChartArea chartArea6 in this.chartAreas)
				{
					if (chartArea6.Visible)
					{
						chartArea6.Resize(chartGraph);
					}
				}
				this.AlignChartAreas(AreaAlignTypes.PlotPosition);
				TitleCollection.CalcInsideTitlePosition(this, chartGraph, 3f);
				this.Legends.CalcInsideLegendPosition(chartGraph, this.legendMaxAutoSize, 3f);
			}
		}

		internal void ResetMinMaxFromData()
		{
			foreach (ChartArea chartArea in this.chartAreas)
			{
				if (chartArea.Visible)
				{
					chartArea.ResetMinMaxFromData();
				}
			}
		}

		public void Recalculate()
		{
			foreach (ChartArea chartArea in this.chartAreas)
			{
				if (chartArea.Visible)
				{
					chartArea.ReCalcInternal();
				}
			}
		}

		private bool IsAreasAlignmentRequired()
		{
			bool result = false;
			foreach (ChartArea chartArea in this.ChartAreas)
			{
				if (chartArea.Visible && chartArea.AlignWithChartArea != "NotSet")
				{
					result = true;
					try
					{
						ChartArea chartArea2 = this.ChartAreas[chartArea.AlignWithChartArea];
					}
					catch
					{
						throw new InvalidOperationException(SR.ExceptionChartAreaNameReferenceInvalid(chartArea.Name, chartArea.AlignWithChartArea));
					}
				}
			}
			return result;
		}

		private ArrayList GetAlignedAreasGroup(ChartArea masterArea, AreaAlignTypes type, AreaAlignOrientations orientation)
		{
			ArrayList arrayList = new ArrayList();
			foreach (ChartArea chartArea in this.ChartAreas)
			{
				if (chartArea.Visible && chartArea.Name != masterArea.Name && chartArea.AlignWithChartArea == masterArea.Name && (chartArea.AlignType & type) == type && (chartArea.AlignOrientation & orientation) == orientation)
				{
					arrayList.Add(chartArea);
				}
			}
			if (arrayList.Count > 0)
			{
				arrayList.Insert(0, masterArea);
			}
			return arrayList;
		}

		internal void AlignChartAreas(AreaAlignTypes type)
		{
			if (this.IsAreasAlignmentRequired())
			{
				foreach (ChartArea chartArea in this.ChartAreas)
				{
					if (chartArea.Visible)
					{
						ArrayList alignedAreasGroup = this.GetAlignedAreasGroup(chartArea, type, AreaAlignOrientations.Vertical);
						if (alignedAreasGroup.Count > 0)
						{
							this.AlignChartAreasPlotPosition(alignedAreasGroup, AreaAlignOrientations.Vertical);
						}
						alignedAreasGroup = this.GetAlignedAreasGroup(chartArea, type, AreaAlignOrientations.Horizontal);
						if (alignedAreasGroup.Count > 0)
						{
							this.AlignChartAreasPlotPosition(alignedAreasGroup, AreaAlignOrientations.Horizontal);
						}
					}
				}
			}
		}

		private void AlignChartAreasPlotPosition(ArrayList areasGroup, AreaAlignOrientations orientation)
		{
			RectangleF rectangleF = ((ChartArea)areasGroup[0]).PlotAreaPosition.ToRectangleF();
			foreach (ChartArea item in areasGroup)
			{
				if (item.PlotAreaPosition.X > rectangleF.X)
				{
					rectangleF.X += item.PlotAreaPosition.X - rectangleF.X;
					rectangleF.Width -= item.PlotAreaPosition.X - rectangleF.X;
				}
				if (item.PlotAreaPosition.Y > rectangleF.Y)
				{
					rectangleF.Y += item.PlotAreaPosition.Y - rectangleF.Y;
					rectangleF.Height -= item.PlotAreaPosition.Y - rectangleF.Y;
				}
				if (item.PlotAreaPosition.Right() < rectangleF.Right)
				{
					rectangleF.Width -= rectangleF.Right - item.PlotAreaPosition.Right();
					if (rectangleF.Width < 5.0)
					{
						rectangleF.Width = 5f;
					}
				}
				if (item.PlotAreaPosition.Bottom() < rectangleF.Bottom)
				{
					rectangleF.Height -= rectangleF.Bottom - item.PlotAreaPosition.Bottom();
					if (rectangleF.Height < 5.0)
					{
						rectangleF.Height = 5f;
					}
				}
			}
			foreach (ChartArea item2 in areasGroup)
			{
				RectangleF rectangleF2 = item2.PlotAreaPosition.ToRectangleF();
				if ((orientation & AreaAlignOrientations.Vertical) == AreaAlignOrientations.Vertical)
				{
					rectangleF2.X = rectangleF.X;
					rectangleF2.Width = rectangleF.Width;
				}
				if ((orientation & AreaAlignOrientations.Horizontal) == AreaAlignOrientations.Horizontal)
				{
					rectangleF2.Y = rectangleF.Y;
					rectangleF2.Height = rectangleF.Height;
				}
				item2.PlotAreaPosition.SetPositionNoAuto(rectangleF2.X, rectangleF2.Y, rectangleF2.Width, rectangleF2.Height);
				rectangleF2.X = (float)((rectangleF2.X - item2.Position.X) / item2.Position.Width * 100.0);
				rectangleF2.Y = (float)((rectangleF2.Y - item2.Position.Y) / item2.Position.Height * 100.0);
				rectangleF2.Width = (float)(rectangleF2.Width / item2.Position.Width * 100.0);
				rectangleF2.Height = (float)(rectangleF2.Height / item2.Position.Height * 100.0);
				item2.InnerPlotPosition.SetPositionNoAuto(rectangleF2.X, rectangleF2.Y, rectangleF2.Width, rectangleF2.Height);
				if ((orientation & AreaAlignOrientations.Vertical) == AreaAlignOrientations.Vertical)
				{
					item2.AxisX2.AdjustLabelFontAtSecondPass(this.chartGraph, item2.InnerPlotPosition.Auto);
					item2.AxisX.AdjustLabelFontAtSecondPass(this.chartGraph, item2.InnerPlotPosition.Auto);
				}
				if ((orientation & AreaAlignOrientations.Horizontal) == AreaAlignOrientations.Horizontal)
				{
					item2.AxisY2.AdjustLabelFontAtSecondPass(this.chartGraph, item2.InnerPlotPosition.Auto);
					item2.AxisY.AdjustLabelFontAtSecondPass(this.chartGraph, item2.InnerPlotPosition.Auto);
				}
			}
		}

		private void AlignChartAreasPosition()
		{
			if (this.IsAreasAlignmentRequired())
			{
				foreach (ChartArea chartArea3 in this.ChartAreas)
				{
					if (chartArea3.Visible && chartArea3.AlignWithChartArea != "NotSet" && (chartArea3.AlignType & AreaAlignTypes.Position) == AreaAlignTypes.Position)
					{
						RectangleF rectangleF = chartArea3.Position.ToRectangleF();
						ChartArea chartArea2 = this.ChartAreas[chartArea3.AlignWithChartArea];
						if ((chartArea3.AlignOrientation & AreaAlignOrientations.Vertical) == AreaAlignOrientations.Vertical)
						{
							rectangleF.X = chartArea2.Position.X;
							rectangleF.Width = chartArea2.Position.Width;
						}
						if ((chartArea3.AlignOrientation & AreaAlignOrientations.Horizontal) == AreaAlignOrientations.Horizontal)
						{
							rectangleF.Y = chartArea2.Position.Y;
							rectangleF.Height = chartArea2.Position.Height;
						}
						chartArea3.Position.SetPositionNoAuto(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height);
					}
				}
			}
		}

		internal void AlignChartAreasCursor(ChartArea changedArea, AreaAlignOrientations orientation, bool selectionChanged)
		{
			if (this.IsAreasAlignmentRequired())
			{
				foreach (ChartArea chartArea3 in this.ChartAreas)
				{
					if (chartArea3.Visible)
					{
						ArrayList alignedAreasGroup = this.GetAlignedAreasGroup(chartArea3, AreaAlignTypes.Cursor, orientation);
						if (alignedAreasGroup.Contains(changedArea))
						{
							foreach (ChartArea item in alignedAreasGroup)
							{
								item.alignmentInProcess = true;
								if (orientation == AreaAlignOrientations.Vertical)
								{
									if (selectionChanged)
									{
										item.CursorX.SelectionStart = changedArea.CursorX.SelectionStart;
										item.CursorX.SelectionEnd = changedArea.CursorX.SelectionEnd;
									}
									else
									{
										item.CursorX.Position = changedArea.CursorX.Position;
									}
								}
								if (orientation == AreaAlignOrientations.Horizontal)
								{
									if (selectionChanged)
									{
										item.CursorY.SelectionStart = changedArea.CursorY.SelectionStart;
										item.CursorY.SelectionEnd = changedArea.CursorY.SelectionEnd;
									}
									else
									{
										item.CursorY.Position = changedArea.CursorY.Position;
									}
								}
								item.alignmentInProcess = false;
							}
						}
					}
				}
			}
		}

		internal void AlignChartAreasZoomed(ChartArea changedArea, AreaAlignOrientations orientation, bool disposeBufferBitmap)
		{
			if (this.IsAreasAlignmentRequired())
			{
				foreach (ChartArea chartArea3 in this.ChartAreas)
				{
					if (chartArea3.Visible)
					{
						ArrayList alignedAreasGroup = this.GetAlignedAreasGroup(chartArea3, AreaAlignTypes.AxesView, orientation);
						if (alignedAreasGroup.Contains(changedArea))
						{
							foreach (ChartArea item in alignedAreasGroup)
							{
								if (orientation == AreaAlignOrientations.Vertical)
								{
									item.CursorX.SelectionStart = double.NaN;
									item.CursorX.SelectionEnd = double.NaN;
								}
								if (orientation == AreaAlignOrientations.Horizontal)
								{
									item.CursorY.SelectionStart = double.NaN;
									item.CursorY.SelectionEnd = double.NaN;
								}
							}
						}
					}
				}
			}
		}

		internal void AlignChartAreasAxesView(ChartArea changedArea, AreaAlignOrientations orientation)
		{
			if (this.IsAreasAlignmentRequired())
			{
				foreach (ChartArea chartArea3 in this.ChartAreas)
				{
					if (chartArea3.Visible)
					{
						ArrayList alignedAreasGroup = this.GetAlignedAreasGroup(chartArea3, AreaAlignTypes.AxesView, orientation);
						if (alignedAreasGroup.Contains(changedArea))
						{
							foreach (ChartArea item in alignedAreasGroup)
							{
								item.alignmentInProcess = true;
								if (orientation == AreaAlignOrientations.Vertical)
								{
									item.AxisX.View.Position = changedArea.AxisX.View.Position;
									item.AxisX.View.Size = changedArea.AxisX.View.Size;
									item.AxisX.View.SizeType = changedArea.AxisX.View.SizeType;
									item.AxisX2.View.Position = changedArea.AxisX2.View.Position;
									item.AxisX2.View.Size = changedArea.AxisX2.View.Size;
									item.AxisX2.View.SizeType = changedArea.AxisX2.View.SizeType;
								}
								if (orientation == AreaAlignOrientations.Horizontal)
								{
									item.AxisY.View.Position = changedArea.AxisY.View.Position;
									item.AxisY.View.Size = changedArea.AxisY.View.Size;
									item.AxisY.View.SizeType = changedArea.AxisY.View.SizeType;
									item.AxisY2.View.Position = changedArea.AxisY2.View.Position;
									item.AxisY2.View.Size = changedArea.AxisY2.View.Size;
									item.AxisY2.View.SizeType = changedArea.AxisY2.View.SizeType;
								}
								item.alignmentInProcess = false;
							}
						}
					}
				}
			}
		}

		internal bool IsRightToLeft()
		{
			return this.RightToLeft == RightToLeft.Yes;
		}

		internal static string GetDefaultFontFamilyName()
		{
			if (ChartPicture.defaultFontFamilyName.Length == 0)
			{
				ChartPicture.defaultFontFamilyName = "Microsoft Sans Serif";
				bool flag = false;
				FontFamily[] families = FontFamily.Families;
				FontFamily[] array = families;
				foreach (FontFamily fontFamily in array)
				{
					if (fontFamily.Name == ChartPicture.defaultFontFamilyName)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					try
					{
						FontFamily genericSansSerif = FontFamily.GenericSansSerif;
						ChartPicture.defaultFontFamilyName = genericSansSerif.Name;
					}
					catch
					{
					}
				}
			}
			return ChartPicture.defaultFontFamilyName;
		}

		public void LoadTemplate(string name)
		{
			Stream stream = this.LoadTemplateData(name);
			this.LoadTemplate(stream);
			stream.Close();
		}

		public void LoadTemplate(Stream stream)
		{
			if (stream != null)
			{
				ChartSerializer chartSerializer = (ChartSerializer)this.common.container.GetService(typeof(ChartSerializer));
				if (chartSerializer != null)
				{
					string serializableContent = chartSerializer.SerializableContent;
					string nonSerializableContent = chartSerializer.NonSerializableContent;
					SerializationFormat format = chartSerializer.Format;
					bool ignoreUnknownXmlAttributes = chartSerializer.IgnoreUnknownXmlAttributes;
					bool templateMode = chartSerializer.TemplateMode;
					chartSerializer.Content = SerializationContents.Appearance;
					chartSerializer.Format = SerializationFormat.Xml;
					chartSerializer.IgnoreUnknownXmlAttributes = true;
					chartSerializer.TemplateMode = true;
					try
					{
						chartSerializer.Load(stream);
					}
					catch (Exception ex)
					{
						throw new InvalidOperationException(ex.Message);
					}
					finally
					{
						chartSerializer.SerializableContent = serializableContent;
						chartSerializer.NonSerializableContent = nonSerializableContent;
						chartSerializer.Format = format;
						chartSerializer.IgnoreUnknownXmlAttributes = ignoreUnknownXmlAttributes;
						chartSerializer.TemplateMode = templateMode;
					}
				}
			}
		}

		private Stream LoadTemplateData(string url)
		{
			Stream stream = null;
			if (stream == null)
			{
				stream = new FileStream(url, FileMode.Open);
			}
			return stream;
		}

		internal void WriteChartMapTag(TextWriter output, string mapName)
		{
			output.Write("\r\n<map name=\"" + mapName + "\" id=\"" + mapName + "\">");
			MapAreasCollection mapAreasCollection = new MapAreasCollection();
			for (int i = 0; i < this.mapAreas.Count; i++)
			{
				if (!this.mapAreas[i].Custom)
				{
					mapAreasCollection.Add(this.mapAreas[i]);
					this.mapAreas.RemoveAt(i);
					i--;
				}
			}
			this.common.EventsManager.OnCustomizeMapAreas(mapAreasCollection);
			foreach (MapArea item in mapAreasCollection)
			{
				item.Custom = false;
				this.mapAreas.Add(item);
			}
			foreach (MapArea mapArea in this.mapAreas)
			{
				output.Write(mapArea.GetTag(this.chartGraph));
			}
			if (this.mapAreas.Count == 0)
			{
				output.Write("<area shape=\"rect\" coords=\"0,0,0,0\" alt=\"\" />");
			}
			output.Write("\r\n</map>\r\n");
		}

		internal Title GetDefaultTitle(bool create)
		{
			Title title = null;
			foreach (Title title2 in this.Titles)
			{
				if (title2.Name == "Default Title")
				{
					title = title2;
				}
			}
			if (title == null && create)
			{
				title = new Title();
				title.Name = "Default Title";
				this.Titles.Insert(0, title);
			}
			return title;
		}

		private bool IsToolTipsEnabled()
		{
			foreach (Series item in this.common.DataManager.Series)
			{
				if (item.ToolTip.Length > 0)
				{
					return true;
				}
				if (item.LegendToolTip.Length > 0 || item.LabelToolTip.Length > 0)
				{
					return true;
				}
				if (!item.IsFastChartType())
				{
					foreach (DataPoint point in item.Points)
					{
						if (point.ToolTip.Length > 0)
						{
							return true;
						}
						if (point.LegendToolTip.Length > 0 || point.LabelToolTip.Length > 0)
						{
							return true;
						}
					}
				}
			}
			foreach (Legend legend2 in this.Legends)
			{
				foreach (LegendItem customItem in legend2.CustomItems)
				{
					if (customItem.ToolTip.Length > 0)
					{
						return true;
					}
				}
			}
			foreach (Title title in this.Titles)
			{
				if (title.ToolTip.Length > 0)
				{
					return true;
				}
			}
			return false;
		}
	}
}
