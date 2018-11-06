using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Workbook : OoxmlComplexType
	{
		private CT_FileVersion _fileVersion;

		private CT_WorkbookPr _workbookPr;

		private CT_BookViews _bookViews;

		private CT_Sheets _sheets;

		private CT_DefinedNames _definedNames;

		private CT_CalcPr _calcPr;

		public CT_FileVersion FileVersion
		{
			get
			{
				return this._fileVersion;
			}
			set
			{
				this._fileVersion = value;
			}
		}

		public CT_WorkbookPr WorkbookPr
		{
			get
			{
				return this._workbookPr;
			}
			set
			{
				this._workbookPr = value;
			}
		}

		public CT_BookViews BookViews
		{
			get
			{
				return this._bookViews;
			}
			set
			{
				this._bookViews = value;
			}
		}

		public CT_Sheets Sheets
		{
			get
			{
				return this._sheets;
			}
			set
			{
				this._sheets = value;
			}
		}

		public CT_DefinedNames DefinedNames
		{
			get
			{
				return this._definedNames;
			}
			set
			{
				this._definedNames = value;
			}
		}

		public CT_CalcPr CalcPr
		{
			get
			{
				return this._calcPr;
			}
			set
			{
				this._calcPr = value;
			}
		}

		public static string FileVersionElementName
		{
			get
			{
				return "fileVersion";
			}
		}

		public static string WorkbookPrElementName
		{
			get
			{
				return "workbookPr";
			}
		}

		public static string BookViewsElementName
		{
			get
			{
				return "bookViews";
			}
		}

		public static string SheetsElementName
		{
			get
			{
				return "sheets";
			}
		}

		public static string DefinedNamesElementName
		{
			get
			{
				return "definedNames";
			}
		}

		public static string CalcPrElementName
		{
			get
			{
				return "calcPr";
			}
		}

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
			this._sheets = new CT_Sheets();
		}

		protected override void InitCollections()
		{
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
			this.Write_fileVersion(s, depth, namespaces);
			this.Write_workbookPr(s, depth, namespaces);
			this.Write_bookViews(s, depth, namespaces);
			this.Write_sheets(s, depth, namespaces);
			this.Write_definedNames(s, depth, namespaces);
			this.Write_calcPr(s, depth, namespaces);
		}

		public void Write_fileVersion(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._fileVersion != null)
			{
				this._fileVersion.Write(s, "fileVersion", depth + 1, namespaces);
			}
		}

		public void Write_workbookPr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._workbookPr != null)
			{
				this._workbookPr.Write(s, "workbookPr", depth + 1, namespaces);
			}
		}

		public void Write_bookViews(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._bookViews != null)
			{
				this._bookViews.Write(s, "bookViews", depth + 1, namespaces);
			}
		}

		public void Write_sheets(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._sheets != null)
			{
				this._sheets.Write(s, "sheets", depth + 1, namespaces);
			}
		}

		public void Write_definedNames(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._definedNames != null)
			{
				this._definedNames.Write(s, "definedNames", depth + 1, namespaces);
			}
		}

		public void Write_calcPr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._calcPr != null)
			{
				this._calcPr.Write(s, "calcPr", depth + 1, namespaces);
			}
		}
	}
}
