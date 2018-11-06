using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Stylesheet : OoxmlComplexType
	{
		private CT_NumFmts _numFmts;

		private CT_Fonts _fonts;

		private CT_Fills _fills;

		private CT_Borders _borders;

		private CT_CellStyleXfs _cellStyleXfs;

		private CT_CellXfs _cellXfs;

		private CT_CellStyles _cellStyles;

		private CT_Dxfs _dxfs;

		private CT_TableStyles _tableStyles;

		private CT_Colors _colors;

		public CT_NumFmts NumFmts
		{
			get
			{
				return this._numFmts;
			}
			set
			{
				this._numFmts = value;
			}
		}

		public CT_Fonts Fonts
		{
			get
			{
				return this._fonts;
			}
			set
			{
				this._fonts = value;
			}
		}

		public CT_Fills Fills
		{
			get
			{
				return this._fills;
			}
			set
			{
				this._fills = value;
			}
		}

		public CT_Borders Borders
		{
			get
			{
				return this._borders;
			}
			set
			{
				this._borders = value;
			}
		}

		public CT_CellStyleXfs CellStyleXfs
		{
			get
			{
				return this._cellStyleXfs;
			}
			set
			{
				this._cellStyleXfs = value;
			}
		}

		public CT_CellXfs CellXfs
		{
			get
			{
				return this._cellXfs;
			}
			set
			{
				this._cellXfs = value;
			}
		}

		public CT_CellStyles CellStyles
		{
			get
			{
				return this._cellStyles;
			}
			set
			{
				this._cellStyles = value;
			}
		}

		public CT_Dxfs Dxfs
		{
			get
			{
				return this._dxfs;
			}
			set
			{
				this._dxfs = value;
			}
		}

		public CT_TableStyles TableStyles
		{
			get
			{
				return this._tableStyles;
			}
			set
			{
				this._tableStyles = value;
			}
		}

		public CT_Colors Colors
		{
			get
			{
				return this._colors;
			}
			set
			{
				this._colors = value;
			}
		}

		public static string NumFmtsElementName
		{
			get
			{
				return "numFmts";
			}
		}

		public static string FontsElementName
		{
			get
			{
				return "fonts";
			}
		}

		public static string FillsElementName
		{
			get
			{
				return "fills";
			}
		}

		public static string BordersElementName
		{
			get
			{
				return "borders";
			}
		}

		public static string CellStyleXfsElementName
		{
			get
			{
				return "cellStyleXfs";
			}
		}

		public static string CellXfsElementName
		{
			get
			{
				return "cellXfs";
			}
		}

		public static string CellStylesElementName
		{
			get
			{
				return "cellStyles";
			}
		}

		public static string DxfsElementName
		{
			get
			{
				return "dxfs";
			}
		}

		public static string TableStylesElementName
		{
			get
			{
				return "tableStyles";
			}
		}

		public static string ColorsElementName
		{
			get
			{
				return "colors";
			}
		}

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
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
			this.Write_numFmts(s, depth, namespaces);
			this.Write_fonts(s, depth, namespaces);
			this.Write_fills(s, depth, namespaces);
			this.Write_borders(s, depth, namespaces);
			this.Write_cellStyleXfs(s, depth, namespaces);
			this.Write_cellXfs(s, depth, namespaces);
			this.Write_cellStyles(s, depth, namespaces);
			this.Write_dxfs(s, depth, namespaces);
			this.Write_tableStyles(s, depth, namespaces);
			this.Write_colors(s, depth, namespaces);
		}

		public void Write_numFmts(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._numFmts != null)
			{
				this._numFmts.Write(s, "numFmts", depth + 1, namespaces);
			}
		}

		public void Write_fonts(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._fonts != null)
			{
				this._fonts.Write(s, "fonts", depth + 1, namespaces);
			}
		}

		public void Write_fills(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._fills != null)
			{
				this._fills.Write(s, "fills", depth + 1, namespaces);
			}
		}

		public void Write_borders(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._borders != null)
			{
				this._borders.Write(s, "borders", depth + 1, namespaces);
			}
		}

		public void Write_cellStyleXfs(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._cellStyleXfs != null)
			{
				this._cellStyleXfs.Write(s, "cellStyleXfs", depth + 1, namespaces);
			}
		}

		public void Write_cellXfs(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._cellXfs != null)
			{
				this._cellXfs.Write(s, "cellXfs", depth + 1, namespaces);
			}
		}

		public void Write_cellStyles(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._cellStyles != null)
			{
				this._cellStyles.Write(s, "cellStyles", depth + 1, namespaces);
			}
		}

		public void Write_dxfs(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._dxfs != null)
			{
				this._dxfs.Write(s, "dxfs", depth + 1, namespaces);
			}
		}

		public void Write_tableStyles(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._tableStyles != null)
			{
				this._tableStyles.Write(s, "tableStyles", depth + 1, namespaces);
			}
		}

		public void Write_colors(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._colors != null)
			{
				this._colors.Write(s, "colors", depth + 1, namespaces);
			}
		}
	}
}
