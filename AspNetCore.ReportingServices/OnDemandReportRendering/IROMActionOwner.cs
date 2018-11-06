using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal interface IROMActionOwner
	{
		string UniqueName
		{
			get;
		}

		List<string> FieldsUsedInValueExpression
		{
			get;
		}
	}
}
