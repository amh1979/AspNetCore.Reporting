using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class FieldPropertyHashtable
	{
		private Hashtable m_hashtable;

		internal int Count
		{
			get
			{
				return this.m_hashtable.Count;
			}
		}

		internal FieldPropertyHashtable()
		{
			this.m_hashtable = new Hashtable();
		}

		internal FieldPropertyHashtable(int capacity)
		{
			this.m_hashtable = new Hashtable(capacity);
		}

		internal void Add(string key)
		{
			this.m_hashtable.Add(key, null);
		}

		internal bool ContainsKey(string key)
		{
			return this.m_hashtable.ContainsKey(key);
		}

		internal IDictionaryEnumerator GetEnumerator()
		{
			return this.m_hashtable.GetEnumerator();
		}
	}
}
