using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class GdiGraphics : IChartRenderingEngine
	{
		private Graphics graphics;

		public Matrix Transform
		{
			get
			{
				return this.graphics.Transform;
			}
			set
			{
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

		public bool IsClipEmpty
		{
			get
			{
				return this.graphics.IsClipEmpty;
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

		public void DrawLine(Pen pen, PointF pt1, PointF pt2)
		{
			this.graphics.DrawLine(pen, pt1, pt2);
		}

		public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
		{
			this.graphics.DrawLine(pen, x1, y1, x2, y2);
		}

		public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr)
		{
			this.graphics.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttr);
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			this.graphics.DrawEllipse(pen, x, y, width, height);
		}

		public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
		{
			this.graphics.DrawCurve(pen, points, offset, numberOfSegments, tension);
		}

		public void DrawRectangle(Pen pen, int x, int y, int width, int height)
		{
			this.graphics.DrawRectangle(pen, x, y, width, height);
		}

		public void DrawPolygon(Pen pen, PointF[] points)
		{
			this.graphics.DrawPolygon(pen, points);
		}

		public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
		{
			this.graphics.DrawString(s, font, brush, layoutRectangle, format);
		}

		public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
		{
			this.graphics.DrawString(s, font, brush, point, format);
		}

		public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
		{
			this.graphics.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs);
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			this.graphics.DrawRectangle(pen, x, y, width, height);
		}

		public void DrawPath(Pen pen, GraphicsPath path)
		{
			this.graphics.DrawPath(pen, path);
		}

		public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			this.graphics.DrawPie(pen, x, y, width, height, startAngle, sweepAngle);
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			this.graphics.DrawArc(pen, x, y, width, height, startAngle, sweepAngle);
		}

		public void DrawImage(Image image, RectangleF rect)
		{
			this.graphics.DrawImage(image, rect);
		}

		public void DrawEllipse(Pen pen, RectangleF rect)
		{
			this.graphics.DrawEllipse(pen, rect);
		}

		public void DrawLines(Pen pen, PointF[] points)
		{
			this.graphics.DrawLines(pen, points);
		}

		public void FillEllipse(Brush brush, RectangleF rect)
		{
			this.graphics.FillEllipse(brush, rect);
		}

		public void FillPath(Brush brush, GraphicsPath path)
		{
			this.graphics.FillPath(brush, path);
		}

		public void FillRegion(Brush brush, Region region)
		{
			this.graphics.FillRegion(brush, region);
		}

		public void FillRectangle(Brush brush, RectangleF rect)
		{
			this.graphics.FillRectangle(brush, rect);
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			this.graphics.FillRectangle(brush, x, y, width, height);
		}

		public void FillPolygon(Brush brush, PointF[] points)
		{
			this.graphics.FillPolygon(brush, points);
		}

		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			this.graphics.FillPie(brush, x, y, width, height, startAngle, sweepAngle);
		}

		public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
		{
			return this.graphics.MeasureString(text, font, layoutArea, stringFormat);
		}

		public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, out int charactersFitted, out int linesFilled)
		{
			return this.graphics.MeasureString(text, font, layoutArea, stringFormat, out charactersFitted, out linesFilled);
		}

		public SizeF MeasureString(string text, Font font)
		{
			return this.graphics.MeasureString(text, font);
		}

		public GraphicsState Save()
		{
			return this.graphics.Save();
		}

		public void Restore(GraphicsState gstate)
		{
			this.graphics.Restore(gstate);
		}

		public void ResetClip()
		{
			this.graphics.ResetClip();
		}

		public void SetClip(RectangleF rect)
		{
			this.graphics.SetClip(rect);
		}

		public void SetClip(GraphicsPath path, CombineMode combineMode)
		{
			this.graphics.SetClip(path, combineMode);
		}

		public void TranslateTransform(float dx, float dy)
		{
			this.graphics.TranslateTransform(dx, dy);
		}

		public void BeginSelection(string hRef, string title)
		{
		}

		public void EndSelection()
		{
		}
	}
}
