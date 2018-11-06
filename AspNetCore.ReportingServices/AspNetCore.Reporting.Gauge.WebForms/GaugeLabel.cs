using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.IO;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(GaugeLabelConverter))]
	internal class GaugeLabel : NamedElement, IRenderable, IToolTipProvider, ISelectable, IImageMapProvider
	{
		private string parent = string.Empty;

		private int zOrder;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private GaugeLocation location;

		private GaugeSize size;

		private ContentAlignment textAlignment = ContentAlignment.TopLeft;

		private bool visible = true;

		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private ResizeMode resizeMode = ResizeMode.AutoFit;

		private FontUnit fontUnit = FontUnit.Default;

		private Color borderColor = Color.Black;

		private GaugeDashStyle borderStyle;

		private int borderWidth = 1;

		private Color backColor = Color.Empty;

		private Color textColor = Color.Black;

		private GradientType backGradientType;

		private Color backGradientEndColor = Color.Empty;

		private GaugeHatchStyle backHatchStyle;

		private string text = "Text";

		private int backShadowOffset;

		private int textShadowOffset;

		private float angle;

		private bool selected;

		private NamedElement parentSystem;

		private bool defaultParent = true;

		private object imageMapProviderTag;

		[NotifyParentProperty(true)]
		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeGaugeLabel_Parent")]
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
		[SRDescription("DescriptionAttributeGaugeLabel_ZOrder")]
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

		[DefaultValue("")]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeGaugeLabel_ToolTip")]
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
				this.Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeGaugeLabel_Href")]
		[Localizable(true)]
		[Browsable(false)]
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

		[Browsable(false)]
		[SRCategory("CategoryBehavior")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeMapAreaAttributes4")]
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

		[SRDescription("DescriptionAttributeGaugeLabel_Name")]
		[SRCategory("CategoryMisc")]
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

		[ValidateBound(100.0, 100.0)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryPosition")]
		[SRDescription("DescriptionAttributeGaugeLabel_Location")]
		[TypeConverter(typeof(LocationConverter))]
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

		[SRCategory("CategoryPosition")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[ValidateBound(100.0, 100.0)]
		[TypeConverter(typeof(SizeConverter))]
		[SRDescription("DescriptionAttributeGaugeLabel_Size")]
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

		[DefaultValue(ContentAlignment.TopLeft)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeGaugeLabel_TextAlignment")]
		public ContentAlignment TextAlignment
		{
			get
			{
				return this.textAlignment;
			}
			set
			{
				this.textAlignment = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeGaugeLabel_Visible")]
		[ParenthesizePropertyName(true)]
		[SRCategory("CategoryAppearance")]
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
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8.25pt")]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeLabel_Font")]
		public Font Font
		{
			get
			{
				return this.font;
			}
			set
			{
				this.font = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeResizeMode")]
		[DefaultValue(ResizeMode.AutoFit)]
		[SRCategory("CategoryBehavior")]
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

		[SRDescription("DescriptionAttributeFontUnit3")]
		[DefaultValue(FontUnit.Default)]
		[SRCategory("CategoryAppearance")]
		public FontUnit FontUnit
		{
			get
			{
				return this.fontUnit;
			}
			set
			{
				this.fontUnit = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeLabel_BorderColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Black")]
		public Color BorderColor
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

		[NotifyParentProperty(true)]
		[DefaultValue(GaugeDashStyle.NotSet)]
		[SRDescription("DescriptionAttributeGaugeLabel_BorderStyle")]
		[SRCategory("CategoryAppearance")]
		public GaugeDashStyle BorderStyle
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

		[SRCategory("CategoryAppearance")]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeGaugeLabel_BorderWidth")]
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
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
			}
		}

		[SRDescription("DescriptionAttributeGaugeLabel_BackColor")]
		[DefaultValue(typeof(Color), "Empty")]
		[SRCategory("CategoryAppearance")]
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
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeGaugeLabel_TextColor")]
		[DefaultValue(typeof(Color), "Black")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAppearance")]
		public Color TextColor
		{
			get
			{
				return this.textColor;
			}
			set
			{
				this.textColor = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[DefaultValue(GradientType.None)]
		[SRDescription("DescriptionAttributeGaugeLabel_BackGradientType")]
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
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeGaugeLabel_BackGradientEndColor")]
		[DefaultValue(typeof(Color), "Empty")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAppearance")]
		public Color BackGradientEndColor
		{
			get
			{
				return this.backGradientEndColor;
			}
			set
			{
				this.backGradientEndColor = value;
				this.Invalidate();
			}
		}

		[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(GaugeHatchStyle.None)]
		[SRDescription("DescriptionAttributeGaugeLabel_BackHatchStyle")]
		public GaugeHatchStyle BackHatchStyle
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

		[SRCategory("CategoryBehavior")]
		[DefaultValue("Text")]
		[Localizable(true)]
		[SRDescription("DescriptionAttributeGaugeLabel_Text")]
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[NotifyParentProperty(true)]
		[ValidateBound(-5.0, 5.0)]
		[SRDescription("DescriptionAttributeGaugeLabel_BackShadowOffset")]
		[DefaultValue(0)]
		public int BackShadowOffset
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
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
			}
		}

		[ValidateBound(-5.0, 5.0)]
		[SRDescription("DescriptionAttributeGaugeLabel_TextShadowOffset")]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(0)]
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
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
			}
		}

		[ValidateBound(0.0, 360.0)]
		[DefaultValue(0f)]
		[SRCategory("CategoryPosition")]
		[SRDescription("DescriptionAttributeGaugeLabel_Angle")]
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

		[SRCategory("CategoryAppearance")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeGaugeLabel_Selected")]
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

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Bindable(false)]
		[Browsable(false)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeParentObject3")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

		public GaugeLabel()
		{
			this.location = new GaugeLocation(this, 26f, 47f);
			this.size = new GaugeSize(this, 12f, 6f);
			this.location.DefaultValues = true;
			this.size.DefaultValues = true;
		}

		public override string ToString()
		{
			return this.Name;
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

		internal GraphicsPath GetTextPath(GaugeGraphics g)
		{
			if (!this.Visible)
			{
				return null;
			}
			if (this.Text.Length == 0)
			{
				return null;
			}
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			GraphicsPath graphicsPath = new GraphicsPath();
			string text = this.Text;
			Font font = this.Font;
			text = text.Replace("\\n", "\n");
			float emSize;
			if (this.ResizeMode == ResizeMode.AutoFit)
			{
				SizeF sizeF = g.MeasureString(text, font);
				SizeF absoluteSize = g.GetAbsoluteSize(new SizeF(100f, 100f));
				float num = absoluteSize.Width / sizeF.Width;
				float num2 = absoluteSize.Height / sizeF.Height;
				emSize = (float)((!(num < num2)) ? (font.SizeInPoints * num2 * 1.2999999523162842 * g.Graphics.DpiY / 96.0) : (font.SizeInPoints * num * 1.2999999523162842 * g.Graphics.DpiY / 96.0));
			}
			else
			{
				if (this.FontUnit == FontUnit.Percent)
				{
					g.RestoreDrawRegion();
					emSize = g.GetAbsoluteDimension(font.Size);
					RectangleF boundRect = ((IRenderable)this).GetBoundRect(g);
					g.CreateDrawRegion(boundRect);
				}
				else
				{
					emSize = (float)(font.SizeInPoints * g.Graphics.DpiY / 96.0);
				}
				emSize = (float)(emSize * 1.2999999523162842);
			}
			StringFormat stringFormat = new StringFormat();
			if (this.TextAlignment == ContentAlignment.TopLeft)
			{
				stringFormat.Alignment = StringAlignment.Near;
				stringFormat.LineAlignment = StringAlignment.Near;
			}
			else if (this.TextAlignment == ContentAlignment.TopCenter)
			{
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Near;
			}
			else if (this.TextAlignment == ContentAlignment.TopRight)
			{
				stringFormat.Alignment = StringAlignment.Far;
				stringFormat.LineAlignment = StringAlignment.Near;
			}
			else if (this.TextAlignment == ContentAlignment.MiddleLeft)
			{
				stringFormat.Alignment = StringAlignment.Near;
				stringFormat.LineAlignment = StringAlignment.Center;
			}
			else if (this.TextAlignment == ContentAlignment.MiddleCenter)
			{
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;
			}
			else if (this.TextAlignment == ContentAlignment.MiddleRight)
			{
				stringFormat.Alignment = StringAlignment.Far;
				stringFormat.LineAlignment = StringAlignment.Center;
			}
			else if (this.TextAlignment == ContentAlignment.BottomLeft)
			{
				stringFormat.Alignment = StringAlignment.Near;
				stringFormat.LineAlignment = StringAlignment.Far;
			}
			else if (this.TextAlignment == ContentAlignment.BottomCenter)
			{
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Far;
			}
			else
			{
				stringFormat.Alignment = StringAlignment.Far;
				stringFormat.LineAlignment = StringAlignment.Far;
			}
			graphicsPath.AddString(text, font.FontFamily, (int)font.Style, emSize, absoluteRectangle, stringFormat);
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

		internal Brush GetBackBrush(GaugeGraphics g)
		{
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			Brush brush = null;
			Color color = this.BackColor;
			Color color2 = this.BackGradientEndColor;
			GradientType gradientType = this.BackGradientType;
			GaugeHatchStyle gaugeHatchStyle = this.BackHatchStyle;
			if (gaugeHatchStyle != 0)
			{
				brush = GaugeGraphics.GetHatchBrush(gaugeHatchStyle, color, color2);
			}
			else if (gradientType != 0)
			{
				brush = g.GetGradientBrush(absoluteRectangle, color, color2, gradientType);
				if (this.Angle != 0.0)
				{
					PointF pointF = new PointF((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0), (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0));
					if (brush is LinearGradientBrush)
					{
						((LinearGradientBrush)brush).TranslateTransform((float)(0.0 - pointF.X), (float)(0.0 - pointF.Y), MatrixOrder.Append);
						((LinearGradientBrush)brush).RotateTransform(this.Angle, MatrixOrder.Append);
						((LinearGradientBrush)brush).TranslateTransform(pointF.X, pointF.Y, MatrixOrder.Append);
					}
					else if (brush is PathGradientBrush)
					{
						((PathGradientBrush)brush).TranslateTransform((float)(0.0 - pointF.X), (float)(0.0 - pointF.Y), MatrixOrder.Append);
						((PathGradientBrush)brush).RotateTransform(this.Angle, MatrixOrder.Append);
						((PathGradientBrush)brush).TranslateTransform(pointF.X, pointF.Y, MatrixOrder.Append);
					}
				}
			}
			else
			{
				brush = new SolidBrush(color);
			}
			return brush;
		}

		internal Pen GetPen(GaugeGraphics g)
		{
			if (this.BorderWidth > 0 && this.BorderStyle != 0)
			{
				Color borderColor2 = this.BorderColor;
				int borderWidth2 = this.BorderWidth;
				GaugeDashStyle borderStyle2 = this.BorderStyle;
				Pen pen = new Pen(this.BorderColor, (float)this.BorderWidth);
				pen.DashStyle = g.GetPenStyle(this.BorderStyle);
				pen.Alignment = PenAlignment.Center;
				return pen;
			}
			return null;
		}

		void IRenderable.RenderStaticElements(GaugeGraphics g)
		{
		}

		void IRenderable.RenderDynamicElements(GaugeGraphics g)
		{
			if (this.Visible)
			{
				g.StartHotRegion(this);
				GraphicsPath graphicsPath = null;
				GraphicsPath graphicsPath2 = null;
				Brush brush = null;
				Brush brush2 = null;
				Brush brush3 = null;
				Pen pen = null;
				try
				{
					graphicsPath = this.GetBackPath(g);
					graphicsPath2 = this.GetTextPath(g);
					if (graphicsPath != null)
					{
						if (this.BackShadowOffset != 0)
						{
							using (Matrix matrix = new Matrix())
							{
								brush3 = g.GetShadowBrush();
								matrix.Translate((float)this.BackShadowOffset, (float)this.BackShadowOffset, MatrixOrder.Append);
								graphicsPath.Transform(matrix);
								g.FillPath(brush3, graphicsPath);
								matrix.Reset();
								matrix.Translate((float)(-this.BackShadowOffset), (float)(-this.BackShadowOffset), MatrixOrder.Append);
								graphicsPath.Transform(matrix);
							}
						}
						brush2 = this.GetBackBrush(g);
						g.FillPath(brush2, graphicsPath);
						pen = this.GetPen(g);
						if (pen != null)
						{
							g.DrawPath(pen, graphicsPath);
						}
						this.Common.GaugeCore.HotRegionList.SetHotRegion(this, Point.Empty, (GraphicsPath)graphicsPath.Clone());
					}
					if (graphicsPath2 != null)
					{
						if (this.TextShadowOffset != 0)
						{
							using (Matrix matrix2 = new Matrix())
							{
								brush3 = g.GetShadowBrush();
								matrix2.Translate((float)this.TextShadowOffset, (float)this.TextShadowOffset, MatrixOrder.Append);
								graphicsPath2.Transform(matrix2);
								g.FillPath(brush3, graphicsPath2);
								matrix2.Reset();
								matrix2.Translate((float)(-this.TextShadowOffset), (float)(-this.TextShadowOffset), MatrixOrder.Append);
								graphicsPath2.Transform(matrix2);
							}
						}
						brush = new SolidBrush(this.TextColor);
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
						g.FillPath(brush, graphicsPath2);
						g.AntiAliasing = antiAliasing2;
					}
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
					if (brush3 != null)
					{
						brush3.Dispose();
					}
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
			GaugeLabel gaugeLabel = new GaugeLabel();
			binaryFormatSerializer.Deserialize(gaugeLabel, stream);
			return gaugeLabel;
		}
	}
}
