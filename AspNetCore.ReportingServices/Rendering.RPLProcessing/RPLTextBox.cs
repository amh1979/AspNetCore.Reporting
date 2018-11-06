using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLTextBox : RPLItem
	{
		private int m_paragraphCount;

		private Queue<RPLParagraph> m_paragraphs;

		private long m_paragraphOffsets = -1L;

		internal long ParagraphOffsets
		{
			get
			{
				return this.m_paragraphOffsets;
			}
			set
			{
				this.m_paragraphOffsets = value;
			}
		}

		internal int ParagraphCount
		{
			set
			{
				this.m_paragraphCount = value;
			}
		}

		internal Queue<RPLParagraph> Paragraphs
		{
			set
			{
				this.m_paragraphs = value;
				if (this.m_paragraphs != null)
				{
					this.m_paragraphCount = this.m_paragraphs.Count;
				}
			}
		}

		internal RPLTextBox()
		{
			base.m_rplElementProps = new RPLTextBoxProps();
			base.m_rplElementProps.Definition = new RPLTextBoxPropsDef();
		}

		internal RPLTextBox(long startOffset, RPLContext context)
			: base(startOffset, context)
		{
		}

		internal RPLTextBox(RPLItemProps rplElementProps)
			: base(rplElementProps)
		{
			RPLTextBoxProps rPLTextBoxProps = rplElementProps as RPLTextBoxProps;
			if (rPLTextBoxProps != null)
			{
				rPLTextBoxProps.Value = null;
				this.m_paragraphs = null;
				this.m_paragraphCount = 0;
			}
		}

		public RPLParagraph GetNextParagraph()
		{
			if (this.m_paragraphs != null)
			{
				if (this.m_paragraphs.Count == 0)
				{
					this.m_paragraphs = null;
					return null;
				}
				return this.m_paragraphs.Dequeue();
			}
			if (this.m_paragraphOffsets >= 0 && this.m_paragraphCount > 0)
			{
				base.m_context.BinaryReader.BaseStream.Seek(this.m_paragraphOffsets, SeekOrigin.Begin);
				long num = base.m_context.BinaryReader.ReadInt64();
				if (num == -1)
				{
					this.m_paragraphOffsets = -1L;
					return null;
				}
				this.m_paragraphCount--;
				this.m_paragraphOffsets += 8L;
				return RPLReader.ReadParagraph(num, base.m_context);
			}
			return null;
		}

		internal void AddParagraph(RPLParagraph paragraph)
		{
			if (this.m_paragraphs == null)
			{
				this.m_paragraphs = new Queue<RPLParagraph>();
			}
			this.m_paragraphs.Enqueue(paragraph);
			this.m_paragraphCount++;
		}

		public void GetSimpleStyles(out RPLStyleProps nonShared, out RPLStyleProps shared, RPLParagraph paragraph, RPLTextRun textRun)
		{
			shared = new RPLStyleProps();
			nonShared = new RPLStyleProps();
			shared.AddAll(this.ElementPropsDef.SharedStyle);
			nonShared.AddAll(this.ElementProps.NonSharedStyle);
			nonShared.AddAll(paragraph.ElementProps.NonSharedStyle);
			if (paragraph.ElementProps.Definition != null)
			{
				shared.AddAll(paragraph.ElementProps.Definition.SharedStyle);
			}
			nonShared.AddAll(textRun.ElementProps.NonSharedStyle);
			if (textRun.ElementProps.Definition != null)
			{
				shared.AddAll(textRun.ElementProps.Definition.SharedStyle);
			}
		}

		public RPLElementStyle GetSimpleStyles(RPLParagraph paragraph, RPLTextRun textRun)
		{
			RPLStyleProps sharedProps = null;
			RPLStyleProps nonSharedProps = null;
			this.GetSimpleStyles(out nonSharedProps, out sharedProps, paragraph, textRun);
			return new RPLElementStyle(nonSharedProps, sharedProps);
		}
	}
}
