using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class BarStyleAttrib
	{
		public GraphicsPath primaryPath;

		public Brush primaryBrush;

		public GraphicsPath[] secondaryPaths;

		public Brush[] secondaryBrushes;

		public GraphicsPath totalPath;

		public Brush totalBrush;

		public BarStyleAttrib()
		{
			this.primaryPath = null;
			this.primaryBrush = null;
			this.secondaryPaths = null;
			this.secondaryBrushes = null;
			this.totalPath = null;
			this.totalBrush = null;
		}

		public void Dispose()
		{
			if (this.primaryPath != null)
			{
				this.primaryPath.Dispose();
				this.primaryPath = null;
			}
			if (this.primaryBrush != null)
			{
				this.primaryBrush.Dispose();
				this.primaryBrush = null;
			}
			if (this.secondaryPaths != null)
			{
				GraphicsPath[] array = this.secondaryPaths;
				foreach (GraphicsPath graphicsPath in array)
				{
					if (graphicsPath != null)
					{
						graphicsPath.Dispose();
					}
				}
				this.secondaryPaths = null;
			}
			if (this.secondaryBrushes != null)
			{
				Brush[] array2 = this.secondaryBrushes;
				foreach (Brush brush in array2)
				{
					if (brush != null)
					{
						brush.Dispose();
					}
				}
				this.secondaryBrushes = null;
			}
			if (this.totalPath != null)
			{
				this.totalPath.Dispose();
				this.totalPath = null;
			}
			if (this.totalBrush != null)
			{
				this.totalBrush.Dispose();
				this.totalBrush = null;
			}
		}
	}
}
