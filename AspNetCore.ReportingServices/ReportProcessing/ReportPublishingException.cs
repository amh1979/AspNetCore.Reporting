using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportPublishingException : ReportProcessingException
	{
		private const string ReportProcessingFlagsName = "ReportProcessingFlags";

		private ReportProcessingFlags m_processingFlags;

		public ReportProcessingFlags ReportProcessingFlags
		{
			get
			{
				return this.m_processingFlags;
			}
		}

		public ReportPublishingException(ProcessingMessageList messages, ReportProcessingFlags processingFlags)
			: base(messages)
		{
			this.m_processingFlags = processingFlags;
		}

		public ReportPublishingException(ProcessingMessageList messages, Exception innerException, ReportProcessingFlags processingFlags)
			: base(messages, innerException)
		{
			this.m_processingFlags = processingFlags;
		}

		public ReportPublishingException(ErrorCode code, Exception innerException, params object[] arguments)
			: base(code, innerException, arguments)
		{
		}

		private ReportPublishingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.m_processingFlags = (ReportProcessingFlags)info.GetValue("ReportProcessingFlags", typeof(ReportProcessingFlags));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ReportProcessingFlags", this.m_processingFlags);
		}
	}
}
