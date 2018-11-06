using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal class RichTextRenderer : IDisposable
	{
		private float m_width;

		private float m_flowHeight = 3.40282347E+38f;

		private float m_height;

		private bool m_lineLimit = true;

		private bool m_charTrimLastLine = true;

		private float m_dpi;

		private FontCache m_fontCache;

		private TextBox m_rttextbox;

		private List<Paragraph> m_cachedrtparagraphs;

		internal bool LineLimit
		{
			get
			{
				return this.m_lineLimit;
			}
			set
			{
				if (this.m_lineLimit != value)
				{
					this.m_lineLimit = value;
					this.ResetCachedObjects();
				}
			}
		}

		internal bool CharTrimLastLine
		{
			get
			{
				return this.m_charTrimLastLine;
			}
			set
			{
				if (this.m_charTrimLastLine != value)
				{
					this.m_charTrimLastLine = value;
					this.ResetCachedObjects();
				}
			}
		}

		internal float Width
		{
			get
			{
				return this.m_width;
			}
			set
			{
				if (this.m_width != value)
				{
					this.m_width = value;
					this.ResetCachedObjects();
				}
			}
		}

		internal int WidthInPX
		{
			get
			{
				return TextBox.ConvertToPixels(this.Width, this.Dpi);
			}
			set
			{
				this.Width = TextBox.ConvertToMillimeters(value, this.Dpi);
			}
		}

		internal float FlowHeight
		{
			get
			{
				return this.m_flowHeight;
			}
			set
			{
				if (this.m_flowHeight != value)
				{
					this.m_flowHeight = value;
					this.ResetCachedObjects();
				}
			}
		}

		internal int FlowHeightInPX
		{
			get
			{
				return TextBox.ConvertToPixels(this.FlowHeight, this.Dpi);
			}
			set
			{
				this.FlowHeight = TextBox.ConvertToMillimeters(value, this.Dpi);
			}
		}

		internal float Dpi
		{
			get
			{
				if (this.m_dpi == 0.0)
				{
					using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
					{
						this.m_dpi = graphics.DpiX;
					}
				}
				return this.m_dpi;
			}
			set
			{
				this.m_dpi = value;
			}
		}

		internal FontCache FontCache
		{
			get
			{
				if (this.m_fontCache == null)
				{
					this.m_fontCache = new FontCache(this.Dpi);
				}
				return this.m_fontCache;
			}
			set
			{
				this.m_fontCache = value;
			}
		}

		internal List<Paragraph> RTParagraphs
		{
			get
			{
				if (this.m_cachedrtparagraphs == null)
				{
					this.UpdateCachedObjects();
				}
				return this.m_cachedrtparagraphs;
			}
			set
			{
				this.m_cachedrtparagraphs = value;
			}
		}

		internal TextBox RTTextbox
		{
			get
			{
				return this.m_rttextbox;
			}
		}

		internal RichTextRenderer()
		{
		}

		internal void Render(Graphics g, RectangleF rectangle)
		{
			this.Render(g, rectangle, null, true);
		}

		internal void Render(Graphics g, RectangleF rectangle, bool unitsInMM)
		{
			this.Render(g, rectangle, null, unitsInMM);
		}

		internal void Render(Graphics g, RectangleF rectangle, IEnumerable<RTSelectionHighlight> highlights)
		{
			this.Render(g, rectangle, highlights, true);
		}

		internal void Render(Graphics g, RectangleF rectangle, IEnumerable<RTSelectionHighlight> highlights, bool unitsInMM)
		{
			this.Render(g, rectangle, PointF.Empty, highlights, unitsInMM);
		}

		internal void Render(Graphics g, RectangleF rectangle, PointF offset, IEnumerable<RTSelectionHighlight> highlights, bool unitsInMM)
		{
			List<Paragraph> rTParagraphs = this.RTParagraphs;
			if (rTParagraphs != null && rTParagraphs.Count != 0)
			{
				using (RevertingDeviceContext revertingDeviceContext = new RevertingDeviceContext(g, this.Dpi))
				{
					Win32DCSafeHandle hdc = revertingDeviceContext.Hdc;
					if (highlights != null)
					{
						RPLFormat.Directions direction = this.RTTextbox.TextBoxProps.Direction;
						foreach (RTSelectionHighlight highlight in highlights)
						{
							if (!RichTextRenderer.HighlightStartLessThanOrEqualToEnd(highlight.SelectionStart, highlight.SelectionEnd))
							{
								TextBoxContext selectionStart = highlight.SelectionStart;
								highlight.SelectionStart = highlight.SelectionEnd;
								highlight.SelectionEnd = selectionStart;
							}
							TextRun textRun = default(TextRun);
							CaretInfo caretInfo = this.MapLocation(hdc, highlight.SelectionStart, true, true, out textRun);
							TextRun textRun2 = default(TextRun);
							CaretInfo caretInfo2 = this.MapLocation(hdc, highlight.SelectionEnd, true, true, out textRun2);
							if (caretInfo != null && caretInfo2 != null && textRun != null && textRun2 != null)
							{
								this.SetHighlighting(rTParagraphs, hdc, highlight, textRun, textRun2, caretInfo.Position.X, caretInfo2.Position.X);
							}
						}
					}
					Rectangle rectangle2 = (!unitsInMM) ? Rectangle.Round(rectangle) : new Rectangle(TextBox.ConvertToPixels(rectangle.X, this.m_dpi), TextBox.ConvertToPixels(rectangle.Y, this.m_dpi), TextBox.ConvertToPixels(rectangle.Width, this.m_dpi), TextBox.ConvertToPixels(rectangle.Height, this.m_dpi));
					revertingDeviceContext.XForm.Transform(ref rectangle2);
					Win32ObjectSafeHandle win32ObjectSafeHandle = Win32.CreateRectRgn(rectangle2.Left - 1, rectangle2.Top - 1, rectangle2.Right + 1, rectangle2.Bottom + 1);
					if (!win32ObjectSafeHandle.IsInvalid)
					{
						try
						{
							if (Win32.SelectClipRgn(hdc, win32ObjectSafeHandle) == 0)
							{
								Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
							}
						}
						finally
						{
							win32ObjectSafeHandle.Close();
						}
					}
					TextBox.Render(this.RTTextbox, rTParagraphs, hdc, this.FontCache, offset, rectangle, this.m_dpi, unitsInMM);
					if (Win32.SelectClipRgn(hdc, Win32ObjectSafeHandle.Zero) == 0)
					{
						Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
					}
				}
			}
		}

		private void SetHighlighting(List<Paragraph> paragraphs, Win32DCSafeHandle hdc, RTSelectionHighlight highlight, TextRun runStart, TextRun runEnd, int x1, int x2)
		{
			if (runStart == runEnd)
			{
				if (x1 != x2)
				{
					if (x1 < x2)
					{
						runStart.HighlightStart = x1;
						runStart.HighlightEnd = x2;
					}
					else
					{
						runStart.HighlightStart = x2;
						runStart.HighlightEnd = x1;
					}
					runStart.HighlightColor = highlight.Color;
					runStart.AllowColorInversion = highlight.AllowColorInversion;
				}
			}
			else
			{
				bool flag = false;
				bool flag2 = false;
				Color color = highlight.Color;
				bool allowColorInversion = highlight.AllowColorInversion;
				for (int i = highlight.SelectionStart.ParagraphIndex; i <= highlight.SelectionEnd.ParagraphIndex; i++)
				{
					int count = paragraphs[i].TextLines.Count;
					for (int j = 0; j < count; j++)
					{
						List<TextRun> logicalRuns = paragraphs[i].TextLines[j].LogicalRuns;
						int count2 = logicalRuns.Count;
						for (int k = 0; k < count2; k++)
						{
							TextRun textRun = logicalRuns[k];
							bool flag3 = textRun.ScriptAnalysis.fLayoutRTL == 0;
							if (textRun == runStart)
							{
								flag2 = true;
								if (flag3 && x1 < textRun.GetWidth(hdc, this.FontCache))
								{
									textRun.HighlightStart = x1;
									textRun.HighlightEnd = -1;
									textRun.HighlightColor = color;
									textRun.AllowColorInversion = allowColorInversion;
								}
								else if (!flag3 && x1 > 0)
								{
									textRun.HighlightStart = -1;
									textRun.HighlightEnd = x1;
									textRun.HighlightColor = color;
									textRun.AllowColorInversion = allowColorInversion;
								}
							}
							else
							{
								if (textRun == runEnd)
								{
									flag2 = false;
									if (flag3 && x2 > 0)
									{
										textRun.HighlightStart = -1;
										textRun.HighlightEnd = x2;
										textRun.HighlightColor = color;
										textRun.AllowColorInversion = allowColorInversion;
									}
									else if (!flag3 && x2 < textRun.GetWidth(hdc, this.FontCache))
									{
										textRun.HighlightStart = x2;
										textRun.HighlightEnd = -1;
										textRun.HighlightColor = color;
										textRun.AllowColorInversion = allowColorInversion;
									}
									flag = true;
									break;
								}
								if (flag2)
								{
									textRun.HighlightStart = -1;
									textRun.HighlightEnd = -1;
									textRun.HighlightColor = color;
									textRun.AllowColorInversion = allowColorInversion;
								}
							}
						}
						if (flag)
						{
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
		}

		private static bool HighlightStartLessThanOrEqualToEnd(TextBoxContext start, TextBoxContext end)
		{
			if (start.ParagraphIndex == end.ParagraphIndex)
			{
				if (start.TextRunIndex == end.TextRunIndex)
				{
					return start.TextRunCharacterIndex <= end.TextRunCharacterIndex;
				}
				return start.TextRunIndex <= end.TextRunIndex;
			}
			return start.ParagraphIndex <= end.ParagraphIndex;
		}

		internal Rectangle GetTextBoundingBoxPx(Rectangle rect, RPLFormat.VerticalAlignments vAlign)
		{
			if (this.RTParagraphs == null)
			{
				return new Rectangle(rect.Location, Size.Empty);
			}
			Rectangle rectangle = Rectangle.Empty;
			Size size = default(Size);
			int num = rect.Top;
			Size size2 = rect.Size;
			if (this.RTTextbox.VerticalText)
			{
				size2 = new Size(size2.Height, size2.Width);
			}
			foreach (Paragraph rTParagraph in this.RTParagraphs)
			{
				int num2 = 0;
				if (rTParagraph.TextLines.Count == 1 && rTParagraph.ParagraphProps.HangingIndent > 0.0)
				{
					num2 += TextBox.ConvertToPixels(rTParagraph.ParagraphProps.HangingIndent, this.Dpi);
				}
				foreach (TextLine textLine in rTParagraph.TextLines)
				{
					size.Width = Math.Max(Math.Max(size.Width, textLine.GetWidth(Win32DCSafeHandle.Zero, this.FontCache)), 10);
					size.Height += textLine.GetHeight(Win32DCSafeHandle.Zero, this.FontCache);
				}
				switch (rTParagraph.ParagraphProps.Alignment)
				{
				case RPLFormat.TextAlignments.General:
					if (this.RTTextbox.TextBoxProps.Direction != RPLFormat.Directions.RTL)
					{
						goto case RPLFormat.TextAlignments.Left;
					}
					goto case RPLFormat.TextAlignments.Right;
				case RPLFormat.TextAlignments.Left:
					num2 += rect.Left;
					break;
				case RPLFormat.TextAlignments.Center:
					num2 += rect.Left + (size2.Width - size.Width) / 2;
					break;
				case RPLFormat.TextAlignments.Right:
					num2 += rect.Left + size2.Width - size.Width;
					break;
				default:
					throw new ArgumentException("Unknown TextAlignment: " + rTParagraph.ParagraphProps.Alignment.ToString());
				}
				num2 = ((this.RTTextbox.TextBoxProps.Direction != 0) ? (num2 - TextBox.ConvertToPixels((float)(rTParagraph.ParagraphProps.RightIndent + 10.583333015441895 * (float)rTParagraph.ParagraphProps.ListLevel), this.Dpi)) : (num2 + TextBox.ConvertToPixels((float)(rTParagraph.ParagraphProps.LeftIndent + 10.583333015441895 * (float)rTParagraph.ParagraphProps.ListLevel), this.Dpi)));
				Rectangle rectangle2 = new Rectangle(new Point(num2, num), size);
				num += size.Height;
				rectangle = ((!rectangle.IsEmpty) ? Rectangle.Union(rectangle, rectangle2) : rectangle2);
			}
			if (rectangle.Height < size2.Height)
			{
				switch (vAlign)
				{
				case RPLFormat.VerticalAlignments.Middle:
					rectangle.Y += (size2.Height - rectangle.Height) / 2;
					break;
				case RPLFormat.VerticalAlignments.Bottom:
					rectangle.Y = rect.Top + size2.Height - rectangle.Height;
					break;
				}
			}
			if (this.RTTextbox.VerticalText)
			{
				Point location = new Point(rect.Right - (rectangle.Top - rect.Top) - rectangle.Height, rectangle.Left);
				Size size3 = new Size(rectangle.Height, rectangle.Width);
				rectangle = new Rectangle(location, size3);
			}
			return Rectangle.Intersect(rect, rectangle);
		}

		internal TextBoxContext MapPoint(PointF pt)
		{
			bool flag = default(bool);
			return this.MapPoint((Graphics)null, pt, out flag);
		}

		internal TextBoxContext MapPoint(PointF pt, out bool atEndOfLine)
		{
			return this.MapPoint((Graphics)null, pt, out atEndOfLine);
		}

		internal TextBoxContext MapPoint(Graphics g, PointF pt)
		{
			bool flag = default(bool);
			return this.MapPoint(g, pt, out flag);
		}

		internal TextBoxContext MapPoint(Graphics g, PointF pt, out bool atEndOfLine)
		{
			TextBoxContext textBoxContext = null;
			atEndOfLine = false;
			TextRun textRun = default(TextRun);
			int iX = default(int);
			textBoxContext = this.GetParagraphAndRunIndex(g, (int)pt.X, (int)pt.Y, out textRun, out iX, out atEndOfLine);
			if (textRun != null)
			{
				GlyphData glyphData = textRun.GlyphData;
				GlyphShapeData glyphScriptShapeData = glyphData.GlyphScriptShapeData;
				if (glyphData != null && textRun.CharacterCount > 0)
				{
					int num = 0;
					int num2 = 0;
					int num3 = Win32.ScriptXtoCP(iX, textRun.CharacterCount, glyphScriptShapeData.GlyphCount, glyphScriptShapeData.Clusters, glyphScriptShapeData.VisAttrs, glyphData.Advances, ref textRun.SCRIPT_ANALYSIS, ref num, ref num2);
					if (Win32.Failed(num3))
					{
						Marshal.ThrowExceptionForHR(num3);
					}
					if (textRun.ScriptAnalysis.fLayoutRTL == 1)
					{
						if (num == -1)
						{
							textBoxContext.TextRunCharacterIndex += textRun.CharacterCount;
						}
						else if (pt.X <= 0.0)
						{
							TextBoxContext textBoxContext2 = textBoxContext;
							textBoxContext2.TextRunCharacterIndex = textBoxContext2.TextRunCharacterIndex;
						}
						else
						{
							textBoxContext.TextRunCharacterIndex += num + num2;
						}
					}
					else
					{
						textBoxContext.TextRunCharacterIndex += num + num2;
					}
				}
			}
			if (textBoxContext == null)
			{
				textBoxContext = new TextBoxContext();
			}
			return textBoxContext;
		}

		private TextBoxContext GetParagraphAndRunIndex(Graphics g, int x, int y, out TextRun run, out int runX, out bool atEndOfLine)
		{
			List<Paragraph> rTParagraphs = this.RTParagraphs;
			Paragraph paragraph = null;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int i = 0;
			bool flag = true;
			run = null;
			runX = 0;
			atEndOfLine = false;
			if (rTParagraphs != null && rTParagraphs.Count != 0)
			{
				if (y < 0)
				{
					y = 0;
				}
				for (; i < rTParagraphs.Count; i++)
				{
					paragraph = rTParagraphs[i];
					if (y >= paragraph.OffsetY && y < paragraph.OffsetY + paragraph.Height)
					{
						goto IL_006f;
					}
					if (i + 1 == rTParagraphs.Count)
					{
						goto IL_006f;
					}
					continue;
					IL_006f:
					y -= paragraph.OffsetY;
					num = TextBox.ConvertToPixels(paragraph.ParagraphProps.LeftIndent, this.Dpi);
					num2 = TextBox.ConvertToPixels(paragraph.ParagraphProps.RightIndent, this.Dpi);
					num3 = TextBox.ConvertToPixels(paragraph.ParagraphProps.HangingIndent, this.Dpi);
					flag = (this.m_rttextbox.TextBoxProps.Direction == RPLFormat.Directions.LTR);
					if (num3 < 0)
					{
						if (flag)
						{
							num -= num3;
						}
						else
						{
							num2 -= num3;
						}
					}
					int listLevel = paragraph.ParagraphProps.ListLevel;
					if (listLevel > 0)
					{
						int num4 = listLevel * TextBox.ConvertToPixels(10.583333f, this.Dpi);
						if (flag)
						{
							num += num4;
						}
						else
						{
							num2 += num4;
						}
					}
					break;
				}
				TextBoxContext paragraphAndRunIndex = this.GetParagraphAndRunIndex(g, paragraph, num, num2, num3, x, y, flag, i == rTParagraphs.Count - 1, out run, out runX, out atEndOfLine);
				if (paragraphAndRunIndex != null)
				{
					paragraphAndRunIndex.ParagraphIndex = i;
				}
				return paragraphAndRunIndex;
			}
			return null;
		}

		private TextBoxContext GetParagraphAndRunIndex(Graphics g, Paragraph paragraph, int leftIndent, int rightIndent, int hangingIndent, int x, int y, bool isLTR, bool lastParagraph, out TextRun run, out int runX, out bool atEndOfLine)
		{
			Win32DCSafeHandle win32DCSafeHandle = null;
			bool flag = false;
			try
			{
				if (g == null)
				{
					flag = true;
					g = Graphics.FromHwnd(IntPtr.Zero);
				}
				win32DCSafeHandle = new Win32DCSafeHandle(g.GetHdc(), false);
				runX = x;
				run = null;
				atEndOfLine = false;
				int num = 0;
				while (num < paragraph.TextLines.Count)
				{
					TextLine textLine = paragraph.TextLines[num];
					y -= textLine.GetHeight(win32DCSafeHandle, this.FontCache);
					if (textLine.FirstLine)
					{
						y -= TextBox.ConvertToPixels(paragraph.ParagraphProps.SpaceBefore, this.Dpi);
					}
					else if (textLine.LastLine)
					{
						y -= TextBox.ConvertToPixels(paragraph.ParagraphProps.SpaceAfter, this.Dpi);
					}
					if (y >= 0 && num + 1 != paragraph.TextLines.Count)
					{
						num++;
						continue;
					}
					int width = textLine.GetWidth(win32DCSafeHandle, this.FontCache);
					if (x > this.WidthInPX)
					{
						atEndOfLine = (width > 0);
						int num2 = default(int);
						bool flag2 = default(bool);
						run = this.GetLastNonLineBreakRun(textLine.LogicalRuns, out num2, out flag2);
						TextBoxContext textBoxContext = new TextBoxContext();
						textBoxContext.TextRunIndex = run.TextRunProperties.IndexInParagraph;
						if (flag2 && run.CharacterCount > 0)
						{
							textBoxContext.TextRunCharacterIndex = run.CharacterIndexInOriginal + num2 + 1;
						}
						else
						{
							textBoxContext.TextRunCharacterIndex = run.CharacterIndexInOriginal;
						}
						run = null;
						return textBoxContext;
					}
					if (x < 0)
					{
						atEndOfLine = false;
						int num3 = default(int);
						run = this.GetFirstNonLineBreakRun(textLine.LogicalRuns, out num3);
						TextBoxContext textBoxContext2 = new TextBoxContext();
						textBoxContext2.TextRunIndex = run.TextRunProperties.IndexInParagraph;
						textBoxContext2.TextRunCharacterIndex = run.CharacterIndexInOriginal + num3;
						run = null;
						return textBoxContext2;
					}
					RPLFormat.TextAlignments textAlignments = paragraph.ParagraphProps.Alignment;
					if (textAlignments == RPLFormat.TextAlignments.General)
					{
						textAlignments = this.m_rttextbox.TextBoxProps.DefaultAlignment;
						if (!isLTR)
						{
							switch (textAlignments)
							{
							case RPLFormat.TextAlignments.Left:
								textAlignments = RPLFormat.TextAlignments.Right;
								break;
							case RPLFormat.TextAlignments.Right:
								textAlignments = RPLFormat.TextAlignments.Left;
								break;
							}
						}
					}
					switch (textAlignments)
					{
					case RPLFormat.TextAlignments.Center:
						runX = x - (leftIndent + (this.WidthInPX - leftIndent - rightIndent) / 2 - width / 2);
						break;
					case RPLFormat.TextAlignments.Right:
						runX = x - (this.WidthInPX - width - rightIndent);
						break;
					default:
						runX = x - leftIndent;
						break;
					}
					if (textLine.FirstLine && hangingIndent != 0)
					{
						if (isLTR)
						{
							switch (textAlignments)
							{
							case RPLFormat.TextAlignments.Left:
								runX -= hangingIndent;
								break;
							case RPLFormat.TextAlignments.Center:
								runX -= hangingIndent / 2;
								break;
							}
						}
						else
						{
							switch (textAlignments)
							{
							case RPLFormat.TextAlignments.Right:
								runX += hangingIndent;
								break;
							case RPLFormat.TextAlignments.Center:
								runX += hangingIndent / 2;
								break;
							}
						}
					}
					runX = Math.Max(0, runX);
					return this.GetParagraphAndRunIndex(win32DCSafeHandle, paragraph, textLine, x, width, lastParagraph, out run, ref runX, out atEndOfLine);
				}
				return null;
			}
			finally
			{
				if (!win32DCSafeHandle.IsInvalid)
				{
					g.ReleaseHdc();
				}
				if (flag)
				{
					g.Dispose();
					g = null;
				}
			}
		}

		private TextRun GetLastNonLineBreakRun(List<TextRun> runs, out int charIndex, out bool hasNonCRLFChars)
		{
			for (int num = runs.Count - 1; num >= 0; num--)
			{
				TextRun textRun = runs[num];
				if (textRun.CharacterCount > 0)
				{
					string text = textRun.Text;
					for (charIndex = textRun.CharacterCount - 1; charIndex >= 0; charIndex--)
					{
						char c = text[charIndex];
						if (c != '\n' && c != '\r')
						{
							hasNonCRLFChars = true;
							return textRun;
						}
					}
				}
			}
			charIndex = 0;
			hasNonCRLFChars = false;
			return runs[runs.Count - 1];
		}

		private TextRun GetFirstNonLineBreakRun(List<TextRun> runs, out int charIndex)
		{
			TextRun textRun;
			for (int i = 0; i < runs.Count; i++)
			{
				textRun = runs[i];
				if (textRun.CharacterCount > 0)
				{
					string text = textRun.Text;
					for (charIndex = 0; charIndex < textRun.CharacterCount; charIndex++)
					{
						char c = text[charIndex];
						if (c != '\n' && c != '\r')
						{
							return textRun;
						}
					}
				}
			}
			textRun = runs[runs.Count - 1];
			if (textRun.CharacterCount > 0)
			{
				charIndex = textRun.CharacterCount - 1;
			}
			else
			{
				charIndex = 0;
			}
			return textRun;
		}

		private TextBoxContext GetParagraphAndRunIndex(Win32DCSafeHandle hdc, Paragraph paragraph, TextLine line, int x, int lineWidth, bool lastParagraph, out TextRun run, ref int runX, out bool atEndOfLine)
		{
			atEndOfLine = false;
			run = null;
			int count = line.VisualRuns.Count;
			int num = 0;
			while (num < count)
			{
				run = line.VisualRuns[num];
				int width = run.GetWidth(hdc, this.FontCache, num == count - 1);
				if (runX - width > 0 && num + 1 != count)
				{
					runX -= width;
					num++;
					continue;
				}
				TextBoxContext textBoxContext;
				if (runX - width > 0)
				{
					atEndOfLine = true;
					if (run.ScriptAnalysis.fLayoutRTL == 1 && x >= lineWidth)
					{
						run = line.VisualRuns[count - 1];
						textBoxContext = new TextBoxContext();
						textBoxContext.TextRunIndex = run.TextRunProperties.IndexInParagraph;
						textBoxContext.TextRunCharacterIndex = run.CharacterIndexInOriginal;
						return textBoxContext;
					}
				}
				int num2 = 0;
				int characterCount = run.CharacterCount;
				if (characterCount > 0)
				{
					if (run.ScriptAnalysis.fLayoutRTL == 0 && num + 1 == count)
					{
						goto IL_00f0;
					}
					if (run.ScriptAnalysis.fLayoutRTL == 1 && num == 0)
					{
						goto IL_00f0;
					}
					if (num == 0 && (run.Text[0] == '\r' || run.Text[0] == '\n'))
					{
						num2 = -1;
					}
				}
				else if (lastParagraph && line.LastLine && !line.FirstLine)
				{
					num2 = 1;
				}
				goto IL_025d;
				IL_025d:
				textBoxContext = new TextBoxContext();
				textBoxContext.TextRunIndex = run.TextRunProperties.IndexInParagraph;
				textBoxContext.TextRunCharacterIndex = run.CharacterIndexInOriginal + num2;
				return textBoxContext;
				IL_00f0:
				if (width <= 0)
				{
					string text = run.Text;
					if (characterCount > 1 && text[characterCount - 2] == '\r' && text[characterCount - 1] == '\n')
					{
						num2 = ((run.ScriptAnalysis.fLayoutRTL != 0) ? (-1) : (-2));
					}
					else if (text[characterCount - 1] == '\n' && run.ScriptAnalysis.fLayoutRTL == 0)
					{
						if (num > 0)
						{
							run = line.VisualRuns[num - 1];
							if (run.CharacterCount > 0 && run.Text[run.CharacterCount - 1] == '\r')
							{
								num2 = -1;
							}
						}
						else
						{
							num2 = -1;
						}
					}
				}
				else if (num + 1 == count && (run.Text[characterCount - 1] == '\r' || run.Text[characterCount - 1] == '\n'))
				{
					num2 = -1;
				}
				else if (num == 0 && (run.Text[0] == '\r' || run.Text[0] == '\n'))
				{
					num2 = -1;
				}
				goto IL_025d;
			}
			return null;
		}

		internal CaretInfo MapLocation(TextBoxContext location)
		{
			return this.MapLocation(null, location, false);
		}

		internal CaretInfo MapLocation(Graphics g, TextBoxContext location)
		{
			return this.MapLocation(g, location, false);
		}

		internal CaretInfo MapLocation(TextBoxContext location, bool moveCaretToNextLine)
		{
			return this.MapLocation(null, location, moveCaretToNextLine);
		}

		internal CaretInfo MapLocation(Graphics g, TextBoxContext location, bool moveCaretToNextLine)
		{
			TextRun textRun = default(TextRun);
			return this.MapLocation(g, location, false, moveCaretToNextLine, out textRun);
		}

		private CaretInfo MapLocation(Graphics g, TextBoxContext location, bool relativeToRun, bool moveCaretToNextLine, out TextRun run)
		{
			Win32DCSafeHandle win32DCSafeHandle = null;
			bool flag = false;
			try
			{
				if (g == null)
				{
					flag = true;
					g = Graphics.FromHwnd(IntPtr.Zero);
				}
				win32DCSafeHandle = new Win32DCSafeHandle(g.GetHdc(), false);
				return this.MapLocation(win32DCSafeHandle, location, relativeToRun, moveCaretToNextLine, out run);
			}
			finally
			{
				if (!win32DCSafeHandle.IsInvalid)
				{
					g.ReleaseHdc();
				}
				if (flag)
				{
					g.Dispose();
					g = null;
				}
			}
		}

		private CaretInfo MapLocation(Win32DCSafeHandle hdc, TextBoxContext location, bool relativeToRun, bool moveCaretToNextLine, out TextRun run)
		{
			CaretInfo caretInfo = null;
			run = null;
			int lineYOffset = default(int);
			int lineHeight = default(int);
			int iCP = default(int);
			bool isFirstLine = default(bool);
			bool isLastLine = default(bool);
			Point paragraphAndRunCoordinates = this.GetParagraphAndRunCoordinates(hdc, location, moveCaretToNextLine, out lineYOffset, out lineHeight, out run, out iCP, out isFirstLine, out isLastLine);
			if (run != null)
			{
				GlyphData glyphData = run.GlyphData;
				GlyphShapeData glyphScriptShapeData = glyphData.GlyphScriptShapeData;
				int num = 0;
				if (glyphData != null && run.CharacterCount > 0)
				{
					int num2 = Win32.ScriptCPtoX(iCP, false, run.CharacterCount, glyphScriptShapeData.GlyphCount, glyphScriptShapeData.Clusters, glyphScriptShapeData.VisAttrs, glyphData.Advances, ref run.SCRIPT_ANALYSIS, ref num);
					if (Win32.Failed(num2))
					{
						Marshal.ThrowExceptionForHR(num2);
					}
				}
				caretInfo = new CaretInfo();
				CachedFont cachedFont = run.GetCachedFont(hdc, this.FontCache);
				caretInfo.Height = cachedFont.GetHeight(hdc, this.FontCache);
				caretInfo.Ascent = cachedFont.GetAscent(hdc, this.FontCache);
				caretInfo.Descent = cachedFont.GetDescent(hdc, this.FontCache);
				caretInfo.LineHeight = lineHeight;
				caretInfo.LineYOffset = lineYOffset;
				caretInfo.IsFirstLine = isFirstLine;
				caretInfo.IsLastLine = isLastLine;
				List<Paragraph> rTParagraph = this.RTParagraphs;
				int y = paragraphAndRunCoordinates.Y - caretInfo.Ascent;
				if (relativeToRun)
				{
					caretInfo.Position = new Point(num, y);
				}
				else
				{
					caretInfo.Position = new Point(paragraphAndRunCoordinates.X + num, y);
				}
			}
			return caretInfo;
		}

		private Point GetParagraphAndRunCoordinates(Win32DCSafeHandle hdc, TextBoxContext location, bool moveCaretToNextLine, out int lineYOffset, out int lineHeight, out TextRun textRun, out int textRunCharacterIndex, out bool isFirstLine, out bool isLastLine)
		{
			int num = 0;
			int textRunIndex = location.TextRunIndex;
			textRunCharacterIndex = location.TextRunCharacterIndex;
			lineYOffset = 0;
			lineHeight = 0;
			textRun = null;
			isFirstLine = true;
			isLastLine = true;
			List<Paragraph> rTParagraphs = this.RTParagraphs;
			if (rTParagraphs != null && location.ParagraphIndex < rTParagraphs.Count)
			{
				Paragraph paragraph = rTParagraphs[location.ParagraphIndex];
				int num2 = paragraph.OffsetY + TextBox.ConvertToPixels(paragraph.ParagraphProps.SpaceBefore, this.Dpi);
				int num3 = TextBox.ConvertToPixels(paragraph.ParagraphProps.LeftIndent, this.Dpi);
				int num4 = TextBox.ConvertToPixels(paragraph.ParagraphProps.RightIndent, this.Dpi);
				int num5 = TextBox.ConvertToPixels(paragraph.ParagraphProps.HangingIndent, this.Dpi);
				bool flag = this.m_rttextbox.TextBoxProps.Direction == RPLFormat.Directions.LTR;
				if (num5 < 0)
				{
					if (flag)
					{
						num3 -= num5;
					}
					else
					{
						num4 -= num5;
					}
				}
				int listLevel = paragraph.ParagraphProps.ListLevel;
				if (listLevel > 0)
				{
					int num6 = listLevel * TextBox.ConvertToPixels(10.583333f, this.Dpi);
					if (flag)
					{
						num3 += num6;
					}
					else
					{
						num4 += num6;
					}
				}
				for (int i = 0; i < paragraph.TextLines.Count; i++)
				{
					TextLine textLine = paragraph.TextLines[i];
					int descent = textLine.GetDescent(hdc, this.FontCache);
					lineHeight = textLine.GetHeight(hdc, this.FontCache);
					lineYOffset = num2;
					num2 += lineHeight;
					RPLFormat.TextAlignments textAlignments = paragraph.ParagraphProps.Alignment;
					if (textAlignments == RPLFormat.TextAlignments.General)
					{
						textAlignments = this.m_rttextbox.TextBoxProps.DefaultAlignment;
						if (!flag)
						{
							switch (textAlignments)
							{
							case RPLFormat.TextAlignments.Left:
								textAlignments = RPLFormat.TextAlignments.Right;
								break;
							case RPLFormat.TextAlignments.Right:
								textAlignments = RPLFormat.TextAlignments.Left;
								break;
							}
						}
					}
					switch (textAlignments)
					{
					case RPLFormat.TextAlignments.Center:
						num = num3 + (this.WidthInPX - num3 - num4) / 2 - textLine.GetWidth(hdc, this.FontCache) / 2;
						break;
					case RPLFormat.TextAlignments.Right:
						num = this.WidthInPX - textLine.GetWidth(hdc, this.FontCache) - num4;
						break;
					default:
						num = num3;
						break;
					}
					if (textLine.FirstLine && num5 != 0)
					{
						if (flag)
						{
							switch (textAlignments)
							{
							case RPLFormat.TextAlignments.Left:
								num += num5;
								break;
							case RPLFormat.TextAlignments.Center:
								num += num5 / 2;
								break;
							}
						}
						else
						{
							switch (textAlignments)
							{
							case RPLFormat.TextAlignments.Right:
								num -= num5;
								break;
							case RPLFormat.TextAlignments.Center:
								num -= num5 / 2;
								break;
							}
						}
					}
					int count = textLine.VisualRuns.Count;
					for (int j = 0; j < count; num += textRun.GetWidth(hdc, this.FontCache), j++)
					{
						textRun = textLine.VisualRuns[j];
						if (textRun.TextRunProperties.IndexInParagraph == textRunIndex && textRunCharacterIndex >= textRun.CharacterIndexInOriginal)
						{
							bool flag2 = (moveCaretToNextLine || textRun.CharacterCount <= 0 || textRun.Text[textRun.CharacterCount - 1] != '\n') ? (textRunCharacterIndex <= textRun.CharacterIndexInOriginal + textRun.CharacterCount) : (textRunCharacterIndex < textRun.CharacterIndexInOriginal + textRun.CharacterCount);
							if (!flag2 && (i + 1 != paragraph.TextLines.Count || j + 1 != count))
							{
								continue;
							}
							if (moveCaretToNextLine && textRunCharacterIndex == textRun.CharacterIndexInOriginal + textRun.CharacterCount && j + 1 == count && i + 1 < paragraph.TextLines.Count)
							{
								location = location.Clone();
								if (paragraph.TextLines[i + 1].VisualRuns[0].TextRunProperties.IndexInParagraph != textRunIndex)
								{
									location.TextRunIndex++;
									location.TextRunCharacterIndex = 0;
								}
								Point paragraphAndRunCoordinates = this.GetParagraphAndRunCoordinates(hdc, location, false, out lineYOffset, out lineHeight, out textRun, out textRunCharacterIndex, out isFirstLine, out isLastLine);
								textRunCharacterIndex = Math.Max(textRunCharacterIndex - 1, 0);
								return paragraphAndRunCoordinates;
							}
							textRunCharacterIndex -= textRun.CharacterIndexInOriginal;
							isFirstLine = textLine.FirstLine;
							isLastLine = textLine.LastLine;
							return new Point(num, num2 - descent);
						}
					}
				}
				textRun = null;
				return Point.Empty;
			}
			return Point.Empty;
		}

		internal float GetHeight()
		{
			if (this.m_height == 0.0)
			{
				this.UpdateCachedObjects();
			}
			return this.m_height;
		}

		internal int GetHeightInPX()
		{
			return TextBox.ConvertToPixels(this.GetHeight(), this.Dpi);
		}

		internal void SetTextbox(TextBox textbox)
		{
			this.m_rttextbox = textbox;
			this.ResetCachedObjects();
		}

		private void ResetCachedObjects()
		{
			this.m_cachedrtparagraphs = null;
			this.m_height = 0f;
		}

		private void UpdateCachedObjects()
		{
			using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
			{
				this.UpdateCachedObjects(g);
			}
		}

		private void UpdateCachedObjects(Graphics g)
		{
			using (new PerfCheck("UpdateCachedObjects"))
			{
				using (new PerfCheck("ScriptItemize"))
				{
					this.RTTextbox.ScriptItemize();
				}
				using (new PerfCheck("Flow"))
				{
					FlowContext flowContext = new FlowContext(this.Width, this.FlowHeight);
					flowContext.LineLimit = this.LineLimit;
					flowContext.CharTrimLastLine = this.CharTrimLastLine;
					this.m_cachedrtparagraphs = LineBreaker.Flow(this.RTTextbox, g, this.FontCache, flowContext, true, out this.m_height);
				}
			}
		}

		public void Dispose()
		{
			if (this.m_fontCache != null)
			{
				this.m_fontCache.Dispose();
				this.m_fontCache = null;
			}
			GC.SuppressFinalize(this);
		}
	}
}
