using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeMemberCollection : DataRegionMemberCollection<GaugeMember>
	{
		private GaugeMember m_parent;

		private GaugeMemberList m_memberDefs;

		public override string DefinitionPath
		{
			get
			{
				if (base.m_parentDefinitionPath is GaugeMember)
				{
					return base.m_parentDefinitionPath.DefinitionPath + "xM";
				}
				return base.m_parentDefinitionPath.DefinitionPath;
			}
		}

		internal GaugePanel OwnerGaugePanel
		{
			get
			{
				return base.m_owner as GaugePanel;
			}
		}

		public override GaugeMember this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					if (base.m_children == null)
					{
						base.m_children = new DataRegionMember[this.Count];
					}
					GaugeMember gaugeMember = (GaugeMember)base.m_children[index];
					if (gaugeMember == null)
					{
						IReportScope reportScope = (this.m_parent != null) ? this.m_parent.ReportScope : base.m_owner.ReportScope;
						gaugeMember = (GaugeMember)(base.m_children[index] = new GaugeMember(reportScope, this, this.OwnerGaugePanel, this.m_parent, this.m_memberDefs[index]));
					}
					return gaugeMember;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return 1;
			}
		}

		internal GaugeMemberCollection(IDefinitionPath parentDefinitionPath, GaugePanel owner)
			: base(parentDefinitionPath, (ReportItem)owner)
		{
		}

		internal GaugeMemberCollection(IDefinitionPath parentDefinitionPath, GaugePanel owner, GaugeMember parent, GaugeMemberList memberDefs)
			: base(parentDefinitionPath, (ReportItem)owner)
		{
			this.m_parent = parent;
			this.m_memberDefs = memberDefs;
		}
	}
}
