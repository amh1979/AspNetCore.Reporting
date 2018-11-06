using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class StreamingAtomicDataPipelineManager : AtomicDataPipelineManager
	{
		public StreamingAtomicDataPipelineManager(OnDemandProcessingContext odpContext, DataSet dataSet)
			: base(odpContext, dataSet)
		{
		}

		protected override RuntimeDataSourceDataProcessing CreateDataSource()
		{
			return new RuntimeDataSourceStreamingDataProcessing(base.m_dataSet, base.m_odpContext);
		}

		public override void Abort()
		{
			base.Abort();
			if (this.RuntimeDataSource != null)
			{
				this.RuntimeDataSource.Abort();
			}
		}
	}
}
