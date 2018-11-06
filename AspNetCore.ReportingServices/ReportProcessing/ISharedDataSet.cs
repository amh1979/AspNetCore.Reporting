using AspNetCore.ReportingServices.DataExtensions;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal interface ISharedDataSet
	{
		bool HasUserDependencies
		{
			get;
		}

		IChunkFactory TargetSnapshot
		{
			set;
		}

		void Process(DataSetInfo sharedDataSet, string targetChunkNameInReportSnapshot, bool originalRequestNeedsDataChunk, IRowConsumer originalRequest, ParameterInfoCollection dataSetParameterValues, ReportProcessingContext originalProcessingContext);
	}
}
