using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Models;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Archive;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Relationships;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Packaging;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLStreamsheetModel : XMLWorksheetModel, IStreamsheetModel, IWorksheetModel, ICloneable
	{
		private readonly Streamsheet _interface;

		private string _partName;

		private Stream _headStream;

		private Stream _tailStream;

		private StreamsheetParser _headWriter;

		private StreamsheetParser _tailWriter;

		private bool _complete;

		private int _currentColNumber = -1;

		private int _currentRowNumber = -1;

		private IOoxmlCtWrapperModel _stagedHeadTag;

		private IOoxmlCtWrapperModel _stagedTailTag;

		private AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main.CT_Drawing _stagedDrawingTag;

		private CT_SheetBackgroundPicture _stagedSheetBackgroundTag;

		private XMLPageSetupModel _stagedPageSetupModel;

		private CT_SheetView _stagedSheetView;

		private static readonly object _writeLock = new object();

		public override Streamsheet Interface
		{
			get
			{
				return this._interface;
			}
		}

		public int MaxRowIndex
		{
			get
			{
				return 1048575;
			}
		}

		public int MaxColIndex
		{
			get
			{
				return 16383;
			}
		}

		public override IPictureShapesModel Pictures
		{
			get
			{
				if (base.PicturesModel == null)
				{
					if (this._tailWriter.CurrentSection > StreamsheetParser.StreamSheetSection.Drawing)
					{
						throw new FatalException();
					}
					WsDrPart part = new WsDrPart();
					Relationship relationship = base._manager.AddPartToTree(part, "application/vnd.openxmlformats-officedocument.drawing+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing", "xl/drawings/drawing{0}.xml", base._manager.GetWorksheetXmlPart(base._sheetentry));
					this._stagedDrawingTag = new AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main.CT_Drawing();
					this._stagedDrawingTag.Id_Attr = relationship.RelationshipId;
					base.PicturesModel = new XMLPictureShapesModel(base._manager, base._sheetentry, relationship.RelationshipId);
				}
				return base.PicturesModel;
			}
		}

		public override IPageSetupModel PageSetup
		{
			get
			{
				if (this._tailWriter.CurrentSection > StreamsheetParser.StreamSheetSection.HeaderFooter)
				{
					throw new FatalException();
				}
				if (this._stagedPageSetupModel == null)
				{
					this._stagedPageSetupModel = new XMLPageSetupModel(new CT_Worksheet(), this);
				}
				return this._stagedPageSetupModel;
			}
		}

		public override bool ShowGridlines
		{
			set
			{
				if (this._headWriter.CurrentSection > StreamsheetParser.StreamSheetSection.SheetViews)
				{
					throw new FatalException();
				}
				if (this._stagedSheetView == null)
				{
					this._stagedSheetView = new CT_SheetView();
				}
				this._stagedSheetView.ShowGridLines_Attr = value;
			}
		}

		public XMLStreamsheetModel(XMLWorkbookModel workbook, XMLWorksheetsModel sheets, PartManager manager, string name, ExcelGeneratorConstants.CreateTempStream createTempStream)
		{
			base._workbookModel = workbook;
			base._worksheetsModel = sheets;
			base._manager = manager;
			this._interface = new Streamsheet(this);
			Relationship relationship = base._manager.AddStreamingPartToTree("application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet", "xl/worksheets/sheet{0}.xml", (XmlPart)base._manager.GetPartByContentType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"));
			List<CT_Sheet> sheet = ((CT_Workbook)base._manager.Workbook.Root).Sheets.Sheet;
			base._sheetentry = new CT_Sheet();
			base._sheetentry.Id_Attr = relationship.RelationshipId;
			base._sheetentry.Name_Attr = name;
			base._sheetentry.SheetId_Attr = sheets.NextId;
			sheet.Add(base._sheetentry);
			this._partName = relationship.RelatedPart;
			this._headStream = createTempStream(string.Format(CultureInfo.InvariantCulture, "streamsheetHead{0}", base._sheetentry.SheetId_Attr));
			this._tailStream = createTempStream(string.Format(CultureInfo.InvariantCulture, "streamsheetTail{0}", base._sheetentry.SheetId_Attr));
			this._headWriter = new StreamsheetParser(new StreamWriter(this._headStream), true);
			this._tailWriter = new StreamsheetParser(new StreamWriter(this._tailStream), false);
			this._headWriter.WritePrelude();
		}

		public override IColumnModel getColumn(int index)
		{
			this.AdvanceStreamsheetTo(StreamsheetParser.StreamSheetSection.Cols, true);
			if (index <= this._currentColNumber)
			{
				throw new FatalException();
			}
			this._currentColNumber = index;
			return (IColumnModel)(this._stagedHeadTag = new XMLColumnModel(new CT_Col(), index + 1));
		}

		public IRowModel CreateRow()
		{
			return this.CreateRow(this._currentRowNumber + 1);
		}

		public IRowModel CreateRow(int index)
		{
			this.AdvanceStreamsheetTo(StreamsheetParser.StreamSheetSection.SheetData, false);
			if (index <= this._currentRowNumber)
			{
				throw new FatalException();
			}
			this._currentRowNumber = index;
			return (IRowModel)(this._stagedTailTag = new XMLRowModel(this, base._manager, index));
		}

		public void MergeCells(int firstRow, int firstCol, int rowCount, int colCount)
		{
			this.AdvanceStreamsheetTo(StreamsheetParser.StreamSheetSection.MergeCells, false);
			CT_MergeCell cT_MergeCell = new CT_MergeCell();
			cT_MergeCell._ref_Attr = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", CellPair.Name(firstRow, firstCol), CellPair.Name(firstRow + rowCount - 1, firstCol + colCount - 1));
			this._tailWriter.WriteMergedCell(cT_MergeCell);
		}

		public void CreateHyperlink(string areaFormula, string href, string label)
		{
			this.AdvanceStreamsheetTo(StreamsheetParser.StreamSheetSection.Hyperlinks, false);
			XMLHyperlinkModel xMLHyperlinkModel = (XMLHyperlinkModel)(this._stagedTailTag = new XMLHyperlinkModel(areaFormula, href, label, base._manager, base._sheetentry));
		}

		public override void SetBackgroundPicture(string uniqueId, string extension, Stream pictureStream)
		{
			if (this._tailWriter.CurrentSection > StreamsheetParser.StreamSheetSection.Picture)
			{
				throw new FatalException();
			}
			this._stagedSheetBackgroundTag = new CT_SheetBackgroundPicture();
			this._stagedSheetBackgroundTag.Id_Attr = base.InsertBackgroundPicture(uniqueId, extension, pictureStream);
		}

		public void SetFreezePanes(int row, int col)
		{
			if (this._headWriter.CurrentSection > StreamsheetParser.StreamSheetSection.SheetViews)
			{
				throw new FatalException();
			}
			if (this._stagedSheetView == null)
			{
				this._stagedSheetView = new CT_SheetView();
			}
			this._stagedSheetView.Pane = new CT_Pane();
			this._stagedSheetView.Pane = new CT_Pane();
			this._stagedSheetView.Pane.State_Attr = ST_PaneState.frozen;
			this._stagedSheetView.Pane.XSplit_Attr = (double)col;
			this._stagedSheetView.Pane.YSplit_Attr = (double)row;
			this._stagedSheetView.Pane.TopLeftCell_Attr = CellPair.Name(row, col);
		}

		public override void Cleanup()
		{
			if (!this._complete)
			{
				this.AdvanceStreamsheetTo(StreamsheetParser.StreamSheetSection.AfterWord, false);
				this.AdvanceStreamsheetTo(StreamsheetParser.StreamSheetSection.Cols, true);
				this._tailWriter.WriteAfterword();
				this._headWriter.FinishCurrentTag();
				this._tailStream.Seek(0L, SeekOrigin.Begin);
				this._headStream.Seek(0L, SeekOrigin.Begin);
				Package zipPackage = ((XMLStreambookModel)base.Workbook).ZipPackage;
				PackagePart part = zipPackage.GetPart(new Uri(Utils.CleanName(this._partName), UriKind.Relative));
				Stream stream = part.GetStream();
				lock (XMLStreamsheetModel._writeLock)
				{
					this.WriteStreamToStream(this._headStream, stream);
					this.WriteStreamToStream(this._tailStream, stream);
				}
				this._complete = true;
			}
		}

		private void WriteStreamToStream(Stream from, Stream to)
		{
			byte[] buffer = new byte[1024];
			long num = from.Length;
			while (num > 0)
			{
				int num2 = from.Read(buffer, 0, 1024);
				num -= num2;
				to.Write(buffer, 0, num2);
			}
		}

		private void CheckAdvancingStatus(StreamsheetParser.StreamSheetSection newSection, StreamsheetParser writer)
		{
			if (this._complete)
			{
				throw new FatalException();
			}
			if (writer.CurrentSection <= newSection)
			{
				return;
			}
			throw new FatalException();
		}

		private bool IsGoingPast(StreamsheetParser.StreamSheetSection currentSection, StreamsheetParser.StreamSheetSection newSection, StreamsheetParser.StreamSheetSection targetsection)
		{
			if (targetsection >= currentSection)
			{
				return newSection > targetsection;
			}
			return false;
		}

		private void AdvanceStreamsheetTo(StreamsheetParser.StreamSheetSection newSection, bool head)
		{
			StreamsheetParser streamsheetParser = head ? this._headWriter : this._tailWriter;
			IOoxmlCtWrapperModel ooxmlCtWrapperModel = head ? this._stagedHeadTag : this._stagedTailTag;
			this.CheckAdvancingStatus(newSection, streamsheetParser);
			if (ooxmlCtWrapperModel != null)
			{
				ooxmlCtWrapperModel.Cleanup();
				streamsheetParser.WriteTag(ooxmlCtWrapperModel.OoxmlTag);
				if (head)
				{
					this._stagedHeadTag = null;
				}
				else
				{
					this._stagedTailTag = null;
				}
			}
			if (this._stagedPageSetupModel != null && this.IsGoingPast(streamsheetParser.CurrentSection, newSection, StreamsheetParser.StreamSheetSection.SheetPr))
			{
				this._stagedPageSetupModel.Cleanup();
				streamsheetParser.WriteSheetProperties(this._stagedPageSetupModel.BackingSheet.SheetPr);
			}
			if (this._stagedSheetView != null && this.IsGoingPast(streamsheetParser.CurrentSection, newSection, StreamsheetParser.StreamSheetSection.SheetViews))
			{
				streamsheetParser.WriteSheetView(this._stagedSheetView);
				this._stagedSheetView = null;
			}
			if (this._stagedPageSetupModel != null && this.IsGoingPast(streamsheetParser.CurrentSection, newSection, StreamsheetParser.StreamSheetSection.PageMargins))
			{
				this._stagedPageSetupModel.Cleanup();
				streamsheetParser.WritePageMargins(this._stagedPageSetupModel.BackingSheet.PageMargins);
			}
			if (this._stagedPageSetupModel != null && this.IsGoingPast(streamsheetParser.CurrentSection, newSection, StreamsheetParser.StreamSheetSection.PageSetup))
			{
				this._stagedPageSetupModel.Cleanup();
				streamsheetParser.WritePageSetup(this._stagedPageSetupModel.BackingSheet.PageSetup);
			}
			if (this._stagedPageSetupModel != null && this.IsGoingPast(streamsheetParser.CurrentSection, newSection, StreamsheetParser.StreamSheetSection.HeaderFooter))
			{
				this._stagedPageSetupModel.Cleanup();
				if (this._stagedPageSetupModel.BackingSheet.HeaderFooter == null)
				{
					this._stagedPageSetupModel.BackingSheet.HeaderFooter = new CT_HeaderFooter();
				}
				this._stagedPageSetupModel.BackingSheet.HeaderFooter.AlignWithMargins_Attr = false;
				streamsheetParser.WriteHeaderFooter(this._stagedPageSetupModel.BackingSheet.HeaderFooter);
			}
			if (this._stagedDrawingTag != null && this.IsGoingPast(streamsheetParser.CurrentSection, newSection, StreamsheetParser.StreamSheetSection.Drawing))
			{
				streamsheetParser.WriteTag(this._stagedDrawingTag);
				this._stagedDrawingTag = null;
			}
			if (this._stagedSheetBackgroundTag != null && this.IsGoingPast(streamsheetParser.CurrentSection, newSection, StreamsheetParser.StreamSheetSection.Picture))
			{
				streamsheetParser.WriteTag(this._stagedSheetBackgroundTag);
				this._stagedSheetBackgroundTag = null;
			}
			this.CheckAdvancingStatus(newSection, streamsheetParser);
		}
	}
}
