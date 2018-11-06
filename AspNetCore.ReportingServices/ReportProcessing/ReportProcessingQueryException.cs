using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingQueryException : ReportProcessingException
	{
		public ReportProcessingQueryException(ErrorCode errorCode, Exception innerException, params object[] arguments)
			: base(errorCode, innerException, arguments)
		{
		}

		private ReportProcessingQueryException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
