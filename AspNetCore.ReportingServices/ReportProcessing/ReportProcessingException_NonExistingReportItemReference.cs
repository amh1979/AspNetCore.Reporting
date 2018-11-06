using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_NonExistingReportItemReference : Exception
	{
		internal ReportProcessingException_NonExistingReportItemReference(string itemName)
			: base(string.Format(CultureInfo.CurrentCulture, RPRes.rsNonExistingReportItemReference(itemName)))
		{
		}

		private ReportProcessingException_NonExistingReportItemReference(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
