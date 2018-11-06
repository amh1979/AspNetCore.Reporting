using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class TextBoxInstance : ReportItemInstance
	{
		private ParagraphInstanceCollection m_paragraphInstances;

		private bool m_formattedValueEvaluated;

		private string m_formattedValue;

		private bool m_originalValueEvaluated;

		private VariantResult m_originalValue;

		private bool m_toggleState;

		private bool m_toggleStateEvaluated;

		private bool? m_duplicate = null;

		private TypeCode? m_typeCode = null;

		private TextBox m_textBoxDef;

		private bool? m_isToggleParent = null;

		public ParagraphInstanceCollection ParagraphInstances
		{
			get
			{
				if (this.m_paragraphInstances == null)
				{
					this.m_paragraphInstances = new ParagraphInstanceCollection(this.m_textBoxDef);
				}
				return this.m_paragraphInstances;
			}
		}

		public string Value
		{
			get
			{
				if (!this.m_formattedValueEvaluated)
				{
					this.m_formattedValueEvaluated = true;
					if (base.m_reportElementDef.IsOldSnapshot)
					{
						this.m_formattedValue = ((AspNetCore.ReportingServices.ReportRendering.TextBox)base.m_reportElementDef.RenderReportItem).Value;
					}
					else if (this.m_textBoxDef.IsSimple)
					{
						this.m_formattedValue = ((ReportElementCollectionBase<TextRun>)((ReportElementCollectionBase<Paragraph>)this.m_textBoxDef.Paragraphs)[0].TextRuns)[0].Instance.Value;
					}
					else
					{
						StringBuilder stringBuilder = new StringBuilder();
						bool flag = false;
						bool flag2 = true;
						foreach (ParagraphInstance paragraphInstance in this.ParagraphInstances)
						{
							if (!flag2)
							{
								flag = true;
								stringBuilder.Append(Environment.NewLine);
							}
							else
							{
								flag2 = false;
							}
							foreach (TextRunInstance textRunInstance in paragraphInstance.TextRunInstances)
							{
								string value = textRunInstance.Value;
								if (value != null)
								{
									flag = true;
									stringBuilder.Append(value);
								}
							}
						}
						if (flag)
						{
							this.m_formattedValue = stringBuilder.ToString();
						}
					}
				}
				return this.m_formattedValue;
			}
		}

		public object OriginalValue
		{
			get
			{
				if (base.m_reportElementDef.IsOldSnapshot)
				{
					return ((AspNetCore.ReportingServices.ReportRendering.TextBox)base.m_reportElementDef.RenderReportItem).OriginalValue;
				}
				this.EvaluateOriginalValue();
				return this.m_originalValue.Value;
			}
		}

		public bool IsToggleParent
		{
			get
			{
				if (!this.m_isToggleParent.HasValue)
				{
					if (base.m_reportElementDef.IsOldSnapshot)
					{
						this.m_isToggleParent = ((AspNetCore.ReportingServices.ReportRendering.TextBox)base.m_reportElementDef.RenderReportItem).IsToggleParent;
					}
					else
					{
						this.m_isToggleParent = this.m_textBoxDef.TexBoxDef.EvaluateIsToggle(this.ReportScopeInstance, base.RenderingContext.OdpContext);
					}
				}
				return this.m_isToggleParent.Value;
			}
		}

		public bool ToggleState
		{
			get
			{
				if (!this.m_toggleStateEvaluated)
				{
					this.m_toggleStateEvaluated = true;
					if (base.m_reportElementDef.IsOldSnapshot)
					{
						this.m_toggleState = ((AspNetCore.ReportingServices.ReportRendering.TextBox)base.m_reportElementDef.RenderReportItem).ToggleState;
					}
					else if (this.IsToggleParent)
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox texBoxDef = this.m_textBoxDef.TexBoxDef;
						this.m_toggleState = texBoxDef.EvaluateInitialToggleState(this.ReportScopeInstance, base.RenderingContext.OdpContext);
						if (base.RenderingContext.IsSenderToggled(this.UniqueName))
						{
							this.m_toggleState = !this.m_toggleState;
						}
					}
					else
					{
						this.m_toggleState = false;
					}
				}
				return this.m_toggleState;
			}
		}

		public SortOptions SortState
		{
			get
			{
				if (base.m_reportElementDef.IsOldSnapshot)
				{
					return ((AspNetCore.ReportingServices.ReportRendering.TextBox)base.m_reportElementDef.RenderReportItem).SortState;
				}
				return base.RenderingContext.GetSortState(this.UniqueName);
			}
		}

		public bool Duplicate
		{
			get
			{
				if (base.m_reportElementDef.IsOldSnapshot)
				{
					return ((AspNetCore.ReportingServices.ReportRendering.TextBox)base.m_reportElementDef.RenderReportItem).Duplicate;
				}
				if (!this.m_duplicate.HasValue)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox texBoxDef = this.m_textBoxDef.TexBoxDef;
					if (texBoxDef.HideDuplicates == null)
					{
						return false;
					}
					this.EvaluateOriginalValue();
					this.m_duplicate = texBoxDef.CalculateDuplicates(this.m_originalValue, base.RenderingContext.OdpContext);
				}
				return this.m_duplicate.Value;
			}
		}

		public TypeCode TypeCode
		{
			get
			{
				if (!this.m_typeCode.HasValue)
				{
					if (base.m_reportElementDef.IsOldSnapshot)
					{
						object originalValue = ((AspNetCore.ReportingServices.ReportRendering.TextBox)base.m_reportElementDef.RenderReportItem).OriginalValue;
						if (originalValue != null)
						{
							Type type = originalValue.GetType();
							this.m_typeCode = Type.GetTypeCode(type);
						}
						else
						{
							this.m_typeCode = TypeCode.Empty;
						}
					}
					else
					{
						this.EvaluateOriginalValue();
					}
				}
				return this.m_typeCode.Value;
			}
		}

		public bool ProcessedWithError
		{
			get
			{
				if (!base.m_reportElementDef.IsOldSnapshot && !this.m_textBoxDef.TexBoxDef.IsSimple)
				{
					return false;
				}
				return ((ReportElementCollectionBase<TextRun>)((ReportElementCollectionBase<Paragraph>)this.m_textBoxDef.Paragraphs)[0].TextRuns)[0].Instance.ProcessedWithError;
			}
		}

		internal TextBoxInstance(TextBox reportItemDef)
			: base(reportItemDef)
		{
			this.m_textBoxDef = reportItemDef;
		}

		public void AddToCurrentPage()
		{
			base.m_reportElementDef.RenderingContext.AddToCurrentPage(this.m_textBoxDef.Name, this.OriginalValue);
		}

		public void RegisterToggleSender()
		{
			if (!base.m_reportElementDef.IsOldSnapshot && this.IsToggleParent)
			{
				base.m_reportElementDef.RenderingContext.AddValidToggleSender(this.UniqueName);
			}
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			if (base.m_reportElementDef.IsOldSnapshot)
			{
				this.m_typeCode = null;
			}
			else
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox texBoxDef = this.m_textBoxDef.TexBoxDef;
				if (texBoxDef.HasExpressionBasedValue)
				{
					texBoxDef.ResetTextBoxImpl(base.RenderingContext.OdpContext);
					this.m_originalValueEvaluated = false;
					if (texBoxDef.IsSimple)
					{
						this.m_typeCode = null;
					}
					else
					{
						this.m_typeCode = TypeCode.String;
					}
				}
			}
			this.m_formattedValueEvaluated = false;
			this.m_formattedValue = null;
			this.m_toggleStateEvaluated = false;
			this.m_duplicate = null;
			this.m_isToggleParent = null;
		}

		private void EvaluateOriginalValue()
		{
			if (!this.m_originalValueEvaluated)
			{
				this.m_originalValueEvaluated = true;
				AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox texBoxDef = this.m_textBoxDef.TexBoxDef;
				if (texBoxDef.HasValue)
				{
					OnDemandProcessingContext odpContext = base.RenderingContext.OdpContext;
					this.m_originalValue = default(VariantResult);
					if (texBoxDef.IsSimple)
					{
						InternalTextRunInstance internalTextRunInstance = (InternalTextRunInstance)((ReportElementCollectionBase<TextRun>)((ReportElementCollectionBase<Paragraph>)this.m_textBoxDef.Paragraphs)[0].TextRuns)[0].Instance;
						this.m_originalValue.Value = internalTextRunInstance.OriginalValue;
						this.m_originalValue.ErrorOccurred = internalTextRunInstance.ProcessedWithError;
						this.m_typeCode = internalTextRunInstance.TypeCode;
						this.m_originalValue.TypeCode = this.m_typeCode.Value;
					}
					else
					{
						StringBuilder stringBuilder = new StringBuilder();
						bool flag = false;
						bool flag2 = true;
						foreach (ParagraphInstance paragraphInstance in this.ParagraphInstances)
						{
							if (!flag2)
							{
								flag = true;
								stringBuilder.Append(Environment.NewLine);
							}
							else
							{
								flag2 = false;
							}
							foreach (TextRunInstance textRunInstance in paragraphInstance.TextRunInstances)
							{
								object originalValue = textRunInstance.OriginalValue;
								if (originalValue != null)
								{
									flag = true;
									stringBuilder.Append(originalValue);
								}
							}
						}
						if (flag)
						{
							this.m_originalValue.Value = stringBuilder.ToString();
							this.m_originalValue.TypeCode = TypeCode.String;
							this.m_typeCode = TypeCode.String;
						}
					}
				}
				else
				{
					this.m_typeCode = TypeCode.Empty;
				}
			}
		}

		internal List<string> GetFieldsUsedInValueExpression()
		{
			List<string> result = null;
			AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox texBoxDef = this.m_textBoxDef.TexBoxDef;
			if (texBoxDef.HasExpressionBasedValue)
			{
				result = texBoxDef.GetFieldsUsedInValueExpression(this.ReportScopeInstance, base.RenderingContext.OdpContext);
			}
			return result;
		}
	}
}
