using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal struct GridLine
	{
		public GridType GridType;

		public PointF[] Points;

		public GraphicsPath Path;

		public RectangleF LabelRect;

		public double Coordinate;

		public PointF[] SelectionMarkerPositions;

		public void Dispose()
		{
			if (this.Path != null)
			{
				this.Path.Dispose();
			}
		}
	}
}
