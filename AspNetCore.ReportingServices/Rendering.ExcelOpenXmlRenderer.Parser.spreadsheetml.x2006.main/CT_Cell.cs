using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Cell : OoxmlComplexType
	{
		private uint _s_attr;

		private ST_CellType _t_attr;

		private uint _cm_attr;

		private uint _vm_attr;

		private OoxmlBool _ph_attr;

		private string _r_attr;

		private bool _r_attr_is_specified;

		private string _v;

		private CT_Rst _is;

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

		public ST_CellType T_Attr
		{
			get
			{
				return this._t_attr;
			}
			set
			{
				this._t_attr = value;
			}
		}

		public uint Cm_Attr
		{
			get
			{
				return this._cm_attr;
			}
			set
			{
				this._cm_attr = value;
			}
		}

		public uint Vm_Attr
		{
			get
			{
				return this._vm_attr;
			}
			set
			{
				this._vm_attr = value;
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

		public string R_Attr
		{
			get
			{
				return this._r_attr;
			}
			set
			{
				this._r_attr = value;
				this._r_attr_is_specified = (value != null);
			}
		}

		public string V
		{
			get
			{
				return this._v;
			}
			set
			{
				this._v = value;
			}
		}

		public CT_Rst Is
		{
			get
			{
				return this._is;
			}
			set
			{
				this._is = value;
			}
		}

		public static string IsElementName
		{
			get
			{
				return "is";
			}
		}

		public static string VElementName
		{
			get
			{
				return "v";
			}
		}

		protected override void InitAttributes()
		{
			this._s_attr = Convert.ToUInt32("0", CultureInfo.InvariantCulture);
			this._t_attr = ST_CellType.n;
			this._cm_attr = Convert.ToUInt32("0", CultureInfo.InvariantCulture);
			this._vm_attr = Convert.ToUInt32("0", CultureInfo.InvariantCulture);
			this._ph_attr = OoxmlBool.OoxmlFalse;
			this._r_attr_is_specified = false;
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
			if (this._s_attr != Convert.ToUInt32("0", CultureInfo.InvariantCulture))
			{
				s.Write(" s=\"");
				OoxmlComplexType.WriteData(s, this._s_attr);
				s.Write("\"");
			}
			if (this._t_attr != ST_CellType.n)
			{
				s.Write(" t=\"");
				OoxmlComplexType.WriteData(s, this._t_attr);
				s.Write("\"");
			}
			if (this._cm_attr != Convert.ToUInt32("0", CultureInfo.InvariantCulture))
			{
				s.Write(" cm=\"");
				OoxmlComplexType.WriteData(s, this._cm_attr);
				s.Write("\"");
			}
			if (this._vm_attr != Convert.ToUInt32("0", CultureInfo.InvariantCulture))
			{
				s.Write(" vm=\"");
				OoxmlComplexType.WriteData(s, this._vm_attr);
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
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			this.Write_v(s, depth, namespaces);
			this.Write_is(s, depth, namespaces);
		}

		public void Write_is(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._is != null)
			{
				this._is.Write(s, "is", depth + 1, namespaces);
			}
		}

		public void Write_v(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._v != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "v", "http://schemas.openxmlformats.org/spreadsheetml/2006/main", this._v);
			}
		}
	}
}
