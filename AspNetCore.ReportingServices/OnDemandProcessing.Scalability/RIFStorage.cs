using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class RIFStorage : IStorage, IDisposable
	{
		private PageBufferedStream m_stream;

		private MemoryStream m_memoryStream;

		private IntermediateFormatWriter m_writer;

		private IntermediateFormatReader m_reader;

		private int m_bufferPageSize;

		private int m_bufferPageCount;

		private int m_tempStreamSize;

		private IScalabilityCache m_scalabilityCache;

		private IStreamHandler m_streamCreator;

		private ISpaceManager m_spaceManager;

		private IReferenceCreator m_referenceCreator;

		private UnifiedObjectCreator m_unifiedObjectCreator;

		private bool m_fromExistingStream;

		private GlobalIDOwnerCollection m_globalIdsFromOtherStream;

		private bool m_freezeAllocations;

		private readonly int m_rifCompatVersion;

		public long StreamSize
		{
			get
			{
				return this.m_spaceManager.StreamEnd;
			}
		}

		public IScalabilityCache ScalabilityCache
		{
			get
			{
				return this.m_scalabilityCache;
			}
			set
			{
				this.m_scalabilityCache = value;
				this.m_unifiedObjectCreator.ScalabilityCache = value;
			}
		}

		public IReferenceCreator ReferenceCreator
		{
			get
			{
				return this.m_referenceCreator;
			}
		}

		public bool FreezeAllocations
		{
			get
			{
				return this.m_freezeAllocations;
			}
			set
			{
				this.m_freezeAllocations = value;
				if (this.m_stream != null)
				{
					this.m_stream.FreezePageAllocations = value;
				}
			}
		}

		public RIFStorage(IStreamHandler streamHandler, int bufferPageSize, int bufferPageCount, int tempStreamSize, ISpaceManager spaceManager, IScalabilityObjectCreator appObjectCreator, IReferenceCreator appReferenceCreator, GlobalIDOwnerCollection globalIdsFromOtherStream, bool fromExistingStream, int rifCompatVersion)
		{
			this.m_streamCreator = streamHandler;
			this.m_scalabilityCache = null;
			this.m_bufferPageSize = bufferPageSize;
			this.m_bufferPageCount = bufferPageCount;
			this.m_tempStreamSize = tempStreamSize;
			this.m_stream = null;
			this.m_spaceManager = spaceManager;
			this.m_unifiedObjectCreator = new UnifiedObjectCreator(appObjectCreator, appReferenceCreator);
			this.m_referenceCreator = new UnifiedReferenceCreator(appReferenceCreator);
			this.m_fromExistingStream = fromExistingStream;
			this.m_globalIdsFromOtherStream = globalIdsFromOtherStream;
			this.m_rifCompatVersion = rifCompatVersion;
		}

		private void SetupStorage()
		{
			if (this.m_stream == null)
			{
				Stream stream = this.m_streamCreator.OpenStream();
				this.m_streamCreator = null;
				this.m_stream = new PageBufferedStream(stream, this.m_bufferPageSize, this.m_bufferPageCount);
				this.m_stream.FreezePageAllocations = this.m_freezeAllocations;
				List<Declaration> declarations = this.m_unifiedObjectCreator.GetDeclarations();
				this.m_memoryStream = new MemoryStream(this.m_tempStreamSize);
				this.m_writer = new IntermediateFormatWriter(this.m_memoryStream, declarations, this.m_scalabilityCache, this.m_rifCompatVersion);
				if (this.m_fromExistingStream)
				{
					this.m_spaceManager.StreamEnd = this.m_stream.Length;
					this.m_reader = new IntermediateFormatReader(this.m_stream, this.m_unifiedObjectCreator, this.m_globalIdsFromOtherStream, this.m_scalabilityCache, declarations, IntermediateFormatVersion.Current, PersistenceFlags.Seekable);
				}
				else
				{
					this.m_spaceManager.StreamEnd = this.m_stream.Position;
					this.m_reader = new IntermediateFormatReader(this.m_stream, this.m_unifiedObjectCreator, this.m_globalIdsFromOtherStream, this.m_scalabilityCache, declarations, IntermediateFormatVersion.Current, PersistenceFlags.Seekable);
				}
			}
		}

		public IPersistable Retrieve(long offset)
		{
			long num = default(long);
			return this.Retrieve(offset, out num);
		}

		public IPersistable Retrieve(long offset, out long persistedSize)
		{
			this.SetupStorage();
			this.Seek(offset, SeekOrigin.Begin);
			IPersistable persistable = this.m_reader.ReadRIFObject();
			persistedSize = this.CalculatePersistedSize(persistable, offset);
			return persistable;
		}

		public T Retrieve<T>(long offset, out long persistedSize) where T : IPersistable, new()
		{
			this.SetupStorage();
			this.Seek(offset, SeekOrigin.Begin);
			T val = this.m_reader.ReadRIFObject<T>();
			persistedSize = this.CalculatePersistedSize((IPersistable)(object)val, offset);
			return val;
		}

		private long CalculatePersistedSize(IPersistable item, long offset)
		{
			return this.m_stream.Position - offset;
		}

		public long Allocate(IPersistable obj)
		{
			return this.WriteObject(obj, -1L, -1L);
		}

		private long SeekToFreeSpace(long size)
		{
			long num = this.m_spaceManager.AllocateSpace(size);
			this.Seek(num, SeekOrigin.Begin);
			return num;
		}

		public long ReserveSpace(int length)
		{
			this.SetupStorage();
			return this.m_spaceManager.AllocateSpace(length);
		}

		public long Update(IPersistable obj, long offset, long oldPersistedSize)
		{
			return this.WriteObject(obj, offset, oldPersistedSize);
		}

		private long WriteObject(IPersistable obj, long offset, long oldSize)
		{
			this.SetupStorage();
			this.m_memoryStream.Seek(0L, SeekOrigin.Begin);
			this.m_writer.Write(obj);
			long position = this.m_memoryStream.Position;
			if (oldSize < 0 || offset < 0)
			{
				offset = this.SeekToFreeSpace(position);
			}
			else if (position != oldSize)
			{
				offset = this.m_spaceManager.Resize(offset, oldSize, position);
				this.Seek(offset, SeekOrigin.Begin);
			}
			else
			{
				this.Seek(offset, SeekOrigin.Begin);
			}
			this.m_stream.Write(this.m_memoryStream.GetBuffer(), 0, (int)position);
			return offset;
		}

		public void Free(long offset, int size)
		{
			this.m_spaceManager.Free(offset, size);
		}

		public void Close()
		{
			if (this.m_stream != null)
			{
				this.m_stream.Close();
				this.m_stream = null;
				this.m_memoryStream.Close();
				this.m_memoryStream = null;
			}
		}

		public void Flush()
		{
			if (this.m_stream != null)
			{
				this.m_stream.Flush();
			}
		}

		public void Dispose()
		{
			this.Close();
		}

		private void Seek(long offset, SeekOrigin origin)
		{
			this.m_stream.Seek(offset, origin);
			this.m_spaceManager.Seek(offset, origin);
		}

		public void TraceStats()
		{
			this.m_spaceManager.TraceStats();
		}
	}
}
