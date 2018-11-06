using System;
using System.Collections.Generic;
using System.Xml;

namespace AspNetCore.ReportingServices.Common
{
	internal sealed class ScopeValue : SerializableValue
	{
		internal const string SCOPEVALUE = "ScopeValue";

		internal const string SCOPETYPE = "ScopeType";

		private readonly string m_key;

		private ScopeIDType m_scopeType = ScopeIDType.GroupValues;

		internal ScopeIDType ScopeType
		{
			get
			{
				return this.m_scopeType;
			}
		}

		internal string Key
		{
			get
			{
				return this.m_key;
			}
		}

		internal bool IsGroupExprValue
		{
			get
			{
				if (this.m_scopeType != ScopeIDType.GroupValues)
				{
					return this.m_scopeType == ScopeIDType.SortGroup;
				}
				return true;
			}
		}

		internal bool IsSortExprValue
		{
			get
			{
				if (this.m_scopeType != ScopeIDType.SortValues)
				{
					return this.m_scopeType == ScopeIDType.SortGroup;
				}
				return true;
			}
		}

		internal ScopeValue()
		{
		}

		internal ScopeValue(object value, ScopeIDType scopeType)
			: base(value)
		{
			this.m_scopeType = scopeType;
		}

		internal ScopeValue(object value, ScopeIDType scopeType, string key)
			: this(value, scopeType)
		{
			this.m_key = key;
		}

		internal ScopeValue(object value, ScopeIDType scopeType, DataTypeCode dataTypeCode)
			: base(value, dataTypeCode)
		{
			this.m_scopeType = scopeType;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ScopeValue);
		}

		internal bool Equals(ScopeValue scopeValue)
		{
			if (object.ReferenceEquals(this, scopeValue))
			{
				return true;
			}
			if (object.ReferenceEquals(scopeValue, null))
			{
				return false;
			}
			return this.Equals(scopeValue, null);
		}

		public static bool operator ==(ScopeValue scopeValue1, ScopeValue scopeValue2)
		{
			if (object.ReferenceEquals(scopeValue1, scopeValue2))
			{
				return true;
			}
			if (object.ReferenceEquals(scopeValue1, null))
			{
				return false;
			}
			return scopeValue1.Equals(scopeValue2);
		}

		public static bool operator !=(ScopeValue scopeValue1, ScopeValue scopeValue2)
		{
			return !(scopeValue1 == scopeValue2);
		}

		internal bool Equals(ScopeValue scopeValue, IEqualityComparer<object> comparer)
		{
			if (object.ReferenceEquals(scopeValue, null))
			{
				return false;
			}
			if (this.ScopeType != scopeValue.ScopeType)
			{
				return false;
			}
			if (comparer == null)
			{
				return ObjectSerializer.Equals(base.Value, scopeValue.Value, base.TypeCode, scopeValue.TypeCode);
			}
			return comparer.Equals(base.Value, scopeValue.Value);
		}

		internal int GetHashCode(IEqualityComparer<object> comparer)
		{
			int num = 0;
			if (base.Value != null)
			{
				num = comparer.GetHashCode(base.Value);
				num <<= 20;
			}
			return num | ((int)this.ScopeType << 8 & (int)base.TypeCode);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		protected override void ReadDerivedXml(XmlReader xmlReader)
		{
			XmlNodeType nodeType = xmlReader.NodeType;
			string name;
			if (nodeType == XmlNodeType.Element && (name = xmlReader.Name) != null && name == "ScopeValue")
			{
				this.ReadAttributes(xmlReader);
			}
		}

		private void ReadAttributes(XmlReader xmlReader)
		{
			for (int i = 0; i < xmlReader.AttributeCount; i++)
			{
				xmlReader.MoveToAttribute(i);
				string name;
				if ((name = xmlReader.Name) != null && name == "ScopeType")
				{
					this.m_scopeType = (ScopeIDType)Enum.Parse(typeof(ScopeIDType), xmlReader.Value, false);
				}
			}
		}

		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("ScopeValue");
			writer.WriteAttributeString("ScopeType", this.m_scopeType.ToString());
			base.WriteBaseXml(writer);
			writer.WriteEndElement();
		}
	}
}
