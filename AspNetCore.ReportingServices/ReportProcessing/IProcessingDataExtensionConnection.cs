using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.DataProcessing;
using AspNetCore.ReportingServices.Diagnostics;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal interface IProcessingDataExtensionConnection
	{
		bool MustResolveSharedDataSources
		{
			get;
		}

		void DataSetRetrieveForReportInstance(ICatalogItemContext itemContext, ParameterInfoCollection reportParameters);

		IDbConnection OpenDataSourceExtensionConnection(IProcessingDataSource dataSource, string connectionString, DataSourceInfo dataSourceInfo, string datasetName);

		void HandleImpersonation(IProcessingDataSource dataSource, DataSourceInfo dataSourceInfo, string datasetName, IDbConnection connection, System.Action afterImpersonationAction);

		void CloseConnectionWithoutPool(IDbConnection connection);

		void CloseConnection(IDbConnection connection, IProcessingDataSource dataSource, DataSourceInfo dataSourceInfo);
	}
}
