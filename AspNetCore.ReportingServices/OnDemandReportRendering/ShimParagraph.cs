using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimParagraph : Paragraph
	{
		public override string ID
		{
			get
			{
				return base.TextBox.ID + "xK";
			}
		}

		public override Style Style
		{
			get
			{
				if (base.m_style == null)
				{
					base.m_style = new ParagraphFilteredStyle(this.RenderReportItem, base.RenderingContext, this.UseRenderStyle);
				}
				return base.m_style;
			}
		}

		public override ReportEnumProperty<ListStyle> ListStyle
		{
			get
			{
				if (base.m_listStyle == null)
				{
					base.m_listStyle = new ReportEnumProperty<ListStyle>(AspNetCore.ReportingServices.OnDemandReportRendering.ListStyle.None);
				}
				return base.m_listStyle;
			}
		}

		public override ReportIntProperty ListLevel
		{
			get
			{
				if (base.m_listLevel == null)
				{
					base.m_listLevel = new ReportIntProperty(0);
				}
				return base.m_listLevel;
			}
		}

		public override ParagraphInstance Instance
		{
			get
			{
				if (base.m_instance == null)
				{
					base.m_instance = new ShimParagraphInstance(this);
				}
				return base.m_instance;
			}
		}

		internal ShimParagraph(TextBox textBox, RenderingContext renderingContext)
			: base(textBox, renderingContext)
		{
		}

		internal override void UpdateRenderReportItem(AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			base.UpdateRenderReportItem(renderReportItem);
			if (base.m_style != null)
			{
				base.m_style.UpdateStyleCache(renderReportItem);
			}
		}
	}
}
