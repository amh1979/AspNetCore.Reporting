using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class RectangleInstanceInfo : ReportItemInstanceInfo
	{
		internal RectangleInstanceInfo(ReportProcessing.ProcessingContext pc, Rectangle reportItemDef, RectangleInstance owner, int index)
			: base(pc, reportItemDef, owner, index)
		{
		}

		internal RectangleInstanceInfo(Rectangle reportItemDef)
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
