using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeTextAnnotation_TextAnnotation")]
	internal class TextAnnotation : Annotation
	{
		private string text = "";

		private bool multiline;

		internal SizeF contentSize = SizeF.Empty;

		internal bool isEllipse;

		internal bool restrictedPermissions;

		[DefaultValue("")]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeText4")]
		public virtual string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
				base.Invalidate();
				this.contentSize = SizeF.Empty;
			}
		}

		[SRDescription("DescriptionAttributeMultiline")]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(false)]
		public virtual bool Multiline
		{
			get
			{
				return this.multiline;
			}
			set
			{
				this.multiline = value;
				base.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeTextFont4")]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		public override Font TextFont
		{
			get
			{
				return base.TextFont;
			}
			set
			{
				base.TextFont = value;
				this.contentSize = SizeF.Empty;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		[Browsable(false)]
		[DefaultValue(typeof(Color), "Black")]
		public override Color LineColor
		{
			get
			{
				return base.LineColor;
			}
			set
			{
				base.LineColor = value;
			}
		}

		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		public override int LineWidth
		{
			get
			{
				return base.LineWidth;
			}
			set
			{
				base.LineWidth = value;
			}
		}

		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		public override ChartDashStyle LineStyle
		{
			get
			{
				return base.LineStyle;
			}
			set
			{
				base.LineStyle = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		[NotifyParentProperty(true)]
		[Browsable(false)]
		[DefaultValue(typeof(Color), "")]
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
		[DefaultValue(ChartHatchStyle.None)]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		public override ChartHatchStyle BackHatchStyle
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

		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		[DefaultValue(GradientType.None)]
		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[Browsable(false)]
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

		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		public override Color BackGradientEndColor
		{
			get
			{
				return base.BackGradientEndColor;
			}
			set
			{
				base.BackGradientEndColor = value;
			}
		}

		[SRDescription("DescriptionAttributeTextAnnotation_AnnotationType")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[Browsable(false)]
		public override string AnnotationType
		{
			get
			{
				return "Text";
			}
		}

		[Browsable(false)]
		[SRDescription("DescriptionAttributeSelectionPointsStyle3")]
		[DefaultValue(SelectionPointsStyle.Rectangle)]
		[ParenthesizePropertyName(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		internal override SelectionPointsStyle SelectionPointsStyle
		{
			get
			{
				return SelectionPointsStyle.Rectangle;
			}
		}

		internal override void Paint(Chart chart, ChartGraphics graphics)
		{
			this.Chart = chart;
			PointF empty = PointF.Empty;
			PointF empty2 = PointF.Empty;
			SizeF empty3 = SizeF.Empty;
			((Annotation)this).GetRelativePosition(out empty, out empty3, out empty2);
			PointF pointF = new PointF(empty.X + empty3.Width, empty.Y + empty3.Height);
			RectangleF rect = new RectangleF(empty, new SizeF(pointF.X - empty.X, pointF.Y - empty.Y));
			RectangleF rectangleF = new RectangleF(rect.Location, rect.Size);
			if (rectangleF.Width < 0.0)
			{
				rectangleF.X = rectangleF.Right;
				rectangleF.Width = (float)(0.0 - rectangleF.Width);
			}
			if (rectangleF.Height < 0.0)
			{
				rectangleF.Y = rectangleF.Bottom;
				rectangleF.Height = (float)(0.0 - rectangleF.Height);
			}
			if (!rectangleF.IsEmpty && !float.IsNaN(rectangleF.X) && !float.IsNaN(rectangleF.Y) && !float.IsNaN(rectangleF.Right) && !float.IsNaN(rectangleF.Bottom))
			{
				if (this.Chart.chartPicture.common.ProcessModePaint)
				{
					this.DrawText(graphics, rectangleF, false, false);
				}
				if (this.Chart.chartPicture.common.ProcessModeRegions)
				{
					if (this.isEllipse)
					{
						GraphicsPath graphicsPath = new GraphicsPath();
						graphicsPath.AddEllipse(rectangleF);
						this.Chart.chartPicture.common.HotRegionsList.AddHotRegion(graphics, graphicsPath, true, base.ReplaceKeywords(this.ToolTip), base.ReplaceKeywords(this.Href), base.ReplaceKeywords(this.MapAreaAttributes), this, ChartElementType.Annotation);
					}
					else
					{
						this.Chart.chartPicture.common.HotRegionsList.AddHotRegion(graphics, rectangleF, base.ReplaceKeywords(this.ToolTip), base.ReplaceKeywords(this.Href), base.ReplaceKeywords(this.MapAreaAttributes), this, ChartElementType.Annotation, string.Empty);
					}
				}
				this.PaintSelectionHandles(graphics, rect, null);
			}
		}

		internal RectangleF DrawText(ChartGraphics graphics, RectangleF textPosition, bool noSpacingForCenteredText, bool getTextPosition)
		{
			RectangleF result = RectangleF.Empty;
			bool flag = false;
			RectangleF textSpacing = this.GetTextSpacing(out flag);
			float num = 1f;
			float num2 = 1f;
			if (flag)
			{
				if (textPosition.Width > 25.0)
				{
					num = (float)(textPosition.Width / 50.0);
					num = Math.Max(1f, num);
				}
				if (textPosition.Height > 25.0)
				{
					num2 = (float)(textPosition.Height / 50.0);
					num2 = Math.Max(1f, num2);
				}
			}
			RectangleF rectangleF = new RectangleF(textPosition.Location, textPosition.Size);
			rectangleF.Width -= (textSpacing.Width + textSpacing.X) * num;
			rectangleF.X += textSpacing.X * num;
			rectangleF.Height -= (textSpacing.Height + textSpacing.Y) * num2;
			rectangleF.Y += textSpacing.Y * num2;
			string text = base.ReplaceKeywords(this.Text.Replace("\\n", "\n"));
			if (noSpacingForCenteredText && text.IndexOf('\n') == -1)
			{
				if (this.Alignment == ContentAlignment.MiddleCenter || this.Alignment == ContentAlignment.MiddleLeft || this.Alignment == ContentAlignment.MiddleRight)
				{
					rectangleF.Y = textPosition.Y;
					rectangleF.Height = textPosition.Height;
					rectangleF.Height -= (float)(textSpacing.Height / 2.0 + textSpacing.Y / 2.0);
					rectangleF.Y += (float)(textSpacing.Y / 2.0);
				}
				if (this.Alignment == ContentAlignment.BottomCenter || this.Alignment == ContentAlignment.MiddleCenter || this.Alignment == ContentAlignment.TopCenter)
				{
					rectangleF.X = textPosition.X;
					rectangleF.Width = textPosition.Width;
					rectangleF.Width -= (float)(textSpacing.Width / 2.0 + textSpacing.X / 2.0);
					rectangleF.X += (float)(textSpacing.X / 2.0);
				}
			}
			using (Brush brush = new SolidBrush(this.TextColor))
			{
				StringFormat stringFormat = new StringFormat(StringFormat.GenericTypographic);
				stringFormat.FormatFlags ^= StringFormatFlags.LineLimit;
				stringFormat.Trimming = StringTrimming.EllipsisCharacter;
				if (this.Alignment == ContentAlignment.BottomRight || this.Alignment == ContentAlignment.MiddleRight || this.Alignment == ContentAlignment.TopRight)
				{
					stringFormat.Alignment = StringAlignment.Far;
				}
				if (this.Alignment == ContentAlignment.BottomCenter || this.Alignment == ContentAlignment.MiddleCenter || this.Alignment == ContentAlignment.TopCenter)
				{
					stringFormat.Alignment = StringAlignment.Center;
				}
				if (this.Alignment == ContentAlignment.BottomCenter || this.Alignment == ContentAlignment.BottomLeft || this.Alignment == ContentAlignment.BottomRight)
				{
					stringFormat.LineAlignment = StringAlignment.Far;
				}
				if (this.Alignment == ContentAlignment.MiddleCenter || this.Alignment == ContentAlignment.MiddleLeft || this.Alignment == ContentAlignment.MiddleRight)
				{
					stringFormat.LineAlignment = StringAlignment.Center;
				}
				Color color = ChartGraphics.GetGradientColor(this.TextColor, Color.Black, 0.8);
				int num3 = 1;
				TextStyle textStyle = this.TextStyle;
				if (textStyle == TextStyle.Shadow && this.ShadowOffset != 0)
				{
					color = this.ShadowColor;
					num3 = this.ShadowOffset;
				}
				if (getTextPosition)
				{
					SizeF size = graphics.MeasureStringRel(base.ReplaceKeywords(this.text.Replace("\\n", "\n")), this.TextFont, rectangleF.Size, stringFormat);
					result = new RectangleF(rectangleF.Location, size);
					if (this.Alignment == ContentAlignment.BottomRight || this.Alignment == ContentAlignment.MiddleRight || this.Alignment == ContentAlignment.TopRight)
					{
						result.X += rectangleF.Width - size.Width;
					}
					if (this.Alignment == ContentAlignment.BottomCenter || this.Alignment == ContentAlignment.MiddleCenter || this.Alignment == ContentAlignment.TopCenter)
					{
						result.X += (float)((rectangleF.Width - size.Width) / 2.0);
					}
					if (this.Alignment == ContentAlignment.BottomCenter || this.Alignment == ContentAlignment.BottomLeft || this.Alignment == ContentAlignment.BottomRight)
					{
						result.Y += rectangleF.Height - size.Height;
					}
					if (this.Alignment == ContentAlignment.MiddleCenter || this.Alignment == ContentAlignment.MiddleLeft || this.Alignment == ContentAlignment.MiddleRight)
					{
						result.Y += (float)((rectangleF.Height - size.Height) / 2.0);
					}
					result.Intersect(rectangleF);
				}
				RectangleF absoluteRectangle = graphics.GetAbsoluteRectangle(rectangleF);
				switch (textStyle)
				{
				case TextStyle.Default:
					graphics.DrawStringRel(text, this.TextFont, brush, rectangleF, stringFormat);
					return result;
				case TextStyle.Frame:
					using (GraphicsPath graphicsPath = new GraphicsPath())
					{
						graphicsPath.AddString(text, this.TextFont.FontFamily, (int)this.TextFont.Style, (float)(this.TextFont.Size * 1.2999999523162842), absoluteRectangle, stringFormat);
						graphicsPath.CloseAllFigures();
						graphics.DrawPath(new Pen(this.TextColor, 1f), graphicsPath);
						return result;
					}
				case TextStyle.Embed:
				{
					RectangleF layoutRectangle3 = new RectangleF(absoluteRectangle.Location, absoluteRectangle.Size);
					layoutRectangle3.X -= 1f;
					layoutRectangle3.Y -= 1f;
					graphics.DrawString(text, this.TextFont, brush, layoutRectangle3, stringFormat);
					layoutRectangle3.X += 2f;
					layoutRectangle3.Y += 2f;
					Color gradientColor2 = ChartGraphics.GetGradientColor(Color.White, this.TextColor, 0.3);
					graphics.DrawString(text, this.TextFont, new SolidBrush(gradientColor2), layoutRectangle3, stringFormat);
					graphics.DrawString(text, this.TextFont, brush, absoluteRectangle, stringFormat);
					return result;
				}
				case TextStyle.Emboss:
				{
					RectangleF layoutRectangle2 = new RectangleF(absoluteRectangle.Location, absoluteRectangle.Size);
					layoutRectangle2.X += 1f;
					layoutRectangle2.Y += 1f;
					graphics.DrawString(text, this.TextFont, new SolidBrush(color), layoutRectangle2, stringFormat);
					layoutRectangle2.X -= 2f;
					layoutRectangle2.Y -= 2f;
					Color gradientColor = ChartGraphics.GetGradientColor(Color.White, this.TextColor, 0.3);
					graphics.DrawString(text, this.TextFont, new SolidBrush(gradientColor), layoutRectangle2, stringFormat);
					graphics.DrawString(text, this.TextFont, brush, absoluteRectangle, stringFormat);
					return result;
				}
				case TextStyle.Shadow:
				{
					RectangleF layoutRectangle = new RectangleF(absoluteRectangle.Location, absoluteRectangle.Size);
					layoutRectangle.X += (float)num3;
					layoutRectangle.Y += (float)num3;
					graphics.DrawString(text, this.TextFont, new SolidBrush(color), layoutRectangle, stringFormat);
					graphics.DrawString(text, this.TextFont, brush, absoluteRectangle, stringFormat);
					return result;
				}
				default:
					throw new InvalidOperationException(SR.ExceptionAnnotationTextDrawingStyleUnknown);
				}
			}
		}

		internal override RectangleF GetContentPosition()
		{
			if (!this.contentSize.IsEmpty)
			{
				return new RectangleF(float.NaN, float.NaN, this.contentSize.Width, this.contentSize.Height);
			}
			Graphics graphics = null;
			Image image = null;
			ChartGraphics chartGraphics = null;
			if (base.GetGraphics() == null && base.chart != null && base.chart.chartPicture != null && base.chart.chartPicture.common != null)
			{
				image = new Bitmap(base.chart.chartPicture.Width, base.chart.chartPicture.Height);
				graphics = Graphics.FromImage(image);
				chartGraphics = new ChartGraphics(base.chart.chartPicture.common);
				chartGraphics.Graphics = graphics;
				chartGraphics.SetPictureSize(base.chart.chartPicture.Width, base.chart.chartPicture.Height);
				base.chart.chartPicture.common.graph = chartGraphics;
			}
			RectangleF result = RectangleF.Empty;
			if (base.GetGraphics() != null && this.Text.Trim().Length > 0)
			{
				StringFormat stringFormat = new StringFormat(StringFormat.GenericTypographic);
				this.contentSize = base.GetGraphics().MeasureString("W" + base.ReplaceKeywords(this.Text.Replace("\\n", "\n")), this.TextFont, new SizeF(2000f, 2000f), stringFormat);
				this.contentSize.Height *= 1.04f;
				this.contentSize = base.GetGraphics().GetRelativeSize(this.contentSize);
				bool flag = false;
				RectangleF textSpacing = this.GetTextSpacing(out flag);
				float num = 1f;
				float num2 = 1f;
				if (flag)
				{
					if (this.contentSize.Width > 25.0)
					{
						num = (float)(this.contentSize.Width / 25.0);
						num = Math.Max(1f, num);
					}
					if (this.contentSize.Height > 25.0)
					{
						num2 = (float)(this.contentSize.Height / 25.0);
						num2 = Math.Max(1f, num2);
					}
				}
				this.contentSize.Width += (textSpacing.X + textSpacing.Width) * num;
				this.contentSize.Height += (textSpacing.Y + textSpacing.Height) * num2;
				result = new RectangleF(float.NaN, float.NaN, this.contentSize.Width, this.contentSize.Height);
			}
			if (chartGraphics != null)
			{
				chartGraphics.Dispose();
				graphics.Dispose();
				image.Dispose();
				base.chart.chartPicture.common.graph = null;
			}
			return result;
		}

		internal virtual RectangleF GetTextSpacing(out bool annotationRelative)
		{
			annotationRelative = false;
			RectangleF rectangleF = new RectangleF(3f, 3f, 3f, 3f);
			if (base.GetGraphics() != null)
			{
				return base.GetGraphics().GetRelativeRectangle(rectangleF);
			}
			return rectangleF;
		}
	}
}
