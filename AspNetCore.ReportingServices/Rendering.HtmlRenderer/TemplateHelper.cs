using System.IO;
using System.Reflection;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal static class TemplateHelper
	{
		public const string TemplateResourcePrefix = "AspNetCore.ReportingServices.Rendering.HtmlRenderer.Templates.";

		public static string GetTemplate(Template template)
		{
			string name = "AspNetCore.ReportingServices.Rendering.HtmlRenderer.Templates." + template.ToFullName();
			return TemplateHelper.GetText(name);
		}

		private static string GetText(string name)
		{
			using (StreamReader streamReader = new StreamReader(TemplateHelper.GetStream(name)))
			{
				return streamReader.ReadToEnd();
			}
		}

		private static Stream GetStream(string name)
		{
			return Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
		}
	}
}
