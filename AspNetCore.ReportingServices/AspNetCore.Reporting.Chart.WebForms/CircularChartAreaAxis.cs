using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class CircularChartAreaAxis
	{
		internal float AxisPosition;

		internal float AxisSectorSize;

		internal string Title = string.Empty;

		internal Color TitleColor = Color.Empty;

		public CircularChartAreaAxis()
		{
		}

		public CircularChartAreaAxis(float axisPosition, float axisSectorSize)
		{
			this.AxisPosition = axisPosition;
			this.AxisSectorSize = axisSectorSize;
		}
	}
}
