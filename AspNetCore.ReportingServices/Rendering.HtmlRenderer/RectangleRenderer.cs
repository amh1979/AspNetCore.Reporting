using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal class RectangleRenderer : IReportItemRenderer
	{
		private const string GrowRectanglesSuffix = "_gr";

		private HTML5Renderer html5Renderer;

		private bool needsGrowRectangleScript;

		public RectangleRenderer(HTML5Renderer renderer)
		{
			this.html5Renderer = renderer;
		}

		public void RenderReportItem(RPLElement reportItem, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, bool renderId, bool treatAsTopLevel)
		{
			RPLElementProps elementProps = reportItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			this.RenderRectangle((RPLContainer)reportItem, elementProps, definition, measurement, ref borderContext, renderId, styleContext, treatAsTopLevel);
		}

		protected void RenderRectangle(RPLContainer rectangle, RPLElementProps props, RPLElementPropsDef def, RPLItemMeasurement measurement, ref int borderContext, bool renderId, StyleContext styleContext, bool treatAsTopLevel)
		{
			RPLItemMeasurement[] children = rectangle.Children;
			RPLRectanglePropsDef rPLRectanglePropsDef = def as RPLRectanglePropsDef;
			if (rPLRectanglePropsDef != null && rPLRectanglePropsDef.LinkToChildId != null)
			{
				this.html5Renderer.m_linkToChildStack.Push(rPLRectanglePropsDef.LinkToChildId);
			}
			bool expandItem = this.html5Renderer.m_expandItem;
			bool flag = renderId;
			string text = props.UniqueName;
			bool flag2 = children == null || children.Length == 0;
			if (flag2 && styleContext.InTablix)
			{
				return;
			}
			bool flag3 = this.html5Renderer.m_deviceInfo.OutlookCompat || !this.html5Renderer.m_browserIE || (flag2 && this.html5Renderer.m_usePercentWidth);
			if (!styleContext.InTablix || renderId)
			{
				if (flag3)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_openTable);
					this.html5Renderer.WriteStream(HTMLElements.m_zeroBorder);
				}
				else
				{
					this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
					if (this.html5Renderer.m_deviceInfo.IsBrowserIE && this.html5Renderer.m_deviceInfo.AllowScript)
					{
						if (!this.needsGrowRectangleScript)
						{
							this.CreateGrowRectIdsStream();
						}
						flag = true;
						if (!renderId)
						{
							text = props.UniqueName + "_gr";
						}
						this.html5Renderer.WriteIdToSecondaryStream(this.html5Renderer.m_growRectangleIdsStream, text);
					}
				}
				if (flag)
				{
					this.html5Renderer.RenderReportItemId(text);
				}
				if (this.html5Renderer.m_isBody)
				{
					this.html5Renderer.m_isBody = false;
					styleContext.RenderMeasurements = false;
					if (flag2)
					{
						this.html5Renderer.OpenStyle();
						if (this.html5Renderer.m_usePercentWidth)
						{
							this.html5Renderer.RenderMeasurementHeight(measurement.Height);
							this.html5Renderer.WriteStream(HTMLElements.m_styleWidth);
							this.html5Renderer.WriteStream(HTMLElements.m_percent);
							this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
						}
						else
						{
							this.RenderRectangleMeasurements(measurement, props.Style);
						}
					}
					else if (flag3 && this.html5Renderer.m_usePercentWidth)
					{
						this.html5Renderer.OpenStyle();
						this.html5Renderer.WriteStream(HTMLElements.m_styleWidth);
						this.html5Renderer.WriteStream(HTMLElements.m_percent);
						this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
					}
					this.html5Renderer.m_usePercentWidth = false;
				}
				if (!styleContext.InTablix)
				{
					if (styleContext.RenderMeasurements)
					{
						this.html5Renderer.OpenStyle();
						this.RenderRectangleMeasurements(measurement, props.Style);
					}
					this.html5Renderer.RenderReportItemStyle((RPLElement)rectangle, props, def, measurement, styleContext, ref borderContext, def.ID);
				}
				this.html5Renderer.CloseStyle(true);
				this.html5Renderer.WriteToolTip(props);
				this.html5Renderer.WriteStreamCR(HTMLElements.m_closeBracket);
				if (flag3)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_firstTD);
					this.html5Renderer.OpenStyle();
					if (flag2)
					{
						this.html5Renderer.RenderMeasurementStyle(measurement.Height, measurement.Width);
						this.html5Renderer.WriteStream(HTMLElements.m_fontSize);
						this.html5Renderer.WriteStream("1pt");
					}
					else
					{
						this.html5Renderer.WriteStream(HTMLElements.m_verticalAlign);
						this.html5Renderer.WriteStream(HTMLElements.m_topValue);
					}
					this.html5Renderer.CloseStyle(true);
					this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				}
			}
			if (flag2)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_nbsp);
			}
			else
			{
				bool inTablix = styleContext.InTablix;
				styleContext.InTablix = false;
				flag2 = this.html5Renderer.GenerateHTMLTable(children, measurement.Top, measurement.Left, measurement.Width, measurement.Height, borderContext, expandItem, SharedListLayoutState.None, null, props.Style, treatAsTopLevel);
				if (inTablix)
				{
					styleContext.InTablix = true;
				}
			}
			if (!styleContext.InTablix || renderId)
			{
				if (flag3)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_lastTD);
					this.html5Renderer.WriteStream(HTMLElements.m_closeTable);
				}
				else
				{
					this.html5Renderer.WriteStreamCR(HTMLElements.m_closeDiv);
				}
			}
			if (this.html5Renderer.m_linkToChildStack.Count > 0 && rPLRectanglePropsDef != null && rPLRectanglePropsDef.LinkToChildId != null && rPLRectanglePropsDef.LinkToChildId.Equals(this.html5Renderer.m_linkToChildStack.Peek()))
			{
				this.html5Renderer.m_linkToChildStack.Pop();
			}
		}

		private void CreateGrowRectIdsStream()
		{
			string streamName = HTML5Renderer.GetStreamName(this.html5Renderer.m_rplReport.ReportName, this.html5Renderer.m_pageNum, "_gr");
			Stream stream = this.html5Renderer.CreateStream(streamName, "txt", Encoding.UTF8, "text/plain", true, AspNetCore.ReportingServices.Interfaces.StreamOper.CreateOnly);
			this.html5Renderer.m_growRectangleIdsStream = new BufferedStream(stream);
			this.needsGrowRectangleScript = true;
		}

		private void RenderRectangleMeasurements(RPLItemMeasurement measurement, IRPLStyle style)
		{
			float width = measurement.Width;
			float height = measurement.Height;
			this.html5Renderer.RenderMeasurementWidth(width, true);
			if (this.html5Renderer.m_deviceInfo.IsBrowserIE && this.html5Renderer.m_deviceInfo.BrowserMode == BrowserMode.Standards && !this.html5Renderer.m_deviceInfo.IsBrowserIE6)
			{
				this.html5Renderer.RenderMeasurementMinHeight(height);
			}
			else
			{
				this.html5Renderer.RenderMeasurementHeight(height);
			}
		}
	}
}
