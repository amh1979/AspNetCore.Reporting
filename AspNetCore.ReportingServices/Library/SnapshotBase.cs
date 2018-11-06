using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.IO;

namespace AspNetCore.ReportingServices.Library
{
	[Serializable]
	internal abstract class SnapshotBase
	{
		private Guid m_snapshotDataID;

		private bool m_isPermanentSnapshot;

		internal Guid SnapshotDataID
		{
			get
			{
				return this.m_snapshotDataID;
			}
		}

		internal bool IsPermanentSnapshot
		{
			get
			{
				return this.m_isPermanentSnapshot;
			}
		}

		protected SnapshotBase(Guid snapshotDataID, bool isPermanentSnapshot)
		{
			this.m_snapshotDataID = snapshotDataID;
			this.m_isPermanentSnapshot = isPermanentSnapshot;
		}

		protected SnapshotBase(bool isPermanentSnapshot)
		{
			this.m_snapshotDataID = Guid.NewGuid();
			this.m_isPermanentSnapshot = isPermanentSnapshot;
		}

		protected SnapshotBase(SnapshotBase snapshotDataToCopy)
		{
			this.m_snapshotDataID = snapshotDataToCopy.SnapshotDataID;
			this.m_isPermanentSnapshot = snapshotDataToCopy.m_isPermanentSnapshot;
		}

		public abstract SnapshotBase Duplicate();

		[Obsolete("Use PrepareExecutionSnapshot instead")]
		public abstract void CopyImageChunksTo(SnapshotBase target);

		public abstract void PrepareExecutionSnapshot(SnapshotBase target, string compiledDefinitionChunkName);

		public abstract Stream GetChunk(string name, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type, out string mimeType);

		public abstract string GetStreamMimeType(string name, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type);

		public abstract Stream CreateChunk(string name, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type, string mimeType);

		public abstract void DeleteSnapshotAndChunks();

		internal virtual void UpdatePerfData(Stream chunk)
		{
		}

		internal virtual void WritePerfData()
		{
		}
	}
}
