using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class Group : IPageBreakItem
	{
		private bool m_isOldSnapshot;

		private bool m_isDetailGroup;

		private DataRegion m_ownerItem;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode m_memberDef;

		private PageBreak m_pageBreak;

		private ReportStringProperty m_pageName;

		private GroupExpressionCollection m_groupExpressions;

		private CustomPropertyCollection m_customProperties;

		private ReportStringProperty m_documentMapLabel;

		private GroupInstance m_instance;

		private DataRegionMember m_dataMember;

		private ShimTablixMember m_dynamicMember;

		private ShimTableMember m_tableDetailMember;

		private CustomReportItem m_criOwner;

		private ShimRenderGroups m_renderGroups;

		private AspNetCore.ReportingServices.ReportRendering.Group m_currentRenderGroupCache;

		private int m_currentRenderGroupIndex = -1;

		public string ID
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					if (this.m_currentRenderGroupIndex >= 0 && this.m_renderGroups != null)
					{
						return this.m_renderGroups[this.m_currentRenderGroupIndex].ID;
					}
					return null;
				}
				if (this.m_memberDef != null)
				{
					return this.m_memberDef.RenderingModelID;
				}
				return null;
			}
		}

		public string Name
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					if (this.m_renderGroups == null)
					{
						return null;
					}
					return this.CurrentShimRenderGroup.Name;
				}
				if (this.m_memberDef != null && this.m_memberDef.Grouping != null)
				{
					return this.m_memberDef.Grouping.Name;
				}
				return null;
			}
		}

		public ReportStringProperty DocumentMapLabel
		{
			get
			{
				if (this.m_documentMapLabel == null)
				{
					if (!this.m_isOldSnapshot)
					{
						if (this.m_memberDef != null && this.m_memberDef.Grouping != null)
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo groupLabel = this.m_memberDef.Grouping.GroupLabel;
							if (groupLabel != null)
							{
								this.m_documentMapLabel = new ReportStringProperty(groupLabel.IsExpression, groupLabel.OriginalText, groupLabel.StringValue);
							}
							else
							{
								this.m_documentMapLabel = new ReportStringProperty();
							}
							goto IL_009e;
						}
						return null;
					}
					if (this.m_renderGroups == null)
					{
						return null;
					}
					if (this.CurrentShimRenderGroup.m_groupingDef != null)
					{
						AspNetCore.ReportingServices.ReportProcessing.ExpressionInfo groupLabel2 = this.CurrentShimRenderGroup.m_groupingDef.GroupLabel;
						this.m_documentMapLabel = new ReportStringProperty(groupLabel2);
					}
				}
				goto IL_009e;
				IL_009e:
				return this.m_documentMapLabel;
			}
		}

		public PageBreak PageBreak
		{
			get
			{
				if (this.m_pageBreak == null)
				{
					RenderingContext renderingContext = (this.m_criOwner != null) ? this.m_criOwner.RenderingContext : this.m_ownerItem.RenderingContext;
					if (this.IsOldSnapshot)
					{
						if (this.m_dynamicMember != null)
						{
							this.m_pageBreak = new PageBreak(renderingContext, this.m_dataMember, this.m_dynamicMember.PropagatedGroupBreak);
						}
						else
						{
							this.m_pageBreak = new PageBreak(renderingContext, null, PageBreakLocation.None);
						}
					}
					else if (this.m_memberDef != null && this.m_memberDef.Grouping != null)
					{
						this.m_pageBreak = new PageBreak(renderingContext, this.m_dataMember, this.m_memberDef.Grouping);
					}
					else
					{
						this.m_pageBreak = new PageBreak(renderingContext, this.m_dataMember, PageBreakLocation.None);
					}
				}
				return this.m_pageBreak;
			}
		}

		public ReportStringProperty PageName
		{
			get
			{
				if (this.m_pageName == null)
				{
					if (this.m_isOldSnapshot)
					{
						this.m_pageName = new ReportStringProperty();
					}
					else if (this.m_memberDef != null && this.m_memberDef.Grouping != null)
					{
						this.m_pageName = new ReportStringProperty(this.m_memberDef.Grouping.PageName);
					}
					else
					{
						this.m_pageName = new ReportStringProperty();
					}
				}
				return this.m_pageName;
			}
		}

		public GroupExpressionCollection GroupExpressions
		{
			get
			{
				if (this.m_groupExpressions == null)
				{
					if (this.m_isOldSnapshot)
					{
						if (this.CurrentShimRenderGroup != null && this.CurrentShimRenderGroup.m_groupingDef != null)
						{
							this.m_groupExpressions = new GroupExpressionCollection(this.CurrentShimRenderGroup.m_groupingDef);
						}
					}
					else
					{
						this.m_groupExpressions = new GroupExpressionCollection(this.m_memberDef.Grouping);
					}
				}
				return this.m_groupExpressions;
			}
		}

		internal CustomPropertyCollection CustomProperties
		{
			get
			{
				if (this.m_customProperties == null && this.m_isOldSnapshot && this.CurrentShimRenderGroup != null && this.CurrentShimRenderGroup.CustomProperties != null)
				{
					RenderingContext renderingContext = (this.m_criOwner != null) ? this.m_criOwner.RenderingContext : this.m_ownerItem.RenderingContext;
					this.m_customProperties = new CustomPropertyCollection(renderingContext, this.CurrentShimRenderGroup.CustomProperties);
					if (this.m_currentRenderGroupIndex < 0)
					{
						this.m_customProperties.UpdateCustomProperties(null);
					}
				}
				return this.m_customProperties;
			}
		}

		public string DataElementName
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					if (this.m_renderGroups != null)
					{
						if (this.m_criOwner == null)
						{
							return this.CurrentShimRenderGroup.DataElementName;
						}
						return null;
					}
					if (DataRegion.Type.Table == this.m_ownerItem.DataRegionType)
					{
						return ((Tablix)this.m_ownerItem).RenderTable.DetailDataElementName;
					}
					return null;
				}
				if (this.m_memberDef != null && this.m_memberDef.Grouping != null)
				{
					return this.m_memberDef.Grouping.DataElementName;
				}
				return null;
			}
		}

		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					if (this.m_renderGroups != null)
					{
						if (this.m_criOwner == null)
						{
							return (DataElementOutputTypes)this.CurrentShimRenderGroup.DataElementOutput;
						}
						return DataElementOutputTypes.Output;
					}
					if (DataRegion.Type.Table == this.m_ownerItem.DataRegionType)
					{
						return (DataElementOutputTypes)((Tablix)this.m_ownerItem).RenderTable.DetailDataElementOutput;
					}
					return DataElementOutputTypes.Output;
				}
				if (this.m_memberDef != null && this.m_memberDef.Grouping != null)
				{
					return this.m_memberDef.Grouping.DataElementOutput;
				}
				return DataElementOutputTypes.Output;
			}
		}

		public bool IsRecursive
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					if (this.m_renderGroups != null)
					{
						AspNetCore.ReportingServices.ReportProcessing.Grouping groupingDef = this.CurrentShimRenderGroup.m_groupingDef;
						if (groupingDef != null)
						{
							if (groupingDef.Parent != null)
							{
								return groupingDef.Parent.Count > 0;
							}
							return false;
						}
					}
				}
				else if (this.m_memberDef != null && this.m_memberDef.Grouping != null)
				{
					if (this.m_memberDef.Grouping.Parent != null)
					{
						return this.m_memberDef.Grouping.Parent.Count > 0;
					}
					return false;
				}
				return false;
			}
		}

		internal DataRegion OwnerDataRegion
		{
			get
			{
				return this.m_ownerItem;
			}
		}

		[Obsolete("Use PageBreak.BreakLocation instead.")]
		PageBreakLocation IPageBreakItem.PageBreakLocation
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					if (this.m_dynamicMember != null)
					{
						return this.m_dynamicMember.PropagatedGroupBreak;
					}
				}
				else if (this.m_memberDef != null && this.m_memberDef.Grouping != null && this.m_memberDef.Grouping.PageBreak != null)
				{
					return this.m_memberDef.Grouping.PageBreak.BreakLocation;
				}
				return PageBreakLocation.None;
			}
		}

		internal ShimRenderGroups RenderGroups
		{
			get
			{
				return this.m_renderGroups;
			}
			set
			{
				this.m_renderGroups = value;
			}
		}

		internal int CurrentRenderGroupIndex
		{
			get
			{
				return this.m_currentRenderGroupIndex;
			}
			set
			{
				if (value != 0 || this.m_currentRenderGroupIndex != -1)
				{
					this.m_currentRenderGroupCache = null;
				}
				this.m_currentRenderGroupIndex = value;
				if (this.m_instance != null)
				{
					this.m_instance.SetNewContext();
				}
				if (this.m_isOldSnapshot && this.m_renderGroups != null && this.m_customProperties != null)
				{
					if (this.m_currentRenderGroupIndex < 0)
					{
						this.m_customProperties.UpdateCustomProperties(null);
					}
					else
					{
						this.m_customProperties.UpdateCustomProperties(this.m_renderGroups[this.m_currentRenderGroupIndex].CustomProperties);
					}
				}
			}
		}

		internal AspNetCore.ReportingServices.ReportRendering.Group CurrentShimRenderGroup
		{
			get
			{
				if (this.m_isOldSnapshot && this.m_renderGroups != null)
				{
					if (this.m_currentRenderGroupCache == null)
					{
						if (this.m_currentRenderGroupIndex < 0)
						{
							this.m_currentRenderGroupCache = this.m_renderGroups[0];
						}
						else
						{
							this.m_currentRenderGroupCache = this.m_renderGroups[this.m_currentRenderGroupIndex];
						}
					}
					return this.m_currentRenderGroupCache;
				}
				return null;
			}
		}

		internal bool IsOldSnapshot
		{
			get
			{
				return this.m_isOldSnapshot;
			}
		}

		internal bool IsDetailGroup
		{
			get
			{
				return this.m_isDetailGroup;
			}
		}

		internal ShimTableMember TableDetailMember
		{
			get
			{
				return this.m_tableDetailMember;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode MemberDefinition
		{
			get
			{
				return this.m_memberDef;
			}
		}

		public GroupInstance Instance
		{
			get
			{
				if (this.m_ownerItem != null && this.m_ownerItem.RenderingContext.InstanceAccessDisallowed)
				{
					goto IL_0034;
				}
				if (this.m_criOwner != null && this.m_criOwner.RenderingContext.InstanceAccessDisallowed)
				{
					goto IL_0034;
				}
				if (this.m_instance == null)
				{
					if (this.m_isOldSnapshot)
					{
						this.m_instance = new GroupInstance(this);
					}
					else
					{
						this.m_instance = new GroupInstance(this, this.m_dataMember);
					}
				}
				return this.m_instance;
				IL_0034:
				return null;
			}
		}

		internal Group(DataRegion owner, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode memberDef, DataRegionMember dataMember)
		{
			this.m_isOldSnapshot = false;
			this.m_ownerItem = owner;
			this.m_memberDef = memberDef;
			this.m_dataMember = dataMember;
		}

		internal Group(CustomReportItem owner, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode memberDef, DataRegionMember dataMember)
		{
			this.m_isOldSnapshot = false;
			this.m_criOwner = owner;
			this.m_memberDef = memberDef;
			this.m_dataMember = dataMember;
		}

		internal Group(DataRegion owner, ShimRenderGroups renderGroups, ShimTablixMember dynamicMember)
		{
			this.m_isOldSnapshot = true;
			this.m_ownerItem = owner;
			this.m_renderGroups = renderGroups;
			this.m_dynamicMember = dynamicMember;
		}

		internal Group(DataRegion owner, ShimRenderGroups renderGroups)
		{
			this.m_isOldSnapshot = true;
			this.m_ownerItem = owner;
			this.m_renderGroups = renderGroups;
		}

		internal Group(DataRegion owner, ShimTableMember tableDetailMember)
		{
			this.m_isOldSnapshot = true;
			this.m_isDetailGroup = true;
			this.m_tableDetailMember = tableDetailMember;
			this.m_dynamicMember = tableDetailMember;
			this.m_ownerItem = owner;
			this.m_renderGroups = null;
		}

		internal Group(CustomReportItem owner, ShimRenderGroups renderGroups)
		{
			this.m_isOldSnapshot = true;
			this.m_renderGroups = renderGroups;
			this.m_criOwner = owner;
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			if (this.m_pageBreak != null)
			{
				this.m_pageBreak.SetNewContext();
			}
		}
	}
}
