using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RequestEncodingFormatException : ReportCatalogException
	{
		public RequestEncodingFormatException(Exception innerException)
			: base(ErrorCode.rsRequestEncodingFormatException, ErrorStrings.rsRequestEncodingFormatException, innerException, null)
		{
		}

		private RequestEncodingFormatException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
