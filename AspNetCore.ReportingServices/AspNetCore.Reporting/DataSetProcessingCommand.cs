using AspNetCore.ReportingServices.DataProcessing;
using System;

namespace AspNetCore.Reporting
{
	internal class DataSetProcessingCommand : IDbCommand, IDisposable
	{
		private IDataReader m_dataReader;

		private IDbTransaction m_transaction;

		private DataSetProcessingParameters m_parameters;

		public string CommandText { get; set; }

        public int CommandTimeout { get; set; }

        public CommandType CommandType { get; set; }

        public IDataParameterCollection Parameters
		{
			get
			{
				return this.m_parameters;
			}
		}

		public IDbTransaction Transaction
		{
			get
			{
				return this.m_transaction;
			}
			set
			{
				this.m_transaction = value;
			}
		}

		internal DataSetProcessingCommand(IDataReader dataReader)
		{
			this.m_dataReader = dataReader;
			this.m_parameters = new DataSetProcessingParameters();
		}

		public IDataReader ExecuteReader(CommandBehavior behavior)
		{
			return this.m_dataReader;
		}

		public IDataParameter CreateParameter()
		{
			return new DataSetProcessingParameter();
		}

		public void Cancel()
		{
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}
