using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class RenderingChunkManager
	{
		private string m_rendererID;

		private IChunkFactory m_chunkFactory;

		private Dictionary<string, Stream> m_chunks;

		internal RenderingChunkManager(string rendererID, IChunkFactory chunkFactory)
		{
			this.m_rendererID = rendererID;
			this.m_chunkFactory = chunkFactory;
			this.m_chunks = new Dictionary<string, Stream>();
		}

		internal Stream GetOrCreateChunk(AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type, string chunkName, bool createChunkIfNotExists, out bool isNewChunk)
		{
			isNewChunk = false;
			if (chunkName == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, "chunkName");
			}
			if (this.m_chunks.ContainsKey(chunkName))
			{
				return this.m_chunks[chunkName];
			}
			string text = default(string);
			Stream stream = this.m_chunkFactory.GetChunk(chunkName, type, ChunkMode.Open, out text);
			if (createChunkIfNotExists && stream == null)
			{
				stream = this.m_chunkFactory.CreateChunk(chunkName, type, null);
				isNewChunk = true;
			}
			if (stream != null)
			{
				this.m_chunks.Add(chunkName, stream);
			}
			return stream;
		}

		internal Stream CreateChunk(AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type, string chunkName)
		{
			if (chunkName == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, "chunkName");
			}
			Stream stream = default(Stream);
			if (this.m_chunks.TryGetValue(chunkName, out stream))
			{
				stream.Close();
				this.m_chunks.Remove(chunkName);
			}
			stream = this.m_chunkFactory.CreateChunk(chunkName, type, null);
			if (stream != null)
			{
				this.m_chunks.Add(chunkName, stream);
			}
			return stream;
		}

		internal void CloseAllChunks()
		{
			foreach (Stream value in this.m_chunks.Values)
			{
				value.Close();
			}
			this.m_chunks.Clear();
		}
	}
}
