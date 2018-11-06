using AspNetCore.Reporting.Common;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal static class HTMLRendererResources
	{
		public const string ResourceNamespace = "AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.";

		public const string PushPin = "PushPin.png";

		public const string TogglePlus = "TogglePlus.gif";

		public const string ToggleMinus = "ToggleMinus.gif";

		public const string SortAsc = "sortAsc.gif";

		public const string SortDesc = "sortDesc.gif";

		public const string SortNone = "unsorted.gif";

		public const string Blank = "Blank.gif";

		public const string CommonScript = "Common.js";

		public const string FitProportionalScript = "FitProportional.js";

		public const string FixedHeaderScript = "FixedHeader.js";

		public const string CanGrowFalseScript = "CanGrowFalse.js";

		public const string ImageConsolidationScript = "ImageConsolidation.js";

		public const string Html5ToolbarCss = "Html5Toolbar.css";

		public const string Html40ViewerCss = "Html40Viewer.css";

		public const string ReportingServices40Css = "ReportingServices40.css";

		public const string Html5RendererCss = "Html5Renderer.css";

		public const string JQueryUiCss = "jqueryui.min.css";

		public const string ReportingServicesHybridCss = "ReportingServicesHybrid.css";

		public const string ReportingServicesCss = "ReportingServices.css";

		public const string Html5RenderingExtensionJs = "Html5RenderingExtensionJs.js";

		public const string Html5RendererJs = "Html5Renderer.js";

		public const string ReportingServicesJs = "ReportingServices.js";

		public const string JQueryJs = "jquery.min.js";

		public const string JQueryUiJs = "jqueryui.min.js";

		public const string KnockoutJs = "knockoutjs.js";

		public const string ReportingServicesHybridJs = "ReportingServicesHybrid.js";

		public const string PrintControlCab = "RSClientPrint.cab";

		private static ResourceList m_resourceList;

		static HTMLRendererResources()
		{
			HTMLRendererResources.m_resourceList = new ResourceList();
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.TogglePlus.gif", "image/gif");
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.ToggleMinus.gif", "image/gif");
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.sortAsc.gif", "image/gif");
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.sortDesc.gif", "image/gif");
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.unsorted.gif", "image/gif");
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.Blank.gif", "image/gif");
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.PushPin.png", "image/png");
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.Common.js", "application/javascript", true);
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.FitProportional.js", "application/javascript", true);
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.FixedHeader.js", "application/javascript", true);
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.CanGrowFalse.js", "application/javascript", true);
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.ImageConsolidation.js", "application/javascript", true);
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.ReportingServices.js", "application/javascript", true);
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.Html40Viewer.css", "text/css", true);
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.Html5Renderer.css", "text/css", true);
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.Html5Toolbar.css", "text/css", true);
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.jqueryui.min.css", "text/css", true);
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.ReportingServices40.css", "text/css", true);
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.ReportingServicesHybrid.css", "text/css", true);
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.ReportingServices.css", "text/css", true);
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.Html5Renderer.js", "application/javascript", true);
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.Html5RenderingExtensionJs.js", "application/javascript", true);
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.jquery.min.js", "application/javascript", true);
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.jqueryui.min.js", "application/javascript", true);
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.knockoutjs.js", "application/javascript", true);
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.ReportingServicesHybrid.js", "application/javascript", true);
			HTMLRendererResources.m_resourceList.Add("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.RSClientPrint.cab", "application/vnd.ms-cab-compressed");
		}

		public static void PopulateResources(Dictionary<string, byte[]> nameToResourceMap, string prefix)
		{
			Encoding uTF = Encoding.UTF8;
			nameToResourceMap["PushPin.png"] = HTMLRendererResources.CreateFullName(uTF, prefix, "PushPin.png");
			nameToResourceMap["TogglePlus.gif"] = HTMLRendererResources.CreateFullName(uTF, prefix, "TogglePlus.gif");
			nameToResourceMap["ToggleMinus.gif"] = HTMLRendererResources.CreateFullName(uTF, prefix, "ToggleMinus.gif");
			nameToResourceMap["sortAsc.gif"] = HTMLRendererResources.CreateFullName(uTF, prefix, "sortAsc.gif");
			nameToResourceMap["sortDesc.gif"] = HTMLRendererResources.CreateFullName(uTF, prefix, "sortDesc.gif");
			nameToResourceMap["unsorted.gif"] = HTMLRendererResources.CreateFullName(uTF, prefix, "unsorted.gif");
			nameToResourceMap["Blank.gif"] = HTMLRendererResources.CreateFullName(uTF, prefix, "Blank.gif");
		}

		public static Stream GetStream(string name, out string mimeType)
		{
			return EmbeddedResources.GetStream(HTMLRendererResources.m_resourceList, "AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources." + name, out mimeType);
		}

		private static byte[] CreateFullName(Encoding encoding, string prefix, string name)
		{
			return encoding.GetBytes(prefix + name);
		}

		public static byte[] GetBytes(string name)
		{
			string text = null;
			return HTMLRendererResources.GetBytes(name, out text);
		}

		public static byte[] GetBytes(string name, out string mimeType)
		{
			return EmbeddedResources.Get(HTMLRendererResources.m_resourceList, "AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources." + name, out mimeType);
		}

		public static byte[] GetBytesFullname(string nameWithNamespace, out string mimeType)
		{
			return EmbeddedResources.Get(HTMLRendererResources.m_resourceList, nameWithNamespace, out mimeType);
		}
	}
}
