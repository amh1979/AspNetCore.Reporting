using AspNetCore.ReportingServices.DataProcessing;
using AspNetCore.ReportingServices.Interfaces;
using System;
using System.Collections;
using System.Data;
using System.Globalization;

namespace AspNetCore.Reporting
{
	internal class DataSetProcessingExtension : AspNetCore.ReportingServices.DataProcessing.IDbConnection, IDisposable, IExtension
	{
		private string m_dataSetName;

		private AspNetCore.ReportingServices.DataProcessing.IDataReader m_dataReader;

		public string ConnectionString
		{
            get;set;
		}

		public int ConnectionTimeout
		{
			get
			{
				return 0;
			}
		}

		public string LocalizedName
		{
			get
			{
				return ProcessingStrings.DataSetExtensionName;
			}
		}

		public DataSetProcessingExtension(IEnumerable dataSources, string dataSetName)
		{
			if (dataSources != null && dataSetName != null)
			{
				this.m_dataSetName = dataSetName;
				foreach (DataSourceWrapper dataSource in dataSources)
				{
					if (dataSource.Name == dataSetName && dataSource.Value != null)
					{
						if (dataSource.Value is DataTable)
						{
							this.m_dataReader = new DataTableReader((DataTable)dataSource.Value);
						}
						else
						{
							this.CreateDataReaderFromObject(dataSource.Value);
						}
						break;
					}
				}
			}
		}

		internal void CreateDataReaderFromObject(object dataSourceValue)
		{
			IEnumerable enumerable = dataSourceValue as IEnumerable;
			if (enumerable != null)
			{
				this.m_dataReader = new DataEnumerableReader(enumerable);
			}
			else
			{
				bool flag = false;
				if (dataSourceValue is Type)
				{
					try
					{
						dataSourceValue = Activator.CreateInstance((Type)dataSourceValue);
					}
					catch
					{
						flag = true;
					}
				}
				if (!flag)
				{
					ArrayList arrayList = new ArrayList(1);
					arrayList.Add(dataSourceValue);
					this.m_dataReader = new DataEnumerableReader(arrayList);
				}
			}
		}

		public void Open()
		{
		}

		public void Close()
		{
		}

		public AspNetCore.ReportingServices.DataProcessing.IDbCommand CreateCommand()
		{
			return new DataSetProcessingCommand(this.m_dataReader);
		}

		public AspNetCore.ReportingServices.DataProcessing.IDbTransaction BeginTransaction()
		{
			return new DataSetProcessingTransaction();
		}

		public void Dispose()
		{
			if (this.m_dataReader != null)
			{
				this.m_dataReader.Dispose();
			}
			GC.SuppressFinalize(this);
		}

		public void SetConfiguration(string configuration)
		{
		}

		public AspNetCore.ReportingServices.DataProcessing.IDataReader GetReader()
		{
			if (this.m_dataReader == null)
			{
				throw new Exception(string.Format(CultureInfo.InvariantCulture, ProcessingStrings.MissingDataReader, this.m_dataSetName));
			}
			return this.m_dataReader;
		}
	}
}
