namespace AspNetCore.ReportingServices.Interfaces
{
	internal enum CatalogOperation
	{
		CreateRoles,
		DeleteRoles,
		ReadRoleProperties,
		UpdateRoleProperties,
		ReadSystemProperties,
		UpdateSystemProperties,
		GenerateEvents,
		ReadSystemSecurityPolicy,
		UpdateSystemSecurityPolicy,
		CreateSchedules,
		DeleteSchedules,
		ReadSchedules,
		UpdateSchedules,
		ListJobs,
		CancelJobs,
		ExecuteReportDefinition
	}
}
