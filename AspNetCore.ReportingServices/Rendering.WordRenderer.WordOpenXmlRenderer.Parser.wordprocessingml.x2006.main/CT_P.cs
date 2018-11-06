using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_P : OoxmlComplexType, IEG_BlockLevelElts, IOoxmlComplexType
	{
		private string _rsidRPr_attr;

		private string _rsidR_attr;

		private string _rsidDel_attr;

		private string _rsidP_attr;

		private string _rsidRDefault_attr;

		private CT_PPr _pPr;

		private List<IEG_PContent> _EG_PContents;

		public override GeneratedType GroupInterfaceType
		{
			get
			{
				return GeneratedType.CT_P;
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

		public string RsidP_Attr
		{
			get
			{
				return this._rsidP_attr;
			}
			set
			{
				this._rsidP_attr = value;
			}
		}

		public string RsidRDefault_Attr
		{
			get
			{
				return this._rsidRDefault_attr;
			}
			set
			{
				this._rsidRDefault_attr = value;
			}
		}

		public CT_PPr PPr
		{
			get
			{
				return this._pPr;
			}
			set
			{
				this._pPr = value;
			}
		}

		public List<IEG_PContent> EG_PContents
		{
			get
			{
				return this._EG_PContents;
			}
			set
			{
				this._EG_PContents = value;
			}
		}

		public static string PPrElementName
		{
			get
			{
				return "pPr";
			}
		}

		public static string CustomXmlElementName
		{
			get
			{
				return "customXml";
			}
		}

		public static string SmartTagElementName
		{
			get
			{
				return "smartTag";
			}
		}

		public static string SdtElementName
		{
			get
			{
				return "sdt";
			}
		}

		public static string DirElementName
		{
			get
			{
				return "dir";
			}
		}

		public static string BdoElementName
		{
			get
			{
				return "bdo";
			}
		}

		public static string RElementName
		{
			get
			{
				return "r";
			}
		}

		public static string ProofErrElementName
		{
			get
			{
				return "proofErr";
			}
		}

		public static string PermStartElementName
		{
			get
			{
				return "permStart";
			}
		}

		public static string PermEndElementName
		{
			get
			{
				return "permEnd";
			}
		}

		public static string BookmarkStartElementName
		{
			get
			{
				return "bookmarkStart";
			}
		}

		public static string BookmarkEndElementName
		{
			get
			{
				return "bookmarkEnd";
			}
		}

		public static string MoveFromRangeStartElementName
		{
			get
			{
				return "moveFromRangeStart";
			}
		}

		public static string CustomXmlInsRangeStartElementName
		{
			get
			{
				return "customXmlInsRangeStart";
			}
		}

		public static string CustomXmlInsRangeEndElementName
		{
			get
			{
				return "customXmlInsRangeEnd";
			}
		}

		public static string InsElementName
		{
			get
			{
				return "ins";
			}
		}

		public static string FldSimpleElementName
		{
			get
			{
				return "fldSimple";
			}
		}

		public static string HyperlinkElementName
		{
			get
			{
				return "hyperlink";
			}
		}

		public static string SubDocElementName
		{
			get
			{
				return "subDoc";
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
			this._EG_PContents = new List<IEG_PContent>();
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
			s.Write(" w:rsidR=\"");
			OoxmlComplexType.WriteData(s, this._rsidR_attr);
			s.Write("\"");
			s.Write(" w:rsidDel=\"");
			OoxmlComplexType.WriteData(s, this._rsidDel_attr);
			s.Write("\"");
			s.Write(" w:rsidP=\"");
			OoxmlComplexType.WriteData(s, this._rsidP_attr);
			s.Write("\"");
			s.Write(" w:rsidRDefault=\"");
			OoxmlComplexType.WriteData(s, this._rsidRDefault_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
			this.Write_pPr(s);
			this.Write_EG_PContents(s);
		}

		public void Write_pPr(TextWriter s)
		{
			if (this._pPr != null)
			{
				this._pPr.Write(s, "pPr");
			}
		}

		public void Write_EG_PContents(TextWriter s)
		{
			for (int i = 0; i < this._EG_PContents.Count; i++)
			{
				string tagName = null;
				switch (this._EG_PContents[i].GroupInterfaceType)
				{
				case GeneratedType.CT_CustomXmlRun:
					tagName = "customXml";
					break;
				case GeneratedType.CT_SmartTagRun:
					tagName = "smartTag";
					break;
				case GeneratedType.CT_SdtRun:
					tagName = "sdt";
					break;
				case GeneratedType.CT_DirContentRun:
					tagName = "dir";
					break;
				case GeneratedType.CT_BdoContentRun:
					tagName = "bdo";
					break;
				case GeneratedType.CT_R:
					tagName = "r";
					break;
				case GeneratedType.CT_ProofErr:
					tagName = "proofErr";
					break;
				case GeneratedType.CT_PermStart:
					tagName = "permStart";
					break;
				case GeneratedType.CT_Perm:
					tagName = "permEnd";
					break;
				case GeneratedType.CT_Bookmark:
					tagName = "bookmarkStart";
					break;
				case GeneratedType.CT_MarkupRange:
					tagName = "bookmarkEnd";
					break;
				case GeneratedType.CT_MoveBookmark:
					tagName = "moveFromRangeStart";
					break;
				case GeneratedType.CT_TrackChange:
					tagName = "customXmlInsRangeStart";
					break;
				case GeneratedType.CT_Markup:
					tagName = "customXmlInsRangeEnd";
					break;
				case GeneratedType.CT_RunTrackChange:
					tagName = "ins";
					break;
				case GeneratedType.CT_SimpleField:
					tagName = "fldSimple";
					break;
				case GeneratedType.CT_Hyperlink:
					tagName = "hyperlink";
					break;
				case GeneratedType.CT_Rel:
					tagName = "subDoc";
					break;
				}
				this._EG_PContents[i].Write(s, tagName);
			}
		}
	}
}
