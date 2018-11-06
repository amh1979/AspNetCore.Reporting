using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class OWCChartColumn
	{
		private ChartColumn m_chartColumnDef;

		private ArrayList m_data;

		public string Name
		{
			get
			{
				return this.m_chartColumnDef.Name;
			}
		}

		public object this[int index]
		{
			get
			{
				if (0 <= index && index < this.m_data.Count)
				{
					if (this.m_chartColumnDef.Value.Type == ExpressionInfo.Types.Constant)
					{
						return this.m_chartColumnDef.Value.Value;
					}
					if (this.m_data != null)
					{
						return this.m_data[index];
					}
					return null;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public int Count
		{
			get
			{
				if (this.m_data != null)
				{
					return this.m_data.Count;
				}
				return 0;
			}
		}

		internal OWCChartColumn(ChartColumn chartColumnDef, ArrayList columnData)
		{
			this.m_chartColumnDef = chartColumnDef;
			this.m_data = columnData;
		}
	}
}
