using AspNetCore.ReportingServices.Rendering.HtmlRenderer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;

namespace AspNetCore.Reporting
{
	internal sealed class ReportControl //: CompositeControl, IPostBackEventHandler, IScriptControl
	{
		public const string AutoRefreshParam = "auto";

		private ReportControlSession m_session;

		private Stream m_reportStream;

		private string m_styleBytesString;

		private string m_viewerInstanceIdentifier;

		private int m_pageNumber;

		//private SearchState m_searchState;

		private int m_autoRefreshInterval;

		private string m_alertMessage;

		//private ScrollTarget m_scrollTarget;

		private InteractivityPostBackMode m_interactivityMode;

		private string m_visibleContainerId;

		

		private string m_scrollScript;

		private string m_scrollContainerId;

		private string m_pageStyles;

		private static DeviceInfoNameBlackList m_blackListDeviceInfoNames;

		internal bool EnableHybrid
		{
			get;
			set;
		}

 

		public string ScrollContainerId
		{
			get
			{
				return this.m_scrollContainerId;
			}
			set
			{
				this.m_scrollContainerId = value;
			}
		}

		public string VisibleContainerId
		{
			get
			{
				return this.m_visibleContainerId;
			}
			set
			{
				this.m_visibleContainerId = value;
			}
		}



		private int ViewIteration
		{
            get;set;
		}

		private string UniqueRenderingId
		{
			get
			{
				return ReportControl.MakeUniqueRenderingId(this.m_viewerInstanceIdentifier, this.ViewIteration);
			}
		}

		public event EventHandler<ReportActionEventArgs> ReportAction;

		 
		public ReportControl()
		{
			 
		}

		public void Dispose()
		{
			this.ClearReport();
			 
		}

		

		

		private void RemoveNulls(char[] chars, int count)
		{
			if (chars != null)
			{
				for (int i = 0; i < chars.Length && i < count; i++)
				{
					if (chars[i] == '\0')
					{
						chars[i] = ' ';
					}
				}
			}
		}

		protected  void RenderChildren(TextWriter writer)
		{
			
			if (this.m_reportStream != null)
			{
				//writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
				//writer.RenderBeginTag(HtmlTextWriterTag.Div);
				int num = 40960;
				byte[] array = new byte[num];
				char[] array2 = new char[num];
				int num2 = 0;
				Decoder decoder = Encoding.UTF8.GetDecoder();
				int num3 = 0;
				bool flag = true;
				while ((num2 = this.m_reportStream.Read(array, 0, num)) > 0)
				{
					int chars = decoder.GetChars(array, 0, num2, array2, 0, false);
					if (flag)
					{
						this.RemoveNulls(array2, chars);
					}
					writer.Write(array2, 0, chars);
					num3 += num2;
					if (num3 >= num)
					{
						writer.Flush();
						num3 = 0;
					}
				}
				//writer.RenderEndTag();
			}
		}

		public void ClearReport()
		{
			if (this.m_reportStream != null)
			{
				this.m_scrollScript = null;
				this.m_reportStream.Dispose();
				this.m_reportStream = null;
				this.m_pageStyles = null;
			}
		}



		public int RenderReport(ReportControlSession session, string viewerInstanceIdentifier, PageCountMode pageCountMode, int pageNumber, InteractivityPostBackMode interactivityMode, string searchState, string replacementRoot, string hyperlinkTarget, string alertMessage, DeviceInfoCollection initialDeviceInfos, string browserMode, bool sizeToContent)
		{
			if (this.m_reportStream != null)
			{
				throw new InvalidOperationException();
			}
			this.m_session = session;
			this.ViewIteration++;
			this.m_pageNumber = pageNumber;
		
			this.m_viewerInstanceIdentifier = viewerInstanceIdentifier;
			this.m_alertMessage = alertMessage;
			this.m_interactivityMode = interactivityMode;
			bool useImageConsolidation = !sizeToContent;
			NameValueCollection deviceInfo = this.CreateDeviceInfo(initialDeviceInfos, session.Report, pageNumber, searchState, replacementRoot, hyperlinkTarget, browserMode, useImageConsolidation, this.EnableHybrid);
			this.m_reportStream = session.RenderReportHTML(deviceInfo, pageCountMode, out this.m_scrollScript, out this.m_pageStyles);
			this.m_autoRefreshInterval = session.Report.AutoRefreshInterval;
			int totalPages = session.Report.GetTotalPages();
			if (this.m_pageNumber > totalPages)
			{
				this.m_pageNumber = totalPages;
			}
			this.m_autoRefreshInterval = session.Report.AutoRefreshInterval;
			if (sizeToContent)
			{
				this.m_scrollScript = null;
			}
			string styleStreamName = LocalHtmlRenderer.GetStyleStreamName(pageNumber);
			string text = default(string);
			byte[] streamImage = session.GetStreamImage(styleStreamName, (string)null, out text);
			this.m_styleBytesString = null;
			if (streamImage != null && streamImage.Length > 0)
			{
				Encoding encoding = new UTF8Encoding(false);
				this.m_styleBytesString = encoding.GetString(streamImage);
			}
			return this.m_pageNumber;
		}

