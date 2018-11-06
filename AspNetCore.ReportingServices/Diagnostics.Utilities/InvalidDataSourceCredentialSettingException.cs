using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidDataSourceCredentialSettingException : ReportCatalogException
	{
		public InvalidDataSourceCredentialSettingException()
			: base(ErrorCode.rsInvalidDataSourceCredentialSetting, ErrorStrings.rsInvalidDataSourceCredentialSetting, null, null)
		{
		}

		private InvalidDataSourceCredentialSettingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
