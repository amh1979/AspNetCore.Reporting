namespace AspNetCore.ReportingServices.DataProcessing
{
	internal interface IDbCollationProperties
	{
		bool GetCollationProperties(out string cultureName, out bool caseSensitive, out bool accentSensitive, out bool kanatypeSensitive, out bool widthSensitive);
	}
}
