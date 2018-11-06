using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class DataMember : DataRegionMember
	{
		protected DataMemberCollection m_children;

		protected DataMemberInstance m_instance;

		public DataMember Parent
		{
			get
			{
				return base.m_parent as DataMember;
			}
		}

		public virtual DataMemberCollection Children
		{
			get
			{
				return this.m_children;
			}
		}

		public abstract bool IsColumn
		{
			get;
		}

		public abstract int RowSpan
		{
			get;
		}

		public abstract int ColSpan
		{
			get;
		}

		internal abstract AspNetCore.ReportingServices.ReportIntermediateFormat.DataMember MemberDefinition
		{
			get;
		}

		internal override ReportHierarchyNode DataRegionMemberDefinition
		{
			get
			{
				return this.MemberDefinition;
			}
		}

		internal CustomReportItem OwnerCri
		{
			get
			{
				return base.m_owner as CustomReportItem;
			}
		}

		public abstract DataMemberInstance Instance
		{
			get;
		}

		internal override IDataRegionMemberCollection SubMembers
		{
			get
			{
				return this.m_children;
			}
		}

		internal DataMember(IDefinitionPath parentDefinitionPath, CustomReportItem owner, DataMember parent, int parentCollectionIndex)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex)
		{
		}

		internal override bool GetIsColumn()
		{
			return this.IsColumn;
		}

		internal override void SetNewContext(bool fromMoveNext)
		{
			base.SetNewContext(fromMoveNext);
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
		}
	}
}
