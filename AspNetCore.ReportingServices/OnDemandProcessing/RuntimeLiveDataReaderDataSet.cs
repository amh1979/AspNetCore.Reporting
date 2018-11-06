using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class RuntimeLiveDataReaderDataSet : RuntimeIncrementalDataSet
	{
		internal RuntimeLiveDataReaderDataSet(DataSource dataSource, DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext odpContext)
			: base(dataSource, dataSet, dataSetInstance, odpContext)
		{
		}

		internal RecordRow ReadNextRow()
		{
			try
			{
				int num = default(int);
				return base.ReadOneRow(out num);
			}
			catch (Exception)
			{
				this.CleanupForException();
				this.FinalCleanup();
				throw;
			}
		}

		protected override void InitializeBeforeProcessingRows(bool aReaderExtensionsSupported)
		{
		}

		protected override void ProcessExtendedPropertyMappings()
		{
		}
	}
}
