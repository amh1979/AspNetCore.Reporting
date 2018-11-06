using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalChartMemberCollection : ChartMemberCollection
	{
		private ChartMember m_parent;

		private ChartMemberList m_memberDefs;

		public override ChartMember this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					if (base.m_children == null)
					{
						base.m_children = new DataRegionMember[this.Count];
					}
					ChartMember chartMember = (ChartMember)base.m_children[index];
					if (chartMember == null)
					{
						IReportScope reportScope = (this.m_parent != null) ? this.m_parent.ReportScope : base.m_owner.ReportScope;
						chartMember = (ChartMember)(base.m_children[index] = new InternalChartMember(reportScope, this, base.OwnerChart, this.m_parent, this.m_memberDefs[index], index));
					}
					return chartMember;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_memberDefs.OriginalNodeCount;
			}
		}

		internal InternalChartMemberCollection(IDefinitionPath parentDefinitionPath, Chart owner, ChartMember parent, ChartMemberList memberDefs)
			: base(parentDefinitionPath, owner)
		{
			this.m_parent = parent;
			this.m_memberDefs = memberDefs;
		}
	}
}
