using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimTextRun : TextRun
	{
		public override string ID
		{
			get
			{
				return base.TextBox.ID + "xL";
			}
		}

		public override Style Style
		{
			get
			{
				if (base.m_style == null)
				{
					base.m_style = new TextRunFilteredStyle(this.RenderReportItem, base.RenderingContext, this.UseRenderStyle);
				}
				return base.m_style;
			}
		}

		public override ReportStringProperty Value
		{
			get
			{
				if (base.m_value == null)
				{
					AspNetCore.ReportingServices.ReportProcessing.TextBox textBox = (AspNetCore.ReportingServices.ReportProcessing.TextBox)this.RenderReportItem.ReportItemDef;
					base.m_value = new ReportStringProperty(textBox.Value, textBox.Formula);
				}
				return base.m_value;
			}
		}

		public override TypeCode SharedTypeCode
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportRendering.TextBox)base.TextBox.RenderReportItem).SharedTypeCode;
			}
		}

		public override ReportEnumProperty<MarkupType> MarkupType
		{
			get
			{
				if (base.m_markupType == null)
				{
					base.m_markupType = new ReportEnumProperty<MarkupType>(AspNetCore.ReportingServices.OnDemandReportRendering.MarkupType.None);
				}
				return base.m_markupType;
			}
		}

		public override bool FormattedValueExpressionBased
		{
			get
			{
				if (!base.m_formattedValueExpressionBased.HasValue)
				{
					AspNetCore.ReportingServices.ReportProcessing.TextBox textBox = (AspNetCore.ReportingServices.ReportProcessing.TextBox)this.RenderReportItem.ReportItemDef;
					if (textBox.Value != null)
					{
						base.m_formattedValueExpressionBased = textBox.Value.IsExpression;
					}
					else
					{
						base.m_formattedValueExpressionBased = false;
					}
				}
				return base.m_formattedValueExpressionBased.Value;
			}
		}

		public override TextRunInstance Instance
		{
			get
			{
				if (base.m_instance == null)
				{
					base.m_instance = new ShimTextRunInstance(this, (TextBoxInstance)base.TextBox.Instance);
				}
				return base.m_instance;
			}
		}

		internal ShimTextRun(Paragraph paragraph, RenderingContext renderingContext)
			: base(paragraph, renderingContext)
		{
		}

		internal override void UpdateRenderReportItem(AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			if (base.m_style != null)
			{
				base.m_style.UpdateStyleCache(renderReportItem);
			}
		}
	}
}
