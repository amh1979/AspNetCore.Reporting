using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class RuntimeDataSourcePrefetch : RuntimeAtomicDataSource
	{
		private readonly ReportInstance m_reportInstance;

		private bool m_initialNoRowsState;

		protected override bool AllowConcurrentProcessing
		{
			get
			{
				return true;
			}
		}

		protected override bool CreatesDataChunks
		{
			get
			{
				return true;
			}
		}

		internal override bool NoRows
		{
			get
			{
				bool flag = this.m_initialNoRowsState;
				if (base.m_runtimeDataSets != null)
				{
					{
						foreach (RuntimeDataSet runtimeDataSet in base.m_runtimeDataSets)
						{
							if (!runtimeDataSet.UsedOnlyInParameters)
							{
								flag &= runtimeDataSet.NoRows;
							}
						}
						return flag;
					}
				}
				return flag;
			}
		}

		internal RuntimeDataSourcePrefetch(Report report, ReportInstance reportInstance, DataSource dataSource, OnDemandProcessingContext processingContext, bool mergeTransactions)
			: base(report, dataSource, processingContext, mergeTransactions)
		{
			this.m_reportInstance = reportInstance;
		}

		protected override List<RuntimeDataSet> CreateRuntimeDataSets()
		{
			int count = base.DataSourceDefinition.DataSets.Count;
			List<RuntimeDataSet> list = new List<RuntimeDataSet>(count);
			for (int i = 0; i < count; i++)
			{
				DataSet dataSet = base.DataSourceDefinition.DataSets[i];
				RuntimeDataSet runtimeDataSet = null;
				if (!dataSet.UsedOnlyInParameters)
				{
					this.m_initialNoRowsState = true;
				}
				if (!dataSet.UsedOnlyInParameters || base.DataSourceDefinition.Transaction)
				{
					if (base.OdpContext.InSubreport && base.OdpContext.FoundExistingSubReportInstance)
					{
						DataSetInstance dataSetInstance = base.OdpContext.GetDataSetInstance(dataSet);
						this.m_initialNoRowsState &= dataSetInstance.NoRows;
					}
					else
					{
						DataSetInstance dataSetInstance2 = new DataSetInstance(dataSet);
						this.m_reportInstance.SetDataSetInstance(dataSetInstance2);
						if (dataSet.IndexInCollection == base.ReportDefinition.FirstDataSetIndexToProcess && !dataSet.UsedOnlyInParameters)
						{
							runtimeDataSet = new RuntimeOnDemandDataSet(base.DataSourceDefinition, dataSet, dataSetInstance2, base.OdpContext, true, true, true);
						}
						else
						{
							bool processRetrievedData = !dataSet.UsedOnlyInParameters;
							runtimeDataSet = new RuntimePrefetchDataSet(base.DataSourceDefinition, dataSet, dataSetInstance2, base.OdpContext, true, processRetrievedData);
						}
					}
				}
				if (runtimeDataSet != null)
				{
					list.Add(runtimeDataSet);
				}
			}
			return list;
		}
	}
}
