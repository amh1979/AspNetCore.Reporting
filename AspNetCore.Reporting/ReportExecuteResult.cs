using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Reporting
{
    public class ReportExecuteResult
    {
        internal ReportExecuteResult()
        {

        }
        public string Contents { get; internal set; }
        public byte[] Stream { get; internal set; }
        public string MimeType { get; internal set; }
        public string Encoding { get; internal set; }
        public string Extension { get; internal set; }
        public int PageCount { get; internal set; }
        public int PageIndex { get; internal set; }
        public string SessionId { get; internal set; }
        public bool ParametersRequired { get; internal set; }
        //public Version Version { get; set; }
    }
}
