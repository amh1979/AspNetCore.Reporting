using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class RuntimeDataSourceStreamingDataProcessing : RuntimeDataSourceDataProcessing
	{
		internal RuntimeDataSourceStreamingDataProcessing(DataSet dataSet, OnDemandProcessingContext processingContext)
			: base(dataSet, processingContext)
		{
		}

		protected override RuntimeOnDemandDataSet CreateRuntimeDataSet()
		{
			DataSetInstance dataSetInstance = new DataSetInstance(base.m_dataSet);
			base.m_odpContext.CurrentReportInstance.SetDataSetInstance(dataSetInstance);
			return new RuntimeOnDemandDataSet(base.DataSourceDefinition, base.m_dataSet, dataSetInstance, base.m_odpContext, true, false, false);
		}
	}
}
