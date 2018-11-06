namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class ChartMemberCollection : DataRegionMemberCollection<ChartMember>
	{
		public override string DefinitionPath
		{
			get
			{
				if (base.m_parentDefinitionPath is ChartMember)
				{
					return base.m_parentDefinitionPath.DefinitionPath + "xM";
				}
				return base.m_parentDefinitionPath.DefinitionPath;
			}
		}

		internal Chart OwnerChart
		{
			get
			{
				return base.m_owner as Chart;
			}
		}

		internal ChartMemberCollection(IDefinitionPath parentDefinitionPath, Chart owner)
			: base(parentDefinitionPath, (ReportItem)owner)
		{
		}
	}
}
