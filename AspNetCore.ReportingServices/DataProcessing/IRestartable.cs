using System.Collections.Generic;

namespace AspNetCore.ReportingServices.DataProcessing
{
	internal interface IRestartable
	{
		IDataParameter[] StartAt(List<ScopeValueFieldName> scopeValueFieldNameCollection);
	}
}
