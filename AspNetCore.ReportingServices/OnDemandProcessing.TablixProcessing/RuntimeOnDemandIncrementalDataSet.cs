using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class RuntimeOnDemandIncrementalDataSet : RuntimeIncrementalDataSetWithProcessingController
	{
		public RuntimeOnDemandIncrementalDataSet(DataSource dataSource, DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext odpContext)
			: base(dataSource, dataSet, dataSetInstance, odpContext)
		{
		}

		public void Advance()
		{
			try
			{
				bool flag = default(bool);
				while (this.ReadAndProcessOneRow(out flag) && !base.m_odpContext.StateManager.ShouldStopPipelineAdvance(!flag))
				{
				}
			}
			catch (Exception)
			{
				this.CleanupForException();
				this.FinalCleanup();
				throw;
			}
		}

		private bool ReadAndProcessOneRow(out bool isAggregateRow)
		{
			isAggregateRow = false;
			int rowNumber = default(int);
			RecordRow recordRow = base.ReadOneRow(out rowNumber);
			if (recordRow == null)
			{
				return false;
			}
			isAggregateRow = recordRow.IsAggregateRow;
			base.m_dataProcessingController.NextRow(recordRow, rowNumber, true, base.HasServerAggregateMetadata);
			return true;
		}
	}
}
