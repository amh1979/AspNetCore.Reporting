using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ParagraphInstanceEnumerator : IEnumerator<ParagraphInstance>, IDisposable, IEnumerator
	{
		private TextBox m_textbox;

		private ParagraphInstance m_currentParagraphInstance;

		private int m_currentCompiledIndex;

		private int m_currentIndex;

		private CompiledParagraphInstanceCollection m_paragraphs;

		public ParagraphInstance Current
		{
			get
			{
				return this.m_currentParagraphInstance;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.m_currentParagraphInstance;
			}
		}

		internal ParagraphInstanceEnumerator(TextBox textbox)
		{
			this.m_textbox = textbox;
		}

		public void Dispose()
		{
			this.Reset();
		}

		public bool MoveNext()
		{
			if (this.m_currentIndex < this.m_textbox.Paragraphs.Count)
			{
				Paragraph paragraph = ((ReportElementCollectionBase<Paragraph>)this.m_textbox.Paragraphs)[this.m_currentIndex];
				if (paragraph.TextRuns.Count == 1 && ((ReportElementCollectionBase<TextRun>)paragraph.TextRuns)[0].Instance.MarkupType != 0)
				{
					if (this.m_paragraphs == null)
					{
						this.m_paragraphs = ((ReportElementCollectionBase<TextRun>)paragraph.TextRuns)[0].CompiledInstance.CompiledParagraphInstances;
					}
					if (this.m_currentCompiledIndex < this.m_paragraphs.Count)
					{
						this.m_currentParagraphInstance = ((ReportElementCollectionBase<CompiledParagraphInstance>)this.m_paragraphs)[this.m_currentCompiledIndex];
						this.m_currentCompiledIndex++;
						goto IL_00f9;
					}
					this.m_paragraphs = null;
					this.m_currentCompiledIndex = 0;
					this.m_currentIndex++;
					return this.MoveNext();
				}
				this.m_currentParagraphInstance = paragraph.Instance;
				this.m_currentIndex++;
				goto IL_00f9;
			}
			return false;
			IL_00f9:
			return true;
		}

		public void Reset()
		{
			this.m_paragraphs = null;
			this.m_currentParagraphInstance = null;
			this.m_currentIndex = 0;
			this.m_currentCompiledIndex = 0;
		}
	}
}
