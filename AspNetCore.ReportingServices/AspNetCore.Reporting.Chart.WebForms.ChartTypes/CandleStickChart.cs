using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class CandleStickChart : StockChart
	{
		public override string Name
		{
			get
			{
				return "Candlestick";
			}
		}

		public CandleStickChart()
			: base(StockOpenCloseMarkStyle.Candlestick)
		{
			base.forceCandleStick = true;
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}
	}
}
