using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Xml;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class ParamValues : Hashtable
	{
		private const string _ParameterValues = "ParameterValues";

		internal Hashtable m_fields = new Hashtable();

		public ParamValueList this[string name]
		{
			get
			{
				return (ParamValueList)base[name];
			}
		}

		public Hashtable Fields
		{
			get
			{
				return this.m_fields;
			}
		}

		public string[] FieldKeys
		{
			get
			{
				string[] array = new string[this.m_fields.Keys.Count];
				int num = 0;
				foreach (string key in this.m_fields.Keys)
				{
					array[num++] = key;
				}
				return array;
			}
		}

		public NameValueCollection AsNameValueCollection
		{
			get
			{
				int count = this.Count;
				NameValueCollection nameValueCollection = new NameValueCollection(count, StringComparer.CurrentCulture);
				foreach (ParamValueList value in this.Values)
				{
					for (int i = 0; i < value.Count; i++)
					{
						nameValueCollection.Add(value[i].Name, value[i].Value);
					}
				}
				return nameValueCollection;
			}
		}

		internal void AddField(string fieldName)
		{
			if (this.m_fields[fieldName] == null)
			{
				this.m_fields.Add(fieldName, fieldName);
			}
		}

		internal string GetFieldValue(string fieldName)
		{
			return (string)this.m_fields[fieldName];
		}

		internal void AddFieldValue(string fieldName, string fieldValue)
		{
			if (this.m_fields[fieldName] != null)
			{
				this.m_fields[fieldName] = fieldValue;
			}
		}

		public void FromXml(string xml)
		{
			if (xml != null && !(xml == ""))
			{
				XmlReader xmlReader = XmlUtil.SafeCreateXmlTextReader(xml);
				try
				{
					if (xmlReader.Read() && !(xmlReader.Name != "ParameterValues"))
					{
						while (true)
						{
							if (xmlReader.Read())
							{
								if (xmlReader.IsStartElement("ParameterValue"))
								{
									ParamValue paramValue = new ParamValue(xmlReader, this);
									bool flag = false;
									ParamValueList paramValueList = this[paramValue.Name];
									if (paramValueList == null)
									{
										flag = true;
										paramValueList = new ParamValueList();
									}
									paramValueList.Add(paramValue);
									if (flag)
									{
										this.Add(paramValue.Name, paramValueList);
									}
								}
								else if (xmlReader.NodeType == XmlNodeType.Element)
								{
									break;
								}
								continue;
							}
							return;
						}
						throw new InvalidXmlException();
					}
					throw new InvalidXmlException();
				}
				catch (XmlException ex)
				{
					throw new MalformedXmlException(ex);
				}
			}
		}

		public string ToXml(bool outputFieldElements)
		{
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			try
			{
				xmlTextWriter.WriteStartElement("ParameterValues");
				foreach (ParamValueList value in this.Values)
				{
					for (int i = 0; i < value.Count; i++)
					{
						value[i].ToXml(xmlTextWriter, outputFieldElements);
					}
				}
				xmlTextWriter.WriteEndElement();
				return stringWriter.ToString();
			}
			finally
			{
				xmlTextWriter.Close();
				stringWriter.Close();
			}
		}

		public string ToOldParameterXml()
		{
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			try
			{
				xmlTextWriter.WriteStartElement("Parameters");
				foreach (ParamValueList value in this.Values)
				{
					for (int i = 0; i < value.Count; i++)
					{
						value[i].ToOldParameterXml(xmlTextWriter);
					}
				}
				xmlTextWriter.WriteEndElement();
				return stringWriter.ToString();
			}
			finally
			{
				xmlTextWriter.Close();
				stringWriter.Close();
			}
		}
	}
}
