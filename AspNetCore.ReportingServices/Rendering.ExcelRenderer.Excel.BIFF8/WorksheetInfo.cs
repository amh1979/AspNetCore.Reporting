using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8.Records;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class WorksheetInfo
	{
		private sealed class ImageCache
		{
			private uint m_shapeID;

			private string m_name;

			private Escher.ClientAnchor.SPRC m_clientAnchor;

			private uint m_refIndex;

			private string m_hyperlinkURL;

			private bool m_isBookmark;

			internal uint ShapeID
			{
				get
				{
					return this.m_shapeID;
				}
				set
				{
					this.m_shapeID = value;
				}
			}

			internal string Name
			{
				get
				{
					return this.m_name;
				}
				set
				{
					this.m_name = value;
				}
			}

			internal Escher.ClientAnchor.SPRC ClientAnchor
			{
				get
				{
					return this.m_clientAnchor;
				}
				set
				{
					this.m_clientAnchor = value;
				}
			}

			internal uint RefIndex
			{
				get
				{
					return this.m_refIndex;
				}
				set
				{
					this.m_refIndex = value;
				}
			}

			internal string HyperlinkURL
			{
				get
				{
					return this.m_hyperlinkURL;
				}
				set
				{
					this.m_hyperlinkURL = value;
				}
			}

			internal bool IsBookmark
			{
				get
				{
					return this.m_isBookmark;
				}
				set
				{
					this.m_isBookmark = value;
				}
			}

			internal ImageCache(uint shapeID, string name, Escher.ClientAnchor.SPRC clientAnchor, uint refIndex)
			{
				this.ShapeID = shapeID;
				this.Name = name;
				this.ClientAnchor = clientAnchor;
				this.RefIndex = refIndex;
				this.HyperlinkURL = null;
				this.IsBookmark = false;
			}

			internal ImageCache(uint shapeID, string name, Escher.ClientAnchor.SPRC clientAnchor, uint refIndex, string linkURL, bool isBookmark)
				: this(shapeID, name, clientAnchor, refIndex)
			{
				this.HyperlinkURL = linkURL;
				this.IsBookmark = isBookmark;
			}
		}

		internal sealed class ColumnInfo
		{
			private double m_width;

			private byte m_outline;

			private bool m_collapsed;

			public double Width
			{
				get
				{
					return this.m_width;
				}
				set
				{
					this.m_width = value;
				}
			}

			public bool Collapsed
			{
				get
				{
					return this.m_collapsed;
				}
				set
				{
					this.m_collapsed = value;
				}
			}

			public byte OutlineLevel
			{
				get
				{
					return this.m_outline;
				}
				set
				{
					this.m_outline = value;
				}
			}

			public ColumnInfo(double width)
			{
				this.m_width = width;
			}
		}

		private ushort m_rowFirst = 65535;

		private ushort m_rowLast;

		private ushort m_colFirst = 65535;

		private ushort m_colLast;

		private ColumnInfo[] m_columns = new ColumnInfo[256];

		private Stream m_cellData;

		private string m_sheetName;

		private long m_BOFStartOffset;

		private List<uint> m_DBCellOffsets;

		private List<ushort> m_sizeOfCellData;

		private List<AreaInfo> m_mergeCellAreas;

		private List<HyperlinkInfo> m_hyperlinks;

		private Escher.DrawingContainer m_drawingContainer;

		private List<ImageCache> m_images;

		private uint m_currentShapeID;

		private string m_headerString;

		private string m_footerString;

		private ushort m_rowSplit;

		private ushort m_columnSplit;

		private int m_paperSize;

		private bool m_isPortrait = true;

		private double m_headerMargin;

		private double m_footerMargin;

		private double m_topMargin;

		private double m_bottomMargin;

		private double m_leftMargin;

		private double m_rightMargin;

		private bool m_summaryRowBelow = true;

		private bool m_summaryColumnToRight = true;

		private byte m_maxRowOutline;

		private byte m_maxColOutline;

		private PrintTitleInfo m_printTitle;

		private int m_sheetIndex = -1;

		internal ushort RowFirst
		{
			get
			{
				return this.m_rowFirst;
			}
			set
			{
				this.m_rowFirst = value;
			}
		}

		internal ushort RowLast
		{
			get
			{
				return this.m_rowLast;
			}
			set
			{
				this.m_rowLast = value;
			}
		}

		internal ushort ColFirst
		{
			get
			{
				return this.m_colFirst;
			}
			set
			{
				this.m_colFirst = value;
			}
		}

		internal ushort ColLast
		{
			get
			{
				return this.m_colLast;
			}
			set
			{
				this.m_colLast = value;
			}
		}

		internal Stream CellData
		{
			get
			{
				return this.m_cellData;
			}
			set
			{
				this.m_cellData = value;
			}
		}

		internal string SheetName
		{
			get
			{
				return this.m_sheetName;
			}
			set
			{
				this.m_sheetName = value;
			}
		}

		internal long BOFStartOffset
		{
			get
			{
				return this.m_BOFStartOffset;
			}
			set
			{
				this.m_BOFStartOffset = value;
			}
		}

		internal List<uint> DBCellOffsets
		{
			get
			{
				return this.m_DBCellOffsets;
			}
		}

		internal List<ushort> SizeOfCellData
		{
			get
			{
				return this.m_sizeOfCellData;
			}
		}

		internal ColumnInfo[] Columns
		{
			get
			{
				return this.m_columns;
			}
		}

		internal List<AreaInfo> MergeCellAreas
		{
			get
			{
				return this.m_mergeCellAreas;
			}
		}

		internal string HeaderString
		{
			get
			{
				return this.m_headerString;
			}
			set
			{
				this.m_headerString = value;
			}
		}

		internal string FooterString
		{
			get
			{
				return this.m_footerString;
			}
			set
			{
				this.m_footerString = value;
			}
		}

		internal bool SummaryRowAfter
		{
			get
			{
				return this.m_summaryRowBelow;
			}
			set
			{
				this.m_summaryRowBelow = value;
			}
		}

		internal bool SummaryColumnToRight
		{
			get
			{
				return this.m_summaryColumnToRight;
			}
			set
			{
				this.m_summaryColumnToRight = value;
			}
		}

		internal byte MaxRowOutline
		{
			get
			{
				return this.m_maxRowOutline;
			}
			set
			{
				this.m_maxRowOutline = Math.Min((byte)7, value);
			}
		}

		internal byte MaxColumnOutline
		{
			get
			{
				return this.m_maxColOutline;
			}
			set
			{
				this.m_maxColOutline = Math.Min((byte)7, value);
			}
		}

		internal PrintTitleInfo PrintTitle
		{
			get
			{
				return this.m_printTitle;
			}
			set
			{
				this.m_printTitle = value;
			}
		}

		internal int SheetIndex
		{
			get
			{
				return this.m_sheetIndex;
			}
			set
			{
				this.m_sheetIndex = value;
			}
		}

		internal WorksheetInfo(Stream cellDataStream, string name)
		{
			this.m_cellData = cellDataStream;
			this.m_sheetName = name;
			this.m_mergeCellAreas = new List<AreaInfo>();
			this.m_sizeOfCellData = new List<ushort>();
			this.m_DBCellOffsets = new List<uint>();
			this.m_currentShapeID = 0u;
			this.m_hyperlinks = new List<HyperlinkInfo>();
			this.m_images = new List<ImageCache>();
		}

		internal void ResolveCellReferences(Dictionary<string, string> lookup)
		{
			foreach (HyperlinkInfo hyperlink in this.m_hyperlinks)
			{
				if (hyperlink.IsBookmark && lookup.ContainsKey(hyperlink.URL))
				{
					hyperlink.URL = lookup[hyperlink.URL];
				}
			}
			foreach (ImageCache image in this.m_images)
			{
				if (image.HyperlinkURL != null && image.IsBookmark && lookup.ContainsKey(image.HyperlinkURL))
				{
					image.HyperlinkURL = lookup[image.HyperlinkURL];
				}
			}
		}

		internal void Write(BinaryWriter writer, bool isFirstPage, ExcelGeneratorConstants.CreateTempStream createTempStream, Stream backgroundImage, ushort backgroundImageWidth, ushort backgroundImageHeight)
		{
			this.BOFStartOffset = writer.BaseStream.Position;
			RecordFactory.BOF(writer, RecordFactory.BOFSubstreamType.Worksheet);
			RecordFactory.INDEX(writer, this.RowFirst, this.RowLast, this.DBCellOffsets);
			writer.BaseStream.Write(Constants.WORKSHEET1, 0, Constants.WORKSHEET1.Length);
			RecordFactory.GUTS(writer, this.m_maxRowOutline, this.m_maxColOutline);
			RecordFactory.WSBOOL(writer, this.m_summaryRowBelow, this.m_summaryColumnToRight);
			writer.BaseStream.Write(Constants.WORKSHEET2, 0, Constants.WORKSHEET2.Length);
			RecordFactory.SETUP(writer, (ushort)this.m_paperSize, this.m_isPortrait, this.m_headerMargin, this.m_footerMargin);
			RecordFactory.MARGINS(writer, this.m_topMargin, this.m_bottomMargin, this.m_leftMargin, this.m_rightMargin);
			if (backgroundImage != null)
			{
				RecordFactory.BACKGROUNDIMAGE(writer, backgroundImage, backgroundImageWidth, backgroundImageHeight);
			}
			if (this.HeaderString != null && this.HeaderString.Length > 0)
			{
				RecordFactory.HEADER(writer, this.HeaderString);
			}
			if (this.FooterString != null && this.FooterString.Length > 0)
			{
				RecordFactory.FOOTER(writer, this.FooterString);
			}
			for (int i = this.ColFirst; i <= this.ColLast; i++)
			{
				ColumnInfo columnInfo = this.Columns[i];
				if (columnInfo != null)
				{
					RecordFactory.COLINFO(writer, (ushort)i, columnInfo.Width, columnInfo.OutlineLevel, columnInfo.Collapsed);
				}
				else
				{
					RecordFactory.COLINFO(writer, (ushort)i, 0.0, 0, false);
				}
			}
			RecordFactory.DIMENSIONS(writer, this.RowFirst, this.RowLast, this.ColFirst, this.ColLast);
			byte[] array = new byte[4096];
			this.CellData.Seek(0L, SeekOrigin.Begin);
			int count;
			while ((count = this.CellData.Read(array, 0, array.Length)) > 0)
			{
				writer.BaseStream.Write(array, 0, count);
			}
			this.CellData.Close();
			this.CellData = null;
			if (this.m_drawingContainer != null)
			{
				foreach (ImageCache image in this.m_images)
				{
					if (image.HyperlinkURL == null)
					{
						this.m_drawingContainer.AddShape(image.ShapeID, image.Name, image.ClientAnchor, image.RefIndex);
					}
					else
					{
						this.m_drawingContainer.AddShape(image.ShapeID, image.Name, image.ClientAnchor, image.RefIndex, image.HyperlinkURL, (Escher.HyperlinkType)(image.IsBookmark ? 3 : 0));
					}
				}
				this.m_drawingContainer.WriteToStream(writer);
			}
			RecordFactory.WINDOW2(writer, this.m_rowSplit > 0 || this.m_columnSplit > 0, isFirstPage);
			if (this.m_rowSplit > 0 || this.m_columnSplit > 0)
			{
				RecordFactory.PANE(writer, this.m_columnSplit, this.m_rowSplit, this.m_rowSplit, this.m_columnSplit, 2);
			}
			writer.BaseStream.Write(Constants.WORKSHEET3, 0, Constants.WORKSHEET3.Length);
			RecordFactory.MERGECELLS(writer, this.MergeCellAreas);
			foreach (HyperlinkInfo hyperlink in this.m_hyperlinks)
			{
				RecordFactory.HLINK(writer, hyperlink);
			}
			writer.BaseStream.Write(Constants.WORKSHEET4, 0, Constants.WORKSHEET4.Length);
		}

		internal void AddImage(ushort drawingID, uint starterShapeID, string name, Escher.ClientAnchor.SPRC clientAnchor, uint referenceIndex, string hyperlinkURL, bool isBookmark)
		{
			if (this.m_drawingContainer == null)
			{
				this.m_drawingContainer = new Escher.DrawingContainer(drawingID);
				this.m_currentShapeID = starterShapeID;
			}
			if (hyperlinkURL != null)
			{
				this.m_images.Add(new ImageCache(this.m_currentShapeID, name, clientAnchor, referenceIndex, hyperlinkURL, isBookmark));
			}
			else
			{
				this.m_images.Add(new ImageCache(this.m_currentShapeID, name, clientAnchor, referenceIndex));
			}
			this.m_currentShapeID += 1u;
		}

		internal void AddFreezePane(int row, int column)
		{
			this.m_rowSplit = (ushort)row;
			this.m_columnSplit = (ushort)column;
		}

		internal void AddHyperlink(int row, int column, string url, string label)
		{
			this.m_hyperlinks.Add(new HyperlinkInfo(url, label, row, row, column, column));
		}

		internal void AddBookmark(int row, int column, string bookmark, string label)
		{
			this.m_hyperlinks.Add(new BookmarkInfo(bookmark, label, row, row, column, column));
		}

		internal void AddPrintTitle(int externSheetIndex, int rowStart, int rowEnd)
		{
			this.m_printTitle = new PrintTitleInfo((ushort)externSheetIndex, (ushort)(this.SheetIndex + 1), (ushort)rowStart, (ushort)rowEnd);
		}

		internal void SetPageContraints(int paperSize, bool isPortrait, double headerMargin, double footerMargin)
		{
			this.m_paperSize = paperSize;
			this.m_isPortrait = isPortrait;
			this.m_headerMargin = headerMargin;
			this.m_footerMargin = footerMargin;
		}

		internal void SetMargins(double topMargin, double bottomMargin, double leftMargin, double rightMargin)
		{
			this.m_topMargin = topMargin;
			this.m_bottomMargin = bottomMargin;
			this.m_leftMargin = leftMargin;
			this.m_rightMargin = rightMargin;
		}
	}
}
