using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class RuntimeSharedDataSet : RuntimeParameterDataSet
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkWriter m_dataChunkWriter;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow m_currentRow;

		private IRowConsumer m_consumerRequest;

		protected override bool WritesDataChunk
		{
			get
			{
				return base.m_odpContext.ExternalDataSetContext.MustCreateDataChunk;
			}
		}

		public RuntimeSharedDataSet(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext processingContext)
			: base(dataSource, dataSet, dataSetInstance, processingContext, dataSet.DataSetCore.Filters != null || dataSet.DataSetCore.HasCalculatedFields(), null)
		{
			this.m_consumerRequest = base.m_odpContext.ExternalDataSetContext.ConsumerRequest;
		}

		protected override void InitializeBeforeProcessingRows(bool aReaderExtensionsSupported)
		{
			base.InitializeBeforeProcessingRows(aReaderExtensionsSupported);
			if (this.WritesDataChunk)
			{
				this.m_dataChunkWriter = new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkWriter(base.m_dataReader.RecordSetInfo, base.m_dataSetInstance, base.m_odpContext);
			}
		}

		protected override void InitializeBeforeFirstRow(bool hasRows)
		{
			base.InitializeBeforeFirstRow(hasRows);
			if (this.WritesDataChunk)
			{
				if (hasRows)
				{
					base.m_dataReader.RecordSetInfo.PopulateExtendedFieldsProperties(base.m_dataSetInstance);
				}
				this.m_dataChunkWriter.CreateDataChunkAndWriteHeader(base.m_dataReader.RecordSetInfo);
			}
			if (this.m_consumerRequest != null)
			{
				this.m_consumerRequest.SetProcessingDataReader(base.m_dataReader);
			}
		}

		protected override void ProcessRow(AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow row, int rowNumber)
		{
			if (base.m_mustEvaluateThroughReportObjectModel)
			{
				base.ProcessRow(row, rowNumber);
			}
			else
			{
				this.m_currentRow = row;
				this.PostFilterNextRow();
			}
		}

		protected override void AllRowsRead()
		{
			base.m_dataSetInstance.RecordSetSize = base.NumRowsRead;
			base.AllRowsRead();
		}

		internal override void EraseDataChunk()
		{
			if (this.WritesDataChunk)
			{
				RuntimeDataSet.EraseDataChunk(base.m_odpContext, base.m_dataSetInstance, ref this.m_dataChunkWriter);
			}
		}

		protected override void FinalCleanup()
		{
			base.FinalCleanup();
			if (this.m_dataChunkWriter != null)
			{
				this.m_dataChunkWriter.Close();
				this.m_dataChunkWriter = null;
			}
		}

		public override void PostFilterNextRow()
		{
			if (base.m_mustEvaluateThroughReportObjectModel)
			{
				this.m_currentRow = new AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow(base.m_odpContext.ReportObjectModel.FieldsImpl, base.m_dataSet.DataSetCore.Fields.Count, base.m_dataSetInstance.FieldInfos);
			}
			if (this.WritesDataChunk)
			{
				this.m_dataChunkWriter.WriteRecordRow(this.m_currentRow);
			}
			if (this.m_consumerRequest != null)
			{
				this.m_consumerRequest.NextRow(this.m_currentRow);
			}
		}
	}
}
