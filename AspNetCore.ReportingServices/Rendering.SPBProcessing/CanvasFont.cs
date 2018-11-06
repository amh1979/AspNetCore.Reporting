using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RichText;
using System;
using System.Drawing;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal class CanvasFont : IDisposable
	{
		private Font m_gdiFont;

		private StringFormat m_stringFormat;

		private bool m_writingModeTopBottom;

		internal Font GDIFont
		{
			get
			{
				return this.m_gdiFont;
			}
		}

		internal StringFormat TrimStringFormat
		{
			get
			{
				return this.m_stringFormat;
			}
		}

		internal bool WritingModeTopBottom
		{
			get
			{
				return this.m_writingModeTopBottom;
			}
		}

		internal CanvasFont(CanvasFont copyFont)
		{
			this.m_gdiFont = copyFont.GDIFont;
			this.m_stringFormat = copyFont.TrimStringFormat;
			this.m_writingModeTopBottom = copyFont.WritingModeTopBottom;
		}

		internal CanvasFont(string family, ReportSize size, FontStyles style, FontWeights weight, TextDecorations decoration, TextAlignments alignment, VerticalAlignments verticalAlignment, Directions direction, WritingModes writingMode)
		{
			this.CreateFont(family, size, style, weight, decoration, alignment, verticalAlignment, direction, writingMode);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.m_gdiFont != null)
				{
					this.m_gdiFont.Dispose();
					this.m_gdiFont = null;
				}
				if (this.m_stringFormat != null)
				{
					this.m_stringFormat.Dispose();
					this.m_stringFormat = null;
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void CreateFont(string family, ReportSize size, FontStyles style, FontWeights weight, TextDecorations decoration, TextAlignments alignment, VerticalAlignments verticalAlignment, Directions direction, WritingModes writingMode)
		{
			this.CreateGDIFont(family, size, style, weight, decoration);
			StringAlignment textStringAlignment = this.CreateTextStringAlignment(alignment);
			bool directionRightToLeft = false;
			if (direction == Directions.RTL)
			{
				directionRightToLeft = true;
			}
			this.SetWritingMode(writingMode);
			StringAlignment lineStringAlignment = this.CreateLineStringAlignment(verticalAlignment);
			this.CreateFormatString(textStringAlignment, lineStringAlignment, directionRightToLeft);
		}

		private StringAlignment CreateLineStringAlignment(VerticalAlignments verticalAlignment)
		{
			switch (verticalAlignment)
			{
			case VerticalAlignments.Middle:
				return StringAlignment.Center;
			case VerticalAlignments.Bottom:
				if (this.m_writingModeTopBottom)
				{
					return StringAlignment.Near;
				}
				return StringAlignment.Far;
			default:
				if (this.m_writingModeTopBottom)
				{
					return StringAlignment.Far;
				}
				return StringAlignment.Near;
			}
		}

		internal void NewFormatStrings(bool newFormatStrings)
		{
			if (newFormatStrings)
			{
				this.m_stringFormat = new StringFormat(this.m_stringFormat);
			}
		}

		internal void SetWritingMode(WritingModes writingMode)
		{
			if (writingMode != WritingModes.Vertical && writingMode != WritingModes.Rotate270)
			{
				return;
			}
			this.m_writingModeTopBottom = true;
		}

		internal void SetTextStringAlignment(TextAlignments alignment, bool newFormatStrings)
		{
			StringAlignment alignment2 = this.CreateTextStringAlignment(alignment);
			this.NewFormatStrings(newFormatStrings);
			this.m_stringFormat.Alignment = alignment2;
		}

		internal void SetLineStringAlignment(VerticalAlignments verticalAlignment, bool newFormatStrings)
		{
			StringAlignment lineAlignment = this.CreateLineStringAlignment(verticalAlignment);
			this.NewFormatStrings(newFormatStrings);
			this.m_stringFormat.LineAlignment = lineAlignment;
		}

		internal void SetFormatFlags(Directions direction, bool setWritingMode, bool newFormatStrings)
		{
			bool directionRightToLeft = false;
			if (direction == Directions.RTL)
			{
				directionRightToLeft = true;
			}
			this.NewFormatStrings(newFormatStrings);
			StringFormatFlags formatFlags = this.m_stringFormat.FormatFlags;
			this.UpdateFormatFlags(ref formatFlags, setWritingMode, directionRightToLeft);
			this.m_stringFormat.FormatFlags = formatFlags;
		}

		private StringAlignment CreateTextStringAlignment(TextAlignments alignment)
		{
			StringAlignment stringAlignment = StringAlignment.Near;
			switch (alignment)
			{
			case TextAlignments.Center:
				return StringAlignment.Center;
			case TextAlignments.Right:
				return StringAlignment.Far;
			default:
				return StringAlignment.Near;
			}
		}

		internal void CreateGDIFont(string family, ReportSize size, FontStyles style, FontWeights weight, TextDecorations decoration)
		{
			double num = 12.0;
			bool flag = this.IsBold(weight);
			bool flag2 = style == FontStyles.Italic;
			if (size != null)
			{
				num = size.ToPoints();
			}
			string fontFamilyName = "Arial";
			if (family != null)
			{
				fontFamilyName = family;
			}
			bool lineThrough = false;
			bool underLine = false;
			switch (decoration)
			{
			case TextDecorations.Underline:
				underLine = true;
				break;
			case TextDecorations.LineThrough:
				lineThrough = true;
				break;
			}
			this.m_gdiFont = FontCache.CreateGdiPlusFont(fontFamilyName, (float)num, ref flag, ref flag2, lineThrough, underLine);
		}

		private void CreateFormatString(StringAlignment textStringAlignment, StringAlignment lineStringAlignment, bool directionRightToLeft)
		{
			this.m_stringFormat = new StringFormat(StringFormat.GenericDefault);
			this.m_stringFormat.Alignment = textStringAlignment;
			this.m_stringFormat.LineAlignment = lineStringAlignment;
			this.m_stringFormat.Trimming = StringTrimming.Word;
			StringFormatFlags stringFormatFlags = this.m_stringFormat.FormatFlags;
			stringFormatFlags &= ~StringFormatFlags.NoWrap;
			stringFormatFlags |= StringFormatFlags.LineLimit;
			this.UpdateFormatFlags(ref stringFormatFlags, true, directionRightToLeft);
			this.m_stringFormat.FormatFlags = stringFormatFlags;
		}

		private void UpdateFormatFlags(ref StringFormatFlags formatFlags, bool setWritingMode, bool directionRightToLeft)
		{
			if (setWritingMode)
			{
				if (this.m_writingModeTopBottom)
				{
					formatFlags |= StringFormatFlags.DirectionVertical;
				}
				else
				{
					formatFlags &= ~StringFormatFlags.DirectionVertical;
				}
			}
			if (directionRightToLeft)
			{
				formatFlags |= StringFormatFlags.DirectionRightToLeft;
			}
			else
			{
				formatFlags &= ~StringFormatFlags.DirectionRightToLeft;
			}
		}

		private bool IsBold(FontWeights fontWeight)
		{
			if (fontWeight != FontWeights.Bold && fontWeight != FontWeights.ExtraBold && fontWeight != FontWeights.Heavy)
			{
				return false;
			}
			return true;
		}
	}
}
