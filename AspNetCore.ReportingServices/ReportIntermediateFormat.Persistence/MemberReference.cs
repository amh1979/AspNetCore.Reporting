namespace AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal class MemberReference
	{
		private MemberName m_memberName;

		private int m_refID;

		internal MemberName MemberName
		{
			get
			{
				return this.m_memberName;
			}
		}

		internal int RefID
		{
			get
			{
				return this.m_refID;
			}
		}

		internal MemberReference(MemberName memberName, int refID)
		{
			this.m_memberName = memberName;
			this.m_refID = refID;
		}
	}
}
