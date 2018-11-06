namespace AspNetCore.ReportingServices.DataProcessing
{
	internal interface IDbErrorInspectorFactory
	{
		IDbErrorInspector CreateErrorInspector();
	}
}
