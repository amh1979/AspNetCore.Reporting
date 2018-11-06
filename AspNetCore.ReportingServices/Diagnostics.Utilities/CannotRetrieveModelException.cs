using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class CannotRetrieveModelException : ReportCatalogException
	{
		public static bool IsCannotRetrieveModelErrorCode(ErrorCode errorCode)
		{
			if (errorCode != ErrorCode.rsCannotRetrieveModel && errorCode != ErrorCode.rsUnsupportedMetadataVersionRequested && errorCode != ErrorCode.rsInvalidPerspectiveAndVersion)
			{
				return false;
			}
			return true;
		}

		public CannotRetrieveModelException(ErrorCode errorCode, string itemName, Exception innerException)
			: base(errorCode, ErrorStrings.rsCannotRetrieveModel(itemName), innerException, null)
		{
		}

		public CannotRetrieveModelException(string itemName, Exception innerException)
			: base(ErrorCode.rsCannotRetrieveModel, ErrorStrings.rsCannotRetrieveModel(itemName), innerException, null)
		{
		}

		private CannotRetrieveModelException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
