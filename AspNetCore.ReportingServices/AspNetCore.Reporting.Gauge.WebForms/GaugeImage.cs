using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(GaugeImageConverter))]
	internal class GaugeImage : NamedElement, IRenderable, IToolTipProvider, ISelectable, IImageMapProvider
	{
		private string parent = string.Empty;

		private int zOrder;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private GaugeLocation location;

		private GaugeSize size;

		private bool visible = true;

		private ResizeMode resizeMode = ResizeMode.AutoFit;

		private string image = "";

		private Color imageTransColor = Color.Empty;

		private float shadowOffset = 1f;

		private float angle;

		private float transparency;

		private bool selected;

		private NamedElement parentSystem;

		private bool defaultParent = true;

		private object imageMapProviderTag;

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeGaugeImage_Parent")]
		[NotifyParentProperty(true)]
		[TypeConverter(typeof(ParentSourceConverter))]
		public string Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				string text = this.parent;
				if (value == "(none)")
				{
					value = string.Empty;
				}
				this.parent = value;
				try
				{
					this.ConnectToParent(true);
				}
				catch
				{
					this.parent = text;
					throw;
				}
				this.DefaultParent = false;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeGaugeImage_ZOrder")]
		public int ZOrder
		{
			get
			{
				return this.zOrder;
			}
			set
			{
				this.zOrder = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[Localizable(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeGaugeImage_ToolTip")]
		public string ToolTip
		{
			get
			{
				return this.toolTip;
			}
			set
			{
				this.toolTip = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeGaugeImage_Href")]
		[Browsable(false)]
		[DefaultValue("")]
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

		[SRCategory("CategoryBehavior")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeGaugeImage_MapAreaAttributes")]
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

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeGaugeImage_Name")]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryPosition")]
		[ValidateBound(100.0, 100.0)]
		[SRDescription("DescriptionAttributeGaugeImage_Location")]
		[TypeConverter(typeof(LocationConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GaugeLocation Location
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
			}
		}

		[ValidateBound(100.0, 100.0)]
		[SRCategory("CategoryPosition")]
		[SRDescription("DescriptionAttributeGaugeImage_Size")]
		[TypeConverter(typeof(SizeConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GaugeSize Size
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
			}
		}

		[SRCategory("CategoryAppearance")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeGaugeImage_Visible")]
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
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeGaugeImage_ResizeMode")]
		[SRCategory("CategoryImage")]
		[DefaultValue(ResizeMode.AutoFit)]
		public ResizeMode ResizeMode
		{
			get
			{
				return this.resizeMode;
			}
			set
			{
				this.resizeMode = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryImage")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeGaugeImage_Image")]
		public string Image
		{
			get
			{
				return this.image;
			}
			set
			{
				this.image = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeImageTransColor6")]
		public Color ImageTransColor
		{
			get
			{
				return this.imageTransColor;
			}
			set
			{
				this.imageTransColor = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[ValidateBound(-5.0, 5.0)]
		[SRDescription("DescriptionAttributeShadowOffset4")]
		[NotifyParentProperty(true)]
		[DefaultValue(1f)]
		public float ShadowOffset
		{
			get
			{
				return this.shadowOffset;
			}
			set
			{
				if (!(value < -100.0) && !(value > 100.0))
				{
					this.shadowOffset = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
			}
		}

		[ValidateBound(0.0, 360.0)]
		[SRCategory("CategoryPosition")]
		[SRDescription("DescriptionAttributeGaugeImage_Angle")]
		[DefaultValue(0f)]
		public float Angle
		{
			get
			{
				return this.angle;
			}
			set
			{
				if (!(value > 360.0) && !(value < 0.0))
				{
					this.angle = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 360));
			}
		}

		[ValidateBound(0.0, 100.0)]
		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeGaugeImage_Transparency")]
		[DefaultValue(0f)]
		public float Transparency
		{
			get
			{
				return this.transparency;
			}
			set
			{
				if (!(value > 100.0) && !(value < 0.0))
				{
					this.transparency = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 100));
			}
		}

		[DefaultValue(false)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeImage_Selected")]
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
				this.Invalidate();
			}
		}

		internal Position Position
		{
			get
			{
				return new Position(this.Location, this.Size, ContentAlignment.TopLeft);
			}
		}

		[Browsable(false)]
		[Bindable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeParentObject3")]
		public NamedElement ParentObject
		{
			get
			{
				return this.parentSystem;
			}
		}

		internal bool DefaultParent
		{
			get
			{
				return this.defaultParent;
			}
			set
			{
				this.defaultParent = value;
			}
		}

		object IImageMapProvider.Tag
		{
			get
			{
				return this.imageMapProviderTag;
			}
			set
			{
				this.imageMapProviderTag = value;
			}
		}

		public GaugeImage()
		{
			this.location = new GaugeLocation(this, 26f, 47f);
			this.size = new GaugeSize(this, 15f, 15f);
			this.location.DefaultValues = true;
			this.size.DefaultValues = true;
		}

		public override string ToString()
		{
			return this.Name;
		}

		internal void DrawImage(GaugeGraphics g, string imageName, bool drawShadow)
		{
			if (drawShadow && this.ShadowOffset == 0.0)
			{
				return;
			}
			Image image = this.Common.ImageLoader.LoadImage(imageName);
			if (image.Width != 0 && image.Height != 0)
			{
				RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
				Rectangle rectangle = Rectangle.Empty;
				if (this.ResizeMode == ResizeMode.AutoFit)
				{
					rectangle = new Rectangle((int)absoluteRectangle.X, (int)absoluteRectangle.Y, (int)absoluteRectangle.Width, (int)absoluteRectangle.Height);
				}
				else
				{
					rectangle = new Rectangle(0, 0, image.Width, image.Height);
					PointF absolutePoint = g.GetAbsolutePoint(new PointF(50f, 50f));
					rectangle.X = (int)(absolutePoint.X - (float)(rectangle.Size.Width / 2));
					rectangle.Y = (int)(absolutePoint.Y - (float)(rectangle.Size.Height / 2));
				}
				ImageAttributes imageAttributes = new ImageAttributes();
				if (this.ImageTransColor != Color.Empty)
				{
					imageAttributes.SetColorKey(this.ImageTransColor, this.ImageTransColor, ColorAdjustType.Default);
				}
				float num = (float)((100.0 - this.Transparency) / 100.0);
				float num2 = (float)(this.Common.GaugeCore.ShadowIntensity / 100.0);
				if (drawShadow)
				{
					ColorMatrix colorMatrix = new ColorMatrix();
					colorMatrix.Matrix00 = 0f;
					colorMatrix.Matrix11 = 0f;
					colorMatrix.Matrix22 = 0f;
					colorMatrix.Matrix33 = num2 * num;
					imageAttributes.SetColorMatrix(colorMatrix);
				}
				else if (this.Transparency > 0.0)
				{
					ColorMatrix colorMatrix2 = new ColorMatrix();
					colorMatrix2.Matrix33 = num;
					imageAttributes.SetColorMatrix(colorMatrix2);
				}
				if (this.Angle != 0.0)
				{
					PointF point = new PointF((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0), (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0));
					Matrix transform = g.Transform;
					Matrix matrix = g.Transform.Clone();
					float offsetX = matrix.OffsetX;
					float offsetY = matrix.OffsetY;
					point.X += offsetX;
					point.Y += offsetY;
					matrix.RotateAt(this.Angle, point, MatrixOrder.Append);
					if (drawShadow)
					{
						matrix.Translate(this.ShadowOffset, this.ShadowOffset, MatrixOrder.Append);
					}
					g.Transform = matrix;
					ImageSmoothingState imageSmoothingState = new ImageSmoothingState(g);
					imageSmoothingState.Set();
					g.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
					imageSmoothingState.Restore();
					g.Transform = transform;
				}
				else
				{
					if (drawShadow)
					{
						rectangle.X += (int)this.ShadowOffset;
						rectangle.Y += (int)this.ShadowOffset;
					}
					ImageSmoothingState imageSmoothingState2 = new ImageSmoothingState(g);
					imageSmoothingState2.Set();
					g.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
					imageSmoothingState2.Restore();
				}
				if (!drawShadow)
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					graphicsPath.AddRectangle(rectangle);
					if (this.Angle != 0.0)
					{
						PointF point2 = new PointF((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0), (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0));
						using (Matrix matrix2 = new Matrix())
						{
							matrix2.RotateAt(this.Angle, point2, MatrixOrder.Append);
							graphicsPath.Transform(matrix2);
						}
					}
					this.Common.GaugeCore.HotRegionList.SetHotRegion(this, Point.Empty, graphicsPath);
				}
			}
		}

		internal GraphicsPath GetTextPath(GaugeGraphics g)
		{
			if (!this.Visible)
			{
				return null;
			}
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			GraphicsPath graphicsPath = new GraphicsPath();
			string noImageString = SR.NoImageString;
			Font font = new Font("Microsoft Sans Serif", 8.25f);
			SizeF sizeF = g.MeasureString(noImageString, font);
			SizeF absoluteSize = g.GetAbsoluteSize(new SizeF(100f, 100f));
			float num = absoluteSize.Width / sizeF.Width;
			float num2 = absoluteSize.Height / sizeF.Height;
			float emSize = (float)((!(num < num2)) ? (font.SizeInPoints * num2 * 1.2999999523162842) : (font.SizeInPoints * num * 1.2999999523162842));
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			graphicsPath.AddString(noImageString, font.FontFamily, (int)font.Style, emSize, absoluteRectangle, stringFormat);
			if (this.Angle != 0.0)
			{
				PointF point = new PointF((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0), (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0));
				using (Matrix matrix = new Matrix())
				{
					matrix.RotateAt(this.Angle, point);
					graphicsPath.Transform(matrix);
					return graphicsPath;
				}
			}
			return graphicsPath;
		}

		internal GraphicsPath GetBackPath(GaugeGraphics g)
		{
			if (!this.Visible)
			{
				return null;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			graphicsPath.AddRectangle(absoluteRectangle);
			if (this.Angle != 0.0)
			{
				PointF point = new PointF((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0), (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0));
				using (Matrix matrix = new Matrix())
				{
					matrix.RotateAt(this.Angle, point);
					graphicsPath.Transform(matrix);
					return graphicsPath;
				}
			}
			return graphicsPath;
		}

		internal override void EndInit()
		{
			base.EndInit();
			this.ConnectToParent(true);
		}

		private void ConnectToParent(bool exact)
		{
			if (this.Common != null && !this.Common.GaugeCore.isInitializing)
			{
				if (this.parent == string.Empty)
				{
					this.parentSystem = null;
				}
				else
				{
					this.Common.ObjectLinker.IsParentElementValid(this, this, exact);
					this.parentSystem = this.Common.ObjectLinker.GetElement(this.parent);
				}
			}
		}

		internal override void Notify(MessageType msg, NamedElement element, object param)
		{
			base.Notify(msg, element, param);
			switch (msg)
			{
			case MessageType.NamedElementRemove:
				if (this.parentSystem == element)
				{
					this.Parent = string.Empty;
				}
				break;
			case MessageType.NamedElementRename:
				if (this.parentSystem == element)
				{
					this.parent = element.GetNameAsParent((string)param);
				}
				break;
			}
		}

		internal override void OnAdded()
		{
			base.OnAdded();
			this.ConnectToParent(true);
		}

		void IRenderable.RenderStaticElements(GaugeGraphics g)
		{
			if (this.Visible)
			{
				g.StartHotRegion(this);
				if (this.Image.Length != 0)
				{
					this.DrawImage(g, this.image, true);
					this.DrawImage(g, this.image, false);
					g.EndHotRegion();
				}
				else
				{
					GraphicsPath graphicsPath = null;
					GraphicsPath graphicsPath2 = null;
					try
					{
						graphicsPath = this.GetTextPath(g);
						graphicsPath2 = this.GetBackPath(g);
						g.FillPath(Brushes.White, graphicsPath2);
						AntiAliasing antiAliasing = this.Common.GaugeContainer.AntiAliasing;
						AntiAliasing antiAliasing2 = g.AntiAliasing;
						if (this.Common.GaugeContainer.AntiAliasing == AntiAliasing.Text)
						{
							antiAliasing = AntiAliasing.Graphics;
						}
						else if (this.Common.GaugeContainer.AntiAliasing == AntiAliasing.Graphics)
						{
							antiAliasing = AntiAliasing.None;
						}
						g.AntiAliasing = antiAliasing;
						g.FillPath(Brushes.Black, graphicsPath);
						g.AntiAliasing = antiAliasing2;
						g.DrawPath(Pens.Black, graphicsPath2);
						this.Common.GaugeCore.HotRegionList.SetHotRegion(this, Point.Empty, (GraphicsPath)graphicsPath2.Clone());
					}
					finally
					{
						if (graphicsPath != null)
						{
							graphicsPath.Dispose();
						}
						if (graphicsPath2 != null)
						{
							graphicsPath2.Dispose();
						}
						g.EndHotRegion();
					}
				}
			}
		}

		void IRenderable.RenderDynamicElements(GaugeGraphics g)
		{
		}

		int IRenderable.GetZOrder()
		{
			return this.ZOrder;
		}

		RectangleF IRenderable.GetBoundRect(GaugeGraphics g)
		{
			return this.Position.Rectangle;
		}

		object IRenderable.GetParentRenderable()
		{
			return this.ParentObject;
		}

		string IRenderable.GetParentRenderableName()
		{
			return this.parent;
		}

		string IToolTipProvider.GetToolTip(HitTestResult ht)
		{
			if (this.Common != null && this.Common.GaugeCore != null)
			{
				return this.Common.GaugeCore.ResolveAllKeywords(this.ToolTip, this);
			}
			return this.ToolTip;
		}

		string IImageMapProvider.GetToolTip()
		{
			return ((IToolTipProvider)this).GetToolTip((HitTestResult)null);
		}

		string IImageMapProvider.GetHref()
		{
			if (this.Common != null && this.Common.GaugeCore != null)
			{
				return this.Common.GaugeCore.ResolveAllKeywords(this.Href, this);
			}
			return this.Href;
		}

		string IImageMapProvider.GetMapAreaAttributes()
		{
			if (this.Common != null && this.Common.GaugeCore != null)
			{
				return this.Common.GaugeCore.ResolveAllKeywords(this.MapAreaAttributes, this);
			}
			return this.MapAreaAttributes;
		}

		void ISelectable.DrawSelection(GaugeGraphics g, bool designTimeSelection)
		{
			Stack stack = new Stack();
			for (NamedElement namedElement = this.ParentObject; namedElement != null; namedElement = (NamedElement)((IRenderable)namedElement).GetParentRenderable())
			{
				stack.Push(namedElement);
			}
			foreach (IRenderable item in stack)
			{
				g.CreateDrawRegion(item.GetBoundRect(g));
			}
			g.CreateDrawRegion(((IRenderable)this).GetBoundRect(g));
			g.DrawSelection(g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f)), designTimeSelection, this.Common.GaugeCore.SelectionBorderColor, this.Common.GaugeCore.SelectionMarkerColor);
			g.RestoreDrawRegion();
			foreach (IRenderable item2 in stack)
			{
				IRenderable renderable = item2;
				g.RestoreDrawRegion();
			}
		}

		public override object Clone()
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatSerializer binaryFormatSerializer = new BinaryFormatSerializer();
			binaryFormatSerializer.Serialize(this, stream);
			GaugeImage gaugeImage = new GaugeImage();
			binaryFormatSerializer.Deserialize(gaugeImage, stream);
			return gaugeImage;
		}
	}
}
