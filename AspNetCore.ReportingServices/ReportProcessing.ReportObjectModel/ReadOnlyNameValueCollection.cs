using System;
using System.Collections;
using System.Collections.Specialized;

namespace AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel
{
	public sealed class ReadOnlyNameValueCollection : MarshalByRefObject
	{
		private NameValueCollection m_originalCollection;

		public string[] AllKeys
		{
			get
			{
				return this.m_originalCollection.AllKeys;
			}
		}

		public string[] AllValues
		{
			get
			{
				int count = this.m_originalCollection.Count;
				string[] array = new string[count];
				if (count > 0)
				{
					this.m_originalCollection.CopyTo(array, 0);
				}
				return array;
			}
		}

		public string this[int index]
		{
			get
			{
				return this.m_originalCollection[index];
			}
		}

		public string this[string name]
		{
			get
			{
				return this.m_originalCollection[name];
			}
		}

		public int Count
		{
			get
			{
				return this.m_originalCollection.Count;
			}
		}

		public NameObjectCollectionBase.KeysCollection Keys
		{
			get
			{
				return this.m_originalCollection.Keys;
			}
		}

		internal ReadOnlyNameValueCollection(NameValueCollection originalCollection)
		{
			if (originalCollection == null)
			{
				throw new ArgumentNullException("originalCollection");
			}
			this.m_originalCollection = originalCollection;
		}

		public void CopyTo(Array dest, int index)
		{
			this.m_originalCollection.CopyTo(dest, index);
		}

		public string Get(int index)
		{
			return this.m_originalCollection.Get(index);
		}

		public string Get(string name)
		{
			return this.m_originalCollection.Get(name);
		}

		public string GetKey(int index)
		{
			return this.m_originalCollection.GetKey(index);
		}

		public string[] GetValues(int index)
		{
			return this.m_originalCollection.GetValues(index);
		}

		public string[] GetValues(string name)
		{
			return this.m_originalCollection.GetValues(name);
		}

		public bool HasKeys()
		{
			return this.m_originalCollection.HasKeys();
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_originalCollection.GetEnumerator();
		}
	}
}
