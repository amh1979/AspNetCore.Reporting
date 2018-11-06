using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class IncrementalDataPipelineManager : DataPipelineManager
	{
		private RuntimeDataSourceIncrementalDataProcessing m_runtimeDataSource;

		public override IOnDemandScopeInstance GroupTreeRoot
		{
			get
			{
				return this.m_runtimeDataSource.GroupTreeRoot;
			}
		}

		protected override RuntimeDataSource RuntimeDataSource
		{
			get
			{
				return this.m_runtimeDataSource;
			}
		}

		public IncrementalDataPipelineManager(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
			: base(odpContext, dataSet)
		{
		}

		protected override void InternalStartProcessing()
		{
			Global.Tracer.Assert(this.m_runtimeDataSource == null, "Cannot StartProcessing twice for the same pipeline manager");
			this.m_runtimeDataSource = new RuntimeDataSourceIncrementalDataProcessing(base.m_dataSet, base.m_odpContext);
			this.m_runtimeDataSource.Initialize();
		}

		protected override void InternalStopProcessing()
		{
			if (this.m_runtimeDataSource != null)
			{
				this.m_runtimeDataSource.Teardown();
				base.m_odpContext.ReportRuntime.CurrentScope = null;
			}
		}

		public override void Abort()
		{
			base.Abort();
			if (this.m_runtimeDataSource != null)
			{
				this.m_runtimeDataSource.Abort();
			}
		}

		public override void Advance()
		{
			this.m_runtimeDataSource.Advance();
		}
	}
}
