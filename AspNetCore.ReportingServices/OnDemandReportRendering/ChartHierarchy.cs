namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartHierarchy : MemberHierarchy<ChartMember>
	{
		private Chart OwnerChart
		{
			get
			{
				return base.m_owner as Chart;
			}
		}

		public ChartMemberCollection MemberCollection
		{
			get
			{
				if (base.m_members == null)
				{
					if (this.OwnerChart.IsOldSnapshot)
					{
						this.OwnerChart.ResetMemberCellDefinitionIndex(0);
						base.m_members = new ShimChartMemberCollection(this, this.OwnerChart, base.m_isColumn, null, base.m_isColumn ? this.OwnerChart.RenderChart.CategoryMemberCollection : this.OwnerChart.RenderChart.SeriesMemberCollection);
					}
					else
					{
						this.OwnerChart.ResetMemberCellDefinitionIndex(0);
						base.m_members = new InternalChartMemberCollection(this, this.OwnerChart, null, base.m_isColumn ? this.OwnerChart.ChartDef.CategoryMembers : this.OwnerChart.ChartDef.SeriesMembers);
					}
				}
				return (ChartMemberCollection)base.m_members;
			}
		}

		internal ChartHierarchy(Chart owner, bool isColumn)
			: base((ReportItem)owner, isColumn)
		{
		}

		internal override void ResetContext()
		{
			if (base.m_members != null)
			{
				if (this.OwnerChart.IsOldSnapshot)
				{
					((ShimChartMemberCollection)base.m_members).UpdateContext();
				}
				else
				{
					((IDataRegionMemberCollection)base.m_members).SetNewContext();
				}
			}
		}
	}
}
