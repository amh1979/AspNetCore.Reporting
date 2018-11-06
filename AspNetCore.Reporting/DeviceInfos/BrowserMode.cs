using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Reporting.DeviceInfos
{
    [JsonConverter(typeof(StringEnumConverter))]
    internal enum BrowserMode  
    {
        Unknown,
        Quirks,
        Standards
    }
}
