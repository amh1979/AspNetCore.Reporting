using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;
using System.Threading;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal abstract class BaseScalabilityCache : IScalabilityCache, PersistenceHelper, IDisposable
	{
		internal const long DefaultCacheExpansionMaxBytes = 15728640L;

		internal const long CacheExpansionMinBytes = 1048576L;

		protected bool m_disposed;

		protected long m_minReservedMemoryKB;

		protected long m_cacheSizeBytes;

		protected long m_cacheCapacityBytes = BaseScalabilityCache.DefaultCacheCapacityBytes;

		protected long m_totalAuditedBytes;

		protected long m_totalFreedBytes;

		protected int m_pendingNotificationCount;

		protected long m_inProgressFreeBytes;

		protected long m_pendingFreeBytes;

		protected bool m_freeingSpace;

		protected bool m_inStreamOper;

		protected IStorage m_storage;

		protected IReferenceCreator m_referenceCreator;

		protected ComponentType m_ownerComponent;

		protected Stopwatch m_serializationTimer = new Stopwatch();

		protected Stopwatch m_deserializationTimer = new Stopwatch();

		protected long m_totalBytesSerialized;

		protected long m_lastExpansionOrNotificationMs = -1L;

		protected long m_expansionIntervalMs;

		protected Stopwatch m_cacheLifetimeTimer;

		protected double m_cacheExpansionRatio;

		protected bool m_receivedShrinkRequest;

		protected long m_peakCacheSizeBytes = -1L;

		protected static long DefaultCacheCapacityBytes = -1L;

		protected static readonly long CacheExpansionMaxBytes = BaseScalabilityCache.ComputeMaxExpansionBytes(Environment.ProcessorCount);

		public IStorage Storage
		{
			get
			{
				return this.m_storage;
			}
		}

		public abstract ScalabilityCacheType CacheType
		{
			get;
		}

		public ComponentType OwnerComponent
		{
			get
			{
				return this.m_ownerComponent;
			}
		}

		public long SerializationDurationMs
		{
			get
			{
				return this.ReadTime(this.m_serializationTimer);
			}
		}

		public long DeserializationDurationMs
		{
			get
			{
				return this.ReadTime(this.m_deserializationTimer);
			}
		}

		public long ScalabilityDurationMs
		{
			get
			{
				return this.SerializationDurationMs + this.DeserializationDurationMs;
			}
		}

		public long PeakMemoryUsageKBytes
		{
			get
			{
				long num = Math.Max(this.m_peakCacheSizeBytes, this.m_cacheSizeBytes);
				return num / 1024;
			}
		}

		public long CacheSizeKBytes
		{
			get
			{
				return this.m_cacheSizeBytes / 1024;
			}
		}

		protected long MinReservedMemoryKB
		{
			get
			{
				return this.m_minReservedMemoryKB;
			}
		}

		protected abstract long InternalFreeableBytes
		{
			get;
		}

		internal long TotalSerializedHeapBytes
		{
			get
			{
				return this.m_totalBytesSerialized;
			}
		}

		internal BaseScalabilityCache(IStorage storage, long cacheExpansionIntervalMs, double cacheExpansionRatio, ComponentType ownerComponent, long minReservedMemoryBytes)
		{
			this.m_cacheSizeBytes = 0L;
			this.m_storage = storage;
			this.m_referenceCreator = storage.ReferenceCreator;
			storage.ScalabilityCache = this;
			Global.Tracer.Assert(cacheExpansionIntervalMs > 0, "CacheExpansionIntervalMs must be greater than 0");
			this.m_expansionIntervalMs = cacheExpansionIntervalMs;
			this.m_cacheExpansionRatio = cacheExpansionRatio;
			this.m_ownerComponent = ownerComponent;
			this.m_minReservedMemoryKB = minReservedMemoryBytes / 1024;
		}

		public abstract IReference<T> Allocate<T>(T obj, int priority) where T : IStorable;

		public abstract IReference<T> Allocate<T>(T obj, int priority, int initialSize) where T : IStorable;

		public abstract IReference<T> AllocateAndPin<T>(T obj, int priority) where T : IStorable;

		public abstract IReference<T> AllocateAndPin<T>(T obj, int priority, int initialSize) where T : IStorable;

		public abstract IReference<T> GenerateFixedReference<T>(T obj) where T : IStorable;

		public void Close()
		{
			this.Dispose();
		}

		public abstract int StoreStaticReference(object item);

		public abstract object FetchStaticReference(int id);

		public abstract IReference PoolReference(IReference reference);

		public virtual void Dispose()
		{
			try
			{
				if (!this.m_disposed)
				{
					this.m_disposed = true;
					if (this.m_serializationTimer != null && Global.Tracer.TraceVerbose)
					{
						long num = -1L;
						if (this.m_storage != null)
						{
							num = this.m_storage.StreamSize;
						}
						long num2 = this.m_totalBytesSerialized / 1024;
						double num3 = (double)this.SerializationDurationMs / 1000.0;
						Global.Tracer.Trace(TraceLevel.Verbose, "ScalabilityCache {0}|{1} Done. AuditedHeapSerialized: {2} KB. Serialization Time {3} s. AvgSpeed {4:F2} KB/s. StreamSize {5} MB. FinalAuditedHeapSize {6} KB. LifetimeFreedHeapSize {7} KB.", this.OwnerComponent, this.CacheType, num2, num3, (double)num2 / num3, num / 1048576, this.m_totalAuditedBytes / 1024, this.m_totalFreedBytes / 1024);
					}
					if (this.m_storage != null)
					{
						this.m_storage.Close();
						this.m_storage = null;
					}
					this.m_serializationTimer = null;
					this.m_deserializationTimer = null;
				}
			}
			finally
			{
				this.m_cacheSizeBytes = 0L;
				this.m_cacheCapacityBytes = 0L;
			}
		}

		internal abstract void Free(BaseReference reference);

		internal abstract IStorable Retrieve(BaseReference reference);

		[DebuggerStepThrough]
		internal virtual void ReferenceValueCallback(BaseReference reference)
		{
			this.PeriodicOperationCheck();
		}

		internal abstract void UnPin(BaseReference reference);

		internal abstract void Pin(BaseReference reference);

		internal abstract void ReferenceSerializeCallback(BaseReference reference);

		internal abstract void UpdateTargetSize(BaseReference reference, int sizeDeltaBytes);

		internal abstract BaseReference TransferTo(BaseReference reference);

		[DebuggerStepThrough]
		internal void PeriodicOperationCheck()
		{
			if (this.m_pendingNotificationCount > 0)
			{
				this.FreeCacheSpace(0, this.InternalFreeableBytes);
			}
		}

		protected abstract void FulfillInProgressFree();

		protected BaseReference CreateReference(IStorable storable)
		{
			BaseReference result = default(BaseReference);
			if (!this.m_referenceCreator.TryCreateReference(storable, out result))
			{
				Global.Tracer.Assert(false, "Cannot create reference to: {0}", storable);
			}
			return result;
		}

		[Conditional("DEBUG")]
		protected void CheckDisposed(string opName)
		{
			bool disposed = this.m_disposed;
		}

		protected void UpdatePeakCacheSize()
		{
			this.m_peakCacheSizeBytes = Math.Max(this.m_peakCacheSizeBytes, this.m_cacheSizeBytes);
		}

		protected void FreeCacheSpace(int count, long freeableBytes)
		{
			if (!this.m_freeingSpace && !this.m_inStreamOper)
			{
				int num = 0;
				try
				{
					this.m_inStreamOper = true;
					this.m_freeingSpace = true;
					long num2 = Interlocked.Read(ref this.m_cacheCapacityBytes);
					long num3;
					if (num2 > 0)
					{
						num = Interlocked.Exchange(ref this.m_pendingNotificationCount, 0);
						Interlocked.Exchange(ref this.m_pendingFreeBytes, 0L);
						num3 = freeableBytes + count - num2;
						if (num3 > 0)
						{
							if (num == 0 && this.m_cacheLifetimeTimer != null && this.m_cacheLifetimeTimer.ElapsedMilliseconds - this.m_expansionIntervalMs > this.m_lastExpansionOrNotificationMs)
							{
								this.ResetExpansionOrNotificationInterval();
								long num4 = this.m_totalAuditedBytes - num2;
								long num5 = (long)((double)num4 * this.m_cacheExpansionRatio);
								if (num5 > BaseScalabilityCache.CacheExpansionMaxBytes)
								{
									num5 = BaseScalabilityCache.CacheExpansionMaxBytes;
								}
								else if (num5 < 1048576)
								{
									num5 = 1048576L;
									this.m_totalAuditedBytes += 1048576L;
								}
								if (num5 > 0)
								{
									num2 = Interlocked.Add(ref this.m_cacheCapacityBytes, num5);
									if (Global.Tracer.TraceVerbose)
									{
										Global.Tracer.Trace(TraceLevel.Verbose, "ScalabilityCache {0}|{1} expanding cache. ExpansionKB: {2} TotalAllocation: {3} CacheSizeKB: {4} CacheCapacityKB: {5}", this.OwnerComponent, this.CacheType, num5 / 1024, this.m_totalAuditedBytes / 1024, this.m_cacheSizeBytes / 1024, this.m_cacheCapacityBytes / 1024);
									}
									num3 -= num5;
									if (num3 > 0)
									{
										goto IL_01a4;
									}
									goto end_IL_0013;
								}
							}
							goto IL_01a4;
						}
					}
					goto end_IL_0013;
					IL_01a4:
					Stopwatch stopwatch = null;
					if (num > 0 && Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "ScalabilityCache {0}|{1} responding to pressure.  PendingNotifications: {2} CacheSizeKB: {3} CacheCapacityKB: {4}", this.OwnerComponent, this.CacheType, num, this.m_cacheSizeBytes / 1024, this.m_cacheCapacityBytes / 1024);
						stopwatch = new Stopwatch();
						stopwatch.Start();
					}
					this.m_serializationTimer.Start();
					this.m_inProgressFreeBytes = num3;
					this.FulfillInProgressFree();
					this.m_serializationTimer.Stop();
					long num6 = num3 - this.m_inProgressFreeBytes;
					this.m_totalBytesSerialized += num6;
					this.UpdatePeakCacheSize();
					if (num > 0 && Global.Tracer.TraceVerbose)
					{
						stopwatch.Stop();
						double num7 = (double)stopwatch.ElapsedMilliseconds / 1000.0;
						long num8 = num6 / 1024;
						double num9 = (double)num8 / num7;
						Global.Tracer.Trace(TraceLevel.Verbose, "ScalabilityCache {0}|{1} done responding to pressure.  Freed: {2} KB. Speed: {3:F2} KB/Sec.  CacheSizeKB {4} CacheCapacityKB {5}", this.OwnerComponent, this.CacheType, num8, num9, this.m_cacheSizeBytes / 1024, this.m_cacheCapacityBytes / 1024);
					}
					end_IL_0013:;
				}
				finally
				{
					this.m_freeingSpace = false;
					this.m_inStreamOper = false;
					this.m_inProgressFreeBytes = 0L;
					if (num > 0)
					{
						this.ResetExpansionOrNotificationInterval();
					}
				}
			}
		}

		private long ReadTime(Stopwatch timer)
		{
			long num = 0L;
			if (timer != null)
			{
				num = timer.ElapsedMilliseconds;
			}
			if (num < 0)
			{
				num = -1L;
			}
			return num;
		}

		private void ResetExpansionOrNotificationInterval()
		{
			if (this.m_cacheLifetimeTimer == null)
			{
				this.m_cacheLifetimeTimer = new Stopwatch();
				this.m_cacheLifetimeTimer.Start();
				Random random = new Random();
				this.m_expansionIntervalMs += random.Next(1500);
			}
			this.m_lastExpansionOrNotificationMs = this.m_cacheLifetimeTimer.ElapsedMilliseconds;
		}

		private static void SetDefaultCacheCapacityBytes(long defaultCapacityBytes)
		{
			BaseScalabilityCache.DefaultCacheCapacityBytes = defaultCapacityBytes;
		}

		internal static long ComputeMaxExpansionBytes(int processorCount)
		{
			long num = Math.Max(1, processorCount / 4 * 2);
			long val = 15728640 / num;
			return Math.Max(val, 1048576L);
		}
	}
}
