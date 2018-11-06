using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CustomReportItemInstanceInfo : ReportItemInstanceInfo
	{
		internal CustomReportItemInstanceInfo(ReportProcessing.ProcessingContext pc, CustomReportItem reportItemDef, CustomReportItemInstance owner)
			: base(pc, reportItemDef, owner, true)
		{
		}

		internal CustomReportItemInstanceInfo(CustomReportItem reportItemDef)
			: base(reportItemDef)
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList members = new MemberInfoList();
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, members);
		}
	}
}
