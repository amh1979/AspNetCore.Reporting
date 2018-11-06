using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class CannotDeleteRootPolicyException : ReportCatalogException
	{
		public CannotDeleteRootPolicyException()
			: base(ErrorCode.rsCannotDeleteRootPolicy, ErrorStrings.rsCannotDeleteRootPolicy, null, null)
		{
		}

		private CannotDeleteRootPolicyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
