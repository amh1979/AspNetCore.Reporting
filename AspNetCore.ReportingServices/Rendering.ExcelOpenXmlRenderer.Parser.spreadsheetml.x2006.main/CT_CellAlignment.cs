using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_CellAlignment : OoxmlComplexType
	{
		private ST_HorizontalAlignment _horizontal_attr;

		private bool _horizontal_attr_is_specified;

		private ST_VerticalAlignment _vertical_attr;

		private bool _vertical_attr_is_specified;

		private uint _textRotation_attr;

		private bool _textRotation_attr_is_specified;

		private OoxmlBool _wrapText_attr;

		private bool _wrapText_attr_is_specified;

		private uint _indent_attr;

		private bool _indent_attr_is_specified;

		private int _relativeIndent_attr;

		private bool _relativeIndent_attr_is_specified;

		private OoxmlBool _justifyLastLine_attr;

		private bool _justifyLastLine_attr_is_specified;

		private OoxmlBool _shrinkToFit_attr;

		private bool _shrinkToFit_attr_is_specified;

		private uint _readingOrder_attr;

		private bool _readingOrder_attr_is_specified;

		public ST_HorizontalAlignment Horizontal_Attr
		{
			get
			{
				return this._horizontal_attr;
			}
			set
			{
				this._horizontal_attr = value;
				this._horizontal_attr_is_specified = true;
			}
		}

		public bool Horizontal_Attr_Is_Specified
		{
			get
			{
				return this._horizontal_attr_is_specified;
			}
			set
			{
				this._horizontal_attr_is_specified = value;
			}
		}

		public ST_VerticalAlignment Vertical_Attr
		{
			get
			{
				return this._vertical_attr;
			}
			set
			{
				this._vertical_attr = value;
				this._vertical_attr_is_specified = true;
			}
		}

		public bool Vertical_Attr_Is_Specified
		{
			get
			{
				return this._vertical_attr_is_specified;
			}
			set
			{
				this._vertical_attr_is_specified = value;
			}
		}

		public uint TextRotation_Attr
		{
			get
			{
				return this._textRotation_attr;
			}
			set
			{
				this._textRotation_attr = value;
				this._textRotation_attr_is_specified = true;
			}
		}

		public bool TextRotation_Attr_Is_Specified
		{
			get
			{
				return this._textRotation_attr_is_specified;
			}
			set
			{
				this._textRotation_attr_is_specified = value;
			}
		}

		public OoxmlBool WrapText_Attr
		{
			get
			{
				return this._wrapText_attr;
			}
			set
			{
				this._wrapText_attr = value;
				this._wrapText_attr_is_specified = true;
			}
		}

		public bool WrapText_Attr_Is_Specified
		{
			get
			{
				return this._wrapText_attr_is_specified;
			}
			set
			{
				this._wrapText_attr_is_specified = value;
			}
		}

		public uint Indent_Attr
		{
			get
			{
				return this._indent_attr;
			}
			set
			{
				this._indent_attr = value;
				this._indent_attr_is_specified = true;
			}
		}

		public bool Indent_Attr_Is_Specified
		{
			get
			{
				return this._indent_attr_is_specified;
			}
			set
			{
				this._indent_attr_is_specified = value;
			}
		}

		public int RelativeIndent_Attr
		{
			get
			{
				return this._relativeIndent_attr;
			}
			set
			{
				this._relativeIndent_attr = value;
				this._relativeIndent_attr_is_specified = true;
			}
		}

		public bool RelativeIndent_Attr_Is_Specified
		{
			get
			{
				return this._relativeIndent_attr_is_specified;
			}
			set
			{
				this._relativeIndent_attr_is_specified = value;
			}
		}

		public OoxmlBool JustifyLastLine_Attr
		{
			get
			{
				return this._justifyLastLine_attr;
			}
			set
			{
				this._justifyLastLine_attr = value;
				this._justifyLastLine_attr_is_specified = true;
			}
		}

		public bool JustifyLastLine_Attr_Is_Specified
		{
			get
			{
				return this._justifyLastLine_attr_is_specified;
			}
			set
			{
				this._justifyLastLine_attr_is_specified = value;
			}
		}

		public OoxmlBool ShrinkToFit_Attr
		{
			get
			{
				return this._shrinkToFit_attr;
			}
			set
			{
				this._shrinkToFit_attr = value;
				this._shrinkToFit_attr_is_specified = true;
			}
		}

		public bool ShrinkToFit_Attr_Is_Specified
		{
			get
			{
				return this._shrinkToFit_attr_is_specified;
			}
			set
			{
				this._shrinkToFit_attr_is_specified = value;
			}
		}

		public uint ReadingOrder_Attr
		{
			get
			{
				return this._readingOrder_attr;
			}
			set
			{
				this._readingOrder_attr = value;
				this._readingOrder_attr_is_specified = true;
			}
		}

		public bool ReadingOrder_Attr_Is_Specified
		{
			get
			{
				return this._readingOrder_attr_is_specified;
			}
			set
			{
				this._readingOrder_attr_is_specified = value;
			}
		}

		protected override void InitAttributes()
		{
			this._horizontal_attr_is_specified = false;
			this._vertical_attr_is_specified = false;
			this._textRotation_attr_is_specified = false;
			this._wrapText_attr_is_specified = false;
			this._indent_attr_is_specified = false;
			this._relativeIndent_attr_is_specified = false;
			this._justifyLastLine_attr_is_specified = false;
			this._shrinkToFit_attr_is_specified = false;
			this._readingOrder_attr_is_specified = false;
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
			if (this._horizontal_attr_is_specified)
			{
				s.Write(" horizontal=\"");
				OoxmlComplexType.WriteData(s, this._horizontal_attr);
				s.Write("\"");
			}
			if (this._vertical_attr_is_specified)
			{
				s.Write(" vertical=\"");
				OoxmlComplexType.WriteData(s, this._vertical_attr);
				s.Write("\"");
			}
			if (this._textRotation_attr_is_specified)
			{
				s.Write(" textRotation=\"");
				OoxmlComplexType.WriteData(s, this._textRotation_attr);
				s.Write("\"");
			}
			if (this._wrapText_attr_is_specified)
			{
				s.Write(" wrapText=\"");
				OoxmlComplexType.WriteData(s, this._wrapText_attr);
				s.Write("\"");
			}
			if (this._indent_attr_is_specified)
			{
				s.Write(" indent=\"");
				OoxmlComplexType.WriteData(s, this._indent_attr);
				s.Write("\"");
			}
			if (this._relativeIndent_attr_is_specified)
			{
				s.Write(" relativeIndent=\"");
				OoxmlComplexType.WriteData(s, this._relativeIndent_attr);
				s.Write("\"");
			}
			if (this._justifyLastLine_attr_is_specified)
			{
				s.Write(" justifyLastLine=\"");
				OoxmlComplexType.WriteData(s, this._justifyLastLine_attr);
				s.Write("\"");
			}
			if (this._shrinkToFit_attr_is_specified)
			{
				s.Write(" shrinkToFit=\"");
				OoxmlComplexType.WriteData(s, this._shrinkToFit_attr);
				s.Write("\"");
			}
			if (this._readingOrder_attr_is_specified)
			{
				s.Write(" readingOrder=\"");
				OoxmlComplexType.WriteData(s, this._readingOrder_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
