using AspNetCore.ReportingServices.ReportProcessing;
using System;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal interface IInternalProcessingContext
	{
		ErrorContext ErrorContext
		{
			get;
		}

		bool SnapshotProcessing
		{
			get;
			set;
		}

		DateTime ExecutionTime
		{
			get;
		}

		bool EnableDataBackedParameters
		{
			get;
		}
	}
}
