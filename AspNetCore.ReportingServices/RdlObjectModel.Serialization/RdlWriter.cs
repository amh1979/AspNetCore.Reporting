using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace AspNetCore.ReportingServices.RdlObjectModel.Serialization
{
	internal class RdlWriter : RdlReaderWriterBase
	{
		public RdlWriter(RdlSerializerSettings settings)
			: base(settings)
		{
		}

		public void Serialize(XmlWriter writer, object root)
		{
			XmlDocument xmlDocument = new XmlDocument();
			XPathNavigator xPathNavigator = xmlDocument.CreateNavigator();
			using (XmlWriter xmlWriter = xPathNavigator.AppendChild())
			{
				xmlWriter.WriteStartDocument();
				this.WriteObject(xmlWriter, root, null, null, null, 0);
				xmlWriter.WriteEndDocument();
			}
			NamespaceUpdater namespaceUpdater = new NamespaceUpdater();
			namespaceUpdater.Update(xmlDocument, base.Host);
			xmlDocument.Save(writer);
		}

		private void WriteObject(XmlWriter writer, object obj, string name, string ns, MemberMapping member, int nestingLevel)
		{
			if (obj != null && (!(obj is IShouldSerialize) || ((IShouldSerialize)obj).ShouldSerializeThis()))
			{
				this.WriteObjectContent(writer, null, obj, name, ns, member, nestingLevel);
			}
		}

		private void WriteObjectContent(XmlWriter writer, object component, object obj, string name, string ns, MemberMapping member, int nestingLevel)
		{
			Type serializationType = base.GetSerializationType(obj);
			TypeMapping typeMapping = TypeMapper.GetTypeMapping(serializationType);
			if (name == null)
			{
				name = typeMapping.Name;
				ns = typeMapping.Namespace;
			}
			if (typeMapping is PrimitiveMapping)
			{
				this.WritePrimitive(writer, component, obj, name, ns, member, typeMapping);
			}
			else if (typeMapping is ArrayMapping)
			{
				this.WriteArray(writer, component, obj, name, ns, member, (ArrayMapping)typeMapping, nestingLevel);
			}
			else if (typeMapping is SpecialMapping)
			{
				this.WriteSpecialMapping(writer, component, obj, name, ns, member);
			}
			else if (typeMapping is StructMapping)
			{
				this.WriteStructure(writer, component, obj, name, ns, member, (StructMapping)typeMapping);
			}
		}

		private void WriteStartElement(XmlWriter writer, object component, string name, string ns, MemberMapping member)
		{
			writer.WriteStartElement(name, ns);
			if (component != null && member != null && member.ChildAttributes != null)
			{
				foreach (MemberMapping childAttribute in member.ChildAttributes)
				{
					this.WriteChildAttribute(writer, childAttribute.GetValue(component), childAttribute);
				}
			}
		}

		private void WriteSpecialContent(XmlWriter writer, object obj)
		{
			IXmlSerializable xmlSerializable = (IXmlSerializable)obj;
			if (xmlSerializable != null)
			{
				xmlSerializable.WriteXml(writer);
			}
		}

		private void WriteSpecialMapping(XmlWriter writer, object component, object obj, string name, string ns, MemberMapping member)
		{
			this.WriteStartElement(writer, component, name, ns, member);
			this.WriteSpecialContent(writer, obj);
			writer.WriteEndElement();
		}

		private void WritePrimitiveContent(XmlWriter writer, TypeMapping mapping, object obj)
		{
			if (obj != null)
			{
				Type type = obj.GetType();
				string text = null;
				if (type == typeof(string))
				{
					text = (string)obj;
					if (text == "")
					{
						return;
					}
				}
				else
				{
					text = ((type != typeof(bool)) ? ((type != typeof(DateTime)) ? obj.ToString() : XmlCustomFormatter.FromDateTime((DateTime)obj)) : (((bool)obj) ? "true" : "false"));
				}
				writer.WriteString(text);
			}
		}

		private void WritePrimitive(XmlWriter writer, object component, object obj, string name, string ns, MemberMapping member, TypeMapping mapping)
		{
			this.WriteStartElement(writer, component, name, ns, member);
			this.WritePrimitiveContent(writer, mapping, obj);
			writer.WriteEndElement();
		}

		private void WriteArrayContent(XmlWriter writer, object array, ArrayMapping mapping, MemberMapping member, int nestingLevel, string ns)
		{
			Dictionary<string, Type> elementTypes = mapping.ElementTypes;
			foreach (object item in (IEnumerable)array)
			{
				string text = null;
				bool flag = false;
				if (member != null && member.XmlAttributes.XmlArrayItems.Count > nestingLevel)
				{
					XmlArrayItemAttribute xmlArrayItemAttribute = member.XmlAttributes.XmlArrayItems[nestingLevel];
					text = xmlArrayItemAttribute.ElementName;
					flag = xmlArrayItemAttribute.IsNullable;
				}
				else
				{
					Type serializationType = base.GetSerializationType(item);
					TypeMapping typeMapping = TypeMapper.GetTypeMapping(serializationType);
					if (typeMapping != null)
					{
						text = typeMapping.Name;
						ns = typeMapping.Namespace;
					}
					else
					{
						foreach (KeyValuePair<string, Type> item2 in elementTypes)
						{
							if (item2.Value == serializationType)
							{
								text = item2.Key;
								break;
							}
						}
					}
				}
				if (text == null)
				{
					throw new Exception("No array element name.");
				}
				if (item != null)
				{
					this.WriteObject(writer, item, text, ns, member, nestingLevel + 1);
				}
				else if (flag)
				{
					this.WriteNilElement(writer, text, ns);
				}
			}
		}

		private bool ShouldSerializeArray(object array)
		{
			if (array is ICollection && ((ICollection)array).Count == 0)
			{
				return false;
			}
			foreach (object item in (IEnumerable)array)
			{
				if (!(item is IShouldSerialize) || ((IShouldSerialize)item).ShouldSerializeThis())
				{
					return true;
				}
			}
			return false;
		}

		private void WriteArray(XmlWriter writer, object component, object array, string name, string ns, MemberMapping member, ArrayMapping mapping, int nestingLevel)
		{
			if (this.ShouldSerializeArray(array))
			{
				this.WriteStartElement(writer, component, name, ns, member);
				this.WriteArrayContent(writer, array, mapping, member, nestingLevel, ns);
				writer.WriteEndElement();
			}
		}

		private bool ShouldSerializeValue(object component, object obj, MemberMapping memberMapping)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is IShouldSerialize && !((IShouldSerialize)obj).ShouldSerializeThis())
			{
				return false;
			}
			if (component is IShouldSerialize)
			{
				switch (((IShouldSerialize)component).ShouldSerializeProperty(memberMapping.Name))
				{
				case SerializationMethod.Never:
					return false;
				case SerializationMethod.Always:
					return true;
				}
			}
			object xmlDefaultValue = memberMapping.XmlAttributes.XmlDefaultValue;
			if (xmlDefaultValue != null && obj.Equals(xmlDefaultValue))
			{
				return false;
			}
			return true;
		}

		private void WriteMember(XmlWriter writer, object component, object obj, MemberMapping memberMapping, string name, string ns)
		{
			if (this.ShouldSerializeValue(component, obj, memberMapping))
			{
				XmlElementAttributes xmlElements = memberMapping.XmlAttributes.XmlElements;
				if (xmlElements.Count > 0)
				{
					Type serializationType = base.GetSerializationType(obj);
					foreach (XmlElementAttribute item in xmlElements)
					{
						if (serializationType == item.Type)
						{
							if (!string.IsNullOrEmpty(item.ElementName))
							{
								name = item.ElementName;
							}
							if (!string.IsNullOrEmpty(item.Namespace))
							{
								ns = item.Namespace;
							}
							break;
						}
					}
				}
				this.WriteObjectContent(writer, component, obj, name, ns, memberMapping, 0);
			}
		}

		private void WriteStructContent(XmlWriter writer, object obj, StructMapping mapping, string ns)
		{
			foreach (MemberMapping value2 in mapping.Attributes.Values)
			{
				if (value2.Type == typeof(string) && value2.XmlAttributes.XmlElements.Count == 0)
				{
					object value = value2.GetValue(obj);
					if (this.ShouldSerializeValue(obj, value, value2))
					{
						writer.WriteAttributeString(value2.Name, value2.Namespace, (value != null) ? ((string)value) : "");
					}
				}
			}
			foreach (MemberMapping member in mapping.Members)
			{
				if (member.XmlAttributes.XmlAttribute == null)
				{
					string ns2 = (member.Namespace != string.Empty) ? member.Namespace : ns;
					this.WriteMember(writer, obj, member.GetValue(obj), member, member.Name, ns2);
				}
			}
		}

		private void WriteStructure(XmlWriter writer, object component, object obj, string name, string ns, MemberMapping member, StructMapping mapping)
		{
			if (obj != null)
			{
				this.WriteStartElement(writer, component, name, ns, member);
				this.WriteStructContent(writer, obj, mapping, ns);
				writer.WriteEndElement();
			}
		}

		private void WriteNilElement(XmlWriter writer, string name, string ns)
		{
			writer.WriteStartElement(name, ns);
			writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
			writer.WriteAttributeString("nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
			writer.WriteEndElement();
		}

		private void WriteChildAttribute(XmlWriter writer, object obj, MemberMapping mapping)
		{
			if (obj != null)
			{
				XmlAttributeAttribute xmlAttribute = mapping.XmlAttributes.XmlAttribute;
				string value = obj.ToString();
				if (!string.IsNullOrEmpty(value))
				{
					writer.WriteAttributeString(xmlAttribute.AttributeName, xmlAttribute.Namespace, value);
				}
			}
		}
	}
}
