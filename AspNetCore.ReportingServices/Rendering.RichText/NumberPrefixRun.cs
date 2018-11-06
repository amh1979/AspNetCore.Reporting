namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal class NumberPrefixRun : PrefixRun
	{
		private const string PrefixNumberFontFamily = "Arial";

		internal override string FontName
		{
			get
			{
				return "Arial";
			}
		}
	}
}
