using System;

namespace AspNetCore.ReportingServices.DataProcessing
{
	internal interface IDataReaderExtension : IDataReader, IDisposable
	{
		bool IsAggregateRow
		{
			get;
		}

		int AggregationFieldCount
		{
			get;
		}

		bool IsAggregationField(int index);
	}
}
