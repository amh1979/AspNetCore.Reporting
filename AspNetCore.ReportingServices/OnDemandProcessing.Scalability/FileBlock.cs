namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class FileBlock
	{
		private DynamicBucketedHeapSpaceManager m_spaceManager;

		internal FileBlock()
		{
			this.m_spaceManager = new DynamicBucketedHeapSpaceManager();
			this.m_spaceManager.AllowEndAllocation = false;
		}

		public void Free(long offset, long size)
		{
			this.m_spaceManager.Free(offset, size);
		}

		public long AllocateSpace(long size)
		{
			return this.m_spaceManager.AllocateSpace(size);
		}

		public long Resize(long offset, long oldSize, long newSize)
		{
			return this.m_spaceManager.Resize(offset, oldSize, newSize);
		}

		public void TraceStats(string desc)
		{
			this.m_spaceManager.TraceStats();
		}
	}
}
