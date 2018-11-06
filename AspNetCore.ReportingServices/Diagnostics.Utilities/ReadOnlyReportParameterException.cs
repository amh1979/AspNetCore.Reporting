using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReadOnlyReportParameterException : ReportCatalogException
	{
		public ReadOnlyReportParameterException(string parameterName)
			: base(ErrorCode.rsReadOnlyReportParameter, ErrorStrings.rsReadOnlyReportParameter(parameterName), null, null)
		{
		}

		private ReadOnlyReportParameterException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
