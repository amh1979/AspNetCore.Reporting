using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class AuthorizationExtensionException : RSException
	{
		public AuthorizationExtensionException(Exception innerException)
			: this(innerException, null)
		{
		}

		public AuthorizationExtensionException(Exception innerException, string additionalTraceMessage)
			: base(ErrorCode.rsAuthorizationExtensionError, ErrorStrings.rsAuthorizationExtensionError, innerException, RSTrace.IsTraceInitialized ? RSTrace.CatalogTrace : null, additionalTraceMessage)
		{
		}

		private AuthorizationExtensionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
