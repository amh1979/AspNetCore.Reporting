using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal abstract class RuntimeAtomicDataSet : RuntimeDataSet, IRowConsumer
	{
		private int[] m_iRowConsumerMappingDataSetFieldIndexesToDataChunk;

		private bool m_iRowConsumerMappingIdentical;

		public string ReportDataSetName
		{
			get
			{
				return base.m_dataSet.Name;
			}
		}

		protected RuntimeAtomicDataSet(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext odpContext, bool processRetrievedData)
			: base(dataSource, dataSet, dataSetInstance, odpContext, processRetrievedData)
		{
		}

		internal void ProcessConcurrent(object threadSet)
		{
			Global.Tracer.Assert(base.m_dataSet.Name != null, "The name of a data set cannot be null.");
			try
			{
				if (Global.Tracer.TraceVerbose)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Thread has started processing data set '{0}'", base.m_dataSet.Name.MarkAsPrivate());
				}
				this.Process(null);
			}
			catch (ProcessingAbortedException)
			{
				if (Global.Tracer.TraceWarning)
				{
					Global.Tracer.Trace(TraceLevel.Warning, "Data set '{0}': Report processing has been aborted.", base.m_dataSet.Name.MarkAsPrivate());
				}
				if (!base.m_odpContext.StreamingMode)
				{
					goto end_IL_0063;
				}
				throw;
				end_IL_0063:;
			}
			catch (Exception ex2)
			{
				if (Global.Tracer.TraceError)
				{
					Global.Tracer.Trace(TraceLevel.Error, "An exception has occurred in data set '{0}'. Details: {1}", base.m_dataSet.Name.MarkAsPrivate(), ex2.ToString());
				}
				if (base.m_odpContext.AbortInfo != null)
				{
					base.m_odpContext.AbortInfo.SetError(ex2, base.m_odpContext.ProcessingAbortItemUniqueIdentifier);
					goto end_IL_00ac;
				}
				throw;
				end_IL_00ac:;
			}
			finally
			{
				if (Global.Tracer.TraceVerbose)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Processing of data set '{0}' completed.", base.m_dataSet.Name.MarkAsPrivate());
				}
				ThreadSet threadSet2 = threadSet as ThreadSet;
				if (threadSet2 != null)
				{
					threadSet2.ThreadCompleted();
				}
			}
		}

		public void ProcessInline(ExecutedQuery existingQuery)
		{
			this.Process(existingQuery);
		}

		private void Process(ExecutedQuery existingQuery)
		{
			this.InitializeDataSet();
			try
			{
				try
				{
					this.InitializeRowSourceAndProcessRows(existingQuery);
				}
				finally
				{
					this.CleanupProcess();
				}
				this.AllRowsRead();
				this.TeardownDataSet();
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				this.CleanupForException();
				throw;
			}
			finally
			{
				this.FinalCleanup();
			}
		}

		protected virtual void InitializeRowSourceAndProcessRows(ExecutedQuery existingQuery)
		{
			if (base.m_dataSet.IsReferenceToSharedDataSet)
			{
				base.ProcessSharedDataSetReference();
			}
			else
			{
				if (existingQuery != null)
				{
					base.InitializeAndRunFromExistingQuery(existingQuery);
				}
				else
				{
					base.InitializeAndRunLiveQuery();
				}
				if (base.ProcessRetrievedData)
				{
					this.ProcessRows();
				}
			}
		}

		protected virtual void AllRowsRead()
		{
		}

		protected void ProcessRows()
		{
			int rowNumber = default(int);
			for (AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow recordRow = base.ReadOneRow(out rowNumber); recordRow != null; recordRow = base.ReadOneRow(out rowNumber))
			{
				this.ProcessRow(recordRow, rowNumber);
			}
		}

		protected abstract void ProcessRow(AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow aRow, int rowNumber);

		public virtual void SetProcessingDataReader(IProcessingDataReader dataReader)
		{
			base.m_dataReader = dataReader;
			base.m_dataReader.OverrideWithDataReaderSettings(base.m_odpContext, base.m_dataSetInstance, base.m_dataSet.DataSetCore);
			if (base.ProcessRetrievedData)
			{
				base.m_dataReader.GetDataReaderMappingForRowConsumer(base.m_dataSetInstance, out this.m_iRowConsumerMappingIdentical, out this.m_iRowConsumerMappingDataSetFieldIndexesToDataChunk);
			}
			this.InitializeBeforeProcessingRows(base.HasServerAggregateMetadata);
		}

		public void NextRow(AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow originalRow)
		{
			if (base.ProcessRetrievedData)
			{
				base.m_odpContext.CheckAndThrowIfAborted();
				if (base.m_dataRowsRead == 0)
				{
					this.InitializeBeforeFirstRow(true);
				}
				AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow recordRow = null;
				recordRow = ((!this.m_iRowConsumerMappingIdentical) ? new AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow(originalRow, this.m_iRowConsumerMappingDataSetFieldIndexesToDataChunk) : originalRow);
				if (base.m_dataSet.IsReferenceToSharedDataSet && recordRow.IsAggregateRow && base.m_dataSet.InterpretSubtotalsAsDetails != AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet.TriState.False)
				{
					recordRow.IsAggregateRow = false;
				}
				this.ProcessRow(recordRow, base.m_dataRowsRead);
				base.IncrementRowCounterAndTrace();
			}
		}
	}
}
