using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence
{
	[Serializable]
	internal sealed class IncompatibleRIFVersionException : RSException
	{
		internal IncompatibleRIFVersionException(int documentCompatVersion, int codeCompatVersion)
			: base(ErrorCode.rsIncompatibleRIFVersion, string.Format(CultureInfo.InvariantCulture, "The RIF document is not compatible with this code version.  Document Version: {0} Code Version: {1}", documentCompatVersion, codeCompatVersion), null, Global.Tracer, null)
		{
		}

		internal IncompatibleRIFVersionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		internal static void ThrowIfIncompatible(int documentCompatVersion, int codeCompatVersion)
		{
			if (documentCompatVersion == codeCompatVersion)
			{
				return;
			}
			if (documentCompatVersion == 0)
			{
				return;
			}
			if (documentCompatVersion <= codeCompatVersion)
			{
				return;
			}
			throw new IncompatibleRIFVersionException(documentCompatVersion, codeCompatVersion);
		}
	}
}
