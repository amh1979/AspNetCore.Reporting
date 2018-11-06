using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class ParagraphImpl : Paragraph
	{
		private TextRunsImpl m_textRuns;

		public override TextRuns TextRuns
		{
			get
			{
				return this.m_textRuns;
			}
		}

		internal ParagraphImpl(AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph paragraphDef, AspNetCore.ReportingServices.RdlExpressions.ReportRuntime reportRT, IErrorContext iErrorContext, IScope scope)
		{
			this.m_textRuns = new TextRunsImpl(paragraphDef, reportRT, iErrorContext, scope);
		}

		internal void Reset()
		{
			this.m_textRuns.Reset();
		}
	}
}
