using System.Drawing;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class PDFPagePoint
	{
		internal int PageObjectId;

		internal PointF Point;

		internal PDFPagePoint(int pageObjectId, PointF point)
		{
			this.PageObjectId = pageObjectId;
			this.Point = point;
		}
	}
}
