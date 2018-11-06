namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class Block
	{
		private long m_offset;

		private long m_endOffset;

		private long m_size;

		public long Offset
		{
			get
			{
				return this.m_offset;
			}
			set
			{
				this.m_offset = value;
			}
		}

		public long EndOffset
		{
			get
			{
				return this.m_endOffset;
			}
			set
			{
				this.m_endOffset = value;
			}
		}

		public long Size
		{
			get
			{
				return this.m_size;
			}
			set
			{
				this.m_size = value;
			}
		}

		public Block(long offset, long size)
		{
			this.m_offset = offset;
			this.m_size = size;
		}
	}
}
