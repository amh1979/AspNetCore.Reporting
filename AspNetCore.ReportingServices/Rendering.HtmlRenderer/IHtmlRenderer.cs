namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal interface IHtmlRenderer
	{
		void WriteStream(byte[] bytes);

		void WriteStream(string value);
	}
}
