namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal interface IHtmlWriter
	{
		bool IsBrowserIE
		{
			get;
		}

		void WriteClassName(byte[] className, byte[] classNameIfNoPrefix);
	}
}
