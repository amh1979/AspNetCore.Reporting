using System.IO;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal class ChunkFactoryAdapter
	{
		private IChunkFactory m_chunkFactory;

		internal ChunkFactoryAdapter(IChunkFactory aFactory)
		{
			this.m_chunkFactory = aFactory;
		}

		public Stream CreateReportChunk(string name, ReportProcessing.ReportChunkTypes type, string mimeType)
		{
			return this.m_chunkFactory.CreateChunk(name, type, mimeType);
		}

		public Stream GetReportChunk(string name, ReportProcessing.ReportChunkTypes type, out string mimeType)
		{
			return this.m_chunkFactory.GetChunk(name, type, ChunkMode.Open, out mimeType);
		}

		public string GetChunkMimeType(string name, ReportProcessing.ReportChunkTypes type)
		{
			string result = default(string);
			Stream chunk = this.m_chunkFactory.GetChunk(name, type, ChunkMode.Open, out result);
			chunk.Close();
			return result;
		}
	}
}
