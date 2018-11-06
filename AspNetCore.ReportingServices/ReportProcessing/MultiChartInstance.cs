using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MultiChartInstance
	{
		private ChartHeadingInstanceList m_columnInstances;

		private ChartHeadingInstanceList m_rowInstances;

		private ChartDataPointInstancesList m_cellDataPoints;

		[NonSerialized]
		private ChartHeadingInstanceList m_innerHeadingInstanceList;

		internal ChartHeadingInstanceList ColumnInstances
		{
			get
			{
				return this.m_columnInstances;
			}
			set
			{
				this.m_columnInstances = value;
			}
		}

		internal ChartHeadingInstanceList RowInstances
		{
			get
			{
				return this.m_rowInstances;
			}
			set
			{
				this.m_rowInstances = value;
			}
		}

		internal ChartDataPointInstancesList DataPoints
		{
			get
			{
				return this.m_cellDataPoints;
			}
			set
			{
				this.m_cellDataPoints = value;
			}
		}

		internal ChartHeadingInstanceList InnerHeadingInstanceList
		{
			get
			{
				return this.m_innerHeadingInstanceList;
			}
			set
			{
				this.m_innerHeadingInstanceList = value;
			}
		}

		internal MultiChartInstance(Chart reportItemDef)
		{
			this.m_columnInstances = new ChartHeadingInstanceList();
			this.m_rowInstances = new ChartHeadingInstanceList();
			this.m_cellDataPoints = new ChartDataPointInstancesList();
		}

		internal MultiChartInstance()
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ColumnInstances, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartHeadingInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.RowInstances, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartHeadingInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.CellDataPoints, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartDataPointInstancesList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
