using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlObjectModel.Serialization
{
	internal class NameTable<T>
	{
		private Hashtable table = new Hashtable();

		public IEnumerable<NameKey> Keys
		{
			get
			{
				foreach (object key in this.table.Keys)
				{
					yield return (NameKey)key;
				}
			}
		}

		public IEnumerable<T> Values
		{
			get
			{
				foreach (object value in this.table.Values)
				{
					yield return (T)value;
				}
			}
		}

		public T this[string name, string ns]
		{
			get
			{
				return (T)this.table[new NameKey(name, ns)];
			}
			set
			{
				this.table[new NameKey(name, ns)] = value;
			}
		}

		public void Add(string name, string ns, T value)
		{
			NameKey key = new NameKey(name, ns);
			this.table.Add(key, value);
		}
	}
}
