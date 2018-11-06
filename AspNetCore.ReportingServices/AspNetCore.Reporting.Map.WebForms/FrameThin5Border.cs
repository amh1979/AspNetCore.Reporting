namespace AspNetCore.Reporting.Map.WebForms
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
