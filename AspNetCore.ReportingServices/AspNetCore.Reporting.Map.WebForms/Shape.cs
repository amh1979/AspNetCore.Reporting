//
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(ShapeConverter))]
	internal class Shape : NamedElement, IContentElement, ILayerElement, IToolTipProvider, ISelectable, ISpatialElement, IImageMapProvider
	{
		private GraphicsPath[] cachedPaths;

		private RectangleF[] cachedPathBounds;

		private RectangleF cachedTextBounds = Rectangle.Empty;

		internal int largestPathIndex;

		internal Hashtable fields;

		private string fieldDataBuffer = string.Empty;

		private ShapeData shapeData;

		private Offset offset;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private ContentAlignment textAlignment = ContentAlignment.MiddleCenter;

		private bool visible = true;

		private TextVisibility textVisibility = TextVisibility.Auto;

		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private Color borderColor = Color.DarkGray;

		private MapDashStyle borderStyle = MapDashStyle.Solid;

		private int borderWidth = 1;

		private Color color = Color.Empty;

		private Color textColor = Color.Black;

		private GradientType gradientType;

		private Color secondaryColor = Color.Empty;

		private MapHatchStyle hatchStyle;

		private string text = "#NAME";

		private int shadowOffset;

		private int textShadowOffset;

		private bool selected;

		private Offset centralPointOffset;

		private string category = string.Empty;

		private string parentGroup = "(none)";

		private int childSymbolMargin;

		private double scaleFactor = 1.0;

		private ArrayList symbols;

		private Group parentGroupObject;

		private bool useInternalProperties;

		private Color borderColorInt = Color.DarkGray;

		private Color colorInt = Color.Empty;

		private GradientType gradientTypeInt;

		private Color secondaryColorInt = Color.Empty;

		private MapHatchStyle hatchStyleInt;

		private string textInt = "#NAME";

		private string toolTipInt = "";

		private object mapAreaTag;

		private string layer = "(none)";

		private bool belongsToLayer;

		private bool belongsToAllLayers;

		private Layer layerObject;

		MapPoint ISpatialElement.MinimumExtent
		{
			get
			{
				return this.ShapeData.MinimumExtent;
			}
		}

		MapPoint ISpatialElement.MaximumExtent
		{
			get
			{
				return this.ShapeData.MaximumExtent;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeShape_ShapeData")]
		public ShapeData ShapeData
		{
			get
			{
				return this.shapeData;
			}
			set
			{
				this.shapeData = value;
				this.ResetCachedPaths();
				this.InvalidateCachedBounds();
				this.InvalidateViewport();
			}
		}

		MapPoint[] ISpatialElement.Points
		{
			get
			{
				return this.ShapeData.Points;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public string EncodedShapeData
		{
			get
			{
				return ShapeData.ShapeDataToString(this.ShapeData);
			}
			set
			{
				this.shapeData = ShapeData.ShapeDataFromString(value);
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

		[SRDescription("DescriptionAttributeShape_Offset")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryAttribute_Behavior")]
		[TypeConverter(typeof(ShapeOffsetConverter))]
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

		[TypeConverter(typeof(KeywordConverter))]
		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeShape_ToolTip")]
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
				this.InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeShape_Href")]
		[Localizable(true)]
		[DefaultValue("")]
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
		[NotifyParentProperty(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeShape_MapAreaAttributes")]
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

		[SRDescription("DescriptionAttributeShape_Name")]
		[SRCategory("CategoryAttribute_Data")]
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

		[SRDescription("DescriptionAttributeShape_TextAlignment")]
		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue(ContentAlignment.MiddleCenter)]
		public ContentAlignment TextAlignment
		{
			get
			{
				return this.textAlignment;
			}
			set
			{
				this.textAlignment = value;
				this.InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeShape_Visible")]
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

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeShape_TextVisibility")]
		[DefaultValue(TextVisibility.Auto)]
		public TextVisibility TextVisibility
		{
			get
			{
				return this.textVisibility;
			}
			set
			{
				this.textVisibility = value;
				this.InvalidateViewport();
			}
		}

		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8.25pt")]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeShape_Font")]
		public Font Font
		{
			get
			{
				return this.font;
			}
			set
			{
				this.font = value;
				this.InvalidateViewport();
			}
		}

		[DefaultValue(typeof(Color), "DarkGray")]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeShape_BorderColor")]
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
				this.InvalidateViewport();
			}
		}

		[SRDescription("DescriptionAttributeShape_BorderStyle")]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(MapDashStyle.Solid)]
		[NotifyParentProperty(true)]
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

		[DefaultValue(1)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeShape_BorderWidth")]
		[NotifyParentProperty(true)]
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

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeShape_Color")]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(typeof(Color), "")]
		public Color Color
		{
			get
			{
				return this.color;
			}
			set
			{
				this.color = value;
				this.InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeShape_TextColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Black")]
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

		[DefaultValue(GradientType.None)]
		[SRDescription("DescriptionAttributeShape_GradientType")]
		[NotifyParentProperty(true)]
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

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeShape_SecondaryColor")]
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

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeShape_HatchStyle")]
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

		[TypeConverter(typeof(KeywordConverter))]
		[DefaultValue("#NAME")]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeShape_Text")]
		[Localizable(true)]
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
				this.cachedTextBounds = RectangleF.Empty;
				this.InvalidateViewport();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeShape_ShadowOffset")]
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

		[SRDescription("DescriptionAttributeShape_TextShadowOffset")]
		[DefaultValue(0)]
		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
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

		[SRDescription("DescriptionAttributeShape_Selected")]
		[DefaultValue(false)]
		[Browsable(false)]
		[SRCategory("CategoryAttribute_Behavior")]
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

		[SRCategory("CategoryAttribute_Behavior")]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeShape_CentralPoint")]
		public MapPoint CentralPoint
		{
			get
			{
				if (this.ShapeData.Points != null)
				{
					if (this.ShapeData.Segments.Length != 0 && this.ShapeData.Points.Length != 0)
					{
						return this.ShapeData.Segments[this.ShapeData.LargestSegmentIndex].PolygonCentroid;
					}
					return new MapPoint(0.0, 0.0);
				}
				return new MapPoint(0.0, 0.0);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeShape_CentralPointOffset")]
		[TypeConverter(typeof(ShapeOffsetConverter))]
		public Offset CentralPointOffset
		{
			get
			{
				return this.centralPointOffset;
			}
			set
			{
				this.centralPointOffset = value;
				this.centralPointOffset.Parent = this;
				this.InvalidateViewport();
			}
		}

		[SRDescription("DescriptionAttributeShape_Category")]
		[SRCategory("CategoryAttribute_Data")]
		[DefaultValue("")]
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

		[SRDescription("DescriptionAttributeShape_ParentGroup")]
		[TypeConverter(typeof(DesignTimeGroupConverter))]
		[DefaultValue("(none)")]
		[SRCategory("CategoryAttribute_Behavior")]
		public string ParentGroup
		{
			get
			{
				return this.parentGroup;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.parentGroup = "(none)";
				}
				else
				{
					this.parentGroup = value;
				}
				this.parentGroupObject = null;
				this.InvalidateCachedShapesInGroups();
				this.InvalidateViewport();
			}
		}

		[DefaultValue(0)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeShape_ChildSymbolMargin")]
		public int ChildSymbolMargin
		{
			get
			{
				return this.childSymbolMargin;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException(SR.ExceptionValueCannotBeNegative);
				}
				this.childSymbolMargin = value;
				this.InvalidateChildSymbols();
				this.InvalidateViewport();
			}
		}

		[SRDescription("DescriptionAttributeShape_ScaleFactor")]
		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue(1.0)]
		public double ScaleFactor
		{
			get
			{
				return this.scaleFactor;
			}
			set
			{
				if (!(value < 0.0) && !(value > 100.0))
				{
					this.scaleFactor = value;
					this.ResetCachedPaths();
					this.InvalidateViewport();
					return;
				}
				throw new ArgumentException(SR.must_in_range(0.0, 100.0));
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
						Field field = (Field)mapCore.ShapeFields.GetByName(name);
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

		internal ArrayList Symbols
		{
			get
			{
				MapCore mapCore = this.GetMapCore();
				if (mapCore != null && this.symbols == null)
				{
					this.symbols = new ArrayList();
					foreach (Symbol symbol in mapCore.Symbols)
					{
						if (symbol.ParentShape == this.Name)
						{
							this.symbols.Add(symbol);
						}
					}
				}
				return this.symbols;
			}
			set
			{
				this.symbols = value;
			}
		}

		internal Group ParentGroupObject
		{
			get
			{
				if (this.parentGroup == "(none)")
				{
					return null;
				}
				if (this.parentGroupObject == null)
				{
					MapCore mapCore = this.GetMapCore();
					if (mapCore != null)
					{
						this.parentGroupObject = (Group)mapCore.Groups.GetByName(this.parentGroup);
					}
				}
				return this.parentGroupObject;
			}
			set
			{
				this.parentGroupObject = value;
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
				if (this.borderColor == Color.DarkGray)
				{
					if (this.ParentGroupObject != null)
					{
						return this.ParentGroupObject.BorderColorInt;
					}
					if (this.useInternalProperties)
					{
						return this.borderColorInt;
					}
				}
				return this.borderColor;
			}
			set
			{
				this.borderColorInt = value;
				this.InvalidateViewport();
			}
		}

		internal Color ColorInt
		{
			get
			{
				if (this.color.IsEmpty)
				{
					if (this.ParentGroupObject != null)
					{
						return this.ParentGroupObject.ColorInt;
					}
					if (this.useInternalProperties)
					{
						return this.colorInt;
					}
				}
				return this.color;
			}
			set
			{
				this.colorInt = value;
				this.InvalidateViewport();
			}
		}

		internal GradientType GradientTypeInt
		{
			get
			{
				if (this.gradientType == GradientType.None)
				{
					if (this.ParentGroupObject != null)
					{
						return this.ParentGroupObject.GradientTypeInt;
					}
					if (this.useInternalProperties)
					{
						return this.gradientTypeInt;
					}
				}
				return this.gradientType;
			}
			set
			{
				this.gradientTypeInt = value;
				this.InvalidateViewport();
			}
		}

		internal Color SecondaryColorInt
		{
			get
			{
				if (this.secondaryColor.IsEmpty)
				{
					if (this.ParentGroupObject != null)
					{
						return this.ParentGroupObject.SecondaryColorInt;
					}
					if (this.useInternalProperties)
					{
						return this.secondaryColorInt;
					}
				}
				return this.secondaryColor;
			}
			set
			{
				this.secondaryColorInt = value;
				this.InvalidateViewport();
			}
		}

		internal MapHatchStyle HatchStyleInt
		{
			get
			{
				if (this.hatchStyle == MapHatchStyle.None)
				{
					if (this.ParentGroupObject != null)
					{
						return this.ParentGroupObject.HatchStyleInt;
					}
					if (this.useInternalProperties)
					{
						return this.hatchStyleInt;
					}
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
				if (this.text == "#NAME" && this.useInternalProperties)
				{
					return this.textInt;
				}
				return this.text;
			}
			set
			{
				this.textInt = value;
				this.cachedTextBounds = RectangleF.Empty;
				this.InvalidateViewport();
			}
		}

		internal string ToolTipInt
		{
			get
			{
				if (string.IsNullOrEmpty(this.toolTip))
				{
					if (this.ParentGroupObject != null)
					{
						return ((IToolTipProvider)this.ParentGroupObject).GetToolTip();
					}
					if (this.useInternalProperties)
					{
						return this.toolTipInt;
					}
				}
				return this.toolTip;
			}
			set
			{
				this.toolTipInt = value;
			}
		}

		internal Offset OffsetInt
		{
			get
			{
				if (this.ParentGroupObject != null)
				{
					return new Offset(this, this.ParentGroupObject.Offset.X + this.offset.X, this.ParentGroupObject.Offset.Y + this.offset.Y);
				}
				return this.offset;
			}
		}

		internal MapDashStyle BorderStyleInt
		{
			get
			{
				if (this.borderStyle == MapDashStyle.Solid && this.ParentGroupObject != null)
				{
					return this.ParentGroupObject.BorderStyle;
				}
				return this.borderStyle;
			}
		}

		internal int BorderWidthInt
		{
			get
			{
				if (this.borderWidth == 1 && this.ParentGroupObject != null)
				{
					return this.ParentGroupObject.BorderWidth;
				}
				return this.borderWidth;
			}
		}

		internal int ShadowOffsetInt
		{
			get
			{
				if (this.shadowOffset == 0 && this.ParentGroupObject != null)
				{
					return this.ParentGroupObject.ShadowOffset;
				}
				return this.shadowOffset;
			}
		}

		internal bool VisibleInt
		{
			get
			{
				if (this.visible && this.ParentGroupObject != null)
				{
					return this.ParentGroupObject.Visible;
				}
				return this.visible;
			}
		}

		internal string HrefInt
		{
			get
			{
				if (string.IsNullOrEmpty(this.href) && this.ParentGroupObject != null)
				{
					return this.ParentGroupObject.Href;
				}
				return this.href;
			}
		}

		internal string MapAreaAttributesInt
		{
			get
			{
				if (string.IsNullOrEmpty(this.mapAreaAttributes) && this.ParentGroupObject != null)
				{
					return this.ParentGroupObject.MapAreaAttributes;
				}
				return this.mapAreaAttributes;
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

		[SRDescription("DescriptionAttributeShape_Layer")]
		[TypeConverter(typeof(DesignTimeLayerConverter))]
		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue("(none)")]
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
					this.layerObject = null;
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

		public Shape()
			: this(null)
		{
		}

		internal Shape(CommonElements common)
			: base(common)
		{
			this.shapeData = new ShapeData();
			this.offset = new Offset(this, 0.0, 0.0);
			this.centralPointOffset = new Offset(this, 0.0, 0.0);
			this.fields = new Hashtable();
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected bool ShouldSerializeCentralPointOffset()
		{
			if (this.CentralPointOffset.X == 0.0)
			{
				return this.CentralPointOffset.Y != 0.0;
			}
			return true;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected void ResetCentralPointOffset()
		{
			this.CentralPointOffset.X = 0.0;
			this.CentralPointOffset.Y = 0.0;
		}

		public override string ToString()
		{
			return this.Name;
		}

		public void StartNewFigure()
		{
			if (this.ShapeData.Segments != null && this.ShapeData.Segments.Length != 0)
			{
				ShapeSegment[] segments = this.ShapeData.Segments;
				Array.Resize(ref segments, this.ShapeData.Segments.Length + 1);
				this.ShapeData.Segments = segments;
				this.ShapeData.Segments[this.ShapeData.Segments.Length - 1].Type = SegmentType.StartFigure;
			}
		}

		public void AddSegments(MapPoint[] points, ShapeSegment[] segments)
		{
			if (this.ShapeData.Points == null || this.ShapeData.Points.Length == 0)
			{
				this.ShapeData.Points = points;
			}
			else
			{
				MapPoint[] array = new MapPoint[this.ShapeData.Points.Length + points.Length];
				Array.Copy(this.ShapeData.Points, array, this.ShapeData.Points.Length);
				Array.Copy(points, 0, array, this.ShapeData.Points.Length, points.Length);
				this.ShapeData.Points = array;
			}
			if (this.ShapeData.Segments == null || this.ShapeData.Segments.Length == 0)
			{
				this.ShapeData.Segments = segments;
			}
			else
			{
				ShapeSegment[] array2 = new ShapeSegment[this.ShapeData.Segments.Length + segments.Length];
				Array.Copy(this.ShapeData.Segments, array2, this.ShapeData.Segments.Length);
				Array.Copy(segments, 0, array2, this.ShapeData.Segments.Length, segments.Length);
				this.ShapeData.Segments = array2;
			}
			this.ShapeData.UpdateStoredParameters();
			this.InvalidateCachedBounds();
			this.InvalidateCachedShapesInGroups();
			this.InvalidateRules();
			this.InvalidateViewport();
		}

		public void ClearShapeData()
		{
			this.ShapeData.Segments = null;
			this.ShapeData.Points = null;
			this.ShapeData.UpdateStoredParameters();
			this.InvalidateCachedBounds();
			this.InvalidateCachedShapesInGroups();
			this.InvalidateRules();
			this.InvalidateViewport();
		}

		public PointF GetCenterPointInContentPixels(MapGraphics g)
		{
			MapCore mapCore = this.GetMapCore();
			if (mapCore == null)
			{
				return PointF.Empty;
			}
			MapPoint centralPoint = this.CentralPoint;
			centralPoint.X += this.OffsetInt.X + this.CentralPointOffset.X;
			centralPoint.Y += this.OffsetInt.Y + this.CentralPointOffset.Y;
			PointF relative = mapCore.GeographicToPercents(centralPoint).ToPointF();
			return g.GetAbsolutePoint(relative);
		}

		public bool IsPointInShape(MapPoint mapPoint)
		{
			bool result = false;
			PointF point = new PointF((float)mapPoint.X, (float)mapPoint.Y);
			GraphicsPath[] geographicGraphicsPaths = this.GetGeographicGraphicsPaths();
			GraphicsPath[] array = geographicGraphicsPaths;
			foreach (GraphicsPath graphicsPath in array)
			{
				if (graphicsPath.IsVisible(point) || graphicsPath.IsOutlineVisible(point, Pens.Black))
				{
					result = true;
				}
				graphicsPath.Dispose();
			}
			return result;
		}

		internal MapCore GetMapCore()
		{
			return (MapCore)this.ParentElement;
		}

		private void InvalidateCachedShapesInGroups()
		{
			MapCore mapCore = this.GetMapCore();
			if (mapCore != null)
			{
				foreach (Group group in mapCore.Groups)
				{
					group.Shapes = null;
				}
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

		private string FieldDataToString()
		{
			string text = string.Empty;
			MapCore mapCore = this.GetMapCore();
			if (mapCore != null)
			{
				foreach (Field shapeField in mapCore.ShapeFields)
				{
					if (!shapeField.IsTemporary)
					{
						string text2 = shapeField.FormatValue(this.fields[shapeField.Name]);
						if (!string.IsNullOrEmpty(text2))
						{
							string text3 = text;
							text = text3 + XmlConvert.EncodeName(shapeField.Name) + "=" + text2 + "&";
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
						Field field = (Field)mapCore.ShapeFields.GetByName(name);
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
			this.InvalidateRules();
		}

		internal override void OnRemove()
		{
			base.OnRemove();
			MapCore mapCore = this.GetMapCore();
			if (mapCore != null)
			{
				foreach (Symbol symbol in mapCore.Symbols)
				{
					if (symbol.ParentShape == this.Name)
					{
						symbol.ParentShape = "";
					}
				}
				mapCore.InvalidateRules();
				mapCore.InvalidateDataBinding();
			}
			this.InvalidateCachedShapesInGroups();
		}

		protected override void OnDispose()
		{
			this.ResetCachedPaths();
			this.ShapeData.Points = null;
			this.ShapeData.Segments = null;
			if (this.fields != null)
			{
				this.fields.Clear();
			}
			if (this.symbols != null)
			{
				this.symbols.Clear();
			}
			base.OnDispose();
		}

		internal void ApplyCustomColorAttributes(CustomColor customColor)
		{
			this.UseInternalProperties = true;
			this.BorderColorInt = customColor.BorderColor;
			this.ColorInt = customColor.Color;
			this.SecondaryColorInt = customColor.SecondaryColor;
			this.GradientTypeInt = customColor.GradientType;
			this.HatchStyleInt = customColor.HatchStyle;
			this.TextInt = customColor.Text;
			this.ToolTipInt = customColor.ToolTip;
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

		internal IEnumerable<RectangleF> GetGeographicRectangles()
		{
			for (int i = 0; i < this.ShapeData.Segments.Length; i++)
			{
				double verticalOffsetFactor = this.GetVerticalOffsetFactor();
				ShapeSegment segment = this.ShapeData.Segments[i];
				if (segment.Length > 0)
				{
					MapPoint min = this.OffsetAndScaleGeoPoint(segment.MinimumExtent, verticalOffsetFactor);
					MapPoint max = this.OffsetAndScaleGeoPoint(segment.MaximumExtent, verticalOffsetFactor);
					RectangleF result = new RectangleF((float)min.X, (float)min.Y, (float)(max.X - min.X), (float)(max.Y - min.Y));
					yield return result;
				}
			}
		}
        /*
		public bool AddGeometry(SqlGeometry geometry)
		{
			geometry = geometry.MakeValid();
			if (geometry.STIsEmpty())
			{
				return false;
			}
			geometry = geometry.STCurveToLine();
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			this.AddGeometryRec(geometry, arrayList, arrayList2);
			if (arrayList.Count > 0)
			{
				this.AddSegments((MapPoint[])arrayList.ToArray(typeof(MapPoint)), (ShapeSegment[])arrayList2.ToArray(typeof(ShapeSegment)));
				return true;
			}
			return false;
		}

		private void AddGeometryRec(SqlGeometry geometry, ArrayList pointsList, ArrayList segmentsList)
		{
			string value = geometry.STGeometryType().Value;
			bool flag = value == "GeometryCollection";
			if (flag || value == "MultiPolygon")
			{
				for (int i = 1; i <= geometry.STNumGeometries().Value; i++)
				{
					if (flag)
					{
						this.AddGeometryRec(geometry.STGeometryN(i), pointsList, segmentsList);
					}
					else
					{
						this.AddSimpleGeometry(geometry.STGeometryN(i), pointsList, segmentsList);
					}
				}
			}
			else if (value == "Polygon")
			{
				this.AddSimpleGeometry(geometry, pointsList, segmentsList);
			}
		}

		private void AddSimpleGeometry(SqlGeometry geometry, ArrayList pointsList, ArrayList segmentsList)
		{
			if (!geometry.STIsEmpty())
			{
				ArrayList arrayList = new ArrayList();
				if (!geometry.STExteriorRing().IsNull)
				{
					arrayList.Add(geometry.STExteriorRing());
				}
				for (int i = 1; i <= geometry.STNumInteriorRing().Value; i++)
				{
					if (!geometry.STInteriorRingN(i).IsNull)
					{
						arrayList.Add(geometry.STInteriorRingN(i));
					}
				}
				SqlGeometry[] array = (SqlGeometry[])arrayList.ToArray(typeof(SqlGeometry));
				ShapeSegment[] array2 = new ShapeSegment[array.Length];
				MapPoint[] array3 = new MapPoint[geometry.STNumPoints().Value];
				int num = 1;
				int num2 = 1;
				for (int j = 0; j < array.Length; j++)
				{
					array2[j].Type = SegmentType.Polygon;
					array2[j].Length = array[j].STNumPoints().Value;
					for (int num3 = num + array2[j].Length - 1; num3 >= num; num3--)
					{
						array3[num3 - 1].X = geometry.STPointN(num2).STX.Value;
						array3[num3 - 1].Y = geometry.STPointN(num2).STY.Value;
						num2++;
					}
					num += array2[j].Length;
				}
				GeoUtils.FixOrientationForGeometry(ref array3, ref array2);
				pointsList.AddRange(array3);
				ShapeSegment shapeSegment = default(ShapeSegment);
				shapeSegment.Type = SegmentType.StartFigure;
				segmentsList.Add(shapeSegment);
				segmentsList.AddRange(array2);
			}
		}

		public bool AddGeography(SqlGeography geography)
		{
			geography = geography.MakeValid();
			if (geography.STIsEmpty())
			{
				return false;
			}
			geography = geography.STIntersection(this.GetMapCore().GetClippingPolygon(geography.STSrid.Value));
			geography = geography.STCurveToLine();
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			this.AddGeographyRec(geography, arrayList, arrayList2);
			if (arrayList.Count > 0)
			{
				this.AddSegments((MapPoint[])arrayList.ToArray(typeof(MapPoint)), (ShapeSegment[])arrayList2.ToArray(typeof(ShapeSegment)));
				return true;
			}
			return false;
		}

		private void AddGeographyRec(SqlGeography geography, ArrayList pointsList, ArrayList segmentsList)
		{
			string value = geography.STGeometryType().Value;
			bool flag = value == "GeometryCollection";
			if (flag || value == "MultiPolygon")
			{
				for (int i = 1; i <= geography.STNumGeometries().Value; i++)
				{
					if (flag)
					{
						this.AddGeographyRec(geography.STGeometryN(i), pointsList, segmentsList);
					}
					else
					{
						this.AddSimpleGeography(geography.STGeometryN(i), pointsList, segmentsList);
					}
				}
			}
			else if (value == "Polygon")
			{
				this.AddSimpleGeography(geography, pointsList, segmentsList);
			}
		}

		private void AddSimpleGeography(SqlGeography geography, ArrayList pointsList, ArrayList segmentsList)
		{
			if (!geography.STIsEmpty())
			{
				ArrayList arrayList = new ArrayList();
				if (!geography.NumRings().IsNull && geography.NumRings().Value > 0)
				{
					for (int i = 1; i <= geography.NumRings().Value; i++)
					{
						if (!geography.RingN(i).IsNull)
						{
							arrayList.Add(geography.RingN(i));
						}
					}
				}
				else if (!geography.IsNull)
				{
					arrayList.Add(geography);
				}
				SqlGeography[] array = (SqlGeography[])arrayList.ToArray(typeof(SqlGeography));
				ShapeSegment[] array2 = new ShapeSegment[array.Length];
				MapPoint[] array3 = new MapPoint[geography.STNumPoints().Value];
				int num = 1;
				int num2 = 1;
				for (int j = 0; j < array.Length; j++)
				{
					array2[j].Type = SegmentType.Polygon;
					array2[j].Length = array[j].STNumPoints().Value;
					for (int num3 = num + array2[j].Length - 1; num3 >= num; num3--)
					{
						array3[num3 - 1].X = geography.STPointN(num2).Long.Value;
						array3[num3 - 1].Y = geography.STPointN(num2).Lat.Value;
						num2++;
					}
					num += array2[j].Length;
				}
				GeoUtils.NormalizePointsLongigute(ref array3);
				GeoUtils.CutShapes(ref array3, ref array2);
				pointsList.AddRange(array3);
				ShapeSegment shapeSegment = default(ShapeSegment);
				shapeSegment.Type = SegmentType.StartFigure;
				segmentsList.Add(shapeSegment);
				segmentsList.AddRange(array2);
			}
		}
        */
		public bool LoadWKT(string wkt)
		{
			this.ClearShapeData();
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
			this.ClearShapeData();
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
			if (this.ShapeData != null && this.ShapeData.Segments != null && this.ShapeData.Segments.Length != 0 && this.ShapeData.Points != null && this.ShapeData.Points.Length != 0)
			{
				MapPoint[] points = this.ShapeData.Points;
				ShapeSegment[] segments = this.ShapeData.Segments;
				int num = 0;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("MULTIPOLYGON(");
				int num2 = 0;
				for (int i = 0; i < segments.Length; i++)
				{
					if (segments[i].Type == SegmentType.StartFigure)
					{
						num2++;
					}
					else
					{
						if (i > num2)
						{
							stringBuilder.Append(", ");
						}
						if (segments[i].PolygonSignedArea <= 0.0)
						{
							stringBuilder.Append("(");
						}
						stringBuilder.Append("(");
						for (int num3 = num + segments[i].Length - 1; num3 >= num; num3--)
						{
							stringBuilder.Append(points[num3].X.ToString(CultureInfo.InvariantCulture) + " " + points[num3].Y.ToString(CultureInfo.InvariantCulture));
							if (num3 > num)
							{
								stringBuilder.Append(", ");
							}
						}
						stringBuilder.Append(")");
						if (i == segments.Length - 1 || segments[i + 1].PolygonSignedArea <= 0.0)
						{
							stringBuilder.Append(")");
						}
						num += segments[i].Length;
					}
				}
				stringBuilder.Append(")");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public byte[] SaveWKB()
		{
			if (this.ShapeData == null)
			{
				return null;
			}
			MemoryStream memoryStream = new MemoryStream();
			this.SaveWKBToStream(memoryStream);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return memoryStream.ToArray();
		}

		private void SaveWKBToStream(Stream stream)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			byte value = (byte)(BitConverter.IsLittleEndian ? 1 : 0);
			binaryWriter.Write(value);
			binaryWriter.Write(6u);
			if (this.ShapeData == null || this.ShapeData.Segments == null || this.ShapeData.Segments.Length == 0 || this.ShapeData.Points == null || this.ShapeData.Points.Length == 0)
			{
				binaryWriter.Write(0u);
			}
			else
			{
				MapPoint[] points = this.ShapeData.Points;
				ShapeSegment[] segments = this.ShapeData.Segments;
				List<int> list = new List<int>();
				for (int i = 0; i < segments.Length; i++)
				{
					if (segments[i].Type != SegmentType.StartFigure)
					{
						if (segments[i].PolygonSignedArea <= 0.0)
						{
							list.Add(1);
						}
						else if (list.Count > 0)
						{
							List<int> list2;
							int index;
							(list2 = list)[index = list.Count - 1] = list2[index] + 1;
						}
					}
				}
				binaryWriter.Write((uint)list.Count);
				int num = 0;
				int num2 = 0;
				for (int j = 0; j < segments.Length; j++)
				{
					if (segments[j].Type != SegmentType.StartFigure)
					{
						if (segments[j].PolygonSignedArea <= 0.0)
						{
							binaryWriter.Write(value);
							binaryWriter.Write(3u);
							binaryWriter.Write((uint)list[num2]);
							num2++;
						}
						binaryWriter.Write((uint)segments[j].Length);
						for (int num3 = num + segments[j].Length - 1; num3 >= num; num3--)
						{
							binaryWriter.Write(points[num3].X);
							binaryWriter.Write(points[num3].Y);
						}
						num += segments[j].Length;
					}
				}
			}
		}

		private void RenderText(MapGraphics g)
		{
			if (!string.IsNullOrEmpty(this.TextInt) && this.cachedPaths.Length != 0 && this.TextVisibility != TextVisibility.Hidden)
			{
				string text = (this.TextInt.IndexOf("#", StringComparison.Ordinal) == -1) ? this.TextInt : this.GetMapCore().ResolveAllKeywords(this.TextInt, this);
				text = text.Replace("\\n", "\n");
				StringFormat stringFormat = new StringFormat();
				if (this.TextAlignment == ContentAlignment.BottomRight)
				{
					stringFormat.Alignment = StringAlignment.Near;
					stringFormat.LineAlignment = StringAlignment.Near;
				}
				else if (this.TextAlignment == ContentAlignment.BottomCenter)
				{
					stringFormat.Alignment = StringAlignment.Center;
					stringFormat.LineAlignment = StringAlignment.Near;
				}
				else if (this.TextAlignment == ContentAlignment.BottomLeft)
				{
					stringFormat.Alignment = StringAlignment.Far;
					stringFormat.LineAlignment = StringAlignment.Near;
				}
				else if (this.TextAlignment == ContentAlignment.MiddleRight)
				{
					stringFormat.Alignment = StringAlignment.Near;
					stringFormat.LineAlignment = StringAlignment.Center;
				}
				else if (this.TextAlignment == ContentAlignment.MiddleCenter)
				{
					stringFormat.Alignment = StringAlignment.Center;
					stringFormat.LineAlignment = StringAlignment.Center;
				}
				else if (this.TextAlignment == ContentAlignment.MiddleLeft)
				{
					stringFormat.Alignment = StringAlignment.Far;
					stringFormat.LineAlignment = StringAlignment.Center;
				}
				else if (this.TextAlignment == ContentAlignment.TopRight)
				{
					stringFormat.Alignment = StringAlignment.Near;
					stringFormat.LineAlignment = StringAlignment.Far;
				}
				else if (this.TextAlignment == ContentAlignment.TopCenter)
				{
					stringFormat.Alignment = StringAlignment.Center;
					stringFormat.LineAlignment = StringAlignment.Far;
				}
				else
				{
					stringFormat.Alignment = StringAlignment.Far;
					stringFormat.LineAlignment = StringAlignment.Far;
				}
				SizeF sizeF = g.MeasureString(text, this.Font, new SizeF(0f, 0f), StringFormat.GenericTypographic);
				if (this.TextVisibility == TextVisibility.Auto && sizeF.Width > this.cachedPathBounds[this.largestPathIndex].Width)
				{
					return;
				}
				PointF centerPointInContentPixels = this.GetCenterPointInContentPixels(g);
				new RectangleF(centerPointInContentPixels.X, (float)(centerPointInContentPixels.Y - 1.0), 0f, 0f).Inflate((float)(sizeF.Width / 2.0), (float)(sizeF.Height / 2.0));
				if (this.TextShadowOffset != 0)
				{
					using (Brush brush = g.GetShadowBrush())
					{
						PointF point = new PointF(centerPointInContentPixels.X + (float)this.TextShadowOffset, centerPointInContentPixels.Y + (float)this.TextShadowOffset);
						g.DrawString(text, this.Font, brush, point, stringFormat);
					}
				}
				using (Brush brush2 = new SolidBrush(this.TextColor))
				{
					g.DrawString(text, this.Font, brush2, centerPointInContentPixels, stringFormat);
				}
			}
		}

		internal bool IsRectangleVisible(MapGraphics g, RectangleF clipRect, MapPoint minExtent, MapPoint maxExtent)
		{
			MapCore mapCore = this.GetMapCore();
			PointF pointF = mapCore.GeographicToPercents(minExtent).ToPointF();
			PointF pointF2 = mapCore.GeographicToPercents(maxExtent).ToPointF();
			RectangleF relative = new RectangleF(pointF.X, pointF2.Y, pointF2.X - pointF.X, pointF.Y - pointF2.Y);
			relative = g.GetAbsoluteRectangle(relative);
			return clipRect.IntersectsWith(relative);
		}

		internal RectangleF GetTextBounds(MapGraphics g)
		{
			if (this.TextInt == string.Empty)
			{
				return RectangleF.Empty;
			}
			if (!this.cachedTextBounds.IsEmpty)
			{
				return this.cachedTextBounds;
			}
			string text = (this.TextInt.IndexOf("#", StringComparison.Ordinal) == -1) ? this.TextInt : this.GetMapCore().ResolveAllKeywords(this.TextInt, this);
			text = text.Replace("\\n", "\n");
			SizeF sizeF = g.MeasureString(text, this.Font, new SizeF(0f, 0f), StringFormat.GenericTypographic);
			PointF centerPointInContentPixels = this.GetCenterPointInContentPixels(g);
			this.cachedTextBounds = new RectangleF(centerPointInContentPixels.X, centerPointInContentPixels.Y, sizeF.Width, sizeF.Height);
			this.cachedTextBounds.Inflate((float)(sizeF.Width / 2.0), (float)(sizeF.Height / 2.0));
			return this.cachedTextBounds;
		}

		internal GraphicsPath[] GetPaths(MapGraphics g)
		{
			if (this.VisibleInt && this.ShapeData.Points != null)
			{
				if (this.cachedPaths != null)
				{
					return this.cachedPaths;
				}
				ArrayList arrayList = new ArrayList();
				ArrayList arrayList2 = new ArrayList();
				int num = 0;
				int index = 0;
				this.largestPathIndex = 0;
				double geographicResolutionAtEquator = this.Common.MapCore.Viewport.GetGeographicResolutionAtEquator();
				double num2 = geographicResolutionAtEquator * geographicResolutionAtEquator;
				double verticalOffsetFactor = this.GetVerticalOffsetFactor();
				for (int i = 0; i < this.ShapeData.Segments.Length; i++)
				{
					ShapeSegment shapeSegment = this.ShapeData.Segments[i];
					List<Point3D> list = new List<Point3D>();
					MapPoint pointB = default(MapPoint);
					for (int j = 0; j < shapeSegment.Length; j++)
					{
						MapPoint mapPoint = this.ShapeData.Points[num];
						mapPoint = this.OffsetAndScaleGeoPoint(mapPoint, verticalOffsetFactor);
						if (j == 0 || j == shapeSegment.Length - 1 || Utils.GetDistanceSqr(mapPoint, pointB) > num2)
						{
							pointB = mapPoint;
							Point3D point3D = this.GetMapCore().GeographicToPercents(mapPoint);
							PointF absolutePoint = g.GetAbsolutePoint(point3D.ToPointF());
							Point3D item = new Point3D((double)absolutePoint.X, (double)absolutePoint.Y, point3D.Z);
							list.Add(item);
						}
						num++;
					}
					PointF[] array = this.ReducePoints(list.ToArray());
					if (array.Length > 2)
					{
						GraphicsPath graphicsPath = new GraphicsPath();
						graphicsPath.StartFigure();
						graphicsPath.AddPolygon(array);
						graphicsPath.CloseFigure();
						graphicsPath.SetMarkers();
						if (this.ShapeData.MultiPolygonWithHoles || this.ShapeData.Segments[0].PolygonSignedArea > 0.0)
						{
							if (arrayList.Count == 0 || this.ShapeData.Segments[i - 1].Type == SegmentType.StartFigure)
							{
								arrayList.Add(graphicsPath);
								arrayList2.Add(graphicsPath.GetBounds());
								index = arrayList.Count - 1;
								if (this.ShapeData.LargestSegmentIndex == i)
								{
									this.largestPathIndex = arrayList.Count - 1;
								}
							}
							else
							{
								((GraphicsPath)arrayList[index]).AddPath(graphicsPath, false);
								arrayList2[index] = RectangleF.Union((RectangleF)arrayList2[index], graphicsPath.GetBounds());
							}
						}
						else if (this.ShapeData.Segments[i].PolygonSignedArea > 0.0 && arrayList.Count > 0)
						{
							((GraphicsPath)arrayList[index]).AddPath(graphicsPath, false);
							arrayList2[index] = RectangleF.Union((RectangleF)arrayList2[index], graphicsPath.GetBounds());
						}
						else
						{
							arrayList.Add(graphicsPath);
							arrayList2.Add(graphicsPath.GetBounds());
							index = arrayList.Count - 1;
							if (this.ShapeData.LargestSegmentIndex == i)
							{
								this.largestPathIndex = arrayList.Count - 1;
							}
						}
					}
				}
				this.cachedPaths = (GraphicsPath[])arrayList.ToArray(typeof(GraphicsPath));
				this.cachedPathBounds = (RectangleF[])arrayList2.ToArray(typeof(RectangleF));
				return this.cachedPaths;
			}
			return null;
		}

		private MapPoint OffsetAndScaleGeoPoint(MapPoint mapPoint, double verticalOffsetFactor)
		{
			if (this.OffsetInt.Y != 0.0)
			{
				double num = mapPoint.X - this.CentralPoint.X;
				mapPoint.X = this.CentralPoint.X + num * verticalOffsetFactor;
			}
			if (this.ScaleFactor != 1.0)
			{
				double num2 = mapPoint.X - this.CentralPoint.X;
				double num3 = mapPoint.Y - this.CentralPoint.Y;
				mapPoint.X = this.CentralPoint.X + num2 * this.ScaleFactor;
				mapPoint.Y = this.CentralPoint.Y + num3 * this.ScaleFactor;
			}
			mapPoint.X += this.OffsetInt.X;
			mapPoint.Y += this.OffsetInt.Y;
			return mapPoint;
		}

		internal GraphicsPath[] GetGeographicGraphicsPaths()
		{
			ArrayList arrayList = new ArrayList();
			int num = 0;
			for (int i = 0; i < this.ShapeData.Segments.Length; i++)
			{
				PointF[] array = new PointF[this.ShapeData.Segments[i].Length];
				for (int j = 0; j < array.Length; j++)
				{
					MapPoint mapPoint = this.ShapeData.Points[num];
					array[j].X = (float)this.ShapeData.Points[num].X;
					array[j].Y = (float)this.ShapeData.Points[num].Y;
					num++;
				}
				if (array.Length > 2)
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					graphicsPath.StartFigure();
					graphicsPath.AddPolygon(array);
					graphicsPath.CloseFigure();
					graphicsPath.SetMarkers();
					arrayList.Add(graphicsPath);
				}
			}
			return (GraphicsPath[])arrayList.ToArray(typeof(GraphicsPath));
		}

		internal double GetVerticalOffsetFactor()
		{
			MapCore mapCore = this.GetMapCore();
			if (this.OffsetInt.Y != 0.0 && mapCore != null && mapCore.Projection != 0 && mapCore.Projection != Projection.Mercator)
			{
				double y = this.CentralPoint.Y;
				double y2 = y + this.OffsetInt.Y;
				double num = mapCore.MeasureDistance(new MapPoint(0.0, y), new MapPoint(1.0, y));
				double num2 = mapCore.MeasureDistance(new MapPoint(0.0, y2), new MapPoint(1.0, y2));
				if (num2 > 4.94065645841247E-324)
				{
					return num / num2;
				}
				return 0.0;
			}
			return 1.0;
		}

		internal void ArrangeChildSymbols(MapGraphics g)
		{
			PointF centerPointInContentPixels = this.GetCenterPointInContentPixels(g);
			float num = 0f;
			foreach (Symbol symbol3 in this.Symbols)
			{
				num += symbol3.GetWidth() + (float)this.ChildSymbolMargin;
			}
			if (num > 0.0)
			{
				num -= (float)this.ChildSymbolMargin;
			}
			float num2 = 0f;
			foreach (Symbol symbol4 in this.Symbols)
			{
				symbol4.precalculatedCenterPoint.X = (float)(centerPointInContentPixels.X + num2 - num / 2.0 + symbol4.GetWidth() / 2.0);
				num2 += symbol4.Width + (float)this.ChildSymbolMargin;
				symbol4.precalculatedCenterPoint.Y = centerPointInContentPixels.Y;
				symbol4.ResetCachedPaths();
			}
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
				this.cachedPaths = null;
			}
			if (this.cachedPathBounds != null)
			{
				this.cachedPathBounds = null;
			}
			this.cachedTextBounds = RectangleF.Empty;
		}

		private PointF[] ReducePoints(Point3D[] points)
		{
			ArrayList arrayList = new ArrayList();
			bool flag = false;
			for (int i = 0; i < points.Length; i++)
			{
				if (!(points[i].Z < 0.0))
				{
					if (!flag)
					{
						arrayList.Add(points[i].ToPointF());
						flag = true;
					}
					else
					{
						float distanceSqr = Utils.GetDistanceSqr((PointF)arrayList[arrayList.Count - 1], points[i].ToPointF());
						if (distanceSqr > 1.0)
						{
							arrayList.Add(points[i].ToPointF());
						}
					}
				}
			}
			return (PointF[])arrayList.ToArray(typeof(PointF));
		}

		internal Brush GetBackBrush(MapGraphics g, GraphicsPath path, RectangleF pathBounds)
		{
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
				return g.GetGradientBrush(pathBounds, color, color2, gradientType);
			}
			return new SolidBrush(color);
		}

		internal Pen GetPen(Brush backBrush)
		{
			Pen pen = null;
			if (this.BorderWidthInt <= 0)
			{
				pen = new Pen(backBrush);
				pen.Width = 1f;
			}
			else
			{
				pen = new Pen(this.ApplyLayerTransparency(this.BorderColorInt), (float)this.BorderWidthInt);
				pen.DashStyle = MapGraphics.GetPenStyle(this.BorderStyleInt);
			}
			pen.Alignment = PenAlignment.Center;
			pen.LineJoin = LineJoin.Round;
			return pen;
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
			if (this.ParentGroupObject != null)
			{
				return ((IContentElement)this.ParentGroupObject).IsVisible(g, layer, allLayers, clipRect);
			}
			if (allLayers)
			{
				if (!((ILayerElement)this).BelongsToAllLayers)
				{
					return false;
				}
				goto IL_0064;
			}
			if (layer != null)
			{
				if (((ILayerElement)this).BelongsToLayer && !(layer.Name != ((ILayerElement)this).Layer))
				{
					goto IL_0064;
				}
				return false;
			}
			if (!((ILayerElement)this).BelongsToAllLayers && !((ILayerElement)this).BelongsToLayer)
			{
				goto IL_0064;
			}
			return false;
			IL_0064:
			if (this.ShapeData.Points != null && this.ShapeData.Segments.Length != 0)
			{
				if (this.Selected)
				{
					return true;
				}
				RectangleF geographicClipRectangle = this.Common.MapCore.GetGeographicClipRectangle(clipRect);
				bool flag = false;
				foreach (RectangleF geographicRectangle in this.GetGeographicRectangles())
				{
					if (geographicClipRectangle.IntersectsWith(geographicRectangle))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
				GraphicsPath[] paths = this.GetPaths(g);
				if (paths == null)
				{
					return false;
				}
				for (int i = 0; i < paths.Length; i++)
				{
					RectangleF rect = this.cachedPathBounds[i];
					if (clipRect.IntersectsWith(rect))
					{
						return true;
					}
				}
				if (this.TextInt != string.Empty && clipRect.IntersectsWith(this.GetTextBounds(g)))
				{
					return true;
				}
				return false;
			}
			return false;
		}

		void IContentElement.RenderShadow(MapGraphics g)
		{
			if (this.VisibleInt && this.ShadowOffsetInt != 0)
			{
				GraphicsPath[] paths = this.GetPaths(g);
				for (int i = 0; i < paths.Length; i++)
				{
					if (paths[i] != null)
					{
						using (Brush brush = g.GetShadowBrush())
						{
							Matrix matrix = new Matrix();
							int shadowOffsetInt = this.ShadowOffsetInt;
							matrix.Translate((float)shadowOffsetInt, (float)shadowOffsetInt, MatrixOrder.Append);
							paths[i].Transform(matrix);
							g.FillPath(brush, paths[i]);
							matrix.Reset();
							matrix.Translate((float)(-shadowOffsetInt), (float)(-shadowOffsetInt), MatrixOrder.Append);
							paths[i].Transform(matrix);
						}
					}
				}
			}
		}

		void IContentElement.RenderBack(MapGraphics g, HotRegionList hotRegions)
		{
			if (this.VisibleInt)
			{
				g.StartHotRegion(this);
				try
				{
					GraphicsPath[] paths = this.GetPaths(g);
					for (int i = 0; i < paths.Length; i++)
					{
						if (paths[i] != null)
						{
							using (Brush brush = this.GetBackBrush(g, paths[i], this.cachedPathBounds[i]))
							{
								g.FillPath(brush, paths[i]);
								using (Pen pen = this.GetPen(brush))
								{
									if (pen != null)
									{
										g.DrawPath(pen, paths[i]);
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
		}

		void IContentElement.RenderFront(MapGraphics g, HotRegionList hotRegions)
		{
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
				return this.Common.MapCore.ResolveAllKeywords(this.HrefInt, this);
			}
			return this.HrefInt;
		}

		string IImageMapProvider.GetMapAreaAttributes()
		{
			if (this.Common != null && this.Common.MapCore != null)
			{
				return this.Common.MapCore.ResolveAllKeywords(this.MapAreaAttributesInt, this);
			}
			return this.MapAreaAttributesInt;
		}

		void ISelectable.DrawSelection(MapGraphics g, RectangleF clipRect, bool designTimeSelection)
		{
			MapCore mapCore = this.GetMapCore();
			RectangleF selectionRectangle = ((ISelectable)this).GetSelectionRectangle(g, clipRect);
			RectangleF rect = selectionRectangle;
			rect.Inflate(6f, 6f);
			if (clipRect.IntersectsWith(rect) && !selectionRectangle.IsEmpty)
			{
				g.DrawSelection(selectionRectangle, designTimeSelection, mapCore.SelectionBorderColor, mapCore.SelectionMarkerColor);
				if (mapCore.IsDesignMode())
				{
					PointF centerPointInContentPixels = this.GetCenterPointInContentPixels(g);
					AntiAliasing antiAliasing = g.AntiAliasing;
					g.AntiAliasing = AntiAliasing.None;
					g.DrawLine(Pens.Red, (float)(centerPointInContentPixels.X - 8.0), centerPointInContentPixels.Y, (float)(centerPointInContentPixels.X + 8.0), centerPointInContentPixels.Y);
					g.DrawLine(Pens.Red, centerPointInContentPixels.X, (float)(centerPointInContentPixels.Y - 8.0), centerPointInContentPixels.X, (float)(centerPointInContentPixels.Y + 8.0));
					g.AntiAliasing = antiAliasing;
				}
			}
		}

		RectangleF ISelectable.GetSelectionRectangle(MapGraphics g, RectangleF clipRect)
		{
			RectangleF rectangleF = RectangleF.Empty;
			GraphicsPath[] paths = this.GetPaths(g);
			if (paths != null && paths.Length != 0)
			{
				RectangleF rectangleF2 = this.cachedPathBounds[this.largestPathIndex];
				float width = rectangleF2.Width;
				float height = rectangleF2.Height;
				for (int i = 0; i < paths.Length; i++)
				{
					RectangleF rectangleF3 = this.cachedPathBounds[i];
					RectangleF rect = rectangleF3;
					rect.Inflate(6f, 6f);
					if (clipRect.IntersectsWith(rect))
					{
						rectangleF = ((!rectangleF.IsEmpty) ? RectangleF.Union(rectangleF, rectangleF3) : rectangleF3);
					}
				}
				return rectangleF;
			}
			return RectangleF.Empty;
		}

		bool ISelectable.IsSelected()
		{
			return this.Selected;
		}

		bool ISelectable.IsVisible()
		{
			return this.VisibleInt;
		}
	}
}
