using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_FileVersion : OoxmlComplexType
	{
		private string _appName_attr;

		private bool _appName_attr_is_specified;

		private string _lastEdited_attr;

		private bool _lastEdited_attr_is_specified;

		private string _lowestEdited_attr;

		private bool _lowestEdited_attr_is_specified;

		private string _rupBuild_attr;

		private bool _rupBuild_attr_is_specified;

		private string _codeName_attr;

		private bool _codeName_attr_is_specified;

		public string AppName_Attr
		{
			get
			{
				return this._appName_attr;
			}
			set
			{
				this._appName_attr = value;
				this._appName_attr_is_specified = (value != null);
			}
		}

		public string LastEdited_Attr
		{
			get
			{
				return this._lastEdited_attr;
			}
			set
			{
				this._lastEdited_attr = value;
				this._lastEdited_attr_is_specified = (value != null);
			}
		}

		public string LowestEdited_Attr
		{
			get
			{
				return this._lowestEdited_attr;
			}
			set
			{
				this._lowestEdited_attr = value;
				this._lowestEdited_attr_is_specified = (value != null);
			}
		}

		public string RupBuild_Attr
		{
			get
			{
				return this._rupBuild_attr;
			}
			set
			{
				this._rupBuild_attr = value;
				this._rupBuild_attr_is_specified = (value != null);
			}
		}

		public string CodeName_Attr
		{
			get
			{
				return this._codeName_attr;
			}
			set
			{
				this._codeName_attr = value;
				this._codeName_attr_is_specified = (value != null);
			}
		}

		protected override void InitAttributes()
		{
			this._appName_attr_is_specified = false;
			this._lastEdited_attr_is_specified = false;
			this._lowestEdited_attr_is_specified = false;
			this._rupBuild_attr_is_specified = false;
			this._codeName_attr_is_specified = false;
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
			if (this._appName_attr_is_specified)
			{
				s.Write(" appName=\"");
				OoxmlComplexType.WriteData(s, this._appName_attr);
				s.Write("\"");
			}
			if (this._lastEdited_attr_is_specified)
			{
				s.Write(" lastEdited=\"");
				OoxmlComplexType.WriteData(s, this._lastEdited_attr);
				s.Write("\"");
			}
			if (this._lowestEdited_attr_is_specified)
			{
				s.Write(" lowestEdited=\"");
				OoxmlComplexType.WriteData(s, this._lowestEdited_attr);
				s.Write("\"");
			}
			if (this._rupBuild_attr_is_specified)
			{
				s.Write(" rupBuild=\"");
				OoxmlComplexType.WriteData(s, this._rupBuild_attr);
				s.Write("\"");
			}
			if (this._codeName_attr_is_specified)
			{
				s.Write(" codeName=\"");
				OoxmlComplexType.WriteData(s, this._codeName_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
