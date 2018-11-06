namespace AspNetCore.ReportingServices.ReportProcessing.Persistence
{
	internal sealed class Declaration
	{
		private ObjectType m_baseType;

		private MemberInfoList m_members;

		internal ObjectType BaseType
		{
			get
			{
				return this.m_baseType;
			}
		}

		internal MemberInfoList Members
		{
			get
			{
				return this.m_members;
			}
		}

		internal Declaration(ObjectType baseType, MemberInfoList members)
		{
			this.m_baseType = baseType;
			Global.Tracer.Assert(null != members);
			this.m_members = members;
		}
	}
}
