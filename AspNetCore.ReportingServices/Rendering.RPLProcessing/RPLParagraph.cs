using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLParagraph : RPLElement
	{
		private int m_textRunCount;

		private RPLSizes m_contentSizes;

		private Queue<RPLTextRun> m_textRuns;

		private long m_textRunOffsets = -1L;

		public RPLSizes ContentSizes
		{
			get
			{
				return this.m_contentSizes;
			}
			set
			{
				this.m_contentSizes = value;
			}
		}

		public int TextRunCount
		{
			get
			{
				return this.m_textRunCount;
			}
			set
			{
				this.m_textRunCount = value;
			}
		}

		internal Queue<RPLTextRun> TextRuns
		{
			set
			{
				this.m_textRuns = value;
				if (this.m_textRuns != null)
				{
					this.m_textRunCount = this.m_textRuns.Count;
				}
			}
		}

		internal RPLParagraph()
		{
			base.m_rplElementProps = new RPLParagraphProps();
			base.m_rplElementProps.Definition = new RPLParagraphPropsDef();
		}

		internal RPLParagraph(long textRunOffsets, RPLContext context)
			: base(context)
		{
			this.m_textRunOffsets = textRunOffsets;
		}

		internal RPLParagraph(Queue<RPLTextRun> textRuns, RPLParagraphProps rplElementProps)
			: base(rplElementProps)
		{
			this.m_textRuns = textRuns;
		}

		internal void AddTextRun(RPLTextRun textRun)
		{
			if (this.m_textRuns == null)
			{
				this.m_textRuns = new Queue<RPLTextRun>();
			}
			this.m_textRuns.Enqueue(textRun);
			this.m_textRunCount++;
		}

		public RPLTextRun GetNextTextRun()
		{
			if (this.m_textRuns != null)
			{
				if (this.m_textRuns.Count == 0)
				{
					this.m_textRuns = null;
					return null;
				}
				return this.m_textRuns.Dequeue();
			}
			if (this.m_textRunOffsets >= 0 && this.m_textRunCount > 0)
			{
				base.m_context.BinaryReader.BaseStream.Seek(this.m_textRunOffsets, SeekOrigin.Begin);
				long num = base.m_context.BinaryReader.ReadInt64();
				if (num == -1)
				{
					this.m_textRunOffsets = -1L;
					return null;
				}
				this.m_textRunCount--;
				this.m_textRunOffsets += 8L;
				return RPLReader.ReadTextRun(num, base.m_context);
			}
			return null;
		}
	}
}
