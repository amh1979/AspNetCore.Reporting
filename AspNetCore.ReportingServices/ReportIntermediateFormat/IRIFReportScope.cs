using AspNetCore.ReportingServices.OnDemandProcessing;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal interface IRIFReportScope : IInstancePath
	{
		bool NeedToCacheDataRows
		{
			get;
			set;
		}

		void AddInScopeEventSource(IInScopeEventSource eventSource);

		void AddInScopeTextBox(TextBox textbox);

		void ResetTextBoxImpls(OnDemandProcessingContext context);

		bool VariableInScope(int sequenceIndex);

		bool TextboxInScope(int sequenceIndex);
	}
}
