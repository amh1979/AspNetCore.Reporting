using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalDataMember : DataMember
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataMember m_memberDef;

		private IReportScope m_reportScope;

		private string m_uniqueName;

		private bool m_customPropertyCollectionReady;

		internal override string UniqueName
		{
			get
			{
				if (this.m_uniqueName != null)
				{
					this.m_uniqueName = this.m_memberDef.UniqueName;
				}
				return this.m_uniqueName;
			}
		}

		public override int ColSpan
		{
			get
			{
				return this.m_memberDef.ColSpan;
			}
		}

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				if (base.m_customPropertyCollection == null)
				{
					string objectName = (this.m_memberDef.Grouping != null) ? this.m_memberDef.Grouping.Name : base.OwnerCri.Name;
					base.m_customPropertyCollection = new CustomPropertyCollection(this.ReportScope.ReportScopeInstance, base.OwnerCri.RenderingContext, null, this.m_memberDef, ObjectType.CustomReportItem, objectName);
					this.m_customPropertyCollectionReady = true;
				}
				else if (!this.m_customPropertyCollectionReady)
				{
					string objectName2 = (this.m_memberDef.Grouping != null) ? this.m_memberDef.Grouping.Name : base.OwnerCri.Name;
					base.m_customPropertyCollection.UpdateCustomProperties(this.ReportScope.ReportScopeInstance, this.m_memberDef, base.OwnerCri.RenderingContext.OdpContext, ObjectType.CustomReportItem, objectName2);
					this.m_customPropertyCollectionReady = true;
				}
				return base.m_customPropertyCollection;
			}
		}

		public override string ID
		{
			get
			{
				return this.m_memberDef.RenderingModelID;
			}
		}

		public override bool IsColumn
		{
			get
			{
				return this.m_memberDef.IsColumn;
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

		public override int MemberCellIndex
		{
			get
			{
				return this.m_memberDef.MemberCellIndex;
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

		public override int RowSpan
		{
			get
			{
				return this.m_memberDef.RowSpan;
			}
		}

		public override DataMemberInstance Instance
		{
			get
			{
				if (base.OwnerCri.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					if (this.IsStatic)
					{
						base.m_instance = new DataMemberInstance(base.OwnerCri, this);
					}
					else
					{
						DataDynamicMemberInstance instance = new DataDynamicMemberInstance(base.OwnerCri, this, this.BuildOdpMemberLogic(base.OwnerCri.RenderingContext.OdpContext));
						base.m_owner.RenderingContext.AddDynamicInstance(instance);
						base.m_instance = instance;
					}
				}
				return base.m_instance;
			}
		}

		internal override AspNetCore.ReportingServices.ReportIntermediateFormat.DataMember MemberDefinition
		{
			get
			{
				return this.m_memberDef;
			}
		}

		public override DataMemberCollection Children
		{
			get
			{
				DataMemberList subMembers = this.m_memberDef.SubMembers;
				if (subMembers == null)
				{
					return null;
				}
				if (base.m_children == null)
				{
					base.m_children = new InternalDataMemberCollection(this, base.OwnerCri, this, subMembers);
				}
				return base.m_children;
			}
		}

		internal InternalDataMember(IReportScope reportScope, IDefinitionPath parentDefinitionPath, CustomReportItem owner, DataMember parent, AspNetCore.ReportingServices.ReportIntermediateFormat.DataMember memberDef, int parentCollectionIndex)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex)
		{
			this.m_memberDef = memberDef;
			if (this.m_memberDef.IsStatic)
			{
				this.m_reportScope = reportScope;
			}
			base.m_group = new Group(owner, this.m_memberDef, this);
		}

		internal override void SetNewContext(bool fromMoveNext)
		{
			if (!fromMoveNext && base.m_instance != null && !this.IsStatic)
			{
				((IDynamicInstance)base.m_instance).ResetContext();
			}
			base.SetNewContext(fromMoveNext);
			this.m_customPropertyCollectionReady = false;
		}
	}
}
