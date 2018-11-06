using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Web;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal class ImageRenderer : IReportItemRenderer
	{
		private HTML5Renderer html5Renderer;

		public ImageRenderer(HTML5Renderer renderer)
		{
			this.html5Renderer = renderer;
		}

		public void RenderReportItem(RPLElement reportItem, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, bool renderId, bool treatAsTopLevel)
		{
			RPLImageProps rPLImageProps = (RPLImageProps)reportItem.ElementProps;
			RPLImagePropsDef rPLImagePropsDef = (RPLImagePropsDef)rPLImageProps.Definition;
			RPLFormat.Sizings sizing = rPLImagePropsDef.Sizing;
			RPLImageData image = rPLImageProps.Image;
			this.html5Renderer.GetInnerContainerHeightSubtractBorders(measurement, rPLImageProps.Style);
			float innerContainerWidthSubtractBorders = this.html5Renderer.GetInnerContainerWidthSubtractBorders(measurement, rPLImageProps.Style);
			string text = this.html5Renderer.GetImageUrl(true, image);
			string ariaLabel = null;
			string role = null;
			string tooltip = this.html5Renderer.GetTooltip(rPLImageProps);
			if (treatAsTopLevel)
			{
				ariaLabel = (string.IsNullOrEmpty(tooltip) ? RenderRes.AccessibleImageLabel : RenderRes.AccessibleImageNavigationGroupLabel(tooltip));
				role = HTMLElements.m_navigationRole;
			}
			string altText = string.IsNullOrEmpty(tooltip) ? RenderRes.AccessibleImageLabel : tooltip;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (this.html5Renderer.m_elementExtender.ShouldApplyToElement(treatAsTopLevel))
			{
				dictionary.Add(HTMLElements.m_reportItemDataName, this.html5Renderer.m_elementExtender.ApplyToElement());
				dictionary.Add(HTMLElements.m_reportItemCustomAttrStr, this.html5Renderer.GetReportItemPath(rPLImagePropsDef.Name));
			}
			bool flag = rPLImageProps.ActionImageMapAreas != null && rPLImageProps.ActionImageMapAreas.Length > 0;
			if (flag)
			{
				string s = HTMLElements.m_hashTag + this.html5Renderer.m_deviceInfo.HtmlPrefixId + HTMLElements.m_mapPrefixString + rPLImageProps.UniqueName;
				dictionary.Add(HTMLElements.m_useMapName, HttpUtility.HtmlAttributeEncode(s));
			}
			if (this.html5Renderer.HasAction(rPLImageProps.ActionInfo))
			{
				this.RenderElementHyperlink(rPLImageProps.Style, rPLImageProps.ActionInfo.Actions[0]);
			}
			if (!styleContext.InTablix)
			{
				if (sizing == RPLFormat.Sizings.AutoSize)
				{
					styleContext.RenderMeasurements = false;
				}
				this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
				this.html5Renderer.RenderReportItemStyle(reportItem, (RPLElementProps)rPLImageProps, (RPLElementPropsDef)rPLImagePropsDef, measurement, styleContext, ref borderContext, rPLImagePropsDef.ID);
				this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "data:image/gif;base64," + Convert.ToBase64String(HTMLRendererResources.GetBytes("Blank.gif"));
			}
			HtmlElement htmlElement;
			switch (sizing)
			{
			case RPLFormat.Sizings.FitProportional:
				htmlElement = new FitProportionalImageElement(text, innerContainerWidthSubtractBorders, role, altText, ariaLabel, dictionary);
				break;
			case RPLFormat.Sizings.Fit:
				htmlElement = new FitImageElement(text, role, altText, ariaLabel, dictionary);
				break;
			case RPLFormat.Sizings.Clip:
				htmlElement = new ClipImageElement(text, role, altText, ariaLabel, dictionary);
				break;
			default:
				htmlElement = new OriginalSizeImageElement(text, role, altText, ariaLabel, dictionary);
				break;
			}
			htmlElement.Render(new Html5OutputStream(this.html5Renderer));
			if (!styleContext.InTablix)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
			}
			if (this.html5Renderer.HasAction(rPLImageProps.ActionInfo))
			{
				this.html5Renderer.WriteStream(HTMLElements.m_closeA);
			}
			if (flag)
			{
				this.html5Renderer.RenderImageMapAreas(rPLImageProps.ActionImageMapAreas, (double)measurement.Width, (double)measurement.Height, rPLImageProps.UniqueName, 0, 0);
			}
		}

		private bool RenderHyperlink(RPLAction action, RPLFormat.TextDecorations textDec, string color)
		{
			this.html5Renderer.WriteStream(HTMLElements.m_openA);
			this.html5Renderer.RenderTabIndex();
			this.html5Renderer.RenderActionHref(action, textDec, color);
			this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			return true;
		}

		private bool RenderElementHyperlink(IRPLStyle style, RPLAction action)
		{
			object obj = style[24];
			obj = ((obj != null) ? obj : ((object)RPLFormat.TextDecorations.None));
			string color = (string)style[27];
			return this.RenderHyperlink(action, (RPLFormat.TextDecorations)obj, color);
		}
	}
}
