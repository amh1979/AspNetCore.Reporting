using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalTablixMember : TablixMember
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.TablixMember m_memberDef;

		private bool m_customPropertyCollectionReady;

		private IReportScope m_reportScope;

		internal override string UniqueName
		{
			get
			{
				return this.m_memberDef.UniqueName;
			}
		}

		public override string ID
		{
			get
			{
				return this.m_memberDef.RenderingModelID;
			}
		}

		public override string DataElementName
		{
			get
			{
				return this.m_memberDef.DataElementName;
			}
		}

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				return this.m_memberDef.DataElementOutput;
			}
		}

		public override TablixHeader TablixHeader
		{
			get
			{
				if (base.m_header == null && this.m_memberDef.TablixHeader != null)
				{
					base.m_header = new TablixHeader(base.OwnerTablix, this);
				}
				return base.m_header;
			}
		}

		public override TablixMemberCollection Children
		{
			get
			{
				TablixMemberList subMembers = this.m_memberDef.SubMembers;
				if (subMembers == null)
				{
					return null;
				}
				if (base.m_children == null)
				{
					base.m_children = new InternalTablixMemberCollection(this, base.OwnerTablix, this, subMembers);
				}
				return base.m_children;
			}
		}

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				if (base.m_customPropertyCollection == null)
				{
					string objectName = (this.m_memberDef.Grouping != null) ? this.m_memberDef.Grouping.Name : base.OwnerTablix.Name;
					base.m_customPropertyCollection = new CustomPropertyCollection(this.ReportScope.ReportScopeInstance, base.OwnerTablix.RenderingContext, null, this.m_memberDef, ObjectType.Tablix, objectName);
					this.m_customPropertyCollectionReady = true;
				}
				else if (!this.m_customPropertyCollectionReady)
				{
					string objectName2 = (this.m_memberDef.Grouping != null) ? this.m_memberDef.Grouping.Name : base.OwnerTablix.Name;
					base.m_customPropertyCollection.UpdateCustomProperties(this.ReportScope.ReportScopeInstance, this.m_memberDef, base.OwnerTablix.RenderingContext.OdpContext, ObjectType.Tablix, objectName2);
					this.m_customPropertyCollectionReady = true;
				}
				return base.m_customPropertyCollection;
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

		public override bool IsColumn
		{
			get
			{
				return this.m_memberDef.IsColumn;
			}
		}

		internal override int RowSpan
		{
			get
			{
				return this.m_memberDef.RowSpan;
			}
		}

		internal override int ColSpan
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

		public override bool IsTotal
		{
			get
			{
				return this.m_memberDef.IsAutoSubtotal;
			}
		}

		internal override PageBreakLocation PropagatedGroupBreak
		{
			get
			{
				if (!this.IsStatic)
				{
					PageBreak pageBreak = base.m_group.PageBreak;
					if (pageBreak.Instance != null && !pageBreak.Instance.Disabled)
					{
						return pageBreak.BreakLocation;
					}
				}
				return PageBreakLocation.None;
			}
		}

		public override Visibility Visibility
		{
			get
			{
				if (base.m_visibility == null && this.m_memberDef.Visibility != null && !this.m_memberDef.IsAutoSubtotal)
				{
					base.m_visibility = new InternalTablixMemberVisibility(this);
				}
				return base.m_visibility;
			}
		}

		public override bool HideIfNoRows
		{
			get
			{
				return this.m_memberDef.HideIfNoRows;
			}
		}

		public override bool KeepTogether
		{
			get
			{
				return this.m_memberDef.KeepTogether;
			}
		}

		internal override AspNetCore.ReportingServices.ReportIntermediateFormat.TablixMember MemberDefinition
		{
			get
			{
				return this.m_memberDef;
			}
		}

		public override bool FixedData
		{
			get
			{
				return this.MemberDefinition.FixedData;
			}
		}

		public override KeepWithGroup KeepWithGroup
		{
			get
			{
				return this.MemberDefinition.KeepWithGroup;
			}
		}

		public override bool RepeatOnNewPage
		{
			get
			{
				return this.MemberDefinition.RepeatOnNewPage;
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

		public override TablixMemberInstance Instance
		{
			get
			{
				if (base.OwnerTablix.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					if (this.IsStatic)
					{
						base.m_instance = new TablixMemberInstance(base.OwnerTablix, this);
					}
					else
					{
						TablixDynamicMemberInstance instance = new TablixDynamicMemberInstance(base.OwnerTablix, this, this.BuildOdpMemberLogic(base.OwnerTablix.RenderingContext.OdpContext));
						base.m_owner.RenderingContext.AddDynamicInstance(instance);
						base.m_instance = instance;
					}
				}
				return base.m_instance;
			}
		}

		internal InternalTablixMember(IReportScope reportScope, IDefinitionPath parentDefinitionPath, Tablix owner, TablixMember parent, AspNetCore.ReportingServices.ReportIntermediateFormat.TablixMember memberDef, int index)
			: base(parentDefinitionPath, owner, parent, index)
		{
			if (memberDef.IsStatic)
			{
				this.m_reportScope = reportScope;
			}
			base.m_owner = owner;
			this.m_memberDef = memberDef;
			if (this.m_memberDef.Grouping != null)
			{
				base.m_group = new Group(base.OwnerTablix, this.m_memberDef, this);
			}
			this.m_memberDef.ROMScopeInstance = this.ReportScope.ReportScopeInstance;
			this.m_memberDef.ResetVisibilityComputationCache();
		}

		internal override void SetNewContext(bool fromMoveNext)
		{
			if (!fromMoveNext && base.m_instance != null && !this.IsStatic)
			{
				((IDynamicInstance)base.m_instance).ResetContext();
			}
			base.SetNewContext(fromMoveNext);
			this.m_customPropertyCollectionReady = false;
			this.m_memberDef.ResetTextBoxImpls(base.m_owner.m_renderingContext.OdpContext);
		}
	}
}
