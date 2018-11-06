using System.Collections;
using System.Globalization;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal static class SmartClientSerializerHelper
	{
		private static CaseInsensitiveHashCodeProvider hashCodeProvider = new CaseInsensitiveHashCodeProvider(CultureInfo.InvariantCulture);
	}
}
