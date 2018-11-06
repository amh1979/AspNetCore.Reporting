using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class RuntimeDataSourceSharedDataSet : RuntimeAtomicDataSource
	{
		private RuntimeSharedDataSet m_runtimeDataSet;

		private readonly DataSetDefinition m_dataSetDefinition;

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
				return base.CheckNoRows(this.m_runtimeDataSet);
			}
		}

		internal RuntimeDataSourceSharedDataSet(DataSetDefinition dataSetDefinition, OnDemandProcessingContext odpContext)
			: base(null, new DataSource(-1, dataSetDefinition.SharedDataSourceReferenceId, dataSetDefinition.DataSetCore), odpContext, false)
		{
			this.m_dataSetDefinition = dataSetDefinition;
		}

		protected override List<RuntimeDataSet> CreateRuntimeDataSets()
		{
			List<RuntimeDataSet> list = new List<RuntimeDataSet>(1);
			DataSet dataSet = base.DataSourceDefinition.DataSets[0];
			DataSetInstance dataSetInstance = new DataSetInstance(dataSet);
			this.m_runtimeDataSet = new RuntimeSharedDataSet(base.DataSourceDefinition, dataSet, dataSetInstance, base.OdpContext);
			list.Add(this.m_runtimeDataSet);
			return list;
		}

		protected override void OpenInitialConnectionAndTransaction()
		{
		}
	}
}
