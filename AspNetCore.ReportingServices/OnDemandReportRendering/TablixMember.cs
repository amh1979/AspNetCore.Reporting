using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class TablixMember : DataRegionMember
	{
		protected TablixMemberCollection m_children;

		protected Visibility m_visibility;

		protected TablixMemberInstance m_instance;

		protected TablixHeader m_header;

		public TablixMember Parent
		{
			get
			{
				return base.m_parent as TablixMember;
			}
		}

		public abstract string DataElementName
		{
			get;
		}

		public abstract DataElementOutputTypes DataElementOutput
		{
			get;
		}

		public abstract TablixHeader TablixHeader
		{
			get;
		}

		public abstract TablixMemberCollection Children
		{
			get;
		}

		public abstract bool FixedData
		{
			get;
		}

		public abstract KeepWithGroup KeepWithGroup
		{
			get;
		}

		public abstract bool RepeatOnNewPage
		{
			get;
		}

		public virtual bool KeepTogether
		{
			get
			{
				return false;
			}
		}

		public abstract bool IsColumn
		{
			get;
		}

		internal abstract int RowSpan
		{
			get;
		}

		internal abstract int ColSpan
		{
			get;
		}

		public abstract bool IsTotal
		{
			get;
		}

		internal abstract PageBreakLocation PropagatedGroupBreak
		{
			get;
		}

		public abstract Visibility Visibility
		{
			get;
		}

		public abstract bool HideIfNoRows
		{
			get;
		}

		internal abstract AspNetCore.ReportingServices.ReportIntermediateFormat.TablixMember MemberDefinition
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

		internal Tablix OwnerTablix
		{
			get
			{
				return base.m_owner as Tablix;
			}
		}

		public abstract TablixMemberInstance Instance
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

		internal TablixMember(IDefinitionPath parentDefinitionPath, Tablix owner, TablixMember parent, int parentCollectionIndex)
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
			if (this.m_header != null)
			{
				this.m_header.SetNewContext();
			}
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			AspNetCore.ReportingServices.ReportIntermediateFormat.TablixMember memberDefinition = this.MemberDefinition;
			if (memberDefinition != null)
			{
				memberDefinition.ResetVisibilityComputationCache();
			}
		}
	}
}
