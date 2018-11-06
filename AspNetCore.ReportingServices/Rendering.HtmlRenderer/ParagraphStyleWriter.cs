using AspNetCore.ReportingServices.Rendering.RPLProcessing;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal class ParagraphStyleWriter : ElementStyleWriter
	{
		internal enum Mode
		{
			ListOnly = 1,
			ParagraphOnly,
			All
		}

		private RPLParagraph m_paragraph;

		private RPLTextBox m_textBox;

		private bool m_outputSharedInNonShared;

		private Mode m_mode = Mode.All;

		private int m_currentListLevel;

		internal RPLParagraph Paragraph
		{
			get
			{
				return this.m_paragraph;
			}
			set
			{
				this.m_paragraph = value;
			}
		}

		internal Mode ParagraphMode
		{
			get
			{
				return this.m_mode;
			}
			set
			{
				this.m_mode = value;
			}
		}

		internal int CurrentListLevel
		{
			get
			{
				return this.m_currentListLevel;
			}
			set
			{
				this.m_currentListLevel = value;
			}
		}

		internal bool OutputSharedInNonShared
		{
			get
			{
				return this.m_outputSharedInNonShared;
			}
			set
			{
				this.m_outputSharedInNonShared = value;
			}
		}

		internal ParagraphStyleWriter(IHtmlReportWriter renderer, RPLTextBox textBox)
			: base(renderer)
		{
			this.m_textBox = textBox;
		}

		internal override bool NeedsToWriteNullStyle(StyleWriterMode mode)
		{
			RPLParagraph paragraph = this.m_paragraph;
			switch (mode)
			{
			case StyleWriterMode.NonShared:
			{
				RPLParagraphProps rPLParagraphProps = paragraph.ElementProps as RPLParagraphProps;
				if (rPLParagraphProps.LeftIndent == null && rPLParagraphProps.RightIndent == null && rPLParagraphProps.SpaceBefore == null && rPLParagraphProps.SpaceAfter == null && rPLParagraphProps.HangingIndent == null)
				{
					RPLStyleProps nonSharedStyle = this.m_textBox.ElementProps.NonSharedStyle;
					if (!this.m_outputSharedInNonShared)
					{
						break;
					}
					return true;
				}
				return true;
			}
			case StyleWriterMode.Shared:
			{
				if (this.m_outputSharedInNonShared)
				{
					return false;
				}
				RPLParagraphPropsDef rPLParagraphPropsDef = paragraph.ElementProps.Definition as RPLParagraphPropsDef;
				if (rPLParagraphPropsDef.LeftIndent == null && rPLParagraphPropsDef.RightIndent == null && rPLParagraphPropsDef.SpaceBefore == null && rPLParagraphPropsDef.SpaceAfter == null && rPLParagraphPropsDef.HangingIndent == null)
				{
					IRPLStyle sharedStyle = this.m_textBox.ElementPropsDef.SharedStyle;
					if (sharedStyle == null)
					{
						break;
					}
					if (!HTML4Renderer.IsDirectionRTL(sharedStyle))
					{
						break;
					}
					return true;
				}
				return true;
			}
			}
			return false;
		}

		internal override void WriteStyles(StyleWriterMode mode, IRPLStyle style)
		{
			RPLParagraph paragraph = this.m_paragraph;
			RPLTextBox textBox = this.m_textBox;
			RPLTextBoxProps rPLTextBoxProps = textBox.ElementProps as RPLTextBoxProps;
			if (paragraph != null)
			{
				RPLParagraphProps rPLParagraphProps = paragraph.ElementProps as RPLParagraphProps;
				RPLParagraphPropsDef rPLParagraphPropsDef = rPLParagraphProps.Definition as RPLParagraphPropsDef;
				RPLReportSize rPLReportSize = null;
				RPLReportSize rPLReportSize2 = null;
				RPLReportSize rPLReportSize3 = null;
				RPLReportSize rPLReportSize4 = null;
				RPLReportSize rPLReportSize5 = null;
				switch (mode)
				{
				case StyleWriterMode.All:
					rPLReportSize = rPLParagraphProps.HangingIndent;
					if (rPLReportSize == null)
					{
						rPLReportSize = rPLParagraphPropsDef.HangingIndent;
					}
					rPLReportSize2 = rPLParagraphProps.LeftIndent;
					if (rPLReportSize2 == null)
					{
						rPLReportSize2 = rPLParagraphPropsDef.LeftIndent;
					}
					rPLReportSize3 = rPLParagraphProps.RightIndent;
					if (rPLReportSize3 == null)
					{
						rPLReportSize3 = rPLParagraphPropsDef.RightIndent;
					}
					rPLReportSize4 = rPLParagraphProps.SpaceBefore;
					if (rPLReportSize4 == null)
					{
						rPLReportSize4 = rPLParagraphPropsDef.SpaceBefore;
					}
					rPLReportSize5 = rPLParagraphProps.SpaceAfter;
					if (rPLReportSize5 == null)
					{
						rPLReportSize5 = rPLParagraphPropsDef.SpaceAfter;
					}
					break;
				case StyleWriterMode.NonShared:
				{
					RPLStyleProps nonSharedStyle = this.m_textBox.ElementProps.NonSharedStyle;
					rPLReportSize = rPLParagraphProps.HangingIndent;
					rPLReportSize3 = rPLParagraphProps.RightIndent;
					rPLReportSize2 = rPLParagraphProps.LeftIndent;
					rPLReportSize5 = rPLParagraphProps.SpaceAfter;
					rPLReportSize4 = rPLParagraphProps.SpaceBefore;
					if (this.m_outputSharedInNonShared)
					{
						if (rPLReportSize == null)
						{
							rPLReportSize = rPLParagraphPropsDef.HangingIndent;
						}
						if (rPLReportSize3 == null)
						{
							rPLReportSize3 = rPLParagraphPropsDef.RightIndent;
						}
						if (rPLReportSize2 == null)
						{
							rPLReportSize2 = rPLParagraphPropsDef.LeftIndent;
						}
						if (rPLReportSize5 == null)
						{
							rPLReportSize5 = rPLParagraphPropsDef.SpaceAfter;
						}
						if (rPLReportSize4 == null)
						{
							rPLReportSize4 = rPLParagraphPropsDef.SpaceBefore;
						}
					}
					else
					{
						bool flag = HTML4Renderer.IsDirectionRTL(this.m_textBox.ElementProps.Style);
						if (rPLReportSize == null)
						{
							if (flag)
							{
								if (rPLReportSize3 != null)
								{
									rPLReportSize = rPLParagraphPropsDef.HangingIndent;
								}
							}
							else if (rPLReportSize2 != null)
							{
								rPLReportSize = rPLParagraphPropsDef.HangingIndent;
							}
						}
						else if (flag)
						{
							if (rPLReportSize3 == null)
							{
								rPLReportSize3 = rPLParagraphPropsDef.RightIndent;
							}
						}
						else if (rPLReportSize2 == null)
						{
							rPLReportSize2 = rPLParagraphPropsDef.LeftIndent;
						}
					}
					break;
				}
				case StyleWriterMode.Shared:
				{
					RPLStyleProps sharedStyle = this.m_textBox.ElementPropsDef.SharedStyle;
					rPLReportSize = rPLParagraphPropsDef.HangingIndent;
					rPLReportSize2 = rPLParagraphPropsDef.LeftIndent;
					rPLReportSize3 = rPLParagraphPropsDef.RightIndent;
					rPLReportSize4 = rPLParagraphPropsDef.SpaceBefore;
					rPLReportSize5 = rPLParagraphPropsDef.SpaceAfter;
					break;
				}
				}
				if (this.m_currentListLevel > 0 && rPLReportSize != null && rPLReportSize.ToMillimeters() < 0.0 && !base.m_renderer.IsBrowserIE)
				{
					rPLReportSize = null;
				}
				if (this.m_mode != Mode.ParagraphOnly)
				{
					this.FixIndents(ref rPLReportSize2, ref rPLReportSize3, ref rPLReportSize4, ref rPLReportSize5, rPLReportSize);
					bool flag2 = HTML4Renderer.IsWritingModeVertical(rPLTextBoxProps.Style);
					if (flag2 && base.m_renderer.IsBrowserIE)
					{
						base.WriteStyle(HTML4Renderer.m_paddingLeft, rPLReportSize2);
					}
					else
					{
						base.WriteStyle(HTML4Renderer.m_marginLeft, rPLReportSize2);
					}
					base.WriteStyle(HTML4Renderer.m_marginRight, rPLReportSize3);
					base.WriteStyle(HTML4Renderer.m_marginTop, rPLReportSize4);
					if (flag2 && base.m_renderer.IsBrowserIE)
					{
						base.WriteStyle(HTML4Renderer.m_marginBottom, rPLReportSize5);
					}
					else
					{
						base.WriteStyle(HTML4Renderer.m_paddingBottom, rPLReportSize5);
					}
				}
				if (this.m_mode == Mode.ListOnly)
				{
					base.WriteStyle(HTML4Renderer.m_fontFamily, "Arial");
					base.WriteStyle(HTML4Renderer.m_fontSize, "10pt");
				}
				else if (rPLReportSize != null && rPLReportSize.ToMillimeters() < 0.0)
				{
					base.WriteStyle(HTML4Renderer.m_textIndent, rPLReportSize);
				}
			}
			if (style != null)
			{
				if (this.m_mode != Mode.All && this.m_mode != Mode.ParagraphOnly)
				{
					return;
				}
				object obj = style[25];
				if (obj != null || mode != 0)
				{
					RPLFormat.TextAlignments textAlignments = RPLFormat.TextAlignments.General;
					if (obj != null)
					{
						textAlignments = (RPLFormat.TextAlignments)obj;
					}
					if (textAlignments == RPLFormat.TextAlignments.General)
					{
						bool flag3 = HTML4Renderer.GetTextAlignForType(rPLTextBoxProps);
						if (HTML4Renderer.IsDirectionRTL(rPLTextBoxProps.Style))
						{
							flag3 = ((byte)((!flag3) ? 1 : 0) != 0);
						}
						base.WriteStream(HTML4Renderer.m_textAlign);
						if (flag3)
						{
							base.WriteStream(HTML4Renderer.m_rightValue);
						}
						else
						{
							base.WriteStream(HTML4Renderer.m_leftValue);
						}
						base.WriteStream(HTML4Renderer.m_semiColon);
					}
					else
					{
						base.WriteStyle(HTML4Renderer.m_textAlign, EnumStrings.GetValue(textAlignments), null);
					}
				}
				base.WriteStyle(HTML4Renderer.m_lineHeight, style[28]);
			}
		}

		internal void FixIndents(ref RPLReportSize leftIndent, ref RPLReportSize rightIndent, ref RPLReportSize spaceBefore, ref RPLReportSize spaceAfter, RPLReportSize hangingIndent)
		{
			RPLTextBoxProps rPLTextBoxProps = this.m_textBox.ElementProps as RPLTextBoxProps;
			if (HTML4Renderer.IsDirectionRTL(rPLTextBoxProps.Style))
			{
				rightIndent = this.FixHangingIndent(rightIndent, hangingIndent);
			}
			else
			{
				leftIndent = this.FixHangingIndent(leftIndent, hangingIndent);
			}
			object obj = rPLTextBoxProps.Style[30];
			if (base.m_renderer.IsBrowserIE && obj != null && HTML4Renderer.IsWritingModeVertical((RPLFormat.WritingModes)obj))
			{
				RPLReportSize rPLReportSize = leftIndent;
				leftIndent = spaceAfter;
				spaceAfter = rightIndent;
				rightIndent = spaceBefore;
				spaceBefore = rPLReportSize;
			}
		}

		internal RPLReportSize FixHangingIndent(RPLReportSize leftIndent, RPLReportSize hangingIndent)
		{
			if (hangingIndent == null)
			{
				return leftIndent;
			}
			double num = hangingIndent.ToMillimeters();
			if (num < 0.0)
			{
				double num2 = 0.0;
				if (leftIndent != null)
				{
					num2 = leftIndent.ToMillimeters();
				}
				num2 -= num;
				leftIndent = new RPLReportSize(num2);
			}
			return leftIndent;
		}
	}
}
