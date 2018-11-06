using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Interfaces
{
	internal interface IReportDefinitionCustomizationExtension : IExtension
	{
		bool ProcessReportDefinition(byte[] reportDefinition, IReportContext reportContext, IUserContext userContext, out byte[] reportDefinitionProcessed, out IEnumerable<RdceCustomizableElementId> customizedElementIds);
	}
}
