using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_PageMar : OoxmlComplexType, IOoxmlComplexType
	{
		private string _top_attr;

		private string _right_attr;

		private string _bottom_attr;

		private string _left_attr;

		private string _header_attr;

		private string _footer_attr;

		private string _gutter_attr;

		public string Top_Attr
		{
			get
			{
				return this._top_attr;
			}
			set
			{
				this._top_attr = value;
			}
		}

		public string Right_Attr
		{
			get
			{
				return this._right_attr;
			}
			set
			{
				this._right_attr = value;
			}
		}

		public string Bottom_Attr
		{
			get
			{
				return this._bottom_attr;
			}
			set
			{
				this._bottom_attr = value;
			}
		}

		public string Left_Attr
		{
			get
			{
				return this._left_attr;
			}
			set
			{
				this._left_attr = value;
			}
		}

		public string Header_Attr
		{
			get
			{
				return this._header_attr;
			}
			set
			{
				this._header_attr = value;
			}
		}

		public string Footer_Attr
		{
			get
			{
				return this._footer_attr;
			}
			set
			{
				this._footer_attr = value;
			}
		}

		public string Gutter_Attr
		{
			get
			{
				return this._gutter_attr;
			}
			set
			{
				this._gutter_attr = value;
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
			s.Write(" w:top=\"");
			OoxmlComplexType.WriteData(s, this._top_attr);
			s.Write("\"");
			s.Write(" w:right=\"");
			OoxmlComplexType.WriteData(s, this._right_attr);
			s.Write("\"");
			s.Write(" w:bottom=\"");
			OoxmlComplexType.WriteData(s, this._bottom_attr);
			s.Write("\"");
			s.Write(" w:left=\"");
			OoxmlComplexType.WriteData(s, this._left_attr);
			s.Write("\"");
			s.Write(" w:header=\"");
			OoxmlComplexType.WriteData(s, this._header_attr);
			s.Write("\"");
			s.Write(" w:footer=\"");
			OoxmlComplexType.WriteData(s, this._footer_attr);
			s.Write("\"");
			s.Write(" w:gutter=\"");
			OoxmlComplexType.WriteData(s, this._gutter_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
		}
	}
}
