using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReportTimeoutExpiredException : ReportCatalogException
	{
		public ReportTimeoutExpiredException(Exception innerException)
			: base(ErrorCode.rsReportTimeoutExpired, ErrorStrings.rsReportTimeoutExpired, innerException, null)
		{
		}

		private ReportTimeoutExpiredException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
