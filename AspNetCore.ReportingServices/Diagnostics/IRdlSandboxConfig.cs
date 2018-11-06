using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal interface IRdlSandboxConfig
	{
		IList<IRdlSandboxTypeInfo> AllowedTypes
		{
			get;
		}

		IList<string> DeniedMembers
		{
			get;
		}

		int MaxExpressionLength
		{
			get;
		}

		int MaxResourceSizeKB
		{
			get;
		}

		int MaxStringResultLength
		{
			get;
		}

		int MaxArrayResultLength
		{
			get;
		}
	}
}
