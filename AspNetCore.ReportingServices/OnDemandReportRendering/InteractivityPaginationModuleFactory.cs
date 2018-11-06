using AspNetCore.ReportingServices.Rendering.SPBProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal static class InteractivityPaginationModuleFactory
	{
		internal static IInteractivityPaginationModule CreateInteractivityPaginationModule()
		{
			return new SPBInteractivityProcessing();
		}
	}
}
