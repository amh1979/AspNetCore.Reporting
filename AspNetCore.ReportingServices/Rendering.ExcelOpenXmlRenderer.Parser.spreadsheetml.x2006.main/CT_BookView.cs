using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_BookView : OoxmlComplexType
	{
		private ST_Visibility _visibility_attr;

		private OoxmlBool _minimized_attr;

		private OoxmlBool _showHorizontalScroll_attr;

		private OoxmlBool _showVerticalScroll_attr;

		private OoxmlBool _showSheetTabs_attr;

		private uint _tabRatio_attr;

		private uint _firstSheet_attr;

		private uint _activeTab_attr;

		private OoxmlBool _autoFilterDateGrouping_attr;

		private int _xWindow_attr;

		private bool _xWindow_attr_is_specified;

		private int _yWindow_attr;

		private bool _yWindow_attr_is_specified;

		private uint _windowWidth_attr;

		private bool _windowWidth_attr_is_specified;

		private uint _windowHeight_attr;

		private bool _windowHeight_attr_is_specified;

		public ST_Visibility Visibility_Attr
		{
			get
			{
				return this._visibility_attr;
			}
			set
			{
				this._visibility_attr = value;
			}
		}

		public OoxmlBool Minimized_Attr
		{
			get
			{
				return this._minimized_attr;
			}
			set
			{
				this._minimized_attr = value;
			}
		}

		public OoxmlBool ShowHorizontalScroll_Attr
		{
			get
			{
				return this._showHorizontalScroll_attr;
			}
			set
			{
				this._showHorizontalScroll_attr = value;
			}
		}

		public OoxmlBool ShowVerticalScroll_Attr
		{
			get
			{
				return this._showVerticalScroll_attr;
			}
			set
			{
				this._showVerticalScroll_attr = value;
			}
		}

		public OoxmlBool ShowSheetTabs_Attr
		{
			get
			{
				return this._showSheetTabs_attr;
			}
			set
			{
				this._showSheetTabs_attr = value;
			}
		}

		public uint TabRatio_Attr
		{
			get
			{
				return this._tabRatio_attr;
			}
			set
			{
				this._tabRatio_attr = value;
			}
		}

		public uint FirstSheet_Attr
		{
			get
			{
				return this._firstSheet_attr;
			}
			set
			{
				this._firstSheet_attr = value;
			}
		}

		public uint ActiveTab_Attr
		{
			get
			{
				return this._activeTab_attr;
			}
			set
			{
				this._activeTab_attr = value;
			}
		}

		public OoxmlBool AutoFilterDateGrouping_Attr
		{
			get
			{
				return this._autoFilterDateGrouping_attr;
			}
			set
			{
				this._autoFilterDateGrouping_attr = value;
			}
		}

		public int XWindow_Attr
		{
			get
			{
				return this._xWindow_attr;
			}
			set
			{
				this._xWindow_attr = value;
				this._xWindow_attr_is_specified = true;
			}
		}

		public bool XWindow_Attr_Is_Specified
		{
			get
			{
				return this._xWindow_attr_is_specified;
			}
			set
			{
				this._xWindow_attr_is_specified = value;
			}
		}

		public int YWindow_Attr
		{
			get
			{
				return this._yWindow_attr;
			}
			set
			{
				this._yWindow_attr = value;
				this._yWindow_attr_is_specified = true;
			}
		}

		public bool YWindow_Attr_Is_Specified
		{
			get
			{
				return this._yWindow_attr_is_specified;
			}
			set
			{
				this._yWindow_attr_is_specified = value;
			}
		}

		public uint WindowWidth_Attr
		{
			get
			{
				return this._windowWidth_attr;
			}
			set
			{
				this._windowWidth_attr = value;
				this._windowWidth_attr_is_specified = true;
			}
		}

		public bool WindowWidth_Attr_Is_Specified
		{
			get
			{
				return this._windowWidth_attr_is_specified;
			}
			set
			{
				this._windowWidth_attr_is_specified = value;
			}
		}

		public uint WindowHeight_Attr
		{
			get
			{
				return this._windowHeight_attr;
			}
			set
			{
				this._windowHeight_attr = value;
				this._windowHeight_attr_is_specified = true;
			}
		}

		public bool WindowHeight_Attr_Is_Specified
		{
			get
			{
				return this._windowHeight_attr_is_specified;
			}
			set
			{
				this._windowHeight_attr_is_specified = value;
			}
		}

		protected override void InitAttributes()
		{
			this._visibility_attr = ST_Visibility.visible;
			this._minimized_attr = OoxmlBool.OoxmlFalse;
			this._showHorizontalScroll_attr = OoxmlBool.OoxmlTrue;
			this._showVerticalScroll_attr = OoxmlBool.OoxmlTrue;
			this._showSheetTabs_attr = OoxmlBool.OoxmlTrue;
			this._tabRatio_attr = Convert.ToUInt32("600", CultureInfo.InvariantCulture);
			this._firstSheet_attr = Convert.ToUInt32("0", CultureInfo.InvariantCulture);
			this._activeTab_attr = Convert.ToUInt32("0", CultureInfo.InvariantCulture);
			this._autoFilterDateGrouping_attr = OoxmlBool.OoxmlTrue;
			this._xWindow_attr_is_specified = false;
			this._yWindow_attr_is_specified = false;
			this._windowWidth_attr_is_specified = false;
			this._windowHeight_attr_is_specified = false;
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
			if (this._visibility_attr != ST_Visibility.visible)
			{
				s.Write(" visibility=\"");
				OoxmlComplexType.WriteData(s, this._visibility_attr);
				s.Write("\"");
			}
			if ((bool)(this._minimized_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" minimized=\"");
				OoxmlComplexType.WriteData(s, this._minimized_attr);
				s.Write("\"");
			}
			if ((bool)(this._showHorizontalScroll_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showHorizontalScroll=\"");
				OoxmlComplexType.WriteData(s, this._showHorizontalScroll_attr);
				s.Write("\"");
			}
			if ((bool)(this._showVerticalScroll_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showVerticalScroll=\"");
				OoxmlComplexType.WriteData(s, this._showVerticalScroll_attr);
				s.Write("\"");
			}
			if ((bool)(this._showSheetTabs_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showSheetTabs=\"");
				OoxmlComplexType.WriteData(s, this._showSheetTabs_attr);
				s.Write("\"");
			}
			if (this._tabRatio_attr != Convert.ToUInt32("600", CultureInfo.InvariantCulture))
			{
				s.Write(" tabRatio=\"");
				OoxmlComplexType.WriteData(s, this._tabRatio_attr);
				s.Write("\"");
			}
			if (this._firstSheet_attr != Convert.ToUInt32("0", CultureInfo.InvariantCulture))
			{
				s.Write(" firstSheet=\"");
				OoxmlComplexType.WriteData(s, this._firstSheet_attr);
				s.Write("\"");
			}
			if (this._activeTab_attr != Convert.ToUInt32("0", CultureInfo.InvariantCulture))
			{
				s.Write(" activeTab=\"");
				OoxmlComplexType.WriteData(s, this._activeTab_attr);
				s.Write("\"");
			}
			if ((bool)(this._autoFilterDateGrouping_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" autoFilterDateGrouping=\"");
				OoxmlComplexType.WriteData(s, this._autoFilterDateGrouping_attr);
				s.Write("\"");
			}
			if (this._xWindow_attr_is_specified)
			{
				s.Write(" xWindow=\"");
				OoxmlComplexType.WriteData(s, this._xWindow_attr);
				s.Write("\"");
			}
			if (this._yWindow_attr_is_specified)
			{
				s.Write(" yWindow=\"");
				OoxmlComplexType.WriteData(s, this._yWindow_attr);
				s.Write("\"");
			}
			if (this._windowWidth_attr_is_specified)
			{
				s.Write(" windowWidth=\"");
				OoxmlComplexType.WriteData(s, this._windowWidth_attr);
				s.Write("\"");
			}
			if (this._windowHeight_attr_is_specified)
			{
				s.Write(" windowHeight=\"");
				OoxmlComplexType.WriteData(s, this._windowHeight_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
