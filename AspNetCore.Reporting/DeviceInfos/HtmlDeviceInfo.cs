using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Reporting.DeviceInfos
{
    //[System.Runtime.Serialization.DataContract]
    //[System.Xml.Serialization.XmlInclude(typeof(DeviceInfo))]
    //[System.Xml.Serialization.XmlRoot("DeviceInfo")]
    internal class HtmlDeviceInfo: DeviceInfoBase
    {
        public static HtmlDeviceInfo Default = new HtmlDeviceInfo
        {
            Toolbar = true,
            OnlyVisibleStyles = true,            
            Zoom = 100,
            IsBrowserIE = true,
            ImageConsolidation = true
        };

        public bool? ToggleItems { get; set; }
        public bool? MeasureItems { get; set; }
        public string UserAgent { get; set; }
        public string ActionScript { get; set;}
        public bool? JavaScript { get; set; }
        public string BookmarkId { get; set; }
        public bool? ExpandContent { get; set; }
        public bool? HasActionScript { get; set; }
        public bool HTMLFragment { get; set; } = true;
        public bool? OnlyVisibleStyles { get; set; } = true;
        public bool? EnablePowerBIFeatures { get; set; }
        public string FindString { get; set; }
        //public string HtmlPrefixId { get; set; } = "rs";
        public string PrefixId { get; set; } = "rs";
        //public string JavascriptPrefixId { get; set; }
        public string LinkTarget { get; set; }
        public string ReplacementRoot { get; set; }
        public string ResourceStreamRoot { get; set; }
        public string StreamRoot { get; set; }
        /// <summary>
        /// page index
        /// </summary>
        public int? Section { get; set; }
        //public string StylePrefixId { get; set; } = "a";
        public bool? StyleStream { get; set; }
        public bool? OutlookCompat { get; set; }
        public int? Zoom { get; set; }
        public bool? AccessibleTablix { get; set; }
        public DataVisualizationFitSizing? DataVisualizationFitSizing { get; set; }
        public bool? IsBrowserIE { get; set; }
        public bool? IsBrowserSafari { get; set; }
        public bool? IsBrowserGeckoEngine { get; set; }
        public bool? IsBrowserIE6Or7StandardsMode { get; set; }
        public bool? IsBrowserIE6 { get; set; }
        public bool? IsBrowserIE7 { get; set;}
        public BrowserMode? BrowserMode { get; set; }
        public string NavigationId { get; set; }
        public bool? ImageConsolidation { get; set; }
    }
}
