using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(MapLabelConverter))]
	internal class MapLabel : AutoSizePanel
	{
		private ContentAlignment textAlignment = ContentAlignment.TopCenter;

		private Font font = new Font("Microsoft Sans Serif", 12f);

		private Color textColor = Color.Black;

		private string text = "Text";

		private int textShadowOffset;

		private float angle = float.NaN;

		[Localizable(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeMapLabel_ToolTip")]
		[Browsable(false)]
		public override string ToolTip
		{
			get
			{
				return base.ToolTip;
			}
			set
			{
				base.ToolTip = value;
			}
		}

		[SRDescription("DescriptionAttributeMapLabel_Href")]
		[SRCategory("CategoryAttribute_Behavior")]
		[Localizable(true)]
		[DefaultValue("")]
		public override string Href
		{
			get
			{
				return base.Href;
			}
			set
			{
				base.Href = value;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapLabel_MapAreaAttributes")]
		[DefaultValue("")]
		public override string MapAreaAttributes
		{
			get
			{
				return base.MapAreaAttributes;
			}
			set
			{
				base.MapAreaAttributes = value;
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[SerializationVisibility(SerializationVisibility.Attribute)]
		[SRDescription("DescriptionAttributeMapLabel_Name")]
		[Browsable(true)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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

		[DefaultValue(ContentAlignment.TopCenter)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapLabel_TextAlignment")]
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

		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeMapLabel_Visible")]
		[ParenthesizePropertyName(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				base.Visible = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapLabel_Font")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 12pt")]
		public Font Font
		{
			get
			{
				return this.font;
			}
			set
			{
				this.font = value;
				base.Invalidate(true);
			}
		}

		[DefaultValue(MapDashStyle.Solid)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeMapLabel_BorderStyle")]
		[SRCategory("CategoryAttribute_Appearance")]
		public override MapDashStyle BorderStyle
		{
			get
			{
				return base.BorderStyle;
			}
			set
			{
				base.BorderStyle = value;
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapLabel_BackColor")]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeMapLabel_TextColor")]
		[SRCategory("CategoryAttribute_Appearance")]
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
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeMapLabel_BackGradientType")]
		public override GradientType BackGradientType
		{
			get
			{
				return base.BackGradientType;
			}
			set
			{
				base.BackGradientType = value;
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeMapLabel_BackSecondaryColor")]
		[SRCategory("CategoryAttribute_Appearance")]
		public override Color BackSecondaryColor
		{
			get
			{
				return base.BackSecondaryColor;
			}
			set
			{
				base.BackSecondaryColor = value;
			}
		}

		[NotifyParentProperty(true)]
		//[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapLabel_BackHatchStyle")]
		public override MapHatchStyle BackHatchStyle
		{
			get
			{
				return base.BackHatchStyle;
			}
			set
			{
				base.BackHatchStyle = value;
			}
		}

		[Localizable(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue("Text")]
		[SRDescription("DescriptionAttributeMapLabel_Text")]
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
				base.Invalidate(true);
			}
		}

		[DefaultValue(0)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapLabel_BackShadowOffset")]
		[NotifyParentProperty(true)]
		public override int BackShadowOffset
		{
			get
			{
				return base.BackShadowOffset;
			}
			set
			{
				base.BackShadowOffset = value;
			}
		}

		[DefaultValue(0)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapLabel_TextShadowOffset")]
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
				throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
			}
		}

		[TypeConverter(typeof(FloatAutoValueConverter))]
		[SRCategory("CategoryAttribute_Position")]
		[SRDescription("DescriptionAttributeMapLabel_Angle")]
		[DefaultValue(float.NaN)]
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
					base.Invalidate(true);
					return;
				}
				throw new ArgumentOutOfRangeException(SR.out_of_range(0.0, 360.0));
			}
		}

		internal Position Position
		{
			get
			{
				return new Position(this.Location, this.Size, ContentAlignment.TopLeft);
			}
		}

		internal override bool IsEmpty
		{
			get
			{
				if (this.Common != null && this.Common.MapCore.IsDesignMode())
				{
					return false;
				}
				return string.IsNullOrEmpty(this.Text);
			}
		}

		public MapLabel()
			: this(null)
		{
		}

		internal MapLabel(CommonElements common)
			: base(common)
		{
			this.BackShadowOffset = 0;
			this.Location.DefaultValues = true;
			this.Size.DefaultValues = true;
			this.BorderStyle = MapDashStyle.Solid;
			this.Visible = true;
		}

		public override string ToString()
		{
			return this.Name;
		}

		internal override bool ShouldRenderBackground()
		{
			return true;
		}

		internal override void Render(MapGraphics g)
		{
			if (this.IsVisible())
			{
				if (this.TextShadowOffset != 0)
				{
					this.DrawText(g, true);
				}
				this.DrawText(g, false);
			}
		}

		private void DrawText(MapGraphics g, bool drawShadow)
		{
			if (!string.IsNullOrEmpty(this.Text))
			{
				RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
				string text = this.Text;
				Font font2 = this.Font;
				text = text.Replace("\\n", "\n");
				StringFormat stringFormat = this.GetStringFormat();
				TextRenderingHint textRenderingHint = g.TextRenderingHint;
				float num = this.DetermineAngle();
				if (num % 90.0 != 0.0)
				{
					g.TextRenderingHint = TextRenderingHint.AntiAlias;
				}
				Brush brush = null;
				brush = ((!drawShadow) ? new SolidBrush(this.TextColor) : g.GetShadowBrush());
				try
				{
					if (num != 0.0)
					{
						RectangleF layoutRectangle = this.DetermineTextRectangle(g, stringFormat);
						PointF point = new PointF((float)(layoutRectangle.X + layoutRectangle.Width / 2.0), (float)(layoutRectangle.Y + layoutRectangle.Height / 2.0));
						Matrix transform = g.Transform;
						Matrix matrix = g.Transform.Clone();
						matrix.RotateAt(num, point, MatrixOrder.Prepend);
						if (drawShadow)
						{
							matrix.Translate((float)this.TextShadowOffset, (float)this.TextShadowOffset, MatrixOrder.Append);
						}
						g.Transform = matrix;
						StringFormat stringFormat2 = new StringFormat();
						stringFormat2.Alignment = StringAlignment.Center;
						stringFormat2.LineAlignment = StringAlignment.Center;
						stringFormat2.Trimming = StringTrimming.EllipsisCharacter;
						layoutRectangle.Inflate(1000f, 1000f);
						g.DrawString(text, this.Font, brush, layoutRectangle, stringFormat2);
						g.Transform = transform;
					}
					else
					{
						if (drawShadow)
						{
							absoluteRectangle.X += (float)this.TextShadowOffset;
							absoluteRectangle.Y += (float)this.TextShadowOffset;
						}
						g.DrawString(text, this.Font, brush, absoluteRectangle, stringFormat);
					}
				}
				finally
				{
					if (brush != null)
					{
						brush.Dispose();
					}
				}
				g.Graphics.TextRenderingHint = textRenderingHint;
			}
		}

		private RectangleF DetermineTextRectangle(MapGraphics g, StringFormat stringFormat)
		{
			RectangleF result = default(RectangleF);
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			SizeF sizeF = this.DetermineTextSizeAfterRotation(g);
			if (stringFormat.Alignment == StringAlignment.Near)
			{
				result.X = absoluteRectangle.X;
			}
			else if (stringFormat.Alignment == StringAlignment.Center)
			{
				result.X = (float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0 - sizeF.Width / 2.0);
			}
			else if (stringFormat.Alignment == StringAlignment.Far)
			{
				result.X = absoluteRectangle.Right - sizeF.Width;
			}
			if (stringFormat.LineAlignment == StringAlignment.Near)
			{
				result.Y = absoluteRectangle.Y;
			}
			else if (stringFormat.LineAlignment == StringAlignment.Center)
			{
				result.Y = (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0 - sizeF.Height / 2.0);
			}
			else if (stringFormat.LineAlignment == StringAlignment.Far)
			{
				result.Y = absoluteRectangle.Bottom - sizeF.Height;
			}
			result.Width = sizeF.Width;
			result.Height = sizeF.Height;
			return result;
		}

		private StringFormat GetStringFormat()
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
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
			return stringFormat;
		}

		internal GraphicsPath GetPath(MapGraphics g)
		{
			if (!this.IsVisible())
			{
				return null;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			graphicsPath.AddRectangle(absoluteRectangle);
			float num = this.DetermineAngle();
			if (num != 0.0)
			{
				PointF point = new PointF((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0), (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0));
				using (Matrix matrix = new Matrix())
				{
					matrix.RotateAt(num, point);
					graphicsPath.Transform(matrix);
					return graphicsPath;
				}
			}
			return graphicsPath;
		}

		internal Brush GetBackBrush(MapGraphics g)
		{
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			Brush brush = null;
			Color backColor = this.BackColor;
			Color backSecondaryColor = this.BackSecondaryColor;
			GradientType backGradientType = this.BackGradientType;
			MapHatchStyle backHatchStyle = this.BackHatchStyle;
			if (backHatchStyle != 0)
			{
				brush = MapGraphics.GetHatchBrush(backHatchStyle, backColor, backSecondaryColor);
			}
			else if (backGradientType != 0)
			{
				brush = g.GetGradientBrush(absoluteRectangle, backColor, backSecondaryColor, backGradientType);
				float num = this.DetermineAngle();
				if (num != 0.0)
				{
					PointF pointF = new PointF((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0), (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0));
					if (brush is LinearGradientBrush)
					{
						((LinearGradientBrush)brush).TranslateTransform((float)(0.0 - pointF.X), (float)(0.0 - pointF.Y), MatrixOrder.Append);
						((LinearGradientBrush)brush).RotateTransform(num, MatrixOrder.Append);
						((LinearGradientBrush)brush).TranslateTransform(pointF.X, pointF.Y, MatrixOrder.Append);
					}
					else if (brush is PathGradientBrush)
					{
						((PathGradientBrush)brush).TranslateTransform((float)(0.0 - pointF.X), (float)(0.0 - pointF.Y), MatrixOrder.Append);
						((PathGradientBrush)brush).RotateTransform(num, MatrixOrder.Append);
						((PathGradientBrush)brush).TranslateTransform(pointF.X, pointF.Y, MatrixOrder.Append);
					}
				}
			}
			else
			{
				brush = new SolidBrush(backColor);
			}
			return brush;
		}

		internal Pen GetPen()
		{
			if (this.BorderWidth > 0 && this.BorderStyle != 0)
			{
				Color borderColor = this.BorderColor;
				int borderWidth = this.BorderWidth;
				MapDashStyle borderStyle = this.BorderStyle;
				Pen pen = new Pen(this.BorderColor, (float)this.BorderWidth);
				pen.DashStyle = MapGraphics.GetPenStyle(this.BorderStyle);
				pen.Alignment = PenAlignment.Center;
				return pen;
			}
			return null;
		}

		internal override object GetDefaultPropertyValue(string prop, object currentValue)
		{
			object obj = null;
			switch (prop)
			{
			case "Size":
				return new MapSize(null, 100f, 10f);
			case "SizeUnit":
				return CoordinateUnit.Percent;
			case "Dock":
				return PanelDockStyle.Top;
			case "DockAlignment":
				return DockAlignment.Center;
			case "BackColor":
				return Color.Empty;
			case "BackGradientType":
				return GradientType.None;
			case "BorderWidth":
				return 0;
			default:
				return base.GetDefaultPropertyValue(prop, currentValue);
			}
		}

		internal override SizeF GetOptimalSize(MapGraphics g, SizeF maxSizeAbs)
		{
			SizeF result = this.DetermineTextSizeAfterRotation(g);
			SizeF sizeF;
			if (base.DockedInsideViewport)
			{
				sizeF = this.Common.MapCore.Viewport.GetSizeInPixels();
				sizeF.Width -= (float)(this.Common.MapCore.Viewport.Margins.Left + this.Common.MapCore.Viewport.Margins.Right);
				sizeF.Height -= (float)(this.Common.MapCore.Viewport.Margins.Top + this.Common.MapCore.Viewport.Margins.Bottom);
			}
			else
			{
				sizeF = this.Common.MapCore.CalculateMapDockBounds(g).Size;
			}
			if (this.Dock == PanelDockStyle.Top || this.Dock == PanelDockStyle.Bottom)
			{
				result.Width = sizeF.Width - (float)base.Margins.Left - (float)base.Margins.Right;
			}
			else if (this.Dock == PanelDockStyle.Left || this.Dock == PanelDockStyle.Right)
			{
				result.Height = sizeF.Height - (float)base.Margins.Top - (float)base.Margins.Bottom;
			}
			return result;
		}

		private SizeF DetermineTextSizeAfterRotation(MapGraphics g)
		{
			string text = this.Text.Replace("\\n", "\n");
			SizeF unrotatedSize = g.MeasureString(text, this.Font, new SizeF(0f, 0f), this.GetStringFormat());
			unrotatedSize.Width += 1f;
			unrotatedSize.Height += 1f;
			return this.CalculateRotatedSize(unrotatedSize, this.DetermineAngle());
		}

		internal SizeF CalculateRotatedSize(SizeF unrotatedSize, float andgleOfRotation)
		{
			andgleOfRotation = (float)(andgleOfRotation % 180.0);
			if (andgleOfRotation > 90.0)
			{
				andgleOfRotation = (float)(andgleOfRotation % 90.0);
				float width = unrotatedSize.Width;
				unrotatedSize.Width = unrotatedSize.Height;
				unrotatedSize.Height = width;
			}
			double num = (double)andgleOfRotation * 3.1415926535897931 / 180.0;
			double num2 = Math.Cos(num);
			double num3 = Math.Cos(1.5707963267948966 - num);
			return new SizeF((float)Math.Abs((double)unrotatedSize.Width * num2 + (double)unrotatedSize.Height * num3), (float)Math.Abs((double)unrotatedSize.Height * num2 + (double)unrotatedSize.Width * num3));
		}

		internal float DetermineAngle()
		{
			if (double.IsNaN((double)this.Angle))
			{
				if (this.Dock == PanelDockStyle.None)
				{
					SizeF sizeInPixels = base.GetSizeInPixels();
					if (sizeInPixels.Width >= sizeInPixels.Height)
					{
						return 0f;
					}
					return 90f;
				}
				if (this.Dock == PanelDockStyle.Left)
				{
					return 270f;
				}
				if (this.Dock == PanelDockStyle.Right)
				{
					return 90f;
				}
				return 0f;
			}
			return this.Angle;
		}
	}
}
