using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalTextRunInstance : TextRunInstance
	{
		private string m_toolTip;

		private MarkupType? m_markupType = null;

		private bool m_formattedValueEvaluated;

		private string m_formattedValue;

		private VariantResult m_originalValue;

		private bool m_originalValueEvaluated;

		private bool m_originalValueNeedsReset;

		public override string UniqueName
		{
			get
			{
				if (base.m_uniqueName == null)
				{
					base.m_uniqueName = InstancePathItem.GenerateUniqueNameString(this.TextRunDef.IDString, this.TextRunDef.InstancePath);
				}
				return base.m_uniqueName;
			}
		}

		public override string Value
		{
			get
			{
				if (!this.m_formattedValueEvaluated)
				{
					this.m_formattedValueEvaluated = true;
					this.EvaluateOriginalValue();
					if (this.m_originalValue.TypeCode == TypeCode.String)
					{
						this.m_formattedValue = (this.m_originalValue.Value as string);
					}
					else
					{
						this.m_formattedValue = this.TextRunDef.FormatTextRunValue(this.m_originalValue, base.ReportElementDef.RenderingContext.OdpContext);
					}
				}
				return this.m_formattedValue;
			}
		}

		public override object OriginalValue
		{
			get
			{
				this.EvaluateOriginalValue();
				if (this.IsDateTimeOffsetOrTimeSpan())
				{
					if (this.Value != null)
					{
						return this.Value;
					}
					return ReportRuntime.ConvertToStringFallBack(this.m_originalValue.Value);
				}
				return this.m_originalValue.Value;
			}
		}

		public override string ToolTip
		{
			get
			{
				if (this.m_toolTip == null)
				{
					ExpressionInfo toolTip = this.TextRunDef.ToolTip;
					if (toolTip != null)
					{
						if (toolTip.IsExpression)
						{
							this.m_toolTip = this.TextRunDef.EvaluateToolTip(this.ReportScopeInstance, base.m_reportElementDef.RenderingContext.OdpContext);
						}
						else
						{
							this.m_toolTip = toolTip.StringValue;
						}
					}
				}
				return this.m_toolTip;
			}
		}

		public override MarkupType MarkupType
		{
			get
			{
				if (!this.m_markupType.HasValue)
				{
					ExpressionInfo markupType = this.TextRunDef.MarkupType;
					if (markupType != null)
					{
						if (markupType.IsExpression)
						{
							this.m_markupType = RichTextHelpers.TranslateMarkupType(this.TextRunDef.EvaluateMarkupType(this.ReportScopeInstance, base.m_reportElementDef.RenderingContext.OdpContext));
						}
						else
						{
							this.m_markupType = RichTextHelpers.TranslateMarkupType(markupType.StringValue);
						}
					}
					else
					{
						this.m_markupType = MarkupType.None;
					}
				}
				return this.m_markupType.Value;
			}
		}

		public override TypeCode TypeCode
		{
			get
			{
				this.EvaluateOriginalValue();
				if (this.IsDateTimeOffsetOrTimeSpan())
				{
					return TypeCode.String;
				}
				return this.m_originalValue.TypeCode;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun TextRunDef
		{
			get
			{
				return ((InternalTextRun)base.m_reportElementDef).TextRunDef;
			}
		}

		public override bool IsCompiled
		{
			get
			{
				return false;
			}
		}

		public override bool ProcessedWithError
		{
			get
			{
				this.EvaluateOriginalValue();
				return this.m_originalValue.ErrorOccurred;
			}
		}

		internal InternalTextRunInstance(InternalTextRun textRunDef)
			: base(textRunDef)
		{
		}

		internal VariantResult GetOriginalValue()
		{
			this.EvaluateOriginalValue();
			return this.m_originalValue;
		}

		private void EvaluateOriginalValue()
		{
			if (!this.m_originalValueEvaluated)
			{
				this.m_originalValueEvaluated = true;
				AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun textRunDef = this.TextRunDef;
				ExpressionInfo value = textRunDef.Value;
				if (value != null)
				{
					if (value.IsExpression)
					{
						this.m_originalValue = textRunDef.EvaluateValue(this.ReportScopeInstance, base.ReportElementDef.RenderingContext.OdpContext);
						this.m_originalValueNeedsReset = true;
					}
					else
					{
						this.m_originalValue = default(VariantResult);
						this.m_originalValue.Value = value.Value;
						ReportRuntime.SetVariantType(ref this.m_originalValue);
					}
				}
			}
		}

		private bool IsDateTimeOffsetOrTimeSpan()
		{
			if (this.m_originalValue.TypeCode == TypeCode.Object && (this.m_originalValue.Value is DateTimeOffset || this.m_originalValue.Value is TimeSpan))
			{
				return true;
			}
			return false;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_formattedValueEvaluated = false;
			this.m_formattedValue = null;
			this.m_toolTip = null;
			this.m_markupType = null;
			if (this.m_originalValueNeedsReset)
			{
				this.m_originalValueNeedsReset = false;
				this.m_originalValueEvaluated = false;
			}
		}

		internal List<string> GetFieldsUsedInValueExpression()
		{
			List<string> result = null;
			ExpressionInfo value = this.TextRunDef.Value;
			if (value != null && value.IsExpression)
			{
				result = this.TextRunDef.GetFieldsUsedInValueExpression(this.ReportScopeInstance, base.m_reportElementDef.RenderingContext.OdpContext);
			}
			return result;
		}
	}
}
