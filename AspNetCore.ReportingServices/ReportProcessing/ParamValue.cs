using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System.Xml;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class ParamValue
	{
		internal const string _ParameterValue = "ParameterValue";

		private const string _Name = "Name";

		private const string _Value = "Value";

		private const string _Field = "Field";

		private ParamValues m_parent;

		private string m_name;

		private string m_value;

		private string m_fieldName;

		public string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public string Value
		{
			get
			{
				return this.m_value;
			}
		}

		public string FieldName
		{
			get
			{
				return this.m_fieldName;
			}
		}

		public bool UseField
		{
			get
			{
				return this.FieldName != null;
			}
		}

		public bool UseValue
		{
			get
			{
				return !this.UseField;
			}
		}

		public string FieldValue
		{
			get
			{
				Global.Tracer.Assert(this.UseField, "Trying to get a field value for a non field setting.");
				return this.m_parent.GetFieldValue(this.FieldName);
			}
		}

		public bool IsValid()
		{
			if (this.UseField && this.Value != null)
			{
				return false;
			}
			return true;
		}

		public ParamValue(XmlReader reader, ParamValues parent)
		{
			this.m_parent = parent;
			while (reader.Read() && (reader.NodeType != XmlNodeType.EndElement || !(reader.Name == "ParameterValue")))
			{
				if (reader.IsStartElement("Name"))
				{
					this.m_name = reader.ReadString();
				}
				else if (reader.IsStartElement("Value"))
				{
					this.m_value = reader.ReadString();
				}
				else if (reader.IsStartElement("Field"))
				{
					this.m_fieldName = reader.ReadString();
					this.m_parent.AddField(this.FieldName);
				}
				else if (reader.NodeType == XmlNodeType.Element)
				{
					throw new InvalidXmlException();
				}
			}
			if (this.IsValid())
			{
				return;
			}
			throw new InvalidXmlException();
		}

		internal void ToXml(XmlTextWriter writer, bool outputFieldElements)
		{
			writer.WriteStartElement("ParameterValue");
			writer.WriteElementString("Name", this.Name);
			if (this.UseField)
			{
				if (outputFieldElements)
				{
					writer.WriteElementString("Field", this.FieldName);
				}
				else if (this.FieldValue != null)
				{
					writer.WriteElementString("Value", this.FieldValue);
				}
			}
			else if (this.Value != null)
			{
				writer.WriteElementString("Value", this.Value);
			}
			writer.WriteEndElement();
		}

		internal void ToOldParameterXml(XmlTextWriter writer)
		{
			writer.WriteStartElement("Parameter");
			writer.WriteElementString("Name", this.Name);
			if (this.Value != null)
			{
				writer.WriteElementString("Value", this.Value);
			}
			writer.WriteEndElement();
		}
	}
}
