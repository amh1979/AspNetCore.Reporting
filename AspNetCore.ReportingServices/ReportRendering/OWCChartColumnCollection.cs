using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class OWCChartColumnCollection
	{
		private ReportItem m_owner;

		private AspNetCore.ReportingServices.ReportProcessing.OWCChart m_chartDef;

		private OWCChartInstance m_chartInstance;

		private OWCChartColumn[] m_chartData;

		public OWCChartColumn this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					OWCChartColumn oWCChartColumn = null;
					if (this.m_chartData == null || this.m_chartData[index] == null)
					{
						ArrayList columnData = null;
						if (this.m_chartInstance != null)
						{
							columnData = ((OWCChartInstanceInfo)this.m_owner.InstanceInfo)[index];
						}
						oWCChartColumn = new OWCChartColumn(this.m_chartDef.ChartData[index], columnData);
						if (this.m_owner.RenderingContext.CacheState)
						{
							if (this.m_chartData == null)
							{
								this.m_chartData = new OWCChartColumn[this.m_chartDef.ChartData.Count];
							}
							this.m_chartData[index] = oWCChartColumn;
						}
					}
					else
					{
						oWCChartColumn = this.m_chartData[index];
					}
					return oWCChartColumn;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public int Count
		{
			get
			{
				return this.m_chartDef.ChartData.Count;
			}
		}

		internal OWCChartColumnCollection(AspNetCore.ReportingServices.ReportProcessing.OWCChart chartDef, OWCChartInstance chartInstance, ReportItem owner)
		{
			this.m_owner = owner;
			this.m_chartInstance = chartInstance;
			this.m_chartDef = chartDef;
		}
	}
}
