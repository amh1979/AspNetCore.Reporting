using System;

namespace AspNetCore.ReportingServices.DataProcessing
{
	internal interface IDbErrorInspector
	{
		bool IsQueryTimeout(Exception e);

		bool IsQueryMemoryLimitExceeded(Exception e);

		bool IsOnPremisesServiceException(Exception e);
	}
}
