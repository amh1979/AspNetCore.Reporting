/* ===============================================
* 功能描述：ReflectorReport.AspNetCore.ReportingServices.Common.SoapException
* 创 建 者：WeiGe
* 创建日期：8/9/2018 14:36:19
* ===============================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Xml;

namespace System.Web.Services.Protocols
{
    /// <summary>Represents the exception that is thrown when an XML Web service method is called over SOAP and an exception occurs.</summary>
    [Serializable]
    internal class SoapException : SystemException
    {
        /// <summary>Returns a value that indicates whether the SOAP fault code is equivalent to the Server SOAP fault code regardless of the version of the SOAP protocol used.</summary>
        /// <returns>true if <paramref name="code" /> is equivalent to the Server SOAP fault code; otherwise, false.</returns>
        /// <param name="code">An <see cref="T:System.Xml.XmlQualifiedName" /> that contains a SOAP fault code. </param>
        public static bool IsServerFaultCode(XmlQualifiedName code)
        {
            return code == SoapException.ServerFaultCode || code == Soap12FaultCodes.ReceiverFaultCode;
        }

        /// <summary>Returns a value that indicates whether the SOAP fault code is equivalent to the Client SOAP fault code regardless of the version of the SOAP protocol used.</summary>
        /// <returns>true if <paramref name="code" /> is equivalent to the Client SOAP fault code; otherwise, false.</returns>
        /// <param name="code">An <see cref="T:System.Xml.XmlQualifiedName" /> that contains a SOAP fault code. </param>
        public static bool IsClientFaultCode(XmlQualifiedName code)
        {
            return code == SoapException.ClientFaultCode || code == Soap12FaultCodes.SenderFaultCode;
        }

        /// <summary>Returns a value that indicates whether the SOAP fault code is equivalent to the VersionMismatch SOAP fault code regardless of the version of the SOAP protocol used.</summary>
        /// <returns>true if <paramref name="code" /> is equivalent to the VersionMismatch SOAP fault code; otherwise, false.</returns>
        /// <param name="code">An <see cref="T:System.Xml.XmlQualifiedName" /> that contains a SOAP fault code. </param>
        public static bool IsVersionMismatchFaultCode(XmlQualifiedName code)
        {
            return code == SoapException.VersionMismatchFaultCode || code == Soap12FaultCodes.VersionMismatchFaultCode;
        }

        /// <summary>Returns a value that indicates whether the SOAP fault code is equivalent to MustUnderstand regardless of the version of the SOAP protocol used.</summary>
        /// <returns>true if <paramref name="code" /> is equivalent to the MustUnderstand SOAP fault code; otherwise, false.</returns>
        /// <param name="code">An <see cref="T:System.Xml.XmlQualifiedName" /> that contains a SOAP fault code. </param>
        public static bool IsMustUnderstandFaultCode(XmlQualifiedName code)
        {
            return code == SoapException.MustUnderstandFaultCode || code == Soap12FaultCodes.MustUnderstandFaultCode;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Services.Protocols.SoapException" /> class.</summary>
        public SoapException() : base(null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Services.Protocols.SoapException" /> class with the specified exception message, exception code, and URI that identifies the piece of code that caused the exception.</summary>
        /// <param name="message">A message that identifies the reason the exception occurred. This parameter sets the <see cref="P:System.Exception.Message" /> property. </param>
        /// <param name="code">An <see cref="T:System.Xml.XmlQualifiedName" /> that specifies the type of error that occurred. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Code" /> property. </param>
        /// <param name="actor">A URI that identifies the code that caused the exception. Typically, this is a URL to an XML Web service method. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Actor" /> property. </param>
        public SoapException(string message, XmlQualifiedName code, string actor) : base(message)
        {
            this.code = code;
            this.actor = actor;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Services.Protocols.SoapException" /> class with the specified exception message, exception code, URI that identifies the code that caused the exception, and reference to the root cause of the exception.</summary>
        /// <param name="message">A message that identifies the reason the exception occurred. This parameter sets the <see cref="P:System.Exception.Message" /> property. </param>
        /// <param name="code">An <see cref="T:System.Xml.XmlQualifiedName" /> that specifies the type of error that occurred. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Code" /> property. </param>
        /// <param name="actor">A URI that identifies the piece of code that caused the exception. Typically, this is a URL to an XML Web service method. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Actor" /> property. </param>
        /// <param name="innerException">An exception that is the root cause of the exception. This parameter sets the <see cref="P:System.Exception.InnerException" /> property. </param>
        public SoapException(string message, XmlQualifiedName code, string actor, Exception innerException) : base(message, innerException)
        {
            this.code = code;
            this.actor = actor;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Services.Protocols.SoapException" /> class with the specified exception message and exception code.</summary>
        /// <param name="message">A message that identifies the reason the exception occurred. This parameter sets the <see cref="P:System.Exception.Message" /> property. </param>
        /// <param name="code">An <see cref="T:System.Xml.XmlQualifiedName" /> that specifies the type of error that occurred. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Code" /> property. </param>
        public SoapException(string message, XmlQualifiedName code) : base(message)
        {
            this.code = code;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Services.Protocols.SoapException" /> class with the specified exception message, exception code, and reference to the root cause of the exception.</summary>
        /// <param name="message">A message that identifies the reason the exception occurred. This parameter sets the <see cref="P:System.Exception.Message" /> property. </param>
        /// <param name="code">An <see cref="T:System.Xml.XmlQualifiedName" /> that specifies the type of error that occurred. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Code" /> property. </param>
        /// <param name="innerException">An exception that is the root cause of the exception. This parameter sets the <see cref="P:System.Exception.InnerException" /> property. </param>
        public SoapException(string message, XmlQualifiedName code, Exception innerException) : base(message, innerException)
        {
            this.code = code;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Services.Protocols.SoapException" /> class with the specified exception message, exception code, URI that identifies the piece of code that caused the exception, and application specific exception information.</summary>
        /// <param name="message">A message that identifies the reason the exception occurred. This parameter sets the <see cref="P:System.Exception.Message" /> property. </param>
        /// <param name="code">An <see cref="T:System.Xml.XmlQualifiedName" /> that specifies the type of error that occurred. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Code" /> property. </param>
        /// <param name="actor">A URI that identifies the piece of code that caused the exception. Typically, this is a URL to an XML Web service method. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Actor" /> property. </param>
        /// <param name="detail">An <see cref="T:System.Xml.XmlNode" /> that contains application specific exception information. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Detail" /> property. </param>
        public SoapException(string message, XmlQualifiedName code, string actor, XmlNode detail) : base(message)
        {
            this.code = code;
            this.actor = actor;
            this.detail = detail;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Services.Protocols.SoapException" /> class with the specified exception message, exception code, URI that identifies the piece of code that caused the exception, application-specific exception information, and reference to the root cause of the exception.</summary>
        /// <param name="message">A message that identifies the reason the exception occurred. This parameter sets the <see cref="P:System.Exception.Message" /> property. </param>
        /// <param name="code">An <see cref="T:System.Xml.XmlQualifiedName" /> that specifies the type of error that occurred. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Code" /> property. </param>
        /// <param name="actor">A URI that identifies the piece of code that caused the exception. Typically, this is a URL to an XML Web service method. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Actor" /> property. </param>
        /// <param name="detail">An <see cref="T:System.Xml.XmlNode" /> that contains application specific exception information. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Detail" /> property. </param>
        /// <param name="innerException">An exception that is the root cause of the exception. This parameter sets the <see cref="P:System.Exception.InnerException" /> property. </param>
        public SoapException(string message, XmlQualifiedName code, string actor, XmlNode detail, Exception innerException) : base(message, innerException)
        {
            this.code = code;
            this.actor = actor;
            this.detail = detail;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Services.Protocols.SoapException" /> class with the specified exception message, exception code, and subcode.</summary>
        /// <param name="message">A message that identifies the reason the exception occurred. This parameter sets the <see cref="P:System.Exception.Message" /> property.</param>
        /// <param name="code">An <see cref="T:System.Xml.XmlQualifiedName" /> that specifies the type of error that occurred. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Code" /> property.</param>
        /// <param name="subCode">An optional subcode for the SOAP fault. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.SubCode" /> property.</param>
        public SoapException(string message, XmlQualifiedName code, SoapFaultSubCode subCode) : base(message)
        {
            this.code = code;
            this.subCode = subCode;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Services.Protocols.SoapException" /> class with the specified exception message, exception code, URI that identifies the piece of code that caused the exception, application-specific exception information, and reference to the root cause of the exception.</summary>
        /// <param name="message">A message that identifies the reason the exception occurred. This parameter sets the <see cref="P:System.Exception.Message" /> property.</param>
        /// <param name="code">An <see cref="T:System.Xml.XmlQualifiedName" /> that specifies the type of error that occurred. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Code" /> property.</param>
        /// <param name="actor">A URI that identifies the piece of code that caused the exception. Typically, this is a URL to an XML Web service method. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Actor" /> property.</param>
        /// <param name="role">A URI that represents the XML Web service's function in processing the SOAP message. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Role" /> property.</param>
        /// <param name="detail">An <see cref="T:System.Xml.XmlNode" /> that contains application specific exception information. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Detail" /> property.</param>
        /// <param name="subCode">An optional subcode for the SOAP fault. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.SubCode" /> property.</param>
        /// <param name="innerException">An exception that is the root cause of the exception. This parameter sets the <see cref="P:System.Exception.InnerException" /> property.</param>
        public SoapException(string message, XmlQualifiedName code, string actor, string role, XmlNode detail, SoapFaultSubCode subCode, Exception innerException) : base(message, innerException)
        {
            this.code = code;
            this.actor = actor;
            this.role = role;
            this.detail = detail;
            this.subCode = subCode;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Services.Protocols.SoapException" /> class with the specified exception message, exception code, URI that identifies the piece of code that caused the exception, URI that represents the XML Web service's function in processing the SOAP message, the human language associated with the exception, the application-specific exception information, the subcode for the SOAP fault and reference to the root cause of the exception.</summary>
        /// <param name="message">A message that identifies the reason the exception occurred. This parameter sets the <see cref="P:System.Exception.Message" /> property.</param>
        /// <param name="code">An <see cref="T:System.Xml.XmlQualifiedName" /> that specifies the type of error that occurred. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Code" /> property.</param>
        /// <param name="actor">A URI that identifies the piece of code that caused the exception. Typically, this is a URL to an XML Web service method. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Actor" /> property.</param>
        /// <param name="role">A URI that represents the XML Web service's function in processing the SOAP message. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Role" /> property.</param>
        /// <param name="lang">A human language associated with the exception. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Lang" /> property.</param>
        /// <param name="detail">An <see cref="T:System.Xml.XmlNode" /> that contains application specific exception information. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.Detail" /> property.</param>
        /// <param name="subCode">An optional subcode for the SOAP fault. This parameter sets the <see cref="P:System.Web.Services.Protocols.SoapException.SubCode" /> property.</param>
        /// <param name="innerException">An exception that is the root cause of the exception. This parameter sets the <see cref="P:System.Exception.InnerException" /> property.</param>
        public SoapException(string message, XmlQualifiedName code, string actor, string role, string lang, XmlNode detail, SoapFaultSubCode subCode, Exception innerException) : base(message, innerException)
        {
            this.code = code;
            this.actor = actor;
            this.role = role;
            this.detail = detail;
            this.lang = lang;
            this.subCode = subCode;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Services.Protocols.SoapException" /> class with serialized data.</summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The T:System.Runtime.Serialization.StreamingContext  that contains contextual information about the source or destination.</param>
        protected SoapException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            IDictionary data = base.Data;
            this.code = (XmlQualifiedName)data["code"];
            this.actor = (string)data["actor"];
            this.role = (string)data["role"];
            this.subCode = (SoapFaultSubCode)data["subCode"];
            this.lang = (string)data["lang"];
        }

        /// <summary>Gets the piece of code that caused the exception.</summary>
        /// <returns>The piece of code that caused the exception.</returns>
        public string Actor
        {
            get
            {
                if (this.actor != null)
                {
                    return this.actor;
                }
                return string.Empty;
            }
        }

        /// <summary>Gets the type of SOAP fault code.</summary>
        /// <returns>An <see cref="T:System.Xml.XmlQualifiedName" /> that specifies the SOAP fault code that occurred.</returns>
        public XmlQualifiedName Code
        {
            get
            {
                return this.code;
            }
        }

        /// <summary>Gets an <see cref="T:System.Xml.XmlNode" /> that represents the application-specific error information details.</summary>
        /// <returns>The application-specific error information.</returns>
        public XmlNode Detail
        {
            get
            {
                return this.detail;
            }
        }

        /// <summary>Gets the human language associated with the exception.</summary>
        /// <returns>A <see cref="T:System.String" /> value that identifies the human language associated with the exception.</returns>
        [ComVisible(false)]
        public string Lang
        {
            get
            {
                if (this.lang != null)
                {
                    return this.lang;
                }
                return string.Empty;
            }
        }

        /// <summary>Gets a URI that represents the piece of code that caused the exception.</summary>
        /// <returns>The piece of code that caused the exception.</returns>
        [ComVisible(false)]
        public string Node
        {
            get
            {
                if (this.actor != null)
                {
                    return this.actor;
                }
                return string.Empty;
            }
        }

        /// <summary>Gets a URI that represents the XML Web service's function in processing the SOAP message.</summary>
        /// <returns>The role of the XML Web service throwing the <see cref="T:System.Web.Services.Protocols.SoapException" />. The default is <see cref="F:System.String.Empty" />.</returns>
        [ComVisible(false)]
        public string Role
        {
            get
            {
                if (this.role != null)
                {
                    return this.role;
                }
                return string.Empty;
            }
        }

        /// <summary>Gets the optional error information contained in the subcode XML element of a SOAP fault.</summary>
        /// <returns>A <see cref="T:System.Web.Services.Protocols.SoapFaultSubcode" /> that represents the contents of the subcode XML element of a SOAP fault.</returns>
        [ComVisible(false)]
        public SoapFaultSubCode SubCode
        {
            get
            {
                return this.subCode;
            }
        }

        internal void ClearSubCode()
        {
            if (this.subCode != null)
            {
                this.subCode = this.subCode.SubCode;
            }
        }

        /// <summary>Sets the <see cref="T:System.Runtime.Serialization." /><see cref="SerializationInfo" /> with information about the exception.</summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            IDictionary data = this.Data;
            data["code"] = this.Code;
            data["actor"] = this.Actor;
            data["role"] = this.Role;
            data["subCode"] = this.SubCode;
            data["lang"] = this.Lang;
            base.GetObjectData(info, context);
        }

        private static SoapException CreateSuppressedException(SoapProtocolVersion soapVersion, string message, Exception innerException)
        {
            return new SoapException("Res.GetString('WebSuppressedExceptionMessage')", (soapVersion == SoapProtocolVersion.Soap12) ? new XmlQualifiedName("Receiver", "http://www.w3.org/2003/05/soap-envelope") : new XmlQualifiedName("Server", "http://schemas.xmlsoap.org/soap/envelope/"));
        }

        internal static SoapException Create(SoapProtocolVersion soapVersion, string message, XmlQualifiedName code, string actor, string role, XmlNode detail, SoapFaultSubCode subCode, Exception innerException)
        {
            //if (WebServicesSection.Current.Diagnostics.SuppressReturningExceptions)
            //{
            //    return SoapException.CreateSuppressedException(soapVersion, Res.GetString("WebSuppressedExceptionMessage"), innerException);
            //}
            return new SoapException(message, code, actor, role, detail, subCode, innerException);
        }

        internal static SoapException Create(SoapProtocolVersion soapVersion, string message, XmlQualifiedName code, Exception innerException)
        {
            //if (WebServicesSection.Current.Diagnostics.SuppressReturningExceptions)
            //{
            //    return SoapException.CreateSuppressedException(soapVersion, Res.GetString("WebSuppressedExceptionMessage"), innerException);
            //}
            return new SoapException(message, code, innerException);
        }

        private XmlQualifiedName code = XmlQualifiedName.Empty;

        private string actor;

        private string role;

        private XmlNode detail;

        private SoapFaultSubCode subCode;

        private string lang;

        /// <summary>Specifies that a SOAP fault code that represents an error occurred during the processing of a client call on the server, where the problem is not due to the message contents.</summary>
        public static readonly XmlQualifiedName ServerFaultCode = new XmlQualifiedName("Server", "http://schemas.xmlsoap.org/soap/envelope/");

        /// <summary>Specifies a SOAP fault code that represents a client call that is not formatted correctly or does not contain the appropriate information.</summary>
        public static readonly XmlQualifiedName ClientFaultCode = new XmlQualifiedName("Client", "http://schemas.xmlsoap.org/soap/envelope/");

        /// <summary>A SOAP fault code that represents an invalid namespace for a SOAP envelope was found during the processing of the SOAP message.</summary>
        public static readonly XmlQualifiedName VersionMismatchFaultCode = new XmlQualifiedName("VersionMismatch", "http://schemas.xmlsoap.org/soap/envelope/");

        /// <summary>A SOAP Fault Code that represents a SOAP element marked with the MustUnderstand attribute was not processed.</summary>
        public static readonly XmlQualifiedName MustUnderstandFaultCode = new XmlQualifiedName("MustUnderstand", "http://schemas.xmlsoap.org/soap/envelope/");

        /// <summary>Gets an <see cref="T:System.Xml.XmlQualifiedName" /> that represents the <see cref="P:System.Web.Services.Protocols.SoapException.Detail" /> element of a SOAP Fault code.</summary>
        public static readonly XmlQualifiedName DetailElementName = new XmlQualifiedName("detail", "");
    }
}
