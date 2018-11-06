using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class StyleProperties : ICloneable
	{
		private Hashtable m_nameMap;

		private ArrayList m_valueCollection;

		public object this[int index]
		{
			get
			{
				return this.m_valueCollection[index];
			}
		}

		public object this[string styleName]
		{
			get
			{
				object obj = this.m_nameMap[styleName];
				if (obj != null)
				{
					return this.m_valueCollection[(int)obj];
				}
				return null;
			}
		}

		public int Count
		{
			get
			{
				return this.m_valueCollection.Count;
			}
		}

		public ICollection Keys
		{
			get
			{
				return this.m_nameMap.Keys;
			}
		}

		internal StyleProperties()
		{
			this.m_nameMap = new Hashtable();
			this.m_valueCollection = new ArrayList();
		}

		internal StyleProperties(int capacity)
		{
			this.m_nameMap = new Hashtable(capacity);
			this.m_valueCollection = new ArrayList(capacity);
		}

		public bool ContainStyleProperty(string styleName)
		{
			if (styleName != null && this.m_nameMap.Count != 0)
			{
				return this.m_nameMap.ContainsKey(styleName);
			}
			return false;
		}

		internal void Add(string name, object value)
		{
			if (!this.m_nameMap.Contains(name))
			{
				this.m_nameMap.Add(name, this.m_valueCollection.Count);
				this.m_valueCollection.Add(value);
			}
		}

		internal void Set(string name, object value)
		{
			object obj = this.m_nameMap[name];
			if (obj != null)
			{
				this.m_valueCollection[(int)obj] = value;
			}
			else
			{
				this.m_nameMap.Add(name, this.m_valueCollection.Count);
				this.m_valueCollection.Add(value);
			}
		}

		internal DataValueInstanceList ExtractRenderStyles()
		{
			DataValueInstanceList dataValueInstanceList = null;
			if (this.m_nameMap != null)
			{
				Global.Tracer.Assert(this.m_valueCollection != null && this.m_nameMap.Count == this.m_valueCollection.Count);
				dataValueInstanceList = new DataValueInstanceList(this.m_nameMap.Count);
				IDictionaryEnumerator enumerator = this.m_nameMap.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DataValueInstance dataValueInstance = new DataValueInstance();
					dataValueInstance.Name = (enumerator.Key as string);
					object obj = this.m_valueCollection[(int)enumerator.Value];
					if (obj == null)
					{
						dataValueInstance.Value = null;
					}
					else if (obj is string)
					{
						dataValueInstance.Value = string.Copy(obj as string);
					}
					else if (obj is int)
					{
						dataValueInstance.Value = (int)obj;
					}
					else if (obj is ReportSize)
					{
						dataValueInstance.Value = ((ReportSize)obj).DeepClone();
					}
					dataValueInstanceList.Add(dataValueInstance);
				}
			}
			return dataValueInstanceList;
		}

		public object Clone()
		{
			StyleProperties styleProperties = (StyleProperties)base.MemberwiseClone();
			if (this.m_nameMap != null)
			{
				styleProperties.m_nameMap = new Hashtable(this.m_nameMap.Count);
				styleProperties.m_valueCollection = new ArrayList(this.m_valueCollection.Count);
				IDictionaryEnumerator enumerator = this.m_nameMap.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
						styleProperties.m_nameMap.Add((string)dictionaryEntry.Key, dictionaryEntry.Value);
						object obj = this.m_valueCollection[(int)dictionaryEntry.Value];
						object value = null;
						if (obj is string)
						{
							value = string.Copy(obj as string);
						}
						else if (obj is int)
						{
							value = (int)obj;
						}
						else if (obj is ReportSize)
						{
							value = ((ReportSize)obj).DeepClone();
						}
						styleProperties.m_valueCollection[(int)dictionaryEntry.Value] = value;
					}
					return styleProperties;
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
			return styleProperties;
		}
	}
}
