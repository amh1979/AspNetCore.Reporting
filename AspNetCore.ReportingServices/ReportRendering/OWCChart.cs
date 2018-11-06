using AspNetCore.ReportingServices.ReportProcessing;
using System.Drawing.Imaging;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class OWCChart : DataRegion
	{
		private OWCChartColumnCollection m_chartData;

		public OWCChartColumnCollection ChartData
		{
			get
			{
				OWCChartColumnCollection oWCChartColumnCollection = this.m_chartData;
				if (this.m_chartData == null)
				{
					oWCChartColumnCollection = new OWCChartColumnCollection((AspNetCore.ReportingServices.ReportProcessing.OWCChart)base.ReportItemDef, (OWCChartInstance)base.ReportItemInstance, this);
					if (base.RenderingContext.CacheState)
					{
						this.m_chartData = oWCChartColumnCollection;
					}
				}
				return oWCChartColumnCollection;
			}
		}

		public string ChartDefinition
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.OWCChart)base.ReportItemDef).ChartDefinition;
			}
		}

		public override bool NoRows
		{
			get
			{
				int count = ((AspNetCore.ReportingServices.ReportProcessing.OWCChart)base.ReportItemDef).ChartData.Count;
				bool result = true;
				if (count > 0)
				{
					OWCChartInstanceInfo oWCChartInstanceInfo = (OWCChartInstanceInfo)base.InstanceInfo;
					int num = 0;
					while (num < count)
					{
						if (0 >= oWCChartInstanceInfo[num].Count)
						{
							num++;
							continue;
						}
						result = false;
						break;
					}
				}
				return result;
			}
		}

		internal override string InstanceInfoNoRowMessage
		{
			get
			{
				if (base.InstanceInfo != null)
				{
					return ((OWCChartInstanceInfo)base.InstanceInfo).NoRows;
				}
				return null;
			}
		}

		internal OWCChart(int intUniqueName, AspNetCore.ReportingServices.ReportProcessing.OWCChart reportItemDef, OWCChartInstance reportItemInstance, RenderingContext renderingContext)
			: base(intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
		}

		public void ChartDataXML(IChartStream chartStream)
		{
			((OWCChartInstanceInfo)base.InstanceInfo).ChartDataXML(chartStream);
		}

		internal bool ProcessChartXMLPivotList(ref string newDefinition, string chartDataUrl)
		{
			return false;
		}

		public Metafile GetImage()
		{
			return null;
		}

		public byte[] GetChart()
		{
			return null;
		}
	}
}
