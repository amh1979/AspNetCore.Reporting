using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class RuntimeIdcIncrementalDataSource : RuntimeIncrementalDataSource
	{
		private RuntimeIdcIncrementalDataSet m_runtimeDataSet;

		protected override RuntimeIncrementalDataSet RuntimeDataSet
		{
			get
			{
				return this.m_runtimeDataSet;
			}
		}

		internal RuntimeIdcIncrementalDataSource(DataSet dataSet, OnDemandProcessingContext odpContext)
			: base(odpContext.ReportDefinition, dataSet, odpContext)
		{
		}

		protected override List<RuntimeDataSet> CreateRuntimeDataSets()
		{
			DataSetInstance dataSetInstance = new DataSetInstance(base.m_dataSet);
			this.m_runtimeDataSet = new RuntimeIdcIncrementalDataSet(base.DataSourceDefinition, base.m_dataSet, dataSetInstance, base.OdpContext);
			List<RuntimeDataSet> list = new List<RuntimeDataSet>(1);
			list.Add(this.m_runtimeDataSet);
			return list;
		}

		public bool SetupNextRow()
		{
			try
			{
				return null != this.m_runtimeDataSet.GetNextRow();
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
