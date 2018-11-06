using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Body : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_SectPr _sectPr;

		private List<IEG_BlockLevelElts> _EG_BlockLevelEltss;

		public CT_SectPr SectPr
		{
			get
			{
				return this._sectPr;
			}
			set
			{
				this._sectPr = value;
			}
		}

		public List<IEG_BlockLevelElts> EG_BlockLevelEltss
		{
			get
			{
				return this._EG_BlockLevelEltss;
			}
			set
			{
				this._EG_BlockLevelEltss = value;
			}
		}

		public static string SectPrElementName
		{
			get
			{
				return "sectPr";
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

		public static string PElementName
		{
			get
			{
				return "p";
			}
		}

		public static string TblElementName
		{
			get
			{
				return "tbl";
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

		public static string AltChunkElementName
		{
			get
			{
				return "altChunk";
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
			this._EG_BlockLevelEltss = new List<IEG_BlockLevelElts>();
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
			this.Write_EG_BlockLevelEltss(s);
			this.Write_sectPr(s);
		}

		public void Write_sectPr(TextWriter s)
		{
			if (this._sectPr != null)
			{
				this._sectPr.Write(s, "sectPr");
			}
		}

		public void Write_EG_BlockLevelEltss(TextWriter s)
		{
			for (int i = 0; i < this._EG_BlockLevelEltss.Count; i++)
			{
				string tagName = null;
				switch (this._EG_BlockLevelEltss[i].GroupInterfaceType)
				{
				case GeneratedType.CT_CustomXmlBlock:
					tagName = "customXml";
					break;
				case GeneratedType.CT_SdtBlock:
					tagName = "sdt";
					break;
				case GeneratedType.CT_P:
					tagName = "p";
					break;
				case GeneratedType.CT_Tbl:
					tagName = "tbl";
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
				case GeneratedType.CT_AltChunk:
					tagName = "altChunk";
					break;
				}
				this._EG_BlockLevelEltss[i].Write(s, tagName);
			}
		}
	}
}
