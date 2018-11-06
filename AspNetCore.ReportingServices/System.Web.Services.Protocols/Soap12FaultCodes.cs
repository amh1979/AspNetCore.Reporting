/* ===============================================
* 功能描述：ReflectorReport.System.Web.Services.Protocols.Soap12FaultCodes
* 创 建 者：WeiGe
* 创建日期：8/9/2018 14:42:37
* ===============================================*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace System.Web.Services.Protocols
{
    /// <summary>Defines the SOAP fault codes that appear in a SOAP message when an error occurs while communicating with XML Web services using the SOAP version 1.2 protocol.</summary>
    internal sealed class Soap12FaultCodes
    {
        private Soap12FaultCodes()
        {
        }

        /// <summary>Represents the SOAP version 1.2 Receiver fault code indicating an error occurred during the processing of a client call on the server due to a problem with the recipient.</summary>
        public static readonly XmlQualifiedName ReceiverFaultCode = new XmlQualifiedName("Receiver", "http://www.w3.org/2003/05/soap-envelope");

        /// <summary>Represents the SOAP version 1.2 Sender fault code indicating a client call was not formatted correctly or did not contain the appropriate information.</summary>
        public static readonly XmlQualifiedName SenderFaultCode = new XmlQualifiedName("Sender", "http://www.w3.org/2003/05/soap-envelope");

        /// <summary>Represents the SOAP version 1.2 VersionMismatch fault code indicating the XML Web service expected SOAP messages conforming to the SOAP 1.2 specification, yet received one conforming to SOAP 1.1.</summary>
        public static readonly XmlQualifiedName VersionMismatchFaultCode = new XmlQualifiedName("VersionMismatch", "http://www.w3.org/2003/05/soap-envelope");

        /// <summary>Represents the SOAP version 1.2 MustUnderstand fault code indicating a SOAP header marked with the MustUnderstand attribute was not processed.</summary>
        public static readonly XmlQualifiedName MustUnderstandFaultCode = new XmlQualifiedName("MustUnderstand", "http://www.w3.org/2003/05/soap-envelope");

        /// <summary>Represents the SOAP version 1.2 DataEncodingUnknown fault code indicating the SOAP message is encoded in an unrecognized format.</summary>
        public static readonly XmlQualifiedName DataEncodingUnknownFaultCode = new XmlQualifiedName("DataEncodingUnknown", "http://www.w3.org/2003/05/soap-envelope");

        /// <summary>Represents the SOAP version 1.2 rpc:ProcedureNotPresent fault subcode indicating the XML Web service does not support the requested XML Web service method.</summary>
        public static readonly XmlQualifiedName RpcProcedureNotPresentFaultCode = new XmlQualifiedName("ProcedureNotPresent", "http://www.w3.org/2003/05/soap-rpc");

        /// <summary>Represents the SOAP version 1.2 rpc:BadArguments fault subcode indicating that arguments sent to the XML Web service method were incorrect or could not be parsed.</summary>
        public static readonly XmlQualifiedName RpcBadArgumentsFaultCode = new XmlQualifiedName("BadArguments", "http://www.w3.org/2003/05/soap-rpc");

        /// <summary>Represents the fault code for missing ID in SOAP encoding data.</summary>
        public static readonly XmlQualifiedName EncodingMissingIdFaultCode = new XmlQualifiedName("MissingID", "http://www.w3.org/2003/05/soap-encoding");

        /// <summary>Represents the SOAP version 1.2 enc:UntypedValue fault subcode indicating that the SOAP message was RPC-encoded and the type name property of an encoded graph node was not specified.</summary>
        public static readonly XmlQualifiedName EncodingUntypedValueFaultCode = new XmlQualifiedName("UntypedValue", "http://www.w3.org/2003/05/soap-encoding");

        internal static readonly XmlQualifiedName UnsupportedMediaTypeFaultCode = new XmlQualifiedName("UnsupportedMediaType", "http://microsoft.com/soap/");

        internal static readonly XmlQualifiedName MethodNotAllowed = new XmlQualifiedName("MethodNotAllowed", "http://microsoft.com/soap/");
    }
}
