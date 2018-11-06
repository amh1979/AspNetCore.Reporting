using AspNetCore.ReportingServices.ReportProcessing;
using System.Diagnostics;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class AppendOnlySpaceManager : ISpaceManager
	{
		private long m_streamEnd;

		private long m_unuseableBytes;

		public long StreamEnd
		{
			get
			{
				return this.m_streamEnd;
			}
			set
			{
				this.m_streamEnd = value;
			}
		}

		internal AppendOnlySpaceManager()
		{
			this.m_streamEnd = 0L;
		}

		public void Seek(long offset, SeekOrigin origin)
		{
		}

		public void Free(long offset, long size)
		{
			this.m_unuseableBytes += size;
		}

		public long AllocateSpace(long size)
		{
			long streamEnd = this.m_streamEnd;
			this.m_streamEnd += size;
			return streamEnd;
		}

		public long Resize(long offset, long oldSize, long newSize)
		{
			this.Free(offset, oldSize);
			return this.AllocateSpace(newSize);
		}

		public void TraceStats()
		{
			Global.Tracer.Trace(TraceLevel.Verbose, "AppendOnlySpaceManager Stats. StreamSize: {0} MB. UnusuableSpace: {1} KB.", this.m_streamEnd / 1048576, this.m_unuseableBytes / 1024);
		}
	}
}
