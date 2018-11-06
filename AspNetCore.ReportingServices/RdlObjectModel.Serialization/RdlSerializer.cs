using System;
using System.IO;
using System.Xml;

namespace AspNetCore.ReportingServices.RdlObjectModel.Serialization
{
	internal class RdlSerializer
	{
		private RdlSerializerSettings m_settings;

		public RdlSerializerSettings Settings
		{
			get
			{
				return this.m_settings;
			}
		}

		public RdlSerializer()
		{
			this.m_settings = new RdlSerializerSettings();
		}

		public RdlSerializer(RdlSerializerSettings settings)
		{
			this.m_settings = settings;
		}

		public Report Deserialize(Stream stream)
		{
			return (Report)this.Deserialize(stream, typeof(Report));
		}

		public Report Deserialize(TextReader textReader)
		{
			return (Report)this.Deserialize(textReader, typeof(Report));
		}

		public Report Deserialize(XmlReader xmlReader)
		{
			return (Report)this.Deserialize(xmlReader, typeof(Report));
		}

		public object Deserialize(Stream stream, Type objectType)
		{
			RdlReader rdlReader = new RdlReader(this.m_settings);
			return rdlReader.Deserialize(stream, objectType);
		}

		public object Deserialize(TextReader textReader, Type objectType)
		{
			RdlReader rdlReader = new RdlReader(this.m_settings);
			return rdlReader.Deserialize(textReader, objectType);
		}

		public object Deserialize(XmlReader xmlReader, Type objectType)
		{
			RdlReader rdlReader = new RdlReader(this.m_settings);
			return rdlReader.Deserialize(xmlReader, objectType);
		}

		public void Serialize(Stream stream, object o)
		{
			XmlWriter xmlWriter = XmlWriter.Create(stream, this.GetXmlWriterSettings());
			this.Serialize(xmlWriter, o);
		}

		public void Serialize(TextWriter textWriter, object o)
		{
			XmlWriter xmlWriter = XmlWriter.Create(textWriter, this.GetXmlWriterSettings());
			this.Serialize(xmlWriter, o);
		}

		public void Serialize(XmlWriter xmlWriter, object o)
		{
			RdlWriter rdlWriter = new RdlWriter(this.m_settings);
			rdlWriter.Serialize(xmlWriter, o);
		}

		private XmlWriterSettings GetXmlWriterSettings()
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Indent = true;
			return xmlWriterSettings;
		}
	}
}
