using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Spacing : OoxmlComplexType, IOoxmlComplexType
	{
		private string _after_attr;

		private bool _after_attr_is_specified;

		private string _line_attr;

		private bool _line_attr_is_specified;

		private ST_LineSpacingRule _lineRule_attr;

		private bool _lineRule_attr_is_specified;

		public ST_LineSpacingRule LineRule_Attr
		{
			get
			{
				return this._lineRule_attr;
			}
			set
			{
				this._lineRule_attr = value;
				this._lineRule_attr_is_specified = true;
			}
		}

		public string After_Attr
		{
			get
			{
				return this._after_attr;
			}
			set
			{
				this._after_attr = value;
				this._after_attr_is_specified = (value != null);
			}
		}

		public string Line_Attr
		{
			get
			{
				return this._line_attr;
			}
			set
			{
				this._line_attr = value;
				this._line_attr_is_specified = (value != null);
			}
		}

		protected override void InitAttributes()
		{
			this._after_attr_is_specified = false;
			this._line_attr_is_specified = false;
			this._lineRule_attr_is_specified = false;
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
		}

		public override void Write(TextWriter s, string tagName)
		{
			base.WriteEmptyTag(s, tagName, "w");
		}

		public override void WriteOpenTag(TextWriter s, string tagName, Dictionary<string, string> namespaces)
		{
			base.WriteOpenTag(s, tagName, "w", namespaces);
		}

		public override void WriteCloseTag(TextWriter s, string tagName)
		{
			s.Write("</w:");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			if (this._after_attr_is_specified)
			{
				s.Write(" w:after=\"");
				OoxmlComplexType.WriteData(s, this._after_attr);
				s.Write("\"");
			}
			if (this._line_attr_is_specified)
			{
				s.Write(" w:line=\"");
				OoxmlComplexType.WriteData(s, this._line_attr);
				s.Write("\"");
			}
			if (this._lineRule_attr_is_specified)
			{
				s.Write(" w:lineRule=\"");
				OoxmlComplexType.WriteData(s, this._lineRule_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s)
		{
		}
	}
}
