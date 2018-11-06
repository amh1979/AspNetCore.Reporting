using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class LinkedPriorityQueue<T> where T : class
	{
		internal struct PriorityQueueDecumulator : IDecumulator<T>, IEnumerator<T>, IDisposable, IEnumerator
		{
			private IDecumulator<T> m_currentLevelDecumulator;

			private PriorityLevel m_currentLevel;

			private LinkedPriorityQueue<T> m_queue;

			private IEnumerator<PriorityLevel> m_enumerator;

			private List<int> m_pendingLevelRemovals;

			public T Current
			{
				get
				{
					return this.m_currentLevelDecumulator.Current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal PriorityQueueDecumulator(LinkedPriorityQueue<T> queue)
			{
				this.m_queue = queue;
				this.m_enumerator = (IEnumerator<PriorityLevel>)(object)this.m_queue.m_priorityLevels.Values.GetEnumerator();
				this.m_pendingLevelRemovals = new List<int>();
				this.m_currentLevelDecumulator = null;
				this.m_currentLevel = null;
			}

			public bool MoveNext()
			{
				if (this.m_currentLevel != null && (this.m_currentLevelDecumulator == null || this.m_currentLevelDecumulator.MoveNext()))
				{
					return true;
				}
				if (!this.m_enumerator.MoveNext())
				{
					return false;
				}
				this.m_currentLevel = this.m_enumerator.Current;
				this.m_currentLevelDecumulator = this.m_currentLevel.Queue.GetDecumulator();
				return this.m_currentLevelDecumulator.MoveNext();
			}

			public void RemoveCurrent()
			{
				this.m_currentLevelDecumulator.RemoveCurrent();
				if (this.m_currentLevel.Queue.Count == 0)
				{
					this.m_pendingLevelRemovals.Add(this.m_currentLevel.Priority);
					this.m_currentLevel = null;
				}
			}

			public void Dispose()
			{
				this.m_enumerator.Dispose();
				this.m_enumerator = null;
				for (int i = 0; i < this.m_pendingLevelRemovals.Count; i++)
				{
					this.m_queue.m_priorityLevels.Remove(this.m_pendingLevelRemovals[i]);
				}
				this.m_queue.m_pendingDecumulatorCommit = false;
			}

			public void Reset()
			{
				Global.Tracer.Assert(false, "Reset is not supported");
			}
		}

		internal class PriorityLevel
		{
			public LinkedBucketedQueue<T> Queue;

			public int Priority;
		}

		private const int QueueBucketSize = 100;

		private SortedDictionary<int, PriorityLevel> m_priorityLevels;

		private bool m_pendingDecumulatorCommit;

		internal int LevelCount
		{
			get
			{
				Global.Tracer.Assert(!this.m_pendingDecumulatorCommit, "Cannot perform operations on the queue until the open enumerator is Disposed");
				return this.m_priorityLevels.Count;
			}
		}

		internal LinkedPriorityQueue()
		{
			this.m_priorityLevels = new SortedDictionary<int, PriorityLevel>(EqualityComparers.ReversedInt32ComparerInstance);
		}

		internal void Enqueue(T item, int priority)
		{
			Global.Tracer.Assert(!this.m_pendingDecumulatorCommit, "Cannot perform operations on the queue until the open enumerator is Disposed");
			PriorityLevel priorityLevel = default(PriorityLevel);
			if (!this.m_priorityLevels.TryGetValue(priority, out priorityLevel))
			{
				priorityLevel = new PriorityLevel();
				priorityLevel.Priority = priority;
				priorityLevel.Queue = new LinkedBucketedQueue<T>(100);
				this.m_priorityLevels[priority] = priorityLevel;
			}
			priorityLevel.Queue.Enqueue(item);
		}

		internal T Dequeue()
		{
			Global.Tracer.Assert(!this.m_pendingDecumulatorCommit, "Cannot perform operations on the queue until the open enumerator is Disposed");
			using (IDecumulator<T> decumulator = this.GetDecumulator())
			{
				decumulator.MoveNext();
				T current = decumulator.Current;
				decumulator.RemoveCurrent();
				return current;
			}
		}

		internal IDecumulator<T> GetDecumulator()
		{
			Global.Tracer.Assert(!this.m_pendingDecumulatorCommit, "Cannot perform operations on the queue until the open enumerator is Disposed");
			this.m_pendingDecumulatorCommit = true;
			return (IDecumulator<T>)(object)new PriorityQueueDecumulator(this);
		}

		internal void Clear()
		{
			this.m_priorityLevels.Clear();
		}
	}
}
