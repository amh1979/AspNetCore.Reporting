using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class SplineChart : LineChart
	{
		public override string Name
		{
			get
			{
				return "Spline";
			}
		}

		public SplineChart()
		{
			base.lineTension = 0.5f;
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}

		protected override bool IsLineTensionSupported()
		{
			return true;
		}

		protected override PointF[] GetPointsPosition(ChartGraphics graph, Series series, bool indexedSeries)
		{
			base.lineTension = this.GetDefaultTension();
			if (this.IsLineTensionSupported() && series.IsAttributeSet("LineTension"))
			{
				base.lineTension = CommonElements.ParseFloat(((DataPointAttributes)series)["LineTension"]);
			}
			return base.GetPointsPosition(graph, series, indexedSeries);
		}

		protected override float GetDefaultTension()
		{
			return 0.5f;
		}
	}
}
