using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class ParagraphsImpl : Paragraphs
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox m_textBoxDef;

		private ParagraphImpl[] m_paragraphs;

		private AspNetCore.ReportingServices.RdlExpressions.ReportRuntime m_reportRT;

		private IErrorContext m_iErrorContext;

		private IScope m_scope;

		public override Paragraph this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					ParagraphImpl paragraphImpl = this.m_paragraphs[index];
					if (paragraphImpl == null)
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph paragraphDef = this.m_textBoxDef.Paragraphs[index];
						paragraphImpl = new ParagraphImpl(paragraphDef, this.m_reportRT, this.m_iErrorContext, this.m_scope);
						this.m_paragraphs[index] = paragraphImpl;
					}
					return paragraphImpl;
				}
				throw new ArgumentOutOfRangeException("index");
			}
		}

		internal int Count
		{
			get
			{
				return this.m_paragraphs.Length;
			}
		}

		internal ParagraphsImpl(AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox textBoxDef, AspNetCore.ReportingServices.RdlExpressions.ReportRuntime reportRT, IErrorContext iErrorContext, IScope scope)
		{
			this.m_textBoxDef = textBoxDef;
			this.m_reportRT = reportRT;
			this.m_iErrorContext = iErrorContext;
			this.m_scope = scope;
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph> paragraphs = this.m_textBoxDef.Paragraphs;
			if (paragraphs != null)
			{
				this.m_paragraphs = new ParagraphImpl[paragraphs.Count];
			}
			else
			{
				this.m_paragraphs = new ParagraphImpl[0];
			}
		}

		internal void Reset()
		{
			for (int i = 0; i < this.m_paragraphs.Length; i++)
			{
				ParagraphImpl paragraphImpl = this.m_paragraphs[i];
				if (paragraphImpl != null)
				{
					paragraphImpl.Reset();
				}
			}
		}
	}
}
