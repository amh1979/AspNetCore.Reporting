namespace AspNetCore.Reporting.Chart.WebForms.Borders3D
{
	internal class RaisedBorder : SunkenBorder
	{
		public override string Name
		{
			get
			{
				return "Raised";
			}
		}

		public RaisedBorder()
		{
			base.sunken = false;
		}
	}
}
