namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartCustomPaletteColorCollection : ChartObjectCollectionBase<ChartCustomPaletteColor, ChartCustomPaletteColorInstance>
	{
		private Chart m_chart;

		public override int Count
		{
			get
			{
				if (this.m_chart.IsOldSnapshot)
				{
					return 0;
				}
				if (this.m_chart.ChartDef.CustomPaletteColors != null)
				{
					return this.m_chart.ChartDef.CustomPaletteColors.Count;
				}
				return 0;
			}
		}

		internal ChartCustomPaletteColorCollection(Chart chart)
		{
			this.m_chart = chart;
		}

		protected override ChartCustomPaletteColor CreateChartObject(int index)
		{
			if (this.m_chart.IsOldSnapshot)
			{
				return null;
			}
			return new ChartCustomPaletteColor(this.m_chart.ChartDef.CustomPaletteColors[index], this.m_chart);
		}
	}
}
