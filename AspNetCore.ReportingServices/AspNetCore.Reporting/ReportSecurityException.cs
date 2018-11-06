using System;
using System.Runtime.Serialization;

namespace AspNetCore.Reporting
{
	[Serializable]
	internal sealed class ReportSecurityException : ReportViewerException
	{
		internal ReportSecurityException(string message)
			: base(message)
		{
		}

		private ReportSecurityException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
