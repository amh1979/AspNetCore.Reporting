using System;
using System.Collections;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal class RDLValidatingReader : XmlValidatingReader
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
				throw new XmlException("Can't resolve URI reference ", null);
			}
		}

		private RdlElementStack m_rdlElementStack;

		private string m_validationNamespace;

		public RDLValidatingReader(XmlReader xmlReader, string validationNamespace)
			: base(xmlReader)
		{
			this.m_validationNamespace = validationNamespace;
			base.XmlResolver = new XmlNullResolver();
		}

		private static int CompareWithInvariantCulture(string x, string y, bool ignoreCase)
		{
			return string.Compare(x, y, (StringComparison)(ignoreCase ? 5 : 4));
		}

		public bool Validate(out string message)
		{
			message = null;
			if (RDLValidatingReader.CompareWithInvariantCulture(this.m_validationNamespace, base.NamespaceURI, false) != 0)
			{
				return true;
			}
			XmlSchemaComplexType xmlSchemaComplexType = null;
			bool result = true;
			ArrayList arrayList = new ArrayList();
			switch (base.NodeType)
			{
			case XmlNodeType.Element:
				if (this.m_rdlElementStack == null)
				{
					this.m_rdlElementStack = new RdlElementStack();
				}
				xmlSchemaComplexType = (base.SchemaType as XmlSchemaComplexType);
				if (xmlSchemaComplexType != null)
				{
					RDLValidatingReader.TraverseParticle(xmlSchemaComplexType.ContentTypeParticle, arrayList);
				}
				if (!base.IsEmptyElement)
				{
					if (xmlSchemaComplexType != null && 1 < arrayList.Count && RDLValidatingReader.CompareWithInvariantCulture("ReportItemsType", xmlSchemaComplexType.Name, false) != 0 && RDLValidatingReader.CompareWithInvariantCulture("MapLayersType", xmlSchemaComplexType.Name, false) != 0)
					{
						Hashtable hashtable2 = new Hashtable(arrayList.Count);
						hashtable2.Add("_ParentName", base.LocalName);
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
							message = RDLValidatingReaderStrings.rdlValidationMissingChildElement(base.LocalName, xmlSchemaElement2.Name, base.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), base.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
						}
					}
				}
				if (0 < base.Depth && this.m_rdlElementStack != null)
				{
					Hashtable hashtable3 = this.m_rdlElementStack[base.Depth - 1];
					if (hashtable3 != null)
					{
						if (hashtable3.ContainsKey(base.LocalName))
						{
							result = false;
							message = RDLValidatingReaderStrings.rdlValidationInvalidElement(hashtable3["_ParentName"] as string, base.LocalName, base.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), base.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
						}
						else
						{
							hashtable3.Add(base.LocalName, null);
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
								message = RDLValidatingReaderStrings.rdlValidationMissingChildElement(base.LocalName, xmlSchemaElement.Name, base.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), base.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
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
		}
	}
}
