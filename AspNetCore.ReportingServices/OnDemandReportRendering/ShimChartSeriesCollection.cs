using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimChartSeriesCollection : ChartSeriesCollection
	{
		private List<ShimChartSeries> m_series;

		public override ChartSeries this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					return this.m_series[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_series.Count;
			}
		}

		internal ShimChartSeriesCollection(Chart owner)
			: base(owner)
		{
			this.m_series = new List<ShimChartSeries>();
			this.AppendChartSeries(null, owner.SeriesHierarchy.MemberCollection as ShimChartMemberCollection);
		}

		private void AppendChartSeries(ShimChartMember seriesParentMember, ShimChartMemberCollection seriesMembers)
		{
			if (seriesMembers == null)
			{
				this.m_series.Add(new ShimChartSeries(base.m_owner, this.m_series.Count, seriesParentMember));
			}
			else
			{
				int count = seriesMembers.Count;
				for (int i = 0; i < count; i++)
				{
					ShimChartMember shimChartMember = ((ReportElementCollectionBase<ChartMember>)seriesMembers)[i] as ShimChartMember;
					this.AppendChartSeries(shimChartMember, shimChartMember.Children as ShimChartMemberCollection);
				}
			}
		}

		internal void UpdateCells(ShimChartMember innermostMember)
		{
			if (innermostMember != null && innermostMember.Children == null)
			{
				if (!innermostMember.IsCategory)
				{
					int memberCellIndex = innermostMember.MemberCellIndex;
					int count = this.m_series[memberCellIndex].Count;
					for (int i = 0; i < count; i++)
					{
						((ShimChartDataPoint)((ReportElementCollectionBase<ChartDataPoint>)this.m_series[memberCellIndex])[i]).SetNewContext();
					}
				}
				else
				{
					int memberCellIndex2 = innermostMember.MemberCellIndex;
					int count2 = this.m_series.Count;
					for (int j = 0; j < count2; j++)
					{
						((ShimChartDataPoint)((ReportElementCollectionBase<ChartDataPoint>)this.m_series[j])[memberCellIndex2]).SetNewContext();
					}
				}
			}
		}
	}
}
