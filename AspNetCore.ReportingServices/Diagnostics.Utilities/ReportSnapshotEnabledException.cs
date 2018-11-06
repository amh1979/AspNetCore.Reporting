using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReportSnapshotEnabledException : ReportCatalogException
	{
		public ReportSnapshotEnabledException()
			: base(ErrorCode.rsReportSnapshotEnabled, ErrorStrings.rsReportSnapshotEnabled, null, null)
		{
		}

		private ReportSnapshotEnabledException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
