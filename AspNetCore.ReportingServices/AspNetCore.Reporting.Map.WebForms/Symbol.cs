//
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(SymbolConverter))]
	internal class Symbol : NamedElement, IContentElement, ILayerElement, IToolTipProvider, ISelectable, ISpatialElement, IImageMapProvider
	{
		private GraphicsPath[] cachedPaths;

		private RectangleF[] cachedPathBounds;

		internal Hashtable fields;

		private string fieldDataBuffer = string.Empty;

		internal PointF precalculatedCenterPoint = PointF.Empty;

		private XamlRenderer[] xamlRenderers;

		private SymbolData symbolData;

		private Offset offset;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private TextAlignment textAlignment = TextAlignment.Bottom;

		private bool visible = true;

		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private Color borderColor = Color.DarkGray;

		private MapDashStyle borderStyle = MapDashStyle.Solid;

		private int borderWidth = 1;

		private Color color = Color.Red;

		private Color textColor = Color.Black;

		private GradientType gradientType;

		private Color secondaryColor = Color.Empty;

		private MapHatchStyle hatchStyle;

		private string text = "";

		private int shadowOffset;

		private int textShadowOffset;

		private bool selected;

		private MarkerStyle markerStyle = MarkerStyle.Circle;

		private float width = 7f;

		private float height = 7f;

		private ResizeMode imageResizeMode = ResizeMode.AutoFit;

		private string image = "";

		private Color imageTransColor = Color.Empty;

		private string category = string.Empty;

		private string parentShape = "(none)";

		private Shape parentShapeObject;

		private bool useInternalProperties;

		private Color borderColorInt = Color.DarkGray;

		private MapDashStyle borderStyleInt = MapDashStyle.Solid;

		private int borderWidthInt = 1;

		private Color colorInt = Color.Red;

		private Color secondaryColorInt = Color.Empty;

		private GradientType gradientTypeInt;

		private MapHatchStyle hatchStyleInt;

		private string textInt = "";

		private TextAlignment textAlignmentInt = TextAlignment.Bottom;

		private string toolTipInt = "";

		private Font fontInt = new Font("Microsoft Sans Serif", 8.25f);

		private Color textColorInt = Color.Black;

		private int textShadowOffsetInt;

		private MarkerStyle markerStyleInt = MarkerStyle.Circle;

		private float widthInt = 7f;

		private float heightInt = 7f;

		private int shadowOffsetInt;

		private string imageInt = "";

		private Color imageTransColorInt = Color.Empty;

		private ResizeMode imageResizeModeInt = ResizeMode.AutoFit;

		private bool visibleInt = true;

		private object mapAreaTag;

		private string layer = "(none)";

		private bool belongsToLayer;

		private bool belongsToAllLayers;

		private Layer layerObject;

		MapPoint ISpatialElement.MinimumExtent
		{
			get
			{
				return this.SymbolData.MinimumExtent;
			}
		}

		MapPoint ISpatialElement.MaximumExtent
		{
			get
			{
				return this.SymbolData.MaximumExtent;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeSymbol_SymbolData")]
		public SymbolData SymbolData
		{
			get
			{
				return this.symbolData;
			}
			set
			{
				this.symbolData = value;
				this.ResetCachedPaths();
				this.InvalidateCachedBounds();
				this.InvalidateViewport();
			}
		}

		MapPoint[] ISpatialElement.Points
		{
			get
			{
				return this.SymbolData.Points;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string EncodedSymbolData
		{
			get
			{
				return SymbolData.SymbolDataToString(this.SymbolData);
			}
			set
			{
				this.symbolData = SymbolData.SymbolDataFromString(value);
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue("")]
		public string FieldData
		{
			get
			{
				return this.FieldDataToString();
			}
			set
			{
				this.fieldDataBuffer = value;
				this.InvalidateViewport();
			}
		}

		[SRDescription("DescriptionAttributeSymbol_Offset")]
		[SRCategory("CategoryAttribute_Behavior")]
		[TypeConverter(typeof(ShapeOffsetConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Offset Offset
		{
			get
			{
				return this.offset;
			}
			set
			{
				this.offset = value;
				this.offset.Parent = this;
				this.ResetCachedPaths();
				this.InvalidateCachedBounds();
				this.InvalidateViewport();
			}
		}

		[DefaultValue("")]
		[SRDescription("DescriptionAttributeSymbol_ToolTip")]
		[TypeConverter(typeof(KeywordConverter))]
		[Localizable(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttribute_Behavior")]
		public string ToolTip
		{
			get
			{
				return this.toolTip;
			}
			set
			{
				this.toolTip = value;
				this.InvalidateViewport();
			}
		}

		[DefaultValue("")]
		[Localizable(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeSymbol_Href")]
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

		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeSymbol_MapAreaAttributes")]
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

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSymbol_Name")]
		public sealed override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
				this.InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue(TextAlignment.Bottom)]
		[SRDescription("DescriptionAttributeSymbol_TextAlignment")]
		public TextAlignment TextAlignment
		{
			get
			{
				return this.textAlignment;
			}
			set
			{
				this.textAlignment = value;
				this.ResetCachedPaths();
				this.InvalidateViewport();
			}
		}

		[SRDescription("DescriptionAttributeSymbol_Visible")]
		[SRCategory("CategoryAttribute_Appearance")]
		[ParenthesizePropertyName(true)]
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
				this.InvalidateViewport();
			}
		}

		[SRDescription("DescriptionAttributeSymbol_Font")]
		[SRCategory("CategoryAttribute_Behavior")]
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
				this.ResetCachedPaths();
				this.InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeSymbol_BorderColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "DarkGray")]
		public Color BorderColor
		{
			get
			{
				return this.borderColor;
			}
			set
			{
				this.borderColor = value;
				this.InvalidateViewport();
			}
		}

		[DefaultValue(MapDashStyle.Solid)]
		[SRDescription("DescriptionAttributeSymbol_BorderStyle")]
		[SRCategory("CategoryAttribute_Appearance")]
		public MapDashStyle BorderStyle
		{
			get
			{
				return this.borderStyle;
			}
			set
			{
				this.borderStyle = value;
				this.InvalidateViewport();
			}
		}

		[SRDescription("DescriptionAttributeSymbol_BorderWidth")]
		[DefaultValue(1)]
		[SRCategory("CategoryAttribute_Appearance")]
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
					this.InvalidateViewport();
					return;
				}
				throw new ArgumentException(SR.must_in_range(0.0, 100.0));
			}
		}

		[DefaultValue(typeof(Color), "Red")]
		[SRDescription("DescriptionAttributeSymbol_Color")]
		[SRCategory("CategoryAttribute_Appearance")]
		public Color Color
		{
			get
			{
				return this.color;
			}
			set
			{
				if (this.color != value)
				{
					this.color = value;
					this.ResetCachedXamlRenderers();
					this.InvalidateViewport();
				}
			}
		}

		[SRDescription("DescriptionAttributeSymbol_TextColor")]
		[DefaultValue(typeof(Color), "Black")]
		[SRCategory("CategoryAttribute_Appearance")]
		public Color TextColor
		{
			get
			{
				return this.textColor;
			}
			set
			{
				this.textColor = value;
				this.InvalidateViewport();
			}
		}

		[SRDescription("DescriptionAttributeSymbol_GradientType")]
		[DefaultValue(GradientType.None)]
		//[Editor(typeof(GradientEditor), typeof(UITypeEditor))]
		[SRCategory("CategoryAttribute_Appearance")]
		public GradientType GradientType
		{
			get
			{
				return this.gradientType;
			}
			set
			{
				this.gradientType = value;
				this.InvalidateViewport();
			}
		}

		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeSymbol_SecondaryColor")]
		[SRCategory("CategoryAttribute_Appearance")]
		public Color SecondaryColor
		{
			get
			{
				return this.secondaryColor;
			}
			set
			{
				this.secondaryColor = value;
				this.InvalidateViewport();
			}
		}

		[SRDescription("DescriptionAttributeSymbol_HatchStyle")]
		//[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		[DefaultValue(MapHatchStyle.None)]
		[SRCategory("CategoryAttribute_Appearance")]
		public MapHatchStyle HatchStyle
		{
			get
			{
				return this.hatchStyle;
			}
			set
			{
				this.hatchStyle = value;
				this.InvalidateViewport();
			}
		}

		[SRDescription("DescriptionAttributeSymbol_Text")]
		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Behavior")]
		[Localizable(true)]
		[TypeConverter(typeof(KeywordConverter))]
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
				this.ResetCachedPaths();
				this.InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeSymbol_ShadowOffset")]
		public int ShadowOffset
		{
			get
			{
				return this.shadowOffset;
			}
			set
			{
				if (value >= -100 && value <= 100)
				{
					this.shadowOffset = value;
					this.InvalidateViewport();
					return;
				}
				throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
			}
		}

		[SRDescription("DescriptionAttributeSymbol_TextShadowOffset")]
		[NotifyParentProperty(true)]
		[DefaultValue(0)]
		[SRCategory("CategoryAttribute_Appearance")]
		public int TextShadowOffset
		{
			get
			{
				return this.textShadowOffset;
			}
			set
			{
				if (value >= -100 && value <= 100)
				{
					this.textShadowOffset = value;
					this.InvalidateViewport();
					return;
				}
				throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeSymbol_Selected")]
		public bool Selected
		{
			get
			{
				return this.selected;
			}
			set
			{
				this.selected = value;
				this.InvalidateViewport();
			}
		}

		[SRDescription("DescriptionAttributeSymbol_MarkerStyle")]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(MarkerStyle.Circle)]
		public MarkerStyle MarkerStyle
		{
			get
			{
				return this.markerStyle;
			}
			set
			{
				if (this.markerStyle != value)
				{
					this.markerStyle = value;
					this.ResetCachedPaths();
					this.InvalidateViewport();
				}
			}
		}

		[SRDescription("DescriptionAttributeSymbol_Width")]
		[DefaultValue(7f)]
		[SRCategory("CategoryAttribute_Size")]
		public float Width
		{
			get
			{
				return this.width;
			}
			set
			{
				if (!(value < 0.0) && !(value > 1000.0))
				{
					this.width = value;
					this.ResetCachedPaths();
					this.InvalidateViewport();
					return;
				}
				throw new ArgumentException(SR.must_in_range(0.0, 1000.0));
			}
		}

		[DefaultValue(7f)]
		[SRCategory("CategoryAttribute_Size")]
		[SRDescription("DescriptionAttributeSymbol_Height")]
		public float Height
		{
			get
			{
				return this.height;
			}
			set
			{
				if (!(value < 0.0) && !(value > 1000.0))
				{
					this.height = value;
					this.ResetCachedPaths();
					this.InvalidateViewport();
					return;
				}
				throw new ArgumentException(SR.must_in_range(0.0, 1000.0));
			}
		}

		[SRDescription("DescriptionAttributeSymbol_X")]
		[SRCategory("CategoryAttribute_Coordinates")]
		[DefaultValue(typeof(MapCoordinate), "0d")]
		public MapCoordinate X
		{
			get
			{
				return new MapCoordinate(this.SymbolData.Points[0].X);
			}
			set
			{
				this.SymbolData.Points[0].X = value.ToDouble();
				this.SymbolData.UpdateStoredParameters();
				this.InvalidateCachedBounds();
				this.ResetCachedPaths();
				this.InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Coordinates")]
		[SRDescription("DescriptionAttributeSymbol_Y")]
		[DefaultValue(typeof(MapCoordinate), "0d")]
		public MapCoordinate Y
		{
			get
			{
				return new MapCoordinate(this.SymbolData.Points[0].Y);
			}
			set
			{
				this.SymbolData.Points[0].Y = value.ToDouble();
				this.SymbolData.UpdateStoredParameters();
				this.InvalidateCachedBounds();
				this.ResetCachedPaths();
				this.InvalidateViewport();
			}
		}

		[DefaultValue(ResizeMode.AutoFit)]
		[SRDescription("DescriptionAttributeSymbol_ImageResizeMode")]
		[SRCategory("CategoryAttribute_Image")]
		public ResizeMode ImageResizeMode
		{
			get
			{
				return this.imageResizeMode;
			}
			set
			{
				this.imageResizeMode = value;
				this.ResetCachedPaths();
				this.InvalidateViewport();
			}
		}

		[SRDescription("DescriptionAttributeSymbol_Image")]
		[SRCategory("CategoryAttribute_Image")]
		[DefaultValue("")]
		public string Image
		{
			get
			{
				return this.image;
			}
			set
			{
				this.image = value;
				this.InvalidateViewport();
			}
		}

		[SRDescription("DescriptionAttributeSymbol_ImageTransColor")]
		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAttribute_Image")]
		public Color ImageTransColor
		{
			get
			{
				return this.imageTransColor;
			}
			set
			{
				this.imageTransColor = value;
				this.InvalidateViewport();
			}
		}

		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSymbol_Category")]
		public string Category
		{
			get
			{
				return this.category;
			}
			set
			{
				this.category = value;
				this.InvalidateRules();
				this.InvalidateViewport();
			}
		}

		[SRDescription("DescriptionAttributeSymbol_ParentShape")]
		[TypeConverter(typeof(DesignTimeShapeConverter))]
		[DefaultValue("(none)")]
		[SRCategory("CategoryAttribute_Behavior")]
		public string ParentShape
		{
			get
			{
				return this.parentShape;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.parentShape = "(none)";
				}
				else
				{
					this.parentShape = value;
				}
				this.ParentShapeObject = null;
				this.InvalidateChildSymbols();
				this.InvalidateCachedBounds();
				this.InvalidateDistanceScalePanel();
				this.InvalidateViewport();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object this[string name]
		{
			get
			{
				return this.fields[name];
			}
			set
			{
				if (value != null && !(value is DBNull))
				{
					MapCore mapCore = this.GetMapCore();
					if (mapCore != null)
					{
						Field field = (Field)mapCore.SymbolFields.GetByName(name);
						if (field == null)
						{
							throw new ArgumentException(SR.ExceptionFieldNameDoesNotExist(name));
						}
						if (!field.Type.IsInstanceOfType(value) && field.Type != Field.ConvertToSupportedType(value.GetType()))
						{
							throw new ArgumentException(SR.ExceptionFieldMustBeOfType(name, field.Type.ToString()));
						}
						field.SetValue(Field.ConvertToSupportedValue(value), this.fields);
						mapCore.InvalidateRules();
						mapCore.InvalidateDataBinding();
					}
					else
					{
						this.fields[name] = Field.ConvertToSupportedValue(value);
					}
					this.InvalidateViewport();
				}
			}
		}

		internal Shape ParentShapeObject
		{
			get
			{
				if (this.parentShape == "(none)")
				{
					return null;
				}
				if (this.parentShapeObject == null)
				{
					MapCore mapCore = this.GetMapCore();
					if (mapCore != null)
					{
						this.parentShapeObject = (Shape)mapCore.Shapes.GetByName(this.parentShape);
					}
				}
				return this.parentShapeObject;
			}
			set
			{
				this.parentShapeObject = value;
			}
		}

		internal bool UseInternalProperties
		{
			get
			{
				return this.useInternalProperties;
			}
			set
			{
				this.useInternalProperties = value;
				this.InvalidateViewport();
			}
		}

		internal Color BorderColorInt
		{
			get
			{
				if (this.borderColor == Color.DarkGray && this.useInternalProperties)
				{
					return this.borderColorInt;
				}
				return this.borderColor;
			}
			set
			{
				this.borderColorInt = value;
				this.InvalidateViewport();
			}
		}

		internal MapDashStyle BorderStyleInt
		{
			get
			{
				if (this.borderStyle == MapDashStyle.Solid && this.useInternalProperties)
				{
					return this.borderStyleInt;
				}
				return this.borderStyle;
			}
			set
			{
				this.borderStyleInt = value;
				this.InvalidateViewport();
			}
		}

		internal int BorderWidthInt
		{
			get
			{
				if (this.borderWidth == 1 && this.useInternalProperties)
				{
					return this.borderWidthInt;
				}
				return this.borderWidth;
			}
			set
			{
				this.borderWidthInt = value;
				this.InvalidateViewport();
			}
		}

		internal Color ColorInt
		{
			get
			{
				if (this.color == Color.Red && this.useInternalProperties)
				{
					return this.colorInt;
				}
				return this.color;
			}
			set
			{
				if (this.colorInt != value)
				{
					this.colorInt = value;
					this.ResetCachedXamlRenderers();
					this.InvalidateViewport();
				}
			}
		}

		internal Color SecondaryColorInt
		{
			get
			{
				if (this.secondaryColor == Color.Empty && this.useInternalProperties)
				{
					return this.secondaryColorInt;
				}
				return this.secondaryColor;
			}
			set
			{
				this.secondaryColorInt = value;
				this.InvalidateViewport();
			}
		}

		internal GradientType GradientTypeInt
		{
			get
			{
				if (this.gradientType == GradientType.None && this.useInternalProperties)
				{
					return this.gradientTypeInt;
				}
				return this.gradientType;
			}
			set
			{
				this.gradientTypeInt = value;
				this.InvalidateViewport();
			}
		}

		internal MapHatchStyle HatchStyleInt
		{
			get
			{
				if (this.hatchStyle == MapHatchStyle.None && this.useInternalProperties)
				{
					return this.hatchStyleInt;
				}
				return this.hatchStyle;
			}
			set
			{
				this.hatchStyleInt = value;
				this.InvalidateViewport();
			}
		}

		internal string TextInt
		{
			get
			{
				if (string.IsNullOrEmpty(this.text) && this.useInternalProperties)
				{
					return this.textInt;
				}
				return this.text;
			}
			set
			{
				this.textInt = value;
				this.ResetCachedPaths();
				this.InvalidateViewport();
			}
		}

		internal TextAlignment TextAlignmentInt
		{
			get
			{
				if (this.textAlignment == TextAlignment.Bottom && this.useInternalProperties)
				{
					return this.textAlignmentInt;
				}
				return this.textAlignment;
			}
			set
			{
				this.textAlignmentInt = value;
				this.ResetCachedPaths();
				this.InvalidateViewport();
			}
		}

		internal string ToolTipInt
		{
			get
			{
				if (string.IsNullOrEmpty(this.toolTip) && this.useInternalProperties)
				{
					return this.toolTipInt;
				}
				return this.toolTip;
			}
			set
			{
				this.toolTipInt = value;
				this.InvalidateViewport();
			}
		}

		internal Font FontInt
		{
			get
			{
				if (this.font == new Font("Microsoft Sans Serif", 8.25f) && this.useInternalProperties)
				{
					return this.fontInt;
				}
				return this.font;
			}
			set
			{
				this.fontInt = value;
				this.ResetCachedPaths();
				this.InvalidateViewport();
			}
		}

		internal Color TextColorInt
		{
			get
			{
				if (this.textColor == Color.Black && this.useInternalProperties)
				{
					return this.textColorInt;
				}
				return this.textColor;
			}
			set
			{
				this.textColorInt = value;
				this.InvalidateViewport();
			}
		}

		internal int TextShadowOffsetInt
		{
			get
			{
				if (this.textShadowOffset == 0 && this.useInternalProperties)
				{
					return this.textShadowOffsetInt;
				}
				return this.textShadowOffset;
			}
			set
			{
				this.textShadowOffsetInt = value;
				this.InvalidateViewport();
			}
		}

		internal MarkerStyle MarkerStyleInt
		{
			get
			{
				if (this.markerStyle == MarkerStyle.Circle && this.useInternalProperties)
				{
					return this.markerStyleInt;
				}
				return this.markerStyle;
			}
			set
			{
				if (this.markerStyleInt != value)
				{
					this.markerStyleInt = value;
					this.ResetCachedPaths();
					this.InvalidateViewport();
				}
			}
		}

		internal float WidthInt
		{
			get
			{
				if (this.width == 7.0 && this.useInternalProperties)
				{
					return this.widthInt;
				}
				return this.width;
			}
			set
			{
				this.widthInt = value;
				this.ResetCachedPaths();
				this.InvalidateViewport();
			}
		}

		internal float HeightInt
		{
			get
			{
				if (this.height == 7.0 && this.useInternalProperties)
				{
					return this.heightInt;
				}
				return this.height;
			}
			set
			{
				this.heightInt = value;
				this.ResetCachedPaths();
				this.InvalidateViewport();
			}
		}

		internal int ShadowOffsetInt
		{
			get
			{
				if (this.shadowOffset == 0 && this.useInternalProperties)
				{
					return this.shadowOffsetInt;
				}
				return this.shadowOffset;
			}
			set
			{
				this.shadowOffsetInt = value;
				this.InvalidateViewport();
			}
		}

		internal string ImageInt
		{
			get
			{
				if (string.IsNullOrEmpty(this.image) && this.useInternalProperties)
				{
					return this.imageInt;
				}
				return this.image;
			}
			set
			{
				this.imageInt = value;
				this.InvalidateViewport();
			}
		}

		internal Color ImageTransColorInt
		{
			get
			{
				if (this.imageTransColor == Color.Empty && this.useInternalProperties)
				{
					return this.imageTransColorInt;
				}
				return this.imageTransColor;
			}
			set
			{
				this.imageTransColorInt = value;
				this.InvalidateViewport();
			}
		}

		internal ResizeMode ImageResizeModeInt
		{
			get
			{
				if (this.imageResizeMode == ResizeMode.AutoFit && this.useInternalProperties)
				{
					return this.imageResizeModeInt;
				}
				return this.imageResizeMode;
			}
			set
			{
				this.imageResizeModeInt = value;
				this.ResetCachedPaths();
				this.InvalidateViewport();
			}
		}

		internal bool VisibleInt
		{
			get
			{
				if (this.visible && this.useInternalProperties)
				{
					return this.visibleInt;
				}
				return this.visible;
			}
			set
			{
				this.visibleInt = value;
				this.InvalidateViewport();
			}
		}

		object IImageMapProvider.Tag
		{
			get
			{
				return this.mapAreaTag;
			}
			set
			{
				this.mapAreaTag = value;
			}
		}

		[TypeConverter(typeof(DesignTimeLayerConverter))]
		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue("(none)")]
		[SRDescription("DescriptionAttributeSymbol_Layer")]
		public string Layer
		{
			get
			{
				return this.layer;
			}
			set
			{
				if (this.layer != value)
				{
					if (string.IsNullOrEmpty(value) || value == "(none)")
					{
						this.layer = "(none)";
						this.belongsToLayer = false;
						this.belongsToAllLayers = false;
					}
					else if (value == "(all)")
					{
						this.layer = value;
						this.belongsToLayer = true;
						this.belongsToAllLayers = true;
					}
					else
					{
						this.layer = value;
						this.belongsToLayer = true;
						this.belongsToAllLayers = false;
					}
					this.InvalidateViewport();
				}
			}
		}

		bool ILayerElement.BelongsToLayer
		{
			get
			{
				return this.belongsToLayer;
			}
		}

		bool ILayerElement.BelongsToAllLayers
		{
			get
			{
				return this.belongsToAllLayers;
			}
		}

		Layer ILayerElement.LayerObject
		{
			get
			{
				if (this.layerObject != null)
				{
					return this.layerObject;
				}
				MapCore mapCore = this.GetMapCore();
				if (this.belongsToLayer && !this.belongsToAllLayers && this.layerObject == null && mapCore != null)
				{
					this.layerObject = mapCore.Layers[this.Layer];
				}
				return this.layerObject;
			}
			set
			{
				this.layerObject = value;
				if (value != null)
				{
					this.layer = value.Name;
					this.belongsToAllLayers = false;
					this.belongsToLayer = true;
				}
				else
				{
					this.layer = "(none)";
					this.belongsToAllLayers = false;
					this.belongsToLayer = false;
				}
				this.InvalidateViewport();
			}
		}

		public Symbol()
			: this(null)
		{
		}

		internal Symbol(CommonElements common)
			: base(common)
		{
			this.symbolData = new SymbolData();
			this.fields = new Hashtable();
			this.offset = new Offset(this, 0.0, 0.0);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected bool ShouldSerializeOffset()
		{
			if (this.Offset.X == 0.0)
			{
				return this.Offset.Y != 0.0;
			}
			return true;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected void ResetOffset()
		{
			this.Offset.X = 0.0;
			this.Offset.Y = 0.0;
		}

		public override string ToString()
		{
			return this.Name;
		}

		public void AddPoints(MapPoint[] points)
		{
			if (points == null)
			{
				throw new ArgumentException(SR.ExceptionArrayOfPointsCannotBeNull);
			}
			MapPoint[] array = new MapPoint[this.SymbolData.Points.Length + points.Length];
			Array.Copy(this.SymbolData.Points, array, this.SymbolData.Points.Length);
			Array.Copy(points, 0, array, this.SymbolData.Points.Length, points.Length);
			this.SymbolData.Points = array;
			this.SymbolData.UpdateStoredParameters();
			this.InvalidateChildSymbols();
			this.InvalidateCachedBounds();
			this.InvalidateRules();
			this.InvalidateViewport();
		}

		public void ClearSymbolData()
		{
			this.X = 0.0;
			this.Y = 0.0;
			this.SymbolData.Points = new MapPoint[1]
			{
				new MapPoint(this.X, this.Y)
			};
			this.SymbolData.UpdateStoredParameters();
			this.InvalidateChildSymbols();
			this.InvalidateCachedBounds();
			this.InvalidateRules();
			this.InvalidateViewport();
		}

		public void SetPoints(MapPoint[] points)
		{
			if (points == null)
			{
				throw new ArgumentException(SR.ExceptionArrayOfPointsCannotBeNull);
			}
			if (points.Length < 1)
			{
				throw new ArgumentException(SR.ExceptionArrayOfPointsMustContainOnePoint);
			}
			this.SymbolData.Points = points;
			this.SymbolData.UpdateStoredParameters();
			this.InvalidateCachedBounds();
			this.InvalidateRules();
			this.InvalidateViewport();
		}

		public void SetCoordinates(double longitude, double latitude)
		{
			this.X = longitude;
			this.Y = latitude;
		}

		public void SetCoordinates(string longitude, string latitude)
		{
			this.X = longitude;
			this.Y = latitude;
		}

		public void SetCoordinates(string latitudeAndLongitude)
		{
			Regex regex = new Regex("[-+]?[0-9]*\\.?[0-9]+");
			MatchCollection matchCollection = regex.Matches(latitudeAndLongitude);
			if (matchCollection.Count >= 2 && matchCollection.Count != 3 && matchCollection.Count != 5 && matchCollection.Count <= 6)
			{
				string latitude = latitudeAndLongitude.Substring(0, matchCollection[matchCollection.Count / 2].Index);
				string longitude = latitudeAndLongitude.Substring(matchCollection[matchCollection.Count / 2].Index);
				this.SetCoordinates(longitude, latitude);
				return;
			}
			throw new ArgumentException(SR.ExceptionInvalidCoordonateString(latitudeAndLongitude));
		}

		internal PointF GetCenterPointInContentPixels(MapGraphics g, int pointIndex, out bool visible)
		{
			visible = true;
			if (this.ParentShape != "(none)")
			{
				return new PointF(this.precalculatedCenterPoint.X + (float)this.Offset.X, this.precalculatedCenterPoint.Y - (float)this.Offset.Y);
			}
			MapCore mapCore = this.GetMapCore();
			if (mapCore != null)
			{
				Point3D point3D = this.GetMapCore().GeographicToPercents(this.SymbolData.Points[pointIndex].X + this.Offset.X, this.SymbolData.Points[pointIndex].Y + this.Offset.Y);
				visible = (point3D.Z >= 0.0);
				return g.GetAbsolutePoint(point3D.ToPointF());
			}
			return PointF.Empty;
		}

		public PointF GetCenterPointInContentPixels(MapGraphics g, int pointIndex)
		{
			bool flag = default(bool);
			return this.GetCenterPointInContentPixels(g, pointIndex, out flag);
		}

		internal MapCore GetMapCore()
		{
			return (MapCore)this.ParentElement;
		}

		private string FieldDataToString()
		{
			string text = string.Empty;
			MapCore mapCore = this.GetMapCore();
			if (mapCore != null)
			{
				foreach (Field symbolField in mapCore.SymbolFields)
				{
					if (!symbolField.IsTemporary)
					{
						string text2 = symbolField.FormatValue(this.fields[symbolField.Name]);
						if (!string.IsNullOrEmpty(text2))
						{
							string text3 = text;
							text = text3 + XmlConvert.EncodeName(symbolField.Name) + "=" + text2 + "&";
						}
					}
				}
				text = text.TrimEnd('&');
			}
			return text;
		}

		internal void FieldDataFromBuffer()
		{
			if (this.fieldDataBuffer.Length != 0)
			{
				MapCore mapCore = this.GetMapCore();
				if (mapCore != null)
				{
					this.fields.Clear();
					string[] array = this.fieldDataBuffer.Split('&');
					string[] array2 = array;
					foreach (string text in array2)
					{
						string[] array3 = text.Split('=');
						string name = XmlConvert.DecodeName(array3[0]);
						string fieldValue = XmlConvert.DecodeName(array3[1]);
						Field field = (Field)mapCore.SymbolFields.GetByName(name);
						if (field != null)
						{
							field.ParseValue(fieldValue, this.fields);
						}
					}
					this.fieldDataBuffer = "";
				}
			}
		}

		internal override void OnAdded()
		{
			base.OnAdded();
		}

		internal override void OnRemove()
		{
			this.InvalidateChildSymbols();
			this.InvalidateViewport();
			base.OnRemove();
		}

		protected override void OnDispose()
		{
			this.ResetCachedPaths();
			this.SymbolData.Points = null;
			if (this.fields != null)
			{
				this.fields.Clear();
			}
			base.OnDispose();
		}

		internal void ApplyPredefinedSymbolAttributes(PredefinedSymbol predefinedSymbol, AffectedSymbolAttributes affectedAttributes)
		{
			this.UseInternalProperties = true;
			switch (affectedAttributes)
			{
			case AffectedSymbolAttributes.ColorOnly:
				this.ColorInt = predefinedSymbol.Color;
				break;
			case AffectedSymbolAttributes.MarkerOnly:
				this.MarkerStyleInt = predefinedSymbol.MarkerStyle;
				this.ImageInt = predefinedSymbol.Image;
				break;
			case AffectedSymbolAttributes.SizeOnly:
				this.WidthInt = predefinedSymbol.Width;
				this.HeightInt = predefinedSymbol.Height;
				break;
			default:
				this.BorderColorInt = predefinedSymbol.BorderColor;
				this.BorderStyleInt = predefinedSymbol.BorderStyle;
				this.BorderWidthInt = predefinedSymbol.BorderWidth;
				this.ColorInt = predefinedSymbol.Color;
				this.SecondaryColorInt = predefinedSymbol.SecondaryColor;
				this.GradientTypeInt = predefinedSymbol.GradientType;
				this.HatchStyleInt = predefinedSymbol.HatchStyle;
				this.TextInt = predefinedSymbol.Text;
				this.TextAlignmentInt = predefinedSymbol.TextAlignment;
				this.ToolTipInt = predefinedSymbol.ToolTip;
				this.FontInt = predefinedSymbol.Font;
				this.TextColorInt = predefinedSymbol.TextColor;
				this.TextShadowOffsetInt = predefinedSymbol.TextShadowOffset;
				this.MarkerStyleInt = predefinedSymbol.MarkerStyle;
				this.WidthInt = predefinedSymbol.Width;
				this.HeightInt = predefinedSymbol.Height;
				this.ShadowOffsetInt = predefinedSymbol.ShadowOffset;
				this.ImageInt = predefinedSymbol.Image;
				this.ImageTransColorInt = predefinedSymbol.ImageTransColor;
				this.ImageResizeModeInt = predefinedSymbol.ImageResizeMode;
				this.VisibleInt = predefinedSymbol.Visible;
				break;
			}
		}

		internal void InvalidateChildSymbols()
		{
			MapCore mapCore = this.GetMapCore();
			if (mapCore != null)
			{
				mapCore.InvalidateChildSymbols();
			}
		}

		internal float GetWidth()
		{
			return this.Width;
		}

		internal void InvalidateRules()
		{
			MapCore mapCore = this.GetMapCore();
			if (mapCore != null)
			{
				mapCore.InvalidateRules();
				mapCore.Invalidate();
			}
		}

		internal static XamlRenderer CreateXamlRenderer(MarkerStyle markerStyle, Color color, RectangleF rect)
		{
			XamlRenderer xamlRenderer = new XamlRenderer(markerStyle.ToString() + ".xaml");
			xamlRenderer.AllowPathGradientTransform = false;
			xamlRenderer.ParseXaml(rect, new Color[1]
			{
				color
			});
			return xamlRenderer;
		}

		internal XamlRenderer[] GetXamlRenderers(MapGraphics g)
		{
			if (this.xamlRenderers != null)
			{
				return this.xamlRenderers;
			}
			GraphicsPath[] paths = this.GetPaths(g);
			XamlRenderer[] array = new XamlRenderer[paths.Length - 1];
			for (int i = 0; i < paths.Length - 1; i++)
			{
				if (paths[i] != null)
				{
					array[i] = Symbol.CreateXamlRenderer(this.MarkerStyleInt, this.ColorInt, this.cachedPathBounds[i]);
				}
			}
			this.xamlRenderers = array;
			return this.xamlRenderers;
		}

		internal void ResetCachedXamlRenderers()
		{
			if (this.xamlRenderers != null)
			{
				XamlRenderer[] array = this.xamlRenderers;
				foreach (XamlRenderer xamlRenderer in array)
				{
					if (xamlRenderer != null)
					{
						xamlRenderer.Dispose();
					}
				}
				this.xamlRenderers = null;
			}
		}

		internal static bool IsXamlMarker(MarkerStyle markerStyle)
		{
			if (markerStyle == MarkerStyle.PushPin)
			{
				return true;
			}
			return false;
		}

		internal static RectangleF CalculateXamlMarkerBounds(MarkerStyle markerStyle, PointF centerPoint, float width, float height)
		{
			RectangleF result = RectangleF.Empty;
			if (markerStyle == MarkerStyle.PushPin)
			{
				result = new RectangleF(centerPoint.X, centerPoint.Y, 0f, 0f);
				result.Inflate(width, height);
				result.Offset((float)(0.0 - width), (float)(0.0 - height));
			}
			return result;
		}
        /*
      public bool AddGeometry(SqlGeometry geometry)
      {
          geometry = geometry.MakeValid();
          if (geometry.STIsEmpty())
          {
              return false;
          }
          ArrayList arrayList = new ArrayList();
          this.AddGeometryRec(geometry, arrayList);
          if (arrayList.Count > 0)
          {
              this.SetPoints((MapPoint[])arrayList.ToArray(typeof(MapPoint)));
              return true;
          }
          return false;
      }

      private void AddGeometryRec(SqlGeometry geometry, ArrayList pointsList)
      {
          string value = geometry.STGeometryType().Value;
          if (value == "GeometryCollection")
          {
              for (int i = 1; i <= geometry.STNumGeometries().Value; i++)
              {
                  this.AddGeometryRec(geometry.STGeometryN(i), pointsList);
              }
          }
          else
          {
              if (!(value == "MultiPoint") && !(value == "Point"))
              {
                  return;
              }
              this.AddSimpleGeometry(geometry, pointsList);
          }
      }

      private void AddSimpleGeometry(SqlGeometry geometry, ArrayList pointsList)
      {
          if (!geometry.STIsEmpty())
          {
              for (int i = 1; (SqlInt32)i <= geometry.STNumPoints(); i++)
              {
                  SqlGeometry sqlGeometry = geometry.STPointN(i);
                  if (!sqlGeometry.IsNull)
                  {
                      pointsList.Add(new MapPoint(sqlGeometry.STX.Value, sqlGeometry.STY.Value));
                  }
              }
          }
      }

     public bool AddGeography(SqlGeography geography)
     {
         geography = geography.MakeValid();
         if (geography.STIsEmpty())
         {
             return false;
         }
         ArrayList arrayList = new ArrayList();
         this.AddGeographyRec(geography, arrayList);
         if (arrayList.Count > 0)
         {
             this.SetPoints((MapPoint[])arrayList.ToArray(typeof(MapPoint)));
             return true;
         }
         return false;
     }

     private void AddGeographyRec(SqlGeography geography, ArrayList pointsList)
     {
         string value = geography.STGeometryType().Value;
         if (value == "GeometryCollection")
         {
             for (int i = 1; i <= geography.STNumGeometries().Value; i++)
             {
                 this.AddGeographyRec(geography.STGeometryN(i), pointsList);
             }
         }
         else
         {
             if (!(value == "MultiPoint") && !(value == "Point"))
             {
                 return;
             }
             this.AddSimpleGeography(geography, pointsList);
         }
     }

     internal void AddSimpleGeography(SqlGeography geography, ArrayList pointsList)
     {
         if (!geography.STIsEmpty())
         {
             List<MapPoint> list = new List<MapPoint>();
             for (int i = 1; (SqlInt32)i <= geography.STNumPoints(); i++)
             {
                 SqlGeography sqlGeography = geography.STPointN(i);
                 if (!sqlGeography.IsNull)
                 {
                     list.Add(new MapPoint(sqlGeography.Long.Value, sqlGeography.Lat.Value));
                 }
             }
             if (list.Count > 0)
             {
                 MapPoint[] c = list.ToArray();
                 GeoUtils.NormalizePointsLongigute(ref c);
                 pointsList.AddRange(c);
             }
         }
     }
     */

        public bool LoadWKT(string wkt)
		{
			this.ClearSymbolData();
			return this.AddWKT(wkt);
		}

		public bool AddWKT(string wkt)
		{
            //SqlGeometry geometry = SqlGeometry.STGeomFromText(new SqlChars(new SqlString(wkt)), 4326);
            //return this.AddGeometry(geometry);
            return false;
		}

		public bool LoadWKB(byte[] wkb)
		{
			this.ClearSymbolData();
			return this.AddWKB(wkb);
		}

		public bool AddWKB(byte[] wkb)
		{
            //SqlGeometry geometry = SqlGeometry.STGeomFromWKB(new SqlBytes(wkb), 4326);
            //return this.AddGeometry(geometry);
            return false;
		}

		public string SaveWKT()
		{
			if (this.SymbolData.Points.Length == 1)
			{
				return "POINT(" + this.X.ToDouble().ToString(CultureInfo.InvariantCulture) + " " + this.Y.ToDouble().ToString(CultureInfo.InvariantCulture) + ")";
			}
			MapPoint[] points = this.SymbolData.Points;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("MULTIPOINT(");
			for (int i = 0; i < points.Length; i++)
			{
				stringBuilder.Append("(" + points[i].X.ToString(CultureInfo.InvariantCulture) + " " + points[i].Y.ToString(CultureInfo.InvariantCulture) + ")");
				if (i < points.Length - 1)
				{
					stringBuilder.Append(", ");
				}
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public byte[] SaveWKB()
		{
			MemoryStream memoryStream = new MemoryStream();
			this.SaveWKBToStream(memoryStream);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return memoryStream.ToArray();
		}

		private void SaveWKBToStream(Stream stream)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			byte value = (byte)(BitConverter.IsLittleEndian ? 1 : 0);
			if (this.SymbolData.Points.Length == 1)
			{
				binaryWriter.Write(value);
				binaryWriter.Write(1u);
				binaryWriter.Write(this.X.ToDouble());
				binaryWriter.Write(this.Y.ToDouble());
			}
			else
			{
				MapPoint[] points = this.SymbolData.Points;
				binaryWriter.Write(value);
				binaryWriter.Write(4u);
				binaryWriter.Write((uint)points.Length);
				for (int i = 0; i < points.Length; i++)
				{
					binaryWriter.Write(value);
					binaryWriter.Write(1u);
					binaryWriter.Write(points[i].X);
					binaryWriter.Write(points[i].Y);
				}
			}
		}

		internal GraphicsPath[] GetPaths(MapGraphics g)
		{
			if (!this.VisibleInt)
			{
				return null;
			}
			if (this.cachedPaths != null)
			{
				return this.cachedPaths;
			}
			GraphicsPath[] array = new GraphicsPath[this.SymbolData.Points.Length + 1];
			RectangleF[] array2 = new RectangleF[this.SymbolData.Points.Length + 1];
			for (int i = 0; i < this.SymbolData.Points.Length; i++)
			{
				bool flag = default(bool);
				PointF centerPointInContentPixels = this.GetCenterPointInContentPixels(g, i, out flag);
				if (flag)
				{
					if (this.ImageInt != string.Empty)
					{
						Image image = this.Common.ImageLoader.LoadImage(this.ImageInt);
						RectangleF rectangleF = new RectangleF(centerPointInContentPixels.X, centerPointInContentPixels.Y, 0f, 0f);
						if (this.ImageResizeModeInt == ResizeMode.AutoFit)
						{
							rectangleF.Inflate((float)(this.WidthInt / 2.0), (float)(this.HeightInt / 2.0));
						}
						else
						{
							rectangleF.Inflate((float)((float)image.Width / 2.0), (float)((float)image.Height / 2.0));
						}
						GraphicsPath graphicsPath = new GraphicsPath();
						graphicsPath.AddRectangle(rectangleF);
						array[i] = graphicsPath;
						array2[i] = rectangleF;
					}
					else if (Symbol.IsXamlMarker(this.MarkerStyleInt))
					{
						RectangleF rectangleF2 = Symbol.CalculateXamlMarkerBounds(this.MarkerStyleInt, centerPointInContentPixels, this.WidthInt, this.HeightInt);
						GraphicsPath graphicsPath2 = new GraphicsPath();
						graphicsPath2.AddRectangle(rectangleF2);
						array[i] = graphicsPath2;
						array2[i] = rectangleF2;
					}
					else
					{
						GraphicsPath graphicsPath3 = array[i] = g.CreateMarker(centerPointInContentPixels, this.WidthInt, this.HeightInt, this.MarkerStyleInt);
						array2[i] = graphicsPath3.GetBounds();
					}
				}
				else
				{
					array[i] = null;
					array2[i] = RectangleF.Empty;
				}
			}
			if (this.TextInt != string.Empty && this.IsLabelVisible() && array[0] != null)
			{
				GraphicsPath graphicsPath4 = new GraphicsPath();
				RectangleF labelRect = this.GetLabelRect(g, array2[0]);
				graphicsPath4.AddRectangle(labelRect);
				array[array.Length - 1] = graphicsPath4;
				array2[array2.Length - 1] = labelRect;
			}
			else
			{
				array[array.Length - 1] = null;
			}
			this.cachedPaths = array;
			this.cachedPathBounds = array2;
			return this.cachedPaths;
		}

		internal void InvalidateCachedBounds()
		{
			MapCore mapCore = this.GetMapCore();
			if (mapCore != null)
			{
				mapCore.InvalidateCachedBounds();
			}
		}

		internal void ResetCachedPaths()
		{
			this.ResetCachedXamlRenderers();
			if (this.cachedPaths != null)
			{
				GraphicsPath[] array = this.cachedPaths;
				foreach (GraphicsPath graphicsPath in array)
				{
					if (graphicsPath != null)
					{
						graphicsPath.Dispose();
					}
				}
			}
			this.cachedPaths = null;
			this.cachedPathBounds = null;
		}

		private bool IsLabelVisible()
		{
			MapCore mapCore = this.GetMapCore();
			if (mapCore != null && ((ILayerElement)this).BelongsToLayer)
			{
				if (!((ILayerElement)this).BelongsToAllLayers)
				{
					Layer layer = mapCore.Layers[((ILayerElement)this).Layer];
					if (!layer.LabelVisible)
					{
						return false;
					}
				}
				else if (!mapCore.Layers.HasVisibleLayer())
				{
					return false;
				}
			}
			return true;
		}

		private RectangleF GetLabelRect(MapGraphics g, RectangleF symbolRect)
		{
			PointF centerPointInContentPixels = this.GetCenterPointInContentPixels(g, 0);
			string text = (this.TextInt.IndexOf("#", StringComparison.Ordinal) == -1) ? this.TextInt : this.GetMapCore().ResolveAllKeywords(this.TextInt, this);
			text = text.Replace("\\n", "\n");
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			SizeF sizeF = g.MeasureString(text, this.FontInt, new SizeF(0f, 0f), stringFormat);
			RectangleF result = new RectangleF(centerPointInContentPixels.X, centerPointInContentPixels.Y, 0f, 0f);
			result.Inflate((float)(sizeF.Width / 2.0), (float)(sizeF.Height / 2.0));
			if (this.TextAlignmentInt == TextAlignment.Left)
			{
				float num = (float)(symbolRect.Width / 2.0 + result.Width / 2.0 + 3.0);
				result.X -= num;
			}
			else if (this.TextAlignmentInt == TextAlignment.Right)
			{
				float num2 = (float)(symbolRect.Width / 2.0 + result.Width / 2.0 + 3.0);
				result.X += num2;
			}
			else if (this.TextAlignmentInt == TextAlignment.Top)
			{
				float num3 = (float)(symbolRect.Height / 2.0 + result.Height / 2.0 + 3.0);
				result.Y -= num3;
			}
			else if (this.TextAlignmentInt == TextAlignment.Bottom)
			{
				float num4 = (float)(symbolRect.Height / 2.0 + result.Height / 2.0 + 3.0);
				result.Y += num4;
			}
			if (this.MarkerStyleInt == MarkerStyle.PushPin)
			{
				result.Offset((float)(0.0 - this.WidthInt), (float)(0.0 - this.HeightInt));
			}
			return result;
		}

		private void RenderText(MapGraphics g)
		{
			GraphicsPath[] paths = this.GetPaths(g);
			if (!string.IsNullOrEmpty(this.TextInt) && paths.Length != 0)
			{
				string text = (this.TextInt.IndexOf("#", StringComparison.Ordinal) == -1) ? this.TextInt : this.GetMapCore().ResolveAllKeywords(this.TextInt, this);
				text = text.Replace("\\n", "\n");
				StringFormat stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;
				RectangleF rectangleF = this.cachedPathBounds[this.cachedPathBounds.Length - 1];
				PointF point = new PointF((float)(rectangleF.Left + rectangleF.Width / 2.0), (float)(rectangleF.Top + rectangleF.Height / 2.0));
				if (this.TextShadowOffsetInt != 0)
				{
					using (Brush brush = g.GetShadowBrush())
					{
						PointF point2 = new PointF(point.X + (float)this.TextShadowOffsetInt, point.Y + (float)this.TextShadowOffsetInt);
						g.DrawString(text, this.FontInt, brush, point2, stringFormat);
					}
				}
				using (Brush brush2 = new SolidBrush(this.TextColorInt))
				{
					g.DrawString(text, this.FontInt, brush2, point, stringFormat);
				}
			}
		}

		internal bool IsRectangleVisible(MapGraphics g, RectangleF clipRect, MapPoint minExtent, MapPoint maxExtent)
		{
			PointF pointF = this.GetMapCore().GeographicToPercents(minExtent).ToPointF();
			PointF pointF2 = this.GetMapCore().GeographicToPercents(maxExtent).ToPointF();
			RectangleF relative = new RectangleF(pointF.X, pointF2.Y, pointF2.X - pointF.X, pointF.Y - pointF2.Y);
			relative = g.GetAbsoluteRectangle(relative);
			return clipRect.IntersectsWith(relative);
		}

		internal Brush GetBackBrush(MapGraphics g, GraphicsPath path)
		{
			RectangleF bounds = path.GetBounds();
			Brush brush = null;
			Color color = this.ApplyLayerTransparency(this.ColorInt);
			Color color2 = this.ApplyLayerTransparency(this.SecondaryColorInt);
			GradientType gradientType = this.GradientTypeInt;
			MapHatchStyle mapHatchStyle = this.HatchStyleInt;
			if (mapHatchStyle != 0)
			{
				return MapGraphics.GetHatchBrush(mapHatchStyle, color, color2);
			}
			if (gradientType != 0)
			{
				return g.GetGradientBrush(bounds, color, color2, gradientType);
			}
			return new SolidBrush(color);
		}

		internal Pen GetPen()
		{
			Pen pen = new Pen(this.ApplyLayerTransparency(this.BorderColorInt), (float)this.BorderWidthInt);
			pen.DashStyle = MapGraphics.GetPenStyle(this.BorderStyleInt);
			pen.Alignment = PenAlignment.Center;
			pen.LineJoin = LineJoin.Round;
			return pen;
		}

		internal RectangleF[] DrawImage(MapGraphics g, string imageName, bool drawShadow)
		{
			if (drawShadow && this.ShadowOffsetInt == 0)
			{
				return null;
			}
			Image image = this.Common.ImageLoader.LoadImage(imageName);
			if (image.Width != 0 && image.Height != 0)
			{
				ImageAttributes imageAttributes = new ImageAttributes();
				if (this.ImageTransColorInt != Color.Empty)
				{
					imageAttributes.SetColorKey(this.ImageTransColorInt, this.ImageTransColorInt, ColorAdjustType.Default);
				}
				float num = 1f;
				if (((ILayerElement)this).LayerObject != null && ((ILayerElement)this).LayerObject.Transparency > 0.0)
				{
					num = (float)((100.0 - ((ILayerElement)this).LayerObject.Transparency) / 100.0);
				}
				if (drawShadow)
				{
					float num2 = (float)(this.Common.MapCore.ShadowIntensity / 100.0);
					ColorMatrix colorMatrix = new ColorMatrix();
					colorMatrix.Matrix00 = 0f;
					colorMatrix.Matrix11 = 0f;
					colorMatrix.Matrix22 = 0f;
					colorMatrix.Matrix33 = num2 * num;
					imageAttributes.SetColorMatrix(colorMatrix);
				}
				else if (num < 1.0)
				{
					ColorMatrix colorMatrix2 = new ColorMatrix();
					colorMatrix2.Matrix33 = num;
					imageAttributes.SetColorMatrix(colorMatrix2);
				}
				RectangleF[] array = new RectangleF[this.SymbolData.Points.Length];
				for (int i = 0; i < this.SymbolData.Points.Length; i++)
				{
					bool flag = default(bool);
					PointF centerPointInContentPixels = this.GetCenterPointInContentPixels(g, i, out flag);
					if (flag)
					{
						RectangleF rectangleF = new RectangleF(centerPointInContentPixels.X, centerPointInContentPixels.Y, 0f, 0f);
						rectangleF.Inflate((float)(this.WidthInt / 2.0), (float)(this.HeightInt / 2.0));
						Rectangle destRect = Rectangle.Empty;
						if (this.ImageResizeModeInt == ResizeMode.AutoFit)
						{
							destRect = new Rectangle((int)rectangleF.X, (int)rectangleF.Y, (int)rectangleF.Width, (int)rectangleF.Height);
						}
						else
						{
							destRect = new Rectangle(0, 0, image.Width, image.Height);
							destRect.X = (int)Math.Round(centerPointInContentPixels.X - (float)destRect.Size.Width / 2.0);
							destRect.Y = (int)Math.Round(centerPointInContentPixels.Y - (float)destRect.Size.Height / 2.0);
						}
						if (drawShadow)
						{
							destRect.X += this.ShadowOffsetInt;
							destRect.Y += this.ShadowOffsetInt;
						}
						g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
						array[i] = new RectangleF((float)destRect.X, (float)destRect.Y, (float)destRect.Width, (float)destRect.Height);
					}
					else
					{
						array[i] = RectangleF.Empty;
					}
				}
				return array;
			}
			return null;
		}

		private Color ApplyLayerTransparency(Color color)
		{
			if (((ILayerElement)this).LayerObject != null && ((ILayerElement)this).LayerObject.Transparency != 0.0)
			{
				float num = (float)((100.0 - ((ILayerElement)this).LayerObject.Transparency) / 100.0);
				return Color.FromArgb((int)Math.Round((double)(num * (float)(int)color.A)), color);
			}
			return color;
		}

		bool IContentElement.IsVisible(MapGraphics g, Layer layer, bool allLayers, RectangleF clipRect)
		{
			if (!this.VisibleInt)
			{
				return false;
			}
			if (allLayers)
			{
				if (!((ILayerElement)this).BelongsToAllLayers)
				{
					return false;
				}
				goto IL_004b;
			}
			if (layer != null)
			{
				if (((ILayerElement)this).BelongsToLayer && !(layer.Name != ((ILayerElement)this).Layer))
				{
					goto IL_004b;
				}
				return false;
			}
			if (!((ILayerElement)this).BelongsToAllLayers && !((ILayerElement)this).BelongsToLayer)
			{
				goto IL_004b;
			}
			return false;
			IL_004b:
			GraphicsPath[] paths = this.GetPaths(g);
			if (paths == null)
			{
				return false;
			}
			for (int i = 0; i < paths.Length; i++)
			{
				if (paths[i] != null)
				{
					RectangleF rect = this.cachedPathBounds[i];
					if (clipRect.IntersectsWith(rect))
					{
						return true;
					}
				}
			}
			return false;
		}

		void IContentElement.RenderShadow(MapGraphics g)
		{
		}

		void IContentElement.RenderBack(MapGraphics g, HotRegionList hotRegions)
		{
		}

		void IContentElement.RenderFront(MapGraphics g, HotRegionList hotRegions)
		{
			g.StartHotRegion(this);
			if (!string.IsNullOrEmpty(this.ImageInt))
			{
				ImageSmoothingState imageSmoothingState = new ImageSmoothingState(g);
				imageSmoothingState.Set();
				this.DrawImage(g, this.ImageInt, true);
				this.DrawImage(g, this.ImageInt, false);
				imageSmoothingState.Restore();
			}
			try
			{
				GraphicsPath[] paths = this.GetPaths(g);
				if (this.MarkerStyleInt != 0)
				{
					for (int i = 0; i < paths.Length - 1; i++)
					{
						if (string.IsNullOrEmpty(this.ImageInt))
						{
							if (Symbol.IsXamlMarker(this.MarkerStyleInt))
							{
								XamlRenderer xamlRenderer = this.GetXamlRenderers(g)[i];
								if (xamlRenderer != null)
								{
									XamlLayer[] layers = xamlRenderer.Layers;
									foreach (XamlLayer xamlLayer in layers)
									{
										xamlLayer.Render(g);
									}
								}
							}
							else
							{
								if (this.ShadowOffsetInt != 0)
								{
									using (Matrix matrix = new Matrix())
									{
										int num = this.ShadowOffsetInt;
										matrix.Translate((float)num, (float)num, MatrixOrder.Append);
										paths[i].Transform(matrix);
										using (Brush brush = g.GetShadowBrush())
										{
											g.FillPath(brush, paths[i]);
										}
										matrix.Reset();
										matrix.Translate((float)(-num), (float)(-num), MatrixOrder.Append);
										paths[i].Transform(matrix);
									}
								}
								using (Brush brush2 = this.GetBackBrush(g, paths[i]))
								{
									g.FillPath(brush2, paths[i]);
								}
								if (this.BorderWidthInt > 0)
								{
									using (this.GetPen())
									{
										g.DrawPath(this.GetPen(), paths[i]);
									}
								}
							}
						}
					}
				}
				hotRegions.SetHotRegion(g, this, paths);
			}
			finally
			{
				g.EndHotRegion();
			}
		}

		void IContentElement.RenderText(MapGraphics g, HotRegionList hotRegions)
		{
			this.RenderText(g);
		}

		RectangleF IContentElement.GetBoundRect(MapGraphics g)
		{
			return new RectangleF(0f, 0f, 100f, 100f);
		}

		string IToolTipProvider.GetToolTip()
		{
			if (this.Common != null && this.Common.MapCore != null)
			{
				return this.Common.MapCore.ResolveAllKeywords(this.ToolTipInt, this);
			}
			return this.ToolTipInt;
		}

		string IImageMapProvider.GetToolTip()
		{
			return ((IToolTipProvider)this).GetToolTip();
		}

		string IImageMapProvider.GetHref()
		{
			if (this.Common != null && this.Common.MapCore != null)
			{
				return this.Common.MapCore.ResolveAllKeywords(this.Href, this);
			}
			return this.Href;
		}

		string IImageMapProvider.GetMapAreaAttributes()
		{
			if (this.Common != null && this.Common.MapCore != null)
			{
				return this.Common.MapCore.ResolveAllKeywords(this.MapAreaAttributes, this);
			}
			return this.MapAreaAttributes;
		}

		void ISelectable.DrawSelection(MapGraphics g, RectangleF clipRect, bool designTimeSelection)
		{
			MapCore mapCore = this.GetMapCore();
			GraphicsPath[] paths = this.GetPaths(g);
			if (paths != null && paths.Length != 0)
			{
				RectangleF selectionRectangle = ((ISelectable)this).GetSelectionRectangle(g, clipRect);
				RectangleF rect = selectionRectangle;
				rect.Inflate(6f, 6f);
				if (clipRect.IntersectsWith(rect) && !selectionRectangle.IsEmpty)
				{
					g.DrawSelection(selectionRectangle, designTimeSelection, mapCore.SelectionBorderColor, mapCore.SelectionMarkerColor);
				}
			}
		}

		RectangleF ISelectable.GetSelectionRectangle(MapGraphics g, RectangleF clipRect)
		{
			GraphicsPath[] paths = this.GetPaths(g);
			RectangleF rectangleF = this.cachedPathBounds[0];
			for (int i = 0; i < paths.Length; i++)
			{
				if (paths[i] != null)
				{
					RectangleF b = this.cachedPathBounds[i];
					rectangleF = RectangleF.Union(rectangleF, b);
				}
			}
			return rectangleF;
		}

		bool ISelectable.IsSelected()
		{
			return this.Selected;
		}

		bool ISelectable.IsVisible()
		{
			return this.Visible;
		}
	}
}
