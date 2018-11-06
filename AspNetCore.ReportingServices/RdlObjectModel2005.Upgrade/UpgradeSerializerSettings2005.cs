using AspNetCore.ReportingServices.RdlObjectModel;
using AspNetCore.ReportingServices.RdlObjectModel.Serialization;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel2005.Upgrade
{
	internal class UpgradeSerializerSettings2005 : RdlSerializerSettings
	{
		private const string m_xsdResourceId = "AspNetCore.ReportingServices.RdlObjectModel.RdlUpgrade.Rdl2005ObjectModel.ReportDefinition.xsd";

		private bool m_skippingInvalidElements;

		private SerializerHost2005 m_host;

		private static XmlElementAttribute[] m_deserializingReportItems = new XmlElementAttribute[10]
		{
			new XmlElementClassAttribute("Line", typeof(Line2005)),
			new XmlElementClassAttribute("Rectangle", typeof(Rectangle2005)),
			new XmlElementClassAttribute("Textbox", typeof(Textbox2005)),
			new XmlElementClassAttribute("Image", typeof(Image2005)),
			new XmlElementClassAttribute("Subreport", typeof(Subreport2005)),
			new XmlElementClassAttribute("Chart", typeof(Chart2005)),
			new XmlElementClassAttribute("List", typeof(List2005)),
			new XmlElementClassAttribute("Table", typeof(Table2005)),
			new XmlElementClassAttribute("Matrix", typeof(Matrix2005)),
			new XmlElementClassAttribute("CustomReportItem", typeof(CustomReportItem2005))
		};

		public SerializerHost2005 SerializerHost
		{
			get
			{
				return this.m_host;
			}
		}

		private UpgradeSerializerSettings2005(bool serializing)
		{
			this.m_host = new SerializerHost2005(serializing);
			base.Host = this.m_host;
		}

		public static UpgradeSerializerSettings2005 CreateReaderSettings()
		{
			UpgradeSerializerSettings2005 upgradeSerializerSettings = new UpgradeSerializerSettings2005(false);
			upgradeSerializerSettings.ValidateXml = true;
			upgradeSerializerSettings.Normalize = false;
			upgradeSerializerSettings.XmlSchema = XmlSchema.Read(Assembly.GetExecutingAssembly().GetManifestResourceStream("AspNetCore.ReportingServices.RdlObjectModel.RdlUpgrade.Rdl2005ObjectModel.ReportDefinition.xsd"), null);
			UpgradeSerializerSettings2005 upgradeSerializerSettings2 = upgradeSerializerSettings;
			upgradeSerializerSettings2.XmlValidationEventHandler = (ValidationEventHandler)Delegate.Combine(upgradeSerializerSettings2.XmlValidationEventHandler, new ValidationEventHandler(upgradeSerializerSettings.ValidationEventHandler));
			upgradeSerializerSettings.IgnoreWhitespace = true;
			XmlAttributeOverrides xmlAttributeOverrides2 = upgradeSerializerSettings.XmlAttributeOverrides = new XmlAttributeOverrides();
			XmlAttributes xmlAttributes = new XmlAttributes();
			XmlElementAttribute[] deserializingReportItems = UpgradeSerializerSettings2005.m_deserializingReportItems;
			foreach (XmlElementAttribute attribute in deserializingReportItems)
			{
				xmlAttributes.XmlElements.Add(attribute);
			}
			xmlAttributeOverrides2.Add(typeof(AspNetCore.ReportingServices.RdlObjectModel.ReportItem), xmlAttributes);
			xmlAttributes = new XmlAttributes();
			xmlAttributes.XmlElements.Add(new XmlElementAttribute("SortBy", typeof(SortBy2005)));
			xmlAttributeOverrides2.Add(typeof(SortExpression), xmlAttributes);
			return upgradeSerializerSettings;
		}

		public static UpgradeSerializerSettings2005 CreateWriterSettings()
		{
			return new UpgradeSerializerSettings2005(true);
		}

		private void ValidationEventHandler(object sender, ValidationEventArgs e)
		{
			XmlReader xmlReader = sender as XmlReader;
			if (xmlReader != null)
			{
				string b = RDLUpgrader.Get2005NamespaceURI();
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
