using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RichText;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class PDFWriter : WriterBase
	{
		internal const float GRID_CONVERSION = 2.834646f;

		private const string CRLF = "\r\n";

		private const string INTEGER_FORMAT = "#########0";

		private const string DECIMAL_FORMAT = "#########0.0##";

		private const int PNG_HEADER_SIZE = 8;

		private const string STRING_ITALIC = "Italic";

		private const string STRING_BOLD = "Bold";

		private int m_procSetId;

		private int m_contentsId;

		private List<string> m_pageContentsSection;

		private List<int> m_pageIds = new List<int>();

		private int m_pageId;

		private int m_pagesId = -1;

		private int m_infoId;

		private int m_rootId;

		private int m_outlinesId = -1;

		private RectangleF m_bounds = RectangleF.Empty;

		private Dictionary<int, PDFUriAction> m_actions = new Dictionary<int, PDFUriAction>();

		private Dictionary<string, PDFFont> m_fonts = new Dictionary<string, PDFFont>();

		private int m_nextObjectId = 1;

		private List<long> m_objectOffsets = new List<long>();

		private UnicodeEncoding m_unicodeEncoding = new UnicodeEncoding(true, false);

		private Encoding m_ansiEncoding = Encoding.GetEncoding(1252);

		private SizeF m_mediaBoxSize;

		private Dictionary<string, PDFImage> m_images = new Dictionary<string, PDFImage>();

		private List<int> m_fontsUsedInCurrentPage = new List<int>();

		private List<int> m_imagesUsedInCurrentPage = new List<int>();

		private MD5 m_md5Hasher;

		internal Dictionary<string, PDFPagePoint> DocumentMapLabelPoints;

		internal PDFLabel DocumentMapRootLabel;

		internal bool HumanReadablePDF;

		internal bool Test;

		internal bool PrintOnOpen;

		internal FontEmbedding EmbedFonts;

		private static readonly string m_assemblyVersionString;

		private static readonly Dictionary<string, string> m_internalFonts;

		private static readonly Dictionary<char, char> m_unicodeToWinAnsi;

		private int m_imageDpiX;

		private int m_imageDpiY;

		private static readonly Hashtable m_pdfDelimiterChars;

		private bool TestOutputEnabled
		{
			get
			{
				if (this.HumanReadablePDF)
				{
					return this.Test;
				}
				return false;
			}
		}

		private MD5 Md5Hasher
		{
			get
			{
				if (this.m_md5Hasher == null)
				{
					this.m_md5Hasher = MD5.Create();
				}
				return this.m_md5Hasher;
			}
		}

		internal override float HalfPixelWidthY
		{
			get
			{
				return (float)(SharedRenderer.ConvertToMillimeters(1, (float)this.m_imageDpiY) / 2.0);
			}
		}

		internal override float HalfPixelWidthX
		{
			get
			{
				return (float)(SharedRenderer.ConvertToMillimeters(1, (float)this.m_imageDpiX) / 2.0);
			}
		}

		static PDFWriter()
		{
			PDFWriter.m_unicodeToWinAnsi = new Dictionary<char, char>();
			PDFWriter.m_unicodeToWinAnsi.Add('Œ', '\u008c');
			PDFWriter.m_unicodeToWinAnsi.Add('œ', '\u009c');
			PDFWriter.m_unicodeToWinAnsi.Add('Š', '\u008a');
			PDFWriter.m_unicodeToWinAnsi.Add('š', '\u009a');
			PDFWriter.m_unicodeToWinAnsi.Add('Ÿ', '\u009f');
			PDFWriter.m_unicodeToWinAnsi.Add('Ž', '\u008e');
			PDFWriter.m_unicodeToWinAnsi.Add('ž', '\u009e');
			PDFWriter.m_unicodeToWinAnsi.Add('ƒ', '\u0083');
			PDFWriter.m_unicodeToWinAnsi.Add('\u02c6', '\u0088');
			PDFWriter.m_unicodeToWinAnsi.Add('\u02dc', '\u0098');
			PDFWriter.m_unicodeToWinAnsi.Add('–', '\u0096');
			PDFWriter.m_unicodeToWinAnsi.Add('—', '\u0097');
			PDFWriter.m_unicodeToWinAnsi.Add('‘', '\u0091');
			PDFWriter.m_unicodeToWinAnsi.Add('’', '\u0092');
			PDFWriter.m_unicodeToWinAnsi.Add('‚', '\u0082');
			PDFWriter.m_unicodeToWinAnsi.Add('“', '\u0093');
			PDFWriter.m_unicodeToWinAnsi.Add('”', '\u0094');
			PDFWriter.m_unicodeToWinAnsi.Add('„', '\u0084');
			PDFWriter.m_unicodeToWinAnsi.Add('†', '\u0086');
			PDFWriter.m_unicodeToWinAnsi.Add('‡', '\u0087');
			PDFWriter.m_unicodeToWinAnsi.Add('•', '\u0095');
			PDFWriter.m_unicodeToWinAnsi.Add('…', '\u0085');
			PDFWriter.m_unicodeToWinAnsi.Add('‰', '\u0089');
			PDFWriter.m_unicodeToWinAnsi.Add('‹', '\u008b');
			PDFWriter.m_unicodeToWinAnsi.Add('›', '\u009b');
			PDFWriter.m_unicodeToWinAnsi.Add('€', '\u0080');
			PDFWriter.m_unicodeToWinAnsi.Add('™', '\u0099');
			PDFWriter.m_internalFonts = new Dictionary<string, string>();
			PDFWriter.m_internalFonts.Add("Times New Roman", "Times-Roman");
			PDFWriter.m_internalFonts.Add("Times New Roman,Bold", "Times-Bold");
			PDFWriter.m_internalFonts.Add("Times New Roman,Italic", "Times-Italic");
			PDFWriter.m_internalFonts.Add("Times New Roman,BoldItalic", "Times-BoldItalic");
			PDFWriter.m_internalFonts.Add("Arial", "Helvetica");
			PDFWriter.m_internalFonts.Add("Arial,Bold", "Helvetica-Bold");
			PDFWriter.m_internalFonts.Add("Arial,Italic", "Helvetica-Oblique");
			PDFWriter.m_internalFonts.Add("Arial,BoldItalic", "Helvetica-BoldOblique");
			PDFWriter.m_internalFonts.Add("Courier New", "Courier");
			PDFWriter.m_internalFonts.Add("Courier New,Bold", "Courier-Bold");
			PDFWriter.m_internalFonts.Add("Courier New,Italic", "Courier-Oblique");
			PDFWriter.m_internalFonts.Add("Courier New,BoldItalic", "Courier-BoldOblique");
			PDFWriter.m_internalFonts.Add("Symbol", "Symbol");
			PDFWriter.m_pdfDelimiterChars = new Hashtable();
			PDFWriter.m_pdfDelimiterChars.Add('(', PDFWriter.ConvertReservedCharToASCII('('));
			PDFWriter.m_pdfDelimiterChars.Add(')', PDFWriter.ConvertReservedCharToASCII(')'));
			PDFWriter.m_pdfDelimiterChars.Add('<', PDFWriter.ConvertReservedCharToASCII('<'));
			PDFWriter.m_pdfDelimiterChars.Add('>', PDFWriter.ConvertReservedCharToASCII('>'));
			PDFWriter.m_pdfDelimiterChars.Add('[', PDFWriter.ConvertReservedCharToASCII('['));
			PDFWriter.m_pdfDelimiterChars.Add(']', PDFWriter.ConvertReservedCharToASCII(']'));
			PDFWriter.m_pdfDelimiterChars.Add('{', PDFWriter.ConvertReservedCharToASCII('{'));
			PDFWriter.m_pdfDelimiterChars.Add('}', PDFWriter.ConvertReservedCharToASCII('}'));
			PDFWriter.m_pdfDelimiterChars.Add('/', PDFWriter.ConvertReservedCharToASCII('/'));
			PDFWriter.m_pdfDelimiterChars.Add('%', PDFWriter.ConvertReservedCharToASCII('%'));
			PDFWriter.m_assemblyVersionString = typeof(PDFRenderer).Assembly.GetName().Version.ToString();
		}

		internal PDFWriter(Renderer renderer, Stream stream, bool disposeRenderer, CreateAndRegisterStream createAndRegisterStream, int imageDpiX, int imageDpiY)
			: base(renderer, stream, disposeRenderer, createAndRegisterStream)
		{
			this.m_objectOffsets.Add(0L);
			this.Write("%PDF-1.3");
			this.m_imageDpiX = imageDpiX;
			this.m_imageDpiY = imageDpiY;
		}

		internal override void BeginPage(float pageWidth, float pageHeight)
		{
			this.m_mediaBoxSize = new SizeF((float)(pageWidth * 2.8346459865570068), (float)(pageHeight * 2.8346459865570068));
			this.m_pageContentsSection = new List<string>();
			this.m_pageId = this.ReserveObjectId();
		}

		internal override void BeginPageSection(RectangleF bounds)
		{
			base.BeginPageSection(bounds);
			this.m_bounds = this.ConvertBoundsToPDFUnits(bounds);
			StringBuilder stringBuilder = new StringBuilder();
			PDFWriter.WriteClipBounds(stringBuilder, this.m_bounds.Left, this.m_bounds.Top - this.m_bounds.Height, this.m_bounds.Size);
			this.m_pageContentsSection.Add(stringBuilder.ToString());
		}

		internal override void BeginReport(int dpiX, int dpiY)
		{
			this.m_procSetId = this.WriteObject("[/PDF /Text /ImageB /ImageC /ImageI]");
			base.m_commonGraphics = new GraphicsBase((float)dpiX, (float)dpiY);
		}

		internal override RectangleF CalculateColumnBounds(RPLReportSection reportSection, RPLPageLayout pageLayout, RPLItemMeasurement column, int columnNumber, float top, float columnHeight, float columnWidth)
		{
			return HardPageBreakShared.CalculateColumnBounds(reportSection, pageLayout, columnNumber, top, columnHeight);
		}

		internal override RectangleF CalculateHeaderBounds(RPLReportSection reportSection, RPLPageLayout pageLayout, float top, float width)
		{
			return HardPageBreakShared.CalculateHeaderBounds(reportSection, pageLayout, top, width);
		}

		internal override RectangleF CalculateFooterBounds(RPLReportSection reportSection, RPLPageLayout pageLayout, float top, float width)
		{
			return HardPageBreakShared.CalculateFooterBounds(reportSection, pageLayout, top, width);
		}

		internal override void DrawBackgroundImage(RPLImageData imageData, RPLFormat.BackgroundRepeatTypes repeat, PointF start, RectangleF position)
		{
			PDFImage image = this.GetImage(imageData.ImageName, imageData.ImageData, imageData.ImageDataOffset, imageData.GDIImageProps);
			if (image != null)
			{
				SizeF size = new SizeF(this.ConvertPixelsToPDFUnits(image.GdiProperties.Width, (float)this.m_imageDpiX), this.ConvertPixelsToPDFUnits(image.GdiProperties.Height, (float)this.m_imageDpiY));
				StringBuilder stringBuilder = new StringBuilder();
				if (repeat == RPLFormat.BackgroundRepeatTypes.Clip)
				{
					RectangleF bounds = this.ConvertToPDFUnits(position);
					PDFWriter.WriteImage(stringBuilder, image.ImageId, bounds, bounds.Left + start.X, bounds.Top - size.Height + start.Y, size);
				}
				else
				{
					float num = SharedRenderer.ConvertToMillimeters(image.GdiProperties.Width, (float)this.m_imageDpiX);
					float num2 = SharedRenderer.ConvertToMillimeters(image.GdiProperties.Height, (float)this.m_imageDpiY);
					RectangleF bounds2 = this.ConvertToPDFUnits(position);
					float num3 = position.Width;
					if (repeat == RPLFormat.BackgroundRepeatTypes.RepeatY)
					{
						num3 = num;
					}
					float num4 = position.Height;
					if (repeat == RPLFormat.BackgroundRepeatTypes.RepeatX)
					{
						num4 = num2;
					}
					for (float num5 = start.X; num5 < num3; num5 += num)
					{
						for (float num6 = start.Y; num6 < num4; num6 += num2)
						{
							PointF pointF = this.ConvertToPDFUnits(position.Left + num5, position.Top + num6);
							PDFWriter.WriteImage(stringBuilder, image.ImageId, bounds2, pointF.X, pointF.Y - size.Height, size);
						}
					}
				}
				this.m_pageContentsSection.Add(stringBuilder.ToString());
			}
		}

		internal override void DrawLine(Color color, float size, RPLFormat.BorderStyles style, float x1, float y1, float x2, float y2)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("\r\n");
			PDFWriter.WriteColor(stringBuilder, color, true);
			PDFWriter.WriteSizeAndStyle(stringBuilder, size, style);
			PointF point = this.ConvertToPDFUnits(x1, y1);
			PDFWriter.Write(stringBuilder, point);
			stringBuilder.Append(" m ");
			point = this.ConvertToPDFUnits(x2, y2);
			PDFWriter.Write(stringBuilder, point);
			stringBuilder.Append(" l ");
			stringBuilder.Append("S");
			this.m_pageContentsSection.Add(stringBuilder.ToString());
		}

		internal override void DrawDynamicImage(string imageName, Stream imageStream, long imageDataOffset, RectangleF position)
		{
			byte[] array = null;
			if (imageStream != null)
			{
				array = new byte[(int)imageStream.Length];
				imageStream.Position = 0L;
				imageStream.Read(array, 0, (int)imageStream.Length);
			}
			PDFImage pDFImage = this.GetImage(imageName, array, imageDataOffset, null);
			bool flag = true;
			if (pDFImage == null)
			{
				pDFImage = this.GetDefaultImage();
				flag = false;
			}
			position = this.ConvertToPDFUnits(position);
			SizeF size = new SizeF(this.ConvertPixelsToPDFUnits(pDFImage.GdiProperties.Width, (float)this.m_imageDpiX), this.ConvertPixelsToPDFUnits(pDFImage.GdiProperties.Height, (float)this.m_imageDpiY));
			if (flag)
			{
				size.Width *= position.Width / size.Width;
				size.Height *= position.Height / size.Height;
			}
			StringBuilder stringBuilder = new StringBuilder();
			PDFWriter.WriteImage(stringBuilder, pDFImage.ImageId, position, size);
			this.m_pageContentsSection.Add(stringBuilder.ToString());
		}

		internal override void DrawImage(RectangleF position, RPLImage image, RPLImageProps instanceProperties, RPLImagePropsDef definitionProperties)
		{
			RPLImageData image2 = instanceProperties.Image;
			PDFImage pDFImage = this.GetImage(image2.ImageName, image2.ImageData, image2.ImageDataOffset, image2.GDIImageProps);
			RPLFormat.Sizings sizings = definitionProperties.Sizing;
			if (pDFImage == null)
			{
				pDFImage = this.GetDefaultImage();
				sizings = RPLFormat.Sizings.Clip;
			}
			position = this.ConvertToPDFUnits(position);
			SizeF size = new SizeF(this.ConvertPixelsToPDFUnits(pDFImage.GdiProperties.Width, (float)this.m_imageDpiX), this.ConvertPixelsToPDFUnits(pDFImage.GdiProperties.Height, (float)this.m_imageDpiY));
			StringBuilder stringBuilder = new StringBuilder();
			switch (sizings)
			{
			case RPLFormat.Sizings.AutoSize:
			case RPLFormat.Sizings.Clip:
				PDFWriter.WriteImage(stringBuilder, pDFImage.ImageId, position, position.Left, position.Y - size.Height, size);
				break;
			case RPLFormat.Sizings.FitProportional:
			{
				float num = position.Width / size.Width;
				float num2 = position.Height / size.Height;
				if (num > num2)
				{
					size.Width *= num2;
					size.Height *= num2;
				}
				else
				{
					size.Width *= num;
					size.Height *= num;
				}
				PDFWriter.WriteImage(stringBuilder, pDFImage.ImageId, position, position.Left, position.Y - size.Height, size);
				break;
			}
			default:
				size.Width = position.Width;
				size.Height = position.Height;
				PDFWriter.WriteImage(stringBuilder, pDFImage.ImageId, position, size);
				break;
			}
			this.m_pageContentsSection.Add(stringBuilder.ToString());
		}

		internal override void DrawRectangle(Color color, float size, RPLFormat.BorderStyles style, RectangleF rectangle)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("\r\n");
			PDFWriter.WriteColor(stringBuilder, color, true);
			PDFWriter.WriteSizeAndStyle(stringBuilder, size, style);
			rectangle = this.ConvertToPDFUnits(rectangle);
			PDFWriter.Write(stringBuilder, rectangle.Location);
			stringBuilder.Append(" m ");
			PDFWriter.Write(stringBuilder, rectangle.X);
			stringBuilder.Append(" ");
			PDFWriter.Write(stringBuilder, rectangle.Top - rectangle.Height);
			stringBuilder.Append(" l ");
			PDFWriter.Write(stringBuilder, rectangle.Left + rectangle.Width);
			stringBuilder.Append(" ");
			PDFWriter.Write(stringBuilder, rectangle.Top - rectangle.Height);
			stringBuilder.Append(" l ");
			PDFWriter.Write(stringBuilder, rectangle.Left + rectangle.Width);
			stringBuilder.Append(" ");
			PDFWriter.Write(stringBuilder, rectangle.Top);
			stringBuilder.Append(" l ");
			stringBuilder.Append("s");
			this.m_pageContentsSection.Add(stringBuilder.ToString());
		}

		private void WriteTextRunTestOutput(StringBuilder sb, ReportTextBox textBox, AspNetCore.ReportingServices.Rendering.RichText.TextRun run)
		{
			if (this.TestOutputEnabled)
			{
				ReportTextRun reportTextRun = (ReportTextRun)run.TextRunProperties;
				string uniqueName = reportTextRun.UniqueName;
				if (uniqueName == null)
				{
					uniqueName = textBox.UniqueName;
				}
				string comment = string.Format(CultureInfo.InvariantCulture, "#TextRun#UniqueName={0}", uniqueName);
				this.WriteComment(sb, comment);
			}
		}

		internal override void DrawTextRun(Win32DCSafeHandle hdc, FontCache fontCache, ReportTextBox textBox, AspNetCore.ReportingServices.Rendering.RichText.TextRun run, TypeCode typeCode, RPLFormat.TextAlignments textAlign, RPLFormat.VerticalAlignments verticalAlign, RPLFormat.WritingModes writingMode, RPLFormat.Directions direction, Point position, System.Drawing.Rectangle layoutRectangle, int lHeight, int baselineY)
		{
			if (!string.IsNullOrEmpty(run.Text))
			{
				ITextRunProps textRunProperties = run.TextRunProperties;
				bool flag = run.TextRunProperties.TextDecoration == RPLFormat.TextDecorations.Underline;
				bool flag2 = run.TextRunProperties.TextDecoration == RPLFormat.TextDecorations.LineThrough;
				bool flag3 = run.HasEastAsianChars && writingMode == RPLFormat.WritingModes.Vertical;
				PDFFont pDFFont = this.ProcessDrawStringFont(run, typeCode, textAlign, verticalAlign, writingMode, direction);
				PointF pointF;
				switch (writingMode)
				{
				case RPLFormat.WritingModes.Horizontal:
					pointF = new PointF(SharedRenderer.ConvertToMillimeters(layoutRectangle.Left, fontCache.Dpi), SharedRenderer.ConvertToMillimeters(layoutRectangle.Top, fontCache.Dpi));
					break;
				case RPLFormat.WritingModes.Vertical:
					pointF = new PointF(SharedRenderer.ConvertToMillimeters(layoutRectangle.Right, fontCache.Dpi), SharedRenderer.ConvertToMillimeters(layoutRectangle.Top, fontCache.Dpi));
					break;
				case RPLFormat.WritingModes.Rotate270:
					pointF = new PointF(SharedRenderer.ConvertToMillimeters(layoutRectangle.Left, fontCache.Dpi), SharedRenderer.ConvertToMillimeters(layoutRectangle.Bottom, fontCache.Dpi));
					break;
				default:
					throw new NotSupportedException();
				}
				PointF pointF2 = new PointF(SharedRenderer.ConvertToMillimeters(position.X, fontCache.Dpi), SharedRenderer.ConvertToMillimeters(position.Y, fontCache.Dpi));
				float num = SharedRenderer.ConvertToMillimeters(baselineY, fontCache.Dpi);
				float lineWidth = -3.40282347E+38f;
				if (flag || flag2)
				{
					lineWidth = (float)(SharedRenderer.ConvertToMillimeters(run.GetWidth(hdc, fontCache), fontCache.Dpi) * 2.8346459865570068);
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("\r\n");
				this.WriteTextRunTestOutput(stringBuilder, textBox, run);
				stringBuilder.Append("BT ");
				stringBuilder.Append("/F");
				stringBuilder.Append(pDFFont.FontId);
				stringBuilder.Append(" ");
				PDFWriter.Write(stringBuilder, textRunProperties.FontSize);
				stringBuilder.Append(" Tf ");
				PDFWriter.WriteColor(stringBuilder, textRunProperties.Color, false);
				float gridHeight = pDFFont.GridHeight;
				PDFWriter.Write(stringBuilder, gridHeight);
				stringBuilder.Append(" TL ");
				float num2 = (float)(this.m_bounds.Left + pointF.X * 2.8346459865570068);
				float num3 = (float)(this.m_bounds.Top - pointF.Y * 2.8346459865570068);
				if (writingMode == RPLFormat.WritingModes.Horizontal || flag3)
				{
					if (flag3)
					{
						float num4 = SharedRenderer.ConvertToMillimeters(run.GetAscent(hdc, fontCache), fontCache.Dpi);
						num2 = (float)(num2 - pointF2.Y * 2.8346459865570068);
						num3 = (float)(num3 - (pointF2.X * 2.8346459865570068 + num4 * 2.8346459865570068));
						if (pDFFont.SimulateItalic)
						{
							num3 = (float)(num3 + num4 * 0.16664999723434448 * 2.8346459865570068);
						}
					}
					else
					{
						num2 = (float)(num2 + pointF2.X * 2.8346459865570068);
						num3 = (float)(num3 - num * 2.8346459865570068);
					}
					PDFWriter.Write(stringBuilder, num2);
					stringBuilder.Append(" ");
					PDFWriter.Write(stringBuilder, num3);
					stringBuilder.Append(" Td ");
					if (pDFFont.SimulateItalic)
					{
						if (flag3)
						{
							stringBuilder.Append("1 -0.3333 0 1 ");
						}
						else
						{
							stringBuilder.Append("1 0 0.3333 1 ");
						}
						PDFWriter.Write(stringBuilder, num2);
						stringBuilder.Append(" ");
						PDFWriter.Write(stringBuilder, num3);
						stringBuilder.Append(" Tm ");
					}
				}
				else
				{
					if (writingMode == RPLFormat.WritingModes.Vertical)
					{
						num2 = (float)(num2 - num * 2.8346459865570068);
						num3 = (float)(num3 - pointF2.X * 2.8346459865570068);
					}
					else
					{
						num2 = (float)(num2 + num * 2.8346459865570068);
						num3 = (float)(num3 + pointF2.X * 2.8346459865570068);
					}
					if (pDFFont.SimulateItalic)
					{
						if (writingMode == RPLFormat.WritingModes.Vertical)
						{
							stringBuilder.Append("0 -1 1 -0.3333 ");
						}
						else
						{
							stringBuilder.Append("0 1 -1 0.3333 ");
						}
					}
					else if (writingMode == RPLFormat.WritingModes.Vertical)
					{
						stringBuilder.Append("0 -1 1 0 ");
					}
					else
					{
						stringBuilder.Append("0 1 -1 0 ");
					}
					PDFWriter.Write(stringBuilder, num2);
					stringBuilder.Append(" ");
					PDFWriter.Write(stringBuilder, num3);
					stringBuilder.Append(" Tm ");
				}
				if (pDFFont.SimulateBold)
				{
					stringBuilder.Append("2 Tr ");
					float value = (float)(run.TextRunProperties.FontSize / 35.0);
					PDFWriter.Write(stringBuilder, value);
					stringBuilder.Append(" w ");
					PDFWriter.WriteColor(stringBuilder, textRunProperties.Color, true);
				}
				string text = run.Text;
				if (!pDFFont.IsComposite)
				{
					text = PDFWriter.EscapeString(text);
				}
				if (flag3)
				{
					List<int> list = PDFWriter.WriteVerticallyStackedText(fontCache, run, stringBuilder, pDFFont);
					num2 = (float)(this.m_bounds.Left + pointF.X * 2.8346459865570068 - num * 2.8346459865570068);
					num3 = (float)(this.m_bounds.Top - pointF.Y * 2.8346459865570068 - pointF2.X * 2.8346459865570068);
					if (list != null)
					{
						if (pDFFont.SimulateItalic)
						{
							stringBuilder.Append("0 -1 1 -0.3333 ");
						}
						else
						{
							stringBuilder.Append("0 -1 1 0 ");
						}
						float num5 = 0f;
						if (!list.Contains(0))
						{
							float num6 = SharedRenderer.ConvertToMillimeters(run.GetAscent(hdc, fontCache), fontCache.Dpi);
							float num7 = SharedRenderer.ConvertToMillimeters(run.GlyphData.Advances[0], fontCache.Dpi);
							num5 = num6 - num7;
						}
						PDFWriter.Write(stringBuilder, num2);
						stringBuilder.Append(" ");
						PDFWriter.Write(stringBuilder, (float)(num3 - num5 * 2.8346459865570068));
						stringBuilder.Append(" Tm ");
						PDFWriter.WriteVerticalText(fontCache, run, stringBuilder, pDFFont, list);
					}
				}
				else
				{
					PDFWriter.WriteText(run, stringBuilder, pDFFont, text, this.HumanReadablePDF);
				}
				stringBuilder.Append("T* ET");
				if (pDFFont.SimulateBold)
				{
					stringBuilder.Append(" 0 Tr");
				}
				if (flag)
				{
					float lineHeight = (float)(0.125 * SharedRenderer.ConvertToMillimeters(run.UnderlineHeight, fontCache.Dpi));
					float lineOffset = pointF2.Y - num;
					this.DrawLine(stringBuilder, writingMode, num2, num3, lineOffset, lineHeight, lineWidth);
				}
				if (flag2)
				{
					float num8 = (float)(SharedRenderer.ConvertToMillimeters(run.GetAscent(hdc, fontCache), fontCache.Dpi) * 2.8346459865570068);
					float num9 = (float)(SharedRenderer.ConvertToMillimeters(run.GetDescent(hdc, fontCache), fontCache.Dpi) * 2.8346459865570068);
					float num10 = num8 + num9;
					float num11 = (float)(0.05000000074505806 * num10);
					float lineOffset2 = (float)(0.0 - (num8 - num10 / 2.0 - num11 / 2.0));
					this.DrawLine(stringBuilder, writingMode, num2, num3, lineOffset2, num11, lineWidth);
				}
				this.m_pageContentsSection.Add(stringBuilder.ToString());
			}
		}

		private void DrawLine(StringBuilder sb, RPLFormat.WritingModes writingMode, float textLineStart, float textBlockStart, float lineOffset, float lineHeight, float lineWidth)
		{
			sb.Append(" ");
			switch (writingMode)
			{
			case RPLFormat.WritingModes.Horizontal:
				PDFWriter.Write(sb, textLineStart);
				sb.Append(" ");
				PDFWriter.Write(sb, textBlockStart - lineOffset);
				sb.Append(" ");
				PDFWriter.Write(sb, lineWidth);
				sb.Append(" ");
				PDFWriter.Write(sb, (float)(0.0 - lineHeight));
				break;
			case RPLFormat.WritingModes.Vertical:
				PDFWriter.Write(sb, textLineStart - lineOffset);
				sb.Append(" ");
				PDFWriter.Write(sb, textBlockStart);
				sb.Append(" ");
				PDFWriter.Write(sb, (float)(0.0 - lineHeight));
				sb.Append(" ");
				PDFWriter.Write(sb, (float)(0.0 - lineWidth));
				break;
			default:
				PDFWriter.Write(sb, textLineStart + lineOffset);
				sb.Append(" ");
				PDFWriter.Write(sb, textBlockStart);
				sb.Append(" ");
				PDFWriter.Write(sb, lineHeight);
				sb.Append(" ");
				PDFWriter.Write(sb, lineWidth);
				break;
			}
			sb.Append(" re f");
		}

		internal override void EndPage()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.m_pageContentsSection.Count; i++)
			{
				stringBuilder.Append(this.m_pageContentsSection[i]);
			}
			string text = this.CompressString(stringBuilder.ToString());
			this.m_contentsId = this.ReserveObjectId();
			this.UpdateCrossRefPosition(this.m_contentsId);
			this.Write("\r\n");
			this.Write(this.m_contentsId);
			this.Write(" 0 obj");
			this.Write("\r\n");
			this.Write("<< /Length ");
			this.Write(text.Length);
			if (!this.HumanReadablePDF)
			{
				this.Write(" /Filter /FlateDecode");
			}
			this.Write(" >> stream");
			if (this.HumanReadablePDF)
			{
				this.Write(" ");
			}
			else
			{
				this.Write("\r\n");
			}
			this.Write(text);
			this.Write("\r\n");
			this.Write("endstream");
			this.Write("\r\n");
			this.Write("endobj");
			if (this.m_pagesId == -1)
			{
				this.m_pagesId = this.ReserveObjectId();
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append("<< /Type /Page /Parent ");
			stringBuilder2.Append(this.m_pagesId);
			stringBuilder2.Append(" 0 R /MediaBox [0 0 ");
			PDFWriter.Write(stringBuilder2, this.m_mediaBoxSize);
			stringBuilder2.Append("] /Contents ");
			stringBuilder2.Append(this.m_contentsId);
			stringBuilder2.Append(" 0 R");
			this.EndPage_WriteActions(stringBuilder2);
			stringBuilder2.Append(" /Resources << /ProcSet ");
			stringBuilder2.Append(this.m_procSetId);
			stringBuilder2.Append(" 0 R /XObject <<");
			for (int j = 0; j < this.m_imagesUsedInCurrentPage.Count; j++)
			{
				stringBuilder2.Append(" /Im");
				PDFWriter.Write(stringBuilder2, this.m_imagesUsedInCurrentPage[j]);
				stringBuilder2.Append(" ");
				PDFWriter.Write(stringBuilder2, this.m_imagesUsedInCurrentPage[j]);
				stringBuilder2.Append(" 0 R");
			}
			this.m_imagesUsedInCurrentPage.Clear();
			stringBuilder2.Append(" >> /Font <<");
			for (int k = 0; k < this.m_fontsUsedInCurrentPage.Count; k++)
			{
				stringBuilder2.Append(" /F");
				PDFWriter.Write(stringBuilder2, this.m_fontsUsedInCurrentPage[k]);
				stringBuilder2.Append(" ");
				PDFWriter.Write(stringBuilder2, this.m_fontsUsedInCurrentPage[k]);
				stringBuilder2.Append(" 0 R");
			}
			this.m_fontsUsedInCurrentPage.Clear();
			stringBuilder2.Append(" >> >> >>");
			this.WriteObject(this.m_pageId, stringBuilder2.ToString());
			this.m_pageIds.Add(this.m_pageId);
			foreach (string key in this.m_images.Keys)
			{
				PDFImage pDFImage = this.m_images[key];
				if (pDFImage.ImageData != null)
				{
					this.ProcessImage(pDFImage);
				}
			}
		}

		private void EndPage_WriteActions(StringBuilder sb)
		{
			if (this.m_actions != null && this.m_actions.Count > 0)
			{
				sb.Append(" /Annots [ ");
				foreach (int key in this.m_actions.Keys)
				{
					PDFUriAction pDFUriAction = this.m_actions[key];
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("<< /Border [0 0 0] /Subtype /Link /A << /URI (");
					stringBuilder.Append(PDFWriter.EscapeString(pDFUriAction.Uri));
					stringBuilder.Append(") /IsMap false /Type /Action /S /URI >> /Type /Annot /Rect [");
					PDFWriter.WriteRectangle(stringBuilder, pDFUriAction.Rectangle.Left, pDFUriAction.Rectangle.Top - pDFUriAction.Rectangle.Height, pDFUriAction.Rectangle.Right, pDFUriAction.Rectangle.Top);
					stringBuilder.Append("] >>");
					this.WriteObject(key, stringBuilder.ToString());
					PDFWriter.Write(sb, key);
					sb.Append(" 0 R ");
				}
				sb.Append("]");
				this.m_actions.Clear();
			}
		}

		internal override void EndPageSection()
		{
			this.m_pageContentsSection.Add("\r\nQ");
		}

		internal override void EndReport()
		{
			Dictionary<string, EmbeddedFont> dictionary = new Dictionary<string, EmbeddedFont>();
			foreach (string key in this.m_fonts.Keys)
			{
				this.ProcessFontForFontEmbedding(this.m_fonts[key], dictionary);
			}
			foreach (string key2 in dictionary.Keys)
			{
				this.WriteEmbeddedFont(dictionary[key2]);
				this.WriteToUnicodeMap(dictionary[key2]);
			}
			dictionary = null;
			foreach (string key3 in this.m_fonts.Keys)
			{
				this.WriteFont(this.m_fonts[key3]);
			}
			this.m_fonts = null;
			this.EndReport_WriteDocumentMap();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<< /Type /Pages /Kids [ ");
			for (int i = 0; i < this.m_pageIds.Count; i++)
			{
				stringBuilder.Append(this.m_pageIds[i]);
				stringBuilder.Append(" 0 R ");
			}
			stringBuilder.Append("] /Count ");
			stringBuilder.Append(this.m_pageIds.Count);
			stringBuilder.Append(" >>");
			this.WriteObject(this.m_pagesId, stringBuilder.ToString());
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append("<< /Type /Catalog /Pages ");
			PDFWriter.Write(stringBuilder2, this.m_pagesId);
			stringBuilder2.Append(" 0 R");
			if (this.m_outlinesId != -1)
			{
				stringBuilder2.Append(" /Outlines ");
				PDFWriter.Write(stringBuilder2, this.m_outlinesId);
				stringBuilder2.Append(" 0 R");
			}
			if (this.PrintOnOpen)
			{
				stringBuilder2.Append(" /OpenAction <</S /Named /N /Print>>");
			}
			stringBuilder2.Append(" >>");
			this.m_rootId = this.WriteObject(stringBuilder2.ToString());
			StringBuilder stringBuilder3 = new StringBuilder();
			stringBuilder3.Append("<< /Title ");
			this.WriteUnicodeString(stringBuilder3, base.m_renderer.RplReport.ReportName);
			stringBuilder3.Append("\r\n");
			stringBuilder3.Append("/Author ");
			this.WriteUnicodeString(stringBuilder3, base.m_renderer.RplReport.Author);
			stringBuilder3.Append("\r\n");
			stringBuilder3.Append("/Subject ");
			this.WriteUnicodeString(stringBuilder3, base.m_renderer.RplReport.Description);
			stringBuilder3.Append("\r\n");
			stringBuilder3.Append("/Creator (Microsoft Reporting Services ");
			stringBuilder3.Append(PDFWriter.m_assemblyVersionString);
			stringBuilder3.Append(")");
			stringBuilder3.Append("\r\n");
			stringBuilder3.Append("/Producer (Microsoft Reporting Services PDF Rendering Extension ");
			stringBuilder3.Append(PDFWriter.m_assemblyVersionString);
			stringBuilder3.Append(")");
			stringBuilder3.Append("\r\n");
			stringBuilder3.Append("/CreationDate (D:");
			DateTime now = DateTime.Now;
			TimeSpan utcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(now);
			stringBuilder3.Append(now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture));
			if (utcOffset.Hours > 0)
			{
				stringBuilder3.Append("+");
				stringBuilder3.Append(utcOffset.Hours.ToString("00", CultureInfo.InvariantCulture));
				stringBuilder3.Append("'");
			}
			else if (utcOffset.Hours < 0)
			{
				stringBuilder3.Append("-");
				stringBuilder3.Append(Math.Abs(utcOffset.Hours).ToString("00", CultureInfo.InvariantCulture));
				stringBuilder3.Append("'");
			}
			else
			{
				stringBuilder3.Append("Z00'");
			}
			stringBuilder3.Append(utcOffset.Minutes.ToString("00", CultureInfo.InvariantCulture));
			stringBuilder3.Append("')");
			stringBuilder3.Append("\r\n");
			stringBuilder3.Append(">>");
			this.m_infoId = this.WriteObject(stringBuilder3.ToString());
			this.WriteCrossRef();
		}

		private void EndReport_WriteDocumentMap()
		{
			if (this.DocumentMapRootLabel != null && this.DocumentMapRootLabel.Children != null && this.DocumentMapRootLabel.Children.Count != 0)
			{
				this.m_outlinesId = this.ReserveObjectId();
				int value = default(int);
				int value2 = default(int);
				int num = this.EndReport_WriteDocumentMap_Recursive(this.DocumentMapRootLabel.Children, this.m_outlinesId, out value, out value2);
				if (num > 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("<< /Type /Outlines /First ");
					PDFWriter.Write(stringBuilder, value);
					stringBuilder.Append(" 0 R /Last ");
					PDFWriter.Write(stringBuilder, value2);
					stringBuilder.Append(" 0 R /Count ");
					PDFWriter.Write(stringBuilder, num);
					stringBuilder.Append(" >>");
					this.WriteObject(this.m_outlinesId, stringBuilder.ToString());
				}
				this.DocumentMapLabelPoints = null;
				this.DocumentMapRootLabel = null;
			}
		}

		private int EndReport_WriteDocumentMap_Recursive(List<PDFLabel> labels, int parentId, out int firstChildId, out int lastChildId)
		{
			firstChildId = (lastChildId = -1);
			int num = -1;
			int previousId = -1;
			int num2 = -1;
			int num3 = 0;
			int num4 = -1;
			if (labels.Count == 0)
			{
				return 0;
			}
			PDFPagePoint pagePoint = null;
			PDFPagePoint pDFPagePoint = default(PDFPagePoint);
			int firstChildId2;
			int lastChildId2;
			for (int i = 0; i < labels.Count; i++)
			{
				PDFLabel pDFLabel = labels[i];
				if (this.DocumentMapLabelPoints.TryGetValue(pDFLabel.UniqueName, out pDFPagePoint) && pDFPagePoint.PageObjectId != -1)
				{
					if (num == -1)
					{
						num = (firstChildId = this.ReserveObjectId());
						num4 = i;
						pagePoint = pDFPagePoint;
					}
					else
					{
						int childCount;
						firstChildId2 = (lastChildId2 = (childCount = -1));
						if (labels[num4].Children != null && labels[num4].Children.Count > 0)
						{
							childCount = this.EndReport_WriteDocumentMap_Recursive(labels[num4].Children, num, out firstChildId2, out lastChildId2);
						}
						num2 = this.ReserveObjectId();
						this.WriteDocumentMapEntry(num, labels[num4], pagePoint, parentId, previousId, num2, firstChildId2, lastChildId2, childCount);
						previousId = num;
						num = num2;
						num4 = i;
						pagePoint = pDFPagePoint;
						num3++;
					}
				}
			}
			if (num4 != -1)
			{
				PDFLabel pDFLabel = labels[num4];
				if (this.DocumentMapLabelPoints.TryGetValue(pDFLabel.UniqueName, out pDFPagePoint))
				{
					int childCount;
					firstChildId2 = (lastChildId2 = (childCount = -1));
					if (labels[num4].Children != null && labels[num4].Children.Count > 0)
					{
						childCount = this.EndReport_WriteDocumentMap_Recursive(labels[num4].Children, num, out firstChildId2, out lastChildId2);
					}
					this.WriteDocumentMapEntry(num, labels[num4], pDFPagePoint, parentId, previousId, -1, firstChildId2, lastChildId2, childCount);
					num3++;
					if (lastChildId == -1)
					{
						lastChildId = num;
					}
				}
			}
			return num3;
		}

		internal override void FillPolygon(Color color, PointF[] polygon)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("\r\n");
			PDFWriter.WriteColor(stringBuilder, color, false);
			PDFWriter.Write(stringBuilder, this.ConvertToPDFUnits(polygon[0]));
			stringBuilder.Append(" m ");
			for (int i = 1; i < polygon.Length; i++)
			{
				PDFWriter.Write(stringBuilder, this.ConvertToPDFUnits(polygon[i]));
				stringBuilder.Append(" l ");
			}
			stringBuilder.Append("f");
			this.m_pageContentsSection.Add(stringBuilder.ToString());
		}

		internal override void FillRectangle(Color color, RectangleF rectangle)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("\r\n");
			PDFWriter.WriteColor(stringBuilder, color, false);
			rectangle = this.ConvertToPDFUnits(rectangle);
			PDFWriter.WriteRectangle(stringBuilder, rectangle.Left, rectangle.Top - rectangle.Height, rectangle.Size);
			stringBuilder.Append(" re f");
			this.m_pageContentsSection.Add(stringBuilder.ToString());
		}

		internal override void ProcessAction(string uniqueName, RPLActionInfo actionInfo, RectangleF position)
		{
			RPLAction rPLAction = actionInfo.Actions[0];
			if (!string.IsNullOrEmpty(rPLAction.Hyperlink))
			{
				this.m_actions.Add(this.ReserveObjectId(), new PDFUriAction(rPLAction.Hyperlink, this.ConvertToPDFUnits(position)));
			}
		}

		internal override void ProcessLabel(string uniqueName, string label, PointF point)
		{
			if (this.DocumentMapRootLabel != null && !this.DocumentMapLabelPoints.ContainsKey(uniqueName))
			{
				PDFPagePoint value = new PDFPagePoint(this.m_pageId, point);
				this.DocumentMapLabelPoints.Add(uniqueName, value);
			}
		}

		protected override void Dispose(bool disposing)
		{
			this.m_fonts = null;
			this.m_actions = null;
			this.m_images = null;
			this.m_pageContentsSection = null;
			this.DocumentMapLabelPoints = null;
			base.Dispose(disposing);
		}

		~PDFWriter()
		{
			this.Dispose(false);
		}

		private string CompressString(string text)
		{
			if (this.HumanReadablePDF)
			{
				return text;
			}
			byte[] bytes = this.m_ansiEncoding.GetBytes(text);
			byte[] bytes2 = this.CompressBytes(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			PDFWriter.AppendBytes(stringBuilder, bytes2);
			return stringBuilder.ToString();
		}

		private byte[] CompressBytes(byte[] bytes)
		{
			return PDFWriter.ManagedCompress(bytes);
		}

		private static int CompressBytes(StringBuilder stringBuilder, byte[] bytes, int offset, int count)
		{
			byte[] array = PDFWriter.ManagedCompress(bytes, offset, count);
			int num = array.Length;
			PDFWriter.AppendBytes(stringBuilder, array, num);
			return num;
		}

		private static void AppendBytes(StringBuilder stringBuilder, byte[] bytes)
		{
			PDFWriter.AppendBytes(stringBuilder, bytes, bytes.Length);
		}

		private static void AppendBytes(StringBuilder stringBuilder, byte[] bytes, int count)
		{
			for (int i = 0; i < count; i++)
			{
				stringBuilder.Append((char)bytes[i]);
			}
		}

		private static byte[] ManagedCompress(byte[] bytes)
		{
			return PDFWriter.ManagedCompress(bytes, 0, bytes.Length);
		}

		private static byte[] ManagedCompress(byte[] bytes, int offset, int count)
		{
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.WriteByte(88);
			memoryStream.WriteByte(9);
			DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, true);
			deflateStream.Write(bytes, offset, count);
			deflateStream.Close();
			uint num = PDFWriter.CalculateAdler32Checksum(bytes, offset, count);
			memoryStream.WriteByte((byte)(num >> 24));
			memoryStream.WriteByte((byte)((num & 0xFF0000) >> 16));
			memoryStream.WriteByte((byte)((num & 0xFF00) >> 8));
			memoryStream.WriteByte((byte)num);
			return memoryStream.ToArray();
		}

		private static uint CalculateAdler32Checksum(byte[] bytes, int offset, int count)
		{
			uint num = 1u;
			uint num2 = 0u;
			if (bytes == null)
			{
				return 1u;
			}
			for (int i = offset; i < count; i++)
			{
				num = (num + bytes[i]) % 65521u;
				num2 = (num2 + num) % 65521u;
			}
			return num2 * 65536 + num;
		}

		private PointF ConvertToPDFUnits(PointF point)
		{
			return this.ConvertToPDFUnits(point.X, point.Y);
		}

		private PointF ConvertToPDFUnits(float x, float y)
		{
			return new PointF((float)(this.m_bounds.Left + x * 2.8346459865570068), (float)(this.m_bounds.Top - y * 2.8346459865570068));
		}

		private RectangleF ConvertBoundsToPDFUnits(RectangleF rectangle)
		{
			return new RectangleF((float)(rectangle.X * 2.8346459865570068), (float)(this.m_mediaBoxSize.Height - rectangle.Y * 2.8346459865570068), (float)(rectangle.Width * 2.8346459865570068), (float)(rectangle.Height * 2.8346459865570068));
		}

		private RectangleF ConvertToPDFUnits(RectangleF rectangle)
		{
			return new RectangleF((float)(this.m_bounds.Left + rectangle.X * 2.8346459865570068), (float)(this.m_bounds.Top - rectangle.Y * 2.8346459865570068), (float)(rectangle.Width * 2.8346459865570068), (float)(rectangle.Height * 2.8346459865570068));
		}

		private float ConvertPixelsToPDFUnits(int pixels, float dpi)
		{
			return (float)(SharedRenderer.ConvertToMillimeters(pixels, dpi) * 2.8346459865570068);
		}

		private static string ConvertReservedCharToASCII(char reservedChar)
		{
			byte value = Convert.ToByte(reservedChar);
			return Convert.ToString(value, 16);
		}

		private static string EncodePDFName(string literalName)
		{
			StringBuilder stringBuilder = new StringBuilder(literalName);
			for (int i = 0; i < stringBuilder.Length; i++)
			{
				char c = stringBuilder[i];
				string text = null;
				text = (string)PDFWriter.m_pdfDelimiterChars[c];
				if (text == null && char.IsWhiteSpace(c))
				{
					text = PDFWriter.ConvertReservedCharToASCII(c);
				}
				if (text != null)
				{
					stringBuilder[i] = '#';
					stringBuilder.Insert(i + 1, text);
					i += text.Length;
				}
			}
			return stringBuilder.ToString();
		}

		private static string EscapeString(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder(text);
			for (int i = 0; i < stringBuilder.Length; i++)
			{
				switch (stringBuilder[i])
				{
				case '(':
				case ')':
				case '\\':
					stringBuilder.Insert(i, '\\');
					i++;
					break;
				case '\n':
					stringBuilder[i] = 'n';
					stringBuilder.Insert(i, '\\');
					i++;
					break;
				case '\r':
					stringBuilder[i] = 'r';
					stringBuilder.Insert(i, '\\');
					i++;
					break;
				case '\t':
					stringBuilder[i] = 't';
					stringBuilder.Insert(i, '\\');
					i++;
					break;
				case '\b':
					stringBuilder[i] = 'b';
					stringBuilder.Insert(i, '\\');
					i++;
					break;
				case '\f':
					stringBuilder[i] = 'f';
					stringBuilder.Insert(i, '\\');
					i++;
					break;
				}
			}
			return stringBuilder.ToString();
		}

		private PDFImage GetDefaultImage()
		{
			string key = "__int__InvalidImage";
			PDFImage pDFImage = default(PDFImage);
			if (!this.m_images.TryGetValue(key, out pDFImage))
			{
				pDFImage = new PDFImage();
				pDFImage.ImageId = this.ReserveObjectId();
				Bitmap bitmap = Renderer.ImageResources["InvalidImage"];
				lock (bitmap)
				{
					GDIImageProps gdiProperties = new GDIImageProps(bitmap);
					using (MemoryStream memoryStream = new MemoryStream())
					{
						bitmap.Save(memoryStream, bitmap.RawFormat);
						pDFImage.ImageData = memoryStream.GetBuffer();
					}
					pDFImage.GdiProperties = gdiProperties;
				}
				this.m_images.Add(key, pDFImage);
			}
			if (!this.m_imagesUsedInCurrentPage.Contains(pDFImage.ImageId))
			{
				this.m_imagesUsedInCurrentPage.Add(pDFImage.ImageId);
			}
			return pDFImage;
		}

		private PDFImage GetImage(string imageName, byte[] imageData, long imageDataOffset, GDIImageProps gdiImageProps)
		{
			if (string.IsNullOrEmpty(imageName) && imageData != null)
			{
				byte[] array = this.Md5Hasher.ComputeHash(imageData);
				StringBuilder stringBuilder = new StringBuilder("__int__");
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.Append(array[i].ToString("x2"));
				}
				imageName = stringBuilder.ToString();
			}
			PDFImage pDFImage = default(PDFImage);
			if (string.IsNullOrEmpty(imageName) || !this.m_images.TryGetValue(imageName, out pDFImage))
			{
				if (!SharedRenderer.GetImage(base.m_renderer.RplReport, ref imageData, imageDataOffset, ref gdiImageProps))
				{
					return null;
				}
				pDFImage = new PDFImage();
				pDFImage.ImageId = this.ReserveObjectId();
				pDFImage.ImageData = imageData;
				pDFImage.GdiProperties = gdiImageProps;
				if (string.IsNullOrEmpty(imageName))
				{
					imageName = "__int__" + pDFImage.ImageId.ToString(CultureInfo.InvariantCulture);
				}
				this.m_images.Add(imageName, pDFImage);
			}
			if (!this.m_imagesUsedInCurrentPage.Contains(pDFImage.ImageId))
			{
				this.m_imagesUsedInCurrentPage.Add(pDFImage.ImageId);
			}
			return pDFImage;
		}

		private static int GetInt32(byte[] data, int offset)
		{
			int num = data[offset] << 24;
			num += data[offset + 1] << 16;
			num += data[offset + 2] << 8;
			return num + data[offset + 3];
		}

		private static bool IsUnicode(char character)
		{
			if (character <= '\u007f')
			{
				return false;
			}
			if (PDFWriter.m_unicodeToWinAnsi.ContainsKey(character))
			{
				return false;
			}
			return true;
		}

		private int ReserveObjectId()
		{
			return this.m_nextObjectId++;
		}

		private void UpdateCrossRefPosition(int objectId)
		{
			if (objectId == this.m_objectOffsets.Count)
			{
				this.m_objectOffsets.Add(base.m_outputStream.Position);
			}
			else if (objectId < this.m_objectOffsets.Count)
			{
				this.m_objectOffsets[objectId] = base.m_outputStream.Position;
			}
			else
			{
				while (objectId > this.m_objectOffsets.Count)
				{
					this.m_objectOffsets.Add(0L);
				}
				this.m_objectOffsets.Add(base.m_outputStream.Position);
			}
		}

		private bool UnicodeOutput(AspNetCore.ReportingServices.Rendering.RichText.TextRun run)
		{
			if (!run.IsComplex && run.ScriptAnalysis.fRTL != 1 && run.CachedFont.TextMetric.tmCharSet != 2)
			{
				string text = run.Text;
				for (int i = 0; i < text.Length; i++)
				{
					if (PDFWriter.IsUnicode(text[i]))
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}

		private string GetFontKey(Font font)
		{
			StringBuilder stringBuilder = new StringBuilder(font.FontFamily.GetName(1033));
			FontStyle style = font.Style;
			if ((style & FontStyle.Bold) > FontStyle.Regular && (style & FontStyle.Italic) > FontStyle.Regular)
			{
				stringBuilder.Append(",BoldItalic");
			}
			else if ((style & FontStyle.Bold) > FontStyle.Regular)
			{
				stringBuilder.Append(",Bold");
			}
			else if ((style & FontStyle.Italic) > FontStyle.Regular)
			{
				stringBuilder.Append(",Italic");
			}
			return stringBuilder.ToString();
		}

		private PDFFont ProcessDrawStringFont(AspNetCore.ReportingServices.Rendering.RichText.TextRun run, TypeCode typeCode, RPLFormat.TextAlignments textAlign, RPLFormat.VerticalAlignments verticalAlign, RPLFormat.WritingModes writingMode, RPLFormat.Directions direction)
		{
			Font font = run.CachedFont.Font;
			bool flag = this.UnicodeOutput(run);
			string text = this.GetFontKey(font);
			if (flag)
			{
				text += "+UnicodeFont";
			}
			string text2 = default(string);
			bool flag2 = PDFWriter.m_internalFonts.TryGetValue(text, out text2);
			if (flag2)
			{
				text = text2;
			}
			PDFFont pDFFont = default(PDFFont);
			if (!this.m_fonts.TryGetValue(text, out pDFFont))
			{
				bool simulateItalic = false;
				bool simulateBold = false;
				if (!flag2)
				{
					Win32DCSafeHandle hdc = base.m_commonGraphics.GetHdc();
					Win32ObjectSafeHandle win32ObjectSafeHandle = Win32ObjectSafeHandle.Zero;
					try
					{
						win32ObjectSafeHandle = AspNetCore.ReportingServices.Rendering.RichText.Win32.SelectObject(hdc, run.CachedFont.Hfont);
						FontPackage.CheckSimulatedFontStyles(hdc, run.CachedFont.TextMetric, ref simulateItalic, ref simulateBold);
					}
					finally
					{
						if (!win32ObjectSafeHandle.IsInvalid)
						{
							win32ObjectSafeHandle.SetHandleAsInvalid();
						}
						base.m_commonGraphics.ReleaseHdc();
					}
				}
				string text3 = text;
				if (!flag2)
				{
					text3 = PDFWriter.EncodePDFName(text3);
				}
				FontStyle style = font.Style;
				int emHeight = font.FontFamily.GetEmHeight(style);
				float gridHeight = (float)(font.GetHeight((float)base.m_commonGraphics.DpiY) * 2.8346459865570068);
				string fontCMap = null;
				string registry = null;
				string ordering = null;
				string supplement = null;
				if (flag)
				{
					fontCMap = "Identity-H";
					registry = "Adobe";
					ordering = "Identity";
					supplement = "0";
				}
				pDFFont = new PDFFont(run.CachedFont, font.FontFamily.Name, text3, fontCMap, registry, ordering, supplement, style, emHeight, gridHeight, flag2, simulateItalic, simulateBold);
				pDFFont.FontId = this.ReserveObjectId();
				this.m_fonts.Add(text, pDFFont);
			}
			if (!this.m_fontsUsedInCurrentPage.Contains(pDFFont.FontId))
			{
				this.m_fontsUsedInCurrentPage.Add(pDFFont.FontId);
			}
			return pDFFont;
		}

		private static void WriteGlyph(AspNetCore.ReportingServices.Rendering.RichText.TextRun textRun, StringBuilder sb, PDFFont pdfFont, int glyphIndex)
		{
			ushort num = (ushort)textRun.GlyphData.GlyphScriptShapeData.Glyphs[glyphIndex];
			PDFFont.GlyphData glyphData = pdfFont.AddUniqueGlyph(num, (float)textRun.GlyphData.ScaledAdvances[glyphIndex] * pdfFont.EMGridConversion);
			PDFWriter.MapGlyphToUnicodeChar(glyphData, textRun, glyphIndex);
			if (num == 65535)
			{
				num = 34;
			}
			PDFWriter.WriteHex(sb, num >> 8);
			PDFWriter.WriteHex(sb, num & 0xFF);
		}

		private static List<int> WriteVerticallyStackedText(FontCache fontCache, AspNetCore.ReportingServices.Rendering.RichText.TextRun textRun, StringBuilder sb, PDFFont pdfFont)
		{
			List<int> list = null;
			int glyphCount = textRun.GlyphData.GlyphScriptShapeData.GlyphCount;
			for (int i = 0; i < glyphCount; i++)
			{
				ScriptVisAttr scriptVisAttr = new ScriptVisAttr(textRun.GlyphData.GlyphScriptShapeData.VisAttrs[i].word1);
				if (scriptVisAttr.uJustification != 4)
				{
					if (AspNetCore.ReportingServices.Rendering.RichText.Utilities.IsEastAsianChar(textRun.Text[i]))
					{
						sb.Append("<");
						PDFWriter.WriteGlyph(textRun, sb, pdfFont, i);
						sb.Append("> Tj");
					}
					else
					{
						if (list == null)
						{
							list = new List<int>();
						}
						list.Add(i);
					}
					if (i < glyphCount - 1)
					{
						float num = SharedRenderer.ConvertToMillimeters(textRun.GlyphData.Advances[i], fontCache.Dpi);
						sb.AppendFormat(" 0 {0} TD", (float)((0.0 - num) * 2.8346459865570068));
					}
					sb.AppendLine();
				}
			}
			return list;
		}

		private static void WriteVerticalText(FontCache fontCache, AspNetCore.ReportingServices.Rendering.RichText.TextRun textRun, StringBuilder sb, PDFFont pdfFont, List<int> skippedCharacterIndeces)
		{
			int num = 0;
			foreach (int skippedCharacterIndece in skippedCharacterIndeces)
			{
				if (skippedCharacterIndece > num)
				{
					int num2 = 0;
					for (int i = num; i < skippedCharacterIndece; i++)
					{
						num2 += textRun.GlyphData.Advances[i];
					}
					float num3 = SharedRenderer.ConvertToMillimeters(num2, fontCache.Dpi);
					sb.AppendFormat("{0} 0 TD ", (float)(num3 * 2.8346459865570068));
					num = skippedCharacterIndece;
				}
				sb.Append("<");
				PDFWriter.WriteGlyph(textRun, sb, pdfFont, skippedCharacterIndece);
				sb.Append("> Tj");
				sb.AppendLine();
			}
		}

		private static void WriteText(AspNetCore.ReportingServices.Rendering.RichText.TextRun textRun, StringBuilder sb, PDFFont pdfFont, string text, bool humanReadablePDF)
		{
			if (string.IsNullOrEmpty(text))
			{
				sb.Append("()Tj ");
			}
			else
			{
				if (pdfFont.IsComposite)
				{
					sb.Append("<");
					for (int i = 0; i < textRun.GlyphData.GlyphScriptShapeData.GlyphCount; i++)
					{
						PDFWriter.WriteGlyph(textRun, sb, pdfFont, i);
					}
					sb.Append("> ");
				}
				else
				{
					sb.Append("(");
					if (humanReadablePDF)
					{
						foreach (char c in text)
						{
							char value = default(char);
							if (PDFWriter.m_unicodeToWinAnsi.TryGetValue(c, out value))
							{
								sb.Append(value);
							}
							else
							{
								sb.Append(c);
							}
						}
					}
					else
					{
						sb.Append(text);
					}
					sb.Append(") ");
					if (!pdfFont.InternalFont)
					{
						for (int k = 0; k < textRun.GlyphData.GlyphScriptShapeData.GlyphCount; k++)
						{
							ushort glyph = (ushort)textRun.GlyphData.GlyphScriptShapeData.Glyphs[k];
							PDFFont.GlyphData glyphData = pdfFont.AddUniqueGlyph(glyph, (float)textRun.GlyphData.ScaledAdvances[k] * pdfFont.EMGridConversion);
							PDFWriter.MapGlyphToUnicodeChar(glyphData, textRun, k);
						}
					}
				}
				sb.Append("Tj ");
			}
		}

		private static void MapGlyphToUnicodeChar(PDFFont.GlyphData glyphData, AspNetCore.ReportingServices.Rendering.RichText.TextRun textRun, int glyphIndex)
		{
			if (glyphData != null && textRun.ScriptAnalysis.fLayoutRTL == 0 && textRun.ScriptAnalysis.fRTL == 0 && textRun.Text.Length == textRun.GlyphData.GlyphScriptShapeData.GlyphCount && textRun.Text.Length == textRun.GlyphData.GlyphScriptShapeData.Clusters.Length && glyphIndex < textRun.Text.Length)
			{
				if (textRun.CachedFont.DefaultGlyph.HasValue && textRun.CachedFont.DefaultGlyph == glyphData.Glyph)
				{
					return;
				}
				glyphData.Character = textRun.Text[glyphIndex];
			}
		}

		private unsafe int Process32bppArgbImage(StringBuilder sb, StringBuilder imageContent, PDFImage image)
		{
			sb.Append(" /ColorSpace /DeviceRGB /BitsPerComponent 8 ");
			if (!this.HumanReadablePDF)
			{
				sb.Append("/Filter /FlateDecode ");
			}
			MemoryStream memoryStream = new MemoryStream();
			int i = 0;
			bool flag = false;
			using (Bitmap bitmap = new Bitmap(System.Drawing.Image.FromStream(new MemoryStream(image.ImageData))))
			{
				System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, image.GdiProperties.Width, image.GdiProperties.Height);
				BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
				IntPtr scan = bitmapData.Scan0;
				byte* ptr = null;
				uint num = (uint)Math.Abs(bitmapData.Stride);
				ptr = (byte*)((num == 0) ? ((byte*)scan.ToPointer() + (long)bitmapData.Stride * (long)(image.GdiProperties.Height - 1)) : scan.ToPointer());
				bool flag2 = false;
				bool flag3 = false;
				Color color;
				for (int j = 0; j < image.GdiProperties.Height; j++)
				{
					for (int k = 0; k < image.GdiProperties.Width; k++)
					{
						byte* ptr2 = ptr + j * num + (long)k * 4L;
						color = Color.FromArgb(ptr2[3], ptr2[2], ptr2[1], *ptr2);
						int num2;
						if (color.A == 0)
						{
							num2 = i;
							flag = true;
						}
						else
						{
							num2 = (color.ToArgb() & 0xFFFFFF);
							if (num2 == i)
							{
								flag3 = true;
							}
						}
						if (flag && flag3)
						{
							flag2 = true;
							break;
						}
						memoryStream.WriteByte((byte)((num2 & 0xFF0000) >> 16));
						memoryStream.WriteByte((byte)((num2 & 0xFF00) >> 8));
						memoryStream.WriteByte((byte)(num2 & 0xFF));
					}
					if (flag2)
					{
						break;
					}
				}
				if (flag2)
				{
					Dictionary<int, byte> dictionary = new Dictionary<int, byte>();
					for (int l = 0; l < image.GdiProperties.Height; l++)
					{
						for (int m = 0; m < image.GdiProperties.Width; m++)
						{
							byte* ptr3 = ptr + l * num + (long)m * 4L;
							color = Color.FromArgb(ptr3[3], ptr3[2], ptr3[1], *ptr3);
							int num2 = color.ToArgb() & 0xFFFFFF;
							if (!dictionary.ContainsKey(num2))
							{
								dictionary.Add(num2, 0);
							}
						}
					}
					for (; dictionary.ContainsKey(i); i++)
					{
					}
					memoryStream = new MemoryStream();
					for (int n = 0; n < image.GdiProperties.Height; n++)
					{
						for (int num3 = 0; num3 < image.GdiProperties.Width; num3++)
						{
							byte* ptr4 = ptr + n * num + (long)num3 * 4L;
							color = Color.FromArgb(ptr4[3], ptr4[2], ptr4[1], *ptr4);
							int num2 = (color.A != 0) ? (color.ToArgb() & 0xFFFFFF) : i;
							memoryStream.WriteByte((byte)((num2 & 0xFF0000) >> 16));
							memoryStream.WriteByte((byte)((num2 & 0xFF00) >> 8));
							memoryStream.WriteByte((byte)(num2 & 0xFF));
						}
					}
				}
				bitmap.UnlockBits(bitmapData);
			}
			if (flag)
			{
				int value = (i & 0xFF0000) >> 16;
				int value2 = (i & 0xFF00) >> 8;
				int value3 = i & 0xFF;
				sb.Append("/Mask [");
				sb.Append(value);
				sb.Append(" ");
				sb.Append(value);
				sb.Append(" ");
				sb.Append(value2);
				sb.Append(" ");
				sb.Append(value2);
				sb.Append(" ");
				sb.Append(value3);
				sb.Append(" ");
				sb.Append(value3);
				sb.Append("]");
			}
			if (this.HumanReadablePDF)
			{
				PDFWriter.Write(imageContent, memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
			}
			else
			{
				PDFWriter.CompressBytes(imageContent, memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
			}
			return imageContent.Length;
		}

		private void ProcessImage(PDFImage image)
		{
			if (image.ImageData != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("<< /Type /XObject /Subtype /Image");
				StringBuilder stringBuilder2 = new StringBuilder();
				int num;
				if (image.GdiProperties.RawFormat.Equals(ImageFormat.Jpeg))
				{
					stringBuilder.Append(" /BitsPerComponent 8 /Filter /DCTDecode /ColorSpace ");
					if (image.IsMonochromeJpeg)
					{
						stringBuilder.Append("/DeviceGray");
					}
					else
					{
						stringBuilder.Append("/DeviceRGB");
					}
					PDFWriter.Write(stringBuilder2, image.ImageData);
					num = image.ImageData.Length;
				}
				else if (image.GdiProperties.RawFormat.Equals(ImageFormat.Png))
				{
					num = PDFWriter.ProcessPngImage(stringBuilder, stringBuilder2, image);
					if (num == -1)
					{
						num = this.Process32bppArgbImage(stringBuilder, stringBuilder2, image);
					}
				}
				else
				{
					num = this.Process32bppArgbImage(stringBuilder, stringBuilder2, image);
				}
				image.ImageData = null;
				stringBuilder.Append(" /Width ");
				PDFWriter.Write(stringBuilder, image.GdiProperties.Width);
				stringBuilder.Append(" /Height ");
				PDFWriter.Write(stringBuilder, image.GdiProperties.Height);
				stringBuilder.Append(" /Length ");
				PDFWriter.Write(stringBuilder, num);
				stringBuilder.Append(" >>");
				this.UpdateCrossRefPosition(image.ImageId);
				this.Write("\r\n");
				this.Write(image.ImageId);
				this.Write(" 0 obj");
				this.Write("\r\n");
				this.Write(stringBuilder.ToString());
				this.Write("\r\n");
				this.Write("stream");
				this.Write("\r\n");
				this.Write(stringBuilder2.ToString());
				this.Write("\r\n");
				this.Write("endstream");
				this.Write("\r\n");
				this.Write("endobj");
			}
		}

		private static int ProcessPngImage(StringBuilder sb, StringBuilder imageContent, PDFImage image)
		{
			int num = 8;
			int num2 = 0;
			int num3 = 0;
			bool flag = false;
			byte[] imageData = image.ImageData;
			int num4 = 0;
			if (imageData[0] == 137 && imageData[1] == 80 && imageData[2] == 78)
			{
				ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
				string @string;
				do
				{
					int @int = PDFWriter.GetInt32(imageData, num);
					@string = aSCIIEncoding.GetString(imageData, num + 4, 4);
					switch (@string)
					{
					case "IHDR":
					{
						num2 = imageData[num + 8 + 8];
						num3 = imageData[num + 8 + 9];
						byte b = imageData[num + 8 + 12];
						if (num3 == 3 && b == 0)
						{
							break;
						}
						return -1;
					}
					case "PLTE":
						flag = true;
						sb.Append(" /ColorSpace [/Indexed /DeviceRGB ");
						PDFWriter.Write(sb, @int / 3 - 1);
						sb.Append("<");
						sb.Append("\r\n");
						for (int i = 0; i < @int; i++)
						{
							PDFWriter.WriteHex(sb, imageData[num + 8 + i]);
						}
						sb.Append(">]");
						sb.Append("\r\n");
						break;
					case "IDAT":
						PDFWriter.Write(imageContent, imageData, num + 8, @int);
						num4 += @int;
						break;
					}
					num += @int + 8 + 4;
				}
				while (@string != "IEND" && num < imageData.Length);
				if (!flag)
				{
					if (num3 == 3)
					{
						sb.Append(" /ColorSpace [/Indexed /DeviceRGB 255  <");
						sb.Append("\r\n");
						for (int j = 0; j < 768; j++)
						{
							PDFWriter.Write(sb, j & 0xFF);
							PDFWriter.Write(sb, j & 0xFF);
							PDFWriter.Write(sb, j & 0xFF);
						}
						sb.Append(">]");
						sb.Append("\r\n");
					}
					else
					{
						sb.Append(" /ColorSpace /DeviceRGB");
					}
				}
				sb.Append(" /BitsPerComponent ");
				PDFWriter.Write(sb, num2);
				sb.Append(" /Filter /FlateDecode /DecodeParms << /Predictor 15 /Columns ");
				int num5 = image.GdiProperties.Width * num2 / 8 + (((image.GdiProperties.Width * num2 & 7) != 0) ? 1 : 0);
				if (num3 == 6)
				{
					num5 *= 4;
				}
				PDFWriter.Write(sb, num5);
				if (num3 == 2)
				{
					sb.Append(" /Colors 3");
				}
				else
				{
					sb.Append(" /Colors 1");
				}
				sb.Append(" >>");
				return num4;
			}
			throw new InvalidDataException();
		}

		private void Write(string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				int length = text.Length;
				byte[] array = new byte[length];
				for (int i = 0; i < length; i++)
				{
					char c = text[i];
					if (c <= 'ÿ')
					{
						array[i] = (byte)c;
					}
					else
					{
						array[i] = 63;
					}
				}
				base.m_outputStream.Write(array, 0, length);
			}
		}

		private void Write(int value)
		{
			this.Write(value.ToString("#########0", NumberFormatInfo.InvariantInfo));
		}

		private void Write(long value)
		{
			this.Write(value.ToString("#########0", NumberFormatInfo.InvariantInfo));
		}

		private void Write(byte[] buffer)
		{
			base.m_outputStream.Write(buffer, 0, buffer.Length);
		}

		private static void Write(StringBuilder sb, ushort value)
		{
			sb.Append(value.ToString("#########0", NumberFormatInfo.InvariantInfo));
		}

		private static void Write(StringBuilder sb, int value)
		{
			sb.Append(value.ToString("#########0", NumberFormatInfo.InvariantInfo));
		}

		private static void Write(StringBuilder sb, float value)
		{
			if ((float)(long)value == value)
			{
				sb.Append(value.ToString("#########0", NumberFormatInfo.InvariantInfo));
			}
			else
			{
				sb.Append(value.ToString("#########0.0##", NumberFormatInfo.InvariantInfo));
			}
		}

		private static void Write(StringBuilder sb, PointF point)
		{
			PDFWriter.Write(sb, point.X);
			sb.Append(" ");
			PDFWriter.Write(sb, point.Y);
		}

		private static void Write(StringBuilder sb, SizeF size)
		{
			PDFWriter.Write(sb, size.Width);
			sb.Append(" ");
			PDFWriter.Write(sb, size.Height);
		}

		private static void Write(StringBuilder sb, byte[] data)
		{
			PDFWriter.Write(sb, data, 0, data.Length);
		}

		private static void Write(StringBuilder sb, byte[] data, int offset, int length)
		{
			for (int i = offset; i < offset + length; i++)
			{
				sb.Append((char)data[i]);
			}
		}

		private static void WriteClipBounds(StringBuilder sb, float left, float bottom, SizeF size)
		{
			sb.Append("\r\n");
			sb.Append("q ");
			PDFWriter.WriteRectangle(sb, left, bottom, size);
			sb.Append(" re W n");
		}

		private static void WriteColor(StringBuilder sb, Color color, bool isStroke)
		{
			PDFWriter.Write(sb, (float)((float)(int)color.R / 255.0));
			sb.Append(" ");
			PDFWriter.Write(sb, (float)((float)(int)color.G / 255.0));
			sb.Append(" ");
			PDFWriter.Write(sb, (float)((float)(int)color.B / 255.0));
			if (!isStroke)
			{
				sb.Append(" rg ");
			}
			else
			{
				sb.Append(" RG ");
			}
		}

		private void WriteCrossRef()
		{
			this.Write("\r\n");
			long position = base.m_outputStream.Position;
			this.Write("xref");
			this.Write("\r\n");
			this.Write("0 ");
			this.Write(this.m_objectOffsets.Count);
			this.Write("\r\n");
			this.Write("0000000000 65535 f");
			this.Write("\r\n");
			for (int i = 1; i < this.m_objectOffsets.Count; i++)
			{
				this.Write((this.m_objectOffsets[i] + 2).ToString("0000000000", CultureInfo.InvariantCulture));
				this.Write(" 00000 n");
				this.Write("\r\n");
			}
			this.Write("trailer << /Size ");
			this.Write(this.m_objectOffsets.Count);
			this.Write(" /Root ");
			this.Write(this.m_rootId);
			this.Write(" 0 R /Info ");
			this.Write(this.m_infoId);
			this.Write(" 0 R >>");
			this.Write("\r\n");
			this.Write("startxref");
			this.Write("\r\n");
			this.Write(position);
			this.Write("\r\n");
			this.Write("%%EOF");
		}

		private void WriteDocumentMapEntry(int id, PDFLabel label, PDFPagePoint pagePoint, int parentId, int previousId, int nextId, int firstChildId, int lastChildId, int childCount)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<< /Title ");
			this.WriteUnicodeString(stringBuilder, label.Label);
			stringBuilder.Append(" /Parent ");
			PDFWriter.Write(stringBuilder, parentId);
			stringBuilder.Append(" 0 R /Dest [");
			PDFWriter.Write(stringBuilder, pagePoint.PageObjectId);
			stringBuilder.Append(" 0 R /XYZ ");
			PDFWriter.Write(stringBuilder, this.ConvertToPDFUnits(pagePoint.Point));
			stringBuilder.Append(" null]");
			if (firstChildId != -1)
			{
				stringBuilder.Append(" /First ");
				PDFWriter.Write(stringBuilder, firstChildId);
				stringBuilder.Append(" 0 R /Last ");
				PDFWriter.Write(stringBuilder, lastChildId);
				stringBuilder.Append(" 0 R /Count ");
				PDFWriter.Write(stringBuilder, childCount);
			}
			if (previousId != -1)
			{
				stringBuilder.Append(" /Prev ");
				PDFWriter.Write(stringBuilder, previousId);
				stringBuilder.Append(" 0 R");
			}
			if (nextId != -1)
			{
				stringBuilder.Append(" /Next ");
				PDFWriter.Write(stringBuilder, nextId);
				stringBuilder.Append(" 0 R");
			}
			stringBuilder.Append(" >>");
			this.WriteObject(id, stringBuilder.ToString());
		}

		private void WriteEmbeddedFont(EmbeddedFont embeddedFont)
		{
			Win32DCSafeHandle hdc = base.m_commonGraphics.GetHdc();
			Win32ObjectSafeHandle win32ObjectSafeHandle = Win32ObjectSafeHandle.Zero;
			try
			{
				PDFFont pDFFont = embeddedFont.PDFFonts[0];
				win32ObjectSafeHandle = AspNetCore.ReportingServices.Rendering.RichText.Win32.SelectObject(hdc, pDFFont.CachedFont.Hfont);
				ushort[] glyphIdArray = embeddedFont.GetGlyphIdArray();
				byte[] buffer = FontPackage.Generate(hdc, pDFFont.FontFamily, glyphIdArray);
				this.WriteFontBuffer(embeddedFont.ObjectId, buffer);
				foreach (PDFFont pDFFont2 in embeddedFont.PDFFonts)
				{
					pDFFont2.FontPDFFamily = "ABCDEE+" + pDFFont2.FontPDFFamily;
				}
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				foreach (PDFFont pDFFont3 in embeddedFont.PDFFonts)
				{
					pDFFont3.EmbeddedFont = null;
				}
				if (RSTrace.ImageRendererTracer.TraceError)
				{
					RSTrace.ImageRendererTracer.Trace(TraceLevel.Error, "Exception in WriteEmbeddedFont for Font {0}: {1}", embeddedFont.PDFFonts[0].FontFamily, ex2.Message);
				}
			}
			finally
			{
				if (!win32ObjectSafeHandle.IsInvalid)
				{
					win32ObjectSafeHandle.SetHandleAsInvalid();
				}
				base.m_commonGraphics.ReleaseHdc();
			}
		}

		private void WriteCMapEntry(StringBuilder stringBuilder, ushort value)
		{
			stringBuilder.Append("<");
			PDFWriter.WriteHex(stringBuilder, value >> 8);
			PDFWriter.WriteHex(stringBuilder, value & 0xFF);
			stringBuilder.Append(">");
		}

		private void WriteCMapMappingRangeEntry(StringBuilder stringBuilder, CMapMappingRange item)
		{
			this.WriteCMapEntry(stringBuilder, item.Mapping.Source);
			stringBuilder.Append(" ");
			this.WriteCMapEntry(stringBuilder, (ushort)(item.Mapping.Source + item.Length));
			stringBuilder.Append(" ");
			this.WriteCMapEntry(stringBuilder, item.Mapping.Destination);
			stringBuilder.AppendLine();
		}

		private void WriteCMapMappingEntry(StringBuilder stringBuilder, CMapMapping item)
		{
			this.WriteCMapEntry(stringBuilder, item.Source);
			stringBuilder.Append(" ");
			this.WriteCMapEntry(stringBuilder, item.Destination);
			stringBuilder.AppendLine();
		}

		private void WriteCMapEntries<T>(StringBuilder stringBuilder, List<T> items, Action<StringBuilder, T> writeItem, string cMapEntryStart, string cMapEntryEnd)
		{
			int num = 0;
			foreach (T item in items)
			{
				if (num % 100 == 0)
				{
					int val = items.Count - num;
					int value = Math.Min(100, val);
					stringBuilder.Append(value);
					stringBuilder.AppendLine(" " + cMapEntryStart);
				}
				writeItem(stringBuilder, item);
				num++;
				if (num % 100 == 0)
				{
					stringBuilder.AppendLine(cMapEntryEnd);
				}
			}
			if (num % 100 != 0)
			{
				stringBuilder.AppendLine(cMapEntryEnd);
			}
		}

		private static void FlushToUnicodeRange(List<CMapMapping> singleMappings, List<CMapMappingRange> rangeMappings, List<CMapMapping> range)
		{
			int count = range.Count;
			if (count != 0)
			{
				CMapMapping cMapMapping = range[0];
				if (count == 1)
				{
					singleMappings.Add(cMapMapping);
				}
				else
				{
					CMapMapping cMapMapping2 = range[count - 1];
					rangeMappings.Add(new CMapMappingRange(cMapMapping, (ushort)(cMapMapping2.Source - cMapMapping.Source)));
				}
				range.Clear();
			}
		}

		private void WriteToUnicodeMappingEntries(StringBuilder stringBuilder, IEnumerable<CMapMapping> glyphIdToUnicodeMapping)
		{
			List<CMapMapping> list = new List<CMapMapping>();
			List<CMapMappingRange> list2 = new List<CMapMappingRange>();
			List<CMapMapping> list3 = new List<CMapMapping>();
			foreach (CMapMapping item in from cMapMapping in glyphIdToUnicodeMapping
			orderby cMapMapping
			select cMapMapping)
			{
				if (list3.Count > 0)
				{
					CMapMapping cMapMapping2 = list3[list3.Count - 1];
					if (item.GetSourceDelta(cMapMapping2) != item.GetDestinationDelta(cMapMapping2) || cMapMapping2.GetSourceLeftByte() != item.GetSourceLeftByte())
					{
						PDFWriter.FlushToUnicodeRange(list, list2, list3);
					}
				}
				list3.Add(item);
			}
			if (list3.Count > 0)
			{
				PDFWriter.FlushToUnicodeRange(list, list2, list3);
			}
			this.WriteCMapEntries(stringBuilder, list, this.WriteCMapMappingEntry, "beginbfchar", "endbfchar");
			this.WriteCMapEntries(stringBuilder, list2, this.WriteCMapMappingRangeEntry, "beginbfrange", "endbfrange");
		}

		private string GetToUnicodeMap(IEnumerable<CMapMapping> glyphIdToUnicodeMapping)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("/CIDInit /ProcSet findresource begin");
			stringBuilder.AppendLine("12 dict begin");
			stringBuilder.AppendLine("begincmap");
			stringBuilder.AppendLine("/CIDSystemInfo << /Registry (Adobe) /Ordering (UCS) /Supplement 0 >> def");
			stringBuilder.AppendLine("/CMapName /Adobe-Identity-UCS def");
			stringBuilder.AppendLine("/CMapType 2 def");
			stringBuilder.AppendLine("1 begincodespacerange");
			stringBuilder.AppendLine("<0000> <FFFF>");
			stringBuilder.AppendLine("endcodespacerange");
			this.WriteToUnicodeMappingEntries(stringBuilder, glyphIdToUnicodeMapping);
			stringBuilder.AppendLine("endcmap");
			stringBuilder.AppendLine("CMapName currentdict /CMap defineresource pop");
			stringBuilder.AppendLine("end");
			stringBuilder.Append("end");
			return stringBuilder.ToString();
		}

		private void WriteToUnicodeMap(EmbeddedFont embeddedFont)
		{
			IEnumerable<CMapMapping> glyphIdToUnicodeMapping = embeddedFont.GetGlyphIdToUnicodeMapping();
			string toUnicodeMap = this.GetToUnicodeMap(glyphIdToUnicodeMapping);
			string text = this.CompressString(toUnicodeMap);
			int toUnicodeId = embeddedFont.ToUnicodeId;
			this.UpdateCrossRefPosition(toUnicodeId);
			this.Write("\r\n");
			this.Write(toUnicodeId);
			this.Write(" 0 obj");
			this.Write("\r\n");
			this.Write("<< /Filter /FlateDecode ");
			this.Write(" /Length ");
			this.Write(text.Length);
			this.Write(" /Length1 ");
			this.Write(toUnicodeMap.Length);
			this.Write(" >>");
			this.Write("\r\n");
			this.Write("stream");
			this.Write("\r\n");
			this.Write(text);
			this.Write("\r\n");
			this.Write("endstream");
			this.Write("\r\n");
			this.Write("endobj");
		}

		private void WriteFont(PDFFont pdfFont)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<< /Type /Font ");
			if (!pdfFont.IsComposite && pdfFont.InternalFont)
			{
				stringBuilder.Append("/Subtype /Type1 /BaseFont /");
				stringBuilder.Append(pdfFont.FontPDFFamily);
				stringBuilder.Append(" /Encoding /WinAnsiEncoding");
				stringBuilder.Append(" >>");
				this.WriteObject(pdfFont.FontId, stringBuilder.ToString());
				stringBuilder = null;
			}
			else
			{
				int num = -1;
				int num2 = -1;
				Win32ObjectSafeHandle win32ObjectSafeHandle = Win32ObjectSafeHandle.Zero;
				Win32DCSafeHandle hdc = base.m_commonGraphics.GetHdc();
				Win32ObjectSafeHandle win32ObjectSafeHandle2 = Win32ObjectSafeHandle.Zero;
				try
				{
					Font font = new Font(pdfFont.FontFamily, (float)pdfFont.EMHeight, pdfFont.GDIFontStyle, GraphicsUnit.World);
					win32ObjectSafeHandle = new Win32ObjectSafeHandle(font.ToHfont(), true);
					win32ObjectSafeHandle2 = AspNetCore.ReportingServices.Rendering.RichText.Win32.SelectObject(hdc, win32ObjectSafeHandle);
					if (!string.IsNullOrEmpty(pdfFont.FontCMap))
					{
						stringBuilder.Append("/Subtype /Type0 /BaseFont /");
						stringBuilder.Append(pdfFont.FontPDFFamily);
						stringBuilder.Append(" /Encoding /");
						stringBuilder.Append(pdfFont.FontCMap);
						num = this.ReserveObjectId();
						stringBuilder.Append(" /DescendantFonts [");
						PDFWriter.Write(stringBuilder, num);
						stringBuilder.Append(" 0 R]");
						if (pdfFont.EmbeddedFont != null)
						{
							stringBuilder.Append(" /ToUnicode ");
							PDFWriter.Write(stringBuilder, pdfFont.EmbeddedFont.ToUnicodeId);
							stringBuilder.Append(" 0 R");
						}
					}
					else
					{
						stringBuilder.Append("/Subtype /TrueType /FirstChar 0 /LastChar 255 /Widths [");
						AspNetCore.ReportingServices.Rendering.RichText.Win32.ABCFloat[] array = new AspNetCore.ReportingServices.Rendering.RichText.Win32.ABCFloat[256];
						if (AspNetCore.ReportingServices.Rendering.RichText.Win32.GetCharABCWidthsFloat(hdc, 0u, 255u, array) == 0)
						{
							throw new ReportRenderingException(ErrorCode.rrRenderingError);
						}
						for (int i = 0; i <= 255; i++)
						{
							float num3 = (float)Math.Round((double)(array[i].abcfA + array[i].abcfB + array[i].abcfC));
							num3 *= pdfFont.EMGridConversion;
							stringBuilder.Append(" ");
							PDFWriter.Write(stringBuilder, (int)num3);
						}
						stringBuilder.Append(" ] /Encoding /WinAnsiEncoding /BaseFont /");
						stringBuilder.Append(pdfFont.FontPDFFamily);
						num2 = this.ReserveObjectId();
						stringBuilder.Append(" /FontDescriptor ");
						PDFWriter.Write(stringBuilder, num2);
						stringBuilder.Append(" 0 R");
					}
					stringBuilder.Append(" >>");
					this.WriteObject(pdfFont.FontId, stringBuilder.ToString());
					stringBuilder = null;
					if (num != -1)
					{
						StringBuilder stringBuilder2 = new StringBuilder();
						stringBuilder2.Append("<< /Type /Font /Subtype /CIDFontType2 /BaseFont /");
						stringBuilder2.Append(pdfFont.FontPDFFamily);
						stringBuilder2.Append(" /CIDSystemInfo << /Registry (");
						stringBuilder2.Append(pdfFont.Registry);
						stringBuilder2.Append(") /Ordering (");
						stringBuilder2.Append(pdfFont.Ordering);
						stringBuilder2.Append(") /Supplement ");
						stringBuilder2.Append(pdfFont.Supplement);
						stringBuilder2.Append(" >> ");
						if (pdfFont.FontCMap == "Identity-H")
						{
							stringBuilder2.Append("/W [");
							for (int j = 0; j < pdfFont.UniqueGlyphs.Count; j++)
							{
								PDFFont.GlyphData glyphData = pdfFont.UniqueGlyphs[j];
								PDFWriter.Write(stringBuilder2, glyphData.Glyph);
								stringBuilder2.Append(" [");
								PDFWriter.Write(stringBuilder2, glyphData.Width);
								stringBuilder2.Append("] ");
							}
							stringBuilder2.Append("]");
						}
						num2 = this.ReserveObjectId();
						stringBuilder2.Append(" /FontDescriptor ");
						PDFWriter.Write(stringBuilder2, num2);
						stringBuilder2.Append(" 0 R >>");
						this.WriteObject(num, stringBuilder2.ToString());
					}
					if (num2 != -1)
					{
						AspNetCore.ReportingServices.Rendering.RichText.Win32.OutlineTextMetric outlineTextMetric = default(AspNetCore.ReportingServices.Rendering.RichText.Win32.OutlineTextMetric);
						if (AspNetCore.ReportingServices.Rendering.RichText.Win32.GetOutlineTextMetrics(hdc, (uint)Marshal.SizeOf(outlineTextMetric), ref outlineTextMetric) == 0)
						{
							throw new ReportRenderingException(ErrorCode.rrRenderingError);
						}
						StringBuilder stringBuilder3 = new StringBuilder();
						stringBuilder3.Append("<< /Type /FontDescriptor /Ascent ");
						PDFWriter.Write(stringBuilder3, (int)Math.Round((double)((float)outlineTextMetric.otmAscent * pdfFont.EMGridConversion)));
						stringBuilder3.Append(" /CapHeight 0 /Descent ");
						PDFWriter.Write(stringBuilder3, (int)Math.Round((double)((float)outlineTextMetric.otmDescent * pdfFont.EMGridConversion)));
						stringBuilder3.Append(" /Flags 32 /FontBBox [ ");
						PDFWriter.Write(stringBuilder3, (int)Math.Round((double)((float)outlineTextMetric.left * pdfFont.EMGridConversion)));
						stringBuilder3.Append(" ");
						PDFWriter.Write(stringBuilder3, (int)Math.Round((double)((float)outlineTextMetric.bottom * pdfFont.EMGridConversion)));
						stringBuilder3.Append(" ");
						PDFWriter.Write(stringBuilder3, (int)Math.Round((double)((float)outlineTextMetric.right * pdfFont.EMGridConversion)));
						stringBuilder3.Append(" ");
						PDFWriter.Write(stringBuilder3, (int)Math.Round((double)((float)outlineTextMetric.top * pdfFont.EMGridConversion)));
						stringBuilder3.Append(" ] /FontName /");
						stringBuilder3.Append(pdfFont.FontPDFFamily);
						stringBuilder3.Append(" /ItalicAngle ");
						PDFWriter.Write(stringBuilder3, (int)Math.Round((double)((float)outlineTextMetric.otmItalicAngle * pdfFont.EMGridConversion)));
						stringBuilder3.Append(" /StemV 0 ");
						if (pdfFont.EmbeddedFont != null)
						{
							stringBuilder3.Append(" /FontFile2 ");
							PDFWriter.Write(stringBuilder3, pdfFont.EmbeddedFont.ObjectId);
							stringBuilder3.Append(" 0 R ");
						}
						stringBuilder3.Append(">>");
						this.WriteObject(num2, stringBuilder3.ToString());
					}
				}
				finally
				{
					if (!win32ObjectSafeHandle.IsInvalid)
					{
						win32ObjectSafeHandle.Close();
					}
					if (!win32ObjectSafeHandle2.IsInvalid)
					{
						win32ObjectSafeHandle2.SetHandleAsInvalid();
					}
					base.m_commonGraphics.ReleaseHdc();
				}
			}
		}

		private void ProcessFontForFontEmbedding(PDFFont pdfFont, Dictionary<string, EmbeddedFont> embeddedFonts)
		{
			pdfFont.FontPDFFamily = pdfFont.FontPDFFamily.Replace("+UnicodeFont", "");
			if (pdfFont.SimulateItalic || pdfFont.SimulateBold)
			{
				int num = pdfFont.FontPDFFamily.LastIndexOf(',');
				if (num != -1)
				{
					if (pdfFont.SimulateItalic)
					{
						int num2 = pdfFont.FontPDFFamily.IndexOf("Italic", num, StringComparison.Ordinal);
						if (num2 != -1)
						{
							pdfFont.FontPDFFamily = pdfFont.FontPDFFamily.Remove(num2, "Italic".Length);
						}
					}
					if (pdfFont.SimulateBold)
					{
						int num3 = pdfFont.FontPDFFamily.IndexOf("Bold", num, StringComparison.Ordinal);
						if (num3 != -1)
						{
							pdfFont.FontPDFFamily = pdfFont.FontPDFFamily.Remove(num3, "Bold".Length);
						}
					}
					if (pdfFont.FontPDFFamily.Length - 1 == num)
					{
						pdfFont.FontPDFFamily = pdfFont.FontPDFFamily.Remove(num);
					}
				}
			}
			if (!pdfFont.IsComposite && pdfFont.InternalFont)
			{
				return;
			}
			bool flag = this.EmbedFonts == FontEmbedding.Subset;
			if (flag)
			{
				flag = (pdfFont.UniqueGlyphs.Count > 0);
				if (flag)
				{
					Win32DCSafeHandle hdc = base.m_commonGraphics.GetHdc();
					Win32ObjectSafeHandle win32ObjectSafeHandle = Win32ObjectSafeHandle.Zero;
					try
					{
						win32ObjectSafeHandle = AspNetCore.ReportingServices.Rendering.RichText.Win32.SelectObject(hdc, pdfFont.CachedFont.Hfont);
						flag = FontPackage.CheckEmbeddingRights(hdc);
					}
					finally
					{
						if (!win32ObjectSafeHandle.IsInvalid)
						{
							win32ObjectSafeHandle.SetHandleAsInvalid();
						}
						base.m_commonGraphics.ReleaseHdc();
					}
					if (!flag && RSTrace.ImageRendererTracer.TraceVerbose)
					{
						RSTrace.ImageRendererTracer.Trace(TraceLevel.Verbose, "The font {0} cannot be embedded due to privileges", pdfFont.FontFamily);
					}
				}
			}
			if (flag)
			{
				EmbeddedFont embeddedFont = default(EmbeddedFont);
				if (!embeddedFonts.TryGetValue(pdfFont.FontPDFFamily, out embeddedFont))
				{
					embeddedFont = new EmbeddedFont(this.ReserveObjectId(), this.ReserveObjectId());
					embeddedFonts.Add(pdfFont.FontPDFFamily, embeddedFont);
				}
				embeddedFont.PDFFonts.Add(pdfFont);
				pdfFont.EmbeddedFont = embeddedFont;
			}
		}

		private static void WriteHex(StringBuilder sb, int value)
		{
			string text = Convert.ToString(value, 16);
			if (text.Length != 2)
			{
				sb.Append("0");
			}
			sb.Append(text);
		}

		private static void WriteImage(StringBuilder sb, int id, RectangleF bounds, SizeF size)
		{
			PDFWriter.WriteImage(sb, id, bounds, bounds.Left, bounds.Top - bounds.Height, size);
		}

		private static void WriteImage(StringBuilder sb, int id, RectangleF bounds, float left, float bottom, SizeF size)
		{
			PDFWriter.WriteClipBounds(sb, bounds.Left, bounds.Top - bounds.Height, bounds.Size);
			sb.Append(" ");
			PDFWriter.Write(sb, size.Width);
			sb.Append(" 0 0 ");
			PDFWriter.Write(sb, size.Height);
			sb.Append(" ");
			PDFWriter.Write(sb, left);
			sb.Append(" ");
			PDFWriter.Write(sb, bottom);
			sb.Append(" cm /Im");
			PDFWriter.Write(sb, id);
			sb.Append(" Do Q");
		}

		private int WriteObject(string pdfObject)
		{
			return this.WriteObject(this.m_nextObjectId++, pdfObject);
		}

		private int WriteObject(int objectId, string pdfObject)
		{
			this.UpdateCrossRefPosition(objectId);
			this.Write("\r\n");
			this.Write(objectId);
			this.Write(" 0 obj");
			this.Write("\r\n");
			this.Write(pdfObject);
			this.Write("\r\n");
			this.Write("endobj");
			return objectId;
		}

		private void WriteFontBuffer(int objectId, byte[] buffer)
		{
			this.UpdateCrossRefPosition(objectId);
			byte[] array = this.CompressBytes(buffer);
			this.Write("\r\n");
			this.Write(objectId);
			this.Write(" 0 obj");
			this.Write("\r\n");
			this.Write("<< /Filter /FlateDecode ");
			this.Write(" /Length ");
			this.Write(array.Length);
			this.Write(" /Length1 ");
			this.Write(buffer.Length);
			this.Write(" >>");
			this.Write("\r\n");
			this.Write("stream");
			this.Write("\r\n");
			this.Write(array);
			this.Write("\r\n");
			this.Write("endstream");
			this.Write("\r\n");
			this.Write("endobj");
		}

		private static void WriteRectangle(StringBuilder sb, float left, float bottom, SizeF size)
		{
			PDFWriter.Write(sb, left);
			sb.Append(" ");
			PDFWriter.Write(sb, bottom);
			sb.Append(" ");
			PDFWriter.Write(sb, size.Width);
			sb.Append(" ");
			PDFWriter.Write(sb, size.Height);
		}

		private static void WriteRectangle(StringBuilder sb, float left, float bottom, float right, float top)
		{
			PDFWriter.Write(sb, left);
			sb.Append(" ");
			PDFWriter.Write(sb, bottom);
			sb.Append(" ");
			PDFWriter.Write(sb, right);
			sb.Append(" ");
			PDFWriter.Write(sb, top);
		}

		private static void WriteSizeAndStyle(StringBuilder sb, float size, RPLFormat.BorderStyles style)
		{
			PDFWriter.Write(sb, (float)(size * 2.8346459865570068));
			sb.Append(" w ");
			switch (style)
			{
			case RPLFormat.BorderStyles.Dashed:
				sb.Append("[7 4] 0 d ");
				break;
			case RPLFormat.BorderStyles.Dotted:
				sb.Append("[2 3] 0 d ");
				break;
			default:
				sb.Append("[] 0 d ");
				break;
			}
		}

		private void WriteUnicodeString(StringBuilder sb, string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				sb.Append("<>");
			}
			else
			{
				sb.Append('<');
				PDFWriter.WriteHex(sb, 254);
				PDFWriter.WriteHex(sb, 255);
				byte[] bytes = this.m_unicodeEncoding.GetBytes(text);
				for (int i = 0; i < bytes.Length; i++)
				{
					char value = (char)bytes[i];
					PDFWriter.WriteHex(sb, value);
				}
				sb.Append('>');
			}
		}

		private void WriteComment(StringBuilder sb, string comment)
		{
			sb.Append('%');
			sb.Append(comment);
			sb.Append("\r\n");
		}

		internal override void ClipTextboxRectangle(Win32DCSafeHandle hdc, RectangleF textposition)
		{
			RectangleF rectangleF = this.ConvertToPDFUnits(textposition);
			StringBuilder stringBuilder = new StringBuilder();
			PDFWriter.WriteClipBounds(stringBuilder, rectangleF.Left, rectangleF.Top - rectangleF.Height, rectangleF.Size);
			this.m_pageContentsSection.Add(stringBuilder.ToString());
		}

		internal override void UnClipTextboxRectangle(Win32DCSafeHandle hdc)
		{
			this.m_pageContentsSection.Add("\r\nQ");
		}
	}
}
