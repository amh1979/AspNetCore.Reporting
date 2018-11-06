using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class SplineRangeChart : RangeChart
	{
		public override string Name
		{
			get
			{
				return "SplineRange";
			}
		}

		public SplineRangeChart()
		{
			base.lineTension = 0.5f;
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}

		protected override float GetDefaultTension()
		{
			return 0.5f;
		}

		protected override bool IsLineTensionSupported()
		{
			return true;
		}
	}
}
