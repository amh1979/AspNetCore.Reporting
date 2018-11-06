using System;
using System.Runtime.Serialization;

namespace AspNetCore.Reporting
{
	[Serializable]
	internal sealed class AspNetSessionExpiredException : ReportViewerException
	{
		internal AspNetSessionExpiredException()
			: base(Errors.ASPNetSessionExpired)
		{
		}

		private AspNetSessionExpiredException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
