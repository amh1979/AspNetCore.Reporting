namespace AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal class MemberInfo
	{
		private MemberName m_name;

		private Token m_token = Token.Object;

		private ObjectType m_type = ObjectType.None;

		private ObjectType m_containedType = ObjectType.None;

		private Lifetime m_lifetime = Lifetime.Unspecified;

		internal MemberName MemberName
		{
			get
			{
				return this.m_name;
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
				return this.m_type;
			}
		}

		internal ObjectType ContainedType
		{
			get
			{
				return this.m_containedType;
			}
		}

		internal Lifetime Lifetime
		{
			get
			{
				return this.m_lifetime;
			}
		}

		internal MemberInfo(MemberName name, Token token)
		{
			this.m_name = name;
			this.m_token = token;
		}

		internal MemberInfo(MemberName name, Token token, Lifetime lifetime)
		{
			this.m_name = name;
			this.m_token = token;
			this.m_lifetime = lifetime;
		}

		internal MemberInfo(MemberName name, ObjectType type)
		{
			this.m_name = name;
			this.m_type = type;
		}

		internal MemberInfo(MemberName name, ObjectType type, Lifetime lifetime)
		{
			this.m_name = name;
			this.m_type = type;
			this.m_lifetime = lifetime;
		}

		internal MemberInfo(MemberName name, ObjectType type, ObjectType containedType)
		{
			this.m_name = name;
			this.m_type = type;
			this.m_containedType = containedType;
		}

		internal MemberInfo(MemberName name, ObjectType type, ObjectType containedType, Lifetime lifetime)
		{
			this.m_name = name;
			this.m_type = type;
			this.m_containedType = containedType;
			this.m_lifetime = lifetime;
		}

		internal MemberInfo(MemberName name, ObjectType type, Token token)
		{
			this.m_name = name;
			this.m_token = token;
			this.m_type = type;
		}

		internal MemberInfo(MemberName name, ObjectType type, Token token, Lifetime lifetime)
		{
			this.m_name = name;
			this.m_token = token;
			this.m_type = type;
			this.m_lifetime = lifetime;
		}

		internal MemberInfo(MemberName name, ObjectType type, Token token, ObjectType containedType)
		{
			this.m_name = name;
			this.m_token = token;
			this.m_type = type;
			this.m_containedType = containedType;
		}

		internal MemberInfo(MemberName name, ObjectType type, Token token, ObjectType containedType, Lifetime lifetime)
		{
			this.m_name = name;
			this.m_token = token;
			this.m_type = type;
			this.m_containedType = containedType;
			this.m_lifetime = lifetime;
		}

		internal virtual bool IsWrittenForCompatVersion(int compatVersion)
		{
			return this.m_lifetime.IncludesVersion(compatVersion);
		}

		public override int GetHashCode()
		{
			return (int)this.m_name ^ (int)((uint)this.m_token << 8) ^ (int)this.m_type << 16 ^ (int)this.m_containedType << 24;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is MemberInfo)
			{
				return this.Equals((MemberInfo)obj);
			}
			return false;
		}

		internal bool Equals(MemberInfo otherMember)
		{
			if (otherMember != null && this.m_name == otherMember.m_name && this.m_token == otherMember.m_token && this.m_type == otherMember.m_type && this.m_containedType == otherMember.m_containedType)
			{
				return true;
			}
			return false;
		}
	}
}
