using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.Common
{
	internal class ScopeID : IXmlSerializable
	{
		internal const string SCOPEID = "ScopeID";

		internal const string SCOPEVALUES = "ScopeValues";

		private ScopeValue[] m_scopeValues;

		public int ScopeValueCount
		{
			get
			{
				if (this.m_scopeValues != null)
				{
					return this.m_scopeValues.Length;
				}
				return 0;
			}
		}

		public IEnumerable<ScopeValue> InstanceID
		{
			get
			{
				int count = this.ScopeValueCount;
				for (int cursor = 0; cursor < count; cursor++)
				{
					ScopeValue val = this.m_scopeValues[cursor];
					if (val.IsGroupExprValue)
					{
						yield return val;
					}
				}
			}
		}

		public IEnumerable<ScopeValue> QueryRestartPosition
		{
			get
			{
				int count = this.ScopeValueCount;
				for (int cursor = 0; cursor < count; cursor++)
				{
					ScopeValue val = this.m_scopeValues[cursor];
					if (val.IsSortExprValue)
					{
						yield return val;
					}
				}
			}
		}

		internal ScopeID()
		{
		}

		internal ScopeID(ScopeValue[] scopeValues)
		{
			this.m_scopeValues = scopeValues;
		}

		internal ScopeID(ScopeID scopeID)
			: this(scopeID.m_scopeValues)
		{
		}

		public ScopeValue GetScopeValue(int index)
		{
			return this.m_scopeValues[index];
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{ ");
			string value = string.Empty;
			int scopeValueCount = this.ScopeValueCount;
			for (int i = 0; i < scopeValueCount; i++)
			{
				stringBuilder.Append(value);
				stringBuilder.Append(Convert.ToString(this.m_scopeValues[i].Value, CultureInfo.InvariantCulture));
				stringBuilder.Append("[");
				stringBuilder.Append(this.m_scopeValues[i].ScopeType.ToString());
				stringBuilder.Append("]");
				value = ", ";
			}
			stringBuilder.Append(" }");
			return stringBuilder.ToString();
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ScopeID);
		}

		internal bool Equals(ScopeID scopeID)
		{
			if (object.ReferenceEquals(this, scopeID))
			{
				return true;
			}
			if (object.ReferenceEquals(scopeID, null))
			{
				return false;
			}
			return this.Equals(scopeID, null);
		}

		internal bool Equals(ScopeID scopeID, IEqualityComparer<object> comparer)
		{
			if (object.ReferenceEquals(scopeID, null))
			{
				return false;
			}
			if (object.ReferenceEquals(this.m_scopeValues, scopeID.m_scopeValues))
			{
				return true;
			}
			int scopeValueCount = this.ScopeValueCount;
			if (scopeValueCount != scopeID.ScopeValueCount)
			{
				return false;
			}
			for (int i = 0; i < scopeValueCount; i++)
			{
				if (!this.m_scopeValues[i].Equals(scopeID.m_scopeValues[i], comparer))
				{
					return false;
				}
			}
			return true;
		}

		internal int Compare(ScopeID scopeID, IComparer<object> comparer)
		{
			int scopeValueCount = this.ScopeValueCount;
			if (scopeValueCount == 0)
			{
				if (!object.ReferenceEquals(scopeID, null) && scopeID.ScopeValueCount != 0)
				{
					return -1;
				}
				return 0;
			}
			if (!object.ReferenceEquals(scopeID, null) && scopeID.ScopeValueCount != 0)
			{
				if (scopeValueCount != scopeID.ScopeValueCount)
				{
					throw new ArgumentException();
				}
				for (int i = 0; i < scopeValueCount; i++)
				{
					int num = comparer.Compare(this.m_scopeValues[i].Value, scopeID.m_scopeValues[i].Value);
					if (num != 0)
					{
						return num;
					}
				}
				return 0;
			}
			return 1;
		}

		internal int GetHashCode(IEqualityComparer<object> comparer)
		{
			int scopeValueCount = this.ScopeValueCount;
			int num = scopeValueCount;
			for (int i = 0; i < scopeValueCount; i++)
			{
				num ^= this.m_scopeValues[i].GetHashCode(comparer);
			}
			return num;
		}

		public static bool operator ==(ScopeID scopeID1, ScopeID scopeID2)
		{
			if (object.ReferenceEquals(scopeID1, scopeID2))
			{
				return true;
			}
			if (object.ReferenceEquals(scopeID1, null))
			{
				return false;
			}
			return scopeID1.Equals(scopeID2);
		}

		public static bool operator !=(ScopeID scopeID1, ScopeID scopeID2)
		{
			return !(scopeID1 == scopeID2);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public virtual XmlSchema GetSchema()
		{
			throw new NotImplementedException();
		}

		public virtual void ReadXml(XmlReader xmlReader)
		{
			while (xmlReader.Read())
			{
				if (xmlReader.NodeType == XmlNodeType.Element)
				{
					this.ReadXmlElement(xmlReader);
				}
			}
		}

		protected virtual void ReadXmlElement(XmlReader xmlReader)
		{
			if (!(xmlReader.Name != "ScopeValues"))
			{
				List<ScopeValue> list = new List<ScopeValue>();
				using (XmlReader xmlReader2 = xmlReader.ReadSubtree())
				{
					while (xmlReader2.Read())
					{
						if (xmlReader2.NodeType == XmlNodeType.Element && xmlReader2.Name == "ScopeValue")
						{
							using (XmlReader xmlReader3 = xmlReader.ReadSubtree())
							{
								ScopeValue scopeValue = new ScopeValue();
								scopeValue.ReadXml(xmlReader3);
								list.Add(scopeValue);
							}
						}
					}
				}
				if (list.Count > 0)
				{
					this.m_scopeValues = list.ToArray();
				}
				else
				{
					this.m_scopeValues = null;
				}
			}
		}

		public virtual void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("ScopeID");
			this.WriteBaseXml(writer);
			writer.WriteEndElement();
		}

		protected void WriteBaseXml(XmlWriter writer)
		{
			writer.WriteStartElement("ScopeValues");
			ScopeValue[] scopeValues = this.m_scopeValues;
			foreach (ScopeValue scopeValue in scopeValues)
			{
				scopeValue.WriteXml(writer);
			}
			writer.WriteEndElement();
		}
	}
}
