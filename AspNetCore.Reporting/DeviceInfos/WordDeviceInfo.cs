using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Reporting.DeviceInfos
{
    internal class WordDeviceInfo : DeviceInfoBase
    {
        public AutoFit? AutoFit { get; set; }
        public bool? ExpandToggles { get; set; }
        public bool? FixedPageWidth { get; set; }
        public bool? OmitHyperlinks { get; set; }
        public bool? OmitDrillthroughs { get; set; }
    }
}
