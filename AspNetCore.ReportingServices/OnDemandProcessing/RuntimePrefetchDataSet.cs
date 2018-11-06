using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal class RuntimePrefetchDataSet : RuntimeAtomicDataSet
	{
		private ChunkManager.DataChunkWriter m_dataChunkWriter;

		protected readonly bool m_canWriteDataChunk;

		private RecordSetInfo m_recordSetInfo;

		protected override bool WritesDataChunk
		{
			get
			{
				return true;
			}
		}

		public RuntimePrefetchDataSet(DataSource dataSource, DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext processingContext, bool canWriteDataChunk, bool processRetrievedData)
			: base(dataSource, dataSet, dataSetInstance, processingContext, processRetrievedData)
		{
			this.m_canWriteDataChunk = canWriteDataChunk;
		}

		protected override void ProcessRow(RecordRow aRow, int rowNumber)
		{
			if (!base.m_dataSet.IsReferenceToSharedDataSet && this.m_canWriteDataChunk)
			{
				this.m_dataChunkWriter.WriteRecordRow(aRow);
			}
		}

		protected override void ProcessExtendedPropertyMappings()
		{
			if (!base.m_dataSet.IsReferenceToSharedDataSet)
			{
				this.m_recordSetInfo.PopulateExtendedFieldsProperties(base.m_dataSetInstance);
			}
		}

		protected override void InitializeBeforeProcessingRows(bool aReaderExtensionsSupported)
		{
			if (!base.m_dataSet.IsReferenceToSharedDataSet)
			{
				if (base.m_dataReader != null)
				{
					this.m_recordSetInfo = base.m_dataReader.RecordSetInfo;
				}
				else
				{
					this.m_recordSetInfo = new RecordSetInfo(aReaderExtensionsSupported, base.m_odpContext.IsSharedDataSetExecutionOnly, base.m_dataSetInstance, base.m_odpContext.ExecutionTime);
				}
				if (this.m_canWriteDataChunk)
				{
					this.m_dataChunkWriter = new ChunkManager.DataChunkWriter(this.m_recordSetInfo, base.m_dataSetInstance, base.m_odpContext);
				}
			}
		}

		protected override void AllRowsRead()
		{
			base.m_dataSetInstance.RecordSetSize = base.NumRowsRead;
		}

		protected override void CleanupProcess()
		{
			base.CleanupProcess();
			if (this.m_dataChunkWriter != null)
			{
				this.m_dataChunkWriter.Close();
				this.m_dataChunkWriter = null;
			}
		}

		internal override void EraseDataChunk()
		{
			if (!base.m_dataSet.IsReferenceToSharedDataSet && this.m_canWriteDataChunk)
			{
				RuntimeDataSet.EraseDataChunk(base.m_odpContext, base.m_dataSetInstance, ref this.m_dataChunkWriter);
			}
		}
	}
}
