using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal class TextBoxRenderer : IReportItemRenderer
	{
		private const string FitVertTextSuffix = "_fvt";

		private HTML5Renderer html5Renderer;

		public TextBoxRenderer(HTML5Renderer renderer)
		{
			this.html5Renderer = renderer;
		}

		public void RenderReportItem(RPLElement reportItem, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, bool renderId, bool treatAsTopLevel)
		{
			RPLTextBox rPLTextBox = reportItem as RPLTextBox;
			if (HTML5Renderer.IsWritingModeVertical(rPLTextBox.ElementProps.Style))
			{
				this.RenderVerticalTextBox(rPLTextBox, rPLTextBox.ElementProps as RPLTextBoxProps, rPLTextBox.ElementProps.Definition as RPLTextBoxPropsDef, measurement, styleContext, ref borderContext, renderId, treatAsTopLevel);
			}
			else if (styleContext.InTablix)
			{
				this.RenderTextBoxPercent(rPLTextBox, rPLTextBox.ElementProps as RPLTextBoxProps, rPLTextBox.ElementProps.Definition as RPLTextBoxPropsDef, measurement, styleContext, renderId, treatAsTopLevel);
			}
			else
			{
				this.RenderTextBox(rPLTextBox, rPLTextBox.ElementProps as RPLTextBoxProps, rPLTextBox.ElementProps.Definition as RPLTextBoxPropsDef, measurement, styleContext, ref borderContext, renderId, treatAsTopLevel);
			}
		}

		protected void RenderVerticalTextBox(RPLTextBox textBox, RPLTextBoxProps textBoxProps, RPLTextBoxPropsDef textBoxPropsDef, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, bool renderId, bool treatAsTopLevel = false)
		{
			string text = null;
			RPLElementStyle style = textBoxProps.Style;
			RPLStyleProps nonSharedStyle = textBoxProps.NonSharedStyle;
			RPLStyleProps sharedStyle = textBoxPropsDef.SharedStyle;
			RPLStyleProps actionStyle = null;
			RPLActionInfo actionInfo = textBoxProps.ActionInfo;
			bool flag = this.html5Renderer.NeedSharedToggleParent(textBoxProps);
			bool flag2 = this.html5Renderer.CanSort(textBoxPropsDef);
			bool flag3 = false;
			text = textBoxProps.Value;
			if (string.IsNullOrEmpty(text))
			{
				text = textBoxPropsDef.Value;
			}
			this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
			this.html5Renderer.WriteAccesibilityTags(RenderRes.AccessibleTextBoxLabel, textBoxProps, treatAsTopLevel);
			if (textBoxPropsDef.CanGrow && textBoxPropsDef.CanShrink)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_classCanGrowBothTextBox);
			}
			else if (textBoxPropsDef.CanGrow)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_classCanGrowVerticalTextBox);
			}
			else if (textBoxPropsDef.CanShrink)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_classCanShrinkVerticalTextBox);
			}
			this.html5Renderer.OpenStyle();
			this.html5Renderer.RenderMeasurementHeight(measurement.Height);
			if (textBoxPropsDef.CanGrow || textBoxPropsDef.CanShrink)
			{
				if (this.html5Renderer.m_deviceInfo.IsBrowserIE)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_wordBreakAll);
				}
				else
				{
					this.html5Renderer.WriteStream(HTMLElements.m_wordBreak);
					this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
				}
			}
			if (!styleContext.InTablix)
			{
				this.html5Renderer.RenderMeasurementWidth(measurement.Width, false);
				this.html5Renderer.WriteStream(HTMLElements.m_overflowHidden);
				this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
			}
			this.html5Renderer.CloseStyle(true);
			this.html5Renderer.WriteToolTip(textBoxProps);
			this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
			string language = (string)style[32];
			this.html5Renderer.RenderLanguage(language);
			this.RenderVerticalAlignStyle(sharedStyle, styleContext, textBoxProps);
			this.html5Renderer.RenderReportItemStyle((RPLElement)textBox, (RPLElementProps)textBoxProps, (RPLElementPropsDef)textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, textBoxPropsDef.ID + "vtb", true);
			this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			if (flag)
			{
				this.html5Renderer.RenderToggleImage(textBoxProps);
			}
			if (flag2)
			{
				object obj = style[26];
				RPLFormat.VerticalAlignments val = RPLFormat.VerticalAlignments.Top;
				if (obj != null)
				{
					val = (RPLFormat.VerticalAlignments)obj;
				}
				this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
				this.html5Renderer.WriteStream(HTMLElements.m_openStyle);
				this.html5Renderer.WriteStream(HTMLElements.m_verticalAlign);
				this.html5Renderer.WriteStream(EnumStrings.GetValue(val));
				this.html5Renderer.WriteStream(HTMLElements.m_quote);
				this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				this.html5Renderer.RenderSortImage(textBoxProps);
				this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
			}
			RPLAction rPLAction = null;
			if (this.html5Renderer.HasAction(actionInfo))
			{
				rPLAction = actionInfo.Actions[0];
				this.html5Renderer.RenderElementHyperlinkAllTextStyles(textBoxProps.Style, rPLAction, textBoxPropsDef.ID + "a");
				flag3 = true;
			}
			this.RenderTextBoxContent(textBox, textBoxProps, textBoxPropsDef, text, actionStyle, flag || flag2, measurement, rPLAction);
			if (flag3)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_closeA);
			}
			this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
			this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
		}

		protected void RenderTextBox(RPLTextBox textBox, RPLTextBoxProps textBoxProps, RPLTextBoxPropsDef textBoxPropsDef, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, bool renderId, bool treatAsTopLevel = false)
		{
			string text = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			RPLStyleProps actionStyle = null;
			RPLActionInfo actionInfo = textBoxProps.ActionInfo;
			RPLElementStyle style = textBoxProps.Style;
			bool flag4 = this.html5Renderer.CanSort(textBoxPropsDef);
			bool flag5 = this.html5Renderer.NeedSharedToggleParent(textBoxProps);
			bool flag6 = false;
			bool isSimple = textBoxPropsDef.IsSimple;
			bool flag7 = !isSimple && flag5;
			bool flag8 = flag4 || flag7;
			HTML5Renderer.IsDirectionRTL(style);
			RPLStyleProps nonSharedStyle = textBoxProps.NonSharedStyle;
			RPLStyleProps sharedStyle = textBoxPropsDef.SharedStyle;
			bool ignoreVerticalAlign = styleContext.IgnoreVerticalAlign;
			float num = this.html5Renderer.GetInnerContainerWidthSubtractBorders(measurement, textBoxProps.Style);
			HTML5Renderer.GetInnerContainerHeightSubtractHalfBorders(measurement, style);
			HTML5Renderer.GetInnerContainerWidthSubtractHalfBorders(measurement, style);
			if (isSimple)
			{
				text = textBoxProps.Value;
				if (string.IsNullOrEmpty(text))
				{
					text = textBoxPropsDef.Value;
				}
				if (string.IsNullOrEmpty(text) && !flag4 && !flag5)
				{
					flag = true;
				}
			}
			if (textBoxProps.UniqueName == null)
			{
				flag4 = false;
				flag5 = false;
				renderId = false;
			}
			this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
			this.html5Renderer.WriteAccesibilityTags(RenderRes.AccessibleTextBoxLabel, textBoxProps, treatAsTopLevel);
			if (flag)
			{
				styleContext.EmptyTextBox = true;
				this.html5Renderer.OpenStyle();
				this.html5Renderer.RenderMeasurementWidth(measurement.Width, true);
				if (!textBoxPropsDef.CanShrink)
				{
					this.html5Renderer.RenderMeasurementHeight(measurement.Height);
				}
			}
			else
			{
				object obj = style[26];
				if (textBoxPropsDef.CanGrow)
				{
					this.html5Renderer.OpenStyle();
					this.html5Renderer.RenderMeasurementWidth(measurement.Width, true);
					if (!textBoxPropsDef.CanShrink)
					{
						this.html5Renderer.RenderMeasurementMinHeight(measurement.Height);
					}
					styleContext.RenderMeasurements = false;
					if (flag8)
					{
						if (!textBoxPropsDef.CanShrink)
						{
							this.RenderVerticalAlignStyle(sharedStyle, styleContext, textBoxProps);
						}
						styleContext.StyleOnCell = true;
						this.html5Renderer.RenderReportItemStyle((RPLElement)textBox, (RPLElementProps)textBoxProps, (RPLElementPropsDef)textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, textBoxPropsDef.ID + "p", false);
						styleContext.StyleOnCell = false;
						styleContext.NoBorders = true;
					}
					if (textBoxPropsDef.CanShrink)
					{
						if (flag5)
						{
							flag2 = true;
						}
						if (!flag2 && obj != null && !styleContext.IgnoreVerticalAlign)
						{
							obj = EnumStrings.GetValue((RPLFormat.VerticalAlignments)obj);
							this.html5Renderer.WriteStream(HTMLElements.m_verticalAlign);
							this.html5Renderer.WriteStream(obj);
							this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
						}
						this.html5Renderer.CloseStyle(true);
						this.html5Renderer.WriteStreamCR(HTMLElements.m_closeBracket);
						if (!flag2)
						{
							styleContext.IgnoreVerticalAlign = true;
						}
						this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
						if (flag8)
						{
							this.RenderVerticalAlignStyle(sharedStyle, styleContext, textBoxProps);
						}
					}
				}
				else
				{
					styleContext.IgnoreVerticalAlign = true;
					if (!flag8)
					{
						bool onlyRenderMeasurementsBackgroundBorders = styleContext.OnlyRenderMeasurementsBackgroundBorders;
						bool noBorder = styleContext.NoBorders;
						styleContext.OnlyRenderMeasurementsBackgroundBorders = true;
						int num2 = 0;
						this.html5Renderer.RenderReportItemStyle(textBox, textBoxProps, textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref num2, textBoxPropsDef.ID + "v", false);
						styleContext.OnlyRenderMeasurementsBackgroundBorders = onlyRenderMeasurementsBackgroundBorders;
						measurement = null;
						styleContext.NoBorders = true;
					}
					this.html5Renderer.WriteStreamCR(HTMLElements.m_closeBracket);
					styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
					if (obj != null && (RPLFormat.VerticalAlignments)obj != 0)
					{
						this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
						flag2 = true;
					}
					else
					{
						this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
						flag3 = true;
					}
					if (flag8)
					{
						this.html5Renderer.OpenStyle();
						this.html5Renderer.RenderMeasurementWidth(measurement.Width, true);
						this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
						this.RenderVerticalAlignStyle(sharedStyle, styleContext, textBoxProps);
					}
					if (textBoxPropsDef.CanShrink)
					{
						bool noBorders = styleContext.NoBorders;
						styleContext.NoBorders = true;
						this.html5Renderer.RenderReportItemStyle((RPLElement)textBox, (RPLElementProps)textBoxProps, (RPLElementPropsDef)textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, textBoxPropsDef.ID + "s", false);
						this.html5Renderer.CloseStyle(true);
						this.html5Renderer.WriteStreamCR(HTMLElements.m_closeBracket);
						this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
						styleContext.NoBorders = noBorders;
						styleContext.StyleOnCell = true;
					}
					if (flag8)
					{
						this.html5Renderer.RenderReportItemStyle((RPLElement)textBox, (RPLElementProps)textBoxProps, (RPLElementPropsDef)textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, textBoxPropsDef.ID + "p", false);
						styleContext.StyleOnCell = false;
						this.html5Renderer.CloseStyle(true);
					}
				}
			}
			if (!flag8)
			{
				this.RenderVerticalAlignStyle(sharedStyle, styleContext, textBoxProps);
			}
			string textBoxClass = this.html5Renderer.GetTextBoxClass(textBoxPropsDef, textBoxProps, nonSharedStyle, textBoxPropsDef.ID);
			this.html5Renderer.RenderReportItemStyle((RPLElement)textBox, (RPLElementProps)textBoxProps, (RPLElementPropsDef)textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, textBoxClass, false);
			this.html5Renderer.CloseStyle(true);
			styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
			if (renderId || flag5 || flag4)
			{
				this.html5Renderer.RenderReportItemId(textBoxProps.UniqueName);
			}
			this.html5Renderer.WriteToolTip(textBoxProps);
			if (!flag)
			{
				string language = (string)style[32];
				this.html5Renderer.RenderLanguage(language);
			}
			this.html5Renderer.WriteStreamCR(HTMLElements.m_closeBracket);
			if (flag8)
			{
				styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
				this.RenderAtStart(textBoxProps, style, flag4, flag7);
				styleContext.InTablix = true;
			}
			if (flag5 && isSimple)
			{
				this.html5Renderer.RenderToggleImage(textBoxProps);
			}
			RPLAction rPLAction = null;
			if (this.html5Renderer.HasAction(actionInfo))
			{
				rPLAction = actionInfo.Actions[0];
				this.html5Renderer.RenderElementHyperlinkAllTextStyles(textBoxProps.Style, rPLAction, textBoxPropsDef.ID + "a");
				flag6 = true;
				if (flag)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
					this.html5Renderer.OpenStyle();
					float num3 = 0f;
					if (measurement != null)
					{
						num3 = measurement.Height;
					}
					if (num3 > 0.0)
					{
						num3 = this.html5Renderer.GetInnerContainerHeightSubtractBorders(measurement, textBoxProps.Style);
						this.html5Renderer.RenderMeasurementMinHeight(num3);
					}
					this.html5Renderer.WriteStream(HTMLElements.m_cursorHand);
					this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
					this.html5Renderer.CloseStyle(true);
					this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				}
			}
			this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
			this.html5Renderer.OpenStyle();
			if (flag5)
			{
				num = (float)(num - 3.25);
			}
			if (flag4)
			{
				num = (float)(num - 4.2300000190734863);
			}
			num = (float)((num > 0.0) ? num : 0.0);
			this.html5Renderer.RenderMeasurementWidth(num, true);
			this.html5Renderer.CloseStyle(true);
			this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			this.RenderTextBoxContent(textBox, textBoxProps, textBoxPropsDef, text, actionStyle, flag5 || flag4, measurement, rPLAction);
			this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
			if (flag6)
			{
				if (flag)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
				}
				this.html5Renderer.WriteStream(HTMLElements.m_closeA);
			}
			if (isSimple && !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text.Trim()))
			{
				this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
			}
			if (flag)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
			}
			else if (textBoxPropsDef.CanGrow)
			{
				if (textBoxPropsDef.CanShrink)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
				}
				this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
			}
			else
			{
				if (flag2)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
				}
				if (flag3)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
				}
				this.html5Renderer.WriteStreamCR(HTMLElements.m_closeDiv);
			}
		}

		protected void RenderTextBoxPercent(RPLTextBox textBox, RPLTextBoxProps textBoxProps, RPLTextBoxPropsDef textBoxPropsDef, RPLItemMeasurement measurement, StyleContext styleContext, bool renderId, bool treatAsTopLevel = false)
		{
			RPLStyleProps actionStyle = null;
			RPLActionInfo actionInfo = textBoxProps.ActionInfo;
			RPLStyleProps nonSharedStyle = textBoxProps.NonSharedStyle;
			RPLStyleProps sharedStyle = textBoxPropsDef.SharedStyle;
			RPLElementStyle style = textBoxProps.Style;
			bool flag = this.html5Renderer.CanSort(textBoxPropsDef);
			bool flag2 = this.html5Renderer.NeedSharedToggleParent(textBoxProps);
			bool flag3 = false;
			bool isSimple = textBoxPropsDef.IsSimple;
			bool flag4 = HTML5Renderer.IsDirectionRTL(style);
			bool renderDirectionStyles = HTML5Renderer.IsWritingModeVertical(style);
			float innerContainerWidthSubtractHalfBorders = HTML5Renderer.GetInnerContainerWidthSubtractHalfBorders(measurement, style);
			float innerContainerHeightSubtractHalfBorders = HTML5Renderer.GetInnerContainerHeightSubtractHalfBorders(measurement, style);
			this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
			this.html5Renderer.WriteAccesibilityTags(RenderRes.AccessibleTextBoxLabel, textBoxProps, treatAsTopLevel);
			bool flag5 = flag2 && !isSimple;
			bool flag6 = flag || flag5;
			if (renderId || flag2 || flag)
			{
				this.html5Renderer.RenderReportItemId(textBoxProps.UniqueName);
				this.html5Renderer.OpenStyle();
				this.html5Renderer.WriteStream(HTMLElements.m_styleDisplayFlex);
				if (flag2 && !flag && !flag4)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_flexFlowRowReverse);
				}
				if (flag4 && !flag6)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_msFlexJustifyContent);
					this.html5Renderer.WriteStream(HTMLElements.m_msFlexStart);
					this.html5Renderer.WriteStream(HTMLElements.m_webkitFlexJustifyContent);
					this.html5Renderer.WriteStream(HTMLElements.m_flexStart);
					this.html5Renderer.WriteStream(HTMLElements.m_flexJustifyContent);
					this.html5Renderer.WriteStream(HTMLElements.m_flexStart);
					this.html5Renderer.WriteStream(HTMLElements.m_direction);
					this.html5Renderer.WriteStream("rtl");
				}
				if (textBoxPropsDef.CanGrow)
				{
					this.html5Renderer.CloseStyle(true);
				}
			}
			if (flag2 && !flag)
			{
				if (!textBoxPropsDef.CanGrow)
				{
					this.html5Renderer.CloseStyle(true);
				}
				this.html5Renderer.WriteStreamCR(HTMLElements.m_closeBracket);
				this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
				this.html5Renderer.OpenStyle();
				this.html5Renderer.WriteStream(HTMLElements.m_styleDisplayFlex);
				this.html5Renderer.WriteStream(HTMLElements.m_styleWidth);
				this.html5Renderer.WriteStream(HTMLElements.m_percent);
				this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
				this.RenderJustifyContentStyle(sharedStyle, textBoxProps);
				if (textBoxPropsDef.CanGrow)
				{
					this.html5Renderer.CloseStyle(true);
				}
			}
			if (!textBoxPropsDef.CanGrow)
			{
				if (!renderId && !flag2 && !flag)
				{
					this.html5Renderer.OpenStyle();
				}
				if (measurement != null)
				{
					styleContext.RenderMeasurements = false;
					if (textBoxPropsDef.CanShrink)
					{
						this.html5Renderer.RenderMeasurementMaxHeight(innerContainerHeightSubtractHalfBorders);
					}
					else
					{
						this.html5Renderer.RenderMeasurementHeight(innerContainerHeightSubtractHalfBorders);
					}
					this.html5Renderer.RenderMeasurementWidth(innerContainerWidthSubtractHalfBorders, true);
					this.html5Renderer.WriteStream(HTMLElements.m_overflowHidden);
					this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
				}
				else
				{
					styleContext.RenderMeasurements = true;
				}
				if (!flag6)
				{
					object obj = style[26];
					bool flag7 = obj != null && (RPLFormat.VerticalAlignments)obj != 0 && !textBoxPropsDef.CanGrow;
					flag6 = flag7;
				}
				measurement = null;
			}
			if (flag6)
			{
				styleContext.RenderMeasurements = false;
				this.html5Renderer.CloseStyle(true);
				this.html5Renderer.WriteStreamCR(HTMLElements.m_closeBracket);
				this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
				if (isSimple && (flag || flag2))
				{
					this.html5Renderer.WriteClassName(HTMLElements.m_percentHeightInlineTable, HTMLElements.m_classPercentHeightInlineTable);
				}
				else
				{
					this.html5Renderer.WriteClassName(HTMLElements.m_percentSizeInlineTable, HTMLElements.m_classPercentSizeInlineTable);
				}
				this.html5Renderer.RenderReportLanguage();
				this.html5Renderer.OpenStyle();
				if (flag4 && (flag || flag2) && (!flag || !flag2))
				{
					this.html5Renderer.WriteStream(HTMLElements.m_msFlexJustifyContent);
					if (flag)
					{
						this.html5Renderer.WriteStream(HTMLElements.m_msFlexEnd);
					}
					else
					{
						this.html5Renderer.WriteStream(HTMLElements.m_msFlexStart);
					}
					this.html5Renderer.WriteStream(HTMLElements.m_webkitFlexJustifyContent);
					if (flag)
					{
						this.html5Renderer.WriteStream(HTMLElements.m_flexEnd);
					}
					else
					{
						this.html5Renderer.WriteStream(HTMLElements.m_flexStart);
					}
					this.html5Renderer.WriteStream(HTMLElements.m_flexJustifyContent);
					if (flag)
					{
						this.html5Renderer.WriteStream(HTMLElements.m_flexEnd);
					}
					else
					{
						this.html5Renderer.WriteStream(HTMLElements.m_flexStart);
					}
				}
				this.html5Renderer.WriteStream(HTMLElements.m_styleWidth);
				this.html5Renderer.WriteStream(HTMLElements.m_percent);
				this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
				this.html5Renderer.WriteStream(HTMLElements.m_styleDisplayFlex);
				this.html5Renderer.WriteStream(HTMLElements.m_flexFlowRow);
				this.html5Renderer.CloseStyle(true);
				this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				if (flag2)
				{
					this.html5Renderer.RenderToggleImage(textBoxProps);
				}
				this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
			}
			this.html5Renderer.WriteStream(HTMLElements.m_id);
			this.html5Renderer.WriteReportItemId(textBoxProps.UniqueName + HTMLElements.m_ariaSuffix);
			this.html5Renderer.WriteStream(HTMLElements.m_quote);
			this.html5Renderer.OpenStyle();
			this.html5Renderer.WriteStream(HTMLElements.m_styleMaxWidth);
			this.html5Renderer.WriteStream(innerContainerWidthSubtractHalfBorders.ToString("f2", CultureInfo.InvariantCulture));
			this.html5Renderer.WriteStream(HTMLElements.m_mm);
			this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
			if (flag4 && flag && flag2)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_direction);
				this.html5Renderer.WriteStream("rtl");
				this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
			}
			if (flag && textBoxPropsDef.IsSimple)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_styleDisplayFlex);
				this.html5Renderer.WriteStream(HTMLElements.m_flexFlowRow);
			}
			if (!flag4)
			{
				this.RenderJustifyContentStyle(sharedStyle, textBoxProps);
			}
			if (!textBoxPropsDef.CanGrow)
			{
				object obj2 = textBox.ElementProps.Style[26];
				if (obj2 != null && (RPLFormat.VerticalAlignments)obj2 != 0)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_stylePositionRelative);
				}
				else if (!flag4 && flag2)
				{
					this.html5Renderer.RenderMeasurementLeft(0f);
				}
			}
			int num = 0;
			this.html5Renderer.RenderReportItemStyle(textBox, textBoxProps, textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref num, textBoxPropsDef.ID, renderDirectionStyles);
			this.html5Renderer.CloseStyle(true);
			this.html5Renderer.WriteToolTip(textBoxProps);
			this.html5Renderer.WriteStreamCR(HTMLElements.m_closeBracket);
			if (flag2 && !flag6)
			{
				this.html5Renderer.RenderToggleImage(textBoxProps);
			}
			RPLAction rPLAction = null;
			if (this.html5Renderer.HasAction(actionInfo))
			{
				rPLAction = actionInfo.Actions[0];
				this.html5Renderer.RenderElementHyperlinkAllTextStyles(style, rPLAction, textBoxPropsDef.ID + "a");
				flag3 = true;
			}
			string text = null;
			if (textBoxPropsDef.IsSimple)
			{
				text = textBoxProps.Value;
				if (string.IsNullOrEmpty(text))
				{
					text = textBoxPropsDef.Value;
				}
			}
			this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
			this.html5Renderer.OpenStyle();
			float num2 = innerContainerWidthSubtractHalfBorders;
			if (flag2)
			{
				num2 = (float)(num2 - 3.25);
			}
			if (flag)
			{
				num2 = (float)(num2 - 4.2300000190734863);
			}
			num2 = (float)((num2 > 0.0) ? num2 : 0.0);
			this.html5Renderer.RenderMeasurementWidth(num2, true);
			this.html5Renderer.CloseStyle(true);
			this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			this.RenderTextBoxContent(textBox, textBoxProps, textBoxPropsDef, text, actionStyle, flag2 || flag, measurement, rPLAction);
			this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
			if (flag3)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_closeA);
			}
			if (flag6 || flag2)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
			}
			if (flag)
			{
				this.RenderAtStart(textBoxProps, style, flag, false);
			}
			if (flag6)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
			}
			this.html5Renderer.WriteStreamCR(HTMLElements.m_closeDiv);
		}

		private void RenderJustifyContentStyle(IRPLStyle styleProps, RPLTextBoxProps textBoxProps)
		{
			object obj = styleProps[25];
			if (obj != null)
			{
				RPLFormat.TextAlignments textAlignments = (RPLFormat.TextAlignments)obj;
				if (textAlignments != 0)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_msFlexJustifyContent);
					switch (textAlignments)
					{
					case RPLFormat.TextAlignments.Left:
						this.html5Renderer.WriteStream(HTMLElements.m_msFlexStart);
						break;
					case RPLFormat.TextAlignments.Center:
						this.html5Renderer.WriteStream(HTMLElements.m_flexCenter);
						break;
					case RPLFormat.TextAlignments.Right:
						this.html5Renderer.WriteStream(HTMLElements.m_msFlexEnd);
						break;
					}
					this.html5Renderer.WriteStream(HTMLElements.m_webkitFlexJustifyContent);
					switch (textAlignments)
					{
					case RPLFormat.TextAlignments.Left:
						this.html5Renderer.WriteStream(HTMLElements.m_flexStart);
						break;
					case RPLFormat.TextAlignments.Center:
						this.html5Renderer.WriteStream(HTMLElements.m_flexCenter);
						break;
					case RPLFormat.TextAlignments.Right:
						this.html5Renderer.WriteStream(HTMLElements.m_flexEnd);
						break;
					}
					this.html5Renderer.WriteStream(HTMLElements.m_flexJustifyContent);
					switch (textAlignments)
					{
					case RPLFormat.TextAlignments.Left:
						this.html5Renderer.WriteStream(HTMLElements.m_flexStart);
						break;
					case RPLFormat.TextAlignments.Center:
						this.html5Renderer.WriteStream(HTMLElements.m_flexCenter);
						break;
					case RPLFormat.TextAlignments.Right:
						this.html5Renderer.WriteStream(HTMLElements.m_flexEnd);
						break;
					}
				}
				else if (HTML5Renderer.GetTextAlignForType(textBoxProps))
				{
					this.html5Renderer.WriteStream(HTMLElements.m_flexEnd);
				}
			}
			if (HTML5Renderer.IsDirectionRTL(styleProps))
			{
				this.html5Renderer.WriteStream(HTMLElements.m_flexFlowRowReverse);
			}
		}

		private void RenderVerticalAlignStyle(RPLStyleProps sharedStyle, StyleContext styleContext, RPLTextBoxProps textBoxProps)
		{
			this.html5Renderer.OpenStyle();
			bool flag = this.html5Renderer.m_deviceInfo.IsBrowserGeckoEngine && HTML5Renderer.IsWritingModeVertical(sharedStyle);
			if (!flag)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_styleDisplayFlex);
			}
			object obj = ((IRPLStyle)sharedStyle)[26];
			if (obj != null && !styleContext.IgnoreVerticalAlign && !flag)
			{
				RPLFormat.VerticalAlignments verticalAlignments = (RPLFormat.VerticalAlignments)obj;
				this.html5Renderer.WriteStream(HTMLElements.m_msFlexAlignItems);
				switch (verticalAlignments)
				{
				case RPLFormat.VerticalAlignments.Top:
					this.html5Renderer.WriteStream(HTMLElements.m_msFlexStart);
					break;
				case RPLFormat.VerticalAlignments.Middle:
					this.html5Renderer.WriteStream(HTMLElements.m_flexCenter);
					break;
				case RPLFormat.VerticalAlignments.Bottom:
					this.html5Renderer.WriteStream(HTMLElements.m_msFlexEnd);
					break;
				}
				this.html5Renderer.WriteStream(HTMLElements.m_webkitFlexAlignItems);
				switch (verticalAlignments)
				{
				case RPLFormat.VerticalAlignments.Top:
					this.html5Renderer.WriteStream(HTMLElements.m_flexStart);
					break;
				case RPLFormat.VerticalAlignments.Middle:
					this.html5Renderer.WriteStream(HTMLElements.m_flexCenter);
					break;
				case RPLFormat.VerticalAlignments.Bottom:
					this.html5Renderer.WriteStream(HTMLElements.m_flexEnd);
					break;
				}
				this.html5Renderer.WriteStream(HTMLElements.m_flexAlignItems);
				switch (verticalAlignments)
				{
				case RPLFormat.VerticalAlignments.Top:
					this.html5Renderer.WriteStream(HTMLElements.m_flexStart);
					break;
				case RPLFormat.VerticalAlignments.Middle:
					this.html5Renderer.WriteStream(HTMLElements.m_flexCenter);
					break;
				case RPLFormat.VerticalAlignments.Bottom:
					this.html5Renderer.WriteStream(HTMLElements.m_flexEnd);
					break;
				}
			}
			this.RenderJustifyContentStyle(sharedStyle, textBoxProps);
		}

		private void RenderAtStart(RPLTextBoxProps textBoxProps, IRPLStyle style, bool renderSort, bool renderToggle)
		{
			if (!renderSort && !renderToggle)
			{
				return;
			}
			this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
			object obj = style[26];
			RPLFormat.VerticalAlignments verticalAlignments = RPLFormat.VerticalAlignments.Top;
			if (obj != null)
			{
				verticalAlignments = (RPLFormat.VerticalAlignments)obj;
			}
			if (HTML5Renderer.IsWritingModeVertical(style))
			{
				this.html5Renderer.WriteStream(HTMLElements.m_openStyle);
				this.html5Renderer.WriteStream(HTMLElements.m_textAlign);
				switch (verticalAlignments)
				{
				case RPLFormat.VerticalAlignments.Top:
					this.html5Renderer.WriteStream(HTMLElements.m_rightValue);
					break;
				case RPLFormat.VerticalAlignments.Middle:
					this.html5Renderer.WriteStream(HTMLElements.m_centerValue);
					break;
				case RPLFormat.VerticalAlignments.Bottom:
					this.html5Renderer.WriteStream(HTMLElements.m_leftValue);
					break;
				}
				this.html5Renderer.WriteStream(HTMLElements.m_quote);
				this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				if (renderSort)
				{
					this.html5Renderer.RenderSortImage(textBoxProps);
				}
				if (renderToggle)
				{
					this.html5Renderer.RenderToggleImage(textBoxProps);
				}
				this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
				this.html5Renderer.WriteStream(HTMLElements.m_closeTD);
				this.html5Renderer.WriteStream(HTMLElements.m_closeTR);
				this.html5Renderer.WriteStream(HTMLElements.m_firstTD);
			}
			else
			{
				this.html5Renderer.WriteStream(HTMLElements.m_openStyle);
				if (renderSort)
				{
					this.html5Renderer.WriteStream("margin-left:auto;");
				}
				if (renderToggle)
				{
					this.html5Renderer.WriteStream("margin-right:auto;");
				}
				this.html5Renderer.WriteStream(HTMLElements.m_verticalAlign);
				this.html5Renderer.WriteStream(EnumStrings.GetValue(verticalAlignments));
				this.html5Renderer.WriteStream(HTMLElements.m_quote);
				this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				if (renderSort)
				{
					this.html5Renderer.RenderSortImage(textBoxProps);
				}
				if (renderToggle)
				{
					this.html5Renderer.RenderToggleImage(textBoxProps);
				}
			}
			this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
		}

		private void RenderTextBoxContent(RPLTextBox textBox, RPLTextBoxProps tbProps, RPLTextBoxPropsDef tbDef, string textBoxValue, RPLStyleProps actionStyle, bool renderImages, RPLItemMeasurement measurement, RPLAction textBoxAction)
		{
			if (tbDef.IsSimple)
			{
				bool flag = false;
				object obj = null;
				bool flag2 = string.IsNullOrEmpty(textBoxValue);
				if (!flag2 && renderImages)
				{
					obj = tbProps.Style[24];
					if (obj != null && (RPLFormat.TextDecorations)obj != 0)
					{
						obj = EnumStrings.GetValue((RPLFormat.TextDecorations)obj);
						flag = true;
						this.html5Renderer.WriteStream(HTMLElements.m_openSpan);
						this.html5Renderer.WriteStream(HTMLElements.m_openStyle);
						this.html5Renderer.WriteStream(HTMLElements.m_textDecoration);
						this.html5Renderer.WriteStream(obj);
						this.html5Renderer.WriteStream(HTMLElements.m_closeQuote);
					}
				}
				if (HTML5Renderer.IsDirectionRTL(tbProps.Style))
				{
					this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
					this.html5Renderer.OpenStyle();
					this.html5Renderer.WriteStream(HTMLElements.m_direction);
					this.html5Renderer.WriteStream("rtl");
					this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
					this.html5Renderer.WriteStream(HTMLElements.m_styleWidth);
					this.html5Renderer.WriteStream(HTMLElements.m_percent);
					this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
					this.html5Renderer.CloseStyle(true);
					this.html5Renderer.WriteStream(HTMLElements.m_classStyle);
					this.html5Renderer.WriteStream(HTMLElements.m_rtlEmbed);
					this.html5Renderer.WriteStream(HTMLElements.m_quoteString);
					this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				}
				if (flag2)
				{
					if (!this.html5Renderer.NeedSharedToggleParent(tbProps))
					{
						this.html5Renderer.WriteStream(HTMLElements.m_nbsp);
					}
				}
				else
				{
					List<int> list = null;
					if (!string.IsNullOrEmpty(this.html5Renderer.m_searchText))
					{
						int startIndex = 0;
						int length = this.html5Renderer.m_searchText.Length;
						string text = textBoxValue;
						string text2 = this.html5Renderer.m_searchText;
						if (text2.IndexOf(' ') >= 0)
						{
							text2 = text2.Replace('\u00a0', ' ');
							text = text.Replace('\u00a0', ' ');
						}
						while ((startIndex = text.IndexOf(text2, startIndex, StringComparison.OrdinalIgnoreCase)) != -1)
						{
							if (list == null)
							{
								list = new List<int>(2);
							}
							list.Add(startIndex);
							startIndex += length;
						}
						if (list == null)
						{
							this.RenderMultiLineText(textBoxValue);
						}
						else
						{
							this.RenderMultiLineTextWithHits(textBoxValue, list);
						}
					}
					else
					{
						this.RenderMultiLineText(textBoxValue);
					}
				}
				if (HTML5Renderer.IsDirectionRTL(tbProps.Style))
				{
					this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
				}
				if (flag)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_closeSpan);
				}
			}
			else
			{
				this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
				this.html5Renderer.OpenStyle();
				RPLElementStyle style = tbProps.Style;
				bool flag3 = false;
				bool flag4 = HTML5Renderer.IsWritingModeVertical(style);
				if (flag4)
				{
					this.html5Renderer.RenderMeasurementHeight(measurement.Height);
				}
				if (HTML5Renderer.IsDirectionRTL(style))
				{
					this.html5Renderer.WriteStream(HTMLElements.m_direction);
					this.html5Renderer.WriteStream("rtl");
					this.html5Renderer.CloseStyle(true);
					flag3 = true;
					this.html5Renderer.WriteStream(HTMLElements.m_classStyle);
					this.html5Renderer.WriteStream(HTMLElements.m_rtlEmbed);
				}
				else
				{
					this.html5Renderer.CloseStyle(true);
				}
				if (textBoxAction != null)
				{
					if (!flag3)
					{
						flag3 = true;
						this.html5Renderer.WriteStream(HTMLElements.m_classStyle);
					}
					else
					{
						this.html5Renderer.WriteStream(HTMLElements.m_space);
					}
					this.html5Renderer.WriteStream(HTMLElements.m_styleAction);
				}
				if (flag3)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_quote);
				}
				this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				TextRunStyleWriter trsw = new TextRunStyleWriter(this.html5Renderer);
				HTML5ParagraphStyleWriter hTML5ParagraphStyleWriter = new HTML5ParagraphStyleWriter(this.html5Renderer, textBox);
				RPLStyleProps nonSharedStyle = tbProps.NonSharedStyle;
				if (nonSharedStyle != null && (nonSharedStyle[30] != null || nonSharedStyle[29] != null))
				{
					hTML5ParagraphStyleWriter.OutputSharedInNonShared = true;
				}
				RPLParagraph nextParagraph = textBox.GetNextParagraph();
				ListLevelStack listLevelStack = null;
				while (nextParagraph != null)
				{
					RPLParagraphProps rPLParagraphProps = nextParagraph.ElementProps as RPLParagraphProps;
					RPLParagraphPropsDef rPLParagraphPropsDef = rPLParagraphProps.Definition as RPLParagraphPropsDef;
					int num = rPLParagraphProps.ListLevel ?? rPLParagraphPropsDef.ListLevel;
					RPLFormat.ListStyles listStyles = rPLParagraphProps.ListStyle ?? rPLParagraphPropsDef.ListStyle;
					string text3 = null;
					RPLStyleProps nonSharedStyle2 = rPLParagraphProps.NonSharedStyle;
					RPLStyleProps shared = null;
					if (rPLParagraphPropsDef != null)
					{
						if (num == 0)
						{
							num = rPLParagraphPropsDef.ListLevel;
						}
						if (listStyles == RPLFormat.ListStyles.None)
						{
							listStyles = rPLParagraphPropsDef.ListStyle;
						}
						text3 = rPLParagraphPropsDef.ID;
						if (!hTML5ParagraphStyleWriter.OutputSharedInNonShared)
						{
							shared = rPLParagraphPropsDef.SharedStyle;
						}
					}
					hTML5ParagraphStyleWriter.Paragraph = nextParagraph;
					hTML5ParagraphStyleWriter.ParagraphMode = HTML5ParagraphStyleWriter.Mode.All;
					hTML5ParagraphStyleWriter.CurrentListLevel = num;
					byte[] array = null;
					if (num > 0)
					{
						if (listLevelStack == null)
						{
							listLevelStack = new ListLevelStack();
						}
						bool writeNoVerticalMargin = !flag4;
						listLevelStack.PushTo(this.html5Renderer, num, listStyles, writeNoVerticalMargin);
						if (listStyles != 0)
						{
							this.html5Renderer.WriteStream(HTMLElements.m_openLi);
							hTML5ParagraphStyleWriter.ParagraphMode = HTML5ParagraphStyleWriter.Mode.ListOnly;
							this.html5Renderer.WriteStyles(text3 + "l", nonSharedStyle2, shared, hTML5ParagraphStyleWriter);
							this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
							array = HTMLElements.m_closeLi;
							hTML5ParagraphStyleWriter.ParagraphMode = HTML5ParagraphStyleWriter.Mode.ParagraphOnly;
							text3 += "p";
						}
					}
					else if (listLevelStack != null)
					{
						listLevelStack.PopAll();
						listLevelStack = null;
					}
					this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
					this.html5Renderer.WriteStyles(text3, nonSharedStyle2, shared, hTML5ParagraphStyleWriter);
					this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
					RPLReportSize hangingIndent = rPLParagraphProps.HangingIndent;
					if (hangingIndent == null)
					{
						hangingIndent = rPLParagraphPropsDef.HangingIndent;
					}
					float num2 = 0f;
					if (hangingIndent != null)
					{
						num2 = (float)hangingIndent.ToMillimeters();
					}
					if (num2 > 0.0)
					{
						this.html5Renderer.WriteStream(HTMLElements.m_openSpan);
						this.html5Renderer.OpenStyle();
						this.html5Renderer.RenderMeasurementWidth(num2, true);
						this.html5Renderer.WriteStream(HTMLElements.m_styleDisplayInlineBlock);
						this.html5Renderer.CloseStyle(true);
						this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
						if (this.html5Renderer.m_deviceInfo.IsBrowserGeckoEngine)
						{
							this.html5Renderer.WriteStream(HTMLElements.m_nbsp);
						}
						this.html5Renderer.WriteStream(HTMLElements.m_closeSpan);
					}
					this.RenderTextRuns(nextParagraph, trsw, textBoxAction);
					this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
					if (array != null)
					{
						this.html5Renderer.WriteStream(array);
					}
					nextParagraph = textBox.GetNextParagraph();
				}
				if (listLevelStack != null)
				{
					listLevelStack.PopAll();
				}
				this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
			}
		}

		private void RenderTextRuns(RPLParagraph paragraph, TextRunStyleWriter trsw, RPLAction textBoxAction)
		{
			int num = 0;
			RPLTextRun rPLTextRun = null;
			if (!string.IsNullOrEmpty(this.html5Renderer.m_searchText))
			{
				RPLTextRun nextTextRun = paragraph.GetNextTextRun();
				rPLTextRun = nextTextRun;
				List<RPLTextRun> list = new List<RPLTextRun>();
				StringBuilder stringBuilder = new StringBuilder();
				while (nextTextRun != null)
				{
					list.Add(nextTextRun);
					string value = (nextTextRun.ElementProps as RPLTextRunProps).Value;
					if (string.IsNullOrEmpty(value))
					{
						value = (nextTextRun.ElementPropsDef as RPLTextRunPropsDef).Value;
					}
					stringBuilder.Append(value);
					nextTextRun = paragraph.GetNextTextRun();
				}
				string text = stringBuilder.ToString();
				string text2 = this.html5Renderer.m_searchText;
				if (text2.IndexOf(' ') >= 0)
				{
					text2 = text2.Replace('\u00a0', ' ');
					text = text.Replace('\u00a0', ' ');
				}
				int num2 = text.IndexOf(text2, StringComparison.OrdinalIgnoreCase);
				List<int> list2 = new List<int>();
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int length = this.html5Renderer.m_searchText.Length;
				for (int i = 0; i < list.Count; i++)
				{
					nextTextRun = list[i];
					string value2 = (nextTextRun.ElementProps as RPLTextRunProps).Value;
					if (string.IsNullOrEmpty(value2))
					{
						value2 = (nextTextRun.ElementPropsDef as RPLTextRunPropsDef).Value;
					}
					if (!string.IsNullOrEmpty(value2))
					{
						while (num2 > -1 && num2 < num3 + value2.Length)
						{
							list2.Add(num2 - num3);
							num2 = text.IndexOf(text2, num2 + length, StringComparison.OrdinalIgnoreCase);
						}
						if (list2.Count > 0 || num4 > 0)
						{
							num += this.RenderTextRunFindString(nextTextRun, list2, num4, ref num5, trsw, textBoxAction);
							if (num4 > 0)
							{
								num4 -= value2.Length;
								if (num4 < 0)
								{
									num4 = 0;
								}
							}
							if (list2.Count > 0)
							{
								int num6 = list2[list2.Count - 1];
								list2.Clear();
								if (value2.Length < num6 + length)
								{
									num4 = length - (value2.Length - num6);
								}
							}
						}
						else
						{
							num += this.RenderTextRun(nextTextRun, trsw, textBoxAction);
						}
						num3 += value2.Length;
					}
				}
			}
			else
			{
				RPLTextRun nextTextRun2 = paragraph.GetNextTextRun();
				rPLTextRun = nextTextRun2;
				while (nextTextRun2 != null)
				{
					num += this.RenderTextRun(nextTextRun2, trsw, textBoxAction);
					nextTextRun2 = paragraph.GetNextTextRun();
				}
			}
			if (num == 0 && rPLTextRun != null)
			{
				RPLTextRunProps rPLTextRunProps = rPLTextRun.ElementProps as RPLTextRunProps;
				RPLElementPropsDef definition = rPLTextRunProps.Definition;
				this.html5Renderer.WriteStream(HTMLElements.m_openSpan);
				this.html5Renderer.WriteStyles(definition.ID, rPLTextRunProps.NonSharedStyle, definition.SharedStyle, trsw);
				this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				this.html5Renderer.WriteStream(HTMLElements.m_nbsp);
				this.html5Renderer.WriteStream(HTMLElements.m_closeSpan);
			}
		}

		private int RenderTextRunFindString(RPLTextRun textRun, List<int> hits, int remainingChars, ref int runOffsetCount, TextRunStyleWriter trsw, RPLAction textBoxAction)
		{
			RPLTextRunProps rPLTextRunProps = textRun.ElementProps as RPLTextRunProps;
			RPLTextRunPropsDef rPLTextRunPropsDef = rPLTextRunProps.Definition as RPLTextRunPropsDef;
			RPLStyleProps shared = null;
			string id = null;
			string value = rPLTextRunProps.Value;
			string toolTip = rPLTextRunProps.ToolTip;
			if (rPLTextRunPropsDef != null)
			{
				shared = rPLTextRunPropsDef.SharedStyle;
				id = rPLTextRunPropsDef.ID;
				if (string.IsNullOrEmpty(value))
				{
					value = rPLTextRunPropsDef.Value;
				}
				if (string.IsNullOrEmpty(toolTip))
				{
					toolTip = rPLTextRunPropsDef.ToolTip;
				}
			}
			if (string.IsNullOrEmpty(value))
			{
				return 0;
			}
			byte[] theBytes = HTMLElements.m_closeSpan;
			RPLAction rPLAction = null;
			if (textBoxAction == null && this.html5Renderer.HasAction(rPLTextRunProps.ActionInfo))
			{
				rPLAction = rPLTextRunProps.ActionInfo.Actions[0];
			}
			if (rPLAction != null)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_openA);
				this.html5Renderer.RenderTabIndex();
				this.html5Renderer.RenderActionHref(rPLAction, RPLFormat.TextDecorations.Underline, null);
				theBytes = HTMLElements.m_closeA;
			}
			else
			{
				this.html5Renderer.WriteStream(HTMLElements.m_openSpan);
			}
			if (toolTip != null)
			{
				this.html5Renderer.WriteToolTipAttribute(toolTip);
			}
			this.html5Renderer.WriteStyles(id, rPLTextRunProps.NonSharedStyle, shared, trsw);
			this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			int num = 0;
			int num2 = 0;
			int length = value.Length;
			if (remainingChars > 0)
			{
				int num3 = remainingChars;
				if (num3 > length)
				{
					num3 = length;
				}
				if (num3 > 0)
				{
					this.OutputFindString(value.Substring(0, num3), runOffsetCount++);
					num += num3;
					if (num3 >= remainingChars)
					{
						this.html5Renderer.m_currentHitCount++;
						runOffsetCount = 0;
					}
				}
			}
			int num4 = hits.Count - 1;
			bool flag = false;
			int length2 = this.html5Renderer.m_searchText.Length;
			if (hits.Count > 0)
			{
				num2 = hits[hits.Count - 1];
				if (num2 + length2 > length)
				{
					flag = true;
				}
				else
				{
					num4 = hits.Count;
				}
			}
			for (int i = 0; i < num4; i++)
			{
				num2 = hits[i];
				if (num < num2)
				{
					this.RenderMultiLineText(value.Substring(num, num2 - num));
				}
				this.OutputFindString(value.Substring(num2, length2), 0);
				this.html5Renderer.m_currentHitCount++;
				runOffsetCount = 0;
				num = num2 + length2;
			}
			if (flag)
			{
				num2 = hits[hits.Count - 1];
				if (num < num2)
				{
					this.RenderMultiLineText(value.Substring(num, num2 - num));
				}
				this.OutputFindString(value.Substring(num2, length - num2), runOffsetCount++);
			}
			else if (num < length)
			{
				this.RenderMultiLineText(value.Substring(num));
			}
			this.html5Renderer.WriteStream(theBytes);
			return length;
		}

		private int RenderTextRun(RPLTextRun textRun, TextRunStyleWriter trsw, RPLAction textBoxAction)
		{
			RPLTextRunProps rPLTextRunProps = textRun.ElementProps as RPLTextRunProps;
			RPLTextRunPropsDef rPLTextRunPropsDef = rPLTextRunProps.Definition as RPLTextRunPropsDef;
			RPLStyleProps shared = null;
			string id = null;
			string value = rPLTextRunProps.Value;
			string toolTip = rPLTextRunProps.ToolTip;
			if (rPLTextRunPropsDef != null)
			{
				shared = rPLTextRunPropsDef.SharedStyle;
				id = rPLTextRunPropsDef.ID;
				if (string.IsNullOrEmpty(value))
				{
					value = rPLTextRunPropsDef.Value;
				}
				if (string.IsNullOrEmpty(toolTip))
				{
					toolTip = rPLTextRunPropsDef.ToolTip;
				}
			}
			if (string.IsNullOrEmpty(value))
			{
				return 0;
			}
			byte[] theBytes = HTMLElements.m_closeSpan;
			RPLAction rPLAction = null;
			if (textBoxAction == null)
			{
				rPLAction = textBoxAction;
				if (this.html5Renderer.HasAction(rPLTextRunProps.ActionInfo))
				{
					rPLAction = rPLTextRunProps.ActionInfo.Actions[0];
				}
			}
			if (rPLAction != null)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_openA);
				this.html5Renderer.RenderTabIndex();
				this.html5Renderer.RenderActionHref(rPLAction, RPLFormat.TextDecorations.Underline, null);
				theBytes = HTMLElements.m_closeA;
			}
			else
			{
				this.html5Renderer.WriteStream(HTMLElements.m_openSpan);
			}
			if (toolTip != null)
			{
				this.html5Renderer.WriteToolTipAttribute(toolTip);
			}
			this.html5Renderer.WriteStyles(id, rPLTextRunProps.NonSharedStyle, shared, trsw);
			this.html5Renderer.RenderLanguage(rPLTextRunProps.Style[32] as string);
			this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			this.RenderMultiLineText(value);
			this.html5Renderer.WriteStream(theBytes);
			return value.Length;
		}

		protected void RenderMultiLineTextWithHits(string text, List<int> hits)
		{
			if (text != null)
			{
				int num = 0;
				int startPos = 0;
				int num2 = 0;
				int length = text.Length;
				for (int i = 0; i < length; i++)
				{
					switch (text[i])
					{
					case '\r':
						this.RenderTextWithHits(text, startPos, num, hits, ref num2);
						startPos = num + 1;
						break;
					case '\n':
						this.RenderTextWithHits(text, startPos, num, hits, ref num2);
						this.html5Renderer.WriteStreamCR(HTMLElements.m_br);
						startPos = num + 1;
						break;
					}
					num++;
				}
				this.RenderTextWithHits(text, startPos, num, hits, ref num2);
			}
		}

		protected void RenderTextWithHits(string text, int startPos, int endPos, List<int> hitIndices, ref int currentHitIndex)
		{
			int length = this.html5Renderer.m_searchText.Length;
			while (currentHitIndex < hitIndices.Count && hitIndices[currentHitIndex] < endPos)
			{
				int num = hitIndices[currentHitIndex];
				string theString = text.Substring(startPos, num - startPos);
				this.html5Renderer.WriteStreamEncoded(theString);
				theString = text.Substring(num, length);
				this.OutputFindString(theString, 0);
				startPos = num + length;
				currentHitIndex++;
				this.html5Renderer.m_currentHitCount++;
			}
			if (startPos <= endPos)
			{
				string theString = text.Substring(startPos, endPos - startPos);
				this.html5Renderer.WriteStreamEncoded(theString);
			}
		}

		private void OutputFindString(string findString, int offset)
		{
			this.html5Renderer.WriteStream(HTMLElements.m_openSpan);
			this.html5Renderer.WriteStream(HTMLElements.m_id);
			this.html5Renderer.WriteAttrEncoded(this.html5Renderer.m_deviceInfo.HtmlPrefixId);
			this.html5Renderer.WriteStream(HTML5Renderer.m_searchHitIdPrefix);
			this.html5Renderer.WriteStream(this.html5Renderer.m_currentHitCount.ToString(CultureInfo.InvariantCulture));
			if (offset > 0)
			{
				this.html5Renderer.WriteStream("_");
				this.html5Renderer.WriteStream(offset.ToString(CultureInfo.InvariantCulture));
			}
			this.html5Renderer.WriteStream(HTMLElements.m_quote);
			if (this.html5Renderer.m_currentHitCount == 0)
			{
				this.html5Renderer.WriteStream(" class=\"searchHighlighting\">");
			}
			else
			{
				this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			}
			this.html5Renderer.WriteStreamEncoded(findString);
			this.html5Renderer.WriteStream(HTMLElements.m_closeSpan);
		}

		private void RenderMultiLineText(string text)
		{
			if (text != null)
			{
				int num = 0;
				int num2 = 0;
				int length = text.Length;
				string text2 = null;
				for (int i = 0; i < length; i++)
				{
					switch (text[i])
					{
					case '\r':
						text2 = text.Substring(num2, num - num2);
						this.html5Renderer.WriteStreamEncoded(text2);
						num2 = num + 1;
						break;
					case '\n':
						text2 = text.Substring(num2, num - num2);
						if (!string.IsNullOrEmpty(text2.Trim()))
						{
							this.html5Renderer.WriteStreamEncoded(text2);
						}
						this.html5Renderer.WriteStreamCR(HTMLElements.m_br);
						num2 = num + 1;
						break;
					}
					num++;
				}
				if (num2 == 0)
				{
					this.html5Renderer.WriteStreamEncoded(text);
				}
				else
				{
					this.html5Renderer.WriteStreamEncoded(text.Substring(num2, num - num2));
				}
			}
		}

		private void CreateFitVertTextIdsStream()
		{
			string streamName = HTML5Renderer.GetStreamName(this.html5Renderer.m_rplReport.ReportName, this.html5Renderer.m_pageNum, "_fvt");
			Stream stream = this.html5Renderer.CreateStream(streamName, "txt", Encoding.UTF8, "text/plain", true, AspNetCore.ReportingServices.Interfaces.StreamOper.CreateOnly);
			this.html5Renderer.m_fitVertTextIdsStream = new BufferedStream(stream);
			this.html5Renderer.m_needsFitVertTextScript = true;
		}
	}
}
