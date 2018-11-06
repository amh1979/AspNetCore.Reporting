using System;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal interface IDataShapeQueryAbortHelper : IDataShapeAbortHelper, IAbortHelper, IDisposable
	{
		IDataShapeAbortHelper CreateDataShapeAbortHelper();
	}
}
