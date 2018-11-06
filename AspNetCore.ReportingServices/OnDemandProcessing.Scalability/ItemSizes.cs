using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal static class ItemSizes
	{
		public const int BoolSize = 1;

		public const int ByteSize = 1;

		public const int SByteSize = 1;

		public const int Int16Size = 2;

		public const int UInt16Size = 2;

		public const int Int32Size = 4;

		public const int UInt32Size = 4;

		public const int Int64Size = 8;

		public const int UInt64Size = 8;

		public const int CharSize = 2;

		public const int DoubleSize = 8;

		public const int SingleSize = 4;

		public const int DecimalSize = 16;

		public const int DateTimeSize = 8;

		public const int TimeSpanSize = 8;

		public const int DateTimeOffsetSize = 16;

		public const int GuidSize = 16;

		public const int Int32EnumSize = 4;

		public const int NullableOverhead = 1;

		public const int ListOverhead = 24;

		public const int ArrayOverhead = 8;

		public const int HashtableOverhead = 56;

		public const int HashtableEntryOverhead = 4;

		public static readonly int ReferenceSize = IntPtr.Size;

		public static readonly int NullableBoolSize = ItemSizes.PointerAlign(2);

		public static readonly int NullableByteSize = ItemSizes.PointerAlign(2);

		public static readonly int NullableSByteSize = ItemSizes.PointerAlign(2);

		public static readonly int NullableInt16Size = ItemSizes.PointerAlign(3);

		public static readonly int NullableUInt16Size = ItemSizes.PointerAlign(3);

		public static readonly int NullableInt32Size = ItemSizes.PointerAlign(5);

		public static readonly int NullableUInt32Size = ItemSizes.PointerAlign(5);

		public static readonly int NullableInt64Size = ItemSizes.PointerAlign(9);

		public static readonly int NullableUInt64Size = ItemSizes.PointerAlign(9);

		public static readonly int NullableCharSize = ItemSizes.PointerAlign(3);

		public static readonly int NullableDoubleSize = ItemSizes.PointerAlign(9);

		public static readonly int NullableSingleSize = ItemSizes.PointerAlign(5);

		public static readonly int NullableDecimalSize = ItemSizes.PointerAlign(17);

		public static readonly int NullableDateTimeSize = ItemSizes.PointerAlign(9);

		public static readonly int NullableGuidSize = ItemSizes.PointerAlign(17);

		public static readonly int NullableTimeSpanSize = ItemSizes.PointerAlign(9);

		public static readonly int GdiColorSize = 12 + ItemSizes.ReferenceSize;

		public static readonly int ObjectOverhead = ItemSizes.ReferenceSize * 2;

		public static readonly int NonNullIStorableOverhead = ItemSizes.ReferenceSize + ItemSizes.ObjectOverhead;

		public static int PointerAlign(int size)
		{
			int num = size % ItemSizes.ReferenceSize;
			if (num > 0)
			{
				return size - num + ItemSizes.ReferenceSize;
			}
			return size;
		}

		public static int SizeOf(IStorable obj)
		{
			int num = ItemSizes.ReferenceSize;
			if (obj != null)
			{
				num += ItemSizes.ObjectOverhead + obj.Size;
			}
			return num;
		}

		public static int SizeOf<T>(ScalableList<T> obj)
		{
			return ItemSizes.SizeOf((IStorable)obj);
		}

		public static int SizeOf<T>(List<T> list) where T : IStorable
		{
			int num = ItemSizes.ReferenceSize;
			if (list != null)
			{
				num += 24;
				for (int i = 0; i < list.Count; i++)
				{
					num += ItemSizes.SizeOf((IStorable)(object)list[i]);
				}
			}
			return num;
		}

		public static int SizeOfEmptyObjectArray(int length)
		{
			return ItemSizes.ReferenceSize + 8 + ItemSizes.ReferenceSize * length;
		}

		public static int SizeOf<T>(List<List<T>> listOfLists) where T : IStorable
		{
			int num = ItemSizes.ReferenceSize;
			if (listOfLists != null)
			{
				num += 24;
				for (int i = 0; i < listOfLists.Count; i++)
				{
					num += ItemSizes.SizeOf(listOfLists[i]);
				}
			}
			return num;
		}

		public static int SizeOf(List<object> list)
		{
			int num = ItemSizes.ReferenceSize;
			if (list != null)
			{
				num += 24;
				for (int i = 0; i < list.Count; i++)
				{
					num += ItemSizes.SizeOf(list[i]);
				}
			}
			return num;
		}

		public static int SizeOf<T>(T[] array) where T : IStorable
		{
			int num = ItemSizes.ReferenceSize;
			if (array != null)
			{
				num += 8;
				for (int i = 0; i < array.Length; i++)
				{
					num += ItemSizes.SizeOf((IStorable)(object)array[i]);
				}
			}
			return num;
		}

		public static int SizeOf(object[] array)
		{
			int num = ItemSizes.ReferenceSize;
			if (array != null)
			{
				num += 8;
				for (int i = 0; i < array.Length; i++)
				{
					num += ItemSizes.SizeOf(array[i]);
				}
			}
			return num;
		}

		public static int SizeOf(int[] obj)
		{
			int num = ItemSizes.ReferenceSize;
			if (obj != null)
			{
				num += 8;
				num += obj.Length * 4;
			}
			return num;
		}

		public static int SizeOf(long[] obj)
		{
			int num = ItemSizes.ReferenceSize;
			if (obj != null)
			{
				num += 8;
				num += obj.Length * 8;
			}
			return num;
		}

		public static int SizeOf(double[] obj)
		{
			int num = ItemSizes.ReferenceSize;
			if (obj != null)
			{
				num += 8;
				num += obj.Length * 8;
			}
			return num;
		}

		public static int SizeOf(bool[] obj)
		{
			int num = ItemSizes.ReferenceSize;
			if (obj != null)
			{
				num += 8;
				num += obj.Length;
			}
			return num;
		}

		public static int SizeOf(string[] obj)
		{
			int num = ItemSizes.ReferenceSize;
			if (obj != null)
			{
				num += 8;
				for (int i = 0; i < obj.Length; i++)
				{
					num += ItemSizes.SizeOf(obj[i]);
				}
			}
			return num;
		}

		public static int SizeOf(Array arr)
		{
			int num = ItemSizes.ReferenceSize;
			if (arr != null)
			{
				num += 8;
				int[] indices = new int[arr.Rank];
				num += ItemSizes.TraverseArrayDim(arr, 0, indices);
			}
			return num;
		}

		private static int TraverseArrayDim(Array arr, int dim, int[] indices)
		{
			int num = 0;
			bool flag = arr.Rank == dim + 1;
			int length = arr.GetLength(dim);
			for (int i = 0; i < length; i++)
			{
				indices[dim] = i;
				num = ((!flag) ? (num + ItemSizes.TraverseArrayDim(arr, dim + 1, indices)) : (num + ItemSizes.SizeOf(arr.GetValue(indices))));
			}
			return num;
		}

		public static int SizeOf(List<string> obj)
		{
			int num = ItemSizes.ReferenceSize;
			if (obj != null)
			{
				num += 24;
				for (int i = 0; i < obj.Count; i++)
				{
					num += ItemSizes.SizeOf(obj[i]);
				}
			}
			return num;
		}

		public static int SizeOf(List<int> obj)
		{
			int num = ItemSizes.ReferenceSize;
			if (obj != null)
			{
				num += 24;
				num += obj.Count * 4;
			}
			return num;
		}

		public static int SizeOf(List<object>[] obj)
		{
			int num = ItemSizes.ReferenceSize;
			if (obj != null)
			{
				num += 8;
				for (int i = 0; i < obj.Length; i++)
				{
					num += ItemSizes.SizeOf(obj[i]);
				}
			}
			return num;
		}

		public static int SizeOf<T>(List<T>[] obj) where T : IStorable
		{
			int num = ItemSizes.ReferenceSize;
			if (obj != null)
			{
				num += 8;
				for (int i = 0; i < obj.Length; i++)
				{
					num += ItemSizes.SizeOf(obj[i]);
				}
			}
			return num;
		}

		public static int SizeOf<T>(List<T[]> obj) where T : IStorable
		{
			int num = ItemSizes.ReferenceSize;
			if (obj != null)
			{
				num += 24;
				for (int i = 0; i < obj.Count; i++)
				{
					num += ItemSizes.SizeOf(obj[i]);
				}
			}
			return num;
		}

		public static int SizeOf(Hashtable obj)
		{
			int referenceSize = ItemSizes.ReferenceSize;
			if (obj != null)
			{
				referenceSize += 56;
				IDictionaryEnumerator enumerator = obj.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
						referenceSize += 4;
						referenceSize += ItemSizes.SizeOf(dictionaryEntry.Key);
						referenceSize += ItemSizes.SizeOf(dictionaryEntry.Value);
					}
					return referenceSize;
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
			return referenceSize;
		}

		public static int SizeOf<K, V>(Dictionary<K, V> obj)
		{
			int referenceSize = ItemSizes.ReferenceSize;
			if (obj != null)
			{
				referenceSize += 56;
				{
					foreach (KeyValuePair<K, V> item in obj)
					{
						referenceSize += 4;
						referenceSize += ItemSizes.SizeOf(item.Key);
						referenceSize += ItemSizes.SizeOf(item.Value);
					}
					return referenceSize;
				}
			}
			return referenceSize;
		}

		public static int SizeOf(IList obj)
		{
			int num = ItemSizes.ReferenceSize;
			if (obj != null)
			{
				num += 24;
				for (int i = 0; i < obj.Count; i++)
				{
					num += ItemSizes.SizeOf(obj[i]);
				}
			}
			return num;
		}

		public static int SizeOf(string str)
		{
			int num = ItemSizes.ReferenceSize;
			if (str != null)
			{
				num += ItemSizes.ObjectOverhead + 4 + 4 + str.Length * 2;
			}
			return num;
		}

		public static int SizeOfInObjectArray(object obj)
		{
			return ItemSizes.SizeOf(obj) - ItemSizes.ReferenceSize;
		}

		public static int SizeOf(object obj)
		{
			if (obj == null)
			{
				return ItemSizes.ReferenceSize;
			}
			if (obj is IStorable)
			{
				return ItemSizes.SizeOf((IStorable)obj);
			}
			if (obj is IConvertible)
			{
				switch (((IConvertible)obj).GetTypeCode())
				{
				case TypeCode.Boolean:
				case TypeCode.Char:
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Single:
					return ItemSizes.ReferenceSize + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Double:
					return 8 + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
				case TypeCode.String:
					return ItemSizes.SizeOf((string)obj);
				case TypeCode.Decimal:
					return 16 + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
				case TypeCode.DateTime:
					return 8 + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
				case TypeCode.Object:
					if (obj is TimeSpan)
					{
						return 8 + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
					}
					if (obj is DateTimeOffset)
					{
						return 16 + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
					}
					if (obj is Guid)
					{
						return 16 + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
					}
					if (obj is Color)
					{
						return ItemSizes.GdiColorSize + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
					}
					return ItemSizes.ReferenceSize;
				}
			}
			else
			{
				if (obj is Array)
				{
					return ItemSizes.SizeOf((Array)obj);
				}
				if (obj is IList)
				{
					return ItemSizes.SizeOf((IList)obj);
				}
				if (Nullable.GetUnderlyingType(obj.GetType()) != null)
				{
					if (obj is bool?)
					{
						return ItemSizes.NullableBoolSize + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
					}
					if (obj is byte?)
					{
						return ItemSizes.NullableByteSize + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
					}
					if (obj is sbyte?)
					{
						return ItemSizes.NullableByteSize + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
					}
					if (obj is short?)
					{
						return ItemSizes.NullableInt16Size + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
					}
					if (obj is int?)
					{
						return ItemSizes.NullableInt32Size + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
					}
					if (obj is long?)
					{
						return ItemSizes.NullableInt64Size + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
					}
					if (obj is ushort?)
					{
						return ItemSizes.NullableInt16Size + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
					}
					if (obj is uint?)
					{
						return ItemSizes.NullableInt32Size + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
					}
					if (obj is ulong?)
					{
						return ItemSizes.NullableInt64Size + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
					}
					if (obj is char?)
					{
						return ItemSizes.NullableCharSize + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
					}
					if (obj is double?)
					{
						return ItemSizes.NullableDoubleSize + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
					}
					if (obj is float?)
					{
						return ItemSizes.NullableSingleSize + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
					}
					if (obj is DateTime?)
					{
						return ItemSizes.NullableDateTimeSize + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
					}
					if (obj is Guid?)
					{
						return ItemSizes.NullableGuidSize + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
					}
					if (obj is TimeSpan?)
					{
						return ItemSizes.NullableTimeSpanSize + ItemSizes.ObjectOverhead + ItemSizes.ReferenceSize;
					}
				}
			}
			return ItemSizes.ReferenceSize;
		}
	}
}
