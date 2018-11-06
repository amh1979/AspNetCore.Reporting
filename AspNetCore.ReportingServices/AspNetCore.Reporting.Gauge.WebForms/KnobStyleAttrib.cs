using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class KnobStyleAttrib
	{
		public GraphicsPath[] paths;

		public Brush[] brushes;

		public KnobStyleAttrib()
		{
			this.paths = null;
			this.brushes = null;
		}

		public void Dispose()
		{
			if (this.paths != null)
			{
				GraphicsPath[] array = this.paths;
				foreach (GraphicsPath graphicsPath in array)
				{
					if (graphicsPath != null)
					{
						graphicsPath.Dispose();
					}
				}
				this.paths = null;
			}
			if (this.brushes != null)
			{
				Brush[] array2 = this.brushes;
				foreach (Brush brush in array2)
				{
					if (brush != null)
					{
						brush.Dispose();
					}
				}
				this.brushes = null;
			}
		}
	}
}
