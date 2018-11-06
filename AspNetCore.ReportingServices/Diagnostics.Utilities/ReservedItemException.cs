using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReservedItemException : ReportCatalogException
	{
		public ReservedItemException(string itemPath)
			: base(ErrorCode.rsReservedItem, ErrorStrings.rsReservedItem(itemPath), null, null)
		{
		}

		private ReservedItemException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
