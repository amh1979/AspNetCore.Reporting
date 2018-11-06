using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class LineInstanceInfo : ReportItemInstanceInfo
	{
		internal LineInstanceInfo(ReportProcessing.ProcessingContext pc, Line reportItemDef, ReportItemInstance owner, int index)
			: base(pc, reportItemDef, owner, index)
		{
		}

		internal LineInstanceInfo(Line reportItemDef)
			: base(reportItemDef)
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList members = new MemberInfoList();
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstanceInfo, members);
		}
	}
}
