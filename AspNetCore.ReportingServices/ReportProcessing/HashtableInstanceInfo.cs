using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal abstract class HashtableInstanceInfo : InstanceInfo
	{
		protected Hashtable m_hashtable;

		internal int Count
		{
			get
			{
				return this.m_hashtable.Count;
			}
		}

		protected HashtableInstanceInfo()
		{
			this.m_hashtable = new Hashtable();
		}

		protected HashtableInstanceInfo(int capacity)
		{
			this.m_hashtable = new Hashtable(capacity);
		}

		internal bool ContainsKey(int key)
		{
			return this.m_hashtable.ContainsKey(key);
		}

		internal IDictionaryEnumerator GetEnumerator()
		{
			return this.m_hashtable.GetEnumerator();
		}
	}
}
