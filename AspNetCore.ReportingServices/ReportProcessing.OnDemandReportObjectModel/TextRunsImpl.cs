using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class TextRunsImpl : TextRuns
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox m_textBoxDef;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph m_paragraphDef;

		private TextRunImpl[] m_textRuns;

		private AspNetCore.ReportingServices.RdlExpressions.ReportRuntime m_reportRT;

		private IErrorContext m_iErrorContext;

		private IScope m_scope;

		public override TextRun this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					TextRunImpl textRunImpl = this.m_textRuns[index];
					if (textRunImpl == null)
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun textRunDef = this.m_paragraphDef.TextRuns[index];
						textRunImpl = new TextRunImpl(this.m_textBoxDef, textRunDef, this.m_reportRT, this.m_iErrorContext, this.m_scope);
						this.m_textRuns[index] = textRunImpl;
					}
					return textRunImpl;
				}
				throw new ArgumentOutOfRangeException("index");
			}
		}

		internal int Count
		{
			get
			{
				return this.m_textRuns.Length;
			}
		}

		internal TextRunsImpl(AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph paragraphDef, AspNetCore.ReportingServices.RdlExpressions.ReportRuntime reportRT, IErrorContext iErrorContext, IScope scope)
		{
			this.m_textBoxDef = paragraphDef.TextBox;
			this.m_paragraphDef = paragraphDef;
			this.m_reportRT = reportRT;
			this.m_iErrorContext = iErrorContext;
			this.m_scope = scope;
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun> textRuns = this.m_paragraphDef.TextRuns;
			if (textRuns != null)
			{
				this.m_textRuns = new TextRunImpl[textRuns.Count];
			}
			else
			{
				this.m_textRuns = new TextRunImpl[0];
			}
		}

		internal void Reset()
		{
			for (int i = 0; i < this.m_textRuns.Length; i++)
			{
				TextRunImpl textRunImpl = this.m_textRuns[i];
				if (textRunImpl != null)
				{
					textRunImpl.Reset();
				}
			}
		}
	}
}
