/* ===============================================
* 功能描述：ReflectorReport.AspNetCore.ReportingServices.Common.SoapProtocolVersion
* 创 建 者：WeiGe
* 创建日期：8/9/2018 14:39:55
* ===============================================*/

using System;
using System.Collections.Generic;
using System.Text;


namespace System.Web.Services.Protocols
{
    /// <summary>Specifies the version of the SOAP protocol used to communicate with an XML Web service.</summary>
    internal enum SoapProtocolVersion
    {
        /// <summary>The default value for this enumeration.</summary>
        Default,
        /// <summary>SOAP protocol version 1.1.</summary>
        Soap11,
        /// <summary>SOAP protocol version 1.2.</summary>
        Soap12
    }
}
