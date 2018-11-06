using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimListMember : ShimTablixMember
	{
		public override string ID
		{
			get
			{
				return base.DefinitionPath;
			}
		}

		public override TablixMemberCollection Children
		{
			get
			{
				return null;
			}
		}

		public override bool IsStatic
		{
			get
			{
				if (base.m_group != null)
				{
					return null == base.m_group.RenderGroups;
				}
				return true;
			}
		}

		internal override int RowSpan
		{
			get
			{
				if (this.IsColumn)
				{
					return 0;
				}
				return 1;
			}
		}

		internal override int ColSpan
		{
			get
			{
				if (this.IsColumn)
				{
					return 1;
				}
				return 0;
			}
		}

		public override int MemberCellIndex
		{
			get
			{
				return 0;
			}
		}

		public override bool KeepTogether
		{
			get
			{
				return true;
			}
		}

		public override bool IsTotal
		{
			get
			{
				return false;
			}
		}

		public override TablixHeader TablixHeader
		{
			get
			{
				return null;
			}
		}

		public override Visibility Visibility
		{
			get
			{
				if (base.m_visibility == null && !this.IsColumn && base.OwnerTablix.RenderList.ReportItemDef.Visibility != null)
				{
					base.m_visibility = new ShimListMemberVisibility(this);
				}
				return base.m_visibility;
			}
		}

		internal override PageBreakLocation PropagatedGroupBreak
		{
			get
			{
				if (this.IsStatic)
				{
					return PageBreakLocation.None;
				}
				return base.m_propagatedPageBreak;
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
						TablixDynamicMemberInstance instance = new TablixDynamicMemberInstance(base.OwnerTablix, this, new InternalShimDynamicMemberLogic(this));
						base.m_owner.RenderingContext.AddDynamicInstance(instance);
						base.m_instance = instance;
					}
				}
				return base.m_instance;
			}
		}

		internal ShimListMember(IDefinitionPath parentDefinitionPath, Tablix owner, ShimRenderGroups renderGroups, int parentCollectionIndex, bool isColumn)
			: base(parentDefinitionPath, owner, null, parentCollectionIndex, isColumn)
		{
			base.m_group = new Group(owner, renderGroups, this);
		}

		internal override bool SetNewContext(int index)
		{
			base.ResetContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (base.m_group != null && base.m_group.RenderGroups != null)
			{
				if (base.OwnerTablix.RenderList.NoRows)
				{
					return false;
				}
				if (index >= 0 && index < base.m_group.RenderGroups.Count)
				{
					base.m_group.CurrentRenderGroupIndex = index;
					((ShimListRow)((ReportElementCollectionBase<TablixRow>)(ShimListRowCollection)base.OwnerTablix.Body.RowCollection)[0]).UpdateCells(base.m_group.RenderGroups[index] as ListContent);
					return true;
				}
				return false;
			}
			return index <= 1;
		}

		internal override void ResetContext()
		{
			base.ResetContext();
			if (base.m_group.CurrentRenderGroupIndex >= 0)
			{
				this.ResetContext(null);
			}
		}

		internal void ResetContext(ShimRenderGroups renderGroups)
		{
			if (base.m_group != null)
			{
				base.m_group.CurrentRenderGroupIndex = -1;
				if (renderGroups != null)
				{
					base.m_group.RenderGroups = renderGroups;
				}
			}
		}
	}
}
