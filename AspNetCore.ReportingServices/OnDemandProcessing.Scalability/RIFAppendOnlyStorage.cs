using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class RIFAppendOnlyStorage : IStorage, IDisposable
	{
		private Stream m_stream;

		private IntermediateFormatWriter m_writer;

		private IntermediateFormatReader m_reader;

		private IScalabilityCache m_scalabilityCache;

		private bool m_writerSetup;

		private IStreamHandler m_streamCreator;

		private IReferenceCreator m_referenceCreator;

		private UnifiedObjectCreator m_unifiedObjectCreator;

		private bool m_fromExistingStream;

		private GlobalIDOwnerCollection m_globalIdsFromOtherStream;

		private readonly int m_rifCompatVersion;

		private bool m_atEnd;

		private long m_streamLength = -1L;

		private readonly bool m_prohibitSerializableValues;

		internal bool FromExistingStream
		{
			get
			{
				return this.m_fromExistingStream;
			}
		}

		public long StreamSize
		{
			get
			{
				return this.m_streamLength;
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
				return false;
			}
			set
			{
			}
		}

		internal RIFAppendOnlyStorage(IStreamHandler streamHandler, IScalabilityObjectCreator appObjectCreator, IReferenceCreator appReferenceCreator, GlobalIDOwnerCollection globalIdsFromOtherStream, bool fromExistingStream, int rifCompatVersion, bool prohibitSerializableValues)
		{
			this.m_streamCreator = streamHandler;
			this.m_scalabilityCache = null;
			this.m_stream = null;
			this.m_unifiedObjectCreator = new UnifiedObjectCreator(appObjectCreator, appReferenceCreator);
			this.m_referenceCreator = new UnifiedReferenceCreator(appReferenceCreator);
			this.m_fromExistingStream = fromExistingStream;
			this.m_globalIdsFromOtherStream = globalIdsFromOtherStream;
			this.m_prohibitSerializableValues = prohibitSerializableValues;
			this.m_rifCompatVersion = rifCompatVersion;
		}

		private void SetupStorage()
		{
			if (this.m_stream == null)
			{
				this.m_stream = this.m_streamCreator.OpenStream();
				this.m_streamCreator = null;
				List<Declaration> declarations = this.m_unifiedObjectCreator.GetDeclarations();
				if (this.m_fromExistingStream)
				{
					this.m_reader = new IntermediateFormatReader(this.m_stream, this.m_unifiedObjectCreator, this.m_globalIdsFromOtherStream, this.m_scalabilityCache);
					if (this.m_stream.CanWrite)
					{
						this.m_writer = new IntermediateFormatWriter(this.m_stream, this.m_stream.Length, declarations, this.m_scalabilityCache, this.m_rifCompatVersion, this.m_prohibitSerializableValues);
						this.m_writerSetup = true;
					}
					this.m_atEnd = false;
				}
				else
				{
					this.m_writer = new IntermediateFormatWriter(this.m_stream, declarations, this.m_scalabilityCache, this.m_rifCompatVersion, this.m_prohibitSerializableValues);
					this.m_writerSetup = true;
					this.m_reader = new IntermediateFormatReader(this.m_stream, this.m_unifiedObjectCreator, this.m_globalIdsFromOtherStream, this.m_scalabilityCache, declarations, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatVersion.Current, PersistenceFlags.Seekable);
					this.m_atEnd = true;
				}
				this.m_fromExistingStream = true;
			}
		}

		public IPersistable Retrieve(long offset, out long persistedSize)
		{
			this.SetupStorage();
			this.m_stream.Seek(offset, SeekOrigin.Begin);
			this.m_atEnd = false;
			long position = this.m_stream.Position;
			IPersistable result = this.m_reader.ReadRIFObject();
			persistedSize = this.m_stream.Position - position;
			return result;
		}

		public IPersistable Retrieve(long offset)
		{
			long num = default(long);
			return this.Retrieve(offset, out num);
		}

		public T Retrieve<T>(long offset, out long persistedSize) where T : IPersistable, new()
		{
			this.SetupStorage();
			this.m_stream.Seek(offset, SeekOrigin.Begin);
			this.m_atEnd = false;
			long position = this.m_stream.Position;
			T result = this.m_reader.ReadRIFObject<T>();
			persistedSize = this.m_stream.Position - position;
			return result;
		}

		public long Allocate(IPersistable obj)
		{
			this.SetupStorage();
			if (!this.m_atEnd)
			{
				this.m_stream.Seek(0L, SeekOrigin.End);
				this.m_atEnd = true;
			}
			this.m_streamLength = this.m_stream.Position;
			this.m_writer.Write(obj);
			return this.m_streamLength;
		}

		public void Free(long offset, int size)
		{
			Global.Tracer.Assert(false, "RIFAppendOnlyStorage does not support Free(...)");
		}

		public long Update(IPersistable obj, long offset, long oldPersistedSize)
		{
			Global.Tracer.Assert(false, "RIFAppendOnlyStorage does not support Update(...)s");
			return -1L;
		}

		public void Close()
		{
			if (this.m_stream != null)
			{
				this.m_stream.Close();
				this.m_stream = null;
			}
		}

		public void Flush()
		{
			if (this.m_stream != null)
			{
				this.m_stream.Flush();
			}
		}

		internal void Reset(IStreamHandler streamHandler)
		{
			this.Close();
			this.m_writerSetup = false;
			this.m_streamCreator = streamHandler;
		}

		public void TraceStats()
		{
		}

		public void Dispose()
		{
			this.Close();
		}
	}
}
