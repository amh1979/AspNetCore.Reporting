namespace AspNetCore.Reporting
{
	internal enum ParameterState
	{
		HasValidValue,
		MissingValidValue,
		HasOutstandingDependencies,
		DynamicValuesUnavailable
	}
}
