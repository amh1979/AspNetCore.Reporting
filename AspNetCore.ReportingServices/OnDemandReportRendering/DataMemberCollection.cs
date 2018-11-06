namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class DataMemberCollection : DataRegionMemberCollection<DataMember>
	{
		public override string DefinitionPath
		{
			get
			{
				if (base.m_parentDefinitionPath is DataMember)
				{
					return base.m_parentDefinitionPath.DefinitionPath + "xM";
				}
				return base.m_parentDefinitionPath.DefinitionPath;
			}
		}

		internal CustomReportItem OwnerCri
		{
			get
			{
				return base.m_owner as CustomReportItem;
			}
		}

		internal DataMemberCollection(IDefinitionPath parentDefinitionPath, CustomReportItem owner)
			: base(parentDefinitionPath, (ReportItem)owner)
		{
		}
	}
}
