using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Fonts : OoxmlComplexType, IOoxmlComplexType
	{
		private string _ascii_attr;

		private bool _ascii_attr_is_specified;

		private string _hAnsi_attr;

		private bool _hAnsi_attr_is_specified;

		private string _eastAsia_attr;

		private bool _eastAsia_attr_is_specified;

		private string _cs_attr;

		private bool _cs_attr_is_specified;

		public string Ascii_Attr
		{
			get
			{
				return this._ascii_attr;
			}
			set
			{
				this._ascii_attr = value;
				this._ascii_attr_is_specified = (value != null);
			}
		}

		public string HAnsi_Attr
		{
			get
			{
				return this._hAnsi_attr;
			}
			set
			{
				this._hAnsi_attr = value;
				this._hAnsi_attr_is_specified = (value != null);
			}
		}

		public string EastAsia_Attr
		{
			get
			{
				return this._eastAsia_attr;
			}
			set
			{
				this._eastAsia_attr = value;
				this._eastAsia_attr_is_specified = (value != null);
			}
		}

		public string Cs_Attr
		{
			get
			{
				return this._cs_attr;
			}
			set
			{
				this._cs_attr = value;
				this._cs_attr_is_specified = (value != null);
			}
		}

		protected override void InitAttributes()
		{
			this._ascii_attr_is_specified = false;
			this._hAnsi_attr_is_specified = false;
			this._eastAsia_attr_is_specified = false;
			this._cs_attr_is_specified = false;
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
			if (this._ascii_attr_is_specified)
			{
				s.Write(" w:ascii=\"");
				OoxmlComplexType.WriteData(s, this._ascii_attr);
				s.Write("\"");
			}
			if (this._hAnsi_attr_is_specified)
			{
				s.Write(" w:hAnsi=\"");
				OoxmlComplexType.WriteData(s, this._hAnsi_attr);
				s.Write("\"");
			}
			if (this._eastAsia_attr_is_specified)
			{
				s.Write(" w:eastAsia=\"");
				OoxmlComplexType.WriteData(s, this._eastAsia_attr);
				s.Write("\"");
			}
			if (this._cs_attr_is_specified)
			{
				s.Write(" w:cs=\"");
				OoxmlComplexType.WriteData(s, this._cs_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s)
		{
		}
	}
}
