using System.Web;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Web;
using System.Xml;

namespace AspNetCore.Reporting
{
	internal sealed class ReportImageOperation : ReportDataOperation
	{
		private const string UrlParamStreamID = "StreamID";

		private const string UrlParamResourceStreamID = "ResourceStreamID";

		private const string UrlParamIterationId = "IterationId";

		public ReportImageOperation(IReportServerConnectionProvider connectionProvider)
			: base(connectionProvider, false)
		{
		}

		public static string CreateUrl(Report report, string instanceID, bool isResourceStreamRoot)
		{
			return ReportImageOperation.CreateUrl(report, instanceID, isResourceStreamRoot, null);
		}

		private static string CreateUrl(Report report, string instanceID, bool isResourceStreamRoot, string iterationId)
		{
			string str = isResourceStreamRoot ? "ResourceStreamID" : "StreamID";
			//UriBuilder handlerUri = ReportViewerFactory.HttpHandler.HandlerUri;
			string str2 = ReportDataOperation.BaseQuery(report, instanceID) + "&OpType=ReportImage&";
			if (!isResourceStreamRoot)
			{
				if (iterationId == null)
				{
					iterationId = Guid.NewGuid().ToString("N");
				}
				str2 = str2 + "IterationId=" + HttpUtility.UrlEncode(iterationId) + "&";
			}
			str2 = ( str2 + str + "=");
			return str2;
		}

		public override void PerformOperation(NameValueCollection urlQuery, HttpResponse response)
		{
			string text = urlQuery["StreamID"];
			string text2 = urlQuery["ResourceStreamID"];
			if (text != null && text.Length > 0)
			{
				string andEnsureParam = HandlerOperation.GetAndEnsureParam(urlQuery, "IterationId");
				this.GetStreamImage(text, response, andEnsureParam);
				return;
			}
			if (text2 != null && text2.Length > 0)
			{
				this.GetRendererImage(text2, response);
				return;
			}
			throw new HttpHandlerInputException(Errors.MissingUrlParameter("StreamID"));
		}

		private void GetStreamImage(string streamID, HttpResponse response, string iterationId)
		{
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			xmlTextWriter.WriteStartElement("DeviceInfo");
			string value = ReportImageOperation.CreateUrl(base.m_reportControlSession.Report, base.InstanceID, false, iterationId);
			xmlTextWriter.WriteElementString("StreamRoot", value);
			xmlTextWriter.WriteEndElement();
			string mimeType = default(string);
			byte[] streamImage = base.m_reportControlSession.GetStreamImage(streamID, stringWriter.ToString(), out mimeType);
			this.WriteBytesToResponse(streamImage, mimeType, response);
		}

		private void GetRendererImage(string resourceID, HttpResponse response)
		{
			string mimeType = default(string);
			byte[] rendererImage = base.m_reportControlSession.GetRendererImage(resourceID, out mimeType);
			this.WriteBytesToResponse(rendererImage, mimeType, response);
		}

		private void WriteBytesToResponse(byte[] bytes, string mimeType, HttpResponse response)
		{
			if (bytes != null && bytes.Length > 0)
			{
				response.ContentType = mimeType;
				//response.BinaryWrite(bytes);
			}
			else
			{
				response.StatusCode = 404;
			}
		}
	}
}
