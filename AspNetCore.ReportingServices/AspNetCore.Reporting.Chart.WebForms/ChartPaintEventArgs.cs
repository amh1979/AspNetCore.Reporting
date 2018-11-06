using System;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class ChartPaintEventArgs : EventArgs
	{
		private ChartGraphics chartGraph;

		private CommonElements common;

		private Chart chart;

		private ElementPosition position;

		public ChartGraphics ChartGraphics
		{
			get
			{
				return this.chartGraph;
			}
		}

		internal CommonElements CommonElements
		{
			get
			{
				return this.common;
			}
		}

		public ElementPosition Position
		{
			get
			{
				return this.position;
			}
		}

		internal Chart Chart
		{
			get
			{
				if (this.chart == null && this.common != null && this.common.container != null)
				{
					this.chart = (Chart)this.common.container.GetService(typeof(Chart));
				}
				return this.chart;
			}
		}

		private ChartPaintEventArgs()
		{
		}

		public ChartPaintEventArgs(ChartGraphics chartGraph, CommonElements common, ElementPosition position)
		{
			this.chartGraph = chartGraph;
			this.common = common;
			this.position = position;
		}
	}
}
