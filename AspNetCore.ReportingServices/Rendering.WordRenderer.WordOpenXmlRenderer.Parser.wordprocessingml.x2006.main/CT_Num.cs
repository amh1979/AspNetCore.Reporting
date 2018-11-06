using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Num : OoxmlComplexType, IOoxmlComplexType
	{
		private int _numId_attr;

		private CT_DecimalNumber _abstractNumId;

		public int NumId_Attr
		{
			get
			{
				return this._numId_attr;
			}
			set
			{
				this._numId_attr = value;
			}
		}

		public CT_DecimalNumber AbstractNumId
		{
			get
			{
				return this._abstractNumId;
			}
			set
			{
				this._abstractNumId = value;
			}
		}

		public static string AbstractNumIdElementName
		{
			get
			{
				return "abstractNumId";
			}
		}

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
			this._abstractNumId = new CT_DecimalNumber();
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
			s.Write(" w:numId=\"");
			OoxmlComplexType.WriteData(s, this._numId_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
			this.Write_abstractNumId(s);
		}

		public void Write_abstractNumId(TextWriter s)
		{
			if (this._abstractNumId != null)
			{
				this._abstractNumId.Write(s, "abstractNumId");
			}
		}
	}
}
