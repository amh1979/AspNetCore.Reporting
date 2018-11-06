using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class NeedleStyleAttrib
	{
		public GraphicsPath primaryPath;

		public GraphicsPath secondaryPath;

		public Brush primaryBrush;

		public Brush secondaryBrush;

		public GraphicsPath[] reflectionPaths;

		public Brush[] reflectionBrushes;

		public NeedleStyleAttrib()
		{
			this.primaryPath = null;
			this.secondaryPath = null;
			this.primaryBrush = null;
			this.secondaryBrush = null;
			this.reflectionPaths = null;
			this.reflectionBrushes = null;
		}

		public void Dispose()
		{
			if (this.primaryPath != null)
			{
				this.primaryPath.Dispose();
				this.primaryPath = null;
			}
			if (this.secondaryPath != null)
			{
				this.secondaryPath.Dispose();
				this.secondaryPath = null;
			}
			if (this.primaryBrush != null)
			{
				this.primaryBrush.Dispose();
				this.primaryBrush = null;
			}
			if (this.secondaryBrush != null)
			{
				this.secondaryBrush.Dispose();
				this.secondaryBrush = null;
			}
			if (this.reflectionPaths != null)
			{
				GraphicsPath[] array = this.reflectionPaths;
				foreach (GraphicsPath graphicsPath in array)
				{
					if (graphicsPath != null)
					{
						graphicsPath.Dispose();
					}
				}
				this.reflectionPaths = null;
			}
			if (this.reflectionBrushes != null)
			{
				Brush[] array2 = this.reflectionBrushes;
				foreach (Brush brush in array2)
				{
					if (brush != null)
					{
						brush.Dispose();
					}
				}
				this.reflectionBrushes = null;
			}
		}
	}
}
