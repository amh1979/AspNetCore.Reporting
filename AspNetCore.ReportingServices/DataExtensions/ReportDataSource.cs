using AspNetCore.ReportingServices.DataProcessing;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.DataExtensions
{
	internal sealed class ReportDataSource
	{
		private readonly string m_dataSourceType;

		private readonly Guid m_modelID;

		private readonly AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.CreateDataExtensionInstance m_createDataExtensionInstance;

		public ReportDataSource(string dataSourceType, Guid modelID, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.CreateDataExtensionInstance createDataExtensionInstance)
		{
			if (dataSourceType == null)
			{
				if (Global.Tracer.TraceError)
				{
					Global.Tracer.Trace(TraceLevel.Error, "The data source type is null. Cannot instantiate data processing component.");
				}
				throw new ReportProcessingException(ErrorCode.rsDataSourceTypeNull);
			}
			this.m_dataSourceType = dataSourceType;
			this.m_modelID = modelID;
			this.m_createDataExtensionInstance = createDataExtensionInstance;
		}

		public IDbConnection CreateConnection()
		{
			IDbConnection dbConnection = this.m_createDataExtensionInstance(this.m_dataSourceType, this.m_modelID);
			if (dbConnection == null)
			{
				if (Global.Tracer.TraceError)
				{
					Global.Tracer.Trace(TraceLevel.Error, "The connection object of the data source type {0} does not implement any of the required interfaces.", this.m_dataSourceType);
				}
				throw new DataExtensionNotFoundException(this.m_dataSourceType);
			}
			if (Global.Tracer.TraceVerbose)
			{
				Global.Tracer.Trace(TraceLevel.Verbose, "A connection object for the {0} data source has been created.", this.m_dataSourceType);
			}
			return dbConnection;
		}
	}
}
