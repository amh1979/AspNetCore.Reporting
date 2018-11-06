using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class RdlValidator
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
		}

		private const string MustUnderstandAttributeName = "MustUnderstand";

		private readonly XmlReader m_reader;

		private RdlElementStack m_rdlElementStack;

		private readonly HashSet<string> m_validationNamespaces;

		public RdlValidator(XmlReader xmlReader, IEnumerable<string> validationNamespaces)
		{
			this.m_reader = xmlReader;
			this.m_validationNamespaces = new HashSet<string>(validationNamespaces);
		}

		public bool ValidateStartElement(out string message)
		{
			message = null;
			XmlSchemaComplexType xmlSchemaComplexType = null;
			ArrayList arrayList = null;
			if (this.m_rdlElementStack == null)
			{
				this.m_rdlElementStack = new RdlElementStack();
			}
			if (this.m_reader.SchemaInfo != null && this.m_validationNamespaces.Contains(this.m_reader.NamespaceURI))
			{
				xmlSchemaComplexType = (this.m_reader.SchemaInfo.SchemaType as XmlSchemaComplexType);
			}
			if (xmlSchemaComplexType != null)
			{
				arrayList = new ArrayList();
				RdlValidator.TraverseParticle(xmlSchemaComplexType.ContentTypeParticle, arrayList);
			}
			if (xmlSchemaComplexType != null && 1 < arrayList.Count && "MapLayersType" != xmlSchemaComplexType.Name && "ReportItemsType" != xmlSchemaComplexType.Name)
			{
				Hashtable hashtable = new Hashtable(arrayList.Count);
				hashtable.Add("_ParentName", this.m_reader.LocalName);
				hashtable.Add("_Type", xmlSchemaComplexType);
				this.m_rdlElementStack.Add(hashtable);
			}
			else
			{
				this.m_rdlElementStack.Add(null);
			}
			if (0 < this.m_reader.Depth && this.m_rdlElementStack != null)
			{
				Hashtable hashtable2 = this.m_rdlElementStack[this.m_reader.Depth - 1];
				if (hashtable2 != null)
				{
					if (hashtable2.ContainsKey(this.m_reader.LocalName))
					{
						message = this.ValidationMessage("rdlValidationInvalidElement", (string)hashtable2["_ParentName"], this.m_reader.LocalName);
						return false;
					}
					hashtable2.Add(this.m_reader.LocalName, null);
				}
			}
			string text = (this.m_reader.GetAttribute("MustUnderstand") ?? string.Empty).Trim();
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split();
				foreach (string text2 in array)
				{
					string text3 = this.m_reader.LookupNamespace(text2);
					if (!this.m_validationNamespaces.Contains(text3))
					{
						int num = 0;
						int num2 = 0;
						IXmlLineInfo xmlLineInfo = (IXmlLineInfo)this.m_reader;
						num = xmlLineInfo.LineNumber;
						num2 = xmlLineInfo.LinePosition;
						message = RDLValidatingReaderStrings.rdlValidationUnknownRequiredNamespaces(text2, text3, "Microsoft SQL Server 2017", num.ToString(CultureInfo.InvariantCulture.NumberFormat), num2.ToString(CultureInfo.InvariantCulture.NumberFormat));
						return false;
					}
				}
			}
			return true;
		}

		public bool ValidateEndElement(out string message)
		{
			XmlSchemaComplexType xmlSchemaComplexType = null;
			message = null;
			bool result = true;
			if (this.m_rdlElementStack != null)
			{
				Hashtable hashtable = this.m_rdlElementStack[this.m_rdlElementStack.Count - 1];
				if (hashtable != null)
				{
					xmlSchemaComplexType = (hashtable["_Type"] as XmlSchemaComplexType);
					ArrayList arrayList = new ArrayList();
					RdlValidator.TraverseParticle(xmlSchemaComplexType.ContentTypeParticle, arrayList);
					for (int i = 0; i < arrayList.Count; i++)
					{
						XmlSchemaElement xmlSchemaElement = arrayList[i] as XmlSchemaElement;
						if (xmlSchemaElement.MinOccurs > 0m && !hashtable.ContainsKey(xmlSchemaElement.Name))
						{
							result = false;
							message = this.ValidationMessage("rdlValidationMissingChildElement", hashtable["_ParentName"] as string, xmlSchemaElement.Name);
						}
					}
					this.m_rdlElementStack[this.m_rdlElementStack.Count - 1] = null;
				}
				this.m_rdlElementStack.RemoveAt(this.m_rdlElementStack.Count - 1);
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
						RdlValidator.TraverseParticle(particle2, elementDeclsInContentModel);
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

		private string ValidationMessage(string id, string parentType, string childType)
		{
			int num = 0;
			int num2 = 0;
			IXmlLineInfo xmlLineInfo = this.m_reader as IXmlLineInfo;
			if (xmlLineInfo != null)
			{
				num = xmlLineInfo.LineNumber;
				num2 = xmlLineInfo.LinePosition;
			}
			return RDLValidatingReaderStrings.Keys.GetString(id, parentType, childType, num.ToString(CultureInfo.InvariantCulture.NumberFormat), num2.ToString(CultureInfo.InvariantCulture.NumberFormat));
		}
	}
}
