using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidDataSourceReferenceException : ReportCatalogException
	{
		public InvalidDataSourceReferenceException(string datasourceName)
			: base(ErrorCode.rsInvalidDataSourceReference, ErrorStrings.rsInvalidDataSourceReference(datasourceName), null, null)
		{
		}

		private InvalidDataSourceReferenceException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
