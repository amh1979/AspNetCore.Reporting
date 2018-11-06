using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class SplineAreaChart : AreaChart
	{
		public override string Name
		{
			get
			{
				return "SplineArea";
			}
		}

		public SplineAreaChart()
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
