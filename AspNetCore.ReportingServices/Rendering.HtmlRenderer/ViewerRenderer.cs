using AspNetCore.Reporting;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.Rendering.SPBProcessing;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;


namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal class ViewerRenderer : HTML4Renderer
	{
		private sealed class DetachedReportWrapper : IReportWrapper
		{
			private string m_StreamRoot;

			private string m_ReportLocation;

			private bool m_HasBookmarks;

			private string m_SortItem;

			private string m_ShowHideToggle;

			private Encoding m_encoding = Encoding.UTF8;

			private Dictionary<string, byte[]> m_imageMap = new Dictionary<string, byte[]>();

			public bool HasBookmarks
			{
				get
				{
					return this.m_HasBookmarks;
				}
			}

			public string SortItem
			{
				get
				{
					return this.m_SortItem;
				}
			}

			public string ShowHideToggle
			{
				get
				{
					return this.m_ShowHideToggle;
				}
			}

			public DetachedReportWrapper(string aStreamRoot)
			{
				this.m_StreamRoot = aStreamRoot;
			}

			public string GetStreamUrl(bool useSessionId, string streamName)
			{
				if (this.m_StreamRoot != null && this.m_StreamRoot != string.Empty)
				{
					StringBuilder stringBuilder = new StringBuilder(this.m_StreamRoot);
					if (streamName != null)
					{
						stringBuilder.Append(streamName);
					}
					return stringBuilder.ToString();
				}
				return null;
			}

			public string GetReportUrl(bool addParams)
			{
				return this.m_ReportLocation;
			}

			public byte[] GetImageName(string imageID)
			{
				if (this.m_imageMap.ContainsKey(imageID))
				{
					return this.m_imageMap[imageID];
				}
				byte[] bytes = this.m_encoding.GetBytes(imageID);
				this.m_imageMap[imageID] = bytes;
				return bytes;
			}
		}

		private sealed class SPBProcessingStub : ISPBProcessing
		{
			private const string DEVICE_INFO_TEMPLATE = "<DeviceInfo><StartPage>{0}</StartPage><EndPage>{1}</EndPage><ToggleItems>{2}</ToggleItems><MeasureItems>{3}</MeasureItems><SecondaryStreams>{4}</SecondaryStreams><StreamNames>{5}</StreamNames><StreamRoot>{6}</StreamRoot><RPLVersion>{7}</RPLVersion><ImageConsolidation>{8}</ImageConsolidation></DeviceInfo>";

			private ReportControlSession m_reportControlSession;

			private PageCountMode m_pageCountMode;

			private string m_streamRoot;

			private SPBContext m_spbContext = new SPBContext();

			public bool Done
			{
				get
				{
					return true;
				}
			}

			internal SPBProcessingStub(ReportControlSession reportControlSession, string streamRoot, PageCountMode pageCountMode)
			{
				this.m_reportControlSession = reportControlSession;
				this.m_pageCountMode = pageCountMode;
				this.m_spbContext.StartPage = 1;
				this.m_spbContext.EndPage = 1;
				this.m_spbContext.MeasureItems = false;
				this.m_spbContext.AddToggledItems = false;
				this.m_spbContext.SecondaryStreams = SecondaryStreams.Server;
				this.m_spbContext.AddSecondaryStreamNames = true;
				this.m_spbContext.UseImageConsolidation = true;
				this.m_streamRoot = streamRoot;
			}

			public Stream GetNextPage(out RPLReport rplReport)
			{
				string text = "10.6";
				string deviceInfo = string.Format(CultureInfo.InvariantCulture, "<DeviceInfo><StartPage>{0}</StartPage><EndPage>{1}</EndPage><ToggleItems>{2}</ToggleItems><MeasureItems>{3}</MeasureItems><SecondaryStreams>{4}</SecondaryStreams><StreamNames>{5}</StreamNames><StreamRoot>{6}</StreamRoot><RPLVersion>{7}</RPLVersion><ImageConsolidation>{8}</ImageConsolidation></DeviceInfo>", this.m_spbContext.StartPage, this.m_spbContext.EndPage, this.m_spbContext.AddToggledItems, this.m_spbContext.MeasureItems, this.m_spbContext.SecondaryStreams.ToString(), this.m_spbContext.AddSecondaryStreamNames, this.m_streamRoot, text, this.m_spbContext.UseImageConsolidation);
				NameValueCollection nameValueCollection = new NameValueCollection();
				nameValueCollection.Add("rs:PageCountMode", this.m_pageCountMode.ToString());
				string text2 = default(string);
				string text3 = default(string);
				Stream stream = this.m_reportControlSession.RenderReport("RPL", true, deviceInfo, nameValueCollection, true, out text2, out text3);
				rplReport = null;
				if (stream == null || stream.Length <= 0)
				{
					if (stream != null)
					{
						stream.Dispose();
					}
					stream = null;
					int totalPages = this.m_reportControlSession.Report.GetTotalPages();
					if (totalPages < this.m_spbContext.EndPage)
					{
						this.m_spbContext.EndPage = totalPages;
						if (this.m_spbContext.StartPage > this.m_spbContext.EndPage)
						{
							this.m_spbContext.StartPage = this.m_spbContext.EndPage;
						}
						deviceInfo = string.Format(CultureInfo.InvariantCulture, "<DeviceInfo><StartPage>{0}</StartPage><EndPage>{1}</EndPage><ToggleItems>{2}</ToggleItems><MeasureItems>{3}</MeasureItems><SecondaryStreams>{4}</SecondaryStreams><StreamNames>{5}</StreamNames><StreamRoot>{6}</StreamRoot><RPLVersion>{7}</RPLVersion><ImageConsolidation>{8}</ImageConsolidation></DeviceInfo>", this.m_spbContext.StartPage, this.m_spbContext.EndPage, this.m_spbContext.AddToggledItems, this.m_spbContext.MeasureItems, this.m_spbContext.SecondaryStreams.ToString(), this.m_spbContext.AddSecondaryStreamNames, this.m_streamRoot, text, this.m_spbContext.UseImageConsolidation);
						stream = this.m_reportControlSession.RenderReport("RPL", true, deviceInfo, nameValueCollection, true, out text2, out text3);
					}
				}
				if (stream != null && stream.Length > 0)
				{
					BinaryReader reader = new BinaryReader(stream, Encoding.Unicode);
					rplReport = new RPLReport(reader);
				}
				return stream;
			}

			public void SetContext(SPBContext spbContext)
			{
				this.m_spbContext = spbContext;
			}
		}

		internal string PageStyle;

		private string m_fixedHeaderScript;

		internal string FixedHeaderScript
		{
			get
			{
				return this.m_fixedHeaderScript;
			}
		}

		public ViewerRenderer(ReportControlSession reportControlSession, AspNetCore.ReportingServices.Interfaces.CreateAndRegisterStream streamCallback, ViewerRendererDeviceInfo deviceInfo, NameValueCollection browserCaps, SecondaryStreams secondaryStreams, PageCountMode pageCountMode)
			: base(new DetachedReportWrapper(deviceInfo.RawDeviceInfo["StreamRoot"] ?? ""), new SPBProcessingStub(reportControlSession, HttpUtility.HtmlEncode(deviceInfo.RawDeviceInfo["StreamRoot"] ?? ""), pageCountMode), new NameValueCollection(), deviceInfo, deviceInfo.RawDeviceInfo, browserCaps, streamCallback, secondaryStreams)
		{
		}

		public override void Render(TextWriter outputWriter)
		{
			base.InitializeReport();
			base.m_encoding = outputWriter.Encoding;
			base.m_mainStream = Utility.CreateBufferedStream(outputWriter);
			string styleStreamName = ViewerRenderer.GetStyleStreamName(base.m_pageNum);
			Stream sourceStream = this.CreateStyleStream(styleStreamName);
			base.m_styleStream = Utility.CreateBufferedStream(sourceStream);
			string str = base.m_deviceInfo.HtmlPrefixId + "oReportDiv";
			base.m_styleClassPrefix = base.m_encoding.GetBytes("#" + str + " ");
			base.RenderHtmlBody();
			base.RenderSecondaryStreamSpanTagsForJavascriptFunctions();
			base.m_mainStream.Flush();
			base.m_styleStream.Flush();
			this.m_fixedHeaderScript = this.GetFixedHeaderScripts();
			Stream mainStream = base.m_mainStream;
			base.m_mainStream = base.m_styleStream;
			base.PredefinedStyles();
			base.m_styleStream.Flush();
			base.m_mainStream = mainStream;
		}

		private Stream CreateStyleStream(string styleStreamName)
		{
			return base.CreateStream(styleStreamName, "css", Encoding.UTF8, "text/css", false, AspNetCore.ReportingServices.Interfaces.StreamOper.CreateAndRegister);
		}

		public static string GetStyleStreamName(int pageNumber)
		{
			return HTML4Renderer.GetStyleStreamName("STYLESTREAM", pageNumber);
		}

		protected override void RenderInteractionAction(RPLAction action, ref bool hasHref)
		{
			base.RenderControlActionScript(action);
			this.WriteStream(HTML4Renderer.m_href);
			this.WriteStream(HTML4Renderer.m_quote);
			base.OpenStyle();
			this.WriteStream(HTML4Renderer.m_cursorHand);
			this.WriteStream(HTML4Renderer.m_semiColon);
			hasHref = true;
		}

		protected override void RenderSortAction(RPLTextBoxProps textBoxProps, RPLFormat.SortOptions sortState)
		{
			this.WriteStream(HTML4Renderer.m_openStyle);
			this.WriteStream(HTML4Renderer.m_cursorHand);
			this.WriteStream(HTML4Renderer.m_semiColon);
			this.WriteStream(HTML4Renderer.m_quote);
			string uniqueName = textBoxProps.UniqueName;
			uniqueName = ((sortState != RPLFormat.SortOptions.Descending && sortState != 0) ? (uniqueName + "_D") : (uniqueName + "_A"));
			base.RenderOnClickActionScript("Sort", uniqueName);
			this.WriteStream(HTML4Renderer.m_closeBracket);
		}

		protected override void RenderInternalImageSrc()
		{
			base.WriteAttrEncoded(base.m_deviceInfo.ResourceStreamRoot);
		}

		protected override void RenderToggleImage(RPLTextBoxProps textBoxProps)
		{
			bool toggleState = textBoxProps.ToggleState;
			if (textBoxProps.IsToggleParent)
			{
				this.WriteStream(HTML4Renderer.m_openA);
				this.WriteStream(HTML4Renderer.m_tabIndex);
				base.WriteStream(++base.m_tabIndexNum);
				this.WriteStream(HTML4Renderer.m_quote);
				this.WriteStream(HTML4Renderer.m_openStyle);
				this.WriteStream(HTML4Renderer.m_cursorHand);
				this.WriteStream(HTML4Renderer.m_semiColon);
				this.WriteStream(HTML4Renderer.m_quote);
				base.RenderOnClickActionScript("Toggle", textBoxProps.UniqueName);
				this.WriteStream(HTML4Renderer.m_closeBracket);
				this.WriteStream(HTML4Renderer.m_img);
				if (base.m_browserIE)
				{
					this.WriteStream(HTML4Renderer.m_imgOnError);
				}
				this.WriteStream(HTML4Renderer.m_zeroBorder);
				this.WriteStream(HTML4Renderer.m_src);
				this.WriteToggleImage(toggleState);
				this.WriteStream(HTML4Renderer.m_quote);
				this.WriteStream(HTML4Renderer.m_alt);
				if (toggleState)
				{
					this.WriteStream(RenderRes.ToggleStateCollapse);
				}
				else
				{
					this.WriteStream(RenderRes.ToggleStateExpand);
				}
				this.WriteStream(HTML4Renderer.m_closeTag);
				this.WriteStream(HTML4Renderer.m_closeA);
				this.WriteStream(HTML4Renderer.m_nbsp);
			}
		}

		private void WriteToggleImage(bool toggleState)
		{
			if (toggleState)
			{
				this.WriteStream(EmbeddedResourceOperation.CreateUrl("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.ToggleMinus.gif"));
			}
			else
			{
				this.WriteStream(EmbeddedResourceOperation.CreateUrl("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.TogglePlus.gif"));
			}
		}

		protected override void RenderSortImageText(RPLFormat.SortOptions sortState)
		{
			switch (sortState)
			{
			case RPLFormat.SortOptions.Ascending:
				this.WriteStream(EmbeddedResourceOperation.CreateUrl("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.sortAsc.gif"));
				break;
			case RPLFormat.SortOptions.Descending:
				this.WriteStream(EmbeddedResourceOperation.CreateUrl("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.sortDesc.gif"));
				break;
			default:
				this.WriteStream(EmbeddedResourceOperation.CreateUrl("AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources.unsorted.gif"));
				break;
			}
		}

		protected override void WriteScrollbars()
		{
		}

		protected override void WriteFixedHeaderOnScrollScript()
		{
		}

		protected override void WriteFixedHeaderPropertyChangeScript()
		{
		}

		protected override void WriteFitProportionalScript(double pv, double ph)
		{
			this.WriteStream(HTML4Renderer.m_onLoadFitProportionalPv);
			this.WriteStream(Utility.MmToPxAsString(pv));
			this.WriteStream(";this.ph=");
			this.WriteStream(Utility.MmToPxAsString(ph));
			this.WriteStream(";\"");
		}

		protected override void RenderPageStart(bool firstPage, bool lastPage, RPLElementStyle pageStyle)
		{
			this.WriteStream(HTML4Renderer.m_openDiv);
			this.WriteStream(HTML4Renderer.m_ltrDir);
			this.WriteStream(HTML4Renderer.m_openStyle);
			if (base.m_deviceInfo.IsBrowserIE)
			{
				this.WriteStream(HTML4Renderer.m_styleHeight);
				this.WriteStream(HTML4Renderer.m_percent);
				this.WriteStream(HTML4Renderer.m_semiColon);
			}
			this.WriteStream(HTML4Renderer.m_styleWidth);
			this.WriteStream(HTML4Renderer.m_percent);
			this.WriteStream(HTML4Renderer.m_semiColon);
			this.WriteStream("direction:ltr");
			this.WriteStream(HTML4Renderer.m_quote);
			this.RenderReportItemId("oReportDiv");
			if (base.m_pageHasStyle)
			{
				Stream mainStream = base.m_mainStream;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					base.m_mainStream = memoryStream;
					base.RenderBackgroundStyleProps(pageStyle);
					int num = 0;
					base.RenderHtmlBorders(pageStyle, ref num, 0, true, true, null);
					Encoding encoding = new UTF8Encoding(false);
					this.PageStyle = encoding.GetString(memoryStream.ToArray());
				}
				base.m_mainStream = mainStream;
				mainStream = null;
			}
			else
			{
				this.PageStyle = null;
			}
			this.WriteStream(HTML4Renderer.m_closeBracket);
			this.WriteStream(HTML4Renderer.m_openTable);
			this.WriteStream(HTML4Renderer.m_closeBracket);
			this.WriteStream(HTML4Renderer.m_firstTD);
			this.RenderReportItemId("oReportCell");
			base.RenderZoom();
			this.WriteStream(HTML4Renderer.m_closeBracket);
		}

		protected override void RenderPageEnd()
		{
			this.WriteStream(HTML4Renderer.m_lastTD);
			this.WriteStream(HTML4Renderer.m_closeTable);
			this.WriteStream(HTML4Renderer.m_closeDiv);
		}

		private string GetFixedHeaderScripts()
		{
			if (base.m_fixedHeaders != null && base.m_fixedHeaders.Count != 0 && base.m_hasOnePage)
			{
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				base.RenderCreateFixedHeaderFunction("this.", "this.m_fixedHeader", stringBuilder2, stringBuilder, true);
				StringBuilder stringBuilder3 = new StringBuilder();
				stringBuilder3.Append("function(firstTime) {");
				stringBuilder3.Append("if(firstTime){");
				stringBuilder3.Append(stringBuilder);
				stringBuilder3.Append("}");
				stringBuilder3.Append(stringBuilder2);
				stringBuilder3.Append("}");
				return stringBuilder3.ToString();
			}
			return null;
		}
	}
}
