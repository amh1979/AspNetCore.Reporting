using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class RuntimeDataSourceParameters : RuntimeAtomicDataSource
	{
		private RuntimeParameterDataSet m_runtimeDataSet;

		private readonly int m_parameterDataSetIndex;

		private readonly ReportParameterDataSetCache m_paramDataCache;

		internal override bool NoRows
		{
			get
			{
				return base.CheckNoRows(this.m_runtimeDataSet);
			}
		}

		internal RuntimeDataSourceParameters(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource, OnDemandProcessingContext processingContext, int parameterDataSetIndex, ReportParameterDataSetCache aCache)
			: base(report, dataSource, processingContext, false)
		{
			Global.Tracer.Assert(parameterDataSetIndex != -1, "Parameter DataSet index must be specified when processing parameters");
			this.m_parameterDataSetIndex = parameterDataSetIndex;
			this.m_paramDataCache = aCache;
		}

		protected override List<RuntimeDataSet> CreateRuntimeDataSets()
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = base.DataSourceDefinition.DataSets[this.m_parameterDataSetIndex];
			DataSetInstance dataSetInstance = new DataSetInstance(dataSet);
			this.m_runtimeDataSet = new RuntimeParameterDataSet(base.DataSourceDefinition, dataSet, dataSetInstance, base.OdpContext, true, this.m_paramDataCache);
			List<RuntimeDataSet> list = new List<RuntimeDataSet>(1);
			list.Add(this.m_runtimeDataSet);
			return list;
		}
	}
}
