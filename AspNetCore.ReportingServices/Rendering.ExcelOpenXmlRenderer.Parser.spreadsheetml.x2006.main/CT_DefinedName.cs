using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_DefinedName : OoxmlComplexType
	{
		private string _name_attr;

		private OoxmlBool _hidden_attr;

		private OoxmlBool _function_attr;

		private OoxmlBool _vbProcedure_attr;

		private OoxmlBool _xlm_attr;

		private OoxmlBool _publishToServer_attr;

		private OoxmlBool _workbookParameter_attr;

		private string _comment_attr;

		private bool _comment_attr_is_specified;

		private string _customMenu_attr;

		private bool _customMenu_attr_is_specified;

		private string _description_attr;

		private bool _description_attr_is_specified;

		private string _help_attr;

		private bool _help_attr_is_specified;

		private string _statusBar_attr;

		private bool _statusBar_attr_is_specified;

		private uint _localSheetId_attr;

		private bool _localSheetId_attr_is_specified;

		private uint _functionGroupId_attr;

		private bool _functionGroupId_attr_is_specified;

		private string _shortcutKey_attr;

		private bool _shortcutKey_attr_is_specified;

		private string _content;

		public string Name_Attr
		{
			get
			{
				return this._name_attr;
			}
			set
			{
				this._name_attr = value;
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

		public OoxmlBool Function_Attr
		{
			get
			{
				return this._function_attr;
			}
			set
			{
				this._function_attr = value;
			}
		}

		public OoxmlBool VbProcedure_Attr
		{
			get
			{
				return this._vbProcedure_attr;
			}
			set
			{
				this._vbProcedure_attr = value;
			}
		}

		public OoxmlBool Xlm_Attr
		{
			get
			{
				return this._xlm_attr;
			}
			set
			{
				this._xlm_attr = value;
			}
		}

		public OoxmlBool PublishToServer_Attr
		{
			get
			{
				return this._publishToServer_attr;
			}
			set
			{
				this._publishToServer_attr = value;
			}
		}

		public OoxmlBool WorkbookParameter_Attr
		{
			get
			{
				return this._workbookParameter_attr;
			}
			set
			{
				this._workbookParameter_attr = value;
			}
		}

		public uint LocalSheetId_Attr
		{
			get
			{
				return this._localSheetId_attr;
			}
			set
			{
				this._localSheetId_attr = value;
				this._localSheetId_attr_is_specified = true;
			}
		}

		public bool LocalSheetId_Attr_Is_Specified
		{
			get
			{
				return this._localSheetId_attr_is_specified;
			}
			set
			{
				this._localSheetId_attr_is_specified = value;
			}
		}

		public uint FunctionGroupId_Attr
		{
			get
			{
				return this._functionGroupId_attr;
			}
			set
			{
				this._functionGroupId_attr = value;
				this._functionGroupId_attr_is_specified = true;
			}
		}

		public bool FunctionGroupId_Attr_Is_Specified
		{
			get
			{
				return this._functionGroupId_attr_is_specified;
			}
			set
			{
				this._functionGroupId_attr_is_specified = value;
			}
		}

		public string Comment_Attr
		{
			get
			{
				return this._comment_attr;
			}
			set
			{
				this._comment_attr = value;
				this._comment_attr_is_specified = (value != null);
			}
		}

		public string CustomMenu_Attr
		{
			get
			{
				return this._customMenu_attr;
			}
			set
			{
				this._customMenu_attr = value;
				this._customMenu_attr_is_specified = (value != null);
			}
		}

		public string Description_Attr
		{
			get
			{
				return this._description_attr;
			}
			set
			{
				this._description_attr = value;
				this._description_attr_is_specified = (value != null);
			}
		}

		public string Help_Attr
		{
			get
			{
				return this._help_attr;
			}
			set
			{
				this._help_attr = value;
				this._help_attr_is_specified = (value != null);
			}
		}

		public string StatusBar_Attr
		{
			get
			{
				return this._statusBar_attr;
			}
			set
			{
				this._statusBar_attr = value;
				this._statusBar_attr_is_specified = (value != null);
			}
		}

		public string ShortcutKey_Attr
		{
			get
			{
				return this._shortcutKey_attr;
			}
			set
			{
				this._shortcutKey_attr = value;
				this._shortcutKey_attr_is_specified = (value != null);
			}
		}

		public string Content
		{
			get
			{
				return this._content;
			}
			set
			{
				this._content = value;
			}
		}

		protected override void InitAttributes()
		{
			this._hidden_attr = OoxmlBool.OoxmlFalse;
			this._function_attr = OoxmlBool.OoxmlFalse;
			this._vbProcedure_attr = OoxmlBool.OoxmlFalse;
			this._xlm_attr = OoxmlBool.OoxmlFalse;
			this._publishToServer_attr = OoxmlBool.OoxmlFalse;
			this._workbookParameter_attr = OoxmlBool.OoxmlFalse;
			this._comment_attr_is_specified = false;
			this._customMenu_attr_is_specified = false;
			this._description_attr_is_specified = false;
			this._help_attr_is_specified = false;
			this._statusBar_attr_is_specified = false;
			this._localSheetId_attr_is_specified = false;
			this._functionGroupId_attr_is_specified = false;
			this._shortcutKey_attr_is_specified = false;
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
			s.Write(" name=\"");
			OoxmlComplexType.WriteData(s, this._name_attr);
			s.Write("\"");
			if ((bool)(this._hidden_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" hidden=\"");
				OoxmlComplexType.WriteData(s, this._hidden_attr);
				s.Write("\"");
			}
			if ((bool)(this._function_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" function=\"");
				OoxmlComplexType.WriteData(s, this._function_attr);
				s.Write("\"");
			}
			if ((bool)(this._vbProcedure_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" vbProcedure=\"");
				OoxmlComplexType.WriteData(s, this._vbProcedure_attr);
				s.Write("\"");
			}
			if ((bool)(this._xlm_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" xlm=\"");
				OoxmlComplexType.WriteData(s, this._xlm_attr);
				s.Write("\"");
			}
			if ((bool)(this._publishToServer_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" publishToServer=\"");
				OoxmlComplexType.WriteData(s, this._publishToServer_attr);
				s.Write("\"");
			}
			if ((bool)(this._workbookParameter_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" workbookParameter=\"");
				OoxmlComplexType.WriteData(s, this._workbookParameter_attr);
				s.Write("\"");
			}
			if (this._comment_attr_is_specified)
			{
				s.Write(" comment=\"");
				OoxmlComplexType.WriteData(s, this._comment_attr);
				s.Write("\"");
			}
			if (this._customMenu_attr_is_specified)
			{
				s.Write(" customMenu=\"");
				OoxmlComplexType.WriteData(s, this._customMenu_attr);
				s.Write("\"");
			}
			if (this._description_attr_is_specified)
			{
				s.Write(" description=\"");
				OoxmlComplexType.WriteData(s, this._description_attr);
				s.Write("\"");
			}
			if (this._help_attr_is_specified)
			{
				s.Write(" help=\"");
				OoxmlComplexType.WriteData(s, this._help_attr);
				s.Write("\"");
			}
			if (this._statusBar_attr_is_specified)
			{
				s.Write(" statusBar=\"");
				OoxmlComplexType.WriteData(s, this._statusBar_attr);
				s.Write("\"");
			}
			if (this._localSheetId_attr_is_specified)
			{
				s.Write(" localSheetId=\"");
				OoxmlComplexType.WriteData(s, this._localSheetId_attr);
				s.Write("\"");
			}
			if (this._functionGroupId_attr_is_specified)
			{
				s.Write(" functionGroupId=\"");
				OoxmlComplexType.WriteData(s, this._functionGroupId_attr);
				s.Write("\"");
			}
			if (this._shortcutKey_attr_is_specified)
			{
				s.Write(" shortcutKey=\"");
				OoxmlComplexType.WriteData(s, this._shortcutKey_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteData(s, this._content);
		}
	}
}
