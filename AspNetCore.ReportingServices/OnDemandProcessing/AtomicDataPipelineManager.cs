using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal abstract class AtomicDataPipelineManager : DataPipelineManager
	{
		private RuntimeDataSourceDataProcessing m_runtimeDataSource;

		public override IOnDemandScopeInstance GroupTreeRoot
		{
			get
			{
				return this.m_runtimeDataSource.RuntimeDataSet.GroupTreeRoot;
			}
		}

		protected override RuntimeDataSource RuntimeDataSource
		{
			get
			{
				return this.m_runtimeDataSource;
			}
		}

		public AtomicDataPipelineManager(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
			: base(odpContext, dataSet)
		{
		}

		protected override void InternalStartProcessing()
		{
			Global.Tracer.Assert(this.m_runtimeDataSource == null, "Cannot StartProcessing twice for the same pipeline manager");
			this.m_runtimeDataSource = this.CreateDataSource();
			this.m_runtimeDataSource.ProcessSingleOdp();
			base.m_odpContext.CheckAndThrowIfAborted();
		}

		protected abstract RuntimeDataSourceDataProcessing CreateDataSource();

		protected override void InternalStopProcessing()
		{
			this.m_runtimeDataSource = null;
		}

		public override void Advance()
		{
		}
	}
}
