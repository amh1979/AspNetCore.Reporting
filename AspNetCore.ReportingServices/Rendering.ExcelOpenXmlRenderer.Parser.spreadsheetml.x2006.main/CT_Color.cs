using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Color : OoxmlComplexType
	{
		private OoxmlBool _auto_attr;

		private double _tint_attr;

		private uint _indexed_attr;

		private bool _indexed_attr_is_specified;

		private string _rgb_attr;

		private bool _rgb_attr_is_specified;

		private uint _theme_attr;

		private bool _theme_attr_is_specified;

		public OoxmlBool Auto_Attr
		{
			get
			{
				return this._auto_attr;
			}
			set
			{
				this._auto_attr = value;
			}
		}

		public double Tint_Attr
		{
			get
			{
				return this._tint_attr;
			}
			set
			{
				this._tint_attr = value;
			}
		}

		public uint Indexed_Attr
		{
			get
			{
				return this._indexed_attr;
			}
			set
			{
				this._indexed_attr = value;
				this._indexed_attr_is_specified = true;
			}
		}

		public bool Indexed_Attr_Is_Specified
		{
			get
			{
				return this._indexed_attr_is_specified;
			}
			set
			{
				this._indexed_attr_is_specified = value;
			}
		}

		public uint Theme_Attr
		{
			get
			{
				return this._theme_attr;
			}
			set
			{
				this._theme_attr = value;
				this._theme_attr_is_specified = true;
			}
		}

		public bool Theme_Attr_Is_Specified
		{
			get
			{
				return this._theme_attr_is_specified;
			}
			set
			{
				this._theme_attr_is_specified = value;
			}
		}

		public string Rgb_Attr
		{
			get
			{
				return this._rgb_attr;
			}
			set
			{
				this._rgb_attr = value;
				this._rgb_attr_is_specified = (value != null);
			}
		}

		protected override void InitAttributes()
		{
			this._auto_attr = OoxmlBool.OoxmlFalse;
			this._tint_attr = Convert.ToDouble("0.0", CultureInfo.InvariantCulture);
			this._indexed_attr_is_specified = false;
			this._rgb_attr_is_specified = false;
			this._theme_attr_is_specified = false;
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
			if ((bool)(this._auto_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" auto=\"");
				OoxmlComplexType.WriteData(s, this._auto_attr);
				s.Write("\"");
			}
			if (this._tint_attr != Convert.ToDouble("0.0", CultureInfo.InvariantCulture))
			{
				s.Write(" tint=\"");
				OoxmlComplexType.WriteData(s, this._tint_attr);
				s.Write("\"");
			}
			if (this._indexed_attr_is_specified)
			{
				s.Write(" indexed=\"");
				OoxmlComplexType.WriteData(s, this._indexed_attr);
				s.Write("\"");
			}
			if (this._rgb_attr_is_specified)
			{
				s.Write(" rgb=\"");
				OoxmlComplexType.WriteData(s, this._rgb_attr);
				s.Write("\"");
			}
			if (this._theme_attr_is_specified)
			{
				s.Write(" theme=\"");
				OoxmlComplexType.WriteData(s, this._theme_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
