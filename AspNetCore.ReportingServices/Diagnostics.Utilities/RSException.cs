using AspNetCore.ReportingServices.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal class RSException : Exception
    {
        protected RSException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        public RSException(ErrorCode errorCode, string localizedMessage, Exception innerException, RSTrace tracer, string additionalTraceMessage, params object[] exceptionData)
            : this(errorCode, localizedMessage, innerException, tracer, additionalTraceMessage, TraceLevel.Error, exceptionData)
        {
        }

        public RSException(ErrorCode errorCode, string localizedMessage, Exception innerException, RSTrace tracer, string additionalTraceMessage, TraceLevel traceLevel, params object[] exceptionData)
            : base($"{localizedMessage}{(innerException!=null? $"{Environment.NewLine}{innerException.Message}":"")}", innerException)
        {

            this.m_ErrorCode = errorCode;
            this.m_ProductLocaleID = CultureInfo.CurrentCulture.LCID;
            this.m_CountryLocaleID = CultureInfo.InstalledUICulture.LCID;
            this.m_OS = AspNetCore.ReportingServices.Diagnostics.Utilities.OperatingSystem.OsIndependent;
            this.m_AdditionalTraceMessage = additionalTraceMessage;
            this.m_tracer = tracer;
            this.m_traceLevel = traceLevel;
            this.m_exceptionData = exceptionData;
            this.Trace();
            this.OnExceptionCreated();
        }

        public RSException(RSException inner)
            : base(inner.Message, inner)
        {
            this.m_ErrorCode = inner.m_ErrorCode;
            this.m_ActorUri = inner.m_ActorUri;
            this.m_ProductName = inner.m_ProductName;
            this.m_ProductVersion = inner.m_ProductVersion;
            this.m_ProductLocaleID = inner.m_ProductLocaleID;
            this.m_CountryLocaleID = inner.m_CountryLocaleID;
            this.m_OS = inner.m_OS;
            this.m_AdditionalTraceMessage = inner.m_AdditionalTraceMessage;
        }
        internal sealed class AdditionalMessage
		{
			internal string Code
			{
				get;
				private set;
			}

			internal string Severity
			{
				get;
				private set;
			}

			internal string Message
			{
				get;
				private set;
			}

			internal string ObjectType
			{
				get;
				private set;
			}

			internal string ObjectName
			{
				get;
				private set;
			}

			internal string PropertyName
			{
				get;
				private set;
			}

			internal string[] AffectedItems
			{
				get;
				private set;
			}

			internal AdditionalMessage(string code, string severity, string message, string objectType = null, string objectName = null, string propertyName = null, string[] affectedItems = null)
			{
				this.Code = code;
				this.Severity = severity;
				this.Message = message;
				this.ObjectType = objectType;
				this.ObjectName = objectName;
				this.PropertyName = propertyName;
				this.AffectedItems = affectedItems;
			}
		}

		private const int ExceptionMessageLimit = 3000;

		private string m_toString;

		private string m_ActorUri = "";

		private ErrorCode m_ErrorCode = ErrorCode.rsInternalError;

		private string m_ProductName = "change this";

		private string m_ProductVersion = "1.0";

		private int m_ProductLocaleID = Localization.CatalogCulture.LCID;

		private int m_CountryLocaleID = Localization.ClientPrimaryCulture.LCID;

		private OperatingSystem m_OS;

		private string m_AdditionalTraceMessage;

		private RSTrace m_tracer;

		private TraceLevel m_traceLevel = TraceLevel.Error;

		private object[] m_exceptionData;

		public override string Message
		{
			get
			{
				return RSException.TrimExtraLength(base.Message);
			}
		}

		public string ExceptionLevelHelpLink
		{
			get
			{
				return this.CreateHelpLink(typeof(ErrorStrings).FullName, this.Code.ToString());
			}
		}

		public bool SkipTopLevelMessage
		{
			get
			{
				RSException ex = base.InnerException as RSException;
				if (ex != null)
				{
					return ex.Code == this.Code;
				}
				return false;
			}
		}

		public ErrorCode Code
		{
			get
			{
				return this.m_ErrorCode;
			}
		}

		internal List<AdditionalMessage> AdditionalMessages
		{
			get
			{
				return this.GetAdditionalMessages();
			}
		}

		protected virtual bool TraceFullException
		{
			get
			{
				return true;
			}
		}

		public static string ErrorNotVisibleOnRemoteBrowsers
		{
			get
			{
				return ErrorStrings.rsErrorNotVisibleToRemoteBrowsers;
			}
		}

		public string AdditionalTraceMessage
		{
			get
			{
				return this.m_AdditionalTraceMessage;
			}
		}

		public object[] ExceptionData
		{
			get
			{
				return this.m_exceptionData;
			}
		}

		internal string ProductName
		{
			get
			{
				return this.m_ProductName;
			}
		}

		internal string ProductVersion
		{
			get
			{
				return this.m_ProductVersion;
			}
		}

		internal int ProductLocaleID
		{
			get
			{
				return this.m_ProductLocaleID;
			}
		}

		internal string OperatingSystem
		{
			get
			{
				return this.m_OS.ToString();
			}
		}

		internal int CountryLocaleID
		{
			get
			{
				return this.m_CountryLocaleID;
			}
		}

		public static event EventHandler<RSExceptionCreatedEventArgs> ExceptionCreated;

		internal static bool IsClientLocal()
		{
			return true;
		}

		public override string ToString()
		{
			if (this.m_toString == null)
			{
				this.m_toString = RSException.TrimExtraLength(base.ToString());
				if (RSException.IsClientLocal())
				{
					this.m_toString = base.GetType() + ": " + this.Message;
					for (Exception innerException = base.InnerException; innerException != null; innerException = innerException.InnerException)
					{
						object toString = this.m_toString;
						this.m_toString = toString + " ---> " + innerException.GetType() + ": " + innerException.Message;
					}
				}
				this.m_toString = RSException.TrimExtraLength(this.m_toString);
			}
			return this.m_toString;
		}

		

		public void Trace()
		{
			if (this.m_tracer != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (this.TraceFullException)
				{
					stringBuilder.AppendFormat("Throwing {0}: {1}, {2};", base.GetType().FullName, this.m_AdditionalTraceMessage, base.ToString());
				}
				else
				{
					stringBuilder.AppendFormat("Throwing {0}: {1}, {2};", base.GetType().FullName, this.m_AdditionalTraceMessage, base.Message);
				}
				this.m_tracer.TraceException(this.m_traceLevel, (3000 > stringBuilder.Length) ? stringBuilder.ToString() : stringBuilder.ToString(0, 3000));
			}
		}

		public void SetExceptionProperties(string actorUri, string productName, string productVersion)
		{
			this.m_ActorUri = actorUri;
			this.m_ProductName = productName;
			this.m_ProductVersion = productVersion;
		}

		private XmlNode DetailsAsXml(bool enableRemoteErrors, out StringBuilder errorMsgBuilder)
		{
			string text = SoapUtil.RemoveInvalidXmlChars(this.Message);
			errorMsgBuilder = new StringBuilder();
			errorMsgBuilder.Append(text);
			return this.ToXml(new XmlDocument(), enableRemoteErrors, text, errorMsgBuilder);
		}

		public string DetailsAsXmlString(bool enableRemoteErrors)
		{
			StringBuilder stringBuilder = default(StringBuilder);
			XmlNode xmlNode = this.DetailsAsXml(enableRemoteErrors, out stringBuilder);
			return xmlNode.OuterXml;
		}


		protected XmlNode ToXml(XmlDocument doc, bool enableRemoteErrors, string detailedMsg, StringBuilder errorMsgBuilder)
		{
			XmlNode xmlNode = SoapUtil.CreateExceptionDetailsNode(doc, this.Code.ToString(), detailedMsg, this.ExceptionLevelHelpLink, this.m_ProductName, this.m_ProductVersion, this.m_ProductLocaleID, this.m_OS.ToString(), this.m_CountryLocaleID);
			this.AddMoreInformation(doc, xmlNode, enableRemoteErrors, errorMsgBuilder);
			this.AddWarningsInternal(doc, xmlNode);
			return xmlNode;
		}

		protected virtual XmlNode AddMoreInformationForThis(XmlDocument doc, XmlNode parent, StringBuilder errorMsgBuilder)
		{
			return this.AddMoreInformationForException(doc, parent, this, errorMsgBuilder);
		}

		internal void AddWarningsInternal(XmlDocument doc, XmlNode parent)
		{
			XmlNode xmlNode = SoapUtil.CreateWarningNode(doc);
			parent.AppendChild(xmlNode);
			this.AddWarnings(doc, xmlNode);
		}

		protected virtual void AddWarnings(XmlDocument doc, XmlNode parent)
		{
			RSException ex = base.InnerException as RSException;
			if (ex != null)
			{
				ex.AddWarnings(doc, parent);
			}
		}

		protected virtual List<AdditionalMessage> GetAdditionalMessages()
		{
			return null;
		}

		protected static XmlNode CreateMoreInfoNode(string source, XmlDocument doc, XmlNode parent)
		{
			XmlNode xmlNode = SoapUtil.CreateMoreInfoNode(doc);
			XmlNode xmlNode2 = SoapUtil.CreateMoreInfoSourceNode(doc);
			xmlNode2.InnerText = source;
			xmlNode.AppendChild(xmlNode2);
			if (parent != null)
			{
				parent.AppendChild(xmlNode);
			}
			return xmlNode;
		}

		protected static string AddMessageToMoreInfoNode(XmlDocument doc, XmlNode moreInfoNode, string errCode, string message, string helpLink)
		{
			XmlNode xmlNode = SoapUtil.CreateMoreInfoMessageNode(doc);
			string result = xmlNode.InnerText = SoapUtil.RemoveInvalidXmlChars(message);
			if (errCode != null)
			{
				XmlAttribute xmlAttribute = SoapUtil.CreateErrorCodeAttr(doc);
				xmlAttribute.Value = errCode;
				xmlNode.Attributes.Append(xmlAttribute);
				XmlAttribute xmlAttribute2 = SoapUtil.CreateHelpLinkTagAttr(doc);
				xmlAttribute2.Value = helpLink;
				xmlNode.Attributes.Append(xmlAttribute2);
			}
			moreInfoNode.AppendChild(xmlNode);
			return result;
		}

		protected static void AddWarningNode(XmlDocument doc, XmlNode parent, string code, string severity, string objectName, string objectType, string message)
		{
			XmlNode xmlNode = SoapUtil.CreateWarningNode(doc);
			XmlNode xmlNode2 = SoapUtil.CreateWarningCodeNode(doc);
			xmlNode2.InnerText = code;
			xmlNode.AppendChild(xmlNode2);
			XmlNode xmlNode3 = SoapUtil.CreateWarningSeverityNode(doc);
			xmlNode3.InnerText = severity;
			xmlNode.AppendChild(xmlNode3);
			XmlNode xmlNode4 = SoapUtil.CreateWarningObjectNameNode(doc);
			xmlNode4.InnerText = objectName;
			xmlNode.AppendChild(xmlNode4);
			XmlNode xmlNode5 = SoapUtil.CreateWarningObjectTypeNode(doc);
			xmlNode5.InnerText = objectType;
			xmlNode.AppendChild(xmlNode5);
			XmlNode xmlNode6 = SoapUtil.CreateWarningMessageNode(doc);
			xmlNode6.InnerText = SoapUtil.RemoveInvalidXmlChars(message);
			xmlNode.AppendChild(xmlNode6);
			parent.AppendChild(xmlNode);
		}

		protected string CreateHelpLink(string messageSource, string id)
		{
			return string.Format(CultureInfo.CurrentCulture, "https://go.microsoft.com/fwlink/?LinkId=20476&EvtSrc={0}&EvtID={1}&ProdName=Microsoft%20SQL%20Server%20Reporting%20Services&ProdVer={2}", messageSource, id, this.m_ProductVersion);
		}

		internal void AddMoreInformation(XmlDocument doc, XmlNode parent, bool enableRemoteErrors, StringBuilder errorMsgBuilder)
		{
			Exception ex = this;
			XmlNode parent2 = parent;
			if (this.SkipTopLevelMessage)
			{
				ex = base.InnerException;
			}
			bool flag = enableRemoteErrors || RSException.IsClientLocal();
			while (true)
			{
				if (ex != null)
				{
					RSException ex2 = ex as RSException;
					if (ex2 != null)
					{
						parent2 = ex2.AddMoreInformationForThis(doc, parent2, errorMsgBuilder);
						if (!flag && ex2 is SharePointException && ex2.InnerException != null)
						{
							parent2 = this.AddMoreInformationForException(doc, parent2, ex2.InnerException, errorMsgBuilder);
						}
					}
					else
					{
						if (!flag)
						{
							break;
						}
						parent2 = this.AddMoreInformationForException(doc, parent2, ex, errorMsgBuilder);
					}
					ex = ex.InnerException;
					continue;
				}
				return;
			}
			Exception e = new Exception(RSException.ErrorNotVisibleOnRemoteBrowsers);
			parent2 = this.AddMoreInformationForException(doc, parent2, e, errorMsgBuilder);
		}

		private XmlNode AddMoreInformationForException(XmlDocument doc, XmlNode parent, Exception e, StringBuilder errorMsgBuilder)
		{
			XmlNode xmlNode = RSException.CreateMoreInfoNode(e.Source, doc, parent);
			if (xmlNode != null)
			{
				string text = null;
				RSException ex = e as RSException;
				if (ex != null)
				{
					text = ex.Code.ToString();
				}
				string helpLink = this.CreateHelpLink(typeof(ErrorStrings).FullName, text);
				string filteredMsg = RSException.AddMessageToMoreInfoNode(doc, xmlNode, text, e.Message, helpLink);
				RSException.BuildExceptionMessage(e, filteredMsg, errorMsgBuilder);
			}
			return xmlNode;
		}

		private static void BuildExceptionMessage(Exception e, string filteredMsg, StringBuilder errorMsgBuilder)
		{
			if (e != null)
			{
				errorMsgBuilder.Append(" ---> " + e.GetType().FullName);
				if (!string.IsNullOrEmpty(filteredMsg))
				{
					errorMsgBuilder.Append(": " + filteredMsg);
				}
			}
		}

		internal bool ContainsErrorCode(ErrorCode code)
		{
			for (RSException ex = this; ex != null; ex = (ex.InnerException as RSException))
			{
				if (code == ex.Code)
				{
					return true;
				}
			}
			return false;
		}

		private static string TrimExtraLength(string input)
		{
			return input.Substring(0, Math.Min(3000, input.Length));
		}

		private void OnExceptionCreated()
		{
			EventHandler<RSExceptionCreatedEventArgs> exceptionCreated = RSException.ExceptionCreated;
			if (exceptionCreated != null)
			{
				RSExceptionCreatedEventArgs e = new RSExceptionCreatedEventArgs(this);
				exceptionCreated(this, e);
			}
		}
	}
}
