using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class SvgMapGraphics : SvgRendering, IMapRenderingEngine
	{
		private Graphics graphics;

		public new Matrix Transform
		{
			get
			{
				return this.graphics.Transform;
			}
			set
			{
				base.mapMatrix = value;
				this.graphics.Transform = value;
			}
		}

		public SmoothingMode SmoothingMode
		{
			get
			{
				return this.graphics.SmoothingMode;
			}
			set
			{
				this.graphics.SmoothingMode = value;
				base.SetSmoothingMode(value == SmoothingMode.AntiAlias, true);
			}
		}

		public bool IsClipEmpty
		{
			get
			{
				return this.graphics.IsClipEmpty;
			}
		}

		public Region Clip
		{
			get
			{
				return this.graphics.Clip;
			}
			set
			{
				this.graphics.Clip = value;
			}
		}

		public Graphics Graphics
		{
			get
			{
				return this.graphics;
			}
			set
			{
				this.graphics = value;
			}
		}

		public TextRenderingHint TextRenderingHint
		{
			get
			{
				return this.graphics.TextRenderingHint;
			}
			set
			{
				this.graphics.TextRenderingHint = value;
				base.SetSmoothingMode(value == TextRenderingHint.AntiAlias || value == TextRenderingHint.SystemDefault || value == TextRenderingHint.ClearTypeGridFit, false);
			}
		}

		public SvgMapGraphics(CommonElements common)
		{
		}

		public void DrawLine(Pen pen, PointF pt1, PointF pt2)
		{
			base.Pen = pen;
			base.DrawLine(pt1, pt2);
		}

		public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
		{
			this.DrawLine(pen, new PointF(x1, y1), new PointF(x2, y2));
		}

		public new void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr)
		{
			base.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttr);
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			base.Pen = pen;
			base.DrawEllipse(new RectangleF(x, y, width, height));
		}

		public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
		{
			base.Pen = pen;
			base.DrawCurve(points, offset, numberOfSegments, tension);
		}

		public void DrawRectangle(Pen pen, int x, int y, int width, int height)
		{
			this.DrawRectangle(pen, (float)x, (float)y, (float)width, (float)height);
		}

		public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
		{
			base.mapFont = font;
			base.Brush = brush;
			base.mapStringFormat = format;
			base.DrawString(s, layoutRectangle);
		}

		public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
		{
			base.mapFont = font;
			base.Brush = brush;
			base.mapStringFormat = format;
			base.DrawString(s, point);
		}

		public new void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
		{
			base.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs);
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			base.Pen = pen;
			base.DrawRectangle(new RectangleF(x, y, width, height));
		}

		public void DrawPath(Pen pen, GraphicsPath path)
		{
			base.Pen = pen;
			base.DrawPath(path);
		}

		public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			base.Pen = pen;
			base.DrawPie(new RectangleF(x, y, width, height), startAngle, sweepAngle);
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			base.Pen = pen;
			base.DrawArc(new RectangleF(x, y, width, height), startAngle, sweepAngle);
		}

		public new void DrawImage(Image image, RectangleF rect)
		{
			base.DrawImage(image, rect);
		}

		public void DrawEllipse(Pen pen, RectangleF rect)
		{
			base.Pen = pen;
			base.DrawEllipse(rect);
		}

		public void DrawLines(Pen pen, PointF[] points)
		{
			base.Pen = pen;
			base.DrawLines(points);
		}

		public void FillEllipse(Brush brush, RectangleF rect)
		{
			base.Brush = brush;
			base.FillEllipse(rect);
		}

		public void FillPath(Brush brush, GraphicsPath path)
		{
			base.Brush = brush;
			base.FillPath(path);
		}

		public void FillPath(Brush brush, GraphicsPath path, float angle, bool useBrushOffset, bool circularFill)
		{
			base.Brush = brush;
			base.FillPath(path);
		}

		public void FillRegion(Brush brush, Region region)
		{
		}

		public void FillRectangle(Brush brush, RectangleF rect)
		{
			base.Brush = brush;
			if (brush is TextureBrush)
			{
				base.FillTexturedRectangle((TextureBrush)brush, rect);
			}
			else
			{
				base.FillRectangle(rect);
			}
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			base.Brush = brush;
			if (brush is TextureBrush)
			{
				base.FillTexturedRectangle((TextureBrush)brush, new RectangleF(x, y, width, height));
			}
			else
			{
				base.FillRectangle(new RectangleF(x, y, width, height));
			}
		}

		public void FillPolygon(Brush brush, PointF[] points)
		{
			base.Brush = brush;
			base.FillPolygon(points);
		}

		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			base.Brush = brush;
			base.FillPie(new RectangleF(x, y, width, height), startAngle, sweepAngle);
		}

		public new void SetClip(RectangleF rect)
		{
			base.SetClip(rect);
			this.graphics.SetClip(rect);
		}

		public new void ResetClip()
		{
			base.ResetClip();
			this.graphics.ResetClip();
		}

		public void SetClip(GraphicsPath path, CombineMode combineMode)
		{
		}

		public void TranslateTransform(float dx, float dy)
		{
			this.graphics.TranslateTransform(dx, dy);
		}

		public void SetGradient(Color firstColor, Color secondColor, GradientType gradientType)
		{
			base.mapSvgGradientType = (SvgGradientType)Enum.Parse(typeof(SvgGradientType), ((Enum)(object)gradientType).ToString((IFormatProvider)CultureInfo.CurrentCulture));
			base.mapBrushColor = firstColor;
			base.mapBrushSecondColor = secondColor;
		}

		public void DrawPolygon(Pen pen, PointF[] points)
		{
			base.Pen = pen;
			base.DrawPolygon(points);
		}

		public GraphicsState Save()
		{
			return this.graphics.Save();
		}

		public void Restore(GraphicsState gstate)
		{
			this.graphics.Restore(gstate);
			this.Transform = this.graphics.Transform;
		}

		public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
		{
			return this.graphics.MeasureString(text, font, layoutArea, stringFormat);
		}

		public SizeF MeasureString(string text, Font font)
		{
			return this.graphics.MeasureString(text, font);
		}

		public void BeginSelection(string hRef, string title)
		{
			base.BeginSvgSelection(hRef, title);
		}

		public void EndSelection()
		{
			base.EndSvgSelection();
		}
	}
}
