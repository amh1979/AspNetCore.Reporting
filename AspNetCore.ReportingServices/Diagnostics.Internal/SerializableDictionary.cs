using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Internal
{
    internal sealed class SerializableDictionary : Dictionary<string, int>, IXmlSerializable
	{
		internal SerializableDictionary()
		{
		}

		internal SerializableDictionary(IDictionary<string, int> dictionary)
			: base(dictionary)
		{
		}

		internal SerializableDictionary(IEqualityComparer<string> comparer)
			: base(comparer)
		{
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			foreach (KeyValuePair<string, int> item in this)
			{
				writer.WriteStartElement(item.Key);
				writer.WriteValue(item.Value);
				writer.WriteEndElement();
			}
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			throw new NotImplementedException();
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}
	}
}
