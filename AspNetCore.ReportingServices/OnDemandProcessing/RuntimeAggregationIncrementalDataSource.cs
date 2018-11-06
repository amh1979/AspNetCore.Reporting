using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class RuntimeAggregationIncrementalDataSource : RuntimeIncrementalDataSource
	{
		private RuntimeAggregationIncrementalDataSet m_runtimeDataSet;

		public IOnDemandScopeInstance GroupTreeRoot
		{
			get
			{
				return this.m_runtimeDataSet.GroupTreeRoot;
			}
		}

		protected override RuntimeIncrementalDataSet RuntimeDataSet
		{
			get
			{
				return this.m_runtimeDataSet;
			}
		}

		internal RuntimeAggregationIncrementalDataSource(DataSet dataSet, OnDemandProcessingContext odpContext)
			: base(odpContext.ReportDefinition, dataSet, odpContext)
		{
		}

		protected override List<RuntimeDataSet> CreateRuntimeDataSets()
		{
			DataSetInstance dataSetInstance = new DataSetInstance(base.m_dataSet);
			base.m_odpContext.CurrentReportInstance.SetDataSetInstance(dataSetInstance);
			this.m_runtimeDataSet = new RuntimeAggregationIncrementalDataSet(base.DataSourceDefinition, base.m_dataSet, dataSetInstance, base.OdpContext);
			List<RuntimeDataSet> list = new List<RuntimeDataSet>(1);
			list.Add(this.m_runtimeDataSet);
			return list;
		}

		public void CalculateDataSetAggregates()
		{
			try
			{
				this.m_runtimeDataSet.CalculateDataSetAggregates();
			}
			catch (Exception e)
			{
				base.HandleException(e);
				this.FinalCleanup();
				throw;
			}
		}
	}
}
