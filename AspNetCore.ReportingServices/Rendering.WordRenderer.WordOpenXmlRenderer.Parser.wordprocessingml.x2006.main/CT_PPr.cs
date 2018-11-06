using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_PPr : CT_PPrBase, IOoxmlComplexType
	{
		private CT_SectPr _sectPr;

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

		public static string SectPrElementName
		{
			get
			{
				return "sectPr";
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
			base.WriteAttributes(s);
		}

		public override void WriteElements(TextWriter s)
		{
			base.WriteElements(s);
			this.Write_sectPr(s);
		}

		public void Write_sectPr(TextWriter s)
		{
			if (this._sectPr != null)
			{
				this._sectPr.Write(s, "sectPr");
			}
		}
	}
}
