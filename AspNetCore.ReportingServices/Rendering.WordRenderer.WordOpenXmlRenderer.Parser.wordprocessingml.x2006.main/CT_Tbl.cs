using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Tbl : OoxmlComplexType, IEG_BlockLevelElts, IOoxmlComplexType
	{
		private CT_TblPr _tblPr;

		private CT_TblGrid _tblGrid;

		private List<IEG_RangeMarkupElements> _EG_RangeMarkupElementss;

		private List<IEG_ContentRowContent> _EG_ContentRowContents;

		public override GeneratedType GroupInterfaceType
		{
			get
			{
				return GeneratedType.CT_Tbl;
			}
		}

		public CT_TblPr TblPr
		{
			get
			{
				return this._tblPr;
			}
			set
			{
				this._tblPr = value;
			}
		}

		public CT_TblGrid TblGrid
		{
			get
			{
				return this._tblGrid;
			}
			set
			{
				this._tblGrid = value;
			}
		}

		public List<IEG_RangeMarkupElements> EG_RangeMarkupElementss
		{
			get
			{
				return this._EG_RangeMarkupElementss;
			}
			set
			{
				this._EG_RangeMarkupElementss = value;
			}
		}

		public List<IEG_ContentRowContent> EG_ContentRowContents
		{
			get
			{
				return this._EG_ContentRowContents;
			}
			set
			{
				this._EG_ContentRowContents = value;
			}
		}

		public static string TblPrElementName
		{
			get
			{
				return "tblPr";
			}
		}

		public static string TblGridElementName
		{
			get
			{
				return "tblGrid";
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

		public static string TrElementName
		{
			get
			{
				return "tr";
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
			this._tblPr = new CT_TblPr();
			this._tblGrid = new CT_TblGrid();
		}

		protected override void InitCollections()
		{
			this._EG_RangeMarkupElementss = new List<IEG_RangeMarkupElements>();
			this._EG_ContentRowContents = new List<IEG_ContentRowContent>();
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
		}

		public override void WriteElements(TextWriter s)
		{
			this.Write_EG_RangeMarkupElementss(s);
			this.Write_tblPr(s);
			this.Write_tblGrid(s);
			this.Write_EG_ContentRowContents(s);
		}

		public void Write_tblPr(TextWriter s)
		{
			if (this._tblPr != null)
			{
				this._tblPr.Write(s, "tblPr");
			}
		}

		public void Write_tblGrid(TextWriter s)
		{
			if (this._tblGrid != null)
			{
				this._tblGrid.Write(s, "tblGrid");
			}
		}

		public void Write_EG_RangeMarkupElementss(TextWriter s)
		{
			for (int i = 0; i < this._EG_RangeMarkupElementss.Count; i++)
			{
				string tagName = null;
				switch (this._EG_RangeMarkupElementss[i].GroupInterfaceType)
				{
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
				}
				this._EG_RangeMarkupElementss[i].Write(s, tagName);
			}
		}

		public void Write_EG_ContentRowContents(TextWriter s)
		{
			for (int i = 0; i < this._EG_ContentRowContents.Count; i++)
			{
				string tagName = null;
				switch (this._EG_ContentRowContents[i].GroupInterfaceType)
				{
				case GeneratedType.CT_Row:
					tagName = "tr";
					break;
				case GeneratedType.CT_CustomXmlRow:
					tagName = "customXml";
					break;
				case GeneratedType.CT_SdtRow:
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
				this._EG_ContentRowContents[i].Write(s, tagName);
			}
		}
	}
}
