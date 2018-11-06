using System;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SharePointException : ReportCatalogException
	{
		public SharePointException(Exception innerException)
			: base(ErrorCode.rsSharePointError, SharePointException.GetExceptionMessage(innerException), innerException, null)
		{
		}

		private SharePointException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private static string GetExceptionMessage(Exception innerException)
		{
			SqlException ex = innerException as SqlException;
			if (ex != null)
			{
				return ErrorStrings.rsSharePointContentDBAccessError;
			}
			return ErrorStrings.rsSharePointError;
		}
	}
}
