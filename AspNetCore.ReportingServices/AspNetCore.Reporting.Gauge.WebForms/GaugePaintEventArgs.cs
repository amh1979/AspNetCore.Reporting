using System;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class GaugePaintEventArgs : EventArgs
	{
		private GaugeContainer gauge;

		private GaugeGraphics graphics;

		internal GaugeContainer Gauge
		{
			get
			{
				return this.gauge;
			}
		}

		public GaugeGraphics Graphics
		{
			get
			{
				return this.graphics;
			}
		}

		internal GaugePaintEventArgs(GaugeContainer gauge, GaugeGraphics graphics)
		{
			this.gauge = gauge;
			this.graphics = graphics;
		}
	}
}
