using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class VersionMismatchException : ReportCatalogException
	{
		private Guid m_reportID;

		private bool m_isPermanentSnapshot;

		public Guid ReportID
		{
			get
			{
				return this.m_reportID;
			}
		}

		public bool IsPermanentSnapshot
		{
			get
			{
				return this.m_isPermanentSnapshot;
			}
		}

		protected override bool TraceFullException
		{
			get
			{
				return false;
			}
		}

		public VersionMismatchException(Guid reportID, bool isPermanentSnapshot)
			: base(ErrorCode.rsSnapshotVersionMismatch, ErrorStrings.rsSnapshotVersionMismatch, null, "version mismatch found", TraceLevel.Verbose)
		{
			this.m_reportID = reportID;
			this.m_isPermanentSnapshot = isPermanentSnapshot;
		}

		private VersionMismatchException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
