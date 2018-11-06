using AspNetCore.Reporting.Chart.WebForms.ChartTypes;
using AspNetCore.Reporting.Chart.WebForms.Design;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[DefaultProperty("Enabled")]
	[SRDescription("DescriptionAttributeLegend_Legend")]
	internal class Legend : ChartElement
	{
		private ElementPosition position = new ElementPosition();

		private bool enabled = true;

		private LegendStyle legendStyle = LegendStyle.Table;

		private LegendTableStyle legendTableStyle;

		private LegendItemsCollection customLegends;

		private ChartHatchStyle backHatchStyle;

		private string backImage = "";

		private ChartImageWrapMode backImageMode;

		private Color backImageTranspColor = Color.Empty;

		private ChartImageAlign backImageAlign;

		private GradientType backGradientType;

		private Color backGradientEndColor = Color.Empty;

		private Color borderColor = Color.Black;

		private Color backColor = Color.Transparent;

		private int borderWidth = 1;

		private ChartDashStyle borderStyle;

		private Font font = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);

		private Color fontColor = Color.Black;

		private StringAlignment legendAlignment;

		private LegendDocking legendDocking = LegendDocking.Right;

		private int shadowOffset;

		private Color shadowColor = Color.FromArgb(128, 0, 0, 0);

		private bool autoFitText = true;

		private string name = "";

		private string dockToChartArea = "NotSet";

		private bool dockInsideChartArea = true;

		internal LegendItemsCollection legendItems;

		private SizeF sizeLargestItemText = SizeF.Empty;

		private SizeF sizeAverageItemText = SizeF.Empty;

		private SizeF sizeItemImage = SizeF.Empty;

		private int itemColumns;

		private SizeF itemCellSize = SizeF.Empty;

		internal Font autofitFont;

		private bool equallySpacedItems;

		private bool interlacedRows;

		private Color interlacedRowsColor = Color.Empty;

		private Size offset = Size.Empty;

		private float maximumLegendAutoSize = 50f;

		private PointF animationLocationAdjustment = PointF.Empty;

		private int textWrapThreshold = 25;

		private int autoFitFontSizeAdjustment;

		private LegendCellColumnCollection cellColumns;

		private AutoBool reversed;

		private string title = string.Empty;

		private Color titleColor = Color.Black;

		private Color titleBackColor = Color.Empty;

		private Font titleFont = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f, FontStyle.Bold);

		private StringAlignment titleAlignment = StringAlignment.Center;

		private LegendSeparatorType titleSeparator;

		private Color titleSeparatorColor = Color.Black;

		private LegendSeparatorType headerSeparator;

		private Color headerSeparatorColor = Color.Black;

		private LegendSeparatorType itemColumnSeparator;

		private Color itemColumnSeparatorColor = Color.Black;

		private int itemColumnSpacing = 50;

		private int itemColumnSpacingRel;

		private Rectangle titlePosition = Rectangle.Empty;

		private Rectangle headerPosition = Rectangle.Empty;

		private int autoFitMinFontSize = 7;

		private int horizontalSpaceLeft;

		private int verticalSpaceLeft;

		private int[,] subColumnSizes;

		private int[,] cellHeights;

		private int[] numberOfRowsPerColumn;

		private int numberOfLegendItemsToProcess = -1;

		private Rectangle legendItemsAreaPosition = Rectangle.Empty;

		private bool legendItemsTruncated;

		private int truncatedDotsSize = 3;

		private int numberOfCells = -1;

		internal Size singleWCharacterSize = Size.Empty;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegend_Name")]
		[NotifyParentProperty(true)]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (value != this.name && !(this.name == "Default"))
				{
					if (value == null || value.Length == 0)
					{
						throw new ArgumentException(SR.ExceptionLegendNameIsEmpty);
					}
					if (base.Common != null && base.Common.ChartPicture != null && base.Common.ChartPicture.Legends.GetIndex(value) != -1)
					{
						throw new ArgumentException(SR.ExceptionLegendNameIsNotUnique(value));
					}
					this.name = value;
				}
			}
		}

		[DefaultValue("NotSet")]
		[Bindable(true)]
		[SRCategory("CategoryAttributeDocking")]
		[SRDescription("DescriptionAttributeLegend_DockToChartArea")]
		[TypeConverter(typeof(LegendAreaNameConverter))]
		[NotifyParentProperty(true)]
		public string DockToChartArea
		{
			get
			{
				return this.dockToChartArea;
			}
			set
			{
				if (value != this.dockToChartArea)
				{
					if (value.Length == 0)
					{
						this.dockToChartArea = "NotSet";
					}
					else
					{
						this.dockToChartArea = value;
					}
					this.Invalidate(false);
				}
			}
		}

		[SRCategory("CategoryAttributeDocking")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeLegend_DockInsideChartArea")]
		[NotifyParentProperty(true)]
		public bool DockInsideChartArea
		{
			get
			{
				return this.dockInsideChartArea;
			}
			set
			{
				if (value != this.dockInsideChartArea)
				{
					this.dockInsideChartArea = value;
					this.Invalidate(false);
				}
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeLegend_Position")]
		[DefaultValue(typeof(ElementPosition), "Auto")]
		[NotifyParentProperty(true)]
		[TypeConverter(typeof(ElementPositionConverter))]
		[SerializationVisibility(SerializationVisibility.Element)]
		public ElementPosition Position
		{
			get
			{
				if (base.Common != null && base.Common.Chart != null && base.Common.Chart.serializationStatus == SerializationStatus.Saving)
				{
					if (this.position.Auto)
					{
						return new ElementPosition();
					}
					ElementPosition elementPosition = new ElementPosition();
					elementPosition.Auto = true;
					elementPosition.SetPositionNoAuto(this.position.X, this.position.Y, this.position.Width, this.position.Height);
					return elementPosition;
				}
				return this.position;
			}
			set
			{
				this.position = value;
				this.Invalidate(false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeLegend_EquallySpacedItems")]
		[NotifyParentProperty(true)]
		public bool EquallySpacedItems
		{
			get
			{
				return this.equallySpacedItems;
			}
			set
			{
				this.equallySpacedItems = value;
				this.Invalidate(false);
			}
		}

		[ParenthesizePropertyName(true)]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeLegend_Enabled")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeAppearance")]
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
				this.Invalidate(false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeLegend_AutoFitText")]
		[NotifyParentProperty(true)]
		public bool AutoFitText
		{
			get
			{
				return this.autoFitText;
			}
			set
			{
				this.autoFitText = value;
				if (this.autoFitText)
				{
					if (this.font != null)
					{
						Font font = new Font(this.font.FontFamily, 8f, this.font.Style);
						this.font.Dispose();
						this.font = font;
					}
					else
					{
						this.font = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);
					}
				}
				this.Invalidate(false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(LegendStyle.Table)]
		[SRDescription("DescriptionAttributeLegend_LegendStyle")]
		[NotifyParentProperty(true)]
		[ParenthesizePropertyName(true)]
		public LegendStyle LegendStyle
		{
			get
			{
				return this.legendStyle;
			}
			set
			{
				this.legendStyle = value;
				this.Invalidate(false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(7)]
		[SRDescription("DescriptionAttributeLegend_AutoFitMinFontSize")]
		public int AutoFitMinFontSize
		{
			get
			{
				return this.autoFitMinFontSize;
			}
			set
			{
				if (value < 5)
				{
					throw new InvalidOperationException(SR.ExceptionLegendAutoFitMinFontSizeInvalid);
				}
				this.autoFitMinFontSize = value;
				this.Invalidate(false);
			}
		}

		[SRCategory("CategoryAttributeDocking")]
		[DefaultValue(50f)]
		[SRDescription("DescriptionAttributeLegend_MaxAutoSize")]
		public float MaxAutoSize
		{
			get
			{
				return this.maximumLegendAutoSize;
			}
			set
			{
				if (!(value < 0.0) && !(value > 100.0))
				{
					this.maximumLegendAutoSize = value;
					this.Invalidate(false);
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionLegendMaximumAutoSizeInvalid);
			}
		}

		[SRDescription("DescriptionAttributeLegend_CellColumns")]
		[SRCategory("CategoryAttributeCellColumns")]
		public LegendCellColumnCollection CellColumns
		{
			get
			{
				return this.cellColumns;
			}
		}

		[DefaultValue(LegendTableStyle.Auto)]
		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeLegend_TableStyle")]
		[NotifyParentProperty(true)]
		[ParenthesizePropertyName(true)]
		public LegendTableStyle TableStyle
		{
			get
			{
				return this.legendTableStyle;
			}
			set
			{
				this.legendTableStyle = value;
				this.Invalidate(false);
			}
		}

		[SRCategory("CategoryAttributeCellColumns")]
		[DefaultValue(typeof(LegendSeparatorType), "None")]
		[SRDescription("DescriptionAttributeLegend_HeaderSeparator")]
		public LegendSeparatorType HeaderSeparator
		{
			get
			{
				return this.headerSeparator;
			}
			set
			{
				if (value != this.headerSeparator)
				{
					this.headerSeparator = value;
					this.Invalidate(false);
				}
			}
		}

		[DefaultValue(typeof(Color), "Black")]
		[SRCategory("CategoryAttributeCellColumns")]
		[SRDescription("DescriptionAttributeLegend_HeaderSeparatorColor")]
		public Color HeaderSeparatorColor
		{
			get
			{
				return this.headerSeparatorColor;
			}
			set
			{
				if (value != this.headerSeparatorColor)
				{
					this.headerSeparatorColor = value;
					this.Invalidate(false);
				}
			}
		}

		[SRCategory("CategoryAttributeCellColumns")]
		[DefaultValue(typeof(LegendSeparatorType), "None")]
		[SRDescription("DescriptionAttributeLegend_ItemColumnSeparator")]
		public LegendSeparatorType ItemColumnSeparator
		{
			get
			{
				return this.itemColumnSeparator;
			}
			set
			{
				if (value != this.itemColumnSeparator)
				{
					this.itemColumnSeparator = value;
					this.Invalidate(false);
				}
			}
		}

		[SRCategory("CategoryAttributeCellColumns")]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeLegend_ItemColumnSeparatorColor")]
		public Color ItemColumnSeparatorColor
		{
			get
			{
				return this.itemColumnSeparatorColor;
			}
			set
			{
				if (value != this.itemColumnSeparatorColor)
				{
					this.itemColumnSeparatorColor = value;
					this.Invalidate(false);
				}
			}
		}

		[SRCategory("CategoryAttributeCellColumns")]
		[DefaultValue(50)]
		[SRDescription("DescriptionAttributeLegend_ItemColumnSpacing")]
		public int ItemColumnSpacing
		{
			get
			{
				return this.itemColumnSpacing;
			}
			set
			{
				if (value != this.itemColumnSpacing)
				{
					if (value < 0)
					{
						throw new ArgumentOutOfRangeException("value", SR.ExceptionLegendColumnSpacingInvalid);
					}
					this.itemColumnSpacing = value;
					this.Invalidate(false);
				}
			}
		}

		[Bindable(true)]
		[DefaultValue(typeof(Color), "Transparent")]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeLegend_BackColor")]
		[NotifyParentProperty(true)]
		public Color BackColor
		{
			get
			{
				return this.backColor;
			}
			set
			{
				this.backColor = value;
				this.Invalidate(false);
			}
		}

		[Browsable(false)]
		[DefaultValue(typeof(Color), "Black")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegend_BorderColor")]
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
				this.Invalidate(false);
			}
		}

		[Browsable(false)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartDashStyle.NotSet)]
		[SRDescription("DescriptionAttributeLegend_BorderStyle")]
		public ChartDashStyle BorderStyle
		{
			get
			{
				return this.borderStyle;
			}
			set
			{
				this.borderStyle = value;
				this.Invalidate(false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[NotifyParentProperty(true)]
		[Bindable(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeLegend_BorderWidth")]
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
					throw new ArgumentOutOfRangeException("value", SR.ExceptionLegendBorderWidthIsNegative);
				}
				this.borderWidth = value;
				this.Invalidate(false);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeLegendBackImage7")]
		public string BackImage
		{
			get
			{
				return this.backImage;
			}
			set
			{
				this.backImage = value;
				this.Invalidate(true);
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(ChartImageWrapMode.Tile)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLegend_BackImageMode")]
		public ChartImageWrapMode BackImageMode
		{
			get
			{
				return this.backImageMode;
			}
			set
			{
				this.backImageMode = value;
				this.Invalidate(true);
			}
		}

		[SRDescription("DescriptionAttributeLegend_BackImageTransparentColor")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		public Color BackImageTransparentColor
		{
			get
			{
				return this.backImageTranspColor;
			}
			set
			{
				this.backImageTranspColor = value;
				this.Invalidate(true);
			}
		}

		[DefaultValue(ChartImageAlign.TopLeft)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLegend_BackImageAlign")]
		public ChartImageAlign BackImageAlign
		{
			get
			{
				return this.backImageAlign;
			}
			set
			{
				this.backImageAlign = value;
				this.Invalidate(true);
			}
		}

		[Browsable(false)]
		[SRDescription("DescriptionAttributeLegend_BackGradientType")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(GradientType.None)]
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
				this.Invalidate(true);
			}
		}

		[NotifyParentProperty(true)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegend_BackGradientEndColor")]
		public Color BackGradientEndColor
		{
			get
			{
				return this.backGradientEndColor;
			}
			set
			{
				this.backGradientEndColor = value;
				this.Invalidate(true);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartHatchStyle.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLegend_BackHatchStyle")]
		public ChartHatchStyle BackHatchStyle
		{
			get
			{
				return this.backHatchStyle;
			}
			set
			{
				this.backHatchStyle = value;
				this.Invalidate(true);
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeLegend_Font")]
		[NotifyParentProperty(true)]
		public Font Font
		{
			get
			{
				return this.font;
			}
			set
			{
				this.AutoFitText = false;
				this.font = value;
				this.Invalidate(false);
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeLegend_FontColor")]
		[NotifyParentProperty(true)]
		public Color FontColor
		{
			get
			{
				return this.fontColor;
			}
			set
			{
				this.fontColor = value;
				this.Invalidate(true);
			}
		}

		[SRCategory("CategoryAttributeDocking")]
		[Bindable(true)]
		[DefaultValue(StringAlignment.Near)]
		[SRDescription("DescriptionAttributeLegend_Alignment")]
		[NotifyParentProperty(true)]
		public StringAlignment Alignment
		{
			get
			{
				return this.legendAlignment;
			}
			set
			{
				this.legendAlignment = value;
				this.Invalidate(false);
			}
		}

		[NotifyParentProperty(true)]
		[Bindable(true)]
		[DefaultValue(LegendDocking.Right)]
		[SRDescription("DescriptionAttributeLegend_Docking")]
		[SRCategory("CategoryAttributeDocking")]
		public LegendDocking Docking
		{
			get
			{
				return this.legendDocking;
			}
			set
			{
				this.legendDocking = value;
				this.Invalidate(false);
			}
		}

		[DefaultValue(0)]
		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeLegend_ShadowOffset")]
		[NotifyParentProperty(true)]
		public int ShadowOffset
		{
			get
			{
				return this.shadowOffset;
			}
			set
			{
				this.shadowOffset = value;
				this.Invalidate(false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "128, 0, 0, 0")]
		[SRDescription("DescriptionAttributeLegend_ShadowColor")]
		[NotifyParentProperty(true)]
		public Color ShadowColor
		{
			get
			{
				return this.shadowColor;
			}
			set
			{
				this.shadowColor = value;
				this.Invalidate(true);
			}
		}

		[NotifyParentProperty(true)]
		[Browsable(false)]
		[Bindable(false)]
		[DefaultValue("NotSet")]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeLegend_InsideChartArea")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[TypeConverter(typeof(LegendAreaNameConverter))]
		public string InsideChartArea
		{
			get
			{
				if (base.Common != null && base.Common.Chart != null && base.Common.Chart.serializing)
				{
					return "NotSet";
				}
				return this.DockToChartArea;
			}
			set
			{
				if (value.Length == 0)
				{
					this.DockToChartArea = "NotSet";
				}
				else
				{
					this.DockToChartArea = value;
				}
				this.Invalidate(false);
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLegend_CustomItems")]
		public LegendItemsCollection CustomItems
		{
			get
			{
				return this.customLegends;
			}
		}

		[SRDescription("DescriptionAttributeLegend_TextWrapThreshold")]
		[DefaultValue(25)]
		[SRCategory("CategoryAttributeAppearance")]
		public int TextWrapThreshold
		{
			get
			{
				return this.textWrapThreshold;
			}
			set
			{
				if (value != this.textWrapThreshold)
				{
					if (value < 0)
					{
						throw new ArgumentException(SR.ExceptionTextThresholdIsNegative, "value");
					}
					this.textWrapThreshold = value;
					this.Invalidate(false);
				}
			}
		}

		[DefaultValue(AutoBool.Auto)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeLegend_Reversed")]
		public AutoBool Reversed
		{
			get
			{
				return this.reversed;
			}
			set
			{
				if (value != this.reversed)
				{
					this.reversed = value;
					this.Invalidate(false);
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeLegend_InterlacedRows")]
		public bool InterlacedRows
		{
			get
			{
				return this.interlacedRows;
			}
			set
			{
				if (value != this.interlacedRows)
				{
					this.interlacedRows = value;
					this.Invalidate(false);
				}
			}
		}

		[SRDescription("DescriptionAttributeLegend_InterlacedRowsColor")]
		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAttributeAppearance")]
		public Color InterlacedRowsColor
		{
			get
			{
				return this.interlacedRowsColor;
			}
			set
			{
				if (value != this.interlacedRowsColor)
				{
					this.interlacedRowsColor = value;
					this.Invalidate(false);
				}
			}
		}

		[SRCategory("CategoryAttributeTitle")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeLegend_Title")]
		public string Title
		{
			get
			{
				return this.title;
			}
			set
			{
				if (value != this.title)
				{
					this.title = value;
					this.Invalidate(false);
				}
			}
		}

		[DefaultValue(typeof(Color), "Black")]
		[SRCategory("CategoryAttributeTitle")]
		[SRDescription("DescriptionAttributeLegend_TitleColor")]
		public Color TitleColor
		{
			get
			{
				return this.titleColor;
			}
			set
			{
				if (value != this.titleColor)
				{
					this.titleColor = value;
					this.Invalidate(false);
				}
			}
		}

		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAttributeTitle")]
		[SRDescription("DescriptionAttributeLegend_TitleBackColor")]
		public Color TitleBackColor
		{
			get
			{
				return this.titleBackColor;
			}
			set
			{
				if (value != this.titleBackColor)
				{
					this.titleBackColor = value;
					this.Invalidate(false);
				}
			}
		}

		[SRCategory("CategoryAttributeTitle")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt, style=Bold")]
		[SRDescription("DescriptionAttributeLegend_TitleFont")]
		public Font TitleFont
		{
			get
			{
				return this.titleFont;
			}
			set
			{
				if (value != this.titleFont)
				{
					this.titleFont = value;
					this.Invalidate(false);
				}
			}
		}

		[DefaultValue(typeof(StringAlignment), "Center")]
		[SRCategory("CategoryAttributeTitle")]
		[SRDescription("DescriptionAttributeLegend_TitleAlignment")]
		public StringAlignment TitleAlignment
		{
			get
			{
				return this.titleAlignment;
			}
			set
			{
				if (value != this.titleAlignment)
				{
					this.titleAlignment = value;
					this.Invalidate(false);
				}
			}
		}

		[SRCategory("CategoryAttributeTitle")]
		[DefaultValue(typeof(LegendSeparatorType), "None")]
		[SRDescription("DescriptionAttributeLegend_TitleSeparator")]
		public LegendSeparatorType TitleSeparator
		{
			get
			{
				return this.titleSeparator;
			}
			set
			{
				if (value != this.titleSeparator)
				{
					this.titleSeparator = value;
					this.Invalidate(false);
				}
			}
		}

		[SRDescription("DescriptionAttributeLegend_TitleSeparatorColor")]
		[DefaultValue(typeof(Color), "Black")]
		[SRCategory("CategoryAttributeTitle")]
		public Color TitleSeparatorColor
		{
			get
			{
				return this.titleSeparatorColor;
			}
			set
			{
				if (value != this.titleSeparatorColor)
				{
					this.titleSeparatorColor = value;
					this.Invalidate(false);
				}
			}
		}

		public Legend()
		{
			this.customLegends = new LegendItemsCollection();
			this.customLegends.legend = this;
			this.legendItems = new LegendItemsCollection();
			this.legendItems.legend = this;
			this.cellColumns = new LegendCellColumnCollection(this);
		}

		public Legend(CommonElements common)
		{
			base.Common = common;
			this.customLegends.common = common;
			this.position.common = common;
			this.customLegends = new LegendItemsCollection();
			this.customLegends.legend = this;
			this.legendItems = new LegendItemsCollection();
			this.legendItems.legend = this;
			this.cellColumns = new LegendCellColumnCollection(this);
		}

		public Legend(string name)
		{
			this.Name = name;
			this.customLegends = new LegendItemsCollection();
			this.customLegends.legend = this;
			this.legendItems = new LegendItemsCollection();
			this.legendItems.legend = this;
			this.cellColumns = new LegendCellColumnCollection(this);
		}

		public Legend(CommonElements common, string name)
		{
			this.Name = name;
			base.Common = common;
			this.position.common = common;
			this.customLegends = new LegendItemsCollection();
			this.customLegends.common = common;
			this.customLegends.legend = this;
			this.legendItems = new LegendItemsCollection();
			this.legendItems.legend = this;
			this.cellColumns = new LegendCellColumnCollection(this);
		}

		private void RecalcLegendInfo(ChartGraphics chartGraph)
		{
			RectangleF relative = this.position.ToRectangleF();
			Rectangle rectangle = Rectangle.Round(chartGraph.GetAbsoluteRectangle(relative));
			this.singleWCharacterSize = chartGraph.MeasureStringAbs("W", this.Font);
			Size size = chartGraph.MeasureStringAbs("WW", this.Font);
			this.singleWCharacterSize.Width = size.Width - this.singleWCharacterSize.Width;
			this.offset.Width = (int)Math.Ceiling((float)this.singleWCharacterSize.Width / 2.0);
			this.offset.Height = (int)Math.Ceiling((float)this.singleWCharacterSize.Width / 3.0);
			this.itemColumnSpacingRel = (int)((float)this.singleWCharacterSize.Width * ((float)this.itemColumnSpacing / 100.0));
			if (this.itemColumnSpacingRel % 2 == 1)
			{
				this.itemColumnSpacingRel++;
			}
			this.titlePosition = Rectangle.Empty;
			if (this.Title.Length > 0)
			{
				Size titleSize = this.GetTitleSize(chartGraph, rectangle.Size);
				this.titlePosition = new Rectangle(rectangle.Location.X, rectangle.Location.Y, rectangle.Width, Math.Min(rectangle.Height, titleSize.Height));
				rectangle.Height -= this.titlePosition.Height;
				this.titlePosition.Y += this.GetBorderSize();
			}
			this.headerPosition = Rectangle.Empty;
			Size empty = Size.Empty;
			foreach (LegendCellColumn cellColumn in this.CellColumns)
			{
				if (cellColumn.HeaderText.Length > 0)
				{
					Size headerSize = this.GetHeaderSize(chartGraph, cellColumn);
					empty.Height = Math.Max(empty.Height, headerSize.Height);
				}
			}
			if (!empty.IsEmpty)
			{
				this.headerPosition = new Rectangle(rectangle.Location.X + this.GetBorderSize() + this.offset.Width, rectangle.Location.Y + this.titlePosition.Height, rectangle.Width - (this.GetBorderSize() + this.offset.Width) * 2, Math.Min(rectangle.Height - this.titlePosition.Height, empty.Height));
				this.headerPosition.Height = Math.Max(this.headerPosition.Height, 0);
				rectangle.Height -= this.headerPosition.Height;
				this.headerPosition.Y += this.GetBorderSize();
			}
			this.legendItemsAreaPosition = new Rectangle(rectangle.X + this.offset.Width + this.GetBorderSize(), rectangle.Y + this.offset.Height + this.GetBorderSize() + this.titlePosition.Height + this.headerPosition.Height, rectangle.Width - 2 * (this.offset.Width + this.GetBorderSize()), rectangle.Height - 2 * (this.offset.Height + this.GetBorderSize()));
			this.GetNumberOfRowsAndColumns(chartGraph, this.legendItemsAreaPosition.Size, -1, out this.numberOfRowsPerColumn, out this.itemColumns, out this.horizontalSpaceLeft, out this.verticalSpaceLeft);
			this.autoFitFontSizeAdjustment = 0;
			this.legendItemsTruncated = false;
			bool flag = this.horizontalSpaceLeft >= 0 && this.verticalSpaceLeft >= 0;
			this.numberOfLegendItemsToProcess = this.legendItems.Count;
			int num = 0;
			for (int i = 0; i < this.itemColumns; i++)
			{
				num += this.numberOfRowsPerColumn[i];
			}
			if (num < this.numberOfLegendItemsToProcess)
			{
				flag = false;
			}
			this.autofitFont = new Font(this.Font, this.Font.Style);
			if (!flag)
			{
				do
				{
					if (this.AutoFitText && this.Font.Size - (float)this.autoFitFontSizeAdjustment > (float)this.autoFitMinFontSize)
					{
						this.autoFitFontSizeAdjustment++;
						int num2 = (int)Math.Round((double)(this.Font.Size - (float)this.autoFitFontSizeAdjustment));
						if (num2 < 1)
						{
							num2 = 1;
						}
						if (this.autofitFont != null)
						{
							this.autofitFont.Dispose();
							this.autofitFont = null;
						}
						this.autofitFont = new Font(this.Font.FontFamily, (float)num2, this.Font.Style, this.Font.Unit);
						this.GetNumberOfRowsAndColumns(chartGraph, this.legendItemsAreaPosition.Size, -1, out this.numberOfRowsPerColumn, out this.itemColumns, out this.horizontalSpaceLeft, out this.verticalSpaceLeft);
						flag = (this.horizontalSpaceLeft >= 0 && this.verticalSpaceLeft >= 0);
						num = 0;
						for (int j = 0; j < this.itemColumns; j++)
						{
							num += this.numberOfRowsPerColumn[j];
						}
						if (num < this.numberOfLegendItemsToProcess)
						{
							flag = false;
						}
					}
					else
					{
						if (this.numberOfLegendItemsToProcess > 2)
						{
							if (this.itemColumns == 1 && this.horizontalSpaceLeft < 0 && this.verticalSpaceLeft >= 0)
							{
								flag = true;
								this.numberOfLegendItemsToProcess = Math.Min(this.numberOfLegendItemsToProcess, this.numberOfRowsPerColumn[0]);
							}
							else if (this.GetMaximumNumberOfRows() == 1 && this.verticalSpaceLeft < 0 && this.horizontalSpaceLeft >= 0)
							{
								flag = true;
								this.numberOfLegendItemsToProcess = Math.Min(this.numberOfLegendItemsToProcess, this.itemColumns);
							}
							else
							{
								if (!this.legendItemsTruncated)
								{
									this.legendItemsAreaPosition.Height -= this.truncatedDotsSize;
								}
								this.legendItemsTruncated = true;
								this.numberOfLegendItemsToProcess--;
								this.GetNumberOfRowsAndColumns(chartGraph, this.legendItemsAreaPosition.Size, this.numberOfLegendItemsToProcess, out this.numberOfRowsPerColumn, out this.itemColumns);
							}
							if (flag && !this.legendItemsTruncated && this.numberOfLegendItemsToProcess < this.legendItems.Count)
							{
								this.legendItemsAreaPosition.Height -= this.truncatedDotsSize;
								this.legendItemsTruncated = true;
							}
						}
						else
						{
							flag = true;
						}
						if (!flag)
						{
							flag = this.CheckLegendItemsFit(chartGraph, this.legendItemsAreaPosition.Size, this.numberOfLegendItemsToProcess, this.autoFitFontSizeAdjustment, this.itemColumns, this.numberOfRowsPerColumn, out this.subColumnSizes, out this.cellHeights, out this.horizontalSpaceLeft, out this.verticalSpaceLeft);
						}
					}
				}
				while (!flag);
			}
			Size empty2 = Size.Empty;
			if (this.verticalSpaceLeft > 0)
			{
				empty2.Height = this.verticalSpaceLeft / this.GetMaximumNumberOfRows() / 2;
			}
			if (this.horizontalSpaceLeft > 0)
			{
				empty2.Width = this.horizontalSpaceLeft / 2;
			}
			int num3 = 0;
			int num4 = 0;
			if (this.numberOfLegendItemsToProcess < 0)
			{
				this.numberOfLegendItemsToProcess = this.legendItems.Count;
			}
			for (int k = 0; k < this.numberOfLegendItemsToProcess; k++)
			{
				LegendItem legendItem = this.legendItems[k];
				int num5;
				for (num5 = 0; num5 < legendItem.Cells.Count; num5++)
				{
					LegendCell legendCell = legendItem.Cells[num5];
					Rectangle cellPosition = this.GetCellPosition(chartGraph, num3, num4, num5, empty2);
					int num6 = 0;
					if (legendCell.CellSpan > 1)
					{
						for (int l = 1; l < legendCell.CellSpan && num5 + l < legendItem.Cells.Count; l++)
						{
							Rectangle cellPosition2 = this.GetCellPosition(chartGraph, num3, num4, num5 + l, empty2);
							if (cellPosition.Right < cellPosition2.Right)
							{
								cellPosition.Width += cellPosition2.Right - cellPosition.Right;
							}
							num6++;
							LegendCell legendCell2 = legendItem.Cells[num5 + l];
							legendCell2.SetCellPosition(chartGraph, num3, num4, Rectangle.Empty, this.autoFitFontSizeAdjustment, (this.autofitFont == null) ? this.Font : this.autofitFont, this.singleWCharacterSize);
						}
					}
					cellPosition.Intersect(this.legendItemsAreaPosition);
					legendCell.SetCellPosition(chartGraph, num3, num4, cellPosition, this.autoFitFontSizeAdjustment, (this.autofitFont == null) ? this.Font : this.autofitFont, this.singleWCharacterSize);
					num5 += num6;
				}
				num4++;
				if (num4 >= this.numberOfRowsPerColumn[num3])
				{
					num3++;
					num4 = 0;
					if (num3 >= this.itemColumns)
					{
						break;
					}
				}
			}
		}

		private Rectangle GetCellPosition(ChartGraphics chartGraph, int columnIndex, int rowIndex, int cellIndex, Size itemHalfSpacing)
		{
			Rectangle result = this.legendItemsAreaPosition;
			for (int i = 0; i < rowIndex; i++)
			{
				result.Y += this.cellHeights[columnIndex, i];
			}
			if (itemHalfSpacing.Height > 0)
			{
				result.Y += itemHalfSpacing.Height * rowIndex * 2 + itemHalfSpacing.Height;
			}
			if (this.horizontalSpaceLeft > 0)
			{
				result.X += itemHalfSpacing.Width;
			}
			int num = this.GetNumberOfCells();
			for (int j = 0; j < columnIndex; j++)
			{
				for (int k = 0; k < num; k++)
				{
					result.X += this.subColumnSizes[j, k];
				}
				result.X += this.GetSeparatorSize(chartGraph, this.ItemColumnSeparator).Width;
			}
			for (int l = 0; l < cellIndex; l++)
			{
				result.X += this.subColumnSizes[columnIndex, l];
			}
			result.Height = this.cellHeights[columnIndex, rowIndex];
			result.Width = this.subColumnSizes[columnIndex, cellIndex];
			return result;
		}

		private SizeF GetOptimalSize(ChartGraphics chartGraph, SizeF maxSizeRel)
		{
			this.offset = Size.Empty;
			this.itemColumns = 0;
			this.horizontalSpaceLeft = 0;
			this.verticalSpaceLeft = 0;
			this.subColumnSizes = null;
			this.numberOfRowsPerColumn = null;
			this.cellHeights = null;
			this.autofitFont = null;
			this.autoFitFontSizeAdjustment = 0;
			this.numberOfCells = -1;
			this.numberOfLegendItemsToProcess = -1;
			Size empty = Size.Empty;
			SizeF absoluteSize = chartGraph.GetAbsoluteSize(maxSizeRel);
			Size size = new Size((int)absoluteSize.Width, (int)absoluteSize.Height);
			foreach (LegendItem legendItem in this.legendItems)
			{
				foreach (LegendCell cell in legendItem.Cells)
				{
					cell.ResetCache();
				}
			}
			if (this.IsEnabled())
			{
				this.FillLegendItemsCollection();
				base.Common.EventsManager.OnCustomizeLegend(this.legendItems, this.Name);
				if (this.legendItems.Count > 0)
				{
					this.singleWCharacterSize = chartGraph.MeasureStringAbs("W", this.Font);
					Size size2 = chartGraph.MeasureStringAbs("WW", this.Font);
					this.singleWCharacterSize.Width = size2.Width - this.singleWCharacterSize.Width;
					this.offset.Width = (int)Math.Ceiling((float)this.singleWCharacterSize.Width / 2.0);
					this.offset.Height = (int)Math.Ceiling((float)this.singleWCharacterSize.Width / 3.0);
					this.itemColumnSpacingRel = (int)((float)this.singleWCharacterSize.Width * ((float)this.itemColumnSpacing / 100.0));
					if (this.itemColumnSpacingRel % 2 == 1)
					{
						this.itemColumnSpacingRel++;
					}
					Size size3 = Size.Empty;
					if (this.Title.Length > 0)
					{
						size3 = this.GetTitleSize(chartGraph, size);
					}
					Size empty2 = Size.Empty;
					foreach (LegendCellColumn cellColumn in this.CellColumns)
					{
						if (cellColumn.HeaderText.Length > 0)
						{
							Size headerSize = this.GetHeaderSize(chartGraph, cellColumn);
							empty2.Height = Math.Max(empty2.Height, headerSize.Height);
						}
					}
					Size legendSize = size;
					legendSize.Width -= 2 * (this.offset.Width + this.GetBorderSize());
					legendSize.Height -= 2 * (this.offset.Height + this.GetBorderSize());
					legendSize.Height -= size3.Height;
					legendSize.Height -= empty2.Height;
					this.autoFitFontSizeAdjustment = 0;
					this.autofitFont = new Font(this.Font, this.Font.Style);
					int num = 0;
					int num2 = 0;
					bool flag = this.AutoFitText;
					bool flag2 = false;
					do
					{
						this.GetNumberOfRowsAndColumns(chartGraph, legendSize, -1, out this.numberOfRowsPerColumn, out this.itemColumns, out num2, out num);
						int num3 = 0;
						for (int i = 0; i < this.itemColumns; i++)
						{
							num3 += this.numberOfRowsPerColumn[i];
						}
						flag2 = (num2 >= 0 && num >= 0 && num3 >= this.legendItems.Count);
						if (flag && !flag2)
						{
							if (this.Font.Size - (float)this.autoFitFontSizeAdjustment > (float)this.autoFitMinFontSize)
							{
								this.autoFitFontSizeAdjustment++;
								int num4 = (int)Math.Round((double)(this.Font.Size - (float)this.autoFitFontSizeAdjustment));
								if (num4 < 1)
								{
									num4 = 1;
								}
								if (this.autofitFont != null)
								{
									this.autofitFont.Dispose();
									this.autofitFont = null;
								}
								this.autofitFont = new Font(this.Font.FontFamily, (float)num4, this.Font.Style, this.Font.Unit);
							}
							else
							{
								flag = false;
							}
						}
					}
					while (flag && !flag2);
					num2 -= Math.Min(4, num2);
					num -= Math.Min(2, num);
					empty.Width = legendSize.Width - num2;
					empty.Width = Math.Max(empty.Width, size3.Width);
					empty.Width += 2 * (this.offset.Width + this.GetBorderSize());
					empty.Height = legendSize.Height - num + size3.Height + empty2.Height;
					empty.Height += 2 * (this.offset.Height + this.GetBorderSize());
					if (num2 < 0 || num < 0)
					{
						empty.Height += this.truncatedDotsSize;
					}
					if (empty.Width > size.Width)
					{
						empty.Width = size.Width;
					}
					if (empty.Height > size.Height)
					{
						empty.Height = size.Height;
					}
					if (empty.Width < 0)
					{
						empty.Width = 0;
					}
					if (empty.Height < 0)
					{
						empty.Height = 0;
					}
				}
			}
			return chartGraph.GetRelativeSize(empty);
		}

		internal void CalcLegendPosition(ChartGraphics chartGraph, ref RectangleF chartAreasRectangle, float maxLegendSize, float elementSpacing)
		{
			RectangleF rectangleF = default(RectangleF);
			SizeF maxSizeRel = new SizeF((float)(chartAreasRectangle.Width - 2.0 * elementSpacing), (float)(chartAreasRectangle.Height - 2.0 * elementSpacing));
			if (this.DockToChartArea == "NotSet")
			{
				if (this.Docking == LegendDocking.Top || this.Docking == LegendDocking.Bottom)
				{
					maxSizeRel.Height = (float)(maxSizeRel.Height / 100.0 * this.maximumLegendAutoSize);
				}
				else
				{
					maxSizeRel.Width = (float)(maxSizeRel.Width / 100.0 * this.maximumLegendAutoSize);
				}
			}
			if (!(maxSizeRel.Width <= 0.0) && !(maxSizeRel.Height <= 0.0))
			{
				SizeF optimalSize = this.GetOptimalSize(chartGraph, maxSizeRel);
				rectangleF.Height = optimalSize.Height;
				rectangleF.Width = optimalSize.Width;
				if (!float.IsNaN(optimalSize.Height) && !float.IsNaN(optimalSize.Width))
				{
					if (this.Docking == LegendDocking.Top)
					{
						rectangleF.Y = chartAreasRectangle.Y + elementSpacing;
						if (this.Alignment == StringAlignment.Near)
						{
							rectangleF.X = chartAreasRectangle.X + elementSpacing;
						}
						else if (this.Alignment == StringAlignment.Far)
						{
							rectangleF.X = chartAreasRectangle.Right - optimalSize.Width - elementSpacing;
						}
						else if (this.Alignment == StringAlignment.Center)
						{
							rectangleF.X = (float)(chartAreasRectangle.X + (chartAreasRectangle.Width - optimalSize.Width) / 2.0);
						}
						chartAreasRectangle.Height -= rectangleF.Height + elementSpacing;
						chartAreasRectangle.Y = rectangleF.Bottom;
					}
					else if (this.Docking == LegendDocking.Bottom)
					{
						rectangleF.Y = chartAreasRectangle.Bottom - optimalSize.Height - elementSpacing;
						if (this.Alignment == StringAlignment.Near)
						{
							rectangleF.X = chartAreasRectangle.X + elementSpacing;
						}
						else if (this.Alignment == StringAlignment.Far)
						{
							rectangleF.X = chartAreasRectangle.Right - optimalSize.Width - elementSpacing;
						}
						else if (this.Alignment == StringAlignment.Center)
						{
							rectangleF.X = (float)(chartAreasRectangle.X + (chartAreasRectangle.Width - optimalSize.Width) / 2.0);
						}
						chartAreasRectangle.Height -= rectangleF.Height + elementSpacing;
					}
					if (this.Docking == LegendDocking.Left)
					{
						rectangleF.X = chartAreasRectangle.X + elementSpacing;
						if (this.Alignment == StringAlignment.Near)
						{
							rectangleF.Y = chartAreasRectangle.Y + elementSpacing;
						}
						else if (this.Alignment == StringAlignment.Far)
						{
							rectangleF.Y = chartAreasRectangle.Bottom - optimalSize.Height - elementSpacing;
						}
						else if (this.Alignment == StringAlignment.Center)
						{
							rectangleF.Y = (float)(chartAreasRectangle.Y + (chartAreasRectangle.Height - optimalSize.Height) / 2.0);
						}
						chartAreasRectangle.Width -= rectangleF.Width + elementSpacing;
						chartAreasRectangle.X = rectangleF.Right;
					}
					if (this.Docking == LegendDocking.Right)
					{
						rectangleF.X = chartAreasRectangle.Right - optimalSize.Width - elementSpacing;
						if (this.Alignment == StringAlignment.Near)
						{
							rectangleF.Y = chartAreasRectangle.Y + elementSpacing;
						}
						else if (this.Alignment == StringAlignment.Far)
						{
							rectangleF.Y = chartAreasRectangle.Bottom - optimalSize.Height - elementSpacing;
						}
						else if (this.Alignment == StringAlignment.Center)
						{
							rectangleF.Y = (float)(chartAreasRectangle.Y + (chartAreasRectangle.Height - optimalSize.Height) / 2.0);
						}
						chartAreasRectangle.Width -= rectangleF.Width + elementSpacing;
					}
					this.Position.SetPositionNoAuto(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height);
				}
			}
		}

		private void GetNumberOfRowsAndColumns(ChartGraphics chartGraph, Size legendSize, int numberOfItemsToCheck, out int[] numberOfRowsPerColumn, out int columnNumber)
		{
			int num = 0;
			int num2 = 0;
			this.GetNumberOfRowsAndColumns(chartGraph, legendSize, numberOfItemsToCheck, out numberOfRowsPerColumn, out columnNumber, out num, out num2);
		}

		private void GetNumberOfRowsAndColumns(ChartGraphics chartGraph, Size legendSize, int numberOfItemsToCheck, out int[] numberOfRowsPerColumn, out int columnNumber, out int horSpaceLeft, out int vertSpaceLeft)
		{
			numberOfRowsPerColumn = null;
			columnNumber = 1;
			horSpaceLeft = 0;
			vertSpaceLeft = 0;
			if (numberOfItemsToCheck < 0)
			{
				numberOfItemsToCheck = this.legendItems.Count;
			}
			if (this.LegendStyle == LegendStyle.Column || numberOfItemsToCheck <= 1)
			{
				columnNumber = 1;
				numberOfRowsPerColumn = new int[1]
				{
					numberOfItemsToCheck
				};
			}
			else if (this.LegendStyle == LegendStyle.Row)
			{
				columnNumber = numberOfItemsToCheck;
				numberOfRowsPerColumn = new int[columnNumber];
				for (int i = 0; i < columnNumber; i++)
				{
					numberOfRowsPerColumn[i] = 1;
				}
			}
			else if (this.LegendStyle == LegendStyle.Table)
			{
				columnNumber = 1;
				numberOfRowsPerColumn = new int[1]
				{
					1
				};
				switch (this.GetLegendTableStyle(chartGraph))
				{
				case LegendTableStyle.Tall:
				{
					bool flag4 = false;
					int num6 = 1;
					for (num6 = 1; !flag4 && num6 < numberOfItemsToCheck; num6++)
					{
						numberOfRowsPerColumn[columnNumber - 1]++;
						if (!this.CheckLegendItemsFit(chartGraph, legendSize, num6 + 1, this.autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out this.subColumnSizes, out this.cellHeights, out horSpaceLeft, out vertSpaceLeft))
						{
							if (columnNumber != 1 && horSpaceLeft >= 0)
							{
								goto IL_0114;
							}
							if (vertSpaceLeft <= 0)
							{
								goto IL_0114;
							}
						}
						continue;
						IL_0114:
						if (numberOfRowsPerColumn[columnNumber - 1] > 1)
						{
							numberOfRowsPerColumn[columnNumber - 1]--;
						}
						int num7 = 0;
						if (horSpaceLeft > 0)
						{
							num7 = (int)Math.Round((double)(legendSize.Width - horSpaceLeft) / (double)columnNumber) / 2;
						}
						if (columnNumber < 50 && horSpaceLeft >= num7)
						{
							columnNumber++;
							int[] array2 = numberOfRowsPerColumn;
							numberOfRowsPerColumn = new int[columnNumber];
							for (int num8 = 0; num8 < array2.Length; num8++)
							{
								numberOfRowsPerColumn[num8] = array2[num8];
							}
							numberOfRowsPerColumn[columnNumber - 1] = 1;
							if (num6 == numberOfItemsToCheck - 1)
							{
								this.CheckLegendItemsFit(chartGraph, legendSize, num6 + 1, this.autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out this.subColumnSizes, out this.cellHeights, out horSpaceLeft, out vertSpaceLeft);
							}
						}
						else
						{
							flag4 = true;
						}
					}
					if (columnNumber > 1)
					{
						bool flag5 = false;
						while (!flag5)
						{
							flag5 = true;
							int num9 = -1;
							for (int num10 = 0; num10 < columnNumber; num10++)
							{
								int num11 = 0;
								for (int num12 = 0; num12 < this.numberOfRowsPerColumn[num10] - 1; num12++)
								{
									num11 += this.cellHeights[num10, num12];
								}
								num9 = Math.Max(num9, num11);
							}
							int num13 = 0;
							for (int num14 = 0; num14 < columnNumber - 1; num14++)
							{
								if (this.numberOfRowsPerColumn[num14] > 1)
								{
									num13 += this.cellHeights[num14, this.numberOfRowsPerColumn[num14] - 1];
								}
							}
							if (num13 > 0)
							{
								int columnHeight2 = this.GetColumnHeight(columnNumber - 1);
								if (columnHeight2 + num13 <= num9)
								{
									int num15 = 0;
									for (int num16 = 0; num16 < columnNumber - 1; num16++)
									{
										if (this.numberOfRowsPerColumn[num16] > 1)
										{
											this.numberOfRowsPerColumn[num16]--;
											num15++;
										}
									}
									if (num15 > 0)
									{
										this.numberOfRowsPerColumn[columnNumber - 1] += num15;
										this.CheckLegendItemsFit(chartGraph, legendSize, num6 + 1, this.autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out this.subColumnSizes, out this.cellHeights, out horSpaceLeft, out vertSpaceLeft);
										flag5 = false;
									}
								}
							}
						}
					}
					break;
				}
				case LegendTableStyle.Wide:
				{
					bool flag = false;
					int num = 1;
					for (num = 1; !flag && num < numberOfItemsToCheck; num++)
					{
						columnNumber++;
						int[] array = numberOfRowsPerColumn;
						numberOfRowsPerColumn = new int[columnNumber];
						for (int j = 0; j < array.Length; j++)
						{
							numberOfRowsPerColumn[j] = array[j];
						}
						numberOfRowsPerColumn[columnNumber - 1] = 1;
						bool flag2 = this.CheckLegendItemsFit(chartGraph, legendSize, num + 1, this.autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out this.subColumnSizes, out this.cellHeights, out horSpaceLeft, out vertSpaceLeft);
						if (!flag2)
						{
							if (this.GetMaximumNumberOfRows(numberOfRowsPerColumn) != 1 && vertSpaceLeft >= 0)
							{
								goto IL_03ee;
							}
							if (horSpaceLeft <= 0)
							{
								goto IL_03ee;
							}
						}
						continue;
						IL_03ee:
						bool flag3 = true;
						while (flag3)
						{
							flag3 = false;
							int num2 = 0;
							if (columnNumber > 1)
							{
								num2 = numberOfRowsPerColumn[columnNumber - 1];
								columnNumber--;
								array = numberOfRowsPerColumn;
								numberOfRowsPerColumn = new int[columnNumber];
								for (int k = 0; k < columnNumber; k++)
								{
									numberOfRowsPerColumn[k] = array[k];
								}
							}
							for (int l = 0; l < num2; l++)
							{
								int num3 = -1;
								int num4 = 2147483647;
								for (int m = 0; m < columnNumber; m++)
								{
									int columnHeight = this.GetColumnHeight(m);
									int num5 = 0;
									if (m < columnNumber - 1)
									{
										num5 = this.cellHeights[m + 1, 0];
									}
									if (columnHeight < num4 && columnHeight + num5 < legendSize.Height)
									{
										num4 = columnHeight;
										num3 = m;
									}
								}
								if (num3 < 0)
								{
									flag2 = this.CheckLegendItemsFit(chartGraph, legendSize, num + 1, this.autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out this.subColumnSizes, out this.cellHeights, out horSpaceLeft, out vertSpaceLeft);
									flag = true;
									break;
								}
								numberOfRowsPerColumn[num3]++;
								if (num3 < columnNumber - 1 && numberOfRowsPerColumn[num3 + 1] == 1)
								{
									array = numberOfRowsPerColumn;
									for (int n = num3 + 1; n < array.Length - 1; n++)
									{
										numberOfRowsPerColumn[n] = array[n + 1];
									}
									numberOfRowsPerColumn[columnNumber - 1] = 1;
								}
								flag2 = this.CheckLegendItemsFit(chartGraph, legendSize, num + 1, this.autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out this.subColumnSizes, out this.cellHeights, out horSpaceLeft, out vertSpaceLeft);
							}
							if (!flag2 && (float)horSpaceLeft < 0.0 && columnNumber > 1)
							{
								flag3 = true;
							}
						}
					}
					break;
				}
				}
			}
			this.CheckLegendItemsFit(chartGraph, legendSize, -1, this.autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out this.subColumnSizes, out this.cellHeights, out horSpaceLeft, out vertSpaceLeft);
		}

		private int GetColumnHeight(int columnIndex)
		{
			int num = 0;
			for (int i = 0; i < this.numberOfRowsPerColumn[columnIndex]; i++)
			{
				num += this.cellHeights[columnIndex, i];
			}
			return num;
		}

		private int GetMaximumNumberOfRows()
		{
			return this.GetMaximumNumberOfRows(this.numberOfRowsPerColumn);
		}

		private int GetMaximumNumberOfRows(int[] rowsPerColumn)
		{
			int num = 0;
			if (rowsPerColumn != null)
			{
				for (int i = 0; i < rowsPerColumn.Length; i++)
				{
					num = Math.Max(num, rowsPerColumn[i]);
				}
			}
			return num;
		}

		private bool CheckLegendItemsFit(ChartGraphics graph, Size legendItemsAreaSize, int numberOfItemsToCheck, int fontSizeReducedBy, int numberOfColumns, int[] numberOfRowsPerColumn, out int[,] subColumnSizes, out int[,] cellHeights, out int horizontalSpaceLeft, out int verticalSpaceLeft)
		{
			bool result = true;
			horizontalSpaceLeft = 0;
			verticalSpaceLeft = 0;
			if (numberOfItemsToCheck < 0)
			{
				numberOfItemsToCheck = this.legendItems.Count;
			}
			int num = this.GetNumberOfCells();
			int maximumNumberOfRows = this.GetMaximumNumberOfRows(numberOfRowsPerColumn);
			int[,,] array = new int[numberOfColumns, maximumNumberOfRows, num];
			cellHeights = new int[numberOfColumns, maximumNumberOfRows];
			this.singleWCharacterSize = graph.MeasureStringAbs("W", (this.autofitFont == null) ? this.Font : this.autofitFont);
			Size size = graph.MeasureStringAbs("WW", (this.autofitFont == null) ? this.Font : this.autofitFont);
			this.singleWCharacterSize.Width = size.Width - this.singleWCharacterSize.Width;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < numberOfItemsToCheck; i++)
			{
				LegendItem legendItem = this.legendItems[i];
				int num4 = 0;
				for (int j = 0; j < legendItem.Cells.Count; j++)
				{
					LegendCell legendCell = legendItem.Cells[j];
					LegendCellColumn legendCellColumn = null;
					if (j < this.CellColumns.Count)
					{
						legendCellColumn = this.CellColumns[j];
					}
					if (num4 > 0)
					{
						array[num2, num3, j] = -1;
						num4--;
					}
					else
					{
						if (legendCell.CellSpan > 1)
						{
							num4 = legendCell.CellSpan - 1;
						}
						Size size2 = legendCell.MeasureCell(graph, fontSizeReducedBy, (this.autofitFont == null) ? this.Font : this.autofitFont, this.singleWCharacterSize);
						if (legendCellColumn != null)
						{
							if (legendCellColumn.MinimumWidth >= 0)
							{
								size2.Width = (int)Math.Max((float)size2.Width, (float)((float)(legendCellColumn.MinimumWidth * this.singleWCharacterSize.Width) / 100.0));
							}
							if (legendCellColumn.MaximumWidth >= 0)
							{
								size2.Width = (int)Math.Min((float)size2.Width, (float)((float)(legendCellColumn.MaximumWidth * this.singleWCharacterSize.Width) / 100.0));
							}
						}
						int[,,] array2 = array;
						int num5 = num2;
						int num6 = num3;
						int num7 = j;
						int width = size2.Width;
						array2[num5, num6, num7] = width;
						if (j == 0)
						{
							int[,] obj = cellHeights;
							int num8 = num2;
							int num9 = num3;
							int height = size2.Height;
							obj[num8, num9] = height;
						}
						else
						{
							int[,] obj2 = cellHeights;
							int num10 = num2;
							int num11 = num3;
							int num12 = Math.Max(cellHeights[num2, num3], size2.Height);
							obj2[num10, num11] = num12;
						}
					}
				}
				num3++;
				if (num3 >= numberOfRowsPerColumn[num2])
				{
					num2++;
					num3 = 0;
					if (num2 >= numberOfColumns)
					{
						if (i < numberOfItemsToCheck - 1)
						{
							result = false;
						}
						break;
					}
				}
			}
			subColumnSizes = new int[numberOfColumns, num];
			bool flag = false;
			for (num2 = 0; num2 < numberOfColumns; num2++)
			{
				for (int k = 0; k < num; k++)
				{
					int num13 = 0;
					for (num3 = 0; num3 < numberOfRowsPerColumn[num2]; num3++)
					{
						int num14 = array[num2, num3, k];
						if (num14 < 0)
						{
							flag = true;
							continue;
						}
						if (k + 1 < num)
						{
							int num15 = array[num2, num3, k + 1];
							if (num15 >= 0)
							{
								goto IL_02cf;
							}
							continue;
						}
						goto IL_02cf;
						IL_02cf:
						num13 = Math.Max(num13, num14);
					}
					subColumnSizes[num2, k] = num13;
				}
			}
			for (num2 = 0; num2 < numberOfColumns; num2++)
			{
				for (int l = 0; l < num; l++)
				{
					if (l < this.CellColumns.Count)
					{
						LegendCellColumn legendCellColumn2 = this.CellColumns[l];
						if (legendCellColumn2.HeaderText.Length > 0)
						{
							Size size3 = graph.MeasureStringAbs(legendCellColumn2.HeaderText + "I", legendCellColumn2.HeaderFont);
							if (size3.Width > subColumnSizes[num2, l])
							{
								int[,] obj3 = subColumnSizes;
								int num16 = num2;
								int num17 = l;
								int width2 = size3.Width;
								obj3[num16, num17] = width2;
								if (legendCellColumn2.MinimumWidth >= 0)
								{
									int[,] obj4 = subColumnSizes;
									int num18 = num2;
									int num19 = l;
									int num20 = (int)Math.Max((float)subColumnSizes[num2, l], (float)((float)(legendCellColumn2.MinimumWidth * this.singleWCharacterSize.Width) / 100.0));
									obj4[num18, num19] = num20;
								}
								if (legendCellColumn2.MaximumWidth >= 0)
								{
									int[,] obj5 = subColumnSizes;
									int num21 = num2;
									int num22 = l;
									int num23 = (int)Math.Min((float)subColumnSizes[num2, l], (float)((float)(legendCellColumn2.MaximumWidth * this.singleWCharacterSize.Width) / 100.0));
									obj5[num21, num22] = num23;
								}
							}
						}
					}
				}
			}
			if (flag)
			{
				for (num2 = 0; num2 < numberOfColumns; num2++)
				{
					for (int m = 0; m < num; m++)
					{
						for (num3 = 0; num3 < numberOfRowsPerColumn[num2]; num3++)
						{
							int num24 = array[num2, num3, m];
							int n;
							for (n = 0; m + n + 1 < num; n++)
							{
								int num25 = array[num2, num3, m + n + 1];
								if (num25 >= 0)
								{
									break;
								}
							}
							if (n > 0)
							{
								int num26 = 0;
								for (int num27 = 0; num27 <= n; num27++)
								{
									num26 += subColumnSizes[num2, m + num27];
								}
								if (num24 > num26)
								{
									subColumnSizes[num2, m + n] += num24 - num26;
								}
							}
						}
					}
				}
			}
			if (this.EquallySpacedItems)
			{
				for (int num28 = 0; num28 < num; num28++)
				{
					int num29 = 0;
					for (num2 = 0; num2 < numberOfColumns; num2++)
					{
						num29 = Math.Max(num29, subColumnSizes[num2, num28]);
					}
					for (num2 = 0; num2 < numberOfColumns; num2++)
					{
						subColumnSizes[num2, num28] = num29;
					}
				}
			}
			int num30 = 0;
			int num31 = 0;
			for (num2 = 0; num2 < numberOfColumns; num2++)
			{
				for (int num32 = 0; num32 < num; num32++)
				{
					num30 += subColumnSizes[num2, num32];
				}
				if (num2 < numberOfColumns - 1)
				{
					num31 += this.GetSeparatorSize(graph, this.ItemColumnSeparator).Width;
				}
			}
			int num33 = 0;
			for (num2 = 0; num2 < numberOfColumns; num2++)
			{
				int num34 = 0;
				for (num3 = 0; num3 < numberOfRowsPerColumn[num2]; num3++)
				{
					num34 += cellHeights[num2, num3];
				}
				num33 = Math.Max(num33, num34);
			}
			horizontalSpaceLeft = legendItemsAreaSize.Width - num30 - num31;
			if (horizontalSpaceLeft < 0)
			{
				result = false;
			}
			verticalSpaceLeft = legendItemsAreaSize.Height - num33;
			if (verticalSpaceLeft < 0)
			{
				result = false;
			}
			return result;
		}

		private int GetNumberOfCells()
		{
			if (this.numberOfCells < 0)
			{
				this.numberOfCells = this.CellColumns.Count;
				foreach (LegendItem legendItem in this.legendItems)
				{
					this.numberOfCells = Math.Max(this.numberOfCells, legendItem.Cells.Count);
				}
			}
			return this.numberOfCells;
		}

		private void FillLegendItemsCollection()
		{
			this.legendItems.Clear();
			foreach (Series item in base.Common.DataManager.Series)
			{
				try
				{
					Legend legend = base.Common.ChartPicture.Legends[item.Legend];
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionLegendReferencedInSeriesNotFound(item.Name, item.Legend));
				}
			}
			bool flag = false;
			foreach (Series item2 in base.Common.DataManager.Series)
			{
				if (base.Common.ChartPicture.Legends[item2.Legend] == this && item2.ChartArea.Length > 0)
				{
					bool flag2 = false;
					foreach (ChartArea chartArea in base.Common.ChartPicture.ChartAreas)
					{
						if (chartArea.Name == item2.ChartArea)
						{
							flag2 = true;
							break;
						}
					}
					if (item2.IsVisible() && flag2)
					{
						IChartType chartType = base.Common.ChartTypeRegistry.GetChartType(item2.ChartTypeName);
						if (this.Reversed == AutoBool.Auto && (item2.ChartType == SeriesChartType.StackedArea || item2.ChartType == SeriesChartType.StackedArea100 || item2.ChartType == SeriesChartType.Pyramid || item2.ChartType == SeriesChartType.StackedColumn || item2.ChartType == SeriesChartType.StackedColumn100))
						{
							flag = true;
						}
						if (chartType.DataPointsInLegend)
						{
							bool flag3 = false;
							foreach (DataPoint point in item2.Points)
							{
								if (point.XValue != 0.0)
								{
									flag3 = true;
									break;
								}
							}
							int num = 0;
							foreach (DataPoint point2 in item2.Points)
							{
								if (point2.Empty)
								{
									num++;
								}
								else if (!point2.ShowInLegend)
								{
									num++;
								}
								else
								{
									LegendItem legendItem = new LegendItem(point2.Label, point2.Color, "");
									bool enable3D = base.Common.Chart.ChartAreas[item2.ChartArea].Area3DStyle.Enable3D;
									legendItem.SetAttributes(base.Common, item2);
									legendItem.SetAttributes(point2, enable3D);
									legendItem.ToolTip = point2.ReplaceKeywords(point2.LegendToolTip);
									legendItem.MapAreaAttributes = point2.ReplaceKeywords(point2.LegendMapAreaAttributes);
									legendItem.Href = point2.ReplaceKeywords(point2.LegendHref);
									((IMapAreaAttributes)legendItem).Tag = point2.LegendTag;
									legendItem.Name = point2.ReplaceKeywords(point2.LegendText);
									legendItem.SeriesPointIndex = num++;
									if (legendItem.Name.Length == 0)
									{
										legendItem.Name = point2.ReplaceKeywords((point2.Label.Length > 0) ? point2.Label : point2.AxisLabel);
									}
									if (legendItem.Name.Length == 0 && flag3)
									{
										legendItem.Name = ValueConverter.FormatValue(item2.chart, this, point2.XValue, "", point2.series.XValueType, ChartElementType.LegendItem);
									}
									if (legendItem.Name.Length == 0)
									{
										legendItem.Name = "Point " + num;
									}
									legendItem.AddAutomaticCells(this);
									foreach (LegendCell cell in legendItem.Cells)
									{
										if (cell.Text.Length > 0)
										{
											cell.Text = cell.Text.Replace("#LEGENDTEXT", legendItem.Name);
											cell.Text = point2.ReplaceKeywords(cell.Text);
											cell.ToolTip = point2.ReplaceKeywords(cell.ToolTip);
											cell.Href = point2.ReplaceKeywords(cell.Href);
											cell.MapAreaAttributes = point2.ReplaceKeywords(cell.MapAreaAttributes);
										}
									}
									if (item2.chart != null && item2.chart.LocalizeTextHandler != null)
									{
										legendItem.Name = item2.chart.LocalizeTextHandler(this, legendItem.Name, point2.ElementId, ChartElementType.LegendItem);
									}
									this.legendItems.Add(legendItem);
								}
							}
						}
						else if (item2.ShowInLegend)
						{
							LegendItem legendItem2 = new LegendItem(item2.Name, item2.Color, "");
							legendItem2.SetAttributes(base.Common, item2);
							legendItem2.ToolTip = item2.ReplaceKeywords(item2.LegendToolTip);
							legendItem2.Href = item2.ReplaceKeywords(item2.LegendHref);
							legendItem2.MapAreaAttributes = item2.ReplaceKeywords(item2.LegendMapAreaAttributes);
							((IMapAreaAttributes)legendItem2).Tag = item2.LegendTag;
							if (item2.LegendText.Length > 0)
							{
								legendItem2.Name = item2.ReplaceKeywords(item2.LegendText);
							}
							legendItem2.AddAutomaticCells(this);
							foreach (LegendCell cell2 in legendItem2.Cells)
							{
								if (cell2.Text.Length > 0)
								{
									cell2.Text = cell2.Text.Replace("#LEGENDTEXT", legendItem2.Name);
									cell2.Text = item2.ReplaceKeywords(cell2.Text);
									cell2.ToolTip = item2.ReplaceKeywords(cell2.ToolTip);
									cell2.Href = item2.ReplaceKeywords(cell2.Href);
									cell2.MapAreaAttributes = item2.ReplaceKeywords(cell2.MapAreaAttributes);
								}
							}
							if (item2.chart != null && item2.chart.LocalizeTextHandler != null)
							{
								legendItem2.Name = item2.chart.LocalizeTextHandler(this, legendItem2.Name, item2.ElementId, ChartElementType.LegendItem);
							}
							this.legendItems.Add(legendItem2);
						}
					}
				}
			}
			if (this.Reversed == AutoBool.True || (this.Reversed == AutoBool.Auto && flag))
			{
				this.legendItems.Reverse();
			}
			foreach (LegendItem customLegend in this.customLegends)
			{
				if (customLegend.Enabled)
				{
					this.legendItems.Add(customLegend);
				}
			}
			if (this.legendItems.Count == 0 && base.Common != null && base.Common.Chart != null && base.Common.Chart.IsDesignMode())
			{
				LegendItem legendItem4 = new LegendItem(this.Name + " - " + SR.DescriptionTypeEmpty, Color.White, "");
				legendItem4.Style = LegendImageStyle.Line;
				this.legendItems.Add(legendItem4);
			}
			foreach (LegendItem legendItem5 in this.legendItems)
			{
				legendItem5.AddAutomaticCells(this);
			}
		}

		internal void Paint(ChartGraphics chartGraph)
		{
			this.offset = Size.Empty;
			this.itemColumns = 0;
			this.horizontalSpaceLeft = 0;
			this.verticalSpaceLeft = 0;
			this.subColumnSizes = null;
			this.numberOfRowsPerColumn = null;
			this.cellHeights = null;
			this.autofitFont = null;
			this.autoFitFontSizeAdjustment = 0;
			this.numberOfCells = -1;
			this.numberOfLegendItemsToProcess = -1;
			if (this.IsEnabled())
			{
				if (this.MaxAutoSize == 0.0 && this.Position.Auto)
				{
					return;
				}
				base.Common.TraceWrite("ChartPainting", SR.TraceMessageBeginDrawingChartLegend);
				this.FillLegendItemsCollection();
				foreach (LegendItem legendItem4 in this.legendItems)
				{
					foreach (LegendCell cell in legendItem4.Cells)
					{
						cell.ResetCache();
					}
				}
				base.Common.EventsManager.OnCustomizeLegend(this.legendItems, this.Name);
				if (this.legendItems.Count != 0)
				{
					this.RecalcLegendInfo(chartGraph);
					this.animationLocationAdjustment = PointF.Empty;
					if (base.Common.ProcessModePaint)
					{
						chartGraph.StartAnimation();
						chartGraph.FillRectangleRel(chartGraph.GetRelativeRectangle(Rectangle.Round(chartGraph.GetAbsoluteRectangle(this.Position.ToRectangleF()))), this.BackColor, this.BackHatchStyle, this.BackImage, this.BackImageMode, this.BackImageTransparentColor, this.BackImageAlign, this.BackGradientType, this.BackGradientEndColor, this.BorderColor, this.GetBorderSize(), this.BorderStyle, this.ShadowColor, this.ShadowOffset, PenAlignment.Inset);
						chartGraph.StopAnimation();
						base.Common.EventsManager.OnBackPaint(this, new ChartPaintEventArgs(chartGraph, base.Common, this.Position));
					}
					if (base.Common.ProcessModeRegions)
					{
						this.SelectLegendBackground(chartGraph);
					}
					this.DrawLegendHeader(chartGraph);
					this.DrawLegendTitle(chartGraph);
					if (base.Common.ProcessModeRegions && !this.titlePosition.IsEmpty)
					{
						base.Common.HotRegionsList.AddHotRegion(chartGraph.GetRelativeRectangle(this.titlePosition), this, ChartElementType.LegendTitle, true);
					}
					if (this.numberOfLegendItemsToProcess < 0)
					{
						this.numberOfLegendItemsToProcess = this.legendItems.Count;
					}
					for (int i = 0; i < this.numberOfLegendItemsToProcess; i++)
					{
						LegendItem legendItem2 = this.legendItems[i];
						for (int j = 0; j < legendItem2.Cells.Count; j++)
						{
							LegendCell legendCell2 = legendItem2.Cells[j];
							legendCell2.Paint(chartGraph, this.autoFitFontSizeAdjustment, this.autofitFont, this.singleWCharacterSize, this.animationLocationAdjustment);
						}
						if (legendItem2.Separator != 0 && legendItem2.Cells.Count > 0)
						{
							Rectangle empty = Rectangle.Empty;
							empty.X = legendItem2.Cells[0].cellPosition.Left;
							int num = 0;
							for (int num2 = legendItem2.Cells.Count - 1; num2 >= 0; num2--)
							{
								num = legendItem2.Cells[num2].cellPosition.Right;
								if (num > 0)
								{
									break;
								}
							}
							empty.Width = num - empty.X;
							empty.Y = legendItem2.Cells[0].cellPosition.Bottom;
							empty.Height = this.GetSeparatorSize(chartGraph, legendItem2.Separator).Height;
							empty.Intersect(this.legendItemsAreaPosition);
							this.DrawSeparator(chartGraph, legendItem2.Separator, legendItem2.SeparatorColor, true, empty);
						}
					}
					if (this.ItemColumnSeparator != 0)
					{
						Rectangle rectangle = Rectangle.Round(chartGraph.GetAbsoluteRectangle(this.Position.ToRectangleF()));
						rectangle.Y += this.GetBorderSize() + this.titlePosition.Height;
						rectangle.Height -= 2 * this.GetBorderSize() + this.titlePosition.Height;
						rectangle.X += this.GetBorderSize() + this.offset.Width;
						rectangle.Width = this.GetSeparatorSize(chartGraph, this.ItemColumnSeparator).Width;
						if (this.horizontalSpaceLeft > 0)
						{
							rectangle.X += this.horizontalSpaceLeft / 2;
						}
						if (rectangle.Width > 0 && rectangle.Height > 0)
						{
							for (int k = 0; k < this.itemColumns; k++)
							{
								int num3 = this.GetNumberOfCells();
								for (int l = 0; l < num3; l++)
								{
									rectangle.X += this.subColumnSizes[k, l];
								}
								if (k < this.itemColumns - 1)
								{
									this.DrawSeparator(chartGraph, this.ItemColumnSeparator, this.ItemColumnSeparatorColor, false, rectangle);
								}
								rectangle.X += rectangle.Width;
							}
						}
					}
					if (this.legendItemsTruncated && this.legendItemsAreaPosition.Height > this.truncatedDotsSize / 2)
					{
						chartGraph.StartAnimation();
						int num4 = 3;
						int val = this.legendItemsAreaPosition.Width / 3 / num4;
						val = Math.Min(val, 10);
						PointF absolute = new PointF((float)(this.legendItemsAreaPosition.X + this.legendItemsAreaPosition.Width / 2) - (float)val * (float)Math.Floor((float)num4 / 2.0), (float)(this.legendItemsAreaPosition.Bottom + (this.truncatedDotsSize + this.offset.Height) / 2));
						for (int m = 0; m < num4; m++)
						{
							chartGraph.DrawMarkerRel(chartGraph.GetRelativePoint(absolute), MarkerStyle.Circle, this.truncatedDotsSize, this.FontColor, Color.Empty, 0, string.Empty, Color.Empty, 0, Color.Empty, RectangleF.Empty);
							absolute.X += (float)val;
						}
						chartGraph.StopAnimation();
					}
					if (base.Common.ProcessModePaint)
					{
						base.Common.EventsManager.OnPaint(this, new ChartPaintEventArgs(chartGraph, base.Common, this.Position));
					}
					base.Common.TraceWrite("ChartPainting", SR.TraceMessageEndDrawingChartLegend);
					foreach (LegendItem legendItem5 in this.legendItems)
					{
						if (legendItem5.clearTempCells)
						{
							legendItem5.clearTempCells = false;
							legendItem5.Cells.Clear();
						}
					}
				}
			}
		}

		private Size GetTitleSize(ChartGraphics chartGraph, Size titleMaxSize)
		{
			Size result = Size.Empty;
			if (this.Title.Length > 0)
			{
				titleMaxSize.Width -= this.GetBorderSize() * 2 + this.offset.Width;
				result = chartGraph.MeasureStringAbs(this.Title.Replace("\\n", "\n"), this.TitleFont, titleMaxSize, new StringFormat());
				result.Height += this.offset.Height;
				result.Width += this.offset.Width;
				result.Height += this.GetSeparatorSize(chartGraph, this.TitleSeparator).Height;
			}
			return result;
		}

		private Size GetHeaderSize(ChartGraphics chartGraph, LegendCellColumn legendColumn)
		{
			Size result = Size.Empty;
			if (legendColumn.HeaderText.Length > 0)
			{
				result = chartGraph.MeasureStringAbs(legendColumn.HeaderText.Replace("\\n", "\n") + "I", legendColumn.HeaderFont);
				result.Height += this.offset.Height;
				result.Width += this.offset.Width;
				result.Height += this.GetSeparatorSize(chartGraph, this.HeaderSeparator).Height;
			}
			return result;
		}

		private void DrawLegendHeader(ChartGraphics chartGraph)
		{
			if (!this.headerPosition.IsEmpty && this.headerPosition.Width > 0 && this.headerPosition.Height > 0)
			{
				int num = -1;
				bool flag = false;
				Rectangle rect = Rectangle.Round(chartGraph.GetAbsoluteRectangle(this.Position.ToRectangleF()));
				rect.Y += this.GetBorderSize();
				rect.Height -= 2 * (this.offset.Height + this.GetBorderSize());
				rect.X += this.GetBorderSize();
				rect.Width -= 2 * this.GetBorderSize();
				if (this.GetBorderSize() > 0)
				{
					rect.Height++;
					rect.Width++;
				}
				bool flag2 = false;
				for (int i = 0; i < this.CellColumns.Count; i++)
				{
					LegendCellColumn legendCellColumn = this.CellColumns[i];
					if (!legendCellColumn.HeaderBackColor.IsEmpty)
					{
						flag2 = true;
					}
				}
				for (int j = 0; j < this.itemColumns; j++)
				{
					int num2 = 0;
					int num3 = 0;
					int length = this.subColumnSizes.GetLength(1);
					for (int k = 0; k < length; k++)
					{
						Rectangle rectangle = this.headerPosition;
						if (this.horizontalSpaceLeft > 0)
						{
							rectangle.X += (int)((float)this.horizontalSpaceLeft / 2.0);
						}
						if (num != -1)
						{
							rectangle.X = num;
						}
						rectangle.Width = this.subColumnSizes[j, k];
						num = rectangle.Right;
						if (k == 0)
						{
							num2 = rectangle.Left;
						}
						num3 += rectangle.Width;
						rectangle.Intersect(rect);
						Rectangle r;
						if (rectangle.Width > 0 && rectangle.Height > 0)
						{
							r = rectangle;
							if (this.titlePosition.Height <= 0)
							{
								r.Y -= this.offset.Height;
								r.Height += this.offset.Height;
							}
							if (this.itemColumns == 1 && flag2)
							{
								goto IL_0229;
							}
							if (this.ItemColumnSeparator != 0)
							{
								goto IL_0229;
							}
							goto IL_033c;
						}
						continue;
						IL_033c:
						if (k < this.CellColumns.Count)
						{
							LegendCellColumn legendCellColumn2 = this.CellColumns[k];
							if (!legendCellColumn2.HeaderBackColor.IsEmpty)
							{
								flag = true;
								chartGraph.StartAnimation();
								if (r.Right > rect.Right)
								{
									r.Width -= rect.Right - r.Right;
								}
								if (r.X < rect.X)
								{
									r.X += rect.X - r.X;
									r.Width -= rect.X - r.X;
								}
								r.Intersect(rect);
								chartGraph.FillRectangleRel(chartGraph.GetRelativeRectangle(r), legendCellColumn2.HeaderBackColor, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, Color.Empty, 0, ChartDashStyle.NotSet, Color.Empty, 0, PenAlignment.Inset);
								chartGraph.StopAnimation();
							}
							using (SolidBrush brush = new SolidBrush(legendCellColumn2.HeaderColor))
							{
								StringFormat stringFormat = new StringFormat();
								stringFormat.Alignment = legendCellColumn2.HeaderTextAlignment;
								stringFormat.LineAlignment = StringAlignment.Center;
								stringFormat.FormatFlags = StringFormatFlags.LineLimit;
								stringFormat.Trimming = StringTrimming.EllipsisCharacter;
								chartGraph.StartAnimation();
								chartGraph.DrawStringRel(legendCellColumn2.HeaderText, legendCellColumn2.HeaderFont, brush, chartGraph.GetRelativeRectangle(rectangle), stringFormat);
								chartGraph.StopAnimation();
							}
						}
						continue;
						IL_0229:
						if (j == 0 && k == 0)
						{
							int x = rect.X;
							num3 += num2 - x;
							num2 = x;
							r.Width += r.X - rect.X;
							r.X = x;
						}
						if (j == this.itemColumns - 1 && k == length - 1)
						{
							num3 += rect.Right - r.Right + 1;
							r.Width += rect.Right - r.Right + 1;
						}
						if (j != 0 && k == 0)
						{
							num3 += this.itemColumnSpacingRel / 2;
							num2 -= this.itemColumnSpacingRel / 2;
							r.Width += this.itemColumnSpacingRel / 2;
							r.X -= this.itemColumnSpacingRel / 2;
						}
						if (j != this.itemColumns - 1 && k == length - 1)
						{
							num3 += this.itemColumnSpacingRel / 2;
							r.Width += this.itemColumnSpacingRel / 2;
						}
						goto IL_033c;
					}
					Rectangle rectangle2 = this.headerPosition;
					rectangle2.X = num2;
					rectangle2.Width = num3;
					if (this.HeaderSeparator == LegendSeparatorType.Line || this.HeaderSeparator == LegendSeparatorType.DoubleLine)
					{
						rect.Width--;
					}
					rectangle2.Intersect(rect);
					this.DrawSeparator(chartGraph, this.HeaderSeparator, this.HeaderSeparatorColor, true, rectangle2);
					num += this.GetSeparatorSize(chartGraph, this.ItemColumnSeparator).Width;
				}
				if (flag)
				{
					chartGraph.StartAnimation();
					chartGraph.FillRectangleRel(chartGraph.GetRelativeRectangle(Rectangle.Round(chartGraph.GetAbsoluteRectangle(this.Position.ToRectangleF()))), Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, this.BorderColor, this.GetBorderSize(), this.BorderStyle, Color.Empty, 0, PenAlignment.Inset);
					chartGraph.StopAnimation();
				}
				if (base.Common.ProcessModeRegions && !this.headerPosition.IsEmpty)
				{
					base.Common.HotRegionsList.AddHotRegion(chartGraph.GetRelativeRectangle(this.headerPosition), this, ChartElementType.LegendHeader, true);
				}
			}
		}

		private void DrawLegendTitle(ChartGraphics chartGraph)
		{
			if (this.Title.Length > 0 && !this.titlePosition.IsEmpty)
			{
				Rectangle rect = Rectangle.Round(chartGraph.GetAbsoluteRectangle(this.Position.ToRectangleF()));
				rect.Y += this.GetBorderSize();
				rect.Height -= 2 * this.GetBorderSize();
				rect.X += this.GetBorderSize();
				rect.Width -= 2 * this.GetBorderSize();
				if (this.GetBorderSize() > 0)
				{
					rect.Height++;
					rect.Width++;
				}
				if (!this.TitleBackColor.IsEmpty)
				{
					chartGraph.StartAnimation();
					Rectangle r = this.titlePosition;
					r.Intersect(rect);
					chartGraph.FillRectangleRel(chartGraph.GetRelativeRectangle(r), this.TitleBackColor, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, Color.Empty, 0, ChartDashStyle.NotSet, Color.Empty, 0, PenAlignment.Inset);
					chartGraph.StopAnimation();
				}
				using (SolidBrush brush = new SolidBrush(this.TitleColor))
				{
					StringFormat stringFormat = new StringFormat();
					stringFormat.Alignment = this.TitleAlignment;
					Rectangle r2 = this.titlePosition;
					r2.Y += this.offset.Height;
					r2.X += this.offset.Width;
					r2.X += this.GetBorderSize();
					r2.Width -= this.GetBorderSize() * 2 + this.offset.Width;
					chartGraph.StartAnimation();
					r2.Intersect(rect);
					chartGraph.DrawStringRel(this.Title.Replace("\\n", "\n"), this.TitleFont, brush, chartGraph.GetRelativeRectangle(r2), stringFormat);
					chartGraph.StopAnimation();
				}
				Rectangle rectangle = this.titlePosition;
				if (this.TitleSeparator == LegendSeparatorType.Line || this.TitleSeparator == LegendSeparatorType.DoubleLine)
				{
					rect.Width--;
				}
				rectangle.Intersect(rect);
				this.DrawSeparator(chartGraph, this.TitleSeparator, this.TitleSeparatorColor, true, rectangle);
				if (this.TitleBackColor.IsEmpty && this.TitleSeparator == LegendSeparatorType.None)
				{
					return;
				}
				chartGraph.StartAnimation();
				chartGraph.FillRectangleRel(chartGraph.GetRelativeRectangle(Rectangle.Round(chartGraph.GetAbsoluteRectangle(this.Position.ToRectangleF()))), Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, this.BorderColor, this.GetBorderSize(), this.BorderStyle, Color.Empty, 0, PenAlignment.Inset);
				chartGraph.StopAnimation();
			}
		}

		internal Size GetSeparatorSize(ChartGraphics chartGraph, LegendSeparatorType separatorType)
		{
			Size result = Size.Empty;
			switch (separatorType)
			{
			case LegendSeparatorType.None:
				result = Size.Empty;
				break;
			case LegendSeparatorType.Line:
				result = new Size(1, 1);
				break;
			case LegendSeparatorType.DashLine:
				result = new Size(1, 1);
				break;
			case LegendSeparatorType.DotLine:
				result = new Size(1, 1);
				break;
			case LegendSeparatorType.ThickLine:
				result = new Size(2, 2);
				break;
			case LegendSeparatorType.DoubleLine:
				result = new Size(3, 3);
				break;
			case LegendSeparatorType.GradientLine:
				result = new Size(1, 1);
				break;
			case LegendSeparatorType.ThickGradientLine:
				result = new Size(2, 2);
				break;
			default:
				throw new InvalidOperationException(SR.ExceptionLegendSeparatorTypeUnknown(separatorType.ToString()));
			}
			result.Width += this.itemColumnSpacingRel;
			return result;
		}

		private void DrawSeparator(ChartGraphics chartGraph, LegendSeparatorType separatorType, Color color, bool horizontal, Rectangle position)
		{
			SmoothingMode smoothingMode = chartGraph.SmoothingMode;
			chartGraph.SmoothingMode = SmoothingMode.None;
			RectangleF rectangleF = position;
			if (!horizontal)
			{
				rectangleF.X += (float)(int)((float)this.itemColumnSpacingRel / 2.0);
				rectangleF.Width -= (float)this.itemColumnSpacingRel;
			}
			chartGraph.StartAnimation();
			switch (separatorType)
			{
			case LegendSeparatorType.Line:
				if (horizontal)
				{
					chartGraph.DrawLineAbs(color, 1, ChartDashStyle.Solid, new PointF(rectangleF.Left, (float)(rectangleF.Bottom - 1.0)), new PointF(rectangleF.Right, (float)(rectangleF.Bottom - 1.0)));
				}
				else
				{
					chartGraph.DrawLineAbs(color, 1, ChartDashStyle.Solid, new PointF((float)(rectangleF.Right - 1.0), rectangleF.Top), new PointF((float)(rectangleF.Right - 1.0), rectangleF.Bottom));
				}
				break;
			case LegendSeparatorType.DashLine:
				if (horizontal)
				{
					chartGraph.DrawLineAbs(color, 1, ChartDashStyle.Dash, new PointF(rectangleF.Left, (float)(rectangleF.Bottom - 1.0)), new PointF(rectangleF.Right, (float)(rectangleF.Bottom - 1.0)));
				}
				else
				{
					chartGraph.DrawLineAbs(color, 1, ChartDashStyle.Dash, new PointF((float)(rectangleF.Right - 1.0), rectangleF.Top), new PointF((float)(rectangleF.Right - 1.0), rectangleF.Bottom));
				}
				break;
			case LegendSeparatorType.DotLine:
				if (horizontal)
				{
					chartGraph.DrawLineAbs(color, 1, ChartDashStyle.Dot, new PointF(rectangleF.Left, (float)(rectangleF.Bottom - 1.0)), new PointF(rectangleF.Right, (float)(rectangleF.Bottom - 1.0)));
				}
				else
				{
					chartGraph.DrawLineAbs(color, 1, ChartDashStyle.Dot, new PointF((float)(rectangleF.Right - 1.0), rectangleF.Top), new PointF((float)(rectangleF.Right - 1.0), rectangleF.Bottom));
				}
				break;
			case LegendSeparatorType.ThickLine:
				if (horizontal)
				{
					chartGraph.DrawLineAbs(color, 2, ChartDashStyle.Solid, new PointF(rectangleF.Left, (float)(rectangleF.Bottom - 1.0)), new PointF(rectangleF.Right, (float)(rectangleF.Bottom - 1.0)));
				}
				else
				{
					chartGraph.DrawLineAbs(color, 2, ChartDashStyle.Solid, new PointF((float)(rectangleF.Right - 1.0), rectangleF.Top), new PointF((float)(rectangleF.Right - 1.0), rectangleF.Bottom));
				}
				break;
			case LegendSeparatorType.DoubleLine:
				if (horizontal)
				{
					chartGraph.DrawLineAbs(color, 1, ChartDashStyle.Solid, new PointF(rectangleF.Left, (float)(rectangleF.Bottom - 3.0)), new PointF(rectangleF.Right, (float)(rectangleF.Bottom - 3.0)));
					chartGraph.DrawLineAbs(color, 1, ChartDashStyle.Solid, new PointF(rectangleF.Left, (float)(rectangleF.Bottom - 1.0)), new PointF(rectangleF.Right, (float)(rectangleF.Bottom - 1.0)));
				}
				else
				{
					chartGraph.DrawLineAbs(color, 1, ChartDashStyle.Solid, new PointF((float)(rectangleF.Right - 3.0), rectangleF.Top), new PointF((float)(rectangleF.Right - 3.0), rectangleF.Bottom));
					chartGraph.DrawLineAbs(color, 1, ChartDashStyle.Solid, new PointF((float)(rectangleF.Right - 1.0), rectangleF.Top), new PointF((float)(rectangleF.Right - 1.0), rectangleF.Bottom));
				}
				break;
			case LegendSeparatorType.GradientLine:
				if (horizontal)
				{
					chartGraph.FillRectangleAbs(new RectangleF(rectangleF.Left, (float)(rectangleF.Bottom - 1.0), rectangleF.Width, 0f), Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.VerticalCenter, color, Color.Empty, 0, ChartDashStyle.NotSet, PenAlignment.Inset);
				}
				else
				{
					chartGraph.FillRectangleAbs(new RectangleF((float)(rectangleF.Right - 1.0), rectangleF.Top, 0f, rectangleF.Height), Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.HorizontalCenter, color, Color.Empty, 0, ChartDashStyle.NotSet, PenAlignment.Inset);
				}
				break;
			case LegendSeparatorType.ThickGradientLine:
				if (horizontal)
				{
					chartGraph.FillRectangleAbs(new RectangleF(rectangleF.Left, (float)(rectangleF.Bottom - 2.0), rectangleF.Width, 1f), Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.VerticalCenter, color, Color.Empty, 0, ChartDashStyle.NotSet, PenAlignment.Inset);
				}
				else
				{
					chartGraph.FillRectangleAbs(new RectangleF((float)(rectangleF.Right - 2.0), rectangleF.Top, 1f, rectangleF.Height), Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.HorizontalCenter, color, Color.Empty, 0, ChartDashStyle.NotSet, PenAlignment.Inset);
				}
				break;
			}
			chartGraph.StopAnimation();
			chartGraph.SmoothingMode = smoothingMode;
		}

		private int GetBorderSize()
		{
			if (this.BorderWidth > 0 && this.BorderStyle != 0 && !this.BorderColor.IsEmpty && this.BorderColor != Color.Transparent)
			{
				return this.BorderWidth;
			}
			return 0;
		}

		private LegendTableStyle GetLegendTableStyle(ChartGraphics chartGraph)
		{
			LegendTableStyle tableStyle = this.TableStyle;
			if (this.TableStyle == LegendTableStyle.Auto)
			{
				if (this.Position.Auto)
				{
					if (this.Docking != LegendDocking.Left && this.Docking != LegendDocking.Right)
					{
						return LegendTableStyle.Wide;
					}
					return LegendTableStyle.Tall;
				}
				SizeF size = chartGraph.GetAbsoluteRectangle(this.Position.ToRectangleF()).Size;
				if (size.Width < size.Height)
				{
					return LegendTableStyle.Tall;
				}
				return LegendTableStyle.Wide;
			}
			return tableStyle;
		}

		internal bool IsEnabled()
		{
			if (this.Enabled)
			{
				if (this.DockToChartArea.Length > 0 && base.Common != null && base.Common.ChartPicture != null && base.Common.ChartPicture.ChartAreas.GetIndex(this.DockToChartArea) >= 0)
				{
					ChartArea chartArea = base.Common.ChartPicture.ChartAreas[this.DockToChartArea];
					if (!chartArea.Visible)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		internal void Invalidate(bool invalidateLegendOnly)
		{
		}

		internal void SelectLegendBackground(ChartGraphics chartGraph)
		{
			base.Common.HotRegionsList.AddHotRegion(this.Position.ToRectangleF(), this, ChartElementType.LegendArea, true);
		}
	}
}
