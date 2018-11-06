using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel.Serialization
{
	internal class RdlReader : RdlReaderWriterBase
	{
		private const string m_xsdResourceId = "AspNetCore.ReportingServices.RdlObjectModel.Serialization.ReportDefinition.xsd";

		private XmlReader m_reader;

		private RdlValidator m_validator;

		private XmlSchema m_schema;

		private readonly HashSet<string> m_validNamespaces = new HashSet<string>
		{
			"http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition",
			"http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition/defaultfontfamily"
		};

		public RdlReader(RdlSerializerSettings settings)
			: base(settings)
		{
		}

		public object Deserialize(Stream stream, Type rootType)
		{
			this.m_reader = XmlReader.Create(stream, this.GetXmlReaderSettings());
			return this.Deserialize(rootType);
		}

		public object Deserialize(TextReader textReader, Type rootType)
		{
			this.m_reader = XmlReader.Create(textReader, this.GetXmlReaderSettings());
			return this.Deserialize(rootType);
		}

		public object Deserialize(XmlReader xmlReader, Type rootType)
		{
			this.m_reader = XmlReader.Create(xmlReader, this.GetXmlReaderSettings());
			return this.Deserialize(rootType);
		}

		private object Deserialize(Type rootType)
		{
			List<string> list = new List<string>(this.m_validNamespaces);
			if (this.m_schema != null)
			{
				list.Add(this.m_schema.TargetNamespace);
			}
			if (base.Settings.ValidateXml)
			{
				this.m_validator = new RdlValidator(this.m_reader, list);
			}
			object result = this.ReadRoot(rootType);
			this.m_reader.Close();
			return result;
		}

		private XmlReaderSettings GetXmlReaderSettings()
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.CheckCharacters = false;
			xmlReaderSettings.IgnoreComments = true;
			xmlReaderSettings.IgnoreProcessingInstructions = true;
			xmlReaderSettings.IgnoreWhitespace = base.Settings.IgnoreWhitespace;
			if (base.Settings.ValidateXml)
			{
				xmlReaderSettings.ValidationType = ValidationType.Schema;
				XmlSchema schema = XmlSchema.Read(Assembly.GetExecutingAssembly().GetManifestResourceStream("AspNetCore.ReportingServices.RdlObjectModel.Serialization.ReportDefinition.xsd"), null);
				xmlReaderSettings.Schemas.Add(schema);
				if (base.Settings.XmlSchema != null)
				{
					if (base.Settings.XmlSchema.TargetNamespace.EndsWith("/reportdefinition", StringComparison.Ordinal))
					{
						this.m_schema = base.Settings.XmlSchema;
					}
					xmlReaderSettings.Schemas.Add(base.Settings.XmlSchema);
				}
				if (this.m_schema == null)
				{
					this.m_schema = schema;
				}
				if (base.Settings.XmlValidationEventHandler != null)
				{
					xmlReaderSettings.ValidationEventHandler += base.Settings.XmlValidationEventHandler;
				}
			}
			return xmlReaderSettings;
		}

		private object ReadRoot(Type type)
		{
			try
			{
				this.m_reader.MoveToContent();
				TypeMapping typeMapping = TypeMapper.GetTypeMapping(type);
				if (this.m_reader.NamespaceURI != typeMapping.Namespace)
				{
					throw new XmlException(SRErrors.NoRootElement);
				}
				return this.ReadObject(type, null, 0);
			}
			catch (XmlException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				Exception ex3 = ex2;
				if (ex3 is TargetInvocationException && ex3.InnerException != null)
				{
					ex3 = ex3.InnerException;
				}
				string message;
				if (ex3 is TargetInvocationException)
				{
					MethodBase targetSite = ((TargetInvocationException)ex3).TargetSite;
					string methodName = (targetSite != null) ? (targetSite.DeclaringType.Name + "." + targetSite.Name) : null;
					message = SRErrors.DeserializationFailedMethod(methodName);
				}
				else
				{
					message = SRErrors.DeserializationFailed(ex3.Message);
				}
				IXmlLineInfo xmlLineInfo = this.m_reader as IXmlLineInfo;
				XmlException ex4 = (xmlLineInfo == null) ? new XmlException(message, ex3) : new XmlException(message, ex3, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
				throw ex4;
			}
		}

		private object ReadObject(Type type, MemberMapping member, int nestingLevel)
		{
			this.ValidateStartElement();
			object result = (!TypeMapper.IsPrimitiveType(type)) ? this.ReadClassObject(type, member, nestingLevel) : this.ReadPrimitive(type);
			this.ValidateEndElement();
			return result;
		}

		private object ReadObjectContent(object value, MemberMapping member, int nestingLevel)
		{
			Type type = value.GetType();
			TypeMapping typeMapping = TypeMapper.GetTypeMapping(type);
			if (typeMapping is ArrayMapping)
			{
				this.ReadArrayContent(value, (ArrayMapping)typeMapping, member, nestingLevel);
			}
			else if (typeMapping is StructMapping)
			{
				this.ReadStructContent(value, (StructMapping)typeMapping);
			}
			else if (typeMapping is SpecialMapping)
			{
				this.ReadSpecialContent(value);
			}
			else
			{
				this.m_reader.Skip();
			}
			if (base.Host != null)
			{
				base.Host.OnDeserialization(value);
			}
			return value;
		}

		private object ReadPrimitive(Type type)
		{
			object result = null;
			string text = this.m_reader.ReadString();
			if (type.IsPrimitive)
			{
				switch (Type.GetTypeCode(type))
				{
				case TypeCode.Boolean:
					result = XmlConvert.ToBoolean(text);
					break;
				case TypeCode.Int16:
					result = XmlConvert.ToInt16(text);
					break;
				case TypeCode.Int32:
					result = XmlConvert.ToInt32(text);
					break;
				case TypeCode.Int64:
					result = XmlConvert.ToInt64(text);
					break;
				case TypeCode.Double:
					result = XmlConvert.ToDouble(text);
					break;
				case TypeCode.Single:
					result = XmlConvert.ToSingle(text);
					break;
				}
			}
			else if (type == typeof(string))
			{
				result = text;
				if (base.Settings.Normalize)
				{
					result = Regex.Replace(text, "(?<!\r)\n", "\r\n");
				}
			}
			else if (type.IsEnum)
			{
				result = Enum.Parse(type, text, true);
			}
			else if (type == typeof(Guid))
			{
				result = new Guid(text);
			}
			else if (type == typeof(DateTime))
			{
				result = XmlCustomFormatter.ToDateTime(text);
			}
			this.m_reader.Skip();
			return result;
		}

		private object ReadClassObject(Type type, MemberMapping member, int nestingLevel)
		{
			type = base.GetSerializationType(type);
			object obj = Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);
			this.ReadObjectContent(obj, member, nestingLevel);
			return obj;
		}

		private object ReadSpecialContent(object obj)
		{
			IXmlSerializable xmlSerializable = (IXmlSerializable)obj;
			if (xmlSerializable != null)
			{
				xmlSerializable.ReadXml(this.m_reader);
			}
			return obj;
		}

		private object ReadArrayContent(object array, ArrayMapping mapping, MemberMapping member, int nestingLevel)
		{
			IList list = (IList)array;
			if (this.m_reader.IsEmptyElement)
			{
				this.m_reader.Skip();
			}
			else
			{
				this.m_reader.ReadStartElement();
				this.m_reader.MoveToContent();
				while (this.m_reader.NodeType != XmlNodeType.EndElement && this.m_reader.NodeType != 0)
				{
					if (this.m_reader.NodeType == XmlNodeType.Element)
					{
						string localName = this.m_reader.LocalName;
						string namespaceURI = this.m_reader.NamespaceURI;
						Type type = null;
						bool flag = false;
						if (member != null && member.XmlAttributes.XmlArrayItems.Count > nestingLevel)
						{
							if (localName == member.XmlAttributes.XmlArrayItems[nestingLevel].ElementName)
							{
								XmlArrayItemAttribute xmlArrayItemAttribute = member.XmlAttributes.XmlArrayItems[nestingLevel];
								type = xmlArrayItemAttribute.Type;
								flag = xmlArrayItemAttribute.IsNullable;
							}
						}
						else
						{
							XmlElementAttributes xmlElementAttributes = null;
							if (base.XmlOverrides != null)
							{
								XmlAttributes xmlAttributes = base.XmlOverrides[mapping.ItemType];
								if (xmlAttributes != null && xmlAttributes.XmlElements != null)
								{
									xmlElementAttributes = xmlAttributes.XmlElements;
								}
							}
							if (xmlElementAttributes == null)
							{
								mapping.ElementTypes.TryGetValue(localName, out type);
							}
							else
							{
								foreach (XmlElementAttribute item in xmlElementAttributes)
								{
									if (localName == item.ElementName)
									{
										type = item.Type;
										break;
									}
								}
							}
						}
						if (type != null)
						{
							object value;
							if (flag && this.m_reader.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance") == "true")
							{
								this.m_reader.Skip();
								value = null;
							}
							else
							{
								value = this.ReadObject(type, member, nestingLevel + 1);
							}
							list.Add(value);
						}
						else
						{
							this.m_reader.Skip();
						}
					}
					else
					{
						this.m_reader.Skip();
					}
					this.m_reader.MoveToContent();
				}
				this.m_reader.ReadEndElement();
			}
			return array;
		}

		private void ReadStructContent(object obj, StructMapping mapping)
		{
			this.m_reader.MoveToContent();
			string name = this.m_reader.Name;
			string namespaceURI = this.m_reader.NamespaceURI;
			this.ReadStructAttributes(obj, mapping);
			if (this.m_reader.IsEmptyElement)
			{
				this.m_reader.Skip();
			}
			else
			{
				this.m_reader.ReadStartElement();
				this.m_reader.MoveToContent();
				while (this.m_reader.NodeType != XmlNodeType.EndElement && this.m_reader.NodeType != 0)
				{
					string localName = this.m_reader.LocalName;
					string namespaceURI2 = this.m_reader.NamespaceURI;
					namespaceURI2 = ((namespaceURI == namespaceURI2) ? string.Empty : namespaceURI2);
					MemberMapping memberMapping = mapping.GetElement(localName, namespaceURI2);
					Type type = null;
					if (memberMapping != null)
					{
						type = memberMapping.Type;
					}
					else
					{
						List<MemberMapping> typeNameElements = mapping.GetTypeNameElements();
						if (typeNameElements != null)
						{
							bool flag = false;
							for (int i = 0; i < typeNameElements.Count; i++)
							{
								memberMapping = typeNameElements[i];
								XmlElementAttributes xmlElements = memberMapping.XmlAttributes.XmlElements;
								if (base.XmlOverrides != null)
								{
									XmlAttributes xmlAttributes = base.XmlOverrides[obj.GetType()];
									if (xmlAttributes == null)
									{
										xmlAttributes = base.XmlOverrides[memberMapping.Type];
									}
									if (xmlAttributes != null && xmlAttributes.XmlElements != null)
									{
										xmlElements = xmlAttributes.XmlElements;
									}
								}
								foreach (XmlElementAttribute item in xmlElements)
								{
									if (item.ElementName == localName && item.Type != null)
									{
										type = item.Type;
										flag = true;
										break;
									}
								}
								if (flag)
								{
									break;
								}
							}
						}
					}
					if (type != null)
					{
						if (memberMapping.ChildAttributes != null)
						{
							foreach (MemberMapping childAttribute in memberMapping.ChildAttributes)
							{
								this.ReadChildAttribute(obj, mapping, childAttribute);
							}
						}
						if (memberMapping.IsReadOnly)
						{
							if (!TypeMapper.IsPrimitiveType(type))
							{
								object value = memberMapping.GetValue(obj);
								if (value != null)
								{
									this.ReadObjectContent(value, memberMapping, 0);
								}
								else
								{
									this.m_reader.Skip();
								}
							}
							else
							{
								this.m_reader.Skip();
							}
						}
						else
						{
							object obj2 = this.ReadObject(type, memberMapping, 0);
							if (obj2 != null)
							{
								memberMapping.SetValue(obj, obj2);
							}
						}
					}
					else
					{
						if (namespaceURI2 != string.Empty && this.m_validNamespaces.Contains(namespaceURI2))
						{
							IXmlLineInfo xmlLineInfo = (IXmlLineInfo)this.m_reader;
							string message = RDLValidatingReaderStrings.rdlValidationInvalidMicroVersionedElement(this.m_reader.Name, name, xmlLineInfo.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), xmlLineInfo.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
							throw new XmlException(message);
						}
						this.m_reader.Skip();
					}
					this.m_reader.MoveToContent();
				}
				this.m_reader.ReadEndElement();
			}
		}

		private void ReadStructAttributes(object obj, StructMapping mapping)
		{
			if (this.m_reader.HasAttributes)
			{
				string text = null;
				foreach (MemberMapping value in mapping.Attributes.Values)
				{
					if (value.Type == typeof(string))
					{
						text = this.m_reader.GetAttribute(value.Name, value.Namespace);
						if (text != null)
						{
							value.SetValue(obj, text);
						}
					}
				}
			}
		}

		private void ReadChildAttribute(object obj, StructMapping mapping, MemberMapping childMapping)
		{
			XmlAttributeAttribute xmlAttribute = childMapping.XmlAttributes.XmlAttribute;
			string attribute = this.m_reader.GetAttribute(xmlAttribute.AttributeName, xmlAttribute.Namespace);
			if (attribute != null)
			{
				childMapping.SetValue(obj, attribute);
			}
		}

		private void ValidateStartElement()
		{
			if (!base.Settings.ValidateXml)
			{
				return;
			}
			string str = default(string);
			if (this.m_validator.ValidateStartElement(out str))
			{
				return;
			}
			throw new XmlSchemaException(str + "\r\n");
		}

		private void ValidateEndElement()
		{
			if (!base.Settings.ValidateXml)
			{
				return;
			}
			string str = default(string);
			if (this.m_validator.ValidateEndElement(out str))
			{
				return;
			}
			throw new XmlSchemaException(str + "\r\n");
		}
	}
}
