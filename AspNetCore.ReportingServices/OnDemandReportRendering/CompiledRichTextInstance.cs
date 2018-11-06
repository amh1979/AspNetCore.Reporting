using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class CompiledRichTextInstance : BaseInstance, IRichTextInstanceCreator, IRichTextLogger
	{
		private bool m_multipleParagraphsAllowed;

		private TextRun m_textRunDef;

		private Paragraph m_paragraphDef;

		private CompiledParagraphInstanceCollection m_compiledParagraphCollection;

		private bool m_parseErrorOccured;

		private bool m_parsed;

		private string m_uniqueName;

		private int m_objectCount;

		private IErrorContext m_errorContext;

		public string UniqueName
		{
			get
			{
				if (this.m_uniqueName == null)
				{
					this.m_uniqueName = this.m_textRunDef.InstanceUniqueName + 'x' + this.GenerateID().ToString(CultureInfo.InvariantCulture);
				}
				return this.m_uniqueName;
			}
		}

		public CompiledParagraphInstanceCollection CompiledParagraphInstances
		{
			get
			{
				this.Parse();
				return this.m_compiledParagraphCollection;
			}
		}

		internal TextRun TextRunDefinition
		{
			get
			{
				return this.m_textRunDef;
			}
		}

		internal Paragraph ParagraphDefinition
		{
			get
			{
				return this.m_paragraphDef;
			}
		}

		public bool ParseErrorOccured
		{
			get
			{
				this.Parse();
				return this.m_parseErrorOccured;
			}
		}

		RSTrace IRichTextLogger.Tracer
		{
			get
			{
				return Global.Tracer;
			}
		}

		internal CompiledRichTextInstance(IReportScope reportScope, TextRun textRunDef, Paragraph paragraphDef, bool multipleParagraphsAllowed)
			: base(reportScope)
		{
			this.m_paragraphDef = paragraphDef;
			this.m_textRunDef = textRunDef;
			this.m_multipleParagraphsAllowed = multipleParagraphsAllowed;
			this.m_errorContext = this.m_textRunDef.RenderingContext.ErrorContext;
		}

		private void Parse()
		{
			if (!this.m_parsed)
			{
				try
				{
					this.m_parsed = true;
					this.m_paragraphDef.CriGenerationPhase = ReportElement.CriGenerationPhases.Definition;
					this.m_textRunDef.CriGenerationPhase = ReportElement.CriGenerationPhases.Definition;
					ReportEnumProperty<MarkupType> markupType = this.m_textRunDef.MarkupType;
					MarkupType markupType2 = (!markupType.IsExpression) ? markupType.Value : this.m_textRunDef.Instance.MarkupType;
					RichTextParser richTextParser = null;
					MarkupType markupType3 = markupType2;
					if (markupType3 == MarkupType.HTML)
					{
						richTextParser = new HtmlParser(this.m_multipleParagraphsAllowed, this, this);
						InternalTextRunInstance internalTextRunInstance = (InternalTextRunInstance)this.m_textRunDef.Instance;
						AspNetCore.ReportingServices.RdlExpressions.VariantResult originalValue = internalTextRunInstance.GetOriginalValue();
						if (!originalValue.ErrorOccurred && originalValue.TypeCode != 0)
						{
							try
							{
								string richText = (originalValue.TypeCode != TypeCode.String) ? internalTextRunInstance.TextRunDef.FormatTextRunValue(originalValue.Value, originalValue.TypeCode, this.m_textRunDef.RenderingContext.OdpContext) : (originalValue.Value as string);
								this.m_compiledParagraphCollection = (CompiledParagraphInstanceCollection)richTextParser.Parse(richText);
							}
							catch (Exception ex)
							{
								this.m_errorContext.Register(ProcessingErrorCode.rsInvalidRichTextParseFailed, Severity.Warning, "TextRun", internalTextRunInstance.TextRunDef.Name, ex.Message);
								this.m_parseErrorOccured = true;
								ICompiledTextRunInstance compiledTextRunInstance = this.CreateSingleTextRun();
								compiledTextRunInstance.Value = RPRes.rsRichTextParseErrorValue;
							}
						}
						else
						{
							ICompiledTextRunInstance compiledTextRunInstance2 = this.CreateSingleTextRun();
							if (originalValue.ErrorOccurred)
							{
								compiledTextRunInstance2.Value = RPRes.rsExpressionErrorValue;
							}
						}
					}
				}
				finally
				{
					this.m_textRunDef.CriGenerationPhase = ReportElement.CriGenerationPhases.None;
					this.m_paragraphDef.CriGenerationPhase = ReportElement.CriGenerationPhases.None;
				}
			}
		}

		private ICompiledTextRunInstance CreateSingleTextRun()
		{
			ICompiledParagraphInstance compiledParagraphInstance = new CompiledParagraphInstance(this);
			ICompiledTextRunInstance compiledTextRunInstance = new CompiledTextRunInstance(this);
			CompiledRichTextStyleInstance style = new CompiledRichTextStyleInstance(this.m_textRunDef, this.m_textRunDef.ReportScope, this.m_textRunDef.RenderingContext);
			this.m_compiledParagraphCollection = new CompiledParagraphInstanceCollection(this);
			compiledParagraphInstance.CompiledTextRunInstances = new CompiledTextRunInstanceCollection(this);
			compiledTextRunInstance.Style = style;
			compiledParagraphInstance.Style = style;
			((ICollection<ICompiledParagraphInstance>)this.m_compiledParagraphCollection).Add(compiledParagraphInstance);
			compiledParagraphInstance.CompiledTextRunInstances.Add(compiledTextRunInstance);
			return compiledTextRunInstance;
		}

		protected override void ResetInstanceCache()
		{
			this.m_compiledParagraphCollection = null;
			this.m_parseErrorOccured = false;
			this.m_parsed = false;
			this.m_uniqueName = null;
			this.m_objectCount = 0;
		}

		internal int GenerateID()
		{
			return this.m_objectCount++;
		}

		IList<ICompiledParagraphInstance> IRichTextInstanceCreator.CreateParagraphInstanceCollection()
		{
			return new CompiledParagraphInstanceCollection(this);
		}

		ICompiledParagraphInstance IRichTextInstanceCreator.CreateParagraphInstance()
		{
			return new CompiledParagraphInstance(this);
		}

		ICompiledTextRunInstance IRichTextInstanceCreator.CreateTextRunInstance()
		{
			return new CompiledTextRunInstance(this);
		}

		IList<ICompiledTextRunInstance> IRichTextInstanceCreator.CreateTextRunInstanceCollection()
		{
			return new CompiledTextRunInstanceCollection(this);
		}

		ICompiledStyleInstance IRichTextInstanceCreator.CreateStyleInstance(bool isParagraphStyle)
		{
			if (isParagraphStyle)
			{
				return new CompiledRichTextStyleInstance(this.m_paragraphDef, this.m_paragraphDef.ReportScope, this.m_paragraphDef.RenderingContext);
			}
			return new CompiledRichTextStyleInstance(this.m_textRunDef, this.m_textRunDef.ReportScope, this.m_textRunDef.RenderingContext);
		}

		IActionInstance IRichTextInstanceCreator.CreateActionInstance()
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ActionItem actionItem = new AspNetCore.ReportingServices.ReportIntermediateFormat.ActionItem();
			AspNetCore.ReportingServices.ReportIntermediateFormat.Action action = new AspNetCore.ReportingServices.ReportIntermediateFormat.Action();
			action.ActionItems.Add(actionItem);
			ActionInfo owner = new ActionInfo(this.m_textRunDef.RenderingContext, this.m_textRunDef.ReportScope, action, ((InternalTextRun)this.m_textRunDef).TextRunDef, this.m_textRunDef, ObjectType.TextRun, ((InternalTextRun)this.m_textRunDef).TextRunDef.Name, this.m_textRunDef);
			return new Action(owner, actionItem, 0).Instance;
		}

		void IRichTextLogger.RegisterOutOfRangeSizeWarning(string propertyName, string value, string minVal, string maxVal)
		{
			this.m_errorContext.Register(ProcessingErrorCode.rsParseErrorOutOfRangeSize, Severity.Warning, ObjectType.TextRun, ((InternalTextRun)this.m_textRunDef).TextRunDef.Name, propertyName, value, minVal, maxVal);
		}

		void IRichTextLogger.RegisterInvalidValueWarning(string propertyName, string value, int charPosition)
		{
			this.m_errorContext.Register(ProcessingErrorCode.rsParseErrorInvalidValue, Severity.Warning, ObjectType.TextRun, ((InternalTextRun)this.m_textRunDef).TextRunDef.Name, propertyName, value, charPosition.ToString(CultureInfo.InvariantCulture));
		}

		void IRichTextLogger.RegisterInvalidColorWarning(string propertyName, string value, int charPosition)
		{
			this.m_errorContext.Register(ProcessingErrorCode.rsParseErrorInvalidColor, Severity.Warning, ObjectType.TextRun, ((InternalTextRun)this.m_textRunDef).TextRunDef.Name, propertyName, value, charPosition.ToString(CultureInfo.InvariantCulture));
		}

		void IRichTextLogger.RegisterInvalidSizeWarning(string propertyName, string value, int charPosition)
		{
			this.m_errorContext.Register(ProcessingErrorCode.rsParseErrorInvalidSize, Severity.Warning, ObjectType.TextRun, ((InternalTextRun)this.m_textRunDef).TextRunDef.Name, propertyName, value, charPosition.ToString(CultureInfo.InvariantCulture));
		}
	}
}
