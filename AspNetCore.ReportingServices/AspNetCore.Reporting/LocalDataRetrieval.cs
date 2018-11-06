using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections;

namespace AspNetCore.Reporting
{
	internal abstract class LocalDataRetrieval
	{
		public abstract bool SupportsQueries
		{
			get;
		}

		public abstract ProcessingContext CreateProcessingContext(PreviewItemContext itemContext, ParameterInfoCollection parameters, IEnumerable dataSources, RuntimeDataSourceInfoCollection dataSourceInfoColl, RuntimeDataSetInfoCollection dataSetInfoColl, SharedDataSetCompiler sharedDataSetCompiler, DatasourceCredentialsCollection credentials, ReportProcessing.OnDemandSubReportCallback subReportCallback, IGetResource getResourceFunction, IChunkFactory chunkFactory, ReportRuntimeSetup runtimeSetup, CreateAndRegisterStream createStreamCallback);
	}
}
