using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_FieldError : Exception
	{
		private DataFieldStatus m_status;

		internal DataFieldStatus Status
		{
			get
			{
				return this.m_status;
			}
		}

		internal ReportProcessingException_FieldError(DataFieldStatus status, string message)
			: base((message == null) ? "" : message, null)
		{
			this.m_status = status;
		}

		private ReportProcessingException_FieldError(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
