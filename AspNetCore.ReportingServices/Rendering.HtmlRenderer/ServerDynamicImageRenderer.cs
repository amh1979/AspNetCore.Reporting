using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Drawing;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal class ServerDynamicImageRenderer : IReportItemRenderer
	{
		private HTML5Renderer html5Renderer;

		public ServerDynamicImageRenderer(HTML5Renderer renderer)
		{
			this.html5Renderer = renderer;
		}

		public void RenderReportItem(RPLElement dynamicImage, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, bool renderId, bool treatAsTopLevel)
		{
			RPLElementProps elementProps = dynamicImage.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RPLDynamicImageProps rPLDynamicImageProps = (RPLDynamicImageProps)elementProps;
			string tooltip = this.html5Renderer.GetTooltip(rPLDynamicImageProps);
			if (dynamicImage != null)
			{
				bool flag = rPLDynamicImageProps.ActionImageMapAreas != null && rPLDynamicImageProps.ActionImageMapAreas.Length > 0;
				Rectangle rectangle = this.RenderDynamicImage(measurement, rPLDynamicImageProps);
				int xOffset = 0;
				int yOffset = 0;
				bool flag2 = !rectangle.IsEmpty;
				bool flag3 = !this.html5Renderer.m_deviceInfo.IsBrowserSafari || this.html5Renderer.m_deviceInfo.AllowScript || !styleContext.InTablix;
				if (flag3)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
					if (flag2 && this.html5Renderer.m_elementExtender.ShouldApplyToElement(treatAsTopLevel))
					{
						this.WriteReportItemDataName();
					}
					this.html5Renderer.WriteAccesibilityTags(RenderRes.AccessibleChartLabel, rPLDynamicImageProps, treatAsTopLevel);
					if (treatAsTopLevel)
					{
						string accessibleAriaName = string.IsNullOrEmpty(tooltip) ? RenderRes.AccessibleChartLabel : RenderRes.AccessibleChartNavigationGroupLabel(tooltip);
						this.html5Renderer.WriteAriaAccessibleTags(accessibleAriaName);
					}
				}
				bool flag4 = this.html5Renderer.m_deviceInfo.DataVisualizationFitSizing == DataVisualizationFitSizing.Exact && styleContext.InTablix;
				if (flag2)
				{
					RPLFormat.Sizings sizing = (RPLFormat.Sizings)(flag4 ? 1 : 0);
					this.html5Renderer.WriteOuterConsolidation(rectangle, sizing, rPLDynamicImageProps.UniqueName);
					this.html5Renderer.RenderReportItemStyle(dynamicImage, (RPLItemMeasurement)null, ref borderContext);
					xOffset = rectangle.Left;
					yOffset = rectangle.Top;
				}
				else if (flag4 && this.html5Renderer.m_deviceInfo.AllowScript)
				{
					if (this.html5Renderer.m_imgFitDivIdsStream == null)
					{
						this.html5Renderer.CreateImgFitDivImageIdsStream();
					}
					this.html5Renderer.WriteIdToSecondaryStream(this.html5Renderer.m_imgFitDivIdsStream, rPLDynamicImageProps.UniqueName + "_ifd");
					this.html5Renderer.RenderReportItemId(rPLDynamicImageProps.UniqueName + "_ifd");
				}
				if (flag3)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				}
				this.html5Renderer.WriteStream(HTMLElements.m_img);
				if (this.html5Renderer.m_elementExtender.ShouldApplyToElement(treatAsTopLevel))
				{
					if (!flag2)
					{
						this.WriteReportItemDataName();
					}
					RPLItemPropsDef rPLItemPropsDef = (RPLItemPropsDef)definition;
					this.html5Renderer.WriteAttrEncoded(HTMLElements.m_reportItemCustomAttr, this.html5Renderer.GetReportItemPath(rPLItemPropsDef.Name));
				}
				if (this.html5Renderer.m_browserIE)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_imgOnError);
				}
				if (renderId)
				{
					this.html5Renderer.RenderReportItemId(rPLDynamicImageProps.UniqueName);
				}
				this.html5Renderer.WriteStream(HTMLElements.m_zeroBorder);
				bool flag5 = dynamicImage is RPLChart;
				if (flag)
				{
					this.html5Renderer.WriteAttrEncoded(HTMLElements.m_useMap, "#" + this.html5Renderer.m_deviceInfo.HtmlPrefixId + HTMLElements.m_mapPrefixString + rPLDynamicImageProps.UniqueName);
					if (flag4)
					{
						this.html5Renderer.OpenStyle();
						if (this.html5Renderer.m_useInlineStyle && !flag2)
						{
							this.html5Renderer.WriteStream(HTMLElements.m_styleHeight);
							this.html5Renderer.WriteStream(HTMLElements.m_percent);
							this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
							this.html5Renderer.WriteStream(HTMLElements.m_styleWidth);
							this.html5Renderer.WriteStream(HTMLElements.m_percent);
							this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
							flag5 = false;
						}
						this.html5Renderer.WriteStream("border-style:none;");
					}
				}
				else if (flag4 && this.html5Renderer.m_useInlineStyle && !flag2)
				{
					this.html5Renderer.PercentSizes();
					flag5 = false;
				}
				StyleContext styleContext2 = new StyleContext();
				if (!flag4 && (this.html5Renderer.m_deviceInfo.IsBrowserIE7 || this.html5Renderer.m_deviceInfo.IsBrowserIE6))
				{
					styleContext2.RenderMeasurements = false;
					styleContext2.RenderMinMeasurements = false;
				}
				if (!flag2)
				{
					if (flag4)
					{
						this.html5Renderer.RenderReportItemStyle(dynamicImage, (RPLItemMeasurement)null, ref borderContext, styleContext2);
					}
					else if (flag5)
					{
						RPLElementProps elementProps2 = dynamicImage.ElementProps;
						StyleContext styleContext3 = new StyleContext();
						styleContext3.RenderMeasurements = false;
						this.html5Renderer.OpenStyle();
						this.html5Renderer.RenderMeasurementStyle(measurement.Height, measurement.Width);
						this.html5Renderer.RenderReportItemStyle(dynamicImage, elementProps2, definition, measurement, styleContext3, ref borderContext, definition.ID);
					}
					else
					{
						this.html5Renderer.RenderReportItemStyle(dynamicImage, measurement, ref borderContext, styleContext2);
					}
				}
				else
				{
					this.html5Renderer.WriteClippedDiv(rectangle);
				}
				tooltip = (string.IsNullOrEmpty(tooltip) ? RenderRes.AccessibleChartLabel : tooltip);
				this.html5Renderer.WriteToolTipAttribute(tooltip);
				this.html5Renderer.WriteStream(HTMLElements.m_src);
				this.html5Renderer.RenderDynamicImageSrc(rPLDynamicImageProps);
				this.html5Renderer.WriteStreamCR(HTMLElements.m_closeTag);
				if (flag)
				{
					this.html5Renderer.RenderImageMapAreas(rPLDynamicImageProps.ActionImageMapAreas, (double)measurement.Width, (double)measurement.Height, rPLDynamicImageProps.UniqueName, xOffset, yOffset);
				}
				if (flag3)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
				}
			}
		}

		private void WriteReportItemDataName()
		{
			this.html5Renderer.WriteStream(HTMLElements.m_space);
			this.html5Renderer.WriteStream(HTMLElements.m_reportItemDataName);
			this.html5Renderer.WriteStream(HTMLElements.m_equal);
			this.html5Renderer.WriteStream(HTMLElements.m_quoteString);
			this.html5Renderer.WriteStream(this.html5Renderer.m_elementExtender.ApplyToElement());
			this.html5Renderer.WriteStream(HTMLElements.m_quoteString);
		}

		private Rectangle RenderDynamicImage(RPLItemMeasurement measurement, RPLDynamicImageProps dynamicImageProps)
		{
			if (this.html5Renderer.m_createSecondaryStreams != 0)
			{
				return dynamicImageProps.ImageConsolidationOffsets;
			}
			Stream stream = null;
			stream = this.html5Renderer.CreateStream(dynamicImageProps.StreamName, "png", null, "image/png", false, AspNetCore.ReportingServices.Interfaces.StreamOper.CreateAndRegister);
			if (dynamicImageProps.DynamicImageContentOffset >= 0)
			{
				this.html5Renderer.m_rplReport.GetImage(dynamicImageProps.DynamicImageContentOffset, stream);
			}
			else if (dynamicImageProps.DynamicImageContent != null)
			{
				byte[] array = new byte[4096];
				dynamicImageProps.DynamicImageContent.Position = 0L;
				int num2;
				for (int num = (int)dynamicImageProps.DynamicImageContent.Length; num > 0; num -= num2)
				{
					num2 = dynamicImageProps.DynamicImageContent.Read(array, 0, Math.Min(array.Length, num));
					stream.Write(array, 0, num2);
				}
			}
			return Rectangle.Empty;
		}
	}
}
