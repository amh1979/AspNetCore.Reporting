using AspNetCore.ReportingServices.Rendering.RPLProcessing;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal class SubReportRenderer : IReportItemRenderer
	{
		private HTML5Renderer html5Renderer;

		public SubReportRenderer(HTML5Renderer renderer)
		{
			this.html5Renderer = renderer;
		}

		public void RenderReportItem(RPLElement reportItem, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, bool renderId, bool treatAsTopLevel)
		{
			RPLSubReport rPLSubReport = (RPLSubReport)reportItem;
			RPLElementProps elementProps = reportItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			using (this.html5Renderer.m_reportContext.EnterSubreport(definition))
			{
				if (!styleContext.InTablix || renderId)
				{
					styleContext.RenderMeasurements = false;
					this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
					this.html5Renderer.RenderReportItemStyle((RPLElement)rPLSubReport, elementProps, definition, measurement, styleContext, ref borderContext, definition.ID);
					if (renderId)
					{
						this.html5Renderer.RenderReportItemId(elementProps.UniqueName);
					}
					this.html5Renderer.WriteStreamCR(HTMLElements.m_closeBracket);
				}
				RPLItemMeasurement[] children = rPLSubReport.Children;
				int num = 0;
				int num2 = borderContext;
				bool usePercentWidth = children.Length > 0;
				int num3 = children.Length;
				for (int i = 0; i < num3; i++)
				{
					if (i == 0 && num3 > 1 && (borderContext & 8) > 0)
					{
						num2 &= -9;
					}
					else if (i == 1 && (borderContext & 4) > 0)
					{
						num2 &= -5;
					}
					if (i > 0 && i == num3 - 1 && (borderContext & 8) > 0)
					{
						num2 |= 8;
					}
					num = num2;
					RPLItemMeasurement rPLItemMeasurement = children[i];
					RPLContainer rPLContainer = (RPLContainer)rPLItemMeasurement.Element;
					RPLElementProps elementProps2 = rPLContainer.ElementProps;
					RPLElementPropsDef definition2 = elementProps2.Definition;
					this.html5Renderer.m_isBody = true;
					this.html5Renderer.m_usePercentWidth = usePercentWidth;
					new RectangleRenderer(this.html5Renderer).RenderReportItem(rPLContainer, rPLItemMeasurement, new StyleContext(), ref num, false, treatAsTopLevel);
				}
				if (!styleContext.InTablix || renderId)
				{
					this.html5Renderer.WriteStreamCR(HTMLElements.m_closeDiv);
				}
			}
		}
	}
}
