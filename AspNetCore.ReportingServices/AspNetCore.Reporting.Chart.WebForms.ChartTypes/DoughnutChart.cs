using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class DoughnutChart : PieChart
	{
		public override string Name
		{
			get
			{
				return "Doughnut";
			}
		}

		public override bool Stacked
		{
			get
			{
				return false;
			}
		}

		public override bool RequireAxes
		{
			get
			{
				return false;
			}
		}

		public override bool SupportLogarithmicAxes
		{
			get
			{
				return false;
			}
		}

		public override bool SwitchValueAxes
		{
			get
			{
				return false;
			}
		}

		public override bool SideBySideSeries
		{
			get
			{
				return false;
			}
		}

		public override bool ZeroCrossing
		{
			get
			{
				return false;
			}
		}

		public override bool DataPointsInLegend
		{
			get
			{
				return true;
			}
		}

		public override bool ExtraYValuesConnectedToYAxis
		{
			get
			{
				return false;
			}
		}

		public override bool ApplyPaletteColorsToPoints
		{
			get
			{
				return true;
			}
		}

		public override int YValuesPerPoint
		{
			get
			{
				return 1;
			}
		}

		public override bool Doughnut
		{
			get
			{
				return true;
			}
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}

		public override LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Rectangle;
		}
	}
}
