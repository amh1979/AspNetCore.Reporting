using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ModelGenerationNotSupportedException : ReportCatalogException
	{
		public ModelGenerationNotSupportedException()
			: base(ErrorCode.rsModelGenerationNotSupported, ErrorStrings.rsModelGenerationNotSupported, null, null)
		{
		}

		private ModelGenerationNotSupportedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
