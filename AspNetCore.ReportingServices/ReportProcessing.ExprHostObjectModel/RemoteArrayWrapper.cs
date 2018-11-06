using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	[CLSCompliant(false)]
	public sealed class RemoteArrayWrapper<T> : MarshalByRefObject, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		private readonly T[] m_array;

		public T this[int index]
		{
			get
			{
				return this.m_array[index];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public int Count
		{
			get
			{
				return this.m_array.Length;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public RemoteArrayWrapper(params T[] array)
		{
			this.m_array = array;
		}

		public int IndexOf(T item)
		{
			throw new NotSupportedException();
		}

		public void Insert(int index, T item)
		{
			throw new NotSupportedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		public void Add(T item)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(T item)
		{
			throw new NotSupportedException();
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.m_array.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			throw new NotSupportedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < this.m_array.Length; i++)
			{
				yield return (object)this.m_array[i];
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			for (int i = 0; i < this.m_array.Length; i++)
			{
				yield return this.m_array[i];
			}
		}
	}
}
