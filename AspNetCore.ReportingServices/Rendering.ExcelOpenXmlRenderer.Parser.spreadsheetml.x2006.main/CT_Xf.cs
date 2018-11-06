using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Xf : OoxmlComplexType
	{
		private OoxmlBool _quotePrefix_attr;

		private OoxmlBool _pivotButton_attr;

		private OoxmlBool _applyNumberFormat_attr;

		private OoxmlBool _applyFont_attr;

		private OoxmlBool _applyFill_attr;

		private OoxmlBool _applyBorder_attr;

		private OoxmlBool _applyAlignment_attr;

		private OoxmlBool _applyProtection_attr;

		private uint _numFmtId_attr;

		private bool _numFmtId_attr_is_specified;

		private uint _fontId_attr;

		private bool _fontId_attr_is_specified;

		private uint _fillId_attr;

		private bool _fillId_attr_is_specified;

		private uint _borderId_attr;

		private bool _borderId_attr_is_specified;

		private uint _xfId_attr;

		private bool _xfId_attr_is_specified;

		private CT_CellAlignment _alignment;

		public OoxmlBool QuotePrefix_Attr
		{
			get
			{
				return this._quotePrefix_attr;
			}
			set
			{
				this._quotePrefix_attr = value;
			}
		}

		public OoxmlBool PivotButton_Attr
		{
			get
			{
				return this._pivotButton_attr;
			}
			set
			{
				this._pivotButton_attr = value;
			}
		}

		public OoxmlBool ApplyNumberFormat_Attr
		{
			get
			{
				return this._applyNumberFormat_attr;
			}
			set
			{
				this._applyNumberFormat_attr = value;
			}
		}

		public OoxmlBool ApplyFont_Attr
		{
			get
			{
				return this._applyFont_attr;
			}
			set
			{
				this._applyFont_attr = value;
			}
		}

		public OoxmlBool ApplyFill_Attr
		{
			get
			{
				return this._applyFill_attr;
			}
			set
			{
				this._applyFill_attr = value;
			}
		}

		public OoxmlBool ApplyBorder_Attr
		{
			get
			{
				return this._applyBorder_attr;
			}
			set
			{
				this._applyBorder_attr = value;
			}
		}

		public OoxmlBool ApplyAlignment_Attr
		{
			get
			{
				return this._applyAlignment_attr;
			}
			set
			{
				this._applyAlignment_attr = value;
			}
		}

		public OoxmlBool ApplyProtection_Attr
		{
			get
			{
				return this._applyProtection_attr;
			}
			set
			{
				this._applyProtection_attr = value;
			}
		}

		public uint NumFmtId_Attr
		{
			get
			{
				return this._numFmtId_attr;
			}
			set
			{
				this._numFmtId_attr = value;
				this._numFmtId_attr_is_specified = true;
			}
		}

		public bool NumFmtId_Attr_Is_Specified
		{
			get
			{
				return this._numFmtId_attr_is_specified;
			}
			set
			{
				this._numFmtId_attr_is_specified = value;
			}
		}

		public uint FontId_Attr
		{
			get
			{
				return this._fontId_attr;
			}
			set
			{
				this._fontId_attr = value;
				this._fontId_attr_is_specified = true;
			}
		}

		public bool FontId_Attr_Is_Specified
		{
			get
			{
				return this._fontId_attr_is_specified;
			}
			set
			{
				this._fontId_attr_is_specified = value;
			}
		}

		public uint FillId_Attr
		{
			get
			{
				return this._fillId_attr;
			}
			set
			{
				this._fillId_attr = value;
				this._fillId_attr_is_specified = true;
			}
		}

		public bool FillId_Attr_Is_Specified
		{
			get
			{
				return this._fillId_attr_is_specified;
			}
			set
			{
				this._fillId_attr_is_specified = value;
			}
		}

		public uint BorderId_Attr
		{
			get
			{
				return this._borderId_attr;
			}
			set
			{
				this._borderId_attr = value;
				this._borderId_attr_is_specified = true;
			}
		}

		public bool BorderId_Attr_Is_Specified
		{
			get
			{
				return this._borderId_attr_is_specified;
			}
			set
			{
				this._borderId_attr_is_specified = value;
			}
		}

		public uint XfId_Attr
		{
			get
			{
				return this._xfId_attr;
			}
			set
			{
				this._xfId_attr = value;
				this._xfId_attr_is_specified = true;
			}
		}

		public bool XfId_Attr_Is_Specified
		{
			get
			{
				return this._xfId_attr_is_specified;
			}
			set
			{
				this._xfId_attr_is_specified = value;
			}
		}

		public CT_CellAlignment Alignment
		{
			get
			{
				return this._alignment;
			}
			set
			{
				this._alignment = value;
			}
		}

		public static string AlignmentElementName
		{
			get
			{
				return "alignment";
			}
		}

		protected override void InitAttributes()
		{
			this._quotePrefix_attr = OoxmlBool.OoxmlFalse;
			this._pivotButton_attr = OoxmlBool.OoxmlFalse;
			this._applyNumberFormat_attr = OoxmlBool.OoxmlFalse;
			this._applyFont_attr = OoxmlBool.OoxmlFalse;
			this._applyFill_attr = OoxmlBool.OoxmlFalse;
			this._applyBorder_attr = OoxmlBool.OoxmlFalse;
			this._applyAlignment_attr = OoxmlBool.OoxmlFalse;
			this._applyProtection_attr = OoxmlBool.OoxmlFalse;
			this._numFmtId_attr_is_specified = false;
			this._fontId_attr_is_specified = false;
			this._fillId_attr_is_specified = false;
			this._borderId_attr_is_specified = false;
			this._xfId_attr_is_specified = false;
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
			if ((bool)(this._quotePrefix_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" quotePrefix=\"");
				OoxmlComplexType.WriteData(s, this._quotePrefix_attr);
				s.Write("\"");
			}
			if ((bool)(this._pivotButton_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" pivotButton=\"");
				OoxmlComplexType.WriteData(s, this._pivotButton_attr);
				s.Write("\"");
			}
			if ((bool)(this._applyNumberFormat_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" applyNumberFormat=\"");
				OoxmlComplexType.WriteData(s, this._applyNumberFormat_attr);
				s.Write("\"");
			}
			if ((bool)(this._applyFont_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" applyFont=\"");
				OoxmlComplexType.WriteData(s, this._applyFont_attr);
				s.Write("\"");
			}
			if ((bool)(this._applyFill_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" applyFill=\"");
				OoxmlComplexType.WriteData(s, this._applyFill_attr);
				s.Write("\"");
			}
			if ((bool)(this._applyBorder_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" applyBorder=\"");
				OoxmlComplexType.WriteData(s, this._applyBorder_attr);
				s.Write("\"");
			}
			if ((bool)(this._applyAlignment_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" applyAlignment=\"");
				OoxmlComplexType.WriteData(s, this._applyAlignment_attr);
				s.Write("\"");
			}
			if ((bool)(this._applyProtection_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" applyProtection=\"");
				OoxmlComplexType.WriteData(s, this._applyProtection_attr);
				s.Write("\"");
			}
			if (this._numFmtId_attr_is_specified)
			{
				s.Write(" numFmtId=\"");
				OoxmlComplexType.WriteData(s, this._numFmtId_attr);
				s.Write("\"");
			}
			if (this._fontId_attr_is_specified)
			{
				s.Write(" fontId=\"");
				OoxmlComplexType.WriteData(s, this._fontId_attr);
				s.Write("\"");
			}
			if (this._fillId_attr_is_specified)
			{
				s.Write(" fillId=\"");
				OoxmlComplexType.WriteData(s, this._fillId_attr);
				s.Write("\"");
			}
			if (this._borderId_attr_is_specified)
			{
				s.Write(" borderId=\"");
				OoxmlComplexType.WriteData(s, this._borderId_attr);
				s.Write("\"");
			}
			if (this._xfId_attr_is_specified)
			{
				s.Write(" xfId=\"");
				OoxmlComplexType.WriteData(s, this._xfId_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			this.Write_alignment(s, depth, namespaces);
		}

		public void Write_alignment(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._alignment != null)
			{
				this._alignment.Write(s, "alignment", depth + 1, namespaces);
			}
		}
	}
}
