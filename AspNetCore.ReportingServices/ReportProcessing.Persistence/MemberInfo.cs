namespace AspNetCore.ReportingServices.ReportProcessing.Persistence
{
	internal sealed class MemberInfo
	{
		private MemberName m_memberName;

		private Token m_token;

		private ObjectType m_objectType;

		internal MemberName MemberName
		{
			get
			{
				return this.m_memberName;
			}
			set
			{
				this.m_memberName = value;
			}
		}

		internal Token Token
		{
			get
			{
				return this.m_token;
			}
		}

		internal ObjectType ObjectType
		{
			get
			{
				return this.m_objectType;
			}
		}

		internal MemberInfo(MemberName memberName, Token token)
		{
			this.m_memberName = memberName;
			this.m_token = token;
			this.m_objectType = ObjectType.None;
		}

		internal MemberInfo(MemberName memberName, ObjectType objectType)
		{
			this.m_memberName = memberName;
			this.m_token = Token.Object;
			this.m_objectType = objectType;
		}

		internal MemberInfo(MemberName memberName, Token token, ObjectType objectType)
		{
			this.m_memberName = memberName;
			this.m_token = token;
			this.m_objectType = objectType;
		}

		internal static bool Equals(MemberInfo a, MemberInfo b)
		{
			if (a != null && b != null)
			{
				if (a.MemberName == b.MemberName && a.Token == b.Token)
				{
					return a.ObjectType == b.ObjectType;
				}
				return false;
			}
			return false;
		}
	}
}
