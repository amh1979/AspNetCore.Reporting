using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class Panel : NamedElement, IToolTipProvider, IZOrderedObject, ISelectable, IDefaultValueProvider, IImageMapProvider
	{
		private const int DefaultMarginsAllValues = 5;

		private int zOrder;

		private PanelMargins margins;

		private MapLocation location;

		private CoordinateUnit locationUnit = CoordinateUnit.Percent;

		private MapSize size;

		private CoordinateUnit sizeUnit = CoordinateUnit.Percent;

		private bool visible;

		private Color borderColor;

		private MapDashStyle borderStyle = MapDashStyle.Solid;

		private int borderWidth;

		private Color backColor;

		private GradientType backGradientType;

		private Color backSecondaryColor = Color.Empty;

		private MapHatchStyle backHatchStyle;

		private int backShadowOffset = 2;

		private bool selected;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private object mapAreaTag;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[TypeConverter(typeof(StringConverter))]
		public override object Tag
		{
			get
			{
				return base.Tag;
			}
			set
			{
				base.Tag = value;
			}
		}

		[SRDescription("DescriptionAttributePanel_ZOrder")]
		[SRCategory("CategoryAttribute_Layout")]
		[DefaultValue(0)]
		[NotifyParentProperty(true)]
		public virtual int ZOrder
		{
			get
			{
				return this.zOrder;
			}
			set
			{
				if (this.zOrder != value)
				{
					this.zOrder = value;
					this.Invalidate();
					this.SizeLocationChanged(SizeLocationChangeInfo.ZOrder);
				}
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributePanel_Margins")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryAttribute_Layout")]
		public PanelMargins Margins
		{
			get
			{
				return this.margins;
			}
			set
			{
				if (this.margins != null && this.margins.Equals(value))
				{
					return;
				}
				this.margins = value;
				this.margins.Owner = this;
				this.Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(LocationConverter))]
		[SRCategory("CategoryAttribute_Layout")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributePanel_Location")]
		public virtual MapLocation Location
		{
			get
			{
				return this.location;
			}
			set
			{
				this.location = value;
				this.location.Parent = this;
				this.Invalidate();
				this.SizeLocationChanged(SizeLocationChangeInfo.Location);
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributePanel_LocationUnit")]
		[NotifyParentProperty(true)]
		public virtual CoordinateUnit LocationUnit
		{
			get
			{
				return this.locationUnit;
			}
			set
			{
				this.locationUnit = value;
				this.Invalidate();
				this.SizeLocationChanged(SizeLocationChangeInfo.LocationUnit);
			}
		}

		[TypeConverter(typeof(SizeConverter))]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributePanel_Size")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual MapSize Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
				this.size.Parent = this;
				this.Invalidate();
				this.SizeLocationChanged(SizeLocationChangeInfo.Size);
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributePanel_SizeUnit")]
		[SRCategory("CategoryAttribute_Layout")]
		public virtual CoordinateUnit SizeUnit
		{
			get
			{
				return this.sizeUnit;
			}
			set
			{
				this.sizeUnit = value;
				this.Invalidate();
				this.SizeLocationChanged(SizeLocationChangeInfo.SizeUnit);
			}
		}

		[SRDescription("DescriptionAttributePanel_Visible")]
		[DefaultValue(false)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[ParenthesizePropertyName(true)]
		public virtual bool Visible
		{
			get
			{
				return this.visible;
			}
			set
			{
				this.visible = value;
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePanel_BorderColor")]
		public virtual Color BorderColor
		{
			get
			{
				return this.borderColor;
			}
			set
			{
				this.borderColor = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributePanel_BorderStyle")]
		[DefaultValue(MapDashStyle.Solid)]
		public virtual MapDashStyle BorderStyle
		{
			get
			{
				return this.borderStyle;
			}
			set
			{
				this.borderStyle = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePanel_BorderWidth")]
		[NotifyParentProperty(true)]
		public virtual int BorderWidth
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
					this.Invalidate();
					return;
				}
				throw new ArgumentException(SR.must_in_range(0.0, 100.0));
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePanel_BackColor")]
		public virtual Color BackColor
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

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePanel_BackGradientType")]
		//[Editor(typeof(GradientEditor), typeof(UITypeEditor))]
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

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePanel_BackSecondaryColor")]
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

		[SRDescription("DescriptionAttributePanel_BackHatchStyle")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		//[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributePanel_BackShadowOffset")]
		[DefaultValue(2)]
		public virtual int BackShadowOffset
		{
			get
			{
				return this.backShadowOffset;
			}
			set
			{
				if (value >= -100 && value <= 100)
				{
					this.backShadowOffset = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributePanel_Selected")]
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
				this.Invalidate();
			}
		}

		[Localizable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributePanel_ToolTip")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue("")]
		[Browsable(false)]
		public virtual string ToolTip
		{
			get
			{
				return this.toolTip;
			}
			set
			{
				this.toolTip = value;
			}
		}

		[DefaultValue("")]
		[Localizable(true)]
		[SRDescription("DescriptionAttributePanel_Href")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		public virtual string Href
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
		[SRDescription("DescriptionAttributePanel_MapAreaAttributes")]
		[NotifyParentProperty(true)]
		public virtual string MapAreaAttributes
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

		protected bool ShouldSerializeMargins()
		{
			PanelMargins panelMargins = (PanelMargins)this.GetDefaultPropertyValue("Margins", this.Margins);
			if (this.Margins.Top == panelMargins.Top && this.Margins.Bottom == panelMargins.Bottom && this.Margins.Left == panelMargins.Left && this.Margins.Right == panelMargins.Right)
			{
				return false;
			}
			return true;
		}

		protected void ResetMargins()
		{
			PanelMargins panelMargins = (PanelMargins)this.GetDefaultPropertyValue("Margins", this.Margins);
			this.margins.Top = panelMargins.Top;
			this.margins.Bottom = panelMargins.Bottom;
			this.margins.Left = panelMargins.Left;
			this.margins.Right = panelMargins.Right;
		}

		protected void ResetLocation()
		{
			MapLocation mapLocation = (MapLocation)this.GetDefaultPropertyValue("Location", this.Location);
			this.Location.X = mapLocation.X;
			this.Location.Y = mapLocation.Y;
		}

		protected bool ShouldSerializeLocation()
		{
			MapLocation mapLocation = (MapLocation)this.GetDefaultPropertyValue("Location", this.Location);
			if (this.Location.X == mapLocation.X)
			{
				return this.Location.Y != mapLocation.Y;
			}
			return true;
		}

		protected void ResetLocationUnit()
		{
			this.LocationUnit = (CoordinateUnit)this.GetDefaultPropertyValue("LocationUnit", this.LocationUnit);
		}

		protected bool ShouldSerializeLocationUnit()
		{
			return !this.LocationUnit.Equals(this.GetDefaultPropertyValue("LocationUnit", this.LocationUnit));
		}

		protected void ResetSize()
		{
			MapSize mapSize = (MapSize)this.GetDefaultPropertyValue("Size", this.Size);
			this.Size.Width = mapSize.Width;
			this.Size.Height = mapSize.Height;
		}

		protected bool ShouldSerializeSize()
		{
			MapSize mapSize = (MapSize)this.GetDefaultPropertyValue("Size", this.Size);
			if (this.Size.Width == mapSize.Width)
			{
				return this.Size.Height != mapSize.Height;
			}
			return true;
		}

		protected void ResetSizeUnit()
		{
			this.SizeUnit = (CoordinateUnit)this.GetDefaultPropertyValue("SizeUnit", this.SizeUnit);
		}

		protected bool ShouldSerializeSizeUnit()
		{
			return !this.SizeUnit.Equals(this.GetDefaultPropertyValue("SizeUnit", this.SizeUnit));
		}

		protected void ResetBorderColor()
		{
			Color color2 = this.BorderColor = (Color)this.GetDefaultPropertyValue("BorderColor", this.BorderColor);
		}

		protected bool ShouldSerializeBorderColor()
		{
			Color right = (Color)this.GetDefaultPropertyValue("BorderColor", this.BorderColor);
			return this.BorderColor != right;
		}

		protected void ResetBorderWidth()
		{
			int num2 = this.BorderWidth = (int)this.GetDefaultPropertyValue("BorderWidth", this.BorderWidth);
		}

		protected bool ShouldSerializeBorderWidth()
		{
			int num = (int)this.GetDefaultPropertyValue("BorderWidth", this.BorderWidth);
			return this.BorderWidth != num;
		}

		protected void ResetBackColor()
		{
			Color color2 = this.BackColor = (Color)this.GetDefaultPropertyValue("BackColor", this.BackColor);
		}

		protected bool ShouldSerializeBackColor()
		{
			Color right = (Color)this.GetDefaultPropertyValue("BackColor", this.BackColor);
			return this.BackColor != right;
		}

		protected void ResetBackGradientType()
		{
			GradientType gradientType2 = this.BackGradientType = (GradientType)this.GetDefaultPropertyValue("BackGradientType", this.BackGradientType);
		}

		protected bool ShouldSerializeBackGradientType()
		{
			GradientType gradientType = (GradientType)this.GetDefaultPropertyValue("BackGradientType", this.BackGradientType);
			return this.BackGradientType != gradientType;
		}

		protected void ResetBackSecondaryColor()
		{
			Color color2 = this.BackSecondaryColor = (Color)this.GetDefaultPropertyValue("BackSecondaryColor", this.BackSecondaryColor);
		}

		protected bool ShouldSerializeBackSecondaryColor()
		{
			Color right = (Color)this.GetDefaultPropertyValue("BackSecondaryColor", this.BackSecondaryColor);
			return this.BackSecondaryColor != right;
		}

		protected void ResetBackHatchStyle()
		{
			MapHatchStyle mapHatchStyle2 = this.BackHatchStyle = (MapHatchStyle)this.GetDefaultPropertyValue("BackHatchStyle", this.BackHatchStyle);
		}

		protected bool ShouldSerializeBackHatchStyle()
		{
			MapHatchStyle mapHatchStyle = (MapHatchStyle)this.GetDefaultPropertyValue("BackHatchStyle", this.BackHatchStyle);
			return this.BackHatchStyle != mapHatchStyle;
		}

		public Panel()
			: this(null)
		{
		}

		internal Panel(CommonElements common)
			: base(common)
		{
			this.Margins = new PanelMargins((PanelMargins)this.GetDefaultPropertyValue("Margins", null));
			this.Location = new MapLocation((MapLocation)this.GetDefaultPropertyValue("Location", null));
			this.LocationUnit = (CoordinateUnit)this.GetDefaultPropertyValue("LocationUnit", null);
			this.Size = new MapSize((MapSize)this.GetDefaultPropertyValue("Size", null));
			this.SizeUnit = (CoordinateUnit)this.GetDefaultPropertyValue("SizeUnit", null);
			this.BackColor = (Color)this.GetDefaultPropertyValue("BackColor", null);
			this.BorderColor = (Color)this.GetDefaultPropertyValue("BorderColor", null);
			this.BorderWidth = (int)this.GetDefaultPropertyValue("BorderWidth", null);
			this.BackGradientType = (GradientType)this.GetDefaultPropertyValue("BackGradientType", null);
			this.BackHatchStyle = (MapHatchStyle)this.GetDefaultPropertyValue("BackHatchStyle", null);
			this.BackSecondaryColor = (Color)this.GetDefaultPropertyValue("BackSecondaryColor", null);
		}

		public SizeF GetSizeInPixels()
		{
			SizeF result = default(SizeF);
			if (this.SizeUnit == CoordinateUnit.Pixel)
			{
				result.Width = this.Size.Width;
				result.Height = this.Size.Height;
			}
			else
			{
				MapCore mapCore = this.GetMapCore();
				if (mapCore == null)
				{
					return result;
				}
				result.Width = (float)((float)mapCore.Width * this.Size.Width / 100.0);
				result.Height = (float)((float)mapCore.Height * this.Size.Height / 100.0);
			}
			return result;
		}

		public RectangleF GetBoundsInPixels()
		{
			return new RectangleF(this.GetLocationInPixels(), this.GetSizeInPixels());
		}

		public void SetLocationInPixels(PointF location)
		{
			MapCore mapCore = this.GetMapCore();
			if (mapCore != null)
			{
				if (this.LocationUnit == CoordinateUnit.Pixel)
				{
					this.Location = new MapLocation(this, location.X, location.Y);
				}
				else
				{
					float num = (float)(100.0 * location.X / (float)(mapCore.Width - 1));
					num = (float)((!float.IsNaN(num)) ? num : 0.0);
					float num2 = (float)(100.0 * location.Y / (float)(mapCore.Height - 1));
					num2 = (float)((!float.IsNaN(num2)) ? num2 : 0.0);
					this.Location = new MapLocation(this, num, num2);
				}
			}
		}

		public void SetSizeInPixels(SizeF size)
		{
			MapCore mapCore = this.GetMapCore();
			if (mapCore != null)
			{
				if (this.SizeUnit == CoordinateUnit.Pixel)
				{
					this.Size = new MapSize(this, size.Width, size.Height);
				}
				else
				{
					float num = (float)(100.0 * size.Width / (float)mapCore.Width);
					num = (float)((!float.IsNaN(num) && !float.IsInfinity(num)) ? num : 0.0);
					float num2 = (float)(100.0 * size.Height / (float)mapCore.Height);
					num2 = (float)((!float.IsNaN(num2) && !float.IsInfinity(num2)) ? num2 : 0.0);
					this.Size = new MapSize(this, num, num2);
				}
			}
		}

		public void SetBoundsInPixels(RectangleF bounds)
		{
			this.SetLocationInPixels(bounds.Location);
			this.SetSizeInPixels(bounds.Size);
		}

		public PointF GetLocationInPixels()
		{
			PointF result = default(PointF);
			if (this.LocationUnit == CoordinateUnit.Pixel)
			{
				result.X = this.Location.X;
				result.Y = this.Location.Y;
			}
			else
			{
				MapCore mapCore = this.GetMapCore();
				if (mapCore == null)
				{
					return result;
				}
				result.X = (float)((float)(mapCore.Width - 1) * this.Location.X / 100.0);
				result.Y = (float)((float)(mapCore.Height - 1) * this.Location.Y / 100.0);
			}
			return result;
		}

		public virtual RectangleF GetBoundRect(MapGraphics g)
		{
			RectangleF result = default(RectangleF);
			if (this.LocationUnit == CoordinateUnit.Percent)
			{
				result.Location = this.Location;
			}
			else
			{
				result.Location = g.GetRelativePoint(this.Location);
			}
			if (this.SizeUnit == CoordinateUnit.Percent)
			{
				result.Size = this.Size;
			}
			else
			{
				result.Size = g.GetRelativeSize(this.Size);
			}
			return result;
		}

		internal virtual GraphicsPath GetHotRegionPath(MapGraphics g)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			RectangleF rect = new RectangleF(this.GetAbsoluteLocation(), this.GetAbsoluteSize());
			graphicsPath.AddRectangle(rect);
			return graphicsPath;
		}

		internal void SizeChanged(MapSize size)
		{
			this.SizeLocationChanged(SizeLocationChangeInfo.Size);
		}

		internal virtual void LocationChanged(MapLocation size)
		{
			this.SizeLocationChanged(SizeLocationChangeInfo.Location);
		}

		internal virtual void Render(MapGraphics g)
		{
		}

		internal virtual bool ShouldRenderBackground()
		{
			return true;
		}

		internal virtual void RenderBackground(MapGraphics g)
		{
			AntiAliasing antiAliasing = g.AntiAliasing;
			g.AntiAliasing = AntiAliasing.None;
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			absoluteRectangle.X = (float)Math.Round((double)absoluteRectangle.X);
			absoluteRectangle.Y = (float)Math.Round((double)absoluteRectangle.Y);
			absoluteRectangle.Width = (float)Math.Round((double)absoluteRectangle.Width);
			absoluteRectangle.Height = (float)Math.Round((double)absoluteRectangle.Height);
			try
			{
				if (this.BackShadowOffset != 0)
				{
					RectangleF rect = absoluteRectangle;
					rect.Offset((float)this.BackShadowOffset, (float)this.BackShadowOffset);
					g.FillRectangle(g.GetShadowBrush(), rect);
				}
				if (this.IsMakeTransparentRequired())
				{
					using (Brush brush = new SolidBrush(this.GetColorForMakeTransparent()))
					{
						g.FillRectangle(brush, absoluteRectangle);
					}
				}
				using (Brush brush2 = g.CreateBrush(absoluteRectangle, this.BackColor, this.BackHatchStyle, "", MapImageWrapMode.Unscaled, Color.Empty, MapImageAlign.Center, this.BackGradientType, this.BackSecondaryColor))
				{
					g.FillRectangle(brush2, absoluteRectangle);
				}
			}
			finally
			{
				g.AntiAliasing = antiAliasing;
			}
		}

		internal virtual void RenderBorder(MapGraphics g)
		{
			AntiAliasing antiAliasing = g.AntiAliasing;
			g.AntiAliasing = AntiAliasing.None;
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			absoluteRectangle.X = (float)Math.Round((double)absoluteRectangle.X);
			absoluteRectangle.Y = (float)Math.Round((double)absoluteRectangle.Y);
			absoluteRectangle.Width = (float)Math.Round((double)absoluteRectangle.Width);
			absoluteRectangle.Height = (float)Math.Round((double)absoluteRectangle.Height);
			try
			{
				if (this.BorderWidth > 0 && !this.BorderColor.IsEmpty && this.BorderStyle != 0)
				{
					using (Pen pen = new Pen(this.BorderColor, (float)this.BorderWidth))
					{
						pen.DashStyle = MapGraphics.GetPenStyle(this.BorderStyle);
						pen.Alignment = PenAlignment.Inset;
						if (this.BorderWidth == 1)
						{
							absoluteRectangle.Width -= 1f;
							absoluteRectangle.Height -= 1f;
						}
						g.DrawRectangle(pen, absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height);
					}
				}
			}
			finally
			{
				g.AntiAliasing = antiAliasing;
			}
		}

		internal virtual void SizeLocationChanged(SizeLocationChangeInfo info)
		{
		}

		internal void RenderPanel(MapGraphics g)
		{
			if (this.IsVisible())
			{
				try
				{
					RectangleF relativeRectangle = g.GetRelativeRectangle(this.Margins.AdjustRectangle(this.GetBoundsInPixels()));
					g.CreateDrawRegion(relativeRectangle);
					SizeF absoluteSize = g.GetAbsoluteSize(new SizeF(100f, 100f));
					if (!(absoluteSize.Width < 1.0) && !(absoluteSize.Height < 1.0))
					{
						if (this.ShouldRenderBackground() && this.GetMapCore().RenderingMode != RenderingMode.ZoomThumb)
						{
							this.RenderBackground(g);
							this.RenderBorder(g);
						}
						if (this.BorderWidth > 0 && this.ShouldRenderBackground())
						{
							try
							{
								RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
								absoluteRectangle.Inflate((float)(-this.BorderWidth), (float)(-this.BorderWidth));
								absoluteRectangle.Width = Math.Max(2f, absoluteRectangle.Width);
								absoluteRectangle.Height = Math.Max(2f, absoluteRectangle.Height);
								g.CreateDrawRegion(g.GetRelativeRectangle(absoluteRectangle));
								this.Render(g);
							}
							finally
							{
								g.RestoreDrawRegion();
							}
						}
						else
						{
							this.Render(g);
						}
					}
				}
				finally
				{
					g.RestoreDrawRegion();
				}
			}
		}

		internal virtual bool IsRenderVisible(MapGraphics g, RectangleF clipRect)
		{
			if (!this.IsVisible())
			{
				return false;
			}
			return clipRect.IntersectsWith(this.GetBoundsInPixels());
		}

		internal PointF GetAbsoluteLocation()
		{
			PointF locationInPixels = this.GetLocationInPixels();
			locationInPixels.X += (float)this.Margins.Left;
			locationInPixels.Y += (float)this.Margins.Top;
			return locationInPixels;
		}

		internal SizeF GetAbsoluteSize()
		{
			SizeF sizeInPixels = this.GetSizeInPixels();
			sizeInPixels.Width -= (float)(this.Margins.Left + this.Margins.Right);
			sizeInPixels.Height -= (float)(this.Margins.Top + this.Margins.Bottom);
			return sizeInPixels;
		}

		internal virtual object GetDefaultPropertyValue(string prop, object currentValue)
		{
			object result = null;
			switch (prop)
			{
			case "Margins":
				result = new PanelMargins(5, 5, 5, 5);
				break;
			case "Location":
				result = new MapLocation(null, 0f, 0f);
				break;
			case "LocationUnit":
				result = CoordinateUnit.Percent;
				break;
			case "Size":
				result = new MapSize(null, 200f, 100f);
				break;
			case "SizeUnit":
				result = CoordinateUnit.Pixel;
				break;
			case "BackColor":
				result = Color.White;
				break;
			case "BorderColor":
				result = Color.DarkGray;
				break;
			case "BorderWidth":
				result = 1;
				break;
			case "BackGradientType":
				result = GradientType.DiagonalLeft;
				break;
			case "BackHatchStyle":
				result = MapHatchStyle.None;
				break;
			case "BackSecondaryColor":
				result = Color.FromArgb(230, 230, 230);
				break;
			}
			return result;
		}

		internal virtual bool IsVisible()
		{
			return this.Visible;
		}

		internal bool IsMakeTransparentRequired()
		{
			MapCore mapCore = this.GetMapCore();
			if (mapCore.RenderingMode != RenderingMode.SinglePanel)
			{
				return false;
			}
			if (this is Legend || this is ColorSwatchPanel || this is DistanceScalePanel || this is MapLabel)
			{
				if (this.BackColor.A == 0 && this.BackGradientType == GradientType.None)
				{
					goto IL_006a;
				}
				if (this.BackColor.A == 0 && this.BackSecondaryColor.A == 0)
				{
					goto IL_006a;
				}
			}
			return false;
			IL_006a:
			return true;
		}

		internal Color GetColorForMakeTransparent()
		{
			MapCore mapCore = this.GetMapCore();
			RectangleF rectangleF = new RectangleF(mapCore.Viewport.GetLocationInPixels(), mapCore.Viewport.GetSizeInPixels());
			RectangleF rect = new RectangleF(mapCore.Viewport.GetLocationInPixels(), mapCore.Viewport.GetSizeInPixels());
			if (rectangleF.Contains(rect) && mapCore.Viewport.BackColor.A != 0)
			{
				return Color.FromArgb(255, mapCore.Viewport.BackColor);
			}
			return Color.FromArgb(255, mapCore.MapControl.BackColor);
		}

		protected MapCore GetMapCore()
		{
			if (this.Common != null)
			{
				return this.Common.MapCore;
			}
			return null;
		}

		int IZOrderedObject.GetZOrder()
		{
			return this.ZOrder;
		}

		void ISelectable.DrawSelection(MapGraphics g, RectangleF clipRect, bool designTimeSelection)
		{
			MapCore mapCore = this.GetMapCore();
			if (mapCore != null && this.IsVisible())
			{
				RectangleF selectionRectangle = ((ISelectable)this).GetSelectionRectangle(g, clipRect);
				if (!selectionRectangle.IsEmpty)
				{
					g.DrawSelection(selectionRectangle, designTimeSelection, mapCore.SelectionBorderColor, mapCore.SelectionMarkerColor);
				}
			}
		}

		public virtual RectangleF GetSelectionRectangle(MapGraphics g, RectangleF clipRect)
		{
			return new RectangleF(this.GetAbsoluteLocation(), this.GetAbsoluteSize());
		}

		bool ISelectable.IsSelected()
		{
			return this.Selected;
		}

		bool ISelectable.IsVisible()
		{
			return this.IsVisible();
		}

		string IToolTipProvider.GetToolTip()
		{
			if (this.Common != null && this.Common.MapCore != null)
			{
				return this.Common.MapCore.ResolveAllKeywords(this.ToolTip, this);
			}
			return this.ToolTip;
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

		object IDefaultValueProvider.GetDefaultValue(string prop, object currentValue)
		{
			return this.GetDefaultPropertyValue(prop, currentValue);
		}
	}
}
