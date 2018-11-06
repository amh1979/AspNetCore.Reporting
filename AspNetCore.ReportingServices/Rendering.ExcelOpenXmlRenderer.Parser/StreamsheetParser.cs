using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser
{
	internal sealed class StreamsheetParser : WorksheetPart
	{
		private delegate void WriteDelegate(TextWriter s, int depth, Dictionary<string, string> namespaces);

		internal enum StreamSheetSection
		{
			Prelude,
			SheetPr,
			SheetViews,
			SheetFormatPr,
			Cols,
			SheetData,
			MergeCells,
			Hyperlinks,
			PageMargins,
			PageSetup,
			HeaderFooter,
			Drawing,
			Picture,
			AfterWord
		}

		private const int ROOT_DEPTH = 0;

		private readonly WriteDelegate[] _sectionsInOrder;

		private readonly CT_Worksheet _worksheet;

		private StreamSheetSection _currentSection;

		private string _pendingCloseTag;

		private TextWriter _output;

		public StreamSheetSection CurrentSection
		{
			get
			{
				return this._currentSection;
			}
		}

		public StreamsheetParser(TextWriter output, bool startAtPrelude)
		{
			this._output = output;
			this._worksheet = (CT_Worksheet)this.Root;
			this._sectionsInOrder = new WriteDelegate[14]
			{
				this.WritePrelude,
				this._worksheet.Write_sheetPr,
				this._worksheet.Write_sheetViews,
				this._worksheet.Write_sheetFormatPr,
				this._worksheet.Write_cols,
				this._worksheet.Write_sheetData,
				this._worksheet.Write_mergeCells,
				this._worksheet.Write_hyperlinks,
				this._worksheet.Write_pageMargins,
				this._worksheet.Write_pageSetup,
				this._worksheet.Write_headerFooter,
				this._worksheet.Write_drawing,
				this._worksheet.Write_picture,
				this.WriteAfterword
			};
			this._pendingCloseTag = null;
			if (startAtPrelude)
			{
				this._currentSection = StreamSheetSection.Prelude;
			}
			else
			{
				this.TryMoveToSection(StreamSheetSection.SheetData, 1);
				this._pendingCloseTag = CT_Worksheet.SheetDataElementName;
				new CT_SheetData().WriteOpenTag(this._output, CT_Worksheet.SheetDataElementName, 1, this.Namespaces, false);
			}
		}

		private void TryMoveToSection(StreamSheetSection section, int depth)
		{
			if (this._currentSection != section)
			{
				if (section <= this._currentSection)
				{
					throw new FatalException();
				}
				this.FinishCurrentTag();
				this.FastForward(section, depth);
			}
		}

		private void FastForward(StreamSheetSection section, int depth)
		{
			for (int i = (int)(this._currentSection + 1); i <= (int)(section - 1); i++)
			{
				this._sectionsInOrder[i](this._output, depth, this.Namespaces);
			}
			this._currentSection = section;
		}

		public void FinishCurrentTag()
		{
			if (this._pendingCloseTag != null)
			{
				this._output.Write(string.Format(CultureInfo.InvariantCulture, "</{0}>", this._pendingCloseTag));
				this._pendingCloseTag = null;
			}
			this._output.Flush();
		}

		public void WritePrelude()
		{
			this.WritePrelude(this._output, 0, this.Namespaces);
		}

		private void WritePrelude(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			this.TryMoveToSection(StreamSheetSection.Prelude, depth);
			s.Write(OoxmlPart.XmlDeclaration);
			this._worksheet.WriteOpenTag(s, this.Tag, 0, this.Namespaces, true);
		}

		public void WriteAfterword()
		{
			this.WriteAfterword(this._output, 0, this.Namespaces);
			this._output.Flush();
		}

		private void WriteAfterword(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			this.TryMoveToSection(StreamSheetSection.AfterWord, depth);
			this._worksheet.WriteCloseTag(s, this.Tag, 0, this.Namespaces);
		}

		private void WriteTagInCollection<TCol>(OoxmlComplexType tag, StreamSheetSection section, string tagname, string coltagname) where TCol : OoxmlComplexType, new()
		{
			if (tag != null)
			{
				if (this._currentSection != section)
				{
					this.TryMoveToSection(section, 1);
					this._pendingCloseTag = coltagname;
					TCol val = new TCol();
					val.WriteOpenTag(this._output, coltagname, 1, this.Namespaces, false);
				}
				tag.Write(this._output, tagname, 2, this.Namespaces);
			}
		}

		private void WriteStandaloneTag(OoxmlComplexType tag, StreamSheetSection section, string tagname)
		{
			if (tag != null)
			{
				if (this._currentSection != section)
				{
					this.TryMoveToSection(section, 1);
				}
				tag.Write(this._output, tagname, 1, this.Namespaces);
			}
		}

		public void WriteSheetProperties(CT_SheetPr props)
		{
			this.WriteStandaloneTag(props, StreamSheetSection.SheetPr, CT_Worksheet.SheetPrElementName);
		}

		public void WriteSheetView(CT_SheetView view)
		{
			this.WriteTagInCollection<CT_SheetViews>(view, StreamSheetSection.SheetViews, CT_SheetViews.SheetViewElementName, CT_Worksheet.SheetViewsElementName);
		}

		public void WriteSheetFormatProperties(CT_SheetFormatPr props)
		{
			this.WriteStandaloneTag(props, StreamSheetSection.SheetFormatPr, CT_Worksheet.SheetFormatPrElementName);
		}

		public void WriteColumnProperty(CT_Col prop)
		{
			this.WriteTagInCollection<CT_Cols>(prop, StreamSheetSection.Cols, CT_Cols.ColElementName, CT_Worksheet.ColsElementName);
		}

		public void WriteRow(CT_Row row)
		{
			this.WriteTagInCollection<CT_SheetData>(row, StreamSheetSection.SheetData, CT_SheetData.RowElementName, CT_Worksheet.SheetDataElementName);
		}

		public void WriteMergedCell(CT_MergeCell merge)
		{
			this.WriteTagInCollection<CT_MergeCells>(merge, StreamSheetSection.MergeCells, CT_MergeCells.MergeCellElementName, CT_Worksheet.MergeCellsElementName);
		}

		public void WriteHyperlink(CT_Hyperlink link)
		{
			this.WriteTagInCollection<CT_Hyperlinks>(link, StreamSheetSection.Hyperlinks, CT_Hyperlinks.HyperlinkElementName, CT_Worksheet.HyperlinksElementName);
		}

		public void WritePageMargins(CT_PageMargins margins)
		{
			this.WriteStandaloneTag(margins, StreamSheetSection.PageMargins, CT_Worksheet.PageMarginsElementName);
		}

		public void WritePageSetup(CT_PageSetup setup)
		{
			this.WriteStandaloneTag(setup, StreamSheetSection.PageSetup, CT_Worksheet.PageSetupElementName);
		}

		public void WriteHeaderFooter(CT_HeaderFooter hf)
		{
			this.WriteStandaloneTag(hf, StreamSheetSection.HeaderFooter, CT_Worksheet.HeaderFooterElementName);
		}

		public void WriteDrawing(CT_Drawing drawing)
		{
			this.WriteStandaloneTag(drawing, StreamSheetSection.Drawing, CT_Worksheet.DrawingElementName);
		}

		public void WritePicture(CT_SheetBackgroundPicture picture)
		{
			this.WriteStandaloneTag(picture, StreamSheetSection.Picture, CT_Worksheet.PictureElementName);
		}

		public void WriteTag(OoxmlComplexType tag)
		{
			if (tag != null)
			{
				if (!(tag is CT_SheetPr))
				{
					if (tag is CT_SheetView)
					{
						this.WriteSheetView((CT_SheetView)tag);
						return;
					}
					if (tag is CT_SheetFormatPr)
					{
						this.WriteSheetFormatProperties((CT_SheetFormatPr)tag);
						return;
					}
					if (tag is CT_Col)
					{
						this.WriteColumnProperty((CT_Col)tag);
						return;
					}
					if (tag is CT_Row)
					{
						this.WriteRow((CT_Row)tag);
						return;
					}
					if (tag is CT_MergeCell)
					{
						this.WriteMergedCell((CT_MergeCell)tag);
						return;
					}
					if (tag is CT_Hyperlink)
					{
						this.WriteHyperlink((CT_Hyperlink)tag);
						return;
					}
					if (tag is CT_PageMargins)
					{
						this.WritePageMargins((CT_PageMargins)tag);
						return;
					}
					if (tag is CT_PageSetup)
					{
						this.WritePageSetup((CT_PageSetup)tag);
						return;
					}
					if (tag is CT_HeaderFooter)
					{
						this.WriteHeaderFooter((CT_HeaderFooter)tag);
						return;
					}
					if (tag is CT_Drawing)
					{
						this.WriteDrawing((CT_Drawing)tag);
						return;
					}
					if (tag is CT_SheetBackgroundPicture)
					{
						this.WritePicture((CT_SheetBackgroundPicture)tag);
						return;
					}
					throw new FatalException();
				}
				this.WriteSheetProperties((CT_SheetPr)tag);
			}
		}
	}
}
