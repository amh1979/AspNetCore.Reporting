using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Hyperlink : OoxmlComplexType, IEG_PContent, IOoxmlComplexType
	{
		internal enum ChoiceBucket_0
		{
			customXml,
			smartTag,
			sdt,
			dir,
			bdo,
			r,
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
			fldSimple,
			hyperlink,
			subDoc
		}

		private string _id_attr;

		private CT_R _r;

		private CT_Hyperlink _hyperlink;

		private CT_Rel _subDoc;

		private ChoiceBucket_0 _choice_0;

		public override GeneratedType GroupInterfaceType
		{
			get
			{
				return GeneratedType.CT_Hyperlink;
			}
		}

		public string Id_Attr
		{
			get
			{
				return this._id_attr;
			}
			set
			{
				this._id_attr = value;
			}
		}

		public CT_R R
		{
			get
			{
				return this._r;
			}
			set
			{
				this._r = value;
			}
		}

		public CT_Hyperlink Hyperlink
		{
			get
			{
				return this._hyperlink;
			}
			set
			{
				this._hyperlink = value;
			}
		}

		public CT_Rel SubDoc
		{
			get
			{
				return this._subDoc;
			}
			set
			{
				this._subDoc = value;
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

		public static string RElementName
		{
			get
			{
				return "r";
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
			s.Write(" r:id=\"");
			OoxmlComplexType.WriteData(s, this._id_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
			this.Write_r(s);
			this.Write_hyperlink(s);
			this.Write_subDoc(s);
		}

		public void Write_r(TextWriter s)
		{
			if (this._choice_0 == ChoiceBucket_0.r && this._r != null)
			{
				this._r.Write(s, "r");
			}
		}

		public void Write_hyperlink(TextWriter s)
		{
			if (this._choice_0 == ChoiceBucket_0.hyperlink && this._hyperlink != null)
			{
				this._hyperlink.Write(s, "hyperlink");
			}
		}

		public void Write_subDoc(TextWriter s)
		{
			if (this._choice_0 == ChoiceBucket_0.subDoc && this._subDoc != null)
			{
				this._subDoc.Write(s, "subDoc");
			}
		}
	}
}
