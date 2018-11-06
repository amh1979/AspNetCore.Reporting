using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_SheetView : OoxmlComplexType
	{
		private OoxmlBool _windowProtection_attr;

		private OoxmlBool _showFormulas_attr;

		private OoxmlBool _showGridLines_attr;

		private OoxmlBool _showRowColHeaders_attr;

		private OoxmlBool _showZeros_attr;

		private OoxmlBool _rightToLeft_attr;

		private OoxmlBool _tabSelected_attr;

		private OoxmlBool _showRuler_attr;

		private OoxmlBool _showOutlineSymbols_attr;

		private OoxmlBool _defaultGridColor_attr;

		private OoxmlBool _showWhiteSpace_attr;

		private ST_SheetViewType _view_attr;

		private uint _colorId_attr;

		private uint _zoomScale_attr;

		private uint _zoomScaleNormal_attr;

		private uint _zoomScaleSheetLayoutView_attr;

		private uint _zoomScalePageLayoutView_attr;

		private uint _workbookViewId_attr;

		private string _topLeftCell_attr;

		private bool _topLeftCell_attr_is_specified;

		private CT_Pane _pane;

		public OoxmlBool WindowProtection_Attr
		{
			get
			{
				return this._windowProtection_attr;
			}
			set
			{
				this._windowProtection_attr = value;
			}
		}

		public OoxmlBool ShowFormulas_Attr
		{
			get
			{
				return this._showFormulas_attr;
			}
			set
			{
				this._showFormulas_attr = value;
			}
		}

		public OoxmlBool ShowGridLines_Attr
		{
			get
			{
				return this._showGridLines_attr;
			}
			set
			{
				this._showGridLines_attr = value;
			}
		}

		public OoxmlBool ShowRowColHeaders_Attr
		{
			get
			{
				return this._showRowColHeaders_attr;
			}
			set
			{
				this._showRowColHeaders_attr = value;
			}
		}

		public OoxmlBool ShowZeros_Attr
		{
			get
			{
				return this._showZeros_attr;
			}
			set
			{
				this._showZeros_attr = value;
			}
		}

		public OoxmlBool RightToLeft_Attr
		{
			get
			{
				return this._rightToLeft_attr;
			}
			set
			{
				this._rightToLeft_attr = value;
			}
		}

		public OoxmlBool TabSelected_Attr
		{
			get
			{
				return this._tabSelected_attr;
			}
			set
			{
				this._tabSelected_attr = value;
			}
		}

		public OoxmlBool ShowRuler_Attr
		{
			get
			{
				return this._showRuler_attr;
			}
			set
			{
				this._showRuler_attr = value;
			}
		}

		public OoxmlBool ShowOutlineSymbols_Attr
		{
			get
			{
				return this._showOutlineSymbols_attr;
			}
			set
			{
				this._showOutlineSymbols_attr = value;
			}
		}

		public OoxmlBool DefaultGridColor_Attr
		{
			get
			{
				return this._defaultGridColor_attr;
			}
			set
			{
				this._defaultGridColor_attr = value;
			}
		}

		public OoxmlBool ShowWhiteSpace_Attr
		{
			get
			{
				return this._showWhiteSpace_attr;
			}
			set
			{
				this._showWhiteSpace_attr = value;
			}
		}

		public ST_SheetViewType View_Attr
		{
			get
			{
				return this._view_attr;
			}
			set
			{
				this._view_attr = value;
			}
		}

		public uint ColorId_Attr
		{
			get
			{
				return this._colorId_attr;
			}
			set
			{
				this._colorId_attr = value;
			}
		}

		public uint ZoomScale_Attr
		{
			get
			{
				return this._zoomScale_attr;
			}
			set
			{
				this._zoomScale_attr = value;
			}
		}

		public uint ZoomScaleNormal_Attr
		{
			get
			{
				return this._zoomScaleNormal_attr;
			}
			set
			{
				this._zoomScaleNormal_attr = value;
			}
		}

		public uint ZoomScaleSheetLayoutView_Attr
		{
			get
			{
				return this._zoomScaleSheetLayoutView_attr;
			}
			set
			{
				this._zoomScaleSheetLayoutView_attr = value;
			}
		}

		public uint ZoomScalePageLayoutView_Attr
		{
			get
			{
				return this._zoomScalePageLayoutView_attr;
			}
			set
			{
				this._zoomScalePageLayoutView_attr = value;
			}
		}

		public uint WorkbookViewId_Attr
		{
			get
			{
				return this._workbookViewId_attr;
			}
			set
			{
				this._workbookViewId_attr = value;
			}
		}

		public string TopLeftCell_Attr
		{
			get
			{
				return this._topLeftCell_attr;
			}
			set
			{
				this._topLeftCell_attr = value;
				this._topLeftCell_attr_is_specified = (value != null);
			}
		}

		public CT_Pane Pane
		{
			get
			{
				return this._pane;
			}
			set
			{
				this._pane = value;
			}
		}

		public static string PaneElementName
		{
			get
			{
				return "pane";
			}
		}

		protected override void InitAttributes()
		{
			this._windowProtection_attr = OoxmlBool.OoxmlFalse;
			this._showFormulas_attr = OoxmlBool.OoxmlFalse;
			this._showGridLines_attr = OoxmlBool.OoxmlTrue;
			this._showRowColHeaders_attr = OoxmlBool.OoxmlTrue;
			this._showZeros_attr = OoxmlBool.OoxmlTrue;
			this._rightToLeft_attr = OoxmlBool.OoxmlFalse;
			this._tabSelected_attr = OoxmlBool.OoxmlFalse;
			this._showRuler_attr = OoxmlBool.OoxmlTrue;
			this._showOutlineSymbols_attr = OoxmlBool.OoxmlTrue;
			this._defaultGridColor_attr = OoxmlBool.OoxmlTrue;
			this._showWhiteSpace_attr = OoxmlBool.OoxmlTrue;
			this._view_attr = ST_SheetViewType.normal;
			this._colorId_attr = Convert.ToUInt32("64", CultureInfo.InvariantCulture);
			this._zoomScale_attr = Convert.ToUInt32("100", CultureInfo.InvariantCulture);
			this._zoomScaleNormal_attr = Convert.ToUInt32("0", CultureInfo.InvariantCulture);
			this._zoomScaleSheetLayoutView_attr = Convert.ToUInt32("0", CultureInfo.InvariantCulture);
			this._zoomScalePageLayoutView_attr = Convert.ToUInt32("0", CultureInfo.InvariantCulture);
			this._topLeftCell_attr_is_specified = false;
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
			s.Write(" workbookViewId=\"");
			OoxmlComplexType.WriteData(s, this._workbookViewId_attr);
			s.Write("\"");
			if ((bool)(this._windowProtection_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" windowProtection=\"");
				OoxmlComplexType.WriteData(s, this._windowProtection_attr);
				s.Write("\"");
			}
			if ((bool)(this._showFormulas_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" showFormulas=\"");
				OoxmlComplexType.WriteData(s, this._showFormulas_attr);
				s.Write("\"");
			}
			if ((bool)(this._showGridLines_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showGridLines=\"");
				OoxmlComplexType.WriteData(s, this._showGridLines_attr);
				s.Write("\"");
			}
			if ((bool)(this._showRowColHeaders_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showRowColHeaders=\"");
				OoxmlComplexType.WriteData(s, this._showRowColHeaders_attr);
				s.Write("\"");
			}
			if ((bool)(this._showZeros_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showZeros=\"");
				OoxmlComplexType.WriteData(s, this._showZeros_attr);
				s.Write("\"");
			}
			if ((bool)(this._rightToLeft_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" rightToLeft=\"");
				OoxmlComplexType.WriteData(s, this._rightToLeft_attr);
				s.Write("\"");
			}
			if ((bool)(this._tabSelected_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" tabSelected=\"");
				OoxmlComplexType.WriteData(s, this._tabSelected_attr);
				s.Write("\"");
			}
			if ((bool)(this._showRuler_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showRuler=\"");
				OoxmlComplexType.WriteData(s, this._showRuler_attr);
				s.Write("\"");
			}
			if ((bool)(this._showOutlineSymbols_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showOutlineSymbols=\"");
				OoxmlComplexType.WriteData(s, this._showOutlineSymbols_attr);
				s.Write("\"");
			}
			if ((bool)(this._defaultGridColor_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" defaultGridColor=\"");
				OoxmlComplexType.WriteData(s, this._defaultGridColor_attr);
				s.Write("\"");
			}
			if ((bool)(this._showWhiteSpace_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showWhiteSpace=\"");
				OoxmlComplexType.WriteData(s, this._showWhiteSpace_attr);
				s.Write("\"");
			}
			if (this._view_attr != ST_SheetViewType.normal)
			{
				s.Write(" view=\"");
				OoxmlComplexType.WriteData(s, this._view_attr);
				s.Write("\"");
			}
			if (this._colorId_attr != Convert.ToUInt32("64", CultureInfo.InvariantCulture))
			{
				s.Write(" colorId=\"");
				OoxmlComplexType.WriteData(s, this._colorId_attr);
				s.Write("\"");
			}
			if (this._zoomScale_attr != Convert.ToUInt32("100", CultureInfo.InvariantCulture))
			{
				s.Write(" zoomScale=\"");
				OoxmlComplexType.WriteData(s, this._zoomScale_attr);
				s.Write("\"");
			}
			if (this._zoomScaleNormal_attr != Convert.ToUInt32("0", CultureInfo.InvariantCulture))
			{
				s.Write(" zoomScaleNormal=\"");
				OoxmlComplexType.WriteData(s, this._zoomScaleNormal_attr);
				s.Write("\"");
			}
			if (this._zoomScaleSheetLayoutView_attr != Convert.ToUInt32("0", CultureInfo.InvariantCulture))
			{
				s.Write(" zoomScaleSheetLayoutView=\"");
				OoxmlComplexType.WriteData(s, this._zoomScaleSheetLayoutView_attr);
				s.Write("\"");
			}
			if (this._zoomScalePageLayoutView_attr != Convert.ToUInt32("0", CultureInfo.InvariantCulture))
			{
				s.Write(" zoomScalePageLayoutView=\"");
				OoxmlComplexType.WriteData(s, this._zoomScalePageLayoutView_attr);
				s.Write("\"");
			}
			if (this._topLeftCell_attr_is_specified)
			{
				s.Write(" topLeftCell=\"");
				OoxmlComplexType.WriteData(s, this._topLeftCell_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			this.Write_pane(s, depth, namespaces);
		}

		public void Write_pane(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._pane != null)
			{
				this._pane.Write(s, "pane", depth + 1, namespaces);
			}
		}
	}
}
