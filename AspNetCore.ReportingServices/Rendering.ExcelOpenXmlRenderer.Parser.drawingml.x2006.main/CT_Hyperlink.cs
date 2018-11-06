using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class CT_Hyperlink : OoxmlComplexType
	{
		private string _invalidUrl_attr;

		private string _action_attr;

		private string _tgtFrame_attr;

		private string _tooltip_attr;

		private OoxmlBool _history_attr;

		private OoxmlBool _highlightClick_attr;

		private OoxmlBool _endSnd_attr;

		private string _id_attr;

		private bool _id_attr_is_specified;

		public string InvalidUrl_Attr
		{
			get
			{
				return this._invalidUrl_attr;
			}
			set
			{
				this._invalidUrl_attr = value;
			}
		}

		public string Action_Attr
		{
			get
			{
				return this._action_attr;
			}
			set
			{
				this._action_attr = value;
			}
		}

		public string TgtFrame_Attr
		{
			get
			{
				return this._tgtFrame_attr;
			}
			set
			{
				this._tgtFrame_attr = value;
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
			}
		}

		public OoxmlBool History_Attr
		{
			get
			{
				return this._history_attr;
			}
			set
			{
				this._history_attr = value;
			}
		}

		public OoxmlBool HighlightClick_Attr
		{
			get
			{
				return this._highlightClick_attr;
			}
			set
			{
				this._highlightClick_attr = value;
			}
		}

		public OoxmlBool EndSnd_Attr
		{
			get
			{
				return this._endSnd_attr;
			}
			set
			{
				this._endSnd_attr = value;
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

		protected override void InitAttributes()
		{
			this._invalidUrl_attr = Convert.ToString("", CultureInfo.InvariantCulture);
			this._action_attr = Convert.ToString("", CultureInfo.InvariantCulture);
			this._tgtFrame_attr = Convert.ToString("", CultureInfo.InvariantCulture);
			this._tooltip_attr = Convert.ToString("", CultureInfo.InvariantCulture);
			this._history_attr = OoxmlBool.OoxmlTrue;
			this._highlightClick_attr = OoxmlBool.OoxmlFalse;
			this._endSnd_attr = OoxmlBool.OoxmlFalse;
			this._id_attr_is_specified = false;
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/main");
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/main");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			if (this._invalidUrl_attr != Convert.ToString("", CultureInfo.InvariantCulture))
			{
				s.Write(" invalidUrl=\"");
				OoxmlComplexType.WriteData(s, this._invalidUrl_attr);
				s.Write("\"");
			}
			if (this._action_attr != Convert.ToString("", CultureInfo.InvariantCulture))
			{
				s.Write(" action=\"");
				OoxmlComplexType.WriteData(s, this._action_attr);
				s.Write("\"");
			}
			if (this._tgtFrame_attr != Convert.ToString("", CultureInfo.InvariantCulture))
			{
				s.Write(" tgtFrame=\"");
				OoxmlComplexType.WriteData(s, this._tgtFrame_attr);
				s.Write("\"");
			}
			if (this._tooltip_attr != Convert.ToString("", CultureInfo.InvariantCulture))
			{
				s.Write(" tooltip=\"");
				OoxmlComplexType.WriteData(s, this._tooltip_attr);
				s.Write("\"");
			}
			if ((bool)(this._history_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" history=\"");
				OoxmlComplexType.WriteData(s, this._history_attr);
				s.Write("\"");
			}
			if ((bool)(this._highlightClick_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" highlightClick=\"");
				OoxmlComplexType.WriteData(s, this._highlightClick_attr);
				s.Write("\"");
			}
			if ((bool)(this._endSnd_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" endSnd=\"");
				OoxmlComplexType.WriteData(s, this._endSnd_attr);
				s.Write("\"");
			}
			if (this._id_attr_is_specified)
			{
				s.Write(" r:id=\"");
				OoxmlComplexType.WriteData(s, this._id_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
