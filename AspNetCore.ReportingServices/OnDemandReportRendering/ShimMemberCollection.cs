namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class ShimMemberCollection : TablixMemberCollection
	{
		protected bool m_isColumnGroup;

		internal ShimMemberCollection(IDefinitionPath parentDefinitionPath, Tablix owner, bool isColumnGroup)
			: base(parentDefinitionPath, owner)
		{
			this.m_isColumnGroup = isColumnGroup;
		}
	}
}
