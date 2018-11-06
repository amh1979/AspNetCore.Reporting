/* ===============================================
* 功能描述：AspNetCore.ReportingServices.Rendering.HtmlRenderer.HTMLRenderer
* 创 建 者：WeiGe
* 创建日期：8/16/2018 01:26:51
* ===============================================*/

using AspNetCore.Reporting;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
    internal sealed class HTMLRenderer
    {
        public static NameValueCollection CreateDeviceInfo( int pageNumber, string findString, string replacementRoot, string linkTarget, string browserMode, bool useImageConsolidation, bool enablePowerBIFeatures)
        {
            NameValueCollection nameValueCollection = new NameValueCollection();
            nameValueCollection.Set("HTMLFragment", "true");
            nameValueCollection.Set("Section", pageNumber.ToString(CultureInfo.InvariantCulture));
            //string value = ReportImageOperation.CreateUrl(report, this.m_viewerInstanceIdentifier, false);
            //nameValueCollection.Set("StreamRoot", value);
            //string value2 = ReportImageOperation.CreateUrl(report, this.m_viewerInstanceIdentifier, true);
            //nameValueCollection.Set("ResourceStreamRoot", value2);
            nameValueCollection.Set("EnablePowerBIFeatures", enablePowerBIFeatures.ToString());
            //nameValueCollection.Set("ActionScript", this.ActionScriptMethod);
            if (!string.IsNullOrEmpty(findString))
            {
                nameValueCollection.Set("FindString", findString);
            }
            if (!string.IsNullOrEmpty(replacementRoot))
            {
                nameValueCollection.Set("ReplacementRoot", replacementRoot);
            }
            nameValueCollection.Set("PrefixId", "rs");
            nameValueCollection.Set("StyleStream", "true");
            if (!string.IsNullOrEmpty(linkTarget))
            {
                nameValueCollection.Set("LinkTarget", linkTarget);
            }

            if (!string.IsNullOrEmpty(browserMode) && nameValueCollection["BrowserMode"] == null)
            {
                nameValueCollection.Set("BrowserMode", browserMode);
            }
            if (!useImageConsolidation)
            {
                nameValueCollection.Set("ImageConsolidation", "false");
            }
            return nameValueCollection;
        }
       
    }
}
