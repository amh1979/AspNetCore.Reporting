using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Common
{
	internal static class CollectionUtil
	{
		public static bool IsEmpty<T>(IEnumerable<T> items)
		{
			ICollection<T> collection = items as ICollection<T>;
			if (collection == null)
			{
				return !items.GetEnumerator().MoveNext();
			}
			return collection.Count > 0;
		}

		public static bool ElementsEqual<T>(ICollection<T> items1, ICollection<T> items2)
		{
			return CollectionUtil.ElementsEqual(items1, items2, null);
		}

		public static bool ElementsEqual<T>(ICollection<T> items1, ICollection<T> items2, IEqualityComparer<T> comparer)
		{
			if (items1 == null)
			{
				throw new ArgumentNullException("items1");
			}
			if (items2 == null)
			{
				throw new ArgumentNullException("items2");
			}
			if (comparer == null)
			{
				comparer = EqualityComparer<T>.Default;
			}
			if (items1.Count != items2.Count)
			{
				return false;
			}
			IList<T> list = items1 as IList<T>;
			IList<T> list2 = items2 as IList<T>;
			if (list != null && list2 != null)
			{
				for (int i = 0; i < ((ICollection<T>)list).Count; i++)
				{
					if (!comparer.Equals(list[i], list2[i]))
					{
						return false;
					}
				}
				return true;
			}
			IEnumerator<T> enumerator = ((IEnumerable<T>)items1).GetEnumerator();
			IEnumerator<T> enumerator2 = ((IEnumerable<T>)items2).GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator2.MoveNext();
				if (!comparer.Equals(enumerator.Current, enumerator2.Current))
				{
					return false;
				}
			}
			return true;
		}

		public static bool ContainsAll<T>(ICollection<T> items1, ICollection<T> items2)
		{
			if (items1 == null)
			{
				throw new ArgumentNullException("items1");
			}
			if (items2 == null)
			{
				throw new ArgumentNullException("items2");
			}
			foreach (T item in (IEnumerable<T>)items2)
			{
				if (!items1.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		public static void RemoveDuplicates<T>(IList<T> items)
		{
			CollectionUtil.RemoveDuplicates(items, null);
		}

		public static void RemoveDuplicates<T>(IList<T> items, IEqualityComparer<T> comparer)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			if (comparer == null)
			{
				comparer = EqualityComparer<T>.Default;
			}
			for (int i = 0; i < ((ICollection<T>)items).Count; i++)
			{
				for (int num = ((ICollection<T>)items).Count - 1; num > i; num--)
				{
					if (comparer.Equals(items[i], items[num]))
					{
						items.RemoveAt(num);
					}
				}
			}
		}
	}
}
