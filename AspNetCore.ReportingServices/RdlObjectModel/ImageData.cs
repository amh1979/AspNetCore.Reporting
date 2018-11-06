using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	internal struct ImageData : IXmlSerializable
	{
		private byte[] m_data;

		public byte[] Bytes
		{
			get
			{
				return this.m_data;
			}
			set
			{
				this.m_data = value;
			}
		}

		public ImageData(byte[] bytes)
		{
			this.m_data = bytes;
		}

		public static implicit operator ImageData(byte[] bytes)
		{
			return new ImageData(bytes);
		}

		public static explicit operator byte[](ImageData value)
		{
			return value.Bytes;
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			string s = reader.ReadString();
			this.m_data = Convert.FromBase64String(s);
			reader.Skip();
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			if (this.m_data != null)
			{
				string text = Convert.ToBase64String(this.m_data);
				for (int i = 0; i < text.Length; i += 1000)
				{
					if (i > 0)
					{
						writer.WriteString("\n");
					}
					int length = Math.Min(1000, text.Length - i);
					char[] array = text.ToCharArray(i, length);
					writer.WriteChars(array, 0, array.Length);
				}
			}
		}
	}
}
