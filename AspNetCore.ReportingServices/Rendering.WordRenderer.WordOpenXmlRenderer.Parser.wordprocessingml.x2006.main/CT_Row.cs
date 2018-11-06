using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Row : OoxmlComplexType, IEG_ContentRowContent, IOoxmlComplexType
	{
		private string _rsidRPr_attr;

		private string _rsidR_attr;

		private string _rsidDel_attr;

		private string _rsidTr_attr;

		private CT_TrPr _trPr;

		private List<IEG_ContentCellContent> _EG_ContentCellContents;

		public override GeneratedType GroupInterfaceType
		{
			get
			{
				return GeneratedType.CT_Row;
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

		public string RsidTr_Attr
		{
			get
			{
				return this._rsidTr_attr;
			}
			set
			{
				this._rsidTr_attr = value;
			}
		}

		public CT_TrPr TrPr
		{
			get
			{
				return this._trPr;
			}
			set
			{
				this._trPr = value;
			}
		}

		public List<IEG_ContentCellContent> EG_ContentCellContents
		{
			get
			{
				return this._EG_ContentCellContents;
			}
			set
			{
				this._EG_ContentCellContents = value;
			}
		}

		public static string TrPrElementName
		{
			get
			{
				return "trPr";
			}
		}

		public static string TcElementName
		{
			get
			{
				return "tc";
			}
		}

		public static string CustomXmlElementName
		{
			get
			{
				return "customXml";
			}
		}

		public static string SdtElementName
		{
			get
			{
				return "sdt";
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

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
			this._EG_ContentCellContents = new List<IEG_ContentCellContent>();
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
			s.Write(" w:rsidTr=\"");
			OoxmlComplexType.WriteData(s, this._rsidTr_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
			this.Write_trPr(s);
			this.Write_EG_ContentCellContents(s);
		}

		public void Write_trPr(TextWriter s)
		{
			if (this._trPr != null)
			{
				this._trPr.Write(s, "trPr");
			}
		}

		public void Write_EG_ContentCellContents(TextWriter s)
		{
			for (int i = 0; i < this._EG_ContentCellContents.Count; i++)
			{
				string tagName = null;
				switch (this._EG_ContentCellContents[i].GroupInterfaceType)
				{
				case GeneratedType.CT_Tc:
					tagName = "tc";
					break;
				case GeneratedType.CT_CustomXmlCell:
					tagName = "customXml";
					break;
				case GeneratedType.CT_SdtCell:
					tagName = "sdt";
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
				}
				this._EG_ContentCellContents[i].Write(s, tagName);
			}
		}
	}
}
