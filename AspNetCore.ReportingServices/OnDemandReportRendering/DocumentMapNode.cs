using AspNetCore.ReportingServices.ReportProcessing;
using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	[Serializable]
	internal sealed class DocumentMapNode : OnDemandDocumentMapNode
	{
		internal DocumentMapNode(string aLabel, string aId, int aLevel)
			: base(aLabel, aId, aLevel)
		{
		}
	}
}
