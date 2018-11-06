using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_R : OoxmlComplexType, IEG_PContent, IOoxmlComplexType
	{
		private string _rsidRPr_attr;

		private string _rsidDel_attr;

		private string _rsidR_attr;

		private CT_RPr _rPr;

		private List<IEG_RunInnerContent> _EG_RunInnerContents;

		public override GeneratedType GroupInterfaceType
		{
			get
			{
				return GeneratedType.CT_R;
			}
		}

		public string RsidRPr_Attr
		{
			get
			{
				return this._rsidRPr_attr;
			}
			set
			{
				this._rsidRPr_attr = value;
			}
		}

		public string RsidDel_Attr
		{
			get
			{
				return this._rsidDel_attr;
			}
			set
			{
				this._rsidDel_attr = value;
			}
		}

		public string RsidR_Attr
		{
			get
			{
				return this._rsidR_attr;
			}
			set
			{
				this._rsidR_attr = value;
			}
		}

		public CT_RPr RPr
		{
			get
			{
				return this._rPr;
			}
			set
			{
				this._rPr = value;
			}
		}

		public List<IEG_RunInnerContent> EG_RunInnerContents
		{
			get
			{
				return this._EG_RunInnerContents;
			}
			set
			{
				this._EG_RunInnerContents = value;
			}
		}

		public static string RPrElementName
		{
			get
			{
				return "rPr";
			}
		}

		public static string BrElementName
		{
			get
			{
				return "br";
			}
		}

		public static string TElementName
		{
			get
			{
				return "t";
			}
		}

		public static string ContentPartElementName
		{
			get
			{
				return "contentPart";
			}
		}

		public static string InstrTextElementName
		{
			get
			{
				return "instrText";
			}
		}

		public static string NoBreakHyphenElementName
		{
			get
			{
				return "noBreakHyphen";
			}
		}

		public static string SymElementName
		{
			get
			{
				return "sym";
			}
		}

		public static string ObjectElementName
		{
			get
			{
				return "object";
			}
		}

		public static string FldCharElementName
		{
			get
			{
				return "fldChar";
			}
		}

		public static string RubyElementName
		{
			get
			{
				return "ruby";
			}
		}

		public static string FootnoteReferenceElementName
		{
			get
			{
				return "footnoteReference";
			}
		}

		public static string CommentReferenceElementName
		{
			get
			{
				return "commentReference";
			}
		}

		public static string DrawingElementName
		{
			get
			{
				return "drawing";
			}
		}

		public static string PtabElementName
		{
			get
			{
				return "ptab";
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
			this._EG_RunInnerContents = new List<IEG_RunInnerContent>();
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
			s.Write(" w:rsidRPr=\"");
			OoxmlComplexType.WriteData(s, this._rsidRPr_attr);
			s.Write("\"");
			s.Write(" w:rsidDel=\"");
			OoxmlComplexType.WriteData(s, this._rsidDel_attr);
			s.Write("\"");
			s.Write(" w:rsidR=\"");
			OoxmlComplexType.WriteData(s, this._rsidR_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
			this.Write_rPr(s);
			this.Write_EG_RunInnerContents(s);
		}

		public void Write_rPr(TextWriter s)
		{
			if (this._rPr != null)
			{
				this._rPr.Write(s, "rPr");
			}
		}

		public void Write_EG_RunInnerContents(TextWriter s)
		{
			for (int i = 0; i < this._EG_RunInnerContents.Count; i++)
			{
				string tagName = null;
				switch (this._EG_RunInnerContents[i].GroupInterfaceType)
				{
				case GeneratedType.CT_Br:
					tagName = "br";
					break;
				case GeneratedType.CT_Text:
					tagName = "t";
					break;
				case GeneratedType.CT_Rel:
					tagName = "contentPart";
					break;
				case GeneratedType.CT_InstrText:
					tagName = "instrText";
					break;
				case GeneratedType.CT_Empty:
					tagName = "noBreakHyphen";
					break;
				case GeneratedType.CT_Sym:
					tagName = "sym";
					break;
				case GeneratedType.CT_Object:
					tagName = "object";
					break;
				case GeneratedType.CT_FldChar:
					tagName = "fldChar";
					break;
				case GeneratedType.CT_Ruby:
					tagName = "ruby";
					break;
				case GeneratedType.CT_FtnEdnRef:
					tagName = "footnoteReference";
					break;
				case GeneratedType.CT_Markup:
					tagName = "commentReference";
					break;
				case GeneratedType.CT_Drawing:
					tagName = "drawing";
					break;
				case GeneratedType.CT_PTab:
					tagName = "ptab";
					break;
				}
				this._EG_RunInnerContents[i].Write(s, tagName);
			}
		}
	}
}
