using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapMemberCollection : DataRegionMemberCollection<MapMember>
	{
		private MapMember m_parent;

		private MapMemberList m_memberDefs;

		public override string DefinitionPath
		{
			get
			{
				if (base.m_parentDefinitionPath is MapMember)
				{
					return base.m_parentDefinitionPath.DefinitionPath + "xM";
				}
				return base.m_parentDefinitionPath.DefinitionPath;
			}
		}

		internal MapDataRegion OwnerMapDataRegion
		{
			get
			{
				return base.m_owner as MapDataRegion;
			}
		}

		public override MapMember this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					if (base.m_children == null)
					{
						base.m_children = new DataRegionMember[this.Count];
					}
					MapMember mapMember = (MapMember)base.m_children[index];
					if (mapMember == null)
					{
						IReportScope reportScope = (this.m_parent != null) ? this.m_parent.ReportScope : base.m_owner.ReportScope;
						mapMember = (MapMember)(base.m_children[index] = new MapMember(reportScope, this, this.OwnerMapDataRegion, this.m_parent, this.m_memberDefs[index]));
					}
					return mapMember;
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

		internal MapMemberCollection(IDefinitionPath parentDefinitionPath, MapDataRegion owner)
			: base(parentDefinitionPath, (ReportItem)owner)
		{
		}

		internal MapMemberCollection(IDefinitionPath parentDefinitionPath, MapDataRegion owner, MapMember parent, MapMemberList memberDefs)
			: base(parentDefinitionPath, (ReportItem)owner)
		{
			this.m_parent = parent;
			this.m_memberDefs = memberDefs;
		}
	}
}
