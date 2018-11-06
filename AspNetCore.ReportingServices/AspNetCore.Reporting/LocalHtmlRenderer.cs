using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.Rendering.HtmlRenderer;
using AspNetCore.ReportingServices.Rendering.SPBProcessing;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Security;
using System.Text;


namespace AspNetCore.Reporting
{
	internal static class LocalHtmlRenderer
	{
		private const string c_htmlMimeType = "text/html";

		public static byte[] GetResource(string name, out string mimeType)
		{
			try
			{
				return HTMLRendererResources.GetBytes(name, out mimeType);
			}
			catch (Exception renderingException)
			{
				throw  renderingException;
			}
		}

		internal static TextWriter CreateWriter(string streamName, string mimeType, AspNetCore.ReportingServices.Interfaces.CreateAndRegisterStream createStreamCallback, AspNetCore.ReportingServices.Interfaces.StreamOper streamOper)
		{
			Stream stream = LocalHtmlRenderer.CreateHTMLStream(streamName, mimeType, createStreamCallback, streamOper);
            
			TextWriter textWriter =new StreamWriter(stream, new UTF8Encoding(false));
            textWriter.NewLine = null;
			return textWriter;
		}

		[SecurityTreatAsSafe]
		[SecurityCritical]
		internal static Stream CreateHTMLStream(string streamName, string mimeType, AspNetCore.ReportingServices.Interfaces.CreateAndRegisterStream createStreamCallback, AspNetCore.ReportingServices.Interfaces.StreamOper streamOper)
		{
			return createStreamCallback(streamName, "html", Encoding.UTF8, mimeType, false, streamOper);
		}

		public static void Render(NameValueCollection deviceInfo, PageCountMode pageCountMode, ReportControlSession reportControlSession, AspNetCore.ReportingServices.Interfaces.CreateAndRegisterStream streamCallback, out string scrollScript, out string pageStyle)
		{
			TextWriter htmlTextWriter = null;
			try
			{
				deviceInfo.Add("OnlyVisibleStyles", "True");
				htmlTextWriter = LocalHtmlRenderer.CreateWriter(reportControlSession.Report.DisplayNameForUse, "text/html", streamCallback, AspNetCore.ReportingServices.Interfaces.StreamOper.CreateAndRegister);
				NameValueCollection browserCaps = new NameValueCollection();
				ViewerRendererDeviceInfo viewerRendererDeviceInfo = new ViewerRendererDeviceInfo();
				viewerRendererDeviceInfo.ParseDeviceInfo(deviceInfo, new NameValueCollection());
				HTML5ViewerRenderer hTML5ViewerRenderer = new HTML5ViewerRenderer(reportControlSession,streamCallback, viewerRendererDeviceInfo, browserCaps, SecondaryStreams.Server, pageCountMode,null);
				hTML5ViewerRenderer.Render(htmlTextWriter);
				scrollScript = hTML5ViewerRenderer.FixedHeaderScript;
                //var stream=(streamCallback.Target as StreamCache).GetMainStream(true);
                //System.IO.StreamReader sr = new StreamReader(stream);
                //var html=sr.ReadToEnd();
                pageStyle = hTML5ViewerRenderer.PageStyle;
			}			
			catch (LocalProcessingException)
			{
				throw;
			}
			catch (Exception renderingException)
			{
				throw renderingException;
			}
			finally
			{
				if (htmlTextWriter != null)
				{
					htmlTextWriter.Flush();
				}
			}
		}

		public static string GetStyleStreamName(int pageNumber)
		{
			try
			{
				return ViewerRenderer.GetStyleStreamName(pageNumber);
			}
			catch (Exception renderingException)
			{
				throw renderingException;
			}
		}

		private static bool GetBooleanValueFromDeviceInfo(NameValueCollection deviceInfo, string key)
		{
			string value = deviceInfo[key];
			bool result = default(bool);
			if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out result))
			{
				return result;
			}
			return false;
		}
	}
}
