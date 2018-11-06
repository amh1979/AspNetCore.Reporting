using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence
{
	public class Declaration
	{
		private List<MemberInfo> m_memberInfoList = new List<MemberInfo>();

		private ObjectType m_type;

		private ObjectType m_baseType;

		private Pair<bool, int>[] m_usableMembers;

		private bool m_hasSkippedMembers;

		internal List<MemberInfo> MemberInfoList
		{
			get
			{
				return this.m_memberInfoList;
			}
		}

		internal ObjectType ObjectType
		{
			get
			{
				return this.m_type;
			}
		}

		internal ObjectType BaseObjectType
		{
			get
			{
				return this.m_baseType;
			}
		}

		internal bool RegisteredCurrentDeclaration
		{
			get
			{
				return this.m_usableMembers != null;
			}
		}

		internal bool HasSkippedMembers
		{
			get
			{
				return this.m_hasSkippedMembers;
			}
		}

		internal Declaration(ObjectType type, ObjectType baseType, List<MemberInfo> memberInfoList)
		{
			this.m_type = type;
			this.m_baseType = baseType;
			this.m_memberInfoList = memberInfoList;
		}

		internal bool IsMemberSkipped(int index)
		{
			if (this.m_hasSkippedMembers)
			{
				return this.m_usableMembers[index].First;
			}
			return false;
		}

		internal int MembersToSkip(int index)
		{
			if (this.m_hasSkippedMembers)
			{
				return this.m_usableMembers[index].Second;
			}
			return 0;
		}

		internal void RegisterCurrentDeclaration(Declaration currentDeclaration)
		{
			this.m_hasSkippedMembers = false;
			this.m_usableMembers = new Pair<bool, int>[this.m_memberInfoList.Count];
			int num = 0;
			for (int num2 = this.m_memberInfoList.Count - 1; num2 >= 0; num2--)
			{
				if (currentDeclaration.Contains(this.m_memberInfoList[num2]))
				{
					num = 0;
				}
				else
				{
					this.m_hasSkippedMembers = true;
					num++;
					this.m_usableMembers[num2].Second = num;
					this.m_usableMembers[num2].First = true;
				}
			}
			if (!this.m_hasSkippedMembers)
			{
				this.m_usableMembers = new Pair<bool, int>[0];
			}
		}

		private bool Contains(MemberInfo otherMember)
		{
			return this.m_memberInfoList.Contains(otherMember);
		}

		internal Declaration CreateFilteredDeclarationForWriteVersion(int compatVersion)
		{
			List<MemberInfo> list = new List<MemberInfo>(this.m_memberInfoList.Count);
			for (int i = 0; i < this.m_memberInfoList.Count; i++)
			{
				MemberInfo memberInfo = this.m_memberInfoList[i];
				if (memberInfo.IsWrittenForCompatVersion(compatVersion))
				{
					list.Add(memberInfo);
				}
			}
			return new Declaration(this.m_type, this.m_baseType, list);
		}
	}
}
