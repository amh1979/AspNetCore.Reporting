using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class SunkenBorder : IBorderType
	{
		protected float defaultRadiusSize = 15f;

		protected float outsideShadowRate = 0.9f;

		protected bool sunken = true;

		protected bool drawBottomShadow = true;

		protected bool drawOutsideTopLeftShadow;

		protected float[] cornerRadius = new float[8]
		{
			15f,
			15f,
			15f,
			15f,
			15f,
			15f,
			15f,
			15f
		};

		protected SizeF sizeLeftTop = SizeF.Empty;

		protected SizeF sizeRightBottom = SizeF.Empty;

		protected bool drawScrews;

		public virtual string Name
		{
			get
			{
				return "Sunken";
			}
		}

		public virtual RectangleF GetTitlePositionInBorder()
		{
			return RectangleF.Empty;
		}

		public virtual void AdjustAreasPosition(MapGraphics graph, ref RectangleF areasRect)
		{
			SizeF size = new SizeF(this.sizeLeftTop);
			SizeF size2 = new SizeF(this.sizeRightBottom);
			size.Width += (float)(this.defaultRadiusSize * 0.699999988079071);
			size.Height += (float)(this.defaultRadiusSize * 0.85000002384185791);
			size2.Width += (float)(this.defaultRadiusSize * 0.699999988079071);
			size2.Height += (float)(this.defaultRadiusSize * 0.699999988079071);
			size = graph.GetRelativeSize(size);
			size2 = graph.GetRelativeSize(size2);
			if (size.Width > 30.0)
			{
				size.Width = 0f;
			}
			if (size.Height > 30.0)
			{
				size.Height = 0f;
			}
			if (size2.Width > 30.0)
			{
				size2.Width = 0f;
			}
			if (size2.Height > 30.0)
			{
				size2.Height = 0f;
			}
			areasRect.X += size.Width;
			areasRect.Width -= Math.Min(areasRect.Width, size.Width + size2.Width);
			areasRect.Y += size.Height;
			areasRect.Height -= Math.Min(areasRect.Height, size.Height + size2.Height);
			if (areasRect.Right > 100.0)
			{
				if (areasRect.Width > 100.0 - areasRect.Right)
				{
					areasRect.Width -= (float)(100.0 - areasRect.Right);
				}
				else
				{
					areasRect.X -= (float)(100.0 - areasRect.Right);
				}
			}
			if (areasRect.Bottom > 100.0)
			{
				if (areasRect.Height > 100.0 - areasRect.Bottom)
				{
					areasRect.Height -= (float)(100.0 - areasRect.Bottom);
				}
				else
				{
					areasRect.Y -= (float)(100.0 - areasRect.Bottom);
				}
			}
		}

		public virtual void DrawBorder(MapGraphics graph, Frame borderSkin, RectangleF rect, Color backColor, MapHatchStyle backHatchStyle, string backImage, MapImageWrapMode backImageMode, Color backImageTranspColor, MapImageAlign backImageAlign, GradientType backGradientType, Color backSecondaryColor, Color borderColor, int borderWidth, MapDashStyle borderStyle)
		{
			RectangleF rectangleF = MapGraphics.Round(rect);
			RectangleF rectangleF2 = rectangleF;
			float num = (float)(0.30000001192092896 + 0.40000000596046448 * (float)(borderSkin.PageColor.R + borderSkin.PageColor.G + borderSkin.PageColor.B) / 765.0);
			Color color = Color.FromArgb((int)((float)(int)backColor.R * num), (int)((float)(int)backColor.G * num), (int)((float)(int)backColor.B * num));
			num = (float)(num + 0.20000000298023224);
			Color centerColor = Color.FromArgb((int)((float)(int)borderSkin.PageColor.R * num), (int)((float)(int)borderSkin.PageColor.G * num), (int)((float)(int)borderSkin.PageColor.B * num));
			if (borderSkin.PageColor == Color.Transparent)
			{
				centerColor = Color.FromArgb(60, 0, 0, 0);
			}
			Color.FromArgb((int)((float)(int)backColor.R * num), (int)((float)(int)backColor.G * num), (int)((float)(int)backColor.B * num));
			float val = this.defaultRadiusSize;
			val = Math.Max(val, 2f);
			val = Math.Min(val, (float)(rect.Width / 2.0));
			val = Math.Min(val, (float)(rect.Height / 2.0));
			val = (float)Math.Ceiling((double)val);
			graph.FillRectangle(new SolidBrush(borderSkin.PageColor), rect);
			if (this.drawOutsideTopLeftShadow)
			{
				rectangleF2 = rectangleF;
				rectangleF2.X -= (float)(val * 0.30000001192092896);
				rectangleF2.Y -= (float)(val * 0.30000001192092896);
				rectangleF2.Width -= (float)(val * 0.30000001192092896);
				rectangleF2.Height -= (float)(val * 0.30000001192092896);
				graph.DrawRoundedRectShadowAbs(rectangleF2, this.cornerRadius, val, Color.FromArgb(128, Color.Black), borderSkin.PageColor, this.outsideShadowRate);
			}
			rectangleF2 = rectangleF;
			rectangleF2.X += (float)(val * 0.30000001192092896);
			rectangleF2.Y += (float)(val * 0.30000001192092896);
			rectangleF2.Width -= (float)(val * 0.30000001192092896);
			rectangleF2.Height -= (float)(val * 0.30000001192092896);
			graph.DrawRoundedRectShadowAbs(rectangleF2, this.cornerRadius, val, centerColor, borderSkin.PageColor, this.outsideShadowRate);
			rectangleF2 = rectangleF;
			rectangleF2.Width -= (float)(val * 0.30000001192092896);
			rectangleF2.Height -= (float)(val * 0.30000001192092896);
			GraphicsPath graphicsPath = graph.CreateRoundedRectPath(rectangleF2, this.cornerRadius);
			graph.DrawPathAbs(graphicsPath, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backSecondaryColor, borderColor, borderWidth, borderStyle, PenAlignment.Inset);
			if (graphicsPath != null)
			{
				graphicsPath.Dispose();
			}
			if (this.drawScrews)
			{
				RectangleF empty = RectangleF.Empty;
				float num2 = (float)(val * 0.40000000596046448);
				empty.X = rectangleF2.X + num2;
				empty.Y = rectangleF2.Y + num2;
				empty.Width = (float)(val * 0.550000011920929);
				empty.Height = empty.Width;
				this.DrawScrew(graph, empty);
				empty.X = rectangleF2.Right - num2 - empty.Width;
				this.DrawScrew(graph, empty);
				empty.X = rectangleF2.Right - num2 - empty.Width;
				empty.Y = rectangleF2.Bottom - num2 - empty.Height;
				this.DrawScrew(graph, empty);
				empty.X = rectangleF2.X + num2;
				empty.Y = rectangleF2.Bottom - num2 - empty.Height;
				this.DrawScrew(graph, empty);
			}
			if (this.drawBottomShadow)
			{
				rectangleF2 = rectangleF;
				rectangleF2.Width -= (float)(val * 0.30000001192092896);
				rectangleF2.Height -= (float)(val * 0.30000001192092896);
				using (Region region = new Region(graph.CreateRoundedRectPath(new RectangleF(rectangleF2.X - val, rectangleF2.Y - val, (float)(rectangleF2.Width + 0.5 * val), (float)(rectangleF2.Height + 0.5 * val)), this.cornerRadius)))
				{
					region.Complement(graph.CreateRoundedRectPath(rectangleF2, this.cornerRadius));
					Region clip = graph.Clip;
					graph.Clip = region;
					rectangleF2.X -= (float)(0.5 * val);
					rectangleF2.Width += (float)(0.5 * val);
					rectangleF2.Y -= (float)(0.5 * val);
					rectangleF2.Height += (float)(0.5 * val);
					graph.DrawRoundedRectShadowAbs(rectangleF2, this.cornerRadius, val, Color.Transparent, Color.FromArgb(175, this.sunken ? Color.White : color), 1f);
					graph.Clip = clip;
				}
			}
			rectangleF2 = rectangleF;
			rectangleF2.Width -= (float)(val * 0.30000001192092896);
			rectangleF2.Height -= (float)(val * 0.30000001192092896);
			using (Region region2 = new Region(graph.CreateRoundedRectPath(new RectangleF((float)(rectangleF2.X + val * 0.5), (float)(rectangleF2.Y + val * 0.5), (float)(rectangleF2.Width - 0.20000000298023224 * val), (float)(rectangleF2.Height - 0.20000000298023224 * val)), this.cornerRadius)))
			{
				RectangleF rect2 = rectangleF2;
				rect2.Width += val;
				rect2.Height += val;
				region2.Complement(graph.CreateRoundedRectPath(rect2, this.cornerRadius));
				region2.Intersect(graph.CreateRoundedRectPath(rectangleF2, this.cornerRadius));
				Region clip2 = graph.Clip;
				graph.Clip = region2;
				graph.DrawRoundedRectShadowAbs(rect2, this.cornerRadius, val, Color.Transparent, Color.FromArgb(175, this.sunken ? color : Color.White), 1f);
				graph.Clip = clip2;
			}
		}

		public bool IsVisible(MapGraphics g)
		{
			RectangleF relative = new RectangleF(0f, 0f, 100f, 100f);
			this.AdjustAreasPosition(g, ref relative);
			relative = g.GetAbsoluteRectangle(relative);
			return !relative.Contains(g.Clip.GetBounds(g.Graphics));
		}

		private void DrawScrew(MapGraphics graph, RectangleF rect)
		{
			Pen pen = new Pen(Color.FromArgb(128, 255, 255, 255), 1f);
			graph.DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
			graph.DrawLine(pen, (float)(rect.X + 2.0), (float)(rect.Y + rect.Height - 2.0), (float)(rect.Right - 2.0), (float)(rect.Y + 2.0));
			pen = new Pen(Color.FromArgb(128, Color.Black), 1f);
			graph.DrawEllipse(pen, (float)(rect.X + 1.0), (float)(rect.Y + 1.0), rect.Width, rect.Height);
			graph.DrawLine(pen, (float)(rect.X + 3.0), (float)(rect.Y + rect.Height - 1.0), (float)(rect.Right - 1.0), (float)(rect.Y + 3.0));
		}
	}
}
