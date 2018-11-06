using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimChartMemberCollection : ChartMemberCollection
	{
		private bool m_isDynamic;

		private bool m_isCategoryGroup;

		private int m_definitionStartIndex = -1;

		private int m_definitionEndIndex = -1;

		public override ChartMember this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					return (ChartMember)base.m_children[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return base.m_children.Length;
			}
		}

		internal ShimChartMemberCollection(IDefinitionPath parentDefinitionPath, Chart owner, bool isCategoryGroup, ShimChartMember parent, AspNetCore.ReportingServices.ReportRendering.ChartMemberCollection renderMemberCollection)
			: base(parentDefinitionPath, owner)
		{
			this.m_isCategoryGroup = isCategoryGroup;
			this.m_definitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			int count = renderMemberCollection.Count;
			if (renderMemberCollection[0].IsStatic)
			{
				this.m_isDynamic = false;
				base.m_children = new ShimChartMember[count];
				for (int i = 0; i < count; i++)
				{
					base.m_children[i] = new ShimChartMember(this, owner, parent, i, isCategoryGroup, renderMemberCollection[i]);
				}
			}
			else
			{
				this.m_isDynamic = true;
				base.m_children = new ShimChartMember[1];
				base.m_children[0] = new ShimChartMember(this, owner, parent, 0, isCategoryGroup, new ShimRenderGroups(renderMemberCollection));
			}
			this.m_definitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		internal void UpdateContext()
		{
			if (base.m_children != null)
			{
				if (this.m_isCategoryGroup)
				{
					this.ResetContext(base.OwnerChart.RenderChart.CategoryMemberCollection);
				}
				else
				{
					this.ResetContext(base.OwnerChart.RenderChart.SeriesMemberCollection);
				}
			}
		}

		internal void ResetContext(AspNetCore.ReportingServices.ReportRendering.ChartMemberCollection newRenderMemberCollection)
		{
			if (base.m_children != null)
			{
				if (this.m_isDynamic)
				{
					ShimRenderGroups renderGroups = (newRenderMemberCollection != null) ? new ShimRenderGroups(newRenderMemberCollection) : null;
					((ShimChartMember)base.m_children[0]).ResetContext(null, renderGroups);
				}
				else
				{
					for (int i = 0; i < base.m_children.Length; i++)
					{
						AspNetCore.ReportingServices.ReportRendering.ChartMember staticOrSubtotal = (newRenderMemberCollection != null) ? newRenderMemberCollection[i] : null;
						((ShimChartMember)base.m_children[i]).ResetContext(staticOrSubtotal, null);
					}
				}
			}
		}
	}
}
