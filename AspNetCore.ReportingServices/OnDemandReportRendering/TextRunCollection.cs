using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class TextRunCollection : ReportElementCollectionBase<TextRun>
	{
		private Paragraph m_paragraph;

		private TextRun[] m_textRuns;

		public override TextRun this[int i]
		{
			get
			{
				if (i >= 0 && i < this.Count)
				{
					TextRun textRun = this.m_textRuns[i];
					if (textRun == null)
					{
						if (this.m_paragraph.IsOldSnapshot)
						{
							textRun = new ShimTextRun(this.m_paragraph, this.m_paragraph.RenderingContext);
						}
						else
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun textRun2 = ((InternalParagraph)this.m_paragraph).ParagraphDef.TextRuns[i];
							textRun = new InternalTextRun(this.m_paragraph, i, textRun2, this.m_paragraph.RenderingContext);
						}
						this.m_textRuns[i] = textRun;
					}
					return textRun;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, i, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_textRuns.Length;
			}
		}

		internal TextRunCollection(Paragraph paragraph)
		{
			this.m_paragraph = paragraph;
			if (this.m_paragraph.IsOldSnapshot)
			{
				this.m_textRuns = new TextRun[1];
			}
			else
			{
				List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun> textRuns = ((InternalParagraph)this.m_paragraph).ParagraphDef.TextRuns;
				if (textRuns != null)
				{
					this.m_textRuns = new TextRun[textRuns.Count];
				}
				else
				{
					this.m_textRuns = new TextRun[0];
				}
			}
		}

		internal void SetNewContext()
		{
			for (int i = 0; i < this.m_textRuns.Length; i++)
			{
				if (this.m_textRuns[i] != null)
				{
					this.m_textRuns[i].SetNewContext();
				}
			}
		}
	}
}
