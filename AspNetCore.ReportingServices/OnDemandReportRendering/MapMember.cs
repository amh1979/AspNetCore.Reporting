using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapMember : DataRegionMember
	{
		private MapMemberCollection m_children;

		private MapMemberInstance m_instance;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapMember m_memberDef;

		private IReportScope m_reportScope;

		public MapMember Parent
		{
			get
			{
				return base.m_parent as MapMember;
			}
		}

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

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapMember MemberDefinition
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

		internal MapDataRegion OwnerMapDataRegion
		{
			get
			{
				return base.m_owner as MapDataRegion;
			}
		}

		public MapMemberInstance Instance
		{
			get
			{
				if (this.OwnerMapDataRegion.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					if (this.IsStatic)
					{
						this.m_instance = new MapMemberInstance(this.OwnerMapDataRegion, this);
					}
					else
					{
						MapDynamicMemberInstance instance = new MapDynamicMemberInstance(this.OwnerMapDataRegion, this, this.BuildOdpMemberLogic(this.OwnerMapDataRegion.RenderingContext.OdpContext));
						base.m_owner.RenderingContext.AddDynamicInstance(instance);
						this.m_instance = instance;
					}
				}
				return this.m_instance;
			}
		}

		public MapMember ChildMapMember
		{
			get
			{
				if (this.m_children != null && this.m_children.Count == 1)
				{
					return ((ReportElementCollectionBase<MapMember>)this.m_children)[0];
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
					MapMemberList mapMemberList = (MapMemberList)this.m_memberDef.InnerHierarchy;
					if (mapMemberList == null)
					{
						return null;
					}
					this.m_children = new MapMemberCollection(this, this.OwnerMapDataRegion, this, mapMemberList);
				}
				return this.m_children;
			}
		}

		internal MapMember(IReportScope reportScope, IDefinitionPath parentDefinitionPath, MapDataRegion owner, MapMember parent, AspNetCore.ReportingServices.ReportIntermediateFormat.MapMember memberDef)
			: base(parentDefinitionPath, owner, parent, 0)
		{
			this.m_memberDef = memberDef;
			if (this.m_memberDef.IsStatic)
			{
				this.m_reportScope = reportScope;
			}
			if (this.m_memberDef.Grouping != null)
			{
				base.m_group = new Group(owner, this.m_memberDef, this);
			}
		}

		internal MapMember(IDefinitionPath parentDefinitionPath, MapDataRegion owner, MapMember parent)
			: base(parentDefinitionPath, owner, parent, 0)
		{
		}

		internal override bool GetIsColumn()
		{
			return this.m_memberDef.IsColumn;
		}

		private List<MapVectorLayer> GetChildLayers()
		{
			return ((MapDataRegion)base.m_owner).GetChildLayers();
		}

		internal override void SetNewContext(bool fromMoveNext)
		{
			if (!fromMoveNext && this.m_instance != null && !this.IsStatic)
			{
				((IDynamicInstance)this.m_instance).ResetContext();
			}
			base.SetNewContext(fromMoveNext);
			if (this.ChildMapMember == null)
			{
				List<MapVectorLayer> childLayers = this.GetChildLayers();
				foreach (MapVectorLayer item in childLayers)
				{
					item.SetNewContext();
				}
			}
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
		}
	}
}
