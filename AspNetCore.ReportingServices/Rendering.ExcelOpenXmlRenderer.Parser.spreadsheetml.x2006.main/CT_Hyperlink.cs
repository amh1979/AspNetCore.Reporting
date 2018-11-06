using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Hyperlink : OoxmlComplexType
	{
		private string __ref_attr;

		private string _id_attr;

		private bool _id_attr_is_specified;

		private string _location_attr;

		private bool _location_attr_is_specified;

		private string _tooltip_attr;

		private bool _tooltip_attr_is_specified;

		private string _display_attr;

		private bool _display_attr_is_specified;

		public string _ref_Attr
		{
			get
			{
				return this.__ref_attr;
			}
			set
			{
				this.__ref_attr = value;
			}
		}

		public string Id_Attr
		{
			get
			{
				return this._id_attr;
			}
			set
			{
				this._id_attr = value;
				this._id_attr_is_specified = (value != null);
			}
		}

		public string Location_Attr
		{
			get
			{
				return this._location_attr;
			}
			set
			{
				this._location_attr = value;
				this._location_attr_is_specified = (value != null);
			}
		}

		public string Tooltip_Attr
		{
			get
			{
				return this._tooltip_attr;
			}
			set
			{
				this._tooltip_attr = value;
				this._tooltip_attr_is_specified = (value != null);
			}
		}

		public string Display_Attr
		{
			get
			{
				return this._display_attr;
			}
			set
			{
				this._display_attr = value;
				this._display_attr_is_specified = (value != null);
			}
		}

		protected override void InitAttributes()
		{
			this._id_attr_is_specified = false;
			this._location_attr_is_specified = false;
			this._tooltip_attr_is_specified = false;
			this._display_attr_is_specified = false;
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
			s.Write(" ref=\"");
			OoxmlComplexType.WriteData(s, this.__ref_attr);
			s.Write("\"");
			if (this._id_attr_is_specified)
			{
				s.Write(" r:id=\"");
				OoxmlComplexType.WriteData(s, this._id_attr);
				s.Write("\"");
			}
			if (this._location_attr_is_specified)
			{
				s.Write(" location=\"");
				OoxmlComplexType.WriteData(s, this._location_attr);
				s.Write("\"");
			}
			if (this._tooltip_attr_is_specified)
			{
				s.Write(" tooltip=\"");
				OoxmlComplexType.WriteData(s, this._tooltip_attr);
				s.Write("\"");
			}
			if (this._display_attr_is_specified)
			{
				s.Write(" display=\"");
				OoxmlComplexType.WriteData(s, this._display_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
