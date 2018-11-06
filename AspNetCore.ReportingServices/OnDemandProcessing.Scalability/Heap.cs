using AspNetCore.ReportingServices.ReportProcessing;
using System;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class Heap<K, V> where K : IComparable<K>
	{
		private struct HeapEntry : IComparable<HeapEntry>
		{
			private K m_key;

			private V m_value;

			private int m_insertIndex;

			public K Key
			{
				get
				{
					return this.m_key;
				}
			}

			public V Value
			{
				get
				{
					return this.m_value;
				}
			}

			public HeapEntry(K key, V value, int insertIndex)
			{
				this.m_key = key;
				this.m_value = value;
				this.m_insertIndex = insertIndex;
			}

			public int CompareTo(HeapEntry other)
			{
				int num = this.m_key.CompareTo(other.m_key);
				if (num == 0)
				{
					num = this.m_insertIndex - other.m_insertIndex;
				}
				return num;
			}
		}

		private HeapEntry[] m_keys;

		private int m_count;

		private int m_insertIndex;

		private int m_maxCapacity;

		public int Count
		{
			get
			{
				return this.m_count;
			}
		}

		public int Capacity
		{
			get
			{
				return this.m_keys.Length;
			}
		}

		public Heap(int capacity)
			: this(capacity, -1)
		{
		}

		public Heap(int initialCapacity, int maxCapacity)
		{
			this.m_keys = new HeapEntry[initialCapacity];
			this.m_count = 0;
			this.m_insertIndex = 0;
			this.m_maxCapacity = maxCapacity;
		}

		public void Insert(K key, V value)
		{
			if (this.m_keys.Length == this.m_count)
			{
				if (this.m_count < this.m_maxCapacity || this.m_maxCapacity == -1)
				{
					int num = (int)((double)this.m_keys.Length * 1.5);
					if (this.m_maxCapacity > 0 && num > this.m_maxCapacity)
					{
						num = this.m_maxCapacity;
					}
					Array.Resize<HeapEntry>(ref this.m_keys, num);
				}
				else
				{
					Global.Tracer.Assert(false, "Invalid Operation: Cannot add to heap at maximum capacity");
				}
			}
			int num2 = this.m_count;
			this.m_count++;
			this.m_keys[num2] = new HeapEntry(key, value, this.m_insertIndex);
			this.m_insertIndex++;
			int num3 = (num2 - 1) / 2;
			while (num2 > 0 && this.LessThan(num3, num2))
			{
				this.Swap(num3, num2);
				num2 = num3;
				num3 = (num2 - 1) / 2;
			}
		}

		public V ExtractMax()
		{
			V result = this.Peek();
			int num = this.m_count - 1;
			this.m_keys[0] = this.m_keys[num];
			this.m_count--;
			this.Heapify(0);
			if (this.m_maxCapacity > 0 && (double)this.m_count < 0.5 * (double)this.m_keys.Length && this.m_keys.Length > 10)
			{
				int num2 = (int)(0.6 * (double)this.m_keys.Length);
				if (num2 < this.m_count)
				{
					num2 = this.m_count;
				}
				if (num2 < 10)
				{
					num2 = 10;
				}
				Array.Resize<HeapEntry>(ref this.m_keys, num2);
			}
			return result;
		}

		public V Peek()
		{
			if (this.m_count == 0)
			{
				Global.Tracer.Assert(false, "Cannot Peek from empty heap");
			}
			return this.m_keys[0].Value;
		}

		private void Heapify(int startIndex)
		{
			int num = 2 * startIndex + 1;
			int num2 = num + 1;
			int num3 = -1;
			num3 = ((num >= this.m_count || !this.GreaterThan(num, startIndex)) ? startIndex : num);
			if (num2 < this.m_count && this.GreaterThan(num2, num3))
			{
				num3 = num2;
			}
			if (num3 != startIndex)
			{
				this.Swap(num3, startIndex);
				this.Heapify(num3);
			}
		}

		private bool GreaterThan(int index1, int index2)
		{
			return this.m_keys[index1].CompareTo(this.m_keys[index2]) > 0;
		}

		private bool LessThan(int index1, int index2)
		{
			return this.m_keys[index1].CompareTo(this.m_keys[index2]) < 0;
		}

		private void Swap(int index1, int index2)
		{
			HeapEntry heapEntry = this.m_keys[index1];
			this.m_keys[index1] = this.m_keys[index2];
			this.m_keys[index2] = heapEntry;
		}
	}
}
