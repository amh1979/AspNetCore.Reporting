using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_AbstractNum : OoxmlComplexType, IOoxmlComplexType
	{
		private int _abstractNumId_attr;

		private CT_LongHexNumber _nsid;

		private CT_MultiLevelType _multiLevelType;

		private CT_LongHexNumber _tmpl;

		private CT_String _name;

		private CT_String _styleLink;

		private CT_String _numStyleLink;

		private List<CT_Lvl> _lvl;

		public int AbstractNumId_Attr
		{
			get
			{
				return this._abstractNumId_attr;
			}
			set
			{
				this._abstractNumId_attr = value;
			}
		}

		public CT_LongHexNumber Nsid
		{
			get
			{
				return this._nsid;
			}
			set
			{
				this._nsid = value;
			}
		}

		public CT_MultiLevelType MultiLevelType
		{
			get
			{
				return this._multiLevelType;
			}
			set
			{
				this._multiLevelType = value;
			}
		}

		public CT_LongHexNumber Tmpl
		{
			get
			{
				return this._tmpl;
			}
			set
			{
				this._tmpl = value;
			}
		}

		public CT_String Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		public CT_String StyleLink
		{
			get
			{
				return this._styleLink;
			}
			set
			{
				this._styleLink = value;
			}
		}

		public CT_String NumStyleLink
		{
			get
			{
				return this._numStyleLink;
			}
			set
			{
				this._numStyleLink = value;
			}
		}

		public List<CT_Lvl> Lvl
		{
			get
			{
				return this._lvl;
			}
			set
			{
				this._lvl = value;
			}
		}

		public static string NsidElementName
		{
			get
			{
				return "nsid";
			}
		}

		public static string MultiLevelTypeElementName
		{
			get
			{
				return "multiLevelType";
			}
		}

		public static string TmplElementName
		{
			get
			{
				return "tmpl";
			}
		}

		public static string NameElementName
		{
			get
			{
				return "name";
			}
		}

		public static string StyleLinkElementName
		{
			get
			{
				return "styleLink";
			}
		}

		public static string NumStyleLinkElementName
		{
			get
			{
				return "numStyleLink";
			}
		}

		public static string LvlElementName
		{
			get
			{
				return "lvl";
			}
		}

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
			this._lvl = new List<CT_Lvl>();
		}

		public override void Write(TextWriter s, string tagName)
		{
			this.WriteOpenTag(s, tagName, null);
			this.WriteElements(s);
			this.WriteCloseTag(s, tagName);
		}

		public override void WriteOpenTag(TextWriter s, string tagName, Dictionary<string, string> namespaces)
		{
			base.WriteOpenTag(s, tagName, "w", namespaces);
		}

		public override void WriteCloseTag(TextWriter s, string tagName)
		{
			s.Write("</w:");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			s.Write(" w:abstractNumId=\"");
			OoxmlComplexType.WriteData(s, this._abstractNumId_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
			this.Write_nsid(s);
			this.Write_multiLevelType(s);
			this.Write_tmpl(s);
			this.Write_name(s);
			this.Write_styleLink(s);
			this.Write_numStyleLink(s);
			this.Write_lvl(s);
		}

		public void Write_nsid(TextWriter s)
		{
			if (this._nsid != null)
			{
				this._nsid.Write(s, "nsid");
			}
		}

		public void Write_multiLevelType(TextWriter s)
		{
			if (this._multiLevelType != null)
			{
				this._multiLevelType.Write(s, "multiLevelType");
			}
		}

		public void Write_tmpl(TextWriter s)
		{
			if (this._tmpl != null)
			{
				this._tmpl.Write(s, "tmpl");
			}
		}

		public void Write_name(TextWriter s)
		{
			if (this._name != null)
			{
				this._name.Write(s, "name");
			}
		}

		public void Write_styleLink(TextWriter s)
		{
			if (this._styleLink != null)
			{
				this._styleLink.Write(s, "styleLink");
			}
		}

		public void Write_numStyleLink(TextWriter s)
		{
			if (this._numStyleLink != null)
			{
				this._numStyleLink.Write(s, "numStyleLink");
			}
		}

		public void Write_lvl(TextWriter s)
		{
			if (this._lvl != null)
			{
				foreach (CT_Lvl item in this._lvl)
				{
					if (item != null)
					{
						item.Write(s, "lvl");
					}
				}
			}
		}
	}
}
