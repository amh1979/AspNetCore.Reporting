using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class SenderInformation
	{
		private bool m_startHidden;

		private IntList m_receiverUniqueNames;

		private int[] m_containerUniqueNames;

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

		internal IntList ReceiverUniqueNames
		{
			get
			{
				return this.m_receiverUniqueNames;
			}
			set
			{
				this.m_receiverUniqueNames = value;
			}
		}

		internal int[] ContainerUniqueNames
		{
			get
			{
				return this.m_containerUniqueNames;
			}
			set
			{
				this.m_containerUniqueNames = value;
			}
		}

		internal SenderInformation()
		{
		}

		internal SenderInformation(bool startHidden, int[] containerUniqueNames)
		{
			this.m_startHidden = startHidden;
			this.m_receiverUniqueNames = new IntList();
			this.m_containerUniqueNames = containerUniqueNames;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Hidden, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.ReceiverUniqueNames, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.IntList));
			memberInfoList.Add(new MemberInfo(MemberName.ContainerUniqueNames, Token.TypedArray));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
