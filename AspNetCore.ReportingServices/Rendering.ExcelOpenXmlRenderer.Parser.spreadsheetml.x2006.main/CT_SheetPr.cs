using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_SheetPr : OoxmlComplexType
	{
		private OoxmlBool _syncHorizontal_attr;

		private OoxmlBool _syncVertical_attr;

		private OoxmlBool _transitionEvaluation_attr;

		private OoxmlBool _transitionEntry_attr;

		private OoxmlBool _published_attr;

		private OoxmlBool _filterMode_attr;

		private OoxmlBool _enableFormatConditionsCalculation_attr;

		private string _syncRef_attr;

		private bool _syncRef_attr_is_specified;

		private string _codeName_attr;

		private bool _codeName_attr_is_specified;

		private CT_Color _tabColor;

		private CT_OutlinePr _outlinePr;

		public OoxmlBool SyncHorizontal_Attr
		{
			get
			{
				return this._syncHorizontal_attr;
			}
			set
			{
				this._syncHorizontal_attr = value;
			}
		}

		public OoxmlBool SyncVertical_Attr
		{
			get
			{
				return this._syncVertical_attr;
			}
			set
			{
				this._syncVertical_attr = value;
			}
		}

		public OoxmlBool TransitionEvaluation_Attr
		{
			get
			{
				return this._transitionEvaluation_attr;
			}
			set
			{
				this._transitionEvaluation_attr = value;
			}
		}

		public OoxmlBool TransitionEntry_Attr
		{
			get
			{
				return this._transitionEntry_attr;
			}
			set
			{
				this._transitionEntry_attr = value;
			}
		}

		public OoxmlBool Published_Attr
		{
			get
			{
				return this._published_attr;
			}
			set
			{
				this._published_attr = value;
			}
		}

		public OoxmlBool FilterMode_Attr
		{
			get
			{
				return this._filterMode_attr;
			}
			set
			{
				this._filterMode_attr = value;
			}
		}

		public OoxmlBool EnableFormatConditionsCalculation_Attr
		{
			get
			{
				return this._enableFormatConditionsCalculation_attr;
			}
			set
			{
				this._enableFormatConditionsCalculation_attr = value;
			}
		}

		public string SyncRef_Attr
		{
			get
			{
				return this._syncRef_attr;
			}
			set
			{
				this._syncRef_attr = value;
				this._syncRef_attr_is_specified = (value != null);
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

		public CT_Color TabColor
		{
			get
			{
				return this._tabColor;
			}
			set
			{
				this._tabColor = value;
			}
		}

		public CT_OutlinePr OutlinePr
		{
			get
			{
				return this._outlinePr;
			}
			set
			{
				this._outlinePr = value;
			}
		}

		public static string TabColorElementName
		{
			get
			{
				return "tabColor";
			}
		}

		public static string OutlinePrElementName
		{
			get
			{
				return "outlinePr";
			}
		}

		protected override void InitAttributes()
		{
			this._syncHorizontal_attr = OoxmlBool.OoxmlFalse;
			this._syncVertical_attr = OoxmlBool.OoxmlFalse;
			this._transitionEvaluation_attr = OoxmlBool.OoxmlFalse;
			this._transitionEntry_attr = OoxmlBool.OoxmlFalse;
			this._published_attr = OoxmlBool.OoxmlTrue;
			this._filterMode_attr = OoxmlBool.OoxmlFalse;
			this._enableFormatConditionsCalculation_attr = OoxmlBool.OoxmlTrue;
			this._syncRef_attr_is_specified = false;
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
			if ((bool)(this._syncHorizontal_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" syncHorizontal=\"");
				OoxmlComplexType.WriteData(s, this._syncHorizontal_attr);
				s.Write("\"");
			}
			if ((bool)(this._syncVertical_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" syncVertical=\"");
				OoxmlComplexType.WriteData(s, this._syncVertical_attr);
				s.Write("\"");
			}
			if ((bool)(this._transitionEvaluation_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" transitionEvaluation=\"");
				OoxmlComplexType.WriteData(s, this._transitionEvaluation_attr);
				s.Write("\"");
			}
			if ((bool)(this._transitionEntry_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" transitionEntry=\"");
				OoxmlComplexType.WriteData(s, this._transitionEntry_attr);
				s.Write("\"");
			}
			if ((bool)(this._published_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" published=\"");
				OoxmlComplexType.WriteData(s, this._published_attr);
				s.Write("\"");
			}
			if ((bool)(this._filterMode_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" filterMode=\"");
				OoxmlComplexType.WriteData(s, this._filterMode_attr);
				s.Write("\"");
			}
			if ((bool)(this._enableFormatConditionsCalculation_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" enableFormatConditionsCalculation=\"");
				OoxmlComplexType.WriteData(s, this._enableFormatConditionsCalculation_attr);
				s.Write("\"");
			}
			if (this._syncRef_attr_is_specified)
			{
				s.Write(" syncRef=\"");
				OoxmlComplexType.WriteData(s, this._syncRef_attr);
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
			this.Write_tabColor(s, depth, namespaces);
			this.Write_outlinePr(s, depth, namespaces);
		}

		public void Write_tabColor(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._tabColor != null)
			{
				this._tabColor.Write(s, "tabColor", depth + 1, namespaces);
			}
		}

		public void Write_outlinePr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._outlinePr != null)
			{
				this._outlinePr.Write(s, "outlinePr", depth + 1, namespaces);
			}
		}
	}
}
