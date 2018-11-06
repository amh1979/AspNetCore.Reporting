using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class EmbossBorder : IBorderType
	{
		public const float defaultRadiusSize = 15f;

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

		public virtual string Name
		{
			get
			{
				return "Emboss";
			}
		}

		public virtual RectangleF GetTitlePositionInBorder()
		{
			return RectangleF.Empty;
		}

		public virtual void AdjustAreasPosition(MapGraphics graph, ref RectangleF areasRect)
		{
			SizeF size = new SizeF(7.5f, 7.5f);
			size = graph.GetRelativeSize(size);
			if (size.Width < 30.0)
			{
				areasRect.X += size.Width;
				areasRect.Width -= Math.Min(areasRect.Width, (float)(size.Width * 2.5));
			}
			if (size.Height < 30.0)
			{
				areasRect.Y += size.Height;
				areasRect.Height -= Math.Min(areasRect.Height, (float)(size.Height * 2.5));
			}
			if (areasRect.X + areasRect.Width > 100.0)
			{
				areasRect.X -= (float)(100.0 - areasRect.Width);
			}
			if (areasRect.Y + areasRect.Height > 100.0)
			{
				areasRect.Y -= (float)(100.0 - areasRect.Height);
			}
		}

		public virtual void DrawBorder(MapGraphics graph, Frame borderSkin, RectangleF rect, Color backColor, MapHatchStyle backHatchStyle, string backImage, MapImageWrapMode backImageMode, Color backImageTranspColor, MapImageAlign backImageAlign, GradientType backGradientType, Color backSecondaryColor, Color borderColor, int borderWidth, MapDashStyle borderStyle)
		{
			RectangleF rectangleF = MapGraphics.Round(rect);
			RectangleF rect2 = rectangleF;
			float num = (float)(0.20000000298023224 + 0.40000000596046448 * (float)(borderSkin.PageColor.R + borderSkin.PageColor.G + borderSkin.PageColor.B) / 765.0);
			Color centerColor = Color.FromArgb((int)((float)(int)borderSkin.PageColor.R * num), (int)((float)(int)borderSkin.PageColor.G * num), (int)((float)(int)borderSkin.PageColor.B * num));
			if (borderSkin.PageColor == Color.Transparent)
			{
				centerColor = Color.FromArgb(60, 0, 0, 0);
			}
			num = (float)(num + 0.20000000298023224);
			Color centerColor2 = Color.FromArgb((int)((float)(int)borderSkin.PageColor.R * num), (int)((float)(int)borderSkin.PageColor.G * num), (int)((float)(int)borderSkin.PageColor.B * num));
			float val = 15f;
			val = Math.Max(val, 2f);
			val = Math.Min(val, (float)(rect.Width / 2.0));
			val = Math.Min(val, (float)(rect.Height / 2.0));
			val = (float)Math.Ceiling((double)val);
			graph.FillRectangle(new SolidBrush(borderSkin.PageColor), rect);
			rect2 = rectangleF;
			rect2.Width -= (float)(val * 0.30000001192092896);
			rect2.Height -= (float)(val * 0.30000001192092896);
			graph.DrawRoundedRectShadowAbs(rect2, this.cornerRadius, (float)(val + 1.0), centerColor2, borderSkin.PageColor, 1.4f);
			rect2 = rectangleF;
			rect2.X = (float)(rectangleF.X + val / 3.0);
			rect2.Y = (float)(rectangleF.Y + val / 3.0);
			rect2.Width -= (float)(val / 3.5);
			rect2.Height -= (float)(val / 3.5);
			graph.DrawRoundedRectShadowAbs(rect2, this.cornerRadius, val, centerColor, borderSkin.PageColor, 1.3f);
			rect2 = rectangleF;
			rect2.X = (float)(rectangleF.X + 3.0);
			rect2.Y = (float)(rectangleF.Y + 3.0);
			rect2.Width -= (float)(val * 0.75);
			rect2.Height -= (float)(val * 0.75);
			GraphicsPath graphicsPath = graph.CreateRoundedRectPath(rect2, this.cornerRadius);
			graph.DrawPathAbs(graphicsPath, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backSecondaryColor, borderColor, borderWidth, borderStyle, PenAlignment.Inset);
			if (graphicsPath != null)
			{
				graphicsPath.Dispose();
			}
			using (Region region = new Region(graph.CreateRoundedRectPath(new RectangleF(rect2.X - val, rect2.Y - val, (float)(rect2.Width + val - val * 0.25), (float)(rect2.Height + val - val * 0.25)), this.cornerRadius)))
			{
				region.Complement(graph.CreateRoundedRectPath(rect2, this.cornerRadius));
				Region clip = graph.Clip;
				graph.Clip = region;
				graph.DrawRoundedRectShadowAbs(rect2, this.cornerRadius, val, Color.Transparent, Color.FromArgb(128, Color.Gray), 0.5f);
				graph.Clip = clip;
			}
		}

		public bool IsVisible(MapGraphics g)
		{
			RectangleF relative = new RectangleF(0f, 0f, 100f, 100f);
			this.AdjustAreasPosition(g, ref relative);
			relative = g.GetAbsoluteRectangle(relative);
			return !relative.Contains(g.Clip.GetBounds(g.Graphics));
		}
	}
}
