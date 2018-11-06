using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InternalDataSourceNotFoundException : ReportCatalogException
	{
		public InternalDataSourceNotFoundException()
			: base(ErrorCode.rsInternalDataSourceNotFound, ErrorStrings.internalDataSourceNotFound, null, null)
		{
		}

		private InternalDataSourceNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
