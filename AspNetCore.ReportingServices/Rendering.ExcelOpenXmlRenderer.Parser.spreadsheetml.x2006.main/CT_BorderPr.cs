using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_BorderPr : OoxmlComplexType
	{
		private ST_BorderStyle _style_attr;

		private CT_Color _color;

		public ST_BorderStyle Style_Attr
		{
			get
			{
				return this._style_attr;
			}
			set
			{
				this._style_attr = value;
			}
		}

		public CT_Color Color
		{
			get
			{
				return this._color;
			}
			set
			{
				this._color = value;
			}
		}

		public static string ColorElementName
		{
			get
			{
				return "color";
			}
		}

		protected override void InitAttributes()
		{
			this._style_attr = ST_BorderStyle.none;
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
			if (this._style_attr != ST_BorderStyle.none)
			{
				s.Write(" style=\"");
				OoxmlComplexType.WriteData(s, this._style_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			this.Write_color(s, depth, namespaces);
		}

		public void Write_color(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._color != null)
			{
				this._color.Write(s, "color", depth + 1, namespaces);
			}
		}
	}
}
