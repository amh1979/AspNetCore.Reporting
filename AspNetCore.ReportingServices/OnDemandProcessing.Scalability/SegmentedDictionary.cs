using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class SegmentedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
	{
		internal interface ISegmentedDictionaryEntry
		{
			SegmentedDictionaryEntryType EntryType
			{
				get;
			}
		}

		internal enum SegmentedDictionaryEntryType
		{
			Node,
			Values
		}

		internal class SegmentedDictionaryNode : ISegmentedDictionaryEntry
		{
			internal ISegmentedDictionaryEntry[] Entries;

			internal int Count;

			public SegmentedDictionaryEntryType EntryType
			{
				get
				{
					return SegmentedDictionaryEntryType.Node;
				}
			}

			internal SegmentedDictionaryNode(int capacity)
			{
				this.Entries = new ISegmentedDictionaryEntry[capacity];
			}
		}

		internal class SegmentedDictionaryValues : ISegmentedDictionaryEntry
		{
			private TKey[] m_keys;

			private TValue[] m_values;

			private int m_count;

			public TKey[] Keys
			{
				get
				{
					return this.m_keys;
				}
			}

			public TValue[] Values
			{
				get
				{
					return this.m_values;
				}
			}

			public int Count
			{
				get
				{
					return this.m_count;
				}
				set
				{
					this.m_count = value;
				}
			}

			public int Capacity
			{
				get
				{
					return this.m_keys.Length;
				}
			}

			public SegmentedDictionaryEntryType EntryType
			{
				get
				{
					return SegmentedDictionaryEntryType.Values;
				}
			}

			public SegmentedDictionaryValues(int capacity)
			{
				this.m_count = 0;
				this.m_keys = new TKey[capacity];
				this.m_values = new TValue[capacity];
			}
		}

		internal struct SegmentedDictionaryEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IEnumerator
		{
			private class ContextItem<KeyType, ValueType>
			{
				public KeyType Key;

				public ValueType Value;

				public ContextItem(KeyType key, ValueType value)
				{
					this.Key = key;
					this.Value = value;
				}
			}

			private int m_currentValueIndex;

			private KeyValuePair<TKey, TValue> m_currentPair;

			private Stack<ContextItem<int, SegmentedDictionaryNode>> m_context;

			private int m_version;

			private SegmentedDictionary<TKey, TValue> m_dictionary;

			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					if (this.m_dictionary.m_version != this.m_version)
					{
						Global.Tracer.Assert(false, "SegmentedDictionaryEnumerator: Cannot use enumerator after modifying the underlying collection");
					}
					if (this.m_context.Count < 1)
					{
						Global.Tracer.Assert(false, "SegmentedDictionaryEnumerator: Enumerator beyond the bounds of the underlying collection");
					}
					return this.m_currentPair;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal SegmentedDictionaryEnumerator(SegmentedDictionary<TKey, TValue> dictionary)
			{
				this.m_dictionary = dictionary;
				this.m_version = dictionary.m_version;
				this.m_currentValueIndex = -1;
				this.m_currentPair = default(KeyValuePair<TKey, TValue>);
				this.m_context = null;
				this.Reset();
			}

			public void Dispose()
			{
				this.m_context = null;
				this.m_dictionary = null;
			}

			public bool MoveNext()
			{
				if (this.m_dictionary.m_version != this.m_version)
				{
					Global.Tracer.Assert(false, "SegmentedDictionaryEnumerator: Cannot use enumerator after modifying the underlying collection");
				}
				if (this.m_context.Count < 1 && this.m_currentValueIndex != -1)
				{
					return false;
				}
				if (this.m_context.Count == 0)
				{
					ContextItem<int, SegmentedDictionaryNode> item = new ContextItem<int, SegmentedDictionaryNode>(0, this.m_dictionary.m_root);
					this.m_currentValueIndex = 0;
					this.m_context.Push(item);
				}
				return this.FindNext();
			}

			private bool FindNext()
			{
				bool flag = false;
				while (this.m_context.Count > 0 && !flag)
				{
					ContextItem<int, SegmentedDictionaryNode> contextItem = this.m_context.Peek();
					flag = this.FindNext(contextItem.Value, contextItem);
				}
				return flag;
			}

			private bool FindNext(SegmentedDictionaryNode node, ContextItem<int, SegmentedDictionaryNode> curContext)
			{
				bool flag = false;
				while (!flag && curContext.Key < node.Entries.Length)
				{
					ISegmentedDictionaryEntry segmentedDictionaryEntry = node.Entries[curContext.Key];
					if (segmentedDictionaryEntry != null)
					{
						switch (segmentedDictionaryEntry.EntryType)
						{
						case SegmentedDictionaryEntryType.Node:
						{
							SegmentedDictionaryNode value = segmentedDictionaryEntry as SegmentedDictionaryNode;
							this.m_context.Push(new ContextItem<int, SegmentedDictionaryNode>(0, value));
							flag = this.FindNext();
							break;
						}
						case SegmentedDictionaryEntryType.Values:
						{
							SegmentedDictionaryValues segmentedDictionaryValues = segmentedDictionaryEntry as SegmentedDictionaryValues;
							if (this.m_currentValueIndex < segmentedDictionaryValues.Count)
							{
								this.m_currentPair = new KeyValuePair<TKey, TValue>(segmentedDictionaryValues.Keys[this.m_currentValueIndex], segmentedDictionaryValues.Values[this.m_currentValueIndex]);
								this.m_currentValueIndex++;
								return true;
							}
							this.m_currentValueIndex = 0;
							break;
						}
						default:
							Global.Tracer.Assert(false, "Unknown ObjectType");
							break;
						}
					}
					curContext.Key++;
				}
				if (!flag)
				{
					this.m_currentValueIndex = 0;
					this.m_context.Pop();
				}
				return flag;
			}

			public void Reset()
			{
				this.m_currentValueIndex = -1;
				this.m_context = new Stack<ContextItem<int, SegmentedDictionaryNode>>();
				this.m_version = this.m_dictionary.m_version;
			}
		}

		internal struct SegmentedDictionaryKeysEnumerator : IEnumerator<TKey>, IDisposable, IEnumerator
		{
			private SegmentedDictionary<TKey, TValue> m_dictionary;

			private IEnumerator<KeyValuePair<TKey, TValue>> m_enumerator;

			public TKey Current
			{
				get
				{
					return this.m_enumerator.Current.Key;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal SegmentedDictionaryKeysEnumerator(SegmentedDictionary<TKey, TValue> dictionary)
			{
				this.m_dictionary = dictionary;
				this.m_enumerator = dictionary.GetEnumerator();
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				return this.m_enumerator.MoveNext();
			}

			public void Reset()
			{
				this.m_enumerator.Reset();
			}
		}

		internal struct SegmentedDictionaryValuesEnumerator : IEnumerator<TValue>, IDisposable, IEnumerator
		{
			private SegmentedDictionary<TKey, TValue> m_dictionary;

			private IEnumerator<KeyValuePair<TKey, TValue>> m_enumerator;

			public TValue Current
			{
				get
				{
					return this.m_enumerator.Current.Value;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal SegmentedDictionaryValuesEnumerator(SegmentedDictionary<TKey, TValue> dictionary)
			{
				this.m_dictionary = dictionary;
				this.m_enumerator = dictionary.GetEnumerator();
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				return this.m_enumerator.MoveNext();
			}

			public void Reset()
			{
				this.m_enumerator.Reset();
			}
		}

		internal class SegmentedDictionaryKeysCollection : ICollection<TKey>, IEnumerable<TKey>, IEnumerable
		{
			private SegmentedDictionary<TKey, TValue> m_dictionary;

			public int Count
			{
				get
				{
					return this.m_dictionary.Count;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			internal SegmentedDictionaryKeysCollection(SegmentedDictionary<TKey, TValue> dictionary)
			{
				this.m_dictionary = dictionary;
			}

			public void Add(TKey item)
			{
				Global.Tracer.Assert(false, "SegmentedDictionaryKeysCollection: Dictionary keys collection is read only");
			}

			public void Clear()
			{
				Global.Tracer.Assert(false, "SegmentedDictionaryKeysCollection: Dictionary keys collection is read only");
			}

			public bool Contains(TKey item)
			{
				return this.m_dictionary.ContainsKey(item);
			}

			public void CopyTo(TKey[] array, int arrayIndex)
			{
				if (arrayIndex < 0)
				{
					Global.Tracer.Assert(false, "SegmentedDictionaryKeysCollection.CopyTo: Index must be greater than 0");
				}
				if (array == null)
				{
					Global.Tracer.Assert(false, "SegmentedDictionaryKeysCollection.CopyTo: Specified array must not be null");
				}
				if (array.Rank > 1)
				{
					Global.Tracer.Assert(false, "SegmentedDictionaryKeysCollection.CopyTo: Specified array must be 1 dimensional");
				}
				if (arrayIndex + this.Count > array.Length)
				{
					Global.Tracer.Assert(false, "SegmentedDictionaryKeysCollection.CopyTo: Insufficent space in destination array");
				}
				foreach (TKey item in this)
				{
					TKey val = array[arrayIndex] = item;
					arrayIndex++;
				}
			}

			public bool Remove(TKey item)
			{
				Global.Tracer.Assert(false, "SegmentedDictionaryKeysCollection.Remove: Dictionary keys collection is read only");
				return false;
			}

			public IEnumerator<TKey> GetEnumerator()
			{
				return (IEnumerator<TKey>)(object)new SegmentedDictionaryKeysEnumerator(this.m_dictionary);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
		}

		internal class SegmentedDictionaryValuesCollection : ICollection<TValue>, IEnumerable<TValue>, IEnumerable
		{
			private SegmentedDictionary<TKey, TValue> m_dictionary;

			public int Count
			{
				get
				{
					return this.m_dictionary.Count;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			internal SegmentedDictionaryValuesCollection(SegmentedDictionary<TKey, TValue> dictionary)
			{
				this.m_dictionary = dictionary;
			}

			public void Add(TValue item)
			{
				Global.Tracer.Assert(false, "SegmentedDictionaryValuesCollection.Add: Dictionary values collection is read only");
			}

			public void Clear()
			{
				Global.Tracer.Assert(false, "SegmentedDictionaryValuesCollection.Clear: Dictionary values collection is read only");
			}

			public bool Contains(TValue item)
			{
				return this.m_dictionary.ContainsValue(item);
			}

			public void CopyTo(TValue[] array, int arrayIndex)
			{
				if (arrayIndex < 0)
				{
					Global.Tracer.Assert(false, "SegmentedDictionaryValuesCollection.CopyTo: Index must be greater than 0");
				}
				if (array == null)
				{
					Global.Tracer.Assert(false, "SegmentedDictionaryValuesCollection.CopyTo: Specified array must not be null");
				}
				if (array.Rank > 1)
				{
					Global.Tracer.Assert(false, "SegmentedDictionaryValuesCollection.CopyTo: Specified array must be 1 dimensional");
				}
				if (arrayIndex + this.Count > array.Length)
				{
					Global.Tracer.Assert(false, "SegmentedDictionaryValuesCollection.CopyTo: Insufficent space in destination array");
				}
				foreach (TValue item in this)
				{
					TValue val = array[arrayIndex] = item;
					arrayIndex++;
				}
			}

			public bool Remove(TValue item)
			{
				Global.Tracer.Assert(false, "SegmentedDictionaryValuesCollection.Remove: Dictionary values collection is read only");
				return false;
			}

			public IEnumerator<TValue> GetEnumerator()
			{
				return (IEnumerator<TValue>)(object)new SegmentedDictionaryValuesEnumerator(this.m_dictionary);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
		}

		private int m_nodeCapacity;

		private int m_valuesCapacity;

		private IEqualityComparer<TKey> m_comparer;

		private int m_count;

		private int m_version;

		private SegmentedDictionaryNode m_root;

		private SegmentedDictionaryKeysCollection m_keysCollection;

		private SegmentedDictionaryValuesCollection m_valuesCollection;

		public ICollection<TKey> Keys
		{
			get
			{
				if (this.m_keysCollection == null)
				{
					this.m_keysCollection = new SegmentedDictionaryKeysCollection(this);
				}
				return this.m_keysCollection;
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				if (this.m_valuesCollection == null)
				{
					this.m_valuesCollection = new SegmentedDictionaryValuesCollection(this);
				}
				return this.m_valuesCollection;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				TValue result = default(TValue);
				if (!this.TryGetValue(key, out result))
				{
					Global.Tracer.Assert(false, "Given key is not present in the dictionary");
				}
				return result;
			}
			set
			{
				if (this.Insert(this.m_root, this.m_comparer.GetHashCode(key), key, value, false, 0))
				{
					this.m_count++;
				}
				this.m_version++;
			}
		}

		public IEqualityComparer<TKey> Comparer
		{
			get
			{
				return this.m_comparer;
			}
		}

		public int Count
		{
			get
			{
				return this.m_count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		internal SegmentedDictionary(int priority, IScalabilityCache cache)
			: this(23, 5, (IEqualityComparer<TKey>)null)
		{
		}

		internal SegmentedDictionary(int nodeCapacity, int entryCapacity)
			: this(nodeCapacity, entryCapacity, (IEqualityComparer<TKey>)null)
		{
		}

		internal SegmentedDictionary(int nodeCapacity, int entryCapacity, IEqualityComparer<TKey> comparer)
		{
			this.m_nodeCapacity = nodeCapacity;
			this.m_valuesCapacity = entryCapacity;
			this.m_comparer = comparer;
			this.m_version = 0;
			this.m_count = 0;
			if (this.m_comparer == null)
			{
				this.m_comparer = EqualityComparer<TKey>.Default;
			}
			this.m_root = this.BuildNode(0, this.m_nodeCapacity);
		}

		public void Add(TKey key, TValue value)
		{
			if (this.Insert(this.m_root, this.m_comparer.GetHashCode(key), key, value, true, 0))
			{
				this.m_count++;
			}
			this.m_version++;
		}

		public bool ContainsKey(TKey key)
		{
			TValue val = default(TValue);
			return this.TryGetValue(key, out val);
		}

		public bool Remove(TKey key)
		{
			int num = default(int);
			bool flag = this.Remove(this.m_root, this.m_comparer.GetHashCode(key), key, 0, out num);
			if (flag)
			{
				this.m_count--;
				this.m_version++;
			}
			return flag;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (key == null)
			{
				Global.Tracer.Assert(false, "SegmentedDictionary: Key cannot be null");
			}
			return this.Find(this.m_root, this.m_comparer.GetHashCode(key), key, 0, out value);
		}

		public bool ContainsValue(TValue value)
		{
			IEqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
			foreach (KeyValuePair<TKey, TValue> item in this)
			{
				if (@default.Equals(item.Value, value))
				{
					return true;
				}
			}
			return false;
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			this.Add(item.Key, item.Value);
		}

		public void Clear()
		{
			this.m_root = this.BuildNode(0, this.m_nodeCapacity);
			this.m_count = 0;
			this.m_version++;
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			IEqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
			TValue y = default(TValue);
			if (this.TryGetValue(item.Key, out y))
			{
				return @default.Equals(item.Value, y);
			}
			return false;
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			if (arrayIndex < 0)
			{
				Global.Tracer.Assert(false, "SegmentedDictionary.CopyTo: Index must be greater than 0");
			}
			if (array == null)
			{
				Global.Tracer.Assert(false, "SegmentedDictionary.CopyTo: Specified array must not be null");
			}
			if (array.Rank > 1)
			{
				Global.Tracer.Assert(false, "SegmentedDictionary.CopyTo: Specified array must be 1 dimensional", "array");
			}
			if (arrayIndex + this.Count > array.Length)
			{
				Global.Tracer.Assert(false, "SegmentedDictionary.CopyTo: Insufficent space in destination array");
			}
			foreach (KeyValuePair<TKey, TValue> item in this)
			{
				array[arrayIndex] = item;
				arrayIndex++;
			}
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			if (((ICollection<KeyValuePair<TKey, TValue>>)this).Contains(item))
			{
				return this.Remove(item.Key);
			}
			return false;
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return (IEnumerator<KeyValuePair<TKey, TValue>>)(object)new SegmentedDictionaryEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private SegmentedDictionaryNode BuildNode(int level, int capacity)
		{
			return new SegmentedDictionaryNode(capacity);
		}

		private bool Insert(SegmentedDictionaryNode node, int hashCode, TKey key, TValue value, bool add, int level)
		{
			bool flag = false;
			int num = this.HashToSlot(node, hashCode, level);
			ISegmentedDictionaryEntry segmentedDictionaryEntry = node.Entries[num];
			if (segmentedDictionaryEntry == null)
			{
				SegmentedDictionaryValues segmentedDictionaryValues = new SegmentedDictionaryValues(this.m_valuesCapacity);
				segmentedDictionaryValues.Keys[0] = key;
				segmentedDictionaryValues.Values[0] = value;
				segmentedDictionaryValues.Count++;
				node.Entries[num] = segmentedDictionaryValues;
				flag = true;
			}
			else
			{
				switch (segmentedDictionaryEntry.EntryType)
				{
				case SegmentedDictionaryEntryType.Node:
				{
					SegmentedDictionaryNode node2 = segmentedDictionaryEntry as SegmentedDictionaryNode;
					flag = this.Insert(node2, hashCode, key, value, add, level + 1);
					break;
				}
				case SegmentedDictionaryEntryType.Values:
				{
					SegmentedDictionaryValues segmentedDictionaryValues2 = segmentedDictionaryEntry as SegmentedDictionaryValues;
					bool flag2 = false;
					int num2 = 0;
					while (num2 < segmentedDictionaryValues2.Count)
					{
						if (!this.m_comparer.Equals(key, segmentedDictionaryValues2.Keys[num2]))
						{
							num2++;
							continue;
						}
						if (add)
						{
							Global.Tracer.Assert(false, "SegmentedDictionary: An element with the same key already exists within the Dictionary");
						}
						segmentedDictionaryValues2.Values[num2] = value;
						flag2 = true;
						flag = false;
						break;
					}
					if (!flag2)
					{
						if (segmentedDictionaryValues2.Count < segmentedDictionaryValues2.Capacity)
						{
							int count = segmentedDictionaryValues2.Count;
							segmentedDictionaryValues2.Keys[count] = key;
							segmentedDictionaryValues2.Values[count] = value;
							segmentedDictionaryValues2.Count++;
							flag = true;
						}
						else
						{
							SegmentedDictionaryNode segmentedDictionaryNode = this.BuildNode(level + 1, this.m_nodeCapacity);
							node.Entries[num] = segmentedDictionaryNode;
							for (int i = 0; i < segmentedDictionaryValues2.Count; i++)
							{
								TKey val = segmentedDictionaryValues2.Keys[i];
								this.Insert(segmentedDictionaryNode, this.m_comparer.GetHashCode(val), val, segmentedDictionaryValues2.Values[i], false, level + 1);
							}
							flag = this.Insert(segmentedDictionaryNode, hashCode, key, value, add, level + 1);
						}
					}
					break;
				}
				default:
					Global.Tracer.Assert(false, "Unknown ObjectType");
					break;
				}
			}
			if (flag)
			{
				node.Count++;
			}
			return flag;
		}

		private int HashToSlot(SegmentedDictionaryNode node, int hashCode, int level)
		{
			int prime = PrimeHelper.GetPrime(level);
			int hashInputA = PrimeHelper.GetHashInputA(level);
			int hashInputB = PrimeHelper.GetHashInputB(level);
			int num = Math.Abs(hashInputA * hashCode + hashInputB);
			return num % prime % node.Entries.Length;
		}

		private bool Find(SegmentedDictionaryNode node, int hashCode, TKey key, int level, out TValue value)
		{
			value = default(TValue);
			bool result = false;
			int num = this.HashToSlot(node, hashCode, level);
			ISegmentedDictionaryEntry segmentedDictionaryEntry = node.Entries[num];
			if (segmentedDictionaryEntry != null)
			{
				switch (segmentedDictionaryEntry.EntryType)
				{
				case SegmentedDictionaryEntryType.Node:
				{
					SegmentedDictionaryNode node2 = segmentedDictionaryEntry as SegmentedDictionaryNode;
					result = this.Find(node2, hashCode, key, level + 1, out value);
					break;
				}
				case SegmentedDictionaryEntryType.Values:
				{
					SegmentedDictionaryValues segmentedDictionaryValues = segmentedDictionaryEntry as SegmentedDictionaryValues;
					for (int i = 0; i < segmentedDictionaryValues.Count; i++)
					{
						if (this.m_comparer.Equals(key, segmentedDictionaryValues.Keys[i]))
						{
							value = segmentedDictionaryValues.Values[i];
							return true;
						}
					}
					break;
				}
				default:
					Global.Tracer.Assert(false, "Unknown ObjectType");
					break;
				}
			}
			return result;
		}

		private bool Remove(SegmentedDictionaryNode node, int hashCode, TKey key, int level, out int newCount)
		{
			bool flag = false;
			int num = this.HashToSlot(node, hashCode, level);
			ISegmentedDictionaryEntry segmentedDictionaryEntry = node.Entries[num];
			if (segmentedDictionaryEntry == null)
			{
				flag = false;
			}
			else
			{
				switch (segmentedDictionaryEntry.EntryType)
				{
				case SegmentedDictionaryEntryType.Node:
				{
					SegmentedDictionaryNode node2 = segmentedDictionaryEntry as SegmentedDictionaryNode;
					int num4 = default(int);
					flag = this.Remove(node2, hashCode, key, level + 1, out num4);
					if (flag && num4 == 0)
					{
						node.Entries[num] = null;
					}
					break;
				}
				case SegmentedDictionaryEntryType.Values:
				{
					SegmentedDictionaryValues segmentedDictionaryValues = segmentedDictionaryEntry as SegmentedDictionaryValues;
					int num2 = 0;
					while (num2 < segmentedDictionaryValues.Count)
					{
						if (!this.m_comparer.Equals(key, segmentedDictionaryValues.Keys[num2]))
						{
							num2++;
							continue;
						}
						if (segmentedDictionaryValues.Count == 1)
						{
							node.Entries[num] = null;
						}
						else
						{
							segmentedDictionaryValues.Keys[num2] = default(TKey);
							segmentedDictionaryValues.Values[num2] = default(TValue);
							segmentedDictionaryValues.Count--;
							int num3 = segmentedDictionaryValues.Count - num2;
							if (num3 > 0)
							{
								Array.Copy(segmentedDictionaryValues.Keys, num2 + 1, segmentedDictionaryValues.Keys, num2, num3);
								Array.Copy(segmentedDictionaryValues.Values, num2 + 1, segmentedDictionaryValues.Values, num2, num3);
							}
						}
						flag = true;
						break;
					}
					break;
				}
				default:
					Global.Tracer.Assert(false, "Unknown ObjectType");
					break;
				}
			}
			if (flag)
			{
				node.Count--;
			}
			newCount = node.Count;
			return flag;
		}
	}
}
