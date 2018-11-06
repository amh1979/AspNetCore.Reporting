using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalTextRun : TextRun, IROMActionOwner
	{
		private ReportStringProperty m_toolTip;

		private ActionInfo m_actionInfo;

		private CompiledRichTextInstance m_compiledRichTextInstance;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun m_textRunDef;

		internal override IStyleContainer StyleContainer
		{
			get
			{
				return this.m_textRunDef;
			}
		}

		public override string ID
		{
			get
			{
				return this.m_textRunDef.RenderingModelID;
			}
		}

		public override string Label
		{
			get
			{
				return this.m_textRunDef.Label;
			}
		}

		public override ReportStringProperty Value
		{
			get
			{
				if (base.m_value == null)
				{
					base.m_value = new ReportStringProperty(this.m_textRunDef.Value);
				}
				return base.m_value;
			}
		}

		string IROMActionOwner.UniqueName
		{
			get
			{
				return this.TextRunDef.UniqueName;
			}
		}

		public override ActionInfo ActionInfo
		{
			get
			{
				if (this.m_actionInfo == null && this.m_textRunDef.Action != null)
				{
					this.m_actionInfo = new ActionInfo(base.RenderingContext, this.ReportScope, this.m_textRunDef.Action, this.TextRunDef, this, this.m_textRunDef.ObjectType, this.m_textRunDef.Name, this);
				}
				return this.m_actionInfo;
			}
		}

		public override ReportStringProperty ToolTip
		{
			get
			{
				if (this.m_toolTip == null && this.m_textRunDef.ToolTip != null)
				{
					this.m_toolTip = new ReportStringProperty(this.m_textRunDef.ToolTip);
				}
				return this.m_toolTip;
			}
		}

		public override ReportEnumProperty<MarkupType> MarkupType
		{
			get
			{
				if (base.m_markupType == null)
				{
					ExpressionInfo markupType = this.m_textRunDef.MarkupType;
					if (markupType != null)
					{
						MarkupType value = AspNetCore.ReportingServices.OnDemandReportRendering.MarkupType.None;
						if (!markupType.IsExpression)
						{
							value = RichTextHelpers.TranslateMarkupType(markupType.StringValue);
						}
						base.m_markupType = new ReportEnumProperty<MarkupType>(markupType.IsExpression, markupType.OriginalText, value);
					}
					else
					{
						base.m_markupType = new ReportEnumProperty<MarkupType>(AspNetCore.ReportingServices.OnDemandReportRendering.MarkupType.None);
					}
				}
				return base.m_markupType;
			}
		}

		public override TypeCode SharedTypeCode
		{
			get
			{
				return this.m_textRunDef.ValueTypeCode;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun TextRunDef
		{
			get
			{
				return this.m_textRunDef;
			}
		}

		public override bool FormattedValueExpressionBased
		{
			get
			{
				if (!base.m_formattedValueExpressionBased.HasValue)
				{
					if (!this.MarkupType.IsExpression && this.MarkupType.Value == AspNetCore.ReportingServices.OnDemandReportRendering.MarkupType.None)
					{
						if (this.m_textRunDef.Value != null && this.m_textRunDef.ValueTypeCode != TypeCode.String)
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.Style styleClass = this.m_textRunDef.StyleClass;
							base.m_formattedValueExpressionBased = (this.m_textRunDef.Value.IsExpression || (styleClass != null && (this.StyleAttributeExpressionBased(styleClass, "Language") || this.StyleAttributeExpressionBased(styleClass, "Format") || this.StyleAttributeExpressionBased(styleClass, "Calendar") || this.StyleAttributeExpressionBased(styleClass, "NumeralLanguage") || this.StyleAttributeExpressionBased(styleClass, "NumeralVariant"))));
						}
						else
						{
							base.m_formattedValueExpressionBased = false;
						}
					}
					else
					{
						base.m_formattedValueExpressionBased = true;
					}
				}
				return base.m_formattedValueExpressionBased.Value;
			}
		}

		public override TextRunInstance Instance
		{
			get
			{
				if (base.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = new InternalTextRunInstance(this);
				}
				return base.m_instance;
			}
		}

		internal override List<string> FieldsUsedInValueExpression
		{
			get
			{
				if (base.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				return ((InternalTextRunInstance)this.Instance).GetFieldsUsedInValueExpression();
			}
		}

		public override CompiledRichTextInstance CompiledInstance
		{
			get
			{
				if (this.Instance.MarkupType == AspNetCore.ReportingServices.OnDemandReportRendering.MarkupType.None)
				{
					return null;
				}
				if (this.m_compiledRichTextInstance == null)
				{
					this.m_compiledRichTextInstance = new CompiledRichTextInstance(this.ReportScope, this, base.m_paragraph, base.m_paragraph.TextRuns.Count == 1);
				}
				return this.m_compiledRichTextInstance;
			}
		}

		internal InternalTextRun(Paragraph paragraph, int indexIntoParentCollectionDef, AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun textRun, RenderingContext renderingContext)
			: base(paragraph, indexIntoParentCollectionDef, renderingContext)
		{
			this.m_textRunDef = textRun;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
		}

		internal override void SetNewContextChildren()
		{
			base.SetNewContextChildren();
			if (this.m_compiledRichTextInstance != null)
			{
				this.m_compiledRichTextInstance.SetNewContext();
			}
			if (this.m_actionInfo != null)
			{
				this.m_actionInfo.SetNewContext();
			}
		}

		private bool StyleAttributeExpressionBased(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, string styleName)
		{
			AttributeInfo attributeInfo = default(AttributeInfo);
			if (style.GetAttributeInfo(styleName, out attributeInfo))
			{
				return attributeInfo.IsExpression;
			}
			return false;
		}
	}
}
