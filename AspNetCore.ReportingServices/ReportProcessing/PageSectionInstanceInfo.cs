using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class PageSectionInstanceInfo : ReportItemInstanceInfo
	{
		internal PageSectionInstanceInfo(ReportProcessing.ProcessingContext pc, PageSection reportItemDef, PageSectionInstance owner)
			: base(pc, reportItemDef, owner, true)
		{
		}

		internal PageSectionInstanceInfo(PageSection reportItemDef)
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
