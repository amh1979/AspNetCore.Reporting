using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_InvalidOperationException : Exception
	{
		internal ReportProcessingException_InvalidOperationException()
		{
		}

		private ReportProcessingException_InvalidOperationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
