using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Internal;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class DataSourceMetrics
	{
		private readonly string m_dataSourceName;

		private readonly string m_dataSourceReference;

		private readonly string m_dataSourceType;

		private readonly string m_embeddedConnectionString;

		private readonly long m_openConnectionDurationMs;

		private readonly bool? m_connectionFromPool;

		private readonly DataProcessingMetrics[] m_dataSetsMetrics;

		public long OpenConnectionDurationMs
		{
			get
			{
				return this.m_openConnectionDurationMs;
			}
		}

		public DataSourceMetrics(string dataSourceName, string dataSourceReference, string dataSourceType, DataProcessingMetrics aggregatedMetrics, DataProcessingMetrics[] dataSetsMetrics)
			: this(dataSourceName, dataSourceReference, dataSourceType, aggregatedMetrics.ResolvedConnectionString, aggregatedMetrics.OpenConnectionDurationMs, aggregatedMetrics.ConnectionFromPool)
		{
			this.m_dataSetsMetrics = dataSetsMetrics;
		}

		public DataSourceMetrics(string dataSourceName, string dataSourceReference, string dataSourceType, DataProcessingMetrics parallelDataSetMetrics)
			: this(dataSourceName, dataSourceReference, dataSourceType, parallelDataSetMetrics.ResolvedConnectionString, parallelDataSetMetrics.OpenConnectionDurationMs, parallelDataSetMetrics.ConnectionFromPool)
		{
			this.m_dataSetsMetrics = new DataProcessingMetrics[1];
			this.m_dataSetsMetrics[0] = parallelDataSetMetrics;
		}

		private DataSourceMetrics(string dataSourceName, string dataSourceReference, string dataSourceType, string embeddedConnectionString, long openConnectionDurationMs, bool? connectionFromPool)
		{
			this.m_dataSourceName = dataSourceName;
			this.m_dataSourceReference = dataSourceReference;
			this.m_dataSourceType = dataSourceType;
			this.m_embeddedConnectionString = ((dataSourceReference == null) ? embeddedConnectionString : null);
			this.m_openConnectionDurationMs = openConnectionDurationMs;
			this.m_connectionFromPool = connectionFromPool;
		}

		internal Connection ToAdditionalInfoConnection(IJobContext jobContext)
		{
			if (jobContext == null)
			{
				return null;
			}
			Connection connection = new Connection();
			connection.ConnectionOpenTime = this.m_openConnectionDurationMs;
			connection.ConnectionFromPool = this.m_connectionFromPool;
			if (jobContext.ExecutionLogLevel == ExecutionLogLevel.Verbose)
			{
				DataSource dataSource = new DataSource();
				dataSource.Name = this.m_dataSourceName;
				if (this.m_dataSourceReference != null)
				{
					dataSource.DataSourceReference = this.m_dataSourceReference;
				}
				else if (this.m_embeddedConnectionString != null)
				{
					dataSource.ConnectionString = this.m_embeddedConnectionString;
				}
				dataSource.DataExtension = this.m_dataSourceType;
				connection.DataSource = dataSource;
			}
			if (this.m_dataSetsMetrics != null)
			{
				connection.DataSets = new List<DataSet>(this.m_dataSetsMetrics.Length);
				for (int i = 0; i < this.m_dataSetsMetrics.Length; i++)
				{
					connection.DataSets.Add(this.m_dataSetsMetrics[i].ToAdditionalInfoDataSet(jobContext));
				}
			}
			return connection;
		}
	}
}
