using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimDataMember : DataMember, IShimDataRegionMember
	{
		private bool m_isColumn;

		private bool m_isStatic;

		private int m_staticIndex = -1;

		private int m_definitionStartIndex = -1;

		private int m_definitionEndIndex = -1;

		private AspNetCore.ReportingServices.ReportRendering.DataMemberCollection m_renderMembers;

		internal override string UniqueName
		{
			get
			{
				return this.ID;
			}
		}

		public override string ID
		{
			get
			{
				if (this.m_isStatic)
				{
					return this.m_renderMembers[this.m_staticIndex].ID;
				}
				return ((AspNetCore.ReportingServices.ReportRendering.DataMember)base.m_group.CurrentShimRenderGroup).ID;
			}
		}

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				if (base.m_customPropertyCollection == null)
				{
					if (base.m_group != null && base.m_group.CustomProperties != null)
					{
						base.m_customPropertyCollection = base.m_group.CustomProperties;
					}
					else
					{
						base.m_customPropertyCollection = new CustomPropertyCollection();
					}
				}
				return base.m_customPropertyCollection;
			}
		}

		public override bool IsStatic
		{
			get
			{
				return this.m_isStatic;
			}
		}

		public override bool IsColumn
		{
			get
			{
				return this.m_isColumn;
			}
		}

		public override int RowSpan
		{
			get
			{
				if (this.m_isColumn)
				{
					if (this.m_isStatic)
					{
						return this.m_renderMembers[this.m_staticIndex].MemberHeadingSpan;
					}
					return ((AspNetCore.ReportingServices.ReportRendering.DataMember)base.m_group.CurrentShimRenderGroup).MemberHeadingSpan;
				}
				return this.m_definitionEndIndex - this.m_definitionStartIndex;
			}
		}

		public override int ColSpan
		{
			get
			{
				if (this.m_isColumn)
				{
					return this.m_definitionEndIndex - this.m_definitionStartIndex;
				}
				if (this.IsStatic)
				{
					return this.m_renderMembers[this.m_staticIndex].MemberHeadingSpan;
				}
				return ((AspNetCore.ReportingServices.ReportRendering.DataMember)base.m_group.CurrentShimRenderGroup).MemberHeadingSpan;
			}
		}

		public override int MemberCellIndex
		{
			get
			{
				return this.m_definitionStartIndex;
			}
		}

		internal override AspNetCore.ReportingServices.ReportIntermediateFormat.DataMember MemberDefinition
		{
			get
			{
				return null;
			}
		}

		internal override IRIFReportScope RIFReportScope
		{
			get
			{
				return null;
			}
		}

		internal override IReportScopeInstance ReportScopeInstance
		{
			get
			{
				return null;
			}
		}

		internal override IReportScope ReportScope
		{
			get
			{
				return null;
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
						DataDynamicMemberInstance instance = new DataDynamicMemberInstance(base.OwnerCri, this, new InternalShimDynamicMemberLogic(this));
						base.OwnerCri.RenderingContext.AddDynamicInstance(instance);
						base.m_instance = instance;
					}
				}
				return base.m_instance;
			}
		}

		internal int DefinitionStartIndex
		{
			get
			{
				return this.m_definitionStartIndex;
			}
		}

		internal int DefinitionEndIndex
		{
			get
			{
				return this.m_definitionEndIndex;
			}
		}

		internal AspNetCore.ReportingServices.ReportRendering.DataMember CurrentRenderDataMember
		{
			get
			{
				if (this.m_isStatic)
				{
					return this.m_renderMembers[this.m_staticIndex];
				}
				return base.m_group.CurrentShimRenderGroup as AspNetCore.ReportingServices.ReportRendering.DataMember;
			}
		}

		internal ShimDataMember(IDefinitionPath parentDefinitionPath, CustomReportItem owner, ShimDataMember parent, int parentCollectionIndex, bool isColumn, bool isStatic, AspNetCore.ReportingServices.ReportRendering.DataMemberCollection renderMembers, int staticIndex)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex)
		{
			this.m_definitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			this.m_isColumn = isColumn;
			this.m_isStatic = isStatic;
			this.m_renderMembers = renderMembers;
			this.m_staticIndex = staticIndex;
			DataGroupingCollection children;
			if (isStatic)
			{
				children = renderMembers[staticIndex].Children;
			}
			else
			{
				base.m_group = new Group(owner, new ShimRenderGroups(renderMembers));
				children = renderMembers[0].Children;
			}
			if (children != null)
			{
				base.m_children = new ShimDataMemberCollection(this, owner, isColumn, this, children);
			}
			else
			{
				owner.GetAndIncrementMemberCellDefinitionIndex();
			}
			this.m_definitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		internal bool SetNewContext(int index)
		{
			base.ResetContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (this.m_isStatic)
			{
				return index <= 1;
			}
			if (base.OwnerCri.RenderCri.CustomData.NoRows)
			{
				return false;
			}
			if (index >= 0 && index < base.m_group.RenderGroups.Count)
			{
				base.m_group.CurrentRenderGroupIndex = index;
				this.UpdateInnerContext(base.m_group.RenderGroups[index] as AspNetCore.ReportingServices.ReportRendering.DataMember);
				return true;
			}
			return false;
		}

		internal override void ResetContext()
		{
			this.ResetContext(null);
		}

		internal void ResetContext(AspNetCore.ReportingServices.ReportRendering.DataMemberCollection renderMembers)
		{
			if (renderMembers != null)
			{
				this.m_renderMembers = renderMembers;
			}
			if (base.m_group != null)
			{
				base.m_group.CurrentRenderGroupIndex = -1;
			}
			AspNetCore.ReportingServices.ReportRendering.DataMember currentRenderMember = this.IsStatic ? this.m_renderMembers[this.m_staticIndex] : (base.m_group.CurrentShimRenderGroup as AspNetCore.ReportingServices.ReportRendering.DataMember);
			this.UpdateInnerContext(currentRenderMember);
		}

		private void UpdateInnerContext(AspNetCore.ReportingServices.ReportRendering.DataMember currentRenderMember)
		{
			if (base.m_children != null)
			{
				((ShimDataMemberCollection)base.m_children).ResetContext(currentRenderMember.Children);
			}
			else
			{
				((ShimDataRowCollection)base.OwnerCri.CustomData.RowCollection).UpdateCells(this);
			}
		}

		bool IShimDataRegionMember.SetNewContext(int index)
		{
			return this.SetNewContext(index);
		}

		void IShimDataRegionMember.ResetContext()
		{
			this.ResetContext();
		}
	}
}
