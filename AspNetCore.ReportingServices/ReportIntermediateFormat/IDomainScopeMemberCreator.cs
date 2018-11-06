using AspNetCore.ReportingServices.ReportPublishing;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal interface IDomainScopeMemberCreator
	{
		void CreateDomainScopeMember(ReportHierarchyNode parentNode, Grouping grouping, AutomaticSubtotalContext context);
	}
}
