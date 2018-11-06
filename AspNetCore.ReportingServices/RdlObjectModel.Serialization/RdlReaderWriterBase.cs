using System;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel.Serialization
{
	internal abstract class RdlReaderWriterBase
	{
		private RdlSerializerSettings m_settings;

		private ISerializerHost m_host;

		private XmlAttributeOverrides m_xmlOverrides;

		protected RdlSerializerSettings Settings
		{
			get
			{
				return this.m_settings;
			}
		}

		protected ISerializerHost Host
		{
			get
			{
				return this.m_host;
			}
		}

		protected XmlAttributeOverrides XmlOverrides
		{
			get
			{
				return this.m_xmlOverrides;
			}
		}

		protected RdlReaderWriterBase(RdlSerializerSettings settings)
		{
			this.m_settings = settings;
			if (this.m_settings != null)
			{
				this.m_host = this.m_settings.Host;
				this.m_xmlOverrides = this.m_settings.XmlAttributeOverrides;
			}
		}

		protected Type GetSerializationType(object obj)
		{
			return this.GetSerializationType(obj.GetType());
		}

		protected Type GetSerializationType(Type type)
		{
			if (this.m_host != null)
			{
				return this.m_host.GetSubstituteType(type);
			}
			return type;
		}
	}
}
