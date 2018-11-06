using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ActiveXControlInstance : ReportItemInstance
	{
		internal ActiveXControlInstanceInfo InstanceInfo
		{
			get
			{
				if (base.m_instanceInfo is OffsetInfo)
				{
					Global.Tracer.Assert(false, string.Empty);
					return null;
				}
				return (ActiveXControlInstanceInfo)base.m_instanceInfo;
			}
		}

		internal ActiveXControlInstance(ReportProcessing.ProcessingContext pc, ActiveXControl reportItemDef, int index)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			base.m_instanceInfo = new ActiveXControlInstanceInfo(pc, reportItemDef, this, index);
		}

		internal ActiveXControlInstance()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList members = new MemberInfoList();
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance, members);
		}

		internal override ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(base.m_instanceInfo is OffsetInfo);
			return reader.ReadActiveXControlInstanceInfo((ActiveXControl)base.m_reportItemDef);
		}
	}
}
