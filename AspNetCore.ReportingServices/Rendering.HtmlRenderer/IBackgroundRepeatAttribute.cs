namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal interface IBackgroundRepeatAttribute
	{
		void Render(IOutputStream outputStream);
	}
}
