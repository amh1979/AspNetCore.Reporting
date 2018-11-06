using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class HotRegion
	{
		private GraphicsPath path;

		private bool relativeCoordinates = true;

		private RectangleF boundingRectangle = RectangleF.Empty;

		private object selectedObject;

		private int pointIndex = -1;

		private string seriesName = "";

		private ChartElementType type;

		private object selectedSubObject;

		internal GraphicsPath Path
		{
			get
			{
				return this.path;
			}
			set
			{
				this.path = value;
			}
		}

		internal bool RelativeCoordinates
		{
			get
			{
				return this.relativeCoordinates;
			}
			set
			{
				this.relativeCoordinates = value;
			}
		}

		internal RectangleF BoundingRectangle
		{
			get
			{
				return this.boundingRectangle;
			}
			set
			{
				this.boundingRectangle = value;
			}
		}

		internal object SelectedObject
		{
			get
			{
				return this.selectedObject;
			}
			set
			{
				this.selectedObject = value;
			}
		}

		internal object SelectedSubObject
		{
			get
			{
				return this.selectedSubObject;
			}
			set
			{
				this.selectedSubObject = value;
			}
		}

		internal int PointIndex
		{
			get
			{
				return this.pointIndex;
			}
			set
			{
				this.pointIndex = value;
			}
		}

		internal string SeriesName
		{
			get
			{
				return this.seriesName;
			}
			set
			{
				this.seriesName = value;
			}
		}

		internal ChartElementType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}
	}
}
