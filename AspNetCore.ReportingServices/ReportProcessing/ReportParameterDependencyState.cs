namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal enum ReportParameterDependencyState
	{
		AllDependenciesSpecified,
		HasOutstandingDependencies,
		MissingUpstreamDataSourcePrompt
	}
}
