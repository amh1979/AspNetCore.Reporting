using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class DataSourceDisabledException : ReportCatalogException
	{
		public DataSourceDisabledException()
			: base(ErrorCode.rsDataSourceDisabled, ErrorStrings.rsDataSourceDisabled, null, null)
		{
		}

		private DataSourceDisabledException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
