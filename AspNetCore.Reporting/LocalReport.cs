using AspNetCore.ReportingServices.Rendering.SPBProcessing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace AspNetCore.Reporting
{
    /// <summary>
    /// execute local report. such as: rdl,rdlc
    /// </summary>
    public class LocalReport
    {
        static LocalReport()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        private InternalLocalReport localReport = new InternalLocalReport();
        /// <summary>
        /// absolute local report path.
        /// such as: rdl,rdlc
        /// </summary>
        /// <param name="reportPath"></param>
        public LocalReport(string reportPath)
        {         
            Check.NotEmpty(reportPath, nameof(reportPath));
            if (File.Exists(reportPath))
            {
                localReport.ReportPath = reportPath;
            }
            else
            {
                throw new FileNotFoundException("The report file not found.", reportPath);
            }
        }

        /// <summary>
        /// add report data source datatable,reader,dataset, model
        /// </summary>
        /// <param name="dataSetName"></param>
        /// <param name="dataSource"></param>
        public void AddDataSource(string dataSetName, object dataSource)
        {
            this.localReport.DataSources.Add(new ReportDataSource(dataSetName, dataSource));
        }
        /// <summary>
        /// Execute report result
        /// </summary>
        /// <param name="renderType">renderType</param>
        /// <param name="parameters"> request parameters</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="findString">find string</param>
        public ReportResult Execute(RenderType renderType, int pageIndex = 1, Dictionary<string, string> parameters = null, string findString = "")
        {
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            if (parameters != null)
            {
                foreach (var kv in parameters)
                {
                    this.localReport.SetParameters(new ReportParameter(kv.Key, kv.Value));
                }
            }
            var xml = JsonConvert.DeserializeXNode(JsonConvert.SerializeObject(new { Section = pageIndex, FindString = findString }), "DeviceInfo");
            var deviceInfo = xml.ToString(System.Xml.Linq.SaveOptions.DisableFormatting);
            this.localReport.PageIndex = pageIndex;
            if (renderType == RenderType.Rpl)
            {
                string _deviceInfo = string.Format(CultureInfo.InvariantCulture, "<DeviceInfo><StartPage>{0}</StartPage><EndPage>{1}</EndPage><ToggleItems>{2}</ToggleItems><MeasureItems>{3}</MeasureItems><SecondaryStreams>{4}</SecondaryStreams><StreamNames>{5}</StreamNames><StreamRoot>{6}</StreamRoot><RPLVersion>{7}</RPLVersion><ImageConsolidation>{8}</ImageConsolidation></DeviceInfo>", new object[]
                 {pageIndex,pageIndex,false,false,"Server",true,true,"","10.6",true});
                var stream = this.RenderStream(_deviceInfo,out Encoding encoding, out string mimeType,out string fileNameExtensions);
                var bytes = new byte[0];
                if (stream != null && stream.Length > 0L)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                }
                return new ReportResult
                {
                    MainStream = new ReportStream(bytes)
                    {
                        Encoding = encoding,
                        Extension = fileNameExtensions,
                        MimeType = mimeType
                    },
                    TotalPages = localReport.GetTotalPages(),
                    PageIndex = pageIndex,
                };
            }
            else
            {
                var bytes = this.localReport.Render(renderType.ToString(), deviceInfo, PageCountMode.Actual, out string mimeType
                      , out Encoding encoding, out string fileNameExtension, out string[] streams, out Warning[] warnings);
                Encoding _encoding = encoding ?? Encoding.UTF8;
 
                return new ReportResult
                {
                    MainStream =new ReportStream(bytes) { 
                        Extension=fileNameExtension,
                        Encoding=_encoding,
                        MimeType=mimeType,
                    },
                    SecondaryStream =new ReportStream(this.localReport.Styles) {
                        Encoding=Encoding.UTF8,
                        Extension="css",
                        MimeType="text/css"
                    },
                    TotalPages = localReport.GetTotalPages(),
                    PageIndex = pageIndex,
                };
            }
        }
        private Stream RenderStream(string deviceInfo,out Encoding encoding, out string mimeType, out string fileNameExtension)
        {
            using (AspNetCore.Reporting.StreamCache streamCache = new AspNetCore.Reporting.StreamCache())
            { 
                Warning[] array = default(Warning[]);
                this.localReport.InternalRender("RPL", true, deviceInfo, PageCountMode.Actual,
                    (AspNetCore.ReportingServices.Interfaces.CreateAndRegisterStream)streamCache.StreamCallback, out array);
                return streamCache.GetMainStream(true, out encoding, out mimeType, out fileNameExtension);
            }
        }
    }
    /// <summary>
    /// report result
    /// </summary>
    public class ReportResult
    {
        internal ReportResult()
        {

        }
        /// <summary>
        /// stream encoding
        /// </summary>
       
        /// <summary>
        /// current page index
        /// </summary>
        public int PageIndex { get; internal set; }
        /// <summary>
        /// report body
        /// </summary>
        public ReportStream MainStream { get; internal set; }
        /// <summary>
        /// such as css style
        /// </summary>
        public ReportStream SecondaryStream { get; internal set; }
        /// <summary>
        /// the report total pages
        /// </summary>
        public int TotalPages { get; internal set; }
    }
    public class ReportStream
    {
        internal ReportStream(byte[] bytes)
        {
            if (bytes == null)
            {
                bytes = new byte[0];
            }
            Stream = bytes;
        }
        public byte[] Stream { get; }
        public string MimeType { get; internal set; }
        public string Extension { get; internal set; }
        public Encoding Encoding { get; internal set; }
        public string GetString()
        {
            return this.Encoding.GetString(this.Stream);
        }
    }
}
