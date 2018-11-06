using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ProcessingAbortedException : RSException
	{
		internal enum Reason
		{
			UserCanceled,
			AbnormalTermination
		}

		private Reason m_reason;

		private readonly CancelationTrigger m_cancelationTrigger;

		public Reason ReasonForAbort
		{
			get
			{
				return this.m_reason;
			}
		}

		internal CancelationTrigger Trigger
		{
			get
			{
				return this.m_cancelationTrigger;
			}
		}

		private ProcessingAbortedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		internal ProcessingAbortedException()
			: this(CancelationTrigger.None)
		{
		}

		internal ProcessingAbortedException(CancelationTrigger cancelationTrigger)
			: base(ErrorCode.rsProcessingAborted, RPRes.rsProcessingAbortedByUser, null, Global.Tracer, ProcessingAbortedException.CreateAdditionalTraceMessage(Reason.UserCanceled, cancelationTrigger))
		{
			this.m_reason = Reason.UserCanceled;
			this.m_cancelationTrigger = cancelationTrigger;
		}

		internal ProcessingAbortedException(Exception innerException)
			: this(CancelationTrigger.None, innerException)
		{
		}

		internal ProcessingAbortedException(CancelationTrigger cancelationTrigger, Exception innerException)
			: base(ErrorCode.rsProcessingAborted, RPRes.rsProcessingAbortedByError, innerException, Global.Tracer, ProcessingAbortedException.CreateAdditionalTraceMessage(Reason.AbnormalTermination, cancelationTrigger))
		{
			this.m_reason = Reason.AbnormalTermination;
			this.m_cancelationTrigger = cancelationTrigger;
		}

		private static string CreateAdditionalTraceMessage(Reason reason, CancelationTrigger trigger)
		{
			return string.Format(CultureInfo.InvariantCulture, "[{0}:{1}]", reason.ToString(), trigger.ToString());
		}
	}
}
