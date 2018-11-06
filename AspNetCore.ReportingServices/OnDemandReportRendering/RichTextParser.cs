using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class RichTextParser
	{
		internal const int ParagraphListLevelMin = 0;

		internal const int ParagraphListLevelMax = 9;

		private const string m_propertyListLevel = "ListLevel";

		protected CompiledStyleInfo m_currentStyle;

		protected CompiledParagraphInfo m_currentParagraph;

		protected static ReportSize DefaultParagraphSpacing = new ReportSize("10pt");

		protected bool m_allowMultipleParagraphs;

		protected ICompiledParagraphInstance m_currentParagraphInstance;

		protected ICompiledTextRunInstance m_currentTextRunInstance;

		protected IRichTextInstanceCreator m_IRichTextInstanceCreator;

		protected IList<ICompiledParagraphInstance> m_paragraphInstanceCollection;

		protected ICompiledParagraphInstance m_onlyParagraphInstance;

		protected IRichTextLogger m_richTextLogger;

		private bool m_loggedListLevelWarning;

		internal RichTextParser(bool allowMultipleParagraphs, IRichTextInstanceCreator iRichTextInstanceCreator, IRichTextLogger richTextLogger)
		{
			this.m_allowMultipleParagraphs = allowMultipleParagraphs;
			this.m_IRichTextInstanceCreator = iRichTextInstanceCreator;
			this.m_richTextLogger = richTextLogger;
		}

		internal virtual IList<ICompiledParagraphInstance> Parse(string richText)
		{
			this.m_currentStyle = new CompiledStyleInfo();
			this.m_currentParagraph = new CompiledParagraphInfo();
			this.m_paragraphInstanceCollection = this.m_IRichTextInstanceCreator.CreateParagraphInstanceCollection();
			if (!string.IsNullOrEmpty(richText))
			{
				this.InternalParse(richText);
			}
			this.m_currentParagraph = new CompiledParagraphInfo();
			if (this.m_paragraphInstanceCollection.Count == 0)
			{
				this.m_currentParagraphInstance = this.CreateParagraphInstance();
				this.m_currentTextRunInstance = this.CreateTextRunInstance();
				this.m_currentParagraphInstance.Style = this.m_IRichTextInstanceCreator.CreateStyleInstance(true);
				this.m_currentTextRunInstance.Style = this.m_IRichTextInstanceCreator.CreateStyleInstance(false);
			}
			else
			{
				for (int i = 0; i < this.m_paragraphInstanceCollection.Count; i++)
				{
					this.m_currentParagraphInstance = this.m_paragraphInstanceCollection[i];
					if (this.m_currentParagraphInstance.CompiledTextRunInstances == null || this.m_currentParagraphInstance.CompiledTextRunInstances.Count == 0)
					{
						this.m_currentTextRunInstance = this.CreateTextRunInstance();
						this.m_currentTextRunInstance.Style = this.m_IRichTextInstanceCreator.CreateStyleInstance(true);
					}
				}
			}
			this.CloseParagraph();
			return this.m_paragraphInstanceCollection;
		}

		protected abstract void InternalParse(string richText);

		protected virtual bool AppendText(string value)
		{
			return this.AppendText(value, false);
		}

		protected virtual bool AppendText(string value, bool onlyIfValueExists)
		{
			if (this.m_currentParagraphInstance != null)
			{
				IList<ICompiledTextRunInstance> compiledTextRunInstances = this.m_currentParagraphInstance.CompiledTextRunInstances;
				if (compiledTextRunInstances.Count > 0)
				{
					this.m_currentTextRunInstance = compiledTextRunInstances[compiledTextRunInstances.Count - 1];
					if (onlyIfValueExists && string.IsNullOrEmpty(this.m_currentTextRunInstance.Value))
					{
						this.m_currentTextRunInstance = null;
						return false;
					}
				}
			}
			this.SetTextRunValue(value);
			return true;
		}

		protected virtual void SetTextRunValue(string value)
		{
			if (this.m_currentTextRunInstance == null)
			{
				this.m_currentTextRunInstance = this.CreateTextRunInstance();
			}
			this.m_currentTextRunInstance.Value = this.m_currentTextRunInstance.Value + value;
			if (this.m_currentTextRunInstance.Style == null)
			{
				ICompiledStyleInstance compiledStyleInstance = this.m_IRichTextInstanceCreator.CreateStyleInstance(false);
				this.m_currentStyle.PopulateStyleInstance(compiledStyleInstance, false);
				this.m_currentTextRunInstance.Style = compiledStyleInstance;
			}
			if (this.m_currentParagraphInstance.Style == null)
			{
				this.m_currentParagraphInstance.Style = this.m_IRichTextInstanceCreator.CreateStyleInstance(true);
			}
			this.m_currentTextRunInstance = null;
		}

		protected virtual ICompiledParagraphInstance CreateParagraphInstance()
		{
			if (!this.m_allowMultipleParagraphs && this.m_onlyParagraphInstance != null)
			{
				this.m_currentParagraphInstance = this.m_onlyParagraphInstance;
				this.AppendText(Environment.NewLine, true);
				return this.m_onlyParagraphInstance;
			}
			ICompiledParagraphInstance compiledParagraphInstance = this.m_IRichTextInstanceCreator.CreateParagraphInstance();
			if (this.m_allowMultipleParagraphs)
			{
				this.m_currentParagraph.PopulateParagraph(compiledParagraphInstance);
				int listLevel = compiledParagraphInstance.ListLevel;
				if (listLevel > 9)
				{
					if (!this.m_loggedListLevelWarning)
					{
						this.m_richTextLogger.RegisterOutOfRangeSizeWarning("ListLevel", Convert.ToString(listLevel, CultureInfo.InvariantCulture), Convert.ToString(0, CultureInfo.InvariantCulture), Convert.ToString(9, CultureInfo.InvariantCulture));
						this.m_loggedListLevelWarning = true;
					}
					compiledParagraphInstance.ListLevel = 9;
				}
			}
			else
			{
				this.m_onlyParagraphInstance = compiledParagraphInstance;
			}
			ICompiledStyleInstance compiledStyleInstance = this.m_IRichTextInstanceCreator.CreateStyleInstance(true);
			this.m_currentStyle.PopulateStyleInstance(compiledStyleInstance, true);
			compiledParagraphInstance.Style = compiledStyleInstance;
			IList<ICompiledTextRunInstance> list2 = compiledParagraphInstance.CompiledTextRunInstances = this.m_IRichTextInstanceCreator.CreateTextRunInstanceCollection();
			this.m_paragraphInstanceCollection.Add(compiledParagraphInstance);
			return compiledParagraphInstance;
		}

		protected virtual ICompiledTextRunInstance CreateTextRunInstance()
		{
			if (this.m_currentParagraphInstance == null)
			{
				this.m_currentParagraphInstance = this.CreateParagraphInstance();
			}
			IList<ICompiledTextRunInstance> compiledTextRunInstances = this.m_currentParagraphInstance.CompiledTextRunInstances;
			ICompiledTextRunInstance compiledTextRunInstance = this.m_IRichTextInstanceCreator.CreateTextRunInstance();
			ICompiledStyleInstance styleInstance = compiledTextRunInstance.Style = this.m_IRichTextInstanceCreator.CreateStyleInstance(false);
			this.m_currentStyle.PopulateStyleInstance(styleInstance, false);
			compiledTextRunInstances.Add(compiledTextRunInstance);
			return compiledTextRunInstance;
		}

		protected virtual void CloseParagraph()
		{
			this.m_currentParagraphInstance = null;
			this.m_currentTextRunInstance = null;
		}
	}
}
