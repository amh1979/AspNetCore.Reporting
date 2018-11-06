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
	[TypeConverter(typeof(PathConverter))]
	internal class Path : NamedElement, IContentElement, ILayerElement, IToolTipProvider, ISelectable, ISpatialElement, IImageMapProvider
	{
		private GraphicsPath[] cachedPaths;

		private RectangleF[] cachedPathBounds;

		private RectangleF cachedUnionRectangle = RectangleF.Empty;

		private GraphicsPath[] cachedLabelPaths;

		private double[] cachedSegmentLengths;

		internal int largestPathIndex;

		internal Hashtable fields;

		private string fieldDataBuffer = string.Empty;

		private PathData pathData;

		private Offset offset;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private bool visible = true;

		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private Color borderColor = Color.DarkGray;

		private MapDashStyle lineStyle = MapDashStyle.Solid;

		private int borderWidth = 1;

		private float width = 5f;

		private Color color = Color.LightSalmon;

		private Color textColor = Color.Black;

		private GradientType gradientType;

		private Color secondaryColor = Color.Empty;

		private MapHatchStyle hatchStyle;

		private string text = "#NAME";

		private int shadowOffset;

		private int textShadowOffset;

		private bool selected;

		private string category = string.Empty;

		private string parentGroup = "(none)";

		private PathLabelPosition labelPosition;

		private Group parentGroupObject;

		private bool useInternalProperties;

		private Color borderColorInt = Color.DarkGray;

		private Color colorInt = Color.LightSalmon;

		private GradientType gradientTypeInt;

		private Color secondaryColorInt = Color.Empty;

		private MapHatchStyle hatchStyleInt;

		private float widthInt = 5f;

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
				return this.PathData.MinimumExtent;
			}
		}

		MapPoint ISpatialElement.MaximumExtent
		{
			get
			{
				return this.PathData.MaximumExtent;
			}
		}

		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributePath_PathData")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PathData PathData
		{
			get
			{
				return this.pathData;
			}
			set
			{
				this.pathData = value;
				this.ResetCachedPaths();
				this.InvalidateCachedBounds();
				this.InvalidateViewport();
			}
		}

		MapPoint[] ISpatialElement.Points
		{
			get
			{
				return this.PathData.Points;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public string EncodedPathData
		{
			get
			{
				return PathData.PathDataToString(this.PathData);
			}
			set
			{
				this.pathData = PathData.PathDataFromString(value);
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

		[SRDescription("DescriptionAttributePath_Offset")]
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

		[SRDescription("DescriptionAttributePath_ToolTip")]
		[TypeConverter(typeof(KeywordConverter))]
		[Browsable(false)]
		[Localizable(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue("")]
		[EditorBrowsable(EditorBrowsableState.Never)]
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
		[SRDescription("DescriptionAttributePath_Href")]
		[Localizable(true)]
		[SRCategory("CategoryAttribute_Behavior")]
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
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributePath_MapAreaAttributes")]
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

		[SRDescription("DescriptionAttributePath_Name")]
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

		[ParenthesizePropertyName(true)]
		[SRDescription("DescriptionAttributePath_Visible")]
		[SRCategory("CategoryAttribute_Appearance")]
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

		[SRDescription("DescriptionAttributePath_Font")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8.25pt")]
		[SRCategory("CategoryAttribute_Appearance")]
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
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributePath_BorderColor")]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(MapDashStyle.Solid)]
		[SRDescription("DescriptionAttributePath_LineStyle")]
		[NotifyParentProperty(true)]
		public MapDashStyle LineStyle
		{
			get
			{
				return this.lineStyle;
			}
			set
			{
				this.lineStyle = value;
				this.InvalidateViewport();
			}
		}

		[DefaultValue(1)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePath_BorderWidth")]
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

		[DefaultValue(5f)]
		[SRDescription("DescriptionAttributePath_Width")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		public float Width
		{
			get
			{
				return this.width;
			}
			set
			{
				if (!(value < 0.0) && !(value > 100.0))
				{
					this.width = value;
					this.InvalidateViewport();
					return;
				}
				throw new ArgumentException(SR.must_in_range(0.0, 100.0));
			}
		}

		[SRDescription("DescriptionAttributePath_Color")]
		[DefaultValue(typeof(Color), "LightSalmon")]
		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
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

		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributePath_TextColor")]
		[NotifyParentProperty(true)]
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

		[SRDescription("DescriptionAttributePath_GradientType")]
		//[Editor(typeof(GradientEditor), typeof(UITypeEditor))]
		[NotifyParentProperty(true)]
		[DefaultValue(GradientType.None)]
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

		[SRDescription("DescriptionAttributePath_SecondaryColor")]
		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "")]
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

		//[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		[NotifyParentProperty(true)]
		[DefaultValue(MapHatchStyle.None)]
		[SRDescription("DescriptionAttributePath_HatchStyle")]
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

		[SRCategory("CategoryAttribute_Behavior")]
		[TypeConverter(typeof(KeywordConverter))]
		[DefaultValue("#NAME")]
		[SRDescription("DescriptionAttributePath_Text")]
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
				this.ResetCachedPaths();
				this.InvalidateViewport();
			}
		}

		[DefaultValue(0)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePath_ShadowOffset")]
		[NotifyParentProperty(true)]
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

		[NotifyParentProperty(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributePath_TextShadowOffset")]
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

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributePath_Selected")]
		[DefaultValue(false)]
		[Browsable(false)]
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

		[SRDescription("DescriptionAttributePath_Category")]
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

		[DefaultValue("(none)")]
		[TypeConverter(typeof(DesignTimeGroupConverter))]
		[SRDescription("DescriptionAttributePath_ParentGroup")]
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
				this.InvalidateViewport();
			}
		}

		[SRDescription("DescriptionAttributePath_LabelPosition")]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(PathLabelPosition.Above)]
		[NotifyParentProperty(true)]
		public PathLabelPosition LabelPosition
		{
			get
			{
				return this.labelPosition;
			}
			set
			{
				this.labelPosition = value;
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
						Field field = (Field)mapCore.PathFields.GetByName(name);
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
				if (this.color == Color.LightSalmon)
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

		internal float WidthInt
		{
			get
			{
				if (this.width == 5.0 && this.useInternalProperties)
				{
					return this.widthInt;
				}
				return this.width;
			}
			set
			{
				this.widthInt = value;
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

		[SRDescription("DescriptionAttributePath_Layer")]
		[DefaultValue("(none)")]
		[SRCategory("CategoryAttribute_Behavior")]
		[TypeConverter(typeof(DesignTimeLayerConverter))]
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

		public Path()
			: this(null)
		{
		}

		internal Path(CommonElements common)
			: base(common)
		{
			this.pathData = new PathData();
			this.offset = new Offset(this, 0.0, 0.0);
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

		public override string ToString()
		{
			return this.Name;
		}

		public void AddSegments(MapPoint[] points, PathSegment[] segments)
		{
			if (this.PathData.Points == null || this.PathData.Points.Length == 0)
			{
				this.PathData.Points = points;
			}
			else
			{
				MapPoint[] array = new MapPoint[this.PathData.Points.Length + points.Length];
				Array.Copy(this.PathData.Points, array, this.PathData.Points.Length);
				Array.Copy(points, 0, array, this.PathData.Points.Length, points.Length);
				this.PathData.Points = array;
			}
			if (this.PathData.Segments == null || this.PathData.Segments.Length == 0)
			{
				this.PathData.Segments = segments;
			}
			else
			{
				PathSegment[] array2 = new PathSegment[this.PathData.Segments.Length + segments.Length];
				Array.Copy(this.PathData.Segments, array2, this.PathData.Segments.Length);
				Array.Copy(segments, 0, array2, this.PathData.Segments.Length, segments.Length);
				this.PathData.Segments = array2;
			}
			this.PathData.UpdateStoredParameters();
			this.InvalidateCachedBounds();
			this.InvalidateRules();
			this.InvalidateViewport();
		}

		public void ClearPathData()
		{
			this.PathData.Segments = null;
			this.PathData.Points = null;
			this.PathData.UpdateStoredParameters();
			this.InvalidateCachedBounds();
			this.InvalidateRules();
			this.InvalidateViewport();
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
				foreach (Field pathField in mapCore.PathFields)
				{
					if (!pathField.IsTemporary)
					{
						string text2 = pathField.FormatValue(this.fields[pathField.Name]);
						if (!string.IsNullOrEmpty(text2))
						{
							string text3 = text;
							text = text3 + XmlConvert.EncodeName(pathField.Name) + "=" + text2 + "&";
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
						Field field = (Field)mapCore.PathFields.GetByName(name);
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
				mapCore.InvalidateRules();
				mapCore.InvalidateDataBinding();
			}
		}

		protected override void OnDispose()
		{
			this.ResetCachedPaths();
			this.PathData.Points = null;
			this.PathData.Segments = null;
			if (this.fields != null)
			{
				this.fields.Clear();
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

		internal void ApplyCustomWidthAttributes(CustomWidth customWidth)
		{
			this.UseInternalProperties = true;
			this.WidthInt = customWidth.Width;
			this.TextInt = customWidth.Text;
			this.ToolTipInt = customWidth.ToolTip;
		}

		internal int GetLongestVisibleSegmentIndex(MapGraphics g)
		{
			int num = -1;
			double num2 = double.NegativeInfinity;
			MapCore mapCore = this.GetMapCore();
			PointF location = mapCore.PixelsToContent(mapCore.Viewport.GetAbsoluteLocation());
			SizeF sizeInPixels = mapCore.Viewport.GetSizeInPixels();
			RectangleF rectangleF = new RectangleF(location, sizeInPixels);
			GraphicsPath[] paths = this.GetPaths(g);
			for (int i = 0; i < paths.Length; i++)
			{
				if (rectangleF.Contains(this.cachedPathBounds[i]) && this.cachedSegmentLengths[i] > num2)
				{
					num2 = this.cachedSegmentLengths[i];
					num = i;
				}
			}
			if (num == -1)
			{
				num2 = double.NegativeInfinity;
				for (int j = 0; j < paths.Length; j++)
				{
					if (rectangleF.IntersectsWith(this.cachedPathBounds[j]) && this.cachedSegmentLengths[j] > num2)
					{
						num2 = this.cachedSegmentLengths[j];
						num = j;
					}
				}
			}
			return num;
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
				this.AddSegments((MapPoint[])arrayList.ToArray(typeof(MapPoint)), (PathSegment[])arrayList2.ToArray(typeof(PathSegment)));
				return true;
			}
			return false;
		}

		private void AddGeometryRec(SqlGeometry geometry, ArrayList pointsList, ArrayList segmentsList)
		{
			string value = geometry.STGeometryType().Value;
			if (value == "GeometryCollection")
			{
				for (int i = 1; i <= geometry.STNumGeometries().Value; i++)
				{
					this.AddGeometryRec(geometry.STGeometryN(i), pointsList, segmentsList);
				}
			}
			else
			{
				if (!(value == "MultiLineString") && !(value == "LineString"))
				{
					return;
				}
				this.AddSimpleGeometry(geometry, pointsList, segmentsList);
			}
		}

		private void AddSimpleGeometry(SqlGeometry geometry, ArrayList pointsList, ArrayList segmentsList)
		{
			if (!geometry.STIsEmpty())
			{
				PathSegment[] array = new PathSegment[geometry.STNumGeometries().Value];
				for (int i = 1; (SqlInt32)i <= geometry.STNumGeometries(); i++)
				{
					array[i - 1].Type = SegmentType.PolyLine;
					array[i - 1].Length = geometry.STGeometryN(i).STNumPoints().Value;
				}
				MapPoint[] array2 = new MapPoint[geometry.STNumPoints().Value];
				for (int j = 1; (SqlInt32)j <= geometry.STNumPoints(); j++)
				{
					array2[j - 1].X = geometry.STPointN(j).STX.Value;
					array2[j - 1].Y = geometry.STPointN(j).STY.Value;
				}
				pointsList.AddRange(array2);
				segmentsList.AddRange(array);
			}
		}

		public bool AddGeography(SqlGeography geography)
		{
			geography = geography.MakeValid();
			if (geography.STIsEmpty())
			{
				return false;
			}
			geography = this.GetMapCore().NormalizeLongitude(geography);
			geography = geography.STIntersection(this.GetMapCore().GetClippingPolygon(geography.STSrid.Value));
			geography = geography.STCurveToLine();
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			this.AddGeographyRec(geography, arrayList, arrayList2);
			if (arrayList.Count > 0)
			{
				this.AddSegments((MapPoint[])arrayList.ToArray(typeof(MapPoint)), (PathSegment[])arrayList2.ToArray(typeof(PathSegment)));
				return true;
			}
			return false;
		}

		private void AddGeographyRec(SqlGeography geography, ArrayList pointsList, ArrayList segmentsList)
		{
			string value = geography.STGeometryType().Value;
			if (value == "GeometryCollection")
			{
				for (int i = 1; i <= geography.STNumGeometries().Value; i++)
				{
					this.AddGeographyRec(geography.STGeometryN(i), pointsList, segmentsList);
				}
			}
			else
			{
				if (!(value == "MultiLineString") && !(value == "LineString"))
				{
					return;
				}
				this.AddSimpleGeography(geography, pointsList, segmentsList);
			}
		}

		internal void AddSimpleGeography(SqlGeography geography, ArrayList pointsList, ArrayList segmentsList)
		{
			if (!geography.STIsEmpty())
			{
				PathSegment[] array = new PathSegment[geography.STNumGeometries().Value];
				for (int i = 1; (SqlInt32)i <= geography.STNumGeometries(); i++)
				{
					array[i - 1].Type = SegmentType.PolyLine;
					array[i - 1].Length = geography.STGeometryN(i).STNumPoints().Value;
				}
				MapPoint[] array2 = new MapPoint[geography.STNumPoints().Value];
				for (int j = 1; (SqlInt32)j <= geography.STNumPoints(); j++)
				{
					array2[j - 1].X = geography.STPointN(j).Long.Value;
					array2[j - 1].Y = geography.STPointN(j).Lat.Value;
				}
				GeoUtils.CutPaths(ref array2, ref array);
				pointsList.AddRange(array2);
				segmentsList.AddRange(array);
			}
		}
        */
		public bool LoadWKT(string wkt)
		{
			this.ClearPathData();
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
			this.ClearPathData();
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
			if (this.PathData != null && this.PathData.Segments != null && this.PathData.Segments.Length != 0 && this.PathData.Points != null && this.PathData.Points.Length != 0)
			{
				MapPoint[] points = this.PathData.Points;
				PathSegment[] segments = this.PathData.Segments;
				int num = 0;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("MULTILINESTRING(");
				for (int i = 0; i < segments.Length; i++)
				{
					stringBuilder.Append("(");
					for (int j = num; j < num + segments[i].Length; j++)
					{
						stringBuilder.Append(points[j].X.ToString(CultureInfo.InvariantCulture) + " " + points[j].Y.ToString(CultureInfo.InvariantCulture));
						if (j < num + segments[i].Length - 1)
						{
							stringBuilder.Append(", ");
						}
					}
					stringBuilder.Append(")");
					if (i < segments.Length - 1)
					{
						stringBuilder.Append(", ");
					}
					num += segments[i].Length;
				}
				stringBuilder.Append(")");
				return stringBuilder.ToString();
			}
			return string.Empty;
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
			binaryWriter.Write(value);
			binaryWriter.Write(5u);
			if (this.PathData == null || this.PathData.Segments == null || this.PathData.Segments.Length == 0 || this.PathData.Points == null || this.PathData.Points.Length == 0)
			{
				binaryWriter.Write(0u);
			}
			else
			{
				binaryWriter.Write((uint)this.PathData.Segments.Length);
				MapPoint[] points = this.PathData.Points;
				PathSegment[] segments = this.PathData.Segments;
				int num = 0;
				for (int i = 0; i < segments.Length; i++)
				{
					binaryWriter.Write(value);
					binaryWriter.Write(2u);
					binaryWriter.Write((uint)segments[i].Length);
					for (int j = num; j < num + segments[i].Length; j++)
					{
						binaryWriter.Write(points[j].X);
						binaryWriter.Write(points[j].Y);
					}
					num += segments[i].Length;
				}
			}
		}

		private void RenderText(MapGraphics g)
		{
			if (!string.IsNullOrEmpty(this.TextInt) && this.cachedPaths.Length != 0)
			{
				string text = (this.TextInt.IndexOf("#", StringComparison.Ordinal) == -1) ? this.TextInt : this.GetMapCore().ResolveAllKeywords(this.TextInt, this);
				int labelOffset = 0;
				if (this.LabelPosition == PathLabelPosition.Above)
				{
					labelOffset = (int)Math.Round(this.WidthInt * 2.0 + 10.0);
				}
				else if (this.LabelPosition == PathLabelPosition.Below)
				{
					labelOffset = -(int)Math.Round(this.WidthInt * 2.0 + 10.0);
				}
				text = text.Replace("\\n", "\n");
				text = "   " + text;
				using (Brush brush2 = new SolidBrush(this.TextColor))
				{
					int longestVisibleSegmentIndex = this.GetLongestVisibleSegmentIndex(g);
					if (longestVisibleSegmentIndex != -1)
					{
						GraphicsPath graphicsPath = this.cachedLabelPaths[longestVisibleSegmentIndex];
						if (graphicsPath == null)
						{
							PointF[] pathPoints = this.cachedPaths[longestVisibleSegmentIndex].PathPoints;
							BendingText bendingText = new BendingText();
							if (pathPoints.Length > 1)
							{
								graphicsPath = bendingText.CreatePath(this.Font, pathPoints, text, 0, labelOffset);
								this.cachedLabelPaths[longestVisibleSegmentIndex] = graphicsPath;
							}
						}
						if (graphicsPath != null)
						{
							if (this.TextShadowOffset != 0)
							{
								using (Brush brush = g.GetShadowBrush())
								{
									Matrix matrix = new Matrix();
									matrix.Translate((float)this.TextShadowOffset, (float)this.TextShadowOffset, MatrixOrder.Append);
									graphicsPath.Transform(matrix);
									g.FillPath(brush, graphicsPath);
									matrix.Reset();
									matrix.Translate((float)(-this.TextShadowOffset), (float)(-this.TextShadowOffset), MatrixOrder.Append);
									graphicsPath.Transform(matrix);
								}
							}
							g.FillPath(brush2, graphicsPath);
						}
					}
				}
			}
		}

		internal bool IsRectangleVisible(MapGraphics g, RectangleF clipRect, MapPoint minExtent, MapPoint maxExtent)
		{
			MapCore mapCore = this.GetMapCore();
			Point3D point3D = mapCore.GeographicToPercents(minExtent);
			Point3D point3D2 = mapCore.GeographicToPercents(maxExtent);
			RectangleF relative = new RectangleF((float)point3D.X, (float)point3D2.Y, (float)(point3D2.X - point3D.X), (float)(point3D.Y - point3D2.Y));
			relative = g.GetAbsoluteRectangle(relative);
			return clipRect.IntersectsWith(relative);
		}

		internal GraphicsPath[] GetPaths(MapGraphics g)
		{
			if (this.Visible && this.PathData.Points != null)
			{
				if (this.cachedPaths != null)
				{
					return this.cachedPaths;
				}
				ArrayList arrayList = new ArrayList();
				ArrayList arrayList2 = new ArrayList();
				ArrayList arrayList3 = new ArrayList();
				int num = 0;
				this.largestPathIndex = 0;
				double geographicResolutionAtEquator = this.Common.MapCore.Viewport.GetGeographicResolutionAtEquator();
				double num2 = geographicResolutionAtEquator * geographicResolutionAtEquator;
				for (int i = 0; i < this.PathData.Segments.Length; i++)
				{
					PathSegment pathSegment = this.PathData.Segments[i];
					List<Point3D> list = new List<Point3D>();
					MapPoint pointB = default(MapPoint);
					for (int j = 0; j < pathSegment.Length; j++)
					{
						MapPoint mapPoint = this.PathData.Points[num];
						if (j == 0 || j == pathSegment.Length - 1 || Utils.GetDistanceSqr(mapPoint, pointB) > num2)
						{
							pointB = mapPoint;
							mapPoint.X += this.OffsetInt.X;
							mapPoint.Y += this.OffsetInt.Y;
							Point3D point3D = this.GetMapCore().GeographicToPercents(mapPoint);
							PointF absolutePoint = g.GetAbsolutePoint(point3D.ToPointF());
							Point3D item = new Point3D((double)absolutePoint.X, (double)absolutePoint.Y, point3D.Z);
							list.Add(item);
						}
						num++;
					}
					PointF[] array = this.ReducePoints(list.ToArray());
					if (array.Length > 1)
					{
						GraphicsPath graphicsPath = new GraphicsPath();
						graphicsPath.AddLines(array);
						arrayList.Add(graphicsPath);
						RectangleF bounds = graphicsPath.GetBounds();
						arrayList2.Add(bounds);
						arrayList3.Add(this.PathData.Segments[i].SegmentLength);
						if (this.cachedUnionRectangle.IsEmpty)
						{
							this.cachedUnionRectangle = bounds;
						}
						else
						{
							this.cachedUnionRectangle = RectangleF.Union(this.cachedUnionRectangle, bounds);
						}
					}
				}
				this.cachedUnionRectangle.Inflate((float)(this.WidthInt / 2.0), (float)(this.WidthInt / 2.0));
				this.cachedPaths = (GraphicsPath[])arrayList.ToArray(typeof(GraphicsPath));
				this.cachedPathBounds = (RectangleF[])arrayList2.ToArray(typeof(RectangleF));
				this.cachedLabelPaths = new GraphicsPath[this.cachedPaths.Length];
				this.cachedSegmentLengths = (double[])arrayList3.ToArray(typeof(double));
				return this.cachedPaths;
			}
			return null;
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
			this.cachedUnionRectangle = RectangleF.Empty;
			if (this.cachedLabelPaths != null)
			{
				GraphicsPath[] array2 = this.cachedLabelPaths;
				foreach (GraphicsPath graphicsPath2 in array2)
				{
					if (graphicsPath2 != null)
					{
						graphicsPath2.Dispose();
					}
				}
				this.cachedLabelPaths = null;
			}
			if (this.cachedSegmentLengths != null)
			{
				this.cachedSegmentLengths = null;
			}
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
						float distance = this.GetDistance((PointF)arrayList[arrayList.Count - 1], points[i].ToPointF());
						if (distance > 1.0)
						{
							arrayList.Add(points[i].ToPointF());
						}
					}
				}
			}
			return (PointF[])arrayList.ToArray(typeof(PointF));
		}

		private float GetDistance(PointF pointA, PointF pointB)
		{
			double num = (double)Math.Abs(pointA.X - pointB.X);
			double num2 = (double)Math.Abs(pointA.Y - pointB.Y);
			return (float)Math.Sqrt(num * num + num2 * num2);
		}

		internal static Brush GetBackBrush(MapGraphics g, GraphicsPath path, RectangleF pathBounds, Color fillColor, Color secondaryColor, GradientType gradientType, MapHatchStyle hatchStyle)
		{
			Brush brush = null;
			if (hatchStyle != 0)
			{
				return MapGraphics.GetHatchBrush(hatchStyle, fillColor, secondaryColor);
			}
			if (gradientType != 0)
			{
				return g.GetGradientBrush(pathBounds, fillColor, secondaryColor, gradientType);
			}
			return new SolidBrush(fillColor);
		}

		internal Pen GetBorderPen()
		{
			Pen pen = new Pen(this.ApplyLayerTransparency(this.BorderColorInt), this.WidthInt + (float)(this.BorderWidthInt * 2));
			pen.Alignment = PenAlignment.Center;
			pen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
			pen.LineJoin = LineJoin.Round;
			return pen;
		}

		internal static Pen GetColorPen(Color color, float width, float borderWidth)
		{
			if (width + borderWidth * 2.0 == 0.0)
			{
				return null;
			}
			Pen pen = new Pen(color, (float)(width + borderWidth * 2.0));
			pen.Alignment = PenAlignment.Center;
			pen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
			pen.LineJoin = LineJoin.Round;
			return pen;
		}

		internal static Pen GetFillPen(MapGraphics g, GraphicsPath path, RectangleF pathBounds, float width, MapDashStyle lineStyle, Color fillColor, Color secondaryColor, GradientType gradientType, MapHatchStyle hatchStyle)
		{
			if (width == 0.0)
			{
				return null;
			}
			Brush backBrush = Path.GetBackBrush(g, path, pathBounds, fillColor, secondaryColor, gradientType, hatchStyle);
			Pen pen = new Pen(backBrush);
			pen.Width = width;
			pen.DashStyle = MapGraphics.GetPenStyle(lineStyle);
			pen.Alignment = PenAlignment.Center;
			pen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
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
			if (!this.Visible)
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
			if (this.PathData.Points != null && this.PathData.Segments.Length != 0)
			{
				GraphicsPath[] paths = this.GetPaths(g);
				if (paths == null)
				{
					return false;
				}
				for (int i = 0; i < paths.Length; i++)
				{
					RectangleF rect = this.cachedPathBounds[i];
					rect.Inflate((float)(this.WidthInt / 2.0 + (float)this.BorderWidthInt), (float)(this.WidthInt / 2.0 + (float)this.BorderWidthInt));
					if (clipRect.IntersectsWith(rect))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		void IContentElement.RenderShadow(MapGraphics g)
		{
			if (this.Visible && this.ShadowOffset != 0)
			{
				if (this.WidthInt == 0.0 && this.BorderWidthInt == 0)
				{
					return;
				}
				GraphicsPath[] paths = this.GetPaths(g);
				GraphicsPath[] array = paths;
				foreach (GraphicsPath graphicsPath in array)
				{
					if (graphicsPath != null)
					{
						using (Pen pen = Path.GetColorPen(g.GetShadowColor(), this.WidthInt, (float)this.BorderWidthInt))
						{
							if (pen != null)
							{
								Matrix matrix = new Matrix();
								int shadowOffsetInt = this.ShadowOffsetInt;
								matrix.Translate((float)shadowOffsetInt, (float)shadowOffsetInt, MatrixOrder.Append);
								graphicsPath.Transform(matrix);
								g.DrawPath(pen, graphicsPath);
								matrix.Reset();
								matrix.Translate((float)(-shadowOffsetInt), (float)(-shadowOffsetInt), MatrixOrder.Append);
								graphicsPath.Transform(matrix);
							}
						}
					}
				}
			}
		}

		void IContentElement.RenderBack(MapGraphics g, HotRegionList hotRegions)
		{
			if (this.Visible && !(this.BorderColorInt == Color.Empty) && this.BorderWidthInt >= 1 && this.WidthInt != 0.0)
			{
				g.StartHotRegion(this);
				Brush brush = null;
				Brush brush2 = null;
				Pen pen = null;
				try
				{
					GraphicsPath[] paths = this.GetPaths(g);
					GraphicsPath[] array = paths;
					foreach (GraphicsPath graphicsPath in array)
					{
						if (graphicsPath != null)
						{
							pen = this.GetBorderPen();
							if (pen != null && this.BorderWidthInt != 0 && this.BorderColorInt.A != 0)
							{
								g.DrawPath(pen, graphicsPath);
							}
						}
					}
				}
				finally
				{
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

		void IContentElement.RenderFront(MapGraphics g, HotRegionList hotRegions)
		{
			if (this.Visible && this.WidthInt != 0.0)
			{
				g.StartHotRegion(this);
				try
				{
					GraphicsPath[] paths = this.GetPaths(g);
					GraphicsPath[] array = paths;
					foreach (GraphicsPath graphicsPath in array)
					{
						if (graphicsPath != null)
						{
							using (Pen pen = Path.GetFillPen(g, graphicsPath, this.cachedUnionRectangle, this.WidthInt, this.LineStyle, this.ApplyLayerTransparency(this.ColorInt), this.ApplyLayerTransparency(this.SecondaryColorInt), this.GradientTypeInt, this.HatchStyleInt))
							{
								if (pen != null)
								{
									g.DrawPath(pen, graphicsPath);
									if (pen.Brush != null)
									{
										pen.Brush.Dispose();
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
			RectangleF selectionRectangle = ((ISelectable)this).GetSelectionRectangle(g, clipRect);
			RectangleF rect = selectionRectangle;
			rect.Inflate(6f, 6f);
			if (clipRect.IntersectsWith(rect) && !selectionRectangle.IsEmpty)
			{
				g.DrawSelection(selectionRectangle, designTimeSelection, mapCore.SelectionBorderColor, mapCore.SelectionMarkerColor);
			}
		}

		RectangleF ISelectable.GetSelectionRectangle(MapGraphics g, RectangleF clipRect)
		{
			RectangleF rectangleF = RectangleF.Empty;
			GraphicsPath[] paths = this.GetPaths(g);
			if (paths != null && paths.Length != 0)
			{
				RectangleF rectangleF2 = this.cachedPathBounds[this.largestPathIndex];
				double num = (double)(rectangleF2.Width * rectangleF2.Height);
				for (int i = 0; i < paths.Length; i++)
				{
					RectangleF rectangleF3 = this.cachedPathBounds[i];
					double num2 = (double)(rectangleF3.Width * rectangleF3.Height);
					if (!(num2 < num / 20.0))
					{
						RectangleF rect = rectangleF3;
						rect.Inflate(6f, 6f);
						if (clipRect.IntersectsWith(rect))
						{
							rectangleF = ((!rectangleF.IsEmpty) ? RectangleF.Union(rectangleF, rectangleF3) : rectangleF3);
						}
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
			return this.Visible;
		}
	}
}
