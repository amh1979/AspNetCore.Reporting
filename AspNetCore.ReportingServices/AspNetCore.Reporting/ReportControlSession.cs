using System.Web;
using AspNetCore.ReportingServices.Interfaces;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Security;
 
using Html = AspNetCore.ReportingServices.Rendering.HtmlRenderer;
using System.Text;

namespace AspNetCore.Reporting
{
	[Serializable]
	internal abstract class ReportControlSession : IDisposable
	{
		protected AspNetCore.Reporting.StreamCache m_htmlStreamCache = new AspNetCore.Reporting.StreamCache();

		private AspNetCore.Reporting.CreateStreamDelegate CreateStreamCallback
		{
			get
			{
                ITemporaryStorage tempStorage = null;// WebConfigReader.Current.TempStorage;
				if (tempStorage == null)
				{
					return null;
				}
				return () => new MemoryThenTempStorageStream(tempStorage);
			}
		}

		public abstract Report Report
		{
			get;
		}

		public void Dispose()
		{
			this.m_htmlStreamCache.Dispose();
			GC.SuppressFinalize(this);
		}

		public abstract void DisposeNonSessionResources();

        public void RenderReportHTML(string deviceInfo, PageCountMode pageCountMode, CreateAndRegisterStream createStreamCallback, out string scrollScript, out string pageStyle)
        {
            var di = ReportingServices.Diagnostics.RSRequestParameters.ShallowXmlToNameValueCollection(deviceInfo, "DeviceInfo");
            var info = Html.HTMLRenderer.CreateDeviceInfo(1, "", "", "", "", false, false);
            LocalHtmlRenderer.Render(CombineDeviceInfo(info,di), pageCountMode, this, createStreamCallback, out scrollScript, out pageStyle);
        }
        NameValueCollection CombineDeviceInfo(NameValueCollection old, NameValueCollection current)
        {
            foreach (string d in current)
            {
                if (!string.IsNullOrEmpty(current[d]))
                    old.Set(d, current[d]);
            }
            return old;
        }
		public Stream RenderReportHTML(NameValueCollection deviceInfo, PageCountMode pageCountMode, out string scrollScript, out string pageStyle)
		{
			this.m_htmlStreamCache.Clear();
			using (AspNetCore.Reporting.StreamCache streamCache = new AspNetCore.Reporting.StreamCache(this.CreateStreamCallback))
			{
				try
				{
					LocalHtmlRenderer.Render(deviceInfo, pageCountMode, this, this.GetStreamCallback(streamCache), out scrollScript, out pageStyle);
				}
				finally
				{
					streamCache.MoveSecondaryStreamsTo(this.m_htmlStreamCache);
				}
				return streamCache.GetMainStream(true);
			}
		}

		[SecurityCritical]
		[SecurityTreatAsSafe]
		private AspNetCore.ReportingServices.Interfaces.CreateAndRegisterStream GetStreamCallback(AspNetCore.Reporting.StreamCache streamCache)
		{
			return streamCache.StreamCallback;
		}

		public byte[] GetRendererImage(string streamID, out string mimeType)
		{
			return LocalHtmlRenderer.GetResource(streamID, out mimeType);
		}

		public void DeliverReportItem(string reportVisualName, string groupId, string dashboardId, string dashboardName, string scheduleInfo)
		{
			DateTime utcNow = DateTime.UtcNow;
			string text = null;
			switch (scheduleInfo)
			{
			case "Hourly":
				text = string.Format(CultureInfo.InvariantCulture, "<ScheduleDefinition xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><StartDateTime xmlns=\"http://schemas.microsoft.com/sqlserver/reporting/2010/03/01/ReportServer\">{0}</StartDateTime>{1}</ScheduleDefinition>", utcNow.ToString("s") + "Z", "<MinuteRecurrence xmlns=\"http://schemas.microsoft.com/sqlserver/reporting/2010/03/01/ReportServer\"><MinutesInterval>60</MinutesInterval></MinuteRecurrence>", CultureInfo.InvariantCulture);
				break;
			case "Daily":
				text = string.Format(CultureInfo.InvariantCulture, "<ScheduleDefinition xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><StartDateTime xmlns=\"http://schemas.microsoft.com/sqlserver/reporting/2010/03/01/ReportServer\">{0}</StartDateTime>{1}</ScheduleDefinition>", utcNow.ToString("s") + "Z", "<DailyRecurrence xmlns=\"http://schemas.microsoft.com/sqlserver/reporting/2010/03/01/ReportServer\"><DaysInterval>1</DaysInterval></DailyRecurrence>");
				break;
			case "Weekly":
				text = string.Format(CultureInfo.InvariantCulture, "<ScheduleDefinition xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><StartDateTime xmlns=\"http://schemas.microsoft.com/sqlserver/reporting/2010/03/01/ReportServer\">{0}</StartDateTime>{1}</ScheduleDefinition>", utcNow.ToString("s") + "Z", string.Format(CultureInfo.InvariantCulture, "<WeeklyRecurrence xmlns=\"http://schemas.microsoft.com/sqlserver/reporting/2010/03/01/ReportServer\"><WeeksInterval>1</WeeksInterval><DaysOfWeek><{0}>true</{0}></DaysOfWeek></WeeklyRecurrence>", utcNow.DayOfWeek));
				break;
			default:
				throw new HttpHandlerInputException("Strings.InvalidUrlParameter(\"frequencyOfUpdate\")");
			}
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection.Add("DashboardID", dashboardId);
			nameValueCollection.Add("ReportVisualName", reportVisualName);
			nameValueCollection.Add("DashboardName", dashboardName);
			nameValueCollection.Add("GroupID", groupId);
			nameValueCollection.Add("ContentType", "Visual");
			ExtensionSettings settings = new ExtensionSettings("Report Server PowerBI", nameValueCollection);
			this.Report.InternalDeliverReportItem("IMAGE", null, settings, string.Empty, "TimedSubscription", text);
		}

		public byte[] GetStreamImage(string streamID, string deviceInfo, out string mimeType)
		{
 
			string text2 = default(string);
			byte[] array = this.m_htmlStreamCache.GetSecondaryStream(true, streamID, out Encoding text, out mimeType, out text2);
			if (array == null)
			{
				array = this.Report.InternalRenderStream("RPL", streamID, deviceInfo, out mimeType, out text);
			}
			return array;
		}

		public abstract Stream RenderReport(string format, bool allowInternalRenderers, string deviceInfo, NameValueCollection additionalParams, bool cacheSecondaryStreamsForHtml, out string mimeType, out string fileExtension);

		public abstract void RenderReportForPrint(string deviceInfo, NameValueCollection additonalParams, HttpResponse response);
	}
}
