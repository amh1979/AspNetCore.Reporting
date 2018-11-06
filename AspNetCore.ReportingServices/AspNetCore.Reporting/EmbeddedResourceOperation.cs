using System.Web;
using AspNetCore.ReportingServices.Rendering.HtmlRenderer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Web;

namespace AspNetCore.Reporting
{
	internal class EmbeddedResourceOperation : HandlerOperation
	{
		private const string UrlParamName = "Name";

		private const string ResourceNameScript = "ViewerScript";

		private static byte[] m_viewerScript;

		private static string m_assemblyVersion;

		public override bool IsCacheable
		{
			get
			{
				return true;
			}
		}

		private static string ProductVersion
		{
			get
			{
				return EmbeddedResourceOperation.m_assemblyVersion;
			}
		}

		public static string CreateUrl(string resourceName)
		{
			return EmbeddedResourceOperation.CreateUrl(resourceName, "Resource");
		}

		public static string CreateUrlForScriptFile()
		{
			return EmbeddedResourceOperation.CreateUrl("ViewerScript");
		}

		protected static string CreateUrl(string resourceName, string operationType, params string[] additionalParams)
		{

			StringBuilder stringBuilder = new StringBuilder();
			foreach (string str in additionalParams)
			{
				stringBuilder.Append(str + "&");
			}
			//UriBuilder handlerUri = ReportViewerFactory.HttpHandler.HandlerUri;
			return "OpType=" + HttpUtility.UrlEncode(operationType) + "&Version=" + EmbeddedResourceOperation.ProductVersion + "&" + stringBuilder.ToString() + "Name=" + HttpUtility.UrlEncode(resourceName);
			//return handlerUri.Uri.PathAndQuery;
		}

		public static string CreateReference(string resourceName)
		{
			return "{" + resourceName + "}";
		}

		protected virtual byte[] GetResource(string resourceName, out string mimeType)
		{
			return this.GetResource(resourceName, out mimeType, (NameValueCollection)null);
		}

		protected virtual byte[] GetResource(string resourceName, out string mimeType, NameValueCollection urlQuery)
		{
			if (resourceName.StartsWith("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.", StringComparison.OrdinalIgnoreCase))
			{
				return HTMLRendererResources.GetBytesFullname(resourceName, out mimeType);
			}
			return ReportViewerEmbeddedResources.Get(resourceName, out mimeType);
		}

		public override void PerformOperation(NameValueCollection urlQuery, HttpResponse response)
		{
			string andEnsureParam = HandlerOperation.GetAndEnsureParam(urlQuery, "Name");
			string contentType = null;
			byte[] array = null;
			if (!string.IsNullOrEmpty(andEnsureParam))
			{
				if (andEnsureParam.Equals("ViewerScript", StringComparison.Ordinal))
				{
					array = this.GetViewerScript();
					contentType = "application/javascript";
				}
				else
				{
					array = this.GetResource(andEnsureParam, out contentType, urlQuery);
				}
			}
			if (array != null)
			{
				response.ContentType = contentType;
				//response.BinaryWrite(array);
				//DateTime expires = DateTime.Now.AddMonths(1);
				//response.Cache.SetExpires(expires);
			}
			else
			{
				response.StatusCode = 404;
			}
		}

		private byte[] GetViewerScript()
		{
			if (EmbeddedResourceOperation.m_viewerScript == null)
			{
				List<byte> list = new List<byte>(327680);
				this.LoadScriptFiles(list);
				list.AddRange(Encoding.UTF8.GetBytes("\nif (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();"));
				EmbeddedResourceOperation.m_viewerScript = list.ToArray();
			}
			return EmbeddedResourceOperation.m_viewerScript;
		}

		protected virtual void LoadScriptFiles(List<byte> viewerScript)
		{
			string text = default(string);
			viewerScript.AddRange(this.GetResource("AspNetCore.Reporting.Scripts.Common.js", out text));
			viewerScript.AddRange(this.GetResource("AspNetCore.Reporting.Scripts.ToolbarImageHolder.js", out text));
			viewerScript.AddRange(this.GetResource("AspNetCore.Reporting.Scripts.HoverImage.js", out text));
			viewerScript.AddRange(this.GetResource("AspNetCore.Reporting.Scripts.StickyImage.js", out text));
			viewerScript.AddRange(this.GetResource("AspNetCore.Reporting.Scripts.InternalReportViewer.js", out text));
			viewerScript.AddRange(this.GetResource("AspNetCore.Reporting.Scripts.ParameterInputControls.js", out text));
			viewerScript.AddRange(this.GetResource("AspNetCore.Reporting.Scripts.PromptArea.js", out text));
			viewerScript.AddRange(this.GetResource("AspNetCore.Reporting.Scripts.ReportArea.js", out text));
			viewerScript.AddRange(this.GetResource("AspNetCore.Reporting.Scripts.ReportPage.js", out text));
			viewerScript.AddRange(this.GetResource("AspNetCore.Reporting.Scripts.SessionKeepAlive.js", out text));
			viewerScript.AddRange(this.GetResource("AspNetCore.Reporting.Scripts.ScriptStickyImage.js", out text));
			viewerScript.AddRange(this.GetResource("AspNetCore.Reporting.Scripts.ScriptSwitchImage.js", out text));
			viewerScript.AddRange(this.GetResource("AspNetCore.Reporting.Scripts.TextButton.js", out text));
			viewerScript.AddRange(this.GetResource("AspNetCore.Reporting.Scripts.Toolbar.js", out text));
			viewerScript.AddRange(this.GetResource("AspNetCore.Reporting.Scripts.ReportViewer.js", out text));
			viewerScript.AddRange(this.GetResource("AspNetCore.Reporting.Scripts.ToolbarMenu.js", out text));
			viewerScript.AddRange(this.GetResource("AspNetCore.Reporting.Scripts.Splitter.js", out text));
			viewerScript.AddRange(this.GetResource("AspNetCore.Reporting.Scripts.ResizableBehavior.js", out text));
			viewerScript.AddRange(this.GetResource("AspNetCore.Reporting.Scripts.AsyncWaitControl.js", out text));
			viewerScript.AddRange(this.GetResource("AspNetCore.Reporting.Scripts.DocMapArea.js", out text));
			viewerScript.AddRange(LocalHtmlRenderer.GetResource("Common.js", out text));
			viewerScript.AddRange(LocalHtmlRenderer.GetResource("FitProportional.js", out text));
			viewerScript.AddRange(LocalHtmlRenderer.GetResource("FixedHeader.js", out text));
			viewerScript.AddRange(LocalHtmlRenderer.GetResource("CanGrowFalse.js", out text));
			viewerScript.AddRange(LocalHtmlRenderer.GetResource("ImageConsolidation.js", out text));
		}

	}
}
