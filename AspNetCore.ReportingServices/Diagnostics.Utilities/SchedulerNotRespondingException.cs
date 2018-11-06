using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SchedulerNotRespondingException : ReportCatalogException
	{
		public SchedulerNotRespondingException()
			: base(ErrorCode.rsSchedulerNotResponding, ErrorStrings.rsSchedulerNotResponding, null, null)
		{
		}

		private SchedulerNotRespondingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
