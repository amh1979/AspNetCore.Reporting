using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal interface IActionOwner
	{
		Action Action
		{
			get;
		}

		List<string> FieldsUsedInValueExpression
		{
			get;
			set;
		}
	}
}
