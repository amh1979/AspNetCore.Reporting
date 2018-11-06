using AspNetCore.Reporting;
using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.DataProcessing;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;

namespace AspNetCore.Reporting
{
	internal class DataSetExtensionConnection : IProcessingDataExtensionConnection
	{
		private LocalDataRetrievalFromDataSet.GetSubReportDataSetCallback m_subreportCallback;

		private IEnumerable m_dataSources;

		public bool MustResolveSharedDataSources
		{
			get
			{
				return false;
			}
		}

		public DataSetExtensionConnection(LocalDataRetrievalFromDataSet.GetSubReportDataSetCallback subreportCallback, IEnumerable dataSources)
		{
			this.m_subreportCallback = subreportCallback;
			this.m_dataSources = dataSources;
		}

		public void DataSetRetrieveForReportInstance(ICatalogItemContext itemContext, ParameterInfoCollection reportParameters)
		{
			IEnumerable enumerable = this.m_subreportCallback((PreviewItemContext)itemContext, reportParameters);
			this.m_dataSources = new DataSourceCollectionWrapper((ReportDataSourceCollection)enumerable);
		}

		public void HandleImpersonation(IProcessingDataSource dataSource, DataSourceInfo dataSourceInfo, string datasetName, IDbConnection connection, System.Action afterImpersonationAction)
		{
			if (afterImpersonationAction != null)
			{
				afterImpersonationAction();
			}
		}

		public IDbConnection OpenDataSourceExtensionConnection(IProcessingDataSource dataSource, string connectionString, DataSourceInfo dataSourceInfo, string datasetName)
		{
			return new DataSetProcessingExtension(this.m_dataSources, datasetName);
		}

		public void CloseConnection(IDbConnection connection, IProcessingDataSource dataSourceObj, DataSourceInfo dataSourceInfo)
		{
			this.CloseConnectionWithoutPool(connection);
		}

		public void CloseConnectionWithoutPool(IDbConnection connection)
		{
			connection.Close();
		}
	}
}
