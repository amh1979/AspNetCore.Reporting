using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ContainerTypeNotSupportedException : ReportCatalogException
	{
		public ContainerTypeNotSupportedException()
			: base(ErrorCode.rsContainerNotSupported, ErrorStrings.rsContainerNotSupported, null, null)
		{
		}

		private ContainerTypeNotSupportedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
