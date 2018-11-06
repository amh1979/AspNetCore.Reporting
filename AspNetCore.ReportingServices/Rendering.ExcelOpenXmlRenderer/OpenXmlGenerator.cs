using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.Layout;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.Rendering.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal class OpenXmlGenerator : IExcelGenerator
	{
		internal struct MergeInfo
		{
			private int _firstRow;

			private int _rowCount;

			private int _firstColumn;

			private int _columnCount;

			public int FirstColumn
			{
				get
				{
					return this._firstColumn;
				}
			}

			public int FirstRow
			{
				get
				{
					return this._firstRow;
				}
			}

			public int RowCount
			{
				get
				{
					return this._rowCount;
				}
			}

			public int ColumnCount
			{
				get
				{
					return this._columnCount;
				}
			}

			public MergeInfo(int firstRow, int lastRow, int firstColumn, int lastColumn)
			{
				this._firstRow = firstRow;
				this._rowCount = lastRow - firstRow + 1;
				this._firstColumn = firstColumn;
				this._columnCount = lastColumn - firstColumn + 1;
			}
		}

		internal struct BookmarkTargetInfo
		{
			private readonly string _sheet;

			private readonly string _cell;

			public string Sheet
			{
				get
				{
					return this._sheet;
				}
			}

			public string Cell
			{
				get
				{
					return this._cell;
				}
			}

			public BookmarkTargetInfo(string sheet, string cell)
			{
				this._sheet = sheet;
				this._cell = cell;
			}
		}

		internal struct LinkInfo
		{
			private readonly string _areaFormula;

			private readonly string _href;

			private string _label;

			public string AreaFormula
			{
				get
				{
					return this._areaFormula;
				}
			}

			public string Href
			{
				get
				{
					return this._href;
				}
			}

			public string Label
			{
				get
				{
					return this._label;
				}
			}

			public LinkInfo(string areaFormula, string href, string label)
			{
				this._areaFormula = areaFormula;
				this._href = href;
				this._label = label;
			}
		}

		internal struct PictureLinkInfo
		{
			private readonly Picture _picture;

			private readonly string _target;

			public Picture Picture
			{
				get
				{
					return this._picture;
				}
			}

			public string Target
			{
				get
				{
					return this._target;
				}
			}

			public PictureLinkInfo(Picture picture, string target)
			{
				this._picture = picture;
				this._target = target;
			}
		}

		internal struct UnresolvedStreamsheet
		{
			private readonly Streamsheet _streamsheet;

			private readonly List<LinkInfo> _bookmarks;

			public Streamsheet Streamsheet
			{
				get
				{
					return this._streamsheet;
				}
			}

			public List<LinkInfo> Bookmarks
			{
				get
				{
					return this._bookmarks;
				}
			}

			public UnresolvedStreamsheet(Streamsheet sheet, List<LinkInfo> bookmarks)
			{
				this._streamsheet = sheet;
				this._bookmarks = bookmarks;
			}

			public bool ResolveTarget(string label, string target)
			{
				for (int num = this._bookmarks.Count - 1; num >= 0; num--)
				{
					if (this._bookmarks[num].Href == label)
					{
						this._streamsheet.CreateHyperlink(this._bookmarks[num].AreaFormula, target, this._bookmarks[num].Label);
						this._bookmarks.RemoveAt(num);
					}
				}
				if (this._bookmarks.Count == 0)
				{
					this._streamsheet.Cleanup();
					return true;
				}
				return false;
			}
		}

		private const int MAX_HEADER_FOOTER_SIZE = 255;

		private const int HEADER_FOOTER_SECTION_CODE_SIZE = 2;

		private const string CELL_BOOKMARK_FORMAT = "'{0}'!{1}";

		private const string PICTURE_BOOKMARK_FORMAT = "#{0}!{1}";

		private const int BIFF8_BLOCK_SIZE = 32;

		private const int MAX_OUTLINE_LEVEL = 7;

		private readonly ExcelApplication _application;

		private Workbook _workbook;

		private Streamsheet _currentSheet;

		private Row _currentRow;

		private Cell _currentCell;

		private bool _currentCellHasBeenModified;

		private RichTextInfo _currentRichText;

		private ExcelGeneratorConstants.CreateTempStream _createTempStream;

		private Stream _workbookStream;

		private Dictionary<string, int> _worksheetNames;

		private List<MergeInfo> _mergedCells;

		private List<LinkInfo> _hyperlinks;

		private List<LinkInfo> _bookmarkLinks;

		private List<UnresolvedStreamsheet> _unresolvedStreamsheets;

		private Dictionary<string, BookmarkTargetInfo> _bookmarkTargets;

		private Style _currentStyle;

		private Style _cachedStyle;

		private Dictionary<string, Style> _cachedstyles;

		private int _nextPaletteIndex;

		private bool _checkForRotatedEastAsianChars;

		private List<Picture> _picturesStartingOnCurrentRow;

		private Dictionary<int, List<Picture>> _picturesToUpdateByEndRow;

		private Dictionary<int, List<Picture>> _picturesToUpdateByStartColumn;

		private Dictionary<int, List<Picture>> _picturesToUpdateByEndColumn;

		private List<PictureLinkInfo> _unresolvedPictureBookmarks;

		private int _maxRowIndex;

		private int _currentColumn;

		public int MaxRows
		{
			get
			{
				return this._currentSheet.MaxRowIndex;
			}
		}

		public int MaxColumns
		{
			get
			{
				return this._currentSheet.MaxColIndex;
			}
		}

		public int RowBlockSize
		{
			get
			{
				return 32;
			}
		}

		public OpenXmlGenerator(ExcelGeneratorConstants.CreateTempStream createTempStream)
		{
			this._createTempStream = createTempStream;
			this._application = new ExcelApplication();
			this._workbookStream = this._createTempStream("oxmlWorkbook");
			this._workbook = this._application.CreateStreaming(this._workbookStream);
			this.CreateNewSheet();
			this._worksheetNames = new Dictionary<string, int>();
			this._worksheetNames.Add(ExcelRenderRes.SheetName.ToUpperInvariant(), 1);
			this._mergedCells = new List<MergeInfo>();
			this._hyperlinks = new List<LinkInfo>();
			this._bookmarkLinks = new List<LinkInfo>();
			this._unresolvedStreamsheets = new List<UnresolvedStreamsheet>();
			this._bookmarkTargets = new Dictionary<string, BookmarkTargetInfo>();
			this._cachedstyles = new Dictionary<string, Style>();
			this._nextPaletteIndex = 0;
			this._picturesStartingOnCurrentRow = new List<Picture>();
			this._picturesToUpdateByEndRow = new Dictionary<int, List<Picture>>();
			this._picturesToUpdateByStartColumn = new Dictionary<int, List<Picture>>();
			this._picturesToUpdateByEndColumn = new Dictionary<int, List<Picture>>();
			this._unresolvedPictureBookmarks = new List<PictureLinkInfo>();
		}

		private void CreateNewSheet()
		{
			this._currentSheet = this._workbook.Worksheets.CreateStreamsheet(ExcelRenderRes.SheetName + (this._workbook.Worksheets.Count + 1).ToString(CultureInfo.InvariantCulture), this._createTempStream);
			this._currentSheet.ShowGridlines = false;
		}

		private bool CheckForSheetNameConflict(string pageName)
		{
			if (pageName.Length >= ExcelRenderRes.SheetName.Length && pageName.StartsWith(ExcelRenderRes.SheetName, StringComparison.OrdinalIgnoreCase))
			{
				int num = 0;
				return int.TryParse(pageName.Substring(ExcelRenderRes.SheetName.Length - 1), out num);
			}
			return false;
		}

		private string GetUsablePortionOfHeaderFooterString(string candidate, ref int spaceRemaining)
		{
			if (!string.IsNullOrEmpty(candidate) && spaceRemaining > 2)
			{
				StringBuilder stringBuilder = new StringBuilder(candidate.Length);
				ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(candidate, stringBuilder);
				int num = Math.Min(spaceRemaining - 2, stringBuilder.Length);
				spaceRemaining -= num + 2;
				return stringBuilder.ToString(0, num);
			}
			return null;
		}

		private void FinalizeWorksheet()
		{
			this.FinalizeCell();
			for (int i = this._currentRow.RowNumber; i < this._maxRowIndex; i++)
			{
				this._currentSheet.CreateRow().Height = 0.0;
			}
			this._maxRowIndex = 0;
			foreach (MergeInfo mergedCell in this._mergedCells)
			{
				this._currentSheet.MergeCells(mergedCell.FirstRow, mergedCell.FirstColumn, mergedCell.RowCount, mergedCell.ColumnCount);
			}
			this._mergedCells = new List<MergeInfo>();
			foreach (LinkInfo hyperlink in this._hyperlinks)
			{
				this._currentSheet.CreateHyperlink(hyperlink.AreaFormula, hyperlink.Href, hyperlink.Label);
			}
			this._hyperlinks.Clear();
			for (int num = this._bookmarkLinks.Count - 1; num >= 0; num--)
			{
				BookmarkTargetInfo bookmark = default(BookmarkTargetInfo);
				if (this._bookmarkTargets.TryGetValue(this._bookmarkLinks[num].Href, out bookmark))
				{
					this._currentSheet.CreateHyperlink(this._bookmarkLinks[num].AreaFormula, this.GetCellBookmarkLink(bookmark), this._bookmarkLinks[num].Label);
					this._bookmarkLinks.RemoveAt(num);
				}
			}
			if (this._bookmarkLinks.Count == 0)
			{
				this._currentSheet.Cleanup();
			}
			else
			{
				this._unresolvedStreamsheets.Add(new UnresolvedStreamsheet(this._currentSheet, this._bookmarkLinks));
				this._bookmarkLinks = new List<LinkInfo>();
			}
			this._picturesStartingOnCurrentRow.Clear();
			this._picturesToUpdateByEndColumn.Clear();
			this._picturesToUpdateByEndRow.Clear();
			this._picturesToUpdateByStartColumn.Clear();
		}

		private void FinalizeCell()
		{
			if (this._currentRow != null && !this._currentCellHasBeenModified && (this._currentStyle == null || !this._currentStyle.HasBeenModified))
			{
				this._currentRow.ClearCell(this._currentColumn);
			}
			else
			{
				bool flag = false;
				if (this._currentRichText != null)
				{
					this._currentRichText.Commit(this._currentStyle);
					flag = this._currentRichText.FoundRotatedEastAsianChar;
					this._currentRichText = null;
				}
				else if (this._currentCell != null && this._currentCell.ValueType == Cell.CellValueType.Text && this._currentCell.Value != null)
				{
					string text = this._currentCell.Value.ToString();
					if (text.Length > 0)
					{
						StringBuilder stringBuilder = new StringBuilder(text.Length);
						ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(text, stringBuilder, this._checkForRotatedEastAsianChars, out flag);
						this._currentCell.Value = stringBuilder.ToString();
					}
				}
				if (flag)
				{
					this._currentStyle.Orientation = Orientation.Vertical;
				}
				if (this._currentCell != null)
				{
					if (this._currentStyle != null && (this._currentCell.ValueType == Cell.CellValueType.Text || this._currentCell.ValueType == Cell.CellValueType.Blank || this._currentCell.ValueType == Cell.CellValueType.Error))
					{
						this._currentStyle.NumberFormat = null;
					}
					this._currentCell.Style = this._currentStyle;
				}
				this._currentStyle = null;
			}
			this._checkForRotatedEastAsianChars = false;
		}

		private void LimitPercentage(ref double percentage)
		{
			if (percentage < 0.0)
			{
				percentage = 0.0;
			}
			if (percentage > 100.0)
			{
				percentage = 100.0;
			}
		}

		private double ValidMargin(double margin)
		{
			if (margin < 0.0)
			{
				return 0.0;
			}
			if (margin > 49.0)
			{
				return 49.0;
			}
			return margin;
		}

		private Style GetStyle()
		{
			return this._workbook.CreateStyle();
		}

		private void AddPictureToUpdateCollection(ref Dictionary<int, List<Picture>> collection, int key, Picture value)
		{
			List<Picture> list = default(List<Picture>);
			if (collection.TryGetValue(key, out list))
			{
				if (list == null)
				{
					collection[key] = new List<Picture>();
					collection[key].Add(value);
				}
				else
				{
					list.Add(value);
				}
			}
			else
			{
				collection[key] = new List<Picture>();
				collection[key].Add(value);
			}
		}

		private string GetCellBookmarkLink(BookmarkTargetInfo bookmark)
		{
			return string.Format(CultureInfo.InvariantCulture, "'{0}'!{1}", bookmark.Sheet, bookmark.Cell);
		}

		private string GetPictureBookmarkLink(BookmarkTargetInfo bookmark)
		{
			return string.Format(CultureInfo.InvariantCulture, "#{0}!{1}", bookmark.Sheet, bookmark.Cell);
		}

		public void NextWorksheet()
		{
			this.FinalizeWorksheet();
			this.CreateNewSheet();
		}

		public void SetCurrentSheetName(string name)
		{
			this._currentSheet.Name = name;
		}

		public void AdjustFirstWorksheetName(string reportName, bool addedDocMap)
		{
			if (!string.IsNullOrEmpty(reportName))
			{
				Streamsheet streamsheet = null;
				if (addedDocMap)
				{
					if (this._workbook.Worksheets.Count >= 2)
					{
						streamsheet = this._workbook[1];
					}
				}
				else if (this._workbook.Worksheets.Count >= 1)
				{
					streamsheet = this._workbook[0];
				}
				if (streamsheet != null)
				{
					streamsheet.Name = ExcelGeneratorStringUtil.SanitizeSheetName(reportName).ToString();
				}
			}
		}

		public void GenerateWorksheetName(string pageName)
		{
			if (!string.IsNullOrEmpty(pageName))
			{
				StringBuilder stringBuilder = ExcelGeneratorStringUtil.SanitizeSheetName(pageName);
				if (stringBuilder.Length != 0)
				{
					pageName = stringBuilder.ToString();
					int num = 0;
					while (true)
					{
						if (!this._worksheetNames.TryGetValue(pageName.ToUpperInvariant(), out num) && !this.CheckForSheetNameConflict(pageName))
						{
							break;
						}
						num++;
						this._worksheetNames[pageName.ToUpperInvariant()] = num;
						string text = string.Format(CultureInfo.InvariantCulture, ExcelRenderRes.SheetNameCounterSuffix, num);
						if (pageName.Length + text.Length > 31)
						{
							pageName = pageName.Remove(31 - text.Length);
						}
						pageName += text;
					}
					this._currentSheet.Name = pageName;
					this._worksheetNames.Add(pageName.ToUpperInvariant(), 1);
				}
			}
		}

		public void SetSummaryRowAfter(bool after)
		{
			this._currentSheet.PageSetup.SummaryRowsBelow = after;
		}

		public void SetSummaryColumnToRight(bool after)
		{
			this._currentSheet.PageSetup.SummaryColumnsRight = after;
		}

		public void SetColumnProperties(int columnIndex, double widthInPoints, byte columnOutlineLevel, bool collapsed)
		{
			if (columnIndex > this.MaxColumns)
			{
				throw new ReportRenderingException(ExcelRenderRes.MaxColExceededInSheet(columnIndex.ToString(CultureInfo.InvariantCulture), this.MaxColumns.ToString(CultureInfo.InvariantCulture)));
			}
			ColumnProperties columnProperties = this._currentSheet.GetColumnProperties(columnIndex);
			columnProperties.Width = widthInPoints;
			if (columnOutlineLevel > 7)
			{
				columnOutlineLevel = 7;
			}
			columnProperties.OutlineLevel = columnOutlineLevel;
			columnProperties.OutlineCollapsed = collapsed;
			columnProperties.Hidden = collapsed;
			List<Picture> list = default(List<Picture>);
			if (this._picturesToUpdateByStartColumn.TryGetValue(columnIndex, out list))
			{
				foreach (Picture item in list)
				{
					item.UpdateColumnOffset(widthInPoints, true);
				}
			}
			if (this._picturesToUpdateByEndColumn.TryGetValue(columnIndex, out list))
			{
				foreach (Picture item2 in list)
				{
					item2.UpdateColumnOffset(widthInPoints, false);
				}
			}
		}

		public void AddRow(int rowIndex)
		{
			if (rowIndex > this.MaxRows)
			{
				throw new ReportRenderingException(ExcelRenderRes.MaxRowExceededInSheet(rowIndex.ToString(CultureInfo.InvariantCulture), this.MaxRows.ToString(CultureInfo.InvariantCulture)));
			}
			this._maxRowIndex = Math.Max(this._maxRowIndex, rowIndex);
		}

		public void SetRowProperties(int rowIndex, int heightIn20thPoints, byte rowOutlineLevel, bool collapsed, bool autoSize)
		{
			if (rowIndex > this.MaxRows)
			{
				throw new ReportRenderingException(ExcelRenderRes.MaxRowExceededInSheet(rowIndex.ToString(CultureInfo.InvariantCulture), this.MaxRows.ToString(CultureInfo.InvariantCulture)));
			}
			double num = (double)heightIn20thPoints / 20.0;
			this._currentRow.Height = num;
			this._currentRow.CustomHeight = !autoSize;
			if (rowOutlineLevel > 7)
			{
				rowOutlineLevel = 7;
			}
			this._currentRow.OutlineLevel = rowOutlineLevel;
			this._currentRow.OutlineCollapsed = collapsed;
			this._currentRow.Hidden = collapsed;
			foreach (Picture item in this._picturesStartingOnCurrentRow)
			{
				item.UpdateRowOffset(num, true);
			}
			this._picturesStartingOnCurrentRow.Clear();
			List<Picture> list = default(List<Picture>);
			if (this._picturesToUpdateByEndRow.TryGetValue(rowIndex, out list))
			{
				foreach (Picture item2 in list)
				{
					item2.UpdateRowOffset(num, false);
				}
			}
		}

		public void SetRowContext(int rowIndex)
		{
			this.FinalizeCell();
			if (rowIndex > this.MaxRows)
			{
				throw new ReportRenderingException(ExcelRenderRes.MaxRowExceededInSheet(rowIndex.ToString(CultureInfo.InvariantCulture), this.MaxRows.ToString(CultureInfo.InvariantCulture)));
			}
			this._currentRow = this._currentSheet.CreateRow(rowIndex);
		}

		public void SetColumnExtents(int min, int max)
		{
		}

		public void SetColumnContext(int columnIndex)
		{
			this.FinalizeCell();
			this._currentCell = this._currentRow[columnIndex];
			this._currentColumn = columnIndex;
			this._currentCellHasBeenModified = false;
		}

		public void SetCellValue(object value, TypeCode type)
		{
			string text = value as string;
			if (text != null && text.Length > 32767)
			{
				throw new ReportRenderingException(ExcelRenderRes.MaxStringLengthExceeded(this._currentRow.RowNumber.ToString(CultureInfo.InvariantCulture), this._currentColumn.ToString(CultureInfo.InvariantCulture)));
			}
			this._currentCell.Value = value;
			this._currentCellHasBeenModified = true;
		}

		public void SetCellError(ExcelErrorCode errorCode)
		{
			this.SetCellValue(errorCode, TypeCode.Empty);
			this._currentCellHasBeenModified = true;
		}

		public void SetModifiedRotationForEastAsianChars(bool value)
		{
			this._checkForRotatedEastAsianChars = value;
		}

		public IRichTextInfo GetCellRichTextInfo()
		{
			this._currentRichText = new RichTextInfo(this._currentCell, this.GetStyle, this._currentRow.RowNumber, this._currentColumn);
			this.GetCellStyle().WrapText = true;
			this._currentCellHasBeenModified = true;
			return this._currentRichText;
		}

		public IStyle GetCellStyle()
		{
			if (this._cachedStyle != null)
			{
				return this._cachedStyle;
			}
			if (this._currentStyle == null)
			{
				this._currentStyle = this.GetStyle();
			}
			return this._currentStyle;
		}

		public TypeCode GetCellValueType()
		{
			Cell.CellValueType valueType = this._currentCell.ValueType;
			if (valueType == Cell.CellValueType.Boolean)
			{
				return TypeCode.Boolean;
			}
			if (valueType == Cell.CellValueType.Date)
			{
				return TypeCode.DateTime;
			}
			if (valueType == Cell.CellValueType.Error)
			{
				return TypeCode.String;
			}
			if (valueType == Cell.CellValueType.Double)
			{
				return TypeCode.Double;
			}
			if (valueType == Cell.CellValueType.Integer)
			{
				return TypeCode.Int32;
			}
			return TypeCode.String;
		}

		public void AddImage(string imageName, Stream imageData, ImageFormat format, int rowStart, double rowStartPercentage, int columnStart, double columnStartPercentage, int rowEnd, double rowEndPercentage, int columnEnd, double colEndPercentage, string hyperlinkURL, bool isBookmarkLink)
		{
			if (imageData.CanSeek)
			{
				imageData.Seek(0L, SeekOrigin.Begin);
			}
			if (rowStart < 0)
			{
				rowStart = 0;
			}
			if (columnStart < 0)
			{
				columnStart = 0;
			}
			if (rowEnd < 0)
			{
				rowEnd = 0;
			}
			if (columnEnd < 0)
			{
				columnEnd = 0;
			}
			this.LimitPercentage(ref rowStartPercentage);
			this.LimitPercentage(ref columnStartPercentage);
			this.LimitPercentage(ref rowEndPercentage);
			this.LimitPercentage(ref colEndPercentage);
			Anchor startPosition = this._currentSheet.CreateAnchor(rowStart, columnStart, columnStartPercentage, rowStartPercentage);
			Anchor endPosition = this._currentSheet.CreateAnchor(rowEnd, columnEnd, colEndPercentage, rowEndPercentage);
			string extension = (!(format.Guid == ImageFormat.Bmp.Guid)) ? ((!(format.Guid == ImageFormat.Gif.Guid)) ? ((!(format.Guid == ImageFormat.Jpeg.Guid)) ? "png" : "jpg") : "gif") : "bmp";
			OfficeImageHasher officeImageHasher = new OfficeImageHasher(imageData);
			string uniqueId = Convert.ToBase64String(officeImageHasher.Hash);
			imageData.Seek(0L, SeekOrigin.Begin);
			Picture picture = this._currentSheet.Pictures.CreatePicture(uniqueId, extension, imageData, startPosition, endPosition);
			this._picturesStartingOnCurrentRow.Add(picture);
			this.AddPictureToUpdateCollection(ref this._picturesToUpdateByEndRow, rowEnd, picture);
			this.AddPictureToUpdateCollection(ref this._picturesToUpdateByStartColumn, columnStart, picture);
			this.AddPictureToUpdateCollection(ref this._picturesToUpdateByEndColumn, columnEnd, picture);
			if (!string.IsNullOrEmpty(hyperlinkURL))
			{
				if (isBookmarkLink)
				{
					BookmarkTargetInfo bookmark = default(BookmarkTargetInfo);
					if (this._bookmarkTargets.TryGetValue(hyperlinkURL, out bookmark))
					{
						picture.Hyperlink = this.GetPictureBookmarkLink(bookmark);
					}
					else
					{
						this._unresolvedPictureBookmarks.Add(new PictureLinkInfo(picture, hyperlinkURL));
					}
				}
				else
				{
					picture.Hyperlink = hyperlinkURL;
				}
			}
		}

		public void AddMergeCell(int rowStart, int columnStart, int rowStop, int columnStop)
		{
			this._mergedCells.Add(new MergeInfo(rowStart, rowStop, columnStart, columnStop));
		}

		public void AddHyperlink(string label, string reportUrl)
		{
			this._hyperlinks.Add(new LinkInfo(this._currentCell.Name, reportUrl, label));
		}

		public void AddHeader(string left, string center, string right)
		{
			int num = 255;
			this._currentSheet.PageSetup.LeftHeader = this.GetUsablePortionOfHeaderFooterString(left, ref num);
			this._currentSheet.PageSetup.CenterHeader = this.GetUsablePortionOfHeaderFooterString(center, ref num);
			this._currentSheet.PageSetup.RightHeader = this.GetUsablePortionOfHeaderFooterString(right, ref num);
		}

		public void AddFooter(string left, string center, string right)
		{
			int num = 255;
			this._currentSheet.PageSetup.LeftFooter = this.GetUsablePortionOfHeaderFooterString(left, ref num);
			this._currentSheet.PageSetup.CenterFooter = this.GetUsablePortionOfHeaderFooterString(center, ref num);
			this._currentSheet.PageSetup.RightFooter = this.GetUsablePortionOfHeaderFooterString(right, ref num);
		}

		public void AddFreezePane(int aRow, int aColumn)
		{
			this._currentSheet.SetFreezePanes(aRow, aColumn);
		}

		public void AddBookmarkTarget(string value)
		{
			BookmarkTargetInfo bookmarkTargetInfo = new BookmarkTargetInfo(this._currentSheet.Name, this._currentCell.Name);
			string cellBookmarkLink = this.GetCellBookmarkLink(bookmarkTargetInfo);
			for (int num = this._unresolvedStreamsheets.Count - 1; num >= 0; num--)
			{
				if (this._unresolvedStreamsheets[num].ResolveTarget(value, cellBookmarkLink))
				{
					this._unresolvedStreamsheets.RemoveAt(num);
				}
			}
			string pictureBookmarkLink = this.GetPictureBookmarkLink(bookmarkTargetInfo);
			for (int num2 = this._unresolvedPictureBookmarks.Count - 1; num2 >= 0; num2--)
			{
				if (this._unresolvedPictureBookmarks[num2].Target == value)
				{
					this._unresolvedPictureBookmarks[num2].Picture.Hyperlink = pictureBookmarkLink;
					this._unresolvedPictureBookmarks.RemoveAt(num2);
				}
			}
			this._bookmarkTargets[value] = bookmarkTargetInfo;
		}

		public void AddBookmarkLink(string label, string link)
		{
			this._bookmarkLinks.Add(new LinkInfo(this._currentCell.Name, link, label));
		}

		public void AddBackgroundImage(byte[] data, string imageName, ref Stream backgroundImage, ref ushort backgroundImageWidth, ref ushort backgroundImageHeight)
		{
			using (MemoryStream pictureStream = new MemoryStream(data))
			{
				this._currentSheet.SetBackgroundPicture(imageName, "png", pictureStream);
			}
		}

		public void SetPageContraints(int paperSize, bool isPortrait, double headerMargin, double footerMargin)
		{
			PageSetup pageSetup = this._currentSheet.PageSetup;
			pageSetup.HeaderMargin = this.ValidMargin(headerMargin);
			pageSetup.FooterMargin = this.ValidMargin(footerMargin);
			pageSetup.Orientation = (isPortrait ? PageSetup.PageOrientation.Portrait : PageSetup.PageOrientation.Landscape);
			pageSetup.PaperSize = PageSetup.PagePaperSize.findByValue(paperSize);
		}

		public void SetMargins(double topMargin, double bottomMargin, double leftMargin, double rightMargin)
		{
			PageSetup pageSetup = this._currentSheet.PageSetup;
			pageSetup.TopMargin = this.ValidMargin(topMargin);
			pageSetup.BottomMargin = this.ValidMargin(bottomMargin);
			pageSetup.LeftMargin = this.ValidMargin(leftMargin);
			pageSetup.RightMargin = this.ValidMargin(rightMargin);
		}

		public IColor AddColor(string colorString)
		{
			System.Drawing.Color color = LayoutConvert.ToColor(colorString);
			if (this._nextPaletteIndex < 56)
			{
				int colorIndex = this._workbook.Palette.GetColorIndex(color.R, color.G, color.B);
				if (colorIndex == -1)
				{
					this._workbook.Palette.SetColorAt(this._nextPaletteIndex, color.R, color.G, color.B);
					this._nextPaletteIndex++;
				}
				else if (colorIndex >= this._nextPaletteIndex)
				{
					if (colorIndex == this._nextPaletteIndex)
					{
						this._nextPaletteIndex++;
					}
					else
					{
						Color colorAt = this._workbook.Palette.GetColorAt(colorIndex);
						Color colorAt2 = this._workbook.Palette.GetColorAt(this._nextPaletteIndex);
						this._workbook.Palette.SetColorAt(colorIndex, colorAt2.Red, colorAt2.Green, colorAt2.Blue);
						this._workbook.Palette.SetColorAt(this._nextPaletteIndex, colorAt.Red, colorAt.Green, colorAt.Blue);
						this._nextPaletteIndex++;
					}
				}
			}
			return this._workbook.Palette.GetColor(color);
		}

		public void SaveSpreadsheet(Stream outputStream, Stream backgroundImage, ushort backgroundImageWidth, ushort backgroundImageHeight)
		{
			this.FinalizeWorksheet();
			this._application.Save(this._workbook);
			byte[] buffer = new byte[1024];
			long num = this._workbookStream.Length;
			this._workbookStream.Seek(0L, SeekOrigin.Begin);
			while (num > 0)
			{
				int num2 = this._workbookStream.Read(buffer, 0, 1024);
				num -= num2;
				outputStream.Write(buffer, 0, num2);
			}
		}

		public Stream CreateStream(string name)
		{
			return this._createTempStream(name);
		}

		public bool UseCachedStyle(string id)
		{
			Style currentStyle = default(Style);
			if (this._cachedstyles.TryGetValue(id, out currentStyle))
			{
				this._currentStyle = currentStyle;
				return true;
			}
			return false;
		}

		public void DefineCachedStyle(string id)
		{
			this._cachedStyle = this._workbook.CreateStyle();
			this._cachedstyles.Add(id, this._cachedStyle);
		}

		public void EndCachedStyle()
		{
			this._cachedStyle = null;
		}

		public void AddPrintTitle(int rowStart, int rowEnd)
		{
			if (rowStart >= 0 && rowStart <= this.MaxRows)
			{
				if (rowEnd >= 0 && rowEnd <= this.MaxRows)
				{
					this._currentSheet.PageSetup.SetPrintTitleToRows(rowStart, rowEnd);
					return;
				}
				throw new ReportRenderingException(ExcelRenderRes.ValueOutOfRange(0.ToString(CultureInfo.InvariantCulture), this.MaxRows.ToString(CultureInfo.InvariantCulture), rowEnd.ToString(CultureInfo.InvariantCulture)));
			}
			throw new ReportRenderingException(ExcelRenderRes.ValueOutOfRange(0.ToString(CultureInfo.InvariantCulture), this.MaxRows.ToString(CultureInfo.InvariantCulture), rowStart.ToString(CultureInfo.InvariantCulture)));
		}

		public void BuildHeaderFooterString(StringBuilder str, RPLTextBoxProps textBox, ref string lastFont, ref double lastFontSize)
		{
			RPLElementStyle style = textBox.Style;
			string text = (string)style[20];
			str.Append("&").Append("\"");
			if (!string.IsNullOrEmpty(text) && !text.Equals(lastFont))
			{
				FormulaHandler.EncodeHeaderFooterString(str, text);
				lastFont = text;
			}
			else
			{
				str.Append("-");
			}
			str.Append(",");
			bool flag = false;
			bool flag2 = false;
			object obj = textBox.Style[22];
			if (obj != null && LayoutConvert.ToFontWeight((RPLFormat.FontWeights)obj) >= 600)
			{
				str.Append("Bold");
				flag = true;
			}
			object obj2 = style[19];
			if (obj2 != null && (RPLFormat.FontStyles)obj2 == RPLFormat.FontStyles.Italic)
			{
				if (flag)
				{
					str.Append(" ");
				}
				str.Append("Italic");
				flag2 = true;
			}
			if (!flag && !flag2)
			{
				str.Append("Regular");
			}
			str.Append("\"");
			string text2 = (string)style[21];
			if (!string.IsNullOrEmpty(text2))
			{
				double num = LayoutConvert.ToPoints(text2);
				if (num != 0.0 && num != lastFontSize)
				{
					str.Append("&").Append((int)num);
				}
				lastFontSize = num;
			}
			object obj3 = textBox.Style[24];
			if (obj3 != null)
			{
				RPLFormat.TextDecorations textDecorations = (RPLFormat.TextDecorations)obj3;
				if (textDecorations == RPLFormat.TextDecorations.Underline)
				{
					str.Append("&u");
				}
				if (textDecorations == RPLFormat.TextDecorations.LineThrough)
				{
					str.Append("&s");
				}
			}
			string text3 = string.Empty;
			string text4 = ((RPLTextBoxPropsDef)textBox.Definition).Formula;
			if (text4 != null && text4.Length != 0)
			{
				if (text4.StartsWith("=", StringComparison.Ordinal))
				{
					text4 = text4.Remove(0, 1);
				}
				text3 = FormulaHandler.ProcessHeaderFooterFormula(text4);
			}
			if (text3 != null && text3.Length != 0)
			{
				str.Append(text3);
			}
			else
			{
				string text5 = textBox.Value;
				if (text5 == null)
				{
					text5 = ((RPLTextBoxPropsDef)textBox.Definition).Value;
				}
				if (text5 == null && textBox.OriginalValue != null)
				{
					text5 = textBox.OriginalValue.ToString();
				}
				if (text5 != null)
				{
					FormulaHandler.EncodeHeaderFooterString(str, text5.Trim());
				}
			}
			str.Append(" ");
		}
	}
}
