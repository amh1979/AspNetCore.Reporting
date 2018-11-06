namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class ItemBoundaries
	{
		private long m_startOffset;

		private long m_endOffset;

		internal long StartOffset
		{
			get
			{
				return this.m_startOffset;
			}
		}

		internal long EndOffset
		{
			get
			{
				return this.m_endOffset;
			}
		}

		internal ItemBoundaries(long start, long end)
		{
			this.m_startOffset = start;
			this.m_endOffset = end;
		}
	}
}
