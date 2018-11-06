namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class DataMemberInstance : BaseInstance
	{
		protected CustomReportItem m_owner;

		protected DataMember m_memberDef;

		internal DataMemberInstance(CustomReportItem owner, DataMember memberDef)
			: base(memberDef.ReportScope)
		{
			this.m_owner = owner;
			this.m_memberDef = memberDef;
		}

		protected override void ResetInstanceCache()
		{
		}
	}
}
