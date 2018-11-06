using AspNetCore.Reporting;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.Rendering.SPBProcessing;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;


namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
    internal sealed class HTML5ViewerRenderer : HTML5Renderer
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

        internal sealed class SPBProcessingStub : ISPBProcessing
        {
            private const string DEVICE_INFO_TEMPLATE = "<DeviceInfo><StartPage>{0}</StartPage><EndPage>{1}</EndPage><ToggleItems>{2}</ToggleItems><MeasureItems>{3}</MeasureItems><SecondaryStreams>{4}</SecondaryStreams><StreamNames>{5}</StreamNames><StreamRoot>{6}</StreamRoot><RPLVersion>{7}</RPLVersion><ImageConsolidation>{8}</ImageConsolidation></DeviceInfo>";

            private ReportControlSession m_reportControlSession;

            private PageCountMode m_pageCountMode;

            private string m_streamRoot;

            private SPBContext m_spbContext = new SPBContext();

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
                string deviceInfo = string.Format(CultureInfo.InvariantCulture, "<DeviceInfo><StartPage>{0}</StartPage><EndPage>{1}</EndPage><ToggleItems>{2}</ToggleItems><MeasureItems>{3}</MeasureItems><SecondaryStreams>{4}</SecondaryStreams><StreamNames>{5}</StreamNames><StreamRoot>{6}</StreamRoot><RPLVersion>{7}</RPLVersion><ImageConsolidation>{8}</ImageConsolidation></DeviceInfo>", new object[]
                {
                    this.m_spbContext.StartPage,
                    this.m_spbContext.EndPage,
                    this.m_spbContext.AddToggledItems,
                    this.m_spbContext.MeasureItems,
                    this.m_spbContext.SecondaryStreams.ToString(),
                    this.m_spbContext.AddSecondaryStreamNames,
                    this.m_streamRoot,
                    text,
                    this.m_spbContext.UseImageConsolidation
                });
                NameValueCollection nameValueCollection = new NameValueCollection();
                nameValueCollection.Add("rs:PageCountMode", this.m_pageCountMode.ToString());
                string text2;
                string text3;
                Stream stream = this.m_reportControlSession.RenderReport("RPL", true, deviceInfo, nameValueCollection, true, out text2, out text3);
                rplReport = null;
                if (stream == null || stream.Length <= 0L)
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
                        deviceInfo = string.Format(CultureInfo.InvariantCulture, "<DeviceInfo><StartPage>{0}</StartPage><EndPage>{1}</EndPage><ToggleItems>{2}</ToggleItems><MeasureItems>{3}</MeasureItems><SecondaryStreams>{4}</SecondaryStreams><StreamNames>{5}</StreamNames><StreamRoot>{6}</StreamRoot><RPLVersion>{7}</RPLVersion><ImageConsolidation>{8}</ImageConsolidation></DeviceInfo>", new object[]
                        {
                            this.m_spbContext.StartPage,
                            this.m_spbContext.EndPage,
                            this.m_spbContext.AddToggledItems,
                            this.m_spbContext.MeasureItems,
                            this.m_spbContext.SecondaryStreams.ToString(),
                            this.m_spbContext.AddSecondaryStreamNames,
                            this.m_streamRoot,
                            text,
                            this.m_spbContext.UseImageConsolidation
                        });
                        stream = this.m_reportControlSession.RenderReport("RPL", true, deviceInfo, nameValueCollection, true, out text2, out text3);
                    }
                }
                if (stream != null && stream.Length > 0L)
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

            public bool Done
            {
                get
                {
                    return true;
                }
            }
        }

        internal string PageStyle;

        private string m_fixedHeaderScript;

        public HTML5ViewerRenderer(ReportControlSession reportControlSession, CreateAndRegisterStream streamCallback, ViewerRendererDeviceInfo deviceInfo, NameValueCollection browserCaps, SecondaryStreams secondaryStreams, PageCountMode pageCountMode, IElementExtender elementExtender) 
            : base(new HTML5ViewerRenderer.DetachedReportWrapper(deviceInfo.RawDeviceInfo["StreamRoot"] ?? ""), new HTML5ViewerRenderer.SPBProcessingStub(reportControlSession, HttpUtility.HtmlEncode(deviceInfo.RawDeviceInfo["StreamRoot"] ?? ""), pageCountMode), new NameValueCollection(), deviceInfo, deviceInfo.RawDeviceInfo, browserCaps, streamCallback, secondaryStreams, elementExtender)
        {
        }

        public override void Render(TextWriter outputWriter)
        {
            base.InitializeReport();
            this.m_encoding = outputWriter.Encoding;
            this.m_mainStream = Utility.CreateBufferedStream(outputWriter);
            string styleStreamName = HTML5ViewerRenderer.GetStyleStreamName(this.m_pageNum);
            Stream sourceStream = this.CreateStyleStream(styleStreamName);
            this.m_styleStream = Utility.CreateBufferedStream(sourceStream);
            string str = this.m_deviceInfo.HtmlPrefixId + "oReportDiv";
            this.m_styleClassPrefix = this.m_encoding.GetBytes("#" + str + " ");
            base.RenderHtmlBody();
            base.RenderSecondaryStreamSpanTagsForJavascriptFunctions();
            this.m_mainStream.Flush();
            this.m_styleStream.Flush();
            this.m_fixedHeaderScript = this.GetFixedHeaderScripts();
            Stream mainStream = this.m_mainStream;
            this.m_mainStream = this.m_styleStream;
            base.PredefinedStyles();
            this.m_styleStream.Flush();
            this.m_mainStream = mainStream;
            /*this.m_mainStream.Flush();
            this.m_styleStream.Flush();
            this.m_fixedHeaderScript = this.GetFixedHeaderScripts();
            //Stream mainStream = this.m_mainStream;
            //this.m_mainStream = this.m_styleStream;
            base.PredefinedStyles();
            this.m_styleStream.Flush();
            
            var mainStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(mainStream);
            var bytes = this.m_encoding.GetBytes("<style type='text/css'>");
            mainStream.Write(bytes, 0, bytes.Length);

            this.m_styleStream.Position = 0;
            bytes = new byte[this.m_styleStream.Length];
            this.m_styleStream.Read(bytes, 0, bytes.Length);
            mainStream.Write(bytes, 0, bytes.Length);

            bytes = this.m_encoding.GetBytes("</style>");
            mainStream.Write(bytes, 0, bytes.Length);

            bytes = new byte[this.m_styleStream.Length];
            this.m_mainStream.Read(bytes, 0, bytes.Length);
            mainStream.Write(bytes, 0, bytes.Length);
            
            outputWriter = new StreamWriter(mainStream);
            //((StreamWriter)outputWriter).BaseStream = mainStream;
            this.m_mainStream = mainStream;
            this.m_mainStream.Flush();
         */
        }

        private Stream CreateStyleStream(string styleStreamName)
        {
            return base.CreateStream(styleStreamName, "css", Encoding.UTF8, "text/css", false, StreamOper.CreateAndRegister);
        }

        public static string GetStyleStreamName(int pageNumber)
        {
            return HTML5Renderer.GetStyleStreamName("STYLESTREAM", pageNumber);
        }

        protected override void RenderInteractionAction(RPLAction action, ref bool hasHref)
        {
            base.RenderControlActionScript(action);
            this.WriteStream(HTMLElements.m_href);
            this.WriteStream(HTMLElements.m_quote);
            base.OpenStyle();
            this.WriteStream(HTMLElements.m_cursorHand);
            this.WriteStream(HTMLElements.m_semiColon);
            hasHref = true;
        }

        protected internal override void RenderSortAction(RPLTextBoxProps textBoxProps, RPLFormat.SortOptions sortState)
        {
            this.WriteStream(HTMLElements.m_openStyle);
            this.WriteStream(HTMLElements.m_cursorHand);
            this.WriteStream(HTMLElements.m_semiColon);
            this.WriteStream(HTMLElements.m_quote);
            string text = textBoxProps.UniqueName;
            if (sortState == RPLFormat.SortOptions.Descending || sortState == RPLFormat.SortOptions.None)
            {
                text += "_A";
            }
            else
            {
                text += "_D";
            }
            base.RenderOnClickActionScript("Sort", text, null);
            this.WriteStream(HTMLElements.m_closeBracket);
        }

        protected override void RenderInternalImageSrc()
        {
            base.WriteAttrEncoded(this.m_deviceInfo.ResourceStreamRoot);
        }

        protected internal override void RenderToggleImage(RPLTextBoxProps textBoxProps)
        {
            RPLTextBoxPropsDef rpltextBoxPropsDef = (RPLTextBoxPropsDef)textBoxProps.Definition;
            bool isSimple = rpltextBoxPropsDef.IsSimple;
            bool toggleState = textBoxProps.ToggleState;
            if (!textBoxProps.IsToggleParent)
            {
                return;
            }
            this.WriteStream(HTMLElements.m_openA);
            this.WriteStream(HTMLElements.m_ariaLabeledBy);
            base.WriteReportItemId(textBoxProps.UniqueName + HTMLElements.m_ariaSuffix);
            this.WriteStream(HTMLElements.m_quote);
            this.WriteStream(HTMLElements.m_ariaExpanded);
            this.WriteStream(toggleState ? "true" : "false");
            this.WriteStream(HTMLElements.m_quote);
            this.WriteStream(HTMLElements.m_id);
            base.WriteReportItemId(textBoxProps.UniqueName + "_na");
            this.WriteStream(HTMLElements.m_quote);
            this.WriteStream(HTMLElements.m_tabIndex);
            base.WriteStream(++this.m_tabIndexNum);
            this.WriteStream(HTMLElements.m_quote);
            this.WriteStream(HTMLElements.m_openStyle);
            this.WriteStream(HTMLElements.m_paddingLeft);
            this.WriteStream("2px;");
            this.WriteStream(HTMLElements.m_paddingRight);
            this.WriteStream("2px;");
            this.WriteStream(HTMLElements.m_cursorHand);
            this.WriteStream(HTMLElements.m_semiColon);
            this.WriteStream(HTMLElements.m_quote);
            base.RenderOnClickActionScript("Toggle", textBoxProps.UniqueName, null);
            this.WriteStream(HTMLElements.m_closeBracket);
            this.WriteStream(HTMLElements.m_img);
            if (this.m_browserIE)
            {
                this.WriteStream(HTMLElements.m_imgOnError);
            }
            this.WriteStream(HTMLElements.m_zeroBorder);
            this.WriteStream(HTMLElements.m_src);
            this.WriteToggleImage(toggleState);
            this.WriteStream(HTMLElements.m_quote);
            string value = textBoxProps.Value;
            if (string.IsNullOrEmpty(value))
            {
                value = rpltextBoxPropsDef.Value;
            }
            this.WriteStream(HTMLElements.m_alt);
            if (isSimple)
            {
                if (toggleState)
                {
                    base.WriteStreamEncoded(string.Format("Collapse {0}",value));
                }
                else
                {
                    base.WriteStreamEncoded(string.Format("Expand {0}", value));
                }
            }
            else if (toggleState)
            {
                base.WriteStreamEncoded(RenderRes.ToggleStateCollapse);
            }
            else
            {
                base.WriteStreamEncoded(RenderRes.ToggleStateExpand);
            }
            this.WriteStream(HTMLElements.m_closeTag);
            this.WriteStream(HTMLElements.m_closeA);
        }

        private void WriteToggleImage(bool toggleState)
        {
            if (toggleState)
            {
                this.WriteStream(EmbeddedResourceOperation.CreateUrl("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.ToggleMinus.gif"));
                return;
            }
            this.WriteStream(EmbeddedResourceOperation.CreateUrl("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.TogglePlus.gif"));
        }

        protected override void RenderSortImageText(RPLFormat.SortOptions sortState)
        {
            if (sortState == RPLFormat.SortOptions.Ascending)
            {
                this.WriteStream(EmbeddedResourceOperation.CreateUrl("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.sortAsc.gif"));
                return;
            }
            if (sortState == RPLFormat.SortOptions.Descending)
            {
                this.WriteStream(EmbeddedResourceOperation.CreateUrl("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.sortDesc.gif"));
                return;
            }
            this.WriteStream(EmbeddedResourceOperation.CreateUrl("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.unsorted.gif"));
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

        protected internal override void WriteFitProportionalScript(double pv, double ph)
        {
            this.WriteStream(HTMLElements.m_onLoadFitProportionalPv);
            this.WriteStream(Utility.MmToPxAsString(pv));
            this.WriteStream(";this.ph=");
            this.WriteStream(Utility.MmToPxAsString(ph));
            this.WriteStream(";\"");
        }

        protected override void RenderPageStart(bool firstPage, bool lastPage, RPLElementStyle pageStyle)
        {
            this.WriteStream(HTMLElements.m_openDiv);
            this.WriteStream(HTMLElements.m_ltrDir);
            this.WriteStream(HTMLElements.m_openStyle);
            if (this.m_deviceInfo.IsBrowserIE)
            {
                this.WriteStream(HTMLElements.m_styleHeight);
                this.WriteStream(HTMLElements.m_percent);
                this.WriteStream(HTMLElements.m_semiColon);
            }
            this.WriteStream(HTMLElements.m_styleWidth);
            this.WriteStream(HTMLElements.m_percent);
            this.WriteStream(HTMLElements.m_semiColon);
            this.WriteStream("direction:ltr");
            this.WriteStream(HTMLElements.m_quote);
            this.RenderReportItemId("oReportDiv");
            if (this.m_pageHasStyle)
            {
                Stream mainStream = this.m_mainStream;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    this.m_mainStream = memoryStream;
                    base.RenderBackgroundStyleProps(pageStyle);
                    int num = 0;
                    base.RenderHtmlBorders(pageStyle, ref num, 0, true, true, null);
                    Encoding encoding = new UTF8Encoding(false);
                    this.PageStyle = encoding.GetString(memoryStream.ToArray());
                }
                this.m_mainStream = mainStream;
                mainStream = null;
            }
            else
            {
                this.PageStyle = null;
            }
            this.WriteStream(HTMLElements.m_closeBracket);
            this.WriteStream(HTMLElements.m_openTable);
            this.WriteStream(HTMLElements.m_closeBracket);
            this.WriteStream(HTMLElements.m_firstTD);
            this.RenderReportItemId("oReportCell");
            this.WriteStream(HTMLElements.m_closeBracket);
        }

        protected override void RenderPageEnd()
        {
            this.WriteStream(HTMLElements.m_lastTD);
            this.WriteStream(HTMLElements.m_closeTable);
            this.WriteStream(HTMLElements.m_closeDiv);
        }

        private string GetFixedHeaderScripts()
        {
            if (this.m_fixedHeaders == null || this.m_fixedHeaders.Count == 0 || !this.m_hasOnePage)
            {
                return null;
            }
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

        internal string FixedHeaderScript
        {
            get
            {
                return this.m_fixedHeaderScript;
            }
        }
    }
}
