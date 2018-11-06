using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidReportParameterException : ReportCatalogException
	{
		public InvalidReportParameterException(string parameterName)
			: base(ErrorCode.rsInvalidReportParameter, ErrorStrings.rsInvalidReportParameter(parameterName), null, null)
		{
		}

		private InvalidReportParameterException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
