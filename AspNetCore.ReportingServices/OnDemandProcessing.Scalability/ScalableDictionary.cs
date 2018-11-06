using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class ScalableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IStorable, IPersistable
	{
		internal struct ScalableDictionaryEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IEnumerator
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

			private Stack<ContextItem<int, ScalableDictionaryNodeReference>> m_context;

			private int m_version;

			private ScalableDictionary<TKey, TValue> m_dictionary;

			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					if (this.m_dictionary.m_version != this.m_version)
					{
						Global.Tracer.Assert(false, "ScalableDictionaryEnumerator: Cannot use enumerator after modifying the underlying collection");
					}
					if (this.m_context.Count < 1)
					{
						Global.Tracer.Assert(false, "ScalableDictionaryEnumerator: Enumerator beyond the bounds of the underlying collection");
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

			internal ScalableDictionaryEnumerator(ScalableDictionary<TKey, TValue> dictionary)
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
					Global.Tracer.Assert(false, "ScalableDictionaryEnumerator: Cannot use enumerator after modifying the underlying collection");
				}
				if (this.m_context.Count < 1 && this.m_currentValueIndex != -1)
				{
					return false;
				}
				if (this.m_context.Count == 0)
				{
					ContextItem<int, ScalableDictionaryNodeReference> item = new ContextItem<int, ScalableDictionaryNodeReference>(0, this.m_dictionary.m_root);
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
					ContextItem<int, ScalableDictionaryNodeReference> contextItem = this.m_context.Peek();
					ScalableDictionaryNodeReference value = contextItem.Value;
					using (value.PinValue())
					{
						flag = this.FindNext(value.Value(), contextItem);
					}
				}
				return flag;
			}

			private bool FindNext(ScalableDictionaryNode node, ContextItem<int, ScalableDictionaryNodeReference> curContext)
			{
				bool flag = false;
				while (!flag && curContext.Key < node.Entries.Length)
				{
					IScalableDictionaryEntry scalableDictionaryEntry = node.Entries[curContext.Key];
					if (scalableDictionaryEntry != null)
					{
						switch (scalableDictionaryEntry.GetObjectType())
						{
						case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference:
						{
							ScalableDictionaryNodeReference value = scalableDictionaryEntry as ScalableDictionaryNodeReference;
							this.m_context.Push(new ContextItem<int, ScalableDictionaryNodeReference>(0, value));
							flag = this.FindNext();
							break;
						}
						case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryValues:
						{
							ScalableDictionaryValues scalableDictionaryValues = scalableDictionaryEntry as ScalableDictionaryValues;
							if (this.m_currentValueIndex < scalableDictionaryValues.Count)
							{
								this.m_currentPair = new KeyValuePair<TKey, TValue>((TKey)scalableDictionaryValues.Keys[this.m_currentValueIndex], (TValue)scalableDictionaryValues.Values[this.m_currentValueIndex]);
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
				this.m_context = new Stack<ContextItem<int, ScalableDictionaryNodeReference>>();
				this.m_version = this.m_dictionary.m_version;
			}
		}

		internal struct ScalableDictionaryKeysEnumerator : IEnumerator<TKey>, IDisposable, IEnumerator
		{
			private ScalableDictionary<TKey, TValue> m_dictionary;

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

			internal ScalableDictionaryKeysEnumerator(ScalableDictionary<TKey, TValue> dictionary)
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

		internal struct ScalableDictionaryValuesEnumerator : IEnumerator<TValue>, IDisposable, IEnumerator
		{
			private ScalableDictionary<TKey, TValue> m_dictionary;

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

			internal ScalableDictionaryValuesEnumerator(ScalableDictionary<TKey, TValue> dictionary)
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

		internal sealed class ScalableDictionaryKeysCollection : ICollection<TKey>, IEnumerable<TKey>, IEnumerable
		{
			private ScalableDictionary<TKey, TValue> m_dictionary;

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

			internal ScalableDictionaryKeysCollection(ScalableDictionary<TKey, TValue> dictionary)
			{
				this.m_dictionary = dictionary;
			}

			public void Add(TKey item)
			{
				Global.Tracer.Assert(false, "ScalableDictionaryKeysCollection: Dictionary keys collection is read only");
			}

			public void Clear()
			{
				Global.Tracer.Assert(false, "ScalableDictionaryKeysCollection: Dictionary keys collection is read only");
			}

			public bool Contains(TKey item)
			{
				return this.m_dictionary.ContainsKey(item);
			}

			public void CopyTo(TKey[] array, int arrayIndex)
			{
				if (arrayIndex < 0)
				{
					Global.Tracer.Assert(false, "ScalableDictionaryKeysCollection.CopyTo: Index must be greater than 0");
				}
				if (array == null)
				{
					Global.Tracer.Assert(false, "ScalableDictionaryKeysCollection.CopyTo: Specified array must not be null");
				}
				if (array.Rank > 1)
				{
					Global.Tracer.Assert(false, "ScalableDictionaryKeysCollection.CopyTo: Specified array must be 1 dimensional");
				}
				if (arrayIndex + this.Count > array.Length)
				{
					Global.Tracer.Assert(false, "ScalableDictionaryKeysCollection.CopyTo: Insufficent space in destination array");
				}
				foreach (TKey item in this)
				{
					TKey val = array[arrayIndex] = item;
					arrayIndex++;
				}
			}

			public bool Remove(TKey item)
			{
				Global.Tracer.Assert(false, "ScalableDictionaryKeysCollection.Remove: Dictionary keys collection is read only");
				return false;
			}

			public IEnumerator<TKey> GetEnumerator()
			{
				return (IEnumerator<TKey>)(object)new ScalableDictionaryKeysEnumerator(this.m_dictionary);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
		}

		internal sealed class ScalableDictionaryValuesCollection : ICollection<TValue>, IEnumerable<TValue>, IEnumerable
		{
			private ScalableDictionary<TKey, TValue> m_dictionary;

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

			internal ScalableDictionaryValuesCollection(ScalableDictionary<TKey, TValue> dictionary)
			{
				this.m_dictionary = dictionary;
			}

			public void Add(TValue item)
			{
				Global.Tracer.Assert(false, "ScalableDictionaryValuesCollection.Add: Dictionary values collection is read only");
			}

			public void Clear()
			{
				Global.Tracer.Assert(false, "ScalableDictionaryValuesCollection.Clear: Dictionary values collection is read only");
			}

			public bool Contains(TValue item)
			{
				return this.m_dictionary.ContainsValue(item);
			}

			public void CopyTo(TValue[] array, int arrayIndex)
			{
				if (arrayIndex < 0)
				{
					Global.Tracer.Assert(false, "ScalableDictionaryValuesCollection.CopyTo: Index must be greater than 0");
				}
				if (array == null)
				{
					Global.Tracer.Assert(false, "ScalableDictionaryValuesCollection.CopyTo: Specified array must not be null");
				}
				if (array.Rank > 1)
				{
					Global.Tracer.Assert(false, "ScalableDictionaryValuesCollection.CopyTo: Specified array must be 1 dimensional");
				}
				if (arrayIndex + this.Count > array.Length)
				{
					Global.Tracer.Assert(false, "ScalableDictionaryValuesCollection.CopyTo: Insufficent space in destination array");
				}
				foreach (TValue item in this)
				{
					TValue val = array[arrayIndex] = item;
					arrayIndex++;
				}
			}

			public bool Remove(TValue item)
			{
				Global.Tracer.Assert(false, "ScalableDictionaryValuesCollection.Remove: Dictionary values collection is read only");
				return false;
			}

			public IEnumerator<TValue> GetEnumerator()
			{
				return (IEnumerator<TValue>)(object)new ScalableDictionaryValuesEnumerator(this.m_dictionary);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
		}

		private int m_nodeCapacity;

		private int m_valuesCapacity;

		[StaticReference]
		private IEqualityComparer<TKey> m_comparer;

		private int m_count;

		private int m_version;

		private ScalableDictionaryNodeReference m_root;

		private bool m_useFixedReferences;

		private int m_priority;

		[NonSerialized]
		private IScalabilityCache m_scalabilityCache;

		[NonSerialized]
		private ScalableDictionaryKeysCollection m_keysCollection;

		[NonSerialized]
		private ScalableDictionaryValuesCollection m_valuesCollection;

		[NonSerialized]
		private static Declaration m_declaration = ScalableDictionary<TKey, TValue>.GetDeclaration();

		public ICollection<TKey> Keys
		{
			get
			{
				if (this.m_keysCollection == null)
				{
					this.m_keysCollection = new ScalableDictionaryKeysCollection(this);
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
					this.m_valuesCollection = new ScalableDictionaryValuesCollection(this);
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
				IDisposable disposable = default(IDisposable);
				if (this.Insert(this.m_root, this.GetHashCode(key), key, value, false, 0, true, out disposable))
				{
					this.m_count++;
				}
				this.m_version++;
				disposable.Dispose();
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

		public int Size
		{
			get
			{
				return 8 + ItemSizes.ReferenceSize + 4 + 4 + ItemSizes.SizeOf(this.m_root) + ItemSizes.ReferenceSize + ItemSizes.ReferenceSize + ItemSizes.ReferenceSize + ItemSizes.ReferenceSize + 4 + 1;
			}
		}

		public ScalableDictionary()
		{
		}

		internal ScalableDictionary(int priority, IScalabilityCache cache)
			: this(priority, cache, 23, 5, (IEqualityComparer<TKey>)null)
		{
		}

		internal ScalableDictionary(int priority, IScalabilityCache cache, int nodeCapacity, int entryCapacity)
			: this(priority, cache, nodeCapacity, entryCapacity, (IEqualityComparer<TKey>)null)
		{
		}

		internal ScalableDictionary(int priority, IScalabilityCache cache, int nodeCapacity, int entryCapacity, IEqualityComparer<TKey> comparer)
			: this(priority, cache, nodeCapacity, entryCapacity, comparer, false)
		{
		}

		internal ScalableDictionary(int priority, IScalabilityCache cache, int nodeCapacity, int entryCapacity, IEqualityComparer<TKey> comparer, bool useFixedReferences)
		{
			this.m_priority = priority;
			this.m_scalabilityCache = cache;
			this.m_nodeCapacity = nodeCapacity;
			this.m_valuesCapacity = entryCapacity;
			this.m_comparer = comparer;
			this.m_version = 0;
			this.m_count = 0;
			this.m_useFixedReferences = useFixedReferences;
			if (this.m_comparer == null)
			{
				this.m_comparer = EqualityComparer<TKey>.Default;
			}
			this.m_root = this.BuildNode(0, this.m_nodeCapacity);
		}

		public void Add(TKey key, TValue value)
		{
			IDisposable disposable = this.AddAndPin(key, value);
			disposable.Dispose();
		}

		public IDisposable AddAndPin(TKey key, TValue value)
		{
			IDisposable result = default(IDisposable);
			if (this.Insert(this.m_root, this.GetHashCode(key), key, value, true, 0, true, out result))
			{
				this.m_count++;
			}
			this.m_version++;
			return result;
		}

		public bool ContainsKey(TKey key)
		{
			TValue val = default(TValue);
			return this.TryGetValue(key, out val);
		}

		public bool Remove(TKey key)
		{
			int num = default(int);
			bool flag = this.Remove(this.m_root, this.GetHashCode(key), key, 0, out num);
			if (flag)
			{
				this.m_count--;
				this.m_version++;
			}
			return flag;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			IDisposable disposable = default(IDisposable);
			bool flag = this.TryGetAndPin(key, out value, out disposable);
			if (flag)
			{
				disposable.Dispose();
			}
			return flag;
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

		public bool TryGetAndPin(TKey key, out TValue value, out IDisposable reference)
		{
			if (key == null)
			{
				Global.Tracer.Assert(false, "ScalableDictionary: Key cannot be null");
			}
			return this.Find(this.m_root, this.GetHashCode(key), key, 0, out value, out reference);
		}

		public IDisposable GetAndPin(TKey key, out TValue value)
		{
			IDisposable result = default(IDisposable);
			bool condition = this.TryGetAndPin(key, out value, out result);
			Global.Tracer.Assert(condition, "Missing expected dictionary item with key");
			return result;
		}

		public void TransferTo(IScalabilityCache scaleCache)
		{
			this.m_root = (ScalableDictionaryNodeReference)this.m_root.TransferTo(scaleCache);
			this.m_scalabilityCache = scaleCache;
		}

		public void UpdateComparer(IEqualityComparer<TKey> comparer)
		{
			Global.Tracer.Assert(this.m_comparer == null, "Cannot update equality comparer in the middle of a table computation.");
			this.m_comparer = comparer;
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			this.Add(item.Key, item.Value);
		}

		public void Clear()
		{
			this.FreeChildren(this.m_root);
			this.m_count = 0;
			this.m_version++;
		}

		public void Dispose()
		{
			if ((BaseReference)this.m_root != (object)null)
			{
				this.Clear();
				this.m_root.Free();
			}
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
				Global.Tracer.Assert(false, "ScalableDictionary.CopyTo: Index must be greater than 0");
			}
			if (array == null)
			{
				Global.Tracer.Assert(false, "ScalableDictionary.CopyTo: Specified array must not be null");
			}
			if (array.Rank > 1)
			{
				Global.Tracer.Assert(false, "ScalableDictionary.CopyTo: Specified array must be 1 dimensional", "array");
			}
			if (arrayIndex + this.Count > array.Length)
			{
				Global.Tracer.Assert(false, "ScalableDictionary.CopyTo: Insufficent space in destination array");
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
			return (IEnumerator<KeyValuePair<TKey, TValue>>)(object)new ScalableDictionaryEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private int GetHashCode(TKey key)
		{
			if (key != null && !(((object)key) is DBNull))
			{
				return this.m_comparer.GetHashCode(key);
			}
			return DBNull.Value.GetHashCode();
		}

		private void FreeChildren(ScalableDictionaryNodeReference nodeRef)
		{
			using (nodeRef.PinValue())
			{
				ScalableDictionaryNode scalableDictionaryNode = nodeRef.Value();
				for (int i = 0; i < scalableDictionaryNode.Entries.Length; i++)
				{
					IScalableDictionaryEntry scalableDictionaryEntry = scalableDictionaryNode.Entries[i];
					if (scalableDictionaryEntry != null)
					{
						switch (scalableDictionaryEntry.GetObjectType())
						{
						case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference:
						{
							ScalableDictionaryNodeReference scalableDictionaryNodeReference = scalableDictionaryEntry as ScalableDictionaryNodeReference;
							this.FreeChildren(scalableDictionaryNodeReference);
							scalableDictionaryNodeReference.Free();
							break;
						}
						default:
							Global.Tracer.Assert(false, "Unknown ObjectType");
							break;
						case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryValues:
							break;
						}
					}
					scalableDictionaryNode.Entries[i] = null;
				}
				scalableDictionaryNode.Count = 0;
			}
		}

		private ScalableDictionaryNodeReference BuildNode(int level, int capacity)
		{
			ScalableDictionaryNode scalableDictionaryNode = new ScalableDictionaryNode(capacity);
			if (this.m_useFixedReferences)
			{
				return (ScalableDictionaryNodeReference)this.m_scalabilityCache.GenerateFixedReference<ScalableDictionaryNode>(scalableDictionaryNode);
			}
			return (ScalableDictionaryNodeReference)this.m_scalabilityCache.Allocate<ScalableDictionaryNode>(scalableDictionaryNode, this.m_priority, scalableDictionaryNode.EmptySize);
		}

		private bool Insert(ScalableDictionaryNodeReference nodeRef, int hashCode, TKey key, TValue value, bool add, int level, bool updateSize, out IDisposable cleanupRef)
		{
			IDisposable disposable = nodeRef.PinValue();
			ScalableDictionaryNode scalableDictionaryNode = nodeRef.Value();
			bool flag = false;
			int num = this.HashToSlot(scalableDictionaryNode, hashCode, level);
			IScalableDictionaryEntry scalableDictionaryEntry = scalableDictionaryNode.Entries[num];
			if (scalableDictionaryEntry == null)
			{
				ScalableDictionaryValues scalableDictionaryValues = new ScalableDictionaryValues(this.m_valuesCapacity);
				scalableDictionaryValues.Keys[0] = key;
				scalableDictionaryValues.Values[0] = value;
				scalableDictionaryValues.Count++;
				scalableDictionaryNode.Entries[num] = scalableDictionaryValues;
				flag = true;
				cleanupRef = disposable;
				if (!this.m_useFixedReferences && updateSize)
				{
					int sizeBytesDelta = ItemSizes.SizeOfInObjectArray(key) + ItemSizes.SizeOfInObjectArray(value) + scalableDictionaryValues.EmptySize;
					nodeRef.UpdateSize(sizeBytesDelta);
				}
			}
			else
			{
				switch (scalableDictionaryEntry.GetObjectType())
				{
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference:
				{
					ScalableDictionaryNodeReference nodeRef2 = scalableDictionaryEntry as ScalableDictionaryNodeReference;
					flag = this.Insert(nodeRef2, hashCode, key, value, add, level + 1, updateSize, out cleanupRef);
					break;
				}
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryValues:
				{
					ScalableDictionaryValues scalableDictionaryValues2 = scalableDictionaryEntry as ScalableDictionaryValues;
					bool flag2 = false;
					cleanupRef = null;
					int num2 = 0;
					while (num2 < scalableDictionaryValues2.Count)
					{
						if (!this.m_comparer.Equals(key, (TKey)scalableDictionaryValues2.Keys[num2]))
						{
							num2++;
							continue;
						}
						if (add)
						{
							Global.Tracer.Assert(false, "ScalableDictionary: An element with the same key already exists within the Dictionary");
						}
						scalableDictionaryValues2.Values[num2] = value;
						flag2 = true;
						flag = false;
						cleanupRef = disposable;
						break;
					}
					if (!flag2)
					{
						if (scalableDictionaryValues2.Count < scalableDictionaryValues2.Capacity)
						{
							int count = scalableDictionaryValues2.Count;
							scalableDictionaryValues2.Keys[count] = key;
							scalableDictionaryValues2.Values[count] = value;
							scalableDictionaryValues2.Count++;
							flag = true;
							cleanupRef = disposable;
							if (!this.m_useFixedReferences && updateSize)
							{
								nodeRef.UpdateSize(ItemSizes.SizeOfInObjectArray(key));
								nodeRef.UpdateSize(ItemSizes.SizeOfInObjectArray(value));
							}
						}
						else
						{
							ScalableDictionaryNodeReference scalableDictionaryNodeReference = this.BuildNode(level + 1, this.m_nodeCapacity);
							scalableDictionaryNode.Entries[num] = scalableDictionaryNodeReference;
							using (scalableDictionaryNodeReference.PinValue())
							{
								if (!this.m_useFixedReferences && updateSize)
								{
									int num3 = ItemSizes.SizeOfInObjectArray(scalableDictionaryValues2);
									nodeRef.UpdateSize(num3 * -1);
									scalableDictionaryNodeReference.UpdateSize(num3);
								}
								for (int i = 0; i < scalableDictionaryValues2.Count; i++)
								{
									TKey key2 = (TKey)scalableDictionaryValues2.Keys[i];
									IDisposable disposable2 = default(IDisposable);
									this.Insert(scalableDictionaryNodeReference, this.GetHashCode(key2), key2, (TValue)scalableDictionaryValues2.Values[i], false, level + 1, false, out disposable2);
									disposable2.Dispose();
								}
								flag = this.Insert(scalableDictionaryNodeReference, hashCode, key, value, add, level + 1, updateSize, out cleanupRef);
							}
						}
					}
					break;
				}
				default:
					Global.Tracer.Assert(false, "Unknown ObjectType");
					cleanupRef = null;
					break;
				}
			}
			if (flag)
			{
				scalableDictionaryNode.Count++;
			}
			if (disposable != cleanupRef)
			{
				disposable.Dispose();
			}
			return flag;
		}

		private int HashToSlot(ScalableDictionaryNode node, int hashCode, int level)
		{
			int prime = PrimeHelper.GetPrime(level);
			int hashInputA = PrimeHelper.GetHashInputA(level);
			int hashInputB = PrimeHelper.GetHashInputB(level);
			int num = Math.Abs(hashInputA * hashCode + hashInputB);
			return num % prime % node.Entries.Length;
		}

		private bool Find(ScalableDictionaryNodeReference nodeRef, int hashCode, TKey key, int level, out TValue value, out IDisposable containingNodeRef)
		{
			containingNodeRef = null;
			IDisposable disposable = nodeRef.PinValue();
			ScalableDictionaryNode scalableDictionaryNode = nodeRef.Value();
			value = default(TValue);
			bool result = false;
			int num = this.HashToSlot(scalableDictionaryNode, hashCode, level);
			IScalableDictionaryEntry scalableDictionaryEntry = scalableDictionaryNode.Entries[num];
			if (scalableDictionaryEntry != null)
			{
				switch (scalableDictionaryEntry.GetObjectType())
				{
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference:
				{
					ScalableDictionaryNodeReference nodeRef2 = scalableDictionaryEntry as ScalableDictionaryNodeReference;
					result = this.Find(nodeRef2, hashCode, key, level + 1, out value, out containingNodeRef);
					break;
				}
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryValues:
				{
					ScalableDictionaryValues scalableDictionaryValues = scalableDictionaryEntry as ScalableDictionaryValues;
					for (int i = 0; i < scalableDictionaryValues.Count; i++)
					{
						if (this.m_comparer.Equals(key, (TKey)scalableDictionaryValues.Keys[i]))
						{
							value = (TValue)scalableDictionaryValues.Values[i];
							containingNodeRef = disposable;
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
			disposable.Dispose();
			return result;
		}

		private bool Remove(ScalableDictionaryNodeReference nodeRef, int hashCode, TKey key, int level, out int newCount)
		{
			using (nodeRef.PinValue())
			{
				ScalableDictionaryNode scalableDictionaryNode = nodeRef.Value();
				bool flag = false;
				int num = this.HashToSlot(scalableDictionaryNode, hashCode, level);
				IScalableDictionaryEntry scalableDictionaryEntry = scalableDictionaryNode.Entries[num];
				if (scalableDictionaryEntry == null)
				{
					flag = false;
				}
				else
				{
					switch (scalableDictionaryEntry.GetObjectType())
					{
					case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference:
					{
						ScalableDictionaryNodeReference scalableDictionaryNodeReference = scalableDictionaryEntry as ScalableDictionaryNodeReference;
						int num4 = default(int);
						flag = this.Remove(scalableDictionaryNodeReference, hashCode, key, level + 1, out num4);
						if (flag && num4 == 0)
						{
							scalableDictionaryNode.Entries[num] = null;
							scalableDictionaryNodeReference.Free();
						}
						break;
					}
					case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryValues:
					{
						ScalableDictionaryValues scalableDictionaryValues = scalableDictionaryEntry as ScalableDictionaryValues;
						int num2 = 0;
						while (num2 < scalableDictionaryValues.Count)
						{
							if (!this.m_comparer.Equals(key, (TKey)scalableDictionaryValues.Keys[num2]))
							{
								num2++;
								continue;
							}
							if (scalableDictionaryValues.Count == 1)
							{
								scalableDictionaryNode.Entries[num] = null;
							}
							else
							{
								scalableDictionaryValues.Keys[num2] = null;
								scalableDictionaryValues.Values[num2] = null;
								scalableDictionaryValues.Count--;
								int num3 = scalableDictionaryValues.Count - num2;
								if (num3 > 0)
								{
									Array.Copy(scalableDictionaryValues.Keys, num2 + 1, scalableDictionaryValues.Keys, num2, num3);
									Array.Copy(scalableDictionaryValues.Values, num2 + 1, scalableDictionaryValues.Values, num2, num3);
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
					scalableDictionaryNode.Count--;
				}
				newCount = scalableDictionaryNode.Count;
				return flag;
			}
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ScalableDictionary<TKey, TValue>.m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.NodeCapacity:
					writer.Write(this.m_nodeCapacity);
					break;
				case MemberName.ValuesCapacity:
					writer.Write(this.m_valuesCapacity);
					break;
				case MemberName.Comparer:
				{
					int value = -2147483648;
					if (scalabilityCache.CacheType == ScalabilityCacheType.Standard)
					{
						value = scalabilityCache.StoreStaticReference(this.m_comparer);
					}
					writer.Write(value);
					break;
				}
				case MemberName.Count:
					writer.Write(this.m_count);
					break;
				case MemberName.Version:
					writer.Write(this.m_version);
					break;
				case MemberName.Root:
					writer.Write(this.m_root);
					break;
				case MemberName.UseFixedReferences:
					writer.Write(this.m_useFixedReferences);
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
			reader.RegisterDeclaration(ScalableDictionary<TKey, TValue>.m_declaration);
			IScalabilityCache scalabilityCache = this.m_scalabilityCache = (reader.PersistenceHelper as IScalabilityCache);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.NodeCapacity:
					this.m_nodeCapacity = reader.ReadInt32();
					break;
				case MemberName.ValuesCapacity:
					this.m_valuesCapacity = reader.ReadInt32();
					break;
				case MemberName.Comparer:
				{
					int id = reader.ReadInt32();
					if (scalabilityCache.CacheType == ScalabilityCacheType.Standard)
					{
						this.m_comparer = (IEqualityComparer<TKey>)scalabilityCache.FetchStaticReference(id);
					}
					break;
				}
				case MemberName.Count:
					this.m_count = reader.ReadInt32();
					break;
				case MemberName.Version:
					this.m_version = reader.ReadInt32();
					break;
				case MemberName.Root:
					this.m_root = (ScalableDictionaryNodeReference)reader.ReadRIFObject();
					break;
				case MemberName.UseFixedReferences:
					this.m_useFixedReferences = reader.ReadBoolean();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionary;
		}

		public static Declaration GetDeclaration()
		{
			if (ScalableDictionary<TKey, TValue>.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.NodeCapacity, Token.Int32));
				list.Add(new MemberInfo(MemberName.ValuesCapacity, Token.Int32));
				list.Add(new MemberInfo(MemberName.Comparer, Token.Int32));
				list.Add(new MemberInfo(MemberName.Count, Token.Int32));
				list.Add(new MemberInfo(MemberName.Version, Token.Int32));
				list.Add(new MemberInfo(MemberName.Root, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference));
				list.Add(new MemberInfo(MemberName.UseFixedReferences, Token.Boolean));
				list.Add(new MemberInfo(MemberName.Priority, Token.Int32));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionary, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return ScalableDictionary<TKey, TValue>.m_declaration;
		}
	}
}
