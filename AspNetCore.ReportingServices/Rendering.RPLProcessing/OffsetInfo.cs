namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal class OffsetInfo : IRPLObjectFactory
	{
		protected long m_endOffset = -1L;

		internal RPLContext m_context;

		public long EndOffset
		{
			get
			{
				return this.m_endOffset;
			}
		}

		internal OffsetInfo(long endOffset, RPLContext context)
		{
			this.m_endOffset = endOffset;
			this.m_context = context;
		}
	}
}
