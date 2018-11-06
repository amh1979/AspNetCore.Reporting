using AspNetCore.ReportingServices.ReportProcessing;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class ChunkFactoryStreamHandler : IStreamHandler
	{
		private string m_chunkName;

		private AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes m_chunkType;

		private IChunkFactory m_chunkFactory;

		private bool m_existingChunk;

		internal ChunkFactoryStreamHandler(string chunkName, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes chunkType, IChunkFactory chunkFactory, bool existingChunk)
		{
			this.m_chunkName = chunkName;
			this.m_chunkType = chunkType;
			this.m_chunkFactory = chunkFactory;
			this.m_existingChunk = existingChunk;
		}

		public Stream OpenStream()
		{
			if (this.m_existingChunk)
			{
				string text = default(string);
				return this.m_chunkFactory.GetChunk(this.m_chunkName, this.m_chunkType, ChunkMode.Open, out text);
			}
			return this.m_chunkFactory.CreateChunk(this.m_chunkName, this.m_chunkType, null);
		}
	}
}
