using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class TextBoxImpl : ReportItemImpl
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox m_textBox;

		private AspNetCore.ReportingServices.RdlExpressions.VariantResult m_result;

		private bool m_isValueReady;

		private bool m_isVisited;

		private List<string> m_fieldsUsedInValueExpression;

		private ParagraphsImpl m_paragraphs;

		public override object Value
		{
			get
			{
				this.GetResult(null, true);
				return this.m_result.Value;
			}
		}

		internal Paragraphs Paragraphs
		{
			get
			{
				return this.m_paragraphs;
			}
		}

		internal TextBoxImpl(AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox itemDef, AspNetCore.ReportingServices.RdlExpressions.ReportRuntime reportRT, IErrorContext iErrorContext)
			: base(itemDef, reportRT, iErrorContext)
		{
			this.m_textBox = itemDef;
			this.m_paragraphs = new ParagraphsImpl(this.m_textBox, base.m_reportRT, base.m_iErrorContext, base.m_scope);
		}

		private bool IsTextboxInScope()
		{
			OnDemandProcessingContext odpContext = base.m_reportRT.ReportObjectModel.OdpContext;
			IRIFReportScope iRIFReportScope = null;
			if (odpContext.IsTablixProcessingMode)
			{
				iRIFReportScope = odpContext.LastTablixProcessingReportScope;
				if (iRIFReportScope == null)
				{
					iRIFReportScope = odpContext.ReportDefinition;
				}
			}
			else if (odpContext.IsTopLevelSubReportProcessing)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport = odpContext.LastRIFObject as AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport;
				Global.Tracer.Assert(subReport != null, "Missing reference to subreport object");
				iRIFReportScope = subReport.GetContainingSection(odpContext);
			}
			else
			{
				IReportScope currentReportScope = odpContext.CurrentReportScope;
				iRIFReportScope = ((currentReportScope == null) ? odpContext.ReportDefinition : currentReportScope.RIFReportScope);
			}
			if (iRIFReportScope != null && iRIFReportScope.TextboxInScope(this.m_textBox.SequenceID))
			{
				return true;
			}
			return false;
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult GetResult(IReportScopeInstance romInstance, bool calledFromValue)
		{
			if (calledFromValue && !this.IsTextboxInScope())
			{
				this.m_result = default(AspNetCore.ReportingServices.RdlExpressions.VariantResult);
			}
			else if (!this.m_isValueReady)
			{
				if (this.m_isVisited)
				{
					base.m_iErrorContext.Register(ProcessingErrorCode.rsCyclicExpression, Severity.Warning, this.m_textBox.ObjectType, this.m_textBox.Name, "Value");
					throw new ReportProcessingException_InvalidOperationException();
				}
				this.m_isVisited = true;
				ObjectModelImpl reportObjectModel = base.m_reportRT.ReportObjectModel;
				OnDemandProcessingContext odpContext = base.m_reportRT.ReportObjectModel.OdpContext;
				bool contextUpdated = base.m_reportRT.ContextUpdated;
				IInstancePath originalObject = null;
				base.m_reportRT.ContextUpdated = false;
				if (odpContext.IsTablixProcessingMode || calledFromValue)
				{
					originalObject = odpContext.LastRIFObject;
				}
				bool flag = this.m_textBox.Action != null && this.m_textBox.Action.TrackFieldsUsedInValueExpression;
				Dictionary<string, bool> dictionary = null;
				if (flag)
				{
					dictionary = new Dictionary<string, bool>();
				}
				try
				{
					bool flag2 = false;
					if (this.m_paragraphs.Count == 1)
					{
						TextRunsImpl textRunsImpl = (TextRunsImpl)((Paragraphs)this.m_paragraphs)[0].TextRuns;
						if (textRunsImpl.Count == 1)
						{
							flag2 = true;
							TextRunImpl textRunImpl = (TextRunImpl)((TextRuns)textRunsImpl)[0];
							this.m_result = textRunImpl.GetResult(romInstance);
							if (flag)
							{
								textRunImpl.MergeFieldsUsedInValueExpression(dictionary);
							}
						}
					}
					if (!flag2)
					{
						bool flag3 = false;
						this.m_result = default(AspNetCore.ReportingServices.RdlExpressions.VariantResult);
						StringBuilder stringBuilder = new StringBuilder();
						for (int i = 0; i < this.m_paragraphs.Count; i++)
						{
							if (i > 0)
							{
								flag3 = true;
								stringBuilder.Append(Environment.NewLine);
							}
							TextRunsImpl textRunsImpl2 = (TextRunsImpl)((Paragraphs)this.m_paragraphs)[i].TextRuns;
							for (int j = 0; j < textRunsImpl2.Count; j++)
							{
								TextRunImpl textRunImpl2 = (TextRunImpl)((TextRuns)textRunsImpl2)[j];
								AspNetCore.ReportingServices.RdlExpressions.VariantResult result = textRunImpl2.GetResult(romInstance);
								if (result.Value != null)
								{
									if (result.TypeCode == TypeCode.Object && (result.Value is TimeSpan || result.Value is DateTimeOffset))
									{
										string text = textRunImpl2.TextRunDef.FormatTextRunValue(result, odpContext);
										if (text != null)
										{
											result.Value = text;
										}
										else
										{
											result.Value = AspNetCore.ReportingServices.RdlExpressions.ReportRuntime.ConvertToStringFallBack(result.Value);
										}
									}
									flag3 = true;
									stringBuilder.Append(result.Value);
								}
								if (flag)
								{
									textRunImpl2.MergeFieldsUsedInValueExpression(dictionary);
								}
							}
						}
						if (flag3)
						{
							this.m_result.Value = stringBuilder.ToString();
							this.m_result.TypeCode = TypeCode.String;
						}
					}
					if (flag)
					{
						this.m_fieldsUsedInValueExpression = new List<string>();
						foreach (string key in dictionary.Keys)
						{
							this.m_fieldsUsedInValueExpression.Add(key);
						}
					}
				}
				finally
				{
					odpContext.RestoreContext(originalObject);
					base.m_reportRT.ContextUpdated = contextUpdated;
					this.m_isVisited = false;
					this.m_isValueReady = true;
				}
			}
			return this.m_result;
		}

		internal List<string> GetFieldsUsedInValueExpression(IReportScopeInstance romInstance)
		{
			if (!this.m_isValueReady)
			{
				this.GetResult(romInstance, true);
			}
			return this.m_fieldsUsedInValueExpression;
		}

		internal override void Reset()
		{
			if (this.m_textBox.HasExpressionBasedValue)
			{
				this.m_isValueReady = false;
				this.m_paragraphs.Reset();
			}
		}

		internal override void Reset(AspNetCore.ReportingServices.RdlExpressions.VariantResult value)
		{
			this.SetResult(value);
		}

		internal void SetResult(AspNetCore.ReportingServices.RdlExpressions.VariantResult result)
		{
			this.m_result = result;
			this.m_isValueReady = true;
		}
	}
}
