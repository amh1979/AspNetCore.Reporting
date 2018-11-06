using System.Xml.Schema;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel.Serialization
{
	internal class RdlSerializerSettings
	{
		private ISerializerHost m_serializerHost;

		private XmlAttributeOverrides m_xmlOverrides;

		private XmlSchema m_xmlSchema;

		private bool m_validate = true;

		private ValidationEventHandler m_validationEventHandler;

		private bool m_normalize = true;

		private bool m_ignoreWhitespace;

		internal ISerializerHost Host
		{
			get
			{
				return this.m_serializerHost;
			}
			set
			{
				this.m_serializerHost = value;
			}
		}

		internal XmlAttributeOverrides XmlAttributeOverrides
		{
			get
			{
				return this.m_xmlOverrides;
			}
			set
			{
				this.m_xmlOverrides = value;
			}
		}

		internal XmlSchema XmlSchema
		{
			get
			{
				return this.m_xmlSchema;
			}
			set
			{
				this.m_xmlSchema = value;
			}
		}

		internal bool ValidateXml
		{
			get
			{
				return this.m_validate;
			}
			set
			{
				this.m_validate = value;
			}
		}

		internal ValidationEventHandler XmlValidationEventHandler
		{
			get
			{
				return this.m_validationEventHandler;
			}
			set
			{
				this.m_validationEventHandler = value;
			}
		}

		internal bool IgnoreWhitespace
		{
			get
			{
				return this.m_ignoreWhitespace;
			}
			set
			{
				this.m_ignoreWhitespace = value;
			}
		}

		internal bool Normalize
		{
			get
			{
				return this.m_normalize;
			}
			set
			{
				this.m_normalize = value;
			}
		}
	}
}
