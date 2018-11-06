using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal sealed class TextBox
	{
		internal const float PrefixIndent = 10.583333f;

		internal const float PrefixSpace = 4.233333f;

		internal const float INCH_TO_MILLIMETER = 25.4f;

		internal const float RoundDelta = 0.1f;

		private List<Paragraph> m_paragraphs;

		private ITextBoxProps m_textBoxProps;

		internal List<Paragraph> Paragraphs
		{
			get
			{
				return this.m_paragraphs;
			}
			set
			{
				this.m_paragraphs = value;
			}
		}

		internal ITextBoxProps TextBoxProps
		{
			get
			{
				return this.m_textBoxProps;
			}
		}

		internal bool VerticalText
		{
			get
			{
				if (this.m_textBoxProps.WritingMode != RPLFormat.WritingModes.Vertical)
				{
					return this.m_textBoxProps.WritingMode == RPLFormat.WritingModes.Rotate270;
				}
				return true;
			}
		}

		internal bool HorizontalText
		{
			get
			{
				return this.m_textBoxProps.WritingMode == RPLFormat.WritingModes.Horizontal;
			}
		}

		internal string Value
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < this.m_paragraphs.Count; i++)
				{
					Paragraph paragraph = this.m_paragraphs[i];
					for (int j = 0; j < paragraph.Runs.Count; j++)
					{
						TextRun textRun = paragraph.Runs[j];
						stringBuilder.Append(textRun.Text);
					}
				}
				return stringBuilder.ToString();
			}
		}

		private TextBox()
		{
		}

		internal TextBox(ITextBoxProps textBoxProps)
		{
			this.m_textBoxProps = textBoxProps;
		}

		internal static float MeasureFullHeight(TextBox textBox, Graphics g, FontCache fontCache, FlowContext flowContext, out float contentHeight)
		{
			if (flowContext.Width <= 0.0)
			{
				contentHeight = 0f;
				return 0f;
			}
			FlowContext flowContext2 = flowContext.Clone();
			flowContext2.Reset();
			flowContext2.Height = 3.40282347E+38f;
			float num = 0f;
			if (textBox.VerticalText)
			{
				flowContext2.LineLimit = false;
				float num2 = 0f;
				float val = 0f;
				float num3 = 0f;
				float num4 = flowContext.Height;
				float num5 = 0f;
				if (flowContext.Height == 3.4028234663852886E+38)
				{
					num4 = 0f;
				}
				num3 = (contentHeight = LineBreaker.FlowVertical(textBox, g, fontCache, flowContext2, out num2, out val));
				num = num2;
				num5 = num2;
				bool flag = flowContext2.CharTrimmedRunWidth > 0;
				float num6 = 0f;
				while (num3 < flowContext.Width && num2 > num4)
				{
					flowContext2.Reset();
					flowContext2.Height = Math.Max(val, num4);
					flowContext2.Width = 3.40282347E+38f;
					num3 = LineBreaker.FlowVertical(textBox, g, fontCache, flowContext2, out num2, out val);
					if (num3 < flowContext.Width)
					{
						num6 = num3 - contentHeight;
						if (num3 <= contentHeight || num6 <= 0.10000000149011612)
						{
							if (flag)
							{
								contentHeight = num3;
								num = flowContext2.Height;
								break;
							}
							if (flowContext2.CharTrimmedRunWidth <= 0)
							{
								break;
							}
						}
						contentHeight = num3;
						num = flowContext2.Height;
						num5 = num2;
					}
					else
					{
						num6 = num - flowContext2.Height;
						if (!(num6 > 0.10000000149011612))
						{
							break;
						}
						val = (float)(flowContext2.Height + (num - flowContext2.Height) / 2.0);
						contentHeight = num3;
						num2 = num5;
					}
					flag = (flowContext2.CharTrimmedRunWidth > 0);
				}
			}
			else
			{
				LineBreaker.Flow(textBox, g, fontCache, flowContext2, false, out contentHeight);
				num = contentHeight;
			}
			return num;
		}

		internal static void Render(TextBox textBox, List<Paragraph> paragraphs, Graphics g, FontCache fontCache, PointF offset, RectangleF layoutRectangle)
		{
			float dpiX = g.DpiX;
			Win32DCSafeHandle win32DCSafeHandle = new Win32DCSafeHandle(g.GetHdc(), false);
			try
			{
				TextBox.Render(textBox, paragraphs, win32DCSafeHandle, fontCache, offset, layoutRectangle, dpiX);
			}
			finally
			{
				fontCache.ResetGraphics();
				if (!win32DCSafeHandle.IsInvalid)
				{
					g.ReleaseHdc();
				}
			}
		}

		internal static void Render(TextBox textBox, List<Paragraph> paragraphs, Win32DCSafeHandle hdc, FontCache fontCache, PointF offset, RectangleF layoutRectangle, float dpiX)
		{
			TextBox.Render(textBox, paragraphs, hdc, fontCache, offset, layoutRectangle, dpiX, true);
		}

		internal static void Render(TextBox textBox, List<Paragraph> paragraphs, Win32DCSafeHandle hdc, FontCache fontCache, PointF offset, RectangleF layoutRectangle, float dpiX, bool unitsInMM)
		{
			if (paragraphs != null && paragraphs.Count != 0)
			{
				Rectangle layoutRectangle2;
				Point point;
				if (unitsInMM)
				{
					layoutRectangle2 = new Rectangle(TextBox.ConvertToPixels(layoutRectangle.X, dpiX), TextBox.ConvertToPixels(layoutRectangle.Y, dpiX), TextBox.ConvertToPixels(layoutRectangle.Width, dpiX), TextBox.ConvertToPixels(layoutRectangle.Height, dpiX));
					point = new Point(TextBox.ConvertToPixels(offset.X, dpiX), TextBox.ConvertToPixels(offset.Y, dpiX));
				}
				else
				{
					layoutRectangle2 = new Rectangle((int)layoutRectangle.X, (int)layoutRectangle.Y, (int)layoutRectangle.Width, (int)layoutRectangle.Height);
					point = new Point((int)offset.X, (int)offset.Y);
				}
				uint fMode = Win32.SetTextAlign(hdc, 24u);
				int iBkMode = Win32.SetBkMode(hdc, 1);
				Win32ObjectSafeHandle win32ObjectSafeHandle = Win32.SelectObject(hdc, Win32ObjectSafeHandle.Zero);
				try
				{
					fontCache.WritingMode = textBox.TextBoxProps.WritingMode;
					int y = point.Y;
					for (int i = 0; i < paragraphs.Count; i++)
					{
						TextBox.RenderParagraph(textBox, paragraphs[i], hdc, fontCache, point.X, ref y, layoutRectangle2, dpiX);
					}
				}
				finally
				{
					fMode = Win32.SetTextAlign(hdc, fMode);
					iBkMode = Win32.SetBkMode(hdc, iBkMode);
					if (!win32ObjectSafeHandle.IsInvalid)
					{
						Win32ObjectSafeHandle win32ObjectSafeHandle2 = Win32.SelectObject(hdc, win32ObjectSafeHandle);
						win32ObjectSafeHandle2.SetHandleAsInvalid();
						win32ObjectSafeHandle.SetHandleAsInvalid();
					}
				}
			}
		}

		private static void RenderParagraph(TextBox textBox, Paragraph paragraph, Win32DCSafeHandle hdc, FontCache fontCache, int offsetX, ref int offsetY, Rectangle layoutRectangle, float dpiX)
		{
			List<TextLine> textLines = paragraph.TextLines;
			IParagraphProps paragraphProps = paragraph.ParagraphProps;
			bool flag = textBox.TextBoxProps.Direction == RPLFormat.Directions.LTR;
			RPLFormat.TextAlignments textAlignments = paragraphProps.Alignment;
			if (textAlignments == RPLFormat.TextAlignments.General)
			{
				textAlignments = textBox.TextBoxProps.DefaultAlignment;
				if (!flag)
				{
					switch (textAlignments)
					{
					case RPLFormat.TextAlignments.Right:
						textAlignments = RPLFormat.TextAlignments.Left;
						break;
					case RPLFormat.TextAlignments.Left:
						textAlignments = RPLFormat.TextAlignments.Right;
						break;
					}
				}
			}
			int num = TextBox.ConvertToPixels(paragraphProps.LeftIndent, dpiX);
			int num2 = TextBox.ConvertToPixels(paragraphProps.RightIndent, dpiX);
			int num3 = TextBox.ConvertToPixels(paragraphProps.HangingIndent, dpiX);
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
			if (paragraphProps.ListLevel > 0)
			{
				int num4 = paragraphProps.ListLevel * TextBox.ConvertToPixels(10.583333f, dpiX);
				if (flag)
				{
					num += num4;
				}
				else
				{
					num2 += num4;
				}
			}
			if (textLines == null || textLines.Count == 0)
			{
				offsetY += TextBox.ConvertToPixels(paragraphProps.SpaceBefore, dpiX);
				offsetY += TextBox.ConvertToPixels(paragraphProps.SpaceAfter, dpiX);
			}
			else
			{
				Graphics graphics = null;
				try
				{
					for (int i = 0; i < textLines.Count; i++)
					{
						TextLine textLine = textLines[i];
						int ascent = textLine.GetAscent(hdc, fontCache);
						textLine.GetDescent(hdc, fontCache);
						int height = textLine.GetHeight(hdc, fontCache);
						if (textLine.FirstLine)
						{
							offsetY += TextBox.ConvertToPixels(paragraphProps.SpaceBefore, dpiX);
						}
						int baselineY = offsetY + ascent;
						offsetY += height;
						int num5;
						switch (textAlignments)
						{
						case RPLFormat.TextAlignments.Left:
							num5 = num;
							break;
						case RPLFormat.TextAlignments.Center:
						{
							int num6 = 0;
							num6 = ((!textBox.HorizontalText) ? layoutRectangle.Height : layoutRectangle.Width);
							num5 = num + (num6 - num - num2) / 2 - textLine.GetWidth(hdc, fontCache) / 2;
							break;
						}
						default:
							num5 = ((!textBox.HorizontalText) ? (layoutRectangle.Height - num2 - textLine.GetWidth(hdc, fontCache)) : (layoutRectangle.Width - num2 - textLine.GetWidth(hdc, fontCache)));
							break;
						}
						if (textLine.Prefix != null && textLine.Prefix.Count > 0)
						{
							int num7 = (!flag) ? (num5 + textLine.GetWidth(hdc, fontCache) + TextBox.ConvertToPixels(4.233333f, dpiX)) : (num5 - TextBox.ConvertToPixels(4.233333f, dpiX) - textLine.GetPrefixWidth(hdc, fontCache));
							if (num3 < 0)
							{
								if (flag && textAlignments == RPLFormat.TextAlignments.Left)
								{
									num7 += num3;
								}
								else if (!flag && textAlignments == RPLFormat.TextAlignments.Right)
								{
									num7 -= num3;
								}
							}
							for (int j = 0; j < textLine.Prefix.Count; j++)
							{
								TextRun textRun = textLine.Prefix[j];
								textBox.TextBoxProps.DrawTextRun(textRun, paragraph, hdc, dpiX, fontCache, num7, offsetY, baselineY, height, layoutRectangle);
								num7 += textRun.GetWidth(hdc, fontCache);
							}
						}
						if (textLine.FirstLine && num3 != 0)
						{
							if (flag)
							{
								switch (textAlignments)
								{
								case RPLFormat.TextAlignments.Left:
									num5 += num3;
									break;
								case RPLFormat.TextAlignments.Center:
									num5 += num3 / 2;
									break;
								}
							}
							else
							{
								switch (textAlignments)
								{
								case RPLFormat.TextAlignments.Right:
									num5 -= num3;
									break;
								case RPLFormat.TextAlignments.Center:
									num5 -= num3 / 2;
									break;
								}
							}
						}
						int prevRunWidth = 0;
						int prevRunX = 0;
						TextRun prevRun = null;
						int count = textLine.VisualRuns.Count;
						for (int k = 0; k < count; k++)
						{
							TextRun textRun2 = textLine.VisualRuns[k];
							int width = textRun2.GetWidth(hdc, fontCache, k == count - 1);
							if (!textRun2.IsHighlightTextRun)
							{
								if (width > 0)
								{
									textBox.TextBoxProps.DrawTextRun(textRun2, paragraph, hdc, dpiX, fontCache, num5, offsetY, baselineY, height, layoutRectangle);
								}
							}
							else
							{
								bool flag2 = (flag && k + 1 == count) || (!flag && k == 0);
								if (width > 0 || flag2)
								{
									if (graphics == null)
									{
										graphics = Graphics.FromHdc(hdc.Handle);
									}
									TextBox.RenderHighlightedTextRun(textBox, paragraph, textRun2, prevRun, hdc, graphics, fontCache, dpiX, num5, offsetY, baselineY, height, layoutRectangle, width, prevRunWidth, prevRunX, flag2, textLine.LastLine);
								}
							}
							prevRunX = num5;
							prevRunWidth = width;
							num5 += width;
							prevRun = textRun2;
						}
						if (textLine.LastLine)
						{
							offsetY += TextBox.ConvertToPixels(paragraphProps.SpaceAfter, dpiX);
						}
					}
				}
				finally
				{
					if (graphics != null)
					{
						graphics.Dispose();
						graphics = null;
					}
				}
			}
		}

		private static void RenderHighlightedTextRun(TextBox textBox, Paragraph paragraph, TextRun run, TextRun prevRun, Win32DCSafeHandle hdc, Graphics g, FontCache fontCache, float dpiX, int x, int offsetY, int baselineY, int lineHeight, Rectangle layoutRectangle, int runWidth, int prevRunWidth, int prevRunX, bool lastRunInLine, bool lastLineInParagraph)
		{
			uint? nullable = null;
			Rectangle? nullable2 = null;
			bool flag = false;
			Color color = run.HighlightColor;
			int num;
			int num2;
			bool flag2;
			if (!color.IsEmpty)
			{
				num = ((run.HighlightStart >= 0) ? run.HighlightStart : 0);
				num2 = ((run.HighlightEnd >= 0) ? run.HighlightEnd : runWidth);
				if (lastRunInLine)
				{
					flag2 = (run.ScriptAnalysis.fLayoutRTL == 1);
					if (lastLineInParagraph && runWidth != 0)
					{
						if (num != num2)
						{
							if (flag2 && run.HighlightStart < 0)
							{
								goto IL_0094;
							}
							if (!flag2 && run.HighlightEnd < 0)
							{
								goto IL_0094;
							}
						}
					}
					else if (runWidth == 0)
					{
						if (flag2)
						{
							num -= 5;
						}
						else
						{
							num2 += 5;
						}
					}
				}
				goto IL_0102;
			}
			goto IL_024e;
			IL_024e:
			if (runWidth > 0)
			{
				textBox.TextBoxProps.DrawTextRun(run, paragraph, hdc, dpiX, fontCache, x, offsetY, baselineY, lineHeight, layoutRectangle);
				if (nullable.HasValue)
				{
					textBox.TextBoxProps.DrawClippedTextRun(run, paragraph, hdc, dpiX, fontCache, x, offsetY, baselineY, lineHeight, layoutRectangle, nullable.Value, nullable2.Value);
				}
			}
			if (flag)
			{
				Rectangle empty = Rectangle.Empty;
				empty = ((!textBox.HorizontalText) ? new Rectangle(layoutRectangle.Right - offsetY, layoutRectangle.Y + x, lineHeight, prevRunWidth) : new Rectangle(layoutRectangle.X + x, layoutRectangle.Y + offsetY - lineHeight, prevRunWidth, lineHeight));
				Color color2 = prevRun.TextRunProperties.Color;
				if (run.AllowColorInversion && TextBox.NeedsColorInversion(color, color2))
				{
					color2 = TextBox.InvertColor(color2);
					nullable = (uint)(color2.B << 16 | color2.G << 8 | color2.R);
				}
				else
				{
					nullable = prevRun.ColorInt;
				}
				textBox.TextBoxProps.DrawClippedTextRun(prevRun, paragraph, hdc, dpiX, fontCache, prevRunX, offsetY, baselineY, lineHeight, layoutRectangle, nullable.Value, empty);
			}
			return;
			IL_0094:
			if (flag2)
			{
				num -= 5;
				int abcA = run.GetGlyphData(hdc, fontCache).ABC.abcA;
				if (abcA < 0)
				{
					num += abcA;
				}
			}
			else
			{
				num2 += 5;
				int abcC = run.GetGlyphData(hdc, fontCache).ABC.abcC;
				if (abcC < 0)
				{
					num2 -= abcC;
				}
			}
			goto IL_0102;
			IL_0102:
			if (num != num2)
			{
				if (num == 0 && prevRun != null && (prevRun.GetGlyphData(hdc, fontCache).ABC.abcC <= 0 || run.GetGlyphData(hdc, fontCache).ABC.abcA < 0))
				{
					flag = true;
				}
				if (run.AllowColorInversion && TextBox.NeedsColorInversion(color, textBox.TextBoxProps.BackgroundColor))
				{
					color = TextBox.InvertColor(textBox.TextBoxProps.BackgroundColor);
				}
				using (Brush brush = new SolidBrush(color))
				{
					nullable2 = ((!textBox.HorizontalText) ? new Rectangle?(new Rectangle(layoutRectangle.Right - offsetY, layoutRectangle.Y + x + num, lineHeight, num2 - num)) : new Rectangle?(new Rectangle(layoutRectangle.X + x + num, layoutRectangle.Y + offsetY - lineHeight, num2 - num, lineHeight)));
					g.FillRectangle(brush, nullable2.Value);
				}
				if (run.AllowColorInversion && TextBox.NeedsColorInversion(color, run.TextRunProperties.Color))
				{
					Color color3 = TextBox.InvertColor(run.TextRunProperties.Color);
					nullable = (uint)(color3.B << 16 | color3.G << 8 | color3.R);
				}
			}
			run.HighlightColor = Color.Empty;
			goto IL_024e;
		}

		private static bool NeedsColorInversion(Color color1, Color color2)
		{
			if (!color2.IsEmpty && !(color2 == Color.Transparent))
			{
				int num = Math.Abs(color1.R - color2.R) + Math.Abs(color1.G - color2.G) + Math.Abs(color1.B - color2.B);
				return num < 192;
			}
			return false;
		}

		private static Color InvertColor(Color color)
		{
			int num = 255 - color.R;
			if (num >= 118 && num <= 138)
			{
				num = 117;
			}
			int num2 = 255 - color.G;
			if (num2 >= 118 && num2 <= 138)
			{
				num2 = 117;
			}
			int num3 = 255 - color.B;
			if (num3 >= 118 && num3 <= 138)
			{
				num3 = 117;
			}
			return Color.FromArgb(color.A, num, num2, num3);
		}

		internal static void DrawTextRun(TextRun run, Win32DCSafeHandle hdc, FontCache fontCache, int x, int baselineY, Underline underline)
		{
			uint crColor = 0u;
			try
			{
				uint colorInt = run.ColorInt;
				if (underline != null)
				{
					underline.Draw(hdc, (int)((double)run.UnderlineHeight * 0.085), colorInt);
				}
				crColor = Win32.SetTextColor(hdc, colorInt);
				GlyphData glyphData = run.GetGlyphData(hdc, fontCache);
				GlyphShapeData glyphScriptShapeData = glyphData.GlyphScriptShapeData;
				CachedFont cachedFont = run.GetCachedFont(hdc, fontCache);
				fontCache.SelectFontObject(hdc, cachedFont.Hfont);
				int num = Win32.ScriptTextOut(hdc, ref cachedFont.ScriptCache, x, baselineY, 4u, IntPtr.Zero, ref run.SCRIPT_ANALYSIS, IntPtr.Zero, 0, glyphScriptShapeData.Glyphs, glyphScriptShapeData.GlyphCount, glyphData.Advances, (int[])null, glyphData.GOffsets);
				if (Win32.Failed(num))
				{
					Marshal.ThrowExceptionForHR(num);
				}
			}
			finally
			{
				crColor = Win32.SetTextColor(hdc, crColor);
			}
		}

		internal static void ExtDrawTextRun(TextRun run, Win32DCSafeHandle hdc, FontCache fontCache, int x, int baselineY, Underline underline)
		{
			uint crColor = 0u;
			try
			{
				uint colorInt = run.ColorInt;
				if (underline != null)
				{
					underline.Draw(hdc, (int)((double)run.UnderlineHeight * 0.085), colorInt);
				}
				crColor = Win32.SetTextColor(hdc, colorInt);
				CachedFont cachedFont = run.GetCachedFont(hdc, fontCache);
				fontCache.SelectFontObject(hdc, cachedFont.Hfont);
				int[] lpDx = null;
				uint fuOptions = 0u;
				if (run.ScriptAnalysis.fRTL == 1)
				{
					fuOptions = 128u;
				}
				if (!Win32.ExtTextOut(hdc, x, baselineY, fuOptions, IntPtr.Zero, run.Text, (uint)run.Text.Length, lpDx))
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
			}
			finally
			{
				crColor = Win32.SetTextColor(hdc, crColor);
			}
		}

		internal static void DrawClippedTextRun(TextRun run, Win32DCSafeHandle hdc, FontCache fontCache, int x, int baselineY, uint fontColorOverride, Rectangle clipRect, Underline underline)
		{
			uint crColor = 0u;
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				if (underline != null)
				{
					underline.Draw(hdc, (int)((double)run.UnderlineHeight * 0.085), fontColorOverride);
				}
				RECT rECT = default(RECT);
				rECT.left = clipRect.Left;
				rECT.right = clipRect.Right;
				rECT.top = clipRect.Top;
				rECT.bottom = clipRect.Bottom;
				crColor = Win32.SetTextColor(hdc, fontColorOverride);
				GlyphData glyphData = run.GetGlyphData(hdc, fontCache);
				GlyphShapeData glyphScriptShapeData = glyphData.GlyphScriptShapeData;
				CachedFont cachedFont = run.GetCachedFont(hdc, fontCache);
				fontCache.SelectFontObject(hdc, cachedFont.Hfont);
				intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(rECT));
				Marshal.StructureToPtr(rECT, intPtr, false);
				int num = Win32.ScriptTextOut(hdc, ref cachedFont.ScriptCache, x, baselineY, 4u, intPtr, ref run.SCRIPT_ANALYSIS, IntPtr.Zero, 0, glyphScriptShapeData.Glyphs, glyphScriptShapeData.GlyphCount, glyphData.Advances, (int[])null, glyphData.GOffsets);
				if (Win32.Failed(num))
				{
					Marshal.ThrowExceptionForHR(num);
				}
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
				crColor = Win32.SetTextColor(hdc, crColor);
			}
		}

		internal static int ConvertToPixels(float mm, float dpi)
		{
			if (mm == 3.4028234663852886E+38)
			{
				return 2147483647;
			}
			return Convert.ToInt32((double)dpi * 0.03937007874 * (double)mm);
		}

		internal static float ConvertToMillimeters(int pixels, float dpi)
		{
			if (dpi == 0.0)
			{
				return 3.40282347E+38f;
			}
			return (float)(1.0 / dpi * (float)pixels * 25.399999618530273);
		}

		internal static float ConvertToPoints(float pixels, float dpi)
		{
			if (dpi == 0.0)
			{
				return 3.40282347E+38f;
			}
			return (float)(pixels * 72.0 / dpi);
		}

		internal static bool IsWhitespaceControlChar(char c)
		{
			switch (c)
			{
			case '\n':
			case '\v':
			case '\f':
			case '\r':
			case '\u0085':
				return true;
			default:
				return false;
			}
		}

		internal void ScriptItemize()
		{
			RPLFormat.Directions direction = this.m_textBoxProps.Direction;
			for (int i = 0; i < this.m_paragraphs.Count; i++)
			{
				this.m_paragraphs[i].ScriptItemize(direction);
			}
		}
	}
}
