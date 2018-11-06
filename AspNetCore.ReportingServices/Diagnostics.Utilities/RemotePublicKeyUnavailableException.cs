using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RemotePublicKeyUnavailableException : ReportCatalogException
	{
		public RemotePublicKeyUnavailableException()
			: base(ErrorCode.rsRemotePublicKeyUnavailable, ErrorStrings.rsRemotePublicKeyUnavailable, null, null)
		{
		}

		private RemotePublicKeyUnavailableException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
