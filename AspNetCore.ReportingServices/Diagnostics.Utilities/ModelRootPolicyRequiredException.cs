using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ModelRootPolicyRequiredException : ReportCatalogException
	{
		public ModelRootPolicyRequiredException()
			: base(ErrorCode.rsModelRootPolicyRequired, ErrorStrings.rsModelRootPolicyRequired, null, null)
		{
		}

		private ModelRootPolicyRequiredException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
