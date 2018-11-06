namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal struct HSV
	{
		internal int Hue;

		internal int Saturation;

		internal int value;

		internal HSV(int H, int S, int V)
		{
			this.Hue = H;
			this.Saturation = S;
			this.value = V;
		}
	}
}
