using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class ScalableList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, IStorable, IPersistable, IDisposable
	{
		private enum BucketPinState
		{
			None,
			UntilBucketFull,
			UntilListEnd
		}

		private struct ScalableListEnumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			private int m_currentIndex;

			private ScalableList<T> m_list;

			private int m_version;

			public T Current
			{
				get
				{
					if (this.m_list.m_version != this.m_version)
					{
						Global.Tracer.Assert(false, "ScalableListEnumerator: Cannot use enumerator after modifying the underlying collection");
					}
					if (this.m_currentIndex < 0 || this.m_currentIndex > this.m_list.Count)
					{
						Global.Tracer.Assert(false, "ScalableListEnumerator: Enumerator beyond the bounds of the underlying collection");
					}
					return this.m_list[this.m_currentIndex];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal ScalableListEnumerator(ScalableList<T> list)
			{
				this.m_list = list;
				this.m_version = list.m_version;
				this.m_currentIndex = -1;
				this.Reset();
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				if (this.m_list.m_version != this.m_version)
				{
					Global.Tracer.Assert(false, "ScalableListEnumerator: Cannot use enumerator after modifying the underlying collection");
				}
				if (this.m_currentIndex < this.m_list.Count - 1)
				{
					this.m_currentIndex++;
					return true;
				}
				return false;
			}

			public void Reset()
			{
				this.m_currentIndex = -1;
				this.m_version = this.m_list.m_version;
			}
		}

		private IScalabilityCache m_cache;

		private int m_bucketSize;

		private int m_count;

		private int m_capacity;

		private int m_priority;

		private ScalableList<IReference<StorableArray>> m_buckets;

		private IReference<StorableArray> m_array;

		private int m_version;

		private bool m_isReadOnly;

		private BucketPinState m_bucketPinState;

		private static Declaration m_declaration = ScalableList<T>.GetDeclaration();

		public T this[int index]
		{
			get
			{
				this.CheckIndex(index, this.m_count - 1);
				if (this.m_array != null)
				{
					using (this.m_array.PinValue())
					{
						return (T)this.m_array.Value().Array[index];
					}
				}
				IReference<StorableArray> reference = this.m_buckets[this.GetBucketIndex(index)];
				using (reference.PinValue())
				{
					StorableArray bucket = reference.Value();
					return this.GetValueAt(index, bucket);
				}
			}
			set
			{
				this.CheckReadOnly("set value");
				this.SetValue(index, value, false);
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (T)value;
			}
		}

		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

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
				return this.m_capacity;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.m_isReadOnly;
			}
		}

		public int Size
		{
			get
			{
				return 12 + ItemSizes.SizeOf<IReference<StorableArray>>(this.m_buckets) + ItemSizes.SizeOf(this.m_array) + 4 + 4;
			}
		}

		public ScalableList()
		{
		}

		internal ScalableList(int priority, IScalabilityCache cache)
			: this(priority, cache, 10)
		{
		}

		internal ScalableList(int priority, IScalabilityCache cache, int segmentSize)
			: this(priority, cache, segmentSize, segmentSize)
		{
		}

		internal ScalableList(int priority, IScalabilityCache cache, int segmentSize, int capacity)
			: this(priority, cache, segmentSize, capacity, false)
		{
		}

		internal ScalableList(int priority, IScalabilityCache cache, int segmentSize, int capacity, bool keepAllBucketsPinned)
		{
			this.m_priority = priority;
			this.m_cache = cache;
			this.m_bucketSize = segmentSize;
			this.m_count = 0;
			if (keepAllBucketsPinned)
			{
				this.m_bucketPinState = BucketPinState.UntilListEnd;
			}
			else if (cache.CacheType == ScalabilityCacheType.GroupTree || cache.CacheType == ScalabilityCacheType.Lookup)
			{
				this.m_bucketPinState = BucketPinState.UntilBucketFull;
			}
			this.EnsureCapacity(capacity);
		}

		public int IndexOf(T item)
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			if (this.m_array != null)
			{
				object[] array = this.m_array.Value().Array;
				for (int i = 0; i < this.m_count; i++)
				{
					if (@default.Equals(item, (T)array[i]))
					{
						return i;
					}
				}
			}
			else
			{
				int count = this.m_buckets.Count;
				for (int j = 0; j < count; j++)
				{
					IReference<StorableArray> reference = this.m_buckets[j];
					using (reference.PinValue())
					{
						object[] array2 = reference.Value().Array;
						int num = 0;
						if (j == count - 1)
						{
							num = this.m_count % this.m_bucketSize;
						}
						if (num == 0)
						{
							num = this.m_bucketSize;
						}
						for (int k = 0; k < num; k++)
						{
							if (@default.Equals(item, (T)array2[k]))
							{
								return j * this.m_bucketSize + k;
							}
						}
					}
				}
			}
			return -1;
		}

		public void Insert(int index, T itemToInsert)
		{
			this.CheckReadOnly("Insert");
			this.CheckIndex(index, this.m_count);
			this.EnsureCapacity(this.m_count + 1);
			if (this.m_array != null)
			{
				using (this.m_array.PinValue())
				{
					object[] array = this.m_array.Value().Array;
					Array.Copy(array, index, array, index + 1, this.m_count - index);
					array[index] = itemToInsert;
				}
			}
			else
			{
				object obj = itemToInsert;
				object obj2 = obj;
				for (int i = this.GetBucketIndex(index); i < this.m_buckets.Count; i++)
				{
					obj = obj2;
					int indexInBucket = this.GetIndexInBucket(index);
					int num = (i != this.m_buckets.Count - 1) ? (this.m_bucketSize - 1) : this.GetIndexInBucket(this.m_count);
					IReference<StorableArray> reference = this.m_buckets[i];
					using (reference.PinValue())
					{
						object[] array2 = reference.Value().Array;
						obj2 = array2[num];
						Array.Copy(array2, indexInBucket, array2, indexInBucket + 1, num - indexInBucket);
						array2[indexInBucket] = obj;
					}
					index = 0;
				}
			}
			this.m_count++;
			this.m_version++;
		}

		public void RemoveAt(int index)
		{
			this.RemoveRange(index, 1);
		}

		private T GetValueAt(int index, StorableArray bucket)
		{
			object obj = bucket.Array[this.GetIndexInBucket(index)];
			if (obj == null)
			{
				return default(T);
			}
			return (T)obj;
		}

		private void SetValue(int index, T value, bool fromAdd)
		{
			this.CheckIndex(index, this.m_count - 1);
			if (this.m_array != null)
			{
				using (this.m_array.PinValue())
				{
					this.m_array.Value().Array[index] = value;
					if (fromAdd)
					{
						this.m_array.UpdateSize(ItemSizes.SizeOfInObjectArray(value));
					}
				}
			}
			else
			{
				IReference<StorableArray> reference = this.m_buckets[this.GetBucketIndex(index)];
				using (reference.PinValue())
				{
					StorableArray storableArray = reference.Value();
					storableArray.Array[this.GetIndexInBucket(index)] = value;
					if (fromAdd)
					{
						reference.UpdateSize(ItemSizes.SizeOfInObjectArray(value));
					}
				}
			}
			this.m_version++;
		}

		public void SetValueWithExtension(int index, T item)
		{
			this.CheckReadOnly("SetValueWithExtension");
			int count = Math.Max(this.m_count, index + 1);
			this.EnsureCapacity(count);
			this.m_count = count;
			this.SetValue(index, item, true);
		}

		public IDisposable GetAndPin(int index, out T item)
		{
			this.CheckIndex(index, this.m_count - 1);
			if (this.m_array != null)
			{
				this.m_array.PinValue();
				item = (T)this.m_array.Value().Array[index];
				return (IDisposable)this.m_array;
			}
			IReference<StorableArray> reference = this.m_buckets[this.GetBucketIndex(index)];
			reference.PinValue();
			StorableArray bucket = reference.Value();
			item = this.GetValueAt(index, bucket);
			return (IDisposable)reference;
		}

		public IDisposable AddAndPin(T item)
		{
			this.CheckReadOnly("AddAndPin");
			this.EnsureCapacity(this.m_count + 1);
			this.m_count++;
			this.m_version++;
			IDisposable result = this.SetAndPin(this.m_count - 1, item, true);
			this.CheckFilledBucket();
			return result;
		}

		public IDisposable SetAndPin(int index, T item)
		{
			this.CheckReadOnly("SetAndPin");
			return this.SetAndPin(index, item, false);
		}

		private IDisposable SetAndPin(int index, T item, bool fromAdd)
		{
			this.CheckIndex(index, this.m_count - 1);
			IDisposable result;
			if (this.m_array != null)
			{
				result = this.m_array.PinValue();
				this.m_array.Value().Array[index] = item;
				if (fromAdd)
				{
					this.m_array.UpdateSize(ItemSizes.SizeOfInObjectArray(item));
				}
			}
			else
			{
				int bucketIndex = this.GetBucketIndex(index);
				IReference<StorableArray> reference = this.m_buckets[bucketIndex];
				UnPinCascadeHolder unPinCascadeHolder = new UnPinCascadeHolder();
				unPinCascadeHolder.AddCleanupRef(reference.PinValue());
				this.m_buckets.PinContainingBucket(bucketIndex, unPinCascadeHolder);
				result = unPinCascadeHolder;
				StorableArray storableArray = reference.Value();
				storableArray.Array[this.GetIndexInBucket(index)] = item;
				if (fromAdd)
				{
					reference.UpdateSize(ItemSizes.SizeOfInObjectArray(item));
				}
			}
			return result;
		}

		public void RemoveRange(int index, int count)
		{
			this.CheckReadOnly("RemoveRange");
			if (index < 0)
			{
				Global.Tracer.Assert(false, "ScalableList.RemoveRange: Index may not be less than 0");
			}
			if (count < 0)
			{
				Global.Tracer.Assert(false, "ScalableList.RemoveRange: Count may not be less than 0");
			}
			if (index + count > this.Count)
			{
				Global.Tracer.Assert(false, "ScalableList.RemoveRange: Index + Count may not be larger than the number of elements in the list");
			}
			if (this.m_array != null)
			{
				using (this.m_array.PinValue())
				{
					object[] array = this.m_array.Value().Array;
					int length = array.Length - count - index;
					Array.Copy(array, index + count, array, index, length);
				}
				this.m_count -= count;
			}
			else
			{
				int num = index + count;
				int num2 = index;
				while (true)
				{
					int indexInBucket = this.GetIndexInBucket(num);
					int indexInBucket2 = this.GetIndexInBucket(num2);
					int num3 = Math.Min(this.m_bucketSize - indexInBucket2, Math.Min(this.m_bucketSize - indexInBucket, this.m_count - num));
					if (num3 <= 0)
					{
						break;
					}
					IReference<StorableArray> reference = this.m_buckets[this.GetBucketIndex(num2)];
					IReference<StorableArray> reference2 = this.m_buckets[this.GetBucketIndex(num)];
					using (reference.PinValue())
					{
						using (reference2.PinValue())
						{
							object[] array2 = reference2.Value().Array;
							object[] array3 = reference.Value().Array;
							Array.Copy(array2, indexInBucket, array3, indexInBucket2, num3);
						}
					}
					num2 += num3;
					num += num3;
				}
				this.m_count -= count;
				int num4 = this.GetBucketIndex(this.m_count);
				if (this.m_count % this.m_bucketSize != 0 || num4 == 0)
				{
					num4++;
				}
				int num5 = this.m_buckets.Count - num4;
				if (num5 > 0)
				{
					this.m_buckets.RemoveRange(num4, num5);
					this.m_capacity -= num5 * this.m_bucketSize;
				}
			}
			this.m_version++;
		}

		public int BinarySearch(T value, IComparer comparer)
		{
			if (comparer == null)
			{
				Global.Tracer.Assert(false, "Cannot pass null comparer to BinarySearch");
			}
			if (this.m_array != null)
			{
				StorableArray storableArray = this.m_array.Value();
				return Array.BinarySearch(storableArray.Array, 0, this.Count, value, comparer);
			}
			ArrayList arrayList = ArrayList.Adapter(this);
			return arrayList.BinarySearch(value, comparer);
		}

		int IList.Add(object value)
		{
			int count = this.Count;
			this.Add((T)value);
			return count;
		}

		bool IList.Contains(object value)
		{
			return this.Contains((T)value);
		}

		int IList.IndexOf(object value)
		{
			return this.IndexOf((T)value);
		}

		void IList.Insert(int index, object value)
		{
			this.Insert(index, (T)value);
		}

		void IList.Remove(object value)
		{
			this.Remove((T)value);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.InternalCopyTo(array, index);
		}

		private void CheckIndex(int index, int inclusiveLimit)
		{
			if (index >= 0 && index <= inclusiveLimit)
			{
				return;
			}
			Global.Tracer.Assert(false, "ScalableList: Index {0} outside the allowed range [0::{1}]", index, inclusiveLimit);
		}

		private void CheckReadOnly(string operation)
		{
			if (this.m_isReadOnly)
			{
				Global.Tracer.Assert(false, "Cannot {0} on a read-only ScalableList", operation);
			}
		}

		private int GetBucketIndex(int index)
		{
			return index / this.m_bucketSize;
		}

		private int GetIndexInBucket(int index)
		{
			return index % this.m_bucketSize;
		}

		private void EnsureCapacity(int count)
		{
			if (count > this.m_capacity)
			{
				if (this.m_array == null && this.m_buckets == null)
				{
					StorableArray storableArray = new StorableArray();
					storableArray.Array = new object[count];
					int emptySize = storableArray.EmptySize;
					if (this.m_bucketPinState == BucketPinState.UntilBucketFull || this.m_bucketPinState == BucketPinState.UntilListEnd)
					{
						this.m_array = this.m_cache.AllocateAndPin<StorableArray>(storableArray, this.m_priority, emptySize);
					}
					else
					{
						this.m_array = this.m_cache.Allocate<StorableArray>(storableArray, this.m_priority, emptySize);
					}
					this.m_capacity = count;
				}
				if (this.m_array != null)
				{
					if (count <= this.m_bucketSize)
					{
						int num = Math.Min(Math.Max(count, this.m_capacity * 2), this.m_bucketSize);
						using (this.m_array.PinValue())
						{
							StorableArray storableArray2 = this.m_array.Value();
							Array.Resize<object>(ref storableArray2.Array, num);
						}
						this.m_capacity = num;
					}
					else
					{
						if (this.m_capacity < this.m_bucketSize)
						{
							using (this.m_array.PinValue())
							{
								StorableArray storableArray3 = this.m_array.Value();
								Array.Resize<object>(ref storableArray3.Array, this.m_bucketSize);
							}
							this.m_capacity = this.m_bucketSize;
						}
						this.m_buckets = new ScalableList<IReference<StorableArray>>(this.m_priority, this.m_cache, 100, 10, this.m_bucketPinState == BucketPinState.UntilListEnd);
						this.m_buckets.Add(this.m_array);
						this.m_array = null;
					}
				}
				if (this.m_buckets != null)
				{
					while (this.GetBucketIndex(count - 1) >= this.m_buckets.Count)
					{
						StorableArray storableArray4 = new StorableArray();
						storableArray4.Array = new object[this.m_bucketSize];
						int emptySize2 = storableArray4.EmptySize;
						if (this.m_bucketPinState == BucketPinState.UntilListEnd)
						{
							IReference<StorableArray> item = this.m_cache.AllocateAndPin<StorableArray>(storableArray4, this.m_priority, emptySize2);
							this.m_buckets.Add(item);
						}
						else if (this.m_bucketPinState == BucketPinState.UntilBucketFull)
						{
							IReference<StorableArray> item = this.m_cache.AllocateAndPin<StorableArray>(storableArray4, this.m_priority, emptySize2);
							this.m_buckets.AddAndPin(item);
						}
						else
						{
							IReference<StorableArray> item = this.m_cache.Allocate<StorableArray>(storableArray4, this.m_priority, emptySize2);
							this.m_buckets.Add(item);
						}
						this.m_capacity += this.m_bucketSize;
					}
				}
			}
		}

		private void UnPinContainingBucket(int index)
		{
			if (this.m_array != null)
			{
				this.m_array.UnPinValue();
			}
			else
			{
				int bucketIndex = this.GetBucketIndex(index);
				this.m_buckets[bucketIndex].UnPinValue();
				this.m_buckets.UnPinContainingBucket(bucketIndex);
			}
		}

		private void PinContainingBucket(int index, UnPinCascadeHolder cascadeHolder)
		{
			if (this.m_array != null)
			{
				cascadeHolder.AddCleanupRef(this.m_array.PinValue());
			}
			else
			{
				int bucketIndex = this.GetBucketIndex(index);
				cascadeHolder.AddCleanupRef(this.m_buckets[bucketIndex].PinValue());
				this.m_buckets.PinContainingBucket(bucketIndex, cascadeHolder);
			}
		}

		private void InternalCopyTo(Array array, int arrayIndex)
		{
			if (array == null)
			{
				Global.Tracer.Assert(false, "ScalableList.CopyTo: Dest array cannot be null");
			}
			if (arrayIndex < 0)
			{
				Global.Tracer.Assert(false, "ScalableList.CopyTo: Index must not be less than 0");
			}
			if (arrayIndex != 1)
			{
				Global.Tracer.Assert(false, "ScalableList.CopyTo: Array must be one-dimensional");
			}
			if (arrayIndex > array.Length - 1)
			{
				Global.Tracer.Assert(false, "ScalableList.CopyTo: Start index must be less than the size of the array");
			}
			if (arrayIndex + this.m_count > array.Length)
			{
				Global.Tracer.Assert(false, "ScalableList.CopyTo: Insufficent space in the target array");
			}
			if (this.m_array != null)
			{
				using (this.m_array.PinValue())
				{
					Array.Copy(this.m_array.Value().Array, 0, array, arrayIndex, this.m_count);
				}
			}
			else
			{
				int num = this.m_buckets.Count - 1;
				IReference<StorableArray> reference;
				for (int i = 0; i < num; i++)
				{
					reference = this.m_buckets[i];
					using (reference.PinValue())
					{
						object[] array2 = reference.Value().Array;
						Array.Copy(array2, 0, array, arrayIndex + i * this.m_bucketSize, this.m_bucketSize);
					}
				}
				int num2 = this.GetIndexInBucket(this.m_count);
				if (num2 == 0)
				{
					num2 = this.m_bucketSize;
				}
				reference = this.m_buckets[num];
				using (reference.PinValue())
				{
					object[] array2 = reference.Value().Array;
					Array.Copy(array2, 0, array, arrayIndex + num * this.m_bucketSize, num2);
				}
			}
		}

		public void Add(T item)
		{
			this.CheckReadOnly("Add");
			this.EnsureCapacity(this.m_count + 1);
			this.m_count++;
			this.m_version++;
			this.SetValue(this.m_count - 1, item, true);
			this.CheckFilledBucket();
		}

		private void CheckFilledBucket()
		{
			if (this.m_bucketPinState == BucketPinState.UntilBucketFull && this.m_count % this.m_bucketSize == 0)
			{
				if (this.m_array != null)
				{
					this.m_array.UnPinValue();
				}
				else
				{
					int bucketIndex = this.GetBucketIndex(this.m_count - 1);
					this.m_buckets[bucketIndex].UnPinValue();
					this.m_buckets.UnPinContainingBucket(bucketIndex);
				}
			}
		}

		public void AddRange(IEnumerable<T> collection)
		{
			if (collection == null)
			{
				Global.Tracer.Assert(false, "ScalableList.AddRange: Collection cannot be null");
			}
			foreach (T item in collection)
			{
				this.Add(item);
			}
		}

		public void AddRange(IList<T> list)
		{
			this.CheckReadOnly("AddRange");
			if (list == null)
			{
				Global.Tracer.Assert(false, "ScalableList.AddRange(IList<T>): List to add may not be null");
			}
			this.EnsureCapacity(this.m_count + list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				this.Add(list[i]);
			}
		}

		public void Clear()
		{
			this.CheckReadOnly("Clear");
			this.m_count = 0;
			if (this.m_array != null)
			{
				this.m_array.Free();
				this.m_array = null;
			}
			if (this.m_buckets != null)
			{
				int count = this.m_buckets.Count;
				for (int i = 0; i < count; i++)
				{
					this.m_buckets[i].Free();
				}
				this.m_buckets.Clear();
			}
			this.m_buckets = null;
			this.m_capacity = 0;
			this.m_version++;
		}

		public void UnPinAll()
		{
			if (this.m_bucketPinState == BucketPinState.UntilListEnd)
			{
				if (this.m_array != null)
				{
					this.m_array.UnPinValue();
				}
				if (this.m_buckets != null)
				{
					for (int i = 0; i < this.m_buckets.Count; i++)
					{
						this.m_buckets[i].UnPinValue();
					}
					this.m_buckets.UnPinAll();
				}
			}
			else if (this.m_bucketPinState == BucketPinState.UntilBucketFull)
			{
				if (this.m_count < Math.Max(this.m_capacity, this.m_bucketSize))
				{
					if (this.m_array != null)
					{
						this.m_array.UnPinValue();
					}
					else
					{
						int bucketIndex = this.GetBucketIndex(this.m_count - 1);
						IReference<StorableArray> reference = this.m_buckets[bucketIndex];
						reference.UnPinValue();
						this.m_buckets.UnPinContainingBucket(bucketIndex);
					}
				}
				if (this.m_buckets != null)
				{
					this.m_buckets.UnPinAll();
				}
			}
		}

		public void TransferTo(IScalabilityCache scaleCache)
		{
			if (this.m_array != null)
			{
				this.m_array = (IReference<StorableArray>)this.m_array.TransferTo(scaleCache);
			}
			else
			{
				this.m_buckets.TransferTo(scaleCache);
			}
			this.m_cache = scaleCache;
		}

		public bool Contains(T item)
		{
			return this.IndexOf(item) != -1;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.InternalCopyTo(array, arrayIndex);
		}

		public void SetReadOnly()
		{
			this.m_isReadOnly = true;
		}

		public bool Remove(T item)
		{
			this.CheckReadOnly("Remove");
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			if (this.m_array != null)
			{
				using (this.m_array.PinValue())
				{
					object[] array = this.m_array.Value().Array;
					return this.Remove(item, 0, array, this.m_count, @default);
				}
			}
			bool flag = false;
			int count = this.m_buckets.Count;
			for (int i = 0; i < count; i++)
			{
				if (flag)
				{
					break;
				}
				IReference<StorableArray> reference = this.m_buckets[i];
				using (reference.PinValue())
				{
					int limit = (i != count - 1) ? this.m_bucketSize : (this.m_count - i * this.m_bucketSize);
					object[] array2 = reference.Value().Array;
					flag = this.Remove(item, i * this.m_bucketSize, array2, limit, @default);
				}
			}
			return flag;
		}

		private bool Remove(T item, int baseIndex, object[] array, int limit, IEqualityComparer<T> comparer)
		{
			for (int i = 0; i < limit; i++)
			{
				if (comparer.Equals(item, (T)array[i]))
				{
					this.RemoveAt(baseIndex + i);
					return true;
				}
			}
			return false;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return (IEnumerator<T>)(object)new ScalableListEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)(object)new ScalableListEnumerator(this);
		}

		public void Dispose()
		{
			this.Clear();
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ScalableList<T>.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.BucketSize:
					writer.Write(this.m_bucketSize);
					break;
				case MemberName.Count:
					writer.Write(this.m_count);
					break;
				case MemberName.Capacity:
					writer.Write(this.m_capacity);
					break;
				case MemberName.Buckets:
					writer.Write(this.m_buckets);
					break;
				case MemberName.Array:
					writer.Write(this.m_array);
					break;
				case MemberName.Version:
					writer.Write(this.m_version);
					break;
				case MemberName.Priority:
					writer.Write(this.m_priority);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ScalableList<T>.m_declaration);
			this.m_cache = (reader.PersistenceHelper as IScalabilityCache);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.BucketSize:
					this.m_bucketSize = reader.ReadInt32();
					break;
				case MemberName.Count:
					this.m_count = reader.ReadInt32();
					break;
				case MemberName.Capacity:
					this.m_capacity = reader.ReadInt32();
					break;
				case MemberName.Buckets:
					this.m_buckets = reader.ReadRIFObject<ScalableList<IReference<StorableArray>>>();
					break;
				case MemberName.Array:
					this.m_array = (IReference<StorableArray>)reader.ReadRIFObject();
					break;
				case MemberName.Version:
					this.m_version = reader.ReadInt32();
					break;
				case MemberName.Priority:
					this.m_priority = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList;
		}

		public static Declaration GetDeclaration()
		{
			if (ScalableList<T>.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.BucketSize, Token.Int32));
				list.Add(new MemberInfo(MemberName.Count, Token.Int32));
				list.Add(new MemberInfo(MemberName.Capacity, Token.Int32));
				list.Add(new MemberInfo(MemberName.Buckets, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
				list.Add(new MemberInfo(MemberName.Array, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArrayReference));
				list.Add(new MemberInfo(MemberName.Version, Token.Int32));
				list.Add(new MemberInfo(MemberName.Priority, Token.Int32));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return ScalableList<T>.m_declaration;
		}
	}
}
