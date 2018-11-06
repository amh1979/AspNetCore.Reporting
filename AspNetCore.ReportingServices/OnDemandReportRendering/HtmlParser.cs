using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class HtmlParser : RichTextParser
	{
		internal sealed class Constants
		{
			internal const string HtmlSize = "size";

			internal const string HtmlColor = "color";

			internal const string HtmlAlign = "align";

			internal const string CssFontSize = "font-size";

			internal const string CssFontStyle = "font-style";

			internal const string CssFontWeight = "font-weight";

			internal const string CssTextAlign = "text-align";

			internal const string CssTextIndent = "text-indent";

			internal const string CssPadding = "padding";

			internal const string CssColor = "color";

			internal const char NonBreakingSpace = '\u00a0';
		}

		internal static class StyleDefaults
		{
			internal static ReportSize H1FontSize = new ReportSize("24pt");

			internal static ReportSize H2FontSize = new ReportSize("18pt");

			internal static ReportSize H3FontSize = new ReportSize("14pt");

			internal static ReportSize H4FontSize = new ReportSize("12pt");

			internal static ReportSize H5FontSize = new ReportSize("10pt");

			internal static ReportSize H6FontSize = new ReportSize("8pt");

			internal static ReportSize PFontSize = new ReportSize("10pt");

			internal static ReportSize H1Margin = StyleDefaults.H1FontSize;

			internal static ReportSize H2Margin = StyleDefaults.H2FontSize;

			internal static ReportSize H3Margin = StyleDefaults.H3FontSize;

			internal static ReportSize H4Margin = StyleDefaults.H4FontSize;

			internal static ReportSize H5Margin = StyleDefaults.H5FontSize;

			internal static ReportSize H6Margin = StyleDefaults.H6FontSize;

			internal static ReportSize PMargin = StyleDefaults.PFontSize;
		}

		private HtmlElement m_currentHtmlElement;

		private string m_currentHyperlinkText;

		private HtmlLexer m_htmlLexer;

		internal HtmlParser(bool multipleParagraphsAllowed, IRichTextInstanceCreator iRichTextInstanceCreator, IRichTextLogger richTextLogger)
			: base(multipleParagraphsAllowed, iRichTextInstanceCreator, richTextLogger)
		{
		}

		private string HtmlTrimStart(string input)
		{
			for (int i = 0; i < input.Length; i++)
			{
				char c = input[i];
				if (!char.IsWhiteSpace(c) || c == '\u00a0')
				{
					return input.Substring(i);
				}
			}
			return string.Empty;
		}

		protected override void InternalParse(string richText)
		{
			this.m_htmlLexer = new HtmlLexer(richText);
			int num = 0;
			FunctionalList<ListStyle> functionalList = FunctionalList<ListStyle>.Empty;
			HtmlElement.HtmlNodeType htmlNodeType = HtmlElement.HtmlNodeType.Element;
			HtmlElement.HtmlNodeType htmlNodeType2 = HtmlElement.HtmlNodeType.Element;
			HtmlElement.HtmlElementType htmlElementType = HtmlElement.HtmlElementType.None;
			HtmlElement.HtmlElementType htmlElementType2 = HtmlElement.HtmlElementType.None;
			while (this.m_htmlLexer.Read())
			{
				this.m_currentHtmlElement = this.m_htmlLexer.CurrentElement;
				htmlElementType2 = this.m_currentHtmlElement.ElementType;
				htmlNodeType2 = this.m_currentHtmlElement.NodeType;
				switch (htmlNodeType2)
				{
				case HtmlElement.HtmlNodeType.Element:
					if (num != 0 && htmlElementType2 != HtmlElement.HtmlElementType.TITLE)
					{
						break;
					}
					switch (htmlElementType2)
					{
					case HtmlElement.HtmlElementType.TITLE:
						if (!this.m_currentHtmlElement.IsEmptyElement)
						{
							num++;
						}
						htmlElementType2 = htmlElementType;
						htmlNodeType2 = htmlNodeType;
						break;
					case HtmlElement.HtmlElementType.P:
					case HtmlElement.HtmlElementType.DIV:
					case HtmlElement.HtmlElementType.LI:
					case HtmlElement.HtmlElementType.H1:
					case HtmlElement.HtmlElementType.H2:
					case HtmlElement.HtmlElementType.H3:
					case HtmlElement.HtmlElementType.H4:
					case HtmlElement.HtmlElementType.H5:
					case HtmlElement.HtmlElementType.H6:
						this.ParseParagraphElement(htmlElementType2, functionalList);
						break;
					case HtmlElement.HtmlElementType.UL:
					case HtmlElement.HtmlElementType.OL:
					{
						this.FlushPendingLI();
						this.CloseParagraph();
						ListStyle listStyleForElement2 = this.GetListStyleForElement(htmlElementType2);
						functionalList = functionalList.Add(listStyleForElement2);
						base.m_currentParagraph.ListLevel = functionalList.Count;
						break;
					}
					case HtmlElement.HtmlElementType.SPAN:
					case HtmlElement.HtmlElementType.FONT:
					case HtmlElement.HtmlElementType.STRONG:
					case HtmlElement.HtmlElementType.STRIKE:
					case HtmlElement.HtmlElementType.B:
					case HtmlElement.HtmlElementType.I:
					case HtmlElement.HtmlElementType.U:
					case HtmlElement.HtmlElementType.S:
					case HtmlElement.HtmlElementType.EM:
						this.ParseTextRunElement(htmlElementType2);
						break;
					case HtmlElement.HtmlElementType.A:
						this.ParseActionElement(functionalList.Count);
						break;
					case HtmlElement.HtmlElementType.BR:
						if (htmlNodeType != HtmlElement.HtmlNodeType.EndElement)
						{
							this.AppendText(Environment.NewLine);
						}
						else
						{
							this.SetTextRunValue(Environment.NewLine);
						}
						break;
					default:
						htmlElementType2 = htmlElementType;
						htmlNodeType2 = htmlNodeType;
						break;
					}
					break;
				case HtmlElement.HtmlNodeType.Text:
					if (num == 0)
					{
						string text = this.m_currentHtmlElement.Value;
						if (htmlNodeType == HtmlElement.HtmlNodeType.Text)
						{
							this.AppendText(text);
						}
						else if (htmlElementType == HtmlElement.HtmlElementType.BR)
						{
							this.AppendText(this.HtmlTrimStart(text));
						}
						else
						{
							if (base.m_currentParagraphInstance == null)
							{
								text = this.HtmlTrimStart(text);
							}
							if (!string.IsNullOrEmpty(text))
							{
								this.SetTextRunValue(text);
							}
							else
							{
								htmlElementType2 = htmlElementType;
								htmlNodeType2 = htmlNodeType;
							}
						}
					}
					break;
				case HtmlElement.HtmlNodeType.EndElement:
					if (num != 0 && htmlElementType2 != HtmlElement.HtmlElementType.TITLE)
					{
						break;
					}
					switch (htmlElementType2)
					{
					case HtmlElement.HtmlElementType.TITLE:
						if (num > 0)
						{
							num--;
						}
						htmlElementType2 = htmlElementType;
						htmlNodeType2 = htmlNodeType;
						break;
					case HtmlElement.HtmlElementType.UL:
					case HtmlElement.HtmlElementType.OL:
						this.FlushPendingLI();
						this.CloseParagraph();
						if (functionalList.Count > 0)
						{
							ListStyle listStyleForElement = this.GetListStyleForElement(htmlElementType2);
							bool flag = false;
							FunctionalList<ListStyle> functionalList2 = functionalList;
							do
							{
								flag = (functionalList2.First == listStyleForElement);
								functionalList2 = functionalList2.Rest;
							}
							while (!flag && functionalList2.Count > 0);
							if (flag)
							{
								functionalList = functionalList2;
								base.m_currentParagraph.ListLevel = functionalList.Count;
							}
						}
						break;
					case HtmlElement.HtmlElementType.LI:
						this.CloseParagraph();
						break;
					case HtmlElement.HtmlElementType.P:
					case HtmlElement.HtmlElementType.DIV:
					case HtmlElement.HtmlElementType.H1:
					case HtmlElement.HtmlElementType.H2:
					case HtmlElement.HtmlElementType.H3:
					case HtmlElement.HtmlElementType.H4:
					case HtmlElement.HtmlElementType.H5:
					case HtmlElement.HtmlElementType.H6:
						this.CloseParagraph();
						base.m_currentParagraph = base.m_currentParagraph.RemoveParagraph(htmlElementType2);
						goto case HtmlElement.HtmlElementType.SPAN;
					case HtmlElement.HtmlElementType.SPAN:
					case HtmlElement.HtmlElementType.FONT:
					case HtmlElement.HtmlElementType.STRONG:
					case HtmlElement.HtmlElementType.STRIKE:
					case HtmlElement.HtmlElementType.B:
					case HtmlElement.HtmlElementType.I:
					case HtmlElement.HtmlElementType.U:
					case HtmlElement.HtmlElementType.S:
					case HtmlElement.HtmlElementType.EM:
						base.m_currentStyle = base.m_currentStyle.RemoveStyle(htmlElementType2);
						break;
					case HtmlElement.HtmlElementType.A:
						this.RevertActionElement(htmlElementType2);
						break;
					default:
						htmlElementType2 = htmlElementType;
						htmlNodeType2 = htmlNodeType;
						break;
					}
					break;
				}
				htmlNodeType = htmlNodeType2;
				htmlElementType = htmlElementType2;
			}
			if (base.m_paragraphInstanceCollection.Count == 0)
			{
				this.CreateTextRunInstance();
			}
			base.m_currentParagraph = base.m_currentParagraph.RemoveAll();
		}

		private ListStyle GetListStyleForElement(HtmlElement.HtmlElementType elementType)
		{
			if (elementType == HtmlElement.HtmlElementType.OL)
			{
				return ListStyle.Numbered;
			}
			return ListStyle.Bulleted;
		}

		private void ParseParagraphElement(HtmlElement.HtmlElementType elementType, FunctionalList<ListStyle> listStyles)
		{
			this.CloseParagraph();
			if (base.m_currentParagraph.ElementType == HtmlElement.HtmlElementType.P)
			{
				base.m_currentParagraph = base.m_currentParagraph.RemoveParagraph(HtmlElement.HtmlElementType.P);
				base.m_currentStyle = base.m_currentStyle.RemoveStyle(HtmlElement.HtmlElementType.P);
			}
			if (elementType == HtmlElement.HtmlElementType.LI)
			{
				this.FlushPendingLI();
				if (listStyles.Count > 0)
				{
					base.m_currentParagraph.ListStyle = listStyles.First;
				}
				else
				{
					base.m_currentParagraph.ListStyle = ListStyle.Bulleted;
				}
			}
			else
			{
				base.m_currentStyle = base.m_currentStyle.CreateChildStyle(elementType);
				base.m_currentParagraph = base.m_currentParagraph.CreateChildParagraph(elementType);
				switch (elementType)
				{
				case HtmlElement.HtmlElementType.H1:
					base.m_currentStyle.FontSize = StyleDefaults.H1FontSize;
					base.m_currentStyle.FontWeight = FontWeights.Bold;
					this.SetMarginTopAndBottom(StyleDefaults.H1Margin);
					break;
				case HtmlElement.HtmlElementType.H2:
					base.m_currentStyle.FontSize = StyleDefaults.H2FontSize;
					base.m_currentStyle.FontWeight = FontWeights.Bold;
					this.SetMarginTopAndBottom(StyleDefaults.H2Margin);
					break;
				case HtmlElement.HtmlElementType.H3:
					base.m_currentStyle.FontSize = StyleDefaults.H3FontSize;
					base.m_currentStyle.FontWeight = FontWeights.Bold;
					this.SetMarginTopAndBottom(StyleDefaults.H3Margin);
					break;
				case HtmlElement.HtmlElementType.H4:
					base.m_currentStyle.FontSize = StyleDefaults.H4FontSize;
					base.m_currentStyle.FontWeight = FontWeights.Bold;
					this.SetMarginTopAndBottom(StyleDefaults.H4Margin);
					break;
				case HtmlElement.HtmlElementType.H5:
					base.m_currentStyle.FontSize = StyleDefaults.H5FontSize;
					base.m_currentStyle.FontWeight = FontWeights.Bold;
					this.SetMarginTopAndBottom(StyleDefaults.H5Margin);
					break;
				case HtmlElement.HtmlElementType.H6:
					base.m_currentStyle.FontSize = StyleDefaults.H6FontSize;
					base.m_currentStyle.FontWeight = FontWeights.Bold;
					this.SetMarginTopAndBottom(StyleDefaults.H6Margin);
					break;
				case HtmlElement.HtmlElementType.P:
					this.SetMarginTopAndBottom(StyleDefaults.PMargin);
					break;
				}
				string text = default(string);
				if (!this.m_currentHtmlElement.IsEmptyElement && this.m_currentHtmlElement.HasAttributes && base.m_allowMultipleParagraphs && this.m_currentHtmlElement.Attributes.TryGetValue("align", out text))
				{
					TextAlignments textAlign = default(TextAlignments);
					if (RichTextStyleTranslator.TranslateTextAlign(text, out textAlign))
					{
						base.m_currentStyle.TextAlign = textAlign;
					}
					else
					{
						base.m_richTextLogger.RegisterInvalidValueWarning("align", text, this.m_currentHtmlElement.CharacterPosition);
					}
				}
			}
			this.SetStyleValues(true);
		}

		private void FlushPendingLI()
		{
			if (base.m_allowMultipleParagraphs && base.m_currentParagraph.ListStyle != 0)
			{
				this.CreateParagraphInstance();
				this.CloseParagraph();
			}
		}

		private void SetMarginTopAndBottom(ReportSize marginValue)
		{
			base.m_currentParagraph.UpdateMarginTop(marginValue);
			base.m_currentParagraph.AddMarginBottom(marginValue);
		}

		private void ParseTextRunElement(HtmlElement.HtmlElementType elementType)
		{
			base.m_currentStyle = base.m_currentStyle.CreateChildStyle(elementType);
			bool flag = false;
			switch (this.m_currentHtmlElement.ElementType)
			{
			case HtmlElement.HtmlElementType.I:
			case HtmlElement.HtmlElementType.EM:
				base.m_currentStyle.FontStyle = FontStyles.Italic;
				break;
			case HtmlElement.HtmlElementType.U:
				base.m_currentStyle.TextDecoration = TextDecorations.Underline;
				break;
			case HtmlElement.HtmlElementType.STRONG:
			case HtmlElement.HtmlElementType.B:
				base.m_currentStyle.FontWeight = FontWeights.Bold;
				break;
			case HtmlElement.HtmlElementType.STRIKE:
			case HtmlElement.HtmlElementType.S:
				base.m_currentStyle.TextDecoration = TextDecorations.LineThrough;
				break;
			case HtmlElement.HtmlElementType.SPAN:
			case HtmlElement.HtmlElementType.FONT:
				flag = true;
				break;
			}
			if (flag && !this.m_currentHtmlElement.IsEmptyElement && this.m_currentHtmlElement.HasAttributes)
			{
				if (this.m_currentHtmlElement.ElementType == HtmlElement.HtmlElementType.FONT)
				{
					string text = default(string);
					if (this.m_currentHtmlElement.Attributes.TryGetValue("size", out text))
					{
						string size = default(string);
						if (RichTextStyleTranslator.TranslateHtmlFontSize(text, out size))
						{
							base.m_currentStyle.FontSize = new ReportSize(size);
						}
						else
						{
							base.m_richTextLogger.RegisterInvalidSizeWarning("size", text, this.m_currentHtmlElement.CharacterPosition);
						}
					}
					if (this.m_currentHtmlElement.Attributes.TryGetValue("face", out text))
					{
						base.m_currentStyle.FontFamily = text;
					}
					if (this.m_currentHtmlElement.Attributes.TryGetValue("color", out text))
					{
						ReportColor color = default(ReportColor);
						if (ReportColor.TryParse(RichTextStyleTranslator.TranslateHtmlColor(text), out color))
						{
							base.m_currentStyle.Color = color;
						}
						else
						{
							base.m_richTextLogger.RegisterInvalidColorWarning("color", text, this.m_currentHtmlElement.CharacterPosition);
						}
					}
				}
				else if (this.m_currentHtmlElement.ElementType == HtmlElement.HtmlElementType.SPAN)
				{
					this.SetStyleValues(false);
				}
			}
		}

		private void RevertActionElement(HtmlElement.HtmlElementType elementType)
		{
			if (this.m_currentHyperlinkText != null)
			{
				this.m_currentHyperlinkText = null;
				base.m_currentStyle = base.m_currentStyle.RemoveStyle(elementType);
			}
		}

		private void ParseActionElement(int listLevel)
		{
			this.RevertActionElement(HtmlElement.HtmlElementType.A);
			string text = default(string);
			if (!this.m_currentHtmlElement.IsEmptyElement && this.m_currentHtmlElement.HasAttributes && this.m_currentHtmlElement.Attributes.TryGetValue("href", out text))
			{
				IActionInstance actionInstance = base.m_IRichTextInstanceCreator.CreateActionInstance();
				actionInstance.SetHyperlinkText(text);
				if (actionInstance.HyperlinkText != null)
				{
					base.m_currentStyle = base.m_currentStyle.CreateChildStyle(HtmlElement.HtmlElementType.A);
					base.m_currentStyle.Color = new ReportColor("Blue");
					base.m_currentStyle.TextDecoration = TextDecorations.Underline;
					this.m_currentHyperlinkText = text;
				}
			}
		}

		protected override ICompiledTextRunInstance CreateTextRunInstance()
		{
			ICompiledTextRunInstance compiledTextRunInstance = base.CreateTextRunInstance();
			compiledTextRunInstance.MarkupType = MarkupType.HTML;
			if (this.m_currentHyperlinkText != null)
			{
				IActionInstance actionInstance = base.m_IRichTextInstanceCreator.CreateActionInstance();
				actionInstance.SetHyperlinkText(this.m_currentHyperlinkText);
				compiledTextRunInstance.ActionInstance = actionInstance;
			}
			return compiledTextRunInstance;
		}

		private void SetStyleValues(bool isParagraph)
		{
			if (this.m_currentHtmlElement.CssStyle != null)
			{
				string text = default(string);
				ReportSize reportSize = default(ReportSize);
				if (isParagraph && base.m_allowMultipleParagraphs)
				{
					if (this.m_currentHtmlElement.CssStyle.TryGetValue("text-align", out text))
					{
						TextAlignments textAlign = default(TextAlignments);
						if (RichTextStyleTranslator.TranslateTextAlign(text, out textAlign))
						{
							base.m_currentStyle.TextAlign = textAlign;
						}
						else
						{
							base.m_richTextLogger.RegisterInvalidValueWarning("text-align", text, this.m_currentHtmlElement.CharacterPosition);
						}
					}
					if (this.m_currentHtmlElement.CssStyle.TryGetValue("text-indent", out text))
					{
						if (ReportSize.TryParse(text, true, out reportSize))
						{
							base.m_currentParagraph.HangingIndent = reportSize;
						}
						else
						{
							base.m_richTextLogger.RegisterInvalidSizeWarning("text-indent", text, this.m_currentHtmlElement.CharacterPosition);
						}
					}
					ReportSize generalPadding = null;
					if (this.m_currentHtmlElement.CssStyle.TryGetValue("padding", out text))
					{
						if (ReportSize.TryParse(text, out reportSize))
						{
							generalPadding = reportSize;
						}
						else
						{
							base.m_richTextLogger.RegisterInvalidSizeWarning("padding", text, this.m_currentHtmlElement.CharacterPosition);
						}
					}
					ReportSize size = default(ReportSize);
					if (this.HasPaddingValue("padding-top", generalPadding, out size))
					{
						base.m_currentParagraph.AddSpaceBefore(size);
					}
					if (this.HasPaddingValue("padding-bottom", generalPadding, out size))
					{
						base.m_currentParagraph.AddSpaceAfter(size);
					}
					if (this.HasPaddingValue("padding-left", generalPadding, out size))
					{
						base.m_currentParagraph.AddLeftIndent(size);
					}
					if (this.HasPaddingValue("padding-right", generalPadding, out size))
					{
						base.m_currentParagraph.AddRightIndent(size);
					}
				}
				if (this.m_currentHtmlElement.CssStyle.TryGetValue("font-family", out text))
				{
					base.m_currentStyle.FontFamily = text;
				}
				if (this.m_currentHtmlElement.CssStyle.TryGetValue("font-size", out text))
				{
					if (ReportSize.TryParse(text, out reportSize))
					{
						base.m_currentStyle.FontSize = reportSize;
					}
					else
					{
						base.m_richTextLogger.RegisterInvalidSizeWarning("font-size", text, this.m_currentHtmlElement.CharacterPosition);
					}
				}
				if (this.m_currentHtmlElement.CssStyle.TryGetValue("font-weight", out text))
				{
					FontWeights fontWeight = default(FontWeights);
					if (RichTextStyleTranslator.TranslateFontWeight(text, out fontWeight))
					{
						base.m_currentStyle.FontWeight = fontWeight;
					}
					else
					{
						base.m_richTextLogger.RegisterInvalidValueWarning("font-weight", text, this.m_currentHtmlElement.CharacterPosition);
					}
				}
				if (this.m_currentHtmlElement.CssStyle.TryGetValue("color", out text))
				{
					ReportColor color = default(ReportColor);
					if (ReportColor.TryParse(RichTextStyleTranslator.TranslateHtmlColor(text), out color))
					{
						base.m_currentStyle.Color = color;
					}
					else
					{
						base.m_richTextLogger.RegisterInvalidColorWarning("color", text, this.m_currentHtmlElement.CharacterPosition);
					}
				}
			}
		}

		private bool HasPaddingValue(string attrName, ReportSize generalPadding, out ReportSize effectivePadding)
		{
			string value = default(string);
			if (this.m_currentHtmlElement.CssStyle.TryGetValue(attrName, out value))
			{
				ReportSize reportSize = default(ReportSize);
				if (ReportSize.TryParse(value, out reportSize))
				{
					effectivePadding = reportSize;
					return true;
				}
				base.m_richTextLogger.RegisterInvalidSizeWarning("padding", value, this.m_currentHtmlElement.CharacterPosition);
			}
			if (generalPadding != null)
			{
				effectivePadding = generalPadding;
				return true;
			}
			effectivePadding = null;
			return false;
		}
	}
}
