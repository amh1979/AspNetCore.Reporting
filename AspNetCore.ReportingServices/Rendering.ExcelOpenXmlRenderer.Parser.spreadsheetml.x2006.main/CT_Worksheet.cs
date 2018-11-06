using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Worksheet : OoxmlComplexType
	{
		private CT_SheetPr _sheetPr;

		private CT_SheetViews _sheetViews;

		private CT_SheetFormatPr _sheetFormatPr;

		private CT_SheetData _sheetData;

		private CT_MergeCells _mergeCells;

		private CT_Hyperlinks _hyperlinks;

		private CT_PageMargins _pageMargins;

		private CT_PageSetup _pageSetup;

		private CT_HeaderFooter _headerFooter;

		private CT_Drawing _drawing;

		private CT_SheetBackgroundPicture _picture;

		private List<CT_Cols> _cols;

		public CT_SheetPr SheetPr
		{
			get
			{
				return this._sheetPr;
			}
			set
			{
				this._sheetPr = value;
			}
		}

		public CT_SheetViews SheetViews
		{
			get
			{
				return this._sheetViews;
			}
			set
			{
				this._sheetViews = value;
			}
		}

		public CT_SheetFormatPr SheetFormatPr
		{
			get
			{
				return this._sheetFormatPr;
			}
			set
			{
				this._sheetFormatPr = value;
			}
		}

		public CT_SheetData SheetData
		{
			get
			{
				return this._sheetData;
			}
			set
			{
				this._sheetData = value;
			}
		}

		public CT_MergeCells MergeCells
		{
			get
			{
				return this._mergeCells;
			}
			set
			{
				this._mergeCells = value;
			}
		}

		public CT_Hyperlinks Hyperlinks
		{
			get
			{
				return this._hyperlinks;
			}
			set
			{
				this._hyperlinks = value;
			}
		}

		public CT_PageMargins PageMargins
		{
			get
			{
				return this._pageMargins;
			}
			set
			{
				this._pageMargins = value;
			}
		}

		public CT_PageSetup PageSetup
		{
			get
			{
				return this._pageSetup;
			}
			set
			{
				this._pageSetup = value;
			}
		}

		public CT_HeaderFooter HeaderFooter
		{
			get
			{
				return this._headerFooter;
			}
			set
			{
				this._headerFooter = value;
			}
		}

		public CT_Drawing Drawing
		{
			get
			{
				return this._drawing;
			}
			set
			{
				this._drawing = value;
			}
		}

		public CT_SheetBackgroundPicture Picture
		{
			get
			{
				return this._picture;
			}
			set
			{
				this._picture = value;
			}
		}

		public List<CT_Cols> Cols
		{
			get
			{
				return this._cols;
			}
			set
			{
				this._cols = value;
			}
		}

		public static string SheetPrElementName
		{
			get
			{
				return "sheetPr";
			}
		}

		public static string SheetViewsElementName
		{
			get
			{
				return "sheetViews";
			}
		}

		public static string SheetFormatPrElementName
		{
			get
			{
				return "sheetFormatPr";
			}
		}

		public static string SheetDataElementName
		{
			get
			{
				return "sheetData";
			}
		}

		public static string MergeCellsElementName
		{
			get
			{
				return "mergeCells";
			}
		}

		public static string HyperlinksElementName
		{
			get
			{
				return "hyperlinks";
			}
		}

		public static string PageMarginsElementName
		{
			get
			{
				return "pageMargins";
			}
		}

		public static string PageSetupElementName
		{
			get
			{
				return "pageSetup";
			}
		}

		public static string HeaderFooterElementName
		{
			get
			{
				return "headerFooter";
			}
		}

		public static string DrawingElementName
		{
			get
			{
				return "drawing";
			}
		}

		public static string PictureElementName
		{
			get
			{
				return "picture";
			}
		}

		public static string ColsElementName
		{
			get
			{
				return "cols";
			}
		}

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
			this._sheetData = new CT_SheetData();
		}

		protected override void InitCollections()
		{
			this._cols = new List<CT_Cols>();
		}

		public override void WriteAsRoot(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			this.WriteOpenTag(s, tagName, depth, namespaces, true);
			this.WriteElements(s, depth, namespaces);
			this.WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void Write(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			this.WriteOpenTag(s, tagName, depth, namespaces, false);
			this.WriteElements(s, depth, namespaces);
			this.WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void WriteOpenTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces, bool root)
		{
			s.Write("<");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
			s.Write(tagName);
			this.WriteAttributes(s);
			if (root)
			{
				foreach (string key in namespaces.Keys)
				{
					s.Write(" xmlns");
					if (namespaces[key] != "")
					{
						s.Write(":");
						s.Write(namespaces[key]);
					}
					s.Write("=\"");
					s.Write(key);
					s.Write("\"");
				}
			}
			s.Write(">");
		}

		public override void WriteCloseTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			s.Write("</");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			this.Write_sheetPr(s, depth, namespaces);
			this.Write_sheetViews(s, depth, namespaces);
			this.Write_sheetFormatPr(s, depth, namespaces);
			this.Write_cols(s, depth, namespaces);
			this.Write_sheetData(s, depth, namespaces);
			this.Write_mergeCells(s, depth, namespaces);
			this.Write_hyperlinks(s, depth, namespaces);
			this.Write_pageMargins(s, depth, namespaces);
			this.Write_pageSetup(s, depth, namespaces);
			this.Write_headerFooter(s, depth, namespaces);
			this.Write_drawing(s, depth, namespaces);
			this.Write_picture(s, depth, namespaces);
		}

		public void Write_sheetPr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._sheetPr != null)
			{
				this._sheetPr.Write(s, "sheetPr", depth + 1, namespaces);
			}
		}

		public void Write_sheetViews(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._sheetViews != null)
			{
				this._sheetViews.Write(s, "sheetViews", depth + 1, namespaces);
			}
		}

		public void Write_sheetFormatPr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._sheetFormatPr != null)
			{
				this._sheetFormatPr.Write(s, "sheetFormatPr", depth + 1, namespaces);
			}
		}

		public void Write_sheetData(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._sheetData != null)
			{
				this._sheetData.Write(s, "sheetData", depth + 1, namespaces);
			}
		}

		public void Write_mergeCells(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._mergeCells != null)
			{
				this._mergeCells.Write(s, "mergeCells", depth + 1, namespaces);
			}
		}

		public void Write_hyperlinks(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._hyperlinks != null)
			{
				this._hyperlinks.Write(s, "hyperlinks", depth + 1, namespaces);
			}
		}

		public void Write_pageMargins(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._pageMargins != null)
			{
				this._pageMargins.Write(s, "pageMargins", depth + 1, namespaces);
			}
		}

		public void Write_pageSetup(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._pageSetup != null)
			{
				this._pageSetup.Write(s, "pageSetup", depth + 1, namespaces);
			}
		}

		public void Write_headerFooter(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._headerFooter != null)
			{
				this._headerFooter.Write(s, "headerFooter", depth + 1, namespaces);
			}
		}

		public void Write_drawing(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._drawing != null)
			{
				this._drawing.Write(s, "drawing", depth + 1, namespaces);
			}
		}

		public void Write_picture(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._picture != null)
			{
				this._picture.Write(s, "picture", depth + 1, namespaces);
			}
		}

		public void Write_cols(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._cols != null)
			{
				foreach (CT_Cols col in this._cols)
				{
					if (col != null)
					{
						col.Write(s, "cols", depth + 1, namespaces);
					}
				}
			}
		}
	}
}
