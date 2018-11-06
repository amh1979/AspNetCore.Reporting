namespace AspNetCore.ReportingServices.ReportRendering
{
	internal interface IDeepCloneable
	{
		ReportItem DeepClone();
	}
}
