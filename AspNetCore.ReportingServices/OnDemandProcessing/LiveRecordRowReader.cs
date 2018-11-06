using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class LiveRecordRowReader : IRecordRowReader, IDisposable
	{
		private RuntimeLiveReaderDataSource m_dataSource;

		private RecordRow m_currentRow;

		public RecordRow RecordRow
		{
			get
			{
				return this.m_currentRow;
			}
		}

		public DataSetInstance DataSetInstance
		{
			get
			{
				return this.m_dataSource.DataSetInstance;
			}
		}

		public LiveRecordRowReader(DataSet dataSet, OnDemandProcessingContext odpContext)
		{
			this.m_dataSource = new RuntimeLiveReaderDataSource(odpContext.ReportDefinition, dataSet, odpContext);
			this.m_dataSource.Initialize();
		}

		public bool GetNextRow()
		{
			this.m_currentRow = this.m_dataSource.ReadNextRow();
			return this.m_currentRow != null;
		}

		public bool MoveToFirstRow()
		{
			return false;
		}

		public void Close()
		{
			if (this.m_dataSource != null)
			{
				this.m_dataSource.RecordTimeDataRetrieval();
				this.m_dataSource.Teardown();
				this.m_dataSource = null;
			}
		}

		public void Dispose()
		{
			this.Close();
		}
	}
}