		internal static DeviceInfoNameBlackList GetDeviceInfoBlackList()
		{
			if (ReportControl.m_blackListDeviceInfoNames == null)
			{
				ReportControl.m_blackListDeviceInfoNames = new DeviceInfoNameBlackList();
				ReportControl.m_blackListDeviceInfoNames.Add("HTMLFragment");
				ReportControl.m_blackListDeviceInfoNames.Add("Section", Errors.InvalidDeviceInfoSection);
				ReportControl.m_blackListDeviceInfoNames.Add("StreamRoot");
				ReportControl.m_blackListDeviceInfoNames.Add("ResourceStreamRoot");
				ReportControl.m_blackListDeviceInfoNames.Add("ActionScript");
				ReportControl.m_blackListDeviceInfoNames.Add("JavaScript");
				ReportControl.m_blackListDeviceInfoNames.Add("FindString", Errors.InvalidDeviceInfoFind);
				ReportControl.m_blackListDeviceInfoNames.Add("ReplacementRoot");
				ReportControl.m_blackListDeviceInfoNames.Add("PrefixId");
				ReportControl.m_blackListDeviceInfoNames.Add("StyleStream");
				ReportControl.m_blackListDeviceInfoNames.Add("LinkTarget", Errors.InvalidDeviceInfoLinkTarget);
				ReportControl.m_blackListDeviceInfoNames.Add("ExpandContent");
			}
			return ReportControl.m_blackListDeviceInfoNames;
		}

		private NameValueCollection CreateDeviceInfo(DeviceInfoCollection initialDeviceInfos, Report report, int pageNumber, string searchState, string replacementRoot, string linkTarget, string browserMode, bool useImageConsolidation, bool enablePowerBIFeatures)
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			foreach (DeviceInfo initialDeviceInfo in initialDeviceInfos)
			{
				nameValueCollection.Add(initialDeviceInfo.Name, initialDeviceInfo.Value);
			}
			nameValueCollection.Set("HTMLFragment", "true");
			nameValueCollection.Set("Section", pageNumber.ToString(CultureInfo.InvariantCulture));
			string value = ReportImageOperation.CreateUrl(report, this.m_viewerInstanceIdentifier, false);
			nameValueCollection.Set("StreamRoot", value);
			string value2 = ReportImageOperation.CreateUrl(report, this.m_viewerInstanceIdentifier, true);
			nameValueCollection.Set("ResourceStreamRoot", value2);
			nameValueCollection.Set("EnablePowerBIFeatures", enablePowerBIFeatures.ToString());
			//nameValueCollection.Set("ActionScript", this.ActionScriptMethod);
			if (!string.IsNullOrWhiteSpace(searchState))
			{
				nameValueCollection.Set("FindString", searchState);
			}
			if (!string.IsNullOrEmpty(replacementRoot))
			{
				nameValueCollection.Set("ReplacementRoot", replacementRoot);
			}
			nameValueCollection.Set("PrefixId", this.UniqueRenderingId);
			nameValueCollection.Set("StyleStream", "true");
			if (!string.IsNullOrEmpty(linkTarget))
			{
				nameValueCollection.Set("LinkTarget", linkTarget);
			}
			if (HttpContext.Current != null && HttpContext.Current.Request != null)
			{
				string userAgent = HttpContext.Current.Request.Headers["User-Agent"];
				if (userAgent != null)
				{
					nameValueCollection.Set("UserAgent", userAgent);
				}
			}
			if (!string.IsNullOrEmpty(browserMode) && nameValueCollection["BrowserMode"] == null)
			{
				nameValueCollection.Set("BrowserMode", browserMode);
			}
			if (!useImageConsolidation)
			{
				nameValueCollection.Set("ImageConsolidation", "false");
			}
			return nameValueCollection;
		}

		public void RaisePostBackEvent(string eventArgument)
		{
			this.OnReportAction(this, EventArgs.Empty);
		}

		private void OnReportAction(object sender, EventArgs e)
		{

			this.OnReportAction(string.Empty, string.Empty);
		}

		private void OnAutoRefresh(object sender, EventArgs e)
		{
			this.OnReportAction("Refresh", "auto");
		}

		private void OnReportAction(string actionType, string actionParam)
		{
			if (this.ReportAction != null)
			{
				this.ReportAction(this, new ReportActionEventArgs(actionType, actionParam));
			}
		}

		private static string MakeUniqueRenderingId(string instanceId, int viewiteration)
		{
			return string.Format(CultureInfo.InvariantCulture, "P{0}_{1}_", instanceId, viewiteration);
		}

		


	}
}
