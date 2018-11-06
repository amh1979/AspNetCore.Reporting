using AspNetCore.ReportingServices.RdlObjectModel.Serialization;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel2010.Upgrade
{
	internal class UpgradeSerializerSettings2010 : RdlSerializerSettings
	{
		private const string m_xsdResourceId = "AspNetCore.ReportingServices.RdlObjectModel.RdlUpgrade.Rdl2010ObjectModel.ReportDefinition.xsd";

		private bool m_skippingInvalidElements;

		private SerializerHost2010 m_host;

		private static XmlElementAttribute[] m_deserializingReportItems = new XmlElementAttribute[0];

		public SerializerHost2010 SerializerHost
		{
			get
			{
				return this.m_host;
			}
		}

		private UpgradeSerializerSettings2010(bool serializing)
		{
			this.m_host = new SerializerHost2010(serializing);
			base.Host = this.m_host;
		}

		public static UpgradeSerializerSettings2010 CreateReaderSettings()
		{
			UpgradeSerializerSettings2010 upgradeSerializerSettings = new UpgradeSerializerSettings2010(false);
			upgradeSerializerSettings.ValidateXml = true;
			upgradeSerializerSettings.Normalize = false;
			upgradeSerializerSettings.XmlSchema = XmlSchema.Read(Assembly.GetExecutingAssembly().GetManifestResourceStream("AspNetCore.ReportingServices.RdlObjectModel.RdlUpgrade.Rdl2010ObjectModel.ReportDefinition.xsd"), null);
			UpgradeSerializerSettings2010 upgradeSerializerSettings2 = upgradeSerializerSettings;
			upgradeSerializerSettings2.XmlValidationEventHandler = (ValidationEventHandler)Delegate.Combine(upgradeSerializerSettings2.XmlValidationEventHandler, new ValidationEventHandler(upgradeSerializerSettings.ValidationEventHandler));
			upgradeSerializerSettings.IgnoreWhitespace = false;
			return upgradeSerializerSettings;
		}

		public static UpgradeSerializerSettings2010 CreateWriterSettings()
		{
			return new UpgradeSerializerSettings2010(true);
		}

		private void ValidationEventHandler(object sender, ValidationEventArgs e)
		{
			XmlReader xmlReader = sender as XmlReader;
			if (xmlReader != null)
			{
				string b = RDLUpgrader.Get2010NamespaceURI();
				if (xmlReader.NamespaceURI == b)
				{
					throw e.Exception;
				}
				if (!this.m_skippingInvalidElements)
				{
					this.m_skippingInvalidElements = true;
					StringBuilder stringBuilder = new StringBuilder();
					while (!xmlReader.EOF)
					{
						if (xmlReader.NodeType != XmlNodeType.Element && xmlReader.NodeType != XmlNodeType.Text)
						{
							break;
						}
						if (!(xmlReader.NamespaceURI != b))
						{
							break;
						}
						if (xmlReader.NodeType == XmlNodeType.Text)
						{
							stringBuilder.Append(xmlReader.ReadString());
						}
						else
						{
							xmlReader.Skip();
						}
						xmlReader.MoveToContent();
					}
					this.m_host.ExtraStringData = stringBuilder.ToString();
					this.m_skippingInvalidElements = false;
				}
			}
		}
	}
}
