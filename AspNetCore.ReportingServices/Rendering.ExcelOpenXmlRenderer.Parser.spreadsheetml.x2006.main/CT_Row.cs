using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Row : OoxmlComplexType
	{
		private uint _s_attr;

		private OoxmlBool _customFormat_attr;

		private OoxmlBool _hidden_attr;

		private byte _outlineLevel_attr;

		private OoxmlBool _collapsed_attr;

		private OoxmlBool _thickTop_attr;

		private OoxmlBool _thickBot_attr;

		private OoxmlBool _ph_attr;

		private uint _r_attr;

		private bool _r_attr_is_specified;

		private List<string> _spans_attr;

		private bool _spans_attr_is_specified;

		private double _ht_attr;

		private bool _ht_attr_is_specified;

		private OoxmlBool _customHeight_attr;

		private bool _customHeight_attr_is_specified;

		private List<CT_Cell> _c;

		public uint S_Attr
		{
			get
			{
				return this._s_attr;
			}
			set
			{
				this._s_attr = value;
			}
		}

		public OoxmlBool CustomFormat_Attr
		{
			get
			{
				return this._customFormat_attr;
			}
			set
			{
				this._customFormat_attr = value;
			}
		}

		public OoxmlBool Hidden_Attr
		{
			get
			{
				return this._hidden_attr;
			}
			set
			{
				this._hidden_attr = value;
			}
		}

		public byte OutlineLevel_Attr
		{
			get
			{
				return this._outlineLevel_attr;
			}
			set
			{
				this._outlineLevel_attr = value;
			}
		}

		public OoxmlBool Collapsed_Attr
		{
			get
			{
				return this._collapsed_attr;
			}
			set
			{
				this._collapsed_attr = value;
			}
		}

		public OoxmlBool ThickTop_Attr
		{
			get
			{
				return this._thickTop_attr;
			}
			set
			{
				this._thickTop_attr = value;
			}
		}

		public OoxmlBool ThickBot_Attr
		{
			get
			{
				return this._thickBot_attr;
			}
			set
			{
				this._thickBot_attr = value;
			}
		}

		public OoxmlBool Ph_Attr
		{
			get
			{
				return this._ph_attr;
			}
			set
			{
				this._ph_attr = value;
			}
		}

		public uint R_Attr
		{
			get
			{
				return this._r_attr;
			}
			set
			{
				this._r_attr = value;
				this._r_attr_is_specified = true;
			}
		}

		public bool R_Attr_Is_Specified
		{
			get
			{
				return this._r_attr_is_specified;
			}
			set
			{
				this._r_attr_is_specified = value;
			}
		}

		public double Ht_Attr
		{
			get
			{
				return this._ht_attr;
			}
			set
			{
				this._ht_attr = value;
				this._ht_attr_is_specified = true;
			}
		}

		public bool Ht_Attr_Is_Specified
		{
			get
			{
				return this._ht_attr_is_specified;
			}
			set
			{
				this._ht_attr_is_specified = value;
			}
		}

		public OoxmlBool CustomHeight_Attr
		{
			get
			{
				return this._customHeight_attr;
			}
			set
			{
				this._customHeight_attr = value;
				this._customHeight_attr_is_specified = true;
			}
		}

		public bool CustomHeight_Attr_Is_Specified
		{
			get
			{
				return this._customHeight_attr_is_specified;
			}
			set
			{
				this._customHeight_attr_is_specified = value;
			}
		}

		public List<string> Spans_Attr
		{
			get
			{
				return this._spans_attr;
			}
			set
			{
				this._spans_attr = value;
				this._spans_attr_is_specified = (value != null);
			}
		}

		public List<CT_Cell> C
		{
			get
			{
				return this._c;
			}
			set
			{
				this._c = value;
			}
		}

		public static string CElementName
		{
			get
			{
				return "c";
			}
		}

		protected override void InitAttributes()
		{
			this._s_attr = Convert.ToUInt32("0", CultureInfo.InvariantCulture);
			this._customFormat_attr = OoxmlBool.OoxmlFalse;
			this._hidden_attr = OoxmlBool.OoxmlFalse;
			this._outlineLevel_attr = Convert.ToByte("0", CultureInfo.InvariantCulture);
			this._collapsed_attr = OoxmlBool.OoxmlFalse;
			this._thickTop_attr = OoxmlBool.OoxmlFalse;
			this._thickBot_attr = OoxmlBool.OoxmlFalse;
			this._ph_attr = OoxmlBool.OoxmlFalse;
			this._r_attr_is_specified = false;
			this._spans_attr_is_specified = false;
			this._ht_attr_is_specified = false;
			this._customHeight_attr_is_specified = false;
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
			this._c = new List<CT_Cell>();
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
			if (this._s_attr != Convert.ToUInt32("0", CultureInfo.InvariantCulture))
			{
				s.Write(" s=\"");
				OoxmlComplexType.WriteData(s, this._s_attr);
				s.Write("\"");
			}
			if ((bool)(this._customFormat_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" customFormat=\"");
				OoxmlComplexType.WriteData(s, this._customFormat_attr);
				s.Write("\"");
			}
			if ((bool)(this._hidden_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" hidden=\"");
				OoxmlComplexType.WriteData(s, this._hidden_attr);
				s.Write("\"");
			}
			if (this._outlineLevel_attr != Convert.ToByte("0", CultureInfo.InvariantCulture))
			{
				s.Write(" outlineLevel=\"");
				OoxmlComplexType.WriteData(s, this._outlineLevel_attr);
				s.Write("\"");
			}
			if ((bool)(this._collapsed_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" collapsed=\"");
				OoxmlComplexType.WriteData(s, this._collapsed_attr);
				s.Write("\"");
			}
			if ((bool)(this._thickTop_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" thickTop=\"");
				OoxmlComplexType.WriteData(s, this._thickTop_attr);
				s.Write("\"");
			}
			if ((bool)(this._thickBot_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" thickBot=\"");
				OoxmlComplexType.WriteData(s, this._thickBot_attr);
				s.Write("\"");
			}
			if ((bool)(this._ph_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" ph=\"");
				OoxmlComplexType.WriteData(s, this._ph_attr);
				s.Write("\"");
			}
			if (this._r_attr_is_specified)
			{
				s.Write(" r=\"");
				OoxmlComplexType.WriteData(s, this._r_attr);
				s.Write("\"");
			}
			if (this._ht_attr_is_specified)
			{
				s.Write(" ht=\"");
				OoxmlComplexType.WriteData(s, this._ht_attr);
				s.Write("\"");
			}
			if (this._customHeight_attr_is_specified)
			{
				s.Write(" customHeight=\"");
				OoxmlComplexType.WriteData(s, this._customHeight_attr);
				s.Write("\"");
			}
			if (this._spans_attr_is_specified)
			{
				s.Write(" spans=\"");
				for (int i = 0; i < this._spans_attr.Count - 1; i++)
				{
					OoxmlComplexType.WriteData(s, this._spans_attr[i]);
					s.Write(" ");
				}
				OoxmlComplexType.WriteData(s, this._spans_attr[this._spans_attr.Count - 1]);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			this.Write_c(s, depth, namespaces);
		}

		public void Write_c(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._c != null)
			{
				foreach (CT_Cell item in this._c)
				{
					if (item != null)
					{
						item.Write(s, "c", depth + 1, namespaces);
					}
				}
			}
		}
	}
}
