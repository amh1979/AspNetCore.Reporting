using System.Drawing;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class FrameThin1Border : RaisedBorder
	{
		protected float[] innerCorners = new float[8]
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

		public override string Name
		{
			get
			{
				return "FrameThin1";
			}
		}

		public FrameThin1Border()
		{
			base.sizeLeftTop = new SizeF((float)(base.defaultRadiusSize * 0.800000011920929), (float)(base.defaultRadiusSize * 0.800000011920929));
			base.sizeRightBottom = new SizeF((float)(base.defaultRadiusSize * 0.800000011920929), (float)(base.defaultRadiusSize * 0.800000011920929));
		}

		public override void DrawBorder(MapGraphics graph, Frame borderSkin, RectangleF rect, Color backColor, MapHatchStyle backHatchStyle, string backImage, MapImageWrapMode backImageMode, Color backImageTranspColor, MapImageAlign backImageAlign, GradientType backGradientType, Color backSecondaryColor, Color borderColor, int borderWidth, MapDashStyle borderStyle)
		{
			base.drawBottomShadow = true;
			base.sunken = false;
			base.outsideShadowRate = 0.9f;
			base.drawOutsideTopLeftShadow = false;
			bool drawScrews = base.drawScrews;
			base.drawScrews = false;
			base.DrawBorder(graph, borderSkin, rect, borderSkin.BackColor, borderSkin.BackHatchStyle, borderSkin.BackImage, borderSkin.BackImageMode, borderSkin.BackImageTranspColor, borderSkin.BackImageAlign, borderSkin.BackGradientType, borderSkin.BackSecondaryColor, borderSkin.BorderColor, borderSkin.BorderWidth, borderSkin.BorderStyle);
			base.drawScrews = drawScrews;
			rect.X += base.sizeLeftTop.Width;
			rect.Y += base.sizeLeftTop.Height;
			rect.Width -= base.sizeRightBottom.Width + base.sizeLeftTop.Width;
			rect.Height -= base.sizeRightBottom.Height + base.sizeLeftTop.Height;
			if (rect.Width > 0.0 && rect.Height > 0.0)
			{
				float[] array = new float[8];
				array = (float[])base.cornerRadius.Clone();
				base.cornerRadius = this.innerCorners;
				base.drawBottomShadow = false;
				base.sunken = true;
				base.drawOutsideTopLeftShadow = true;
				base.outsideShadowRate = 1.4f;
				Color pageColor = borderSkin.PageColor;
				borderSkin.PageColor = Color.Transparent;
				base.DrawBorder(graph, borderSkin, rect, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backSecondaryColor, borderColor, borderWidth, borderStyle);
				borderSkin.PageColor = pageColor;
				base.cornerRadius = array;
			}
		}
	}
}
