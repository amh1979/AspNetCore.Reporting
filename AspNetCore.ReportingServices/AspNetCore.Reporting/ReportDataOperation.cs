//using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;

namespace AspNetCore.Reporting
{
	internal abstract class ReportDataOperation : ViewerDataOperation
	{
		private const string ParamCulture = "Culture";

		private const string ParamCultureUI = "UICulture";

		private const string ParamCultureUserOverride = "CultureOverrides";

		private const string ParamCultureUIUserOverride = "UICultureOverrides";

		private const string ParamDrillDepth = "ReportStack";

		protected ReportControlSession m_reportControlSession;

		protected ReportDataOperation(IReportServerConnectionProvider connectionProvider, bool requiresFullReportLoad = true)
			: base(connectionProvider)
		{

            NameValueCollection requestParameters = new NameValueCollection();
            //HttpHandler.RequestParameters;
			bool flag = base.ProcessingMode == ProcessingMode.Local;
			if (base.IsUsingSession)
			{
				ReportHierarchy reportHierarchy = base.ReportHierarchy;
				int clientStackSize = HandlerOperation.ParseRequiredInt(requestParameters, "ReportStack");
				
				ReportInfo reportInfo = reportHierarchy.Peek();
				if (flag)
				{
					this.m_reportControlSession = reportInfo.LocalSession;
				}
				else
				{
					//this.m_reportControlSession = reportInfo.ServerSession;
				}
			}
			else
			{
				if (flag)
				{
					throw new HttpHandlerInputException(new NotSupportedException());
				}
				//ServerReport serverReport = base.CreateTempServerReport();
				//serverReport.LoadFromUrlQuery(requestParameters, requiresFullReportLoad);
				//this.m_reportControlSession = new ServerModeSession(serverReport);
			}
			int culture = HandlerOperation.ParseRequiredInt(requestParameters, "Culture");
			int culture2 = HandlerOperation.ParseRequiredInt(requestParameters, "UICulture");
			bool useUserOverride = HandlerOperation.ParseRequiredBool(requestParameters, "CultureOverrides");
			bool useUserOverride2 = HandlerOperation.ParseRequiredBool(requestParameters, "UICultureOverrides");
			Thread.CurrentThread.CurrentCulture = new CultureInfo(culture, useUserOverride);
			Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture2, useUserOverride2);
		}

		public override void Dispose()
		{
			if (this.m_reportControlSession != null)
			{
				if (!base.IsUsingSession)
				{
					this.m_reportControlSession.Dispose();
				}
				else
				{
					this.m_reportControlSession.DisposeNonSessionResources();
				}
			}
			base.Dispose();
			GC.SuppressFinalize(this);
		}

		protected static string BaseQuery(Report report, string instanceID)
		{
			//ServerReport serverReport = report as ServerReport;
			Uri reportServerUri = null;
			string value = string.Empty;
			
			StringBuilder stringBuilder = new StringBuilder(value);
			stringBuilder.AppendFormat("&{0}={1}", "Culture", CultureInfo.CurrentCulture.LCID.ToString(CultureInfo.InvariantCulture));
			stringBuilder.AppendFormat("&{0}={1}", "CultureOverrides", CultureInfo.CurrentCulture.UseUserOverride.ToString(CultureInfo.InvariantCulture));
			stringBuilder.AppendFormat("&{0}={1}", "UICulture", CultureInfo.CurrentUICulture.LCID.ToString(CultureInfo.InvariantCulture));
			stringBuilder.AppendFormat("&{0}={1}", "UICultureOverrides", CultureInfo.CurrentUICulture.UseUserOverride.ToString(CultureInfo.InvariantCulture));
			if (report != null)
			{
				stringBuilder.AppendFormat("&{0}={1}", "ReportStack", report.DrillthroughDepth.ToString(CultureInfo.InvariantCulture));
			}
			string value2 = ViewerDataOperation.ViewerDataOperationQuery(reportServerUri, instanceID);
			stringBuilder.Append(value2);
			if (stringBuilder[0] == '&')
			{
				stringBuilder.Remove(0, 1);
			}
			return stringBuilder.ToString();
		}

		internal static void SetStreamingHeaders(string mimeType, HttpResponse response)
		{
			//response.BufferOutput = false;
			if (!string.IsNullOrEmpty(mimeType))
			{
				response.ContentType = mimeType;
			}
			//response.Expires = -1;
		}

		internal static void StreamToResponse(Stream data, string mimeType, HttpResponse response)
		{
			ReportDataOperation.SetStreamingHeaders(mimeType, response);
			ReportDataOperation.StreamToResponse(data, response);
		}

		internal static void StreamToResponse(Stream data, HttpResponse response)
		{
			
		}


	}
}
