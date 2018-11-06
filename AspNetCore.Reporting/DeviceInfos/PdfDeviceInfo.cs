using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Reporting.DeviceInfos
{
    /// <summary>
    /// See https://msdn.microsoft.com/en-us/library/ms154682(v=sql.105).aspx
    /// </summary>
    internal class PdfDeviceInfo: DeviceInfoBase
    {
        public PdfDeviceInfo()
        {
            this.Toolbar = null;
        }
        public int? StartPage { get; set; }
        public int? EndPage { get; set; }
        public int? Columns { get; set; }
        public int? ColumnSpacing { get; set; }
        public int? DpiX { get; set; }
        public int? DpiY { get; set; }
        public bool? HumanReadablePDF { get; set; }
        public int? MarginBottom { get; set; }
        public int? MarginLeft { get; set; }
        public int? MarginRight { get; set; }
        public int? MarginTop { get; set; }
        public int? PageHeight { get; set; }
        public int? PageWidth { get; set; }
    }
}
