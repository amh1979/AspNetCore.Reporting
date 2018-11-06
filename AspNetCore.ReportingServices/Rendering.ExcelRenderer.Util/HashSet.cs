using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Util
{
	internal sealed class HashSet<T> : ICollection<T>, IEnumerable<T>, IEnumerable
	{
		private Dictionary<T, T> mHashTable = new Dictionary<T, T>();

		public int Count
		{
			get
			{
				return this.mHashTable.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public void Add(T item)
		{
			if (!this.mHashTable.ContainsKey(item))
			{
				this.mHashTable.Add(item, item);
			}
		}

		public void Clear()
		{
			this.mHashTable.Clear();
		}

		public bool Contains(T item)
		{
			return this.mHashTable.ContainsKey(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			foreach (KeyValuePair<T, T> item in this.mHashTable)
			{
				if (arrayIndex >= this.mHashTable.Count)
				{
					break;
				}
				if (arrayIndex >= array.Length)
				{
					break;
				}
				array[arrayIndex] = item.Value;
				arrayIndex++;
			}
		}

		public bool Remove(T item)
		{
			return this.mHashTable.Remove(item);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return (IEnumerator<T>)(object)this.mHashTable.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.mHashTable.Values).GetEnumerator();
		}
	}
}
