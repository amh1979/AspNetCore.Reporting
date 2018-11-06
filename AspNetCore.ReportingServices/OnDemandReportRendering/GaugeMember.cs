using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeMember : DataRegionMember
	{
		private GaugeMemberCollection m_children;

		private GaugeMemberInstance m_instance;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeMember m_memberDef;

		private IReportScope m_reportScope;

		private string m_uniqueName;

		public GaugeMember Parent
		{
			get
			{
				return base.m_parent as GaugeMember;
			}
		}

		internal override string UniqueName
		{
			get
			{
				if (this.m_uniqueName == null)
				{
					this.m_uniqueName = this.m_memberDef.UniqueName;
				}
				return this.m_uniqueName;
			}
		}

		public override string ID
		{
			get
			{
				return this.m_memberDef.RenderingModelID;
			}
		}

		public override bool IsStatic
		{
			get
			{
				if (this.m_memberDef.Grouping == null)
				{
					return true;
				}
				return false;
			}
		}

		public bool IsColumn
		{
			get
			{
				return this.m_memberDef.IsColumn;
			}
		}

		public int RowSpan
		{
			get
			{
				return this.m_memberDef.RowSpan;
			}
		}

		public int ColumnSpan
		{
			get
			{
				return this.m_memberDef.ColSpan;
			}
		}

		public override int MemberCellIndex
		{
			get
			{
				return this.m_memberDef.MemberCellIndex;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeMember MemberDefinition
		{
			get
			{
				return this.m_memberDef;
			}
		}

		internal override ReportHierarchyNode DataRegionMemberDefinition
		{
			get
			{
				return this.MemberDefinition;
			}
		}

		internal override IReportScope ReportScope
		{
			get
			{
				if (this.IsStatic)
				{
					return this.m_reportScope;
				}
				return this;
			}
		}

		internal override IRIFReportScope RIFReportScope
		{
			get
			{
				if (this.IsStatic)
				{
					return this.m_reportScope.RIFReportScope;
				}
				return this.MemberDefinition;
			}
		}

		internal override IReportScopeInstance ReportScopeInstance
		{
			get
			{
				if (this.IsStatic)
				{
					return this.m_reportScope.ReportScopeInstance;
				}
				return (IReportScopeInstance)this.Instance;
			}
		}

		internal GaugePanel OwnerGaugePanel
		{
			get
			{
				return base.m_owner as GaugePanel;
			}
		}

		public GaugeMemberInstance Instance
		{
			get
			{
				if (this.OwnerGaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					if (this.IsStatic)
					{
						this.m_instance = new GaugeMemberInstance(this.OwnerGaugePanel, this);
					}
					else
					{
						GaugeDynamicMemberInstance instance = new GaugeDynamicMemberInstance(this.OwnerGaugePanel, this, this.BuildOdpMemberLogic(this.OwnerGaugePanel.RenderingContext.OdpContext));
						base.m_owner.RenderingContext.AddDynamicInstance(instance);
						this.m_instance = instance;
					}
				}
				return this.m_instance;
			}
		}

		public GaugeMember ChildGaugeMember
		{
			get
			{
				if (this.m_children != null && this.m_children.Count == 1)
				{
					return ((ReportElementCollectionBase<GaugeMember>)this.m_children)[0];
				}
				return null;
			}
		}

		internal override IDataRegionMemberCollection SubMembers
		{
			get
			{
				if (this.m_children == null && this.m_memberDef.InnerHierarchy != null)
				{
					GaugeMemberList gaugeMemberList = (GaugeMemberList)this.m_memberDef.InnerHierarchy;
					if (gaugeMemberList == null)
					{
						return null;
					}
					this.m_children = new GaugeMemberCollection(this, this.OwnerGaugePanel, this, gaugeMemberList);
				}
				return this.m_children;
			}
		}

		internal GaugeMember(IReportScope reportScope, IDefinitionPath parentDefinitionPath, GaugePanel owner, GaugeMember parent, AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeMember memberDef)
			: base(parentDefinitionPath, owner, parent, 0)
		{
			this.m_memberDef = memberDef;
			if (this.m_memberDef.IsStatic)
			{
				this.m_reportScope = reportScope;
			}
			base.m_group = new Group(owner, this.m_memberDef, this);
		}

		internal GaugeMember(IDefinitionPath parentDefinitionPath, GaugePanel owner, GaugeMember parent)
			: base(parentDefinitionPath, owner, parent, 0)
		{
		}

		internal override bool GetIsColumn()
		{
			return this.IsColumn;
		}

		internal override void SetNewContext(bool fromMoveNext)
		{
			if (!fromMoveNext && this.m_instance != null && !this.IsStatic)
			{
				((IDynamicInstance)this.m_instance).ResetContext();
			}
			base.SetNewContext(fromMoveNext);
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
		}
	}
}
