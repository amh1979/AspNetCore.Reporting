using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class FailedToExportSymmetricKeyException : ReportCatalogException
	{
		public FailedToExportSymmetricKeyException()
			: base(ErrorCode.rsFailedToExportSymmetricKey, ErrorStrings.rsFailedToExportSymmetricKey, null, null)
		{
		}

		private FailedToExportSymmetricKeyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
