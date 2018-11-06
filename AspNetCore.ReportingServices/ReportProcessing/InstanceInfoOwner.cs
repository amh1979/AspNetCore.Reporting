using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal abstract class InstanceInfoOwner
	{
		protected InfoBase m_instanceInfo;

		internal OffsetInfo OffsetInfo
		{
			get
			{
				if (this.m_instanceInfo == null)
				{
					return null;
				}
				Global.Tracer.Assert(this.m_instanceInfo is OffsetInfo);
				return (OffsetInfo)this.m_instanceInfo;
			}
			set
			{
				this.m_instanceInfo = value;
			}
		}

		internal long ChunkOffset
		{
			get
			{
				if (this.m_instanceInfo != null && this.m_instanceInfo is OffsetInfo)
				{
					return ((OffsetInfo)this.m_instanceInfo).Offset;
				}
				return 0L;
			}
		}

		internal void SetOffset(long offset)
		{
			this.m_instanceInfo = new OffsetInfo(offset);
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.OffsetInfo, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.OffsetInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
