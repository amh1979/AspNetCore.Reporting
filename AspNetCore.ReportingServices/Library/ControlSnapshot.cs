using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Library
{
	[Serializable]
	internal sealed class ControlSnapshot : SnapshotBase, IChunkFactory, IDisposable
	{
		[Serializable]
		private class Chunk : IDisposable
		{
			private ChunkHeader m_header;

			private ChunkMemoryStream m_stream = new ChunkMemoryStream();

			public ChunkHeader Header
			{
				get
				{
					return this.m_header;
				}
			}

			public ChunkMemoryStream Stream
			{
				get
				{
					return this.m_stream;
				}
			}

			public Chunk(string mimeType, string name, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type)
			{
				this.m_header = new ChunkHeader(name, (int)type, ChunkFlags.None, mimeType, ChunkHeader.CurrentVersion, 0L);
			}

			public Chunk(Chunk baseChunk)
			{
				this.m_header = new ChunkHeader(baseChunk.Header);
				this.m_stream = baseChunk.Stream;
			}

			public void Dispose()
			{
				if (this.m_stream != null)
				{
					this.m_stream.Dispose();
					this.m_stream = null;
				}
				GC.SuppressFinalize(this);
			}
		}

		private IList<Chunk> m_allChunks = new List<Chunk>();

		public ReportProcessingFlags ReportProcessingFlags
		{
			get
			{
				return ReportProcessingFlags.OnDemandEngine;
			}
		}

		public ControlSnapshot()
			: base(false)
		{
		}

		public void Dispose()
		{
			this.DeleteSnapshotAndChunks();
			GC.SuppressFinalize(this);
		}

		public override SnapshotBase Duplicate()
		{
			throw new NotImplementedException();
		}

		private void CopyAllChunksTo(ControlSnapshot target)
		{
			foreach (Chunk allChunk in this.m_allChunks)
			{
				if (!target.m_allChunks.Contains(allChunk))
				{
					target.m_allChunks.Add(allChunk);
				}
			}
		}

		public void CopyDataChunksTo(IChunkFactory chunkFactory, out bool hasDataChunks)
		{
			hasDataChunks = false;
			foreach (Chunk allChunk in this.m_allChunks)
			{
				if (allChunk.Header.ChunkType == 5)
				{
					hasDataChunks = true;
					using (Stream to = chunkFactory.CreateChunk(allChunk.Header.ChunkName, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Data, null))
					{
						allChunk.Stream.Position = 0L;
						StreamSupport.CopyStreamUsingBuffer(allChunk.Stream, to, 4096);
					}
				}
			}
		}

		public override void PrepareExecutionSnapshot(SnapshotBase target, string compiledDefinitionChunkName)
		{
			ControlSnapshot controlSnapshot = (ControlSnapshot)target;
			int imageChunkTypeToCopy = (int)AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.GetImageChunkTypeToCopy(ReportProcessingFlags.OnDemandEngine);
			string text = compiledDefinitionChunkName;
			if (text == null)
			{
				text = "CompiledDefinition";
			}
			foreach (Chunk allChunk in this.m_allChunks)
			{
				ChunkHeader header = allChunk.Header;
				if (header.ChunkType == imageChunkTypeToCopy)
				{
					controlSnapshot.m_allChunks.Add(allChunk);
				}
				else if (header.ChunkName.Equals("CompiledDefinition", StringComparison.Ordinal))
				{
					Chunk chunk = new Chunk(allChunk);
					chunk.Header.ChunkName = text;
					controlSnapshot.m_allChunks.Add(chunk);
				}
				else if (header.ChunkType == 5 || header.ChunkType == 9)
				{
					Chunk item = new Chunk(allChunk);
					controlSnapshot.m_allChunks.Add(item);
				}
			}
		}

		[Obsolete("Use PrepareExecutionSnapshot instead")]
		public override void CopyImageChunksTo(SnapshotBase target)
		{
			ControlSnapshot target2 = (ControlSnapshot)target;
			this.CopyAllChunksTo(target2);
		}

		public override void DeleteSnapshotAndChunks()
		{
			foreach (Chunk allChunk in this.m_allChunks)
			{
				allChunk.Stream.CanBeClosed = true;
				allChunk.Dispose();
			}
			this.m_allChunks.Clear();
		}

		public override Stream GetChunk(string name, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type, out string mimeType)
		{
			Chunk chunkImpl = this.GetChunkImpl(name, type);
			if (chunkImpl == null)
			{
				mimeType = null;
				return null;
			}
			mimeType = chunkImpl.Header.MimeType;
			chunkImpl.Stream.Seek(0L, SeekOrigin.Begin);
			if (chunkImpl.Header.ChunkFlag == ChunkFlags.Compressed)
			{
				throw new InternalCatalogException("Cannot read compressed chunk.");
			}
			return chunkImpl.Stream;
		}

		public override string GetStreamMimeType(string name, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type)
		{
			Chunk chunkImpl = this.GetChunkImpl(name, type);
			if (chunkImpl == null)
			{
				return null;
			}
			return chunkImpl.Header.MimeType;
		}

		public override Stream CreateChunk(string name, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type, string mimeType)
		{
			this.Erase(name, type);
			Chunk chunk = new Chunk(mimeType, name, type);
			this.m_allChunks.Add(chunk);
			return chunk.Stream;
		}

		public Stream GetChunk(string chunkName, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes chunkType, ChunkMode chunkMode, out string mimeType)
		{
			Stream stream = ((SnapshotBase)this).GetChunk(chunkName, chunkType, out mimeType);
			if (chunkMode == ChunkMode.OpenOrCreate && stream == null)
			{
				mimeType = null;
				stream = this.CreateChunk(chunkName, chunkType, mimeType);
			}
			return stream;
		}

		public bool Erase(string chunkName, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type)
		{
			foreach (Chunk allChunk in this.m_allChunks)
			{
				if (allChunk.Header.ChunkName == chunkName && allChunk.Header.ChunkType == (int)type)
				{
					this.m_allChunks.Remove(allChunk);
					return true;
				}
			}
			return false;
		}

		private Chunk GetChunkImpl(string name, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type)
		{
			foreach (Chunk allChunk in this.m_allChunks)
			{
				if (allChunk.Header.ChunkName == name && allChunk.Header.ChunkType == (int)type)
				{
					return allChunk;
				}
			}
			return null;
		}
	}
}
