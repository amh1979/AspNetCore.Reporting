using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class FunctionalList<T> : IEnumerable<T>, IEnumerable
	{
		private class FunctionalListEnumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			private FunctionalList<T> m_list;

			private FunctionalList<T> m_rest;

			private T m_item = default(T);

			public T Current
			{
				get
				{
					if (this.m_rest == null)
					{
						throw new InvalidOperationException("MoveNext must be called before calling Current");
					}
					return this.m_item;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public FunctionalListEnumerator(FunctionalList<T> aList)
			{
				this.m_list = aList;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				if (this.m_rest == null)
				{
					this.m_rest = this.m_list;
				}
				if (this.m_rest.Count > 0)
				{
					this.m_item = this.m_rest.First;
					this.m_rest = this.m_rest.Rest;
					return true;
				}
				return false;
			}

			public void Reset()
			{
				this.m_rest = null;
			}
		}

		private T m_car;

		private FunctionalList<T> m_cdr;

		private int m_size;

		private static FunctionalList<T> m_emptyList = new FunctionalList<T>();

		internal T First
		{
			get
			{
				return this.m_car;
			}
		}

		internal FunctionalList<T> Rest
		{
			get
			{
				return this.m_cdr;
			}
		}

		internal int Count
		{
			get
			{
				return this.m_size;
			}
		}

		internal static FunctionalList<T> Empty
		{
			get
			{
				return FunctionalList<T>.m_emptyList;
			}
		}

		private FunctionalList()
		{
		}

		private FunctionalList(T aItem, FunctionalList<T> aCdr)
		{
			this.m_car = aItem;
			this.m_cdr = aCdr;
			this.m_size = aCdr.Count + 1;
		}

		internal FunctionalList<T> Add(T aItem)
		{
			return new FunctionalList<T>(aItem, this);
		}

		internal bool IsEmpty()
		{
			return this.m_size == 0;
		}

		internal int IndexOf(T aItem)
		{
			if (this.Count == 0)
			{
				return -1;
			}
			if (object.Equals(this.First, aItem))
			{
				return this.m_size - 1;
			}
			return this.Rest.IndexOf(aItem);
		}

		internal bool Contains(T aItem)
		{
			return this.IndexOf(aItem) != -1;
		}

		internal T Get(T aItem)
		{
			if (this.Count == 0)
			{
				return default(T);
			}
			if (object.Equals(this.First, aItem))
			{
				return this.First;
			}
			return this.Rest.Get(aItem);
		}

		internal FunctionalList<T> Reverse()
		{
			FunctionalList<T> functionalList = FunctionalList<T>.Empty;
			foreach (T item in this)
			{
				functionalList = functionalList.Add(item);
			}
			return functionalList;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new FunctionalListEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
