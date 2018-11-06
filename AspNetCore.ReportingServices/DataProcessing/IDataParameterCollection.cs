using System.Collections;

namespace AspNetCore.ReportingServices.DataProcessing
{
	internal interface IDataParameterCollection : IEnumerable
	{
		int Add(IDataParameter parameter);
	}
}
