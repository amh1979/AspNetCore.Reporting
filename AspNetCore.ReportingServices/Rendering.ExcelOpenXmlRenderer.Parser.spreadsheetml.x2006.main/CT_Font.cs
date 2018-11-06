using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Font : OoxmlComplexType
	{
		private CT_BooleanProperty _b;

		private CT_BooleanProperty _i;

		private CT_BooleanProperty _strike;

		private CT_BooleanProperty _condense;

		private CT_BooleanProperty _extend;

		private CT_BooleanProperty _outline;

		private CT_BooleanProperty _shadow;

		private CT_UnderlineProperty _u;

		private CT_VerticalAlignFontProperty _vertAlign;

		private CT_FontSize _sz;

		private CT_Color _color;

		private CT_FontName _name;

		private CT_IntProperty _family;

		private CT_IntProperty _charset;

		private CT_FontScheme _scheme;

		public CT_BooleanProperty B
		{
			get
			{
				return this._b;
			}
			set
			{
				this._b = value;
			}
		}

		public CT_BooleanProperty I
		{
			get
			{
				return this._i;
			}
			set
			{
				this._i = value;
			}
		}

		public CT_BooleanProperty Strike
		{
			get
			{
				return this._strike;
			}
			set
			{
				this._strike = value;
			}
		}

		public CT_BooleanProperty Condense
		{
			get
			{
				return this._condense;
			}
			set
			{
				this._condense = value;
			}
		}

		public CT_BooleanProperty Extend
		{
			get
			{
				return this._extend;
			}
			set
			{
				this._extend = value;
			}
		}

		public CT_BooleanProperty Outline
		{
			get
			{
				return this._outline;
			}
			set
			{
				this._outline = value;
			}
		}

		public CT_BooleanProperty Shadow
		{
			get
			{
				return this._shadow;
			}
			set
			{
				this._shadow = value;
			}
		}

		public CT_UnderlineProperty U
		{
			get
			{
				return this._u;
			}
			set
			{
				this._u = value;
			}
		}

		public CT_VerticalAlignFontProperty VertAlign
		{
			get
			{
				return this._vertAlign;
			}
			set
			{
				this._vertAlign = value;
			}
		}

		public CT_FontSize Sz
		{
			get
			{
				return this._sz;
			}
			set
			{
				this._sz = value;
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

		public CT_FontName Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		public CT_IntProperty Family
		{
			get
			{
				return this._family;
			}
			set
			{
				this._family = value;
			}
		}

		public CT_IntProperty Charset
		{
			get
			{
				return this._charset;
			}
			set
			{
				this._charset = value;
			}
		}

		public CT_FontScheme Scheme
		{
			get
			{
				return this._scheme;
			}
			set
			{
				this._scheme = value;
			}
		}

		public static string BElementName
		{
			get
			{
				return "b";
			}
		}

		public static string IElementName
		{
			get
			{
				return "i";
			}
		}

		public static string StrikeElementName
		{
			get
			{
				return "strike";
			}
		}

		public static string CondenseElementName
		{
			get
			{
				return "condense";
			}
		}

		public static string ExtendElementName
		{
			get
			{
				return "extend";
			}
		}

		public static string OutlineElementName
		{
			get
			{
				return "outline";
			}
		}

		public static string ShadowElementName
		{
			get
			{
				return "shadow";
			}
		}

		public static string UElementName
		{
			get
			{
				return "u";
			}
		}

		public static string VertAlignElementName
		{
			get
			{
				return "vertAlign";
			}
		}

		public static string SzElementName
		{
			get
			{
				return "sz";
			}
		}

		public static string ColorElementName
		{
			get
			{
				return "color";
			}
		}

		public static string NameElementName
		{
			get
			{
				return "name";
			}
		}

		public static string FamilyElementName
		{
			get
			{
				return "family";
			}
		}

		public static string CharsetElementName
		{
			get
			{
				return "charset";
			}
		}

		public static string SchemeElementName
		{
			get
			{
				return "scheme";
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
			this.Write_b(s, depth, namespaces);
			this.Write_i(s, depth, namespaces);
			this.Write_strike(s, depth, namespaces);
			this.Write_condense(s, depth, namespaces);
			this.Write_extend(s, depth, namespaces);
			this.Write_outline(s, depth, namespaces);
			this.Write_shadow(s, depth, namespaces);
			this.Write_u(s, depth, namespaces);
			this.Write_vertAlign(s, depth, namespaces);
			this.Write_sz(s, depth, namespaces);
			this.Write_color(s, depth, namespaces);
			this.Write_name(s, depth, namespaces);
			this.Write_family(s, depth, namespaces);
			this.Write_charset(s, depth, namespaces);
			this.Write_scheme(s, depth, namespaces);
		}

		public void Write_b(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._b != null)
			{
				this._b.Write(s, "b", depth + 1, namespaces);
			}
		}

		public void Write_i(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._i != null)
			{
				this._i.Write(s, "i", depth + 1, namespaces);
			}
		}

		public void Write_strike(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._strike != null)
			{
				this._strike.Write(s, "strike", depth + 1, namespaces);
			}
		}

		public void Write_condense(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._condense != null)
			{
				this._condense.Write(s, "condense", depth + 1, namespaces);
			}
		}

		public void Write_extend(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._extend != null)
			{
				this._extend.Write(s, "extend", depth + 1, namespaces);
			}
		}

		public void Write_outline(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._outline != null)
			{
				this._outline.Write(s, "outline", depth + 1, namespaces);
			}
		}

		public void Write_shadow(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._shadow != null)
			{
				this._shadow.Write(s, "shadow", depth + 1, namespaces);
			}
		}

		public void Write_u(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._u != null)
			{
				this._u.Write(s, "u", depth + 1, namespaces);
			}
		}

		public void Write_vertAlign(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._vertAlign != null)
			{
				this._vertAlign.Write(s, "vertAlign", depth + 1, namespaces);
			}
		}

		public void Write_sz(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._sz != null)
			{
				this._sz.Write(s, "sz", depth + 1, namespaces);
			}
		}

		public void Write_color(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._color != null)
			{
				this._color.Write(s, "color", depth + 1, namespaces);
			}
		}

		public void Write_name(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._name != null)
			{
				this._name.Write(s, "name", depth + 1, namespaces);
			}
		}

		public void Write_family(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._family != null)
			{
				this._family.Write(s, "family", depth + 1, namespaces);
			}
		}

		public void Write_charset(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._charset != null)
			{
				this._charset.Write(s, "charset", depth + 1, namespaces);
			}
		}

		public void Write_scheme(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._scheme != null)
			{
				this._scheme.Write(s, "scheme", depth + 1, namespaces);
			}
		}
	}
}
