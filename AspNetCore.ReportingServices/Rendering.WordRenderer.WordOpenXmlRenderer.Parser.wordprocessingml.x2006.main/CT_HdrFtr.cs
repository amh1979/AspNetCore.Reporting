using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_HdrFtr : OoxmlComplexType, IOoxmlComplexType
	{
		internal enum ChoiceBucket_0
		{
			customXml,
			sdt,
			p,
			tbl,
			proofErr,
			permStart,
			permEnd,
			bookmarkStart,
			bookmarkEnd,
			moveFromRangeStart,
			moveFromRangeEnd,
			moveToRangeStart,
			moveToRangeEnd,
			commentRangeStart,
			commentRangeEnd,
			customXmlInsRangeStart,
			customXmlInsRangeEnd,
			customXmlDelRangeStart,
			customXmlDelRangeEnd,
			customXmlMoveFromRangeStart,
			customXmlMoveFromRangeEnd,
			customXmlMoveToRangeStart,
			customXmlMoveToRangeEnd,
			ins,
			del,
			moveFrom,
			moveTo,
			oMathPara,
			oMath,
			altChunk
		}

		private List<CT_P> _p;

		private List<CT_Tbl> _tbl;

		private ChoiceBucket_0 _choice_0;

		public List<CT_P> P
		{
			get
			{
				return this._p;
			}
			set
			{
				this._p = value;
			}
		}

		public List<CT_Tbl> Tbl
		{
			get
			{
				return this._tbl;
			}
			set
			{
				this._tbl = value;
			}
		}

		public ChoiceBucket_0 Choice_0
		{
			get
			{
				return this._choice_0;
			}
			set
			{
				this._choice_0 = value;
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

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
			this._p = new List<CT_P>();
			this._tbl = new List<CT_Tbl>();
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
			this.Write_p(s);
			this.Write_tbl(s);
		}

		public void Write_p(TextWriter s)
		{
			if (this._choice_0 == ChoiceBucket_0.p && this._p != null)
			{
				foreach (CT_P item in this._p)
				{
					if (item != null)
					{
						item.Write(s, "p");
					}
				}
			}
		}

		public void Write_tbl(TextWriter s)
		{
			if (this._choice_0 == ChoiceBucket_0.tbl && this._tbl != null)
			{
				foreach (CT_Tbl item in this._tbl)
				{
					if (item != null)
					{
						item.Write(s, "tbl");
					}
				}
			}
		}
	}
}
