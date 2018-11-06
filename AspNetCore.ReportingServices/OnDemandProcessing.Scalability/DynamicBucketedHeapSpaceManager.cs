using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class DynamicBucketedHeapSpaceManager : ISpaceManager
	{
		private const int DefaultSpacesPerBucket = 2500;

		private const int DefaultMaxBucketCount = 10;

		private const int DefaultBucketSplitThreshold = 50;

		private const int DefaultMinimumTrackedSize = 35;

		private bool m_allowEndAllocation = true;

		private SortedBucket[] m_buckets;

		private int m_bucketCount;

		private long m_end;

		private long m_unuseableBytes;

		private int m_maxSpacesPerBucket;

		private int m_maxBucketCount;

		private int m_bucketSplitThreshold;

		private int m_minimumTrackedSize;

		public long StreamEnd
		{
			get
			{
				return this.m_end;
			}
			set
			{
				this.m_end = value;
			}
		}

		public bool AllowEndAllocation
		{
			get
			{
				return this.m_allowEndAllocation;
			}
			set
			{
				this.m_allowEndAllocation = value;
			}
		}

		public long UnuseableBytes
		{
			get
			{
				return this.m_unuseableBytes;
			}
		}

		internal SortedBucket[] Buckets
		{
			get
			{
				return this.m_buckets;
			}
		}

		internal int BucketCount
		{
			get
			{
				return this.m_bucketCount;
			}
		}

		internal DynamicBucketedHeapSpaceManager()
			: this(50, 10, 2500, 35)
		{
		}

		internal DynamicBucketedHeapSpaceManager(int splitThreshold, int maxBucketCount, int maxSpacesPerBucket, int minTrackedSizeBytes)
		{
			this.m_bucketSplitThreshold = splitThreshold;
			this.m_maxBucketCount = maxBucketCount;
			this.m_maxSpacesPerBucket = maxSpacesPerBucket;
			this.m_minimumTrackedSize = minTrackedSizeBytes;
			this.m_buckets = new SortedBucket[this.m_maxBucketCount];
			SortedBucket sortedBucket = new SortedBucket(this.m_maxSpacesPerBucket)
			{
				Limit = 0
			};
			this.m_buckets[0] = sortedBucket;
			this.m_bucketCount++;
		}

		public void Seek(long offset, SeekOrigin origin)
		{
		}

		public void Free(long offset, long size)
		{
			Space space = new Space(offset, size);
			this.InsertSpace(space);
		}

		private void InsertSpace(Space space)
		{
			if (space.Size < this.m_minimumTrackedSize)
			{
				this.m_unuseableBytes += space.Size;
			}
			else
			{
				int bucketIndex = this.GetBucketIndex(space.Size);
				SortedBucket sortedBucket = this.m_buckets[bucketIndex];
				if (sortedBucket.Count == this.m_maxSpacesPerBucket)
				{
					if (this.m_bucketCount < this.m_maxBucketCount && sortedBucket.Maximum - sortedBucket.Minimum > this.m_bucketSplitThreshold)
					{
						SortedBucket sortedBucket2 = sortedBucket.Split(this.m_maxSpacesPerBucket);
						for (int num = this.m_bucketCount; num > bucketIndex + 1; num--)
						{
							this.m_buckets[num] = this.m_buckets[num - 1];
						}
						this.m_buckets[bucketIndex + 1] = sortedBucket2;
						this.m_bucketCount++;
						this.InsertSpace(space);
					}
					else if (sortedBucket.Peek().Size < space.Size)
					{
						Space space2 = sortedBucket.ExtractMax();
						this.m_unuseableBytes += space2.Size;
						sortedBucket.Insert(space);
					}
					else
					{
						this.m_unuseableBytes += space.Size;
					}
				}
				else
				{
					sortedBucket.Insert(space);
				}
			}
		}

		private int GetBucketIndex(long size)
		{
			for (int i = 1; i < this.m_bucketCount; i++)
			{
				if (this.m_buckets[i].Limit > size)
				{
					return i - 1;
				}
			}
			return this.m_bucketCount - 1;
		}

		public long AllocateSpace(long size)
		{
			long num = -1L;
			for (int i = this.GetBucketIndex(size); i < this.m_bucketCount; i++)
			{
				if (num != -1)
				{
					break;
				}
				SortedBucket sortedBucket = this.m_buckets[i];
				if (sortedBucket.Count > 0)
				{
					Space space = sortedBucket.Peek();
					if (space.Size >= size)
					{
						sortedBucket.ExtractMax();
						num = space.Offset;
						space.Offset += size;
						space.Size -= size;
						if (space.Size > 0)
						{
							this.InsertSpace(space);
						}
						if (sortedBucket.Count == 0 && i != 0)
						{
							Array.Copy(this.m_buckets, i + 1, this.m_buckets, i, this.m_bucketCount - i - 1);
							this.m_bucketCount--;
						}
					}
				}
			}
			if (num == -1 && this.m_allowEndAllocation)
			{
				num = this.m_end;
				this.m_end += size;
			}
			return num;
		}

		public long Resize(long offset, long oldSize, long newSize)
		{
			this.Free(offset, oldSize);
			return this.AllocateSpace(newSize);
		}

		public void TraceStats()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.m_bucketCount; i++)
			{
				SortedBucket sortedBucket = this.m_buckets[i];
				stringBuilder.AppendFormat("\r\n\t\tBucket: Limit: {0} Count: {1}", sortedBucket.Limit, sortedBucket.Count);
			}
			Global.Tracer.Trace(TraceLevel.Verbose, "DynamicBucketedHeapSpaceManager Stats. StreamSize: {0} MB. UnusableSpace: {1} KB. \r\n\tBucketInfo: {2}", this.m_end / 1048576, this.m_unuseableBytes / 1024, stringBuilder.ToString());
		}
	}
}
