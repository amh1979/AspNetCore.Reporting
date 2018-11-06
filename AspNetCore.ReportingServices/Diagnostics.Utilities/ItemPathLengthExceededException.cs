using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ItemPathLengthExceededException : ReportCatalogException
	{
		public ItemPathLengthExceededException(string itemPath)
			: base(ErrorCode.rsItemPathLengthExceeded, ErrorStrings.rsItemPathLengthExceeded(itemPath, CatalogItemNameUtility.MaxItemPathLength), null, null)
		{
		}

		private ItemPathLengthExceededException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
