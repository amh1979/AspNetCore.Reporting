using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_CellStyle : OoxmlComplexType
	{
		private uint _xfId_attr;

		private string _name_attr;

		private bool _name_attr_is_specified;

		private uint _builtinId_attr;

		private bool _builtinId_attr_is_specified;

		private uint _iLevel_attr;

		private bool _iLevel_attr_is_specified;

		private OoxmlBool _hidden_attr;

		private bool _hidden_attr_is_specified;

		private OoxmlBool _customBuiltin_attr;

		private bool _customBuiltin_attr_is_specified;

		public uint XfId_Attr
		{
			get
			{
				return this._xfId_attr;
			}
			set
			{
				this._xfId_attr = value;
			}
		}

		public uint BuiltinId_Attr
		{
			get
			{
				return this._builtinId_attr;
			}
			set
			{
				this._builtinId_attr = value;
				this._builtinId_attr_is_specified = true;
			}
		}

		public bool BuiltinId_Attr_Is_Specified
		{
			get
			{
				return this._builtinId_attr_is_specified;
			}
			set
			{
				this._builtinId_attr_is_specified = value;
			}
		}

		public uint ILevel_Attr
		{
			get
			{
				return this._iLevel_attr;
			}
			set
			{
				this._iLevel_attr = value;
				this._iLevel_attr_is_specified = true;
			}
		}

		public bool ILevel_Attr_Is_Specified
		{
			get
			{
				return this._iLevel_attr_is_specified;
			}
			set
			{
				this._iLevel_attr_is_specified = value;
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
				this._hidden_attr_is_specified = true;
			}
		}

		public bool Hidden_Attr_Is_Specified
		{
			get
			{
				return this._hidden_attr_is_specified;
			}
			set
			{
				this._hidden_attr_is_specified = value;
			}
		}

		public OoxmlBool CustomBuiltin_Attr
		{
			get
			{
				return this._customBuiltin_attr;
			}
			set
			{
				this._customBuiltin_attr = value;
				this._customBuiltin_attr_is_specified = true;
			}
		}

		public bool CustomBuiltin_Attr_Is_Specified
		{
			get
			{
				return this._customBuiltin_attr_is_specified;
			}
			set
			{
				this._customBuiltin_attr_is_specified = value;
			}
		}

		public string Name_Attr
		{
			get
			{
				return this._name_attr;
			}
			set
			{
				this._name_attr = value;
				this._name_attr_is_specified = (value != null);
			}
		}

		protected override void InitAttributes()
		{
			this._name_attr_is_specified = false;
			this._builtinId_attr_is_specified = false;
			this._iLevel_attr_is_specified = false;
			this._hidden_attr_is_specified = false;
			this._customBuiltin_attr_is_specified = false;
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
			s.Write(" xfId=\"");
			OoxmlComplexType.WriteData(s, this._xfId_attr);
			s.Write("\"");
			if (this._name_attr_is_specified)
			{
				s.Write(" name=\"");
				OoxmlComplexType.WriteData(s, this._name_attr);
				s.Write("\"");
			}
			if (this._builtinId_attr_is_specified)
			{
				s.Write(" builtinId=\"");
				OoxmlComplexType.WriteData(s, this._builtinId_attr);
				s.Write("\"");
			}
			if (this._iLevel_attr_is_specified)
			{
				s.Write(" iLevel=\"");
				OoxmlComplexType.WriteData(s, this._iLevel_attr);
				s.Write("\"");
			}
			if (this._hidden_attr_is_specified)
			{
				s.Write(" hidden=\"");
				OoxmlComplexType.WriteData(s, this._hidden_attr);
				s.Write("\"");
			}
			if (this._customBuiltin_attr_is_specified)
			{
				s.Write(" customBuiltin=\"");
				OoxmlComplexType.WriteData(s, this._customBuiltin_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
