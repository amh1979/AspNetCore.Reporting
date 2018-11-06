/* ===============================================
* 功能描述：ReflectorReport.AspNetCore.ReportingServices.Common.SoapFaultSubCode
* 创 建 者：WeiGe
* 创建日期：8/9/2018 14:38:19
* ===============================================*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace System.Web.Services.Protocols
{
    /// <summary>Represents the contents of the optional Subcode element of a SOAP fault when SOAP version 1.2 is used to communicate between a client and an XML Web service.</summary>
	[Serializable]
    internal class SoapFaultSubCode
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Services.Protocols.SoapFaultSubcode" /> class sets the application specific error code.</summary>
        /// <param name="code">An <see cref="T:System.Xml.XmlQualifiedName" /> specifying the application specific error code. Sets the <see cref="P:System.Web.Services.Protocols.SoapFaultSubcode.Code" /> property. </param>
        public SoapFaultSubCode(XmlQualifiedName code) : this(code, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Services.Protocols.SoapFaultSubcode" /> class setting the application specific error code and additional error information.</summary>
        /// <param name="code">An <see cref="T:System.Xml.XmlQualifiedName" /> specifying the application specific error code. Sets the <see cref="P:System.Web.Services.Protocols.SoapFaultSubcode.Code" /> property. </param>
        /// <param name="subCode"></param>
        public SoapFaultSubCode(XmlQualifiedName code, SoapFaultSubCode subCode)
        {
            this.code = code;
            this.subCode = subCode;
        }

        /// <summary>Gets the application specific error code in the form of an XML qualified name.</summary>
        /// <returns>An <see cref="T:System.Xml.XmlQualifiedName" /> representing the application specific error code.</returns>
        public XmlQualifiedName Code
        {
            get
            {
                return this.code;
            }
        }

        /// <summary>Gets additional error information contained within a child Subcode element.</summary>
        /// <returns>A <see cref="T:System.Web.Services.Protocols.SoapFaultSubcode" /> containing additional error information.</returns>
        public SoapFaultSubCode SubCode
        {
            get
            {
                return this.subCode;
            }
        }

        private XmlQualifiedName code;

        private SoapFaultSubCode subCode;
    }
}
