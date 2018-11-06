namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal class BulletPrefixRun : PrefixRun
	{
		private const string PrefixBulletFontFamily = "Courier New";

		internal override string FontName
		{
			get
			{
				return "Courier New";
			}
		}
	}
}
