using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ModelGenerationErrorException : ReportCatalogException
	{
		public ModelGenerationErrorException(Exception innerException)
			: base(ErrorCode.rsModelGenerationError, ErrorStrings.rsModelGenerationError, innerException, null)
		{
		}

		private ModelGenerationErrorException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
