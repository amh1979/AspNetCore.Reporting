using System.Drawing;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class PDFUriAction
	{
		internal string Uri;

		internal RectangleF Rectangle;

		internal PDFUriAction(string uri, RectangleF rectangle)
		{
			this.Uri = uri;
			this.Rectangle = rectangle;
		}
	}
}
