using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class RuntimeAggregationIncrementalDataSet : RuntimeIncrementalDataSetWithProcessingController
	{
		protected override bool ShouldCancelCommandDuringCleanup
		{
			get
			{
				return false;
			}
		}

		public RuntimeAggregationIncrementalDataSet(DataSource dataSource, DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext odpContext)
			: base(dataSource, dataSet, dataSetInstance, odpContext)
		{
		}

		public void CalculateDataSetAggregates()
		{
			try
			{
				int rowNumber = default(int);
				RecordRow recordRow = base.ReadOneRow(out rowNumber);
				if (recordRow != null)
				{
					base.m_dataProcessingController.NextRow(recordRow, rowNumber, true, base.HasServerAggregateMetadata);
				}
				base.m_dataProcessingController.AllRowsRead();
			}
			catch (Exception)
			{
				this.CleanupForException();
				this.FinalCleanup();
				throw;
			}
		}
	}
}
