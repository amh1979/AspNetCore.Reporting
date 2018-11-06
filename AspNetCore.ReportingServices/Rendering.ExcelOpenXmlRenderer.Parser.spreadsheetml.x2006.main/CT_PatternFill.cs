using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_PatternFill : OoxmlComplexType
	{
		private ST_PatternType _patternType_attr;

		private bool _patternType_attr_is_specified;

		private CT_Color _fgColor;

		private CT_Color _bgColor;

		public ST_PatternType PatternType_Attr
		{
			get
			{
				return this._patternType_attr;
			}
			set
			{
				this._patternType_attr = value;
				this._patternType_attr_is_specified = true;
			}
		}

		public bool PatternType_Attr_Is_Specified
		{
			get
			{
				return this._patternType_attr_is_specified;
			}
			set
			{
				this._patternType_attr_is_specified = value;
			}
		}

		public CT_Color FgColor
		{
			get
			{
				return this._fgColor;
			}
			set
			{
				this._fgColor = value;
			}
		}

		public CT_Color BgColor
		{
			get
			{
				return this._bgColor;
			}
			set
			{
				this._bgColor = value;
			}
		}

		public static string FgColorElementName
		{
			get
			{
				return "fgColor";
			}
		}

		public static string BgColorElementName
		{
			get
			{
				return "bgColor";
			}
		}

		protected override void InitAttributes()
		{
			this._patternType_attr_is_specified = false;
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
			if (this._patternType_attr_is_specified)
			{
				s.Write(" patternType=\"");
				OoxmlComplexType.WriteData(s, this._patternType_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			this.Write_fgColor(s, depth, namespaces);
			this.Write_bgColor(s, depth, namespaces);
		}

		public void Write_fgColor(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._fgColor != null)
			{
				this._fgColor.Write(s, "fgColor", depth + 1, namespaces);
			}
		}

		public void Write_bgColor(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._bgColor != null)
			{
				this._bgColor.Write(s, "bgColor", depth + 1, namespaces);
			}
		}
	}
}
