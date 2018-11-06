using System;
using System.Drawing;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal class ImageInfo : IDisposable
	{
		public Stream ImageData;

		public int Width;

		public int Height;

		public void RenderAndDispose(Graphics g, int x, int y)
		{
			using (System.Drawing.Image image = System.Drawing.Image.FromStream(this.ImageData))
			{
				g.DrawImage(image, new Point(x, y));
			}
			this.Dispose();
		}

		public void Dispose()
		{
			if (this.ImageData != null)
			{
				this.ImageData.Dispose();
				this.ImageData = null;
			}
			GC.SuppressFinalize(this);
		}
	}
}
