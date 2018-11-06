using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ParagraphCollection : ReportElementCollectionBase<Paragraph>
	{
		private TextBox m_textBox;

		private Paragraph[] m_paragraphs;

		public override Paragraph this[int i]
		{
			get
			{
				if (i >= 0 && i < this.Count)
				{
					Paragraph paragraph = this.m_paragraphs[i];
					if (paragraph == null)
					{
						if (this.m_textBox.IsOldSnapshot)
						{
							paragraph = new ShimParagraph(this.m_textBox, this.m_textBox.RenderingContext);
						}
						else
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph paragraph2 = this.m_textBox.TexBoxDef.Paragraphs[i];
							paragraph = new InternalParagraph(this.m_textBox, i, paragraph2, this.m_textBox.RenderingContext);
						}
						this.m_paragraphs[i] = paragraph;
					}
					return paragraph;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, i, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_paragraphs.Length;
			}
		}

		internal ParagraphCollection(TextBox textBox)
		{
			this.m_textBox = textBox;
			if (this.m_textBox.IsOldSnapshot)
			{
				this.m_paragraphs = new Paragraph[1];
			}
			else
			{
				List<AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph> paragraphs = this.m_textBox.TexBoxDef.Paragraphs;
				if (paragraphs != null)
				{
					this.m_paragraphs = new Paragraph[paragraphs.Count];
				}
				else
				{
					this.m_paragraphs = new Paragraph[0];
				}
			}
		}

		internal void SetNewContext()
		{
			for (int i = 0; i < this.m_paragraphs.Length; i++)
			{
				if (this.m_paragraphs[i] != null)
				{
					this.m_paragraphs[i].SetNewContext();
				}
			}
		}
	}
}
