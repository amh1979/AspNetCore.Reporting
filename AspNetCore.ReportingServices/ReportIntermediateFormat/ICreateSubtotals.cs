using AspNetCore.ReportingServices.ReportPublishing;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal interface ICreateSubtotals
	{
		void CreateAutomaticSubtotals(AutomaticSubtotalContext context);
	}
}
