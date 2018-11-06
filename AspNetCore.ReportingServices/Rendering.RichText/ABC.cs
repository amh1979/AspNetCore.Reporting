namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal struct ABC
	{
		internal int abcA;

		internal uint abcB;

		internal int abcC;

		internal int Width
		{
			get
			{
				return (int)(this.abcA + this.abcB + this.abcC);
			}
		}

		internal void SetToZeroWidth()
		{
			this.abcA = 0;
			this.abcB = 0u;
			this.abcC = 0;
		}
	}
}
