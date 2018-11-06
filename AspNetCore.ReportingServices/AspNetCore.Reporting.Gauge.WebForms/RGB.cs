using System;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal struct RGB
	{
		internal int Red;

		internal int Green;

		internal int Blue;

		internal RGB(int R, int G, int B)
		{
			this.Red = Math.Max(Math.Min(R, 255), 0);
			this.Green = Math.Max(Math.Min(G, 255), 0);
			this.Blue = Math.Max(Math.Min(B, 255), 0);
		}
	}
}
