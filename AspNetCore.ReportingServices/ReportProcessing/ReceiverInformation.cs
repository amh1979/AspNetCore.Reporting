using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReceiverInformation
	{
		private bool m_startHidden;

		private int m_senderUniqueName;

		internal bool StartHidden
		{
			get
			{
				return this.m_startHidden;
			}
			set
			{
				this.m_startHidden = value;
			}
		}

		internal int SenderUniqueName
		{
			get
			{
				return this.m_senderUniqueName;
			}
			set
			{
				this.m_senderUniqueName = value;
			}
		}

		internal ReceiverInformation()
		{
		}

		internal ReceiverInformation(bool startHidden, int senderUniqueName)
		{
			this.m_startHidden = startHidden;
			this.m_senderUniqueName = senderUniqueName;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.StartHidden, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.SenderUniqueName, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
