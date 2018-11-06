using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class DataSetAggregateDataPipelineManager : DataPipelineManager
	{
		private RuntimeAggregationIncrementalDataSource m_runtimeDataSource;

		public override IOnDemandScopeInstance GroupTreeRoot
		{
			get
			{
				Global.Tracer.Assert(false, "DataSetAggregateDataPipelineManager GroupTreeRoot property must not be accessed");
				throw new NotImplementedException();
			}
		}

		protected override RuntimeDataSource RuntimeDataSource
		{
			get
			{
				return this.m_runtimeDataSource;
			}
		}

		public DataSetAggregateDataPipelineManager(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
			: base(odpContext, dataSet)
		{
		}

		protected override void InternalStartProcessing()
		{
			Global.Tracer.Assert(this.m_runtimeDataSource == null, "Cannot StartProcessing twice for the same pipeline manager");
			this.m_runtimeDataSource = new RuntimeAggregationIncrementalDataSource(base.m_dataSet, base.m_odpContext);
			this.m_runtimeDataSource.Initialize();
			this.m_runtimeDataSource.CalculateDataSetAggregates();
		}

		protected override void InternalStopProcessing()
		{
			if (this.m_runtimeDataSource != null)
			{
				this.m_runtimeDataSource.Teardown();
				base.m_odpContext.ReportRuntime.CurrentScope = null;
			}
		}

		public override void Advance()
		{
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
