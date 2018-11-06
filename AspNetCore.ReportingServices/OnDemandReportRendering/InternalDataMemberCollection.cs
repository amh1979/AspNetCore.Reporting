using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalDataMemberCollection : DataMemberCollection
	{
		private DataMember m_parent;

		private DataMemberList m_memberDefs;

		public override DataMember this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					if (base.m_children == null)
					{
						base.m_children = new DataRegionMember[this.Count];
					}
					DataMember dataMember = (DataMember)base.m_children[index];
					if (dataMember == null)
					{
						IReportScope reportScope = (this.m_parent != null) ? this.m_parent.ReportScope : base.m_owner.ReportScope;
						dataMember = (DataMember)(base.m_children[index] = new InternalDataMember(reportScope, this, base.OwnerCri, this.m_parent, this.m_memberDefs[index], index));
					}
					return dataMember;
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

		internal InternalDataMemberCollection(IDefinitionPath parentDefinitionPath, CustomReportItem owner, DataMember parent, DataMemberList memberDefs)
			: base(parentDefinitionPath, owner)
		{
			this.m_parent = parent;
			this.m_memberDefs = memberDefs;
		}
	}
}
