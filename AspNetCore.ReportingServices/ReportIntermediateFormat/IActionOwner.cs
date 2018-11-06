using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
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
