using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Common
{
	internal static class Iterators
	{
		public struct ReverseEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator, IEnumerable<T>, IEnumerable
		{
			private readonly IList<T> m_list;

			private int m_index;

			private T m_current;

			public T Current
			{
				get
				{
					return this.m_current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.m_current;
				}
			}

			internal ReverseEnumerator(IList<T> list)
			{
				this.m_list = list;
				this.m_index = this.m_list.Count;
				this.m_current = default(T);
			}

			public void Reset()
			{
				this.m_index = this.m_list.Count;
				this.m_current = default(T);
			}

			public bool MoveNext()
			{
				if (this.m_index == 0)
				{
					return false;
				}
				this.m_index--;
				this.m_current = this.m_list[this.m_index];
				return true;
			}

			public ReverseEnumerator<T> GetEnumerator()
			{
				return new ReverseEnumerator<T>(this.m_list);
			}

			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				return (IEnumerator<T>)(object)this.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return (IEnumerator)(object)new ReverseEnumerator<T>(this.m_list);
			}

			void IDisposable.Dispose()
			{
			}
		}

		public static ReverseEnumerator<T> Reverse<T>(IList<T> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			return new ReverseEnumerator<T>(list);
		}

		public static IEnumerable<T> FilterByType<T>(IEnumerable<T> items, Type returnType)
		{
			IList<T> list = items as IList<T>;
			if (list != null)
			{
				for (int i = 0; i < ((ICollection<T>)list).Count; i++)
				{
					if (returnType.IsInstanceOfType(list[i]))
					{
						yield return list[i];
					}
				}
			}
			else
			{
				foreach (T item in items)
				{
					if (returnType.IsInstanceOfType(item))
					{
						yield return item;
					}
				}
			}
		}

		public static IEnumerable<T> Filter<T>(IEnumerable<T> items, Predicate<T> match)
		{
			IList<T> list = items as IList<T>;
			if (list != null)
			{
				for (int i = 0; i < ((ICollection<T>)list).Count; i++)
				{
					if (match(list[i]))
					{
						yield return list[i];
					}
				}
			}
			else
			{
				foreach (T item in items)
				{
					if (match(item))
					{
						yield return item;
					}
				}
			}
		}
	}
}
