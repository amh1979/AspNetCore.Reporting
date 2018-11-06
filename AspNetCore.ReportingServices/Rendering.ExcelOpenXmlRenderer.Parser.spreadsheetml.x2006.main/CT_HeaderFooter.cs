using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_HeaderFooter : OoxmlComplexType
	{
		private OoxmlBool _differentOddEven_attr;

		private OoxmlBool _differentFirst_attr;

		private OoxmlBool _scaleWithDoc_attr;

		private OoxmlBool _alignWithMargins_attr;

		private string _oddHeader;

		private string _oddFooter;

		private string _evenHeader;

		private string _evenFooter;

		private string _firstHeader;

		private string _firstFooter;

		public OoxmlBool DifferentOddEven_Attr
		{
			get
			{
				return this._differentOddEven_attr;
			}
			set
			{
				this._differentOddEven_attr = value;
			}
		}

		public OoxmlBool DifferentFirst_Attr
		{
			get
			{
				return this._differentFirst_attr;
			}
			set
			{
				this._differentFirst_attr = value;
			}
		}

		public OoxmlBool ScaleWithDoc_Attr
		{
			get
			{
				return this._scaleWithDoc_attr;
			}
			set
			{
				this._scaleWithDoc_attr = value;
			}
		}

		public OoxmlBool AlignWithMargins_Attr
		{
			get
			{
				return this._alignWithMargins_attr;
			}
			set
			{
				this._alignWithMargins_attr = value;
			}
		}

		public string OddHeader
		{
			get
			{
				return this._oddHeader;
			}
			set
			{
				this._oddHeader = value;
			}
		}

		public string OddFooter
		{
			get
			{
				return this._oddFooter;
			}
			set
			{
				this._oddFooter = value;
			}
		}

		public string EvenHeader
		{
			get
			{
				return this._evenHeader;
			}
			set
			{
				this._evenHeader = value;
			}
		}

		public string EvenFooter
		{
			get
			{
				return this._evenFooter;
			}
			set
			{
				this._evenFooter = value;
			}
		}

		public string FirstHeader
		{
			get
			{
				return this._firstHeader;
			}
			set
			{
				this._firstHeader = value;
			}
		}

		public string FirstFooter
		{
			get
			{
				return this._firstFooter;
			}
			set
			{
				this._firstFooter = value;
			}
		}

		public static string OddHeaderElementName
		{
			get
			{
				return "oddHeader";
			}
		}

		public static string OddFooterElementName
		{
			get
			{
				return "oddFooter";
			}
		}

		public static string EvenHeaderElementName
		{
			get
			{
				return "evenHeader";
			}
		}

		public static string EvenFooterElementName
		{
			get
			{
				return "evenFooter";
			}
		}

		public static string FirstHeaderElementName
		{
			get
			{
				return "firstHeader";
			}
		}

		public static string FirstFooterElementName
		{
			get
			{
				return "firstFooter";
			}
		}

		protected override void InitAttributes()
		{
			this._differentOddEven_attr = OoxmlBool.OoxmlFalse;
			this._differentFirst_attr = OoxmlBool.OoxmlFalse;
			this._scaleWithDoc_attr = OoxmlBool.OoxmlTrue;
			this._alignWithMargins_attr = OoxmlBool.OoxmlTrue;
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
			if ((bool)(this._differentOddEven_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" differentOddEven=\"");
				OoxmlComplexType.WriteData(s, this._differentOddEven_attr);
				s.Write("\"");
			}
			if ((bool)(this._differentFirst_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" differentFirst=\"");
				OoxmlComplexType.WriteData(s, this._differentFirst_attr);
				s.Write("\"");
			}
			if ((bool)(this._scaleWithDoc_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" scaleWithDoc=\"");
				OoxmlComplexType.WriteData(s, this._scaleWithDoc_attr);
				s.Write("\"");
			}
			if ((bool)(this._alignWithMargins_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" alignWithMargins=\"");
				OoxmlComplexType.WriteData(s, this._alignWithMargins_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			this.Write_oddHeader(s, depth, namespaces);
			this.Write_oddFooter(s, depth, namespaces);
			this.Write_evenHeader(s, depth, namespaces);
			this.Write_evenFooter(s, depth, namespaces);
			this.Write_firstHeader(s, depth, namespaces);
			this.Write_firstFooter(s, depth, namespaces);
		}

		public void Write_oddHeader(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._oddHeader != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "oddHeader", "http://schemas.openxmlformats.org/spreadsheetml/2006/main", this._oddHeader);
			}
		}

		public void Write_oddFooter(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._oddFooter != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "oddFooter", "http://schemas.openxmlformats.org/spreadsheetml/2006/main", this._oddFooter);
			}
		}

		public void Write_evenHeader(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._evenHeader != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "evenHeader", "http://schemas.openxmlformats.org/spreadsheetml/2006/main", this._evenHeader);
			}
		}

		public void Write_evenFooter(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._evenFooter != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "evenFooter", "http://schemas.openxmlformats.org/spreadsheetml/2006/main", this._evenFooter);
			}
		}

		public void Write_firstHeader(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._firstHeader != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "firstHeader", "http://schemas.openxmlformats.org/spreadsheetml/2006/main", this._firstHeader);
			}
		}

		public void Write_firstFooter(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._firstFooter != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "firstFooter", "http://schemas.openxmlformats.org/spreadsheetml/2006/main", this._firstFooter);
			}
		}
	}
}
