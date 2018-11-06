using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;

namespace AspNetCore.Reporting.DeviceInfos
{
    //[System.Runtime.Serialization.DataContract]
    internal class DeviceInfoBase
    {
        public bool? Toolbar { get; set; }

        public new string ToString()
        {
            return this.ToString(false);
        }

        public string ToString(bool format)
        {            
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(this, new Newtonsoft.Json.JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatString = "MM/dd/yyyy HH:mm"                
            });

            var doc = Newtonsoft.Json.JsonConvert.DeserializeXNode(str, "DeviceInfo");

            var options = System.Xml.Linq.SaveOptions.DisableFormatting;
            if (format)
            {
                options = System.Xml.Linq.SaveOptions.None;
            }
            return doc.ToString(options);
        }
    }   
}
