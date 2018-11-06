using AspNetCore.Reporting;
using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections;

namespace AspNetCore.Reporting
{
	internal class LocalDataRetrievalFromDataSet : LocalDataRetrieval
	{
		public delegate IEnumerable GetSubReportDataSetCallback(PreviewItemContext subReportContext, ParameterInfoCollection initialParameters);

		private GetSubReportDataSetCallback m_subreportDataCallback;

		public GetSubReportDataSetCallback SubReportDataSetCallback
		{
			set
			{
				this.m_subreportDataCallback = value;
			}
		}

		public override bool SupportsQueries
		{
			get
			{
				return false;
			}
		}

		public override ProcessingContext CreateProcessingContext(PreviewItemContext itemContext, ParameterInfoCollection parameters, IEnumerable dataSources, RuntimeDataSourceInfoCollection dataSourceInfoColl, RuntimeDataSetInfoCollection dataSetInfoColl, SharedDataSetCompiler sharedDataSetCompiler, DatasourceCredentialsCollection credentials, ReportProcessing.OnDemandSubReportCallback subReportCallback, IGetResource getResourceFunction, IChunkFactory chunkFactory, ReportRuntimeSetup runtimeSetup, AspNetCore.ReportingServices.Interfaces.CreateAndRegisterStream createStreamCallback)
		{
			return new ProcessingContextForDataSets(itemContext, parameters, new DataSourceCollectionWrapper((ReportDataSourceCollection)dataSources), subReportCallback, this.m_subreportDataCallback, getResourceFunction, chunkFactory, runtimeSetup, (AspNetCore.ReportingServices.Interfaces.CreateAndRegisterStream)createStreamCallback);
		}
	}
}
