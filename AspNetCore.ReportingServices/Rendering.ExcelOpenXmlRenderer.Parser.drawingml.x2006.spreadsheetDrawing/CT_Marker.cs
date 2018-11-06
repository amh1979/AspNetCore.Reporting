using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing
{
	internal class CT_Marker : OoxmlComplexType
	{
		private int _col;

		private string _colOff;

		private int _row;

		private string _rowOff;

		public int Col
		{
			get
			{
				return this._col;
			}
			set
			{
				this._col = value;
			}
		}

		public string ColOff
		{
			get
			{
				return this._colOff;
			}
			set
			{
				this._colOff = value;
			}
		}

		public int Row
		{
			get
			{
				return this._row;
			}
			set
			{
				this._row = value;
			}
		}

		public string RowOff
		{
			get
			{
				return this._rowOff;
			}
			set
			{
				this._rowOff = value;
			}
		}

		public static string ColElementName
		{
			get
			{
				return "col";
			}
		}

		public static string RowElementName
		{
			get
			{
				return "row";
			}
		}

		public static string ColOffElementName
		{
			get
			{
				return "colOff";
			}
		}

		public static string RowOffElementName
		{
			get
			{
				return "rowOff";
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			this.Write_col(s, depth, namespaces);
			this.Write_colOff(s, depth, namespaces);
			this.Write_row(s, depth, namespaces);
			this.Write_rowOff(s, depth, namespaces);
		}

		public void Write_col(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "col", "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing", this._col);
		}

		public void Write_row(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "row", "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing", this._row);
		}

		public void Write_colOff(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._colOff != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "colOff", "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing", this._colOff);
			}
		}

		public void Write_rowOff(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._rowOff != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "rowOff", "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing", this._rowOff);
			}
		}
	}
}
