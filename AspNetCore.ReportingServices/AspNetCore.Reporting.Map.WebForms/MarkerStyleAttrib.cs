using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class MarkerStyleAttrib
	{
		public GraphicsPath path;

		public Brush brush;

		public MarkerStyleAttrib()
		{
			this.path = null;
			this.brush = null;
		}

		public void Dispose()
		{
			if (this.path != null)
			{
				this.path.Dispose();
			}
			if (this.brush != null)
			{
				this.brush.Dispose();
			}
		}
	}
}
