namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal interface IDeepCloneable<T>
	{
		T DeepClone();
	}
}
