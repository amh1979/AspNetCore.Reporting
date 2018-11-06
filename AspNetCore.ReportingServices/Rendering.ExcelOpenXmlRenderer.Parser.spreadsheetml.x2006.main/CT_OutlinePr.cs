using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_OutlinePr : OoxmlComplexType
	{
		private OoxmlBool _applyStyles_attr;

		private OoxmlBool _summaryBelow_attr;

		private OoxmlBool _summaryRight_attr;

		private OoxmlBool _showOutlineSymbols_attr;

		public OoxmlBool ApplyStyles_Attr
		{
			get
			{
				return this._applyStyles_attr;
			}
			set
			{
				this._applyStyles_attr = value;
			}
		}

		public OoxmlBool SummaryBelow_Attr
		{
			get
			{
				return this._summaryBelow_attr;
			}
			set
			{
				this._summaryBelow_attr = value;
			}
		}

		public OoxmlBool SummaryRight_Attr
		{
			get
			{
				return this._summaryRight_attr;
			}
			set
			{
				this._summaryRight_attr = value;
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

		protected override void InitAttributes()
		{
			this._applyStyles_attr = OoxmlBool.OoxmlFalse;
			this._summaryBelow_attr = OoxmlBool.OoxmlTrue;
			this._summaryRight_attr = OoxmlBool.OoxmlTrue;
			this._showOutlineSymbols_attr = OoxmlBool.OoxmlTrue;
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
			if ((bool)(this._applyStyles_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" applyStyles=\"");
				OoxmlComplexType.WriteData(s, this._applyStyles_attr);
				s.Write("\"");
			}
			if ((bool)(this._summaryBelow_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" summaryBelow=\"");
				OoxmlComplexType.WriteData(s, this._summaryBelow_attr);
				s.Write("\"");
			}
			if ((bool)(this._summaryRight_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" summaryRight=\"");
				OoxmlComplexType.WriteData(s, this._summaryRight_attr);
				s.Write("\"");
			}
			if ((bool)(this._showOutlineSymbols_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showOutlineSymbols=\"");
				OoxmlComplexType.WriteData(s, this._showOutlineSymbols_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
