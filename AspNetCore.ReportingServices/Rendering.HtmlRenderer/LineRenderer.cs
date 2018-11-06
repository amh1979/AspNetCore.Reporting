using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal class LineRenderer : IReportItemRenderer
	{
		private HTML5Renderer html5Renderer;

		private bool hasSlantedLines;

		public LineRenderer(HTML5Renderer renderer)
		{
			this.html5Renderer = renderer;
		}

		public void RenderReportItem(RPLElement reportItem, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, bool renderId, bool treatAsTopLevel)
		{
			RPLElementProps elementProps = reportItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			this.RenderLine((RPLLine)reportItem, elementProps, (RPLLinePropsDef)definition, measurement, renderId, styleContext);
		}

		protected void RenderLine(RPLLine reportItem, RPLElementProps rplProps, RPLLinePropsDef rplPropsDef, RPLItemMeasurement measurement, bool renderId, StyleContext styleContext)
		{
			if (this.html5Renderer.IsLineSlanted(measurement))
			{
				if (renderId)
				{
					this.html5Renderer.RenderNavigationId(rplProps.UniqueName);
				}
				if (this.html5Renderer.m_deviceInfo.BrowserMode == BrowserMode.Quirks)
				{
					this.RenderVMLLine(reportItem, measurement, styleContext);
				}
			}
			else
			{
				bool flag = measurement.Height == 0.0;
				this.html5Renderer.WriteStream(HTMLElements.m_openSpan);
				if (renderId)
				{
					this.html5Renderer.RenderReportItemId(rplProps.UniqueName);
				}
				int num = 0;
				object obj = rplProps.Style[10];
				if (obj != null)
				{
					this.html5Renderer.OpenStyle();
					if (flag)
					{
						this.html5Renderer.WriteStream(HTMLElements.m_styleHeight);
					}
					else
					{
						this.html5Renderer.WriteStream(HTMLElements.m_styleWidth);
					}
					this.html5Renderer.WriteStream(obj);
					this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
				}
				obj = rplProps.Style[0];
				if (obj != null)
				{
					this.html5Renderer.OpenStyle();
					this.html5Renderer.WriteStream(HTMLElements.m_backgroundColor);
					this.html5Renderer.WriteStream(obj);
				}
				this.html5Renderer.RenderReportItemStyle(reportItem, measurement, ref num);
				this.html5Renderer.CloseStyle(true);
				this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				this.html5Renderer.WriteStream(HTMLElements.m_closeSpan);
			}
		}

		private void RenderVMLLine(RPLLine line, RPLItemMeasurement measurement, StyleContext styleContext)
		{
			if (!this.hasSlantedLines)
			{
				this.html5Renderer.WriteStream("<?XML:NAMESPACE PREFIX=v /><?IMPORT NAMESPACE=\"v\" IMPLEMENTATION=\"#default#VML\" />");
				this.hasSlantedLines = true;
			}
			this.html5Renderer.WriteStream(HTMLElements.m_openVGroup);
			this.html5Renderer.WriteStream(HTMLElements.m_openStyle);
			this.html5Renderer.WriteStream(HTMLElements.m_styleWidth);
			if (styleContext.InTablix)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_percent);
				this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
				this.html5Renderer.WriteStream(HTMLElements.m_styleHeight);
				this.html5Renderer.WriteStream(HTMLElements.m_percent);
			}
			else
			{
				this.html5Renderer.WriteRSStream(measurement.Width);
				this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
				this.html5Renderer.WriteStream(HTMLElements.m_styleHeight);
				this.html5Renderer.WriteRSStream(measurement.Height);
			}
			this.html5Renderer.WriteStream(HTMLElements.m_closeQuote);
			this.html5Renderer.WriteStream(HTMLElements.m_openVLine);
			if (((RPLLinePropsDef)line.ElementProps.Definition).Slant)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_rightSlant);
			}
			else
			{
				this.html5Renderer.WriteStream(HTMLElements.m_leftSlant);
			}
			IRPLStyle style = line.ElementProps.Style;
			string text = (string)style[0];
			string text2 = (string)style[10];
			if (text != null && text2 != null)
			{
				int value = new RPLReportColor(text).ToColor().ToArgb() & 0xFFFFFF;
				this.html5Renderer.WriteStream(HTMLElements.m_strokeColor);
				this.html5Renderer.WriteStream("#");
				this.html5Renderer.WriteStream(Convert.ToString(value, 16));
				this.html5Renderer.WriteStream(HTMLElements.m_quote);
				this.html5Renderer.WriteStream(HTMLElements.m_strokeWeight);
				this.html5Renderer.WriteStream(text2);
				this.html5Renderer.WriteStream(HTMLElements.m_closeQuote);
			}
			string theString = "solid";
			string text3 = null;
			object obj = style[5];
			if (obj != null)
			{
				string value2 = EnumStrings.GetValue((RPLFormat.BorderStyles)obj);
				if (string.CompareOrdinal(value2, "dashed") == 0)
				{
					theString = "dash";
				}
				else if (string.CompareOrdinal(value2, "dotted") == 0)
				{
					theString = "dot";
				}
				if (string.CompareOrdinal(value2, "double") == 0)
				{
					text3 = "thinthin";
				}
			}
			this.html5Renderer.WriteStream(HTMLElements.m_dashStyle);
			this.html5Renderer.WriteStream(theString);
			if (text3 != null)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_quote);
				this.html5Renderer.WriteStream(HTMLElements.m_slineStyle);
				this.html5Renderer.WriteStream(text3);
			}
			this.html5Renderer.WriteStream(HTMLElements.m_quote);
			this.html5Renderer.WriteStream(HTMLElements.m_closeTag);
			this.html5Renderer.WriteStreamCR(HTMLElements.m_closeVGroup);
		}
	}
}
