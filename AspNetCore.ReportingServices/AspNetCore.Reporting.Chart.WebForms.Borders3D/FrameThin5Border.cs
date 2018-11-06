namespace AspNetCore.Reporting.Chart.WebForms.Borders3D
{
	internal class FrameThin5Border : FrameThin1Border
	{
		public override string Name
		{
			get
			{
				return "FrameThin5";
			}
		}

		public FrameThin5Border()
		{
			base.drawScrews = true;
		}
	}
}
