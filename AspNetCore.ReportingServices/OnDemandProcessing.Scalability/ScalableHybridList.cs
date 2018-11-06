using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class ScalableHybridList<T> : IEnumerable<T>, IEnumerable, IDisposable
	{
		private sealed class HybridListEnumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			private ScalableHybridList<T> m_list;

			private int m_currentIndex = -1;

			private bool m_afterLast;

			private int m_version;

			public T Current
			{
				get
				{
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

			internal HybridListEnumerator(ScalableHybridList<T> list)
			{
				this.m_list = list;
				this.m_version = this.m_list.m_version;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				if (this.m_version != this.m_list.m_version)
				{
					Global.Tracer.Assert(false, "Cannot continue enumeration, backing list was modified");
				}
				if (this.m_afterLast)
				{
					return false;
				}
				if (this.m_currentIndex == -1)
				{
					this.m_currentIndex = this.m_list.First;
				}
				else
				{
					this.m_currentIndex = this.m_list.Next(this.m_currentIndex);
				}
				if (this.m_currentIndex == -1)
				{
					this.m_afterLast = true;
				}
				return !this.m_afterLast;
			}

			public void Reset()
			{
				this.m_currentIndex = -1;
				this.m_afterLast = false;
			}
		}

		internal const int InvalidIndex = -1;

		private int m_count;

		private ScalableList<ScalableHybridListEntry> m_entries;

		private int m_first = -1;

		private int m_last = -1;

		private int m_firstFree = -1;

		private int m_version;

		internal int Count
		{
			get
			{
				return this.m_count;
			}
		}

		internal T this[int index]
		{
			get
			{
				ScalableHybridListEntry andCheckEntry = this.GetAndCheckEntry(index);
				return (T)andCheckEntry.Item;
			}
		}

		internal int First
		{
			get
			{
				return this.m_first;
			}
		}

		internal int Last
		{
			get
			{
				return this.m_last;
			}
		}

		internal ScalableHybridList(int scalabilityPriority, IScalabilityCache cache, int segmentSize, int initialCapacity)
		{
			this.m_entries = new ScalableList<ScalableHybridListEntry>(scalabilityPriority, cache, segmentSize, initialCapacity);
		}

		internal int Add(T item)
		{
			int num = -1;
			if (this.m_firstFree != -1)
			{
				num = this.m_firstFree;
				ScalableHybridListEntry scalableHybridListEntry = default(ScalableHybridListEntry);
				using (this.m_entries.GetAndPin(this.m_firstFree, out scalableHybridListEntry))
				{
					this.m_firstFree = scalableHybridListEntry.Next;
					this.SetupLastNode(scalableHybridListEntry, item);
				}
			}
			else
			{
				num = this.m_entries.Count;
				ScalableHybridListEntry scalableHybridListEntry2 = new ScalableHybridListEntry();
				this.SetupLastNode(scalableHybridListEntry2, item);
				this.m_entries.Add(scalableHybridListEntry2);
			}
			if (this.m_count == 0)
			{
				this.m_first = num;
			}
			else
			{
				ScalableHybridListEntry scalableHybridListEntry3 = default(ScalableHybridListEntry);
				using (this.m_entries.GetAndPin(this.m_last, out scalableHybridListEntry3))
				{
					scalableHybridListEntry3.Next = num;
				}
			}
			this.m_last = num;
			this.m_count++;
			return num;
		}

		internal void Remove(int index)
		{
			ScalableHybridListEntry scalableHybridListEntry = default(ScalableHybridListEntry);
			using (this.m_entries.GetAndPin(index, out scalableHybridListEntry))
			{
				this.CheckNonFreeEntry(scalableHybridListEntry, index);
				if (scalableHybridListEntry.Previous == -1)
				{
					this.m_first = scalableHybridListEntry.Next;
				}
				else
				{
					ScalableHybridListEntry scalableHybridListEntry2 = default(ScalableHybridListEntry);
					using (this.m_entries.GetAndPin(scalableHybridListEntry.Previous, out scalableHybridListEntry2))
					{
						scalableHybridListEntry2.Next = scalableHybridListEntry.Next;
					}
				}
				if (scalableHybridListEntry.Next == -1)
				{
					this.m_last = scalableHybridListEntry.Previous;
				}
				else
				{
					ScalableHybridListEntry scalableHybridListEntry3 = default(ScalableHybridListEntry);
					using (this.m_entries.GetAndPin(scalableHybridListEntry.Next, out scalableHybridListEntry3))
					{
						scalableHybridListEntry3.Previous = scalableHybridListEntry.Previous;
					}
				}
				scalableHybridListEntry.Next = this.m_firstFree;
				this.m_firstFree = index;
				scalableHybridListEntry.Item = null;
				scalableHybridListEntry.Previous = -1;
				this.m_count--;
			}
		}

		internal int Next(int index)
		{
			ScalableHybridListEntry andCheckEntry = this.GetAndCheckEntry(index);
			return andCheckEntry.Next;
		}

		internal int Previous(int index)
		{
			ScalableHybridListEntry andCheckEntry = this.GetAndCheckEntry(index);
			return andCheckEntry.Previous;
		}

		public void Dispose()
		{
			this.Clear();
		}

		internal void Clear()
		{
			this.m_entries.Clear();
			this.m_count = 0;
			this.m_first = -1;
			this.m_last = -1;
			this.m_firstFree = -1;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new HybridListEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private void SetupLastNode(ScalableHybridListEntry entry, T item)
		{
			entry.Item = item;
			entry.Next = -1;
			entry.Previous = this.m_last;
		}

		private ScalableHybridListEntry GetAndCheckEntry(int index)
		{
			ScalableHybridListEntry scalableHybridListEntry = this.m_entries[index];
			this.CheckNonFreeEntry(scalableHybridListEntry, index);
			return scalableHybridListEntry;
		}

		private void CheckNonFreeEntry(ScalableHybridListEntry entry, int index)
		{
			if (entry.Previous == -1 && index != this.m_first)
			{
				Global.Tracer.Assert(false, "Cannot use index: {0}. It points to a free item", index);
			}
		}
	}
}
