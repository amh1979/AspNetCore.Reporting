using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class RuntimeLiveReaderDataSource : RuntimeIncrementalDataSource
	{
		private DataSetInstance m_dataSetInstance;

		private RuntimeLiveDataReaderDataSet m_runtimeDataSet;

		public DataSetInstance DataSetInstance
		{
			get
			{
				return this.m_dataSetInstance;
			}
		}

		protected override RuntimeIncrementalDataSet RuntimeDataSet
		{
			get
			{
				return this.m_runtimeDataSet;
			}
		}

		internal RuntimeLiveReaderDataSource(Report report, DataSet dataSet, OnDemandProcessingContext odpContext)
			: base(report, dataSet, odpContext)
		{
		}

		protected override List<RuntimeDataSet> CreateRuntimeDataSets()
		{
			this.m_dataSetInstance = new DataSetInstance(base.m_dataSet);
			base.m_odpContext.CurrentReportInstance.SetDataSetInstance(this.m_dataSetInstance);
			this.m_runtimeDataSet = new RuntimeLiveDataReaderDataSet(base.DataSourceDefinition, base.m_dataSet, this.m_dataSetInstance, base.OdpContext);
			List<RuntimeDataSet> list = new List<RuntimeDataSet>(1);
			list.Add(this.m_runtimeDataSet);
			return list;
		}

		public RecordRow ReadNextRow()
		{
			try
			{
				return this.m_runtimeDataSet.ReadNextRow();
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
