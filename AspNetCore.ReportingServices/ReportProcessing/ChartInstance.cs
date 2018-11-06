using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartInstance : ReportItemInstance, IPageItem
	{
		private MultiChartInstanceList m_multiCharts;

		[NonSerialized]
		private int m_currentCellOuterIndex;

		[NonSerialized]
		private int m_currentCellInnerIndex;

		[NonSerialized]
		private int m_currentOuterStaticIndex;

		[NonSerialized]
		private int m_currentInnerStaticIndex;

		[NonSerialized]
		private int m_startPage = -1;

		[NonSerialized]
		private int m_endPage = -1;

		internal MultiChartInstanceList MultiCharts
		{
			get
			{
				return this.m_multiCharts;
			}
			set
			{
				this.m_multiCharts = value;
			}
		}

		private MultiChartInstance CurrentMultiChart
		{
			get
			{
				if (0 >= this.m_multiCharts.Count)
				{
					this.m_multiCharts.Add(new MultiChartInstance((Chart)base.m_reportItemDef));
				}
				return this.m_multiCharts[0];
			}
		}

		internal ChartHeadingInstanceList ColumnInstances
		{
			get
			{
				return this.CurrentMultiChart.ColumnInstances;
			}
		}

		internal ChartHeadingInstanceList RowInstances
		{
			get
			{
				return this.CurrentMultiChart.RowInstances;
			}
		}

		internal ChartDataPointInstancesList DataPoints
		{
			get
			{
				return this.CurrentMultiChart.DataPoints;
			}
		}

		internal int DataPointSeriesCount
		{
			get
			{
				if (this.CurrentMultiChart.DataPoints != null)
				{
					return this.CurrentMultiChart.DataPoints.Count;
				}
				return 0;
			}
		}

		internal int DataPointCategoryCount
		{
			get
			{
				ChartDataPointInstancesList dataPoints = this.CurrentMultiChart.DataPoints;
				if (dataPoints != null)
				{
					Global.Tracer.Assert(null != dataPoints[0]);
					return dataPoints[0].Count;
				}
				return 0;
			}
		}

		internal int CurrentCellOuterIndex
		{
			get
			{
				return this.m_currentCellOuterIndex;
			}
		}

		internal int CurrentCellInnerIndex
		{
			get
			{
				return this.m_currentCellInnerIndex;
			}
		}

		internal int CurrentOuterStaticIndex
		{
			set
			{
				this.m_currentOuterStaticIndex = value;
			}
		}

		internal int CurrentInnerStaticIndex
		{
			set
			{
				this.m_currentInnerStaticIndex = value;
			}
		}

		internal ChartHeadingInstanceList InnerHeadingInstanceList
		{
			get
			{
				return this.CurrentMultiChart.InnerHeadingInstanceList;
			}
			set
			{
				this.CurrentMultiChart.InnerHeadingInstanceList = value;
			}
		}

		int IPageItem.StartPage
		{
			get
			{
				return this.m_startPage;
			}
			set
			{
				this.m_startPage = value;
			}
		}

		int IPageItem.EndPage
		{
			get
			{
				return this.m_endPage;
			}
			set
			{
				this.m_endPage = value;
			}
		}

		internal ChartInstance(ReportProcessing.ProcessingContext pc, Chart reportItemDef)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			base.m_instanceInfo = new ChartInstanceInfo(pc, reportItemDef, this);
			pc.Pagination.EnterIgnoreHeight(reportItemDef.StartHidden);
			this.m_multiCharts = new MultiChartInstanceList();
			pc.QuickFind.Add(base.UniqueName, this);
		}

		internal ChartInstance()
		{
		}

		internal ChartDataPoint GetCellDataPoint(int cellDPIndex)
		{
			if (-1 == cellDPIndex)
			{
				cellDPIndex = this.GetCurrentCellDPIndex();
			}
			return ((Chart)base.m_reportItemDef).ChartDataPoints[cellDPIndex];
		}

		internal ChartDataPointInstance AddCell(ReportProcessing.ProcessingContext pc, int currCellDPIndex)
		{
			ChartDataPointInstancesList dataPoints = this.CurrentMultiChart.DataPoints;
			Chart chart = (Chart)base.m_reportItemDef;
			int num = (currCellDPIndex < 0) ? this.GetCurrentCellDPIndex() : currCellDPIndex;
			ChartDataPointInstance chartDataPointInstance = new ChartDataPointInstance(pc, chart, this.GetCellDataPoint(num), num);
			if (chart.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
			{
				dataPoints[this.m_currentCellOuterIndex].Add(chartDataPointInstance);
			}
			else
			{
				if (this.m_currentCellOuterIndex == 0)
				{
					Global.Tracer.Assert(dataPoints.Count == this.m_currentCellInnerIndex);
					ChartDataPointInstanceList value = new ChartDataPointInstanceList();
					dataPoints.Add(value);
				}
				dataPoints[this.m_currentCellInnerIndex].Add(chartDataPointInstance);
			}
			this.m_currentCellInnerIndex++;
			return chartDataPointInstance;
		}

		internal void NewOuterCells()
		{
			ChartDataPointInstancesList dataPoints = this.CurrentMultiChart.DataPoints;
			if (0 >= this.m_currentCellInnerIndex && dataPoints.Count != 0)
			{
				return;
			}
			Chart chart = (Chart)base.m_reportItemDef;
			if (chart.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
			{
				ChartDataPointInstanceList value = new ChartDataPointInstanceList();
				dataPoints.Add(value);
			}
			if (0 < this.m_currentCellInnerIndex)
			{
				this.m_currentCellOuterIndex++;
				this.m_currentCellInnerIndex = 0;
			}
		}

		internal int GetCurrentCellDPIndex()
		{
			Chart chart = (Chart)base.m_reportItemDef;
			int num = (chart.StaticColumns == null || chart.StaticColumns.Labels == null) ? 1 : chart.StaticColumns.Labels.Count;
			if (chart.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
			{
				return this.m_currentOuterStaticIndex * num + this.m_currentInnerStaticIndex;
			}
			return this.m_currentInnerStaticIndex * num + this.m_currentOuterStaticIndex;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.MultiCharts, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.MultiChartInstanceList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance, memberInfoList);
		}

		internal override ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(base.m_instanceInfo is OffsetInfo);
			return reader.ReadChartInstanceInfo((Chart)base.m_reportItemDef);
		}
	}
}
