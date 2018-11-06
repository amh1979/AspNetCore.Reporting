using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReadOnlyPropertyException : ReportCatalogException
	{
		public ReadOnlyPropertyException(string propertyName)
			: base(ErrorCode.rsReadOnlyProperty, ErrorStrings.rsReadOnlyProperty(propertyName), null, null)
		{
		}

		private ReadOnlyPropertyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
