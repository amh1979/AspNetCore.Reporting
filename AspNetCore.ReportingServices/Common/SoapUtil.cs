using System;
using System.Globalization;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;

namespace AspNetCore.ReportingServices.Common
{
	internal static class SoapUtil
	{
		internal const string DefaultServerErrorNamespace = "http://www.microsoft.com/sql/reportingservices";

		internal const string DefaultNamespacePrefix = "msrs";

		internal const string HelpLinkFormat = "https://go.microsoft.com/fwlink/?LinkId=20476&EvtSrc={0}&EvtID={1}&ProdName=Microsoft%20SQL%20Server%20Reporting%20Services&ProdVer={2}";

		internal const string HelpLinkTag = "HelpLink";

		internal const string XmlMoreInfoElement = "MoreInformation";

		internal const string XmlMoreInfoSource = "Source";

		internal const string XmlMoreInfoMessage = "Message";

		internal const string XmlErrorCode = "ErrorCode";

		internal const string XmlWarningsElement = "Warnings";

		internal const string XmlWarningElement = "Warning";

		internal const string XmlWarningCodeElement = "Code";

		internal const string XmlWarningSeverityElement = "Severity";

		internal const string XmlWarningObjectNameElement = "ObjectName";

		internal const string XmlWarningObjectTypeElement = "ObjectType";

		internal const string XmlWarningMessageElement = "Message";

		internal const string SoapExceptionHttpStatus = "400";

		internal static string RemoveInvalidXmlChars(string origText)
		{
			if (string.IsNullOrEmpty(origText))
			{
				return origText;
			}
			string result = origText;
			try
			{
				bool flag = false;
				StringBuilder stringBuilder = new StringBuilder(origText);
				for (int i = 0; i < stringBuilder.Length; i++)
				{
					char c = stringBuilder[i];
					if (c > '�' || (c < ' ' && c != '\t' && c != '\n' && c != '\r'))
					{
						stringBuilder[i] = ' ';
						flag = true;
					}
				}
				if (flag)
				{
					result = stringBuilder.ToString();
					return result;
				}
				return result;
			}
			catch (Exception)
			{
				return result;
			}
		}

		internal static XmlNode CreateExceptionDetailsNode(XmlDocument doc, string code, string detailedMsg, string helpLink, string productName, string productVersion, int productLocaleId, string operatingSystem, int countryLocaleId)
		{
			XmlNode xmlNode = doc.CreateNode(XmlNodeType.Element, SoapException.DetailElementName.Name, SoapException.DetailElementName.Namespace);
			XmlNode xmlNode2 = SoapUtil.CreateNode(doc, "ErrorCode");
			xmlNode2.InnerText = code;
			xmlNode.AppendChild(xmlNode2);
			XmlNode xmlNode3 = SoapUtil.CreateNode(doc, "HttpStatus");
			xmlNode3.InnerText = "400";
			xmlNode.AppendChild(xmlNode3);
			XmlNode xmlNode4 = SoapUtil.CreateNode(doc, "Message");
			xmlNode4.InnerText = detailedMsg;
			xmlNode.AppendChild(xmlNode4);
			XmlNode xmlNode5 = SoapUtil.CreateNode(doc, "HelpLink");
			xmlNode5.InnerText = helpLink;
			xmlNode.AppendChild(xmlNode5);
			XmlNode xmlNode6 = SoapUtil.CreateNode(doc, "ProductName");
			xmlNode6.InnerText = productName;
			xmlNode.AppendChild(xmlNode6);
			XmlNode xmlNode7 = SoapUtil.CreateNode(doc, "ProductVersion");
			xmlNode7.InnerText = productVersion;
			xmlNode.AppendChild(xmlNode7);
			XmlNode xmlNode8 = SoapUtil.CreateNode(doc, "ProductLocaleId");
			xmlNode8.InnerText = productLocaleId.ToString(CultureInfo.InvariantCulture);
			xmlNode.AppendChild(xmlNode8);
			XmlNode xmlNode9 = SoapUtil.CreateNode(doc, "OperatingSystem");
			xmlNode9.InnerText = operatingSystem;
			xmlNode.AppendChild(xmlNode9);
			XmlNode xmlNode10 = SoapUtil.CreateNode(doc, "CountryLocaleId");
			xmlNode10.InnerText = countryLocaleId.ToString(CultureInfo.InvariantCulture);
			xmlNode.AppendChild(xmlNode10);
			return xmlNode;
		}

		internal static XmlNode CreateNode(XmlDocument doc, string name)
		{
			return doc.CreateNode(XmlNodeType.Element, name, "http://www.microsoft.com/sql/reportingservices");
		}

		internal static XmlNode CreateWarningNode(XmlDocument doc)
		{
			return SoapUtil.CreateNode(doc, "Warnings");
		}

		internal static XmlNode CreateWarningCodeNode(XmlDocument doc)
		{
			return SoapUtil.CreateNode(doc, "Code");
		}

		internal static XmlNode CreateWarningSeverityNode(XmlDocument doc)
		{
			return SoapUtil.CreateNode(doc, "Severity");
		}

		internal static XmlNode CreateWarningObjectNameNode(XmlDocument doc)
		{
			return SoapUtil.CreateNode(doc, "ObjectName");
		}

		internal static XmlNode CreateWarningObjectTypeNode(XmlDocument doc)
		{
			return SoapUtil.CreateNode(doc, "ObjectType");
		}

		internal static XmlNode CreateWarningMessageNode(XmlDocument doc)
		{
			return SoapUtil.CreateNode(doc, "Message");
		}

		internal static XmlNode CreateMoreInfoNode(XmlDocument doc)
		{
			return SoapUtil.CreateNode(doc, "MoreInformation");
		}

		internal static XmlNode CreateMoreInfoSourceNode(XmlDocument doc)
		{
			return SoapUtil.CreateNode(doc, "Source");
		}

		internal static XmlNode CreateMoreInfoMessageNode(XmlDocument doc)
		{
			return SoapUtil.CreateNode(doc, "Message");
		}

		internal static XmlAttribute CreateErrorCodeAttr(XmlDocument doc)
		{
			return doc.CreateAttribute("msrs", "ErrorCode", "http://www.microsoft.com/sql/reportingservices");
		}

		internal static XmlAttribute CreateHelpLinkTagAttr(XmlDocument doc)
		{
			return doc.CreateAttribute("msrs", "HelpLink", "http://www.microsoft.com/sql/reportingservices");
		}
	}
}
