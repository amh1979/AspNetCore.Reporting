using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Xml;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(GroupConverter))]
	internal class Group : NamedElement, IContentElement, ILayerElement, ISelectable, IToolTipProvider
	{
		internal Hashtable fields;

		private string fieldDataBuffer = string.Empty;

		private Offset offset;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private ContentAlignment textAlignment = ContentAlignment.MiddleCenter;

		private bool visible = true;

		private Font font = new Font("Microsoft Sans Serif", 10f);

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

		private ArrayList shapes;

		private bool useInternalProperties;

		private Color borderColorInt = Color.DarkGray;

		private Color colorInt = Color.Empty;

		private GradientType gradientTypeInt;

		private Color secondaryColorInt = Color.Empty;

		private MapHatchStyle hatchStyleInt;

		private string textInt = "#NAME";

		private string toolTipInt = "";

		private string layer = "(none)";

		private bool belongsToLayer;

		private bool belongsToAllLayers;

		private Layer layerObject;

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

		[TypeConverter(typeof(ShapeOffsetConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeGroup_Offset")]
		[SRCategory("CategoryAttribute_Behavior")]
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

		[SRCategory("CategoryAttribute_Behavior")]
		[Localizable(true)]
		[TypeConverter(typeof(KeywordConverter))]
		[DefaultValue("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeGroup_ToolTip")]
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

		[SRDescription("DescriptionAttributeGroup_Href")]
		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Behavior")]
		[Localizable(true)]
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

		[SRDescription("DescriptionAttributeGroup_MapAreaAttributes")]
		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue("")]
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
		[SRDescription("DescriptionAttributeGroup_Name")]
		public override string Name
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

		[DefaultValue(ContentAlignment.MiddleCenter)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeGroup_TextAlignment")]
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
		[SRDescription("DescriptionAttributeGroup_Visible")]
		[DefaultValue(true)]
		[ParenthesizePropertyName(true)]
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

		[SRDescription("DescriptionAttributeGroup_Font")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 10pt")]
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
		[SRDescription("DescriptionAttributeGroup_BorderColor")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
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

		[SRDescription("DescriptionAttributeGroup_BorderStyle")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(MapDashStyle.Solid)]
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

		[NotifyParentProperty(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeGroup_BorderWidth")]
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

		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeGroup_Color")]
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

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeGroup_TextColor")]
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

		[NotifyParentProperty(true)]
		[DefaultValue(GradientType.None)]
		[SRCategory("CategoryAttribute_Appearance")]
		[Editor(typeof(GradientEditor), typeof(UITypeEditor))]
		[SRDescription("DescriptionAttributeGroup_GradientType")]
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
		[SRDescription("DescriptionAttributeGroup_SecondaryColor")]
		[SRCategory("CategoryAttribute_Appearance")]
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

		[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		[NotifyParentProperty(true)]
		[DefaultValue(MapHatchStyle.None)]
		[SRDescription("DescriptionAttributeGroup_HatchStyle")]
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
		[SRDescription("DescriptionAttributeGroup_Text")]
		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue("#NAME")]
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
				this.InvalidateViewport();
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue(0)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeGroup_ShadowOffset")]
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
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeGroup_TextShadowOffset")]
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
		[SRDescription("DescriptionAttributeGroup_Selected")]
		[DefaultValue(false)]
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

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeGroup_CentralPoint")]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRCategory("CategoryAttribute_Behavior")]
		public MapPoint CentralPoint
		{
			get
			{
				MapPoint result = new MapPoint(0.0, 0.0);
				if (this.Shapes.Count > 0)
				{
					foreach (Shape shape in this.Shapes)
					{
						MapPoint centralPoint = shape.CentralPoint;
						centralPoint.X += shape.OffsetInt.X + shape.CentralPointOffset.X;
						centralPoint.Y += shape.OffsetInt.Y + shape.CentralPointOffset.Y;
						result.X += centralPoint.X;
						result.Y += centralPoint.Y;
					}
					result.X /= (double)this.Shapes.Count;
					result.Y /= (double)this.Shapes.Count;
				}
				return result;
			}
		}

		[TypeConverter(typeof(ShapeOffsetConverter))]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeGroup_CentralPointOffset")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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

		[DefaultValue("")]
		[SRDescription("DescriptionAttributeGroup_Category")]
		[SRCategory("CategoryAttribute_Data")]
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
						Field field = (Field)mapCore.GroupFields.GetByName(name);
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

		internal ArrayList Shapes
		{
			get
			{
				MapCore mapCore = this.GetMapCore();
				if (mapCore != null && this.shapes == null)
				{
					this.shapes = new ArrayList();
					foreach (Shape shape in mapCore.Shapes)
					{
						if (shape.ParentGroup == this.Name)
						{
							this.shapes.Add(shape);
						}
					}
				}
				return this.shapes;
			}
			set
			{
				this.shapes = value;
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

		internal Color ColorInt
		{
			get
			{
				if (this.color.IsEmpty && this.useInternalProperties)
				{
					return this.colorInt;
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

		internal Color SecondaryColorInt
		{
			get
			{
				if (this.secondaryColor.IsEmpty && this.useInternalProperties)
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

		[TypeConverter(typeof(DesignTimeLayerConverter))]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeGroup_Layer")]
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

		public Group()
			: this(null)
		{
		}

		internal Group(CommonElements common)
			: base(common)
		{
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

		public PointF GetCenterPointInContentPixels(MapGraphics g)
		{
			MapCore mapCore = this.GetMapCore();
			if (mapCore == null)
			{
				return PointF.Empty;
			}
			MapPoint centralPoint = this.CentralPoint;
			centralPoint.X += this.CentralPointOffset.X;
			centralPoint.Y += this.CentralPointOffset.Y;
			PointF relative = mapCore.GeographicToPercents(centralPoint).ToPointF();
			return g.GetAbsolutePoint(relative);
		}

		internal MapCore GetMapCore()
		{
			return (MapCore)this.ParentElement;
		}

		internal PointF GetCentralPoint()
		{
			int count = this.Shapes.Count;
			if (count == 0)
			{
				return PointF.Empty;
			}
			MapPoint mapPoint = new MapPoint(0.0, 0.0);
			foreach (Shape shape in this.Shapes)
			{
				MapPoint centralPoint = shape.CentralPoint;
				centralPoint.X += shape.OffsetInt.X + shape.CentralPointOffset.X;
				centralPoint.Y += shape.OffsetInt.Y + shape.CentralPointOffset.Y;
				mapPoint.X += centralPoint.X;
				mapPoint.Y += centralPoint.Y;
			}
			mapPoint.X /= (double)count;
			mapPoint.Y /= (double)count;
			return this.GetMapCore().GeographicToPercents(mapPoint).ToPointF();
		}

		private string FieldDataToString()
		{
			string text = string.Empty;
			MapCore mapCore = this.GetMapCore();
			if (mapCore != null)
			{
				foreach (Field groupField in mapCore.GroupFields)
				{
					if (!groupField.IsTemporary)
					{
						string text2 = groupField.FormatValue(this.fields[groupField.Name]);
						if (!string.IsNullOrEmpty(text2))
						{
							string text3 = text;
							text = text3 + XmlConvert.EncodeName(groupField.Name) + "=" + text2 + "&";
						}
					}
				}
				text = text.TrimEnd('&');
			}
			return text;
		}

		internal void FieldDataFromBuffer()
		{
			if (!string.IsNullOrEmpty(this.fieldDataBuffer))
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
						Field field = (Field)mapCore.GroupFields.GetByName(name);
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
				foreach (Shape shape in mapCore.Shapes)
				{
					if (shape.ParentGroup == this.Name)
					{
						shape.ParentGroup = "";
					}
				}
				mapCore.InvalidateCachedPaths();
				mapCore.InvalidateRules();
				mapCore.InvalidateDataBinding();
				mapCore.InvalidateCachedBounds();
				mapCore.InvalidateGridSections();
			}
		}

		protected override void OnDispose()
		{
			this.ResetCachedPaths();
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

		internal void InvalidateRules()
		{
			MapCore mapCore = this.GetMapCore();
			if (mapCore != null)
			{
				mapCore.InvalidateRules();
				mapCore.Invalidate();
			}
		}

		private void RenderText(MapGraphics g)
		{
			if (!(this.TextInt == string.Empty))
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

		internal GraphicsPath GetPath(MapGraphics g, bool outlineOnly)
		{
			if (this.Shapes.Count == 0)
			{
				return null;
			}
			if (outlineOnly)
			{
				ArrayList arrayList = new ArrayList();
				foreach (Shape shape3 in this.Shapes)
				{
					GraphicsPath[] paths = shape3.GetPaths(g);
					arrayList.Add(paths[shape3.largestPathIndex]);
				}
				if (arrayList.Count > 0 && !this.GetMapCore().IsDesignMode())
				{
					GraphicsPathOutliner graphicsPathOutliner = new GraphicsPathOutliner(g.Graphics);
					return graphicsPathOutliner.GetOutlinePath((GraphicsPath[])arrayList.ToArray(typeof(GraphicsPath)));
				}
				return null;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			foreach (Shape shape4 in this.Shapes)
			{
				GraphicsPath[] paths2 = shape4.GetPaths(g);
				GraphicsPath[] array = paths2;
				foreach (GraphicsPath addingPath in array)
				{
					graphicsPath.AddPath(addingPath, false);
				}
			}
			return graphicsPath;
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
			foreach (Shape shape in this.Shapes)
			{
				shape.ResetCachedPaths();
			}
		}

		private float GetDistance(PointF pointA, PointF pointB)
		{
			double num = (double)Math.Abs(pointA.X - pointB.X);
			double num2 = (double)Math.Abs(pointA.Y - pointB.Y);
			return (float)Math.Sqrt(num * num + num2 * num2);
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
			if (this.BorderWidth <= 0)
			{
				return null;
			}
			Pen pen = new Pen(this.ApplyLayerTransparency(this.BorderColorInt), (float)this.BorderWidth);
			pen.DashStyle = MapGraphics.GetPenStyle(this.BorderStyle);
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
			return true;
		}

		void IContentElement.RenderShadow(MapGraphics g)
		{
		}

		void IContentElement.RenderBack(MapGraphics g, HotRegionList hotRegions)
		{
		}

		void IContentElement.RenderFront(MapGraphics g, HotRegionList hotRegions)
		{
		}

		void IContentElement.RenderText(MapGraphics g, HotRegionList hotRegions)
		{
			if (this.Shapes.Count > 0)
			{
				this.RenderText(g);
			}
		}

		RectangleF IContentElement.GetBoundRect(MapGraphics g)
		{
			return new RectangleF(0f, 0f, 100f, 100f);
		}

		void ISelectable.DrawSelection(MapGraphics g, RectangleF clipRect, bool designTimeSelection)
		{
			if (this.Shapes.Count != 0)
			{
				MapCore mapCore = this.GetMapCore();
				using (GraphicsPath graphicsPath = this.GetPath(g, false))
				{
					if (graphicsPath != null)
					{
						RectangleF bounds = graphicsPath.GetBounds();
						RectangleF rect = bounds;
						rect.Inflate(6f, 6f);
						if (clipRect.IntersectsWith(rect) && !bounds.IsEmpty)
						{
							g.DrawSelection(bounds, designTimeSelection, mapCore.SelectionBorderColor, mapCore.SelectionMarkerColor);
							PointF centerPointInContentPixels = this.GetCenterPointInContentPixels(g);
							AntiAliasing antiAliasing = g.AntiAliasing;
							g.AntiAliasing = AntiAliasing.None;
							g.DrawLine(Pens.Red, (float)(centerPointInContentPixels.X - 8.0), centerPointInContentPixels.Y, (float)(centerPointInContentPixels.X + 8.0), centerPointInContentPixels.Y);
							g.DrawLine(Pens.Red, centerPointInContentPixels.X, (float)(centerPointInContentPixels.Y - 8.0), centerPointInContentPixels.X, (float)(centerPointInContentPixels.Y + 8.0));
							g.AntiAliasing = antiAliasing;
						}
					}
				}
			}
		}

		RectangleF ISelectable.GetSelectionRectangle(MapGraphics g, RectangleF clipRect)
		{
			return RectangleF.Empty;
		}

		bool ISelectable.IsVisible()
		{
			return this.Visible;
		}

		bool ISelectable.IsSelected()
		{
			return this.Selected;
		}

		string IToolTipProvider.GetToolTip()
		{
			if (this.Common != null && this.Common.MapCore != null)
			{
				return this.Common.MapCore.ResolveAllKeywords(this.ToolTipInt, this);
			}
			return this.ToolTipInt;
		}
	}
}
