using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class LinkedBucketedQueue<T> : IEnumerable<T>, IEnumerable where T : class
	{
		private class QueueBucket
		{
			internal T[] Array;

			internal QueueBucket PreviousBucket;

			internal QueueBucket NextBucket;

			internal int Count;

			internal QueueBucket(int size)
			{
				this.Array = new T[size];
				this.PreviousBucket = null;
				this.NextBucket = null;
				this.Count = 0;
			}
		}

		private class QueueEnumerator : IDecumulator<T>, IEnumerator<T>, IDisposable, IEnumerator
		{
			private LinkedBucketedQueue<T> m_queue;

			private QueueBucket m_currentBucket;

			private int m_currentIndex;

			public T Current
			{
				get
				{
					return this.m_currentBucket.Array[this.m_currentIndex];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal QueueEnumerator(LinkedBucketedQueue<T> queue)
			{
				this.m_queue = queue;
				this.Reset();
			}

			public void RemoveCurrent()
			{
				this.m_currentBucket.Array[this.m_currentIndex] = null;
				this.m_currentBucket.Count--;
				this.m_queue.m_count--;
				if (this.m_currentBucket.Count == 0)
				{
					if (this.m_currentBucket == this.m_queue.m_firstBucket)
					{
						this.m_queue.RemoveFirstBucket();
						this.m_currentBucket = this.m_queue.m_firstBucket;
						this.m_currentIndex = -1;
					}
					else if (this.m_currentBucket == this.m_queue.m_lastBucket)
					{
						if (this.m_currentBucket.PreviousBucket == null)
						{
							this.m_queue.m_firstBucket = null;
							this.m_queue.m_lastBucket = null;
							this.m_queue.m_count = 0;
						}
						else
						{
							this.m_queue.m_lastBucket = this.m_currentBucket.PreviousBucket;
							this.m_queue.m_lastBucket.NextBucket = null;
							this.m_queue.m_insertIndex = this.m_queue.m_bucketSize;
							this.m_currentBucket.PreviousBucket = null;
						}
					}
					else
					{
						this.m_currentBucket.NextBucket.PreviousBucket = this.m_currentBucket.PreviousBucket;
						this.m_currentBucket.PreviousBucket.NextBucket = this.m_currentBucket.NextBucket;
						QueueBucket currentBucket = this.m_currentBucket;
						this.m_currentBucket = this.m_currentBucket.NextBucket;
						this.m_currentIndex = -1;
						currentBucket.NextBucket = null;
						currentBucket.PreviousBucket = null;
					}
				}
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				if (this.m_currentBucket == null)
				{
					this.m_currentBucket = this.m_queue.m_firstBucket;
					this.m_currentIndex = -1;
				}
				do
				{
					this.m_currentIndex++;
					if (this.m_currentBucket != null && this.m_currentIndex == this.m_queue.m_bucketSize)
					{
						this.m_currentBucket = this.m_currentBucket.NextBucket;
						this.m_currentIndex = 0;
					}
				}
				while (this.m_currentBucket != null && this.m_currentBucket.Array[this.m_currentIndex] == null);
				return this.m_currentBucket != null;
			}

			public void Reset()
			{
				this.m_currentBucket = null;
				this.m_currentIndex = -1;
			}
		}

		private QueueBucket m_firstBucket;

		private QueueBucket m_lastBucket;

		private int m_count;

		private int m_bucketSize = 20;

		private int m_insertIndex;

		private int m_removeIndex;

		internal int Count
		{
			get
			{
				return this.m_count;
			}
		}

		internal LinkedBucketedQueue(int bucketSize)
		{
			this.m_count = 0;
			this.m_bucketSize = bucketSize;
		}

		internal void Enqueue(T item)
		{
			if (this.m_firstBucket == null)
			{
				this.m_firstBucket = new QueueBucket(this.m_bucketSize);
				this.m_lastBucket = this.m_firstBucket;
				this.m_firstBucket.NextBucket = null;
				this.m_firstBucket.PreviousBucket = null;
				this.m_insertIndex = 0;
			}
			if (this.m_insertIndex == this.m_bucketSize)
			{
				QueueBucket lastBucket = this.m_lastBucket;
				this.m_lastBucket = new QueueBucket(this.m_bucketSize);
				this.m_lastBucket.NextBucket = null;
				this.m_lastBucket.PreviousBucket = lastBucket;
				lastBucket.NextBucket = this.m_lastBucket;
				this.m_insertIndex = 0;
			}
			this.m_lastBucket.Array[this.m_insertIndex] = item;
			this.m_lastBucket.Count++;
			this.m_insertIndex++;
			this.m_count++;
		}

		internal T Dequeue()
		{
			T val = null;
			while (val == null && this.m_count > 0)
			{
				val = this.m_firstBucket.Array[this.m_removeIndex];
				this.m_firstBucket.Array[this.m_removeIndex] = null;
				this.m_removeIndex++;
				if (val != null)
				{
					this.m_firstBucket.Count--;
					this.m_count--;
				}
				if (this.m_firstBucket.Count == 0)
				{
					this.RemoveFirstBucket();
				}
			}
			return val;
		}

		private void RemoveFirstBucket()
		{
			this.m_firstBucket = this.m_firstBucket.NextBucket;
			this.m_removeIndex = 0;
			if (this.m_firstBucket == null)
			{
				this.m_lastBucket = null;
			}
			else
			{
				this.m_firstBucket.PreviousBucket = null;
			}
		}

		internal void Clear()
		{
			this.m_count = 0;
			this.m_firstBucket = null;
			this.m_lastBucket = null;
		}

		public IDecumulator<T> GetDecumulator()
		{
			return new QueueEnumerator(this);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.GetDecumulator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetDecumulator();
		}
	}
}
