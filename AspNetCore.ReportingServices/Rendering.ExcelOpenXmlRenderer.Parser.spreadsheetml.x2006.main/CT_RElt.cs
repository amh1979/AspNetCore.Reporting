using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_RElt : OoxmlComplexType
	{
		private CT_RPrElt _rPr;

		private string _t;

		public CT_RPrElt RPr
		{
			get
			{
				return this._rPr;
			}
			set
			{
				this._rPr = value;
			}
		}

		public string T
		{
			get
			{
				return this._t;
			}
			set
			{
				this._t = value;
			}
		}

		public static string RPrElementName
		{
			get
			{
				return "rPr";
			}
		}

		public static string TElementName
		{
			get
			{
				return "t";
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
			this.Write_rPr(s, depth, namespaces);
			this.Write_t(s, depth, namespaces);
		}

		public void Write_rPr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._rPr != null)
			{
				this._rPr.Write(s, "rPr", depth + 1, namespaces);
			}
		}

		public void Write_t(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._t != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "t", true, "http://schemas.openxmlformats.org/spreadsheetml/2006/main", this._t);
			}
		}
	}
}
