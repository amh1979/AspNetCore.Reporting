using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ImageInstance : ReportItemInstance
	{
		internal ImageInstanceInfo InstanceInfo
		{
			get
			{
				if (base.m_instanceInfo is OffsetInfo)
				{
					Global.Tracer.Assert(false, string.Empty);
					return null;
				}
				return (ImageInstanceInfo)base.m_instanceInfo;
			}
		}

		internal ImageInstance(ReportProcessing.ProcessingContext pc, Image reportItemDef, int index)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			base.m_instanceInfo = new ImageInstanceInfo(pc, reportItemDef, this, index, false);
		}

		internal ImageInstance(ReportProcessing.ProcessingContext pc, Image reportItemDef, int index, bool customCreated)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			base.m_instanceInfo = new ImageInstanceInfo(pc, reportItemDef, this, index, customCreated);
		}

		internal ImageInstance()
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
			return reader.ReadImageInstanceInfo((Image)base.m_reportItemDef);
		}
	}
}
