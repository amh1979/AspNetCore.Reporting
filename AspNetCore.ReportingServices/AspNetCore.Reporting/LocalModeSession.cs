using System.Web;
using AspNetCore.ReportingServices.Interfaces;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace AspNetCore.Reporting
{
	[Serializable]
	internal sealed class LocalModeSession : ReportControlSession
	{
		private InternalLocalReport m_localReport;

		public override Report Report
		{
			get
			{
				return this.m_localReport;
			}
		}

		public LocalModeSession()
			: this(new InternalLocalReport())
		{
		}

		public LocalModeSession(InternalLocalReport report)
		{
			this.m_localReport = report;
		}

		public override void DisposeNonSessionResources()
		{
			ReportInfo.DisposeNonSessionResources(this);
		}

		public override Stream RenderReport(string format, bool allowInternalRenderers, string deviceInfo, NameValueCollection additionalParams, bool cacheSecondaryStreamsForHtml, out string mimeType, out string fileNameExtension)
		{
			using (AspNetCore.Reporting.StreamCache streamCache = new AspNetCore.Reporting.StreamCache())
			{
				PageCountMode pageCountMode = PageCountMode.Estimate;
				if (additionalParams != null && string.Compare(additionalParams["rs:PageCountMode"], PageCountMode.Actual.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
				{
					pageCountMode = PageCountMode.Actual;
				}
				Warning[] array = default(Warning[]);
				this.m_localReport.InternalRender(format, allowInternalRenderers, deviceInfo, pageCountMode, (AspNetCore.ReportingServices.Interfaces.CreateAndRegisterStream)streamCache.StreamCallback, out array);
				if (cacheSecondaryStreamsForHtml)
				{
					streamCache.MoveSecondaryStreamsTo(base.m_htmlStreamCache);
				}
 
				return streamCache.GetMainStream(true, out Encoding text, out mimeType, out fileNameExtension);
			}
		}

		public override void RenderReportForPrint(string deviceInfo, NameValueCollection additonalParams, HttpResponse response)
		{
			MemoryStream lastMemoryStream = null;
			ReportDataOperation.SetStreamingHeaders(null, response);
			Warning[] array = default(Warning[]);
			this.m_localReport.Render("IMAGE", deviceInfo, (CreateStreamCallback)delegate
			{
			 
				if (lastMemoryStream != null)
				{
					this.SendPrintStream(lastMemoryStream, response);
					lastMemoryStream.Dispose();
					lastMemoryStream = null;
				}
				lastMemoryStream = new MemoryStream();
				return lastMemoryStream;
			}, out array);
			this.SendPrintStream(lastMemoryStream, response);
			lastMemoryStream.Dispose();
			this.SendPrintStream(null, response);
		}

		private void SendPrintStream(Stream stream, HttpResponse response)
		{
			int value = 0;
			if (stream != null)
			{
				value = (int)stream.Length;
			}
			byte[] bytes = BitConverter.GetBytes(value);
			foreach (byte value2 in bytes)
			{
				//response.OutputStream.WriteByte(value2);
			}
			if (stream != null)
			{
				stream.Position = 0L;
				ReportDataOperation.StreamToResponse(stream, response);
				//response.Flush();
			}
		}
	}
}
