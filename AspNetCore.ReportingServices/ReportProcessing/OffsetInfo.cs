using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class OffsetInfo : InfoBase
	{
		private long m_offset;

		internal long Offset
		{
			get
			{
				return this.m_offset;
			}
			set
			{
				this.m_offset = value;
			}
		}

		internal OffsetInfo()
		{
		}

		internal OffsetInfo(long offset)
		{
			this.m_offset = offset;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Offset, Token.Int64));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.InfoBase, memberInfoList);
		}
	}
}
