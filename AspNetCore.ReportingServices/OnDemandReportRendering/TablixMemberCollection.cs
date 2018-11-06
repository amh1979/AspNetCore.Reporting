namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class TablixMemberCollection : DataRegionMemberCollection<TablixMember>
	{
		public override string DefinitionPath
		{
			get
			{
				if (base.m_parentDefinitionPath is TablixMember)
				{
					return base.m_parentDefinitionPath.DefinitionPath + "xM";
				}
				return base.m_parentDefinitionPath.DefinitionPath;
			}
		}

		internal Tablix OwnerTablix
		{
			get
			{
				return base.m_owner as Tablix;
			}
		}

		internal virtual double SizeDelta
		{
			get
			{
				return 0.0;
			}
		}

		internal TablixMemberCollection(IDefinitionPath parentDefinitionPath, Tablix owner)
			: base(parentDefinitionPath, (ReportItem)owner)
		{
		}
	}
}
