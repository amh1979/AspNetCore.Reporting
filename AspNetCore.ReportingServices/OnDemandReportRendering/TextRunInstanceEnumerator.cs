using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class TextRunInstanceEnumerator : IEnumerator<TextRunInstance>, IDisposable, IEnumerator
	{
		private ParagraphInstance m_paragraphInstance;

		private TextRunInstance m_textRunInstance;

		private int m_currentIndex;

		private int m_currentCompiledIndex;

		private CompiledTextRunInstanceCollection m_textRunInstances;

		public TextRunInstance Current
		{
			get
			{
				return this.m_textRunInstance;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.m_textRunInstance;
			}
		}

		internal TextRunInstanceEnumerator(ParagraphInstance paragraphInstance)
		{
			this.m_paragraphInstance = paragraphInstance;
		}

		public void Dispose()
		{
			this.Reset();
		}

		public bool MoveNext()
		{
			TextRunCollection textRuns = this.m_paragraphInstance.Definition.TextRuns;
			if (this.m_currentIndex < textRuns.Count)
			{
				TextRun textRun = ((ReportElementCollectionBase<TextRun>)textRuns)[this.m_currentIndex];
				if (textRun.Instance.MarkupType != 0)
				{
					if (this.m_textRunInstances == null)
					{
						if (textRuns.Count > 1)
						{
							this.m_textRunInstances = ((ReportElementCollectionBase<CompiledParagraphInstance>)textRun.CompiledInstance.CompiledParagraphInstances)[0].CompiledTextRunInstances;
						}
						else
						{
							this.m_textRunInstances = ((CompiledParagraphInstance)this.m_paragraphInstance).CompiledTextRunInstances;
						}
					}
					if (this.m_currentCompiledIndex < this.m_textRunInstances.Count)
					{
						this.m_textRunInstance = ((ReportElementCollectionBase<CompiledTextRunInstance>)this.m_textRunInstances)[this.m_currentCompiledIndex];
						this.m_currentCompiledIndex++;
						goto IL_00fb;
					}
					this.m_textRunInstances = null;
					this.m_currentCompiledIndex = 0;
					this.m_currentIndex++;
					return this.MoveNext();
				}
				this.m_textRunInstance = textRun.Instance;
				this.m_currentIndex++;
				goto IL_00fb;
			}
			return false;
			IL_00fb:
			return true;
		}

		public void Reset()
		{
			this.m_textRunInstance = null;
			this.m_currentIndex = 0;
			this.m_currentCompiledIndex = 0;
		}
	}
}
