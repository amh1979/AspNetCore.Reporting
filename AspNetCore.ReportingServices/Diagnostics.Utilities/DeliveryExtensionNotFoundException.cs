using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class DeliveryExtensionNotFoundException : ReportCatalogException
	{
		public DeliveryExtensionNotFoundException()
			: base(ErrorCode.rsDeliveryExtensionNotFound, ErrorStrings.rsDeliveryExtensionNotFound, null, null)
		{
		}

		private DeliveryExtensionNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
