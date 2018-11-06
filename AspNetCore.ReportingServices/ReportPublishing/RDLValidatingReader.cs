using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Schema;

namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal class RDLValidatingReader : XmlReader
	{
		private sealed class RdlElementStack : ArrayList
		{
			internal new Hashtable this[int index]
			{
				get
				{
					return (Hashtable)base[index];
				}
				set
				{
					base[index] = value;
				}
			}

			internal RdlElementStack()
			{
			}
		}

		private sealed class XmlNullResolver : XmlUrlResolver
		{
			public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
			{
				throw new XmlException("Can't resolve URI reference.", null);
			}
		}

		private RdlElementStack m_rdlElementStack;

		protected List<string> m_validationNamespaceList;

		protected XmlReader m_reader;

		protected static XmlSchemaContentProcessing m_processContent;

		internal int LineNumber
		{
			get
			{
				IXmlLineInfo xmlLineInfo = this.m_reader as IXmlLineInfo;
				if (xmlLineInfo != null)
				{
					return xmlLineInfo.LineNumber;
				}
				return 0;
			}
		}

		internal int LinePosition
		{
			get
			{
				IXmlLineInfo xmlLineInfo = this.m_reader as IXmlLineInfo;
				if (xmlLineInfo != null)
				{
					return xmlLineInfo.LinePosition;
				}
				return 0;
			}
		}

		public override XmlReaderSettings Settings
		{
			get
			{
				return this.m_reader.Settings;
			}
		}

		public override int AttributeCount
		{
			get
			{
				return this.m_reader.AttributeCount;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.m_reader.BaseURI;
			}
		}

		public override int Depth
		{
			get
			{
				return this.m_reader.Depth;
			}
		}

		public override bool EOF
		{
			get
			{
				return this.m_reader.EOF;
			}
		}

		public override bool HasValue
		{
			get
			{
				return this.m_reader.HasValue;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return this.m_reader.IsEmptyElement;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.m_reader.LocalName;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.m_reader.NameTable;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return this.m_reader.NamespaceURI;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return this.m_reader.NodeType;
			}
		}

		public override string Prefix
		{
			get
			{
				return this.m_reader.Prefix;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				return this.m_reader.ReadState;
			}
		}

		public override string Value
		{
			get
			{
				return this.m_reader.Value;
			}
		}

		public event ValidationEventHandler ValidationEventHandler;

		internal RDLValidatingReader(Stream stream, List<Pair<string, Stream>> namespaceSchemaStreamMap)
		{
			try
			{
				this.m_validationNamespaceList = new List<string>();
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				foreach (Pair<string, Stream> item in namespaceSchemaStreamMap)
				{
					Pair<string, Stream> current = item;
					this.m_validationNamespaceList.Add(current.First);
					xmlReaderSettings.Schemas.Add(current.First, XmlReader.Create(current.Second));
				}
				xmlReaderSettings.ValidationType = ValidationType.Schema;
				xmlReaderSettings.ValidationEventHandler += this.ValidationCallBack;
				xmlReaderSettings.DtdProcessing = DtdProcessing.Prohibit;
				xmlReaderSettings.CloseInput = true;
				xmlReaderSettings.XmlResolver = new XmlNullResolver();
				this.m_reader = XmlReader.Create(stream, xmlReaderSettings);
			}
			catch (SynchronizationLockException innerException)
			{
				throw new ReportProcessingException(RPRes.rsProcessingAbortedByError, ErrorCode.rsProcessingError, innerException);
			}
		}

		private static int CompareWithInvariantCulture(string x, string y, bool ignoreCase)
		{
			return string.Compare(x, y, (StringComparison)(ignoreCase ? 5 : 4));
		}

		public bool Validate(out string message)
		{
			message = null;
			if (!ListUtils.ContainsWithOrdinalComparer(this.m_reader.NamespaceURI, this.m_validationNamespaceList))
			{
				return true;
			}
			XmlSchemaComplexType xmlSchemaComplexType = null;
			bool result = true;
			ArrayList arrayList = new ArrayList();
			switch (this.m_reader.NodeType)
			{
			case XmlNodeType.Element:
				if (this.m_rdlElementStack == null)
				{
					this.m_rdlElementStack = new RdlElementStack();
				}
				xmlSchemaComplexType = (this.m_reader.SchemaInfo.SchemaType as XmlSchemaComplexType);
				if (xmlSchemaComplexType != null)
				{
					RDLValidatingReader.TraverseParticle(xmlSchemaComplexType.ContentTypeParticle, arrayList);
				}
				if (!this.m_reader.IsEmptyElement)
				{
					if (xmlSchemaComplexType != null && 1 < arrayList.Count && RDLValidatingReader.CompareWithInvariantCulture("ReportItemsType", xmlSchemaComplexType.Name, false) != 0 && RDLValidatingReader.CompareWithInvariantCulture("MapLayersType", xmlSchemaComplexType.Name, false) != 0)
					{
						Hashtable hashtable2 = new Hashtable(arrayList.Count);
						hashtable2.Add("_ParentName", this.m_reader.LocalName);
						hashtable2.Add("_Type", xmlSchemaComplexType);
						this.m_rdlElementStack.Add(hashtable2);
					}
					else
					{
						this.m_rdlElementStack.Add(null);
					}
				}
				else if (xmlSchemaComplexType != null)
				{
					for (int j = 0; j < arrayList.Count; j++)
					{
						XmlSchemaElement xmlSchemaElement2 = arrayList[j] as XmlSchemaElement;
						if (xmlSchemaElement2.MinOccurs > 0m)
						{
							result = false;
							message = RDLValidatingReaderStrings.rdlValidationMissingChildElement(this.m_reader.LocalName, xmlSchemaElement2.Name, this.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), this.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
						}
					}
				}
				if (0 < this.m_reader.Depth && this.m_rdlElementStack != null)
				{
					Hashtable hashtable3 = this.m_rdlElementStack[this.m_reader.Depth - 1];
					if (hashtable3 != null)
					{
						string text = (string)hashtable3[this.m_reader.LocalName];
						if (text == null)
						{
							hashtable3.Add(this.m_reader.LocalName, this.m_reader.NamespaceURI);
						}
						else if (RDLValidatingReader.CompareWithInvariantCulture(text, this.m_reader.NamespaceURI, false) == 0)
						{
							result = false;
							message = RDLValidatingReaderStrings.rdlValidationInvalidElement(hashtable3["_ParentName"] as string, this.m_reader.LocalName, this.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), this.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
						}
						else
						{
							string key = this.m_reader.LocalName + "$" + this.m_reader.NamespaceURI;
							if (hashtable3.ContainsKey(key))
							{
								result = false;
								message = RDLValidatingReaderStrings.rdlValidationInvalidElement(hashtable3["_ParentName"] as string, this.m_reader.LocalName, this.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), this.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
							}
							else
							{
								hashtable3.Add(key, this.m_reader.LocalName);
							}
						}
					}
				}
				break;
			case XmlNodeType.EndElement:
				if (this.m_rdlElementStack != null)
				{
					Hashtable hashtable = this.m_rdlElementStack[this.m_rdlElementStack.Count - 1];
					if (hashtable != null)
					{
						xmlSchemaComplexType = (hashtable["_Type"] as XmlSchemaComplexType);
						RDLValidatingReader.TraverseParticle(xmlSchemaComplexType.ContentTypeParticle, arrayList);
						for (int i = 0; i < arrayList.Count; i++)
						{
							XmlSchemaElement xmlSchemaElement = arrayList[i] as XmlSchemaElement;
							if (xmlSchemaElement.MinOccurs > 0m && !hashtable.ContainsKey(xmlSchemaElement.Name))
							{
								result = false;
								message = RDLValidatingReaderStrings.rdlValidationMissingChildElement(this.m_reader.LocalName, xmlSchemaElement.Name, this.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), this.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
							}
						}
						this.m_rdlElementStack[this.m_rdlElementStack.Count - 1] = null;
					}
					this.m_rdlElementStack.RemoveAt(this.m_rdlElementStack.Count - 1);
				}
				break;
			}
			return result;
		}

		private static void TraverseParticle(XmlSchemaParticle particle, ArrayList elementDeclsInContentModel)
		{
			if (particle is XmlSchemaElement)
			{
				XmlSchemaElement value = particle as XmlSchemaElement;
				elementDeclsInContentModel.Add(value);
			}
			else if (particle is XmlSchemaGroupBase)
			{
				XmlSchemaGroupBase xmlSchemaGroupBase = particle as XmlSchemaGroupBase;
				XmlSchemaObjectEnumerator enumerator = xmlSchemaGroupBase.Items.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						XmlSchemaParticle particle2 = (XmlSchemaParticle)enumerator.Current;
						RDLValidatingReader.TraverseParticle(particle2, elementDeclsInContentModel);
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
			else if (particle is XmlSchemaAny)
			{
				XmlSchemaAny xmlSchemaAny = particle as XmlSchemaAny;
				RDLValidatingReader.m_processContent = xmlSchemaAny.ProcessContents;
			}
		}

		private void ValidationCallBack(object sender, ValidationEventArgs args)
		{
			if (this.ValidationEventHandler != null)
			{
				this.ValidationEventHandler(sender, args);
			}
		}

		public override void Close()
		{
			this.m_reader.Close();
		}

		public override string GetAttribute(int i)
		{
			return this.m_reader.GetAttribute(i);
		}

		public override string GetAttribute(string name, string namespaceURI)
		{
			return this.m_reader.GetAttribute(name, namespaceURI);
		}

		public override string GetAttribute(string name)
		{
			return this.m_reader.GetAttribute(name);
		}

		internal string GetAttributeLocalName(string name)
		{
			string result = null;
			if (this.m_reader.HasAttributes)
			{
				while (this.m_reader.MoveToNextAttribute())
				{
					if (!this.m_reader.LocalName.Equals(name, StringComparison.Ordinal))
					{
						continue;
					}
					result = this.m_reader.Value;
					break;
				}
				this.m_reader.MoveToElement();
			}
			return result;
		}

		public override string LookupNamespace(string prefix)
		{
			return this.m_reader.LookupNamespace(prefix);
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			return this.m_reader.MoveToAttribute(name, ns);
		}

		public override bool MoveToAttribute(string name)
		{
			return this.m_reader.MoveToAttribute(name);
		}

		public override bool MoveToElement()
		{
			return this.m_reader.MoveToElement();
		}

		public override bool MoveToFirstAttribute()
		{
			return this.m_reader.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			return this.m_reader.MoveToNextAttribute();
		}

		public override bool Read()
		{
			return this.m_reader.Read();
		}

		public override bool ReadAttributeValue()
		{
			return this.m_reader.ReadAttributeValue();
		}

		public override void ResolveEntity()
		{
			this.m_reader.ResolveEntity();
		}
	}
}
