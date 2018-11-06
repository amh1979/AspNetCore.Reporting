using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System.Drawing;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class DrawLineOp : Operation
	{
		internal float Width;

		internal RPLFormat.BorderStyles Style;

		internal Color Color;

		internal float X1;

		internal float Y1;

		internal float X2;

		internal float Y2;

		internal DrawLineOp(Color color, float width, RPLFormat.BorderStyles style, float x1, float y1, float x2, float y2)
		{
			this.Color = color;
			this.Width = width;
			this.Style = style;
			this.X1 = x1;
			this.Y1 = y1;
			this.X2 = x2;
			this.Y2 = y2;
		}

		internal override void Perform(WriterBase writer)
		{
			writer.DrawLine(this.Color, this.Width, this.Style, this.X1, this.Y1, this.X2, this.Y2);
		}
	}
}
