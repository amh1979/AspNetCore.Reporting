using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class EventLogSourceNotFound : ReportCatalogException
	{
		public EventLogSourceNotFound(string source)
			: base(ErrorCode.rsEventLogSourceNotFound, ErrorStrings.rsEventLogSourceNotFound(source), null, null)
		{
		}

		private EventLogSourceNotFound(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
