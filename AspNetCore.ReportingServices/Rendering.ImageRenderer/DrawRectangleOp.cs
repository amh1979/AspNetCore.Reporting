using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System.Drawing;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class DrawRectangleOp : Operation
	{
		internal float Width;

		internal RPLFormat.BorderStyles Style;

		internal Color Color;

		internal RectangleF Rectangle;

		internal DrawRectangleOp(Color color, float width, RPLFormat.BorderStyles style, RectangleF rectangle)
		{
			this.Color = color;
			this.Width = width;
			this.Style = style;
			this.Rectangle = rectangle;
		}

		internal override void Perform(WriterBase writer)
		{
			writer.DrawRectangle(this.Color, this.Width, this.Style, this.Rectangle);
		}
	}
}
