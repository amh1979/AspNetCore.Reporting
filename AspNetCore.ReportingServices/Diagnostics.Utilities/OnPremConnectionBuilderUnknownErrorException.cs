using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class OnPremConnectionBuilderUnknownErrorException : ReportCatalogException
	{
		public OnPremConnectionBuilderUnknownErrorException(string connectionString, Exception innerException)
			: base(ErrorCode.rsOnPremConnectionBuilderUnknownError, ErrorStrings.rsOnPremConnectionBuilderUnknownError, innerException, null)
		{
		}

		private OnPremConnectionBuilderUnknownErrorException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
