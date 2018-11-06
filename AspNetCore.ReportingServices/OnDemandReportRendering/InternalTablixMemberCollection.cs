using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalTablixMemberCollection : TablixMemberCollection
	{
		private TablixMember m_parent;

		private TablixMemberList m_memberDefs;

		public override TablixMember this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					if (base.m_children == null)
					{
						base.m_children = new DataRegionMember[this.Count];
					}
					TablixMember tablixMember = (TablixMember)base.m_children[index];
					if (tablixMember == null)
					{
						IReportScope reportScope = (this.m_parent != null) ? this.m_parent.ReportScope : base.m_owner.ReportScope;
						tablixMember = (TablixMember)(base.m_children[index] = new InternalTablixMember(reportScope, this, base.OwnerTablix, this.m_parent, this.m_memberDefs[index], index));
					}
					return tablixMember;
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

		internal TablixMemberList MemberDefs
		{
			get
			{
				return this.m_memberDefs;
			}
		}

		internal InternalTablixMemberCollection(IDefinitionPath parentDefinitionPath, Tablix owner, TablixMember parent, TablixMemberList memberDefs)
			: base(parentDefinitionPath, owner)
		{
			this.m_parent = parent;
			this.m_memberDefs = memberDefs;
		}
	}
}
