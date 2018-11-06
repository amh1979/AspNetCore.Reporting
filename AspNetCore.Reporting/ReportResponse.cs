using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Reporting
{
    /// <summary>
    /// the report executed result
    /// </summary>
    public class ReportResponse
    {
        /// <summary>
        /// defalut return success.
        /// </summary>
        public ReportResponse()
        {
        }
        /// <summary>
        /// return error.
        /// </summary>
        /// <param name="errorMessage"></param>
        public ReportResponse(string errorMessage)
        {
            Status = -2;
            Message = errorMessage;
        }
        /// <summary>
        /// set excuted of error
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ReportResponse SetError(string message)
        {
            Status = -1;
            Message = message;
            return this;
        }
        /// <summary>
        /// 0 success
        /// </summary>
        public int Status { get; internal set; } = 0;
        /// <summary>
        /// excute message
        /// </summary>
        public string Message { get; internal set; } = "OK";
        /// <summary>
        /// the report data
        /// </summary>
        public ReportData Data { get; internal set; } = new ReportData();
    }
    /// <summary>
    /// the report data
    /// </summary>
    public class ReportData
    {
        /// <summary>
        /// string contents
        /// e.g. html
        /// </summary>
        public string Contents { get; internal set; }
        /// <summary>
        /// the report session id
        /// </summary>
        public string SessionId { get; internal set; }
        /// <summary>
        /// which page of report to excuting
        /// </summary>
        public int PageIndex { get; internal set; }
        /// <summary>
        /// the total page count of report
        /// </summary>
        public int PageCount { get; internal set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public Version Version { get; internal set; }
        /// <summary>
        /// used for export the fileName
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// the content stream, for download file
        /// </summary>
        public byte[] Stream { get; internal set; }
        /// <summary>
        /// the file mime type
        /// </summary>
        public string MimeType { get;internal set; }
        /// <summary>
        /// the file extension
        /// </summary>
        public string Extension { get;internal set; }       
    }
}
