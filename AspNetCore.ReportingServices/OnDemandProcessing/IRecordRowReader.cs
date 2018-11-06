using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal interface IRecordRowReader : IDisposable
	{
		RecordRow RecordRow
		{
			get;
		}

		bool GetNextRow();

		bool MoveToFirstRow();

		void Close();
	}
}
