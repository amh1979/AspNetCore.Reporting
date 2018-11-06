namespace AspNetCore.Reporting.Map.WebForms
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
