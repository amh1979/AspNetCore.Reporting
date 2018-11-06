using System.Drawing;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class FillPolygonOp : Operation
	{
		internal Color Color;

		internal PointF[] Polygon;

		internal FillPolygonOp(Color color, PointF[] polygon)
		{
			this.Color = color;
			this.Polygon = polygon;
		}

		internal override void Perform(WriterBase writer)
		{
			writer.FillPolygon(this.Color, this.Polygon);
		}
	}
}
