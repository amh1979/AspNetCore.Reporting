using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class DataHierarchy : MemberHierarchy<DataMember>
	{
		private CustomReportItem OwnerCri
		{
			get
			{
				return base.m_owner as CustomReportItem;
			}
		}

		public DataMemberCollection MemberCollection
		{
			get
			{
				if (base.m_members == null)
				{
					if (this.OwnerCri.IsOldSnapshot)
					{
						this.OwnerCri.ResetMemberCellDefinitionIndex(0);
						DataGroupingCollection definitionGroups = base.m_isColumn ? this.OwnerCri.RenderCri.CustomData.DataColumnGroupings : this.OwnerCri.RenderCri.CustomData.DataRowGroupings;
						base.m_members = new ShimDataMemberCollection(this, this.OwnerCri, base.m_isColumn, null, definitionGroups);
					}
					else
					{
						DataMemberList memberDefs = base.m_isColumn ? this.OwnerCri.CriDef.DataColumnMembers : this.OwnerCri.CriDef.DataRowMembers;
						base.m_members = new InternalDataMemberCollection(this, this.OwnerCri, null, memberDefs);
					}
				}
				return (DataMemberCollection)base.m_members;
			}
		}

		internal DataHierarchy(CustomReportItem owner, bool isColumn)
			: base((ReportItem)owner, isColumn)
		{
		}

		internal override void ResetContext()
		{
			if (base.m_members != null)
			{
				((ShimDataMemberCollection)base.m_members).UpdateContext();
			}
		}
	}
}
