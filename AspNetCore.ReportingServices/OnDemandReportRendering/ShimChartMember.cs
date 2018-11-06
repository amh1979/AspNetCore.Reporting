using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimChartMember : ChartMember, IShimDataRegionMember
	{
		private bool m_isCategory;

		private int m_definitionStartIndex = -1;

		private int m_definitionEndIndex = -1;

		private AspNetCore.ReportingServices.ReportRendering.ChartMember m_staticOrSubtotal;

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
				if (this.IsStatic)
				{
					return base.DefinitionPath;
				}
				return ((AspNetCore.ReportingServices.ReportRendering.ChartMember)base.m_group.CurrentShimRenderGroup).ID;
			}
		}

		public override ReportStringProperty Label
		{
			get
			{
				if (base.m_label == null)
				{
					AspNetCore.ReportingServices.ReportProcessing.ExpressionInfo expressionInfo = null;
					expressionInfo = ((!this.IsStatic) ? ((AspNetCore.ReportingServices.ReportRendering.ChartMember)base.m_group.CurrentShimRenderGroup).LabelDefinition : this.m_staticOrSubtotal.LabelDefinition);
					base.m_label = new ReportStringProperty(expressionInfo);
				}
				return base.m_label;
			}
		}

		internal object LabelInstanceValue
		{
			get
			{
				if (this.IsStatic)
				{
					return this.m_staticOrSubtotal.LabelValue;
				}
				return ((AspNetCore.ReportingServices.ReportRendering.ChartMember)base.m_group.CurrentShimRenderGroup).LabelValue;
			}
		}

		public override string DataElementName
		{
			get
			{
				if (this.m_staticOrSubtotal != null)
				{
					return this.m_staticOrSubtotal.DataElementName;
				}
				if (base.m_group != null && base.m_group.CurrentShimRenderGroup != null)
				{
					return base.m_group.CurrentShimRenderGroup.DataCollectionName;
				}
				return null;
			}
		}

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (this.m_staticOrSubtotal != null)
				{
					return (DataElementOutputTypes)this.m_staticOrSubtotal.DataElementOutput;
				}
				return DataElementOutputTypes.Output;
			}
		}

		public override ChartMemberCollection Children
		{
			get
			{
				return base.m_children;
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
				return null != this.m_staticOrSubtotal;
			}
		}

		public override bool IsCategory
		{
			get
			{
				return this.m_isCategory;
			}
		}

		public override int SeriesSpan
		{
			get
			{
				if (this.m_isCategory)
				{
					return 1;
				}
				return this.m_definitionEndIndex - this.m_definitionStartIndex;
			}
		}

		public override int CategorySpan
		{
			get
			{
				if (this.m_isCategory)
				{
					return this.m_definitionEndIndex - this.m_definitionStartIndex;
				}
				return 1;
			}
		}

		public override int MemberCellIndex
		{
			get
			{
				return this.m_definitionStartIndex;
			}
		}

		public override bool IsTotal
		{
			get
			{
				return false;
			}
		}

		internal override AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMember MemberDefinition
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

		public override ChartMemberInstance Instance
		{
			get
			{
				if (base.OwnerChart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					if (this.IsStatic)
					{
						base.m_instance = new ChartMemberInstance(base.OwnerChart, this);
					}
					else
					{
						ChartDynamicMemberInstance instance = new ChartDynamicMemberInstance(base.OwnerChart, this, new InternalShimDynamicMemberLogic(this));
						base.m_owner.RenderingContext.AddDynamicInstance(instance);
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

		internal AspNetCore.ReportingServices.ReportRendering.ChartMember CurrentRenderChartMember
		{
			get
			{
				if (this.m_staticOrSubtotal != null)
				{
					return this.m_staticOrSubtotal;
				}
				return base.m_group.CurrentShimRenderGroup as AspNetCore.ReportingServices.ReportRendering.ChartMember;
			}
		}

		internal ShimChartMember(IDefinitionPath parentDefinitionPath, Chart owner, ShimChartMember parent, int parentCollectionIndex, bool isCategory, AspNetCore.ReportingServices.ReportRendering.ChartMember staticOrSubtotal)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex)
		{
			this.m_definitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			this.m_isCategory = isCategory;
			this.m_staticOrSubtotal = staticOrSubtotal;
			this.GenerateInnerHierarchy(owner, parent, isCategory, staticOrSubtotal.Children);
			this.m_definitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		internal ShimChartMember(IDefinitionPath parentDefinitionPath, Chart owner, ShimChartMember parent, int parentCollectionIndex, bool isCategory, ShimRenderGroups renderGroups)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex)
		{
			this.m_definitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			this.m_isCategory = isCategory;
			base.m_group = new Group(owner, renderGroups);
			this.GenerateInnerHierarchy(owner, parent, isCategory, ((AspNetCore.ReportingServices.ReportRendering.ChartMember)renderGroups[0]).Children);
			this.m_definitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		private void GenerateInnerHierarchy(Chart owner, ShimChartMember parent, bool isCategory, AspNetCore.ReportingServices.ReportRendering.ChartMemberCollection children)
		{
			if (children != null)
			{
				base.m_children = new ShimChartMemberCollection(this, owner, isCategory, this, children);
			}
			else
			{
				owner.GetAndIncrementMemberCellDefinitionIndex();
			}
		}

		internal bool SetNewContext(int index)
		{
			base.ResetContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (base.m_group != null)
			{
				if (base.OwnerChart.RenderChart.NoRows)
				{
					return false;
				}
				if (index >= 0 && index < base.m_group.RenderGroups.Count)
				{
					base.m_group.CurrentRenderGroupIndex = index;
					this.UpdateInnerContext(base.m_group.RenderGroups[index] as AspNetCore.ReportingServices.ReportRendering.ChartMember);
					return true;
				}
				return false;
			}
			return index <= 1;
		}

		internal override void ResetContext()
		{
			this.ResetContext(null, null);
		}

		internal void ResetContext(AspNetCore.ReportingServices.ReportRendering.ChartMember staticOrSubtotal, ShimRenderGroups renderGroups)
		{
			if (base.m_group != null)
			{
				base.m_group.CurrentRenderGroupIndex = -1;
				if (renderGroups != null)
				{
					base.m_group.RenderGroups = renderGroups;
				}
			}
			else if (staticOrSubtotal != null)
			{
				this.m_staticOrSubtotal = staticOrSubtotal;
			}
			AspNetCore.ReportingServices.ReportRendering.ChartMember currentRenderGroup = this.IsStatic ? this.m_staticOrSubtotal : (base.m_group.CurrentShimRenderGroup as AspNetCore.ReportingServices.ReportRendering.ChartMember);
			this.UpdateInnerContext(currentRenderGroup);
		}

		private void UpdateInnerContext(AspNetCore.ReportingServices.ReportRendering.ChartMember currentRenderGroup)
		{
			if (base.m_children != null)
			{
				((ShimChartMemberCollection)base.m_children).ResetContext(currentRenderGroup.Children);
			}
			else
			{
				((ShimChartSeriesCollection)base.OwnerChart.ChartData.SeriesCollection).UpdateCells(this);
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
